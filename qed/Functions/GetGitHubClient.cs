using Octokit;

namespace qed
{
    public static partial class Functions
    {
        public static GitHubClient GetGitHubClient()
        {
            return new GitHubClient(
                new ProductHeaderValue(
                    "qed",
                    typeof(Functions).Assembly.GetName().Version.ToString()));
        }
    }
}
