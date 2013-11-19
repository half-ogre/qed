using System.Collections.Generic;

namespace qed
{
    public static partial class Functions
    {
        public static IDictionary<string, string[]> GetResponseHeaders(this IDictionary<string, object> environment)
        {
            return environment.Get<IDictionary<string, string[]>>(Constants.Owin.ResponseHeadersKey);
        }
    }
}
