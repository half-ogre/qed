using System;
using System.Threading.Tasks;
using Octokit;
using Xunit;
using fn = qed.Functions;

#if DEBUG
namespace qed.UnitTests
{
    public class the_SetGitHubBuidStatus_function
    {
        readonly Func<string, string, BuildConfiguration> _fakeGetBuildConfiguration = (owner, name) => new BuildConfiguration();
        readonly Func<Build, string> _fakeGetBuildDescription = build => "aDescription";
        readonly Func<Task<string>> _fakeGetHost = () => Task.FromResult("aHost");
        
        [Fact]
        public async Task uses_the_build_configuration_token_to_create_the_GitHub_client()
        {
            var build = new Build();

            await fn.SetGitHubBuildStatus(
                build,
                CommitState.Success,
                (owner, name) => new BuildConfiguration { Token = "theToken" },
                _fakeGetBuildDescription,
                _fakeGetHost,
                (token, owner, name, sha, commitState, targetUrl, description) =>
                {
                    Assert.Equal("theToken", token);
                    return Task.FromResult((object)null);
                });
        }

        [Fact]
        public async Task passes_the_repository_owner_to_the_GitHub_API()
        {
            var build = new Build
            {
                RepositoryOwner = "theOwner"
            };

            await fn.SetGitHubBuildStatus(
                build,
                CommitState.Success,
                _fakeGetBuildConfiguration,
                _fakeGetBuildDescription,
                _fakeGetHost,
                (token, owner, name, sha, commitState, targetUrl, description) =>
                {
                    Assert.Equal("theOwner", owner);
                    return Task.FromResult((object)null);
                });
        }

        [Fact]
        public async Task passes_the_repository_name_to_the_GitHub_API()
        {
            var build = new Build
            {
                RepositoryName = "theName"
            };

            await fn.SetGitHubBuildStatus(
                build,
                CommitState.Success,
                _fakeGetBuildConfiguration,
                _fakeGetBuildDescription,
                _fakeGetHost,
                (token, owner, name, sha, commitState, targetUrl, description) =>
                {
                    Assert.Equal("theName", name);
                    return Task.FromResult((object)null);
                });
        }

        [Fact]
        public async Task passes_the_commit_SHA_to_the_GitHub_API()
        {
            var build = new Build
            {
                Revision = "theSHA"
            };

            await fn.SetGitHubBuildStatus(
                build,
                CommitState.Success,
                _fakeGetBuildConfiguration,
                _fakeGetBuildDescription,
                _fakeGetHost,
                (token, owner, name, sha, commitState, targetUrl, description) =>
                {
                    Assert.Equal("theSHA", sha);
                    return Task.FromResult((object)null);
                });
        }

        [Theory]
        [InlineData(CommitState.Error)]
        [InlineData(CommitState.Failure)]
        [InlineData(CommitState.Pending)]
        [InlineData(CommitState.Success)]
        public async Task passes_the_commit_state_to_the_GitHub_API(CommitState state)
        {
            var build = new Build();

            await fn.SetGitHubBuildStatus(
                build,
                state,
                _fakeGetBuildConfiguration,
                _fakeGetBuildDescription,
                _fakeGetHost,
                (token, owner, name, sha, commitState, targetUrl, description) =>
                {
                    Assert.Equal(state, commitState);
                    return Task.FromResult((object)null);
                });
        }

        [Fact]
        public async Task passes_the_target_URL_to_the_GitHub_API()
        {
            var build = new Build
            {
                Id = 42,
                RepositoryOwner = "theOwner",
                RepositoryName = "theName"
            };

            await fn.SetGitHubBuildStatus(
                build,
                CommitState.Success,
                _fakeGetBuildConfiguration,
                _fakeGetBuildDescription,
                () => Task.FromResult("theHost"),
                (token, owner, name, sha, commitState, targetUrl, description) =>
                {
                    Assert.Equal(new Uri("http://theHost/theOwner/theName/builds/42"), targetUrl);
                    return Task.FromResult((object)null);
                });
        }
    }
}
#endif
