using System;
using System.ComponentModel.DataAnnotations;

namespace Loan_CRM.Models
{
    public class ProvinceModel
    {
        [Key]
        public int Id { get; set; }
        public string Province_Name { get; set; }
        public bool Is_ORR { get; set; }
        public DateTime createdon { get; set; }
        public DateTime updatedon { get; set; }
        public bool? isactive { get; set; }
    }
}