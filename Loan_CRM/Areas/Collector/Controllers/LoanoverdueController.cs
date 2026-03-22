using Ionic.Zip;

using Loan_CRM.Areas.Checker.Models;
using Loan_CRM.Areas.Collector.Models;
using Loan_CRM.Models;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web.Mvc;
using System.Web.WebPages;

namespace Loan_CRM.Areas.Collector.Controllers
{
    public class LoanoverdueController : Controller
    {
        readonly CommonOperation _commonOperation = new CommonOperation();
        readonly ApproverCommonOperation approverCommonOperation = new ApproverCommonOperation();


        [HttpPost]
        public ActionResult GetCalcPenalityAmount(int Id, string Date)
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.Collector))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            PaymentInformation paymentInformation = new PaymentInformation();
            try
            {
                if (String.IsNullOrWhiteSpace(Date))
                {
                    Date = DateTime.Now.ToString("dd-MM-yyyy");
                }
                var _Dates = Date.Split('-').ToArray();
                CommonOperation comnoperation = new CommonOperation();
                paymentInformation = comnoperation.GetCalculatedPenalityByDateApplicationNo(Id, new DateTime(Convert.ToInt32(_Dates[2]), Convert.ToInt32(_Dates[1]), Convert.ToInt32(_Dates[0])));
            }
            catch (Exception ex)
            {
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                paymentInformation = new PaymentInformation();
            }
            return Json(paymentInformation);
        }

        /// <summary>
        /// Loan Detail By ID
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult LoanDetail(int Id, string ActionName)
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.Collector))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            PaymentInformation listofpayment = new PaymentInformation();
            try
            {
                ActivityLog.Info($"Application {Id} Get Open on Collector Window by User {Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value)}", $"{this.GetType().Name} ==> {System.Reflection.MethodBase.GetCurrentMethod().Name}");
                CommonOperation comnoperation = new CommonOperation();

                string applicationNo = Request.QueryString["Id"];
                Session["ApplicationNumber"] = applicationNo;

                TempData["ActionName"] = ActionName;
                Session["ActionName"] = ActionName;
                string a = Convert.ToString(System.Web.HttpContext.Current.Request.UrlReferrer);

                listofpayment = comnoperation.GetCollecterListbyPhoneNumber(Id, ActionName);


                Session["Name"] = listofpayment.Name;
                Session["ReferenceNo"] = listofpayment.Referenceno;
                Session["ApplicationNo"] = listofpayment.applicationno;
                listofpayment.RemarkModelList.AddRange(CommonMethods.GetRemarkList(Convert.ToInt32(applicationNo)));
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
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";

            }
            return View(listofpayment);

        }

        [HttpGet]
        public ActionResult LoanPaymentBucket()
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.Collector))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            try
            {
                ViewBag.Reason = CommonMethods.getReason();
                ViewBag.PaymentChannel = CommonMethods.GetPaymentChannel();
                ViewBag.ProofOfPayment = CommonMethods.GetProofOfPayment();
            }
            catch (Exception ex)
            {
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";

            }
            return View();
        }

        public ActionResult GetLoanData(string appNo, string number, string applicationno)
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.Collector))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            List<PaymentInformation> FinalList = new List<PaymentInformation>();
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

                ActivityLog.Info($"Application {appNo} Get Loan Details Open on Collector Window by User {Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value)}", $"{this.GetType().Name} ==> {System.Reflection.MethodBase.GetCurrentMethod().Name}");
                int UserId = Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value);
                CommonOperation comnoperation1 = new CommonOperation();

                if (number == "1" && !String.IsNullOrWhiteSpace(applicationno))
                {
                    FinalList = GetLoanDataByID(applicationno);
                }
                else if (number == "2" && !String.IsNullOrWhiteSpace(applicationno))
                {
                    FinalList = GetLoanDataByName(applicationno);
                }
                else if (number == "3" && !String.IsNullOrWhiteSpace(applicationno))
                {
                    FinalList = GetLoanDataByReference(applicationno);
                }
                else
                {
                    FinalList = comnoperation1.GetLoanData(UserId);
                }

                int totalRecords = FinalList.Count;
                int recFilter = FinalList.Count;
                FinalList = FinalList.Skip(start).Take(length).ToList();
                result = this.Json(new { draw = Convert.ToInt32(draw), recordsTotal = totalRecords, recordsFiltered = recFilter, data = FinalList }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
            }
            return result;
        }

        public List<PaymentInformation> GetLoanDataByID(string ApplicationID)
        {
            List<PaymentInformation> FinalList = new List<PaymentInformation>();
            try
            {
                ActivityLog.Info($"Application {ApplicationID} Get Loan Details Open on Collector Window by User {Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value)}", $"{this.GetType().Name} ==> {System.Reflection.MethodBase.GetCurrentMethod().Name}");
                int UserId = Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value);
                CommonOperation comnoperation1 = new CommonOperation();
                List<PaymentInformation> users = comnoperation1.GetLoanDataById(ApplicationID);

                foreach (var item in users.Where(x => x.SortingOrder == 2).OrderBy(x => x.EMIDetailsID).ToList())
                {
                    var _UniqueRecord = FinalList.FirstOrDefault(x => x.applicationno == item.applicationno);
                    if (_UniqueRecord == null)
                    {
                        FinalList.Add(item);
                    }
                }

                foreach (var item in users.Where(x => x.SortingOrder == 1).OrderBy(x => x.EMIDetailsID).ToList())
                {
                    var _UniqueRecord = FinalList.FirstOrDefault(x => x.applicationno == item.applicationno);
                    if (_UniqueRecord == null)
                    {
                        FinalList.Add(item);
                    }
                }
                foreach (var item in users.Where(x => x.SortingOrder == 0).OrderBy(x => x.EMIDetailsID).ToList())
                {
                    var _UniqueRecord = FinalList.FirstOrDefault(x => x.applicationno == item.applicationno);
                    if (_UniqueRecord == null)
                    {
                        FinalList.Add(item);
                    }
                }
                return FinalList;
            }
            catch (Exception ex)
            {
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                return null;
            }

        }

        public List<PaymentInformation> GetLoanDataByName(string FirstName)
        {
            List<PaymentInformation> FinalList = new List<PaymentInformation>();
            try
            {
                int UserId = Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value);
                CommonOperation comnoperation1 = new CommonOperation();
                List<PaymentInformation> users = comnoperation1.GetLoanDataByName(FirstName);
                foreach (var item in users.Where(x => x.SortingOrder == 2).OrderBy(x => x.EMIDetailsID).ToList())
                {
                    var _UniqueRecord = FinalList.FirstOrDefault(x => x.applicationno == item.applicationno);
                    if (_UniqueRecord == null)
                    {
                        FinalList.Add(item);
                    }
                }

                foreach (var item in users.Where(x => x.SortingOrder == 1).OrderBy(x => x.EMIDetailsID).ToList())
                {
                    var _UniqueRecord = FinalList.FirstOrDefault(x => x.applicationno == item.applicationno);
                    if (_UniqueRecord == null)
                    {
                        FinalList.Add(item);
                    }
                }
                foreach (var item in users.Where(x => x.SortingOrder == 0).OrderBy(x => x.EMIDetailsID).ToList())
                {
                    var _UniqueRecord = FinalList.FirstOrDefault(x => x.applicationno == item.applicationno);
                    if (_UniqueRecord == null)
                    {
                        FinalList.Add(item);
                    }
                }
                return FinalList;
            }
            catch (Exception ex)
            {

                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                return null;
            }
        }

        public List<PaymentInformation> GetLoanDataByReference(string Reference)
        {
            List<PaymentInformation> FinalList = new List<PaymentInformation>();
            try
            {
                int UserId = Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value);
                CommonOperation comnoperation1 = new CommonOperation();
                List<PaymentInformation> users = comnoperation1.GetLoanDataByReference(Reference);
                foreach (var item in users.Where(x => x.SortingOrder == 2).OrderBy(x => x.EMIDetailsID).ToList())
                {
                    var _UniqueRecord = FinalList.FirstOrDefault(x => x.applicationno == item.applicationno);
                    if (_UniqueRecord == null)
                    {
                        FinalList.Add(item);
                    }
                }

                foreach (var item in users.Where(x => x.SortingOrder == 1).OrderBy(x => x.EMIDetailsID).ToList())
                {
                    var _UniqueRecord = FinalList.FirstOrDefault(x => x.applicationno == item.applicationno);
                    if (_UniqueRecord == null)
                    {
                        FinalList.Add(item);
                    }
                }
                foreach (var item in users.Where(x => x.SortingOrder == 0).OrderBy(x => x.EMIDetailsID).ToList())
                {
                    var _UniqueRecord = FinalList.FirstOrDefault(x => x.applicationno == item.applicationno);
                    if (_UniqueRecord == null)
                    {
                        FinalList.Add(item);
                    }
                }
                return FinalList;
            }
            catch (Exception ex)
            {
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                return null;
            }

        }

        [HttpPost]
        public ActionResult LoanDetail(PaymentInformation paymentdetail)
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.Collector))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            string ActionName = "LoanPaymentBucket";
            try
            {

                if (Session["ActionName"] != null)
                {
                    ActionName = Convert.ToString(Session["ActionName"]);
                }
                paymentdetail.applicationno = Convert.ToInt32(Session["ApplicationNo"]);
                paymentdetail.Referenceno = Convert.ToString(Session["ReferenceNo"]);
                paymentdetail.updatedby = Convert.ToString(HttpContext.Request.Cookies[CookiesKey.UserId].Value);
                CommonOperation comnoperation = new CommonOperation();
                comnoperation.Insertloandetail(paymentdetail);
                Int64 Applicationno = Convert.ToInt64(paymentdetail.applicationno);
            }
            catch (Exception ex)
            {
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";

            }
            return RedirectToAction(ActionName);
        }

        public ActionResult Index()
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.Collector))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            return View();
        }

        public ActionResult MakeDefaulter(int AppId)
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.Collector))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            ActivityLog.Info($"Application {AppId} Makes as Defaulter on Collector Window by User {Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value)}", $"{this.GetType().Name} ==> {System.Reflection.MethodBase.GetCurrentMethod().Name}");
            bool flag = false;
            try
            {
                int i = DbHelper.InsertUpdateDelete(string.Format(QueryHelper.UpdateDefaulter, AppId));
                if (i > 0)
                {
                    flag = true;
                }
                return Json(flag, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                return null;
            }

        }

        public ActionResult RemoveDefaulter(int AppId)
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.Collector))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            bool flag = false;
            try
            {
                ActivityLog.Info($"Application {AppId} Removed from Defaulter on Collector Window by User {Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value)}", $"{this.GetType().Name} ==> {System.Reflection.MethodBase.GetCurrentMethod().Name}");
                int i = DbHelper.InsertUpdateDelete(string.Format(QueryHelper.UpdateRemoveDefaulter, AppId));
                if (i > 0)
                {
                    flag = true;
                }
                return Json(flag, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                return null;
            }

        }

        /// <summary>
        /// Defaulter Bucket 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult DefaulterBucket()
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.Collector))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            DefaulterModelVM defaulterModel = new DefaulterModelVM();
            try
            {
                List<Loan_CRM.Models.PaymentChannel> _paymentChannel = new List<Loan_CRM.Models.PaymentChannel>();
                List<Loan_CRM.Models.ProofOfPayment> _proofOfPayment = new List<Loan_CRM.Models.ProofOfPayment>();
                _paymentChannel = CommonMethods.GetPaymentChannel();
                _proofOfPayment = CommonMethods.GetProofOfPayment();
                defaulterModel.PaymentChannelList = _paymentChannel;
                defaulterModel.ProofOfPaymentList = _proofOfPayment;
                defaulterModel.DefaulterModelList.AddRange(DefaulterBucketdata());
            }
            catch (Exception ex)
            {
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";

            }
            return View(defaulterModel);
        }

        /// <summary>
        /// Defaulter Bucket 
        /// </summary>
        /// <returns></returns>
        public List<DefaulterModel> DefaulterBucketdata()
        {
            List<DefaulterModel> DefaulterList = new List<DefaulterModel>();
            List<DefaulterModel> FinalList = new List<DefaulterModel>();
            try
            {
                int UserId = Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value);
                var query = DbHelper.SelectMethod(String.Format(QueryHelper.GetDefaulterBucketById, UserId));
                if (query != null && query.Rows.Count > 0)
                {
                    for (int i = 0; i < query.Rows.Count; i++)
                    {
                        DefaulterModel defaulter = new DefaulterModel
                        {
                            aging = Convert.ToInt32(query.Rows[i]["aging"]),
                            defaulter_id = Convert.ToInt32(query.Rows[i]["defaulter_userid"]),
                            name = Convert.ToString(query.Rows[i]["first_name"]) + " " + Convert.ToString(query.Rows[i]["middle_name"]) + " " + Convert.ToString(query.Rows[i]["last_name"]),
                            application_no = Convert.ToInt64(query.Rows[i]["applicationno"]),
                            refernce_no = Convert.ToString(query.Rows[i]["reference_no"]),
                            outstanding_amount = Convert.ToString(query.Rows[i]["outstanding_amount"]) == null ? 0.0D : Convert.ToDouble(query.Rows[i]["outstanding_amount"]),
                            personalcontactno = Convert.ToString(query.Rows[i]["personalcontactno"]),
                            personal_email = Convert.ToString(query.Rows[i]["personalemail"]),
                            EMIDetailsID = Convert.ToInt32(query.Rows[i]["emi_detail_id"]),
                            complete_outstanding_amount = Convert.ToString(query.Rows[i]["complete_outstanding_amount"]),
                            ptpdate = Convert.ToString(query.Rows[i]["ptp_date"]) == String.Empty ? Convert.ToDateTime(query.Rows[i]["emi_date"]) : Convert.ToDateTime(query.Rows[i]["ptp_date"]),
                            isptp = Convert.ToString(query.Rows[i]["ptp_date"]) != String.Empty
                        };
                        #region Changes Done by Nagesh for Penalty Amount in Defaulter Payment Bucket

                        int PastEmiCount = 0;
                        var PassedEMICount = DbHelper.SelectMethod(string.Format(QueryHelper.PastEmiCount, defaulter.application_no));
                        if (PassedEMICount != null && PassedEMICount.Rows.Count > 0)
                        {
                            PastEmiCount = PassedEMICount.Rows[0]["emicount"] == DBNull.Value ? 0 : Convert.ToInt32(PassedEMICount.Rows[0]["emicount"]);
                        }
                        double PercentPenalty = (Convert.ToDouble(Convert.ToString(query.Rows[i]["late_rate"])) / 100);
                        double penalty = Convert.ToDouble(Convert.ToString(query.Rows[i]["late_penalty"]));
                        double pastduedays = Convert.ToDouble(query.Rows[i]["aging"]);
                        if (defaulter.outstanding_amount > 0 && pastduedays > 0)
                        {
                            if (Convert.ToString(query.Rows[i]["termtype"]) == "Weekly")
                            {
                                double _Mode = 0;
                                if (pastduedays != 0 && pastduedays < 7)
                                {
                                    _Mode = 1;
                                }
                                else if (pastduedays == 0)
                                {
                                    _Mode = 0;
                                }
                                else
                                {
                                    _Mode = (pastduedays % 7);
                                }
                                Double _PenalityCount = Convert.ToDouble(pastduedays / 7);
                                List<string> s = Convert.ToString(_PenalityCount).Split('.').ToList();
                                _PenalityCount = Convert.ToDouble(s.First());
                                int _TotalPenalityCount = _Mode > 0 ? Convert.ToInt32(_PenalityCount + 1) : Convert.ToInt32(_PenalityCount);
                                defaulter.latefee = Math.Round((_TotalPenalityCount * penalty), 2);
                                defaulter.penalty = (((defaulter.outstanding_amount) * (PercentPenalty)) * (pastduedays));
                                defaulter.latepenalty = Math.Round((defaulter.latefee + defaulter.penalty), 2);
                            }
                            if (Convert.ToString(query.Rows[i]["termtype"]) == "Bi-Weekly")
                            {
                                double _Mode = 0;
                                if (pastduedays != 0 && pastduedays < 14)
                                {
                                    _Mode = 1;
                                }
                                else if (pastduedays == 0)
                                {
                                    _Mode = 0;
                                }
                                else
                                {
                                    _Mode = (pastduedays % 14);
                                }
                                Double _PenalityCount = Convert.ToDouble(pastduedays / 14);
                                List<string> s = Convert.ToString(_PenalityCount).Split('.').ToList();
                                _PenalityCount = Convert.ToDouble(s.First());
                                int _TotalPenalityCount = _Mode > 0 ? Convert.ToInt32(_PenalityCount + 1) : Convert.ToInt32(_PenalityCount);
                                defaulter.latefee = Math.Round((_TotalPenalityCount * penalty), 2);
                                defaulter.penalty = (((defaulter.outstanding_amount) * (PercentPenalty)) * (pastduedays));
                                defaulter.latepenalty = Math.Round((defaulter.latefee + defaulter.penalty), 2);
                            }
                            if (Convert.ToString(query.Rows[i]["termtype"]) == "Monthly")
                            {
                                double _Mode = 0;
                                if (pastduedays != 0 && pastduedays < 28)
                                {
                                    _Mode = 1;
                                }
                                else if (pastduedays == 0)
                                {
                                    _Mode = 0;
                                }
                                else
                                {
                                    _Mode = (pastduedays % 28);
                                }
                                Double _PenalityCount = Convert.ToDouble(pastduedays / 28);
                                List<string> s = Convert.ToString(_PenalityCount).Split('.').ToList();
                                _PenalityCount = Convert.ToDouble(s.First());
                                int _TotalPenalityCount = _Mode > 0 ? Convert.ToInt32(_PenalityCount + 1) : Convert.ToInt32(_PenalityCount);
                                defaulter.latefee = Math.Round((_TotalPenalityCount * penalty), 2);
                                defaulter.penalty = (((defaulter.outstanding_amount) * (PercentPenalty)) * (pastduedays));
                                defaulter.latepenalty = Math.Round((defaulter.latefee + defaulter.penalty), 2);
                            }
                        }
                        defaulter.latepenalty = defaulter.latepenalty > 0 ? defaulter.latepenalty : 0;

                        #endregion

                        DefaulterList.Add(defaulter);
                    }

                    List<DefaulterModel> TempList = new List<DefaulterModel>();

                    foreach (var item in DefaulterList.OrderBy(x => x.EMIDetailsID).ToList())
                    {
                        var _UniqueRecord = TempList.FirstOrDefault(x => x.application_no == item.application_no);
                        if (_UniqueRecord == null)
                        {
                            TempList.Add(item);
                        }
                    }
                    long _Applicationno = 0;
                    foreach (var item in TempList.OrderByDescending(x => x.application_no).ToList())
                    {
                        if (_Applicationno != item.application_no)
                        {
                            FinalList.Add(item);
                            _Applicationno = item.application_no;
                        }
                    }
                }
                return FinalList;
            }
            catch (Exception ex)
            {
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                throw ex;
            }

        }

        public DefaulterModel DefaulterSMSData(int AppId)
        {
            DefaulterModel defaulter = new DefaulterModel();
            try
            {
                ActivityLog.Info($"Deafuler SMS Send on Application {AppId} on Collector Window by User {Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value)}", $"{this.GetType().Name} ==> {System.Reflection.MethodBase.GetCurrentMethod().Name}");
                int UserId = Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value);
                var query = DbHelper.SelectMethod(string.Format(QueryHelper.GetDefaulterSMSData, AppId));
                if (query != null && query.Rows.Count > 0)
                {
                    for (int i = 0; i < query.Rows.Count; i++)
                    {


                        defaulter.application_no = Convert.ToInt64(query.Rows[0]["applicationno"]);
                        defaulter.refernce_no = Convert.ToString(query.Rows[0]["reference_no"]);
                        defaulter.outstanding_amount = Convert.ToString(query.Rows[0]["outstanding_amount"]) == null ? 0.0D : Convert.ToDouble(query.Rows[i]["outstanding_amount"]);
                        defaulter.personalcontactno = Convert.ToString(query.Rows[0]["personalcontactno"]);


                    }
                }
                return defaulter;
            }
            catch (Exception ex)
            {
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                throw ex;
            }

        }

        public ActionResult GetDefaulterData()
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.Collector))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            List<DefaulterModel> DefaulterList = new List<DefaulterModel>();
            try
            {
                int UserId = Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value);
                var query = DbHelper.SelectMethod(QueryHelper.GetDefaulterBucket);
                if (query != null && query.Rows.Count > 0)
                {
                    for (int i = 0; i < query.Rows.Count; i++)
                    {
                        DefaulterModel defaulter = new DefaulterModel
                        {
                            aging = Convert.ToInt32(query.Rows[i]["aging"]),
                            defaulter_id = Convert.ToInt32(query.Rows[i]["defaulter_userid"]),
                            name = Convert.ToString(query.Rows[i]["first_name"]) + " " + Convert.ToString(query.Rows[i]["middle_name"]) + " " + Convert.ToString(query.Rows[i]["last_name"]),
                            application_no = Convert.ToInt64(query.Rows[i]["applicationno"]),
                            refernce_no = Convert.ToString(query.Rows[i]["reference_no"]),
                            outstanding_amount = Convert.ToString(query.Rows[i]["outstanding_amount"]) == null ? 0.0D : Convert.ToDouble(query.Rows[i]["outstanding_amount"]),
                            personalcontactno = Convert.ToString(query.Rows[i]["personalcontactno"]),
                            personal_email = Convert.ToString(query.Rows[i]["personalemail"])
                        };
                        if (defaulter.outstanding_amount > 0 && defaulter.defaulter_id == UserId)
                        {
                            DefaulterList.Add(defaulter);
                        }
                    }
                }
                var jsonResult = Json(DefaulterList, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;

            }
            catch (Exception ex)
            {
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                return null;
            }

        }

        [HttpGet]
        public ActionResult LOD_SMS_MAIL(int AppId, string Name, string ContractNumber, string Date, decimal OutstandingBalance, decimal Penality, decimal LateFee, decimal TotalAmount, string EmailId, string emidate)
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.Collector))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            string FileName = String.Empty;
            try
            {
                FileName = CommonMethods.CreateLOD_PDF(AppId, Name, ContractNumber, Date, OutstandingBalance, LateFee, Penality, TotalAmount, emidate, Server);
                if (!String.IsNullOrWhiteSpace(FileName))
                {
                    int i = DbHelper.InsertUpdateDelete(string.Format(QueryHelper.InsertLOD, AppId, FileName));
                    if (i > 0)
                    {
                        SendMail(Name, FileName, EmailId);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
            }
            return Json(FileName, JsonRequestBehavior.AllowGet);
        }

        public FileResult Download(string FilePath, string FileName)
        {
            try
            {
                if (!String.IsNullOrWhiteSpace(FilePath))
                {
                    byte[] fileBytes = System.IO.File.ReadAllBytes(FilePath);
                    var fileData = FilePath.Split('\\');
                    return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileData[fileData.Length - 1]);
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                throw ex;
            }
        }

        public void SendMail(string Name, string LOD_Path, string EmailId)
        {
            try
            {
                StringBuilder EmailBody = new StringBuilder();
                EmailBody.AppendLine("<p>Dear " + Convert.ToString(Name) + "," + "</p><p>Please Find the Attachment</p>");
                new Thread(() => CommonMethods.SendMail(EmailFrom.FromPay, EmailId, Convert.ToString(EmailBody), "Letter of Demand", LOD_Path)).Start();
            }
            catch (Exception ex)
            {
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                throw ex;
            }
        }

        public ActionResult GetPaymentBucketReport(string currentDate)
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.Collector))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            List<PaymentBucket> paymentBucketList = new List<PaymentBucket>();
            try
            {
                if (!string.IsNullOrWhiteSpace(currentDate))
                {
                    DateTime currDate = Convert.ToDateTime(currentDate);
                    string dateStr = string.Empty;
                    dateStr = currDate.Year + "-" + currDate.Month + "-" + currDate.Day;
                    currentDate = dateStr;
                }
                var query = DbHelper.SelectMethod(string.Format(QueryHelper.GetPaymentBucketListByDate, currentDate));
                if (query != null && query.Rows.Count > 0)
                {
                    for (int i = 0; i < query.Rows.Count; i++)
                    {
                        PaymentBucket paymentBucket = new PaymentBucket
                        {
                            Id = 1 + Convert.ToInt32(i),
                            applicationname = Convert.ToString(query.Rows[i]["first_name"]) + " " + Convert.ToString(query.Rows[i]["middle_name"]) + " " + Convert.ToString(query.Rows[i]["last_name"]),
                            applicationno = Convert.ToInt32(query.Rows[i]["applicationno"]),
                            RequestDate = Convert.ToString(query.Rows[i]["dateapplied"]),
                            personalcontactno = Convert.ToString(query.Rows[i]["personalcontactno"]),
                            ContractNo = Convert.ToString(query.Rows[i]["contract_ref_no"]) != string.Empty ? Convert.ToString(query.Rows[i]["contract_ref_no"]) : string.Empty,
                            EmiDate = Convert.ToString(query.Rows[i]["emi_date"]) != "" ? Convert.ToString(query.Rows[i]["emi_date"]) : string.Empty,
                            PTPDate = Convert.ToString(query.Rows[i]["current_date_ptp"]) != "" ? Convert.ToString(query.Rows[i]["current_date_ptp"]) : string.Empty
                        };

                        paymentBucketList.Add(paymentBucket);

                    }
                }
                return Json(paymentBucketList);
            }
            catch (Exception ex)
            {
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                return null;
            }

        }

        public ActionResult UpdateIsPickedForCollector(int AppId)
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.Collector))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            bool flag = false;
            try
            {
                int i = DbHelper.InsertUpdateDelete(string.Format(QueryHelper.UpdateIsPickedForCollector, AppId));
                if (i > 0)
                {
                    flag = true;
                }
                return Json(flag, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";

                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name)); return null;
            }
        }

        public bool SendSMS(int Id, string mobno, string amount, string referenceNo)
        {
            bool flag = false;
            try
            {
                if (mobno[0] != '0' && mobno.Length == 10)
                {
                    mobno = '0' + mobno;
                }
                string username = Convert.ToString(ConfigurationManager.AppSettings["SMSUserName"]);
                string senderpassword = Convert.ToString(ConfigurationManager.AppSettings["SMSUserPassword"]);
                string port_num = Convert.ToString(ConfigurationManager.AppSettings["SMSPortNumber"]);
                string result = string.Empty;
                string SMSBody = string.Format(Convert.ToString(ConfigurationManager.AppSettings["SMSCollectionBucket"]), referenceNo, amount);
                flag = logger.ClicktoSMS(mobno, SMSBody);
                return flag;
            }
            catch (Exception ex)
            {
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name)); return false;
            }

        }

        public ActionResult ClickToSMS(int AppId, string mobno, string amount, string referenceNo)
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.Collector))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            bool res = false;
            try
            {
                res = SendSMS(AppId, mobno, amount, referenceNo);
                return Json(res, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name)); return null;
            }
        }

        public ActionResult InsertRemark(int ApplicationId, string Remark)
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.Collector))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            bool flag = false;
            try
            {
                string username = string.Empty;
                int userid = Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value);


                Remark = RemoveSpecialCharacters(Remark);

                var data = DbHelper.SelectMethod(string.Format(QueryHelper.GetUserName, userid));
                if (data != null)
                {
                    username = Convert.ToString(data.Rows[0]["userfullname"]);
                }
                int i = DbHelper.InsertUpdateDelete(string.Format(QueryHelper.InsertRemark, userid, username, ApplicationId, Remark, RemarkIdentifier.Collector));
                if (i > 0)
                {
                    flag = true;
                }
                return Json(flag, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";

                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name)); return null;
                //flag = false;
            }


        }

        public string RemoveSpecialCharacters(string str)
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

        public ActionResult AddContact(int ApplicationId, string number, string column)
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.Collector))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            string personal_contact = string.Empty;
            string contact = string.Empty;
            var flag = false;
            int i = 0;
            try
            {
                if (number != null && number.Trim() != "")
                {
                    number = RemoveSpecialCharacters(number);
                    if (column == "CP")
                    {
                        #region Get and Update Contact
                        var data = DbHelper.SelectMethod(string.Format(QueryHelper.GetCP, ApplicationId));
                        if (data != null)
                        {
                            personal_contact = Convert.ToString(data.Rows[0]["personalcontactno"]);
                        }
                        contact = personal_contact == "" ? number : personal_contact + "," + number;
                        i = DbHelper.InsertUpdateDelete(string.Format(QueryHelper.UpdateCP, ApplicationId, contact));
                        #endregion
                    }
                    else if (column == "Home")
                    {
                        #region Get and Update Contact
                        var data = DbHelper.SelectMethod(string.Format(QueryHelper.GetHome, ApplicationId));
                        if (data != null)
                        {
                            personal_contact = Convert.ToString(data.Rows[0]["homephoneno"]);
                        }
                        if (personal_contact == "")
                        {
                            contact = number;
                        }
                        else
                        {
                            contact = personal_contact + "," + number;
                        }
                        i = DbHelper.InsertUpdateDelete(string.Format(QueryHelper.UpdateHome, ApplicationId, contact));
                        #endregion
                    }
                    else if (column == "Office")
                    {
                        #region Get and Update Contact
                        var data = DbHelper.SelectMethod(string.Format(QueryHelper.GetOffice, ApplicationId));
                        if (data != null)
                        {
                            personal_contact = Convert.ToString(data.Rows[0]["company_phoneno"]);
                        }
                        if (personal_contact == "")
                        {
                            contact = number;
                        }
                        else
                        {
                            contact = personal_contact + "," + number;
                        }
                        i = DbHelper.InsertUpdateDelete(string.Format(QueryHelper.UpdateOffice, ApplicationId, contact));
                        #endregion
                    }
                    else if (column == "Refrence1")
                    {
                        #region Get and Update Contact
                        var data = DbHelper.SelectMethod(string.Format(QueryHelper.GetReference1, ApplicationId));
                        if (data != null)
                        {
                            personal_contact = Convert.ToString(data.Rows[0]["family_contact1"]);
                        }
                        if (personal_contact == "")
                        {
                            contact = number;
                        }
                        else
                        {
                            contact = personal_contact + "," + number;
                        }
                        i = DbHelper.InsertUpdateDelete(string.Format(QueryHelper.UpdateReference1, ApplicationId, contact));
                        #endregion
                    }
                    else
                    {
                        #region Get and Update Contact
                        var data = DbHelper.SelectMethod(string.Format(QueryHelper.GetReference2, ApplicationId));
                        if (data != null)
                        {
                            personal_contact = Convert.ToString(data.Rows[0]["family_contact2"]);
                        }
                        if (personal_contact == "")
                        {
                            contact = number;
                        }
                        else
                        {
                            contact = personal_contact + "," + number;
                        }
                        i = DbHelper.InsertUpdateDelete(string.Format(QueryHelper.UpdateReference2, ApplicationId, contact));
                        #endregion
                    }


                    if (i > 0)
                    {
                        flag = true;
                    }
                }
                return Json(flag, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";

                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name)); return null;
            }
        }
        [HttpGet]
        public ActionResult PaymentHistory(int Id, string ActionName)
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.Collector))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            //CommonMethods _commandMethod = new CommonMethods();
            CommonOperation comnoperation = new CommonOperation();
            PaymentInformation listofpayment = new PaymentInformation();
            try
            {
                string applicationNo = Request.QueryString["Id"];
                Session["ApplicationNumber"] = applicationNo;

                listofpayment = comnoperation.GetPaymentDetails(Id);
                if (listofpayment != null)
                {
                    if (listofpayment.paymentinformation.Count > 0)
                    {
                        listofpayment.PaymentHistoryBucketColumnList.Add(listofpayment.paymentinformation.FirstOrDefault().Referenceno);
                        listofpayment.PaymentHistoryBucketColumnList.Add("Due Date");
                        listofpayment.PaymentHistoryBucketColumnList.Add("Ammortization");
                        listofpayment.PaymentHistoryBucketColumnList.Add("Payment Date");
                        listofpayment.PaymentHistoryBucketColumnList.Add("Amount Paid");
                    }
                }
                TempData["ActionName"] = ActionName;
                Session["ActionName"] = ActionName;
                string a = Convert.ToString(System.Web.HttpContext.Current.Request.UrlReferrer);
                listofpayment.applicationno = Id;

            }
            catch (Exception ex)
            {
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
            }
            return View(listofpayment);
        }

        [HttpGet]
        public ActionResult ReduceLoanBucket()
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.Collector))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            //CommonMethods _commandMethod = new CommonMethods();
            CommonOperation comnoperation = new CommonOperation();
            PaymentInformation listofpayment = new PaymentInformation();
            try
            {
                Session["ApplicationNumber"] = Request.QueryString["Id"];
                listofpayment = comnoperation.GetReduceLoan();

            }
            catch (Exception ex)
            {
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
            }
            return View(listofpayment);
        }

        [HttpPost]
        public ActionResult ReduceLoan(int id)
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.Collector))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            Session["ApplicationId"] = id;
            bool result = false;
            try
            {
                int i = DbHelper.InsertUpdateDelete(string.Format(QueryHelper.UpdateIsForReduceLoan, id));
                if (i > 0)
                {
                    result = true;
                }
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                return null;
            }

        }

        [HttpGet]
        public ActionResult DefaultRecordDetails(int? ApplicationId)
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.Collector))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            try
            {
                PaymentInformation model = new PaymentInformation();
                return Json(model, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                return null;
            }

        }
        [HttpPost]
        public ActionResult DefaultRecordDetails()
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.Collector))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            List<UserModel> _lst = new List<UserModel>();
            try
            {
                var webAdminRoleId = ConfigurationManager.AppSettings["DefaulterUserId"];
                string query = string.Format(QueryHelper.GetDefaulterUser, webAdminRoleId);
                var data = DbHelper.SelectMethod(query);
                if (data != null && data.Rows.Count > 0)
                {
                    for (int i = 0; i < data.Rows.Count; i++)
                    {
                        _lst.Add(new UserModel
                        {
                            userid = Convert.ToInt32(data.Rows[i]["userid"]),
                            username = Convert.ToString(data.Rows[i]["username"]),
                            userfullname = Convert.ToString(data.Rows[i]["userfullname"]),
                            webadminrole = Convert.ToInt32(data.Rows[i]["webadminrole"])
                        });
                    }
                }
                return Json(_lst, JsonRequestBehavior.AllowGet);
            }

            catch (Exception ex)
            {
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                return null;
            }
        }

        public ActionResult ReloanFreshBucket()
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.Collector))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            return View();
        }

        public ActionResult AddPTP(int ApplicationId, string total, string penality_amount, string reason, string date, string remarks)
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.Collector))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            ActivityLog.Info($"PTP Added in Application {ApplicationId} on Collector Window by User {Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value)}", $"{this.GetType().Name} ==> {System.Reflection.MethodBase.GetCurrentMethod().Name}");
            string Remark = string.Empty;
            string username = string.Empty;
            double penalityamount = Convert.ToDouble(penality_amount);
            var DataSplit = date.Split('-').ToList();
            var ptpdate = new DateTime(Convert.ToInt32(DataSplit[2]), Convert.ToInt32(DataSplit[1]), Convert.ToInt32(DataSplit[0])).ToString("yyyy-MM-dd");
            var flag = false;
            try
            {
                Remark = RemoveSpecialCharacters(remarks);
                int userid = Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value);
                if (total != null && total.Trim() != "")
                {
                    var data1 = DbHelper.SelectMethod(string.Format(QueryHelper.GetUserName, userid));
                    if (data1 != null)
                    {
                        username = Convert.ToString(data1.Rows[0]["userfullname"]);
                    }
                    var data = DbHelper.SelectMethod(string.Format(QueryHelper.GetPTPRecord, ApplicationId));
                    if (data != null && data.Rows.Count > 0)
                    {
                        int k = DbHelper.InsertUpdateDelete(string.Format(QueryHelper.UpdatePTPDetails, total, reason, penalityamount, userid, ptpdate, remarks, ApplicationId));
                        if (k > 0)
                        {
                            int j = DbHelper.InsertUpdateDelete(string.Format(QueryHelper.InsertRemark, userid, username, ApplicationId, Remark, RemarkIdentifier.PTP));
                            if (j > 0)
                            {
                                flag = true;
                            }
                        }
                    }
                    else
                    {
                        int i = DbHelper.InsertUpdateDelete(string.Format(QueryHelper.InsertPTP, ApplicationId, total, reason, penalityamount, ptpdate, remarks));
                        if (i > 0)
                        {
                            int j = DbHelper.InsertUpdateDelete(string.Format(QueryHelper.InsertRemark, userid, username, ApplicationId, Remark, RemarkIdentifier.PTP));
                            if (j > 0)
                            {
                                flag = true;
                            }

                        }
                    }
                }
                return Json(flag, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                return null;
            }
        }

        public ActionResult AddPartial(int ApplicationId, Double ammortization, Double penality_amount, string reason, string date, string remarks, string paymentchannel, string proofofpayment)
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.Collector))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            ActivityLog.Info($"Partial Added in Application {ApplicationId} on Collector Window by User {Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value)}", $"{this.GetType().Name} ==> {System.Reflection.MethodBase.GetCurrentMethod().Name}");
            PaymentInformation paymentInfo = new PaymentInformation();
            Double AmmortizationAmount = new Double();
            AmmortizationAmount = ammortization;
            string username = string.Empty;
            var DataSplit = date.Split('-').ToList();
            var ptpdate = new DateTime(Convert.ToInt32(DataSplit[2]), Convert.ToInt32(DataSplit[1]), Convert.ToInt32(DataSplit[0])).ToString("yyyy-MM-dd");
            List<PartialModel> EMIBalance = new List<PartialModel>();
            int userid = Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value);
            var GetBalanceEMIAmount = DbHelper.SelectMethod(string.Format(QueryHelper.GetBalanceEMIAmount, ApplicationId));
            var data = DbHelper.SelectMethod(string.Format(QueryHelper.GetUserName, userid));
            if (data != null)
            {
                username = Convert.ToString(data.Rows[0]["userfullname"]);
            }
            var flag = false;
            int i = 0;
            try
            {
                if (ammortization >= 0.0)
                {

                    i = DbHelper.InsertUpdateDelete(string.Format(QueryHelper.InsertPartial, ApplicationId, ammortization, ptpdate, userid, paymentchannel, proofofpayment, penality_amount, remarks));

                }
                if (i > 0)
                {
                    int k = DbHelper.InsertUpdateDelete(string.Format(QueryHelper.UpdatePartial, ApplicationId));
                    int j = DbHelper.InsertUpdateDelete(string.Format(QueryHelper.InsertRemark, userid, username, ApplicationId, remarks, RemarkIdentifier.Partial));
                    if (j > 0)
                    {
                        flag = true;
                    }
                }
                return Json(flag, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name)); return null;
            }
        }

        public ActionResult AddPay(int EmiId, int ApplicationId, string total, string proofofpayment, string paymentchannel, string penalty, bool ischecked, string EmiDetailID, string AmortizationAmount, string PayDate = "")
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.Collector))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            ActivityLog.Info($"Pay Added in Application {ApplicationId} on Collector Window by User {Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value)}", $"{this.GetType().Name} ==> {System.Reflection.MethodBase.GetCurrentMethod().Name}");
            PaymentInformation paymentInfo = new PaymentInformation();
            double TotalAmount = Convert.ToDouble(total);
            double Ammortization = Convert.ToDouble(AmortizationAmount);
            string NewDate = string.Empty;

            string emi_date = string.Empty;
            double totalamount = Convert.ToDouble(total);

            //string username = string.Empty;
            //string Remark = string.Empty;
            string paydate = String.Empty;
            if (!String.IsNullOrWhiteSpace(PayDate))
            {
                var DataSplit = PayDate.Split('-').ToList();
                paydate = new DateTime(Convert.ToInt32(DataSplit[2]), Convert.ToInt32(DataSplit[1]), Convert.ToInt32(DataSplit[0])).ToString("yyyy-MM-dd");
            }
            else
            {
                paydate = DateTime.Now.ToString("yyyy-MM-dd");
            }


            int userid = Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value);
            var flag = false;
            int i = 0;
            try
            {
                string AgentName = string.Empty;

                if (ischecked == true)
                {
                    if (AmortizationAmount != null && AmortizationAmount.Trim() != "")
                    {
                        if (HttpContext.Request.Cookies[CookiesKey.AgentName].Value != null)
                        {
                            AgentName = Convert.ToString(HttpContext.Request.Cookies[CookiesKey.AgentName].Value);
                        }

                        i = DbHelper.InsertUpdateDelete(string.Format(QueryHelper.InsertDeferment, EmiId, 0, proofofpayment, paymentchannel, userid, userid, penalty));

                        if (i > 0)
                        {
                            flag = true;
                        }

                    }
                }
                else
                {
                    if (AmortizationAmount != null && AmortizationAmount.Trim() != "")
                    {
                        if (HttpContext.Request.Cookies[CookiesKey.AgentName].Value != null)
                        {
                            AgentName = Convert.ToString(HttpContext.Request.Cookies[CookiesKey.AgentName].Value);
                        }
                        i = DbHelper.InsertUpdateDelete(string.Format(QueryHelper.InsertPAY, ApplicationId, Ammortization, proofofpayment, paymentchannel, penalty ?? "0", userid, paydate));
                        int m = DbHelper.InsertUpdateDelete(string.Format(QueryHelper.UpdatePay, ApplicationId));
                        if (i > 0 && m > 0)
                        {
                            flag = true;
                        }
                    }
                }
                return Json(flag, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";

                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name)); return null;
            }
        }

        [HttpPost]
        public ActionResult DefaulterUser(int application_no, int defaulteruser_id)
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.Collector))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            try
            {
                int userid = Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value);
                int i = DbHelper.InsertUpdateDelete(string.Format(QueryHelper.InsertDefaulter_user, application_no, defaulteruser_id, userid, userid));
                if (i > 0)
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
                return null;

            }

        }

        [HttpGet]
        public ActionResult ReLoanBucket()
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.Collector))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            ApplicationRecordVM record = new ApplicationRecordVM();
            return View(record);

        }

        [HttpPost]
        public ActionResult GetReloanData()
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.Collector))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            try
            {
                JsonResult result = new JsonResult();
                List<ApplicationRecordVM> ApplicationRecordList = new List<ApplicationRecordVM>();
                var RequestedForm = Request.Form;
                string search = String.Empty;
                int start = 10;
                int length = 10;
                string draw = String.Empty;
                string order = string.Empty;
                string orderDir = string.Empty;
                int UserId = Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value);
                if (RequestedForm.Keys.Count > 0)
                {
                    search = Request.Form.GetValues("search[value]")[0];
                    start = Convert.ToInt32(Request["start"]);
                    length = Convert.ToInt32(Request["length"]);
                    draw = Request.Form.GetValues("draw")[0];
                    order = Request.Form.GetValues("order[0][column]")[0];
                    orderDir = Request.Form.GetValues("order[0][dir]")[0];

                }
                ApplicationRecordList = CommonMethods.GetReLoanData(UserId);
                int totalRecords = ApplicationRecordList.Count;
                if (!string.IsNullOrEmpty(search) &&
                !string.IsNullOrWhiteSpace(search))
                {
                    if (ApplicationRecordList != null && ApplicationRecordList.Count > 0)
                    // Apply search 
                    {
                        ApplicationRecordList = ApplicationRecordList.Where(p => p.ApplicationNo.ToString().ToLower().Contains(search.ToLower()) ||
                        p.DateApplied.ToLower().Contains(search.ToLower())
                        ).ToList();
                    }
                }
                ApplicationRecordList = this.SortByColumnWithOrder1(order, orderDir, ApplicationRecordList);
                int recFilter = ApplicationRecordList.Count;
                ApplicationRecordList = ApplicationRecordList.Skip(start).Take(length).ToList<ApplicationRecordVM>();
                result = this.Json(new { draw = Convert.ToInt32(draw), recordsTotal = totalRecords, recordsFiltered = recFilter, data = ApplicationRecordList }, JsonRequestBehavior.AllowGet);
                return result;
            }
            catch (Exception ex)
            {
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                return null;
            }

        }

        private List<ApplicationRecordVM> SortByColumnWithOrder1(string order, string orderDir, List<ApplicationRecordVM> ApplicationRecordList)
        {
            // Initialization. 
            List<ApplicationRecordVM> lst = new List<ApplicationRecordVM>();
            try
            {
                // Sorting 
                switch (order)
                {
                    case "0":

                        lst = orderDir.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? ApplicationRecordList.OrderByDescending(p => p.DateApplied).ToList() : ApplicationRecordList.OrderBy(p => p.DateApplied).ToList();
                        break;
                    case "1":

                        lst = orderDir.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? ApplicationRecordList.OrderByDescending(p => p.ApplicationNo).ToList() : ApplicationRecordList.OrderBy(p => p.ApplicationNo).ToList();
                        break;
                    case "2":

                        lst = orderDir.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? ApplicationRecordList.OrderByDescending(p => p.First_Name).ToList() : ApplicationRecordList.OrderBy(p => p.First_Name).ToList();
                        break;
                    case "3":

                        lst = orderDir.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? ApplicationRecordList.OrderByDescending(p => p.Middle_Name).ToList() : ApplicationRecordList.OrderBy(p => p.Middle_Name).ToList();
                        break;
                    case "4":

                        lst = orderDir.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? ApplicationRecordList.OrderByDescending(p => p.Last_Name).ToList() : ApplicationRecordList.OrderBy(p => p.Last_Name).ToList();
                        break;
                    case "5":

                        lst = orderDir.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? ApplicationRecordList.OrderByDescending(p => p.PurposeOfLoan).ToList() : ApplicationRecordList.OrderBy(p => p.PurposeOfLoan).ToList();
                        break;
                    case "6":

                        lst = orderDir.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? ApplicationRecordList.OrderByDescending(p => p.Loan_Amount).ToList() : ApplicationRecordList.OrderBy(p => p.Loan_Amount).ToList();
                        break;
                    case "7":

                        lst = orderDir.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? ApplicationRecordList.OrderByDescending(p => p.TermType).ToList() : ApplicationRecordList.OrderBy(p => p.TermType).ToList();
                        break;
                    case "8":

                        lst = orderDir.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? ApplicationRecordList.OrderByDescending(p => p.Remark).ToList() : ApplicationRecordList.OrderBy(p => p.Remark).ToList();
                        break;
                    default:

                        lst = orderDir.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? ApplicationRecordList.OrderByDescending(p => p.ApplicationNo).ToList() : ApplicationRecordList.OrderBy(p => p.ApplicationNo).ToList();
                        break;
                }
                return lst;
            }
            catch (Exception ex)
            {
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                return null;
            }
        }

        [HttpGet]
        public ActionResult PreTermBucket()
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.Collector))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            ApplicationRecordVM record = new ApplicationRecordVM();
            return View(record);
        }
        [HttpPost]
        public ActionResult GetPreTermData()
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.Collector))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            try
            {
                JsonResult result = new JsonResult();
                List<ApplicationRecordVM> ApplicationRecordList = new List<ApplicationRecordVM>();
                var RequestedForm = Request.Form;
                string search = String.Empty;
                int start = 10;
                int length = 10;
                string draw = String.Empty;
                string order = string.Empty;
                string orderDir = string.Empty;
                int UserId = Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value);
                if (RequestedForm.Keys.Count > 0)
                {
                    search = Request.Form.GetValues("search[value]")[0];
                    start = Convert.ToInt32(Request["start"]);
                    length = Convert.ToInt32(Request["length"]);
                    draw = Request.Form.GetValues("draw")[0];
                    order = Request.Form.GetValues("order[0][column]")[0];
                    orderDir = Request.Form.GetValues("order[0][dir]")[0];
                }
                ApplicationRecordList = CommonMethods.GetPreTermData(UserId);
                int totalRecords = ApplicationRecordList.Count;
                if (!string.IsNullOrEmpty(search) &&
                !string.IsNullOrWhiteSpace(search))
                {
                    // Apply search 
                    ApplicationRecordList = ApplicationRecordList.Where(p => p.Name.ToString().ToLower().Contains(search.ToLower()) ||
                    p.ApplicationNo.ToString().ToLower().Contains(search.ToLower()) ||
                    p.Term.ToString().ToLower().Contains(search.ToLower()) ||
                    p.Loan_Amount.ToString().ToLower().Contains(search.ToLower()) ||
                    p.Remark.ToString().ToLower().Contains(search.ToLower())).ToList();
                }
                ApplicationRecordList = this.SortByColumnWithOrder(order, orderDir, ApplicationRecordList);
                int recFilter = ApplicationRecordList.Count;
                ApplicationRecordList = ApplicationRecordList.Skip(start).Take(length).ToList<ApplicationRecordVM>();
                result = this.Json(new { draw = Convert.ToInt32(draw), recordsTotal = totalRecords, recordsFiltered = recFilter, data = ApplicationRecordList }, JsonRequestBehavior.AllowGet);
                return result;
            }
            catch (Exception ex)
            {
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                throw ex;
            }

        }

        private List<ApplicationRecordVM> SortByColumnWithOrder(string order, string orderDir, List<ApplicationRecordVM> ApplicationRecordList)
        {
            // Initialization. 
            List<ApplicationRecordVM> lst = new List<ApplicationRecordVM>();
            try
            {
                // Sorting 
                switch (order)
                {
                    case "0":

                        lst = orderDir.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? ApplicationRecordList.OrderByDescending(p => p.DateApplied).ToList() : ApplicationRecordList.OrderBy(p => p.DateApplied).ToList();
                        break;
                    case "1":

                        lst = orderDir.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? ApplicationRecordList.OrderByDescending(p => p.ApplicationNo).ToList() : ApplicationRecordList.OrderBy(p => p.ApplicationNo).ToList();
                        break;
                    case "2":

                        lst = orderDir.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? ApplicationRecordList.OrderByDescending(p => p.First_Name).ToList() : ApplicationRecordList.OrderBy(p => p.First_Name).ToList();
                        break;
                    case "3":

                        lst = orderDir.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? ApplicationRecordList.OrderByDescending(p => p.Middle_Name).ToList() : ApplicationRecordList.OrderBy(p => p.Middle_Name).ToList();
                        break;
                    case "4":

                        lst = orderDir.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? ApplicationRecordList.OrderByDescending(p => p.Last_Name).ToList() : ApplicationRecordList.OrderBy(p => p.Last_Name).ToList();
                        break;
                    case "5":

                        lst = orderDir.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? ApplicationRecordList.OrderByDescending(p => p.PurposeOfLoan).ToList() : ApplicationRecordList.OrderBy(p => p.PurposeOfLoan).ToList();
                        break;
                    case "6":

                        lst = orderDir.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? ApplicationRecordList.OrderByDescending(p => p.Loan_Amount).ToList() : ApplicationRecordList.OrderBy(p => p.Loan_Amount).ToList();
                        break;
                    case "7":

                        lst = orderDir.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? ApplicationRecordList.OrderByDescending(p => p.TermText).ToList() : ApplicationRecordList.OrderBy(p => p.TermText).ToList();
                        break;
                    case "8":

                        lst = orderDir.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? ApplicationRecordList.OrderByDescending(p => p.Remark).ToList() : ApplicationRecordList.OrderBy(p => p.Remark).ToList();
                        break;
                    default:

                        lst = orderDir.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? ApplicationRecordList.OrderByDescending(p => p.ApplicationNo).ToList() : ApplicationRecordList.OrderBy(p => p.ApplicationNo).ToList();
                        break;
                }
                return lst;
            }
            catch (Exception ex)
            {
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                throw ex;
            }

        }

        /// <summary>
        /// Send SMS
        /// </summary>
        /// <returns></returns>
        public bool SendToSMS(int Id, string mobno)
        {
            try
            {
                if (mobno[0] != '0' && mobno.Length == 10)
                {
                    mobno = '0' + mobno;
                }
                bool flag = logger.ClicktoSMS(mobno, Convert.ToString(ConfigurationManager.AppSettings["SMSContentPreTermBucket"]));
                return flag;
            }
            catch (Exception ex)
            {
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                throw ex;

            }
        }
        public ActionResult SendEmailMessage(int AppId, string PersonalEmail, string PersonalContactNo, string ActionName)
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.Collector))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            bool result = false;
            try
            {
                string EmailBody = string.Empty;
                string EmailSubject = string.Empty;

                if (ActionName == "PreTermBucket")
                {
                    int i = DbHelper.InsertUpdateDelete(string.Format(QueryHelper.UpdateForDisbursementNew, AppId));
                    if (i > 0)
                    {
                        result = true;
                        if (result)
                        {
                            DbHelper.InsertUpdateDelete(string.Format(QueryHelper.UpdateCompleteDate, AppId));
                            EmailSubject = "Preterm Application";
                            EmailBody = string.Format("Dear customer,<br/>Your preterm application has been put under processing!<br/><br/>Thank You<br/>Cashmart.");
                            new Thread(() => CommonMethods.SendMail(EmailFrom.FromAccount, PersonalEmail, EmailBody, EmailSubject)).Start();
                        }
                    }
                }
                if (ActionName == "ReLoanBucket")
                {
                    int i = DbHelper.InsertUpdateDelete(string.Format(QueryHelper.UpdateForDisbursementNew, AppId));
                    if (i > 0)
                    {
                        result = true;
                        if (result)
                        {
                            EmailSubject = "Reloan Application";
                            EmailBody = string.Format("Dear customer,<br/>Your reloan application has been put under processing!<br/><br/>Thank You<br/>Cashmart.");
                            new Thread(() => CommonMethods.SendMail(EmailFrom.FromAccount, PersonalEmail, EmailBody, EmailSubject)).Start();
                        }
                    }
                }

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                return null;
            }
        }

        /// <summary>
        /// Reloan SMS 
        /// </summary>
        /// <returns></returns>
        public ActionResult ClickForSMSReloan(int AppId, string mobno)
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.Collector))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            try
            {
                if (mobno[0] != '0' && mobno.Length == 10)
                {
                    mobno = '0' + mobno;
                }
                SendToSMS(AppId, mobno);
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                return null;
            }
        }

        public ActionResult ClickForSMSPreTerm(int AppId, string mobno)
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.Collector))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            try
            {
                if (mobno[0] != '0' && mobno.Length == 10)
                {
                    mobno = '0' + mobno;
                }
                SendToSMS(AppId, mobno);
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                return null;
            }
        }

        public int ClickForCall(string ContactNo)
        {
            try
            {
                return logger.ClicktoCall(ContactNo, Convert.ToString(HttpContext.Request.Cookies[CookiesKey.AgentName].Value), Convert.ToString(Session["IPAddress"]));
            }
            catch (Exception ex)
            {
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                throw ex;
            }

        }

        public int ClickForCallReloan(string ContactNo)
        {
            try
            {
                return logger.ClicktoCall(ContactNo, Convert.ToString(HttpContext.Request.Cookies[CookiesKey.AgentName].Value), Convert.ToString(Session["IPAddress"]));
            }
            catch (Exception ex)
            {
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                throw ex;
            }

        }

        public ActionResult CancelPreTerm(int ApplicationNo)
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.Collector))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            ActivityLog.Info($"Preterm Cancelled for Application {ApplicationNo} on Collector Window by User {Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value)}", $"{this.GetType().Name} ==> {System.Reflection.MethodBase.GetCurrentMethod().Name}");
            var flag = false;
            try
            {
                var query1 = DbHelper.SelectMethod(string.Format(QueryHelper.GetApplicationNoOldest, ApplicationNo));
                if (query1 != null && query1.Rows.Count > 0)
                {
                    int ApplicationNoOld = Convert.ToInt32(query1.Rows[0]["applicationno"]);

                    int k = DbHelper.InsertUpdateDelete(string.Format(QueryHelper.CancelIsPretermGenerated, ApplicationNoOld));
                    if (k > 0)
                    {
                        int i = DbHelper.InsertUpdateDelete(string.Format(QueryHelper.CancelPreTerms, ApplicationNo));
                        if (i > 0)
                        {
                            flag = true;
                            DbHelper.InsertUpdateDelete(String.Format(QueryHelper.DeletePretermReloanRecord, ApplicationNo));
                        }
                    }
                }
                return Json(flag, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                return null;
            }
        }

        public ActionResult CancelReloan(int ApplicationNo)
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.Collector))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            var flag = false;
            try
            {
                ActivityLog.Info($"Reloan Cancelled for Application {ApplicationNo} on Collector Window by User {Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value)}", $"{this.GetType().Name} ==> {System.Reflection.MethodBase.GetCurrentMethod().Name}");
                var query1 = DbHelper.SelectMethod(string.Format(QueryHelper.GetApplicationNoOldest, ApplicationNo));
                if (query1 != null && query1.Rows.Count > 0)
                {
                    int ApplicationNoOld;
                    ApplicationNoOld = Convert.ToInt32(query1.Rows[0]["applicationno"]);

                    int k = DbHelper.InsertUpdateDelete(string.Format(QueryHelper.UpdateisforFSBucket, ApplicationNoOld));
                    if (k > 0)
                    {
                        flag = true;
                        int i = DbHelper.InsertUpdateDelete(string.Format(QueryHelper.CancelPreTerms, ApplicationNo));
                        if (i > 0)
                        {
                            flag = true;
                            DbHelper.InsertUpdateDelete(String.Format(QueryHelper.DeletePretermReloanRecord, ApplicationNo));
                        }
                    }

                }
                return Json(flag, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                return null;
            }

        }

        /// <summary>
        /// FS bucket for collection window
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult FSBucket()
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.Collector))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            try
            {
                SMSTemplateVM model = new SMSTemplateVM();
                EmailTemplateModelVM email = new EmailTemplateModelVM();
                DataTable dtsms = DbHelper.SelectMethod(QueryHelper.GetActivesmsTemplateList);
                if (dtsms != null && dtsms.Rows.Count > 0)
                {
                    for (int i = 0; i < dtsms.Rows.Count; i++)
                    {
                        model.SMSTemplateList.Add(new SMSTemplateVM
                        {
                            id = Convert.ToInt32(dtsms.Rows[i]["id"]),
                            sms_template_name = Convert.ToString(dtsms.Rows[i]["sms_template_name"]),

                        });
                    }
                }
                DataTable dtemail = DbHelper.SelectMethod(QueryHelper.GetActiveEmailTemplateList);
                if (dtemail != null && dtemail.Rows.Count > 0)
                {
                    for (int i = 0; i < dtemail.Rows.Count; i++)
                    {
                        email.EMailTemplateList.Add(new EmailTemplateModel
                        {
                            id = Convert.ToInt32(dtemail.Rows[i]["id"]),
                            email_template_name = Convert.ToString(dtemail.Rows[i]["email_template_name"]),

                        });
                    }
                }
                model.SMSTemplateList.Insert(0, new SMSTemplateVM { id = 0, sms_template_name = "--Select--" });
                email.EMailTemplateList.Insert(0, new EmailTemplateModel { id = 0, email_template_name = "--Select--" });
                if (email.EMailTemplateList != null && email.EMailTemplateList.Count > 0)
                {
                    ViewBag.EmailTemplate = email.EMailTemplateList;
                }
                else
                {
                    ViewBag.EmailTemplate = new List<EmailTemplateModel>();
                }
                if (model.SMSTemplateList != null && model.SMSTemplateList.Count > 0)
                {
                    ViewBag.SMSTemplate = model.SMSTemplateList;
                }
                else
                {
                    ViewBag.SMSTemplate = new List<SMSTemplate>();
                }

            }
            catch (Exception ex)
            {
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";

                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
            }
            return View();
        }

        public ActionResult GetFSBucketData()
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.Collector))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            try
            {
                CommonOperation comnoperation1 = new CommonOperation();
                int _userId = 0;
                if (HttpContext.Request.Cookies[CookiesKey.UserId].Value != null)
                {
                    _userId = Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value);
                }

                var users = comnoperation1.GetFSBucketData(_userId);
                return Json(users, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                return null;
            }
        }

        public ActionResult RemoveApplication(int AppNo)
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.Collector))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            bool flag = false;
            try
            {
                ActivityLog.Info($"Remove Application {AppNo} from FS Bucket on Collector Window by User {Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value)}", $"{this.GetType().Name} ==> {System.Reflection.MethodBase.GetCurrentMethod().Name}");
                int i = DbHelper.InsertUpdateDelete(string.Format(QueryHelper.UpdateIsDeleted, AppNo));
                if (i > 0)
                {
                    flag = true;
                }
                return Json(flag, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                return null;
            }
        }

        public ActionResult ReloanApplication(int AppNo)
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.Collector))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            bool flag = false;
            try
            {
                ActivityLog.Info($"Reloan Application {AppNo} on Collector Window by User {Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value)}", $"{this.GetType().Name} ==> {System.Reflection.MethodBase.GetCurrentMethod().Name}");
                int i = DbHelper.InsertUpdateDelete(string.Format(QueryHelper.UpdateIsReloan, AppNo));
                if (i > 0)
                {
                    flag = true;
                }
                return Json(flag, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                return null;
            }
        }

        public ActionResult GetSMSValues(int Id)
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.Collector))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            try
            {
                SMSTemplate sms = new SMSTemplate();
                var data = DbHelper.SelectMethod(string.Format(QueryHelper.GetActiveSMS_ById, Id));
                if (data != null && data.Rows.Count > 0)
                {
                    sms.sms_template_definition = Convert.ToString(data.Rows[0]["sms_template_definition"]);
                    sms.sms_template_description = Convert.ToString(data.Rows[0]["sms_description"]);
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

        public ActionResult GetEmailValues(int Id)
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.Collector))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            try
            {
                EmailTemplateModel email = new EmailTemplateModel();
                var data = DbHelper.SelectMethod(string.Format(QueryHelper.GetActiveEmail_ById, Id));
                if (data != null && data.Rows.Count > 0)
                {
                    email.email_template_definition = Convert.ToString(data.Rows[0]["email_template_definition"]);
                    email.email_template_description = Convert.ToString(data.Rows[0]["email_description"]);
                }
                return Json(email, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));

                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                return null;
            }

        }

        public ActionResult BroadcastSMS(string SMSBody)
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.Collector))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            bool flag = false;
            try
            {
                CommonOperation comnoperation1 = new CommonOperation();
                int _userId = 0;
                if (HttpContext.Request.Cookies[CookiesKey.UserId].Value != null)
                {
                    _userId = Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value);
                }

                var users = comnoperation1.GetFSBucketData(_userId);
                if (users != null && users.Count > 0)
                {
                    foreach (var item in users)
                    {
                        new System.Threading.Thread(() => logger.ClicktoSMS(item.PersonalContactNumber, SMSBody)).Start();
                    }
                    flag = true;
                }
                return Json(flag, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                return null;
            }
        }

        public ActionResult BroadcastEmail(string EmailBody)
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.Collector))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            bool flag = false;
            try
            {
                CommonOperation comnoperation1 = new CommonOperation();
                int _userId = 0;
                if (HttpContext.Request.Cookies[CookiesKey.UserId].Value != null)
                {
                    _userId = Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value);
                }

                var users = comnoperation1.GetFSBucketData(_userId);
                if (users != null && users.Count > 0)
                {
                    foreach (var item in users)
                    {
                        new Thread(() => CommonMethods.SendMail(EmailFrom.FromInfo, item.personal_email, EmailBody, "CashMart Contact")).Start();
                    }
                    flag = true;
                }
                return Json(flag, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                return null;
            }
        }

        [HttpGet]
        public ActionResult SalesTab()
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.Collector))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            AgingDataVM model = new AgingDataVM();
            try
            {

                model.ReportName = "Daily";
                model.AgingDataList = GetSalesData();

            }
            catch (Exception ex)
            {
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";

            }
            return View(model);
        }
        [HttpPost]
        public ActionResult SalesTab(AgingDataVM model, string CommandName)
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.Collector))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            try
            {
                if (CommandName == "Week")
                {
                    model.ReportName = "Weekly";
                    model.AgingDataList = GetSalesData_Weekly();
                }
                else if (CommandName == "Month")
                {
                    model.ReportName = "Monthly";
                    model.AgingDataList = GetSalesData_Monthly();
                }
                else if (CommandName == "Year")
                {
                    model.ReportName = "Yearly";
                    model.AgingDataList = GetSalesData_Yearly();
                }
                else if (CommandName == "Daily")
                {
                    model.ReportName = "Daily";
                    model.AgingDataList = GetSalesData();
                }
            }
            catch (Exception ex)
            {
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";

            }
            return View(model);
        }

        public List<AgingData> GetSalesData()
        {
            List<AgingData> model = new List<AgingData>();
            try
            {
                DayOfWeek day = DateTime.Now.DayOfWeek;
                int days = day - DayOfWeek.Monday;

                string start = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1, 0, 0, 0).ToString("yyyy-MM-dd");
                DateTime enddate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1, 0, 0, 0).AddMonths(1).AddDays(-1);
                int userid = Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value);

                for (int j = 0; j < enddate.Day; j++)
                {
                    var query = DbHelper.SelectMethod(string.Format(QueryHelper.GetSalesData, start, start, userid));
                    if (query.Rows.Count > 0)
                    {
                        for (int i = 0; i < query.Rows.Count; i++)
                        {
                            model.Add(new AgingData
                            {
                                DueDate = Convert.ToDateTime(start).ToString("dd-MM-yyyy"),
                                TotalMaturities = Convert.ToInt64(query.Rows[i]["TotalMaturity"]),
                                TotalMaturities_Peso = Convert.ToString(query.Rows[i]["TotalMaturityAmount"]) != "" ? Math.Round(Convert.ToDecimal(query.Rows[i]["TotalMaturityAmount"]), 2) : 0,
                                TotalPaid = Convert.ToInt64(query.Rows[i]["totalpaid"]),
                                TotalPaid_Peso = Convert.ToString(query.Rows[i]["totalpaidamount"]) != "" ? Math.Round(Convert.ToDecimal(query.Rows[i]["totalpaidamount"]), 2) : 0,
                                CollectionEfficiency = Convert.ToInt64(query.Rows[i]["efficiency_no"]),
                                CollectionEfficiency_Peso = Convert.ToString(query.Rows[i]["efficiency_amount"]) != "" ? Math.Round(Convert.ToDecimal(query.Rows[i]["efficiency_amount"]), 2) : 0,
                                Reloan = Convert.ToInt64(query.Rows[i]["Reloan_No"]),
                                Reloan_Peso = Convert.ToString(query.Rows[i]["Reloan_Amount"]) != "" ? Math.Round(Convert.ToDecimal(query.Rows[i]["Reloan_Amount"]), 2) : 0,
                                Preterm = Convert.ToInt64(query.Rows[i]["Preterm_No"]),
                                Preterm_Peso = Convert.ToString(query.Rows[i]["Preterm_Amount"]) != "" ? Math.Round(Convert.ToDecimal(query.Rows[i]["Preterm_Amount"]), 2) : 0
                            });
                        }
                    }
                    start = Convert.ToDateTime(start).AddDays(+1).ToString("yyyy-MM-dd");
                }
                if (model != null && model.Count > 0)
                {
                    model.FirstOrDefault().SumMaturity_No = Math.Round(model.Sum(x => x.TotalMaturities), 2);
                    model.FirstOrDefault().SumMaturity_Peso = Math.Round(model.Sum(x => x.TotalMaturities_Peso), 2);
                    model.FirstOrDefault().SumPaid_No = Math.Round(model.Sum(x => x.TotalPaid), 2);
                    model.FirstOrDefault().SumPaid_Peso = Math.Round(model.Sum(x => x.TotalPaid_Peso), 2);
                    model.FirstOrDefault().SumEffeciency_No = Math.Round(Convert.ToDecimal(model.FirstOrDefault().SumPaid_No) * 100 / Convert.ToDecimal(model.FirstOrDefault().SumMaturity_No == 0 ? 1 : model.FirstOrDefault().SumMaturity_No), 2);
                    model.FirstOrDefault().SumEffeciency_Peso = Math.Round(Convert.ToDecimal(model.FirstOrDefault().SumPaid_Peso) * 100 / Convert.ToDecimal(model.FirstOrDefault().SumMaturity_Peso == 0 ? 1 : model.FirstOrDefault().SumMaturity_Peso), 2);
                    model.FirstOrDefault().SumPreterm_No = Math.Round(model.Sum(x => x.Preterm), 2);
                    model.FirstOrDefault().SumPreterm_Peso = Math.Round(model.Sum(x => x.Preterm_Peso), 2);
                    model.FirstOrDefault().SumReloan_No = Math.Round(model.Sum(x => x.Reloan), 2);
                    model.FirstOrDefault().SumReloan_Peso = Math.Round(model.Sum(x => x.Reloan_Peso), 2);
                }
                return model;
            }
            catch (Exception msg)
            {
                logger.WriteErrorLogs(msg, String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                throw msg;
            }
        }

        public List<AgingData> GetSalesData_Weekly()
        {
            List<AgingData> model = new List<AgingData>();
            int userid = Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value);
            try
            {


                int year = DateTime.Now.Year, month = DateTime.Now.Month;
                var calendar = CultureInfo.CurrentCulture.Calendar;
                var firstDayOfWeek = CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek;
                var weekPeriods =
                Enumerable.Range(1, calendar.GetDaysInMonth(year, month))
                          .Select(d =>
                          {
                              var date = new DateTime(year, month, d);
                              var weekNumInYear = calendar.GetWeekOfYear(date, CalendarWeekRule.FirstDay, firstDayOfWeek);
                              return new { date, weekNumInYear };
                          })
                          .GroupBy(x => x.weekNumInYear)
                          .Select(x => new { DateFrom = x.First().date, To = x.Last().date })
                          .ToList();
                int WeekCount = 1;

                foreach (var item in weekPeriods)
                {
                    var query = DbHelper.SelectMethod(string.Format(QueryHelper.GetSalesData, item.DateFrom.ToString("yyyy-MM-dd"), item.To.ToString("yyyy-MM-dd"), userid));
                    if (query.Rows.Count > 0)
                    {
                        for (int i = 0; i < query.Rows.Count; i++)
                        {
                            model.Add(new AgingData
                            {
                                DueDate = $"Week {WeekCount}",
                                DateRange = $"({item.DateFrom:yyyy-MM-dd} to {item.To:yyyy-MM-dd})",
                                TotalMaturities = Convert.ToInt64(query.Rows[i]["TotalMaturity"]),
                                TotalMaturities_Peso = Convert.ToString(query.Rows[i]["TotalMaturityAmount"]) != "" ? Math.Round(Convert.ToDecimal(query.Rows[i]["TotalMaturityAmount"]), 2) : 0,
                                TotalPaid = Convert.ToInt64(query.Rows[i]["totalpaid"]),
                                TotalPaid_Peso = Convert.ToString(query.Rows[i]["totalpaidamount"]) != "" ? Math.Round(Convert.ToDecimal(query.Rows[i]["totalpaidamount"]), 2) : 0,
                                CollectionEfficiency = Convert.ToInt64(query.Rows[i]["efficiency_no"]),
                                CollectionEfficiency_Peso = Convert.ToString(query.Rows[i]["efficiency_amount"]) != "" ? Math.Round(Convert.ToDecimal(query.Rows[i]["efficiency_amount"]), 2) : 0,
                                Reloan = Convert.ToInt64(query.Rows[i]["Reloan_No"]),
                                Reloan_Peso = Convert.ToString(query.Rows[i]["Reloan_Amount"]) != "" ? Math.Round(Convert.ToDecimal(query.Rows[i]["Reloan_Amount"]), 2) : 0,
                                Preterm = Convert.ToInt64(query.Rows[i]["Preterm_No"]),
                                Preterm_Peso = Convert.ToString(query.Rows[i]["Preterm_Amount"]) != "" ? Math.Round(Convert.ToDecimal(query.Rows[i]["Preterm_Amount"]), 2) : 0
                            });
                        }
                        WeekCount += 1;
                    }
                }
                if (model != null && model.Count > 0)
                {
                    model.FirstOrDefault().SumMaturity_No = Math.Round(model.Sum(x => x.TotalMaturities), 2);
                    model.FirstOrDefault().SumMaturity_Peso = Math.Round(model.Sum(x => x.TotalMaturities_Peso), 2);
                    model.FirstOrDefault().SumPaid_No = Math.Round(model.Sum(x => x.TotalPaid), 2);
                    model.FirstOrDefault().SumPaid_Peso = Math.Round(model.Sum(x => x.TotalPaid_Peso), 2);
                    model.FirstOrDefault().SumEffeciency_No = Math.Round(Convert.ToDecimal(model.FirstOrDefault().SumPaid_No) * 100 / Convert.ToDecimal(model.FirstOrDefault().SumMaturity_No == 0 ? 1 : model.FirstOrDefault().SumMaturity_No), 2);
                    model.FirstOrDefault().SumEffeciency_Peso = Math.Round(Convert.ToDecimal(model.FirstOrDefault().SumPaid_Peso) * 100 / Convert.ToDecimal(model.FirstOrDefault().SumMaturity_Peso == 0 ? 1 : model.FirstOrDefault().SumMaturity_Peso), 2);
                    model.FirstOrDefault().SumPreterm_No = Math.Round(model.Sum(x => x.Preterm), 2);
                    model.FirstOrDefault().SumPreterm_Peso = Math.Round(model.Sum(x => x.Preterm_Peso), 2);
                    model.FirstOrDefault().SumReloan_No = Math.Round(model.Sum(x => x.Reloan), 2);
                    model.FirstOrDefault().SumReloan_Peso = Math.Round(model.Sum(x => x.Reloan_Peso), 2);
                }
                return model;
            }
            catch (Exception msg)
            {
                logger.WriteErrorLogs(msg, String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                throw msg;
            }
        }

        public List<AgingData> GetSalesData_Monthly()
        {
            List<AgingData> model = new List<AgingData>();
            DateTime date = DateTime.Now;
            List<DateTime> StartDateList = new List<DateTime>();
            List<DateTime> EndDateList = new List<DateTime>();
            int userid = Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value);
            for (int i = 1; i <= 12; i++)
            {
                var firstDayOfMonth = new DateTime(date.Year, i, 1);
                var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);
                StartDateList.Add(firstDayOfMonth);
                EndDateList.Add(lastDayOfMonth);
            }
            try
            {
                for (int m = 0; m < StartDateList.Count; m++)
                {
                    var query = DbHelper.SelectMethod(string.Format(QueryHelper.GetSalesData, StartDateList[m].ToString("yyyy-MM-dd"), EndDateList[m].ToString("yyyy-MM-dd"), userid));
                    if (query != null && query.Rows.Count > 0)
                    {
                        for (int i = 0; i < query.Rows.Count; i++)
                        {
                            model.Add(new AgingData
                            {
                                DueDate = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(m + 1),
                                TotalMaturities = Convert.ToInt64(query.Rows[i]["TotalMaturity"]),
                                TotalMaturities_Peso = Convert.ToString(query.Rows[i]["TotalMaturityAmount"]) != "" ? Math.Round(Convert.ToDecimal(query.Rows[i]["TotalMaturityAmount"]), 2) : 0,
                                TotalPaid = Convert.ToInt64(query.Rows[i]["totalpaid"]),
                                TotalPaid_Peso = Convert.ToString(query.Rows[i]["totalpaidamount"]) != "" ? Math.Round(Convert.ToDecimal(query.Rows[i]["totalpaidamount"]), 2) : 0,
                                CollectionEfficiency = Convert.ToInt64(query.Rows[i]["efficiency_no"]),
                                CollectionEfficiency_Peso = Convert.ToString(query.Rows[i]["efficiency_amount"]) != "" ? Math.Round(Convert.ToDecimal(query.Rows[i]["efficiency_amount"]), 2) : 0,
                                Reloan = Convert.ToInt64(query.Rows[i]["Reloan_No"]),
                                Reloan_Peso = Convert.ToString(query.Rows[i]["Reloan_Amount"]) != "" ? Math.Round(Convert.ToDecimal(query.Rows[i]["Reloan_Amount"]), 2) : 0,
                                Preterm = Convert.ToInt64(query.Rows[i]["Preterm_No"]),
                                Preterm_Peso = Convert.ToString(query.Rows[i]["Preterm_Amount"]) != "" ? Math.Round(Convert.ToDecimal(query.Rows[i]["Preterm_Amount"]), 2) : 0
                            });
                        }
                    }
                }
                if (model != null && model.Count > 0)
                {
                    model.FirstOrDefault().SumMaturity_No = Math.Round(model.Sum(x => x.TotalMaturities), 2);
                    model.FirstOrDefault().SumMaturity_Peso = Math.Round(model.Sum(x => x.TotalMaturities_Peso), 2);
                    model.FirstOrDefault().SumPaid_No = Math.Round(model.Sum(x => x.TotalPaid), 2);
                    model.FirstOrDefault().SumPaid_Peso = Math.Round(model.Sum(x => x.TotalPaid_Peso), 2);
                    model.FirstOrDefault().SumEffeciency_No = Math.Round(Convert.ToDecimal(model.FirstOrDefault().SumPaid_No) * 100 / Convert.ToDecimal(model.FirstOrDefault().SumMaturity_No == 0 ? 1 : model.FirstOrDefault().SumMaturity_No), 2);
                    model.FirstOrDefault().SumEffeciency_Peso = Math.Round(Convert.ToDecimal(model.FirstOrDefault().SumPaid_Peso) * 100 / Convert.ToDecimal(model.FirstOrDefault().SumMaturity_Peso == 0 ? 1 : model.FirstOrDefault().SumMaturity_Peso), 2);
                    model.FirstOrDefault().SumPreterm_No = Math.Round(model.Sum(x => x.Preterm), 2);
                    model.FirstOrDefault().SumPreterm_Peso = Math.Round(model.Sum(x => x.Preterm_Peso), 2);
                    model.FirstOrDefault().SumReloan_No = Math.Round(model.Sum(x => x.Reloan), 2);
                    model.FirstOrDefault().SumReloan_Peso = Math.Round(model.Sum(x => x.Reloan_Peso), 2);
                }
                return model;
            }
            catch (Exception msg)
            {

                logger.WriteErrorLogs(msg, String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                throw msg;
            }
        }

        public List<AgingData> GetSalesData_Yearly()
        {
            try
            {
                List<AgingData> model = new List<AgingData>();
                List<DateTime> StartDateList = new List<DateTime>();
                List<DateTime> EndDateList = new List<DateTime>();
                int userid = Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value);

                var query = DbHelper.SelectMethod(string.Format(QueryHelper.GetOldestApplication));
                if ((query != null && query.Rows.Count > 0))
                {
                    DateTime age = Convert.ToDateTime(query.Rows[0]["createdon"]);
                    string year = age.Year.ToString();
                    int _year = Convert.ToInt32(year);
                    string currentYear = DateTime.Now.Year.ToString();
                    int _year1 = Convert.ToInt32(currentYear);
                    int num = _year1 - _year;
                    for (int j = _year; j <= _year1; j++)
                    {
                        string firstDay = new DateTime(j, 1, 1).ToString("yyyy-MM-dd");
                        string lastDay = new DateTime(j, 12, 31).ToString("yyyy-MM-dd");
                        var query1 = DbHelper.SelectMethod(string.Format(QueryHelper.GetSalesData, firstDay, lastDay, userid));
                        if (query1 != null && query1.Rows.Count > 0)
                        {
                            for (int i = 0; i < query1.Rows.Count; i++)
                            {
                                model.Add(new AgingData
                                {
                                    DueDate = Convert.ToString(j),
                                    TotalMaturities = Convert.ToInt64(query1.Rows[i]["TotalMaturity"]),
                                    TotalMaturities_Peso = Convert.ToString(query1.Rows[i]["TotalMaturityAmount"]) != "" ? Math.Round(Convert.ToDecimal(query1.Rows[i]["TotalMaturityAmount"]), 2) : 0,
                                    TotalPaid = Convert.ToInt64(query1.Rows[i]["totalpaid"]),
                                    TotalPaid_Peso = Convert.ToString(query1.Rows[i]["totalpaidamount"]) != "" ? Math.Round(Convert.ToDecimal(query1.Rows[i]["totalpaidamount"]), 2) : 0,
                                    CollectionEfficiency = Convert.ToInt64(query1.Rows[i]["efficiency_no"]),
                                    CollectionEfficiency_Peso = Convert.ToString(query1.Rows[i]["efficiency_amount"]) != "" ? Math.Round(Convert.ToDecimal(query1.Rows[i]["efficiency_amount"]), 2) : 0,
                                    Reloan = Convert.ToInt64(query1.Rows[i]["Reloan_No"]),
                                    Reloan_Peso = Convert.ToString(query1.Rows[i]["Reloan_Amount"]) != "" ? Math.Round(Convert.ToDecimal(query1.Rows[i]["Reloan_Amount"]), 2) : 0,
                                    Preterm = Convert.ToInt64(query1.Rows[i]["Preterm_No"]),
                                    Preterm_Peso = Convert.ToString(query1.Rows[i]["Preterm_Amount"]) != "" ? Math.Round(Convert.ToDecimal(query1.Rows[i]["Preterm_Amount"]), 2) : 0
                                });
                            }
                        }
                    }
                    if (model != null && model.Count > 0)
                    {
                        model.FirstOrDefault().SumMaturity_No = Math.Round(model.Sum(x => x.TotalMaturities), 2);
                        model.FirstOrDefault().SumMaturity_Peso = Math.Round(model.Sum(x => x.TotalMaturities_Peso), 2);
                        model.FirstOrDefault().SumPaid_No = Math.Round(model.Sum(x => x.TotalPaid), 2);
                        model.FirstOrDefault().SumPaid_Peso = Math.Round(model.Sum(x => x.TotalPaid_Peso), 2);
                        model.FirstOrDefault().SumEffeciency_No = Math.Round(Convert.ToDecimal(model.FirstOrDefault().SumPaid_No) * 100 / Convert.ToDecimal(model.FirstOrDefault().SumMaturity_No == 0 ? 1 : model.FirstOrDefault().SumMaturity_No), 2);
                        model.FirstOrDefault().SumEffeciency_Peso = Math.Round(Convert.ToDecimal(model.FirstOrDefault().SumPaid_Peso) * 100 / Convert.ToDecimal(model.FirstOrDefault().SumMaturity_Peso == 0 ? 1 : model.FirstOrDefault().SumMaturity_Peso), 2);
                        model.FirstOrDefault().SumPreterm_No = Math.Round(model.Sum(x => x.Preterm), 2);
                        model.FirstOrDefault().SumPreterm_Peso = Math.Round(model.Sum(x => x.Preterm_Peso), 2);
                        model.FirstOrDefault().SumReloan_No = Math.Round(model.Sum(x => x.Reloan), 2);
                        model.FirstOrDefault().SumReloan_Peso = Math.Round(model.Sum(x => x.Reloan_Peso), 2);
                    }
                }

                return model;
            }
            catch (Exception ex)
            {
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                throw ex;
            }

        }

        [HttpPost]
        public ActionResult Disqualify(int id)
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.Collector))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            ActivityLog.Info($"Application {id} Disqualify on Collector Window by User {Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value)}", $"{this.GetType().Name} ==> {System.Reflection.MethodBase.GetCurrentMethod().Name}");
            Session["ApplicationId"] = id;
            int barangay_id = 0;
            string sss_no = string.Empty;
            int city = 0;
            bool flag = false;
            try
            {
                var disqualify = DbHelper.SelectMethod(string.Format(QueryHelper.disqualify, id));
                if (disqualify != null && disqualify.Rows.Count > 0)
                {

                    barangay_id = Convert.ToString(disqualify.Rows[0]["barangay"]) != string.Empty ? Convert.ToInt32(disqualify.Rows[0]["barangay"]) : 0;
                    city = Convert.ToString(disqualify.Rows[0]["city"]) != string.Empty ? Convert.ToInt32(disqualify.Rows[0]["city"]) : 0;
                    sss_no = Convert.ToString(disqualify.Rows[0]["sss_no"]);
                }

                int i = DbHelper.InsertUpdateDelete(string.Format(QueryHelper.UpdateDisqualify, id));
                if (i > 0)
                {
                    int l = DbHelper.InsertUpdateDelete(string.Format(QueryHelper.UpdateSSSOrr, sss_no));
                    if (l > 0)
                    {
                        flag = true;
                    }
                    else
                    {
                        int insert = DbHelper.InsertUpdateDelete(string.Format(QueryHelper.InsertSSSNo, sss_no, true));
                        if (insert > 0)
                        {
                            flag = true;
                        }
                    }
                }
                return Json(flag, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                return null;
            }
        }

        public ActionResult DefaulterPay(int ApplicationId, string total, string proofofpayment, string paymentchannel, string penalty, string Amortization, String PayDate = "")
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.Collector))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            ActivityLog.Info($"Defaulter Pay for Application {ApplicationId} on Collector Window by User {Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value)}", $"{this.GetType().Name} ==> {System.Reflection.MethodBase.GetCurrentMethod().Name}");
            bool flag = false;
            PaymentInformation paymentInfo = new PaymentInformation();
            double TotalAmount = Convert.ToDouble(total);
            double AmortizationAmount = Convert.ToDouble(Amortization);
            int EmiId = 0;
            string NewDate = string.Empty;
            string emi_date = string.Empty;

            string AgentName = string.Empty;

            string Remark = string.Empty;

            int userid = Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value);
            var DataSplit = PayDate.Split('-').ToList();
            var ptpdate = new DateTime(Convert.ToInt32(DataSplit[2]), Convert.ToInt32(DataSplit[1]), Convert.ToInt32(DataSplit[0])).ToString("yyyy-MM-dd");

            int i = 0;
            try
            {
                var Emi = DbHelper.SelectMethod(string.Format(QueryHelper.GetEMIId, ApplicationId));
                if (Emi != null && Emi.Rows.Count > 0)
                {
                    EmiId = Convert.ToInt32(Emi.Rows[0]["emi_id"]);
                }

                if (Amortization != null && Amortization.Trim() != "")
                {
                    if (HttpContext.Request.Cookies[CookiesKey.AgentName].Value != null)
                    {
                        AgentName = Convert.ToString(HttpContext.Request.Cookies[CookiesKey.AgentName].Value);
                    }

                    i = DbHelper.InsertUpdateDelete(string.Format(QueryHelper.InsertPAY, ApplicationId, Amortization, proofofpayment, paymentchannel, penalty ?? "0", userid, ptpdate));

                    int m = DbHelper.InsertUpdateDelete(string.Format(QueryHelper.UpdatePay, ApplicationId));

                    if (i > 0 && m > 0)
                    {
                        flag = true;
                    }
                }

                return Json(flag, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                return null;
            }
        }

        public ActionResult DefaulterPTP(int ApplicationId, string total, string penality_amount, string date, string Amortization)
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.Collector))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            bool flag = false;
            ActivityLog.Info($"Application {ApplicationId} Defaulter PTP on Collector Window by User {Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value)}", $"{this.GetType().Name} ==> {System.Reflection.MethodBase.GetCurrentMethod().Name}");
            double penalityamount = Convert.ToDouble(penality_amount);
            var DataSplit = date.Split('-').ToList();
            var ptpdate = new DateTime(Convert.ToInt32(DataSplit[2]), Convert.ToInt32(DataSplit[1]), Convert.ToInt32(DataSplit[0])).ToString("yyyy-MM-dd");
            int i = 0;
            try
            {
                if (total != null && total.Trim() != "")
                {

                    i = DbHelper.InsertUpdateDelete(string.Format(QueryHelper.InsertDefaulterPTP, ApplicationId, total, penalityamount, ptpdate));

                }
                if (i > 0)
                {

                    flag = true;


                }



                return Json(flag, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                return null;
            }
        }

        public ActionResult DefaulterPartial(int ApplicationId, Double total, Double penality_amount, string date, string paymentchannel, string proofofpayment, Double Amortization, String PayDate = "")
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.Collector))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            ActivityLog.Info($"Application {ApplicationId} Defaulter Partial on Collector Window by User {Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value)}", $"{this.GetType().Name} ==> {System.Reflection.MethodBase.GetCurrentMethod().Name}");
            PaymentInformation paymentInfo = new PaymentInformation();
            Double balance_amount = new Double();
            balance_amount = total;
            string username = string.Empty;
            Double AmmortizationAmount = new Double();
            AmmortizationAmount = Amortization;
            List<PartialModel> EMIBalance = new List<PartialModel>();
            int userid = Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value);
            var GetBalanceEMIAmount = DbHelper.SelectMethod(string.Format(QueryHelper.GetBalanceEMIAmount, ApplicationId));
            var data = DbHelper.SelectMethod(string.Format(QueryHelper.GetUserName, userid));
            if (data != null)
            {
                username = Convert.ToString(data.Rows[0]["userfullname"]);
            }
            var DataSplit = date.Split('-').ToList();
            var ptpdate = new DateTime(Convert.ToInt32(DataSplit[2]), Convert.ToInt32(DataSplit[1]), Convert.ToInt32(DataSplit[0])).ToString("yyyy-MM-dd");
            var flag = false;
            int i = 0;
            try
            {
                if (Amortization >= 0.0)
                {

                    i = DbHelper.InsertUpdateDelete(string.Format(QueryHelper.InsertDefaulterPartial, ApplicationId, Amortization, ptpdate, userid, paymentchannel, proofofpayment, penality_amount));


                    if (i > 0)
                    {
                        int k = DbHelper.InsertUpdateDelete(string.Format(QueryHelper.UpdatePartial, ApplicationId));
                        if (k > 0)
                        {
                            flag = true;
                        }
                    }
                }

                return Json(flag, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                return null;
            }
        }

        public bool DefaulterSMS(int Id, string mobno, string RefernceNo, string Amount)
        {
            bool flag = false;
            try
            {
                if (mobno[0] != '0' && mobno.Length == 10)
                {
                    mobno = '0' + mobno;
                }
                string username = Convert.ToString(ConfigurationManager.AppSettings["SMSUserName"]);
                string senderpassword = Convert.ToString(ConfigurationManager.AppSettings["SMSUserPassword"]);
                string port_num = Convert.ToString(ConfigurationManager.AppSettings["SMSPortNumber"]);
                string result = string.Empty;
                string SMSBody = string.Format(Convert.ToString(ConfigurationManager.AppSettings["SMSCollectionBucket"]), RefernceNo, Amount);
                flag = logger.ClicktoSMS(mobno, SMSBody);
                return flag;
            }
            catch (Exception ex)
            {
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                throw ex;
            }

        }
        [HttpPost]
        public ActionResult MovedToCollector(int AppId)
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.Collector))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            bool flag = false;
            int i = 0;
            try
            {
                i = DbHelper.InsertUpdateDelete(string.Format(QueryHelper.MoveToCollectorNew, AppId));
                if (i > 0)
                {
                    DbHelper.InsertUpdateDelete(string.Format(QueryHelper.InsertDefaulterCollectorMovementHistory, AppId));
                    flag = true;
                }
                return Json(flag, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                return null;
            }
        }

        public ActionResult MovedToAgency(int ApplicationId)
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.Collector))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            bool flag = false;
            try
            {
                int i = DbHelper.InsertUpdateDelete(string.Format(QueryHelper.MoveToAgency, ApplicationId));
                if (i > 0)
                {
                    flag = true;
                }
                return Json(flag, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                return null;
            }
        }

        [HttpPost]
        public ActionResult GetFieldVisitReport(int[] ApplicationNoArr)
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.Collector))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            List<FieldVisitReport> FieldVisitReportList = new List<FieldVisitReport>();
            try
            {
                int UserId = Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value);
                CommonOperation common = new CommonOperation();
                string applicationStr = string.Empty;
                if (ApplicationNoArr != null && ApplicationNoArr.Length > 0)
                {
                    foreach (var item in ApplicationNoArr)
                    {
                        applicationStr += "'" + item + "'" + ",";
                    }
                }
                if (applicationStr.Contains(','))
                {
                    applicationStr = applicationStr.TrimEnd(',');
                }
                var query = DbHelper.SelectMethod(string.Format(QueryHelper.FieldVisitReport, applicationStr));
                if (query != null && query.Rows.Count > 0)
                {
                    for (int i = 0; i < query.Rows.Count; i++)
                    {
                        if (!FieldVisitReportList.Any(x => x.application_no == Convert.ToInt32(query.Rows[i]["applicationno"])))
                        {
                            FieldVisitReport report = new FieldVisitReport
                            {
                                application_no = Convert.ToInt32(query.Rows[i]["applicationno"]),
                                defaulter_id = Convert.ToInt32(query.Rows[i]["defaulter_userid"]),
                                applicant_name = Convert.ToString(query.Rows[i]["first_name"]) + " " + Convert.ToString(query.Rows[i]["middle_name"]) + " " + Convert.ToString(query.Rows[i]["last_name"]),
                                address = Convert.ToString(query.Rows[i]["address"]) + ", " + Convert.ToString(query.Rows[i]["barangay_name"]) + ", " + Convert.ToString(query.Rows[i]["cityname"]) + ", " + Convert.ToString(query.Rows[i]["province_name"]) + ", " + Convert.ToString(query.Rows[i]["zipcode"]),
                                escalated_date = Convert.ToString(query.Rows[i]["escalated_date"]),
                                date_past_due = Convert.ToString(query.Rows[i]["past_due_date"]),
                                maturity_date = Convert.ToString(query.Rows[i]["maturity_date"]),
                                date_of_loan = Convert.ToString(query.Rows[i]["dateapplied"]),
                                outstanding_amount = Convert.ToDouble(query.Rows[i]["outstanding_amount"]),
                                personal_email = Convert.ToString(query.Rows[i]["personalemail"]),
                                payment_refernce = Convert.ToString(query.Rows[i]["reference_no"]),
                                phone_no = Convert.ToString(query.Rows[i]["personalcontactno"])
                            };
                            #region Changes Done by Nagesh for Penalty Amount in Defaulter Payment Bucket

                            int PastEmiCount = 0;
                            var PassedEMICount = DbHelper.SelectMethod(string.Format(QueryHelper.PastEmiCount, report.application_no));
                            if (PassedEMICount != null && PassedEMICount.Rows.Count > 0)
                            {
                                PastEmiCount = PassedEMICount.Rows[0]["emicount"] == DBNull.Value ? 0 : Convert.ToInt32(PassedEMICount.Rows[0]["emicount"]);
                            }
                            double PercentPenalty = (Convert.ToDouble(Convert.ToString(query.Rows[i]["late_rate"])) / 100);
                            double penalty = Convert.ToDouble(Convert.ToString(query.Rows[i]["late_penalty"]));

                            double pastduedays = Convert.ToDouble(query.Rows[i]["aging"]);
                            if (report.outstanding_amount > 0 && pastduedays > 0)
                            {
                                if (Convert.ToString(query.Rows[i]["termtype"]) == "Weekly")
                                {
                                    double _Mode = (pastduedays % 7);
                                    int _PenalityCount = Convert.ToInt32(pastduedays / 7);
                                    int _TotalPenalityCount = _Mode > 0 ? _PenalityCount + 1 : _PenalityCount;
                                    report.latepenalty = Math.Round((_TotalPenalityCount * penalty), 2);
                                    double latefee = (((report.outstanding_amount) * (PercentPenalty)) * (pastduedays));
                                    report.latepenalty = Math.Round((report.latepenalty + latefee), 2);
                                }
                                if (Convert.ToString(query.Rows[i]["termtype"]) == "Bi-Weekly")
                                {
                                    double _Mode = (pastduedays % 14);
                                    int _PenalityCount = Convert.ToInt32(pastduedays / 14);
                                    int _TotalPenalityCount = _Mode > 0 ? _PenalityCount + 1 : _PenalityCount;
                                    report.latepenalty = Math.Round((_TotalPenalityCount * penalty), 2);
                                    double latefee = (((report.outstanding_amount) * (PercentPenalty)) * (pastduedays));
                                    report.latepenalty = Math.Round((report.latepenalty + latefee), 2);
                                }
                                if (Convert.ToString(query.Rows[i]["termtype"]) == "Monthly")
                                {
                                    double _Mode = (pastduedays % 28);
                                    int _PenalityCount = Convert.ToInt32(pastduedays / 28);
                                    int _TotalPenalityCount = _Mode > 0 ? _PenalityCount + 1 : _PenalityCount;
                                    report.latepenalty = Math.Round((_TotalPenalityCount * penalty), 2);
                                    double latefee = (((report.outstanding_amount) * (PercentPenalty)) * (pastduedays));
                                    report.latepenalty = Math.Round((report.latepenalty + latefee), 2);
                                }
                            }
                            report.latepenalty = report.latepenalty > 0 ? report.latepenalty : 0;
                            report.total_amount_due = Math.Round((report.outstanding_amount + report.latepenalty), 2);
                            report.completedue = Convert.ToString(query.Rows[i]["total_amount_due"]);
                            #endregion
                            if (report.outstanding_amount > 0 && report.defaulter_id == UserId)
                            {
                                FieldVisitReportList.Add(report);
                            }
                        }
                    }
                }
                return Json(FieldVisitReportList, JsonRequestBehavior.AllowGet);
            }
            catch (Exception msg)
            {
                logger.WriteErrorLogs(msg, String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                return null;
            }

        }

        [HttpGet]
        public ActionResult TLSales()
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.Collector))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            AgingDataVM model = new AgingDataVM();
            try
            {

                DateTime now = DateTime.Now;
                string s = now.DayOfWeek.ToString();
                DateTime tomorrow = now.AddDays(1);
                model.ReportName = "Daily";
                model.AgingDataList = GetTLSalesData();

            }
            catch (Exception ex)
            {
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));

            }
            return View(model);
        }
        [HttpPost]
        public ActionResult TLSales(AgingDataVM model, string CommandName)
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.Collector))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            try
            {
                if (CommandName == "Week")
                {
                    model.ReportName = "Weekly";
                    model.AgingDataList = GetTLSalesData_Weekly();
                }
                else if (CommandName == "Month")
                {
                    model.ReportName = "Monthly";
                    model.AgingDataList = GetTLSalesData_Monthly();
                }
                else if (CommandName == "Year")
                {
                    model.ReportName = "Yearly";
                    model.AgingDataList = GetTLSalesData_Yearly();
                }
                else if (CommandName == "Daily")
                {
                    model.ReportName = "Daily";
                    model.AgingDataList = GetTLSalesData();
                }
            }
            catch (Exception ex)
            {
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";

            }
            return View(model);
        }

        public List<AgingData> GetTLSalesData()
        {
            List<AgingData> model = new List<AgingData>();
            try
            {
                DayOfWeek day = DateTime.Now.DayOfWeek;
                int days = day - DayOfWeek.Monday;
                string start = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1, 0, 0, 0).ToString("yyyy-MM-dd");
                DateTime enddate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1, 0, 0, 0).AddMonths(1).AddDays(-1);
                int userid = Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value);

                for (int j = 0; j < enddate.Day; j++)
                {
                    var queryforOfficerId = DbHelper.SelectMethod(string.Format(QueryHelper.GetOfficerIdTLSalesNew, start, start, userid));
                    if (queryforOfficerId != null && queryforOfficerId.Rows.Count > 0)
                    {
                        for (int i = 0; i < queryforOfficerId.Rows.Count; i++)
                        {
                            int _officerId = Convert.ToInt32(queryforOfficerId.Rows[i]["agent_id"]);
                            string _officerName = Convert.ToString(queryforOfficerId.Rows[i]["userfullname"]);
                            string dayName = Convert.ToDateTime(start).DayOfWeek.ToString();
                            var query = DbHelper.SelectMethod(string.Format(QueryHelper.GetTLSalesData, start, _officerId));
                            if (query != null && query.Rows.Count > 0)
                            {
                                for (int k = 0; k < query.Rows.Count; k++)
                                {
                                    model.Add(new AgingData
                                    {
                                        DayName = dayName,
                                        OfficerId = _officerId,
                                        OfficerName = _officerName,
                                        DueDate = Convert.ToDateTime(start).ToString("dd-MM-yyyy"),
                                        TotalMaturities = Convert.ToInt64(query.Rows[k]["TotalMaturity"]),
                                        TotalMaturities_Peso = Convert.ToString(query.Rows[k]["TotalMaturityAmount"]) != "" ? Math.Round(Convert.ToDecimal(query.Rows[k]["TotalMaturityAmount"]), 2) : 0,
                                        TotalPaid = Convert.ToInt64(query.Rows[k]["totalpaid"]),
                                        TotalPaid_Peso = Convert.ToString(query.Rows[k]["totalpaidamount"]) != "" ? Math.Round(Convert.ToDecimal(query.Rows[k]["totalpaidamount"]), 2) : 0,
                                        CollectionEfficiency = Convert.ToInt64(query.Rows[k]["efficiency_no"]),
                                        CollectionEfficiency_Peso = Convert.ToString(query.Rows[k]["efficiency_amount"]) != "" ? Math.Round(Convert.ToDecimal(query.Rows[k]["efficiency_amount"]), 2) : 0,
                                        Reloan = Convert.ToInt64(query.Rows[k]["Reloan_No"]),
                                        Reloan_Peso = Convert.ToString(query.Rows[k]["Reloan_Amount"]) != "" ? Math.Round(Convert.ToDecimal(query.Rows[k]["Reloan_Amount"]), 2) : 0,
                                        Preterm = Convert.ToInt64(query.Rows[k]["Preterm_No"]),
                                        Preterm_Peso = Convert.ToString(query.Rows[k]["Preterm_Amount"]) != "" ? Math.Round(Convert.ToDecimal(query.Rows[k]["Preterm_Amount"]), 2) : 0
                                    });
                                }
                            }
                            else
                            {
                                model.Add(new AgingData
                                {
                                    DueDate = Convert.ToDateTime(start).ToString("dd-MM-yyyy"),
                                    DayName = "",
                                    OfficerId = _officerId,
                                    OfficerName = _officerName,
                                    TotalMaturities = 0,
                                    TotalMaturities_Peso = 0,
                                    TotalPaid = 0,
                                    TotalPaid_Peso = 0,
                                    CollectionEfficiency = 0,
                                    CollectionEfficiency_Peso = 0,
                                    Reloan = 0,
                                    Reloan_Peso = 0,
                                    Preterm = 0,
                                    Preterm_Peso = 0
                                });
                            }
                        }

                    }
                    else if (queryforOfficerId.Rows.Count == 0)
                    {
                        model.Add(new AgingData
                        {
                            OfficerId = 0,
                            OfficerName = "",
                            DueDate = Convert.ToDateTime(start).ToString("dd-MM-yyyy"),
                            DayName = Convert.ToDateTime(start).DayOfWeek.ToString(),
                            TotalMaturities = 0,
                            TotalMaturities_Peso = 0,
                            TotalPaid = 0,
                            TotalPaid_Peso = 0,
                            CollectionEfficiency = 0,
                            CollectionEfficiency_Peso = 0,
                            Reloan = 0,
                            Reloan_Peso = 0,
                            Preterm = 0,
                            Preterm_Peso = 0
                        });
                    }

                    start = Convert.ToDateTime(start).AddDays(+1).ToString("yyyy-MM-dd");
                }
                if (model != null && model.Count > 0)
                {
                    model.FirstOrDefault().SumMaturity_No = Math.Round(model.Sum(x => x.TotalMaturities), 2);
                    model.FirstOrDefault().SumMaturity_Peso = Math.Round(model.Sum(x => x.TotalMaturities_Peso), 2);
                    model.FirstOrDefault().SumPaid_No = Math.Round(model.Sum(x => x.TotalPaid), 2);
                    model.FirstOrDefault().SumPaid_Peso = Math.Round(model.Sum(x => x.TotalPaid_Peso), 2);
                    model.FirstOrDefault().SumEffeciency_No = Math.Round((Convert.ToDecimal(model.Sum(x => x.CollectionEfficiency)) / Convert.ToDecimal(model.Count(x => x.CollectionEfficiency > 0) == 0 ? 1 : model.Count(x => x.CollectionEfficiency > 0))), 2);
                    model.FirstOrDefault().SumEffeciency_Peso = Math.Round((Convert.ToDecimal(model.Sum(x => x.CollectionEfficiency_Peso)) / Convert.ToDecimal(model.Count(x => x.CollectionEfficiency_Peso > 0) == 0 ? 1 : model.Count(x => x.CollectionEfficiency_Peso > 0))), 2);
                    model.FirstOrDefault().SumPreterm_No = Math.Round(model.Sum(x => x.Preterm), 2);
                    model.FirstOrDefault().SumPreterm_Peso = Math.Round(model.Sum(x => x.Preterm_Peso), 2);
                    model.FirstOrDefault().SumReloan_No = Math.Round(model.Sum(x => x.Reloan), 2);
                    model.FirstOrDefault().SumReloan_Peso = Math.Round(model.Sum(x => x.Reloan_Peso), 2);

                }
                return model;
            }
            catch (Exception ex)
            {
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                return null;
            }

        }

        public List<AgingData> GetTLSalesData_Weekly()
        {
            List<AgingData> model = new List<AgingData>();
            int userid = Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value);
            try
            {
                int year = DateTime.Now.Year, month = DateTime.Now.Month;
                var calendar = CultureInfo.CurrentCulture.Calendar;
                var firstDayOfWeek = CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek;
                var weekPeriods =
                Enumerable.Range(1, calendar.GetDaysInMonth(year, month))
                          .Select(d =>
                          {
                              var date = new DateTime(year, month, d);
                              var weekNumInYear = calendar.GetWeekOfYear(date, CalendarWeekRule.FirstDay, firstDayOfWeek);
                              return new { date, weekNumInYear };
                          })
                          .GroupBy(x => x.weekNumInYear)
                          .Select(x => new { DateFrom = x.First().date, To = x.Last().date })
                          .ToList();
                int WeekCount = 1;
                foreach (var item in weekPeriods)
                {
                    var queryforOfficerId = DbHelper.SelectMethod(string.Format(QueryHelper.GetOfficerIdTLSalesNew, item.DateFrom.ToString("yyyy-MM-dd"), item.To.ToString("yyyy-MM-dd"), userid));
                    if (queryforOfficerId != null && queryforOfficerId.Rows.Count > 0)
                    {
                        for (int k = 0; k < queryforOfficerId.Rows.Count; k++)
                        {
                            int _officerid = Convert.ToInt32(queryforOfficerId.Rows[k]["agent_id"]);
                            string _officername = Convert.ToString(queryforOfficerId.Rows[k]["userfullname"]);

                            var query = DbHelper.SelectMethod(string.Format(QueryHelper.GetTLSalesData_Weekly, item.DateFrom.ToString("yyyy-MM-dd"), item.To.ToString("yyyy-MM-dd"), _officerid));
                            if (query.Rows.Count > 0)
                            {
                                for (int i = 0; i < query.Rows.Count; i++)
                                {
                                    model.Add(new AgingData
                                    {
                                        DueDate = "Week " + Convert.ToString(WeekCount),
                                        DayName = "",
                                        DateRange = $"({item.DateFrom:yyyy-MM-dd} to {item.To:yyyy-MM-dd})",
                                        OfficerId = _officerid,
                                        OfficerName = _officername,
                                        TotalMaturities = Convert.ToInt64(query.Rows[i]["TotalMaturity"]),
                                        TotalMaturities_Peso = Convert.ToString(query.Rows[i]["TotalMaturityAmount"]) != "" ? Math.Round(Convert.ToDecimal(query.Rows[i]["TotalMaturityAmount"]), 2) : 0,
                                        TotalPaid = Convert.ToInt64(query.Rows[i]["totalpaid"]),
                                        TotalPaid_Peso = Convert.ToString(query.Rows[i]["totalpaidamount"]) != "" ? Math.Round(Convert.ToDecimal(query.Rows[i]["totalpaidamount"]), 2) : 0,
                                        CollectionEfficiency = Convert.ToInt64(query.Rows[i]["efficiency_no"]),
                                        CollectionEfficiency_Peso = Convert.ToString(query.Rows[i]["efficiency_amount"]) != "" ? Math.Round(Convert.ToDecimal(query.Rows[i]["efficiency_amount"]), 2) : 0,
                                        Reloan = Convert.ToInt64(query.Rows[i]["Reloan_No"]),
                                        Reloan_Peso = Convert.ToString(query.Rows[i]["Reloan_Amount"]) != "" ? Math.Round(Convert.ToDecimal(query.Rows[i]["Reloan_Amount"]), 2) : 0,
                                        Preterm = Convert.ToInt64(query.Rows[i]["Preterm_No"]),
                                        Preterm_Peso = Convert.ToString(query.Rows[i]["Preterm_Amount"]) != "" ? Math.Round(Convert.ToDecimal(query.Rows[i]["Preterm_Amount"]), 2) : 0
                                    });
                                }

                            }
                            else
                            {
                                model.Add(new AgingData
                                {
                                    DueDate = "Week " + Convert.ToString(WeekCount),
                                    DayName = "",
                                    DateRange = $"({item.DateFrom:yyyy-MM-dd} to {item.To:yyyy-MM-dd})",
                                    OfficerId = _officerid,
                                    OfficerName = _officername,
                                    TotalMaturities = 0,
                                    TotalMaturities_Peso = 0,
                                    TotalPaid = 0,
                                    TotalPaid_Peso = 0,
                                    CollectionEfficiency = 0,
                                    CollectionEfficiency_Peso = 0,
                                    Reloan = 0,
                                    Reloan_Peso = 0,
                                    Preterm = 0,
                                    Preterm_Peso = 0
                                });
                            }
                        }
                    }
                    else if (queryforOfficerId.Rows.Count == 0)
                    {
                        model.Add(new AgingData
                        {
                            OfficerId = 0,
                            OfficerName = "",
                            DateRange = $"({item.DateFrom:yyyy-MM-dd} to {item.To:yyyy-MM-dd})",
                            DueDate = "Week " + Convert.ToString(WeekCount),
                            DayName = "",
                            TotalMaturities = 0,
                            TotalMaturities_Peso = 0,
                            TotalPaid = 0,
                            TotalPaid_Peso = 0,
                            CollectionEfficiency = 0,
                            CollectionEfficiency_Peso = 0,
                            Reloan = 0,
                            Reloan_Peso = 0,
                            Preterm = 0,
                            Preterm_Peso = 0
                        });
                    }
                    WeekCount += 1;
                }
                if (model != null && model.Count > 0)
                {
                    model.FirstOrDefault().SumMaturity_No = Math.Round(model.Sum(x => x.TotalMaturities), 2);
                    model.FirstOrDefault().SumMaturity_Peso = Math.Round(model.Sum(x => x.TotalMaturities_Peso), 2);
                    model.FirstOrDefault().SumPaid_No = Math.Round(model.Sum(x => x.TotalPaid), 2);
                    model.FirstOrDefault().SumPaid_Peso = Math.Round(model.Sum(x => x.TotalPaid_Peso), 2);
                    model.FirstOrDefault().SumEffeciency_No = Math.Round((Convert.ToDecimal(model.Sum(x => x.CollectionEfficiency)) / Convert.ToDecimal(model.Count(x => x.CollectionEfficiency > 0) == 0 ? 1 : model.Count(x => x.CollectionEfficiency > 0))), 2);
                    model.FirstOrDefault().SumEffeciency_Peso = Math.Round((Convert.ToDecimal(model.Sum(x => x.CollectionEfficiency_Peso)) / Convert.ToDecimal(model.Count(x => x.CollectionEfficiency_Peso > 0) == 0 ? 1 : model.Count(x => x.CollectionEfficiency_Peso > 0))), 2);
                    model.FirstOrDefault().SumPreterm_No = Math.Round(model.Sum(x => x.Preterm), 2);
                    model.FirstOrDefault().SumPreterm_Peso = Math.Round(model.Sum(x => x.Preterm_Peso), 2);
                    model.FirstOrDefault().SumReloan_No = Math.Round(model.Sum(x => x.Reloan), 2);
                    model.FirstOrDefault().SumReloan_Peso = Math.Round(model.Sum(x => x.Reloan_Peso), 2);
                }
                return model;
            }
            catch (Exception msg)
            {
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                logger.WriteErrorLogs(msg, String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                return null;
            }
        }

        public List<AgingData> GetTLSalesData_Monthly()
        {
            List<AgingData> model = new List<AgingData>();
            DateTime date = DateTime.Now;
            List<DateTime> StartDateList = new List<DateTime>();
            List<DateTime> EndDateList = new List<DateTime>();
            int userid = Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value);
            for (int i = 1; i <= 12; i++)
            {
                var firstDayOfMonth = new DateTime(date.Year, i, 1);
                var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);
                StartDateList.Add(firstDayOfMonth);
                EndDateList.Add(lastDayOfMonth);
            }
            try
            {
                for (int m = 0; m < StartDateList.Count; m++)
                {
                    var queryforOfficerId = DbHelper.SelectMethod(string.Format(QueryHelper.GetOfficerIdTLSalesNew, StartDateList[m].ToString("yyyy-MM-dd"), EndDateList[m].ToString("yyyy-MM-dd"), userid));
                    if (queryforOfficerId != null && queryforOfficerId.Rows.Count > 0)
                    {
                        for (int k = 0; k < queryforOfficerId.Rows.Count; k++)
                        {
                            int _officerid = Convert.ToInt32(queryforOfficerId.Rows[k]["agent_id"]);
                            string _officername = Convert.ToString(queryforOfficerId.Rows[k]["userfullname"]);

                            var query = DbHelper.SelectMethod(string.Format(QueryHelper.GetTLSalesData_Weekly, StartDateList[m].ToString("yyyy-MM-dd"), EndDateList[m].ToString("yyyy-MM-dd"), _officerid));
                            if (query != null && query.Rows.Count > 0)
                            {
                                for (int i = 0; i < query.Rows.Count; i++)
                                {
                                    model.Add(new AgingData
                                    {
                                        DueDate = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(m + 1),
                                        DayName = "",
                                        OfficerId = _officerid,
                                        OfficerName = _officername,
                                        TotalMaturities = Convert.ToInt64(query.Rows[i]["TotalMaturity"]),
                                        TotalMaturities_Peso = Convert.ToString(query.Rows[i]["TotalMaturityAmount"]) != "" ? Math.Round(Convert.ToDecimal(query.Rows[i]["TotalMaturityAmount"]), 2) : 0,
                                        TotalPaid = Convert.ToInt64(query.Rows[i]["totalpaid"]),
                                        TotalPaid_Peso = Convert.ToString(query.Rows[i]["totalpaidamount"]) != "" ? Math.Round(Convert.ToDecimal(query.Rows[i]["totalpaidamount"]), 2) : 0,
                                        CollectionEfficiency = Convert.ToInt64(query.Rows[i]["efficiency_no"]),
                                        CollectionEfficiency_Peso = Convert.ToString(query.Rows[i]["efficiency_amount"]) != "" ? Math.Round(Convert.ToDecimal(query.Rows[i]["efficiency_amount"]), 2) : 0,
                                        Reloan = Convert.ToInt64(query.Rows[i]["Reloan_No"]),
                                        Reloan_Peso = Convert.ToString(query.Rows[i]["Reloan_Amount"]) != "" ? Math.Round(Convert.ToDecimal(query.Rows[i]["Reloan_Amount"]), 2) : 0,
                                        Preterm = Convert.ToInt64(query.Rows[i]["Preterm_No"]),
                                        Preterm_Peso = Convert.ToString(query.Rows[i]["Preterm_Amount"]) != "" ? Math.Round(Convert.ToDecimal(query.Rows[i]["Preterm_Amount"]), 2) : 0
                                    });
                                }
                            }
                            else
                            {
                                model.Add(new AgingData
                                {
                                    DueDate = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(m + 1),
                                    DayName = "",
                                    OfficerId = _officerid,
                                    OfficerName = _officername,
                                    TotalMaturities = 0,
                                    TotalMaturities_Peso = 0,
                                    TotalPaid = 0,
                                    TotalPaid_Peso = 0,
                                    CollectionEfficiency = 0,
                                    CollectionEfficiency_Peso = 0,
                                    Reloan = 0,
                                    Reloan_Peso = 0,
                                    Preterm = 0,
                                    Preterm_Peso = 0
                                });
                            }

                        }
                    }
                    else if (queryforOfficerId.Rows.Count == 0)
                    {
                        model.Add(new AgingData
                        {
                            OfficerId = 0,
                            OfficerName = "",
                            DueDate = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(m + 1),
                            DayName = "",
                            TotalMaturities = 0,
                            TotalMaturities_Peso = 0,
                            TotalPaid = 0,
                            TotalPaid_Peso = 0,
                            CollectionEfficiency = 0,
                            CollectionEfficiency_Peso = 0,
                            Reloan = 0,
                            Reloan_Peso = 0,
                            Preterm = 0,
                            Preterm_Peso = 0
                        });
                    }
                }
                if (model != null && model.Count > 0)
                {
                    model.FirstOrDefault().SumMaturity_No = Math.Round(model.Sum(x => x.TotalMaturities), 2);
                    model.FirstOrDefault().SumMaturity_Peso = Math.Round(model.Sum(x => x.TotalMaturities_Peso), 2);
                    model.FirstOrDefault().SumPaid_No = Math.Round(model.Sum(x => x.TotalPaid), 2);
                    model.FirstOrDefault().SumPaid_Peso = Math.Round(model.Sum(x => x.TotalPaid_Peso), 2);
                    model.FirstOrDefault().SumEffeciency_No = Math.Round((Convert.ToDecimal(model.Sum(x => x.CollectionEfficiency)) / Convert.ToDecimal(model.Count(x => x.CollectionEfficiency > 0) == 0 ? 1 : model.Count(x => x.CollectionEfficiency > 0))), 2);
                    model.FirstOrDefault().SumEffeciency_Peso = Math.Round((Convert.ToDecimal(model.Sum(x => x.CollectionEfficiency_Peso)) / Convert.ToDecimal(model.Count(x => x.CollectionEfficiency_Peso > 0) == 0 ? 1 : model.Count(x => x.CollectionEfficiency_Peso > 0))), 2);
                    model.FirstOrDefault().SumPreterm_No = Math.Round(model.Sum(x => x.Preterm), 2);
                    model.FirstOrDefault().SumPreterm_Peso = Math.Round(model.Sum(x => x.Preterm_Peso), 2);
                    model.FirstOrDefault().SumReloan_No = Math.Round(model.Sum(x => x.Reloan), 2);
                    model.FirstOrDefault().SumReloan_Peso = Math.Round(model.Sum(x => x.Reloan_Peso), 2);
                }
                return model;
            }
            catch (Exception ex)
            {
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                return null;
            }


        }

        public List<AgingData> GetTLSalesData_Yearly()
        {
            try
            {
                List<AgingData> model = new List<AgingData>();
                int userid = Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value);
                var query = DbHelper.SelectMethod(string.Format(QueryHelper.GetOldestApplication));
                if ((query != null && query.Rows.Count > 0))
                {
                    DateTime age = Convert.ToDateTime(query.Rows[0]["createdon"]);
                    string year = age.Year.ToString();
                    int _year = Convert.ToInt32(year);
                    string currentYear = DateTime.Now.Year.ToString();
                    int _year1 = Convert.ToInt32(currentYear);
                    int num = _year1 - _year;
                    for (int j = 0; j <= num; j++)
                    {
                        string firstDay = new DateTime(_year, 1, 1).ToString("yyyy-MM-dd");
                        string lastDay = new DateTime(_year, 12, 31).ToString("yyyy-MM-dd");
                        var queryforOfficerId = DbHelper.SelectMethod(string.Format(QueryHelper.GetOfficerIdTLSalesNew, firstDay, lastDay, userid));
                        if (queryforOfficerId != null && queryforOfficerId.Rows.Count > 0)
                        {
                            for (int k = 0; k < queryforOfficerId.Rows.Count; k++)
                            {
                                int _officerid = Convert.ToInt32(queryforOfficerId.Rows[k]["agent_id"]);
                                string _officername = Convert.ToString(queryforOfficerId.Rows[k]["userfullname"]);
                                var query1 = DbHelper.SelectMethod(string.Format(QueryHelper.GetTLSalesData_Weekly, firstDay, lastDay, _officerid));
                                if (query1 != null && query1.Rows.Count > 0)
                                {
                                    for (int i = 0; i < query1.Rows.Count; i++)
                                    {
                                        model.Add(new AgingData
                                        {
                                            DueDate = Convert.ToString(_year),
                                            DayName = "",
                                            OfficerId = _officerid,
                                            OfficerName = _officername,
                                            TotalMaturities = Convert.ToInt64(query1.Rows[i]["TotalMaturity"]),
                                            TotalMaturities_Peso = Convert.ToString(query1.Rows[i]["TotalMaturityAmount"]) != "" ? Math.Round(Convert.ToDecimal(query1.Rows[i]["TotalMaturityAmount"]), 2) : 0,
                                            TotalPaid = Convert.ToInt64(query1.Rows[i]["totalpaid"]),
                                            TotalPaid_Peso = Convert.ToString(query1.Rows[i]["totalpaidamount"]) != "" ? Math.Round(Convert.ToDecimal(query1.Rows[i]["totalpaidamount"]), 2) : 0,
                                            CollectionEfficiency = Convert.ToInt64(query1.Rows[i]["efficiency_no"]),
                                            CollectionEfficiency_Peso = Convert.ToString(query1.Rows[i]["efficiency_amount"]) != "" ? Math.Round(Convert.ToDecimal(query1.Rows[i]["efficiency_amount"]), 2) : 0,
                                            Reloan = Convert.ToInt64(query1.Rows[i]["Reloan_No"]),
                                            Reloan_Peso = Convert.ToString(query1.Rows[i]["Reloan_Amount"]) != "" ? Math.Round(Convert.ToDecimal(query1.Rows[i]["Reloan_Amount"]), 2) : 0,
                                            Preterm = Convert.ToInt64(query1.Rows[i]["Preterm_No"]),
                                            Preterm_Peso = Convert.ToString(query1.Rows[i]["Preterm_Amount"]) != "" ? Math.Round(Convert.ToDecimal(query1.Rows[i]["Preterm_Amount"]), 2) : 0
                                        });

                                    }
                                }
                                else
                                {
                                    model.Add(new AgingData
                                    {
                                        DueDate = Convert.ToString(_year),
                                        DayName = "",
                                        OfficerId = _officerid,
                                        OfficerName = _officername,
                                        TotalMaturities = 0,
                                        TotalMaturities_Peso = 0,
                                        TotalPaid = 0,
                                        TotalPaid_Peso = 0,
                                        CollectionEfficiency = 0,
                                        CollectionEfficiency_Peso = 0,
                                        Reloan = 0,
                                        Reloan_Peso = 0,
                                        Preterm = 0,
                                        Preterm_Peso = 0
                                    });
                                }
                            }
                        }
                        else if (queryforOfficerId.Rows.Count == 0)
                        {
                            model.Add(new AgingData
                            {
                                OfficerId = 0,
                                OfficerName = "",
                                DueDate = Convert.ToString(_year),
                                DayName = "",
                                TotalMaturities = 0,
                                TotalMaturities_Peso = 0,
                                TotalPaid = 0,
                                TotalPaid_Peso = 0,
                                CollectionEfficiency = 0,
                                CollectionEfficiency_Peso = 0,
                                Reloan = 0,
                                Reloan_Peso = 0,
                                Preterm = 0,
                                Preterm_Peso = 0
                            });
                        }
                        _year += 1;
                    }
                    if (model != null && model.Count > 0)
                    {
                        model.FirstOrDefault().SumMaturity_No = Math.Round(model.Sum(x => x.TotalMaturities), 2);
                        model.FirstOrDefault().SumMaturity_Peso = Math.Round(model.Sum(x => x.TotalMaturities_Peso), 2);
                        model.FirstOrDefault().SumPaid_No = Math.Round(model.Sum(x => x.TotalPaid), 2);
                        model.FirstOrDefault().SumPaid_Peso = Math.Round(model.Sum(x => x.TotalPaid_Peso), 2);
                        model.FirstOrDefault().SumEffeciency_No = Math.Round((Convert.ToDecimal(model.Sum(x => x.CollectionEfficiency)) / Convert.ToDecimal(model.Count(x => x.CollectionEfficiency > 0) == 0 ? 1 : model.Count(x => x.CollectionEfficiency > 0))), 2);
                        model.FirstOrDefault().SumEffeciency_Peso = Math.Round((Convert.ToDecimal(model.Sum(x => x.CollectionEfficiency_Peso)) / Convert.ToDecimal(model.Count(x => x.CollectionEfficiency_Peso > 0) == 0 ? 1 : model.Count(x => x.CollectionEfficiency_Peso > 0))), 2);
                        model.FirstOrDefault().SumPreterm_No = Math.Round(model.Sum(x => x.Preterm), 2);
                        model.FirstOrDefault().SumPreterm_Peso = Math.Round(model.Sum(x => x.Preterm_Peso), 2);
                        model.FirstOrDefault().SumReloan_No = Math.Round(model.Sum(x => x.Reloan), 2);
                        model.FirstOrDefault().SumReloan_Peso = Math.Round(model.Sum(x => x.Reloan_Peso), 2);
                    }
                }
                return model;
            }
            catch (Exception ex)
            {
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                return null;
            }

        }

        [HttpGet]
        public ActionResult LoanHistory(int UserID, string ActionName, int ApplicationNo)
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.Collector))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
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
                        double _Penality = _commonOperation.GetPaidPenalty(Convert.ToString(dtLoanHistory.Rows[i]["applicationno"])); ;
                        _commonOperation.GetOutstandingAmount(Convert.ToString(dtLoanHistory.Rows[i]["applicationno"]));

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
        public ActionResult DefaulterReport()
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.Collector))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            DefaulterReportModel model = new DefaulterReportModel();
            try
            {
                if (HttpContext.Request.Cookies[CookiesKey.UserId].Value != null)
                {
                    int UserId = Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value);
                    model.Report_Name = "Weekly";
                    model = GetDefaulterReportWeekly(UserId);
                    if (model == null)
                    {
                        model = new DefaulterReportModel();
                    }
                }
                else
                {
                    return Json("Session Expired! Please Login Again.", JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ex)
            {
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
            }
            return View(model);
        }
        [HttpPost]
        public ActionResult DefaulterReport(DefaulterReportModel model, string CommandName)
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.Collector))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            try
            {
                if (HttpContext.Request.Cookies[CookiesKey.UserId].Value != null)
                {
                    int UserId = Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value);
                    if (CommandName == "Week")
                    {
                        model = GetDefaulterReportWeekly(UserId);
                    }
                    else if (CommandName == "Month")
                    {
                        model = GetDefaulterReportMonthly(UserId);
                    }
                    else if (CommandName == "Year")
                    {
                        model = GetDefaulterReportYearly(UserId);
                    }
                }
                else
                {
                    return Json("Session Expired! Please Login Again.", JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
            }
            return View(model);
        }

        public DefaulterReportModel GetDefaulterReportWeekly(int UserId)
        {
            DefaulterReportModel model = new DefaulterReportModel
            {
                Report_Name = "Weekly"
            };
            try
            {
                int year = DateTime.Now.Year, month = DateTime.Now.Month;
                var calendar = CultureInfo.CurrentCulture.Calendar;
                var firstDayOfWeek = CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek;
                var weekPeriods =
                Enumerable.Range(1, calendar.GetDaysInMonth(year, month))
                          .Select(d =>
                          {
                              var date = new DateTime(year, month, d);
                              var weekNumInYear = calendar.GetWeekOfYear(date, CalendarWeekRule.FirstDay, firstDayOfWeek);
                              return new { date, weekNumInYear };
                          })
                          .GroupBy(x => x.weekNumInYear)
                          .Select(x => new { DateFrom = x.First().date, To = x.Last().date })
                          .ToList();
                int WeekCount = 1;
                foreach (var item in weekPeriods)
                {
                    model.date.Add("Week " + Convert.ToString(WeekCount));
                    model.DateRange.Add($"({item.DateFrom:yyyy-MM-dd} to {item.To:yyyy-MM-dd})");
                    if (item.DateFrom.Date > DateTime.Now.Date)
                    {
                        model.Number_Defaulter.Add(0);
                        model.Outstanding_Loan_Principle.Add(0);
                        model.Outstanding_amount.Add(0);
                        model.Non_Starter_Number.Add(0);
                        model.Partial_Payment_Number.Add(0);
                        model.Defaulter_Collection.Add(0);
                        model.Total_No_of_Defaulter.Add(0);
                    }
                    else
                    {
                        var queryforDefaulterCount = DbHelper.SelectMethod(string.Format(QueryHelper.DefaulterReportNewQuery, item.DateFrom.ToString("yyyy-MM-dd"), item.To.ToString("yyyy-MM-dd"), UserId));
                        if (queryforDefaulterCount != null && queryforDefaulterCount.Rows.Count > 0)
                        {
                            for (int i = 0; i < queryforDefaulterCount.Rows.Count; i++)
                            {
                                model.Number_Defaulter.Add(Convert.ToInt32(queryforDefaulterCount.Rows[i]["defaulter_userid"]));
                                model.Outstanding_Loan_Principle.Add(Convert.ToDouble(queryforDefaulterCount.Rows[i]["outstanding_loan_principle"]));
                                model.Outstanding_amount.Add(Convert.ToDouble(queryforDefaulterCount.Rows[i]["outstanding_amount"]));
                                model.Non_Starter_Number.Add(Convert.ToInt32(queryforDefaulterCount.Rows[i]["non_starter"]));
                                model.Partial_Payment_Number.Add(Convert.ToInt32(queryforDefaulterCount.Rows[i]["Partial_Payment_No"]));
                                model.Defaulter_Collection.Add(Convert.ToInt32(queryforDefaulterCount.Rows[i]["Defaulter_Collection"]));
                                model.Total_No_of_Defaulter.Add((Convert.ToInt32(queryforDefaulterCount.Rows[i]["defaulter_userid"]) + Convert.ToInt32(queryforDefaulterCount.Rows[i]["non_starter"])));
                            }
                        }

                    }

                    WeekCount += 1;

                }
                if (model.Number_Defaulter != null && model.Number_Defaulter.Count > 0)
                {
                    model.Sum_Number_Defaulter = model.Number_Defaulter.Sum();
                }
                if (model.Outstanding_Loan_Principle != null && model.Outstanding_Loan_Principle.Count > 0)
                {
                    model.Sum_Outstanding_Loan_Principle = model.Outstanding_Loan_Principle.Sum();
                }

            }
            catch (Exception msg)
            {
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                logger.WriteErrorLogs(msg, String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                return null;
            }
            return model;
        }

        public DefaulterReportModel GetDefaulterReportYearly(int UserId)
        {
            DefaulterReportModel model = new DefaulterReportModel
            {
                Report_Name = "Yearly"
            };
            try
            {
                var query = DbHelper.SelectMethod(string.Format(QueryHelper.GetOldestApplication));
                if ((query != null && query.Rows.Count > 0))
                {
                    DateTime age = Convert.ToDateTime(query.Rows[0]["createdon"]);
                    string year = age.Year.ToString();
                    int _year = Convert.ToInt32(year);
                    string currentYear = DateTime.Now.Year.ToString();
                    int _year1 = Convert.ToInt32(currentYear);
                    int num = _year1 - _year;
                    for (int j = 0; j <= num; j++)
                    {
                        string firstDay = new DateTime(_year, 1, 1).ToString("yyyy-MM-dd");
                        string lastDay = new DateTime(_year, 12, 31).ToString("yyyy-MM-dd");
                        model.date.Add(Convert.ToString(_year));
                        model.DateRange.Add($"");
                        var queryforDefaulterCount = DbHelper.SelectMethod(string.Format(QueryHelper.DefaulterReportNewQuery, firstDay, lastDay, UserId));
                        if (queryforDefaulterCount != null && queryforDefaulterCount.Rows.Count > 0)
                        {
                            for (int i = 0; i < queryforDefaulterCount.Rows.Count; i++)
                            {
                                model.Number_Defaulter.Add(Convert.ToInt32(queryforDefaulterCount.Rows[i]["defaulter_userid"]));
                                model.Outstanding_Loan_Principle.Add(Convert.ToDouble(queryforDefaulterCount.Rows[i]["outstanding_loan_principle"]));
                                model.Outstanding_amount.Add(Convert.ToDouble(queryforDefaulterCount.Rows[i]["outstanding_amount"]));
                                model.Non_Starter_Number.Add(Convert.ToInt32(queryforDefaulterCount.Rows[i]["non_starter"]));
                                model.Partial_Payment_Number.Add(Convert.ToInt32(queryforDefaulterCount.Rows[i]["Partial_Payment_No"]));
                                model.Defaulter_Collection.Add(Convert.ToInt32(queryforDefaulterCount.Rows[i]["Defaulter_Collection"]));
                                model.Total_No_of_Defaulter.Add(Convert.ToInt32(queryforDefaulterCount.Rows[i]["defaulter_userid"]) + Convert.ToInt32(queryforDefaulterCount.Rows[i]["non_starter"]));
                            }
                        }
                        _year += 1;
                    }
                    if (model.Number_Defaulter != null && model.Number_Defaulter.Count > 0)
                    {
                        model.Sum_Number_Defaulter = model.Number_Defaulter.Sum();
                    }
                    if (model.Outstanding_Loan_Principle != null && model.Outstanding_Loan_Principle.Count > 0)
                    {
                        model.Sum_Outstanding_Loan_Principle = model.Outstanding_Loan_Principle.Sum();
                    }
                }

            }
            catch (Exception ex)
            {
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                return null;
            }
            return model;
        }

        public DefaulterReportModel GetDefaulterReportMonthly(int UserId)
        {
            DefaulterReportModel model = new DefaulterReportModel();
            model.Report_Name = "Monthly";
            DateTime date = DateTime.Now;
            List<DateTime> StartDateList = new List<DateTime>();
            List<DateTime> EndDateList = new List<DateTime>();
            for (int i = 1; i <= 12; i++)
            {
                var firstDayOfMonth = new DateTime(date.Year, i, 1);
                var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);
                StartDateList.Add(firstDayOfMonth);
                EndDateList.Add(lastDayOfMonth);
            }
            try
            {
                for (int m = 0; m < StartDateList.Count; m++)
                {
                    model.date.Add(CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(m + 1));
                    model.DateRange.Add($"");
                    if (StartDateList[m].Date > DateTime.Now.Date)
                    {
                        model.Number_Defaulter.Add(0);
                        model.Outstanding_Loan_Principle.Add(0);
                        model.Outstanding_amount.Add(0);
                        model.Non_Starter_Number.Add(0);
                        model.Partial_Payment_Number.Add(0);
                        model.Defaulter_Collection.Add(0);
                        model.Total_No_of_Defaulter.Add(0);
                    }
                    else
                    {
                        var queryforDefaulterCount = DbHelper.SelectMethod(string.Format(QueryHelper.DefaulterReportNewQuery, StartDateList[m].ToString("yyyy-MM-dd"), EndDateList[m].ToString("yyyy-MM-dd"), UserId));
                        if (queryforDefaulterCount != null && queryforDefaulterCount.Rows.Count > 0)
                        {
                            for (int i = 0; i < queryforDefaulterCount.Rows.Count; i++)
                            {
                                model.Number_Defaulter.Add(Convert.ToInt32(queryforDefaulterCount.Rows[i]["defaulter_userid"]));
                                model.Outstanding_Loan_Principle.Add(Convert.ToDouble(queryforDefaulterCount.Rows[i]["outstanding_loan_principle"]));
                                model.Outstanding_amount.Add(Convert.ToDouble(queryforDefaulterCount.Rows[i]["outstanding_amount"]));
                                model.Non_Starter_Number.Add(Convert.ToInt32(queryforDefaulterCount.Rows[i]["non_starter"]));
                                model.Partial_Payment_Number.Add(Convert.ToInt32(queryforDefaulterCount.Rows[i]["Partial_Payment_No"]));
                                model.Defaulter_Collection.Add(Convert.ToInt32(queryforDefaulterCount.Rows[i]["Defaulter_Collection"]));
                                model.Total_No_of_Defaulter.Add(Convert.ToInt32(queryforDefaulterCount.Rows[i]["defaulter_userid"]) + Convert.ToInt32(queryforDefaulterCount.Rows[i]["non_starter"]));
                            }
                        }
                    }
                }
                if (model.Number_Defaulter != null && model.Number_Defaulter.Count > 0)
                {
                    model.Sum_Number_Defaulter = model.Number_Defaulter.Sum();
                }
                if (model.Outstanding_Loan_Principle != null && model.Outstanding_Loan_Principle.Count > 0)
                {
                    model.Sum_Outstanding_Loan_Principle = model.Outstanding_Loan_Principle.Sum();
                }

            }

            catch (Exception ex)
            {
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                return null;
            }
            return model;
        }

        public ActionResult GetDataAccToTerm(int id)
        {
           
            Terms termVM = new Terms();
            try
            {
                DataTable dt = new DataTable();
                dt = DbHelper.SelectMethod(string.Format(QueryHelper.GetDataAccToTerm, id));
                if (dt != null && dt.Rows.Count > 0)
                {
                    termVM.TermTypeName = Convert.ToString(dt.Rows[0]["termtype"]);
                    termVM.Rate = Convert.ToString(dt.Rows[0]["interest_rate"]) != "" ? Convert.ToDecimal(dt.Rows[0]["interest_rate"]) : 0;
                    termVM.term_Value = Convert.ToString(dt.Rows[0]["term_value"]) != "" ? Convert.ToString(dt.Rows[0]["term_value"]) : "0";
                }
                return Json(termVM);
            }
            catch (Exception ex)
            {
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                return null;
            }
        }

        public ActionResult AddPreterm(int ApplicationNo, string LoanAmount, string InterestRate, string TotalAmountDue, string PretermDeduction, string MaturityDate, string TermValue, int TermId, int TermType)
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.Collector))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            ApplicationPersonalVerificationDetails personalinfo = new ApplicationPersonalVerificationDetails();
            double _LoanAmount = Convert.ToDouble(LoanAmount);
            double _InterestRate = Convert.ToDouble(InterestRate);
            double _TotalAmountDue = Convert.ToDouble(TotalAmountDue);
            var flag = "OK";
            int UserId = Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value);
            try
            {
                if (TermId > 0 && TermType > 0)
                {
                    var ExistingApplication = DbHelper.SelectMethod(String.Format(QueryHelper.CheckExistingPreterm, ApplicationNo));
                    if (ExistingApplication != null && ExistingApplication.Rows.Count > 0)
                    {
                        flag = $"Active Preterm Exist for Application No : {ApplicationNo} PreTerm Application No is {ExistingApplication.Rows[0]["newappno"]}";
                        return Json(flag, JsonRequestBehavior.AllowGet);
                    }

                    var data1 = DbHelper.SelectMethod(string.Format(QueryHelper.InsertApplicationRecordPreterm, ApplicationNo, _LoanAmount, false, TermId, TermType, UserId));
                    int NewApplicationNo = 0;
                    int VDataId = 0;
                    if (data1 != null)
                    {
                        NewApplicationNo = Convert.ToInt32(data1.Rows[0]["currval"]);
                    }
                    if (NewApplicationNo > 0)
                    {
                        DbHelper.InsertUpdateDelete(string.Format(QueryHelper.InsertPretermrecord, ApplicationNo, NewApplicationNo, false, 0));

                        DbHelper.InsertUpdateDelete(string.Format(QueryHelper.UpdateIsPretermGenerated, ApplicationNo));
                        new Thread(() => CopyFolder(NewApplicationNo, ApplicationNo)).Start();
                        int k = DbHelper.InsertUpdateDelete(string.Format(QueryHelper.UpdateIsPretermNew, NewApplicationNo));
                        if (k > 0)
                        {
                            flag = "Preterm Applied Successfully";
                        }
                        else
                        {
                            flag = "Preterm Failed due to Update Query Exception.";
                            return Json(flag, JsonRequestBehavior.AllowGet);
                        }
                        var data2 = DbHelper.SelectMethod(string.Format(QueryHelper.InsertApplicationData_VerificationDetails_Preterm, ApplicationNo));
                        if (data2 != null && data2.Rows.Count > 0)
                        {
                            VDataId = Convert.ToInt32(data2.Rows[0]["currval"]);
                        }
                        int n = DbHelper.InsertUpdateDelete(string.Format(QueryHelper.UpdateNewApplicationNo_VerificationData, VDataId, NewApplicationNo));
                        if (n == 0)
                        {
                            flag = "New Application No not Updated in tblapplicationdata_verification_details Table";
                        }


                        var data = DbHelper.SelectMethod(string.Format(QueryHelper.GetPretermData, ApplicationNo));
                        if (data != null)
                        {
                            personalinfo.ApplicationNo = Convert.ToInt32(data.Rows[0]["application_no"]);
                            personalinfo.RequestedAmt = Convert.ToInt32(data.Rows[0]["requestedamt"]);
                            personalinfo.RequestedTerms = Convert.ToInt32(data.Rows[0]["requestedterms"]);
                            personalinfo.Id1_Status = Convert.ToBoolean(data.Rows[0]["id1_status"]);
                            personalinfo.Id2_Status = Convert.ToBoolean(data.Rows[0]["id2_status"]);
                            personalinfo.Pb_Status = Convert.ToBoolean(data.Rows[0]["pb_status"]);
                            personalinfo.Pi_Status = Convert.ToBoolean(data.Rows[0]["pi_status"]);
                            personalinfo.Remarks = Convert.ToString(data.Rows[0]["remarks"]);
                            personalinfo.AdditionalBirthPlace = Convert.ToString(data.Rows[0]["addtional_birth_place"]);
                            personalinfo.AdditionalProvincialAddress = Convert.ToString(data.Rows[0]["additonal_provincial_address"]);
                            personalinfo.AdditionalLoanPurpose = Convert.ToString(data.Rows[0]["additional_loan_purpose"]);
                            personalinfo.AdditionalRequestedLoanAmount = _LoanAmount;
                            personalinfo.AdditionalRequestedTerm = Convert.ToDouble(data.Rows[0]["additional_requested_term"]);
                            personalinfo.AdditionalEmployedDuration = Convert.ToInt32(data.Rows[0]["additional_employed_duration"]);
                            personalinfo.AdditionalJobPosition = Convert.ToString(data.Rows[0]["additional_job_position"]);
                            personalinfo.AdditionalJobLevel = Convert.ToString(data.Rows[0]["additional_job_level"]);
                            personalinfo.AdditionalPayDate = Convert.ToString(data.Rows[0]["additional_paydate"]);
                            personalinfo.FamilyPersonalNameStatus = Convert.ToBoolean(data.Rows[0]["family_personal_name_status"]);
                            personalinfo.FamilyPersonalContactStatus = Convert.ToBoolean(data.Rows[0]["family_personal_contact_status"]);
                            personalinfo.FamilyRelationWithBorrowerStatus = Convert.ToBoolean(data.Rows[0]["family_relation_with_borrower_status"]);
                            personalinfo.FamilyBorrowerKnownDurationStatus = Convert.ToBoolean(data.Rows[0]["family_borrower_known_duration_status"]);
                            personalinfo.FamilyAddressVerificationStatus = Convert.ToBoolean(data.Rows[0]["family_address_verification_status"]);
                            personalinfo.FamilyBorrowerWorkingPlaceStatus = Convert.ToBoolean(data.Rows[0]["family_borrower_working_place_status"]);
                            personalinfo.FriendPersonalNameStatus = Convert.ToBoolean(data.Rows[0]["friend_personal_name_status"]);
                            personalinfo.FriendPersonalContactStatus = Convert.ToBoolean(data.Rows[0]["friend_personal_contact_status"]);
                            personalinfo.FriendRelationWithBorrowerStatus = Convert.ToBoolean(data.Rows[0]["friend_relation_with_borrower_status"]);
                            personalinfo.FriendBorrowerKnownDurationStatus = Convert.ToBoolean(data.Rows[0]["friend_borrower_known_duration_status"]);
                            personalinfo.FriendAddressVerificationStatus = Convert.ToBoolean(data.Rows[0]["friend_address_verification_status"]);
                            personalinfo.FriendBorrowerWorkingPlaceStatus = Convert.ToBoolean(data.Rows[0]["friend_borrower_working_place"]);
                            personalinfo.CoWorkerPersonalNameStatus = Convert.ToBoolean(data.Rows[0]["co_worker_personal_name_status"]);
                            personalinfo.CoWorkerPersonalContactStatus = Convert.ToBoolean(data.Rows[0]["co_worker_personal_contact_status"]);
                            personalinfo.CoWorkerRelationWithBorrowerStatus = Convert.ToBoolean(data.Rows[0]["co_worker_relation_with_borrower_status"]);
                            personalinfo.CoWorkerBorrowerKnownDurationStatus = Convert.ToBoolean(data.Rows[0]["co_worker_borrower_known_duration_status"]);
                            personalinfo.CoWorkerAddressVerificationStatus = Convert.ToBoolean(data.Rows[0]["co_worker_address_verification_status"]);
                            personalinfo.CoWorkerBorrowerWorkingPlaceStatus = Convert.ToBoolean(data.Rows[0]["co_worker_borrower_working_place"]);
                            personalinfo.EmploymentNameOfWorkContactStatus = Convert.ToBoolean(data.Rows[0]["employement_nameof_work_contact_status"]);
                            personalinfo.EmploymentNoOfWorkConatctStatus = Convert.ToBoolean(data.Rows[0]["employement_no_of_work_contact_status"]);
                            personalinfo.EmploymentBorrowerWorkingStatus = Convert.ToBoolean(data.Rows[0]["employement_borrower_working_status"]);
                            personalinfo.EmploymentBorrowerPositionStatus = Convert.ToBoolean(data.Rows[0]["employement_borrower_position_status"]);
                            personalinfo.EmploymentBorrowerMonthlySalaryStatus = Convert.ToBoolean(data.Rows[0]["employement_borrower_monthly_salary_status"]);
                            personalinfo.EmploymentBorrowerAttendanceStatus = Convert.ToBoolean(data.Rows[0]["employement_borrower_attendance_status"]);
                            personalinfo.EmploymentBorrowerBankPayrollStatus = Convert.ToBoolean(data.Rows[0]["employement_borrower_bank_payroll_status"]);
                            personalinfo.FamilyPersonalName = Convert.ToString(data.Rows[0]["family_personal_name"]);
                            personalinfo.FamilyPersonalContact = Convert.ToString(data.Rows[0]["family_personal_contact"]);
                            personalinfo.FamilyRelationWithBorrower = Convert.ToString(data.Rows[0]["family_relation_with_borrower"]);
                            personalinfo.FamilyBorrowerKnownDuration = Convert.ToString(data.Rows[0]["family_borrower_known_duration"]);
                            personalinfo.FamilyAddressVerification = Convert.ToString(data.Rows[0]["family_address_verification"]);
                            personalinfo.FamilyBorrowerWorkingPlace = Convert.ToString(data.Rows[0]["family_borrower_working_place"]);
                            personalinfo.FriendPersonalName = Convert.ToString(data.Rows[0]["friend_personal_name"]);
                            personalinfo.FriendPersonalConatact = Convert.ToString(data.Rows[0]["friend_personal_contact"]);
                            personalinfo.FriendRelationWithBorrower = Convert.ToString(data.Rows[0]["friend_relation_with_borrower"]);
                            personalinfo.FriendBorrowerKnownDuration = Convert.ToString(data.Rows[0]["friend_borrower_known_duration"]);
                            personalinfo.FriendAddressVerification = Convert.ToString(data.Rows[0]["friend_address_verification"]);
                            personalinfo.FriendBorrowerWorkingPlace = Convert.ToString(data.Rows[0]["friend_borrower_working_place"]);
                            personalinfo.CoWorkerPersonalName = Convert.ToString(data.Rows[0]["co_worker_personal_name"]);
                            personalinfo.CoWorkerPersonalConatact = Convert.ToString(data.Rows[0]["co_worker_personal_contact"]);
                            personalinfo.CoWorkerRelationWithBorrower = Convert.ToString(data.Rows[0]["co_worker_relation_with_borrower"]);
                            personalinfo.CoWorkerBorrowerKnownDuration = Convert.ToString(data.Rows[0]["co_worker_borrower_known_duration"]);
                            personalinfo.CoWorkerAddressVerification = Convert.ToString(data.Rows[0]["co_worker_address_verification"]);
                            personalinfo.CoWorkerBorrowerWorkingPlace = Convert.ToString(data.Rows[0]["co_worker_borrower_working_place"]);
                            personalinfo.EmploymentNameOfWorkContact = Convert.ToString(data.Rows[0]["employement_nameof_work_contact"]);
                            personalinfo.EmploymentNoOfWorkConatct = Convert.ToString(data.Rows[0]["employement_no_of_work_contact"]);
                            personalinfo.EmploymentBorrowerWorking = Convert.ToString(data.Rows[0]["employement_borrower_working"]);
                            personalinfo.EmploymentBorrowerPosition = Convert.ToString(data.Rows[0]["employement_borrower_position"]);
                            personalinfo.EmploymentBorrowerMonthlySalary = Convert.ToString(data.Rows[0]["employement_borrower_monthly_salary"]);
                            personalinfo.EmploymentBorrowerAttendance = Convert.ToString(data.Rows[0]["employement_borrower_attendance"]);
                            personalinfo.EmploymentBorrowerBankPayroll = Convert.ToString(data.Rows[0]["employement_borrower_bank_payroll"]);
                            personalinfo.Question1_Status = Convert.ToBoolean(data.Rows[0]["question1_status"]);
                            personalinfo.Question2_Status = Convert.ToBoolean(data.Rows[0]["question1_status"]);
                            personalinfo.Question3_Status = Convert.ToBoolean(data.Rows[0]["question1_status"]);
                            personalinfo.Question4_Status = Convert.ToBoolean(data.Rows[0]["question1_status"]);
                            personalinfo.Question5_Status = Convert.ToBoolean(data.Rows[0]["question1_status"]);
                            personalinfo.Question6_Status = Convert.ToBoolean(data.Rows[0]["question1_status"]);
                            personalinfo.Question7_Status = Convert.ToBoolean(data.Rows[0]["question1_status"]);
                            personalinfo.Question8_Status = Convert.ToBoolean(data.Rows[0]["question1_status"]);
                            personalinfo.Question9_Status = Convert.ToBoolean(data.Rows[0]["question1_status"]);
                            personalinfo.Question10_Status = Convert.ToBoolean(data.Rows[0]["question1_status"]);
                            personalinfo.ApprovedLoanAmount = 0.0;
                            personalinfo.ApprovedMaturityDate = Convert.ToString(MaturityDate);
                            personalinfo.ApprovedTerm = Convert.ToDouble(TermValue);
                            personalinfo.ApprovedTotalDueAmount = Convert.ToDouble(data.Rows[0]["approved_total_amount_due"]);
                            personalinfo.ApprovedInterestRate = Convert.ToDouble(data.Rows[0]["approved_interest_rate"]);
                            personalinfo.NearestLandmark = Convert.ToString(data.Rows[0]["nearest_landmark"]);
                            personalinfo.TansferResidence = Convert.ToString(data.Rows[0]["tansfer_residence"]);
                            personalinfo.SpouseName = Convert.ToString(data.Rows[0]["spouse_name"]);
                            personalinfo.SpouseOccupation = Convert.ToString(data.Rows[0]["spouse_occupation"]);
                            personalinfo.NumberOfDependent = Convert.ToInt32(data.Rows[0]["number_of_dependent"]);
                            personalinfo.MotherWork = Convert.ToString(data.Rows[0]["mother_work"]);
                            personalinfo.FatherName = Convert.ToString(data.Rows[0]["father_name"]);
                            personalinfo.FatherWork = Convert.ToString(data.Rows[0]["father_work"]);
                            personalinfo.LivingWithMother = Convert.ToString(data.Rows[0]["living_with_mother"]);
                            personalinfo.SiblingCount = Convert.ToInt32(data.Rows[0]["sibling_count"]);
                            personalinfo.SiblingWorks = Convert.ToString(data.Rows[0]["sibling_works"]);
                            personalinfo.Occupation = Convert.ToString(data.Rows[0]["occupation"]);
                            personalinfo.NetIncome = Convert.ToInt32(data.Rows[0]["net_income"]);
                            personalinfo.PendingResignation = Convert.ToString(data.Rows[0]["pending_resignation"]);
                            personalinfo.OtherSourceOfIncome = Convert.ToString(data.Rows[0]["other_source_of_income"]);
                            personalinfo.KnowAboutCashmart = Convert.ToString(data.Rows[0]["know_about_cashmart"]);
                            personalinfo.PendingLoanFronOtherland = Convert.ToString(data.Rows[0]["pending_loan_fron_otherland"]);
                            personalinfo.BankLoanOrCreditCard = Convert.ToString(data.Rows[0]["bank_loan_or_credit_card"]);
                            personalinfo.PermanentAddress = Convert.ToString(data.Rows[0]["permanent_address"]);
                            personalinfo.BankId = Convert.ToString(data.Rows[0]["bankid"]) != string.Empty ? Convert.ToInt32(data.Rows[0]["bankid"]) : 0;
                            personalinfo.FamilyName1 = Convert.ToString(data.Rows[0]["family_name1"]);
                            personalinfo.FamilyAddress1 = Convert.ToString(data.Rows[0]["family_address1"]);
                            personalinfo.FamilyContact1 = Convert.ToString(data.Rows[0]["family_contact1"]);
                            personalinfo.FamilyRelation1 = Convert.ToString(data.Rows[0]["family_relation1"]);
                            personalinfo.FamilyName2 = Convert.ToString(data.Rows[0]["family_name2"]);
                            personalinfo.FamilyAddress2 = Convert.ToString(data.Rows[0]["family_address2"]);
                            personalinfo.FamilyContact2 = Convert.ToString(data.Rows[0]["family_contact2"]);
                            personalinfo.FamilyRelation2 = Convert.ToString(data.Rows[0]["family_relation2"]);
                            personalinfo.OptionalName = Convert.ToString(data.Rows[0]["optional_name"]);
                            personalinfo.OPptionalAddress = Convert.ToString(data.Rows[0]["optional_address"]);
                            personalinfo.OptionalContact = Convert.ToString(data.Rows[0]["optional_contact"]);
                            personalinfo.OptionalRelation = Convert.ToString(data.Rows[0]["optional_relation"]);
                        }

                        int i = DbHelper.InsertUpdateDelete(string.Format(QueryHelper.Inserttblapplicationdata_verification_details_Preterm, NewApplicationNo, personalinfo.RequestedAmt, personalinfo.RequestedTerms, personalinfo.Id1_Status, personalinfo.Id2_Status, personalinfo.Pb_Status, personalinfo.Pi_Status, personalinfo.Remarks, personalinfo.AdditionalBirthPlace, personalinfo.AdditionalProvincialAddress, personalinfo.AdditionalLoanPurpose, personalinfo.AdditionalRequestedLoanAmount, personalinfo.AdditionalRequestedTerm, personalinfo.AdditionalEmployedDuration, personalinfo.AdditionalJobPosition, personalinfo.AdditionalJobLevel, personalinfo.AdditionalPayDate, personalinfo.FamilyPersonalNameStatus, personalinfo.FamilyPersonalContactStatus, personalinfo.FamilyRelationWithBorrowerStatus, personalinfo.FamilyBorrowerKnownDurationStatus, personalinfo.FamilyAddressVerificationStatus, personalinfo.FamilyBorrowerWorkingPlaceStatus, personalinfo.FriendPersonalNameStatus, personalinfo.FriendPersonalContactStatus, personalinfo.FriendRelationWithBorrowerStatus, personalinfo.FriendBorrowerKnownDurationStatus, personalinfo.FriendAddressVerificationStatus, personalinfo.FriendBorrowerWorkingPlaceStatus, personalinfo.CoWorkerPersonalNameStatus, personalinfo.CoWorkerPersonalContactStatus, personalinfo.CoWorkerRelationWithBorrowerStatus, personalinfo.CoWorkerBorrowerKnownDurationStatus, personalinfo.CoWorkerAddressVerificationStatus, personalinfo.CoWorkerBorrowerWorkingPlaceStatus, personalinfo.EmploymentNameOfWorkContactStatus, personalinfo.EmploymentNoOfWorkConatctStatus, personalinfo.EmploymentBorrowerWorkingStatus, personalinfo.EmploymentBorrowerPositionStatus, personalinfo.EmploymentBorrowerMonthlySalaryStatus, personalinfo.EmploymentBorrowerAttendanceStatus, personalinfo.EmploymentBorrowerBankPayrollStatus, personalinfo.FamilyPersonalName, personalinfo.FamilyPersonalContact, personalinfo.FamilyRelationWithBorrower, personalinfo.FamilyBorrowerKnownDuration, personalinfo.FamilyAddressVerification, personalinfo.FamilyBorrowerWorkingPlace, personalinfo.FriendPersonalName, personalinfo.FriendPersonalConatact, personalinfo.FriendRelationWithBorrower, personalinfo.FriendBorrowerKnownDuration, personalinfo.FriendAddressVerification, personalinfo.FriendBorrowerWorkingPlace, personalinfo.CoWorkerPersonalName, personalinfo.CoWorkerPersonalConatact, personalinfo.CoWorkerRelationWithBorrower, personalinfo.CoWorkerBorrowerKnownDuration, personalinfo.CoWorkerAddressVerification, personalinfo.CoWorkerBorrowerWorkingPlace, personalinfo.EmploymentNameOfWorkContact, personalinfo.EmploymentNoOfWorkConatct, personalinfo.EmploymentBorrowerWorking, personalinfo.EmploymentBorrowerPosition, personalinfo.EmploymentBorrowerMonthlySalary, personalinfo.EmploymentBorrowerAttendance, personalinfo.EmploymentBorrowerBankPayroll, personalinfo.Question1_Status, personalinfo.Question2_Status, personalinfo.Question3_Status, personalinfo.Question4_Status, personalinfo.Question5_Status, personalinfo.Question6_Status, personalinfo.Question7_Status, personalinfo.Question8_Status, personalinfo.Question9_Status, personalinfo.Question10_Status, _LoanAmount, personalinfo.ApprovedTerm, _InterestRate, personalinfo.ApprovedMaturityDate, _TotalAmountDue, personalinfo.NearestLandmark, personalinfo.TansferResidence, personalinfo.SpouseName, personalinfo.SpouseOccupation, personalinfo.NumberOfDependent, personalinfo.MotherWork, personalinfo.FatherName, personalinfo.FatherWork, personalinfo.LivingWithMother, personalinfo.SiblingCount, personalinfo.SiblingWorks, personalinfo.Occupation, personalinfo.NetIncome, personalinfo.PendingResignation, personalinfo.OtherSourceOfIncome, personalinfo.KnowAboutCashmart, personalinfo.PendingLoanFronOtherland, personalinfo.BankLoanOrCreditCard, personalinfo.PermanentAddress, personalinfo.BankId, personalinfo.FamilyName1, personalinfo.FamilyAddress1, personalinfo.FamilyContact1, personalinfo.FamilyRelation1, personalinfo.FamilyName2, personalinfo.FamilyAddress2, personalinfo.FamilyContact2, personalinfo.FamilyRelation2, personalinfo.OptionalName, personalinfo.OPptionalAddress, personalinfo.OptionalContact, personalinfo.OptionalRelation));
                        if (i > 0)
                        {
                            flag = "Preterm Applied Successfully";
                        }

                        int h = DbHelper.InsertUpdateDelete(string.Format(QueryHelper.InsertDataVerificationDetails, NewApplicationNo, ApplicationNo));
                        if (h > 0)
                        {
                            flag = "Preterm Applied Successfully";
                        }
                    }
                }
                else {
                    flag = "Check the Term and Term Type.";
                }
            }
            catch (Exception ex)
            {
                flag = "Something Went Wrong, Please check the Logs.";
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
            }
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AddReloan(int ApplicationNo, string LoanAmount, string InterestRate, string TotalAmountDue, string ReloanDeduction, string MaturityDate, string TermValue, int TermId, int TermType)
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.Collector))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            ApplicationPersonalVerificationDetails personalinfo = new ApplicationPersonalVerificationDetails();
            double _LoanAmount = Convert.ToDouble(LoanAmount);
            int UserId = Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value);
            var flag = false;

            try
            {
                if (TermId > 0 && TermType > 0)
                {
                    var data1 = DbHelper.SelectMethod(string.Format(QueryHelper.InsertApplicationRecordReloan, ApplicationNo, _LoanAmount, false, TermId, TermType, UserId));
                    int NewApplicationNo = 0;
                    if (data1 != null)
                    {
                        NewApplicationNo = Convert.ToInt32(data1.Rows[0]["currval"]);
                    }
                    if (NewApplicationNo > 0)
                    {
                        flag = true;
                        DbHelper.InsertUpdateDelete(String.Format(QueryHelper.InsertPretermrecord, ApplicationNo, NewApplicationNo, true, 0));
                        new Thread(() => CopyFolder(NewApplicationNo, ApplicationNo)).Start();
                    }
                    else
                    {
                        flag = false;
                    }
                    int f = DbHelper.InsertUpdateDelete(string.Format(QueryHelper.UpdateisforFSBucketFalse, ApplicationNo));
                    if (f > 0)
                    {
                        flag = true;
                    }
                    else
                    {
                        flag = false;
                    }
                    int k = DbHelper.InsertUpdateDelete(string.Format(QueryHelper.UpdateIsForReLoanNew, NewApplicationNo));
                    if (k > 0)
                    {
                        flag = true;
                    }
                    else
                    {
                        flag = false;
                    }

                    personalinfo.ApprovedMaturityDate = Convert.ToString(MaturityDate);
                    personalinfo.ApprovedTerm = Convert.ToDouble(TermValue);
                    int i = DbHelper.InsertUpdateDelete(string.Format(QueryHelper.InsertPersonalVerificationData, NewApplicationNo, ApplicationNo, personalinfo.ApprovedMaturityDate, personalinfo.ApprovedTerm, LoanAmount, InterestRate, TotalAmountDue));


                    if (i > 0)
                    {
                        flag = true;
                    }
                    else
                    {
                        flag = false;
                    }
                    int h = DbHelper.InsertUpdateDelete(string.Format(QueryHelper.InsertDataVerificationDetails, NewApplicationNo, ApplicationNo));
                    if (h > 0)
                    {
                        flag = true;
                    }
                    else
                    {
                        flag = false;
                    }
                }
                return Json(flag, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                return null;
            }
        }



        [HttpPost]
        public ActionResult ExportLOD(String ApplicationArray)
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.Collector))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            int AppId = 0;
            CommonOperation comnoperation1 = new CommonOperation();
            try
            {
                List<String> ApplicationList = new List<string>();
                ApplicationList = ApplicationArray.Split('~').ToList();
                String __FIleName = DateTime.Now.ToString("ddMMMyyyy");
                var __path = System.IO.Path.Combine("~/Content/LOD/");
                if (!Directory.Exists(Server.MapPath(__path)))
                {
                    System.IO.Directory.CreateDirectory(Server.MapPath(__path));
                }
                if (ApplicationList != null && ApplicationList.Count > 0)
                {
                    List<String> FilePaths = new List<string>();
                    foreach (var item in ApplicationList)
                    {
                        if (!String.IsNullOrWhiteSpace(item))
                        {
                            var list = item.Split('_');
                            AppId = Convert.ToInt32(list[1]);
                            DataTable dataTable = DbHelper.SelectMethod(string.Format(QueryHelper.GetLoanDetailListNew, AppId));
                            string filename = string.Empty;
                            var query = DbHelper.SelectMethod(string.Format(QueryHelper.GetAppIDLOD, AppId));
                            if (query != null && query.Rows.Count > 0)
                            {
                                filename = query.Rows[0]["lod_path"].ToString();
                                if (filename != null && filename != "")
                                {
                                    var path = Path.Combine(Server.MapPath(__path), filename);
                                    FilePaths.Add(path);
                                }
                            }
                        }
                    }

                    using (ZipFile zip = new ZipFile(Path.Combine(Server.MapPath(__path), __FIleName + ".zip")))
                    {
                        try
                        {
                            zip.AddFiles(FilePaths, false, "");
                            zip.Save();
                        }
                        catch (Exception ex)
                        {

                            TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                            logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                        }
                    }
                }
                return File(Path.Combine(Server.MapPath(__path), __FIleName + ".zip"), "application/zip", __FIleName + ".zip");
            }
            catch (Exception ex)
            {
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                return null;
            }

        }

        public bool DefaulterLODMail(int AppId, string Name, string ContractNumber, string Date, decimal OutstandingBalance, decimal Penality, decimal LateFee, decimal TotalAmount, string EmailId, string EmaiDate)
        {
            bool flag = false;
            try
            {
                string FileName = CommonMethods.CreateLOD_PDF(AppId, Name, ContractNumber, Date, OutstandingBalance, LateFee, Penality, TotalAmount, EmaiDate, Server);
                if (!String.IsNullOrWhiteSpace(FileName))
                {
                    int i = DbHelper.InsertUpdateDelete(string.Format(QueryHelper.InsertLOD, AppId, FileName));
                    if (i > 0)
                    {
                        flag = true;
                        SendMail(Name, FileName, EmailId);
                    }
                }
                return flag;
            }
            catch (Exception ex)
            {
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                throw ex;
            }

        }

        public ActionResult DefaulterCheckedLOD(String[] ApplicationList)
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.Collector))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            bool flag = false;
            CommonOperation comnoperation1 = new CommonOperation();
            try
            {
                if (ApplicationList != null && ApplicationList.Length > 0)
                {
                    foreach (var item in ApplicationList)
                    {
                        var list = item.Split('_');
                        int AppId = Convert.ToInt32(list[1]);
                        string Name = list[6];
                        string date = DateTime.Now.ToShortDateString();
                        string ContactNo = list[2];
                        string ReferenceNo = list[3];
                        decimal Penality = Convert.ToDecimal(list[9]);
                        decimal LateFee = Convert.ToDecimal(list[8]);
                        decimal Amount = Convert.ToDecimal(list[7]);
                        decimal TotalAmount = Convert.ToDecimal(list[4]) + Penality + LateFee;
                        string EmailId = list[5];
                        string EmaiDate = list[10];
                        DefaulterLODMail(AppId, Name, ReferenceNo, date, Amount, Penality, LateFee, TotalAmount, EmailId, EmaiDate);
                    }
                    flag = true;
                }

                return Json(flag, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                return null;
            }


        }


        [HttpGet]
        public ActionResult TLReport()
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.Collector))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            DefaulterReportModel model = new DefaulterReportModel();
            try
            {
                if (HttpContext.Request.Cookies[CookiesKey.UserId].Value != null)
                {
                    int UserId = Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value);
                    model.Report_Name = "Monthly";
                    model = GetTLReportMonthly(UserId);
                }
                else
                {
                    return Json("Session Expired! Please Login Again.", JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ex)
            {

                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));

            }
            return View(model);
        }

        public DefaulterReportModel GetTLReportMonthly(int UserId)
        {
            DefaulterReportModel model = new DefaulterReportModel();
            model.Report_Name = "Monthly";
            DateTime date = DateTime.Now;
            List<DateTime> StartDateList = new List<DateTime>();
            List<DateTime> EndDateList = new List<DateTime>();
            for (int i = 1; i <= 12; i++)
            {
                var firstDayOfMonth = new DateTime(date.Year, i, 1);
                var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);
                StartDateList.Add(firstDayOfMonth);
                EndDateList.Add(lastDayOfMonth);
            }
            try
            {
                for (int m = 0; m < StartDateList.Count; m++)
                {
                    model.date.Add(CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(m + 1));
                    var queryforOfficerId = DbHelper.SelectMethod(string.Format(QueryHelper.GetOfficerId, StartDateList[m].ToString("yyyy-MM-dd"), EndDateList[m].ToString("yyyy-MM-dd"), UserId));
                    if (queryforOfficerId != null && queryforOfficerId.Rows.Count > 0)
                    {
                        for (int i = 0; i < queryforOfficerId.Rows.Count; i++)
                        {
                            model.DefaulterIdList.Add(Convert.ToInt32(queryforOfficerId.Rows[i]["defaulter_userid"]));
                            model.DefaulterNameList.Add(Convert.ToString(queryforOfficerId.Rows[i]["userfullname"]));

                            var queryforDefaulterCount1 = DbHelper.SelectMethod(string.Format(QueryHelper.GetNODefaulterMonthlyByOfficerNew, StartDateList[m].ToString("yyyy-MM-dd"), EndDateList[m].ToString("yyyy-MM-dd"), Convert.ToString(queryforOfficerId.Rows[i]["defaulter_userid"])));
                            if (queryforDefaulterCount1 != null && queryforDefaulterCount1.Rows.Count > 0)
                            {
                                model.DefaulterCountList.Add(
                                    new DefaulterCount
                                    {
                                        Date = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(m + 1),
                                        DefaulterId = Convert.ToInt32(queryforOfficerId.Rows[i]["defaulter_userid"]),
                                        DefaulterName = Convert.ToString(queryforOfficerId.Rows[i]["userfullname"]),
                                        Number_Defaulter = Convert.ToInt32(queryforDefaulterCount1.Rows[0]["defaulter_userid_no"])
                                    });
                            }
                            else if (queryforDefaulterCount1.Rows.Count == 0)
                            {
                                model.DefaulterCountList.Add(
                                    new DefaulterCount
                                    {
                                        Date = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(m + 1),
                                        DefaulterId = Convert.ToInt32(queryforOfficerId.Rows[i]["defaulter_userid"]),
                                        DefaulterName = Convert.ToString(queryforOfficerId.Rows[i]["userfullname"]),
                                        Number_Defaulter = 0,
                                    });
                            }

                            var queryforLoanPrinciple1 = DbHelper.SelectMethod(string.Format(QueryHelper.GetLoanPrincipleMonthlyByOfficerNew, StartDateList[m].ToString("yyyy-MM-dd"), EndDateList[m].ToString("yyyy-MM-dd"), Convert.ToString(queryforOfficerId.Rows[i]["defaulter_userid"])));
                            if (queryforLoanPrinciple1 != null && queryforLoanPrinciple1.Rows.Count > 0)
                            {
                                model.OutstandingLoanprincipalList.Add(
                                    new OutstandingLoanprincipal
                                    {
                                        Date = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(m + 1),
                                        DefaulterId = Convert.ToInt32(queryforOfficerId.Rows[i]["defaulter_userid"]),
                                        DefaulterName = Convert.ToString(queryforOfficerId.Rows[i]["userfullname"]),
                                        Outstanding_Loan_Principle = Convert.ToDouble(queryforLoanPrinciple1.Rows[0]["loan_principle"])
                                    });
                            }
                            else if (queryforLoanPrinciple1.Rows.Count == 0)
                            {
                                model.OutstandingLoanprincipalList.Add(
                                    new OutstandingLoanprincipal
                                    {
                                        Date = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(m + 1),
                                        DefaulterId = Convert.ToInt32(queryforOfficerId.Rows[i]["defaulter_userid"]),
                                        DefaulterName = Convert.ToString(queryforOfficerId.Rows[i]["userfullname"]),
                                        Outstanding_Loan_Principle = 0
                                    });
                            }

                            var queryforOutstandingBalance1 = DbHelper.SelectMethod(string.Format(QueryHelper.GetOutstandingBalanceMonthlyByOfficerNew, StartDateList[m].ToString("yyyy-MM-dd"), EndDateList[m].ToString("yyyy-MM-dd"), Convert.ToString(queryforOfficerId.Rows[i]["defaulter_userid"])));
                            if (queryforOutstandingBalance1 != null && queryforOutstandingBalance1.Rows.Count > 0)
                            {
                                model.OutstandingLoanAmountList.Add(
                                    new OutstandingLoanAmount
                                    {
                                        Date = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(m + 1),
                                        DefaulterId = Convert.ToInt32(queryforOfficerId.Rows[i]["defaulter_userid"]),
                                        DefaulterName = Convert.ToString(queryforOfficerId.Rows[i]["userfullname"]),
                                        Outstanding_amount = Convert.ToDouble(queryforOutstandingBalance1.Rows[0]["outstanding_amount"])
                                    });
                            }
                            else if (queryforOutstandingBalance1.Rows.Count == 0)
                            {
                                model.OutstandingLoanAmountList.Add(
                                    new OutstandingLoanAmount
                                    {
                                        Date = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(m + 1),
                                        DefaulterId = Convert.ToInt32(queryforOfficerId.Rows[i]["defaulter_userid"]),
                                        DefaulterName = Convert.ToString(queryforOfficerId.Rows[i]["userfullname"]),
                                        Outstanding_amount = 0
                                    });
                            }

                            var queryforNonStarter2 = DbHelper.SelectMethod(string.Format(QueryHelper.Getnon_starterMonthlyByOfficerNew, StartDateList[m].ToString("yyyy-MM-dd"), EndDateList[m].ToString("yyyy-MM-dd"), Convert.ToString(queryforOfficerId.Rows[i]["defaulter_userid"])));
                            if (queryforNonStarter2 != null && queryforNonStarter2.Rows.Count > 0)
                            {
                                model.NonStarterNumberList.Add(
                                    new NonStarterNumber
                                    {
                                        Date = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(m + 1),
                                        DefaulterId = Convert.ToInt32(queryforOfficerId.Rows[i]["defaulter_userid"]),
                                        DefaulterName = Convert.ToString(queryforOfficerId.Rows[i]["userfullname"]),
                                        Non_Starter_Number = Convert.ToInt32(queryforNonStarter2.Rows[0]["non_starter"])
                                    });
                            }
                            else if (queryforNonStarter2.Rows.Count == 0)
                            {
                                model.NonStarterNumberList.Add(
                                     new NonStarterNumber
                                     {
                                         Date = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(m + 1),
                                         DefaulterId = Convert.ToInt32(queryforOfficerId.Rows[i]["defaulter_userid"]),
                                         DefaulterName = Convert.ToString(queryforOfficerId.Rows[i]["userfullname"]),
                                         Non_Starter_Number = 0
                                     });
                            }

                            var queryforPartialPayment1 = DbHelper.SelectMethod(string.Format(QueryHelper.GetPartialPaymentMonthlyByOfficerNew, StartDateList[i].ToString("yyyy-MM-dd"), EndDateList[m].ToString("yyyy-MM-dd"), Convert.ToString(queryforOfficerId.Rows[i]["defaulter_userid"])));
                            if (queryforPartialPayment1 != null && queryforPartialPayment1.Rows.Count > 0)
                            {
                                model.PartialPaymentNumberList.Add(
                                   new PartialPaymentNumber
                                   {
                                       Date = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(m + 1),
                                       DefaulterId = Convert.ToInt32(queryforOfficerId.Rows[i]["defaulter_userid"]),
                                       DefaulterName = Convert.ToString(queryforOfficerId.Rows[i]["userfullname"]),
                                       Partial_Payment_Number = Convert.ToInt32(queryforPartialPayment1.Rows[0]["Partial_Payment_No"])
                                   });
                            }
                            else if (queryforPartialPayment1.Rows.Count == 0)
                            {
                                model.PartialPaymentNumberList.Add(
                                    new PartialPaymentNumber
                                    {
                                        Date = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(m + 1),
                                        DefaulterId = Convert.ToInt32(queryforOfficerId.Rows[i]["defaulter_userid"]),
                                        DefaulterName = Convert.ToString(queryforOfficerId.Rows[i]["userfullname"]),
                                        Partial_Payment_Number = 0
                                    });
                            }

                            var queryforDefaulterCollection1 = DbHelper.SelectMethod(string.Format(QueryHelper.GetDefaulter_CollectionMonthlyByMonthlyNew, StartDateList[m].ToString("yyyy-MM-dd"), EndDateList[m].ToString("yyyy-MM-dd"), Convert.ToString(queryforOfficerId.Rows[i]["defaulter_userid"])));
                            if (queryforDefaulterCollection1 == null)
                            {
                            }
                            if (queryforDefaulterCollection1 != null && queryforDefaulterCollection1.Rows.Count > 0)
                            {
                                model.DefaulterCollectionList.Add(
                                  new DefaulterCollection
                                  {
                                      Date = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(m + 1),
                                      DefaulterId = Convert.ToInt32(queryforOfficerId.Rows[i]["defaulter_userid"]),
                                      DefaulterName = Convert.ToString(queryforOfficerId.Rows[i]["userfullname"]),
                                      Defaulter_Collection = Convert.ToInt32(queryforDefaulterCollection1.Rows[0]["Defaulter_Collection"])
                                  });
                            }
                            else if (queryforDefaulterCollection1.Rows.Count == 0)
                            {
                                model.DefaulterCollectionList.Add(
                                  new DefaulterCollection
                                  {
                                      Date = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(m + 1),
                                      DefaulterId = Convert.ToInt32(queryforOfficerId.Rows[i]["defaulter_userid"]),
                                      DefaulterName = Convert.ToString(queryforOfficerId.Rows[i]["userfullname"]),
                                      Defaulter_Collection = 0
                                  });
                            }

                        }

                    }
                    else if (queryforOfficerId.Rows.Count == 0)
                    {
                        model.DefaulterIdList.Add(0);
                        model.DefaulterNameList.Add("");
                        model.DefaulterCountList.Add(new DefaulterCount());
                        model.OutstandingLoanprincipalList.Add(new OutstandingLoanprincipal());
                        model.OutstandingLoanAmountList.Add(new OutstandingLoanAmount());
                        model.NonStarterNumberList.Add(new NonStarterNumber());
                        model.PartialPaymentNumberList.Add(new PartialPaymentNumber());
                        model.DefaulterCollectionList.Add(new DefaulterCollection());
                    }
                    int SUmDefault = model.DefaulterCountList.Where(x => x.Date == CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(m + 1)).ToList().Sum(x => x.Number_Defaulter);
                    model.SumOfDefaulterList.Add(
                        new SumOfDefaulter
                        {
                            Date = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(m + 1),
                            Sum_Of_Defaulter = SUmDefault
                        });
                    double OutstandingLoanPrinciple = model.OutstandingLoanprincipalList.Where(x => x.Date == CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(m + 1)).ToList().Sum(x => x.Outstanding_Loan_Principle);
                    model.SumOfOutstandingList.Add(
                        new SumOfOutstanding
                        {
                            Date = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(m + 1),
                            Sum_Of_Outstanding = OutstandingLoanPrinciple
                        });
                    double SumofAgency = model.DefaulterCollectionList.Where(x => x.Date == CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(m + 1)).ToList().Sum(x => x.Defaulter_Collection);
                    model.SumOfAgencyList.Add(
                        new SumOfAgency
                        {
                            Date = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(m + 1),
                            Sum_Of_Agency = SumofAgency
                        });
                }
                return model;
            }
            catch (Exception ex)
            {

                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                return null;
            }

        }

        [HttpPost]
        public ActionResult SearchApplication(int id)
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.Collector))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            try
            {
                var query = DbHelper.SelectMethod(String.Format(QueryHelper.SearchApplicationNo, id));
                if (query != null && query.Rows.Count > 0)
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

                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                return null;
            }

        }
        [HttpPost]
        public ActionResult SearchApplicationByReference(int id)
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.Collector))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            try
            {
                var query = DbHelper.SelectMethod(String.Format(QueryHelper.SearchApplicationNo, id));
                if (query != null && query.Rows.Count > 0)
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

                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                return null;
            }

        }
        [HttpPost]
        public ActionResult SearchApplicationbyName(int id)
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.Collector))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            try
            {
                var query = DbHelper.SelectMethod(String.Format(QueryHelper.SearchApplicationNo, id));
                if (query != null && query.Rows.Count > 0)
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

                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                return null;
            }

        }

        /// <summary>
        /// Defaulter Report Bucket
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult DefaulterReportBucket()
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.Collector))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            ApplicationRecordVM model = new ApplicationRecordVM();
            try
            {
                model.ApplicationRecordList = GetDefaulterBucketData();

            }
            catch (Exception ex)
            {
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
            }
            return View(model);
        }

        /// <summary>
        /// Get Defaulter Bucket Data
        /// </summary>
        /// <returns></returns>
        public List<ApplicationRecordVM> GetDefaulterBucketData()
        {
            List<ApplicationRecordVM> DefaulterList = new List<ApplicationRecordVM>();
            int userid = Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value);
            try
            {
                var query = DbHelper.SelectMethod(string.Format(QueryHelper.GetDefaulterUserData, userid));
                if (query != null && query.Rows.Count > 0)
                {
                    for (int i = 0; i < query.Rows.Count; i++)
                    {
                        ApplicationRecordVM record = new ApplicationRecordVM();
                        record.ApplicationNo = Convert.ToInt32(query.Rows[i]["applicationno"]);
                        record.Assignby = Convert.ToInt32(query.Rows[i]["created_by"]);
                        record.PersonalEmail = Convert.ToString(query.Rows[i]["personalemail"]);
                        record.PersonalContactNo = Convert.ToString(query.Rows[i]["personalcontactno"]);
                        record.Name = Convert.ToString(query.Rows[i]["first_name"]) + " " + Convert.ToString(query.Rows[i]["middle_name"]) + " " + Convert.ToString(query.Rows[i]["last_name"]);
                        record.Address = Convert.ToString(query.Rows[i]["address"]) + " " + Convert.ToString(query.Rows[i]["barangay_name"]) + " " + Convert.ToString(query.Rows[i]["cityname"]) + " " + Convert.ToString(query.Rows[i]["province_name"]) + " " + Convert.ToString(query.Rows[i]["zipcode"]);
                        record.AssignbyName = Convert.ToString(query.Rows[i]["userfullname"]);
                        DefaulterList.Add(record);
                    }
                }
                return DefaulterList;
            }
            catch (Exception ex)
            {
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                return null;
            }

        }

        public ActionResult DefaulterCheckedSMS(String[] ApplicationList)
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.Collector))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            bool flag = false;
            int AppId = 0;
            CommonOperation comnoperation1 = new CommonOperation();
            try
            {
                if (ApplicationList != null && ApplicationList.Length > 0)
                {
                    foreach (var item in ApplicationList)
                    {
                        var list = item.Split('_');
                        AppId = Convert.ToInt32(list[1]);
                        if (AppId > 0)
                        {
                            DefaulterModel defaulter = new DefaulterModel();
                            defaulter = DefaulterSMSData(AppId);
                            if (defaulter != null)
                            {
                                DefaulterSMS(AppId, defaulter.personalcontactno, defaulter.refernce_no, defaulter.outstanding_amount.ToString());
                            }
                        }
                    }
                    flag = true;
                }

                return Json(flag, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                return null;
            }


        }

        /// <summary>
        /// FSBucket SMS in  particular lead
        /// </summary>
        /// <param name="AppId"></param>
        /// <param name="ContactNo"></param>
        /// <returns></returns>
        public ActionResult FSBucketSMS(int AppId, string ContactNo)
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.Collector))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            bool result = false;
            try
            {
                if (ContactNo[0] != '0' && ContactNo.Length == 10)
                {
                    ContactNo = '0' + ContactNo;
                }
                // sms function
                result = SendToSMSInContact(AppId, ContactNo);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                return null;
            }
        }

        /// <summary>
        /// Send To SMS In personal Contact
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="mobno"></param>
        /// <returns></returns>
        public bool SendToSMSInContact(int Id, string mobno)
        {

            if (mobno[0] != '0' && mobno.Length == 10)
            {
                mobno = '0' + mobno;
            }
            bool flag = logger.ClicktoSMS(mobno, Convert.ToString(ConfigurationManager.AppSettings["FSBucketSMS"]));

            return flag;
        }

        public void CopyFolder(int NewApplicationId, int OldApplicationId)
        {
            try
            {
                var _olddata = DbHelper.SelectMethod($"SELECT gov_id_url, companyid_url, billing_url, income_url, other_url, atm_url, signurl FROM tblapplication_record where applicationno={OldApplicationId}");
                if (_olddata != null && _olddata.Rows.Count > 0)
                {
                    string gov_id_url = Convert.ToString(_olddata.Rows[0]["gov_id_url"]);
                    string companyid_url = Convert.ToString(_olddata.Rows[0]["companyid_url"]);
                    string billing_url = Convert.ToString(_olddata.Rows[0]["billing_url"]);
                    string income_url = Convert.ToString(_olddata.Rows[0]["income_url"]);
                    string other_url = Convert.ToString(_olddata.Rows[0]["other_url"]);
                    string atm_url = Convert.ToString(_olddata.Rows[0]["atm_url"]);
                    string signurl = Convert.ToString(_olddata.Rows[0]["signurl"]);
                    logger.CreateFolderFTP($"ApplicationNo{Convert.ToString(NewApplicationId)}");
                    if (!String.IsNullOrWhiteSpace(gov_id_url))
                    {
                        var path = Convert.ToString(_olddata.Rows[0]["gov_id_url"]).Split('/').ToList();
                        if (path != null && path.Count > 0)
                        {
                            logger.CopyImageOnFTP(gov_id_url, Convert.ToString(NewApplicationId), path.Last());
                        }
                    }
                    if (!String.IsNullOrWhiteSpace(companyid_url))
                    {
                        var path = Convert.ToString(_olddata.Rows[0]["companyid_url"]).Split('/').ToList();
                        if (path != null && path.Count > 0)
                        {
                            logger.CopyImageOnFTP(companyid_url, Convert.ToString(NewApplicationId), path.Last());
                        }
                    }
                    if (!String.IsNullOrWhiteSpace(billing_url))
                    {
                        var path = Convert.ToString(_olddata.Rows[0]["billing_url"]).Split('/').ToList();
                        if (path != null && path.Count > 0)
                        {
                            logger.CopyImageOnFTP(billing_url, Convert.ToString(NewApplicationId), path.Last());
                        }
                    }
                    if (!String.IsNullOrWhiteSpace(income_url))
                    {
                        var path = Convert.ToString(_olddata.Rows[0]["income_url"]).Split('/').ToList();
                        if (path != null && path.Count > 0)
                        {
                            logger.CopyImageOnFTP(income_url, Convert.ToString(NewApplicationId), path.Last());
                        }
                    }
                    if (!String.IsNullOrWhiteSpace(other_url))
                    {
                        var path = Convert.ToString(_olddata.Rows[0]["other_url"]).Split('/').ToList();
                        if (path != null && path.Count > 0)
                        {
                            logger.CopyImageOnFTP(other_url, Convert.ToString(NewApplicationId), path.Last());
                        }
                    }
                    if (!String.IsNullOrWhiteSpace(atm_url))
                    {
                        var path = Convert.ToString(_olddata.Rows[0]["atm_url"]).Split('/').ToList();
                        if (path != null && path.Count > 0)
                        {
                            logger.CopyImageOnFTP(atm_url, Convert.ToString(NewApplicationId), path.Last());
                        }
                    }
                    if (!String.IsNullOrWhiteSpace(signurl))
                    {
                        var path = Convert.ToString(_olddata.Rows[0]["signurl"]).Split('/').ToList();
                        if (path != null && path.Count > 0)
                        {
                            logger.CopyImageOnFTP(signurl, Convert.ToString(NewApplicationId), path.Last());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                throw ex;
            }

        }

        public ActionResult AppPayments()
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.Collector))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            List<AppPaymentModel> appPayments = new List<AppPaymentModel>();
            try
            {
                var query = DbHelper.SelectMethod(String.Format(QueryHelper.GetAppPayments));
                if (query != null && query.Rows.Count > 0)
                {
                    foreach (DataRow item in query.Rows)
                    {
                        AppPaymentModel model = new AppPaymentModel
                        {
                            PaymentId = Convert.ToInt32(item["id"]),
                            ApplicationNo = Convert.ToString(item["applicationno"]),
                            ImageUrl = Convert.ToString(item["proof_of_payment"]),
                            ischecked = Convert.ToBoolean(item["ischecked"]),
                            PaidOn = Convert.ToString(item["uploadedon"]),
                            PartnerName = Convert.ToString(item["payment_partner"])
                        };
                        appPayments.Add(model);
                    }
                }
                else
                {
                    appPayments = new List<AppPaymentModel>();
                }
            }
            catch (Exception ex)
            {
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                appPayments = new List<AppPaymentModel>();
            }
            return View(appPayments);
        }
        public ActionResult UpdateAppPayments(int id)
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.Collector))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            try
            {
                DbHelper.SelectMethod(String.Format(QueryHelper.UpdateAppPayment, id));
            }
            catch (Exception ex)
            {
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
            }
            return RedirectToAction("AppPayments");
        }

        [HttpPost]
        public bool SendReloanNotification(string Amount, string OfferAmount, string ApplicationNo)
        {
            List<ReloanData> reloans = new List<ReloanData>();
            double _Offered = 0;
            if (!String.IsNullOrWhiteSpace(Amount))
            {
                reloans.Add(new ReloanData { Amount = Amount });
            }
            if (!String.IsNullOrWhiteSpace(OfferAmount))
            {
                reloans.Add(new ReloanData { Amount = OfferAmount });
                _Offered = Convert.ToDouble(OfferAmount);
            }
            NotificationData notification = new NotificationData
            {
                to = CommonMethods.GetFirebaseToken(ApplicationNo),
                data = new Data
                {
                    Reloan = reloans
                },
                notification = new Notification
                {
                    sound = "default",
                    body = "Congratulations! You have a reloan offer.",
                    title = "Cashmart Reloan"
                }
            };
            DbHelper.InsertUpdateDelete(String.Format(QueryHelper.InsertReloanNotification, ApplicationNo, JsonConvert.SerializeObject(notification), Convert.ToString(HttpContext.Request.Cookies[CookiesKey.UserId].Value)));


            logger.AddWelcomeMessage(History.Application, "", $"Reloan Offered with Amount {Amount} ", _Offered, true, Convert.ToInt32(ApplicationNo));

            logger.SendNotificationFromFirebaseCloud(notification, ApplicationNo);
            return true;
        }
        public ActionResult WindowVerifier(Int64 Id)
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.Collector))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            //string applicationNo = Request.QueryString["Id"];
            //Session["ApplicationNumber"] = applicationNo;
            //int intId = 0;
            //if (!string.IsNullOrWhiteSpace(applicationNo) || applicationNo != null)
            //{
            //    intId = Convert.ToInt32(applicationNo);
            //}
            CompleteAppVerificationDetails completeDetails = new CompleteAppVerificationDetails();
            try
            {
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
                completeDetails.RemarkModelList.AddRange(CommonMethods.GetRemarkList(Convert.ToInt32(Id)));

                if (ActionName != string.Empty)
                {
                    Session["ActionName"] = ActionName;
                }
                if (ActionName == "Index" || ActionName == "KIVBucket")
                {
                    dt = DbHelper.SelectMethod(string.Format(QueryHelper.GetDetailsforApprover, Id));
                }
                else
                {
                    dt = DbHelper.SelectMethod(string.Format(QueryHelper.GetDetailsforApproverFromReVerifier, Id));
                }

                if (dt != null && dt.Rows.Count > 0)
                {
                    completeDetails.jumioreference = Convert.ToString(dt.Rows[0]["jumioreference"]);
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
                    completeDetails.PersonalEmail = dt.Rows[0]["personalemail"].ToString() != string.Empty ? dt.Rows[0]["personalemail"].ToString() : string.Empty;
                    completeDetails.PersonalContactNo = dt.Rows[0]["personalcontactno"].ToString() != string.Empty ? dt.Rows[0]["personalcontactno"].ToString() : string.Empty;
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
                    completeDetails.userid = Convert.ToInt32(dt.Rows[0]["user_id"]); ;


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
                    completeDetails.home_status_text = completeDetails.HomeStatus.FirstOrDefault(x => x.Value == completeDetails.home_status.ToString()).Text;
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
                    dt = DbHelper.SelectMethod(string.Format(QueryHelper.GetApplicationdetails, Id));
                    if (dt != null && dt.Rows.Count > 0)
                    {
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
                completeDetails.ApprovedLoanAmount = completeDetails.Loan_Amount;
                completeDetails.family_contact1 = completeDetails.RelativeContactNo;
                completeDetails.optional_contact = completeDetails.CoworkerContactNo;
                completeDetails.family_name1 = completeDetails.RelativeName;
                completeDetails.optional_name = completeDetails.CoWorkerName;

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
        [HttpGet]
        //To View the Record For Approval
        public ActionResult ViewDetailsForApproval(int id, string ActionName)
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.Collector))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            CompleteAppVerificationDetails appRecordVM = new CompleteAppVerificationDetails();
            try
            {
                Session["ApplicationId"] = id;

                List<ApplicationRecordVM> Record = new List<ApplicationRecordVM>();
                DataTable dt;
                DataTable dtBank = new DataTable();
                DataTable dtOccupation = new DataTable();
                double a = 0d;
                appRecordVM.view = 0;

                dt = DbHelper.SelectMethod(QueryHelper.GetApplicationforApprover);
                if (dt != null && dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        Record.Add(new ApplicationRecordVM
                        {
                            Id = 1 + Convert.ToInt32(i),
                            ApplicationNo = Convert.ToInt32(dt.Rows[i]["applicationno"].ToString()),
                            First_Name = dt.Rows[i]["first_name"].ToString(),
                            Middle_Name = dt.Rows[i]["middle_name"].ToString(),
                            Last_Name = dt.Rows[i]["last_name"].ToString(),
                            DateApplied = dt.Rows[i]["dateapplied"].ToString(),
                            GrossIncome = dt.Rows[i]["gross_income"].ToString() == string.Empty ? a : Convert.ToDouble(dt.Rows[i]["gross_income"]),
                            Loan_Amount = dt.Rows[i]["approved_loan_amount"].ToString() == string.Empty ? 0 : Convert.ToInt32(dt.Rows[i]["approved_loan_amount"]),
                            IsPickedApprover = Convert.ToBoolean(dt.Rows[i]["ispickedapprover"]),
                            username = Convert.ToString(dt.Rows[i]["username"]),
                            user_id = Convert.ToInt32(dt.Rows[i]["user_id"].ToString())
                        });
                    }
                }

                if (id > 0)
                {
                    try
                    {
                        appRecordVM = approverCommonOperation.GetApproverData(id);
                        appRecordVM.RemarkModelList.AddRange(CommonMethods.GetRemarkList(id));
                    }
                    catch (Exception ex)
                    {
                        logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                        TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                    }
                }
                dtBank = DbHelper.SelectMethod(QueryHelper.GetAllBank);
                if (dtBank != null && dtBank.Rows.Count > 0)
                {
                    for (int i = 0; i < dtBank.Rows.Count; i++)
                    {
                        appRecordVM.BankMasterModelList.Add(new BankMasterModel
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
                        appRecordVM.OccupationModelList.Add(new OccupationModel
                        {
                            id = Convert.ToInt32(dtOccupation.Rows[i]["id"]),
                            occupation_name = Convert.ToString(dtOccupation.Rows[i]["occupation_name"]),
                            isactive = Convert.ToBoolean(dtOccupation.Rows[i]["isactive"]),
                            created_on = Convert.ToString(dtOccupation.Rows[i]["created_on"])
                        });
                    }
                }
                appRecordVM.BackActionName = ActionName;
            }
            catch (Exception ex)
            {
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
            }

            return View(appRecordVM);
        }
    }
}


