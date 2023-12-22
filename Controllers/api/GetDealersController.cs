using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
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
    public class GetDealersController : BaseAPIController
    {
        /// <summary>
        /// 取得經銷商資料
        /// </summary>
        /// <param name="Data"></param>
        /// <returns></returns>
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public object Post(Info_DealerModel Data)
        {
            string InputIsok = "Y";
            string ReturnErr = "";
            JArray newJa = new JArray();
            JArray totalJa = new JArray();
            string fetch_subStr = string.Empty;
            var dicValue = new Dictionary<string, string>();

            try
            {
                string brand = string.Empty;
                if (Data.brand != null)
                {
                    brand = APCommonFun.CDBNulltrim(Data.brand);
                }

                string dealer_name = string.Empty;
                if (Data.dealer_name != null)
                {
                    dealer_name = APCommonFun.CDBNulltrim(Data.dealer_name);
                }

                string dealer_id = string.Empty;
                string dealer_condition = " and 1=1 ";
                if (Data.dealer_id != null)
                {
                    dealer_id = APCommonFun.CDBNulltrim(Data.dealer_id);
                    if (!string.IsNullOrEmpty(dealer_id))
                    {
                        dealer_condition = " and seq=@seq ";
                        dicValue.Add("@seq", dealer_id);
                    }
                }

                string areaConditionResult = string.Empty;
                if (Data.areas != null && Data.areas.Count > 0)
                {
                    List<string> areaContition = new List<string>();                    
                    for (int i = 0; i < Data.areas.Count; i++)
                    {
                        areaContition.Add($"@area{i}");
                        dicValue.Add($"@area{i}", Data.areas[i]);
                    }
                    areaConditionResult = $" and area in ({String.Join(",", areaContition.ToArray())}) ";
                }

                string type = string.Empty;
                if (Data.type != null)
                {
                    type = APCommonFun.CDBNulltrim(Data.type);
                }

                string page = string.Empty;
                int page_start = 0;
                int page_size = 0;
                if (Data.page != null && !string.IsNullOrEmpty(Data.page))
                {
                    page = APCommonFun.CDBNulltrim(Data.page);
                    if (!page.Contains(","))
                    {
                        ReturnErr = "執行動作錯誤-page 欄位格式錯誤";
                        APCommonFun.Error("[GetDealersController]90-" + ReturnErr);
                        return ReturnError(ReturnErr);
                    }
                    else
                    {
                        string[] page_data = page.Split(',');
                        fetch_subStr = "OFFSET " + ((Convert.ToInt32(page_data[0].ToString()) - 1) * Convert.ToInt32(page_data[1].ToString())).ToString() + " rows fetch first " + page_data[1].ToString() + " rows only ";
                        page_start = Convert.ToInt32(page_data[0].ToString());
                        page_size = Convert.ToInt32(page_data[1].ToString());
                    }
                }

                //第一步 : 先判斷有沒有必填未填寫，
                if (brand == "") //必填                       
                {
                    InputIsok = "N";
                    ReturnErr = "執行動作錯誤-brand 為必填欄位";
                }

                //第二步 : 如果有必填未給值，回傳告知必填欄位要有資料
                if (InputIsok == "N")
                {
                    APCommonFun.Error("[GetDealersController]90-" + ReturnErr);
                    return ReturnError(ReturnErr);
                }

               
                dicValue.Add("@brand", brand);
                dicValue.Add("@type", type);                                
                string sql = "select * from Dealers where brand=@brand and type=@type " + dealer_condition + (string.IsNullOrEmpty(areaConditionResult) ? "" : areaConditionResult);
                string today = DateTime.Now.ToString("yyyy-MM-dd");

                if (type == "new" && !string.IsNullOrEmpty(dealer_name))
                {
                    sql += " and  dealerName=@dealerName ";
                    dicValue.Add("@dealerName", dealer_name);
                }
                sql += " order by createTime desc ";// + fetch_subStr;

                DataTable dt = APCommonFun.GetDataTable_MSSQL(sql, dicValue);

                string sqlShop = string.Empty;
                DataTable dtShop = new DataTable();
                if (dt.Rows.Count > 0)
                {
                    if (string.IsNullOrEmpty(page))
                    {
                        foreach (DataRow dr in dt.Rows)
                        {
                            string seq = APCommonFun.CDBNulltrim(dr["seq"].ToString());
                            string dealerName = APCommonFun.CDBNulltrim(dr["dealerName"].ToString());
                            string businessOffice = APCommonFun.CDBNulltrim(dr["businessOffice"].ToString());
                            string businessOffice2 = APCommonFun.CDBNulltrim(dr["businessOffice2"].ToString());
                            string telAreaCode = APCommonFun.CDBNulltrim(dr["telAreaCode"].ToString());
                            string tel = APCommonFun.CDBNulltrim(dr["tel"].ToString());
                            string area = APCommonFun.CDBNulltrim(dr["area"].ToString());
                            string address = APCommonFun.CDBNulltrim(dr["address"].ToString());
                            string gmap = APCommonFun.CDBNulltrim(dr["gmap"].ToString());
                            string email = APCommonFun.CDBNulltrim(dr["email"].ToString());
                            string businessStartHourDay = APCommonFun.CDBNulltrim(dr["businessStartHourDay"].ToString());
                            string businessEndHourDay = APCommonFun.CDBNulltrim(dr["businessEndHourDay"].ToString());
                            string businessStartHourNight = APCommonFun.CDBNulltrim(dr["businessStartHourNight"].ToString());
                            string businessEndHourNight = APCommonFun.CDBNulltrim(dr["businessEndHourNight"].ToString());
                            string busDays = APCommonFun.CDBNulltrim(dr["busDay"].ToString());
                            string picture = APCommonFun.CDBNulltrim(dr["picture"].ToString());

                            JObject tmpJoLay01 = new JObject();
                            tmpJoLay01.Add(new JProperty("seq", seq));
                            tmpJoLay01.Add(new JProperty("dealerName", dealerName));
                            tmpJoLay01.Add(new JProperty("businessOffice", businessOffice));
                            tmpJoLay01.Add(new JProperty("businessOffice2", businessOffice2));
                            tmpJoLay01.Add(new JProperty("area", area));
                            tmpJoLay01.Add(new JProperty("telAreaCode", telAreaCode));
                            tmpJoLay01.Add(new JProperty("tel", tel));
                            tmpJoLay01.Add(new JProperty("address", address));
                            tmpJoLay01.Add(new JProperty("gmap", gmap));
                            tmpJoLay01.Add(new JProperty("email", email));
                            tmpJoLay01.Add(new JProperty("businessStartHourDay", businessStartHourDay));
                            tmpJoLay01.Add(new JProperty("businessEndHourDay", businessEndHourDay));
                            tmpJoLay01.Add(new JProperty("businessStartHourNight", businessStartHourNight));
                            tmpJoLay01.Add(new JProperty("businessEndHourNight", businessEndHourNight));
                            if (!string.IsNullOrEmpty(busDays))
                            {
                                JArray ja = new JArray();
                                List<string> busDayArray = busDays.Split(',').ToList();
                                foreach (var item in busDayArray)
                                {
                                    ja.Add(item);
                                }

                                tmpJoLay01.Add(new JProperty("busDays", ja));
                            }


                            tmpJoLay01.Add(new JProperty("picture", new Services.FileService().GetRealUrl(picture)));


                            sqlShop = "select * from Shops where brand =@brand and status = N'上架中' and dealer=@dealer and stronghold=@stronghold ";
                            dicValue.Clear();
                            dicValue.Add("@brand", brand);
                            dicValue.Add("@dealer", dealerName);
                            dicValue.Add("@stronghold", businessOffice);
                            dtShop = APCommonFun.GetDataTable_MSSQL(sqlShop, dicValue);
                            tmpJoLay01.Add(new JProperty("shopCount", dtShop.Rows.Count.ToString()));

                            newJa.Add(tmpJoLay01);
                            totalJa.Add(tmpJoLay01);
                        }
                    }
                    else
                    {
                        int i = 0;
                        foreach (DataRow dr in dt.Rows)
                        {
                            string seq = APCommonFun.CDBNulltrim(dr["seq"].ToString());
                            string dealerName = APCommonFun.CDBNulltrim(dr["dealerName"].ToString());
                            string businessOffice = APCommonFun.CDBNulltrim(dr["businessOffice"].ToString());
                            string businessOffice2 = APCommonFun.CDBNulltrim(dr["businessOffice2"].ToString());
                            string telAreaCode = APCommonFun.CDBNulltrim(dr["telAreaCode"].ToString());
                            string tel = APCommonFun.CDBNulltrim(dr["tel"].ToString());
                            string area = APCommonFun.CDBNulltrim(dr["area"].ToString());
                            string address = APCommonFun.CDBNulltrim(dr["address"].ToString());
                            string gmap = APCommonFun.CDBNulltrim(dr["gmap"].ToString());
                            string email = APCommonFun.CDBNulltrim(dr["email"].ToString());
                            string businessStartHourDay = APCommonFun.CDBNulltrim(dr["businessStartHourDay"].ToString());
                            string businessEndHourDay = APCommonFun.CDBNulltrim(dr["businessEndHourDay"].ToString());
                            string businessStartHourNight = APCommonFun.CDBNulltrim(dr["businessStartHourNight"].ToString());
                            string businessEndHourNight = APCommonFun.CDBNulltrim(dr["businessEndHourNight"].ToString());
                            string busDays = APCommonFun.CDBNulltrim(dr["busDay"].ToString());
                            string picture = APCommonFun.CDBNulltrim(dr["picture"].ToString());

                            JObject tmpJoLay01 = new JObject();
                            tmpJoLay01.Add(new JProperty("seq", seq));
                            tmpJoLay01.Add(new JProperty("dealerName", dealerName));
                            tmpJoLay01.Add(new JProperty("businessOffice", businessOffice));
                            tmpJoLay01.Add(new JProperty("businessOffice2", businessOffice2));
                            tmpJoLay01.Add(new JProperty("area", area));
                            tmpJoLay01.Add(new JProperty("tel", tel));
                            tmpJoLay01.Add(new JProperty("telAreaCode", telAreaCode));
                            tmpJoLay01.Add(new JProperty("address", address));
                            tmpJoLay01.Add(new JProperty("gmap", gmap));
                            tmpJoLay01.Add(new JProperty("email", email));
                            tmpJoLay01.Add(new JProperty("businessStartHourDay", businessStartHourDay));
                            tmpJoLay01.Add(new JProperty("businessEndHourDay", businessEndHourDay));
                            tmpJoLay01.Add(new JProperty("businessStartHourNight", businessStartHourNight));
                            tmpJoLay01.Add(new JProperty("businessEndHourNight", businessEndHourNight));
                            sqlShop = "select * from Shops where brand = @brand and status = N'上架中' and dealer=@dealer and stronghold=@stronghold ";

                            dicValue.Clear();
                            dicValue.Add("@brand", brand);
                            dicValue.Add("@dealer", dealerName);
                            dicValue.Add("@stronghold", businessOffice);

                            dtShop = APCommonFun.GetDataTable_MSSQL(sqlShop, dicValue);
                            tmpJoLay01.Add(new JProperty("shopCount", dtShop.Rows.Count.ToString()));
                            tmpJoLay01.Add(new JProperty("picture", new Services.FileService().GetRealUrl(picture)));

                            totalJa.Add(tmpJoLay01);
                            if ((i >= (page_start - 1) * page_size && i < (page_start - 1) * page_size + page_size))
                            {
                                newJa.Add(tmpJoLay01);
                            }
                            i++;
                        }
                    }
                }

                var data = new
                {
                    Count = totalJa.Count,
                    Data = newJa
                };

                return ReturnOK(data);
            }
            catch (Exception ex)
            {
                APCommonFun.Error("[GetDealersController]99：" + ex.ToString());
                return ReturnException();
            }
        }
    }
}
