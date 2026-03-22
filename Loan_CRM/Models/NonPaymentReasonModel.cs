using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Loan_CRM.Models
{
    public class NonPaymentReasonModel
    {
        public int reason_id { get; set; }
        [Required(ErrorMessage = "Reason Name Required")]
        [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "Only Charcter Allowed")]
        public string reason_text { get; set; }
        public bool? isactive { get; set; }
        public string created_on { get; set; }
        public string updated_on { get; set; }
    }
    public class NonPaymentReasonModelVM : NonPaymentReasonModel
    {
        public List<NonPaymentReasonModel> NonPaymentReasonModelList { get; set; }
        public NonPaymentReasonModelVM()
        {
            NonPaymentReasonModelList = new List<NonPaymentReasonModel>();
        }
    }
}