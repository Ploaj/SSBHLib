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
            this.animationTrack = new System.Windows.Forms.TrackBar();
            this.currentFrame = new System.Windows.Forms.NumericUpDown();
            this.totalFrame = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.playButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.animationTrack)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.currentFrame)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.totalFrame)).BeginInit();
            this.SuspendLayout();
            // 
            // animationTrack
            // 
            this.animationTrack.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.animationTrack.Location = new System.Drawing.Point(3, 7);
            this.animationTrack.Name = "animationTrack";
            this.animationTrack.Size = new System.Drawing.Size(574, 45);
            this.animationTrack.TabIndex = 0;
            this.animationTrack.ValueChanged += new System.EventHandler(this.animationTrack_ValueChanged);
            // 
            // currentFrame
            // 
            this.currentFrame.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.currentFrame.Location = new System.Drawing.Point(424, 45);
            this.currentFrame.Name = "currentFrame";
            this.currentFrame.Size = new System.Drawing.Size(64, 20);
            this.currentFrame.TabIndex = 1;
            this.currentFrame.ValueChanged += new System.EventHandler(this.currentFrame_ValueChanged);
            // 
            // totalFrame
            // 
            this.totalFrame.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.totalFrame.Enabled = false;
            this.totalFrame.Location = new System.Drawing.Point(513, 45);
            this.totalFrame.Name = "totalFrame";
            this.totalFrame.Size = new System.Drawing.Size(64, 20);
            this.totalFrame.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(494, 47);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(12, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "/";
            // 
            // playButton
            // 
            this.playButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.playButton.Location = new System.Drawing.Point(424, 67);
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
            this.Controls.Add(this.currentFrame);
            this.Controls.Add(this.animationTrack);
            this.Name = "AnimationBar";
            this.Size = new System.Drawing.Size(580, 106);
            ((System.ComponentModel.ISupportInitialize)(this.animationTrack)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.currentFrame)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.totalFrame)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TrackBar animationTrack;
        private System.Windows.Forms.NumericUpDown currentFrame;
        private System.Windows.Forms.NumericUpDown totalFrame;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button playButton;
    }
}
