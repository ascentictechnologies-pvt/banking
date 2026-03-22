using System;
using System.Net;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace Loan_CRM
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(new RazorViewEngine());
        }
        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            // Add TLS 1.2 to the supported cryptographic protocols
            //  - uncomment this line if you're using ASP.NET 4.5 and above
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            //  - uncomment this line if you're using ASP.NET 4.0 and below
            // ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072; 
        }
        protected void Application_EndRequest(object sender, EventArgs e)
        {

        }
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
