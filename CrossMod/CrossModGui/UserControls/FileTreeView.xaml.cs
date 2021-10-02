using CrossMod.Nodes;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace CrossModGui.UserControls
{
    /// <summary>
    /// Interaction logic for FileTreeView.xaml
    /// </summary>
    public partial class FileTreeView : UserControl
    {
        public static readonly DependencyProperty ItemsProperty = DependencyProperty.Register(nameof(Items),
            typeof(ObservableCollection<FileNode>), typeof(FileTreeView),
            new FrameworkPropertyMetadata(null));

        public event EventHandler<RoutedPropertyChangedEventArgs<object>>? SelectedItemChanged;

        public ObservableCollection<FileNode>? Items
        {
            get => GetValue(ItemsProperty) as ObservableCollection<FileNode>;
            set => SetValue(ItemsProperty, value);
        }

        public FileTreeView()
        {
            InitializeComponent();
        }

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            SelectedItemChanged?.Invoke(this, new RoutedPropertyChangedEventArgs<object>(e.OldValue, e.NewValue));
        }

        private void TreeView_Selected(object sender, RoutedEventArgs e)
        {
            // Make the items easier to expand:
            // https://stackoverflow.com/questions/2921972/how-to-expand-wpf-treeview-on-single-click-of-item
            var item = e.OriginalSource as TreeViewItem;

            if (item == null || e.Handled) 
                return;

            item.IsExpanded = !item.IsExpanded;
            item.IsSelected = false;
            e.Handled = true;
        }
    }
}
