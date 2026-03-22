using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;

namespace Loan_CRM.Models
{
    public class CompleteAppVerificationDetails
    {
        public string transactionid { get; set; }

        public int ApplicationNo { get; set; }
        public string gov_doc_type { get; set; }
        public string gender { get; set; }
        public string AdditionalLoanPurpose { get; set; }
        public string AdditionalLoanPurpose_Remark { get; set; }
        public string AdditionalRequested_Term { get; set; }
        public double ApprovedLoanAmount { get; set; }
        public double ApprovedTerm { get; set; }
        public double ApprovedInterestRate { get; set; }
        public string ApprovedMaturityDate { get; set; }
        public double ApprovedTotalDueAmount { get; set; }
        public String personal_name_remark { get; set; }
        public String personal_email_remark { get; set; }
        public String personal_contact_no_remark { get; set; }
        public String personal_perma_address_remark { get; set; }
        public String personal_house_ph_remark { get; set; }
        public String personal_birth_date_remark { get; set; }
        public string personal_civil_remark { get; set; }
        public String personal_mother_maiden_name_remark { get; set; }
        public String personal_mother_perma_add_remark { get; set; }
        public String personal_company_name_remark { get; set; }
        public String personal_company_address_remark { get; set; }
        public String personal_job_title_remark { get; set; }
        public String personal_monthly_income_remark { get; set; }
        public String personal_bank_name_remark { get; set; }
        public String personal_bank_ac_no_remark { get; set; }
        public String personal_req_loan_amt_remark { get; set; }
        public String personal_req_loan_term_remark { get; set; }
        public string nearest_landmark_remark { get; set; }
        public string SSS_No_remark { get; set; }
        public string rented_mortgage_owned_remark { get; set; }
        public string Permanent_address_remark { get; set; }
        public string tansfer_residence_remark { get; set; }
        public string spouse_name_remark { get; set; }
        public string spouse_occupation_remark { get; set; }
        public string number_of_dependent_remark { get; set; }
        public string father_name_work_remark { get; set; }
        public string sibling_works_remark { get; set; }
        public string occupation_remark { get; set; }
        public string net_income_remark { get; set; }
        public string scheduled_and_btc_remark { get; set; }
        public string pay_date_remark { get; set; }
        public string pending_resignation_remark { get; set; }
        public string other_source_of_income_remark { get; set; }
        public string know_about_cashmart_remark { get; set; }
        public string pending_loan_fron_otherland_remark { get; set; }
        public string bank_loan_or_credit_card_remark { get; set; }
        public string family_name1_remark { get; set; }
        public string family_address1_remark { get; set; }
        public string family_contact1_remark { get; set; }
        public string family_relation1_remark { get; set; }
        public string family_name2_remark { get; set; }
        public string family_address2_remark { get; set; }
        public string family_contact2_remark { get; set; }
        public string family_relation2_remark { get; set; }
        public string optional_name_remark { get; set; }
        public string optional_address_remark { get; set; }
        public string optional_contact_remark { get; set; }
        public string optional_relation_remark { get; set; }
        public string Name { get; set; }
        public string PersonalEmail { get; set; }
        public string PersonalContactNo { get; set; } = "";
        public string Address { get; set; }
        public string HomePhone { get; set; }
        public string DOB { get; set; }
        public int CivilStatus { get; set; }
        public string CivilStatusText { get; set; }
        public string MotherMaidenName { get; set; }
        public string MotherAddress { get; set; }
        public string CompanyName { get; set; }
        public string CompanyAddress { get; set; }
        public string Designation { get; set; }
        public double GrossIncome { get; set; }
        public string BankName { get; set; }
        public string BankAccNo { get; set; }
        public double Loan_Amount { get; set; }
        public string payDate1 { get; set; }
        public string payDate2 { get; set; }
        public string GovUrl { get; set; } = "";
        public string completeaddress { get; set; } = "";
        public string CompUrl { get; set; } = "";
        public string BillUrl { get; set; } = "";
        public string IncomeUrl { get; set; } = "";
        public string OthUrl { get; set; } = "";
        public string ATMUrl { get; set; } = "";
        public string nearest_landmark { get; set; }
        public string SSS_No { get; set; }
        public string Permanent_address { get; set; }
        public string tansfer_residence { get; set; } = "";
        public string spouse_name { get; set; }
        public string spouse_occupation { get; set; }
        public int number_of_dependent { get; set; }
        public string mother_work { get; set; }
        public string father_name { get; set; }
        public string father_work { get; set; }
        public string living_with_mother { get; set; }
        public int sibling_count { get; set; }
        public string sibling_works { get; set; }
        public string occupation { get; set; }
        public double net_income { get; set; }
        public string MorningTime { get; internal set; }
        public string NoonTime { get; internal set; }
        public string pending_resignation { get; set; }
        public string other_source_of_income { get; set; }
        public string know_about_cashmart { get; set; }
        public string pending_loan_fron_otherland { get; set; }
        public string bank_loan_or_credit_card { get; set; }
        public string family_name1 { get; set; }
        public string family_address1 { get; set; }
        public string family_contact1 { get; set; }
        public string family_relation1 { get; set; }
        public string family_name2 { get; set; }
        public string family_address2 { get; set; }
        public string family_contact2 { get; set; }
        public string family_relation2 { get; set; }
        public string optional_name { get; set; }
        public string optional_address { get; set; }
        public string optional_contact { get; set; }
        public string optional_relation { get; set; }
        public string checker_name { get; set; }
        public int home_status { get; set; }
        public string home_status_text { get; set; }
        public string disbursement_date { get; set; }
        public string notificationtoken { get; set; }
        public string templatename { get; set; }
        public List<BankMasterModel> BankMasterModelList { get; set; }
        public List<OccupationModel> OccupationModelList { get; set; }
        public List<RemarkModel> RemarkModelList { get; set; }
        public IEnumerable<SelectListItem> YesNoList
        {
            get
            {
                return new[]
                {
                            new SelectListItem { Value="0",Text ="Select"},
                            new SelectListItem { Value="No",Text ="No"},
                            new SelectListItem { Value="Yes",Text ="Yes"}
                        };
            }
        }

        public IEnumerable<SelectListItem> GovDocList
        {
            get
            {
                return new[]
                {
                            new SelectListItem { Value="0",Text ="Select"},
                            new SelectListItem { Value="SSS",Text ="SSS"},
                            new SelectListItem { Value="GSIS",Text ="GSIS"},
                            new SelectListItem { Value="TIN",Text ="TIN"},
                            new SelectListItem{Value="None",Text="None"}
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
        public IEnumerable<SelectListItem> HomeStatus
        {
            get
            {
                return new[]
                {
                            new SelectListItem { Value="0",Text ="Select"},
                            new SelectListItem { Value="1",Text ="Owned"},
                            new SelectListItem { Value="2",Text ="Renting less than 1yr"},
                            new SelectListItem { Value="3",Text="Renting for 2yrs"},
                            new SelectListItem { Value="4",Text="Renting for 3yrs & above"},
                             new SelectListItem { Value="5",Text="Living with parents/relatives"},
                             new SelectListItem { Value="6",Text="Living with live-in partner"}
                        };
            }
        }
        public IEnumerable<SelectListItem> PayDateList
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
        new SelectListItem { Value="31",Text ="31"}
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
        public string CityName { get; set; }
        public int userid { get; set; }
        public string jumioreference { get; set; }
        public string ProvinceName { get; set; }
        public string BarangayName { get; set; }
        public string TermType { get; set; }
        public int TermName { get; set; }

        public int ApprovedTermId { get; set; }
        public string signUrl { get; set; }
        public int verification_id { get; set; }
        public int application_no { get; set; }
        public bool personal_name_id1_status { get; set; }
        public bool personal_name_id2_status { get; set; }
        public bool personal_name_pi_status { get; set; }
        public bool personal_name_pb_status { get; set; }
        public bool personal_email_id1_status { get; set; }
        public bool personal_email_id2_status { get; set; }
        public bool personal_email_pi_status { get; set; }
        public bool personal_email_pb_status { get; set; }
        public bool personal_contact_no_id1_status { get; set; }
        public bool personal_contact_no_id2_status { get; set; }
        public bool personal_contact_no_pi_status { get; set; }
        public bool personal_contact_no_pb_status { get; set; }
        public bool personal_perma_address_id1_status { get; set; }
        public bool personal_perma_address_id2_status { get; set; }
        public bool personal_perma_address_pi_status { get; set; }
        public bool personal_perma_address_pb_status { get; set; }
        public bool personal_house_ph_id1_status { get; set; }
        public bool personal_house_ph_id2_status { get; set; }
        public bool personal_house_ph_pi_status { get; set; }
        public bool personal_house_ph_pb_status { get; set; }
        public bool personal_birth_place_id1_status { get; set; }
        public bool personal_birth_place_id2_status { get; set; }
        public bool personal_birth_place_pi_status { get; set; }
        public bool personal_birth_place_pb_status { get; set; }
        public bool personal_birth_date_id1_status { get; set; }
        public bool personal_birth_date_id2_status { get; set; }
        public bool personal_birth_date_pi_status { get; set; }
        public bool personal_birth_date_pb_status { get; set; }
        public bool personal_civil_id1_status { get; set; }
        public bool personal_civil_id2_status { get; set; }
        public bool personal_civil_pi_status { get; set; }
        public bool personal_civil_pb_status { get; set; }
        public bool personal_mother_maiden_name_id1_status { get; set; }
        public bool personal_mother_maiden_name_id2_status { get; set; }
        public bool personal_mother_maiden_name_pi_status { get; set; }
        public bool personal_mother_maiden_name_pb_status { get; set; }
        public bool personal_mother_perma_add_id1_status { get; set; }
        public bool personal_mother_perma_add_id2_status { get; set; }
        public bool personal_mother_perma_add_pi_status { get; set; }
        public bool personal_mother_perma_add_pb_status { get; set; }
        public bool personal_company_name_id1_status { get; set; }
        public bool personal_company_name_id2_status { get; set; }
        public bool personal_company_name_pi_status { get; set; }
        public bool personal_company_name_pb_status { get; set; }
        public bool personal_company_address_id1_status { get; set; }
        public bool personal_company_address_id2_status { get; set; }
        public bool personal_company_address_pi_status { get; set; }
        public bool personal_company_address_pb_status { get; set; }
        public bool personal_job_title_id1_status { get; set; }
        public bool personal_job_title_id2_status { get; set; }
        public bool personal_job_title_pi_status { get; set; }
        public bool personal_job_title_pb_status { get; set; }
        public bool personal_monthly_income_id1_status { get; set; }
        public bool personal_monthly_income_id2_status { get; set; }
        public bool personal_monthly_income_pi_status { get; set; }
        public bool personal_monthly_income_pb_status { get; set; }
        public bool personal_reference_name_id1_status { get; set; }
        public bool personal_reference_name_id2_status { get; set; }
        public bool personal_reference_name_pi_status { get; set; }
        public bool personal_reference_name_pb_status { get; set; }
        public bool personal_reference_contact_id1_status { get; set; }
        public bool personal_reference_contact_id2_status { get; set; }
        public bool personal_reference_contact_pi_status { get; set; }
        public bool personal_reference_contact_pb_status { get; set; }
        public bool personal_bank_name_id1_status { get; set; }
        public bool personal_bank_name_id2_status { get; set; }
        public bool personal_bank_name_pi_status { get; set; }
        public bool personal_bank_name_pb_status { get; set; }
        public bool personal_bank_ac_no_id1_status { get; set; }
        public bool personal_bank_ac_no_id2_status { get; set; }
        public bool personal_bank_ac_no_pi_status { get; set; }
        public bool personal_bank_ac_no_pb_status { get; set; }
        public bool personal_req_loan_amt_id1_status { get; set; }
        public bool personal_req_loan_amt_id2_status { get; set; }
        public bool personal_req_loan_amt_pi_status { get; set; }
        public bool personal_req_loan_amt_pb_status { get; set; }
        public bool personal_req_loan_term_id1_status { get; set; }
        public bool personal_req_loan_term_id2_status { get; set; }
        public bool personal_req_loan_term_pi_status { get; set; }
        public bool personal_req_loan_term_pb_status { get; set; }
        public double AdditionalRequestedLoanAmount { get; set; }
        public double AdditionalRequestedTerm { get; set; }
        public int DetailId { get; set; }
        public string ApprovedTermType { get; set; }
        public int RequestedAmt { get; set; }
        public int RequestedTerms { get; set; }
        public bool Id1_Status { get; set; }
        public bool Id2_Status { get; set; }
        public bool Pi_Status { get; set; }
        public bool Pb_Status { get; set; }
        public string Remarks { get; set; }
        public string AdditionalBirthPlace { get; set; }
        public string AdditionalProvincialAddress { get; set; }
        public string AdditionalEmployedDuration { get; set; }
        public string AdditionalJobPosition { get; set; }
        public string AdditionalJobLevel { get; set; }
        public string AdditionalPayDate { get; set; }
        public bool FamilyPersonalNameStatus { get; set; }
        public bool FamilyPersonalContactStatus { get; set; }
        public bool FamilyRelationWithBorrowerStatus { get; set; }
        public bool FamilyBorrowerKnownDurationStatus { get; set; }
        public bool FamilyAddressVerificationStatus { get; set; }
        public bool FamilyBorrowerWorkingPlaceStatus { get; set; }
        public bool FriendPersonalNameStatus { get; set; }
        public bool FriendPersonalContactStatus { get; set; }
        public bool FriendRelationWithBorrowerStatus { get; set; }
        public bool FriendBorrowerKnownDurationStatus { get; set; }
        public bool FriendAddressVerificationStatus { get; set; }
        public bool FriendBorrowerWorkingPlaceStatus { get; set; }
        public bool CoWorkerPersonalNameStatus { get; set; }
        public bool CoWorkerPersonalContactStatus { get; set; }
        public bool CoWorkerRelationWithBorrowerStatus { get; set; }
        public bool CoWorkerBorrowerKnownDurationStatus { get; set; }
        public bool CoWorkerAddressVerificationStatus { get; set; }
        public bool CoWorkerBorrowerWorkingPlaceStatus { get; set; }
        public bool EmploymentNameOfWorkContactStatus { get; set; }
        public bool EmploymentNoOfWorkConatctStatus { get; set; }
        public bool EmploymentBorrowerWorkingStatus { get; set; }
        public bool EmploymentBorrowerPositionStatus { get; set; }
        public bool EmploymentBorrowerMonthlySalaryStatus { get; set; }
        public bool EmploymentBorrowerAttendanceStatus { get; set; }
        public bool EmploymentBorrowerBankPayrollStatus { get; set; }
        public string FamilyPersonalName { get; set; }
        public string FamilyPersonalContact { get; set; }
        public string FamilyRelationWithBorrower { get; set; }
        public string FamilyBorrowerKnownDuration { get; set; }
        public string FamilyAddressVerification { get; set; }
        public string FamilyBorrowerWorkingPlace { get; set; }
        public string FriendPersonalName { get; set; }
        public string FriendPersonalContact { get; set; }
        public string FriendRelationWithBorrower { get; set; }
        public string FriendBorrowerKnownDuration { get; set; }
        public string FriendAddressVerification { get; set; }
        public string FriendBorrowerWorkingPlace { get; set; }
        public string CoWorkerPersonalName { get; set; }
        public string CoWorkerPersonalConatact { get; set; }
        public string CoWorkerRelationWithBorrower { get; set; }
        public string CoWorkerBorrowerKnownDuration { get; set; }
        public string CoWorkerAddressVerification { get; set; }
        public string CoWorkerBorrowerWorkingPlace { get; set; }
        public string EmploymentNameOfWorkContact { get; set; }
        public string EmploymentNoOfWorkConatct { get; set; }
        public string EmploymentBorrowerWorking { get; set; }
        public string EmploymentBorrowerPosition { get; set; }
        public string EmploymentBorrowerMonthlySalary { get; set; }
        public string EmploymentBorrowerAttendance { get; set; }
        public string EmploymentBorrowerBankPayroll { get; set; }
        public bool Question1_Status { get; set; }
        public bool Question2_Status { get; set; }
        public bool Question3_Status { get; set; }
        public bool Question4_Status { get; set; }
        public bool Question5_Status { get; set; }
        public bool Question6_Status { get; set; }
        public bool Question7_Status { get; set; }
        public bool Question8_Status { get; set; }
        public bool Question9_Status { get; set; }
        public bool Question10_Status { get; set; }
        public String personal_reference_name_remark { get; set; }
        public String personal_reference_contact_remark { get; set; }
        public String personal_birth_place_remark { get; set; }
        public int checked_by { get; set; }
        public int verify_by { get; set; }
        public int approved_by { get; set; }
        public string JobTitle { get; set; }
        public string RefName { get; set; }
        public string DateApplied { get; set; }
        public string Password { get; set; }
        public string RelativeContactNo { get; set; }
        public string CoworkerContactNo { get; set; }
        public string BirthPlace { get; set; }
        public string RefContactNo { get; set; }
        public int City { get; set; }
        public bool Ischeck { get; set; }
        public bool Isverified { get; set; }
        public bool Isrecheck { get; set; }
        public bool Isreverified { get; set; }
        public bool Isapproved { get; set; }
        public string CreatedOn { get; set; }
        public string UpdatedOn { get; set; }
        public string RelativeName { get; set; }
        public string CoWorkerName { get; set; }
        public string RelationWithRelative { get; set; }
        public string friendName { get; set; }
        public string GovId_FileName { get; set; }
        public string CompanyId_FileName { get; set; }
        public string BillingId_FileName { get; set; }
        public string Income_FileName { get; set; }
        public string OtherFileName { get; set; }
        public string Suffix { get; set; }
        public string Street { get; set; }
        public string Barangay { get; set; }
        public string ZipCode { get; set; }
        public string Company_Phoneno { get; set; }
        public string Friend_ContactNo { get; set; }
        public HttpPostedFileBase GovIdFile { get; set; }
        public HttpPostedFileBase CompanyIdFile { get; set; }
        public HttpPostedFileBase BillingIdFile { get; set; }
        public HttpPostedFileBase IncomeIdFile { get; set; }
        public HttpPostedFileBase OtherIdFile { get; set; }
        public HttpPostedFileBase AtmIdFile { get; set; }
        public bool ispickedverifier { get; set; }
        public string Term { get; set; }
        public bool termAccepted { get; set; }
        public string JoinDate { get; set; }
        public string Province { get; set; }
        public string facemapstatus { get; set; }
        public int term_value { get; set; }
        public decimal interest_rate { get; set; }
        public decimal late_rate { get; set; }
        public decimal lateFee { get; set; }
        public string ExistData { get; set; }
        public string username { get; set; }
        public int view { get; set; }
        public string BackActionName { get; set; }
        public List<CompleteAppVerificationDetails> verifiererinformationslist { get; set; }
        public string add_personalcontactno { get; set; }
        public string add_company_phoneno { get; set; }
        public List<int> LeadCount_verifier { get; set; }
        public List<int> LeadCount_VerifyPushed_verifier { get; set; }
        public List<UserForVerifierReport> UserForVerifierReportList { get; set; }
        [Required(ErrorMessage = "Required")]
        public DateTime From_Date { get; set; }
        [Required(ErrorMessage = "Required")]
        public DateTime End_Date { get; set; }
        public string verifier_name { get; set; }
        public string approver_name { get; set; }
        public string checker_oic_on { get; set; }
        public string verifier_oic_on { get; set; }
        public string approver_oic_on { get; set; }
        public int BankId { get; set; }
        public CredoModel CredoModel { get; set; }
        public CompleteAppVerificationDetails()
        {
            BankMasterModelList = new List<BankMasterModel>();
            RemarkModelList = new List<RemarkModel>();
            OccupationModelList = new List<OccupationModel>();
            verifiererinformationslist = new List<CompleteAppVerificationDetails>();
            LeadCount_verifier = new List<int>();
            LeadCount_VerifyPushed_verifier = new List<int>();
            UserForVerifierReportList = new List<UserForVerifierReport>();
            CredoModel = new CredoModel();
        }
    }
    public class UserForVerifierReport
    {
        public int user_id { get; set; }
        public string user_name { get; set; }
    }
}

