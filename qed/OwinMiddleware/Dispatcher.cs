using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace qed
{
    using HandlerFunc = Func<IDictionary<string, object>, Func<IDictionary<string, object>, Task>, Task>;
    using HandlerWithParamsFunc = Func<IDictionary<string, object>, dynamic, Func<IDictionary<string, object>, Task>, Task>;
    using MiddlewareFunc = Func<Func<IDictionary<string, object>, Task>, Func<IDictionary<string, object>, Task>>;

    public class Dispatcher : IDispatcher
    {
        readonly IDictionary<string, List<DispatcherHandler>> _handlers;

        static readonly Regex _tokenRegex = new Regex(@"\{([a-z]+)\}", RegexOptions.IgnoreCase);

        public Dispatcher()
        {
            _handlers = new Dictionary<string, List<DispatcherHandler>>();
        }

        public virtual void AddHandler(
            string method, 
            DispatcherHandler handler)
        {
            var key = method.ToLowerInvariant();

            EnsureHandlersHaveMethodKey(key);

            _handlers[key].Add(handler);
        }

        protected virtual Regex CreateRegexForUrlPattern(string urlPattern)
        {
            var regexString = _tokenRegex.Replace(urlPattern, @"(?<$1>[^/]+)");

            return new Regex(String.Concat("^", regexString, "$"));
        }

        public void Delete(
            string urlPattern,
            HandlerFunc handlerFunc)
        {
            Delete(urlPattern, new MiddlewareFunc[0], (environment, @params, next) => handlerFunc(environment, next));
        }

        public void Delete(
            string urlPattern,
            HandlerWithParamsFunc handlerFunc)
        {
            Delete(urlPattern, new MiddlewareFunc[0], handlerFunc);
        }

        public void Delete(
            string urlPattern,
            MiddlewareFunc middlewareFunc,
            HandlerFunc handlerFunc)
        {
            Delete(urlPattern, new []{ middlewareFunc }, (environment, @params, next) => handlerFunc(environment, next));
        }

        public void Delete(
            string urlPattern,
            MiddlewareFunc middlewareFunc,
            HandlerWithParamsFunc handlerFunc)
        {
            Delete(urlPattern, new []{ middlewareFunc }, handlerFunc);
        }

        public void Delete(
            string urlPattern,
            MiddlewareFunc[] middlewareFuncs,
            HandlerFunc handlerFunc)
        {
            Delete(urlPattern, middlewareFuncs, (environment, @params, next) => handlerFunc(environment, next));
        }

        public void Delete(
            string urlPattern,
            MiddlewareFunc[] middlewareFuncs,
            HandlerWithParamsFunc handlerFunc)
        {
            AddHandler(
                "DELETE",
                new DispatcherHandler(
                    CreateRegexForUrlPattern(urlPattern),
                    middlewareFuncs,
                    handlerFunc));
        }

        void EnsureHandlersHaveMethodKey(string method)
        {
            var key = method.ToLowerInvariant();

            if (!_handlers.ContainsKey(key))
                _handlers.Add(key, new List<DispatcherHandler>());
        }

        public virtual DispatcherHandler FindHandler(
            string method, 
            string path,
            out dynamic @params)
        {
            var key = method.ToLowerInvariant();

            EnsureHandlersHaveMethodKey(key);

            var paramsDictionary = new ExpandoObject() as IDictionary<string, object>; ;

            Match matches = null;

            var matchingHandler = _handlers[key]
                .FirstOrDefault(x =>
                {
                    matches = x.UrlPatternRegex.Match(path);
                    return matches.Success;
                });

            if (matchingHandler == null)
            {
                @params = paramsDictionary;
                return null;
            }

            foreach (var groupName in matchingHandler.UrlPatternRegex.GetGroupNames())
                paramsDictionary.Add(groupName, matches.Groups[groupName].Value);

            @params = paramsDictionary;

            return matchingHandler;
        }

        public void Get(
            string urlPattern,
            HandlerFunc handlerFunc)
        {
            Get(urlPattern, new MiddlewareFunc[0], (environment, @params, next) => handlerFunc(environment, next));
        }

        public void Get(
            string urlPattern,
            HandlerWithParamsFunc handlerFunc)
        {
            Get(urlPattern, new MiddlewareFunc[0], handlerFunc);
        }

        public void Get(
            string urlPattern,
            MiddlewareFunc middlewareFunc,
            HandlerFunc handlerFunc)
        {
            Get(urlPattern, new []{ middlewareFunc }, (environment, @params, next) => handlerFunc(environment, next));
        }

        public void Get(
            string urlPattern,
            MiddlewareFunc middlewareFunc,
            HandlerWithParamsFunc handlerFunc)
        {
            Get(urlPattern, new[] { middlewareFunc }, handlerFunc);
        }

        public void Get(
            string urlPattern,
            MiddlewareFunc[] middlewareFuncs,
            HandlerFunc handlerFunc)
        {
            Get(urlPattern, middlewareFuncs, (environment, @params, next) => handlerFunc(environment, next));
        }

        public void Get(
            string urlPattern,
            MiddlewareFunc[] middlewareFuncs,
            HandlerWithParamsFunc handlerFunc)
        {
            AddHandler(
                "GET",
                new DispatcherHandler(
                    CreateRegexForUrlPattern(urlPattern),
                    middlewareFuncs,
                    handlerFunc));
        }

        public void Patch(
            string urlPattern,
            HandlerFunc handlerFunc)
        {
            Patch(urlPattern, new MiddlewareFunc[0], (environment, @params, next) => handlerFunc(environment, next));
        }

        public void Patch(
            string urlPattern,
            HandlerWithParamsFunc handlerFunc)
        {
            Patch(urlPattern, new MiddlewareFunc[0], handlerFunc);
        }

        public void Patch(
            string urlPattern,
            MiddlewareFunc middlewareFunc,
            HandlerFunc handlerFunc)
        {
            Patch(urlPattern, new[] { middlewareFunc }, (environment, @params, next) => handlerFunc(environment, next));
        }

        public void Patch(
            string urlPattern,
            MiddlewareFunc middlewareFunc,
            HandlerWithParamsFunc handlerFunc)
        {
            Patch(urlPattern, new[] { middlewareFunc }, handlerFunc);
        }
        
        public void Patch(
            string urlPattern,
            MiddlewareFunc[] middlewareFuncs,
            HandlerFunc handlerFunc)
        {
            Patch(urlPattern, middlewareFuncs, (environment, @params, next) => handlerFunc(environment, next));
        }

        public void Patch(
            string urlPattern,
            MiddlewareFunc[] middlewareFuncs,
            HandlerWithParamsFunc handlerFunc)
        {
            AddHandler(
                "PATCH",
                new DispatcherHandler(
                    CreateRegexForUrlPattern(urlPattern),
                    middlewareFuncs,
                    handlerFunc));
        }

        public void Post(
            string urlPattern,
            HandlerFunc handlerFunc)
        {
            Post(urlPattern, new MiddlewareFunc[0], (environment, @params, next) => handlerFunc(environment, next));
        }

        public void Post(
            string urlPattern,
            HandlerWithParamsFunc handlerFunc)
        {
            Post(urlPattern, new MiddlewareFunc[0], handlerFunc);
        }

        public void Post(
            string urlPattern,
            MiddlewareFunc middlewareFunc,
            HandlerFunc handlerFunc)
        {
            Post(urlPattern, new[] { middlewareFunc }, (environment, @params, next) => handlerFunc(environment, next));
        }

        public void Post(
            string urlPattern,
            MiddlewareFunc middlewareFunc,
            HandlerWithParamsFunc handlerFunc)
        {
            Post(urlPattern, new[] { middlewareFunc }, handlerFunc);
        }

        public void Post(
            string urlPattern,
            MiddlewareFunc[] middlewareFuncs,
            HandlerFunc handlerFunc)
        {
            Post(urlPattern, middlewareFuncs, (environment, @params, next) => handlerFunc(environment, next));
        }

        public void Post(
            string urlPattern,
            MiddlewareFunc[] middlewareFuncs,
            HandlerWithParamsFunc handlerFunc)
        {
            AddHandler(
                "POST",
                new DispatcherHandler(
                    CreateRegexForUrlPattern(urlPattern),
                    middlewareFuncs,
                    handlerFunc));
        }

        public void Put(
            string urlPattern,
            HandlerFunc handlerFunc)
        {
            Put(urlPattern, new MiddlewareFunc[0], (environment, @params, next) => handlerFunc(environment, next));
        }

        public void Put(
            string urlPattern,
            HandlerWithParamsFunc handlerFunc)
        {
            Put(urlPattern, new MiddlewareFunc[0], handlerFunc);
        }

        public void Put(
            string urlPattern,
            MiddlewareFunc middlewareFunc,
            HandlerFunc handler)
        {
            Put(urlPattern, new[] { middlewareFunc }, (environment, @params, next) => handler(environment, next));
        }

        public void Put(
            string urlPattern,
            MiddlewareFunc middlewareFunc,
            HandlerWithParamsFunc handlerFunc)
        {
            Put(urlPattern, new[] { middlewareFunc }, handlerFunc);
        }
        
        public void Put(
            string urlPattern,
            MiddlewareFunc[] middlewareFuncs,
            HandlerFunc handlerFunc)
        {
            Put(urlPattern, middlewareFuncs, (environment, @params, next) => handlerFunc(environment, next));
        }

        public void Put(
            string urlPattern,
            MiddlewareFunc[] middlewareFuncs,
            HandlerWithParamsFunc handlerFunc)
        {
            AddHandler(
                "PUT",
                new DispatcherHandler(
                    CreateRegexForUrlPattern(urlPattern),
                    middlewareFuncs,
                    handlerFunc));
        }
    }
}
