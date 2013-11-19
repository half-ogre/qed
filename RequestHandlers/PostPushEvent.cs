using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using fn = qed.Functions;

namespace qed
{
    public static partial class Handlers
    {
        public static async Task PostPushEvent(
            IDictionary<string, object> environment,
            Func<IDictionary<string, object>, Task> next)
        {
            var fail = new Func<int, string, Task>(async (statusCode, message) =>
            {
                environment.SetStatusCode(statusCode);
                await environment.WriteAsync(message);
            });

            var headers = environment.GetRequestHeaders();

            if (!headers.ContainsKey("X-GitHub-Event"))
            {
                await fail(400, "Missing X-GitHub-Event header.");
                return;
            }

            var eventTypes = headers["X-GitHub-Event"];
            if (eventTypes != null)
            {
                var eventType = eventTypes.FirstOrDefault();

                if (eventType == null || !eventType.Equals("push", StringComparison.OrdinalIgnoreCase))
                {
                    await fail(422, String.Concat("Unexpected X-GitHub-Event: ", eventType, "."));
                    return;
                }
            }

            var form = await environment.ReadFormAsync();

            var payload = form["payload"];

            if (payload == null || payload.Count == 0)
            {
                await fail(400, "Missing payload.");
                return;
            }

            var @event = payload[0];

            var pushEvent = JsonConvert.DeserializeObject<PushEvent>(@event);

            // When a branch is deleted, GitHub sends a push event where after is 0000000. 
            // There's no need to do anything in this case.
            if (pushEvent.After.StartsWith("0000000"))
            {
                environment.SetStatusCode(204);
                return;
            }

            var buildConfigurations = fn.GetBuildConfigurations();

            var buildConfiguration = buildConfigurations.FirstOrDefault(bc =>
                bc.Name.Equals(pushEvent.Repository.Name, StringComparison.OrdinalIgnoreCase) &&
                bc.Owner.Equals(pushEvent.Repository.Owner.Name, StringComparison.OrdinalIgnoreCase));

            if (buildConfiguration == null)
            {
                environment.SetStatusCode(204);
                return;
            }

            fn.CreateBuild(
                buildConfiguration.Command,
                buildConfiguration.CommandArguments,
                pushEvent.Repository.Name,
                pushEvent.Repository.Owner.Name,
                pushEvent.Repository.Url,
                pushEvent.Ref,
                pushEvent.After,
                "push",
                @event);

            environment.SetStatusCode(204);
        }
    }
}
