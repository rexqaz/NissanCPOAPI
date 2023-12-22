using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using WebApplication.Extensions;
using WebApplication.Models;
using WebApplication.ViewModels;

namespace WebApplication.Controllers.api
{
    [JwtAuthActionFilter]
    public class GetCarCompareController : BaseAPIController
    {
        CarShopEntities carShopEntities = new CarShopEntities();
        /// <summary>
        /// 取得比較車輛數量
        /// </summary>
        /// <param name="Data"></param>
        /// <returns></returns>
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public object Post(Info_GetCarCompareModel Data)
        {
            string InputIsok = "Y";
            string ReturnErr = "";
            JArray newJa = new JArray();
            JArray ja = new JArray();

            try
            {
                string user_id = string.Empty; //若無則全部回傳
                string brand = string.Empty; //若無則全部回傳
                if (Data.brand != null)
                {
                    brand = APCommonFun.CDBNulltrim(Data.brand);
                }
                if (Data.user_id != null)
                {
                    user_id = APCommonFun.CDBNulltrim(Data.user_id);
                }

                //第一步 : 先判斷有沒有必填未填寫，
                if (user_id == "" || brand == "") //必填                       
                {
                    if (user_id == "")
                    {
                        InputIsok = "N";
                        ReturnErr = "執行動作錯誤-user_id 為必填欄位";
                    }
                    else
                    {
                        InputIsok = "N";
                        ReturnErr = "執行動作錯誤-brand 為必填欄位";
                    }
                }

                //第二步 : 如果有必填未給值，回傳告知必填欄位要有資料
                if (InputIsok == "N")
                {
                    APCommonFun.Error("[GetCarCompareController]90-" + ReturnErr);
                    return ReturnError(ReturnErr);
                }

                string sql = string.Empty;

                sql = "select top 1  * from CarCompareHistory where user_id=@user_id and brand=@brand   order by createTime desc ";

                DataTable dt = APCommonFun.GetSafeDataTable_MSSQL(
                    sql,
                    new List<SqlParameter>
                    {
                        new SqlParameter("@user_id", user_id),
                        new SqlParameter("@brand", brand)
                    }
                );
                //DataTable dt = APCommonFun.GetDataTable_MSSQL(sql);
                string carCompare = string.Empty;
                List<string> result = new List<string>();
                List<long> existShops = new List<long>();
                if (dt.Rows.Count > 0)
                {
                    carCompare = APCommonFun.CDBNulltrim(dt.Rows[0]["carCompare"].ToString());
                    if (!string.IsNullOrEmpty(carCompare))
                    {
                        result = carCompare.Split(',').ToList();
                        existShops = carShopEntities.Shops.Where(x => result.Contains(x.seq.ToString()) && x.status == "上架中").Select(x => x.seq).ToList();
                    }

                    foreach (var item in existShops)
                    {
                        ja.Add(item.ToString());
                    }
                }


                var data = new
                {
                    Count = result.Count(),
                    Data = ja
                };

                return ReturnOK(data);
            }
            catch (Exception ex)
            {
                APCommonFun.Error("[GetCarCompareController]99：" + ex.ToString());
                return ReturnException();
            }
        }
    }
}
