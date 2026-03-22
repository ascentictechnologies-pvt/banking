using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Loan_CRM.Models
{
    public class SMSTemplate
    {
        [Key]
        public int id { get; set; }
        [Required(ErrorMessage = "Enter Template Name")]
        public string sms_template_name { get; set; }

        [Required(ErrorMessage = "Enter Template Definition")]
        public string sms_template_definition { get; set; }

        [Required(ErrorMessage = "Enter Template Description")]
        public string sms_template_description { get; set; }
        public string createdon { get; set; }
        public string updatedon { get; set; }
        public bool? isactive { get; set; }
    }
    public class SMSTemplateVM : SMSTemplate
    {
        public List<SMSTemplate> SMSTemplateList { get; set; }
        public SMSTemplateVM()
        {
            SMSTemplateList = new List<SMSTemplate>();
        }

    }
}