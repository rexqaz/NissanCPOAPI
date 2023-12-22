using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication.Models
{
    public class CarBuyViewModel
    {
        public string category { get; set; }
        public DateTime createTime { get; set; }
        public string shopNo { get; set; }
        public string dealer { get; set; }
        public string stronghold { get; set; }
        public string member { get; set; }
        public string mobile { get; set; }
        public string assignedConsulant { get; set; }
        public string serveConsulant { get; set; }
        public string status { get; set; }
        public string cashFlow { get; set; }
        public DateTime updateTime { get; set; }
        public DateTime? visitTime { get; set; }
        public string period { get; set; }
        public string seq { get; set; }
        public string id { get; set; }
        public string email { get; set; }
        public string address { get; set; }
        public string area { get; set; }
        public string note { get; set; }
        public string returnReason { get; set; }
        public string loseReason { get; set; }
        public string action { get; set; }
        public string formalNo { get; set; }
        public string prepaidNo { get; set; }
    }
}