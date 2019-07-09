using CrossMod.GUI;
using CrossMod.Nodes;
using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using CrossMod.IO;
using System.Collections.Generic;

namespace CrossMod
{
    public partial class MainForm : Form
    {
        public static ImageList iconList;

        // Controls
        private ModelViewport modelViewport;

        private ContextMenu fileTreeContextMenu;

        private CameraControl cameraControl;

        public MainForm()
        {
            InitializeComponent();

            modelViewport = new ModelViewport
            {
                Dock = DockStyle.Fill
            };

            fileTreeContextMenu = new ContextMenu();

            iconList = iconList = new ImageList();
            iconList.ImageSize = new Size(24, 24);

            iconList.Images.Add("unknown", Properties.Resources.ico_unknown);
            iconList.Images.Add("folder", Properties.Resources.ico_folder);
            iconList.Images.Add("model", Properties.Resources.ico_model);
            iconList.Images.Add("mesh", Properties.Resources.ico_mesh);
            iconList.Images.Add("material", Properties.Resources.ico_material);
            iconList.Images.Add("skeleton", Properties.Resources.ico_skeleton);
            iconList.Images.Add("texture", Properties.Resources.ico_texture);
            iconList.Images.Add("animation", Properties.Resources.ico_animation);

            fileTree.ImageList = iconList;
        }

        public void HideControl()
        {
            contentBox.Controls.Clear();
        }

        public void ShowModelViewport()
        {
            contentBox.Controls.Clear();
            contentBox.Controls.Add(modelViewport);
        }

        private void openFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string folderPath = FileTools.TryOpenFolder();
            if (string.IsNullOrEmpty(folderPath))
                return;

            LoadWorkspace(folderPath);

            ShowModelViewport();
        }

        /// <summary>
        /// Loads a directory and all sub-directories into the filetree.
        /// </summary>
        /// <param name="folderPath"></param>
        private void LoadWorkspace(string folderPath)
        {
            var mainNode = new DirectoryNode(folderPath);
            mainNode.Open();
            fileTree.Nodes.Add(mainNode);

            mainNode.Expand();

            // Enable rendering of the model if we have directly selected a model file.
            // Nested ones won't render a model
            SKEL_Node Skel = null;
            foreach (FileNode node in mainNode.Nodes)
            {
                if (node.Text?.EndsWith("numdlb") == true)
                {
                    fileTree.SelectedNode = node as FileNode;
                }
                else if (Skel == null && node is SKEL_Node)
                {
                    Skel = node as SKEL_Node;
                }
            }
            if (Skel == null)
                return;
            foreach (FileNode node in mainNode.Nodes)
            {
                if (node is ScriptNode scriptNode)
                {
                    scriptNode.SkelNode = Skel;
                    modelViewport.ScriptNode = scriptNode;
                    //only do this once, there should only be one anyway
                    break;
                }
            }
            ParamNodeContainer.SkelNode = Skel;
        }

        private void fileTree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            //oof
            if (fileTree.SelectedNode.Text.EndsWith("nuanmb") && modelViewport.ScriptNode != null)
            {
                modelViewport.ScriptNode.CurrentAnimationName = fileTree.SelectedNode.Text;
            }

            if (fileTree.SelectedNode is NUTEX_Node texture)
            {
                ShowModelViewport();
                modelViewport.UpdateTexture(texture);
            }
            else if (fileTree.SelectedNode is IRenderableNode renderableNode)
            {
                ShowModelViewport();
                var node = (FileNode)fileTree.SelectedNode;
                modelViewport.AddRenderableNode(node.AbsolutePath, renderableNode);
                modelViewport.UpdateTexture(null);
            }
            else if (fileTree.SelectedNode is NUANIM_Node animation)
            {
                ShowModelViewport();
                modelViewport.RenderableAnimation = (Rendering.IRenderableAnimation)animation.GetRenderableNode();
                modelViewport.UpdateTexture(null);
            }
            
            modelViewport.RenderFrame();
        }

        private void reloadShadersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Force the shaders to be generated again.
            Rendering.ShaderContainer.ReloadShaders();
        }

        private void renderSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var settingsMenu = new RenderSettingsMenu();
            settingsMenu.Show();
        }

        private void fileTree_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                // Point where the mouse is clicked.
                Point p = new Point(e.X, e.Y);

                // Get the node that the user has clicked.
                TreeNode node = fileTree.GetNodeAt(p);
                if (node != null)
                {
                    fileTree.SelectedNode = node;
                    fileTreeContextMenu.MenuItems.Clear();

                    // gather all options for this node
                    if (node is IExportableTextureNode exportableTextureNode)
                    {
                        MenuItem ExportPNG = new MenuItem("Export As");
                        ExportPNG.Click += exportExportableTexture;
                        ExportPNG.Tag = exportableTextureNode;
                        fileTreeContextMenu.MenuItems.Add(ExportPNG);
                    }
                    if (node is IExportableModelNode exportableNode)
                    {
                        MenuItem ExportSMD = new MenuItem("Export As");
                        ExportSMD.Click += exportExportableModelAsSMD;
                        ExportSMD.Tag = exportableNode;
                        fileTreeContextMenu.MenuItems.Add(ExportSMD);
                    }
                    if (node is IExportableAnimationNode exportableAnimNode)
                    {
                        MenuItem ExportAnim = new MenuItem("Export Anim");
                        ExportAnim.Click += exportExportableAnimation;
                        ExportAnim.Tag = exportableAnimNode;
                        fileTreeContextMenu.MenuItems.Add(ExportAnim);
                    }

                    // show if it has at least 1 option
                    if (fileTreeContextMenu.MenuItems.Count != 0)
                        fileTreeContextMenu.Show(fileTree, p);
                }
            }
        }

        private void exportExportableTexture(object sender, EventArgs args)
        {
            if (FileTools.TrySaveFile(out string fileName, "Portable Networks Graphic(*.png)|*.png", ((MenuItem)sender).Tag.ToString()))
            {
                // need to get RSkeleton First for some types
                if (fileName.EndsWith(".png"))
                {
                    ((IExportableTextureNode)((MenuItem)sender).Tag).SaveTexturePNG(fileName);
                }
            }
        }

        private void exportExportableAnimation(object sender, EventArgs args)
        {
            if (FileTools.TrySaveFile(out string fileName, "Supported Files(*.smd, *.seanim, *.anim)|*.smd;*.seanim;*.anim"))
            {
                // need to get RSkeleton First for some types
                if (fileName.EndsWith(".smd") || fileName.EndsWith(".anim"))
                {
                    Rendering.RSkeleton SkeletonNode = null;
                    if (FileTools.TryOpenFile(out string SkeletonFileName, "SKEL (*.nusktb)|*.nusktb"))
                    {
                        if (SkeletonFileName != null)
                        {
                            SKEL_Node node = new SKEL_Node(SkeletonFileName);
                            node.Open();
                            SkeletonNode = (Rendering.RSkeleton)node.GetRenderableNode();
                        }
                    }
                    if (SkeletonNode == null)
                    {
                        MessageBox.Show("No Skeleton File Selected");
                        return;
                    }

                    if (fileName.EndsWith(".anim"))
                    {
                        bool Ordinal = false;
                        DialogResult dialogResult = MessageBox.Show("In most cases choose \"No\"", "Use ordinal bone order?", MessageBoxButtons.YesNo);
                        if (dialogResult == DialogResult.Yes)
                            Ordinal = true;
                        IO_MayaANIM.ExportIOAnimationAsANIM(fileName, ((IExportableAnimationNode)((MenuItem)sender).Tag).GetIOAnimation(), SkeletonNode, Ordinal);
                    }

                    if (fileName.EndsWith(".smd"))
                        IO_SMD.ExportIOAnimationAsSMD(fileName, ((IExportableAnimationNode)((MenuItem)sender).Tag).GetIOAnimation(), SkeletonNode);
                }

                // other types like SEAnim go here
                if (fileName.EndsWith(".seanim"))
                {
                    IO_SEANIM.ExportIOAnimation(fileName, ((IExportableAnimationNode)((MenuItem)sender).Tag).GetIOAnimation());
                }
            }
        }

        private void exportExportableModelAsSMD(object sender, EventArgs args)
        {
            if (FileTools.TrySaveFile(out string fileName, "Supported Files(*.smd*.obj*.dae*.ply)|*.smd;*.obj;*.dae;*.ply"))
            {
                if (fileName.EndsWith(".smd"))
                    IO_SMD.ExportIOModelAsSMD(fileName, ((IExportableModelNode)((MenuItem)sender).Tag).GetIOModel());
                if (fileName.EndsWith(".obj"))
                    IO_OBJ.ExportIOModelAsOBJ(fileName, ((IExportableModelNode)((MenuItem)sender).Tag).GetIOModel());
                if (fileName.EndsWith(".dae"))
                {
                    bool Optimize = false;
                    bool Materials = false;
                    DialogResult dialogResult = MessageBox.Show("Smaller filesize but takes longer to export", "Optimize Geometry?", MessageBoxButtons.YesNo);
                    if (dialogResult == DialogResult.Yes)
                    {
                        Optimize = true;
                    }
                    dialogResult = MessageBox.Show("Export texture names inside of dae?", "Export Materials?", MessageBoxButtons.YesNo);
                    if (dialogResult == DialogResult.Yes)
                    {
                        Materials = true;
                    }
                    IO_DAE.ExportIOModelAsDAE(fileName, ((IExportableModelNode)((MenuItem)sender).Tag).GetIOModel(), Optimize, Materials);
                }
                if (fileName.EndsWith(".ply"))
                    IO_PLY.ExportIOModelAsPLY(fileName, ((IExportableModelNode)((MenuItem)sender).Tag).GetIOModel());
            }
        }

        private void clearWorkspaceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClearWorkspace();
        }

        private void ClearWorkspace()
        {
            fileTree.Nodes.Clear();
            ParamNodeContainer.Unload();
            modelViewport.ClearFiles();
            HideControl();
            GC.Collect();
        }

        private void batchRenderModelsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BatchRenderModels();
        }

        private void BatchRenderModels()
        {
            string folderPath = FileTools.TryOpenFolder("Select Source Directory");
            if (string.IsNullOrEmpty(folderPath))
                return;

            string outputPath = FileTools.TryOpenFolder("Select PNG Output Directory");
            if (string.IsNullOrEmpty(outputPath))
                return;

            foreach (var file in Directory.EnumerateFiles(folderPath, "*model.numdlb", SearchOption.AllDirectories))
            {
                // Just render the first alt costume, which will include models without slot specific variants.
                //if (!file.Contains("c00"))
                //    continue;

                string sourceFolder = Directory.GetParent(file).FullName;

                LoadWorkspace(sourceFolder);

                ShowModelViewport();

                modelViewport.RenderFrame();

                // Save screenshot.
                using (var bmp = modelViewport.GetScreenshot())
                {
                    string condensedName = GetCondensedPathName(folderPath, file);
                    bmp.Save(Path.Combine(outputPath, $"{condensedName}.png"));
                }

                // Cleanup.
                ClearWorkspace();
                System.Diagnostics.Debug.WriteLine($"Rendered {sourceFolder}");
            }
        }

        private static string GetCondensedPathName(string folderPath, string file)
        {
            string condensedName = file.Replace(folderPath, "");
            condensedName = condensedName.Replace(Path.DirectorySeparatorChar, '_');
            condensedName = condensedName.Substring(1); // remove leading underscore
            return condensedName;
        }

        private void frameSelectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (fileTree.SelectedNode is NUMDL_Node node)
            {
                var rnumdl = node.GetRenderableNode() as Rendering.RNUMDL;
                modelViewport.FrameSelection(rnumdl.Model);
            }
        }

        private void printMaterialValuesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var folderPath = FileTools.TryOpenFolder("Select Source Directory");
            if (string.IsNullOrEmpty(folderPath))
                return;

            WriteMaterialValuesToFile(folderPath);
        }

        private static void WriteMaterialValuesToFile(string folderPath)
        {
            var enumValues = Enum.GetNames(typeof(SSBHLib.Formats.Materials.MatlEnums.ParamId));

            var valuesByParamId = new Dictionary<SSBHLib.Formats.Materials.MatlEnums.ParamId, HashSet<string>>();
            var outputByParamId = new Dictionary<SSBHLib.Formats.Materials.MatlEnums.ParamId, System.Text.StringBuilder>();

            foreach (var file in Directory.EnumerateFiles(folderPath, "*numatb", SearchOption.AllDirectories))
            {
                var matl = new MATL_Node(file);
                matl.Open();

                foreach (var entry in matl.Material.Entries)
                {
                    foreach (var attribute in entry.Attributes)
                    {
                        string text = $"{attribute.ParamID} {attribute.DataObject} {file.Replace(folderPath, "")}";

                        if (!outputByParamId.ContainsKey(attribute.ParamID))
                            outputByParamId.Add(attribute.ParamID, new System.Text.StringBuilder());

                        if (!valuesByParamId.ContainsKey(attribute.ParamID))
                            valuesByParamId.Add(attribute.ParamID, new HashSet<string>());

                        // Don't check duplicates for booleans.
                        if (attribute.DataType == SSBHLib.Formats.Materials.MatlEnums.ParamDataType.Boolean)
                        {
                            outputByParamId[attribute.ParamID].AppendLine(text);
                        }
                        else if (!valuesByParamId.ContainsKey(attribute.ParamID) || !valuesByParamId[attribute.ParamID].Contains(attribute.DataObject.ToString()))
                        {
                            outputByParamId[attribute.ParamID].AppendLine(text);
                            valuesByParamId[attribute.ParamID].Add(attribute.DataObject.ToString());
                        }
                    }
                }
            }

            foreach (var pair in outputByParamId)
            {
                File.WriteAllText($"{pair.Key}_unique_values.txt", pair.Value.ToString());
            }
        }

        private static void WriteAttributesToFile()
        {
            var folderPath = FileTools.TryOpenFolder("Select Source Directory");
            if (string.IsNullOrEmpty(folderPath))
                return;

            var meshesByAttribute = new Dictionary<string, List<string>>();


            foreach (var file in Directory.EnumerateFiles(folderPath, "*numshb", SearchOption.AllDirectories))
            {
                var node = new NUMSHB_Node(file);
                node.Open();

                UpdateMeshAttributes(folderPath, meshesByAttribute, file, node);
            }

            foreach (var pair in meshesByAttribute)
            {
                var outputText = new System.Text.StringBuilder();
                foreach (var value in pair.Value)
                {
                    outputText.AppendLine(value);
                }

                File.WriteAllText($"{pair.Key} Meshes.txt", outputText.ToString());
            }
        }

        private static void UpdateMeshAttributes(string folderPath, Dictionary<string, List<string>> meshesByAttribute, string file, NUMSHB_Node node)
        {
            foreach (var meshObject in node.mesh.Objects)
            {
                foreach (var attribute in meshObject.Attributes)
                {
                    if (!meshesByAttribute.ContainsKey(attribute.Name))
                        meshesByAttribute.Add(attribute.Name, new List<string>());

                    var text = $"{meshObject.Name} {file.Replace(folderPath, "")}";
                    if (!meshesByAttribute[attribute.Name].Contains(text))
                    {
                        meshesByAttribute[attribute.Name].Add(text);
                    }
                }
            }
        }

        private void printAttributesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            WriteAttributesToFile();
        }

        /// <summary>
        /// When a node 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fileTree_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            if (e.Node is DirectoryNode node)
            {
                node.OpenNodes();
            }
        }

        private void cameraToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (cameraControl == null || cameraControl.IsDisposed)
                cameraControl = modelViewport.GetCameraControl();
            cameraControl.Focus();
            cameraControl.Show();
        }
        private void printLightValuesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var folderPath = FileTools.TryOpenFolder("Select Source Directory");
            if (string.IsNullOrEmpty(folderPath))
                return;

            var valuesByName = new Dictionary<string, HashSet<string>>();

            foreach (var file in Directory.EnumerateFiles(folderPath, "*nuanmb", SearchOption.AllDirectories))
            {
                if (!file.Contains("render") || !file.Contains("light"))
                    continue;

                var node = new NUANIM_Node(file);
                node.Open();

                node.UpdateUniqueLightValues(valuesByName);
            }

            foreach (var pair in valuesByName)
            {
                var output = new System.Text.StringBuilder();
                foreach (var value in pair.Value)
                {
                    output.AppendLine(value);
                }

                File.WriteAllText($"{pair.Key} unique values.txt", output.ToString());
            }
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            modelViewport.Close();
        }
    }
}
