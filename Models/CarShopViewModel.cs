using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication.Models
{    
    public  class CarShopViewModel
    {
        public long seq { get; set; }
        public int price { get; set; }
        public string licensePlate { get; set; }
        public string licensePicture { get; set; }
        public int milage { get; set; }
        public int yearOfManufacture { get; set; }
        public int monthOfManufacture { get; set; }
        public string carModel { get; set; }
        public string carType { get; set; }
        public string driveMode { get; set; }
        public string transmissionType { get; set; }
        public string fuelType { get; set; }
        public string displacement { get; set; }
        public string horsepower { get; set; }
        public string outerColor { get; set; }
        public string innerColor { get; set; }
        public List<string> feature { get; set; }
        public string dealer { get; set; }
        public string stronghold { get; set; }
        public string salesRep { get; set; }
        public string carCondition1 { get; set; }
        public string carCondition2 { get; set; }
        public string carCondition3 { get; set; }
        public string carCondition4 { get; set; }
        public string otherCondition1 { get; set; }
        public string otherCondition2 { get; set; }
        public string contact { get; set; }
        public string createDate { get; set; }
        public string creator { get; set; }
        public string status { get; set; }
        public Nullable<System.DateTime> ListingDate { get; set; }
        public string description { get; set; }
        public string inspectionTable { get; set; }
        public string brand { get; set; }
        public Nullable<System.DateTime> createTime { get; set; }
        public Nullable<System.DateTime> updateTime { get; set; }
        public string ShopNo { get; set; }
        public string area { get; set; }
        public List<string> outEquip { get; set; }
        public string innerEquip { get; set; }
        public string member { get; set; }
        public string consultant { get; set; }
        public string process { get; set; }
        public string cashStatus { get; set; }
        public int finalPrice { get; set; }
        public string contract { get; set; }
        public string action { get; set; }
        public int sellingPrice { get; set; }
        public Nullable<System.DateTime> sellTime { get; set; }
        public int NoticeCount { get; set; }
        public string carCondition5 { get; set; }
        public string carCondition6 { get; set; }
        public string carCondition7 { get; set; }
        public string carCondition8 { get; set; }
        public string carCondition9 { get; set; }
        public string carCondition10 { get; set; }
        public string formalShopNo { get; set; }
        public string isHit { get; set; }
        public string isFrame { get; set; }
        public Nullable<int> hitOrder { get; set; }
    }
}