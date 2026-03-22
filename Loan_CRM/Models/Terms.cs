namespace Loan_CRM.Models
{
    public class Terms
    {
        public int Id { get; set; }
        public int term_Id { get; set; }
        public string term_Name { get; set; }
        public string term_Value { get; set; }
        public bool isPublic { get; set; }
        public decimal Rate { get; set; }
        public decimal admin_fee { get; set; }
        public decimal late_fee { get; set; }
        public decimal late_penalty { get; set; }
        public string TermTypeName { get; set; }
        public string approved_term { get; set; }
        public string applicabletill { get; set; }
        public string suffix { get; set; }
        public bool upto10k { get; set; }
    }
}
