using System;
using System.Threading.Tasks;

namespace qed
{
    public static partial class Functions
    {
        public static Task<bool> GetHeadSha(
            Build build,
            string repositoryDirectory,
            Action<string> log)
        {
            if (build == null) throw new ArgumentNullException("build");
            if (String.IsNullOrEmpty(repositoryDirectory)) throw new ArgumentNullException("repositoryDirectory");
            if (log == null) throw new ArgumentNullException("log");

            log("STEP: Getting HEAD's SHA.");

            return RunStep(() =>
            {
                if (!string.IsNullOrEmpty(build.Revision))
                {
                    log(String.Format("Build #{0} already has a SHA. Skipping.", build.Id));
                    return true;
                }

                var repo = (LibGit2Sharp.Repository)null;
                try
                {
                    repo = new LibGit2Sharp.Repository(repositoryDirectory);
                }
                catch (Exception ex)
                {
                    log(String.Format("ERROR: Could not get repository at {0} via LibGit2Sharp. Exception: ", repositoryDirectory));
                    log(ex.ToString());
                    return false;
                }

                var head = (LibGit2Sharp.Reference)null;
                try
                {
                    head = repo.Refs[build.Ref];
                }
                catch (Exception ex)
                {
                    log(String.Format("ERROR: Could not get HEAD for ref {0}. Exception: ", build.Ref));
                    log(ex.ToString());
                    return false;
                }

                log(String.Format("Setting SHA to {0}.", head.TargetIdentifier));
                SetBuildRevision(build, head.TargetIdentifier);

                return true;

            }, log);
        }
    }
}
