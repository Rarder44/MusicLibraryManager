using ExtendCSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicLibraryManager.GUI.Controls.ConvertionPanel
{
    class SecondPanel : SliderPanel
    {
        private System.Windows.Forms.Label label1;

        public override CanGoReturn CanGo(SlideFormButton dir)
        {
            switch (dir)
            {
                case SlideFormButton.Bottom:
                    return new CanGoReturn(false, "Non Implementato");
                case SlideFormButton.Top:
                    return new CanGoReturn(false, "Non Implementato");
                case SlideFormButton.Right:
                    return new CanGoReturn(true);
                case SlideFormButton.Left:
                    return new CanGoReturn(true);
            }
            return new CanGoReturn(false, "Non Implementato");
        }
        public SecondPanel()
        {
            InitializeComponent();
        }
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(159, 27);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(50, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Seconda";
            // 
            // SecondPanel
            // 
            this.BackColor = System.Drawing.SystemColors.Control;
            this.Controls.Add(this.label1);
            this.Name = "SecondPanel";
            this.Size = new System.Drawing.Size(454, 224);
            this.ResumeLayout(false);
            this.PerformLayout();

        }
    }
}
