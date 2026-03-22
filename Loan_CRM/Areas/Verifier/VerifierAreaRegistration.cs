using System.Web.Mvc;

namespace Loan_CRM.Areas.Verifier
{
    public class VerifierAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Verifier";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Verifier_default",
                "Verifier/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}