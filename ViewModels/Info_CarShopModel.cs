using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication.ViewModels
{
    public class Info_CarShopModel
    {
        /// <summary>
        /// 售價
        /// </summary>
        public string price { get; set; }

        /// <summary>
        /// 車牌號碼
        /// </summary>
        public string licensePlate { get; set; }

        /// <summary>
        /// 行照(照片)
        /// </summary>
        public HttpPostedFileBase licensePicture { get; set; }

        /// <summary>
        /// 里程數
        /// </summary>
        public string milage { get; set; }

        /// <summary>
        /// 出廠年份
        /// </summary>
        public string yearOfManufacture { get; set; }

        /// <summary>
        /// 車種
        /// </summary>
        public string carModel { get; set; }

        /// <summary>
        /// 車款
        /// </summary>
        public string carType { get; set; }

        /// <summary>
        /// 驅動方式
        /// </summary>
        public string driveMode { get; set; }

        /// <summary>
        /// 變速箱形式
        /// </summary>
        public string transmissionType { get; set; }

        /// <summary>
        /// 燃料型態
        /// </summary>
        public string fuelType { get; set; }


        /// <summary>
        /// 排氣量
        /// </summary>
        public string displacement { get; set; }

        /// <summary>
        /// 馬力
        /// </summary>
        public string horsepower { get; set; }

        /// <summary>
        /// 外觀顏色
        /// </summary>
        public string outerColor { get; set; }

        /// <summary>
        /// 內裝顏色
        /// </summary>
        public string innerColor { get; set; }

        /// <summary>
        /// 重點特色
        /// </summary>
        public string feature { get; set; }

        /// <summary>
        /// 經銷商
        /// </summary>
        public string dealer { get; set; }

        /// <summary>
        /// 據點
        /// </summary>
        public string stronghold { get; set; }

        /// <summary>
        /// 業代
        /// </summary>
        public string salesRep { get; set; }

        /// <summary>
        /// 車況照1
        /// </summary>
        public HttpPostedFileBase carCondition1 { get; set; }

        /// <summary>
        /// 車況照2
        /// </summary>
        public HttpPostedFileBase carCondition2 { get; set; }

        /// <summary>
        /// 車況照3
        /// </summary>
        public HttpPostedFileBase carCondition3 { get; set; }

        /// <summary>
        /// 車況照4
        /// </summary>
        public HttpPostedFileBase carCondition4 { get; set; }

        /// <summary>
        /// 其他角度照1
        /// </summary>
        public HttpPostedFileBase otherCondition1 { get; set; }

        /// <summary>
        /// 其他角度照2
        /// </summary>
        public HttpPostedFileBase otherCondition2 { get; set; }

        /// <summary>
        /// 聯絡方式
        /// </summary>
        public string contact { get; set; }

        /// <summary>
        /// 建立者
        /// </summary>
        public string creator { get; set; }
    }
}