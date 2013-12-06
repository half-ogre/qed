using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace qed
{
    using HandlerFunc = Func<IDictionary<string, object>, Func<IDictionary<string, object>, Task>, Task>;
    using HandlerWithParamsFunc = Func<IDictionary<string, object>, dynamic, Func<IDictionary<string, object>, Task>, Task>;
    using MiddlewareFunc = Func<Func<IDictionary<string, object>, Task>, Func<IDictionary<string, object>, Task>>;

    public interface IDispatcher
    {
        void AddHandler(
            string method,
            DispatcherHandler handler);

        void Delete(
            string urlPattern,
            HandlerFunc handler);

        void Delete(
            string urlPattern,
            HandlerWithParamsFunc handler);

        void Delete(
            string urlPattern,
            MiddlewareFunc middlewareFunc,
            HandlerFunc handler);

        void Delete(
            string urlPattern,
            MiddlewareFunc middlewareFunc,
            HandlerWithParamsFunc handler);
        
        void Delete(
            string urlPattern,
            MiddlewareFunc[] middlewareFuncs,
            HandlerFunc handler);

        void Delete(
            string urlPattern,
            MiddlewareFunc[] middlewareFuncs,
            HandlerWithParamsFunc handler);

        void Get(
            string urlPattern,
            HandlerFunc handler);

        void Get(
            string urlPattern,
            HandlerWithParamsFunc handler);

        void Get(
            string urlPattern,
            MiddlewareFunc middlewareFunc,
            HandlerFunc handler);

        void Get(
            string urlPattern,
            MiddlewareFunc middlewareFunc,
            HandlerWithParamsFunc handler);

        void Get(
            string urlPattern,
            MiddlewareFunc[] middlewareFuncs,
            HandlerFunc handler);

        void Get(
            string urlPattern,
            MiddlewareFunc[] middlewareFuncs,
            HandlerWithParamsFunc handler);

        void Patch(
            string urlPattern,
            HandlerFunc handler);

        void Patch(
            string urlPattern,
            HandlerWithParamsFunc handler);

        void Patch(
            string urlPattern,
            MiddlewareFunc middlewareFunc,
            HandlerFunc handler);

        void Patch(
            string urlPattern,
            MiddlewareFunc middlewareFunc,
            HandlerWithParamsFunc handler);

        void Patch(
            string urlPattern,
            MiddlewareFunc[] middlewareFuncs,
            HandlerFunc handler);

        void Patch(
            string urlPattern,
            MiddlewareFunc[] middlewareFuncs,
            HandlerWithParamsFunc handler);

        void Post(
            string urlPattern,
            HandlerFunc handler);

        void Post(
            string urlPattern,
            HandlerWithParamsFunc handler);

        void Post(
            string urlPattern,
            MiddlewareFunc middlewareFunc,
            HandlerFunc handler);

        void Post(
            string urlPattern,
            MiddlewareFunc middlewareFunc,
            HandlerWithParamsFunc handler);

        void Post(
            string urlPattern,
            MiddlewareFunc[] middlewareFuncs,
            HandlerFunc handler);

        void Post(
            string urlPattern,
            MiddlewareFunc[] middlewareFuncs,
            HandlerWithParamsFunc handler);

        void Put(
            string urlPattern,
            HandlerFunc handler);

        void Put(
            string urlPattern,
            HandlerWithParamsFunc handler);

        void Put(
            string urlPattern,
            MiddlewareFunc middlewareFunc,
            HandlerFunc handler);

        void Put(
            string urlPattern,
            MiddlewareFunc middlewareFunc,
            HandlerWithParamsFunc handler);

        void Put(
            string urlPattern,
            MiddlewareFunc[] middlewareFuncs,
            HandlerFunc handler);

        void Put(
            string urlPattern,
            MiddlewareFunc[] middlewareFuncs,
            HandlerWithParamsFunc handler);
    }
}