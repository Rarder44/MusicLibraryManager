namespace MusicLibraryManager.GUI.Forms
{
    partial class ConvertMediaForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.textBox_source = new System.Windows.Forms.TextBox();
            this.textBox_destination = new System.Windows.Forms.TextBox();
            this.progressBar_total = new System.Windows.Forms.ProgressBar();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.progressBar_single = new System.Windows.Forms.ProgressBar();
            this.SuspendLayout();
            // 
            // textBox_source
            // 
            this.textBox_source.Location = new System.Drawing.Point(12, 12);
            this.textBox_source.Name = "textBox_source";
            this.textBox_source.ReadOnly = true;
            this.textBox_source.Size = new System.Drawing.Size(556, 20);
            this.textBox_source.TabIndex = 0;
            // 
            // textBox_destination
            // 
            this.textBox_destination.Location = new System.Drawing.Point(12, 38);
            this.textBox_destination.Name = "textBox_destination";
            this.textBox_destination.ReadOnly = true;
            this.textBox_destination.Size = new System.Drawing.Size(556, 20);
            this.textBox_destination.TabIndex = 0;
            // 
            // progressBar_total
            // 
            this.progressBar_total.Location = new System.Drawing.Point(12, 93);
            this.progressBar_total.Name = "progressBar_total";
            this.progressBar_total.Size = new System.Drawing.Size(556, 23);
            this.progressBar_total.TabIndex = 1;
            // 
            // textBox1
            // 
            this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox1.Location = new System.Drawing.Point(12, 122);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(556, 100);
            this.textBox1.TabIndex = 2;
            this.textBox1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBox1_KeyDown);
            // 
            // progressBar_single
            // 
            this.progressBar_single.Location = new System.Drawing.Point(12, 64);
            this.progressBar_single.Name = "progressBar_single";
            this.progressBar_single.Size = new System.Drawing.Size(556, 23);
            this.progressBar_single.TabIndex = 3;
            // 
            // ConvertMediaForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(580, 234);
            this.Controls.Add(this.progressBar_single);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.progressBar_total);
            this.Controls.Add(this.textBox_destination);
            this.Controls.Add(this.textBox_source);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ConvertMediaForm";
            this.ShowIcon = false;
            this.Text = "ConvertMedia";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ConvertMedia_FormClosing);
            this.Load += new System.EventHandler(this.ConvertMediaForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        public System.Windows.Forms.TextBox textBox_source;
        public System.Windows.Forms.TextBox textBox_destination;
        public System.Windows.Forms.ProgressBar progressBar_total;
        public System.Windows.Forms.ProgressBar progressBar_single;
        public System.Windows.Forms.TextBox textBox1;
    }
}