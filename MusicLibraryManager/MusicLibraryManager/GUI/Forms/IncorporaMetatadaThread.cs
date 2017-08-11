using ExtendCSharp;
using ExtendCSharp.ExtendedClass;
using ExtendCSharp.Interfaces;
using ExtendCSharp.Services;
using MusicLibraryManager.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static MusicLibraryManager.Services.MetadataIncluderServices;

namespace MusicLibraryManager.GUI.Forms
{
    public partial class IncorporaMetatadaThread : Form
    {
        public static int width = 500;
        public static int height = 108;



        MutexObjectDispatcherServices<Nodo_pathsource> dispatcher;
        IncorporaMetadataNodeProcessed OnNodeProcessed;
        PauseCatcher pc;


        bool formClosing = false;



        public IncorporaMetatadaThread(MutexObjectDispatcherServices<Nodo_pathsource> dispatcher, IncorporaMetadataNodeProcessed OnNodeProcessed, PauseCatcher pc, int ypos =0)
        {
            InitializeComponent();
            this.Width = width;
            this.Height = height;
            this.LockSizeInvoke();

            Location = new Point(Screen.FromControl(this).Bounds.Width - Width, ypos);

            this.dispatcher = dispatcher;
            this.OnNodeProcessed = OnNodeProcessed;
            this.pc = pc;
        }




        private void IncorporaMetatadaThread_Load(object sender, EventArgs e)
        {
            //Faccio partire il thread di lettura del dispatcher
            if (!dispatcher.End)
            {
                Thread t = new Thread(() =>
                {
                    while (!dispatcher.End && !formClosing)
                    {
                        while (dispatcher.pause)
                            Thread.Sleep(100);
                        Nodo_pathsource np = dispatcher.Get();
                        if (np == null)
                            Thread.Sleep(100);
                        else
                        {
                            IncorporaMetadataNodo(np.nodo, np.Pathsource);
                        }
                    }

                    CloseRequestFromThread(Thread.CurrentThread);

                });
                t.Start();
            }
            else
                RealClose();
        }

        private void IncorporaMetadataNodo(FileSystemNodePlus<MyAddittionalData> nodo, String PathSource)
        {

            SystemService ss = ServicesManager.Get<SystemService>();
            if (ss.FileExist(PathSource))
            {
                textBox1.SetTextInvoke(PathSource);
                progressBar1.SetValueNoAnimationInvoke(0);


                if (nodo.AddittionalData == null)
                    nodo.AddittionalData = new MyAddittionalData();

                if (nodo.AddittionalData.Metadata == null)
                    nodo.AddittionalData.Metadata = new FFmpegMetadata();

                FFmpeg fs = ServicesManager.Get<FFmpeg>();
                nodo.AddittionalData.Metadata = fs.GetMetadata(PathSource);
                nodo.AddittionalData.Size = ss.FileSize(PathSource);

                ss.GetMD5(PathSource, (double percent) =>
                {
                    progressBar1.SetValueNoAnimationInvoke((int)percent);
                }, (byte[] Hash) =>
                {
                    progressBar1.SetValueNoAnimationInvoke(100);
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




        private void RealClose()
        {
            formClosing = true;
            this.Close();
        }
        private void CloseRequestFromThread(Thread t)
        {

            try
            {
                this.BeginInvoke((MethodInvoker)delegate
                {
                    if(t.IsAlive)
                        t.Join();
                    RealClose();
                });
            }
            catch (Exception e)
            {

            };
        }

        private void IncorporaMetatadaThread_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!formClosing)
            {
                e.Cancel = true;
                formClosing = true;
            }

        }


    }


}
