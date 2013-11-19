using System;
using Octokit;

namespace qed
{
    public static partial class Functions
    {
        public static async void SetBuildFinished(
            Build build,
            bool succeeded,
            DateTimeOffset finished)
        {
            using (var ravenSession = OpenRavenSession())
            {
                build.Succeeded = succeeded;
                build.Finished = finished;

                ravenSession.Store(build);
                ravenSession.SaveChanges();

                await SetGitHubBuildStatus(build, succeeded ? CommitState.Success : CommitState.Failure);
            }
        }
    }
}
