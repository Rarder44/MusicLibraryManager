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

        FileSystemNodePlus<MyAddittionalData> _Nodo;
        public FileSystemNodePlus<MyAddittionalData> Nodo
        {
            get { return _Nodo; }
        }


        public String RealName { get { return _Nodo.Name; } }
        public String ShowedName
        {
            get { return label1.Text; }
            set { label1.SetTextInvoke(value); }
        }

        public bool Selected
        {
            get { return sc.NodeSelect; }
            set { sc.Select = value; }
        }

        SingleFileStatus _status;
        public SingleFileStatus Status
        {
            get { return _status; }
            set
            {
                _status = value;
                if(_status==SingleFileStatus.Normal)
                {
                    label1.SetEnableInvoke( true);
                    label1.SetVisibleInvoke(true);
                    textBox1.SetEnableInvoke(false);
                    textBox1.SetVisibleInvoke(false);
                }
                else if (_status == SingleFileStatus.Rename)
                {
                    label1.SetEnableInvoke(false);
                    label1.SetVisibleInvoke(false);
                    textBox1.SetTextInvoke(label1.Text);
                    textBox1.SetEnableInvoke(true);
                    textBox1.SetVisibleInvoke(true);
                    textBox1.Focus();
                }
                else
                {
                    throw new Exception("Status non implementato");
                }
            }

        }

        #endregion

        public bool EqualNodo(FileSystemNodePlus<MyAddittionalData> n)
        {
            return n == _Nodo;
        }

        #region Delegati ed Eventi

        public delegate void SingleFileDoubleClick(FileSystemNodePlus<MyAddittionalData> Nodo);
        public event SingleFileDoubleClick OnSingleFileDoubleClick;

        public delegate void SingleFileRightClick(FileSystemNodePlus<MyAddittionalData> Nodo);
        public event SingleFileRightClick OnSingleFileRightClick;


        public delegate void SingleFileMouseDown(SingleFile SF);
        public event SingleFileMouseDown OnSingleFileMouseDown;

        public delegate void SingleFileMouseUp(SingleFile SF);
        public event SingleFileMouseUp OnSingleFileMouseUp;

        public delegate void SingleFileMouseMove(SingleFile SF);
        public event SingleFileMouseMove OnSingleFileMouseMove;


        public delegate void SingleFileSelectChange(FileSystemNodePlus<MyAddittionalData> Nodo, Keys Modificatore);
        public event SingleFileSelectChange OnSingleFileSelectChange;

        public delegate void SingleFileNodoChangeName(SingleFile Sender,FileSystemNodePlus<MyAddittionalData> Nodo, String NewName);
        public event SingleFileNodoChangeName OnSingleFileNodoChangeName;


        #endregion

        #region Costruttori

        public SingleFile(FileSystemNodePlus<MyAddittionalData> Nodo)
        {
            InitializeComponent();

            this._Nodo = Nodo;

            if (Nodo.Type == FileSystemNodePlusType.Directory)
                this.Icon.BackgroundImage = global::MusicLibraryManager.Properties.Resources.Folder;
            else if (Nodo.Type == FileSystemNodePlusType.File)
            {
                Icon icon = RegisteredFileType.GetIconFromExtension(Path.GetExtension(Nodo.Name));
                if (icon != null)
                    this.Icon.BackgroundImage = icon.ToBitmap();
            }

            
            label1.Text = Nodo.ToString();


            
            Control[] c = { this, label1, Icon, textBox1 };
            foreach(Control cc in c)
            {
                cc.MouseClick += SingleFile_MouseClick;
                cc.MouseClick += SingleFile_RightClickCheck;
                cc.MouseDoubleClick += SingleFile_MouseDoubleClick;
                cc.MouseDown += SingleFile_MouseDown;
                cc.MouseUp += SingleFile_MouseUp;
                cc.MouseMove += SingleFile_MouseMove;
            }
            


            checkBox1.MouseClick += SingleFile_RightClickCheck;


        



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


            Status = SingleFileStatus.Normal;
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

        private void SingleFile_MouseDown(object sender, MouseEventArgs e)
        {
            if (OnSingleFileMouseDown != null)
                OnSingleFileMouseDown(this);
        }
        private void SingleFile_MouseUp(object sender, MouseEventArgs e)
        {
            if (this.Parent == null)
                return;
            if (OnSingleFileMouseUp != null)
            {
                Point ptCursor = this.Parent.PointToClient(Cursor.Position);
                Control pBox = this.Parent.GetChildAtPoint(ptCursor);
                if (pBox != null)
                {
                    if (pBox is Label || pBox is CheckBox)
                    {
                        pBox = pBox.Parent;
                    }
                    OnSingleFileMouseUp((pBox as SingleFile));
                }
            }
        }
        private void SingleFile_MouseMove(object sender, MouseEventArgs e)
        {
            if ( OnSingleFileMouseMove != null)
                OnSingleFileMouseMove(this);
        }

        private void SingleFile_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if(e.Button==MouseButtons.Left && OnSingleFileDoubleClick != null)
                OnSingleFileDoubleClick(_Nodo);
        }
        private void SingleFile_MouseClick(object sender, MouseEventArgs e)
        {

        }
        private void SingleFile_RightClickCheck(object sender, MouseEventArgs e)
        {
            if(e.Button==MouseButtons.Right && OnSingleFileRightClick!=null)
            {
                if(sender is SingleFile)
                    OnSingleFileRightClick(sender._Cast<SingleFile>()._Nodo);
                else
                    if(sender._Cast<Control>().Parent is SingleFile)
                        OnSingleFileRightClick(sender._Cast<Control>().Parent._Cast<SingleFile>()._Nodo);
            }
                
        }

        #endregion

        #region Dispatcher

        public void SetSelectParentNode(bool Value)
        {
            if (_Nodo.Parent != null)
                SetSelectParentNodeRecursive(_Nodo.Parent, Value);
        }
        private void SetSelectParentNodeRecursive(FileSystemNodePlus<MyAddittionalData> Nodo, bool Value)
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
            SetSelectChildNodeRecursive(_Nodo, Value);
        }
        private void SetSelectChildNodeRecursive(FileSystemNodePlus<MyAddittionalData> Node, bool Value)
        {
            Node.AddittionalData.Selezionato = Value;
            foreach(FileSystemNodePlus<MyAddittionalData> n in Node.GetAllNode())
                SetSelectChildNodeRecursive(n, Value);
            
        }

        #endregion

        private void SingleFile_Load(object sender, EventArgs e)
        {

        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode==Keys.Enter && Status==SingleFileStatus.Rename)
            {
                if (OnSingleFileNodoChangeName != null)
                    OnSingleFileNodoChangeName(this,_Nodo, textBox1.Text);
            }
            else if (e.KeyCode == Keys.Escape && Status == SingleFileStatus.Rename)
            {
                Status = SingleFileStatus.Normal;
            }
        }
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
        Normal,
        Rename
    }

}
