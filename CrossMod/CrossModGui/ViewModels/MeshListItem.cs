using System;
using System.Collections.ObjectModel;

namespace CrossModGui.ViewModels
{
    public partial class MainWindowViewModel
    {
        public class MeshListItem : ViewModelBase
        {
            public string Name { get; set; } = "";

            public bool IsExpanded
            {
                get => isExpanded;
                set
                {
                    isExpanded = value;
                    OnPropertyChanged();
                }
            }
            private bool isExpanded;

            public bool IsChecked 
            { 
                get => isChecked;
                set
                {
                    isChecked = value;

                    foreach (var child in Children)
                    {
                        child.IsChecked = value;
                    }

                    OnPropertyChanged();
                }
            }
            private bool isChecked;

            public ObservableCollection<MeshListItem> Children { get; } = new ObservableCollection<MeshListItem>();
        }
    }
}
