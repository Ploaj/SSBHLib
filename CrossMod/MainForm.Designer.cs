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
            this.reloadShadersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clearWorkspaceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.renderSettingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.frameSelectionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.experimentalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.batchRenderModelsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fileTree = new System.Windows.Forms.TreeView();
            this.contentBox = new System.Windows.Forms.GroupBox();
            this.printMaterialValuesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.viewportToolStripMenuItem,
            this.experimentalToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(872, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openModelFolderToolStripMenuItem,
            this.reloadShadersToolStripMenuItem,
            this.clearWorkspaceToolStripMenuItem});
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
            // reloadShadersToolStripMenuItem
            // 
            this.reloadShadersToolStripMenuItem.Name = "reloadShadersToolStripMenuItem";
            this.reloadShadersToolStripMenuItem.Size = new System.Drawing.Size(219, 22);
            this.reloadShadersToolStripMenuItem.Text = "Reload Shaders";
            this.reloadShadersToolStripMenuItem.Click += new System.EventHandler(this.reloadShadersToolStripMenuItem_Click);
            // 
            // clearWorkspaceToolStripMenuItem
            // 
            this.clearWorkspaceToolStripMenuItem.Name = "clearWorkspaceToolStripMenuItem";
            this.clearWorkspaceToolStripMenuItem.Size = new System.Drawing.Size(219, 22);
            this.clearWorkspaceToolStripMenuItem.Text = "Clear Workspace";
            this.clearWorkspaceToolStripMenuItem.Click += new System.EventHandler(this.clearWorkspaceToolStripMenuItem_Click);
            // 
            // viewportToolStripMenuItem
            // 
            this.viewportToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.renderSettingsToolStripMenuItem,
            this.frameSelectionToolStripMenuItem});
            this.viewportToolStripMenuItem.Name = "viewportToolStripMenuItem";
            this.viewportToolStripMenuItem.Size = new System.Drawing.Size(66, 20);
            this.viewportToolStripMenuItem.Text = "Viewport";
            // 
            // renderSettingsToolStripMenuItem
            // 
            this.renderSettingsToolStripMenuItem.Name = "renderSettingsToolStripMenuItem";
            this.renderSettingsToolStripMenuItem.Size = new System.Drawing.Size(158, 22);
            this.renderSettingsToolStripMenuItem.Text = "Render Settings";
            this.renderSettingsToolStripMenuItem.Click += new System.EventHandler(this.renderSettingsToolStripMenuItem_Click);
            // 
            // frameSelectionToolStripMenuItem
            // 
            this.frameSelectionToolStripMenuItem.Name = "frameSelectionToolStripMenuItem";
            this.frameSelectionToolStripMenuItem.Size = new System.Drawing.Size(158, 22);
            this.frameSelectionToolStripMenuItem.Text = "Frame Selection";
            this.frameSelectionToolStripMenuItem.Click += new System.EventHandler(this.frameSelectionToolStripMenuItem_Click);
            // 
            // experimentalToolStripMenuItem
            // 
            this.experimentalToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.batchRenderModelsToolStripMenuItem,
            this.printMaterialValuesToolStripMenuItem});
            this.experimentalToolStripMenuItem.Name = "experimentalToolStripMenuItem";
            this.experimentalToolStripMenuItem.Size = new System.Drawing.Size(87, 20);
            this.experimentalToolStripMenuItem.Text = "Experimental";
            // 
            // batchRenderModelsToolStripMenuItem
            // 
            this.batchRenderModelsToolStripMenuItem.Name = "batchRenderModelsToolStripMenuItem";
            this.batchRenderModelsToolStripMenuItem.Size = new System.Drawing.Size(186, 22);
            this.batchRenderModelsToolStripMenuItem.Text = "Batch Render Models";
            this.batchRenderModelsToolStripMenuItem.Click += new System.EventHandler(this.batchRenderModelsToolStripMenuItem_Click);
            // 
            // fileTree
            // 
            this.fileTree.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.fileTree.ItemHeight = 24;
            this.fileTree.Location = new System.Drawing.Point(12, 27);
            this.fileTree.Name = "fileTree";
            this.fileTree.Size = new System.Drawing.Size(241, 538);
            this.fileTree.TabIndex = 2;
            this.fileTree.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.fileTree_AfterSelect);
            this.fileTree.MouseUp += new System.Windows.Forms.MouseEventHandler(this.fileTree_MouseUp);
            // 
            // contentBox
            // 
            this.contentBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.contentBox.Location = new System.Drawing.Point(259, 27);
            this.contentBox.Name = "contentBox";
            this.contentBox.Size = new System.Drawing.Size(601, 538);
            this.contentBox.TabIndex = 3;
            this.contentBox.TabStop = false;
            this.contentBox.Text = "Viewer";
            // 
            // printMaterialValuesToolStripMenuItem
            // 
            this.printMaterialValuesToolStripMenuItem.Name = "printMaterialValuesToolStripMenuItem";
            this.printMaterialValuesToolStripMenuItem.Size = new System.Drawing.Size(186, 22);
            this.printMaterialValuesToolStripMenuItem.Text = "Print Material Values";
            this.printMaterialValuesToolStripMenuItem.Click += new System.EventHandler(this.printMaterialValuesToolStripMenuItem_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(872, 577);
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
        private System.Windows.Forms.TreeView fileTree;
        private System.Windows.Forms.GroupBox contentBox;
        private System.Windows.Forms.ToolStripMenuItem reloadShadersToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewportToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem renderSettingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem clearWorkspaceToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem experimentalToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem batchRenderModelsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem frameSelectionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem printMaterialValuesToolStripMenuItem;
    }
}

