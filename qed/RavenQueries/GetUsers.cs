using System.Collections.Generic;
using System.Linq;

namespace qed
{
    public static partial class Functions
    {
        public static IList<User> GetUsers()
        {
            using (var ravenSession = OpenRavenSession())
            {
                return ravenSession.Query<User>()
                    .ToList();
            }
        }
    }
}
