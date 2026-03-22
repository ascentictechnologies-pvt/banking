using Loan_CRM.Areas.Account.Models;
using Loan_CRM.Areas.Checker.Models;
using Loan_CRM.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace Loan_CRM.Areas.Account.Controllers
{
    public class SummeryDetailController : Controller
    {
        readonly CommonOperation _commonOperation = new CommonOperation();

        /// <summary>
        ///  // GET: Account/SummeryDetail
        /// </summary>
        /// <param name="applicationno"></param>
        /// <returns></returns>
        public ActionResult AccountSummery()
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.Accounts))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            return View();
        }

        /// <summary>
        /// EmiInfo
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="_DisbursementDate"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult EmiInfo(int Id, string _DisbursementDate)
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.Accounts))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            ActivityLog.Info($"EMI Info for Application {Id} Summary Details on Account Window by User {Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value)}", $"{this.GetType().Name} ==> {System.Reflection.MethodBase.GetCurrentMethod().Name}");
            ViewBag.EMIFlag = false;
            Session["ApplicationNo"] = Id;
            TempData["Success"] = null;
            EmiInformation emilist = new EmiInformation();
            ViewBag.govidurl = String.Empty;
            ViewBag.companyidurl = String.Empty;
            ViewBag.billingurl = String.Empty;
            ViewBag.incom_url = String.Empty;
            ViewBag.other_url = String.Empty;
            ViewBag.contract_url = "../../../Content/PDF/";
            CommonOperation objmethod = new CommonOperation();
            try
            {
                emilist = objmethod.GetEmiListbyID(Id);
                var govermentidurl = emilist.gov_id_url;
                var conpanyid = emilist.companyid_url;
                var billing = emilist.billing_url;
                var income = emilist.income_url;
                var other = emilist.other_url;
                var ATM = emilist.atm_url;
                var contracturl = emilist.contract_ref_no;
                if (contracturl != null && contracturl != "")
                {
                    contracturl += ".pdf";
                }
                var customerName = emilist.ApplicantName;
                DateTime DisbursementDate;
                if (_DisbursementDate != null && _DisbursementDate.Length > 2)
                {
                    emilist.disbursement_date = Convert.ToString(_DisbursementDate);
                }

                var DataSplit = emilist.disbursement_date.Split('-').ToList();
                DateTime dateTime = new DateTime(Convert.ToInt32(DataSplit[2]), Convert.ToInt32(DataSplit[1]), Convert.ToInt32(DataSplit[0]));
                DisbursementDate = dateTime;

                Session["TermType"] = emilist.term_type;
                Session["Term"] = emilist.term;
                DataTable dtLoanAmt = new DataTable();
                dtLoanAmt = DbHelper.SelectMethod(string.Format("Select a.applicationno, a.loanamount, b.approved_loan_amount,b.approved_term, b.approved_interest_rate, b.approved_total_amount_due, a.user_id from tblapplication_record a join tblapplication_personal_verification_details b on a.applicationno=b.application_no where a.applicationno = '{0}'", Id));
                double RequestedLoanAmt = 0.00;
                double ApprovedLoanAmt = 0.00;
                double ApprovedLoanAmt_withRate = 0.00;
                double ApprovedInterestRate = 0.00;
                double ApprovedTerm = 0.00;
                if (dtLoanAmt != null && dtLoanAmt.Rows.Count > 0)
                {
                    RequestedLoanAmt = Convert.ToString(dtLoanAmt.Rows[0]["loanamount"]) == "" ? 0.00 : Convert.ToDouble(dtLoanAmt.Rows[0]["loanamount"]);
                    ApprovedLoanAmt = Convert.ToString(dtLoanAmt.Rows[0]["approved_loan_amount"]) == "" ? 0.00 : Convert.ToDouble(dtLoanAmt.Rows[0]["approved_loan_amount"]);
                    ApprovedLoanAmt_withRate = Convert.ToString(dtLoanAmt.Rows[0]["approved_total_amount_due"]) == "" ? 0.00 : Convert.ToDouble(dtLoanAmt.Rows[0]["approved_total_amount_due"]);
                    ApprovedInterestRate = Convert.ToString(dtLoanAmt.Rows[0]["approved_interest_rate"]) == "" ? 0.00 : Convert.ToDouble(dtLoanAmt.Rows[0]["approved_interest_rate"]);
                    ApprovedTerm = Convert.ToString(dtLoanAmt.Rows[0]["approved_term"]) == "" ? 0.00 : Convert.ToDouble(dtLoanAmt.Rows[0]["approved_term"]);
                    emilist.user_id = Convert.ToString(dtLoanAmt.Rows[0]["user_id"]) == "" ? 0 : Convert.ToInt32(dtLoanAmt.Rows[0]["user_id"]);
                }

                if (emilist.Records.Count == 0)
                {
                    emilist.Records = EmiRowsByDisbursmentDate(emilist.term_name, emilist.Records.Count, DisbursementDate, ApprovedLoanAmt, ApprovedLoanAmt_withRate, ApprovedTerm);
                }
                else
                {
                    ViewBag.EMIFlag = true;
                    emilist.Records = EmiRowsByDisbursmentDate(emilist.term_name, emilist.Records.Count, DisbursementDate, ApprovedLoanAmt, ApprovedLoanAmt_withRate, ApprovedTerm);
                }
                ViewBag.govidurl += govermentidurl;
                ViewBag.companyidurl += conpanyid;
                ViewBag.billingurl += billing;
                ViewBag.incom_url += income;
                ViewBag.other_url += other;
                ViewBag.contract_url += contracturl;
                ViewBag.ApprovedLoan = ApprovedLoanAmt;

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
            return View(emilist);
        }

        private List<TermsRecord> EmiRows(string term_name, int Records_Count)
        {
            try
            {
                List<TermsRecord> de = new List<TermsRecord>();
                string s = term_name;
                if (s != null)
                {
                    int TermRows = Convert.ToInt32(s.Substring(0, 1));
                    ViewBag.rowcount = Records_Count;
                    if (Records_Count == 0)
                    {
                        for (int i = 0; i < TermRows; i++)
                        {
                            de.Add(new TermsRecord());
                        }
                    }
                    ViewBag.TermRows = TermRows;
                }
                return de;
            }
            catch (Exception ex)
            {
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                throw ex;
            }

        }

        private List<TermsRecord> EmiRowsByDisbursmentDate(string term_name, int Records_Count, DateTime DisbursementDate, double ApprovedLoanAmt, double LoanAmtWIthRate, double ApprovedTerm)
        {
            try
            {
                logger.WriteErrorLogs($"term_name : {term_name}, Records_Count : {Records_Count}, DisbursementDate : {DisbursementDate}, ApprovedLoanAmt : {ApprovedLoanAmt}, LoanAmtWIthRate : {LoanAmtWIthRate}, ApprovedTerm : {ApprovedTerm}", $"{this.GetType().Name} ==> {System.Reflection.MethodBase.GetCurrentMethod().Name}");
                List<TermsRecord> de = new List<TermsRecord>();
                if (!String.IsNullOrWhiteSpace(term_name))
                {
                    int TermRows = Convert.ToInt32(term_name.Split(' ').FirstOrDefault());
                    ViewBag.rowcount = Records_Count;
                    if (Records_Count == 0)
                    {
                        for (int i = 0; i < TermRows; i++)
                        {
                            DisbursementDate = DisbursementDate.AddDays(ApprovedTerm / TermRows);
                            string _fourthweekemiamount = Convert.ToString(Math.Round((LoanAmtWIthRate / TermRows), 2));
                            string _fourthweekprinciple = Convert.ToString(Math.Round((ApprovedLoanAmt / TermRows), 2));
                            string _Rate = Convert.ToString(Math.Round((Convert.ToDouble(_fourthweekemiamount) - Convert.ToDouble(_fourthweekprinciple)), 2));
                            de.Add(new TermsRecord
                            {
                                fourthweekdate = Convert.ToString(DisbursementDate.ToString("dd-MM-yyyy")),
                                fourthweekemiamount = _fourthweekemiamount,
                                fourthweekprinciple = _fourthweekprinciple,
                                fourthweekrate = _Rate
                            });
                        }
                    }
                    else
                    {
                        for (int i = 0; i < TermRows; i++)
                        {
                            DisbursementDate = DisbursementDate.AddDays(ApprovedTerm / TermRows);
                            string _fourthweekemiamount = Convert.ToString(Math.Round((LoanAmtWIthRate / TermRows), 2));
                            string _fourthweekprinciple = Convert.ToString(Math.Round((ApprovedLoanAmt / TermRows), 2));
                            string _Rate = Convert.ToString(Math.Round((Convert.ToDouble(_fourthweekemiamount) - Convert.ToDouble(_fourthweekprinciple)), 2));
                            de.Add(new TermsRecord
                            {
                                fourthweekdate = Convert.ToString(DisbursementDate.ToString("dd-MM-yyyy")),
                                fourthweekemiamount = _fourthweekemiamount,
                                fourthweekprinciple = _fourthweekprinciple,
                                fourthweekrate = _Rate
                            });
                        }
                    }
                    ViewBag.TermRows = TermRows;
                }
                return de;
            }
            catch (Exception ex)
            {
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                throw ex;
            }

        }

        [HttpPost]
        public ActionResult AddEmiInfo(EmiInformation emilist)
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.Accounts))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            try
            {
                ActivityLog.Info($"EMI Info for Application {emilist.applicationno} Summary Details on Account Window by User {Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value)}", $"{this.GetType().Name} ==> {System.Reflection.MethodBase.GetCurrentMethod().Name}");

                CommonOperation objmethod = new CommonOperation();
                emilist.term_type = Convert.ToString(Session["TermType"]);
                emilist.applicationno = Convert.ToInt32(Session["ApplicationNo"]);
                int UserId = 0;
                double loanamount = Convert.ToDouble(emilist.ApprovedLoanAmt);
                UserId = Convert.ToInt32(Request.Cookies[CookiesKey.UserId].Value);
                if (!String.IsNullOrWhiteSpace(emilist.new_referenceno))
                {
                    if (Convert.ToString(emilist.new_referenceno).Trim() != Convert.ToString(emilist.referenceno))
                    {
                        if (!String.IsNullOrWhiteSpace(emilist.referenceno))
                        {
                            string query1 = (string.Format(QueryHelper.InsertNewReference, emilist.applicationno, emilist.referenceno, emilist.new_referenceno, UserId));
                            var result = DbHelper.InsertUpdateDelete(query1);
                        }
                    }
                }
                UpdateIsPickedAccount(Convert.ToString(emilist.applicationno));
                objmethod.InsertEmiinfo(emilist, loanamount);
                var DataSplit = emilist.disbursement_date.Split('-').ToList();
                DateTime dateTime = new DateTime(Convert.ToInt32(DataSplit[0]), Convert.ToInt32(DataSplit[1]), Convert.ToInt32(DataSplit[2]));
                var date = dateTime.ToString("dd-MM-yyyy");

                #region if Reloan or Preterm taken on this application then Assign to Same Collector

                DataTable dataTable = new DataTable();
                int assignedAgentId = 0;
                int newEmiId = 0;
                int applicationUserId = 0;
                DataTable dataTableUser = new DataTable();
                dataTableUser = DbHelper.SelectMethod(string.Format(QueryHelper.GetUserIdByApplicationNo, emilist.applicationno));
                if (dataTableUser != null && dataTableUser.Rows.Count > 0)
                {
                    applicationUserId = Convert.ToInt32(dataTableUser.Rows[0]["user_id"]);
                    loanamount = Convert.ToDouble(dataTableUser.Rows[0]["loanamount"] == DBNull.Value ? 0 : dataTableUser.Rows[0]["loanamount"]);
                }
                List<int> collectors = new List<int>();
                var _collecter = DbHelper.SelectMethod($"Select cu.userid, cu.username, cu.userpass,cu.webadminrole,cr.id, cr.rolename from  ct_user cu join ct_roles cr on cu.webadminrole=cr.id where lower(cr.rolename)='{RoleType.Collector}' order by userid;");
                if (_collecter != null && _collecter.Rows.Count > 0)
                {
                    foreach (DataRow item in _collecter.Rows)
                    {
                        collectors.Add(Convert.ToInt32(item["userid"]));
                    }
                }

                dataTable = DbHelper.SelectMethod(string.Format(QueryHelper.GetAgentIdForReloanAndPreterm, applicationUserId));
                if (dataTable != null && dataTable.Rows.Count > 0)
                {
                    assignedAgentId = dataTable.Rows[0]["agent_id"] != null ? Convert.ToInt32(dataTable.Rows[0]["agent_id"]) : 0;
                    if (assignedAgentId != 0)
                    {
                        #region Assign EMI to the same Agent

                        var query = DbHelper.SelectMethod(string.Format(QueryHelper.GetEmiID, emilist.applicationno));
                        if (query != null && query.Rows.Count > 0)
                        {
                            newEmiId = Convert.ToInt32(query.Rows[0]["emi_id"]);
                        }
                        try
                        {
                            DbHelper.InsertUpdateDelete(string.Format(QueryHelper.InsertCollectionMapping, newEmiId, assignedAgentId));
                        }
                        catch (Exception ex)
                        {
                            logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                        }
                        #endregion
                    }
                    else
                    {
                        var query = DbHelper.SelectMethod(string.Format(QueryHelper.GetEmiID, emilist.applicationno));
                        if (query != null && query.Rows.Count > 0)
                        {
                            newEmiId = Convert.ToInt32(query.Rows[0]["emi_id"]);
                        }
                        try
                        {
                             DbHelper.InsertUpdateDelete(string.Format(QueryHelper.InsertCollectionMapping, newEmiId, _commonOperation.GetCollectionAgentId(collectors)));
                        }
                        catch (Exception ex)
                        {
                            logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                        }
                    }
                }
                else
                {
                    var query = DbHelper.SelectMethod(string.Format(QueryHelper.GetEmiID, emilist.applicationno));
                    if (query != null && query.Rows.Count > 0)
                    {
                        newEmiId = Convert.ToInt32(query.Rows[0]["emi_id"]);
                    }
                    try
                    {
                         DbHelper.InsertUpdateDelete(string.Format(QueryHelper.InsertCollectionMapping, newEmiId, _commonOperation.GetCollectionAgentId(collectors)));
                    }
                    catch (Exception ex)
                    {
                        logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                    }
                }
                #endregion

                return Redirect("EmiInfo?Id=" + emilist.applicationno + "&_DisbursementDate=" + date + "");
            }
            catch (Exception ex)
            {
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                return Redirect("EmiInfo?Id=" + emilist.applicationno + "&_DisbursementDate=" + DateTime.Now.ToString("dd-MM-yyyy") + "");
            }
        }

        /// <summary>
        /// click get record button then this function will call
        /// </summary>
        /// <param name="Fromdate"></param>
        /// <param name="ToDate"></param>
        /// <param name="appNo"></param>
        /// <param name="contractNo"></param>
        /// <param name="clientName"></param>
        /// <returns></returns>
        public ActionResult GetUsersData(string Fromdate, string ToDate)
        {
            try
            {
                CommonOperation comnoperation1 = new CommonOperation();
                var users = comnoperation1.GetEmiNotCreated(Fromdate, ToDate);
                var jsonResult = Json(users, JsonRequestBehavior.AllowGet);
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

        /// <summary>
        /// Export to excel button on AccountSummery
        /// </summary>
        /// <param name="viewName"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        protected string RenderPartialViewToString(string viewName, object model)
        {
            try
            {
                if (string.IsNullOrEmpty(viewName))
                    viewName = ControllerContext.RouteData.GetRequiredString("action");

                ViewData.Model = model;

                using (StringWriter sw = new StringWriter())
                {
                    ViewEngineResult viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                    ViewContext viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
                    viewResult.View.Render(viewContext, sw);
                    return sw.GetStringBuilder().ToString();
                }

            }
            catch (Exception ex)
            {
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                throw ex;
            }
        }

        public void GetExcel()
        {
            try
            {
                CommonOperation objmethod = new CommonOperation();
                List<ProfileSummery> profilesummeryList1 = objmethod.GetPortfolioList();
                string excelstring = RenderPartialViewToString("_ExcelExport", profilesummeryList1);
                Response.ClearContent();
                Response.AddHeader("content-disposition", "attachment; filename=LoanDetail.xls");
                Response.ContentType = "application/excel";
                Response.Write(excelstring);
                Response.End();
            }
            catch (Exception ex)
            {
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                throw ex;
            }


        }

        //function not used!
        /// <summary>
        /// get pending payment
        /// </summary>
        /// <param name="appNo"></param>
        /// <returns></returns>
        public ActionResult GetPendingPaymentInfo(int appNo)
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.Accounts))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            object _obj;
            bool _response;
            try
            {
                _obj = _commonOperation.GetPendingEmi(appNo);
                _response = true;
            }
            catch (Exception ex)
            {
                _obj = null;
                _response = false;
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
            }
            var result = new { obj = _obj, response = _response };
            var jsonResult = Json(result, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        /// <summary>
        /// Delete PendingPayment
        /// </summary>
        /// <param name="penaltyAmount"></param>
        /// <param name="remark"></param>
        /// <param name="payId"></param>
        /// <param name="EmiPayment"></param>
        /// <returns></returns>
        public ActionResult DeletePendingPayment(decimal penaltyAmount, string remark, int payId, int EmiPayment)
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.Accounts))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            bool result = false;
            try
            {
                ActivityLog.Info($"Payment {payId} Deleted from Summary Details on Account Window by User {Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value)}", $"{this.GetType().Name} ==> {System.Reflection.MethodBase.GetCurrentMethod().Name}");
                result = _commonOperation.DeletePendingPayment(penaltyAmount, remark, payId, EmiPayment);
            }
            catch (Exception ex)
            {
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
            }
            var jsonResult = Json(result, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        /// <summary>
        /// Approve PendingPayment
        /// </summary>
        /// <param name="paidAmt"></param>
        /// <param name="penaltyAmount"></param>
        /// <param name="remark"></param>
        /// <param name="payId"></param>
        /// <param name="appNo"></param>
        /// <param name="EmiPayment"></param>
        /// <returns></returns>
        public ActionResult ApprovePendingPayment(decimal paidAmt, decimal penaltyAmount, string remark, int payId, int appNo, int EmiPayment, int user_id, string PaymentChannel, string ContractNumber)
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.Accounts))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            bool result = false;
            try
            {
                ActivityLog.Info($"Payment {payId} Approved from Summary Details on Account Window by User {Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value)}", $"{this.GetType().Name} ==> {System.Reflection.MethodBase.GetCurrentMethod().Name}");
                result = _commonOperation.ApprovePayment(paidAmt, penaltyAmount, remark, payId, appNo, EmiPayment);
                var _Data = _commonOperation.GetOutstandingAmount(Convert.ToString(appNo));
                logger.AddWelcomeMessage(History.Paid, ContractNumber, "Thank you. Your payment was received.", Convert.ToDouble(paidAmt + penaltyAmount), false, appNo, PaymentChannel);
            }
            catch (Exception ex)
            {
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
            }
            var jsonResult = Json(result, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        //function not used
        public ActionResult UpdatePendingPayment(decimal penaltyAmount, string remark, int payId)
        {

            try
            {
                var jsonResult = Json(_commonOperation.UpdatePendingPayments(penaltyAmount, remark, payId), JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
            catch (Exception ex)
            {
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                return Json(false);
            }
        }

        public bool UpdateIsPickedAccount(string appNo)
        {
            try
            {
                bool response = false;
                int intAppNo = 0;
                try
                {
                    if (!string.IsNullOrWhiteSpace(appNo))
                    {
                        intAppNo = Convert.ToInt32(appNo);
                    }
                    int query = DbHelper.InsertUpdateDelete(string.Format(QueryHelper.UpdateIsPickedAccount, intAppNo));
                    if (query > 0)
                    {
                        response = true;
                    }
                }
                catch (Exception ex)
                {
                    response = false;
                    logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                }
                return response;
            }
            catch (Exception ex)
            {
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                throw ex;
            }

        }

        public ActionResult GetApprovedPendingPaymentInfo(int appNo)
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.Accounts))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            List<EmiInformation> emiInfo = new List<EmiInformation>();
            try
            {
                var query = DbHelper.SelectMethod(string.Format(QueryHelper.GetApprovedPendingPaymentsInfo, appNo));
                if (query.Rows.Count > 0)
                {
                    for (int i = 0; i < query.Rows.Count; i++)
                    {
                        EmiInformation emiInfoVM = new EmiInformation()
                        {
                            applicationno = Convert.ToString(query.Rows[i]["applicationno"]) != "" ? Convert.ToInt32(query.Rows[i]["applicationno"]) : 0,
                            ApplicantName = Convert.ToString(query.Rows[i]["first_name"]) + " " + Convert.ToString(query.Rows[i]["middle_name"]) + " " + Convert.ToString(query.Rows[i]["last_name"]),
                            referenceno = Convert.ToString(query.Rows[i]["reference_no"]),
                            personalcontactnoone = Convert.ToString(query.Rows[i]["personalcontactno"]),
                            DateOfPayment = Convert.ToString(query.Rows[i]["paid_on"]),
                            ProofOfPayment = Convert.ToString(query.Rows[i]["proofofpayment"]),
                            PaymentChannel = Convert.ToString(query.Rows[i]["whatpaymentchennel"]),
                            PaidAmount = Convert.ToString(query.Rows[i]["paid_amount"]) != "" ? Convert.ToDecimal(query.Rows[i]["paid_amount"]) : 0,
                            PaymentId = Convert.ToString(query.Rows[i]["payment_id"]) != "" ? Convert.ToInt32(query.Rows[i]["payment_id"]) : 0,
                            LatePenalty = Convert.ToString(query.Rows[i]["penalty_amount"]) != "" ? Convert.ToDecimal(query.Rows[i]["penalty_amount"]) : 0,
                            Remarks = Convert.ToString(query.Rows[i]["remarks"])
                        };
                        emiInfo.Add(emiInfoVM);
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                emiInfo = null;
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
            }
            var jsonResult = Json(emiInfo, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        //function not used
        [HttpPost]
        public ActionResult GetFilterData()
        {
            return Json("", JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// create pending payment
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult PendingPayments()
        {
            //---Changes
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.Accounts))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            DisbursementModel model = new DisbursementModel();
            return View(model);
        }

        public ActionResult GetPendingPayments()
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.Accounts))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            List<DisbursementModelDetails> model = new List<DisbursementModelDetails>();
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

                DataTable dt = DbHelper.SelectMethod(string.Format(QueryHelper.GetPendingPaymentsData));

                try
                {
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            DisbursementModelDetails _DisbursementModelDetails = new DisbursementModelDetails
                            {
                                ApplicationNo = DBNull.Value.Equals(dt.Rows[i]["applicationno"]) ? 0 : Convert.ToInt32(dt.Rows[i]["applicationno"]),
                                Name = $"{(Convert.ToString(dt.Rows[i]["first_name"]) != string.Empty ? Convert.ToString(dt.Rows[i]["first_name"]) : string.Empty)} {(Convert.ToString(dt.Rows[i]["middle_name"]) != string.Empty ? Convert.ToString(dt.Rows[i]["middle_name"]) : string.Empty)} {(Convert.ToString(dt.Rows[i]["last_name"]) != string.Empty ? Convert.ToString(dt.Rows[i]["last_name"]) : string.Empty)}",
                                First_Name = Convert.ToString(dt.Rows[i]["first_name"]) != string.Empty ? Convert.ToString(dt.Rows[i]["first_name"]) : string.Empty,
                                ContractNumber = DBNull.Value.Equals(dt.Rows[i]["reference_no"]) ? "" : Convert.ToString(dt.Rows[i]["reference_no"]) != string.Empty ? Convert.ToString(dt.Rows[i]["reference_no"]) : string.Empty,
                                ReferenceNumber = DBNull.Value.Equals(dt.Rows[i]["contract_ref_no"]) ? "" : Convert.ToString(dt.Rows[i]["contract_ref_no"]) != string.Empty ? Convert.ToString(dt.Rows[i]["contract_ref_no"]) : string.Empty,
                                PaymentDate = DBNull.Value.Equals(dt.Rows[i]["paid_on"]) ? DateTime.Now : Convert.ToString(dt.Rows[i]["paid_on"]) != string.Empty ? Convert.ToDateTime(dt.Rows[i]["paid_on"]) : DateTime.Now,
                                DateofPayment = DBNull.Value.Equals(dt.Rows[i]["paid_on"]) ? DateTime.Now.ToString("dd-MM-yyyy") : Convert.ToString(dt.Rows[i]["paid_on"]) != string.Empty ? Convert.ToDateTime(dt.Rows[i]["paid_on"]).ToString("dd-MM-yyyy") : DateTime.Now.ToString("dd-MM-yyyy"),
                                LatePenalties = DBNull.Value.Equals(dt.Rows[i]["penalty_amount"]) ? "" : Convert.ToString(dt.Rows[i]["penalty_amount"]) != string.Empty ? Convert.ToString(dt.Rows[i]["penalty_amount"]) : string.Empty,
                                PaymentChannel = DBNull.Value.Equals(dt.Rows[i]["whatpaymentchennel"]) ? "" : Convert.ToString(dt.Rows[i]["whatpaymentchennel"]) != string.Empty ? Convert.ToString(dt.Rows[i]["whatpaymentchennel"]) : string.Empty,
                                remarks = DBNull.Value.Equals(dt.Rows[i]["remarks"]) ? "" : Convert.ToString(dt.Rows[i]["remarks"]) != string.Empty ? Convert.ToString(dt.Rows[i]["remarks"]) : string.Empty,
                                proofofpayment = DBNull.Value.Equals(dt.Rows[i]["proofofpayment"]) ? "" : Convert.ToString(dt.Rows[i]["proofofpayment"]) != string.Empty ? Convert.ToString(dt.Rows[i]["proofofpayment"]) : string.Empty,
                                Amortization = DBNull.Value.Equals(dt.Rows[i]["paid_amount"]) ? 0 : Convert.ToString(dt.Rows[i]["paid_amount"]) != string.Empty ? Convert.ToDecimal(dt.Rows[i]["paid_amount"]) : 0,
                                TotalAmount = DBNull.Value.Equals(dt.Rows[i]["totalamt"]) ? "" : Convert.ToString(dt.Rows[i]["totalamt"]) != string.Empty ? Convert.ToString(dt.Rows[i]["totalamt"]) : string.Empty,
                                PaymentId = DBNull.Value.Equals(dt.Rows[i]["payment_id"]) ? 0 : Convert.ToString(dt.Rows[i]["payment_id"]) != "" ? Convert.ToInt32(dt.Rows[i]["payment_id"]) : 0,
                                EmiPayment = DBNull.Value.Equals(dt.Rows[i]["emi_payment"]) ? 0 : Convert.ToInt32(dt.Rows[i]["emi_payment"]),
                                user_id = DBNull.Value.Equals(dt.Rows[i]["user_id"]) ? 0 : Convert.ToInt32(dt.Rows[i]["user_id"]),
                            };
                            var _remarks = DbHelper.SelectMethod(string.Format(QueryHelper.GetRemarkByApplicationId, Convert.ToString(_DisbursementModelDetails.ApplicationNo)));
                            if (_remarks != null && _remarks.Rows.Count > 0)
                            {
                                String _Remark = String.Empty;
                                foreach (DataRow _remarksitem in _remarks.Rows)
                                {
                                    _Remark += $"{_remarksitem["createdbyname"]} ({_remarksitem["remarkidentifier"]}) :{_remarksitem["remark"]}\n";
                                }
                                _DisbursementModelDetails.checker_remark = _Remark;
                            }
                            model.Add(_DisbursementModelDetails);
                        }
                    }
                }

                catch (Exception ex)
                {
                    TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                    logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                }


                int totalRecords = model.Count;
                if (!string.IsNullOrEmpty(search) &&
                !string.IsNullOrWhiteSpace(search))
                {
                    // Apply search 
                    model = model.Where(p => p.ReferenceNumber.ToString().ToLower().Contains(search.ToLower()) ||
                    p.ContractNumber.ToString().ToLower().Contains(search.ToLower()) ||
                     p.Name.ToString().ToLower().Contains(search.ToLower()) ||
                      p.PaymentDate.ToString().ToLower().Contains(search.ToLower()) ||
                       p.Amortization.ToString().ToLower().Contains(search.ToLower()) ||
                        p.LatePenalties.ToString().ToLower().Contains(search.ToLower()) ||
                         p.TotalAmount.ToString().ToLower().Contains(search.ToLower()) ||
                          p.PaymentChannel.ToString().ToLower().Contains(search.ToLower()) ||
                           p.ApplicationNo.ToString().ToLower().Contains(search.ToLower()) ||
                    p.proofofpayment.ToString().ToLower().Contains(search.ToLower())).ToList();
                }

                model = this.SortByColumnWithOrder(order, orderDir, model);
                int recFilter = model.Count;
                model = model.Skip(start).Take(length).ToList<DisbursementModelDetails>();
                result = this.Json(new { draw = Convert.ToInt32(draw), recordsTotal = totalRecords, recordsFiltered = recFilter, data = model }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
            }
            return result;
        }
        private List<DisbursementModelDetails> SortByColumnWithOrder(string order, string orderDir, List<DisbursementModelDetails> ApplicationRecordList)
        {
            List<DisbursementModelDetails> lst = new List<DisbursementModelDetails>();
            try
            {
                switch (order)
                {
                    case "0":
                        lst = orderDir.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? ApplicationRecordList.OrderByDescending(p => p.ReferenceNumber).ToList() : ApplicationRecordList.OrderBy(p => p.ReferenceNumber).ToList();
                        break;
                    case "1":
                        lst = orderDir.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? ApplicationRecordList.OrderByDescending(p => p.ContractNumber).ToList() : ApplicationRecordList.OrderBy(p => p.ContractNumber).ToList();
                        break;
                    case "2":
                        lst = orderDir.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? ApplicationRecordList.OrderByDescending(p => p.Name).ToList() : ApplicationRecordList.OrderBy(p => p.Name).ToList();
                        break;
                    case "3":
                        lst = orderDir.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? ApplicationRecordList.OrderByDescending(p => p.PaymentDate).ToList() : ApplicationRecordList.OrderBy(p => p.PaymentDate).ToList();
                        break;
                    case "4":
                        lst = orderDir.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? ApplicationRecordList.OrderByDescending(p => p.Amortization).ToList() : ApplicationRecordList.OrderBy(p => p.Amortization).ToList();
                        break;
                    case "5":
                        lst = orderDir.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? ApplicationRecordList.OrderByDescending(p => p.LatePenalties).ToList() : ApplicationRecordList.OrderBy(p => p.LatePenalties).ToList();
                        break;
                    case "6":
                        lst = orderDir.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? ApplicationRecordList.OrderByDescending(p => p.TotalAmount).ToList() : ApplicationRecordList.OrderBy(p => p.TotalAmount).ToList();
                        break;
                    case "7":
                        lst = orderDir.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? ApplicationRecordList.OrderByDescending(p => p.PaymentChannel).ToList() : ApplicationRecordList.OrderBy(p => p.PaymentChannel).ToList();
                        break;
                    case "8":
                        lst = orderDir.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? ApplicationRecordList.OrderByDescending(p => p.proofofpayment).ToList() : ApplicationRecordList.OrderBy(p => p.proofofpayment).ToList();
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
        public ActionResult DoReject(int id)
        {
           
            Session["ApplicationId"] = id;
            bool result = false;
            try
            {
                ActivityLog.Info($"Payment {id} Deleted from Summary Details on Account Window by User {Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value)}", $"{this.GetType().Name} ==> {System.Reflection.MethodBase.GetCurrentMethod().Name}");
                int i = DbHelper.InsertUpdateDelete(string.Format(QueryHelper.RejectedByAccount, id));
                if (i > 0)
                {
                    result = true;
                }
                else
                {
                    result = false;
                }
            }
            catch (Exception ex)
            {

                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
            }
            var jsonResult = Json(result, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpPost]
        public ActionResult DoApprove(int id)
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.Accounts))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            Session["ApplicationId"] = id;
            bool result = false;
            try
            {
                ActivityLog.Info($"Payment {id} Approve from Summary Details on Account Window by User {Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value)}", $"{this.GetType().Name} ==> {System.Reflection.MethodBase.GetCurrentMethod().Name}");
                List<Parameters> parameters = new List<Parameters>();
                parameters.Add(new Parameters() { ParameterName = "applicationno", ParameterValue = id, DbType = NpgsqlTypes.NpgsqlDbType.Varchar });
                int i = DbHelper.InsertUpdateDelete(QueryHelper.ApprovedByAccount, parameters);
                if (i > 0)
                {
                    result = true;
                }
                else
                {
                    result = false;
                }
            }
            catch (Exception ex)
            {
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                //result = false;
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
            }
            var jsonResult = Json(result, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        //function not used
        /// <summary>
        /// get emi data
        /// </summary>
        /// <param name="id"></param>
        /// <param name="DisbursementDate"></param>
        /// <returns></returns>
        public ActionResult GetEmiData(int id, string DisbursementDate)
        {
           
            try
            {
                Session["ApplicationId"] = id;
                CommonOperation objmethod = new CommonOperation();
                EmiInformation emilist = objmethod.GetEmiListbyID(id);
                DataTable dtLoanAmt = new DataTable();
                List<Parameters> parameters = new List<Parameters>();
                parameters.Add(new Parameters() { ParameterName = "applicationno", ParameterValue = id, DbType = NpgsqlTypes.NpgsqlDbType.Varchar });
                dtLoanAmt = DbHelper.SelectMethod(QueryHelper.GetEmiDetailsbyApplicationNo, parameters);
                double RequestedLoanAmt = 0.00;
                double ApprovedLoanAmt = 0.00;
                double ApprovedLoanAmt_withRate = 0.00;
                double ApprovedInterestRate = 0.00;
                double ApprovedTerm = 0.00;
                if (dtLoanAmt != null && dtLoanAmt.Rows.Count > 0)
                {
                    RequestedLoanAmt = Convert.ToString(dtLoanAmt.Rows[0]["loanamount"]) == "" ? 0.00 : Convert.ToDouble(dtLoanAmt.Rows[0]["loanamount"]);
                    ApprovedLoanAmt = Convert.ToString(dtLoanAmt.Rows[0]["approved_loan_amount"]) == "" ? 0.00 : Convert.ToDouble(dtLoanAmt.Rows[0]["approved_loan_amount"]);
                    ApprovedLoanAmt_withRate = Convert.ToString(dtLoanAmt.Rows[0]["approved_total_amount_due"]) == "" ? 0.00 : Convert.ToDouble(dtLoanAmt.Rows[0]["approved_total_amount_due"]);
                    ApprovedInterestRate = Convert.ToString(dtLoanAmt.Rows[0]["approved_interest_rate"]) == "" ? 0.00 : Convert.ToDouble(dtLoanAmt.Rows[0]["approved_interest_rate"]);
                    ApprovedTerm = Convert.ToString(dtLoanAmt.Rows[0]["approved_term"]) == "" ? 0.00 : Convert.ToDouble(dtLoanAmt.Rows[0]["approved_term"]);
                }
                emilist.Records = EmiRowsByDisbursmentDate(emilist.term_name, emilist.Records.Count, Convert.ToDateTime(DisbursementDate), ApprovedLoanAmt, ApprovedLoanAmt_withRate, ApprovedTerm);
            }
            catch (Exception ex)
            {
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
            }

            return Json(true, JsonRequestBehavior.AllowGet);
        }
    }
}