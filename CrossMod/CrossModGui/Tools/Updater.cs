using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Octokit;
using System.Linq;

namespace CrossModGui.Tools
{
    public static class Updater
    {
        const string owner = "Ploaj";
        const string repository = "SSBHLib";
        const string product = "CrossModGUI";
        public static async Task<Release?> TryFindNewerReleaseAsync(string currentReleaseTag)
        {
            var client = new GitHubClient(new ProductHeaderValue(product));
            var releases = await client.Repository.Release.GetAll(owner, repository);
            var latestRelease = releases.FirstOrDefault();
            if (latestRelease == null)
                return null;

            // Assume tags only contain the version, so compare tag names alphabetically to see if the release is newer.
            // ex: "0.16" is not greater than "0.16", so don't return a new release.
            if (latestRelease.TagName.CompareTo(currentReleaseTag) <= 0)
                return null;

            return latestRelease;
        }
    }
}
