using Loan_CRM.Models;
using System;
using System.Collections.Generic;

namespace Loan_CRM.Areas.Collector.Models
{
    public class PaymentInformation
    {
        public string templatename { get; set; }
        public string notificationtoken { get; set; }
        public string facemapstatus { get; set; }
        public string jumioreference { get; set; }
        public string Aging { get; set; }
        public String emiamount { get; set; }
        public int EMIDetailsID { get; set; }
        public int SortingOrder { get; set; }
        public string Paidamount { get; set; }
        public string completeon { get; set; }
        public String Balanceamount { get; set; }
        public string PayableAmount { get; set; }
        public String TotalBalanceamount { get; set; }
        public Int64 emistatus { get; set; }
        public string Priority { get; set; }
        public Int64 emidetailid { get; set; }
        public String Name { get; set; }
        public String txtdatepaid { get; set; }
        public String txtamountpaid { get; set; }
        public String whatpaymnetchannel { get; set; }

        public String proofofpayment { get; set; }

        public String contactno { get; set; }

        public String updatedby { get; set; }

        public String txtbesttimetocallback { get; set; }


        public String promisepay { get; set; }

        public String ptpamount { get; set; }

        public String nonpaymentreson { get; set; }

        public String txtotherremarks { get; set; }


        public Int64 emiid { get; set; }

        public String existid { get; set; }
        public Int64 applicationno { get; set; }

        public string personalcontactno { get; set; }

        public Int64 EmpID { get; set; }
        public string Referenceno { get; set; }
        public string Dateofdistursement { get; set; }
        public string Dueedate { get; set; }

        public string Term { get; set; }

        public string Termtype { get; set; }

        public string disbursemant { get; set; }

        public string nonpaymentreason { get; set; }

        public string currentdateptp { get; set; }

        public string currentamountptp { get; set; }
        public string Amountdue { get; set; }
        public string Whatpaymentchannel { get; set; }
        public string Profofpayment { get; set; }
        public string Besttimetocallback { get; set; }
        public string Promisetopay { get; set; }
        public string Resonofnonpayment { get; set; }
        public string Othersremark { get; set; }
        public string Contractno { get; set; }

        public string Datepaid { get; set; }

        public string Amountpaid { get; set; }

        public string timetocallback { get; set; }

        public string Promisetopaydate { get; set; }

        public string Amountptp { get; set; }

        public string Reasonofnonpayment { get; set; }

        public string Otherremarks { get; set; }

        public string Dateandtimeofsms { get; set; }

        public string Dateandtimeofmsg { get; set; }

        public string Dateandtimeoflastcall { get; set; }

        public string Nooftimecalls { get; set; }
        public string totalamountpaid { get; set; }
        public string totalemiamount { get; set; }
        public string balanceamt { get; set; }

        public Int64 existtermdata { get; set; }

        public string Historydatepaid { get; set; }

        public string HistoryAmountPaid { get; set; }

        public string HistoryBestTimeToCallBack { get; set; }

        public string HistoryPTP { get; set; }


        public string HistoryAmountPTP { get; set; }

        public string HistoryReasonOfNonPayment { get; set; }

        public string HistoryOtherRemarks { get; set; }

        //public string HistoryDateandTimeThreedays { get; set; }

        public string HistoryDateandTimeVoice { get; set; }

        public string HistoryDateandTimeLastCall { get; set; }

        public string HistoryNoOfTimeCall { get; set; }
        public string EmailId { get; set; }

        //New Changes
        public string verifier { get; set; }
        public string checker { get; set; }
        public string familyname1 { get; set; }
        public string familyrelation1 { get; set; }
        public string familycontact1 { get; set; }
        public string familyname2 { get; set; }
        public string familyrelation2 { get; set; }
        public string familycontact2 { get; set; }
        public string ptpdate { get; set; }
        public string amortization { get; set; } = "0";
        public decimal outstanding { get; set; }
        public string principal { get; set; }
        public string paymentdate { get; set; }
        public string emidate { get; set; }
        public double emi_amount { get; set; }
        public string termtype { get; set; }
        public double penalty { get; set; }
        public double pastduedays { get; set; }
        public int ReduceLoanCount { get; set; }
        public int ReloanCount { get; set; }
        public decimal LoanAmount { get; set; }
        public double LateFee { get; set; }
        public decimal AmmortizationPaid { get; set; }
        public decimal AmmortizationToBePaid { get; set; }
        public string paiddate { get; set; }
        public string ApprovedTerm { get; set; }
        public Double AmortizationAmount { get; set; }
        public int UserID { get; set; }

        public string _LateFee { get; set; } = "0";
        public string _Penality { get; set; } = "0";
        public string _DueAmount { get; set; } = "0";
        public List<RemarkModel> RemarkModelList { get; set; }
        public List<PaymentInformation> paymentinformation { get; set; }
        public List<PaymentInformation> LoanHistory { get; set; }
        public List<string> PaymentHistoryBucketColumnList { get; set; }
        public List<PaymentChannel> PaymentChannelList { get; set; }
        public List<ProofOfPayment> ProofOfPaymentList { get; set; }

        public PaymentInformation()
        {
            RemarkModelList = new List<RemarkModel>();
            PaymentHistoryBucketColumnList = new List<string>();
            paymentinformation = new List<PaymentInformation>();
            PaymentChannelList = new List<PaymentChannel>();
            ProofOfPaymentList = new List<ProofOfPayment>();
            LoanHistory = new List<PaymentInformation>();
        }

    }

    public class PaymentChannel
    {
        public int id { get; set; }
        public string payment_channel { get; set; }
        public string created_on { get; set; }
        public string updated_on { get; set; }
        public int created_by { get; set; }
        public int updated_by { get; set; }
        public bool isactive { get; set; }
    }

    public class ProofOfPayment
    {
        public int id { get; set; }
        public string proof_of_payment { get; set; }
        public string created_on { get; set; }
        public string updated_on { get; set; }
        public int created_by { get; set; }
        public int updated_by { get; set; }
        public bool isactive { get; set; }
    }
}