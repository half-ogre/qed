using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using fn = qed.Functions;
using OwinExtensions;

namespace qed
{
    public static partial class Handlers
    {
        public static Task PostForceEvent(
            IDictionary<string, object> environment,
            Func<IDictionary<string, object>, Task> next)
        {
            var fail = new Func<int, string, Task>((statusCode, message) =>
            {
                environment.SetStatusCode(statusCode);
                return environment.WriteAsync(message);
            });

            var form = environment.ReadForm();

            var payload = form["payload"];

            if (payload == null || payload.Count == 0)
            {
                fail(400, "Missing payload.");
                return environment.GetCompleted();
            }

            var @event = payload[0];

            var forceEvent = JsonConvert.DeserializeObject<ForceEvent>(@event);

            // TODO: validate the event's properties

            var buildConfiguration = fn.GetBuildConfiguration(forceEvent.Repository.Owner.Name, forceEvent.Repository.Name);
            if (buildConfiguration == null)
            {
                fail(422, "No build configuraion matches the identified build.");
                return environment.GetCompleted();
            }

            var newBuild = fn.CreateBuild(
                buildConfiguration.Command,
                buildConfiguration.CommandArguments,
                forceEvent.Repository.Name,
                forceEvent.Repository.Owner.Name,
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
                fn.GetHost(),
                location);

            environment.SetStatusCode(201);
            var responseHeaders = environment.GetResponseHeaders();
            responseHeaders.Add("Location", new [] { absoluteLocation });
            return environment.Render("queued", new { id = newBuild.Id, location});
        }
    }
}
