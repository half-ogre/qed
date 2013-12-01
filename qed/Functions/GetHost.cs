using System;
using Octokit;

namespace qed
{
    public static partial class Functions
    {
        public static string GetHost()
        {
            var host = GetConfiguration<string>(Constants.Configuration.HostKey);
            if (host != null)
                return host;

            var github = GetGitHubClient();

            var response = github.Connection.GetHtml(new Uri("http://ifconfig.me/ip", UriKind.Absolute)).Result;
            host = response.Body.TrimEnd('\r', '\n');

            SetConfiguration(Constants.Configuration.HostKey, host);

            return host;
        }
    }
}