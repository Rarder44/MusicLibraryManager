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
    public partial class ConvertMedia : Form
    {
        ConversionParameter CP;
        ListPlus<Tuple<String, String,FFmpegError>> Error;

        public bool Pause { get; set; }
        Thread Esecuzione;
        
        //TODO: implementare metodi per la selezione del formato di output e se convertire o no determinati formati/tutti  i file


        public ConvertMedia(ConversionParameter cp)
        {
            InitializeComponent();
            CP = cp;
        }

        private void ConvertMedia_FormClosing(object sender, FormClosingEventArgs e)
        {
            Pause = true;
            if (MessageBox.Show("Sicuro di voler interrompere il processo?", "Interrompere?", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation)==DialogResult.OK)
            {
                Stop();
                FormClosing -= ConvertMedia_FormClosing;
                this.Close();
                return;
            }
            Pause = false;
        }


        public void Start()
        {
            Esecuzione = new Thread((object cp) =>
            {
                if (cp == null || !(cp is ConversionParameter))
                {
                    MessageBox.Show("Occorre passare i parametri di conversione!", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                ConversionParameter conv = (ConversionParameter)cp;
                if (conv.SourceDestination == null)
                {
                    MessageBox.Show("Occorre passare una lista di file da convertire!", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else if (!FFmpeg.Loaded)
                {
                    MessageBox.Show("FFmpeg non inizializzato!", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                Error = new ListPlus<Tuple<string, string, FFmpegError>>();


                progressBar_total.SetMaximumInvoke(conv.SourceDestination.Count);
                foreach (Tuple<String, String> t in conv.SourceDestination)
                {
                    while(Pause)
                        Thread.Sleep(100);
                    

                    if (t.Item1.ToLower().EndsWith("mp3"))
                    {
                        textBox_source.SetTextInvoke(t.Item1);
                        textBox_destination.SetTextInvoke(t.Item2);
                        SystemService.CopySecure(t.Item1, t.Item2, true, (double p, ref bool f) => { progressBar_single.SetValueInvoke((int)p); });
                    }
                    else
                    {
                        FFmpeg.ToMp3(t.Item1, t.Item2, conv.OverrideIFExist, OnFFmpegStatusChanged, OnFFmpegProgressChanged, false);
                    }
                    progressBar_total.SetValueInvoke(progressBar_total.Value+1);
                }
                FormClosing -= ConvertMedia_FormClosing;
                this.CloseInvoke();
            });
            Esecuzione.Start(CP);
        }


        void OnFFmpegStatusChanged(FFmpegStatus Status, String Source, String Destination)
        {
            if(Status==FFmpegStatus.Running)
            {
                textBox_source.SetTextInvoke(Source);
                textBox_destination.SetTextInvoke(Destination);
            }
        }
        void OnFFmpegProgressChanged(int Percent, String Source, String Destination,FFmpegError Err)
        {
            if(Err==FFmpegError.DestFolderNotFound)
                Error.Add(new Tuple<string, string, FFmpegError>(Source, Destination, Err));
            else if(Err == FFmpegError.nul)
                progressBar_single.SetValueInvoke(Percent);
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
    }

    public class ConversionParameter
    {
        public ConversionParameter(ListPlus<Tuple<String, String>> SourceDestination,bool OverrideIfExist=true)
        {
            this.SourceDestination = SourceDestination;
        }
        public ListPlus<Tuple<String, String>> SourceDestination;
        public bool OverrideIFExist { get; set; }
    }




}
