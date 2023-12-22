using Newtonsoft.Json;
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
using System.Net.Mail;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using WebApplication.Extensions;
using WebApplication.ViewModels;

namespace WebApplication.Controllers.api
{
    [JwtAuthActionFilter]
    public class CarSubscriptionController : BaseAPIController
    {
        /// <summary>
        /// 車輛通知訂閱
        /// </summary>
        /// <param name="Data"></param>
        /// <returns></returns>
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public object Post(Info_CarSubscriptionModel Data)
        {
            string InputIsok = "Y";
            string ReturnErr = "";
            JArray newJa = new JArray();
            string driveMode = string.Empty;
            string carType = string.Empty;
            string carModel = string.Empty;
            string outerColor = string.Empty;
            string area = string.Empty;
            string dealer = string.Empty;
            string price = string.Empty;
            string milage = string.Empty;
            string yearOfManufacture = string.Empty;
            string brand = string.Empty;
            string email = string.Empty;
            string user_id = string.Empty;
            var dicValue = new Dictionary<string, string>();

            try
            {
                if (Data.brand != null)
                {
                    brand = APCommonFun.CDBNulltrim(Data.brand);
                }
                if (Data.user_id != null)
                {
                    user_id = APCommonFun.CDBNulltrim(Data.user_id);
                }
                if (Data.email != null)
                {
                    email = APCommonFun.CDBNulltrim(Data.email);
                }

                //第一步 : 先判斷有沒有必填未填寫，
                if (user_id == "" || brand == "" || email == "") //必填                       
                {

                    if (brand == "")
                    {
                        InputIsok = "N";
                        ReturnErr = "執行動作錯誤-brand 為必填欄位";
                    }
                    else if (email == "")
                    {
                        InputIsok = "N";
                        ReturnErr = "執行動作錯誤-email 為必填欄位";
                    }
                    else
                    {
                        InputIsok = "N";
                        ReturnErr = "執行動作錯誤-user_id 為必填欄位";
                    }
                }


                //第二步 : 如果有必填未給值，回傳告知必填欄位要有資料
                if (InputIsok == "N")
                {
                    APCommonFun.Error("[CarSubscriptionController]90-" + ReturnErr);
                    return ReturnError(ReturnErr);
                }

                string sql2 = "select * from Shops where brand=@brand  ";
                dicValue.Add("@brand", brand);

                if (Data.price.Count == 2)
                {
                    sql2 += " and (price >= @price1 and price <=  @price2) ";
                    dicValue.Add("@price1", Data.price[0].ToString());
                    dicValue.Add("@price2", Data.price[1].ToString());
                }

                if (Data.milage.Count == 2)
                {
                    sql2 += " and (milage >= @milage1 and milage <= @milage2 ) ";
                    dicValue.Add("@milage1", Data.milage[0].ToString());
                    dicValue.Add("@milage2", Data.milage[1].ToString());
                }

                if (Data.yearOfManufacture.Count == 2)
                {
                    sql2 += " and (yearOfManufacture >= @yearOfManufacture1 and yearOfManufacture <= @yearOfManufacture2 ) ";
                    dicValue.Add("@yearOfManufacture1", Data.yearOfManufacture[0].ToString());
                    dicValue.Add("@yearOfManufacture2", Data.yearOfManufacture[1].ToString());
                }

                List<string> stringList = new List<string>();
                for (int i = 0; i < Data.carType.Count; i++)
                {
                    stringList.Add($"@carType{i}");
                    dicValue.Add($"@carType{i}", Data.carType[i]);
                }
                sql2 += stringList.Count == 0 ? "" : $" and carType in ({String.Join(",", stringList.ToArray())}) ";


                stringList.Clear();
                for (int i = 0; i < Data.carModel.Count; i++)
                {
                    stringList.Add($"@carModel{i}");
                    dicValue.Add($"@carModel{i}", Data.carModel[i]);
                }
                sql2 += stringList.Count == 0 ? "" : $" and carModel in ({String.Join(",", stringList.ToArray())}) ";


                stringList.Clear();
                for (int i = 0; i < Data.area.Count; i++)
                {
                    stringList.Add($"@area{i}");
                    dicValue.Add($"@area{i}", Data.area[i]);
                }
                sql2 += stringList.Count == 0 ? "" : $" and area in ({String.Join(",", stringList.ToArray())}) ";


                stringList.Clear();
                for (int i = 0; i < Data.driveMode.Count; i++)
                {
                    stringList.Add($"@driveMode{i}");
                    dicValue.Add($"@driveMode{i}", Data.driveMode[i]);
                }
                sql2 += stringList.Count == 0 ? "" : $" and driveMode in ({String.Join(",", stringList.ToArray())}) ";


                stringList.Clear();
                for (int i = 0; i < Data.dealer.Count; i++)
                {
                    stringList.Add($"@dealer{i}");
                    dicValue.Add($"@dealer{i}", Data.dealer[i]);
                }
                sql2 += stringList.Count == 0 ? "" : $" and dealer in ({String.Join(",", stringList.ToArray())}) ";

                stringList.Clear();
                for (int i = 0; i < Data.outerColor.Count; i++)
                {
                    stringList.Add($"@outerColor{i}");
                    dicValue.Add($"@outerColor{i}", Data.outerColor[i]);
                }
                sql2 += stringList.Count == 0 ? "" : $" and outerColor in ({String.Join(",", stringList.ToArray())}) ";
              

                DataTable dt = APCommonFun.GetDataTable_MSSQL(sql2, dicValue);

                string innerMailIp = ConfigurationManager.AppSettings["mailServerIp"]; ;
                string innerMailPort = ConfigurationManager.AppSettings["mailServerPort"]; ;

                using (MailMessage mail = new MailMessage())
                {
                    string urlBase = string.Empty;
                    if (brand == "NISSAN")
                    {
                        urlBase = ConfigurationManager.AppSettings["nissanPath"];
                        mail.From = new MailAddress("nissanCPO@yulon-nissan.com.tw");
                        mail.Body = "NISSAN 會員您好<br>";
                    }
                    else
                    {
                        urlBase = ConfigurationManager.AppSettings["infinitiPath"];
                        mail.From = new MailAddress("infinitiCPO@infiniti-taiwan.com");
                        mail.Body = "INFINITI 會員您好<br>";
                    }

                    mail.Body += "已收到您條件訂閱的資料填寫。<br><br>以下為您在條件訂閱的設定條件。<br>價格：" + (Data.price[0].ToString() == "0" ? "0" : Convert.ToInt32(Data.price[0].ToString()).ToString("###,###")) + " ~ " + Convert.ToInt32(Data.price[1].ToString()).ToString("###,###") + "元<br>里程數：" + (Data.milage[0].ToString() == "0" ? "0" : Convert.ToInt32(Data.milage[0].ToString()).ToString("###,###")) + " ~ " + Convert.ToInt32(Data.milage[1].ToString()).ToString("###,###") + "<br>車廠年份：" + Convert.ToInt32(Data.yearOfManufacture[0].ToString()).ToString("###,###") + " ~ " + Convert.ToInt32(Data.yearOfManufacture[1].ToString()).ToString("###,###") + "<br>驅動方式：" + driveMode + "<br>車種：" + carType + "<br>車型：" + carModel + "<br>顏色：" + outerColor + "<br>車輛所在地：" + area + "<br>服務據點：" + dealer + "<br><br>歡迎造訪" + (brand == "NISSAN" ? "NISSA" : "INFINITI") + "中古車網站。<br>網址：" + urlBase + "<br><br>";


                    mail.Body += "若您想取消訂閱，請點選連結<br>連結：" + urlBase + "backend/api/CancelCarSubscription/" + user_id + "<br><BR>此為系統自動發送的郵件，請勿回覆";

                    mail.To.Add(new MailAddress(email));
                    mail.IsBodyHtml = true;
                    mail.Subject = "訂閱車輛通知";

                    using (SmtpClient smtp = new SmtpClient(innerMailIp))
                    {
                        smtp.Port = Convert.ToInt32(innerMailPort);
                        smtp.Send(mail);
                    }
                }


                string sql = @"insert into Subscriptions (user_id, createTime, driveMode, carType, carModel, outerColor, area, 
                               dealer, priceStr, milageStr, yearOfManufactureStr, brand, email) 
                               values (@user_id , getdate(), @driveMode , @carType , @carModel , @outerColor , @area , 
                               @dealer, @priceStr, @milageStr , @yearOfManufactureStr , @brand, @email  ) ";

                APCommonFun.ExecSafeSqlCommand_MSSQL(
                    sql,
                    new List<SqlParameter>
                    {
                        new SqlParameter("@user_id", user_id),
                        new SqlParameter("@driveMode", driveMode ),
                        new SqlParameter("@carType", carType ),
                        new SqlParameter("@carModel", carModel ),
                        new SqlParameter("@outerColor", outerColor),
                        new SqlParameter("@area", area ),
                        new SqlParameter("@dealer", dealer ),
                        new SqlParameter("@priceStr", Data.price[0].ToString() + "," + Data.price[1].ToString() ),
                        new SqlParameter("@milageStr", Data.milage[0].ToString() + "," + Data.milage[1].ToString() ),
                        new SqlParameter("@yearOfManufactureStr", Data.yearOfManufacture[0].ToString() + "," + Data.yearOfManufacture[1].ToString() ),
                        new SqlParameter("@brand", brand ),
                        new SqlParameter("@email",email )
                    }
                );
                if (user_id != "")
                {
                    new Services.OneidService().UpdateMemberLog(RequestToken, Convert.ToInt64(user_id), "條件訂閱", brand);
                }

                return ReturnOK();
            }
            catch (Exception ex)
            {
                APCommonFun.Error("[CarSubscriptionController]99：" + ex.ToString());
                return ReturnException();
            }
        }

    }
}
