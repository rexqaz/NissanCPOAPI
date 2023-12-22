using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication.ViewModels
{
    public class Info_AddNoticeRecordModel
    {
        /// <summary>
        ///     token
        /// </summary>
        public string token { get; set; }

        /// <summary>
        /// 產品編號
        /// </summary>
        public string shopNo { get; set; }

        /// <summary>
        /// 會員id
        /// </summary>
        public string user_id { get; set; }

        /// <summary>
        /// 品牌
        /// </summary>
        public string brand { get; set; }
    }
}