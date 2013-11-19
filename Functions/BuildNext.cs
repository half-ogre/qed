using System;
using Raven.Client;

namespace qed
{
    public static partial class Functions
    {
        public static void BuildNext()
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
                if (!RunStep(build, CloneRepository(buildConfiguration, build, repositoryOwnerDirectory, repositoryDirectory, log)))
                {
                    return;
                }

                if (!RunStep(build, ResetRepository(build, repositoryDirectory, log)))
                {
                    return;
                }

                if (!RunStep(build, FetchRepository(repositoryDirectory, log)))
                {
                    return;
                }

                if (!RunStep(build, CleanRepository(repositoryDirectory, log)))
                {
                    return;
                }

                if (!RunStep(build, RunBuild(build, repositoryDirectory, log)))
                {
                    return;
                }

                SetBuildFinished(build, true, DateTimeOffset.UtcNow);
            }
            catch(Exception ex)
            {
                log("ERROR: An unexpected error occurred:");
                log(ex.ToString());
                log(""); // this line intentionally left blank
                SetBuildFinished(build, false, DateTimeOffset.UtcNow);
            }
        }

        static bool RunStep(
            Build build,
            int exitCode)
        {
            if (exitCode > 0)
            {
                SetBuildFinished(build, false, DateTimeOffset.UtcNow);
                return false;
            }

            return true;
        }
    }
}
