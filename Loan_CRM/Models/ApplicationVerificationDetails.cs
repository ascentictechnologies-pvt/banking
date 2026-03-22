namespace Loan_CRM.Models
{
    public class ApplicationVerificationDetails
    {
        public int DetailId { get; set; }
        public int ApplicationId { get; set; }
        public int RequestAmount { get; set; }
        public int RequestTerm { get; set; }
        public bool Id1_Status { get; set; }
        public bool Id2_Status { get; set; }
        public bool PI_Status { get; set; }
        public bool PB_Status { get; set; }
        public string Remark { get; set; }
        public string Addtional_Birth_Place { get; set; }
        public string Additonal_Provincial_Address { get; set; }
        public string Additional_Loan_Purpose { get; set; }
        public decimal Additional_Requested_Loan_Amount { get; set; }
        public decimal Additional_Requested_Term { get; set; }
        public decimal Additional_Employed_Duration { get; set; }
        public string Additional_Job_Position { get; set; }
        public string Additional_Job_Level { get; set; }
        public string Additional_Paydate { get; set; }
        public bool Family_Personal_Name_Status { get; set; }
        public bool Family_Personal_Contact_Status { get; set; }
        public bool Family_Relation_With_Borrower_Status { get; set; }
        public bool Family_Borrower_Known_Duration_Status { get; set; }
        public bool Family_Address_Verification_Status { get; set; }
        public bool Family_Borrower_Working_Place_Status { get; set; }
        public bool Friend_Personal_Name_Status { get; set; }
        public bool Friend_Personal_Contact_Status { get; set; }
        public bool Friend_Relation_With_Borrower_Status { get; set; }

        public bool Friend_Borrower_Known_Duration_Status { get; set; }

        public bool Friend_Address_Verification_Status { get; set; }

        public bool Friend_Borrower_Working_Place_status { get; set; }
        public bool Co_worker_Personal_Name_Status { get; set; }
        public bool Co_worker_Personal_Contact_Status { get; set; }
        public bool Co_worker_Relation_With_Borrower_Status { get; set; }
        public bool Co_worker_Borrower_Known_Duration_Status { get; set; }

        public bool Co_worker_Address_Verification_Status { get; set; }
        public bool Co_worker_Borrower_Working_Place_Status { get; set; }
        public bool Employement_Nameof_Work_Contact_Status { get; set; }
        public bool Employement_no_of_Work_Contact_Status { get; set; }
        public bool Employement_Borrower_Working_Status { get; set; }
        public bool Employement_Borrower_Position_Status { get; set; }
        public bool Employement_Borrower_Monthly_Salary_Status { get; set; }
        public bool Employement_Borrower_Attendance_Status { get; set; }
        public bool Employement_Borrower_Bank_Payroll_Status { get; set; }
        public string Family_Personal_Name { get; set; }
        public string Family_Personal_Contact { get; set; }
        public string Family_Relation_With_Borrower { get; set; }
        public string Family_Borrower_Known_Duration { get; set; }
        public string Family_Address_Verification { get; set; }
        public string Family_Borrower_Working_Place { get; set; }
        public string Friend_Personal_Name { get; set; }
        public string Friend_Personal_Contact { get; set; }
        public string Friend_Relation_with_Borrower { get; set; }
        public string Friend_Borrower_Known_Duration { get; set; }
        public string Friend_Address_Verification { get; set; }
        public string Friend_Borrower_Working { get; set; }
        public string Co_worker_Personal_Name { get; set; }
        public string Co_worker_Personal_Contact { get; set; }
        public string Co_worker_Relation_With_Borrower { get; set; }
        public string Co_worker_Borrower_Known_Duration { get; set; }
        public string Co_worker_Address_Verification { get; set; }
        public string Co_worker_Borrower_Working { get; set; }
        public string Employement_Nameof_Work_Contact { get; set; }
        public string Employement_no_of_work_Contact { get; set; }
        public string Employement_Borrower_Working { get; set; }
        public string Employement_Borrower_Position { get; set; }
        public decimal Employement_Borrower_Monthly_Salary { get; set; }
        public string Employement_Borrower_Attendance { get; set; }
        public string Employement_Borrower_Bank_Payroll { get; set; }
        public bool Question1_Status { get; set; }
        public bool Question2_Status { get; set; }
        public bool Question3_Status { get; set; }
        public bool Question4_Status { get; set; }
        public bool Question5_Status { get; set; }
        public bool Question6_Status { get; set; }
        public bool Question7_Status { get; set; }
        public bool Question8_Status { get; set; }
        public bool Question9_Status { get; set; }
        public bool Question10_Status { get; set; }
        public decimal Approved_Loan_Amount { get; set; }
        public decimal Approved_Term { get; set; }
        public decimal Approved_Interest_Rate { get; set; }
        public string Approved_Maturity_Date { get; set; }
        public decimal Approved_Total_Amount_Due { get; set; }



    }
}