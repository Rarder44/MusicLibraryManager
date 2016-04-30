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
using System.Threading;
using System.IO;


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
        IndexFile IndexMediaLibrary;

        string[] args = null;

        public MainForm()
        {
            InitializeComponent();
        }
        public MainForm(string[] args)
        {
            InitializeComponent();
            this.args = args;
            
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            foreach(String s in GlobalVar.Extensions)
            {
                if (SystemService.NormalizePath(SystemService.FileExtentionInfo(SystemService.AssocStr.Executable, s)) != SystemService.NormalizePath((Application.ExecutablePath)))
                {
                    SystemService.SetAssociationFileExtention(s, "MusicLibraryManager.Rarder44", Application.ExecutablePath, "Music Library Manager RUUUUULEXXXXXX");
                }
            }

            Directory.SetCurrentDirectory(SystemService.GetParent(Application.ExecutablePath));
            LoadOptionFromFile(GlobalVar.PathOption);
            LoadIndexFromFile(GlobalVar.PathIndexFile,true,option.PathMedia,option.LoadMediaOption());
            LoadIndexMediaLibrary();
            LoadPlaylistlsocationFromFile(GlobalVar.PathPlaylistlsocation);
            status = MainFormStatus.RootBrowsing;

            fileBrowser1.AddItemRequest += (Playlist p, MyFileSystemPlus f) =>
            {
                MyFileSystemPlus c = f.Clone()._Cast<MyFileSystemPlus>();

                c.RimuoviFileNonSelezionati();
                c.RimuoviCartelleVuote();
                
                if (p.FileSystem == null)
                    p.FileSystem = new MyFileSystemPlus();
                p.FileSystem.Merge(c);
                p.FileSystem.Root.SetParentOnAllChild(FileSystemNodePlusLevelType.AllNode);
                p.FileSystem.RootPath = f.RootPath;
                

                
                new SingleFile(p.FileSystem.Root).SetSelectChildNode(false);
                new SingleFile(IndexMediaLibrary.RootFileSystem.Root).SetSelectChildNode(false);
                SavePlaylist(p);
            };
            fileBrowser1.RemoveItemRequest += (MyFileSystemPlus ToRemoveSelect) =>
            {
                ToRemoveSelect.RimuoviFileSelezionati();
                ToRemoveSelect.RimuoviCertelleVuoteSelezionate();
                fileBrowser1.ReloadNode();
                SavePlaylist(listBox_playlists.Items.Cast<Playlist>().Where(x => x.FileSystem == ToRemoveSelect).First());
            };


            if(args!=null && args.Length!=0)
            {
                foreach (String s in args)
                {
                    if (SystemService.FileExist(s))
                    {
                        FileData FD = FileService.ReadFile(s);
                        if (FD == null)
                            continue;

                        if (FD.Type == FileDataType.Playlist)
                        {
                            if (!CheckPlaylistIsInList(s))
                            {
                                LoadPlaylist(s);
                                SavePlaylistlsocation(GlobalVar.PathPlaylistlsocation);
                            }
                        }
                        else if (FD.Type == FileDataType.Option)
                        {
                            if(MessageBox.Show("Vuoi sostituire le opzioni?","Continuare?",MessageBoxButtons.YesNo,MessageBoxIcon.Warning)==DialogResult.Yes)
                            {
                                SystemService.CopySecure(s, GlobalVar.PathOption, true);
                                LoadOptionFromFile(GlobalVar.PathOption);
                            }
                        }
                        else if (FD.Type == FileDataType.IndexFile)
                        {
                            if (MessageBox.Show("Vuoi sostituire il file di indice?", "Continuare?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                            {
                                SystemService.CopySecure(s, GlobalVar.PathIndexFile, true);
                                LoadIndexFromFile(GlobalVar.PathIndexFile,true,option.PathMedia,option.LoadMediaOption());
                            }
                        }
                        else if (FD.Type == FileDataType.Playlistlsocation)
                        {
                            Playlistlsocation pll = FD.o._Cast<Playlistlsocation>();
                            if (pll.PathPlaylist != null)
                                LoadPlaylists(pll.PathPlaylist,true,false,true);
                        }
                    }
                }
            }
        }



        void LoadIndexFromFile(String PathIndexFile, bool Update = false, String PathMediaLibrary = null, FileSystemPlusLoadOption lo = null, bool GUI = true)
        {
            
            FileData FD = FileService.ReadFile(PathIndexFile);
            if (FD == null)
            {
                if (GUI)
                    MessageBox.Show("File di Index: " + PathIndexFile + " Non trovato.\r\nVerrà creato un nuovo file Indice");
                IndexMediaLibrary = new IndexFile();
                IndexMediaLibrary.LoadOption = lo;
                FileService.WriteFile(PathIndexFile, IndexMediaLibrary, FileDataType.IndexFile);
            }
            else if (FD.o == null || !(FD.o is IndexFile))
            {
                if (GUI)
                    MessageBox.Show("File di Index: " + PathIndexFile + " non caricato correttamente.\r\nVerrà creato un nuovo file Indice");
                IndexMediaLibrary = new IndexFile();
                IndexMediaLibrary.LoadOption = lo;
                FileService.WriteFile(PathIndexFile, IndexMediaLibrary, FileDataType.IndexFile);
            }
            else
            {
                IndexMediaLibrary = FD.o._Cast<IndexFile>();
                if(lo!=null)
                    IndexMediaLibrary.LoadOption = lo;
            }


            if (Update)
            {
                Index_UpdateAndSave(PathIndexFile, PathMediaLibrary, null); 
            }
        }
        void Index_UpdateAndSave(String PathIndexFile,String PathMediaLibrary = null, FileSystemPlusLoadOption lo = null)
        {
            if (lo != null)
                IndexMediaLibrary.LoadOption = lo;

            if (PathMediaLibrary == null)
                IndexMediaLibrary.Update();
            else
                IndexMediaLibrary.Update(PathMediaLibrary);

            FileService.WriteFile(PathIndexFile, IndexMediaLibrary, FileDataType.IndexFile);

        }

        void LoadOptionFromFile(String Path,bool GUI=true)
        {
            FileData FD = FileService.ReadFile(Path);
            if (FD == null)
            {
                if(GUI)
                    MessageBox.Show("File di opzioni: " + Path + " Non trovato.\r\nVerrà creato un nuovo file Opzioni");
                option = new Option();
                FileService.WriteFile(Path, option, FileDataType.Option);
            }
            else if (FD.o == null || !(FD.o is Option))
            {
                if(GUI)
                    MessageBox.Show("File di opzioni: " + Path + " non caricato correttamente.\r\nVerrà creato un nuovo file Opzioni");
                option = new Option();
                FileService.WriteFile(Path, option, FileDataType.Option);
            }
            else
            {
                option = FD.o._Cast<Option>();
            }

            option.OnSomethingChenged += (ChangedVar var) =>
            {
                FileService.WriteFile(Path, option, FileDataType.Option);
                if ((var & ChangedVar.PathMedia) == ChangedVar.PathMedia || (var & ChangedVar.Extensions) == ChangedVar.Extensions)
                {
                    Index_UpdateAndSave(GlobalVar.PathIndexFile, option.PathMedia,option.LoadMediaOption());
                }
                else if((var & ChangedVar.PathFFmpeg) == ChangedVar.PathFFmpeg)
                {
                    FFmpeg.Initialize(option.PathFFmpeg);
                }
            };


            if (option.PathFFmpeg != null)
                FFmpeg.Initialize(option.PathFFmpeg);

        }

        bool CheckPlaylistIsInList(String Path)
        {
            FileData FD = FileService.ReadFile(GlobalVar.PathPlaylistlsocation);
            if (FD == null || FD.o == null || !(FD.o is Playlistlsocation))
            {
                return false;
            }
            else
            {
                Playlistlsocation pll = FD.o._Cast<Playlistlsocation>();
                return pll.PathPlaylist.Contains(SystemService.NormalizePath(Path));
            }
        }
        void LoadPlaylistlsocationFromFile(String Path,bool GUI=true)
        {
            FileData FD = FileService.ReadFile(Path);
            if (FD == null)
            {
                FileService.WriteFile(GlobalVar.PathPlaylistlsocation, new Playlistlsocation(), FileDataType.Playlistlsocation);
            }
            else if (FD.o == null || !(FD.o is Playlistlsocation))
            {
                if(GUI)
                    MessageBox.Show("File di Playlists: " + GlobalVar.PathOption + " non caricato correttamente.\r\n");
                FileService.WriteFile(GlobalVar.PathPlaylistlsocation, new Playlistlsocation(), FileDataType.Playlistlsocation);
            }
            else
            {
                Playlistlsocation pll = FD.o._Cast<Playlistlsocation>();
                if (pll.PathPlaylist != null)
                    LoadPlaylists(pll.PathPlaylist);
            }            
        }
        void SavePlaylistlsocation(String Path)
        {
            Playlistlsocation pll = new Playlistlsocation();
            foreach (Playlist p in listBox_playlists.Items)
                pll.PathPlaylist.Add(p.Path);
            FileService.WriteFile(Path, pll, FileDataType.Playlistlsocation);
        }




        void LoadIndexMediaLibrary()
        {
            Current = IndexMediaLibrary.RootFileSystem;
            status = MainFormStatus.RootBrowsing;
            ReloadCurrentFileSystem();
        }


        bool LoadPlaylist(String Path, bool ShowMessage = true)
        {
            Path = SystemService.NormalizePath(Path);
            FileData FD = FileService.ReadFile(Path);
            if (FD == null)
            {
                if (ShowMessage)
                    MessageBox.Show("Playlist : " + Path + " non trovata.\r\n");
                return false;
            }
            else if (FD.o == null || !(FD.o is Playlist))
            {
                if (ShowMessage)
                    MessageBox.Show("Playlist: " + Path + " non caricata correttamente.\r\n");
                return false;
            }
            else
            {
                Playlist p = FD.o._Cast<Playlist>();
                if (p == null)
                {
                    if (ShowMessage)
                        MessageBox.Show("Playlist: " + Path + " non caricata correttamente.\r\n");
                    return false;
                }
                else
                {
                    p.Path = Path;
                    if (p.FileSystem != null && p.FileSystem.Root != null)
                        p.FileSystem.Root.SetParentOnAllChild(FileSystemNodePlusLevelType.AllNode);
                    listBox_playlists.Items.Add(p);
                    return true;
                }
            }
        }
        void LoadPlaylists(List<String> Paths,bool ShowMessage=true,bool ClearOld=true,bool ForceResavePlaylist=false)
        {
            if (ClearOld)
                listBox_playlists.ClearInvoke();

            bool err = false;
            foreach (String Path in Paths)
            {
                err = !LoadPlaylist(Path, ShowMessage);
              
            }
            if(err || ForceResavePlaylist)
                SavePlaylistlsocation(GlobalVar.PathPlaylistlsocation);

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
                FileService.WriteFile(p.Path, p, FileDataType.Playlist);
        }

        void ReloadPlaylistFromFile(Playlist p)
        {
            if(SystemService.FileExist(p.Path))
            {
                FileData FD = FileService.ReadFile(p.Path);
                if (FD != null && FD.o is Playlist)
                {
                    Playlist temp = FD.o._Cast<Playlist>();
                    if (p != null)
                    {
                        p.FileSystem = temp.FileSystem;
                        if (p.FileSystem != null && p.FileSystem.Root != null)
                            p.FileSystem.Root.SetParentOnAllChild(FileSystemNodePlusLevelType.AllNode);
                    }
                }

            }
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

            LoadIndexMediaLibrary();
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
                    SavePlaylistlsocation(GlobalVar.PathPlaylistlsocation);
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
            SavePlaylistlsocation(GlobalVar.PathPlaylistlsocation);
        }





        private void originaleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listBox_playlists.SelectedItem is Playlist)
            {
                FolderSelectDialog fsd = new FolderSelectDialog();
                if (fsd.ShowDialog())
                {
                    DialogResult dr = MessageBox.Show("Sovrascrivere eventuali file esistenti?\r\n\r\nSi: il processo Sovrascriverà eventuali file già presenti con lo stesso nome\r\nNo: Il processo VERRA' comunque avviato ma, se un file è già presente nella cartella di Output, questo non verrà sovrascritto\r\nAnnulla: Il processo verrà interrotto", "Conferma Sovrascrittura", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation);
                    if (dr == DialogResult.Cancel)
                        return;

                    bool OverrideIfExist = dr == DialogResult.Yes ? true : false;
                    String destFolder = fsd.FileName;
                    Playlist p = (Playlist)listBox_playlists.SelectedItem;
                    ListPlus<String> ls = p.FileSystem.GetAllFilePath();

                    ListPlus<Tuple<String, String>> lss = new ListPlus<Tuple<string, string>>();
                    foreach (String s in ls)
                    {
                        lss.Add(new Tuple<string, string>(SystemService.Combine(p.FileSystem.RootPath, s.TrimStart('\\', '/')), SystemService.Combine(destFolder, s.TrimStart('\\', '/'))));
                    }


                    ConvertMedia CM = new ConvertMedia(new ConversionParameter(lss,ConversinType.Mai,FFmpegConversionEndFormat.mp3, OverrideIfExist));
                    CM.OnFFmpegConversionEnd += () =>
                    {
                        if (CM.Error != null && CM.Error.Count != 0)
                        {
                            MessageBox.Show("Errori presenti durante la conversione!");
                            //TODO: visualizzazione degli errori
                        }
                    };
                    CM.Show();
                    CM.Start();
                }
            }


        }
        private void convertiMP3ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listBox_playlists.SelectedItem is Playlist)
            {
                FolderSelectDialog fsd = new FolderSelectDialog();
                if (fsd.ShowDialog())
                {
                    DialogResult dr = MessageBox.Show("Sovrascrivere eventuali file esistenti?\r\n\r\nSi: il processo Sovrascriverà eventuali file già presenti con lo stesso nome\r\nNo: Il processo VERRA' comunque avviato ma, se un file è già presente nella cartella di Output, questo non verrà sovrascritto\r\nAnnulla: Il processo verrà interrotto", "Conferma Sovrascrittura", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation);
                    if (dr == DialogResult.Cancel)
                        return;
                    bool OverrideIfExist = dr == DialogResult.Yes ? true : false;

                    String destFolder = fsd.FileName;
                    Playlist p = (Playlist)listBox_playlists.SelectedItem;
                    ListPlus<Tuple<String, String>> lss = new ListPlus<Tuple<string, string>>();
                    foreach (FileSystemNodePlus<MyAddittionalData> n in  p.FileSystem.Flatten().Where(x=> x.Type==FileSystemNodePlusType.File))
                    {
                        FileSystemNodePlus<MyAddittionalData> t =IndexMediaLibrary.RootFileSystem.FindFirst((x) => { return x.AddittionalData.MD5 == n.AddittionalData.MD5; });
                        if(t==null)
                        {
                            //TODO: non trovato -> gestire la ricerca dei file non trovati
                        }
                        else
                        {
                            lss.Add(new Tuple<string, string>(IndexMediaLibrary.RootFileSystem.GetFullPath(t), SystemService.Combine(destFolder, SystemService.ChangeExtension(t.GetFullPath().TrimStart('\\', '/'), "mp3"))));
                        }
                    }
                    ConvertMedia CM = new ConvertMedia(new ConversionParameter(lss, ConversinType.SoloDiversi, FFmpegConversionEndFormat.mp3, OverrideIfExist));
                    CM.OnFFmpegConversionEnd += () =>
                    {
                        if (CM.Error != null && CM.Error.Count != 0)
                        {
                            MessageBox.Show("Errori presenti durante la conversione!");
                            //TODO: visualizzazione degli errori
                        }
                    };
                    CM.Show();
                    CM.Start();
                }
            }


            return;

            if (listBox_playlists.SelectedItem is Playlist)
            {
                FolderSelectDialog fsd = new FolderSelectDialog();
                if (fsd.ShowDialog())
                {
                    DialogResult dr = MessageBox.Show("Sovrascrivere eventuali file esistenti?\r\n\r\nSi: il processo Sovrascriverà eventuali file già presenti con lo stesso nome\r\nNo: Il processo VERRA' comunque avviato ma, se un file è già presente nella cartella di Output, questo non verrà sovrascritto\r\nAnnulla: Il processo verrà interrotto", "Conferma Sovrascrittura", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation);
                    if (dr == DialogResult.Cancel)
                        return;
                    bool OverrideIfExist = dr == DialogResult.Yes ? true : false;

                    String destFolder = fsd.FileName;
                    Playlist p = (Playlist)listBox_playlists.SelectedItem;
                    ListPlus<String> ls = p.FileSystem.GetAllFilePath();
                    
                    ListPlus<Tuple<String, String>> lss = new ListPlus<Tuple<string, string>>();
                    foreach (String s in ls)
                    {    
                        lss.Add(new Tuple<string, string>(SystemService.Combine(p.FileSystem.RootPath, s.TrimStart('\\', '/')), SystemService.Combine(destFolder, SystemService.ChangeExtension(s.TrimStart('\\', '/'), "mp3"))));
                    }
                    
                    ConvertMedia CM = new ConvertMedia(new ConversionParameter(lss, ConversinType.SoloDiversi, FFmpegConversionEndFormat.mp3, OverrideIfExist));
                    CM.OnFFmpegConversionEnd += () =>
                    {
                        if (CM.Error != null && CM.Error.Count != 0)
                        {
                            MessageBox.Show("Errori presenti durante la conversione!");
                            //TODO: visualizzazione degli errori
                        }
                    };
                    CM.Show();
                    CM.Start();
                }
            }




        }




        private void incorporaMetadataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listBox_playlists.SelectedItem is Playlist)
                if (MessageBox.Show("Attenzione!\r\nSe un file non viene trovato NON verrà eliminato dalla lista ma i metadati rimmarrano vuoti\r\nTip: Esegui prima \"Check dei File\"\r\n\r\nIl processo può richiedere paracchio tempo, continuare?","Attenzione",MessageBoxButtons.YesNo,MessageBoxIcon.Exclamation) == DialogResult.Yes)
                {
                    Playlist p = (Playlist)listBox_playlists.SelectedItem;
                    IncorporaMetadataPlaylist IM = new IncorporaMetadataPlaylist(p);
                    IM.OnIncorporaMetadataPlaylistFormEnd += (Playlist pp, IncorporaMetadataFormResult Result) =>
                    {
                        if (Result == IncorporaMetadataFormResult.Finish)
                        {
                            if(pp!=null)
                                SavePlaylist(pp);
                        }
                        else
                        {
                            if (pp != null)
                                ReloadPlaylistFromFile(pp);
                        }
                    };
                    IM.Show();
                    //IM.Start();
                    //IM.ShowDialog();
                }
        }



        private void CheckDeiFiletoolStripMenuItem1_Click(object sender, EventArgs e)
        {
            //TODO: verificare per ogni file se esiste, se non viene trovato salvare ogni nodo in un nodo temporaneo
            //TODO: i nodi che hanno i metadata li sfoglio ed in base all'MD5 controllo se trovo qualcosa nella libreria ( per ora solo ricerca come MD5 )
            //TODO: cerco di correggere i nodi errati e mostro un riepilogo 

        }


        //esporta singola cartella Originale
        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            if (listBox_playlists.SelectedItem is Playlist)
            {
                FolderSelectDialog fsd = new FolderSelectDialog();
                if (fsd.ShowDialog())
                {
                    DialogResult dr = MessageBox.Show("Sovrascrivere eventuali file esistenti?\r\n\r\nSi: il processo Sovrascriverà eventuali file già presenti con lo stesso nome\r\nNo: Il processo VERRA' comunque avviato ma, se un file è già presente nella cartella di Output, questo non verrà sovrascritto\r\nAnnulla: Il processo verrà interrotto", "Conferma Sovrascrittura", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation);
                    if (dr == DialogResult.Cancel)
                        return;

                    bool OverrideIfExist = dr == DialogResult.Yes ? true : false;
                    String destFolder = fsd.FileName;
                    Playlist p = (Playlist)listBox_playlists.SelectedItem;
                    ListPlus<String> ls = p.FileSystem.GetAllFilePath();

                    ListPlus<Tuple<String, String>> lss = new ListPlus<Tuple<string, string>>();
                    foreach (String s in ls)
                    {
                        String t= s.TrimStart('\\', '/');
                        String destFileName = SystemService.GetFileName(t);
                        t = SystemService.Combine(destFolder, destFileName);

                        while (lss.Count(x => x.Item2 ==t) > 0)
                        {
                            destFileName = "_" + destFileName;
                            t = SystemService.Combine(destFolder, destFileName);
                        }

                        lss.Add(new Tuple<string, string>(SystemService.Combine(p.FileSystem.RootPath, s.TrimStart('\\', '/')), t));
                    }


                    ConvertMedia CM = new ConvertMedia(new ConversionParameter(lss, ConversinType.Mai, FFmpegConversionEndFormat.mp3, OverrideIfExist));
                    CM.OnFFmpegConversionEnd += () =>
                    {
                        if (CM.Error != null && CM.Error.Count != 0)
                        {
                            MessageBox.Show("Errori presenti durante la conversione!");
                            //TODO: visualizzazione degli errori
                        }
                    };
                    CM.Show();
                    CM.Start();
                }
            }
        }



        //esporta singola cartella Convertito
        private void toolStripMenuItem4_Click(object sender, EventArgs e)
        {
            if (listBox_playlists.SelectedItem is Playlist)
            {
                FolderSelectDialog fsd = new FolderSelectDialog();
                if (fsd.ShowDialog())
                {
                    DialogResult dr = MessageBox.Show("Sovrascrivere eventuali file esistenti?\r\n\r\nSi: il processo Sovrascriverà eventuali file già presenti con lo stesso nome\r\nNo: Il processo VERRA' comunque avviato ma, se un file è già presente nella cartella di Output, questo non verrà sovrascritto\r\nAnnulla: Il processo verrà interrotto", "Conferma Sovrascrittura", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation);
                    if (dr == DialogResult.Cancel)
                        return;
                    bool OverrideIfExist = dr == DialogResult.Yes ? true : false;

                    String destFolder = fsd.FileName;
                    Playlist p = (Playlist)listBox_playlists.SelectedItem;
                    ListPlus<String> ls = p.FileSystem.GetAllFilePath();

                    ListPlus<Tuple<String, String>> lss = new ListPlus<Tuple<string, string>>();
                    foreach (String s in ls)
                    {
                        String t = SystemService.ChangeExtension(s.TrimStart('\\', '/'), "mp3");
                        String destFileName = SystemService.GetFileName(t);
                        t = SystemService.Combine(destFolder, destFileName);

                        while (lss.Count(x => x.Item2 == t) > 0)
                        {
                            destFileName = "_" + destFileName;
                            t = SystemService.Combine(destFolder, destFileName);
                        }
                        lss.Add(new Tuple<string, string>(SystemService.Combine(p.FileSystem.RootPath, s.TrimStart('\\', '/')), t));
                    }

                    ConvertMedia CM = new ConvertMedia(new ConversionParameter(lss, ConversinType.SoloDiversi, FFmpegConversionEndFormat.mp3, OverrideIfExist));
                    CM.OnFFmpegConversionEnd += () =>
                    {
                        if (CM.Error != null && CM.Error.Count != 0)
                        {
                            MessageBox.Show("Errori presenti durante la conversione!");
                            //TODO: visualizzazione degli errori
                        }
                    };
                    CM.Show();
                    CM.Start();
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Index_UpdateAndSave(GlobalVar.PathIndexFile, option.PathMedia, option.LoadMediaOption());
        }

        /* private void convertitoMP3ToolStripMenuItem_Click(object sender, EventArgs e)
         {
             if (listBox_playlists.SelectedItem is Playlist)
             {
                 Playlist p = (Playlist)listBox_playlists.SelectedItem;
                 CountSpace cs = new CountSpace(new CountParameter(p.FileSystem,FFmpegConversionEndFormat.mp3));
                 cs.Show();
                 cs.Start();
             }    
         }*/







    }




    public enum MainFormStatus
    {
        RootBrowsing,
        PlaylistBrowsing
    }



}
