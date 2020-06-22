using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrossModGui.ViewModels
{
    public class MainWindowViewModel
    {
        public ObservableCollection<CrossMod.Nodes.FileNode> FileTreeItems { get; } = new ObservableCollection<CrossMod.Nodes.FileNode>();
    }
}
