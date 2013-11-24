using System;
using Xunit;
using fn = qed.Functions;

#if DEBUG
namespace qed.UnitTests
{
    public class the_CreateFetchRefspec_function
    {
        [Fact]
        public void creates_a_refspec_for_the_master_branch()
        {
            var actual = fn.CreateFetchRefspec("refs/heads/master");

            Assert.Equal("+refs/heads/master:refs/remotes/origin/master", actual);
        }

        [Fact]
        public void creates_a_refspec_for_the_a_branch_with_a_slash()
        {
            var actual = fn.CreateFetchRefspec("refs/heads/half-ogre/1-a-description");

            Assert.Equal("+refs/heads/half-ogre/1-a-description:refs/remotes/origin/half-ogre/1-a-description", actual);
        }

        [Fact]
        public void creates_a_refspec_for_a_pull_request()
        {
            var actual = fn.CreateFetchRefspec("refs/pull/42/head");

            Assert.Equal("+refs/pull/42/head:refs/remotes/origin/pr/42", actual);
        }

        [Fact]
        public void throws_for_an_unexpected_type_of_ref()
        {
            Assert.Throws<ArgumentException>(() => fn.CreateFetchRefspec("refs/an-unepected-thing"));
        }
    }
}
#endif