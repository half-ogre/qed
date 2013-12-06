using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using fn = qed.Functions;

namespace qed
{
    public static partial class Handlers
    {
        public static Task GetUsers(
            IDictionary<string, object> environment,
            Func<IDictionary<string, object>, Task> next)
        {
            var users = fn.GetUsers();

            return environment.Render("users", new
            {
                users = users.Select(user => new
                {
                    administrator = user.IsAdministrator ? " (Administrator)" : null,
                    isMe = user.Username == environment.GetUser().Username,
                    username = user.Username
                })
            });
        }
    }
}
