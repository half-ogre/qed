using System.Collections.Generic;
using System.Threading.Tasks;

namespace qed
{
    public static partial class Functions
    {
        public static Task GetCompleted(this IDictionary<string, object> environment)
        {
            var completed = environment.Get<Task>(Constants.OwinExtensions.CompletedKey);

            if (completed != null)
                return completed;

            var tcs = new TaskCompletionSource<object>();
            tcs.SetResult(null);
            completed = tcs.Task;

            environment[Constants.OwinExtensions.CompletedKey] = completed;

            return completed;
        }
    }
}
