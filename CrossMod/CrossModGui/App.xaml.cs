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
        async void App_Startup(object sender, StartupEventArgs e)
        {
            // Only check for updates once a day to avoid API rate limits.
            // Multiple releases a day would be a pretty extreme release schedule.
            Octokit.Release? latestRelease = null;
            if (PreferencesWindowViewModel.Instance.LastUpdateCheckTime.Date < DateTime.Today)
            {
                latestRelease = await Tools.Updater.TryFindNewerReleaseAsync(PreferencesWindowViewModel.Instance.ReleaseTag);
                PreferencesWindowViewModel.Instance.LastUpdateCheckTime = DateTime.Now;
            }

            // The behavior for PropertyChanged after .NET 4.5 doesn't work with separator chars for floats/doubles.
            FrameworkCompatibilityPreferences.KeepTextBoxDisplaySynchronizedWithTextProperty = false;

            // Force initialization and a theme refresh.
            PreferencesWindowViewModel.Instance.UpdateTheme();


            MainWindow = new MainWindow();
            MainWindow.Show();

            // TODO: Display the release info and download link.
            //if (latestRelease != null)
            {
                var updateWindow = new NewReleaseWindow();
                updateWindow.ShowDialog();
            }
        }
    }
}
