using Microsoft.Owin.BuilderProperties;
using Microsoft.Owin.Security.Twitter.Messages;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mail;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Helpers;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Xml.Linq;
using WebApplication.Models;
using static Azure.Core.HttpHeader;

namespace WebApplication.Controllers.api
{
    public class PayCallbackController : ApiController
    {
        /// <summary>
        /// 支付
        /// </summary>
        /// <param name="Data"></param>
        /// <returns></returns>
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public HttpResponseMessage Get(string DATA, string MACD)
        {
            APCommonFun.Error($"[玉山金流回傳]：DATA={DATA} MACD={MACD}");
            // 解析 DATA 參數
            string nissanPath = ConfigurationManager.AppSettings["nissanPath"];
            var dataPairs = DATA.Split(',');
            Dictionary<string, string> dataDict = new Dictionary<string, string>();
            var dicValue = new Dictionary<string, string>();

            foreach (var pair in dataPairs)
            {
                var keyValue = pair.Split('=');
                if (keyValue.Length == 2)
                {
                    dataDict[keyValue[0]] = keyValue[1];
                }
            }

            string rc = dataDict.ContainsKey("RC") ? dataDict["RC"] : null;
            string mid = dataDict.ContainsKey("MID") ? dataDict["MID"] : null;
            string ono = dataDict.ContainsKey("ONO") ? dataDict["ONO"] : null;
            string ltd = dataDict.ContainsKey("LTD") ? dataDict["LTD"] : null;
            string ltt = dataDict.ContainsKey("LTT") ? dataDict["LTT"] : null;
            string rrn = dataDict.ContainsKey("RRN") ? dataDict["RRN"] : null;
            string air = dataDict.ContainsKey("AIR") ? dataDict["AIR"] : null;
            string an = dataDict.ContainsKey("AN") ? dataDict["AN"] : null;
            var response = new HttpResponseMessage(HttpStatusCode.Redirect);

            if (rrn == null && air == null)
            {
                response.Headers.Location = new Uri($"{nissanPath}?code=400");
                return response;
            }

            string queryPrepaid = "select top 1 * ";
            queryPrepaid += "from Prepaid as p ";
            queryPrepaid += "join Shops as s on p.shopNo = s.shopNo ";
            queryPrepaid += "where p.prepaidNo = @PrepaidNo and p.paidStatus = N'未付款' ";
            DataTable dt = APCommonFun.GetSafeDataTable_MSSQL(
                    queryPrepaid,
                    new List<SqlParameter>
                    {
                        new SqlParameter("@PrepaidNo", ono)
                    }
                );

            if (dt.Rows.Count == 0)
            {
                response.Headers.Location = new Uri($"{nissanPath}?code=404");
                return response;
            }
            
            string name = dt.Rows[0]["name"].ToString();
            string email = dt.Rows[0]["email"].ToString();
            string mobile = dt.Rows[0]["mobile"].ToString();
            string shopNo = dt.Rows[0]["shopNo"].ToString();
            string salesRep = dt.Rows[0]["salesRep"].ToString();
            string carType = dt.Rows[0]["carType"].ToString();
            string carModel = dt.Rows[0]["carModel"].ToString();
            string price = dt.Rows[0]["price"].ToString();
            string displacement = dt.Rows[0]["displacement"].ToString();
            string fuelType = dt.Rows[0]["fuelType"].ToString();
            string driveMode = dt.Rows[0]["driveMode"].ToString();
            string milage = dt.Rows[0]["milage"].ToString();
            string yearOfManufacture = dt.Rows[0]["yearOfManufacture"].ToString();
            string monthOfManufacture = dt.Rows[0]["monthOfManufacture"].ToString();
            string color = dt.Rows[0]["outerColor"].ToString();
            string outEquip = dt.Rows[0]["outEquip"].ToString();
            string feature = dt.Rows[0]["feature"].ToString();
            string area = dt.Rows[0]["area"].ToString();
            string dealer = dt.Rows[0]["dealer"].ToString();
            string stronghold = dt.Rows[0]["stronghold"].ToString();
            string brand = dt.Rows[0]["brand"].ToString();

            try
            {
                string sql = "update  Prepaid set paidStatus = N'已付訂金' where prepaidNo = @PrepaidNo";

                string sql5 = "select * from Shops where ShopNo = @ShopNo";
                APCommonFun.ExecSafeSqlCommand_MSSQL(
                sql,
                new List<SqlParameter>
                    {
                        new SqlParameter("@PrepaidNo", ono),
                    }
                );

                string innerMailIp = ConfigurationManager.AppSettings["mailServerIp"]; ;
                string innerMailPort = ConfigurationManager.AppSettings["mailServerPort"];

                string urlBase = string.Empty;
                if (brand == "NISSAN")
                    urlBase = ConfigurationManager.AppSettings["nissanPath"];
                else
                    urlBase = ConfigurationManager.AppSettings["infinitiPath"];

                string mailBodyByManage = $"已收到所屬據點預付保留新進件，請盡速與會員聯繫。<br><br>車輛編號: {shopNo}<br>預付保留單號：{ono}<br><br>請點選單一後台網址登入查看<br>連結：<a href=\"{urlBase}backend/\">{urlBase}backend/</a>";
                string mailBody = $"您已完成車輛預付保留，將由中古車銷售顧問與您聯繫後續購車事宜，謝謝。<br>預付保留單號：{ono}<br>訂單日期：{DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss")}<br>金額：3,000<br><br>歡迎造訪NISSAN中古車網站。<br>網址：<a href=\"{urlBase}\">{urlBase}</a>";

                using (MailMessage mail = new MailMessage())
                {
                    if (brand == "NISSAN")
                    {
                        mail.From = new MailAddress("nissanCPO@yulon-nissan.com.tw");
                    }
                    else
                    {
                        mail.From = new MailAddress("infinitiCPO@infiniti-taiwan.com");
                    }

                    mail.Body = $"中古車據點管理員您好<br>{mailBodyByManage}";

                    var mailList = new Services.EmailService().GetUserMail("據點管理人員", brand, dealer);
                    foreach (var eMail in mailList)
                    {
                        mail.To.Add(new MailAddress(eMail));
                        mail.IsBodyHtml = true;
                        mail.Subject = "收到預付保留通知";

                        using (SmtpClient smtp = new SmtpClient(innerMailIp))
                        {
                            smtp.Port = Convert.ToInt32(innerMailPort);
                            smtp.Send(mail);
                        }
                    }
                }

                if (!string.IsNullOrEmpty(salesRep))
                {
                    using (MailMessage mail = new MailMessage())
                    {
                        if (brand == "NISSAN")
                        {
                            mail.From = new MailAddress("nissanCPO@yulon-nissan.com.tw");
                        }
                        else
                        {
                            mail.From = new MailAddress("infinitiCPO@infiniti-taiwan.com");
                        }

                        mail.Body = $"中古車銷售顧問您好<br>{mailBodyByManage}";

                        string sql7 = "SELECT * FROM [CarShop].[dbo].[DealerPersons] where name = @salesRep  ";
                        dicValue.Clear();
                        dicValue.Add("@salesRep", salesRep);
                        DataTable dt3 = APCommonFun.GetDataTable_MSSQL(sql7, dicValue);
                        if (dt3.Rows.Count > 0)
                        {
                            mail.To.Add(new MailAddress(dt3.Rows[0]["Email"].ToString()));

                            mail.IsBodyHtml = true;
                            mail.Subject = "收到預付保留通知";

                            using (SmtpClient smtp = new SmtpClient(innerMailIp))
                            {
                                smtp.Port = Convert.ToInt32(innerMailPort);
                                smtp.Send(mail);
                            }
                        }

                    }
                }

                using (MailMessage mail = new MailMessage())
                {
                    if (brand == "NISSAN")
                    {
                        mail.From = new MailAddress("nissanCPO@yulon-nissan.com.tw");
                    }
                    else
                    {
                        mail.From = new MailAddress("infinitiCPO@infiniti-taiwan.com");
                    }

                    mail.Body = $"{brand}會員您好<br>{mailBody}";

                    mail.To.Add(new MailAddress(email));
                    mail.IsBodyHtml = true;
                    mail.Subject = "預付保留成功通知";

                    using (SmtpClient smtp = new SmtpClient(innerMailIp))
                    {
                        smtp.Port = Convert.ToInt32(innerMailPort);
                        smtp.Send(mail);
                    }
                }

                string smsPassword = ConfigurationManager.AppSettings["smsPassword"];
                string smsAccount = ConfigurationManager.AppSettings["smsAccount"];
                string smsMobile = string.Empty;
                if (brand == "NISSAN")
                {
                    smsMobile = ConfigurationManager.AppSettings["smsMobileNissan"];
                }
                else
                {
                    smsMobile = ConfigurationManager.AppSettings["smsMobileInfiniti"];
                }

                string smsApi = ConfigurationManager.AppSettings["smsApi"];
                string postData = "account=" + smsAccount + "&password=" + smsPassword + "& from_addr_type=0&from_addr=" + smsMobile + "&to_addr_type=0&to_addr=" + mobile + "&msg_expire_time=0&msg_type=0&msg=";

                if (brand == "NISSAN")
                {
                    postData += HttpUtility.UrlEncode("[NISSAN中古車會員通知]親愛的" + name + "您好，您已完成車輛預付保留，請查看Email確認詳細資訊，謝謝。", Encoding.GetEncoding("big5"));
                }
                else
                {
                    postData += HttpUtility.UrlEncode("[INFINITI中古車會員通知]親愛的" + name + "您好，您已完成車輛預付保留，請查看Email確認詳細資訊，謝謝。", Encoding.GetEncoding("big5"));
                }

                string result = APCommonFun.HttpWebPost(smsApi, postData, "UTF-8");

                //var oneid = new Services.OneidService();
                //var member = oneid.GetMemberByMobile(RequestToken, mobile);
                //if (member.StatusCode == 200)
                //{
                //    oneid.UpdateMemberLog(RequestToken, member.Data.seq, "預付保留完成", brand);
                //}
            }
            catch (Exception ex)
            {
                response.Headers.Location = new Uri($"{nissanPath}?code=500");
                return response;
            }

            response.Headers.Location = new Uri($"{nissanPath}keep/result/" + ono + "?code=200");
            return response;
        }
    }
}
