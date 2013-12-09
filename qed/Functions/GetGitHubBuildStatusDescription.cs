using System;

namespace qed
{
    public static partial class Functions
    {
        internal static string GetGitHubBuildStatusDescription(Build build)
        {
            if (build == null) throw new ArgumentNullException("build");

            var now = DateTimeOffset.UtcNow;

            if (!build.Started.HasValue)
                return String.Format("Build #{0} queued.", build.Id);

            if (!build.Finished.HasValue)
                return String.Format("Build #{0} started.", build.Id);

            if (!build.Succeeded.HasValue)
                return String.Format("Build #{0} finished in {1} seconds.", build.Id, build.Started.Value.Until(build.Finished.Value));

            if (build.Succeeded.Value)
                return String.Format("Build #{0} succeeded in {1} seconds.", build.Id, build.Started.Value.Until(build.Finished.Value));

            return String.Format("Build #{0} failed in {1} seconds.", build.Id, build.Started.Value.Until(build.Finished.Value));
        }
    }
}
