using ExtendCSharp;
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
    public delegate void EndIndexFile();

    public class IndexFile
    {
        [JsonProperty]
        public MyFileSystemPlus ListElement;


        public event EndIndexFile OnEnd;
        public event EndIncorporaMetadata OnEndMetadata;
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
            this.ListElement = null; 
        }
        public IndexFile(MyFileSystemPlus ListElement)
        {
            this.ListElement = ListElement;
        }

        public bool LoadFromPath(String Path, FileSystemPlusLoadOption lo, bool GUI)
        {
            if (GetIndexFileStatus() == IndexFileAsyncStatus.nul)
            {
                Worker = new Thread(() =>
                {
                    _CreateIndexFile(Path, lo, GUI);
                    OnEnd?.Invoke();
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



        private static IndexFile _CreateIndexFile(String Path, FileSystemPlusLoadOption lo, bool gui)
        {
            MyFileSystemPlus t = new MyFileSystemPlus(Path, lo);

            if (gui)
            {
                IncorporaMetadata IM = new IncorporaMetadata(t);
                IM.ShowDialog();
            }
            else
            {
                MetadataIncluder MI = new MetadataIncluder();
                MI.IncorporaMetadata(t.Root, t);
            }

            return new IndexFile(t);
        }








        /// <summary>
        /// Controlla l'indice corrente con la Path specificata; rimuove i file non trovati ed aggiunge i nuovi file.
        /// </summary>
        /// <param name="Path">Path della cartella su cui eseguire il controllo </param>
        public void Update(String Path)
        {

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
