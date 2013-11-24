using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using fn = qed.Functions;

namespace qed
{
    public static partial class Handlers
    {
        public static async Task GetBuild(
            IDictionary<string, object> environment,
            dynamic @params,
            Func<IDictionary<string, object>, Task> next)
        {
            var owner = @params.owner as string;
            var name = @params.name as string;
            var id = int.Parse(@params.id as string);

            var buildConfiguration = fn.GetBuildConfiguration(owner, name);
            if (buildConfiguration == null)
            {
                environment.SetStatusCode(400);
                return;
            }

            var build = fn.GetBuild(id);
            
            await environment.Render("build", new
            {
                id = build.Id,
                name = build.RepositoryName,
                owner = build.RepositoryOwner,
                sha = build.Revision.Substring(0, 7),
                failed = !build.Succeeded,
                output = fn.Redact(build.Ouput)
            });
        }
    }
}
