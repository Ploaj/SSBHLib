namespace CrossMod
{
    partial class MainForm
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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openModelFolderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clearFilesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fileTree = new System.Windows.Forms.TreeView();
            this.contentBox = new System.Windows.Forms.GroupBox();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(551, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openModelFolderToolStripMenuItem,
            this.clearFilesToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // openModelFolderToolStripMenuItem
            // 
            this.openModelFolderToolStripMenuItem.Name = "openModelFolderToolStripMenuItem";
            this.openModelFolderToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.openModelFolderToolStripMenuItem.Size = new System.Drawing.Size(219, 22);
            this.openModelFolderToolStripMenuItem.Text = "Open Model Folder";
            this.openModelFolderToolStripMenuItem.Click += new System.EventHandler(this.openModelFolderToolStripMenuItem_Click);
            // 
            // clearFilesToolStripMenuItem
            // 
            this.clearFilesToolStripMenuItem.Name = "clearFilesToolStripMenuItem";
            this.clearFilesToolStripMenuItem.Size = new System.Drawing.Size(219, 22);
            this.clearFilesToolStripMenuItem.Text = "Clear Files";
            // 
            // fileTree
            // 
            this.fileTree.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.fileTree.ItemHeight = 24;
            this.fileTree.Location = new System.Drawing.Point(12, 27);
            this.fileTree.Name = "fileTree";
            this.fileTree.Size = new System.Drawing.Size(241, 293);
            this.fileTree.TabIndex = 2;
            this.fileTree.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.fileTree_AfterSelect);
            // 
            // contentBox
            // 
            this.contentBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.contentBox.Location = new System.Drawing.Point(259, 27);
            this.contentBox.Name = "contentBox";
            this.contentBox.Size = new System.Drawing.Size(280, 293);
            this.contentBox.TabIndex = 3;
            this.contentBox.TabStop = false;
            this.contentBox.Text = "Viewer";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(551, 332);
            this.Controls.Add(this.contentBox);
            this.Controls.Add(this.fileTree);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainForm";
            this.Text = "CrossMod";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openModelFolderToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem clearFilesToolStripMenuItem;
        private System.Windows.Forms.TreeView fileTree;
        private System.Windows.Forms.GroupBox contentBox;
    }
}

