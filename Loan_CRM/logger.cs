
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using Serilog;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Loan_CRM
{
    public class logger
    {
        /// <summary>
        ///  Write Error Log by passing message and function
        /// </summary>
        /// <param name="message"></param> 
        /// <param name="Function"></param> 
        /// <returns></returns>
        /// <remarks></remarks>
        public static void WriteErrorLogs(Exception ex, String Function)
        {
            try
            {
                Logs.WriteFatal(ex, Function);
            }
            catch
            {
            }
        }
        public static void WriteErrorLogs(string ex, String Function)
        {
            try
            {
                if (Convert.ToString(ConfigurationManager.AppSettings["EnableLog"]) == "1")
                    Logs.WriteInformation(ex, Function);
            }
            catch
            {
            }
        }
        public static bool ClicktoSMS(string ContactNumber, string Message)
        {
            try
            {
                List<String> _Mobile = new List<string>();
                if (ContactNumber.Contains(","))
                {
                    var _Temp = ContactNumber.Split(',').ToList();
                    foreach (var item in _Temp)
                    {
                        String _Final = String.Empty;
                        if (!item.StartsWith("0") && item.Length == 10)
                        {
                            _Final = $"0{item}";
                        }
                        else
                        {
                            _Final = item;
                        }
                        _Mobile.Add(_Final);
                    }
                }
                else if (!String.IsNullOrWhiteSpace(ContactNumber))
                {
                    if (!ContactNumber.StartsWith("0") && ContactNumber.Length == 10)
                    {
                        ContactNumber = $"0{ContactNumber}";
                    }
                    _Mobile.Add(ContactNumber);
                }
                else
                {
                    logger.WriteErrorLogs("No Contact Number found for Sending Message", String.Format("Logger ==> ClicktoSMS"));
                    return false;
                }

                foreach (var item in _Mobile)
                {
                    string sURL = $"{ConfigurationManager.AppSettings["SMSURL"]}?do={ConfigurationManager.AppSettings["SMSAction"]}&username={ConfigurationManager.AppSettings["SMSUserName"]}&userpass={ConfigurationManager.AppSettings["SMSUserPassword"]}&phone_number={item ?? ""}&message={Message ?? ""}";
                    logger.WriteErrorLogs(sURL, String.Format("Logger ==> ClicktoSMS"));
                    WebRequest wrGETURL;
                    wrGETURL = WebRequest.Create(sURL);
                    WebProxy myProxy = new WebProxy("myproxy", 80)
                    {
                        BypassProxyOnLocal = true
                    };
                    ServicePointManager.ServerCertificateValidationCallback = delegate (Object obj, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors) { return (true); };
                    Stream objStream;
                    objStream = wrGETURL.GetResponse().GetResponseStream();
                    StreamReader objReader = new StreamReader(objStream);
                }
                return true;
            }
            catch (Exception ex)
            {
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("Logger ==> ClicktoSMS"));
                return false;
            }
        }
        public static int ClicktoCall(string ContactNo, string ExeAgentName, string IPAddress)
        {
            try
            {
                List<string> _Mobile = new List<string>();
                if (ContactNo.Contains(","))
                {
                    var _Temp = ContactNo.Split(',').ToList();
                    foreach (var item in _Temp)
                    {
                        var _Con = String.Empty;
                        if (!item.StartsWith("0") && !string.IsNullOrWhiteSpace(item))
                        {
                            _Con = "0" + item;
                        }
                        else
                        {
                            _Con = item;
                        }
                        _Mobile.Add(_Con);
                    }
                }
                else if (!String.IsNullOrWhiteSpace(ContactNo))
                {
                    if (!ContactNo.StartsWith("0") && !string.IsNullOrWhiteSpace(ContactNo))
                    {
                        ContactNo = "0" + ContactNo;
                    }
                    _Mobile.Add(ContactNo);
                }
                else
                {
                    logger.WriteErrorLogs("No Contact Number found for Dialing", String.Format("Logger ==> ClicktoCall"));
                    return 0;
                }

                string sURL = string.Empty;
                if (ExeAgentName.Contains("@"))
                {
                    sURL = $"https://{IPAddress}:8475/CrmDial?phoneNumber={_Mobile.FirstOrDefault()}&exeUserName={ExeAgentName}";
                }
                else
                {
                    sURL = $"https://{IPAddress}:8475/CrmDial?phoneNumber={_Mobile.FirstOrDefault()}&exeUserName={ExeAgentName}@{ConfigurationManager.AppSettings["LeadId_domain"]}";
                }
                logger.WriteErrorLogs(sURL, String.Format("Logger ==> ClicktoCall"));
                WebRequest wrGETURL;
                wrGETURL = WebRequest.Create(sURL);
                WebProxy myProxy = new WebProxy("myproxy", 80)
                {
                    BypassProxyOnLocal = true
                };
                ServicePointManager.ServerCertificateValidationCallback = delegate (Object obj, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors) { return (true); };
                Stream objStream;
                objStream = wrGETURL.GetResponse().GetResponseStream();
                StreamReader objReader = new StreamReader(objStream);
                return 1;
            }
            catch (Exception ex)
            {
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("Logger ==> ClicktoCall"));
                return 0;
            }
        }
        public static string GetTermName(string term)
        {
            try
            {
                string str = string.Empty;
                List<Parameters> parameters = new List<Parameters>();
                parameters.Add(new Parameters() { DbType = NpgsqlTypes.NpgsqlDbType.Integer, ParameterName = "id", ParameterValue = term });
                DataTable dt = DbHelper.SelectMethod(QueryHelper.GetTermTypebyId, parameters);
                if (dt != null && dt.Rows.Count > 0)
                {
                    str = Convert.ToString(dt.Rows[0]["term_name"]);
                }
                return str;
            }
            catch (Exception ex)
            {
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", "GetTermName", System.Reflection.MethodBase.GetCurrentMethod().Name));
                throw ex;
            }

        }
        public static string SendNotificationFromFirebaseCloud(NotificationData notification, string ApplicationNo)
        {
            var result = "-1";
            try
            {
                var FCMSenderKey = Convert.ToString(ConfigurationManager.AppSettings["FCMSenderKey"]);
                var webAddr = "https://fcm.googleapis.com/fcm/send";
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(webAddr);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Headers.Add(HttpRequestHeader.Authorization, $"key={FCMSenderKey}");
                httpWebRequest.Method = "POST";
                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    streamWriter.Write(JsonConvert.SerializeObject(notification));
                    streamWriter.Flush();
                }
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    result = streamReader.ReadToEnd();
                    WriteErrorLogs(result, string.Format("{0} ==> {1} ==> {2} ==> For Application No : {3}", "SendNotificationFromFirebaseCloud", System.Reflection.MethodBase.GetCurrentMethod().Name, JsonConvert.SerializeObject(notification), ApplicationNo));
                }
            }
            catch (Exception ex)
            {
                WriteErrorLogs(ex, string.Format("{0} ==> {1} ==> {2}", "SendNotificationFromFirebaseCloud", System.Reflection.MethodBase.GetCurrentMethod().Name, JsonConvert.SerializeObject(notification), ApplicationNo));
            }

            return result;
        }
        public static string SendNotificationFromFirebaseCloud(ManualNotificationData notification)
        {
            var result = "-1";
            try
            {
                var FCMSenderKey = Convert.ToString(ConfigurationManager.AppSettings["FCMSenderKey"]);
                var webAddr = "https://fcm.googleapis.com/fcm/send";
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(webAddr);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Headers.Add(HttpRequestHeader.Authorization, $"key={FCMSenderKey}");
                httpWebRequest.Method = "POST";
                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    streamWriter.Write(JsonConvert.SerializeObject(notification));
                    streamWriter.Flush();
                }
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    result = streamReader.ReadToEnd();
                    logger.WriteErrorLogs(result, String.Format("{0} ==> {1}", "GetTermName", System.Reflection.MethodBase.GetCurrentMethod().Name));
                }
            }
            catch (Exception ex)
            {
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", "GetTermName", System.Reflection.MethodBase.GetCurrentMethod().Name));
            }

            return result;
        }
        public static bool CheckFolderExist(String FolderName)
        {
            try
            {
                string uploadUrl = Path.Combine(Convert.ToString(ConfigurationManager.AppSettings["FTPUrl"]), $"ApplicationNo{FolderName}");
                FtpWebRequest request = WebRequest.Create(uploadUrl) as FtpWebRequest;
                request.Credentials = new NetworkCredential(Convert.ToString(ConfigurationManager.AppSettings["FTP_UserName"]), Convert.ToString(ConfigurationManager.AppSettings["FTP_Password"]));
                request.Proxy = null;
                request.UseBinary = true;
                request.UsePassive = true;
                request.Timeout = 5000;
                request.Method = WebRequestMethods.Ftp.ListDirectory;
                return request.GetResponse() != null;
            }
            catch (Exception ex)
            {
                Log.Information(ex, "");
                return false;
            }

        }
        public static string UploadImageToFTP(string Image_Data, String FolderName, string FileName)
        {
            try
            {
                String uploadUrl = string.Empty;
                CreateFolderFTP($"ApplicationNo{FolderName}");

                if (Image_Data.Length > 0)
                {
                    List<string> FilePath = new List<string>();
                    if (FileName.Contains("/"))
                    {
                        FilePath = FileName.Split('/').ToList();
                    }
                    else
                    {
                        FilePath.Add(FileName);
                    }
                    byte[] data = Convert.FromBase64String(Image_Data);
                    uploadUrl = Path.Combine(Convert.ToString(ConfigurationManager.AppSettings["FTPUrl"]), $"ApplicationNo{FolderName}/{FilePath.LastOrDefault()}");
                    FtpWebRequest req = (FtpWebRequest)FtpWebRequest.Create(uploadUrl);
                    req.Proxy = null;
                    req.Method = WebRequestMethods.Ftp.UploadFile;
                    req.Credentials = new NetworkCredential(Convert.ToString(ConfigurationManager.AppSettings["FTP_UserName"]), Convert.ToString(ConfigurationManager.AppSettings["FTP_Password"]));
                    req.UseBinary = true;
                    req.UsePassive = true;
                    req.Timeout = 5000;
                    req.ContentLength = data.Length;
                    using (Stream stream = req.GetRequestStream())
                    {
                        stream.Write(data, 0, data.Length);
                    }
                    using (FtpWebResponse res = (FtpWebResponse)req.GetResponse())
                    {
                        Console.WriteLine("Upload File Complete, status {0}", res.StatusDescription);
                    }
                }
                return uploadUrl;
            }
            catch (Exception ex)
            {
                WriteErrorLogs(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                return string.Empty;
            }
        }
        public static string CreateFolderFTP(String FolderName)
        {
            try
            {
                if (!CheckFolderExist(FolderName) && !String.IsNullOrWhiteSpace(Convert.ToString(ConfigurationManager.AppSettings["FTPUrl"])))
                {
                    String uploadUrl = Path.Combine(Convert.ToString(ConfigurationManager.AppSettings["FTPUrl"]), $"{FolderName}");
                    FtpWebRequest request = (FtpWebRequest)FtpWebRequest.Create(uploadUrl);
                    request.Credentials = new NetworkCredential(Convert.ToString(ConfigurationManager.AppSettings["FTP_UserName"]), Convert.ToString(ConfigurationManager.AppSettings["FTP_Password"]));
                    request.UsePassive = true;
                    request.UseBinary = true;
                    request.KeepAlive = false;
                    request.Timeout = 5000;
                    request.Method = WebRequestMethods.Ftp.MakeDirectory;
                    using (var resp = (FtpWebResponse)request.GetResponse())
                    {
                    }
                }


                return $"{FolderName}";
            }
            catch (Exception ex)
            {
                WriteErrorLogs(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                return string.Empty;
            }
        }
        public static string DownloadImageFromFTP(string url)
        {
            byte[] buf = null;
            try
            {
                if (!String.IsNullOrWhiteSpace(url))
                {
                    var request = (FtpWebRequest)WebRequest.Create(url);
                    request.Method = WebRequestMethods.Ftp.DownloadFile;
                    request.Credentials = new NetworkCredential(Convert.ToString(ConfigurationManager.AppSettings["FTP_UserName"]), Convert.ToString(ConfigurationManager.AppSettings["FTP_Password"]));
                    request.UseBinary = true;
                    using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
                    {
                        using (Stream responseStream = response.GetResponseStream())
                        {
                            byte[] buffer = new byte[16 * 1024];
                            using (MemoryStream ms = new MemoryStream())
                            {
                                int read;
                                while ((read = responseStream.Read(buffer, 0, buffer.Length)) > 0)
                                {
                                    ms.Write(buffer, 0, read);
                                }
                                buf = ms.ToArray();
                            }
                        }
                    }
                    return Convert.ToBase64String(buf);
                }
                else
                {
                    return string.Empty;
                }
            }
            catch (Exception ex)
            {
                WriteErrorLogs(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                return string.Empty;
            }
        }
        public static string CopyImageOnFTP(string url, string FolderName, string FileName)
        {
            return UploadImageToFTP(DownloadImageFromFTP(url), FolderName, FileName);
        }
        public static bool DeleteFileOnServer(Uri serverUri)
        {
            try
            {
                if (serverUri.Scheme != Uri.UriSchemeFtp)
                {
                    return false;
                }
                // Get the object used to communicate with the server.
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(serverUri);
                request.Method = WebRequestMethods.Ftp.DeleteFile;
                request.Credentials = new NetworkCredential(Convert.ToString(ConfigurationManager.AppSettings["FTP_UserName"]), Convert.ToString(ConfigurationManager.AppSettings["FTP_Password"]));
                request.UseBinary = true;
                request.UsePassive = true;
                request.Timeout = 10000;
                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                Console.WriteLine("Delete status: {0}", response.StatusDescription);
                response.Close();
                return true;
            }
            catch (Exception)
            {
                return true;
            }

        }
        public static bool SendNotification(string Message, string ApplicationNo)
        {
            try
            {
                NotificationData notification = new NotificationData
                {
                    to = CommonMethods.GetFirebaseToken(ApplicationNo),
                    notification = new Notification
                    {
                        sound = "default",
                        body = Message,
                        title = "Cashmart"
                    }
                };
                logger.SendNotificationFromFirebaseCloud(notification, ApplicationNo);
            }
            catch (Exception ex)
            {
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", "Logger", System.Reflection.MethodBase.GetCurrentMethod().Name));
            }

            return true;
        }
        public static void AddWelcomeMessage(string Type, string reference, string Message, double amount, bool forNotification, int applicationno, string partnername = "")
        {
            try
            {
                int userid = 0;
                if (applicationno > 0)
                {
                    var _dt = DbHelper.SelectMethod(string.Format(QueryHelper.GetClientName, applicationno));
                    string FullName = string.Empty;
                    if (_dt != null && _dt.Rows.Count > 0)
                    {
                        userid = Convert.ToInt32(_dt.Rows[0]["user_id"]);
                    }

                    List<Parameters> parameters = new List<Parameters>{
                            new Parameters{ParameterName="type",ParameterValue=Convert.ToString(Type) },
                            new Parameters{ParameterName="amount",ParameterValue=Convert.ToString(amount)},
                            new Parameters{ParameterName="historymsg",ParameterValue=Convert.ToString(Message) },
                            new Parameters{ParameterName="referenceno",ParameterValue=Convert.ToString(reference)},
                            new Parameters{ParameterName="userid",ParameterValue=Convert.ToString(userid) },
                            new Parameters{ParameterName="partnername",ParameterValue=Convert.ToString(partnername)},
                            new Parameters{ParameterName="for_notification",ParameterValue=Convert.ToString(forNotification)}
                        };

                    DbHelper.InsertUpdateDelete(String.Format(QueryHelper.InsertNotificationHistory, Type, amount, Message, reference, userid, forNotification, partnername));
                    SendNotification(Message, Convert.ToString(applicationno));
                }
            }
            catch (Exception ex)
            {
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", String.Format("{0} ==> {1}", "Logger", System.Reflection.MethodBase.GetCurrentMethod().Name));
            }

        }
        public static string UploadPdfToFTP(string Image_Data, string FileName)
        {
            try
            {
                String uploadUrl = string.Empty;
                CreateFolderFTP("Contracts");

                if (Image_Data.Length > 0)
                {
                    List<string> FilePath = new List<string>();
                    if (FileName.Contains("/"))
                    {
                        FilePath = FileName.Split('/').ToList();
                    }
                    else
                    {
                        FilePath.Add(FileName);
                    }
                    byte[] data = Convert.FromBase64String(Image_Data);
                    uploadUrl = Path.Combine(Convert.ToString(ConfigurationManager.AppSettings["FTPUrl"]), $"Contracts/{FilePath.LastOrDefault()}");
                    FtpWebRequest req = (FtpWebRequest)FtpWebRequest.Create(uploadUrl);
                    req.Proxy = null;
                    req.Method = WebRequestMethods.Ftp.UploadFile;
                    req.Credentials = new NetworkCredential(Convert.ToString(ConfigurationManager.AppSettings["FTP_UserName"]), Convert.ToString(ConfigurationManager.AppSettings["FTP_Password"]));
                    req.UseBinary = true;
                    req.UsePassive = true;
                    req.Timeout = 5000;
                    req.ContentLength = data.Length;
                    using (Stream stream = req.GetRequestStream())
                    {
                        stream.Write(data, 0, data.Length);
                    }
                    using (FtpWebResponse res = (FtpWebResponse)req.GetResponse())
                    {
                        Console.WriteLine("Upload File Complete, status {0}", res.StatusDescription);
                    }
                }
                return uploadUrl;
            }
            catch (Exception ex)
            {
                WriteErrorLogs(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                return string.Empty;
            }
        }

    }
    public class Data
    {
        public List<ReloanData> Reloan { get; set; }
        public string click_action { get; set; } = "FLUTTER_NOTIFICATION_CLICK";
    }
    public class ReloanData
    {
        public string Amount { get; set; }
    }

    public class Notification
    {
        /// <summary>
        /// 
        /// </summary>
        public string title { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string body { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string sound { get; set; }
    }

    public class NotificationData
    {
        /// <summary>
        /// 
        /// </summary>
        public string to { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Data data { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Notification notification { get; set; }
    }
    public class History
    {
        public const string Paid = "Paid";
        public const string Disbursement = "Disbursement";
        public const string Application = "Application";
        public const string Welcome = "Welcome";
        public const string Offer = "Offer";
        public const string Approve = "Approve";
    }
    public static class ActivityLog
    {
        private static readonly ILogger infolog;

        static ActivityLog()
        {
            infolog = new LoggerConfiguration()
              .WriteTo.File(System.Web.Hosting.HostingEnvironment.MapPath($"~/Log/ActivityLog.log"), restrictedToMinimumLevel: LogEventLevel.Verbose,
               rollingInterval: RollingInterval.Day,
               fileSizeLimitBytes: 5242880,
               rollOnFileSizeLimit: true)
               .CreateLogger();

        }
        public static void Info(string ex, string message)
        {
            infolog.Write(LogEventLevel.Information, $"[{message}]=>{ex}", message);
        }
    }

    public static class QueryLog
    {
        private static readonly ILogger infolog;

        static QueryLog()
        {
            infolog = new LoggerConfiguration()
              .WriteTo.File(System.Web.Hosting.HostingEnvironment.MapPath($"~/Log/QueryLog.log"), restrictedToMinimumLevel: LogEventLevel.Verbose,
               rollingInterval: RollingInterval.Day,
               fileSizeLimitBytes: 5242880,
               rollOnFileSizeLimit: true)
               .CreateLogger();

        }
        public static void Info(string ex, string message)
        {
            if (Convert.ToString(ConfigurationManager.AppSettings["EnableQueryLog"]) == "1")
                infolog.Write(LogEventLevel.Information, $"[{message}]=>{ex}", message);
        }
    }

    public static class Logs
    {
        private static readonly ILogger infolog;

        static Logs()
        {

            // 5 MB = 5242880 bytes

            infolog = new LoggerConfiguration()
              .WriteTo.File(System.Web.Hosting.HostingEnvironment.MapPath($"~/Log/CM.log"), restrictedToMinimumLevel: LogEventLevel.Verbose,
               rollingInterval: RollingInterval.Day,
               fileSizeLimitBytes: 5242880,
               rollOnFileSizeLimit: true)
               .CreateLogger();

        }

        public static void WriteError(string ex, string message)
        {
            //Error - indicating a failure within the application or connected system
            infolog.Write(LogEventLevel.Error, $"[{message}]=>{ex}", message);
        }

        public static void WriteWarning(string ex, string message)
        {
            //Warning - indicators of possible issues or service / functionality degradation
            infolog.Write(LogEventLevel.Warning, $"[{message}]=>{ex}", message);
        }

        public static void WriteDebug(string ex, string message)
        {
            //Debug - internal control flow and diagnostic state dumps to facilitate 
            //          pinpointing of recognised problems
            infolog.Write(LogEventLevel.Debug, $"[{message}]=>{ex}", message);
        }

        public static void WriteVerbose(string ex, string message)
        {
            // Verbose - tracing information and debugging minutiae; 
            //             generally only switched on in unusual situations
            infolog.Write(LogEventLevel.Verbose, $"[{message}]=>{ex}", message);
        }

        public static void WriteFatal(Exception ex, string message)
        {
            //Fatal - critical errors causing complete failure of the application
            infolog.Write(LogEventLevel.Fatal, $"[{message}]=>{ex.StackTrace}", message);
        }

        public static void WriteInformation(string ex, string message)
        {
            //Fatal - critical errors causing complete failure of the application
            infolog.Write(LogEventLevel.Information, $"{ex}", message);
        }

        public static string CheckBlank(this string Data)
        {
            if (String.IsNullOrWhiteSpace(Data))
            {
                return "0";
            }
            else
            {
                return Data;
            }
        }
        public static string CheckQuote(this string Data)
        {
            if (Data.Contains("'"))
            {
                return Data.Replace("'","\'");
            }
            else
            {
                return Data;
            }
        }
        public static string CheckDate(this string Data)
        {
            if (String.IsNullOrWhiteSpace(Data))
            {

                return "";
            }
            else
            {
                if (Data.Contains("/"))
                {
                    var _Date = Data.Split('/').ToList();
                    if (_Date.Count > 2)
                    {
                        return new DateTime(Convert.ToInt32(_Date[2]), Convert.ToInt32(_Date[0]), Convert.ToInt32(_Date[1])).ToString("yyyy-MM-dd");
                    }
                    return "";
                }
                return Convert.ToDateTime(Data).ToString("yyyy-MM-dd");
            }
        }
    }
    public class ManualNotificationData
    {
        public string to { get; set; }
        public Notification notification { get; set; }
    }
}

