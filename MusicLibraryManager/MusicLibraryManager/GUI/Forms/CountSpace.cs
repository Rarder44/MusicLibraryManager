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

namespace MusicLibraryManager.GUI.Forms
{
    public partial class CountSpace : Form
    {
        CountParameter CP;
        public bool Pause { get; set; }
        Thread Esecuzione;
        


        public CountSpace(CountParameter cp)
        {
            InitializeComponent();
            CP = cp;
        }

        private void CountSpace_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
               
                if (Esecuzione.ThreadState == ThreadState.WaitSleepJoin)
                {
                    Pause = true;
                    if (MessageBox.Show("Sicuro di voler interrompere il processo?", "Interrompere?", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
                    {
                        Stop();
                        FormClosing -= CountSpace_FormClosing;
                        this.Close();
                        return;
                    }
                    Pause = false;
                    e.Cancel = true;
                }
            }
            catch(Exception ex)
            {

            }
        }


        public void Start()
        {
            Esecuzione = new Thread((object cp) =>
            {
                if (cp == null || !(cp is CountParameter))
                {
                    MessageBox.Show("Occorre passare i parametri per il conteggio!", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else if (!FFmpeg.Loaded)
                {
                    MessageBox.Show("FFmpeg non inizializzato!", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                CountParameter conv = (CountParameter)cp;

                int Total = conv.mfsp.Root.GetNodeCount(FileSystemNodePlusLevelType.AllNode, FileSystemNodePlusType.File);

                progressBar_total.SetMaximumInvoke(Total);



                long l = CalcoloSpazio(conv.mfsp.Root, conv.mfsp, FFmpegConversionEndFormat.mp3);
                //kb
                String Post = "KB";
                double d = 0;
                if (l > 1024)
                {
                    d = l / 1024.0;//mb
                    Post = "MB";
                    if (d > 1024.0)
                    {
                        d /= 1024.0;//gb 
                        Post = "GB";
                    }
                }
                else
                    d = l;

                textBox1.SetTextInvoke(d + " " + Post);      
            });
            Esecuzione.Start(CP);

            
        }


        private long CalcoloSpazio(FileSystemNodePlus<MyAddittionalData> nodo, MyFileSystemPlus mfsp, FFmpegConversionEndFormat Format)
        {
            if (Format == FFmpegConversionEndFormat.mp3)
            {
                long t = 0;
                foreach (FileSystemNodePlus<MyAddittionalData> n in nodo.GetAllNode())
                {
                    while (Pause)
                        Thread.Sleep(100);

                    if (n.Type == FileSystemNodePlusType.Directory)
                        t += CalcoloSpazio(n, mfsp, Format);
                    else if (n.Type == FileSystemNodePlusType.File)
                    {
                        String p = mfsp.GetFullPath(n);
                        if (SystemService.FileExist(p))
                        {
                            textBox_source.SetTextInvoke(p);
                            String temp = FFmpeg.GetMetadata(p).Duration;
                            string[] st = temp.Split(':');
                            long tt = 0;
                            if (st.Length == 3)
                            {
                                try
                                {
                                    tt = st[0].ParseInt() * 144000; //trovo i secondi e moltiplico x 320/8 -> 3600*40
                                    tt += st[1].ParseInt() * 2400; //trovo i secondi e moltiplico x 320/8 -> 60*40
                                    tt += st[2].Split('.')[0].ParseInt() * 40; //trovo i secondi e moltiplico x 320/8 -> 60*40
                                }
                                catch (Exception e) { }
                                t += tt;
                            }
                        }
                        progressBar_total.SetValueInvoke(progressBar_total.Value + 1);
                    }
                }
                return t;
            }
            return 0;
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



        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.A && e.Modifiers == Keys.Control)
            {
                ((TextBox)sender).SelectAll();
            }
            else if (!(e.KeyCode == Keys.C && e.Modifiers == Keys.Control))
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }
    }

    public class CountParameter
    {
        public CountParameter(MyFileSystemPlus mfsp, FFmpegConversionEndFormat Format)
        {
            this.mfsp = mfsp;
            this.Format = Format;
        }
        public MyFileSystemPlus mfsp;
        public FFmpegConversionEndFormat Format;
    }

    


 


}
