using System;

namespace qed
{
    public static partial class Functions
    {
        static readonly object semaphore = new object();

        public static void AppendBuildOutput(
            Build build,
            string output)
        {
            using (var ravenSession = OpenRavenSession())
            {
                lock (semaphore)
                {
                    build.Ouput += output + Environment.NewLine;

                    ravenSession.Store(build);
                    ravenSession.SaveChanges();
                }
            }
        }
    }
}
