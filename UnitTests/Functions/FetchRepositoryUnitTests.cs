using System;
using System.Collections.Generic;
using System.Diagnostics;
using Xunit;
using fn = qed.Functions;

#if DEBUG
namespace qed.UnitTests
{
    public class the_FetchRepository_function
    {
        readonly Build _fakeBuild = new Build();
        Func<string, string, string, Process> _fakeCreateProcess = (command, arguments, workingDirectory) => new Process();
        Func<string, string> _fakeCreateFetchRefspec = @ref => "a-refspec";
        Action<string> _fakeLog = message => { };
        Func<Process, Action, Action<string>, int> _fakeRunProcess = (process, onFinished, log) => 0;
            
        [Fact]
        public void creates_a_git_fetch_process()
        {
            string actualCommand = null;
            string actualArguments = null;
            string actualWorkingDirectory = null;
            _fakeCreateProcess = (command, arguments, workingDirectory) =>
            {
                actualCommand = command;
                actualArguments = arguments;
                actualWorkingDirectory = workingDirectory;
                return new Process();
            };
            _fakeCreateFetchRefspec = @ref => "the-refspec";

            fn.FetchRepository(
                _fakeBuild, 
                "the-repository-directory", 
                _fakeLog, 
                _fakeCreateProcess, 
                _fakeCreateFetchRefspec, 
                _fakeRunProcess);

            Assert.Equal("git.exe", actualCommand);
            Assert.Equal("fetch origin the-refspec", actualArguments);
            Assert.Equal("the-repository-directory", actualWorkingDirectory);
        }

        [Fact]
        public void runs_the_created_process()
        {
            Process actualProcess = null;
            var expectedProcess = new Process();
            _fakeCreateProcess = (command, arguments, workingDirectory) => expectedProcess;
            _fakeRunProcess = (process, onFinished, log) =>
            {
                actualProcess = process;
                return 0;
            };

            fn.FetchRepository(
                _fakeBuild,
                "a-repository-directory",
                _fakeLog,
                _fakeCreateProcess,
                _fakeCreateFetchRefspec,
                _fakeRunProcess);

            Assert.Same(expectedProcess, actualProcess);
        }

        [Fact]
        public void returns_true_when_the_exit_code_from_running_the_process_is_zero()
        {
            _fakeRunProcess = (process, onFinished, log) => 0;

            var actual = fn.FetchRepository(
                _fakeBuild,
                "a-repository-directory",
                _fakeLog,
                _fakeCreateProcess,
                _fakeCreateFetchRefspec,
                _fakeRunProcess);

            Assert.True(actual);
        }

        [Fact]
        public void returns_false_when_the_exit_code_from_running_the_process_is_not_zero()
        {
            _fakeRunProcess = (process, onFinished, log) => 42;

            var actual = fn.FetchRepository(
                _fakeBuild, 
                "a-repository-directory", 
                _fakeLog, 
                _fakeCreateProcess, 
                _fakeCreateFetchRefspec, 
                _fakeRunProcess);

            Assert.False(actual);
        }

        [Fact]
        public void logs_a_done_message_when_exit_code_is_zero()
        {
            var actualLog = new List<string>();
            _fakeLog = actualLog.Add;

            fn.FetchRepository(
                _fakeBuild,
                "a-repository-directory",
                _fakeLog,
                _fakeCreateProcess,
                _fakeCreateFetchRefspec,
                _fakeRunProcess);

            Assert.Equal("Done", actualLog[1]);
        }

        [Fact]
        public void logs_a_done_message_when_exit_code_is_not_zero()
        {
            var actualLog = new List<string>();
            _fakeLog = actualLog.Add;
            _fakeRunProcess = (process, onFinished, log) => 42;

            fn.FetchRepository(
                _fakeBuild,
                "a-repository-directory",
                _fakeLog,
                _fakeCreateProcess,
                _fakeCreateFetchRefspec,
                _fakeRunProcess);

            Assert.Equal("Done", actualLog[1]);
        }

        [Fact]
        public void logs_a_message_for_starting_the_build_step()
        {
            var actualLog = new List<string>();
            _fakeLog = actualLog.Add;

            fn.FetchRepository(
                _fakeBuild,
                "a-repository-directory",
                _fakeLog,
                _fakeCreateProcess,
                _fakeCreateFetchRefspec,
                _fakeRunProcess);

            Assert.Equal("STEP: Fetching repository.", actualLog[0]);
        }

        [Theory]
        [InlineData(0)]t
        [InlineData(42)]
        public void logs_a_blank_line_at_the_end(int exitCode)
        {
            var actualLog = new List<string>();
            _fakeLog = actualLog.Add;
            _fakeRunProcess = (process, onFinished, log) => exitCode;

            fn.FetchRepository(
                _fakeBuild,
                "a-repository-directory",
                _fakeLog,
                _fakeCreateProcess,
                _fakeCreateFetchRefspec,
                _fakeRunProcess);

            Assert.Equal("", actualLog[2]);
        }
    }
}
#endif