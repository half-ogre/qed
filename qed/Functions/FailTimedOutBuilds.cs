using System;

namespace qed
{
    public static partial class Functions
    {
        public static void FailTimedOutBuilds(Action<string> logConsoleMessage)
        {
            var timedOutbuilds = GetTimedOutBuilds();

            foreach (var timedOutBuild in timedOutbuilds)
            {
                logConsoleMessage(String.Format("Build #{0} timed out; setting to failed.", timedOutBuild.Id));
                AppendBuildOutput(timedOutBuild, "ERROR: The build exceeded the timeout; setting to failed.");
                SetBuildFinished(timedOutBuild, false, DateTimeOffset.UtcNow);
            }
        }
    }
}