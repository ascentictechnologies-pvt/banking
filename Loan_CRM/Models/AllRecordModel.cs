using System.Collections.Generic;
using System.Web.Mvc;

namespace Loan_CRM.Models
{
    public class AllRecordModel
    {
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public int applicationno { get; set; }
        public string dateapplied { get; set; }
        public string name { get; set; }
        public string personalemail { get; set; }
        public string personalcontactno { get; set; }
        public string relativecontactno { get; set; }
        public string coworkercontactno { get; set; }
        public string address { get; set; }
        public string homephoneno { get; set; }
        public string placeofbirth { get; set; }
        public string dateofbirth { get; set; }
        public int civilstatus { get; set; }
        public string mothermaidenname { get; set; }
        public string motheraddress { get; set; }
        public string companyname { get; set; }
        public string companyaddress { get; set; }
        public string designation { get; set; }
        public int gross_income { get; set; }
        public string reference_name { get; set; }
        public string reference_contactno { get; set; }
        public string bankname { get; set; }
        public string bankaccountno { get; set; }
        public string user_password { get; set; }
        public int city { get; set; }
        public bool ischeck { get; set; }
        public bool isverified { get; set; }
        public bool isrecheck { get; set; }
        public bool isreverified { get; set; }
        public bool isapproved { get; set; }
        public string relativename { get; set; }
        public string coworkername { get; set; }
        public string relationwithrelative { get; set; }
        public string friendname { get; set; }
        public string suffix { get; set; }
        public string street { get; set; }
        public string barangay { get; set; }
        public string zipcode { get; set; }
        public string company_phoneno { get; set; }
        public string friend_contactno { get; set; }
        public string date_joining { get; set; }
        public int termtype { get; set; }
        public string purposeofloan { get; set; }
        public int leadid { get; set; }
        public int paydate1 { get; set; }
        public int paydate2 { get; set; }
        public int term { get; set; }
        public bool isfordisbursement { get; set; }
        public bool isdisbursedexport { get; set; }
        public string CompleteTerm { get; set; }

        //Rashmi


        public IEnumerable<SelectListItem> SuffixList
        {
            get
            {
                return new[]
                {
                    new SelectListItem { Value="-1",Text="Select"},
                    new SelectListItem { Value="1",Text="Mr"},
                    new SelectListItem { Value="2",Text="Mrs"},
                    new SelectListItem { Value="3",Text="Miss"},
                    new SelectListItem { Value="4",Text="Dr"},
                    new SelectListItem { Value="5",Text="Eng"},
                };
            }
        }
        public List<AllRecordDetails> AllRecordDetails { get; set; }

        public AllRecordModel()
        {
            AllRecordDetails = new List<AllRecordDetails>();
        }
    }
    public class AllRecordDetails
    {
        public int applicationno { get; set; }
        public string dateapplied { get; set; }
        public string first_name { get; set; }
        public string middle_name { get; set; }
        public string last_name { get; set; }
        public string name { get; set; }
        public string personalemail { get; set; }
        public string personalcontactno { get; set; }
        public string relativecontactno { get; set; }
        public string coworkercontactno { get; set; }
        public string address { get; set; }
        public string homephoneno { get; set; }
        public string placeofbirth { get; set; }
        public string dateofbirth { get; set; }
        public int civilstatus { get; set; }
        public string mothermaidenname { get; set; }
        public string motheraddress { get; set; }
        public string companyname { get; set; }
        public string companyaddress { get; set; }
        public string designation { get; set; }
        public int gross_income { get; set; }
        public string reference_name { get; set; }
        public string reference_contactno { get; set; }
        public string bankname { get; set; }
        public string bankaccountno { get; set; }
        public string user_password { get; set; }
        public int city { get; set; }
        public bool ischeck { get; set; }
        public bool isverified { get; set; }
        public bool isrecheck { get; set; }
        public bool isreverified { get; set; }
        public bool isapproved { get; set; }
        public string relativename { get; set; }
        public string coworkername { get; set; }
        public string relationwithrelative { get; set; }
        public string friendname { get; set; }
        public string suffix { get; set; }
        public string street { get; set; }
        public string barangay { get; set; }
        public string zipcode { get; set; }
        public string company_phoneno { get; set; }
        public string friend_contactno { get; set; }
        public string date_joining { get; set; }
        public int termtype { get; set; }
        public string purposeofloan { get; set; }
        public int leadid { get; set; }
        public int paydate1 { get; set; }
        public int paydate2 { get; set; }
        public int term { get; set; }
        public bool isfordisbursement { get; set; }
        public bool isdisbursedexport { get; set; }
        public string CompleteTerm { get; set; }

        public string CivilStatusName { get; set; }
        public string CityName { get; set; }

    }
    public class AgingDataReport
    {
        public string Age { get; set; }
        public string DueDate { get; set; }
        public long TotalMaturities { get; set; }
        public decimal TotalMaturities_Peso { get; set; }
        public long TotalPaid { get; set; }
        public decimal TotalPaid_Peso { get; set; }
        public long CollectionEfficiency { get; set; }
        public decimal CollectionEfficiency_Peso { get; set; }
        public long Default { get; set; }
        public decimal Default_Peso { get; set; }
        public string Disbus_new_Loan_number { get; set; }
        public decimal Disbus_new_Loan_amount { get; set; }
        public string Disbus_ReLoan_number { get; set; }
        public decimal Disbus_ReLoan_amount { get; set; }
    }
    public class AgingData
    {
        public string Age { get; set; }
        public string DueDate { get; set; }
        public string DateRange { get; set; }
        public decimal TotalMaturities { get; set; }
        public decimal TotalMaturities_Peso { get; set; }
        public decimal TotalPaid { get; set; }
        public decimal TotalPaid_Peso { get; set; }
        public long CollectionEfficiency { get; set; }
        public decimal CollectionEfficiency_Peso { get; set; }
        public long Default { get; set; }
        public decimal Default_Peso { get; set; }
        public string Disbus_new_Loan_number { get; set; }
        public decimal Disbus_new_Loan_amount { get; set; }
        public string Disbus_ReLoan_number { get; set; }
        public decimal Disbus_ReLoan_amount { get; set; }
        public decimal Reloan { get; set; }
        public decimal Reloan_Peso { get; set; }
        public decimal Preterm { get; set; }
        public decimal Preterm_Peso { get; set; }
        public decimal SumMaturity_No { get; set; }
        public decimal SumMaturity_Peso { get; set; }
        public decimal SumPaid_No { get; set; }
        public decimal SumPaid_Peso { get; set; }
        public decimal SumEffeciency_No { get; set; }
        public decimal SumEffeciency_Peso { get; set; }
        public decimal SumPreterm_No { get; set; }
        public decimal SumPreterm_Peso { get; set; }
        public decimal SumReloan_No { get; set; }
        public decimal SumReloan_Peso { get; set; }
        public string ReportName { get; set; }
        public int OfficerId { get; set; }
        public string OfficerName { get; set; }
        public string DayName { get; set; }

    }
    public class AgingDataVM : AgingData
    {
        public List<AgingData> AgingDataList { get; set; }
        public AgingDataVM()
        {
            AgingDataList = new List<AgingData>();
        }

    }

}