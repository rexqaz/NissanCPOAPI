using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication.ViewModels
{
    public class Info_reCAPTCHAPostModel
    {
        /// <summary>
        /// 從前端傳來的 token
        /// </summary>
        public string token { get; set; }
        /// <summary>
        /// 從前端傳來的 IP
        /// </summary>
        public string ip { get; set; }
    }
}