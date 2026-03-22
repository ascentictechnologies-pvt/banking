using System;
using System.Collections.Generic;

namespace Loan_CRM.Models
{
    public class FSBucket
    {
        public int Application_No { get; set; }
        public string FS_Date { get; set; }
        public string Name { get; set; }
        public string LoanTerm { get; set; }
        public int TermId { get; set; }
        public Double Amount { get; set; }
        public int Records { get; set; }
        public bool Is_Reduce_Loan { get; set; }
        public Double BalanceAmount { get; set; }
        public List<FSBucket> FSBucketList { get; set; }
        public string PersonalContactNumber { get; set; }
        public int template_name { get; set; }
        public string personal_email { get; set; }
        public bool Reloan { get; set; }
        public bool Preterm { get; set; }
        public string RecordName { get; set; }
        public FSBucket()
        {
            FSBucketList = new List<FSBucket>();
        }
    }

}