using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication.ViewModels
{
    public class Info_GetPrepaidDataModel
    {
        /// <summary>
        /// 會員id
        /// </summary>
        public string user_id { get; set; }
    }

    public class Info_GetPrepaidDataModelByBrand: Info_GetPrepaidDataModel
    {
        /// <summary>
        /// 品牌
        /// </summary>
        public string brand { get; set; }
        public int page { get; set; } = 0;
        public int limit { get; set; } = 5;
    }
}