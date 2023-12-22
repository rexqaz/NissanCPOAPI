using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication.ViewModels
{
    public class Info_ForgetPasswordModel
    {
        /// <summary>
        ///     token
        /// </summary>
        public string token { get; set; }

        /// <summary>
        ///     透過手機簡訊或Email 收到臨時密碼 (S:簡訊，M:Email)
        /// </summary>
        [Required]
        public string method { get; set; }

        /// <summary>
        ///     Email
        /// </summary>
        [Required]
        public string email { get; set; }

        /// <summary>
        ///     品牌
        /// </summary>
        [Required]
        public string brand { get; set; }
    }
}