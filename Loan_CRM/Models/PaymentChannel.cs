using System;
using System.ComponentModel.DataAnnotations;

namespace Loan_CRM.Models
{
    public class PaymentChannel
    {
        [Key]
        public int id { get; set; }
        public string payment_channel { get; set; }
        public DateTime created_on { get; set; }
        public int created_by { get; set; }
        public DateTime updated_on { get; set; }
        public int updated_by { get; set; }
        public bool isactive { get; set; }
    }
}