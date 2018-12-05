using CrossMod.Rendering;
using GenericValueEditor;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

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

            var editorGenerator = new EditorGenerator<RenderSettings>(RenderSettings.Instance);
            editorGenerator.AddEditorControls(flowLayoutPanel1);

            flowLayoutPanel1.Resize += FlowLayoutPanel1_Resize;
        }

        private void FlowLayoutPanel1_Resize(object sender, EventArgs e)
        {
            GenericValueEditor.Utils.GuiUtils.ScaleControlsHorizontallyToLayoutWidth(flowLayoutPanel1);
        }
    }
}
