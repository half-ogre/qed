using System;
using Octokit;

namespace qed
{
    public static partial class Functions
    {
        public static bool SetGitHubBuildStarted(
            Build build,
            Action<string> logBuildMessage)
        {
            logBuildMessage("STEP: Setting GitHub build status to Pending.");

            return RunStep(() =>
            {
                SetGitHubBuildStatus(build, CommitState.Pending);
                return true;
            }, logBuildMessage);
        }
    }
}
