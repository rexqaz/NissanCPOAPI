using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication.Models
{
    public class BannerViewModel
    {
        public long seq { get; set; }
        public string bodyContent { get; set; }
        public string picture { get; set; }
        public string mobilePicture { get; set; }
        public string title { get; set; }
        public string subTitle { get; set; }
        public string url { get; set; }
        public string status { get; set; }
        public string publishRange { get; set; }
        public DateTime manuscriptTime { get; set; }
        public DateTime publishTime { get; set; }
        public bool isTakeOffRealtime { get; set; }
        public string action { get; set; }
        public string banner_name { get; set; }
        public int banner_sort { get; set; }
    }
}