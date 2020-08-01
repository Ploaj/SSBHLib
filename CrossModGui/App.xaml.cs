using CrossModGui.Views;
using System;
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

        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show($"An unhandled exception just occurred. The application will now close.\n\n{e.Exception.Message}\n\n{e.Exception.StackTrace}",
                "Unhandled Exception", MessageBoxButton.OK, MessageBoxImage.Error);
            e.Handled = true;

            // TODO: Log this error.

            // Ensure all threads exit.
            Environment.Exit(0);
        }
    }
}
