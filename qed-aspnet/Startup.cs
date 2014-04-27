using Microsoft.Owin;
using Owin;
using qed;

[assembly:OwinStartup(typeof(Startup))]

namespace qed
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseQed();
        }
    }
}