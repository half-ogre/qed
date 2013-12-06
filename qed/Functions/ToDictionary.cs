using System.Collections.Generic;
using System.Linq;

namespace qed
{
    public static partial class Functions
    {
        public static IDictionary<string, object> ToDictionary(this object @object)
        {
            if (@object == null)
                return new Dictionary<string, object>();

            return @object.GetType()
                .GetProperties()
                .Select(x => new KeyValuePair<string, object>(x.Name, x.GetValue(@object, null)))
                .ToDictionary(x => x.Key, x => x.Value);
        }
    }
}
