using System;

namespace qed
{
    public static partial class Functions
    {
        public static string GetBuildDescription(Build build)
        {
            return GetBuildDescription(build, false);
        }

        public static string GetBuildDescription(Build build, bool includeRefDescription)
        {
            return GetBuildDescription(build, includeRefDescription, DateTimeOffset.UtcNow);
        }

        internal static string GetBuildDescription(Build build, bool includeRefDescription, DateTimeOffset now)
        {
            if (build == null) throw new ArgumentNullException("build");

            var @ref = "";
            if (includeRefDescription)
                @ref = String.Concat(" on ", GetRefDescription(build));

            if (!build.Started.HasValue)
            {
                if (!build.Queued.HasValue)
                    return String.Format("Build #{0}{1} queued.", build.Id, @ref);

                return String.Format("Build #{0}{1} queued {2} seconds ago.", build.Id, @ref, build.Queued.Value.Since(now));
            }

            if (!build.Finished.HasValue)
                return String.Format("Build #{0}{1} started {2} seconds ago.", build.Id, @ref, build.Started.Value.Since(now));

            if (!build.Succeeded.HasValue)
                return String.Format("Build #{0}{1} finished in {2} seconds.", build.Id, @ref, build.Started.Value.Until(build.Finished.Value));

            if (build.Succeeded.Value)
                return String.Format("Build #{0}{1} succeeded in {2} seconds.", build.Id, @ref, build.Started.Value.Until(build.Finished.Value));

            return String.Format("Build #{0}{1} failed in {2} seconds.", build.Id, @ref, build.Started.Value.Until(build.Finished.Value));
        }
    }
}
