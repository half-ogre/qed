using System;
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
        readonly Func<string> _fakeGetHost = () => "aHost";
        
        [Fact]
        public void uses_the_build_configuration_token_to_create_the_GitHub_client()
        {
            var build = new Build();

            fn.SetGitHubBuildStatus(
                build,
                CommitState.Success,
                (owner, name) => new BuildConfiguration { Token = "theToken" },
                _fakeGetBuildDescription,
                _fakeGetHost,
                (token, owner, name, sha, commitState, targetUrl, description) => 
                    Assert.Equal("theToken", token));
        }

        [Fact]
        public void passes_the_repository_owner_to_the_GitHub_API()
        {
            var build = new Build
            {
                RepositoryOwner = "theOwner"
            };

            fn.SetGitHubBuildStatus(
                build,
                CommitState.Success,
                _fakeGetBuildConfiguration,
                _fakeGetBuildDescription,
                _fakeGetHost,
                (token, owner, name, sha, commitState, targetUrl, description) => 
                    Assert.Equal("theOwner", owner));
        }

        [Fact]
        public void passes_the_repository_name_to_the_GitHub_API()
        {
            var build = new Build
            {
                RepositoryName = "theName"
            };

            fn.SetGitHubBuildStatus(
                build,
                CommitState.Success,
                _fakeGetBuildConfiguration,
                _fakeGetBuildDescription,
                _fakeGetHost,
                (token, owner, name, sha, commitState, targetUrl, description) => 
                    Assert.Equal("theName", name));
        }

        [Fact]
        public void passes_the_commit_SHA_to_the_GitHub_API()
        {
            var build = new Build
            {
                Revision = "theSHA"
            };

            fn.SetGitHubBuildStatus(
                build,
                CommitState.Success,
                _fakeGetBuildConfiguration,
                _fakeGetBuildDescription,
                _fakeGetHost,
                (token, owner, name, sha, commitState, targetUrl, description) => 
                    Assert.Equal("theSHA", sha));
        }

        [Theory]
        [InlineData(CommitState.Error)]
        [InlineData(CommitState.Failure)]
        [InlineData(CommitState.Pending)]
        [InlineData(CommitState.Success)]
        public void passes_the_commit_state_to_the_GitHub_API(CommitState state)
        {
            var build = new Build();

            fn.SetGitHubBuildStatus(
                build,
                state,
                _fakeGetBuildConfiguration,
                _fakeGetBuildDescription,
                _fakeGetHost,
                (token, owner, name, sha, commitState, targetUrl, description) => 
                    Assert.Equal(state, commitState));
        }

        [Fact]
        public void passes_the_target_URL_to_the_GitHub_API()
        {
            var build = new Build
            {
                Id = 42,
                RepositoryOwner = "theOwner",
                RepositoryName = "theName"
            };

            fn.SetGitHubBuildStatus(
                build,
                CommitState.Success,
                _fakeGetBuildConfiguration,
                _fakeGetBuildDescription,
                () => "theHost",
                (token, owner, name, sha, commitState, targetUrl, description) => 
                    Assert.Equal(new Uri("http://theHost/theOwner/theName/builds/42"), targetUrl));
        }

        [Fact]
        public void passes_the_build_description_to_the_GitHub_API()
        {
            var build = new Build();

            fn.SetGitHubBuildStatus(
                build,
                CommitState.Success,
                _fakeGetBuildConfiguration,
                bld => "theDescription",
                _fakeGetHost,
                (token, owner, name, sha, commitState, targetUrl, description) => 
                    Assert.Equal("theDescription", description));
        }
    }
}
#endif
