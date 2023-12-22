using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using WebApplication.Extensions;
using WebApplication.Models;
using WebApplication.Services;
using WebApplication.ViewModels;
namespace WebApplication.Controllers.api
{
    [JwtAuthActionFilter]
    public class PrepaidsController : BaseAPIController
    {
        /// <summary>
        /// 預付保留列表
        /// </summary>
        /// <param name="Data"></param>
        /// <returns></returns>
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public async Task<object> GetAsync([FromUri] Info_GetPrepaidDataModelByBrand Data)
        {
            try
            {
                CarShopEntities _db = new CarShopEntities();
                string InputIsok = "Y";
                string ReturnErr = "";

               
                string brand = string.Empty;

                if (Data.brand != null)
                {
                    brand = APCommonFun.CDBNulltrim(Data.brand);
                }

                //第一步: 先判斷有沒有必填未填寫，
                if (brand == "") //必填                       
                {
                    InputIsok = "N";
                    ReturnErr = "執行動作錯誤-brand 為必填欄位";
                    APCommonFun.Error("[GetPrepaidDataController]90-" + ReturnErr);
                    return ReturnError(ReturnErr);
                }

                Request.Headers.TryGetValues("Authorization", out var headerToken);
                string token = headerToken != null ? headerToken.FirstOrDefault() : "";
                var member = new OneidService().GetMember(token.Replace("Bearer ", ""), Data.user_id);
                string user_id = member.Data != null ? member.Data.seq.ToString() : string.Empty;

                if (user_id == "") //必填                       
                {
                    InputIsok = "N";
                    ReturnErr = "執行動作錯誤-user_id 為必填欄位";
                    APCommonFun.Error("[GetPrepaidDataController]90-" + ReturnErr);
                    return ReturnError(ReturnErr);
                }


                var requset = HttpContext.Current.Request;
                var prepaids = _db.Prepaid.Where(x => x.user_id == Data.user_id).ToList()
                    .Join(_db.Shops.Where(x => x.brand == brand), p => p.shopNo, s => s.ShopNo, (p, s) => new { p, s })
                    .Join(_db.Dealers, p => new { p.s.dealer, p.s.stronghold }, d => new { dealer = d.dealerName, stronghold = d.businessOffice }, (p, d) => new
                    {
                        prepaidNo = p.p.prepaidNo,
                        money = "3000",
                        createTime = p.p.createTime,
                        dueTime = p.p.createTime.Value.AddDays(3),
                        paidStatus = p.p.paidStatus,
                        carModel = p.s.carModel,
                        yearOfManufacture = p.s.yearOfManufacture,
                        monthOfManufacture = p.s.monthOfManufacture,
                        sellingPrice = p.s.price,
                        milage = p.s.milage,
                        dealer = p.s.dealer,
                        tel = string.IsNullOrEmpty(d.telAreaCode) ? d.tel : $"{d.telAreaCode}-{d.tel}",
                        address = string.IsNullOrEmpty(d.area) ? d.address : $"{d.area} {d.address}",
                        stronghold = p.s.stronghold,
                        businessDay = $"{d.busDay}",
                        businessTime = $"{d.businessStartHourDay} ~ {d.businessEndHourNight}",
                        imageUrl = new FileService().GetRealUrl(p.s.carCondition1)
                    }).ToList();

                if (prepaids != null)
                {
                    int count = prepaids.Count();
                    int page = Data.page > 0 ? Data.page - 1 : 0;
                    var qList = prepaids.Skip(page * Data.limit).Take(Data.limit);
                    var data = new
                    {
                        Count = count,
                        Data = qList
                    };
                    return ReturnOK(data);
                }


                return ReturnError("找不到此筆訂單");
            }
            catch (Exception ex)
            {
                APCommonFun.Error("[GetPrepaidDataController]99：" + ex.ToString());
                return ReturnException();
            }
        }

        /// <summary>
        /// 預付保留查詢
        /// </summary>
        /// <param name="prepaidNo"></param>
        /// <param name="Data"></param>
        /// <returns></returns>
        [Route("api/Prepaids/{prepaidNo}")]
        public async Task<object> GetByIdAsync(string prepaidNo, [FromUri] Info_GetPrepaidDataModel Data)
        {
            try
            {
                CarShopEntities _db = new CarShopEntities();

                Request.Headers.TryGetValues("Authorization", out var headerToken);
                string token = headerToken != null ? headerToken.FirstOrDefault() : "";
                var member = new OneidService().GetMember(token.Replace("Bearer ", ""), Data.user_id);
                string user_id = member.Data != null ? member.Data.seq.ToString() : string.Empty;

                if (user_id == "") //必填                       
                {
                    var ReturnErr = "執行動作錯誤-user_id 為必填欄位";
                    APCommonFun.Error("[GetPrepaidDataController]90-" + ReturnErr);
                    return ReturnError(ReturnErr);
                }

                var prepaid = _db.Prepaid.Where(x => x.user_id == Data.user_id && x.prepaidNo == prepaidNo).ToList()
                    .Join(_db.Shops, p => p.shopNo, s => s.ShopNo, (p, s) => new { p, s })
                    .Join(_db.Dealers, p => new { p.s.dealer, p.s.stronghold }, d => new { dealer = d.dealerName, stronghold = d.businessOffice }, (p, d) => new
                    {
                        prepaidNo = p.p.prepaidNo,
                        money = "3000",
                        createTime = p.p.createTime,
                        dueTime = p.p.createTime.Value.AddDays(3),
                        paidStatus = p.p.paidStatus,
                        carModel = p.s.carModel,
                        yearOfManufacture = p.s.yearOfManufacture,
                        monthOfManufacture = p.s.monthOfManufacture,
                        sellingPrice = p.s.price,
                        milage = p.s.milage,
                        dealer = p.s.dealer,
                        tel = string.IsNullOrEmpty(d.telAreaCode) ? d.tel : $"{d.telAreaCode}-{d.tel}",
                        address = string.IsNullOrEmpty(d.area) ? d.address : $"{d.area} {d.address}",
                        stronghold = p.s.stronghold,
                        businessDay = $"{d.busDay}",
                        businessTime = $"{d.businessStartHourDay} ~ {d.businessEndHourNight}"
                    }).FirstOrDefault();

                if (prepaid != null)
                {
                    return ReturnOK(prepaid);
                }
                
                return ReturnError("找不到此筆訂單");
            }
            catch (Exception ex)
            {
                APCommonFun.Error("[GetPrepaidDataController]99：" + ex.ToString());
                return ReturnException();
            }
        }



    }
}
