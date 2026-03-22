using System.Collections.Generic;

namespace Loan_CRM.Models
{
    public class TeamAgentMappingModel
    {
        public int id { get; set; }
        public int tl_id { get; set; }
        public int agent_id { get; set; }
        public bool isactive { get; set; }
        public int created_by { get; set; }
        public int updated_by { get; set; }
        public string created_on { get; set; }
        public string updated_on { get; set; }
    }
    public class TeamAgentMappingVM : TeamAgentMappingModel
    {
        public List<TeamAgentMappingModel> TeamAgentMappingModelList { get; set; }
        public int type { get; set; }
        public TeamAgentMappingVM()
        {
            TeamAgentMappingModelList = new List<TeamAgentMappingModel>();
        }
    }
}