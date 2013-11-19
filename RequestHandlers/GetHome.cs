using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using fn = qed.Functions;

namespace qed
{
    public static partial class Handlers
    {
        public static Task GetHome(
            IDictionary<string, object> environment,
            Func<IDictionary<string, object>, Task> next)
        {
            var buildConfigurations = fn.GetBuildConfigurations();
            
            return environment.Render("home", new { buildConfigurations });
        }
    }
}
