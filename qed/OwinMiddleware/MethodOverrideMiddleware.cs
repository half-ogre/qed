using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OwinExtensions;

namespace qed
{
    using MiddlewareFunc = Func<Func<IDictionary<string, object>, Task>, Func<IDictionary<string, object>, Task>>;

    public static class MethodOverrideMiddleware
    {
        public static MiddlewareFunc Create(string key = "_method")
        {
            return next => environment =>
            {
                var originalMethod = environment.Get<string>("request.OriginalMethod");
                if (!String.IsNullOrEmpty(originalMethod))
                    return next(environment);

                var form = environment.ReadForm();

                if (form.ContainsKey(key))
                {
                    var methodOverride = form[key].FirstOrDefault();
                    if (!String.IsNullOrEmpty(methodOverride))
                    {
                        environment.Set("request.OriginalMethod", environment.GetRequestMethod());
                        environment.SetRequestMethod(methodOverride);
                    }
                }

                return next(environment);
            };
        }
    }
}
