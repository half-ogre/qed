using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using fn = qed.Functions;

namespace qed
{
    public static partial class Handlers
    {
        public static async Task PostRebuildEvent(
            IDictionary<string, object> environment,
            Func<IDictionary<string, object>, Task> next)
        {
            var fail = new Func<int, string, Task>(async (statusCode, message) =>
            {
                environment.SetStatusCode(statusCode);
                await environment.WriteAsync(message);
            });

            var form = await environment.ReadFormAsync();

            var buildIdField = form["build_id"];

            if (buildIdField == null || buildIdField.Count == 0)
            {
                await fail(400, "Entity is missing build identifier.");
                return;
            }

            Guid buildId;
            if (!Guid.TryParse(buildIdField[0], out buildId))
            {
                await fail(422, "Build identifier is not a valid GUID.");
                return;
            }

            var build = fn.GetBuild(buildId);
            if (build == null)
            {
                environment.SetStatusCode(404);
                return;
            }

            var buildConfiguration = fn.GetBuildConfiguration(build.RepositoryOwner, build.RepositoryName);
            if (buildConfiguration == null)
            {
                await fail(422, "No build configuraion matches the identified build.");
                return;
            }

            var newBuild = fn.CreateBuild(
                buildConfiguration.Command,
                buildConfiguration.CommandArguments,
                build.RepositoryName,
                build.RepositoryOwner,
                build.RepositoryUrl,
                build.Ref,
                build.Revision,
                "rebuild",
                buildId.ToString());

            var location = String.Format(
                "http://{0}/{1}/{2}/builds/{3}",
                await fn.GetHost(),
                newBuild.RepositoryOwner,
                newBuild.RepositoryName,
                newBuild.Id);
            
            environment.SetStatusCode(201);
            var responseHeaders = environment.GetResponseHeaders();
            responseHeaders.Add("Location", new [] { location });
            await environment.Render("rebuild", new { id = newBuild.Id, location});
        }
    }
}
