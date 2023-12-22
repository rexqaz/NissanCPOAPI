using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication.ViewModels
{
    public class Info_GetLoginStatusModel
    {
        /// <summary>
        ///     token
        /// </summary>
        public string token { get; set; }

        /// <summary>
        ///     手機號碼
        /// </summary>
        [Required]
        public string mobile { get; set; }
    }
}