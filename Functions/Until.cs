using System;

namespace qed
{
    public static partial class Functions
    {
        public static double Until(this DateTimeOffset first, DateTimeOffset second)
        {
            return Math.Round((second - first).TotalSeconds);
        }
    }
}
