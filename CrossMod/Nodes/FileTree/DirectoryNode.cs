using CrossMod.Nodes.Formats.Models;
using System.IO;

namespace CrossMod.Nodes
{
    /// <summary>
    /// Class for all Directory entries in the file system
    /// </summary>
    public class DirectoryNode : FileNode
    {
        private bool hasOpenedFiles = false;

        /// <summary>
        /// Creates a new DirectoryNode. The FilePath is set to the given value
        /// </summary>
        /// <param name="path"></param>
        public DirectoryNode(string path) : base(path, "folder", true)
        {
            Text = Path.GetFileName(path);

            // Add the children to enable expanding in the GUI.
            CreateAndAddChildren();
            hasOpenedFiles = true;

            Expanded += (s, e) => OnExpand();
        }

        private void CreateAndAddChildren()
        {
            foreach (var name in Directory.EnumerateFileSystemEntries(AbsolutePath))
            {
                if (Directory.Exists(name))
                {
                    var dirNode = new DirectoryNode(name);
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

            foreach (var child in Nodes)
            {
                if (child is DirectoryNode directory)
                    directory.CreateAndAddChildren();
            }    

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
