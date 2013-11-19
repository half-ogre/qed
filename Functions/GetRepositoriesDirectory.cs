using System.IO;

namespace qed
{
    public static partial class Functions
    {
        public static string GetRepositoriesDirectory()
        {
            var repositoriesDirectory = Path.Combine(GetBaseDirectory(), ".repositories");
            EnsureDirectoryExists(repositoriesDirectory);
            return repositoriesDirectory;
        }
    }
}
