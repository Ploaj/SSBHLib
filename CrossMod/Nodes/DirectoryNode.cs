using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrossMod.Nodes
{
    /// <summary>
    /// Class for all Directory entries in the file system. Executing Open()
    /// populates sub-nodes, and executing OpenNodes() calls Open() on all sub-nodes.
    /// </summary>
    class DirectoryNode : FileNode
    {
        private bool isOpened = false;
        private bool isNestedOpened = false;

        private static readonly List<Type> fileNodeTypes = (from domainAssembly in AppDomain.CurrentDomain.GetAssemblies()
                                                            from assemblyType in domainAssembly.GetTypes()
                                                            where typeof(FileNode).IsAssignableFrom(assemblyType)
                                                            select assemblyType).ToList();

        /// <summary>
        /// Creates a new DirectoryNode. The FilePath is set to the given value
        /// </summary>
        /// <param name="path"></param>
        /// <param name="isRoot">Whether this is the topmost parent. Decides whether to display full or partial name.</param>
        public DirectoryNode(string path, bool isRoot=true) : base(path)
        {
            Text = (isRoot) ? Path.GetFullPath(path) : Path.GetFileName(path);
            SelectedImageKey = "folder";
            ImageKey = "folder";
        }

        /// <summary>
        /// Reads the directory, populating all subnodes.
        /// Subnodes are not opened, use OpenNodes() afterwards to do that.
        /// Repeated executions are no-ops.
        /// </summary>
        public override void Open()
        {
            if (isOpened)
            {
                return;
            }

            foreach (var name in Directory.EnumerateFileSystemEntries(AbsolutePath))
            {
                if (Directory.Exists(name))
                {
                    var dirNode = new DirectoryNode(name, isRoot:false);
                    this.Nodes.Add(dirNode);
                }
                else
                {
                    this.Nodes.Add(CreateFileNode(fileNodeTypes, name));
                }
            }

            isOpened = true;
        }

        /// <summary>
        /// Opens all nodes. Make sure to call after Open().
        /// Repeated executions result in a no-op.
        /// </summary>
        public void OpenNodes()
        {
            if (isNestedOpened)
            {
                return;
            }

            foreach (var node in this.Nodes)
            {
                (node as FileNode)?.Open();
            }

            isNestedOpened = true;
        }

        /// <summary>
        /// Internal helper to open a file entry.
        /// </summary>
        /// <param name="Types"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        private FileNode CreateFileNode(IEnumerable<Type> Types, string file)
        {
            // TODO: Possible separation of concerns improvement: IFileLoader injected into DirectoryNode.

            FileNode fileNode = null;

            string Extension = Path.GetExtension(file);

            foreach (var type in Types)
            {
                if (type.GetCustomAttributes(typeof(FileTypeAttribute), true).FirstOrDefault() is FileTypeAttribute attr)
                {
                    if (attr.Extension.Equals(Extension))
                    {
                        fileNode = (FileNode)Activator.CreateInstance(type, file);
                    }
                }
            }

            if (fileNode == null)
                fileNode = new FileNode(file);

            // Change style of unrenderable nodes
            if (!(fileNode is IRenderableNode))
            {
                fileNode.ForeColor = Color.Gray;
            }
            
            fileNode.Text = Path.GetFileName(file);
            return fileNode;
        }
    }
}
