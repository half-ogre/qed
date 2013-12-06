using System.Collections.Generic;
using Microsoft.Owin;

namespace qed
{
    public static partial class Functions
    {
        public static void RemoveUser(this IDictionary<string, object> environment)
        {
            var owinContext = new OwinContext(environment);

            owinContext.Authentication.SignOut(new [] { "Session" });
        }
    }
}
