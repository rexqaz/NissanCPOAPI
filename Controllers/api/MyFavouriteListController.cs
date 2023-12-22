using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using WebApplication.Extensions;
using WebApplication.ViewModels;

namespace WebApplication.Controllers.api
{
    [JwtAuthActionFilter]
    public class MyFavouriteListController : BaseAPIController
    {
        /// <summary>
        /// 取得我的最愛
        /// </summary>
        /// <param name="Data"></param>
        /// <returns></returns>
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public object Post(Info_MyFavouriteListModel Data)
        {
            try
            {

                string InputIsok = "Y";
                string ReturnErr = "";
                string user_id = string.Empty;

                if (Data.user_id != null)
                {
                    user_id = APCommonFun.CDBNulltrim(Data.user_id);
                }

                //第一步 : 先判斷有沒有必填未填寫，
                if (user_id == "") //必填                       
                {
                    InputIsok = "N";
                    ReturnErr = "執行動作錯誤-user_id 為必填欄位";
                }

                //第二步 : 如果有必填未給值，回傳告知必填欄位要有資料
                if (InputIsok == "N")
                {
                    APCommonFun.Error("[MyFavouriteListController]90-" + ReturnErr);
                    return ReturnError(ReturnErr);
                }

                JObject tmpJoLay01 = new JObject();
                JArray newJa2 = new JArray();

                string sql1 = "select * from MyFavourites where user_id =@user_id   ";

                DataTable dt2 = APCommonFun.GetSafeDataTable_MSSQL(
                    sql1,
                    new List<SqlParameter>
                    {
                                        new SqlParameter("@user_id", user_id)
                    }
                );
                //DataTable dt2 = APCommonFun.GetDataTable_MSSQL(sql1);
                if (dt2.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt2.Rows)
                    {
                        string carShop_id = APCommonFun.CDBNulltrim(dr["carShop_id"].ToString());

                        //tmpJoLay01.Add(new JProperty("carShop_id", carShop_id));
                        newJa2.Add(carShop_id);
                    }
                }


                var data = new
                {
                    Data = newJa2
                };
                return ReturnOK(data);
            }
            catch (Exception ex)
            {
                APCommonFun.Error("[MyFavouriteListController]99：" + ex.ToString());
                return ReturnException();
            }
        }

    }
}
