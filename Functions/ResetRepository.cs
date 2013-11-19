using System;

namespace qed
{
    public static partial class Functions
    {
        public static int ResetRepository(
            Build build,
            string repositoryDirectory,
            Action<string> log)
        {
            log("STEP: Reseting repository.");

            var process = CreateProcess("git.exe", String.Concat("reset --hard ", build.Revision), repositoryDirectory);

            var exitCode = RunProcess(process, log);

            if (exitCode > 0)
                log("FAILED: Reseting repository failed. Examine the output above this message for errors or an explanation.");
            else
                log("Finished reseting repository.");

            log(""); // this line intentionally left blank

            return exitCode;
        }
    }
}
