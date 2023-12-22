using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication.ViewModels
{
    public class Info_ChangePasswordModel
    {
        /// <summary>
        ///     token
        /// </summary>
        public string token { get; set; }

        /// <summary>
        ///     舊密碼
        /// </summary>
        public string old_password { get; set; }

        /// <summary>
        ///     新密碼
        /// </summary>
        public string password { get; set; }

        /// <summary>
        ///     新密碼確認
        /// </summary>
        [Required]
        public string confirm_password { get; set; }

        /// <summary>
        ///     系統使用者代碼
        /// </summary>
        [Required]
        public string user_id { get; set; }
    }
}