using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication.Models
{
    public class NoticeRecordViewModel
    {
        public string period { get; set; }
        public string noticeTimes { get; set; }
        public string accumulativeNoticeTimes { get; set; }
        public string price { get; set; }
        public string yearOfManufacture { get; set; }
        public string dealer { get; set; }
        public string carModal { get; set; }
        public DateTime NoticeTime { get; set; }
    }
}