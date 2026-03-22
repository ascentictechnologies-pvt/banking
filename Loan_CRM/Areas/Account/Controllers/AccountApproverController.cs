using Loan_CRM.Areas.Collector.Models;
using Loan_CRM.Models;

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Loan_CRM.Areas.Account.Controllers
{
    public class AccountApproverController : Controller
    {
        public ActionResult Index()
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.Accounts))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            return View();
        }

        // GET: Account/ 
        public ActionResult GetAccountApproverData()
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.Accounts))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            DisbursementModel model = new DisbursementModel();
            return View(model);

        }

        public ActionResult GetAccountData()
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

                var Terms = GetTermType();
                var Tenure = GetTermName();
                DataTable dt = DbHelper.SelectMethod(string.Format(QueryHelper.GetAccountApproverRecordNew));
                if (dt != null && dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        int _Term = dt.Rows[i]["termtype"].ToString() != string.Empty ? Convert.ToInt32(dt.Rows[i]["termtype"]) : 0;
                        string TermType = Terms.FirstOrDefault(x => x.Id == _Term) != null ? Terms.FirstOrDefault(x => x.Id == _Term).term_Type : String.Empty;
                        int _Tenure = dt.Rows[i]["term"].ToString() != string.Empty ? Convert.ToInt32(dt.Rows[i]["term"]) : 0;
                        string TermName = Tenure.FirstOrDefault(x => x.Id == _Tenure) != null ? Tenure.FirstOrDefault(x => x.Id == _Tenure).term_Name : String.Empty;

                        string AdditionalRequestTermStr = TermType + ", " + TermName;
                        string GovUrl = Convert.ToString(dt.Rows[i]["gov_id_url"]) != string.Empty ? Convert.ToString(dt.Rows[i]["gov_id_url"]) : string.Empty;
                        string AtmUrl = Convert.ToString(dt.Rows[i]["atm_url"]) != string.Empty ? Convert.ToString(dt.Rows[i]["atm_url"]) : string.Empty;
                        int Id = Convert.ToString(dt.Rows[i]["applicationno"]) != string.Empty ? Convert.ToInt32(dt.Rows[i]["applicationno"]) : 0;
                        decimal _adminFee = Convert.ToString(dt.Rows[i]["admin_fee"]) != string.Empty ? Convert.ToInt32(dt.Rows[i]["admin_fee"]) : 0;
                        DisbursementModelDetails _data = (new DisbursementModelDetails
                        {
                            Name = Convert.ToString(dt.Rows[i]["first_name"]) + " " + Convert.ToString(dt.Rows[i]["middle_name"]) + " " + Convert.ToString(dt.Rows[i]["last_name"]),
                            ApplicationNo = Convert.ToString(dt.Rows[i]["applicationno"]) != string.Empty ? Convert.ToInt32(dt.Rows[i]["applicationno"]) : 0,
                            Date_Applied = Convert.ToString(dt.Rows[i]["dateapplied"]) != string.Empty ? Convert.ToDateTime(dt.Rows[i]["dateapplied"]) : DateTime.Now,
                            DateApplied = Convert.ToString(dt.Rows[i]["date_applied"]) != string.Empty ? Convert.ToString(dt.Rows[i]["date_applied"]) : string.Empty,
                            ApprovedOn = Convert.ToString(dt.Rows[i]["approvedon"]) != string.Empty ? Convert.ToString(dt.Rows[i]["approvedon"]) : string.Empty,
                            approved_on = Convert.ToString(dt.Rows[i]["approved_on"]) != string.Empty ? Convert.ToDateTime(dt.Rows[i]["approved_on"]) : DateTime.Now,
                            GrossIncome = Convert.ToString(dt.Rows[i]["gross_income"]) != string.Empty ? Convert.ToDouble(dt.Rows[i]["gross_income"]) : 0.00,
                            TermComplete = AdditionalRequestTermStr,
                            Loan_Amount = Convert.ToString(dt.Rows[i]["approved_loan_amount"]) != string.Empty ? Convert.ToInt32(dt.Rows[i]["approved_loan_amount"]) : 0,
                            BankName_or_Status = Convert.ToString(dt.Rows[i]["bankname"]) != string.Empty ? Convert.ToString(dt.Rows[i]["bankname"]) : Convert.ToString(dt.Rows[i]["personal_bank_name_remark"]),
                            BankAccountNumber_or_Status = Convert.ToString(dt.Rows[i]["bankaccountno"]) != string.Empty ? Convert.ToString(dt.Rows[i]["bankaccountno"]) : Convert.ToString(dt.Rows[i]["personal_bank_ac_no_remark"]),
                            AtmUrl = AtmUrl,
                            user_id = Convert.ToString(dt.Rows[i]["user_id"]) != string.Empty ? Convert.ToInt32(dt.Rows[i]["user_id"]) : 0,

                        });
                       // _data.AmountToDisbursement = GetAmountToBeDisburement(_data.Loan_Amount, _adminFee, Convert.ToString(_data.user_id));
                        model.Add(_data);
                    }
                }


                int totalRecords = model.Count;
                if (!string.IsNullOrEmpty(search) &&
                !string.IsNullOrWhiteSpace(search))
                {
                    // Apply search 
                    model = model.Where(p => p.Name.ToString().ToLower().Contains(search.ToLower()) ||
                    p.ApplicationNo.ToString().ToLower().Contains(search.ToLower()) ||
                     p.Date_Applied.ToString().ToLower().Contains(search.ToLower()) ||
                      p.approved_on.ToString().ToLower().Contains(search.ToLower()) ||
                       p.GrossIncome.ToString().ToLower().Contains(search.ToLower()) ||
                        p.TermComplete.ToString().ToLower().Contains(search.ToLower()) ||
                         p.Loan_Amount.ToString().ToLower().Contains(search.ToLower()) ||
                          p.BankName_or_Status.ToString().ToLower().Contains(search.ToLower()) ||
                    p.BankAccountNumber_or_Status.ToString().ToLower().Contains(search.ToLower()) ||
                    p.AtmUrl.ToString().ToLower().Contains(search.ToLower())).ToList();
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

        public decimal GetAmountToBeDisburement(decimal loanAmount, decimal adminfee, String _user_Id)
        {
            if (loanAmount != 0)
            {
                decimal pending_amount = 0;
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
            return loanAmount;
        }

        private List<DisbursementModelDetails> SortByColumnWithOrder(string order, string orderDir, List<DisbursementModelDetails> ApplicationRecordList)
        {
            List<DisbursementModelDetails> lst = new List<DisbursementModelDetails>();
            try
            {
                switch (order)
                {
                    case "0":
                        lst = orderDir.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? ApplicationRecordList.OrderByDescending(p => p.Name).ToList() : ApplicationRecordList.OrderBy(p => p.Name).ToList();
                        break;
                    case "1":
                        lst = orderDir.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? ApplicationRecordList.OrderByDescending(p => p.ApplicationNo).ToList() : ApplicationRecordList.OrderBy(p => p.ApplicationNo).ToList();
                        break;
                    case "2":
                        lst = orderDir.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? ApplicationRecordList.OrderByDescending(p => p.Date_Applied).ToList() : ApplicationRecordList.OrderBy(p => p.Date_Applied).ToList();
                        break;
                    case "3":
                        lst = orderDir.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? ApplicationRecordList.OrderByDescending(p => p.approved_on).ToList() : ApplicationRecordList.OrderBy(p => p.approved_on).ToList();
                        break;
                    case "4":
                        lst = orderDir.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? ApplicationRecordList.OrderByDescending(p => p.BankName_or_Status).ToList() : ApplicationRecordList.OrderBy(p => p.BankName_or_Status).ToList();
                        break;
                    case "5":
                        lst = orderDir.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? ApplicationRecordList.OrderByDescending(p => p.BankAccountNumber_or_Status).ToList() : ApplicationRecordList.OrderBy(p => p.BankAccountNumber_or_Status).ToList();
                        break;
                    case "6":
                        lst = orderDir.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? ApplicationRecordList.OrderByDescending(p => p.TermComplete).ToList() : ApplicationRecordList.OrderBy(p => p.TermComplete).ToList();
                        break;
                    case "7":
                        lst = orderDir.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? ApplicationRecordList.OrderByDescending(p => p.Loan_Amount).ToList() : ApplicationRecordList.OrderBy(p => p.Loan_Amount).ToList();
                        break;
                    case "8":
                        lst = orderDir.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? ApplicationRecordList.OrderByDescending(p => p.AtmUrl).ToList() : ApplicationRecordList.OrderBy(p => p.AtmUrl).ToList();
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


        private List<TermType> GetTermType()
        {
            List<TermType> termTypes = new List<TermType>();
            try
            {
                string str = string.Empty;
                DataTable dt = DbHelper.SelectMethod(QueryHelper.GetAllTermType);
                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow item in dt.Rows)
                    {
                        termTypes.Add(new TermType
                        {
                            Id = Convert.ToInt32(item["id"]),
                            term_Type = Convert.ToString(item["termtype"])
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                throw ex;
            }
            return termTypes;
        }

        private List<Terms> GetTermName()
        {
            List<Terms> Tenure = new List<Terms>();
            try
            {
                DataTable dt = DbHelper.SelectMethod(QueryHelper.GetTermTypeAll);
                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow item in dt.Rows)
                    {
                        Tenure.Add(new Terms
                        {
                            Id = Convert.ToInt32(item["id"]),
                            admin_fee = Convert.ToDecimal(item["admin_fee"]),
                            isPublic = Convert.ToBoolean(item["ispublic"]),
                            Rate = Convert.ToDecimal(item["interest_rate"]),
                            term_Id = Convert.ToInt32(item["term_id"]),
                            term_Name = Convert.ToString(item["term_name"]),
                            term_Value = Convert.ToString(item["term_value"])
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                throw ex;
            }
            return Tenure;
        }

        [HttpPost]
        public ActionResult UpdateApprover(int applicationno, int user_id, int amount)
        {
            bool flag = false;
            try
            {
                ActivityLog.Info($"Application {applicationno} Approved with Amount{amount} from Account Window by User {Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value)}", $"{this.GetType().Name} ==> {System.Reflection.MethodBase.GetCurrentMethod().Name}");

                int i = DbHelper.InsertUpdateDelete(string.Format(QueryHelper.UpdateAccountApprover, applicationno));
                if (i > 0)
                {
                    flag = true;

                }
                logger.AddWelcomeMessage(History.Disbursement, "", "Your account is scheduled for disbursement within the day. Please wait for your funds to be disbursed to your bank account.", amount, false, applicationno, "");
            }
            catch (Exception ex)
            {
                flag = false;
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
            }
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        public ActionResult RejectAccount(int applicationno, int user_id)
        {
            bool flag = false;
            try
            {
                ActivityLog.Info($"Application {applicationno} Rejected from Account Window by User {Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value)}", $"{this.GetType().Name} ==> {System.Reflection.MethodBase.GetCurrentMethod().Name}");
                int i = DbHelper.InsertUpdateDelete(string.Format(QueryHelper.RejectAccountApprover, applicationno));
                if (i > 0)
                {
                    flag = true;

                }
                logger.AddWelcomeMessage(History.Application, "", "Your loan has Been Cancelled. For inquiries call Cash Mart hotline", 0, true, applicationno, "");
            }
            catch (Exception ex)
            {
                flag = false;
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
            }

            return Json(flag, JsonRequestBehavior.AllowGet);
        }
    }

}
