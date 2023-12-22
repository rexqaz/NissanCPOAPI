using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication.ViewModels
{
    public class Info_MyFavouriteAddModel
    {
        /// <summary>
        ///     token
        /// </summary>
        public string token { get; set; }
        /// <summary>
        /// 會員id
        /// </summary>
        public string user_id { get; set; }

        /// <summary>
        /// 車輛銷售id
        /// </summary>
        public string carShop_id { get; set; }
    }
}