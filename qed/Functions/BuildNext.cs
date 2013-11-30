using System;

namespace qed
{
    public static partial class Functions
    {
        public static void BuildNext(Action<string> logConsoleMessage)
        {
            var build = GetNextQueuedBuild();

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
                        build.Revision ?? "HEAD",
                        build.Id));
            logBuildMessage(""); // this line intentionally left blank

            try
            {
                var succeeded = 
                    CloneRepository(buildConfiguration, build, repositoryOwnerDirectory, repositoryDirectory, logBuildMessage) &&
                    CleanRepository(repositoryDirectory, logBuildMessage) &&
                    FetchRepository(build, repositoryDirectory, logBuildMessage) &&
                    GetHeadSha(build, repositoryDirectory, logBuildMessage) &&
                    ResetRepository(build, repositoryDirectory, logBuildMessage) &&
                    SetGitHubBuildStarted(buildConfiguration, build, logBuildMessage) &&
                    RunBuild(build, repositoryDirectory, logBuildMessage);

                SetBuildFinished(build, succeeded, DateTimeOffset.UtcNow);
                
                SetGitHubBuildFinished(buildConfiguration, build, succeeded, logBuildMessage);
            }
            catch (Exception ex)
            {
                SetBuildFinished(build, false, DateTimeOffset.UtcNow);
                logBuildMessage("ERROR: An unexpected error occurred: ");
                logBuildMessage(ex.ToString());
            }
            finally
            {
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