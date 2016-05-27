using ExtendCSharp;
using ExtendCSharp.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MusicLibraryManager.GUI.Forms
{
    public partial class OptionForm : Form
    {
        Option option = null;


        public OptionForm(Option o)
        {
            InitializeComponent();
            option = o;
        }
        private void OptionForm_Load(object sender, EventArgs e)
        {
            textBox_rootMedia.Text = option.PathMedia;
            textBox_ffmpeg.Text = option.PathFFmpeg;
            foreach(string s in option.Extensions)
                listBox1.Items.Add(s);

            checkBox1.Checked = option.IndexFileFormCloseFormAutomatically;
        }

        private void MediaPath_Click(object sender, EventArgs e)
        {
            FolderSelectDialog fsd = new FolderSelectDialog();
            if(fsd.ShowDialog())
            {
                if (Directory.Exists(fsd.FileName))
                {
                    textBox_rootMedia.Text = fsd.FileName;
                }
                else
                    MessageBox.Show("Errore nel recupero della cartella");
            }
        }


        private void FFmpeg_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.FileName = "FFmpeg";
            ofd.Filter = "FFmpeg|FFmpeg.exe";
            if (ofd.ShowDialog()==DialogResult.OK)
            {
                if (File.Exists(ofd.FileName))
                {
                    if(FFmpeg.Initialize(ofd.FileName))
                    {
                        textBox_ffmpeg.Text = ofd.FileName;
                    }
                    else
                    {
                        MessageBox.Show("File FFmpeg non valido");
                    }
                }
                else
                    MessageBox.Show("Errore nel recupero del file");
            }
        }

       
        private void listBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if(e.Button==MouseButtons.Right)
            {
                listBox1.SelectedIndex = listBox1.IndexFromPoint(e.X, e.Y);
                if(listBox1.SelectedIndex!=-1)
                    contextMenuStrip1.Show(Cursor.Position);
            }
        }
        private void button4_Click(object sender, EventArgs e)
        {
            string s = textBox_ext.Text.Trim(' ', '.').ToLower();
            if(s!="")
                listBox1.Items.Add(s);
            textBox_ext.Text = "";
        }
        private void asdToolStripMenuItem_Click(object sender, EventArgs e)
        {
            listBox1.Items.Remove(listBox1.SelectedItem);
        }


        private void Salva_Click(object sender, EventArgs e)
        {
            option.PathMedia = textBox_rootMedia.Text;
            option.PathFFmpeg = textBox_ffmpeg.Text;

            ListPlus<String> ls = listBox1.Items.ToListPlus<String>();
            option.Extensions.RemoveNotInRange(ls);
            option.Extensions.AddUnique(ls);
            option.IndexFileFormCloseFormAutomatically = checkBox1.Checked;

            Close();
      
            
        }

        private void OptionForm_FormClosed(object sender, FormClosedEventArgs e)
        {

        }
    }
}
