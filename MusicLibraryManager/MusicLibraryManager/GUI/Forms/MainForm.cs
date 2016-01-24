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
using MusicLibraryManager.GUI.Controls;

namespace MusicLibraryManager.GUI.Forms
{
    public partial class MainForm : Form
    {
        Option option;
        MainFormStatus _status;
        MainFormStatus status
        {
            get { return _status; }
            set
            {
                _status = status;
                if(_status==MainFormStatus.RootBrowsing)
                {
                    fileBrowser1.Type = GUI.Controls.FileBrowserType.Root;
                    fileBrowser1.lp = listBox_playlists.Items.ToList<Playlist>();
                }
                else if (_status == MainFormStatus.PlaylistBrowsing)
                {
                    fileBrowser1.Type = GUI.Controls.FileBrowserType.Playlist;
                    fileBrowser1.lp = null;
                }
            }
        }

        MyFileSystemPlus Current = null;
        MyFileSystemPlus RootFileSystem;


        public MainForm()
        {
            InitializeComponent();
        }
        private void MainForm_Load(object sender, EventArgs e)
        {
            LoadOptionFromFile();
            LoadPlaylistlsocationFromFile();
            status = MainFormStatus.RootBrowsing;
            fileBrowser1.AddItemRequest += (Playlist p, MyFileSystemPlus f) =>
            {
                MyFileSystemPlus c = f.Clone()._Cast<MyFileSystemPlus>();

                c.RimuoviOggettiNonSelezionati();
                c.CancellaCartelleVuote();
                
                if (p.FileSystem == null)
                    p.FileSystem = new MyFileSystemPlus();
                p.FileSystem.Merge(c);
                p.FileSystem.Root.SetParentOnAllChild(FileSystemNodePlusLevelType.AllNode);
                p.FileSystem.RootPath = f.RootPath;
                new SingleFile(p.FileSystem.Root).SetSelectChildNode(false);
                new SingleFile(RootFileSystem.Root).SetSelectChildNode(false);
                SavePlaylist(p);
            };
            fileBrowser1.RemoveItemRequest += (MyFileSystemPlus ToRemoveSelect) =>
            {
                ToRemoveSelect.RimuoviOggettiSelezionati();
                ToRemoveSelect.CancellaCartelleVuote();
                fileBrowser1.ReloadNode();
                SavePlaylist(listBox_playlists.Items.Cast<Playlist>().Where(x => x.FileSystem == ToRemoveSelect).First());
            };

        }


        void LoadOptionFromFile()
        {
            FileData FD = FileReader.ReadFile(GlobalVar.PathOption);
            if (FD == null)
            {
                MessageBox.Show("File di opzioni: " + GlobalVar.PathOption + " Non trovato.\r\nVerrà creato un nuovo file Opzioni");
                option = new Option();
                FileReader.WriteFile(GlobalVar.PathOption, option, FileDataType.Option);
            }
            else if (FD.o == null || !(FD.o is Option))
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
                if ((var & ChangedVar.PathMedia) == ChangedVar.PathMedia || (var & ChangedVar.Extensions) == ChangedVar.Extensions)
                {
                    LoadRootMediaLibrary();
                }
                else if((var & ChangedVar.PathFFmpeg) == ChangedVar.PathFFmpeg)
                {
                    FFmpeg.Initialize(option.PathFFmpeg);
                }
            };

            if (option.PathMedia != null)
            {
                LoadRootMediaLibrary();
            }
            if (option.PathFFmpeg != null)
            {
                FFmpeg.Initialize(option.PathFFmpeg);
            }

        }
        void LoadPlaylistlsocationFromFile()
        {
            FileData FD = FileReader.ReadFile(GlobalVar.PathPlaylistlsocation);
            if (FD == null)
            {
                FileReader.WriteFile(GlobalVar.PathPlaylistlsocation, new Playlistlsocation(), FileDataType.Playlistlsocation);
            }
            else if (FD.o == null || !(FD.o is Playlistlsocation))
            {
                MessageBox.Show("File di Playlists: " + GlobalVar.PathOption + " non caricato correttamente.\r\n");
                FileReader.WriteFile(GlobalVar.PathPlaylistlsocation, new Playlistlsocation(), FileDataType.Playlistlsocation);
            }
            else
            {
                Playlistlsocation pll = FD.o._Cast<Playlistlsocation>();
                if (pll.PathPlaylist != null)
                    LoadPlaylists(pll.PathPlaylist);
            }            
        }
        void SavePlaylistlsocation()
        {
            Playlistlsocation pll = new Playlistlsocation();
            foreach (Playlist p in listBox_playlists.Items)
                pll.PathPlaylist.Add(p.Path);
            FileReader.WriteFile(GlobalVar.PathPlaylistlsocation, pll, FileDataType.Playlistlsocation);
        }



        void LoadRootMediaLibrary()
        {
            if (option == null || option.PathMedia == null)
                return;

            FileSystemPlusLoadOption lo = new FileSystemPlusLoadOption();
            lo.IgnoreException = true;
            lo.RestrictExtensionEnable = true;
            if (option != null && option.Extensions != null)
                foreach (string s in option.Extensions)
                    lo.RestrictExtension.AddToLower(s);

            RootFileSystem = new MyFileSystemPlus(option.PathMedia, lo);
            LoadLastLoadedRootMediaLibrary();
        }
        void LoadLastLoadedRootMediaLibrary()
        {
            Current = RootFileSystem;
            status = MainFormStatus.RootBrowsing;
            ReloadCurrentFileSystem();
        }
       


        void LoadPlaylists(List<String> Paths)
        {
            bool err = false;
            foreach (String Path in Paths)
            {
                FileData FD = FileReader.ReadFile(Path);
                if (FD == null)
                {
                    MessageBox.Show("Playlist : " + Path + " non trovata.\r\n");
                    err = true;
                }
                else if (FD.o == null || !(FD.o is Playlist))
                {
                    MessageBox.Show("Playlist: " + Path + " non caricata correttamente.\r\n");
                    err = true;
                }
                else
                {
                    Playlist p = FD.o._Cast<Playlist>();
                    if(p==null)
                    {
                        MessageBox.Show("Playlist: " + Path + " non caricata correttamente.\r\n");
                        err = true;
                    }
                    else
                    {
                        p.Path = Path;
                        if (p.FileSystem != null && p.FileSystem.Root!=null)
                            p.FileSystem.Root.SetParentOnAllChild(FileSystemNodePlusLevelType.AllNode);
                        listBox_playlists.Items.Add(p);
                    }
                    
                    
                }  
            }
            if(err)
                SavePlaylistlsocation();

        }
        void SavePlaylists()
        {
            foreach (Playlist p in listBox_playlists.Items)
                SavePlaylist(p);
        }

        void LoadPlaylist(Playlist p)
        {
            Current = p.FileSystem;
            fileBrowser1.Type = GUI.Controls.FileBrowserType.Playlist;
            ReloadCurrentFileSystem();
        }
        void SavePlaylist(Playlist p)
        {
            if(p!=null)
                FileReader.WriteFile(p.Path, p, FileDataType.Playlist);
        }

        void AddPlaylist(Playlist p)
        {
            listBox_playlists.AddInvoke(p);
        }
        void RemovePlaylist(Playlist p)
        {
            listBox_playlists.RemoveInvoke(p);
        }

        void ReloadCurrentFileSystem()
        {
            if (Current != null)
            {
                fileBrowser1.LoadMyFileSystemPlus(Current);
            }
            else
            {
                fileBrowser1.Clear();
            }
        }



   


        private void button2_Click(object sender, EventArgs e)
        {
            listBox_playlists.SelectedIndex = -1;

            LoadLastLoadedRootMediaLibrary();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            OptionForm o = new OptionForm(option);
            o.ShowDialog();
        }




        private void listBox_playlists_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            int i = listBox_playlists.IndexFromPoint(e.X, e.Y);
            if (i != -1)
            {
                LoadPlaylist((Playlist)listBox_playlists.Items[i]);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            RequestForm rq = new RequestForm();
            rq.Text = "Inserisci Nome Playlist";
            rq.label1.Text = "Inserisci il nome della Playlist";
            rq.ShowDialog();
            if(rq.Saved)
            {
                String s = rq.textBox1.Text.Trim();
                if (s=="")
                {
                    MessageBox.Show("Devi Inserire un nome valido");
                    return;
                }

                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "Media Library Manager File | *.mlm;";
                if (sfd.ShowDialog()==DialogResult.OK)
                {
                    Playlist p = new Playlist(sfd.FileName,s);
                    AddPlaylist(p);
                    SavePlaylist(p);
                    SavePlaylistlsocation();
                    status = MainFormStatus.RootBrowsing;
                }
            }
        }

        private void listBox_playlists_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                int i = listBox_playlists.IndexFromPoint(e.X, e.Y);
                if (i != -1)
                {
                    listBox_playlists.SelectedIndex = i;
                    contextMenuStrip1.Show(Cursor.Position);
                }
            }
        }

        private void rimuoviToolStripMenuItem_Click(object sender, EventArgs e)
        {
            listBox_playlists.RemoveAtInvoke(listBox_playlists.SelectedIndex);
            status = MainFormStatus.RootBrowsing;
            SavePlaylistlsocation();
        }





        private void originaleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listBox_playlists.SelectedItem is Playlist)
            {
                FolderSelectDialog fsd = new FolderSelectDialog();
                if (fsd.ShowDialog())
                {
                    String destFolder = fsd.FileName;
                    Playlist p = (Playlist)listBox_playlists.SelectedItem;
                    ListPlus<String> ls = p.FileSystem.GetAllFilePath();

                    ListPlus<Tuple<String, String>> lss = new ListPlus<Tuple<string, string>>();
                    foreach (String s in ls)
                    {
                        lss.Add(new Tuple<string, string>(SystemService.Combine(p.FileSystem.RootPath, s.TrimStart('\\', '/')), SystemService.Combine(destFolder, s.TrimStart('\\', '/'))));
                    }


                    //TODO richiamare la convert dicendogli di non convertire nulla ma di copiare
                    ConvertMedia CM = new ConvertMedia(new ConversionParameter(lss, true));
                    CM.Show();
                    CM.Start();
                }
            }


        }
        private void convertiMP3ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(listBox_playlists.SelectedItem is Playlist)
            {
                FolderSelectDialog fsd = new FolderSelectDialog();
                if (fsd.ShowDialog())
                {
                    String destFolder = fsd.FileName;
                    Playlist p = (Playlist)listBox_playlists.SelectedItem;
                    ListPlus<String> ls = p.FileSystem.GetAllFilePath();
                    
                    ListPlus<Tuple<String, String>> lss = new ListPlus<Tuple<string, string>>();
                    foreach (String s in ls)
                    {    
                        lss.Add(new Tuple<string, string>(SystemService.Combine(p.FileSystem.RootPath, s.TrimStart('\\', '/')), SystemService.Combine(destFolder, SystemService.ChangeExtension(s.TrimStart('\\', '/'), "mp3"))));
                    }

                    ConvertMedia CM = new ConvertMedia(new ConversionParameter(lss, true));
                    CM.Show();
                    CM.Start();
                }
            }
        }

        private void incorporaMetadataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Attenzione!\r\nSe un file non viene trovato NON verrà eliminato dalla lista ma i metadati rimmarrano vuoti\r\nTip: Esegui prima \"Check dei File\"\r\n\r\nIl processo può richiedere paracchio tempo, continuare?","Attenzione",MessageBoxButtons.YesNo,MessageBoxIcon.Exclamation) == DialogResult.Yes)
            {

            }
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            //TODO: testare l'incorporazione dei metadata
            //TODO: implementare finestra e nuovo thread per la ricerca dei metadata
            if (listBox_playlists.SelectedItem is Playlist)
            {
                IncorporaMetadataRicorsivo(((Playlist)listBox_playlists.SelectedItem).FileSystem.Root);
                SavePlaylist((Playlist)listBox_playlists.SelectedItem);
            }

        }


        private void IncorporaMetadataRicorsivo(FileSystemNodePlus<MyAddittionalData> nodo)
        {
            foreach(FileSystemNodePlus<MyAddittionalData> n in nodo.GetAllNode())
            {
                if(n.Type==FileSystemNodePlusType.Directory)
                    IncorporaMetadataRicorsivo(n);
                else if (n.Type == FileSystemNodePlusType.File)
                {
                    String p = n.GetFullPath();
                    if (SystemService.Exist(p))
                    {
                        if (n.AddittionalData == null)
                            n.AddittionalData = new MyAddittionalData();

                        if (n.AddittionalData.Metadata == null)
                            n.AddittionalData.Metadata = new FFmpegMetadata();

                        n.AddittionalData.Metadata = FFmpeg.GetMetadata(p);
                        n.AddittionalData.MD5 = SystemService.GetMD5(p);
                    }
                }
            }
        }
    }

    public enum MainFormStatus
    {
        RootBrowsing,
        PlaylistBrowsing
    }
}
