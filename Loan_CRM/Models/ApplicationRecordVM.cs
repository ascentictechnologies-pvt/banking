using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;

namespace Loan_CRM.Models
{
    public class ApplicationRecordVM
    {
        public int Id { get; set; }
        public long AppID { get; set; }
        public int ApplicationNo { get; set; }
        public string DateApplied { get; set; }
        public int view { get; set; }
        [Required(ErrorMessage = "FIRST NAME is Required")]
        [RegularExpression("^[0-9A-Za-z ]+$", ErrorMessage = "Only Characters And Space Allowed, Special Characters Not allowed")]
        public string First_Name { get; set; }
        [RegularExpression(@"^[a-zA-Z ]*$", ErrorMessage = "Only Characters Allowed, Space and Special Characters Not allowed")]
        public string Middle_Name { get; set; }
        [Required(ErrorMessage = "LAST NAME is Required")]
        [RegularExpression(@"^[a-zA-Z ]*$", ErrorMessage = "Only Characters Allowed, Space and Special Characters are Not allowed")]
        public string Last_Name { get; set; }
        [Required(ErrorMessage = "Personal Email Address is Required")]
        public string PersonalEmail { get; set; }
        [Required(ErrorMessage = "Confirm Email Address is Required")]
        [System.ComponentModel.DataAnnotations.Compare("PersonalEmail", ErrorMessage = "Personal Email and Confirm Email doesn't match")]
        public string ConfirmEmail { get; set; }
        [MinLength(6, ErrorMessage = "Length should be greater than 5")]
        [MaxLength(50, ErrorMessage = "Length should be less than 20")]
        [Required(ErrorMessage = "Password is Required")]
        public string Password { get; set; }
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "Password and Confirm Password doesn't match")]
        [MinLength(6, ErrorMessage = "Length should be greater than 5")]
        [MaxLength(20, ErrorMessage = "Length should be less than 20")]
        public string confirm_password { get; set; }
        [Required(ErrorMessage ="Contact Number Required.")]
        [MinLength(9, ErrorMessage = "Please Enter Contact No")]
        [MaxLength(9, ErrorMessage = "Please Enter Contact No")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Only Numbers Allowed")]
        public string PersonalContactNo { get; set; }
        public string ContactPrefix { get; set; }
        [Range(100000000, 999999999, ErrorMessage = "Please Enter Contact No ")]
        public string CompletePersonalContactNo { get; set; }
        public string Personal_ContactNo { get; set; }
        public string RelativeContactNo { get; set; }
        public string CoworkerContactNo { get; set; }
        [Required(ErrorMessage = "Address is Required")]
        public string Address { get; set; }
        [Required(ErrorMessage = "Province Name is Required")]
        public string Province { get; set; }
        public int province_id { get; set; }
        [RegularExpression("^[0-9]*$", ErrorMessage = "Only Numbers Allowed")]
        public string SSS_No { get; set; }
        public int Assignby { get; set; }
        public string AssignbyName { get; set; }
        public string gender { get; set; }
        public string HomePhone { get; set; }
        public string BirthPlace { get; set; }
        public string DOB { get; set; }
        public int CivilStatus { get; set; }
        public string MotherMaidenName { get; set; }
        public string MotherAddress { get; set; }
        [Required(ErrorMessage = "Company Name is Required")]
        public string CompanyName { get; set; }
        [Required(ErrorMessage = "Company Address is Required")]
        public string CompanyAddress { get; set; }
        [RegularExpression("^[0-9A-Za-z ]+$", ErrorMessage = "Only Characters And Space Allowed")]
        public string Designation { get; set; }
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public double GrossIncome { get; set; }
        public string RefName { get; set; }
        public string RefContactNo { get; set; }
        public string BankName { get; set; }
        public string BankAccNo { get; set; }
        [Required(ErrorMessage = "City Name is Required")]
        public string City_Name { get; set; }
        public int City { get; set; }
        public bool Ischeck { get; set; }
        public bool Isverified { get; set; }
        public bool Isrecheck { get; set; }
        public bool Isreverified { get; set; }
        public bool Isapproved { get; set; }
        public string CreatedOn { get; set; }
        public DateTime AppliedOn { get; set; }
        public DateTime ForwardedOn { get; set; }
        public string UpdatedOn { get; set; }
        public string ApprovedOn { get; set; }
        public string RelativeName { get; set; }
        public string CoWorkerName { get; set; }
        public string RelationWithRelative { get; set; }
        public string friendName { get; set; }
        public string GovId_FileName { get; set; }
        public string CompanyId_FileName { get; set; }
        public string BillingId_FileName { get; set; }
        public string Income_FileName { get; set; }
        public string OtherFileName { get; set; }
        public string Atm_File_Name { get; set; }
        public string Suffix { get; set; }
        public string Street { get; set; }
        public int Barangay_id { get; set; }
        [Required(ErrorMessage = "Barangay Name is Required")]
        public string Barangay { get; set; }
        public string ZipCode { get; set; }
        public string Company_Phoneno { get; set; }
        public string Friend_ContactNo { get; set; }
        [Range(0, Int32.MaxValue, ErrorMessage = ("Enter Loan Amount"))]
        public double Loan_Amount { get; set; }
        public HttpPostedFileBase GovIdFile { get; set; }
        public HttpPostedFileBase CompanyIdFile { get; set; }
        public HttpPostedFileBase BillingIdFile { get; set; }
        public HttpPostedFileBase IncomeIdFile { get; set; }
        public HttpPostedFileBase OtherIdFile { get; set; }
        public HttpPostedFileBase AtmIdFile { get; set; }
        public bool ispickedverifier { get; set; }
        public bool ispickedReverifier { get; set; }
        [Range(-1, Int32.MaxValue, ErrorMessage = ("Select Term  Tupe "))]
        public int TermType { get; set; }
        public string Term { get; set; }
        [Range(-1, Int32.MaxValue, ErrorMessage = ("Select Term Text"))]
        public int TermText { get; set; }
        public double NetIncome { get; set; }
        public string PurposeOfLoan { get; set; }
        public bool termAccepted { get; set; }
        public string payDate { get; set; }
        public string JoinDate { get; set; }
        public string username { get; set; }
        public string GovUrl { get; set; }
        public string CompUrl { get; set; }
        public string BillUrl { get; set; }
        public string IncomeUrl { get; set; }
        public string OthUrl { get; set; }
        public string AtmUrl { get; set; }
        [Range(-1, Int32.MaxValue, ErrorMessage = ("Select PayDate1"))]
        public int PayDate1 { get; set; }
        [Range(-1, Int32.MaxValue, ErrorMessage = ("Select PayDate2"))]
        public int PayDate2 { get; set; }
        public string CheckedOn { get; set; }
        public string ReCheckedOn { get; set; }
        public string VerifiedOn { get; set; }
        public string ReVerifiedOn { get; set; }
        [Range(1, Int32.MaxValue, ErrorMessage = ("Select Industry"))]
        public int Occupation { get; set; }
        public string code { get; set; }
        public bool IsPickedApprover { get; set; }
        public string DateRange { get; set; }
        public bool is_selected { get; set; } = false;
        public string Remark { get; set; }
        public string Name { get; set; }
        public string Schedule_Btc { get; set; }
        public string DatePastDue { get; set; }
        public string MaturityDate { get; set; }
        public string DateofLoan { get; set; }
        public double Outstanding_Balance { get; set; }
        public double TotalAmount_Due { get; set; }
        public string PaymentReference { get; set; }
        public string PermanentAddress { get; set; }
        public string BackActionName { get; set; }
        public string EscalateDate { get; set; }
        public string FromDate { get; set; }
        public string checker_remark { get; set; }
        public string ToDate { get; set; }
        public int ViewID { get; set; }
        public IEnumerable<SelectListItem> PayDateList
        {
            get
            {
                return new[]
                {
                    new SelectListItem { Value="0",Text ="Select"},
                    new SelectListItem { Value="1",Text ="1"},
                    new SelectListItem { Value="2",Text ="2"},
                    new SelectListItem { Value="3",Text ="3"},
                    new SelectListItem { Value="4",Text ="4"},
                    new SelectListItem { Value="5",Text ="5"},
                    new SelectListItem { Value="6",Text ="6"},
                    new SelectListItem { Value="7",Text ="7"},
                    new SelectListItem { Value="8",Text ="8"},
                    new SelectListItem { Value="9",Text ="9"},
                    new SelectListItem { Value="10",Text ="10"},
                    new SelectListItem { Value="11",Text ="11"},
                    new SelectListItem { Value="12",Text ="12"},
                    new SelectListItem { Value="13",Text ="13"},
                    new SelectListItem { Value="14",Text ="14"},
                    new SelectListItem { Value="15",Text ="15"},
                    new SelectListItem { Value="16",Text ="16"},
                    new SelectListItem { Value="17",Text ="17"},
                    new SelectListItem { Value="18",Text ="18"},
                    new SelectListItem { Value="19",Text ="19"},
                    new SelectListItem { Value="20",Text ="20"},
                    new SelectListItem { Value="21",Text ="21"},
                    new SelectListItem { Value="22",Text ="22"},
                    new SelectListItem { Value="23",Text ="23"},
                    new SelectListItem { Value="24",Text ="24"},
                    new SelectListItem { Value="25",Text ="25"},
                    new SelectListItem { Value="26",Text ="26"},
                    new SelectListItem { Value="27",Text ="27"},
                    new SelectListItem { Value="28",Text ="28"},
                    new SelectListItem { Value="29",Text ="29"},
                    new SelectListItem { Value="30",Text ="30"},
                    new SelectListItem { Value="31",Text ="31"},
                };
            }
        }
        public IEnumerable<SelectListItem> PayDate2List
        {
            get
            {
                return new[]
                {
                    new SelectListItem { Value="-1",Text ="Select"},
                    new SelectListItem { Value="1",Text ="1"},
                    new SelectListItem { Value="2",Text ="2"},
                    new SelectListItem { Value="3",Text ="3"},
                    new SelectListItem { Value="4",Text ="4"},
                    new SelectListItem { Value="5",Text ="5"},
                    new SelectListItem { Value="6",Text ="6"},
                    new SelectListItem { Value="7",Text ="7"},
                    new SelectListItem { Value="8",Text ="8"},
                    new SelectListItem { Value="9",Text ="9"},
                    new SelectListItem { Value="10",Text ="10"},
                    new SelectListItem { Value="11",Text ="11"},
                    new SelectListItem { Value="12",Text ="12"},
                    new SelectListItem { Value="13",Text ="13"},
                    new SelectListItem { Value="14",Text ="14"},
                    new SelectListItem { Value="15",Text ="15"},
                    new SelectListItem { Value="16",Text ="16"},
                    new SelectListItem { Value="17",Text ="17"},
                    new SelectListItem { Value="18",Text ="18"},
                    new SelectListItem { Value="19",Text ="19"},
                    new SelectListItem { Value="20",Text ="20"},
                    new SelectListItem { Value="21",Text ="21"},
                    new SelectListItem { Value="22",Text ="22"},
                    new SelectListItem { Value="23",Text ="23"},
                    new SelectListItem { Value="24",Text ="24"},
                    new SelectListItem { Value="25",Text ="25"},
                    new SelectListItem { Value="26",Text ="26"},
                    new SelectListItem { Value="27",Text ="27"},
                    new SelectListItem { Value="28",Text ="28"},
                    new SelectListItem { Value="29",Text ="29"},
                    new SelectListItem { Value="30",Text ="30"},
                    new SelectListItem { Value="31",Text ="31"},
                    new SelectListItem { Value="32",Text="None"}
                };
            }
        }
        public IEnumerable<SelectListItem> BestTimeToCallMorning
        {
            get
            {
                return new[]
                {
                    new SelectListItem { Value="-1",Text ="Select"},
                    new SelectListItem { Value="1",Text ="10-11am"},
                    new SelectListItem { Value="2",Text ="11am-12pm"}
                };
            }
        }
        public IEnumerable<SelectListItem> BestTimeToCallNoon
        {
            get
            {
                return new[]
                {
                    new SelectListItem { Value="-1",Text="Select"},
                    new SelectListItem { Value="1",Text="12-1pm"},
                    new SelectListItem { Value="2",Text="1-2pm"},
                    new SelectListItem { Value="3",Text="2-3pm"},
                    new SelectListItem { Value="4",Text="4-5pm"},
                    new SelectListItem { Value="5",Text="6-7pm"},
                    new SelectListItem { Value="6",Text="7-8pm"}
                };
            }
        }

        public int BestTimeToCallMorning_Id { get; set; }
        public int BestTimeToCallNoon_Id { get; set; }
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
        public IEnumerable<SelectListItem> GenderList
        {
            get
            {
                return new[]
                {
                            new SelectListItem { Value="0",Text ="Select"},
                            new SelectListItem { Value="Male",Text ="Male"},
                            new SelectListItem { Value="Female",Text ="Female"}
                        };
            }
        }
        public List<ApplicationRecordVM> ApplicationRecordList { get; set; }
        public List<ApplicationList> ApplicationList { get; set; }
        public List<OccupationModel> OccupationList { get; set; }
        public List<ProvinceModel> ProvinceList { get; set; }
        public List<BarangayModel> BarangayList { get; set; }
        public List<City> CityList { get; set; }
        public List<ApplicationRecordVM> UserList { get; set; }
        public string MorningTime { get; internal set; }
        public string NoonTime { get; internal set; }
        public string userfullname { get; set; }
        public int userfrom { get; set; }
        public int userto { get; set; }
        public string reference_no { get; set; }
        public string contract_no { get; set; }
        public string signed { get; set; }
        public int user_id { get; set; }
        public int record_count { get; set; } = 10;
        public string ApplicantFullName { get; set; }
        public List<BankMasterModel> BankMasterModelList { get; set; }
        public ApplicationRecordVM()
        {
            ApplicationList = new List<Models.ApplicationList>();
            OccupationList = new List<Models.OccupationModel>();
            CityList = new List<City>();
            BarangayList = new List<BarangayModel>();
            ProvinceList = new List<ProvinceModel>();
            ApplicationRecordList = new List<ApplicationRecordVM>();
            BankMasterModelList = new List<BankMasterModel>();
            UserList = new List<ApplicationRecordVM>();
        }

    }
    public class ApplicationList
    {
        public int ApplicationNo { get; set; }
        public string DateApplied { get; set; }
        public string Name { get; set; }
        public string PersonalEmail { get; set; }
        public string PersonalContactNo { get; set; }
        public bool ispickedverifier { get; set; }
        public bool ispickedReverifier { get; set; }
    }
    public class PartialModel
    {
        public Double balanceamount { get; set; }
        public int emi_details_id { get; set; }
        public Double paidamount { get; set; }
    }

}

