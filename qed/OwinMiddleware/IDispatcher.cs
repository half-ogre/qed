using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace qed
{
    using HandlerFunc = Func<IDictionary<string, object>, Func<IDictionary<string, object>, Task>, Task>;
    using HandlerWithParamsFunc = Func<IDictionary<string, object>, dynamic, Func<IDictionary<string, object>, Task>, Task>;

    public interface IDispatcher
    {
        void Delete(
            string urlPattern,
            HandlerFunc handler);

        void Delete(
            string urlPattern,
            HandlerWithParamsFunc handler);

        void Get(
            string urlPattern,
            HandlerFunc handler);

        void Get(
            string urlPattern,
            HandlerWithParamsFunc handler);

        void Patch(
            string urlPattern,
            HandlerFunc handler);

        void Patch(
            string urlPattern,
            HandlerWithParamsFunc handler);

        void Post(
            string urlPattern,
            HandlerFunc handler);

        void Post(
            string urlPattern,
            HandlerWithParamsFunc handler);

        void Put(
            string urlPattern,
            HandlerFunc handler);

        void Put(
            string urlPattern,
            HandlerWithParamsFunc handler);
    }
}