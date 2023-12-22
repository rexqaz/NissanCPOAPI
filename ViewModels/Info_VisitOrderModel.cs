using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication.ViewModels
{
    public class Info_VisitOrderModel
    {
        /// <summary>
        /// token
        /// </summary>
        public string token { get; set; }

        /// <summary>
        /// 預約賞車款
        /// </summary>
        public string visitCarType { get; set; }


        /// <summary>
        /// 預約賞車年分
        /// </summary>
        public string visitCarYear { get; set; }

        /// <summary>
        /// 聯絡經銷商
        /// </summary>
        public string dealerName { get; set; }

        /// <summary>
        /// 經銷商地址
        /// </summary>
        public string dealerAddress { get; set; }

        /// <summary>
        /// 經銷商電話
        /// </summary>
        public string dealerTel { get; set; }

        /// <summary>
        /// 手機號碼
        /// </summary>
        public string mobile { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// Email
        /// </summary>
        public string email { get; set; }

        /// <summary>
        /// 稱謂
        /// </summary>
        public string title { get; set; }

        /// <summary>
        /// 身分證
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// 出生年月日
        /// </summary>
        public string birthday { get; set; }

        /// <summary>
        /// 現居地址
        /// </summary>
        public string address { get; set; }

        /// <summary>
        /// 指定銷售顧問
        /// </summary>
        public string salesRep { get; set; }

        /// <summary>
        /// 其他需求
        /// </summary>
        public string others { get; set; }

        /// <summary>
        /// 方便連絡時段
        /// </summary>
        public string period { get; set; }

        /// <summary>
        /// 產品編號
        /// </summary>
        public string shopNo { get; set; }

        /// <summary>
        /// 預約時間
        /// </summary>
        public DateTime visitTime { get; set; }

        /// <summary>
        /// 品牌
        /// </summary>
        public string brand { get; set; }
    }
}