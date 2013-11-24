using System;

namespace qed
{
    public static partial class Functions
    {
        public static string GetBuildDescription(Build build)
        {
            return GetBuildDescription(build, DateTimeOffset.UtcNow);
        }

        internal static string GetBuildDescription(Build build, DateTimeOffset now)
        {
            if (build == null) throw new ArgumentNullException("build");

            if (!build.Started.HasValue)
            {
                if (!build.Queued.HasValue)
                    return String.Format("Build #{0} queued.", build.Id);

                return String.Format("Build #{0} queued {1} seconds ago.", build.Id, build.Queued.Value.Since(now));
            }

            if (!build.Finished.HasValue)
                return String.Format("Build #{0} started {1} seconds ago.", build.Id, build.Started.Value.Since(now));

            if (!build.Succeeded.HasValue)
                return String.Format("Build #{0} finished in {1} seconds.", build.Id, build.Started.Value.Until(build.Finished.Value));

            if (build.Succeeded.Value)
                return String.Format("Build #{0} succeeded in {1} seconds.", build.Id, build.Started.Value.Until(build.Finished.Value));

            return String.Format("Build #{0} failed in {1} seconds.", build.Id, build.Started.Value.Until(build.Finished.Value));
        }
    }
}
