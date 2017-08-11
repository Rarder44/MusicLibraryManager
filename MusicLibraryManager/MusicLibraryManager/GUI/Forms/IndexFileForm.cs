using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ExtendCSharp;

namespace MusicLibraryManager.GUI.Forms
{
    public partial class IndexFileForm : Form
    {
        #region Variabili

        private FormStatus _Status;
        public FormStatus Status
        {
            get { return _Status; }
        }
        private Option lo = GlobalVar.ApplicationOption;

        #endregion


        public IndexFileForm()
        {
            InitializeComponent();
            if (lo != null)
                checkBox_CloseAutomatic.Checked = lo.IndexFileFormCloseFormAutomatically;
        }

        public void SetMessage(String s)
        {
            Label_Messages.SetTextInvoke(s);
        }
        public void SetSource(String s)
        {
            textBox_source.SetTextInvoke(s);
        }

        public void SetProgressSingleMax(int max)
        {
            progressBar_single.SetMaximumInvoke(max);
        }
        public void SetProgressSingleValue(int value)
        {
            progressBar_single.SetValueNoAnimationInvoke(value);
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




        public void AddRimossi(object obj)
        {
            listBox_Rimossi.AddInvoke(obj);
            SelectLastIndex(listBox_Rimossi);
        }
        public void AddRimossi(object[] objArr)
        {
            listBox_Rimossi.AddInvoke(objArr);
            SelectLastIndex(listBox_Rimossi);
        }
        public void AddRimossi(ListBox.ObjectCollection objColl)
        {
            listBox_Rimossi.AddInvoke(objColl);
            SelectLastIndex(listBox_Rimossi);
        }
        

        public void AddAggiunti(object obj)
        {
            listBox_Aggiunti.AddInvoke(obj);
            SelectLastIndex(listBox_Aggiunti);
        }
        public void AddAggiunti(object[] objArr)
        {
            listBox_Aggiunti.AddInvoke(objArr);
            SelectLastIndex(listBox_Aggiunti);
        }
        public void AddAggiunti(ListBox.ObjectCollection objColl)
        {
            listBox_Aggiunti.AddInvoke(objColl);
            SelectLastIndex(listBox_Aggiunti);
        }

        public void AddProblemi(object obj)
        {
            listBox_Problemi.AddInvoke(obj);
            SelectLastIndex(listBox_Problemi);
        }
        public void AddProblemi(object[] objArr)
        {
            listBox_Problemi.AddInvoke(objArr);
            SelectLastIndex(listBox_Problemi);
        }
        public void AddProblemi(ListBox.ObjectCollection objColl)
        {
            listBox_Problemi.AddInvoke(objColl);
            SelectLastIndex(listBox_Problemi);
        }

        public void SelectLastIndex(ListBox l)
        {
            l.SetSelectedIndexInvoke(l.GetItemsCountInvoke() - 1);
        }


        private void IndexFileForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _Status = FormStatus.Closed;
        }
        private void IndexFileForm_Load(object sender, EventArgs e)
        {
            _Status = FormStatus.Open;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (lo != null)
            {
                lo.IndexFileFormCloseFormAutomatically = sender._Cast<CheckBox>().Checked;
                lo.SomethingChenged();
            }
        }


        public void Fine()
        {
            if (checkBox_CloseAutomatic.Checked)
                this.CloseInvoke();
        }

    }


}
