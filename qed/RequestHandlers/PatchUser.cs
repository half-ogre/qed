using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using OwinExtensions;
using fn = qed.Functions;

namespace qed
{
    public static partial class Handlers
    {
        public static Task PatchUser(
            IDictionary<string, object> environment,
            dynamic @params,
            Func<IDictionary<string, object>, Task> next)
        {
            var username = @params.username as string;

            var user = fn.GetUserByUsername(username);
            if (user == null)
            {
                environment.SetStatusCode(404);
                return environment.GetCompleted();
            }

            var form = environment.ReadForm();

            var isAdministrator = form["isAdministrator"].FirstOrDefault() ?? "0";

            fn.SetUserIsAdministrator(user, isAdministrator == "1");

            environment.SetRedirect("/users");

            return environment.GetCompleted();
        }
    }
}
