using Owin;

namespace qed
{
    public static partial class Functions
    {
        public static void ConfigureBuilder(IAppBuilder builder)
        {
            builder.Use(ContentType.Create());

            builder.Use(Mustache.Create(
                templateRootDirectoryName: "MustacheTemplates",
                layoutTemplateName: "_layout"));

            builder.Use(DispatcherMiddleware.Create(dispatcher =>
            {
                dispatcher.Get("/", Handlers.GetHome);
                dispatcher.Post("/events/push", Handlers.PostPushEvent);
                dispatcher.Post("/events/force", Handlers.PostForceEvent);
                dispatcher.Get("/{owner}/{name}", Handlers.GetBuilds);
                dispatcher.Get("/{owner}/{name}/builds/{id}", Handlers.GetBuild);
                dispatcher.Post("/{owner}/{name}/builds", Handlers.PostBuild);
            }));
        }
    }
}
