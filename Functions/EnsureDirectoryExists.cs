using System.IO;

namespace qed
{
    public static partial class Functions
    {
        public static void EnsureDirectoryExists(string directory)
        {
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);
        }
    }
}
