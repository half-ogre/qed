using System.Linq;
using Raven.Client;

namespace qed
{
    public static partial class Functions
    {
        public static int GetUsersCount()
        {
            using (var ravenSession = OpenRavenSession())
            {
                RavenQueryStatistics stats;
                
                ravenSession.Query<User>()
                    .Statistics(out stats)
                    .ToArray();

                return stats.TotalResults;
            }
        }
    }
}
