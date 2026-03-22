using System.Collections.Generic;

namespace Loan_CRM.Areas.Account.Models
{
    public class ProfileSummery
    {
        public int applicationno { get; set; }

        public string Contactno { get; set; }

        public string Referenceno { get; set; }

        public string ClientName { get; set; }

        public string Dateofreleased { get; set; }

        public string MaturityDate { get; set; }

        public string Principleloan { get; set; }
        public string term { get; set; }

        public string weeklyterm { get; set; }
        public string interestrate { get; set; }
        public string Osbalance { get; set; }

        public string Installmentpayment { get; set; }
        public string action { get; set; }
        public string DateApplied { get; set; }
        public string PersonalEmail { get; set; }
        public string PersonalContactNo1 { get; set; }
        public string PersonalContactNo2 { get; set; }
        public string PersonalContactNo3 { get; set; }
        public string Address { get; set; }
        public string HomePhone { get; set; }
        public string PlaceOfBirth { get; set; }
        public string DOB { get; set; }
        public string CivilStatus { get; set; }
        public string MotherMaidenName { get; set; }
        public string MotherAdd { get; set; }
        public string CompanyName { get; set; }
        public string CompanyAdd { get; set; }
        public string Position { get; set; }
        public decimal GrossIncome { get; set; }
        public decimal PenaltyAmt1 { get; set; }
        public decimal PenaltyAmt2 { get; set; }
        public decimal PenaltyAmt3 { get; set; }
        public decimal PenaltyAmt4 { get; set; }
        public decimal PenaltyAmt5 { get; set; }
        public decimal Balance { get; set; }
        public decimal AmountPaid { get; set; }
        public string BestTimeToCall { get; set; }
        public string PTP { get; set; }
        public string NonPaymentReason { get; set; }
        public string OtherRemarks { get; set; }
        public string Notes { get; set; }
        public string PhoneStatus { get; set; }
        public string NoOfTimesDialed { get; set; }
        public string RefName { get; set; }
        public string termtype { get; set; }
        public int term_value { get; set; }
        public bool IsApproved { get; set; }
        public int EmiStatus { get; set; }
        public bool IsPickedForAccount { get; set; }
        public List<TermsWeekRecord> weekRecords { get; set; }
    }

    public class TermsWeekRecord
    {
        public string date { get; set; }
        public string principle { get; set; }
        public string rate { get; set; }
        public string emiamount { get; set; }
    }
}