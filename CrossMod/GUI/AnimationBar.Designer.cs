namespace CrossMod.GUI
{
    partial class AnimationBar
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
            this.currentFrame_UpDown = new System.Windows.Forms.NumericUpDown();
            this.totalFrame = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.playButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.currentFrame_UpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.totalFrame)).BeginInit();
            this.SuspendLayout();
            // 
            // currentFrame_UpDown
            // 
            this.currentFrame_UpDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.currentFrame_UpDown.DecimalPlaces = 4;
            this.currentFrame_UpDown.Location = new System.Drawing.Point(424, 3);
            this.currentFrame_UpDown.Name = "currentFrame_UpDown";
            this.currentFrame_UpDown.Size = new System.Drawing.Size(64, 20);
            this.currentFrame_UpDown.TabIndex = 1;
            this.currentFrame_UpDown.ValueChanged += new System.EventHandler(this.currentFrame_ValueChanged);
            // 
            // totalFrame
            // 
            this.totalFrame.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.totalFrame.Enabled = false;
            this.totalFrame.Location = new System.Drawing.Point(513, 3);
            this.totalFrame.Name = "totalFrame";
            this.totalFrame.Size = new System.Drawing.Size(64, 20);
            this.totalFrame.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(495, 5);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(12, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "/";
            // 
            // playButton
            // 
            this.playButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.playButton.Location = new System.Drawing.Point(424, 29);
            this.playButton.Name = "playButton";
            this.playButton.Size = new System.Drawing.Size(153, 28);
            this.playButton.TabIndex = 4;
            this.playButton.Text = ">";
            this.playButton.UseVisualStyleBackColor = true;
            this.playButton.Click += new System.EventHandler(this.playButton_Click);
            // 
            // AnimationBar
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.playButton);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.totalFrame);
            this.Controls.Add(this.currentFrame_UpDown);
            this.Name = "AnimationBar";
            this.Size = new System.Drawing.Size(580, 60);
            ((System.ComponentModel.ISupportInitialize)(this.currentFrame_UpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.totalFrame)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.NumericUpDown currentFrame_UpDown;
        private System.Windows.Forms.NumericUpDown totalFrame;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button playButton;
    }
}
