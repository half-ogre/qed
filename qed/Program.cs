using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Nowin;
using Owin.Builder;
using fn = qed.Functions;

namespace qed
{
    using AppFunc = Func<IDictionary<string, object>, Task>;

    class Program
    {
        static void Main()
        {
            var appBuilder = new AppBuilder();

            fn.ConfigureBuilder(appBuilder);

            var serverBuilder = ServerBuilder
                .New()
                .SetPort(1754)
                .SetOwinApp(appBuilder.Build(typeof(AppFunc)) as AppFunc);

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
    }
}
