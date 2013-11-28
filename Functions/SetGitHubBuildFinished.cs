using System;
using Octokit;

namespace qed
{
    public static partial class Functions
    {
        public static bool SetGitHubBuildFinished(
            Build build,
            bool succeeded,
            Action<string> logBuildMessage)
        {
            var state = succeeded
                ? CommitState.Success
                : CommitState.Failure;

            var stateName = Enum.GetName(typeof(CommitState), state);

            logBuildMessage(String.Format("STEP: Updating GitHub build status to {0}.", stateName));

            return RunStep(() =>
            {
                SetGitHubBuildStatus(build, state);
                return true;
            }, logBuildMessage);
        }
    }
}
