using System.Web.Mvc;

namespace Loan_CRM.Areas.Collector
{
    public class CollectorAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Collector";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Collector_default",
                "Collector/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}