using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication.ViewModels
{
    public class Info_VisitOrderVerifyMobileModel
    {
        /// <summary>
        /// token
        /// </summary>
        public string token { get; set; }

        /// <summary>
        /// 預約賞車代碼
        /// </summary>
        public string order_id { get; set; }

        /// <summary>
        /// 驗證碼
        /// </summary>
        public string code { get; set; }
    }
}