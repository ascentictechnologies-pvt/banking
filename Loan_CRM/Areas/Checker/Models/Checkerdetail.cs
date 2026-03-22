using System;

namespace Loan_CRM.Areas.Checker.Models
{
    public class Checkerdetail
    {

        public bool is_orr_declined { get; set; }
        public int Id { get; set; }
        public Int64 applicationno { get; set; }
        public string applicationname { get; set; }
        public string RequestDate { get; set; }
        public string Detail { get; set; }
        public string personalcontactno { get; set; }
        public string personalemail { get; set; }
        public string address { get; set; }
        public int term { get; set; }
        public string gov_id_url { get; set; }
        public string companyid_url { get; set; }
        public string billing_url { get; set; }
        public string income_url { get; set; }
        public string atm_url { get; set; }
        public DateTime createdon { get; set; }
        public DateTime checker_oic_on { get; set; }
        public string other_url { get; set; }
        public bool ispickedchecker { get; set; }
        public bool ispickedrechecker { get; set; }
        public DateTime UpdateDate { get; set; }
        public bool iscompetemandate { get; set; }
        public bool iscompetedocu { get; set; }
        public string MorningTime { get; set; }
        public string NoonTime { get; internal set; }
        public int aging { get; set; }
        public bool ischeck { get; set; }
        public string sss_no { get; set; }
        public int occupation { get; set; }
        public int barangay { get; set; }
        public DateTime AppliedOn { get; set; }
        public string checker_remark { get; set; }
    }
}