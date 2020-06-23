using CrossModGui.ViewModels;
using System.Windows;

namespace CrossModGui
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        void App_Startup(object sender, StartupEventArgs e)
        {
            MainWindow = new MainWindow(new MainWindowViewModel());
            MainWindow.Show();
        }
    }
}
