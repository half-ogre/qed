using Xunit;
using fn = qed.Functions;

#if DEBUG
namespace qed.UnitTests
{
    public class the_CompareToPasswordHash_function
    {
        [Fact]
        public void returns_true_when_password_matches_hash()
        {
            var passwordHash = fn.GeneratePasswordHash("a-password");

            var actual = "a-password".CompareToPasswordHash(passwordHash);

            Assert.True(actual);
        }

        [Fact]
        public void returns_false_when_password_does_not_match_hash()
        {
            var passwordHash = fn.GeneratePasswordHash("a-password");

            var actual = "a-different-password".CompareToPasswordHash(passwordHash);

            Assert.False(actual);
        }
    }
}
#endif
