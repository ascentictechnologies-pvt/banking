namespace Loan_CRM.Models
{
    public class UserModel
    {
        public int userid { get; set; }
        public string username { get; set; }
        public string userpass { get; set; }
        public string userfullname { get; set; }
        public string usertype { get; set; }
        public string userextension { get; set; }
        public string secondaryextension { get; set; }
        public int webadminrole { get; set; }
        public int usergroupid { get; set; }
        public string userstatus { get; set; }
        public int parentuserid { get; set; }
    }
}