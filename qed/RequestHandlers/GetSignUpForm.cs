using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using fn = qed.Functions;

namespace qed
{
    public static partial class Handlers
    {
        public static Task GetSignUpForm(
            IDictionary<string, object> environment,
            Func<IDictionary<string, object>, Task> next)
        {
            return environment.Render("sign-up-form", new { user = environment.GetUser().To(user => new { username = user.Username }) });
        }
    }
}
