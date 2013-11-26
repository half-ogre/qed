using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OwinExtensions;

namespace qed
{
    using MiddlewareFunc = Func<Func<IDictionary<string, object>, Task>, Func<IDictionary<string, object>, Task>>;

    public static class ContentType
    {
        public static MiddlewareFunc Create()
        {
            return next => environment => next(environment)
                .ContinueWith(_ =>
                {
                    var responseHeaders = environment.GetResponseHeaders();
                    if (!responseHeaders.ContainsKey("Content-Type"))
                    {
                        responseHeaders.Add("Content-Type", new [] { "text/html" });
                    }
                });
        }
    }
}
