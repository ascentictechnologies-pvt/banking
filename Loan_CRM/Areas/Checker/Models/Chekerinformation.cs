using Loan_CRM.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace Loan_CRM.Areas.Checker.Models
{
    public class Chekerinformation
    {
        public int? dateapplied { get; set; }
        public int? applicationno { get; set; }
        public string gov_doc_type { get; set; }
        // applicationno
        public string age { get; set; }
        public string gender { get; set; }
        public string facemapstatus { get; set; }
        public string jumioreference { get; set; }
        public string transactionid { get; set; }
        public HttpPostedFileBase GovIdFile { get; set; }
        public HttpPostedFileBase CompanyIdFile { get; set; }
        public HttpPostedFileBase BillingIdFile { get; set; }
        public HttpPostedFileBase IncomeIdFile { get; set; }
        public HttpPostedFileBase OtherIdFile { get; set; }
        public HttpPostedFileBase AtmIdFile { get; set; }
        public string gov_id_url { get; set; }
        public string companyid_url { get; set; }
        public string billing_url { get; set; }
        public string income_url { get; set; }
        public string atm_url { get; set; }
        public string other_url { get; set; }
        public string name { get; set; }
        public string personalemail { get; set; }
        public string personalcontactno { get; set; }
        public string secondcontactno { get; set; }
        public string company_phoneno { get; set; }
        public string thirdcontactno { get; set; }
        public string address { get; set; }
        public string homephoneno { get; set; }
        public String placeofbirth { get; set; }
        public String dateofbirth { get; set; }
        public String date_joining { get; set; }
        public String pay_date { get; set; }
        public String civilstatus { get; set; }
        public string mothermaidenname { get; set; }
        public string motheraddress { get; set; }
        public string companyname { get; set; }
        public string companyaddress { get; set; } = "";
        public string designation { get; set; }
        public Decimal gross_income { get; set; }
        public string reference_name { get; set; }
        public string city { get; set; }
        public string reference_contactno { get; set; }
        public string notificationtoken { get; set; }
        public string bankname { get; set; }
        public string bankaccountno { get; set; }
        public string employername { get; set; }
        public string employeraddress { get; set; }
        public string employercontectnumber { get; set; }
        public string paydate { get; set; }
        public string remarks { get; set; }
        public string cityname { get; set; }
        public string street { get; set; }
        public string barangay { get; set; }
        public string zipcode { get; set; }
        public int paydate1 { get; set; }
        public int paydate2 { get; set; }
        public string sss_no { get; set; }
        public string ActionName { get; set; }
        public string barangay_name { get; set; }
        public string province_name { get; set; }
        public string Remark { get; set; }
        public string MorningTime { get; set; }
        public string NoonTime { get; set; }
        public List<RemarkModel> RemarkList { get; set; }
        public List<Chekerinformation> chekerinformationslist { get; set; }
        public string add_personalcontactno { get; set; }
        public string add_company_phoneno { get; set; }
        public int user_id { get; set; }
        public List<int> LeadCount { get; set; }
        public List<int> LeadCount_VerifyPushed { get; set; }
        public List<int> LeadCount_VerifyReject { get; set; }
        public List<int> LeadCount_Approved { get; set; }
        public List<UserForReport> UserForReportList { get; set; }
        [Required(ErrorMessage = "Required")]
        public DateTime FromDate { get; set; }
        [Required(ErrorMessage = "Required")]
        public DateTime EndDate { get; set; }
        public string templatename { get; set; }
        public CredoModel CredoModel { get; set; }
        public Chekerinformation()
        {
            RemarkList = new List<RemarkModel>();
            chekerinformationslist = new List<Chekerinformation>();
            LeadCount = new List<int>();
            LeadCount_VerifyPushed = new List<int>();
            LeadCount_VerifyReject = new List<int>();
            LeadCount_Approved = new List<int>();
            UserForReportList = new List<UserForReport>();
            CredoModel= new CredoModel();
        }
    }
    public class UserForReport
    {
        public int userid { get; set; }
        public string username { get; set; }
    }
}