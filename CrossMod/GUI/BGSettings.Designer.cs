namespace CrossMod.GUI
{
    partial class BGSettings
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.redBox = new System.Windows.Forms.TextBox();
            this.greenBox = new System.Windows.Forms.TextBox();
            this.blueBox = new System.Windows.Forms.TextBox();
            this.alphaToggle = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(49, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(27, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Red";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(49, 41);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(36, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Green";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(49, 64);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(28, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Blue";
            // 
            // redBox
            // 
            this.redBox.Location = new System.Drawing.Point(10, 11);
            this.redBox.Name = "redBox";
            this.redBox.Size = new System.Drawing.Size(33, 20);
            this.redBox.TabIndex = 3;
            this.redBox.Text = "64";
            this.redBox.TextChanged += new System.EventHandler(this.RedBox_TextChanged);
            // 
            // greenBox
            // 
            this.greenBox.Location = new System.Drawing.Point(10, 34);
            this.greenBox.Name = "greenBox";
            this.greenBox.Size = new System.Drawing.Size(33, 20);
            this.greenBox.TabIndex = 4;
            this.greenBox.Text = "64";
            this.greenBox.TextChanged += new System.EventHandler(this.GreenBox_TextChanged);
            // 
            // blueBox
            // 
            this.blueBox.Location = new System.Drawing.Point(10, 57);
            this.blueBox.Name = "blueBox";
            this.blueBox.Size = new System.Drawing.Size(33, 20);
            this.blueBox.TabIndex = 5;
            this.blueBox.Text = "64";
            this.blueBox.TextChanged += new System.EventHandler(this.BlueBox_TextChanged);
            // 
            // alphaToggle
            // 
            this.alphaToggle.AutoSize = true;
            this.alphaToggle.Location = new System.Drawing.Point(10, 80);
            this.alphaToggle.Name = "alphaToggle";
            this.alphaToggle.Size = new System.Drawing.Size(91, 17);
            this.alphaToggle.TabIndex = 6;
            this.alphaToggle.Text = "Render Alpha";
            this.alphaToggle.UseVisualStyleBackColor = true;
            this.alphaToggle.CheckedChanged += new System.EventHandler(this.AlphaToggle_CheckedChanged);
            // 
            // BGSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(167, 127);
            this.Controls.Add(this.alphaToggle);
            this.Controls.Add(this.blueBox);
            this.Controls.Add(this.greenBox);
            this.Controls.Add(this.redBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "BGSettings";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox redBox;
        private System.Windows.Forms.TextBox greenBox;
        private System.Windows.Forms.TextBox blueBox;
        private System.Windows.Forms.CheckBox alphaToggle;
    }
}