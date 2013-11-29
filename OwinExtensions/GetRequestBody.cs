using System.Collections.Generic;
using System.IO;
using OwinExtensions;

namespace qed
{
    public static partial class Functions
    {
        public static Stream GetRequestBody(this IDictionary<string, object> environment)
        {
            return environment.Get<Stream>(Constants.Owin.RequestBodyKey);
        }
    }
}
