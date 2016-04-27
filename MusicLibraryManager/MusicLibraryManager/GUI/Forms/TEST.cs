using ExtendCSharp;
using ExtendCSharp.Services;
using MusicLibraryManager.DataSave;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
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
            FFmpeg.Initialize(@"C:\Program Files (x86)\ffmpeg\ffmpeg.exe");

            FileData d = FileService.ReadFile("test.txt");
            if (d.Type == FileDataType.IndexFile)
            {
                IndexFile ii = d.o._Cast<IndexFile>();
                ii.Update();
            }


            /*FileSystemPlusLoadOption lo = new FileSystemPlusLoadOption();
            lo.IgnoreException = true;
            lo.RestrictExtensionEnable = true;
            lo.RestrictExtension.AddToLower("flac");
            lo.RestrictExtension.AddToLower("mp3");

            FFmpeg.Initialize(@"C:\Program Files (x86)\ffmpeg\ffmpeg.exe");
            //IndexFile I = 
          
            IndexFile i = new IndexFile();
            i.OnEnd += I_OnEnd;
            i.BeginLoadFromPath(@"F:\MUSICA FLAC\ORATORIO\", lo,false);
    */


        }

        private void I_OnEnd(IndexFile i)
        {
            FileService.WriteFile("test.txt", i, FileDataType.IndexFile);
            /*FileData d=FileService.ReadFile("test.txt");
            if(d.Type==FileDataType.IndexFile)
            {
                IndexFile ii = d.o._Cast<IndexFile>();
            }*/
        }
    }
}
