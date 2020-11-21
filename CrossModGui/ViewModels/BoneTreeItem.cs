using System.Collections.Generic;

namespace CrossModGui.ViewModels
{
    public partial class MainWindowViewModel
    {
        public class BoneTreeItem : ViewModelBase
        {
            public string Name { get; set; } = "";

            public List<BoneTreeItem> Children { get; set; } = new List<BoneTreeItem>();
        }
    }
}
