using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication.ViewModels
{
    public class Info_DownloadFileModel
    {
        /// <summary>
        ///     token
        /// </summary>
        public string token { get; set; }

        /// <summary>
        /// 檔名
        /// </summary>
        public string filePath { get; set; }

        /// <summary>
        /// 檔案類型
        /// </summary>
        public string contentType { get; set; }
    }
}