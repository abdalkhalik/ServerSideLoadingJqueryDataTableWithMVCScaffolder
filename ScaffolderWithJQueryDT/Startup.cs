using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ScaffolderWithJQueryDT.Startup))]
namespace ScaffolderWithJQueryDT
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
