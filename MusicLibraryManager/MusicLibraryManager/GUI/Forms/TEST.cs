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
            FFmpeg.Initialize(@"C:\Program Files (x86)\ffmpeg\ffmpeg.exe","metaflac.exe");

            /*FileData d = FileService.ReadFile("test.txt");
            if (d.Type == FileDataType.IndexFile)
            {
                IndexFile ii = d.o._Cast<IndexFile>();
                ii.Update();
            }*/


            
            FileSystemPlusLoadOption lo = new FileSystemPlusLoadOption();
            lo.IgnoreException = true;
            lo.RestrictExtensionEnable = true;
            lo.RestrictExtension.AddToLower("flac");
            lo.RestrictExtension.AddToLower("mp3");

            FFmpeg.Initialize(@"C:\Program Files (x86)\ffmpeg\ffmpeg.exe", "metaflac.exe");
            //IndexFile I = 
          
            IndexFile i = new IndexFile();
            i.OnEnd += I_OnEnd;
            i.BeginLoadFromPath(@"F:\MUSICA FLAC\ORATORIO\", lo);
    


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

        private void button2_Click(object sender, EventArgs e)
        {
            FFmpeg.Initialize(@"C:\Program Files (x86)\ffmpeg\ffmpeg.exe", "metaflac.exe");

            FileSystemPlusLoadOption lo = new FileSystemPlusLoadOption();
            lo.IgnoreException = true;
            lo.RestrictExtensionEnable = true;
            lo.RestrictExtension.AddToLower("flac");
            lo.RestrictExtension.AddToLower("mp3");


            IndexFile i = new IndexFile();
            i.OnEnd += I_OnEnd2;
            i.BeginLoadFromPath(@"D:\Musica\DJ CAVA Mashups\", lo);


            
            /* Wrapper w = new Wrapper(new FFMpegMediaMetadataFlac());
             String ser = Json.Serialize(w);
             Wrapper tret = Json.Deserialize<Wrapper>(ser);*/

            //D:\Musica\DJ CAVA Mashups

        }

        private void I_OnEnd2(IndexFile i)
        {

            FileSystemNodePlus<MyAddittionalData> n = i.RootFileSystem.FindFirst((x) => { return x.Name.StartsWith("Summertime"); });

            ConvertionEntity s = new ConvertionEntity(i.RootFileSystem.GetFullPath(n), n.AddittionalData.Metadata.MediaMetadata);
            FFMpegMediaMetadataFlac mm = new FFMpegMediaMetadataFlac(n.AddittionalData.Metadata.MediaMetadata);
            mm.SamplingRate = SamplingRateInfo._44100;
            mm.Bit = BitInfo._24;
            ConvertionEntity d = new ConvertionEntity(SystemService.ChangeExtension(i.RootFileSystem.GetFullPath(n), "flac"),mm);
            
            
            FFmpeg.ConvertTo(s, d, true, true, 
                (status, source, dest) => 
                {
                    if (status == FFmpegStatus.Stop)
                        MessageBox.Show(source + " -> " + dest + "\r\n FINE!");
            }, 
                (percent, source, dest, error) => 
                {

            }, false);

        }

        private void TEST_Load(object sender, EventArgs e)
        {

        }
    }

   
  
}
