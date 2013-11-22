using System;
using System.Threading.Tasks;

namespace qed
{
    public static partial class Functions
    {
        public static Task<bool> RunStep(
            Func<bool> step,
            Action<string> logBuildMessage)
        {
            return RunStep(() => Task.FromResult(step()), logBuildMessage);
        }

        public static async Task<bool> RunStep(
            Func<Task<bool>> step,
            Action<string> logBuildMessage)
        {
            var success = false;

            try
            {
                success = await step();
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
