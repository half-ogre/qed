using System.Collections.Concurrent;
using System.Collections.Generic;

namespace qed
{
    public static partial class Functions
    {
        static readonly IDictionary<string, object> _configuration = new ConcurrentDictionary<string, object>();

        public static void SetConfiguration(string key, object value)
        {
            _configuration[key] = value;
        }
    }
}
