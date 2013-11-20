using System;

namespace qed
{
    public static partial class Functions
    {
        public static string CreateFetchRefspec(string @ref)
        {
            if (@ref.StartsWith("refs/heads/", StringComparison.OrdinalIgnoreCase))
            {
                var branch = @ref.Substring(11);
                return String.Format("+refs/heads/{0}:refs/remotes/origin/{0}", branch);
            }

            if (@ref.StartsWith("refs/pull/", StringComparison.OrdinalIgnoreCase))
            {
                var slashafterPrNumberIndex = @ref.IndexOf("/", 10, StringComparison.InvariantCultureIgnoreCase);
                var prNumber = @ref.Substring(10, slashafterPrNumberIndex - 10);
                return String.Format("+refs/pull/{0}/head:refs/remotes/origin/pr/{0}", prNumber);
            }

            throw new ArgumentException(String.Format("Unexpected type of ref: {0}.", @ref), "ref");
        }
    }
}
