using System;

namespace qed
{
    public static partial class Functions
    {
        public static object To<T>(
            this T @object,
            Func<T, object> to)
            where T : class
        {
            if (@object == null)
                return null;

            return to(@object);
        }
    }
}
