using System.Collections.Generic;

namespace CrossMod.Nodes
{
    /// <summary>
    /// Base class for all nodes that represent a file system entry.
    /// Contains the string absolute path and an overridable Open() method.
    /// </summary>
    public class FileNode
    {
        /// <summary>
        /// Path to the file system entry that this <see cref="FileNode"/> represents
        /// </summary>
        public string AbsolutePath { get; }

        public string Text { get; set; }

        /// <summary>
        /// <c>true</c> if the node can be displayed or edited
        /// </summary>
        public bool IsActive { get; set; }

        public FileNode Parent { get; private set; }

        public string ImageKey { get; set; }

        public bool IsExpanded { get; set; }

        public List<FileNode> Nodes { get; set; } = new List<FileNode>();

        public FileNode(string path)
        {
            AbsolutePath = path;
        }

        // TODO: Ensure nodes can only be added using this method to avoid messing up parent/child relationships.
        public void AddNode(FileNode node)
        {
            node.Parent = this;
            Nodes.Add(node);
        }

        public virtual void Open()
        {

        }
    }
}
