using System;
using System.Collections.Generic;
using System.Windows.Forms;
using CrossMod.Rendering;
using System.Globalization;

namespace CrossMod.GUI
{
    public partial class RenderSettingsMenu : Form
    {
        private readonly List<string> renderModes = new List<string>()
        {
            "Shaded",
            "Col",
            "Prm",
            "Nor",
            "Emi",
            "Bake Lit",
            "Vertex Color",
            "Normals",
            "Tangents",
            "Bake Color",
            "Param ID (Vec4 or Booleans)"
        };

        public RenderSettingsMenu()
        {
            InitializeComponent();

            redButton.ForeColor = RenderSettings.renderChannels.X == 1 ? System.Drawing.Color.Red : System.Drawing.Color.Gray;
            greenButton.ForeColor = RenderSettings.renderChannels.Y == 1 ? System.Drawing.Color.Green : System.Drawing.Color.Gray;
            blueButton.ForeColor = RenderSettings.renderChannels.Z == 1 ? System.Drawing.Color.Blue : System.Drawing.Color.Gray;
            alphaButton.ForeColor = RenderSettings.renderChannels.W == 1 ? System.Drawing.Color.Black : System.Drawing.Color.Gray;

            renderModeComboBox.Items.Clear();
            renderModeComboBox.Items.AddRange(renderModes.ToArray());
            renderModeComboBox.SelectedIndex = RenderSettings.renderMode;
        }

        private void renderModeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            RenderSettings.renderMode = renderModeComboBox.SelectedIndex;
            RenderSettings.useDebugShading = RenderSettings.renderMode != 0;
        }

        private void redButton_Click(object sender, EventArgs e)
        {
            SetChannel(redButton, System.Drawing.Color.Red, 0);
        }

        private void greenButton_Click(object sender, EventArgs e)
        {
            SetChannel(greenButton, System.Drawing.Color.Green, 1);
        }

        private void blueButton_Click(object sender, EventArgs e)
        {
            SetChannel(blueButton, System.Drawing.Color.Blue, 2);
        }

        private void alphaButton_Click(object sender, EventArgs e)
        {
            SetChannel(alphaButton, System.Drawing.Color.Black, 3);
        }

        private void SetChannel(Button button, System.Drawing.Color activeColor, int channelIndex)
        {
            if (RenderSettings.renderChannels[channelIndex] == 1)
            {
                RenderSettings.renderChannels[channelIndex] = 0;
                button.ForeColor = System.Drawing.Color.Gray;
            }
            else
            {
                RenderSettings.renderChannels[channelIndex] = 1;
                button.ForeColor = activeColor;
            }
        }

        private void paramTextBox_TextChanged(object sender, EventArgs e)
        {
            long.TryParse(paramTextBox.Text, NumberStyles.HexNumber, CultureInfo.CurrentCulture, out long id);
            RenderSettings.paramId = id;
        }
    }
}
