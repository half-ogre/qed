namespace qed
{
    public static partial class Functions
    {
        public static void SetBuildRevision(
            Build build,
            string revision)
        {
            using (var ravenSession = OpenRavenSession())
            {
                build.Revision = revision;

                ravenSession.Store(build);
                ravenSession.SaveChanges();
            }
        }
    }
}
