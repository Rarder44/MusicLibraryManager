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
            textBox1.Text = option.PathMedia;
            textBox2.Text = option.PathFFmpeg;
        }
        private void MediaPath_Click(object sender, EventArgs e)
        {
            FolderSelectDialog fsd = new FolderSelectDialog();
            if(fsd.ShowDialog())
            {
                if (Directory.Exists(fsd.FileName))
                {
                    textBox1.Text = fsd.FileName;
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
                        textBox2.Text = ofd.FileName;
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


        private void Salva_Click(object sender, EventArgs e)
        {
            option.PathMedia = textBox1.Text;
            option.PathFFmpeg = textBox2.Text;
            option.SomethingChenged();
            Close();
        }

        
    }
}
