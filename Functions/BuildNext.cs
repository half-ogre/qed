using System;
using System.Threading.Tasks;

namespace qed
{
    public static partial class Functions
    {
        public static async Task BuildNext(Action<string> logConsoleMessage)
        {
            var build = GetNextPendingBuild();

            // TODO: log the non-build error somewhere
            if (build == null)
                return;

            var buildConfiguration = GetBuildConfiguration(
                build.RepositoryOwner,
                build.RepositoryName);

            // TODO: log the non-build error somewhere
            if (buildConfiguration == null)
                return;

            var logBuildMessage = new Action<string>(message =>
            {
                logConsoleMessage(message);
                AppendBuildOutput(build, message);
            });

            var repositoryOwnerDirectory = GetRepositoryOwnerDirectory(build);
            var repositoryDirectory = GetRepositoryDirectory(build);

            SetBuildStarted(build, DateTimeOffset.UtcNow);

            logBuildMessage(
                    String.Format(
                        "STARTED: Building {0} at revision {1} (#{2}).",
                        GetRefDescription(build),
                        build.Revision,
                        build.Id));
            logBuildMessage(""); // this line intentionally left blank

            var succeeded = false;

            try
            {
                succeeded = 
                    await SetGitHubBuildStarted(build, logBuildMessage) &&
                    await CloneRepository(buildConfiguration, build, repositoryOwnerDirectory, repositoryDirectory, logBuildMessage) &&
                    await CleanRepository(repositoryDirectory, logBuildMessage) &&
                    await FetchRepository(build, repositoryDirectory, logBuildMessage) &&
                    await ResetRepository(build, repositoryDirectory, logBuildMessage) &&
                    await RunBuild(build, repositoryDirectory, logBuildMessage);

                await SetGitHubBuildFinished(build, succeeded, logBuildMessage);
            }
            catch (Exception ex)
            {
                succeeded = false;
                logBuildMessage("ERROR: An unexpected error occurred: ");
                logBuildMessage(ex.ToString());
            }
            finally
            {
                SetBuildFinished(build, succeeded, DateTimeOffset.UtcNow);

                logBuildMessage(
                    String.Format(
                        "FINISHED: Building {0} at revision {1} in {2} seconds.",
                        GetRefDescription(build),
                        build.Revision,
                        (build.Finished.GetValueOrDefault() - build.Started.GetValueOrDefault()).TotalSeconds));
                logBuildMessage(""); // this line intentionally left blank
            }
        }
    }
}