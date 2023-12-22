using System;
using System.Collections.Generic;
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
    public class VerifyOtpController : BaseAPIController
    {
        /// <summary>
        /// OTP驗證
        /// </summary>
        /// <param name="Data"></param>
        /// <returns></returns>
        public object Post(Info_VerifyOtpModel Data)
        {
            try
            {
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
                    return ReturnError("token必填");
                }

                string InputIsok = "Y";
                string ReturnErr = "";

                string otp = string.Empty;
                if (Data.otp != null)
                {
                    otp = APCommonFun.CDBNulltrim(Data.otp);
                }

                string mobile = string.Empty;
                if (Data.mobile != null)
                {
                    mobile = APCommonFun.CDBNulltrim(Data.mobile);
                }

                string brand = string.Empty;
                if (Data.brand != null)
                {
                    brand = APCommonFun.CDBNulltrim(Data.brand);
                }

                //第一步 : 先判斷有沒有必填未填寫，
                if (mobile == "" || otp == "" || brand == "") //必填                       
                {
                    if (mobile == "")
                    {
                        InputIsok = "N";
                        ReturnErr = "執行動作錯誤-mobile 為必填欄位";
                    }
                    else if (brand == "")
                    {
                        InputIsok = "N";
                        ReturnErr = "執行動作錯誤-brand 為必填欄位";
                    }
                    else
                    {
                        InputIsok = "N";
                        ReturnErr = "執行動作錯誤-otp 為必填欄位";
                    }
                }

                //第二步 : 如果有必填未給值，回傳告知必填欄位要有資料
                if (InputIsok == "N")
                {
                    APCommonFun.Error("[VerifyOtpController]90-" + ReturnErr);
                    return ReturnError(ReturnErr);
                }
                var dicValue = new Dictionary<string, string>();
                dicValue.Add("@mobile", mobile);


               
                

                DateTime today = DateTime.Now;
                string result = string.Empty;


                string sql = "select * from [OTPs] where mobile=@mobile ";
                dicValue.Clear();
                dicValue.Add("@mobile", mobile);
                DataTable dt = APCommonFun.GetDataTable_MSSQL(sql, dicValue);
                if (dt.Rows.Count > 0)
                {
                    string otpDB = APCommonFun.CDBNulltrim(dt.Rows[0]["OTP"].ToString());
                    string mobileDB = APCommonFun.CDBNulltrim(dt.Rows[0]["mobile"].ToString());
                    DateTime OTPSendTime = DateTime.Parse(APCommonFun.CDBNulltrim(dt.Rows[0]["OTPSendTime"].ToString()));

                    if (otp == otpDB && OTPSendTime.AddMinutes(10) >= today)
                    {
                        sql = "select * FROM  Members where mobile=@mobile ";
                        dicValue.Clear();
                        dicValue.Add("@mobile", mobile);
                        DataTable dt2 = APCommonFun.GetDataTable_MSSQL(sql, dicValue, true);
                        if (dt2.Rows.Count > 0)
                        {                            
                            return ReturnError("手機已存在，請重新輸入手機");
                        }
                        else
                        {
                            sql = "INSERT INTO Members (name, id, mobile, email, title  , birthday, address, interestedCar, isMailVerify, isMobileVerify, password, createTime, status, brand, area, loginStatus) VALUES (N'', '', @mobile, '', N'', '', N'', N'', '0', '1', '' , getdate(), 0, @brand, N'', N'' ) ";
                            dicValue.Clear();
                            dicValue.Add("@mobile", mobile);
                            dicValue.Add("@brand", brand);
                            APCommonFun.ExecSqlCommand_MSSQL(sql, dicValue, true);
                        }
                    }
                    else if (otp != otpDB)
                    {
                        return ReturnError("驗證碼錯誤");
                    }
                    else
                    {
                        return ReturnError("驗證時間超過10分鐘有效期間");
                    }
                }

                return ReturnOK();
            }
            catch (Exception ex)
            {
                APCommonFun.Error("[VerifyOtpController]99：" + ex.ToString());
                return ReturnException();
            }
        }
    }
}
