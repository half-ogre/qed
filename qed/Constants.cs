using System.Text.RegularExpressions;

namespace qed
{
    public static class Constants
    {
        public static readonly Regex UsernameRegex = new Regex("^[a-z][a-z0-9_-]+[a-z0-9]$", RegexOptions.IgnoreCase);

        public static class Configuration
        {
            public const string BaseDirectoryKey = "BaseDirectory";
            public const string BuildConfigurationsKey = "BuildConfigurations";
            public const string HostKey = "Host";
            public const string RavenStoreKey = "RavenStore";
        }
    }
}
