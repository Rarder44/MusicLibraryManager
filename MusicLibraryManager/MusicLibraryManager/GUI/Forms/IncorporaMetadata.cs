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


namespace MusicLibraryManager.GUI.Forms
{
    public partial class IncorporaMetadata : Form
    {
        MyFileSystemPlus BaseFileSystem;
        FileSystemNodePlus<MyAddittionalData> StartNode;

        MetadataIncluder MI = null;
        public bool Pause
        {
            get
            {
                return MI.Pause;
            }
            set
            {
                MI.Pause = value;
            }
        }



        public IncorporaMetadata()
        {
            InitializeComponent();


        }

        public IncorporaMetadata(MyFileSystemPlus mfsp, FileSystemNodePlus<MyAddittionalData> AlternativeNode = null)
        {
            InitializeComponent();
            this.BaseFileSystem = mfsp;
            if(AlternativeNode ==null)
                this.StartNode = mfsp.Root;
            else
                this.StartNode = AlternativeNode;

            MI = new MetadataIncluder();
            MI.OnEnd += MI_OnEnd;
            MI.OnNodeStartProcessing += MI_OnNodeStartProcessing;
            MI.OnNodeProcessed += MI_OnNodeProcessed;
            MI.OnProgressChangedSingleMD5 += MI_OnProgressChangedSingleMD5;

        }

       
        private void IncorporaMetadata_Load(object sender, EventArgs e)
        {
            Start();
        }



        double buffer = 0;
        private void MI_OnProgressChangedSingleMD5(double AddPercent)
        {
            buffer += AddPercent;
            progressBar_single.SetProgressNoAnimationInvoke((int)buffer);
        }

        private void MI_OnNodeProcessed(FileSystemNodePlus<MyAddittionalData> nodo, String Path, MetadataIncluderError Err)
        {
            
            progressBar_total.SetProgressNoAnimationInvoke(progressBar_total.Value + 1);
        }
        private void MI_OnNodeStartProcessing(FileSystemNodePlus<MyAddittionalData> nodo, string Path)
        {
            buffer = 0;
            textBox_source.SetTextInvoke(Path);
        }

        private void MI_OnEnd()
        {

            OnIncorporaMetadataFormEnd?.Invoke(BaseFileSystem, StartNode, IncorporaMetadataFormResult.Finish);
            FormClosing -= IncorporaMetadata_FormClosing;
            this.CloseInvoke();

        }

        private void IncorporaMetadata_FormClosing(object sender, FormClosingEventArgs e)
        {
            Pause = true;
            if (MessageBox.Show("Sicuro di voler interrompere il processo?", "Interrompere?", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation)==DialogResult.OK)
            {
                Stop();
                FormClosing -= IncorporaMetadata_FormClosing;
                OnIncorporaMetadataFormEnd?.Invoke(BaseFileSystem, StartNode, IncorporaMetadataFormResult.Aborted);

                Close();
                return;
            }
            Pause = false;
            e.Cancel = true;
        }


        /// <summary>
        /// Permette di far partire l'incorporazione dei metadata 
        /// </summary>
        public void Start()
        {
            int Total = StartNode.GetNodeCount(FileSystemNodePlusLevelType.AllNode, FileSystemNodePlusType.File);
            progressBar_total.SetMaximumInvoke(Total);
            MI.BeginIncorporaMetadata(StartNode, BaseFileSystem); 
        }

      


        public void Stop()
        {
            try
            {
                MI.AbortIncorporaMetadata();
            }
            catch (Exception)
            {}
        }



       
        



        public delegate void IncorporaMetadataFormEnd(MyFileSystemPlus mfsp, FileSystemNodePlus<MyAddittionalData> AlternativeNode, IncorporaMetadataFormResult Result);
        public event IncorporaMetadataFormEnd OnIncorporaMetadataFormEnd;
        
    }
    public enum IncorporaMetadataFormResult
    {
        Finish,
        Aborted
    }
 

}
