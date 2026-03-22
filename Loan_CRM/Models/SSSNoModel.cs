using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Loan_CRM.Models
{

    public class SSSNoModel
    {
        public int id { get; set; }
        [Required(ErrorMessage = "Required")]
        public string sss_no { get; set; }
        public bool? isactive { get; set; }
        public string created_on { get; set; }
        public string updated_on { get; set; }
        public bool is_orr { get; set; }
    }
    public class SSSNoModelVM : SSSNoModel
    {
        public List<SSSNoModel> SSSNoModelList { get; set; }
        public SSSNoModelVM()
        {
            SSSNoModelList = new List<SSSNoModel>();
        }
    }

}