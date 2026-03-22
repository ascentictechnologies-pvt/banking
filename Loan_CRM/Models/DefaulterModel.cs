using System;
using System.Collections.Generic;

namespace Loan_CRM.Models
{
    public class DefaulterModel
    {
        public int aging { get; set; }
        public int defaulter_id { get; set; }
        public Int64 application_no { get; set; }
        public string name { get; set; }
        public string refernce_no { get; set; }
        public double outstanding_amount { get; set; }
        public double latepenalty { get; set; }
        public double latefee { get; set; }
        public double penalty { get; set; }
        public string personalcontactno { get; set; }
        public string personal_email { get; set; }
        public string ProofOfPayment { get; set; }
        public bool isptp { get; set; }
        public string PaymentChannel { get; set; }
        public string complete_outstanding_amount { get; set; }
        public DateTime ptpdate { get; set; }
        public int EMIDetailsID { get; set; }
        public List<ProofOfPayment> ProofOfPaymentList { get; set; }
        public List<PaymentChannel> PaymentChannelList { get; set; }
        public DefaulterModel()
        {
            ProofOfPaymentList = new List<ProofOfPayment>();
            PaymentChannelList = new List<PaymentChannel>();
        }
    }
    public class DefaulterModelVM : DefaulterModel
    {
        public List<DefaulterModel> DefaulterModelList { get; set; }
        public DefaulterModelVM()
        {
            DefaulterModelList = new List<DefaulterModel>();
        }
    }

    public class FieldVisitReport
    {
        public int application_no { get; set; }
        public int defaulter_id { get; set; }
        public string applicant_name { get; set; }
        public string address { get; set; }
        public string escalated_date { get; set; }
        public string date_past_due { get; set; }
        public string maturity_date { get; set; }
        public string date_of_loan { get; set; }
        public double outstanding_amount { get; set; }
        public double total_amount_due { get; set; }
        public string personal_email { get; set; }
        public string payment_refernce { get; set; }
        public string phone_no { get; set; }
        public double latepenalty { get; set; }
        public string completedue { get; set; }
    }

    public class LODModel
    {
        public int id { get; set; }
        public string lod_path { get; set; }
        public int application_id { get; set; }
        public string created_on { get; set; }
    }

}
