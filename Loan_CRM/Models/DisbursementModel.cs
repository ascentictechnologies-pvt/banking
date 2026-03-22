using System;
using System.Collections.Generic;

namespace Loan_CRM.Models
{
    public class DisbursementModel
    {
        public int Id { get; set; }
        public int ApplicationNo { get; set; }
        public string DateApplied { get; set; }
        public string Name { get; set; }
        public string ApprovedOn { get; set; }

        public double GrossIncome { get; set; }
        public string TermComplete { get; set; }
        public int Loan_Amount { get; set; }
        public string GovUrl { get; set; }

        public string BankName_or_Status { get; set; }
        public string BankAccountNumber_or_Status { get; set; }
        public string ContactNumber { get; set; }

        public string FromDate { get; set; }
        public string ToDate { get; set; }

        public List<DisbursementModelDetails> DisbursementModelDetails { set; get; }

        public DisbursementModel()
        {
            DisbursementModelDetails = new List<DisbursementModelDetails>();
        }
    }
    public class DisbursementModelDetails
    {
        public int Id { get; set; }
        public int ApplicationNo { get; set; }
        public string DateApplied { get; set; }
        public DateTime Date_Applied { get; set; }
        public string Name { get; set; }
        public DateTime approved_on { get; set; }
        public string ApprovedOn { get; set; }
        public int user_id { get; set; }
        public double GrossIncome { get; set; }
        public string TermComplete { get; set; }
        public decimal Loan_Amount { get; set; }
        public string GovUrl { get; set; }
        public string AtmUrl { get; set; }
        public string BankName_or_Status { get; set; }
        public string BankAccountNumber_or_Status { get; set; }
        public string ContactNumber { get; set; }
        public string ContractNumber { get; set; }
        public string First_Name { get; set; }
        public string proofofpayment { get; set; }
        public string ReferenceNumber { get; set; }
        public string DateofPayment { get; set; }
        public DateTime PaymentDate { get; set; }
        public string LatePenalties { get; set; }
        public string PaymentChannel { get; set; }
        public string remarks { get; set; }
        public string TotalAmount { get; set; }
        public decimal Amortization { get; set; }
        public int PaymentId { get; set; }
        public string DisbursementDate { get; set; }
        public string Disbursement_FullDate { get; set; }
        public int EmiPayment { get; set; }
        public string checker_remark { get; set; }
        public decimal AmountToDisbursement { get; set; }
    }
}