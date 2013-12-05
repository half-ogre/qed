using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace qed
{
    using HandlerFunc = Func<IDictionary<string, object>, dynamic, Func<IDictionary<string, object>, Task>, Task>;
    using MiddlewareFunc = Func<Func<IDictionary<string, object>, Task>, Func<IDictionary<string, object>, Task>>;

    public class Dispatcher : IDispatcher
    {
        readonly IDictionary<string, List<Tuple<Regex, HandlerFunc>>> _handlers;

        static readonly Regex _tokenRegex = new Regex(@"\{([a-z]+)\}", RegexOptions.IgnoreCase);

        public Dispatcher()
        {
            _handlers = new Dictionary<string, List<Tuple<Regex, HandlerFunc>>>();
        }

        protected virtual void AddHandler(string method, Tuple<Regex, HandlerFunc> handler)
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
            Func<IDictionary<string, object>, Func<IDictionary<string, object>, Task>, Task> handler)
        {
            Delete(urlPattern, (environment, @params, next) => handler(environment, next));
        }

        public void Delete(
            string urlPattern,
            HandlerFunc handler)
        {
            AddHandler(
                "DELETE",
                new Tuple<Regex, HandlerFunc>(
                    CreateRegexForUrlPattern(urlPattern),
                    handler));
        }

        void EnsureHandlersHaveMethodKey(string method)
        {
            var key = method.ToLowerInvariant();

            if (!_handlers.ContainsKey(key))
                _handlers.Add(key, new List<Tuple<Regex, HandlerFunc>>());
        }

        public virtual Func<IDictionary<string, object>, Func<IDictionary<string, object>, Task>, Task> FindHandler(string method, string path)
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
            HandlerFunc handler)
        {
            AddHandler(
                "GET",
                new Tuple<Regex, HandlerFunc>(
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
            HandlerFunc handler)
        {
            AddHandler(
                "PATCH",
                new Tuple<Regex, HandlerFunc>(
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
            HandlerFunc handler)
        {
            AddHandler(
                "POST",
                new Tuple<Regex, HandlerFunc>(
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
            HandlerFunc handler)
        {
            AddHandler(
                "PUT",
                new Tuple<Regex, HandlerFunc>(
                    CreateRegexForUrlPattern(urlPattern),
                    handler));
        }
    }
}
