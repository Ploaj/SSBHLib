using CrossMod.GUI;
using CrossMod.Nodes;
using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace CrossMod
{
    public partial class MainForm : Form
    {
        // Controls
        private ModelViewport _modelControl;

        public MainForm()
        {
            InitializeComponent();

            _modelControl = new ModelViewport();
            _modelControl.Dock = DockStyle.Fill;
        }

        public void ShowModelControl()
        {
            contentBox.Controls.Clear();
            _modelControl.Clear();
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
                    _modelControl.RenderableNode = ((IRenderableNode)Node).GetRenderableNode();
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
    }
}
