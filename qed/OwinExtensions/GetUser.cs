using System;
using System.Collections.Generic;
using Microsoft.Owin;

namespace qed
{
    public static partial class Functions
    {
        public static User GetUser(this IDictionary<string, object> environment)
        {
            var owinContext = new OwinContext(environment);

            var principal = owinContext.Request.User;

            if (principal == null || String.IsNullOrEmpty(principal.Identity.Name))
                return null;

            return GetUserByUsername(principal.Identity.Name);
        }
    }
}
