using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication.Models
{
    public class PrepaidDataViewModel
    {
        public DateTime createTime { get; set; }
        public string prepaidNo { get; set; }
        public string prepaidMoney { get; set; }
        public string price { get; set; }
        public string yearOfManufacture { get; set; }
        public string dealer { get; set; }
        public string paidStatus { get; set; }
        public string name { get; set; }
        public string mobile { get; set; }
        public string carModel { get; set; }
        public int milage { get; set; }
        public string contact { get; set; }
        public string title { get; set; }
        public string email { get; set; }
        public string id { get; set; }
        public string birthday { get; set; }
        public string address { get; set; }
        public string sales { get; set; }
        public string serveSales { get; set; }
        public string displacement { get; set; }
        public string note { get; set; }
        public DateTime contactTime { get; set; }
        public string action { get; set; }
    }
}