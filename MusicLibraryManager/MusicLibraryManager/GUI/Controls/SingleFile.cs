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
using System.Drawing.Drawing2D;
using ExtendCSharp.Services;
using System.IO;

namespace MusicLibraryManager.GUI.Controls
{
    public partial class SingleFile : UserControl
    {
        //TODO: controllo e migliore gestione del nodo e della selezione ( controllo unico per selezionare/deselezionare grafica e nodo )
        #region Variabili

        public bool ShowCheckBox {
            get { return checkBox1.Visible; }
            set
            {
                if(Selectionable)
                    checkBox1.Visible = value;
                else
                    checkBox1.Visible = false;
            }
        }

        bool _Selectionable = true;
        public bool Selectionable
        {
            get { return _Selectionable; }
            set
            {
                _Selectionable = value;
                if(!_Selectionable)
                {
                    checkBox1.MouseClick -= checkBox1_MouseClick;
                    checkBox1.Checked = false;
                    _Status = SingleFileStatus.NotSelected;
                    checkBox1.MouseClick += checkBox1_MouseClick;
                }
            }
        }

        SingleFileStatus _Status = SingleFileStatus.NotSelected;
        public SingleFileStatus Status
        {
            get { return _Status; }
            set
            {
                if (!Selectionable)
                {
                    checkBox1.MouseClick -= checkBox1_MouseClick;
                    checkBox1.Checked = false;
                    Nodo.AddittionalData.Selezionato = false;
                    _Status = SingleFileStatus.NotSelected;
                    checkBox1.MouseClick += checkBox1_MouseClick;
                    return;
                }
                   
                if (_Status == value)
                    return;

                
                _Status = value;
                
                if (_Status == SingleFileStatus.Selected)
                {
                    checkBox1.MouseClick -= checkBox1_MouseClick;
                    checkBox1.Checked = true;
                    checkBox1.MouseClick += checkBox1_MouseClick;
                }
                else if (_Status == SingleFileStatus.NotSelected)
                {
                    checkBox1.MouseClick -= checkBox1_MouseClick;
                    checkBox1.Checked = false;
                    checkBox1.MouseClick += checkBox1_MouseClick;
                }

                Invalidate();
            }
        } 

        MyFileSystemNode<MyAddittionalDataMyFileSystem> Nodo;

        public String RealName { get { return Nodo.Name; } }
        public String ShowedName
        {
            get { return label1.Text; }
            set { label1.SetTextInvoke(value); }
        }

        #endregion

        public bool EqualNodo(MyFileSystemNode<MyAddittionalDataMyFileSystem> n)
        {
            return n == Nodo;
        }

        #region Delegati ed Eventi

        public delegate void SingleFileDoubleClick(MyFileSystemNode<MyAddittionalDataMyFileSystem> Nodo);
        public event SingleFileDoubleClick OnSingleFileDoubleClick;

        public delegate void SingleFileRightClick(MyFileSystemNode<MyAddittionalDataMyFileSystem> Nodo);
        public event SingleFileRightClick OnSingleFileRightClick;

        public delegate void SingleFileSelectChange(MyFileSystemNode<MyAddittionalDataMyFileSystem> Nodo, Keys Modificatore);
        public event SingleFileSelectChange OnSingleFileSelectChange;

        #endregion

        #region Costruttori

        public SingleFile(MyFileSystemNode<MyAddittionalDataMyFileSystem> Nodo)
        {
            InitializeComponent();

            this.Nodo = Nodo;

            if (Nodo.Type == MyFileSystemNodeType.Directory)
                this.Icon.BackgroundImage = global::MusicLibraryManager.Properties.Resources.folder;
            else if (Nodo.Type == MyFileSystemNodeType.File)
            {
                Icon icon = RegisteredFileType.GetIconFromExtension(Path.GetExtension(Nodo.Name));
                if (icon != null)
                    this.Icon.BackgroundImage = icon.ToBitmap();
            }

            
            label1.Text = Nodo.ToString();


            this.MouseClick += SingleFile_MouseClick;
            label1.MouseClick += SingleFile_MouseClick;
            Icon.MouseClick += SingleFile_MouseClick;

            this.MouseClick += SingleFile_RightClickCheck;
            label1.MouseClick += SingleFile_RightClickCheck;
            Icon.MouseClick += SingleFile_RightClickCheck;
            checkBox1.MouseClick += SingleFile_RightClickCheck;


            this.MouseDoubleClick += SingleFile_MouseDoubleClick;
            label1.MouseDoubleClick += SingleFile_MouseDoubleClick;
            Icon.MouseDoubleClick += SingleFile_MouseDoubleClick;

            
            Status = Nodo.AddittionalData.Selezionato ? SingleFileStatus.Selected : SingleFileStatus.NotSelected;
        }

        #endregion

        #region Override

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams parms = base.CreateParams;
                parms.ExStyle |= 0x20;
                return parms;
            }
        }
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            if (_Status == SingleFileStatus.Selected)
            {
                Rectangle rc = new Rectangle(0, 0, this.ClientSize.Width, this.ClientSize.Height);
                using (LinearGradientBrush brush = new LinearGradientBrush(rc, Color.FromArgb(220,235,252), Color.FromArgb(193, 219, 252), 90))
                {
                    e.Graphics.FillRectangle(brush, rc);
                }
            }
            else if (_Status == SingleFileStatus.NotSelected)
            {
                Rectangle rc = new Rectangle(0, 0, this.ClientSize.Width, this.ClientSize.Height);
                e.Graphics.FillRectangle(Brushes.White, rc);
            }
        }

        #endregion

        #region Handler

        private void SingleFile_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if(e.Button==MouseButtons.Left && OnSingleFileDoubleClick != null)
                OnSingleFileDoubleClick(Nodo);
        }
        private void SingleFile_MouseClick(object sender, MouseEventArgs e)
        {

        }
        private void SingleFile_RightClickCheck(object sender, MouseEventArgs e)
        {
            if(e.Button==MouseButtons.Right && OnSingleFileRightClick!=null)
            {
                if(sender is SingleFile)
                    OnSingleFileRightClick(sender._Cast<SingleFile>().Nodo);
                else
                    if(sender._Cast<Control>().Parent is SingleFile)
                        OnSingleFileRightClick(sender._Cast<Control>().Parent._Cast<SingleFile>().Nodo);
            }
                
        }
        private void checkBox1_MouseClick(object sender, MouseEventArgs e)
        {
            Keys k = Control.ModifierKeys;
            Status = checkBox1.Checked ? SingleFileStatus.Selected : SingleFileStatus.NotSelected;
            SetSelectChildNode(checkBox1.Checked);
            //TODO: SetSelectParentNode -> ma va controllato il seleziona/deseleziona ( due regole diverse: true= abilita "sempre" false= controlla se ci sono altri child prima di disabilitare
            if (OnSingleFileSelectChange != null)
                OnSingleFileSelectChange(Nodo, k);
        }


        #endregion

        #region Dispatcher

        public void SetSelectParentNode(bool Value)
        {
            if (Nodo.Parent != null)
                SetSelectParentNodeRecursive(Nodo, Value);
        }
        public void SetSelectParentNodeRecursive(MyFileSystemNode<MyAddittionalDataMyFileSystem> Node, bool Value)
        {
            Node.AddittionalData.Selezionato = Value;
            if (Nodo.Parent != null)
            {
                if (Value)
                    SetSelectChildNodeRecursive(Node.Parent, true);
                else
                {
                    foreach (MyFileSystemNode<MyAddittionalDataMyFileSystem> n in Nodo.Parent.GetAllNode())
                        if (n.AddittionalData.Selezionato)
                            return;

                    SetSelectChildNodeRecursive(Node.Parent, false);
                }
            }             
        }


        public void SetSelectChildNode(bool Value )
        {
            SetSelectChildNodeRecursive(Nodo, Value);
        }
        public void SetSelectChildNodeRecursive(MyFileSystemNode<MyAddittionalDataMyFileSystem> Node, bool Value)
        {
            Node.AddittionalData.Selezionato = Value;
            foreach(MyFileSystemNode<MyAddittionalDataMyFileSystem> n in Node.GetAllNode())
                SetSelectChildNodeRecursive(n, Value);
            
        }
        #endregion

     
    }
    public enum SingleFileStatus
    {
        Selected,
        NotSelected
    }

}
