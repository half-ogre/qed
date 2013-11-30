using System.Linq;

namespace qed
{
    public static partial class Functions
    {
        public static Build GetLastFinishedBuild(
            string repositoryOwner, 
            string repositoryName)
        {
            using (var ravenSession = OpenRavenSession())
            {
                return ravenSession.Query<Build>()
                    .Where(build => build.RepositoryOwner == repositoryOwner &&
                                    build.RepositoryName == repositoryName && 
                                    build.Finished != null)
                    .OrderByDescending(build => build.Finished)
                    .FirstOrDefault();
            }
        }
    }
}
