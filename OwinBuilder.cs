using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Owin;

namespace qed
{
    using AppFunc = Func<IDictionary<string, object>, Task>;
    using MiddlewareFunc = Func<Func<IDictionary<string, object>, Task>, Func<IDictionary<string, object>, Task>>;

    public class OwinBuilder : IAppBuilder
    {
        static readonly Task _completed = CreateCompletedTask();

        readonly IList<MiddlewareFunc> _middleware;

        static readonly MiddlewareFunc _notFound = next => environment =>
        {
            environment.SetStatusCode(404);
            return _completed;
        };

        readonly IDictionary<string, object> _properties;

        public OwinBuilder()
        {
            _properties = new Dictionary<string, object>();
            _middleware = new List<MiddlewareFunc>();
        }

        internal OwinBuilder(IDictionary<string, object> properties)
        {
            _properties = properties;
        }

        public object Build(Type returnType)
        {
            if (returnType != typeof(AppFunc))
                throw new ArgumentException("Return type must be of type AppFunc.", "returnType");

            return ToOwinApp();
        }
        
        static Task CreateCompletedTask()
        {
            var tcs = new TaskCompletionSource<object>();
            tcs.SetResult(null);
            return tcs.Task;
        }

        public IAppBuilder New()
        {
            return new OwinBuilder(_properties);
        }

        public IDictionary<string, object> Properties
        {
            get { return _properties; }
        }

        public AppFunc ToOwinApp()
        {
            return env =>
            {
                var enumerator = _middleware.Concat(new[] { _notFound }).GetEnumerator();
                AppFunc next = null;
                next = nextEnv => enumerator.MoveNext() ? enumerator.Current(currentEnv => next(currentEnv))(nextEnv) : _completed;
                return next(env);
            };
        }

        public IAppBuilder Use(object middleware, params object[] args)
        {
            var castMiddleware = middleware as MiddlewareFunc;
            if (castMiddleware == null)
                throw new ArgumentException("Middleware must be of type MiddlewareFunc.", "middleware");

            _middleware.Add(castMiddleware);

            return this;
        }
    }
}
