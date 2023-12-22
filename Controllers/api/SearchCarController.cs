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
    public class SearchCarController : BaseAPIController
    {
        /// <summary>
        /// 取得車輛出售資訊
        /// </summary>
        /// <param name="Data"></param>
        /// <returns></returns>
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public object Post(Info_SearchCarModel Data)
        {
            try
            {
                string InputIsok = "Y";
                string ReturnErr = "";
                string fetch_subStr = string.Empty;
                JArray newJa = new JArray();
                JArray totalJa = new JArray();
                CarShopEntities carShopEntities = new CarShopEntities();
                var dicValue = new Dictionary<string, string>();

                string brand = string.Empty;
                if (Data.brand != null)
                {
                    brand = APCommonFun.CDBNulltrim(Data.brand);
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
                        APCommonFun.Error("[SearchCarController]90-" + ReturnErr);
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
                    APCommonFun.Error("[SearchCarController]90-" + ReturnErr);
                    return ReturnError(ReturnErr);
                }

               
                string sql = "select * from Shops where brand=@brand  ";
                dicValue.Add("@brand", brand);

                if (Data.price.Count == 2)
                {
                    sql += " and (price >= @price1 and price <=  @price2) ";
                    dicValue.Add("@price1", Data.price[0].ToString());
                    dicValue.Add("@price2", Data.price[1].ToString());
                }

                if (Data.milage.Count == 2)
                {
                    sql += " and (milage >= @milage1 and milage <= @milage2 ) ";
                    dicValue.Add("@milage1", Data.milage[0].ToString());
                    dicValue.Add("@milage2", Data.milage[1].ToString());
                }

                if (Data.yearOfManufacture.Count == 2)
                {
                    sql += " and (yearOfManufacture >= @yearOfManufacture1 and yearOfManufacture <= @yearOfManufacture2 ) ";
                    dicValue.Add("@yearOfManufacture1", Data.yearOfManufacture[0].ToString());
                    dicValue.Add("@yearOfManufacture2", Data.yearOfManufacture[1].ToString());
                }

                List<string> stringList = new List<string>();
                for (int i = 0; i < Data.carType.Count; i++)
                {
                    stringList.Add($"@carType{i}");
                    dicValue.Add($"@carType{i}", Data.carType[i]);
                }
                sql += stringList.Count == 0 ? "" : $" and carType in ({String.Join(",", stringList.ToArray())}) ";


                stringList.Clear();
                for (int i = 0; i < Data.carModel.Count; i++)
                {
                    stringList.Add($"@carModel{i}");
                    dicValue.Add($"@carModel{i}", Data.carModel[i]);
                }
                sql += stringList.Count == 0 ? "" : $" and carModel in ({String.Join(",", stringList.ToArray())}) ";


                stringList.Clear();
                for (int i = 0; i < Data.area.Count; i++)
                {
                    stringList.Add($"@area{i}");
                    dicValue.Add($"@area{i}", Data.area[i]);
                }
                sql += stringList.Count == 0 ? "" : $" and area in ({String.Join(",", stringList.ToArray())}) ";


                stringList.Clear();
                for (int i = 0; i < Data.driveMode.Count; i++)
                {
                    stringList.Add($"@driveMode{i}");
                    dicValue.Add($"@driveMode{i}", Data.driveMode[i]);
                }
                sql += stringList.Count == 0 ? "" : $" and driveMode in ({String.Join(",", stringList.ToArray())}) ";


                stringList.Clear();
                for (int i = 0; i < Data.dealer.Count; i++)
                {
                    stringList.Add($"@dealer{i}");
                    dicValue.Add($"@dealer{i}", Data.dealer[i]);
                }
                sql += stringList.Count == 0 ? "" : $" and (CONCAT(dealer, '-', stronghold) in ({String.Join(",", stringList.ToArray())})) ";


                stringList.Clear();
                for (int i = 0; i < Data.equipment.Count; i++)
                {
                    stringList.Add($" outEquip like N'%' + @equipment{i} + '%' ");
                    dicValue.Add($"@equipment{i}", Data.equipment[i]);
                }
                sql += stringList.Count == 0 ? "" : $" and ({String.Join(" or ", stringList.ToArray())}) ";


                stringList.Clear();
                for (int i = 0; i < Data.color.Count; i++)
                {
                    stringList.Add($" outerColor like N'%' + @color{i} + '%' ");
                    dicValue.Add($"@color{i}", Data.color[i]);
                }
                sql += stringList.Count == 0 ? "" : $" and ({String.Join(" or ", stringList.ToArray())}) ";


                stringList.Clear();
                for (int i = 0; i < Data.feature.Count; i++)
                {
                    stringList.Add($" feature like N'%' + @feature{i} + '%' ");
                    dicValue.Add($"@feature{i}", Data.feature[i]);
                }
                sql += stringList.Count == 0 ? "" : $" and ({String.Join(" or ", stringList.ToArray())}) ";

                switch (Data.orderBy)
                {
                    case "價格由低到高":
                        sql += " order by price  ";
                        break;
                    case "價格由高到低":
                        sql += " order by price desc ";
                        break;
                    case "上架時間由新到舊":
                        sql += " order by createTime desc ";
                        break;
                    case "上架時間由舊到新":
                        sql += " order by createTime  ";
                        break;
                    case "里程由少到多":
                        sql += " order by milage  ";
                        break;
                    case "里程由多到少":
                        sql += " order by milage desc ";
                        break;
                    case "出廠年份由低到高":
                        sql += " order by yearOfManufacture  ";
                        break;
                    case "出廠年份由高到低":
                        sql += " order by yearOfManufacture desc ";
                        break;
                    default:
                        break;
                }

                //sql +=  fetch_subStr;

                
                DataTable dt = APCommonFun.GetDataTable_MSSQL(sql, dicValue);
                if (dt.Rows.Count > 0)
                {
                    var fileService = new Services.FileService();
                    if (string.IsNullOrEmpty(page))
                    {
                        foreach (DataRow dr in dt.Rows)
                        {
                            string seqShop = APCommonFun.CDBNulltrim(dr["seq"].ToString());
                            string price2 = APCommonFun.CDBNulltrim(dr["price"].ToString());
                            string licensePlate = APCommonFun.CDBNulltrim(dr["licensePlate"].ToString());
                            string licensePicture = APCommonFun.CDBNulltrim(dr["licensePicture"].ToString());
                            string milage2 = APCommonFun.CDBNulltrim(dr["milage"].ToString());
                            string yearOfManufacture2 = APCommonFun.CDBNulltrim(dr["yearOfManufacture"].ToString());
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
                            string area = APCommonFun.CDBNulltrim(dr["area"].ToString());
                            string salesRep = APCommonFun.CDBNulltrim(dr["salesRep"].ToString());
                            string carCondition1 = APCommonFun.CDBNulltrim(dr["carCondition1"].ToString());
                            string contact = APCommonFun.CDBNulltrim(dr["contact"].ToString());
                            string ListingDate = APCommonFun.CDBNulltrim(dr["ListingDate"].ToString());
                            string description = APCommonFun.CDBNulltrim(dr["description"].ToString());
                            string inspectionTable = APCommonFun.CDBNulltrim(dr["inspectionTable"].ToString());
                            string status = APCommonFun.CDBNulltrim(dr["status"].ToString());
                            string ShopNo = APCommonFun.CDBNulltrim(dr["ShopNo"].ToString());
                            DateTime updateTime = DateTime.Parse(APCommonFun.CDBNulltrim(dr["updateTime"].ToString()));
                            DateTime today = DateTime.Now;
                            TimeSpan ts = DateTime.Now.Subtract(updateTime);
                            double NumberOfDays = ts.TotalDays;
                            var prepaid = carShopEntities.Prepaid.Where(x => x.shopNo == ShopNo).FirstOrDefault();

                            if (status == "上架中" || (status == "成交下架" && NumberOfDays <= 3))
                            {
                                JObject tmpJoLay01 = new JObject();
                                tmpJoLay01.Add(new JProperty("seqShop", seqShop));
                                tmpJoLay01.Add(new JProperty("brand", brand));
                                tmpJoLay01.Add(new JProperty("price", price2));
                                tmpJoLay01.Add(new JProperty("licensePlate", licensePlate));
                                tmpJoLay01.Add(new JProperty("milage", milage2));
                                tmpJoLay01.Add(new JProperty("yearOfManufacture", yearOfManufacture2));
                                tmpJoLay01.Add(new JProperty("carModel", carModel));
                                tmpJoLay01.Add(new JProperty("carType", carType));
                                tmpJoLay01.Add(new JProperty("driveMode", driveMode));
                                tmpJoLay01.Add(new JProperty("transmissionType", transmissionType));
                                tmpJoLay01.Add(new JProperty("fuelType", fuelType));
                                tmpJoLay01.Add(new JProperty("displacement", displacement));
                                tmpJoLay01.Add(new JProperty("horsepower", horsepower));
                                tmpJoLay01.Add(new JProperty("outerColor", outerColor));
                                tmpJoLay01.Add(new JProperty("innerColor", innerColor));
                                tmpJoLay01.Add(new JProperty("feature", feature));
                                tmpJoLay01.Add(new JProperty("dealer", dealer));
                                tmpJoLay01.Add(new JProperty("stronghold", stronghold));
                                tmpJoLay01.Add(new JProperty("area", area));
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

                                tmpJoLay01.Add(new JProperty("carCondition1", fileService.GetRealUrl(carCondition1)));
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
                            string price2 = APCommonFun.CDBNulltrim(dr["price"].ToString());
                            string licensePlate = APCommonFun.CDBNulltrim(dr["licensePlate"].ToString());
                            string licensePicture = APCommonFun.CDBNulltrim(dr["licensePicture"].ToString());
                            string milage2 = APCommonFun.CDBNulltrim(dr["milage"].ToString());
                            string yearOfManufacture2 = APCommonFun.CDBNulltrim(dr["yearOfManufacture"].ToString());
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
                            string area = APCommonFun.CDBNulltrim(dr["area"].ToString());
                            string salesRep = APCommonFun.CDBNulltrim(dr["salesRep"].ToString());
                            string carCondition1 = APCommonFun.CDBNulltrim(dr["carCondition1"].ToString());
                            string contact = APCommonFun.CDBNulltrim(dr["contact"].ToString());
                            string ListingDate = APCommonFun.CDBNulltrim(dr["ListingDate"].ToString());
                            string description = APCommonFun.CDBNulltrim(dr["description"].ToString());
                            string inspectionTable = APCommonFun.CDBNulltrim(dr["inspectionTable"].ToString());
                            string status = APCommonFun.CDBNulltrim(dr["status"].ToString());
                            string ShopNo = APCommonFun.CDBNulltrim(dr["ShopNo"].ToString());
                            DateTime updateTime = DateTime.Parse(APCommonFun.CDBNulltrim(dr["updateTime"].ToString()));
                            DateTime today = DateTime.Now;
                            TimeSpan ts = DateTime.Now.Subtract(updateTime);
                            double NumberOfDays = ts.TotalDays;
                            var prepaid = carShopEntities.Prepaid.Where(x => x.shopNo == ShopNo).FirstOrDefault();

                            if (status == "上架中" || (status == "成交下架" && NumberOfDays <= 3))
                            {
                                JObject tmpJoLay01 = new JObject();
                                tmpJoLay01.Add(new JProperty("seqShop", seqShop));
                                tmpJoLay01.Add(new JProperty("brand", brand));
                                tmpJoLay01.Add(new JProperty("price", price2));
                                tmpJoLay01.Add(new JProperty("licensePlate", licensePlate));
                                tmpJoLay01.Add(new JProperty("milage", milage2));
                                tmpJoLay01.Add(new JProperty("yearOfManufacture", yearOfManufacture2));
                                tmpJoLay01.Add(new JProperty("carModel", carModel));
                                tmpJoLay01.Add(new JProperty("carType", carType));
                                tmpJoLay01.Add(new JProperty("driveMode", driveMode));
                                tmpJoLay01.Add(new JProperty("transmissionType", transmissionType));
                                tmpJoLay01.Add(new JProperty("fuelType", fuelType));
                                tmpJoLay01.Add(new JProperty("displacement", displacement));
                                tmpJoLay01.Add(new JProperty("horsepower", horsepower));
                                tmpJoLay01.Add(new JProperty("outerColor", outerColor));
                                tmpJoLay01.Add(new JProperty("innerColor", innerColor));
                                tmpJoLay01.Add(new JProperty("feature", feature));
                                tmpJoLay01.Add(new JProperty("dealer", dealer));
                                tmpJoLay01.Add(new JProperty("stronghold", stronghold));
                                tmpJoLay01.Add(new JProperty("area", area));
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

                                
                                tmpJoLay01.Add(new JProperty("carCondition1", fileService.GetRealUrl(carCondition1)));
                                totalJa.Add(tmpJoLay01);
                                if ((i >= (page_start - 1) * page_size && i < (page_start - 1) * page_size + page_size))
                                {
                                    newJa.Add(tmpJoLay01);
                                }
                                i++;
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
                APCommonFun.Error("[SearchCarController]99：" + ex.ToString());
                return ReturnException();
            }
        }
    }
}
