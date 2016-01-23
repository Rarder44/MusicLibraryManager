using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MusicLibraryManager.GUI.Forms
{
    public partial class RequestForm : Form
    {
        public RequestForm()
        {
            InitializeComponent();
        }
        public bool Saved{get; set;} = false;
        private void button1_Click(object sender, EventArgs e)
        {
            Saved = true;
            Close();
        }
    }
}
