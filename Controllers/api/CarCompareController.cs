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
using WebApplication.ViewModels;

namespace WebApplication.Controllers.api
{
    [JwtAuthActionFilter]
    public class CarCompareController : BaseAPIController
    {
        /// <summary>
        /// 取得車輛比較
        /// </summary>
        /// <param name="Data"></param>
        /// <returns></returns>
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public object Post(Info_CarCompareModel Data)
        {
            string InputIsok = "Y";
            string ReturnErr = "";
            JArray newJa = new JArray();
            var dicValue = new Dictionary<string, string>();

            try
            {

                string brand = string.Empty;
                if (Data.brand != null)
                {
                    brand = APCommonFun.CDBNulltrim(Data.brand);
                }

                string user_id = string.Empty;
                if (Data.user_id != null)
                {
                    user_id = APCommonFun.CDBNulltrim(Data.user_id);
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
                    APCommonFun.Error("[CarCompareController]90-" + ReturnErr);
                    return ReturnError(ReturnErr);
                }

                string sql = "select * from Shops where 1=1 and brand=@brand  ";
                List<string> seqCondition = new List<string>();
                if (Data.seq.Count() > 3)
                {
                    return ReturnError("車輛比較最多三台!!");
                }
               
                if (Data.seq.Count() == 1)
                {
                    sql += " and (seq=@seq1  ) ";
                }
                else if (Data.seq.Count() == 2)
                {
                    sql += " and (seq=@seq1 or seq=@seq2 ) ";
                }
                else if (Data.seq.Count() == 3)
                {
                    sql += " and (seq=@seq1 or seq=@seq2 or seq=@seq3 ) ";
                }
                else
                {
                    sql += " and ( seq='' ) ";
                }

                string sql2 = "insert into CarCompareHistory values (@user_id , @seq , getdate(), @brand  ) ";


                //DataTable dt = APCommonFun.GetDataTable_MSSQL(sql);
                DataTable dt = new DataTable();
                if (Data.seq.Count() == 1)
                {
                    dt = APCommonFun.GetSafeDataTable_MSSQL(
                        sql,
                        new List<SqlParameter>
                        {
                        new SqlParameter("@brand", brand),
                        new SqlParameter("@seq1", Data.seq[0])
                        }
                    );
                }
                else if (Data.seq.Count() == 2)
                {
                    dt = APCommonFun.GetSafeDataTable_MSSQL(
                            sql,
                            new List<SqlParameter>
                            {
                        new SqlParameter("@brand", brand),
                        new SqlParameter("@seq1", Data.seq[0]),
                        new SqlParameter("@seq2", Data.seq[1]),
                            }
                        );
                }
                else if (Data.seq.Count() == 3)
                {
                    dt = APCommonFun.GetSafeDataTable_MSSQL(
                                sql,
                                new List<SqlParameter>
                                {
                        new SqlParameter("@brand", brand),
                        new SqlParameter("@seq1", Data.seq[0]),
                        new SqlParameter("@seq2", Data.seq[1]),
                        new SqlParameter("@seq3", Data.seq[2]),
                                }
                            );

                }
                else
                {
                    dt = APCommonFun.GetSafeDataTable_MSSQL(
                            sql,
                            new List<SqlParameter>
                            {
                    new SqlParameter("@brand", brand)
                            }
                        );

                }

                if (dt.Rows.Count > 0)
                {
                    var request = HttpContext.Current.Request;                   
                    foreach (DataRow dr in dt.Rows)
                    {
                        string seqShop = APCommonFun.CDBNulltrim(dr["seq"].ToString());
                        string price2 = APCommonFun.CDBNulltrim(dr["price"].ToString());
                        string licensePlate = APCommonFun.CDBNulltrim(dr["licensePlate"].ToString());
                        string licensePicture = APCommonFun.CDBNulltrim(dr["licensePicture"].ToString());
                        string milage2 = APCommonFun.CDBNulltrim(dr["milage"].ToString());
                        string yearOfManufacture2 = APCommonFun.CDBNulltrim(dr["yearOfManufacture"].ToString());
                        string monthOfManufacture2 = APCommonFun.CDBNulltrim(dr["monthOfManufacture"].ToString());
                        string carModel = APCommonFun.CDBNulltrim(dr["carModel"].ToString());
                        string carType = APCommonFun.CDBNulltrim(dr["carType"].ToString());
                        string driveMode = APCommonFun.CDBNulltrim(dr["driveMode"].ToString());
                        string transmissionType = APCommonFun.CDBNulltrim(dr["transmissionType"].ToString());
                        string fuelType = APCommonFun.CDBNulltrim(dr["fuelType"].ToString());
                        string displacement = APCommonFun.CDBNulltrim(dr["displacement"].ToString());
                        string horsepower = APCommonFun.CDBNulltrim(dr["horsepower"].ToString());
                        string outerColor = APCommonFun.CDBNulltrim(dr["outerColor"].ToString());
                        string innerColor = APCommonFun.CDBNulltrim(dr["innerColor"].ToString());
                        string outEquip = APCommonFun.CDBNulltrim(dr["outEquip"].ToString());
                        string feature = APCommonFun.CDBNulltrim(dr["feature"].ToString());
                        string dealer = APCommonFun.CDBNulltrim(dr["dealer"].ToString());
                        string stronghold = APCommonFun.CDBNulltrim(dr["stronghold"].ToString());

                        string sql3 = "select * from Dealers where dealerName=@dealer  and businessOffice=@stronghold  ";
                        string dealerAddress = string.Empty;
                        string dealerTime = string.Empty;
                        //DataTable dt3 = APCommonFun.GetDataTable_MSSQL(sql3);
                        DataTable dt3 = APCommonFun.GetSafeDataTable_MSSQL(
                                sql3,
                                new List<SqlParameter>
                                {
                                    new SqlParameter("@dealer", dealer),
                                    new SqlParameter("@stronghold", stronghold)
                                }
                            );

                        if (dt3.Rows.Count > 0)
                        {
                            dealerAddress = APCommonFun.CDBNulltrim(dt3.Rows[0]["address"].ToString());
                            dealerTime = APCommonFun.CDBNulltrim(dt3.Rows[0]["businessStartHourDay"].ToString()) + " - " + APCommonFun.CDBNulltrim(dt3.Rows[0]["businessEndHourNight"].ToString());
                        }

                        string salesRep = APCommonFun.CDBNulltrim(dr["salesRep"].ToString());
                        string carCondition1 = APCommonFun.CDBNulltrim(dr["carCondition1"].ToString());
                        string carCondition2 = APCommonFun.CDBNulltrim(dr["carCondition2"].ToString());
                        string carCondition3 = APCommonFun.CDBNulltrim(dr["carCondition3"].ToString());
                        string carCondition4 = APCommonFun.CDBNulltrim(dr["carCondition4"].ToString());
                        string otherCondition1 = APCommonFun.CDBNulltrim(dr["otherCondition1"].ToString());
                        string otherCondition2 = APCommonFun.CDBNulltrim(dr["otherCondition2"].ToString());
                        string contact = APCommonFun.CDBNulltrim(dr["contact"].ToString());
                        string ListingDate = APCommonFun.CDBNulltrim(dr["ListingDate"].ToString());
                        string description = APCommonFun.CDBNulltrim(dr["description"].ToString());
                        string inspectionTable = APCommonFun.CDBNulltrim(dr["inspectionTable"].ToString());

                        JObject tmpJoLay01 = new JObject();
                        tmpJoLay01.Add(new JProperty("seqShop", seqShop));
                        tmpJoLay01.Add(new JProperty("brand", brand));
                        tmpJoLay01.Add(new JProperty("price", price2));
                        tmpJoLay01.Add(new JProperty("licensePlate", licensePlate));
                        tmpJoLay01.Add(new JProperty("milage", milage2));
                        tmpJoLay01.Add(new JProperty("yearOfManufacture", yearOfManufacture2));
                        tmpJoLay01.Add(new JProperty("monthOfManufacture", monthOfManufacture2));
                        tmpJoLay01.Add(new JProperty("carModel", carModel));
                        tmpJoLay01.Add(new JProperty("carType", carType));
                        tmpJoLay01.Add(new JProperty("driveMode", driveMode));
                        tmpJoLay01.Add(new JProperty("transmissionType", transmissionType));
                        tmpJoLay01.Add(new JProperty("fuelType", fuelType));
                        tmpJoLay01.Add(new JProperty("displacement", displacement));
                        tmpJoLay01.Add(new JProperty("horsepower", horsepower));
                        tmpJoLay01.Add(new JProperty("outerColor", outerColor));
                        tmpJoLay01.Add(new JProperty("outEquip", outEquip));
                        tmpJoLay01.Add(new JProperty("feature", feature));
                        tmpJoLay01.Add(new JProperty("dealer", dealer + " - " + stronghold));
                        tmpJoLay01.Add(new JProperty("dealerAddress", dealerAddress));
                        tmpJoLay01.Add(new JProperty("dealerTime", dealerTime));
                        tmpJoLay01.Add(new JProperty("carCondition1", new Services.FileService().GetRealUrl(carCondition1)));

                        newJa.Add(tmpJoLay01);
                    }
                }

                APCommonFun.ExecSafeSqlCommand_MSSQL(
                      sql2,
                      new List<SqlParameter>
                      {
                            new SqlParameter("@user_id", user_id),
                            new SqlParameter("@seq", String.Join(",", Data.seq)),
                            new SqlParameter("@brand", brand)
                      }
                  );
                if (user_id != "")
                {
                    new Services.OneidService().UpdateMemberLog(RequestToken, Convert.ToInt64(user_id), "比較車輛", brand);
                }

                var data = new
                {
                    Count = newJa.Count,
                    Data = newJa
                };

                return ReturnOK(data);
            }
            catch (Exception ex)
            {
                APCommonFun.Error("[CarCompareController]99：" + ex.ToString());
                return ReturnException();
            }
        }

    }
}
