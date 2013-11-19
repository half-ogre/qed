using System.Diagnostics;

namespace qed
{
    public static partial class Functions
    {
        public static Process CreateProcess(
            string command,
            string arguments,
            string workingDirectory)
        {
            return new Process
            {
                StartInfo = new ProcessStartInfo(command, arguments)
                {
                    UseShellExecute = false,
                    WorkingDirectory = workingDirectory,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                }
            };
        }
    }
}
