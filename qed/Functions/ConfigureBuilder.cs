using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Owin;
using OwinExtensions;

namespace qed
{
    using MiddlewareFunc = Func<Func<IDictionary<string, object>, Task>, Func<IDictionary<string, object>, Task>>;

    public static partial class Functions
    {
        public static void ConfigureBuilder(IAppBuilder builder)
        {
            var forbidIfSignedIn = new MiddlewareFunc(next => env =>
            {
                var principal = env.Get<IPrincipal>("server.User");
                if (principal != null && principal.Identity.IsAuthenticated)
                {
                    env.SetStatusCode(403);
                    return env.GetCompleted();
                }

                return next(env);
            });

            builder.Use(ContentType.Create());

            builder.Use(MethodOverrideMiddleware.Create());

            builder.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationMode = AuthenticationMode.Active,
                AuthenticationType = "Session",
                CookieName = "Fnord",
                LoginPath = new PathString("/forms/sign-in"),
                LogoutPath = new PathString("/forms/sign-out")
            });

            builder.Use(Mustache.Create(
                templateRootDirectoryName: "MustacheTemplates",
                layoutTemplateName: "_layout"));

            builder.Use(DispatcherMiddleware.Create(dispatcher =>
            {
                dispatcher.Get("/", Handlers.GetHome);
                dispatcher.Post("/events/push", Handlers.PostPushEvent);
                dispatcher.Post("/events/force", Handlers.PostForceEvent);
                dispatcher.Get("/forms/sign-up", forbidIfSignedIn, Handlers.GetSignUpForm);
                dispatcher.Post("/users", forbidIfSignedIn, Handlers.PostUsers);
                dispatcher.Get("/{owner}/{name}", Handlers.GetBuilds);
                dispatcher.Get("/{owner}/{name}/builds/{id}", Handlers.GetBuild);
                dispatcher.Post("/{owner}/{name}/builds", Handlers.PostBuild);
            }));
        }
    }
}
