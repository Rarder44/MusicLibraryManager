namespace MusicLibraryManager.GUI.Forms
{
    partial class IncorporaMetadata
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
            this.progressBar_single = new System.Windows.Forms.ProgressBar();
            this.progressBar_total = new System.Windows.Forms.ProgressBar();
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
            // progressBar_single
            // 
            this.progressBar_single.Location = new System.Drawing.Point(12, 64);
            this.progressBar_single.Name = "progressBar_single";
            this.progressBar_single.Size = new System.Drawing.Size(556, 23);
            this.progressBar_single.TabIndex = 1;
            // 
            // progressBar_total
            // 
            this.progressBar_total.Location = new System.Drawing.Point(12, 93);
            this.progressBar_total.Name = "progressBar_total";
            this.progressBar_total.Size = new System.Drawing.Size(556, 23);
            this.progressBar_total.TabIndex = 1;
            // 
            // IncorporaMetadata
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(580, 127);
            this.Controls.Add(this.progressBar_total);
            this.Controls.Add(this.progressBar_single);
            this.Controls.Add(this.textBox_source);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "IncorporaMetadata";
            this.ShowIcon = false;
            this.Text = "IncorporaMetadata";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.IncorporaMetadata_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBox_source;
        private System.Windows.Forms.ProgressBar progressBar_single;
        private System.Windows.Forms.ProgressBar progressBar_total;
    }
}