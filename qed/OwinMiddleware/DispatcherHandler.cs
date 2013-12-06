using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace qed
{
    using HandlerWithParamsFunc = Func<IDictionary<string, object>, dynamic, Func<IDictionary<string, object>, Task>, Task>;
    using MiddlewareFunc = Func<Func<IDictionary<string, object>, Task>, Func<IDictionary<string, object>, Task>>;

    public class DispatcherHandler
    {
        public DispatcherHandler(
            Regex urlPatternRegex, 
            MiddlewareFunc[] middlewareFuncs, 
            HandlerWithParamsFunc handlerFunc)
        {
            if (urlPatternRegex == null) throw new ArgumentNullException("urlPatternRegex");
            if (handlerFunc == null) throw new ArgumentNullException("handlerFunc");

            UrlPatternRegex = urlPatternRegex;
            HandlerFunc = handlerFunc;
            MiddlewareFuncs = middlewareFuncs;
        }

        public HandlerWithParamsFunc HandlerFunc { get; private set; }
        public MiddlewareFunc[] MiddlewareFuncs { get; private set; }
        public Regex UrlPatternRegex { get; private set; }
    }
}
