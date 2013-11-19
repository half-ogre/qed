using System.Collections.Generic;

namespace qed
{
    public static partial class Functions
    {
        public static string GetPath(this IDictionary<string, object> environment)
        {
            return environment.Get<string>(Constants.Owin.RequestPathKey);
        }
    }
}
