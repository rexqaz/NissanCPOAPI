using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication.ViewModels
{
    public class Info_AddBannerHitModel
    {

        /// <summary>
        /// Banner編號
        /// </summary>
        public string banner_id { get; set; }

        /// <summary>
        /// 類別(hit: 點擊數 / view: 曝光數)
        /// </summary>
        public string type { get; set; }
    }
}