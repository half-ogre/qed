using System;
using System.Threading.Tasks;

namespace qed
{
    public static partial class Functions
    {
        public static Task<bool> ResetRepository(
            Build build,
            string repositoryDirectory,
            Action<string> log)
        {
            log("STEP: Reseting repository.");

            return RunStep(() =>
            {
                var process = CreateProcess(
                    "git.exe",
                    String.Concat("reset --hard ", build.Revision), repositoryDirectory);

                return RunProcess(process, log) == 0;
            }, log);
        }
    }
}
