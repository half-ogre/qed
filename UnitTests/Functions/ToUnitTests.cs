using Xunit;
using fn = qed.Functions;

#if DEBUG
namespace qed.UnitTests
{
    public class the_To_function
    {
        [Fact]
        public void returns_null_when_object_is_bull()
        {
            var @object = (object)null;
            var actual = @object.To(o => new object());

            Assert.Null(@actual);
        }

        [Fact]
        public void returns_the_object_from_the_func_when_object_is_not_null()
        {
            var @object = new object();
            var actual = @object.To(o => 42);

            Assert.Equal(42, @actual);
        }
    }
}
#endif
