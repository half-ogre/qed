using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OwinExtensions;
using fn = qed.Functions;

namespace qed
{
    public static partial class Handlers
    {
        public static Task PutSession(
            IDictionary<string, object> environment,
            Func<IDictionary<string, object>, Task> next)
        {
            var form = environment.ReadForm();

            var username = form["username"].FirstOrDefault();
            var password = form["password"].FirstOrDefault();

            var errors = new List<string>();

            var fail = new Func<Task>(() =>
            {
                environment.SetStatusCode(400);

                return environment.Render("sign-in-form", new
                {
                    errors = errors.Select(x => new {error = x}),
                    username
                });
            });

            if (String.IsNullOrEmpty(username))
                errors.Add("Username is required.");

            if (String.IsNullOrEmpty(password))
                errors.Add("Password is required.");

            if (errors.Count > 0)
                return fail();

            var user = fn.GetUserByUsername(username);
            if (user == null || !password.CompareToPasswordHash(user.PasswordHash))
            {
                errors.Add("Sign in failed.");
                return fail();
            }

            if (errors.Count > 0)
            {
                environment.SetStatusCode(400);

                return environment.Render("sign-in-form", new
                {
                    errors = errors.Select(x => new { error = x }),
                    username
                });
            }

            environment.SetUser(user);
            environment.SetRedirect("/");
            return environment.GetCompleted();
        }
    }
}
