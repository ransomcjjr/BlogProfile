using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(CJRProfileBlog.Startup))]
namespace CJRProfileBlog
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
