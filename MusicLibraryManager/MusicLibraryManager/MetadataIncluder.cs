using ExtendCSharp;
using ExtendCSharp.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static ExtendCSharp.Extension;

namespace MusicLibraryManager
{
    class MetadataIncluder
    {
        public event EndIncorporaMetadata OnEnd;
        public event IncorporaMetadataNodeStartProcessing OnNodeStartProcessing;
        public event IncorporaMetadataNodeProcessed OnNodeProcessed;
        public event ProgressChangedSingleMD5 OnProgressChangedSingleMD5;

        Thread Worker;
        bool Completed = false;
        bool _Pause = false;

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

        public bool IncorporaMetadata(FileSystemNodePlus<MyAddittionalData> nodo, MyFileSystemPlus mfsp)
        {
            if (GetIncorporaMetadataStatus() == MetadataIncluderAsyncStatus.nul)
            {
                Worker = new Thread(() =>
                {
                    IncorporaMetadataRicorsivo(nodo, mfsp);
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


        public void IncorporaMetadataNodo(FileSystemNodePlus<MyAddittionalData> nodo,String PathSource)
        {
           
            if (SystemService.FileExist(PathSource))
            {
                OnNodeStartProcessing?.Invoke(nodo, PathSource);
                if (nodo.AddittionalData == null)
                    nodo.AddittionalData = new MyAddittionalData();

                if (nodo.AddittionalData.Metadata == null)
                    nodo.AddittionalData.Metadata = new FFmpegMetadata();

                nodo.AddittionalData.Metadata = FFmpeg.GetMetadata(PathSource);
                nodo.AddittionalData.Size = SystemService.FileSize(PathSource);

                SystemService.GetMD5(PathSource, (double percent) =>
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

    public delegate void EndIncorporaMetadata();
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



}
