using System;

namespace qed
{
    public static partial class Functions
    {
        public static bool RunBuild(
            Build build, 
            string repositoryDirectory,
            Action<string> log)
        {
            log("STEP: Running build command.");

            Func<bool> step = () =>
            {
                using (var process = CreateProcess(
                    build.Command,
                    build.CommandArguments,
                    repositoryDirectory))
                {
                    return RunProcess(process, log) == 0;
                }
            };

            return RunStep(step, log);
        }
    }
}
