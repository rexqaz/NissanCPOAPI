using Jose;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using WebApplication.Extensions;
using WebApplication.Models;
using WebApplication.ViewModels;

namespace WebApplication.Controllers.api
{
    [JwtAuthActionFilter]
    public class GetCarShopController : BaseAPIController
    {
        /// <summary>
        /// 取得車輛出售資訊
        /// </summary>
        /// <param name="Data"></param>
        /// <returns></returns>
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public object Post(Info_GetCarShopModel Data)
        {
            string InputIsok = "Y";
            string ReturnErr = "";
            JArray newJa = new JArray();
            JArray totalJa = new JArray();
            string fetch_subStr = string.Empty;
            CarShopEntities carShopEntities = new CarShopEntities();
            var dicValue = new Dictionary<string, string>();

            try
            {

               


                string brand = string.Empty;
                if (Data.brand != null)
                {
                    brand = APCommonFun.CDBNulltrim(Data.brand);
                }

                string strongholdQry = string.Empty;
                if (Data.stronghold != null)
                {
                    strongholdQry = APCommonFun.CDBNulltrim(Data.stronghold);
                }

                string dealerQry = string.Empty;
                if (Data.dealer != null)
                {
                    dealerQry = APCommonFun.CDBNulltrim(Data.dealer);
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
                        APCommonFun.Error("[GetCarShopController]90-" + ReturnErr);
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

                ////第一步 : 先判斷有沒有必填未填寫，
                if (brand == "") //必填                       
                {
                    InputIsok = "N";
                    ReturnErr = "執行動作錯誤-brand 為必填欄位";
                }

                //第二步 : 如果有必填未給值，回傳告知必填欄位要有資料
                if (InputIsok == "N")
                {
                    APCommonFun.Error("[GetCarShopController]90-" + ReturnErr);
                    return ReturnError(ReturnErr);
                }

                string sql = "select * from Shops where 1=1 and brand=@brand ";
                dicValue.Add("@brand", brand);

                if (Data.seq.Count > 0)
                {
                    List<string> stringList = new List<string>();
                    for (int i = 0; i < Data.seq.Count; i++)
                    {
                        stringList.Add($"@seq{i}");
                        dicValue.Add($"@seq{i}", Data.seq[i]);
                    }
                    sql += $" and seq in ({String.Join(",", stringList.ToArray())}) ";
                }

                if (!string.IsNullOrEmpty(dealerQry) && !string.IsNullOrEmpty(strongholdQry))
                {
                    sql = sql + " and dealer=@dealerQry  and stronghold=@strongholdQry  ";
                    dicValue.Add("@dealerQry", dealerQry);
                    dicValue.Add("@strongholdQry", strongholdQry);
                }

                sql += " order by createTime desc ";// + fetch_subStr;


                DataTable dt = APCommonFun.GetDataTable_MSSQL(sql, dicValue);
                if (dt.Rows.Count > 0)
                {
                    //var request = HttpContext.Current.Request;
                    var fileService = new Services.FileService();
                    if (string.IsNullOrEmpty(page))
                    {
                        foreach (DataRow dr in dt.Rows)
                        {
                            string seqShop = APCommonFun.CDBNulltrim(dr["seq"].ToString());
                            string price = APCommonFun.CDBNulltrim(dr["price"].ToString());
                            string licensePlate = APCommonFun.CDBNulltrim(dr["licensePlate"].ToString());
                            string licensePicture = APCommonFun.CDBNulltrim(dr["licensePicture"].ToString());
                            string milage = APCommonFun.CDBNulltrim(dr["milage"].ToString());
                            string yearOfManufacture = APCommonFun.CDBNulltrim(dr["yearOfManufacture"].ToString());
                            string monthOfManufacture = APCommonFun.CDBNulltrim(dr["monthOfManufacture"].ToString());
                            string carModel = APCommonFun.CDBNulltrim(dr["carModel"].ToString());
                            string carType = APCommonFun.CDBNulltrim(dr["carType"].ToString());
                            string driveMode = APCommonFun.CDBNulltrim(dr["driveMode"].ToString());
                            string transmissionType = APCommonFun.CDBNulltrim(dr["transmissionType"].ToString());
                            string fuelType = APCommonFun.CDBNulltrim(dr["fuelType"].ToString());
                            string displacement = APCommonFun.CDBNulltrim(dr["displacement"].ToString());
                            string horsepower = APCommonFun.CDBNulltrim(dr["horsepower"].ToString());
                            string outerColor = APCommonFun.CDBNulltrim(dr["outerColor"].ToString());
                            string innerColor = APCommonFun.CDBNulltrim(dr["innerColor"].ToString());
                            string feature = APCommonFun.CDBNulltrim(dr["feature"].ToString());
                            string dealer = APCommonFun.CDBNulltrim(dr["dealer"].ToString());
                            string stronghold = APCommonFun.CDBNulltrim(dr["stronghold"].ToString());


                            string salesRep = APCommonFun.CDBNulltrim(dr["salesRep"].ToString());
                            string carCondition1 = APCommonFun.CDBNulltrim(dr["carCondition1"].ToString());
                            string carCondition2 = APCommonFun.CDBNulltrim(dr["carCondition2"].ToString());
                            string carCondition3 = APCommonFun.CDBNulltrim(dr["carCondition3"].ToString());
                            string carCondition4 = APCommonFun.CDBNulltrim(dr["carCondition4"].ToString());
                            string carCondition5 = APCommonFun.CDBNulltrim(dr["carCondition5"].ToString());
                            string carCondition6 = APCommonFun.CDBNulltrim(dr["carCondition6"].ToString());
                            string carCondition7 = APCommonFun.CDBNulltrim(dr["carCondition7"].ToString());
                            string carCondition8 = APCommonFun.CDBNulltrim(dr["carCondition8"].ToString());
                            string carCondition9 = APCommonFun.CDBNulltrim(dr["carCondition9"].ToString());

                            string carCondition10 = APCommonFun.CDBNulltrim(dr["carCondition10"].ToString());
                            string otherCondition1 = APCommonFun.CDBNulltrim(dr["otherCondition1"].ToString());
                            string otherCondition2 = APCommonFun.CDBNulltrim(dr["otherCondition2"].ToString());
                            string contact = APCommonFun.CDBNulltrim(dr["contact"].ToString());
                            string ListingDate = APCommonFun.CDBNulltrim(dr["ListingDate"].ToString());
                            string description = APCommonFun.CDBNulltrim(dr["description"].ToString());
                            string inspectionTable = APCommonFun.CDBNulltrim(dr["inspectionTable"].ToString());
                            string status = APCommonFun.CDBNulltrim(dr["status"].ToString());
                            string ShopNo = APCommonFun.CDBNulltrim(dr["ShopNo"].ToString());
                            string outEquip = APCommonFun.CDBNulltrim(dr["outEquip"].ToString());
                            DateTime updateTime = DateTime.Parse(APCommonFun.CDBNulltrim(dr["updateTime"].ToString()));
                            DateTime today = DateTime.Now;
                            TimeSpan ts = DateTime.Now.Subtract(updateTime);
                            double NumberOfDays = ts.TotalDays;

                            var prepaid = carShopEntities.Prepaid.Where(x => x.shopNo == ShopNo).FirstOrDefault();

                            if (status == "上架中" || (status == "成交下架" && NumberOfDays <= 3))
                            {
                                JObject tmpJoLay01 = new JObject();
                                tmpJoLay01.Add(new JProperty("shop_id", seqShop));
                                tmpJoLay01.Add(new JProperty("price", price));
                                tmpJoLay01.Add(new JProperty("milage", milage));
                                tmpJoLay01.Add(new JProperty("yearOfManufacture", yearOfManufacture));
                                tmpJoLay01.Add(new JProperty("monthOfManufacture", monthOfManufacture));
                                tmpJoLay01.Add(new JProperty("carModel", carModel));
                                tmpJoLay01.Add(new JProperty("carType", carType));
                                tmpJoLay01.Add(new JProperty("driveMode", driveMode));
                                tmpJoLay01.Add(new JProperty("fuelType", fuelType));
                                tmpJoLay01.Add(new JProperty("displacement", displacement));
                                tmpJoLay01.Add(new JProperty("horsepower", horsepower));
                                tmpJoLay01.Add(new JProperty("outerColor", outerColor));
                                JArray newJa02 = new JArray();
                                string[] featureArray = feature.Split(',');
                                foreach (string featureStr in featureArray)
                                {
                                    newJa02.Add(featureStr);
                                }
                                tmpJoLay01.Add(new JProperty("feature", newJa02));

                                tmpJoLay01.Add(new JProperty("dealer", dealer));
                                tmpJoLay01.Add(new JProperty("stronghold", stronghold));
                                string sqlDealer = "select * from Dealers where dealerName=@dealer  and businessOffice=@stronghold and brand=@brand ";

                                DataTable dtDealer = APCommonFun.GetSafeDataTable_MSSQL(
                                            sqlDealer,
                                            new List<SqlParameter>
                                            {
                                new SqlParameter("@brand", brand),
                                new SqlParameter("@dealer", dealer),
                                new SqlParameter("@stronghold", stronghold)
                                            }
                                );
                                if (dtDealer.Rows.Count > 0)
                                {
                                    tmpJoLay01.Add(new JProperty("dealer_id", dtDealer.Rows[0]["seq"].ToString()));
                                    tmpJoLay01.Add(new JProperty("dealer_area", dtDealer.Rows[0]["area"].ToString()));
                                }
                                else
                                {
                                    tmpJoLay01.Add(new JProperty("dealer_id", string.Empty));
                                    tmpJoLay01.Add(new JProperty("dealer_area", string.Empty));
                                }


                                tmpJoLay01.Add(new JProperty("salesRep", salesRep));
                                tmpJoLay01.Add(new JProperty("description", description));

                                JArray newJa03 = new JArray();
                                string[] outEquipArray = outEquip.Split(',');
                                foreach (string outEquipStr in outEquipArray)
                                {
                                    newJa03.Add(outEquipStr);
                                }
                                tmpJoLay01.Add(new JProperty("outEquip", newJa03));

                                if (prepaid != null && prepaid.paidStatus == "已付訂金")
                                {
                                    tmpJoLay01.Add(new JProperty("status", "已保留"));
                                }
                                else if (status != "上架中")
                                {
                                    tmpJoLay01.Add(new JProperty("status", "已售出"));
                                }
                                else
                                {
                                    tmpJoLay01.Add(new JProperty("status", status));
                                }


                                JArray jaPicture = new JArray();
                                if (string.IsNullOrEmpty(carCondition1) == false)                                
                                    jaPicture.Add(fileService.GetRealUrl(carCondition1));
                                if (string.IsNullOrEmpty(carCondition2) == false)
                                    jaPicture.Add(fileService.GetRealUrl(carCondition2));
                                if (string.IsNullOrEmpty(carCondition3) == false)
                                    jaPicture.Add(fileService.GetRealUrl(carCondition3));
                                if (string.IsNullOrEmpty(carCondition4) == false)
                                    jaPicture.Add(fileService.GetRealUrl(carCondition4));
                                if (string.IsNullOrEmpty(carCondition5) == false)
                                    jaPicture.Add(fileService.GetRealUrl(carCondition5));
                                if (string.IsNullOrEmpty(carCondition6) == false)
                                    jaPicture.Add(fileService.GetRealUrl(carCondition6));
                                if (string.IsNullOrEmpty(carCondition7) == false)
                                    jaPicture.Add(fileService.GetRealUrl(carCondition7));
                                if (string.IsNullOrEmpty(carCondition8) == false)
                                    jaPicture.Add(fileService.GetRealUrl(carCondition8));
                                if (string.IsNullOrEmpty(carCondition9) == false)
                                    jaPicture.Add(fileService.GetRealUrl(carCondition9));
                                if (string.IsNullOrEmpty(carCondition10) == false)
                                    jaPicture.Add(fileService.GetRealUrl(carCondition10));

                                tmpJoLay01.Add(new JProperty("carPictures", jaPicture));
                                tmpJoLay01.Add(new JProperty("inspectionTable", fileService.GetRealUrl(inspectionTable)));

                                
                                newJa.Add(tmpJoLay01);
                                totalJa.Add(tmpJoLay01);
                            }
                        }
                    }
                    else
                    {
                        int i = 0;
                        foreach (DataRow dr in dt.Rows)
                        {
                            string seqShop = APCommonFun.CDBNulltrim(dr["seq"].ToString());
                            string price = APCommonFun.CDBNulltrim(dr["price"].ToString());
                            string licensePlate = APCommonFun.CDBNulltrim(dr["licensePlate"].ToString());
                            string licensePicture = APCommonFun.CDBNulltrim(dr["licensePicture"].ToString());
                            string milage = APCommonFun.CDBNulltrim(dr["milage"].ToString());
                            string yearOfManufacture = APCommonFun.CDBNulltrim(dr["yearOfManufacture"].ToString());
                            string monthOfManufacture = APCommonFun.CDBNulltrim(dr["monthOfManufacture"].ToString());
                            string carModel = APCommonFun.CDBNulltrim(dr["carModel"].ToString());
                            string carType = APCommonFun.CDBNulltrim(dr["carType"].ToString());
                            string driveMode = APCommonFun.CDBNulltrim(dr["driveMode"].ToString());
                            string transmissionType = APCommonFun.CDBNulltrim(dr["transmissionType"].ToString());
                            string fuelType = APCommonFun.CDBNulltrim(dr["fuelType"].ToString());
                            string displacement = APCommonFun.CDBNulltrim(dr["displacement"].ToString());
                            string horsepower = APCommonFun.CDBNulltrim(dr["horsepower"].ToString());
                            string outerColor = APCommonFun.CDBNulltrim(dr["outerColor"].ToString());
                            string innerColor = APCommonFun.CDBNulltrim(dr["innerColor"].ToString());
                            string feature = APCommonFun.CDBNulltrim(dr["feature"].ToString());
                            string dealer = APCommonFun.CDBNulltrim(dr["dealer"].ToString());
                            string stronghold = APCommonFun.CDBNulltrim(dr["stronghold"].ToString());


                            string salesRep = APCommonFun.CDBNulltrim(dr["salesRep"].ToString());
                            string carCondition1 = APCommonFun.CDBNulltrim(dr["carCondition1"].ToString());
                            string carCondition2 = APCommonFun.CDBNulltrim(dr["carCondition2"].ToString());
                            string carCondition3 = APCommonFun.CDBNulltrim(dr["carCondition3"].ToString());
                            string carCondition4 = APCommonFun.CDBNulltrim(dr["carCondition4"].ToString());
                            string carCondition5 = APCommonFun.CDBNulltrim(dr["carCondition5"].ToString());
                            string carCondition6 = APCommonFun.CDBNulltrim(dr["carCondition6"].ToString());
                            string carCondition7 = APCommonFun.CDBNulltrim(dr["carCondition7"].ToString());
                            string carCondition8 = APCommonFun.CDBNulltrim(dr["carCondition8"].ToString());
                            string carCondition9 = APCommonFun.CDBNulltrim(dr["carCondition9"].ToString());

                            string carCondition10 = APCommonFun.CDBNulltrim(dr["carCondition10"].ToString());
                            string otherCondition1 = APCommonFun.CDBNulltrim(dr["otherCondition1"].ToString());
                            string otherCondition2 = APCommonFun.CDBNulltrim(dr["otherCondition2"].ToString());
                            string contact = APCommonFun.CDBNulltrim(dr["contact"].ToString());
                            string ListingDate = APCommonFun.CDBNulltrim(dr["ListingDate"].ToString());
                            string description = APCommonFun.CDBNulltrim(dr["description"].ToString());
                            string inspectionTable = APCommonFun.CDBNulltrim(dr["inspectionTable"].ToString());
                            string status = APCommonFun.CDBNulltrim(dr["status"].ToString());
                            string ShopNo = APCommonFun.CDBNulltrim(dr["ShopNo"].ToString());
                            string outEquip = APCommonFun.CDBNulltrim(dr["outEquip"].ToString());
                            DateTime updateTime = DateTime.Parse(APCommonFun.CDBNulltrim(dr["updateTime"].ToString()));
                            DateTime today = DateTime.Now;
                            TimeSpan ts = DateTime.Now.Subtract(updateTime);
                            double NumberOfDays = ts.TotalDays;

                            var prepaid = carShopEntities.Prepaid.Where(x => x.shopNo == ShopNo).FirstOrDefault();

                            if (status == "上架中" || (status == "成交下架" && NumberOfDays <= 3))
                            {
                                JObject tmpJoLay01 = new JObject();
                                tmpJoLay01.Add(new JProperty("shop_id", seqShop));
                                tmpJoLay01.Add(new JProperty("price", price));
                                tmpJoLay01.Add(new JProperty("milage", milage));
                                tmpJoLay01.Add(new JProperty("yearOfManufacture", yearOfManufacture));
                                tmpJoLay01.Add(new JProperty("monthOfManufacture", monthOfManufacture));
                                tmpJoLay01.Add(new JProperty("carModel", carModel));
                                tmpJoLay01.Add(new JProperty("carType", carType));
                                tmpJoLay01.Add(new JProperty("driveMode", driveMode));
                                tmpJoLay01.Add(new JProperty("fuelType", fuelType));
                                tmpJoLay01.Add(new JProperty("displacement", displacement));
                                tmpJoLay01.Add(new JProperty("horsepower", horsepower));
                                tmpJoLay01.Add(new JProperty("outerColor", outerColor));
                                JArray newJa02 = new JArray();
                                string[] featureArray = feature.Split(',');
                                foreach (string featureStr in featureArray)
                                {
                                    newJa02.Add(featureStr);
                                }
                                tmpJoLay01.Add(new JProperty("feature", newJa02));

                                tmpJoLay01.Add(new JProperty("dealer", dealer));
                                tmpJoLay01.Add(new JProperty("stronghold", stronghold));
                                string sqlDealer = "select * from Dealers where dealerName=@dealer  and businessOffice= @stronghold  ";
                                
                                DataTable dtDealer = APCommonFun.GetSafeDataTable_MSSQL(
                                             sqlDealer,
                                             new List<SqlParameter>
                                             {
                                new SqlParameter("@brand", brand),
                                new SqlParameter("@dealer", dealer),
                                new SqlParameter("@stronghold", stronghold)
                                             }
                                 );
                                if (dtDealer.Rows.Count > 0)
                                {
                                    tmpJoLay01.Add(new JProperty("dealer_id", dtDealer.Rows[0]["seq"].ToString()));
                                    tmpJoLay01.Add(new JProperty("dealer_area", dtDealer.Rows[0]["area"].ToString()));
                                }
                                tmpJoLay01.Add(new JProperty("salesRep", salesRep));
                                tmpJoLay01.Add(new JProperty("description", description));
                                JArray newJa03 = new JArray();
                                string[] outEquipArray = outEquip.Split(',');
                                foreach (string outEquipStr in outEquipArray)
                                {
                                    newJa03.Add(outEquipStr);
                                }
                                tmpJoLay01.Add(new JProperty("outEquip", newJa03));

                                if (prepaid != null && prepaid.paidStatus == "已付訂金")
                                {
                                    tmpJoLay01.Add(new JProperty("status", "已保留"));
                                }
                                else if (status != "上架中")
                                {
                                    tmpJoLay01.Add(new JProperty("status", "已售出"));
                                }
                                else
                                {
                                    tmpJoLay01.Add(new JProperty("status", status));
                                }


                                JArray jaPicture = new JArray();

                                if (string.IsNullOrEmpty(carCondition1) == false)
                                    jaPicture.Add(fileService.GetRealUrl(carCondition1));
                                if (string.IsNullOrEmpty(carCondition2) == false)
                                    jaPicture.Add(fileService.GetRealUrl(carCondition2));
                                if (string.IsNullOrEmpty(carCondition3) == false)
                                    jaPicture.Add(fileService.GetRealUrl(carCondition3));
                                if (string.IsNullOrEmpty(carCondition4) == false)
                                    jaPicture.Add(fileService.GetRealUrl(carCondition4));
                                if (string.IsNullOrEmpty(carCondition5) == false)
                                    jaPicture.Add(fileService.GetRealUrl(carCondition5));
                                if (string.IsNullOrEmpty(carCondition6) == false)
                                    jaPicture.Add(fileService.GetRealUrl(carCondition6));
                                if (string.IsNullOrEmpty(carCondition7) == false)
                                    jaPicture.Add(fileService.GetRealUrl(carCondition7));
                                if (string.IsNullOrEmpty(carCondition8) == false)
                                    jaPicture.Add(fileService.GetRealUrl(carCondition8));
                                if (string.IsNullOrEmpty(carCondition9) == false)
                                    jaPicture.Add(fileService.GetRealUrl(carCondition9));
                                if (string.IsNullOrEmpty(carCondition10) == false)
                                    jaPicture.Add(fileService.GetRealUrl(carCondition10));
                              
                                tmpJoLay01.Add(new JProperty("carPictures", jaPicture));
                                tmpJoLay01.Add(new JProperty("inspectionTable", fileService.GetRealUrl(inspectionTable)));


                                if ((i >= (page_start - 1) * page_size && i < (page_start - 1) * page_size + page_size))
                                {
                                    newJa.Add(tmpJoLay01);
                                }
                                totalJa.Add(tmpJoLay01);
                                i++;
                            }
                        }
                    }
                }

                var data = new
                {
                    Result = "T",
                    Message = "成功",
                    Count = totalJa.Count,
                    Data = newJa
                };

                return ReturnOK(data);
            }
            catch (Exception ex)
            {
                APCommonFun.Error("[GetCarShopController]99：" + ex.ToString());
                return ReturnException();
            }
        }
    }
}
