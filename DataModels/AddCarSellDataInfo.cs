using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication.DataModels
{
    public class AddCarSellDataInfo
    {
        public AddCarSellDataInfo()
        {

        }

        /// <summary>
        /// 西元年
        /// </summary>
        public int yearOfManufacture { get; set; }
        /// <summary>
        /// 月份
        /// </summary>
        public int monthOfManufacture { get; set; }
        /// <summary>
        /// 使用里程
        /// </summary>
        public int milage { get; set; }
        /// <summary>
        /// 品牌
        /// </summary>
        public string carBrand { get; set; } = "";
        /// <summary>
        /// 品牌-選擇其他時手動填入的品牌
        /// </summary>
        public string otherBrand { get; set; } = "";
        /// <summary>
        /// 車型
        /// </summary>
        public string carModel { get; set; } = "";
        /// <summary>
        /// 車牌號碼
        /// </summary>
        public string licensePlate { get; set; } = "";
        /// <summary>
        /// 中古車營業據點-地區
        /// </summary>
        public string area { get; set; } = "";
        /// <summary>
        /// 中古車營業據點
        /// </summary>
        public string dealer { get; set; } = "";
        /// <summary>
        /// 新車營業據點(經銷商名稱)
        /// </summary>
        public string dealerName { get; set; } = "";
        /// <summary>
        /// 新車營業據點(seq)
        /// </summary>
        public int newDealerSeq { get; set; }
        public string salesRep { get; set; } = "";
        /// <summary>
        /// 品牌
        /// </summary>
        public string brand { get; set; } = "";
        /// <summary>
        /// 客戶-手機
        /// </summary>
        public string mobile { get; set; } = "";
        /// <summary>
        /// 客戶-名稱
        /// </summary>
        public string name { get; set; } = "";
        /// <summary>
        /// 客戶-尊稱
        /// </summary>
        public string title { get; set; } = "";
        /// <summary>
        /// 客戶-生日
        /// </summary>
        public DateTime? birthday { get; set; }
        /// <summary>
        /// 客戶-電子信箱
        /// </summary>
        public string email { get; set; } = "";
        /// <summary>
        /// 我同意接受新車諮詢服務
        /// </summary>
        public bool needConsult { get; set; }
        /// <summary>
        /// 新車銷售顧問
        /// </summary>
        public string newCarConsultant { get; set; } = "";
        /// <summary>
        /// 新車營業據點
        /// </summary>
        public string newCarDealer { get; set; } = "";


        /// <summary>
        /// 行照
        /// </summary>
        public string licensePicture { get; set; } = "";
        public string carCondition1 { get; set; } = "";
        public string carCondition2 { get; set; } = "";
        public string carCondition3 { get; set; } = "";
        public string carCondition4 { get; set; } = "";
        public string carCondition5 { get; set; } = "";
        public int fileCount { get; set; } = 0;

        /// <summary>
        /// 紀錄檢查資料時的錯誤訊息
        /// </summary>
        public string ReturnErr { get; set; } = "";
        public string InputIsok { get; set; } = "";


        /// <summary>
        /// 寫入資料庫後回傳的流水號
        /// </summary>
        public string SellNo { get; set; } = "";
        /// <summary>
        /// 寫入資料庫後回傳的seq
        /// </summary>
        public int Sells_seq { get; set; }
    }
}