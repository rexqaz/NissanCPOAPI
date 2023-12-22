using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Cors;
using System.Web.Mvc;
using WebApplication.Models;
using WebApplication.ViewModels;

namespace WebApplication.Controllers.api
{
    public class ShopController : BaseAPIController
    {
        CarShopEntities carShopEntities = new CarShopEntities();
        /// <summary>
        /// 檢查車輛是否上架中
        /// </summary>
        /// <param name="Data"></param>
        /// <returns></returns>
        [HttpPost]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public object CheckOnline(Info_GetShopModel Data)
        {
            if (Data.ShopSeqList != null)
            {
                if (Data.ShopSeqList.Count > 0)
                {
                    var Shops = carShopEntities.Shops.Where(x => Data.ShopSeqList.Contains(x.seq) && x.status == "上架中").ToList();
                    JArray ja = new JArray();
                    foreach (var shop in Shops)
                    {
                        ja.Add(shop.seq);
                    }
                    var data = new
                    {
                        Count = ja.Count,
                        Data = ja
                    };
                    return ReturnOK(data);
                }
                return ReturnOK();
            }
            else
            {
                return ReturnError("缺少參數");
            }
        }
    }
}