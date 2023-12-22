using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using WebApplication.Models;

namespace WebApplication.Controllers
{
    public class APCommonFun
    {
        ////public class HelloController : Controller  //取得環境資料夾
        //// {
        //public readonly IHostingEnvironment _hostEnvironment;

        //public APCommonFun(IHostingEnvironment hostEnvironment)
        //{
        //    _hostEnvironment = hostEnvironment;
        //}
        //// }


        /// <summary>
        /// 紀錄錯誤訊息
        /// </summary>
        /// <param name="ErrorLog"></param>
        /// <returns></returns>
        public static string Error(string ErrorLog)
        {
            /////*以純文字檔案紀錄Bug資訊*/

            //////今日日期
            DateTime Date = DateTime.Now;
            string TodyMillisecond = Date.ToString("yyyy-MM-dd HH:mm:ss");
            string Tody = Date.ToString("yyyy-MM-dd");
            string Folder = Date.ToString("yyyy-MM");

            string strPath = System.AppDomain.CurrentDomain.BaseDirectory;

            //如果此路徑沒有資料夾
            if (!Directory.Exists(strPath + "/XG_Log/" + Folder))
            {
                //新增資料夾
                Directory.CreateDirectory(strPath + "/XG_Log/" + Folder);
            }

            //把內容寫到目的檔案，若檔案存在則附加在原本內容之後(換行)
            File.AppendAllText(strPath + "/XG_Log/" + Folder + "/" + Tody + "_Error.txt", "\r\n" + TodyMillisecond + "： " + ErrorLog);

            return Tody;
        }

        public static string Log_Event(string EventLog)
        {
            /////*以純文字檔案紀錄Bug資訊*/

            //////今日日期
            DateTime Date = DateTime.Now;
            string TodyMillisecond = Date.ToString("yyyy-MM-dd HH:mm:ss");
            string Tody = Date.ToString("yyyy-MM-dd");
            string Folder = Date.ToString("yyyy-MM");

            string strPath = System.Environment.CurrentDirectory;

            //如果此路徑沒有資料夾
            if (!Directory.Exists(strPath + "/XG_Log/" + Folder))
            {
                //新增資料夾
                Directory.CreateDirectory(strPath + "/XG_Log/" + Folder);
            }

            //把內容寫到目的檔案，若檔案存在則附加在原本內容之後(換行)
            File.AppendAllText(strPath + "/XG_Log/" + Folder + "/" + Tody + "_Log_Event.txt", "\r\n" + TodyMillisecond + "： " + EventLog);

            return Tody;
        }

        public static string Log_CallAPI_Message(string CallAPI_MessageLog)
        {
            /////*以純文字檔案紀錄Bug資訊*/

            //////今日日期
            DateTime Date = DateTime.Now;
            string TodyMillisecond = Date.ToString("yyyy-MM-dd HH:mm:ss");
            string Tody = Date.ToString("yyyy-MM-dd");
            string Folder = Date.ToString("yyyy-MM");

            string strPath = System.Environment.CurrentDirectory;

            //如果此路徑沒有資料夾
            if (!Directory.Exists(strPath + "/XG_Log/" + Folder))
            {
                //新增資料夾
                Directory.CreateDirectory(strPath + "/XG_Log/" + Folder);
            }

            //把內容寫到目的檔案，若檔案存在則附加在原本內容之後(換行)
            File.AppendAllText(strPath + "/XG_Log/" + Folder + "/" + Tody + "_Log_CallAPI_Message.txt", "\r\n" + TodyMillisecond + "： " + CallAPI_MessageLog);

            return Tody;
        }


        public static string LogLogin(string LoginID)
        {
            /////*以純文字檔案紀錄Bug資訊*/

            //今日日期
            DateTime Date = DateTime.Now;
            string TodyMillisecond = Date.ToString("yyyy-MM-dd HH:mm:ss");
            string Tody = Date.ToString("yyyy-MM-dd");

            //////如果此路徑沒有資料夾
            ////if (!Directory.Exists(Properties.Settings.Default.AppLog))
            ////{
            ////    //新增資料夾
            ////    Directory.CreateDirectory(Properties.Settings.Default.AppLog);
            ////}

            //////把內容寫到目的檔案，若檔案存在則附加在原本內容之後(換行)
            ////File.AppendAllText(Properties.Settings.Default.AppLog + "Temp" + Tody + ".txt", "\r\n" + TodyMillisecond + "： " + LoginID);

            return Tody;
        }


        //把字串長度都補齊，讓Word排版一致
        public static string TurnStringLength(string OldStr, int lenStr, int Blank_F = 0, string IsMiddle = "N")
        //                                      轉換前字串     轉換後字串    字串前面要空白幾格     字串是否置中
        {

            string NewStr = OldStr;

            if (Blank_F != 0)
            {
                for (int i = 0; i < Blank_F; i++)
                {
                    NewStr = " " + NewStr;
                }
            }

            //先把前面空白補齊
            if (IsMiddle == "Y")  //字串要置中
            {
                byte[] byteStr2 = Encoding.GetEncoding("big5").GetBytes(NewStr); //把string轉為byte 

                int iStart = (lenStr - byteStr2.Length) / 2; ;

                for (int i = 0; i < iStart; i++)
                {
                    NewStr = " " + NewStr;
                }
            }

            byte[] byteStr = Encoding.GetEncoding("big5").GetBytes(NewStr); //把string轉為byte 
            if (byteStr.Length >= lenStr)
            {
            }
            else
            {
                //再把後面空白補齊
                for (int i = 0; i < lenStr - byteStr.Length - 1; i++)
                {
                    NewStr = NewStr + " ";
                }
            }
            NewStr = NewStr + " ";
            return NewStr;

            //string NewStr = " " + OldStr;

            //byte[] byteStr = Encoding.GetEncoding("big5").GetBytes(textBox1.Text); //把string轉為byte 
            //byteStr.Length
            //if (OldStr.Length >= lenStr)
            //{
            //}
            //else
            //{
            //    for (int i = 0; i < lenStr - OldStr.Length - 1; i++)
            //    {
            //        NewStr = NewStr + " ";
            //    }
            //}
        }

        /// <summary>
        /// 日 (dd)前面補0
        /// </summary>
        /// <param name="ConverStr"></param>
        /// <returns></returns>
        public static string DayAdd_0(string ConverStr)
        {
            if (ConverStr.Length <= 1)
            {
                ConverStr = "0" + ConverStr;   // 前面補0
                return ConverStr;
            }
            else
            {
                return ConverStr.Trim();
            }
        }

        public static string CheckData(string ConverStr)
        {
            if (ConverStr == "NoData")
            {
                ConverStr = "";
                return ConverStr;
            }
            else
            {
                return ConverStr.Trim();
            }
        }

        public static string CDBNulltrim(string ConverStr)
        {
            if (ConverStr == null)
            {
                ConverStr = "";
                return ConverStr;
            }
            else
            {
                return ConverStr.Trim();
            }
        }

        public static DataTable GetSafeDataTable_MSSQL(string ComStr, List<SqlParameter> parameters = null, bool getMember = false)
        {
            DataTable myDataTable = new DataTable();

            string conStr = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
            if (getMember)
            {
                conStr = System.Configuration.ConfigurationManager.ConnectionStrings["MemberConnection"].ToString();
            }
            using (SqlConnection conn = new SqlConnection(conStr))
            {
                conn.Open();
                using (SqlCommand command = new SqlCommand(ComStr, conn))
                {
                    command.CommandTimeout = 600;
                    if (parameters != null)
                    {
                        command.Parameters.AddRange(parameters.ToArray());
                    }

                    using (SqlDataAdapter da = new SqlDataAdapter(command))
                    {
                        da.Fill(myDataTable);
                    }
                }
            }

            return myDataTable;
        }



        

        /// <summary>
        /// 交易模式，拿來寫入車輛管理流水號用的
        /// </summary>
        public static string Transaction_ExecShopsSeq_MSSQL(long shops_seq, bool getMember = false)
        {
            string conStr = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
            if (getMember)
            {
                conStr = System.Configuration.ConfigurationManager.ConnectionStrings["MemberConnection"].ToString();
            }

            var today = DateTime.Now.ToString("yyyyMMdd");
            var shopNo = "";

            using (SqlConnection connection = new SqlConnection(conStr))
            {
                connection.Open();

                SqlTransaction transaction = connection.BeginTransaction();
                try
                {
                    var ComStr = "select top 1 shopNo from Shops where shopNo like 'M' + CONVERT(VARCHAR(10), GETDATE(), 112) + '%' order by shopNo desc";
                    SqlCommand command = new SqlCommand(ComStr, connection);
                    DataSet ds = new DataSet();
                    command.Transaction = transaction;
                    command.CommandText = ComStr;
                    command.CommandTimeout = 600;
                    SqlDataAdapter da = new SqlDataAdapter(command);
                    da.Fill(ds);

                    var dt = ds.Tables[0];
                    if (dt.Rows.Count > 0)
                    {
                        shopNo = CDBNulltrim(dt.Rows[0]["shopNo"].ToString());
                    }

                    if (string.IsNullOrEmpty(shopNo))
                    {
                        shopNo = "M" + today + "00001";
                    }
                    else
                    {
                        shopNo = "M" + today + (Convert.ToInt32(shopNo.Split(new string[] { "M" + today }, StringSplitOptions.None)[1]) + 1).ToString().PadLeft(5, '0');
                    }

                    using (SqlCommand command2 = connection.CreateCommand())
                    {
                        command2.Transaction = transaction;
                        command2.CommandText = $"update [Shops] set [ShopNo] = N'{shopNo}' where seq = {shops_seq}";
                        command2.ExecuteNonQuery();
                    }


                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw;
                }
            }

            return shopNo;
        }


        /// <summary>
        /// 交易模式，拿來寫入前端[我要賣車]用的
        /// </summary>
        public static void Transaction_AddCarSellController_MSSQL(DataModels.AddCarSellDataInfo info, bool getMember = false)
        {
            string conStr = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
            if (getMember)
            {
                conStr = System.Configuration.ConfigurationManager.ConnectionStrings["MemberConnection"].ToString();
            }

            var today = DateTime.Now.ToString("yyyyMMdd");

            using (SqlConnection connection = new SqlConnection(conStr))
            {
                connection.Open();

                SqlTransaction transaction = connection.BeginTransaction();
                try
                {
                    // 1. 產生新的流水號
                    using (SqlCommand command = connection.CreateCommand())
                    {
                        command.CommandTimeout = 600;
                        command.Transaction = transaction;
                        command.CommandText = $"select top 1 sellNo from Sells where sellNo like @Value1 order by sellNo desc";
                        command.Parameters.AddWithValue("@Value1", "S" + today + "%");

                        DataSet ds = new DataSet();
                        SqlDataAdapter da = new SqlDataAdapter(command);
                        da.Fill(ds);
                        var dt = ds.Tables[0];
                        if (dt.Rows.Count > 0)
                        {
                            info.SellNo = CDBNulltrim(dt.Rows[0]["sellNo"].ToString());
                        }

                        if (string.IsNullOrEmpty(info.SellNo))
                        {
                            info.SellNo = "S" + today + "00001";
                        }
                        else
                        {
                            info.SellNo = "S" + today + (Convert.ToInt32(info.SellNo.Split(new string[] { "S" + today }, StringSplitOptions.None)[1]) + 1).ToString().PadLeft(5, '0');
                        }
                    }

                    // 2. 寫入資料
                    using (SqlCommand command = connection.CreateCommand())
                    {
                        var sql = "INSERT INTO Sells (brand, mobile, licensePlate, licensePicture, milage, " +
                        "yearMonthOfManufacture, yearOfManufacture, carModel, owner, title, birthday, email, " +
                        "needConsult, dealer, stronghold,   carCondition1, carCondition2, carCondition3, carCondition4, " +
                        "createTime, updateTime, consultant, area, sellNo, status,  salesRep, carBrand, otherBrand) values " +
                        " (@brand, @mobile , @licensePlate, @licensePicture, @milage, @yearMonthOfManufacture, @yearOfManufacture , " +
                        "@carModel, @name , @title, @birthday, @email, @needConsult, @dealer, @stronghold, @carCondition1 , " +
                        "@carCondition2 , @carCondition3 , @carCondition4 , getdate(), getdate(), @consultant, @area, @sellNo , " +
                        "@status, @salesRep , @carBrand, @otherBrand  );SELECT  SCOPE_IDENTITY() ";

                        var parameterList = new List<SqlParameter>
                            {
                                new SqlParameter("@sellNo", info.SellNo),
                                new SqlParameter("@brand", info.brand),
                                new SqlParameter("@mobile", info.mobile ),
                                new SqlParameter("@licensePlate", info.licensePlate),
                                new SqlParameter("@licensePicture",info.licensePicture),
                                new SqlParameter("@milage", info.milage),
                                new SqlParameter("@yearMonthOfManufacture", info.yearOfManufacture + "/" + info.monthOfManufacture ),
                                new SqlParameter("@yearOfManufacture", info.yearOfManufacture),
                                new SqlParameter("@carModel", info.carModel),
                                new SqlParameter("@name", info.name ),
                                new SqlParameter("@title", info.title),
                                new SqlParameter("@birthday",info.birthday == null? "" : info.birthday.Value.ToString("yyyy/MM/dd")),
                                new SqlParameter("@email", info.email),
                                new SqlParameter("@needConsult", info.needConsult ? "1" : "0" ),
                                new SqlParameter("@dealer", info.dealer.Split('-')[1] ),
                                new SqlParameter("@stronghold",info.dealer.Split('-')[0] ),
                                new SqlParameter("@carCondition1", info.carCondition1),
                                new SqlParameter("@carCondition2", info.carCondition2 ),
                                new SqlParameter("@carCondition3", info.carCondition3),
                                new SqlParameter("@carCondition4",info.carCondition4),
                                new SqlParameter("@consultant", info.salesRep),
                                new SqlParameter("@area", info.area),                               
                                new SqlParameter("@status",(string.IsNullOrEmpty(info.salesRep) ? "未處理" : "已指派")),
                                new SqlParameter("@salesRep",  (string.IsNullOrEmpty(info.salesRep) ? "無" : info.salesRep)),
                                new SqlParameter("@carBrand", info.carBrand ),
                                new SqlParameter("@otherBrand", info.otherBrand)
                            };

                        command.CommandTimeout = 600;
                        command.Transaction = transaction;
                        command.CommandText = sql;
                        command.Parameters.AddRange(parameterList.ToArray());
                        info.Sells_seq = Convert.ToInt32(command.ExecuteScalar());
                    }

                    // 2. 寫入紀錄
                    using (SqlCommand command = connection.CreateCommand())
                    {
                        var sql = "INSERT INTO [dbo].[SellsHistory]([sellNo],[recordDate],[status],[consultant],[salesRep],[admin],[note]) " +
                            "VALUES (@sellNo, getdate(), @status, @consultant, @salesRep, 'Admin', '' )"; ;

                        var parameterList = new List<SqlParameter>
                            {
                                new SqlParameter("@sellNo", info.SellNo),
                                new SqlParameter("@status", (string.IsNullOrEmpty(info.salesRep) ? "未處理" : "已指派") ),
                                new SqlParameter("@consultant", (string.IsNullOrEmpty(info.salesRep) ? "無" : info.salesRep) ),
                                new SqlParameter("@salesRep", (string.IsNullOrEmpty(info.salesRep) ? "無" : info.salesRep) )
                            };

                        command.CommandTimeout = 600;
                        command.Transaction = transaction;
                        command.CommandText = sql;
                        command.Parameters.AddRange(parameterList.ToArray());
                        command.ExecuteNonQuery();
                    }


                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    Error("Transaction_AddCarSellController_MSSQL:" + ex.Message + "\n" + ex.StackTrace);
                    transaction.Rollback();
                    throw;
                }
            }
        }

        /// <summary>
        /// 交易模式，拿來寫入前端[預付保留]用的
        /// </summary>
        public static string Transaction_PrepaidController_MSSQL(Models.PrepaidDataInfo info, bool getMember = false)
        {
            string conStr = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
            if (getMember)
            {
                conStr = System.Configuration.ConfigurationManager.ConnectionStrings["MemberConnection"].ToString();
            }

            var today = DateTime.Now.ToString("yyyyMMdd");

            using (SqlConnection connection = new SqlConnection(conStr))
            {
                connection.Open();

                SqlTransaction transaction = connection.BeginTransaction();
                try
                {
                    string brand = info.Brand;
                    string brandPrefix = 'C' + brand.Substring(0, 1) + today;

                    // 1. 產生新的流水號
                    using (SqlCommand command = connection.CreateCommand())
                    {
                        command.CommandTimeout = 600;
                        command.Transaction = transaction;
                        command.CommandText = $"select top 1 prepaidNo from Prepaid where prepaidNo like @BrandPrefix order by prepaidNo desc";
                        command.Parameters.AddWithValue("@BrandPrefix", brandPrefix + "%");

                        DataSet ds = new DataSet();
                        SqlDataAdapter da = new SqlDataAdapter(command);
                        da.Fill(ds);
                        var dt = ds.Tables[0];

                        if (dt.Rows.Count > 0)
                        {
                            info.PrepaidNo = APCommonFun.CDBNulltrim(dt.Rows[0]["prepaidNo"].ToString());
                        }

                        if (string.IsNullOrEmpty(info.PrepaidNo))
                        {
                            info.PrepaidNo = brandPrefix + "000001";
                        }
                        else
                        {
                            info.PrepaidNo = brandPrefix + (Convert.ToInt32(info.PrepaidNo.Split(new string[] { brandPrefix }, StringSplitOptions.None)[1]) + 1).ToString().PadLeft(6, '0');
                        }
                    }

                    // 2. 寫入資料
                    using (SqlCommand command = connection.CreateCommand())
                    {
                        var sql = "insert into Prepaid (prepaidNo, shopNo, user_id, name, mobile, email, title, id, birthday, address, contactPeriod, others, createTime, updateTime, paidStatus, contactStatus, sales, brand, isClose) values (@PrepaidNo, @ShopNo, @UserId, @Name, @Mobile, @Email, @Title, @Id, @Birthday, @Address, @ContactPeriod, @Others, getdate(), getdate(), N'未付款', N'未聯絡', @Sales, @brand, 'N' )";

                        var parameterList = new List<SqlParameter>
                            {
                                new SqlParameter("@PrepaidNo", info.PrepaidNo),
                                new SqlParameter("@ShopNo", info.ShopNo),
                                new SqlParameter("@UserId", info.UserId),
                                new SqlParameter("@Name", info.Name),
                                new SqlParameter("@Mobile", info.Mobile),
                                new SqlParameter("@Email", info.Email),
                                new SqlParameter("@Title", info.Title),
                                new SqlParameter("@Id", info.Id),
                                new SqlParameter("@Birthday", info.Birthday),
                                new SqlParameter("@Address", info.Address),
                                new SqlParameter("@ContactPeriod", info.ContactPeriod),
                                new SqlParameter("@Others", info.Others),
                                new SqlParameter("@Sales", info.Sales),
                                new SqlParameter("@Brand", info.Brand)
                            };

                        command.CommandTimeout = 600;
                        command.Transaction = transaction;
                        command.CommandText = sql;
                        command.Parameters.AddRange(parameterList.ToArray());
                        command.ExecuteNonQuery();
                    }

                    transaction.Commit();
                    return info.PrepaidNo;
                }
                catch (Exception ex)
                {
                    Error("Transaction_AddCarSellController_MSSQL:" + ex.Message + "\n" + ex.StackTrace);
                    transaction.Rollback();
                    throw;
                }
            }
        }

        /// <summary>
        /// 參數化查詢
        /// </summary>
        public static DataTable GetDataTable_MSSQL(string ComStr, Dictionary<string, string> comValues, bool getMember = false)
        {
            DataTable myDataTable = new DataTable();

            string conStr = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
            if (getMember)
            {
                conStr = System.Configuration.ConfigurationManager.ConnectionStrings["MemberConnection"].ToString();
            }


            using (SqlConnection conn = new SqlConnection(conStr))
            {
                conn.Open();
                SqlCommand command = new SqlCommand(ComStr, conn);

                foreach (var item in comValues)
                {
                    command.Parameters.AddWithValue(item.Key, item.Value);
                }

                command.CommandText = ComStr;
                command.CommandTimeout = 600;

                DataSet ds = new DataSet();
                SqlDataAdapter da = new SqlDataAdapter(command);
                da.Fill(ds);

                myDataTable = ds.Tables[0];
                return myDataTable;
            }

        }


        /// <summary>
        /// 參數化執行
        /// </summary>
        public static void ExecSqlCommand_MSSQL(string ComStr, Dictionary<string, string> comValues, bool getMember = false)
        {
            string conStr = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
            if (getMember)
            {
                conStr = System.Configuration.ConfigurationManager.ConnectionStrings["MemberConnection"].ToString();
            }
            using (SqlConnection conn = new SqlConnection(conStr))
            {
                conn.Open();
                SqlCommand command = new SqlCommand(ComStr, conn);

                foreach (var item in comValues)
                {
                    command.Parameters.AddWithValue(item.Key, item.Value);
                }

                command.CommandTimeout = 600;
                command.ExecuteNonQuery();
            }
        }




        public static void ExecSafeSqlCommand_MSSQL(string ComStr, List<SqlParameter> parameters = null, bool getMember = false)
        {
            string conStr = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
            if (getMember)
            {
                conStr = System.Configuration.ConfigurationManager.ConnectionStrings["MemberConnection"].ToString();
            }

            using (SqlConnection conn = new SqlConnection(conStr))
            {
                conn.Open();
                SqlCommand command = new SqlCommand(ComStr, conn);
                command.CommandTimeout = 600;

                // Add parameters to the SqlCommand
                if (parameters != null)
                {
                    command.Parameters.AddRange(parameters.ToArray());
                }

                command.ExecuteNonQuery();
            }
        }


       

        public static int ExecSafeSqlCommandGetResult_MSSQL(string ComStr, List<SqlParameter> parameters = null, bool getMember = false)
        {
            int id = 0;
            string conStr = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
            if (getMember)
            {
                conStr = System.Configuration.ConfigurationManager.ConnectionStrings["MemberConnection"].ToString();
            }
            using (SqlConnection conn = new SqlConnection(conStr))
            {
                conn.Open();
                SqlCommand command = new SqlCommand(ComStr, conn);
                command.CommandTimeout = 600;

                // Add parameters to the SqlCommand
                if (parameters != null)
                {
                    command.Parameters.AddRange(parameters.ToArray());
                }
                id = Convert.ToInt32(command.ExecuteScalar());
            }

            return id;
        }

        /// <summary>
        /// 呼叫-慧誠API 使用 (2022-05-03 Ben)
        /// </summary>
        /// <param name="jArray"></param>
        /// <param name="imedtac_API_Base"></param>
        /// <param name="imedtac_API_Url"></param>
        /// <returns></returns>
        public static string Call_imedtac_Api(JArray jArray, string imedtac_API_Base, string imedtac_API_Url)
        {
            // (1) 呼叫API 
            var client = new HttpClient();

            // (2) 設定基底URL
            client.BaseAddress = new Uri(imedtac_API_Base);

            // (3) Initializes a new instance of the HttpRequestMessage class with an HTTP method and a request Uri
            HttpRequestMessage request = new System.Net.Http.HttpRequestMessage(HttpMethod.Post, imedtac_API_Url);

            // (4) 設定body內容和格式
            request.Content = new StringContent(jArray.ToString(), Encoding.UTF8, "application/json");

            var response = client.SendAsync(request).Result;

            string return_tmp = "false";
            // (5) 判斷是否連線成功
            if (response.IsSuccessStatusCode)
            {
                try
                {
                    return_tmp = "true";
                    APCommonFun.Log_Event("[呼叫慧誠API，寫入慧誠資料庫]97： Post慧誠 API 呼叫成功 -" + imedtac_API_Url); // (Log)_97
                }
                catch (Exception ex)
                {
                    APCommonFun.Error("[呼叫慧誠API，寫入慧誠資料庫]99： Post慧誠 API 呼叫失敗 -" + ex.ToString()); // (Log)_99
                }
            }

            return return_tmp;
        }




        /// <summary>
        /// 日期轉換：MM/dd/yyyy to yyyy-MM-dd
        /// </summary>
        /// <param name="DateChang"></param>
        /// <returns>usbs</returns>
        public static string DateChangToEn(string DateChang)
        {
            string usbs = "";
            if (DateChang.Length == 10)
            {
                usbs = DateChang.Substring(6, 4) + "-" + DateChang.Substring(0, 2) + "-" + DateChang.Substring(3, 2);
            }

            return usbs;
        }

        /// <summary>
        /// 時間轉換：HHmm to HH:mm
        /// </summary>
        /// <param name="TimeChangPara"></param>
        /// <returns>usbs</returns>
        public static string TimeChang(string TimeChangPara)
        {
            string usbs = "";
            if (TimeChangPara != "")
            {
                usbs = TimeChangPara.Substring(0, 2) + ":" + TimeChangPara.Substring(2, 2);
            }
            return usbs;
        }


        /// <summary>
        /// 確認欄位是否為nil
        /// </summary>
        /// <param name="Nil"></param>
        /// <returns></returns>
        public static string CheckNil(string Nil)
        {
            string usbs = "";
            if (Nil == "nil")
            {
                usbs = "";
            }
            else { usbs = Nil; }


            return usbs;
        }
        /// <summary>
        /// 時間字串轉 hh:mm 格式 5/4 Jamie
        /// </summary>
        /// <param name="timeStr">傳入值</param>
        /// <returns>newNumber</returns>
        public static string timeformat(string timeStr)
        {

            string timeformat = "";

            try
            {
                string tfront = timeStr.Substring(0, 2);
                string tback = timeStr.Substring(2, 2);
                timeformat = tfront + ":" + tback;
            }
            catch (Exception ex)
            {
                Error("timeformat Error-" + ex.ToString());
            }
            return timeformat;
        }

        private static Random random = new Random();

        public static string RandomString(int length)
        {
            const string chars1 = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string chars2 = "abcdefghijklmnopqrstuvwxyz";
            const string chars3 = "0123456789";
            return new string(Enumerable.Repeat(chars1, length / 3)
                .Select(s => s[random.Next(s.Length)]).ToArray()) + new string(Enumerable.Repeat(chars2, length / 3)
                .Select(s => s[random.Next(s.Length)]).ToArray()) + new string(Enumerable.Repeat(chars3, length / 3)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public static string RandomOTP(int length)
        {
            const string chars = "0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }


        public static string HttpWebPost(string url, string postdata, string Encod)
        {
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                url = url + postdata;
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
                request.UserAgent = "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 6.1; Trident/4.0; QQWubi 133; SLCC2; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729; Media Center PC 6.0; CIBA; InfoPath.2)";
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                request.KeepAlive = false;
                request.Timeout = 60000;
                request.Headers.Add("cache-control", "no-cache");

                System.IO.Stream outputStream = request.GetRequestStream();
                outputStream.Close();


                //接收返回的页面
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    System.IO.Stream responseStream = response.GetResponseStream();
                    System.IO.StreamReader reader = new System.IO.StreamReader(responseStream, Encoding.GetEncoding(Encod));
                    return reader.ReadToEnd();
                }
                else
                {
                    return "请求错误";
                }
            }
            catch (Exception ex)
            {
                APCommonFun.Error("[Message]" + ex.Message);
                return "内部错误";
            }
        }

        public static string HttpWebGet(string url)
        {
            try
            {
                string AccessToken = string.Empty;
                string result = string.Empty;
                Encoding encoding = Encoding.UTF8;
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "GET";
                request.Accept = "text/html, application/xhtml+xml, */*";
                request.ContentType = "application/json";

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                {
                    result = reader.ReadToEnd();
                }
                getKey keyResult = JsonConvert.DeserializeObject<getKey>(result);
                return keyResult.data;
            }
            catch (Exception ex)
            {
                APCommonFun.Error("[Message]" + ex.Message);
                return "内部错误";
            }
        }

        public class getKey
        {
            public string result { get; set; }
            public string errorcode { get; set; }
            public string message { get; set; }
            public string data { get; set; }
        }
        public static string SHA256Hash(string input)
        {
            SHA256 sha256 = new SHA256CryptoServiceProvider();//建立一個SHA256
            byte[] source = Encoding.Default.GetBytes(input);//將字串轉為Byte[]
            byte[] crypto = sha256.ComputeHash(source);//進行SHA256加密
            string result = Convert.ToBase64String(crypto);//把加密後的字串從Byte[]轉為字串

            return result;
        }

        public static string SHA256HashHex(string input)
        {
            SHA256 sha256 = new SHA256CryptoServiceProvider();//建立一個SHA256
            byte[] source = Encoding.Default.GetBytes(input);//將字串轉為Byte[]
            byte[] crypto = sha256.ComputeHash(source);//進行SHA256加密
            StringBuilder result = new StringBuilder();
            foreach (byte byteValue in crypto)
            {
                result.AppendFormat("{0:x2}", byteValue);   // 轉為十六進位
            }

            return result.ToString();
        }
    }
}
