using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication.ViewModels
{
    public class Info_NewsModel
    {
        /// <summary>
        ///     token
        /// </summary>
        public string token { get; set; }

        /// <summary>
        /// 頁數( 第幾頁+每頁幾筆，以逗點區隔，例 " 3,20 " ) (空白代表全部)
        /// </summary>
        public string page { get; set; }

        /// <summary>
        /// 品牌
        /// </summary>
        public string brand { get; set; }
    }
}