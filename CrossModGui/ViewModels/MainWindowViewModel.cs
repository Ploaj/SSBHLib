using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace CrossModGui.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
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

        public ObservableCollection<CrossMod.Nodes.FileNode> FileTreeItems { get; } = new ObservableCollection<CrossMod.Nodes.FileNode>();

        public ObservableCollection<BoneTreeItem> BoneTreeItems { get; } = new ObservableCollection<BoneTreeItem>();

        public ObservableCollection<MeshListItem> MeshListItems { get; } = new ObservableCollection<MeshListItem>();

        public event PropertyChangedEventHandler PropertyChanged;

        public void Clear()
        {
            FileTreeItems.Clear();
            BoneTreeItems.Clear();
            MeshListItems.Clear();
        }
    }
}
