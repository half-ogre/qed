using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using fn = qed.Functions;

namespace qed
{
    public static partial class Handlers
    {
        public static Task GetBuild(
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
                return environment.GetCompleted();
            }

            var build = fn.GetBuild(id);

            var sha = (string)null;
            if (build.Revision != null)
                sha = build.Revision.Substring(0, 7);
            
            return environment.Render("build", new
            {
                id = build.Id,
                description = fn.GetBuildDescription(build, includeRefDescription: true),
                name = build.RepositoryName,
                owner = build.RepositoryOwner,
                @ref = build.Ref,
                refDescription = fn.GetRefDescription(build),
                revision = build.Revision,
                sha,
                failed = !build.Succeeded,
                output = fn.Redact(build.Ouput),
                status = GetBuildStatus(build)
            });
        }
    }
}
