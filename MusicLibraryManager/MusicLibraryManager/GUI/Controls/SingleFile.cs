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
        public bool ShowCheckBox {
            get { return checkBox1.Visible; }
            set
            {
                checkBox1.Visible = value;
            }
        }
        public bool Selectionable { get; set; } = true;

        SingleFileStatus _Status = SingleFileStatus.NotSelected;
        SingleFileStatus Status
        {
            get { return _Status; }
            set
            {
                if (!Selectionable)
                {
                    checkBox1.CheckedChanged -= checkBox1_CheckedChanged;
                    checkBox1.Checked = false;
                    Nodo.AddittionalData.Selezionato = false;
                    _Status = SingleFileStatus.NotSelected;
                    checkBox1.CheckedChanged += checkBox1_CheckedChanged;
                    return;
                }
                   
                if (_Status == value)
                    return;

                
                _Status = value;
                
                if (_Status == SingleFileStatus.Selected)
                {
                    checkBox1.CheckedChanged -= checkBox1_CheckedChanged;
                    checkBox1.Checked = true;
                    Nodo.AddittionalData.Selezionato = true;
                    checkBox1.CheckedChanged += checkBox1_CheckedChanged;
                }
                else if (_Status == SingleFileStatus.NotSelected)
                {
                    checkBox1.CheckedChanged -= checkBox1_CheckedChanged;
                    checkBox1.Checked = false;
                    Nodo.AddittionalData.Selezionato = false;
                    checkBox1.CheckedChanged += checkBox1_CheckedChanged;
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



        public delegate void SingleFileDoubleClick(MyFileSystemNode<MyAddittionalDataMyFileSystem> Nodo);
        public event SingleFileDoubleClick OnSingleFileDoubleClick;



        public SingleFile(MyFileSystemNode<MyAddittionalDataMyFileSystem> Nodo)
        {
            InitializeComponent();

            this.Nodo = Nodo;

            if (Nodo.Type == MyFileSystemNodeType.Directory)
                this.Icon.BackgroundImage = global::MusicLibraryManager.Properties.Resources.folder;
            else if (Nodo.Type == MyFileSystemNodeType.File)
            {
                object o = GlobalVar.iconsInfo[Path.GetExtension(Nodo.Name)];
                if(o!=null)
                {
                    string fileAndParam = o.ToString();
                    if (!String.IsNullOrEmpty(fileAndParam))
                    {
                        Icon icon = null;
                        bool isLarge = true;
                        icon = RegisteredFileType.ExtractIconFromFile(fileAndParam, isLarge);
                        if (icon != null)
                            this.Icon.BackgroundImage = icon.ToBitmap();
                    }
                } 
            }

            
            label1.Text = Nodo.ToString();


            this.Click += new System.EventHandler(this.SingleFile_Click);
            label1.Click += new System.EventHandler(this.SingleFile_Click);
            Icon.Click += new System.EventHandler(this.SingleFile_Click);

            this.DoubleClick += new System.EventHandler(this.SingleFile_DoubleClick);
            label1.DoubleClick += new System.EventHandler(this.SingleFile_DoubleClick);
            Icon.DoubleClick += new System.EventHandler(this.SingleFile_DoubleClick);

            Status = Nodo.AddittionalData.Selezionato ? SingleFileStatus.Selected : SingleFileStatus.NotSelected;
        }



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



        private void SingleFile_DoubleClick(object sender, EventArgs e)
        {
            if (OnSingleFileDoubleClick != null)
                OnSingleFileDoubleClick(Nodo);
        }

        private void SingleFile_Click(object sender, EventArgs e)
        {
            if(Status!=SingleFileStatus.Selected)
                Status = SingleFileStatus.Selected;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            Status =  checkBox1.Checked ? SingleFileStatus.Selected : SingleFileStatus.NotSelected ;  
        }


        
    }
    public enum SingleFileStatus
    {
        Selected,
        NotSelected
    }

}
