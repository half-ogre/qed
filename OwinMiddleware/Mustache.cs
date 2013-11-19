using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Nustache.Core;

namespace qed
{
    using MiddlewareFunc = Func<Func<IDictionary<string, object>, Task>, Func<IDictionary<string, object>, Task>>;

    public static class Mustache
    {
        class MustacheConfiguration
        {
            public string LayoutTemplateName { get; set; }
            public string TemplateFileExtension { get; set; }
            public string TemplateRootPath { get; set; }
        }

        const string _confiugrationKey = "owinmustache.Confguration";

        public static MiddlewareFunc Create(
            string templateRootDirectoryName = null,
            string templateFileExtension = null,
            string layoutTemplateName = null)
        {
            templateRootDirectoryName = templateRootDirectoryName ?? "templates";
            templateFileExtension = templateFileExtension ?? ".mustache";
            layoutTemplateName = layoutTemplateName ?? "_layout";

            var templateRootPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, templateRootDirectoryName);

            var configuration = new MustacheConfiguration
            {
                TemplateRootPath = templateRootPath,
                TemplateFileExtension = templateFileExtension,
                LayoutTemplateName = layoutTemplateName
            };

            return next => environment =>
            {
                environment[_confiugrationKey] = configuration;

                return next(environment);
            };
        }

        static Template GetTemplate(
            MustacheConfiguration configuration, 
            string templateName)
        {
            var templatePath = Path.Combine(
                configuration.TemplateRootPath,
                String.Concat(templateName, configuration.TemplateFileExtension));

            if (!File.Exists(templatePath))
                throw new InvalidOperationException("Template path does not exist.");

            var templateSource = File.ReadAllText(templatePath);

            var template = new Template();
            template.Load(new StringReader(templateSource));
            return template;
        }

        static bool HasLayout(MustacheConfiguration configuration)
        {
            return File.Exists(
                Path.Combine(
                    configuration.TemplateRootPath,
                    String.Concat(configuration.LayoutTemplateName, configuration.TemplateFileExtension)));
        }

        public static Task Render(
            this IDictionary<string, object> environment,
            string templateName,
            object data)
        {
            var configuration = environment[_confiugrationKey] as MustacheConfiguration;
            if (configuration == null)
                throw new InvalidOperationException("The OwinMustache middleware is not in use.");

            object responseStream;
            if (!environment.TryGetValue("owin.ResponseBody", out responseStream))
                throw new InvalidOperationException("The OWIN environment did not have a response stream.");

            if (HasLayout(configuration) && !templateName.Equals(configuration.LayoutTemplateName))
            {
                var layout = GetTemplate(configuration, configuration.LayoutTemplateName);
                return RenderTemplate(configuration, (Stream)responseStream, layout, data, hasLayout: true,  bodyTemplateName: templateName);
            }

            var template = GetTemplate(configuration, templateName);
            return RenderTemplate(configuration, (Stream)responseStream, template, data, hasLayout: false);
        }

        static Task RenderTemplate(
            MustacheConfiguration configuration,
            Stream responseStream,
            Template template, 
            object data,
            bool hasLayout,
            string bodyTemplateName = null)
        {
            return Task.Run(() =>
            {
                using (var writer = new StreamWriter(responseStream, Encoding.UTF8, 1, true))
                {
                    template.Render(
                        data,
                        writer,
                        name =>
                        {
                            if (hasLayout && name.Equals("body", StringComparison.OrdinalIgnoreCase))
                                return GetTemplate(configuration, bodyTemplateName);

                            return GetTemplate(configuration, name);
                        });
                }
            });
        }
    }
}
