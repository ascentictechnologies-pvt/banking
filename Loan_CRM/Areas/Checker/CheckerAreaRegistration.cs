using System.Web.Mvc;

namespace Loan_CRM.Areas.Checker
{
    public class CheckerAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Checker";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Checker_default",
                "Checker/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}