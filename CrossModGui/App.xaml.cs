using CrossModGui.Views;
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
            // The behavior for PropertyChanged after .NET 4.5 doesn't work with separator chars for floats/doubles.
            FrameworkCompatibilityPreferences.KeepTextBoxDisplaySynchronizedWithTextProperty = false;

            MainWindow = new MainWindow();
            MainWindow.Show();
        }
    }
}
