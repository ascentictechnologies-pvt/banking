using Loan_CRM.Areas.Checker.Models;
using Loan_CRM.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace Loan_CRM.Areas.Verifier.Controllers
{
    public class LoanRequestVerifierController : Controller
    {
        ApproverCommonOperation approverCommonOperation = new ApproverCommonOperation();
        /// <summary>
        /// Bucket list
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.Verifier))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            Session["_ActionName"] = "Index";
            try
            {
                int UserId = Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId].Value == null ? "0" : HttpContext.Request.Cookies[CookiesKey.UserId].Value);
                Session["VerifierKIVCount"] = false;
                var CountKIV = DbHelper.SelectMethod(string.Format(QueryHelper.GetVerifierKIV_Count, UserId));
                if (CountKIV != null && CountKIV.Rows.Count > 0)
                {
                    if (Convert.ToInt32(CountKIV.Rows[0]["count"]) > Convert.ToInt32(ConfigurationManager.AppSettings["VerifierBucket_LeadCount"]))
                    {
                        Session["VerifierKIVCount"] = true;
                    }
                    else
                    {
                        Session["VerifierKIVCount"] = false;
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
            }
            return View();
        }

        /// <summary>
        /// ReVerifier Bucket List
        /// </summary>
        /// <returns></returns>
        public ActionResult GetLoanReVerifierData()
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.Verifier))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            JsonResult jsonResult = new JsonResult();
            List<ApplicationRecordVM> appRecordVM = new List<ApplicationRecordVM>();
            try
            {

                DataTable dt;
                int UserId = 0;
                if (HttpContext.Request.Cookies[CookiesKey.UserId].Value != null)
                    UserId = Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value);
                TempData["Record"] = "Get Method";
                dt = DbHelper.SelectMethod(string.Format(QueryHelper.GetApplicationforReVerifier, UserId));
                if (dt != null && dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        string Address = Convert.ToString(dt.Rows[i]["address"]) != string.Empty ? Convert.ToString(dt.Rows[i]["address"]) : string.Empty;
                        string Province = Convert.ToString(dt.Rows[i]["province"]) != string.Empty ? Convert.ToString(dt.Rows[i]["province"]) : string.Empty;
                        string SSS_No = Convert.ToString(dt.Rows[i]["sss_no"]) != string.Empty ? Convert.ToString(dt.Rows[i]["sss_no"]) : string.Empty;
                        string Street = Convert.ToString(dt.Rows[i]["street"]) != string.Empty ? Convert.ToString(dt.Rows[i]["street"]) : string.Empty;
                        string Barangay = Convert.ToString(dt.Rows[i]["barangay"]) != string.Empty ? Convert.ToString(dt.Rows[i]["barangay"]) : string.Empty;
                        string CityName = CommonMethods.GetCityName(Convert.ToString(dt.Rows[i]["city"]) != string.Empty ? Convert.ToInt32(dt.Rows[i]["city"]) : 0);
                        string ZipCode = Convert.ToString(dt.Rows[i]["zipcode"]) != string.Empty ? Convert.ToString(dt.Rows[i]["zipcode"]) : string.Empty;
                        string CompleteAddress = Address + ", " + Street + ", " + Barangay + ", " + CityName + ", " + ZipCode + "," + Province + "," + SSS_No;
                        ApplicationRecordVM checkdetail = new ApplicationRecordVM();
                        //Best Time to Morning Call
                        int BestTimeToCallMorning = Convert.ToString(dt.Rows[i]["morning_time"]).Trim() == string.Empty ? -1 : Convert.ToInt32(dt.Rows[i]["morning_time"]);
                        if (BestTimeToCallMorning != -1)
                            checkdetail.MorningTime = CommonMethods.GetBestTimeToCallMorning(BestTimeToCallMorning);
                        else
                            checkdetail.MorningTime = string.Empty;

                        //Best Time to Noon Call
                        int BestTimeToCallNoon = Convert.ToString(dt.Rows[i]["noon_time"]).Trim() == string.Empty ? -1 : Convert.ToInt32(dt.Rows[i]["noon_time"]);
                        if (BestTimeToCallNoon != -1)
                            checkdetail.NoonTime = CommonMethods.GetBestTimeToCallNoon(BestTimeToCallNoon);
                        else
                            checkdetail.NoonTime = string.Empty;

                        checkdetail.ApplicationNo = Convert.ToInt32(dt.Rows[i]["applicationno"].ToString());
                        checkdetail.First_Name = dt.Rows[i]["first_name"].ToString();
                        checkdetail.Middle_Name = dt.Rows[i]["middle_name"].ToString();
                        checkdetail.Last_Name = dt.Rows[i]["last_name"].ToString();
                        checkdetail.PersonalEmail = dt.Rows[i]["personalemail"].ToString();
                        checkdetail.PersonalContactNo = dt.Rows[i]["personalcontactno"].ToString();
                        checkdetail.CompanyName = dt.Rows[i]["companyname"].ToString();
                        checkdetail.Designation = dt.Rows[i]["designation"].ToString();
                        checkdetail.ispickedReverifier = Convert.ToBoolean(dt.Rows[i]["ispickedreverifier"]);
                        checkdetail.userfullname = Convert.ToString(dt.Rows[i]["userfullname"]) != string.Empty ? Convert.ToString(dt.Rows[i]["userfullname"]) : string.Empty;
                        checkdetail.CreatedOn = Convert.ToString(dt.Rows[i]["createdon"]);
                        checkdetail.CheckedOn = Convert.ToString(dt.Rows[i]["checkedon"]);

                        appRecordVM.Add(checkdetail);
                    }
                }
                jsonResult = Json(appRecordVM, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
            }
            catch (Exception ex)
            {
                jsonResult = Json(null, JsonRequestBehavior.AllowGet);
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
            }

            return jsonResult;
        }

        /// <summary>
        /// ReVerifier Bucket List
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetLoanRequestForVerification()
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.Verifier))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            Session["_ActionName"] = "GetLoanRequestForVerification";
            return View();
        }

        /// <summary>
        /// Verification Window
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult VerificationWindow()
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.Verifier))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }

            CompleteAppVerificationDetails completeDetails = new CompleteAppVerificationDetails();
            try
            {
                string applicationNo = Request.QueryString["Id"];
                ActivityLog.Info($"Application {applicationNo} Get on Verifer Window by User {Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value)}", $"{this.GetType().Name} ==> {System.Reflection.MethodBase.GetCurrentMethod().Name}");
                int intId = 0;
                if (!string.IsNullOrWhiteSpace(applicationNo) || applicationNo != null)
                {
                    intId = Convert.ToInt32(applicationNo);
                    #region Update VerifyBy

                    if (HttpContext.Request.Cookies[CookiesKey.UserId].Value != null)
                    {
                        int UserId = Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value);
                        DbHelper.InsertUpdateDelete(string.Format(QueryHelper.UpdateVerifyBy, UserId, applicationNo));
                    }
                    #endregion
                }

                DataTable dt;
                string ActionName = Request.QueryString["ActionName"];
                DataTable dtBank = new DataTable();
                DataTable dtOccupation = new DataTable();
                dtBank = DbHelper.SelectMethod(QueryHelper.GetAllBank);
                if (dtBank != null && dtBank.Rows.Count > 0)
                {
                    for (int i = 0; i < dtBank.Rows.Count; i++)
                    {
                        completeDetails.BankMasterModelList.Add(new BankMasterModel
                        {
                            bank_id = Convert.ToInt32(dtBank.Rows[i]["bank_id"]),
                            bank_code = Convert.ToString(dtBank.Rows[i]["bank_code"]),
                            bank_name = Convert.ToString(dtBank.Rows[i]["bank_name"]),
                            bank_name_with_bank_code = Convert.ToString(dtBank.Rows[i]["bank_name_with_bank_code"]),
                            is_active = Convert.ToBoolean(dtBank.Rows[i]["is_active"]),
                            created_on = Convert.ToString(dtBank.Rows[i]["created_on"]),
                            updated_on = Convert.ToString(dtBank.Rows[i]["updated_on"])
                        });
                    }
                }
                dtOccupation = DbHelper.SelectMethod(QueryHelper.GetOccupation_Active);
                if (dtOccupation != null && dtOccupation.Rows.Count > 0)
                {
                    for (int i = 0; i < dtOccupation.Rows.Count; i++)
                    {
                        completeDetails.OccupationModelList.Add(new OccupationModel
                        {
                            id = Convert.ToInt32(dtOccupation.Rows[i]["id"]),
                            occupation_name = Convert.ToString(dtOccupation.Rows[i]["occupation_name"]),
                            isactive = Convert.ToBoolean(dtOccupation.Rows[i]["isactive"]),
                            created_on = Convert.ToString(dtOccupation.Rows[i]["created_on"])
                        });
                    }
                }
                completeDetails.RemarkModelList.AddRange(CommonMethods.GetRemarkList(Convert.ToInt32(applicationNo)));

                if (ActionName != string.Empty)
                {
                    Session["ActionName"] = ActionName;
                }
                if (ActionName == "Index" || ActionName == "KIVBucket")
                {
                    dt = DbHelper.SelectMethod(string.Format(QueryHelper.GetDetailsforApprover, intId));
                }
                else
                {
                    dt = DbHelper.SelectMethod(string.Format(QueryHelper.GetDetailsforApproverFromReVerifier, intId));
                }

                if (dt != null && dt.Rows.Count > 0)
                {
                    if (Convert.ToBoolean(dt.Rows[0]["isverified"]))
                    {
                        TempData["msg"] = "Application Already Verified.";
                    }
                    else
                    {
                        TempData["msg"] = null;
                    }
                    completeDetails.jumioreference = Convert.ToString(dt.Rows[0]["jumioreference"]);
                    if (!String.IsNullOrWhiteSpace(completeDetails.jumioreference))
                    {
                        var _TransactionData = DbHelper.SelectMethod(String.Format(QueryHelper.GetTransactionId, completeDetails.jumioreference));
                        if (_TransactionData != null && _TransactionData.Rows.Count > 0)
                        {
                            completeDetails.transactionid = Convert.ToString(_TransactionData.Rows[0]["transcationid"]);
                        }
                    }
                    completeDetails.notificationtoken = Convert.ToString(dt.Rows[0]["notificationtoken"]);
                    completeDetails.facemapstatus = Convert.ToString(dt.Rows[0]["facemapstatus"]);
                    completeDetails.checker_name = dt.Rows[0]["userfullname"].ToString().Trim() != string.Empty ? dt.Rows[0]["userfullname"].ToString() : string.Empty;
                    string TermType = dt.Rows[0]["termtype"].ToString() != string.Empty ? Convert.ToString(dt.Rows[0]["termtype"]) : string.Empty;
                    if (!String.IsNullOrWhiteSpace(TermType))
                        TermType = GetTermType(TermType);
                    completeDetails.TermType = TermType;
                    string TermName = dt.Rows[0]["term"].ToString() != string.Empty ? Convert.ToString(dt.Rows[0]["term"]) : string.Empty;
                    completeDetails.TermName = dt.Rows[0]["term"].ToString() != string.Empty ? Convert.ToInt32(dt.Rows[0]["term"]) : -1;
                    if (!String.IsNullOrWhiteSpace(TermName))
                        TermName = GetTermName(TermName);

                    string AdditionalRequestTermStr = TermType + ", " + TermName;

                    var joiningDate = !String.IsNullOrWhiteSpace(Convert.ToString(dt.Rows[0]["date_joining"])) ? Convert.ToDateTime(dt.Rows[0]["date_joining"]) : DateTime.Now;
                    var currentDate = DateTime.Now;
                    var dateDiff = ((currentDate.Year - Convert.ToDateTime(joiningDate).Year) * 12) + currentDate.Month - Convert.ToDateTime(joiningDate).Month;

                    completeDetails.ApplicationNo = dt.Rows[0]["applicationno"].ToString() != string.Empty ? Convert.ToInt32(dt.Rows[0]["applicationno"].ToString()) : 0;
                    completeDetails.DateApplied = Convert.ToString(dt.Rows[0]["dateapplied"]);
                    var firstName = dt.Rows[0]["first_name"].ToString() != string.Empty ? dt.Rows[0]["first_name"].ToString() : string.Empty;
                    var middleName = dt.Rows[0]["middle_name"].ToString() != string.Empty ? dt.Rows[0]["middle_name"].ToString() : string.Empty;
                    var lastName = dt.Rows[0]["last_name"].ToString() != string.Empty ? dt.Rows[0]["last_name"].ToString() : string.Empty;
                    completeDetails.Name = firstName + " " + middleName + " " + lastName;
                    completeDetails.gov_doc_type = Convert.ToString(dt.Rows[0]["gov_doc_type"]);
                    completeDetails.PersonalEmail = dt.Rows[0]["personalemail"].ToString() != string.Empty ? dt.Rows[0]["personalemail"].ToString() : string.Empty;
                    completeDetails.PersonalContactNo = dt.Rows[0]["personalcontactno"].ToString() != string.Empty ? dt.Rows[0]["personalcontactno"].ToString() : string.Empty;
                    completeDetails.gender = dt.Rows[0]["gender"].ToString() != string.Empty ? dt.Rows[0]["gender"].ToString() : string.Empty;
                    completeDetails.RelativeContactNo = dt.Rows[0]["relativecontactno"].ToString() != string.Empty ? dt.Rows[0]["relativecontactno"].ToString() : string.Empty;
                    completeDetails.CoworkerContactNo = dt.Rows[0]["coworkercontactno"].ToString() != string.Empty ? dt.Rows[0]["coworkercontactno"].ToString() : string.Empty;
                    completeDetails.Address = dt.Rows[0]["address"].ToString() != string.Empty ? dt.Rows[0]["address"].ToString() : string.Empty;
                    completeDetails.HomePhone = dt.Rows[0]["homephoneno"].ToString() != string.Empty ? dt.Rows[0]["homephoneno"].ToString() : string.Empty;
                    completeDetails.BirthPlace = dt.Rows[0]["placeofbirth"].ToString() != string.Empty ? dt.Rows[0]["placeofbirth"].ToString() : string.Empty;
                    completeDetails.DOB = Convert.ToString(dt.Rows[0]["date_of_birth"]) != string.Empty ? Convert.ToString(dt.Rows[0]["date_of_birth"]) : string.Empty;
                    completeDetails.CivilStatus = dt.Rows[0]["civilstatus"].ToString() != string.Empty ? Convert.ToInt32(dt.Rows[0]["civilstatus"].ToString()) : 0;
                    completeDetails.CivilStatusText = approverCommonOperation.getCivilStatus(completeDetails.CivilStatus);
                    completeDetails.MotherMaidenName = dt.Rows[0]["mothermaidenname"].ToString() != string.Empty ? dt.Rows[0]["mothermaidenname"].ToString() : string.Empty;
                    completeDetails.MotherAddress = dt.Rows[0]["motheraddress"].ToString() != string.Empty ? dt.Rows[0]["motheraddress"].ToString() : string.Empty;
                    completeDetails.CompanyName = dt.Rows[0]["companyname"].ToString() != string.Empty ? dt.Rows[0]["companyname"].ToString() : string.Empty;
                    completeDetails.CompanyAddress = dt.Rows[0]["companyaddress"].ToString() != string.Empty ? dt.Rows[0]["companyaddress"].ToString() : string.Empty;
                    completeDetails.Designation = dt.Rows[0]["designation"].ToString() != string.Empty ? dt.Rows[0]["designation"].ToString() : string.Empty;
                    completeDetails.GrossIncome = dt.Rows[0]["gross_income"].ToString() != string.Empty ? Convert.ToDouble(dt.Rows[0]["gross_income"].ToString()) : 0;
                    completeDetails.RefName = dt.Rows[0]["reference_name"].ToString() != string.Empty ? dt.Rows[0]["reference_name"].ToString() : string.Empty;
                    completeDetails.RefContactNo = dt.Rows[0]["reference_contactno"].ToString() != string.Empty ? dt.Rows[0]["reference_contactno"].ToString() : string.Empty;
                    completeDetails.BankName = dt.Rows[0]["bankname"].ToString() != string.Empty ? dt.Rows[0]["bankname"].ToString() : string.Empty;
                    completeDetails.BankAccNo = dt.Rows[0]["bankaccountno"].ToString() != string.Empty ? dt.Rows[0]["bankaccountno"].ToString() : string.Empty;
                    completeDetails.Password = dt.Rows[0]["user_password"].ToString() != string.Empty ? dt.Rows[0]["user_password"].ToString() : string.Empty;
                    completeDetails.City = dt.Rows[0]["city"].ToString() != string.Empty ? Convert.ToInt32(dt.Rows[0]["city"].ToString()) : 0;
                    if (completeDetails.City != 0)
                    {
                        completeDetails.CityName = CommonMethods.GetCityName(completeDetails.City);
                    }

                    else
                    { completeDetails.CityName = ""; }
                    completeDetails.Ischeck = Convert.ToBoolean(dt.Rows[0]["ischeck"].ToString());
                    completeDetails.Isverified = Convert.ToBoolean(dt.Rows[0]["isverified"].ToString());
                    completeDetails.Isrecheck = Convert.ToBoolean(dt.Rows[0]["isrecheck"].ToString());
                    completeDetails.Isreverified = Convert.ToBoolean(dt.Rows[0]["isreverified"].ToString());
                    completeDetails.Isapproved = Convert.ToBoolean(dt.Rows[0]["isapproved"].ToString());
                    completeDetails.RelativeName = dt.Rows[0]["relativename"].ToString() != string.Empty ? dt.Rows[0]["relativename"].ToString() : string.Empty;
                    completeDetails.CoWorkerName = dt.Rows[0]["coworkername"].ToString() != string.Empty ? dt.Rows[0]["coworkername"].ToString() : string.Empty;
                    completeDetails.RelationWithRelative = dt.Rows[0]["relationwithrelative"] != null ? dt.Rows[0]["relationwithrelative"].ToString() : string.Empty;
                    completeDetails.friendName = dt.Rows[0]["friendname"].ToString() != string.Empty ? dt.Rows[0]["friendname"].ToString() : string.Empty;
                    completeDetails.Suffix = dt.Rows[0]["suffix"].ToString() != string.Empty ? dt.Rows[0]["suffix"].ToString() : string.Empty;
                    completeDetails.Street = dt.Rows[0]["street"].ToString() != string.Empty ? dt.Rows[0]["street"].ToString() : string.Empty;
                    completeDetails.Barangay = dt.Rows[0]["barangay"].ToString() != string.Empty ? dt.Rows[0]["barangay"].ToString() : string.Empty;
                    if (completeDetails.Barangay != "")
                    {
                        int BarangayId = 0;
                        BarangayId = Convert.ToInt32(completeDetails.Barangay);
                        completeDetails.BarangayName = CommonMethods.GetBarangayName(BarangayId);
                    }

                    else
                    { completeDetails.BarangayName = ""; }

                    completeDetails.Province = dt.Rows[0]["province"].ToString() != string.Empty ? dt.Rows[0]["province"].ToString() : string.Empty;
                    if (completeDetails.Province != "")
                    {
                        int ProvinceId = 0;
                        ProvinceId = Convert.ToInt32(completeDetails.Province);
                        completeDetails.ProvinceName = CommonMethods.GetProvinceName(ProvinceId);
                    }

                    else
                    { completeDetails.ProvinceName = ""; }

                    completeDetails.ZipCode = dt.Rows[0]["zipcode"].ToString() != string.Empty ? dt.Rows[0]["zipcode"].ToString() : string.Empty;
                    completeDetails.Company_Phoneno = dt.Rows[0]["company_phoneno"].ToString() != string.Empty ? dt.Rows[0]["company_phoneno"].ToString() : string.Empty;
                    completeDetails.Friend_ContactNo = dt.Rows[0]["friend_contactno"].ToString() != string.Empty ? dt.Rows[0]["friend_contactno"].ToString() : string.Empty;
                    completeDetails.AdditionalPayDate = dt.Rows[0]["pay_date"].ToString() != string.Empty ? Convert.ToDateTime(dt.Rows[0]["pay_date"]).ToShortDateString() : string.Empty;
                    completeDetails.AdditionalRequested_Term = AdditionalRequestTermStr;
                    completeDetails.Loan_Amount = dt.Rows[0]["loanamount"].ToString() != string.Empty ? Convert.ToDouble(dt.Rows[0]["loanamount"].ToString()) : 0.00;
                    completeDetails.AdditionalRequestedLoanAmount = dt.Rows[0]["loanamount"].ToString() != string.Empty ? Convert.ToDouble(dt.Rows[0]["loanamount"].ToString()) : 0.00;
                    completeDetails.AdditionalEmployedDuration = Convert.ToString(dateDiff);
                    completeDetails.AdditionalJobLevel = Convert.ToString(dt.Rows[0]["designation"]) != string.Empty ? Convert.ToString(dt.Rows[0]["designation"]) : string.Empty;
                    completeDetails.AdditionalJobPosition = Convert.ToString(dt.Rows[0]["designation"]) != string.Empty ? Convert.ToString(dt.Rows[0]["designation"]) : string.Empty;
                    completeDetails.userid = Convert.ToInt32(dt.Rows[0]["user_id"]);
                    DataTable _CredoDt = CommonMethods.GettCredoScore(Convert.ToInt32(dt.Rows[0]["user_id"]));
                    if (_CredoDt != null && _CredoDt.Rows.Count > 0)
                    {
                        completeDetails.CredoModel = new CredoModel()
                        {
                            Probability = (string)_CredoDt.Rows[0]["probability"],
                            Score = (string)_CredoDt.Rows[0]["score"]
                        };
                    }

                    completeDetails.payDate1 = Convert.ToString(dt.Rows[0]["paydate1"]) != string.Empty ? Convert.ToString(dt.Rows[0]["paydate1"]) : string.Empty;
                    completeDetails.payDate2 = Convert.ToString(dt.Rows[0]["paydate2"]) != string.Empty ? Convert.ToString(dt.Rows[0]["paydate2"]) : string.Empty;
                    completeDetails.GovUrl = Convert.ToString(dt.Rows[0]["gov_id_url"]);
                    completeDetails.CompUrl = Convert.ToString(dt.Rows[0]["companyid_url"]);
                    completeDetails.BillUrl = Convert.ToString(dt.Rows[0]["billing_url"]);
                    completeDetails.IncomeUrl = Convert.ToString(dt.Rows[0]["income_url"]);
                    completeDetails.OthUrl = Convert.ToString(dt.Rows[0]["other_url"]);
                    completeDetails.ATMUrl = Convert.ToString(dt.Rows[0]["atm_url"]);
                    completeDetails.EmploymentNoOfWorkConatct = Convert.ToString(dt.Rows[0]["company_phoneno"]) != string.Empty ? Convert.ToString(dt.Rows[0]["company_phoneno"]) : string.Empty;
                    completeDetails.term_value = Convert.ToString(dt.Rows[0]["term_value"]) == "" ? 0 : Convert.ToInt32(dt.Rows[0]["term_value"]);
                    completeDetails.interest_rate = Convert.ToString(dt.Rows[0]["interest_rate"]) == "" ? 0 : Convert.ToDecimal(dt.Rows[0]["interest_rate"]);
                    completeDetails.late_rate = Convert.ToString(dt.Rows[0]["interest_rate"]) == "" ? 0 : Convert.ToDecimal(dt.Rows[0]["late_rate"]);
                    completeDetails.lateFee = TermType.ToLower() == "weekly" ? Convert.ToInt32(ConfigurationManager.AppSettings["WeeklyFee"]) : TermType.ToLower() == "bi-weekly" ? Convert.ToInt32(ConfigurationManager.AppSettings["BiWeeklyFee"]) : TermType.ToLower() == "monthly" ? Convert.ToInt32(ConfigurationManager.AppSettings["MonthlyFee"]) : 0;
                    completeDetails.ApprovedLoanAmount = dt.Rows[0]["approved_loan_amount"].ToString() != string.Empty ? Convert.ToDouble(dt.Rows[0]["approved_loan_amount"]) : 0.00;
                    completeDetails.ApprovedTerm = dt.Rows[0]["approved_term"].ToString() != string.Empty ? Convert.ToDouble(dt.Rows[0]["approved_term"]) : 0.00;
                    completeDetails.ApprovedInterestRate = dt.Rows[0]["approved_interest_rate"].ToString() != string.Empty ? Convert.ToDouble(dt.Rows[0]["approved_interest_rate"]) : 0.00;
                    completeDetails.ApprovedMaturityDate = Convert.ToString(dt.Rows[0]["approvedmaturitydate"]) != string.Empty ? Convert.ToString(dt.Rows[0]["approvedmaturitydate"]) : string.Empty;
                    completeDetails.ApprovedTotalDueAmount = dt.Rows[0]["approved_total_amount_due"].ToString() != string.Empty ? Convert.ToDouble(dt.Rows[0]["approved_total_amount_due"]) : 0.00;

                    #region Additiona Field Changes

                    #region New Fields
                    completeDetails.nearest_landmark = dt.Rows[0]["nearest_landmark"].ToString().Trim() != string.Empty ? dt.Rows[0]["nearest_landmark"].ToString() : string.Empty;
                    completeDetails.SSS_No = dt.Rows[0]["sss_no"].ToString().Trim() != string.Empty ? dt.Rows[0]["sss_no"].ToString() : string.Empty;
                    completeDetails.home_status = dt.Rows[0]["home_status"].ToString().Trim() != string.Empty ? Convert.ToInt32(dt.Rows[0]["home_status"].ToString()) : 0;
                    completeDetails.Permanent_address = dt.Rows[0]["permanent_address"].ToString().Trim() != string.Empty ? dt.Rows[0]["permanent_address"].ToString() : string.Empty;
                    completeDetails.tansfer_residence = dt.Rows[0]["tansfer_residence"].ToString().Trim() != string.Empty ? dt.Rows[0]["tansfer_residence"].ToString() : string.Empty;
                    completeDetails.spouse_name = dt.Rows[0]["spouse_name"].ToString().Trim() != string.Empty ? dt.Rows[0]["spouse_name"].ToString() : string.Empty;
                    completeDetails.spouse_occupation = dt.Rows[0]["spouse_occupation"].ToString().Trim() != string.Empty ? dt.Rows[0]["spouse_occupation"].ToString() : string.Empty;
                    completeDetails.number_of_dependent = dt.Rows[0]["number_of_dependent"] == DBNull.Value ? 0 : Convert.ToInt32(dt.Rows[0]["number_of_dependent"]);
                    completeDetails.mother_work = dt.Rows[0]["mother_work"].ToString().Trim() != string.Empty ? dt.Rows[0]["mother_work"].ToString() : string.Empty;
                    completeDetails.father_name = dt.Rows[0]["father_name"].ToString().Trim() != string.Empty ? dt.Rows[0]["father_name"].ToString() : string.Empty;
                    completeDetails.father_work = dt.Rows[0]["father_work"].ToString().Trim() != string.Empty ? dt.Rows[0]["father_work"].ToString() : string.Empty;
                    completeDetails.living_with_mother = dt.Rows[0]["living_with_mother"].ToString().Trim() != string.Empty ? dt.Rows[0]["living_with_mother"].ToString() : string.Empty;
                    completeDetails.sibling_count = dt.Rows[0]["sibling_count"] == DBNull.Value ? 0 : Convert.ToInt32(dt.Rows[0]["sibling_count"]);
                    completeDetails.sibling_works = dt.Rows[0]["sibling_works"].ToString().Trim() != string.Empty ? dt.Rows[0]["sibling_works"].ToString() : string.Empty;
                    completeDetails.occupation = dt.Rows[0]["industry"].ToString().Trim() != string.Empty ? dt.Rows[0]["industry"].ToString() : string.Empty;

                    //Best Time to Morning Call
                    int BestTimeToCallMorning = Convert.ToString(dt.Rows[0]["morning_time"]).Trim() == string.Empty ? -1 : Convert.ToInt32(dt.Rows[0]["morning_time"]);
                    if (BestTimeToCallMorning != -1)
                        completeDetails.MorningTime = CommonMethods.GetBestTimeToCallMorning(BestTimeToCallMorning);
                    else
                        completeDetails.MorningTime = string.Empty;

                    //Best Time to Noon Call
                    int BestTimeToCallNoon = Convert.ToString(dt.Rows[0]["noon_time"]).Trim() == string.Empty ? -1 : Convert.ToInt32(dt.Rows[0]["noon_time"]);
                    if (BestTimeToCallNoon != -1)
                        completeDetails.NoonTime = CommonMethods.GetBestTimeToCallNoon(BestTimeToCallNoon);
                    else
                        completeDetails.NoonTime = string.Empty;

                    completeDetails.net_income = dt.Rows[0]["net_income"].ToString() != string.Empty ? Convert.ToDouble(dt.Rows[0]["net_income"]) : 0.00;
                    completeDetails.pending_resignation = dt.Rows[0]["pending_resignation"].ToString().Trim() != string.Empty ? dt.Rows[0]["pending_resignation"].ToString() : string.Empty;
                    completeDetails.other_source_of_income = dt.Rows[0]["other_source_of_income"].ToString().Trim() != string.Empty ? dt.Rows[0]["other_source_of_income"].ToString() : string.Empty;
                    completeDetails.know_about_cashmart = dt.Rows[0]["know_about_cashmart"].ToString().Trim() != string.Empty ? dt.Rows[0]["know_about_cashmart"].ToString() : string.Empty;
                    completeDetails.pending_loan_fron_otherland = dt.Rows[0]["pending_loan_fron_otherland"].ToString().Trim() != string.Empty ? dt.Rows[0]["pending_loan_fron_otherland"].ToString() : string.Empty;
                    completeDetails.bank_loan_or_credit_card = dt.Rows[0]["bank_loan_or_credit_card"].ToString().Trim() != string.Empty ? dt.Rows[0]["bank_loan_or_credit_card"].ToString() : string.Empty;
                    completeDetails.Permanent_address = dt.Rows[0]["permanent_address"].ToString().Trim() != string.Empty ? dt.Rows[0]["permanent_address"].ToString() : string.Empty;
                    completeDetails.BankId = Convert.ToString(dt.Rows[0]["bankid"]) == "" ? 0 : Convert.ToInt32(dt.Rows[0]["bankid"]);
                    completeDetails.family_name1 = dt.Rows[0]["family_name1"].ToString().Trim() != string.Empty ? dt.Rows[0]["family_name1"].ToString() : string.Empty;
                    completeDetails.family_address1 = dt.Rows[0]["family_address1"].ToString().Trim() != string.Empty ? dt.Rows[0]["family_address1"].ToString() : string.Empty;
                    completeDetails.family_contact1 = dt.Rows[0]["family_contact1"].ToString().Trim() != string.Empty ? dt.Rows[0]["family_contact1"].ToString() : string.Empty;
                    completeDetails.family_relation1 = dt.Rows[0]["family_relation1"].ToString().Trim() != string.Empty ? dt.Rows[0]["family_relation1"].ToString() : string.Empty;
                    completeDetails.family_name2 = dt.Rows[0]["family_name2"].ToString().Trim() != string.Empty ? dt.Rows[0]["family_name2"].ToString() : string.Empty;
                    completeDetails.family_address2 = dt.Rows[0]["family_address2"].ToString().Trim() != string.Empty ? dt.Rows[0]["family_address2"].ToString() : string.Empty;
                    completeDetails.family_contact2 = dt.Rows[0]["family_contact2"].ToString().Trim() != string.Empty ? dt.Rows[0]["family_contact2"].ToString() : string.Empty;
                    completeDetails.family_relation2 = dt.Rows[0]["family_relation2"].ToString().Trim() != string.Empty ? dt.Rows[0]["family_relation2"].ToString() : string.Empty;
                    completeDetails.optional_name = dt.Rows[0]["optional_name"].ToString().Trim() != string.Empty ? dt.Rows[0]["optional_name"].ToString() : string.Empty;
                    completeDetails.optional_address = dt.Rows[0]["optional_address"].ToString().Trim() != string.Empty ? dt.Rows[0]["optional_address"].ToString() : string.Empty;
                    completeDetails.optional_contact = dt.Rows[0]["optional_contact"].ToString().Trim() != string.Empty ? dt.Rows[0]["optional_contact"].ToString() : string.Empty;
                    completeDetails.optional_relation = dt.Rows[0]["optional_relation"].ToString().Trim() != string.Empty ? dt.Rows[0]["optional_relation"].ToString() : string.Empty;
                    completeDetails.checker_oic_on = dt.Rows[0]["checker_oic_on"].ToString().Trim() != string.Empty ? dt.Rows[0]["checker_oic_on"].ToString() : string.Empty;
                    completeDetails.disbursement_date = dt.Rows[0]["disbursement_date"].ToString().Trim() != string.Empty ? dt.Rows[0]["disbursement_date"].ToString() : string.Empty;

                    #endregion

                    #region New Fields Remarks
                    completeDetails.nearest_landmark_remark = dt.Rows[0]["nearest_landmark_remark"].ToString().Trim() != string.Empty ? dt.Rows[0]["nearest_landmark_remark"].ToString() : string.Empty;
                    completeDetails.SSS_No_remark = dt.Rows[0]["sss_no_remark"].ToString().Trim() != string.Empty ? dt.Rows[0]["sss_no_remark"].ToString() : string.Empty;
                    completeDetails.rented_mortgage_owned_remark = dt.Rows[0]["rented_mortgage_owned_remark"].ToString().Trim() != string.Empty ? dt.Rows[0]["rented_mortgage_owned_remark"].ToString() : string.Empty;
                    completeDetails.Permanent_address_remark = dt.Rows[0]["permanent_address_remark"].ToString().Trim() != string.Empty ? dt.Rows[0]["permanent_address_remark"].ToString() : string.Empty;
                    completeDetails.tansfer_residence_remark = dt.Rows[0]["tansfer_residence_remark"].ToString().Trim() != string.Empty ? dt.Rows[0]["tansfer_residence_remark"].ToString() : string.Empty;
                    completeDetails.spouse_name_remark = dt.Rows[0]["spouse_name_remark"].ToString().Trim() != string.Empty ? dt.Rows[0]["spouse_name_remark"].ToString() : string.Empty;
                    completeDetails.spouse_occupation_remark = dt.Rows[0]["spouse_occupation_remark"].ToString().Trim() != string.Empty ? dt.Rows[0]["spouse_occupation_remark"].ToString() : string.Empty;
                    completeDetails.number_of_dependent_remark = dt.Rows[0]["number_of_dependent_remark"] == null ? "" : Convert.ToString(dt.Rows[0]["number_of_dependent_remark"]);
                    completeDetails.father_name_work_remark = dt.Rows[0]["father_name_work_remark"].ToString().Trim() != string.Empty ? dt.Rows[0]["father_name_work_remark"].ToString() : string.Empty;
                    completeDetails.sibling_works_remark = dt.Rows[0]["sibling_works_remark"].ToString().Trim() != string.Empty ? dt.Rows[0]["sibling_works_remark"].ToString() : string.Empty;
                    completeDetails.occupation_remark = dt.Rows[0]["occupation_remark"].ToString().Trim() != string.Empty ? dt.Rows[0]["occupation_remark"].ToString() : string.Empty;
                    completeDetails.scheduled_and_btc_remark = dt.Rows[0]["scheduled_and_btc_remark"].ToString().Trim() != string.Empty ? dt.Rows[0]["scheduled_and_btc_remark"].ToString() : string.Empty;
                    completeDetails.net_income_remark = dt.Rows[0]["net_income_remark"].ToString().Trim() != string.Empty ? dt.Rows[0]["net_income_remark"].ToString() : string.Empty;
                    completeDetails.pending_resignation_remark = dt.Rows[0]["pending_resignation_remark"].ToString().Trim() != string.Empty ? dt.Rows[0]["pending_resignation_remark"].ToString() : string.Empty;
                    completeDetails.other_source_of_income_remark = dt.Rows[0]["other_source_of_income_remark"].ToString().Trim() != string.Empty ? dt.Rows[0]["other_source_of_income_remark"].ToString() : string.Empty;
                    completeDetails.know_about_cashmart_remark = dt.Rows[0]["know_about_cashmart_remark"].ToString().Trim() != string.Empty ? dt.Rows[0]["know_about_cashmart_remark"].ToString() : string.Empty;
                    completeDetails.pending_loan_fron_otherland_remark = dt.Rows[0]["pending_loan_fron_otherland_remark"].ToString().Trim() != string.Empty ? dt.Rows[0]["pending_loan_fron_otherland_remark"].ToString() : string.Empty;
                    completeDetails.bank_loan_or_credit_card_remark = dt.Rows[0]["bank_loan_or_credit_card_remark"].ToString().Trim() != string.Empty ? dt.Rows[0]["bank_loan_or_credit_card_remark"].ToString() : string.Empty;
                    completeDetails.personal_bank_name_remark = dt.Rows[0]["personal_bank_name_remark"].ToString().Trim() != string.Empty ? dt.Rows[0]["personal_bank_name_remark"].ToString() : string.Empty;
                    completeDetails.family_name1_remark = dt.Rows[0]["family_name1_remark"].ToString().Trim() != string.Empty ? dt.Rows[0]["family_name1_remark"].ToString() : string.Empty;
                    completeDetails.family_address1_remark = dt.Rows[0]["family_address1_remark"].ToString().Trim() != string.Empty ? dt.Rows[0]["family_address1_remark"].ToString() : string.Empty;
                    completeDetails.family_contact1_remark = dt.Rows[0]["family_contact1_remark"].ToString().Trim() != string.Empty ? dt.Rows[0]["family_contact1_remark"].ToString() : string.Empty;
                    completeDetails.family_relation1_remark = dt.Rows[0]["family_relation1_remark"].ToString().Trim() != string.Empty ? dt.Rows[0]["family_relation1_remark"].ToString() : string.Empty;
                    completeDetails.family_name2_remark = dt.Rows[0]["family_name2_remark"].ToString().Trim() != string.Empty ? dt.Rows[0]["family_name2_remark"].ToString() : string.Empty;
                    completeDetails.family_address2_remark = dt.Rows[0]["family_address2_remark"].ToString().Trim() != string.Empty ? dt.Rows[0]["family_address2_remark"].ToString() : string.Empty;
                    completeDetails.family_contact2_remark = dt.Rows[0]["family_contact2_remark"].ToString().Trim() != string.Empty ? dt.Rows[0]["family_contact2_remark"].ToString() : string.Empty;
                    completeDetails.family_relation2_remark = dt.Rows[0]["family_relation2_remark"].ToString().Trim() != string.Empty ? dt.Rows[0]["family_relation2_remark"].ToString() : string.Empty;
                    completeDetails.optional_name_remark = dt.Rows[0]["optional_name_remark"].ToString().Trim() != string.Empty ? dt.Rows[0]["optional_name_remark"].ToString() : string.Empty;
                    completeDetails.optional_address_remark = dt.Rows[0]["optional_address_remark"].ToString().Trim() != string.Empty ? dt.Rows[0]["optional_address_remark"].ToString() : string.Empty;
                    completeDetails.optional_contact_remark = dt.Rows[0]["optional_contact_remark"].ToString().Trim() != string.Empty ? dt.Rows[0]["optional_contact_remark"].ToString() : string.Empty;
                    completeDetails.optional_relation_remark = dt.Rows[0]["optional_relation_remark"].ToString().Trim() != string.Empty ? dt.Rows[0]["optional_relation_remark"].ToString() : string.Empty;
                    #endregion

                    #endregion


                    completeDetails.pay_date_remark = dt.Rows[0]["pay_date_remark"].ToString().Trim() != string.Empty ? dt.Rows[0]["pay_date_remark"].ToString() : string.Empty;
                    completeDetails.personal_name_remark = dt.Rows[0]["personal_name_remark"].ToString().Trim() != string.Empty ? dt.Rows[0]["personal_name_remark"].ToString() : string.Empty;
                    completeDetails.personal_email_remark = dt.Rows[0]["personal_email_remark"].ToString().Trim() != string.Empty ? dt.Rows[0]["personal_email_remark"].ToString() : string.Empty;
                    completeDetails.personal_contact_no_remark = dt.Rows[0]["personal_contact_no_remark"].ToString().Trim() != string.Empty ? dt.Rows[0]["personal_contact_no_remark"].ToString() : string.Empty;
                    completeDetails.personal_perma_address_remark = dt.Rows[0]["personal_perma_address_remark"].ToString().Trim() != string.Empty ? dt.Rows[0]["personal_perma_address_remark"].ToString() : string.Empty;
                    completeDetails.personal_house_ph_remark = dt.Rows[0]["personal_house_ph_remark"].ToString().Trim() != string.Empty ? dt.Rows[0]["personal_house_ph_remark"].ToString() : string.Empty;
                    completeDetails.personal_birth_place_remark = dt.Rows[0]["personal_birth_place_remark"].ToString().Trim() != string.Empty ? dt.Rows[0]["personal_birth_place_remark"].ToString() : string.Empty;
                    completeDetails.personal_birth_date_remark = dt.Rows[0]["personal_birth_date_remark"].ToString().Trim() != string.Empty ? dt.Rows[0]["personal_birth_date_remark"].ToString() : string.Empty;
                    completeDetails.personal_civil_remark = dt.Rows[0]["personal_civil_remark"].ToString().Trim() != string.Empty ? dt.Rows[0]["personal_civil_remark"].ToString() : string.Empty;
                    completeDetails.personal_mother_maiden_name_remark = dt.Rows[0]["personal_mother_maiden_name_remark"].ToString().Trim() != string.Empty ? dt.Rows[0]["personal_mother_maiden_name_remark"].ToString() : string.Empty;
                    completeDetails.personal_mother_perma_add_remark = dt.Rows[0]["personal_mother_perma_add_remark"].ToString().Trim() != string.Empty ? dt.Rows[0]["personal_mother_perma_add_remark"].ToString() : string.Empty;
                    completeDetails.personal_company_name_remark = dt.Rows[0]["personal_company_name_remark"].ToString().Trim() != string.Empty ? dt.Rows[0]["personal_company_name_remark"].ToString() : string.Empty;
                    completeDetails.personal_company_address_remark = dt.Rows[0]["personal_company_address_remark"].ToString().Trim() != string.Empty ? dt.Rows[0]["personal_company_address_remark"].ToString() : string.Empty;
                    completeDetails.personal_job_title_remark = dt.Rows[0]["personal_job_title_remark"].ToString().Trim() != string.Empty ? dt.Rows[0]["personal_job_title_remark"].ToString() : string.Empty;
                    completeDetails.personal_monthly_income_remark = dt.Rows[0]["personal_monthly_income_remark"].ToString().Trim() != string.Empty ? dt.Rows[0]["personal_monthly_income_remark"].ToString() : string.Empty;
                    completeDetails.personal_reference_name_remark = dt.Rows[0]["personal_reference_name_remark"].ToString().Trim() != string.Empty ? dt.Rows[0]["personal_reference_name_remark"].ToString() : string.Empty;
                    completeDetails.personal_reference_contact_remark = dt.Rows[0]["personal_reference_contact_remark"].ToString().Trim() != string.Empty ? dt.Rows[0]["personal_reference_contact_remark"].ToString() : string.Empty;
                    completeDetails.personal_bank_name_remark = dt.Rows[0]["personal_bank_name_remark"].ToString().Trim() != string.Empty ? dt.Rows[0]["personal_bank_name_remark"].ToString() : string.Empty;
                    completeDetails.personal_bank_ac_no_remark = dt.Rows[0]["personal_bank_ac_no_remark"].ToString().Trim() != string.Empty ? dt.Rows[0]["personal_bank_ac_no_remark"].ToString() : string.Empty;
                    completeDetails.personal_req_loan_amt_remark = dt.Rows[0]["personal_req_loan_amt_remark"].ToString().Trim() != string.Empty ? dt.Rows[0]["personal_req_loan_amt_remark"].ToString() : string.Empty;
                    completeDetails.personal_req_loan_term_remark = dt.Rows[0]["personal_req_loan_term_remark"].ToString().Trim() != string.Empty ? dt.Rows[0]["personal_req_loan_term_remark"].ToString() : string.Empty;

                    completeDetails.AdditionalBirthPlace = dt.Rows[0]["addtional_birth_place"].ToString().Trim() != string.Empty ? dt.Rows[0]["addtional_birth_place"].ToString() : string.Empty;
                    completeDetails.AdditionalProvincialAddress = dt.Rows[0]["additonal_provincial_address"].ToString().Trim() != string.Empty ? dt.Rows[0]["additonal_provincial_address"].ToString() : string.Empty;
                    completeDetails.AdditionalLoanPurpose = Convert.ToString(dt.Rows[0]["purposeofloan"]) != string.Empty ? Convert.ToString(dt.Rows[0]["purposeofloan"]) : string.Empty;
                    completeDetails.AdditionalLoanPurpose_Remark = Convert.ToString(dt.Rows[0]["additional_loan_purpose"]) != string.Empty ? Convert.ToString(dt.Rows[0]["additional_loan_purpose"]) : string.Empty;
                    if (completeDetails.AdditionalLoanPurpose_Remark == string.Empty)
                        completeDetails.AdditionalLoanPurpose_Remark = Convert.ToString(dt.Rows[0]["purposeofloan"]) != string.Empty ? Convert.ToString(dt.Rows[0]["purposeofloan"]) : string.Empty;

                    completeDetails.AdditionalRequestedLoanAmount = dt.Rows[0]["additional_requested_loan_amount"].ToString().Trim() != string.Empty ? Convert.ToDouble(dt.Rows[0]["additional_requested_loan_amount"]) : 0.00;
                    completeDetails.AdditionalEmployedDuration = dt.Rows[0]["additional_employed_duration"].ToString().Trim() != string.Empty ? Convert.ToString(dt.Rows[0]["additional_employed_duration"]) : String.Empty;
                    completeDetails.AdditionalRequested_Term = AdditionalRequestTermStr;
                    completeDetails.AdditionalJobPosition = dt.Rows[0]["additional_job_position"].ToString().Trim() != string.Empty ? dt.Rows[0]["additional_job_position"].ToString() : string.Empty;
                    completeDetails.AdditionalJobLevel = dt.Rows[0]["additional_job_level"].ToString().Trim() != string.Empty ? dt.Rows[0]["additional_job_level"].ToString() : string.Empty;
                    completeDetails.AdditionalPayDate = dt.Rows[0]["additional_paydate"].ToString().Trim() != string.Empty ? Convert.ToString(dt.Rows[0]["additional_paydate"]) : string.Empty;

                    completeDetails.FamilyPersonalName = dt.Rows[0]["family_personal_name"].ToString().Trim() != string.Empty ? dt.Rows[0]["family_personal_name"].ToString() : string.Empty;
                    completeDetails.FamilyPersonalContact = dt.Rows[0]["family_personal_contact"].ToString().Trim() != string.Empty ? dt.Rows[0]["family_personal_contact"].ToString() : string.Empty;
                    completeDetails.FamilyRelationWithBorrower = dt.Rows[0]["family_relation_with_borrower"].ToString().Trim() != string.Empty ? dt.Rows[0]["family_relation_with_borrower"].ToString() : string.Empty;
                    completeDetails.FamilyBorrowerKnownDuration = dt.Rows[0]["family_borrower_known_duration"].ToString().Trim() != string.Empty ? dt.Rows[0]["family_borrower_known_duration"].ToString() : string.Empty;
                    completeDetails.FamilyAddressVerification = dt.Rows[0]["family_address_verification"].ToString().Trim() != string.Empty ? dt.Rows[0]["family_address_verification"].ToString() : string.Empty;
                    completeDetails.FamilyBorrowerWorkingPlace = dt.Rows[0]["family_borrower_working_place"].ToString().Trim() != string.Empty ? dt.Rows[0]["family_borrower_working_place"].ToString() : string.Empty;
                    completeDetails.FamilyPersonalNameStatus = dt.Rows[0]["family_personal_name_status"].ToString().Trim() != string.Empty ? Convert.ToBoolean(dt.Rows[0]["family_personal_name_status"]) : false;
                    completeDetails.FamilyPersonalContactStatus = dt.Rows[0]["family_personal_contact_status"].ToString().Trim() != string.Empty ? Convert.ToBoolean(dt.Rows[0]["family_personal_contact_status"]) : false;
                    completeDetails.FamilyRelationWithBorrowerStatus = dt.Rows[0]["family_relation_with_borrower_status"].ToString().Trim() != string.Empty ? Convert.ToBoolean(dt.Rows[0]["family_relation_with_borrower_status"]) : false;
                    completeDetails.FamilyBorrowerKnownDurationStatus = dt.Rows[0]["family_borrower_known_duration_status"].ToString().Trim() != string.Empty ? Convert.ToBoolean(dt.Rows[0]["family_borrower_known_duration_status"]) : false;
                    completeDetails.FamilyAddressVerificationStatus = dt.Rows[0]["family_address_verification_status"].ToString().Trim() != string.Empty ? Convert.ToBoolean(dt.Rows[0]["family_address_verification_status"]) : false;
                    completeDetails.FamilyBorrowerWorkingPlaceStatus = dt.Rows[0]["family_borrower_working_place_status"].ToString().Trim() != string.Empty ? Convert.ToBoolean(dt.Rows[0]["family_borrower_working_place_status"]) : false;


                    completeDetails.FriendPersonalName = dt.Rows[0]["friend_personal_name"].ToString().Trim() != string.Empty ? dt.Rows[0]["friend_personal_name"].ToString() : string.Empty;
                    completeDetails.FriendPersonalContact = dt.Rows[0]["friend_personal_contact"].ToString().Trim() != string.Empty ? dt.Rows[0]["friend_personal_contact"].ToString() : string.Empty;
                    completeDetails.FriendRelationWithBorrower = dt.Rows[0]["friend_relation_with_borrower"].ToString().Trim() != string.Empty ? dt.Rows[0]["friend_relation_with_borrower"].ToString() : string.Empty;
                    completeDetails.FriendBorrowerKnownDuration = dt.Rows[0]["friend_borrower_known_duration"].ToString().Trim() != string.Empty ? dt.Rows[0]["friend_borrower_known_duration"].ToString() : string.Empty;
                    completeDetails.FriendAddressVerification = dt.Rows[0]["friend_address_verification"].ToString().Trim() != string.Empty ? dt.Rows[0]["friend_address_verification"].ToString() : string.Empty;
                    completeDetails.FriendBorrowerWorkingPlace = dt.Rows[0]["friend_borrower_working"].ToString().Trim() != string.Empty ? dt.Rows[0]["friend_borrower_working"].ToString() : string.Empty;
                    completeDetails.FriendPersonalNameStatus = dt.Rows[0]["friend_personal_name_status"].ToString().Trim() != string.Empty ? Convert.ToBoolean(dt.Rows[0]["friend_personal_name_status"]) : false;
                    completeDetails.FriendPersonalContactStatus = dt.Rows[0]["friend_personal_contact_status"].ToString().Trim() != string.Empty ? Convert.ToBoolean(dt.Rows[0]["friend_personal_contact_status"]) : false;
                    completeDetails.FriendRelationWithBorrowerStatus = dt.Rows[0]["friend_relation_with_borrower_status"].ToString().Trim() != string.Empty ? Convert.ToBoolean(dt.Rows[0]["friend_relation_with_borrower_status"]) : false;
                    completeDetails.FriendBorrowerKnownDurationStatus = dt.Rows[0]["friend_borrower_known_duration_status"].ToString().Trim() != string.Empty ? Convert.ToBoolean(dt.Rows[0]["friend_borrower_known_duration_status"]) : false;
                    completeDetails.FriendAddressVerificationStatus = dt.Rows[0]["friend_address_verification_status"].ToString().Trim() != string.Empty ? Convert.ToBoolean(dt.Rows[0]["friend_address_verification_status"]) : false;
                    completeDetails.FriendBorrowerWorkingPlaceStatus = dt.Rows[0]["friend_borrower_working_place"].ToString().Trim() != string.Empty ? Convert.ToBoolean(dt.Rows[0]["friend_borrower_working_place"]) : false;

                    completeDetails.CoWorkerPersonalName = dt.Rows[0]["co_worker_personal_name"].ToString().Trim() != string.Empty ? dt.Rows[0]["co_worker_personal_name"].ToString() : string.Empty;
                    completeDetails.CoWorkerPersonalConatact = dt.Rows[0]["co_worker_personal_contact"].ToString().Trim() != string.Empty ? dt.Rows[0]["co_worker_personal_contact"].ToString() : string.Empty;
                    completeDetails.CoWorkerRelationWithBorrower = dt.Rows[0]["co_worker_relation_with_borrower"].ToString().Trim() != string.Empty ? dt.Rows[0]["co_worker_relation_with_borrower"].ToString() : string.Empty;
                    completeDetails.CoWorkerBorrowerKnownDuration = dt.Rows[0]["co_worker_borrower_known_duration"].ToString().Trim() != string.Empty ? dt.Rows[0]["co_worker_borrower_known_duration"].ToString() : string.Empty;
                    completeDetails.CoWorkerAddressVerification = dt.Rows[0]["co_worker_address_verification"].ToString().Trim() != string.Empty ? dt.Rows[0]["co_worker_address_verification"].ToString() : string.Empty;
                    completeDetails.CoWorkerBorrowerWorkingPlace = dt.Rows[0]["co_worker_borrower_working"].ToString().Trim() != string.Empty ? dt.Rows[0]["co_worker_borrower_working"].ToString() : string.Empty;
                    completeDetails.CoWorkerPersonalNameStatus = dt.Rows[0]["co_worker_personal_name_status"].ToString().Trim() != string.Empty ? Convert.ToBoolean(dt.Rows[0]["co_worker_personal_name_status"]) : false;
                    completeDetails.CoWorkerPersonalContactStatus = dt.Rows[0]["co_worker_personal_contact_status"].ToString().Trim() != string.Empty ? Convert.ToBoolean(dt.Rows[0]["co_worker_personal_contact_status"]) : false;
                    completeDetails.CoWorkerRelationWithBorrowerStatus = dt.Rows[0]["co_worker_relation_with_borrower_status"].ToString().Trim() != string.Empty ? Convert.ToBoolean(dt.Rows[0]["co_worker_relation_with_borrower_status"]) : false;
                    completeDetails.CoWorkerBorrowerKnownDurationStatus = dt.Rows[0]["co_worker_borrower_known_duration_status"].ToString().Trim() != string.Empty ? Convert.ToBoolean(dt.Rows[0]["co_worker_borrower_known_duration_status"]) : false;
                    completeDetails.CoWorkerAddressVerificationStatus = dt.Rows[0]["co_worker_address_verification_status"].ToString().Trim() != string.Empty ? Convert.ToBoolean(dt.Rows[0]["co_worker_address_verification_status"]) : false;
                    completeDetails.CoWorkerBorrowerWorkingPlaceStatus = dt.Rows[0]["co_worker_borrower_working_place"].ToString().Trim() != string.Empty ? Convert.ToBoolean(dt.Rows[0]["co_worker_borrower_working_place"]) : false;

                    completeDetails.EmploymentNameOfWorkContact = dt.Rows[0]["employement_nameof_work_contact"].ToString().Trim() != string.Empty ? dt.Rows[0]["employement_nameof_work_contact"].ToString() : string.Empty;
                    completeDetails.EmploymentNoOfWorkConatct = dt.Rows[0]["employement_no_of_work_contact"].ToString().Trim() != string.Empty ? dt.Rows[0]["employement_no_of_work_contact"].ToString() : string.Empty;
                    completeDetails.EmploymentBorrowerWorking = dt.Rows[0]["employement_borrower_working"].ToString().Trim() != string.Empty ? dt.Rows[0]["employement_borrower_working"].ToString() : string.Empty;
                    completeDetails.EmploymentBorrowerPosition = dt.Rows[0]["employement_borrower_position"].ToString().Trim() != string.Empty ? dt.Rows[0]["employement_borrower_position"].ToString() : string.Empty;
                    completeDetails.EmploymentBorrowerMonthlySalary = dt.Rows[0]["employement_borrower_monthly_salary"].ToString().Trim() != string.Empty ? Convert.ToString(dt.Rows[0]["employement_borrower_monthly_salary"]) : string.Empty;
                    completeDetails.EmploymentBorrowerAttendance = dt.Rows[0]["employement_borrower_attendance"].ToString().Trim() != string.Empty ? dt.Rows[0]["employement_borrower_attendance"].ToString() : string.Empty;
                    completeDetails.EmploymentBorrowerBankPayroll = dt.Rows[0]["employement_borrower_bank_payroll"].ToString().Trim() != string.Empty ? dt.Rows[0]["employement_borrower_bank_payroll"].ToString() : string.Empty;
                    completeDetails.EmploymentNameOfWorkContactStatus = dt.Rows[0]["employement_nameof_work_contact_status"].ToString().Trim() != string.Empty ? Convert.ToBoolean(dt.Rows[0]["employement_nameof_work_contact_status"]) : false;
                    completeDetails.EmploymentNoOfWorkConatctStatus = dt.Rows[0]["employement_no_of_work_contact_status"].ToString().Trim() != string.Empty ? Convert.ToBoolean(dt.Rows[0]["employement_no_of_work_contact_status"]) : false;
                    completeDetails.EmploymentBorrowerWorkingStatus = dt.Rows[0]["employement_borrower_working_status"].ToString().Trim() != string.Empty ? Convert.ToBoolean(dt.Rows[0]["employement_borrower_working_status"]) : false;
                    completeDetails.EmploymentBorrowerPositionStatus = dt.Rows[0]["employement_borrower_position_status"].ToString().Trim() != string.Empty ? Convert.ToBoolean(dt.Rows[0]["employement_borrower_position_status"]) : false;
                    completeDetails.EmploymentBorrowerMonthlySalaryStatus = dt.Rows[0]["employement_borrower_monthly_salary_status"].ToString().Trim() != string.Empty ? Convert.ToBoolean(dt.Rows[0]["employement_borrower_monthly_salary_status"]) : false;
                    completeDetails.EmploymentBorrowerAttendanceStatus = dt.Rows[0]["employement_borrower_attendance_status"].ToString().Trim() != string.Empty ? Convert.ToBoolean(dt.Rows[0]["employement_borrower_attendance_status"]) : false;
                    completeDetails.EmploymentBorrowerBankPayrollStatus = dt.Rows[0]["employement_borrower_bank_payroll_status"].ToString().Trim() != string.Empty ? Convert.ToBoolean(dt.Rows[0]["employement_borrower_bank_payroll_status"]) : false;

                    completeDetails.Question1_Status = dt.Rows[0]["question1_status"].ToString().Trim() != string.Empty ? Convert.ToBoolean(dt.Rows[0]["question1_status"]) : false;
                    completeDetails.Question2_Status = dt.Rows[0]["question2_status"].ToString().Trim() != string.Empty ? Convert.ToBoolean(dt.Rows[0]["question2_status"]) : false;
                    completeDetails.Question3_Status = dt.Rows[0]["question3_status"].ToString().Trim() != string.Empty ? Convert.ToBoolean(dt.Rows[0]["question3_status"]) : false;
                    completeDetails.Question4_Status = dt.Rows[0]["question4_status"].ToString().Trim() != string.Empty ? Convert.ToBoolean(dt.Rows[0]["question4_status"]) : false;
                    completeDetails.Question5_Status = dt.Rows[0]["question5_status"].ToString().Trim() != string.Empty ? Convert.ToBoolean(dt.Rows[0]["question5_status"]) : false;
                    completeDetails.Question6_Status = dt.Rows[0]["question6_status"].ToString().Trim() != string.Empty ? Convert.ToBoolean(dt.Rows[0]["question6_status"]) : false;
                    completeDetails.Question7_Status = dt.Rows[0]["question7_status"].ToString().Trim() != string.Empty ? Convert.ToBoolean(dt.Rows[0]["question7_status"]) : false;
                    completeDetails.Question8_Status = dt.Rows[0]["question8_status"].ToString().Trim() != string.Empty ? Convert.ToBoolean(dt.Rows[0]["question8_status"]) : false;
                    completeDetails.Question9_Status = dt.Rows[0]["question9_status"].ToString().Trim() != string.Empty ? Convert.ToBoolean(dt.Rows[0]["question9_status"]) : false;
                    completeDetails.Question10_Status = dt.Rows[0]["question10_status"].ToString().Trim() != string.Empty ? Convert.ToBoolean(dt.Rows[0]["question10_status"]) : false;

                }
                else
                {
                    dt = DbHelper.SelectMethod(string.Format(QueryHelper.GetApplicationdetails, intId));
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        if (Convert.ToBoolean(dt.Rows[0]["isverified"]))
                        {
                            TempData["msg"] = "Application Already Verified.";
                        }
                        else
                        {
                            TempData["msg"] = null;
                        }
                        completeDetails.notificationtoken = Convert.ToString(dt.Rows[0]["notificationtoken"]);
                        completeDetails.SSS_No = dt.Rows[0]["sss_no"].ToString().Trim() != string.Empty ? dt.Rows[0]["sss_no"].ToString() : string.Empty;
                        completeDetails.checker_name = dt.Rows[0]["userfullname"].ToString().Trim() != string.Empty ? dt.Rows[0]["userfullname"].ToString() : string.Empty;
                        string TermType = dt.Rows[0]["termtype"].ToString() != string.Empty ? Convert.ToString(dt.Rows[0]["termtype"]) : string.Empty;
                        if (!String.IsNullOrWhiteSpace(TermType))
                            TermType = GetTermType(TermType);
                        completeDetails.TermType = TermType;
                        string TermName = dt.Rows[0]["term"].ToString() != string.Empty ? Convert.ToString(dt.Rows[0]["term"]) : string.Empty;
                        completeDetails.TermName = dt.Rows[0]["term"].ToString() != string.Empty ? Convert.ToInt32(dt.Rows[0]["term"]) : -1;
                        if (!String.IsNullOrWhiteSpace(TermName))
                            TermName = GetTermName(TermName);

                        string AdditionalRequestTermStr = TermType + ", " + TermName;
                        var joiningDate = !String.IsNullOrWhiteSpace(Convert.ToString(dt.Rows[0]["date_joining"])) ? Convert.ToDateTime(dt.Rows[0]["date_joining"]) : DateTime.Now;
                        var currentDate = DateTime.Now;
                        var dateDiff = ((currentDate.Year - Convert.ToDateTime(joiningDate).Year) * 12) + currentDate.Month - Convert.ToDateTime(joiningDate).Month;
                        completeDetails.ApplicationNo = dt.Rows[0]["applicationno"].ToString() != string.Empty ? Convert.ToInt32(dt.Rows[0]["applicationno"].ToString()) : 0;
                        completeDetails.DateApplied = Convert.ToString(dt.Rows[0]["dateapplied"]);
                        var firstName = dt.Rows[0]["first_name"].ToString() != string.Empty ? dt.Rows[0]["first_name"].ToString() : string.Empty;
                        var middleName = dt.Rows[0]["middle_name"].ToString() != string.Empty ? dt.Rows[0]["middle_name"].ToString() : string.Empty;
                        var lastName = dt.Rows[0]["last_name"].ToString() != string.Empty ? dt.Rows[0]["last_name"].ToString() : string.Empty;
                        completeDetails.Name = firstName + " " + middleName + " " + lastName;
                        completeDetails.PersonalEmail = dt.Rows[0]["personalemail"].ToString() != string.Empty ? dt.Rows[0]["personalemail"].ToString() : string.Empty;
                        completeDetails.PersonalContactNo = dt.Rows[0]["personalcontactno"].ToString() != string.Empty ? dt.Rows[0]["personalcontactno"].ToString() : string.Empty;
                        completeDetails.RelativeContactNo = dt.Rows[0]["relativecontactno"].ToString() != string.Empty ? dt.Rows[0]["relativecontactno"].ToString() : string.Empty;
                        completeDetails.CoworkerContactNo = dt.Rows[0]["coworkercontactno"].ToString() != string.Empty ? dt.Rows[0]["coworkercontactno"].ToString() : string.Empty;

                        completeDetails.Address = dt.Rows[0]["address"].ToString() != string.Empty ? dt.Rows[0]["address"].ToString() : string.Empty;
                        completeDetails.HomePhone = dt.Rows[0]["homephoneno"].ToString() != string.Empty ? dt.Rows[0]["homephoneno"].ToString() : string.Empty;
                        completeDetails.BirthPlace = dt.Rows[0]["placeofbirth"].ToString() != string.Empty ? dt.Rows[0]["placeofbirth"].ToString() : string.Empty;
                        completeDetails.DOB = Convert.ToString(dt.Rows[0]["dateofbirth"]) != string.Empty ? Convert.ToString(dt.Rows[0]["dateofbirth"]) : string.Empty;
                        completeDetails.CivilStatus = dt.Rows[0]["civilstatus"].ToString() != string.Empty ? Convert.ToInt32(dt.Rows[0]["civilstatus"].ToString()) : 0;
                        completeDetails.CivilStatusText = approverCommonOperation.getCivilStatus(completeDetails.CivilStatus);
                        completeDetails.MotherMaidenName = dt.Rows[0]["mothermaidenname"].ToString() != string.Empty ? dt.Rows[0]["mothermaidenname"].ToString() : string.Empty;
                        completeDetails.MotherAddress = dt.Rows[0]["motheraddress"].ToString() != string.Empty ? dt.Rows[0]["motheraddress"].ToString() : string.Empty;
                        completeDetails.CompanyName = dt.Rows[0]["companyname"].ToString() != string.Empty ? dt.Rows[0]["companyname"].ToString() : string.Empty;
                        completeDetails.CompanyAddress = dt.Rows[0]["companyaddress"].ToString() != string.Empty ? dt.Rows[0]["companyaddress"].ToString() : string.Empty;
                        completeDetails.Designation = dt.Rows[0]["designation"].ToString() != string.Empty ? dt.Rows[0]["designation"].ToString() : string.Empty;
                        completeDetails.GrossIncome = dt.Rows[0]["gross_income"].ToString() != string.Empty ? Convert.ToDouble(dt.Rows[0]["gross_income"].ToString()) : 0;
                        completeDetails.RefName = dt.Rows[0]["reference_name"].ToString() != string.Empty ? dt.Rows[0]["reference_name"].ToString() : string.Empty;
                        completeDetails.RefContactNo = dt.Rows[0]["reference_contactno"].ToString() != string.Empty ? dt.Rows[0]["reference_contactno"].ToString() : string.Empty;
                        completeDetails.BankName = dt.Rows[0]["bankname"].ToString() != string.Empty ? dt.Rows[0]["bankname"].ToString() : string.Empty;
                        completeDetails.BankAccNo = dt.Rows[0]["bankaccountno"].ToString() != string.Empty ? dt.Rows[0]["bankaccountno"].ToString() : string.Empty;
                        completeDetails.Password = dt.Rows[0]["user_password"].ToString() != string.Empty ? dt.Rows[0]["user_password"].ToString() : string.Empty;
                        completeDetails.City = dt.Rows[0]["city"].ToString() != string.Empty ? Convert.ToInt32(dt.Rows[0]["city"].ToString()) : 0;
                        if (completeDetails.City != 0)
                        {
                            completeDetails.CityName = CommonMethods.GetCityName(completeDetails.City);
                        }
                        else
                        { completeDetails.CityName = ""; }
                        completeDetails.userid = Convert.ToInt32(dt.Rows[0]["user_id"]); ;
                        completeDetails.Ischeck = Convert.ToBoolean(dt.Rows[0]["ischeck"].ToString());
                        completeDetails.Isverified = Convert.ToBoolean(dt.Rows[0]["isverified"].ToString());
                        completeDetails.Isrecheck = Convert.ToBoolean(dt.Rows[0]["isrecheck"].ToString());
                        completeDetails.Isreverified = Convert.ToBoolean(dt.Rows[0]["isreverified"].ToString());
                        completeDetails.Isapproved = Convert.ToBoolean(dt.Rows[0]["isapproved"].ToString());
                        completeDetails.FamilyPersonalName = dt.Rows[0]["relativename"].ToString() != string.Empty ? dt.Rows[0]["relativename"].ToString() : string.Empty;
                        completeDetails.CoWorkerPersonalName = dt.Rows[0]["coworkername"].ToString() != string.Empty ? dt.Rows[0]["coworkername"].ToString() : string.Empty;
                        completeDetails.FamilyRelationWithBorrower = dt.Rows[0]["relationwithrelative"] != null ? dt.Rows[0]["relationwithrelative"].ToString() : string.Empty;
                        completeDetails.FriendPersonalName = dt.Rows[0]["friendname"].ToString() != string.Empty ? dt.Rows[0]["friendname"].ToString() : string.Empty;
                        completeDetails.Suffix = dt.Rows[0]["suffix"].ToString() != string.Empty ? dt.Rows[0]["suffix"].ToString() : string.Empty;
                        completeDetails.Street = dt.Rows[0]["street"].ToString() != string.Empty ? dt.Rows[0]["street"].ToString() : string.Empty;
                        completeDetails.Barangay = dt.Rows[0]["barangay"].ToString() != string.Empty ? dt.Rows[0]["barangay"].ToString() : string.Empty;
                        completeDetails.ZipCode = dt.Rows[0]["zipcode"].ToString() != string.Empty ? dt.Rows[0]["zipcode"].ToString() : string.Empty;
                        completeDetails.Company_Phoneno = dt.Rows[0]["company_phoneno"].ToString() != string.Empty ? dt.Rows[0]["company_phoneno"].ToString() : string.Empty;
                        completeDetails.FriendPersonalContact = dt.Rows[0]["friend_contactno"].ToString() != string.Empty ? dt.Rows[0]["friend_contactno"].ToString() : string.Empty;
                        completeDetails.AdditionalPayDate = dt.Rows[0]["pay_date"].ToString() != string.Empty ? Convert.ToDateTime(dt.Rows[0]["pay_date"]).ToShortDateString() : string.Empty;
                        completeDetails.AdditionalRequested_Term = AdditionalRequestTermStr;
                        completeDetails.Loan_Amount = dt.Rows[0]["loanamount"].ToString() != string.Empty ? Convert.ToDouble(dt.Rows[0]["loanamount"].ToString()) : 0.00;
                        completeDetails.AdditionalRequestedLoanAmount = dt.Rows[0]["loanamount"].ToString() != string.Empty ? Convert.ToDouble(dt.Rows[0]["loanamount"].ToString()) : 0.00;
                        completeDetails.AdditionalEmployedDuration = Convert.ToString(dateDiff);
                        completeDetails.AdditionalJobLevel = Convert.ToString(dt.Rows[0]["designation"]) != string.Empty ? Convert.ToString(dt.Rows[0]["designation"]) : string.Empty;
                        completeDetails.AdditionalJobPosition = Convert.ToString(dt.Rows[0]["designation"]) != string.Empty ? Convert.ToString(dt.Rows[0]["designation"]) : string.Empty;
                        completeDetails.AdditionalLoanPurpose = Convert.ToString(dt.Rows[0]["purposeofloan"]) != string.Empty ? Convert.ToString(dt.Rows[0]["purposeofloan"]) : string.Empty;
                        completeDetails.payDate1 = Convert.ToString(dt.Rows[0]["paydate1"]) != string.Empty ? Convert.ToString(dt.Rows[0]["paydate1"]) : string.Empty;
                        completeDetails.payDate2 = Convert.ToString(dt.Rows[0]["paydate2"]) != string.Empty ? Convert.ToString(dt.Rows[0]["paydate2"]) : string.Empty;
                        completeDetails.GovUrl = Convert.ToString(dt.Rows[0]["gov_id_url"]);
                        completeDetails.CompUrl = Convert.ToString(dt.Rows[0]["companyid_url"]);
                        completeDetails.BillUrl = Convert.ToString(dt.Rows[0]["billing_url"]);
                        completeDetails.IncomeUrl = Convert.ToString(dt.Rows[0]["income_url"]);
                        completeDetails.OthUrl = Convert.ToString(dt.Rows[0]["other_url"]);
                        completeDetails.ATMUrl = Convert.ToString(dt.Rows[0]["atm_url"]);
                        completeDetails.EmploymentNameOfWorkContact = dt.Rows[0]["companyname"].ToString().Trim() != string.Empty ? dt.Rows[0]["companyname"].ToString() : string.Empty;
                        completeDetails.EmploymentNoOfWorkConatct = Convert.ToString(dt.Rows[0]["company_phoneno"]) != string.Empty ? Convert.ToString(dt.Rows[0]["company_phoneno"]) : string.Empty;
                        completeDetails.term_value = Convert.ToString(dt.Rows[0]["term_value"]) == "" ? 0 : Convert.ToInt32(dt.Rows[0]["term_value"]);
                        completeDetails.interest_rate = Convert.ToString(dt.Rows[0]["interest_rate"]) == "" ? 0 : Convert.ToDecimal(dt.Rows[0]["interest_rate"]);
                        completeDetails.late_rate = Convert.ToString(dt.Rows[0]["late_rate"]) == "" ? 0 : Convert.ToDecimal(dt.Rows[0]["late_rate"]);
                        completeDetails.lateFee = TermType.ToLower() == "weekly" ? Convert.ToInt32(ConfigurationManager.AppSettings["WeeklyFee"]) : TermType.ToLower() == "bi-weekly" ? Convert.ToInt32(ConfigurationManager.AppSettings["BiWeeklyFee"]) : TermType.ToLower() == "monthly" ? Convert.ToInt32(ConfigurationManager.AppSettings["MonthlyFee"]) : 0;
                        completeDetails.ApprovedLoanAmount = dt.Rows[0]["approved_loan_amount"].ToString() != string.Empty ? Convert.ToDouble(dt.Rows[0]["approved_loan_amount"]) : 0.00;
                        completeDetails.ApprovedTerm = dt.Rows[0]["approved_term"].ToString() != string.Empty ? Convert.ToDouble(dt.Rows[0]["approved_term"]) : 0.00;
                        completeDetails.ApprovedInterestRate = dt.Rows[0]["approved_interest_rate"].ToString() != string.Empty ? Convert.ToDouble(dt.Rows[0]["approved_interest_rate"]) : 0.00;
                        completeDetails.ApprovedMaturityDate = Convert.ToString(dt.Rows[0]["approvedmaturitydate"]) != string.Empty ? Convert.ToString(dt.Rows[0]["approvedmaturitydate"]) : string.Empty;
                        completeDetails.ApprovedTotalDueAmount = dt.Rows[0]["approved_total_amount_due"].ToString() != string.Empty ? Convert.ToDouble(dt.Rows[0]["approved_total_amount_due"]) : 0.00;

                    }
                }
                NotificationTemplateVM model = new NotificationTemplateVM();
                DataTable dtsms = DbHelper.SelectMethod(QueryHelper.GetActiveNotificationTemplateList);
                if (dtsms != null && dtsms.Rows.Count > 0)
                {
                    for (int i = 0; i < dtsms.Rows.Count; i++)
                    {
                        model.NotificationTemplateList.Add(new NotificationTemplateVM
                        {
                            id = Convert.ToInt32(dtsms.Rows[i]["id"]),
                            NotificationTemplateName = Convert.ToString(dtsms.Rows[i]["template_name"]),

                        });
                    }
                }

                if (model.NotificationTemplateList != null && model.NotificationTemplateList.Count > 0)
                {
                    model.NotificationTemplateList.Add(new NotificationTemplate() { NotificationTemplateName = "--Select--", id = 0, NotificationTemplateDesc = "", NotificationTemplateDefinition = "" });
                    ViewBag.NotificationTemplateList = model.NotificationTemplateList.OrderBy(x => x.id).ToList();
                }
                else
                {
                    ViewBag.NotificationTemplateList = new List<NotificationTemplate>();
                }
            }
            catch (Exception ex)
            {
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
            }
            return View(completeDetails);
        }

        private string GetTermType(string termType)
        {
            string str = string.Empty;
            try
            {
                DataTable dt = DbHelper.SelectMethod(string.Format(QueryHelper.GetTermTypeId, termType));
                if (dt != null && dt.Rows.Count > 0)
                {
                    str = Convert.ToString(dt.Rows[0]["termtype"]);
                }
            }
            catch (Exception ex)
            {
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                throw ex;
            }

            return str;
        }

        private string GetTermName(string term)
        {
            string str = string.Empty;
            try
            {
                DataTable dt = DbHelper.SelectMethod(string.Format(QueryHelper.GetTermTypebyId, term));
                if (dt != null && dt.Rows.Count > 0)
                {
                    str = Convert.ToString(dt.Rows[0]["term_name"]);
                }
            }
            catch (Exception ex)
            {
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                throw ex;
            }

            return str;

        }

        public bool DeclineClick(string id)
        {
            ActivityLog.Info($"Application {id} Declined on Verifer Window by User {Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value)}", $"{this.GetType().Name} ==> {System.Reflection.MethodBase.GetCurrentMethod().Name}");
            var result = false;
            int intAppId = 0;
            var currentDate = DateTime.Now;
            if (!string.IsNullOrEmpty(id))
            {
                intAppId = Convert.ToInt32(id);
            }
            try
            {
                var _Preterm_Reloan = DbHelper.SelectMethod(String.Format(QueryHelper.SelectPreTermReloanRecord, id));
                if (_Preterm_Reloan != null && _Preterm_Reloan.Rows.Count > 0)
                {
                    if (Convert.ToBoolean(_Preterm_Reloan.Rows[0]["isreloan"]))
                    {
                        DbHelper.InsertUpdateDelete(String.Format(QueryHelper.CancelReloan, Convert.ToInt32(_Preterm_Reloan.Rows[0]["oldappno"])));
                    }
                    else
                    {
                        DbHelper.InsertUpdateDelete(string.Format(QueryHelper.CancelIsPretermGenerated, Convert.ToInt32(_Preterm_Reloan.Rows[0]["oldappno"])));
                    }
                    DbHelper.InsertUpdateDelete(String.Format(QueryHelper.CancelPreTerms, Convert.ToInt32(_Preterm_Reloan.Rows[0]["newappno"])));
                    DbHelper.InsertUpdateDelete(String.Format(QueryHelper.DeletePretermReloanRecord, Convert.ToInt32(_Preterm_Reloan.Rows[0]["newappno"])));
                }
                else
                {
                    var UserLoginType = DbHelper.SelectMethod($"SELECT ctr.rolename FROM public.tblapplication_record tar join public.ct_user ctu on tar.checked_by=ctu.userid join ct_roles ctr on ctu.webadminrole=ctr.id and tar.applicationno={intAppId};");
                    if (UserLoginType != null && UserLoginType.Rows.Count > 0)
                    {
                        if (Convert.ToString(UserLoginType.Rows[0]["rolename"]) == "Adminuser")
                        {
                            var query = DbHelper.InsertUpdateDelete(string.Format(QueryHelper.UpdateReCheckAdmin, currentDate, intAppId));
                            if (query > 0)
                            {
                                result = true;
                            }
                        }
                        else
                        {
                            var query = DbHelper.InsertUpdateDelete(string.Format(QueryHelper.UpdateReCheck, currentDate, intAppId));
                            if (query > 0)
                            {
                                result = true;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                throw ex;
            }
            return result;
        }

        /// <summary>
        /// Transfer To Approver or Upload Files
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult VerificationWindow(CompleteAppVerificationDetails model, string CommandName)
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.Verifier))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            ActivityLog.Info($"Application {model.ApplicationNo} Posted as {CommandName} with Amount {model.ApprovedLoanAmount} on Verifer Window by User {Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value)}", $"{this.GetType().Name} ==> {System.Reflection.MethodBase.GetCurrentMethod().Name}");
            string ActionName = "GetLoanRequestForVerification";
            string GovtFileName = string.Empty;
            string CompanyFileName = string.Empty;
            string BillFileName = string.Empty;
            string IncomeFileName = string.Empty;
            string OtherFileName = string.Empty;
            string ATMFileName = string.Empty;
            int appNo = Convert.ToInt32(model.ApplicationNo);
            int user_id = Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value);
            CommonOperation comnoperation = new CommonOperation();
            var appinformation = comnoperation.GetListbyID(appNo, ActionName, user_id);
            if (Session["ActionName"] != null)
            {
                ActionName = Convert.ToString(Session["ActionName"]);
            }
            try
            {
                if (CommandName == "Save Upload Files")
                {
                    var PhoneNumber = appinformation.personalcontactno;
                    var AgentName = appinformation.name;
                    var GovtFileIdUrl = appinformation.gov_id_url;
                    var CompanyFileIdUrl = appinformation.companyid_url;
                    var BillFileIdUrl = appinformation.billing_url;
                    var IncomeFileIdUrl = appinformation.income_url;
                    var OtherFileIdUrl = appinformation.other_url;
                    var ATMFileIdURL = appinformation.atm_url;

                    var _GovtFileIdUrl = Upload(model.GovIdFile, appNo, GovtFileIdUrl);
                    var _CompanyFileIdUrl = Upload(model.CompanyIdFile, appNo, CompanyFileIdUrl);
                    var _BillFileIdUrl = Upload(model.BillingIdFile, appNo, BillFileIdUrl);
                    var _IncomeFileIdUrl = Upload(model.IncomeIdFile, appNo, IncomeFileIdUrl);
                    var _OtherFileIdUrl = Upload(model.OtherIdFile, appNo, OtherFileIdUrl);
                    var _ATMFileIdURL = Upload(model.AtmIdFile, appNo, ATMFileIdURL);

                    GovtFileName = _GovtFileIdUrl == "" ? Convert.ToString(GovtFileIdUrl) : _GovtFileIdUrl;
                    CompanyFileName = _CompanyFileIdUrl == "" ? Convert.ToString(CompanyFileIdUrl) : _CompanyFileIdUrl;
                    BillFileName = _BillFileIdUrl == "" ? Convert.ToString(BillFileIdUrl) : _BillFileIdUrl;
                    IncomeFileName = _IncomeFileIdUrl == "" ? Convert.ToString(IncomeFileIdUrl) : _IncomeFileIdUrl;
                    OtherFileName = _OtherFileIdUrl == "" ? Convert.ToString(OtherFileIdUrl) : _OtherFileIdUrl;
                    ATMFileName = _ATMFileIdURL == "" ? Convert.ToString(ATMFileIdURL) : _ATMFileIdURL;

                    comnoperation.UpdateFiles(appNo, GovtFileName, CompanyFileName, BillFileName, IncomeFileName, OtherFileName, ATMFileName);
                    return Redirect("VerificationWindow?Id=" + appNo + "&phoneNumber=" + PhoneNumber + "&ActionName=Index");
                }
                else if (String.IsNullOrWhiteSpace(model.BankName) || String.IsNullOrEmpty(model.BankAccNo) || String.IsNullOrWhiteSpace(model.AdditionalLoanPurpose))
                {
                    var PhoneNumber = appinformation.personalcontactno;
                    TempData["msg"] = string.Format($"Bank Name and Bank Account and Purpose of Loan Mandatory.");
                    return Redirect("VerificationWindow?Id=" + appNo + "&phoneNumber=" + PhoneNumber + "&ActionName=Index");
                }
                else
                {
                    logger.AddWelcomeMessage(History.Application, "", "Your loan is pre-approved. Go to MY LOAN to sign your contract and wait for the system approval.", model.ApprovedLoanAmount, true, appNo);
                    ActivityLog.Info($"Application {model.ApplicationNo} Move to Approver window from Verifer Window by User {Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value)}", $"{this.GetType().Name} ==> {System.Reflection.MethodBase.GetCurrentMethod().Name}");
                    #region PersonalVerificationDetails

                    #region NewField Changes-Phase3
                    var nearest_landmark = model.nearest_landmark == null ? "" : model.nearest_landmark.Replace("'", "''");
                    var sss_no = model.SSS_No == null ? "" : model.SSS_No.Replace("'", "''");
                    var PayDate = model.payDate1 == null ? "" : model.payDate1.Replace("'", "''");
                    var province = model.Province == null ? "" : model.Province.Replace("'", "''");
                    var home_status = model.home_status;
                    var Permanent_address = model.Permanent_address == null ? "" : model.Permanent_address.Replace("'", "''");
                    var tansfer_residence = model.tansfer_residence == null ? "" : model.tansfer_residence.Replace("'", "''");
                    var spouse_name = model.spouse_name == null ? "" : model.spouse_name.Replace("'", "''");
                    var spouse_occupation = model.spouse_occupation == null ? "" : model.spouse_occupation.Replace("'", "''");
                    var number_of_dependent = model.number_of_dependent;
                    var mother_work = model.mother_work == null ? "" : model.mother_work.Replace("'", "''");
                    var father_name = model.father_name == null ? "" : model.father_name.Replace("'", "''");
                    var father_work = model.father_work == null ? "" : model.father_work.Replace("'", "''");
                    var living_with_mother = model.living_with_mother == null ? "" : model.living_with_mother.Replace("'", "''");
                    var sibling_count = model.sibling_count;
                    var sibling_works = model.sibling_works == null ? "" : model.sibling_works.Replace("'", "''");
                    var occupation = String.IsNullOrWhiteSpace(model.occupation) == true ? "0" : model.occupation.Replace("'", "''");
                    var net_income = model.net_income;
                    var pending_resignation = model.pending_resignation == null ? "" : model.pending_resignation.Replace("'", "''");
                    var other_source_of_income = model.other_source_of_income == null ? "" : model.other_source_of_income.Replace("'", "''");
                    var know_about_cashmart = model.know_about_cashmart == null ? "" : model.know_about_cashmart.Replace("'", "''");
                    var pending_loan_fron_otherland = model.pending_loan_fron_otherland == null ? "" : model.pending_loan_fron_otherland.Replace("'", "''");
                    var bank_loan_or_credit_card = model.bank_loan_or_credit_card == null ? "" : model.bank_loan_or_credit_card.Replace("'", "''");
                    var BankId = model.BankId;
                    var family_name1 = model.family_name1 == null ? "" : model.family_name1.Replace("'", "''");
                    var family_address1 = model.family_address1 == null ? "" : model.family_address1.Replace("'", "''");
                    var family_contact1 = model.family_contact1 == null ? "" : model.family_contact1.Replace("'", "''");
                    var family_relation1 = model.family_relation1 == null ? "" : model.family_relation1.Replace("'", "''");
                    var family_name2 = model.family_name2 == null ? "" : model.family_name2.Replace("'", "''");
                    var family_address2 = model.family_address2 == null ? "" : model.family_address2.Replace("'", "''");
                    var family_contact2 = model.family_contact2 == null ? "" : model.family_contact2.Replace("'", "''");
                    var family_relation2 = model.family_relation2 == null ? "" : model.family_relation2.Replace("'", "''");
                    var optional_name = model.optional_name == null ? "" : model.optional_name.Replace("'", "''");
                    var optional_address = model.optional_address == null ? "" : model.optional_address.Replace("'", "''");
                    var optional_contact = model.optional_contact == null ? "" : model.optional_contact.Replace("'", "''");
                    var optional_relation = model.optional_relation == null ? "" : model.optional_relation.Replace("'", "''");

                    ///============Remarks Fields here============

                    var nearest_landmark_remark = Convert.ToString(model.nearest_landmark_remark) == null ? "" : model.nearest_landmark_remark.Replace("'", "''");
                    var SSS_No_remark = Convert.ToString(model.SSS_No_remark) == null ? "" : model.SSS_No_remark.Replace("'", "''");
                    var rented_mortgage_owned_remark = Convert.ToString(model.rented_mortgage_owned_remark) == null ? "" : model.rented_mortgage_owned_remark.Replace("'", "''");
                    var Permanent_address_remark = Convert.ToString(model.Permanent_address_remark) == null ? "" : model.Permanent_address_remark.Replace("'", "''");
                    var tansfer_residence_remark = Convert.ToString(model.tansfer_residence_remark) == null ? "" : model.tansfer_residence_remark.Replace("'", "''");
                    var spouse_name_remark = Convert.ToString(model.spouse_name_remark) == null ? "" : model.spouse_name_remark.Replace("'", "''");
                    var spouse_occupation_remark = Convert.ToString(model.spouse_occupation_remark) == null ? "" : model.spouse_occupation_remark.Replace("'", "''");
                    var number_of_dependent_remark = Convert.ToString(model.number_of_dependent_remark) == null ? "" : model.number_of_dependent_remark.Replace("'", "''");
                    var father_name_work_remark = Convert.ToString(model.father_name_work_remark) == null ? "" : model.father_name_work_remark.Replace("'", "''");
                    var sibling_works_remark = Convert.ToString(model.sibling_works_remark) == null ? "" : model.sibling_works_remark.Replace("'", "''");
                    var occupation_remark = Convert.ToString(model.occupation_remark) == null ? "" : model.occupation_remark.Replace("'", "''");
                    var net_income_remark = Convert.ToString(model.net_income_remark) == null ? "" : model.net_income_remark.Replace("'", "''");
                    var scheduled_and_btc_remark = Convert.ToString(model.scheduled_and_btc_remark) == null ? "" : model.scheduled_and_btc_remark.Replace("'", "''");
                    var pay_date_remark = Convert.ToString(model.pay_date_remark) == null ? "" : model.pay_date_remark.Replace("'", "''");
                    var pending_resignation_remark = Convert.ToString(model.pending_resignation_remark) == null ? "" : model.pending_resignation_remark.Replace("'", "''");
                    var other_source_of_income_remark = Convert.ToString(model.other_source_of_income_remark) == null ? "" : model.other_source_of_income_remark.Replace("'", "''");
                    var know_about_cashmart_remark = Convert.ToString(model.know_about_cashmart_remark) == null ? "" : model.know_about_cashmart_remark.Replace("'", "''");
                    var pending_loan_fron_otherland_remark = Convert.ToString(model.pending_loan_fron_otherland_remark) == null ? "" : model.pending_loan_fron_otherland_remark.Replace("'", "''");
                    var bank_loan_or_credit_card_remark = Convert.ToString(model.bank_loan_or_credit_card_remark) == null ? "" : model.bank_loan_or_credit_card_remark.Replace("'", "''");
                    var family_name1_remark = Convert.ToString(model.family_name1_remark) == null ? "" : model.family_name1_remark.Replace("'", "''");
                    var family_address1_remark = Convert.ToString(model.family_address1_remark) == null ? "" : model.family_address1_remark.Replace("'", "''");
                    var family_contact1_remark = Convert.ToString(model.family_contact1_remark) == null ? "" : model.family_contact1_remark.Replace("'", "''");
                    var family_relation1_remark = Convert.ToString(model.family_relation1_remark) == null ? "" : model.family_relation1_remark.Replace("'", "''");
                    var family_name2_remark = Convert.ToString(model.family_name2_remark) == null ? "" : model.family_name2_remark.Replace("'", "''");
                    var family_address2_remark = Convert.ToString(model.family_address2_remark) == null ? "" : model.family_address2_remark.Replace("'", "''");
                    var family_contact2_remark = Convert.ToString(model.family_contact2_remark) == null ? "" : model.family_contact2_remark.Replace("'", "''");
                    var family_relation2_remark = Convert.ToString(model.family_relation2_remark) == null ? "" : model.family_relation2_remark.Replace("'", "''");
                    var optional_name_remark = Convert.ToString(model.optional_name_remark) == null ? "" : model.optional_name_remark.Replace("'", "''");
                    var optional_address_remark = Convert.ToString(model.optional_address_remark) == null ? "" : model.optional_address_remark.Replace("'", "''");
                    var optional_contact_remark = Convert.ToString(model.optional_contact_remark) == null ? "" : model.optional_contact_remark.Replace("'", "''");
                    var optional_relation_remark = Convert.ToString(model.optional_relation_remark) == null ? "" : model.optional_relation_remark.Replace("'", "''");




                    #endregion


                    var requestedAmt = model.RequestedAmt;
                    var requestedTerms = model.RequestedTerms;
                    var id1Status = model.Id1_Status;
                    var id2Status = model.Id2_Status;
                    var piStatus = model.Pi_Status;
                    var pbStatus = model.Pb_Status;
                    var remarks = model.Remarks == null ? "" : model.Remarks.Replace("'", "''");
                    var addBirthPlace = model.AdditionalBirthPlace == null ? "" : model.AdditionalBirthPlace.Replace("'", "''");
                    var addProvincialAdd = model.AdditionalProvincialAddress == null ? "" : model.AdditionalProvincialAddress.Replace("'", "''");
                    var addLoanPurpose = model.AdditionalLoanPurpose == null ? "" : model.AdditionalLoanPurpose.Replace("'", "''");
                    var AdditionalLoanPurpose_Remark = model.AdditionalLoanPurpose_Remark == null ? "" : model.AdditionalLoanPurpose_Remark.Replace("'", "''");
                    var addRequestedLoanAmount = model.AdditionalRequestedLoanAmount;
                    var addrequestedTerm = model.AdditionalRequestedTerm;
                    var addEmpDuration = model.AdditionalEmployedDuration == null ? "" : model.AdditionalEmployedDuration.Replace("'", "''");
                    if (addEmpDuration == "" || addEmpDuration == null)
                    {
                        addEmpDuration = "0";
                    }
                    var addJobPosition = model.AdditionalJobPosition == null ? "" : model.AdditionalJobPosition.Replace("'", "''");
                    var addJobLevel = model.AdditionalJobLevel == null ? "" : model.AdditionalJobLevel.Replace("'", "''");
                    var addPayDate = model.AdditionalPayDate == null ? "" : model.AdditionalPayDate.Replace("'", "''");
                    var familyPersonalNameStatus = model.FamilyPersonalNameStatus;
                    var familyPersonalContactStatus = model.FamilyPersonalContactStatus;
                    var familyRelationWithBorrowerStatus = model.FamilyRelationWithBorrowerStatus;
                    var familyBorrowerKnownDurationStatus = model.FamilyBorrowerKnownDurationStatus;
                    var familyAddVerificationStatus = model.FamilyAddressVerificationStatus;
                    var familyBorrowerWorkingPlaceStatus = model.FamilyBorrowerWorkingPlaceStatus;
                    var friendPersonalNameStatus = model.FriendPersonalNameStatus;
                    var friendPersonalContactStatus = model.FriendPersonalContactStatus;
                    var friendRelationWithBorrowerStatus = model.FriendRelationWithBorrowerStatus;
                    var friendBorrowerKnownDurationStatus = model.FriendBorrowerKnownDurationStatus;
                    var friendAddVerificationStatus = model.FriendAddressVerificationStatus;
                    var friendBorrowerWorkingPlaceStatus = model.FriendBorrowerWorkingPlaceStatus;
                    var coworkerPersonalNameStatus = model.CoWorkerPersonalNameStatus;
                    var coworkerPersonalContactStatus = model.CoWorkerPersonalContactStatus;
                    var coworkerRelationWithBorrowerStatus = model.CoWorkerRelationWithBorrowerStatus;
                    var coworkerBorrowerKnownDurationStatus = model.CoWorkerBorrowerKnownDurationStatus;
                    var coworkerAddVerificationStatus = model.CoWorkerAddressVerificationStatus;
                    var coworkerBorrowerWorkingPlaceStatus = model.CoWorkerBorrowerWorkingPlaceStatus;
                    var employmentNameOfWorkContactStatus = model.EmploymentNameOfWorkContactStatus;
                    var employmentNoOfWorkContactStatus = model.EmploymentNoOfWorkConatctStatus;
                    var employmentBorrowerWorkingStatus = model.EmploymentBorrowerWorkingStatus;
                    var employmentBorrowerPositionStatus = model.EmploymentBorrowerPositionStatus;
                    var employmentBorrowerMonthlySalaryStatus = model.EmploymentBorrowerMonthlySalaryStatus;
                    var employmentBorrowerAttendanceStatus = model.EmploymentBorrowerAttendanceStatus;
                    var employmentBorrowerBankPayrollStatus = model.EmploymentBorrowerBankPayrollStatus;
                    var familyPersonalName = model.FamilyPersonalName == null ? "" : model.FamilyPersonalName.Replace("'", "''");
                    var familyPersonalContact = model.FamilyPersonalContact == null ? "" : model.FamilyPersonalContact.Replace("'", "''");
                    var familyRelationWithBorrower = model.FamilyRelationWithBorrower == null ? "" : model.FamilyRelationWithBorrower.Replace("'", "''");
                    var familyBorrowerKnownDuration = model.FamilyBorrowerKnownDuration == null ? "" : model.FamilyBorrowerKnownDuration.Replace("'", "''");
                    var familyAddVerification = model.FamilyAddressVerification == null ? "" : model.FamilyAddressVerification.Replace("'", "''");
                    var familyBorrowerWorkingPlace = model.FamilyBorrowerWorkingPlace == null ? "" : model.FamilyBorrowerWorkingPlace.Replace("'", "''");
                    var friendPersonalName = model.FriendPersonalName == null ? "" : model.FriendPersonalName.Replace("'", "''");
                    var friendPersonalContact = model.FriendPersonalContact == null ? "" : model.FriendPersonalContact.Replace("'", "''");
                    var friendRelationWithBorrower = model.FriendRelationWithBorrower == null ? "" : model.FriendRelationWithBorrower.Replace("'", "''");
                    var friendBorrowerKnownDuration = model.FriendBorrowerKnownDuration == null ? "" : model.FriendBorrowerKnownDuration.Replace("'", "''");
                    var friendAddVerification = model.FriendAddressVerification == null ? "" : model.FriendAddressVerification.Replace("'", "''");
                    var friendBorrowerWorkingPlace = model.FriendBorrowerWorkingPlace == null ? "" : model.FriendBorrowerWorkingPlace.Replace("'", "''");
                    var coworkerPersonalName = model.CoWorkerPersonalName == null ? "" : model.CoWorkerPersonalName.Replace("'", "''");
                    var coworkerPersonalContact = model.CoWorkerPersonalConatact == null ? "" : model.CoWorkerPersonalConatact.Replace("'", "''");
                    var coworkerRelationWithBorrower = model.CoWorkerRelationWithBorrower == null ? "" : model.CoWorkerRelationWithBorrower.Replace("'", "''");
                    var coworkerBorrowerKnownDuration = model.CoWorkerBorrowerKnownDuration == null ? "" : model.CoWorkerBorrowerKnownDuration.Replace("'", "''");
                    var coworkerAddVerification = model.CoWorkerAddressVerification == null ? "" : model.CoWorkerAddressVerification.Replace("'", "''");
                    var coworkerBorrowerWorkingPlace = model.CoWorkerBorrowerWorkingPlace == null ? "" : model.CoWorkerBorrowerWorkingPlace.Replace("'", "''");
                    var employmentNameOfWorkContact = model.EmploymentNameOfWorkContact == null ? "" : model.EmploymentNameOfWorkContact.Replace("'", "''");
                    var employmentNoOfWorkContact = model.EmploymentNoOfWorkConatct == null ? "" : model.EmploymentNoOfWorkConatct.Replace("'", "''");
                    var employmentBorrowerWorking = model.EmploymentBorrowerWorking == null ? "" : model.EmploymentBorrowerWorking.Replace("'", "''");
                    var employmentBorrowerPosition = model.EmploymentBorrowerPosition == null ? "" : model.EmploymentBorrowerPosition.Replace("'", "''");
                    var employmentBorrowerMonthlySalary = model.EmploymentBorrowerMonthlySalary == null ? "" : model.EmploymentBorrowerMonthlySalary.Replace("'", "''");
                    var employmentBorrowerAttendance = model.EmploymentBorrowerAttendance == null ? "" : model.EmploymentBorrowerAttendance.Replace("'", "''");
                    var employmentBorrowerBankPayroll = model.EmploymentBorrowerBankPayroll == null ? "" : model.EmploymentBorrowerBankPayroll.Replace("'", "''");
                    var question1Status = model.Question1_Status;
                    var question2Status = model.Question2_Status;
                    var question3Status = model.Question3_Status;
                    var question4Status = model.Question4_Status;
                    var question5Status = model.Question5_Status;
                    var question6Status = model.Question6_Status;
                    var question7Status = model.Question7_Status;
                    var question8Status = model.Question8_Status;
                    var question9Status = model.Question9_Status;
                    var question10Status = model.Question10_Status;
                    var approvedLoanAmt = model.ApprovedLoanAmount;

                    var TermName = model.TermName; //
                    var approvedTerm = model.ApprovedTerm; //Days
                    string ApprovedTermType = model.ApprovedTermType;
                    double Admin_Fee = 0;
                    double Penalty_Rate = 0;
                    int _Tenure = 0;
                    double _Late_fee = 0;
                    int Term = 0;
                    if (ApprovedTermType == "Weekly")
                    {
                        Term = 1;
                        _Tenure = 7;
                    }
                    if (ApprovedTermType == "Bi-Weekly")
                    {
                        Term = 2;
                        _Tenure = 14;
                    }
                    if (ApprovedTermType == "Monthly")
                    {
                        Term = 3;
                        _Tenure = 28;
                    }
                    //Update the Term and TermName value with Approved Term//GetTermDetailsByDaysandType
                    logger.WriteErrorLogs($"model.TermName : {model.TermName}", "Versdhfghdsjgfhjsdgfjhgsdjhfgsdjhfgjhsdgfjdsgf");
                    var GetTermDetailsByDaysandType = DbHelper.SelectMethod(string.Format(QueryHelper.GetTermDetailsById, model.TermName));
                    if (GetTermDetailsByDaysandType != null && GetTermDetailsByDaysandType.Rows.Count > 0)
                    {
                        Admin_Fee = Convert.ToDouble(GetTermDetailsByDaysandType.Rows[0]["admin_fee"]);
                        Penalty_Rate = Convert.ToDouble(GetTermDetailsByDaysandType.Rows[0]["late_rate"]);
                        logger.WriteErrorLogs($"Penalty_Rate:{Penalty_Rate}\n Converted ToDecimal Penalty_Rate : {Convert.ToDecimal(GetTermDetailsByDaysandType.Rows[0]["late_rate"])}\n Converted ToString Penalty_Rate : {Convert.ToString(GetTermDetailsByDaysandType.Rows[0]["late_rate"])}", $"{MethodBase.GetCurrentMethod().Name}");
                        _Late_fee = Convert.ToDouble(GetTermDetailsByDaysandType.Rows[0]["late_penalty"]);
                    }

                    var approvedInterestRate = model.ApprovedInterestRate;
                    var approvedMaturityDate = model.ApprovedMaturityDate == null ? "" : model.ApprovedMaturityDate.Replace("'", "''");

                    if (approvedMaturityDate != "" && approvedMaturityDate != null)
                    {
                        string[] ar = approvedMaturityDate.Split('-');
                        string mdStr = string.Empty;
                        mdStr = Convert.ToString(ar[2]) + "-" + Convert.ToString(ar[1]) + "-" + Convert.ToString(ar[0]);
                        approvedMaturityDate = mdStr;
                    }
                    else
                    {
                        DateTime maturityDate = DateTime.Now;
                        string twoDigitDay = Convert.ToString(maturityDate.Day);
                        if (maturityDate.Day < 10)
                            twoDigitDay = "0" + twoDigitDay;

                        string twoDigitMonth = Convert.ToString(maturityDate.Month);
                        if (maturityDate.Month < 10)
                            twoDigitMonth = "0" + twoDigitMonth;

                        string strDate = string.Empty;
                        strDate = maturityDate.Year + "-" + twoDigitMonth + "-" + twoDigitDay;
                        approvedMaturityDate = strDate;
                    }

                    var approvedTotalDueAmt = model.ApprovedTotalDueAmount;
                    #endregion

                    #region AppDataVerificationDetails
                    var personalNameId1Status = model.personal_name_id1_status;
                    var personalNameId2Status = model.personal_name_id2_status;
                    var personalNamePiStatus = model.personal_name_pi_status;
                    var personalNamePbStatus = model.personal_name_pb_status;
                    var personalEmailId1Status = model.personal_email_id1_status;
                    var personalEmailId2Status = model.personal_email_id2_status;
                    var personalEmailPiStatus = model.personal_email_pi_status;
                    var personalEmailPbStatus = model.personal_email_pb_status;
                    var personalContactId1Status = model.personal_contact_no_id1_status;
                    var personalContactId2Status = model.personal_contact_no_id2_status;
                    var personalContactPiStatus = model.personal_contact_no_pi_status;
                    var personalContactPbStatus = model.personal_contact_no_pb_status;
                    var personalPerAddId1Status = model.personal_perma_address_id1_status;
                    var personalPerAddId2Status = model.personal_perma_address_id2_status;
                    var personalPerAddPiStatus = model.personal_perma_address_pi_status;
                    var personalPerAddPbStatus = model.personal_perma_address_pb_status;
                    var personalHousePhId1Status = model.personal_house_ph_id1_status;
                    var personalHousePhId2Status = model.personal_house_ph_id2_status;
                    var personalHousePhPiStatus = model.personal_house_ph_pi_status;
                    var personalHousePhPbStatus = model.personal_house_ph_pb_status;
                    var personalBirthPlaceId1Status = model.personal_birth_place_id1_status;
                    var personalBirthPlaceId2Status = model.personal_birth_place_id2_status;
                    var personalBirthPlacePiStatus = model.personal_birth_place_pi_status;
                    var personalBirthPlacePbStatus = model.personal_birth_place_pb_status;
                    var personalBirthDateId1Status = model.personal_birth_date_id1_status;
                    var personalBirthDateId2Status = model.personal_birth_date_id2_status;
                    var personalBirthDatePiStatus = model.personal_birth_date_pi_status;
                    var personalBirthDatePbStatus = model.personal_birth_date_pb_status;
                    var personalCivilId1Status = model.personal_civil_id1_status;
                    var personalCivilId2Status = model.personal_civil_id2_status;
                    var personalCivilPiStatus = model.personal_civil_pi_status;
                    var personalCivilPbStatus = model.personal_civil_pb_status;
                    var personalMotherMaidenNameId1Status = model.personal_mother_maiden_name_id1_status;
                    var personalMotherMaidenNameId2Status = model.personal_mother_maiden_name_id2_status;
                    var personalMotherMaidenNamePiStatus = model.personal_mother_maiden_name_pi_status;
                    var personalMotherMaidenNamePbStatus = model.personal_mother_maiden_name_pb_status;
                    var personalMotherPerAddId1Status = model.personal_mother_perma_add_id1_status;
                    var personalMotherPerAddId2Status = model.personal_mother_perma_add_id2_status;
                    var personalMotherPerAddPiStatus = model.personal_mother_perma_add_pi_status;
                    var personalMotherPerAddPbStatus = model.personal_mother_perma_add_pb_status;
                    var personalCompanyNameId1Status = model.personal_company_name_id1_status;
                    var personalCompanyNameId2Status = model.personal_company_name_id2_status;
                    var personalCompanyNamePiStatus = model.personal_company_name_pi_status;
                    var personalCompanyNamePbStatus = model.personal_company_name_pb_status;
                    var personalCompanyAddId1Status = model.personal_company_address_id1_status;
                    var personalCompanyAddId2Status = model.personal_company_address_id2_status;
                    var personalCompanyAddPiStatus = model.personal_company_address_pi_status;
                    var personalCompanyAddPbStatus = model.personal_company_address_pb_status;
                    var personalJobTitleId1Status = model.personal_job_title_id1_status;
                    var personalJobTitleId2Status = model.personal_job_title_id2_status;
                    var personalJobTitlePiStatus = model.personal_job_title_pi_status;
                    var personalJobTitlePbStatus = model.personal_job_title_pb_status;
                    var personalMonthlyIncomeId1Status = model.personal_monthly_income_id1_status;
                    var personalMonthlyIncomeId2Status = model.personal_monthly_income_id2_status;
                    var personalMonthlyIncomePiStatus = model.personal_monthly_income_pi_status;
                    var personalMonthlyIncomePbStatus = model.personal_monthly_income_pb_status;
                    var personalRefNameId1Status = model.personal_reference_name_id1_status;
                    var personalRefNameId2Status = model.personal_reference_name_id2_status;
                    var personalRefNamePiStatus = model.personal_reference_name_pi_status;
                    var personalRefNamePbStatus = model.personal_reference_name_pb_status;
                    var personalRefContactId1Status = model.personal_reference_contact_id1_status;
                    var personalRefContactId2Status = model.personal_reference_contact_id2_status;
                    var personalRefContactPiStatus = model.personal_reference_contact_pi_status;
                    var personalRefContactPbStatus = model.personal_reference_contact_pb_status;
                    var personalBankNameId1Status = model.personal_bank_name_id1_status;
                    var personalBankNameId2Status = model.personal_bank_name_id2_status;
                    var personalBankNamePiStatus = model.personal_bank_name_pi_status;
                    var personalBankNamePbStatus = model.personal_bank_name_pb_status;
                    var personalBankAccNoId1Status = model.personal_bank_ac_no_id1_status;
                    var personalBankAccNoId2Status = model.personal_bank_ac_no_id2_status;
                    var personalBankAccNoPiStatus = model.personal_bank_ac_no_pi_status;
                    var personalBankAccNoPbStatus = model.personal_bank_ac_no_pb_status;
                    var personalReqLoanAmtId1Status = model.personal_req_loan_amt_id1_status;
                    var personalReqLoanAmtId2Status = model.personal_req_loan_amt_id2_status;
                    var personalReqLoanAmtPiStatus = model.personal_req_loan_amt_pi_status;
                    var personalReqLoanAmtPbStatus = model.personal_req_loan_amt_pb_status;
                    var personalReqLoanTermId1Status = model.personal_req_loan_term_id1_status;
                    var personalReqLoanTermId2Status = model.personal_req_loan_term_id2_status;
                    var personalReqLoanTermPiStatus = model.personal_req_loan_term_pi_status;
                    var personalReqLoanTermPbStatus = model.personal_req_loan_term_pb_status;
                    var personalNameRemark = model.personal_name_remark == null ? "" : model.personal_name_remark.Replace("'", "''");
                    var personalEmailRemark = model.personal_email_remark == null ? "" : model.personal_email_remark.Replace("'", "''");
                    var persoanlContactRemark = model.personal_contact_no_remark == null ? "" : model.personal_contact_no_remark.Replace("'", "''");
                    var persoanlPerAddRemark = model.personal_perma_address_remark == null ? "" : model.personal_perma_address_remark.Replace("'", "''");
                    var personalHousePhRemark = model.personal_house_ph_remark == null ? "" : model.personal_house_ph_remark.Replace("'", "''");
                    var personalBirthPlaceRemark = model.personal_birth_place_remark == null ? "" : model.personal_birth_place_remark.Replace("'", "''");
                    var personalBirthDateRemark = model.personal_birth_date_remark == null ? "" : model.personal_birth_date_remark.Replace("'", "''");
                    var personalCivilRemark = model.personal_civil_remark == null ? "" : model.personal_civil_remark.Replace("'", "''");
                    var personalMotherMaidenNameRemark = model.personal_mother_maiden_name_remark == null ? "" : model.personal_mother_maiden_name_remark.Replace("'", "''");
                    var personalMotherPerAddRemark = model.personal_mother_perma_add_remark == null ? "" : model.personal_mother_perma_add_remark.Replace("'", "''");
                    var personalCompanyNameRemark = model.personal_company_name_remark == null ? "" : model.personal_company_name_remark.Replace("'", "''");
                    var personalCompanyAddRemark = model.personal_company_address_remark == null ? "" : model.personal_company_address_remark.Replace("'", "''");
                    var personalJobTitleremark = model.personal_job_title_remark == null ? "" : model.personal_job_title_remark.Replace("'", "''");
                    var personalMonthlyIncomeRemark = model.personal_monthly_income_remark == null ? "" : model.personal_monthly_income_remark.Replace("'", "''");
                    var personalRefNameRemark = model.personal_reference_name_remark == null ? "" : model.personal_reference_name_remark.Replace("'", "''");
                    var personalRefContactRemark = model.personal_reference_contact_remark == null ? "" : model.personal_reference_contact_remark.Replace("'", "''");
                    var personalBankNameRemark = model.personal_bank_name_remark == null ? "" : model.personal_bank_name_remark.Replace("'", "''");
                    var personalBankAccNoRemark = model.personal_bank_ac_no_remark == null ? "" : model.personal_bank_ac_no_remark.Replace("'", "''");
                    var personalReqLoanAmtRemark = model.personal_req_loan_amt_remark == null ? "" : model.personal_req_loan_amt_remark.Replace("'", "''");
                    var personalReqLoanTermRemark = model.personal_req_loan_term_remark == null ? "" : model.personal_req_loan_term_remark.Replace("'", "''");
                    #endregion

                    #region addsome extra field
                    //add field
                    var name = model.Name == null ? "" : model.Name.Replace("'", "''");
                    var address = model.Address == null ? "" : model.Address.Replace("'", "''");
                    var completeaddress = $"{(model.BarangayName == null ? "" : model.BarangayName.Replace("'", "''"))},{(model.CityName == null ? "" : model.CityName.Replace("'", "''"))},{(model.ProvinceName == null ? "" : model.ProvinceName.Replace("'", "''"))},{(model.ZipCode == null ? "" : model.ZipCode.Replace("'", "''"))}";
                    var city = model.CityName == null ? "" : model.CityName.Replace("'", "''");
                    var dob = model.DOB == null ? "" : model.DOB.Replace("'", "''");
                    var civilstatus = model.CivilStatus;
                    var CivilStatusText = model.CivilStatusText == null ? "" : model.CivilStatusText.Replace("'", "''");
                    var companyName = model.CompanyName == null ? "" : model.CompanyName.Replace("'", "''");
                    var compnayaddress = model.CompanyAddress == null ? "" : model.CompanyAddress.Replace("'", "''");
                    var company_phNo = model.Company_Phoneno == null ? "" : model.Company_Phoneno.Replace("'", "''");
                    var designation = model.Designation == null ? "" : model.Designation.Replace("'", "''");
                    var GrossIncome = model.GrossIncome;
                    var loanAmt = model.Loan_Amount;
                    var termtype = model.TermType == null ? "" : model.TermType.Replace("'", "''");
                    var term = model.Term == null ? "" : model.Term.Replace("'", "''");
                    var MotherMaidenName = model.MotherMaidenName == null ? "" : model.MotherMaidenName.Replace("'", "''");
                    var MotherPermanentAddress = model.MotherAddress == null ? "" : model.MotherAddress.Replace("'", "''");
                    var Designation = model.Designation == null ? "" : model.Designation.Replace("'", "''");
                    var GrossMonthlyIncome = model.GrossIncome;
                    var refereneceName = model.RefName == null ? "" : model.RefName.Replace("'", "''");
                    var ReferenceContactNo = model.RefContactNo == null ? "" : model.RefContactNo.Replace("'", "''");
                    var bankAcctNo = model.BankAccNo == null ? "" : model.BankAccNo.Replace("'", "''");
                    var requestLoanAmt = model.AdditionalRequestedLoanAmount;
                    var requestTerm = model.AdditionalRequested_Term == null ? "" : model.AdditionalRequested_Term.Replace("'", "''");
                    var personalemailaddress = model.PersonalEmail == null ? "" : model.PersonalEmail.Replace("'", "''");
                    var personalContactNo = model.PersonalContactNo == null ? "" : model.PersonalContactNo.Replace("'", "''");
                    var HomePhone = model.HomePhone == null ? "" : model.HomePhone.Replace("'", "''");
                    var birthPlace = model.BirthPlace == null ? "" : model.BirthPlace.Replace("'", "''");
                    //End
                    #endregion

                    if (String.IsNullOrWhiteSpace(employmentBorrowerMonthlySalary))
                    {
                        employmentBorrowerMonthlySalary = "0.00";
                    }
                    var currentDate = DateTime.Now;
                    var existquery = DbHelper.SelectMethod(string.Format(QueryHelper.ExistVerifierRecord, appNo));
                    model.ExistData = Convert.ToString(existquery.Rows[0]["existdetail"]);
                    if (model.ExistData == "1")
                    {
                        int deleteContract = DbHelper.InsertUpdateDelete(string.Format(QueryHelper.DeleteContractDetails, appNo));
                        int update = DbHelper.InsertUpdateDelete(string.Format("UPDATE tblapplication_record set term=" + TermName + ", designation='{116}',gov_doc_type='" + model.gov_doc_type + "',SSS_No='" + model.SSS_No + "',home_status={83},purposeofloan='{10}', bankname='" + model.BankName + "', industry='{94}', isverified = true, verify_by='{117}', isreverified = false, isrecheck = false, rejectedbyverifier=false,  rejectedbyapprover=false,gender='" + model.gender + "' where applicationno='{0}'; UPDATE tblapplication_personal_verification_details SET requestedamt = '{1}', requestedterms = '{2}', id1_status = '{3}', id2_status = '{4}', pi_status = '{5}', pb_status = '{6}', remarks = '{7}', addtional_birth_place = '{8}', additonal_provincial_address = '{9}', additional_loan_purpose = '{118}', additional_requested_loan_amount = '{11}', additional_requested_term = '{12}', additional_employed_duration = '{13}', additional_job_position = '{14}', additional_job_level = '{15}', additional_paydate = '{16}', family_personal_name_status = '{17}', family_personal_contact_status = '{18}', family_relation_with_borrower_status = '{19}', family_borrower_known_duration_status = '{20}', family_address_verification_status = '{21}', family_borrower_working_place_status = '{22}', friend_personal_name_status = '{23}', friend_personal_contact_status = '{24}', friend_relation_with_borrower_status = '{25}', friend_borrower_known_duration_status = '{26}', friend_address_verification_status = '{27}', friend_borrower_working_place = '{28}', co_worker_personal_name_status = '{29}', co_worker_personal_contact_status = '{30}', co_worker_relation_with_borrower_status = '{31}', co_worker_borrower_known_duration_status = '{32}', co_worker_address_verification_status = '{33}', co_worker_borrower_working_place = '{34}', employement_nameof_work_contact_status = '{35}', employement_no_of_work_contact_status = '{36}', employement_borrower_working_status = '{37}', employement_borrower_position_status = '{38}', employement_borrower_monthly_salary_status = '{39}', employement_borrower_attendance_status = '{40}', employement_borrower_bank_payroll_status = '{41}', family_personal_name = '{42}', family_personal_contact = '{43}', family_relation_with_borrower = '{44}', family_borrower_known_duration = '{45}', family_address_verification = '{46}', family_borrower_working_place = '{47}', friend_personal_name = '{48}', friend_personal_contact = '{49}', friend_relation_with_borrower = '{50}', friend_borrower_known_duration = '{51}', friend_address_verification = '{52}', friend_borrower_working = '{53}', co_worker_personal_name = '{54}', co_worker_personal_contact = '{55}', co_worker_relation_with_borrower = '{56}', co_worker_borrower_known_duration = '{57}', co_worker_address_verification = '{58}', co_worker_borrower_working = '{59}', employement_nameof_work_contact = '{60}', employement_no_of_work_contact = '{61}', employement_borrower_working = '{62}', employement_borrower_position = '{63}', employement_borrower_monthly_salary = '{64}', employement_borrower_attendance = '{65}', employement_borrower_bank_payroll = '{66}', question1_status = '{67}', question2_status = '{68}', question3_status = '{69}', question4_status = '{70}', question5_status = '{71}', question6_status = '{72}', question7_status = '{73}', question8_status = '{74}', question9_status = '{75}', question10_status = '{76}', approved_loan_amount = '{77}', approved_term = '{78}', approved_interest_rate = '{79}', approved_maturity_date = '{80}', approved_total_amount_due = '{81}',nearest_landmark='{82}',  tansfer_residence='{84}', spouse_name='{85}', spouse_occupation='{86}', number_of_dependent='{87}', mother_work='{88}', father_name='{89}', father_work='{90}', living_with_mother='{91}', sibling_count='{92}', sibling_works='{93}', occupation='{94}', net_income='{96}', pending_resignation='{97}', other_source_of_income='{98}', know_about_cashmart='{99}', pending_loan_fron_otherland='{100}', bank_loan_or_credit_card='{101}', permanent_address='{102}', bankid='{103}', family_name1='{104}', family_address1='{105}', family_contact1='{106}', family_relation1='{107}', family_name2='{108}', family_address2='{109}', family_contact2='{110}', family_relation2='{111}', optional_name='{112}', optional_address='{113}', optional_contact='{114}', optional_relation='{115}' WHERE application_no = '{0}';", appNo, requestedAmt, requestedTerms, id1Status, id2Status, piStatus, pbStatus, remarks, addBirthPlace, addProvincialAdd, addLoanPurpose, addRequestedLoanAmount, addrequestedTerm, addEmpDuration, addJobPosition, addJobLevel, addPayDate, familyPersonalNameStatus, familyPersonalContactStatus, familyRelationWithBorrowerStatus, familyBorrowerKnownDurationStatus, familyAddVerificationStatus, familyBorrowerWorkingPlaceStatus, friendPersonalNameStatus, friendPersonalContactStatus, friendRelationWithBorrowerStatus, friendBorrowerKnownDurationStatus, friendAddVerificationStatus, friendBorrowerWorkingPlaceStatus, coworkerPersonalNameStatus, coworkerPersonalContactStatus, coworkerRelationWithBorrowerStatus, coworkerBorrowerKnownDurationStatus, coworkerAddVerificationStatus, coworkerBorrowerWorkingPlaceStatus, employmentNameOfWorkContactStatus, employmentNoOfWorkContactStatus, employmentBorrowerWorkingStatus, employmentBorrowerPositionStatus, employmentBorrowerMonthlySalaryStatus, employmentBorrowerAttendanceStatus, employmentBorrowerBankPayrollStatus, familyPersonalName, familyPersonalContact, familyRelationWithBorrower, familyBorrowerKnownDuration, familyAddVerification, familyBorrowerWorkingPlace, friendPersonalName, friendPersonalContact, friendRelationWithBorrower, friendBorrowerKnownDuration, friendAddVerification, friendBorrowerWorkingPlace, coworkerPersonalName, coworkerPersonalContact, coworkerRelationWithBorrower, coworkerBorrowerKnownDuration, coworkerAddVerification, coworkerBorrowerWorkingPlace, employmentNameOfWorkContact, employmentNoOfWorkContact, employmentBorrowerWorking, employmentBorrowerPosition, employmentBorrowerMonthlySalary, employmentBorrowerAttendance, employmentBorrowerBankPayroll, question1Status, question2Status, question3Status, question4Status, question5Status, question6Status, question7Status, question8Status, question9Status, question10Status, approvedLoanAmt, approvedTerm, approvedInterestRate, approvedMaturityDate, approvedTotalDueAmt, nearest_landmark, home_status, tansfer_residence, spouse_name, spouse_occupation, number_of_dependent, mother_work, father_name, father_work, living_with_mother, sibling_count, sibling_works, occupation, "", net_income, pending_resignation, other_source_of_income, know_about_cashmart, pending_loan_fron_otherland, bank_loan_or_credit_card, Permanent_address, BankId, family_name1, family_address1, family_contact1, family_relation1, family_name2, family_address2, family_contact2, family_relation2, optional_name, optional_address, optional_contact, optional_relation, Designation, user_id, AdditionalLoanPurpose_Remark));
                        if (update > 1)
                        {
                            int UpdateQuery = DbHelper.InsertUpdateDelete(string.Format("UPDATE tblapplicationdata_verification_details SET personal_name_id1_status='{1}',personal_name_id2_status='{2}',personal_name_pi_status='{3}',personal_name_pb_status='{4}',personal_email_id1_status='{5}',personal_email_id2_status='{6}',personal_email_pi_status='{7}',personal_email_pb_status='{8}',personal_contact_no_id1_status='{9}',personal_contact_no_id2_status='{10}',personal_contact_no_pi_status='{11}',personal_contact_no_pb_status='{12}',personal_perma_address_id1_status='{13}',personal_perma_address_id2_status='{14}',personal_perma_address_pi_status='{15}',personal_perma_address_pb_status='{16}',personal_house_ph_id1_status='{17}',personal_house_ph_id2_status='{18}',personal_house_ph_pi_status='{19}',personal_house_ph_pb_status='{20}',personal_birth_place_id1_status='{21}',personal_birth_place_id2_status='{22}',personal_birth_place_pi_status='{23}',personal_birth_place_pb_status='{24}',personal_birth_date_id1_status='{25}',personal_birth_date_id2_status='{26}',personal_birth_date_pi_status='{27}',personal_birth_date_pb_status='{28}',personal_civil_id1_status='{29}',personal_civil_id2_status='{30}',personal_civil_pi_status='{31}',personal_civil_pb_status='{32}',personal_mother_maiden_name_id1_status='{33}',personal_mother_maiden_name_id2_status='{34}',personal_mother_maiden_name_pi_status='{35}',personal_mother_maiden_name_pb_status='{36}',personal_mother_perma_add_id1_status='{37}',personal_mother_perma_add_id2_status='{38}',personal_mother_perma_add_pi_status='{39}',personal_mother_perma_add_pb_status='{40}',personal_company_name_id1_status='{41}',personal_company_name_id2_status='{42}',personal_company_name_pi_status='{43}',personal_company_name_pb_status='{44}',personal_company_address_id1_status='{45}',personal_company_address_id2_status='{46}',personal_company_address_pi_status='{47}',personal_company_address_pb_status='{48}',personal_job_title_id1_status='{49}',personal_job_title_id2_status='{50}',personal_job_title_pi_status='{51}',personal_job_title_pb_status='{52}',personal_monthly_income_id1_status='{53}',personal_monthly_income_id2_status='{54}',personal_monthly_income_pi_status='{55}',personal_monthly_income_pb_status='{56}',personal_reference_name_id1_status='{57}',personal_reference_name_id2_status='{58}',personal_reference_name_pi_status='{59}',personal_reference_name_pb_status='{60}',personal_reference_contact_id1_status='{61}',personal_reference_contact_id2_status='{62}',personal_reference_contact_pi_status='{63}',personal_reference_contact_pb_status='{64}',personal_bank_name_id1_status='{65}',personal_bank_name_id2_status='{66}',personal_bank_name_pi_status='{67}',personal_bank_name_pb_status='{68}',personal_bank_ac_no_id1_status='{69}',personal_bank_ac_no_id2_status='{70}',personal_bank_ac_no_pi_status='{71}',personal_bank_ac_no_pb_status='{72}',personal_req_loan_amt_id1_status='{73}',personal_req_loan_amt_id2_status='{74}',personal_req_loan_amt_pi_status='{75}',personal_req_loan_amt_pb_status='{76}',personal_req_loan_term_id1_status='{77}',personal_req_loan_term_id2_status='{78}',personal_req_loan_term_pi_status='{79}',personal_req_loan_term_pb_status='{80}',personal_name_remark='{81}',personal_email_remark='{82}',personal_contact_no_remark='{83}',personal_perma_address_remark='{84}',personal_house_ph_remark='{85}',personal_birth_place_remark='{86}',personal_birth_date_remark='{87}',personal_civil_remark='{88}',personal_mother_maiden_name_remark='{89}',personal_mother_perma_add_remark='{90}',personal_company_name_remark='{91}',personal_company_address_remark='{92}',personal_job_title_remark='{93}',personal_monthly_income_remark='{94}',personal_reference_name_remark='{95}',personal_reference_contact_remark='{96}',personal_bank_name_remark='{97}',personal_bank_ac_no_remark='{98}',personal_req_loan_amt_remark='{99}',personal_req_loan_term_remark='{100}',nearest_landmark_remark='{101}', sss_no_remark='{102}', rented_mortgage_owned_remark='{103}', permanent_address_remark='{104}', tansfer_residence_remark='{105}', spouse_name_remark='{106}', spouse_occupation_remark='{107}', number_of_dependent_remark='{108}', father_name_work_remark='{109}', sibling_works_remark='{110}', occupation_remark='{111}', net_income_remark='{112}', scheduled_and_btc_remark='{113}', pay_date_remark='{114}', pending_resignation_remark='{115}', other_source_of_income_remark='{116}', know_about_cashmart_remark='{117}', pending_loan_fron_otherland_remark='{118}', bank_loan_or_credit_card_remark='{119}', family_name1_remark='{120}', family_address1_remark='{121}', family_contact1_remark='{122}', family_relation1_remark='{123}', family_name2_remark='{124}', family_address2_remark='{125}', family_contact2_remark='{126}', family_relation2_remark='{127}', optional_name_remark='{128}', optional_address_remark='{129}', optional_contact_remark='{130}', optional_relation_remark='{131}' WHERE application_no ={0};Update tblapplication_record Set isverified=true,verify_by='{132}', isrecheck=false, rejectedbyverifier=false, rejectedbyapprover=false where applicationno={0};", appNo, personalNameId1Status, personalNameId2Status, personalNamePiStatus, personalNamePbStatus, personalEmailId1Status, personalEmailId2Status, personalEmailPiStatus, personalEmailPbStatus, personalContactId1Status, personalContactId2Status, personalContactPiStatus, personalContactPbStatus, personalPerAddId1Status, personalPerAddId2Status, personalPerAddPiStatus, personalPerAddPbStatus, personalHousePhId1Status, personalHousePhId2Status, personalHousePhPiStatus, personalHousePhPbStatus, personalBirthPlaceId1Status, personalBirthPlaceId2Status, personalBirthPlacePiStatus, personalBirthPlacePbStatus, personalBirthDateId1Status, personalBirthDateId2Status, personalBirthDatePiStatus, personalBirthDatePbStatus, personalCivilId1Status, personalCivilId2Status, personalCivilPiStatus, personalCivilPbStatus, personalMotherMaidenNameId1Status, personalMotherMaidenNameId2Status, personalMotherMaidenNamePiStatus, personalMotherMaidenNamePbStatus, personalMotherPerAddId1Status, personalMotherPerAddId2Status, personalMotherPerAddPiStatus, personalMotherPerAddPbStatus, personalCompanyNameId1Status, personalCompanyNameId2Status, personalCompanyNamePiStatus, personalCompanyNamePbStatus, personalCompanyAddId1Status, personalCompanyAddId2Status, personalCompanyAddPiStatus, personalCompanyAddPbStatus, personalJobTitleId1Status, personalJobTitleId2Status, personalJobTitlePiStatus, personalJobTitlePbStatus, personalMonthlyIncomeId1Status, personalMonthlyIncomeId2Status, personalMonthlyIncomePiStatus, personalMonthlyIncomePbStatus, personalRefNameId1Status, personalRefNameId2Status, personalRefNamePiStatus, personalRefNamePbStatus, personalRefContactId1Status, personalRefContactId2Status, personalRefContactPiStatus, personalRefContactPbStatus, personalBankNameId1Status, personalBankNameId2Status, personalBankNamePiStatus, personalBankNamePbStatus, personalBankAccNoId1Status, personalBankAccNoId2Status, personalBankAccNoPiStatus, personalBankAccNoPbStatus, personalReqLoanAmtId1Status, personalReqLoanAmtId2Status, personalReqLoanAmtPiStatus, personalReqLoanAmtPbStatus, personalReqLoanTermId1Status, personalReqLoanTermId2Status, personalReqLoanTermPiStatus, personalReqLoanTermPbStatus, personalNameRemark, personalEmailRemark, persoanlContactRemark, persoanlPerAddRemark, personalHousePhRemark, personalBirthPlaceRemark, personalBirthDateRemark, personalCivilRemark, personalMotherMaidenNameRemark, personalMotherPerAddRemark, personalCompanyNameRemark, personalCompanyAddRemark, personalJobTitleremark, personalMonthlyIncomeRemark, personalRefNameRemark, personalRefContactRemark, personalBankNameRemark, personalBankAccNoRemark, personalReqLoanAmtRemark, personalReqLoanTermRemark, nearest_landmark_remark, SSS_No_remark, rented_mortgage_owned_remark, Permanent_address_remark, tansfer_residence_remark, spouse_name_remark, spouse_occupation_remark, number_of_dependent_remark, father_name_work_remark, sibling_works_remark, occupation_remark, net_income_remark, scheduled_and_btc_remark, pay_date_remark, pending_resignation_remark, other_source_of_income_remark, know_about_cashmart_remark, pending_loan_fron_otherland_remark, bank_loan_or_credit_card_remark, family_name1_remark, family_address1_remark, family_contact1_remark, family_relation1_remark, family_name2_remark, family_address2_remark, family_contact2_remark, family_relation2_remark, optional_name_remark, optional_address_remark, optional_contact_remark, optional_relation_remark, user_id));

                            if (UpdateQuery > 0)
                            {
                                logger.WriteErrorLogs($"Term Name:{TermName} ===>  Term Type:{Term}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                                if (TermName > 0 && Term > 0)
                                {
                                    var updateQuery = DbHelper.InsertUpdateDelete(string.Format(QueryHelper.Updateispickedverifier, currentDate, appNo, TermName, Term));
                                    if (updateQuery > 0)
                                    {
                                        var updateQuery1 = DbHelper.InsertUpdateDelete("update tblapplication_record set address = '" + address + "', homephoneno = '" + HomePhone + "', mothermaidenname = '" + MotherMaidenName + "', motheraddress = '" + MotherPermanentAddress + "', companyname = '" + companyName + "', companyaddress = '" + compnayaddress + "', gross_income = '" + GrossMonthlyIncome + "', reference_name = '" + refereneceName + "', reference_contactno = '" + ReferenceContactNo + "', bankname = '" + model.BankName + "', bankaccountno = '" + bankAcctNo + "' where applicationno = '" + appNo + "'");
                                        TempData["Result"] = "Data Saved Successfully.";
                                    }
                                }
                            }
                            else
                            {
                                int insertQuery = DbHelper.InsertUpdateDelete(string.Format("insert into tblapplicationdata_verification_details(application_no, personal_name_id1_status, personal_name_id2_status,personal_name_pi_status, personal_name_pb_status, personal_email_id1_status,personal_email_id2_status, personal_email_pi_status, personal_email_pb_status,personal_contact_no_id1_status, personal_contact_no_id2_status,personal_contact_no_pi_status, personal_contact_no_pb_status,personal_perma_address_id1_status, personal_perma_address_id2_status,personal_perma_address_pi_status, personal_perma_address_pb_status,personal_house_ph_id1_status, personal_house_ph_id2_status, personal_house_ph_pi_status,personal_house_ph_pb_status, personal_birth_place_id1_status,personal_birth_place_id2_status, personal_birth_place_pi_status,personal_birth_place_pb_status, personal_birth_date_id1_status,personal_birth_date_id2_status, personal_birth_date_pi_status,personal_birth_date_pb_status, personal_civil_id1_status, personal_civil_id2_status,personal_civil_pi_status, personal_civil_pb_status, personal_mother_maiden_name_id1_status,personal_mother_maiden_name_id2_status, personal_mother_maiden_name_pi_status,personal_mother_maiden_name_pb_status, personal_mother_perma_add_id1_status,personal_mother_perma_add_id2_status, personal_mother_perma_add_pi_status,personal_mother_perma_add_pb_status, personal_company_name_id1_status,personal_company_name_id2_status, personal_company_name_pi_status,personal_company_name_pb_status, personal_company_address_id1_status,personal_company_address_id2_status, personal_company_address_pi_status,personal_company_address_pb_status, personal_job_title_id1_status,personal_job_title_id2_status, personal_job_title_pi_status,personal_job_title_pb_status,personal_monthly_income_id1_status,personal_monthly_income_id2_status, personal_monthly_income_pi_status,personal_monthly_income_pb_status, personal_reference_name_id1_status,personal_reference_name_id2_status, personal_reference_name_pi_status,personal_reference_name_pb_status, personal_reference_contact_id1_status,personal_reference_contact_id2_status, personal_reference_contact_pi_status,personal_reference_contact_pb_status, personal_bank_name_id1_status,personal_bank_name_id2_status, personal_bank_name_pi_status,personal_bank_name_pb_status, personal_bank_ac_no_id1_status,personal_bank_ac_no_id2_status, personal_bank_ac_no_pi_status,personal_bank_ac_no_pb_status, personal_req_loan_amt_id1_status,personal_req_loan_amt_id2_status, personal_req_loan_amt_pi_status,personal_req_loan_amt_pb_status, personal_req_loan_term_id1_status,personal_req_loan_term_id2_status, personal_req_loan_term_pi_status,personal_req_loan_term_pb_status, personal_name_remark, personal_email_remark,personal_contact_no_remark, personal_perma_address_remark, personal_house_ph_remark,personal_birth_place_remark, personal_birth_date_remark, personal_civil_remark,personal_mother_maiden_name_remark, personal_mother_perma_add_remark,personal_company_name_remark, personal_company_address_remark,personal_job_title_remark, personal_monthly_income_remark, personal_reference_name_remark,personal_reference_contact_remark, personal_bank_name_remark,personal_bank_ac_no_remark, personal_req_loan_amt_remark, personal_req_loan_term_remark, nearest_landmark_remark, sss_no_remark, rented_mortgage_owned_remark, permanent_address_remark, tansfer_residence_remark, spouse_name_remark, spouse_occupation_remark, number_of_dependent_remark, father_name_work_remark, sibling_works_remark, occupation_remark, net_income_remark, scheduled_and_btc_remark, pay_date_remark, pending_resignation_remark, other_source_of_income_remark, know_about_cashmart_remark, pending_loan_fron_otherland_remark, bank_loan_or_credit_card_remark, family_name1_remark, family_address1_remark, family_contact1_remark, family_relation1_remark, family_name2_remark, family_address2_remark, family_contact2_remark, family_relation2_remark, optional_name_remark, optional_address_remark, optional_contact_remark, optional_relation_remark)VALUES('{0}','{1}', '{2}','{3}','{4}', '{5}', '{6}', '{7}', '{8}','{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}', '{16}', '{17}', '{18}', '{19}', '{20}',  '{21}', '{22}', '{23}', '{24}','{25}','{26}','{27}','{28}','{29}','{30}','{31}','{32}','{33}','{34}','{35}','{36}','{37}','{38}','{39}','{40}', '{41}', '{42}','{43}','{44}','{45}', '{46}','{47}', '{48}','{49}','{50}', '{51}','{52}','{53}','{54}','{55}','{56}','{57}','{58}', '{59}','{60}','{61}','{62}','{63}','{64}','{65}','{66}','{67}','{68}','{69}','{70}','{71}','{72}','{73}','{74}','{75}','{76}','{77}', '{78}','{79}','{80}','{81}','{82}','{83}','{84}','{85}','{86}','{87}','{88}','{89}','{90}','{91}','{92}','{93}','{94}','{95}','{96}','{97}','{98}','{99}','{100}','{101}','{102}','{103}','{104}','{105}','{106}','{107}','{108}','{109}','{110}','{111}','{112}','{113}','{114}','{115}','{116}','{117}','{118}','{119}','{120}','{121}','{122}','{123}','{124}','{125}','{126}','{127}','{128}','{129}','{130}','{131}');", appNo, personalNameId1Status, personalNameId2Status, personalNamePiStatus, personalNamePbStatus, personalEmailId1Status, personalEmailId2Status, personalEmailPiStatus, personalEmailPbStatus, personalContactId1Status, personalContactId2Status, personalContactPiStatus, personalContactPbStatus, personalPerAddId1Status, personalPerAddId2Status, personalPerAddPiStatus, personalPerAddPbStatus, personalHousePhId1Status, personalHousePhId2Status, personalHousePhPiStatus, personalHousePhPbStatus, personalBirthPlaceId1Status, personalBirthPlaceId2Status, personalBirthPlacePiStatus, personalBirthPlacePbStatus, personalBirthDateId1Status, personalBirthDateId2Status, personalBirthDatePiStatus, personalBirthDatePbStatus, personalCivilId1Status, personalCivilId2Status, personalCivilPiStatus, personalCivilPbStatus, personalMotherMaidenNameId1Status, personalMotherMaidenNameId2Status, personalMotherMaidenNamePiStatus, personalMotherMaidenNamePbStatus, personalMotherPerAddId1Status, personalMotherPerAddId2Status, personalMotherPerAddPiStatus, personalMotherPerAddPbStatus, personalCompanyNameId1Status, personalCompanyNameId2Status, personalCompanyNamePiStatus, personalCompanyNamePbStatus, personalCompanyAddId1Status, personalCompanyAddId2Status, personalCompanyAddPiStatus, personalCompanyAddPbStatus, personalJobTitleId1Status, personalJobTitleId2Status, personalJobTitlePiStatus, personalJobTitlePbStatus, personalMonthlyIncomeId1Status, personalMonthlyIncomeId2Status, personalMonthlyIncomePiStatus, personalMonthlyIncomePbStatus, personalRefNameId1Status, personalRefNameId2Status, personalRefNamePiStatus, personalRefNamePbStatus, personalRefContactId1Status, personalRefContactId2Status, personalRefContactPiStatus, personalRefContactPbStatus, personalBankNameId1Status, personalBankNameId2Status, personalBankNamePiStatus, personalBankNamePbStatus, personalBankAccNoId1Status, personalBankAccNoId2Status, personalBankAccNoPiStatus, personalBankAccNoPbStatus, personalReqLoanAmtId1Status, personalReqLoanAmtId2Status, personalReqLoanAmtPiStatus, personalReqLoanAmtPbStatus, personalReqLoanTermId1Status, personalReqLoanTermId2Status, personalReqLoanTermPiStatus, personalReqLoanTermPbStatus, personalNameRemark, personalEmailRemark, persoanlContactRemark, persoanlPerAddRemark, personalHousePhRemark, personalBirthPlaceRemark, personalBirthDateRemark, personalCivilRemark, personalMotherMaidenNameRemark, personalMotherPerAddRemark, personalCompanyNameRemark, personalCompanyAddRemark, personalJobTitleremark, personalMonthlyIncomeRemark, personalRefNameRemark, personalRefContactRemark, personalBankNameRemark, personalBankAccNoRemark, personalReqLoanAmtRemark, personalReqLoanTermRemark, nearest_landmark_remark, SSS_No_remark, rented_mortgage_owned_remark, Permanent_address_remark, tansfer_residence_remark, spouse_name_remark, spouse_occupation_remark, number_of_dependent_remark, father_name_work_remark, sibling_works_remark, occupation_remark, net_income_remark, scheduled_and_btc_remark, pay_date_remark, pending_resignation_remark, other_source_of_income_remark, know_about_cashmart_remark, pending_loan_fron_otherland_remark, bank_loan_or_credit_card_remark, family_name1_remark, family_address1_remark, family_contact1_remark, family_relation1_remark, family_name2_remark, family_address2_remark, family_contact2_remark, family_relation2_remark, optional_name_remark, optional_address_remark, optional_contact_remark, optional_relation_remark));
                                if (insertQuery > 0)
                                {
                                    var updateQuery1 = DbHelper.InsertUpdateDelete("update tblapplication_record set address = '" + address + "', homephoneno = '" + HomePhone + "', mothermaidenname = '" + MotherMaidenName + "', motheraddress = '" + MotherPermanentAddress + "', companyname = '" + companyName + "', companyaddress = '" + compnayaddress + "', gross_income = '" + GrossMonthlyIncome + "', reference_name = '" + refereneceName + "', reference_contactno = '" + ReferenceContactNo + "', bankname = '" + model.BankName + "', bankaccountno = '" + bankAcctNo + "', term='" + TermName + "',termtype='" + Term + "', purposeofloan='" + addLoanPurpose + "' where applicationno = '" + appNo + "'");


                                }
                            }
                        }
                    }
                    else
                    {
                        int insert = DbHelper.InsertUpdateDelete(string.Format("UPDATE tblapplication_record set term=" + TermName + ", designation='{116}',gov_doc_type='" + model.gov_doc_type + "',SSS_No='" + model.SSS_No + "',home_status={83},bankname='" + model.BankName + "', industry='" + occupation + "', isverified=true,verify_by='{117}', isreverified = 'false', rejectedbyverifier=false, rejectedbyapprover=false,gender='" + model.gender + "' where applicationno='{0}';Insert Into tblapplication_personal_verification_details(application_no, requestedamt, requestedterms, id1_status, id2_status, pi_status, pb_status, remarks, addtional_birth_place, additonal_provincial_address, additional_loan_purpose, additional_requested_loan_amount, additional_requested_term, additional_employed_duration, additional_job_position, additional_job_level, additional_paydate, family_personal_name_status, family_personal_contact_status, family_relation_with_borrower_status, family_borrower_known_duration_status, family_address_verification_status, family_borrower_working_place_status, friend_personal_name_status, friend_personal_contact_status, friend_relation_with_borrower_status, friend_borrower_known_duration_status, friend_address_verification_status, friend_borrower_working_place, co_worker_personal_name_status, co_worker_personal_contact_status, co_worker_relation_with_borrower_status, co_worker_borrower_known_duration_status, co_worker_address_verification_status, co_worker_borrower_working_place, employement_nameof_work_contact_status, employement_no_of_work_contact_status, employement_borrower_working_status, employement_borrower_position_status, employement_borrower_monthly_salary_status, employement_borrower_attendance_status, employement_borrower_bank_payroll_status, family_personal_name, family_personal_contact, family_relation_with_borrower, family_borrower_known_duration, family_address_verification, family_borrower_working_place, friend_personal_name, friend_personal_contact, friend_relation_with_borrower, friend_borrower_known_duration, friend_address_verification, friend_borrower_working, co_worker_personal_name, co_worker_personal_contact, co_worker_relation_with_borrower, co_worker_borrower_known_duration, co_worker_address_verification, co_worker_borrower_working, employement_nameof_work_contact, employement_no_of_work_contact, employement_borrower_working, employement_borrower_position, employement_borrower_monthly_salary, employement_borrower_attendance, employement_borrower_bank_payroll, question1_status, question2_status, question3_status, question4_status, question5_status, question6_status, question7_status, question8_status, question9_status, question10_status, approved_loan_amount, approved_term, approved_interest_rate, approved_maturity_date, approved_total_amount_due,nearest_landmark, tansfer_residence, spouse_name,spouse_occupation, number_of_dependent, mother_work, father_name, father_work, living_with_mother, sibling_count, sibling_works, occupation, scheduled_and_btc, net_income, pending_resignation, other_source_of_income, know_about_cashmart, pending_loan_fron_otherland, bank_loan_or_credit_card, permanent_address, bankid, family_name1, family_address1, family_contact1, family_relation1, family_name2, family_address2, family_contact2, family_relation2, optional_name, optional_address, optional_contact, optional_relation)VALUES('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}','{20}','{21}','{22}','{23}','{24}','{25}','{26}','{27}','{28}','{29}','{30}','{31}','{32}','{33}','{34}','{35}','{36}','{37}','{38}','{39}','{40}','{41}','{42}','{43}','{44}','{45}','{46}','{47}','{48}','{49}','{50}','{51}','{52}','{53}','{54}', '{55}', '{56}','{57}','{58}','{59}','{60}','{61}', '{62}','{63}','{64}', '{65}','{66}','{67}','{68}','{69}','{70}','{71}','{72}','{73}','{74}','{75}','{76}','{77}','{78}','{79}','{80}','{81}','{82}','{84}','{85}','{86}','{87}','{88}','{89}','{90}','{91}','{92}','{93}','{94}','{95}','{96}','{97}','{98}','{99}','{100}','{101}','{102}','{103}','{104}','{105}','{106}','{107}','{108}','{109}','{110}','{111}','{112}','{113}','{114}','{115}');", appNo, requestedAmt, requestedTerms, id1Status, id2Status, piStatus, pbStatus, remarks, addBirthPlace, addProvincialAdd, AdditionalLoanPurpose_Remark, addRequestedLoanAmount, addrequestedTerm, addEmpDuration, addJobPosition, addJobLevel, addPayDate, familyPersonalNameStatus, familyPersonalContactStatus, familyRelationWithBorrowerStatus, familyBorrowerKnownDurationStatus, familyAddVerificationStatus, familyBorrowerWorkingPlaceStatus, friendPersonalNameStatus, friendPersonalContactStatus, friendRelationWithBorrowerStatus, friendBorrowerKnownDurationStatus, friendAddVerificationStatus, friendBorrowerWorkingPlaceStatus, coworkerPersonalNameStatus, coworkerPersonalContactStatus, coworkerRelationWithBorrowerStatus, coworkerBorrowerKnownDurationStatus, coworkerAddVerificationStatus, coworkerBorrowerWorkingPlaceStatus, employmentNameOfWorkContactStatus, employmentNoOfWorkContactStatus, employmentBorrowerWorkingStatus, employmentBorrowerPositionStatus, employmentBorrowerMonthlySalaryStatus, employmentBorrowerAttendanceStatus, employmentBorrowerBankPayrollStatus, familyPersonalName, familyPersonalContact, familyRelationWithBorrower, familyBorrowerKnownDuration, familyAddVerification, familyBorrowerWorkingPlace, friendPersonalName, friendPersonalContact, friendRelationWithBorrower, friendBorrowerKnownDuration, friendAddVerification, friendBorrowerWorkingPlace, coworkerPersonalName, coworkerPersonalContact, coworkerRelationWithBorrower, coworkerBorrowerKnownDuration, coworkerAddVerification, coworkerBorrowerWorkingPlace, employmentNameOfWorkContact, employmentNoOfWorkContact, employmentBorrowerWorking, employmentBorrowerPosition, employmentBorrowerMonthlySalary, employmentBorrowerAttendance, employmentBorrowerBankPayroll, question1Status, question2Status, question3Status, question4Status, question5Status, question6Status, question7Status, question8Status, question9Status, question10Status, approvedLoanAmt, approvedTerm, approvedInterestRate, approvedMaturityDate, approvedTotalDueAmt, nearest_landmark, home_status, tansfer_residence, spouse_name, spouse_occupation, number_of_dependent, mother_work, father_name, father_work, living_with_mother, sibling_count, sibling_works, occupation, "", net_income, pending_resignation, other_source_of_income, know_about_cashmart, pending_loan_fron_otherland, bank_loan_or_credit_card, Permanent_address, BankId, family_name1, family_address1, family_contact1, family_relation1, family_name2, family_address2, family_contact2, family_relation2, optional_name, optional_address, optional_contact, optional_relation, Designation, user_id));
                        if (insert > 1)
                        {
                            int insertQuery = DbHelper.InsertUpdateDelete(string.Format("insert into tblapplicationdata_verification_details(application_no, personal_name_id1_status, personal_name_id2_status,personal_name_pi_status, personal_name_pb_status, personal_email_id1_status,personal_email_id2_status, personal_email_pi_status, personal_email_pb_status,personal_contact_no_id1_status, personal_contact_no_id2_status,personal_contact_no_pi_status, personal_contact_no_pb_status,personal_perma_address_id1_status, personal_perma_address_id2_status,personal_perma_address_pi_status, personal_perma_address_pb_status,personal_house_ph_id1_status, personal_house_ph_id2_status, personal_house_ph_pi_status,personal_house_ph_pb_status, personal_birth_place_id1_status,personal_birth_place_id2_status, personal_birth_place_pi_status,personal_birth_place_pb_status, personal_birth_date_id1_status,personal_birth_date_id2_status, personal_birth_date_pi_status,personal_birth_date_pb_status, personal_civil_id1_status, personal_civil_id2_status,personal_civil_pi_status, personal_civil_pb_status, personal_mother_maiden_name_id1_status,personal_mother_maiden_name_id2_status, personal_mother_maiden_name_pi_status,personal_mother_maiden_name_pb_status, personal_mother_perma_add_id1_status,personal_mother_perma_add_id2_status, personal_mother_perma_add_pi_status,personal_mother_perma_add_pb_status, personal_company_name_id1_status,personal_company_name_id2_status, personal_company_name_pi_status,personal_company_name_pb_status, personal_company_address_id1_status,personal_company_address_id2_status, personal_company_address_pi_status,personal_company_address_pb_status, personal_job_title_id1_status,personal_job_title_id2_status, personal_job_title_pi_status,personal_job_title_pb_status, personal_monthly_income_id1_status,personal_monthly_income_id2_status, personal_monthly_income_pi_status,personal_monthly_income_pb_status, personal_reference_name_id1_status,personal_reference_name_id2_status, personal_reference_name_pi_status,personal_reference_name_pb_status, personal_reference_contact_id1_status,personal_reference_contact_id2_status, personal_reference_contact_pi_status,personal_reference_contact_pb_status, personal_bank_name_id1_status,personal_bank_name_id2_status, personal_bank_name_pi_status,personal_bank_name_pb_status, personal_bank_ac_no_id1_status,personal_bank_ac_no_id2_status, personal_bank_ac_no_pi_status,personal_bank_ac_no_pb_status, personal_req_loan_amt_id1_status,personal_req_loan_amt_id2_status, personal_req_loan_amt_pi_status,personal_req_loan_amt_pb_status, personal_req_loan_term_id1_status,personal_req_loan_term_id2_status, personal_req_loan_term_pi_status,personal_req_loan_term_pb_status, personal_name_remark, personal_email_remark,personal_contact_no_remark, personal_perma_address_remark, personal_house_ph_remark,personal_birth_place_remark, personal_birth_date_remark, personal_civil_remark,personal_mother_maiden_name_remark, personal_mother_perma_add_remark,personal_company_name_remark, personal_company_address_remark,personal_job_title_remark, personal_monthly_income_remark, personal_reference_name_remark,personal_reference_contact_remark, personal_bank_name_remark,personal_bank_ac_no_remark, personal_req_loan_amt_remark, personal_req_loan_term_remark, nearest_landmark_remark, sss_no_remark, rented_mortgage_owned_remark, permanent_address_remark, tansfer_residence_remark, spouse_name_remark, spouse_occupation_remark, number_of_dependent_remark, father_name_work_remark, sibling_works_remark, occupation_remark, net_income_remark, scheduled_and_btc_remark, pay_date_remark, pending_resignation_remark, other_source_of_income_remark, know_about_cashmart_remark, pending_loan_fron_otherland_remark, bank_loan_or_credit_card_remark, family_name1_remark, family_address1_remark, family_contact1_remark, family_relation1_remark, family_name2_remark, family_address2_remark, family_contact2_remark, family_relation2_remark, optional_name_remark, optional_address_remark, optional_contact_remark, optional_relation_remark)VALUES('{0}','{1}', '{2}','{3}','{4}', '{5}', '{6}', '{7}', '{8}','{9}', '{10}', '{11}', '{12}', '{13}', '{14}','{15}','{16}','{17}','{18}','{19}','{20}','{21}','{22}','{23}','{24}','{25}','{26}','{27}','{28}','{29}','{30}','{31}','{32}','{33}','{34}','{35}','{36}','{37}','{38}','{39}','{40}', '{41}', '{42}','{43}','{44}','{45}', '{46}','{47}', '{48}','{49}','{50}', '{51}','{52}','{53}','{54}','{55}','{56}','{57}','{58}', '{59}','{60}','{61}','{62}','{63}','{64}','{65}','{66}','{67}','{68}','{69}','{70}','{71}','{72}','{73}','{74}','{75}','{76}','{77}', '{78}','{79}','{80}','{81}','{82}','{83}','{84}','{85}','{86}','{87}','{88}','{89}','{90}','{91}','{92}','{93}','{94}','{95}','{96}','{97}','{98}','{99}','{100}','{101}','{102}','{103}','{104}','{105}','{106}','{107}','{108}','{109}','{110}','{111}','{112}','{113}','{114}','{115}','{116}','{117}','{118}','{119}','{120}','{121}','{122}','{123}','{124}','{125}','{126}','{127}','{128}','{129}','{130}','{131}');Update tblapplication_record Set isverified=true, verify_by='{132}', rejectedbyverifier=false,rejectedbyapprover=false where applicationno={0};", appNo, personalNameId1Status, personalNameId2Status, personalNamePiStatus, personalNamePbStatus, personalEmailId1Status, personalEmailId2Status, personalEmailPiStatus, personalEmailPbStatus, personalContactId1Status, personalContactId2Status, personalContactPiStatus, personalContactPbStatus, personalPerAddId1Status, personalPerAddId2Status, personalPerAddPiStatus, personalPerAddPbStatus, personalHousePhId1Status, personalHousePhId2Status, personalHousePhPiStatus, personalHousePhPbStatus, personalBirthPlaceId1Status, personalBirthPlaceId2Status, personalBirthPlacePiStatus, personalBirthPlacePbStatus, personalBirthDateId1Status, personalBirthDateId2Status, personalBirthDatePiStatus, personalBirthDatePbStatus, personalCivilId1Status, personalCivilId2Status, personalCivilPiStatus, personalCivilPbStatus, personalMotherMaidenNameId1Status, personalMotherMaidenNameId2Status, personalMotherMaidenNamePiStatus, personalMotherMaidenNamePbStatus, personalMotherPerAddId1Status, personalMotherPerAddId2Status, personalMotherPerAddPiStatus, personalMotherPerAddPbStatus, personalCompanyNameId1Status, personalCompanyNameId2Status, personalCompanyNamePiStatus, personalCompanyNamePbStatus, personalCompanyAddId1Status, personalCompanyAddId2Status, personalCompanyAddPiStatus, personalCompanyAddPbStatus, personalJobTitleId1Status, personalJobTitleId2Status, personalJobTitlePiStatus, personalJobTitlePbStatus, personalMonthlyIncomeId1Status, personalMonthlyIncomeId2Status, personalMonthlyIncomePiStatus, personalMonthlyIncomePbStatus, personalRefNameId1Status, personalRefNameId2Status, personalRefNamePiStatus, personalRefNamePbStatus, personalRefContactId1Status, personalRefContactId2Status, personalRefContactPiStatus, personalRefContactPbStatus, personalBankNameId1Status, personalBankNameId2Status, personalBankNamePiStatus, personalBankNamePbStatus, personalBankAccNoId1Status, personalBankAccNoId2Status, personalBankAccNoPiStatus, personalBankAccNoPbStatus, personalReqLoanAmtId1Status, personalReqLoanAmtId2Status, personalReqLoanAmtPiStatus, personalReqLoanAmtPbStatus, personalReqLoanTermId1Status, personalReqLoanTermId2Status, personalReqLoanTermPiStatus, personalReqLoanTermPbStatus, personalNameRemark, personalEmailRemark, persoanlContactRemark, persoanlPerAddRemark, personalHousePhRemark, personalBirthPlaceRemark, personalBirthDateRemark, personalCivilRemark, personalMotherMaidenNameRemark, personalMotherPerAddRemark, personalCompanyNameRemark, personalCompanyAddRemark, personalJobTitleremark, personalMonthlyIncomeRemark, personalRefNameRemark, personalRefContactRemark, personalBankNameRemark, personalBankAccNoRemark, personalReqLoanAmtRemark, personalReqLoanTermRemark, nearest_landmark_remark, SSS_No_remark, rented_mortgage_owned_remark, Permanent_address_remark, tansfer_residence_remark, spouse_name_remark, spouse_occupation_remark, number_of_dependent_remark, father_name_work_remark, sibling_works_remark, occupation_remark, net_income_remark, scheduled_and_btc_remark, pay_date_remark, pending_resignation_remark, other_source_of_income_remark, know_about_cashmart_remark, pending_loan_fron_otherland_remark, bank_loan_or_credit_card_remark, family_name1_remark, family_address1_remark, family_contact1_remark, family_relation1_remark, family_name2_remark, family_address2_remark, family_contact2_remark, family_relation2_remark, optional_name_remark, optional_address_remark, optional_contact_remark, optional_relation_remark, user_id));
                            if (insertQuery > 0)
                            {
                                logger.WriteErrorLogs($"Term Name:{TermName} ===>  Term Type:{Term}", string.Format("{0} ==> {1}", this.GetType().Name, MethodBase.GetCurrentMethod().Name));
                                if (TermName != 0 && Term != 0)
                                {
                                    var updateQuery1 = DbHelper.InsertUpdateDelete($"update tblapplication_record set address='{address}', homephoneno='{HomePhone}', mothermaidenname='{MotherMaidenName}', motheraddress='{MotherPermanentAddress}', companyname='{companyName}', companyaddress='{compnayaddress}', gross_income='{GrossMonthlyIncome}', reference_name='{refereneceName}', reference_contactno='{ReferenceContactNo}', bankname='{model.BankName}', bankaccountno='{bankAcctNo}', term='{TermName}', termtype='{Term}', isverified=true, isrecheck=false, verifiedon=Current_Timestamp, ispickedverifier='false', ispickedreverifier=false, rejectedbyverifier=false, rejectedbyapprover=false where applicationno='{appNo}'");
                                    TempData["Result"] = "Data Saved Successfully.";
                                }
                                else
                                {
                                    TempData["Result"] = "Check the Term Values.";
                                }
                            }

                        }
                    }

                    string FileName = CommonMethods.GetCashMartRefNo(Convert.ToString(appNo));
                    CommonMethods.CreateCreditAgreementPdf(Convert.ToString(HttpContext.Request.Cookies[CookiesKey.AgentName].Value), model.Name, $"{model.Address} {model.completeaddress}", model.ApprovedLoanAmount, Convert.ToInt32(model.ApprovedTerm), Convert.ToDouble(model.ApprovedInterestRate), model.AdditionalLoanPurpose, FileName, appinformation.user_id, ApprovedTermType, Convert.ToInt32(approvedTerm), appNo, Penalty_Rate, Admin_Fee, _Tenure, _Late_fee, model.BankName, model.BankAccNo);
                    string sendby = Convert.ToString(HttpContext.Request.Cookies[CookiesKey.AgentName].Value);
                    string query1 = string.Format(QueryHelper.Insertapplication_contract_details, appNo, FileName, sendby, model.PersonalEmail);
                    int res = DbHelper.InsertUpdateDelete(query1);
                    if (res > 0)
                    {
                        string folderName = "../../Content/PDF/";
                        if (!Directory.Exists(Server.MapPath(folderName)))
                        {
                            Directory.CreateDirectory(Server.MapPath(folderName));
                        }
                        folderName = Server.MapPath(folderName + FileName + ".pdf");
                        String _ContractUrl = CommonMethods.UploadContractPDF(Convert.ToString(appNo), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), $"{FileName}.pdf");

                        //if (System.IO.File.Exists(folderName))
                        //{
                        //    System.IO.File.Delete(folderName);
                        //}

                        ApplicationRecordVM _obj = new ApplicationRecordVM();
                        DataTable _dt;
                        _dt = DbHelper.SelectMethod(string.Format(QueryHelper.GetClientName, appNo));
                        string FullName = string.Empty;
                        if (_dt != null && _dt.Rows.Count > 0)
                        {
                            _obj.First_Name = Convert.ToString(_dt.Rows[0]["first_name"]);
                            _obj.Middle_Name = Convert.ToString(_dt.Rows[0]["middle_name"]);
                            _obj.Last_Name = Convert.ToString(_dt.Rows[0]["last_name"]);
                            _obj.PersonalContactNo = Convert.ToString(_dt.Rows[0]["personalcontactno"]);
                            _obj.PersonalEmail = Convert.ToString(_dt.Rows[0]["personalemail"]);
                            FullName = _obj.First_Name + " " + _obj.Middle_Name + " " + _obj.Last_Name;
                        }
                        if (!String.IsNullOrWhiteSpace(Convert.ToString(FullName)) && !String.IsNullOrWhiteSpace(_obj.PersonalEmail))
                        {
                            StringBuilder EmailBody = new StringBuilder();
                            EmailBody.AppendLine("<p>Congratulations Mr./Ms. " + FullName + "!</p><p>Upang madeposit ang cash sa iyong bank account, mangyaring sundin lamang ang lahat ng mga instruction sa ibaba para sa pinakamabilis na pagproseso ng iyong loan.</p><p>1. Buksan ang iyong Cash Mart Mobile App <br/>2. Pindutin ang My Loan<br/>3. I-click ang \"Sign Contract\"<br/>4. Basahin ang Disclosure statement, Loan Agreement, at terms and conditions<br/>5. Pindutin ang dalawang maliit na box at i-click ang \"Done\"<br/>6. Siguraduhing ang pirma ay hawig sa iyong pirma sa government ID<br/></p><p>Hintayin ang aming email or text para sa confirmation na ang iyong cash ay nadisburse na.</p><p>Ang iyong loan disbursement ay aming pinoprosesa within 24 working hours after namin marecieve ang inyong pirma.</p><p>Please take note na ang aming disbursement ay mula Monday to Friday, 8am to 6pm lamang.</p><p>Thank you for choosing Cash Mart.</p>");

                            new Thread(() => CommonMethods.SendMail(EmailFrom.FromAccount, _obj.PersonalEmail, Convert.ToString(EmailBody), "Loan Contract", folderName)).Start();
                            string SMSBody = String.Format(Convert.ToString(ConfigurationManager.AppSettings["ContractSMSBody"]), _obj.First_Name);
                            new Thread(() => logger.ClicktoSMS(_obj.PersonalContactNo, SMSBody)).Start();
                        }
                        else
                        {
                            logger.WriteErrorLogs($"Contract Email not Sent : Full Name {FullName} & Email {_obj.PersonalEmail}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                        }
                    }

                }

            }
            catch (Exception ex)
            {
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
            }
            return RedirectToAction(ActionName);
        }

        public ActionResult UpdateReVerifyRequestDetail(int Id, int user_id, string Actionname)
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.Verifier))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            try
            {
                ActivityLog.Info($"Application {Id} Declined  on Re-Verifer Window by User {Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value)}", $"{this.GetType().Name} ==> {System.Reflection.MethodBase.GetCurrentMethod().Name}");
                logger.AddWelcomeMessage(History.Application, "", "Your loan has Been Declined. You may reapply after 3 months.", 0, true, Id);
                DeclineClick(Id.ToString());
                var UserLoginType = DbHelper.SelectMethod($"SELECT ctr.rolename FROM public.tblapplication_record tar join public.ct_user ctu on tar.checked_by=ctu.userid join ct_roles ctr on ctu.webadminrole=ctr.id and tar.applicationno={Id};");
                if (UserLoginType != null && UserLoginType.Rows.Count > 0)
                {
                    if (Convert.ToString(UserLoginType.Rows[0]["rolename"]) == "Adminuser")
                    {
                        var query = DbHelper.InsertUpdateDelete(string.Format(QueryHelper.UpdateReCheckAdmin, DateTime.Now, Id));
                    }
                    else
                    {
                        var query = DbHelper.InsertUpdateDelete(string.Format(QueryHelper.UpdateReCheck, DateTime.Now, Id));
                    }
                }
            }
            catch (Exception ex)
            {
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                throw ex;
            }
            return RedirectToAction(Actionname);
        }

        public bool NextClick(string id)
        {

            var result = false;
            int intAppId = 0;
            var currentDate = DateTime.Now;
            if (!string.IsNullOrEmpty(id))
            {
                intAppId = Convert.ToInt32(id);
            }
            try
            {
                var query = DbHelper.InsertUpdateDelete(string.Format(QueryHelper.UpdatIsverified, currentDate, intAppId, Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value)));
                if (query > 0)
                {
                    result = true;
                }
            }
            catch (Exception ex)
            {
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                throw ex;
            }
            return result;
        }

        public int ClicktoCall(string ContactNumber)
        {
            try
            {
                ActivityLog.Info($"Call {ContactNumber} on Verifer Window by User {Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value)}", $"{this.GetType().Name} ==> {System.Reflection.MethodBase.GetCurrentMethod().Name}");
                return logger.ClicktoCall(ContactNumber, Convert.ToString(HttpContext.Request.Cookies[CookiesKey.AgentName].Value), Convert.ToString(Session["IPAddress"]));
            }
            catch (Exception ex)
            {
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                throw ex;
            }

        }

        public int ClicktoCallOptionalContact(string OptionalContactNumber)
        {
            try
            {
                ActivityLog.Info($"Call {OptionalContactNumber} on Verifer Window by User {Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value)}", $"{this.GetType().Name} ==> {System.Reflection.MethodBase.GetCurrentMethod().Name}");
                return logger.ClicktoCall(OptionalContactNumber, Convert.ToString(HttpContext.Request.Cookies[CookiesKey.AgentName].Value), Convert.ToString(Session["IPAddress"]));
            }
            catch (Exception ex)
            {
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                throw ex;
            }

        }

        public int ClicktoCallFamilyOneContact(string FamilyOneContactNumber)
        {
            try
            {
                ActivityLog.Info($"Call {FamilyOneContactNumber} on Verifer Window by User {Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value)}", $"{this.GetType().Name} ==> {System.Reflection.MethodBase.GetCurrentMethod().Name}");
                return logger.ClicktoCall(FamilyOneContactNumber, Convert.ToString(HttpContext.Request.Cookies[CookiesKey.AgentName].Value), Convert.ToString(Session["IPAddress"]));
            }
            catch (Exception ex)
            {
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                throw ex;
            }

        }

        public int ClicktoCallFamilyTwoContact(string FamilyTwoContactNumber)
        {
            try
            {
                ActivityLog.Info($"Call {FamilyTwoContactNumber} on Verifer Window by User {Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value)}", $"{this.GetType().Name} ==> {System.Reflection.MethodBase.GetCurrentMethod().Name}");
                return logger.ClicktoCall(FamilyTwoContactNumber, Convert.ToString(HttpContext.Request.Cookies[CookiesKey.AgentName].Value), Convert.ToString(Session["IPAddress"]));
            }
            catch (Exception ex)
            {
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult GetTermName(int Amount)
        {
            List<Terms> termVM = new List<Terms>();
            try
            {
                DataTable dt = new DataTable();
                dt = DbHelper.SelectMethod(String.Format(QueryHelper.GetTermName, Convert.ToDecimal(Amount) > 10000 ? "true" : "false"));
                if (dt != null && dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        termVM.Add(new Terms
                        {
                            Id = Convert.ToInt32(dt.Rows[i]["id"]),
                            term_Id = Convert.ToInt32(dt.Rows[i]["term_id"]),
                            term_Name = dt.Rows[i]["term_name"].ToString(),
                            term_Value = dt.Rows[i]["term_value"].ToString(),
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                return Json(null);
            }
            return Json(termVM);
        }
        [HttpPost]
        public ActionResult Get_TermName()
        {
           
            List<Terms> termVM = new List<Terms>();
            try
            {
                DataTable dt = new DataTable();
                dt = DbHelper.SelectMethod(String.Format(QueryHelper.Get_TermName));
                if (dt != null && dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        termVM.Add(new Terms
                        {
                            Id = Convert.ToInt32(dt.Rows[i]["id"]),
                            term_Id = Convert.ToInt32(dt.Rows[i]["term_id"]),
                            term_Name = dt.Rows[i]["term_name"].ToString(),
                            term_Value = dt.Rows[i]["term_value"].ToString(),
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                return Json(null);
            }
            return Json(termVM);
        }

        public ActionResult GetTermType()
        {
           
            List<TermType> termTypeVM = new List<TermType>();
            try
            {
                DataTable dt = new DataTable();
                dt = DbHelper.SelectMethod(QueryHelper.GetTermType);
                if (dt != null && dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        termTypeVM.Add(new TermType
                        {
                            Id = Convert.ToInt32(dt.Rows[i]["id"]),
                            term_Type = dt.Rows[i]["termtype"].ToString()
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                return Json(null);
            }
            return Json(termTypeVM);
        }

        public bool AutoSave(CompleteAppVerificationDetails model)
        {
            var result = false;
            try
            {
                ActivityLog.Info($"Application {model.ApplicationNo} Auto Saved on Verifer Window by User {Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value)}", $"{this.GetType().Name} ==> {System.Reflection.MethodBase.GetCurrentMethod().Name}");
                #region PersonalVerificationDetails
                var appNo = Convert.ToInt32(model.ApplicationNo);
                var requestedAmt = model.RequestedAmt;
                var requestedTerms = model.RequestedTerms;
                var id1Status = model.Id1_Status;
                var id2Status = model.Id2_Status;
                var piStatus = model.Pi_Status;
                var pbStatus = model.Pb_Status;
                var bankName = model.BankName;

                var address = RemoveSpecialCharacters(model.Address);
                var companyName = RemoveSpecialCharacters(model.CompanyName);
                var compnayaddress = RemoveSpecialCharacters(model.CompanyAddress);
                var designation = RemoveSpecialCharacters(model.Designation);
                var MotherMaidenName = RemoveSpecialCharacters(model.MotherMaidenName);
                var MotherPermanentAddress = RemoveSpecialCharacters(model.MotherAddress);
                var GrossMonthlyIncome = model.GrossIncome;
                var refereneceName = RemoveSpecialCharacters(model.RefName);
                var ReferenceContactNo = RemoveSpecialCharacters(model.RefContactNo);
                var bankAcctNo = RemoveSpecialCharacters(model.BankAccNo);
                var HomePhone = RemoveSpecialCharacters(model.HomePhone);

                var remarks = RemoveSpecialCharacters(model.Remarks);
                var addBirthPlace = RemoveSpecialCharacters(model.AdditionalBirthPlace);
                var addProvincialAdd = RemoveSpecialCharacters(model.AdditionalProvincialAddress);
                var addLoanPurpose = RemoveSpecialCharacters(model.AdditionalLoanPurpose);
                var addRequestedLoanAmount = model.AdditionalRequestedLoanAmount;
                var addrequestedTerm = model.AdditionalRequestedTerm;
                var addEmpDuration = RemoveSpecialCharacters(model.AdditionalEmployedDuration);
                if (String.IsNullOrWhiteSpace(addEmpDuration))
                {
                    addEmpDuration = "0";
                }
                var addJobPosition = RemoveSpecialCharacters(model.AdditionalJobPosition);
                var addJobLevel = RemoveSpecialCharacters(model.AdditionalJobLevel);
                var addPayDate = RemoveSpecialCharacters(model.AdditionalPayDate);
                var familyPersonalNameStatus = model.FamilyPersonalNameStatus;
                var familyPersonalContactStatus = model.FamilyPersonalContactStatus;
                var familyRelationWithBorrowerStatus = model.FamilyRelationWithBorrowerStatus;
                var familyBorrowerKnownDurationStatus = model.FamilyBorrowerKnownDurationStatus;
                var familyAddVerificationStatus = model.FamilyAddressVerificationStatus;
                var familyBorrowerWorkingPlaceStatus = model.FamilyBorrowerWorkingPlaceStatus;
                var friendPersonalNameStatus = model.FriendPersonalNameStatus;
                var friendPersonalContactStatus = model.FriendPersonalContactStatus;
                var friendRelationWithBorrowerStatus = model.FriendRelationWithBorrowerStatus;
                var friendBorrowerKnownDurationStatus = model.FriendBorrowerKnownDurationStatus;
                var friendAddVerificationStatus = model.FriendAddressVerificationStatus;
                var friendBorrowerWorkingPlaceStatus = model.FriendBorrowerWorkingPlaceStatus;
                var coworkerPersonalNameStatus = model.CoWorkerPersonalNameStatus;
                var coworkerPersonalContactStatus = model.CoWorkerPersonalContactStatus;
                var coworkerRelationWithBorrowerStatus = model.CoWorkerRelationWithBorrowerStatus;
                var coworkerBorrowerKnownDurationStatus = model.CoWorkerBorrowerKnownDurationStatus;
                var coworkerAddVerificationStatus = model.CoWorkerAddressVerificationStatus;
                var coworkerBorrowerWorkingPlaceStatus = model.CoWorkerBorrowerWorkingPlaceStatus;
                var employmentNameOfWorkContactStatus = model.EmploymentNameOfWorkContactStatus;
                var employmentNoOfWorkContactStatus = model.EmploymentNoOfWorkConatctStatus;
                var employmentBorrowerWorkingStatus = model.EmploymentBorrowerWorkingStatus;
                var employmentBorrowerPositionStatus = model.EmploymentBorrowerPositionStatus;
                var employmentBorrowerMonthlySalaryStatus = model.EmploymentBorrowerMonthlySalaryStatus;
                var employmentBorrowerAttendanceStatus = model.EmploymentBorrowerAttendanceStatus;
                var employmentBorrowerBankPayrollStatus = model.EmploymentBorrowerBankPayrollStatus;
                var familyPersonalName = RemoveSpecialCharacters(model.FamilyPersonalName);
                var familyPersonalContact = RemoveSpecialCharacters(model.FamilyPersonalContact);
                var familyRelationWithBorrower = RemoveSpecialCharacters(model.FamilyRelationWithBorrower);
                var familyBorrowerKnownDuration = RemoveSpecialCharacters(model.FamilyBorrowerKnownDuration);
                var familyAddVerification = RemoveSpecialCharacters(model.FamilyAddressVerification);
                var familyBorrowerWorkingPlace = RemoveSpecialCharacters(model.FamilyBorrowerWorkingPlace);
                var friendPersonalName = RemoveSpecialCharacters(model.FriendPersonalName);
                var friendPersonalContact = RemoveSpecialCharacters(model.FriendPersonalContact);
                var friendRelationWithBorrower = RemoveSpecialCharacters(model.FriendRelationWithBorrower);
                var friendBorrowerKnownDuration = RemoveSpecialCharacters(model.FriendBorrowerKnownDuration);
                var friendAddVerification = RemoveSpecialCharacters(model.FriendAddressVerification);
                var friendBorrowerWorkingPlace = RemoveSpecialCharacters(model.FriendBorrowerWorkingPlace);
                var coworkerPersonalName = RemoveSpecialCharacters(model.CoWorkerPersonalName);
                var coworkerPersonalContact = RemoveSpecialCharacters(model.CoWorkerPersonalConatact);
                var coworkerRelationWithBorrower = RemoveSpecialCharacters(model.CoWorkerRelationWithBorrower);
                var coworkerBorrowerKnownDuration = RemoveSpecialCharacters(model.CoWorkerBorrowerKnownDuration);
                var coworkerAddVerification = RemoveSpecialCharacters(model.CoWorkerAddressVerification);
                var coworkerBorrowerWorkingPlace = RemoveSpecialCharacters(model.CoWorkerBorrowerWorkingPlace);
                var employmentNameOfWorkContact = RemoveSpecialCharacters(model.EmploymentNameOfWorkContact);
                var employmentNoOfWorkContact = RemoveSpecialCharacters(model.EmploymentNoOfWorkConatct);
                var employmentBorrowerWorking = RemoveSpecialCharacters(model.EmploymentBorrowerWorking);
                var employmentBorrowerPosition = RemoveSpecialCharacters(model.EmploymentBorrowerPosition);
                var employmentBorrowerMonthlySalary = RemoveSpecialCharacters(model.EmploymentBorrowerMonthlySalary);
                var employmentBorrowerAttendance = RemoveSpecialCharacters(model.EmploymentBorrowerAttendance);
                var employmentBorrowerBankPayroll = RemoveSpecialCharacters(model.EmploymentBorrowerBankPayroll);
                var question1Status = model.Question1_Status;
                var question2Status = model.Question2_Status;
                var question3Status = model.Question3_Status;
                var question4Status = model.Question4_Status;
                var question5Status = model.Question5_Status;
                var question6Status = model.Question6_Status;
                var question7Status = model.Question7_Status;
                var question8Status = model.Question8_Status;
                var question9Status = model.Question9_Status;
                var question10Status = model.Question10_Status;
                var approvedLoanAmt = model.ApprovedLoanAmount;
                var approvedTerm = model.ApprovedTerm;
                var TermName = model.TermName;
                string ApprovedTermType = model.ApprovedTermType;
                int Term = 0;
                if (ApprovedTermType == "Weekly")
                {
                    Term = 1;
                }
                if (ApprovedTermType == "Bi-Weekly")
                {
                    Term = 2;
                }
                if (ApprovedTermType == "Monthly")
                {
                    Term = 3;
                }

                var approvedInterestRate = model.ApprovedInterestRate;
                var approvedMaturityDate = model.ApprovedMaturityDate;

                if (!String.IsNullOrWhiteSpace(approvedMaturityDate))
                {
                    string[] ar = approvedMaturityDate.Split('-');
                    string mdStr = string.Empty;
                    mdStr = Convert.ToString(ar[2]) + "-" + Convert.ToString(ar[1]) + "-" + Convert.ToString(ar[0]);
                    approvedMaturityDate = mdStr;
                }
                else
                {
                    DateTime maturityDate = DateTime.Now;

                    string twoDigitDay = Convert.ToString(maturityDate.Day);
                    if (maturityDate.Day < 10)
                        twoDigitDay = "0" + twoDigitDay;

                    string twoDigitMonth = Convert.ToString(maturityDate.Month);
                    if (maturityDate.Month < 10)
                        twoDigitMonth = "0" + twoDigitMonth;

                    string strDate = string.Empty;
                    strDate = maturityDate.Year + "-" + twoDigitMonth + "-" + twoDigitDay;
                    approvedMaturityDate = strDate;
                }
                var approvedTotalDueAmt = model.ApprovedTotalDueAmount;
                #endregion

                #region AppDataVerificationDetails
                var personalNameId1Status = model.personal_name_id1_status;
                var personalNameId2Status = model.personal_name_id2_status;
                var personalNamePiStatus = model.personal_name_pi_status;
                var personalNamePbStatus = model.personal_name_pb_status;
                var personalEmailId1Status = model.personal_email_id1_status;
                var personalEmailId2Status = model.personal_email_id2_status;
                var personalEmailPiStatus = model.personal_email_pi_status;
                var personalEmailPbStatus = model.personal_email_pb_status;
                var personalContactId1Status = model.personal_contact_no_id1_status;
                var personalContactId2Status = model.personal_contact_no_id2_status;
                var personalContactPiStatus = model.personal_contact_no_pi_status;
                var personalContactPbStatus = model.personal_contact_no_pb_status;
                var personalPerAddId1Status = model.personal_perma_address_id1_status;
                var personalPerAddId2Status = model.personal_perma_address_id2_status;
                var personalPerAddPiStatus = model.personal_perma_address_pi_status;
                var personalPerAddPbStatus = model.personal_perma_address_pb_status;
                var personalHousePhId1Status = model.personal_house_ph_id1_status;
                var personalHousePhId2Status = model.personal_house_ph_id2_status;
                var personalHousePhPiStatus = model.personal_house_ph_pi_status;
                var personalHousePhPbStatus = model.personal_house_ph_pb_status;
                var personalBirthPlaceId1Status = model.personal_birth_place_id1_status;
                var personalBirthPlaceId2Status = model.personal_birth_place_id2_status;
                var personalBirthPlacePiStatus = model.personal_birth_place_pi_status;
                var personalBirthPlacePbStatus = model.personal_birth_place_pb_status;
                var personalBirthDateId1Status = model.personal_birth_date_id1_status;
                var personalBirthDateId2Status = model.personal_birth_date_id2_status;
                var personalBirthDatePiStatus = model.personal_birth_date_pi_status;
                var personalBirthDatePbStatus = model.personal_birth_date_pb_status;
                var personalCivilId1Status = model.personal_civil_id1_status;
                var personalCivilId2Status = model.personal_civil_id2_status;
                var personalCivilPiStatus = model.personal_civil_pi_status;
                var personalCivilPbStatus = model.personal_civil_pb_status;
                var personalMotherMaidenNameId1Status = model.personal_mother_maiden_name_id1_status;
                var personalMotherMaidenNameId2Status = model.personal_mother_maiden_name_id2_status;
                var personalMotherMaidenNamePiStatus = model.personal_mother_maiden_name_pi_status;
                var personalMotherMaidenNamePbStatus = model.personal_mother_maiden_name_pb_status;
                var personalMotherPerAddId1Status = model.personal_mother_perma_add_id1_status;
                var personalMotherPerAddId2Status = model.personal_mother_perma_add_id2_status;
                var personalMotherPerAddPiStatus = model.personal_mother_perma_add_pi_status;
                var personalMotherPerAddPbStatus = model.personal_mother_perma_add_pb_status;
                var personalCompanyNameId1Status = model.personal_company_name_id1_status;
                var personalCompanyNameId2Status = model.personal_company_name_id2_status;
                var personalCompanyNamePiStatus = model.personal_company_name_pi_status;
                var personalCompanyNamePbStatus = model.personal_company_name_pb_status;
                var personalCompanyAddId1Status = model.personal_company_address_id1_status;
                var personalCompanyAddId2Status = model.personal_company_address_id2_status;
                var personalCompanyAddPiStatus = model.personal_company_address_pi_status;
                var personalCompanyAddPbStatus = model.personal_company_address_pb_status;
                var personalJobTitleId1Status = model.personal_job_title_id1_status;
                var personalJobTitleId2Status = model.personal_job_title_id2_status;
                var personalJobTitlePiStatus = model.personal_job_title_pi_status;
                var personalJobTitlePbStatus = model.personal_job_title_pb_status;
                var personalMonthlyIncomeId1Status = model.personal_monthly_income_id1_status;
                var personalMonthlyIncomeId2Status = model.personal_monthly_income_id2_status;
                var personalMonthlyIncomePiStatus = model.personal_monthly_income_pi_status;
                var personalMonthlyIncomePbStatus = model.personal_monthly_income_pb_status;
                var personalRefNameId1Status = model.personal_reference_name_id1_status;
                var personalRefNameId2Status = model.personal_reference_name_id2_status;
                var personalRefNamePiStatus = model.personal_reference_name_pi_status;
                var personalRefNamePbStatus = model.personal_reference_name_pb_status;
                var personalRefContactId1Status = model.personal_reference_contact_id1_status;
                var personalRefContactId2Status = model.personal_reference_contact_id2_status;
                var personalRefContactPiStatus = model.personal_reference_contact_pi_status;
                var personalRefContactPbStatus = model.personal_reference_contact_pb_status;
                var personalBankNameId1Status = model.personal_bank_name_id1_status;
                var personalBankNameId2Status = model.personal_bank_name_id2_status;
                var personalBankNamePiStatus = model.personal_bank_name_pi_status;
                var personalBankNamePbStatus = model.personal_bank_name_pb_status;
                var personalBankAccNoId1Status = model.personal_bank_ac_no_id1_status;
                var personalBankAccNoId2Status = model.personal_bank_ac_no_id2_status;
                var personalBankAccNoPiStatus = model.personal_bank_ac_no_pi_status;
                var personalBankAccNoPbStatus = model.personal_bank_ac_no_pb_status;
                var personalReqLoanAmtId1Status = model.personal_req_loan_amt_id1_status;
                var personalReqLoanAmtId2Status = model.personal_req_loan_amt_id2_status;
                var personalReqLoanAmtPiStatus = model.personal_req_loan_amt_pi_status;
                var personalReqLoanAmtPbStatus = model.personal_req_loan_amt_pb_status;
                var personalReqLoanTermId1Status = model.personal_req_loan_term_id1_status;
                var personalReqLoanTermId2Status = model.personal_req_loan_term_id2_status;
                var personalReqLoanTermPiStatus = model.personal_req_loan_term_pi_status;
                var personalReqLoanTermPbStatus = model.personal_req_loan_term_pb_status;
                var personalNameRemark = RemoveSpecialCharacters(model.personal_name_remark);
                var personalEmailRemark = RemoveSpecialCharacters(model.personal_email_remark);
                var persoanlContactRemark = RemoveSpecialCharacters(model.personal_contact_no_remark);
                var persoanlPerAddRemark = RemoveSpecialCharacters(model.personal_perma_address_remark);
                var personalHousePhRemark = RemoveSpecialCharacters(model.personal_house_ph_remark);
                var personalBirthPlaceRemark = RemoveSpecialCharacters(model.personal_birth_place_remark);
                var personalBirthDateRemark = RemoveSpecialCharacters(model.personal_birth_date_remark);
                var personalCivilRemark = RemoveSpecialCharacters(model.personal_civil_remark);
                var personalMotherMaidenNameRemark = RemoveSpecialCharacters(model.personal_mother_maiden_name_remark);
                var personalMotherPerAddRemark = RemoveSpecialCharacters(model.personal_mother_perma_add_remark);
                var personalCompanyNameRemark = RemoveSpecialCharacters(model.personal_company_name_remark);
                var personalCompanyAddRemark = RemoveSpecialCharacters(model.personal_company_address_remark);
                var personalJobTitleremark = RemoveSpecialCharacters(model.personal_job_title_remark);
                var personalMonthlyIncomeRemark = RemoveSpecialCharacters(model.personal_monthly_income_remark);
                var personalRefNameRemark = RemoveSpecialCharacters(model.personal_reference_name_remark);
                var personalRefContactRemark = RemoveSpecialCharacters(model.personal_reference_contact_remark);
                var personalBankNameRemark = RemoveSpecialCharacters(model.personal_bank_name_remark);
                var personalBankAccNoRemark = RemoveSpecialCharacters(model.personal_bank_ac_no_remark);
                var personalReqLoanAmtRemark = RemoveSpecialCharacters(model.personal_req_loan_amt_remark);
                var personalReqLoanTermRemark = RemoveSpecialCharacters(model.personal_req_loan_term_remark);
                #endregion

                #region NewField Changes-Phase3
                var AdditionalLoanPurpose_Remark = RemoveSpecialCharacters(model.AdditionalLoanPurpose_Remark);
                var nearest_landmark = RemoveSpecialCharacters(model.nearest_landmark);
                var PayDate = RemoveSpecialCharacters(model.payDate1);
                var home_status = model.home_status;
                var Permanent_address = RemoveSpecialCharacters(model.Permanent_address);
                var tansfer_residence = RemoveSpecialCharacters(model.tansfer_residence);
                var spouse_name = RemoveSpecialCharacters(model.spouse_name);
                var spouse_occupation = RemoveSpecialCharacters(model.spouse_occupation);
                var number_of_dependent = model.number_of_dependent;
                var mother_work = RemoveSpecialCharacters(model.mother_work);
                var father_name = RemoveSpecialCharacters(model.father_name);
                var father_work = RemoveSpecialCharacters(model.father_work);
                var living_with_mother = RemoveSpecialCharacters(model.living_with_mother);
                var sibling_count = model.sibling_count;
                var sibling_works = RemoveSpecialCharacters(model.sibling_works);
                var occupation = String.IsNullOrWhiteSpace(model.occupation) == true ? "0" : RemoveSpecialCharacters(model.occupation);
                var net_income = model.net_income;
                var scheduled_and_btc = "";
                var pending_resignation = RemoveSpecialCharacters(model.pending_resignation);
                var other_source_of_income = RemoveSpecialCharacters(model.other_source_of_income);
                var know_about_cashmart = RemoveSpecialCharacters(model.know_about_cashmart);
                var pending_loan_fron_otherland = RemoveSpecialCharacters(model.pending_loan_fron_otherland);
                var bank_loan_or_credit_card = RemoveSpecialCharacters(model.bank_loan_or_credit_card);
                var BankId = model.BankId;
                var family_name1 = RemoveSpecialCharacters(model.family_name1);
                var family_address1 = RemoveSpecialCharacters(model.family_address1);
                var family_contact1 = RemoveSpecialCharacters(model.family_contact1);
                var family_relation1 = RemoveSpecialCharacters(model.family_relation1);
                var family_name2 = RemoveSpecialCharacters(model.family_name2);
                var family_address2 = RemoveSpecialCharacters(model.family_address2);
                var family_contact2 = RemoveSpecialCharacters(model.family_contact2);
                var family_relation2 = RemoveSpecialCharacters(model.family_relation2);
                var optional_name = RemoveSpecialCharacters(model.optional_name);
                var optional_address = RemoveSpecialCharacters(model.optional_address);
                var optional_contact = RemoveSpecialCharacters(model.optional_contact);
                var optional_relation = RemoveSpecialCharacters(model.optional_relation);

                ///============Remarks Fields here============

                var nearest_landmark_remark = RemoveSpecialCharacters(model.nearest_landmark_remark);
                var SSS_No_remark = RemoveSpecialCharacters(model.SSS_No_remark);
                var rented_mortgage_owned_remark = RemoveSpecialCharacters(model.rented_mortgage_owned_remark);
                var Permanent_address_remark = RemoveSpecialCharacters(model.Permanent_address_remark);
                var tansfer_residence_remark = RemoveSpecialCharacters(model.tansfer_residence_remark);
                var spouse_name_remark = RemoveSpecialCharacters(model.spouse_name_remark);
                var spouse_occupation_remark = RemoveSpecialCharacters(model.spouse_occupation_remark);
                var number_of_dependent_remark = RemoveSpecialCharacters(model.number_of_dependent_remark);
                var father_name_work_remark = RemoveSpecialCharacters(model.father_name_work_remark);
                var sibling_works_remark = RemoveSpecialCharacters(model.sibling_works_remark);
                var occupation_remark = RemoveSpecialCharacters(model.occupation_remark);
                var net_income_remark = RemoveSpecialCharacters(model.net_income_remark);
                var scheduled_and_btc_remark = RemoveSpecialCharacters(model.scheduled_and_btc_remark);
                var pay_date_remark = RemoveSpecialCharacters(model.pay_date_remark);
                var pending_resignation_remark = RemoveSpecialCharacters(model.pending_resignation_remark);
                var other_source_of_income_remark = RemoveSpecialCharacters(model.other_source_of_income_remark);
                var know_about_cashmart_remark = RemoveSpecialCharacters(model.know_about_cashmart_remark);
                var pending_loan_fron_otherland_remark = RemoveSpecialCharacters(model.pending_loan_fron_otherland_remark);
                var bank_loan_or_credit_card_remark = RemoveSpecialCharacters(model.bank_loan_or_credit_card_remark);
                var family_name1_remark = RemoveSpecialCharacters(model.family_name1_remark);
                var family_address1_remark = RemoveSpecialCharacters(model.family_address1_remark);
                var family_contact1_remark = RemoveSpecialCharacters(model.family_contact1_remark);
                var family_relation1_remark = RemoveSpecialCharacters(model.family_relation1_remark);
                var family_name2_remark = RemoveSpecialCharacters(model.family_name2_remark);
                var family_address2_remark = RemoveSpecialCharacters(model.family_address2_remark);
                var family_contact2_remark = RemoveSpecialCharacters(model.family_contact2_remark);
                var family_relation2_remark = RemoveSpecialCharacters(model.family_relation2_remark);
                var optional_name_remark = RemoveSpecialCharacters(model.optional_name_remark);
                var optional_address_remark = RemoveSpecialCharacters(model.optional_address_remark);
                var optional_contact_remark = RemoveSpecialCharacters(model.optional_contact_remark);
                var optional_relation_remark = RemoveSpecialCharacters(model.optional_relation_remark);

                #endregion

                if (employmentBorrowerMonthlySalary == "" || employmentBorrowerMonthlySalary == null)
                {
                    employmentBorrowerMonthlySalary = "0.00";
                }
                var currentDate = DateTime.Now;
                var existquery = DbHelper.SelectMethod(string.Format(QueryHelper.ExistVerifierRecord, appNo));
                model.ExistData = Convert.ToString(existquery.Rows[0]["existdetail"]);
                if (model.ExistData == "1")
                {
                    int update = DbHelper.InsertUpdateDelete(string.Format("UPDATE tblapplication_record set designation='{116}',gov_doc_type='" + model.gov_doc_type + "',SSS_No='" + model.SSS_No + "',home_status={83},purposeofloan='" + addLoanPurpose + "', bankname='" + bankName + "', industry='{94}',gender='" + model.gender + "' where applicationno='{0}'; UPDATE tblapplication_personal_verification_details SET requestedamt ='{1}', requestedterms ='{2}', id1_status ='{3}', id2_status ='{4}', pi_status ='{5}', pb_status ='{6}', remarks ='{7}',addtional_birth_place ='{8}', additonal_provincial_address ='{9}', additional_loan_purpose ='{10}',additional_requested_loan_amount ='{11}', additional_requested_term ='{12}',additional_employed_duration ='{13}', additional_job_position ='{14}', additional_job_level ='{15}',additional_paydate ='{16}', family_personal_name_status ='{17}', family_personal_contact_status ='{18}',family_relation_with_borrower_status ='{19}', family_borrower_known_duration_status ='{20}',family_address_verification_status ='{21}', family_borrower_working_place_status ='{22}',friend_personal_name_status ='{23}', friend_personal_contact_status ='{24}',friend_relation_with_borrower_status ='{25}', friend_borrower_known_duration_status ='{26}',friend_address_verification_status ='{27}', friend_borrower_working_place ='{28}',co_worker_personal_name_status ='{29}', co_worker_personal_contact_status ='{30}', co_worker_relation_with_borrower_status ='{31}', co_worker_borrower_known_duration_status ='{32}',co_worker_address_verification_status ='{33}', co_worker_borrower_working_place ='{34}',employement_nameof_work_contact_status ='{35}', employement_no_of_work_contact_status ='{36}',employement_borrower_working_status ='{37}', employement_borrower_position_status ='{38}',employement_borrower_monthly_salary_status ='{39}', employement_borrower_attendance_status ='{40}',employement_borrower_bank_payroll_status ='{41}', family_personal_name ='{42}',family_personal_contact ='{43}', family_relation_with_borrower ='{44}', family_borrower_known_duration ='{45}',family_address_verification ='{46}', family_borrower_working_place ='{47}',friend_personal_name ='{48}', friend_personal_contact ='{49}', friend_relation_with_borrower ='{50}',friend_borrower_known_duration ='{51}', friend_address_verification ='{52}',friend_borrower_working ='{53}', co_worker_personal_name ='{54}', co_worker_personal_contact ='{55}',co_worker_relation_with_borrower ='{56}', co_worker_borrower_known_duration ='{57}',co_worker_address_verification ='{58}', co_worker_borrower_working ='{59}',employement_nameof_work_contact ='{60}', employement_no_of_work_contact ='{61}',employement_borrower_working ='{62}', employement_borrower_position ='{63}',employement_borrower_monthly_salary ='{64}', employement_borrower_attendance ='{65}',employement_borrower_bank_payroll ='{66}', question1_status ='{67}', question2_status ='{68}',question3_status ='{69}', question4_status ='{70}', question5_status ='{71}', question6_status ='{72}',question7_status ='{73}', question8_status ='{74}', question9_status ='{75}', question10_status ='{76}',approved_loan_amount ='{77}', approved_term ='{78}', approved_interest_rate ='{79}',approved_maturity_date ='{80}', approved_total_amount_due ='{81}',nearest_landmark='{82}', tansfer_residence='{84}', spouse_name='{85}', spouse_occupation='{86}', number_of_dependent='{87}', mother_work='{88}', father_name='{89}', father_work='{90}', living_with_mother='{91}', sibling_count='{92}', sibling_works='{93}', occupation='{94}', scheduled_and_btc='{95}', net_income='{96}', pending_resignation='{97}', other_source_of_income='{98}', know_about_cashmart='{99}', pending_loan_fron_otherland='{100}', bank_loan_or_credit_card='{101}', permanent_address='{102}', bankid='{103}', family_name1='{104}', family_address1='{105}', family_contact1='{106}', family_relation1='{107}', family_name2='{108}', family_address2='{109}', family_contact2='{110}', family_relation2='{111}', optional_name='{112}', optional_address='{113}', optional_contact='{114}', optional_relation='{115}' WHERE application_no ='{0}';", appNo, requestedAmt, requestedTerms, id1Status, id2Status, piStatus, pbStatus, remarks, addBirthPlace, addProvincialAdd, AdditionalLoanPurpose_Remark, addRequestedLoanAmount, addrequestedTerm, addEmpDuration, addJobPosition, addJobLevel, addPayDate, familyPersonalNameStatus, familyPersonalContactStatus, familyRelationWithBorrowerStatus, familyBorrowerKnownDurationStatus, familyAddVerificationStatus, familyBorrowerWorkingPlaceStatus, friendPersonalNameStatus, friendPersonalContactStatus, friendRelationWithBorrowerStatus, friendBorrowerKnownDurationStatus, friendAddVerificationStatus, friendBorrowerWorkingPlaceStatus, coworkerPersonalNameStatus, coworkerPersonalContactStatus, coworkerRelationWithBorrowerStatus, coworkerBorrowerKnownDurationStatus, coworkerAddVerificationStatus, coworkerBorrowerWorkingPlaceStatus, employmentNameOfWorkContactStatus, employmentNoOfWorkContactStatus, employmentBorrowerWorkingStatus, employmentBorrowerPositionStatus, employmentBorrowerMonthlySalaryStatus, employmentBorrowerAttendanceStatus, employmentBorrowerBankPayrollStatus, familyPersonalName, familyPersonalContact, familyRelationWithBorrower, familyBorrowerKnownDuration, familyAddVerification, familyBorrowerWorkingPlace, friendPersonalName, friendPersonalContact, friendRelationWithBorrower, friendBorrowerKnownDuration, friendAddVerification, friendBorrowerWorkingPlace, coworkerPersonalName, coworkerPersonalContact, coworkerRelationWithBorrower, coworkerBorrowerKnownDuration, coworkerAddVerification, coworkerBorrowerWorkingPlace, employmentNameOfWorkContact, employmentNoOfWorkContact, employmentBorrowerWorking, employmentBorrowerPosition, employmentBorrowerMonthlySalary, employmentBorrowerAttendance, employmentBorrowerBankPayroll, question1Status, question2Status, question3Status, question4Status, question5Status, question6Status, question7Status, question8Status, question9Status, question10Status, approvedLoanAmt, approvedTerm, approvedInterestRate, approvedMaturityDate, approvedTotalDueAmt, nearest_landmark, home_status, tansfer_residence, spouse_name, spouse_occupation, number_of_dependent, mother_work, father_name, father_work, living_with_mother, sibling_count, sibling_works, occupation, scheduled_and_btc, net_income, pending_resignation, other_source_of_income, know_about_cashmart, pending_loan_fron_otherland, bank_loan_or_credit_card, Permanent_address, BankId, family_name1, family_address1, family_contact1, family_relation1, family_name2, family_address2, family_contact2, family_relation2, optional_name, optional_address, optional_contact, optional_relation, designation, PayDate));
                    if (update > 0)
                    {
                        int UpdateQuery = DbHelper.InsertUpdateDelete(string.Format("UPDATE tblapplicationdata_verification_details SET personal_name_id1_status='{1}',personal_name_id2_status='{2}',personal_name_pi_status='{3}',personal_name_pb_status='{4}',personal_email_id1_status='{5}',personal_email_id2_status='{6}',personal_email_pi_status='{7}',personal_email_pb_status='{8}',personal_contact_no_id1_status='{9}',personal_contact_no_id2_status='{10}',personal_contact_no_pi_status='{11}',personal_contact_no_pb_status='{12}',personal_perma_address_id1_status='{13}',personal_perma_address_id2_status='{14}',personal_perma_address_pi_status='{15}',personal_perma_address_pb_status='{16}',personal_house_ph_id1_status='{17}',personal_house_ph_id2_status='{18}',personal_house_ph_pi_status='{19}',personal_house_ph_pb_status='{20}',personal_birth_place_id1_status='{21}',personal_birth_place_id2_status='{22}',personal_birth_place_pi_status='{23}',personal_birth_place_pb_status='{24}',personal_birth_date_id1_status='{25}',personal_birth_date_id2_status='{26}',personal_birth_date_pi_status='{27}',personal_birth_date_pb_status='{28}',personal_civil_id1_status='{29}',personal_civil_id2_status='{30}',personal_civil_pi_status='{31}',personal_civil_pb_status='{32}',personal_mother_maiden_name_id1_status='{33}',personal_mother_maiden_name_id2_status='{34}',personal_mother_maiden_name_pi_status='{35}',personal_mother_maiden_name_pb_status='{36}',personal_mother_perma_add_id1_status='{37}',personal_mother_perma_add_id2_status='{38}',personal_mother_perma_add_pi_status='{39}',personal_mother_perma_add_pb_status='{40}',personal_company_name_id1_status='{41}',personal_company_name_id2_status='{42}',personal_company_name_pi_status='{43}',personal_company_name_pb_status='{44}',personal_company_address_id1_status='{45}',personal_company_address_id2_status='{46}',personal_company_address_pi_status='{47}',personal_company_address_pb_status='{48}',personal_job_title_id1_status='{49}',personal_job_title_id2_status='{50}',personal_job_title_pi_status='{51}',personal_job_title_pb_status='{52}',personal_monthly_income_id1_status='{53}',personal_monthly_income_id2_status='{54}',personal_monthly_income_pi_status='{55}',personal_monthly_income_pb_status='{56}',personal_reference_name_id1_status='{57}',personal_reference_name_id2_status='{58}',personal_reference_name_pi_status='{59}',personal_reference_name_pb_status='{60}',personal_reference_contact_id1_status='{61}',personal_reference_contact_id2_status='{62}',personal_reference_contact_pi_status='{63}',personal_reference_contact_pb_status='{64}',personal_bank_name_id1_status='{65}',personal_bank_name_id2_status='{66}',personal_bank_name_pi_status='{67}',personal_bank_name_pb_status='{68}',personal_bank_ac_no_id1_status='{69}',personal_bank_ac_no_id2_status='{70}',personal_bank_ac_no_pi_status='{71}',personal_bank_ac_no_pb_status='{72}',personal_req_loan_amt_id1_status='{73}',personal_req_loan_amt_id2_status='{74}',personal_req_loan_amt_pi_status='{75}',personal_req_loan_amt_pb_status='{76}',personal_req_loan_term_id1_status='{77}',personal_req_loan_term_id2_status='{78}',personal_req_loan_term_pi_status='{79}',personal_req_loan_term_pb_status='{80}',personal_name_remark='{81}',personal_email_remark='{82}',personal_contact_no_remark='{83}',personal_perma_address_remark='{84}',personal_house_ph_remark='{85}',personal_birth_place_remark='{86}',personal_birth_date_remark='{87}',personal_civil_remark='{88}',personal_mother_maiden_name_remark='{89}',personal_mother_perma_add_remark='{90}',personal_company_name_remark='{91}',personal_company_address_remark='{92}',personal_job_title_remark='{93}',personal_monthly_income_remark='{94}',personal_reference_name_remark='{95}',personal_reference_contact_remark='{96}',personal_bank_name_remark='{97}',personal_bank_ac_no_remark='{98}',personal_req_loan_amt_remark='{99}',personal_req_loan_term_remark='{100}',nearest_landmark_remark='{101}', sss_no_remark='{102}', rented_mortgage_owned_remark='{103}', permanent_address_remark='{104}', tansfer_residence_remark='{105}', spouse_name_remark='{106}', spouse_occupation_remark='{107}', number_of_dependent_remark='{108}', father_name_work_remark='{109}', sibling_works_remark='{110}', occupation_remark='{111}', net_income_remark='{112}', scheduled_and_btc_remark='{113}', pay_date_remark='{114}', pending_resignation_remark='{115}', other_source_of_income_remark='{116}', know_about_cashmart_remark='{117}', pending_loan_fron_otherland_remark='{118}', bank_loan_or_credit_card_remark='{119}', family_name1_remark='{120}', family_address1_remark='{121}', family_contact1_remark='{122}', family_relation1_remark='{123}', family_name2_remark='{124}', family_address2_remark='{125}', family_contact2_remark='{126}', family_relation2_remark='{127}', optional_name_remark='{128}', optional_address_remark='{129}', optional_contact_remark='{130}', optional_relation_remark='{131}' WHERE application_no ={0};", appNo, personalNameId1Status, personalNameId2Status, personalNamePiStatus, personalNamePbStatus, personalEmailId1Status, personalEmailId2Status, personalEmailPiStatus, personalEmailPbStatus, personalContactId1Status, personalContactId2Status, personalContactPiStatus, personalContactPbStatus, personalPerAddId1Status, personalPerAddId2Status, personalPerAddPiStatus, personalPerAddPbStatus, personalHousePhId1Status, personalHousePhId2Status, personalHousePhPiStatus, personalHousePhPbStatus, personalBirthPlaceId1Status, personalBirthPlaceId2Status, personalBirthPlacePiStatus, personalBirthPlacePbStatus, personalBirthDateId1Status, personalBirthDateId2Status, personalBirthDatePiStatus, personalBirthDatePbStatus, personalCivilId1Status, personalCivilId2Status, personalCivilPiStatus, personalCivilPbStatus, personalMotherMaidenNameId1Status, personalMotherMaidenNameId2Status, personalMotherMaidenNamePiStatus, personalMotherMaidenNamePbStatus, personalMotherPerAddId1Status, personalMotherPerAddId2Status, personalMotherPerAddPiStatus, personalMotherPerAddPbStatus, personalCompanyNameId1Status, personalCompanyNameId2Status, personalCompanyNamePiStatus, personalCompanyNamePbStatus, personalCompanyAddId1Status, personalCompanyAddId2Status, personalCompanyAddPiStatus, personalCompanyAddPbStatus, personalJobTitleId1Status, personalJobTitleId2Status, personalJobTitlePiStatus, personalJobTitlePbStatus, personalMonthlyIncomeId1Status, personalMonthlyIncomeId2Status, personalMonthlyIncomePiStatus, personalMonthlyIncomePbStatus, personalRefNameId1Status, personalRefNameId2Status, personalRefNamePiStatus, personalRefNamePbStatus, personalRefContactId1Status, personalRefContactId2Status, personalRefContactPiStatus, personalRefContactPbStatus, personalBankNameId1Status, personalBankNameId2Status, personalBankNamePiStatus, personalBankNamePbStatus, personalBankAccNoId1Status, personalBankAccNoId2Status, personalBankAccNoPiStatus, personalBankAccNoPbStatus, personalReqLoanAmtId1Status, personalReqLoanAmtId2Status, personalReqLoanAmtPiStatus, personalReqLoanAmtPbStatus, personalReqLoanTermId1Status, personalReqLoanTermId2Status, personalReqLoanTermPiStatus, personalReqLoanTermPbStatus, personalNameRemark, personalEmailRemark, persoanlContactRemark, persoanlPerAddRemark, personalHousePhRemark, personalBirthPlaceRemark, personalBirthDateRemark, personalCivilRemark, personalMotherMaidenNameRemark, personalMotherPerAddRemark, personalCompanyNameRemark, personalCompanyAddRemark, personalJobTitleremark, personalMonthlyIncomeRemark, personalRefNameRemark, personalRefContactRemark, personalBankNameRemark, personalBankAccNoRemark, personalReqLoanAmtRemark, personalReqLoanTermRemark, nearest_landmark_remark, SSS_No_remark, rented_mortgage_owned_remark, Permanent_address_remark, tansfer_residence_remark, spouse_name_remark, spouse_occupation_remark, number_of_dependent_remark, father_name_work_remark, sibling_works_remark, occupation_remark, net_income_remark, scheduled_and_btc_remark, pay_date_remark, pending_resignation_remark, other_source_of_income_remark, know_about_cashmart_remark, pending_loan_fron_otherland_remark, bank_loan_or_credit_card_remark, family_name1_remark, family_address1_remark, family_contact1_remark, family_relation1_remark, family_name2_remark, family_address2_remark, family_contact2_remark, family_relation2_remark, optional_name_remark, optional_address_remark, optional_contact_remark, optional_relation_remark));
                        if (UpdateQuery > 0)
                        {
                            var updateQuery = DbHelper.InsertUpdateDelete(string.Format("Update tblapplication_record Set term='{1}',termtype='{2}', purposeofloan='{3}' where applicationno='{0}';", appNo, TermName, Term, addLoanPurpose));
                            var updateQuery1 = DbHelper.InsertUpdateDelete("update tblapplication_record set address = '" + address + "', homephoneno = '" + HomePhone + "', mothermaidenname = '" + MotherMaidenName + "', motheraddress = '" + MotherPermanentAddress + "', companyname = '" + companyName + "', companyaddress = '" + compnayaddress + "', gross_income = '" + GrossMonthlyIncome + "', reference_name = '" + refereneceName + "', reference_contactno = '" + ReferenceContactNo + "', bankname = '" + bankName + "', bankaccountno = '" + bankAcctNo + "' where applicationno = '" + appNo + "'");
                            result = true;
                        }
                        else
                        {
                            int insertQuery = DbHelper.InsertUpdateDelete(string.Format("insert into tblapplicationdata_verification_details(application_no, personal_name_id1_status, personal_name_id2_status,personal_name_pi_status, personal_name_pb_status, personal_email_id1_status,personal_email_id2_status, personal_email_pi_status, personal_email_pb_status,personal_contact_no_id1_status, personal_contact_no_id2_status,personal_contact_no_pi_status, personal_contact_no_pb_status,personal_perma_address_id1_status, personal_perma_address_id2_status,personal_perma_address_pi_status, personal_perma_address_pb_status,personal_house_ph_id1_status, personal_house_ph_id2_status, personal_house_ph_pi_status,personal_house_ph_pb_status, personal_birth_place_id1_status,personal_birth_place_id2_status, personal_birth_place_pi_status,personal_birth_place_pb_status, personal_birth_date_id1_status,personal_birth_date_id2_status, personal_birth_date_pi_status,personal_birth_date_pb_status, personal_civil_id1_status, personal_civil_id2_status,personal_civil_pi_status, personal_civil_pb_status, personal_mother_maiden_name_id1_status,personal_mother_maiden_name_id2_status, personal_mother_maiden_name_pi_status,personal_mother_maiden_name_pb_status, personal_mother_perma_add_id1_status,personal_mother_perma_add_id2_status, personal_mother_perma_add_pi_status,personal_mother_perma_add_pb_status, personal_company_name_id1_status,personal_company_name_id2_status, personal_company_name_pi_status,personal_company_name_pb_status, personal_company_address_id1_status,personal_company_address_id2_status, personal_company_address_pi_status,personal_company_address_pb_status, personal_job_title_id1_status,personal_job_title_id2_status, personal_job_title_pi_status,personal_job_title_pb_status,personal_monthly_income_id1_status,personal_monthly_income_id2_status, personal_monthly_income_pi_status,personal_monthly_income_pb_status, personal_reference_name_id1_status,personal_reference_name_id2_status, personal_reference_name_pi_status,personal_reference_name_pb_status, personal_reference_contact_id1_status,personal_reference_contact_id2_status, personal_reference_contact_pi_status,personal_reference_contact_pb_status, personal_bank_name_id1_status,personal_bank_name_id2_status, personal_bank_name_pi_status,personal_bank_name_pb_status, personal_bank_ac_no_id1_status,personal_bank_ac_no_id2_status, personal_bank_ac_no_pi_status,personal_bank_ac_no_pb_status, personal_req_loan_amt_id1_status,personal_req_loan_amt_id2_status, personal_req_loan_amt_pi_status,personal_req_loan_amt_pb_status, personal_req_loan_term_id1_status,personal_req_loan_term_id2_status, personal_req_loan_term_pi_status,personal_req_loan_term_pb_status, personal_name_remark, personal_email_remark,personal_contact_no_remark, personal_perma_address_remark, personal_house_ph_remark,personal_birth_place_remark, personal_birth_date_remark, personal_civil_remark,personal_mother_maiden_name_remark, personal_mother_perma_add_remark,personal_company_name_remark, personal_company_address_remark,personal_job_title_remark, personal_monthly_income_remark, personal_reference_name_remark,personal_reference_contact_remark, personal_bank_name_remark,personal_bank_ac_no_remark, personal_req_loan_amt_remark, personal_req_loan_term_remark, nearest_landmark_remark, sss_no_remark, rented_mortgage_owned_remark, permanent_address_remark, tansfer_residence_remark, spouse_name_remark, spouse_occupation_remark, number_of_dependent_remark, father_name_work_remark, sibling_works_remark, occupation_remark, net_income_remark, scheduled_and_btc_remark, pay_date_remark, pending_resignation_remark, other_source_of_income_remark, know_about_cashmart_remark, pending_loan_fron_otherland_remark, bank_loan_or_credit_card_remark, family_name1_remark, family_address1_remark, family_contact1_remark, family_relation1_remark, family_name2_remark, family_address2_remark, family_contact2_remark, family_relation2_remark, optional_name_remark, optional_address_remark, optional_contact_remark, optional_relation_remark)VALUES('{0}','{1}', '{2}','{3}','{4}', '{5}', '{6}', '{7}', '{8}','{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}', '{16}', '{17}', '{18}', '{19}', '{20}',  '{21}', '{22}', '{23}', '{24}','{25}','{26}','{27}','{28}','{29}','{30}','{31}','{32}','{33}','{34}','{35}','{36}','{37}','{38}','{39}','{40}', '{41}', '{42}','{43}','{44}','{45}', '{46}','{47}', '{48}','{49}','{50}', '{51}','{52}','{53}','{54}','{55}','{56}','{57}','{58}', '{59}','{60}','{61}','{62}','{63}','{64}','{65}','{66}','{67}','{68}','{69}','{70}','{71}','{72}','{73}','{74}','{75}','{76}','{77}', '{78}','{79}','{80}','{81}','{82}','{83}','{84}','{85}','{86}','{87}','{88}','{89}','{90}','{91}','{92}','{93}','{94}','{95}','{96}','{97}','{98}','{99}','{100}','{101}','{102}','{103}','{104}','{105}','{106}','{107}','{108}','{109}','{110}','{111}','{112}','{113}','{114}','{115}','{116}','{117}','{118}','{119}','{120}','{121}','{122}','{123}','{124}','{125}','{126}','{127}','{128}','{129}','{130}','{131}');", appNo, personalNameId1Status, personalNameId2Status, personalNamePiStatus, personalNamePbStatus, personalEmailId1Status, personalEmailId2Status, personalEmailPiStatus, personalEmailPbStatus, personalContactId1Status, personalContactId2Status, personalContactPiStatus, personalContactPbStatus, personalPerAddId1Status, personalPerAddId2Status, personalPerAddPiStatus, personalPerAddPbStatus, personalHousePhId1Status, personalHousePhId2Status, personalHousePhPiStatus, personalHousePhPbStatus, personalBirthPlaceId1Status, personalBirthPlaceId2Status, personalBirthPlacePiStatus, personalBirthPlacePbStatus, personalBirthDateId1Status, personalBirthDateId2Status, personalBirthDatePiStatus, personalBirthDatePbStatus, personalCivilId1Status, personalCivilId2Status, personalCivilPiStatus, personalCivilPbStatus, personalMotherMaidenNameId1Status, personalMotherMaidenNameId2Status, personalMotherMaidenNamePiStatus, personalMotherMaidenNamePbStatus, personalMotherPerAddId1Status, personalMotherPerAddId2Status, personalMotherPerAddPiStatus, personalMotherPerAddPbStatus, personalCompanyNameId1Status, personalCompanyNameId2Status, personalCompanyNamePiStatus, personalCompanyNamePbStatus, personalCompanyAddId1Status, personalCompanyAddId2Status, personalCompanyAddPiStatus, personalCompanyAddPbStatus, personalJobTitleId1Status, personalJobTitleId2Status, personalJobTitlePiStatus, personalJobTitlePbStatus, personalMonthlyIncomeId1Status, personalMonthlyIncomeId2Status, personalMonthlyIncomePiStatus, personalMonthlyIncomePbStatus, personalRefNameId1Status, personalRefNameId2Status, personalRefNamePiStatus, personalRefNamePbStatus, personalRefContactId1Status, personalRefContactId2Status, personalRefContactPiStatus, personalRefContactPbStatus, personalBankNameId1Status, personalBankNameId2Status, personalBankNamePiStatus, personalBankNamePbStatus, personalBankAccNoId1Status, personalBankAccNoId2Status, personalBankAccNoPiStatus, personalBankAccNoPbStatus, personalReqLoanAmtId1Status, personalReqLoanAmtId2Status, personalReqLoanAmtPiStatus, personalReqLoanAmtPbStatus, personalReqLoanTermId1Status, personalReqLoanTermId2Status, personalReqLoanTermPiStatus, personalReqLoanTermPbStatus, personalNameRemark, personalEmailRemark, persoanlContactRemark, persoanlPerAddRemark, personalHousePhRemark, personalBirthPlaceRemark, personalBirthDateRemark, personalCivilRemark, personalMotherMaidenNameRemark, personalMotherPerAddRemark, personalCompanyNameRemark, personalCompanyAddRemark, personalJobTitleremark, personalMonthlyIncomeRemark, personalRefNameRemark, personalRefContactRemark, personalBankNameRemark, personalBankAccNoRemark, personalReqLoanAmtRemark, personalReqLoanTermRemark, nearest_landmark_remark, SSS_No_remark, rented_mortgage_owned_remark, Permanent_address_remark, tansfer_residence_remark, spouse_name_remark, spouse_occupation_remark, number_of_dependent_remark, father_name_work_remark, sibling_works_remark, occupation_remark, net_income_remark, scheduled_and_btc_remark, pay_date_remark, pending_resignation_remark, other_source_of_income_remark, know_about_cashmart_remark, pending_loan_fron_otherland_remark, bank_loan_or_credit_card_remark, family_name1_remark, family_address1_remark, family_contact1_remark, family_relation1_remark, family_name2_remark, family_address2_remark, family_contact2_remark, family_relation2_remark, optional_name_remark, optional_address_remark, optional_contact_remark, optional_relation_remark));
                            if (insertQuery > 0)
                            {
                                var updateQuery = DbHelper.InsertUpdateDelete(string.Format("Update tblapplication_record Set term='{1}',termtype='{2}', purposeofloan='{3}' where applicationno='{0}';", appNo, TermName, Term, addLoanPurpose));
                                var updateQuery1 = DbHelper.InsertUpdateDelete("update tblapplication_record set address = '" + address + "', homephoneno = '" + HomePhone + "', mothermaidenname = '" + MotherMaidenName + "', motheraddress = '" + MotherPermanentAddress + "', companyname = '" + companyName + "', companyaddress = '" + compnayaddress + "', gross_income = '" + GrossMonthlyIncome + "', reference_name = '" + refereneceName + "', reference_contactno = '" + ReferenceContactNo + "', bankname = '" + bankName + "', bankaccountno = '" + bankAcctNo + "' where applicationno = '" + appNo + "'");
                                result = true;
                            }
                        }
                    }
                }
                else
                {
                    int insert = DbHelper.InsertUpdateDelete(string.Format("UPDATE tblapplication_record set designation='{116}',gov_doc_type='" + model.gov_doc_type + "',SSS_No='" + model.SSS_No + "',home_status={83},gender='" + model.gender + "',purposeofloan='" + addLoanPurpose + "', bankname='" + bankName + "', industry='" + occupation + "' where applicationno='{0}'; Insert Into tblapplication_personal_verification_details(application_no, requestedamt, requestedterms, id1_status, id2_status, pi_status, pb_status, remarks, addtional_birth_place, additonal_provincial_address, additional_loan_purpose, additional_requested_loan_amount, additional_requested_term, additional_employed_duration, additional_job_position, additional_job_level, additional_paydate, family_personal_name_status, family_personal_contact_status, family_relation_with_borrower_status, family_borrower_known_duration_status, family_address_verification_status, family_borrower_working_place_status, friend_personal_name_status, friend_personal_contact_status, friend_relation_with_borrower_status, friend_borrower_known_duration_status, friend_address_verification_status, friend_borrower_working_place, co_worker_personal_name_status, co_worker_personal_contact_status, co_worker_relation_with_borrower_status, co_worker_borrower_known_duration_status, co_worker_address_verification_status, co_worker_borrower_working_place, employement_nameof_work_contact_status, employement_no_of_work_contact_status, employement_borrower_working_status, employement_borrower_position_status, employement_borrower_monthly_salary_status, employement_borrower_attendance_status, employement_borrower_bank_payroll_status, family_personal_name, family_personal_contact, family_relation_with_borrower, family_borrower_known_duration, family_address_verification, family_borrower_working_place, friend_personal_name, friend_personal_contact, friend_relation_with_borrower, friend_borrower_known_duration, friend_address_verification, friend_borrower_working, co_worker_personal_name, co_worker_personal_contact, co_worker_relation_with_borrower, co_worker_borrower_known_duration, co_worker_address_verification, co_worker_borrower_working, employement_nameof_work_contact, employement_no_of_work_contact, employement_borrower_working, employement_borrower_position, employement_borrower_monthly_salary, employement_borrower_attendance, employement_borrower_bank_payroll, question1_status, question2_status, question3_status, question4_status, question5_status, question6_status, question7_status, question8_status, question9_status, question10_status, approved_loan_amount, approved_term, approved_interest_rate, approved_maturity_date, approved_total_amount_due,nearest_landmark, tansfer_residence, spouse_name,spouse_occupation, number_of_dependent, mother_work, father_name, father_work, living_with_mother, sibling_count, sibling_works, occupation, scheduled_and_btc, net_income, pending_resignation, other_source_of_income, know_about_cashmart, pending_loan_fron_otherland, bank_loan_or_credit_card, permanent_address, bankid, family_name1, family_address1, family_contact1, family_relation1, family_name2, family_address2, family_contact2, family_relation2, optional_name, optional_address, optional_contact, optional_relation)VALUES('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}','{20}','{21}','{22}','{23}','{24}','{25}','{26}','{27}','{28}','{29}','{30}','{31}','{32}','{33}','{34}','{35}','{36}','{37}','{38}','{39}','{40}','{41}','{42}','{43}','{44}','{45}','{46}','{47}','{48}','{49}','{50}','{51}','{52}','{53}','{54}', '{55}','{56}','{57}','{58}','{59}','{60}','{61}','{62}','{63}','{64}','{65}','{66}','{67}','{68}','{69}','{70}','{71}','{72}','{73}','{74}','{75}','{76}','{77}','{78}','{79}','{80}','{81}','{82}','{84}','{85}','{86}','{87}','{88}','{89}','{90}','{91}','{92}','{93}','{94}','{95}','{96}','{97}','{98}','{99}','{100}','{101}','{102}','{103}','{104}','{105}','{106}','{107}','{108}','{109}','{110}','{111}','{112}','{113}','{114}','{115}');", appNo, requestedAmt, requestedTerms, id1Status, id2Status, piStatus, pbStatus, remarks, addBirthPlace, addProvincialAdd, AdditionalLoanPurpose_Remark, addRequestedLoanAmount, addrequestedTerm, addEmpDuration, addJobPosition, addJobLevel, addPayDate, familyPersonalNameStatus, familyPersonalContactStatus, familyRelationWithBorrowerStatus, familyBorrowerKnownDurationStatus, familyAddVerificationStatus, familyBorrowerWorkingPlaceStatus, friendPersonalNameStatus, friendPersonalContactStatus, friendRelationWithBorrowerStatus, friendBorrowerKnownDurationStatus, friendAddVerificationStatus, friendBorrowerWorkingPlaceStatus, coworkerPersonalNameStatus, coworkerPersonalContactStatus, coworkerRelationWithBorrowerStatus, coworkerBorrowerKnownDurationStatus, coworkerAddVerificationStatus, coworkerBorrowerWorkingPlaceStatus, employmentNameOfWorkContactStatus, employmentNoOfWorkContactStatus, employmentBorrowerWorkingStatus, employmentBorrowerPositionStatus, employmentBorrowerMonthlySalaryStatus, employmentBorrowerAttendanceStatus, employmentBorrowerBankPayrollStatus, familyPersonalName, familyPersonalContact, familyRelationWithBorrower, familyBorrowerKnownDuration, familyAddVerification, familyBorrowerWorkingPlace, friendPersonalName, friendPersonalContact, friendRelationWithBorrower, friendBorrowerKnownDuration, friendAddVerification, friendBorrowerWorkingPlace, coworkerPersonalName, coworkerPersonalContact, coworkerRelationWithBorrower, coworkerBorrowerKnownDuration, coworkerAddVerification, coworkerBorrowerWorkingPlace, employmentNameOfWorkContact, employmentNoOfWorkContact, employmentBorrowerWorking, employmentBorrowerPosition, employmentBorrowerMonthlySalary, employmentBorrowerAttendance, employmentBorrowerBankPayroll, question1Status, question2Status, question3Status, question4Status, question5Status, question6Status, question7Status, question8Status, question9Status, question10Status, approvedLoanAmt, approvedTerm, approvedInterestRate, approvedMaturityDate, approvedTotalDueAmt, nearest_landmark, home_status, tansfer_residence, spouse_name, spouse_occupation, number_of_dependent, mother_work, father_name, father_work, living_with_mother, sibling_count, sibling_works, occupation, scheduled_and_btc, net_income, pending_resignation, other_source_of_income, know_about_cashmart, pending_loan_fron_otherland, bank_loan_or_credit_card, Permanent_address, BankId, family_name1, family_address1, family_contact1, family_relation1, family_name2, family_address2, family_contact2, family_relation2, optional_name, optional_address, optional_contact, optional_relation, designation, PayDate));
                    if (insert > 0)
                    {
                        int insertQuery = DbHelper.InsertUpdateDelete(string.Format("insert into tblapplicationdata_verification_details(application_no, personal_name_id1_status, personal_name_id2_status,personal_name_pi_status, personal_name_pb_status, personal_email_id1_status,personal_email_id2_status, personal_email_pi_status, personal_email_pb_status,personal_contact_no_id1_status, personal_contact_no_id2_status,personal_contact_no_pi_status, personal_contact_no_pb_status,personal_perma_address_id1_status, personal_perma_address_id2_status,personal_perma_address_pi_status, personal_perma_address_pb_status,personal_house_ph_id1_status, personal_house_ph_id2_status, personal_house_ph_pi_status,personal_house_ph_pb_status, personal_birth_place_id1_status,personal_birth_place_id2_status, personal_birth_place_pi_status,personal_birth_place_pb_status, personal_birth_date_id1_status,personal_birth_date_id2_status, personal_birth_date_pi_status,personal_birth_date_pb_status, personal_civil_id1_status, personal_civil_id2_status,personal_civil_pi_status, personal_civil_pb_status, personal_mother_maiden_name_id1_status,personal_mother_maiden_name_id2_status, personal_mother_maiden_name_pi_status,personal_mother_maiden_name_pb_status, personal_mother_perma_add_id1_status,personal_mother_perma_add_id2_status, personal_mother_perma_add_pi_status,personal_mother_perma_add_pb_status, personal_company_name_id1_status,personal_company_name_id2_status, personal_company_name_pi_status,personal_company_name_pb_status, personal_company_address_id1_status,personal_company_address_id2_status, personal_company_address_pi_status,personal_company_address_pb_status, personal_job_title_id1_status,personal_job_title_id2_status, personal_job_title_pi_status,personal_job_title_pb_status,personal_monthly_income_id1_status,personal_monthly_income_id2_status, personal_monthly_income_pi_status,personal_monthly_income_pb_status, personal_reference_name_id1_status,personal_reference_name_id2_status, personal_reference_name_pi_status,personal_reference_name_pb_status, personal_reference_contact_id1_status,personal_reference_contact_id2_status, personal_reference_contact_pi_status,personal_reference_contact_pb_status, personal_bank_name_id1_status,personal_bank_name_id2_status, personal_bank_name_pi_status,personal_bank_name_pb_status, personal_bank_ac_no_id1_status,personal_bank_ac_no_id2_status, personal_bank_ac_no_pi_status,personal_bank_ac_no_pb_status, personal_req_loan_amt_id1_status,personal_req_loan_amt_id2_status, personal_req_loan_amt_pi_status,personal_req_loan_amt_pb_status, personal_req_loan_term_id1_status,personal_req_loan_term_id2_status, personal_req_loan_term_pi_status,personal_req_loan_term_pb_status, personal_name_remark, personal_email_remark,personal_contact_no_remark, personal_perma_address_remark, personal_house_ph_remark,personal_birth_place_remark, personal_birth_date_remark, personal_civil_remark,personal_mother_maiden_name_remark, personal_mother_perma_add_remark,personal_company_name_remark, personal_company_address_remark,personal_job_title_remark, personal_monthly_income_remark, personal_reference_name_remark,personal_reference_contact_remark, personal_bank_name_remark,personal_bank_ac_no_remark, personal_req_loan_amt_remark, personal_req_loan_term_remark, nearest_landmark_remark, sss_no_remark, rented_mortgage_owned_remark, permanent_address_remark, tansfer_residence_remark, spouse_name_remark, spouse_occupation_remark, number_of_dependent_remark, father_name_work_remark, sibling_works_remark, occupation_remark, net_income_remark, scheduled_and_btc_remark, pay_date_remark, pending_resignation_remark, other_source_of_income_remark, know_about_cashmart_remark, pending_loan_fron_otherland_remark, bank_loan_or_credit_card_remark, family_name1_remark, family_address1_remark, family_contact1_remark, family_relation1_remark, family_name2_remark, family_address2_remark, family_contact2_remark, family_relation2_remark, optional_name_remark, optional_address_remark, optional_contact_remark, optional_relation_remark)VALUES('{0}','{1}', '{2}','{3}','{4}', '{5}', '{6}', '{7}', '{8}','{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}', '{16}', '{17}', '{18}', '{19}', '{20}',  '{21}', '{22}', '{23}', '{24}','{25}','{26}','{27}','{28}','{29}','{30}','{31}','{32}','{33}','{34}','{35}','{36}','{37}','{38}','{39}','{40}', '{41}', '{42}','{43}','{44}','{45}', '{46}','{47}', '{48}','{49}','{50}', '{51}','{52}','{53}','{54}','{55}','{56}','{57}','{58}', '{59}','{60}','{61}','{62}','{63}','{64}','{65}','{66}','{67}','{68}','{69}','{70}','{71}','{72}','{73}','{74}','{75}','{76}','{77}', '{78}','{79}','{80}','{81}','{82}','{83}','{84}','{85}','{86}','{87}','{88}','{89}','{90}','{91}','{92}','{93}','{94}','{95}','{96}','{97}','{98}','{99}','{100}','{101}','{102}','{103}','{104}','{105}','{106}','{107}','{108}','{109}','{110}','{111}','{112}','{113}','{114}','{115}','{116}','{117}','{118}','{119}','{120}','{121}','{122}','{123}','{124}','{125}','{126}','{127}','{128}','{129}','{130}','{131}');", appNo, personalNameId1Status, personalNameId2Status, personalNamePiStatus, personalNamePbStatus, personalEmailId1Status, personalEmailId2Status, personalEmailPiStatus, personalEmailPbStatus, personalContactId1Status, personalContactId2Status, personalContactPiStatus, personalContactPbStatus, personalPerAddId1Status, personalPerAddId2Status, personalPerAddPiStatus, personalPerAddPbStatus, personalHousePhId1Status, personalHousePhId2Status, personalHousePhPiStatus, personalHousePhPbStatus, personalBirthPlaceId1Status, personalBirthPlaceId2Status, personalBirthPlacePiStatus, personalBirthPlacePbStatus, personalBirthDateId1Status, personalBirthDateId2Status, personalBirthDatePiStatus, personalBirthDatePbStatus, personalCivilId1Status, personalCivilId2Status, personalCivilPiStatus, personalCivilPbStatus, personalMotherMaidenNameId1Status, personalMotherMaidenNameId2Status, personalMotherMaidenNamePiStatus, personalMotherMaidenNamePbStatus, personalMotherPerAddId1Status, personalMotherPerAddId2Status, personalMotherPerAddPiStatus, personalMotherPerAddPbStatus, personalCompanyNameId1Status, personalCompanyNameId2Status, personalCompanyNamePiStatus, personalCompanyNamePbStatus, personalCompanyAddId1Status, personalCompanyAddId2Status, personalCompanyAddPiStatus, personalCompanyAddPbStatus, personalJobTitleId1Status, personalJobTitleId2Status, personalJobTitlePiStatus, personalJobTitlePbStatus, personalMonthlyIncomeId1Status, personalMonthlyIncomeId2Status, personalMonthlyIncomePiStatus, personalMonthlyIncomePbStatus, personalRefNameId1Status, personalRefNameId2Status, personalRefNamePiStatus, personalRefNamePbStatus, personalRefContactId1Status, personalRefContactId2Status, personalRefContactPiStatus, personalRefContactPbStatus, personalBankNameId1Status, personalBankNameId2Status, personalBankNamePiStatus, personalBankNamePbStatus, personalBankAccNoId1Status, personalBankAccNoId2Status, personalBankAccNoPiStatus, personalBankAccNoPbStatus, personalReqLoanAmtId1Status, personalReqLoanAmtId2Status, personalReqLoanAmtPiStatus, personalReqLoanAmtPbStatus, personalReqLoanTermId1Status, personalReqLoanTermId2Status, personalReqLoanTermPiStatus, personalReqLoanTermPbStatus, personalNameRemark, personalEmailRemark, persoanlContactRemark, persoanlPerAddRemark, personalHousePhRemark, personalBirthPlaceRemark, personalBirthDateRemark, personalCivilRemark, personalMotherMaidenNameRemark, personalMotherPerAddRemark, personalCompanyNameRemark, personalCompanyAddRemark, personalJobTitleremark, personalMonthlyIncomeRemark, personalRefNameRemark, personalRefContactRemark, personalBankNameRemark, personalBankAccNoRemark, personalReqLoanAmtRemark, personalReqLoanTermRemark, nearest_landmark_remark, SSS_No_remark, rented_mortgage_owned_remark, Permanent_address_remark, tansfer_residence_remark, spouse_name_remark, spouse_occupation_remark, number_of_dependent_remark, father_name_work_remark, sibling_works_remark, occupation_remark, net_income_remark, scheduled_and_btc_remark, pay_date_remark, pending_resignation_remark, other_source_of_income_remark, know_about_cashmart_remark, pending_loan_fron_otherland_remark, bank_loan_or_credit_card_remark, family_name1_remark, family_address1_remark, family_contact1_remark, family_relation1_remark, family_name2_remark, family_address2_remark, family_contact2_remark, family_relation2_remark, optional_name_remark, optional_address_remark, optional_contact_remark, optional_relation_remark));
                        if (insertQuery > 0)
                        {
                            var updateQuery = DbHelper.InsertUpdateDelete(string.Format("Update tblapplication_record Set term='{1}',termtype='{2}', purposeofloan='{3}' where applicationno='{0}';", appNo, TermName, Term, addLoanPurpose));
                            var updateQuery1 = DbHelper.InsertUpdateDelete("update tblapplication_record set address = '" + address + "', homephoneno = '" + HomePhone + "', mothermaidenname = '" + MotherMaidenName + "', motheraddress = '" + MotherPermanentAddress + "', companyname = '" + companyName + "', companyaddress = '" + compnayaddress + "', gross_income = '" + GrossMonthlyIncome + "', reference_name = '" + refereneceName + "', reference_contactno = '" + ReferenceContactNo + "', bankname = '" + bankName + "', bankaccountno = '" + bankAcctNo + "' where applicationno = '" + appNo + "'");
                            result = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result = false;
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
            }
            return result;
        }

        [HttpPost]
        public ActionResult GetDataAccToTerm(int id)
        {
           
            Terms termVM = new Terms();
            try
            {
                DataTable dt = new DataTable();
                dt = DbHelper.SelectMethod(string.Format(QueryHelper.GetDataAccToTermNew, id));
                if (dt != null && dt.Rows.Count > 0)
                {
                    termVM.TermTypeName = Convert.ToString(dt.Rows[0]["termtype"]);
                    termVM.Rate = Convert.ToString(dt.Rows[0]["interest_rate"]) != "" ? Convert.ToDecimal(dt.Rows[0]["interest_rate"]) : 0;
                    termVM.term_Value = Convert.ToString(dt.Rows[0]["term_value"]) != "" ? Convert.ToString(dt.Rows[0]["term_value"]) : "0";
                    termVM.approved_term = Convert.ToString(dt.Rows[0]["id"]) != "" ? Convert.ToString(dt.Rows[0]["id"]) : "0";
                    termVM.admin_fee = Convert.ToDecimal(dt.Rows[0]["admin_fee"]);
                }
            }
            catch (Exception ex)
            {
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                return Json(null);
            }
            return Json(termVM);
        }

        public string UpdateIsPickedVerifierFalse(string appNo)
        {
            string ActionName = "Index";
            if (Session["ActionName"] != null)
            {
                ActionName = Convert.ToString(Session["ActionName"]);
            }
            int intAppNo = 0;
            int query = 0;
            try
            {
                if (!string.IsNullOrWhiteSpace(appNo))
                {
                    intAppNo = Convert.ToInt32(appNo);
                }
                if (ActionName == "Index")
                {
                    query = DbHelper.InsertUpdateDelete(string.Format(QueryHelper.UpdateIsPickedVerifierFalse, intAppNo));
                }
                else
                {
                    query = DbHelper.InsertUpdateDelete(string.Format(QueryHelper.UpdateIsPickedReVerifierFalse, intAppNo));
                }
            }
            catch (Exception ex)
            {
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                return null;
            }
            return ActionName;
        }

        /// <summary>
        /// KIV Bucket,
        /// Created By : Bilal 
        ///On Date: 28/08/2018
        /// </summary>
        /// <returns></returns>
        public ActionResult KIVBucket()
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.Verifier))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            Session["ActionName"] = "KIVBucket";
            return View();
        }

        /// <summary>
        /// KIV Bucket Data
        /// </summary>
        /// <returns></returns>
        public ActionResult GetKIVBucketData()
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.Verifier))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            JsonResult jsonResult = new JsonResult();
            List<ApplicationRecordVM> appRecordVM = new List<ApplicationRecordVM>();
            try
            {
                DataTable dt;
                TempData["Record"] = "Get Method";
                int UserId = Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId].Value == null ? "0" : HttpContext.Request.Cookies[CookiesKey.UserId].Value);
                dt = DbHelper.SelectMethod(string.Format(QueryHelper.GetApplicationforVerifier_KIV, UserId));
                if (dt != null && dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        ApplicationRecordVM checkdetail = new ApplicationRecordVM();
                        checkdetail.ApplicationNo = Convert.ToInt32(dt.Rows[i]["applicationno"].ToString());
                        checkdetail.First_Name = Convert.ToString(dt.Rows[i]["first_name"]);
                        checkdetail.Middle_Name = Convert.ToString(dt.Rows[i]["middle_name"]);
                        checkdetail.Last_Name = Convert.ToString(dt.Rows[i]["last_name"]);
                        checkdetail.PersonalEmail = Convert.ToString(dt.Rows[i]["personalemail"]);
                        checkdetail.PersonalContactNo = Convert.ToString(dt.Rows[i]["personalcontactno"]);
                        checkdetail.Address = Convert.ToString(dt.Rows[i]["address"]) ?? "";
                        checkdetail.SSS_No = Convert.ToString(dt.Rows[i]["sss_no"]) ?? "";
                        checkdetail.Province = Convert.ToString(dt.Rows[i]["province"]) ?? "";
                        checkdetail.CompanyName = Convert.ToString(dt.Rows[i]["companyname"]) == null ? "" : Convert.ToString(dt.Rows[i]["companyname"]);
                        checkdetail.Designation = Convert.ToString(dt.Rows[i]["designation"]) == null ? "" : Convert.ToString(dt.Rows[i]["designation"]);
                        checkdetail.userfullname = Convert.ToString(dt.Rows[i]["userfullname"]) ?? "";
                        checkdetail.CreatedOn = Convert.ToString(dt.Rows[i]["createdon"]) ?? "";
                        checkdetail.CheckedOn = Convert.ToString(dt.Rows[i]["checkedon"]) ?? "";
                        checkdetail.ispickedverifier = Convert.ToString(dt.Rows[i]["ispickedverifier"]) != string.Empty ? Convert.ToBoolean(dt.Rows[i]["ispickedverifier"]) : false;

                        //Best Time to Morning Call
                        int BestTimeToCallMorning = Convert.ToString(dt.Rows[i]["morning_time"]).Trim() == string.Empty ? -1 : Convert.ToInt32(dt.Rows[i]["morning_time"]);
                        if (BestTimeToCallMorning != -1)
                            checkdetail.MorningTime = CommonMethods.GetBestTimeToCallMorning(BestTimeToCallMorning);
                        else
                            checkdetail.MorningTime = string.Empty;

                        //Best Time to Noon Call
                        int BestTimeToCallNoon = Convert.ToString(dt.Rows[i]["noon_time"]).Trim() == string.Empty ? -1 : Convert.ToInt32(dt.Rows[i]["noon_time"]);
                        if (BestTimeToCallNoon != -1)
                            checkdetail.NoonTime = CommonMethods.GetBestTimeToCallNoon(BestTimeToCallNoon);
                        else
                            checkdetail.NoonTime = string.Empty;

                        appRecordVM.Add(checkdetail);
                    }
                }
                var response = appRecordVM;

                jsonResult = Json(response, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
            }
            catch (Exception ex)
            {
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                jsonResult = Json(null, JsonRequestBehavior.AllowGet);
            }

            return jsonResult;
        }

        private string Upload(HttpPostedFileBase fileUpload, int ApplicationId, string OldFileName)
        {
            try
            {
                if (fileUpload != null && fileUpload.ContentLength > 0)
                {
                    if (!String.IsNullOrWhiteSpace(OldFileName))
                        logger.DeleteFileOnServer(new Uri(OldFileName));
                    string theFileName = Path.GetFileName(fileUpload.FileName);
                    byte[] thePictureAsBytes = new byte[fileUpload.ContentLength];
                    using (BinaryReader theReader = new BinaryReader(fileUpload.InputStream))
                    {
                        thePictureAsBytes = theReader.ReadBytes(fileUpload.ContentLength);
                    }
                    string thePictureDataAsString = Convert.ToBase64String(thePictureAsBytes);
                    String FinalFIleName = logger.UploadImageToFTP(thePictureDataAsString, $"{ApplicationId}", $"{Guid.NewGuid()}.{Path.GetExtension(fileUpload.FileName)}");
                    return FinalFIleName;
                }
                else
                {
                    return "";
                }
            }
            catch (Exception ex)
            {
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                throw ex;
            }
        }

        public string RemoveSpecialCharactersFile(string str)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                foreach (char c in str)
                {
                    if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || (c == '.'))
                    {
                        sb.Append(c);
                    }
                }

                return sb.ToString();
            }
            catch (Exception ex)
            {
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                throw ex;
            }

        }

        public string RemoveSpecialCharacters(string str)
        {
            try
            {
                if (String.IsNullOrWhiteSpace(str))
                {
                    return "";
                }
                StringBuilder sb = new StringBuilder();
                foreach (char c in str)
                {
                    if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || (c == '.') || (c == ' ') || (c == '!') || (c == '@') || (c == '#') || (c == '$') || (c == '%') || (c == '^') || (c == '&') || (c == '{') || (c == '}') || (c == '[') || (c == ']') || (c == '/') || (c == '<') || (c == '>'))
                    {
                        sb.Append(c);
                    }
                }
                return sb.ToString();
            }
            catch (Exception ex)
            {
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                throw ex;
            }

        }




        [HttpPost]
        public ActionResult GetLoanVerifierData()
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.Verifier))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            List<ApplicationRecordVM> model = new List<ApplicationRecordVM>();
            JsonResult result = new JsonResult();
            try
            {
                var RequestedForm = Request.Form;
                string search = String.Empty;
                int start = 10;
                int length = 10;
                string draw = String.Empty;
                string order = string.Empty;
                string orderDir = string.Empty;

                if (RequestedForm.Keys.Count > 0)
                {
                    search = Request.Form.GetValues("search[value]")[0];
                    start = Convert.ToInt32(Request["start"]);
                    length = Convert.ToInt32(Request["length"]);
                    draw = Request.Form.GetValues("draw")[0];
                    order = Request.Form.GetValues("order[0][column]")[0];
                    orderDir = Request.Form.GetValues("order[0][dir]")[0];
                }

                DataTable dt = DbHelper.SelectMethod(QueryHelper.GetApplicationforVerifier);
                if (dt != null && dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        ApplicationRecordVM checkdetail = new ApplicationRecordVM();
                        checkdetail.ApplicationNo = Convert.ToInt32(dt.Rows[i]["applicationno"].ToString());
                        checkdetail.First_Name = Convert.ToString(dt.Rows[i]["first_name"]);
                        checkdetail.Middle_Name = Convert.ToString(dt.Rows[i]["middle_name"]);
                        checkdetail.Last_Name = Convert.ToString(dt.Rows[i]["last_name"]);
                        checkdetail.PersonalEmail = Convert.ToString(dt.Rows[i]["personalemail"]);
                        checkdetail.PersonalContactNo = Convert.ToString(dt.Rows[i]["personalcontactno"]);
                        checkdetail.Address = Convert.ToString(dt.Rows[i]["address"]) == null ? "" : Convert.ToString(dt.Rows[i]["address"]);
                        checkdetail.SSS_No = Convert.ToString(dt.Rows[i]["sss_no"]) == null ? "" : Convert.ToString(dt.Rows[i]["sss_no"]);
                        checkdetail.userfullname = Convert.ToString(dt.Rows[i]["userfullname"]) == null ? "" : Convert.ToString(dt.Rows[i]["userfullname"]);
                        checkdetail.CreatedOn = Convert.ToString(dt.Rows[i]["createdon"]) == null ? "" : Convert.ToString(dt.Rows[i]["createdon"]);
                        checkdetail.CheckedOn = Convert.ToString(dt.Rows[i]["checkedon"]) == null ? "" : Convert.ToString(dt.Rows[i]["checkedon"]);
                        checkdetail.AppliedOn = Convert.ToDateTime(dt.Rows[i]["appliedon"]);
                        checkdetail.ForwardedOn = Convert.ToDateTime(dt.Rows[i]["forwardedon"]);
                        checkdetail.ispickedverifier = Convert.ToString(dt.Rows[i]["ispickedverifier"]) != string.Empty ? Convert.ToBoolean(dt.Rows[i]["ispickedverifier"]) : false;

                        //Best Time to Morning Call
                        int BestTimeToCallMorning = Convert.ToString(dt.Rows[i]["morning_time"]).Trim() == string.Empty ? -1 : Convert.ToInt32(dt.Rows[i]["morning_time"]);
                        if (BestTimeToCallMorning != -1)
                            checkdetail.MorningTime = CommonMethods.GetBestTimeToCallMorning(BestTimeToCallMorning);
                        else
                            checkdetail.MorningTime = string.Empty;

                        //Best Time to Noon Call
                        int BestTimeToCallNoon = Convert.ToString(dt.Rows[i]["noon_time"]).Trim() == string.Empty ? -1 : Convert.ToInt32(dt.Rows[i]["noon_time"]);
                        if (BestTimeToCallNoon != -1)
                            checkdetail.NoonTime = CommonMethods.GetBestTimeToCallNoon(BestTimeToCallNoon);
                        else
                            checkdetail.NoonTime = string.Empty;

                        model.Add(checkdetail);
                    }
                }

                int totalRecords = model.Count;
                if (!string.IsNullOrEmpty(search) &&
                !string.IsNullOrWhiteSpace(search))
                {
                    // Apply search 
                    model = model.Where(p => p.ApplicationNo.ToString().ToLower().Contains(search.ToLower()) ||
                    p.First_Name.ToString().ToLower().Contains(search.ToLower()) ||
                     p.Middle_Name.ToString().ToLower().Contains(search.ToLower()) ||
                      p.Last_Name.ToString().ToLower().Contains(search.ToLower()) ||
                        p.CheckedOn.ToString().ToLower().Contains(search.ToLower()) ||
                          p.MorningTime.ToString().ToLower().Contains(search.ToLower()) ||
                            p.NoonTime.ToString().ToLower().Contains(search.ToLower()) ||
                             p.userfullname.ToString().ToLower().Contains(search.ToLower()) ||
                       p.CreatedOn.ToString().ToLower().Contains(search.ToLower())).ToList();
                }

                model = this.SortByColumnWithOrder(order, orderDir, model);
                int recFilter = model.Count;
                model = model.Skip(start).Take(length).ToList();
                result = this.Json(new { draw = Convert.ToInt32(draw), recordsTotal = totalRecords, recordsFiltered = recFilter, data = model }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
            }
            return result;
        }

        public ActionResult GetLastPaymentRemark(int UserId, int ApplicationNo)
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.Verifier))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            JsonResult jsonResult = new JsonResult();
            try
            {
                DataTable dt = DbHelper.SelectMethod($"SELECT remarks FROM tblapplication_emi_payment WHERE application_no in (select applicationno from tblapplication_record where user_id = '{UserId}' and applicationno<>{ApplicationNo}) order by payment_id desc limit 1;");
                if (dt != null && dt.Rows.Count > 0)
                {
                    jsonResult = Json(Convert.ToString(dt.Rows[0]["remarks"]), JsonRequestBehavior.AllowGet);
                }
                else
                {
                    jsonResult = Json("No Remarks Found", JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                jsonResult = Json(null, JsonRequestBehavior.AllowGet);
            }
            return jsonResult;
        }

        private List<ApplicationRecordVM> SortByColumnWithOrder(string order, string orderDir, List<ApplicationRecordVM> ApplicationRecordList)
        {
            List<ApplicationRecordVM> lst = new List<ApplicationRecordVM>();
            try
            {
                switch (order)
                {
                    case "0":
                        lst = orderDir.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? ApplicationRecordList.OrderByDescending(p => p.ApplicationNo).ToList() : ApplicationRecordList.OrderBy(p => p.ApplicationNo).ToList();
                        break;
                    case "1":
                        lst = orderDir.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? ApplicationRecordList.OrderByDescending(p => p.First_Name).ToList() : ApplicationRecordList.OrderBy(p => p.First_Name).ToList();
                        break;
                    case "2":
                        lst = orderDir.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? ApplicationRecordList.OrderByDescending(p => p.AppliedOn).ToList() : ApplicationRecordList.OrderBy(p => p.AppliedOn).ToList();
                        break;
                    case "3":
                        lst = orderDir.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? ApplicationRecordList.OrderByDescending(p => p.ForwardedOn).ToList() : ApplicationRecordList.OrderBy(p => p.ForwardedOn).ToList();
                        break;
                    case "4":
                        lst = orderDir.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? ApplicationRecordList.OrderByDescending(p => p.MorningTime).ToList() : ApplicationRecordList.OrderBy(p => p.MorningTime).ToList();
                        break;
                    case "5":
                        lst = orderDir.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? ApplicationRecordList.OrderByDescending(p => p.NoonTime).ToList() : ApplicationRecordList.OrderBy(p => p.NoonTime).ToList();
                        break;
                    case "6":
                        lst = orderDir.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? ApplicationRecordList.OrderByDescending(p => p.userfullname).ToList() : ApplicationRecordList.OrderBy(p => p.userfullname).ToList();
                        break;
                    default:
                        lst = orderDir.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? ApplicationRecordList.OrderByDescending(p => p.ApplicationNo).ToList() : ApplicationRecordList.OrderBy(p => p.ApplicationNo).ToList();
                        break;
                }
            }
            catch (Exception ex)
            {
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
            }
            return lst;
        }

        [HttpPost]
        public bool CheckDuplicateApplicationNo(int appno)
        {
            try
            {
                var query = DbHelper.SelectMethod(string.Format(QueryHelper.CheckDuplicateApplicationNo, appno));
                if (query != null)
                {
                    if (query.Rows.Count > 0)
                    {
                        return false;
                    }

                }
                return true;
            }
            catch (Exception ex)
            {
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, MethodBase.GetCurrentMethod().Name));
                return true;
            }

        }
    }
}