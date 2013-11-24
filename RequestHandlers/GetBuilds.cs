using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using fn = qed.Functions;

namespace qed
{
    public static partial class Handlers
    {
        public static async Task GetBuilds(
            IDictionary<string, object> environment,
            dynamic @params,
            Func<IDictionary<string, object>, Task> next)
        {
            var owner = @params.owner as string;
            var name = @params.name as string;

            var buildConfiguration = fn.GetBuildConfiguration(owner, name);
            if (buildConfiguration == null)
            {
                environment.SetStatusCode(400);
                return;
            }

            var builds = fn.GetBuilds(buildConfiguration.Owner, buildConfiguration.Name)
                .Reverse()
                .Select(build => new {
                    id = build.Id,
                    description = fn.GetBuildDescription(build, true),
                    status = GetBuildStatus(build)
                });
            
            await environment.Render("builds", new
            {
                owner = buildConfiguration.Owner, 
                name = buildConfiguration.Name,  
                builds
            });
        }

        static string GetBuildStatus(Build build)
        {
            if (build.Started == null)
                return "queued";
            if (build.Finished == null)
                return "building";
            if (build.Succeeded.HasValue && build.Succeeded.Value)
                return "succeeded";
            return "failed";
        }
    }
}
