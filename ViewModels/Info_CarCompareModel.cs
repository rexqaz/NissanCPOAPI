﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication.ViewModels
{
    public class Info_CarCompareModel
    {
        /// <summary>
        ///     token
        /// </summary>
        public string token { get; set; }

        /// <summary>
        ///     品牌
        /// </summary>
        public string brand { get; set; }

        /// <summary>
        ///     系統售車編號
        /// </summary>
        public List<string> seq { get; set; }

        /// <summary>
        ///     會員系統編號
        /// </summary>
        public string user_id { get; set; }
    }
}