namespace CrossMod.GUI
{
    partial class ColorSelector
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
            this.components = new System.ComponentModel.Container();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.replaceSelectedColorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.hitboxList = new System.Windows.Forms.ListView();
            this.hitColorLbl = new System.Windows.Forms.Label();
            this.hurtboxList = new System.Windows.Forms.ListView();
            this.hurtboxLbl = new System.Windows.Forms.Label();
            this.contextMenuStrip2 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip1.SuspendLayout();
            this.contextMenuStrip2.SuspendLayout();
            this.SuspendLayout();
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.replaceSelectedColorToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(195, 26);
            this.contextMenuStrip1.Text = "Hitbox Options";
            // 
            // replaceSelectedColorToolStripMenuItem
            // 
            this.replaceSelectedColorToolStripMenuItem.Name = "replaceSelectedColorToolStripMenuItem";
            this.replaceSelectedColorToolStripMenuItem.Size = new System.Drawing.Size(194, 22);
            this.replaceSelectedColorToolStripMenuItem.Text = "Replace Selected Color";
            this.replaceSelectedColorToolStripMenuItem.Click += new System.EventHandler(this.ReplaceSelectedColorToolStripMenuItem_Click);
            // 
            // hitboxList
            // 
            this.hitboxList.HideSelection = false;
            this.hitboxList.Location = new System.Drawing.Point(12, 29);
            this.hitboxList.MultiSelect = false;
            this.hitboxList.Name = "hitboxList";
            this.hitboxList.Size = new System.Drawing.Size(197, 263);
            this.hitboxList.TabIndex = 0;
            this.hitboxList.UseCompatibleStateImageBehavior = false;
            this.hitboxList.View = System.Windows.Forms.View.List;
            this.hitboxList.MouseDown += new System.Windows.Forms.MouseEventHandler(this.DisplayContextStrip);
            // 
            // hitColorLbl
            // 
            this.hitColorLbl.AutoSize = true;
            this.hitColorLbl.Location = new System.Drawing.Point(12, 13);
            this.hitColorLbl.Name = "hitColorLbl";
            this.hitColorLbl.Size = new System.Drawing.Size(111, 13);
            this.hitColorLbl.TabIndex = 1;
            this.hitColorLbl.Text = "Hitbox Color Selection";
            // 
            // hurtboxList
            // 
            this.hurtboxList.CheckBoxes = true;
            this.hurtboxList.HideSelection = false;
            this.hurtboxList.Location = new System.Drawing.Point(215, 29);
            this.hurtboxList.Name = "hurtboxList";
            this.hurtboxList.Size = new System.Drawing.Size(197, 263);
            this.hurtboxList.TabIndex = 2;
            this.hurtboxList.UseCompatibleStateImageBehavior = false;
            this.hurtboxList.View = System.Windows.Forms.View.List;
            this.hurtboxList.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.HurtboxList_ItemChecked);
            this.hurtboxList.MouseDown += new System.Windows.Forms.MouseEventHandler(this.DisplayContextStrip);
            // 
            // hurtboxLbl
            // 
            this.hurtboxLbl.AutoSize = true;
            this.hurtboxLbl.Location = new System.Drawing.Point(293, 13);
            this.hurtboxLbl.Name = "hurtboxLbl";
            this.hurtboxLbl.Size = new System.Drawing.Size(118, 13);
            this.hurtboxLbl.TabIndex = 3;
            this.hurtboxLbl.Text = "Hurtbox Color Selection";
            // 
            // contextMenuStrip2
            // 
            this.contextMenuStrip2.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMenuStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1});
            this.contextMenuStrip2.Name = "contextMenuStrip1";
            this.contextMenuStrip2.Size = new System.Drawing.Size(195, 26);
            this.contextMenuStrip2.Text = "Hitbox Options";
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(194, 22);
            this.toolStripMenuItem1.Text = "Replace Selected Color";
            this.toolStripMenuItem1.Click += new System.EventHandler(this.ToolStripMenuItem1_Click);
            // 
            // HitboxColorSelector
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(423, 304);
            this.Controls.Add(this.hurtboxLbl);
            this.Controls.Add(this.hurtboxList);
            this.Controls.Add(this.hitColorLbl);
            this.Controls.Add(this.hitboxList);
            this.Name = "HitboxColorSelector";
            this.Text = "Color Selector";
            this.contextMenuStrip1.ResumeLayout(false);
            this.contextMenuStrip2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem replaceSelectedColorToolStripMenuItem;
        private System.Windows.Forms.ListView hitboxList;
        private System.Windows.Forms.Label hitColorLbl;
        private System.Windows.Forms.ListView hurtboxList;
        private System.Windows.Forms.Label hurtboxLbl;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip2;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
    }
}