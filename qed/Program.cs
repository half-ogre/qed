using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
           
            Console.WriteLine("There are no administrator users.");
            Console.WriteLine("You must create an administrator user to start QED.");
            Console.WriteLine("");
            Console.WriteLine("Enter a username for your new administrator user:");
            var username = Console.ReadLine();
            if (String.IsNullOrEmpty(username) || !Constants.UsernameRegex.IsMatch(username))
            {
                Console.WriteLine("Invalid username (must have just letters and numbers). Exiting.");
                Environment.Exit(1);
            }
            Console.WriteLine("Enter a password for your new administrator user:");
            var password = Console.ReadLine();
            if (String.IsNullOrEmpty(password) || password.Length < 8)
            {
                Console.WriteLine("Invalid password (must be at least 8 characters). Exiting.");
                Environment.Exit(1);
            }

            fn.CreateUser(username, password, null);

            Console.WriteLine("Created admnistrator.");
        }

        static void ShowHelp(OptionSet options)
        {
            Console.WriteLine("Usage: qed.exe [options]");
            Console.WriteLine();
            Console.WriteLine("Options:");
            options.WriteOptionDescriptions(Console.Out);
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
            string hostArg = null;
            var showHelp = false;

            var options = new OptionSet
            {
                {"host=", v => hostArg = v},
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

            fn.SetConfiguration(Constants.Configuration.HostKey, hostArg);
        }

        static void RunBuilds()
        {
            // TODO: Decide on a better loggin approach
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

        static void StartWebServer()
        {
            var appBuilder = new AppBuilder();

            OwinServerFactory.Initialize(appBuilder.Properties);

            appBuilder.Properties.Add("host.AppName", "QED");

            fn.ConfigureBuilder(appBuilder);

            var serverBuilder = ServerBuilder
                .New()
                .SetPort(1754)
                .SetOwinApp(appBuilder.Build())
                .SetOwinCapabilities((IDictionary<string, object>)appBuilder.Properties[OwinKeys.ServerCapabilitiesKey]);

            var server = serverBuilder.Start();

            Console.CancelKeyPress += (sender, eventArgs) => server.Dispose();

            Console.WriteLine("Started qed.");
            Console.WriteLine("Listening on port 1754.");
            Console.WriteLine("Press CTRL+C to exit.");
            Console.WriteLine();
        }
    }
}
