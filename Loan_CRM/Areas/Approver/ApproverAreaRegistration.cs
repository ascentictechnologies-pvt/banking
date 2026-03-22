using System.Web.Mvc;

namespace Loan_CRM.Areas.Approver
{
    public class ApproverAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Approver";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Approver_default",
                "Approver/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}