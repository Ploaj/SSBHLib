namespace CrossMod.GUI
{
    partial class CameraControl
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.posY_tb = new System.Windows.Forms.TextBox();
            this.posZ_tb = new System.Windows.Forms.TextBox();
            this.posX_tb = new System.Windows.Forms.TextBox();
            this.rotX_tb = new System.Windows.Forms.TextBox();
            this.rotY_tb = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.update_button = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.posY_tb);
            this.groupBox1.Controls.Add(this.posZ_tb);
            this.groupBox1.Controls.Add(this.posX_tb);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Left;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.MinimumSize = new System.Drawing.Size(0, 100);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(132, 101);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Position";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 74);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(14, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "Z";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 48);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(14, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "Y";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(14, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "X";
            // 
            // posY_tb
            // 
            this.posY_tb.Location = new System.Drawing.Point(36, 45);
            this.posY_tb.Name = "posY_tb";
            this.posY_tb.Size = new System.Drawing.Size(90, 20);
            this.posY_tb.TabIndex = 5;
            // 
            // posZ_tb
            // 
            this.posZ_tb.Location = new System.Drawing.Point(36, 71);
            this.posZ_tb.Name = "posZ_tb";
            this.posZ_tb.Size = new System.Drawing.Size(90, 20);
            this.posZ_tb.TabIndex = 4;
            // 
            // posX_tb
            // 
            this.posX_tb.Location = new System.Drawing.Point(36, 19);
            this.posX_tb.Name = "posX_tb";
            this.posX_tb.Size = new System.Drawing.Size(90, 20);
            this.posX_tb.TabIndex = 3;
            // 
            // rotX_tb
            // 
            this.rotX_tb.Location = new System.Drawing.Point(36, 19);
            this.rotX_tb.Name = "rotX_tb";
            this.rotX_tb.Size = new System.Drawing.Size(90, 20);
            this.rotX_tb.TabIndex = 0;
            // 
            // rotY_tb
            // 
            this.rotY_tb.Location = new System.Drawing.Point(36, 45);
            this.rotY_tb.Name = "rotY_tb";
            this.rotY_tb.Size = new System.Drawing.Size(90, 20);
            this.rotY_tb.TabIndex = 1;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 48);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(14, 13);
            this.label4.TabIndex = 2;
            this.label4.Text = "Y";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 22);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(14, 13);
            this.label5.TabIndex = 3;
            this.label5.Text = "X";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.update_button);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.rotX_tb);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.rotY_tb);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Location = new System.Drawing.Point(132, 0);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(132, 101);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Rotation";
            // 
            // update_button
            // 
            this.update_button.Location = new System.Drawing.Point(9, 71);
            this.update_button.Name = "update_button";
            this.update_button.Size = new System.Drawing.Size(117, 20);
            this.update_button.TabIndex = 4;
            this.update_button.Text = "Update Camera";
            this.update_button.UseVisualStyleBackColor = true;
            this.update_button.Click += new System.EventHandler(this.update_button_Click);
            // 
            // CameraControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(264, 101);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "CameraControl";
            this.Text = "Camera";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox posY_tb;
        private System.Windows.Forms.TextBox posZ_tb;
        private System.Windows.Forms.TextBox posX_tb;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox rotY_tb;
        private System.Windows.Forms.TextBox rotX_tb;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button update_button;
    }
}