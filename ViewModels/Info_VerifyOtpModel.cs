using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication.ViewModels
{
    public class Info_VerifyOtpModel
    {
        /// <summary>
        ///     token
        /// </summary>
        public string token { get; set; }

        /// <summary>
        /// OTP
        /// </summary>
        public string otp { get; set; }

        /// <summary>
        /// 手機
        /// </summary>
        public string mobile { get; set; }

        /// <summary>
        /// 品牌
        /// </summary>
        public string brand { get; set; }
    }
}