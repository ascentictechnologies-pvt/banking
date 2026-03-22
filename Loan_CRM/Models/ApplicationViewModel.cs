namespace Loan_CRM.Models
{
    public class ApplicationViewModel
    {
        public ApplicationVerificationDetails ApplicationVerificationDetails;
        public ApplicationRecordVM ApplicationRecordVM;
        public ApplicationViewModel()
        {
            ApplicationVerificationDetails = new ApplicationVerificationDetails();
            ApplicationRecordVM = new ApplicationRecordVM();
        }
    }
}