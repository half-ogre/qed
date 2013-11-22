using System;

namespace qed
{
    public static partial class Functions
    {
        public static void SetBuildFinished(
            Build build,
            bool succeeded,
            DateTimeOffset finished)
        {
            using (var ravenSession = OpenRavenSession())
            {
                build.Succeeded = succeeded;
                build.Finished = finished;

                ravenSession.Store(build);
                ravenSession.SaveChanges();
            }
        }
    }
}
