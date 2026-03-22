using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Loan_CRM.Models
{
    public class BankMasterModel
    {
        public int bank_id { get; set; }
        [Required(ErrorMessage = "Bank Code Required")]
        [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "Only Charcter Allowed")]
        public string bank_code { get; set; }
        [Required(ErrorMessage = "Bank Name Required")]
        [RegularExpression(@"^[a-zA-Z ]+$", ErrorMessage = "Only Charcter Allowed")]
        public string bank_name { get; set; }
        public string bank_name_with_bank_code { get; set; }
        public bool? is_active { get; set; }
        public string created_on { get; set; }
        public string updated_on { get; set; }
    }
    public class BankMasterModelVM : BankMasterModel
    {
        public List<BankMasterModel> BankMasterModelList { get; set; }
        public BankMasterModelVM()
        {
            BankMasterModelList = new List<BankMasterModel>();
        }
    }
}