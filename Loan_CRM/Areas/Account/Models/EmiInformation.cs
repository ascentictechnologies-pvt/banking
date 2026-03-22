using System.Collections.Generic;

namespace Loan_CRM.Areas.Account.Models
{
    public class EmiInformation
    {
        public string notificationtoken { get; set; }
        public string templatename { get; set; }
        public int user_id { get; set; }
        public int applicationno { get; set; }
        public int emistatus { get; set; }
        public string new_referenceno { get; set; }
        public string referenceno { get; set; }
        public string disbursement_date { get; set; }
        public string thirdweekfirstemiamount { get; set; }
        public string thirdweeksecoundemiamount { get; set; }
        public string thirdweekthirdemiamount { get; set; }
        public string fourthweekemiamount { get; set; }
        public string fiveweekemiamount { get; set; }
        public string bifirstweekemiamount { get; set; }
        public string bisecoundweekemiamount { get; set; }
        public string monthlyemiamount { get; set; }
        public string updatedby { get; set; }
        public string besttimetocall { get; set; }
        public string ptp { get; set; }
        public string resonofnonpayment { get; set; }
        public string otherremarks { get; set; }
        public string notes { get; set; }
        public string phonestatus { get; set; }

        public string nooftimesdialed { get; set; }

        public string dateapplied { get; set; }

        public string personalemail { get; set; }

        public string personalcontactnoone { get; set; }

        public string personalcontactnotwo { get; set; }

        public string personalcontactnothree { get; set; }

        public string address { get; set; }

        public string homephonenoifany { get; set; }

        public string placeofbirth { get; set; }

        public string dateofbirth { get; set; }

        public string civilstatus { get; set; }

        public string mothermaidenname { get; set; }

        public string mothersaddress { get; set; }

        public string companyname { get; set; }

        public string companyaddress { get; set; }

        public string position { get; set; }

        public string grossincome { get; set; }

        public string referancename { get; set; }

        public string referancecontactno { get; set; }

        public string bankname { get; set; }

        public string bankaccountno { get; set; }

        public string balanceamt { get; set; }


        public string totalamountpaid { get; set; }

        public string totalemiamount { get; set; }

        public string panalityamountfirst { get; set; }

        public string panalityamountsecound { get; set; }

        public string panalityamountthird { get; set; }

        public string panalityamountfour { get; set; }

        public string panalityamountfive { get; set; }

        public string thirdweekfirstdate { get; set; }

        public string thirdweeksecounddate { get; set; }

        public string thirdweekthirddate { get; set; }

        public string thirdweekfirstprinciple { get; set; }

        public string thirdweeksecoundprinciple { get; set; }

        public string thirdweekthirdprinciple { get; set; }

        public string thirdweekfirstrate { get; set; }

        public string thirdweeksecoundrate { get; set; }

        public string thirdweekthirdrate { get; set; }

        public string fourthweekdate { get; set; }

        public string fourthweekprinciple { get; set; }

        public string fourthweekrate { get; set; }

        public string fiveweekdate { get; set; }

        public string fiveweekprinciple { get; set; }

        public string fiveweekrate { get; set; }

        public string bifirstweekdate { get; set; }

        public string bifirstweekprinciple { get; set; }

        public string bifirstweekrate { get; set; }

        public string bisecoundweekdate { get; set; }

        public string bisecoundweekprinciple { get; set; }

        public string bisecoundweekrate { get; set; }

        public string monthlydate { get; set; }

        public string monthlyprinciple { get; set; }

        public string monthlyrate { get; set; }
        public string gov_id_url { get; set; }

        public string companyid_url { get; set; }

        public string billing_url { get; set; }

        public string income_url { get; set; }

        public string other_url { get; set; }

        public string atm_url { get; set; }
        public string term_type { get; set; }

        public string term { get; set; }
        public string emiamount { get; set; }

        public string term_name { get; set; }
        public decimal ApprovedLoanAmt { get; set; }

        public string term_value { get; set; }
        public List<TermsRecord> Records { get; set; }
        public string street { get; internal set; }
        public string barangay { get; internal set; }
        public string province_name { get; internal set; }
        public string cityname { get; internal set; }
        public string zipcode { get; internal set; }
        public string contract_ref_no { get; internal set; }

        public string ApplicantName { get; set; }
        public string DateOfPayment { get; set; }
        public decimal PaidAmount { get; set; }
        public string PaymentChannel { get; set; }
        public string ProofOfPayment { get; set; }
        public bool PaymentApproved { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsCompleted { get; set; }
        public int PaymentId { get; set; }
        public decimal LatePenalty { get; set; }
        public string Remarks { get; set; }

        public List<EmiInformationDetails> EmiInformationDetail { get; set; }

        public EmiInformation()
        {
            EmiInformationDetail = new List<EmiInformationDetails>();
        }

        public class EmiInformationDetails
        {
            public string Date { get; set; }
            public string Principle { get; set; }
            public string Rate { get; set; }
            public string EmiAmount { get; set; }
        }
    }

    public class TermsRecord
    {
        public string fourthweekdate { get; set; }
        public string fourthweekprinciple { get; set; }
        public string fourthweekrate { get; set; }
        public string fourthweekemiamount { get; set; }
        public int emistatus { get; set; }
    }

    //Start - JSon class sent from Datatables

    public class DataTableAjaxPostModel
    {
        // properties are not capital due to json mapping
        public int draw { get; set; }
        public int start { get; set; }
        public int length { get; set; }
        public List<Column> columns { get; set; }
        public Search search { get; set; }
        public List<Order> order { get; set; }
    }

    public class Column
    {
        public string data { get; set; }
        public string name { get; set; }
        public bool searchable { get; set; }
        public bool orderable { get; set; }
        public Search search { get; set; }
    }

    public class Search
    {
        public string value { get; set; }
        public string regex { get; set; }
    }

    public class Order
    {
        public int column { get; set; }
        public string dir { get; set; }
    }
    /// End- JSon class sent from Datatables
}