using System;
using System.Collections.Generic;

namespace Loan_CRM.Models
{
    public class NewReferenceApprovalModel
    {

        public int id { get; set; }
        public int application_no { get; set; }
        public string old_reference_no { get; set; }
        public string new_reference_no { get; set; }
        public string userfullname { get; set; }
        public DateTime? approved_on { get; set; }
        public int edited_by { get; set; }
        public bool is_approved { get; set; }

        public List<NewReferenceApprovalModel> NewReferenceList { get; set; }

        public NewReferenceApprovalModel()
        {
            NewReferenceList = new List<NewReferenceApprovalModel>();
        }
    }
}