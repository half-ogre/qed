using System.IO;

namespace qed
{
    public static partial class Functions
    {
        public static string GetRepositoryOwnerDirectory(Build build)
        {
            var repositoryOwnerDirectory = Path.Combine(GetRepositoriesDirectory(), build.RepositoryOwner);
            EnsureDirectoryExists(repositoryOwnerDirectory);
            return repositoryOwnerDirectory;
        }
    }
}
