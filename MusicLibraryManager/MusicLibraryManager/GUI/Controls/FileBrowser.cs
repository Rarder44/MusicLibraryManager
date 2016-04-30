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

            if(OverrideName!=null)
                s.ShowedName = OverrideName;
            
            if (_Status == FileBrowserStatus.browsing)
                s.sc.ShowCheckBox = false;
            else if (_Status == FileBrowserStatus.Select)
                s.sc.ShowCheckBox = true;

            s.sc.Selectionable = Selectionable;


            Controls.Add(s);
        }

        FileSystemNodePlus<MyAddittionalData> DownNode = null;
        private void S_OnSingleFileMouseDown(FileSystemNodePlus<MyAddittionalData> Nodo)
        {
            DownNode = Nodo;
            Cursor.Current = Cursors.Cross;
        }
        private void S_OnSingleFileMouseUp(FileSystemNodePlus<MyAddittionalData> Nodo)
        {
            
            Cursor.Current = Cursors.Default;
            MessageBox.Show(DownNode + " " + Nodo);
        }
        private void S_OnSingleFileMouseMove(FileSystemNodePlus<MyAddittionalData> Nodo)
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
                    };
                    aggiungiRimuoviToolStripMenuItem.DropDownItems.Add(t);
                }
            }
            else if (Type == FileBrowserType.Playlist)
            {
                creaCartellaToolStripMenuItem.Enabled = true;
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

        private void S_OnSingleFileDoubleClick(FileSystemNodePlus<MyAddittionalData> Nodo)
        {
            if (Nodo.Type == FileSystemNodePlusType.Directory)
                LoadNode(Nodo);
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
                    CurrentNode.Add(new FileSystemNodePlus<MyAddittionalData>(Base+n, FileSystemNodePlusType.Directory, CurrentNode));
                    ReloadNode();
                }

                    
            }
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
