using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SFGraphics;
using System.IO;
using CrossMod.Nodes;
using CrossMod.GUI;

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
    }
}
