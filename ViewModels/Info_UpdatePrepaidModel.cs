using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication.ViewModels
{
    public class Info_UpdatePrepaidModel
    {
        /// <summary>
        /// 會員id
        /// </summary>
        public string user_id { get; set; }

        /// <summary>
        /// 退訂原因
        /// </summary>
        public string reason { get; set; }

    }
}