using System;
using System.Configuration;
using System.Data;

namespace Loan_CRM.Models
{
    public class ApproverCommonOperation
    {
        public CompleteAppVerificationDetails GetApproverData(int id)
        {
            try
            {
                CompleteAppVerificationDetails appRecordVM = new CompleteAppVerificationDetails();
                DataTable dt = DbHelper.SelectMethod(String.Format(QueryHelper.GetDetailsforApprover, id));
                if (dt != null && dt.Rows.Count > 0)
                {
                    int Verify_by = Convert.ToString(dt.Rows[0]["Verify_by"]) != string.Empty ? Convert.ToInt32(dt.Rows[0]["Verify_by"]) : 0;
                    if (Verify_by > 0)
                    {
                        DataTable dtVerify = DbHelper.SelectMethod(string.Format("Select username, userfullname from ct_user where userid='{0}'", Verify_by));
                        if (dtVerify != null && dtVerify.Rows.Count > 0)
                        {
                            appRecordVM.username = Convert.ToString(dtVerify.Rows[0]["username"]);
                        }
                    }
                    string TermType = dt.Rows[0]["termtype"].ToString() != string.Empty ? Convert.ToString(dt.Rows[0]["termtype"]) : string.Empty;
                    if (!String.IsNullOrWhiteSpace(TermType))
                        TermType = GetTermType(TermType);
                    string TermName = dt.Rows[0]["term"].ToString() != string.Empty ? Convert.ToString(dt.Rows[0]["term"]) : string.Empty;
                    if (!String.IsNullOrWhiteSpace(TermName))
                        TermName = GetTermName(TermName);

                    string AdditionalRequestTermStr = TermType + ", " + TermName;

                    appRecordVM.GovUrl = Convert.ToString(dt.Rows[0]["gov_id_url"]);
                    appRecordVM.CompUrl = Convert.ToString(dt.Rows[0]["companyid_url"]);
                    appRecordVM.BillUrl = Convert.ToString(dt.Rows[0]["billing_url"]);
                    appRecordVM.IncomeUrl = Convert.ToString(dt.Rows[0]["income_url"]);
                    appRecordVM.OthUrl = Convert.ToString(dt.Rows[0]["other_url"]);
                    appRecordVM.ATMUrl = Convert.ToString(dt.Rows[0]["atm_url"]);
                    appRecordVM.userid = Convert.ToInt32(dt.Rows[0]["user_id"]);
                    appRecordVM.jumioreference = Convert.ToString(dt.Rows[0]["jumioreference"]);
                    appRecordVM.facemapstatus = Convert.ToString(dt.Rows[0]["facemapstatus"]);
                    appRecordVM.signUrl = Convert.ToString(dt.Rows[0]["signurl"]);
                    appRecordVM.ApplicationNo = dt.Rows[0]["applicationno"].ToString().Trim() != string.Empty ? Convert.ToInt32(dt.Rows[0]["applicationno"].ToString()) : 0;
                    appRecordVM.DateApplied = Convert.ToString(dt.Rows[0]["dateapplied"]);
                    var firstName = dt.Rows[0]["first_name"].ToString() != string.Empty ? dt.Rows[0]["first_name"].ToString() : string.Empty;
                    var middleName = dt.Rows[0]["middle_name"].ToString() != string.Empty ? dt.Rows[0]["middle_name"].ToString() : string.Empty;
                    var lastName = dt.Rows[0]["last_name"].ToString() != string.Empty ? dt.Rows[0]["last_name"].ToString() : string.Empty;
                    appRecordVM.Name = firstName + " " + middleName + " " + lastName;
                    appRecordVM.PersonalEmail = dt.Rows[0]["personalemail"].ToString().Trim() != string.Empty ? dt.Rows[0]["personalemail"].ToString() : string.Empty;
                    appRecordVM.PersonalContactNo = dt.Rows[0]["personalcontactno"].ToString().Trim() != string.Empty ? dt.Rows[0]["personalcontactno"].ToString() : string.Empty;
                    appRecordVM.RelativeContactNo = dt.Rows[0]["relativecontactno"].ToString().Trim() != string.Empty ? dt.Rows[0]["relativecontactno"].ToString() : string.Empty;
                    appRecordVM.CoworkerContactNo = dt.Rows[0]["coworkercontactno"].ToString().Trim() != string.Empty ? dt.Rows[0]["coworkercontactno"].ToString() : string.Empty;
                    appRecordVM.Address = dt.Rows[0]["address"].ToString().Trim() != string.Empty ? dt.Rows[0]["address"].ToString() : string.Empty;
                    appRecordVM.HomePhone = dt.Rows[0]["homephoneno"].ToString().Trim() != string.Empty ? dt.Rows[0]["homephoneno"].ToString() : string.Empty;
                    appRecordVM.BirthPlace = dt.Rows[0]["placeofbirth"].ToString().Trim() != string.Empty ? dt.Rows[0]["placeofbirth"].ToString() : string.Empty;
                    appRecordVM.DOB = Convert.ToString(dt.Rows[0]["dateofbirth"]) != string.Empty ? Convert.ToDateTime(dt.Rows[0]["dateofbirth"]).ToShortDateString() : null;
                    appRecordVM.CivilStatus = dt.Rows[0]["civilstatus"].ToString().Trim() != string.Empty ? Convert.ToInt32(dt.Rows[0]["civilstatus"].ToString()) : 0;
                    appRecordVM.CivilStatusText = getCivilStatus(appRecordVM.CivilStatus);

                    appRecordVM.MotherMaidenName = dt.Rows[0]["mothermaidenname"].ToString().Trim() != string.Empty ? dt.Rows[0]["mothermaidenname"].ToString() : string.Empty;
                    appRecordVM.MotherAddress = dt.Rows[0]["motheraddress"].ToString().Trim() != string.Empty ? dt.Rows[0]["motheraddress"].ToString() : string.Empty;
                    appRecordVM.CompanyName = dt.Rows[0]["companyname"].ToString().Trim() != string.Empty ? dt.Rows[0]["companyname"].ToString() : string.Empty;
                    appRecordVM.CompanyAddress = dt.Rows[0]["companyaddress"].ToString().Trim() != string.Empty ? dt.Rows[0]["companyaddress"].ToString() : string.Empty;
                    appRecordVM.Designation = dt.Rows[0]["designation"].ToString().Trim() != string.Empty ? dt.Rows[0]["designation"].ToString() : string.Empty;
                    appRecordVM.GrossIncome = dt.Rows[0]["gross_income"].ToString().Trim() != string.Empty ? Convert.ToDouble(dt.Rows[0]["gross_income"].ToString()) : 0.00;
                    appRecordVM.RefName = dt.Rows[0]["reference_name"].ToString().Trim() != string.Empty ? dt.Rows[0]["reference_name"].ToString() : string.Empty;
                    appRecordVM.RefContactNo = dt.Rows[0]["reference_contactno"].ToString().Trim() != string.Empty ? dt.Rows[0]["reference_contactno"].ToString() : string.Empty;
                    appRecordVM.BankName = dt.Rows[0]["bankname"].ToString().Trim() != string.Empty ? dt.Rows[0]["bankname"].ToString() : string.Empty;
                    appRecordVM.BankAccNo = dt.Rows[0]["bankaccountno"].ToString().Trim() != string.Empty ? dt.Rows[0]["bankaccountno"].ToString() : string.Empty;
                    appRecordVM.Password = dt.Rows[0]["user_password"].ToString().Trim() != string.Empty ? dt.Rows[0]["user_password"].ToString() : string.Empty;
                    appRecordVM.City = dt.Rows[0]["city"].ToString().Trim() != string.Empty ? Convert.ToInt32(dt.Rows[0]["city"].ToString()) : 0;
                    if (appRecordVM.City != 0)
                    {
                        appRecordVM.CityName = CommonMethods.GetCityName(appRecordVM.City);
                    }
                    else
                    { appRecordVM.CityName = ""; }

                    appRecordVM.Province = dt.Rows[0]["province"].ToString() != string.Empty ? dt.Rows[0]["province"].ToString() : string.Empty;
                    if (appRecordVM.Province != "")
                    {
                        int ProvinceId = 0;
                        ProvinceId = Convert.ToInt32(appRecordVM.Province);
                        appRecordVM.ProvinceName = CommonMethods.GetProvinceName(ProvinceId);
                    }

                    else
                    { appRecordVM.BarangayName = ""; }
                    appRecordVM.Ischeck = dt.Rows[0]["ischeck"].ToString().Trim() != string.Empty ? Convert.ToBoolean(dt.Rows[0]["ischeck"].ToString()) : false;
                    appRecordVM.Isverified = dt.Rows[0]["isverified"].ToString().Trim() != string.Empty ? Convert.ToBoolean(dt.Rows[0]["isverified"].ToString()) : false;
                    appRecordVM.Isrecheck = dt.Rows[0]["isrecheck"].ToString().Trim() != string.Empty ? Convert.ToBoolean(dt.Rows[0]["isrecheck"].ToString()) : false;
                    appRecordVM.Isreverified = dt.Rows[0]["isreverified"].ToString().Trim() != string.Empty ? Convert.ToBoolean(dt.Rows[0]["isreverified"].ToString()) : false;

                    appRecordVM.Isapproved = dt.Rows[0]["isapproved"].ToString().Trim() != string.Empty ? Convert.ToBoolean(dt.Rows[0]["isapproved"].ToString()) : false;
                    appRecordVM.RelativeName = dt.Rows[0]["relativename"].ToString().Trim() != string.Empty ? dt.Rows[0]["relativename"].ToString() : string.Empty;
                    appRecordVM.CoWorkerName = dt.Rows[0]["coworkername"].ToString().Trim() != string.Empty ? dt.Rows[0]["coworkername"].ToString() : string.Empty;
                    appRecordVM.RelationWithRelative = dt.Rows[0]["relationwithrelative"].ToString().Trim() != string.Empty ? dt.Rows[0]["relationwithrelative"].ToString() : string.Empty;
                    appRecordVM.friendName = dt.Rows[0]["friendname"].ToString().Trim() != string.Empty ? dt.Rows[0]["friendname"].ToString() : string.Empty;

                    appRecordVM.Suffix = dt.Rows[0]["suffix"].ToString().Trim() != string.Empty ? dt.Rows[0]["suffix"].ToString() : string.Empty;
                    appRecordVM.Street = dt.Rows[0]["street"].ToString().Trim() != string.Empty ? dt.Rows[0]["street"].ToString() : string.Empty;
                    appRecordVM.Barangay = dt.Rows[0]["barangay"].ToString().Trim() != string.Empty ? dt.Rows[0]["barangay"].ToString() : string.Empty;

                    if (appRecordVM.Barangay != "")
                    {
                        int BarangayId = 0;
                        BarangayId = Convert.ToInt32(appRecordVM.Barangay);
                        appRecordVM.BarangayName = CommonMethods.GetBarangayName(BarangayId);
                    }

                    else
                    { appRecordVM.BarangayName = ""; }

                    appRecordVM.ZipCode = dt.Rows[0]["zipcode"].ToString().Trim() != string.Empty ? dt.Rows[0]["zipcode"].ToString() : string.Empty;
                    appRecordVM.Company_Phoneno = dt.Rows[0]["company_phoneno"].ToString().Trim() != string.Empty ? dt.Rows[0]["company_phoneno"].ToString() : string.Empty;
                    appRecordVM.Friend_ContactNo = dt.Rows[0]["friend_contactno"].ToString().Trim() != string.Empty ? dt.Rows[0]["friend_contactno"].ToString() : string.Empty;
                    appRecordVM.ispickedverifier = dt.Rows[0]["ispickedverifier"].ToString().Trim() != string.Empty ? Convert.ToBoolean(dt.Rows[0]["ispickedverifier"]) : false;
                    appRecordVM.Loan_Amount = dt.Rows[0]["loanamount"].ToString().Trim() != string.Empty ? Convert.ToInt32(dt.Rows[0]["loanamount"]) : 0;
                    appRecordVM.Term = AdditionalRequestTermStr;

                    appRecordVM.personal_name_id1_status = dt.Rows[0]["personal_name_id1_status"].ToString().Trim() != string.Empty ? Convert.ToBoolean(dt.Rows[0]["personal_name_id1_status"]) : false;
                    appRecordVM.personal_name_id2_status = dt.Rows[0]["personal_name_id2_status"].ToString().Trim() != string.Empty ? Convert.ToBoolean(dt.Rows[0]["personal_name_id2_status"]) : false;
                    appRecordVM.personal_name_pi_status = dt.Rows[0]["personal_name_pi_status"].ToString().Trim() != string.Empty ? Convert.ToBoolean(dt.Rows[0]["personal_name_pi_status"]) : false;
                    appRecordVM.personal_name_pb_status = dt.Rows[0]["personal_name_pb_status"].ToString().Trim() != string.Empty ? Convert.ToBoolean(dt.Rows[0]["personal_name_pb_status"]) : false;
                    appRecordVM.personal_email_id1_status = dt.Rows[0]["personal_email_id1_status"].ToString().Trim() != string.Empty ? Convert.ToBoolean(dt.Rows[0]["personal_email_id1_status"]) : false;
                    appRecordVM.personal_email_id2_status = dt.Rows[0]["personal_email_id2_status"].ToString().Trim() != string.Empty ? Convert.ToBoolean(dt.Rows[0]["personal_email_id2_status"]) : false;
                    appRecordVM.personal_email_pi_status = dt.Rows[0]["personal_email_pi_status"].ToString().Trim() != string.Empty ? Convert.ToBoolean(dt.Rows[0]["personal_email_pi_status"]) : false;
                    appRecordVM.personal_email_pb_status = dt.Rows[0]["personal_email_pb_status"].ToString().Trim() != string.Empty ? Convert.ToBoolean(dt.Rows[0]["personal_email_pb_status"]) : false;
                    appRecordVM.personal_contact_no_id1_status = dt.Rows[0]["personal_contact_no_id1_status"].ToString().Trim() != string.Empty ? Convert.ToBoolean(dt.Rows[0]["personal_contact_no_id1_status"]) : false;
                    appRecordVM.personal_contact_no_id2_status = dt.Rows[0]["personal_contact_no_id2_status"].ToString().Trim() != string.Empty ? Convert.ToBoolean(dt.Rows[0]["personal_contact_no_id2_status"]) : false;
                    appRecordVM.personal_contact_no_pi_status = dt.Rows[0]["personal_contact_no_pi_status"].ToString().Trim() != string.Empty ? Convert.ToBoolean(dt.Rows[0]["personal_contact_no_pi_status"]) : false;
                    appRecordVM.personal_contact_no_pb_status = dt.Rows[0]["personal_contact_no_pb_status"].ToString().Trim() != string.Empty ? Convert.ToBoolean(dt.Rows[0]["personal_contact_no_pb_status"]) : false;
                    appRecordVM.personal_perma_address_id1_status = dt.Rows[0]["personal_perma_address_id1_status"].ToString().Trim() != string.Empty ? Convert.ToBoolean(dt.Rows[0]["personal_perma_address_id1_status"]) : false;
                    appRecordVM.personal_perma_address_id2_status = dt.Rows[0]["personal_perma_address_id2_status"].ToString().Trim() != string.Empty ? Convert.ToBoolean(dt.Rows[0]["personal_perma_address_id2_status"]) : false;
                    appRecordVM.personal_perma_address_pi_status = dt.Rows[0]["personal_perma_address_pi_status"].ToString().Trim() != string.Empty ? Convert.ToBoolean(dt.Rows[0]["personal_perma_address_pi_status"]) : false;
                    appRecordVM.personal_perma_address_pb_status = dt.Rows[0]["personal_perma_address_pb_status"].ToString().Trim() != string.Empty ? Convert.ToBoolean(dt.Rows[0]["personal_perma_address_pb_status"]) : false;
                    appRecordVM.personal_house_ph_id1_status = dt.Rows[0]["personal_house_ph_id1_status"].ToString().Trim() != string.Empty ? Convert.ToBoolean(dt.Rows[0]["personal_house_ph_id1_status"]) : false;
                    appRecordVM.personal_house_ph_id2_status = dt.Rows[0]["personal_house_ph_id2_status"].ToString().Trim() != string.Empty ? Convert.ToBoolean(dt.Rows[0]["personal_house_ph_id2_status"]) : false;
                    appRecordVM.personal_house_ph_pi_status = dt.Rows[0]["personal_house_ph_pi_status"].ToString().Trim() != string.Empty ? Convert.ToBoolean(dt.Rows[0]["personal_house_ph_pi_status"]) : false;
                    appRecordVM.personal_house_ph_pb_status = dt.Rows[0]["personal_house_ph_pb_status"].ToString().Trim() != string.Empty ? Convert.ToBoolean(dt.Rows[0]["personal_house_ph_pb_status"]) : false;
                    appRecordVM.personal_birth_place_id1_status = dt.Rows[0]["personal_name_id1_status"].ToString().Trim() != string.Empty ? Convert.ToBoolean(dt.Rows[0]["personal_birth_place_id1_status"]) : false;
                    appRecordVM.personal_birth_place_id2_status = dt.Rows[0]["personal_birth_place_id2_status"].ToString().Trim() != string.Empty ? Convert.ToBoolean(dt.Rows[0]["personal_birth_place_id2_status"]) : false;
                    appRecordVM.personal_birth_place_pi_status = dt.Rows[0]["personal_birth_place_pi_status"].ToString().Trim() != string.Empty ? Convert.ToBoolean(dt.Rows[0]["personal_birth_place_pi_status"]) : false;
                    appRecordVM.personal_birth_place_pb_status = dt.Rows[0]["personal_birth_place_pb_status"].ToString().Trim() != string.Empty ? Convert.ToBoolean(dt.Rows[0]["personal_birth_place_pb_status"]) : false;
                    appRecordVM.personal_birth_date_id1_status = dt.Rows[0]["personal_birth_date_id1_status"].ToString().Trim() != string.Empty ? Convert.ToBoolean(dt.Rows[0]["personal_birth_date_id1_status"]) : false;
                    appRecordVM.personal_birth_date_id2_status = dt.Rows[0]["personal_birth_date_id2_status"].ToString().Trim() != string.Empty ? Convert.ToBoolean(dt.Rows[0]["personal_birth_date_id2_status"]) : false;
                    appRecordVM.personal_birth_date_pi_status = dt.Rows[0]["personal_birth_date_pi_status"].ToString().Trim() != string.Empty ? Convert.ToBoolean(dt.Rows[0]["personal_birth_date_pi_status"]) : false;
                    appRecordVM.personal_birth_date_pb_status = dt.Rows[0]["personal_birth_date_pb_status"].ToString().Trim() != string.Empty ? Convert.ToBoolean(dt.Rows[0]["personal_birth_date_pb_status"]) : false;
                    appRecordVM.personal_civil_id1_status = dt.Rows[0]["personal_civil_id1_status"].ToString().Trim() != string.Empty ? Convert.ToBoolean(dt.Rows[0]["personal_civil_id1_status"]) : false;
                    appRecordVM.personal_civil_id2_status = dt.Rows[0]["personal_civil_id2_status"].ToString().Trim() != string.Empty ? Convert.ToBoolean(dt.Rows[0]["personal_civil_id2_status"]) : false;
                    appRecordVM.personal_civil_pi_status = dt.Rows[0]["personal_civil_pi_status"].ToString().Trim() != string.Empty ? Convert.ToBoolean(dt.Rows[0]["personal_civil_pi_status"]) : false;
                    appRecordVM.personal_civil_pb_status = dt.Rows[0]["personal_civil_pb_status"].ToString().Trim() != string.Empty ? Convert.ToBoolean(dt.Rows[0]["personal_civil_pb_status"]) : false;
                    appRecordVM.personal_mother_maiden_name_id1_status = dt.Rows[0]["personal_mother_maiden_name_id1_status"].ToString().Trim() != string.Empty ? Convert.ToBoolean(dt.Rows[0]["personal_mother_maiden_name_id1_status"]) : false;
                    appRecordVM.personal_mother_maiden_name_id2_status = dt.Rows[0]["personal_mother_maiden_name_id2_status"].ToString().Trim() != string.Empty ? Convert.ToBoolean(dt.Rows[0]["personal_mother_maiden_name_id2_status"]) : false;
                    appRecordVM.personal_mother_maiden_name_pi_status = dt.Rows[0]["personal_mother_maiden_name_pi_status"].ToString().Trim() != string.Empty ? Convert.ToBoolean(dt.Rows[0]["personal_mother_maiden_name_pi_status"]) : false;
                    appRecordVM.personal_mother_maiden_name_pb_status = dt.Rows[0]["personal_mother_maiden_name_pb_status"].ToString().Trim() != string.Empty ? Convert.ToBoolean(dt.Rows[0]["personal_mother_maiden_name_pb_status"]) : false;
                    appRecordVM.personal_mother_perma_add_id1_status = dt.Rows[0]["personal_mother_perma_add_id1_status"].ToString().Trim() != string.Empty ? Convert.ToBoolean(dt.Rows[0]["personal_mother_perma_add_id1_status"]) : false;
                    appRecordVM.personal_mother_perma_add_id2_status = dt.Rows[0]["personal_mother_perma_add_id2_status"].ToString().Trim() != string.Empty ? Convert.ToBoolean(dt.Rows[0]["personal_mother_perma_add_id2_status"]) : false;
                    appRecordVM.personal_mother_perma_add_pi_status = dt.Rows[0]["personal_mother_perma_add_pi_status"].ToString().Trim() != string.Empty ? Convert.ToBoolean(dt.Rows[0]["personal_mother_perma_add_pi_status"]) : false;
                    appRecordVM.personal_mother_perma_add_pb_status = dt.Rows[0]["personal_mother_perma_add_pb_status"].ToString().Trim() != string.Empty ? Convert.ToBoolean(dt.Rows[0]["personal_mother_perma_add_pb_status"]) : false;
                    appRecordVM.personal_company_name_id1_status = dt.Rows[0]["personal_company_name_id1_status"].ToString().Trim() != string.Empty ? Convert.ToBoolean(dt.Rows[0]["personal_company_name_id1_status"]) : false;
                    appRecordVM.personal_company_name_id2_status = dt.Rows[0]["personal_company_name_id2_status"].ToString().Trim() != string.Empty ? Convert.ToBoolean(dt.Rows[0]["personal_company_name_id2_status"]) : false;
                    appRecordVM.personal_company_name_pi_status = dt.Rows[0]["personal_company_name_pi_status"].ToString().Trim() != string.Empty ? Convert.ToBoolean(dt.Rows[0]["personal_company_name_pi_status"]) : false;
                    appRecordVM.personal_company_name_pb_status = dt.Rows[0]["personal_company_name_pb_status"].ToString().Trim() != string.Empty ? Convert.ToBoolean(dt.Rows[0]["personal_company_name_pb_status"]) : false;
                    appRecordVM.personal_company_address_id1_status = dt.Rows[0]["personal_company_address_id1_status"].ToString().Trim() != string.Empty ? Convert.ToBoolean(dt.Rows[0]["personal_company_address_id1_status"]) : false;
                    appRecordVM.personal_company_address_id2_status = dt.Rows[0]["personal_company_address_id2_status"].ToString().Trim() != string.Empty ? Convert.ToBoolean(dt.Rows[0]["personal_company_address_id2_status"]) : false;
                    appRecordVM.personal_company_address_pi_status = dt.Rows[0]["personal_company_address_pi_status"].ToString().Trim() != string.Empty ? Convert.ToBoolean(dt.Rows[0]["personal_company_address_pi_status"]) : false;
                    appRecordVM.personal_company_address_pb_status = dt.Rows[0]["personal_company_address_pb_status"].ToString().Trim() != string.Empty ? Convert.ToBoolean(dt.Rows[0]["personal_company_address_pb_status"]) : false;
                    appRecordVM.personal_job_title_id1_status = dt.Rows[0]["personal_job_title_id1_status"].ToString().Trim() != string.Empty ? Convert.ToBoolean(dt.Rows[0]["personal_job_title_id1_status"]) : false;
                    appRecordVM.personal_job_title_id2_status = dt.Rows[0]["personal_job_title_id2_status"].ToString().Trim() != string.Empty ? Convert.ToBoolean(dt.Rows[0]["personal_job_title_id2_status"]) : false;
                    appRecordVM.personal_job_title_pi_status = dt.Rows[0]["personal_job_title_pi_status"].ToString().Trim() != string.Empty ? Convert.ToBoolean(dt.Rows[0]["personal_job_title_pi_status"]) : false;
                    appRecordVM.personal_job_title_pb_status = dt.Rows[0]["personal_job_title_pb_status"].ToString().Trim() != string.Empty ? Convert.ToBoolean(dt.Rows[0]["personal_job_title_pb_status"]) : false;
                    appRecordVM.personal_monthly_income_id1_status = dt.Rows[0]["personal_monthly_income_id1_status"].ToString().Trim() != string.Empty ? Convert.ToBoolean(dt.Rows[0]["personal_monthly_income_id1_status"]) : false;
                    appRecordVM.personal_monthly_income_id2_status = dt.Rows[0]["personal_monthly_income_id2_status"].ToString().Trim() != string.Empty ? Convert.ToBoolean(dt.Rows[0]["personal_monthly_income_id2_status"]) : false;
                    appRecordVM.personal_monthly_income_pi_status = dt.Rows[0]["personal_monthly_income_pi_status"].ToString().Trim() != string.Empty ? Convert.ToBoolean(dt.Rows[0]["personal_monthly_income_pi_status"]) : false;
                    appRecordVM.personal_monthly_income_pb_status = dt.Rows[0]["personal_monthly_income_pb_status"].ToString().Trim() != string.Empty ? Convert.ToBoolean(dt.Rows[0]["personal_monthly_income_pb_status"]) : false;
                    appRecordVM.personal_reference_name_id1_status = dt.Rows[0]["personal_reference_name_id1_status"].ToString().Trim() != string.Empty ? Convert.ToBoolean(dt.Rows[0]["personal_reference_name_id1_status"]) : false;
                    appRecordVM.personal_reference_name_id2_status = dt.Rows[0]["personal_reference_name_id2_status"].ToString().Trim() != string.Empty ? Convert.ToBoolean(dt.Rows[0]["personal_reference_name_id2_status"]) : false;
                    appRecordVM.personal_reference_name_pi_status = dt.Rows[0]["personal_reference_name_pi_status"].ToString().Trim() != string.Empty ? Convert.ToBoolean(dt.Rows[0]["personal_reference_name_pi_status"]) : false;
                    appRecordVM.personal_reference_name_pb_status = dt.Rows[0]["personal_reference_name_pb_status"].ToString().Trim() != string.Empty ? Convert.ToBoolean(dt.Rows[0]["personal_reference_name_pb_status"]) : false;
                    appRecordVM.personal_reference_contact_id1_status = dt.Rows[0]["personal_reference_contact_id1_status"].ToString().Trim() != string.Empty ? Convert.ToBoolean(dt.Rows[0]["personal_reference_contact_id1_status"]) : false;
                    appRecordVM.personal_reference_contact_id2_status = dt.Rows[0]["personal_reference_contact_id2_status"].ToString().Trim() != string.Empty ? Convert.ToBoolean(dt.Rows[0]["personal_reference_contact_id2_status"]) : false;
                    appRecordVM.personal_reference_contact_pi_status = dt.Rows[0]["personal_reference_contact_pi_status"].ToString().Trim() != string.Empty ? Convert.ToBoolean(dt.Rows[0]["personal_reference_contact_pi_status"]) : false;
                    appRecordVM.personal_reference_contact_pb_status = dt.Rows[0]["personal_reference_contact_pb_status"].ToString().Trim() != string.Empty ? Convert.ToBoolean(dt.Rows[0]["personal_reference_contact_pb_status"]) : false;
                    appRecordVM.personal_bank_name_id1_status = dt.Rows[0]["personal_bank_name_id1_status"].ToString().Trim() != string.Empty ? Convert.ToBoolean(dt.Rows[0]["personal_bank_name_id1_status"]) : false;
                    appRecordVM.personal_bank_name_id2_status = dt.Rows[0]["personal_bank_name_id2_status"].ToString().Trim() != string.Empty ? Convert.ToBoolean(dt.Rows[0]["personal_bank_name_id2_status"]) : false;
                    appRecordVM.personal_bank_name_pi_status = dt.Rows[0]["personal_bank_name_pi_status"].ToString().Trim() != string.Empty ? Convert.ToBoolean(dt.Rows[0]["personal_bank_name_pi_status"]) : false;
                    appRecordVM.personal_bank_name_pb_status = dt.Rows[0]["personal_bank_name_pb_status"].ToString().Trim() != string.Empty ? Convert.ToBoolean(dt.Rows[0]["personal_bank_name_pb_status"]) : false;
                    appRecordVM.personal_bank_ac_no_id1_status = dt.Rows[0]["personal_bank_ac_no_id1_status"].ToString().Trim() != string.Empty ? Convert.ToBoolean(dt.Rows[0]["personal_bank_ac_no_id1_status"]) : false;
                    appRecordVM.personal_bank_ac_no_id2_status = dt.Rows[0]["personal_bank_ac_no_id2_status"].ToString().Trim() != string.Empty ? Convert.ToBoolean(dt.Rows[0]["personal_bank_ac_no_id2_status"]) : false;
                    appRecordVM.personal_bank_ac_no_pi_status = dt.Rows[0]["personal_bank_ac_no_pi_status"].ToString().Trim() != string.Empty ? Convert.ToBoolean(dt.Rows[0]["personal_bank_ac_no_pi_status"]) : false;
                    appRecordVM.personal_bank_ac_no_pb_status = dt.Rows[0]["personal_bank_ac_no_pb_status"].ToString().Trim() != string.Empty ? Convert.ToBoolean(dt.Rows[0]["personal_bank_ac_no_pb_status"]) : false;
                    appRecordVM.personal_req_loan_amt_id1_status = dt.Rows[0]["personal_req_loan_amt_id1_status"].ToString().Trim() != string.Empty ? Convert.ToBoolean(dt.Rows[0]["personal_req_loan_amt_id1_status"]) : false;
                    appRecordVM.personal_req_loan_amt_id2_status = dt.Rows[0]["personal_req_loan_amt_id2_status"].ToString().Trim() != string.Empty ? Convert.ToBoolean(dt.Rows[0]["personal_req_loan_amt_id2_status"]) : false;
                    appRecordVM.personal_req_loan_amt_pi_status = dt.Rows[0]["personal_req_loan_amt_pi_status"].ToString().Trim() != string.Empty ? Convert.ToBoolean(dt.Rows[0]["personal_req_loan_amt_pi_status"]) : false;
                    appRecordVM.personal_req_loan_amt_pb_status = dt.Rows[0]["personal_req_loan_amt_pb_status"].ToString().Trim() != string.Empty ? Convert.ToBoolean(dt.Rows[0]["personal_req_loan_amt_pb_status"]) : false;
                    appRecordVM.personal_req_loan_term_id1_status = dt.Rows[0]["personal_req_loan_term_id1_status"].ToString().Trim() != string.Empty ? Convert.ToBoolean(dt.Rows[0]["personal_req_loan_term_id1_status"]) : false;
                    appRecordVM.personal_req_loan_term_id2_status = dt.Rows[0]["personal_req_loan_term_id2_status"].ToString().Trim() != string.Empty ? Convert.ToBoolean(dt.Rows[0]["personal_req_loan_term_id2_status"]) : false;
                    appRecordVM.personal_req_loan_term_pi_status = dt.Rows[0]["personal_req_loan_term_pi_status"].ToString().Trim() != string.Empty ? Convert.ToBoolean(dt.Rows[0]["personal_req_loan_term_pi_status"]) : false;
                    appRecordVM.personal_req_loan_term_pb_status = dt.Rows[0]["personal_req_loan_term_pb_status"].ToString().Trim() != string.Empty ? Convert.ToBoolean(dt.Rows[0]["personal_req_loan_term_pb_status"]) : false;
                    #region Additiona Field Changes

                    #region New Fields
                    appRecordVM.nearest_landmark = dt.Rows[0]["nearest_landmark"].ToString().Trim() != string.Empty ? dt.Rows[0]["nearest_landmark"].ToString() : string.Empty;
                    appRecordVM.SSS_No = dt.Rows[0]["sss_no"].ToString().Trim() != string.Empty ? dt.Rows[0]["sss_no"].ToString() : string.Empty;
                    appRecordVM.home_status = dt.Rows[0]["home_status"].ToString().Trim() != string.Empty ? Convert.ToInt32(dt.Rows[0]["home_status"].ToString()) : 0;
                    appRecordVM.Permanent_address = dt.Rows[0]["permanent_address"].ToString().Trim() != string.Empty ? dt.Rows[0]["permanent_address"].ToString() : string.Empty;
                    appRecordVM.tansfer_residence = dt.Rows[0]["tansfer_residence"].ToString().Trim() != string.Empty ? dt.Rows[0]["tansfer_residence"].ToString() : string.Empty;
                    appRecordVM.spouse_name = dt.Rows[0]["spouse_name"].ToString().Trim() != string.Empty ? dt.Rows[0]["spouse_name"].ToString() : string.Empty;
                    appRecordVM.spouse_occupation = dt.Rows[0]["spouse_occupation"].ToString().Trim() != string.Empty ? dt.Rows[0]["spouse_occupation"].ToString() : string.Empty;
                    appRecordVM.number_of_dependent = Convert.ToString(dt.Rows[0]["number_of_dependent"]) == string.Empty ? 0 : Convert.ToInt32(dt.Rows[0]["number_of_dependent"]);
                    appRecordVM.mother_work = dt.Rows[0]["mother_work"].ToString().Trim() != string.Empty ? dt.Rows[0]["mother_work"].ToString() : string.Empty;
                    appRecordVM.father_name = dt.Rows[0]["father_name"].ToString().Trim() != string.Empty ? dt.Rows[0]["father_name"].ToString() : string.Empty;
                    appRecordVM.father_work = dt.Rows[0]["father_work"].ToString().Trim() != string.Empty ? dt.Rows[0]["father_work"].ToString() : string.Empty;
                    appRecordVM.living_with_mother = dt.Rows[0]["living_with_mother"].ToString().Trim() != string.Empty ? dt.Rows[0]["living_with_mother"].ToString() : string.Empty;
                    appRecordVM.sibling_count = Convert.ToString(dt.Rows[0]["sibling_count"]) == string.Empty ? 0 : Convert.ToInt32(dt.Rows[0]["sibling_count"]);
                    appRecordVM.sibling_works = dt.Rows[0]["sibling_works"].ToString().Trim() != string.Empty ? dt.Rows[0]["sibling_works"].ToString() : string.Empty;
                    appRecordVM.occupation = dt.Rows[0]["occupation"].ToString().Trim() != string.Empty ? dt.Rows[0]["occupation"].ToString() : string.Empty;

                    //Best Time to Morning Call
                    int BestTimeToCallMorning = Convert.ToString(dt.Rows[0]["morning_time"]).Trim() == string.Empty ? -1 : Convert.ToInt32(dt.Rows[0]["morning_time"]);
                    if (BestTimeToCallMorning != -1)
                        appRecordVM.MorningTime = CommonMethods.GetBestTimeToCallMorning(BestTimeToCallMorning);
                    else
                        appRecordVM.MorningTime = string.Empty;

                    //Best Time to Noon Call
                    int BestTimeToCallNoon = Convert.ToString(dt.Rows[0]["noon_time"]).Trim() == string.Empty ? -1 : Convert.ToInt32(dt.Rows[0]["noon_time"]);
                    if (BestTimeToCallNoon != -1)
                        appRecordVM.NoonTime = CommonMethods.GetBestTimeToCallNoon(BestTimeToCallNoon);
                    else
                        appRecordVM.NoonTime = string.Empty;

                    appRecordVM.net_income = dt.Rows[0]["net_income"].ToString() != string.Empty ? Convert.ToDouble(dt.Rows[0]["net_income"]) : 0.00;
                    appRecordVM.pending_resignation = dt.Rows[0]["pending_resignation"].ToString().Trim() != string.Empty ? dt.Rows[0]["pending_resignation"].ToString() : string.Empty;
                    appRecordVM.other_source_of_income = dt.Rows[0]["other_source_of_income"].ToString().Trim() != string.Empty ? dt.Rows[0]["other_source_of_income"].ToString() : string.Empty;
                    appRecordVM.know_about_cashmart = dt.Rows[0]["know_about_cashmart"].ToString().Trim() != string.Empty ? dt.Rows[0]["know_about_cashmart"].ToString() : string.Empty;
                    appRecordVM.pending_loan_fron_otherland = dt.Rows[0]["pending_loan_fron_otherland"].ToString().Trim() != string.Empty ? dt.Rows[0]["pending_loan_fron_otherland"].ToString() : string.Empty;
                    appRecordVM.bank_loan_or_credit_card = dt.Rows[0]["bank_loan_or_credit_card"].ToString().Trim() != string.Empty ? dt.Rows[0]["bank_loan_or_credit_card"].ToString() : string.Empty;
                    appRecordVM.Permanent_address = dt.Rows[0]["permanent_address"].ToString().Trim() != string.Empty ? dt.Rows[0]["permanent_address"].ToString() : string.Empty;
                    appRecordVM.BankId = Convert.ToString(dt.Rows[0]["bankid"]) == "" ? 0 : Convert.ToInt32(dt.Rows[0]["bankid"]);
                    appRecordVM.family_name1 = dt.Rows[0]["family_name1"].ToString().Trim() != string.Empty ? dt.Rows[0]["family_name1"].ToString() : string.Empty;
                    appRecordVM.family_address1 = dt.Rows[0]["family_address1"].ToString().Trim() != string.Empty ? dt.Rows[0]["family_address1"].ToString() : string.Empty;
                    appRecordVM.family_contact1 = dt.Rows[0]["family_contact1"].ToString().Trim() != string.Empty ? dt.Rows[0]["family_contact1"].ToString() : string.Empty;
                    appRecordVM.family_relation1 = dt.Rows[0]["family_relation1"].ToString().Trim() != string.Empty ? dt.Rows[0]["family_relation1"].ToString() : string.Empty;
                    appRecordVM.family_name2 = dt.Rows[0]["family_name2"].ToString().Trim() != string.Empty ? dt.Rows[0]["family_name2"].ToString() : string.Empty;
                    appRecordVM.family_address2 = dt.Rows[0]["family_address2"].ToString().Trim() != string.Empty ? dt.Rows[0]["family_address2"].ToString() : string.Empty;
                    appRecordVM.family_contact2 = dt.Rows[0]["family_contact2"].ToString().Trim() != string.Empty ? dt.Rows[0]["family_contact2"].ToString() : string.Empty;
                    appRecordVM.family_relation2 = dt.Rows[0]["family_relation2"].ToString().Trim() != string.Empty ? dt.Rows[0]["family_relation2"].ToString() : string.Empty;
                    appRecordVM.optional_name = dt.Rows[0]["optional_name"].ToString().Trim() != string.Empty ? dt.Rows[0]["optional_name"].ToString() : string.Empty;
                    appRecordVM.optional_address = dt.Rows[0]["optional_address"].ToString().Trim() != string.Empty ? dt.Rows[0]["optional_address"].ToString() : string.Empty;
                    appRecordVM.optional_contact = dt.Rows[0]["optional_contact"].ToString().Trim() != string.Empty ? dt.Rows[0]["optional_contact"].ToString() : string.Empty;
                    appRecordVM.optional_relation = dt.Rows[0]["optional_relation"].ToString().Trim() != string.Empty ? dt.Rows[0]["optional_relation"].ToString() : string.Empty;
                    appRecordVM.checker_oic_on = dt.Rows[0]["checker_oic_on"].ToString().Trim() != string.Empty ? dt.Rows[0]["checker_oic_on"].ToString() : string.Empty;
                    appRecordVM.disbursement_date = dt.Rows[0]["disbursement_date"].ToString().Trim() != string.Empty ? dt.Rows[0]["disbursement_date"].ToString() : string.Empty;

                    #endregion

                    #region New Fields Remarks
                    appRecordVM.nearest_landmark_remark = dt.Rows[0]["nearest_landmark_remark"].ToString().Trim() != string.Empty ? dt.Rows[0]["nearest_landmark_remark"].ToString() : string.Empty;
                    appRecordVM.SSS_No_remark = dt.Rows[0]["sss_no_remark"].ToString().Trim() != string.Empty ? dt.Rows[0]["sss_no_remark"].ToString() : string.Empty;
                    appRecordVM.rented_mortgage_owned_remark = dt.Rows[0]["rented_mortgage_owned_remark"].ToString().Trim() != string.Empty ? dt.Rows[0]["rented_mortgage_owned_remark"].ToString() : string.Empty;
                    appRecordVM.Permanent_address_remark = dt.Rows[0]["permanent_address_remark"].ToString().Trim() != string.Empty ? dt.Rows[0]["permanent_address_remark"].ToString() : string.Empty;
                    appRecordVM.tansfer_residence_remark = dt.Rows[0]["tansfer_residence_remark"].ToString().Trim() != string.Empty ? dt.Rows[0]["tansfer_residence_remark"].ToString() : string.Empty;
                    appRecordVM.spouse_name_remark = dt.Rows[0]["spouse_name_remark"].ToString().Trim() != string.Empty ? dt.Rows[0]["spouse_name_remark"].ToString() : string.Empty;
                    appRecordVM.spouse_occupation_remark = dt.Rows[0]["spouse_occupation_remark"].ToString().Trim() != string.Empty ? dt.Rows[0]["spouse_occupation_remark"].ToString() : string.Empty;
                    appRecordVM.number_of_dependent_remark = dt.Rows[0]["number_of_dependent_remark"].ToString().Trim() != string.Empty ? dt.Rows[0]["number_of_dependent_remark"].ToString() : string.Empty;
                    appRecordVM.father_name_work_remark = dt.Rows[0]["father_name_work_remark"].ToString().Trim() != string.Empty ? dt.Rows[0]["father_name_work_remark"].ToString() : string.Empty;
                    appRecordVM.sibling_works_remark = dt.Rows[0]["sibling_works_remark"].ToString().Trim() != string.Empty ? dt.Rows[0]["sibling_works_remark"].ToString() : string.Empty;
                    appRecordVM.occupation_remark = dt.Rows[0]["occupation_remark"].ToString().Trim() != string.Empty ? dt.Rows[0]["occupation_remark"].ToString() : string.Empty;
                    appRecordVM.scheduled_and_btc_remark = dt.Rows[0]["scheduled_and_btc_remark"].ToString().Trim() != string.Empty ? dt.Rows[0]["scheduled_and_btc_remark"].ToString() : string.Empty;
                    appRecordVM.net_income_remark = dt.Rows[0]["net_income_remark"].ToString().Trim() != string.Empty ? dt.Rows[0]["net_income_remark"].ToString() : string.Empty;
                    appRecordVM.pending_resignation_remark = dt.Rows[0]["pending_resignation_remark"].ToString().Trim() != string.Empty ? dt.Rows[0]["pending_resignation_remark"].ToString() : string.Empty;
                    appRecordVM.other_source_of_income_remark = dt.Rows[0]["other_source_of_income_remark"].ToString().Trim() != string.Empty ? dt.Rows[0]["other_source_of_income_remark"].ToString() : string.Empty;
                    appRecordVM.know_about_cashmart_remark = dt.Rows[0]["know_about_cashmart_remark"].ToString().Trim() != string.Empty ? dt.Rows[0]["know_about_cashmart_remark"].ToString() : string.Empty;
                    appRecordVM.pending_loan_fron_otherland_remark = dt.Rows[0]["pending_loan_fron_otherland_remark"].ToString().Trim() != string.Empty ? dt.Rows[0]["pending_loan_fron_otherland_remark"].ToString() : string.Empty;
                    appRecordVM.bank_loan_or_credit_card_remark = dt.Rows[0]["bank_loan_or_credit_card_remark"].ToString().Trim() != string.Empty ? dt.Rows[0]["bank_loan_or_credit_card_remark"].ToString() : string.Empty;
                    appRecordVM.personal_bank_name_remark = dt.Rows[0]["personal_bank_name_remark"].ToString().Trim() != string.Empty ? dt.Rows[0]["personal_bank_name_remark"].ToString() : string.Empty;
                    appRecordVM.family_name1_remark = dt.Rows[0]["family_name1_remark"].ToString().Trim() != string.Empty ? dt.Rows[0]["family_name1_remark"].ToString() : string.Empty;
                    appRecordVM.family_address1_remark = dt.Rows[0]["family_address1_remark"].ToString().Trim() != string.Empty ? dt.Rows[0]["family_address1_remark"].ToString() : string.Empty;
                    appRecordVM.family_contact1_remark = dt.Rows[0]["family_contact1_remark"].ToString().Trim() != string.Empty ? dt.Rows[0]["family_contact1_remark"].ToString() : string.Empty;
                    appRecordVM.family_relation1_remark = dt.Rows[0]["family_relation1_remark"].ToString().Trim() != string.Empty ? dt.Rows[0]["family_relation1_remark"].ToString() : string.Empty;
                    appRecordVM.family_name2_remark = dt.Rows[0]["family_name2_remark"].ToString().Trim() != string.Empty ? dt.Rows[0]["family_name2_remark"].ToString() : string.Empty;
                    appRecordVM.family_address2_remark = dt.Rows[0]["family_address2_remark"].ToString().Trim() != string.Empty ? dt.Rows[0]["family_address2_remark"].ToString() : string.Empty;
                    appRecordVM.family_contact2_remark = dt.Rows[0]["family_contact2_remark"].ToString().Trim() != string.Empty ? dt.Rows[0]["family_contact2_remark"].ToString() : string.Empty;
                    appRecordVM.family_relation2_remark = dt.Rows[0]["family_relation2_remark"].ToString().Trim() != string.Empty ? dt.Rows[0]["family_relation2_remark"].ToString() : string.Empty;
                    appRecordVM.optional_name_remark = dt.Rows[0]["optional_name_remark"].ToString().Trim() != string.Empty ? dt.Rows[0]["optional_name_remark"].ToString() : string.Empty;
                    appRecordVM.optional_address_remark = dt.Rows[0]["optional_address_remark"].ToString().Trim() != string.Empty ? dt.Rows[0]["optional_address_remark"].ToString() : string.Empty;
                    appRecordVM.optional_contact_remark = dt.Rows[0]["optional_contact_remark"].ToString().Trim() != string.Empty ? dt.Rows[0]["optional_contact_remark"].ToString() : string.Empty;
                    appRecordVM.optional_relation_remark = dt.Rows[0]["optional_relation_remark"].ToString().Trim() != string.Empty ? dt.Rows[0]["optional_relation_remark"].ToString() : string.Empty;
                    #endregion

                    #endregion

                    appRecordVM.personal_name_remark = dt.Rows[0]["personal_name_remark"].ToString().Trim() != string.Empty ? dt.Rows[0]["personal_name_remark"].ToString() : string.Empty;
                    appRecordVM.personal_email_remark = dt.Rows[0]["personal_email_remark"].ToString().Trim() != string.Empty ? dt.Rows[0]["personal_email_remark"].ToString() : string.Empty;
                    appRecordVM.personal_contact_no_remark = dt.Rows[0]["personal_contact_no_remark"].ToString().Trim() != string.Empty ? dt.Rows[0]["personal_contact_no_remark"].ToString() : string.Empty;
                    appRecordVM.personal_perma_address_remark = dt.Rows[0]["personal_perma_address_remark"].ToString().Trim() != string.Empty ? dt.Rows[0]["personal_perma_address_remark"].ToString() : string.Empty;
                    appRecordVM.personal_house_ph_remark = dt.Rows[0]["personal_house_ph_remark"].ToString().Trim() != string.Empty ? dt.Rows[0]["personal_house_ph_remark"].ToString() : string.Empty;
                    appRecordVM.personal_birth_place_remark = dt.Rows[0]["personal_birth_place_remark"].ToString().Trim() != string.Empty ? dt.Rows[0]["personal_birth_place_remark"].ToString() : string.Empty;
                    appRecordVM.personal_birth_date_remark = dt.Rows[0]["personal_birth_date_remark"].ToString().Trim() != string.Empty ? dt.Rows[0]["personal_birth_date_remark"].ToString() : string.Empty;
                    appRecordVM.personal_civil_remark = dt.Rows[0]["personal_civil_remark"].ToString().Trim() != string.Empty ? dt.Rows[0]["personal_civil_remark"].ToString() : string.Empty;
                    appRecordVM.personal_mother_maiden_name_remark = dt.Rows[0]["personal_mother_maiden_name_remark"].ToString().Trim() != string.Empty ? dt.Rows[0]["personal_mother_maiden_name_remark"].ToString() : string.Empty;
                    appRecordVM.personal_mother_perma_add_remark = dt.Rows[0]["personal_mother_perma_add_remark"].ToString().Trim() != string.Empty ? dt.Rows[0]["personal_mother_perma_add_remark"].ToString() : string.Empty;
                    appRecordVM.personal_company_name_remark = dt.Rows[0]["personal_company_name_remark"].ToString().Trim() != string.Empty ? dt.Rows[0]["personal_company_name_remark"].ToString() : string.Empty;
                    appRecordVM.personal_company_address_remark = dt.Rows[0]["personal_company_address_remark"].ToString().Trim() != string.Empty ? dt.Rows[0]["personal_company_address_remark"].ToString() : string.Empty;
                    appRecordVM.personal_job_title_remark = dt.Rows[0]["personal_job_title_remark"].ToString().Trim() != string.Empty ? dt.Rows[0]["personal_job_title_remark"].ToString() : string.Empty;
                    appRecordVM.personal_monthly_income_remark = dt.Rows[0]["personal_monthly_income_remark"].ToString().Trim() != string.Empty ? dt.Rows[0]["personal_monthly_income_remark"].ToString() : string.Empty;
                    appRecordVM.personal_reference_name_remark = dt.Rows[0]["personal_reference_name_remark"].ToString().Trim() != string.Empty ? dt.Rows[0]["personal_reference_name_remark"].ToString() : string.Empty;
                    appRecordVM.personal_reference_contact_remark = dt.Rows[0]["personal_reference_contact_remark"].ToString().Trim() != string.Empty ? dt.Rows[0]["personal_reference_contact_remark"].ToString() : string.Empty;
                    appRecordVM.personal_bank_name_remark = dt.Rows[0]["personal_bank_name_remark"].ToString().Trim() != string.Empty ? dt.Rows[0]["personal_bank_name_remark"].ToString() : string.Empty;
                    appRecordVM.personal_bank_ac_no_remark = dt.Rows[0]["personal_bank_ac_no_remark"].ToString().Trim() != string.Empty ? dt.Rows[0]["personal_bank_ac_no_remark"].ToString() : string.Empty;
                    appRecordVM.personal_req_loan_amt_remark = dt.Rows[0]["personal_req_loan_amt_remark"].ToString().Trim() != string.Empty ? dt.Rows[0]["personal_req_loan_amt_remark"].ToString() : string.Empty;
                    appRecordVM.personal_req_loan_term_remark = dt.Rows[0]["personal_req_loan_term_remark"].ToString().Trim() != string.Empty ? dt.Rows[0]["personal_req_loan_term_remark"].ToString() : string.Empty;

                    appRecordVM.AdditionalBirthPlace = dt.Rows[0]["addtional_birth_place"].ToString().Trim() != string.Empty ? dt.Rows[0]["addtional_birth_place"].ToString() : string.Empty;
                    appRecordVM.AdditionalProvincialAddress = dt.Rows[0]["additonal_provincial_address"].ToString().Trim() != string.Empty ? dt.Rows[0]["additonal_provincial_address"].ToString() : string.Empty;
                    appRecordVM.AdditionalLoanPurpose = Convert.ToString(dt.Rows[0]["purposeofloan"]) != string.Empty ? Convert.ToString(dt.Rows[0]["purposeofloan"]) : string.Empty;
                    appRecordVM.AdditionalLoanPurpose_Remark = Convert.ToString(dt.Rows[0]["additional_loan_purpose"]) != string.Empty ? Convert.ToString(dt.Rows[0]["additional_loan_purpose"]) : string.Empty;
                    if (appRecordVM.AdditionalLoanPurpose_Remark == string.Empty)
                        appRecordVM.AdditionalLoanPurpose_Remark = Convert.ToString(dt.Rows[0]["purposeofloan"]) != string.Empty ? Convert.ToString(dt.Rows[0]["purposeofloan"]) : string.Empty;

                    appRecordVM.AdditionalRequestedLoanAmount = dt.Rows[0]["additional_requested_loan_amount"].ToString().Trim() != string.Empty ? Convert.ToDouble(dt.Rows[0]["additional_requested_loan_amount"]) : 0.00;
                    appRecordVM.AdditionalEmployedDuration = dt.Rows[0]["additional_employed_duration"].ToString().Trim() != string.Empty ? Convert.ToString(dt.Rows[0]["additional_employed_duration"]) : String.Empty;
                    appRecordVM.AdditionalRequested_Term = AdditionalRequestTermStr;
                    appRecordVM.AdditionalJobPosition = dt.Rows[0]["additional_job_position"].ToString().Trim() != string.Empty ? dt.Rows[0]["additional_job_position"].ToString() : string.Empty;
                    appRecordVM.AdditionalJobLevel = dt.Rows[0]["additional_job_level"].ToString().Trim() != string.Empty ? dt.Rows[0]["additional_job_level"].ToString() : string.Empty;
                    appRecordVM.AdditionalPayDate = dt.Rows[0]["additional_paydate"].ToString().Trim() != string.Empty ? Convert.ToString(dt.Rows[0]["additional_paydate"]) : string.Empty;

                    #region PayDate
                    appRecordVM.payDate1 = Convert.ToString(dt.Rows[0]["paydate1"]) != string.Empty ? Convert.ToString(dt.Rows[0]["paydate1"]) : string.Empty;
                    appRecordVM.payDate2 = Convert.ToString(dt.Rows[0]["paydate2"]) != string.Empty ? Convert.ToString(dt.Rows[0]["paydate2"]) : string.Empty;
                    #endregion

                    appRecordVM.FamilyPersonalName = dt.Rows[0]["family_personal_name"].ToString().Trim() != string.Empty ? dt.Rows[0]["family_personal_name"].ToString() : string.Empty;
                    appRecordVM.FamilyPersonalContact = dt.Rows[0]["family_personal_contact"].ToString().Trim() != string.Empty ? dt.Rows[0]["family_personal_contact"].ToString() : string.Empty;
                    appRecordVM.FamilyRelationWithBorrower = dt.Rows[0]["family_relation_with_borrower"].ToString().Trim() != string.Empty ? dt.Rows[0]["family_relation_with_borrower"].ToString() : string.Empty;
                    appRecordVM.FamilyBorrowerKnownDuration = dt.Rows[0]["family_borrower_known_duration"].ToString().Trim() != string.Empty ? dt.Rows[0]["family_borrower_known_duration"].ToString() : string.Empty;
                    appRecordVM.FamilyAddressVerification = dt.Rows[0]["family_address_verification"].ToString().Trim() != string.Empty ? dt.Rows[0]["family_address_verification"].ToString() : string.Empty;
                    appRecordVM.FamilyBorrowerWorkingPlace = dt.Rows[0]["family_borrower_working_place"].ToString().Trim() != string.Empty ? dt.Rows[0]["family_borrower_working_place"].ToString() : string.Empty;
                    appRecordVM.FamilyPersonalNameStatus = dt.Rows[0]["family_personal_name_status"].ToString().Trim() != string.Empty ? Convert.ToBoolean(dt.Rows[0]["family_personal_name_status"]) : false;
                    appRecordVM.FamilyPersonalContactStatus = dt.Rows[0]["family_personal_contact_status"].ToString().Trim() != string.Empty ? Convert.ToBoolean(dt.Rows[0]["family_personal_contact_status"]) : false;
                    appRecordVM.FamilyRelationWithBorrowerStatus = dt.Rows[0]["family_relation_with_borrower_status"].ToString().Trim() != string.Empty ? Convert.ToBoolean(dt.Rows[0]["family_relation_with_borrower_status"]) : false;
                    appRecordVM.FamilyBorrowerKnownDurationStatus = dt.Rows[0]["family_borrower_known_duration_status"].ToString().Trim() != string.Empty ? Convert.ToBoolean(dt.Rows[0]["family_borrower_known_duration_status"]) : false;
                    appRecordVM.FamilyAddressVerificationStatus = dt.Rows[0]["family_address_verification_status"].ToString().Trim() != string.Empty ? Convert.ToBoolean(dt.Rows[0]["family_address_verification_status"]) : false;
                    appRecordVM.FamilyBorrowerWorkingPlaceStatus = dt.Rows[0]["family_borrower_working_place_status"].ToString().Trim() != string.Empty ? Convert.ToBoolean(dt.Rows[0]["family_borrower_working_place_status"]) : false;


                    appRecordVM.FriendPersonalName = dt.Rows[0]["friend_personal_name"].ToString().Trim() != string.Empty ? dt.Rows[0]["friend_personal_name"].ToString() : string.Empty;
                    appRecordVM.FriendPersonalContact = dt.Rows[0]["friend_personal_contact"].ToString().Trim() != string.Empty ? dt.Rows[0]["friend_personal_contact"].ToString() : string.Empty;
                    appRecordVM.FriendRelationWithBorrower = dt.Rows[0]["friend_relation_with_borrower"].ToString().Trim() != string.Empty ? dt.Rows[0]["friend_relation_with_borrower"].ToString() : string.Empty;
                    appRecordVM.FriendBorrowerKnownDuration = dt.Rows[0]["friend_borrower_known_duration"].ToString().Trim() != string.Empty ? dt.Rows[0]["friend_borrower_known_duration"].ToString() : string.Empty;
                    appRecordVM.FriendAddressVerification = dt.Rows[0]["friend_address_verification"].ToString().Trim() != string.Empty ? dt.Rows[0]["friend_address_verification"].ToString() : string.Empty;
                    appRecordVM.FriendBorrowerWorkingPlace = dt.Rows[0]["friend_borrower_working"].ToString().Trim() != string.Empty ? dt.Rows[0]["friend_borrower_working"].ToString() : string.Empty;
                    appRecordVM.FriendPersonalNameStatus = dt.Rows[0]["friend_personal_name_status"].ToString().Trim() != string.Empty ? Convert.ToBoolean(dt.Rows[0]["friend_personal_name_status"]) : false;
                    appRecordVM.FriendPersonalContactStatus = dt.Rows[0]["friend_personal_contact_status"].ToString().Trim() != string.Empty ? Convert.ToBoolean(dt.Rows[0]["friend_personal_contact_status"]) : false;
                    appRecordVM.FriendRelationWithBorrowerStatus = dt.Rows[0]["friend_relation_with_borrower_status"].ToString().Trim() != string.Empty ? Convert.ToBoolean(dt.Rows[0]["friend_relation_with_borrower_status"]) : false;
                    appRecordVM.FriendBorrowerKnownDurationStatus = dt.Rows[0]["friend_borrower_known_duration_status"].ToString().Trim() != string.Empty ? Convert.ToBoolean(dt.Rows[0]["friend_borrower_known_duration_status"]) : false;
                    appRecordVM.FriendAddressVerificationStatus = dt.Rows[0]["friend_address_verification_status"].ToString().Trim() != string.Empty ? Convert.ToBoolean(dt.Rows[0]["friend_address_verification_status"]) : false;
                    appRecordVM.FriendBorrowerWorkingPlaceStatus = dt.Rows[0]["friend_borrower_working_place"].ToString().Trim() != string.Empty ? Convert.ToBoolean(dt.Rows[0]["friend_borrower_working_place"]) : false;

                    appRecordVM.CoWorkerPersonalName = dt.Rows[0]["co_worker_personal_name"].ToString().Trim() != string.Empty ? dt.Rows[0]["co_worker_personal_name"].ToString() : string.Empty;
                    appRecordVM.CoWorkerPersonalConatact = dt.Rows[0]["co_worker_personal_contact"].ToString().Trim() != string.Empty ? dt.Rows[0]["co_worker_personal_contact"].ToString() : string.Empty;
                    appRecordVM.CoWorkerRelationWithBorrower = dt.Rows[0]["co_worker_relation_with_borrower"].ToString().Trim() != string.Empty ? dt.Rows[0]["co_worker_relation_with_borrower"].ToString() : string.Empty;
                    appRecordVM.CoWorkerBorrowerKnownDuration = dt.Rows[0]["co_worker_borrower_known_duration"].ToString().Trim() != string.Empty ? dt.Rows[0]["co_worker_borrower_known_duration"].ToString() : string.Empty;
                    appRecordVM.CoWorkerAddressVerification = dt.Rows[0]["co_worker_address_verification"].ToString().Trim() != string.Empty ? dt.Rows[0]["co_worker_address_verification"].ToString() : string.Empty;
                    appRecordVM.CoWorkerBorrowerWorkingPlace = dt.Rows[0]["co_worker_borrower_working"].ToString().Trim() != string.Empty ? dt.Rows[0]["co_worker_borrower_working"].ToString() : string.Empty;
                    appRecordVM.CoWorkerPersonalNameStatus = dt.Rows[0]["co_worker_personal_name_status"].ToString().Trim() != string.Empty ? Convert.ToBoolean(dt.Rows[0]["co_worker_personal_name_status"]) : false;
                    appRecordVM.CoWorkerPersonalContactStatus = dt.Rows[0]["co_worker_personal_contact_status"].ToString().Trim() != string.Empty ? Convert.ToBoolean(dt.Rows[0]["co_worker_personal_contact_status"]) : false;
                    appRecordVM.CoWorkerRelationWithBorrowerStatus = dt.Rows[0]["co_worker_relation_with_borrower_status"].ToString().Trim() != string.Empty ? Convert.ToBoolean(dt.Rows[0]["co_worker_relation_with_borrower_status"]) : false;
                    appRecordVM.CoWorkerBorrowerKnownDurationStatus = dt.Rows[0]["co_worker_borrower_known_duration_status"].ToString().Trim() != string.Empty ? Convert.ToBoolean(dt.Rows[0]["co_worker_borrower_known_duration_status"]) : false;
                    appRecordVM.CoWorkerAddressVerificationStatus = dt.Rows[0]["co_worker_address_verification_status"].ToString().Trim() != string.Empty ? Convert.ToBoolean(dt.Rows[0]["co_worker_address_verification_status"]) : false;
                    appRecordVM.CoWorkerBorrowerWorkingPlaceStatus = dt.Rows[0]["co_worker_borrower_working_place"].ToString().Trim() != string.Empty ? Convert.ToBoolean(dt.Rows[0]["co_worker_borrower_working_place"]) : false;

                    appRecordVM.EmploymentNameOfWorkContact = dt.Rows[0]["employement_nameof_work_contact"].ToString().Trim() != string.Empty ? dt.Rows[0]["employement_nameof_work_contact"].ToString() : string.Empty;
                    appRecordVM.EmploymentNoOfWorkConatct = dt.Rows[0]["employement_no_of_work_contact"].ToString().Trim() != string.Empty ? dt.Rows[0]["employement_no_of_work_contact"].ToString() : string.Empty;
                    appRecordVM.EmploymentBorrowerWorking = dt.Rows[0]["employement_borrower_working"].ToString().Trim() != string.Empty ? dt.Rows[0]["employement_borrower_working"].ToString() : string.Empty;
                    appRecordVM.EmploymentBorrowerPosition = dt.Rows[0]["employement_borrower_position"].ToString().Trim() != string.Empty ? dt.Rows[0]["employement_borrower_position"].ToString() : string.Empty;
                    appRecordVM.EmploymentBorrowerMonthlySalary = dt.Rows[0]["employement_borrower_monthly_salary"].ToString().Trim() != string.Empty ? Convert.ToString(dt.Rows[0]["employement_borrower_monthly_salary"]) : string.Empty;
                    appRecordVM.EmploymentBorrowerAttendance = dt.Rows[0]["employement_borrower_attendance"].ToString().Trim() != string.Empty ? dt.Rows[0]["employement_borrower_attendance"].ToString() : string.Empty;
                    appRecordVM.EmploymentBorrowerBankPayroll = dt.Rows[0]["employement_borrower_bank_payroll"].ToString().Trim() != string.Empty ? dt.Rows[0]["employement_borrower_bank_payroll"].ToString() : string.Empty;
                    appRecordVM.EmploymentNameOfWorkContactStatus = dt.Rows[0]["employement_nameof_work_contact_status"].ToString().Trim() != string.Empty ? Convert.ToBoolean(dt.Rows[0]["employement_nameof_work_contact_status"]) : false;
                    appRecordVM.EmploymentNoOfWorkConatctStatus = dt.Rows[0]["employement_no_of_work_contact_status"].ToString().Trim() != string.Empty ? Convert.ToBoolean(dt.Rows[0]["employement_no_of_work_contact_status"]) : false;
                    appRecordVM.EmploymentBorrowerWorkingStatus = dt.Rows[0]["employement_borrower_working_status"].ToString().Trim() != string.Empty ? Convert.ToBoolean(dt.Rows[0]["employement_borrower_working_status"]) : false;
                    appRecordVM.EmploymentBorrowerPositionStatus = dt.Rows[0]["employement_borrower_position_status"].ToString().Trim() != string.Empty ? Convert.ToBoolean(dt.Rows[0]["employement_borrower_position_status"]) : false;
                    appRecordVM.EmploymentBorrowerMonthlySalaryStatus = dt.Rows[0]["employement_borrower_monthly_salary_status"].ToString().Trim() != string.Empty ? Convert.ToBoolean(dt.Rows[0]["employement_borrower_monthly_salary_status"]) : false;
                    appRecordVM.EmploymentBorrowerAttendanceStatus = dt.Rows[0]["employement_borrower_attendance_status"].ToString().Trim() != string.Empty ? Convert.ToBoolean(dt.Rows[0]["employement_borrower_attendance_status"]) : false;
                    appRecordVM.EmploymentBorrowerBankPayrollStatus = dt.Rows[0]["employement_borrower_bank_payroll_status"].ToString().Trim() != string.Empty ? Convert.ToBoolean(dt.Rows[0]["employement_borrower_bank_payroll_status"]) : false;

                    appRecordVM.Question1_Status = dt.Rows[0]["question1_status"].ToString().Trim() != string.Empty ? Convert.ToBoolean(dt.Rows[0]["question1_status"]) : false;
                    appRecordVM.Question2_Status = dt.Rows[0]["question2_status"].ToString().Trim() != string.Empty ? Convert.ToBoolean(dt.Rows[0]["question2_status"]) : false;
                    appRecordVM.Question3_Status = dt.Rows[0]["question3_status"].ToString().Trim() != string.Empty ? Convert.ToBoolean(dt.Rows[0]["question3_status"]) : false;
                    appRecordVM.Question4_Status = dt.Rows[0]["question4_status"].ToString().Trim() != string.Empty ? Convert.ToBoolean(dt.Rows[0]["question4_status"]) : false;
                    appRecordVM.Question5_Status = dt.Rows[0]["question5_status"].ToString().Trim() != string.Empty ? Convert.ToBoolean(dt.Rows[0]["question5_status"]) : false;
                    appRecordVM.Question6_Status = dt.Rows[0]["question6_status"].ToString().Trim() != string.Empty ? Convert.ToBoolean(dt.Rows[0]["question6_status"]) : false;
                    appRecordVM.Question7_Status = dt.Rows[0]["question7_status"].ToString().Trim() != string.Empty ? Convert.ToBoolean(dt.Rows[0]["question7_status"]) : false;
                    appRecordVM.Question8_Status = dt.Rows[0]["question8_status"].ToString().Trim() != string.Empty ? Convert.ToBoolean(dt.Rows[0]["question8_status"]) : false;
                    appRecordVM.Question9_Status = dt.Rows[0]["question9_status"].ToString().Trim() != string.Empty ? Convert.ToBoolean(dt.Rows[0]["question9_status"]) : false;
                    appRecordVM.Question10_Status = dt.Rows[0]["question10_status"].ToString().Trim() != string.Empty ? Convert.ToBoolean(dt.Rows[0]["question10_status"]) : false;

                    appRecordVM.ApprovedLoanAmount = dt.Rows[0]["approved_loan_amount"].ToString() != string.Empty ? Convert.ToDouble(dt.Rows[0]["approved_loan_amount"]) : 0.00;
                    appRecordVM.ApprovedTerm = dt.Rows[0]["approved_term"].ToString() != string.Empty ? Convert.ToInt32(dt.Rows[0]["approved_term"]) : 0;
                    appRecordVM.ApprovedInterestRate = dt.Rows[0]["approved_interest_rate"].ToString() != string.Empty ? Convert.ToDouble(dt.Rows[0]["approved_interest_rate"]) : 0.00;
                    appRecordVM.ApprovedMaturityDate = dt.Rows[0]["approved_maturity_date"].ToString() != string.Empty ? Convert.ToDateTime(dt.Rows[0]["approved_maturity_date"]).ToShortDateString().Replace('/', '-') : string.Empty;
                    appRecordVM.ApprovedTotalDueAmount = dt.Rows[0]["approved_total_amount_due"].ToString() != string.Empty ? Convert.ToDouble(dt.Rows[0]["approved_total_amount_due"]) : 0.00;
                    appRecordVM.term_value = dt.Rows[0]["term_value"].ToString().Trim() != string.Empty ? Convert.ToInt32(dt.Rows[0]["term_value"].ToString()) : 0;
                    appRecordVM.interest_rate = dt.Rows[0]["interest_rate"].ToString().Trim() != string.Empty ? Convert.ToDecimal(dt.Rows[0]["interest_rate"].ToString()) : 0;
                    appRecordVM.late_rate = dt.Rows[0]["late_rate"].ToString().Trim() != string.Empty ? Convert.ToDecimal(dt.Rows[0]["late_rate"].ToString()) : 0;
                    appRecordVM.lateFee = dt.Rows[0]["late_penalty"].ToString().Trim() != string.Empty ? Convert.ToDecimal(dt.Rows[0]["late_penalty"].ToString()) : 0;
                }
                return appRecordVM;
            }
            catch (Exception ex)
            {
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                throw ex;
            }

        }

        public string getCivilStatus(int civilid)
        {
            try
            {
                string CivilStatus = "No Status Selected";
                if (civilid > 0)
                {
                    DataTable dt = DbHelper.SelectMethod("select civilname from tblcivil_status where civilid='" + civilid + "';");
                    if (dt != null && dt.Rows.Count > 0)
                        CivilStatus = dt.Rows[0]["civilname"].ToString();
                }
                return CivilStatus;
            }
            catch (Exception ex)
            {
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                throw ex;
            }

        }

        public string getEmailId(int AppId)
        {
            try
            {
                string EmailId = string.Empty;
                if (AppId > 0)
                {
                    DataTable dt = DbHelper.SelectMethod("select personalemail from tblapplication_record a join tblapplication_user b on a.user_id=b.id where applicationno='" + AppId + "';");
                    if (dt != null && dt.Rows.Count > 0)
                        EmailId = dt.Rows[0]["personalemail"].ToString();
                }
                return EmailId;
            }
            catch (Exception ex)
            {
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                throw;
            }

        }

        private string GetTermType(string termType)
        {
            try
            {
                string str = string.Empty;
                DataTable dt = DbHelper.SelectMethod(string.Format(QueryHelper.GetTermTypeId, termType));
                if (dt != null && dt.Rows.Count > 0)
                {
                    str = Convert.ToString(dt.Rows[0]["termtype"]);
                }
                return str;
            }
            catch (Exception ex)
            {
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                throw ex;
            }

        }

        private string GetTermName(string term)
        {
            try
            {
                string str = string.Empty;
                DataTable dt = DbHelper.SelectMethod(string.Format(QueryHelper.GetTermTypebyId, term));
                if (dt != null && dt.Rows.Count > 0)
                {
                    str = Convert.ToString(dt.Rows[0]["term_name"]);
                }
                return str;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
    }
}