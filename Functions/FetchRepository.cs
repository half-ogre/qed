using System;

namespace qed
{
    public static partial class Functions
    {
        public static int FetchRepository(
            string repositoryDirectory,
            Action<string> log)
        {
            log("STEP: Fetching repository.");

            var process = CreateProcess("git.exe", "fetch", repositoryDirectory);

            var exitCode = RunProcess(process, log);

            if (exitCode > 0)
                log("FAILED: Fetching repository failed. Examine the output above this message for errors or an explanation.");
            else
                log("Finished fetching repository.");

            log(""); // this line intentionally left blank

            return exitCode;
        }
    }
}
