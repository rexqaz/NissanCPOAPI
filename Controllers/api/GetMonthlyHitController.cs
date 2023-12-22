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
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using WebApplication.Extensions;
using WebApplication.Models;
using WebApplication.ViewModels;

namespace WebApplication.Controllers.api
{
    [JwtAuthActionFilter]
    public class GetMonthlyHitController : BaseAPIController
    {
        /// <summary>
        /// 取得本月主打
        /// </summary>
        /// <param name="Data"></param>
        /// <returns></returns>
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public object Post(Info_GetMonthlyHitModel Data)
        {
            try
            {

                string InputIsok = "Y";
                string ReturnErr = "";
                JArray newJa = new JArray();
                JArray totalJa = new JArray();
                string fetch_subStr = string.Empty;
                CarShopEntities carShopEntities = new CarShopEntities();

                string seq = string.Empty; //若無則全部回傳
                string brandType = string.Empty;
                if (Data.seq != null)
                {
                    seq = APCommonFun.CDBNulltrim(Data.seq);
                }
                if (Data.brand != null)
                {
                    brandType = APCommonFun.CDBNulltrim(Data.brand);
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
                        APCommonFun.Error("[GetMonthlyHitController]90-" + ReturnErr);
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

                if (string.IsNullOrEmpty(brandType))
                {
                    return ReturnError("執行動作錯誤-brand 請帶入欄位");
                }


                string sql = $"select * from Shops where isHit='Y' and brand=@brand";
                if (!string.IsNullOrEmpty(seq))
                {
                    sql = sql + " and seq = @seq   ";
                }

                sql += " order by hitOrder  ";// + fetch_subStr;


                //DataTable dt = APCommonFun.GetDataTable_MSSQL(sql);
                DataTable dt = new DataTable();
                if (!string.IsNullOrEmpty(seq))
                {
                    dt = APCommonFun.GetSafeDataTable_MSSQL(
                        sql,
                        new List<SqlParameter>
                        {
                            new SqlParameter("@seq", seq),
                            new SqlParameter("@brand", brandType),
                        }
                    );
                }
                else
                {
                    dt = APCommonFun.GetSafeDataTable_MSSQL(
                            sql,
                             new List<SqlParameter>
                             {
                                new SqlParameter("@brand", brandType),
                             }
                        );
                }
                if (dt.Rows.Count > 0)
                {
                    var fileService = new Services.FileService();
                    if (string.IsNullOrEmpty(page))
                    {
                        foreach (DataRow dr in dt.Rows)
                        {
                            string seqShop = APCommonFun.CDBNulltrim(dr["seq"].ToString());
                            string brand = APCommonFun.CDBNulltrim(dr["brand"].ToString());
                            string price = APCommonFun.CDBNulltrim(dr["price"].ToString());
                            string licensePlate = APCommonFun.CDBNulltrim(dr["licensePlate"].ToString());
                            string licensePicture = APCommonFun.CDBNulltrim(dr["licensePicture"].ToString());
                            string milage = APCommonFun.CDBNulltrim(dr["milage"].ToString());
                            string yearOfManufacture = APCommonFun.CDBNulltrim(dr["yearOfManufacture"].ToString());
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
                            string isFrame = APCommonFun.CDBNulltrim(dr["isFrame"].ToString());
                            string hitOrder = APCommonFun.CDBNulltrim(dr["hitOrder"].ToString());
                            string status = APCommonFun.CDBNulltrim(dr["status"].ToString());
                            DateTime updateTime = DateTime.Parse(APCommonFun.CDBNulltrim(dr["updateTime"].ToString()));
                            DateTime today = DateTime.Now;
                            TimeSpan ts = DateTime.Now.Subtract(updateTime);
                            double NumberOfDays = ts.TotalDays;
                            string ShopNo = APCommonFun.CDBNulltrim(dr["ShopNo"].ToString());
                            string outEquip = APCommonFun.CDBNulltrim(dr["outEquip"].ToString());



                            var prepaid = carShopEntities.Prepaid.Where(x => x.shopNo == ShopNo).FirstOrDefault();

                            if (status == "上架中" || (status == "成交下架" && NumberOfDays <= 3))
                            {
                                JObject tmpJoLay01 = new JObject();
                                tmpJoLay01.Add(new JProperty("shop_id", seqShop));
                                tmpJoLay01.Add(new JProperty("price", price));
                                tmpJoLay01.Add(new JProperty("licensePlate", licensePlate));
                                tmpJoLay01.Add(new JProperty("licensePicture", licensePicture));
                                tmpJoLay01.Add(new JProperty("milage", milage));
                                tmpJoLay01.Add(new JProperty("yearOfManufacture", yearOfManufacture));
                                tmpJoLay01.Add(new JProperty("carModel", carModel));
                                tmpJoLay01.Add(new JProperty("carType", carType));
                                tmpJoLay01.Add(new JProperty("driveMode", driveMode));
                                tmpJoLay01.Add(new JProperty("transmissionType", transmissionType));
                                tmpJoLay01.Add(new JProperty("fuelType", fuelType));
                                tmpJoLay01.Add(new JProperty("displacement", displacement));
                                tmpJoLay01.Add(new JProperty("horsepower", horsepower));
                                tmpJoLay01.Add(new JProperty("outerColor", outerColor));
                                tmpJoLay01.Add(new JProperty("feature", feature));
                                tmpJoLay01.Add(new JProperty("dealer", dealer));
                                tmpJoLay01.Add(new JProperty("stronghold", stronghold));
                                tmpJoLay01.Add(new JProperty("salesRep", salesRep));
                                tmpJoLay01.Add(new JProperty("isFrame", isFrame));
                                tmpJoLay01.Add(new JProperty("hitOrder", hitOrder));
                                JArray newJa02 = new JArray();
                                string[] featureArray = feature.Split(',');
                                foreach (string featureStr in featureArray)
                                {
                                    newJa02.Add(featureStr);
                                }

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

                                jaPicture.Add(fileService.GetRealUrl(carCondition1));
                                jaPicture.Add(fileService.GetRealUrl(carCondition2));
                                jaPicture.Add(fileService.GetRealUrl(carCondition3));
                                jaPicture.Add(fileService.GetRealUrl(carCondition4));
                                jaPicture.Add(fileService.GetRealUrl(carCondition5));
                                jaPicture.Add(fileService.GetRealUrl(carCondition6));
                                jaPicture.Add(fileService.GetRealUrl(carCondition7));
                                jaPicture.Add(fileService.GetRealUrl(carCondition8));
                                jaPicture.Add(fileService.GetRealUrl(carCondition9));
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
                            string brand = APCommonFun.CDBNulltrim(dr["brand"].ToString());
                            string price = APCommonFun.CDBNulltrim(dr["price"].ToString());
                            string licensePlate = APCommonFun.CDBNulltrim(dr["licensePlate"].ToString());
                            string licensePicture = APCommonFun.CDBNulltrim(dr["licensePicture"].ToString());
                            string milage = APCommonFun.CDBNulltrim(dr["milage"].ToString());
                            string yearOfManufacture = APCommonFun.CDBNulltrim(dr["yearOfManufacture"].ToString());
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
                            string isFrame = APCommonFun.CDBNulltrim(dr["isFrame"].ToString());
                            string hitOrder = APCommonFun.CDBNulltrim(dr["hitOrder"].ToString());
                            string status = APCommonFun.CDBNulltrim(dr["status"].ToString());
                            DateTime updateTime = DateTime.Parse(APCommonFun.CDBNulltrim(dr["updateTime"].ToString()));
                            DateTime today = DateTime.Now;
                            TimeSpan ts = DateTime.Now.Subtract(updateTime);
                            double NumberOfDays = ts.TotalDays;
                            string ShopNo = APCommonFun.CDBNulltrim(dr["ShopNo"].ToString());
                            string outEquip = APCommonFun.CDBNulltrim(dr["outEquip"].ToString());



                            var prepaid = carShopEntities.Prepaid.Where(x => x.shopNo == ShopNo).FirstOrDefault();

                            if (status == "上架中" || (status == "成交下架" && NumberOfDays <= 3))
                            {
                                JObject tmpJoLay01 = new JObject();
                                tmpJoLay01.Add(new JProperty("shop_id", seqShop));
                                tmpJoLay01.Add(new JProperty("price", price));
                                tmpJoLay01.Add(new JProperty("licensePlate", licensePlate));
                                tmpJoLay01.Add(new JProperty("licensePicture", licensePicture));
                                tmpJoLay01.Add(new JProperty("milage", milage));
                                tmpJoLay01.Add(new JProperty("yearOfManufacture", yearOfManufacture));
                                tmpJoLay01.Add(new JProperty("carModel", carModel));
                                tmpJoLay01.Add(new JProperty("carType", carType));
                                tmpJoLay01.Add(new JProperty("driveMode", driveMode));
                                tmpJoLay01.Add(new JProperty("transmissionType", transmissionType));
                                tmpJoLay01.Add(new JProperty("fuelType", fuelType));
                                tmpJoLay01.Add(new JProperty("displacement", displacement));
                                tmpJoLay01.Add(new JProperty("horsepower", horsepower));
                                tmpJoLay01.Add(new JProperty("outerColor", outerColor));
                                tmpJoLay01.Add(new JProperty("feature", feature));
                                tmpJoLay01.Add(new JProperty("dealer", dealer));
                                tmpJoLay01.Add(new JProperty("stronghold", stronghold));
                                tmpJoLay01.Add(new JProperty("salesRep", salesRep));
                                tmpJoLay01.Add(new JProperty("isFrame", isFrame));
                                tmpJoLay01.Add(new JProperty("hitOrder", hitOrder));
                                JArray newJa02 = new JArray();
                                string[] featureArray = feature.Split(',');
                                foreach (string featureStr in featureArray)
                                {
                                    newJa02.Add(featureStr);
                                }

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
                                jaPicture.Add(fileService.GetRealUrl(carCondition1));
                                jaPicture.Add(fileService.GetRealUrl(carCondition2));
                                jaPicture.Add(fileService.GetRealUrl(carCondition3));
                                jaPicture.Add(fileService.GetRealUrl(carCondition4));
                                jaPicture.Add(fileService.GetRealUrl(carCondition5));
                                jaPicture.Add(fileService.GetRealUrl(carCondition6));
                                jaPicture.Add(fileService.GetRealUrl(carCondition7));
                                jaPicture.Add(fileService.GetRealUrl(carCondition8));
                                jaPicture.Add(fileService.GetRealUrl(carCondition9));
                                jaPicture.Add(fileService.GetRealUrl(carCondition10));
                                tmpJoLay01.Add(new JProperty("carPictures", jaPicture));
                                tmpJoLay01.Add(new JProperty("inspectionTable", fileService.GetRealUrl(inspectionTable)));

                                if ((i >= (page_start - 1) * page_size && i < (page_start - 1) * page_size + page_size))
                                {
                                    newJa.Add(tmpJoLay01);
                                }
                                i++;
                                totalJa.Add(tmpJoLay01);
                            }

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
                APCommonFun.Error("[GetMonthlyHitController]99：" + ex.ToString());
                return ReturnException();
            }
        }
    }
}
