using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApplication.Models
{
    public class NewsViewModel
    {
        public string bodyContent { get; set; }
        public string picture { get; set; }
        public string title { get; set; }
        public DateTime createTime { get; set; }
        public string id { get; set; }
        public string status { get; set; }
        public string publishRange { get; set; }
        public string action { get; set; }
        public string showBody { get; set; }
    }
}