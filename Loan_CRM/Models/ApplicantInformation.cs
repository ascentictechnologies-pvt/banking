using System;

namespace Loan_CRM.Models
{
    public class ApplicantInformation
    {
        public int dateapplied { get; set; }
        public int age { get; set; }
        public string name { get; set; }
        public string personalemail { get; set; }
        public Int64 personalcontactno { get; set; }
        public int secondcontactno { get; set; }
        public string address { get; set; }
        public Int64 homephoneno { get; set; }
        public String placeofbirth { get; set; }
        public int dateofbirth { get; set; }
        public int civilstatus { get; set; }
        public string mothermaidenname { get; set; }
        public string motheraddress { get; set; }
        public string companyname { get; set; }
        public string companyaddress { get; set; }

        public string designation { get; set; }
        public double gross_income { get; set; }

        public string reference_name { get; set; }
        public Int64 reference_contactno { get; set; }
        public string bankname { get; set; }

        public int bankaccountno { get; set; }

        public string employername { get; set; }

        public string employeraddress { get; set; }

        public string employercontectnumber { get; set; }

        public DateTime paydate { get; set; }

    }
}