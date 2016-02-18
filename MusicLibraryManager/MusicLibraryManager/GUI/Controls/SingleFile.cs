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
        #region Variabili

        public SelectControl sc = null;

        FileSystemNodePlus<MyAddittionalData> Nodo;

        public String RealName { get { return Nodo.Name; } }
        public String ShowedName
        {
            get { return label1.Text; }
            set { label1.SetTextInvoke(value); }
        }

        #endregion

        public bool EqualNodo(FileSystemNodePlus<MyAddittionalData> n)
        {
            return n == Nodo;
        }

        #region Delegati ed Eventi

        public delegate void SingleFileDoubleClick(FileSystemNodePlus<MyAddittionalData> Nodo);
        public event SingleFileDoubleClick OnSingleFileDoubleClick;

        public delegate void SingleFileRightClick(FileSystemNodePlus<MyAddittionalData> Nodo);
        public event SingleFileRightClick OnSingleFileRightClick;

        public delegate void SingleFileSelectChange(FileSystemNodePlus<MyAddittionalData> Nodo, Keys Modificatore);
        public event SingleFileSelectChange OnSingleFileSelectChange;

        #endregion

        #region Costruttori

        public SingleFile(FileSystemNodePlus<MyAddittionalData> Nodo)
        {
            InitializeComponent();

            this.Nodo = Nodo;

            if (Nodo.Type == FileSystemNodePlusType.Directory)
                this.Icon.BackgroundImage = global::MusicLibraryManager.Properties.Resources.Folder;
            else if (Nodo.Type == FileSystemNodePlusType.File)
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

            sc = new SelectControl(Nodo, checkBox1);
            sc.OnShowCheckBoxChanged += () => { Invalidate(); };
            sc.OnCurrentSelectChanged += () => { Invalidate(); };
            sc.OnNodeSelectChanged += () => 
            {
                SetSelectParentNode(sc.NodeSelect);
                SetSelectChildNode(sc.NodeSelect);
            };
            sc.OnCheckBoxClick += (Keys Modificatore) =>
            {
                if (OnSingleFileSelectChange != null)
                    OnSingleFileSelectChange(Nodo, Modificatore);
                Invalidate();
            };


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
            /*if (_Status == SingleFileStatus.Selected)
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
            }*/

            if (sc.CurrentSelect)
            {
                Rectangle rc = new Rectangle(0, 0, this.ClientSize.Width, this.ClientSize.Height);
                using (LinearGradientBrush brush = new LinearGradientBrush(rc, Color.FromArgb(220, 235, 252), Color.FromArgb(193, 219, 252), 90))
                {
                    e.Graphics.FillRectangle(brush, rc);
                }
            }
            else
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

        #endregion

        #region Dispatcher

        public void SetSelectParentNode(bool Value)
        {
            if (Nodo.Parent != null)
                SetSelectParentNodeRecursive(Nodo.Parent, Value);
        }
        public void SetSelectParentNodeRecursive(FileSystemNodePlus<MyAddittionalData> Nodo, bool Value)
        {
            if (Value)
            {
                Nodo.AddittionalData.Selezionato = true;
            }
            else
            {
                foreach (FileSystemNodePlus<MyAddittionalData> n in Nodo.GetAllNode())
                    if (n.AddittionalData.Selezionato)
                        return;
                Nodo.AddittionalData.Selezionato = false;

            }
            if (Nodo.Parent != null)
                SetSelectParentNodeRecursive(Nodo.Parent, Value);
        }


        public void SetSelectChildNode(bool Value )
        {
            SetSelectChildNodeRecursive(Nodo, Value);
        }
        public void SetSelectChildNodeRecursive(FileSystemNodePlus<MyAddittionalData> Node, bool Value)
        {
            Node.AddittionalData.Selezionato = Value;
            foreach(FileSystemNodePlus<MyAddittionalData> n in Node.GetAllNode())
                SetSelectChildNodeRecursive(n, Value);
            
        }
        #endregion

     
    }

    public class SelectControl
    {
        #region Variabili

        FileSystemNodePlus<MyAddittionalData> Node = null;
        CheckBox c;

        #endregion

        #region Costruttori

        public SelectControl(FileSystemNodePlus<MyAddittionalData> Node, CheckBox c)
        {
            this.Node = Node;
            this.c = c;
            c.MouseClick += C_MouseClick;
            CurrentSelect = NodeSelect;
        }


        #endregion


        private void C_MouseClick(object sender, MouseEventArgs e)
        {
            Keys k = Control.ModifierKeys;
            Select = sender._Cast<CheckBox>().Checked;

            if (OnCheckBoxClick != null)
                OnCheckBoxClick(k);
        }


        #region Delegati ed Eventi

        public delegate void ShowCheckBoxChangedHandler();
        public delegate void NodeSelectChangedHandler();
        public delegate void CurrentSelectChangedHandler();
        public delegate void CheckBoxClickHandler(Keys Modificatore);

        public event ShowCheckBoxChangedHandler OnShowCheckBoxChanged;
        public event NodeSelectChangedHandler OnNodeSelectChanged;
        public event CurrentSelectChangedHandler OnCurrentSelectChanged;
        public event CheckBoxClickHandler OnCheckBoxClick;

        #endregion

        #region Proprietà

        bool _Selectionable = true;
        public bool Selectionable
        {
            get
            {
                return _Selectionable;
            }
            set
            {
                if (_Selectionable = value)
                    return;

                _Selectionable = value;
                if(_Selectionable)
                {
                    ShowCheckBox = true;
                    CurrentSelect = NodeSelect;
                }
                else
                {
                    ShowCheckBox = false;
                    CurrentSelect = false;
                }
            }
        }
        public bool ShowCheckBox
        {
            get { return c.Visible; }
            set
            {
                if (c.Visible != value)
                {
                    c.Visible = value;
                    if (OnShowCheckBoxChanged != null)
                        OnShowCheckBoxChanged();
                }
            }
        }

        public bool NodeSelect {
            get
            {
                return Node.AddittionalData.Selezionato;
            }
            set
            {
                if (Node.AddittionalData.Selezionato != value)
                {
                    Node.AddittionalData.Selezionato = value;
                    if (OnNodeSelectChanged != null)
                        OnNodeSelectChanged();
                }
            }
        }
        public bool CurrentSelect
        {
            get
            {
                return c.Checked;
            }
            set
            {
                if (c.Checked != value)
                {
                    c.Checked = value;
                    if (OnCurrentSelectChanged != null)
                        OnCurrentSelectChanged();
                }
            }
        }

        public bool Select
        {
            set
            {
                NodeSelect = value;
                CurrentSelect = value;
            }
        }

        #endregion

    }
    public enum SingleFileStatus
    {
        Selected,
        NotSelected
    }

}
