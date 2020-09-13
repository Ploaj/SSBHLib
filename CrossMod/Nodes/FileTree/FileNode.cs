using System.Windows.Forms;

namespace CrossMod.Nodes
{
    /// <summary>
    /// Base class for all nodes that represent a file system entry.
    /// Contains the string absolute path and an overridable Open() method.
    /// </summary>
    // TODO: Add the necessary properties instead of inheriting from treenode.
    public class FileNode : TreeNode
    {
        /// <summary>
        /// Path to the file system entry that this <see cref="FileNode"/> represents
        /// </summary>
        public string AbsolutePath { get; }

        /// <summary>
        /// <c>true</c> if the node can be displayed or edited
        /// </summary>
        public bool IsActive { get; set; }

        public FileNode(string path)
        {
            AbsolutePath = path;
        }

        public virtual void Open()
        {

        }
    }
}
