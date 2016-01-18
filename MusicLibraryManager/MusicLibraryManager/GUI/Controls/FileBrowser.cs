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

        public FileBrowser()
        {
            InitializeComponent();
        }


        public void LoadNode(MyFileSystemNode<MyAddittionalDataMyFileSystem> Node)
        {
            Controls.Clear();
            if (Node.Parent != null)
            {
                AddComponent(Node.Parent, false, "..");
            }
            foreach (MyFileSystemNode<MyAddittionalDataMyFileSystem> nd in Node.GetAllNode(MyFileSystemNodeType.Directory))
            {
                AddComponent(nd);
            }
            foreach (MyFileSystemNode<MyAddittionalDataMyFileSystem> nf in Node.GetAllNode(MyFileSystemNodeType.File))
            {
                AddComponent(nf);
            }
            this.Focus();
        }
        public void AddComponent(MyFileSystemNode<MyAddittionalDataMyFileSystem> Component,bool Selectionable=true,String OverrideName=null)
        {
            SingleFile s = new SingleFile(Component);
            s.Location = new Point(0, s.Height * Controls.Count);
            s.Width = Width;
            s.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            s.OnSingleFileDoubleClick += S_OnSingleFileDoubleClick;
            s.OnSingleFileRightClick += S_OnSingleFileRightClick;
            s.OnSingleFileSelectChange += S_OnSingleFileSelectChange;

            if(OverrideName!=null)
                s.ShowedName = OverrideName;
            s.Selectionable = Selectionable;

            if (_Status == FileBrowserStatus.browsing)
                s.ShowCheckBox = false;
            else if (_Status == FileBrowserStatus.Select)
                s.ShowCheckBox = true;


            Controls.Add(s);
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
            foreach(Control c in Controls)
                if (c is SingleFile)
                    c._Cast<SingleFile>().ShowCheckBox = Status; 
        }


        private void ApricontextMenuStrip(Point position)
        {
            if (_Status == FileBrowserStatus.browsing)
                sceltaCheckBoxToolStripMenuItem.Text = "Modalità Scelta";
            else if(_Status == FileBrowserStatus.Select)
                    sceltaCheckBoxToolStripMenuItem.Text = "Modalità Naviga";
            contextMenuStrip1.Show(Cursor.Position);
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
            //TODO: controllo e miglioro deselezione
            foreach (SingleFile sf in Controls.OfType<SingleFile>())
            {
                sf.Status = SingleFileStatus.NotSelected;
                sf.SetSelectChildNode(false);
            }
                    
        }

        private void S_OnSingleFileDoubleClick(MyFileSystemNode<MyAddittionalDataMyFileSystem> Nodo)
        {
            if (Nodo.Type == MyFileSystemNodeType.Directory)
                LoadNode(Nodo);
        }
        private void S_OnSingleFileRightClick(MyFileSystemNode<MyAddittionalDataMyFileSystem> Nodo)
        {
            ApricontextMenuStrip(Cursor.Position);
        }
        private void FileBrowser_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                ApricontextMenuStrip(Cursor.Position);
        }
        private void S_OnSingleFileSelectChange(MyFileSystemNode<MyAddittionalDataMyFileSystem> Nodo, Keys Modificatore)
        {
            if(Modificatore!=Keys.Shift)
                LastClicked = Controls.IndexOf(Controls.OfType<SingleFile>().Where(sf => sf.EqualNodo(Nodo)).First());
            else
            {
                //TODO: selezione con modificatore
            }
        }

       
    }

    public enum FileBrowserStatus
    {
        Select,
        browsing,
    }
}
