using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Principal;
using Microsoft.Owin;

namespace qed
{
    public static partial class Functions
    {
        public static void SetUser(
            this IDictionary<string, object> environment,
            User user)
        {
            var owinContext = new OwinContext(environment);

            var identity = new GenericIdentity(user.Username, "Session");

            owinContext.Authentication.SignIn(new[] { new ClaimsIdentity(identity) });
        }
    }
}
