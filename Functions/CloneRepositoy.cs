using System;
using System.IO;

namespace qed
{
    public static partial class Functions
    {
        public static int CloneRepository(
            BuildConfiguration buildConfiguration,
            Build build,
            string repositoryOwnerDirectory,
            string repositoryDirectory,
            Action<string> log)
        {
            log("STEP: Cloning repository.");

            log("Checking whether repository has already been cloned.");

            var exitCode = 0;
            if (Directory.Exists(repositoryDirectory))
            {
                log(String.Format("{0} already exists. Skipping clone.", repositoryDirectory));
            }
            else
            {
                var uriBuilder = new UriBuilder(build.RepositoryUrl)
                {
                    UserName = buildConfiguration.Token,
                    Password = ""
                };

                var url = uriBuilder.ToString();

                var process = CreateProcess(
                    command: "git.exe",
                    arguments: String.Format("clone --recursive {0} {1}", url, repositoryDirectory),
                    workingDirectory: repositoryOwnerDirectory);

                exitCode = RunProcess(process, log);
            }

            if (exitCode > 0)
                log("FAILED: Cloning repository failed. Examine the output above this message for errors or an explanation.");
            else
                log("Finished cloning repository.");

            log(""); // this line intentionally left blank

            return exitCode;
        }
    }
}
