using System.Web.Optimization;

namespace Loan_CRM
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include("~/Content/js/jquery.js"));
            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include("~/Scripts/jquery.validate*"));
            bundles.Add(new ScriptBundle("~/bundles/jqueryTimer").Include("~/Content/js/timer.jquery.min.js"));
            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include("~/Scripts/bootstrap.js", "~/Scripts/respond.js"));
            bundles.Add(new ScriptBundle("~/bundles/js").Include("~/Content/js/ValidateCharcterText.js", "~/Content/js/ValidateNumericText.js", "~/Content/js/emailvalidation.js"));
            bundles.Add(new ScriptBundle("~/bundle/datepicker").Include("~/Content/js/jquery-ui.js"));
            bundles.Add(new StyleBundle("~/Content/css").Include("~/Content/bootstrap.css", "~/Content/Site.css"));
            bundles.Add(new StyleBundle("~/Content/Datepicker").Include("~/Content/jquery-ui.css"));
            bundles.Add(new StyleBundle("~/Content/DataTable").Include("~/Content/datatables/dataTables.bootstrap.css"));
            bundles.Add(new ScriptBundle("~/Script/DataTable").Include("~/Content/datatables/jquery.dataTables.min.js", "~/Content/datatables/dataTables.bootstrap.min.js"));

            //For New Design
            bundles.Add(new StyleBundle("~/Content/NewDesign").Include("~/Content/bootstrap_NewDesign.css", "~/Content/style_NewDesign.css"));
            bundles.Add(new ScriptBundle("~/Script/NewDesign").Include("~/Content/js/jquery-ui-slider-pips.js"));

            bundles.Add(new StyleBundle("~/Content/SweetAlertcss").Include("~/Content/sweetalert/sweetalert2.min.css"));
            bundles.Add(new ScriptBundle("~/Content/SweetAlertjs").Include("~/Content/sweetalert/sweetalert2.all.min.js"));
        }
    }
}
