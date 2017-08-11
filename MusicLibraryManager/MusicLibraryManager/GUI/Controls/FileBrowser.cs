using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ExtendCSharp;
using System.Text.RegularExpressions;
using ExtendCSharp.ExtendedClass;
using ExtendCSharp.Controls;

namespace MusicLibraryManager.GUI.Controls
{
    public partial class FileBrowser : UserControl
    {
        #region Variable

        FileBrowserStatus _Status = FileBrowserStatus.browsing;
        public FileBrowserStatus Status
        {
            get
            {
                return _Status;
            }
            set
            {
                _Status = value;
                ReloadCheckBoxPropriety();
            }
        }

        public int LastClicked = -1;

        FileSystemNodePlus<MyAddittionalData> CurrentNode = null;
        MyFileSystemPlus currentFileSystem = null;

        public FileBrowserType Type = FileBrowserType.Root;
        public List<Playlist> lp = null;


      

        #endregion

        public delegate void AddItemRequestEventHandler(Playlist p, MyFileSystemPlus f);
        public event AddItemRequestEventHandler AddItemRequest;

        public delegate void RemoveItemRequestEventHandler(MyFileSystemPlus ToRemoveSelect);
        public event RemoveItemRequestEventHandler RemoveItemRequest;

        public delegate void PlaylistChangedEventHandler(MyFileSystemPlus PlaylistSystem);
        public event PlaylistChangedEventHandler PlaylistChanged;


        public FileBrowser()
        {
            InitializeComponent();
        }

        public void Clear()
        {
            Controls.Clear();
        }
        public void LoadMyFileSystemPlus(MyFileSystemPlus mfsp)
        {
            currentFileSystem = mfsp;
            if(currentFileSystem!=null)
                LoadNode(currentFileSystem.Root);
            else
                LoadNode(null);
        }
        private void LoadNode(FileSystemNodePlus<MyAddittionalData> Node)
        {
            if (Node != null)
            {
                CurrentNode = Node;
                Controls.Clear();
                if (Node.Parent != null)
                {
                    AddComponent(Node.Parent, false, "..");
                }
                foreach (FileSystemNodePlus<MyAddittionalData> nd in Node.GetAllNode(FileSystemNodePlusType.Directory).OrderBy(x => x.Name))
                {
                    AddComponent(nd);
                }
                foreach (FileSystemNodePlus<MyAddittionalData> nf in Node.GetAllNode(FileSystemNodePlusType.File).OrderBy(x => x.Name))
                {
                    AddComponent(nf);
                }
                this.Focus();
            }
        }
        public void ReloadNode()
        {
            LoadNode(CurrentNode);
        }
        public void AddComponent(FileSystemNodePlus<MyAddittionalData> Component,bool Selectionable=true,String OverrideName=null)
        {
            SingleFile s = new SingleFile(Component);
            s.Location = new Point(0, s.Height * Controls.Count);
            s.Width = Width;
            s.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            s.OnSingleFileDoubleClick += S_OnSingleFileDoubleClick;
            s.OnSingleFileRightClick += S_OnSingleFileRightClick;
            s.OnSingleFileSelectChange += S_OnSingleFileSelectChange;
            s.OnSingleFileMouseDown += S_OnSingleFileMouseDown;
            s.OnSingleFileMouseUp += S_OnSingleFileMouseUp;
            s.OnSingleFileMouseMove += S_OnSingleFileMouseMove;
            s.OnSingleFileNodoChangeName += S_OnSingleFileNodoChangeName;
            s.OnSingleFileKeyDown += S_OnSingleFileKeyDown;


            if(OverrideName!=null)
                s.ShowedName = OverrideName;
            
            if (_Status == FileBrowserStatus.browsing)
                s.sc.ShowCheckBox = false;
            else if (_Status == FileBrowserStatus.Select)
                s.sc.ShowCheckBox = true;

            s.sc.Selectionable = Selectionable;


            Controls.Add(s);
        }

        

        public SingleFile GetSingleFileFromNode(FileSystemNodePlus<MyAddittionalData> Node)
        {
            foreach(SingleFile s in Controls)
            {
                if (s.Nodo == Node)
                    return s;
            }
            return null;
        }


        private void S_OnSingleFileNodoChangeName(SingleFile Sender, FileSystemNodePlus<MyAddittionalData> Nodo, string NewName)
        {
            if (Nodo.Parent == null)
                Sender.Status = SingleFileStatus.Normal;
            else
            {
                FileSystemNodePlus<MyAddittionalData> parent = Nodo.Parent;
                FileSystemNodePlus<MyAddittionalData> nt = Nodo.Clone(NewName);
                parent.Remove((x) => { return x == Nodo; }, FileSystemNodePlusLevelType.FirstLevel, FileSystemNodePlusControlType.Pre);
                parent.Add(nt);
                ReloadNode();
                if (PlaylistChanged != null)
                    PlaylistChanged(currentFileSystem);
            }
        }

        FileSystemNodePlus<MyAddittionalData> DownNode = null;
        SingleFile DownSingleFile = null;
        private void S_OnSingleFileMouseDown(SingleFile SF)
        {
            if (Type == FileBrowserType.Playlist)
            {
                DownSingleFile = SF;
                DownNode = SF.Nodo;
                Cursor.Current = Cursors.Cross;
            }
        }
        private void S_OnSingleFileMouseUp(SingleFile SF)
        {
            if (Type == FileBrowserType.Playlist)
            {
                Cursor.Current = Cursors.Default;

                if (DownNode != SF.Nodo)
                {
                    if (SF.Nodo.Type == FileSystemNodePlusType.Directory)
                    {

                        String path = CurrentNode.GetFullPath();
                        MyFileSystemPlus Clo = currentFileSystem.Clone();
                        FileSystemNodePlus<MyAddittionalData> nn = Clo.GetNodeFromPath(path);
                        
                        MyFileSystemPlus temp = new MyFileSystemPlus();
                        temp.Root = nn;
                        temp = temp.FindPreserveTree((x) => { return x.AddittionalData.Selezionato; }, FileSystemNodePlusControlType.Pre);
                        CurrentNode.Remove((t) => { return t.Type == FileSystemNodePlusType.File && t.AddittionalData.Selezionato; }, FileSystemNodePlusLevelType.AllNode, FileSystemNodePlusControlType.Pre);
                        CurrentNode.Remove((t) => { return t.Type == FileSystemNodePlusType.Directory && t.ChildCount == 0 && t.AddittionalData.Selezionato; }, FileSystemNodePlusLevelType.AllNode, FileSystemNodePlusControlType.Post);
                        temp.DeselectAll();
                        SF.Nodo.Merge(temp.Root);
                        SF.Selected = false;
                        ReloadNode();

                        if (PlaylistChanged != null)
                            PlaylistChanged(currentFileSystem);
                    }
                    
                }
                DownNode = null;
            }
            
        }
        private void S_OnSingleFileMouseMove(SingleFile SF)
        {
            
        }

        

       

        public void ReloadCheckBoxPropriety()
        {
            if (_Status == FileBrowserStatus.browsing)
                ChekBoxVisible(false);
            else if (_Status == FileBrowserStatus.Select)
                ChekBoxVisible(true);
        }
        public void ChekBoxVisible(bool Status)
        {
            if(Status)
            {
                foreach (Control c in Controls)
                    if (c is SingleFile)
                    {
                        SingleFile ss = c._Cast<SingleFile>();
                        if (ss.sc.Selectionable)
                            c._Cast<SingleFile>().sc.ShowCheckBox = true;
                    }
            }
            else
                foreach (Control c in Controls)
                    if (c is SingleFile)
                        c._Cast<SingleFile>().sc.ShowCheckBox = false;
                    
        }


        private void ApricontextMenuStrip(Point position)
        {
            if (_Status == FileBrowserStatus.browsing)
                sceltaCheckBoxToolStripMenuItem.Text = "Modalità Scelta";
            else if(_Status == FileBrowserStatus.Select)
                    sceltaCheckBoxToolStripMenuItem.Text = "Modalità Naviga";

            if (Type == FileBrowserType.Root)
            {
                creaCartellaToolStripMenuItem.Enabled = false;
                mergeEdEstraiToolStripMenuItem.Enabled = false;
                mergeToolStripMenuItem.Enabled = false;
                rinominaToolStripMenuItem.Enabled = false;
                aggiungiRimuoviToolStripMenuItem.Text = "Aggiungi a";
                aggiungiRimuoviToolStripMenuItem.DropDownItems.Clear();
                foreach (Playlist p in lp)
                {
                    ToolStripMenuItemPlus t = new ToolStripMenuItemPlus(p);
                    t.Click += (object sender, EventArgs e) =>
                    {
                        if (AddItemRequest != null)
                        {
                            AddItemRequest(sender._Cast<ToolStripMenuItemPlus>().TextObject._Cast<Playlist>(), currentFileSystem);
                        }
                        ReloadNode();
                    };
                    aggiungiRimuoviToolStripMenuItem.DropDownItems.Add(t);
                }
            }
            else if (Type == FileBrowserType.Playlist)
            {
                creaCartellaToolStripMenuItem.Enabled = true;
                mergeEdEstraiToolStripMenuItem.Enabled = true;
                mergeToolStripMenuItem.Enabled = true;
                rinominaToolStripMenuItem.Enabled = true;
                aggiungiRimuoviToolStripMenuItem.Text = "Rimuovi";
                aggiungiRimuoviToolStripMenuItem.DropDownItems.Clear();
                aggiungiRimuoviToolStripMenuItem.Click -= AggiungiRimuoviToolStripMenuItem_Click;
                aggiungiRimuoviToolStripMenuItem.Click += AggiungiRimuoviToolStripMenuItem_Click;
                


            }
            contextMenuStrip1.Show(Cursor.Position);
        }

        private void AggiungiRimuoviToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Type == FileBrowserType.Playlist)
                if (RemoveItemRequest != null)
                {
                    RemoveItemRequest(currentFileSystem);
                }
        }

        private void sceltaCheckBoxToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_Status == FileBrowserStatus.browsing)
                Status = FileBrowserStatus.Select;
            else if (_Status == FileBrowserStatus.Select)
                Status = FileBrowserStatus.browsing;
        }
        private void deselezionaTuttoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (SingleFile sf in Controls.OfType<SingleFile>())
            {
                if(!sf.EqualNodo(CurrentNode.Parent))
                    sf.sc.Select = false;
            }
            
        }

        private void S_OnSingleFileDoubleClick(SingleFile Sender,FileSystemNodePlus<MyAddittionalData> Nodo)
        {
            if (Nodo.Type == FileSystemNodePlusType.Directory)
            {
                if (Sender.ShowedName == "..")
                {
                    String s = CurrentNode.Name;
                    LoadNode(Nodo);
                    ScrollTo("^" + s);
                }
                else
                {
                    LoadNode(Nodo);
                }
                
                
            }

        }
        private void S_OnSingleFileRightClick(FileSystemNodePlus<MyAddittionalData> Nodo)
        {
            ApricontextMenuStrip(Cursor.Position);
        }
        private void FileBrowser_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                ApricontextMenuStrip(Cursor.Position);
        }
        private void S_OnSingleFileSelectChange(FileSystemNodePlus<MyAddittionalData> Nodo, Keys Modificatore)
        {
            if(Modificatore!=Keys.Shift)
                LastClicked = Controls.IndexOf(Controls.OfType<SingleFile>().Where(sf => sf.EqualNodo(Nodo)).First());
            else
            {
                int NowClicked= Controls.IndexOf(Controls.OfType<SingleFile>().Where(sf => sf.EqualNodo(Nodo)).First());
                int max = LastClicked > NowClicked ? LastClicked : NowClicked;
                for (int i=LastClicked<NowClicked?LastClicked:NowClicked;i<max;i++)
                {
                    if (Controls[i] is SingleFile)
                        Controls[i]._Cast<SingleFile>().sc.Select = true;
                }
                LastClicked = NowClicked;

            }
        }

        

        private void creaCartellaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Type == FileBrowserType.Playlist)
            {
                if(CurrentNode!=null)
                {
                    
                    String Base = "Nuova Cartella ";
                    int n =0;
                    while (CurrentNode.FindAll((x) => { return x.Name == Base + n; }, FileSystemNodePlusLevelType.FirstLevel, FileSystemNodePlusControlType.Pre).Count > 0)
                        n++;

                    FileSystemNodePlus<MyAddittionalData> NodoT = CurrentNode.CreateNode(Base + n, FileSystemNodePlusType.Directory);
                    ReloadNode();
                    if (PlaylistChanged != null)
                        PlaylistChanged(currentFileSystem);

                    SingleFile s = GetSingleFileFromNode(NodoT);
                    if (s != null)
                        s.Status = SingleFileStatus.Rename;
                }

                    
            }
        }

        private void mergeToolStripMenuItem_Click(object sender, EventArgs e)
        {

            List<FileSystemNodePlus<MyAddittionalData>> ListaFileDir = new List<FileSystemNodePlus<MyAddittionalData>>();
            FileSystemNodePlus<MyAddittionalData> clonato = CurrentNode.Clone();
            clonato.Remove((t) => { return t.Type == FileSystemNodePlusType.File && !t.AddittionalData.Selezionato; }, FileSystemNodePlusLevelType.AllNode, FileSystemNodePlusControlType.Pre);
            clonato.Remove((t) => { return t.Type == FileSystemNodePlusType.Directory && t.ChildCount == 0 && !t.AddittionalData.Selezionato; }, FileSystemNodePlusLevelType.AllNode, FileSystemNodePlusControlType.Post);
            clonato.FindAll((x) => { return x.AddittionalData.Selezionato; }, FileSystemNodePlusLevelType.FirstLevel, FileSystemNodePlusControlType.Pre).ForEach((x) => { ListaFileDir.Add(x.Clone()); });
            


            CurrentNode.Remove((t) => { return t.Type == FileSystemNodePlusType.File && t.AddittionalData.Selezionato; }, FileSystemNodePlusLevelType.AllNode, FileSystemNodePlusControlType.Pre);
            CurrentNode.Remove((t) => { return t.Type == FileSystemNodePlusType.Directory && t.ChildCount == 0 && t.AddittionalData.Selezionato; }, FileSystemNodePlusLevelType.AllNode, FileSystemNodePlusControlType.Post);

            String Base = "Nuova Cartella ";
            int n = 0;
            while (CurrentNode.FindAll((x) => { return x.Name == Base + n; }, FileSystemNodePlusLevelType.FirstLevel, FileSystemNodePlusControlType.Pre).Count > 0)
                n++;

            FileSystemNodePlus<MyAddittionalData> NodoT = CurrentNode.CreateNode(Base + n, FileSystemNodePlusType.Directory);
            


            foreach (FileSystemNodePlus<MyAddittionalData> f in ListaFileDir)
            {
                if (f.Type == FileSystemNodePlusType.File)
                {
                    NodoT.Add(f);
                }
                else if (f.Type == FileSystemNodePlusType.Directory)
                {
                    NodoT.Merge(f);
                }
                else
                {
                    throw new Exception("Type non trovato");
                }
            }
            ReloadNode();
            if (PlaylistChanged != null)
                PlaylistChanged(currentFileSystem);


            SingleFile s = GetSingleFileFromNode(NodoT);
            if(s!=null)
                s.Status = SingleFileStatus.Rename;



        }

        private void mergeEdEstraiToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<FileSystemNodePlus<MyAddittionalData>> ListaFileDir = new List<FileSystemNodePlus<MyAddittionalData>>();
            FileSystemNodePlus<MyAddittionalData> clonato = CurrentNode.Clone();
            clonato.Remove((t) => { return t.Type == FileSystemNodePlusType.File && !t.AddittionalData.Selezionato; }, FileSystemNodePlusLevelType.AllNode, FileSystemNodePlusControlType.Pre);
            clonato.Remove((t) => { return t.Type == FileSystemNodePlusType.Directory && t.ChildCount == 0 && !t.AddittionalData.Selezionato; }, FileSystemNodePlusLevelType.AllNode, FileSystemNodePlusControlType.Post);
            clonato.FindAll((x) => { return x.AddittionalData.Selezionato; }, FileSystemNodePlusLevelType.FirstLevel, FileSystemNodePlusControlType.Pre).ForEach((x) => { ListaFileDir.Add(x.Clone()); });


            

            CurrentNode.Remove((t) => { return t.Type == FileSystemNodePlusType.File && t.AddittionalData.Selezionato; }, FileSystemNodePlusLevelType.AllNode, FileSystemNodePlusControlType.Pre);
            CurrentNode.Remove((t) => { return t.Type == FileSystemNodePlusType.Directory && t.ChildCount == 0 && t.AddittionalData.Selezionato; }, FileSystemNodePlusLevelType.AllNode, FileSystemNodePlusControlType.Post);


            foreach(FileSystemNodePlus<MyAddittionalData> f in ListaFileDir)
            {
                if(f.Type==FileSystemNodePlusType.File)
                {
                    CurrentNode.Add(f);
                }
                else if (f.Type == FileSystemNodePlusType.Directory)
                {
                    CurrentNode.Merge(f);
                }
                else
                {
                    throw new Exception("Type non trovato");
                }
            }
            ReloadNode();
            if (PlaylistChanged != null)
                PlaylistChanged(currentFileSystem);
        }

        private void rinominaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(DownSingleFile!=null)
                DownSingleFile.Status = SingleFileStatus.Rename;
        }



        /// <summary>
        /// Scrolla nel punto in cui la Regex viene soddisfatta
        /// </summary>
        /// <param name="Pattern"></param>
        public void ScrollTo(String PatternRegex, RegexOptions o= RegexOptions.None)
        {
            Regex r = new Regex(PatternRegex,o);

            foreach (Control c in Controls)
            {
                if(c is SingleFile)
                {
                    if(r.IsMatch((c as SingleFile).Nodo.Name))
                    {
                        ScrollTo(c.Location);
                        break;
                    }
                }
            }
        }
        /// <summary>
        /// Scrolla nel punto specificato
        /// </summary>
        /// <param name="p"></param>
        public void ScrollTo(Point p)
        {
            AutoScrollPosition = p.Sub(AutoScrollPosition);
        }



        private void S_OnSingleFileKeyDown(object sender, KeyEventArgs e)
        {
            FileBrowser_KeyDown(sender, e);
        }

        String CurrentSearchString = "";
        TimeSpanPlus CurrentTime = null;
        private void FileBrowser_KeyDown(object sender, KeyEventArgs e)
        {
            if (CurrentTime == null)
            {
                CurrentSearchString = "^"+e.KeyCode.ToString();
                CurrentTime = TimeSpanPlus.Now;
            }
            else
            {
                TimeSpanPlus Old = CurrentTime;
                CurrentTime= TimeSpanPlus.Now;
                if ((CurrentTime - Old).TotalSeconds < 2)
                {
                    CurrentSearchString += e.KeyCode.ToString();
                }
                else
                {
                    CurrentSearchString = "^" + e.KeyCode.ToString();
                }
            }

            ScrollTo(CurrentSearchString,RegexOptions.IgnoreCase);
        }



        protected override System.Drawing.Point ScrollToControl(Control activeControl)
        {
            return DisplayRectangle.Location;
        }
    }

    public enum FileBrowserStatus
    {
        Select,
        browsing,
    }


    public enum FileBrowserType
    {
        Root,
        Playlist,
    }
}
