using System;
using System.ComponentModel.DataAnnotations;

namespace Loan_CRM.Models
{
    public class City
    {
        [Key]
        public int cityid { get; set; }
        public string cityname { get; set; }
        public int provinceId { get; set; }
        public bool is_orr { get; set; }
        public DateTime createdon { get; set; }
        public DateTime updatedon { get; set; }
        public bool? isactive { get; set; }
    }
}