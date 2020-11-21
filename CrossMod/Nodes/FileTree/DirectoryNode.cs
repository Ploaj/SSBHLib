using CrossMod.Nodes.Formats.Models;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace CrossMod.Nodes
{
    /// <summary>
    /// Class for all Directory entries in the file system. Executing Open()
    /// populates sub-nodes, and executing OpenNodes() calls Open() on all sub-nodes.
    /// </summary>
    public class DirectoryNode : FileNode
    {
        private bool isOpened = false;
        private bool hasOpenedFiles = false;

        /// <summary>
        /// Creates a new DirectoryNode. The FilePath is set to the given value
        /// </summary>
        /// <param name="path"></param>
        /// <param name="isRoot">Whether this is the topmost parent. Decides whether to display full or partial name.</param>
        public DirectoryNode(string path, bool isRoot = true) : base(path)
        {
            Text = isRoot ? Path.GetFullPath(path) : Path.GetFileName(path);
            ImageKey = "folder";

            // Make the font color use the default foreground color.
            // TODO: "IsActive" should be reworked at some point (it only applies to renderables).
            IsActive = true;
        }

        /// <summary>
        /// Reads the directory, populating all subnodes.
        /// Subnodes are not opened, use <see cref="OpenChildNodes"/> afterwards to do that.
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
                    var dirNode = new DirectoryNode(name, isRoot: false);
                    AddNode(dirNode);
                    dirNode.Open();
                }
                else
                {
                    var fileNode = CreateFileNode(name);
                    if (fileNode != null)
                        AddNode(fileNode);
                }
            }

            isOpened = true;
        }

        /// <summary>
        /// Opens all files in this directory.
        /// Repeated calls are ignored.
        /// </summary>
        public void OpenFileNodes()
        {
            if (hasOpenedFiles)
                return;

            // Some nodes take a while to open, so use a threadpool to save time.
            var openNodes = new List<Task>();
            foreach (var node in Nodes)
            {
                if (node is FileNode file)
                {
                    openNodes.Add(Task.Run(() => file.Open()));
                }
            }

            Task.WaitAll(openNodes.ToArray());
            hasOpenedFiles = true;
        }

        private FileNode? CreateFileNode(string filePath)
        {
            return Path.GetExtension(filePath) switch
            {
                ".nutexb" => new NutexNode(filePath),
                ".numshb" => new NumsbhNode(filePath),
                ".numatb" => new MatlNode(filePath),
                ".numdlb" => new NumdlNode(filePath),
                ".nusktb" => new SkelNode(filePath),
                ".script" => new ScriptNode(filePath),
                ".nuanmb" => new NuanimNode(filePath),
                ".xmb" => new XmbNode(filePath),
                ".prc" => new ParamNode(filePath),
                ".nuhlp" => new NuhlpbNode(filePath),
                _ => null
            };
        }
    }
}
