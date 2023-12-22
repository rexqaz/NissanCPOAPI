using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication.ViewModels
{
    public class Info_CarSubscriptionModel
    {
        /// <summary>
        /// token
        /// </summary>
        public string token { get; set; }

        /// <summary>
        /// 品牌
        /// </summary>
        public string brand { get; set; }

        /// <summary>
        /// 驅動方式
        /// </summary>
        public List<string> driveMode { get; set; }
        /// <summary>
        /// 車種
        /// </summary>
        public List<string> carType { get; set; }

        /// <summary>
        /// 車型
        /// </summary>
        public List<string> carModel { get; set; }

        /// <summary>
        /// 顏色
        /// </summary>
        public List<string> outerColor { get; set; }

        /// <summary>
        /// 所在區域
        /// </summary>
        public List<string> area { get; set; }

        /// <summary>
        /// 中古車營業據點
        /// </summary>
        public List<string> dealer { get; set; }

        /// <summary>
        /// 價格
        /// </summary>
        public List<int> price { get; set; }

        /// <summary>
        /// 出廠年份
        /// </summary>
        public List<int> yearOfManufacture { get; set; }

        /// <summary>
        /// 里程數
        /// </summary>
        public List<int> milage { get; set; }

        /// <summary>
        /// 會員id
        /// </summary>
        public string user_id { get; set; }

        /// <summary>
        /// 會員email
        /// </summary>
        public string email { get; set; }
    }
}