using System;
using System.Threading.Tasks;

namespace qed
{
    public static partial class Functions
    {
        public static async Task BuildNext()
        {
            var build = GetNextPendingBuild();

            if (build == null)
                return;

            SetBuildStarted(build, DateTimeOffset.UtcNow);

            var log = new Action<string>(message =>
            {
                Console.WriteLine(message);
                AppendBuildOutput(build, message);
            });

            var buildConfiguration = GetBuildConfiguration(
                build.RepositoryOwner,
                build.RepositoryName);
                
            if (buildConfiguration == null)
            {
                log(String.Format(
                    "ERROR: The is no build configuration for {0}/{1}.",
                    build.RepositoryOwner,
                    build.RepositoryName));
                SetBuildFinished(build, false, DateTimeOffset.UtcNow);
                return;
            }

            var repositoryOwnerDirectory = GetRepositoryOwnerDirectory(build);
            var repositoryDirectory = GetRepositoryDirectory(build);

            try
            {
                var succeeded = await CloneRepository(
                    buildConfiguration,
                    build,
                    repositoryOwnerDirectory,
                    repositoryDirectory,
                    log);

                if (succeeded)
                    succeeded = await CleanRepository(
                        repositoryDirectory,
                        log);

                if (succeeded)
                    succeeded = await FetchRepository(
                        build,
                        repositoryDirectory,
                        log);

                if (succeeded)
                    succeeded = await ResetRepository(
                        build,
                        repositoryDirectory,
                        log);

                if (succeeded)
                    succeeded = await RunBuild(
                        build,
                        repositoryDirectory,
                        log);

                SetBuildFinished(build, succeeded, DateTimeOffset.UtcNow);
            }
            catch(Exception ex)
            {
                log("ERROR: An unexpected error occurred:");
                log(ex.ToString());
                log(""); // this line intentionally left blank
                SetBuildFinished(build, false, DateTimeOffset.UtcNow);
            }
        }
    }
}
