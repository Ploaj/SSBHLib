namespace CrossMod.GUI
{
    partial class RenderSettingsMenu
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
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.redButton = new System.Windows.Forms.Button();
            this.greenButton = new System.Windows.Forms.Button();
            this.blueButton = new System.Windows.Forms.Button();
            this.alphaButton = new System.Windows.Forms.Button();
            this.diffuseCB = new System.Windows.Forms.CheckBox();
            this.specularCB = new System.Windows.Forms.CheckBox();
            this.wireframeCB = new System.Windows.Forms.CheckBox();
            this.renderModeComboBox = new System.Windows.Forms.ComboBox();
            this.paramTextBox = new System.Windows.Forms.TextBox();
            this.dittoCB = new System.Windows.Forms.CheckBox();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.redButton);
            this.flowLayoutPanel1.Controls.Add(this.greenButton);
            this.flowLayoutPanel1.Controls.Add(this.blueButton);
            this.flowLayoutPanel1.Controls.Add(this.alphaButton);
            this.flowLayoutPanel1.Controls.Add(this.diffuseCB);
            this.flowLayoutPanel1.Controls.Add(this.specularCB);
            this.flowLayoutPanel1.Controls.Add(this.wireframeCB);
            this.flowLayoutPanel1.Controls.Add(this.dittoCB);
            this.flowLayoutPanel1.Controls.Add(this.renderModeComboBox);
            this.flowLayoutPanel1.Controls.Add(this.paramTextBox);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(327, 146);
            this.flowLayoutPanel1.TabIndex = 0;
            // 
            // redButton
            // 
            this.redButton.Location = new System.Drawing.Point(3, 3);
            this.redButton.Name = "redButton";
            this.redButton.Size = new System.Drawing.Size(75, 23);
            this.redButton.TabIndex = 0;
            this.redButton.Text = "Red";
            this.redButton.UseVisualStyleBackColor = true;
            this.redButton.Click += new System.EventHandler(this.redButton_Click);
            // 
            // greenButton
            // 
            this.greenButton.Location = new System.Drawing.Point(84, 3);
            this.greenButton.Name = "greenButton";
            this.greenButton.Size = new System.Drawing.Size(75, 23);
            this.greenButton.TabIndex = 1;
            this.greenButton.Text = "Green";
            this.greenButton.UseVisualStyleBackColor = true;
            this.greenButton.Click += new System.EventHandler(this.greenButton_Click);
            // 
            // blueButton
            // 
            this.blueButton.Location = new System.Drawing.Point(165, 3);
            this.blueButton.Name = "blueButton";
            this.blueButton.Size = new System.Drawing.Size(75, 23);
            this.blueButton.TabIndex = 2;
            this.blueButton.Text = "Blue";
            this.blueButton.UseVisualStyleBackColor = true;
            this.blueButton.Click += new System.EventHandler(this.blueButton_Click);
            // 
            // alphaButton
            // 
            this.alphaButton.Location = new System.Drawing.Point(246, 3);
            this.alphaButton.Name = "alphaButton";
            this.alphaButton.Size = new System.Drawing.Size(75, 23);
            this.alphaButton.TabIndex = 3;
            this.alphaButton.Text = "Alpha";
            this.alphaButton.UseVisualStyleBackColor = true;
            this.alphaButton.Click += new System.EventHandler(this.alphaButton_Click);
            // 
            // diffuseCB
            // 
            this.diffuseCB.AutoSize = true;
            this.diffuseCB.Location = new System.Drawing.Point(3, 32);
            this.diffuseCB.Name = "diffuseCB";
            this.diffuseCB.Size = new System.Drawing.Size(95, 17);
            this.diffuseCB.TabIndex = 6;
            this.diffuseCB.Text = "Enable Diffuse";
            this.diffuseCB.UseVisualStyleBackColor = true;
            this.diffuseCB.CheckedChanged += new System.EventHandler(this.diffuseCB_CheckedChanged);
            // 
            // specularCB
            // 
            this.specularCB.AutoSize = true;
            this.specularCB.Location = new System.Drawing.Point(104, 32);
            this.specularCB.Name = "specularCB";
            this.specularCB.Size = new System.Drawing.Size(104, 17);
            this.specularCB.TabIndex = 7;
            this.specularCB.Text = "Enable Specular";
            this.specularCB.UseVisualStyleBackColor = true;
            this.specularCB.CheckedChanged += new System.EventHandler(this.specularCB_CheckedChanged);
            // 
            // wireframeCB
            // 
            this.wireframeCB.AutoSize = true;
            this.wireframeCB.Location = new System.Drawing.Point(3, 55);
            this.wireframeCB.Name = "wireframeCB";
            this.wireframeCB.Size = new System.Drawing.Size(112, 17);
            this.wireframeCB.TabIndex = 8;
            this.wireframeCB.Text = "Render Wireframe";
            this.wireframeCB.UseVisualStyleBackColor = true;
            this.wireframeCB.CheckedChanged += new System.EventHandler(this.wireframeCB_CheckedChanged);
            // 
            // renderModeComboBox
            // 
            this.renderModeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.renderModeComboBox.FormattingEnabled = true;
            this.renderModeComboBox.Items.AddRange(new object[] {
            "Shaded",
            "Col",
            "Prm",
            "Nor",
            "Emi",
            "Vertex Color",
            "Normals",
            "Tangent",
            "Bake Color",
            "Param ID (Vector 4 Only)"});
            this.renderModeComboBox.Location = new System.Drawing.Point(3, 78);
            this.renderModeComboBox.Name = "renderModeComboBox";
            this.renderModeComboBox.Size = new System.Drawing.Size(318, 21);
            this.renderModeComboBox.TabIndex = 4;
            this.renderModeComboBox.SelectedIndexChanged += new System.EventHandler(this.renderModeComboBox_SelectedIndexChanged);
            // 
            // paramTextBox
            // 
            this.paramTextBox.Location = new System.Drawing.Point(3, 105);
            this.paramTextBox.Name = "paramTextBox";
            this.paramTextBox.Size = new System.Drawing.Size(318, 20);
            this.paramTextBox.TabIndex = 5;
            this.paramTextBox.TextChanged += new System.EventHandler(this.paramTextBox_TextChanged);
            // 
            // dittoCB
            // 
            this.dittoCB.AutoSize = true;
            this.dittoCB.Location = new System.Drawing.Point(121, 55);
            this.dittoCB.Name = "dittoCB";
            this.dittoCB.Size = new System.Drawing.Size(127, 17);
            this.dittoCB.TabIndex = 9;
            this.dittoCB.Text = "Metamon Form (Ditto)";
            this.dittoCB.UseVisualStyleBackColor = true;
            this.dittoCB.CheckedChanged += new System.EventHandler(this.dittoCB_CheckedChanged);
            // 
            // RenderSettingsMenu
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(327, 146);
            this.Controls.Add(this.flowLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "RenderSettingsMenu";
            this.Text = "Render Settings";
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Button redButton;
        private System.Windows.Forms.Button greenButton;
        private System.Windows.Forms.Button blueButton;
        private System.Windows.Forms.Button alphaButton;
        private System.Windows.Forms.ComboBox renderModeComboBox;
        private System.Windows.Forms.TextBox paramTextBox;
        private System.Windows.Forms.CheckBox diffuseCB;
        private System.Windows.Forms.CheckBox specularCB;
        private System.Windows.Forms.CheckBox wireframeCB;
        private System.Windows.Forms.CheckBox dittoCB;
    }
}