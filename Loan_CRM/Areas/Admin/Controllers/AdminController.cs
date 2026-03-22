
using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Loan_CRM.Areas.Admin.Models;
using Loan_CRM.Areas.Checker.Models;
using Loan_CRM.Areas.Collector.Models;
using Loan_CRM.Models;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Data.OleDb;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;


namespace Loan_CRM.Areas.Admin.Controllers
{
    public class AdminController : Controller
    {
        [HttpGet]
        public ActionResult Advisory()
        {
            AdvisoryModel advisoryModel = new AdvisoryModel();
            var query = DbHelper.SelectMethod(QueryHelper.GetAdvisory);
            if (query != null && query.Rows.Count > 0)
            {
                advisoryModel.AdvisoryText = Convert.ToString(query.Rows[0]["advisory_text"]);
            }
            return View(advisoryModel);
        }

        [HttpPost]
        public ActionResult Advisory(AdvisoryModel advisoryModel)
        {
            try
            {
                bool result = false;
                var query = DbHelper.InsertUpdateDelete(string.Format(QueryHelper.UpdateAdvisory, advisoryModel.AdvisoryText));
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
            return View(advisoryModel);
        }

        // GET: Admin/Admin
        public ActionResult Index()
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.AdminUser))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            return View();
        }

        /// <summary>
        /// All Record Export
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult AllRecordExport()
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.AdminUser))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            List<AgingDataReport> model = new List<AgingDataReport>();
            try
            {
                model = GetAgingData();
            }
            catch (Exception ex)
            {
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
            }
            return View(model);
        }
        [HttpPost]
        public ActionResult AllRecordExport(string Search = "")
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.AdminUser))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            try
            {
                List<AgingDataReport> model = GetAgingData();
                if (model.Count > 0)
                {
                    string fileName = string.Empty;
                    fileName = "Aging" + DateTime.Now.ToString("ddMMyyyyHHmmss");
                    var grid = new GridView
                    {
                        DataSource = model
                    };
                    grid.DataBind();
                    Response.ClearContent();
                    Response.Buffer = true;
                    Response.AddHeader("content-disposition", "attachment; filename=" + fileName + ".xls");
                    Response.ContentType = "application/ms-excel";
                    Response.Charset = "";
                    StringWriter sw = new StringWriter();
                    HtmlTextWriter htw = new HtmlTextWriter(sw);
                    grid.RenderControl(htw);
                    Response.Output.Write(sw.ToString());
                    Response.Flush();
                    Response.End();
                    return RedirectToAction("AllRecordExport");
                }
                else
                {
                    TempData["Result"] = "No Record Found to Export";
                    return View();
                }
            }
            catch (Exception ex)
            {
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                return View();
            }
        }
        private AllRecordModel GetAllDataWithDate(string fromDate, string toDate)
        {
            AllRecordModel model = new AllRecordModel();
            try
            {
                DataTable dt = DbHelper.SelectMethod(string.Format(QueryHelper.GetAllRecordDateWise, fromDate, toDate));
                if (dt != null && dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        string Suffix = dt.Rows[i]["suffix"].ToString() != string.Empty ? Convert.ToString(dt.Rows[i]["suffix"]) : string.Empty;
                        Suffix = model.SuffixList.FirstOrDefault(x => x.Value == Suffix).Text;
                        string Name = dt.Rows[i]["name"].ToString() != string.Empty ? Convert.ToString(dt.Rows[i]["name"]) : string.Empty;
                        Name = Suffix + " " + Name;
                        string CityName = CommonMethods.GetCityName(dt.Rows[i]["city"].ToString() != string.Empty ? Convert.ToInt32(dt.Rows[i]["city"]) : -1);
                        string CivilName = CommonMethods.GetCivilStatusName(dt.Rows[i]["civilstatus"].ToString() != string.Empty ? Convert.ToInt32(dt.Rows[i]["civilstatus"]) : -1);
                        string TermType = dt.Rows[i]["termtype"].ToString() != string.Empty ? Convert.ToString(dt.Rows[0]["termtype"]) : string.Empty;
                        TermType = GetTermType(TermType);
                        string TermName = dt.Rows[i]["term"].ToString() != string.Empty ? Convert.ToString(dt.Rows[0]["term"]) : string.Empty;
                        TermName = GetTermName(TermName);
                        string AdditionalRequestTermStr = TermType + ", " + TermName;
                        string DateApplied = dt.Rows[i]["dateapplied"].ToString() != string.Empty ? Convert.ToString(dt.Rows[i]["dateapplied"]) : string.Empty;
                        if (DateApplied != string.Empty)
                        {
                            DateTime dob = Convert.ToDateTime(DateApplied);
                            string dobstr = string.Empty;
                            dobstr = dob.Day + "-" + dob.Month + "-" + dob.Year;
                            DateApplied = dobstr;
                        }

                        string DateBirth = dt.Rows[i]["dateofbirth"].ToString() != string.Empty ? Convert.ToString(dt.Rows[i]["dateofbirth"]) : string.Empty;
                        if (DateBirth != string.Empty)
                        {
                            DateTime dob = Convert.ToDateTime(DateBirth);
                            string dobstr = string.Empty;
                            dobstr = dob.Day + "-" + dob.Month + "-" + dob.Year;
                            DateBirth = dobstr;
                        }

                        string JoiningDate = dt.Rows[i]["date_joining"].ToString() != string.Empty ? Convert.ToString(dt.Rows[i]["date_joining"]) : string.Empty;
                        if (JoiningDate != string.Empty)
                        {
                            DateTime dob = Convert.ToDateTime(JoiningDate);
                            string dobstr = string.Empty;
                            dobstr = dob.Day + "-" + dob.Month + "-" + dob.Year;
                            JoiningDate = dobstr;
                        }

                        model.AllRecordDetails.Add(new AllRecordDetails
                        {
                            name = Name,
                            applicationno = dt.Rows[i]["applicationno"].ToString() != string.Empty ? Convert.ToInt32(dt.Rows[i]["applicationno"]) : 0,
                            dateapplied = DateApplied,
                            personalemail = dt.Rows[i]["personalemail"].ToString() != string.Empty ? Convert.ToString(dt.Rows[i]["personalemail"]) : string.Empty,
                            personalcontactno = dt.Rows[i]["personalcontactno"].ToString() != string.Empty ? Convert.ToString(dt.Rows[i]["personalcontactno"]) : string.Empty,
                            relativecontactno = dt.Rows[i]["relativecontactno"].ToString() != string.Empty ? Convert.ToString(dt.Rows[i]["relativecontactno"]) : string.Empty,
                            coworkercontactno = dt.Rows[i]["coworkercontactno"].ToString() != string.Empty ? Convert.ToString(dt.Rows[i]["coworkercontactno"]) : string.Empty,
                            address = dt.Rows[i]["address"].ToString() != string.Empty ? Convert.ToString(dt.Rows[i]["address"]) : string.Empty,
                            homephoneno = dt.Rows[i]["homephoneno"].ToString() != string.Empty ? Convert.ToString(dt.Rows[i]["homephoneno"]) : string.Empty,
                            placeofbirth = dt.Rows[i]["placeofbirth"].ToString() != string.Empty ? Convert.ToString(dt.Rows[i]["placeofbirth"]) : string.Empty,
                            dateofbirth = DateBirth,
                            CivilStatusName = CivilName,
                            mothermaidenname = dt.Rows[i]["mothermaidenname"].ToString() != string.Empty ? Convert.ToString(dt.Rows[i]["mothermaidenname"]) : string.Empty,
                            motheraddress = dt.Rows[i]["motheraddress"].ToString() != string.Empty ? Convert.ToString(dt.Rows[i]["motheraddress"]) : string.Empty,
                            companyname = dt.Rows[i]["companyname"].ToString() != string.Empty ? Convert.ToString(dt.Rows[i]["companyname"]) : string.Empty,
                            companyaddress = dt.Rows[i]["companyaddress"].ToString() != string.Empty ? Convert.ToString(dt.Rows[i]["companyaddress"]) : string.Empty,
                            designation = dt.Rows[i]["designation"].ToString() != string.Empty ? Convert.ToString(dt.Rows[i]["designation"]) : string.Empty,
                            gross_income = dt.Rows[i]["gross_income"].ToString() != string.Empty ? Convert.ToInt32(dt.Rows[i]["gross_income"]) : 0,
                            reference_name = dt.Rows[i]["reference_name"].ToString() != string.Empty ? Convert.ToString(dt.Rows[i]["reference_name"]) : string.Empty,
                            reference_contactno = dt.Rows[i]["reference_contactno"].ToString() != string.Empty ? Convert.ToString(dt.Rows[i]["reference_contactno"]) : string.Empty,
                            bankname = dt.Rows[i]["bankname"].ToString() != string.Empty ? Convert.ToString(dt.Rows[i]["bankname"]) : string.Empty,
                            bankaccountno = dt.Rows[i]["bankaccountno"].ToString() != string.Empty ? Convert.ToString(dt.Rows[i]["bankaccountno"]) : string.Empty,

                            CityName = CityName,
                            ischeck = dt.Rows[i]["ischeck"].ToString() != string.Empty ? Convert.ToBoolean(dt.Rows[i]["ischeck"]) : false,
                            isverified = dt.Rows[i]["isverified"].ToString() != string.Empty ? Convert.ToBoolean(dt.Rows[i]["isverified"]) : false,
                            isrecheck = dt.Rows[i]["isrecheck"].ToString() != string.Empty ? Convert.ToBoolean(dt.Rows[i]["isrecheck"]) : false,
                            isreverified = dt.Rows[i]["isreverified"].ToString() != string.Empty ? Convert.ToBoolean(dt.Rows[i]["isreverified"]) : false,
                            isapproved = dt.Rows[i]["isapproved"].ToString() != string.Empty ? Convert.ToBoolean(dt.Rows[i]["isapproved"]) : false,
                            relativename = dt.Rows[i]["relativename"].ToString() != string.Empty ? Convert.ToString(dt.Rows[i]["relativename"]) : string.Empty,
                            coworkername = dt.Rows[i]["coworkername"].ToString() != string.Empty ? Convert.ToString(dt.Rows[i]["coworkername"]) : string.Empty,
                            relationwithrelative = dt.Rows[i]["relationwithrelative"].ToString() != string.Empty ? Convert.ToString(dt.Rows[i]["relationwithrelative"]) : string.Empty,
                            friendname = dt.Rows[i]["friendname"].ToString() != string.Empty ? Convert.ToString(dt.Rows[i]["friendname"]) : string.Empty,

                            street = dt.Rows[i]["street"].ToString() != string.Empty ? Convert.ToString(dt.Rows[i]["street"]) : string.Empty,
                            barangay = dt.Rows[i]["barangay"].ToString() != string.Empty ? Convert.ToString(dt.Rows[i]["barangay"]) : string.Empty,
                            zipcode = dt.Rows[i]["zipcode"].ToString() != string.Empty ? Convert.ToString(dt.Rows[i]["zipcode"]) : string.Empty,
                            company_phoneno = dt.Rows[i]["company_phoneno"].ToString() != string.Empty ? Convert.ToString(dt.Rows[i]["company_phoneno"]) : string.Empty,
                            friend_contactno = dt.Rows[i]["friend_contactno"].ToString() != string.Empty ? Convert.ToString(dt.Rows[i]["friend_contactno"]) : string.Empty,
                            date_joining = JoiningDate,
                            purposeofloan = dt.Rows[i]["purposeofloan"].ToString() != string.Empty ? Convert.ToString(dt.Rows[i]["purposeofloan"]) : string.Empty,
                            leadid = dt.Rows[i]["leadid"].ToString() != string.Empty ? Convert.ToInt32(dt.Rows[i]["leadid"]) : 0,
                            paydate1 = dt.Rows[i]["paydate1"].ToString() != string.Empty ? Convert.ToInt32(dt.Rows[i]["paydate1"]) : 0,
                            paydate2 = dt.Rows[i]["paydate2"].ToString() != string.Empty ? Convert.ToInt32(dt.Rows[i]["paydate2"]) : 0,
                            term = dt.Rows[i]["term"].ToString() != string.Empty ? Convert.ToInt32(dt.Rows[i]["term"]) : 0,
                            isfordisbursement = dt.Rows[i]["isfordisbursement"].ToString() != string.Empty ? Convert.ToBoolean(dt.Rows[i]["isfordisbursement"]) : false,
                            isdisbursedexport = dt.Rows[i]["isdisbursedexport"].ToString() != string.Empty ? Convert.ToBoolean(dt.Rows[i]["isdisbursedexport"]) : false,
                            CompleteTerm = AdditionalRequestTermStr
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                throw ex;
            }
            return model;
        }

        /// <summary>
        /// Get TermType
        /// </summary>
        /// <param name="termType"></param>
        /// <returns></returns>
        private string GetTermType(string termType)
        {
            try
            {
                string str = string.Empty;
                if (termType != string.Empty)
                {
                    DataTable dt = DbHelper.SelectMethod(string.Format(QueryHelper.GetTermTypeId, termType));
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        str = Convert.ToString(dt.Rows[0]["termtype"]);
                    }
                }
                return str;
            }
            catch (Exception ex)
            {
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                throw ex;
            }

        }

        /// <summary>
        /// Get TermName
        /// </summary>
        /// <param name="term"></param>
        /// <returns></returns>
        private string GetTermName(string term)
        {
            try
            {

                string str = string.Empty;
                if (term != string.Empty)
                {
                    DataTable dt = DbHelper.SelectMethod(string.Format(QueryHelper.GetTermTypebyId, term));
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        str = Convert.ToString(dt.Rows[0]["term_name"]);
                    }
                }
                return str;
            }
            catch (Exception ex)
            {
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                throw ex;
            }
        }

        /// <summary>
        /// Get AgingData
        /// </summary>
        /// <returns></returns>
        public List<AgingDataReport> GetAgingData()
        {
            List<AgingDataReport> model = new List<AgingDataReport>();
            try
            {
                List<AgingModel> models = new List<AgingModel>();
                DataTable_GenericList<AgingModel> genericList = new DataTable_GenericList<AgingModel>();
                genericList.ReadData(QueryHelper.GetAgingTempData, models, null);


                List<disbursementdetails> disbursementdetails = new List<disbursementdetails>();
                DataTable_GenericList<disbursementdetails> dis = new DataTable_GenericList<disbursementdetails>();
                dis.ReadData(QueryHelper.GetDisbursement, disbursementdetails, null);

                if (models != null && models.Count > 0)
                {
                    var StartDate = models.OrderBy(x => x.disbursement_date).FirstOrDefault().disbursement_date;
                    DataTable data = DbHelper.SelectMethod($"select i::date from generate_series('{StartDate:yyyy-MM-dd}', '{DateTime.Now.AddDays(62):yyyy-MM-dd}', '1 day'::interval) i");
                    foreach (DataRow it in data.Rows)
                    {
                        if (!model.Any(x => x.DueDate == Convert.ToDateTime(it["i"]).ToString("dd-MMM-yyyy")))
                        {
                            var Record = models.FirstOrDefault(x => x.emi_date.ToString("dd-MMM-yyyy") == Convert.ToDateTime(it["i"]).ToString("dd-MMM-yyyy"));
                            if (Record != null)
                            {
                                if (Record.emi_date.Date > DateTime.Now)
                                {
                                    model.Add(new AgingDataReport
                                    {
                                        Age = (DateTime.Now.Date - Record.emi_date.Date).TotalDays.ToString(),
                                        DueDate = Record.emi_date.ToString("dd-MMM-yyyy"),
                                        TotalMaturities = models.Count(x => x.emi_date == Record.emi_date),
                                        TotalMaturities_Peso = models.Where(x => x.emi_date == Record.emi_date).Sum(x => x.emi_amount),
                                        TotalPaid = models.Count(x => x.paidamount > 0 && x.emi_date == Record.emi_date),
                                        TotalPaid_Peso = models.Where(x => x.paidamount > 0 && x.emi_date == Record.emi_date).Sum(x => x.paidamount),
                                        CollectionEfficiency = Convert.ToInt64((Convert.ToDecimal(models.Count(x => x.paidamount > 0 && x.emi_date == Record.emi_date)) / (models.Count(x => x.emi_date == Record.emi_date) == 0 ? Convert.ToDecimal(1) : Convert.ToDecimal(models.Count(x => x.emi_date == Record.emi_date)))) * 100),
                                        CollectionEfficiency_Peso = ((models.Where(x => x.emi_date == Record.emi_date).Sum(x => x.paidamount) / (models.Where(x => x.emi_date == Record.emi_date).Sum(x => x.emi_amount) == 0 ? 1 : models.Where(x => x.emi_date == Record.emi_date).Sum(x => x.emi_amount))) * 100),
                                        Default = 0,
                                        Default_Peso = 0,
                                        Disbus_new_Loan_amount = disbursementdetails.Where(x => x.disbursement_date.Date == Record.emi_date.Date && x.isfor_reloan == false && x.is_preterm == false).Sum(x => x.disbursementamount),
                                        Disbus_new_Loan_number = disbursementdetails.Count(x => x.disbursement_date.Date == Record.emi_date.Date && x.isfor_reloan == false && x.is_preterm == false).ToString(),
                                        Disbus_ReLoan_amount = disbursementdetails.Where(x => x.disbursement_date.Date == Record.emi_date.Date && x.isfor_reloan == true && x.is_preterm == false).Sum(x => x.disbursementamount),
                                        Disbus_ReLoan_number = disbursementdetails.Count(x => x.disbursement_date.Date == Record.emi_date.Date && x.isfor_reloan == true && x.is_preterm == false).ToString()
                                    });
                                }
                                else
                                {
                                    model.Add(new AgingDataReport
                                    {
                                        Age = (DateTime.Now.Date - Record.emi_date.Date).TotalDays.ToString(),
                                        DueDate = Record.emi_date.ToString("dd-MMM-yyyy"),
                                        TotalMaturities = models.Count(x => x.emi_date == Record.emi_date),
                                        TotalMaturities_Peso = models.Where(x => x.emi_date == Record.emi_date).Sum(x => x.emi_amount),
                                        TotalPaid = models.Count(x => x.paidamount > 0 && x.emi_date == Record.emi_date),
                                        TotalPaid_Peso = models.Where(x => x.paidamount > 0 && x.emi_date == Record.emi_date).Sum(x => x.paidamount),
                                        CollectionEfficiency = Convert.ToInt64((Convert.ToDecimal(models.Count(x => x.paidamount > 0 && x.emi_date == Record.emi_date)) / (models.Count(x => x.emi_date == Record.emi_date) == 0 ? Convert.ToDecimal(1) : Convert.ToDecimal(models.Count(x => x.emi_date == Record.emi_date)))) * 100),
                                        CollectionEfficiency_Peso = ((models.Where(x => x.emi_date == Record.emi_date).Sum(x => x.paidamount) / (models.Where(x => x.emi_date == Record.emi_date).Sum(x => x.emi_amount) == 0 ? 1 : models.Where(x => x.emi_date == Record.emi_date).Sum(x => x.emi_amount))) * 100),
                                        Default = ((models.Count(x => x.emi_date == Record.emi_date && x.emi_date.Date < DateTime.Now.Date)) - (models.Count(x => x.paidamount > 0 && x.emi_date == Record.emi_date && x.emi_date.Date < DateTime.Now.Date))),
                                        Default_Peso = ((models.Where(x => x.emi_date == Record.emi_date && x.emi_date.Date < DateTime.Now.Date).Sum(x => x.emi_amount)) - (models.Where(x => x.paidamount > 0 && x.emi_date == Record.emi_date && x.emi_date.Date < DateTime.Now.Date).Sum(x => x.paidamount))),
                                        Disbus_new_Loan_amount = disbursementdetails.Where(x => x.disbursement_date.Date == Record.emi_date.Date && x.isfor_reloan == false && x.is_preterm == false).Sum(x => x.disbursementamount),
                                        Disbus_new_Loan_number = disbursementdetails.Count(x => x.disbursement_date.Date == Record.emi_date.Date && x.isfor_reloan == false && x.is_preterm == false).ToString(),
                                        Disbus_ReLoan_amount = disbursementdetails.Where(x => x.disbursement_date.Date == Record.emi_date.Date && x.isfor_reloan == true && x.is_preterm == false).Sum(x => x.disbursementamount),
                                        Disbus_ReLoan_number = disbursementdetails.Count(x => x.disbursement_date.Date == Record.emi_date.Date && x.isfor_reloan == true && x.is_preterm == false).ToString()
                                    });
                                }

                            }
                            else
                            {
                                model.Add(new AgingDataReport
                                {
                                    Age = (DateTime.Now.Date - Convert.ToDateTime(it["i"])).TotalDays.ToString(),
                                    DueDate = Convert.ToDateTime(it["i"]).ToString("dd-MMM-yyyy"),
                                    TotalMaturities = 0,
                                    TotalMaturities_Peso = 0,
                                    TotalPaid = 0,
                                    TotalPaid_Peso = 0,
                                    CollectionEfficiency = 0,
                                    CollectionEfficiency_Peso = 0,
                                    Default = 0,
                                    Default_Peso = 0,
                                    Disbus_new_Loan_amount = disbursementdetails.Where(x => x.disbursement_date.Date == Convert.ToDateTime(it["i"]).Date.Date && x.isfor_reloan == false && x.is_preterm == false).Sum(x => x.disbursementamount),
                                    Disbus_new_Loan_number = disbursementdetails.Count(x => x.disbursement_date.Date == Convert.ToDateTime(it["i"]).Date.Date && x.isfor_reloan == false && x.is_preterm == false).ToString(),
                                    Disbus_ReLoan_amount = disbursementdetails.Where(x => x.disbursement_date.Date == Convert.ToDateTime(it["i"]).Date.Date && x.isfor_reloan == true && x.is_preterm == false).Sum(x => x.disbursementamount),
                                    Disbus_ReLoan_number = disbursementdetails.Count(x => x.disbursement_date.Date == Convert.ToDateTime(it["i"]).Date.Date && x.isfor_reloan == true && x.is_preterm == false).ToString()
                                });
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
            return model;

        }

        /// <summary>
        /// Term Master 
        /// </summary>
        /// <returns></returns>
        public ActionResult TermMaster()
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.AdminUser))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            return View();
        }

        /// <summary>
        /// save term
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public bool AddTerm(Terms model)
        {
            try
            {
                if (String.IsNullOrWhiteSpace(model.applicabletill))
                {
                    model.applicabletill = DateTime.Now.AddYears(100).ToString("yyyy-MM-dd");
                }
                bool result = false;
                var query = DbHelper.InsertUpdateDelete(string.Format(QueryHelper.InsertTerm, model.term_Id, $"{model.term_Name} term({model.term_Name}x{(model.term_Id == 1 ? "7" : model.term_Id == 2 ? "14" : "28")}days) - {model.suffix}", (Convert.ToInt32(model.term_Name) * (model.term_Id == 1 ? 7 : model.term_Id == 2 ? 14 : 28)), model.Rate, model.late_fee, model.isPublic, model.admin_fee, model.late_penalty, model.applicabletill, model.upto10k));
                if (query > 0)
                {
                    result = true;
                }

                return result;
            }
            catch (Exception ex)
            {
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                throw ex;
            }

        }

        /// <summary>
        /// Get All Terms
        /// </summary>
        /// <returns></returns>
        public ActionResult GetAllTerms()
        {
          
            List<Terms> term = new List<Terms>();
            try
            {

                DataTable dt = new DataTable();
                dt = DbHelper.SelectMethod(QueryHelper.GetAllTerms);
                if (dt != null && dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        term.Add(new Terms
                        {
                            Id = Convert.ToInt32(dt.Rows[i]["id"]),
                            TermTypeName = dt.Rows[i]["termtype"].ToString(),
                            term_Name = dt.Rows[i]["term_name"].ToString(),
                            Rate = Convert.ToDecimal(dt.Rows[i]["interest_rate"]),
                            admin_fee = Convert.ToDecimal(dt.Rows[i]["admin_fee"]),
                            isPublic = Convert.ToBoolean(dt.Rows[i]["ispublic"]),
                            late_fee = Convert.ToDecimal(dt.Rows[i]["late_rate"]),
                            late_penalty = Convert.ToDecimal(dt.Rows[i]["late_penalty"]),
                            applicabletill = Convert.ToDateTime(dt.Rows[i]["applicabletill"]).ToString("dd-MMM-yyyy"),
                            upto10k = Convert.ToBoolean(dt.Rows[i]["upto10k"])
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                term = null;
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
            }
            var jsonResult = Json(term, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        public bool DeleteTerm(string id)
        {
            try
            {
                bool result = false;
                int termId = 0;
                if (!string.IsNullOrWhiteSpace(id))
                {
                    termId = Convert.ToInt32(id);
                }
                int deleteTerm = DbHelper.InsertUpdateDelete(string.Format(QueryHelper.DeleteTerm, termId));
                if (deleteTerm > 0)
                {
                    result = true;
                }

                return result;
            }
            catch (Exception ex)
            {
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                throw ex;
            }

        }

        public bool IsPublicTerm(string id)
        {
            try
            {

                var result = false;
                int termId = 0;
                if (!string.IsNullOrWhiteSpace(id))
                {
                    termId = Convert.ToInt32(id);
                }
                var query = DbHelper.SelectMethod(string.Format(QueryHelper.GetTermByID, termId));
                if (query != null && query.Rows.Count > 0)
                {
                    bool IsPublic = Convert.ToBoolean(query.Rows[0]["ispublic"]);
                    if (IsPublic == true)
                    {
                        int IsPublicTerm = DbHelper.InsertUpdateDelete(string.Format(QueryHelper.IsPublicFalseTerm, termId));
                        if (IsPublicTerm > 0)
                        {
                            result = true;
                        }
                    }
                    else
                    {
                        int IsPublicTerm = DbHelper.InsertUpdateDelete(string.Format(QueryHelper.IsPublicTrueTerm, termId));
                        if (IsPublicTerm > 0)
                        {
                            result = true;
                        }
                    }
                }
                return result;

            }
            catch (Exception ex)
            {
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                throw ex;
            }

        }

        /// <summary>
        /// Delete Duplicate Application
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult DeleteDuplicate()
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.AdminUser))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            return View();
        }

        public ActionResult GetDuplicateApplicationData()
        {
            List<ApplicationRecordVM> lst = new List<ApplicationRecordVM>();
            try
            {
                DataTable dt = DbHelper.SelectMethod(QueryHelper.GetDuplicateApplicationRecord);

                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        ApplicationRecordVM obj = new ApplicationRecordVM
                        {
                            ApplicationNo = Convert.ToString(dt.Rows[i]["applicationno"]) != string.Empty ? Convert.ToInt32(dt.Rows[i]["applicationno"]) : 0,
                            First_Name = dt.Rows[i]["first_name"].ToString() + " " + dt.Rows[i]["middle_name"].ToString() + " " + dt.Rows[i]["last_name"].ToString(),
                            PersonalContactNo = Convert.ToString(dt.Rows[i]["personalcontactno"]) != string.Empty ? Convert.ToString(dt.Rows[i]["personalcontactno"]) : string.Empty,
                            PersonalEmail = Convert.ToString(dt.Rows[i]["personalemail"]) != string.Empty ? Convert.ToString(dt.Rows[i]["personalemail"]) : string.Empty,
                            DateApplied = Convert.ToString(dt.Rows[i]["dateapplied"]) != string.Empty ? Convert.ToString(dt.Rows[i]["dateapplied"]) : string.Empty
                        };
                        lst.Add(obj);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                return Json(null, JsonRequestBehavior.AllowGet);
            }
            var jsonResult = Json(lst, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        /// <summary>
        /// After click on delete icon on orrapplication list
        /// </summary>
        /// <param name="AppId"></param>
        /// <returns></returns>
        public ActionResult DeleteDuplicateRecord(int AppId)
        {
            var flag = false;
            try
            {
                int i = DbHelper.InsertUpdateDelete(string.Format(QueryHelper.DeleteDuplicaterecord, AppId));
                if (i > 0)
                {
                    flag = true;
                }
                else
                {
                    flag = false;
                }
            }
            catch (Exception ex)
            {
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
            }
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteReferenceRecord(int id)
        {
            var flag = false;
            try
            {
                int i = DbHelper.InsertUpdateDelete(string.Format(QueryHelper.DeleteReferenceRecord, id));
                if (i > 0)
                {
                    flag = true;
                }
                else
                {
                    flag = false;
                }
            }
            catch (Exception ex)
            {
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
            }
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeclinedApplication()
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.AdminUser))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            return View();
        }

        /// <summary>
        /// Get Declined Application Data
        /// </summary>
        /// <param name="records"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetDeclinedApplicationData()
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.AdminUser))
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
                List<Checkerdetail> namesList = new List<Checkerdetail>();
                var query = DbHelper.SelectMethod(QueryHelper.GetDeclinedApplicationRecord);
                if (query != null && query.Rows.Count > 0)
                {
                    for (int i = 0; i < query.Rows.Count; i++)
                    {
                        namesList.Add(new Checkerdetail
                        {
                            Id = 1 + Convert.ToInt32(i),
                            applicationname = query.Rows[i]["first_name"].ToString() + " " + query.Rows[i]["middle_name"].ToString() + " " + query.Rows[i]["last_name"].ToString(),
                            applicationno = Convert.ToInt32(query.Rows[i]["applicationno"]),
                            RequestDate = Convert.ToString(query.Rows[i]["dateapplied"]),
                            address = Convert.ToString(query.Rows[i]["address"]),
                            checker_remark = ""
                        });
                    }
                }
                int totalRecords = namesList.Count;

                if (!string.IsNullOrEmpty(search) &&
                !string.IsNullOrWhiteSpace(search))
                {
                    // Apply search 
                    namesList = namesList.Where(p => p.applicationno.ToString().ToLower().Contains(search.ToLower()) ||
                    p.applicationname.ToString().ToLower().Contains(search.ToLower()) ||
                     p.RequestDate.ToString().ToLower().Contains(search.ToLower())).ToList();
                }
                namesList = this.SortByColumnWithOrder(order, orderDir, namesList);
                int recFilter = namesList.Count;
                namesList = namesList.Skip(start).Take(length).ToList();
                foreach (var item in namesList)
                {
                    var _remarks = DbHelper.SelectMethod(string.Format(QueryHelper.GetRemarkByApplicationId, item.applicationno));
                    if (_remarks != null && _remarks.Rows.Count > 0)
                    {
                        String _Remark = String.Empty;
                        foreach (DataRow _remarksitem in _remarks.Rows)
                        {
                            Checkerdetail checkerdetail = namesList.FirstOrDefault(x => x.applicationno == Convert.ToInt64(_remarksitem["application_id"]));
                            if (checkerdetail != null)
                            {
                                namesList.FirstOrDefault(x => x.applicationno == Convert.ToInt64(_remarksitem["application_id"])).checker_remark += $"{_remarksitem["createdbyname"]} ({_remarksitem["remarkidentifier"]}) :{_remarksitem["remark"]}\n";
                            }
                        }
                    }
                }
                result = this.Json(new { draw = Convert.ToInt32(draw), recordsTotal = totalRecords, recordsFiltered = recFilter, data = namesList }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
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

        /// <summary>
        /// Resume DeclinedApplication
        /// </summary>
        /// <param name="AppId"></param>
        /// <returns></returns>
        public ActionResult ResumeDeclinedApplication(int AppId)
        {

            var flag = false;
            try
            {
                int i = DbHelper.InsertUpdateDelete(string.Format(QueryHelper.ResumeDeclinedApp, AppId));
                if (i > 0)
                {
                    flag = true;
                }
            }
            catch (Exception ex)
            {
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
            }
            return Json(flag, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        ///save Bank
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult BankMaster()
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.AdminUser))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            BankMasterModelVM model = new BankMasterModelVM();
            try
            {
                DataTable dtBank = DbHelper.SelectMethod(QueryHelper.GetAllBank);
                if (dtBank != null && dtBank.Rows.Count > 0)
                {
                    for (int i = 0; i < dtBank.Rows.Count; i++)
                    {
                        model.BankMasterModelList.Add(new BankMasterModel
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
            }
            catch (Exception ex)
            {
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult BankMaster(BankMasterModelVM model)
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.AdminUser))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            try
            {
                if (model.bank_id > 0)
                {
                    string DuplicacyQuery = string.Format(QueryHelper.CheckDuplicateBank_forUpdate, model.bank_code.Trim().ToLower(), model.bank_name.Trim().ToLower(), model.bank_id);
                    DataTable dtDuplicate = DbHelper.SelectMethod(DuplicacyQuery);
                    if (dtDuplicate != null && dtDuplicate.Rows.Count > 0)
                    {
                        if (Convert.ToInt32(dtDuplicate.Rows[0][0]) > 0)
                        {
                            TempData["msg"] = "Duplicate Data Found ! Please Change the Bank Name and Bank Code.";
                        }
                        else
                        {
                            int i = DbHelper.InsertUpdateDelete(string.Format(QueryHelper.UpdateBank, model.bank_code, model.bank_name, model.bank_id));
                            if (i > 0)
                            {
                                TempData["msg"] = "Data Updated Successfully!";
                            }
                            else
                            {
                                TempData["msg"] = "Something went wrong while updating the data!";
                            }
                        }
                    }
                    else
                    {
                        TempData["msg"] = "Something went wrong while updating the data!";
                    }
                }
                else
                {
                    string DuplicacyQuery = string.Format(QueryHelper.CheckDuplicateBank_forInsert, model.bank_code.Trim().ToLower(), model.bank_name.Trim().ToLower());
                    DataTable dtDuplicate = DbHelper.SelectMethod(DuplicacyQuery);
                    if (dtDuplicate != null && dtDuplicate.Rows.Count > 0)
                    {
                        if (Convert.ToInt32(dtDuplicate.Rows[0][0]) > 0)
                        {
                            TempData["msg"] = "Duplicate Data Found ! Please Change the Bank Name and Bank Code.";
                        }
                        else
                        {
                            int i = DbHelper.InsertUpdateDelete(string.Format(QueryHelper.InsertBank, model.bank_code, model.bank_name));
                            if (i > 0)
                            {
                                TempData["msg"] = "Data Inserted Successfully!";
                            }
                            else
                            {
                                TempData["msg"] = "Something went wrong while Inserting the data!";
                            }
                        }
                    }
                    else
                    {
                        TempData["msg"] = "Something went wrong while Inserting the data!";
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
            }
            return RedirectToAction("BankMaster", "Admin", new { Area = "Admin" });
        }

        /// <summary>
        /// Edit BankData
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult EditBankData(int Id)
        {

            BankMasterModelVM model = new BankMasterModelVM();
            try
            {
                DataTable dtBank = DbHelper.SelectMethod(string.Format(QueryHelper.GetBankById, Id));
                if (dtBank != null && dtBank.Rows.Count > 0)
                {
                    for (int i = 0; i < dtBank.Rows.Count; i++)
                    {
                        model.BankMasterModelList.Add(new BankMasterModel
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
                return Json(model.BankMasterModelList, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Disable Bank
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DisableBank(int id)
        {

            try
            {
                int i = DbHelper.InsertUpdateDelete(string.Format(QueryHelper.DisableBank, id));
                if (i > 0)
                {
                    TempData["msg"] = "Bank Disabled Successfully";
                    return Json(true, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    TempData["msg"] = "Bank not Disabled Successfully !!";
                    return Json(false, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Enable Bank
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult EnableBank(int id)
        {
            try
            {
                int i = DbHelper.InsertUpdateDelete(string.Format(QueryHelper.EnableBank, id));
                if (i > 0)
                {
                    TempData["msg"] = "Bank Enabled Successfully";
                    return Json(true, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    TempData["msg"] = "Bank not Enabled Successfully !!";
                    return Json(false, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                return Json(false, JsonRequestBehavior.AllowGet);
            }

        }

        /// <summary>
        /// create Occupation
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult OccupationMaster()
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.AdminUser))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            OccupationModelVM model = new OccupationModelVM();
            try
            {
                DataTable dtOccupation = DbHelper.SelectMethod(QueryHelper.GetAllOccupation);
                if (dtOccupation != null && dtOccupation.Rows.Count > 0)
                {
                    for (int i = 0; i < dtOccupation.Rows.Count; i++)
                    {
                        model.OccupationModelList.Add(new OccupationModel
                        {
                            id = Convert.ToInt32(dtOccupation.Rows[i]["id"]),
                            occupation_name = Convert.ToString(dtOccupation.Rows[i]["occupation_name"]),
                            is_orr = Convert.ToBoolean(dtOccupation.Rows[i]["is_orr"]),
                            isactive = Convert.ToBoolean(dtOccupation.Rows[i]["isactive"]),
                            created_on = Convert.ToString(dtOccupation.Rows[i]["created_on"]),
                            updated_on = Convert.ToString(dtOccupation.Rows[i]["updated_on"])
                        });
                    }
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
        public ActionResult OccupationMaster(OccupationModelVM model)
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.AdminUser))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            try
            {
                if (model.id > 0)
                {
                    string DuplicacyQuery = string.Format(QueryHelper.CheckDuplicateOccupation_forUpdate, model.occupation_name.Trim().ToLower(), model.id);
                    DataTable dtDuplicate = DbHelper.SelectMethod(DuplicacyQuery);
                    if (dtDuplicate != null && dtDuplicate.Rows.Count > 0)
                    {
                        if (Convert.ToInt32(dtDuplicate.Rows[0][0]) > 0)
                        {
                            TempData["msg"] = "Duplicate Data Found ! Please Change the Occupation Name !!";
                        }
                        else
                        {
                            int i = DbHelper.InsertUpdateDelete(string.Format(QueryHelper.UpdateOccupation, model.occupation_name, model.id));
                            if (i > 0)
                            {
                                TempData["msg"] = "Data Updated Successfully!";
                            }
                            else
                            {
                                TempData["msg"] = "Something went wrong while updating the data!";
                            }
                        }
                    }
                    else
                    {
                        TempData["msg"] = "Something went wrong while updating the data!";
                    }
                }
                else
                {
                    string DuplicacyQuery = string.Format(QueryHelper.CheckDuplicateOccupation_forInsert, model.occupation_name.Trim().ToLower());
                    DataTable dtDuplicate = DbHelper.SelectMethod(DuplicacyQuery);
                    if (dtDuplicate != null && dtDuplicate.Rows.Count > 0)
                    {
                        if (Convert.ToInt32(dtDuplicate.Rows[0][0]) > 0)
                        {
                            TempData["msg"] = "Duplicate Data Found ! Please Change the Occupation Name !!";
                        }
                        else
                        {
                            int i = DbHelper.InsertUpdateDelete(string.Format(QueryHelper.InsertOccupation, model.occupation_name, model.is_orr));
                            if (i > 0)
                            {
                                TempData["msg"] = "Data Inserted Successfully!";
                            }
                            else
                            {
                                TempData["msg"] = "Something went wrong while Inserting the data!";
                            }
                        }
                    }
                    else
                    {
                        TempData["msg"] = "Something went wrong while Inserting the data!";
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
            }
            return RedirectToAction("OccupationMaster", "Admin", new { Area = "Admin" });
        }

        /// <summary>
        /// Edit OccupationData
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult EditOccupationData(int Id)
        {

            OccupationModelVM model = new OccupationModelVM();
            try
            {
                DataTable dtOccupation = DbHelper.SelectMethod(string.Format(QueryHelper.GetOccupation_ById, Id));
                if (dtOccupation != null && dtOccupation.Rows.Count > 0)
                {
                    for (int i = 0; i < dtOccupation.Rows.Count; i++)
                    {
                        model.OccupationModelList.Add(new OccupationModel
                        {
                            id = Convert.ToInt32(dtOccupation.Rows[i]["id"]),
                            occupation_name = Convert.ToString(dtOccupation.Rows[i]["occupation_name"]),
                            isactive = Convert.ToBoolean(dtOccupation.Rows[i]["isactive"]),
                            created_on = Convert.ToString(dtOccupation.Rows[i]["created_on"]),
                            updated_on = Convert.ToString(dtOccupation.Rows[i]["updated_on"])
                        });
                    }
                }
                return Json(model.OccupationModelList, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";

                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                return Json(null, JsonRequestBehavior.AllowGet);
            }

        }

        /// <summary>
        /// Disable Occupation
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DisableOccupation(int id)
        {

            try
            {
                int i = DbHelper.InsertUpdateDelete(string.Format(QueryHelper.DisableOccupation, id));
                if (i > 0)
                {
                    TempData["msg"] = "Occupation Disabled Successfully";
                }
                else
                {
                    TempData["msg"] = "Occupation not Disabled Successfully !!";
                }
            }
            catch (Exception ex)
            {
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Enable Occupation
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult EnableOccupation(int id)
        {

            try
            {
                int i = DbHelper.InsertUpdateDelete(string.Format(QueryHelper.EnableOccupation, id));
                if (i > 0)
                {
                    TempData["msg"] = "Occupation Enabled Successfully";
                }
                else
                {
                    TempData["msg"] = "Occupation not Enabled Successfully !!";
                }
            }
            catch (Exception ex)
            {
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Disable IsOrr Occupation
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DisableIsOrrOccupation(int id)
        {

            try
            {
                int i = DbHelper.InsertUpdateDelete(string.Format(QueryHelper.DisableIsOrrOccupation, id));
                if (i > 0)
                {
                    TempData["msg"] = "Orr Disabled Successfully";
                }
                else
                {
                    TempData["msg"] = "Not Disabled !!";
                }
            }
            catch (Exception ex)
            {
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Enable IsOrr Occupation
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult EnableIsOrrOccupation(int id)
        {

            try
            {
                int i = DbHelper.InsertUpdateDelete(string.Format(QueryHelper.EnableIsOrrOccupation, id));
                if (i > 0)
                {
                    TempData["msg"] = "ORR Enabled Successfully";
                }
                else
                {
                    TempData["msg"] = "Not Enabled !!";
                }
            }
            catch (Exception ex)
            {
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Create Barangay
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult BarangayMaster()
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.AdminUser))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            BarangayModelVM model = new BarangayModelVM();
            try
            {
                DataTable dtBarangay = DbHelper.SelectMethod(QueryHelper.GetBarangayList);
                DataTable dtCity = DbHelper.SelectMethod(QueryHelper.GetCityList);
                model.CityList.Add(new City() { cityid = 0, cityname = "--Select--" });
                if (dtCity != null && dtCity.Rows.Count > 0)
                {
                    for (int i = 0; i < dtCity.Rows.Count; i++)
                    {
                        model.CityList.Add(new City
                        {
                            cityid = Convert.ToInt32(dtCity.Rows[i]["cityid"]),
                            cityname = Convert.ToString(dtCity.Rows[i]["cityname"])
                        });
                    }
                }
                if (dtBarangay != null && dtBarangay.Rows.Count > 0)
                {
                    for (int i = 0; i < dtBarangay.Rows.Count; i++)
                    {
                        model.BarangayModelList.Add(new BarangayModel
                        {
                            Id = Convert.ToInt32(dtBarangay.Rows[i]["id"]),
                            Barangay_Name = Convert.ToString(dtBarangay.Rows[i]["barangay_name"]),
                            Is_ORR = Convert.ToBoolean(dtBarangay.Rows[i]["is_orr"]),
                            is_active = Convert.ToBoolean(dtBarangay.Rows[i]["isactive"]),
                            created_on = Convert.ToString(dtBarangay.Rows[i]["created_on"]),
                            updated_on = Convert.ToString(dtBarangay.Rows[i]["updated_on"])
                        });
                    }
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
        public ActionResult BarangayMaster(BarangayModelVM model)
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.AdminUser))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            try
            {
                if (model.Id > 0)
                {
                    string DuplicacyQuery = string.Format(QueryHelper.CheckDuplicateBarangay_forUpdate, model.Barangay_Name.Trim().ToLower(), model.Id);
                    DataTable dtDuplicate = DbHelper.SelectMethod(DuplicacyQuery);
                    if (dtDuplicate != null && dtDuplicate.Rows.Count > 0)
                    {
                        if (Convert.ToInt32(dtDuplicate.Rows[0][0]) > 0)
                        {
                            TempData["msg"] = "Duplicate Data Found ! Please Change the Barangay Name !!";
                        }
                        else
                        {
                            int i = DbHelper.InsertUpdateDelete(string.Format(QueryHelper.UpdateBarangay, model.Barangay_Name, model.Id, model.cityId));
                            if (i > 0)
                            {
                                TempData["msg"] = "Data Updated Successfully!";
                            }
                            else
                            {
                                TempData["msg"] = "Something went wrong while updating the data!";
                            }
                        }
                    }
                    else
                    {
                        TempData["msg"] = "Something went wrong while updating the data!";
                    }
                }
                else
                {
                    string DuplicacyQuery = string.Format(QueryHelper.CheckDuplicateBarangay_forInsert, model.Barangay_Name.Trim().ToLower());
                    DataTable dtDuplicate = DbHelper.SelectMethod(DuplicacyQuery);
                    if (dtDuplicate != null && dtDuplicate.Rows.Count > 0)
                    {
                        if (Convert.ToInt32(dtDuplicate.Rows[0][0]) > 0)
                        {
                            TempData["msg"] = "Duplicate Data Found ! Please Change the Barangay Name !!";
                        }
                        else
                        {
                            int i = DbHelper.InsertUpdateDelete(string.Format(QueryHelper.InsertBarangay, model.Barangay_Name, model.Is_ORR, model.cityId));
                            if (i > 0)
                            {
                                TempData["msg"] = "Data Inserted Successfully!";
                            }
                            else
                            {
                                TempData["msg"] = "Something went wrong while Inserting the data!";
                            }
                        }
                    }
                    else
                    {
                        TempData["msg"] = "Something went wrong while Inserting the data!";
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
            }
            return RedirectToAction("BarangayMaster", "Admin", new { Area = "Admin" });
        }

        /// <summary>
        /// Get Barangay details Data
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetBarangayData()
        {

            JsonResult result = new JsonResult();
            try
            {
                List<BarangayModel> barangayModelList = new List<BarangayModel>();
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
                DataTable dtBarangay = DbHelper.SelectMethod(QueryHelper.GetBarangayList);
                if (dtBarangay != null && dtBarangay.Rows.Count > 0)
                {
                    for (int i = 0; i < dtBarangay.Rows.Count; i++)
                    {
                        barangayModelList.Add(new BarangayModel
                        {
                            Id = Convert.ToInt32(dtBarangay.Rows[i]["id"]),
                            Barangay_Name = Convert.ToString(dtBarangay.Rows[i]["barangay_name"]),
                            Is_ORR = Convert.ToBoolean(dtBarangay.Rows[i]["is_orr"]),
                            is_active = Convert.ToBoolean(dtBarangay.Rows[i]["isactive"]),
                            created_on = Convert.ToString(dtBarangay.Rows[i]["created_on"]),
                            updated_on = Convert.ToString(dtBarangay.Rows[i]["updated_on"]),
                            cityname = Convert.ToString(dtBarangay.Rows[i]["cityname"])
                        });
                    }
                }
                int totalRecords = barangayModelList.Count;
                if (!string.IsNullOrEmpty(search) &&
                !string.IsNullOrWhiteSpace(search))
                {
                    // Apply search 
                    barangayModelList = barangayModelList.Where(p => p.Barangay_Name.ToString().ToLower().Contains(search.ToLower()) ||
                    p.created_on.ToLower().Contains(search.ToLower())).ToList();
                }
                barangayModelList = this.SortByColumnWithOrderBarangay(order, orderDir, barangayModelList);
                int recFilter = barangayModelList.Count;
                barangayModelList = barangayModelList.Skip(start).Take(length).ToList<BarangayModel>();
                result = this.Json(new { draw = Convert.ToInt32(draw), recordsTotal = totalRecords, recordsFiltered = recFilter, data = barangayModelList }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                return null;
            }

            return result;
        }

        /// <summary>
        /// Sort Barangay details  By Column
        /// </summary>
        /// <param name="order"></param>
        /// <param name="orderDir"></param>
        /// <param name="barangayModelList"></param>
        /// <returns></returns>
        private List<BarangayModel> SortByColumnWithOrderBarangay(string order, string orderDir, List<BarangayModel> barangayModelList)
        {
            // Initialization. 
            List<BarangayModel> lst = new List<BarangayModel>();
            try
            {
                // Sorting 
                switch (order)
                {
                    case "0":

                        lst = orderDir.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? barangayModelList.OrderByDescending(p => p.Barangay_Name).ToList() : barangayModelList.OrderBy(p => p.Barangay_Name).ToList();
                        break;
                    case "1":

                        lst = orderDir.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? barangayModelList.OrderByDescending(p => p.created_on).ToList() : barangayModelList.OrderBy(p => p.created_on).ToList();
                        break;
                    default:

                        lst = orderDir.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? barangayModelList.OrderByDescending(p => p.created_on).ToList() : barangayModelList.OrderBy(p => p.created_on).ToList();
                        break;
                }
            }
            catch (Exception ex)
            {

                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                throw ex;
            }
            return lst;
        }

        /// <summary>
        /// Edit BarangayData
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult EditBarangayData(int Id)
        {

            BarangayModelVM model = new BarangayModelVM();
            try
            {
                DataTable dtBarangay = DbHelper.SelectMethod(string.Format(QueryHelper.GetBarangay_ById, Id));
                if (dtBarangay != null && dtBarangay.Rows.Count > 0)
                {
                    for (int i = 0; i < dtBarangay.Rows.Count; i++)
                    {
                        model.BarangayModelList.Add(new BarangayModel
                        {
                            Id = Convert.ToInt32(dtBarangay.Rows[i]["id"]),
                            Barangay_Name = Convert.ToString(dtBarangay.Rows[i]["barangay_name"]),
                            is_active = Convert.ToBoolean(dtBarangay.Rows[i]["isactive"]),
                            cityId = Convert.ToInt32(dtBarangay.Rows[i]["city_id"]),
                            created_on = Convert.ToString(dtBarangay.Rows[i]["created_on"]),
                            updated_on = Convert.ToString(dtBarangay.Rows[i]["updated_on"])
                        });
                    }
                }
                return Json(model.BarangayModelList, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";

                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                return Json(null, JsonRequestBehavior.AllowGet);
            }

        }

        /// <summary>
        /// Disable Barangay
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DisableBarangay(int id)
        {

            try
            {
                int i = DbHelper.InsertUpdateDelete(string.Format(QueryHelper.DisableBarangay, id));
                if (i > 0)
                {
                    TempData["msg"] = "Barangay Disabled Successfully";
                }
                else
                {
                    TempData["msg"] = "Barangay not Disabled Successfully !!";
                }
            }
            catch (Exception ex)
            {
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Enable Barangay
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult EnableBarangay(int id)
        {

            try
            {
                int i = DbHelper.InsertUpdateDelete(string.Format(QueryHelper.EnableBarangay, id));
                if (i > 0)
                {
                    TempData["msg"] = "Barangay Enabled Successfully";
                }
                else
                {
                    TempData["msg"] = "Barangay not Enabled Successfully !!";
                }
            }
            catch (Exception ex)
            {
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Disable IsOrr Barangay
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DisableIsOrrBarangay(int id)
        {
            try
            {
                int i = DbHelper.InsertUpdateDelete(string.Format(QueryHelper.DisableIsOrrBarangay, id));
                if (i > 0)
                {
                    TempData["msg"] = "Orr Disabled Successfully";
                }
                else
                {
                    TempData["msg"] = "Not Disabled !!";
                }
            }
            catch (Exception ex)
            {
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Enable IsOrr Barangay
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult EnableIsOrrBarangay(int id)
        {
            try
            {
                int i = DbHelper.InsertUpdateDelete(string.Format(QueryHelper.EnableIsOrrBarangay, id));
                if (i > 0)
                {
                    TempData["msg"] = "ORR Enabled Successfully";
                }
                else
                {
                    TempData["msg"] = "Not Enabled !!";
                }
            }
            catch (Exception ex)
            {
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Create SSSNoMaster
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult SSSNoMaster()
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.AdminUser))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            SSSNoModelVM model = new SSSNoModelVM();
            try
            {
                DataTable dtSSSNo = DbHelper.SelectMethod(QueryHelper.GetSSSNoList);


                if (dtSSSNo != null && dtSSSNo.Rows.Count > 0)
                {
                    for (int i = 0; i < dtSSSNo.Rows.Count; i++)
                    {
                        model.SSSNoModelList.Add(new SSSNoModel
                        {
                            id = Convert.ToInt32(dtSSSNo.Rows[i]["id"]),
                            sss_no = Convert.ToString(dtSSSNo.Rows[i]["sss_no"]),
                            is_orr = Convert.ToBoolean(dtSSSNo.Rows[i]["is_orr"]),
                            isactive = Convert.ToBoolean(dtSSSNo.Rows[i]["isactive"]),
                            created_on = Convert.ToString(dtSSSNo.Rows[i]["created_on"]),
                            updated_on = Convert.ToString(dtSSSNo.Rows[i]["updated_on"])
                        });
                    }
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
        public ActionResult SSSNoMaster(SSSNoModelVM model, string CommandName)
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.AdminUser))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            try
            {
                if (CommandName == "Save" || CommandName == "Update")
                {
                    if (model.id > 0)
                    {
                        string DuplicacyQuery = string.Format(QueryHelper.CheckDuplicateSSSNo_forUpdate, model.sss_no.Trim().ToLower(), model.id);
                        DataTable dtDuplicate = DbHelper.SelectMethod(DuplicacyQuery);
                        if (dtDuplicate != null && dtDuplicate.Rows.Count > 0)
                        {
                            if (Convert.ToInt32(dtDuplicate.Rows[0][0]) > 0)
                            {
                                TempData["msg"] = "Duplicate Data Found ! Please Change SSS No !!";
                            }
                            else
                            {
                                int i = DbHelper.InsertUpdateDelete(string.Format(QueryHelper.UpdateSSSNo, model.sss_no, model.id));
                                if (i > 0)
                                {
                                    TempData["msg"] = "Data Updated Successfully!";
                                }
                                else
                                {
                                    TempData["msg"] = "Something went wrong while updating the data!";
                                }
                            }
                        }
                        else
                        {
                            TempData["msg"] = "Something went wrong while updating the data!";
                        }
                    }
                    else
                    {
                        string DuplicacyQuery = string.Format(QueryHelper.CheckDuplicateSSSNo_forInsert, model.sss_no.Trim().ToLower());
                        DataTable dtDuplicate = DbHelper.SelectMethod(DuplicacyQuery);
                        if (dtDuplicate != null && dtDuplicate.Rows.Count > 0)
                        {
                            if (Convert.ToInt32(dtDuplicate.Rows[0][0]) > 0)
                            {
                                TempData["msg"] = "Duplicate Data Found ! Please Change the SSS No !!";
                            }
                            else
                            {
                                model.is_orr = false;
                                int i = DbHelper.InsertUpdateDelete(string.Format(QueryHelper.InsertSSSNo, model.sss_no, model.is_orr));
                                if (i > 0)
                                {
                                    TempData["msg"] = "Data Inserted Successfully!";
                                }
                                else
                                {
                                    TempData["msg"] = "Something went wrong while Inserting the data!";
                                }
                            }
                        }
                        else
                        {
                            TempData["msg"] = "Something went wrong while Inserting the data!";
                        }
                    }
                }
                else if (CommandName == "Download Format")
                {
                    var path = AppDomain.CurrentDomain.BaseDirectory + "\\Content\\UploadFile\\Format\\Format.xlsx";
                    byte[] fileBytes = System.IO.File.ReadAllBytes(path);
                    string fileName = "Format.xlsx";
                    return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
                }
                else
                {
                    if (Request.Files["FileName"].ContentLength > 0)
                    {
                        string extension = System.IO.Path.GetExtension(Request.Files["FileName"].FileName).ToLower();
                        DataTable dt = new DataTable();
                        int number = 0;
                        string message = "";
                        int k = 0;
                        int duplicate = 0;
                        string connString = "";
                        string[] validFileTypes = { ".xls", ".xlsx" };
                        string path1 = string.Format("{0}/{1}", Server.MapPath("~/Content/UploadFile/SSSNoExcel"), Request.Files["FileName"].FileName);
                        if (!Directory.Exists(path1))
                        {
                            Directory.CreateDirectory(Server.MapPath("~/Content/UploadFile/SSSNoExcel"));
                        }
                        if (validFileTypes.Contains(extension))
                        {
                            if (System.IO.File.Exists(path1))
                            {
                                System.IO.File.Delete(path1);
                            }
                            Request.Files["FileName"].SaveAs(path1);
                            //Connection String to Excel Workbook  
                            if (extension.Trim() == ".xls")
                            {
                                connString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + path1 + ";Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=2\"";
                                dt = ConvertXSLXtoDataTable(connString);

                            }
                            else if (extension.Trim() == ".xlsx")
                            {
                                connString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + path1 + ";Extended Properties=\"Excel 12.0;HDR=YES;IMEX=2\"";
                                dt = ConvertXSLXtoDataTable(connString);

                            }

                            if (dt != null && dt.Rows.Count > 0)
                            {

                                for (int i = 0; i < dt.Rows.Count; i++)
                                {
                                    if (Convert.ToString(dt.Rows[i]["SSSNo"]) != null && Convert.ToString(dt.Rows[i]["SSSNo"]).Trim() != "")
                                    {
                                        if (int.TryParse(Convert.ToString(dt.Rows[i]["SSSNo"]), out number))
                                        {
                                            string DuplicacyQuery = string.Format(QueryHelper.CheckDuplicateSSSNo_forInsert, Convert.ToString(dt.Rows[i]["SSSNo"]).Trim().ToLower());
                                            DataTable dtDuplicate = DbHelper.SelectMethod(DuplicacyQuery);

                                            if (Convert.ToInt32(dtDuplicate.Rows[0][0]) > 0)
                                            {
                                                duplicate++;
                                            }
                                            else
                                            {
                                                k = DbHelper.InsertUpdateDelete(string.Format(QueryHelper.UploadSSSNo, Convert.ToString(dt.Rows[i]["SSSNo"])));
                                            }
                                        }
                                        else
                                        {
                                            if (message != "")
                                            {
                                                message += "," + Convert.ToString(dt.Rows[i]["SSSNo"]);
                                            }
                                            else
                                            {
                                                message = Convert.ToString(dt.Rows[i]["SSSNo"]);
                                            }
                                            message += " are not numeric values in the excel.";
                                        }
                                    }
                                    else
                                    {
                                        message = "the file contains Non Numeric value ";
                                    }
                                }
                            }
                            if (message == "" && k > 0 && duplicate == 0)
                            {
                                TempData["msg"] = "Excel Uploaded Successfully";
                            }
                            else if (k > 0 && duplicate > 0 && message != "")
                            {
                                TempData["msg"] = "Excel Uploaded but " + message + " and DUPLICATE values,which are discarded.";
                            }
                            else if (dt.Rows.Count == duplicate)
                            {
                                TempData["msg"] = "All the values in the excel were duplicate entries.";
                            }
                            else if (duplicate > 0)
                            {
                                TempData["msg"] = "Excel upload but the file contains sone duplicate entries which are discarded.";
                            }
                            else
                            {
                                TempData["msg"] = "Excel Not Uploaded";
                            }
                        }
                        else
                        {
                            TempData["msg"] = "Please Upload Files in .xls or  .xlsx format";

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["msg"] = "Something Went Wrong, Please check the Logs. Also Check the Excel Format";
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
            }
            return RedirectToAction("SSSNoMaster", "Admin", new { Area = "Admin" });
        }

        /// <summary>
        /// Edit SSSNoData
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult EditSSSNoData(int Id)
        {

            SSSNoModelVM model = new SSSNoModelVM();
            try
            {
                DataTable dtSSSNo = DbHelper.SelectMethod(string.Format(QueryHelper.GetSSSNo_ById, Id));
                if (dtSSSNo != null && dtSSSNo.Rows.Count > 0)
                {
                    for (int i = 0; i < dtSSSNo.Rows.Count; i++)
                    {
                        model.SSSNoModelList.Add(new SSSNoModel
                        {
                            id = Convert.ToInt32(dtSSSNo.Rows[i]["id"]),
                            sss_no = Convert.ToString(dtSSSNo.Rows[i]["sss_no"]),
                            is_orr = Convert.ToBoolean(dtSSSNo.Rows[i]["is_orr"]),
                            isactive = Convert.ToBoolean(dtSSSNo.Rows[i]["isactive"]),
                            created_on = Convert.ToString(dtSSSNo.Rows[i]["created_on"]),
                            updated_on = Convert.ToString(dtSSSNo.Rows[i]["updated_on"])
                        });
                    }
                }
                return Json(model.SSSNoModelList, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                return Json(null, JsonRequestBehavior.AllowGet);
            }

        }

        /// <summary>
        /// Disable SSSNo
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DisableSSSNo(int id)
        {
            try
            {
                int i = DbHelper.InsertUpdateDelete(string.Format(QueryHelper.DisableSSSNo, id));
                if (i > 0)
                {
                    TempData["msg"] = "SSS No Disabled Successfully";
                }
                else
                {
                    TempData["msg"] = "SSS No not Disabled Successfully !!";
                }
            }
            catch (Exception ex)
            {
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Enable SSSNo
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult EnableSSSNo(int id)
        {

            try
            {
                int i = DbHelper.InsertUpdateDelete(string.Format(QueryHelper.EnableSSSNo, id));
                if (i > 0)
                {
                    TempData["msg"] = "SSS No Enabled Successfully";
                }
                else
                {
                    TempData["msg"] = "SSS No not Enabled Successfully !!";
                }
            }
            catch (Exception ex)
            {
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Disable IsOrr SSSNo
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DisableIsOrrSSSNo(int id)
        {

            try
            {
                int i = DbHelper.InsertUpdateDelete(string.Format(QueryHelper.DisableIsOrrSSSNo, id));
                if (i > 0)
                {
                    TempData["msg"] = "Orr Disabled Successfully";
                }
                else
                {
                    TempData["msg"] = "Not Disabled !!";
                }
            }
            catch (Exception ex)
            {
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Enable IsOrr SSSNo
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult EnableIsOrrSSSNo(int id)
        {

            try
            {
                int i = DbHelper.InsertUpdateDelete(string.Format(QueryHelper.EnableIsOrrSSSNo, id));
                if (i > 0)
                {
                    TempData["msg"] = "ORR Enabled Successfully";
                }
                else
                {
                    TempData["msg"] = "Not Enabled !!";
                }
            }
            catch (Exception ex)
            {
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// this function using in creating SSS no for orr list
        /// </summary>
        /// <param name="strFilePath"></param>
        /// <param name="connString"></param>
        /// <returns></returns>
        public static DataTable ConvertXSLXtoDataTable(string connString, string SheetName = "Sheet1")
        {
            logger.WriteErrorLogs($"ConnString : {connString}", String.Format("{0} ==> {1}", "ConvertXSLXtoDataTable", System.Reflection.MethodBase.GetCurrentMethod().Name));
            OleDbConnection oledbConn = new OleDbConnection(connString);
            DataTable dt = new DataTable();
            try
            {

                oledbConn.Open();
                using (OleDbCommand cmd = new OleDbCommand($"SELECT * FROM [{SheetName}$]", oledbConn))
                {
                    OleDbDataAdapter oleda = new OleDbDataAdapter
                    {
                        SelectCommand = cmd
                    };
                    oleda.Fill(dt);
                }

            }
            catch (Exception ex)
            {
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("AdminController ==> ConvertXSLXtoDataTable"));
                throw ex;
            }
            finally
            {
                oledbConn.Close();
            }
            return dt;

        }



        /// <summary>
        /// List for get Get IsOrrApllications
        /// </summary>
        /// <returns></returns>
        public List<ApplicationRecordVM> GetIsOrrApllications()
        {
            List<ApplicationRecordVM> model = new List<ApplicationRecordVM>();
            try
            {
                var query = DbHelper.SelectMethod(string.Format(QueryHelper.GetApplicationList));
                if (query.Rows.Count > 0)
                {
                    for (int i = 0; i < query.Rows.Count; i++)
                    {
                        DataTable OrrSSSNo = new DataTable();
                        DataTable OrrOccupation = new DataTable();
                        DataTable OrrBarangay = new DataTable();
                        bool is_orr_sssno = false;
                        bool is_orr_occupation = false;
                        bool is_orr_barangay = false;
                        bool is_orr_declined = false;
                        ApplicationRecordVM record = new ApplicationRecordVM
                        {
                            ApplicationNo = Convert.ToInt32(query.Rows[i]["applicationno"]),
                            DateApplied = Convert.ToString(query.Rows[i]["dateapplied"]),
                            First_Name = Convert.ToString(query.Rows[i]["first_name"]),
                            Middle_Name = Convert.ToString(query.Rows[i]["middle_name"]),
                            Last_Name = Convert.ToString(query.Rows[i]["last_name"]),
                            PersonalEmail = Convert.ToString(query.Rows[i]["personalemail"]),
                            PersonalContactNo = Convert.ToString(query.Rows[i]["personalcontactno"]),
                            SSS_No = Convert.ToString(query.Rows[i]["sss_no"]),
                            Barangay_id = Convert.ToString(query.Rows[i]["barangay"]) != "" ? (Convert.ToInt32(query.Rows[i]["barangay"])) : 0,
                            Occupation = Convert.ToString(query.Rows[i]["industry"]) != "" ? (Convert.ToInt32(query.Rows[i]["industry"])) : 0,
                            checker_remark = Convert.ToString(query.Rows[i]["checker_remark"]) ?? ""
                        };
                        is_orr_declined = Convert.ToString(query.Rows[i]["is_orr_declined"]) != "" ? Convert.ToBoolean(query.Rows[i]["is_orr_declined"]) : false;
                        String _OrrDeclineRemarks = String.Empty;

                        if (record.SSS_No != null && record.SSS_No != "")
                        {
                            string IsOrrSSSNo = string.Format(QueryHelper.IsOrrSSSNo, record.SSS_No);
                            OrrSSSNo = DbHelper.SelectMethod(IsOrrSSSNo);
                        }
                        if (record.Barangay_id != 0)
                        {
                            string IsOrrBarangay = string.Format(QueryHelper.IsOrrBarangay, record.Barangay_id);
                            OrrBarangay = DbHelper.SelectMethod(IsOrrBarangay);
                        }
                        if (record.Occupation != 0)
                        {
                            string IsOrrOccupation = string.Format(QueryHelper.IsOrrOccupation, record.Occupation);
                            OrrOccupation = DbHelper.SelectMethod(IsOrrOccupation);
                        }
                        if ((OrrSSSNo != null && OrrSSSNo.Rows.Count > 0))
                        {
                            if ((Convert.ToInt32(OrrSSSNo.Rows[0][0]) > 0))
                            {
                                is_orr_sssno = true;
                                _OrrDeclineRemarks = "Decline due to SSS No exist into ORR List";
                            }
                        }
                        if ((OrrOccupation != null && OrrOccupation.Rows.Count > 0))
                        {
                            if ((Convert.ToInt32(OrrOccupation.Rows[0][0]) > 0))
                            {
                                is_orr_occupation = true;
                                _OrrDeclineRemarks = "Decline due to Occupation exist into ORR List";
                            }
                        }
                        if ((OrrBarangay != null && OrrBarangay.Rows.Count > 0))
                        {
                            if ((Convert.ToInt32(OrrBarangay.Rows[0][0]) > 0))
                            {
                                is_orr_barangay = true;
                                _OrrDeclineRemarks = "Decline due to Barangay exist into ORR List";
                            }
                        }
                        if (is_orr_declined)
                        {
                            _OrrDeclineRemarks = record.checker_remark;
                        }
                        if (is_orr_occupation || is_orr_sssno || is_orr_barangay || is_orr_declined)
                        {
                            record.checker_remark = _OrrDeclineRemarks;
                            model.Add(record);
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                throw ex;
            }
            return model;

        }

        /// <summary>
        /// Get Filter IsOrr Apllications
        /// </summary>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <returns></returns>
        public List<ApplicationRecordVM> GetFilterIsOrrApllications(DateTime StartDate, DateTime EndDate)
        {
            List<ApplicationRecordVM> model = new List<ApplicationRecordVM>();
            try
            {


                var query = DbHelper.SelectMethod(string.Format(QueryHelper.FilterApplicationList, StartDate.ToString("yyyy-MM-dd hh:mm:ss"), EndDate.ToString("yyyy-MM-dd hh:mm:ss")));
                if (query.Rows.Count > 0)
                {
                    for (int i = 0; i < query.Rows.Count; i++)
                    {
                        DataTable OrrSSSNo = new DataTable();
                        DataTable OrrOccupation = new DataTable();
                        DataTable OrrBarangay = new DataTable();
                        bool is_orr_sssno = false;
                        bool is_orr_occupation = false;
                        bool is_orr_barangay = false;
                        bool is_orr_decline = false;
                        ApplicationRecordVM record = new ApplicationRecordVM
                        {
                            ApplicationNo = Convert.ToInt32(query.Rows[i]["applicationno"]),
                            DateApplied = Convert.ToString(query.Rows[i]["dateapplied"]),
                            First_Name = Convert.ToString(query.Rows[i]["first_name"]),
                            Middle_Name = Convert.ToString(query.Rows[i]["middle_name"]),
                            Last_Name = Convert.ToString(query.Rows[i]["last_name"]),
                            PersonalEmail = Convert.ToString(query.Rows[i]["personalemail"]),
                            PersonalContactNo = Convert.ToString(query.Rows[i]["personalcontactno"]),
                            SSS_No = Convert.ToString(query.Rows[i]["sss_no"]),
                            Barangay_id = Convert.ToString(query.Rows[i]["barangay"]) != "" ? (Convert.ToInt32(query.Rows[i]["barangay"])) : 0,
                            Occupation = Convert.ToString(query.Rows[i]["industry"]) != "" ? (Convert.ToInt32(query.Rows[i]["industry"])) : 0,
                            checker_remark = Convert.ToString(query.Rows[i]["checker_remark"]),
                        };
                        record.ApplicantFullName = $"{record.First_Name} {record.Middle_Name} {record.Last_Name}";
                        is_orr_decline = query.Rows[i]["is_orr_declined"] != System.DBNull.Value ? Convert.ToBoolean(query.Rows[i]["is_orr_declined"]) : false;
                        String _OrrDeclineRemarks = String.Empty;

                        if (record.SSS_No != null && record.SSS_No != "")
                        {
                            string IsOrrSSSNo = string.Format(QueryHelper.IsOrrSSSNo, record.SSS_No);
                            OrrSSSNo = DbHelper.SelectMethod(IsOrrSSSNo);
                        }
                        if (record.Barangay_id != 0)
                        {
                            string IsOrrBarangay = string.Format(QueryHelper.IsOrrBarangay, record.Barangay_id);
                            OrrBarangay = DbHelper.SelectMethod(IsOrrBarangay);
                        }
                        if (record.Occupation != 0)
                        {
                            string IsOrrOccupation = string.Format(QueryHelper.IsOrrOccupation, record.Occupation);
                            OrrOccupation = DbHelper.SelectMethod(IsOrrOccupation);
                        }
                        if ((OrrSSSNo != null && OrrSSSNo.Rows.Count > 0))
                        {
                            if ((Convert.ToInt32(OrrSSSNo.Rows[0][0]) > 0))
                            {
                                is_orr_sssno = true;
                                _OrrDeclineRemarks = "Decline due to SSS No exist into ORR List";
                            }
                        }
                        if ((OrrOccupation != null && OrrOccupation.Rows.Count > 0))
                        {
                            if ((Convert.ToInt32(OrrOccupation.Rows[0][0]) > 0))
                            {
                                is_orr_occupation = true;
                                _OrrDeclineRemarks = "Decline due to Occupation exist into ORR List";
                            }
                        }
                        if ((OrrBarangay != null && OrrBarangay.Rows.Count > 0))
                        {
                            if ((Convert.ToInt32(OrrBarangay.Rows[0][0]) > 0))
                            {
                                is_orr_barangay = true;
                                _OrrDeclineRemarks = "Decline due to Barangay exist into ORR List";
                            }

                        }
                        if (is_orr_decline)
                        {
                            _OrrDeclineRemarks = record.checker_remark;
                        }
                        if (is_orr_occupation || is_orr_sssno || is_orr_barangay || is_orr_decline)
                        {
                            record.checker_remark = _OrrDeclineRemarks;
                            model.Add(record);
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                throw ex;
            }
            return model;
        }

        /// <summary>
        /// Get IsOrrApplication
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult IsOrrApplication()
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.AdminUser))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            ApplicationRecordVM info = new ApplicationRecordVM();
            //try
            //{
            //    info.ApplicationRecordList = GetIsOrrApllications();
            //}
            //catch (Exception ex)
            //{
            //    TempData["msg"] = "Something Went Wrong, Please check the Logs.";
            //    logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
            //}


            return View(info);
        }

        [HttpPost]
        public ActionResult IsOrrApplication(ApplicationRecordVM record)
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.AdminUser))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            try
            {
                if (record.DateRange != null && record.DateRange != "")
                {
                    var split_date = record.DateRange.Replace(" - ", "_").Split('_');
                    DateTime StartDate = Convert.ToDateTime(split_date[0].Trim());
                    DateTime EndDate = Convert.ToDateTime(split_date[1].Trim());

                    ApplicationRecordVM info = new ApplicationRecordVM
                    {
                        ApplicationRecordList = GetFilterIsOrrApllications(StartDate, EndDate)
                    };
                    return View(info);
                }
            }
            catch (Exception ex)
            {
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
            }


            return View(record);
        }

        [HttpPost]
        public ActionResult GetOrrData(string DateRange)
        {

            List<ApplicationRecordVM> FinalList = new List<ApplicationRecordVM>();
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
                DateTime StartDate = DateTime.Now;
                DateTime EndDate = DateTime.Now;
                if (RequestedForm.Keys.Count > 0)
                {
                    search = Request.Form.GetValues("search[value]")[0];
                    start = Convert.ToInt32(Request["start"]);
                    length = Convert.ToInt32(Request["length"]);
                    draw = Request.Form.GetValues("draw")[0];
                    order = Request.Form.GetValues("order[0][column]")[0];
                    orderDir = Request.Form.GetValues("order[0][dir]")[0];
                }
                if (DateRange != null && DateRange != "")
                {
                    var split_date = DateRange.Replace(" - ", "_").Split('_');
                    StartDate = Convert.ToDateTime(split_date[0].Trim());
                    EndDate = Convert.ToDateTime(split_date[1].Trim());

                }
                FinalList = GetFilterIsOrrApllications(StartDate, EndDate);

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

        /// <summary>
        /// Record Transfer to checker
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult TransferToChecker(int count = 0)
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.AdminUser))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            ApplicationRecordVM record = new ApplicationRecordVM();
            try
            {
                if (count == 0)
                    count = 100;

                record.ApplicationRecordList = GetIsOrrApllications().Take(count).ToList();
            }
            catch (Exception ex)
            {
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
            }

            return View(record);
        }

        /// <summary>
        /// Transfer to checker
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult TransferToChecker(ApplicationRecordVM record)
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.AdminUser))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            try
            {
                int count = 0;
                int successful_count = 0;
                foreach (var item in record.ApplicationRecordList)
                {
                    if (item.is_selected == true)
                    {
                        count++;
                        int i = DbHelper.InsertUpdateDelete(string.Format(QueryHelper.TransferToChecker, item.ApplicationNo));
                        if (i > 0)
                            successful_count++;

                    }
                }
                if (count == 0)
                {
                    TempData["msg"] = "No Applications were selected to transfer!";
                }
                else
                {
                    if (count == successful_count)
                    {
                        TempData["msg"] = "Applications Transfered to Checker Successfully";
                    }
                    else
                    {
                        TempData["msg"] = "Applications Not Transfered to Checker Successfully";
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
            }
            return RedirectToAction("TransferToChecker", "Admin");
        }

        /// <summary>
        /// Create EMailTemplate
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult EMailTemplate()
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.AdminUser))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            EmailTemplateModelVM model = new EmailTemplateModelVM();
            try
            {
                DataTable dtemail = DbHelper.SelectMethod(QueryHelper.GetEmailTemplateList);
                if (dtemail != null && dtemail.Rows.Count > 0)
                {
                    for (int i = 0; i < dtemail.Rows.Count; i++)
                    {
                        model.EMailTemplateList.Add(new EmailTemplateModel
                        {
                            id = Convert.ToInt32(dtemail.Rows[i]["id"]),
                            email_template_name = Convert.ToString(dtemail.Rows[i]["email_template_name"]),
                            email_template_definition = Convert.ToString(dtemail.Rows[i]["email_template_definition"]),
                            email_template_description = Convert.ToString(dtemail.Rows[i]["email_description"]),
                            isactive = Convert.ToBoolean(dtemail.Rows[i]["isactive"]),
                            createdon = Convert.ToString(dtemail.Rows[i]["created_on"]),
                            updatedon = Convert.ToString(dtemail.Rows[i]["updated_on"])
                        });
                    }
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
        public ActionResult EMailTemplate(EmailTemplateModelVM model)
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.AdminUser))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            try
            {
                if (model.id > 0)
                {
                    string DuplicacyQuery = string.Format(QueryHelper.CheckDuplicateEmail_forUpdate, model.email_template_name.Trim().ToLower(), model.id);
                    DataTable dtemailDuplicate = DbHelper.SelectMethod(DuplicacyQuery);
                    if (dtemailDuplicate != null && dtemailDuplicate.Rows.Count > 0)
                    {
                        if (Convert.ToInt32(dtemailDuplicate.Rows[0][0]) > 0)
                        {
                            TempData["msg"] = "Duplicate Data Found ! Please Change the Email Template Name !!";
                        }
                        else
                        {
                            int i = DbHelper.InsertUpdateDelete(string.Format(QueryHelper.UpdateEmailTemplate, model.email_template_name, model.id, model.email_template_description, model.email_template_definition));
                            if (i > 0)
                            {
                                TempData["msg"] = "Data Updated Successfully!";
                            }
                            else
                            {
                                TempData["msg"] = "Something went wrong while updating the data!";
                            }
                        }
                    }
                    else
                    {
                        TempData["msg"] = "Something went wrong while updating the data!";
                    }
                }
                else
                {
                    string DuplicacyQuery = string.Format(QueryHelper.CheckDuplicateEmail_forInsert, model.email_template_name.Trim().ToLower());
                    DataTable dtEmailDuplicate = DbHelper.SelectMethod(DuplicacyQuery);
                    if (dtEmailDuplicate != null && dtEmailDuplicate.Rows.Count > 0)
                    {
                        if (Convert.ToInt32(dtEmailDuplicate.Rows[0][0]) > 0)
                        {
                            TempData["msg"] = "Duplicate Data Found ! Please Change the Email template Name !!";
                        }
                        else
                        {
                            int i = DbHelper.InsertUpdateDelete(string.Format(QueryHelper.InsertEmailTemplate, model.email_template_name, model.email_template_description, model.email_template_definition));
                            if (i > 0)
                            {
                                TempData["msg"] = "Data Inserted Successfully!";
                            }
                            else
                            {
                                TempData["msg"] = "Something went wrong while Inserting the data!";

                            }
                        }
                    }
                    else
                    {
                        TempData["msg"] = "Something went wrong while Inserting the data!";
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
            }
            return RedirectToAction("EMailTemplate", "Admin", new { Area = "Admin" });
        }
        /// <summary>
        /// Edit EmailData
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult EditEmailData(int Id)
        {

            EmailTemplateModelVM model = new EmailTemplateModelVM();
            try
            {
                DataTable dtemail = DbHelper.SelectMethod(string.Format(QueryHelper.GetEmail_ById, Id));
                if (dtemail != null && dtemail.Rows.Count > 0)
                {
                    for (int i = 0; i < dtemail.Rows.Count; i++)
                    {
                        model.EMailTemplateList.Add(new EmailTemplateModel
                        {
                            id = Convert.ToInt32(dtemail.Rows[i]["id"]),
                            email_template_name = Convert.ToString(dtemail.Rows[i]["email_template_name"]),
                            email_template_definition = Convert.ToString(dtemail.Rows[i]["email_template_definition"]),
                            email_template_description = Convert.ToString(dtemail.Rows[i]["email_description"]),
                            isactive = Convert.ToBoolean(dtemail.Rows[i]["isactive"]),
                            createdon = Convert.ToString(dtemail.Rows[i]["created_on"]),
                            updatedon = Convert.ToString(dtemail.Rows[i]["updated_on"])
                        });
                    }
                }
                return Json(model.EMailTemplateList, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                return Json(null, JsonRequestBehavior.AllowGet);
            }

        }

        /// <summary>
        /// Create SMSTemplate 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult SMSTemplate()
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.AdminUser))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            SMSTemplateVM model = new SMSTemplateVM();
            try
            {
                DataTable dtsms = DbHelper.SelectMethod(QueryHelper.GetsmsTemplateList);
                if (dtsms != null && dtsms.Rows.Count > 0)
                {
                    for (int i = 0; i < dtsms.Rows.Count; i++)
                    {
                        model.SMSTemplateList.Add(new SMSTemplateVM
                        {
                            id = Convert.ToInt32(dtsms.Rows[i]["id"]),
                            sms_template_name = Convert.ToString(dtsms.Rows[i]["sms_template_name"]),
                            sms_template_definition = Convert.ToString(dtsms.Rows[i]["sms_template_definition"]),
                            sms_template_description = Convert.ToString(dtsms.Rows[i]["sms_description"]),
                            isactive = Convert.ToBoolean(dtsms.Rows[i]["isactive"]),
                            createdon = Convert.ToString(dtsms.Rows[i]["created_on"]),
                            updatedon = Convert.ToString(dtsms.Rows[i]["updated_on"])
                        });
                    }
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
        public ActionResult SMSTemplate(SMSTemplateVM model)
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.AdminUser))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            try
            {
                if (model.id > 0)
                {
                    string DuplicacyQuery = string.Format(QueryHelper.CheckDuplicateSMS_forUpdate, model.sms_template_name.Trim().ToLower(), model.id);
                    DataTable dtsmsDuplicate = DbHelper.SelectMethod(DuplicacyQuery);
                    if (dtsmsDuplicate != null && dtsmsDuplicate.Rows.Count > 0)
                    {
                        if (Convert.ToInt32(dtsmsDuplicate.Rows[0][0]) > 0)
                        {
                            TempData["msg"] = "Duplicate Data Found ! Please Change the SMS Template Name !!";
                        }
                        else
                        {
                            int i = DbHelper.InsertUpdateDelete(string.Format(QueryHelper.UpdateSMSTemplate, model.sms_template_name, model.id, model.sms_template_description, model.sms_template_definition));
                            if (i > 0)
                            {
                                TempData["msg"] = "Data Updated Successfully!";
                            }
                            else
                            {
                                TempData["msg"] = "Something went wrong while updating the data!";
                            }
                        }
                    }
                    else
                    {
                        TempData["msg"] = "Something went wrong while updating the data!";
                    }
                }
                else
                {
                    string DuplicacyQuery = string.Format(QueryHelper.CheckDuplicateSMS_forInsert, model.sms_template_name.Trim().ToLower());
                    DataTable dtDuplicate = DbHelper.SelectMethod(DuplicacyQuery);
                    if (dtDuplicate != null && dtDuplicate.Rows.Count > 0)
                    {
                        if (Convert.ToInt32(dtDuplicate.Rows[0][0]) > 0)
                        {
                            TempData["msg"] = "Duplicate Data Found ! Please Change the SMS template Name !!";
                        }
                        else
                        {
                            int i = DbHelper.InsertUpdateDelete(string.Format(QueryHelper.InsertSMSTemplate, model.sms_template_name, model.sms_template_description, model.sms_template_definition));
                            if (i > 0)
                            {
                                TempData["msg"] = "Data Inserted Successfully!";
                            }
                            else
                            {
                                TempData["msg"] = "Something went wrong while Inserting the data!";
                            }
                        }
                    }
                    else
                    {
                        TempData["msg"] = "Something went wrong while Inserting the data!";
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
            }
            return RedirectToAction("SMSTemplate", "Admin", new { Area = "Admin" });
        }

        /// <summary>
        /// Edit SMSData
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult EditSMSData(int Id)
        {

            SMSTemplateVM model = new SMSTemplateVM();
            try
            {
                DataTable dtSMS = DbHelper.SelectMethod(string.Format(QueryHelper.GetSMS_ById, Id));
                if (dtSMS != null && dtSMS.Rows.Count > 0)
                {
                    for (int i = 0; i < dtSMS.Rows.Count; i++)
                    {
                        model.SMSTemplateList.Add(new SMSTemplate
                        {
                            id = Convert.ToInt32(dtSMS.Rows[i]["id"]),
                            sms_template_name = Convert.ToString(dtSMS.Rows[i]["sms_template_name"]),
                            sms_template_definition = Convert.ToString(dtSMS.Rows[i]["sms_template_definition"]),
                            sms_template_description = Convert.ToString(dtSMS.Rows[i]["sms_description"]),
                            isactive = Convert.ToBoolean(dtSMS.Rows[i]["isactive"]),
                            createdon = Convert.ToString(dtSMS.Rows[i]["created_on"]),
                            updatedon = Convert.ToString(dtSMS.Rows[i]["updated_on"])
                        });
                    }
                }
                return Json(model.SMSTemplateList, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                return Json(model.SMSTemplateList, JsonRequestBehavior.AllowGet);
            }

        }

        /// <summary>
        /// ReassignCheckerLead for Admin
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ReassignCheckerLead()
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.AdminUser))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            ApplicationRecordVM record = new ApplicationRecordVM();
            try
            {

                record.view = 0;

                var query = DbHelper.SelectMethod(QueryHelper.GetCheckerUserList);
                if (query != null && query.Rows.Count > 0)
                {
                    for (int i = 0; i < query.Rows.Count; i++)
                    {
                        ApplicationRecordVM user = new ApplicationRecordVM
                        {
                            Id = Convert.ToInt32(query.Rows[i]["userid"]),
                            userfullname = Convert.ToString(query.Rows[i]["userfullname"])
                        };
                        record.UserList.Add(user);
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
            }

            return View(record);

        }

        [HttpPost]
        public ActionResult ReassignCheckerLead(ApplicationRecordVM model, string CommandName)
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.AdminUser))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            try
            {
                List<ApplicationRecordVM> ApplicationList = new List<ApplicationRecordVM>();
                int j = 0;
                int k = 0;
                var query = DbHelper.SelectMethod(QueryHelper.GetCheckerUserList);
                if (query != null && query.Rows.Count > 0)
                {
                    for (int i = 0; i < query.Rows.Count; i++)
                    {
                        ApplicationRecordVM user = new ApplicationRecordVM
                        {
                            Id = Convert.ToInt32(query.Rows[i]["userid"]),
                            userfullname = Convert.ToString(query.Rows[i]["userfullname"])
                        };
                        model.UserList.Add(user);
                    }
                }
                if (CommandName == "Search")
                {
                    if (model.record_count < 10)
                    {
                        model.record_count = 10;
                    }
                    var arr_FromDate = model.FromDate.Split('-');
                    model.FromDate = arr_FromDate[2] + '-' + arr_FromDate[1] + "-" + arr_FromDate[0];
                    var arr_ToDate = model.ToDate.Split('-');
                    model.ToDate = arr_ToDate[2] + '-' + arr_ToDate[1] + "-" + arr_ToDate[0];

                    var checkerfrom = DbHelper.SelectMethod(string.Format(QueryHelper.CheckerFromList, model.userfrom, model.record_count, model.FromDate, model.ToDate));
                    if (checkerfrom != null && checkerfrom.Rows.Count > 0)
                    {
                        for (int i = 0; i < checkerfrom.Rows.Count; i++)
                        {
                            ApplicationRecordVM record = new ApplicationRecordVM
                            {
                                contract_no = Convert.ToString(checkerfrom.Rows[i]["contract_ref_no"]),
                                reference_no = Convert.ToString(checkerfrom.Rows[i]["reference_no"]),
                                ApplicationNo = Convert.ToInt32(checkerfrom.Rows[i]["applicationno"]),
                                DateApplied = Convert.ToString(checkerfrom.Rows[i]["dateapplied"]),
                                First_Name = Convert.ToString(checkerfrom.Rows[i]["first_name"]),
                                Middle_Name = Convert.ToString(checkerfrom.Rows[i]["middle_name"]),
                                Last_Name = Convert.ToString(checkerfrom.Rows[i]["last_name"]),
                                PersonalEmail = Convert.ToString(checkerfrom.Rows[i]["personalemail"]),
                                PersonalContactNo = Convert.ToString(checkerfrom.Rows[i]["personalcontactno"])
                            };
                            ApplicationList.Add(record);
                        }
                        model.ApplicationRecordList = ApplicationList.ToList();
                        model.view = 1;
                        return View(model);
                    }
                }
                else if (CommandName == "ReAssign")
                {
                    foreach (var item in model.ApplicationRecordList)
                    {
                        if (item.is_selected)
                        {
                            k++;
                            int i = DbHelper.InsertUpdateDelete(string.Format(QueryHelper.CheckerReAssign, model.userfrom, model.userto, item.ApplicationNo));
                            if (i > 0)
                            {
                                j++;
                            }
                        }
                    }

                    if (k == 0)
                    {
                        TempData["msg"] = "No Applications were Selected!";
                    }
                    else
                    {
                        if (j == k)
                        {
                            TempData["msg"] = "Checker Lead Reassigned Successfully";
                        }
                        else
                        {
                            TempData["msg"] = "UnSuccessfully";
                        }
                    }

                    return RedirectToAction("ReAssignCheckerLead", "Admin");
                }
            }
            catch (Exception ex)
            {
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
            }

            return RedirectToAction("ReAssignCheckerLead", "Admin");
        }

        /// <summary>
        /// Reassign VerifierLead for Admin
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ReassignVerifierLead()
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.AdminUser))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            ApplicationRecordVM record = new ApplicationRecordVM();
            try
            {

                record.view = 0;
                var query = DbHelper.SelectMethod(QueryHelper.GetVerifierUserList);
                if (query != null && query.Rows.Count > 0)
                {
                    for (int i = 0; i < query.Rows.Count; i++)
                    {
                        ApplicationRecordVM user = new ApplicationRecordVM
                        {
                            Id = Convert.ToInt32(query.Rows[i]["userid"]),
                            userfullname = Convert.ToString(query.Rows[i]["userfullname"])
                        };
                        record.UserList.Add(user);
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
            }

            return View(record);

        }

        [HttpPost]
        public ActionResult ReassignVerifierLead(ApplicationRecordVM model, string CommandName)
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.AdminUser))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            try
            {
                List<ApplicationRecordVM> ApplicationList = new List<ApplicationRecordVM>();
                int j = 0;
                int k = 0;
                var query = DbHelper.SelectMethod(QueryHelper.GetVerifierUserList);
                if (query != null && query.Rows.Count > 0)
                {
                    for (int i = 0; i < query.Rows.Count; i++)
                    {
                        ApplicationRecordVM user = new ApplicationRecordVM
                        {
                            Id = Convert.ToInt32(query.Rows[i]["userid"]),
                            userfullname = Convert.ToString(query.Rows[i]["userfullname"])
                        };
                        model.UserList.Add(user);
                    }
                }
                if (CommandName == "Search")
                {
                    if (model.record_count < 10)
                    {
                        model.record_count = 10;
                    }
                    var arr_FromDate = model.FromDate.Split('-');
                    model.FromDate = arr_FromDate[2] + '-' + arr_FromDate[1] + "-" + arr_FromDate[0];
                    var arr_ToDate = model.ToDate.Split('-');
                    model.ToDate = arr_ToDate[2] + '-' + arr_ToDate[1] + "-" + arr_ToDate[0];

                    var checkerfrom = DbHelper.SelectMethod(string.Format(QueryHelper.VerifierFromList, model.userfrom, model.record_count, model.FromDate, model.ToDate));
                    if (checkerfrom != null && checkerfrom.Rows.Count > 0)
                    {
                        for (int i = 0; i < checkerfrom.Rows.Count; i++)
                        {
                            ApplicationRecordVM record = new ApplicationRecordVM
                            {
                                contract_no = Convert.ToString(checkerfrom.Rows[i]["contract_ref_no"]),
                                reference_no = Convert.ToString(checkerfrom.Rows[i]["reference_no"]),
                                ApplicationNo = Convert.ToInt32(checkerfrom.Rows[i]["applicationno"]),
                                DateApplied = Convert.ToString(checkerfrom.Rows[i]["dateapplied"]),
                                First_Name = Convert.ToString(checkerfrom.Rows[i]["first_name"]),
                                Middle_Name = Convert.ToString(checkerfrom.Rows[i]["middle_name"]),
                                Last_Name = Convert.ToString(checkerfrom.Rows[i]["last_name"]),
                                PersonalEmail = Convert.ToString(checkerfrom.Rows[i]["personalemail"]),
                                PersonalContactNo = Convert.ToString(checkerfrom.Rows[i]["personalcontactno"])
                            };
                            ApplicationList.Add(record);
                        }
                        model.ApplicationRecordList = ApplicationList.ToList();
                        model.view = 1;
                        return View(model);
                    }
                }
                else if (CommandName == "ReAssign")
                {
                    foreach (var item in model.ApplicationRecordList)
                    {
                        if (item.is_selected)
                        {
                            k++;
                            int i = DbHelper.InsertUpdateDelete(string.Format(QueryHelper.VerifierReAssign, model.userfrom, model.userto, item.ApplicationNo));
                            if (i > 0)
                            {
                                j++;
                            }
                        }
                    }
                    if (k == 0)
                    {
                        TempData["msg"] = "No Applications were Selected!";
                    }
                    else
                    {
                        if (j == k)
                        {
                            TempData["msg"] = "Verifier Lead Reassigned Successfully";
                        }
                        else
                        {
                            TempData["msg"] = "UnSuccessfully";
                        }
                    }

                    return RedirectToAction("ReassignVerifierLead", "Admin", new { Area = "Admin" });
                }
            }
            catch (Exception ex)
            {
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
            }

            return RedirectToAction("ReassignVerifierLead", "Admin", new { Area = "Admin" });
        }

        /// <summary>
        /// lead Transfer With Departments
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult TransferWithDepartments()
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.AdminUser))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            ApplicationRecordVM record = new ApplicationRecordVM();
            try
            {

                record.view = 0;
            }
            catch (Exception ex)
            {
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
            }

            return View(record);

        }
        /// <summary>
        /// Lead Transfer With Departments
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult TransferWithDepartments(ApplicationRecordVM model, string CommandName)
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.AdminUser))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            try
            {
                List<ApplicationRecordVM> ApplicationList = new List<ApplicationRecordVM>();
                int j = 0;
                int k = 0;

                if (CommandName == "Search")
                {
                    if (model.userfrom == 0)
                    {
                        CommonOperation comnoperation1 = new CommonOperation();
                        var users = comnoperation1.GetBucketKIVRecheckerList();
                        var checkerfrom = comnoperation1.GetIsOrrFalseCheckerApplications(users);

                        if (checkerfrom != null && checkerfrom.Count > 0)
                        {
                            foreach (var item in checkerfrom)
                            {

                                ApplicationRecordVM record = new ApplicationRecordVM
                                {
                                    ApplicationNo = Convert.ToInt32(item.applicationno),
                                    DateApplied = item.RequestDate,
                                    First_Name = item.applicationname,

                                    PersonalEmail = item.personalemail,
                                    PersonalContactNo = item.personalcontactno
                                };
                                ApplicationList.Add(record);
                            }
                        }
                        model.ApplicationRecordList = ApplicationList.Take(500).ToList();
                        model.view = 1;
                        return View(model);
                    }

                    else if (model.userfrom == 1)
                    {
                        var checkerfrom = DbHelper.SelectMethod(string.Format(QueryHelper.GetApplicationListVerifierKIVReVerifier));
                        if (checkerfrom != null && checkerfrom.Rows.Count > 0)
                        {
                            for (int i = 0; i < checkerfrom.Rows.Count; i++)
                            {
                                ApplicationRecordVM record = new ApplicationRecordVM
                                {
                                    ApplicationNo = Convert.ToInt32(checkerfrom.Rows[i]["applicationno"]),
                                    DateApplied = Convert.ToString(checkerfrom.Rows[i]["dateapplied"]),
                                    First_Name = Convert.ToString(checkerfrom.Rows[i]["first_name"]),
                                    Middle_Name = Convert.ToString(checkerfrom.Rows[i]["middle_name"]),
                                    Last_Name = Convert.ToString(checkerfrom.Rows[i]["last_name"]),
                                    PersonalEmail = Convert.ToString(checkerfrom.Rows[i]["personalemail"]),
                                    PersonalContactNo = Convert.ToString(checkerfrom.Rows[i]["personalcontactno"])
                                };
                                ApplicationList.Add(record);
                            }
                            model.ApplicationRecordList = ApplicationList.Take(500).ToList();
                            model.view = 1;
                            return View(model);
                        }
                    }
                    else
                    {
                        ApplicationRecordVM Record = new ApplicationRecordVM();
                        DataTable dt;
                        dt = DbHelper.SelectMethod(QueryHelper.GetApplicationforApprover);
                        if (dt != null && dt.Rows.Count > 0)
                        {
                            for (int i = 0; i < dt.Rows.Count; i++)
                            {
                                ApplicationRecordVM record = new ApplicationRecordVM
                                {
                                    ApplicationNo = Convert.ToInt32(dt.Rows[i]["applicationno"]),
                                    DateApplied = Convert.ToString(dt.Rows[i]["dateapplied"]),
                                    First_Name = Convert.ToString(dt.Rows[i]["first_name"]),
                                    Middle_Name = Convert.ToString(dt.Rows[i]["middle_name"]),
                                    Last_Name = Convert.ToString(dt.Rows[i]["last_name"]),
                                    PersonalEmail = Convert.ToString(dt.Rows[i]["personalemail"]),
                                    PersonalContactNo = Convert.ToString(dt.Rows[i]["personalcontactno"])
                                };
                                ApplicationList.Add(record);
                            }
                        }
                        model.ApplicationRecordList = ApplicationList.Take(500).ToList();
                        model.view = 1;
                        return View(model);
                    }

                }
                else if (CommandName == "ReAssign")
                {
                    if (model.userto == 0)
                    {
                        foreach (var item in model.ApplicationRecordList)
                        {
                            if (item.is_selected)
                            {
                                k++;
                                int i = DbHelper.InsertUpdateDelete(string.Format(QueryHelper.CheckerTransfer, item.ApplicationNo));
                                if (i > 0)
                                {
                                    j++;
                                }
                            }
                        }
                    }
                    else if (model.userto == 1)
                    {
                        int Id = Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value);
                        foreach (var item in model.ApplicationRecordList)
                        {
                            if (item.is_selected)
                            {
                                k++;
                                int i = DbHelper.InsertUpdateDelete(string.Format(QueryHelper.VerifierTransfer, item.ApplicationNo, Id));
                                if (i > 0)
                                {
                                    j++;
                                }
                            }
                        }
                    }
                    else
                    {
                        int Id = Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value);
                        foreach (var item in model.ApplicationRecordList)
                        {
                            if (item.is_selected)
                            {
                                k++;
                                int i = DbHelper.InsertUpdateDelete(string.Format(QueryHelper.ApproverTransfer, item.ApplicationNo, Id));
                                if (i > 0)
                                {
                                    j++;
                                }
                            }
                        }
                    }
                    if (k == 0)
                    {
                        TempData["msg"] = "No Applications were Selected!";
                    }
                    else
                    {
                        if (j == k)
                        {
                            TempData["msg"] = "Transfered Successfully";
                        }
                        else
                        {
                            TempData["msg"] = "UnSuccessfully";
                        }
                    }
                    return RedirectToAction("TransferWithDepartments", "Admin");
                }
            }
            catch (Exception ex)
            {
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
            }

            return RedirectToAction("TransferWithDepartments", "Admin");
        }

        /// <summary>
        /// Edit Customer Details
        /// </summary>
        /// <param name="applicationNo"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult EditCustomerInfo(int applicationNo)
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.AdminUser))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            try
            {
                ApplicationRecordVM appRecordVM = new ApplicationRecordVM();
                ViewBag.CityData = CommonMethods.getCity();
                ViewBag.CivilStatusData = CommonMethods.getCivilStatus();
                ViewBag.TermTypeData = CommonMethods.getTermType();
                ViewBag.IndustryData = CommonMethods.getOccupation();
                DataTable dt = DbHelper.SelectMethod(string.Format(QueryHelper.GetApplicationdetails_forAdmin, applicationNo));
                if (dt != null && dt.Rows.Count > 0)
                {
                    appRecordVM.ApplicationNo = applicationNo;
                    appRecordVM.First_Name = dt.Rows[0]["first_name"].ToString() != string.Empty ? dt.Rows[0]["first_name"].ToString() : string.Empty;
                    appRecordVM.Middle_Name = dt.Rows[0]["middle_name"].ToString() != string.Empty ? dt.Rows[0]["middle_name"].ToString() : string.Empty;
                    appRecordVM.Last_Name = dt.Rows[0]["last_name"].ToString() != string.Empty ? dt.Rows[0]["last_name"].ToString() : string.Empty;
                    appRecordVM.PersonalEmail = dt.Rows[0]["personalemail"].ToString() != string.Empty ? dt.Rows[0]["personalemail"].ToString() : string.Empty;
                    appRecordVM.Personal_ContactNo = dt.Rows[0]["personalcontactno"].ToString() != string.Empty ? dt.Rows[0]["personalcontactno"].ToString() : string.Empty;
                    if (!String.IsNullOrWhiteSpace(appRecordVM.CompletePersonalContactNo))
                    {
                        string n = string.Empty;
                        n = appRecordVM.CompletePersonalContactNo.Substring(0, 2);
                        if (n == "09" || n == "08")
                        {
                            string NewNumber = string.Empty;
                            NewNumber = appRecordVM.CompletePersonalContactNo.Substring(2);
                            appRecordVM.CompletePersonalContactNo = NewNumber;
                            appRecordVM.ContactPrefix = n;
                        }
                    }


                    appRecordVM.HomePhone = dt.Rows[0]["homephoneno"].ToString() != string.Empty ? dt.Rows[0]["homephoneno"].ToString() : string.Empty;
                    appRecordVM.Address = dt.Rows[0]["address"].ToString() != string.Empty ? dt.Rows[0]["address"].ToString() : string.Empty;
                    appRecordVM.Province = dt.Rows[0]["province_name"].ToString() != string.Empty ? dt.Rows[0]["province_name"].ToString() : string.Empty;
                    appRecordVM.province_id = dt.Rows[0]["province_id"].ToString() != string.Empty ? Convert.ToInt32(dt.Rows[0]["province_id"]) : 0;
                    appRecordVM.Barangay = dt.Rows[0]["barangay_name"].ToString() != string.Empty ? dt.Rows[0]["barangay_name"].ToString() : string.Empty;
                    appRecordVM.Barangay_id = dt.Rows[0]["id"].ToString() != string.Empty ? Convert.ToInt32(dt.Rows[0]["id"]) : 0;
                    appRecordVM.CompanyName = dt.Rows[0]["companyname"].ToString() != string.Empty ? dt.Rows[0]["companyname"].ToString() : string.Empty;
                    appRecordVM.Designation = dt.Rows[0]["designation"].ToString() != string.Empty ? dt.Rows[0]["designation"].ToString() : string.Empty;
                    appRecordVM.BankAccNo = dt.Rows[0]["bankaccountno"].ToString() != string.Empty ? dt.Rows[0]["bankaccountno"].ToString() : string.Empty;
                    appRecordVM.BankName = dt.Rows[0]["bankname"].ToString() != string.Empty ? dt.Rows[0]["bankname"].ToString() : string.Empty;
                    appRecordVM.DOB = Convert.ToString(dt.Rows[0]["dateofbirth"]) != string.Empty ? Convert.ToString(dt.Rows[0]["dateofbirth"]) : string.Empty;
                    appRecordVM.CivilStatus = dt.Rows[0]["civilstatus"].ToString() != string.Empty ? Convert.ToInt32(dt.Rows[0]["civilstatus"].ToString()) : -1;
                    appRecordVM.CompanyAddress = dt.Rows[0]["companyaddress"].ToString() != string.Empty ? dt.Rows[0]["companyaddress"].ToString() : string.Empty;
                    appRecordVM.GrossIncome = dt.Rows[0]["gross_income"].ToString() != string.Empty ? Convert.ToDouble(dt.Rows[0]["gross_income"].ToString()) : 0.00D;
                    appRecordVM.NetIncome = dt.Rows[0]["net_monthly_income"].ToString() != string.Empty ? Convert.ToDouble(dt.Rows[0]["net_monthly_income"].ToString()) : 0.00D;
                    appRecordVM.City_Name = dt.Rows[0]["cityname"].ToString() != string.Empty ? dt.Rows[0]["cityname"].ToString() : string.Empty;
                    appRecordVM.City = dt.Rows[0]["cityid"].ToString() != string.Empty ? Convert.ToInt32(dt.Rows[0]["cityid"]) : 0;
                    appRecordVM.PayDate1 = Convert.ToString(dt.Rows[0]["paydate1"]) != string.Empty ? Convert.ToInt32(dt.Rows[0]["paydate1"]) : -1;
                    appRecordVM.PayDate2 = Convert.ToString(dt.Rows[0]["paydate2"]) != string.Empty ? Convert.ToInt32(dt.Rows[0]["paydate2"]) : -1;
                    //Best Time to Morning Call
                    appRecordVM.BestTimeToCallMorning_Id = Convert.ToString(dt.Rows[0]["morning_time"]).Trim() == string.Empty ? -1 : Convert.ToInt32(dt.Rows[0]["morning_time"]);
                    //Best Time to Noon Call
                    appRecordVM.BestTimeToCallNoon_Id = Convert.ToString(dt.Rows[0]["noon_time"]).Trim() == string.Empty ? -1 : Convert.ToInt32(dt.Rows[0]["noon_time"]);
                    DataTable dtBank = DbHelper.SelectMethod(QueryHelper.GetAllBank);
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

                    ViewData["NoRecordMsg"] = "Record Found";
                }
                else
                    TempData["Result"] = "No Records Found";
                ViewBag.CityData = CommonMethods.getCity();
                ViewBag.CivilStatusData = CommonMethods.getCivilStatus();
                return View(appRecordVM);
            }
            catch (Exception ex)
            {
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                return RedirectToAction("CustomerInfoBucket");
            }

        }
        [HttpPost]
        public ActionResult EditCustomerInfo(ApplicationRecordVM model)
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.AdminUser))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            try
            {
                ViewBag.CityData = CommonMethods.getCity();
                ViewBag.CivilStatusData = CommonMethods.getCivilStatus();
                ViewBag.TermTypeData = CommonMethods.getTermType();
                ViewBag.IndustryData = CommonMethods.getOccupation();
                model.CompletePersonalContactNo = model.ContactPrefix + model.CompletePersonalContactNo;
                int a = DbHelper.InsertUpdateDelete(string.Format(QueryHelper.UpdateInsufficientDocument, model.ApplicationNo, model.BestTimeToCallMorning_Id, model.BestTimeToCallNoon_Id));
                int i = DbHelper.InsertUpdateDelete(string.Format(QueryHelper.UpdateCustomer_Record_OnUpdate, model.Address, model.City, Convert.ToDateTime(model.DOB).ToString("yyyy-MM-dd"), model.CivilStatus, model.CompanyName, model.CompanyAddress, model.Designation, model.GrossIncome, model.PayDate1, model.PayDate2, model.HomePhone, model.BankName, model.BankAccNo, model.Barangay_id, model.ApplicationNo, model.province_id, model.NetIncome));
                if (i > 0)
                {
                    DbHelper.InsertUpdateDelete(String.Format(QueryHelper.UpdateUserDetails, model.ApplicationNo, model.First_Name, model.Middle_Name, model.Last_Name));
                    TempData["msg"] = "Your records are successfully updated!";
                }
                else
                {
                    TempData["msg"] = "Your records are not updated!";
                }
            }
            catch (Exception ex)
            {
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                return Redirect("EditCustomerInfo?applicationNo=" + model.ApplicationNo + "");
            }
            return Redirect("EditCustomerInfo?applicationNo=" + model.ApplicationNo + "");
        }

        /// <summary>
        /// create Customer Info Bucket
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult CustomerInfoBucket()
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.AdminUser))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            ApplicationRecordVM record = new ApplicationRecordVM();
            try
            { }
            catch (Exception ex)
            {
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
            }
            return View(record);
        }

        /// <summary>
        /// Get Customer details Data
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetCustomerData()
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.AdminUser))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            JsonResult result = new JsonResult();
            try
            {
                List<ApplicationRecordVM> ApplicationRecordList = new List<ApplicationRecordVM>();
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
                ApplicationRecordList = CommonMethods.GetUserData();
                int totalRecords = ApplicationRecordList.Count;
                if (!string.IsNullOrEmpty(search) &&
                !string.IsNullOrWhiteSpace(search))
                {
                    // Apply search 
                    ApplicationRecordList = ApplicationRecordList.Where(p => p.ApplicationNo.ToString().ToLower().Contains(search.ToLower()) ||
                    p.DateApplied.ToLower().Contains(search.ToLower()) ||
                    p.First_Name.ToString().ToLower().Contains(search.ToLower()) ||
                    p.Middle_Name.ToLower().Contains(search.ToLower()) ||
                    p.Last_Name.ToLower().Contains(search.ToLower()) ||
                    p.PersonalEmail.ToString().ToLower().Contains(search.ToLower()) ||
                    p.PersonalContactNo.ToString().ToLower().Contains(search.ToLower())).ToList();
                }
                ApplicationRecordList = this.SortByColumnWithOrder(order, orderDir, ApplicationRecordList);
                int recFilter = ApplicationRecordList.Count;
                ApplicationRecordList = ApplicationRecordList.Skip(start).Take(length).ToList<ApplicationRecordVM>();
                result = this.Json(new { draw = Convert.ToInt32(draw), recordsTotal = totalRecords, recordsFiltered = recFilter, data = ApplicationRecordList }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                return null;
            }

            return result;
        }

        /// <summary>
        /// Sort customer details  By Column
        /// </summary>
        /// <param name="order"></param>
        /// <param name="orderDir"></param>
        /// <param name="ApplicationRecordList"></param>
        /// <returns></returns>
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

                        lst = orderDir.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? ApplicationRecordList.OrderByDescending(p => p.ApplicationNo).ToList() : ApplicationRecordList.OrderBy(p => p.ApplicationNo).ToList();
                        break;
                    case "1":

                        lst = orderDir.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? ApplicationRecordList.OrderByDescending(p => p.DateApplied).ToList() : ApplicationRecordList.OrderBy(p => p.DateApplied).ToList();
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

                        lst = orderDir.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? ApplicationRecordList.OrderByDescending(p => p.PersonalEmail).ToList() : ApplicationRecordList.OrderBy(p => p.PersonalEmail).ToList();
                        break;
                    case "6":

                        lst = orderDir.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? ApplicationRecordList.OrderByDescending(p => p.PersonalContactNo).ToList() : ApplicationRecordList.OrderBy(p => p.PersonalContactNo).ToList();
                        break;
                    default:

                        lst = orderDir.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? ApplicationRecordList.OrderByDescending(p => p.ApplicationNo).ToList() : ApplicationRecordList.OrderBy(p => p.ApplicationNo).ToList();
                        break;
                }
            }
            catch (Exception ex)
            {

                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                throw ex;
            }
            return lst;
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
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.AdminUser))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
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
        ///  Get province
        /// </summary>
        /// <param name="Prefix"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Province(string Prefix)
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.AdminUser))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
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
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.AdminUser))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
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
        /// Reason for Non Payment 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult NonPaymentReason()
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.AdminUser))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            NonPaymentReasonModelVM model = new NonPaymentReasonModelVM();
            try
            {
                DataTable dtNonReasonOfPayment = DbHelper.SelectMethod(QueryHelper.GetAllNonReasonOfPayment);
                if (dtNonReasonOfPayment != null && dtNonReasonOfPayment.Rows.Count > 0)
                {
                    for (int i = 0; i < dtNonReasonOfPayment.Rows.Count; i++)
                    {
                        model.NonPaymentReasonModelList.Add(new NonPaymentReasonModel
                        {
                            reason_id = Convert.ToInt32(dtNonReasonOfPayment.Rows[i]["reason_id"]),
                            reason_text = Convert.ToString(dtNonReasonOfPayment.Rows[i]["reason_text"]),
                            isactive = Convert.ToBoolean(dtNonReasonOfPayment.Rows[i]["isactive"]),
                            created_on = Convert.ToString(dtNonReasonOfPayment.Rows[i]["created_on"]),
                            updated_on = Convert.ToString(dtNonReasonOfPayment.Rows[i]["updated_on"])
                        });
                    }
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
        public ActionResult NonPaymentReason(NonPaymentReasonModelVM model)
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.AdminUser))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            try
            {
                if (model.reason_id > 0)
                {
                    string DuplicacyQuery = string.Format(QueryHelper.CheckDuplicateNonReasonOfPayment_forUpdate, model.reason_text.Trim().ToLower(), model.reason_id);
                    DataTable dtDuplicate = DbHelper.SelectMethod(DuplicacyQuery);
                    if (dtDuplicate != null && dtDuplicate.Rows.Count > 0)
                    {
                        if (Convert.ToInt32(dtDuplicate.Rows[0][0]) > 0)
                        {
                            TempData["msg"] = "Duplicate Data Found ! Please Change the Reason Name.";
                        }
                        else
                        {
                            int i = DbHelper.InsertUpdateDelete(string.Format(QueryHelper.UpdateNonReasonOfPayment, model.reason_text, model.reason_id));
                            if (i > 0)
                            {
                                TempData["msg"] = "Data Updated Successfully!";
                            }
                            else
                            {
                                TempData["msg"] = "Something went wrong while updating the data!";
                            }
                        }
                    }
                    else
                    {
                        TempData["msg"] = "Something went wrong while updating the data!";
                    }
                }
                else
                {
                    string DuplicacyQuery = string.Format(QueryHelper.CheckDuplicateNonReasonOfPayment_forInsert, model.reason_text.Trim().ToLower());
                    DataTable dtDuplicate = DbHelper.SelectMethod(DuplicacyQuery);
                    if (dtDuplicate != null && dtDuplicate.Rows.Count > 0)
                    {
                        if (Convert.ToInt32(dtDuplicate.Rows[0][0]) > 0)
                        {
                            TempData["msg"] = "Duplicate Data Found ! Please Change the Bank Name and Bank Code.";
                        }
                        else
                        {
                            int i = DbHelper.InsertUpdateDelete(string.Format(QueryHelper.InsertNonReasonOfPayment, model.reason_text));
                            if (i > 0)
                            {
                                TempData["msg"] = "Data Inserted Successfully!";
                            }
                            else
                            {
                                TempData["msg"] = "Something went wrong while Inserting the data!";
                            }
                        }
                    }
                    else
                    {
                        TempData["msg"] = "Something went wrong while Inserting the data!";
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
            }
            return RedirectToAction("NonPaymentReason", "Admin", new { Area = "Admin" });
        }
        /// <summary>
        /// Edit Reason Of NonPayment
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult EditReasonOfNonPayment(int Id)
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.AdminUser))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            NonPaymentReasonModelVM model = new NonPaymentReasonModelVM();
            try
            {
                DataTable dtOccupation = DbHelper.SelectMethod(string.Format(QueryHelper.GetNonReasonOfPayment_ById, Id));
                if (dtOccupation != null && dtOccupation.Rows.Count > 0)
                {
                    for (int i = 0; i < dtOccupation.Rows.Count; i++)
                    {
                        model.NonPaymentReasonModelList.Add(new NonPaymentReasonModel
                        {
                            reason_id = Convert.ToInt32(dtOccupation.Rows[i]["reason_id"]),
                            reason_text = Convert.ToString(dtOccupation.Rows[i]["reason_text"]),
                            isactive = Convert.ToBoolean(dtOccupation.Rows[i]["isactive"]),
                            created_on = Convert.ToString(dtOccupation.Rows[i]["created_on"]),
                            updated_on = Convert.ToString(dtOccupation.Rows[i]["updated_on"])
                        });
                    }
                }
                return Json(model.NonPaymentReasonModelList, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name)); return Json(null, JsonRequestBehavior.AllowGet);
            }
        }
        /// <summary>
        /// Disabl eReason Of NonPayment
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DisableReasonOfNonPayment(int id)
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.AdminUser))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            try
            {
                int i = DbHelper.InsertUpdateDelete(string.Format(QueryHelper.DisableNonReasonOfPayment, id));
                if (i > 0)
                {
                    TempData["msg"] = "Reason Disabled Successfully";
                }
                else
                {
                    TempData["msg"] = "Reason not Disabled Successfully !!";
                }
            }
            catch (Exception ex)
            {
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Enable Reason OfNon Payment
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult EnableReasonOfNonPayment(int id)
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.AdminUser))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            try
            {
                int i = DbHelper.InsertUpdateDelete(string.Format(QueryHelper.EnableNonReasonOfPayment, id));
                if (i > 0)
                {
                    TempData["msg"] = "Reason Enabled Successfully";
                }
                else
                {
                    TempData["msg"] = "Reason not Enabled Successfully !!";
                }
            }
            catch (Exception ex)
            {
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// term creation
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult TeamCreation()
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.AdminUser))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            TeamAgentMappingVM model = new TeamAgentMappingVM();

            return View(model);
        }
        [HttpGet]
        public ActionResult GetTL(string Type)
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.AdminUser))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            List<UserModel> _lst = new List<UserModel>();
            try
            {
                string WebAdminRoleId = string.Empty;
                if (Type == "TL Sales")
                {
                    WebAdminRoleId = Convert.ToString(ConfigurationManager.AppSettings["TLSaleRoleId"]);
                }
                else if (Type == "TL Defaulter")
                {
                    WebAdminRoleId = Convert.ToString(ConfigurationManager.AppSettings["TLDefaulterRoleId"]);
                }
                string query = string.Format(QueryHelper.GetDefaulterUser, WebAdminRoleId);
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
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name)); return Json(null, JsonRequestBehavior.AllowGet);
            }
        }
        /// <summary>
        /// Get MappedUser
        /// </summary>
        /// <param name="Type"></param>
        /// <param name="TLId"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetMappedUser(string Type, int TLId)
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.AdminUser))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            List<UserModel> _lst = new List<UserModel>();
            try
            {
                string WebAdminRoleId = string.Empty;
                if (Type == "TL Sales")
                {
                    WebAdminRoleId = Convert.ToString(ConfigurationManager.AppSettings["TLCollecterRoleId"]);
                }
                else if (Type == "TL Defaulter")
                {
                    WebAdminRoleId = Convert.ToString(ConfigurationManager.AppSettings["DefaulterUserId"]);
                }
                string query = string.Format(QueryHelper.GetUserForTeamCreation, WebAdminRoleId, TLId);
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
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name)); return Json(null, JsonRequestBehavior.AllowGet);
            }
        }
        /// <summary>
        ///user map for team
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="Type"></param>
        /// <param name="TLId"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SavedMappedUser(int[] UserId, int TLId)
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.AdminUser))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            int LoggedInUser = 0;
            try
            {
                if (HttpContext.Request.Cookies[CookiesKey.UserId].Value != null)
                {
                    LoggedInUser = Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value);
                }
                int Count = 0;
                bool delFlag = true;
                foreach (var item in UserId)
                {
                    if (delFlag)
                    {
                        string CheckExistingMapping = string.Format(QueryHelper.CheckExistingTeamAgentMap, TLId);
                        var dt = DbHelper.SelectMethod(CheckExistingMapping);
                        if (dt != null && dt.Rows.Count > 0)
                        {
                            string DeleteQuery = string.Format(QueryHelper.DeleteExistingAgentMapping, TLId);
                            int del = DbHelper.InsertUpdateDelete(DeleteQuery);
                        }
                        delFlag = false;
                    }
                    string query = string.Format(QueryHelper.InsertTeamAgentMap, TLId, item, LoggedInUser);
                    int j = DbHelper.InsertUpdateDelete(query);
                    if (j > 0)
                        Count += 1;
                    TempData["msg"] = "Data Mapped Successfully";
                }
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
            }
            return Json(false, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult AutoSelectMappedUser(int TLId)
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.AdminUser))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            List<TeamAgentMappingVM> _lst = new List<TeamAgentMappingVM>();
            try
            {
                string query = string.Format(QueryHelper.CheckExistingTeamAgentMap, TLId);
                var data = DbHelper.SelectMethod(query);
                if (data != null && data.Rows.Count > 0)
                {
                    for (int i = 0; i < data.Rows.Count; i++)
                    {
                        _lst.Add(new TeamAgentMappingVM
                        {
                            id = Convert.ToInt32(data.Rows[i]["id"]),
                            tl_id = Convert.ToInt32(data.Rows[i]["tl_id"]),
                            agent_id = Convert.ToInt32(data.Rows[i]["agent_id"]),
                            isactive = Convert.ToBoolean(data.Rows[i]["isactive"])
                        });
                    }
                }
                return Json(_lst, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name)); return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Agency Report
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult AgencyReport()
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.AdminUser))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            ApplicationRecordVM record = new ApplicationRecordVM
            {
                ViewID = 2
            };
            return View(record);
        }

        /// <summary>
        /// Get Agency Data For Report
        /// </summary>
        /// <returns></returns>
        public ActionResult GetAgencyData()
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.AdminUser))
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

                if (RequestedForm.Keys.Count > 0)
                {
                    search = Request.Form.GetValues("search[value]")[0];
                    start = Convert.ToInt32(Request["start"]);
                    length = Convert.ToInt32(Request["length"]);
                    draw = Request.Form.GetValues("draw")[0];
                    order = Request.Form.GetValues("order[0][column]")[0];
                    orderDir = Request.Form.GetValues("order[0][dir]")[0];

                }

                ApplicationRecordList = CommonMethods.GetAgencyData();
                int totalRecords = ApplicationRecordList.Count;
                if (!string.IsNullOrEmpty(search) &&
                !string.IsNullOrWhiteSpace(search))
                {
                    // Apply search 
                    ApplicationRecordList = ApplicationRecordList.Where(p => p.Name.ToString().ToLower().Contains(search.ToLower()) ||
                    p.ApplicationNo.ToString().ToLower().Contains(search.ToLower()) ||
                    p.PersonalContactNo.ToString().ToLower().Contains(search.ToLower()) ||
                    p.PersonalEmail.ToString().ToLower().Contains(search.ToLower())).ToList();
                }
                ApplicationRecordList = this.SortByColumnWithOrderAgency(order, orderDir, ApplicationRecordList);
                int recFilter = ApplicationRecordList.Count;
                ApplicationRecordList = ApplicationRecordList.Skip(start).Take(length).ToList<ApplicationRecordVM>();
                result = this.Json(new { draw = Convert.ToInt32(draw), recordsTotal = totalRecords, recordsFiltered = recFilter, data = ApplicationRecordList }, JsonRequestBehavior.AllowGet);
                return result;
            }
            catch (Exception ex)
            {
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";

                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name)); return Json(null, JsonRequestBehavior.AllowGet);
            }

        }
        /// <summary>
        /// Date Wise Filter Data
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AgencyReport(ApplicationRecordVM obj)
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.AdminUser))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            try
            {
                var FromDataSplit = obj.FromDate.Split('-').ToList();
                var fromdate = new DateTime(Convert.ToInt32(FromDataSplit[2]), Convert.ToInt32(FromDataSplit[1]), Convert.ToInt32(FromDataSplit[0])).ToString("yyyy-MM-dd");
                var ToDateSplit = obj.ToDate.Split('-').ToList();
                var todate = new DateTime(Convert.ToInt32(ToDateSplit[2]), Convert.ToInt32(ToDateSplit[1]), Convert.ToInt32(ToDateSplit[0])).ToString("yyyy-MM-dd");
                obj = CommonMethods.GetAgencyDataByDate(fromdate, todate);
                obj.ViewID = 1;
            }
            catch (Exception ex)
            {
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
            }


            return View(obj);
        }


        private List<ApplicationRecordVM> SortByColumnWithOrderAgency(string order, string orderDir, List<ApplicationRecordVM> ApplicationRecordList)
        {
            // Initialization. 
            List<ApplicationRecordVM> lst = new List<ApplicationRecordVM>();
            try
            {
                // Sorting 
                switch (order)
                {
                    case "0":

                        lst = orderDir.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? ApplicationRecordList.OrderByDescending(p => p.ApplicationNo).ToList() : ApplicationRecordList.OrderBy(p => p.ApplicationNo).ToList();
                        break;
                    case "1":

                        lst = orderDir.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? ApplicationRecordList.OrderByDescending(p => p.Address).ToList() : ApplicationRecordList.OrderBy(p => p.Address).ToList();
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

                        lst = orderDir.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? ApplicationRecordList.OrderByDescending(p => p.PersonalContactNo).ToList() : ApplicationRecordList.OrderBy(p => p.PersonalContactNo).ToList();
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
        /// <summary>
        /// Checker Report for admin 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult CheckerReport()
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.AdminUser))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            try
            {
                CommonOperation _commonMethod = new CommonOperation();
                return View(_commonMethod.GetCheckerReport());
            }
            catch (Exception ex)
            {
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                throw ex;
            }
        }

        /// <summary>
        /// Date Wise Filter Data
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult CheckerReport(Chekerinformation model)
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.AdminUser))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            CommonOperation _commonMethod = new CommonOperation();
            try
            {
                if (model.FromDate.ToString("dd-MM-yyyy") == "01-01-0001" || model.EndDate.ToString("dd-MM-yyyy") == "01-01-0001")
                {
                    return RedirectToAction("CheckerReport", "Admin", new { Area = "Admin" });
                }
                else
                {
                    model = _commonMethod.GetCheckerReportByDate(model.FromDate, model.EndDate);
                }
            }
            catch (Exception ex)
            {
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                throw ex;
            }



            return View(model);
        }

        /// <summary>
        /// Verifier Report for admin
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult VerifierReport()
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.AdminUser))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            try
            {
                CommonOperation _commonMethod = new CommonOperation();
                return View(_commonMethod.GetVerifierReport());
            }
            catch (Exception ex)
            {
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                throw ex;
            }
        }

        /// <summary>
        /// Date Wise Filter Data
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult VerifierReport(CompleteAppVerificationDetails model)
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.AdminUser))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            try
            {
                CommonOperation _commonMethod = new CommonOperation();

                if (model.From_Date.ToString("dd-MM-yyyy") == "01-01-0001" || model.End_Date.ToString("dd-MM-yyyy") == "01-01-0001")
                {
                    return RedirectToAction("VerifierReport", "Admin", new { Area = "Admin" });
                }
                else
                {
                    model = _commonMethod.GetVerifierReportByDate(model.From_Date, model.End_Date);
                }
            }
            catch (Exception ex)
            {
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
            }

            return View(model);
        }

        /// <summary>
        /// New ReferenceApproval
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult NewReferenceApproval()
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.AdminUser))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            NewReferenceApprovalModel model = new NewReferenceApprovalModel();
            try
            {
                model.NewReferenceList = CommonMethods.GetNewReferenceApprovals();
            }
            catch (Exception ex)
            {

                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
            }


            return View(model);
        }

        public ActionResult NewReferenceIsApproval(int id, string ref_no, int AppId)
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.AdminUser))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            try
            {
                int result = 0;
                int finalResult = 0;
                result = DbHelper.InsertUpdateDelete(string.Format(QueryHelper.UpdateApprovedReference, id));
                if (result > 0)
                {
                    finalResult = DbHelper.InsertUpdateDelete(string.Format(QueryHelper.UpdateNewReference, ref_no, AppId));
                }
                if (finalResult > 0)
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
                return Json(false, JsonRequestBehavior.AllowGet);
            }




        }

        /// <summary>
        /// FS BucketForAdmin
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult FSBucketForAdmin()
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.AdminUser))
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
                //return null;
            }
            return View();
        }
        /// <summary>
        /// Get FS Bucket Data in bucket
        /// </summary>
        /// <returns></returns>
        public ActionResult GetFSBucketData()
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.AdminUser))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            try
            {
                var users = CommonMethods.GetFSBucketDataAmin();
                return Json(users, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                return Json(null, JsonRequestBehavior.AllowGet);
            }

        }
        /// <summary>
        /// sent sms
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="mobno"></param>
        public void FSBucketSMS(string content, string mobno)
        {
            try
            {
                logger.ClicktoSMS(mobno, content);
            }
            catch (Exception ex)
            {
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
            }
        }
        /// <summary>
        /// Get SMSValues
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public ActionResult GetSMSValues(int Id)
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.AdminUser))
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
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                return Json(null, JsonRequestBehavior.AllowGet);
            }

        }
        /// <summary>
        /// Get EmailValues
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public ActionResult GetEmailValues(int Id)
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.AdminUser))
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
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                return Json(null, JsonRequestBehavior.AllowGet);

            }

        }
        /// <summary>
        /// Broadcast SMS use in fs bucket
        /// </summary>
        /// <returns></returns>
        public ActionResult BroadcastSMS(string SMSContent)
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.AdminUser))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            bool flag = false;
            try
            {
                CommonOperation comnoperation1 = new CommonOperation();
                #region get fs bucket application list for broadcast sms 
                //changed by priety on 02/04/2019
                var users = CommonMethods.GetFSBucketDataAmin();
                #endregion
                if (users != null && users.Count > 0)
                {
                    foreach (var item in users)
                    {
                        Thread thread = new Thread(() => FSBucketSMS(SMSContent, item.PersonalContactNumber));
                        thread.Start();
                    }
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
        /// <summary>
        /// BroadcastEmail
        /// </summary>
        /// <returns></returns>
        public ActionResult BroadcastEmail(string EmailContent)
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.AdminUser))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            bool flag = false;
            try
            {
                var users = CommonMethods.GetFSBucketDataAmin();
                if (users != null && users.Count > 0)
                {
                    foreach (var item in users)
                    {
                        new Thread(() => CommonMethods.SendMail(EmailFrom.FromInfo, item.personal_email, EmailContent, "CashMart Contact")).Start();
                    }
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

        /// <summary>
        /// Collection Disbursement Report
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult CollectionDisbursementReport()
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.AdminUser))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            DisbursementCollection model = new DisbursementCollection();
            try
            {
                if (model.From_Date.ToString("dd-MM-yyyy") == "01-01-0001")
                {
                    model.From_Date = DateTime.Now;
                }
                model.disbursementCollectionslist = GetDisbursementCollectionData(model.From_Date);
            }
            catch (Exception ex)
            {

                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
            }


            return View(model);
        }
        [HttpPost]
        public ActionResult CollectionDisbursementReport(DisbursementCollection disbursement)
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.AdminUser))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            DisbursementCollection model = new DisbursementCollection();
            try
            {
                if (disbursement.From_Date.ToString("dd-MM-yyyy") == "01-01-0001")
                {
                    disbursement.From_Date = DateTime.Now;
                }
                model.disbursementCollectionslist = GetDisbursementCollectionData(disbursement.From_Date);
            }
            catch (Exception ex)
            {

                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
            }

            return View(model);
        }


        /// <summary>
        /// Get Disbursement Collection Data
        /// </summary>
        /// <returns></returns>
        public List<DisbursementCollection> GetDisbursementCollectionData(DateTime date)
        {
            List<DisbursementCollection> model = new List<DisbursementCollection>();

            try
            {
                List<DateTime> StartDateList = new List<DateTime>();
                List<DateTime> EndDateList = new List<DateTime>();
                for (int i = 1; i <= 12; i++)
                {
                    var firstDayOfMonth = new DateTime(date.Year, i, 1);
                    var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);
                    StartDateList.Add(firstDayOfMonth);
                    EndDateList.Add(lastDayOfMonth);
                }
                for (int m = 0; m < StartDateList.Count; m++)
                {
                    List<ReportModel> _ApprovedLoan = new List<ReportModel>();
                    List<ReportModel> _ApprovedPayment = new List<ReportModel>();
                    List<ReportModel> _Penality = new List<ReportModel>();
                    List<ReportModel> _Payment = new List<ReportModel>();
                    DataTable_GenericList<ReportModel> dataTable = new DataTable_GenericList<ReportModel>();
                    dataTable.ReadData(String.Format(QueryHelper.getapprovedLoanAmountforAdmin, StartDateList[m].ToString("yyyy-MM-dd"), EndDateList[m].ToString("yyyy-MM-dd")), _ApprovedLoan, null);
                    dataTable.ReadData(String.Format(QueryHelper.GetPenality_DisburseAmountForAdmin, StartDateList[m].ToString("yyyy-MM-dd"), EndDateList[m].ToString("yyyy-MM-dd")), _Penality, null);
                    dataTable.ReadData(String.Format(QueryHelper.GetPayment_DisburseAmountForAdmin, StartDateList[m].ToString("yyyy-MM-dd"), EndDateList[m].ToString("yyyy-MM-dd")), _Payment, null);
                    dataTable.ReadData(String.Format(QueryHelper.GetApprovedPaidDetailsForAdmin, StartDateList[m].ToString("yyyy-MM-dd"), EndDateList[m].ToString("yyyy-MM-dd")), _ApprovedPayment, null);

                    model.Add(new DisbursementCollection
                    {
                        Month_Name = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(m + 1),
                        New_Loan_Amount = _ApprovedLoan.Where(x => x.is_preterm == false && x.isfor_reloan == false).Sum(x => x.approved_loan_amount),
                        Reloan_Amount = _ApprovedLoan.Where(x => x.is_preterm == true || x.isfor_reloan == true).Sum(x => x.approved_loan_amount),
                        Total_Amount = _ApprovedLoan.Sum(x => x.approved_loan_amount),
                        New_Loan_Count = _ApprovedLoan.Count(x => x.is_preterm == false && x.isfor_reloan == false),
                        Reloan_count = _ApprovedLoan.Count(x => x.is_preterm == true || x.isfor_reloan == true),
                        Total_count = _ApprovedLoan.Count(),
                        Collected_Amount_Month = _Payment.Sum(x => x.paid_amount),
                        Fully_Paid_new_Loan_Amount = _ApprovedPayment.Where(x => x.isfor_reloan == false && x.is_preterm == false && x.balanceamount == 0).Sum(x => x.paid_amount),
                        Fully_Paid_ReLoan_Amount = _ApprovedPayment.Where(x => x.isfor_reloan == true || x.is_preterm == true && x.balanceamount == 0).Sum(x => x.paid_amount),
                        Late_Penalties = _Penality.Sum(x => x.late_penalty_amount) + _Penality.Sum(x => x.deferment_amount) + _Payment.Sum(x => x.penalty_amount),
                        Total_Amount_New_Laon_Reloan = _ApprovedPayment.Where(x => x.balanceamount == 0).Sum(x => x.paid_amount) + _Penality.Sum(x => x.penalty_amount),
                        Fully_Paid_new_Loan_Count = _ApprovedPayment.Where(x => x.isfor_reloan == false && x.is_preterm == false && x.balanceamount == 0).Select(x => x.applicationno).Distinct().Count(),
                        Fully_Paid_ReLoan_Count = _ApprovedPayment.Where(x => x.isfor_reloan == true || x.is_preterm == true && x.balanceamount == 0).Select(x => x.applicationno).Distinct().Count(),
                        Total_Count_New_Loan_Reloan = _ApprovedPayment.Where(x => x.balanceamount == 0).Select(x => x.applicationno).Distinct().Count(),
                    });
                }
                if (model != null && model.Count > 0)
                {
                    model.FirstOrDefault().SumNewLoan_Peso = model.Sum(x => x.New_Loan_Amount);
                    model.FirstOrDefault().SumReloan_Peso = model.Sum(x => x.Reloan_Amount);
                    model.FirstOrDefault().SumTotal_Peso = model.Sum(x => x.Total_Amount);
                    model.FirstOrDefault().SumNewLoan_No = model.Sum(x => x.New_Loan_Count);
                    model.FirstOrDefault().SumReloan_No = model.Sum(x => x.Reloan_count);
                    model.FirstOrDefault().SumTotal_No = model.Sum(x => x.Total_count);
                    model.FirstOrDefault().SumCollect_Amount = model.Sum(x => x.Collected_Amount_Month);
                    model.FirstOrDefault().SumFullyPaid_NewLoan_Peso = model.Sum(x => x.Fully_Paid_new_Loan_Amount);
                    model.FirstOrDefault().SumFullyPaid_Relaon_Peso = model.Sum(x => x.Fully_Paid_ReLoan_Amount);
                    model.FirstOrDefault().SumLate_Penalties_Peso = model.Sum(x => x.Late_Penalties);
                    model.FirstOrDefault().SumTotal_NewLoan_Reloan_Peso = model.Sum(x => x.Total_Amount_New_Laon_Reloan);
                    model.FirstOrDefault().SumFullyPaid_NewLoan_No = model.Sum(x => x.Fully_Paid_new_Loan_Count);
                    model.FirstOrDefault().SumFullyPaid_ReLoan_No = model.Sum(x => x.Fully_Paid_ReLoan_Count);
                    model.FirstOrDefault().Sumtotal_newLoan_reloan_No = model.Sum(x => x.Total_Count_New_Loan_Reloan);
                }
            }
            catch (Exception ex)
            {
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";

                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name)); return null;
            }
            return model;
        }

        /// <summary>
        /// Reassign Collection for admin
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ReassignCollection()
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.AdminUser))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            ApplicationRecordVM record = new ApplicationRecordVM();
            try
            {

                record.view = 0;
                var query = DbHelper.SelectMethod(QueryHelper.GetCollectorUserList);
                if (query != null && query.Rows.Count > 0)
                {
                    for (int i = 0; i < query.Rows.Count; i++)
                    {
                        ApplicationRecordVM user = new ApplicationRecordVM
                        {
                            Id = Convert.ToInt32(query.Rows[i]["userid"]),
                            userfullname = Convert.ToString(query.Rows[i]["userfullname"])
                        };
                        record.UserList.Add(user);
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
            }

            return View(record);

        }
        [HttpPost]
        public ActionResult ReassignCollection(ApplicationRecordVM model, string CommandName)
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.AdminUser))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            try
            {
                List<ApplicationRecordVM> ApplicationList = new List<ApplicationRecordVM>();
                int j = 0;
                int k = 0;
                var query = DbHelper.SelectMethod(QueryHelper.GetCollectorUserList);
                if (query != null && query.Rows.Count > 0)
                {
                    for (int i = 0; i < query.Rows.Count; i++)
                    {
                        ApplicationRecordVM user = new ApplicationRecordVM
                        {
                            Id = Convert.ToInt32(query.Rows[i]["userid"]),
                            userfullname = Convert.ToString(query.Rows[i]["userfullname"])
                        };
                        model.UserList.Add(user);
                    }
                }
                if (CommandName == "Search")
                {
                    if (model.record_count < 10)
                    {
                        model.record_count = 10;
                    }
                    var arr_FromDate = model.FromDate.Split('-');
                    model.FromDate = arr_FromDate[2] + '-' + arr_FromDate[1] + "-" + arr_FromDate[0];
                    var arr_ToDate = model.ToDate.Split('-');
                    model.ToDate = arr_ToDate[2] + '-' + arr_ToDate[1] + "-" + arr_ToDate[0];

                    var checkerfrom = DbHelper.SelectMethod(string.Format(QueryHelper.GetCollectorListById, model.userfrom, model.record_count, model.FromDate, model.ToDate));
                    if (checkerfrom != null && checkerfrom.Rows.Count > 0)
                    {
                        for (int i = 0; i < checkerfrom.Rows.Count; i++)
                        {
                            ApplicationRecordVM record = new ApplicationRecordVM
                            {
                                contract_no = Convert.ToString(checkerfrom.Rows[i]["contract_ref_no"]),
                                reference_no = Convert.ToString(checkerfrom.Rows[i]["reference_no"]),
                                Id = Convert.ToInt32(checkerfrom.Rows[i]["id"]),
                                ApplicationNo = Convert.ToInt32(checkerfrom.Rows[i]["applicationno"]),
                                DateApplied = Convert.ToString(checkerfrom.Rows[i]["dateapplied"]),
                                First_Name = Convert.ToString(checkerfrom.Rows[i]["first_name"]),
                                Middle_Name = Convert.ToString(checkerfrom.Rows[i]["middle_name"]),
                                Last_Name = Convert.ToString(checkerfrom.Rows[i]["last_name"]),
                                PersonalEmail = Convert.ToString(checkerfrom.Rows[i]["personalemail"]),
                                PersonalContactNo = Convert.ToString(checkerfrom.Rows[i]["personalcontactno"])
                            };
                            ApplicationList.Add(record);
                        }
                        model.ApplicationRecordList = ApplicationList.Take(500).ToList();
                        model.view = 1;
                        return View(model);
                    }


                }
                else if (CommandName == "ReAssign")
                {
                    foreach (var item in model.ApplicationRecordList)
                    {
                        if (item.is_selected)
                        {
                            k++;
                            int i = DbHelper.InsertUpdateDelete(string.Format(QueryHelper.CollectorReAssign, item.Id, model.userto));
                            if (i > 0)
                            {
                                j++;
                            }
                        }
                    }

                    if (k == 0)
                    {
                        TempData["msg"] = "No Applications were Selected!";
                    }
                    else
                    {
                        if (j == k)
                        {
                            TempData["msg"] = "Collector Lead Reassigned Successfully";
                        }
                        else
                        {
                            TempData["msg"] = "UnSuccessfully";
                        }
                    }

                    return RedirectToAction("ReassignCollection", "Admin", new { area = "Admin" });
                }
            }
            catch (Exception ex)
            {
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
            }

            return RedirectToAction("ReassignCollection", "Admin");
        }
        /// <summary>
        /// BankName creation
        /// </summary>
        /// <param name="Prefix"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult BankName(string Prefix)
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.AdminUser))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            try
            {
                List<BankMasterModel> BankList = CommonMethods.getBankName().Where(x => x.bank_name.ToLower().StartsWith(Prefix.ToLower())).ToList();
                return Json(BankList, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                return Json(null, JsonRequestBehavior.AllowGet);
            }

        }
        /// <summary>
        /// Check Bank name
        /// </summary>
        /// <param name="bank"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult CheckBank(string bank)
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.AdminUser))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            bool flag = false;
            try
            {

                List<BankMasterModel> ObjList = CommonMethods.getBankName();
                foreach (var item in ObjList)
                {

                    if (bank == item.bank_name)
                    {
                        flag = true;
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                return Json(null, JsonRequestBehavior.AllowGet);

            }

            return Json(flag, JsonRequestBehavior.AllowGet);


        }

        [HttpPost]
        public ActionResult UpdateAgencyReportList()
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.AdminUser))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            try
            {
                List<ApplicationRecordVM> model = new List<ApplicationRecordVM>();
                model = CommonMethods.GetAgencyData();
                return Json(model, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                return Json(null, JsonRequestBehavior.AllowGet);
            }

        }

        public ActionResult MovedToDefaulter(int ApplicationId)
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.AdminUser))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            bool flag = false;
            try
            {
                int i = DbHelper.InsertUpdateDelete(string.Format(QueryHelper.MoveToDefaulterfromAgency, ApplicationId));
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
        /// Create SMSTemplate 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult NotificationTemplate()
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.AdminUser))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            NotificationTemplateVM model = new NotificationTemplateVM();
            try
            {
                DataTable dtsms = DbHelper.SelectMethod(QueryHelper.GetNotificationTemplateList);
                if (dtsms != null && dtsms.Rows.Count > 0)
                {
                    for (int i = 0; i < dtsms.Rows.Count; i++)
                    {
                        model.NotificationTemplateList.Add(new NotificationTemplateVM
                        {
                            id = Convert.ToInt32(dtsms.Rows[i]["id"]),
                            NotificationTemplateName = Convert.ToString(dtsms.Rows[i]["template_name"]),
                            NotificationTemplateDefinition = Convert.ToString(dtsms.Rows[i]["template_definition"]),
                            NotificationTemplateDesc = Convert.ToString(dtsms.Rows[i]["template_description"]),
                            isactive = Convert.ToBoolean(dtsms.Rows[i]["isactive"]),
                            createdon = Convert.ToString(dtsms.Rows[i]["created_on"]),
                            updatedon = Convert.ToString(dtsms.Rows[i]["updated_on"])
                        });
                    }
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
        public ActionResult NotificationTemplate(NotificationTemplateVM model)
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.AdminUser))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            try
            {
                if (model.id > 0)
                {
                    string DuplicacyQuery = string.Format(QueryHelper.CheckDuplicateNotification_forUpdate, model.NotificationTemplateName.Trim().ToLower(), model.id);
                    DataTable dtsmsDuplicate = DbHelper.SelectMethod(DuplicacyQuery);
                    if (dtsmsDuplicate != null && dtsmsDuplicate.Rows.Count > 0)
                    {
                        if (Convert.ToInt32(dtsmsDuplicate.Rows[0][0]) > 0)
                        {
                            TempData["msg"] = "Duplicate Data Found ! Please Change the Notification Template Name !!";
                        }
                        else
                        {
                            int i = DbHelper.InsertUpdateDelete(string.Format(QueryHelper.UpdateNotificationTemplate, model.NotificationTemplateName, model.id, model.NotificationTemplateDesc, model.NotificationTemplateDefinition));
                            if (i > 0)
                            {
                                TempData["msg"] = "Data Updated Successfully!";
                            }
                            else
                            {
                                TempData["msg"] = "Something went wrong while updating the data!";
                            }
                        }
                    }
                    else
                    {
                        TempData["msg"] = "Something went wrong while updating the data!";
                    }
                }
                else
                {
                    string DuplicacyQuery = string.Format(QueryHelper.CheckDuplicateNotification_forInsert, model.NotificationTemplateName.Trim().ToLower());
                    DataTable dtDuplicate = DbHelper.SelectMethod(DuplicacyQuery);
                    if (dtDuplicate != null && dtDuplicate.Rows.Count > 0)
                    {
                        if (Convert.ToInt32(dtDuplicate.Rows[0][0]) > 0)
                        {
                            TempData["msg"] = "Duplicate Data Found ! Please Change the Notification template Name !!";
                        }
                        else
                        {
                            int i = DbHelper.InsertUpdateDelete(string.Format(QueryHelper.InsertNotificationTemplate, model.NotificationTemplateName, model.NotificationTemplateDesc, model.NotificationTemplateDefinition));
                            if (i > 0)
                            {
                                TempData["msg"] = "Data Inserted Successfully!";
                            }
                            else
                            {
                                TempData["msg"] = "Something went wrong while Inserting the data!";
                            }
                        }
                    }
                    else
                    {
                        TempData["msg"] = "Something went wrong while Inserting the data!";
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
            }
            return RedirectToAction("NotificationTemplate", "Admin", new { Area = "Admin" });
        }

        /// <summary>
        /// Edit SMSData
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult EditNotificationTemplate(int Id)
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.AdminUser))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            NotificationTemplateVM model = new NotificationTemplateVM();
            try
            {
                DataTable dtSMS = DbHelper.SelectMethod(string.Format(QueryHelper.GetNotification_ById, Id));
                if (dtSMS != null && dtSMS.Rows.Count > 0)
                {
                    for (int i = 0; i < dtSMS.Rows.Count; i++)
                    {
                        model.NotificationTemplateList.Add(new NotificationTemplateVM
                        {
                            id = Convert.ToInt32(dtSMS.Rows[i]["id"]),
                            NotificationTemplateName = Convert.ToString(dtSMS.Rows[i]["template_name"]),
                            NotificationTemplateDefinition = Convert.ToString(dtSMS.Rows[i]["template_definition"]),
                            NotificationTemplateDesc = Convert.ToString(dtSMS.Rows[i]["template_description"]),
                            isactive = Convert.ToBoolean(dtSMS.Rows[i]["isactive"]),
                            createdon = Convert.ToString(dtSMS.Rows[i]["created_on"]),
                            updatedon = Convert.ToString(dtSMS.Rows[i]["updated_on"])
                        });
                    }
                }
                return Json(model.NotificationTemplateList, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                TempData["msg"] = "Something Went Wrong, Please check the Logs.";
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                return Json(model.NotificationTemplateList, JsonRequestBehavior.AllowGet);
            }

        }

        [HttpGet]
        public ActionResult UploadData()
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.AdminUser))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            return View();
        }

        [HttpPost]
        public ActionResult UploadData(IFormFile Data)
        {
            if (!CommonMethods.VerifyUserRole(Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.UserId]?.Value), Convert.ToInt32(HttpContext.Request.Cookies[CookiesKey.RoleId]?.Value), RoleType.AdminUser))
            {
                //return RedirectToAction("Lockout", "Home", new { Area = "" });
            }
            if (Request.Files["Data"].ContentLength > 0)
            {
                String _Result = String.Empty;
                string extension = System.IO.Path.GetExtension(Request.Files["Data"].FileName).ToLower();
                DataTable dt = new DataTable();
                int number = 0;
                string message = "";
                int k = 0;
                int duplicate = 0;
                string connString = "";
                string[] validFileTypes = { ".xls", ".xlsx" };
                var __path = System.IO.Path.Combine("~/Content/Manual/");
                if (!Directory.Exists(Server.MapPath(__path)))
                {
                    System.IO.Directory.CreateDirectory(Server.MapPath(__path));
                }
                string path1 = string.Format("{0}/{1}", Server.MapPath(__path), $"{Guid.NewGuid()}.{extension}");
                if (validFileTypes.Contains(extension))
                {
                    if (System.IO.File.Exists(path1))
                    {
                        System.IO.File.Delete(path1);
                    }
                    Request.Files["Data"].SaveAs(path1);
                    //Connection String to Excel Workbook  
                    if (extension.Trim() == ".xls")
                    {
                        connString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + path1 + ";Extended Properties='Excel 8.0;HDR=Yes'";
                        dt = ConvertXSLXtoDataTable(connString, "Database");

                    }
                    else if (extension.Trim() == ".xlsx")
                    {
                        connString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + path1 + ";Extended Properties='Excel 12.0;HDR=YES'";
                        dt = ConvertXSLXtoDataTable(connString, "Database");

                    }

                    if (dt != null && dt.Rows.Count > 0)
                    {
                        logger.WriteErrorLogs($"Excel Row Count is : {dt.Rows.Count}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            if (!String.IsNullOrWhiteSpace(Convert.ToString(dt.Rows[i]["Email Address"])))
                            {
                                //TODO: Do the Database Insert
                                logger.WriteErrorLogs($"Email Address is : {Convert.ToString(dt.Rows[i]["Email Address"])} for Row : {i + 1} is Processing.", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                                List<Parameters> _Data = new List<Parameters>
                                {
                                    new Parameters() { DbType = NpgsqlTypes.NpgsqlDbType.Date, ParameterName = "date_applied", ParameterValue = Convert.ToDateTime(dt.Rows[i]["Date Applied"]) },
                                    new Parameters() { DbType = NpgsqlTypes.NpgsqlDbType.Varchar, ParameterName = "applicationno", ParameterValue = Convert.ToString(dt.Rows[i]["Application No"]).CheckQuote() },
                                    new Parameters() { DbType = NpgsqlTypes.NpgsqlDbType.Varchar, ParameterName = "contractno", ParameterValue = Convert.ToString(dt.Rows[i]["Contract No"]).CheckQuote() },
                                    new Parameters() { DbType = NpgsqlTypes.NpgsqlDbType.Varchar, ParameterName = "payment_ref", ParameterValue = Convert.ToString(dt.Rows[i]["Lifetime Payment Reference No"]).CheckQuote() },
                                    new Parameters() { DbType = NpgsqlTypes.NpgsqlDbType.Varchar, ParameterName = "customername", ParameterValue = Convert.ToString(dt.Rows[i]["Client Name"]).CheckQuote() },
                                    new Parameters() { DbType = NpgsqlTypes.NpgsqlDbType.Varchar, ParameterName = "disburse_date", ParameterValue = Convert.ToString(dt.Rows[i]["Date Disbursed"]) },
                                    new Parameters() { DbType = NpgsqlTypes.NpgsqlDbType.Varchar, ParameterName = "due_date1", ParameterValue = Convert.ToString(dt.Rows[i]["Due date 1"]).CheckDate() },
                                    new Parameters() { DbType = NpgsqlTypes.NpgsqlDbType.Varchar, ParameterName = "due_date2", ParameterValue = Convert.ToString(dt.Rows[i]["Due date 2"]).CheckDate() },
                                    new Parameters() { DbType = NpgsqlTypes.NpgsqlDbType.Varchar, ParameterName = "due_date3", ParameterValue = Convert.ToString(dt.Rows[i]["Due date 3"]).CheckDate() },
                                    new Parameters() { DbType = NpgsqlTypes.NpgsqlDbType.Varchar, ParameterName = "due_date4", ParameterValue = Convert.ToString(dt.Rows[i]["Due date 4"]).CheckDate() },
                                    new Parameters() { DbType = NpgsqlTypes.NpgsqlDbType.Varchar, ParameterName = "due_date5", ParameterValue = Convert.ToString(dt.Rows[i]["Due date 5"]).CheckDate() },
                                    new Parameters() { DbType = NpgsqlTypes.NpgsqlDbType.Varchar, ParameterName = "due_date6", ParameterValue = Convert.ToString(dt.Rows[i]["Due date 6"]).CheckDate() },
                                    new Parameters() { DbType = NpgsqlTypes.NpgsqlDbType.Varchar, ParameterName = "due_date7", ParameterValue = Convert.ToString(dt.Rows[i]["Due date 7"]).CheckDate() },
                                    new Parameters() { DbType = NpgsqlTypes.NpgsqlDbType.Varchar, ParameterName = "due_date8", ParameterValue = Convert.ToString(dt.Rows[i]["Due date 8"]).CheckDate() },
                                    new Parameters() { DbType = NpgsqlTypes.NpgsqlDbType.Varchar, ParameterName = "due_date9", ParameterValue = Convert.ToString(dt.Rows[i]["Due date 9"]).CheckDate() },
                                    new Parameters() { DbType = NpgsqlTypes.NpgsqlDbType.Numeric, ParameterName = "loan_amount", ParameterValue = Convert.ToDecimal(Convert.ToString(dt.Rows[i]["Loan Amount"]).CheckBlank()) },
                                    new Parameters() { DbType = NpgsqlTypes.NpgsqlDbType.Varchar, ParameterName = "term_name", ParameterValue = Convert.ToString(dt.Rows[i]["Term"]).CheckQuote() },
                                    new Parameters() { DbType = NpgsqlTypes.NpgsqlDbType.Numeric, ParameterName = "outstanding", ParameterValue = Convert.ToDecimal(Convert.ToString(dt.Rows[i]["Total Outstanding Balance"]).CheckBlank()) },
                                    new Parameters() { DbType = NpgsqlTypes.NpgsqlDbType.Numeric, ParameterName = "ammortization_payment", ParameterValue = Convert.ToDecimal(Convert.ToString(dt.Rows[i]["Ammortization Payment"]).CheckBlank()) },
                                    new Parameters() { DbType = NpgsqlTypes.NpgsqlDbType.Varchar, ParameterName = "paid_date1", ParameterValue = Convert.ToString(dt.Rows[i]["Paid Date 1"]).CheckDate() },
                                    new Parameters() { DbType = NpgsqlTypes.NpgsqlDbType.Varchar, ParameterName = "paid_date2", ParameterValue = Convert.ToString(dt.Rows[i]["Paid Date 2"]).CheckDate() },
                                    new Parameters() { DbType = NpgsqlTypes.NpgsqlDbType.Varchar, ParameterName = "paid_date3", ParameterValue = Convert.ToString(dt.Rows[i]["Paid Date 3"]).CheckDate() },
                                    new Parameters() { DbType = NpgsqlTypes.NpgsqlDbType.Varchar, ParameterName = "paid_date4", ParameterValue = Convert.ToString(dt.Rows[i]["Paid Date 4"]).CheckDate() },
                                    new Parameters() { DbType = NpgsqlTypes.NpgsqlDbType.Varchar, ParameterName = "paid_date5", ParameterValue = Convert.ToString(dt.Rows[i]["Paid Date 5"]).CheckDate() },
                                    new Parameters() { DbType = NpgsqlTypes.NpgsqlDbType.Varchar, ParameterName = "paid_date6", ParameterValue = Convert.ToString(dt.Rows[i]["Paid Date6"]).CheckDate() },
                                    new Parameters() { DbType = NpgsqlTypes.NpgsqlDbType.Varchar, ParameterName = "paid_date7", ParameterValue = Convert.ToString(dt.Rows[i]["Paid Date7"]).CheckDate() },
                                    new Parameters() { DbType = NpgsqlTypes.NpgsqlDbType.Varchar, ParameterName = "paid_date8", ParameterValue = Convert.ToString(dt.Rows[i]["Paid Date8"]).CheckDate() },
                                    new Parameters() { DbType = NpgsqlTypes.NpgsqlDbType.Varchar, ParameterName = "paid_date9", ParameterValue = Convert.ToString(dt.Rows[i]["Paid Date9"]).CheckDate() },
                                    new Parameters() { DbType = NpgsqlTypes.NpgsqlDbType.Numeric, ParameterName = "paid_amount1", ParameterValue = Convert.ToDecimal(Convert.ToString(dt.Rows[i]["Paid Amount1"]).CheckBlank()) },
                                    new Parameters() { DbType = NpgsqlTypes.NpgsqlDbType.Numeric, ParameterName = "paid_amount2", ParameterValue = Convert.ToDecimal(Convert.ToString(dt.Rows[i]["Paid Amount2"]).CheckBlank()) },
                                    new Parameters() { DbType = NpgsqlTypes.NpgsqlDbType.Numeric, ParameterName = "paid_amount3", ParameterValue = Convert.ToDecimal(Convert.ToString(dt.Rows[i]["Paid Amount3"]).CheckBlank()) },
                                    new Parameters() { DbType = NpgsqlTypes.NpgsqlDbType.Numeric, ParameterName = "paid_amount4", ParameterValue = Convert.ToDecimal(Convert.ToString(dt.Rows[i]["Paid Amount4"]).CheckBlank()) },
                                    new Parameters() { DbType = NpgsqlTypes.NpgsqlDbType.Numeric, ParameterName = "paid_amount5", ParameterValue = Convert.ToDecimal(Convert.ToString(dt.Rows[i]["Paid Amount5"]).CheckBlank()) },
                                    new Parameters() { DbType = NpgsqlTypes.NpgsqlDbType.Numeric, ParameterName = "paid_amount6", ParameterValue = Convert.ToDecimal(Convert.ToString(dt.Rows[i]["Paid Amount6"]).CheckBlank()) },
                                    new Parameters() { DbType = NpgsqlTypes.NpgsqlDbType.Numeric, ParameterName = "paid_amount7", ParameterValue = Convert.ToDecimal(Convert.ToString(dt.Rows[i]["Paid Amount7"]).CheckBlank()) },
                                    new Parameters() { DbType = NpgsqlTypes.NpgsqlDbType.Numeric, ParameterName = "paid_amount8", ParameterValue = Convert.ToDecimal(Convert.ToString(dt.Rows[i]["Paid Amount8"]).CheckBlank()) },
                                    new Parameters() { DbType = NpgsqlTypes.NpgsqlDbType.Numeric, ParameterName = "paid_amount9", ParameterValue = Convert.ToDecimal(Convert.ToString(dt.Rows[i]["Paid Amount9"]).CheckBlank()) },
                                    new Parameters() { DbType = NpgsqlTypes.NpgsqlDbType.Numeric, ParameterName = "late_payment1", ParameterValue = Convert.ToDecimal(Convert.ToString(dt.Rows[i]["Late Fee 1"]).CheckBlank()) },
                                    new Parameters() { DbType = NpgsqlTypes.NpgsqlDbType.Numeric, ParameterName = "late_payment2", ParameterValue = Convert.ToDecimal(Convert.ToString(dt.Rows[i]["Late Fee 2"]).CheckBlank()) },
                                    new Parameters() { DbType = NpgsqlTypes.NpgsqlDbType.Numeric, ParameterName = "late_payment3", ParameterValue = Convert.ToDecimal(Convert.ToString(dt.Rows[i]["Late Fee 3"]).CheckBlank()) },
                                    new Parameters() { DbType = NpgsqlTypes.NpgsqlDbType.Numeric, ParameterName = "late_payment4", ParameterValue = Convert.ToDecimal(Convert.ToString(dt.Rows[i]["Late Fee 4"]).CheckBlank()) },
                                    new Parameters() { DbType = NpgsqlTypes.NpgsqlDbType.Numeric, ParameterName = "late_payment5", ParameterValue = Convert.ToDecimal(Convert.ToString(dt.Rows[i]["Late Fee 5"]).CheckBlank()) },
                                    new Parameters() { DbType = NpgsqlTypes.NpgsqlDbType.Numeric, ParameterName = "late_payment6", ParameterValue = Convert.ToDecimal(Convert.ToString(dt.Rows[i]["Late Fee 6"]).CheckBlank()) },
                                    new Parameters() { DbType = NpgsqlTypes.NpgsqlDbType.Numeric, ParameterName = "late_payment7", ParameterValue = Convert.ToDecimal(Convert.ToString(dt.Rows[i]["Late Fee 7"]).CheckBlank()) },
                                    new Parameters() { DbType = NpgsqlTypes.NpgsqlDbType.Numeric, ParameterName = "late_payment8", ParameterValue = Convert.ToDecimal(Convert.ToString(dt.Rows[i]["Late Fee 8"]).CheckBlank()) },
                                    new Parameters() { DbType = NpgsqlTypes.NpgsqlDbType.Numeric, ParameterName = "late_payment9", ParameterValue = Convert.ToDecimal(Convert.ToString(dt.Rows[i]["Late Fee 9"]).CheckBlank()) },
                                    new Parameters() { DbType = NpgsqlTypes.NpgsqlDbType.Numeric, ParameterName = "penalty1", ParameterValue = Convert.ToDecimal(Convert.ToString(dt.Rows[i]["Penalty 1"]).CheckBlank()) },
                                    new Parameters() { DbType = NpgsqlTypes.NpgsqlDbType.Numeric, ParameterName = "penalty2", ParameterValue = Convert.ToDecimal(Convert.ToString(dt.Rows[i]["Penalty 2"]).CheckBlank()) },
                                    new Parameters() { DbType = NpgsqlTypes.NpgsqlDbType.Numeric, ParameterName = "penalty3", ParameterValue = Convert.ToDecimal(Convert.ToString(dt.Rows[i]["Penalty 3"]).CheckBlank()) },
                                    new Parameters() { DbType = NpgsqlTypes.NpgsqlDbType.Numeric, ParameterName = "penalty4", ParameterValue = Convert.ToDecimal(Convert.ToString(dt.Rows[i]["Penalty 4"]).CheckBlank()) },
                                    new Parameters() { DbType = NpgsqlTypes.NpgsqlDbType.Numeric, ParameterName = "penalty5", ParameterValue = Convert.ToDecimal(Convert.ToString(dt.Rows[i]["Penalty 5"]).CheckBlank()) },
                                    new Parameters() { DbType = NpgsqlTypes.NpgsqlDbType.Numeric, ParameterName = "penalty6", ParameterValue = Convert.ToDecimal(Convert.ToString(dt.Rows[i]["Penalty 6"]).CheckBlank()) },
                                    new Parameters() { DbType = NpgsqlTypes.NpgsqlDbType.Numeric, ParameterName = "penalty7", ParameterValue = Convert.ToDecimal(Convert.ToString(dt.Rows[i]["Penalty 7"]).CheckBlank()) },
                                    new Parameters() { DbType = NpgsqlTypes.NpgsqlDbType.Numeric, ParameterName = "penalty8", ParameterValue = Convert.ToDecimal(Convert.ToString(dt.Rows[i]["Penalty 8"]).CheckBlank()) },
                                    new Parameters() { DbType = NpgsqlTypes.NpgsqlDbType.Numeric, ParameterName = "penalty9", ParameterValue = Convert.ToDecimal(Convert.ToString(dt.Rows[i]["Penalty 9"]).CheckBlank()) },
                                    new Parameters() { DbType = NpgsqlTypes.NpgsqlDbType.Numeric, ParameterName = "remain_balance", ParameterValue = Convert.ToDecimal(Convert.ToString(dt.Rows[i]["Remaining Balance"]).CheckBlank()) },
                                    new Parameters() { DbType = NpgsqlTypes.NpgsqlDbType.Numeric, ParameterName = "total_paid", ParameterValue = Convert.ToDecimal(Convert.ToString(dt.Rows[i]["Total Amount Paid"]).CheckBlank()) },
                                    new Parameters() { DbType = NpgsqlTypes.NpgsqlDbType.Varchar, ParameterName = "account_status", ParameterValue = Convert.ToString(dt.Rows[i]["Type of account (Active, Close)"]).CheckQuote() },
                                    new Parameters() { DbType = NpgsqlTypes.NpgsqlDbType.Varchar, ParameterName = "customeremail", ParameterValue = Convert.ToString(dt.Rows[i]["Email Address"]).CheckQuote() },
                                    new Parameters() { DbType = NpgsqlTypes.NpgsqlDbType.Varchar, ParameterName = "remarks", ParameterValue = Convert.ToString(dt.Rows[i]["Remarks"]).CheckQuote() }
                                };
                                int _i = DbHelper.InsertUpdateDelete(QueryHelper.InsertOldData, _Data);
                                if (_i <= 0)
                                {
                                    if (String.IsNullOrWhiteSpace(_Result))
                                    {
                                        _Result = $"Failed Contract No are {Convert.ToString(dt.Rows[i]["Contract No"])}";
                                    }
                                    else
                                    {
                                        _Result += $", {Convert.ToString(dt.Rows[i]["Contract No"])}";
                                    }
                                }
                                else
                                {
                                    logger.WriteErrorLogs($"Email Address is : {Convert.ToString(dt.Rows[i]["Email Address"])} for Row : {i + 1} is Processed.", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                                }
                            }
                            else
                            {
                                logger.WriteErrorLogs($"Email Address is Blank for Row : {i + 1}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                            }
                        }
                    }
                    if (String.IsNullOrWhiteSpace(_Result))
                    {
                        TempData["msg"] = "Data Uploaded Successfully.";
                    }
                    else
                    {
                        TempData["msg"] = _Result;
                    }
                }
                else
                {
                    TempData["msg"] = "Invalid File Format.";
                }
                return View();
            }
            TempData["msg"] = "Data not in Valid Format.Please use Valid Excel File.";
            return View();
        }
    }
}
