using System;
using System.Diagnostics;

namespace qed
{
    public static partial class Functions
    {
        public static int RunProcess(
            Process process, 
            Action<string> log)
        {
            log(String.Format(
                "Running {0} {1} in {2}", 
                process.StartInfo.FileName,
                process.StartInfo.Arguments,
                process.StartInfo.WorkingDirectory));
            
            // TODO: log start info
            // TODO: use a timeout
            process.Start();

            var output = process.StandardOutput
                .ReadToEnd()
                .TrimEnd();

            var errors = process.StandardError
                .ReadToEnd()
                .TrimEnd();

            process.WaitForExit();
            var exitCode = process.ExitCode;
            process.Dispose();

            if (exitCode == 0 && !String.IsNullOrEmpty(output))
                log(output);
            else if (!String.IsNullOrEmpty(errors))
                log(errors);

            return exitCode;
        }
    }
}
