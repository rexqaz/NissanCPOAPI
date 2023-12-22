using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication.ViewModels
{
    public class Info_EditUserInfoModel
    {
        /// <summary>
        ///     token
        /// </summary>
        public string token { get; set; }

        /// <summary>
        ///     姓名
        /// </summary>
        [Required]
        public string name { get; set; }

        /// <summary>
        ///     身分證
        /// </summary>
        [Required]
        public string id { get; set; }

        /// <summary>
        ///     手機
        /// </summary>
        [Required]
        public string mobile { get; set; }

        /// <summary>
        ///     email
        /// </summary>
        [Required]
        [EmailAddress]
        public string email { get; set; }

        /// <summary>
        ///     職稱
        /// </summary>
        [Required]
        public string title { get; set; }

        /// <summary>
        ///     出生年月日
        /// </summary>
        [Required]
        public string birthday { get; set; }

        /// <summary>
        ///     居住地區
        /// </summary>
        [Required]
        public string area { get; set; }

        /// <summary>
        ///     居住地址
        /// </summary>
        [Required]
        public string address { get; set; }

        /// <summary>
        ///     興趣車款
        /// </summary>
        [Required]
        public string interestedCar { get; set; }

        /// <summary>
        ///     系統使用者代碼
        /// </summary>
        [Required]
        public string user_id { get; set; }
    }
}