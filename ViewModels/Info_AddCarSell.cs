using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication.ViewModels
{
    public class Info_AddCarSellModel
    {
        /// <summary>
        /// 品牌
        /// </summary>
        public string brand { get; set; }

        /// <summary>
        /// 車牌號碼
        /// </summary>
        public string licensePlate { get; set; }

        /// <summary>
        /// 行照(照片)
        /// </summary>
        //public HttpPostedFileBase licensePicture { get; set; }

        /// <summary>
        /// 里程數
        /// </summary>
        public string milage { get; set; }

        /// <summary>
        /// 出廠年份
        /// </summary>
        public string yearOfManufacture { get; set; }

        /// <summary>
        /// 出廠月份
        /// </summary>
        public string monthOfManufacture { get; set; }

        /// <summary>
        /// 車型
        /// </summary>
        public string carModel { get; set; }


        /// <summary>
        /// 地區
        /// </summary>
        public string area { get; set; }

        /// <summary>
        /// 中古車營業據點
        /// </summary>
        public string dealer { get; set; }

        /// <summary>
        /// 中古車銷售顧問
        /// </summary>
        public string salesRep { get; set; }


        /// <summary>
        /// 手機號碼
        /// </summary>
        public string mobile { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 稱謂
        /// </summary>
        public string title { get; set; }

        /// <summary>
        /// 出生年月日
        /// </summary>
        public string birthday { get; set; }

        /// <summary>
        /// Email
        /// </summary>
        public string email { get; set; }

        

        /// <summary>
        /// 車況照1
        /// </summary>
        //public HttpPostedFileBase carCondition1 { get; set; }

        ///// <summary>
        ///// 車況照2
        ///// </summary>
        //public HttpPostedFileBase carCondition2 { get; set; }

        ///// <summary>
        ///// 車況照3
        ///// </summary>
        //public HttpPostedFileBase carCondition3 { get; set; }

        ///// <summary>
        ///// 車況照4
        ///// </summary>
        //public HttpPostedFileBase carCondition4 { get; set; }

       
        /// <summary>
        /// 是否同意接受新車諮詢服務 (0/1)
        /// </summary>
        public string needConsult { get; set; }

       
    }
}