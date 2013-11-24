using System;
using Xunit;
using fn = qed.Functions;

#if DEBUG
namespace qed.UnitTests
{
    public class the_GetBuildDescription_function
    {
        [Fact]
        public void returns_description_for_a_build_not_started_with_no_queued_timestamp()
        {
            var build = new Build
            {
                Finished = null,
                Id = 42,
                Queued = null,
                Started = null,
                Succeeded = null
            };

            var actual = fn.GetBuildDescription(build);

            Assert.Equal(String.Format("Build #{0} queued.", build.Id), actual);
        }

        [Fact]
        public void returns_description_for_a_build_not_started_with_a_queued_timestamp()
        {
            var queued = DateTimeOffset.UtcNow;
            var build = new Build
            {
                Finished = null,
                Id = 42,
                Queued = queued,
                Started = null,
                Succeeded = null
            };

            var actual = fn.GetBuildDescription(build);

            Assert.Equal(String.Format("Build #{0} queued {1} seconds ago.", build.Id, queued.Since()), actual);
        }

        [Fact]
        public void returns_description_for_a_build_started_but_not_finished()
        {
            var now = DateTimeOffset.UtcNow;
            var build = new Build
            {
                Finished = null,
                Id = 42,
                Queued = DateTimeOffset.UtcNow,
                Started = now.AddSeconds(-42),
                Succeeded = null
            };

            var actual = fn.GetBuildDescription(build, false, now);

            Assert.Equal(String.Format("Build #{0} started {1} seconds ago.", build.Id, 42), actual);
        }

        [Fact]
        public void returns_description_for_a_finished_build_with_no_succeeded_bit()
        {
            var now = DateTimeOffset.UtcNow;
            var build = new Build
            {
                Finished = now.AddSeconds(-60),
                Id = 42,
                Queued = DateTimeOffset.UtcNow,
                Started = now.AddSeconds(-102),
                Succeeded = null
            };

            var actual = fn.GetBuildDescription(build, false, now);

            Assert.Equal(String.Format("Build #{0} finished in {1} seconds.", build.Id, 42), actual);
        }

        [Fact]
        public void returns_description_for_a_successful_finished()
        {
            var now = DateTimeOffset.UtcNow;
            var build = new Build
            {
                Finished = now.AddSeconds(-60),
                Id = 42,
                Queued = DateTimeOffset.UtcNow,
                Started = now.AddSeconds(-102),
                Succeeded = true
            };

            var actual = fn.GetBuildDescription(build, false, now);

            Assert.Equal(String.Format("Build #{0} succeeded in {1} seconds.", build.Id, 42), actual);
        }

        [Fact]
        public void returns_description_for_a_failed_finished()
        {
            var now = DateTimeOffset.UtcNow;
            var build = new Build
            {
                Finished = now.AddSeconds(-60),
                Id = 42,
                Queued = DateTimeOffset.UtcNow,
                Started = now.AddSeconds(-102),
                Succeeded = false
            };

            var actual = fn.GetBuildDescription(build, false, now);

            Assert.Equal(String.Format("Build #{0} failed in {1} seconds.", build.Id, 42), actual);
        }
    }
}
#endif
