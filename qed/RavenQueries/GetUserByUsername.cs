using System.Linq;

namespace qed
{
    public static partial class Functions
    {
        public static User GetUserByUsername(string username)
        {
            using (var ravenSession = OpenRavenSession())
            {
                return ravenSession.Query<User>()
                    .FirstOrDefault(user => user.Username == username);
            }
        }
    }
}
