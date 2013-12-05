using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OwinExtensions;

namespace qed
{
    using HandlerFunc = Func<IDictionary<string, object>, dynamic, Func<IDictionary<string, object>, Task>, Task>;
    using MiddlewareFunc = Func<Func<IDictionary<string, object>, Task>, Func<IDictionary<string, object>, Task>>;

    public class DispatcherMiddleware
    {
        public static MiddlewareFunc Create(Action<Dispatcher> configure)
        {
            var dispatcher = new Dispatcher();

            configure(dispatcher);

            return next => environment =>
            {
                var method = environment.GetMethod();
                var path = environment.GetPath();

                var handler = dispatcher.FindHandler(method, path);

                if (handler == null)
                    return next(environment);

                return handler(environment, next);
            };
        }
    }
}
