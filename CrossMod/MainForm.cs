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
        private ModelViewport _modelControl;

        private ContextMenu fileTreeContextMenu;

        public MainForm()
        {
            InitializeComponent();

            _modelControl = new ModelViewport();
            _modelControl.Dock = DockStyle.Fill;

            fileTreeContextMenu = new ContextMenu();
        }

        public void HideControl()
        {
            contentBox.Controls.Clear();
            _modelControl.Clear();
        }

        public void ShowModelControl()
        {
            HideControl();
            contentBox.Controls.Add(_modelControl);
        }

        private void openModelFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string Folder = FileTools.TryOpenFolder();
            if (Folder.Equals("")) return;

            string[] Files = Directory.GetFiles(Folder);
            
            var Types = (from domainAssembly in AppDomain.CurrentDomain.GetAssemblies()
                         from assemblyType in domainAssembly.GetTypes()
                         where typeof(FileNode).IsAssignableFrom(assemblyType)
                         select assemblyType).ToArray();

            TreeNode Parent = new TreeNode(Path.GetDirectoryName(Folder));
            fileTree.Nodes.Add(Parent);

            foreach (string s in Files)
            {
                FileNode Node = null;

                string Extension = Path.GetExtension(s);

                foreach (Type c in Types)
                {
                    var attr = c.GetCustomAttributes(
                    typeof(FileTypeAttribute), true
                    ).FirstOrDefault() as FileTypeAttribute;
                    if (attr != null)
                    {
                        if (attr.Extension.Equals(Extension))
                        {
                            Node = (FileNode)Activator.CreateInstance(c);
                        }
                    }
                }

                if(Node == null)
                    Node = new FileNode();

                Node.Open(s);

                Node.Text = Path.GetFileName(s);
                Parent.Nodes.Add(Node);
            }
        }

        private void fileTree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            TreeNode Node = fileTree.SelectedNode;

            if(Node != null)
            {
                if(Node is IRenderableNode)
                {
                    ShowModelControl();
                    _modelControl.RenderableNode = (IRenderableNode)Node;
                }
            }

            _modelControl.RenderFrame();
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
                    if(node is IExportableModelNode exportableNode)
                    {
                        MenuItem ExportSMD = new MenuItem("Export As");
                        ExportSMD.Click += exportExportableModelAsSMD;
                        ExportSMD.Tag = exportableNode;
                        fileTreeContextMenu.MenuItems.Add(ExportSMD);
                    }

                    // show if it has at least 1 option
                    if(fileTreeContextMenu.MenuItems.Count != 0)
                        fileTreeContextMenu.Show(fileTree, p);
                }
            }
        }

        private void exportExportableModelAsSMD(object sender, EventArgs args)
        {
            string FileName;
            if (FileTools.TrySaveFile(out FileName, "Supported Files(*.smd*.obj)|*.smd;*.obj"))
            {
                if(FileName.EndsWith(".smd"))
                IO_SMD.ExportIOModelAsSMD(FileName, ((IExportableModelNode)((MenuItem)sender).Tag).GetIOModel());
                if (FileName.EndsWith(".obj"))
                    IO_OBJ.ExportIOModelAsOBJ(FileName, ((IExportableModelNode)((MenuItem)sender).Tag).GetIOModel());
            }
        }

        private void clearWorkspaceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fileTree.Nodes.Clear();
            _modelControl.ClearFiles();
            HideControl();
            GC.Collect();
        }
    }
}
