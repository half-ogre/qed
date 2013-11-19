using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace qed
{
    using MiddlewareFunc = Func<Func<IDictionary<string, object>, Task>, Func<IDictionary<string, object>, Task>>;

    public class Dispatcher
    {
        readonly IDictionary<string, List<Tuple<Regex, Func<IDictionary<string, object>, dynamic, Func<IDictionary<string, object>, Task>, Task>>>> _handlers;
        
        static readonly Regex _tokenRegex = new Regex(@"\{([a-z]+)\}", RegexOptions.IgnoreCase);

        private Dispatcher()
        {
            _handlers = new Dictionary<string, List<Tuple<Regex, Func<IDictionary<string, object>, dynamic, Func<IDictionary<string, object>, Task>, Task>>>>();
        }

        void AddHandler(string method, Tuple<Regex, Func<IDictionary<string, object>, dynamic, Func<IDictionary<string, object>, Task>, Task>> handler)
        {
            var key = method.ToLowerInvariant();

            EnsureHandlersHaveMethodKey(key);

            _handlers[key].Add(handler);
        }

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
        
        static Regex CreateRegexForUrlPattern(string urlPattern)
        {
            var regexString = _tokenRegex.Replace(urlPattern, @"(?<$1>[^/]+)");

            return new Regex(String.Concat("^", regexString, "$"));
        }

        public void Delete(
            string urlPattern,
            Func<IDictionary<string, object>, Func<IDictionary<string, object>, Task>, Task> handler)
        {
            Delete(urlPattern, (environment, @params, next) => handler(environment, next));
        }

        public void Delete(
            string urlPattern,
            Func<IDictionary<string, object>, dynamic, Func<IDictionary<string, object>, Task>, Task> handler)
        {
            AddHandler(
                "DELETE",
                new Tuple<Regex, Func<IDictionary<string, object>, dynamic, Func<IDictionary<string, object>, Task>, Task>>(
                    CreateRegexForUrlPattern(urlPattern),
                    handler));
        }

        void EnsureHandlersHaveMethodKey(string method)
        {
            var key = method.ToLowerInvariant();

            if (!_handlers.ContainsKey(key))
                _handlers.Add(key, new List<Tuple<Regex, Func<IDictionary<string, object>, dynamic, Func<IDictionary<string, object>, Task>, Task>>>());
        }

        Func<IDictionary<string, object>, Func<IDictionary<string, object>, Task>, Task> FindHandler(string method, string path)
        {
            var key = method.ToLowerInvariant();

            EnsureHandlersHaveMethodKey(key);

            var @params = new ExpandoObject() as IDictionary<string, object>;

            Match matches = null;

            var matchingHandler = _handlers[key]
                .FirstOrDefault(x =>
                {
                    matches = x.Item1.Match(path);
                    return matches.Success;
                });

            if (matchingHandler == null)
                return null;

            foreach (var groupName in matchingHandler.Item1.GetGroupNames())
                @params.Add(groupName, matches.Groups[groupName].Value);

            return (environement, next) => matchingHandler.Item2(environement, @params, next);
        }

        public void Get(
            string urlPattern,
            Func<IDictionary<string, object>, Func<IDictionary<string, object>, Task>, Task> handler)
        {
            Get(urlPattern, (environment, @params, next) => handler(environment, next));
        }

        public void Get(
            string urlPattern,
            Func<IDictionary<string, object>, dynamic, Func<IDictionary<string, object>, Task>, Task> handler)
        {
            AddHandler(
                "GET",
                new Tuple<Regex, Func<IDictionary<string, object>, dynamic, Func<IDictionary<string, object>, Task>, Task>>(
                    CreateRegexForUrlPattern(urlPattern),
                    handler));
        }

        public void Patch(
            string urlPattern,
            Func<IDictionary<string, object>, Func<IDictionary<string, object>, Task>, Task> handler)
        {
            Patch(urlPattern, (environment, @params, next) => handler(environment, next));
        }

        public void Patch(
            string urlPattern,
            Func<IDictionary<string, object>, dynamic, Func<IDictionary<string, object>, Task>, Task> handler)
        {
            AddHandler(
                "PATCH",
                new Tuple<Regex, Func<IDictionary<string, object>, dynamic, Func<IDictionary<string, object>, Task>, Task>>(
                    CreateRegexForUrlPattern(urlPattern),
                    handler));
        }

        public void Post(
            string urlPattern,
            Func<IDictionary<string, object>, Func<IDictionary<string, object>, Task>, Task> handler)
        {
            Post(urlPattern, (environment, @params, next) => handler(environment, next));
        }

        public void Post(
            string urlPattern,
            Func<IDictionary<string, object>, dynamic, Func<IDictionary<string, object>, Task>, Task> handler)
        {
            AddHandler(
                "POST",
                new Tuple<Regex, Func<IDictionary<string, object>, dynamic, Func<IDictionary<string, object>, Task>, Task>>(
                    CreateRegexForUrlPattern(urlPattern),
                    handler));
        }

        public void Put(
            string urlPattern,
            Func<IDictionary<string, object>, Func<IDictionary<string, object>, Task>, Task> handler)
        {
            Put(urlPattern, (environment, @params, next) => handler(environment, next));
        }

        public void Put(
            string urlPattern,
            Func<IDictionary<string, object>, dynamic, Func<IDictionary<string, object>, Task>, Task> handler)
        {
            AddHandler(
                "PUT",
                new Tuple<Regex, Func<IDictionary<string, object>, dynamic, Func<IDictionary<string, object>, Task>, Task>>(
                    CreateRegexForUrlPattern(urlPattern),
                    handler));
        }
    }
}
