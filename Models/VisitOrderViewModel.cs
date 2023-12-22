using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication.Models
{
    public class VisitOrderViewModel
    {
        public DateTime visitTime { get; set; }
        public string name { get; set; }
        public string title { get; set; }
        public string mobile { get; set; }
        public string salesRep { get; set; }
        public string status { get; set; }
        public long seq { get; set; }
        public string consultant { get; set; }
        public string consultantName { get; set; }
    }
}