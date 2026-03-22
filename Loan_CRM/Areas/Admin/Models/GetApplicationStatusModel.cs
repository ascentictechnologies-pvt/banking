using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Loan_CRM.Areas.Admin.Models
{
    public class GetApplicationStatusModel
    {
        public string SearchText { get; set; }
        public string SearchBy { get; set; }
        public List<GetApplicationStatusData> applicationStatusDatas { get; set; }
        public IEnumerable<SelectListItem> SearchByOptions
        {
            get
            {
                return new[]
                {
                    new SelectListItem { Value="",Text ="Select"},
                    new SelectListItem { Value="ApplicationNo",Text ="Application No"},
                     new SelectListItem { Value="Name",Text ="By Name"},
                    new SelectListItem { Value="Email",Text ="By Email"}
                };
            }
        }
        public GetApplicationStatusModel()
        {
            applicationStatusDatas = new List<GetApplicationStatusData>();
        }
    }

    public class GetApplicationStatusData
    {
        public int ApplicationNo { get; set; }
        public string Name { get; set; }
        public string location { get; set; }
        public string HandleBy { get; set; } = "";
        public string CheckedBy { get; set; }
        public string CheckedOn { get; set; }
        public string VerifiedBy { get; set; }
        public string VerifiedOn { get; set; }
        public string ApprovedOn { get; set; }
        public string IsComplete { get; set; } = "";
        public string ApplicationRemark { get; set; }
    }
}