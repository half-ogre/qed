using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OwinExtensions;

namespace qed
{
    using AppFunc = Func<IDictionary<string, object>, Task>;
    using HandlerFunc = Func<IDictionary<string, object>, dynamic, Func<IDictionary<string, object>, Task>, Task>;
    using MiddlewareFunc = Func<Func<IDictionary<string, object>, Task>, Func<IDictionary<string, object>, Task>>;

    public class DispatcherMiddleware
    {
        public static MiddlewareFunc Create(Action<IDispatcher> configure)
        {
            var dispatcher = new Dispatcher();

            configure(dispatcher);

            return next => environment =>
            {
                var method = environment.GetMethod();
                var path = environment.GetPath();

                dynamic @params;
                
                var handler = dispatcher.FindHandler(method, path, out @params);

                if (handler == null)
                    return next(environment);

                var handlerFuncs = (handler.MiddlewareFuncs ?? new MiddlewareFunc[0])
                    .Concat(new MiddlewareFunc[] { handlerNext => handlerEnv => handler.HandlerFunc(handlerEnv, @params, handlerNext) })
                    .GetEnumerator();

                AppFunc nextHandlerFunc = null;
                    
                nextHandlerFunc = nextHandlerFuncEnv => handlerFuncs.MoveNext() 
                    ? handlerFuncs.Current(currentEnv => nextHandlerFunc(currentEnv))(nextHandlerFuncEnv) 
                    : next(environment);
                    
                return nextHandlerFunc(environment);
            };
        }
    }
}
