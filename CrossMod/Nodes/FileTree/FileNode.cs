using System.Windows.Forms;

namespace CrossMod.Nodes
{
    /// <summary>
    /// Base class for all TreeView nodes that represent a file system entry.
    /// Contains the string absolute path and an overridable Open() method.
    /// </summary>
    public class FileNode : TreeNode
    {
        /// <summary>
        /// Path to the file system entry that this FileNode represents
        /// </summary>
        public string AbsolutePath { get; protected set; }

        public FileNode(string path)
        {
            AbsolutePath = path;
        }

        public virtual void Open()
        {

        }
    }
}
