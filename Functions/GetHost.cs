using System;
using System.Threading.Tasks;
using Octokit;

namespace qed
{
    public static partial class Functions
    {
        public static async Task<string> GetHost()
        {
            var host = GetConfiguration<string>(Constants.Configuration.HostKey);
            if (host != null)
                return host;

            var github = GetGitHubClient();

            var response = (await github.Connection.GetHtml(new Uri("http://ifconfig.me/ip", UriKind.Absolute)));
            host = response.Body.TrimEnd('\r', '\n');

            SetConfiguration(Constants.Configuration.HostKey, host);

            return host;
        }
    }
}