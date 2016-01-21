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
using ExtendCSharp.Services;
using MusicLibraryManager.DataSave;

namespace MusicLibraryManager.GUI.Forms
{
    public partial class MainForm : Form
    {
        Option option;
        public MainForm()
        {
            InitializeComponent();
        }
        private void MainForm_Load(object sender, EventArgs e)
        {
            FileData FD = FileReader.ReadFile(GlobalVar.PathOption);
            if (FD == null)
            {
                MessageBox.Show("File di opzioni: " + GlobalVar.PathOption + " Non trovato.\r\nVerrà creato un nuovo file Opzioni");
                option = new Option();
                FileReader.WriteFile(GlobalVar.PathOption, option, FileDataType.Option);
            }
            else if(FD.o ==null || !(FD.o is Option) )
            {
                MessageBox.Show("File di opzioni: " + GlobalVar.PathOption + " non caricato correttamente.\r\nVerrà creato un nuovo file Opzioni");
                option = new Option();
                FileReader.WriteFile(GlobalVar.PathOption, option, FileDataType.Option);
            }
            else
            {
                option = FD.o._Cast<Option>();
            }

            option.OnSomethingChenged += (ChangedVar var) =>
            {
                FileReader.WriteFile(GlobalVar.PathOption, option, FileDataType.Option);
                if((var&ChangedVar.PathMedia)==ChangedVar.PathMedia)
                {
                    LoadMediaLibrary();
                }
            };


            if (option.PathMedia!=null)
            {
                LoadMediaLibrary();
            }
        }

        void LoadMediaLibrary()
        {
            FileSystemPlusLoadOption lo = new FileSystemPlusLoadOption();
            lo.IgnoreException = true;
            lo.RestrictExtensionEnable = true;
            lo.RestrictExtension.Add("flac");
            lo.RestrictExtension.Add("mp3");
            mfs = new MyFileSystemPlus(option.PathMedia, lo);
            fileBrowser1.LoadNode(mfs.Root);
        }
        MyFileSystemPlus mfs;
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


            

            /*String ss=Json.Serialize(mfs);
            MyFileSystemPlus aa = Json.Deserialize<MyFileSystemPlus > (ss);
            aa.Root.SetParentOnAllChild(FileSystemNodePlusParentSetType.AllNode);
            fileBrowser1.LoadNode(aa.Root);*/




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



        private void button2_Click(object sender, EventArgs e)
        {
            mfs.RimuoviOggettiNonSelezionati();
            mfs.CancellaCartelleVuote();
            fileBrowser1.LoadNode(mfs.Root);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            new OptionForm(option).ShowDialog();
        }

       
    }
}
