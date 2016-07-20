namespace MusicLibraryManager.GUI.Controls
{
    partial class FileBrowser
    {
        /// <summary> 
        /// Variabile di progettazione necessaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Pulire le risorse in uso.
        /// </summary>
        /// <param name="disposing">ha valore true se le risorse gestite devono essere eliminate, false in caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Codice generato da Progettazione componenti

        /// <summary> 
        /// Metodo necessario per il supporto della finestra di progettazione. Non modificare 
        /// il contenuto del metodo con l'editor di codice.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.sceltaCheckBoxToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deselezionaTuttoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.aggiungiRimuoviToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.creaCartellaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rinominaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.mergeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mergeEdEstraiToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.sceltaCheckBoxToolStripMenuItem,
            this.deselezionaTuttoToolStripMenuItem,
            this.toolStripSeparator1,
            this.aggiungiRimuoviToolStripMenuItem,
            this.creaCartellaToolStripMenuItem,
            this.rinominaToolStripMenuItem,
            this.toolStripSeparator2,
            this.mergeToolStripMenuItem,
            this.mergeEdEstraiToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(203, 170);
            // 
            // sceltaCheckBoxToolStripMenuItem
            // 
            this.sceltaCheckBoxToolStripMenuItem.Name = "sceltaCheckBoxToolStripMenuItem";
            this.sceltaCheckBoxToolStripMenuItem.Size = new System.Drawing.Size(202, 22);
            this.sceltaCheckBoxToolStripMenuItem.Text = "Scelta CheckBox";
            this.sceltaCheckBoxToolStripMenuItem.Click += new System.EventHandler(this.sceltaCheckBoxToolStripMenuItem_Click);
            // 
            // deselezionaTuttoToolStripMenuItem
            // 
            this.deselezionaTuttoToolStripMenuItem.Name = "deselezionaTuttoToolStripMenuItem";
            this.deselezionaTuttoToolStripMenuItem.Size = new System.Drawing.Size(202, 22);
            this.deselezionaTuttoToolStripMenuItem.Text = "Deseleziona Tutto";
            this.deselezionaTuttoToolStripMenuItem.Click += new System.EventHandler(this.deselezionaTuttoToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(199, 6);
            // 
            // aggiungiRimuoviToolStripMenuItem
            // 
            this.aggiungiRimuoviToolStripMenuItem.Name = "aggiungiRimuoviToolStripMenuItem";
            this.aggiungiRimuoviToolStripMenuItem.Size = new System.Drawing.Size(202, 22);
            this.aggiungiRimuoviToolStripMenuItem.Text = "Aggiungi Rimuovi";
            // 
            // creaCartellaToolStripMenuItem
            // 
            this.creaCartellaToolStripMenuItem.Name = "creaCartellaToolStripMenuItem";
            this.creaCartellaToolStripMenuItem.Size = new System.Drawing.Size(202, 22);
            this.creaCartellaToolStripMenuItem.Text = "Crea Cartella";
            this.creaCartellaToolStripMenuItem.Click += new System.EventHandler(this.creaCartellaToolStripMenuItem_Click);
            // 
            // rinominaToolStripMenuItem
            // 
            this.rinominaToolStripMenuItem.Name = "rinominaToolStripMenuItem";
            this.rinominaToolStripMenuItem.Size = new System.Drawing.Size(202, 22);
            this.rinominaToolStripMenuItem.Text = "Rinomina";
            this.rinominaToolStripMenuItem.Click += new System.EventHandler(this.rinominaToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(199, 6);
            // 
            // mergeToolStripMenuItem
            // 
            this.mergeToolStripMenuItem.Name = "mergeToolStripMenuItem";
            this.mergeToolStripMenuItem.Size = new System.Drawing.Size(202, 22);
            this.mergeToolStripMenuItem.Text = "Merge in Nuova Cartella";
            this.mergeToolStripMenuItem.Click += new System.EventHandler(this.mergeToolStripMenuItem_Click);
            // 
            // mergeEdEstraiToolStripMenuItem
            // 
            this.mergeEdEstraiToolStripMenuItem.Name = "mergeEdEstraiToolStripMenuItem";
            this.mergeEdEstraiToolStripMenuItem.Size = new System.Drawing.Size(202, 22);
            this.mergeEdEstraiToolStripMenuItem.Text = "Merge ed estrai";
            this.mergeEdEstraiToolStripMenuItem.Click += new System.EventHandler(this.mergeEdEstraiToolStripMenuItem_Click);
            // 
            // FileBrowser
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.SystemColors.ControlDark;
            this.DoubleBuffered = true;
            this.Name = "FileBrowser";
            this.Size = new System.Drawing.Size(926, 513);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FileBrowser_KeyDown);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.FileBrowser_MouseDown);
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem sceltaCheckBoxToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deselezionaTuttoToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem aggiungiRimuoviToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem creaCartellaToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mergeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mergeEdEstraiToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem rinominaToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
    }
}
