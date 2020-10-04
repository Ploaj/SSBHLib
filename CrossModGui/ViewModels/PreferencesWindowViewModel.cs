using Newtonsoft.Json;

namespace CrossModGui.ViewModels
{
    public class PreferencesWindowViewModel : ViewModelBase
    {
        public static PreferencesWindowViewModel Instance { get; } = FromJson();

        public bool EnableDarkTheme { get; set; } = false;

        private const string path = "Preferences.json";

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

            return JsonConvert.DeserializeObject<PreferencesWindowViewModel>(System.IO.File.ReadAllText(path));
        }
    }
}
