using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace CrossModGui.ViewModels
{
    public class MainWindowViewModel
    {
        public class BoneTreeItem
        {
            public string Name { get; set; }

            public List<BoneTreeItem> Children { get; set;  } = new List<BoneTreeItem>();
        }

        public class MeshListItem
        {
            public string Name { get; set; }

            public bool IsChecked { get; set; }
        }

        public CrossMod.Nodes.FileNode SelectedFileNode { get; set; }

        public ObservableCollection<CrossMod.Nodes.FileNode> FileTreeItems { get; } = new ObservableCollection<CrossMod.Nodes.FileNode>();

        public ObservableCollection<BoneTreeItem> BoneTreeItems { get; } = new ObservableCollection<BoneTreeItem>();

        public ObservableCollection<MeshListItem> MeshListItems { get; } = new ObservableCollection<MeshListItem>();
    }
}
