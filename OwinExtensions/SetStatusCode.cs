using System.Collections.Generic;

namespace qed
{
    public static partial class OwinExtensions
    {
        public static void SetStatusCode(
            this IDictionary<string, object> environment, 
            int statusCode)
        {
            environment[Constants.Owin.ResponseStatusCodeKey] = statusCode;
        }
    }
}
