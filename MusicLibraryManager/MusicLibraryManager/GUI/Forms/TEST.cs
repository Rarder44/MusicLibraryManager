using ExtendCSharp;
using ExtendCSharp.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MusicLibraryManager.GUI.Forms
{
    public partial class TEST : Form
    {
        public TEST()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            

            FileSystemPlusLoadOption lo = new FileSystemPlusLoadOption();
            lo.IgnoreException = true;
            lo.RestrictExtensionEnable = true;
            lo.RestrictExtension.AddToLower("flac");
            lo.RestrictExtension.AddToLower("mp3");

            FFmpeg.Initialize(@"C:\Program Files (x86)\ffmpeg\ffmpeg.exe");
            //IndexFile I = 
          
            IndexFile i = new IndexFile();

            i.BeginLoadFromPath(@"F:\MUSICA FLAC\", lo,true);

        }
    }
}
