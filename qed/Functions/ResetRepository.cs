using System;

namespace qed
{
    public static partial class Functions
    {
        public static bool ResetRepository(
            Build build,
            string repositoryDirectory,
            Action<string> log)
        {
            log("STEP: Reseting repository.");

            Func<bool> step = () =>
            {
                using (var process = CreateProcess(
                    "git.exe",
                    String.Concat("reset --hard ", build.Revision), repositoryDirectory))
                {
                    return RunProcess(process, log) == 0;
                }
            };

            return RunStep(step, log);
        }
    }
}
