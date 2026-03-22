using System;
using System.ComponentModel.DataAnnotations;

namespace Loan_CRM.Models
{
    public class RemarkModel
    {
        [Key]
        public int id { get; set; }
        public DateTime createdon { get; set; }
        public int createdby { get; set; }
        public string createdbyname { get; set; }
        public int application_id { get; set; }
        public string remark { get; set; }
        public string remarkon { get; set; }
        public string remarkidentifier { get; set; }
    }
}