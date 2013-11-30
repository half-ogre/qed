using System;
using Octokit;

namespace qed
{
    public static partial class Functions
    {
        public static bool SetGitHubBuildStarted(
            BuildConfiguration buildConfiguration,
            Build build,
            Action<string> logBuildMessage)
        {
            logBuildMessage("STEP: Setting GitHub build status to Pending.");

            return RunStep(() =>
            {
                if (buildConfiguration.Token == null)
                {
                    logBuildMessage("The build configuration has no token; skipping.");
                    return true;
                }

                SetGitHubBuildStatus(build, CommitState.Pending);
                return true;
            }, logBuildMessage);
        }
    }
}
