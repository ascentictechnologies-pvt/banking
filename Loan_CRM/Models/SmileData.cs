using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loan_CRM.Models
{
    public class SmileData
    {
        public long dataid { get; set; }
        public string FullName { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Suffix { get; set; }
        public string DateOfBirth { get; set; }
        public string Gender { get; set; }
        public string MaritalStatus { get; set; }
        public string CountryResidence { get; set; }
        public string Citizendship { get; set; }
        public List<SmileDocument> smileDocuments { get; set; } = new List<SmileDocument>();
        public List<SmileEmployment> smileEmployments { get; set; } = new List<SmileEmployment>();
        public List<SmileLiabilities> smileLiabilities { get; set; } = new List<SmileLiabilities>();
        public List<SmileContributions> smileContributions { get; set; }=new List<SmileContributions>();
        public List<SmileEstimatedIncome> smileEstimatedIncomes { get; set; } = new List<SmileEstimatedIncome>();   
    }
    public class SmileDocument
    {
        public string DocName { get; set; }
        public string DocId { get; set; }
        public string DocType { get; set; }
        public string IssueDate { get; set; }
        public string ExpiryDate { get; set; }
        public string DocStatus { get; set; }
        public string Attachment { get; set; }
    }
    public class SmileEmployment
    {
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string JobName { get; set; }
        public string JobPosition { get; set; }
        public string JobStatus { get; set; }
        public string Department { get; set; }
        public string EmployeeCode { get; set; }
        public string Employee { get; set; }
    }
    public class SmileLiabilities {
        public string ReferenceId { get; set; }
        public string InitialLoan { get; set; }
        public string OutstandingBalance { get; set; }
        public string Overdue { get; set; }
        public string LoanType { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string Frequency { get; set; }
        public string Amount { get; set; }
        public string StartOn { get; set; }
    }
    public class SmileContributions {
        public string ReferenceId { get; set; }
        public string ContributionDate { get; set; }
        public string Currency { get; set; }
        public string Amount { get; set; }
    }
    public class SmileEstimatedIncome {
        public string IncomeMonth { get; set; }
        public string Currency { get; set; }
        public string Amount { get; set; }
    }
}
