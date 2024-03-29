﻿using CrossModGui.ViewModels;
using CrossModGui.Views;
using System;
using System.Net;
using System.Threading.Tasks;
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

            if (latestRelease != null)
            {
                await ShowNewReleaseWindowAsync(latestRelease);
            }
        }

        private static async Task ShowNewReleaseWindowAsync(Octokit.Release latestRelease)
        {
            try
            {
                using (var client = new WebClient())
                {
                    var changeLogText = await client.DownloadStringTaskAsync("https://raw.githubusercontent.com/Ploaj/SSBHLib/master/Changelog.md");

                    var vm = new NewReleaseWindowViewModel(PreferencesWindowViewModel.Instance.ReleaseTag, latestRelease.TagName, changeLogText, latestRelease.HtmlUrl);
                    var updateWindow = new NewReleaseWindow(vm);
                    updateWindow.ShowDialog();
                }
            }
            catch (Exception)
            {
                // TODO: Add some form of logging?
            }
        }
    }
}
