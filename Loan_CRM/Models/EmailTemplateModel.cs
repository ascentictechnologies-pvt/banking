using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Loan_CRM.Models
{
    public class EmailTemplateModel
    {
        [Key]
        public int id { get; set; }

        [Required(ErrorMessage = "Enter Email Template Name")]
        public string email_template_name { get; set; }

        [Required(ErrorMessage = "Enter Email Template Definition")]
        public string email_template_definition { get; set; }

        [Required(ErrorMessage = "Enter Email Template Description")]
        public string email_template_description { get; set; }
        public string createdon { get; set; }
        public string updatedon { get; set; }
        public bool? isactive { get; set; }
    }
    public class EmailTemplateModelVM : EmailTemplateModel
    {
        public List<EmailTemplateModel> EMailTemplateList { get; set; }
        public EmailTemplateModelVM()
        {
            EMailTemplateList = new List<EmailTemplateModel>();
        }

    }
}