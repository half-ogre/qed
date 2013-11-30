using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace qed
{
    public static partial class Functions
    {
        public static int RunProcess(
            Process process, 
            Action onFinished,
            Action<string> log)
        {
            const int timeout = 5 /* minutes */ * 60 /* seconds */ * 1000 /* milliseconds */;
            
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var getRemainingTimeout = new Func<int>(() =>
            {
                if (stopwatch.ElapsedMilliseconds >= timeout)
                    return 0;

                return timeout - unchecked((int)stopwatch.ElapsedMilliseconds);
            });

            try
            {
                log(String.Format(
                    "Running {0} {1} in {2}",
                    process.StartInfo.FileName,
                    process.StartInfo.Arguments,
                    process.StartInfo.WorkingDirectory));

                process.Start();

                var outputTask = new Task(() =>
                {
                    while (!process.StandardOutput.EndOfStream)
                        log(process.StandardOutput.ReadLine());
                });
                outputTask.Start();

                var errorTask = new Task(() =>
                {
                    while (!process.StandardError.EndOfStream)
                        log(process.StandardError.ReadLine());
                });
                errorTask.Start();

                if (!Task.WaitAll(new[] {outputTask, errorTask}, getRemainingTimeout()) ||
                    !process.WaitForExit(getRemainingTimeout()))
                {
                    log(String.Format("ERROR: The process ({0}) exceeded the timeout ({1}s); terminating now.", process.Id, timeout / 1000));
                    process.Kill();
                    process.Dispose();
                    return int.MaxValue;
                }

                var exitCode = process.ExitCode;
                return exitCode;
            }
            finally
            {
                onFinished();
            }
        }
    }
}
