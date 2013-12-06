using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using OwinExtensions;
using fn = qed.Functions;

namespace qed
{
    public static partial class Handlers
    {
        public static Task PostUsers(
            IDictionary<string, object> environment,
            Func<IDictionary<string, object>, Task> next)
        {
            var usernameRegex = new Regex("^[a-z][a-z0-9_-]+[a-z0-9]$", RegexOptions.IgnoreCase);
            
            var form = environment.ReadForm();

            var username = form["username"].FirstOrDefault();
            var password = form["password"].FirstOrDefault();
            var emailAddress = form["email-address"].FirstOrDefault();

            var errors = new List<string>();

            if (String.IsNullOrEmpty(username))
                errors.Add("Username is required.");
            else
            {
                if (!usernameRegex.IsMatch(username))
                    errors.Add("Username is invalid; must match regular expression '^[a-z][a-z0-9_-]+[a-z0-9]$'.");

                var exustingUser = fn.GetUserByUsername(username);
                if (exustingUser != null)
                    errors.Add("Username is not available; try another.");   
            }

            if (String.IsNullOrEmpty(password))
                errors.Add("Password is required.");

            if (errors.Count > 0)
            {
                environment.SetStatusCode(400);

                return environment.Render("sign-up-form", new
                {
                    emailAddress,
                    errors = errors.Select(x => new { error = x }),
                    username
                });
            }
            
            var user = fn.CreateUser(username, password, emailAddress);

            var usersCount = fn.GetUsersCount();
            if (usersCount == 0)
            {
                fn.SetUserIsAdministrator(user, true);
            }

            environment.SetRedirect("/");

            return environment.GetCompleted();
        }
    }
}
