using iTextSharp.text;
using iTextSharp.text.pdf;
using Loan_CRM.Models;
using Newtonsoft.Json;
using Npgsql;
using Serilog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Reflection;
using System.Web;

namespace Loan_CRM
{
    public static class CommonMethods
    {
        public static bool VerifyUserRole(int UserId, int RoleId, string RoleName)
        {
            try
            {
                DataTable dt = DbHelper.SelectMethod(string.Format(QueryHelper.ValidateUserId_Role, UserId, RoleId, RoleName), null);
                if (dt != null && dt.Rows.Count > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", $"CommonMethods ==> {MethodBase.GetCurrentMethod().Name}");
                return false;
            }
        }
        public static DataTable GettCredoScore(int UserId)
        {
            try
            {
                return DbHelper.SelectMethod(String.Format(QueryHelper.GetCredoScore, UserId));

            }
            catch (Exception ex)
            {
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", $"CommonMethods ==> {MethodBase.GetCurrentMethod().Name}");
                return null;
            }
        }
        public static List<City> getCity()
        {
            try
            {
                List<City> city = new List<City>();
                DataTable dt = DbHelper.SelectMethod(string.Format(QueryHelper.GetAllCityList), null);
                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        City obj = new City
                        {
                            cityid = Convert.ToInt32(row["cityid"]),
                            cityname = row["cityname"].ToString()
                        };
                        city.Add(obj);
                    }
                }
                return city;
            }
            catch (Exception ex)
            {
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", $"CommonMethods ==> {MethodBase.GetCurrentMethod().Name}");
                throw;
            }

        }
        /// <summary>
        /// getCityBy ProvinceID
        /// </summary>
        /// <param name="ProvinceID"></param>
        /// <returns></returns>
        public static List<City> getCityByID(int ProvinceID)
        {
            try
            {
                List<City> city = new List<City>();
                List<Parameters> parameters = new List<Parameters>();
                parameters.Add(new Parameters { ParameterName = "province_id", ParameterValue = ProvinceID, DbType = NpgsqlTypes.NpgsqlDbType.Integer });
                DataTable dt = DbHelper.SelectMethod(QueryHelper.GetAllCityListByID, parameters);
                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {

                        City obj = new City
                        {
                            cityid = Convert.ToInt32(row["cityid"]),
                            cityname = row["cityname"].ToString()
                        };
                        city.Add(obj);
                    }
                }
                return city;
            }
            catch (Exception ex)
            {
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", $"CommonMethods ==> {MethodBase.GetCurrentMethod().Name}"); throw;
            }

        }

        /// <summary>
        /// getCityByProvinceName
        /// </summary>
        /// <param name="ProvinceID"></param>
        /// <param name="Prefix"></param>
        /// <returns></returns>
        public static List<City> getCityByIDName(int ProvinceID, string Prefix)
        {
            try
            {
                List<City> city = new List<City>();
                List<Parameters> parameters = new List<Parameters>();
                parameters.Add(new Parameters { ParameterName = "province_id", ParameterValue = ProvinceID, DbType = NpgsqlTypes.NpgsqlDbType.Integer });
                parameters.Add(new Parameters { ParameterName = "cityname", ParameterValue = $"%{Prefix}%", DbType = NpgsqlTypes.NpgsqlDbType.Varchar });
                DataTable dt = DbHelper.SelectMethod(QueryHelper.GetAllCityListByIDName, parameters);
                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {

                        City obj = new City
                        {
                            cityid = Convert.ToInt32(row["cityid"]),
                            cityname = row["cityname"].ToString()
                        };
                        city.Add(obj);
                    }
                }
                return city;
            }
            catch (Exception ex)
            {
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", $"CommonMethods ==> {MethodBase.GetCurrentMethod().Name}"); throw;
            }

        }
        /// <summary>
        /// get Bank Name
        /// </summary>
        /// <returns></returns>
        public static List<BankMasterModel> getBankName()
        {
            try
            {
                List<BankMasterModel> bank = new List<BankMasterModel>();
                DataTable dt = DbHelper.SelectMethod(string.Format(QueryHelper.GetAllBankList));
                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {

                        BankMasterModel obj = new BankMasterModel
                        {
                            bank_id = Convert.ToInt32(row["bank_id"]),
                            bank_name = row["bank_name"].ToString()
                        };
                        bank.Add(obj);
                    }
                }
                return bank;
            }
            catch (Exception ex)
            {
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", $"CommonMethods ==> {MethodBase.GetCurrentMethod().Name}"); throw ex;
            }

        }
        /// <summary>
        /// getCivilStatus
        /// </summary>
        /// <returns></returns>
        public static List<CivilStatus> getCivilStatus()
        {
            try
            {
                List<CivilStatus> civilStatus = new List<CivilStatus>();
                DataTable dt = DbHelper.SelectMethod(QueryHelper.GetAllCivilStatus);
                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        CivilStatus obj = new CivilStatus
                        {
                            civilid = Convert.ToInt32(row["civilid"]),
                            civilname = row["civilname"].ToString()
                        };
                        civilStatus.Add(obj);
                    }
                }
                civilStatus.Insert(0, new CivilStatus { civilid = 0, civilname = "Select" });
                return civilStatus;
            }
            catch (Exception ex)
            {
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", $"CommonMethods ==> {MethodBase.GetCurrentMethod().Name}"); throw ex;
            }
        }
        /// <summary>
        /// getTermType
        /// </summary>
        /// <returns></returns>
        public static List<TermType> getTermType()
        {
            try
            {
                List<TermType> termtype = new List<TermType>();
                DataTable dt = DbHelper.SelectMethod(QueryHelper.GetAllTermType);
                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        TermType obj = new TermType
                        {
                            Id = Convert.ToInt32(row["id"]),
                            term_Type = row["termtype"].ToString()
                        };
                        termtype.Add(obj);
                    }
                }
                termtype.Insert(0, new TermType { Id = -1, term_Type = "Select" });
                return termtype;
            }
            catch (Exception ex)
            {
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", $"CommonMethods ==> {MethodBase.GetCurrentMethod().Name}"); throw ex;
            }

        }
        /// <summary>
        /// getOccupation
        /// </summary>
        /// <returns></returns>
        public static List<OccupationModel> getOccupation()
        {
            try
            {
                List<OccupationModel> OccupationList = new List<OccupationModel>();
                DataTable dt = DbHelper.SelectMethod(QueryHelper.GetOccupation_Active);
                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        OccupationModel obj = new OccupationModel
                        {
                            id = Convert.ToInt32(row["id"]),
                            occupation_name = row["occupation_name"].ToString(),
                            optionGroup = row["group_name"].ToString()
                        };
                        OccupationList.Add(obj);
                    }
                }
                OccupationList.Insert(0, new OccupationModel { id = -1, occupation_name = "Select" });
                return OccupationList;
            }
            catch (Exception ex)
            {
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", $"CommonMethods ==> {MethodBase.GetCurrentMethod().Name}");
                throw ex;
            }
        }
        /// <summary>
        /// Encryption of password
        /// </summary>
        /// <param name="sPassword"></param>
        /// <returns></returns>
        public static string md5(string sPassword)
        {
            System.Security.Cryptography.MD5CryptoServiceProvider x = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] bs = System.Text.Encoding.UTF8.GetBytes(sPassword);
            bs = x.ComputeHash(bs);
            System.Text.StringBuilder s = new System.Text.StringBuilder();
            foreach (byte b in bs)
            {
                s.Append(b.ToString("x2").ToLower());
            }
            return s.ToString();
        }

        /// <summary>
        /// SendMailWithAttachment
        /// </summary>
        /// <param name="_EmailFrom"></param>
        /// <param name="Mailto"></param>
        /// <param name="BodyText"></param>
        /// <param name="MsgSubject"></param>
        /// <param name="_attachment"></param>
        public static void SendMail(string _EmailFrom, string Mailto, string BodyText, string MsgSubject, string _attachment = "")
        {
            try
            {
                var Password = string.Empty;
                switch (_EmailFrom)
                {
                    case EmailFrom.FromPay:
                        Password = Convert.ToString(ConfigurationManager.AppSettings["FROMPWDpay"]);
                        break;
                    case EmailFrom.FromInfo:
                        Password = Convert.ToString(ConfigurationManager.AppSettings["FROMPWDinfo"]);
                        break;
                    case EmailFrom.FromAccount:
                        Password = Convert.ToString(ConfigurationManager.AppSettings["FROMPWDaccount"]);
                        break;
                    default:
                        break;
                }

                MailMessage mail = new MailMessage();
                mail.To.Add(Mailto);
                mail.From = new MailAddress(Convert.ToString(ConfigurationManager.AppSettings[_EmailFrom]));
                mail.Subject = MsgSubject;
                mail.Body = BodyText;
                mail.IsBodyHtml = true;
                if (!string.IsNullOrWhiteSpace(_attachment.Trim()))
                {
                    mail.Attachments.Add(new Attachment(_attachment));
                    mail.Priority = MailPriority.High;
                }

                SmtpClient smtp = new SmtpClient
                {
                    Host = Convert.ToString(ConfigurationManager.AppSettings["Host"]),
                    Port = Convert.ToInt32(ConfigurationManager.AppSettings["PORT"]),
                    UseDefaultCredentials = true,
                    Credentials = new System.Net.NetworkCredential(Convert.ToString(ConfigurationManager.AppSettings[_EmailFrom]), Password), // Enter seders User name and password       
                    EnableSsl = true
                };
                smtp.Send(mail);

            }
            catch (Exception ex)
            {
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", $"CommonMethods ==> {MethodBase.GetCurrentMethod().Name}");
            }
        }
        /// <summary>
        /// To Create LOD PDF
        /// </summary>
        /// <param name="AppId"></param>
        /// <param name="Name"></param>
        /// <param name="ContractNum"></param>
        /// <param name="Date"></param>
        /// <param name="OutStanding"></param>
        /// <param name="LateFee"></param>
        /// <param name="Penality"></param>
        /// <param name="TotalAmount"></param>
        /// <param name="server"></param>
        /// <returns></returns>
        public static string CreateLOD_PDF(int AppId, string Name, string ContractNum, string Date, decimal OutStanding, decimal LateFee, decimal Penality, decimal TotalAmount, string emidate, HttpServerUtilityBase server)
        {
            string path = string.Empty;
            try
            {
                var folderName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Content", "LOD");
                Directory.CreateDirectory(folderName);
                path = Path.Combine(folderName, $"LOD_{AppId}_{DateTime.Now:yyyyMMddHHmmss}.pdf");
                FileStream fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
                Rectangle rec = new Rectangle(PageSize.A4);
                Document doc = new Document(rec, 20, 30, 40, 40);

                using (PdfWriter writer = PdfWriter.GetInstance(doc, fs))
                {
                    doc.Open();
                    var __break = new Paragraph("\n");
                    ////For Image
                    string imageURL = HttpContext.Current.Server.MapPath("../../Content/images/CashMartLogo.png");
                    Image jpg = Image.GetInstance(imageURL);
                    //Resize image depend upon your need
                    jpg.ScaleToFit(150f, 150f);
                    jpg.XYRatio = 2f;
                    jpg.Alignment = Element.ALIGN_LEFT;
                    doc.Add(jpg);

                    //For Date
                    doc.Add(__break);
                    var p1 = new Paragraph("Date: " + DateTime.Now.ToShortDateString())
                    {
                        Alignment = Element.ALIGN_LEFT,
                        IndentationLeft = 40f,
                        IndentationRight = 40f
                    };
                    doc.Add(p1);

                    //For Attention
                    doc.Add(__break);
                    var p2 = new Paragraph("Attention: " + Name)
                    {
                        Alignment = Element.ALIGN_LEFT,
                        IndentationLeft = 40f,
                        IndentationRight = 40f
                    };
                    doc.Add(p2);

                    doc.Add(__break);
                    var p3 = new Paragraph("NOTICE OF DEMAND")
                    {
                        Alignment = Element.ALIGN_CENTER,
                        IndentationLeft = 40f,
                        IndentationRight = 40f
                    };
                    p3.Font.SetStyle("bold");
                    p3.Font.Size = 18;
                    doc.Add(p3);

                    doc.Add(__break);

                    var p4 = new Paragraph($"Our records show that your loan obligation in the amount of {TotalAmount} fall past due last {emidate} and requires immediate attention.")
                    {
                        Alignment = Element.ALIGN_LEFT,
                        IndentationLeft = 40f,
                        IndentationRight = 40f
                    };
                    doc.Add(p4);

                    doc.Add(__break);
                    var p5 = new Paragraph("Loan Details:")
                    {
                        Alignment = Element.ALIGN_LEFT
                    };
                    p5.Font.SetStyle("bold");
                    p5.IndentationLeft = 40f;
                    p5.IndentationRight = 40f;
                    doc.Add(p5);

                    doc.Add(__break);
                    PdfPTable table = new PdfPTable(6);

                    Phrase phrase1 = new Phrase
                    {
                        new Chunk("Contract Number", FontFactory.GetFont("Arial", 11, Font.BOLD))
                    };
                    PdfPCell cell1 = new PdfPCell(phrase1);
                    table.AddCell(cell1);

                    Phrase phrase2 = new Phrase
                    {
                        new Chunk("Date Of Loan", FontFactory.GetFont("Arial", 11, Font.BOLD))
                    };
                    PdfPCell cell2 = new PdfPCell(phrase2);
                    table.AddCell(cell2);

                    Phrase phrase3 = new Phrase
                    {
                        new Chunk("Outstanding Balance", FontFactory.GetFont("Arial", 11, Font.BOLD))
                    };
                    PdfPCell cell3 = new PdfPCell(phrase3);
                    table.AddCell(cell3);

                    Phrase phrase4 = new Phrase
                    {
                        new Chunk("Late Fee", FontFactory.GetFont("Arial", 11, Font.BOLD))
                    };
                    PdfPCell cell4 = new PdfPCell(phrase4);
                    table.AddCell(cell4);

                    Phrase phrase5 = new Phrase
                    {
                        new Chunk("Penalty (1% Per Day)", FontFactory.GetFont("Arial", 11, Font.BOLD))
                    };
                    PdfPCell cell5 = new PdfPCell(phrase5);
                    table.AddCell(cell5);

                    Phrase phrase6 = new Phrase
                    {
                        new Chunk("Total Amount Due", FontFactory.GetFont("Arial", 11, Font.BOLD))
                    };
                    PdfPCell cell6 = new PdfPCell(phrase6);
                    table.AddCell(cell6);


                    table.AddCell(ContractNum);
                    table.AddCell(Date);
                    table.AddCell(Convert.ToString(Math.Round(OutStanding, 2)));
                    if (LateFee > 0)
                    {
                        table.AddCell(Convert.ToString(Math.Round(LateFee, 2)));
                    }
                    else
                    {
                        LateFee = 0;
                        table.AddCell(Convert.ToString("0.00"));
                    }

                    if (Penality > 0)
                    {
                        table.AddCell(Convert.ToString(Math.Round(Penality, 2)));
                    }
                    else
                    {
                        Penality = 0;
                        table.AddCell(Convert.ToString("0.00"));
                    }



                    table.AddCell(Convert.ToString(Math.Round((TotalAmount), 2)));

                    table.TotalWidth = 450f;
                    table.LockedWidth = true;
                    doc.Add(table);

                    doc.Add(__break);
                    var p6 = new Paragraph("These are charges that you have incurred and details of which you are fully aware.We have repeatedly requested for payment and you have not shown the slightest sincerity to come forward in settling this debt.")
                    {
                        Alignment = Element.ALIGN_LEFT,
                        IndentationLeft = 40f,
                        IndentationRight = 40f
                    };
                    doc.Add(p6);

                    doc.Add(__break);
                    var p7 = new Paragraph("We reserve the right to fully commence our debt recovery actions to demand payment of the above amount at your place of residence or place of work without further notice to you.")
                    {
                        Alignment = Element.ALIGN_LEFT,
                        IndentationLeft = 40f,
                        IndentationRight = 40f
                    };
                    doc.Add(p7);

                    doc.Add(__break);
                    var p8 = new Paragraph("TAKE NOTE that unless your outstanding loan and default interest are fully paid through Cash Mart payment partners. Failure to pay, we will exercise whatever rights and remedies under the law to enforce such payment.")
                    {
                        Alignment = Element.ALIGN_LEFT,
                        IndentationLeft = 40f,
                        IndentationRight = 40f
                    };
                    doc.Add(p8);

                    doc.Add(__break);

                    var p9 = new Paragraph("For any queries please don’t hesitate to call our office at (02) 829 0000 ")
                    {
                        Alignment = Element.ALIGN_LEFT,
                        IndentationLeft = 40f,
                        IndentationRight = 40f
                    };
                    doc.Add(p9);


                    doc.Add(__break);
                    doc.Add(__break);

                    var p10 = new Paragraph("This is a computer generated letter, signature is not required")
                    {
                        Alignment = Element.ALIGN_LEFT,
                        IndentationLeft = 40f,
                        IndentationRight = 40f
                    };
                    doc.Add(p10);

                    doc.Add(__break);
                    var p11 = new Paragraph("Please disregard this notice if payment had been made")
                    {
                        Alignment = Element.ALIGN_LEFT,
                        IndentationLeft = 40f,
                        IndentationRight = 40f
                    };
                    p11.Font.SetStyle("underline");
                    p11.Font.SetStyle("bold");
                    p11.Font.SetStyle("italic");
                    doc.Add(p11);

                    doc.Add(__break);
                    var line = new Paragraph("_________________________________________________________________________________");
                    doc.Add(line);
                    var p12 = new Paragraph("Unit 2-C, 53 Bayani Road, Fort Bonifocio,Taguig City")
                    {
                        Alignment = Element.ALIGN_CENTER
                    };
                    p12.Font.SetStyle("bold");

                    doc.Add(p12);

                    var p13 = new Paragraph("T: (02)8829 0000 E: pay@cashmart.ph  W:www.cashmart.ph")
                    {
                        Alignment = Element.ALIGN_CENTER
                    };
                    doc.Add(p13);
                    doc.Add(line);

                    doc.Close();
                }
            }
            catch (Exception ex)
            {
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", $"CommonMethods ==> {MethodBase.GetCurrentMethod().Name}");
                throw ex;
            }

            return path;
        }
        /// <summary>
        /// To Create PDF 
        /// </summary>
        /// <param name="str"></param>
        /// <param name="name"></param>
        /// <param name="address"></param>
        /// <param name="approvedloanamount"></param>
        /// <param name="days"></param>
        /// <param name="intrestrate"></param>
        /// <param name="purposeofloan"></param>
        /// <param name="_pdfname"></param>

        public static void CreateCreditAgreementPdf(string str, string name, string address, double approvedloanamount, int days, double intrestrate, string purposeofloan, string _pdfname, int userId, string term_name, int term_id, int AppNo, double Penalty_Rate, double Admin_Fee, int _Tenure, double _Late_fee, string bankName, string bankAccountNo)
        {
            try
            {

                var termno = term_name == "Weekly" ? 1 : term_name == "Bi-Weekly" ? 2 : 3;
                var emidetails = GetCustomer_Loan_Calcs(userId, new EMICalculatorModel
                {
                    Amount = Convert.ToDecimal(approvedloanamount),
                    LoanDate = DateTime.Now,
                    LoanType = termno,
                    tenure = term_id,
                    Interest = intrestrate
                }); ;

                var deduction = GetPretermDeductionAmount(userId);
                string LifeTimeRrferenceNo = GetReferenceNo(userId);
                string ApplicantName = name != string.Empty ? name : "________________________________";
                string ApplicantAddress = address != string.Empty ? address : "________________________________";
                string ApprovedAmount = Convert.ToString(NumberToWords(Convert.ToInt32(approvedloanamount)));
                string ApprovedLoanAmount = Convert.ToString(approvedloanamount) != string.Empty ? Convert.ToString(approvedloanamount) : "________________________________";
                string DaysTerm = Convert.ToString(days) != string.Empty ? Convert.ToString(days) + "" + " Days" : "______________";
                string[] arr = Convert.ToString(intrestrate).Split('.');
                string InterestRate = String.Empty;
                if (arr.Length <= 1)
                {
                    InterestRate = Convert.ToString(NumberToWords(Convert.ToInt32(arr[0])));
                }
                if (arr.Length > 1)
                {
                    InterestRate = Convert.ToString(NumberToWords(Convert.ToInt32(arr[0]))) + " point " + Convert.ToString(NumberToWords(Convert.ToInt32(arr[1])));

                }
                InterestRate = DecimalToWordConverter.ConvertDecimalToWords(Convert.ToDecimal(intrestrate));
                string InterestRateValue = Convert.ToString(intrestrate) != string.Empty ? Convert.ToString(intrestrate) : "______________";
                string PurposeOfLoan = purposeofloan != string.Empty ? purposeofloan : "________________________________";
                string ShortDate = DateTime.Now.ToShortDateString();
                string ShortTime = DateTime.Now.ToShortTimeString();


                string pdfname = _pdfname;
                string path = Path.Combine(HttpContext.Current.Server.MapPath("../../Content/PDF/"), pdfname + ".pdf");
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
                using (FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    Rectangle rec = new Rectangle(PageSize.A4);
                    Document doc = new Document(rec, 40, 40, 40, 70);
                    doc.AddTitle("Cashmart Contract");
                    doc.AddSubject("Contract against loan");
                    doc.AddCreator("CASHMART");
                    doc.AddAuthor(str);
                    var __break = new Paragraph("\n");
                    using (PdfWriter writer = PdfWriter.GetInstance(doc, fs))
                    {
                        writer.PageEvent = new ITextEvents();
                        doc.Open();


                        var _p1 = new Paragraph("DISCLOSURE STATEMENT ON LOAN/CREDIT TRANSACTION")
                        {
                            Alignment = Element.ALIGN_CENTER
                        };
                        _p1.Font.SetStyle("underline");
                        _p1.Font.SetStyle("bold");
                        doc.Add(_p1);

                        _p1 = new Paragraph("(As Required under R.A. 3765, Truth of Lending Act)")
                        {
                            Alignment = Element.ALIGN_CENTER
                        };
                        _p1.Font.SetStyle("bold");
                        doc.Add(_p1);

                        _p1 = new Paragraph($"Name of Borrower : {ApplicantName}")
                        {
                            Alignment = Element.ALIGN_LEFT
                        };
                        doc.Add(_p1);


                        _p1 = new Paragraph($"Application No : {AppNo}")
                        {
                            Alignment = Element.ALIGN_LEFT
                        };
                        doc.Add(_p1);

                        _p1 = new Paragraph($"Address: {ApplicantAddress}")
                        {
                            Alignment = Element.ALIGN_LEFT
                        };
                        doc.Add(_p1);

                        _p1 = new Paragraph($"Payment Reference Number:{LifeTimeRrferenceNo}")
                        {
                            Alignment = Element.ALIGN_LEFT
                        };
                        doc.Add(_p1);

                        _p1 = new Paragraph($"Approved Loan Amount: {Math.Round(Convert.ToDecimal(ApprovedLoanAmount), 2)}")
                        {
                            Alignment = Element.ALIGN_LEFT
                        };
                        doc.Add(_p1);

                        _p1 = new Paragraph($"Processing Fee:{CommonMethods.NumberToWords(Admin_Fee)} percent ({Math.Round(Admin_Fee, 2)}%)")
                        {
                            Alignment = Element.ALIGN_LEFT
                        };
                        doc.Add(_p1);

                        _p1 = new Paragraph($"Previous Loan Balance: {Math.Round(deduction, 2)}")
                        {
                            Alignment = Element.ALIGN_LEFT
                        };
                        doc.Add(_p1);

                        _p1 = new Paragraph($"TOTAL DEDUCTIONS: {Math.Round((((Convert.ToDecimal(ApprovedLoanAmount) * Convert.ToDecimal(Admin_Fee)) / 100) + (Convert.ToDecimal(deduction))), 2)}")
                        {
                            Alignment = Element.ALIGN_LEFT
                        };
                        doc.Add(_p1);

                        _p1 = new Paragraph($"Net Proceeds: {Math.Round((Convert.ToDecimal(ApprovedLoanAmount) - (((Convert.ToDecimal(ApprovedLoanAmount) * Convert.ToDecimal(Admin_Fee)) / 100) + (Convert.ToDecimal(deduction)))), 2)}")
                        {
                            Alignment = Element.ALIGN_LEFT
                        };
                        doc.Add(_p1);

                        _p1 = new Paragraph($"Applied Interest Rate For Your Loan: {InterestRateValue}")
                        {
                            Alignment = Element.ALIGN_LEFT
                        };
                        doc.Add(_p1);

                        _p1 = new Paragraph($"Annual Effective Interest Rate: {Math.Round(((Convert.ToDecimal(InterestRateValue) / term_id) * 365), 2)}")
                        {
                            Alignment = Element.ALIGN_LEFT
                        };
                        doc.Add(_p1);

                        _p1 = new Paragraph($"Total Number of Instalment: {emidetails.Count()}")
                        {
                            Alignment = Element.ALIGN_LEFT
                        };
                        doc.Add(_p1);

                        _p1 = new Paragraph($"Payment Due Date:")
                        {
                            Alignment = Element.ALIGN_CENTER
                        };
                        _p1.Font.SetStyle("underline");
                        _p1.Font.SetStyle("bold");
                        doc.Add(_p1);
                        doc.Add(__break);

                        int _LeftLine = 10;

                        Decimal _totalBalance = 0;
                        foreach (var item in emidetails)
                        {
                            _p1 = new Paragraph($"Due Date  : {item.EmiDate.ToString("MM/dd/yyyy")}:                     Php : {Math.Round(item.emi_amount, 2)}")
                            {
                                Alignment = Element.ALIGN_LEFT
                            };
                            doc.Add(_p1);
                            _LeftLine--;
                            _totalBalance += item.emi_amount;
                        }


                        for (int i = 0; i < _LeftLine; i++)
                        {
                            doc.Add(__break);
                        }
                        _p1 = new Paragraph($"TOTAL OUTSTANDING BALANCE:   Php : {Math.Round(_totalBalance, 2)}")
                        {
                            Alignment = Element.ALIGN_LEFT
                        };
                        doc.Add(_p1);

                        doc.Add(__break);

                        _p1 = new Paragraph($"Note: Due date will adjust accordingly to the date of disbursement. Should there be any changes, you will immediately receive an updated schedule of payment in your email.")
                        {
                            Alignment = Element.ALIGN_LEFT
                        };
                        _p1.Font.SetStyle("underline");
                        _p1.Font.SetStyle("bold");
                        doc.Add(_p1);

                        _p1 = new Paragraph($"ADDITIONAL CHARGES IN CASE CERTAIN STIPULATIONS ARE NOT MET BY THE BORROWER")
                        {
                            Alignment = Element.ALIGN_CENTER
                        };
                        _p1.Font.SetStyle("underline");
                        _p1.Font.SetStyle("bold");
                        doc.Add(_p1);
                        doc.Add(__break);

                        _p1 = new Paragraph($"Late Fee: Php {_Late_fee} for the first missed due date and every {_Tenure} days thereafter up to the settlement of the account.")
                        {
                            Alignment = Element.ALIGN_LEFT
                        };
                        doc.Add(_p1);
                        doc.Add(__break);

                        _p1 = new Paragraph($"Penalty Charge: {Penalty_Rate}% Daily Penalty Based on The Outstanding Balance")
                        {
                            Alignment = Element.ALIGN_LEFT
                        };
                        doc.Add(_p1);
                        doc.Add(__break);

                        _p1 = new Paragraph("Please verify the information below:")
                        {
                            Alignment = Element.ALIGN_LEFT
                        };
                        doc.Add(_p1);

                        _p1 = new Paragraph($"Bank Name: {bankName}")
                        {
                            Alignment = Element.ALIGN_LEFT
                        };
                        doc.Add(_p1);

                        _p1 = new Paragraph($"Account Holder Name: {ApplicantName}")
                        {
                            Alignment = Element.ALIGN_LEFT
                        };
                        doc.Add(_p1);

                        _p1 = new Paragraph($"Bank Account Number: {bankAccountNo}")
                        {
                            Alignment = Element.ALIGN_LEFT
                        };
                        doc.Add(_p1);

                        _p1 = new Paragraph("I hereby confirm that the above bank details are accurate, and I authorize Cash Mart Asia Lending Inc to utilize this information for processing financial transactions related to our business relationship.”")
                        {
                            Alignment = Element.ALIGN_LEFT
                        };
                        doc.Add(_p1);
                        doc.Add(__break);

                        _p1 = new Paragraph($"I ACKNOWLEDGE RECEIPT OF A COPY OF THIS STATEMENT PRIOR TO THE CONSUMMATION OF THE CREDIT TRANSACTION AND THAT I UNDERSTAND AND FULLY AGREE TO THE TERMS AND CONDITIONS THEREOF.\n\n\n")
                        {
                            Alignment = Element.ALIGN_LEFT
                        };
                        _p1.Font.SetStyle("bold");
                        doc.Add(_p1);
                        doc.Add(__break);

                        var p1 = new Paragraph("CREDIT AGREEMENT\nPARTIES")
                        {
                            Alignment = Element.ALIGN_CENTER
                        };
                        p1.Font.SetStyle("underline");
                        p1.Font.SetStyle("bold");
                        doc.Add(p1);

                        var p1_1 = new Paragraph(pdfname)
                        {
                            Alignment = Element.ALIGN_RIGHT
                        };
                        p1_1.Font.SetStyle("underline");
                        p1_1.Font.SetStyle("bold");
                        p1_1.Font.SetColor(255, 0, 0);
                        doc.Add(p1_1);

                        var p2 = new Paragraph("This Credit Agreement is made between Cash Mart Asia Lending Inc.(hereinafter called the 'Lender') with its office situated at 53 Bayani Road unit 2C, Fort Bonifacio, Taguig City, Metro Manila, Philippines 1630 and " + ApplicantName + " with mailing address at " + ApplicantAddress + " (hereinafter called the 'Borrower'). The parties hereby agree to be bound by the following stipulations: ")
                        {
                            Alignment = Element.ALIGN_JUSTIFIED,
                            IndentationLeft = 40f,
                            IndentationRight = 40f
                        };
                        doc.Add(p2);

                        var p3 = new Paragraph("1. THE LOAN")
                        {
                            Alignment = Element.ALIGN_LEFT
                        };
                        p3.Font.SetStyle("bold");
                        doc.Add(__break);
                        doc.Add(p3);
                        var p4 = new Paragraph("At the request of the Borrower, the Lender extends the Loan to the Borrower in the principal sum of  " + ApprovedAmount + "  Pesos(PhP " + ApprovedLoanAmount + " ), (the “Loan”), the amount of which is acknowledged to have been received by the Borrower.The Borrower, in turn, promises to repay the Loan to the Lender within " + DaysTerm + " from the date hereof(the “Loan Term”) with an interest rate of " + InterestRate + " percent ( " + InterestRateValue + " %) for the duration of the Loan Term.")
                        {
                            Alignment = Element.ALIGN_LEFT
                        };
                        doc.Add(p4);

                        var p5 = new Paragraph("2. PURPOSE OF THE LOAN ")
                        {
                            Alignment = Element.ALIGN_LEFT
                        };
                        p5.Font.SetStyle("bold");
                        doc.Add(__break);
                        doc.Add(p5);
                        var p6 = new Paragraph("The amount granted as Loan is to be utilized by the Borrower for legitimate purposes, specifically for " + PurposeOfLoan + ".")
                        {
                            Alignment = Element.ALIGN_LEFT
                        };
                        doc.Add(p6);

                        var p7 = new Paragraph("3. PROCESSING FEE")
                        {
                            Alignment = Element.ALIGN_LEFT
                        };
                        p7.Font.SetStyle("bold");
                        doc.Add(__break);
                        doc.Add(p7);
                        var p8 = new Paragraph($"The Borrower shall pay to the Lender {DecimalToWordConverter.ConvertDecimalToWords(Convert.ToDecimal(Admin_Fee))} percent ({Admin_Fee} %) of the principal amount of the Loan, which shall be paid upfront before disbursement.")
                        {
                            Alignment = Element.ALIGN_LEFT
                        };
                        doc.Add(p8);

                        var p9 = new Paragraph("4. REPAYMENT")
                        {
                            Alignment = Element.ALIGN_LEFT
                        };
                        p9.Font.SetStyle("bold");
                        doc.Add(__break);
                        doc.Add(p9);
                        var p10 = new Paragraph("a. The Borrower undertakes to repay the outstanding Loan on or before the end of the term as specified in Section 1 of this Agreement.")
                        {
                            Alignment = Element.ALIGN_LEFT
                        };
                        doc.Add(p10);
                        var p10_1 = new Paragraph("b. The repayment schedule shall be furnished upon the final approval and after disbursement to the Borrower of the principal amount applied for and shall follow the approved Loan Term accordingly.The said schedule shall be part of this Agreement and will be attached as Annex A - Repayment Schedule.")
                        {
                            Alignment = Element.ALIGN_LEFT
                        };
                        doc.Add(p10_1);
                        var p10_2 = new Paragraph($"c. If the Borrower fails to pay any scheduled amortization during the term, there shall be charged against him/her, a penalty of {DecimalToWordConverter.ConvertDecimalToWords(Convert.ToDecimal(Penalty_Rate))} percent ({Penalty_Rate}%) late interest per day upon the UNPAID OUTSTANDING BALANCE plus late payment charges of Php {_Late_fee} every {_Tenure} days as stated in Annex A Repayment Schedule. Also, the payment of the outstanding balance shall then be demandable without need of prior notice.")
                        {
                            Alignment = Element.ALIGN_LEFT
                        };
                        doc.Add(p10_2);
                        var p10_3 = new Paragraph("d. The Borrower shall be required to pay in cash at the payment service providers which are in partnership with the Lender or through any payment facilities as directed or accepted by the Lender.")
                        {
                            Alignment = Element.ALIGN_LEFT
                        };
                        doc.Add(p10_3);

                        var p10_4 = new Paragraph("e. Consistent with Article 113 of the Labor Code and Labor Advisory No. 11, series of 2014, the Borrower consents to and authorizes his employer(if employed) to deduct a portion of his salary in order to meet the scheduled loan repayment until such time that the whole Loan Term is met and fulfilled in accordance to Annex A -Repayment Schedule. in favor of the Lender. The consent can be in the form of a signed Certificate Of Employment (COE)wherein the employer should acknowledge and state in a clear manner the purpose of issuance to the employee.")
                        {
                            Alignment = Element.ALIGN_LEFT
                        };
                        doc.Add(p10_4);
                        var p11 = new Paragraph("5. REPRESENTATIONS AND UNDERTAKINGS");
                        p9.Alignment = Element.ALIGN_LEFT;
                        p9.Font.SetStyle("bold");
                        doc.Add(__break);
                        doc.Add(p11);
                        var p12 = new Paragraph("a. The Borrower hereby represents and warrants:");
                        var p13 = new Paragraph("i. The Borrower, and where the Borrower is a juridical person, each officer and director thereof, is not bankrupt, of unsound mind or incapable of managing his / her own affairs and where the Borrower is a corporation, it is duly constituted and validly existing and has the power to own property and assets and to carry on its business as presently conducted;\n ii. The Borrower, or where the Borrower is a juridical person, the authorized representative thereof for purposes of entering into contract, voluntarily entered into and executed herein Agreement; \niii. If the Borrower is a juridical person, it undertakes that its representative is duly authorized to enter into and perform the obligations under this Agreement and that all necessary action has been taken to authorize the creation, delivery and performance of this Agreement, and that entering into this Agreement will not constitute a breach of any restrictions applicable to the Borrower. \n iv. The Borrower will use the loan only according to the purpose given in this Agreement; \n v. The entry into and performance by the Borrower of the obligation contemplated by this Agreement do not and shall not conflict with: (i)any existing law or regulation applicable to the Borrower; or(ii) any agreement or instrument binding upon the Borrower; and \n vi. The Borrower has informed the Lender of all required information which are material and relevant to the approval of the Loan which ought to be disclosed to the Lender in view of the provisions of this Agreement.The Borrower warrants that said information are true, complete, and accurate at the date they were disclosed and are not misleading in any respect.");
                        var p14 = new Paragraph("b. The Borrower covenants with the Lender that during the Loan Term:\ni. The Borrower, and where the Borrower is a juridical person, each officer and director thereof, is not bankrupt, of unsound mind or incapable of managing his / her own affairs and where the Borrower is a corporation, it is duly constituted and validly existing and has the power to own property and assets and to carry on its business as presently conducted;");
                        var p15 = new Paragraph("\n ii. The Borrower, or where the Borrower is a juridical person, the authorized representative thereof for purposes of entering into contract, voluntarily entered into and executed herein Agreement; \n iii. If the Borrower is a juridical person, it undertakes that its representative is duly authorized to enter into and perform the obligations under this Agreement and that all necessary action has been taken to authorize the creation, delivery and performance of this Agreement, and that entering into this Agreement will not constitute a breach of any restrictions applicable to the Borrower. \n iv. The Borrower will use the loan only according to the purpose given in this Agreement; \n v. The entry into and performance by the Borrower of the obligation contemplated by this Agreement do not and shall not conflict with: (i)any existing law or regulation applicable to the Borrower; or(ii) any agreement or instrument binding upon the Borrower; and \n vi. The Borrower has informed the Lender of all required information which are material and relevant to the approval of the Loan which ought to be disclosed to the Lender in view of the provisions of this Agreement.The Borrower warrants that said information are true, complete, and accurate at the date they were disclosed and are not misleading in any respect. \n iv. The Borrower shall provide a prior written notice to the Lender of any change in the Borrower’s name, residential / office address, or any material information indicated in the Loan Application within three(3) calendar days of such change; \n v. The Borrower or his representative shall notify the Lender of any change in the Borrower’s employment, business or profession.In the event the Borrower is self-employed or is a juridical person, the Borrower or the authorized representative, as the case may be, hereby undertakes to keep the Lender informed about the financial situation of the business as may be requested by the Lender. ");
                        p12.Alignment = Element.ALIGN_LEFT;
                        p13.Alignment = Element.ALIGN_LEFT;
                        p14.Alignment = Element.ALIGN_LEFT;
                        p15.Alignment = Element.ALIGN_LEFT;
                        doc.Add(p12);
                        doc.Add(p13);
                        doc.Add(__break);
                        doc.Add(p14);
                        doc.Add(p15);

                        var p16 = new Paragraph("6. EVENT OF DEFAULT")
                        {
                            Alignment = Element.ALIGN_LEFT
                        };
                        p16.Font.SetStyle("bold");
                        doc.Add(__break);
                        doc.Add(p16);
                        var p17 = new Paragraph("The Loan, its interest and penalties shall be immediately due and payable upon the occurrence of one or more of the following: ");
                        var p18 = new Paragraph("a. The Borrower uses the Loan for a purpose other than stated in this Agreement; \n b. The Borrower fails to comply with any provision of this Agreement. This includes any failure to pay, when due, the principal, interest, and / or any fee payable; \n c. The Borrower is discovered to have concealed material information in the loan application or made false or misleading representation / s or statement / s in the course of securing the approval of this Agreement; \n d. Failure of the Borrower to settle any indebtedness in respect of monies borrowed from the Lender, even for a different transaction; \n e. Failure of the Borrower to pay tax obligations or duties on their due date. ");
                        var p19 = new Paragraph("Upon the occurrence of an Event of Default, the Lender is entitled to:");
                        var p20 = new Paragraph("a. Notify the Borrower in writing, declaring any and all amounts outstanding under this Agreement, including all accrued interest and other sums payable in respect thereto, as well as any other indebtedness of the Borrower in favor of the Lender to be immediately due and payable; \n b. Initiate arbitration proceedings as provided under Section 15 hereof. \n c. Exercise all of its rights and remedies under this Agreement and other Transaction Documents, or existing law to protect is interest as creditor against the Borrower or his surety(ies) and / or guarantor(s), when applicable.");
                        p17.Alignment = Element.ALIGN_LEFT;
                        p18.Alignment = Element.ALIGN_LEFT;
                        p19.Alignment = Element.ALIGN_LEFT;
                        p20.Alignment = Element.ALIGN_LEFT;
                        doc.Add(p17);
                        doc.Add(p18);
                        doc.Add(p19);
                        doc.Add(p20);

                        var p21 = new Paragraph("7. INDEMNITY")
                        {
                            Alignment = Element.ALIGN_LEFT
                        };
                        p21.Font.SetStyle("bold");
                        doc.Add(__break);
                        doc.Add(p21);
                        var p22 = new Paragraph("The Borrower shall indemnify the Lender any cost, loss or liability incurred by the Lender arising from arbitration or similar proceedings with respect to this Agreement due to the fault of the Borrower.The Borrower agrees to indemnify and hold free and harmless and defend the Lender, its affiliates, representatives, assigns, officers, directors, employees or agents, from and against any and all claims, actions, suits, demands, damages, losses, liabilities, settlements and costs and expenses whatsoever which Lender may incur by reason of or in connection with(a) the execution, delivery, administration or enforcement of this Agreement, or(b) the execution and delivery or transfer of, or payment or failure to pay under, this Agreement.")
                        {
                            Alignment = Element.ALIGN_LEFT
                        };
                        doc.Add(p22);

                        var p23 = new Paragraph("8. PRIVACY OF INFORMATION")
                        {
                            Alignment = Element.ALIGN_LEFT
                        };
                        p23.Font.SetStyle("bold");
                        doc.Add(__break);
                        doc.Add(p23);
                        var p24 = new Paragraph("The Lender shall respect the privacy of the Borrower’s personal information obtained in relation to this Agreement.For this purpose, the Lender shall implement reasonable and appropriate organizational, physical and technical measures intended for the protection of the Borrower’s personal information.Notwithstanding the foregoing, the Borrower hereby expressly permits the Lender to disclose to(i) the government authorities; (ii)any party / person proposing to tender any payment towards or purchase the indebtedness under the Loan; (iii)its auditors, lawyers or any other debt collection agents; (v)credit reporting agencies; (vi)insurance companies, agents, contractors or third party service providers who are involved in the provision of products and services to or by the Lender and the holding company, head office, other branches, subsidiaries, related companies of the Lender, any information relating to the Borrower’s account in respect this Agreement to such extent as the Lender may reasonably deem necessary.The Borrower hereby agrees that the aforesaid information may be used, encrypted, transmitted, stored and published, either digitally or in print, by the Lender and its holding company, head office, other branches, subsidiaries, related companies, including its digital platform and/ or company circular, and/ or may be exchanged to or with such persons enumerated above as the Lender considers necessary to the extent as permitted by Philippine laws.The aforesaid actions are without liability to the Lender.The Borrower expressly consents to such actions and declares that no further consent from the Borrower is necessary or required in relation thereto.")
                        {
                            Alignment = Element.ALIGN_LEFT
                        };
                        doc.Add(p24);


                        var p25 = new Paragraph("9. NOTICES")
                        {
                            Alignment = Element.ALIGN_LEFT
                        };
                        p25.Font.SetStyle("bold");
                        doc.Add(p25);
                        var p26 = new Paragraph("Notices given to the parties in connection with this Agreement may be delivered by hand, by courier, electronic messaging(e - mail), facsimile to the facsimile number of the other party, or sent via SMS to the mobile number as disclosed.")
                        {
                            Alignment = Element.ALIGN_LEFT
                        };
                        doc.Add(p26);

                        var p27 = new Paragraph("10. ASSIGNMENT")
                        {
                            Alignment = Element.ALIGN_LEFT
                        };
                        p27.Font.SetStyle("bold");
                        doc.Add(__break);
                        doc.Add(p27);
                        var p28 = new Paragraph("Notwithstanding any other provisions of this Agreement, the Lender may at any time: (i) assign its rights and obligations pertaining to the Loan; or(ii) create a security in or over any or all of its rights or obligations under this Agreement to any person provided that the Lender shall notify the Borrower seven(7) business days prior thereto. The Borrower is not permitted to assign or transfer any of the Borrower’s rights or obligations under this Agreement without the prior notification to and written consent of the Lender.")
                        {
                            Alignment = Element.ALIGN_LEFT
                        };
                        doc.Add(p28);

                        var p29 = new Paragraph("11. WAIVER")
                        {
                            Alignment = Element.ALIGN_LEFT
                        };
                        p29.Font.SetStyle("bold");
                        doc.Add(__break);
                        doc.Add(p29);
                        var p30 = new Paragraph("The delay or failure of the Lender to exercise of any of its rights in this Agreement shall not be construed or deemed as a waiver of the Lender to the exercise of such rights.")
                        {
                            Alignment = Element.ALIGN_LEFT
                        };
                        doc.Add(p30);

                        var p31 = new Paragraph("12. GOVERNING LAW")
                        {
                            Alignment = Element.ALIGN_LEFT
                        };
                        p31.Font.SetStyle("bold");
                        doc.Add(__break);
                        doc.Add(p31);
                        var p32 = new Paragraph("This Agreement is governed by the laws of the Philippines.")
                        {
                            Alignment = Element.ALIGN_LEFT
                        };
                        doc.Add(p32);

                        var p33 = new Paragraph("13. ARBITRATION CLAUSE")
                        {
                            Alignment = Element.ALIGN_LEFT
                        };
                        p33.Font.SetStyle("bold");
                        doc.Add(__break);
                        doc.Add(p33);
                        var p34 = new Paragraph("Any dispute, controversy or claim arising out of or relating to this contract, or the breach, termination or invalidity thereof shall be settled by arbitration in accordance with the Philippine Dispute Resolution Center Arbitration Rules in force at the time of the commencement of the arbitration.")
                        {
                            Alignment = Element.ALIGN_LEFT
                        };
                        doc.Add(p34);

                        var p35 = new Paragraph("14. SURETY AND GUARANTEE")
                        {
                            Alignment = Element.ALIGN_LEFT
                        };
                        p35.Font.SetStyle("bold");
                        doc.Add(__break);
                        doc.Add(p35);
                        var p36 = new Paragraph("The Borrower’s surety(ies) or guarantor(s), if any, will be subjected to the terms on payment upon the occurrence of one or more events of default.")
                        {
                            Alignment = Element.ALIGN_LEFT
                        };
                        doc.Add(p36);

                        var p37 = new Paragraph("15. SEVERABILITY")
                        {
                            Alignment = Element.ALIGN_LEFT
                        };
                        p37.Font.SetStyle("bold");
                        doc.Add(__break);
                        doc.Add(p37);
                        var p38 = new Paragraph("If any part of this agreement is declared unenforceable or invalid for any reason, the remainder will continue to be valid and enforceable.")
                        {
                            Alignment = Element.ALIGN_LEFT
                        };
                        doc.Add(p38);

                        var p39 = new Paragraph("IN WITNESS WHEREOF, the parties have hereunto signed this CREDIT AGREEMENT on " + ShortDate + " at " + ShortTime + " at Taguig City.")
                        {
                            Alignment = Element.ALIGN_LEFT
                        };
                        p39.Font.SetStyle("bold");
                        doc.Add(__break);
                        doc.Add(p39);
                        //doc.Add(__break);
                        //doc.Add(__break);
                        //doc.Add(__break);
                        doc.Close();
                    }
                }

            }
            catch (Exception ex)
            {
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", $"CommonMethods ==> {MethodBase.GetCurrentMethod().Name}");
                //throw ex;
            }
            finally
            {

            }
        }

        public static string UploadContractPDF(String ApplicationId, int UserId, string FileName)
        {

            try
            {
                ActivityLog.Info($"File {FileName} uploaded on Application {ApplicationId} on Verifier Window by User {UserId}", $"UploadContractPDF ==> {System.Reflection.MethodBase.GetCurrentMethod().Name}");
                var _Path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Content", "PDF", FileName);
                byte[] thePictureAsBytes = File.ReadAllBytes(_Path);
                string thePictureDataAsString = Convert.ToBase64String(thePictureAsBytes);
                String FinalFIleName = logger.UploadPdfToFTP(thePictureDataAsString, FileName);
                return FinalFIleName;
            }
            catch (Exception ex)
            {
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", $"CommonMethods ==> {MethodBase.GetCurrentMethod().Name}");
                return "";
            }

        }

        /// <summary>
        /// Get Civil StatusName
        /// </summary>
        /// <param name="CivilStatus"></param>
        /// <returns></returns>
        public static string GetCivilStatusName(int CivilStatus)
        {
            try
            {
                string CivilStatusName = string.Empty;
                DataTable dt = DbHelper.SelectMethod(string.Format(QueryHelper.GetCivilName, CivilStatus));
                if (dt != null && dt.Rows.Count > 0)
                {
                    CivilStatusName = dt.Rows[0]["civilname"].ToString() != string.Empty ? dt.Rows[0]["civilname"].ToString() : string.Empty;
                }
                return CivilStatusName;
            }
            catch (Exception ex)
            {
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", $"CommonMethods ==> {MethodBase.GetCurrentMethod().Name}");
                throw ex;
            }

        }

        /// <summary>
        /// GetCityName
        /// </summary>
        /// <param name="CityId"></param>
        /// <returns></returns>
        public static string GetCityName(int CityId)
        {
            try
            {
                string CityName = string.Empty;
                DataTable dt = DbHelper.SelectMethod(string.Format(QueryHelper.GetCityName, CityId));
                if (dt != null && dt.Rows.Count > 0)
                {
                    CityName = dt.Rows[0]["cityname"].ToString() != string.Empty ? dt.Rows[0]["cityname"].ToString() : string.Empty;
                }
                return CityName;
            }
            catch (Exception ex)
            {
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", $"CommonMethods ==> {MethodBase.GetCurrentMethod().Name}");
                throw ex;
            }

        }

        /// <summary>
        /// Get BarangayName
        /// </summary>
        /// <param name="BarangayId"></param>
        /// <returns></returns>
        public static string GetBarangayName(int BarangayId)
        {
            try
            {
                string BarangayName = string.Empty;
                DataTable dt = DbHelper.SelectMethod(string.Format(QueryHelper.GetBarangayName, BarangayId));
                if (dt != null && dt.Rows.Count > 0)
                {
                    BarangayName = dt.Rows[0]["barangay_name"].ToString() != string.Empty ? dt.Rows[0]["barangay_name"].ToString() : string.Empty;
                }
                return BarangayName;
            }
            catch (Exception ex)
            {

                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", $"CommonMethods ==> {MethodBase.GetCurrentMethod().Name}");
                throw ex;
            }

        }
        /// <summary>
        /// GetProvinceName
        /// </summary>
        /// <param name="ProvinceId"></param>
        /// <returns></returns>
        public static string GetProvinceName(int ProvinceId)
        {
            try
            {
                string ProvinceName = string.Empty;
                DataTable dt = DbHelper.SelectMethod(string.Format(QueryHelper.GetProvinceName, ProvinceId));
                if (dt != null && dt.Rows.Count > 0)
                {
                    ProvinceName = dt.Rows[0]["province_name"].ToString() != string.Empty ? dt.Rows[0]["province_name"].ToString() : string.Empty;
                }
                return ProvinceName;
            }
            catch (Exception ex)
            {
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", $"CommonMethods ==> {MethodBase.GetCurrentMethod().Name}");
                throw ex;
            }

        }

        /// <summary>
        /// GetCashMartRefNo
        /// </summary>
        /// <returns></returns>
        public static string GetCashMartRefNo(String ApplicationNo)
        {
            try
            {
                string FileName = string.Empty;
                int CurrentYear = Convert.ToInt32(DateTime.Now.ToString("yy"));
                DataTable _Contract = DbHelper.SelectMethod(QueryHelper.GetLastContractNo);
                if (_Contract != null && _Contract.Rows.Count > 0)
                {
                    FileName = Convert.ToString(_Contract.Rows[0]["contract_ref_no"]).Split('-').LastOrDefault();
                    int fileYear = Convert.ToInt32(FileName.Split('.')[0].Substring(2, 2));
                    if (fileYear != CurrentYear)
                    {
                        FileName = "CM" + Convert.ToString(CurrentYear) + "A00001";
                    }
                    else
                    {
                        int fileNumber = Convert.ToInt32(FileName.Split('.')[0].Substring(5, 5));
                        fileNumber += 1;
                        string NewFileNumber = string.Empty;

                        int i = fileNumber;

                        if (i > 1 && i < 10)
                        {
                            NewFileNumber = "A0000" + i;
                        }
                        else if (i > 9 && i < 100)
                        {
                            NewFileNumber = "A000" + i;
                        }
                        else if (i > 99 && i < 1000)
                        {
                            NewFileNumber = "A00" + i;
                        }
                        else if (i > 999 && i < 10000)
                        {
                            NewFileNumber = "A0" + i;
                        }
                        else if (i > 9999 && i < 99999)
                        {
                            NewFileNumber = "A" + Convert.ToString(i);
                        }
                        FileName = "CM" + Convert.ToString(CurrentYear) + NewFileNumber;
                    }
                }
                else
                {
                    FileName = "CM" + Convert.ToString(CurrentYear) + "A00001";
                }
                return $"{ApplicationNo}-{FileName}";
            }
            catch (Exception ex)
            {
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", $"CommonMethods ==> {MethodBase.GetCurrentMethod().Name}");
                throw ex;
            }
        }

        public static String GetReferenceNo(int _user_Id)
        {
            try
            {
                DataTable _ReferenceNo = DbHelper.SelectMethod(String.Format(QueryHelper.GetReferenceNo, _user_Id), null);
                if (_ReferenceNo != null && _ReferenceNo.Rows.Count > 0)
                {
                    return Convert.ToString(_ReferenceNo.Rows[0]["reference_no"]);
                }
                else
                {
                    Log.Fatal($"No Pending Amount found for USER {_user_Id}", $"CommonMethods ==> {MethodBase.GetCurrentMethod().Name}");
                    return "";
                }
            }
            catch (Exception ex)
            {
                Log.Fatal($"{ex}\n{ex.StackTrace}", $"Exception : {MethodBase.GetCurrentMethod().Name}");
                return "";
            }
        }

        /// <summary>
        /// GetLastWriteTime
        /// </summary>
        /// <param name="files"></param>
        /// <returns></returns>

        // Duplicate Record
        public static bool CheckDuplicateApplicationNo(int appno)
        {
            try
            {
                var query = DbHelper.SelectMethod(string.Format(QueryHelper.CheckDuplicateApplicationNo, appno));
                if (query != null)
                {
                    if (query.Rows.Count > 0)
                    {
                        return false;
                    }

                }
                return true;
            }
            catch (Exception ex)
            {
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", $"CommonMethods ==> {MethodBase.GetCurrentMethod().Name}");
                return true;
            }

        }
        /// <summary>
        /// Convert Number To Words
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>

        public static string NumberToWords(int number)
        {
            try
            {
                if (number == 0)
                    return "zero";

                if (number < 0)
                    return "minus " + NumberToWords(Math.Abs(number));

                string words = "";

                if ((number / 1000000) > 0)
                {
                    words += NumberToWords(number / 1000000) + " million ";
                    number %= 1000000;
                }

                if ((number / 1000) > 0)
                {
                    words += NumberToWords(number / 1000) + " thousand ";
                    number %= 1000;
                }

                if ((number / 100) > 0)
                {
                    words += NumberToWords(number / 100) + " hundred ";
                    number %= 100;
                }

                if (number > 0)
                {
                    if (words != "")
                        words += "and ";

                    var unitsMap = new[] { "zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine", "ten", "eleven", "twelve", "thirteen", "fourteen", "fifteen", "sixteen", "seventeen", "eighteen", "nineteen" };
                    var tensMap = new[] { "zero", "ten", "twenty", "thirty", "forty", "fifty", "sixty", "seventy", "eighty", "ninety" };

                    if (number < 20)
                        words += unitsMap[number];
                    else
                    {
                        words += tensMap[number / 10];
                        if ((number % 10) > 0)
                            words += "-" + unitsMap[number % 10];
                    }
                }

                return words;
            }
            catch (Exception ex)
            {
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("CommonMethods ==> NumberToWords"));
                throw ex;
            }

        }
        public static string NumberToWords(double doubleNumber)
        {
            int count = BitConverter.GetBytes(decimal.GetBits(Convert.ToDecimal(doubleNumber))[3])[2];
            int _Count = 1;
            for (int i = 1; i < count; i++)
            {
                _Count *= 10;
            }

            int beforeFloatingPoint = (int)Math.Floor(doubleNumber);
            string beforeFloatingPointWord = string.Format("{0}", NumberToWords(beforeFloatingPoint));
            string afterFloatingPointWord = string.Format("{0}", SmallNumberToWord((int)((doubleNumber - beforeFloatingPoint) * doubleNumber), ""));
            if ((int)((doubleNumber - beforeFloatingPoint) * _Count) > 0)
            {
                return string.Format("{0} point {1}", beforeFloatingPointWord, afterFloatingPointWord);
            }
            else
            {
                return string.Format("{0}", beforeFloatingPointWord);
            }
        }
        private static string SmallNumberToWord(int number, string words)
        {
            if (number <= 0) return words;
            if (words != "")
                words += " ";

            var unitsMap = new[] { "zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine", "ten", "eleven", "twelve", "thirteen", "fourteen", "fifteen", "sixteen", "seventeen", "eighteen", "nineteen" };
            var tensMap = new[] { "zero", "ten", "twenty", "thirty", "forty", "fifty", "sixty", "seventy", "eighty", "ninety" };

            if (number < 20)
                words += unitsMap[number];
            else
            {
                words += tensMap[number / 10];
                if ((number % 10) > 0)
                    words += " " + unitsMap[number % 10];
            }
            return words;
        }
        /// <summary>
        /// Get BestTimeTo CallMorning
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetBestTimeToCallMorning(int value)
        {
            try
            {
                string MorningTime = string.Empty;
                string _value = Convert.ToString(value);
                ApplicationRecordVM model = new ApplicationRecordVM();
                var data = model.BestTimeToCallMorning.FirstOrDefault(x => x.Value == _value);
                if (data != null)
                    MorningTime = data.Text;
                return MorningTime;
            }
            catch (Exception ex)
            {
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", $"CommonMethods ==> {MethodBase.GetCurrentMethod().Name}");
                throw ex;
            }

        }
        /// <summary>
        /// Get BestTimeTo CallNoon
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetBestTimeToCallNoon(int value)
        {
            try
            {
                string NoonTime = string.Empty;
                string _value = Convert.ToString(value);
                ApplicationRecordVM model = new ApplicationRecordVM();
                var data = model.BestTimeToCallNoon.FirstOrDefault(x => x.Value == _value);
                if (data != null)
                    NoonTime = data.Text;
                return NoonTime;
            }
            catch (Exception ex)
            {
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", $"CommonMethods ==> {MethodBase.GetCurrentMethod().Name}");
                throw ex;
            }

        }
        /// <summary>
        /// To Replcae Quote
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>

        public static string ReplaceQuote(string data)
        {
            try
            {
                string result = string.Empty;
                var arr = data.Split(' ');
                int i = 0;
                foreach (string item in arr)
                {
                    string a = string.Empty;
                    a = item.Replace("'", "''");
                    if (i == 0)
                    {
                        result += a;
                    }
                    else
                    {
                        result = result + ' ' + a;
                    }
                    i += 1;

                }
                return result;
            }
            catch (Exception ex)
            {
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("CommonMethods ==> ReplaceQuote"));
                throw ex;
            }

        }
        /// <summary>
        /// get Province
        /// </summary>
        /// <returns></returns>
        public static List<ProvinceModel> getProvince()
        {
            try
            {
                List<ProvinceModel> ProvinceList = new List<ProvinceModel>();
                DataTable dt = DbHelper.SelectMethod(QueryHelper.GetAllProvinceNew);
                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        ProvinceModel obj = new ProvinceModel
                        {
                            Id = Convert.ToInt32(row["province_id"]),
                            Province_Name = row["province_name"].ToString()
                        };
                        ProvinceList.Add(obj);
                    }
                }
                return ProvinceList;
            }
            catch (Exception ex)
            {
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", $"CommonMethods ==> {MethodBase.GetCurrentMethod().Name}");
                throw ex;
            }
        }
        /// <summary>
        /// get ProvinceByName
        /// </summary>
        /// <param name="province"></param>
        /// <returns></returns>
        public static List<ProvinceModel> getProvinceByName(string province)
        {
            try
            {
                List<ProvinceModel> ProvinceList = new List<ProvinceModel>();
                DataTable dt = DbHelper.SelectMethod(string.Format(QueryHelper.GetAllProvinceLatest, province));
                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        ProvinceModel obj = new ProvinceModel
                        {
                            Id = Convert.ToInt32(row["province_id"]),
                            Province_Name = row["province_name"].ToString()
                        };
                        ProvinceList.Add(obj);
                    }
                }
                return ProvinceList;
            }
            catch (Exception ex)
            {
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", $"CommonMethods ==> {MethodBase.GetCurrentMethod().Name}");
                throw ex;
            }
        }
        /// <summary>
        /// getBarangay
        /// </summary>
        /// <returns></returns>
        public static List<BarangayModel> getBarangay()
        {
            try
            {
                List<BarangayModel> BarangayList = new List<BarangayModel>();
                DataTable dt = DbHelper.SelectMethod(string.Format(QueryHelper.GetAllBarangay));
                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        BarangayModel obj = new BarangayModel
                        {
                            Id = Convert.ToInt32(row["id"]),
                            Barangay_Name = row["barangay_name"].ToString()
                        };
                        BarangayList.Add(obj);
                    }
                }
                return BarangayList;
            }
            catch (Exception ex)
            {
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", $"CommonMethods ==> {MethodBase.GetCurrentMethod().Name}");
                throw ex;
            }
        }

        /// <summary>
        /// getBarangayByID
        /// </summary>
        /// <param name="CityID"></param>
        /// <returns></returns>
        public static List<BarangayModel> getBarangayByID(int CityID)
        {
            try
            {
                List<BarangayModel> BarangayList = new List<BarangayModel>();
                DataTable dt = DbHelper.SelectMethod(string.Format(QueryHelper.GetAllBarangayByID, CityID));
                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        BarangayModel obj = new BarangayModel
                        {
                            Id = Convert.ToInt32(row["id"]),
                            Barangay_Name = row["barangay_name"].ToString()
                        };
                        BarangayList.Add(obj);
                    }
                }
                return BarangayList;
            }
            catch (Exception ex)
            {
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", $"CommonMethods ==> {MethodBase.GetCurrentMethod().Name}");
                throw ex;
            }
        }

        /// <summary>
        /// getBarangayByIDName
        /// </summary>
        /// <param name="CityID"></param>
        /// <param name="Prefix"></param>
        /// <returns></returns>
        public static List<BarangayModel> getBarangayByIDName(int CityID, string Prefix)
        {
            try
            {
                List<BarangayModel> BarangayList = new List<BarangayModel>();
                DataTable dt = DbHelper.SelectMethod(string.Format(QueryHelper.GetAllBarangayByIDName, CityID, Prefix));
                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        BarangayModel obj = new BarangayModel
                        {
                            Id = Convert.ToInt32(row["id"]),
                            Barangay_Name = row["barangay_name"].ToString()
                        };
                        BarangayList.Add(obj);
                    }
                }
                return BarangayList;
            }
            catch (Exception ex)
            {
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", $"CommonMethods ==> {MethodBase.GetCurrentMethod().Name}");
                throw ex;
            }
        }

        /// <summary>
        /// Get User Data
        /// </summary>
        /// <returns></returns>
        public static List<ApplicationRecordVM> GetUserData()
        {
            List<ApplicationRecordVM> model = new List<ApplicationRecordVM>();
            try
            {
                var query = DbHelper.SelectMethod(string.Format(QueryHelper.GetApplicationList));
                if (query.Rows.Count > 0)
                {
                    for (int i = 0; i < query.Rows.Count; i++)
                    {

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
                            Occupation = Convert.ToString(query.Rows[i]["industry"]) != "" ? (Convert.ToInt32(query.Rows[i]["industry"])) : 0
                        };
                        model.Add(record);
                    }
                }
            }
            catch (Exception msg)
            {
                logger.WriteErrorLogs(msg, $"CommonMethods ==> {MethodBase.GetCurrentMethod().Name}");
                throw msg;
            }
            return model;

        }

        /// <summary>
        /// Get PreTermData
        /// </summary>
        /// <param name="UserID"></param>
        /// <returns></returns>
        public static List<ApplicationRecordVM> GetPreTermData(int UserID)
        {
            List<ApplicationRecordVM> model = new List<ApplicationRecordVM>();
            try
            {
                var query = DbHelper.SelectMethod(string.Format(QueryHelper.GetPreTermsDataNew, UserID));
                if (query.Rows.Count > 0)
                {
                    for (int i = 0; i < query.Rows.Count; i++)
                    {
                        ApplicationRecordVM record = new ApplicationRecordVM
                        {
                            ApplicationNo = Convert.ToInt32(query.Rows[i]["applicationno"]),
                            DateApplied = Convert.ToString(query.Rows[i]["dateapplied"]),
                            PersonalEmail = Convert.ToString(query.Rows[i]["personalemail"]),
                            PersonalContactNo = Convert.ToString(query.Rows[i]["personalcontactno"]),
                            Name = Convert.ToString(query.Rows[i]["first_name"]) + " " + Convert.ToString(query.Rows[i]["middle_name"]) + " " + Convert.ToString(query.Rows[i]["last_name"]),
                            Term = Convert.ToString(query.Rows[i]["term_name"]),
                            PurposeOfLoan = Convert.ToString(query.Rows[0]["additional_loan_purpose"]) != string.Empty ? Convert.ToString(query.Rows[0]["additional_loan_purpose"]) : string.Empty
                        };
                        if (record.PurposeOfLoan == string.Empty)
                            record.PurposeOfLoan = Convert.ToString(query.Rows[0]["purposeofloan"]) != string.Empty ? Convert.ToString(query.Rows[0]["purposeofloan"]) : string.Empty;
                        record.Loan_Amount = Convert.ToString(query.Rows[i]["loanamount"]) != "" ? (Convert.ToInt32(query.Rows[i]["loanamount"])) : 0;
                        record.Remark = Convert.ToString(query.Rows[i]["remarks"]);
                        model.Add(record);
                    }
                }
            }
            catch (Exception msg)
            {
                logger.WriteErrorLogs(msg, $"CommonMethods ==> {MethodBase.GetCurrentMethod().Name}");
                throw msg;
            }
            return model;

        }
        /// <summary>
        /// getReason
        /// </summary>
        /// <returns></returns>
        public static List<NonPaymentReasonModel> getReason()
        {
            try
            {
                List<NonPaymentReasonModel> NonPaymentReasonModelList = new List<NonPaymentReasonModel>();
                DataTable dt = DbHelper.SelectMethod(QueryHelper.GetReason);
                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        NonPaymentReasonModel obj = new NonPaymentReasonModel
                        {
                            reason_id = Convert.ToInt32(row["reason_id"]),
                            reason_text = row["reason_text"].ToString()
                        };
                        NonPaymentReasonModelList.Add(obj);
                    }
                }
                NonPaymentReasonModelList.Insert(0, new NonPaymentReasonModel { reason_id = -1, reason_text = "Select" });
                return NonPaymentReasonModelList;
            }
            catch (Exception ex)
            {
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", $"CommonMethods ==> {MethodBase.GetCurrentMethod().Name}");
                throw ex;
            }
        }

        /// <summary>
        /// Get PaymentChannel
        /// </summary>
        /// <returns></returns>
        public static List<PaymentChannel> GetPaymentChannel()
        {
            try
            {
                List<PaymentChannel> PaymentChannelList = new List<PaymentChannel>();
                DataTable dt = DbHelper.SelectMethod(QueryHelper.GetPaymentChannel);
                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        PaymentChannel payment_channel = new PaymentChannel
                        {
                            id = Convert.ToInt32(row["id"]),
                            payment_channel = row["payment_channel"].ToString()
                        };
                        PaymentChannelList.Add(payment_channel);
                    }
                }
                PaymentChannelList.Insert(0, new PaymentChannel { id = -1, payment_channel = "Select" });
                return PaymentChannelList;
            }
            catch (Exception ex)
            {
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", $"CommonMethods ==> {MethodBase.GetCurrentMethod().Name}");
                throw ex;
            }
        }

        /// <summary>
        /// GetProofOfPayment
        /// </summary>
        /// <returns></returns>
        public static List<ProofOfPayment> GetProofOfPayment()
        {
            try
            {
                List<ProofOfPayment> ProofOfPaymentList = new List<ProofOfPayment>();
                DataTable dt = DbHelper.SelectMethod(QueryHelper.GetProofOfPayment);
                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        ProofOfPayment ProofOfPayment = new ProofOfPayment
                        {
                            id = Convert.ToInt32(row["id"]),
                            proof_of_payment = row["proof_of_payment"].ToString()
                        };
                        ProofOfPaymentList.Add(ProofOfPayment);
                    }
                }
                ProofOfPaymentList.Insert(0, new ProofOfPayment { id = -1, proof_of_payment = "Select" });
                return ProofOfPaymentList;
            }
            catch (Exception ex)
            {
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", $"CommonMethods ==> {MethodBase.GetCurrentMethod().Name}");
                throw ex;
            }
        }

        /// <summary>
        /// GetReLoanData
        /// </summary>
        /// <param name="UserId"></param>
        /// <returns></returns>
        public static List<ApplicationRecordVM> GetReLoanData(int UserId)
        {
            List<ApplicationRecordVM> model = new List<ApplicationRecordVM>();
            try
            {
                var query = DbHelper.SelectMethod(string.Format(QueryHelper.GetReLoanData, UserId));
                if (query.Rows.Count > 0)
                {
                    for (int i = 0; i < query.Rows.Count; i++)
                    {
                        ApplicationRecordVM record = new ApplicationRecordVM
                        {
                            ApplicationNo = Convert.ToInt32(query.Rows[i]["applicationno"]),
                            DateApplied = Convert.ToString(query.Rows[i]["dateapplied"]),
                            Name = Convert.ToString(query.Rows[i]["first_name"]) + " " + Convert.ToString(query.Rows[i]["middle_name"]) + " " + Convert.ToString(query.Rows[i]["last_name"]),
                            PersonalEmail = Convert.ToString(query.Rows[i]["personalemail"]),
                            PurposeOfLoan = Convert.ToString(query.Rows[i]["purposeofloan"]),
                            Loan_Amount = Convert.ToString(query.Rows[i]["loanamount"]) != "" ? (Convert.ToInt32(query.Rows[i]["loanamount"])) : 0,
                            Remark = Convert.ToString(query.Rows[i]["remarks"]),
                            PersonalContactNo = Convert.ToString(query.Rows[i]["personalcontactno"]),
                            reference_no = Convert.ToString(query.Rows[i]["reference_no"]),
                            Term = Convert.ToString(query.Rows[i]["term_name"])
                        };

                        record.PurposeOfLoan = Convert.ToString(query.Rows[0]["additional_loan_purpose"]) != string.Empty ? Convert.ToString(query.Rows[0]["additional_loan_purpose"]) : string.Empty;
                        if (record.PurposeOfLoan == string.Empty)
                            record.PurposeOfLoan = Convert.ToString(query.Rows[0]["purposeofloan"]) != string.Empty ? Convert.ToString(query.Rows[0]["purposeofloan"]) : string.Empty;

                        model.Add(record);
                    }
                }
            }
            catch (Exception msg)
            {
                logger.WriteErrorLogs(msg, $"CommonMethods ==> {MethodBase.GetCurrentMethod().Name}");
                throw msg;
            }
            return model;
        }

        /// <summary>
        /// Get AgencyData
        /// </summary>
        /// <returns></returns>
        public static List<ApplicationRecordVM> GetAgencyData()
        {
            List<ApplicationRecordVM> model = new List<ApplicationRecordVM>();
            try
            {

                var query = DbHelper.SelectMethod(string.Format(QueryHelper.GetAgencyData));
                if (query != null && query.Rows.Count > 0)
                {
                    for (int i = 0; i < query.Rows.Count; i++)
                    {
                        ApplicationRecordVM record = new ApplicationRecordVM
                        {
                            ApplicationNo = Convert.ToInt32(query.Rows[i]["applicationno"]),
                            Address = Convert.ToString(query.Rows[i]["address"]) + " " + Convert.ToString(query.Rows[i]["barangay_name"]) + " " + Convert.ToString(query.Rows[i]["cityname"]) + " " + Convert.ToString(query.Rows[i]["province_name"]),
                            MaturityDate = Convert.ToString(query.Rows[i]["maturity_date"]),
                            PersonalContactNo = Convert.ToString(query.Rows[i]["personalcontactno"]),
                            Name = Convert.ToString(query.Rows[i]["first_name"]) + " " + Convert.ToString(query.Rows[i]["middle_name"]) + " " + Convert.ToString(query.Rows[i]["last_name"]),
                            EscalateDate = Convert.ToString(query.Rows[i]["escalate_date"]),
                            DateofLoan = Convert.ToString(query.Rows[i]["disbursement_date"]),
                            DatePastDue = String.IsNullOrWhiteSpace(Convert.ToString(query.Rows[i]["ptp_date"])) == true ? Convert.ToString(query.Rows[i]["emi_date"]) : Convert.ToString(query.Rows[i]["ptp_date"]),
                            PaymentReference = Convert.ToString(query.Rows[i]["reference_no"]),
                            Outstanding_Balance = Convert.ToString(query.Rows[i]["outstanding_amount"]) == null ? 0.0D : Convert.ToDouble(query.Rows[i]["outstanding_amount"]),
                            TotalAmount_Due = DBNull.Value.Equals(query.Rows[i]["total_amount_due"]) ? 0.0 : Convert.ToDouble(query.Rows[i]["total_amount_due"]),
                            PersonalEmail = Convert.ToString(query.Rows[i]["personalemail"])
                        };
                        model.Add(record);
                    }
                }
            }
            catch (Exception msg)
            {
                logger.WriteErrorLogs(msg, $"CommonMethods ==> {MethodBase.GetCurrentMethod().Name}");
                throw msg;
            }
            return model;
        }

        /// <summary>
        /// Get AgencyData ByDate
        /// </summary>
        /// <param name="FromDate"></param>
        /// <param name="ToDate"></param>
        /// <returns></returns>
        public static ApplicationRecordVM GetAgencyDataByDate(String FromDate, String ToDate)
        {
            try
            {
                ApplicationRecordVM model = new ApplicationRecordVM();
                var query = DbHelper.SelectMethod(string.Format(QueryHelper.GetAgencyDataByDate, FromDate, ToDate));
                if (query != null && query.Rows.Count > 0)
                {
                    for (int i = 0; i < query.Rows.Count; i++)
                    {
                        ApplicationRecordVM record = new ApplicationRecordVM
                        {
                            ApplicationNo = Convert.ToInt32(query.Rows[i]["applicationno"]),
                            Address = Convert.ToString(query.Rows[i]["address"]) + " " + Convert.ToString(query.Rows[i]["province_name"]) + " " + Convert.ToString(query.Rows[i]["barangay_name"]) + " " + Convert.ToString(query.Rows[i]["cityname"]) + " " + Convert.ToString(query.Rows[i]["zipcode"]),
                            MaturityDate = Convert.ToString(query.Rows[i]["maturity_date"]),
                            PersonalContactNo = Convert.ToString(query.Rows[i]["personalcontactno"]),
                            Name = Convert.ToString(query.Rows[i]["first_name"]) + " " + Convert.ToString(query.Rows[i]["middle_name"]) + " " + Convert.ToString(query.Rows[i]["last_name"]),
                            EscalateDate = Convert.ToString(query.Rows[i]["escalate_date"]) != string.Empty ? Convert.ToString(query.Rows[i]["escalate_date"]) : string.Empty,
                            DateofLoan = Convert.ToString(query.Rows[i]["disbursement_date"]),
                            DatePastDue = String.IsNullOrWhiteSpace(Convert.ToString(query.Rows[i]["ptp_date"])) == true ? Convert.ToString(query.Rows[i]["emi_date"]) : Convert.ToString(query.Rows[i]["ptp_date"]),
                            PaymentReference = Convert.ToString(query.Rows[i]["reference_no"]),
                            Outstanding_Balance = Convert.ToString(query.Rows[i]["outstanding_amount"]) == null ? 0.0D : Convert.ToDouble(query.Rows[i]["outstanding_amount"]),
                            TotalAmount_Due = DBNull.Value.Equals(query.Rows[i]["total_amount_due"]) ? 0.0 : Convert.ToDouble(query.Rows[i]["total_amount_due"]),
                            PersonalEmail = Convert.ToString(query.Rows[i]["personalemail"])
                        };
                        model.ApplicationRecordList.Add(record);
                    }

                }
                return model;
            }
            catch (Exception ex)
            {
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", $"CommonMethods ==> {MethodBase.GetCurrentMethod().Name}");
                throw ex;
            }
        }

        /// <summary>
        /// Get FSBucketData Admin
        /// </summary>
        /// <returns></returns>
        public static List<FSBucket> GetFSBucketDataAmin()
        {
            List<FSBucket> namesList = new List<FSBucket>();
            try
            {
                var query = DbHelper.SelectMethod(QueryHelper.GetFSDetailsForAdmin);
                if (query != null && query.Rows.Count > 0)
                {
                    for (int i = 0; i < query.Rows.Count; i++)
                    {
                        FSBucket data = new FSBucket
                        {
                            Application_No = Convert.ToInt32(query.Rows[i]["applicationno"]),
                            Name = Convert.ToString(query.Rows[i]["first_name"]).ToString(),
                            FS_Date = Convert.ToDateTime(query.Rows[i]["fs_date"]).ToString("MM/dd/yyyy"),
                            TermId = Convert.ToInt32(query.Rows[i]["term"]),
                            Amount = Convert.ToDouble(query.Rows[i]["amount"]),
                            Records = Convert.ToInt32(query.Rows[i]["records"]),
                            Is_Reduce_Loan = Convert.ToBoolean(query.Rows[i]["is_reduceloan"]),
                            BalanceAmount = Convert.ToDouble(query.Rows[i]["balance"]),
                            LoanTerm = Convert.ToString(query.Rows[i]["termplan"]),
                            PersonalContactNumber = Convert.ToString(query.Rows[i]["personalcontactno"]),
                            personal_email = Convert.ToString(query.Rows[i]["personalemail"])
                        };
                        namesList.Add(data);
                    }
                }
                return namesList;
            }
            catch (Exception msg)
            {
                logger.WriteErrorLogs(msg, $"CommonMethods ==> {MethodBase.GetCurrentMethod().Name}");
                throw msg;
            }
        }

        public static string GetFirebaseToken(String ApplicationNo)
        {
            string Firebasetoken = string.Empty;
            try
            {
                var query = DbHelper.SelectMethod(String.Format(QueryHelper.GetFirebaseToken, ApplicationNo));
                if (query != null && query.Rows.Count > 0)
                {
                    for (int i = 0; i < query.Rows.Count; i++)
                    {
                        Firebasetoken = Convert.ToString(query.Rows[i]["notificationtoken"]);
                    }
                }
                return Firebasetoken;
            }
            catch (Exception msg)
            {
                logger.WriteErrorLogs(msg, $"CommonMethods ==> {MethodBase.GetCurrentMethod().Name}");
                throw msg;
            }
        }


        /// <summary>
        /// Get NewReference Approvals
        /// </summary>
        /// <returns></returns>
        public static List<NewReferenceApprovalModel> GetNewReferenceApprovals()
        {
            List<NewReferenceApprovalModel> _List = new List<NewReferenceApprovalModel>();
            try
            {
                var query = DbHelper.SelectMethod(QueryHelper.GetNewReferences);
                if (query != null && query.Rows.Count > 0)
                {
                    for (int i = 0; i < query.Rows.Count; i++)
                    {
                        NewReferenceApprovalModel data = new NewReferenceApprovalModel
                        {
                            id = Convert.ToInt32(query.Rows[i]["id"]),
                            application_no = Convert.ToInt32(query.Rows[i]["application_no"]),
                            old_reference_no = Convert.ToString(query.Rows[i]["old_reference_no"]),
                            new_reference_no = Convert.ToString(query.Rows[i]["new_reference_no"]),
                            edited_by = Convert.ToInt32(query.Rows[i]["edited_by"]),
                            userfullname = Convert.ToString(query.Rows[i]["userfullname"])
                        };

                        _List.Add(data);
                    }

                }
                return _List;
            }
            catch (Exception msg)
            {
                logger.WriteErrorLogs(msg, $"CommonMethods ==> {MethodBase.GetCurrentMethod().Name}");
                throw msg;
            }
        }

        public static List<RemarkModel> GetRemarkList(int application_id)
        {
            try
            {
                List<RemarkModel> remarkList = new List<RemarkModel>();
                var query = DbHelper.SelectMethod(string.Format(QueryHelper.GetRemark, application_id));
                if (query != null && query.Rows.Count > 0)
                {
                    for (int i = 0; i < query.Rows.Count; i++)
                    {
                        RemarkModel remark = new RemarkModel
                        {
                            remark = Convert.ToString(query.Rows[i]["remark"]),
                            createdby = Convert.ToInt32(query.Rows[i]["createdby"]),
                            createdbyname = Convert.ToString(query.Rows[i]["createdbyname"]),
                            createdon = Convert.ToDateTime(query.Rows[i]["createdon"]),
                            application_id = Convert.ToInt32(query.Rows[i]["application_id"]),
                            id = Convert.ToInt32(query.Rows[i]["id"])
                        };
                        remark.remarkon = remark.createdon.ToString();
                        remarkList.Add(remark);
                    }

                }
                return remarkList;


            }
            catch (Exception ex)
            {
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", $"CommonMethods ==> {MethodBase.GetCurrentMethod().Name}");
                throw ex;
            }
        }


        public static List<customer_loan_calc> GetCustomer_Loan_Calcs(int ApplicantId, EMICalculatorModel eMICalculatorModel)
        {
            try
            {
                List<customer_loan_calc> lstEMIDetail = new List<customer_loan_calc>();
                if (ApplicantId > 0)
                {
                    if (eMICalculatorModel.Amount > 0 && eMICalculatorModel.LoanType > 0 && eMICalculatorModel.tenure > 0)
                    {
                        int _DaysDiff = 0;
                        _DaysDiff = eMICalculatorModel.LoanType switch
                        {
                            1 => 7,
                            2 => 14,
                            3 => 28,
                            _ => 7,
                        };

                        var _tenure = eMICalculatorModel.tenure / _DaysDiff;


                        DateTime DisbursementDate = eMICalculatorModel.LoanDate;
                        for (int i = 0; i < _tenure; i++)
                        {
                            DisbursementDate = DisbursementDate.AddDays(_DaysDiff);
                            decimal principle_amount = Math.Round((eMICalculatorModel.Amount / _tenure), 2);
                            decimal interest_rate = Math.Round(Convert.ToDecimal(eMICalculatorModel.Interest), 2);
                            decimal interest_amount = Math.Round(((principle_amount * interest_rate) / 100), 2);
                            decimal emi_amount = principle_amount + interest_amount;
                            logger.WriteErrorLogs($"principle_amount : {principle_amount}\n interest_rate : {interest_rate}\ninterest_amount : {interest_amount}\nemi_amount :{emi_amount}", $"{MethodBase.GetCurrentMethod().Name}");
                            lstEMIDetail.Add(new customer_loan_calc
                            {
                                id = i + 1,
                                EmiDate = DisbursementDate.Date,
                                principle_amount = principle_amount,
                                interest_rate = interest_rate,
                                interest_amount = interest_amount,
                                emi_amount = emi_amount
                            });
                        }
                        logger.WriteErrorLogs($"{JsonConvert.SerializeObject(lstEMIDetail)}", $"{MethodBase.GetCurrentMethod().Name}");
                        return lstEMIDetail;
                    }
                    else
                    {
                        lstEMIDetail = new List<customer_loan_calc>();
                    }
                }
                logger.WriteErrorLogs($"{JsonConvert.SerializeObject(lstEMIDetail)}", $"{MethodBase.GetCurrentMethod().Name}");
                return lstEMIDetail;
            }
            catch (Exception ex)
            {
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", $"Exception : {System.Reflection.MethodBase.GetCurrentMethod().Name}");
                return new List<customer_loan_calc>();
            }
        }

        public static decimal GetPretermDeductionAmount(int _user_Id)
        {
            try
            {
                decimal pending_amount = 0;
                DataTable _PendinaAmountTable = DbHelper.SelectMethod(String.Format(QueryHelper.GetPendingAmountforPreterm, _user_Id));
                if (_PendinaAmountTable != null && _PendinaAmountTable.Rows.Count > 0)
                {
                    pending_amount = Convert.ToString(_PendinaAmountTable.Rows[0]["pending_amount"]) != string.Empty ? Convert.ToDecimal(_PendinaAmountTable.Rows[0]["pending_amount"]) : 0;
                }
                else
                {
                    logger.WriteErrorLogs($"No Pending Amount found for USER {_user_Id}", $"CommonMethods ==> {MethodBase.GetCurrentMethod().Name}");
                }
                return pending_amount;
            }
            catch (Exception ex)
            {
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", $"Exception : {MethodBase.GetCurrentMethod().Name}");
                return 0;
            }
        }

    }
    public class DataTable_GenericList<T> where T : class, new()
    {
        NpgsqlConnection npgsqlConnection;
        NpgsqlCommand npgsqlCommand;
        NpgsqlDataReader npgsqlDataReader;
        public void ReadData(string query, List<T> TList, List<Parameters> dbDataParameters)
        {
            try
            {
                npgsqlConnection = new NpgsqlConnection(Convert.ToString(ConfigurationManager.ConnectionStrings["DefaultConnection"]));
                if (npgsqlConnection.State == ConnectionState.Closed || npgsqlConnection.State == ConnectionState.Broken)
                {
                    npgsqlConnection.Open();
                }
                npgsqlCommand = new NpgsqlCommand();
                npgsqlCommand = npgsqlConnection.CreateCommand();
                npgsqlCommand.CommandText = query;
                npgsqlCommand.CommandTimeout = 36000;
                if (dbDataParameters != null)
                {
                    foreach (var item in dbDataParameters)
                    {
                        var parameter = npgsqlCommand.CreateParameter();
                        parameter.ParameterName = item.ParameterName;
                        parameter.Value = item.ParameterValue;
                        npgsqlCommand.Parameters.Add(parameter);
                    }
                }

                npgsqlDataReader = npgsqlCommand.ExecuteReader();
                while (npgsqlDataReader.Read())
                {
                    T t = new T();
                    Type TType = t.GetType();
                    for (int x = 0; x < npgsqlDataReader.FieldCount; x++)
                    {
                        PropertyInfo propertyInfo = TType.GetProperty(npgsqlDataReader.GetName(x));

                        if (propertyInfo == null)
                        {
                            throw new Exception("The property " + npgsqlDataReader.GetName(x) + " was not found in the object - " + TType.ToString());
                        }

                        if (propertyInfo.CanWrite && npgsqlDataReader[x] != DBNull.Value)
                        {
                            propertyInfo.SetValue(t, npgsqlDataReader[x], null);
                        }
                    }
                    TList.Add(t);
                }
            }
            catch (Exception ex)
            {
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", MethodBase.GetCurrentMethod().Name);
                TList = new List<T>();
            }
            finally
            {
                if (npgsqlDataReader != null)
                    npgsqlDataReader.Close();
                if (npgsqlCommand != null)
                    npgsqlCommand.Dispose();
                npgsqlConnection.Close();
                npgsqlConnection.Dispose();
            }

        }
    }
    public class EMICalculatorModel
    {
        public decimal Amount { get; set; }
        public int tenure { get; set; }
        public int LoanType { get; set; }
        public double Interest { get; set; }
        public DateTime LoanDate { get; set; }
    }
    public class customer_loan_calc
    {
        public Int64 id { get; set; }
        public DateTime EmiDate { get; set; }
        public decimal principle_amount { get; set; }
        public decimal interest_rate { get; set; }
        public decimal interest_amount { get; set; }
        public decimal emi_amount { get; set; }
        public decimal balance { get; set; }
        public int emi_id { get; set; }
        public string ApplicationNo { get; set; }
        public bool ispaid { get; set; } = false;
        public DateTime paidon { get; set; }
        public string DaysLeft { get; set; } = "";
    }
    public class ITextEvents : PdfPageEventHelper
    {

        // This is the contentbyte object of the writer
        PdfContentByte cb;

        // we will put the final number of pages in a template
        PdfTemplate footerTemplate;

        // this is the BaseFont we are going to use for the header / footer
        BaseFont bf = null;

        // This keeps track of the creation time
        DateTime PrintTime = DateTime.Now;


        #region Fields
        private string _header;
        #endregion

        #region Properties
        public string Header
        {
            get { return _header; }
            set { _header = value; }
        }
        #endregion


        public override void OnOpenDocument(PdfWriter writer, Document document)
        {
            try
            {
                PrintTime = DateTime.Now;
                bf = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                cb = writer.DirectContent;
                footerTemplate = cb.CreateTemplate(60, 60);
            }
            catch (DocumentException de)
            {
                logger.WriteErrorLogs(de, "OnOpenDocument : DocumentException");
            }
            catch (System.IO.IOException ioe)
            {
                logger.WriteErrorLogs(ioe, "OnOpenDocument : IOException");
            }
        }

        public override void OnEndPage(iTextSharp.text.pdf.PdfWriter writer, iTextSharp.text.Document document)
        {
            base.OnEndPage(writer, document);
            //Create PdfTable object
            PdfPTable pdfTab = new PdfPTable(3);

            //We will have to create separate cells to include image logo and 2 separate strings
            //Row 1
            PdfPCell pdfCell1 = new PdfPCell();
            PdfPCell pdfCell3 = new PdfPCell();
            String text = "Signature: ______________________________";
            string text1 = "Borrower’s Full Name and Signature";

            string text2 = "CASH MART ASIA LENDING INC";
            string text3 = "Lender Name";

            string Paging = "Page " + writer.PageNumber + " of ";

            //Add paging to footer
            {

                cb.BeginText();
                cb.SetFontAndSize(bf, 12);
                cb.SetTextMatrix(document.PageSize.GetRight(550), document.PageSize.GetBottom(30));
                cb.SetColorFill(BaseColor.RED);
                cb.ShowText(text);
                cb.EndText();

                cb.BeginText();
                cb.SetFontAndSize(bf, 12);
                cb.SetTextMatrix(document.PageSize.GetRight(490), document.PageSize.GetBottom(15));
                cb.SetColorFill(BaseColor.RED);
                cb.ShowText(text1);
                cb.EndText();

                cb.BeginText();
                cb.SetFontAndSize(bf, 12);
                cb.SetTextMatrix(document.PageSize.GetRight(250), document.PageSize.GetBottom(30));
                cb.SetColorFill(BaseColor.RED);
                cb.ShowText(text2);
                cb.EndText();

                cb.BeginText();
                cb.SetFontAndSize(bf, 12);
                cb.SetTextMatrix(document.PageSize.GetRight(200), document.PageSize.GetBottom(15));
                cb.SetColorFill(BaseColor.RED);
                cb.ShowText(text3);
                cb.EndText();



                cb.BeginText();
                cb.SetFontAndSize(bf, 12);
                cb.SetTextMatrix(document.PageSize.GetRight(100), document.PageSize.GetBottom(10));
                cb.ShowText(Paging);
                cb.EndText();
                float len = bf.GetWidthPoint(Paging, 12);
                cb.AddTemplate(footerTemplate, document.PageSize.GetRight(100) + len, document.PageSize.GetBottom(10));
            }

            //set the alignment of all three cells and set border to 0
            pdfCell1.HorizontalAlignment = Element.ALIGN_CENTER;
            pdfCell3.HorizontalAlignment = Element.ALIGN_CENTER;
            pdfCell3.VerticalAlignment = Element.ALIGN_MIDDLE;

            pdfCell1.Border = 0;
            pdfCell3.Border = 0;

            //add all three cells into PdfTable
            pdfTab.AddCell(pdfCell1);
            pdfTab.AddCell(pdfCell3);

            pdfTab.TotalWidth = document.PageSize.Width - 80f;
            pdfTab.WidthPercentage = 70;


            //call WriteSelectedRows of PdfTable. This writes rows from PdfWriter in PdfTable
            //first param is start row. -1 indicates there is no end row and all the rows to be included to write
            //Third and fourth param is x and y position to start writing
            pdfTab.WriteSelectedRows(0, -1, 40, document.PageSize.Height - 30, writer.DirectContent);
            //set pdfContent value

            //Move the pointer and draw line to separate footer section from rest of page
            cb.MoveTo(40, document.PageSize.GetBottom(60));
            cb.LineTo(document.PageSize.Width - 40, document.PageSize.GetBottom(60));
            cb.Stroke();
        }

        public override void OnCloseDocument(PdfWriter writer, Document document)
        {
            base.OnCloseDocument(writer, document);

            footerTemplate.BeginText();
            footerTemplate.SetFontAndSize(bf, 12);
            footerTemplate.SetTextMatrix(0, 0);
            footerTemplate.ShowText((writer.PageNumber).ToString());
            footerTemplate.EndText();
        }
    }
    public static class DecimalToWordConverter
    {
        private static readonly string[] Ones =
        {
        "", "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine", "Ten",
        "Eleven", "Twelve", "Thirteen", "Fourteen", "Fifteen", "Sixteen", "Seventeen", "Eighteen", "Nineteen"
    };

        private static readonly string[] Tens =
        {
        "", "", "Twenty", "Thirty", "Forty", "Fifty", "Sixty", "Seventy", "Eighty", "Ninety"
    };

        public static string ConvertDecimalToWords(decimal number)
        {
            string words = "";

            // Extract the integer part
            long intPart = (long)number;

            // Extract the decimal part
            decimal decimalPart = number - intPart;
            if (decimalPart > 0)
            {
                words += ConvertToWords(intPart) + " point " + ConvertDecimalPartToWords(decimalPart) + "";
            }
            else
            {
                words += ConvertToWords(intPart);
            }
            return words;
        }

        private static string ConvertToWords(long number)
        {
            if (number == 0)
                return "Zero";

            string words = "";

            if ((number / 1000000) > 0)
            {
                words += ConvertToWords(number / 1000000) + " Million ";
                number %= 1000000;
            }

            if ((number / 1000) > 0)
            {
                words += ConvertToWords(number / 1000) + " Thousand ";
                number %= 1000;
            }

            if ((number / 100) > 0)
            {
                words += ConvertToWords(number / 100) + " Hundred ";
                number %= 100;
            }

            if (number > 0)
            {
                if (number < 20)
                    words += Ones[number];
                else
                {
                    words += Tens[number / 10];
                    if ((number % 10) > 0)
                        words += " " + Ones[number % 10];
                }
            }

            return words;
        }

        private static string ConvertDecimalPartToWords(decimal number)
        {
            int count = BitConverter.GetBytes(decimal.GetBits(number)[3])[2];
            int _Count = 1;
            for (int i = 0; i < count; i++)
            {
                _Count *= 10;
            }
            int decimalValue = (int)(number * _Count); // Convert decimal part to an integer

            return ConvertToWords(decimalValue);
        }

    }
}