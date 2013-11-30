using System.Collections.Generic;
using System.Linq;
using Raven.Client.Linq;

namespace qed
{
    public static partial class Functions
    {
        public static IList<Build> GetBuilds(
            string repositoryOwner,
            string repositoryName)
        {
            using (var ravenSession = OpenRavenSession())
            {
                return ravenSession.Query<Build>()
                    .Where(build =>
                        build.RepositoryOwner == repositoryOwner &&
                        build.RepositoryName == repositoryName)
                    .ToList();
            }
        }
    }
}
