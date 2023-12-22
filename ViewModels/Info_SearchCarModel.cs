using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication.ViewModels
{
    public class Info_SearchCarModel
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
        /// 頁數( 第幾頁+每頁幾筆，以逗點區隔，例 " 3,20 " ) (空白代表全部)
        /// </summary>
        public string page { get; set; }

        /// <summary>
        /// 價格
        /// </summary>
        public List<int> price { get; set; }

        /// <summary>
        /// 里程數
        /// </summary>
        public List<int> milage { get; set; }

        /// <summary>
        /// 出廠年份
        /// </summary>
        public List<int> yearOfManufacture { get; set; }

        /// <summary>
        /// 車種 ex. ["掀背車", "休旅車"]
        /// </summary>
        public List<string> carType { get; set; }

        /// <summary>
        /// 車型 ex. ["Kicks", "Bluebird"]
        /// </summary>
        public List<string> carModel { get; set; }

        /// <summary>
        /// 車輛所在地 ex. ["台北市", "新北市"]
        /// </summary>
        public List<string> area { get; set; }

        /// <summary>
        /// 內外裝配備 ex. ["LED頭燈", "感應電動尾門"]
        /// </summary>
        public List<string> equipment { get; set; }

        /// <summary>
        /// 驅動方式 ex. ["2WD", "4WD"]
        /// </summary>
        public List<string> driveMode { get; set; }

        /// <summary>
        /// 顏色 ex. ["黑色", "紅色"]
        /// </summary>
        public List<string> color { get; set; }

        /// <summary>
        /// 安全重點功能 ex. ["安全氣囊", "ICC智慧型全速域定速控制系統"]
        /// </summary>
        public List<string> feature { get; set; }

        /// <summary>
        /// 中古車營業據點 ex. ["元隆-桃鶯展示中心", "誠隆-高雄展示中心"]
        /// </summary>
        public List<string> dealer { get; set; }

        /// <summary>
        /// 排序方式  ex: 價格由低到高
        /// </summary>
        public string orderBy { get; set; }
    }
}