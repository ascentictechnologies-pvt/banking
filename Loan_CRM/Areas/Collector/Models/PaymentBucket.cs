using System;
using System.Collections.Generic;

namespace Loan_CRM.Areas.Collector.Models
{
    public class PaymentBucket
    {
        public int Id { get; set; }
        public Int64 applicationno { get; set; }
        public string applicationname { get; set; }
        public string RequestDate { get; set; }
        public string Detail { get; set; }
        public string personalcontactno { get; set; }
        public string PTPDate { get; set; }
        public string ContractNo { get; set; }
        public string EmiDate { get; set; }
        public string Priority { get; set; }
        public int _BalanceAmount { get; set; }
        public int EMIDetailsID { get; set; }
        public bool IsPickedForCollector { get; set; }
        public string MorningTime { get; internal set; }
        public string NoonTime { get; internal set; }
        public string Aging { get; set; }
        public double PaidAmount { get; set; }
        public double OutstandingAmount { get; set; }
        public string ReferenceNumber { get; set; }
        public int SortingOrder { get; set; }
        public int reasonid { get; set; }
        public string reason_text { get; set; }
        public int emi_id { get; set; }
        public int emi_detail_id { get; set; }
        public string emi_date { get; set; }
        public Double balanceamount { get; set; }
        public double penalty { get; set; }
        public string termtype { get; set; }
        public int pastduedays { get; set; }
        public string emiamount { get; set; }
        public bool isdefaulter { get; set; }
        public List<PaymentBucket> paymentbucket { get; set; }

        public PaymentBucket()
        {
            paymentbucket = new List<PaymentBucket>();
        }

    }
}
