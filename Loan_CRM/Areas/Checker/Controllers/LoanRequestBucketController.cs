using Loan_CRM.Areas.Checker.Models;
using Loan_CRM.Models;

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace Loan_CRM.Areas.Checker.Controllers
{
    public class LoanRequestBucketController : Controller
    {
        /// <summary>
        /// 
        /// </summary>
        public object MessageBox { get; private set; }
        /// <summary>
        /// Bucket list
        /// </summary>
        /// <returns></returns>
        public ActionResult LoanRequest()
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.Checker))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            return View();
        }
        /// <summary>
        /// Rechecker Users Data
        /// </summary>
        /// <returns></returns>
        public ActionResult GetRechekUsersData()
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.Checker))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            int user_id;
            CommonOperation comnoperation = new CommonOperation();
            user_id = Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value);
            var usersdetail = comnoperation.GetRecheckBucketList(user_id);

            var jsonResult = Json(usersdetail, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }
        /// <summary>
        /// Recheck Bucket List
        /// </summary>
        /// <returns></returns>
        public ActionResult RecheckRequestBucket()
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.Checker))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            return View();
        }
        /// <summary>
        /// Checker Information by ID
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="Actionname"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult RequestDetail(int Id, string Actionname)
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.Checker))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            ActivityLog.Info($"Application {Id} Get on Checker Window by User {Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value)}", $"{this.GetType().Name} ==> {System.Reflection.MethodBase.GetCurrentMethod().Name}");
            Chekerinformation appinformation = new Chekerinformation();
            try
            {
                int user_id;
                TempData["actionname"] = Actionname;
                Session["CheckerActionName"] = Actionname;
                user_id = Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value);
                CommonOperation comnoperation = new CommonOperation();
                appinformation = comnoperation.GetListbyID(Id, Actionname, user_id);
                int _ApplicationUserId = appinformation.user_id;
                appinformation.user_id = user_id;
                ViewBag.govidurl = appinformation.gov_id_url;
                ViewBag.companyidurl = appinformation.companyid_url;
                ViewBag.billingurl = appinformation.billing_url;
                ViewBag.incom_url = appinformation.income_url;
                ViewBag.other_url = appinformation.other_url;
                ViewBag.atm_url = appinformation.atm_url;
               
                if (!string.IsNullOrWhiteSpace(appinformation.jumioreference))
                {
                    var _TransactionData = DbHelper.SelectMethod(QueryHelper.GetTransactionId);
                    if (_TransactionData != null && _TransactionData.Rows.Count > 0)
                    {
                        appinformation.transactionid = Convert.ToString(_TransactionData.Rows[0]["transcationid"]);
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
                DataTable _CredoDt = CommonMethods.GettCredoScore(_ApplicationUserId);
                if (_CredoDt != null && _CredoDt.Rows.Count > 0)
                {
                    appinformation.CredoModel = new CredoModel()
                    {
                        Probability = (string)_CredoDt.Rows[0]["probability"],
                        Score = (string)_CredoDt.Rows[0]["score"]
                    };
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
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
            }

            return View(appinformation);

        }
        /// <summary>
        /// Transfer To Verifier and Upload Documents
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult RequestDetail(Chekerinformation model, string CommandName)
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.Checker))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            string ActionName = Convert.ToString(Session["CheckerActionName"]);
            ActivityLog.Info($"Application {model.applicationno} Post as {CommandName} on Checker Window by User {Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value)}", $"{this.GetType().Name} ==> {System.Reflection.MethodBase.GetCurrentMethod().Name}");
            try
            {
                if (model.applicationno != null && model.applicationno > 0)
                {
                    int user_id;
                    Chekerinformation appinformation = new Chekerinformation();
                    CommonOperation comnoperation = new CommonOperation();


                    string GovtFileName = string.Empty;
                    string CompanyFileName = string.Empty;
                    string BillFileName = string.Empty;
                    string IncomeFileName = string.Empty;
                    string OtherFileName = string.Empty;
                    string ATMFileName = string.Empty;
                    user_id = Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value);
                    appinformation = comnoperation.GetListbyID(model.applicationno.Value, ActionName, user_id);
                    if (model.GovIdFile == null && model.CompanyIdFile == null && model.BillingIdFile == null && model.IncomeIdFile == null && model.OtherIdFile == null && model.AtmIdFile == null && CommandName == "Save Upload Files")
                    {
                        TempData["Result"] = "No File was Selected for Upload";

                        return Redirect(Url.Action("RequestDetail", "LoanRequestBucket") + "?Id=" + model.applicationno.Value + "&Actionname=" + ActionName);
                    }
                    if (CommandName == "NEXT")
                    {
                        var PhoneNumber = appinformation.personalcontactno;
                        var AgentName = appinformation.name;
                        var GovtFileIdUrl = appinformation.gov_id_url;
                        var CompanyFileIdUrl = appinformation.companyid_url;
                        var BillFileIdUrl = appinformation.billing_url;
                        var IncomeFileIdUrl = appinformation.income_url;
                        var OtherFileIdUrl = appinformation.other_url;
                        var ATMFileIdURL = appinformation.atm_url;
                        comnoperation.TransferToReVerifier(model.applicationno.Value, user_id);

                        var _GovtFileIdUrl = Upload(model.GovIdFile, model.applicationno.Value, GovtFileIdUrl);
                        var _CompanyFileIdUrl = Upload(model.CompanyIdFile, model.applicationno.Value, CompanyFileIdUrl);
                        var _BillFileIdUrl = Upload(model.BillingIdFile, model.applicationno.Value, BillFileIdUrl);
                        var _IncomeFileIdUrl = Upload(model.IncomeIdFile, model.applicationno.Value, IncomeFileIdUrl);
                        var _OtherFileIdUrl = Upload(model.OtherIdFile, model.applicationno.Value, OtherFileIdUrl);
                        var _ATMFileIdURL = Upload(model.AtmIdFile, model.applicationno.Value, ATMFileIdURL);

                        GovtFileName = _GovtFileIdUrl == "" ? Convert.ToString(GovtFileIdUrl) : _GovtFileIdUrl;
                        CompanyFileName = _CompanyFileIdUrl == "" ? Convert.ToString(CompanyFileIdUrl) : _CompanyFileIdUrl;
                        BillFileName = _BillFileIdUrl == "" ? Convert.ToString(BillFileIdUrl) : _BillFileIdUrl;
                        IncomeFileName = _IncomeFileIdUrl == "" ? Convert.ToString(IncomeFileIdUrl) : _IncomeFileIdUrl;
                        OtherFileName = _OtherFileIdUrl == "" ? Convert.ToString(OtherFileIdUrl) : _OtherFileIdUrl;
                        ATMFileName = _ATMFileIdURL == "" ? Convert.ToString(ATMFileIdURL) : _ATMFileIdURL;

                        comnoperation.UpdateFiles(model.applicationno.Value, GovtFileName, CompanyFileName, BillFileName, IncomeFileName, OtherFileName, ATMFileName);
                        logger.AddWelcomeMessage(History.Application, "", "Documents Complete. Your application submitted for review. ", 0, true, model.applicationno.Value);
                        return RedirectToAction(ActionName);
                    }
                    else
                    {
                        var GovtFileIdUrl = appinformation.gov_id_url;
                        var CompanyFileIdUrl = appinformation.companyid_url;
                        var BillFileIdUrl = appinformation.billing_url;
                        var IncomeFileIdUrl = appinformation.income_url;
                        var OtherFileIdUrl = appinformation.other_url;
                        var ATMFileIdURL = appinformation.atm_url;


                        var _GovtFileIdUrl = Upload(model.GovIdFile, model.applicationno.Value, GovtFileIdUrl);
                        var _CompanyFileIdUrl = Upload(model.CompanyIdFile, model.applicationno.Value, CompanyFileIdUrl);
                        var _BillFileIdUrl = Upload(model.BillingIdFile, model.applicationno.Value, BillFileIdUrl);
                        var _IncomeFileIdUrl = Upload(model.IncomeIdFile, model.applicationno.Value, IncomeFileIdUrl);
                        var _OtherFileIdUrl = Upload(model.OtherIdFile, model.applicationno.Value, OtherFileIdUrl);
                        var _ATMFileIdURL = Upload(model.AtmIdFile, model.applicationno.Value, ATMFileIdURL);

                        GovtFileName = _GovtFileIdUrl == "" ? Convert.ToString(GovtFileIdUrl) : _GovtFileIdUrl;
                        CompanyFileName = _CompanyFileIdUrl == "" ? Convert.ToString(CompanyFileIdUrl) : _CompanyFileIdUrl;
                        BillFileName = _BillFileIdUrl == "" ? Convert.ToString(BillFileIdUrl) : _BillFileIdUrl;
                        IncomeFileName = _IncomeFileIdUrl == "" ? Convert.ToString(IncomeFileIdUrl) : _IncomeFileIdUrl;
                        OtherFileName = _OtherFileIdUrl == "" ? Convert.ToString(OtherFileIdUrl) : _OtherFileIdUrl;
                        ATMFileName = _ATMFileIdURL == "" ? Convert.ToString(ATMFileIdURL) : _ATMFileIdURL;

                        int i = comnoperation.UpdateFiles(model.applicationno.Value, GovtFileName, CompanyFileName, BillFileName, IncomeFileName, OtherFileName, ATMFileName);
                        if (i == 1)
                        {
                            TempData["Result"] = "File Uploaded Successfully";
                        }
                        else
                        {
                            TempData["Result"] = "Error In Uploading";
                        }

                        return Redirect(Url.Action("RequestDetail", "LoanRequestBucket") + "?Id=" + model.applicationno.Value + "&Actionname=" + ActionName);
                    }

                }
                else
                {
                    return Redirect(Url.Action("RequestDetail", "LoanRequestBucket") + "?Id=" + model.applicationno.Value + "&Actionname=" + ActionName);
                }

            }
            catch (Exception ex)
            {
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
            }
            return Redirect(Url.Action("RequestDetail", "LoanRequestBucket") + "?Id=" + model.applicationno.Value + "&Actionname=" + ActionName);
        }
        private string Upload(HttpPostedFileBase fileUpload, int ApplicationId, string OldFileName)
        {
            try
            {

                if (fileUpload != null && fileUpload.ContentLength > 0)
                {
                    ActivityLog.Info($"File {fileUpload.FileName} uploaded on Application {ApplicationId} on Checker Window by User {Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value)}", $"{this.GetType().Name} ==> {System.Reflection.MethodBase.GetCurrentMethod().Name}");
                    if (!String.IsNullOrWhiteSpace(OldFileName))
                        logger.DeleteFileOnServer(new Uri(OldFileName));
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
        public ActionResult UpdateDecline(int Id, string ContactNo, int userid, string Remaks)
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.Checker))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            try
            {
                ActivityLog.Info($"Application {Id} Declined on Checker Window by User {Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value)}", $"{this.GetType().Name} ==> {System.Reflection.MethodBase.GetCurrentMethod().Name}");

                bool flag = false;
                CommonOperation co = new CommonOperation();
                int a = co.UpdateDecline(Id, Remaks);
                if (a != 0)
                {
                    flag = true;
                }
                InsertRemark(Id, Remaks);
                if (flag)
                {
                    logger.AddWelcomeMessage(History.Application, "", "Your loan has been declined. You may reapply after 3 months.", 0, true, Id);
                    Thread thread = new Thread(() => logger.ClicktoSMS(ContactNo, Convert.ToString(ConfigurationManager.AppSettings["DeclineSmsBody"])));
                    thread.Start();
                }
                return Json(flag, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// 
        public ActionResult Index()
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.Checker))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            return View();
        }
        public int ClicktoCall(string ContactNumber, int userid, int applicationno)
        {
            try
            {
                ActivityLog.Info($"Call on {ContactNumber} from Application {applicationno} on Checker Window by User {Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value)}", $"{this.GetType().Name} ==> {System.Reflection.MethodBase.GetCurrentMethod().Name}");
                logger.AddWelcomeMessage(History.Application, "", "We are now trying to reach you. Keep your lines open.", 0, true, applicationno);
                return logger.ClicktoCall(ContactNumber, Convert.ToString(HttpContext.Request.Cookies[CookiesKey.AgentName].Value), Convert.ToString(Session["IPAddress"]));
            }
            catch (Exception ex)
            {
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                return 0;
            }

        }
        public ActionResult UpdateInsufficientDoc(int ApplicationId, string ContactNo, string Name, string Email)
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.Checker))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            bool status = false;
            try
            {
                ActivityLog.Info($"Application {ApplicationId} Updated as Insufficient Doc  on Checker Window by User {Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value)}", $"{this.GetType().Name} ==> {System.Reflection.MethodBase.GetCurrentMethod().Name}");
                int i = DbHelper.InsertUpdateDelete(string.Format(QueryHelper.UpdateInsufficientDoc, ApplicationId));
                if (i > 0)
                {
                    status = true;
                    if (status == true)
                    {
                        Thread t1 = new Thread(() => InsufficientSendSMS(ContactNo, ApplicationId, Name, Email));
                        t1.Start();
                    }
                }
            }
            catch (Exception ex)
            {
                status = false;
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));

            }

            return Json(status, JsonRequestBehavior.AllowGet);
        }
        public ActionResult InsufficientDocument()
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.Checker))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            return View();
        }
        [HttpPost]
        public ActionResult GetInsufficientData()
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.Checker))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            JsonResult result = new JsonResult();
            try
            {
                var RequestedForm = Request.Form;
                string search = String.Empty;
                int start = 10;
                int length = 10;
                string draw = "0";
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


                CommonOperation comnoperation1 = new CommonOperation();
                var users = comnoperation1.GetInsufficientDocBucketList();
                int totalRecords = users.Count;
                if (!string.IsNullOrEmpty(search) &&
                !string.IsNullOrWhiteSpace(search))
                {
                    // Apply search 
                    users = users.Where(p => p.applicationno.ToString().ToLower().Contains(search.ToLower()) ||
                    p.applicationname.ToString().ToLower().Contains(search.ToLower()) ||
                     p.RequestDate.ToString().ToLower().Contains(search.ToLower()) ||
                      p.MorningTime.ToString().ToLower().Contains(search.ToLower()) ||
                       p.NoonTime.ToString().ToLower().Contains(search.ToLower())).ToList();
                }

                users = this.SortByColumnWithOrder(order, orderDir, users);
                int recFilter = users.Count;
                users = users.Skip(start).Take(length).ToList();
                result = this.Json(new { draw = Convert.ToInt32(draw), recordsTotal = totalRecords, recordsFiltered = recFilter, data = users }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
            }
            return result;
        }
        public ActionResult InsertRemark(int ApplicationId, string Remark)
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.Checker))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            var flag = false;
            try
            {
                ActivityLog.Info($"Remark Inserted for Application {ApplicationId} on Checker Window by User {Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value)}", $"{this.GetType().Name} ==> {System.Reflection.MethodBase.GetCurrentMethod().Name}");
                string username = string.Empty;
                int userid = Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value);


                Remark = RemoveSpecialCharacters(Remark);
                var data = DbHelper.SelectMethod(string.Format(QueryHelper.GetUserName, userid));
                if (data != null)
                {
                    username = Convert.ToString(data.Rows[0]["userfullname"]);
                }
                int i = DbHelper.InsertUpdateDelete(string.Format(QueryHelper.InsertRemark, userid, username, ApplicationId, Remark, RemarkIdentifier.Checker));
                if (i > 0)
                {
                    flag = true;
                }
            }
            catch (Exception ex)
            {
                flag = false;
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
            }

            return Json(flag, JsonRequestBehavior.AllowGet);
        }
        //For Back Button
        public ActionResult UpdateIsPickedChecker(int ApplicationId)
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.Checker))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            var flag = false;
            try
            {
                ActivityLog.Info($"Application {ApplicationId} Picked on Checker Window by User {Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value)}", $"{this.GetType().Name} ==> {System.Reflection.MethodBase.GetCurrentMethod().Name}");
                int i = DbHelper.InsertUpdateDelete(string.Format(QueryHelper.UpdateIsPickedChecker, ApplicationId));
                if (i > 0)
                {
                    flag = true;
                }
            }
            catch (Exception ex)
            {
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                flag = false;
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
            }

            return Json(flag, JsonRequestBehavior.AllowGet);
        }
        public void InsufficientSendSMS(string ContactNumber, int Id, string Name, string Email)
        {
            try
            {
                ActivityLog.Info($"Insufficient SMS send on {ContactNumber} for Application {Id} on Checker Window by User {Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value)}", $"{this.GetType().Name} ==> {System.Reflection.MethodBase.GetCurrentMethod().Name}");
                if (ContactNumber[0] != '0' && ContactNumber.Length == 10)
                {
                    ContactNumber = '0' + ContactNumber;
                }
                string SMSBody = Convert.ToString(ConfigurationManager.AppSettings["InsufficientDocumnetSMSBody"]);
                string sURL = Convert.ToString(String.Format("{0}?do={1}&username={2}&userpass={3}&phone={4}&msg={5}", Convert.ToString(ConfigurationManager.AppSettings["SMSURL"]), Convert.ToString(ConfigurationManager.AppSettings["SMSAction"]), Convert.ToString(ConfigurationManager.AppSettings["SMSUserName"]), Convert.ToString(ConfigurationManager.AppSettings["SMSUserPassword"]), (ContactNumber == null ? "" : ContactNumber), (SMSBody == null ? "" : SMSBody)));
                string result = string.Empty;
                WebRequest wrGETURL;
                wrGETURL = WebRequest.Create(sURL);
                WebProxy myProxy = new WebProxy("myproxy", 80);
                myProxy.BypassProxyOnLocal = true;
                ServicePointManager.ServerCertificateValidationCallback = delegate (Object obj, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors) { return (true); };
                Stream objStream;
                objStream = wrGETURL.GetResponse().GetResponseStream();
                ;
                using (StreamReader objReader = new StreamReader(objStream))
                {
                    result = objReader.ReadToEnd();
                    ActivityLog.Info($"Insufficient Email send on {Email} for Application {Id} on Checker Window by User {Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value)}", $"{this.GetType().Name} ==> {System.Reflection.MethodBase.GetCurrentMethod().Name}");
                    InsufficientSendMail(Name, Email);
                }
            }
            catch (Exception ex)
            {

                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                throw ex;
            }
        }
        private void InsufficientSendMail(string Name, string Email)
        {
            try
            {
                StringBuilder EmailBody = new StringBuilder();
                EmailBody.AppendLine("<b>Dear " + Name + " ,</b><br/><br/>" + "<span>Thank you for choosing Cash Mart.</span><br/><br/><span>would like to confirm that we have received your inquiries and we are actively monitoring your loan application.</span> <br/><br/><span>We noticed your application is incomplete. </span><br/><br/><span>To help speed up your loan process and be verified within 24 hours, you need to send your required documents together with your duly completed loan application details.</span> <br/><br/><b>Complete your application in just 3 steps:</b> <br/><br/><ul><li style='display: inline;'><span><b>Step 1: </b>Login by going to cashmart.ph and then click “Client Login”. Use your email address as username and your generated password. If you are unsure about your password, you can use the “Forgot Password” link to retrieve the password in your email.</span><br/><br/></li><li style='display: inline;'><span><b>Step 2: </b>Once logged in, fill up all the required information in your Cash Mart account.</span><br/><br/></li><li style='display: inline;'><span><b>Step 3: </b>Upload all of your necessary documents. File size of each document is limited to 5MB and file type should be in jpeg or pdf format. If you are experiencing difficulties when uploading your documents, you can just send the following to info@cashmart.ph:</span><br/><br/></li><li><span><b>Government ID</b></span><br/></li><li><span><b>Company ID</b></span><br/></li><li><span><b>Proof of income</b></span><br/></li><li style='display: inline;'><ul><li><span><b>For employed -</b></span> latest 1 month payslip</li><li><span><b>For business owner -</b></span> business permit and 6-months latest bank statement</li><li><span><b>For OFW -</b></span> employment contract and latest 6-months remittance slip</li><li><span><b>For UBER/Grab/Taxi driver -</b></span> weekly income summary or summary of payments, Net of service fee, OR/CR and driver’s license</li></ul></li><li><span><b>Latest proof of billing</b>(electric bill, water bill, internet or telecom bills, credit card bills) NOTE: If proof of billing is not under your name, we will need you to provide an authorization letter of the person in the billing statement alongside with his signature and valid ID.</span><br/><br/></li></ul><span><b>Important Reminder:</b></span><br/><br/><span>Lack of information or documents may put your application on hold.</span><br/><br/><span>Please comply with the aforementioned application steps within 24 hours for us to process your loan the soonest.</span><br/><br/><span>We look forward to serving you.</span><br/><br/><span>Sincerely,</span><br/><span>Cash Mart</span>");
                new Thread(() => CommonMethods.SendMail(EmailFrom.FromInfo, Email, Convert.ToString(EmailBody), "Insufficient Documents")).Start();
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
        /// <summary>
        /// Get Remark
        /// </summary>
        /// <returns></returns>
        public ActionResult GetRemark(int appId)
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.Checker))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            try
            {
                var data = CommonMethods.GetRemarkList(appId);
                if (data != null && data.Count > 0)
                {
                    return Json(data, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(null, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                return Json(null, JsonRequestBehavior.AllowGet);
            }


        }
        /// <summary>
        /// Checker KIV Bucket
        /// </summary>
        /// <returns></returns>
        public ActionResult CheckerKIVBucket()
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.Checker))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            return View();
        }
        /// <summary>
        /// Get KIV Bucket Data
        /// </summary>
        /// <returns></returns>
        //public ActionResult GetKIVBucketData()
        //{
        //    try
        //    {
        //        int user_id;
        //        user_id = Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value);
        //        CommonOperation comnoperation1 = new CommonOperation();

        //        var jsonResult = Json(user_list, JsonRequestBehavior.AllowGet);
        //        jsonResult.MaxJsonLength = int.MaxValue;
        //        return jsonResult;
        //    }
        //    catch (Exception ex)
        //    {
        //        logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
        //        TempData["msg"] = "Something Went Wrong, Please check the Logs.";
        //        return Json(null, JsonRequestBehavior.AllowGet);
        //    }

        //}

        [HttpPost]
        public ActionResult GetKIVBucketData(int userid)
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.Checker))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            List<Checkerdetail> model = new List<Checkerdetail>();
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

                CommonOperation comnoperation1 = new CommonOperation();

                var users = comnoperation1.GetKIVBucketList(userid);
                model = comnoperation1.GetIsOrrFalseCheckerApplications(users);

                int totalRecords = model.Count;
                if (!string.IsNullOrEmpty(search) &&
                !string.IsNullOrWhiteSpace(search))
                {
                    // Apply search 
                    model = model.Where(p => p.applicationno.ToString().ToLower().Contains(search.ToLower()) ||
                    p.applicationname.ToString().ToLower().Contains(search.ToLower()) ||
                     p.RequestDate.ToString().ToLower().Contains(search.ToLower()) ||
                      p.MorningTime.ToString().ToLower().Contains(search.ToLower()) ||
                       p.NoonTime.ToString().ToLower().Contains(search.ToLower())).ToList();
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

        public ActionResult GetFilterData(string filter_text, string filter_date)
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.Checker))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            try
            {
                var split_date = filter_date.Replace(" - ", "_").Split('_');
                DateTime StartDate = Convert.ToDateTime(split_date[0].Trim());
                DateTime EndDate = Convert.ToDateTime(split_date[1].Trim());

                CommonOperation comnoperation1 = new CommonOperation();
                var users = comnoperation1.GetFilterBucketList(filter_text, StartDate, EndDate);
                var user_list = comnoperation1.GetIsOrrFalseCheckerApplications(users);
                var jsonResult = Json(user_list, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;

            }
            catch (Exception ex)
            {
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                return Json(null, JsonRequestBehavior.AllowGet);
            }

        }
        public ActionResult AddPersonalContact(int ApplicationId, string number)
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.Checker))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            var flag = false;
            try
            {
                ActivityLog.Info($"Personal Contact No {number} Added for Application {ApplicationId} on Checker Window by User {Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value)}", $"{this.GetType().Name} ==> {System.Reflection.MethodBase.GetCurrentMethod().Name}");
                string personal_contact = string.Empty;
                string contact = string.Empty;

                if (number != null && number.Trim() != "")
                {
                    number = RemoveSpecialCharacters(number);

                    var data = DbHelper.SelectMethod(string.Format(QueryHelper.GetCP, ApplicationId));
                    if (data != null)
                    {
                        personal_contact = Convert.ToString(data.Rows[0]["personalcontactno"]);
                    }
                    if (personal_contact == "")
                    {
                        contact = number;
                    }
                    else
                    {

                        if (personal_contact.Contains(","))
                        {
                            var number_list = personal_contact.Split(',');
                            foreach (var item in number_list)
                            {
                                if (item == number)
                                {
                                    return Json(flag, JsonRequestBehavior.AllowGet);
                                }
                            }
                        }
                        else
                        {
                            if (personal_contact == number)
                            {
                                return Json(flag, JsonRequestBehavior.AllowGet);
                            }
                        }
                        contact = personal_contact + ", " + number;
                    }
                    int i = DbHelper.InsertUpdateDelete(string.Format(QueryHelper.UpdateCP, ApplicationId, contact));
                    if (i > 0)
                    {
                        flag = true;
                    }
                }
            }
            catch (Exception ex)
            {
                flag = false;
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));

            }

            return Json(flag, JsonRequestBehavior.AllowGet);
        }
        public ActionResult AddCompanyPhoneNo(int ApplicationId, string number)
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.Checker))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            var flag = false;
            try
            {
                ActivityLog.Info($"Company Contact No {number} Added for Application {ApplicationId} on Checker Window by User {Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value)}", $"{this.GetType().Name} ==> {System.Reflection.MethodBase.GetCurrentMethod().Name}");
                string company_contact = string.Empty;
                string contact = string.Empty;

                if (number != null && number.Trim() != "")
                {
                    number = RemoveSpecialCharacters(number);
                    var data = DbHelper.SelectMethod(string.Format(QueryHelper.GetCompanyPhoneNo, ApplicationId));
                    if (data != null)
                    {
                        company_contact = Convert.ToString(data.Rows[0]["company_phoneno"]);
                    }
                    if (company_contact == "")
                    {
                        contact = number;
                    }
                    else
                    {

                        if (company_contact.Contains(","))
                        {
                            var number_list = company_contact.Split(',');
                            foreach (var item in number_list)
                            {
                                if (item == number)
                                {
                                    return Json(flag, JsonRequestBehavior.AllowGet);
                                }
                            }
                        }
                        else
                        {
                            if (company_contact == number)
                            {
                                return Json(flag, JsonRequestBehavior.AllowGet);
                            }
                        }
                        contact = company_contact + ", " + number;
                    }
                    int i = DbHelper.InsertUpdateDelete(string.Format(QueryHelper.UpdateCompanyPhoneNo, ApplicationId, contact));
                    if (i > 0)
                    {
                        flag = true;
                    }
                }
            }
            catch (Exception ex)
            {
                flag = false;
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));

            }

            return Json(flag, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult GetCheckerKIV()
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.Checker))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            CommonOperation comnoperation1 = new CommonOperation();
            bool flag = false;
            try
            {
                int UserId = Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId].Value == null ? "0" : HttpContext.Request.Cookies[CookiesKey.UserId].Value);
                if (Convert.ToInt32(comnoperation1.GetKIVBucketList(UserId).Count) >= Convert.ToInt32(ConfigurationManager.AppSettings["CheckerBucket_LeadCount"]))
                    flag = true;
            }
            catch (Exception ex)
            {
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                throw;
            }
            return Json(flag, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult GetUsersData(string filter_text, string filter_date)
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.Checker))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            List<Checkerdetail> model = new List<Checkerdetail>();
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

                CommonOperation comnoperation1 = new CommonOperation();
                if (filter_text != "All" && !string.IsNullOrWhiteSpace(filter_date))
                {
                    var split_date = filter_date.Replace(" - ", "_").Split('_');
                    DateTime StartDate = Convert.ToDateTime(split_date[0].Trim());
                    DateTime EndDate = Convert.ToDateTime(split_date[1].Trim());
                    var users = comnoperation1.GetFilterBucketList(filter_text, StartDate, EndDate);
                    model = comnoperation1.GetIsOrrFalseCheckerApplications(users);
                }
                else
                {
                    var users = comnoperation1.GetBucketList();
                    model = users.Where(x => x.is_orr_declined == false).ToList();
                }

                int UserId = Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId].Value == null ? "0" : HttpContext.Request.Cookies[CookiesKey.UserId].Value);
                Session["CheckerKIVCount"] = false;
                ViewBag.CheckerKivCount = false;
                var users_kiv = comnoperation1.GetKIVBucketList(UserId);
                var user_list_orr = users_kiv.Where(x => x.is_orr_declined == false).ToList();
                var CountKIV = users_kiv.Count;
                if (CountKIV > 0)
                {
                    if (Convert.ToInt32(CountKIV) >= Convert.ToInt32(ConfigurationManager.AppSettings["CheckerBucket_LeadCount"]))
                    {
                        Session["CheckerKIVCount"] = true;
                        ViewBag.CheckerKivCount = true;
                    }
                    else
                    {
                        Session["CheckerKIVCount"] = false;
                        ViewBag.CheckerKivCount = false;
                    }
                }

                int totalRecords = model.Count;
                if (!string.IsNullOrEmpty(search) &&
                !string.IsNullOrWhiteSpace(search))
                {
                    // Apply search 
                    model = model.Where(p => p.applicationno.ToString().ToLower().Contains(search.ToLower()) ||
                    p.applicationname.ToString().ToLower().Contains(search.ToLower()) ||
                     p.RequestDate.ToString().ToLower().Contains(search.ToLower()) ||
                      p.MorningTime.ToString().ToLower().Contains(search.ToLower()) ||
                       p.NoonTime.ToString().ToLower().Contains(search.ToLower())).ToList();
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
        private List<Checkerdetail> SortByColumnWithOrder(string order, string orderDir, List<Checkerdetail> ApplicationRecordList)
        {
            List<Checkerdetail> lst = new List<Checkerdetail>();
            try
            {
                switch (order)
                {
                    case "0":
                        lst = orderDir.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? ApplicationRecordList.OrderByDescending(p => p.applicationno).ToList() : ApplicationRecordList.OrderBy(p => p.applicationno).ToList();
                        break;
                    case "1":
                        lst = orderDir.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? ApplicationRecordList.OrderByDescending(p => p.applicationname).ToList() : ApplicationRecordList.OrderBy(p => p.applicationname).ToList();
                        break;
                    case "2":
                        lst = orderDir.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? ApplicationRecordList.OrderByDescending(p => p.AppliedOn).ToList() : ApplicationRecordList.OrderBy(p => p.AppliedOn).ToList();
                        break;
                    case "3":
                        lst = orderDir.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? ApplicationRecordList.OrderByDescending(p => p.MorningTime).ToList() : ApplicationRecordList.OrderBy(p => p.MorningTime).ToList();
                        break;
                    case "4":
                        lst = orderDir.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? ApplicationRecordList.OrderByDescending(p => p.NoonTime).ToList() : ApplicationRecordList.OrderBy(p => p.NoonTime).ToList();
                        break;
                    default:
                        lst = orderDir.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? ApplicationRecordList.OrderByDescending(p => p.applicationno).ToList() : ApplicationRecordList.OrderBy(p => p.applicationno).ToList();
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

        public ActionResult GetNotificationValues(int Id)
        {

            try
            {
                NotificationTemplate sms = new NotificationTemplate();
                var data = DbHelper.SelectMethod(string.Format(QueryHelper.GetActiveNotification_ById, Id));
                if (data != null && data.Rows.Count > 0)
                {
                    sms.NotificationTemplateDefinition = Convert.ToString(data.Rows[0]["template_definition"]);
                    sms.NotificationTemplateDesc = Convert.ToString(data.Rows[0]["template_description"]);
                }
                return Json(sms, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                return null;
            }

        }
    }
}