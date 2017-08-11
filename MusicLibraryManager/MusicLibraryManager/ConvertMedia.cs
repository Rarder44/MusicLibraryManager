using ExtendCSharp;
using ExtendCSharp.ExtendedClass;
using ExtendCSharp.Services;
using MusicLibraryManager.GUI.Forms;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MusicLibraryManager
{
    public delegate void OnStartConvertingEventHandler(String Source, String Destination);
    public delegate void OnEndConvertingEventHandler(String Source, String Destination, ConvertMediaError err);
    public delegate void OnProgressConvertingEventHandler(String Source, String Destination, double percent);


    public class ConversionParameter
    {
        public ConversionParameter(ConversinType TipoConversione, FFMpegMediaMetadata ConvertiIn, bool OverrideIfExist = true)
        {
            this.OverrideIfExist = OverrideIfExist;
            this.TipoConversione = TipoConversione;
            this.ConvertiIn = ConvertiIn;
        }
        public bool OverrideIfExist { get; set; }
        public ConversinType TipoConversione { get; set; }
        public FFMpegMediaMetadata ConvertiIn { get; set; }
    }


    class ConvertMedia
    {
        #region Eventi

        public event Action OnEnd;
        public event OnStartConvertingEventHandler OnStartConverting;
        public event OnEndConvertingEventHandler OnEndConverting;
        public event OnProgressConvertingEventHandler OnProgressConverting;

        #endregion

        #region Variabili

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
                ConvertMediaAsyncStatus s = GetConvertMediaStatus();
                if (s == ConvertMediaAsyncStatus.Pause)
                    return true;
                else
                {
                    _Pause = false;
                    return false;
                }
            }
            set
            {
                ConvertMediaAsyncStatus s = GetConvertMediaStatus();
                if (value && s == ConvertMediaAsyncStatus.Running)
                {
                    _Pause = true;     
                }
                else if (!value && s == ConvertMediaAsyncStatus.Pause)
                {
                    _Pause = false;
                }
            }
        }

        [JsonIgnore]
        private bool _GUI = false;
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
                return GUI && CMForm != null && CMForm.Status == FormStatus.Open;
            }
        }




        [JsonIgnore]
        ConvertMediaForm CMForm = null;

        #endregion


        public ConvertMedia()
        {
            this.GUI = true;
        }
        public ConvertMedia(bool GUI = true)
        {
            this.GUI = GUI;
        }

        /// <summary>
        /// Permette di convertire una Playlist partendo dai file sorgenti specificati nel file index e come destinazione ha la DestFolder
        /// Viene usato il ConversionParameter per specificare come convertire i media
        /// </summary>
        /// <param name="p">Playlist da convertire</param>
        /// <param name="IndexFileSorgente"> IndexFile dei media sorgenti</param>
        /// <param name="DestFolder"> Cartella di destinazione dei media</param>
        /// <param name="cp">Parametri di conversione</param>
        public void ConvertPlaylist(Playlist p, IndexFile IndexFileSorgente, String DestFolder, ConversionParameter cp)
        {
            ConvertPlaylist(p.FileSystem.Flatten(), IndexFileSorgente, DestFolder, cp);
        }
        /// <summary>
        /// Permette di convertire una Lista di Nodi partendo dai file sorgenti specificati nel file index e come destinazione ha la DestFolder
        /// Viene usato il ConversionParameter per specificare come convertire i media
        /// </summary>
        /// <param name="ListaNodi">Lista dei Nodi da convertire</param>
        /// <param name="IndexFileSorgente"> IndexFile dei media sorgenti</param>
        /// <param name="DestFolder"> Cartella di destinazione dei media</param>
        /// <param name="cp">Parametri di conversione</param>
        public void ConvertPlaylist(IEnumerable<FileSystemNodePlus<MyAddittionalData>> ListaNodi, IndexFile IndexFileSorgente, String DestFolder, ConversionParameter cp)
        {
            SystemService ss = ServicesManager.Get<SystemService>();
            if (GUI)
                StartGui();

            //seleziono effettivamente tutti i file
            IEnumerable<FileSystemNodePlus<MyAddittionalData>> i = ListaNodi.Where(x => x.Type == FileSystemNodePlusType.File);


            bool Abort = false;
            
            if (CanUseGui)
            {
                CMForm.SetPause += (bool pau) => {
                    Pause = pau;
                };
                CMForm.Stop += () =>
                {
                    Abort = true;
                };

                CMForm.SetProgressTotalMax(i.Count());
            }


            foreach (FileSystemNodePlus<MyAddittionalData> n in i)
            {
                while (Pause && !Abort)
                    Thread.Sleep(100);
                if (Abort)
                    break;

                FileSystemNodePlus<MyAddittionalData> tt = IndexFileSorgente.RootFileSystem.FindFirst((x) => { return x.AddittionalData.MD5 == n.AddittionalData.MD5; });
                if (tt == null)
                {
                    //TODO: non trovato -> gestire la ricerca dei file non trovati
                }
                else
                {
                   



                    if (cp.TipoConversione == ConversinType.Mai)
                    {
                        String PathSource = IndexFileSorgente.RootFileSystem.GetFullPath(tt);
                        String PathDestination = ss.CombinePaths(DestFolder, (n.GetFullPath().TrimStart('\\', '/')));

                        if (CanUseGui)
                            CMForm.StartConvertItem(PathSource, PathDestination);
                        OnStartConverting?.Invoke(PathSource, PathDestination);
                        ss.CopySecure(PathSource, PathDestination, cp.OverrideIfExist, (double Percent, ref bool cancelFlag) =>
                        {
                            if (CanUseGui)
                                CMForm.UpdateProgressPartial((int)Percent);
                            OnProgressConverting?.Invoke(PathSource, PathDestination, Percent);
                        },
                        (bool copiato, Exception ex) =>
                        {
                            OnEndConverting?.Invoke(PathSource, PathDestination, copiato ? ConvertMediaError.nul : ConvertMediaError.ErrDuranteLaCopia);
                        });
                    }
                    else
                    {

                        String PathSource = IndexFileSorgente.RootFileSystem.GetFullPath(tt);
                        String PathDestination = ss.CombinePaths(DestFolder, ss.ChangeExtension(n.GetFullPath().TrimStart('\\', '/'), cp.ConvertiIn.GetDefaultExtension()));



                        ConvertionEntity Source = new ConvertionEntity(PathSource, tt.AddittionalData.Metadata.MediaMetadata);
                        ConvertionEntity Dest = new ConvertionEntity(PathDestination, cp.ConvertiIn);


                        bool ForceConversion = cp.TipoConversione == ConversinType.Sempre ? true : false;
                        bool Err = false;
                        FFmpeg fs = ServicesManager.Get<FFmpeg>();
                        fs.ConvertTo(Source, Dest, ForceConversion, cp.OverrideIfExist, (st, src, des) =>
                        {
                            if (st == FFmpegStatus.Running)
                            {
                                if (CanUseGui)
                                    CMForm.StartConvertItem(src, des);
                                OnStartConverting?.Invoke(src, des);
                            }

                        }, (Percent, src, Destination, Error) =>
                        {
                            if (Error == FFmpegError.DestFolderNotFound)
                            {
                                Err = true;
                                if (CanUseGui)
                                    CMForm.AppendLog("[ERR] File non trovato " + src);
                                OnEndConverting?.Invoke(src, Destination, ConvertMediaError.FileNonTrovato);
                            }
                            else
                            {
                                if (CanUseGui)
                                    CMForm.UpdateProgressPartial(Percent);
                                OnProgressConverting?.Invoke(src, Destination, Percent);
                            }
                        }, false);

                        if (!Err)
                            OnEndConverting?.Invoke(PathSource, PathDestination, ConvertMediaError.nul);
                    }
                }
            }
            if (CanUseGui)
                CMForm.Finito();
            
        }


        /// <summary>
        /// Permette di convertire una Playlist partendo dai file sorgenti specificati nel file index e come destinazione ha la DestFolder
        /// Viene usato il ConversionParameter per specificare come convertire i media
        /// </summary>
        /// <param name="p">Playlist da convertire</param>
        /// <param name="IndexFileSorgente"> IndexFile dei media sorgenti</param>
        /// <param name="DestFolder"> Cartella di destinazione dei media</param>
        /// <param name="cp">Parametri di conversione</param>
        public bool BeginConvertPlaylist(Playlist p, IndexFile IndexFileSorgente, String DestFolder, ConversionParameter cp)
        {
            if (GetConvertMediaStatus() == ConvertMediaAsyncStatus.nul)
            {
                WorkerThread = new Thread(() =>
                {
                    ConvertPlaylist(p, IndexFileSorgente, DestFolder, cp);
                    if (CanUseGui)
                        WaitGUIClose();

                    OnEnd?.Invoke();
                });
                WorkerThread.Start();
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// Permette di convertire una Lista di Nodi partendo dai file sorgenti specificati nel file index e come destinazione ha la DestFolder
        /// Viene usato il ConversionParameter per specificare come convertire i media
        /// </summary>
        /// <param name="ListaNodi">Lista dei Nodi da convertire</param>
        /// <param name="IndexFileSorgente"> IndexFile dei media sorgenti</param>
        /// <param name="DestFolder"> Cartella di destinazione dei media</param>
        /// <param name="cp">Parametri di conversione</param>
        public bool BeginConvertPlaylist(IEnumerable<FileSystemNodePlus<MyAddittionalData>> ListaNodi, IndexFile IndexFileSorgente, String DestFolder, ConversionParameter cp)
        {
            if (GetConvertMediaStatus() == ConvertMediaAsyncStatus.nul)
            {
                WorkerThread = new Thread(() =>
                {
                    ConvertPlaylist(ListaNodi, IndexFileSorgente, DestFolder, cp);
                    if (CanUseGui)
                        WaitGUIClose();

                    OnEnd?.Invoke();
                });
                WorkerThread.Start();
                return true;
            }
            else
            {
                return false;
            }
        }
        public void WaitConvertPlaylist()
        {
            ConvertMediaAsyncStatus s = GetConvertMediaStatus();
            if (s == ConvertMediaAsyncStatus.nul || s == ConvertMediaAsyncStatus.Stop || s == ConvertMediaAsyncStatus.Completed)
                return;
            else
                WorkerThread.Join();
        }
        public void AbortConvertPlaylist()
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
       



        public ConvertMediaAsyncStatus GetConvertMediaStatus()
        {

            if (WorkerThread == null)
                return ConvertMediaAsyncStatus.nul;

            else if (WorkerThread.ThreadState == ThreadState.Running || WorkerThread.ThreadState == ThreadState.WaitSleepJoin)
                if (_Pause)
                    return ConvertMediaAsyncStatus.Pause;
                else
                    return ConvertMediaAsyncStatus.Running;


            else if (WorkerThread.ThreadState == ThreadState.Stopped || WorkerThread.ThreadState == ThreadState.StopRequested || WorkerThread.ThreadState == ThreadState.Aborted || WorkerThread.ThreadState == ThreadState.AbortRequested)
                return ConvertMediaAsyncStatus.Stop;
            else if (Completed)
                return ConvertMediaAsyncStatus.Completed;

            return ConvertMediaAsyncStatus.nul;
        }

       
   

        public void WaitGUIClose()
        {
            if (CanUseGui)
                GUIThread.Join();
        }
        public bool StartGui()
        {
            if (CMForm == null && (GUIThread == null || GUIThread.ThreadState == ThreadState.Stopped || GUIThread.ThreadState == ThreadState.StopRequested || GUIThread.ThreadState == ThreadState.Aborted || GUIThread.ThreadState == ThreadState.AbortRequested))
            {
                CMForm = new ConvertMediaForm();
                GUIThread = new Thread(() =>
                {
                    CMForm.ShowDialog();
                    CMForm.CloseInvoke();
                    CMForm = null;
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
            if (CMForm != null && GUIThread != null && GUIThread.ThreadState == ThreadState.Running)
            {
                CMForm.CloseInvoke();
                GUIThread.Abort();
                CMForm = null;
            }
        }


    }

    public enum ConvertMediaError
    {
        nul,
        FileNonTrovato,
        ErrDuranteLaCopia
    }
    public enum ConvertMediaAsyncStatus
    {
        Completed,
        Running,
        Stop,
        Pause,
        nul

    }
}
