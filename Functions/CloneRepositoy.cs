using System;
using System.IO;
using System.Threading.Tasks;

namespace qed
{
    public static partial class Functions
    {
        public static Task<bool> CloneRepository(
            BuildConfiguration buildConfiguration,
            Build build,
            string repositoryOwnerDirectory,
            string repositoryDirectory,
            Action<string> log)
        {
            log("STEP: Cloning repository.");

            return RunStep(() =>
            {
                log("Checking whether repository has already been cloned.");

                if (Directory.Exists(repositoryDirectory))
                {
                    log(String.Format("{0} already exists. Skipping clone.", repositoryDirectory));
                    return true;
                }

                var url = GetRepositoryUrl(buildConfiguration.Owner, buildConfiguration.Name, buildConfiguration.Token);

                var process = CreateProcess(
                    command: "git.exe",
                    arguments: String.Format("clone --recursive {0} {1}", url, repositoryDirectory),
                    workingDirectory: repositoryOwnerDirectory);

                return RunProcess(process, log) == 0;
            }, log);
        }
    }
}
