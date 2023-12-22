using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using WebApplication.Extensions;
using WebApplication.ViewModels;

namespace WebApplication.Controllers.api
{
    [JwtAuthActionFilter]
    public class GetOtpAndSendSmsController : BaseAPIController
    {

        /// <summary>
        /// 取得OTP並發送簡訊
        /// </summary>
        /// <param name="Data"></param>
        /// <returns></returns>
        // POST: api/GetOtpAndSendSms
        public object Post(Info_GetOtpAndSendSmsModel Data)
        {
            try
            {
                string InputIsok = "Y";
                string ReturnErr = "";
                string mobile = string.Empty;
                string brand = string.Empty;
                string type = "register";               

                string token = string.Empty;
                if (Data.token != null)
                {
                    token = APCommonFun.CDBNulltrim(Data.token);
                    if (String.IsNullOrEmpty(token) || token != "Qai?J57U3cVDaOpUooiR/BNs0VMQ=upZouRecG-VYc0ORi6/yD-KhpkMl1wFZpa9QOrpjb6YfXC0Nj?a1ysty5jF=AzCn13Hvi-1mKgg2tS1C!/aMtatPvx2bkbpGfIw=pR1De74lpd5vnrw7SNqEZqMXwwv14vMsOfI9SogzLr6T3x5thQ-ZKlX2vYlEvgFsZC6!CT8szQ2pE6=HrKDdtDwOIsiiB=MKdH/R/mH4DoZlnH!pfaU1cIavXBBIbFW")
                    {
                        return ReturnError("authorization failed!!");                        
                    }
                }
                else
                {
                    APCommonFun.Error("[GetOtpAndSendSmsController]90-token必填");                    
                    return ReturnError("token必填");
                }

                if (Data.type != null)
                {
                    type = APCommonFun.CDBNulltrim(Data.type);
                }

                if (Data.mobile != null)
                {
                    mobile = APCommonFun.CDBNulltrim(Data.mobile);
                }

                if (Data.brand != null)
                {
                    brand = APCommonFun.CDBNulltrim(Data.brand);
                }

                //第一步 : 先判斷有沒有必填未填寫，
                if (mobile == "") //必填                       
                {
                    InputIsok = "N";
                    ReturnErr = "執行動作錯誤-mobile 為必填欄位";
                }
                if (brand == "") //必填                       
                {
                    InputIsok = "N";
                    ReturnErr = "執行動作錯誤-brand 為必填欄位";
                }

                //第二步 : 如果有必填未給值，回傳告知必填欄位要有資料
                if (InputIsok == "N")
                {
                    APCommonFun.Error("[GetOtpAndSendSmsController]90-" + ReturnErr);
                    return ReturnError(ReturnErr);
                }

                string otp = APCommonFun.RandomOTP(5);
                
                
                
                var sql = "";
                var dicValue = new Dictionary<string, string>();

                if (type == "register")
                {
                    sql = "select * FROM  Members where mobile=@mobile ";
                    dicValue.Clear();
                    dicValue.Add("@mobile", mobile);
                    DataTable dt2 = APCommonFun.GetDataTable_MSSQL(sql, dicValue, true);
                    if (dt2.Rows.Count > 0)
                    {
                        return ReturnError("此手機號碼已存在!");
                    }
                }

                sql = "select * from OTPs where mobile=@mobile ";
                dicValue.Clear();
                dicValue.Add("@mobile", mobile);
                DataTable dt = APCommonFun.GetDataTable_MSSQL(sql, dicValue);
                if (dt.Rows.Count > 0)
                {
                    sql = "update OTPs set OTP=@OTP, OTPSendTime=getdate() where mobile =@mobile ";
                    dicValue.Clear();
                    dicValue.Add("@mobile", mobile);
                    dicValue.Add("@OTP", otp);
                    APCommonFun.ExecSqlCommand_MSSQL(sql, dicValue);
                }
                else
                {
                    sql = "insert into OTPs (OTP, OTPSendTime, mobile) values (@OTP, @createDate, @mobile) ";
                    dicValue.Clear();
                    dicValue.Add("@mobile", mobile);
                    dicValue.Add("@createDate", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    dicValue.Add("@OTP", otp);
                    APCommonFun.ExecSqlCommand_MSSQL(sql, dicValue);
                }

                //string message = "[INFINITI中古車會員通知]\n您的驗證碼為" + otp + "，驗證碼10分鐘內有效。";

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
                string postData = "account=" + smsAccount + "&password=" + smsPassword + "& from_addr_type=0&from_addr=" + smsMobile + "&to_addr_type=0&to_addr=" + mobile + "&msg_expire_time=0&msg_type=0&msg=%5b";
                if (brand == "NISSAN")
                {
                    postData += "NISSAN%a4%a4%a5j%a8%ae%b7%7c%ad%fb%b3q%aa%be%5d%0D%0A%0d%0a%0d%0a%b1z%aa%ba%c5%e7%c3%d2%bdX%ac%b0" + otp + "%a1A%c5%e7%c3%d2%bdX10%a4%c0%c4%c1%a4%ba%a6%b3%ae%c4%a1C";
                }
                else
                {
                    postData += "INFINITI%a4%a4%a5j%a8%ae%b7%7c%ad%fb%b3q%aa%be%5d%0D%0A%0d%0a%0d%0a%b1z%aa%ba%c5%e7%c3%d2%bdX%ac%b0" + otp + "%a1A%c5%e7%c3%d2%bdX10%a4%c0%c4%c1%a4%ba%a6%b3%ae%c4%a1C";
                }

                string result = APCommonFun.HttpWebPost(smsApi, postData, "UTF-8");

                HtmlDocument document = new HtmlDocument();
                document.LoadHtml(result);
                string body = document.DocumentNode.SelectSingleNode("//body").InnerText;
                if (!string.IsNullOrEmpty(body))
                {
                    body = body.Replace("\n", string.Empty);
                    string[] bodyContent = body.Split('|');
                    if (bodyContent[1].ToString() != "0")
                    {
                        return ReturnError("Send SMS失敗: " + bodyContent[1].ToString());
                    }
                }

                return ReturnOK();
            }
            catch (Exception ex)
            {
                APCommonFun.Error("[GetOtpAndSendSmsController]99：" + ex.ToString());
                return ReturnException();
            }
        }

    }
}
