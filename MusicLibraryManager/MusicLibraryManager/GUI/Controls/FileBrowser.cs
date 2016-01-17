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
        public FileBrowser()
        {
            InitializeComponent();
        }


        public void AddComponent(MyFileSystemNode<MyAddittionalDataMyFileSystem> Component,bool Selectionable=true,String OverrideName=null)
        {
            SingleFile s = new SingleFile(Component);
            s.Location = new Point(0, s.Height * Controls.Count);
            s.Width = Width;
            s.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            s.OnSingleFileDoubleClick += S_OnSingleFileDoubleClick;
            if(OverrideName!=null)
                s.ShowedName = OverrideName;
            s.Selectionable = Selectionable;
            Controls.Add(s);
        }

        private void S_OnSingleFileDoubleClick(MyFileSystemNode<MyAddittionalDataMyFileSystem> Nodo)
        {
            if(Nodo.Type==MyFileSystemNodeType.Directory)
                LoadNode(Nodo);
        }




        public void LoadNode(MyFileSystemNode<MyAddittionalDataMyFileSystem> Node)
        {
            Controls.Clear();
            if (Node.Parent!=null)
            {
                AddComponent(Node.Parent, false, "..");
            }
            foreach(MyFileSystemNode<MyAddittionalDataMyFileSystem> nd in Node.GetAllNode(MyFileSystemNodeType.Directory))
            {
                AddComponent(nd);
            }
            foreach (MyFileSystemNode<MyAddittionalDataMyFileSystem> nf in Node.GetAllNode(MyFileSystemNodeType.File))
            {
                AddComponent(nf);
            }
            this.Focus();
        }




    }
}
