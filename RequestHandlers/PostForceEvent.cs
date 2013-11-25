using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using fn = qed.Functions;

namespace qed
{
    public static partial class Handlers
    {
        public static async Task PostForceEvent(
            IDictionary<string, object> environment,
            Func<IDictionary<string, object>, Task> next)
        {
            var fail = new Func<int, string, Task>(async (statusCode, message) =>
            {
                environment.SetStatusCode(statusCode);
                await environment.WriteAsync(message);
            });

            var form = await environment.ReadFormAsync();

            var payload = form["payload"];

            if (payload == null || payload.Count == 0)
            {
                await fail(400, "Missing payload.");
                return;
            }

            var @event = payload[0];

            var forceEvent = JsonConvert.DeserializeObject<ForceEvent>(@event);

            // TODO: validate the event's properties

            var buildConfiguration = fn.GetBuildConfiguration(forceEvent.Repository.Owner.Name, forceEvent.Repository.Name);
            if (buildConfiguration == null)
            {
                await fail(422, "No build configuraion matches the identified build.");
                return;
            }

            var newBuild = fn.CreateBuild(
                buildConfiguration.Command,
                buildConfiguration.CommandArguments,
                forceEvent.Repository.Name,
                forceEvent.Repository.Owner.Name,
                null,
                forceEvent.Ref,
                forceEvent.Revision,
                "rebuild",
                @event);

            var location = String.Format(
                "/{0}/{1}/builds/{2}",
                newBuild.RepositoryOwner,
                newBuild.RepositoryName,
                newBuild.Id);

            var absoluteLocation = String.Format(
                "http://{0}{1}",
                await fn.GetHost(),
                location);

            environment.SetStatusCode(201);
            var responseHeaders = environment.GetResponseHeaders();
            responseHeaders.Add("Location", new [] { absoluteLocation });
            await environment.Render("force", new { id = newBuild.Id, location});
        }
    }
}
