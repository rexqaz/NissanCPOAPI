using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using WebApplication.Extensions;

namespace WebApplication.Controllers.api
{
    /// <summary>
    /// 取得上架中的車款
    /// </summary>
    [JwtAuthActionFilter]
    public class GetReleaseCarModelController : BaseAPIController
    {
        /// <summary>
        /// 取得上架中的車款
        /// </summary>
        /// <param name="brand">品牌</param>
        /// <returns></returns>
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public object Get(string brand)
        {
            
            try
            {
                if (string.IsNullOrWhiteSpace(brand))
                {
                    return ReturnError("執行動作錯誤-請帶入brand欄位");
                }

                var carModelList = new List<string>();

                var dicValue = new Dictionary<string, string>();
                dicValue.Add("@brand", brand);
                string sql = "select carModel from Shops where brand  = @brand and status = N'上架中' group by carModel order by carModel";
                
                DataTable dt = APCommonFun.GetDataTable_MSSQL(sql, dicValue);
                foreach (DataRow item in dt.Rows)
                {
                    carModelList.Add(item["carModel"].ToString());
                }

                return ReturnOK(new { carModelList });
            }
            catch (Exception ex)
            {
                APCommonFun.Error("[GetReleaseCarModelController]99：" + ex.ToString());
                return ReturnException();
            }
        }
    }
}
