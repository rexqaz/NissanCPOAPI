using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication.ViewModels
{
    public class Info_GetOtpAndSendSmsModel
{
        /// <summary>
        ///     token
        /// </summary>
        public string token { get; set; }

        /// <summary>
        ///     手機號碼
        /// </summary>
        public string mobile { get; set; }

        /// <summary>
        ///     品牌
        /// </summary>
        public string brand { get; set; }

        /// <summary>
        ///     種類 (register: 註冊，sell: 我要賣車)
        /// </summary>
        public string type { get; set; }

    }
}