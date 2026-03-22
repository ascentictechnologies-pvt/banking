using System;
using System.Collections.Generic;

namespace Loan_CRM.Models
{
    public class DisbursementCollection
    {
        public DateTime From_Date { get; set; }
        public string Month_Name { get; set; }
        public decimal New_Loan_Amount { get; set; }
        public decimal Reloan_Amount { get; set; }
        public decimal Total_Amount { get; set; }
        public long New_Loan_Count { get; set; }
        public long Reloan_count { get; set; }
        public long Total_count { get; set; }
        public decimal Collected_Amount_Month { get; set; }
        public decimal Fully_Paid_new_Loan_Amount { get; set; }
        public decimal Fully_Paid_ReLoan_Amount { get; set; }
        public decimal Late_Penalties { get; set; }
        public decimal Total_Amount_New_Laon_Reloan { get; set; }
        public long Fully_Paid_new_Loan_Count { get; set; }
        public long Fully_Paid_ReLoan_Count { get; set; }
        public long Total_Count_New_Loan_Reloan { get; set; }
        public string createdon { get; set; }
        public string updatedon { get; set; }
        public bool? isactive { get; set; }
        public decimal SumNewLoan_Peso { get; set; }
        public decimal SumReloan_Peso { get; set; }
        public decimal SumTotal_Peso { get; set; }
        public long SumNewLoan_No { get; set; }
        public long SumReloan_No { get; set; }
        public long SumTotal_No { get; set; }
        public decimal SumCollect_Amount { get; set; }
        public decimal SumFullyPaid_NewLoan_Peso { get; set; }
        public decimal SumFullyPaid_Relaon_Peso { get; set; }
        public decimal SumLate_Penalties_Peso { get; set; }
        public decimal SumTotal_NewLoan_Reloan_Peso { get; set; }
        public long SumFullyPaid_ReLoan_No { get; set; }
        public long SumFullyPaid_NewLoan_No { get; set; }
        public long Sumtotal_newLoan_reloan_No { get; set; }

        public List<DisbursementCollection> disbursementCollectionslist { get; set; }

        public DisbursementCollection()
        {
            disbursementCollectionslist = new List<DisbursementCollection>();
        }
    }

}