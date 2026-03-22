using System;

namespace Loan_CRM.Models
{
    public class AppPaymentModel
    {
        public int PaymentId { get; set; }
        public string ApplicationNo { get; set; }
        public string PaidOn { get; set; }
        public String ImageUrl { get; set; }
        public bool ischecked { get; set; }
        public string PartnerName { get; set; }
    }
}