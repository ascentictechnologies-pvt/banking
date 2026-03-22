using System;

namespace Loan_CRM.Models
{
    public class ReportModel
    {
        public decimal approved_loan_amount { get; set; } = 0;
        public int applicationno { get; set; }
        public bool isfor_reloan { get; set; } = false;
        public bool is_preterm { get; set; } = false;
        public decimal late_penalty_amount { get; set; } = 0;
        public decimal deferment_amount { get; set; } = 0;
        public decimal paid_amount { get; set; } = 0;
        public decimal penalty_amount { get; set; } = 0;
        public decimal balanceamount { get; set; } = 0;
        public int payment_id { get; set; }

    }
    public class AgingModel
    {
        public DateTime emi_date { get; set; }
        public decimal emi_amount { get; set; }
        public decimal paidamount { get; set; }
        public decimal approved_loan_amount { get; set; }
        public decimal admin_fee { get; set; }
        public decimal balanceamount { get; set; }
        public bool iscompleted { get; set; }
        public bool isfor_reloan { get; set; }
        public bool is_preterm { get; set; }
        public bool isdefaulter { get; set; }
        public DateTime disbursement_date { get; set; }
        public DateTime? created_on { get; set; }
    }
    public class DefaultReportModel
    {
        public int emi_detail_id { get; set; }
        public DateTime emi_date { get; set; }
        public int applicationno { get; set; }
        public string personalcontactno { get; set; }
        public string personalemail { get; set; }
        public string first_name { get; set; }
        public string middle_name { get; set; }
        public int defaulter_userid { get; set; }
        public string last_name { get; set; }
        public DateTime dateapplied { get; set; }
        public string reference_no { get; set; }
        public int aging { get; set; }
        public DateTime ptp_date { get; set; }
        public double outstanding_amount { get; set; }
    }
    public class disbursementdetails
    {
        public DateTime disbursement_date { get; set; }
        public decimal disbursementamount { get; set; }
        public bool isfor_reloan { get; set; }
        public bool is_preterm { get; set; }
    }

}