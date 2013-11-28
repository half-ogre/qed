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
        public static Task PostPullRequestEvent(
            IDictionary<string, object> environment,
            Func<IDictionary<string, object>, Task> next)
        {
            var fail = new Func<int, string, Task>((statusCode, message) =>
            {
                environment.SetStatusCode(statusCode);
                return environment.WriteAsync(message);
            });

            var headers = environment.GetRequestHeaders();

            if (!headers.ContainsKey("X-GitHub-Event"))
            {
                fail(400, "Missing X-GitHub-Event header.");
                return environment.GetCompleted();
            }

            var eventTypes = headers["X-GitHub-Event"];
            if (eventTypes != null)
            {
                var eventType = eventTypes.FirstOrDefault();

                if (eventType == null || !eventType.Equals("pull_request", StringComparison.OrdinalIgnoreCase))
                {
                    fail(422, String.Concat("Unexpected X-GitHub-Event: ", eventType, "."));
                    return environment.GetCompleted();
                }
            }

            var form = environment.ReadFormAsync();

            var payload = form["payload"];

            if (payload == null || payload.Count == 0)
            {
                fail(400, "Missing payload.");
                return environment.GetCompleted();
            }

            var @event = payload[0];

            var prEvent = JsonConvert.DeserializeObject<PullRequestEvent>(@event);

            var buildConfigurations = fn.GetBuildConfigurations();

            var buildConfiguration = buildConfigurations.FirstOrDefault(bc =>
                bc.Name.Equals(prEvent.Repository.Name, StringComparison.OrdinalIgnoreCase) &&
                bc.Owner.Equals(prEvent.Repository.Owner.Login, StringComparison.OrdinalIgnoreCase));

            if (buildConfiguration == null)
            {
                environment.SetStatusCode(204);
                return environment.GetCompleted();
            }

            fn.CreateBuild(
                buildConfiguration.Command,
                buildConfiguration.CommandArguments,
                prEvent.Repository.Name,
                prEvent.Repository.Owner.Login,
                String.Concat("refs/pull/", prEvent.Number, "/head"),
                prEvent.PullRequest.Head.Sha,
                "pull_request",
                @event);

            environment.SetStatusCode(204);
            return environment.GetCompleted();
        }
    }
}
