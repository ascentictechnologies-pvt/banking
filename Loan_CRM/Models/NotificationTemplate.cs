using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Loan_CRM.Models
{
    public class NotificationTemplate
    {
        [Key]
        public int id { get; set; }
        [Required(ErrorMessage = "Enter Template Name")]
        public string NotificationTemplateName { get; set; }

        [Required(ErrorMessage = "Enter Template Definition")]
        public string NotificationTemplateDefinition { get; set; }

        [Required(ErrorMessage = "Enter Template Description")]
        public string NotificationTemplateDesc { get; set; }
        public string createdon { get; set; }
        public string updatedon { get; set; }
        public bool? isactive { get; set; }
    }
    public class NotificationTemplateVM : NotificationTemplate
    {
        public List<NotificationTemplate> NotificationTemplateList { get; set; }
        public NotificationTemplateVM()
        {
            NotificationTemplateList = new List<NotificationTemplate>();
        }

    }
}