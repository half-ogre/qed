using System;

namespace qed
{
    public static partial class Functions
    {
        public static string GetRefDescription(Build build)
        {
            if (build.Ref.StartsWith("refs/heads/", StringComparison.OrdinalIgnoreCase))
            {
                var branch = build.Ref.Substring(11);
                return String.Concat(build.RepositoryName, "/", branch);
            }

            if (build.Ref.StartsWith("refs/pull/", StringComparison.OrdinalIgnoreCase))
            {
                var slashafterPrNumberIndex = build.Ref.IndexOf("/", 10, StringComparison.InvariantCultureIgnoreCase);
                var prNumber = build.Ref.Substring(10, slashafterPrNumberIndex - 10);
                return String.Concat(build.RepositoryName, " pull request #", prNumber);
            }

            throw new ArgumentException(String.Format("Unexpected type of ref: {0}.", build.Ref), "build");
        }
    }
}
