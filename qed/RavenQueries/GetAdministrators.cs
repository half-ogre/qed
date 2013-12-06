using System.Collections.Generic;
using System.Linq;

namespace qed
{
    public static partial class Functions
    {
        public static IList<User> GetAdministrators()
        {
            using (var ravenSession = OpenRavenSession())
            {
                return ravenSession.Query<User>()
                    .Where(user => user.IsAdministrator)
                    .ToList();
            }
        }
    }
}
