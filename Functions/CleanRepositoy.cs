using System;

namespace qed
{
    public static partial class Functions
    {
        public static int CleanRepository(
            string repositoryDirectory,
            Action<string> log)
        {
            log("STEP: Cleaning repository.");
            
            var process = CreateProcess("git.exe", "clean -xdf", repositoryDirectory);

            var exitCode = RunProcess(process, log);

            if (exitCode > 0)
                log("FAILED: Cleaning repository failed. Examine the output above this message for errors or an explanation.");
            else
                log("Finished cleaning repository.");

            log(""); // this line intentionally left blank

            return exitCode;
        }
    }
}
