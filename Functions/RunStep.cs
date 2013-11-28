using System;

namespace qed
{
    public static partial class Functions
    {
        public static bool RunStep(
            Func<bool> step,
            Action<string> logBuildMessage)
        {
            var success = false;

            try
            {
                success = step();
            }
            catch (Exception ex)
            {
                logBuildMessage(String.Format("ERROR: {0}", ex));
            }
            finally
            {
                logBuildMessage("Done");
                logBuildMessage(""); // this line intentionally left blank
            }

            return success;
        }
    }
}
