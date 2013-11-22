using System;
using System.Threading.Tasks;
using Octokit;

namespace qed
{
    public static partial class Functions
    {
        public static async Task<bool> SetGitHubBuildStarted(
            Build build,
            Action<string> logBuildMessage)
        {
            logBuildMessage("STEP: Setting GitHub build status to Pending.");

            return await RunStep(async () =>
            {
                await SetGitHubBuildStatus(build, CommitState.Pending);
                return true;
            }, logBuildMessage);
        }
    }
}
