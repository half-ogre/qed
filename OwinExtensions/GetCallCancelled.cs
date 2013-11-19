using System.Collections.Generic;
using System.Threading;

namespace qed
{
    public static partial class Functions
    {
        public static CancellationToken GetCallCancelled(this IDictionary<string, object> environment)
        {
            return environment.Get<CancellationToken>(Constants.Owin.CallCancelledKey);
        }
    }
}
