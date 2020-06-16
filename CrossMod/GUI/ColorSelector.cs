using CrossMod.Nodes;
using CrossMod.Rendering;
using OpenTK;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace CrossMod.GUI
{
    public partial class ColorSelector : Form
    {
        public ColorSelector()
        {
            InitializeComponent();
            for (int i = 0; i < Collision.IDColors.Length; i++)
            {
                Color hitboxColor = Vector3ToColor(Collision.IDColors[i]);
                ListViewItem listViewItem = new ListViewItem()
                {
                    BackColor = hitboxColor,
                    Text = string.Format("Hitbox ID {0} | Color: {1}", hitboxList.Items.Count, ColorTranslator.ToHtml(hitboxColor)),
                };

                hitboxList.Items.Add(listViewItem);
            }

            for (int i = 0; i < ParamNodeContainer.HitData.Length; i++)
            {
                Collision collision = ParamNodeContainer.HitData[i];
                Color hurtboxColor = Vector3ToColor(collision.Color);

                ListViewItem listViewItem = new ListViewItem()
                {
                    BackColor = hurtboxColor,
                    Text = string.Format("Bone ID: {0} | Color: {1}", collision.Bone, ColorTranslator.ToHtml(hurtboxColor)),
                    Checked = true
                };
                hurtboxList.Items.Add(listViewItem);
            }
        }

        private void DisplayContextStrip(object sender, MouseEventArgs e)
        {
            ListView listView = sender as ListView;
            if (e.Button == MouseButtons.Right)
            {
                var hitTestInfo = listView.HitTest(e.X, e.Y);
                if (hitTestInfo.Item != null)
                {
                    var loc = e.Location;
                    loc.Offset(listView.Location);

                    if (listView.Name == "hitboxList")
                        contextMenuStrip1.Show(this, loc);
                    else if (listView.Name == "hurtboxList")
                        contextMenuStrip2.Show(this, loc);
                }
            }
        }

        private void ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (hurtboxList.SelectedItems.Count > 0)
            {
                Color color = SelectColor();
                for (int i = 0; i < hurtboxList.SelectedItems.Count; i++)
                {
                    var itemIndex = hurtboxList.SelectedIndices[i];
                    var item = hurtboxList.SelectedItems[i];

                    if (color != Color.Empty)
                    {
                        item.BackColor = color;
                        item.Text = string.Format("Bone ID: {0} | Color: {1}",
                                                   ParamNodeContainer.HitData[itemIndex].Bone,
                                                   ColorTranslator.ToHtml(color));
                        ParamNodeContainer.HitData[itemIndex].Color = ColorToVector3(color);
                    }
                }
            }
        }

        private void ReplaceSelectedColorToolStripMenuItem_Click(object sender, EventArgs e)
        {   
            if (hitboxList.SelectedItems.Count > 0)
            {
                var item = hitboxList.SelectedItems[0];
                var itemIndex = hitboxList.SelectedIndices[0];

                Color color = SelectColor();
                if (color != Color.Empty)
                {
                    item.BackColor = color;
                    item.Text = string.Format("Hitbox ID {0} | Color: {1}", itemIndex, ColorTranslator.ToHtml(color));
                    Collision.IDColors[itemIndex] = ColorToVector3(color);
                }
            }
        }
        private Vector3 ColorToVector3(Color color)
        {
            return new Vector3(color.R / 255f,
                               color.G / 255f,
                               color.B / 255f);
        }

        private Color Vector3ToColor(Vector3 colorVector)
        {
            return Color.FromArgb((int)(colorVector.X * 255),
                                  (int)(colorVector.Y * 255),
                                  (int)(colorVector.Z * 255));
        }
        private Color SelectColor()
        {
            Color color = Color.Empty;
            using (ColorDialog colorDialog = new ColorDialog())
            {
                if (colorDialog.ShowDialog() == DialogResult.OK)
                    color = colorDialog.Color;
            }
            return color;
        }

        private void HurtboxList_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            if (hurtboxList.SelectedItems.Count > 0)
            {
                for (int i = 0; i < hurtboxList.SelectedItems.Count; i++)
                {
                    var itemIndex = hurtboxList.SelectedIndices[i];
                    ParamNodeContainer.HitData[itemIndex].Enabled = e.Item.Checked;
                } 
            }    
        }
    }
}
