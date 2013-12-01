using Mono.Options;
using Nowin;
using System;
using System.Threading;
using fn = qed.Functions;

namespace qed
{
    class Program
    {
        static void Main(string[] args)
        {
            string passwordArg = null;
            var showHelp = false;
            string userArg = null;

            var options = new OptionSet
            {
                {"u|user=", v => userArg = v},
                {"p|password=", v => passwordArg = v},
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
                Console.WriteLine("Try `greet --help' for more information.");
                return;
            }

            if (showHelp)
            {
                ShowHelp(options);
                return;
            }

            fn.SetConfiguration(Constants.Args.UserKey, userArg);
            fn.SetConfiguration(Constants.Args.PasswordKey, passwordArg);

            var appBuilder = new OwinBuilder();

            fn.ConfigureBuilder(appBuilder);

            var serverBuilder = ServerBuilder
                .New()
                .SetPort(1754)
                .SetOwinApp(appBuilder.ToOwinApp());

            var server = serverBuilder.Start();

            Console.CancelKeyPress += (sender, eventArgs) => server.Dispose();
                
            Console.WriteLine("Started qed.");
            Console.WriteLine("Listening on port 1754.");
            Console.WriteLine("Press CTRL+C to exit.");
            Console.WriteLine();

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

        static void ShowHelp(OptionSet options)
        {
            Console.WriteLine("Usage: qed.exe [options]");
            Console.WriteLine();
            Console.WriteLine("Options:");
            options.WriteOptionDescriptions(Console.Out);
        }

    }
}
