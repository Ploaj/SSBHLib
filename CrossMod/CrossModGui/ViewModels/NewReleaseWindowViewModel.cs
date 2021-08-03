using System;
using System.Collections.Generic;
using System.Text;

namespace CrossModGui.ViewModels
{
    public class NewReleaseWindowViewModel : ViewModelBase
    {
        public string ReleaseNotesMarkdown { get; }

        public NewReleaseWindowViewModel(string currentTag, string newTag, string releaseNotesMarkdown)
        {
            // Extract the relevant portions of the change log.
            var startIndex = releaseNotesMarkdown.IndexOf($"**{newTag}**");
            var endIndex = releaseNotesMarkdown.IndexOf($"**{currentTag}**");

            var changeLogMarkdown = "";
            if (startIndex != -1 && endIndex != -1)
                changeLogMarkdown = releaseNotesMarkdown[startIndex..endIndex];

            // TODO: Display the download link.
            // TODO: Add an ok button to the dialog?

            // Just insert the summary in the markdown to avoid having to match font sizes with WPF labels.
            var fullMarkdown = $"Cross Mod version {newTag} is available. The current version is {currentTag}.  \nRelease Notes:  \n{changeLogMarkdown}";
            ReleaseNotesMarkdown = fullMarkdown;
        }
    }
}
