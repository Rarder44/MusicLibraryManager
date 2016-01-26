using ExtendCSharp.Services;
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
using ExtendCSharp;
using static ExtendCSharp.Services.FFmpeg;
using MusicLibraryManager.Services;
using static ExtendCSharp.Extension;

namespace MusicLibraryManager.GUI.Forms
{
    public partial class IncorporaMetadata : Form
    {
        Playlist p;

        public bool Pause { get; set; }
        Thread Esecuzione;
        

       
        public IncorporaMetadata(Playlist p)
        {
            InitializeComponent();
            this.p = p;
        }


        private void IncorporaMetadata_FormClosing(object sender, FormClosingEventArgs e)
        {
            Pause = true;
            if (MessageBox.Show("Sicuro di voler interrompere il processo?", "Interrompere?", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation)==DialogResult.OK)
            {
                Stop();
                FormClosing -= IncorporaMetadata_FormClosing;
                if (OnIncorporaMetadataFormEnd != null)
                    OnIncorporaMetadataFormEnd(p, IncorporaMetadataFormResult.Aborted);
                this.Close();
                return;
            }
            Pause = false;
            e.Cancel = true;
        }


        public void Start()
        {
            Esecuzione = new Thread((object n) =>
            {
                if (n == null || !(n is Playlist))
                {
                    MessageBox.Show("Occorre passare una Playlist!", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }


                Playlist pp = (Playlist)n;
                int Total = pp.FileSystem.Root.GetNodeCount(FileSystemNodePlusLevelType.AllNode, FileSystemNodePlusType.File);
                progressBar_total.SetMaximumInvoke(Total);

                double buffer = 0;
                IncorporaMetadataRicorsivo(pp.FileSystem.Root,pp.FileSystem, (FileSystemNodePlus<MyAddittionalData> nodo, IncorporaMetadataError Err) =>
                {
                    buffer = 0;
                    progressBar_total.SetProgressNoAnimationInvoke(progressBar_total.Value + 1);
                },(double AddPercent) =>
                {
                    buffer += AddPercent;
                    progressBar_single.SetProgressNoAnimationInvoke((int)buffer);
                   
                },false);

                if (OnIncorporaMetadataFormEnd != null)
                    OnIncorporaMetadataFormEnd(p,IncorporaMetadataFormResult.Finish);
                FormClosing -= IncorporaMetadata_FormClosing;
                this.CloseInvoke();
            });
            Esecuzione.Start(p);

            
        }

        private void IncorporaMetadataRicorsivo(FileSystemNodePlus<MyAddittionalData> nodo, MyFileSystemPlus mfsp, IncorporaMetadataNodeProcessed OnIncorporaMetadataNodeProcessed = null, MD5BlockTransformEventHandler OnProgressChangedSingleMD5 = null, bool Async = true)
        {
            foreach (FileSystemNodePlus<MyAddittionalData> n in nodo.GetAllNode())
            {
                while (Pause)
                    Thread.Sleep(100);

                if (n.Type == FileSystemNodePlusType.Directory)
                    IncorporaMetadataRicorsivo(n, mfsp, OnIncorporaMetadataNodeProcessed, OnProgressChangedSingleMD5, Async);
                else if (n.Type == FileSystemNodePlusType.File)
                {
                    textBox_source.SetTextInvoke(mfsp.GetFullPath(n));
                    progressBar_single.SetValueInvoke(0);
                    MetadataService.IncorporaMetadata(n, mfsp, OnIncorporaMetadataNodeProcessed, OnProgressChangedSingleMD5, Async);
                }
            }
        }




        public void Stop()
        {
            try
            {
                Esecuzione.Abort();
            }
            catch (Exception)
            {}
        }


        public delegate void IncorporaMetadataFormEnd(Playlist p, IncorporaMetadataFormResult Result );
        public event IncorporaMetadataFormEnd OnIncorporaMetadataFormEnd;


    }
    public enum IncorporaMetadataFormResult
    {
        Finish,
        Aborted
    }
 

}
