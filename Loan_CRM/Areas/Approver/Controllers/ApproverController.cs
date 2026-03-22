using Loan_CRM.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading;
using System.Web.Mvc;

namespace Loan_CRM.Areas.Approver.Controllers
{
    public class ApproverController : Controller
    {
        ApproverCommonOperation approverCommonOperation = new ApproverCommonOperation();

        public ActionResult Index()
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.Approver))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            return View();
        }
        // GET: Approver/Approver
        public ActionResult ApproverLoanRequestBucket()
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.Approver))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            return View();
        }

        private List<ApplicationRecordVM> DataBind()
        {
            try
            {
                List<ApplicationRecordVM> Record = new List<ApplicationRecordVM>();
                DataTable dt;
                double a = 0d;
                dt = DbHelper.SelectMethod(QueryHelper.GetApplicationforApproverNew);
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
                            signed = Convert.ToString(!String.IsNullOrWhiteSpace(Convert.ToString(dt.Rows[i]["signurl"]))),
                            user_id = Convert.ToInt32(dt.Rows[i]["user_id"].ToString())
                        });
                    }
                }
                return Record;
            }
            catch (Exception ex)
            {
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                throw ex;
            }

        }

        public ActionResult GetUserData()
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.Approver))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            try
            {
                var jsonResult = Json(DataBind(), JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
            catch (Exception ex)
            {
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                return Json(null, JsonRequestBehavior.AllowGet);
            }

        }

        //To Update the Approval Status
        public ActionResult ApproveLoanRequest(int id, int user_id)
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.Approver))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            bool Status = false;
            try
            {
                ActivityLog.Info($"Application {id} Approved from Approver Window by User {Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value)}", $"{this.GetType().Name} ==> {System.Reflection.MethodBase.GetCurrentMethod().Name}");
                logger.AddWelcomeMessage(History.Approve, "", "We already received your loan contract and your loan has been Approved. ", 0, true, id);
                if (Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value) != 0 && id != 0)
                {

                    string query = string.Format(QueryHelper.UpdateIsApproved, id, Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value));
                    int Result = DbHelper.InsertUpdateDelete(query);
                    if (Result == 1)
                    {
                        Status = true;
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                return Json(null, JsonRequestBehavior.AllowGet);
            }
            return Json(Status, JsonRequestBehavior.AllowGet);
        }
        //To Update the Decline(Is Reverification) Status

        /// <summary>
        /// Decline LoanRequest
        /// </summary>
        /// <param name="remark"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult DeclineLoanRequest(string remark, int id)
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.Approver))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            bool status = false;
            try
            {
                ActivityLog.Info($"Application {id} Declined from Approver Window by User {Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value)}", $"{this.GetType().Name} ==> {System.Reflection.MethodBase.GetCurrentMethod().Name}");
                var UserLoginType = DbHelper.SelectMethod($"SELECT ctr.rolename FROM public.tblapplication_record tar join public.ct_user ctu on tar.verify_by=ctu.userid join ct_roles ctr on ctu.webadminrole=ctr.id and tar.applicationno={id};");
                if (UserLoginType != null && UserLoginType.Rows.Count > 0)
                {
                    if (Convert.ToString(UserLoginType.Rows[0]["rolename"]) == "Adminuser")
                    {
                        var query = DbHelper.InsertUpdateDelete(string.Format(QueryHelper.UpdateIsReVerifyAdmin, "Rejected by Admin", id));
                        if (query > 0)
                        {
                            status = true;
                        }
                    }
                    else
                    {
                        int i = DbHelper.InsertUpdateDelete(string.Format(QueryHelper.RejectedByApprover, id, Convert.ToInt32(Request.Cookies[CookiesKey.UserId].Value),remark));
                        if (i > 0)
                        {
                            status = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                return Json(null, JsonRequestBehavior.AllowGet);
            }

            return Json(status, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        //To View the Record For Approval
        public ActionResult ViewDetailsForApproval(int id, string ActionName)
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.Approver))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            CompleteAppVerificationDetails appRecordVM = new CompleteAppVerificationDetails();
            try
            {
                ActivityLog.Info($"Application {id} Get from Approver Window by User {Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value)}", $"{this.GetType().Name} ==> {System.Reflection.MethodBase.GetCurrentMethod().Name}");
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
                        bool Result = false;
                        Result = UpdateIsPickedApprover(id);

                        if (Result)
                        {
                            appRecordVM = approverCommonOperation.GetApproverData(id);
                        }
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
                    ViewBag.NotificationTemplateList = model.NotificationTemplateList;
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

            return View(appRecordVM);
        }
        //To Update IsPickerApprover 
        private bool UpdateIsPickedApprover(int id)
        {
            try
            {
                int Result = DbHelper.InsertUpdateDelete(string.Format(QueryHelper.Updateispickedapprover, id));
                if (Result == 1)
                    return true;
                else
                    return false;

            }
            catch (Exception ex)
            {
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                throw ex;
            }
        }
        public int ClicktoCall(string ContactNumber)
        {
            try
            {
                return logger.ClicktoCall(ContactNumber, Convert.ToString(HttpContext.Request.Cookies[CookiesKey.AgentName].Value), Convert.ToString(HttpContext.Request.Cookies[CookiesKey.IPAddress].Value));
            }
            catch (Exception ex)
            {
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                throw ex;
            }

        }
        public bool ResetIsPickedApprover(string appNo)
        {
            bool response = false;
            int intAppNo = 0;
            try
            {
                if (!string.IsNullOrWhiteSpace(appNo))
                {
                    intAppNo = Convert.ToInt32(appNo);
                }
                int query = DbHelper.InsertUpdateDelete(string.Format(QueryHelper.ResetIsPickedApprover, intAppNo));
                if (query > 0)
                {
                    response = true;
                }
            }
            catch (Exception ex)
            {
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                throw ex;
            }
            return response;
        }

        [HttpPost]
        public ActionResult DoReject(int id)
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.Approver))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            bool result = false;
            try
            {
                ActivityLog.Info($"Application {id} Rejected from Approver Window by User {Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value)}", $"{this.GetType().Name} ==> {System.Reflection.MethodBase.GetCurrentMethod().Name}");
                var UserLoginType = DbHelper.SelectMethod($"SELECT ctr.rolename FROM public.tblapplication_record tar join public.ct_user ctu on tar.verify_by=ctu.userid join ct_roles ctr on ctu.webadminrole=ctr.id and tar.applicationno={id};");
                if (UserLoginType != null && UserLoginType.Rows.Count > 0)
                {
                    if (Convert.ToString(UserLoginType.Rows[0]["rolename"]) == "Adminuser")
                    {
                        var query = DbHelper.InsertUpdateDelete(string.Format(QueryHelper.UpdateIsReVerifyAdmin, "Rejected by Admin", id));
                        if (query > 0)
                        {
                            result = true;
                        }
                    }
                    else
                    {
                        int i = DbHelper.InsertUpdateDelete(string.Format(QueryHelper.RejectedByApprover, id, Convert.ToInt32(Request.Cookies[CookiesKey.UserId].Value),"Reject From Bucket"));
                        if (i > 0)
                        {
                            result = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                return Json(null, JsonRequestBehavior.AllowGet);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult DoApprove(int id, int user_id)
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.Approver))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            bool Status = false;
            try
            {
                ActivityLog.Info($"Application {id} Approved from Approver Window by User {Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value)}", $"{this.GetType().Name} ==> {System.Reflection.MethodBase.GetCurrentMethod().Name}");
                logger.AddWelcomeMessage(History.Approve, "", "We already received your loan contract and your loan has been Approved. ", 0, true, id);
                if (Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value) != 0 && id != 0)
                {

                    string query = string.Format(QueryHelper.UpdateIsApproved, id, Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value));
                    int Result = DbHelper.InsertUpdateDelete(query);
                    if (Result == 1)
                    {
                        Status = true;
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                return Json(null, JsonRequestBehavior.AllowGet);
            }
            return Json(Status, JsonRequestBehavior.AllowGet);
        }

    }
}
