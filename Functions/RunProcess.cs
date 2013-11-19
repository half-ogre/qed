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
            process.EnableRaisingEvents = true;

            process.OutputDataReceived += (sender, args) =>
            {
                if (args.Data == null) return;
                log(args.Data);
            };

            process.ErrorDataReceived += (sender, args) =>
            {
                if (args.Data == null) return;
                log(args.Data);
            };

            log(String.Format(
                "Running {0} {1} in {2}", 
                process.StartInfo.FileName,
                process.StartInfo.Arguments,
                process.StartInfo.WorkingDirectory));
            
            // TODO: log start info
            // TODO: use a timeout
            process.Start();

            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            process.WaitForExit();
            var exitCode = process.ExitCode;
            process.Dispose();

            return exitCode;
        }
    }
}
