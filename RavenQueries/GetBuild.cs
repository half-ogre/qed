using System;

namespace qed
{
    public static partial class Functions
    {
        public static Build GetBuild(Guid id)
        {
            using (var ravenSession = OpenRavenSession())
            {
                return ravenSession.Load<Build>(id);
            }
        }
    }
}
