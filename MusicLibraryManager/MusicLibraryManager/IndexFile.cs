using ExtendCSharp;
using ExtendCSharp.Services;
using MusicLibraryManager.GUI.Forms;

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
    public delegate void EndIndexFile(IndexFile i);

    public class IndexFile
    {
        [JsonProperty]
        public MyFileSystemPlus RootFileSystem;
        [JsonProperty]
        public FileSystemPlusLoadOption LoadOption;

        public event EndIndexFile OnEnd;
        public event EndIncorporaMetadata OnEndMetadata;
        public event IncorporaMetadataNodeStartProcessing OnNodeStartProcessing;
        public event IncorporaMetadataNodeProcessed OnProcessedMetadata;
        public event MD5BlockTransformEventHandler OnPercentChangeMetadata;


        [JsonIgnore]
        Thread Worker;
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


        public IndexFile()
        {
            this.RootFileSystem = null; 
        }
        public IndexFile(MyFileSystemPlus ListElement)
        {
            this.RootFileSystem = ListElement;
        }

        public bool LoadFromPath(String Path, FileSystemPlusLoadOption lo, bool GUI)
        {
            if (GetIndexFileStatus() == IndexFileAsyncStatus.nul)
            {
                Worker = new Thread(() =>
                {
                    _CreateIndexFile(Path, lo, GUI);
                    OnEnd?.Invoke(this);
                });
                Worker.Start();
                Worker.Join();
                return GetReturnBoolLoadFromPath();
            }
            else
            {
                return false;
            }
        }

        public bool BeginLoadFromPath(String Path, FileSystemPlusLoadOption lo, bool GUI)
        {
            if (GetIndexFileStatus() == IndexFileAsyncStatus.nul)
            {
                Worker = new Thread(() =>
                {
                    _CreateIndexFile(Path, lo, GUI);
                    OnEnd?.Invoke(this);
                });
                Worker.Start();
                return true;
            }
            else
            {
                return false;
            }
        }
        public IndexFileAsyncStatus GetIndexFileStatus()
        {
            if (Worker == null)
                return IndexFileAsyncStatus.nul;

            else if (Worker.ThreadState == ThreadState.Running)
                if (_Pause)
                    return IndexFileAsyncStatus.Pause;
                else
                    return IndexFileAsyncStatus.Running;


            else if (Worker.ThreadState == ThreadState.Stopped || Worker.ThreadState == ThreadState.StopRequested || Worker.ThreadState == ThreadState.Aborted || Worker.ThreadState == ThreadState.AbortRequested)
                return IndexFileAsyncStatus.Stop;
            else if (Completed)
                return IndexFileAsyncStatus.Completed;

            return IndexFileAsyncStatus.nul;
        }
        public bool WaitLoadFromPath()
        {
            IndexFileAsyncStatus s = GetIndexFileStatus();
            if (s == IndexFileAsyncStatus.nul || s == IndexFileAsyncStatus.Stop)
                return false;
            else if (s == IndexFileAsyncStatus.Completed)
                return true;
            else
                Worker.Join();

            return GetReturnBoolLoadFromPath();
        }
        public void AbortLoadFromPath()
        {
            try
            {
                Worker.Abort();
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



        private void _CreateIndexFile(String Path, FileSystemPlusLoadOption lo, bool gui)
        {
            MyFileSystemPlus t = new MyFileSystemPlus(Path, lo);
            LoadOption = lo;
            if (gui)
            {
                IncorporaMetadata IM = new IncorporaMetadata(t);
                IM.OnNodeStartProcessing += OnNodeStartProcessing;
                IM.OnProcessedMetadata += OnProcessedMetadata;

                IM.OnPercentChangeMetadata += OnPercentChangeMetadata;

                IM.OnEndMetadata += OnEndMetadata;
                IM.OnIncorporaMetadataFormEnd += (MyFileSystemPlus mfsp, FileSystemNodePlus<MyAddittionalData> AlternativeNode, IncorporaMetadataFormResult Result) =>
                {
                   
                };

                IM.ShowDialog();
            }
            else
            {
                MetadataIncluder MI = new MetadataIncluder();
                MI.OnNodeStartProcessing += OnNodeStartProcessing;
                MI.OnNodeProcessed += OnProcessedMetadata;

                MI.OnProgressChangedSingleMD5 += (double percent) =>
                {
                    OnPercentChangeMetadata?.Invoke(percent);
                };
                MI.OnEnd += OnEndMetadata;

                MI.IncorporaMetadata(t.Root, t);
            }

            RootFileSystem = t;
            RootFileSystem.Root.SetParentOnAllChild(FileSystemNodePlusLevelType.AllNode);
            Completed = true;
        }












        /// <summary>
        /// Controlla l'IndexFile corrente con la Path memorizzata nell'IndexFile; rimuove i file non trovati ed aggiunge i nuovi file.
        /// </summary>
        public void Update()
        {
            if (RootFileSystem == null)
                return;
            //controllo se esistono, su disco, tutti i file presenti nel mio FileSystem
            // quelli che non esistono li cancello
            // mentre sono in una cartella controllo anche se ci sono nuovi file da aggiungere
            MyFileSystemPlus t = new MyFileSystemPlus();
            t.RootPath = RootFileSystem.RootPath;

            t.Root = new FileSystemNodePlus<MyAddittionalData>(RootFileSystem.RootPath.TrimEnd('\\', '/').SplitAndGetLast("\\", "/"), FileSystemNodePlusType.Directory);
            //UpdateRecursiveListToReal(ListElement, ListElement.Root, t.Root);



            // incorporo i metadati in t

            RootFileSystem.DeselectAll();
            UpdateRecursiveRealToList(RootFileSystem, RootFileSystem.RootPath, t.Root);
            RootFileSystem.RimuoviFileNonSelezionati();
            RootFileSystem.RimuoviCartelleVuote();
            RootFileSystem.DeselectAll();

            IncorporaMetadata IM = new IncorporaMetadata(t);
            IM.Text = "Update Index Media File"; 
            IM.ShowDialog();
            RootFileSystem.Merge(t);

            RootFileSystem.Root.SetParentOnAllChild(FileSystemNodePlusLevelType.AllNode);
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
        /// DECPRECATO!
        /// scorre tutti i file del FileSystem e controlla se sono presenti nella cartella reale
        /// </summary>
        /// <param name="FileSystem"></param>
        /// <param name="Folder"></param>
        /// <param name="Temp"></param>
        private void UpdateRecursiveListToReal(MyFileSystemPlus FileSystem, FileSystemNodePlus<MyAddittionalData> Folder, FileSystemNodePlus<MyAddittionalData> Temp)
        {
            FileSystemNodePlus<MyAddittionalData>[] tfolder=Folder.GetAllNode(FileSystemNodePlusType.Directory);
            foreach (FileSystemNodePlus<MyAddittionalData> n in tfolder)
            {
                UpdateRecursiveListToReal(FileSystem, n, Temp.CreateNode(n.Name, n.Type));
            }

            FileSystemNodePlus<MyAddittionalData>[] tfile = Folder.GetAllNode(FileSystemNodePlusType.File);
            foreach (FileSystemNodePlus<MyAddittionalData> n in tfile)
            {
                String p = FileSystem.GetFullPath(n);
                //se il file non esiste o le dimensioni non corrispondono 
                if(!SystemService.FileExist(p))
                {
                    // il file non esiste piu
                    // cancello il file corrente
                    Folder.Remove((Nodo) => { return Nodo == n; }, FileSystemNodePlusLevelType.FirstLevel, FileSystemNodePlusControlType.Pre);
                }
                else if (n.AddittionalData.Size == SystemService.FileSize(p))
                {
                    // il file è stato modificato
                    // cancello il file corrente
                    Folder.Remove((Nodo) => { return Nodo == n; }, FileSystemNodePlusLevelType.FirstLevel, FileSystemNodePlusControlType.Pre);
                    Temp.CreateNode(n.Name, FileSystemNodePlusType.File);
                }
            }
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
            String[] tfolder = SystemService.GetDirectories(CurrentPath);
            foreach (String s in tfolder)
            { 
                UpdateRecursiveRealToList(FileSystem, s, Temp.CreateNode(s.TrimEnd('\\', '/').SplitAndGetLast("\\", "/"), FileSystemNodePlusType.Directory));
            }

            String[] tfile = SystemService.GetFiles(CurrentPath);
            foreach (String s in tfile)
            {
                if (LoadOption == null || (LoadOption != null && (!LoadOption.RestrictExtensionEnable || LoadOption.RestrictExtension.Contains(System.IO.Path.GetExtension(s).TrimStart('.').ToLower()))))
                {
                    FileSystemNodePlus<MyAddittionalData> n = FileSystem.GetNodeFromPath(s);
                    if (n == null)
                    {
                        //file nuovo
                        Temp.CreateNode(s.TrimEnd('\\', '/').SplitAndGetLast("\\", "/"), FileSystemNodePlusType.File);
                    }
                    else if (n.AddittionalData.Size == SystemService.FileSize(s))
                    {
                        // il file non è stato modificato e lo seleziono.
                        n.AddittionalData.Selezionato = true;
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
