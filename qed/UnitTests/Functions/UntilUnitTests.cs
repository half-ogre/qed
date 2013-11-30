using System;
using Xunit;
using fn = qed.Functions;

#if DEBUG
namespace qed.UnitTests
{
    public class the_Until_function
    {
        [Fact]
        public void returns_the_seconds_between_first_and_last()
        {
            var first = DateTimeOffset.UtcNow;
            var last = first.AddSeconds(42);

            var actual = first.Until(last);

            Assert.Equal(42, actual);
        }
    }
}
#endif
