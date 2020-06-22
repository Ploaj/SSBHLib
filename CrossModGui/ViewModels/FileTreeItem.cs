using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace CrossModGui.ViewModels
{
    public class FileTreeItem
    {
        public string Text { get; set; }

        public string IconPath { get; set; }

        public bool IsExpanded { get; set; }

        public List<FileTreeItem> Children { get; set; } = new List<FileTreeItem>();
    }
}
