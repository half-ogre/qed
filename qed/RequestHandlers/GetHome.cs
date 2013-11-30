using System;
using System.Collections.Generic;
using System.Linq;
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
            var configuredRepos = buildConfigurations
                .Select(bc =>
                    new
                    {
                        owner = bc.Owner,
                        name = bc.Name,
                        lastBuild = fn.GetLastFinishedBuild(bc.Owner, bc.Name).To(build => new
                        {
                            id = build.Id,
                            description = fn.GetBuildDescription(build),
                            status = GetBuildStatus(build)
                        }),
                        currentBuild = fn.GetCurrentBuild(bc.Owner, bc.Name).To(build => new
                        {
                            id = build.Id,
                            description = fn.GetBuildDescription(build),
                            status = GetBuildStatus(build)
                        }),
                        nextBuild = fn.GetNextBuild(bc.Owner, bc.Name).To(build => new
                        {
                            id = build.Id,
                            description = fn.GetBuildDescription(build),
                            status = GetBuildStatus(build)
                        })
                    })
                .ToList();
            
            return environment.Render("home", new { configuredRepos });
        }
    }
}
