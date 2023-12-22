using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication.Models
{
    public class DealersViewModel2
    {
        public long seq { get; set; }
        public string area { get; set; }
        public string dealerName { get; set; }
        public string businessOffice { get; set; }
        public string telAreaCode { get; set; }
        public string tel { get; set; }
        public string address { get; set; }
        public string gmap { get; set; }
        public string businessStartHourDay { get; set; }
        public string businessEndHourDay { get; set; }
        public string businessStartHourNight { get; set; }
        public string businessEndHourNight { get; set; }
        public Nullable<System.DateTime> createTime { get; set; }
        public string type { get; set; }
        public string email { get; set; }
        public string brand { get; set; }
        public string businessOffice2 { get; set; }
        public string picture { get; set; }
        public string busDay { get; set; }
        public List<string> busDays { get; set; }
        public string dealerCode { get; set; }
    }
}