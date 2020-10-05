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

        public event EventHandler<RoutedPropertyChangedEventArgs<object>> SelectedItemChanged;

        public ObservableCollection<FileNode> Items
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
    }
}
