using System;
using System.Collections.Generic;

namespace Loan_CRM.Areas.Collector.Models
{
    public class DefaulterReportModel
    {

        public int id { get; set; }
        public List<string> date { get; set; }
        public List<string> DateRange { get; set; }
        public double Sum_Number_Defaulter { get; set; }
        public double Sum_Outstanding_Loan_Principle { get; set; }
        public List<int> Number_Defaulter { get; set; }
        public List<double> Outstanding_Loan_Principle { get; set; }
        public List<double> Outstanding_amount { get; set; }
        public List<int> Non_Starter_Number { get; set; }
        public List<int> Partial_Payment_Number { get; set; }
        public List<int> Defaulter_Collection { get; set; }
        public List<int> Total_No_of_Defaulter { get; set; }
        public List<int> DefaulterIdList { get; set; }
        public List<string> DefaulterNameList { get; set; }
        public List<DefaulterReportModel> DefaulterReportList { get; set; }
        public string Report_Name { get; set; } = "";
        public int NumberOfDefaulterOfficers { get; set; }
        public int userid { get; set; }
        public string username { get; set; }
        public List<DefaulterCount> DefaulterCountList { get; set; }
        public List<OutstandingLoanprincipal> OutstandingLoanprincipalList { get; set; }
        public List<OutstandingLoanAmount> OutstandingLoanAmountList { get; set; }
        public List<NonStarterNumber> NonStarterNumberList { get; set; }
        public List<PartialPaymentNumber> PartialPaymentNumberList { get; set; }
        public List<DefaulterCollection> DefaulterCollectionList { get; set; }
        public List<SumOfDefaulter> SumOfDefaulterList { get; set; }
        public List<SumOfAgency> SumOfAgencyList { get; set; }
        public List<SumOfOutstanding> SumOfOutstandingList { get; set; }
        public DefaulterReportModel()
        {
            date = new List<string>();
            Number_Defaulter = new List<int>();
            Outstanding_Loan_Principle = new List<double>();
            Outstanding_amount = new List<double>();
            Non_Starter_Number = new List<int>();
            Partial_Payment_Number = new List<int>();
            Defaulter_Collection = new List<int>();
            DefaulterReportList = new List<DefaulterReportModel>();
            DefaulterIdList = new List<int>();
            DefaulterCountList = new List<DefaulterCount>();
            OutstandingLoanprincipalList = new List<OutstandingLoanprincipal>();
            OutstandingLoanAmountList = new List<OutstandingLoanAmount>();
            NonStarterNumberList = new List<NonStarterNumber>();
            PartialPaymentNumberList = new List<PartialPaymentNumber>();
            DefaulterCollectionList = new List<DefaulterCollection>();
            DefaulterNameList = new List<string>();
            SumOfDefaulterList = new List<SumOfDefaulter>();
            SumOfAgencyList = new List<SumOfAgency>();
            SumOfOutstandingList = new List<SumOfOutstanding>();
            Total_No_of_Defaulter = new List<int>();
            DateRange = new List<string>();
        }

    }
    public class DefaulterCount
    {
        public string Date { get; set; }
        public int DefaulterId { get; set; }
        public string DefaulterName { get; set; }
        public int Number_Defaulter { get; set; }

    }
    public class OutstandingLoanprincipal
    {
        public string Date { get; set; }
        public int DefaulterId { get; set; }
        public string DefaulterName { get; set; }
        public double Outstanding_Loan_Principle { get; set; }
        public Double SumOutstanding_Loan_Principle { get; set; }
    }
    public class OutstandingLoanAmount
    {
        public string Date { get; set; }
        public int DefaulterId { get; set; }
        public string DefaulterName { get; set; }
        public double Outstanding_amount { get; set; }
    }
    public class NonStarterNumber
    {
        public string Date { get; set; }
        public int DefaulterId { get; set; }
        public string DefaulterName { get; set; }
        public int Non_Starter_Number { get; set; }
    }
    public class PartialPaymentNumber
    {
        public string Date { get; set; }
        public int DefaulterId { get; set; }
        public string DefaulterName { get; set; }
        public int Partial_Payment_Number { get; set; }
    }
    public class DefaulterCollection
    {
        public string Date { get; set; }
        public int DefaulterId { get; set; }
        public string DefaulterName { get; set; }
        public int Defaulter_Collection { get; set; }
    }
    public class SumOfDefaulter
    {
        public string Date { get; set; }
        public int DefaulterId { get; set; }
        public string DefaulterName { get; set; }
        public int Sum_Of_Defaulter { get; set; }
    }
    public class SumOfAgency
    {
        public string Date { get; set; }
        public int DefaulterId { get; set; }
        public string DefaulterName { get; set; }
        public double Sum_Of_Agency { get; set; }
    }
    public class SumOfOutstanding
    {
        public string Date { get; set; }
        public int DefaulterId { get; set; }
        public string DefaulterName { get; set; }
        public double Sum_Of_Outstanding { get; set; }
    }
}