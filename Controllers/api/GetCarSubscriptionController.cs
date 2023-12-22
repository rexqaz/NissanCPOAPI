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
using WebApplication.ViewModels;

namespace WebApplication.Controllers.api
{
    [JwtAuthActionFilter]
    public class GetCarSubscriptionController : BaseAPIController
    {
        /// <summary>
        /// 取得訂閱車輛
        /// </summary>
        /// <param name="Data"></param>
        /// <returns></returns>
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public object Post(Info_GetCarSubscriptionModel Data)
        {
            string InputIsok = "Y";
            string ReturnErr = "";
            JObject tmpJoLay01 = new JObject();

            try
            {

                string user_id = string.Empty; //若無則全部回傳
                if (Data.user_id != null)
                {
                    user_id = APCommonFun.CDBNulltrim(Data.user_id);
                }

                string brand = string.Empty; //若無則全部回傳
                if (Data.brand != null)
                {
                    brand = APCommonFun.CDBNulltrim(Data.brand);
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
                    APCommonFun.Error("[GetCarSubscriptionController]90-" + ReturnErr);
                    return ReturnError(ReturnErr);
                }

                string sql = string.Empty;

                sql = "select top 1  * from Subscriptions where user_id=@user_id  and brand=@brand  order by createTime desc ";

                DataTable dt = APCommonFun.GetSafeDataTable_MSSQL(
                    sql,
                    new List<SqlParameter>
                    {
                        new SqlParameter("@user_id", user_id),
                        new SqlParameter("@brand", brand)
                    }
                );
                //DataTable dt = APCommonFun.GetDataTable_MSSQL(sql);
                if (dt.Rows.Count > 0)
                {
                    string driveMode = APCommonFun.CDBNulltrim(dt.Rows[0]["driveMode"].ToString());
                    string carType = APCommonFun.CDBNulltrim(dt.Rows[0]["carType"].ToString());
                    string carModel = APCommonFun.CDBNulltrim(dt.Rows[0]["carModel"].ToString());
                    string outerColor = APCommonFun.CDBNulltrim(dt.Rows[0]["outerColor"].ToString());
                    string area = APCommonFun.CDBNulltrim(dt.Rows[0]["area"].ToString());
                    string dealer = APCommonFun.CDBNulltrim(dt.Rows[0]["dealer"].ToString());
                    string price = APCommonFun.CDBNulltrim(dt.Rows[0]["priceStr"].ToString());
                    string milage = APCommonFun.CDBNulltrim(dt.Rows[0]["milageStr"].ToString());
                    string yearOfManufacture = APCommonFun.CDBNulltrim(dt.Rows[0]["yearOfManufactureStr"].ToString());


                    JArray newJa02 = new JArray();
                    string[] driveModeArray = driveMode.Split(',');
                    foreach (string item in driveModeArray)
                    {
                        newJa02.Add(item);
                    }
                    tmpJoLay01.Add(new JProperty("driveMode", newJa02));

                    JArray newJa03 = new JArray();
                    string[] carTypeArray = carType.Split(',');
                    foreach (string item in carTypeArray)
                    {
                        newJa03.Add(item);
                    }
                    tmpJoLay01.Add(new JProperty("carType", newJa03));

                    JArray newJa04 = new JArray();
                    string[] carModelArray = carModel.Split(',');
                    foreach (string item in carModelArray)
                    {
                        newJa04.Add(item);
                    }
                    tmpJoLay01.Add(new JProperty("carModel", newJa04));

                    JArray newJa05 = new JArray();
                    string[] outerColorArray = outerColor.Split(',');
                    foreach (string item in outerColorArray)
                    {
                        newJa05.Add(item);
                    }
                    tmpJoLay01.Add(new JProperty("outerColor", newJa05));

                    JArray newJa06 = new JArray();
                    string[] areaArray = area.Split(',');
                    foreach (string item in areaArray)
                    {
                        newJa06.Add(item);
                    }
                    tmpJoLay01.Add(new JProperty("area", newJa06));

                    JArray newJa07 = new JArray();
                    string[] dealerArray = dealer.Split(',');
                    foreach (string item in dealerArray)
                    {
                        newJa07.Add(item);
                    }
                    tmpJoLay01.Add(new JProperty("dealer", newJa07));

                    JArray newJa08 = new JArray();
                    string[] priceArray = price.Split(',');
                    foreach (string item in priceArray)
                    {
                        newJa08.Add(Convert.ToInt32(item));
                    }
                    tmpJoLay01.Add(new JProperty("price", newJa08));

                    JArray newJa09 = new JArray();
                    string[] milageArray = milage.Split(',');
                    foreach (string item in milageArray)
                    {
                        newJa09.Add(Convert.ToInt32(item));
                    }
                    tmpJoLay01.Add(new JProperty("milage", newJa09));

                    JArray newJa10 = new JArray();
                    string[] yearOfManufactureArray = yearOfManufacture.Split(',');
                    foreach (string item in yearOfManufactureArray)
                    {
                        newJa10.Add(Convert.ToInt32(item));
                    }
                    tmpJoLay01.Add(new JProperty("yearOfManufacture", newJa10));
                }


                var data = new
                {
                    Data = tmpJoLay01
                };

                return ReturnOK(data);
            }
            catch (Exception ex)
            {
                APCommonFun.Error("[GetCarSubscriptionController]99：" + ex.ToString());
                return ReturnException();
            }
        }
    }
}
