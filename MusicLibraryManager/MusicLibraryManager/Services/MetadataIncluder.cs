using ExtendCSharp;
using ExtendCSharp.ExtendedClass;
using ExtendCSharp.Services;
using MusicLibraryManager.GUI.Forms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static ExtendCSharp.Extension;
using ExtendCSharp.Interfaces;

namespace MusicLibraryManager.Services
{


    public class MetadataIncluderServices
    {
        public event Action OnEnd;
        public event IncorporaMetadataNodeStartProcessing OnNodeStartProcessing;
        public event IncorporaMetadataNodeProcessed OnNodeProcessed;
        public event ProgressChangedSingleMD5 OnProgressChangedSingleMD5;

        Thread Worker;
        bool Completed = false;
        bool _Pause = false;
        MutexObjectDispatcherServices<Nodo_pathsource> dispatcher;


        public bool Pause
        {
            get
            {
                MetadataIncluderAsyncStatus s = GetIncorporaMetadataStatus();
                if (s == MetadataIncluderAsyncStatus.Pause)
                    return true;
                else
                {
                    _Pause = false;
                    return false;
                } 
            }
            set
            {
                MetadataIncluderAsyncStatus s = GetIncorporaMetadataStatus();
                if (value && s== MetadataIncluderAsyncStatus.Running)
                    _Pause = true;
                else if(!value && s == MetadataIncluderAsyncStatus.Pause)
                    _Pause = false;
            }
        }
        public class PauseCatcher : ICatcher<bool>
        {
            MetadataIncluderServices s;
            public PauseCatcher(MetadataIncluderServices s)
            {
                this.s = s;
            }
            public bool value
            {
                get
                {
                    return s._Pause;
                }
            }
        }


        public MetadataIncluderServices()
        {
            dispatcher = new MutexObjectDispatcherServices<Nodo_pathsource>(MutexObjectDispatcherListType.fifo,new PauseCatcher(this));
        }

        public bool IncorporaMetadata(FileSystemNodePlus<MyAddittionalData> nodo, MyFileSystemPlus mfsp)
        {
            if (GetIncorporaMetadataStatus() == MetadataIncluderAsyncStatus.nul)
            {
                PauseCatcher pc = new PauseCatcher(this);
                Worker = new Thread(() =>
                {
					//TODO: l'incorpora metadata dovrebbe fuonzionare senza grafica, modificare i metodi affinchè i thread siano slegati dalla grafica
					
					
                    FormService fs = ServicesManager.Get<FormService>();
                    //Creo vari form per l'inclusione a dei metadati a thread (4)
                    int n_thread = 4;
                    int h = IncorporaMetatadaThread.height;
                    int y = 0;

                    FormsCloseCatcher FormChiusi = new FormsCloseCatcher();
                    for (int i = 0; i < n_thread; i++)
                    {
                        int temp = y;
                        fs.StartFormThread(() => { IncorporaMetatadaThread t= new IncorporaMetatadaThread(dispatcher, OnNodeProcessed, pc, temp); FormChiusi.AddForm(t);return t; });
                        y += h;
                    }


                    RiempiDispatcher(nodo, mfsp, dispatcher);

                    

                    while(dispatcher.Count()!=0)    //Aspetto che il dispatcher venga svuotato
                        Thread.Sleep(100);



                    dispatcher.End = true;

                    while (FormChiusi.TotAperti!= n_thread)    //Attendo che tutti i form vengano aperti ( sicurezza ) 
                        Thread.Sleep(100);


                    while (!FormChiusi)    //Attendo la chiusura di tutti form
                        Thread.Sleep(100);



                    OnEnd?.Invoke();
                });
                Worker.Start();
                Worker.Join();
                return GetReturnBool();
            }
            else
            {
                return false;
            }
        }
        public bool BeginIncorporaMetadata(FileSystemNodePlus<MyAddittionalData> nodo, MyFileSystemPlus mfsp)
        {
            if(GetIncorporaMetadataStatus()==MetadataIncluderAsyncStatus.nul)
            {
                Worker = new Thread(() =>
                {
                    IncorporaMetadataRicorsivo(nodo, mfsp);
                    OnEnd?.Invoke();
                });
                Worker.Start();
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool WaitIncorporaMetadata()
        {
            MetadataIncluderAsyncStatus s = GetIncorporaMetadataStatus();
            if (s == MetadataIncluderAsyncStatus.nul || s == MetadataIncluderAsyncStatus.Stop)
                return false;
            else if (s == MetadataIncluderAsyncStatus.Completed)
                return true;
            else
                Worker.Join();

            return GetReturnBool();
        }
        public MetadataIncluderAsyncStatus GetIncorporaMetadataStatus()
        {
            
            if (Worker == null)
                return MetadataIncluderAsyncStatus.nul;
           
            else if (Worker.ThreadState == ThreadState.Running || Worker.ThreadState == ThreadState.WaitSleepJoin)
                if (_Pause)
                    return MetadataIncluderAsyncStatus.Pause;
                else
                    return MetadataIncluderAsyncStatus.Running;
            
                
            else if (Worker.ThreadState == ThreadState.Stopped || Worker.ThreadState == ThreadState.StopRequested || Worker.ThreadState == ThreadState.Aborted || Worker.ThreadState == ThreadState.AbortRequested)
                return MetadataIncluderAsyncStatus.Stop;
            else if (Completed)
                return MetadataIncluderAsyncStatus.Completed;

            return MetadataIncluderAsyncStatus.nul;
        }
        public void AbortIncorporaMetadata()
        {
            try
            {
                Worker.Abort();
                dispatcher.End = true;
                _Pause = false;
            }catch (Exception e)
            {
                //throw e;
            }
        }

        private bool GetReturnBool()
        {
            MetadataIncluderAsyncStatus s = GetIncorporaMetadataStatus();
            if (s == MetadataIncluderAsyncStatus.Completed)
                return true;
            else
                return false;
        }


        private void IncorporaMetadataRicorsivo(FileSystemNodePlus<MyAddittionalData> nodo, MyFileSystemPlus mfsp)
        {
            foreach (FileSystemNodePlus<MyAddittionalData> n in nodo.GetAllNode())
            {
                while (_Pause)
                    Thread.Sleep(100);

                if (n.Type == FileSystemNodePlusType.Directory)
                    IncorporaMetadataRicorsivo(n, mfsp);
                else if (n.Type == FileSystemNodePlusType.File)
                {
                    IncorporaMetadataNodo(n, mfsp.GetFullPath(n));                    
                }
            }
        }

        private void RiempiDispatcher(FileSystemNodePlus<MyAddittionalData> nodo, MyFileSystemPlus mfsp, MutexObjectDispatcherServices<Nodo_pathsource> dispatcher)
        {
            foreach (FileSystemNodePlus<MyAddittionalData> n in nodo.GetAllNode())
            {
                /*while (_Pause)
                    Thread.Sleep(100);
                */
                if (n.Type == FileSystemNodePlusType.Directory)
                    RiempiDispatcher(n, mfsp,dispatcher);
                else if (n.Type == FileSystemNodePlusType.File)
                {
                    dispatcher.Add(new Nodo_pathsource() { nodo = n, Pathsource = mfsp.GetFullPath(n) });
                    //IncorporaMetadataNodo(n, mfsp.GetFullPath(n));                    
                }
            }
        }


        public void IncorporaMetadataNodo(FileSystemNodePlus<MyAddittionalData> nodo,String PathSource)
        {

            SystemService ss = ServicesManager.Get<SystemService>();
            if (ss.FileExist(PathSource))
            {
                OnNodeStartProcessing?.Invoke(nodo, PathSource);
                if (nodo.AddittionalData == null)
                    nodo.AddittionalData = new MyAddittionalData();

                if (nodo.AddittionalData.Metadata == null)
                    nodo.AddittionalData.Metadata = new FFmpegMetadata();

                FFmpeg fs = ServicesManager.Get<FFmpeg>();
                nodo.AddittionalData.Metadata = fs.GetMetadata(PathSource);
                nodo.AddittionalData.Size = ss.FileSize(PathSource);

                ss.GetMD5(PathSource, (double percent) =>
                {
                    OnProgressChangedSingleMD5?.Invoke(percent);
                }, (byte[] Hash) =>
                {
                    if (Hash != null)
                    {
                        nodo.AddittionalData.MD5 = Hash.ToHexString(true);
                        OnNodeProcessed?.Invoke(nodo, PathSource, MetadataIncluderError.nul);
                    }
                    else
                        OnNodeProcessed?.Invoke(nodo, PathSource, MetadataIncluderError.MD5Err);
                });

               
            }
            else
                OnNodeProcessed?.Invoke(nodo, PathSource, MetadataIncluderError.FileNonTrovato);
        }


    }

    public delegate void IncorporaMetadataNodeStartProcessing(FileSystemNodePlus<MyAddittionalData> nodo, String Path);
    public delegate void IncorporaMetadataNodeProcessed(FileSystemNodePlus<MyAddittionalData> nodo,String Path, MetadataIncluderError Err);
    public delegate void ProgressChangedSingleMD5(double percent);

    public enum MetadataIncluderError
    {
        nul,
        FileNonTrovato, 
        MD5Err
    }
    public enum MetadataIncluderAsyncStatus
    {
        Completed,
        Running,
        Stop,
        Pause,
        nul

    }


    public class Nodo_pathsource
    {
        public FileSystemNodePlus<MyAddittionalData> nodo;
        public String Pathsource;
    }



    public class FormsCloseCatcher :ICatcher<bool>
    {
        public int TotAperti = 0;
        int Open = 0;
        public void AddForm(Form f)
        {
            if (!f.IsDisposed)
            {
                Open++;
                TotAperti++;
                f.FormClosed += F_FormClosed;
            }
        }

        private void F_FormClosed(object sender, FormClosedEventArgs e)
        {
            Open--;
        }

        public override bool value
        {
            get
            {
                return Open == 0;
            }
        }
    }
}
