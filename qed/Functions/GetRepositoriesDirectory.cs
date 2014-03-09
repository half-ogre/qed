using System.IO;
using fn = qed.Functions;

namespace qed
{
    public static partial class Functions
    {
        public static string GetRepositoriesDirectory()
        {
            var repositoriesDirectory = Path.Combine(GetBaseDirectory(), GetConfiguration<string>(Constants.Configuration.RepositoriesPathKey));
            EnsureDirectoryExists(repositoriesDirectory);
            return repositoriesDirectory;
        }
    }
}
