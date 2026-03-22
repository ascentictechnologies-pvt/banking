using DocumentFormat.OpenXml.VariantTypes;
using Loan_CRM.Areas.Collector.Models;
using Loan_CRM.Models;

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;

namespace Loan_CRM.Areas.Account.Controllers
{
    public class DisbursementController : Controller
    {
      
        // GET: Account/Disbursement
        public ActionResult Index()
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.Accounts))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            return View();
        }
        /// <summary>
        /// Disbursement List details
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult DisbursementList()
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.Accounts))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            DisbursementModel model = new DisbursementModel();

            return View(model);
        }
        /// <summary>
        /// after click search button 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DisbursementList(DisbursementModel obj)
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.Accounts))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            TempData["Result"] = "Post";
            return View(obj);
        }

        /// <summary>
        /// Export to excel in DisbursementList
        /// </summary>
        /// <param name="FromDate"></param>
        /// <param name="ToDate"></param>
        /// <returns></returns>
        public ActionResult UpdateDisbursementList(string FromDate, string ToDate)
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.Accounts))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            string disbursementfull_date = string.Empty;
            DisbursementModel obj = new DisbursementModel();
            try
            {
                var arr_FromDate = FromDate.Split('-');
                obj.FromDate = arr_FromDate[2] + '-' + arr_FromDate[1] + "-" + arr_FromDate[0];
                var arr_ToDate = ToDate.Split('-');
                obj.ToDate = arr_ToDate[2] + '-' + arr_ToDate[1] + "-" + arr_ToDate[0];


                DataTable dt = DbHelper.SelectMethod(string.Format(QueryHelper.GetDisbursementListForExcelExportNew, obj.FromDate, obj.ToDate));
                if (dt != null && dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        var firstName = Convert.ToString(dt.Rows[i]["first_name"]) != string.Empty ? Convert.ToString(dt.Rows[i]["first_name"]) : string.Empty;
                        var middleName = Convert.ToString(dt.Rows[i]["middle_name"]) != string.Empty ? Convert.ToString(dt.Rows[i]["middle_name"]) : string.Empty;
                        var lastName = Convert.ToString(dt.Rows[i]["last_name"]) != string.Empty ? Convert.ToString(dt.Rows[i]["last_name"]) : string.Empty;
                        decimal loanAmount = Convert.ToString(dt.Rows[i]["approved_loan_amount"]) != string.Empty ? Convert.ToDecimal(dt.Rows[i]["approved_loan_amount"]) : 0;
                        // Made changes for Preterm Loan Application
                        decimal adminfee = Convert.ToString(dt.Rows[i]["admin_fee"]) != string.Empty ? Convert.ToDecimal(dt.Rows[i]["admin_fee"]) : 0;
                        if (loanAmount != 0)
                        {
                            decimal pending_amount = 0;
                            String _user_Id = Convert.ToString(dt.Rows[i]["user_id"]) != string.Empty ? Convert.ToString(dt.Rows[i]["user_id"]) : "";
                            if (!String.IsNullOrWhiteSpace(_user_Id))
                            {
                                DataTable _PendinaAmountTable = DbHelper.SelectMethod(String.Format(QueryHelper.GetPendingAmountforPreterm, _user_Id));
                                if (_PendinaAmountTable != null && _PendinaAmountTable.Rows.Count > 0)
                                {
                                    pending_amount = Convert.ToString(_PendinaAmountTable.Rows[0]["pending_amount"]) != string.Empty ? Convert.ToDecimal(_PendinaAmountTable.Rows[0]["pending_amount"]) : 0;
                                    #region New Code By Nagesh For Preterm Application Deduction Amount Store as Payment in previous Application
                                    if (HttpContext.Request.Cookies[CookiesKey.AgentName].Value != null)
                                    {
                                        string AgentName = Convert.ToString(HttpContext.Request.Cookies[CookiesKey.AgentName].Value);
                                        int ApplicationId = 0;
                                        DataTable _LastApplication = DbHelper.SelectMethod(String.Format(QueryHelper.GetLastApplication, _user_Id));
                                        if (_LastApplication != null && _LastApplication.Rows.Count > 0)
                                        {
                                            ApplicationId = Convert.ToString(_LastApplication.Rows[0]["applicationno"]) != string.Empty ? Convert.ToInt32(_LastApplication.Rows[0]["applicationno"]) : 0;
                                        }
                                        if (ApplicationId > 0)
                                        {
                                            DbHelper.InsertUpdateDelete(String.Format(QueryHelper.UpdatePretermReloanRecord, pending_amount, ApplicationId));
                                            DataTable _Patiddt = DbHelper.SelectMethod(string.Format(QueryHelper.InsertPAY, ApplicationId, pending_amount, "Preterm", "Preterm", 0, _user_Id, DateTime.Now.ToString("yyyy-MM-dd")));
                                            int m = DbHelper.InsertUpdateDelete(string.Format(QueryHelper.UpdatePay, ApplicationId));
                                            if (_Patiddt != null && _Patiddt.Rows.Count > 0)
                                            {
                                                PaymentInformation paymentInfo = new PaymentInformation();
                                                int payId = Convert.ToInt32(_Patiddt.Rows[0]["currval"]);
                                                int _Result = DbHelper.InsertUpdateDelete(string.Format(QueryHelper.ApprovePayment, pending_amount, 0, "", payId, ApplicationId));

                                                if (_Result > 0)
                                                {
                                                    var _dt = DbHelper.SelectMethod(String.Format(QueryHelper.GetAmount, payId));
                                                    if (_dt != null && _dt.Rows.Count > 0)
                                                    {
                                                        var getEmi = DbHelper.SelectMethod(string.Format(QueryHelper.GetEmiDetail, ApplicationId));
                                                        if (getEmi != null && getEmi.Rows.Count > 0)
                                                        {
                                                            double totalamountpaid = 0;
                                                            DateTime _Paid_on = DateTime.Now;
                                                            totalamountpaid = float.Parse(Convert.ToString(_dt.Rows[0]["paid_amount"]));
                                                            _Paid_on = Convert.ToDateTime(_dt.Rows[0]["paid_on"]);
                                                            paymentInfo.emiid = Convert.ToInt64(getEmi.Rows[0]["emi_id"]);
                                                            for (int j = 0; j < getEmi.Rows.Count; j++)
                                                            {
                                                                paymentInfo.emiamount = Convert.ToString(getEmi.Rows[j]["emi_amount"]);
                                                                paymentInfo.Paidamount = Convert.ToString(getEmi.Rows[j]["paidamount"]);
                                                                paymentInfo.Balanceamount = Convert.ToString(getEmi.Rows[j]["balanceamount"]);
                                                                paymentInfo.emidetailid = Convert.ToInt64(getEmi.Rows[j]["emi_detail_id"]);
                                                                paymentInfo.emistatus = Convert.ToInt64(getEmi.Rows[j]["emi_status"]);


                                                                double totalemiamount = float.Parse(paymentInfo.emiamount);
                                                                double Paidamount = 0;
                                                                Paidamount = float.Parse(paymentInfo.Paidamount);

                                                                double Balanceamount = 0;
                                                                Balanceamount = float.Parse(paymentInfo.Balanceamount);

                                                                if (paymentInfo.emistatus != 2)
                                                                {
                                                                    if (totalamountpaid < Balanceamount && totalamountpaid > 0)
                                                                    {
                                                                        paymentInfo.emistatus = 1;
                                                                        var totalBalanceamount = Balanceamount - totalamountpaid;
                                                                        DbHelper.InsertUpdateDelete(string.Format(QueryHelper.UpdateEmiAmountPreterm, paymentInfo.emistatus, totalamountpaid, totalBalanceamount, paymentInfo.emidetailid, _Paid_on.ToString("yyyy-MM-dd hh:mm:ss")));

                                                                        if (totalBalanceamount == 0)
                                                                        {
                                                                            int result2 = DbHelper.InsertUpdateDelete(string.Format(QueryHelper.UpdateIsPay, paymentInfo.emidetailid));
                                                                        }
                                                                        else if (totalBalanceamount > 0)
                                                                        {
                                                                            int result3 = DbHelper.InsertUpdateDelete(string.Format(QueryHelper.UpdateIsPartial, paymentInfo.emidetailid));
                                                                        }
                                                                    }
                                                                    else if (totalamountpaid > Balanceamount && totalamountpaid > 0)
                                                                    {
                                                                        paymentInfo.emistatus = 2;
                                                                        totalamountpaid -= Balanceamount;
                                                                        var EqualBalanceamount = 0;
                                                                        string query1 = (string.Format(QueryHelper.UpdateEmiAmountPreterm, paymentInfo.emistatus, Balanceamount, EqualBalanceamount, paymentInfo.emidetailid, _Paid_on.ToString("yyyy-MM-dd hh:mm:ss")));
                                                                        DbHelper.InsertUpdateDelete(query1);
                                                                        string query3 = (string.Format(QueryHelper.UpdateIsPay, paymentInfo.emidetailid));
                                                                        int result3 = DbHelper.InsertUpdateDelete(query3);
                                                                    }
                                                                    else if (totalamountpaid == Balanceamount && totalamountpaid > 0)
                                                                    {
                                                                        paymentInfo.emistatus = 2;
                                                                        var EqualBalanceamount = 0;
                                                                        string query1 = (string.Format(QueryHelper.UpdateEmiAmountPreterm, paymentInfo.emistatus, totalamountpaid, EqualBalanceamount, paymentInfo.emidetailid, _Paid_on.ToString("yyyy-MM-dd hh:mm:ss")));
                                                                        DbHelper.InsertUpdateDelete(query1);
                                                                        string query4 = (string.Format(QueryHelper.UpdateIsPay, paymentInfo.emidetailid));
                                                                        int result4 = DbHelper.InsertUpdateDelete(query4);
                                                                    }
                                                                }
                                                            }
                                                            var getBalance = DbHelper.SelectMethod(string.Format(QueryHelper.GetBalanceNew, paymentInfo.emiid));
                                                            if (getBalance != null && getBalance.Rows.Count >= 0)
                                                            {
                                                                double balance = 0.0;
                                                                balance = Convert.ToDouble(getBalance.Rows[0]["balanceamount"]);
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    #endregion
                                }
                            }
                            loanAmount = (loanAmount - ((loanAmount * adminfee) / 100)) - pending_amount;
                        }
                        obj.DisbursementModelDetails.Add(new DisbursementModelDetails
                        {
                            ContractNumber = Convert.ToString(dt.Rows[i]["contract_ref_no"]) != string.Empty ? Convert.ToString(dt.Rows[i]["contract_ref_no"]) : string.Empty,
                            BankName_or_Status = Convert.ToString(dt.Rows[i]["bankname"]) != string.Empty ? Convert.ToString(dt.Rows[i]["bankname"]) : Convert.ToString(dt.Rows[i]["personal_bank_name_remark"]),
                            BankAccountNumber_or_Status = Convert.ToString(dt.Rows[i]["bankaccountno"]) != string.Empty ? Convert.ToString(dt.Rows[i]["bankaccountno"]) : Convert.ToString(dt.Rows[i]["personal_bank_ac_no_remark"]),
                            Name = firstName + " " + middleName + " " + lastName,
                            ApplicationNo = Convert.ToString(dt.Rows[i]["applicationno"]) != string.Empty ? Convert.ToInt32(dt.Rows[i]["applicationno"]) : 0,
                            ContactNumber = Convert.ToString(dt.Rows[i]["personalcontactno"]) != string.Empty ? Convert.ToString(dt.Rows[i]["personalcontactno"]) : string.Empty,
                            ApprovedOn = Convert.ToString(dt.Rows[i]["approvedon"]) != string.Empty ? Convert.ToString(dt.Rows[i]["approvedon"]) : string.Empty,
                            DisbursementDate = Convert.ToString(dt.Rows[i]["disbursement_date"]) != string.Empty ? Convert.ToString(dt.Rows[i]["disbursement_date"]) : string.Empty,
                            Disbursement_FullDate = Convert.ToString(dt.Rows[i]["disbursementfull_date"]) != string.Empty ? Convert.ToString(dt.Rows[i]["disbursementfull_date"]) : DateTime.Now.ToString("dd-MMM-yyyy"),
                            Loan_Amount = Convert.ToDecimal(loanAmount)
                        });
                        int a = DbHelper.InsertUpdateDelete(string.Format(QueryHelper.UpdateDisbursementList, Convert.ToString(dt.Rows[i]["applicationno"]) != string.Empty ? Convert.ToInt32(dt.Rows[i]["applicationno"]) : 0));
                        DbHelper.InsertUpdateDelete(string.Format(QueryHelper.InsertDisbursmentAmount, (Convert.ToString(dt.Rows[i]["applicationno"]) != string.Empty ? Convert.ToInt32(dt.Rows[i]["applicationno"]) : 0), Convert.ToDecimal(loanAmount), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value)));
                        ActivityLog.Info($"Application {(Convert.ToString(dt.Rows[i]["applicationno"]) != string.Empty ? Convert.ToInt32(dt.Rows[i]["applicationno"]) : 0)} Disbursed from Account Window by User {Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value)}", $"{this.GetType().Name} ==> {System.Reflection.MethodBase.GetCurrentMethod().Name}");
                    }
                    TempData["Result"] = "Post";
                }
            }
            catch (Exception ex)
            {
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                disbursementfull_date = string.Empty;
            }
            var jsonResult = Json(obj.DisbursementModelDetails, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        public ActionResult GetDisbursementData(string FromDate, string ToDate)
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.Accounts))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            DisbursementModel model = new DisbursementModel();
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
                DataTable dt = null;
                if (!String.IsNullOrWhiteSpace(FromDate) && !String.IsNullOrWhiteSpace(ToDate))
                {
                    var arr_FromDate = FromDate.Split('-');
                    var _FromDate = arr_FromDate[2] + '-' + arr_FromDate[1] + "-" + arr_FromDate[0];
                    var arr_ToDate = ToDate.Split('-');
                    var _ToDate = arr_ToDate[2] + '-' + arr_ToDate[1] + "-" + arr_ToDate[0];
                    dt = DbHelper.SelectMethod(string.Format(QueryHelper.GetDisbursementListForExcelExportUpdated, _FromDate, _ToDate));
                }
                else
                {
                    // dt = DbHelper.SelectMethod(string.Format(QueryHelper.GetDisbursemetnListNew));
                    dt = DbHelper.SelectMethod(string.Format(QueryHelper.GetDisbursementListForExcelExportUpdated, DateTime.Now.AddDays(-5).ToString("yyyy-MM-dd"), DateTime.Now.ToString("yyyy-MM-dd")));
                }

                if (dt != null && dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        decimal loanAmount = Convert.ToString(dt.Rows[i]["approved_loan_amount"]) != string.Empty ? Convert.ToDecimal(dt.Rows[i]["approved_loan_amount"]) : 0;
                        decimal adminfee = Convert.ToString(dt.Rows[i]["admin_fee"]) != string.Empty ? Convert.ToDecimal(dt.Rows[i]["admin_fee"]) : 0;
                        if (loanAmount > 0)
                        {
                            decimal pending_amount = 0;
                            String _user_Id = Convert.ToString(dt.Rows[i]["user_id"]) != string.Empty ? Convert.ToString(dt.Rows[i]["user_id"]) : "";
                            //if (i >= start && i < start + length)
                            //{
                                if (!String.IsNullOrWhiteSpace(_user_Id))
                                {
                                    DataTable _PendinaAmountTable = DbHelper.SelectMethod(String.Format(QueryHelper.GetPendingAmountforPreterm, _user_Id));
                                    if (_PendinaAmountTable != null && _PendinaAmountTable.Rows.Count > 0)
                                    {
                                        pending_amount = Convert.ToString(_PendinaAmountTable.Rows[0]["pending_amount"]) != string.Empty ? Convert.ToDecimal(_PendinaAmountTable.Rows[0]["pending_amount"]) : 0;
                                    }
                                }

                            //}
                            loanAmount = (loanAmount - ((loanAmount * adminfee) / 100)) - pending_amount;
                            //logger.WriteErrorLogs($"Loan Amount : {loanAmount} Admin Fee : {adminfee} : Pending_Amount : {pending_amount}", "");
                        }
                        model.DisbursementModelDetails.Add(new DisbursementModelDetails
                        {
                            BankName_or_Status = Convert.ToString(dt.Rows[i]["bankname"]) != string.Empty ? Convert.ToString(dt.Rows[i]["bankname"]) : Convert.ToString(dt.Rows[i]["personal_bank_name_remark"]),
                            BankAccountNumber_or_Status = Convert.ToString(dt.Rows[i]["bankaccountno"]) != string.Empty ? Convert.ToString(dt.Rows[i]["bankaccountno"]) : Convert.ToString(dt.Rows[i]["personal_bank_ac_no_remark"]),
                            Name = Convert.ToString(dt.Rows[i]["first_name"]) + " " + Convert.ToString(dt.Rows[i]["middle_name"]) + " " + Convert.ToString(dt.Rows[i]["last_name"]),
                            ApplicationNo = Convert.ToString(dt.Rows[i]["applicationno"]) != string.Empty ? Convert.ToInt32(dt.Rows[i]["applicationno"]) : 0,
                            ContactNumber = Convert.ToString(dt.Rows[i]["personalcontactno"]) != string.Empty ? Convert.ToString(dt.Rows[i]["personalcontactno"]) : string.Empty,
                            ApprovedOn = Convert.ToString(dt.Rows[i]["approvedon"]) != string.Empty ? Convert.ToString(dt.Rows[i]["approvedon"]) : string.Empty,
                            Loan_Amount = Convert.ToDecimal(loanAmount),
                            ContractNumber = Convert.ToString(dt.Rows[i]["contract_ref_no"]) != string.Empty ? Convert.ToString(dt.Rows[i]["contract_ref_no"]) : string.Empty,
                            DisbursementDate = Convert.ToString(dt.Rows[i]["disbursement_date"]) != string.Empty ? Convert.ToString(dt.Rows[i]["disbursement_date"]) : string.Empty,
                            Disbursement_FullDate = Convert.ToString(dt.Rows[i]["disbursementfull_date"]) != string.Empty ? Convert.ToString(dt.Rows[i]["disbursementfull_date"]) : string.Empty,
                        });
                    }
                }


                int totalRecords = model.DisbursementModelDetails.Count;
                if (!string.IsNullOrEmpty(search) &&
                !string.IsNullOrWhiteSpace(search))
                {
                    // Apply search 
                    model.DisbursementModelDetails = model.DisbursementModelDetails.Where(p => p.BankName_or_Status.ToString().ToLower().Contains(search.ToLower()) ||
                    p.BankAccountNumber_or_Status.ToString().ToLower().Contains(search.ToLower()) ||
                     p.Name.ToString().ToLower().Contains(search.ToLower()) ||
                      p.Loan_Amount.ToString().ToLower().Contains(search.ToLower()) ||
                       p.Disbursement_FullDate.ToString().ToLower().Contains(search.ToLower()) ||
                        p.ContractNumber.ToString().ToLower().Contains(search.ToLower())).ToList();
                }


                model.DisbursementModelDetails = this.SortByColumnWithOrder(order, orderDir, model.DisbursementModelDetails);
                int recFilter = model.DisbursementModelDetails.Count;
                model.DisbursementModelDetails = model.DisbursementModelDetails.Skip(start).Take(length).ToList<DisbursementModelDetails>();
                result = this.Json(new { draw = Convert.ToInt32(draw), recordsTotal = totalRecords, recordsFiltered = recFilter, data = model.DisbursementModelDetails }, JsonRequestBehavior.AllowGet);

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
                        lst = orderDir.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? ApplicationRecordList.OrderByDescending(p => p.BankName_or_Status).ToList() : ApplicationRecordList.OrderBy(p => p.BankName_or_Status).ToList();
                        break;
                    case "1":
                        lst = orderDir.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? ApplicationRecordList.OrderByDescending(p => p.BankAccountNumber_or_Status).ToList() : ApplicationRecordList.OrderBy(p => p.BankAccountNumber_or_Status).ToList();
                        break;
                    case "2":
                        lst = orderDir.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? ApplicationRecordList.OrderByDescending(p => p.Name).ToList() : ApplicationRecordList.OrderBy(p => p.Name).ToList();
                        break;
                    case "3":
                        lst = orderDir.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? ApplicationRecordList.OrderByDescending(p => p.Loan_Amount).ToList() : ApplicationRecordList.OrderBy(p => p.Loan_Amount).ToList();
                        break;
                    case "4":
                        lst = orderDir.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? ApplicationRecordList.OrderByDescending(p => p.Disbursement_FullDate).ToList() : ApplicationRecordList.OrderBy(p => p.Disbursement_FullDate).ToList();
                        break;
                    case "5":
                        lst = orderDir.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? ApplicationRecordList.OrderByDescending(p => p.ContractNumber).ToList() : ApplicationRecordList.OrderBy(p => p.ContractNumber).ToList();
                        break;
                    default:
                        lst = orderDir.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? ApplicationRecordList.OrderByDescending(p => p.BankName_or_Status).ToList() : ApplicationRecordList.OrderBy(p => p.BankName_or_Status).ToList();
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
    }
}