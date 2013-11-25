using System;
using System.Threading.Tasks;
using Octokit;

namespace qed
{
    public static partial class Functions
    {
        public static Task SetGitHubBuildStatus(
            Build build,
            CommitState state)
        {
            return SetGitHubBuildStatus(
                build,
                state,
                GetBuildConfiguration,
                GetBuildDescription,
                GetHost,
                async (token, owner, name, sha, commitState, targetUrl, description) =>
                {
                    var gitHubClient = GetGitHubClient();
                    gitHubClient.Credentials = new Credentials(token);

                    await gitHubClient.Repository.CommitStatus.Create(
                        owner,
                        name,
                        sha,
                        new NewCommitStatus
                        {
                            State = commitState,
                            TargetUrl = targetUrl,
                            Description = description
                        });
                });
        }

        internal static async Task SetGitHubBuildStatus(
            Build build,
            CommitState state,
            Func<string, string, BuildConfiguration> getBuildConfiguration,
            Func<Build, string> getBuildDescription,
            Func<Task<string>> getHost,
            Func<string, string, string, string, CommitState, Uri, string, Task> createGitHubCommitStatus)
        {
            var buildConfiguration = getBuildConfiguration(
                build.RepositoryOwner,
                build.RepositoryName);

            if (buildConfiguration == null)
                throw new Exception("Could not find build configuration.");

            var targetUrl = new Uri(String.Format(
                "http://{0}/{1}/{2}/builds/{3}",
                await getHost(),
                build.RepositoryOwner,
                build.RepositoryName,
                build.Id));

            await createGitHubCommitStatus(
                buildConfiguration.Token,
                build.RepositoryOwner,
                build.RepositoryName,
                build.Revision,
                state,
                targetUrl,
                null);
        }
    }
}
