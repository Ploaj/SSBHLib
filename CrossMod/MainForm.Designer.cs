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
            this.cameraToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clearViewportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.experimentalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.batchRenderModelsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.animationExportOptionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportAnimationToGIFToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.exportAnimationToFolderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportCurrentFrameToBitmapToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fileTree = new System.Windows.Forms.TreeView();
            this.contentBox = new System.Windows.Forms.GroupBox();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
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
            this.menuStrip1.Size = new System.Drawing.Size(1063, 24);
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
            this.openModelFolderToolStripMenuItem.Size = new System.Drawing.Size(182, 22);
            this.openModelFolderToolStripMenuItem.Text = "Open Folder";
            this.openModelFolderToolStripMenuItem.Click += new System.EventHandler(this.openFolderToolStripMenuItem_Click);
            // 
            // reloadShadersToolStripMenuItem
            // 
            this.reloadShadersToolStripMenuItem.Name = "reloadShadersToolStripMenuItem";
            this.reloadShadersToolStripMenuItem.Size = new System.Drawing.Size(182, 22);
            this.reloadShadersToolStripMenuItem.Text = "Reload Shaders";
            this.reloadShadersToolStripMenuItem.Click += new System.EventHandler(this.reloadShadersToolStripMenuItem_Click);
            // 
            // clearWorkspaceToolStripMenuItem
            // 
            this.clearWorkspaceToolStripMenuItem.Name = "clearWorkspaceToolStripMenuItem";
            this.clearWorkspaceToolStripMenuItem.Size = new System.Drawing.Size(182, 22);
            this.clearWorkspaceToolStripMenuItem.Text = "Clear Workspace";
            this.clearWorkspaceToolStripMenuItem.Click += new System.EventHandler(this.clearWorkspaceToolStripMenuItem_Click);
            // 
            // viewportToolStripMenuItem
            // 
            this.viewportToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.renderSettingsToolStripMenuItem,
            this.frameSelectionToolStripMenuItem,
            this.cameraToolStripMenuItem,
            this.clearViewportToolStripMenuItem});
            this.viewportToolStripMenuItem.Name = "viewportToolStripMenuItem";
            this.viewportToolStripMenuItem.Size = new System.Drawing.Size(66, 20);
            this.viewportToolStripMenuItem.Text = "Viewport";
            // 
            // renderSettingsToolStripMenuItem
            // 
            this.renderSettingsToolStripMenuItem.Name = "renderSettingsToolStripMenuItem";
            this.renderSettingsToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
            this.renderSettingsToolStripMenuItem.Text = "Render Settings";
            this.renderSettingsToolStripMenuItem.Click += new System.EventHandler(this.renderSettingsToolStripMenuItem_Click);
            // 
            // frameSelectionToolStripMenuItem
            // 
            this.frameSelectionToolStripMenuItem.Name = "frameSelectionToolStripMenuItem";
            this.frameSelectionToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
            this.frameSelectionToolStripMenuItem.Text = "Frame Models";
            this.frameSelectionToolStripMenuItem.Click += new System.EventHandler(this.frameSelectionToolStripMenuItem_Click);
            // 
            // cameraToolStripMenuItem
            // 
            this.cameraToolStripMenuItem.Name = "cameraToolStripMenuItem";
            this.cameraToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
            this.cameraToolStripMenuItem.Text = "Camera";
            this.cameraToolStripMenuItem.Click += new System.EventHandler(this.cameraToolStripMenuItem_Click);
            // 
            // clearViewportToolStripMenuItem
            // 
            this.clearViewportToolStripMenuItem.Name = "clearViewportToolStripMenuItem";
            this.clearViewportToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
            this.clearViewportToolStripMenuItem.Text = "Clear Viewport";
            this.clearViewportToolStripMenuItem.Click += new System.EventHandler(this.clearViewportToolStripMenuItem_Click);
            // 
            // experimentalToolStripMenuItem
            // 
            this.experimentalToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.batchRenderModelsToolStripMenuItem,
            this.animationExportOptionsToolStripMenuItem});
            this.experimentalToolStripMenuItem.Name = "experimentalToolStripMenuItem";
            this.experimentalToolStripMenuItem.Size = new System.Drawing.Size(88, 20);
            this.experimentalToolStripMenuItem.Text = "Experimental";
            // 
            // batchRenderModelsToolStripMenuItem
            // 
            this.batchRenderModelsToolStripMenuItem.Name = "batchRenderModelsToolStripMenuItem";
            this.batchRenderModelsToolStripMenuItem.Size = new System.Drawing.Size(212, 22);
            this.batchRenderModelsToolStripMenuItem.Text = "Batch Render Models";
            this.batchRenderModelsToolStripMenuItem.Click += new System.EventHandler(this.batchRenderModelsToolStripMenuItem_Click);
            // 
            // animationExportOptionsToolStripMenuItem
            // 
            this.animationExportOptionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exportAnimationToGIFToolStripMenuItem1,
            this.exportAnimationToFolderToolStripMenuItem,
            this.exportCurrentFrameToBitmapToolStripMenuItem});
            this.animationExportOptionsToolStripMenuItem.Name = "animationExportOptionsToolStripMenuItem";
            this.animationExportOptionsToolStripMenuItem.Size = new System.Drawing.Size(212, 22);
            this.animationExportOptionsToolStripMenuItem.Text = "Animation Export Options";
            // 
            // exportAnimationToGIFToolStripMenuItem1
            // 
            this.exportAnimationToGIFToolStripMenuItem1.Name = "exportAnimationToGIFToolStripMenuItem1";
            this.exportAnimationToGIFToolStripMenuItem1.Size = new System.Drawing.Size(242, 22);
            this.exportAnimationToGIFToolStripMenuItem1.Text = "Export Animation to GIF";
            this.exportAnimationToGIFToolStripMenuItem1.Click += new System.EventHandler(this.ExportAnimationToGIFToolStripMenuItem1_Click);
            // 
            // exportAnimationToFolderToolStripMenuItem
            // 
            this.exportAnimationToFolderToolStripMenuItem.Name = "exportAnimationToFolderToolStripMenuItem";
            this.exportAnimationToFolderToolStripMenuItem.Size = new System.Drawing.Size(242, 22);
            this.exportAnimationToFolderToolStripMenuItem.Text = "Export Animation to Folder";
            this.exportAnimationToFolderToolStripMenuItem.Click += new System.EventHandler(this.ExportAnimationToFolderToolStripMenuItem_Click);
            // 
            // exportCurrentFrameToBitmapToolStripMenuItem
            // 
            this.exportCurrentFrameToBitmapToolStripMenuItem.Name = "exportCurrentFrameToBitmapToolStripMenuItem";
            this.exportCurrentFrameToBitmapToolStripMenuItem.Size = new System.Drawing.Size(242, 22);
            this.exportCurrentFrameToBitmapToolStripMenuItem.Text = "Export Current Frame to Bitmap";
            this.exportCurrentFrameToBitmapToolStripMenuItem.Click += new System.EventHandler(this.ExportCurrentFrameToBitmapToolStripMenuItem_Click);
            // 
            // fileTree
            // 
            this.fileTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fileTree.HideSelection = false;
            this.fileTree.ItemHeight = 24;
            this.fileTree.Location = new System.Drawing.Point(0, 0);
            this.fileTree.Name = "fileTree";
            this.fileTree.Size = new System.Drawing.Size(240, 538);
            this.fileTree.TabIndex = 2;
            this.fileTree.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.fileTree_BeforeExpand);
            this.fileTree.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.fileTree_AfterSelect);
            this.fileTree.MouseUp += new System.Windows.Forms.MouseEventHandler(this.fileTree_MouseUp);
            // 
            // contentBox
            // 
            this.contentBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.contentBox.Location = new System.Drawing.Point(0, 0);
            this.contentBox.Name = "contentBox";
            this.contentBox.Size = new System.Drawing.Size(795, 538);
            this.contentBox.TabIndex = 3;
            this.contentBox.TabStop = false;
            this.contentBox.Text = "Viewer";
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.Location = new System.Drawing.Point(12, 27);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.fileTree);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.contentBox);
            this.splitContainer1.Size = new System.Drawing.Size(1039, 538);
            this.splitContainer1.SplitterDistance = 240;
            this.splitContainer1.TabIndex = 4;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1063, 577);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainForm";
            this.Text = "CrossMod";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
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
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ToolStripMenuItem cameraToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem clearViewportToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem animationExportOptionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportAnimationToGIFToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem exportAnimationToFolderToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportCurrentFrameToBitmapToolStripMenuItem;
    }
}
