using Microsoft.SqlServer.Server;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Loan_CRM.Models
{
    public class BarangayModel
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Required")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Only Charcter Allowed")]
        public string Barangay_Name { get; set; }
        public int cityId { get; set; }
        public bool Is_ORR { get; set; }
        public string created_on { get; set; }
        public string updated_on { get; set; }
        public bool? is_active { get; set; }
        public string cityname { get; set; }
    }

    public class BarangayModelVM : BarangayModel
    {
        public List<BarangayModel> BarangayModelList { get; set; }
        public List<City> CityList { get; set; }
        public BarangayModelVM()
        {
            BarangayModelList = new List<BarangayModel>();
            CityList = new List<City>();
        }

    }
}