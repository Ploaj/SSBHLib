using CrossModGui.ViewModels;
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

            var darkTheme = new ResourceDictionary
            {
                Source = new Uri("/CrossModGUI;component/Resources/DarkTheme.xaml", UriKind.Relative)
            };

            if (PreferencesWindowViewModel.Instance.EnableDarkTheme)
                Current.Resources.MergedDictionaries.Add(darkTheme);

            MainWindow = new MainWindow();
            MainWindow.Show();
        }

        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            // TODO: Log errors.
            e.Handled = true;
        }
    }
}
