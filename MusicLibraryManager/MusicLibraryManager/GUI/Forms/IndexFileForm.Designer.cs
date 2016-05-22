namespace MusicLibraryManager.GUI.Forms
{
    partial class IndexFileForm
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
            this.Label_Messages = new System.Windows.Forms.Label();
            this.listBox_Rimossi = new System.Windows.Forms.ListBox();
            this.checkBox_CloseAutomatic = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.listBox_Aggiunti = new System.Windows.Forms.ListBox();
            this.label3 = new System.Windows.Forms.Label();
            this.listBox_Problemi = new System.Windows.Forms.ListBox();
            this.progressBar_total = new System.Windows.Forms.ProgressBar();
            this.progressBar_single = new System.Windows.Forms.ProgressBar();
            this.textBox_source = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // Label_Messages
            // 
            this.Label_Messages.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Label_Messages.Location = new System.Drawing.Point(12, 9);
            this.Label_Messages.Name = "Label_Messages";
            this.Label_Messages.Size = new System.Drawing.Size(755, 18);
            this.Label_Messages.TabIndex = 0;
            this.Label_Messages.Text = "Messaggio";
            this.Label_Messages.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // listBox_Rimossi
            // 
            this.listBox_Rimossi.FormattingEnabled = true;
            this.listBox_Rimossi.Location = new System.Drawing.Point(12, 171);
            this.listBox_Rimossi.Name = "listBox_Rimossi";
            this.listBox_Rimossi.Size = new System.Drawing.Size(203, 199);
            this.listBox_Rimossi.TabIndex = 1;
            // 
            // checkBox_CloseAutomatic
            // 
            this.checkBox_CloseAutomatic.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBox_CloseAutomatic.AutoSize = true;
            this.checkBox_CloseAutomatic.Location = new System.Drawing.Point(574, 400);
            this.checkBox_CloseAutomatic.Name = "checkBox_CloseAutomatic";
            this.checkBox_CloseAutomatic.Size = new System.Drawing.Size(193, 17);
            this.checkBox_CloseAutomatic.TabIndex = 2;
            this.checkBox_CloseAutomatic.Text = "Chiudi in automatico questa finestra";
            this.checkBox_CloseAutomatic.UseVisualStyleBackColor = true;
            this.checkBox_CloseAutomatic.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(12, 150);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(203, 18);
            this.label2.TabIndex = 0;
            this.label2.Text = "Rimossi";
            this.label2.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(221, 150);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(203, 18);
            this.label1.TabIndex = 0;
            this.label1.Text = "Aggiunti";
            this.label1.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // listBox_Aggiunti
            // 
            this.listBox_Aggiunti.FormattingEnabled = true;
            this.listBox_Aggiunti.Location = new System.Drawing.Point(221, 171);
            this.listBox_Aggiunti.Name = "listBox_Aggiunti";
            this.listBox_Aggiunti.Size = new System.Drawing.Size(203, 199);
            this.listBox_Aggiunti.TabIndex = 1;
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(430, 150);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(203, 18);
            this.label3.TabIndex = 0;
            this.label3.Text = "Problemi";
            this.label3.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // listBox_Problemi
            // 
            this.listBox_Problemi.FormattingEnabled = true;
            this.listBox_Problemi.Location = new System.Drawing.Point(430, 171);
            this.listBox_Problemi.Name = "listBox_Problemi";
            this.listBox_Problemi.Size = new System.Drawing.Size(203, 199);
            this.listBox_Problemi.TabIndex = 1;
            // 
            // progressBar_total
            // 
            this.progressBar_total.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar_total.Location = new System.Drawing.Point(12, 111);
            this.progressBar_total.Name = "progressBar_total";
            this.progressBar_total.Size = new System.Drawing.Size(761, 23);
            this.progressBar_total.TabIndex = 5;
            // 
            // progressBar_single
            // 
            this.progressBar_single.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar_single.Location = new System.Drawing.Point(12, 82);
            this.progressBar_single.Name = "progressBar_single";
            this.progressBar_single.Size = new System.Drawing.Size(761, 23);
            this.progressBar_single.TabIndex = 4;
            // 
            // textBox_source
            // 
            this.textBox_source.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox_source.Location = new System.Drawing.Point(12, 30);
            this.textBox_source.Name = "textBox_source";
            this.textBox_source.ReadOnly = true;
            this.textBox_source.Size = new System.Drawing.Size(761, 20);
            this.textBox_source.TabIndex = 3;
            // 
            // IndexFileForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(779, 429);
            this.Controls.Add(this.progressBar_total);
            this.Controls.Add(this.progressBar_single);
            this.Controls.Add(this.textBox_source);
            this.Controls.Add(this.checkBox_CloseAutomatic);
            this.Controls.Add(this.listBox_Problemi);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.listBox_Aggiunti);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.listBox_Rimossi);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.Label_Messages);
            this.Name = "IndexFileForm";
            this.Text = "Indicizzazione File...";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.IndexFileForm_FormClosing);
            this.Load += new System.EventHandler(this.IndexFileForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label Label_Messages;
        private System.Windows.Forms.ListBox listBox_Rimossi;
        private System.Windows.Forms.CheckBox checkBox_CloseAutomatic;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListBox listBox_Aggiunti;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ListBox listBox_Problemi;
        protected System.Windows.Forms.ProgressBar progressBar_total;
        protected System.Windows.Forms.ProgressBar progressBar_single;
        protected System.Windows.Forms.TextBox textBox_source;
    }
}