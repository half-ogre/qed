using System;
using Octokit;

namespace qed
{
    public static partial class Functions
    {
        public static async void SetBuildStarted(
            Build build,
            DateTimeOffset started)
        {
            using (var ravenSession = OpenRavenSession())
            {
                build.Started = started;

                ravenSession.Store(build);
                ravenSession.SaveChanges();

                await SetGitHubBuildStatus(build, CommitState.Pending);
            }
        }
    }
}
