using System;
using Octokit   ;

namespace qed
{
    public static partial class Functions
    {
        public static void SetGitHubBuildStatus(
            Build build,
            CommitState state)
        {
            SetGitHubBuildStatus(
                build,
                state,
                GetBuildConfiguration,
                GetBuildDescription,
                GetHost,
                (token, owner, name, sha, commitState, targetUrl, description) =>
                {
                    var gitHubClient = GetGitHubClient();
                    gitHubClient.Credentials = new Credentials(token);

                    gitHubClient.Repository.CommitStatus.Create(
                        owner,
                        name,
                        sha,
                        new NewCommitStatus
                        {
                            State = commitState,
                            TargetUrl = targetUrl,
                            Description = description
                        }).Wait();
                });
        }

        internal static void SetGitHubBuildStatus(
            Build build,
            CommitState state,
            Func<string, string, BuildConfiguration> getBuildConfiguration,
            Func<Build, string> getBuildDescription,
            Func<string> getHost,
            Action<string, string, string, string, CommitState, Uri, string> createGitHubCommitStatus)
        {
            var buildConfiguration = getBuildConfiguration(
                build.RepositoryOwner,
                build.RepositoryName);

            if (buildConfiguration == null)
                throw new Exception("Could not find build configuration.");

            var targetUrl = new Uri(String.Format(
                "http://{0}/{1}/{2}/builds/{3}",
                getHost(),
                build.RepositoryOwner,
                build.RepositoryName,
                build.Id));

            createGitHubCommitStatus(
                buildConfiguration.Token,
                build.RepositoryOwner,
                build.RepositoryName,
                build.Revision,
                state,
                targetUrl,
                getBuildDescription(build));
        }
    }
}
