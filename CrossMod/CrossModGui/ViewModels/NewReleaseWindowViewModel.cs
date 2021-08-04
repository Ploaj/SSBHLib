using System;
using System.Collections.Generic;
using System.Text;

namespace CrossModGui.ViewModels
{
    public class NewReleaseWindowViewModel : ViewModelBase
    {
        public string ReleaseNotesMarkdown { get; }

        public string VersionText { get; }

        public string LinkText { get; }

        public NewReleaseWindowViewModel(string currentTag, string newTag, string releaseNotesMarkdown, string releaseLink)
        {
            LinkText = releaseLink;
            VersionText = $"Cross Mod version {newTag} is available. The current version is {currentTag}. Download the new version from here:";

            // Extract the relevant portions of the change log.
            var startIndex = releaseNotesMarkdown.IndexOf($"**{newTag}**");
            var endIndex = releaseNotesMarkdown.IndexOf($"**{currentTag}**");

            var changeLogMarkdown = "";
            if (startIndex != -1 && endIndex != -1)
                changeLogMarkdown = releaseNotesMarkdown[startIndex..endIndex];

            // TODO: Add an ok button to the dialog?
            ReleaseNotesMarkdown = changeLogMarkdown;
        }
    }
}
