using CrossMod.GUI;
using CrossMod.IO;
using CrossMod.Nodes;
using CrossMod.Rendering;
using CrossMod.Tools;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace CrossMod
{
    public partial class MainForm : Form
    {
        public static ImageList iconList;

        // Controls
        private readonly ModelViewport modelViewport = new ModelViewport
        {
            Dock = DockStyle.Fill
        };

        private readonly ContextMenu fileTreeContextMenu = new ContextMenu();

        private CameraControl cameraControl;

        public MainForm()
        {
            InitializeComponent();

            ShowModelViewport();

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

        public void ShowModelViewport()
        {
            contentBox.Controls.Clear();
            contentBox.Controls.Add(modelViewport);
        }

        private void openFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!FileTools.TryOpenFolderDialog(out string folderPath, "Select Source Directory"))
                return;

            WorkSpaceTools.LoadWorkspace(fileTree, modelViewport, folderPath);
        }

        private void fileTree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            // Reset collision color before rendering
            foreach (Collision coll in ParamNodeContainer.HitData)
                coll.Color = Collision.DefaultColor;

            if (fileTree.SelectedNode.Text.EndsWith("nuanmb") && modelViewport.ScriptNode != null)
            {
                modelViewport.ScriptNode.CurrentAnimationName = fileTree.SelectedNode.Text;
            }

            if (fileTree.SelectedNode is IRenderableNode renderableNode)
            {
                // TODO: This should be done automatically.
                modelViewport.Renderer.PauseRendering();

                modelViewport.Renderer.ItemToRender = renderableNode.GetRenderableNode();
                modelViewport.UpdateGui(renderableNode.GetRenderableNode());

                modelViewport.Renderer.RestartRendering();
            }
            else if (fileTree.SelectedNode is NuanimNode animation)
            {
                modelViewport.RenderableAnimation = (IRenderableAnimation)animation.GetRenderableNode();
            }
        }

        private void reloadShadersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Force the shaders to be generated again.
            modelViewport.Renderer.ReloadShaders();
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
                        MenuItem exportPng = new MenuItem("Export As");
                        exportPng.Click += ExportExportableTexture;
                        exportPng.Tag = exportableTextureNode;
                        fileTreeContextMenu.MenuItems.Add(exportPng);
                    }
                    if (node is IExportableModelNode exportableNode)
                    {
                        MenuItem exportSmd = new MenuItem("Export As");
                        exportSmd.Click += ExportExportableModelAsSmd;
                        exportSmd.Tag = exportableNode;
                        fileTreeContextMenu.MenuItems.Add(exportSmd);
                    }
                    if (node is IExportableAnimationNode exportableAnimNode)
                    {
                        MenuItem exportAnim = new MenuItem("Export Anim");
                        exportAnim.Click += ExportExportableAnimation;
                        exportAnim.Tag = exportableAnimNode;
                        fileTreeContextMenu.MenuItems.Add(exportAnim);
                    }

                    // show if it has at least 1 option
                    if (fileTreeContextMenu.MenuItems.Count != 0)
                        fileTreeContextMenu.Show(fileTree, p);
                }
            }
        }

        private void ExportExportableTexture(object sender, EventArgs args)
        {
            modelViewport.Renderer.SwitchContextToCurrentThreadAndPerformAction(() =>
            {
                if (FileTools.TryOpenSaveFileDialog(out string fileName, "Portable Networks Graphic(*.png)|*.png", ((MenuItem)sender).Tag.ToString()))
                {
                    // need to get RSkeleton First for some types
                    if (fileName.EndsWith(".png"))
                    {
                        ((IExportableTextureNode)((MenuItem)sender).Tag).SaveTexturePNG(fileName);
                    }
                }
            });
        }

        private void ExportExportableAnimation(object sender, EventArgs args)
        {
            if (FileTools.TryOpenSaveFileDialog(out string fileName, "Supported Files(*.smd, *.seanim, *.anim)|*.smd;*.seanim;*.anim"))
            {
                // need to get RSkeleton First for some types
                if (fileName.EndsWith(".smd") || fileName.EndsWith(".anim"))
                {
                    RSkeleton skeletonNode = null;
                    if (FileTools.TryOpenFileDialog(out string skeletonFileName, "SKEL (*.nusktb)|*.nusktb"))
                    {
                        if (skeletonFileName != null)
                        {
                            SkelNode node = new SkelNode(skeletonFileName);
                            node.Open();
                            skeletonNode = (RSkeleton)node.GetRenderableNode();
                        }
                    }
                    if (skeletonNode == null)
                    {
                        MessageBox.Show("No Skeleton File Selected");
                        return;
                    }

                    if (fileName.EndsWith(".anim"))
                    {
                        bool ordinal = false;
                        DialogResult dialogResult = MessageBox.Show("In most cases choose \"No\"", "Use ordinal bone order?", MessageBoxButtons.YesNo);
                        if (dialogResult == DialogResult.Yes)
                            ordinal = true;
                        IO_MayaANIM.ExportIOAnimationAsANIM(fileName, ((IExportableAnimationNode)((MenuItem)sender).Tag).GetIOAnimation(), skeletonNode, ordinal);
                    }

                    if (fileName.EndsWith(".smd"))
                        IO_SMD.ExportIOAnimationAsSMD(fileName, ((IExportableAnimationNode)((MenuItem)sender).Tag).GetIOAnimation(), skeletonNode);
                }

                // other types like SEAnim go here
                if (fileName.EndsWith(".seanim"))
                {
                    IO_SEANIM.ExportIOAnimation(fileName, ((IExportableAnimationNode)((MenuItem)sender).Tag).GetIOAnimation());
                }
            }
        }

        private void ExportExportableModelAsSmd(object sender, EventArgs args)
        {
            if (FileTools.TryOpenSaveFileDialog(out string fileName, "Supported Files(*.smd*.obj*.dae*.ply)|*.smd;*.obj;*.dae;*.ply"))
            {
                if (fileName.EndsWith(".smd"))
                    IO_SMD.ExportIOModelAsSMD(fileName, ((IExportableModelNode)((MenuItem)sender).Tag).GetIOModel());
                if (fileName.EndsWith(".obj"))
                    IO_OBJ.ExportIOModelAsOBJ(fileName, ((IExportableModelNode)((MenuItem)sender).Tag).GetIOModel());
                if (fileName.EndsWith(".dae"))
                {
                    bool optimize = false;
                    bool materials = false;
                    DialogResult dialogResult = MessageBox.Show("Smaller filesize but takes longer to export", "Optimize Geometry?", MessageBoxButtons.YesNo);
                    if (dialogResult == DialogResult.Yes)
                    {
                        optimize = true;
                    }
                    dialogResult = MessageBox.Show("Export texture names inside of dae?", "Export Materials?", MessageBoxButtons.YesNo);
                    if (dialogResult == DialogResult.Yes)
                    {
                        materials = true;
                    }
                    IO_DAE.ExportIOModelAsDAE(fileName, ((IExportableModelNode)((MenuItem)sender).Tag).GetIOModel(), optimize, materials);
                }
                if (fileName.EndsWith(".ply"))
                    IO_PLY.ExportIOModelAsPLY(fileName, ((IExportableModelNode)((MenuItem)sender).Tag).GetIOModel());
            }
        }

        private void clearWorkspaceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            WorkSpaceTools.ClearWorkspace(fileTree, modelViewport);
        }

        private void batchRenderModelsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BatchRendering.RenderModels(modelViewport.Renderer);
        }

        private void frameSelectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            modelViewport.Renderer.FrameRenderableModels();
        }

        private void fileTree_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            if (e.Node is DirectoryNode node)
            {
                node.OpenFileNodes();
            }
        }

        private void cameraToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (cameraControl == null || cameraControl.IsDisposed)
                cameraControl = modelViewport.GetCameraControl();
            cameraControl.Focus();
            cameraControl.Show();
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            modelViewport.Close();
        }

        private void clearViewportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            modelViewport.ClearFiles();
        }

        private async void ExportAnimationToGIFToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            await AnimationExport.ConvertAnimationToGif(modelViewport);
        }

        private void ExportAnimationToFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AnimationExport.ExportFramesToFolder(modelViewport);
        }

        private void ExportCurrentFrameToBitmapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (FileTools.TryOpenSaveFileDialog(out string fileName, "PNG|*.png", "CurrentFrame.png"))
                modelViewport.SaveScreenshot(fileName);
            
        }

        private void hitboxHurtboxSelectorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorSelector colorSelector = new ColorSelector();
            colorSelector.ShowDialog();
        }

        private void reloadScriptsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (modelViewport.ScriptNode != null)
            {
                modelViewport.ScriptNode.ReadScriptFile();
                modelViewport.ResetAnimation();
                modelViewport.ScriptNode.Start();
            }
        }
    }
}
