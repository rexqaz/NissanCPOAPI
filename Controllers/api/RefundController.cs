using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Helpers;
using System.Web.Http;
using System.Web.Http.Cors;
using WebApplication.Extensions;
using WebApplication.Models;
using WebApplication.ViewModels;

namespace WebApplication.Controllers.api
{
    [JwtAuthActionFilter]
    public class RefundController : BaseAPIController
    {
        /// <summary>
        /// 更新預付保留狀態
        /// </summary>
        /// <param name="Data"></param>
        /// <returns></returns>
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [Route("api/Refund/{prepaidNo}")]
        public object Post(string prepaidNo, Info_UpdatePrepaidModel Data)
        {
            try
            {
                CarShopEntities _db = new CarShopEntities();
                string InputIsok = "Y";
                string ReturnErr = "";
                string reason = string.Empty;
                string brand = string.Empty;
                string user_id = string.Empty;

                if (Data.user_id != null)
                {
                    user_id = APCommonFun.CDBNulltrim(Data.user_id);
                }
                if (Data.reason != null)
                {
                    reason = APCommonFun.CDBNulltrim(Data.reason);
                }

                //第一步 : 先判斷有沒有必填未填寫，
                if (prepaidNo == "" || reason == "" || user_id == "") //必填                       
                {
                    if (prepaidNo == "")
                    {
                        InputIsok = "N";
                        ReturnErr = "執行動作錯誤-prepaidNo 為必填欄位";
                    }
                    else if (prepaidNo == "")
                    {
                        InputIsok = "N";
                        ReturnErr = "執行動作錯誤-user_id 為必填欄位";
                    }
                    else
                    {
                        InputIsok = "N";
                        ReturnErr = "執行動作錯誤-reason為必填欄位";
                    }
                }

                //第二步 : 如果有必填未給值，回傳告知必填欄位要有資料
                if (InputIsok == "N")
                {
                    APCommonFun.Error("[UpdatePrepaidController]90-" + ReturnErr);
                    return ReturnError(ReturnErr);
                }

                string sql = string.Empty;
                var p = _db.Prepaid.Where(x => x.prepaidNo == prepaidNo && x.user_id == user_id).FirstOrDefault();

                if (p == null)
                {                    
                    return ReturnError("找不到此筆訂單");
                }
                else if (p.paidStatus != "已付訂金" && p.paidStatus != "申請退訂")
                {                    
                    return ReturnError("未付訂金");
                }
                else if (p.paidStatus == "申請退訂")
                {                   
                    return ReturnError("已申請退訂");
                }

                p.paidStatus = "申請退訂";
                p.returnReason = reason;
                p.updateTime = DateTime.Now;

                _db.SaveChanges();

                var shops = _db.Shops.Where(x => x.ShopNo == p.shopNo).FirstOrDefault();

                //send mail
                string innerMailIp = ConfigurationManager.AppSettings["mailServerIp"]; ;
                string innerMailPort = ConfigurationManager.AppSettings["mailServerPort"];

                string urlBase = string.Empty;
                if (brand == "NISSAN")
                    urlBase = ConfigurationManager.AppSettings["nissanPath"];
                else
                    urlBase = ConfigurationManager.AppSettings["infinitiPath"];

                string mailBodyByManage = $"已收到所屬據點預付保留退訂申請，請盡速與會員聯繫。<br><br>車輛編號: {p.shopNo}<br>預付保留單號：{prepaidNo}<br><br>請點選單一後台網址登入查看<br>連結：<a href=\"{urlBase}backend/\">{urlBase}backend/</a>";
                string mailBody = $"已收到您取消車輛預付保留的需求通知，將會由中古車銷售顧問與您聯繫後續退款事宜。<br>預付保留單號：{prepaidNo}<br>訂單日期：{DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss")}<br>金額：3,000<br><br>誠摯的邀請您填寫本次取消預付保留的體驗問卷<br>網址：<a href=\"https://forms.gle/7KrohH6iT3M17G4H9\">https://forms.gle/7KrohH6iT3M17G4H9</a>";
                var dicValue = new Dictionary<string, string>();
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

                    var mailList = new Services.EmailService().GetUserMail("據點管理人員", brand, shops.dealer);
                    foreach (var eMail in mailList)
                    {
                        mail.To.Add(new MailAddress(eMail));
                        mail.IsBodyHtml = true;
                        mail.Subject = "收到預付保留退訂通知";

                        using (SmtpClient smtp = new SmtpClient(innerMailIp))
                        {
                            smtp.Port = Convert.ToInt32(innerMailPort);
                            smtp.Send(mail);
                        }
                    }
                }

                if (shops != null && !string.IsNullOrEmpty(shops.salesRep))
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
                        dicValue.Add("@salesRep", shops.salesRep);
                        DataTable dt3 = APCommonFun.GetDataTable_MSSQL(sql7, dicValue);
                        if (dt3.Rows.Count > 0)
                        {
                            mail.To.Add(new MailAddress(dt3.Rows[0]["Email"].ToString()));

                            mail.IsBodyHtml = true;
                            mail.Subject = "收到預付保留退訂通知";

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

                    mail.To.Add(new MailAddress(p.email));
                    mail.IsBodyHtml = true;
                    mail.Subject = "預付保留退訂通知";

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
                string postData = "account=" + smsAccount + "&password=" + smsPassword + "& from_addr_type=0&from_addr=" + smsMobile + "&to_addr_type=0&to_addr=" + p.mobile + "&msg_expire_time=0&msg_type=0&msg=";

                if (brand == "NISSAN")
                {
                    postData += HttpUtility.UrlEncode("[NISSAN中古車會員通知]親愛的" + p.name + "您好，已收到您申請車輛預付保留退訂，請查看Email確認詳細資訊，謝謝。", Encoding.GetEncoding("big5"));
                }
                else
                {
                    postData += HttpUtility.UrlEncode("[INFINITI中古車會員通知]親愛的" + p.name + "您好，已收到您申請車輛預付保留退訂，請查看Email確認詳細資訊，謝謝。", Encoding.GetEncoding("big5"));
                }

                string result = APCommonFun.HttpWebPost(smsApi, postData, "UTF-8");

                var oneid = new Services.OneidService();
                var member = oneid.GetMemberByMobile(RequestToken, p.mobile);
                if (member.StatusCode == 200)
                {
                    oneid.UpdateMemberLog(RequestToken, member.Data.seq, "預付保留退訂", brand);
                }

                return ReturnOK();
            }
            catch (Exception ex)
            {
                APCommonFun.Error("[UpdatePrepaidController]99：" + ex.ToString());
                return ReturnException();
            }
        }

    }
}
