using CrossMod.GUI;
using CrossMod.Nodes;
using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using CrossMod.IO;

namespace CrossMod
{
    public partial class MainForm : Form
    {
        // Controls
        private ModelViewport modelViewport;

        private ContextMenu fileTreeContextMenu;

        public MainForm()
        {
            InitializeComponent();

            modelViewport = new ModelViewport
            {
                Dock = DockStyle.Fill
            };

            fileTreeContextMenu = new ContextMenu();
        }

        public void HideControl()
        {
            contentBox.Controls.Clear();
            modelViewport.Clear();
        }

        public void ShowModelViewport()
        {
            HideControl();
            contentBox.Controls.Add(modelViewport);
        }

        private void openModelFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string folderPath = FileTools.TryOpenFolder();
            if (string.IsNullOrEmpty(folderPath))
                return;

            OpenFiles(folderPath);

            ShowModelViewport();
        }

        private void OpenFiles(string folderPath)
        {
            string[] files = Directory.GetFiles(folderPath);

            var Types = (from domainAssembly in AppDomain.CurrentDomain.GetAssemblies()
                         from assemblyType in domainAssembly.GetTypes()
                         where typeof(FileNode).IsAssignableFrom(assemblyType)
                         select assemblyType).ToArray();

            TreeNode Parent = new TreeNode(Path.GetDirectoryName(folderPath));
            fileTree.Nodes.Add(Parent);

            foreach (string file in files)
            {
                FileNode Node = null;

                string Extension = Path.GetExtension(file);

                foreach (Type type in Types)
                {
                    if (type.GetCustomAttributes(typeof(FileTypeAttribute), true).FirstOrDefault() is FileTypeAttribute attr)
                    {
                        if (attr.Extension.Equals(Extension))
                        {
                            Node = (FileNode)Activator.CreateInstance(type);
                        }
                    }
                }

                if (Node == null)
                    Node = new FileNode();

                Node.Open(file);

                Node.Text = Path.GetFileName(file);
                Parent.Nodes.Add(Node);
            }
        }

        private void fileTree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (fileTree.SelectedNode is IRenderableNode renderableNode)
            {
                if (renderableNode != null)
                {
                    ShowModelViewport();
                    modelViewport.RenderableNode = renderableNode;
                }
            }

            modelViewport.RenderFrame();
        }

        private void reloadShadersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Force the shader to be generated again.
            Rendering.RModel.shader = null;
            Rendering.RModel.textureDebugShader = null;
            Rendering.RTexture.textureShader = null;
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
                    if (node is IExportableModelNode exportableNode)
                    {
                        MenuItem ExportSMD = new MenuItem("Export As");
                        ExportSMD.Click += exportExportableModelAsSMD;
                        ExportSMD.Tag = exportableNode;
                        fileTreeContextMenu.MenuItems.Add(ExportSMD);
                    }

                    // show if it has at least 1 option
                    if (fileTreeContextMenu.MenuItems.Count != 0)
                        fileTreeContextMenu.Show(fileTree, p);
                }
            }
        }

        private void exportExportableModelAsSMD(object sender, EventArgs args)
        {
            if (FileTools.TrySaveFile(out string fileName, "Supported Files(*.smd*.obj)|*.smd;*.obj"))
            {
                if (fileName.EndsWith(".smd"))
                    IO_SMD.ExportIOModelAsSMD(fileName, ((IExportableModelNode)((MenuItem)sender).Tag).GetIOModel());
                if (fileName.EndsWith(".obj"))
                    IO_OBJ.ExportIOModelAsOBJ(fileName, ((IExportableModelNode)((MenuItem)sender).Tag).GetIOModel());
            }
        }

        private void clearWorkspaceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClearWorkspace();
        }

        private void ClearWorkspace()
        {
            fileTree.Nodes.Clear();
            modelViewport.ClearFiles();
            HideControl();
            GC.Collect();
        }

        private void batchRenderModelsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string folderPath = FileTools.TryOpenFolder();

            // Just render the first alt costume, which should include items without slot specific variants.
            foreach (var file in Directory.EnumerateFiles(folderPath, "*model.numdlb", SearchOption.AllDirectories))
            {
                string folder = Directory.GetParent(file).FullName;
                OpenFiles(folder);

                ShowModelViewport();

                // Necessary workaround for how models are displayed.
                SelectFirstModelNode();

                modelViewport.RenderFrame();

                // TODO: Save screenshot.

                // Cleanup.
                ClearWorkspace();
                System.Diagnostics.Debug.WriteLine(folder);
            }
        }

        private void SelectFirstModelNode()
        {
            foreach (TreeNode folderNode in fileTree.Nodes)
            {
                foreach (TreeNode fileNode in folderNode.Nodes)
                {
                    if (fileNode.Text.EndsWith("numdlb"))
                    {
                        fileTree.SelectedNode = fileNode;
                        return;
                    }
                }
            }
        }
    }
}
