﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ExtendCSharp;
using ExtendCSharp.Services;

namespace MusicLibraryManager
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {

            /*
            File System
            MyFileSystemLoadOption lo =new MyFileSystemLoadOption();
            lo.IgnoreException = true;
            MyFileSystem mfs= new MyFileSystem("D:",lo);
            MyFileSystemNode n = mfs.Root["\\DownloadTorrent\\3DMGAME-7.Days.To.Die.Alpha.13.6.Steam.Edition.X64.Cracked-3DM\\3DMGAME-7.Days.To.Die.Alpha.13.6.Steam.Edition.X64.Cracked-3DM\\7 Days To Die\\serverconfig.xml"];
            n = mfs.Root["\\DownloadTorrent\\3DMGAME-7.Days.To.Die.Alpha.13.6.Steam.Edition.X64.Cracked-3DM\\3DMGAME-7.Days.To.Die.Alpha.13.6.Steam.Edition.X64.Cracked-3DM\\7 Days To Die\\installscript.vdf"];
            String ss = n.GetFullPath();
            */

            /*
            Conversione Flac2Mp3
            FFmpeg f = new FFmpeg(@"C:\Users\Luca\Downloads\ffmpeg-20160116-git-d7c75a5-win64-static\ffmpeg-20160116-git-d7c75a5-win64-static\bin\ffmpeg.exe");
            f.FlacToMp3(@"C:\Users\Luca\Downloads\ffmpeg-20160116-git-d7c75a5-win64-static\ffmpeg-20160116-git-d7c75a5-win64-static\bin\a.flac", @"C:\Users\Luca\Downloads\ffmpeg-20160116-git-d7c75a5-win64-static\ffmpeg-20160116-git-d7c75a5-win64-static\bin\a.mp3", true, OnFFmpegStatusChanged,OnFFmpegProgressChanged, true);
            */
            
        }
       
        /*
        void OnFFmpegStatusChanged(FFmpegStatus Status)
        {
            textBox1.SetTextInvoke( Status.ToString());
        }

        void OnFFmpegProgressChanged(int Percent)
        {
            progressBar1.SetValueInvoke(Percent);
        }
        */




        private void dataGridView1_CellValidated(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                textBox1.Text = "";
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    DataGridViewCheckBoxCell chk = row.Cells[0] as DataGridViewCheckBoxCell;
                    textBox1.Text += Convert.ToInt32(chk.Value) + "\r\n";

                }
                
                


                
            }
            catch(Exception ex) { }
        }
    }
}
