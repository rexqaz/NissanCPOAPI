using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication.ViewModels
{
    public class Info_DealerModel
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

        /// <summary>
        /// 地區
        /// </summary>
        public List<string> areas { get; set; }

        /// <summary>
        /// 中古或新車
        /// </summary>
        public string type { get; set; }

        /// <summary>
        /// 經銷商id (若空白則回傳全部)
        /// </summary>
        public string dealer_id { get; set; }

        /// <summary>
        /// 經銷商名稱 (非必填，搭配type=new查詢)
        /// </summary>
        public string dealer_name { get; set; }
    }
}