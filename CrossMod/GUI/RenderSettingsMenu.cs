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
            if (RenderSettings.renderChannels.X == 1)
            {
                RenderSettings.renderChannels.X = 0;
                redButton.ForeColor = System.Drawing.Color.Gray;
            }
            else
            {
                RenderSettings.renderChannels.X = 1;
                redButton.ForeColor = System.Drawing.Color.Red;
            }
        }

        private void greenButton_Click(object sender, EventArgs e)
        {
            if (RenderSettings.renderChannels.Y == 1)
            {
                RenderSettings.renderChannels.Y = 0;
                greenButton.ForeColor = System.Drawing.Color.Gray;
            }
            else
            {
                RenderSettings.renderChannels.Y = 1;
                greenButton.ForeColor = System.Drawing.Color.Green;
            }
        }

        private void blueButton_Click(object sender, EventArgs e)
        {
            if (RenderSettings.renderChannels.Z == 1)
            {
                RenderSettings.renderChannels.Z = 0;
                blueButton.ForeColor = System.Drawing.Color.Gray;
            }
            else
            {
                RenderSettings.renderChannels.Z = 1;
                blueButton.ForeColor = System.Drawing.Color.Blue;
            }
        }

        private void alphaButton_Click(object sender, EventArgs e)
        {
            if (RenderSettings.renderChannels.W == 1)
            {
                RenderSettings.renderChannels.W = 0;
                alphaButton.ForeColor = System.Drawing.Color.Gray;
            }
            else
            {
                RenderSettings.renderChannels.W = 1;
                alphaButton.ForeColor = System.Drawing.Color.Black;
            }
        }

        private void paramTextBox_TextChanged(object sender, EventArgs e)
        {
            long.TryParse(paramTextBox.Text, NumberStyles.HexNumber, CultureInfo.CurrentCulture, out long id);
            RenderSettings.paramId = id;
        }
    }
}
