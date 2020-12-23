using CrossMod.Nodes.Formats.Models;
using System.IO;

namespace CrossMod.Nodes
{
    /// <summary>
    /// Class for all Directory entries in the file system. Executing Open()
    /// populates sub-nodes, and executing OpenNodes() calls Open() on all sub-nodes.
    /// </summary>
    public class DirectoryNode : FileNode
    {
        private bool hasOpenedFiles = false;

        /// <summary>
        /// Creates a new DirectoryNode. The FilePath is set to the given value
        /// </summary>
        /// <param name="path"></param>
        /// <param name="isRoot">Whether this is the topmost parent. Decides whether to display full or partial name.</param>
        public DirectoryNode(string path, bool isRoot = true) : base(path, "folder", true)
        {
            Text = Path.GetFileName(path);

            CreateAndAddChildren();
            Expanded += (s, e) => OnExpand();
        }

        private void CreateAndAddChildren()
        {
            foreach (var name in Directory.EnumerateFileSystemEntries(AbsolutePath))
            {
                if (Directory.Exists(name))
                {
                    var dirNode = new DirectoryNode(name, isRoot: false);
                    AddNode(dirNode);
                }
                else
                {
                    var fileNode = CreateFileNode(name);
                    if (fileNode != null)
                        AddNode(fileNode);
                }
            }
        }

        private void OnExpand()
        {
            // Nodes only need to be opened once.
            if (hasOpenedFiles)
                return;

            // TODO: Load children?

            hasOpenedFiles = true;
        }

        private FileNode? CreateFileNode(string filePath)
        {
            return Path.GetExtension(filePath) switch
            {
                ".nutexb" => new NutexbNode(filePath),
                ".numshb" => new NumshbNode(filePath),
                ".numatb" => new NumatbNode(filePath),
                ".numdlb" => new NumdlbNode(filePath),
                ".nusktb" => new NusktbNode(filePath),
                ".script" => new ScriptNode(filePath),
                ".nuanmb" => new NuanmbNode(filePath),
                ".xmb" => new XmbNode(filePath),
                ".prc" => new ParamNode(filePath),
                ".nuhlp" => new NuhlpbNode(filePath),
                _ => null
            };
        }
    }
}
