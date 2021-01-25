using System.Collections.Generic;
using System.IO;
using System;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace CrossMod.Nodes
{
    /// <summary>
    /// Base class for all nodes that represent a file system entry.
    /// Contains the string absolute path and an overridable Open() method.
    /// </summary>
    public class FileNode : INotifyPropertyChanged
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

        public FileNode? Parent { get; private set; }

        public string ImageKey { get; }

        public event EventHandler<bool>? Expanded;

        public event PropertyChangedEventHandler? PropertyChanged;

        public bool IsExpanded
        { 
            get => isExpanded;
            set
            {
                isExpanded = value;
                Expanded?.Invoke(this, isExpanded);
            }
        }
        private bool isExpanded;

        public ObservableCollection<FileNode> Nodes { get; set; } = new ObservableCollection<FileNode>();

        public FileNode(string path, string imageKey, bool isActive)
        {
            AbsolutePath = path;
            IsActive = isActive;
            ImageKey = imageKey;
            Text = Path.GetFileName(path);
        }

        // TODO: Ensure nodes can only be added using this method to avoid messing up parent/child relationships.
        public void AddNode(FileNode node)
        {
            node.Parent = this;
            Nodes.Add(node);
        }
    }
}
