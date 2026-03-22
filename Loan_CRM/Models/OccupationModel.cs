using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Loan_CRM.Models
{
    public class OccupationModel
    {
        public int id { get; set; }
        [Required(ErrorMessage = "Required")]
        [RegularExpression(@"^[a-zA-Z ]+$", ErrorMessage = "Only Charcter Allowed")]
        public string occupation_name { get; set; }
        public bool? isactive { get; set; }
        public string created_on { get; set; }
        public string updated_on { get; set; }
        public string optionGroup { get; set; }
        public bool is_orr { get; set; }
    }
    public class OccupationModelVM : OccupationModel
    {
        public List<OccupationModel> OccupationModelList { get; set; }
        public OccupationModelVM()
        {
            OccupationModelList = new List<OccupationModel>();
        }
    }
}