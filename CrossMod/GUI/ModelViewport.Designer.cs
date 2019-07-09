namespace CrossMod.GUI
{
    partial class ModelViewport
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.glViewport = new SFGraphics.Controls.GLViewport();
            this.controlBox = new System.Windows.Forms.GroupBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.meshList = new System.Windows.Forms.ListView();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.boneTree = new System.Windows.Forms.TreeView();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage2.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.SuspendLayout();
            // 
            // glViewport
            // 
            this.glViewport.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.glViewport.BackColor = System.Drawing.Color.Black;
            this.glViewport.Dock = System.Windows.Forms.DockStyle.Fill;
            this.glViewport.Location = new System.Drawing.Point(0, 0);
            this.glViewport.Name = "glViewport";
            this.glViewport.Size = new System.Drawing.Size(435, 306);
            this.glViewport.TabIndex = 0;
            this.glViewport.VSync = false;
            this.glViewport.Load += new System.EventHandler(this.glViewport_Load);
            this.glViewport.Resize += new System.EventHandler(this.glViewport_Resize);
            // 
            // controlBox
            // 
            this.controlBox.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.controlBox.Location = new System.Drawing.Point(0, 306);
            this.controlBox.Name = "controlBox";
            this.controlBox.Size = new System.Drawing.Size(435, 115);
            this.controlBox.TabIndex = 3;
            this.controlBox.TabStop = false;
            this.controlBox.Text = "Animation Controls";
            this.controlBox.Visible = false;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.meshList);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(228, 395);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Mesh";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // meshList
            // 
            this.meshList.CheckBoxes = true;
            this.meshList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.meshList.Location = new System.Drawing.Point(3, 3);
            this.meshList.Name = "meshList";
            this.meshList.Size = new System.Drawing.Size(222, 389);
            this.meshList.TabIndex = 0;
            this.meshList.UseCompatibleStateImageBehavior = false;
            this.meshList.View = System.Windows.Forms.View.List;
            this.meshList.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.meshList_ItemChecked);
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.boneTree);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(228, 395);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Bones";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // boneTree
            // 
            this.boneTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.boneTree.Location = new System.Drawing.Point(3, 3);
            this.boneTree.Name = "boneTree";
            this.boneTree.Size = new System.Drawing.Size(222, 389);
            this.boneTree.TabIndex = 0;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Right;
            this.tabControl1.Location = new System.Drawing.Point(435, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(236, 421);
            this.tabControl1.TabIndex = 1;
            // 
            // ModelViewport
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.Controls.Add(this.glViewport);
            this.Controls.Add(this.controlBox);
            this.Controls.Add(this.tabControl1);
            this.Name = "ModelViewport";
            this.Size = new System.Drawing.Size(671, 421);
            this.tabPage2.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private SFGraphics.Controls.GLViewport glViewport;
        private System.Windows.Forms.GroupBox controlBox;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.ListView meshList;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TreeView boneTree;
        private System.Windows.Forms.TabControl tabControl1;
    }
}
