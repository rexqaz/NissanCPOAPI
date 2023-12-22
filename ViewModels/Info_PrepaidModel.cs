using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication.ViewModels
{
    public class Info_PrepaidModel
    {
        /// <summary>
        /// 賣車編號
        /// </summary>
        public string shopNo { get; set; }

        /// <summary>
        /// 會員編號
        /// </summary>
        public string user_id { get; set; }

        /// <summary>
        /// 方便連絡時段
        /// </summary>
        public string contactPeriod { get; set; }

        /// <summary>
        /// 其他需求
        /// </summary>
        public string others { get; set; }

        /// <summary>
        /// 服務專員
        /// </summary>
        public string salesRep { get; set; }
    }
}