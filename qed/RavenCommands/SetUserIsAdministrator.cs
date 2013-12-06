namespace qed
{
    public static partial class Functions
    {
        public static void SetUserIsAdministrator(
            User user,
            bool isAdministrator)
        {
            using (var ravenSession = OpenRavenSession())
            {
                user.IsAdministrator = isAdministrator;

                ravenSession.Store(user);
                ravenSession.SaveChanges();
            }
        }
    }
}
