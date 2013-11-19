using System;

namespace qed
{
    public static partial class Functions
    {
        public static void AppendBuildOutput(
            Build build,
            string output)
        {
            using (var ravenSession = OpenRavenSession())
            {
                build.Ouput += output + Environment.NewLine;

                ravenSession.Store(build);
                ravenSession.SaveChanges();
            }
        }
    }
}
