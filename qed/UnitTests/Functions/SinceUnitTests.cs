using System;
using Xunit;
using fn = qed.Functions;

#if DEBUG
namespace qed.UnitTests
{
    public class the_Since_function
    {
        [Fact]
        public void returns_the_seconds_since_now()
        {
            var now = DateTimeOffset.UtcNow;
            var actual = now.AddSeconds(-42).Since(now);

            Assert.Equal(42, actual);
        }
    }
}
#endif
