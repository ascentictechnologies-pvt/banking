using DocumentFormat.OpenXml.Spreadsheet;
using Loan_CRM.Areas.Checker.Models;
using Loan_CRM.Areas.Collector.Models;
using Loan_CRM.Models;

using Microsoft.Owin;

using Serilog;

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Web.Mvc;


namespace Loan_CRM.Controllers
{
    public class HomeController : Controller
    {
        public HomeController()
        {

        }
        public string FileNameOnly { get; private set; }
        public string FullPath { get; private set; }

        /// <summary>
        /// user area and role defined
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Index()
        {
            try
            {
                //string folderName = "Content/PDF/";
                //folderName = Server.MapPath(folderName + "CM22A00007.pdf");
                //String _ContractUrl = CommonMethods.UploadContractPDF("", 0, $"CM22A00007.pdf");

                //if (System.IO.File.Exists(folderName))
                //{
                //    System.IO.File.Delete(folderName);
                //}

                var Keys = HttpContext.Request.Cookies.Keys;
                List<String> KeyList = new List<string>();
                foreach (var item in Keys)
                {
                    KeyList.Add($"{item}");
                }
                foreach (var item in KeyList)
                {
                    // Response.Cookies[$"{item}"].Expires = DateTime.Now.AddDays(-1);
                    Request.Cookies.Remove($"{item}");
                }

                if (!String.IsNullOrWhiteSpace(Convert.ToString(Request.QueryString["password"])) && !String.IsNullOrWhiteSpace(Convert.ToString(Request.QueryString["username"])) && !String.IsNullOrWhiteSpace(Convert.ToString(Request.QueryString["IPAddress"])))
                {

                    var username = Request.QueryString["username"] ?? "";
                    if (username.Contains("@"))
                    {
                        var _user = username.Split('@').ToList().First();
                        username = _user;
                    }
                    var password = CommonMethods.md5(Convert.ToString(Request.QueryString["password"] ?? ""));
                    var query = DbHelper.SelectMethod(string.Format(QueryHelper.GetUserRole, username, password));
                    if (query != null && query.Rows.Count > 0)
                    {
                        string rolename = Convert.ToString(query.Rows[0]["rolename"] == DBNull.Value ? "" : query.Rows[0]["rolename"]);
                        TempData["invild"] = "0";
                        HttpContext.Response.Cookies.Add(new HttpCookie(CookiesKey.UserId, $"{query.Rows[0]["userid"]}") { Name = CookiesKey.UserId, Expires = DateTime.Now.AddDays(1), SameSite = System.Web.SameSiteMode.None, Secure = true, Value = $"{query.Rows[0]["userid"]}" });
                        HttpContext.Response.Cookies.Add(new HttpCookie(CookiesKey.AgentName, $"{Request.QueryString["username"]}") { Name = CookiesKey.AgentName, Expires = DateTime.Now.AddDays(1), SameSite = System.Web.SameSiteMode.None, Secure = true, Value = $"{Request.QueryString["username"]}" });
                        HttpContext.Response.Cookies.Add(new HttpCookie(CookiesKey.RoleType, rolename) { Name = CookiesKey.RoleType, Expires = DateTime.Now.AddDays(1), SameSite = System.Web.SameSiteMode.None, Secure = true, Value = rolename });
                        HttpContext.Response.Cookies.Add(new HttpCookie(CookiesKey.RoleId, $"{query.Rows[0]["webadminrole"]}") { Name = CookiesKey.RoleId, Expires = DateTime.Now.AddDays(1), SameSite = System.Web.SameSiteMode.None, Secure = true, Value = $"{query.Rows[0]["webadminrole"]}" });
                        HttpContext.Response.Cookies.Add(new HttpCookie(CookiesKey.IPAddress, $"{Request.QueryString["IPAddress"]}") { Name = CookiesKey.IPAddress, Expires = DateTime.Now.AddDays(1), SameSite = System.Web.SameSiteMode.None, Secure = true, Value = $"{Request.QueryString["IPAddress"]}" });
                        switch (rolename.ToLower())
                        {
                            case RoleType.Checker:
                                return RedirectToAction("LoanRequest", "LoanRequestBucket", new { area = "Checker" });
                            case RoleType.Accounts:
                                return RedirectToAction("GetAccountApproverData", "AccountApprover", new { area = "Account" });
                            case RoleType.AdminUser:
                                return RedirectToAction("CustomerInfoBucket", "Admin", new { area = "Admin" });
                            case RoleType.Approver:
                                return RedirectToAction("ApproverLoanRequestBucket", "Approver", new { area = "Approver" });
                            case RoleType.Collector:
                                return RedirectToAction("LoanPaymentBucket", "Loanoverdue", new { area = "Collector" });
                            case RoleType.Verifier:
                                return RedirectToAction("Index", "LoanRequestVerifier", new { area = "Verifier" });
                            case RoleType.disburser:
                                return RedirectToAction("DisbursementList", "Disbursement", new { area = "Account" });
                            case RoleType.TLSales:
                                return RedirectToAction("TLSales", "Loanoverdue", new { area = "Collector" });
                            case RoleType.TLDefaulter:
                                return RedirectToAction("TLReport", "Loanoverdue", new { area = "Collector" });
                            case RoleType.Defaulter:
                                return RedirectToAction("DefaulterBucket", "Loanoverdue", new { area = "Collector" });
                            case RoleType.StatusUser:
                                return RedirectToAction("GetApplicationStatus", "StatusUser", new { area = "StatusUser" });
                            default:
                                TempData["invild"] = "1";
                                break;
                        }
                    }
                    else
                    {
                        TempData["invild"] = "1";
                        return View();
                    }
                }
                else
                {
                    TempData["invild"] = "2";
                    return View();
                }
            }
            catch (Exception ex)
            {
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                TempData["invild"] = "0";
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
            }
            return View();
        }

        /// <summary>
        /// ApplicationRecord user details 2nd page
        /// created by priety
        /// 29/03/2019
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ApplicationRecord()
        {
            ApplicationRecordVM obj = new ApplicationRecordVM();
            try
            {
                ViewBag.CityData = CommonMethods.getCity();
                ViewBag.CivilStatusData = CommonMethods.getCivilStatus();
                ViewBag.TermTypeData = CommonMethods.getTermType();
                ViewBag.OccupationData = CommonMethods.getOccupation();
            }
            catch (Exception ex)
            {
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
            }
            return View(obj);
        }

        [HttpPost]
        [Obsolete]
        public ActionResult ApplicationRecord(ApplicationRecordVM model, string personalMail, string name, string persoanlContactno)
        {
            try
            {

                if (model.ApplicationNo > 0)
                {
                    logger.CreateFolderFTP($"ApplicationNo{Convert.ToString(model.ApplicationNo)}");
                    DataTable OrrSSSNo = new DataTable();
                    DataTable OrrOccupation = new DataTable();
                    DataTable OrrBarangay = new DataTable();
                    bool is_orr_sssno = false;
                    bool is_orr_occupation = false;
                    bool is_orr_barangay = false;
                    int AppId = Convert.ToInt32(model.ApplicationNo);
                    string GovtFileName = Upload(model.GovIdFile, AppId);
                    string CompanyFileName = Upload(model.CompanyIdFile, AppId);
                    string BillingFileName = Upload(model.BillingIdFile, AppId);
                    string IncomeFileName = Upload(model.IncomeIdFile, AppId);
                    string OtherFileName = Upload(model.OtherIdFile, AppId);
                    string ATMDetailsFileName = Upload(model.AtmIdFile, AppId);

                    ViewBag.CityData = CommonMethods.getCity();
                    ViewBag.CivilStatusData = CommonMethods.getCivilStatus();
                    ViewBag.TermTypeData = CommonMethods.getTermType();
                    ViewBag.IndustryData = CommonMethods.getOccupation();

                    string Data = Convert.ToString(model.GrossIncome);
                    Data = RemoveSpecialCharacters(Data);
                    model.GrossIncome = Convert.ToDouble(Data);
                    Data = Convert.ToString(model.Loan_Amount);
                    Data = RemoveSpecialCharacters(Data);
                    model.Loan_Amount = Convert.ToDouble(Data);

                    logger.WriteErrorLogs($"Application No : {model.ApplicationNo} =====> Term Name:{model.Term} ===>  Term Type:{model.TermType}   ==> TermText:{model.TermText}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                    if (model.TermType > 0 && model.TermText > 0)
                    {
                        int i = DbHelper.InsertUpdateDelete(string.Format(QueryHelper.Updatetblapplication_Record, RemoveSpecialCharacters(model.Address), model.City, Convert.ToDateTime(model.DOB).ToString("yyyy-MM-dd"), model.CivilStatus, RemoveSpecialCharacters(model.CompanyName), RemoveSpecialCharacters(model.CompanyAddress), RemoveSpecialCharacters(model.Company_Phoneno), Convert.ToDateTime(model.JoinDate).ToString("yyyy-MM-dd"), RemoveSpecialCharacters(model.Designation), model.GrossIncome, model.PayDate1, model.PayDate2, model.Loan_Amount, model.TermType, model.TermText, model.BestTimeToCallMorning_Id, model.BestTimeToCallNoon_Id, GovtFileName ?? "", CompanyFileName ?? "", BillingFileName ?? "", IncomeFileName ?? "", OtherFileName ?? "", AppId, model.Barangay_id, RemoveSpecialCharacters(model.SSS_No), model.province_id, model.Occupation, ATMDetailsFileName ?? "", model.gender));
                        if (i > 0)
                        {
                            #region Is ORR Rejection SMS and Email
                            var application_data = DbHelper.SelectMethod(string.Format(QueryHelper.GetApplicationDetails, AppId));
                            if (application_data != null)
                            {
                                model.ApplicationNo = Convert.ToString(application_data.Rows[0]["applicationno"]) != string.Empty ? Convert.ToInt32(application_data.Rows[0]["applicationno"]) : 0;
                                model.First_Name = Convert.ToString(application_data.Rows[0]["first_name"]) != string.Empty ? Convert.ToString(application_data.Rows[0]["first_name"]) : string.Empty;
                                model.Middle_Name = Convert.ToString(application_data.Rows[0]["middle_name"]) != string.Empty ? Convert.ToString(application_data.Rows[0]["middle_name"]) : string.Empty;
                                model.Last_Name = Convert.ToString(application_data.Rows[0]["last_name"]) != string.Empty ? Convert.ToString(application_data.Rows[0]["last_name"]) : string.Empty;
                                model.PersonalEmail = Convert.ToString(application_data.Rows[0]["personalemail"]) != string.Empty ? Convert.ToString(application_data.Rows[0]["personalemail"]) : string.Empty;
                                model.PersonalContactNo = Convert.ToString(application_data.Rows[0]["personalcontactno"]) != string.Empty ? Convert.ToString(application_data.Rows[0]["personalcontactno"]) : string.Empty;

                            }
                            if (model.SSS_No != null && model.SSS_No != "")
                            {
                                string IsOrrSSSNo = string.Format(QueryHelper.IsOrrSSSNo, model.SSS_No);
                                OrrSSSNo = DbHelper.SelectMethod(IsOrrSSSNo);
                            }
                            if (model.Barangay_id != 0)
                            {
                                string IsOrrBarangay = string.Format(QueryHelper.IsOrrBarangay, model.Barangay_id);
                                OrrBarangay = DbHelper.SelectMethod(IsOrrBarangay);
                            }
                            if (model.Occupation != 0)
                            {
                                string IsOrrOccupation = string.Format(QueryHelper.IsOrrOccupation, model.Occupation);
                                OrrOccupation = DbHelper.SelectMethod(IsOrrOccupation);
                            }
                            if ((OrrSSSNo != null && OrrSSSNo.Rows.Count > 0))
                            {
                                if ((Convert.ToInt32(OrrSSSNo.Rows[0][0]) > 0))
                                {
                                    is_orr_sssno = true;
                                }
                            }
                            if ((OrrOccupation != null && OrrOccupation.Rows.Count > 0))
                            {
                                if ((Convert.ToInt32(OrrOccupation.Rows[0][0]) > 0))
                                {
                                    is_orr_occupation = true;
                                }
                            }
                            if ((OrrBarangay != null && OrrBarangay.Rows.Count > 0))
                            {
                                if ((Convert.ToInt32(OrrBarangay.Rows[0][0]) > 0))
                                {
                                    is_orr_barangay = true;
                                }
                            }
                            if (is_orr_occupation || is_orr_sssno || is_orr_barangay)
                            {
                                String _OrrDeclineRemarks = String.Empty;
                                if (is_orr_barangay)
                                {
                                    _OrrDeclineRemarks = "Decline due to Barangay exist into ORR List";
                                }
                                else if (is_orr_sssno)
                                {
                                    _OrrDeclineRemarks = "Decline due to SSS No exist into ORR List";
                                }
                                else
                                {
                                    _OrrDeclineRemarks = "Decline due to Occupation exist into ORR List";
                                }

                                #region Update Is_Orr_Declined and Is_Deleted
                                int query_result = DbHelper.InsertUpdateDelete(string.Format(QueryHelper.UpdateIsOrrDeclined, model.ApplicationNo, _OrrDeclineRemarks));
                                if (query_result > 0)
                                {
                                    #region SendRejectionSMS
                                    try
                                    {
                                        string SMSBody = "Hi " + model.First_Name + " " + model.Middle_Name + " " + model.Last_Name + "," + Convert.ToString(ConfigurationManager.AppSettings["RejectionSMS"]);
                                        if (model.PersonalContactNo[0] != '0' && model.PersonalContactNo.Length == 10)
                                        {
                                            model.PersonalContactNo = '0' + model.PersonalContactNo;
                                        }
                                        new Thread(() => logger.ClicktoSMS(model.PersonalContactNo, SMSBody)).Start();
                                    }
                                    catch (Exception ex)
                                    {
                                        logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                                        throw ex;
                                    }
                                    #endregion

                                    #region SendRejectionEmail
                                    new Thread(() => CommonMethods.SendMail(EmailFrom.FromInfo, model.PersonalEmail, Convert.ToString(ConfigurationManager.AppSettings["RejectionMail"]), "Loan Rejection")).Start();
                                    #endregion
                                }

                                #endregion

                            }
                            #endregion

                            return RedirectToAction("ThankYou", "Home");
                        }
                        else
                        {
                            return View(model);
                        }
                    }
                    else
                    {
                        return View(model);
                    }
                }
                else
                {
                    TempData["msg"] = string.Format($"Application Id Not Generated");
                    return RedirectToAction("ApplicationRecord");
                }
            }
            catch (Exception ex)
            {
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                return View(model);
            }

        }

        /// <summary>
        /// OTP Verification
        /// created by priety
        /// 29/03/2019
        /// </summary>
        /// <param name="code"></param>
        /// <param name="PersonalNO"></param>
        /// <returns></returns>
        public ActionResult VerifyCode(string code, string PersonalNO)
        {
            try
            {
                var dt = DbHelper.SelectMethod(string.Format(QueryHelper.GetOTPDetails, code, PersonalNO));
                if (dt.Rows.Count > 0 && dt != null)
                {
                    return Json(true, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(false, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                return Json(null, JsonRequestBehavior.AllowGet);
            }

        }

        /// <summary>
        /// Upload all file
        /// </summary>
        /// <param name="fileUpload"></param>
        /// <param name="ApplicationId"></param>
        /// <returns></returns>
        private string Upload(HttpPostedFileBase fileUpload, int ApplicationId)
        {
            try
            {
                if (fileUpload != null && fileUpload.ContentLength > 0)
                {
                    string theFileName = Path.GetFileName(fileUpload.FileName);
                    byte[] thePictureAsBytes = new byte[fileUpload.ContentLength];
                    using (BinaryReader theReader = new BinaryReader(fileUpload.InputStream))
                    {
                        thePictureAsBytes = theReader.ReadBytes(fileUpload.ContentLength);
                    }
                    string thePictureDataAsString = Convert.ToBase64String(thePictureAsBytes);
                    String FinalFIleName = logger.UploadImageToFTP(thePictureDataAsString, $"{ApplicationId}", $"{Guid.NewGuid()}{Path.GetExtension(fileUpload.FileName)}");
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

        /// <summary>
        /// ApplicationRecord user details 1st page
        /// created by priety
        /// 29/03/2019
        /// </summary>
        /// <param name="FirstName"></param>
        /// <param name="MiddleName"></param>
        /// <param name="LastName"></param>
        /// <param name="suffix"></param>
        /// <param name="personalMail"></param>
        /// <param name="persoanlContactno"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        [HttpGet]
        public int InsertPersonalRecord(string FirstName, string MiddleName, string LastName, string suffix, string personalMail, string persoanlContactno, string password)
        {
            try
            {
                Regex reg = new Regex(@"^[0-9]{11}$");
                FirstName = RemoveSpecialCharacters(FirstName).ToUpper();
                MiddleName = RemoveSpecialCharacters(MiddleName).ToUpper();
                LastName = RemoveSpecialCharacters(LastName).ToUpper();
                persoanlContactno = RemoveSpecialCharacters(persoanlContactno);
                if (reg.IsMatch(persoanlContactno))
                {
                    Random random = new Random();
                    string destinationaddr = persoanlContactno;
                    int ApplicationNumber = 0;

                    //To Get Lead id.
                    if (Convert.ToString(ConfigurationManager.AppSettings["EnableLeadUPloadNGUCC"]) == "1")
                    {

                        string sURL = $"{ConfigurationManager.AppSettings["LeadId_URL"]}&domainname={ConfigurationManager.AppSettings["LeadId_domain"]}&username={ConfigurationManager.AppSettings["LeadId_username"]}&password={ConfigurationManager.AppSettings["LeadId_password"]}&campname={ConfigurationManager.AppSettings["LeadId_campname"]}&phone1={persoanlContactno}&skillname={ConfigurationManager.AppSettings["LeadId_skillname"]}&listname={ConfigurationManager.AppSettings["LeadId_listname"]}&status=NEW&qname={ConfigurationManager.AppSettings["LeadId_queuename"]}";
                        logger.WriteErrorLogs(sURL, String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                        WebRequest wrGETURL;
                        wrGETURL = WebRequest.Create(sURL);
                        WebProxy myProxy = new WebProxy("myproxy", 80)
                        {
                            BypassProxyOnLocal = true
                        };
                        Stream objStream;
                        objStream = wrGETURL.GetResponse().GetResponseStream();
                        StreamReader objReader = new StreamReader(objStream);
                        string sLine = "";
                        int i = 0;

                        while (sLine != null)
                        {
                            i++;
                            sLine = objReader.ReadLine();

                            if (sLine != null)
                            {
                                logger.WriteErrorLogs($"Lead URL : {sURL} and Response : {sLine}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                            }
                        }
                    }

                    string pass = Encrypt_Decrypt.EncryptDecrypt.Encrypt(password);
                    var data1 = DbHelper.SelectMethod(string.Format(QueryHelper.InsertApplicantDetails, FirstName, MiddleName, LastName, personalMail, pass, persoanlContactno));
                    int j = 0;
                    if (data1 != null && data1.Rows.Count > 0)
                    {
                        j = Convert.ToInt32(data1.Rows[0]["currval"]);
                        logger.WriteErrorLogs($"User Id  : {j}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                    }
                    if (j > 0)
                    {
                        string query = string.Format(QueryHelper.InsertApplication_RecordNew, j);
                        var res = DbHelper.SelectMethod(query);
                        if (res != null && res.Rows.Count > 0)
                        {
                            ApplicationNumber = Convert.ToInt32(res.Rows[0]["currval"]);
                            logger.WriteErrorLogs($"Application Number  : {ApplicationNumber}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));


                            int value = random.Next(10001, 99999);
                            //string message = "Your OTP Number is " + value + " (Sent By : Cashmart) This OTP is valid for 15 minutes";
                            string message = $"Cash Mart has sent an OTP code, {value}, which remains valid for a duration of 15 minutes";
                            //send sms function call
                            new Thread(() => logger.ClicktoSMS(persoanlContactno, message)).Start();
                            // Insert the OTP Details ( OTP,Mobile No) into the  tblotp.
                            DbHelper.InsertUpdateDelete(string.Format(QueryHelper.InsertOTPDetails, persoanlContactno, value));
                        }
                        else
                        {
                            return -1;
                        }
                        return ApplicationNumber;
                    }
                    else
                    {
                        return 0;
                    }
                }
                else
                {
                    return 0;
                }

            }
            catch (Exception ex)
            {
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                return -1;
            }
        }

        /// <summary>
        /// SendEmail after record successfully inserted 
        /// created by priety
        /// 29/03/2019
        /// </summary>
        /// <param name="personalMail"></param>
        /// <param name="name"></param>
        /// <param name="persoanlContactno"></param>
        [Obsolete]
        public void SendEmail(string personalMail, string name, string persoanlContactno)
        {
            try
            {
                StringBuilder EmailBody = new StringBuilder();
                EmailBody.AppendLine("<p><strong>Name: </strong>" + name + "</p>");
                EmailBody.AppendLine("<p><strong>Email ID: </strong>" + personalMail + "</p>");
                EmailBody.AppendLine("<p><strong>Contact No: </strong>" + persoanlContactno + "</p>");
                new Thread(() => CommonMethods.SendMail(EmailFrom.FromInfo, personalMail, Convert.ToString(EmailBody), "Loan Application Details")).Start();
                TempData["Success"] = "Please Check your MailID.";
            }
            catch (Exception ex)
            {
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
            }
        }

        /// <summary>
        /// ViewApplicationRecord update user details
        /// created by priety
        /// 29/03/2019
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ViewApplicationRecord()
        {
            try
            {
                if (Session["UserId"] != null)
                {
                    int UserId = Convert.ToInt32(Session["UserId"]);
                    int applicationno = 0;
                    ApplicationRecordVM appRecordVM = new ApplicationRecordVM();
                    var GetAppNo = DbHelper.SelectMethod(string.Format(QueryHelper.GetUserDetailsById, UserId));
                    if (GetAppNo != null && GetAppNo.Rows.Count > 0)
                    {
                        applicationno = Convert.ToInt32(GetAppNo.Rows[0]["applicationno"]);
                    }
                    Session["Application_No"] = applicationno;
                    ViewBag.CityData = CommonMethods.getCity();
                    ViewBag.CivilStatusData = CommonMethods.getCivilStatus();
                    ViewBag.TermTypeDate = CommonMethods.getTermType();
                    ViewBag.OccupationData = CommonMethods.getOccupation();

                    DataTable dt = DbHelper.SelectMethod(string.Format(QueryHelper.GetApplicationdetails, applicationno));
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        string URL_Gov = (Convert.ToString(dt.Rows[0]["gov_id_url"]) != string.Empty ? Convert.ToString(dt.Rows[0]["gov_id_url"]) : string.Empty);
                        string URL_Company = (Convert.ToString(dt.Rows[0]["companyid_url"]) != string.Empty ? Convert.ToString(dt.Rows[0]["companyid_url"]) : string.Empty);
                        string URL_Billing = (Convert.ToString(dt.Rows[0]["billing_url"]) != string.Empty ? Convert.ToString(dt.Rows[0]["billing_url"]) : string.Empty);
                        string URL_Income = (Convert.ToString(dt.Rows[0]["income_url"]) != string.Empty ? Convert.ToString(dt.Rows[0]["income_url"]) : string.Empty);
                        string URL_other = (Convert.ToString(dt.Rows[0]["other_url"]) != string.Empty ? Convert.ToString(dt.Rows[0]["other_url"]) : string.Empty);
                        string ATM_URL = (Convert.ToString(dt.Rows[0]["atm_url"]) != string.Empty ? Convert.ToString(dt.Rows[0]["atm_url"]) : string.Empty);
                        appRecordVM.First_Name = Convert.ToString(dt.Rows[0]["first_name"]) != string.Empty ? Convert.ToString(dt.Rows[0]["first_name"]) : string.Empty;
                        appRecordVM.Middle_Name = Convert.ToString(dt.Rows[0]["middle_name"]) != string.Empty ? Convert.ToString(dt.Rows[0]["middle_name"]) : string.Empty;
                        appRecordVM.Last_Name = Convert.ToString(dt.Rows[0]["last_name"]) != string.Empty ? Convert.ToString(dt.Rows[0]["last_name"]) : string.Empty;
                        appRecordVM.PersonalEmail = Convert.ToString(dt.Rows[0]["personalemail"]) != string.Empty ? Convert.ToString(dt.Rows[0]["personalemail"]) : string.Empty;
                        appRecordVM.PersonalContactNo = Convert.ToString(dt.Rows[0]["personalcontactno"]) != string.Empty ? Convert.ToString(dt.Rows[0]["personalcontactno"]) : string.Empty;
                        appRecordVM.Address = Convert.ToString(dt.Rows[0]["address"]) != string.Empty ? Convert.ToString(dt.Rows[0]["address"]) : string.Empty;
                        appRecordVM.Province = Convert.ToString(dt.Rows[0]["province_name"]) != string.Empty ? Convert.ToString(dt.Rows[0]["province_name"]) : string.Empty;
                        appRecordVM.province_id = Convert.ToString(dt.Rows[0]["province_id"]) != string.Empty ? Convert.ToInt32(dt.Rows[0]["province_id"]) : 0;
                        appRecordVM.SSS_No = Convert.ToString(dt.Rows[0]["sss_no"]) != string.Empty ? Convert.ToString(dt.Rows[0]["sss_no"]) : string.Empty;
                        appRecordVM.Barangay = Convert.ToString(dt.Rows[0]["barangay_name"]) != string.Empty ? Convert.ToString(dt.Rows[0]["barangay_name"]) : string.Empty;
                        appRecordVM.Barangay_id = Convert.ToString(dt.Rows[0]["id"]) != string.Empty ? Convert.ToInt32(dt.Rows[0]["id"]) : 0;
                        appRecordVM.Occupation = Convert.ToString(dt.Rows[0]["industry"]) != string.Empty ? Convert.ToInt32(dt.Rows[0]["industry"]) : -1;
                        appRecordVM.CompanyName = Convert.ToString(dt.Rows[0]["companyname"]) != string.Empty ? Convert.ToString(dt.Rows[0]["companyname"]) : string.Empty;
                        appRecordVM.Designation = Convert.ToString(dt.Rows[0]["designation"]) != string.Empty ? Convert.ToString(dt.Rows[0]["designation"]) : string.Empty;
                        appRecordVM.DOB = Convert.ToString(dt.Rows[0]["dateofbirth"]) != string.Empty ? Convert.ToString(dt.Rows[0]["dateofbirth"]) : string.Empty;
                        appRecordVM.termAccepted = Convert.ToBoolean(dt.Rows[0]["termaccepted"]);
                        appRecordVM.CivilStatus = Convert.ToString(dt.Rows[0]["civilstatus"]) != string.Empty ? Convert.ToInt32(dt.Rows[0]["civilstatus"]) : -1;
                        appRecordVM.CompanyAddress = Convert.ToString(dt.Rows[0]["companyaddress"]) != string.Empty ? Convert.ToString(dt.Rows[0]["companyaddress"]) : string.Empty;
                        appRecordVM.GrossIncome = Convert.ToString(dt.Rows[0]["gross_income"]) != string.Empty ? Convert.ToDouble(dt.Rows[0]["gross_income"]) : 0.00D;
                        appRecordVM.City_Name = Convert.ToString(dt.Rows[0]["cityname"]) != string.Empty ? Convert.ToString(dt.Rows[0]["cityname"]) : string.Empty;
                        appRecordVM.City = Convert.ToString(dt.Rows[0]["cityid"]) != string.Empty ? Convert.ToInt32(dt.Rows[0]["cityid"]) : 0;
                        appRecordVM.GovUrl = URL_Gov;
                        appRecordVM.CompUrl = URL_Company;
                        appRecordVM.BillUrl = URL_Billing;
                        appRecordVM.IncomeUrl = URL_Income;
                        appRecordVM.OthUrl = URL_other;
                        appRecordVM.AtmUrl = ATM_URL;
                        appRecordVM.Street = Convert.ToString(dt.Rows[0]["street"]) != string.Empty ? Convert.ToString(dt.Rows[0]["street"]) : string.Empty;
                        appRecordVM.Company_Phoneno = Convert.ToString(dt.Rows[0]["company_phoneno"]) != string.Empty ? Convert.ToString(dt.Rows[0]["company_phoneno"]) : string.Empty;
                        appRecordVM.Loan_Amount = Convert.ToString(dt.Rows[0]["loanamount"]) != string.Empty ? Convert.ToDouble(dt.Rows[0]["loanamount"]) : 0.00D;
                        appRecordVM.JoinDate = Convert.ToString(dt.Rows[0]["dateofjoining"]) != string.Empty ? Convert.ToString(dt.Rows[0]["dateofjoining"]) : string.Empty;
                        appRecordVM.GovId_FileName = Convert.ToString(dt.Rows[0]["gov_id_url"]) != string.Empty ? Convert.ToString(dt.Rows[0]["gov_id_url"]) : string.Empty;
                        appRecordVM.CompanyId_FileName = Convert.ToString(dt.Rows[0]["companyid_url"]) != string.Empty ? Convert.ToString(dt.Rows[0]["companyid_url"]) : string.Empty;
                        appRecordVM.BillingId_FileName = Convert.ToString(dt.Rows[0]["billing_url"]) != string.Empty ? Convert.ToString(dt.Rows[0]["billing_url"]) : string.Empty;
                        appRecordVM.Income_FileName = Convert.ToString(dt.Rows[0]["income_url"]) != string.Empty ? Convert.ToString(dt.Rows[0]["income_url"]) : string.Empty;
                        appRecordVM.OtherFileName = Convert.ToString(dt.Rows[0]["other_url"]) != string.Empty ? Convert.ToString(dt.Rows[0]["other_url"]) : string.Empty;
                        appRecordVM.Atm_File_Name = Convert.ToString(dt.Rows[0]["atm_url"]) != string.Empty ? Convert.ToString(dt.Rows[0]["atm_url"]) : string.Empty;
                        Session["GovId_FileName"] = Convert.ToString(dt.Rows[0]["gov_id_url"]) != string.Empty ? Convert.ToString(dt.Rows[0]["gov_id_url"]) : string.Empty;
                        Session["CompanyId_FileName"] = Convert.ToString(dt.Rows[0]["companyid_url"]) != string.Empty ? Convert.ToString(dt.Rows[0]["companyid_url"]) : string.Empty;
                        Session["BillingId_FileName"] = Convert.ToString(dt.Rows[0]["billing_url"]) != string.Empty ? Convert.ToString(dt.Rows[0]["billing_url"]) : string.Empty;
                        Session["Income_FileName"] = Convert.ToString(dt.Rows[0]["income_url"]) != string.Empty ? Convert.ToString(dt.Rows[0]["income_url"]) : string.Empty;
                        Session["OtherFileName"] = Convert.ToString(dt.Rows[0]["other_url"]) != string.Empty ? Convert.ToString(dt.Rows[0]["other_url"]) : string.Empty;
                        Session["Atm_File_Name"] = Convert.ToString(dt.Rows[0]["atm_url"]) != string.Empty ? Convert.ToString(dt.Rows[0]["atm_url"]) : string.Empty;
                        appRecordVM.PayDate1 = Convert.ToString(dt.Rows[0]["paydate1"]) != string.Empty ? Convert.ToInt32(dt.Rows[0]["paydate1"]) : -1;
                        appRecordVM.PayDate2 = Convert.ToString(dt.Rows[0]["paydate2"]) != string.Empty ? Convert.ToInt32(dt.Rows[0]["paydate2"]) : -1;
                        appRecordVM.TermType = Convert.ToString(dt.Rows[0]["termtype"]) != string.Empty ? Convert.ToInt32(dt.Rows[0]["termtype"]) : -1;
                        appRecordVM.TermText = Convert.ToString(dt.Rows[0]["term"]) != string.Empty ? Convert.ToInt32(dt.Rows[0]["term"]) : -1;
                        appRecordVM.BestTimeToCallMorning_Id = Convert.ToString(dt.Rows[0]["morning_time"]) != string.Empty ? Convert.ToInt32(dt.Rows[0]["morning_time"]) : -1;
                        appRecordVM.BestTimeToCallNoon_Id = Convert.ToString(dt.Rows[0]["noon_time"]) != string.Empty ? Convert.ToInt32(dt.Rows[0]["noon_time"]) : -1;
                        ViewData["NoRecordMsg"] = "Record Found";
                        if (Convert.ToBoolean(dt.Rows[0]["ischeck"]) && Convert.ToBoolean(dt.Rows[0]["isverified"]) && !Convert.ToBoolean(dt.Rows[0]["isrecheck"]))
                        {
                            ViewBag.noneditable = true;
                        }
                        else
                        {
                            ViewBag.noneditable = false;
                        }
                    }
                    else
                    {
                        DataTable dt1 = DbHelper.SelectMethod(string.Format(QueryHelper.GetApplicationdetailsSecond, applicationno));
                        if (dt1 != null && dt1.Rows.Count > 0)
                        {
                            string URL_Gov = (Convert.ToString(dt1.Rows[0]["gov_id_url"]) != string.Empty ? Convert.ToString(dt1.Rows[0]["gov_id_url"]) : string.Empty);
                            string URL_Company = (Convert.ToString(dt1.Rows[0]["companyid_url"]) != string.Empty ? Convert.ToString(dt1.Rows[0]["companyid_url"]) : string.Empty);
                            string URL_Billing = (Convert.ToString(dt1.Rows[0]["billing_url"]) != string.Empty ? Convert.ToString(dt1.Rows[0]["billing_url"]) : string.Empty);
                            string URL_Income = (Convert.ToString(dt1.Rows[0]["income_url"]) != string.Empty ? Convert.ToString(dt1.Rows[0]["income_url"]) : string.Empty);
                            string URL_other = (Convert.ToString(dt1.Rows[0]["other_url"]) != string.Empty ? Convert.ToString(dt1.Rows[0]["other_url"]) : string.Empty);
                            string URL_ATM = (Convert.ToString(dt1.Rows[0]["atm_url"]) != string.Empty ? Convert.ToString(dt1.Rows[0]["atm_url"]) : string.Empty);
                            appRecordVM.First_Name = Convert.ToString(dt.Rows[0]["first_name"]) != string.Empty ? Convert.ToString(dt.Rows[0]["first_name"]) : string.Empty;
                            appRecordVM.Middle_Name = Convert.ToString(dt.Rows[0]["middle_name"]) != string.Empty ? Convert.ToString(dt.Rows[0]["middle_name"]) : string.Empty;
                            appRecordVM.Last_Name = Convert.ToString(dt.Rows[0]["last_name"]) != string.Empty ? Convert.ToString(dt.Rows[0]["last_name"]) : string.Empty;
                            appRecordVM.PersonalEmail = Convert.ToString(dt.Rows[0]["personalemail"]) != string.Empty ? Convert.ToString(dt.Rows[0]["personalemail"]) : string.Empty;
                            appRecordVM.PersonalContactNo = Convert.ToString(dt.Rows[0]["personalcontactno"]) != string.Empty ? Convert.ToString(dt.Rows[0]["personalcontactno"]) : string.Empty;
                            appRecordVM.Address = Convert.ToString(dt.Rows[0]["address"]) != string.Empty ? Convert.ToString(dt.Rows[0]["address"]) : string.Empty;
                            appRecordVM.Province = Convert.ToString(dt.Rows[0]["province_name"]) != string.Empty ? Convert.ToString(dt.Rows[0]["province_name"]) : string.Empty;
                            appRecordVM.province_id = Convert.ToString(dt.Rows[0]["province_id"]) != string.Empty ? Convert.ToInt32(dt.Rows[0]["province_id"]) : 0;
                            appRecordVM.SSS_No = Convert.ToString(dt.Rows[0]["sss_no"]) != string.Empty ? Convert.ToString(dt.Rows[0]["sss_no"]) : string.Empty;
                            appRecordVM.Barangay = Convert.ToString(dt.Rows[0]["barangay_name"]) != string.Empty ? Convert.ToString(dt.Rows[0]["barangay_name"]) : string.Empty;
                            appRecordVM.Barangay_id = Convert.ToString(dt.Rows[0]["id"]) != string.Empty ? Convert.ToInt32(dt.Rows[0]["id"]) : 0;
                            appRecordVM.Occupation = Convert.ToString(dt.Rows[0]["industry"]) != string.Empty ? Convert.ToInt32(dt.Rows[0]["industry"]) : -1;
                            appRecordVM.CompanyName = Convert.ToString(dt.Rows[0]["companyname"]) != string.Empty ? Convert.ToString(dt.Rows[0]["companyname"]) : string.Empty;
                            appRecordVM.Designation = Convert.ToString(dt.Rows[0]["designation"]) != string.Empty ? Convert.ToString(dt.Rows[0]["designation"]) : string.Empty;
                            appRecordVM.DOB = Convert.ToString(dt.Rows[0]["dateofbirth"]) != string.Empty ? Convert.ToString(dt.Rows[0]["dateofbirth"]) : string.Empty;
                            appRecordVM.termAccepted = Convert.ToBoolean(dt.Rows[0]["termaccepted"]);
                            appRecordVM.CivilStatus = Convert.ToString(dt.Rows[0]["civilstatus"]) != string.Empty ? Convert.ToInt32(dt.Rows[0]["civilstatus"]) : -1;
                            appRecordVM.CompanyAddress = Convert.ToString(dt.Rows[0]["companyaddress"]) != string.Empty ? Convert.ToString(dt.Rows[0]["companyaddress"]) : string.Empty;
                            appRecordVM.GrossIncome = Convert.ToString(dt.Rows[0]["gross_income"]) != string.Empty ? Convert.ToDouble(dt.Rows[0]["gross_income"]) : 0.00D;
                            appRecordVM.City_Name = Convert.ToString(dt.Rows[0]["cityname"]) != string.Empty ? Convert.ToString(dt.Rows[0]["cityname"]) : string.Empty;
                            appRecordVM.City = Convert.ToString(dt.Rows[0]["cityid"]) != string.Empty ? Convert.ToInt32(dt.Rows[0]["cityid"]) : 0;
                            appRecordVM.GovUrl = URL_Gov;
                            appRecordVM.CompUrl = URL_Company;
                            appRecordVM.BillUrl = URL_Billing;
                            appRecordVM.IncomeUrl = URL_Income;
                            appRecordVM.OthUrl = URL_other;
                            appRecordVM.AtmUrl = URL_ATM;
                            appRecordVM.Street = Convert.ToString(dt.Rows[0]["street"]) != string.Empty ? Convert.ToString(dt.Rows[0]["street"]) : string.Empty;
                            appRecordVM.Company_Phoneno = Convert.ToString(dt.Rows[0]["company_phoneno"]) != string.Empty ? Convert.ToString(dt.Rows[0]["company_phoneno"]) : string.Empty;
                            appRecordVM.Loan_Amount = Convert.ToString(dt.Rows[0]["loanamount"]) != string.Empty ? Convert.ToDouble(dt.Rows[0]["loanamount"]) : 0.00D;
                            appRecordVM.JoinDate = Convert.ToString(dt.Rows[0]["dateofjoining"]) != string.Empty ? Convert.ToString(dt.Rows[0]["dateofjoining"]) : string.Empty;
                            appRecordVM.GovId_FileName = Convert.ToString(dt.Rows[0]["gov_id_url"]) != string.Empty ? Convert.ToString(dt.Rows[0]["gov_id_url"]) : string.Empty;
                            appRecordVM.CompanyId_FileName = Convert.ToString(dt.Rows[0]["companyid_url"]) != string.Empty ? Convert.ToString(dt.Rows[0]["companyid_url"]) : string.Empty;
                            appRecordVM.BillingId_FileName = Convert.ToString(dt.Rows[0]["billing_url"]) != string.Empty ? Convert.ToString(dt.Rows[0]["billing_url"]) : string.Empty;
                            appRecordVM.Income_FileName = Convert.ToString(dt.Rows[0]["income_url"]) != string.Empty ? Convert.ToString(dt.Rows[0]["income_url"]) : string.Empty;
                            appRecordVM.OtherFileName = Convert.ToString(dt.Rows[0]["other_url"]) != string.Empty ? Convert.ToString(dt.Rows[0]["other_url"]) : string.Empty;
                            appRecordVM.Atm_File_Name = Convert.ToString(dt.Rows[0]["atm_url"]) != string.Empty ? Convert.ToString(dt.Rows[0]["atm_url"]) : string.Empty;
                            Session["GovId_FileName"] = Convert.ToString(dt.Rows[0]["gov_id_url"]) != string.Empty ? Convert.ToString(dt.Rows[0]["gov_id_url"]) : string.Empty;
                            Session["CompanyId_FileName"] = Convert.ToString(dt.Rows[0]["companyid_url"]) != string.Empty ? Convert.ToString(dt.Rows[0]["companyid_url"]) : string.Empty;
                            Session["BillingId_FileName"] = Convert.ToString(dt.Rows[0]["billing_url"]) != string.Empty ? Convert.ToString(dt.Rows[0]["billing_url"]) : string.Empty;
                            Session["Income_FileName"] = Convert.ToString(dt.Rows[0]["income_url"]) != string.Empty ? Convert.ToString(dt.Rows[0]["income_url"]) : string.Empty;
                            Session["OtherFileName"] = Convert.ToString(dt.Rows[0]["other_url"]) != string.Empty ? Convert.ToString(dt.Rows[0]["other_url"]) : string.Empty;
                            Session["Atm_File_Name"] = Convert.ToString(dt.Rows[0]["atm_url"]) != string.Empty ? Convert.ToString(dt.Rows[0]["atm_url"]) : string.Empty;
                            appRecordVM.PayDate1 = Convert.ToString(dt.Rows[0]["paydate1"]) != string.Empty ? Convert.ToInt32(dt.Rows[0]["paydate1"]) : -1;
                            appRecordVM.PayDate2 = Convert.ToString(dt.Rows[0]["paydate2"]) != string.Empty ? Convert.ToInt32(dt.Rows[0]["paydate2"]) : -1;
                            appRecordVM.TermType = Convert.ToString(dt.Rows[0]["termtype"]) != string.Empty ? Convert.ToInt32(dt.Rows[0]["termtype"]) : -1;
                            appRecordVM.TermText = Convert.ToString(dt.Rows[0]["term"]) != string.Empty ? Convert.ToInt32(dt.Rows[0]["term"]) : -1;
                            appRecordVM.BestTimeToCallMorning_Id = Convert.ToString(dt.Rows[0]["morning_time"]) != string.Empty ? Convert.ToInt32(dt.Rows[0]["morning_time"]) : -1;
                            appRecordVM.BestTimeToCallNoon_Id = Convert.ToString(dt.Rows[0]["noon_time"]) != string.Empty ? Convert.ToInt32(dt.Rows[0]["noon_time"]) : -1;
                            ViewData["NoRecordMsg"] = "Record Found";
                            if (Convert.ToBoolean(dt1.Rows[0]["ischeck"]) && Convert.ToBoolean(dt1.Rows[0]["isverified"]) && !Convert.ToBoolean(dt1.Rows[0]["isrecheck"]))
                            {
                                ViewBag.noneditable = true;
                            }
                            else
                            {
                                ViewBag.noneditable = false;
                            }
                        }
                        else
                        {
                            TempData["Result"] = "No Records Found";
                        }
                    }


                    return View(appRecordVM);
                }
                else
                {
                    return RedirectToAction("UserLogin", "Home");
                }
            }
            catch (Exception ex)
            {
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                return RedirectToAction("UserLogin", "Home");
            }
        }
        [HttpPost]
        public ActionResult ViewApplicationRecord(ApplicationRecordVM model)
        {
            try
            {
                if (Session["Application_No"] != null)
                {
                    int applicationno = Convert.ToInt32(Session["Application_No"]);
                    string JoinDate = model.JoinDate;
                    if (JoinDate != "")
                    {
                        if (JoinDate.IndexOf('-') != -1)
                        {
                            var arry = JoinDate.Split('-');
                            model.JoinDate = arry[1] + '-' + arry[0] + '-' + arry[2];
                        }
                    }
                    else
                    {
                        DateTime _joinDate = DateTime.Now;
                        string twoDigitDay = Convert.ToString(_joinDate.Day);
                        if (_joinDate.Day < 10)
                        {
                            twoDigitDay = "0" + twoDigitDay;
                        }

                        string twoDigitMonth = Convert.ToString(_joinDate.Month);
                        if (_joinDate.Month < 10)
                        {
                            twoDigitMonth = "0" + twoDigitMonth;
                        }

                        string strDate = string.Empty;
                        strDate = twoDigitMonth + "-" + _joinDate.Year + "-" + twoDigitDay;
                        model.JoinDate = strDate;
                    }
                    string DateOfBirth = model.DOB;
                    if (DateOfBirth != "")
                    {
                        if (DateOfBirth.IndexOf('-') != -1)
                        {
                            var arr = DateOfBirth.Split('-');
                            model.DOB = arr[1] + '-' + arr[0] + '-' + arr[2];
                        }
                    }
                    else
                    {
                        DateTime _DOBDate = DateTime.Now;
                        string twoDigitDay = Convert.ToString(_DOBDate.Day);
                        if (_DOBDate.Day < 10)
                        {
                            twoDigitDay = "0" + twoDigitDay;
                        }

                        string twoDigitMonth = Convert.ToString(_DOBDate.Month);
                        if (_DOBDate.Month < 10)
                        {
                            twoDigitMonth = "0" + twoDigitMonth;
                        }

                        string strDate = string.Empty;
                        strDate = _DOBDate.Year + "-" + twoDigitMonth + "-" + twoDigitDay;
                        model.DOB = strDate;
                    }
                    logger.CreateFolderFTP($"ApplicationNo{Convert.ToString(applicationno)}");
                    ViewBag.CityData = CommonMethods.getCity();
                    ViewBag.CivilStatusData = CommonMethods.getCivilStatus();
                    ViewBag.TermTypeData = CommonMethods.getTermType();
                    ViewBag.IndustryData = CommonMethods.getOccupation();
                    var _GovtFileIdUrl = Upload(model.GovIdFile, applicationno);
                    var _CompanyFileIdUrl = Upload(model.CompanyIdFile, applicationno);
                    var _BillFileIdUrl = Upload(model.BillingIdFile, applicationno);
                    var _IncomeFileIdUrl = Upload(model.IncomeIdFile, applicationno);
                    var _OtherFileIdUrl = Upload(model.OtherIdFile, applicationno);
                    var _ATMFileUrl = Upload(model.AtmIdFile, applicationno);
                    string GovtFileName = _GovtFileIdUrl == "" ? Convert.ToString(Session["GovId_FileName"]) : _GovtFileIdUrl;
                    string CompanyFileName = _CompanyFileIdUrl == "" ? Convert.ToString(Session["CompanyId_FileName"]) : _CompanyFileIdUrl;
                    string BillingFileName = _BillFileIdUrl == "" ? Convert.ToString(Session["BillingId_FileName"]) : _BillFileIdUrl;
                    string IncomeFileName = _IncomeFileIdUrl == "" ? Convert.ToString(Session["Income_FileName"]) : _IncomeFileIdUrl;
                    string OtherFileName = _OtherFileIdUrl == "" ? Convert.ToString(Session["OtherFileName"]) : _OtherFileIdUrl;
                    string AtmFileName = _ATMFileUrl == "" ? Convert.ToString(Session["Atm_File_Name"]) : _ATMFileUrl;

                    //To Update the Records
                    string Data = Convert.ToString(model.GrossIncome);
                    Data = RemoveSpecialCharacters(Data);
                    model.GrossIncome = Convert.ToDouble(Data);

                    Data = Convert.ToString(model.Loan_Amount);
                    Data = RemoveSpecialCharacters(Data);
                    model.Loan_Amount = Convert.ToDouble(Data);
                    logger.WriteErrorLogs($"Term Name:{model.Term} ===>  Term Type:{model.TermType}   ==> TermText:{model.TermText}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                    if (model.TermType != 0 && model.TermText != 0)
                    {
                        int i = DbHelper.InsertUpdateDelete(string.Format(QueryHelper.Updatetblapplication_Record_OnUpdate, RemoveSpecialCharacters(model.Address), model.City, model.DOB, model.CivilStatus, RemoveSpecialCharacters(model.CompanyName), RemoveSpecialCharacters(model.CompanyAddress), RemoveSpecialCharacters(model.Company_Phoneno), model.JoinDate, RemoveSpecialCharacters(model.Designation), model.GrossIncome, model.PayDate1, model.PayDate2, model.Loan_Amount, model.TermType, model.TermText, model.BestTimeToCallMorning_Id, model.BestTimeToCallNoon_Id, GovtFileName ?? "", CompanyFileName ?? "", BillingFileName ?? "", IncomeFileName ?? "", OtherFileName ?? "", applicationno, model.Barangay_id, RemoveSpecialCharacters(model.SSS_No), model.province_id, model.Occupation, AtmFileName ?? ""));

                        if (i > 0)
                        {
                            TempData["Result"] = "Your records are successfully updated!";
                        }
                        else
                        {
                            TempData["Result"] = "Error...";
                        }
                    }
                    else
                    {
                        TempData["Result"] = "Error...";
                    }
                }
                return RedirectToAction("ViewApplicationRecord");
            }
            catch (Exception ex)
            {
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                return RedirectToAction("ViewApplicationRecord");
            }
        }

        /// <summary>
        /// UserLogin page
        /// created by priety
        /// 29/03/2019
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult UserLogin()
        {
            try
            {
                var a = RemoveSpecialCharacters("Naeem02 02#$%");
            }
            catch (Exception ex)
            {
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
            }

            return View();
        }

        [HttpPost]
        public ActionResult UserLogin(ApplicationRecordVM model)
        {
            int appno = 0;
            int userid = 0;
            try
            {
                string pass = Encrypt_Decrypt.EncryptDecrypt.Encrypt(model.Password);

                var query = DbHelper.SelectMethod(string.Format(QueryHelper.GetApplicationUserNew, model.PersonalEmail.Replace("'", "''"), pass));
                if (query != null && query.Rows.Count > 0)
                {
                    appno = Convert.ToInt32(query.Rows[0]["applicationno"].ToString());
                    userid = Convert.ToInt32(query.Rows[0]["id"].ToString());
                    Session["ApplicationNoLogin"] = appno;
                    Session["UserId"] = userid;
                    return RedirectToAction("ViewApplicationRecord");
                }
                else
                {
                    ViewBag.Error = "Incorrect Email ID / Password";
                    return View();
                }
            }
            catch (Exception ex)
            {
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                return View();
            }

        }

        /// <summary>
        /// ThankYou after submit all details in applicaton record
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ThankYou()
        {
            return View();
        }

        /// <summary>
        /// getTerm list 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult GetTerm(int id, string Amount)
        {
            try
            {
                List<Terms> terms = new List<Terms>();
                DataTable dt = DbHelper.SelectMethod(string.Format(QueryHelper.GetTermTypeby_Id, id, Convert.ToDecimal(Amount) > 10000 ? "true" : "false"));
                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        Terms obj = new Terms
                        {
                            Id = Convert.ToInt32(row["id"]),
                            term_Id = Convert.ToInt32(row["term_id"]),
                            term_Name = row["term_name"].ToString(),
                            term_Value = row["term_value"].ToString()
                        };
                        terms.Add(obj);
                    }
                }

                terms.Insert(0, new Terms { Id = -1, term_Name = "Select" });
                return Json(new SelectList(terms, "id", "term_name"), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Check Is EmailAvailable for verification
        /// </summary>
        /// <param name="emailid"></param>
        /// <returns></returns>
        public ActionResult IsEmailAvailable(string emailid)
        {
            bool Result = false;
            try
            {

                DataTable dt = DbHelper.SelectMethod(string.Format(QueryHelper.GetRecordEmailid, emailid.Replace("'", "''")));
                if (dt != null && dt.Rows.Count > 0)
                {
                    Result = true;
                }
            }
            catch (Exception ex)
            {
                Result = false;
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
            }

            return Json(Result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        ///  Check Is ContactNumber Available for verification
        /// </summary>
        /// <param name="ContactNumber"></param>
        /// <returns></returns>
        public ActionResult IsContactNumberAvailable(string ContactNumber)
        {
            bool Result = false;
            try
            {

                DataTable dt = DbHelper.SelectMethod(string.Format(QueryHelper.GetRecordContactNumber, ContactNumber.Replace("'", "''")));
                if (dt != null && dt.Rows.Count > 0)
                {
                    Result = true;
                }
            }
            catch (Exception ex)
            {
                Result = false;
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
            }

            return Json(Result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// LogOut
        /// </summary>
        /// <returns></returns>
        public ActionResult LogOut()
        {
            Session.Abandon();
            return RedirectToAction("userlogin", "Home");
        }

        /// <summary>
        /// ForgotPassword
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        /// <summary>
        /// ForGot PasswordMethod
        /// </summary>
        /// <param name="Email"></param>
        /// <returns></returns>
        public ActionResult ForGotPasswordMethod(string Email)
        {
            bool result = false;
            DataTable dt = DbHelper.SelectMethod(string.Format(QueryHelper.GetPassword, Email));
            if (dt != null && dt.Rows.Count > 0)
            {
                Guid UniqueId = Guid.NewGuid();
                int i = DbHelper.InsertUpdateDelete(string.Format(QueryHelper.UpdateGuid, UniqueId, Email));
                if (i != 0)
                {

                    SendMail_Password(Email, Convert.ToString(UniqueId));
                    result = true;
                }
                else
                {
                    logger.WriteErrorLogs($"value of i {i}", "ForGotPasswordMethod");
                }
            }
            else
            {
                result = false;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ResetPassword(Guid? id)
        {
            Session["UniqueId"] = id;
            return View();
        }

        /// <summary>
        /// ResetPassword 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult UpdatePassword(string password)
        {
            if (Session["UniqueId"] != null)
            {
                string Password = Encrypt_Decrypt.EncryptDecrypt.Encrypt(password);
                string UniqueId = Convert.ToString(Session["UniqueId"]);
                var flag = false;
                int i = DbHelper.InsertUpdateDelete(string.Format(QueryHelper.UpdatePassword, UniqueId, Password));
                if (i > 0)
                {
                    flag = true;
                }

                return Json(flag, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }


        /// <summary>
        /// link send for reset password
        /// </summary>
        /// <param name="EmailId"></param>
        /// <param name="UniqueId"></param>

        private void SendMail_Password(string EmailId, string UniqueId)
        {
            try
            {
                Uri uri = new Uri(Request.Url.AbsoluteUri);
                string Url = string.Format("{0}://{1}", uri.Scheme, uri.Authority);
                string body = Url + "/Home/ResetPassword/" + UniqueId;
                StringBuilder EmailBody = new StringBuilder();
                EmailBody.AppendLine("Reset Password Link is given below.<br/><b>Click Here: </b>" + body);
                new Thread(() => CommonMethods.SendMail(EmailFrom.FromInfo, EmailId, Convert.ToString(EmailBody), "Reset Your Password")).Start();
            }
            catch (Exception ex)
            {
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                throw ex;

            }
        }



        [HttpGet]
        [System.Web.Services.WebMethod]
        private void DownloadFileFromFTP(string url, string name)
        {
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(url);
            request.Method = WebRequestMethods.Ftp.DownloadFile;

            //Enter FTP Server credentials.
            request.Credentials = new NetworkCredential(Convert.ToString(ConfigurationManager.AppSettings["FTP_UserName"]), Convert.ToString(ConfigurationManager.AppSettings["FTP_Password"]));
            request.UsePassive = true;
            request.UseBinary = true;
            request.EnableSsl = false;

            //Fetch the Response and read it into a MemoryStream object.
            FtpWebResponse response = (FtpWebResponse)request.GetResponse();
            using (MemoryStream stream = new MemoryStream())
            {
                //Download the File.
                response.GetResponseStream().CopyTo(stream);
                Response.AddHeader("content-disposition", "attachment;filename=" + name);
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.BinaryWrite(stream.ToArray());
                Response.End();
            }
        }

        /// <summary>
        /// RemoveSpecialCharacters
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public string RemoveSpecialCharacters(string str)
        {
            try
            {
                StringBuilder sb = new StringBuilder();

                foreach (char c in str)
                {
                    if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || (c == ' ') || (c == ',') || (c == '.'))
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

        /// <summary>
        /// Get city according to province
        /// </summary>
        /// <param name="Prefix"></param>
        /// <param name="ProvinceID"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult City(string Prefix, int ProvinceID)
        {
            try
            {
                List<City> ObjList = CommonMethods.getCityByIDName(ProvinceID, Prefix);
                return Json(ObjList, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                return Json(null, JsonRequestBehavior.AllowGet);
            }

        }

        /// <summary>
        /// ResendOTP in user contact number
        /// </summary>
        /// <param name="mobno"></param>
        /// <returns></returns>
        public ActionResult ResendOTP(string mobno)
        {
            try
            {
                Random random = new Random();
                string destinationaddr = mobno;
                int value = random.Next(10001, 99999);
                bool flag = false;
                int i = 0;
                i = DbHelper.InsertUpdateDelete(string.Format(QueryHelper.UpdateOTP, value, mobno));

                if (i != 0)
                {
                    logger.WriteErrorLogs("OTP Updated", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                    string otpmessage = "Your New OTP Number is " + value + " (Sent By : Cashmart) This OTP is valid for 15 minutes";
                    flag = true;
                    flag = logger.ClicktoSMS(mobno, otpmessage);
                }
                else
                {
                    logger.WriteErrorLogs("OTP Not Updated", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                    flag = false;
                }


                return Json(flag, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                return Json(false);
            }
        }

        public ActionResult sendOTP(string mobno)
        {
            try
            {
                Random random = new Random();
                string destinationaddr = mobno;
                int value = random.Next(10001, 99999);
                bool flag = false;
                int i = 0;
                i = DbHelper.InsertUpdateDelete(string.Format(QueryHelper.InsertOTPDetails, mobno, value));

                if (i != 0)
                {
                    logger.WriteErrorLogs("OTP Updated", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                    string otpmessage = "Your New OTP Number is " + value + " (Sent By : Cashmart) This OTP is valid for 15 minutes";
                    flag = true;
                    flag = logger.ClicktoSMS(mobno, otpmessage);
                }
                else
                {
                    logger.WriteErrorLogs("OTP Not Updated", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                    flag = false;
                }


                return Json(flag, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                return Json(false);
            }
        }

        /// <summary>
        ///  Get province
        /// </summary>
        /// <param name="Prefix"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Province(string Prefix)
        {
            try
            {
                List<ProvinceModel> ObjList = CommonMethods.getProvinceByName(Prefix);
                return Json(ObjList, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                return Json(null, JsonRequestBehavior.AllowGet);
            }


        }

        /// <summary>
        ///  Get Barangay according to CityID
        /// </summary>
        /// <param name="Prefix"></param>
        /// <param name="CityID"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Barangay(string Prefix, int CityID)
        {
            try
            {
                List<BarangayModel> ObjList = CommonMethods.getBarangayByIDName(CityID, Prefix);
                return Json(ObjList, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                return Json(null, JsonRequestBehavior.AllowGet);
            }

        }


        /// <summary>
        /// SendSMS in user contact number
        /// </summary>
        /// <param name="mobno"></param>
        /// <param name="Id"></param>
        /// <param name="Name"></param>
        public void SendSMS(string mobno, string Name)
        {
            try
            {
                string otpmessage = "Hi " + Name + "," + Convert.ToString(ConfigurationManager.AppSettings["NoActivitySmsBody"]);
                new Thread(() => logger.ClicktoSMS(mobno, otpmessage)).Start();
            }
            catch (Exception ex)
            {
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
            }
        }


        public ActionResult DownloadAttachment(string url)
        {
            byte[] buf = new byte[] { 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20 };
            try
            {
                if (!String.IsNullOrWhiteSpace(url))
                {
                    var request = (FtpWebRequest)WebRequest.Create(url);
                    request.Method = WebRequestMethods.Ftp.DownloadFile;
                    request.Credentials = new NetworkCredential(Convert.ToString(ConfigurationManager.AppSettings["FTP_UserName"]), Convert.ToString(ConfigurationManager.AppSettings["FTP_Password"]));
                    request.UseBinary = true;
                    request.Timeout = 10000;
                    using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
                    {
                        using (Stream responseStream = response.GetResponseStream())
                        {
                            byte[] buffer = new byte[16 * 1024];
                            using (MemoryStream ms = new MemoryStream())
                            {
                                int read;
                                while ((read = responseStream.Read(buffer, 0, buffer.Length)) > 0)
                                {
                                    ms.Write(buffer, 0, read);
                                }
                                buf = ms.ToArray();
                            }
                        }
                    }
                }
                return File(buf, System.Net.Mime.MediaTypeNames.Application.Octet, $"{Guid.NewGuid()}.{url.Split('.').Last()}");
            }
            catch (Exception ex)
            {
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", System.Reflection.MethodBase.GetCurrentMethod().Name);
                return null;
            }
        }

        public ActionResult TongdonData(string ReferenceNo)
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.Collector))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            TongdunVM tongdun = new TongdunVM();
            try
            {
                List<TongdunModelGreb> tongdunModels = new List<TongdunModelGreb>();
                var grebData = DbHelper.SelectMethod(String.Format(QueryHelper.GetTongdunData, ReferenceNo));
                if (grebData != null && grebData.Rows.Count > 0)
                {
                    foreach (DataRow item in grebData.Rows)
                    {
                        tongdunModels.Add(new TongdunModelGreb
                        {
                            id = Convert.ToInt32(item["id"]),
                            userid = Convert.ToString(item["userid"]),
                            order_no = Convert.ToString(item["order_no"]),
                            date = Convert.ToString(item["date"]),
                            country = Convert.ToString(item["country"]),
                            distance = Convert.ToString(item["distance"]),
                            city = Convert.ToString(item["city"]),
                            evaluation = Convert.ToString(item["evaluation"]),
                            car_driver = Convert.ToString(item["car_driver"]),
                            from_longitude = Convert.ToString(item["from_longitude"]),
                            from_ = Convert.ToString(item["from_"]),
                            pay_type = Convert.ToString(item["pay_type"]),
                            tag = Convert.ToString(item["tag"]),
                            phone_type = Convert.ToString(item["phone_type"]),
                            vehicle_type = Convert.ToString(item["vehicle_type"]),
                            to_detail = Convert.ToString(item["to_detail"]),
                            currency_unit = Convert.ToString(item["currency_unit"]),
                            from_latitude = Convert.ToString(item["from_latitude"]),
                            to_longitude = Convert.ToString(item["to_longitude"]),
                            car_no = Convert.ToString(item["car_no"]),
                            intermediate = Convert.ToString(item["intermediate"]),
                            order_fee = Convert.ToString(item["order_fee"]),
                            to_latitude = Convert.ToString(item["to_latitude"]),
                            to_ = Convert.ToString(item["to_"]),
                            time_ = Convert.ToString(item["time_"]),
                            from_detail = Convert.ToString(item["from_detail"]),
                            status = Convert.ToString(item["status"]),
                            createdon = Convert.ToDateTime(item["createdon"]),
                        });
                    }
                }

                List<TongdunModelShopee> TongdunModelShopees = new List<TongdunModelShopee>();
                var ShopeeData = DbHelper.SelectMethod(String.Format(QueryHelper.GetTongdunShopeeData, ReferenceNo));
                if (ShopeeData != null && ShopeeData.Rows.Count > 0)
                {
                    foreach (DataRow item in ShopeeData.Rows)
                    {
                        TongdunModelShopees.Add(new TongdunModelShopee
                        {
                            id = Convert.ToInt32(item["id"]),
                            order_no = Convert.ToString(item["order_no"]),
                            package_shipping = Convert.ToString(item["package_shipping"]),
                            package_sold_by = Convert.ToString(item["package_sold_by"]),
                            shipping_cost = Convert.ToString(item["shipping_cost"]),
                            payment_time = Convert.ToString(item["payment_time"]),
                            sub_total = Convert.ToString(item["sub_total"]),
                            product_name = Convert.ToString(item["product_name"]),
                            grand_total = Convert.ToString(item["grand_total"]),
                            shipping_address = Convert.ToString(item["shipping_address"]),
                            phone = Convert.ToString(item["phone"]),
                            customername = Convert.ToString(item["customername"]),
                            jumio_reference = Convert.ToString(item["jumio_reference"])
                        });
                    }
                }

                List<TongdunModelLazada> TongdunModelLazadas = new List<TongdunModelLazada>();
                var LazadaData = DbHelper.SelectMethod(String.Format(QueryHelper.GetTongdunLazada, ReferenceNo));
                if (LazadaData != null && LazadaData.Rows.Count > 0)
                {
                    foreach (DataRow item in LazadaData.Rows)
                    {
                        TongdunModelLazadas.Add(new TongdunModelLazada
                        {
                            id = Convert.ToInt32(item["id"]),
                            order_no = Convert.ToString(item["order_no"]),
                            shipping_cost = Convert.ToString(item["shipping_cost"]),
                            sub_total = Convert.ToString(item["sub_total"]),
                            billing_address = Convert.ToString(item["billing_address"]),
                            billing_address_phone = Convert.ToString(item["billing_address_phone"]),
                            billing_address_name = Convert.ToString(item["billing_address_name"]),
                            order_time = Convert.ToString(item["order_time"]),
                            grand_total = Convert.ToString(item["grand_total"]),
                            shipping_address = Convert.ToString(item["shipping_address"]),
                            shipping_address_name = Convert.ToString(item["shipping_address_name"]),
                            shipping_address_phone = Convert.ToString(item["shipping_address_phone"]),
                            jumio_reference = Convert.ToString(item["jumio_reference"])
                        });
                    }
                }


                tongdun.ReferenceNo = ReferenceNo;
                tongdun.tongdunModels = tongdunModels;
                tongdun.shoppeData = TongdunModelShopees;
                tongdun.lazadas = TongdunModelLazadas;
            }
            catch (Exception ex)
            {
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
            }
            return View(tongdun);
        }

        [HttpPost]
        public ActionResult SendManulNotification(string notificationtoken, string message, string MobileNo, String UserName, string PanelName, string ApplicationNo)
        {
            DbHelper.SelectMethod(string.Format(QueryHelper.InsertManualNotificationHistory, ApplicationNo, MobileNo, notificationtoken, message, UserName, PanelName));
            new Thread(() => logger.ClicktoSMS(MobileNo, message)).Start();
            logger.AddWelcomeMessage(History.Application, "", message, 0, true, Convert.ToInt32(ApplicationNo));
            return Json(true);
        }

        public ActionResult GetOldData(string Emailid)
        {
            MainData main = new MainData();
            if (!String.IsNullOrWhiteSpace(Emailid))
            {
                DataTable dt = DbHelper.SelectMethod(string.Format(QueryHelper.GetOldData, Emailid));
                if (dt != null && dt.Rows.Count > 0)
                {
                    main.firstGridData = new List<FirstGridData>();
                    foreach (DataRow row in dt.Rows)
                    {

                        main.firstGridData.Add(new FirstGridData
                        {
                            account_status = Convert.ToString(row["account_status"]),
                            ammortization_payment = Convert.ToDecimal(row["total_paid"]),
                            contractno = Convert.ToString(row["contractno"]),
                            date_applied = Convert.ToDateTime(row["disburse_date"]).ToString("MM-dd-yyyy"),
                            loan_amount = Convert.ToDecimal(row["loan_amount"]),
                            term_name = Convert.ToString(row["term_name"]),
                            total_LatePenalties = Convert.ToDecimal(row["total_LatePenalties"]),
                            totoal_LateFee = Convert.ToDecimal(row["totoal_LateFee"]),
                            Remarks = Convert.ToString(row["remarks"])
                        });
                    }
                }
            }
            else
            {
                TempData["invild"] = "1";
            }
            return View(main);
        }

        [HttpPost]
        public ActionResult GetHistoryData(string contractno)
        {
            List<PaymentHistory> paymentHistories = new List<PaymentHistory>();
            if (!String.IsNullOrWhiteSpace(contractno))
            {
                DataTable dt = DbHelper.SelectMethod(string.Format(QueryHelper.GetOldDataBycontractno, contractno));
                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        logger.WriteErrorLogs($"=============================================================\n due_date1: {row["due_date1"].ToString().Length}\n due_date2: {row["due_date2"].ToString().Length}\n due_date3: {row["due_date3"].ToString().Length}\n due_date4: {row["due_date4"].ToString().Length}\n due_date5: {row["due_date5"].ToString().Length}\n due_date6: {row["due_date6"].ToString().Length}\n due_date7: {row["due_date7"].ToString().Length}\n due_date8: {row["due_date8"].ToString().Length}\n due_date9: {row["due_date9"].ToString().Length}", $"{this.GetType().Name} ==> {System.Reflection.MethodBase.GetCurrentMethod().Name}");

                        if (!String.IsNullOrWhiteSpace(Convert.ToString(row["due_date1"])))
                        {
                            paymentHistories.Add(new PaymentHistory
                            {
                                Ammortization = Convert.ToDecimal(row["ammortization_payment"]),
                                AmountPaid = Convert.ToDecimal(row["paid_amount1"]),
                                contractno = Convert.ToString(row["contractno"]),
                                DailyPenalites = Convert.ToDecimal(row["penalty1"]),
                                Due_Date = Convert.ToString(row["due_date1"]),
                                LatePaymentFee = Convert.ToDecimal(row["late_payment1"]),
                                PaymentDate = Convert.ToString(row["paid_date1"]),
                            });
                        }

                        if (!String.IsNullOrWhiteSpace(Convert.ToString(row["due_date2"])))
                        {
                            paymentHistories.Add(new PaymentHistory
                            {
                                Ammortization = Convert.ToDecimal(row["ammortization_payment"]),
                                AmountPaid = Convert.ToDecimal(row["paid_amount2"]),
                                contractno = Convert.ToString(row["contractno"]),
                                DailyPenalites = Convert.ToDecimal(row["penalty2"]),
                                Due_Date = Convert.ToString(row["due_date2"]),
                                LatePaymentFee = Convert.ToDecimal(row["late_payment2"]),
                                PaymentDate = Convert.ToString(row["paid_date2"]),
                            });
                        }

                        if (!String.IsNullOrWhiteSpace(Convert.ToString(row["due_date3"])))
                        {
                            paymentHistories.Add(new PaymentHistory
                            {
                                Ammortization = Convert.ToDecimal(row["ammortization_payment"]),
                                AmountPaid = Convert.ToDecimal(row["paid_amount3"]),
                                contractno = Convert.ToString(row["contractno"]),
                                DailyPenalites = Convert.ToDecimal(row["penalty3"]),
                                Due_Date = Convert.ToString(row["due_date3"]),
                                LatePaymentFee = Convert.ToDecimal(row["late_payment3"]),
                                PaymentDate = Convert.ToString(row["paid_date3"]),
                            });
                        }

                        if (!String.IsNullOrWhiteSpace(Convert.ToString(row["due_date4"])))
                        {
                            paymentHistories.Add(new PaymentHistory
                            {
                                Ammortization = Convert.ToDecimal(row["ammortization_payment"]),
                                AmountPaid = Convert.ToDecimal(row["paid_amount4"]),
                                contractno = Convert.ToString(row["contractno"]),
                                DailyPenalites = Convert.ToDecimal(row["penalty4"]),
                                Due_Date = Convert.ToString(row["due_date4"]),
                                LatePaymentFee = Convert.ToDecimal(row["late_payment4"]),
                                PaymentDate = Convert.ToString(row["paid_date4"]),
                            });
                        }

                        if (!String.IsNullOrWhiteSpace(Convert.ToString(row["due_date5"])))
                        {
                            paymentHistories.Add(new PaymentHistory
                            {
                                Ammortization = Convert.ToDecimal(row["ammortization_payment"]),
                                AmountPaid = Convert.ToDecimal(row["paid_amount5"]),
                                contractno = Convert.ToString(row["contractno"]),
                                DailyPenalites = Convert.ToDecimal(row["penalty5"]),
                                Due_Date = Convert.ToString(row["due_date5"]),
                                LatePaymentFee = Convert.ToDecimal(row["late_payment5"]),
                                PaymentDate = Convert.ToString(row["paid_date5"]),
                            });
                        }

                        if (!String.IsNullOrWhiteSpace(Convert.ToString(row["due_date6"])))
                        {
                            paymentHistories.Add(new PaymentHistory
                            {
                                Ammortization = Convert.ToDecimal(row["ammortization_payment"]),
                                AmountPaid = Convert.ToDecimal(row["paid_amount6"]),
                                contractno = Convert.ToString(row["contractno"]),
                                DailyPenalites = Convert.ToDecimal(row["penalty6"]),
                                Due_Date = Convert.ToString(row["due_date6"]),
                                LatePaymentFee = Convert.ToDecimal(row["late_payment6"]),
                                PaymentDate = Convert.ToString(row["paid_date6"]),
                            });
                        }

                        if (!String.IsNullOrWhiteSpace(Convert.ToString(row["due_date7"])))
                        {
                            paymentHistories.Add(new PaymentHistory
                            {
                                Ammortization = Convert.ToDecimal(row["ammortization_payment"]),
                                AmountPaid = Convert.ToDecimal(row["paid_amount7"]),
                                contractno = Convert.ToString(row["contractno"]),
                                DailyPenalites = Convert.ToDecimal(row["penalty7"]),
                                Due_Date = Convert.ToString(row["due_date7"]),
                                LatePaymentFee = Convert.ToDecimal(row["late_payment7"]),
                                PaymentDate = Convert.ToString(row["paid_date7"]),
                            });
                        }

                        if (!String.IsNullOrWhiteSpace(Convert.ToString(row["due_date8"])))
                        {
                            paymentHistories.Add(new PaymentHistory
                            {
                                Ammortization = Convert.ToDecimal(row["ammortization_payment"]),
                                AmountPaid = Convert.ToDecimal(row["paid_amount8"]),
                                contractno = Convert.ToString(row["contractno"]),
                                DailyPenalites = Convert.ToDecimal(row["penalty8"]),
                                Due_Date = Convert.ToString(row["due_date8"]),
                                LatePaymentFee = Convert.ToDecimal(row["late_payment8"]),
                                PaymentDate = Convert.ToString(row["paid_date8"]),
                            });
                        }

                        if (!String.IsNullOrWhiteSpace(Convert.ToString(row["due_date9"])))
                        {
                            paymentHistories.Add(new PaymentHistory
                            {
                                Ammortization = Convert.ToDecimal(row["ammortization_payment"]),
                                AmountPaid = Convert.ToDecimal(row["paid_amount9"]),
                                contractno = Convert.ToString(row["contractno"]),
                                DailyPenalites = Convert.ToDecimal(row["penalty9"]),
                                Due_Date = Convert.ToString(row["due_date9"]),
                                LatePaymentFee = Convert.ToDecimal(row["late_payment9"]),
                                PaymentDate = Convert.ToString(row["paid_date9"]),
                            });
                        }
                    }
                }
            }
            else
            {
                return Json(null);
            }
            return Json(paymentHistories);
        }

        [HttpGet]
        public ActionResult LoanHistory(int UserID, string ActionName, int ApplicationNo)
        {
            PaymentInformation listofpayment = new PaymentInformation();
            try
            {
                CommonOperation comnoperation = new CommonOperation();
                DataTable dtLoanHistory = new DataTable();
                dtLoanHistory = DbHelper.SelectMethod(String.Format(QueryHelper.GetLoanHistoryLatest, UserID));

                if (dtLoanHistory != null && dtLoanHistory.Rows.Count > 0)
                {
                    for (int i = 0; i < dtLoanHistory.Rows.Count; i++)
                    {
                        double _Penality = comnoperation.GetPaidPenalty(Convert.ToString(dtLoanHistory.Rows[i]["applicationno"])); ;
                        comnoperation.GetOutstandingAmount(Convert.ToString(dtLoanHistory.Rows[i]["applicationno"]));

                        listofpayment.LoanHistory.Add(new PaymentInformation
                        {
                            Termtype = DBNull.Value.Equals(dtLoanHistory.Rows[i]["term_name"]) ? null : Convert.ToString(dtLoanHistory.Rows[i]["term_name"]),
                            Contractno = DBNull.Value.Equals(dtLoanHistory.Rows[i]["contract_ref_no"]) ? null : Convert.ToString(dtLoanHistory.Rows[i]["contract_ref_no"]),
                            LoanAmount = DBNull.Value.Equals(dtLoanHistory.Rows[i]["approved_loan_amount"]) ? 0 : Convert.ToDecimal(dtLoanHistory.Rows[i]["approved_loan_amount"]),
                            AmmortizationPaid = DBNull.Value.Equals(dtLoanHistory.Rows[i]["paid_amount"]) ? 0 : Convert.ToDecimal(dtLoanHistory.Rows[i]["paid_amount"]),
                            AmmortizationToBePaid = DBNull.Value.Equals(dtLoanHistory.Rows[i]["emiamount"]) ? 0 : Convert.ToDecimal(dtLoanHistory.Rows[i]["emiamount"]),
                            penalty = _Penality,
                            paiddate = DBNull.Value.Equals(dtLoanHistory.Rows[i]["paid_on"]) ? null : Convert.ToString(dtLoanHistory.Rows[i]["paid_on"]),
                            Dateofdistursement = DBNull.Value.Equals(dtLoanHistory.Rows[i]["disbursement_date"]) ? null : Convert.ToString(dtLoanHistory.Rows[i]["disbursement_date"]),
                            completeon = DBNull.Value.Equals(dtLoanHistory.Rows[i]["completeon"]) ? "" : Convert.ToString(dtLoanHistory.Rows[i]["completeon"]),
                            applicationno = DBNull.Value.Equals(dtLoanHistory.Rows[i]["applicationno"]) ? 0 : Convert.ToInt64(dtLoanHistory.Rows[i]["applicationno"])
                        });
                        listofpayment.applicationno = ApplicationNo;
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));

            }
            return View(listofpayment);
        }

        [HttpGet]
        public ActionResult GetSmileData(int ApplicationNo)
        {
            SmileData smileData = new SmileData();
            try
            {
                DataTable dtLoanHistory = new DataTable();
                dtLoanHistory = DbHelper.SelectMethod(String.Format(QueryHelper.GetSmileData, ApplicationNo));
                if (dtLoanHistory != null && dtLoanHistory.Rows.Count > 0)
                {
                    for (int i = 0; i < dtLoanHistory.Rows.Count; i++)
                    {
                        smileData.dataid = DBNull.Value.Equals(dtLoanHistory.Rows[i]["dataid"]) ? 0 : Convert.ToInt64(dtLoanHistory.Rows[i]["dataid"]);
                        smileData.FullName = DBNull.Value.Equals(dtLoanHistory.Rows[i]["fullname"]) ? null : Convert.ToString(dtLoanHistory.Rows[i]["fullname"]);
                        smileData.FirstName = DBNull.Value.Equals(dtLoanHistory.Rows[i]["firstname"]) ? null : Convert.ToString(dtLoanHistory.Rows[i]["firstname"]);
                        smileData.MiddleName = DBNull.Value.Equals(dtLoanHistory.Rows[i]["middlename"]) ? null : Convert.ToString(dtLoanHistory.Rows[i]["middlename"]);
                        smileData.LastName = DBNull.Value.Equals(dtLoanHistory.Rows[i]["lastname"]) ? null : Convert.ToString(dtLoanHistory.Rows[i]["lastname"]);
                        smileData.Suffix = DBNull.Value.Equals(dtLoanHistory.Rows[i]["suffix"]) ? null : Convert.ToString(dtLoanHistory.Rows[i]["suffix"]);
                        smileData.DateOfBirth = DBNull.Value.Equals(dtLoanHistory.Rows[i]["dob"]) ? null : Convert.ToString(dtLoanHistory.Rows[i]["dob"]);
                        smileData.Gender = DBNull.Value.Equals(dtLoanHistory.Rows[i]["gender"]) ? null : Convert.ToString(dtLoanHistory.Rows[i]["gender"]);
                        smileData.MaritalStatus = DBNull.Value.Equals(dtLoanHistory.Rows[i]["maritalstatus"]) ? null : Convert.ToString(dtLoanHistory.Rows[i]["maritalstatus"]);
                        smileData.CountryResidence = DBNull.Value.Equals(dtLoanHistory.Rows[i]["country_residence"]) ? null : Convert.ToString(dtLoanHistory.Rows[i]["country_residence"]);
                        smileData.Citizendship = DBNull.Value.Equals(dtLoanHistory.Rows[i]["citizenship"]) ? null : Convert.ToString(dtLoanHistory.Rows[i]["citizenship"]);

                        //Smile Document Data Fetched.
                        DataTable dtSmileDocument = DbHelper.SelectMethod(String.Format(QueryHelper.GetSmileDocuments, smileData.dataid));
                        if (dtSmileDocument != null && dtSmileDocument.Rows.Count > 0)
                        {
                            foreach (DataRow item in dtSmileDocument.Rows)
                            {
                                SmileDocument smileDocument = new SmileDocument()
                                {
                                    DocName = DBNull.Value.Equals(item["doc_name"]) ? null : Convert.ToString(item["doc_name"]),
                                    DocId = DBNull.Value.Equals(item["doc_id"]) ? null : Convert.ToString(item["doc_id"]),
                                    DocType = DBNull.Value.Equals(item["doc_type"]) ? null : Convert.ToString(item["doc_type"]),
                                    IssueDate = DBNull.Value.Equals(item["issue_date"]) ? null : Convert.ToString(item["issue_date"]),
                                    ExpiryDate = DBNull.Value.Equals(item["expiry_date"]) ? null : Convert.ToString(item["expiry_date"]),
                                    DocStatus = DBNull.Value.Equals(item["status"]) ? null : Convert.ToString(item["status"]),
                                    Attachment = DBNull.Value.Equals(item["attachement"]) ? null : Convert.ToString(item["attachement"]),
                                };
                                smileData.smileDocuments.Add(smileDocument);
                            }
                        }

                        //Smile Employment Data Fetched.
                        DataTable dtSmileEmployment = DbHelper.SelectMethod(String.Format(QueryHelper.GetSmileEmployments, smileData.dataid));
                        if (dtSmileEmployment != null && dtSmileEmployment.Rows.Count > 0)
                        {
                            foreach (DataRow item in dtSmileEmployment.Rows)
                            {
                                SmileEmployment smileEmployment = new SmileEmployment()
                                {
                                    StartDate = DBNull.Value.Equals(item["startdate"]) ? null : Convert.ToString(item["startdate"]),
                                    EndDate = DBNull.Value.Equals(item["enddate"]) ? null : Convert.ToString(item["enddate"]),
                                    JobName = DBNull.Value.Equals(item["job_name"]) ? null : Convert.ToString(item["job_name"]),
                                    JobPosition = DBNull.Value.Equals(item["job_position"]) ? null : Convert.ToString(item["job_position"]),
                                    JobStatus = DBNull.Value.Equals(item["job_status"]) ? null : Convert.ToString(item["job_status"]),
                                    Department = DBNull.Value.Equals(item["department"]) ? null : Convert.ToString(item["department"]),
                                    EmployeeCode = DBNull.Value.Equals(item["emp_code"]) ? null : Convert.ToString(item["emp_code"]),
                                    Employee = DBNull.Value.Equals(item["employer"]) ? null : Convert.ToString(item["employer"]),
                                };
                                smileData.smileEmployments.Add(smileEmployment);
                            }
                        }

                        //Smile Liabilities Data Fetched.
                        DataTable dtSmileLiabilities = DbHelper.SelectMethod(String.Format(QueryHelper.GetSmileLiabilities, smileData.dataid));
                        if (dtSmileLiabilities != null && dtSmileLiabilities.Rows.Count > 0)
                        {
                            foreach (DataRow item in dtSmileLiabilities.Rows)
                            {
                                SmileLiabilities smileLiabilities = new SmileLiabilities()
                                {
                                    ReferenceId = DBNull.Value.Equals(item["refid"]) ? null : Convert.ToString(item["refid"]),
                                    InitialLoan = DBNull.Value.Equals(item["initial_amount"]) ? null : Convert.ToString(item["initial_amount"]),
                                    OutstandingBalance = DBNull.Value.Equals(item["outstanding_amount"]) ? null : Convert.ToString(item["outstanding_amount"]),
                                    Overdue = DBNull.Value.Equals(item["overdue_amount"]) ? null : Convert.ToString(item["overdue_amount"]),
                                    LoanType = DBNull.Value.Equals(item["loan_type"]) ? null : Convert.ToString(item["loan_type"]),
                                    StartDate = DBNull.Value.Equals(item["loan_started"]) ? null : Convert.ToString(item["loan_started"]),
                                    EndDate = DBNull.Value.Equals(item["loan_ends"]) ? null : Convert.ToString(item["loan_ends"]),
                                    Frequency = DBNull.Value.Equals(item["frequency"]) ? null : Convert.ToString(item["frequency"]),
                                    Amount = DBNull.Value.Equals(item["amount"]) ? null : Convert.ToString(item["amount"]),
                                    StartOn = DBNull.Value.Equals(item["startedon"]) ? null : Convert.ToString(item["startedon"]),
                                };
                                smileData.smileLiabilities.Add(smileLiabilities);
                            }
                        }

                        //Smile Contributions Data Fetched.
                        DataTable dtSmileContributions = DbHelper.SelectMethod(String.Format(QueryHelper.GetSmileContributions, smileData.dataid));
                        if (dtSmileContributions != null && dtSmileContributions.Rows.Count > 0)
                        {
                            foreach (DataRow item in dtSmileContributions.Rows)
                            {
                                SmileContributions smileContributions = new SmileContributions()
                                {
                                    ReferenceId = DBNull.Value.Equals(item["refid"]) ? null : Convert.ToString(item["refid"]),
                                    ContributionDate = DBNull.Value.Equals(item["refdate"]) ? null : Convert.ToString(item["refdate"]),
                                    Currency = DBNull.Value.Equals(item["currency"]) ? null : Convert.ToString(item["currency"]),
                                    Amount = DBNull.Value.Equals(item["amount"]) ? null : Convert.ToString(item["amount"])
                                };
                                smileData.smileContributions.Add(smileContributions);
                            }
                        }

                        //Smile EstimatedIncome Data Fetched.
                        DataTable dtSmileEstimatedIncome = DbHelper.SelectMethod(String.Format(QueryHelper.GetSmileEstimatedIncome, smileData.dataid));
                        if (dtSmileEstimatedIncome != null && dtSmileEstimatedIncome.Rows.Count > 0)
                        {
                            foreach (DataRow item in dtSmileEstimatedIncome.Rows)
                            {
                                SmileEstimatedIncome smileEstimatedIncome = new SmileEstimatedIncome()
                                {
                                    IncomeMonth = DBNull.Value.Equals(item["months"]) ? null : Convert.ToString(item["months"]),
                                    Currency = DBNull.Value.Equals(item["currency"]) ? null : Convert.ToString(item["currency"]),
                                    Amount = DBNull.Value.Equals(item["amount"]) ? null : Convert.ToString(item["amount"])
                                };
                                smileData.smileEstimatedIncomes.Add(smileEstimatedIncome);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
            }
            return View(smileData);
        }

        public ActionResult Lockout()
        {
            return View();
        }
    }

}



