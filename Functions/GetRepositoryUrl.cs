using System;

namespace qed
{
    public static partial class Functions
    {
        public static Uri GetRepositoryUrl(
            string owner,
            string name,
            string token = null)
        {
            var url = new Uri(String.Format("https://github.com/{0}/{1}", owner, name));

            if (token != null)
            {
                var uriBuilder = new UriBuilder(url)
                {
                    UserName = token,
                    Password = ""
                };

                url = uriBuilder.Uri;
            }

            return url;
        }
    }
}
