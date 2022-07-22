using Newtonsoft.Json;
using System;
using System.Windows;

namespace CrossModGui.ViewModels
{
    public class PreferencesWindowViewModel : ViewModelBase
    {
        public static PreferencesWindowViewModel Instance { get; } = FromJson();

        private const string path = "Preferences.json";

        public bool EnableDarkTheme
        {
            get => enableDarkTheme;
            set
            {
                enableDarkTheme = value;
                UpdateTheme();
                OnPropertyChanged();
            }
        }
        private bool enableDarkTheme = true;

        public string ReleaseTag { get; set; } = "v0.22";

        public DateTime LastUpdateCheckTime = DateTime.UnixEpoch;

        public void UpdateTheme()
        {
            var replacingColorSchemeUri = new Uri("pack://application:,,,/CrossModGui;component/Resources/GrayscaleDark.xaml", UriKind.Absolute);
            var replacedColorSchemeUri = new Uri("pack://application:,,,/CrossModGui;component/Resources/GrayscaleLight.xaml", UriKind.Absolute);

            if (!EnableDarkTheme)
                AdonisUI.ResourceLocator.SetColorScheme(Application.Current.Resources, AdonisUI.ResourceLocator.LightColorScheme);
            else
                AdonisUI.ResourceLocator.SetColorScheme(Application.Current.Resources, replacingColorSchemeUri, replacedColorSchemeUri);
        }

        public void SaveChangesToFile()
        {
            var json = JsonConvert.SerializeObject(this);
            System.IO.File.WriteAllText(path, json);
        }

        private PreferencesWindowViewModel()
        {

        }

        private static PreferencesWindowViewModel FromJson()
        {
            if (!System.IO.File.Exists(path))
            {
                return new PreferencesWindowViewModel();
            }

            var result = JsonConvert.DeserializeObject<PreferencesWindowViewModel>(System.IO.File.ReadAllText(path));
            result.UpdateTheme();
            return result;
        }
    }
}
