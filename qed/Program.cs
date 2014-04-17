using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LibGit2Sharp;
using Microsoft.Owin.Builder;
using Mono.Options;
using Nowin;
using fn = qed.Functions;

namespace qed
{
    using AppFunc = Func<IDictionary<string, object>, Task>;

    class Program
    {
        static void EnsureAdmiministrator()
        {
            if (fn.GetAdministrators().Any())
                return;

            Console.BackgroundColor = ConsoleColor.Red;
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("There are no administrator users.");
            Console.ResetColor();
            Console.WriteLine("You must create an administrator user to start QED.");
            Console.Write("Enter a username: ");
            var username = Console.ReadLine();
            if (String.IsNullOrEmpty(username) || !Constants.UsernameRegex.IsMatch(username))
            {
                Console.WriteLine("Invalid username (must have just letters and numbers). Exiting.");
                Environment.Exit(1);
            }
            Console.Write("Enter a password: ");
            var password = ReadPassword();
            if (String.IsNullOrEmpty(password) || password.Length < 8)
            {
                Console.WriteLine("Invalid password (must be at least 8 characters). Exiting.");
                Environment.Exit(1);
            }

            fn.CreateUser(username, password, null);

            Console.WriteLine("Created administrator.");
        }

        static void ShowHelp(OptionSet options)
        {
            Console.WriteLine("Usage: qed.exe [options]");
            Console.WriteLine();
            Console.WriteLine("Options:");
            options.WriteOptionDescriptions(Console.Out);
        }

        static string GetBuildConfigurationPath()
        {
            var baseDirectory = fn.GetBaseDirectory();
            var buildConfigurationsPath = Path.Combine(baseDirectory, "build.config");

            if (File.Exists(buildConfigurationsPath)) 
                return buildConfigurationsPath;

            var rootBuildConfigurationsPath = Path.Combine(baseDirectory, @"..\..\..\", "Build.config");

            buildConfigurationsPath = File.Exists(rootBuildConfigurationsPath) ? rootBuildConfigurationsPath : "";

            return buildConfigurationsPath;
        }

        static void Main(string[] args)
        {
            ReadOptions(args);

            EnsureAdmiministrator();

            StartWebServer();

            RunBuilds();
        }

        static void ReadOptions(string[] args)
        {
            var showHelp = false;

            SetDefaultConfig();

            var options = new OptionSet
            {
                {"buildconfig=", "Path for the Build config file", b => fn.SetConfiguration(Constants.Configuration.BuildConfigurationLocationKey, b)},
                {"host=", h => fn.SetConfiguration(Constants.Configuration.HostKey, h)},
                {"port=", "Port the webserver listens on", p => fn.SetConfiguration(Constants.Configuration.PortKey, int.Parse(p))},
                {"ravenconnectionstring=", "Connection string for RavenDb if you don't want to use the local, embedded version", r => fn.SetConfiguration(Constants.Configuration.RavenConnectionStringKey, r)},
                {"ravendatadirectory=", "Path for the local, embedded RavenDb data directory", r => fn.SetConfiguration(Constants.Configuration.RavenDataDirectoryKey, r)},
                {"repositoriespath=", "Path for the local GitHub repositories", r => fn.SetConfiguration(Constants.Configuration.RepositoriesPathKey, r)},
                {"h|?|help", v => showHelp = v != null}
            };

            try
            {
                options.Parse(args);
            }
            catch (OptionException optionEx)
            {
                Console.Write("qed: ");
                Console.WriteLine(optionEx.Message);
                Console.WriteLine("Try `qed.exe --help' for more information.");
                Environment.Exit(1);
            }

            if (showHelp)
            {
                ShowHelp(options);
                Environment.Exit(1);
            }
        }

        static string ReadPassword()
        {
            var password = new StringBuilder();
            ConsoleKeyInfo key;

            do
            {
                key = Console.ReadKey(true);

                if (key.Key != ConsoleKey.Backspace && key.Key != ConsoleKey.Enter)
                {
                    password.Append(key.KeyChar);
                    Console.Write("*");
                }
                else
                {
                    if (key.Key == ConsoleKey.Backspace && password.Length > 0)
                    {
                        password.Length--;
                        Console.Write("\b \b");
                    }
                }
            }
            while (key.Key != ConsoleKey.Enter);

            return password.ToString();
        }

        static void RunBuilds()
        {
            // TODO: Decide on a better login approach
            var fail = new Action<Exception>(ex =>
            {
                Console.WriteLine("BuildNext failed with: ");
                Console.WriteLine(String.Concat("\t", ex));
            });

            while (true)
            {
                try
                {
                    // TODO: Do this with cancellation and timeout
                    fn.BuildNext(Console.WriteLine);
                    fn.FailTimedOutBuilds(Console.WriteLine);
                }
                catch (AggregateException agEx)
                {
                    // TODO: Do I need to worry about InnerExceptions?
                    fail(agEx.InnerException);
                }
                catch (Exception ex)
                {
                    fail(ex);
                }
                finally
                {
                    // TODO: Surely there is a better way to idle?
                    Thread.Sleep(250);
                }
            }
        }

        static void SetDefaultConfig()
        {
            fn.SetConfiguration(Constants.Configuration.BuildConfigurationLocationKey, GetBuildConfigurationPath());
            fn.SetConfiguration(Constants.Configuration.HostKey, null);
            fn.SetConfiguration(Constants.Configuration.PortKey, 1754);
            fn.SetConfiguration(Constants.Configuration.RavenConnectionStringKey, null);
            fn.SetConfiguration(Constants.Configuration.RavenDataDirectoryKey, "~\\.ravendb");
            fn.SetConfiguration(Constants.Configuration.RepositoriesPathKey, ".repositories");
        }

        static void StartWebServer()
        {
            var appBuilder = new AppBuilder();

            OwinServerFactory.Initialize(appBuilder.Properties);

            appBuilder.Properties.Add("host.AppName", "QED");

            fn.ConfigureBuilder(appBuilder);

            var serverBuilder = ServerBuilder
                .New()
                .SetPort(fn.GetConfiguration<int>(Constants.Configuration.PortKey))
                .SetOwinApp(appBuilder.Build())
                .SetOwinCapabilities((IDictionary<string, object>)appBuilder.Properties[OwinKeys.ServerCapabilitiesKey]);

            var server = serverBuilder.Start();

            Console.CancelKeyPress += (sender, eventArgs) => server.Dispose();

            Console.WriteLine("Started qed.");
            Console.WriteLine("Listening on port {0}.", fn.GetConfiguration<int>(Constants.Configuration.PortKey));
            Console.WriteLine("Press CTRL+C to exit.");
            Console.WriteLine();
        }
    }
}
