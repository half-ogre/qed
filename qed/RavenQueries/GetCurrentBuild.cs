using System.Linq;

namespace qed
{
    public static partial class Functions
    {
        public static Build GetCurrentBuild(
            string repositoryOwner,
            string repositoryName)
        {
            using (var ravenSession = OpenRavenSession())
            {
                return ravenSession.Query<Build>()
                    .Where(build => build.RepositoryOwner == repositoryOwner &&
                                    build.RepositoryName == repositoryName &&
                                    build.Started != null &&
                                    build.Finished == null)
                    .OrderByDescending(build => build.Started)
                    .FirstOrDefault();
            }
        }
    }
}
