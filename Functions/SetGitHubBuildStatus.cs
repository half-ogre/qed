using System;
using System.Linq;
using System.Threading.Tasks;
using Octokit;

namespace qed
{
    public static partial class Functions
    {
        public static async Task SetGitHubBuildStatus(
            Build build,
            CommitState state)
        {
            var buildConfigurations = GetBuildConfigurations();

            var buildConfiguration = buildConfigurations.FirstOrDefault(bc =>
                bc.Name.Equals(build.RepositoryName, StringComparison.OrdinalIgnoreCase) &&
                bc.Owner.Equals(build.RepositoryOwner, StringComparison.OrdinalIgnoreCase));

            if (buildConfiguration == null)
                throw new Exception("Could not find build configuration.");

            var github = GetGitHubClient();
            github.Credentials = new Credentials(buildConfiguration.Token);

            await github.Repository.CommitStatus.Create(
                build.RepositoryOwner,
                build.RepositoryName,
                build.Revision,
                new NewCommitStatus
                {
                    State = state,
                    TargetUrl = new Uri(String.Format(
                        "http://{0}/{1}/{2}/builds/{3}",
                        await GetHost(),
                        build.RepositoryOwner,
                        build.RepositoryName,
                        build.Id))
                });
        }
    }
}
