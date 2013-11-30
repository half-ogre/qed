using System.Linq;

namespace qed
{
    public static partial class Functions
    {
        public static Build GetNextQueuedBuild()
        {
            using (var ravenSession = OpenRavenSession())
            {
                return ravenSession.Query<Build>()
                    .FirstOrDefault(build => build.Started == null);
            }
        }
    }
}
