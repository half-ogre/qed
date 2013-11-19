using System;
using System.Threading.Tasks;

namespace qed
{
    public static partial class Functions
    {
        public static int RunBuild(
            Build build, 
            string repositoryDirectory,
            Action<string> log)
        {
            log("STEP: Running build command.");

            var process = CreateProcess(build.Command, build.CommandArguments, repositoryDirectory);

            var exitCode = RunProcess(process, log);

            if (exitCode > 0)
                log("FAILED: Running build command failed. Examine the output above this message for errors or an explanation.");
            else
                log("Finished running build command.");

            log(""); // this line intentionally left blank

            return exitCode;
        }
    }
}
