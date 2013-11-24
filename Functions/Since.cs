using System;

namespace qed
{
    public static partial class Functions
    {
        public static double Since(this DateTimeOffset timestamp)
        {
            return Since(timestamp, DateTimeOffset.UtcNow);
        }

        internal static double Since(this DateTimeOffset timestamp, DateTimeOffset now)
        {
            return Math.Round((now - timestamp).TotalSeconds);
        }
    }
}
