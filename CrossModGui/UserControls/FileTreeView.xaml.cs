using CrossMod.Nodes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
