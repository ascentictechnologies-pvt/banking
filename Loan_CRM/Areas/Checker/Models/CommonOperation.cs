using Loan_CRM.Areas.Account.Models;
using Loan_CRM.Areas.Collector.Models;
using Loan_CRM.Models;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;

namespace Loan_CRM.Areas.Checker.Models
{
    public class CommonOperation
    {

        /// <summary>
        /// Data for Checker Bucket
        /// </summary>
        /// <returns></returns>
        public List<Checkerdetail> GetBucketList()
        {
            List<Checkerdetail> FinalnamesList = new List<Checkerdetail>();
            List<Checkerdetail> namesList = new List<Checkerdetail>();
            try
            {
                var query = DbHelper.SelectMethod(QueryHelper.GetBucketList);
                if (query != null && query.Rows.Count > 0)
                    for (int i = 0; i < query.Rows.Count; i++)
                    {
                        Checkerdetail checkdetail = new Checkerdetail
                        {
                            Id = 1 + Convert.ToInt32(i),
                            is_orr_declined = Convert.ToBoolean(query.Rows[i]["is_orr_declined"]),
                            applicationname = Convert.ToString(query.Rows[i]["first_name"]) + " " + Convert.ToString(query.Rows[i]["middle_name"]) + " " + Convert.ToString(query.Rows[i]["last_name"]),
                            applicationno = Convert.ToInt32(query.Rows[i]["applicationno"]),
                            personalcontactno = Convert.ToString(query.Rows[i]["personalcontactno"]),
                            RequestDate = Convert.ToString(query.Rows[i]["dateapplied"]),
                            AppliedOn = Convert.ToDateTime(query.Rows[i]["appliedon"]),
                            address = Convert.ToString(query.Rows[i]["address"]),
                            sss_no = Convert.ToString(query.Rows[i]["sss_no"]),
                            barangay = Convert.ToString(query.Rows[i]["barangay"]) != "" ? (Convert.ToInt32(query.Rows[i]["barangay"])) : 0,
                            occupation = Convert.ToString(query.Rows[i]["industry"]) != "" ? (Convert.ToInt32(query.Rows[i]["industry"])) : 0,
                            term = Convert.ToString(query.Rows[i]["term"]) != string.Empty ? Convert.ToInt32(query.Rows[i]["term"]) : 0,
                            companyid_url = Convert.ToString(query.Rows[i]["companyid_url"]),
                            gov_id_url = Convert.ToString(query.Rows[i]["gov_id_url"]),
                            billing_url = Convert.ToString(query.Rows[i]["billing_url"]),
                            income_url = Convert.ToString(query.Rows[i]["income_url"]),
                            other_url = Convert.ToString(query.Rows[i]["other_url"]),
                            atm_url = Convert.ToString(query.Rows[i]["atm_url"]),
                            UpdateDate = Convert.ToDateTime(query.Rows[i]["UpdatedDate"]),
                            personalemail = Convert.ToString(query.Rows[i]["personalemail"]),
                            ispickedchecker = Convert.ToString(query.Rows[i]["ispickedchecker"]) != string.Empty && Convert.ToBoolean(query.Rows[i]["ispickedchecker"])
                        };
                        checkdetail.iscompetemandate = Convert.ToBoolean(checkdetail.term);
                        if (checkdetail.companyid_url != "" && checkdetail.gov_id_url != "" && checkdetail.billing_url != "" && checkdetail.income_url != "")
                        {
                            checkdetail.iscompetedocu = true;
                        }
                        else
                        {
                            checkdetail.iscompetedocu = false;
                        }


                        //Best Time to Morning Call
                        int BestTimeToCallMorning = Convert.ToString(query.Rows[i]["morning_time"]).Trim() == string.Empty ? -1 : Convert.ToInt32(query.Rows[i]["morning_time"]);
                        if (BestTimeToCallMorning != -1)
                            checkdetail.MorningTime = CommonMethods.GetBestTimeToCallMorning(BestTimeToCallMorning);
                        else
                            checkdetail.MorningTime = string.Empty;

                        //Best Time to Noon Call
                        int BestTimeToCallNoon = Convert.ToString(query.Rows[i]["noon_time"]).Trim() == string.Empty ? -1 : Convert.ToInt32(query.Rows[i]["noon_time"]);
                        if (BestTimeToCallNoon != -1)
                            checkdetail.NoonTime = CommonMethods.GetBestTimeToCallNoon(BestTimeToCallNoon);
                        else
                            checkdetail.NoonTime = string.Empty;

                        namesList.Add(checkdetail);
                    }
                FinalnamesList.AddRange(namesList.Where(x => x.iscompetemandate && x.iscompetedocu).OrderBy(x => x.UpdateDate).ToList());
                FinalnamesList.AddRange(namesList.Where(x => x.iscompetemandate && !x.iscompetedocu).OrderBy(x => x.UpdateDate).ToList());
                FinalnamesList.AddRange(namesList.Where(x => !x.iscompetemandate && x.iscompetedocu).OrderBy(x => x.UpdateDate).ToList());
                FinalnamesList.AddRange(namesList.Where(x => !x.iscompetemandate && !x.iscompetedocu).OrderBy(x => x.UpdateDate).ToList());

                return FinalnamesList.OrderByDescending(x => x.UpdateDate).ToList();
            }
            catch (Exception ex)
            {
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                throw ex;
            }
        }

        public List<Checkerdetail> GetBucketKIVRecheckerList()
        {
            List<Checkerdetail> FinalnamesList = new List<Checkerdetail>();
            List<Checkerdetail> namesList = new List<Checkerdetail>();
            try
            {
                var query = DbHelper.SelectMethod(QueryHelper.GetBucketKIVRecheckerList);
                if (query != null && query.Rows.Count > 0)
                    for (int i = 0; i < query.Rows.Count; i++)
                    {
                        Checkerdetail checkdetail = new Checkerdetail
                        {
                            Id = 1 + Convert.ToInt32(i),
                            applicationname = Convert.ToString(query.Rows[i]["first_name"]) + " " + Convert.ToString(query.Rows[i]["middle_name"]) + " " + Convert.ToString(query.Rows[i]["last_name"]),
                            applicationno = Convert.ToInt32(query.Rows[i]["applicationno"]),
                            RequestDate = Convert.ToString(query.Rows[i]["dateapplied"]),
                            address = Convert.ToString(query.Rows[i]["address"]),
                            sss_no = Convert.ToString(query.Rows[i]["sss_no"]),
                            barangay = Convert.ToString(query.Rows[i]["barangay"]) != "" ? (Convert.ToInt32(query.Rows[i]["barangay"])) : 0,
                            occupation = Convert.ToString(query.Rows[i]["industry"]) != "" ? (Convert.ToInt32(query.Rows[i]["industry"])) : 0,
                            term = Convert.ToString(query.Rows[i]["term"]) != string.Empty ? Convert.ToInt32(query.Rows[i]["term"]) : 0,
                            companyid_url = Convert.ToString(query.Rows[i]["companyid_url"]),
                            gov_id_url = Convert.ToString(query.Rows[i]["gov_id_url"]),
                            billing_url = Convert.ToString(query.Rows[i]["billing_url"]),
                            income_url = Convert.ToString(query.Rows[i]["income_url"]),
                            other_url = Convert.ToString(query.Rows[i]["other_url"]),
                            atm_url = Convert.ToString(query.Rows[i]["atm_url"]),
                            UpdateDate = Convert.ToDateTime(query.Rows[i]["UpdatedDate"]),
                            personalemail = Convert.ToString(query.Rows[i]["personalemail"]),
                            ispickedchecker = Convert.ToString(query.Rows[i]["ispickedchecker"]) != string.Empty && Convert.ToBoolean(query.Rows[i]["ispickedchecker"])
                        };
                        checkdetail.iscompetemandate = Convert.ToBoolean(checkdetail.term);
                        if (checkdetail.companyid_url != "" && checkdetail.gov_id_url != "" && checkdetail.billing_url != "" && checkdetail.income_url != "")
                        {
                            checkdetail.iscompetedocu = true;
                        }
                        else
                        {
                            checkdetail.iscompetedocu = false;
                        }


                        //Best Time to Morning Call
                        int BestTimeToCallMorning = Convert.ToString(query.Rows[i]["morning_time"]).Trim() == string.Empty ? -1 : Convert.ToInt32(query.Rows[i]["morning_time"]);
                        if (BestTimeToCallMorning != -1)
                            checkdetail.MorningTime = CommonMethods.GetBestTimeToCallMorning(BestTimeToCallMorning);
                        else
                            checkdetail.MorningTime = string.Empty;

                        //Best Time to Noon Call
                        int BestTimeToCallNoon = Convert.ToString(query.Rows[i]["noon_time"]).Trim() == string.Empty ? -1 : Convert.ToInt32(query.Rows[i]["noon_time"]);
                        if (BestTimeToCallNoon != -1)
                            checkdetail.NoonTime = CommonMethods.GetBestTimeToCallNoon(BestTimeToCallNoon);
                        else
                            checkdetail.NoonTime = string.Empty;

                        namesList.Add(checkdetail);
                    }
                FinalnamesList.AddRange(namesList.Where(x => x.iscompetemandate && x.iscompetedocu).OrderBy(x => x.UpdateDate).ToList());
                FinalnamesList.AddRange(namesList.Where(x => x.iscompetemandate && !x.iscompetedocu).OrderBy(x => x.UpdateDate).ToList());
                FinalnamesList.AddRange(namesList.Where(x => !x.iscompetemandate && x.iscompetedocu).OrderBy(x => x.UpdateDate).ToList());
                FinalnamesList.AddRange(namesList.Where(x => !x.iscompetemandate && !x.iscompetedocu).OrderBy(x => x.UpdateDate).ToList());

                return FinalnamesList.OrderByDescending(x => x.UpdateDate).ToList();
            }
            catch (Exception ex)
            {
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                throw ex;
            }
        }

        /// <summary>
        /// Checker KIV Bucket By CheckerID
        /// </summary>
        /// <returns></returns>

        public List<Checkerdetail> GetKIVBucketList(int user_id)
        {

            List<Checkerdetail> FinalnamesList = new List<Checkerdetail>();
            List<Checkerdetail> namesList = new List<Checkerdetail>();
            try
            {
                var query = DbHelper.SelectMethod(string.Format(QueryHelper.GetKIVBucketList, user_id));
                if (query != null && query.Rows.Count > 0)
                {
                    for (int i = 0; i < query.Rows.Count; i++)
                    {
                        Checkerdetail checkdetail = new Checkerdetail
                        {
                            Id = 1 + Convert.ToInt32(i),
                            applicationname = Convert.ToString(query.Rows[i]["first_name"]) + " " + Convert.ToString(query.Rows[i]["middle_name"]) + " " + Convert.ToString(query.Rows[i]["last_name"]),
                            applicationno = Convert.ToInt32(query.Rows[i]["applicationno"]),
                            RequestDate = Convert.ToString(query.Rows[i]["dateapplied"]),
                            address = Convert.ToString(query.Rows[i]["address"]),
                            term = Convert.ToString(query.Rows[i]["term"]) != string.Empty ? Convert.ToInt32(query.Rows[i]["term"]) : 0,
                            sss_no = Convert.ToString(query.Rows[i]["sss_no"]),
                            barangay = Convert.ToString(query.Rows[i]["barangay"]) != "" ? (Convert.ToInt32(query.Rows[i]["barangay"])) : 0,
                            occupation = Convert.ToString(query.Rows[i]["industry"]) != "" ? (Convert.ToInt32(query.Rows[i]["industry"])) : 0,
                            companyid_url = Convert.ToString(query.Rows[i]["companyid_url"]),
                            gov_id_url = Convert.ToString(query.Rows[i]["gov_id_url"]),
                            billing_url = Convert.ToString(query.Rows[i]["billing_url"]),
                            income_url = Convert.ToString(query.Rows[i]["income_url"]),
                            other_url = Convert.ToString(query.Rows[i]["other_url"]),
                            UpdateDate = Convert.ToDateTime(query.Rows[i]["UpdatedDate"]),
                            createdon = Convert.ToDateTime(query.Rows[i]["createdon"]),
                            ispickedchecker = Convert.ToString(query.Rows[i]["ispickedchecker"]) != string.Empty && Convert.ToBoolean(query.Rows[i]["ispickedchecker"]),
                            ischeck = Convert.ToString(query.Rows[i]["ischeck"]) != string.Empty && Convert.ToBoolean(query.Rows[i]["ischeck"])
                        };
                        checkdetail.iscompetemandate = Convert.ToBoolean(checkdetail.term);
                        if (checkdetail.companyid_url != "" && checkdetail.gov_id_url != "" && checkdetail.billing_url != "" && checkdetail.income_url != "")
                        {
                            checkdetail.iscompetedocu = true;
                        }
                        else
                        {
                            checkdetail.iscompetedocu = false;
                        }
                        if (checkdetail.createdon != null)
                        {
                            checkdetail.aging = Convert.ToInt32((DateTime.Now - checkdetail.createdon).Days);
                        }
                        //Best Time to Morning Call
                        int BestTimeToCallMorning = Convert.ToString(query.Rows[i]["morning_time"]).Trim() == string.Empty ? -1 : Convert.ToInt32(query.Rows[i]["morning_time"]);
                        if (BestTimeToCallMorning != -1)
                            checkdetail.MorningTime = CommonMethods.GetBestTimeToCallMorning(BestTimeToCallMorning);
                        else
                            checkdetail.MorningTime = string.Empty;

                        //Best Time to Noon Call
                        int BestTimeToCallNoon = Convert.ToString(query.Rows[i]["noon_time"]).Trim() == string.Empty ? -1 : Convert.ToInt32(query.Rows[i]["noon_time"]);
                        if (BestTimeToCallNoon != -1)
                            checkdetail.NoonTime = CommonMethods.GetBestTimeToCallNoon(BestTimeToCallNoon);
                        else
                            checkdetail.NoonTime = string.Empty;

                        namesList.Add(checkdetail);
                    }
                }
                FinalnamesList.AddRange(namesList.Where(x => x.iscompetemandate && x.iscompetedocu).OrderBy(x => x.UpdateDate).ToList());
                FinalnamesList.AddRange(namesList.Where(x => x.iscompetemandate && !x.iscompetedocu).OrderBy(x => x.UpdateDate).ToList());
                FinalnamesList.AddRange(namesList.Where(x => !x.iscompetemandate && x.iscompetedocu).OrderBy(x => x.UpdateDate).ToList());
                FinalnamesList.AddRange(namesList.Where(x => !x.iscompetemandate && !x.iscompetedocu).OrderBy(x => x.UpdateDate).ToList());
                return FinalnamesList.OrderByDescending(x => x.checker_oic_on).ToList();
            }
            catch (Exception ex)
            {

                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                throw ex;
            }
        }

        /// <summary>
        /// Recheck Bucket List
        /// </summary>
        /// <returns></returns>
        public List<Checkerdetail> GetRecheckBucketList(int userid)
        {
            try
            {
                List<Checkerdetail> namesList = new List<Checkerdetail>();
                var query = DbHelper.SelectMethod(string.Format(QueryHelper.GetRecheckBucketList, userid));
                if (query != null && query.Rows.Count > 0)
                    for (int i = 0; i < query.Rows.Count; i++)
                    {
                        Checkerdetail checkdetail = new Checkerdetail
                        {
                            Id = 1 + Convert.ToInt32(i),
                            applicationname = Convert.ToString(query.Rows[i]["first_name"]) + " " + Convert.ToString(query.Rows[i]["middle_name"]) + " " + Convert.ToString(query.Rows[i]["last_name"]),
                            applicationno = Convert.ToInt32(query.Rows[i]["applicationno"]),
                            RequestDate = Convert.ToString(query.Rows[i]["dateapplied"]),
                            personalcontactno = Convert.ToString(query.Rows[i]["personalcontactno"]),
                            address = Convert.ToString(query.Rows[i]["address"]),
                            term = Convert.ToString(query.Rows[i]["term"]) != string.Empty ? Convert.ToInt32(query.Rows[i]["term"]) : 0,
                            ispickedrechecker = Convert.ToString(query.Rows[i]["is_picked_for_rechecker"]) != string.Empty && Convert.ToBoolean(query.Rows[i]["is_picked_for_rechecker"]),
                            companyid_url = Convert.ToString(query.Rows[i]["companyid_url"]),
                            gov_id_url = Convert.ToString(query.Rows[i]["gov_id_url"]),
                            billing_url = Convert.ToString(query.Rows[i]["billing_url"]),
                            income_url = Convert.ToString(query.Rows[i]["income_url"]),
                            other_url = Convert.ToString(query.Rows[i]["other_url"])
                        };

                        //Best Time to Morning Call
                        int BestTimeToCallMorning = Convert.ToString(query.Rows[i]["morning_time"]).Trim() == string.Empty ? -1 : Convert.ToInt32(query.Rows[i]["morning_time"]);
                        if (BestTimeToCallMorning != -1)
                            checkdetail.MorningTime = CommonMethods.GetBestTimeToCallMorning(BestTimeToCallMorning);
                        else
                            checkdetail.MorningTime = string.Empty;

                        //Best Time to Noon Call
                        int BestTimeToCallNoon = Convert.ToString(query.Rows[i]["noon_time"]).Trim() == string.Empty ? -1 : Convert.ToInt32(query.Rows[i]["noon_time"]);
                        if (BestTimeToCallNoon != -1)
                            checkdetail.NoonTime = CommonMethods.GetBestTimeToCallNoon(BestTimeToCallNoon);
                        else
                            checkdetail.NoonTime = string.Empty;

                        namesList.Add(checkdetail);
                    }

                return namesList;
            }
            catch (Exception ex)
            {
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                throw ex;
            }


        }

        /// <summary>
        /// Get Filter Bucke tList
        /// </summary>
        /// <returns></returns>
        public List<Checkerdetail> GetFilterBucketList(string filter_text, DateTime StartDate, DateTime EndDate)
        {
            List<Checkerdetail> FinalnamesList = new List<Checkerdetail>();
            List<Checkerdetail> namesList = new List<Checkerdetail>();
            try
            {
                var query = new DataTable();

                if (filter_text == "Date Applied")
                {
                    query = DbHelper.SelectMethod(string.Format(QueryHelper.GetDateAppliedFilterList, StartDate.ToString("yyyy-MM-dd hh:mm:ss"), EndDate.ToString("yyyy-MM-dd hh:mm:ss")));
                }
                else if (filter_text == "Date Of Status Changed")
                {
                    query = DbHelper.SelectMethod(string.Format(QueryHelper.GetStatusUpdateFilterList, StartDate.ToString("yyyy-MM-dd hh:mm:ss"), EndDate.ToString("yyyy-MM-dd hh:mm:ss tt")));
                }
                else if (filter_text == "OIC")
                {
                    query = DbHelper.SelectMethod(string.Format(QueryHelper.GetOICFilterList, StartDate.ToString("yyyy-MM-dd hh:mm:ss"), EndDate.ToString("yyyy-MM-dd hh:mm:ss")));
                }

                if (query != null && query.Rows.Count > 0)
                    for (int i = 0; i < query.Rows.Count; i++)
                    {
                        Checkerdetail checkdetail = new Checkerdetail
                        {
                            Id = 1 + Convert.ToInt32(i),
                            applicationname = Convert.ToString(query.Rows[i]["first_name"]) + " " + Convert.ToString(query.Rows[i]["middle_name"]) + " " + Convert.ToString(query.Rows[i]["last_name"]),
                            applicationno = Convert.ToInt32(query.Rows[i]["applicationno"]),
                            RequestDate = Convert.ToString(query.Rows[i]["dateapplied"]),
                            AppliedOn = Convert.ToDateTime(query.Rows[i]["appliedon"]),
                            personalcontactno = Convert.ToString(query.Rows[i]["personalcontactno"]),
                            address = Convert.ToString(query.Rows[i]["address"]),
                            term = Convert.ToString(query.Rows[i]["term"]) != string.Empty ? Convert.ToInt32(query.Rows[i]["term"]) : 0,
                            sss_no = Convert.ToString(query.Rows[i]["sss_no"]),
                            barangay = Convert.ToString(query.Rows[i]["barangay"]) != "" ? (Convert.ToInt32(query.Rows[i]["barangay"])) : 0,
                            occupation = Convert.ToString(query.Rows[i]["industry"]) != "" ? (Convert.ToInt32(query.Rows[i]["industry"])) : 0,
                            companyid_url = Convert.ToString(query.Rows[i]["companyid_url"]),
                            gov_id_url = Convert.ToString(query.Rows[i]["gov_id_url"]),
                            billing_url = Convert.ToString(query.Rows[i]["billing_url"]),
                            income_url = Convert.ToString(query.Rows[i]["income_url"]),
                            other_url = Convert.ToString(query.Rows[i]["other_url"]),
                            UpdateDate = Convert.ToDateTime(query.Rows[i]["UpdatedDate"]),
                            ispickedchecker = Convert.ToString(query.Rows[i]["ispickedchecker"]) != string.Empty && Convert.ToBoolean(query.Rows[i]["ispickedchecker"])
                        };
                        checkdetail.iscompetemandate = Convert.ToBoolean(checkdetail.term);
                        if (checkdetail.companyid_url != "" && checkdetail.gov_id_url != "" && checkdetail.billing_url != "" && checkdetail.income_url != "" && checkdetail.other_url != "")
                        {
                            checkdetail.iscompetedocu = true;
                        }
                        else
                        {
                            checkdetail.iscompetedocu = false;
                        }


                        //Best Time to Morning Call
                        int BestTimeToCallMorning = Convert.ToString(query.Rows[i]["morning_time"]).Trim() == string.Empty ? -1 : Convert.ToInt32(query.Rows[i]["morning_time"]);
                        if (BestTimeToCallMorning != -1)
                            checkdetail.MorningTime = CommonMethods.GetBestTimeToCallMorning(BestTimeToCallMorning);
                        else
                            checkdetail.MorningTime = string.Empty;

                        //Best Time to Noon Call
                        int BestTimeToCallNoon = Convert.ToString(query.Rows[i]["noon_time"]).Trim() == string.Empty ? -1 : Convert.ToInt32(query.Rows[i]["noon_time"]);
                        if (BestTimeToCallNoon != -1)
                            checkdetail.NoonTime = CommonMethods.GetBestTimeToCallNoon(BestTimeToCallNoon);
                        else
                            checkdetail.NoonTime = string.Empty;

                        namesList.Add(checkdetail);
                    }
                FinalnamesList.AddRange(namesList.Where(x => x.iscompetemandate && x.iscompetedocu).OrderBy(x => x.UpdateDate).ToList());
                FinalnamesList.AddRange(namesList.Where(x => x.iscompetemandate && !x.iscompetedocu).OrderBy(x => x.UpdateDate).ToList());
                FinalnamesList.AddRange(namesList.Where(x => !x.iscompetemandate && !x.iscompetedocu).OrderBy(x => x.UpdateDate).ToList());
                FinalnamesList.AddRange(namesList.Where(x => !x.iscompetemandate && x.iscompetedocu).OrderBy(x => x.UpdateDate).ToList());
                return FinalnamesList;
            }
            catch (Exception ex)
            {

                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                throw ex;
            }
        }

        /// <summary>
        /// Get Details by ID
        /// </summary>
        /// <returns></returns>
        public Chekerinformation GetListbyID(int Id, string actionName, int user_id)
        {
            Chekerinformation checkdetail = new Chekerinformation();
            try
            {

                DataTable dt;
                if (actionName == "RecheckRequestBucket")
                {
                    dt = DbHelper.SelectMethod(string.Format(QueryHelper.GetLoanRequestDetailsRecheckerTrue, Id));
                }
                else if (actionName == "GetLoanRequestForVerification")
                {
                    dt = DbHelper.SelectMethod(string.Format(QueryHelper.GetLoanRequestDetails_ForVerifierNew, Id, user_id));
                }
                else
                {
                    dt = DbHelper.SelectMethod(string.Format(QueryHelper.GetLoanRequestDetails, Id, user_id));
                }
                if (dt != null && dt.Rows.Count > 0)
                {
                    checkdetail.name = Convert.ToString(dt.Rows[0]["first_name"]) + " " + Convert.ToString(dt.Rows[0]["middle_name"]) + " " + Convert.ToString(dt.Rows[0]["last_name"]);
                    checkdetail.personalemail = Convert.ToString(dt.Rows[0]["personalemail"]);
                    checkdetail.personalcontactno = Convert.ToString(dt.Rows[0]["personalcontactno"]);
                    checkdetail.secondcontactno = Convert.ToString(dt.Rows[0]["relativecontactno"]);
                    checkdetail.thirdcontactno = Convert.ToString(dt.Rows[0]["coworkercontactno"]);
                    checkdetail.address = Convert.ToString(dt.Rows[0]["address"]);
                    checkdetail.jumioreference = Convert.ToString(dt.Rows[0]["jumioreference"]);
                    checkdetail.facemapstatus = Convert.ToString(dt.Rows[0]["facemapstatus"]);
                    checkdetail.homephoneno = Convert.ToString(dt.Rows[0]["homephoneno"]);
                    checkdetail.placeofbirth = Convert.ToString(dt.Rows[0]["placeofbirth"]);
                    checkdetail.dateofbirth = Convert.ToString(dt.Rows[0]["dateofbirth"]);
                    checkdetail.civilstatus = Convert.ToString(dt.Rows[0]["civilstatus"]);
                    checkdetail.mothermaidenname = Convert.ToString(dt.Rows[0]["mothermaidenname"]);
                    checkdetail.notificationtoken = Convert.ToString(dt.Rows[0]["notificationtoken"]);
                    checkdetail.motheraddress = Convert.ToString(dt.Rows[0]["motheraddress"]);
                    checkdetail.companyname = Convert.ToString(dt.Rows[0]["companyname"]);
                    checkdetail.gender = Convert.ToString(dt.Rows[0]["gender"]);
                    checkdetail.gov_doc_type = Convert.ToString(dt.Rows[0]["gov_doc_type"]);
                    checkdetail.companyaddress = Convert.ToString(dt.Rows[0]["companyaddress"]);
                    checkdetail.designation = Convert.ToString(dt.Rows[0]["designation"]);
                    checkdetail.user_id = Convert.ToInt32(dt.Rows[0]["user_id"]);
                    if (dt.Rows[0]["gross_income"] is DBNull)
                        checkdetail.gross_income = Convert.ToDecimal(0.00);
                    else
                        checkdetail.gross_income = Convert.ToDecimal(dt.Rows[0]["gross_income"]);
                    checkdetail.reference_name = Convert.ToString(dt.Rows[0]["reference_name"]);
                    checkdetail.reference_contactno = Convert.ToString(dt.Rows[0]["reference_contactno"]);
                    checkdetail.bankname = Convert.ToString(dt.Rows[0]["bankname"]);
                    checkdetail.bankaccountno = Convert.ToString(dt.Rows[0]["bankaccountno"]);
                    checkdetail.city = Convert.ToString(dt.Rows[0]["city"]);
                    checkdetail.date_joining = Convert.ToString(dt.Rows[0]["date_joining"]);
                    checkdetail.pay_date = Convert.ToString(dt.Rows[0]["pay_date"]);
                    checkdetail.company_phoneno = Convert.ToString(dt.Rows[0]["company_phoneno"]);
                    checkdetail.applicationno = Convert.ToInt32(dt.Rows[0]["applicationno"]);
                    checkdetail.age = Convert.ToString(dt.Rows[0]["age"]);
                    checkdetail.gov_id_url = Convert.ToString(dt.Rows[0]["gov_id_url"]);
                    checkdetail.companyid_url = Convert.ToString(dt.Rows[0]["companyid_url"]);
                    checkdetail.billing_url = Convert.ToString(dt.Rows[0]["billing_url"]);
                    checkdetail.income_url = Convert.ToString(dt.Rows[0]["income_url"]);
                    checkdetail.other_url = Convert.ToString(dt.Rows[0]["other_url"]);
                    checkdetail.atm_url = Convert.ToString(dt.Rows[0]["atm_url"]);
                    checkdetail.cityname = Convert.ToString(dt.Rows[0]["cityname"]);
                    checkdetail.street = Convert.ToString(dt.Rows[0]["street"]);
                    checkdetail.barangay = Convert.ToString(dt.Rows[0]["barangay"]);
                    checkdetail.barangay_name = Convert.ToString(dt.Rows[0]["barangay_name"]);
                    checkdetail.province_name = Convert.ToString(dt.Rows[0]["province_name"]);
                    checkdetail.zipcode = Convert.ToString(dt.Rows[0]["zipcode"]);
                    checkdetail.sss_no = Convert.ToString(dt.Rows[0]["sss_no"]);
                    if (dt.Rows[0]["paydate1"] is DBNull)
                        checkdetail.paydate1 = Convert.ToInt32(0);
                    else
                        checkdetail.paydate1 = Convert.ToInt32(dt.Rows[0]["paydate1"]);
                    if (dt.Rows[0]["paydate2"] is DBNull)
                        checkdetail.paydate2 = Convert.ToInt32(0);
                    else
                        checkdetail.paydate2 = Convert.ToInt32(dt.Rows[0]["paydate2"]);

                    //Best Time to Morning Call
                    int BestTimeToCallMorning = Convert.ToString(dt.Rows[0]["morning_time"]).Trim() == string.Empty ? -1 : Convert.ToInt32(dt.Rows[0]["morning_time"]);
                    if (BestTimeToCallMorning != -1)
                        checkdetail.MorningTime = CommonMethods.GetBestTimeToCallMorning(BestTimeToCallMorning);
                    else
                        checkdetail.MorningTime = string.Empty;

                    //Best Time to Noon Call
                    int BestTimeToCallNoon = Convert.ToString(dt.Rows[0]["noon_time"]).Trim() == string.Empty ? -1 : Convert.ToInt32(dt.Rows[0]["noon_time"]);
                    if (BestTimeToCallNoon != -1)
                        checkdetail.NoonTime = CommonMethods.GetBestTimeToCallNoon(BestTimeToCallNoon);
                    else
                        checkdetail.NoonTime = string.Empty;
                }
                return checkdetail;
            }
            catch (Exception ex)
            {
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                throw ex;
            }
        }

        public int UpdateFiles(int Id, string GovtFileName, string CompanyFileName, string BillFileName, string IncomeFileIdUrl, string OtherFileName, string ATMFileName)
        {
            try
            {
                int i = 0;
                i = DbHelper.InsertUpdateDelete(string.Format(QueryHelper.UpdateFilesFromChecker, Id, GovtFileName, CompanyFileName, BillFileName, IncomeFileIdUrl, OtherFileName, ATMFileName));
                return i;
            }
            catch (Exception ex)
            {

                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                throw ex;

            }

        }

        public int UpdateDecline(int Id, string Remarks)
        {
            try
            {
                int a = 0;
                a = DbHelper.InsertUpdateDelete(string.Format(QueryHelper.UpdateDecline, Id, Remarks.Replace("'", "\'")));
                return a;
            }
            catch (Exception ex)
            {

                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                throw ex;

            }

        }

        public PaymentInformation GetReduceLoan()
        {
            PaymentInformation payment = new PaymentInformation();
            try
            {
                DataTable dataTable;
                dataTable = DbHelper.SelectMethod(string.Format(QueryHelper.GetReduceLoan_Id));
                if (dataTable != null && dataTable.Rows.Count > 0)
                {
                    for (int i = 0; i < dataTable.Rows.Count; i++)
                    {
                        PaymentInformation paymentdetail = new PaymentInformation
                        {
                            applicationno = Convert.ToInt64(dataTable.Rows[i]["applicationno"]),
                            Referenceno = Convert.ToString(dataTable.Rows[i]["reference_no"]),
                            Name = Convert.ToString(dataTable.Rows[i]["first_name"]) + " " + Convert.ToString(dataTable.Rows[i]["middle_name"]) + " " + Convert.ToString(dataTable.Rows[i]["last_name"])
                        };
                        payment.paymentinformation.Add(paymentdetail);
                    }
                }
            }
            catch (Exception ex)
            {

                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                throw ex;
            }
            return payment;
        }

        //Collector Controller

        public PaymentInformation GetPaymentDetails(Int64 Applicationno)
        {
            PaymentInformation paymentdetail = new PaymentInformation();
            try
            {
                DataTable dataTable = DbHelper.SelectMethod(string.Format(QueryHelper.GetPaymentHistory, Applicationno));

                paymentdetail.paymentinformation = new List<PaymentInformation>();
                if (dataTable != null && dataTable.Rows.Count > 0)
                {
                    for (int i = 0; i < dataTable.Rows.Count; i++)
                    {
                        PaymentInformation record = new PaymentInformation
                        {
                            Dueedate = paymentdetail.Dueedate = Convert.ToString(dataTable.Rows[i]["emi_date"]),
                            amortization = Convert.ToString(dataTable.Rows[i]["emi_amount"]),
                            paymentdate = Convert.ToString(dataTable.Rows[i]["updatedon"]),
                            Paidamount = Convert.ToString(dataTable.Rows[i]["paidamount"])
                        };
                        paymentdetail.paymentinformation.Add(record);
                    }
                    paymentdetail.applicationno = Convert.ToInt64(dataTable.Rows[0]["applicationno"]);
                    paymentdetail.personalcontactno = Convert.ToString(dataTable.Rows[0]["personalcontactno"]);
                    paymentdetail.Referenceno = Convert.ToString(dataTable.Rows[0]["reference_no"]);
                    paymentdetail.balanceamt = Convert.ToString(dataTable.Rows[0]["balance"]);
                    DataTable dataTable1 = DbHelper.SelectMethod(string.Format(QueryHelper.GetReduceLoanCount, paymentdetail.Referenceno));
                    if (dataTable1 != null && dataTable1.Rows.Count > 0)
                    {
                        paymentdetail.ReduceLoanCount = Convert.ToInt32(dataTable1.Rows[0]["reduceloan_no"]);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                throw ex;
            }
            return paymentdetail;
        }

        //Colloctor  Controller
        public PaymentInformation GetCollecterListbyPhoneNumber(Int64 Applicationno)
        {
            PaymentInformation paymentdetail = new PaymentInformation();
            try
            {
                //int PastEmiCount = 0;
                decimal _balance = 0;
                decimal __PaidPenality = 0;
                decimal __PaidAmount = 0;
                double _dayCount = 0;
                //var PassedEMICount = DbHelper.SelectMethod(string.Format(QueryHelper.PastEmiCountByDate, Applicationno, DateTime.Now.ToString("yyyy-MM-dd")));
                //if (PassedEMICount != null && PassedEMICount.Rows.Count > 0)
                //{
                //    PastEmiCount = PassedEMICount.Rows[0]["emicount"] == DBNull.Value ? 0 : Convert.ToInt32(PassedEMICount.Rows[0]["emicount"]);
                //}

                DataTable dataTable = DbHelper.SelectMethod(string.Format(QueryHelper.GetLoanListLatest, Applicationno));

                if (dataTable != null && dataTable.Rows.Count > 0)
                {
                    paymentdetail.Name = Convert.ToString(dataTable.Rows[0]["first_name"]) + " " + Convert.ToString(dataTable.Rows[0]["middle_name"]) + " " + Convert.ToString(dataTable.Rows[0]["last_name"]);
                    paymentdetail.applicationno = Convert.ToInt32(dataTable.Rows[0]["applicationno"]);
                    paymentdetail.Referenceno = Convert.ToString(dataTable.Rows[0]["reference_no"]);
                    paymentdetail.Termtype = Convert.ToString(dataTable.Rows[0]["termtype"]);
                    paymentdetail.emiamount = Convert.ToString(dataTable.Rows[0]["emi_amount"]);
                    paymentdetail.Term = Convert.ToString(dataTable.Rows[0]["term_name"]);
                    paymentdetail.disbursemant = Convert.ToString(dataTable.Rows[0]["disbursement_date"]);
                    paymentdetail.Datepaid = Convert.ToString(dataTable.Rows[0]["date_paid"]);
                    paymentdetail.jumioreference = Convert.ToString(dataTable.Rows[0]["jumioreference"]);
                    paymentdetail.facemapstatus = Convert.ToString(dataTable.Rows[0]["facemapstatus"]);
                    paymentdetail.Besttimetocallback = Convert.ToString(dataTable.Rows[0]["best_time_callback"]);
                    paymentdetail.nonpaymentreason = Convert.ToString(dataTable.Rows[0]["non_payment_reason"]);
                    paymentdetail.currentdateptp = Convert.ToString(dataTable.Rows[0]["current_date_ptp"]);
                    paymentdetail.currentamountptp = Convert.ToString(dataTable.Rows[0]["current_amount_ptp"]);
                    paymentdetail.Otherremarks = Convert.ToString(dataTable.Rows[0]["other_remarks"]);
                    paymentdetail.personalcontactno = Convert.ToString(dataTable.Rows[0]["personalcontactno"]);
                    paymentdetail.Contractno = Convert.ToString(dataTable.Rows[0]["contract_ref_no"]);
                    paymentdetail.EmailId = Convert.ToString(dataTable.Rows[0]["personalemail"]);
                    paymentdetail.Dueedate = Convert.ToString(dataTable.Rows[0]["emi_date"]);
                    paymentdetail.familyname1 = Convert.ToString(dataTable.Rows[0]["family_name1"]);
                    paymentdetail.familyrelation1 = Convert.ToString(dataTable.Rows[0]["family_relation1"]);
                    paymentdetail.familycontact1 = Convert.ToString(dataTable.Rows[0]["family_contact1"]);
                    paymentdetail.familyname2 = Convert.ToString(dataTable.Rows[0]["family_name2"]);
                    paymentdetail.familyrelation2 = Convert.ToString(dataTable.Rows[0]["family_relation2"]);
                    paymentdetail.familycontact2 = Convert.ToString(dataTable.Rows[0]["family_contact2"]);
                    paymentdetail.ptpdate = Convert.ToString(dataTable.Rows[0]["current_date_ptp"]);
                    paymentdetail.emiid = Convert.ToInt64(dataTable.Rows[0]["emi_id"]);
                    var InterRate = Convert.ToDecimal(dataTable.Rows[0]["interest_rate"]);
                    var DueAmount = Convert.ToString(dataTable.Rows[0]["due_amount"]);
                    paymentdetail.pastduedays = Convert.ToInt32(dataTable.Rows[0]["aging"]);
                    paymentdetail.UserID = DBNull.Value.Equals(dataTable.Rows[0]["user_id"]) ? 0 : Convert.ToInt32(dataTable.Rows[0]["user_id"]);
                    string ptpdate = string.Empty;
                    DataTable _PaymentDetails = new DataTable();
                    for (int i = 0; i < dataTable.Rows.Count; i++)
                    {
                        if (Convert.ToString(dataTable.Rows[i]["emi_date"]).Contains('-'))
                        {
                            var DataSplit = Convert.ToString(dataTable.Rows[i]["emi_date"]).Split('-').ToList();
                            ptpdate = new DateTime(Convert.ToInt32(DataSplit[2]), Convert.ToInt32(DataSplit[1]), Convert.ToInt32(DataSplit[0])).ToString("yyyy-MM-dd");
                            if (Convert.ToDateTime(ptpdate).Date < DateTime.Now.Date)
                            {
                                _balance += Convert.ToDecimal(dataTable.Rows[i]["due_amount"]);
                            }
                            var _PenalityData = DbHelper.SelectMethod(String.Format(QueryHelper.getPaidPenality, ptpdate, paymentdetail.applicationno));
                            if (_PenalityData != null && _PenalityData.Rows.Count > 0)
                            {
                                if (!String.IsNullOrWhiteSpace(Convert.ToString(_PenalityData.Rows[0]["penalty_amount"])))
                                {
                                    __PaidPenality += Convert.ToDecimal(_PenalityData.Rows[0]["penalty_amount"]);
                                    __PaidAmount += Convert.ToDecimal(_PenalityData.Rows[0]["paid_amount"]);
                                }
                            }
                            _PaymentDetails = DbHelper.SelectMethod($"SELECT distinct a.payment_id, a.application_no, a.paid_amount, a.paid_on, a.updatedon, a.updatedby, a.whatpaymentchennel, a.proofofpayment, a.paymentapproved, a.isdeleted, a.penalty_amount, a.remarks, a.is_pay, a.is_partial, c.emi_amount FROM public.tblapplication_emi_payment a join tblapplication_emi b on a.application_no=b.application_no join tblapplication_emi_details c on b.emi_id=c.emi_id where a.application_no={Applicationno} and a.paymentapproved=true and a.is_pay=false order by payment_id;");
                        }
                    }

                    var __Term = Convert.ToString(dataTable.Rows[0]["term_name"]).Split(' ').ToList();
                    paymentdetail.termtype = Convert.ToString(dataTable.Rows[0]["termtypename"]);

                    paymentdetail.outstanding = Convert.ToDecimal(dataTable.Rows[0]["totalbalanceamount"]);
                    paymentdetail.principal = Convert.ToString(dataTable.Rows[0]["approved_loan_amount"]);
                    paymentdetail.amortization = Convert.ToString((Convert.ToDecimal(dataTable.Rows[0]["approved_loan_amount"]) + ((Convert.ToDecimal(dataTable.Rows[0]["approved_loan_amount"]) * Convert.ToDecimal(dataTable.Rows[0]["interest_rate"])) / 100)) / Convert.ToDecimal(__Term.FirstOrDefault()));
                    DataTable dataTable1 = DbHelper.SelectMethod(string.Format(QueryHelper.SearchUserIdByApplicationNo, Applicationno));
                    if (dataTable1 != null && dataTable1.Rows.Count > 0)
                    {
                        int userid;
                        userid = DBNull.Value.Equals(dataTable1.Rows[0]["user_id"]) ? 0 : Convert.ToInt32(dataTable1.Rows[0]["user_id"]);
                        var query = DbHelper.SelectMethod(string.Format(QueryHelper.SearchApplicationNoByUserId, userid));
                        if (query != null && query.Rows.Count > 0)
                        {
                            paymentdetail.ReloanCount = query.Rows.Count;
                        }
                    }

                    double PercentPenalty = (Convert.ToDouble(Convert.ToString(dataTable.Rows[0]["late_rate"])) / 100);
                    double penalty = Convert.ToDouble(Convert.ToString(dataTable.Rows[0]["late_penalty"]));
                    #region New Code
                    if (_PaymentDetails == null || _PaymentDetails.Rows.Count == 0)
                    {
                        if (_balance > 0 && paymentdetail.pastduedays > 0)
                        {
                            if (paymentdetail.termtype == "Weekly")
                            {
                                double _Mode = 0;
                                if (paymentdetail.pastduedays != 0 && paymentdetail.pastduedays < 7)
                                {
                                    _Mode = 1;
                                }
                                else if (paymentdetail.pastduedays == 0)
                                {
                                    _Mode = 0;
                                }
                                else
                                {
                                    _Mode = (paymentdetail.pastduedays % 7);
                                }
                                Double _PenalityCount = Convert.ToDouble(paymentdetail.pastduedays / 7);
                                List<string> s = Convert.ToString(_PenalityCount).Split('.').ToList();
                                _PenalityCount = Convert.ToDouble(s.First());
                                int _TotalPenalityCount = _Mode > 0 ? Convert.ToInt32(_PenalityCount + 1) : Convert.ToInt32(_PenalityCount);
                                paymentdetail.penalty = Math.Round((_TotalPenalityCount * penalty), 2);
                                paymentdetail._LateFee = Convert.ToString(paymentdetail.penalty);
                                double latefee = ((Convert.ToDouble(_balance) * (PercentPenalty)) * (paymentdetail.pastduedays));
                                paymentdetail._Penality = Convert.ToString(latefee);
                            }
                            if (paymentdetail.termtype == "Bi-Weekly")
                            {
                                double _Mode = 0;
                                if (paymentdetail.pastduedays != 0 && paymentdetail.pastduedays < 14)
                                {
                                    _Mode = 1;
                                }
                                else if (paymentdetail.pastduedays == 0)
                                {
                                    _Mode = 0;
                                }
                                else
                                {
                                    _Mode = (paymentdetail.pastduedays % 14);
                                }
                                Double _PenalityCount = Convert.ToDouble(paymentdetail.pastduedays / 14);
                                List<string> s = Convert.ToString(_PenalityCount).Split('.').ToList();
                                _PenalityCount = Convert.ToDouble(s.First());
                                int _TotalPenalityCount = _Mode > 0 ? Convert.ToInt32(_PenalityCount + 1) : Convert.ToInt32(_PenalityCount);
                                paymentdetail.penalty = Math.Round((_TotalPenalityCount * penalty), 2);
                                paymentdetail._LateFee = Convert.ToString(paymentdetail.penalty);
                                double latefee = ((Convert.ToDouble(_balance) * (PercentPenalty)) * (paymentdetail.pastduedays));
                                paymentdetail._Penality = Convert.ToString(latefee);
                            }
                            if (paymentdetail.termtype == "Monthly")
                            {
                                double _Mode = 0;

                                if (paymentdetail.pastduedays != 0 && paymentdetail.pastduedays < 28)
                                {
                                    _Mode = 1;
                                }
                                else if (paymentdetail.pastduedays == 0)
                                {
                                    _Mode = 0;
                                }
                                else
                                {
                                    _Mode = (paymentdetail.pastduedays % 28);
                                }
                                Double _PenalityCount = Convert.ToDouble(paymentdetail.pastduedays / 28);
                                List<string> s = Convert.ToString(_PenalityCount).Split('.').ToList();
                                _PenalityCount = Convert.ToDouble(s.First());
                                int _TotalPenalityCount = _Mode > 0 ? Convert.ToInt32(_PenalityCount + 1) : Convert.ToInt32(_PenalityCount);
                                paymentdetail.penalty = Math.Round((_TotalPenalityCount * penalty), 2);
                                paymentdetail._LateFee = Convert.ToString(paymentdetail.penalty);
                                double latefee = ((Convert.ToDouble(_balance) * (PercentPenalty)) * (paymentdetail.pastduedays));
                                paymentdetail._Penality = Convert.ToString(latefee);
                            }
                        }
                        paymentdetail.LateFee = Convert.ToDouble(paymentdetail._Penality);
                        paymentdetail._LateFee = Convert.ToString(paymentdetail.penalty);

                        paymentdetail.penalty = paymentdetail.penalty > 0 ? paymentdetail.penalty : 0;
                        paymentdetail._Penality = Convert.ToString(paymentdetail.penalty);
                        paymentdetail._DueAmount = Convert.ToString(DueAmount);
                        paymentdetail.PayableAmount = Convert.ToString(Convert.ToDouble(DueAmount) + Convert.ToDouble(paymentdetail._Penality) + paymentdetail.LateFee);
                    }
                    else
                    {
                        var StartDate = ptpdate;
                        double __TotalPenality = 0;
                        double __PenaltyAmount = 0;
                        double __Penality = 0;
                        double __lateFee = 0;

                        double __balance = Convert.ToDouble(_PaymentDetails.Rows[0]["emi_amount"]);
                        DateTime LastEMI = DateTime.Now;
                        foreach (DataRow item in _PaymentDetails.Rows)
                        {
                            _dayCount = (Convert.ToDateTime(item["paid_on"]).Date - Convert.ToDateTime(StartDate)).TotalDays;
                            if (__balance > 0 && paymentdetail.pastduedays > 0)
                            {
                                if (paymentdetail.termtype == "Weekly")
                                {
                                    __Penality = ((Convert.ToDouble(__balance) * (PercentPenalty)) * (_dayCount));
                                    double _Mode = 0;
                                    if (_dayCount != 0 && _dayCount < 7)
                                    {
                                        _Mode = 1;
                                    }
                                    else if (_dayCount == 0)
                                    {
                                        _Mode = 0;
                                    }
                                    else
                                    {
                                        _Mode = (_dayCount % 7);
                                    }
                                    Double _PenalityCount = Convert.ToDouble(_dayCount / 7);
                                    List<string> s = Convert.ToString(_PenalityCount).Split('.').ToList();
                                    _PenalityCount = Convert.ToDouble(s.First());
                                    int _TotalPenalityCount = _Mode > 0 ? Convert.ToInt32(_PenalityCount + 1) : Convert.ToInt32(_PenalityCount);
                                    __lateFee += Math.Round((_TotalPenalityCount * penalty), 2);
                                }
                                if (paymentdetail.termtype == "Bi-Weekly")
                                {
                                    __Penality = ((Convert.ToDouble(__balance) * (PercentPenalty)) * (_dayCount));
                                    double _Mode = 0;
                                    if (_dayCount != 0 && _dayCount < 14)
                                    {
                                        _Mode = 1;
                                    }
                                    else if (_dayCount == 0)
                                    {
                                        _Mode = 0;
                                    }
                                    else
                                    {
                                        _Mode = (_dayCount % 14);
                                    }
                                    Double _PenalityCount = Convert.ToDouble(_dayCount / 17);
                                    List<string> s = Convert.ToString(_PenalityCount).Split('.').ToList();
                                    _PenalityCount = Convert.ToDouble(s.First());
                                    int _TotalPenalityCount = _Mode > 0 ? Convert.ToInt32(_PenalityCount + 1) : Convert.ToInt32(_PenalityCount);
                                    __lateFee += Math.Round((_TotalPenalityCount * penalty), 2);
                                }
                                if (paymentdetail.termtype == "Monthly")
                                {
                                    __Penality = ((Convert.ToDouble(__balance) * (PercentPenalty)) * (_dayCount));
                                    double _Mode = 0;
                                    if (_dayCount != 0 && _dayCount < 28)
                                    {
                                        _Mode = 1;
                                    }
                                    else if (_dayCount == 0)
                                    {
                                        _Mode = 0;
                                    }
                                    else
                                    {
                                        _Mode = (_dayCount % 28);
                                    }
                                    Double _PenalityCount = Convert.ToDouble(_dayCount / 28);
                                    List<string> s = Convert.ToString(_PenalityCount).Split('.').ToList();
                                    _PenalityCount = Convert.ToDouble(s.First());
                                    int _TotalPenalityCount = _Mode > 0 ? Convert.ToInt32(_PenalityCount + 1) : Convert.ToInt32(_PenalityCount);
                                    __lateFee += Math.Round((_TotalPenalityCount * penalty), 2);
                                }
                            }
                            __balance -= Convert.ToDouble(item["paid_amount"]);
                            __PenaltyAmount += Convert.ToDouble(item["penalty_amount"]);
                            __TotalPenality += (__Penality + __lateFee);
                            LastEMI = Convert.ToDateTime(item["paid_on"]);
                        }
                        _dayCount = (DateTime.Now.Date - Convert.ToDateTime(LastEMI).Date).TotalDays;
                        if (_balance > 0 && paymentdetail.pastduedays > 0)
                        {
                            if (paymentdetail.termtype == "Weekly")
                            {
                                double _Mode = 0;
                                if (paymentdetail.pastduedays != 0 && paymentdetail.pastduedays < 7)
                                {
                                    _Mode = 1;
                                }
                                else if (paymentdetail.pastduedays == 0)
                                {
                                    _Mode = 0;
                                }
                                else
                                {
                                    _Mode = (paymentdetail.pastduedays % 7);
                                }
                                Double _PenalityCount = Convert.ToDouble(paymentdetail.pastduedays / 7);
                                List<string> s = Convert.ToString(_PenalityCount).Split('.').ToList();
                                _PenalityCount = Convert.ToDouble(s.First());
                                int _TotalPenalityCount = _Mode > 0 ? Convert.ToInt32(_PenalityCount + 1) : Convert.ToInt32(_PenalityCount);
                                paymentdetail.penalty = Math.Round((_TotalPenalityCount * penalty), 2);
                                paymentdetail._LateFee = Convert.ToString(paymentdetail.penalty);
                                double latefee = ((Convert.ToDouble(_balance) * (PercentPenalty)) * (_dayCount));
                                paymentdetail._Penality = Math.Round(latefee, 2).ToString();
                            }
                            if (paymentdetail.termtype == "Bi-Weekly")
                            {
                                double _Mode = 0;
                                if (paymentdetail.pastduedays != 0 && paymentdetail.pastduedays < 14)
                                {
                                    _Mode = 1;
                                }
                                else if (paymentdetail.pastduedays == 0)
                                {
                                    _Mode = 0;
                                }
                                else
                                {
                                    _Mode = (paymentdetail.pastduedays % 14);
                                }
                                Double _PenalityCount = Convert.ToDouble(paymentdetail.pastduedays / 14);
                                List<string> s = Convert.ToString(_PenalityCount).Split('.').ToList();
                                _PenalityCount = Convert.ToDouble(s.First());
                                int _TotalPenalityCount = _Mode > 0 ? Convert.ToInt32(_PenalityCount + 1) : Convert.ToInt32(_PenalityCount);
                                paymentdetail.penalty = Math.Round((_TotalPenalityCount * penalty), 2);
                                paymentdetail._LateFee = Convert.ToString(paymentdetail.penalty);
                                double latefee = ((Convert.ToDouble(_balance) * (PercentPenalty)) * (_dayCount));
                                paymentdetail._Penality = Math.Round(latefee, 2).ToString();
                            }
                            if (paymentdetail.termtype == "Monthly")
                            {
                                double _Mode = 0;
                                if (paymentdetail.pastduedays != 0 && paymentdetail.pastduedays < 28)
                                {
                                    _Mode = 1;
                                }
                                else if (paymentdetail.pastduedays == 0)
                                {
                                    _Mode = 0;
                                }
                                else
                                {
                                    _Mode = (paymentdetail.pastduedays % 28);
                                }
                                Double _PenalityCount = Convert.ToDouble(paymentdetail.pastduedays / 28);
                                List<string> s = Convert.ToString(_PenalityCount).Split('.').ToList();
                                _PenalityCount = Convert.ToDouble(s.First());
                                int _TotalPenalityCount = _Mode > 0 ? Convert.ToInt32(_PenalityCount + 1) : Convert.ToInt32(_PenalityCount);
                                paymentdetail.penalty = Math.Round((_TotalPenalityCount * penalty), 2);
                                paymentdetail._LateFee = Convert.ToString(paymentdetail.penalty);
                                double latefee = ((Convert.ToDouble(_balance) * (PercentPenalty)) * (_dayCount));
                                paymentdetail._Penality = Math.Round(latefee, 2).ToString();
                            }
                        }

                        var _totalDyasforPenalty = (DateTime.Now.Date - Convert.ToDateTime(StartDate).Date).TotalDays;
                        double _TotalPenalityfromLastEMIDate = 0;

                        if (paymentdetail.termtype == "Weekly")
                        {
                            double _Mode = 0;
                            if (_totalDyasforPenalty != 0 && _totalDyasforPenalty < 7)
                            {
                                _Mode = 1;
                            }
                            else if (_totalDyasforPenalty == 0)
                            {
                                _Mode = 0;
                            }
                            else
                            {
                                _Mode = (_totalDyasforPenalty % 7);
                            }
                            Double _PenalityCount = Convert.ToDouble(_totalDyasforPenalty / 7);
                            List<string> s = Convert.ToString(_PenalityCount).Split('.').ToList();
                            _PenalityCount = Convert.ToDouble(s.First());
                            int _TotalPenalityCount = _Mode > 0 ? Convert.ToInt32(_PenalityCount + 1) : Convert.ToInt32(_PenalityCount);
                            _TotalPenalityfromLastEMIDate = Math.Round((_TotalPenalityCount * penalty), 2);
                        }
                        if (paymentdetail.termtype == "Bi-Weekly")
                        {
                            double _Mode = 0;
                            if (_totalDyasforPenalty != 0 && _totalDyasforPenalty < 14)
                            {
                                _Mode = 1;
                            }
                            else if (_totalDyasforPenalty == 0)
                            {
                                _Mode = 0;
                            }
                            else
                            {
                                _Mode = (_totalDyasforPenalty % 14);
                            }
                            Double _PenalityCount = Convert.ToDouble(paymentdetail.pastduedays / 14);
                            List<string> s = Convert.ToString(_PenalityCount).Split('.').ToList();
                            _PenalityCount = Convert.ToDouble(s.First());
                            int _TotalPenalityCount = _Mode > 0 ? Convert.ToInt32(_PenalityCount + 1) : Convert.ToInt32(_PenalityCount);
                            _TotalPenalityfromLastEMIDate = Math.Round((_TotalPenalityCount * penalty), 2);
                        }
                        if (paymentdetail.termtype == "Monthly")
                        {
                            double _Mode = 0;
                            if (_totalDyasforPenalty != 0 && _totalDyasforPenalty < 28)
                            {
                                _Mode = 1;
                            }
                            else if (_totalDyasforPenalty == 0)
                            {
                                _Mode = 0;
                            }
                            else
                            {
                                _Mode = (_totalDyasforPenalty % 28);
                            }
                            Double _PenalityCount = Convert.ToDouble(paymentdetail.pastduedays / 28);
                            List<string> s = Convert.ToString(_PenalityCount).Split('.').ToList();
                            _PenalityCount = Convert.ToDouble(s.First());
                            int _TotalPenalityCount = _Mode > 0 ? Convert.ToInt32(_PenalityCount + 1) : Convert.ToInt32(_PenalityCount);
                            _TotalPenalityfromLastEMIDate = Math.Round((_TotalPenalityCount * penalty), 2);
                        }

                        paymentdetail.LateFee = Convert.ToDouble(paymentdetail._Penality);
                        paymentdetail._LateFee = Convert.ToString(paymentdetail.penalty);

                        paymentdetail.penalty = paymentdetail.penalty > 0 ? paymentdetail.penalty : 0;
                        paymentdetail._Penality = Convert.ToString(Math.Round(_TotalPenalityfromLastEMIDate, 2) - Math.Round(__lateFee, 2));
                        paymentdetail._DueAmount = Convert.ToString(DueAmount);
                        paymentdetail.PayableAmount = Math.Round(Convert.ToDouble(Convert.ToDouble(DueAmount) + Convert.ToDouble(paymentdetail.LateFee) + Convert.ToDouble(paymentdetail._Penality)), 2).ToString();
                    }
                    #endregion
                }

                return paymentdetail;
            }
            catch (Exception ex)
            {

                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                throw ex;
            }
        }
        public PaymentInformation GetCollecterListbyPhoneNumber(Int64 Applicationno, string ActionName)
        {
            PaymentInformation paymentdetail = new PaymentInformation();
            try
            {
                //int PastEmiCount = 0;
                DataTable dataTable;
                var connstring = ConfigurationManager.ConnectionStrings["NguccDefaultConnection"].ToString();
                string Campname = ConfigurationManager.AppSettings["LeadId_campname"];
                string Skillname = ConfigurationManager.AppSettings["LeadId_skillname"];
                string BroadcastingCampname = ConfigurationManager.AppSettings["BroadcastingCampaign"];
                string BroadcastingSkillname = ConfigurationManager.AppSettings["BroadcastingSkill"];
                //var PassedEMICount = DbHelper.SelectMethod(string.Format(QueryHelper.PastEmiCount, Applicationno));
                //if (PassedEMICount != null && PassedEMICount.Rows.Count > 0)
                //{
                //    PastEmiCount = PassedEMICount.Rows[0]["emicount"] == DBNull.Value ? 0 : Convert.ToInt32(PassedEMICount.Rows[0]["emicount"]);
                //}

                decimal _balance = 0;
                decimal __PaidPenality = 0;
                decimal __PaidAmount = 0;
                double _dayCount = 0;
                if (ActionName == "LoanPaymentBucket")
                {
                    dataTable = DbHelper.SelectMethod(string.Format(QueryHelper.GetLoanDetailListLatest, Applicationno));
                }
                else if (ActionName == "LoanPaymentSearch")
                {
                    dataTable = DbHelper.SelectMethod(string.Format(QueryHelper.GetLoanDetailListforSearch, Applicationno));
                }
                else
                {
                    dataTable = DbHelper.SelectMethod(string.Format(QueryHelper.GetLoanDetailListNew, Applicationno));
                }


                if (dataTable != null && dataTable.Rows.Count > 0)
                {
                    paymentdetail.notificationtoken = Convert.ToString(dataTable.Rows[0]["notificationtoken"]);
                    paymentdetail.Name = Convert.ToString(dataTable.Rows[0]["first_name"]) + " " + Convert.ToString(dataTable.Rows[0]["middle_name"]) + " " + Convert.ToString(dataTable.Rows[0]["last_name"]);
                    paymentdetail.applicationno = Convert.ToInt32(dataTable.Rows[0]["applicationno"]);
                    paymentdetail.Referenceno = Convert.ToString(dataTable.Rows[0]["reference_no"]);
                    paymentdetail.Termtype = Convert.ToString(dataTable.Rows[0]["termtype"]);
                    paymentdetail.emiamount = Convert.ToString(dataTable.Rows[0]["emi_amount"]);
                    paymentdetail.Term = Convert.ToString(dataTable.Rows[0]["term_name"]);
                    paymentdetail.disbursemant = Convert.ToString(dataTable.Rows[0]["disbursement_date"]);
                    paymentdetail.Datepaid = Convert.ToString(dataTable.Rows[0]["date_paid"]);
                    paymentdetail.jumioreference = Convert.ToString(dataTable.Rows[0]["jumioreference"]);
                    paymentdetail.facemapstatus = Convert.ToString(dataTable.Rows[0]["facemapstatus"]);
                    paymentdetail.Besttimetocallback = Convert.ToString(dataTable.Rows[0]["best_time_callback"]);
                    paymentdetail.nonpaymentreason = Convert.ToString(dataTable.Rows[0]["non_payment_reason"]);
                    paymentdetail.currentdateptp = Convert.ToString(dataTable.Rows[0]["current_date_ptp"]);
                    paymentdetail.currentamountptp = Convert.ToString(dataTable.Rows[0]["current_amount_ptp"]);
                    paymentdetail.Otherremarks = Convert.ToString(dataTable.Rows[0]["other_remarks"]);
                    paymentdetail.personalcontactno = Convert.ToString(dataTable.Rows[0]["personalcontactno"]);
                    paymentdetail.Contractno = Convert.ToString(dataTable.Rows[0]["contract_ref_no"]);
                    paymentdetail.EmailId = Convert.ToString(dataTable.Rows[0]["personalemail"]);
                    paymentdetail.Dueedate = Convert.ToString(dataTable.Rows[0]["emi_date"]);
                    paymentdetail.familyname1 = Convert.ToString(dataTable.Rows[0]["family_name1"]);
                    paymentdetail.familyrelation1 = Convert.ToString(dataTable.Rows[0]["family_relation1"]);
                    paymentdetail.familycontact1 = Convert.ToString(dataTable.Rows[0]["family_contact1"]);
                    paymentdetail.familyname2 = Convert.ToString(dataTable.Rows[0]["family_name2"]);
                    paymentdetail.familyrelation2 = Convert.ToString(dataTable.Rows[0]["family_relation2"]);
                    paymentdetail.familycontact2 = Convert.ToString(dataTable.Rows[0]["family_contact2"]);
                    paymentdetail.ptpdate = Convert.ToString(dataTable.Rows[0]["current_date_ptp"]);
                    paymentdetail.checker = Convert.ToString(dataTable.Rows[0]["checker"]);
                    paymentdetail.verifier = Convert.ToString(dataTable.Rows[0]["verifier"]);
                    paymentdetail.emiid = Convert.ToInt64(dataTable.Rows[0]["emi_id"]);
                    var InterRate = Convert.ToDecimal(dataTable.Rows[0]["interest_rate"]);
                    var DueAmount = Convert.ToString(dataTable.Rows[0]["due_amount"]);
                    paymentdetail.pastduedays = Convert.ToInt32(dataTable.Rows[0]["aging"]);
                    paymentdetail.UserID = DBNull.Value.Equals(dataTable.Rows[0]["user_id"]) ? 0 : Convert.ToInt32(dataTable.Rows[0]["user_id"]);
                    string ptpdate = string.Empty;
                    DataTable _PaymentDetails = new DataTable();
                    for (int i = 0; i < dataTable.Rows.Count; i++)
                    {
                        if (Convert.ToString(dataTable.Rows[i]["emi_date"]).Contains('-'))
                        {
                            var DataSplit = Convert.ToString(dataTable.Rows[i]["emi_date"]).Split('-').ToList();
                            ptpdate = new DateTime(Convert.ToInt32(DataSplit[2]), Convert.ToInt32(DataSplit[1]), Convert.ToInt32(DataSplit[0])).ToString("yyyy-MM-dd");
                            if (Convert.ToDateTime(ptpdate).Date < DateTime.Now.Date)
                            {
                                _balance += Convert.ToDecimal(dataTable.Rows[i]["due_amount"]);
                            }
                            var _PenalityData = DbHelper.SelectMethod(String.Format(QueryHelper.getPaidPenality, ptpdate, paymentdetail.applicationno));
                            if (_PenalityData != null && _PenalityData.Rows.Count > 0)
                            {
                                if (!String.IsNullOrWhiteSpace(Convert.ToString(_PenalityData.Rows[0]["penalty_amount"])))
                                {
                                    __PaidPenality += Convert.ToDecimal(_PenalityData.Rows[0]["penalty_amount"]);
                                    __PaidAmount += Convert.ToDecimal(_PenalityData.Rows[0]["paid_amount"]);
                                }
                            }
                            _PaymentDetails = DbHelper.SelectMethod($"SELECT distinct a.payment_id, a.application_no, a.paid_amount, a.paid_on, a.updatedon, a.updatedby, a.whatpaymentchennel, a.proofofpayment, a.paymentapproved, a.isdeleted, a.penalty_amount, a.remarks, a.is_pay, a.is_partial, c.emi_amount FROM public.tblapplication_emi_payment a join tblapplication_emi b on a.application_no=b.application_no join tblapplication_emi_details c on b.emi_id=c.emi_id where a.application_no={Applicationno} and a.paymentapproved=true and a.is_pay=false order by payment_id;");
                        }
                    }

                    var __Term = Convert.ToString(dataTable.Rows[0]["term_name"]).Split(' ').ToList();
                    paymentdetail.termtype = Convert.ToString(dataTable.Rows[0]["termtypename"]);

                    paymentdetail.outstanding = Convert.ToDecimal(dataTable.Rows[0]["totalbalanceamount"]);
                    paymentdetail.principal = Convert.ToString(dataTable.Rows[0]["approved_loan_amount"]);
                    paymentdetail.amortization = Convert.ToString((Convert.ToDecimal(dataTable.Rows[0]["approved_loan_amount"]) + ((Convert.ToDecimal(dataTable.Rows[0]["approved_loan_amount"]) * Convert.ToDecimal(dataTable.Rows[0]["interest_rate"])) / 100)) / Convert.ToDecimal(__Term.FirstOrDefault()));
                    DataTable dataTable1 = DbHelper.SelectMethod(string.Format(QueryHelper.SearchUserIdByApplicationNo, Applicationno));
                    if (dataTable1 != null && dataTable1.Rows.Count > 0)
                    {
                        int userid;
                        userid = DBNull.Value.Equals(dataTable1.Rows[0]["user_id"]) ? 0 : Convert.ToInt32(dataTable1.Rows[0]["user_id"]);
                        var query = DbHelper.SelectMethod(string.Format(QueryHelper.SearchApplicationNoByUserId, userid));
                        if (query != null && query.Rows.Count > 0)
                        {
                            paymentdetail.ReloanCount = query.Rows.Count;
                        }
                    }
                    double PercentPenalty = (Convert.ToDouble(Convert.ToString(dataTable.Rows[0]["late_rate"])) / 100);
                    double penalty = Convert.ToDouble(Convert.ToString(dataTable.Rows[0]["late_penalty"]));
                    #region New Code
                    if (_PaymentDetails == null || _PaymentDetails.Rows.Count == 0)
                    {
                        if (_balance > 0 && paymentdetail.pastduedays > 0)
                        {
                            if (paymentdetail.termtype == "Weekly")
                            {
                                double _Mode = 0;
                                if (paymentdetail.pastduedays != 0 && paymentdetail.pastduedays < 7)
                                {
                                    _Mode = 1;
                                }
                                else if (paymentdetail.pastduedays == 0)
                                {
                                    _Mode = 0;
                                }
                                else
                                {
                                    _Mode = (paymentdetail.pastduedays % 7);
                                }
                                Double _PenalityCount = Convert.ToDouble(paymentdetail.pastduedays / 7);
                                List<string> s = Convert.ToString(_PenalityCount).Split('.').ToList();
                                _PenalityCount = Convert.ToDouble(s.First());
                                int _TotalPenalityCount = _Mode > 0 ? Convert.ToInt32(_PenalityCount + 1) : Convert.ToInt32(_PenalityCount);
                                paymentdetail.penalty = Math.Round((_TotalPenalityCount * penalty), 2);
                                paymentdetail._LateFee = Convert.ToString(paymentdetail.penalty);
                                double latefee = ((Convert.ToDouble(_balance) * (PercentPenalty)) * (paymentdetail.pastduedays));
                                paymentdetail._Penality = Convert.ToString(latefee);
                            }
                            if (paymentdetail.termtype == "Bi-Weekly")
                            {
                                double _Mode = 0;
                                if (paymentdetail.pastduedays != 0 && paymentdetail.pastduedays < 14)
                                {
                                    _Mode = 1;
                                }
                                else if (paymentdetail.pastduedays == 0)
                                {
                                    _Mode = 0;
                                }
                                else
                                {
                                    _Mode = (paymentdetail.pastduedays % 14);
                                }
                                Double _PenalityCount = Convert.ToDouble(paymentdetail.pastduedays / 14);
                                List<string> s = Convert.ToString(_PenalityCount).Split('.').ToList();
                                _PenalityCount = Convert.ToDouble(s.First());
                                int _TotalPenalityCount = _Mode > 0 ? Convert.ToInt32(_PenalityCount + 1) : Convert.ToInt32(_PenalityCount);
                                paymentdetail.penalty = Math.Round((_TotalPenalityCount * penalty), 2);
                                paymentdetail._LateFee = Convert.ToString(paymentdetail.penalty);
                                double latefee = ((Convert.ToDouble(_balance) * (PercentPenalty)) * (paymentdetail.pastduedays));
                                paymentdetail._Penality = Convert.ToString(latefee);
                            }
                            if (paymentdetail.termtype == "Monthly")
                            {
                                double _Mode = 0;

                                if (paymentdetail.pastduedays != 0 && paymentdetail.pastduedays < 28)
                                {
                                    _Mode = 1;
                                }
                                else if (paymentdetail.pastduedays == 0)
                                {
                                    _Mode = 0;
                                }
                                else
                                {
                                    _Mode = (paymentdetail.pastduedays % 28);
                                }
                                Double _PenalityCount = Convert.ToDouble(paymentdetail.pastduedays / 28);
                                List<string> s = Convert.ToString(_PenalityCount).Split('.').ToList();
                                _PenalityCount = Convert.ToDouble(s.First());
                                int _TotalPenalityCount = _Mode > 0 ? Convert.ToInt32(_PenalityCount + 1) : Convert.ToInt32(_PenalityCount);
                                paymentdetail.penalty = Math.Round((_TotalPenalityCount * penalty), 2);
                                paymentdetail._LateFee = Convert.ToString(paymentdetail.penalty);
                                double latefee = ((Convert.ToDouble(_balance) * (PercentPenalty)) * (paymentdetail.pastduedays));
                                paymentdetail._Penality = Convert.ToString(latefee);
                            }
                        }
                        paymentdetail.LateFee = Convert.ToDouble(paymentdetail._Penality);
                        paymentdetail._LateFee = Convert.ToString(paymentdetail.penalty);

                        paymentdetail.penalty = paymentdetail.penalty > 0 ? paymentdetail.penalty : 0;
                        paymentdetail._Penality = Convert.ToString(paymentdetail.penalty);
                        paymentdetail._DueAmount = Convert.ToString(DueAmount);
                        paymentdetail.PayableAmount = Convert.ToString(Convert.ToDouble(DueAmount) + Convert.ToDouble(paymentdetail._Penality) + paymentdetail.LateFee);
                    }
                    else
                    {
                        var StartDate = ptpdate;
                        double __TotalPenality = 0;
                        double __PenaltyAmount = 0;
                        double __Penality = 0;
                        double __lateFee = 0;

                        double __balance = Convert.ToDouble(_PaymentDetails.Rows[0]["emi_amount"]);
                        DateTime LastEMI = DateTime.Now;
                        foreach (DataRow item in _PaymentDetails.Rows)
                        {
                            _dayCount = (Convert.ToDateTime(item["paid_on"]).Date - Convert.ToDateTime(StartDate)).TotalDays;
                            if (__balance > 0 && paymentdetail.pastduedays > 0)
                            {
                                if (paymentdetail.termtype == "Weekly")
                                {
                                    __Penality = ((Convert.ToDouble(__balance) * (PercentPenalty)) * (_dayCount));
                                    double _Mode = 0;
                                    if (_dayCount != 0 && _dayCount < 7)
                                    {
                                        _Mode = 1;
                                    }
                                    else if (_dayCount == 0)
                                    {
                                        _Mode = 0;
                                    }
                                    else
                                    {
                                        _Mode = (_dayCount % 7);
                                    }
                                    Double _PenalityCount = Convert.ToDouble(_dayCount / 7);
                                    List<string> s = Convert.ToString(_PenalityCount).Split('.').ToList();
                                    _PenalityCount = Convert.ToDouble(s.First());
                                    int _TotalPenalityCount = _Mode > 0 ? Convert.ToInt32(_PenalityCount + 1) : Convert.ToInt32(_PenalityCount);
                                    __lateFee += Math.Round((_TotalPenalityCount * penalty), 2);
                                }
                                if (paymentdetail.termtype == "Bi-Weekly")
                                {
                                    __Penality = ((Convert.ToDouble(__balance) * (PercentPenalty)) * (_dayCount));
                                    double _Mode = 0;
                                    if (_dayCount != 0 && _dayCount < 14)
                                    {
                                        _Mode = 1;
                                    }
                                    else if (_dayCount == 0)
                                    {
                                        _Mode = 0;
                                    }
                                    else
                                    {
                                        _Mode = (_dayCount % 14);
                                    }
                                    Double _PenalityCount = Convert.ToDouble(_dayCount / 17);
                                    List<string> s = Convert.ToString(_PenalityCount).Split('.').ToList();
                                    _PenalityCount = Convert.ToDouble(s.First());
                                    int _TotalPenalityCount = _Mode > 0 ? Convert.ToInt32(_PenalityCount + 1) : Convert.ToInt32(_PenalityCount);
                                    __lateFee += Math.Round((_TotalPenalityCount * penalty), 2);
                                }
                                if (paymentdetail.termtype == "Monthly")
                                {
                                    __Penality = ((Convert.ToDouble(__balance) * (PercentPenalty)) * (_dayCount));
                                    double _Mode = 0;
                                    if (_dayCount != 0 && _dayCount < 28)
                                    {
                                        _Mode = 1;
                                    }
                                    else if (_dayCount == 0)
                                    {
                                        _Mode = 0;
                                    }
                                    else
                                    {
                                        _Mode = (_dayCount % 28);
                                    }
                                    Double _PenalityCount = Convert.ToDouble(_dayCount / 28);
                                    List<string> s = Convert.ToString(_PenalityCount).Split('.').ToList();
                                    _PenalityCount = Convert.ToDouble(s.First());
                                    int _TotalPenalityCount = _Mode > 0 ? Convert.ToInt32(_PenalityCount + 1) : Convert.ToInt32(_PenalityCount);
                                    __lateFee += Math.Round((_TotalPenalityCount * penalty), 2);
                                }
                            }
                            __balance -= Convert.ToDouble(item["paid_amount"]);
                            __PenaltyAmount += Convert.ToDouble(item["penalty_amount"]);
                            __TotalPenality += (__Penality + __lateFee);
                            LastEMI = Convert.ToDateTime(item["paid_on"]);
                        }
                        _dayCount = (DateTime.Now.Date - Convert.ToDateTime(LastEMI).Date).TotalDays;
                        if (_balance > 0 && paymentdetail.pastduedays > 0)
                        {
                            if (paymentdetail.termtype == "Weekly")
                            {
                                double _Mode = 0;
                                if (paymentdetail.pastduedays != 0 && paymentdetail.pastduedays < 7)
                                {
                                    _Mode = 1;
                                }
                                else if (paymentdetail.pastduedays == 0)
                                {
                                    _Mode = 0;
                                }
                                else
                                {
                                    _Mode = (paymentdetail.pastduedays % 7);
                                }
                                Double _PenalityCount = Convert.ToDouble(paymentdetail.pastduedays / 7);
                                List<string> s = Convert.ToString(_PenalityCount).Split('.').ToList();
                                _PenalityCount = Convert.ToDouble(s.First());
                                int _TotalPenalityCount = _Mode > 0 ? Convert.ToInt32(_PenalityCount + 1) : Convert.ToInt32(_PenalityCount);
                                paymentdetail.penalty = Math.Round((_TotalPenalityCount * penalty), 2);
                                paymentdetail._LateFee = Convert.ToString(paymentdetail.penalty);
                                double latefee = ((Convert.ToDouble(_balance) * (PercentPenalty)) * (_dayCount));
                                paymentdetail._Penality = Math.Round(latefee, 2).ToString();
                            }
                            if (paymentdetail.termtype == "Bi-Weekly")
                            {
                                double _Mode = 0;
                                if (paymentdetail.pastduedays != 0 && paymentdetail.pastduedays < 14)
                                {
                                    _Mode = 1;
                                }
                                else if (paymentdetail.pastduedays == 0)
                                {
                                    _Mode = 0;
                                }
                                else
                                {
                                    _Mode = (paymentdetail.pastduedays % 14);
                                }
                                Double _PenalityCount = Convert.ToDouble(paymentdetail.pastduedays / 14);
                                List<string> s = Convert.ToString(_PenalityCount).Split('.').ToList();
                                _PenalityCount = Convert.ToDouble(s.First());
                                int _TotalPenalityCount = _Mode > 0 ? Convert.ToInt32(_PenalityCount + 1) : Convert.ToInt32(_PenalityCount);
                                paymentdetail.penalty = Math.Round((_TotalPenalityCount * penalty), 2);
                                paymentdetail._LateFee = Convert.ToString(paymentdetail.penalty);
                                double latefee = ((Convert.ToDouble(_balance) * (PercentPenalty)) * (_dayCount));
                                paymentdetail._Penality = Math.Round(latefee, 2).ToString();
                            }
                            if (paymentdetail.termtype == "Monthly")
                            {
                                double _Mode = 0;
                                if (paymentdetail.pastduedays != 0 && paymentdetail.pastduedays < 28)
                                {
                                    _Mode = 1;
                                }
                                else if (paymentdetail.pastduedays == 0)
                                {
                                    _Mode = 0;
                                }
                                else
                                {
                                    _Mode = (paymentdetail.pastduedays % 28);
                                }
                                Double _PenalityCount = Convert.ToDouble(paymentdetail.pastduedays / 28);
                                List<string> s = Convert.ToString(_PenalityCount).Split('.').ToList();
                                _PenalityCount = Convert.ToDouble(s.First());
                                int _TotalPenalityCount = _Mode > 0 ? Convert.ToInt32(_PenalityCount + 1) : Convert.ToInt32(_PenalityCount);
                                paymentdetail.penalty = Math.Round((_TotalPenalityCount * penalty), 2);
                                paymentdetail._LateFee = Convert.ToString(paymentdetail.penalty);
                                double latefee = ((Convert.ToDouble(_balance) * (PercentPenalty)) * (_dayCount));
                                paymentdetail._Penality = Math.Round(latefee, 2).ToString();
                            }
                        }

                        var _totalDyasforPenalty = (DateTime.Now.Date - Convert.ToDateTime(StartDate).Date).TotalDays;
                        double _TotalPenalityfromLastEMIDate = 0;

                        if (paymentdetail.termtype == "Weekly")
                        {
                            double _Mode = 0;
                            if (_totalDyasforPenalty != 0 && _totalDyasforPenalty < 7)
                            {
                                _Mode = 1;
                            }
                            else if (_totalDyasforPenalty == 0)
                            {
                                _Mode = 0;
                            }
                            else
                            {
                                _Mode = (_totalDyasforPenalty % 7);
                            }
                            Double _PenalityCount = Convert.ToDouble(_totalDyasforPenalty / 7);
                            List<string> s = Convert.ToString(_PenalityCount).Split('.').ToList();
                            _PenalityCount = Convert.ToDouble(s.First());
                            int _TotalPenalityCount = _Mode > 0 ? Convert.ToInt32(_PenalityCount + 1) : Convert.ToInt32(_PenalityCount);
                            _TotalPenalityfromLastEMIDate = Math.Round((_TotalPenalityCount * penalty), 2);
                        }
                        if (paymentdetail.termtype == "Bi-Weekly")
                        {
                            double _Mode = 0;
                            if (_totalDyasforPenalty != 0 && _totalDyasforPenalty < 14)
                            {
                                _Mode = 1;
                            }
                            else if (_totalDyasforPenalty == 0)
                            {
                                _Mode = 0;
                            }
                            else
                            {
                                _Mode = (_totalDyasforPenalty % 14);
                            }
                            Double _PenalityCount = Convert.ToDouble(paymentdetail.pastduedays / 14);
                            List<string> s = Convert.ToString(_PenalityCount).Split('.').ToList();
                            _PenalityCount = Convert.ToDouble(s.First());
                            int _TotalPenalityCount = _Mode > 0 ? Convert.ToInt32(_PenalityCount + 1) : Convert.ToInt32(_PenalityCount);
                            _TotalPenalityfromLastEMIDate = Math.Round((_TotalPenalityCount * penalty), 2);
                        }
                        if (paymentdetail.termtype == "Monthly")
                        {
                            double _Mode = 0;
                            if (_totalDyasforPenalty != 0 && _totalDyasforPenalty < 28)
                            {
                                _Mode = 1;
                            }
                            else if (_totalDyasforPenalty == 0)
                            {
                                _Mode = 0;
                            }
                            else
                            {
                                _Mode = (_totalDyasforPenalty % 28);
                            }
                            Double _PenalityCount = Convert.ToDouble(paymentdetail.pastduedays / 28);
                            List<string> s = Convert.ToString(_PenalityCount).Split('.').ToList();
                            _PenalityCount = Convert.ToDouble(s.First());
                            int _TotalPenalityCount = _Mode > 0 ? Convert.ToInt32(_PenalityCount + 1) : Convert.ToInt32(_PenalityCount);
                            _TotalPenalityfromLastEMIDate = Math.Round((_TotalPenalityCount * penalty), 2);
                        }

                        paymentdetail.LateFee = Convert.ToDouble(paymentdetail._Penality);
                        paymentdetail._LateFee = Convert.ToString(paymentdetail.penalty);

                        paymentdetail.penalty = paymentdetail.penalty > 0 ? paymentdetail.penalty : 0;
                        paymentdetail._Penality = Convert.ToString(Math.Round(_TotalPenalityfromLastEMIDate, 2) - Math.Round(__lateFee, 2));
                        paymentdetail._DueAmount = Convert.ToString(DueAmount);
                        paymentdetail.PayableAmount = Math.Round(Convert.ToDouble(Convert.ToDouble(DueAmount) + Convert.ToDouble(paymentdetail.LateFee) + Convert.ToDouble(paymentdetail._Penality)), 2).ToString();
                    }
                    #endregion
                }

                if (!String.IsNullOrWhiteSpace(connstring))
                {
                    NpgsqlConnection conn = new NpgsqlConnection(connstring);
                    conn.Open();
                    DataTable _Data = new DataTable();

                    if (!String.IsNullOrWhiteSpace(paymentdetail.personalcontactno) && !String.IsNullOrWhiteSpace(BroadcastingCampname) && !String.IsNullOrWhiteSpace(BroadcastingSkillname))
                    {
                        var sqlBroadcastLastcall = string.Format(QueryHelper.Getvoicemsg, paymentdetail.personalcontactno, BroadcastingCampname, BroadcastingSkillname);
                        NpgsqlDataAdapter dalast = new NpgsqlDataAdapter(sqlBroadcastLastcall, conn);
                        _Data = new DataTable();
                        dalast.Fill(_Data);
                        if (_Data != null && _Data.Rows.Count > 0)
                        {
                            paymentdetail.HistoryDateandTimeLastCall = Convert.ToString(_Data.Rows[0]["starttime"]);
                        }
                    }
                    if (!String.IsNullOrWhiteSpace(paymentdetail.personalcontactno) && !String.IsNullOrWhiteSpace(Campname) && !String.IsNullOrWhiteSpace(Skillname))
                    {
                        var sqlvoicemasg = string.Format(QueryHelper.Getvoicemsg, paymentdetail.personalcontactno, Campname, Skillname);
                        NpgsqlDataAdapter da = new NpgsqlDataAdapter(sqlvoicemasg, conn);
                        _Data = new DataTable();
                        da.Fill(_Data);
                        if (_Data != null && _Data.Rows.Count > 0)
                        {
                            paymentdetail.HistoryDateandTimeVoice = Convert.ToString(_Data.Rows[0]["starttime"]);
                        }
                    }

                    if (paymentdetail.personalcontactno != "" && paymentdetail.personalcontactno != null)
                    {
                        var sqlnumberofcall = string.Format(QueryHelper.Getnoofcall, paymentdetail.personalcontactno);
                        var danoofcall = new NpgsqlDataAdapter(sqlnumberofcall, conn);
                        _Data = new DataTable();
                        danoofcall.Fill(_Data);
                        if (_Data != null && _Data.Rows.Count > 0)
                        {
                            paymentdetail.HistoryNoOfTimeCall = Convert.ToString(_Data.Rows[0]["numberofcall"]);
                        }
                    }
                }
                if (String.IsNullOrWhiteSpace(paymentdetail.PayableAmount))
                {
                    paymentdetail.PayableAmount = "0";
                }

                var historyquery = DbHelper.SelectMethod(string.Format(QueryHelper.GetHistory, paymentdetail.applicationno));
                if (historyquery != null && historyquery.Rows.Count > 0)
                {
                    paymentdetail.HistoryAmountPaid = Convert.ToString(historyquery.Rows[0]["paid_amount"]);
                    paymentdetail.Historydatepaid = Convert.ToString(historyquery.Rows[0]["paid_on"]);
                }

                var historyptpquery = DbHelper.SelectMethod(string.Format(QueryHelper.GetptpHistory, paymentdetail.applicationno));
                if (historyptpquery != null && historyptpquery.Rows.Count > 0)
                {
                    paymentdetail.HistoryBestTimeToCallBack = Convert.ToString(historyptpquery.Rows[0]["best_time_callback"]);
                    paymentdetail.HistoryPTP = Convert.ToString(historyptpquery.Rows[0]["current_date_ptp"]);
                    paymentdetail.HistoryReasonOfNonPayment = Convert.ToString(historyptpquery.Rows[0]["non_payment_reason"]);
                    paymentdetail.HistoryOtherRemarks = Convert.ToString(historyptpquery.Rows[0]["other_remarks"]);
                    paymentdetail.HistoryAmountPTP = Convert.ToString(historyptpquery.Rows[0]["current_amount_ptp"]);
                }
                if (paymentdetail.Termtype != "" && paymentdetail.Termtype != null)
                {
                    try
                    {
                        string TermType = Convert.ToString(paymentdetail.Termtype) != string.Empty ? Convert.ToString(paymentdetail.Termtype) : string.Empty;
                        if (!String.IsNullOrWhiteSpace(TermType))
                            TermType = GetTermType(TermType);
                        string TermName = Convert.ToString(paymentdetail.Term) != string.Empty ? Convert.ToString(paymentdetail.Term) : string.Empty;

                        string AdditionalRequestTermStr = TermType + ", " + TermName;
                        paymentdetail.Term = AdditionalRequestTermStr;
                    }
                    catch (Exception ex)
                    {
                        logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                    }
                }
                var paidquery = DbHelper.SelectMethod(string.Format(QueryHelper.GetPaidAmount, paymentdetail.applicationno));
                if (paidquery != null && paidquery.Rows.Count > 0)
                {
                    paymentdetail.totalamountpaid = Convert.ToString(paidquery.Rows[0]["totalpaid"]);
                }
                var balancequery = DbHelper.SelectMethod(string.Format(QueryHelper.GetSumEmiAmount, paymentdetail.applicationno));
                if (balancequery != null && balancequery.Rows.Count > 0)
                {
                    paymentdetail.balanceamt = Convert.ToString(balancequery.Rows[0]["balanceamount"]);
                }
                if (paymentdetail.balanceamt == "" && paymentdetail.balanceamt == null)
                {
                    paymentdetail.totalamountpaid = "0.00";
                    paymentdetail.balanceamt = "0.00";
                }
                var dudate = DbHelper.SelectMethod(string.Format(QueryHelper.GetDuDateNew, paymentdetail.applicationno));
                if (dudate != null && dudate.Rows.Count > 0)
                {
                    paymentdetail.Dueedate = Convert.ToString(dudate.Rows[0]["emi_date"]);
                }
                return paymentdetail;
            }
            catch (Exception ex)
            {

                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                throw ex;
            }
        }


        public PaymentInformation GetCalculatedPenalityByDateApplicationNo(Int64 Applicationno, DateTime date)
        {
            PaymentInformation paymentdetail = new PaymentInformation();
            try
            {
                // int PastEmiCount = 0;
                decimal _balance = 0;
                decimal __PaidPenality = 0;
                decimal __PaidAmount = 0;
                double _dayCount = 0;
                //DataTable dataTable;
                //var PassedEMICount = DbHelper.SelectMethod(string.Format(QueryHelper.PastEmiCountByDate, Applicationno, date.ToString("yyyy-MM-dd")));
                //if (PassedEMICount != null && PassedEMICount.Rows.Count > 0)
                //{
                //    PastEmiCount = PassedEMICount.Rows[0]["emicount"] == DBNull.Value ? 0 : Convert.ToInt32(PassedEMICount.Rows[0]["emicount"]);
                //}

                DataTable dataTable = DbHelper.SelectMethod(string.Format(QueryHelper.GetLoanDetailListByDate, Applicationno, date.ToString("yyyy-MM-dd")));

                if (dataTable != null && dataTable.Rows.Count > 0)
                {
                    paymentdetail.Name = Convert.ToString(dataTable.Rows[0]["first_name"]) + " " + Convert.ToString(dataTable.Rows[0]["middle_name"]) + " " + Convert.ToString(dataTable.Rows[0]["last_name"]);
                    paymentdetail.applicationno = Convert.ToInt32(dataTable.Rows[0]["applicationno"]);
                    paymentdetail.Referenceno = Convert.ToString(dataTable.Rows[0]["reference_no"]);
                    paymentdetail.Termtype = Convert.ToString(dataTable.Rows[0]["termtype"]);
                    paymentdetail.Term = Convert.ToString(dataTable.Rows[0]["term_name"]);
                    paymentdetail.disbursemant = Convert.ToString(dataTable.Rows[0]["disbursement_date"]);
                    paymentdetail.Datepaid = Convert.ToString(dataTable.Rows[0]["date_paid"]);
                    paymentdetail.jumioreference = Convert.ToString(dataTable.Rows[0]["jumioreference"]);
                    paymentdetail.facemapstatus = Convert.ToString(dataTable.Rows[0]["facemapstatus"]);
                    paymentdetail.Besttimetocallback = Convert.ToString(dataTable.Rows[0]["best_time_callback"]);
                    paymentdetail.nonpaymentreason = Convert.ToString(dataTable.Rows[0]["non_payment_reason"]);
                    paymentdetail.currentdateptp = Convert.ToString(dataTable.Rows[0]["current_date_ptp"]);
                    paymentdetail.currentamountptp = Convert.ToString(dataTable.Rows[0]["current_amount_ptp"]);
                    paymentdetail.Otherremarks = Convert.ToString(dataTable.Rows[0]["other_remarks"]);
                    paymentdetail.personalcontactno = Convert.ToString(dataTable.Rows[0]["personalcontactno"]);
                    paymentdetail.Contractno = Convert.ToString(dataTable.Rows[0]["contract_ref_no"]);
                    paymentdetail.EmailId = Convert.ToString(dataTable.Rows[0]["personalemail"]);
                    paymentdetail.Dueedate = Convert.ToString(dataTable.Rows[0]["emi_date"]);
                    paymentdetail.familyname1 = Convert.ToString(dataTable.Rows[0]["family_name1"]);
                    paymentdetail.familyrelation1 = Convert.ToString(dataTable.Rows[0]["family_relation1"]);
                    paymentdetail.familycontact1 = Convert.ToString(dataTable.Rows[0]["family_contact1"]);
                    paymentdetail.familyname2 = Convert.ToString(dataTable.Rows[0]["family_name2"]);
                    paymentdetail.familyrelation2 = Convert.ToString(dataTable.Rows[0]["family_relation2"]);
                    paymentdetail.familycontact2 = Convert.ToString(dataTable.Rows[0]["family_contact2"]);
                    paymentdetail.ptpdate = Convert.ToString(dataTable.Rows[0]["current_date_ptp"]);
                    paymentdetail.emiid = Convert.ToInt64(dataTable.Rows[0]["emi_id"]);
                    var InterRate = Convert.ToDecimal(dataTable.Rows[0]["interest_rate"]);
                    var DueAmount = Convert.ToString(dataTable.Rows[0]["due_amount"]);
                    paymentdetail.pastduedays = Convert.ToInt32(dataTable.Rows[0]["aging"]);
                    paymentdetail.UserID = DBNull.Value.Equals(dataTable.Rows[0]["user_id"]) ? 0 : Convert.ToInt32(dataTable.Rows[0]["user_id"]);
                    string ptpdate = string.Empty;
                    DataTable _PaymentDetails = new DataTable();
                    for (int i = 0; i < dataTable.Rows.Count; i++)
                    {
                        if (Convert.ToString(dataTable.Rows[i]["emi_date"]).Contains('-'))
                        {
                            var DataSplit = Convert.ToString(dataTable.Rows[i]["emi_date"]).Split('-').ToList();
                            ptpdate = new DateTime(Convert.ToInt32(DataSplit[2]), Convert.ToInt32(DataSplit[1]), Convert.ToInt32(DataSplit[0])).ToString("yyyy-MM-dd");
                            if (Convert.ToDateTime(ptpdate).Date <= date.Date)
                            {
                                _balance += Convert.ToDecimal(dataTable.Rows[i]["due_amount"]);
                            }
                            var _PenalityData = DbHelper.SelectMethod(String.Format(QueryHelper.getPaidPenality, ptpdate, paymentdetail.applicationno));
                            if (_PenalityData != null && _PenalityData.Rows.Count > 0)
                            {
                                if (!String.IsNullOrWhiteSpace(Convert.ToString(_PenalityData.Rows[0]["penalty_amount"])))
                                {
                                    __PaidPenality += Convert.ToDecimal(_PenalityData.Rows[0]["penalty_amount"]);
                                    __PaidAmount += Convert.ToDecimal(_PenalityData.Rows[0]["paid_amount"]);
                                }
                            }
                            _PaymentDetails = DbHelper.SelectMethod($"SELECT distinct a.payment_id, a.application_no, a.paid_amount, a.paid_on, a.updatedon, a.updatedby, a.whatpaymentchennel, a.proofofpayment, a.paymentapproved, a.isdeleted, a.penalty_amount, a.remarks, a.is_pay, a.is_partial, c.emi_amount FROM public.tblapplication_emi_payment a join tblapplication_emi b on a.application_no=b.application_no join tblapplication_emi_details c on b.emi_id=c.emi_id where a.application_no={Applicationno} and a.paymentapproved=true and a.is_pay=false order by payment_id;");
                        }
                    }

                    var __Term = Convert.ToString(dataTable.Rows[0]["term_name"]).Split(' ').ToList();
                    paymentdetail.termtype = Convert.ToString(dataTable.Rows[0]["termtypename"]);

                    paymentdetail.outstanding = Convert.ToDecimal(dataTable.Rows[0]["totalbalanceamount"]);
                    paymentdetail.principal = Convert.ToString(dataTable.Rows[0]["approved_loan_amount"]);
                    paymentdetail.amortization = Convert.ToString((Convert.ToDecimal(dataTable.Rows[0]["approved_loan_amount"]) + ((Convert.ToDecimal(dataTable.Rows[0]["approved_loan_amount"]) * Convert.ToDecimal(dataTable.Rows[0]["interest_rate"])) / 100)) / Convert.ToDecimal(__Term.FirstOrDefault()));
                    DataTable dataTable1;
                    dataTable1 = DbHelper.SelectMethod(string.Format(QueryHelper.SearchUserIdByApplicationNo, Applicationno));
                    if (dataTable1 != null && dataTable1.Rows.Count > 0)
                    {
                        int userid;
                        userid = DBNull.Value.Equals(dataTable1.Rows[0]["user_id"]) ? 0 : Convert.ToInt32(dataTable1.Rows[0]["user_id"]);
                        var query = DbHelper.SelectMethod(string.Format(QueryHelper.SearchApplicationNoByUserId, userid));
                        if (query != null && query.Rows.Count > 0)
                        {
                            paymentdetail.ReloanCount = query.Rows.Count;
                        }
                    }
                    double PercentPenalty = (Convert.ToDouble(Convert.ToString(dataTable.Rows[0]["late_rate"])) / 100);
                    double penalty = Convert.ToDouble(Convert.ToString(dataTable.Rows[0]["late_penalty"]));
                    #region New Code
                    if (_PaymentDetails == null || _PaymentDetails.Rows.Count == 0)
                    {
                        if (_balance > 0 && paymentdetail.pastduedays > 0)
                        {
                            if (paymentdetail.termtype == "Weekly")
                            {
                                double _Mode = 0;
                                if (paymentdetail.pastduedays != 0 && paymentdetail.pastduedays < 7)
                                {
                                    _Mode = 1;
                                }
                                else if (paymentdetail.pastduedays == 0)
                                {
                                    _Mode = 0;
                                }
                                else
                                {
                                    _Mode = (paymentdetail.pastduedays % 7);
                                }
                                Double _PenalityCount = Convert.ToDouble(paymentdetail.pastduedays / 7);
                                List<string> s = Convert.ToString(_PenalityCount).Split('.').ToList();
                                _PenalityCount = Convert.ToDouble(s.First());
                                int _TotalPenalityCount = _Mode > 0 ? Convert.ToInt32(_PenalityCount + 1) : Convert.ToInt32(_PenalityCount);
                                paymentdetail.penalty = Math.Round((_TotalPenalityCount * penalty), 2);
                                paymentdetail._LateFee = Convert.ToString(paymentdetail.penalty);
                                double latefee = ((Convert.ToDouble(_balance) * (PercentPenalty)) * (paymentdetail.pastduedays));
                                paymentdetail._Penality = Convert.ToString(latefee);
                            }
                            if (paymentdetail.termtype == "Bi-Weekly")
                            {
                                double _Mode = 0;
                                if (paymentdetail.pastduedays != 0 && paymentdetail.pastduedays < 14)
                                {
                                    _Mode = 1;
                                }
                                else if (paymentdetail.pastduedays == 0)
                                {
                                    _Mode = 0;
                                }
                                else
                                {
                                    _Mode = (paymentdetail.pastduedays % 14);
                                }
                                Double _PenalityCount = Convert.ToDouble(paymentdetail.pastduedays / 14);
                                List<string> s = Convert.ToString(_PenalityCount).Split('.').ToList();
                                _PenalityCount = Convert.ToDouble(s.First());
                                int _TotalPenalityCount = _Mode > 0 ? Convert.ToInt32(_PenalityCount + 1) : Convert.ToInt32(_PenalityCount);
                                paymentdetail.penalty = Math.Round((_TotalPenalityCount * penalty), 2);
                                paymentdetail._LateFee = Convert.ToString(paymentdetail.penalty);
                                double latefee = ((Convert.ToDouble(_balance) * (PercentPenalty)) * (paymentdetail.pastduedays));
                                paymentdetail._Penality = Convert.ToString(latefee);
                            }
                            if (paymentdetail.termtype == "Monthly")
                            {
                                double _Mode = 0;

                                if (paymentdetail.pastduedays != 0 && paymentdetail.pastduedays < 28)
                                {
                                    _Mode = 1;
                                }
                                else if (paymentdetail.pastduedays == 0)
                                {
                                    _Mode = 0;
                                }
                                else
                                {
                                    _Mode = (paymentdetail.pastduedays % 28);
                                }
                                Double _PenalityCount = Convert.ToDouble(paymentdetail.pastduedays / 28);
                                List<string> s = Convert.ToString(_PenalityCount).Split('.').ToList();
                                _PenalityCount = Convert.ToDouble(s.First());
                                int _TotalPenalityCount = _Mode > 0 ? Convert.ToInt32(_PenalityCount + 1) : Convert.ToInt32(_PenalityCount);
                                paymentdetail.penalty = Math.Round((_TotalPenalityCount * penalty), 2);
                                paymentdetail._LateFee = Convert.ToString(paymentdetail.penalty);
                                double latefee = ((Convert.ToDouble(_balance) * (PercentPenalty)) * (paymentdetail.pastduedays));
                                paymentdetail._Penality = Convert.ToString(latefee);
                            }
                        }
                        paymentdetail.LateFee = Convert.ToDouble(paymentdetail._Penality);
                        paymentdetail._LateFee = Convert.ToString(paymentdetail.penalty);

                        paymentdetail.penalty = paymentdetail.penalty > 0 ? paymentdetail.penalty : 0;
                        paymentdetail._Penality = Convert.ToString(paymentdetail.penalty);// paymentdetail.penalty;
                        paymentdetail._DueAmount = Convert.ToString(DueAmount);
                        paymentdetail.PayableAmount = Convert.ToString(Convert.ToDouble(DueAmount) + Convert.ToDouble(paymentdetail._Penality) + paymentdetail.LateFee);
                    }
                    else
                    {
                        var StartDate = ptpdate;
                        double __TotalPenality = 0;
                        double __PenaltyAmount = 0;
                        double __Penality = 0;
                        double __lateFee = 0;

                        double __balance = Convert.ToDouble(_PaymentDetails.Rows[0]["emi_amount"]);
                        DateTime LastEMI = DateTime.Now;
                        foreach (DataRow item in _PaymentDetails.Rows)
                        {
                            _dayCount = (Convert.ToDateTime(item["paid_on"]).Date - Convert.ToDateTime(StartDate)).TotalDays;
                            if (__balance > 0 && paymentdetail.pastduedays > 0)
                            {
                                if (paymentdetail.termtype == "Weekly")
                                {
                                    __Penality = ((Convert.ToDouble(__balance) * (PercentPenalty)) * (_dayCount));
                                    double _Mode = 0;
                                    if (_dayCount != 0 && _dayCount < 7)
                                    {
                                        _Mode = 1;
                                    }
                                    else if (_dayCount == 0)
                                    {
                                        _Mode = 0;
                                    }
                                    else
                                    {
                                        _Mode = (_dayCount % 7);
                                    }
                                    Double _PenalityCount = Convert.ToDouble(_dayCount / 7);
                                    List<string> s = Convert.ToString(_PenalityCount).Split('.').ToList();
                                    _PenalityCount = Convert.ToDouble(s.First());
                                    int _TotalPenalityCount = _Mode > 0 ? Convert.ToInt32(_PenalityCount + 1) : Convert.ToInt32(_PenalityCount);
                                    __lateFee += Math.Round((_TotalPenalityCount * penalty), 2);
                                }
                                if (paymentdetail.termtype == "Bi-Weekly")
                                {
                                    __Penality = ((Convert.ToDouble(__balance) * (PercentPenalty)) * (_dayCount));
                                    double _Mode = 0;
                                    if (_dayCount != 0 && _dayCount < 14)
                                    {
                                        _Mode = 1;
                                    }
                                    else if (_dayCount == 0)
                                    {
                                        _Mode = 0;
                                    }
                                    else
                                    {
                                        _Mode = (_dayCount % 14);
                                    }
                                    Double _PenalityCount = Convert.ToDouble(_dayCount / 17);
                                    List<string> s = Convert.ToString(_PenalityCount).Split('.').ToList();
                                    _PenalityCount = Convert.ToDouble(s.First());
                                    int _TotalPenalityCount = _Mode > 0 ? Convert.ToInt32(_PenalityCount + 1) : Convert.ToInt32(_PenalityCount);
                                    __lateFee += Math.Round((_TotalPenalityCount * penalty), 2);
                                }
                                if (paymentdetail.termtype == "Monthly")
                                {
                                    __Penality = ((Convert.ToDouble(__balance) * (PercentPenalty)) * (_dayCount));
                                    double _Mode = 0;
                                    if (_dayCount != 0 && _dayCount < 28)
                                    {
                                        _Mode = 1;
                                    }
                                    else if (_dayCount == 0)
                                    {
                                        _Mode = 0;
                                    }
                                    else
                                    {
                                        _Mode = (_dayCount % 28);
                                    }
                                    Double _PenalityCount = Convert.ToDouble(_dayCount / 28);
                                    List<string> s = Convert.ToString(_PenalityCount).Split('.').ToList();
                                    _PenalityCount = Convert.ToDouble(s.First());
                                    int _TotalPenalityCount = _Mode > 0 ? Convert.ToInt32(_PenalityCount + 1) : Convert.ToInt32(_PenalityCount);
                                    __lateFee += Math.Round((_TotalPenalityCount * penalty), 2);
                                }
                            }
                            __balance -= Convert.ToDouble(item["paid_amount"]);
                            __PenaltyAmount += Convert.ToDouble(item["penalty_amount"]);
                            __TotalPenality += (__Penality + __lateFee);
                            LastEMI = Convert.ToDateTime(item["paid_on"]);
                        }
                        _dayCount = (date - Convert.ToDateTime(LastEMI).Date).TotalDays;
                        if (_balance > 0 && paymentdetail.pastduedays > 0)
                        {
                            if (paymentdetail.termtype == "Weekly")
                            {
                                double _Mode = 0;
                                if (paymentdetail.pastduedays != 0 && paymentdetail.pastduedays < 7)
                                {
                                    _Mode = 1;
                                }
                                else if (paymentdetail.pastduedays == 0)
                                {
                                    _Mode = 0;
                                }
                                else
                                {
                                    _Mode = (paymentdetail.pastduedays % 7);
                                }
                                Double _PenalityCount = Convert.ToDouble(paymentdetail.pastduedays / 7);
                                List<string> s = Convert.ToString(_PenalityCount).Split('.').ToList();
                                _PenalityCount = Convert.ToDouble(s.First());
                                int _TotalPenalityCount = _Mode > 0 ? Convert.ToInt32(_PenalityCount + 1) : Convert.ToInt32(_PenalityCount);
                                paymentdetail.penalty = Math.Round((_TotalPenalityCount * penalty), 2);
                                paymentdetail._LateFee = Convert.ToString(paymentdetail.penalty);
                                double latefee = ((Convert.ToDouble(_balance) * (PercentPenalty)) * (_dayCount));
                                paymentdetail._Penality = Math.Round(latefee, 2).ToString();
                            }
                            if (paymentdetail.termtype == "Bi-Weekly")
                            {
                                double _Mode = 0;
                                if (paymentdetail.pastduedays != 0 && paymentdetail.pastduedays < 14)
                                {
                                    _Mode = 1;
                                }
                                else if (paymentdetail.pastduedays == 0)
                                {
                                    _Mode = 0;
                                }
                                else
                                {
                                    _Mode = (paymentdetail.pastduedays % 14);
                                }
                                Double _PenalityCount = Convert.ToDouble(paymentdetail.pastduedays / 14);
                                List<string> s = Convert.ToString(_PenalityCount).Split('.').ToList();
                                _PenalityCount = Convert.ToDouble(s.First());
                                int _TotalPenalityCount = _Mode > 0 ? Convert.ToInt32(_PenalityCount + 1) : Convert.ToInt32(_PenalityCount);
                                paymentdetail.penalty = Math.Round((_TotalPenalityCount * penalty), 2);
                                paymentdetail._LateFee = Convert.ToString(paymentdetail.penalty);
                                double latefee = ((Convert.ToDouble(_balance) * (PercentPenalty)) * (_dayCount));
                                paymentdetail._Penality = Math.Round(latefee, 2).ToString();
                            }
                            if (paymentdetail.termtype == "Monthly")
                            {
                                double _Mode = 0;
                                if (paymentdetail.pastduedays != 0 && paymentdetail.pastduedays < 28)
                                {
                                    _Mode = 1;
                                }
                                else if (paymentdetail.pastduedays == 0)
                                {
                                    _Mode = 0;
                                }
                                else
                                {
                                    _Mode = (paymentdetail.pastduedays % 28);
                                }
                                Double _PenalityCount = Convert.ToDouble(paymentdetail.pastduedays / 28);
                                List<string> s = Convert.ToString(_PenalityCount).Split('.').ToList();
                                _PenalityCount = Convert.ToDouble(s.First());
                                int _TotalPenalityCount = _Mode > 0 ? Convert.ToInt32(_PenalityCount + 1) : Convert.ToInt32(_PenalityCount);
                                paymentdetail.penalty = Math.Round((_TotalPenalityCount * penalty), 2);
                                paymentdetail._LateFee = Convert.ToString(paymentdetail.penalty);
                                double latefee = ((Convert.ToDouble(_balance) * (PercentPenalty)) * (_dayCount));
                                paymentdetail._Penality = Math.Round(latefee, 2).ToString();
                            }
                        }

                        var _totalDyasforPenalty = (date - Convert.ToDateTime(StartDate).Date).TotalDays;
                        double _TotalPenalityfromLastEMIDate = 0;

                        if (paymentdetail.termtype == "Weekly")
                        {
                            double _Mode = 0;
                            if (_totalDyasforPenalty != 0 && _totalDyasforPenalty < 7)
                            {
                                _Mode = 1;
                            }
                            else if (_totalDyasforPenalty == 0)
                            {
                                _Mode = 0;
                            }
                            else
                            {
                                _Mode = (_totalDyasforPenalty % 7);
                            }
                            Double _PenalityCount = Convert.ToDouble(_totalDyasforPenalty / 7);
                            List<string> s = Convert.ToString(_PenalityCount).Split('.').ToList();
                            _PenalityCount = Convert.ToDouble(s.First());
                            int _TotalPenalityCount = _Mode > 0 ? Convert.ToInt32(_PenalityCount + 1) : Convert.ToInt32(_PenalityCount);
                            _TotalPenalityfromLastEMIDate = Math.Round((_TotalPenalityCount * penalty), 2);
                        }
                        if (paymentdetail.termtype == "Bi-Weekly")
                        {
                            double _Mode = 0;
                            if (_totalDyasforPenalty != 0 && _totalDyasforPenalty < 14)
                            {
                                _Mode = 1;
                            }
                            else if (_totalDyasforPenalty == 0)
                            {
                                _Mode = 0;
                            }
                            else
                            {
                                _Mode = (_totalDyasforPenalty % 14);
                            }
                            Double _PenalityCount = Convert.ToDouble(paymentdetail.pastduedays / 14);
                            List<string> s = Convert.ToString(_PenalityCount).Split('.').ToList();
                            _PenalityCount = Convert.ToDouble(s.First());
                            int _TotalPenalityCount = _Mode > 0 ? Convert.ToInt32(_PenalityCount + 1) : Convert.ToInt32(_PenalityCount);
                            _TotalPenalityfromLastEMIDate = Math.Round((_TotalPenalityCount * penalty), 2);
                        }
                        if (paymentdetail.termtype == "Monthly")
                        {
                            double _Mode = 0;
                            if (_totalDyasforPenalty != 0 && _totalDyasforPenalty < 28)
                            {
                                _Mode = 1;
                            }
                            else if (_totalDyasforPenalty == 0)
                            {
                                _Mode = 0;
                            }
                            else
                            {
                                _Mode = (_totalDyasforPenalty % 28);
                            }
                            Double _PenalityCount = Convert.ToDouble(paymentdetail.pastduedays / 28);
                            List<string> s = Convert.ToString(_PenalityCount).Split('.').ToList();
                            _PenalityCount = Convert.ToDouble(s.First());
                            int _TotalPenalityCount = _Mode > 0 ? Convert.ToInt32(_PenalityCount + 1) : Convert.ToInt32(_PenalityCount);
                            _TotalPenalityfromLastEMIDate = Math.Round((_TotalPenalityCount * penalty), 2);
                        }

                        paymentdetail.LateFee = Convert.ToDouble(paymentdetail._Penality);
                        paymentdetail._LateFee = Convert.ToString(paymentdetail.penalty);

                        paymentdetail.penalty = paymentdetail.penalty > 0 ? paymentdetail.penalty : 0;
                        paymentdetail._Penality = Convert.ToString(Math.Round(_TotalPenalityfromLastEMIDate, 2) - Math.Round(__lateFee, 2));
                        paymentdetail._DueAmount = Convert.ToString(DueAmount);
                        paymentdetail.PayableAmount = Math.Round(Convert.ToDouble(Convert.ToDouble(DueAmount) + Convert.ToDouble(paymentdetail.LateFee) + Convert.ToDouble(paymentdetail._Penality)), 2).ToString();
                    }
                    #endregion
                }
                return paymentdetail;
            }
            catch (Exception ex)
            {

                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                throw ex;
            }
        }

        public void Insertloandetail(PaymentInformation detail)
        {
            try
            {
                var joiningdate = DateTime.Now.Month + "-" + DateTime.Now.Day + "-" + DateTime.Now.Year;

                if (detail.txtdatepaid != "" && detail.txtdatepaid != null)
                {
                    var arr2 = detail.txtdatepaid.Split('-');
                    detail.txtdatepaid = arr2[1] + '-' + arr2[0] + '-' + arr2[2];
                }
                if (String.IsNullOrWhiteSpace(detail.txtdatepaid))
                {
                    detail.txtdatepaid = joiningdate;
                }

                if (detail.promisepay != "" && detail.promisepay != null)
                {
                    var arr2 = detail.promisepay.Split('-');
                    detail.promisepay = arr2[1] + '-' + arr2[0] + '-' + arr2[2];
                }
                if (String.IsNullOrWhiteSpace(detail.promisepay))
                {
                    detail.promisepay = joiningdate;
                }
                if (String.IsNullOrWhiteSpace(detail.txtamountpaid))
                {
                    detail.txtamountpaid = "0.00";
                }
                string insertEmipayamount = (string.Format(QueryHelper.InsertEmiPayment, detail.applicationno, detail.txtamountpaid, detail.txtdatepaid, detail.updatedby, detail.whatpaymnetchannel, detail.proofofpayment));
                DbHelper.InsertUpdateDelete(insertEmipayamount);

                if (String.IsNullOrWhiteSpace(detail.ptpamount))
                {
                    detail.ptpamount = "0.00";
                }

                string insertptpamount = (string.Format(QueryHelper.Insertptpamount, detail.applicationno, detail.txtdatepaid, detail.txtamountpaid, detail.whatpaymnetchannel, detail.proofofpayment, detail.txtbesttimetocallback, detail.promisepay, detail.ptpamount, detail.nonpaymentreson, detail.txtotherremarks, detail.updatedby));
                DbHelper.InsertUpdateDelete(insertptpamount);
            }
            catch (Exception ex)
            {

                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                throw ex;
            }
        }

        //Account Area

        public List<ProfileSummery> GetPortfolioList()
        {
            List<ProfileSummery> listdetail = new List<ProfileSummery>();
            try
            {
                var query = DbHelper.SelectMethod(QueryHelper.GetPortfolioList);
                for (int i = 0; i < query.Rows.Count; i++)
                {
                    ProfileSummery summeryList = new ProfileSummery
                    {
                        ClientName = Convert.ToString(query.Rows[i]["first_name"]) + " " + Convert.ToString(query.Rows[i]["middle_name"]) + " " + Convert.ToString(query.Rows[i]["last_name"]),
                        applicationno = Convert.ToInt32(query.Rows[i]["applicationno"]),
                        Contactno = Convert.ToString(query.Rows[i]["contract_ref_no"]),
                        Principleloan = Convert.ToString(query.Rows[i]["approved_total_amount_due"]),
                        weeklyterm = Convert.ToString(query.Rows[i]["term"]),
                        DateApplied = Convert.ToString(query.Rows[i]["dateapplied"]),
                        PersonalEmail = Convert.ToString(query.Rows[i]["personalemail"]),
                        PersonalContactNo1 = Convert.ToString(query.Rows[i]["personalcontactno"]),
                        PersonalContactNo2 = Convert.ToString(query.Rows[i]["relativecontactno"]),
                        PersonalContactNo3 = Convert.ToString(query.Rows[i]["coworkercontactno"]),
                        Address = Convert.ToString(query.Rows[i]["address"]),
                        HomePhone = Convert.ToString(query.Rows[i]["homephoneno"]),
                        PlaceOfBirth = Convert.ToString(query.Rows[i]["placeofbirth"]),
                        DOB = Convert.ToString(query.Rows[i]["dateofbirth"]),
                        CivilStatus = Convert.ToString(query.Rows[i]["civilstatus"]),
                        MotherMaidenName = Convert.ToString(query.Rows[i]["mothermaidenname"]),
                        MotherAdd = Convert.ToString(query.Rows[i]["motheraddress"]),
                        CompanyName = Convert.ToString(query.Rows[i]["companyname"]),
                        CompanyAdd = Convert.ToString(query.Rows[i]["companyaddress"]),
                        Position = Convert.ToString(query.Rows[i]["designation"]),
                        GrossIncome = Convert.ToDecimal(query.Rows[i]["gross_income"]),
                        RefName = Convert.ToString(query.Rows[i]["reference_name"]),
                        termtype = Convert.ToString(query.Rows[i]["termtype"]),
                        term_value = Convert.ToInt32(query.Rows[i]["term_value"]),
                        PenaltyAmt1 = Convert.ToString(query.Rows[i]["penality_amount1"]) != "" ? Convert.ToDecimal(query.Rows[i]["penality_amount1"].ToString()) : 0,
                        PenaltyAmt2 = Convert.ToString(query.Rows[i]["penality_amount2"]) != "" ? Convert.ToDecimal(query.Rows[i]["penality_amount2"].ToString()) : 0,
                        PenaltyAmt3 = Convert.ToString(query.Rows[i]["penality_amount3"]) != "" ? Convert.ToDecimal(query.Rows[i]["penality_amount3"].ToString()) : 0,
                        PenaltyAmt4 = Convert.ToString(query.Rows[i]["penality_amount4"]) != "" ? Convert.ToDecimal(query.Rows[i]["penality_amount4"].ToString()) : 0,
                        PenaltyAmt5 = Convert.ToString(query.Rows[i]["penality_amount5"]) != "" ? Convert.ToDecimal(query.Rows[i]["penality_amount5"].ToString()) : 0,
                        PTP = Convert.ToString(query.Rows[i]["current_date_ptp"]),
                        BestTimeToCall = Convert.ToString(query.Rows[i]["best_time_callback"]),
                        NonPaymentReason = Convert.ToString(query.Rows[i]["non_payment_reason"]),
                        OtherRemarks = Convert.ToString(query.Rows[i]["other_remarks"]),
                        Notes = "Notes",
                        PhoneStatus = "Status",
                        NoOfTimesDialed = "Default"
                    };
                    if (summeryList.Principleloan == "")
                    {
                        summeryList.Principleloan = "0.00";
                    }
                    summeryList.term = Convert.ToString(query.Rows[i]["approved_term"]);
                    if (summeryList.term == "")
                    {
                        summeryList.term = "00";
                    }
                    summeryList.interestrate = Convert.ToString(query.Rows[i]["approved_interest_rate"]);
                    if (summeryList.interestrate == "")
                    {
                        summeryList.interestrate = "0.00";
                    }
                    summeryList.MaturityDate = Convert.ToString(query.Rows[i]["approved_maturity_date"]);
                    summeryList.Referenceno = Convert.ToString(query.Rows[i]["reference_no"]);

                    summeryList.Dateofreleased = Convert.ToString(query.Rows[i]["disbursement_date"]);
                    string totalamount = Convert.ToString(query.Rows[i]["approved_total_amount_due"]);
                    string term = Convert.ToString(query.Rows[i]["approved_term"]);
                    string rate = Convert.ToString(query.Rows[i]["approved_interest_rate"]);
                    if (totalamount != "" && term != "" && rate != "" && totalamount != null && term != null && rate != null)
                    {
                        double intrate = float.Parse(rate);
                        double intterm = float.Parse(term);
                        double inttotalamount = float.Parse(totalamount);
                        double interestvalue = inttotalamount * intrate;
                        double week = intterm / 7;
                        double osbalance = (inttotalamount + interestvalue);
                        var outstandingbalance = string.Format("{0:0.00}", osbalance);
                        summeryList.Osbalance = outstandingbalance;
                        double installmentamount = (inttotalamount + interestvalue) / week;
                        var instamentsamount = string.Format("{0:0.00}", installmentamount);
                        summeryList.Installmentpayment = instamentsamount;
                        listdetail.Add(summeryList);
                    }
                    else
                    {
                        summeryList.Osbalance = "0.00";
                        summeryList.Installmentpayment = "0.00";
                        listdetail.Add(summeryList);
                    }
                    decimal _balance = 0;
                    decimal _totalpaid = 0;
                    var query2 = DbHelper.SelectMethod(string.Format(QueryHelper.GetEmiDetail, summeryList.applicationno));
                    summeryList.weekRecords = new List<TermsWeekRecord>();
                    if (query2 != null && query2.Rows.Count > 0)
                    {
                        for (int j = 0; j < query2.Rows.Count; j++)
                        {
                            var record = new TermsWeekRecord
                            {
                                date = Convert.ToString(query2.Rows[j]["emi_date"]),
                                principle = Convert.ToString(query2.Rows[j]["emi_principal"]),
                                emiamount = Convert.ToString(query2.Rows[j]["emi_amount"]),
                                rate = Convert.ToString(query2.Rows[j]["emi_rate"])
                            };
                            _balance += Convert.ToDecimal(query2.Rows[j]["balanceamount"]);
                            _totalpaid += Convert.ToDecimal(query2.Rows[j]["paidamount"]);
                            summeryList.weekRecords.Add(record);
                        }
                    }
                    summeryList.Balance = _balance;
                    summeryList.AmountPaid = _totalpaid;
                }
                return listdetail;
            }
            catch (Exception ex)
            {

                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                throw ex;
            }
        }


        /// <summary>
        /// get GetMI Not Created data on behalf of date
        /// </summary>
        /// <param name="FromDate"></param>
        /// <param name="ToDate"></param>
        /// <param name="appNo"></param>
        /// <param name="contractNo"></param>
        /// <param name="clientName"></param>
        /// <returns></returns>
        public List<ProfileSummery> GetEmiNotCreated(String FromDate, String ToDate)
        {
            List<ProfileSummery> listdetail = new List<ProfileSummery>();
            try
            {
                var arr_FromDate = FromDate.Split('-');
                FromDate = arr_FromDate[2] + '-' + arr_FromDate[1] + "-" + arr_FromDate[0];
                var arr_ToDate = ToDate.Split('-');
                ToDate = arr_ToDate[2] + '-' + arr_ToDate[1] + "-" + arr_ToDate[0];

                var query = DbHelper.SelectMethod(String.Format(QueryHelper.GetEmiNotCreatedNew, FromDate, ToDate));
                if (query != null && query.Rows.Count > 0)
                {
                    for (int i = 0; i < query.Rows.Count; i++)
                    {
                        ProfileSummery summeryList = new ProfileSummery();
                        var firstName = query.Rows[i]["first_name"].ToString() != string.Empty ? query.Rows[i]["first_name"].ToString() : string.Empty;
                        var middleName = query.Rows[i]["middle_name"].ToString() != string.Empty ? query.Rows[i]["middle_name"].ToString() : string.Empty;
                        var lastName = query.Rows[i]["last_name"].ToString() != string.Empty ? query.Rows[i]["last_name"].ToString() : string.Empty;
                        summeryList.EmiStatus = Convert.ToString(query.Rows[i]["emi_status"]) != string.Empty ? Convert.ToInt32(query.Rows[i]["emi_status"]) : 0;
                        summeryList.ClientName = firstName + " " + middleName + " " + lastName;
                        summeryList.applicationno = Convert.ToInt32(query.Rows[i]["applicationno"]);
                        summeryList.Contactno = Convert.ToString(query.Rows[i]["contract_ref_no"]);
                        summeryList.Principleloan = Convert.ToString(query.Rows[i]["approved_loan_amount"]);
                        summeryList.IsPickedForAccount = Convert.ToBoolean(query.Rows[i]["ispickedforaccount"]);
                        if (summeryList.Principleloan == "")
                        {
                            summeryList.Principleloan = "0.00";
                        }
                        summeryList.term = Convert.ToString(query.Rows[i]["term_name"]);
                        if (summeryList.term == "")
                        {
                            summeryList.term = "00";
                        }
                        summeryList.interestrate = Convert.ToString(query.Rows[i]["approved_interest_rate"]);
                        if (summeryList.interestrate == "")
                        {
                            summeryList.interestrate = "0.00";
                        }
                        summeryList.MaturityDate = Convert.ToString(query.Rows[i]["approved_maturity_date"]);

                        summeryList.Dateofreleased = Convert.ToString(query.Rows[i]["disbursement_date"]) != string.Empty ? Convert.ToString(query.Rows[i]["disbursement_date"]) : string.Empty;
                        string totalamount = Convert.ToString(query.Rows[i]["approved_loan_amount"]);
                        string term = Convert.ToString(query.Rows[i]["approved_term"]);
                        string rate = Convert.ToString(query.Rows[i]["approved_interest_rate"]);
                        if (totalamount != "" && term != "" && rate != "")
                        {
                            double intrate = Convert.ToDouble(rate);
                            double inttotalamount = Convert.ToDouble(totalamount);
                            double interestvalue = (inttotalamount * intrate) / 100;

                            //Calculate Total Outstanding Amount
                            double osbalance = (inttotalamount + interestvalue);
                            var outstandingbalance = string.Format("{0:0.00}", osbalance);
                            summeryList.Osbalance = outstandingbalance;

                            var _termtext = summeryList.term.Split(' ').ToList().FirstOrDefault();
                            double intterm = float.Parse(_termtext);

                            //Calculate Installation Amount
                            double installmentamount = osbalance / intterm;
                            var instamentsamount = string.Format("{0:0.00}", installmentamount);
                            summeryList.Installmentpayment = instamentsamount;

                            // Add details in List
                            listdetail.Add(summeryList);
                        }
                        else
                        {
                            summeryList.Osbalance = "0.00";
                            summeryList.Installmentpayment = "0.00";
                            listdetail.Add(summeryList);
                        }
                    }
                }
                return listdetail;
            }
            catch (Exception ex)
            {

                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                throw ex;
            }
        }

        private string GetTermType(string termType)
        {
            try
            {
                string str = string.Empty;
                DataTable dt = DbHelper.SelectMethod(string.Format(QueryHelper.GetTermTypeId, termType));
                if (dt != null && dt.Rows.Count > 0)
                {
                    str = Convert.ToString(dt.Rows[0]["termtype"]);
                }
                return str;
            }
            catch (Exception ex)
            {
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                throw ex;
            }
        }


        public void InsertEmiinfo(EmiInformation emilist, double loanamount)
        {
            try
            {
                var date = DateTime.Now.ToString("dd-MM-yyyy");
                var joiningdate = DateTime.Now.Month + "-" + DateTime.Now.Day + "-" + DateTime.Now.Year;

                if (emilist.balanceamt == null)
                {
                    emilist.balanceamt = "0.00";
                }
                if (emilist.totalamountpaid == null)
                {
                    emilist.totalamountpaid = "0.00";
                }
                if (emilist.panalityamountfive == null)
                {
                    emilist.panalityamountfive = "0.00";
                }
                if (emilist.panalityamountfour == null)
                {
                    emilist.panalityamountfour = "0.00";
                }
                if (emilist.panalityamountthird == null)
                {
                    emilist.panalityamountthird = "0.00";
                }
                if (emilist.panalityamountsecound == null)
                {
                    emilist.panalityamountsecound = "0.00";
                }
                if (emilist.panalityamountfirst == null)
                {
                    emilist.panalityamountfirst = "0.00";
                }
                if (emilist.thirdweekfirstprinciple == null)
                {
                    emilist.thirdweekfirstprinciple = "0.00";
                }
                if (emilist.thirdweeksecoundprinciple == null)
                {
                    emilist.thirdweeksecoundprinciple = "0.00";
                }
                if (emilist.thirdweekthirdprinciple == null)
                {
                    emilist.thirdweekthirdprinciple = "0.00";
                }
                if (emilist.thirdweekfirstrate == null)
                {
                    emilist.thirdweekfirstrate = "0.00";
                }
                if (emilist.thirdweeksecoundrate == null)
                {
                    emilist.thirdweeksecoundrate = "0.00";
                }
                if (emilist.thirdweekthirdrate == null)
                {
                    emilist.thirdweekthirdrate = "0.00";
                }
                if (emilist.fourthweekprinciple == null)
                {
                    emilist.fourthweekprinciple = "0.00";
                }
                if (emilist.fourthweekrate == null)
                {
                    emilist.fourthweekrate = "0.00";
                }
                if (emilist.fiveweekprinciple == null)
                {
                    emilist.fiveweekprinciple = "0.00";
                }
                if (emilist.fiveweekrate == null)
                {
                    emilist.fiveweekrate = "0.00";
                }
                if (emilist.bifirstweekprinciple == null)
                {
                    emilist.bifirstweekprinciple = "0.00";
                }
                if (emilist.bisecoundweekprinciple == null)
                {
                    emilist.bisecoundweekprinciple = "0.00";
                }
                if (emilist.bifirstweekrate == null)
                {
                    emilist.bifirstweekrate = "0.00";
                }
                if (emilist.bisecoundweekprinciple == null)
                {
                    emilist.bisecoundweekprinciple = "0.00";
                }
                if (emilist.bisecoundweekrate == null)
                {
                    emilist.bisecoundweekrate = "0.00";
                }
                if (emilist.monthlyprinciple == null)
                {
                    emilist.monthlyprinciple = "0.00";
                }
                if (emilist.monthlyrate == null)
                {
                    emilist.monthlyrate = "0.00";
                }
                if (emilist.emiamount == null)
                {
                    emilist.emiamount = "0.00";
                }
                if (emilist.thirdweekfirstemiamount == null)
                {
                    emilist.thirdweekfirstemiamount = "0.00";
                }
                if (emilist.thirdweeksecoundemiamount == null)
                {
                    emilist.thirdweeksecoundemiamount = "0.00";
                }
                if (emilist.thirdweekthirdemiamount == null)
                {
                    emilist.thirdweekthirdemiamount = "0.00";
                }
                if (emilist.fourthweekemiamount == null)
                {
                    emilist.fourthweekemiamount = "0.00";
                }
                if (emilist.fiveweekemiamount == null)
                {
                    emilist.fiveweekemiamount = "0.00";
                }
                if (emilist.bifirstweekemiamount == null)
                {
                    emilist.bifirstweekemiamount = "0.00";
                }
                if (emilist.bisecoundweekemiamount == null)
                {
                    emilist.bisecoundweekemiamount = "0.00";
                }
                if (emilist.monthlyemiamount == null)
                {
                    emilist.monthlyemiamount = "0.00";
                }
                if (emilist.thirdweekfirstdate == null)
                {
                    emilist.thirdweekfirstdate = date;
                }
                if (emilist.thirdweeksecounddate == null)
                {
                    emilist.thirdweeksecounddate = date;
                }
                if (emilist.thirdweekthirddate == null)
                {
                    emilist.thirdweekthirddate = date;
                }
                if (emilist.fourthweekdate == null)
                {
                    emilist.fourthweekdate = date;
                }
                if (emilist.fiveweekdate == null)
                {
                    emilist.fiveweekdate = date;
                }
                if (emilist.bifirstweekdate == null)
                {
                    emilist.bifirstweekdate = date;
                }
                if (emilist.bisecoundweekdate == null)
                {
                    emilist.bisecoundweekdate = date;
                }
                if (emilist.monthlydate == null)
                {
                    emilist.monthlydate = date;
                }
                if (emilist.thirdweekfirstdate != "")
                {
                    var arr2 = emilist.thirdweekfirstdate.Split('-');
                    emilist.thirdweekfirstdate = arr2[2] + '-' + arr2[1] + '-' + arr2[0];
                }
                if (emilist.thirdweeksecounddate != "")
                {
                    var arr2 = emilist.thirdweeksecounddate.Split('-');
                    emilist.thirdweeksecounddate = arr2[2] + '-' + arr2[1] + '-' + arr2[0];
                }
                if (emilist.thirdweekthirddate != "")
                {
                    var arr2 = emilist.thirdweekthirddate.Split('-');
                    emilist.thirdweekthirddate = arr2[2] + '-' + arr2[1] + '-' + arr2[0];
                }
                if (emilist.fourthweekdate != "")
                {
                    var arr2 = emilist.fourthweekdate.Split('-');
                    emilist.fourthweekdate = arr2[2] + '-' + arr2[1] + '-' + arr2[0];
                }
                if (emilist.fiveweekdate != "")
                {
                    var arr2 = emilist.fiveweekdate.Split('-');
                    emilist.fiveweekdate = arr2[2] + '-' + arr2[1] + '-' + arr2[0];
                }
                if (emilist.bifirstweekdate != "")
                {
                    var arr2 = emilist.bifirstweekdate.Split('-');
                    emilist.bifirstweekdate = arr2[2] + '-' + arr2[1] + '-' + arr2[0];
                }
                if (emilist.bisecoundweekdate != "")
                {
                    var arr2 = emilist.bisecoundweekdate.Split('-');
                    emilist.bisecoundweekdate = arr2[2] + '-' + arr2[1] + '-' + arr2[0];
                }
                if (emilist.monthlydate != "")
                {
                    var arr2 = emilist.monthlydate.Split('-');
                    emilist.monthlydate = arr2[2] + '-' + arr2[1] + '-' + arr2[0];
                }

                PaymentInformation paymentdetail = new PaymentInformation();
                var existquery = DbHelper.SelectMethod(string.Format(QueryHelper.GetEmiCount, emilist.applicationno));
                paymentdetail.existid = Convert.ToString(existquery.Rows[0]["existdetail"]);

                if (Convert.ToInt32(paymentdetail.existid) > 0)
                {
                    if (emilist.disbursement_date != "" && emilist.disbursement_date != null)
                    {
                        var arr2 = emilist.disbursement_date.Split('-');
                        emilist.disbursement_date = arr2[2] + '-' + arr2[1] + '-' + arr2[0];
                    }
                    if (emilist.disbursement_date == null)
                    {
                        emilist.disbursement_date = joiningdate;
                    }
                    string query1 = (string.Format(QueryHelper.UpdatePanalityAmount, emilist.referenceno, emilist.disbursement_date, emilist.panalityamountfirst, emilist.panalityamountsecound, emilist.panalityamountthird, emilist.panalityamountfour, emilist.panalityamountfive, emilist.applicationno));
                    DbHelper.InsertUpdateDelete(query1);
                }
                else
                {
                    if (emilist.disbursement_date != "" && emilist.disbursement_date != null)
                    {
                        var arr2 = emilist.disbursement_date.Split('-');
                        emilist.disbursement_date = arr2[2] + '-' + arr2[1] + '-' + arr2[0];
                    }
                    if (emilist.disbursement_date == null)
                    {
                        emilist.disbursement_date = joiningdate;
                    }
                    string insertpayamountquery = (string.Format(QueryHelper.InsertPanalityAmount, emilist.applicationno, emilist.new_referenceno, emilist.disbursement_date, emilist.panalityamountfirst, emilist.panalityamountsecound, emilist.panalityamountthird, emilist.panalityamountfour, emilist.panalityamountfive));
                    DbHelper.InsertUpdateDelete(insertpayamountquery);
                }
                var query = DbHelper.SelectMethod(string.Format(QueryHelper.GetEmiID, emilist.applicationno));
                if (query != null && query.Rows.Count > 0)
                {
                    paymentdetail.emiid = Convert.ToInt64(query.Rows[0]["emi_id"]);
                }

                var existermdata = DbHelper.SelectMethod(string.Format(QueryHelper.ExistTermData, emilist.applicationno));
                if (existermdata != null && existermdata.Rows.Count > 0)
                {
                    paymentdetail.existtermdata = Convert.ToInt64(existermdata.Rows[0]["termdata"]);
                }

                if (paymentdetail.existtermdata > 0)
                {
                    DbHelper.InsertUpdateDelete(string.Format(QueryHelper.DeleteTermDetail, paymentdetail.emiid));
                }

                var paidquery = DbHelper.SelectMethod(string.Format(QueryHelper.GetPaidAmount, emilist.applicationno));
                if (paidquery != null && paidquery.Rows.Count > 0)
                {
                    emilist.totalamountpaid = Convert.ToString(paidquery.Rows[0]["totalpaid"]);
                }

                foreach (var item in emilist.Records)
                {
                    if (item.fourthweekdate == null)
                    {
                        item.fourthweekdate = date;
                    }
                    if (item.fourthweekemiamount == null)
                    {
                        item.fourthweekemiamount = "0.00";
                    }
                    if (item.fourthweekrate == null)
                    {
                        item.fourthweekrate = "0.00";
                    }
                    if (item.fourthweekprinciple == null)
                    {
                        item.fourthweekprinciple = "0.00";
                    }
                    if (item.fourthweekdate != null)
                    {
                        var arr2 = item.fourthweekdate.Split('-');
                        item.fourthweekdate = arr2[2] + '-' + arr2[1] + '-' + arr2[0];
                    }
                    item.emistatus = 0;
                    string query2 = (string.Format(QueryHelper.InsertEmiDtail, paymentdetail.emiid, item.fourthweekdate, item.fourthweekemiamount, item.fourthweekrate, item.fourthweekprinciple, item.emistatus, emilist.updatedby, item.fourthweekemiamount));
                    DbHelper.InsertUpdateDelete(query2);
                }

                if (Convert.ToInt32(paymentdetail.existid) < 1)
                {
                    logger.AddWelcomeMessage(History.Disbursement, "", "Your scheduled payment are ready for viewing. Go to MY LOAN", loanamount, true, emilist.applicationno, "");
                }
            }
            catch (Exception ex)
            {
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                throw ex;
            }
        }

        public List<PaymentInformation> GetLoanData(int UserId)
        {
            List<PaymentInformation> namesList = new List<PaymentInformation>();
            try
            {
                var query = DbHelper.SelectMethod(String.Format(QueryHelper.NewGetPaymentBucketList, UserId));
                if (query != null && query.Rows.Count > 0)
                {
                    foreach (DataRow item in query.Rows)
                    {
                        if (!namesList.Any(x => x.applicationno == Convert.ToInt32(item["applicationno"])))
                        {
                            PaymentInformation payment = GetCollecterListbyPhoneNumber(Convert.ToInt32(item["applicationno"]));
                            payment.SortingOrder = Convert.ToInt32(item["sortingorder"]);
                            payment.EMIDetailsID = Convert.ToInt32(item["emi_detail_id"]);
                            payment.Priority = Convert.ToString(item["priority"]);
                            payment.Aging = Convert.ToString(item["aging"]) == "+0" ? "0" : Convert.ToString(item["aging"]);
                            namesList.Add(payment);
                        }
                    }
                }
                return namesList;
            }
            catch (Exception ex)
            {
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                throw ex;
            }
        }

        public List<PaymentInformation> GetLoanDataByName(string FirstName)
        {
            List<PaymentInformation> namesList = new List<PaymentInformation>();
            try
            {
                var query = DbHelper.SelectMethod(String.Format(QueryHelper.GetPaymentListByNameLatest, FirstName));
                if (query != null && query.Rows.Count > 0)
                {
                    foreach (DataRow item in query.Rows)
                    {
                        PaymentInformation payment = GetCollecterListbyPhoneNumber(Convert.ToInt32(item["application_no"]));
                        payment.SortingOrder = Convert.ToInt32(item["sortingorder"]);
                        payment.EMIDetailsID = Convert.ToInt32(item["emi_detail_id"]);
                        payment.Priority = Convert.ToString(item["priority"]);
                        payment.Aging = Convert.ToString(item["aging"]) == "+0" ? "0" : Convert.ToString(item["aging"]);
                        namesList.Add(payment);
                    }
                }
                return namesList;
            }
            catch (Exception ex)
            {
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                throw ex;
            }
        }

        public List<PaymentInformation> GetLoanDataById(string ApplicationId)
        {
            List<PaymentInformation> namesList = new List<PaymentInformation>();
            try
            {
                var query = DbHelper.SelectMethod(String.Format(QueryHelper.GetPaymentBucketByIDLatest, ApplicationId));
                if (query != null && query.Rows.Count > 0)
                {
                    foreach (DataRow item in query.Rows)
                    {
                        if (Convert.ToDecimal(item["balanceamount"]) > 0)
                        {
                            PaymentInformation payment = GetCollecterListbyPhoneNumber(Convert.ToInt32(ApplicationId));
                            payment.SortingOrder = Convert.ToInt32(item["sortingorder"]);
                            payment.EMIDetailsID = Convert.ToInt32(item["emi_detail_id"]);
                            payment.Priority = Convert.ToString(item["priority"]);
                            payment.Aging = Convert.ToString(item["aging"]) == "+0" ? "0" : Convert.ToString(item["aging"]);
                            namesList.Add(payment);
                        }
                        
                    }
                }
                return namesList;
            }
            catch (Exception ex)
            {
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                throw ex;
            }
        }

        public List<PaymentInformation> GetLoanDataByReference(string Reference)
        {
            List<PaymentInformation> namesList = new List<PaymentInformation>();
            try
            {
                var query = DbHelper.SelectMethod(String.Format(QueryHelper.GetPaymentListByReferenceLatest, Reference));
                if (query != null && query.Rows.Count > 0)
                {
                    foreach (DataRow item in query.Rows)
                    {
                        PaymentInformation payment = GetCollecterListbyPhoneNumber(Convert.ToInt32(item["application_no"]));
                        payment.SortingOrder = Convert.ToInt32(item["sortingorder"]);
                        payment.EMIDetailsID = Convert.ToInt32(item["emi_detail_id"]);
                        payment.Priority = Convert.ToString(item["priority"]);
                        payment.Aging = Convert.ToString(item["aging"]) == "+0" ? "0" : Convert.ToString(item["aging"]);
                        namesList.Add(payment);
                    }
                }
                return namesList;
            }
            catch (Exception ex)
            {
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                throw ex;
            }
        }

        /// <summary>
        /// send broadcast sms function
        /// </summary>
        /// <param name="UserId"></param>
        /// <returns></returns>
        public List<FSBucket> GetFSBucketData(int UserId)
        {
            List<FSBucket> namesList = new List<FSBucket>();
            try
            {
                var query = DbHelper.SelectMethod(string.Format(QueryHelper.GetFSBucketUpdated, UserId));
                if (query != null && query.Rows.Count > 0)
                {
                    for (int i = 0; i < query.Rows.Count; i++)
                    {
                        FSBucket data = new FSBucket
                        {
                            Application_No = Convert.ToInt32(query.Rows[i]["applicationno"]),
                            Name = Convert.ToString(query.Rows[i]["first_name"]) + " " + Convert.ToString(query.Rows[i]["middle_name"]) + " " + Convert.ToString(query.Rows[i]["last_name"]),
                            FS_Date = Convert.ToDateTime(query.Rows[i]["fs_date"]).ToString("dd/MM/yyyy"),
                            TermId = Convert.ToInt32(query.Rows[i]["term"]),
                            Amount = Convert.ToDouble(query.Rows[i]["approved_loan_amount"]),
                            Records = Convert.ToInt32(query.Rows[i]["records"]),
                            Is_Reduce_Loan = Convert.ToBoolean(query.Rows[i]["is_reduceloan"]),
                            BalanceAmount = Convert.ToDouble(query.Rows[i]["balance"]),
                            LoanTerm = Convert.ToString(query.Rows[i]["termplan"]),
                            PersonalContactNumber = Convert.ToString(query.Rows[i]["personalcontactno"]),
                            personal_email = Convert.ToString(query.Rows[i]["personalemail"]),
                            Reloan = Convert.ToBoolean(query.Rows[i]["isfor_reloan"]),
                            Preterm = Convert.ToBoolean(query.Rows[i]["is_preterm"])
                        };
                        if (data.Reloan == false && data.Preterm == false)
                        {
                            data.RecordName = "N";
                        }
                        if (data.Reloan == true && data.Preterm == false)
                        {
                            int userid = Convert.ToInt32(query.Rows[i]["user_id"]);
                            var query1 = DbHelper.SelectMethod(string.Format(QueryHelper.SearchApplicationNoByUserIdReloan, userid));
                            if (query1 != null && query1.Rows.Count > 0)
                            {
                                int count = query1.Rows.Count;
                                data.Records = count;
                                data.RecordName = "R" + count.ToString();
                            }

                        }
                        if (data.Reloan == false && data.Preterm == true)
                        {
                            int userid = Convert.ToInt32(query.Rows[i]["user_id"]);
                            var query1 = DbHelper.SelectMethod(string.Format(QueryHelper.SearchApplicationNoByUserIdPreterm, userid));
                            if (query1 != null && query1.Rows.Count > 0)
                            {
                                int count = query1.Rows.Count;
                                data.Records = count;
                                data.RecordName = "P" + count.ToString();
                            }

                        }
                        namesList.Add(data);
                    }
                    namesList = namesList.Where(x => x.BalanceAmount == 0).ToList();
                }
                return namesList;
            }
            catch (Exception ex)
            {
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                throw ex;
            }
        }

        /// <summary>
        /// this function is use in EMI Info details
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public EmiInformation GetEmiListbyID(int Id)
        {
            EmiInformation emidetail = new EmiInformation();
            try
            {
                var query = DbHelper.SelectMethod(string.Format(QueryHelper.Getemplistbyid, Id));
                if (query.Rows.Count > 0)
                {
                    emidetail.notificationtoken = Convert.ToString(query.Rows[0]["notificationtoken"]);
                    emidetail.applicationno = Convert.ToInt32(query.Rows[0]["applicationno"]);
                    emidetail.ApplicantName = Convert.ToString(query.Rows[0]["first_name"]) + " " + Convert.ToString(query.Rows[0]["middle_name"]) + " " + Convert.ToString(query.Rows[0]["last_name"]);
                    emidetail.updatedby = Convert.ToString(query.Rows[0]["first_name"]);
                    emidetail.ptp = Convert.ToString(query.Rows[0]["current_date_ptp"]);
                    emidetail.contract_ref_no = Convert.ToString(query.Rows[0]["contract_ref_no"]);
                    emidetail.notes = "done";
                    emidetail.phonestatus = "done";
                    emidetail.nooftimesdialed = "done";
                    emidetail.dateapplied = Convert.ToString(query.Rows[0]["dateapplied"]);
                    emidetail.personalemail = Convert.ToString(query.Rows[0]["personalemail"]);
                    emidetail.personalcontactnoone = Convert.ToString(query.Rows[0]["personalcontactno"]);
                    emidetail.personalcontactnotwo = Convert.ToString(query.Rows[0]["relativecontactno"]);
                    emidetail.personalcontactnothree = Convert.ToString(query.Rows[0]["coworkercontactno"]);
                    emidetail.address = Convert.ToString(query.Rows[0]["address"]);
                    emidetail.province_name = Convert.ToString(query.Rows[0]["province_name"]);
                    emidetail.street = Convert.ToString(query.Rows[0]["street"]);
                    emidetail.barangay = Convert.ToString(query.Rows[0]["barangay_name"]);
                    emidetail.cityname = Convert.ToString(query.Rows[0]["cityname"]);
                    emidetail.zipcode = Convert.ToString(query.Rows[0]["zipcode"]);
                    emidetail.homephonenoifany = Convert.ToString(query.Rows[0]["homephoneno"]);
                    emidetail.placeofbirth = Convert.ToString(query.Rows[0]["placeofbirth"]);
                    emidetail.dateofbirth = Convert.ToString(query.Rows[0]["dateofbirth"]);
                    emidetail.civilstatus = Convert.ToString(query.Rows[0]["civilname"]);
                    emidetail.mothermaidenname = Convert.ToString(query.Rows[0]["mothermaidenname"]);
                    emidetail.mothersaddress = Convert.ToString(query.Rows[0]["motheraddress"]);
                    emidetail.companyname = Convert.ToString(query.Rows[0]["companyname"]);
                    emidetail.companyaddress = Convert.ToString(query.Rows[0]["companyaddress"]);
                    emidetail.position = Convert.ToString(query.Rows[0]["designation"]);
                    emidetail.grossincome = Convert.ToString(query.Rows[0]["gross_income"]);
                    emidetail.referancename = Convert.ToString(query.Rows[0]["reference_name"]);
                    emidetail.referancecontactno = Convert.ToString(query.Rows[0]["reference_contactno"]);
                    emidetail.bankname = Convert.ToString(query.Rows[0]["bankname"]);
                    emidetail.bankaccountno = Convert.ToString(query.Rows[0]["bankaccountno"]);
                    emidetail.gov_id_url = Convert.ToString(query.Rows[0]["gov_id_url"]);
                    emidetail.companyid_url = Convert.ToString(query.Rows[0]["companyid_url"]);
                    emidetail.billing_url = Convert.ToString(query.Rows[0]["billing_url"]);
                    emidetail.income_url = Convert.ToString(query.Rows[0]["income_url"]);
                    emidetail.other_url = Convert.ToString(query.Rows[0]["other_url"]);
                    emidetail.atm_url = Convert.ToString(query.Rows[0]["atm_url"]);
                    emidetail.term_type = Convert.ToString(query.Rows[0]["termtype"]);
                    emidetail.term = Convert.ToString(query.Rows[0]["term"]);
                    emidetail.disbursement_date = Convert.ToString(query.Rows[0]["disbursement_date"]);
                    emidetail.IsCompleted = Convert.ToBoolean(query.Rows[0]["iscompleted"]);
                }
                var historyptpquery = DbHelper.SelectMethod(string.Format(QueryHelper.GetptpHistory, emidetail.applicationno));
                if (historyptpquery != null && historyptpquery.Rows.Count > 0)
                {
                    emidetail.besttimetocall = Convert.ToString(historyptpquery.Rows[0]["best_time_callback"]);
                    emidetail.resonofnonpayment = Convert.ToString(historyptpquery.Rows[0]["non_payment_reason"]);
                    emidetail.otherremarks = Convert.ToString(historyptpquery.Rows[0]["other_remarks"]);

                }
                var query1 = DbHelper.SelectMethod(string.Format(QueryHelper.PenalityAmount, Id));
                if (query1.Rows.Count > 0)
                {
                    emidetail.referenceno = Convert.ToString(query1.Rows[0]["reference_no"]);
                    emidetail.new_referenceno = Convert.ToString(query1.Rows[0]["reference_no"]);
                    emidetail.disbursement_date = Convert.ToString(query1.Rows[0]["disbursement_date"]);
                    emidetail.panalityamountfirst = Convert.ToString(query1.Rows[0]["penality_amount1"]);
                    emidetail.panalityamountsecound = Convert.ToString(query1.Rows[0]["penality_amount2"]);
                    emidetail.panalityamountthird = Convert.ToString(query1.Rows[0]["penality_amount3"]);
                    emidetail.panalityamountfour = Convert.ToString(query1.Rows[0]["penality_amount4"]);
                    emidetail.panalityamountfive = Convert.ToString(query1.Rows[0]["penality_amount5"]);
                }
                var query_2 = DbHelper.SelectMethod(String.Format("select * from tblapplication_emi where application_no in (select applicationno from tblapplication_record where user_id in (select user_id from tblapplication_record where applicationno={0}) and applicationno!={0});", Id));
                if (query_2 != null && query_2.Rows.Count > 0)
                {
                    if (String.IsNullOrWhiteSpace(emidetail.referenceno))
                    {
                        emidetail.referenceno = Convert.ToString(query_2.Rows[0]["reference_no"]);
                    }
                    if (String.IsNullOrWhiteSpace(emidetail.new_referenceno))
                    {
                        emidetail.new_referenceno = Convert.ToString(query_2.Rows[0]["reference_no"]);
                    }

                }

                var query2 = DbHelper.SelectMethod(string.Format(QueryHelper.GetEmiDetail, Id));
                emidetail.Records = new List<TermsRecord>();
                if (query2.Rows.Count > 0)
                {
                    for (int i = 0; i < query2.Rows.Count; i++)
                    {
                        var record = new TermsRecord
                        {
                            fourthweekdate = Convert.ToString(query2.Rows[i]["emi_date"]),
                            fourthweekprinciple = Convert.ToString(query2.Rows[i]["emi_principal"]),
                            fourthweekemiamount = Convert.ToString(query2.Rows[i]["emi_amount"]),
                            fourthweekrate = Convert.ToString(query2.Rows[i]["emi_rate"])
                        };
                        emidetail.Records.Add(record);
                    }
                }

                var paidquery = DbHelper.SelectMethod(string.Format(QueryHelper.GetPaidAmount, Id));
                if (paidquery != null && paidquery.Rows.Count > 0)
                {
                    emidetail.totalamountpaid = Convert.ToString(paidquery.Rows[0]["totalpaid"]) == "" ? "0.00" : paidquery.Rows[0]["totalpaid"] == null ? "0.00" : Convert.ToString(paidquery.Rows[0]["totalpaid"]);
                }
                var balancequery = DbHelper.SelectMethod(string.Format(QueryHelper.GetSumEmiAmount_details, Id));
                if (balancequery.Rows.Count > 0)
                {
                    emidetail.balanceamt = Convert.ToString(balancequery.Rows[0]["balanceamount"]) == "" ? "0.00" : balancequery.Rows[0]["balanceamount"] == null ? "0.00" : Convert.ToString(balancequery.Rows[0]["balanceamount"]);
                }

                if (!String.IsNullOrWhiteSpace(emidetail.term_type) && !String.IsNullOrWhiteSpace(emidetail.term))
                {
                    var query3 = DbHelper.SelectMethod(string.Format(QueryHelper.GetTermDetail, emidetail.term_type, emidetail.term));
                    if (query3.Rows.Count > 0)
                    {
                        emidetail.term_name = Convert.ToString(query3.Rows[0]["term_name"]);
                        emidetail.term_value = Convert.ToString(query3.Rows[0]["term_value"]);
                    }
                }
                return emidetail;
            }
            catch (Exception ex)
            {
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                throw ex;
            }
        }

        //Done By Rashmi
        public List<EmiInformation> GetPendingEmi(int appNo)
        {
            List<EmiInformation> emiInfo = new List<EmiInformation>();
            try
            {
                var query = DbHelper.SelectMethod(string.Format(QueryHelper.GetPendingPaymentsInfo, appNo));
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
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                throw ex;
            }
            return emiInfo;
        }

        public bool DeletePendingPayment(decimal penaltyAmount, string remark, int payId, int EmiPayment)
        {
            bool result = false;

            try
            {
                if (EmiPayment == 1)
                {
                    int result1 = DbHelper.InsertUpdateDelete(string.Format(QueryHelper.DisapprovePaymentDeferment, penaltyAmount, payId));
                    if (result1 > 0)
                    {
                        result = true;
                    }
                }
                else
                {
                    int result1 = DbHelper.InsertUpdateDelete(string.Format(QueryHelper.DisapprovePayment, penaltyAmount, remark, payId));
                    if (result1 > 0)
                    {
                        result = true;
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

        public bool ApprovePayment(decimal paidAmt, decimal penaltyAmount, string remark, int payId, int appNo, int EmiPayment)
        {
            bool result = false;
            bool flag = false;
            int query = 0;
            PaymentInformation paymentInfo = new PaymentInformation();
            try
            {
                if (EmiPayment == 1)
                {
                    DateTime OldEmiDate = new DateTime();
                    DateTime NewEMIDate = new DateTime();
                    string NewDate = string.Empty;
                    string emi_date = string.Empty;
                    string AgentName = string.Empty;
                    double EmiAmount = 0.0;
                    double EmiRate = 0.0;
                    double EmiPrinciple = 0.0;
                    int ApprovedTerm = 0;


                    var _dt = DbHelper.SelectMethod(String.Format(QueryHelper.GetPenaltyDeferment, payId));
                    if (_dt != null && _dt.Rows.Count > 0)
                    {
                        var getEmi = DbHelper.SelectMethod(string.Format(QueryHelper.GetEmiDetailNew, appNo));
                        if (getEmi != null && getEmi.Rows.Count > 0)
                        {
                            double totalamountpaid = 0;
                            totalamountpaid = float.Parse(Convert.ToString(_dt.Rows[0]["late_penalty_amount"]));
                            paymentInfo.emiid = Convert.ToInt64(getEmi.Rows[0]["emi_id"]);

                            DataTable DtSum = new DataTable();
                            DtSum = DbHelper.SelectMethod(String.Format(QueryHelper.EmiDetailsNew, paymentInfo.emiid));
                            if (DtSum != null && DtSum.Rows.Count > 0)
                            {
                                paymentInfo.emidetailid = Convert.ToInt64(getEmi.Rows[0]["emi_detail_id"]);
                                for (int k = 0; k < DtSum.Rows.Count; k++)
                                {
                                    OldEmiDate = Convert.ToDateTime(DtSum.Rows[k]["emi_date"]);
                                    ApprovedTerm = Convert.ToInt32(DtSum.Rows[k]["approved_term"]);
                                }
                                if (DtSum.Rows.Count == 1)
                                {
                                    NewEMIDate = OldEmiDate.AddDays(ApprovedTerm / DtSum.Rows.Count);
                                    NewDate = NewEMIDate.ToString("yyyy-MM-dd");
                                }
                                else
                                {
                                    DateTime secondDate = Convert.ToDateTime(DtSum.Rows[1]["emi_date"]);
                                    DateTime firstDate = Convert.ToDateTime(DtSum.Rows[0]["emi_date"]);

                                    double diff2 = (secondDate - firstDate).TotalDays;
                                    NewEMIDate = OldEmiDate.AddDays(diff2);
                                    NewDate = NewEMIDate.ToString("yyyy-MM-dd");
                                }

                                int a = DtSum.Rows.Count + 1;
                                EmiAmount = Convert.ToDouble(DtSum.Rows[0]["emi_amount"]);
                                EmiPrinciple = Convert.ToDouble(DtSum.Rows[0]["emi_principal"]);
                                EmiRate = Convert.ToDouble(DtSum.Rows[0]["emi_rate"]);
                            }
                            var query1 = DbHelper.SelectMethod(string.Format(QueryHelper.GetPaidAmountByEmiDetailId, paymentInfo.emidetailid));
                            double paidamount = 0.0;
                            DateTime EmiDate;
                            if (query1 != null && query1.Rows.Count > 0)
                            {

                                paidamount = Convert.ToDouble(query1.Rows[0]["paidamount"]);
                                EmiDate = Convert.ToDateTime(query1.Rows[0]["emi_date"]);
                                string EmiDate1 = EmiDate.ToString("yyyy-MM-dd");

                                if (paidamount > 0)
                                {
                                    DataTable DtSum1 = new DataTable();
                                    DtSum1 = DbHelper.SelectMethod(String.Format(QueryHelper.GetNextEmiId, paymentInfo.emiid, EmiDate1));

                                    if (DtSum1 != null && DtSum1.Rows.Count > 0)
                                    {
                                        int NextEmiID = Convert.ToInt32(DtSum1.Rows[0]["emi_detail_id"]);
                                        int l = DbHelper.InsertUpdateDelete(string.Format(QueryHelper.UpdateEmiPaidAmount, paidamount, NextEmiID));
                                    }

                                }

                            }
                            int m = DbHelper.InsertUpdateDelete(string.Format(QueryHelper.UpdateEmiDetails, paymentInfo.emidetailid));
                            if (m > 0)
                            {
                                flag = true;
                            }

                            int i = DbHelper.InsertUpdateDelete(string.Format(QueryHelper.InsertEmiDtail, paymentInfo.emiid, NewDate, EmiAmount, EmiRate, EmiPrinciple, 0, AgentName, EmiAmount));
                            if (i > 0)
                            {
                                flag = true;
                            }
                            result = flag;
                        }

                    }

                    query = DbHelper.InsertUpdateDelete(string.Format(QueryHelper.ApproveDeferment, payId, appNo));
                    if (query > 0)
                    {
                        result = flag;
                    }

                }
                else
                {
                    var query8 = DbHelper.SelectMethod(string.Format(QueryHelper.SelectIsDefaulter, appNo));
                    if (query8 != null && query8.Rows.Count > 0)
                    {
                        bool isdefaulter;
                        isdefaulter = Convert.ToBoolean(query8.Rows[0]["isdefaulter"]);
                        if (isdefaulter == true)
                        {
                            query = DbHelper.InsertUpdateDelete(string.Format(QueryHelper.ApprovePaymentDefaulter, paidAmt, penaltyAmount, remark, payId, appNo));
                        }
                        else
                        {
                            query = DbHelper.InsertUpdateDelete(string.Format(QueryHelper.ApprovePayment, paidAmt, penaltyAmount, remark, payId, appNo));
                        }
                    }

                    if (query > 0)
                    {
                        var _dt = DbHelper.SelectMethod(String.Format(QueryHelper.GetAmount, payId));
                        if (_dt != null && _dt.Rows.Count > 0)
                        {
                            var getEmi = DbHelper.SelectMethod(string.Format(QueryHelper.GetEmiDetail, appNo));
                            if (getEmi != null && getEmi.Rows.Count > 0)
                            {
                                double totalamountpaid = 0;
                                DateTime _Paid_on = DateTime.Now;
                                totalamountpaid = float.Parse(Convert.ToString(_dt.Rows[0]["paid_amount"]));
                                _Paid_on = Convert.ToDateTime(_dt.Rows[0]["paid_on"]);
                                paymentInfo.emiid = Convert.ToInt64(getEmi.Rows[0]["emi_id"]);
                                for (int i = 0; i < getEmi.Rows.Count; i++)
                                {
                                    paymentInfo.emiamount = Convert.ToString(getEmi.Rows[i]["emi_amount"]);
                                    paymentInfo.Paidamount = Convert.ToString(getEmi.Rows[i]["paidamount"]);
                                    paymentInfo.Balanceamount = Convert.ToString(getEmi.Rows[i]["balanceamount"]);
                                    paymentInfo.emidetailid = Convert.ToInt64(getEmi.Rows[i]["emi_detail_id"]);
                                    paymentInfo.emistatus = Convert.ToInt64(getEmi.Rows[i]["emi_status"]);

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
                                            DbHelper.InsertUpdateDelete(string.Format(QueryHelper.UpdateEmiAmount, paymentInfo.emistatus, totalamountpaid, totalBalanceamount, paymentInfo.emidetailid, _Paid_on.ToString("yyyy-MM-dd hh:mm:ss")));

                                            if (totalBalanceamount == 0)
                                            {
                                                int result2 = DbHelper.InsertUpdateDelete(string.Format(QueryHelper.UpdateIsPay, paymentInfo.emidetailid));
                                                if (result2 > 0)
                                                {
                                                    flag = true;
                                                }
                                            }
                                            else if (totalBalanceamount > 0)
                                            {
                                                int result3 = DbHelper.InsertUpdateDelete(string.Format(QueryHelper.UpdateIsPartial, paymentInfo.emidetailid));
                                                if (result3 > 0)
                                                {
                                                    flag = true;
                                                }
                                            }

                                            i = getEmi.Rows.Count;
                                        }
                                        else if (totalamountpaid > Balanceamount && totalamountpaid > 0)
                                        {
                                            paymentInfo.emistatus = 2;
                                            totalamountpaid -= Balanceamount;
                                            var EqualBalanceamount = 0;
                                            DbHelper.InsertUpdateDelete(string.Format(QueryHelper.UpdateEmiAmount, paymentInfo.emistatus, Balanceamount, EqualBalanceamount, paymentInfo.emidetailid, _Paid_on.ToString("yyyy-MM-dd hh:mm:ss")));
                                            int result3 = DbHelper.InsertUpdateDelete(string.Format(QueryHelper.UpdateIsPay, paymentInfo.emidetailid));
                                            if (result3 > 0)
                                            {
                                                flag = true;
                                            }

                                        }
                                        else if (totalamountpaid == Balanceamount && totalamountpaid > 0)
                                        {
                                            paymentInfo.emistatus = 2;
                                            var EqualBalanceamount = 0;
                                            DbHelper.InsertUpdateDelete(string.Format(QueryHelper.UpdateEmiAmount, paymentInfo.emistatus, totalamountpaid, EqualBalanceamount, paymentInfo.emidetailid, _Paid_on.ToString("yyyy-MM-dd hh:mm:ss")));
                                            int result4 = DbHelper.InsertUpdateDelete(string.Format(QueryHelper.UpdateIsPay, paymentInfo.emidetailid));
                                            if (result4 > 0)
                                            {
                                                flag = true;
                                            }

                                            i = getEmi.Rows.Count;
                                        }
                                    }
                                }
                                var getBalance = DbHelper.SelectMethod(string.Format(QueryHelper.GetBalanceNew, paymentInfo.emiid));
                                if (getBalance != null && getBalance.Rows.Count >= 0)
                                {
                                    double balance = 0.0;
                                    balance = Convert.ToDouble(getBalance.Rows[0]["balanceamount"]);
                                    if (balance == 0)
                                    {
                                        int result5 = DbHelper.InsertUpdateDelete(string.Format(QueryHelper.UpdateisforFSBucket, appNo));
                                        if (result5 > 0)
                                        {
                                            flag = true;
                                        }
                                    }
                                }
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

        public List<Checkerdetail> GetInsufficientDocBucketList()
        {
            List<Checkerdetail> namesList = new List<Checkerdetail>();
            try
            {
                var query = DbHelper.SelectMethod(QueryHelper.GetInsufficientDocBucketList);
                if (query != null && query.Rows.Count > 0)
                    for (int i = 0; i < query.Rows.Count; i++)
                    {
                        Checkerdetail checkdetail = new Checkerdetail
                        {
                            Id = 1 + Convert.ToInt32(i),
                            applicationname = Convert.ToString(query.Rows[i]["first_name"]) + " " + Convert.ToString(query.Rows[i]["middle_name"]) + " " + Convert.ToString(query.Rows[i]["last_name"]),
                            applicationno = Convert.ToInt32(query.Rows[i]["applicationno"]),
                            RequestDate = Convert.ToString(query.Rows[i]["dateapplied"]),
                            address = Convert.ToString(query.Rows[i]["address"]),
                            term = Convert.ToString(query.Rows[i]["term"]) != string.Empty ? Convert.ToInt32(query.Rows[i]["term"]) : 0,
                            companyid_url = Convert.ToString(query.Rows[i]["companyid_url"]),
                            gov_id_url = Convert.ToString(query.Rows[i]["gov_id_url"]),
                            billing_url = Convert.ToString(query.Rows[i]["billing_url"]),
                            income_url = Convert.ToString(query.Rows[i]["income_url"]),
                            other_url = Convert.ToString(query.Rows[i]["other_url"])
                        };

                        //Best Time to Morning Call
                        int BestTimeToCallMorning = Convert.ToString(query.Rows[i]["morning_time"]).Trim() == string.Empty ? -1 : Convert.ToInt32(query.Rows[i]["morning_time"]);
                        if (BestTimeToCallMorning != -1)
                            checkdetail.MorningTime = CommonMethods.GetBestTimeToCallMorning(BestTimeToCallMorning);
                        else
                            checkdetail.MorningTime = string.Empty;

                        //Best Time to Noon Call
                        int BestTimeToCallNoon = Convert.ToString(query.Rows[i]["noon_time"]).Trim() == string.Empty ? -1 : Convert.ToInt32(query.Rows[i]["noon_time"]);
                        if (BestTimeToCallNoon != -1)
                            checkdetail.NoonTime = CommonMethods.GetBestTimeToCallNoon(BestTimeToCallNoon);
                        else
                            checkdetail.NoonTime = string.Empty;


                        namesList.Add(checkdetail);
                    }
                return namesList;
            }
            catch (Exception ex)
            {
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                throw ex;
            }
        }

        //Created By Rashmi

        public bool UpdatePendingPayments(decimal penaltyAmount, string remarks, int payId)
        {
            bool result = false;
            try
            {
                int query = DbHelper.InsertUpdateDelete(string.Format(QueryHelper.UpdatePendingPayments, penaltyAmount, remarks, payId));
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

        /// <summary>
        /// Get Remark List
        /// </summary>
        /// <returns></returns>


        public int TransferToVerifier(int application_id, int user_id)
        {

            try
            {
                int i = DbHelper.InsertUpdateDelete(string.Format(QueryHelper.TransferToVerifier, application_id, user_id));
                return i;
            }
            catch (Exception ex)
            {

                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                throw ex;
            }

        }

        /// <summary>
        /// Transfer Lead To Verifier
        /// </summary>
        /// <returns></returns>
        public int TransferToReVerifier(int application_id, int user_id)
        {

            try
            {
                int i = DbHelper.InsertUpdateDelete(string.Format(QueryHelper.TransferToReVerifier, application_id, user_id));
                return i;
            }
            catch (Exception ex)
            {
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                throw ex;
            }

        }

        /// <summary>
        /// Get Orr False Checker Applications
        /// </summary>
        /// <returns></returns>
        public List<Checkerdetail> GetIsOrrFalseCheckerApplications(List<Checkerdetail> list)
        {
            List<Checkerdetail> final_list = new List<Checkerdetail>();
            try
            {

                if (list != null && list.Count > 0)
                {
                    foreach (var item in list)
                    {
                        DataTable OrrSSSNo = new DataTable();
                        DataTable OrrOccupation = new DataTable();
                        DataTable OrrBarangay = new DataTable();

                        bool is_orr_sssno = false;
                        bool is_orr_occupation = false;
                        bool is_orr_barangay = false;

                        if (item.sss_no != null && item.sss_no != "")
                        {
                            string IsOrrSSSNo = string.Format(QueryHelper.IsOrrSSSNo, item.sss_no);
                            OrrSSSNo = DbHelper.SelectMethod(IsOrrSSSNo);
                        }

                        if (item.barangay != 0)
                        {
                            string IsOrrBarangay = string.Format(QueryHelper.IsOrrBarangay, item.barangay);
                            OrrBarangay = DbHelper.SelectMethod(IsOrrBarangay);
                        }
                        if (item.occupation != 0)
                        {
                            string IsOrrOccupation = string.Format(QueryHelper.IsOrrOccupation, item.occupation);
                            OrrOccupation = DbHelper.SelectMethod(IsOrrOccupation);
                        }

                        if ((OrrSSSNo != null && OrrSSSNo.Rows.Count > 0))
                        {
                            if ((Convert.ToInt32(OrrSSSNo.Rows[0][0]) > 0))
                                is_orr_sssno = true;
                        }

                        if ((OrrOccupation != null && OrrOccupation.Rows.Count > 0))
                        {
                            if ((Convert.ToInt32(OrrOccupation.Rows[0][0]) > 0))
                                is_orr_occupation = true;
                        }
                        if ((OrrBarangay != null && OrrBarangay.Rows.Count > 0))
                        {
                            if ((Convert.ToInt32(OrrBarangay.Rows[0][0]) > 0))
                                is_orr_barangay = true;
                        }

                        if (!is_orr_occupation && !is_orr_sssno && !is_orr_barangay)
                        {
                            final_list.Add(item);
                        }
                    }
                }


            }
            catch (Exception ex)
            {
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                throw ex;
            }
            return final_list;

        }

        /// <summary>
        /// Get Orr False Checker Applications
        /// </summary>
        /// <returns></returns>
        public bool GetIsOrrFalseCheckerApplications(string sss_no, int barangay, int occupation)
        {
            try
            {

                DataTable OrrSSSNo = new DataTable();
                DataTable OrrOccupation = new DataTable();
                DataTable OrrBarangay = new DataTable();

                bool is_orr_sssno = false;
                bool is_orr_occupation = false;
                bool is_orr_barangay = false;

                if (sss_no != null && sss_no != "")
                {
                    string IsOrrSSSNo = string.Format(QueryHelper.IsOrrSSSNo, sss_no);
                    OrrSSSNo = DbHelper.SelectMethod(IsOrrSSSNo);
                }

                if (barangay != 0)
                {
                    string IsOrrBarangay = string.Format(QueryHelper.IsOrrBarangay, barangay);
                    OrrBarangay = DbHelper.SelectMethod(IsOrrBarangay);
                }
                if (occupation != 0)
                {
                    string IsOrrOccupation = string.Format(QueryHelper.IsOrrOccupation, occupation);
                    OrrOccupation = DbHelper.SelectMethod(IsOrrOccupation);
                }

                if ((OrrSSSNo != null && OrrSSSNo.Rows.Count > 0))
                {
                    if ((Convert.ToInt32(OrrSSSNo.Rows[0][0]) > 0))
                        is_orr_sssno = true;
                }

                if ((OrrOccupation != null && OrrOccupation.Rows.Count > 0))
                {
                    if ((Convert.ToInt32(OrrOccupation.Rows[0][0]) > 0))
                        is_orr_occupation = true;
                }
                if ((OrrBarangay != null && OrrBarangay.Rows.Count > 0))
                {
                    if ((Convert.ToInt32(OrrBarangay.Rows[0][0]) > 0))
                        is_orr_barangay = true;
                }

                if (is_orr_occupation || is_orr_sssno || is_orr_barangay)
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                throw ex;
            }
            return false;

        }



        /// <summary>
        /// Get CheckerReport FOR ADMIN
        /// </summary>
        /// <returns></returns>
        public Chekerinformation GetCheckerReport()
        {
            try
            {
                Chekerinformation model = new Chekerinformation();
                var query = DbHelper.SelectMethod(string.Format(QueryHelper.GetCheckerTypeUser, RoleType.Checker, RoleType.AdminUser));
                if (query != null && query.Rows.Count > 0)
                {
                    for (int i = 0; i < query.Rows.Count; i++)
                    {
                        model.UserForReportList.Add(new UserForReport
                        {
                            userid = Convert.ToInt32(query.Rows[i]["userid"]),
                            username = Convert.ToString(query.Rows[i]["username"]),
                        });
                    }
                }
                if (model.UserForReportList != null && model.UserForReportList.Count > 0)
                {
                    foreach (var item in model.UserForReportList)
                    {
                        var queryForLeadCount = DbHelper.SelectMethod(string.Format(QueryHelper.GetCheckerLeadCount, item.userid));
                        if (queryForLeadCount != null && queryForLeadCount.Rows.Count > 0)
                        {
                            for (int i = 0; i < queryForLeadCount.Rows.Count; i++)
                            {
                                model.LeadCount.Add(Convert.ToInt32(queryForLeadCount.Rows[i]["lead_count"]));
                            }
                        }
                    }
                }
                if (model.UserForReportList != null && model.UserForReportList.Count > 0)
                {
                    foreach (var item in model.UserForReportList)
                    {
                        var queryForLeadCount = DbHelper.SelectMethod(string.Format(QueryHelper.GetCheckerLeadCount_Verifier_Pushed, item.userid));
                        if (queryForLeadCount != null && queryForLeadCount.Rows.Count > 0)
                        {
                            for (int i = 0; i < queryForLeadCount.Rows.Count; i++)
                            {
                                model.LeadCount_VerifyPushed.Add(Convert.ToInt32(queryForLeadCount.Rows[i]["lead_count"]));
                            }
                        }
                    }
                }
                if (model.UserForReportList != null && model.UserForReportList.Count > 0)
                {
                    foreach (var item in model.UserForReportList)
                    {
                        var queryForLeadCount = DbHelper.SelectMethod(string.Format(QueryHelper.GetCheckerLeadCount_Verifier_Rejected, item.userid));
                        if (queryForLeadCount != null && queryForLeadCount.Rows.Count > 0)
                        {
                            for (int i = 0; i < queryForLeadCount.Rows.Count; i++)
                            {
                                model.LeadCount_VerifyReject.Add(Convert.ToInt32(queryForLeadCount.Rows[i]["lead_count"]));
                            }
                        }
                    }
                }
                if (model.UserForReportList != null && model.UserForReportList.Count > 0)
                {
                    foreach (var item in model.UserForReportList)
                    {
                        var queryForLeadCount = DbHelper.SelectMethod(string.Format(QueryHelper.GetCheckerLeadCount_Approved, item.userid));
                        if (queryForLeadCount != null && queryForLeadCount.Rows.Count > 0)
                        {
                            for (int i = 0; i < queryForLeadCount.Rows.Count; i++)
                            {
                                model.LeadCount_Approved.Add(Convert.ToInt32(queryForLeadCount.Rows[i]["lead_count"]));
                            }
                        }
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

        public Chekerinformation GetCheckerReportByDate(DateTime FromDate, DateTime EndDate)
        {
            try
            {
                Chekerinformation model = new Chekerinformation();
                var query = DbHelper.SelectMethod(string.Format(QueryHelper.GetCheckerTypeUser_dateFilter, RoleType.Checker, RoleType.AdminUser));
                if (query != null && query.Rows.Count > 0)
                {
                    for (int i = 0; i < query.Rows.Count; i++)
                    {
                        model.UserForReportList.Add(new UserForReport
                        {
                            userid = Convert.ToInt32(query.Rows[i]["userid"]),
                            username = Convert.ToString(query.Rows[i]["username"]),
                        });
                    }
                }
                if (model.UserForReportList != null && model.UserForReportList.Count > 0)
                {
                    foreach (var item in model.UserForReportList)
                    {
                        var queryForLeadCount = DbHelper.SelectMethod(string.Format(QueryHelper.GetCheckerLeadCount_dateFilter, item.userid, FromDate.ToString("yyyy-MM-dd"), EndDate.ToString("yyyy-MM-dd")));
                        if (queryForLeadCount != null && queryForLeadCount.Rows.Count > 0)
                        {
                            for (int i = 0; i < queryForLeadCount.Rows.Count; i++)
                            {
                                model.LeadCount.Add(Convert.ToInt32(queryForLeadCount.Rows[i]["lead_count"]));
                            }
                        }
                    }
                }
                if (model.UserForReportList != null && model.UserForReportList.Count > 0)
                {
                    foreach (var item in model.UserForReportList)
                    {
                        var queryForLeadCount = DbHelper.SelectMethod(string.Format(QueryHelper.GetCheckerLeadCount_Verifier_Pushed_dateFilter, item.userid, FromDate.ToString("yyyy-MM-dd"), EndDate.ToString("yyyy-MM-dd")));
                        if (queryForLeadCount != null && queryForLeadCount.Rows.Count > 0)
                        {
                            for (int i = 0; i < queryForLeadCount.Rows.Count; i++)
                            {
                                model.LeadCount_VerifyPushed.Add(Convert.ToInt32(queryForLeadCount.Rows[i]["lead_count"]));
                            }
                        }
                    }
                }
                if (model.UserForReportList != null && model.UserForReportList.Count > 0)
                {
                    foreach (var item in model.UserForReportList)
                    {
                        var queryForLeadCount = DbHelper.SelectMethod(string.Format(QueryHelper.GetCheckerLeadCount_Verifier_Rejected_dateFilter, item.userid, FromDate.ToString("yyyy-MM-dd"), EndDate.ToString("yyyy-MM-dd")));
                        if (queryForLeadCount != null && queryForLeadCount.Rows.Count > 0)
                        {
                            for (int i = 0; i < queryForLeadCount.Rows.Count; i++)
                            {
                                model.LeadCount_VerifyReject.Add(Convert.ToInt32(queryForLeadCount.Rows[i]["lead_count"]));
                            }
                        }
                    }
                }
                if (model.UserForReportList != null && model.UserForReportList.Count > 0)
                {
                    foreach (var item in model.UserForReportList)
                    {
                        var queryForLeadCount = DbHelper.SelectMethod(string.Format(QueryHelper.GetCheckerLeadCount_Approved_dateFilter, item.userid, FromDate.ToString("yyyy-MM-dd"), EndDate.ToString("yyyy-MM-dd")));
                        if (queryForLeadCount != null && queryForLeadCount.Rows.Count > 0)
                        {
                            for (int i = 0; i < queryForLeadCount.Rows.Count; i++)
                            {
                                model.LeadCount_Approved.Add(Convert.ToInt32(queryForLeadCount.Rows[i]["lead_count"]));
                            }
                        }
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

        /// <summary>
        /// Get Verifier report for admin
        /// </summary>
        /// <returns></returns>
        public CompleteAppVerificationDetails GetVerifierReport()
        {
            try
            {
                CompleteAppVerificationDetails model = new CompleteAppVerificationDetails();
                var query = DbHelper.SelectMethod(string.Format(QueryHelper.GetVerifierTypeUser, RoleType.Verifier, RoleType.AdminUser));
                if (query != null && query.Rows.Count > 0)
                {
                    for (int i = 0; i < query.Rows.Count; i++)
                    {
                        model.UserForVerifierReportList.Add(new UserForVerifierReport
                        {
                            user_id = Convert.ToInt32(query.Rows[i]["userid"]),
                            user_name = Convert.ToString(query.Rows[i]["username"]),
                        });
                    }
                }
                if (model.UserForVerifierReportList != null && model.UserForVerifierReportList.Count > 0)
                {
                    foreach (var item in model.UserForVerifierReportList)
                    {
                        var queryForLeadCount = DbHelper.SelectMethod(string.Format(QueryHelper.GetVerifieLeadCount, item.user_id));
                        if (queryForLeadCount != null && queryForLeadCount.Rows.Count > 0)
                        {
                            for (int i = 0; i < queryForLeadCount.Rows.Count; i++)
                            {
                                model.LeadCount_verifier.Add(Convert.ToInt32(queryForLeadCount.Rows[i]["lead_count"]));
                            }
                        }
                    }
                }
                if (model.UserForVerifierReportList != null && model.UserForVerifierReportList.Count > 0)
                {
                    foreach (var item in model.UserForVerifierReportList)
                    {
                        var queryForLeadCount = DbHelper.SelectMethod(string.Format(QueryHelper.GetVerifieLeadCount_Verifier_Pushed, item.user_id));
                        if (queryForLeadCount != null && queryForLeadCount.Rows.Count > 0)
                        {
                            for (int i = 0; i < queryForLeadCount.Rows.Count; i++)
                            {
                                model.LeadCount_VerifyPushed_verifier.Add(Convert.ToInt32(queryForLeadCount.Rows[i]["lead_count"]));
                            }
                        }
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

        public CompleteAppVerificationDetails GetVerifierReportByDate(DateTime From_Date, DateTime End_Date)
        {
            try
            {
                CompleteAppVerificationDetails model = new CompleteAppVerificationDetails();
                var query = DbHelper.SelectMethod(string.Format(QueryHelper.GetVerifiererTypeUser_dateFilter, RoleType.Verifier, RoleType.AdminUser));
                if (query != null && query.Rows.Count > 0)
                {
                    for (int i = 0; i < query.Rows.Count; i++)
                    {
                        model.UserForVerifierReportList.Add(new UserForVerifierReport
                        {
                            user_id = Convert.ToInt32(query.Rows[i]["userid"]),
                            user_name = Convert.ToString(query.Rows[i]["username"]),
                        });
                    }
                }
                if (model.UserForVerifierReportList != null && model.UserForVerifierReportList.Count > 0)
                {
                    foreach (var item in model.UserForVerifierReportList)
                    {
                        var queryForLeadCount = DbHelper.SelectMethod(string.Format(QueryHelper.GetVerifierLeadCount_dateFilter, item.user_id, From_Date.ToString("yyyy-MM-dd"), End_Date.ToString("yyyy-MM-dd")));
                        if (queryForLeadCount != null && queryForLeadCount.Rows.Count > 0)
                        {
                            for (int i = 0; i < queryForLeadCount.Rows.Count; i++)
                            {
                                model.LeadCount_verifier.Add(Convert.ToInt32(queryForLeadCount.Rows[i]["lead_count"]));
                            }
                        }
                    }
                }
                if (model.UserForVerifierReportList != null && model.UserForVerifierReportList.Count > 0)
                {
                    foreach (var item in model.UserForVerifierReportList)
                    {
                        var queryForLeadCount = DbHelper.SelectMethod(string.Format(QueryHelper.GetVerifierLeadCount_Verifier_Pushed_dateFilter, item.user_id, From_Date.ToString("yyyy-MM-dd"), End_Date.ToString("yyyy-MM-dd")));
                        if (queryForLeadCount != null && queryForLeadCount.Rows.Count > 0)
                        {
                            for (int i = 0; i < queryForLeadCount.Rows.Count; i++)
                            {
                                model.LeadCount_VerifyPushed_verifier.Add(Convert.ToInt32(queryForLeadCount.Rows[i]["lead_count"]));
                            }
                        }
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

        /// <summary>
        /// Get FsBucketLis tBy ApplicationNo
        /// </summary>
        /// <param name="application_id"></param>
        /// <returns></returns>

        public List<PaymentBucket> GetOutstandingAmount(string ApplicationId)
        {
            List<PaymentBucket> namesList = new List<PaymentBucket>();
            try
            {
                var query = DbHelper.SelectMethod(String.Format(QueryHelper.GetPaymentBucketByIDLatest, ApplicationId));
                if (query != null && query.Rows.Count > 0)
                {
                    for (int i = 0; i < query.Rows.Count; i++)
                    {
                        PaymentBucket checkdetail = new PaymentBucket
                        {
                            Id = 1 + Convert.ToInt32(i),
                            emi_id = Convert.ToInt32(query.Rows[i]["emi_id"]),
                            EMIDetailsID = Convert.ToInt32(query.Rows[i]["emi_detail_id"]),
                            emiamount = Convert.ToString(query.Rows[i]["emi_amount"]),
                            Aging = Convert.ToString(query.Rows[i]["aging"]) == "+0" ? "0" : Convert.ToString(query.Rows[i]["aging"]),
                            ReferenceNumber = Convert.ToString(query.Rows[i]["reference_no"]) != string.Empty ? Convert.ToString(query.Rows[i]["reference_no"]) : string.Empty,
                            PaidAmount = Convert.ToDouble(query.Rows[i]["paidamount"]),
                            OutstandingAmount = Convert.ToDouble(query.Rows[i]["balanceamount"]),
                            applicationname = Convert.ToString(query.Rows[i]["full_name"]),
                            applicationno = Convert.ToInt32(query.Rows[i]["application_no"]),
                            RequestDate = Convert.ToString(query.Rows[i]["dateapplied"]),
                            personalcontactno = Convert.ToString(query.Rows[i]["personalcontactno"]),
                            Priority = Convert.ToString(query.Rows[i]["priority"]),
                            SortingOrder = Convert.ToInt32(query.Rows[i]["sortingorder"]),
                            ContractNo = Convert.ToString(query.Rows[i]["contract_ref_no"]) != string.Empty ? Convert.ToString(query.Rows[i]["contract_ref_no"]) : string.Empty,
                            IsPickedForCollector = Convert.ToString(query.Rows[i]["ispickedforcollector"]) != string.Empty && Convert.ToBoolean(query.Rows[i]["ispickedforcollector"]),
                            _BalanceAmount = Convert.ToString(query.Rows[i]["balanceamount"]) != "" ? Convert.ToInt32(query.Rows[i]["balanceamount"]) : 0,
                            termtype = Convert.ToString(query.Rows[i]["termtype"]),
                            pastduedays = Convert.ToString(query.Rows[i]["aging"]) == "" ? 0 : Convert.ToInt32(query.Rows[i]["aging"]),
                            isdefaulter = Convert.ToBoolean(query.Rows[i]["isdefaulter"])
                        };

                        #region Penality Calculation

                        int PastEmiCount = 0;
                        var PassedEMICount = DbHelper.SelectMethod(string.Format(QueryHelper.PastEmiCount, checkdetail.applicationno));
                        if (PassedEMICount != null && PassedEMICount.Rows.Count > 0)
                        {
                            PastEmiCount = PassedEMICount.Rows[0]["emicount"] == DBNull.Value ? 0 : Convert.ToInt32(PassedEMICount.Rows[0]["emicount"]);
                        }
                        if (checkdetail._BalanceAmount > 0 && checkdetail.pastduedays > 0)
                        {
                            double PercentPenalty = (Convert.ToDouble(Convert.ToString(query.Rows[i]["late_rate"])) / 100);
                            double penalty = Convert.ToDouble(Convert.ToString(query.Rows[i]["late_penalty"]));

                            if (checkdetail.termtype == "Weekly")
                            {
                                double _Mode = 0;
                                if (checkdetail.pastduedays != 0 && checkdetail.pastduedays < 7)
                                {
                                    _Mode = 1;
                                }
                                else if (checkdetail.pastduedays == 0)
                                {
                                    _Mode = 0;
                                }
                                else
                                {
                                    _Mode = (checkdetail.pastduedays % 7);
                                }
                                Double _PenalityCount = Convert.ToDouble(checkdetail.pastduedays / 7);
                                List<string> s = Convert.ToString(_PenalityCount).Split('.').ToList();
                                _PenalityCount = Convert.ToDouble(s.First());
                                int _TotalPenalityCount = _Mode > 0 ? Convert.ToInt32(_PenalityCount + 1) : Convert.ToInt32(_PenalityCount);
                                checkdetail.penalty = (_TotalPenalityCount * penalty);
                                double latefee = (((checkdetail._BalanceAmount) * (PercentPenalty)) * (checkdetail.pastduedays));
                                checkdetail.penalty = Math.Round(checkdetail.penalty + latefee, 2);
                            }
                            if (checkdetail.termtype == "Bi-Weekly")
                            {
                                double _Mode = 0;
                                if (checkdetail.pastduedays != 0 && checkdetail.pastduedays < 14)
                                {
                                    _Mode = 1;
                                }
                                else if (checkdetail.pastduedays == 0)
                                {
                                    _Mode = 0;
                                }
                                else
                                {
                                    _Mode = (checkdetail.pastduedays % 14);
                                }
                                Double _PenalityCount = Convert.ToDouble(checkdetail.pastduedays / 14);
                                List<string> s = Convert.ToString(_PenalityCount).Split('.').ToList();
                                _PenalityCount = Convert.ToDouble(s.First());
                                int _TotalPenalityCount = _Mode > 0 ? Convert.ToInt32(_PenalityCount + 1) : Convert.ToInt32(_PenalityCount);
                                checkdetail.penalty = (_TotalPenalityCount * penalty);
                                double latefee = (((checkdetail._BalanceAmount) * (PercentPenalty)) * (checkdetail.pastduedays));
                                checkdetail.penalty = Math.Round(checkdetail.penalty + latefee, 2);
                            }
                            if (checkdetail.termtype == "Monthly")
                            {
                                double _Mode = 0;
                                if (checkdetail.pastduedays != 0 && checkdetail.pastduedays < 28)
                                {
                                    _Mode = 1;
                                }
                                else if (checkdetail.pastduedays == 0)
                                {
                                    _Mode = 0;
                                }
                                else
                                {
                                    _Mode = (checkdetail.pastduedays % 28);
                                }
                                Double _PenalityCount = Convert.ToDouble(checkdetail.pastduedays / 28);
                                List<string> s = Convert.ToString(_PenalityCount).Split('.').ToList();
                                _PenalityCount = Convert.ToDouble(s.First());
                                int _TotalPenalityCount = _Mode > 0 ? Convert.ToInt32(_PenalityCount + 1) : Convert.ToInt32(_PenalityCount);
                                checkdetail.penalty = (_TotalPenalityCount * penalty);
                                double latefee = (((checkdetail._BalanceAmount) * (PercentPenalty)) * (checkdetail.pastduedays));
                                checkdetail.penalty = Math.Round(checkdetail.penalty + latefee, 2);
                            }
                        }
                        #endregion
                        checkdetail.penalty = checkdetail.penalty > 0 ? checkdetail.penalty : 0;
                        //Best Time to Morning Call
                        int BestTimeToCallMorning = Convert.ToString(query.Rows[i]["morning_time"]).Trim() == string.Empty ? -1 : Convert.ToInt32(query.Rows[i]["morning_time"]);
                        if (BestTimeToCallMorning != -1)
                            checkdetail.MorningTime = CommonMethods.GetBestTimeToCallMorning(BestTimeToCallMorning);
                        else
                            checkdetail.MorningTime = string.Empty;

                        //Best Time to Noon Call
                        int BestTimeToCallNoon = Convert.ToString(query.Rows[i]["noon_time"]).Trim() == string.Empty ? -1 : Convert.ToInt32(query.Rows[i]["noon_time"]);
                        if (BestTimeToCallNoon != -1)
                            checkdetail.NoonTime = CommonMethods.GetBestTimeToCallNoon(BestTimeToCallNoon);
                        else
                            checkdetail.NoonTime = string.Empty;
                        namesList.Add(checkdetail);
                    }

                }

                List<PaymentBucket> FinalList = new List<PaymentBucket>();
                List<PaymentBucket> TempList = new List<PaymentBucket>();
                // Overdue Edit
                TempList.AddRange(namesList.Where(x => x.SortingOrder == 2).ToList());
                foreach (var item in TempList)
                {
                    var _RemovableData = namesList.Where(x => x.applicationno == item.applicationno).ToList();
                    if (_RemovableData != null && _RemovableData.Count > 0)
                    {
                        foreach (var Childitem in _RemovableData)
                        {
                            namesList.Remove(Childitem);
                        }
                    }
                }
                TempList.AddRange(namesList.Where(x => x.SortingOrder == 1).ToList());
                foreach (var item in TempList)
                {
                    var _RemovableData = namesList.Where(x => x.applicationno == item.applicationno).ToList();
                    if (_RemovableData != null && _RemovableData.Count > 0)
                    {
                        foreach (var Childitem in _RemovableData)
                        {
                            namesList.Remove(Childitem);
                        }
                    }
                }
                TempList.AddRange(namesList.Where(x => x.SortingOrder == 0).ToList());


                long _Applicationno = 0;
                foreach (var item in TempList)
                {
                    if (_Applicationno != item.applicationno)
                    {
                        FinalList.Add(item);
                        _Applicationno = item.applicationno;
                    }
                }
                if (FinalList != null && FinalList.Count > 0)
                {
                    FinalList = FinalList.OrderBy(x => x.Priority).ToList();
                }

                foreach (var item in FinalList)
                {
                    if (item.OutstandingAmount <= 0 && item.isdefaulter == false)
                        DbHelper.SelectMethod(String.Format(QueryHelper.UpdateCompleteDateApplication, item.applicationno));
                }
                return FinalList;
            }
            catch (Exception ex)
            {
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                throw ex;
            }
        }

       
        public double GetPaidPenalty(String ApplicationNo)
        {
            double Penalty = 0;
            try
            {
                var query = DbHelper.SelectMethod(String.Format(QueryHelper.GetPaidPenalty, ApplicationNo));
                if (query != null && query.Rows.Count > 0)
                {
                    Penalty = Convert.ToDouble(query.Rows[0]["penalty_amount"]);
                }
            }
            catch (Exception ex)
            {
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name));
                throw ex;
            }
            return Penalty;
        }

        public int GetCollectionAgentId(List<int> collectors)
        {
            Random random = new Random();
            var _UserId = random.Next(0, collectors.Count - 1);
            return collectors[_UserId];
            //if (collectors.Contains(_UserId))
            //{
            //    return _UserId;
            //}
            //else
            //{
            //    GetCollectionAgentId(collectors);
            //    return 0;
            //}
        }
    }

    public class DynamicLateFee
    {
        public decimal LateFee { get; set; } = 0;
        public decimal Penality { get; set; } = 0;
    }
}