using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication.Models
{
    public class MenuViewModel
    {
        public List<menuItem> Menu { get; set; }
    }

    public class menuItem
    {
        public string id { get; set; }
        public string menuName { get; set; }
        public string isRoot { get; set; }
        public string controller { get; set; }
        public string action { get; set; }
        public string parent { get; set; }
        public string isLink { get; set; }
        public string icon { get; set; }
        public int orderSeq { get; set; }
        public List<menuItem> Menu { get; set; }
    }
}