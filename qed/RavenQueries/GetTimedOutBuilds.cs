using System;
using System.Collections.Generic;
using System.Linq;
using Raven.Client.Linq;

namespace qed
{
    public static partial class Functions
    {
        public static IList<Build> GetTimedOutBuilds()
        {
            using (var ravenSession = OpenRavenSession())
            {
                return ravenSession.Query<Build>()
                    .Where(build =>
                        build.Started != null &&
                        build.Finished == null &&
                        build.Started < DateTimeOffset.UtcNow.AddMinutes(-5))
                    .ToList();
            }
        }
    }
}
