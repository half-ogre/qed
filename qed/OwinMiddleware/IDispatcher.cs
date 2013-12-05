using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace qed
{
    using HandlerFunc = Func<IDictionary<string, object>, dynamic, Func<IDictionary<string, object>, Task>, Task>;
    using MiddlewareFunc = Func<Func<IDictionary<string, object>, Task>, Func<IDictionary<string, object>, Task>>;

    public interface IDispatcher
    {
        void Delete(
            string urlPattern,
            Func<IDictionary<string, object>, Func<IDictionary<string, object>, Task>, Task> handler);

        void Delete(
            string urlPattern,
            HandlerFunc handler);

        void Get(
            string urlPattern,
            Func<IDictionary<string, object>, Func<IDictionary<string, object>, Task>, Task> handler);

        void Get(
            string urlPattern,
            HandlerFunc handler);

        void Patch(
            string urlPattern,
            Func<IDictionary<string, object>, Func<IDictionary<string, object>, Task>, Task> handler);

        void Patch(
            string urlPattern,
            HandlerFunc handler);

        void Post(
            string urlPattern,
            Func<IDictionary<string, object>, Func<IDictionary<string, object>, Task>, Task> handler);

        void Post(
            string urlPattern,
            HandlerFunc handler);

        void Put(
            string urlPattern,
            Func<IDictionary<string, object>, Func<IDictionary<string, object>, Task>, Task> handler);

        void Put(
            string urlPattern,
            HandlerFunc handler);
    }
}