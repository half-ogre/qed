using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.Owin;
using OwinExtensions;
using fn = qed.Functions;

namespace qed
{
    using AuthenticateCallback = Action<IIdentity, IDictionary<string, string>, IDictionary<string, object>, object>;
    using AuthenticateDelegate = Func<string[], Action<IIdentity, IDictionary<string, string>, IDictionary<string, object>, object>, object, Task>;

    public static partial class Handlers
    {
        public static Task DeleteSession(
            IDictionary<string, object> environment,
            Func<IDictionary<string, object>, Task> next)
        {
            var owinContext = new OwinContext(environment);

            owinContext.Authentication.SignOut(new [] { "Session" });

            environment.SetRedirect("/");
            return environment.GetCompleted();
        }
    }
}
