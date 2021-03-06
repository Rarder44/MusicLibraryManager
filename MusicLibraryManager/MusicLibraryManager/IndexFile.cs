﻿using ExtendCSharp;
using ExtendCSharp.ExtendedClass;
using ExtendCSharp.Services;
using MusicLibraryManager.GUI.Forms;
using MusicLibraryManager.Services;
using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static ExtendCSharp.Extension;

namespace MusicLibraryManager
{

    public class IndexFile
    {
        #region Eventi

        public event Action<IndexFile> OnEnd;
        public event Action OnEndMetadata;
        public event IncorporaMetadataNodeStartProcessing OnNodeStartProcessing;
        public event IncorporaMetadataNodeProcessed OnProcessedMetadata;
        public event MD5BlockTransformEventHandler OnPercentChangeMetadata;

        #endregion

        #region Variabili

        [JsonProperty]
        public MyFileSystemPlus RootFileSystem;
        [JsonProperty]
        public FileSystemPlusLoadOption LoadOption;

        
        [JsonIgnore]
        Thread WorkerThread;

        [JsonIgnore]
        Thread GUIThread;




        [JsonIgnore]
        bool Completed = false;

        [JsonIgnore]
        bool _Pause = false;
        [JsonIgnore]
        public bool Pause
        {
            get
            {
                IndexFileAsyncStatus s = GetIndexFileStatus();
                if (s == IndexFileAsyncStatus.Pause)
                    return true;
                else
                {
                    _Pause = false;
                    return false;
                }
            }
            set
            {
                IndexFileAsyncStatus s = GetIndexFileStatus();
                if (value && s == IndexFileAsyncStatus.Running)
                    _Pause = true;
                else if (!value && s == IndexFileAsyncStatus.Pause)
                    _Pause = false;
            }
        }

        [JsonIgnore]
        private bool _GUI=false;
        [JsonIgnore]
        public bool GUI
        {
            get
            {
                return _GUI;
            }
            set
            {
                _GUI = value;
                if (!value)
                {
                    CloseGui();
                }

                
            }
        }
        [JsonIgnore]
        private bool CanUseGui
        {
            get
            {
                return GUI && IFForm != null && IFForm.Status == FormStatus.Open;
            }
        }

        [JsonIgnore]
        IndexFileForm IFForm = null;

        #endregion


        public IndexFile()
        {
            this.RootFileSystem = null;
            this.GUI = true;
        }
        public IndexFile(bool GUI=true)
        {
            this.RootFileSystem = null;
            this.GUI = GUI;
        }
        public IndexFile(MyFileSystemPlus ListElement, bool GUI = true)
        {
            this.RootFileSystem = ListElement;
            this.GUI = GUI;
        }

        public bool LoadFromPath(String Path, FileSystemPlusLoadOption lo)
        {
            if (GetIndexFileStatus() == IndexFileAsyncStatus.nul)
            {
                WorkerThread = new Thread(() =>
                {
                    _CreateIndexFile(Path, lo);
                    if (CanUseGui)
                        WaitGUIClose();

                    OnEnd?.Invoke(this);
                });
                WorkerThread.Start();
                WorkerThread.Join();
                return GetReturnBoolLoadFromPath();
            }
            else
            {
                return false;
            }
        }

        public bool BeginLoadFromPath(String Path, FileSystemPlusLoadOption lo)
        {
            if (GetIndexFileStatus() == IndexFileAsyncStatus.nul)
            {
                WorkerThread = new Thread(() =>
                {
                    _CreateIndexFile(Path, lo);
                    if (CanUseGui)
                        WaitGUIClose();
                        
                    OnEnd?.Invoke(this);
                });
                WorkerThread.Start();
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool WaitLoadFromPath()
        {
            IndexFileAsyncStatus s = GetIndexFileStatus();
            if (s == IndexFileAsyncStatus.nul || s == IndexFileAsyncStatus.Stop)
                return false;
            else if (s == IndexFileAsyncStatus.Completed)
                return true;
            else
                WorkerThread.Join();

            return GetReturnBoolLoadFromPath();
        }
        public void AbortLoadFromPath()
        {
            try
            {
                WorkerThread.Abort();
                _Pause = false;
            }
            catch (Exception e)
            {
                //throw e;
            }
        }
        private bool GetReturnBoolLoadFromPath()
        {
            IndexFileAsyncStatus s = GetIndexFileStatus();
            if (s == IndexFileAsyncStatus.Completed)
                return true;
            else
                return false;
        }


        public IndexFileAsyncStatus GetIndexFileStatus()
        {
            if (WorkerThread == null)
                return IndexFileAsyncStatus.nul;

            else if (WorkerThread.ThreadState == ThreadState.Running)
                if (_Pause)
                    return IndexFileAsyncStatus.Pause;
                else
                    return IndexFileAsyncStatus.Running;


            else if (WorkerThread.ThreadState == ThreadState.Stopped || WorkerThread.ThreadState == ThreadState.StopRequested || WorkerThread.ThreadState == ThreadState.Aborted || WorkerThread.ThreadState == ThreadState.AbortRequested)
                return IndexFileAsyncStatus.Stop;
            else if (Completed)
                return IndexFileAsyncStatus.Completed;

            return IndexFileAsyncStatus.nul;
        }

        private void _IncorporaMetadata(MyFileSystemPlus t)
        {
            MetadataIncluderServices MI = new MetadataIncluderServices();
            MI.OnNodeStartProcessing += OnNodeStartProcessing;
            MI.OnNodeProcessed += OnProcessedMetadata;
            MI.OnProgressChangedSingleMD5 += (double percent) =>
            {
                OnPercentChangeMetadata?.Invoke(percent);
            };
            MI.OnEnd += OnEndMetadata;

            
            if (CanUseGui)
            {
                IFForm.FormClosed += (object sender, System.Windows.Forms.FormClosedEventArgs e) =>
                {
                    MI.AbortIncorporaMetadata();
                };

                int Total = t.Root.GetNodeCount(FileSystemNodePlusLevelType.AllNode, FileSystemNodePlusType.File);
                IFForm.SetProgressTotalMax(Total);
                double buffer = 0;

                MI.OnNodeStartProcessing += (FileSystemNodePlus<MyAddittionalData> nodo, string PathNodo) =>
                {
                    buffer = 0;
                    if (CanUseGui)
                        IFForm.SetSource(PathNodo);
                };
                MI.OnNodeProcessed += (FileSystemNodePlus<MyAddittionalData> nodo, string PathNodo, MetadataIncluderError Err) =>
                {
                    buffer = 100;

                    if (CanUseGui)
                    {
                        IFForm.SetProgressSingleValue((int)buffer);
                        IFForm.SetProgressTotalValue(IFForm.GetProgressTotalValue() + 1);
                        if (Err == MetadataIncluderError.nul)
                        {
                            IFForm.AddAggiunti(nodo.GetFullPath());
                        }
                        else if (Err == MetadataIncluderError.MD5Err)
                        {
                            IFForm.AddProblemi("MD5 error - " + nodo.GetFullPath());
                        }
                        else if (Err == MetadataIncluderError.FileNonTrovato)
                        {
                            IFForm.AddProblemi("Not Found - " + nodo.GetFullPath());
                        }
                        else
                        {
                            throw new Exception("Errore non implementato");
                        }
                    }

                };
                MI.OnEnd += () =>
                {
                    IFForm.SetProgressTotalValue(IFForm.GetProgressTotalValue() + 1);
                    IFForm.Fine();
                };


                MI.OnProgressChangedSingleMD5 += (double AddPercent) =>
                {
                    buffer += AddPercent;
                    if (CanUseGui)
                        IFForm.SetProgressSingleValue((int)buffer);
                };
            }

            MI.IncorporaMetadata(t.Root, t);
            t.Root.Remove((x) => 
            {
                return x.Type == FileSystemNodePlusType.File && (x.AddittionalData.MD5 == "" || x.AddittionalData.MD5 == null);
            }, FileSystemNodePlusLevelType.AllNode, FileSystemNodePlusControlType.Post);
            

        }

        private void _CreateIndexFile(String Path, FileSystemPlusLoadOption lo)
        {
            if (GUI)
                StartGui();

            if (CanUseGui)
                IFForm.SetMessage("Lettura media...");

            MyFileSystemPlus t = new MyFileSystemPlus(Path, lo);

            if (CanUseGui)
                IFForm.SetMessage("Inclusione Metadata...");



            _IncorporaMetadata(t);



            if (CanUseGui)
                IFForm.SetMessage("Finalizzazione Index File");

            RootFileSystem = t;
            RootFileSystem.Root.SetParentOnAllChild(FileSystemNodePlusLevelType.AllNode);
            Completed = true;
            if (CanUseGui)
                IFForm.SetMessage("Fine!");
        }

       

        public void WaitGUIClose()
        {
            if(CanUseGui)
                GUIThread.Join();
        }
        public bool StartGui()
        {
            if (IFForm == null && (GUIThread==null || GUIThread.ThreadState == ThreadState.Stopped || GUIThread.ThreadState == ThreadState.StopRequested || GUIThread.ThreadState == ThreadState.Aborted || GUIThread.ThreadState == ThreadState.AbortRequested))
            {
                IFForm = new IndexFileForm();
                GUIThread = new Thread(() =>
                 {
                    
                     IFForm.ShowDialog();
                     IFForm.CloseInvoke();
                     IFForm = null;
                 });
                GUIThread.Start();
                return true;
            }
            else
            {
                return false;
            }
        }

       

        public void CloseGui()
        {
            if (IFForm != null && GUIThread != null && GUIThread.ThreadState == ThreadState.Running)
            {
                IFForm.CloseInvoke();
                GUIThread.Abort();
                IFForm = null;
            } 
        }





        /// <summary>
        /// Controlla l'IndexFile corrente con la Path memorizzata nell'IndexFile; rimuove i file non trovati ed aggiunge i nuovi file.
        /// </summary>
        public void Update()
        {
            if (GUI)
                StartGui();

            if (CanUseGui)
                IFForm.SetMessage("Lettura media...");

            if (RootFileSystem == null)
                return;
            //controllo se esistono, su disco, tutti i file presenti nel mio FileSystem
            // quelli che non esistono li cancello
            // mentre sono in una cartella controllo anche se ci sono nuovi file da aggiungere
            MyFileSystemPlus t = new MyFileSystemPlus();
            t.RootPath = RootFileSystem.RootPath;

            t.Root = new FileSystemNodePlus<MyAddittionalData>(RootFileSystem.RootPath.TrimEnd('\\', '/').SplitAndGetLast("\\", "/"), FileSystemNodePlusType.Directory);
            
            
            // incorporo i metadati in t

            RootFileSystem.DeselectAll();
            UpdateRecursiveRealToList(RootFileSystem, RootFileSystem.RootPath, t.Root);
            RootFileSystem.Root.SetParentOnAllChild(FileSystemNodePlusLevelType.AllNode);
            if (CanUseGui)
                IFForm.AddRimossi(RootFileSystem.FindAll((x)=> { return x.Type == FileSystemNodePlusType.File && !x.AddittionalData.Selezionato; }).Select((x)=> { return x.GetFullPath(); }).ToArray());

            if (CanUseGui)
                IFForm.SetMessage("Rimozione file non trovati...");

            //cancello tutti i file non trovati o modificati ( i modificati sono anche stati aggiunti a t )
            RootFileSystem.RimuoviFileNonSelezionati();
            RootFileSystem.RimuoviCartelleVuote();
            RootFileSystem.DeselectAll();


            
            if (CanUseGui)
                IFForm.SetMessage("Inclusione Metadata...");

            _IncorporaMetadata(t);


            if (CanUseGui)
                IFForm.SetMessage("Merge dei nuovi dati...");
            RootFileSystem.Merge(t);

            if (CanUseGui)
                IFForm.SetMessage("Finalizzazione Index File");


            RootFileSystem.Root.SetParentOnAllChild(FileSystemNodePlusLevelType.AllNode);

            if (CanUseGui)
                IFForm.SetMessage("Fine!");

            if (CanUseGui)
                WaitGUIClose();

        }

        /// <summary>
        /// Controlla l'indice corrente con la Path specificata; rimuove i file non trovati ed aggiunge i nuovi file.
        /// </summary>
        /// <param name="RootPath">Path della cartella su cui eseguire il controllo </param>
        public void Update(String RootPath)
        {
            if (RootFileSystem == null)
                RootFileSystem = new MyFileSystemPlus();

            RootFileSystem.RootPath = RootPath;
            Update();
        }


        /// <summary>
        /// Scorre tutte le cartelle presenti nella cartella Reale e controlla i file:
        /// se un file viente trovato nella cartella e nel FileSystem allora viene impostato il flag Selezionato a true
        /// se un file non viene trovato nel FileSystem, viene aggiunto al FileSystem Temp
        /// 
        /// </summary>
        /// <param name="FileSystem"></param>
        /// <param name="CurrentPath">Path da controllare</param>
        /// <param name="Temp"></param>
        private void UpdateRecursiveRealToList(MyFileSystemPlus FileSystem, String CurrentPath, FileSystemNodePlus<MyAddittionalData> Temp)
        {
            //scorro tutte le cartelle
            //imposto selezionato a tutti i file che trovo 
            //alla fine rimuovo i file non selezionati
            SystemService ss = ServicesManager.Get<SystemService>();
            String[] tfolder = ss.GetDirectories(CurrentPath);
            foreach (String s in tfolder)
            { 
                UpdateRecursiveRealToList(FileSystem, s, Temp.CreateNode(s.TrimEnd('\\', '/').SplitAndGetLast("\\", "/"), FileSystemNodePlusType.Directory));
            }

            String[] tfile = ss.GetFiles(CurrentPath);
            foreach (String s in tfile)
            {
                if (LoadOption == null || (LoadOption != null && (!LoadOption.RestrictExtensionEnable || LoadOption.RestrictExtension.Contains(System.IO.Path.GetExtension(s).TrimStart('.').ToLower()))))
                {
                    FileSystemNodePlus<MyAddittionalData> n = FileSystem.GetNodeFromPath(s);
                    if (n == null)
                    {
                        //se il file non viene trovato vuol dire che è nuovo e lo aggiungo al temp
                        Temp.CreateNode(s.TrimEnd('\\', '/').SplitAndGetLast("\\", "/"), FileSystemNodePlusType.File);
                    }
                    else if (n.AddittionalData.Size == ss.FileSize(s) && n.AddittionalData.MD5!=null && n.Type == (ss.FileExist(s)?FileSystemNodePlusType.File:FileSystemNodePlusType.Directory))
                    {
                        // il file non è stato modificato ( dimensine ) 
                        // ha l'md5 
                        // che il tipo corrisponda ( per errori di vecchi Index
                        // lo seleziono ( vuol dire che lo tengo ) .
                        n.AddittionalData.Selezionato = true;
                    }
                    else // nel caso in cui il file è stato modificato
                    {
                        //reinserisco il file all'interno di temp in modo che venga rianalizzato
                        Temp.CreateNode(s.TrimEnd('\\', '/').SplitAndGetLast("\\", "/"), FileSystemNodePlusType.File);
                    }
                }
            }
        }



        public enum IndexFileError
        {
            nul,
            FileNonTrovato
        }
        public enum IndexFileAsyncStatus
        {
            Completed,
            Running,
            Stop,
            Pause,
            nul

        }

    }
}
