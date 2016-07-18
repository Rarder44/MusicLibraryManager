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
    public partial class ConvertMediaForm : Form
    {

        #region Event

        public event Action<bool> SetPause;
        public event Action Stop;

        #endregion


        #region Variabili

        private FormStatus _Status;
        public FormStatus Status
        {
            get { return _Status; }
        }
        public bool Pause { get; set; }
        Thread Esecuzione;
        bool _Finito = false;

        #endregion

 

        public ConvertMediaForm()
        {
            InitializeComponent();
        }
     
      

        public void SetSource(String source)
        {
            textBox_source.SetTextInvoke(source);
        }
        public void SetDestination(String destination)
        {
            textBox_destination.SetTextInvoke(destination);
        }


        public void SetProgressSingleMax(int max)
        {
            progressBar_single.SetMaximumInvoke(max);
        }
        public void SetProgressSingleValue(int value)
        {
            progressBar_single.SetValueNoAnimationInvoke(value);
        }
        public int GetProgressSingleValue()
        {
            return progressBar_single.Value;
        }
        public int GetProgressSingleMax()
        {
            return progressBar_single.Maximum;
        }



        public void SetProgressTotalMax(int max)
        {
            progressBar_total.SetMaximumInvoke(max);
        }
        public void SetProgressTotalValue(int value)
        {
            progressBar_total.SetValueNoAnimationInvoke(value);
        }
        public int GetProgressTotalValue()
        {
            return progressBar_total.Value;
        }
        public int GetProgressTotalMax()
        {
            return progressBar_total.Maximum;
        }




        public void StartConvertItem(String source,String dest)
        {
            SetSource(source);
            SetDestination(dest);
            SetProgressTotalValue(GetProgressTotalValue() + 1);
            SetProgressSingleValue(0);
        }
        public void UpdateProgressPartial(int value)
        {
            SetProgressSingleValue(value);
        }
        public void AppendLog(String str)
        {
            textBox1.AppendTextInvoke(str);
        }
        public void Finito()
        {
            _Finito = true;
            SetSource("Finito!");
            SetDestination("");
            SetProgressSingleValue(GetProgressSingleMax());
            SetProgressTotalValue(GetProgressTotalMax());
        }



        private void ConvertMedia_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_Finito)
                return;

            SetPause?.Invoke(true);

            if (MessageBox.Show("Sicuro di voler interrompere il processo?", "Interrompere?", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
            {
                Stop?.Invoke();
                _Status = FormStatus.Closed;
            }
            else
            {
                SetPause?.Invoke(false);
                e.Cancel = true;
            }
        }
        private void ConvertMediaForm_Load(object sender, EventArgs e)
        {
            _Status = FormStatus.Open;
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



    


    public enum ConversinType
    {
        Sempre,
        SoloDiversi,
        Mai
    }
   


}
