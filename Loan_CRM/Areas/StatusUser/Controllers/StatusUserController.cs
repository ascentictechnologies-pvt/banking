using DocumentFormat.OpenXml.Spreadsheet;
using Loan_CRM.Areas.Admin.Models;
using Loan_CRM.Areas.Checker.Models;
using Loan_CRM.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Loan_CRM.Areas.StatusUser.Controllers
{
    public class StatusUserController : Controller
    {
        [HttpGet]
        public ActionResult GetApplicationStatus()
        {
            GetApplicationStatusModel _model = new GetApplicationStatusModel();
            return View(_model);
        }

        [HttpPost]
        public ActionResult GetApplicationStatus(GetApplicationStatusModel model)
        {
            GetApplicationStatusModel _model = new GetApplicationStatusModel();
            if (!String.IsNullOrWhiteSpace(model.SearchBy))
            {
                String Query = QueryHelper.GetApplicationStatusBy;
                if (model.SearchBy == "ApplicationNo")
                {
                    Query += QueryHelper.GetApplicationStatusByApplicationNo;
                }
                else if (model.SearchBy == "Name")
                {
                    Query += QueryHelper.GetApplicationStatusByname;
                }
                else if (model.SearchBy == "Email")
                {
                    Query += QueryHelper.GetApplicationStatusByEMail;
                }
                DataTable dtRecords = DbHelper.SelectMethod(string.Format(Query, model.SearchText));
                if (dtRecords != null && dtRecords.Rows.Count > 0)
                {
                    foreach (DataRow item in dtRecords.Rows)
                    {
                        logger.WriteErrorLogs($"=============================================================", $"{this.GetType().Name} ==> {System.Reflection.MethodBase.GetCurrentMethod().Name}");
                        for (int i = 0; i < dtRecords.Columns.Count; i++)
                        {
                            logger.WriteErrorLogs($"{dtRecords.Columns[i].ColumnName} ==> {item[i]}", $"{this.GetType().Name} ==> {System.Reflection.MethodBase.GetCurrentMethod().Name}");
                        }
                        logger.WriteErrorLogs($"=============================================================", $"{this.GetType().Name} ==> {System.Reflection.MethodBase.GetCurrentMethod().Name}");
                        GetApplicationStatusData data = new GetApplicationStatusData()
                        {
                            ApplicationNo = Convert.ToInt32(item["applicationno"]),
                            Name = $"{Convert.ToString(item["first_name"])} {Convert.ToString(item["middle_name"])} {Convert.ToString(item["last_name"])}",
                            ApprovedOn = Convert.ToString(item["approvedon"]),
                            CheckedBy = Convert.ToString(item["checked_by"]),
                            VerifiedBy = Convert.ToString(item["verify_by"]),
                            IsComplete = Convert.ToBoolean(item["iscompleted"]) == true ? "Completed" : "Not Completed"
                        };
                        if (!String.IsNullOrWhiteSpace(data.CheckedBy))
                        {
                            logger.WriteErrorLogs("Step - 1 ", $"{this.GetType().Name} ==> {System.Reflection.MethodBase.GetCurrentMethod().Name}");
                            data.CheckedOn = Convert.ToString(item["checkedon"]);
                        }
                        if (!String.IsNullOrWhiteSpace(data.VerifiedBy))
                        {
                            logger.WriteErrorLogs("Step - 2 ", $"{this.GetType().Name} ==> {System.Reflection.MethodBase.GetCurrentMethod().Name}");
                            data.VerifiedOn = Convert.ToString(item["verifiedon"]);
                        }
                        if (Convert.ToBoolean(item["isapproved"]))
                        {
                            logger.WriteErrorLogs("Step - 3 ", $"{this.GetType().Name} ==> {System.Reflection.MethodBase.GetCurrentMethod().Name}");
                            data.ApprovedOn = Convert.ToString(item["approvedon"]);
                        }
                        if (Convert.ToBoolean(item["isdeleted"])==false && (Convert.ToBoolean(item["isfor_fsbucket"]) || (Convert.ToInt32(item["reprecount"]) > 0)) && !Convert.ToBoolean(item["isfor_reloan"]) && !Convert.ToBoolean(item["is_preterm"]) && Convert.ToBoolean(item["iscompleted"]) && Convert.ToBoolean(item["isdisbursedexport"]))
                        {
                            logger.WriteErrorLogs("Step - 4 ", $"{this.GetType().Name} ==> {System.Reflection.MethodBase.GetCurrentMethod().Name}");
                            data.location = "FS Bucket";
                            data.HandleBy = Convert.ToString(item["collectorname"]);
                        }
                        else if (Convert.ToBoolean(item["isinsufficient_doc"]))
                        {
                            logger.WriteErrorLogs("Step - 11 ", $"{this.GetType().Name} ==> {System.Reflection.MethodBase.GetCurrentMethod().Name}");
                            data.location = "Insufficient Doc";
                        }
                        else if (Convert.ToBoolean(item["is_orr_declined"]))
                        {
                            logger.WriteErrorLogs("Step - 12 ", $"{this.GetType().Name} ==> {System.Reflection.MethodBase.GetCurrentMethod().Name}");
                            data.location = "ORR Declined";
                        }
                        else if (Convert.ToBoolean(item["isdeleted"]))
                        {
                            logger.WriteErrorLogs("Step - 5 ", $"{this.GetType().Name} ==> {System.Reflection.MethodBase.GetCurrentMethod().Name}");
                            data.location = "Declined bucket";
                        }
                        else if (Convert.ToBoolean(item["isrecheck"]))
                        {
                            logger.WriteErrorLogs("Step - 6 ", $"{this.GetType().Name} ==> {System.Reflection.MethodBase.GetCurrentMethod().Name}");
                            data.location = "Rechecker";
                            data.HandleBy = data.CheckedBy;
                        }
                        else if (Convert.ToBoolean(item["isreverified"]))
                        {
                            logger.WriteErrorLogs("Step - 7 ", $"{this.GetType().Name} ==> {System.Reflection.MethodBase.GetCurrentMethod().Name}");
                            data.location = "Reverification";
                            data.HandleBy = data.VerifiedBy;
                        }
                        else if (Convert.ToBoolean(item["isfor_reloan"]) && Convert.ToBoolean(item["isverified"]) && Convert.ToInt32(item["contractcount"]) <= 0)
                        {
                            logger.WriteErrorLogs("Step - 8 ", $"{this.GetType().Name} ==> {System.Reflection.MethodBase.GetCurrentMethod().Name}");
                            data.location = "Reloan";
                            data.HandleBy = Convert.ToString(item["reloanby"]);
                        }
                        else if (Convert.ToBoolean(item["is_preterm"]) && Convert.ToBoolean(item["isverified"]) && Convert.ToInt32(item["contractcount"]) <= 0)
                        {
                            logger.WriteErrorLogs("Step - 9 ", $"{this.GetType().Name} ==> {System.Reflection.MethodBase.GetCurrentMethod().Name}");
                            data.location = "Preterm";
                            data.HandleBy = Convert.ToString(item["Pretermby"]);
                        }
                        else if (Convert.ToBoolean(item["is_agency"]))
                        {
                            logger.WriteErrorLogs("Step - 10 ", $"{this.GetType().Name} ==> {System.Reflection.MethodBase.GetCurrentMethod().Name}");
                            data.location = "Agency bucket";
                        }
                        else if (Convert.ToBoolean(item["isdefaulter"]) && (Convert.ToInt32(item["emi_count"]) > 0))
                        {
                            logger.WriteErrorLogs("Step - 13 ", $"{this.GetType().Name} ==> {System.Reflection.MethodBase.GetCurrentMethod().Name}");
                            data.location = "Defaulter - Loan payment bucket";
                            data.HandleBy = Convert.ToString(item["defaultername"]);
                        }
                        else if (!Convert.ToBoolean(item["ischeck"]) && !Convert.ToBoolean(item["isrecheck"]) && !Convert.ToBoolean(item["isverified"]) && !Convert.ToBoolean(item["isreverified"]) && !Convert.ToBoolean(item["isapproved"]) && !string.IsNullOrEmpty(data.CheckedBy))
                        {
                            logger.WriteErrorLogs("Step - 14 ", $"{this.GetType().Name} ==> {System.Reflection.MethodBase.GetCurrentMethod().Name}");
                            data.location = "Checker KIV";
                            data.HandleBy = data.CheckedBy;
                        }
                        else if (!Convert.ToBoolean(item["ischeck"]) && !Convert.ToBoolean(item["isrecheck"]) && !Convert.ToBoolean(item["isverified"]) && !Convert.ToBoolean(item["isreverified"]) && !Convert.ToBoolean(item["isapproved"]))
                        {
                            logger.WriteErrorLogs("Step - 14 ", $"{this.GetType().Name} ==> {System.Reflection.MethodBase.GetCurrentMethod().Name}");
                            data.location = "Checker";
                            data.HandleBy = data.CheckedBy;
                        }
                        else if (Convert.ToBoolean(item["ischeck"]) && !Convert.ToBoolean(item["isrecheck"]) && !Convert.ToBoolean(item["isverified"]) && !Convert.ToBoolean(item["isreverified"]) && !Convert.ToBoolean(item["isapproved"]) && !string.IsNullOrEmpty(data.VerifiedBy))
                        {
                            logger.WriteErrorLogs("Step - 15 ", $"{this.GetType().Name} ==> {System.Reflection.MethodBase.GetCurrentMethod().Name}");
                            data.location = "Verification KIV";
                            data.HandleBy = data.VerifiedBy;
                        }
                        else if (Convert.ToBoolean(item["ischeck"]) && !Convert.ToBoolean(item["isrecheck"]) && !Convert.ToBoolean(item["isverified"]) && !Convert.ToBoolean(item["isreverified"]) && !Convert.ToBoolean(item["isapproved"]))
                        {
                            logger.WriteErrorLogs("Step - 15 ", $"{this.GetType().Name} ==> {System.Reflection.MethodBase.GetCurrentMethod().Name}");
                            data.location = "Verification";
                            data.HandleBy = data.VerifiedBy;
                        }
                        else if (Convert.ToBoolean(item["isrecheck"]) && !Convert.ToBoolean(item["isverified"]) && !Convert.ToBoolean(item["isreverified"]) && !Convert.ToBoolean(item["isapproved"]))
                        {
                            logger.WriteErrorLogs("Step - 16 ", $"{this.GetType().Name} ==> {System.Reflection.MethodBase.GetCurrentMethod().Name}");
                            data.location = "Checker";
                            data.HandleBy = data.CheckedBy;
                        }
                        else if (Convert.ToBoolean(item["ischeck"]) && !Convert.ToBoolean(item["isrecheck"]) && Convert.ToBoolean(item["isverified"]) && !Convert.ToBoolean(item["isreverified"]) && !Convert.ToBoolean(item["isapproved"]))
                        {
                            logger.WriteErrorLogs("Step - 17 ", $"{this.GetType().Name} ==> {System.Reflection.MethodBase.GetCurrentMethod().Name}");
                            data.location = "Approver";
                        }
                        else if (Convert.ToBoolean(item["isrecheck"]) && Convert.ToBoolean(item["isverified"]) && !Convert.ToBoolean(item["isreverified"]) && !Convert.ToBoolean(item["isapproved"]))
                        {
                            logger.WriteErrorLogs("Step - 18 ", $"{this.GetType().Name} ==> {System.Reflection.MethodBase.GetCurrentMethod().Name}");
                            data.location = "Checker";
                            data.HandleBy = data.CheckedBy;
                        }
                        else if (Convert.ToBoolean(item["ischeck"]) && !Convert.ToBoolean(item["isrecheck"]) && Convert.ToBoolean(item["isverified"]) && !Convert.ToBoolean(item["isreverified"]) && !Convert.ToBoolean(item["isapproved"]))
                        {
                            logger.WriteErrorLogs("Step - 19 ", $"{this.GetType().Name} ==> {System.Reflection.MethodBase.GetCurrentMethod().Name}");
                            data.location = "Approver";
                        }
                        else if (Convert.ToBoolean(item["isreverified"]) && !Convert.ToBoolean(item["isapproved"]))
                        {
                            logger.WriteErrorLogs("Step - 20 ", $"{this.GetType().Name} ==> {System.Reflection.MethodBase.GetCurrentMethod().Name}");
                            data.location = "Verification";
                            data.HandleBy = data.VerifiedBy;
                        }
                        else if (Convert.ToBoolean(item["isapproved"]) && !Convert.ToBoolean(item["isfordisbursement"]))
                        {
                            logger.WriteErrorLogs("Step - 21 ", $"{this.GetType().Name} ==> {System.Reflection.MethodBase.GetCurrentMethod().Name}");
                            data.location = "Account Approver List";
                        }
                        else if (Convert.ToBoolean(item["isdisbursedexport"]) && Convert.ToBoolean(item["isfordisbursement"]) && (Convert.ToInt32(item["emi_count"]) > 0) && !Convert.ToBoolean(item["ispretermgenerated"]))
                        {
                            logger.WriteErrorLogs("Step - 24 ", $"{this.GetType().Name} ==> {System.Reflection.MethodBase.GetCurrentMethod().Name}");
                            data.location = "Collector - Loan payment bucket";
                            data.HandleBy = Convert.ToString(item["collectorname"]);
                        }
                        else if (Convert.ToBoolean(item["isdisbursedexport"]) && Convert.ToBoolean(item["isfordisbursement"]) && (Convert.ToInt32(item["emi_count"]) < 1))
                        {
                            logger.WriteErrorLogs("Step - 23 ", $"{this.GetType().Name} ==> {System.Reflection.MethodBase.GetCurrentMethod().Name}");
                            data.location = "EMI";
                        }
                        else if (!Convert.ToBoolean(item["isdisbursedexport"]) && Convert.ToBoolean(item["isfordisbursement"]))
                        {
                            logger.WriteErrorLogs("Step - 22 ", $"{this.GetType().Name} ==> {System.Reflection.MethodBase.GetCurrentMethod().Name}");
                            data.location = "Disbursement Screen";
                        }
                        else
                        {
                            //data.location = "Not Found";
                            logger.WriteErrorLogs($"Search : {model.SearchText}, IsChecked = {Convert.ToString(item["ischeck"])}, IsVerified = {Convert.ToString(item["isverified"])},IsApproved = {Convert.ToString(item["isapproved"])}, Re-IsChecked = {Convert.ToString(item["isrecheck"])}, Re-IsVerified = {Convert.ToString(item["isreverified"])},ApprovedBy = {Convert.ToString(item["approved_by"])},IsApproved = {Convert.ToString(item["isapproved"])},isdefaulter = {Convert.ToString(item["isdefaulter"])},isdeleted = {Convert.ToString(item["isdeleted"])},isfor_fsbucket = {Convert.ToString(item["isfor_fsbucket"])},isinsufficient_doc = {Convert.ToString(item["isinsufficient_doc"])},is_orr_declined = {Convert.ToString(item["is_orr_declined"])},isrecheck = {Convert.ToString(item["isrecheck"])},isreverified = {Convert.ToString(item["isreverified"])}", $"{this.GetType().Name} ==> {System.Reflection.MethodBase.GetCurrentMethod().Name}");

                        }
                        bool _result = (new CommonOperation()).GetIsOrrFalseCheckerApplications(item["sss_no"] == DBNull.Value ? "" : Convert.ToString(item["sss_no"]), item["barangay"] == DBNull.Value ? 0 : Convert.ToInt32(item["barangay"]), item["industry"] == DBNull.Value ? 0 : Convert.ToInt32(item["industry"]));
                        if (_result)
                        {
                            logger.WriteErrorLogs("Step - 25 ", $"{this.GetType().Name} ==> {System.Reflection.MethodBase.GetCurrentMethod().Name}");
                            data.location = "ORR Declined";
                        }
                        var _remarks = DbHelper.SelectMethod(string.Format(QueryHelper.GetRemarkByApplicationId, Convert.ToString(item["applicationno"])));
                        if (_remarks != null && _remarks.Rows.Count > 0)
                        {
                            String _Remark = String.Empty;
                            foreach (DataRow _remarksitem in _remarks.Rows)
                            {
                                _Remark += $"{_remarksitem["createdbyname"]} ({_remarksitem["remarkidentifier"]}) :{_remarksitem["remark"]}\n";
                            }
                            data.ApplicationRemark = _Remark;
                        }
                        _model.applicationStatusDatas.Add(data);
                    }
                }
            }

            return View(_model);
        }
    }
}