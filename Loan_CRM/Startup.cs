using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Loan_CRM.Startup))]
namespace Loan_CRM
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
