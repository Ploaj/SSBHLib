using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CrossMod.GUI
{
    public partial class BGSettings : Form
    {

        public static float red { get; set; } = 0.25f;
        public static float green { get; set; } = 0.25f;
        public static float blue { get; set; } = 0.25f;
        public static bool renderAlpha { get; set; } = false;

        public BGSettings()
        {
            InitializeComponent();
        }

        private void RedBox_TextChanged(object sender, EventArgs e)
        {
            if (System.Text.RegularExpressions.Regex.IsMatch(redBox.Text, "[^0-9]"))
            {
                redBox.Text = redBox.Text.Remove(redBox.Text.Length - 1);
            }
            if (redBox.Text == "")
            {
                redBox.Text = "0";
            }
            if (Int32.Parse(redBox.Text) >= 256)
            {
                redBox.Text = "255";
            }
            red = float.Parse(redBox.Text) / 255.0f;
        }

        private void GreenBox_TextChanged(object sender, EventArgs e)
        {
            if (System.Text.RegularExpressions.Regex.IsMatch(greenBox.Text, "[^0-9]"))
            {
                greenBox.Text = greenBox.Text.Remove(greenBox.Text.Length - 1);
            }
            if (greenBox.Text == "")
            {
                greenBox.Text = "0";
            }
            if (Int32.Parse(greenBox.Text) >= 256)
            {
                greenBox.Text = "255";
            }
            green = float.Parse(greenBox.Text) / 255.0f;
        }

        private void BlueBox_TextChanged(object sender, EventArgs e)
        {
            if (System.Text.RegularExpressions.Regex.IsMatch(blueBox.Text, "[^0-9]"))
            {
                blueBox.Text = blueBox.Text.Remove(blueBox.Text.Length - 1);
            }
            if (blueBox.Text == "")
            {
                blueBox.Text = "0";
            }
            if (Int32.Parse(blueBox.Text) >= 256)
            {
                blueBox.Text = "255";
            }
            blue = float.Parse(blueBox.Text) / 255.0f;
        }

        private void AlphaToggle_CheckedChanged(object sender, EventArgs e)
        {
            renderAlpha = alphaToggle.Checked;
        }
    }
}
