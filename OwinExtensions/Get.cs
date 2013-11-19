using System.Collections.Generic;

namespace qed
{
    public static partial class Functions
    {
        public static T Get<T>(
            this IDictionary<string, object> environment, 
            string key)
        {
            object value;
            return environment.TryGetValue(key, out value) ? (T)value : default(T);
        }
    }
}
