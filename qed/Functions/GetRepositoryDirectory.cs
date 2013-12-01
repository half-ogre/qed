using System.IO;

namespace qed
{
    public static partial class Functions
    {
        public static string GetRepositoryDirectory(Build build)
        {
            var repositoryDirectory = Path.Combine(GetRepositoryOwnerDirectory(build), build.RepositoryName);
            return repositoryDirectory;
        }
    }
}
