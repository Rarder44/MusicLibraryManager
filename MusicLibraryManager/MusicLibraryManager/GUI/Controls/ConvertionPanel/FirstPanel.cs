using ExtendCSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicLibraryManager.GUI.Controls.ConvertionPanel
{
    class FirstPanel : SliderPanel
    {
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBox1;

        public override CanGoReturn CanGo(SlideFormButton dir)
        {
            switch(dir)
            {
                case SlideFormButton.Bottom:
                    return new CanGoReturn(false, "Non Implementato");
                case SlideFormButton.Top:
                    return new CanGoReturn(false, "Non Implementato");
                case SlideFormButton.Right:
                    if (textBox1.Text == "Pippo")
                        return new CanGoReturn(true);
                    else
                        return new CanGoReturn(false,"Scrivi \"Pippo\"");
                case SlideFormButton.Left:
                    return new CanGoReturn(false, "Non Implementato");
            }
            return new CanGoReturn(false, "Non Implementato");
        }
        public FirstPanel()
        {
            InitializeComponent();
        }
        private void InitializeComponent()
        {
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(149, 125);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(100, 20);
            this.textBox1.TabIndex = 0;
            this.textBox1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBox1_KeyDown);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(78, 38);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(33, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Prima";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(163, 109);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(73, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Scrivi \"Pippo\"";
            // 
            // FirstPanel
            // 
            this.BackColor = System.Drawing.SystemColors.Control;
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBox1);
            this.Name = "FirstPanel";
            this.Size = new System.Drawing.Size(454, 224);
            this.ResumeLayout(false);
            this.PerformLayout();

        }


        private void textBox1_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
           
          
        }
    }
}
