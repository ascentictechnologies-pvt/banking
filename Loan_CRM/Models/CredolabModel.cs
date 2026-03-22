using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Loan_CRM.Models
{
    public class CredolabErrorModel
    {
        public string traceId { get; set; }
        public string type { get; set; }
        public string title { get; set; }
        public int status { get; set; }
        public string detail { get; set; }
        public object instance { get; set; }
        //public Extensions extensions { get; set; }
    }
}