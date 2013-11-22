using System;
using System.Threading.Tasks;

namespace qed
{
    public static partial class Functions
    {
        public static Task<bool> CleanRepository(
            string repositoryDirectory,
            Action<string> log)
        {
            log("STEP: Cleaning repository.");

            return RunStep(() =>
            {
                var process = CreateProcess(
                    "git.exe",
                    "clean -xdf",
                    repositoryDirectory);

                return RunProcess(process, log) == 0;
            }, log);
        }
    }
}
