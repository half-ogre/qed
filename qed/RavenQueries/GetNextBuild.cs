using System.Linq;

namespace qed
{
    public static partial class Functions
    {
        public static Build GetNextBuild(
            string repositoryOwner, 
            string repositoryName)
        {
            using (var ravenSession = OpenRavenSession())
            {
                return ravenSession.Query<Build>()
                    .Where(build => build.RepositoryOwner == repositoryOwner &&
                                    build.RepositoryName == repositoryName && 
                                    build.Started == null)
                    .OrderBy(build => build.Queued)
                    .FirstOrDefault();
            }
        }
    }
}
