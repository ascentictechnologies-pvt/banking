using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Loan_CRM.Models
{
    public class MainData
    {
        public List<FirstGridData> firstGridData;
        public List<PaymentHistory> paymentHistories;
        public MainData()
        {
            firstGridData = new List<FirstGridData>();
            paymentHistories = new List<PaymentHistory>();
        }
    }
    public class FirstGridData
    {
        public string contractno { get; set; }
        public string date_applied { get; set; }
        public decimal loan_amount { get; set; }
        public string term_name { get; set; }
        public decimal ammortization_payment { get; set; }
        public decimal total_LatePenalties { get; set; }
        public decimal totoal_LateFee { get; set; }
        public string account_status { get; set; }
        public string Remarks { get; set; }

    }
    public class PaymentHistory
    {
        public string contractno { get; set; }
        public string Due_Date { get; set; }
        public Decimal Ammortization { get; set; }
        public string PaymentDate { get; set; }
        public decimal AmountPaid { get; set; }
        public decimal LatePaymentFee { get; set; }
        public decimal DailyPenalites { get; set; }
    }

    //public class OldDataModel
    //{
    //    public Int64 id { get; set; }
    //    public Int64 date_applied { get; set; }
    //    public Int64 applicationno { get; set; }
    //    public Int64 contractno { get; set; }
    //    public Int64 payment_ref { get; set; }
    //    public Int64 customername { get; set; }
    //    public Int64 customeremail { get; set; }
    //    public Int64 disburse_date { get; set; }
    //    public Int64 due_date1 { get; set; }
    //    public Int64 due_date2 { get; set; }
    //    public Int64 due_date3 { get; set; }
    //    public Int64 due_date4 { get; set; }
    //    public Int64 due_date5 { get; set; }
    //    public Int64 due_date6 { get; set; }
    //    public Int64 due_date7 { get; set; }
    //    public Int64 due_date8 { get; set; }
    //    public Int64 due_date9 { get; set; }
    //    public Int64 loan_amount { get; set; }
    //    public Int64 term_name { get; set; }
    //    public Int64 outstanding { get; set; }
    //    public Int64 ammortization_payment { get; set; }
    //    public Int64 paid_date1 { get; set; }
    //    public Int64 paid_date2 { get; set; }
    //    public Int64 paid_date3 { get; set; }
    //    public Int64 paid_date4 { get; set; }

    //    public Int64 paid_date5 { get; set; }
    //    public Int64 paid_date6 { get; set; }
    //    public Int64 paid_date7 { get; set; }
    //    public Int64 paid_date8 { get; set; }
    //    public Int64 paid_date9 { get; set; }
    //    public Int64 paid_amount1 { get; set; }
    //    public Int64 paid_amount2 { get; set; }
    //    public Int64 paid_amount3 { get; set; }
    //    public Int64 paid_amount4 { get; set; }
    //    public Int64 paid_amount5 { get; set; }
    //    public Int64 paid_amount6 { get; set; }
    //    public Int64 paid_amount7 { get; set; }
    //    public Int64 paid_amount8 { get; set; }
    //    public Int64 paid_amount9 { get; set; }
    //    public Int64 late_payment1 { get; set; }
    //    public Int64 late_payment2 { get; set; }
    //    public Int64 late_payment3 { get; set; }
    //    public Int64 late_payment4 { get; set; }
    //    public Int64 late_payment5 { get; set; }
    //    public Int64 late_payment6 { get; set; }
    //    public Int64 late_payment7 { get; set; }
    //    public Int64 late_payment8 { get; set; }
    //    public Int64 late_payment9 { get; set; }
    //    public Int64 penalty1 { get; set; }
    //    public Int64 penalty2 { get; set; }
    //    public Int64 penalty3 { get; set; }
    //    public Int64 penalty4 { get; set; }
    //    public Int64 penalty5 { get; set; }
    //    public Int64 penalty6 { get; set; }
    //    public Int64 penalty7 { get; set; }
    //    public Int64 penalty8 { get; set; }
    //    public Int64 penalty9 { get; set; }
    //    public Int64 remain_balance { get; set; }
    //    public Int64 total_paid { get; set; }
    //    public Int64 account_status { get; set; }
    //    public Int64 remarks { get; set; }

    //}
}