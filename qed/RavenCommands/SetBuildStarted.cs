using System;

namespace qed
{
    public static partial class Functions
    {
        public static void SetBuildStarted(
            Build build,
            DateTimeOffset started)
        {
            using (var ravenSession = OpenRavenSession())
            {
                build.Started = started;

                ravenSession.Store(build);
                ravenSession.SaveChanges();
            }
        }
    }
}
