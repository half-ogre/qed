using System;
using System.Diagnostics;

namespace qed
{
    public static partial class Functions
    {
        public static int FetchRepository(
            Build build,
            string repositoryDirectory,
            Action<string> log)
        {
            return FetchRepository(
                build,
                repositoryDirectory,
                log,
                CreateProcess,
                CreateFetchRefspec,
                RunProcess);
        }

        internal static int FetchRepository(
            Build build,
            string repositoryDirectory,
            Action<string> log,
            Func<string, string, string, Process> createProcess,
            Func<string, string> createFetchRefspec,
            Func<Process, Action<string>, int> runProcess)
        {
            log("STEP: Fetching repository.");

            var process = createProcess(
                "git.exe", 
                String.Concat("fetch origin ", createFetchRefspec(build.Ref)), 
                repositoryDirectory);

            var exitCode = runProcess(process, log);

            if (exitCode > 0)
                log("FAILED: Fetching repository failed. Examine the output above this message for errors or an explanation.");
            else
                log("Finished fetching repository.");

            log(""); // this line intentionally left blank

            return exitCode;
        }
    }
}
