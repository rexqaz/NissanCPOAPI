//------------------------------------------------------------------------------
// <auto-generated>
//     這個程式碼是由範本產生。
//
//     對這個檔案進行手動變更可能導致您的應用程式產生未預期的行為。
//     如果重新產生程式碼，將會覆寫對這個檔案的手動變更。
// </auto-generated>
//------------------------------------------------------------------------------

namespace WebApplication.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Subscriptions
    {
        public long seq { get; set; }
        public Nullable<int> carShop_id { get; set; }
        public Nullable<int> user_id { get; set; }
        public Nullable<System.DateTime> createTime { get; set; }
        public string driveMode { get; set; }
        public string carType { get; set; }
        public string carModel { get; set; }
        public string outerColor { get; set; }
        public string area { get; set; }
        public string dealer { get; set; }
        public int price { get; set; }
        public int milage { get; set; }
        public int yearOfManufacture { get; set; }
        public string brand { get; set; }
        public string priceStr { get; set; }
        public string milageStr { get; set; }
        public string yearOfManufactureStr { get; set; }
        public string email { get; set; }
    }
}
