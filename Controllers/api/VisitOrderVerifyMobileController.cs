using System;
using System.Collections.Generic;
using System.Data;
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
    public class VisitOrderVerifyMobileController : BaseAPIController
    {
        /// <summary>
        /// 預約賞車手機號碼驗證
        /// </summary>
        /// <param name="Data"></param>
        /// <returns></returns>
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public object Post(Info_VisitOrderVerifyMobileModel Data)
        {
            try
            {
                string InputIsok = "Y";
                string ReturnErr = "";

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
                    APCommonFun.Error("[VisitOrderVerifyMobileController]90-token必填");
                    return ReturnError("token必填");
                }

                string code = string.Empty;
                if (Data.code != null)
                {
                    code = APCommonFun.CDBNulltrim(Data.code);
                }

                string order_id = string.Empty;
                if (Data.order_id != null)
                {
                    order_id = APCommonFun.CDBNulltrim(Data.order_id);
                }

                //第一步 : 先判斷有沒有必填未填寫，
                if (code == "" || order_id == "") //必填                       
                {
                    if (code == "")
                    {
                        InputIsok = "N";
                        ReturnErr = "執行動作錯誤-code 為必填欄位";
                    }
                    else
                    {
                        InputIsok = "N";
                        ReturnErr = "執行動作錯誤-order_id 為必填欄位";
                    }
                }

                //第二步 : 如果有必填未給值，回傳告知必填欄位要有資料
                if (InputIsok == "N")
                {
                    APCommonFun.Error("[VisitOrderVerifyMobileController]90-" + ReturnErr);
                    return ReturnError(ReturnErr);
                }
                var dicValue = new Dictionary<string, string>();

                string verifyCode = string.Empty;
                string verifyCodeTime = string.Empty;
               
                string sql = "select * from  VisitOrders where seq=@seq ";
                dicValue.Add("@seq", order_id);

                DataTable dt = APCommonFun.GetDataTable_MSSQL(sql, dicValue);
                if (dt.Rows.Count > 0)
                {
                    verifyCode = APCommonFun.CDBNulltrim(dt.Rows[0]["verifyCode"].ToString());
                    verifyCodeTime = APCommonFun.CDBNulltrim(dt.Rows[0]["verifyCodeTime"].ToString());
                }
                else
                {                    
                    return ReturnError("請確認預約賞車代碼輸入正確!!");
                }

                DateTime validTime = DateTime.Parse(verifyCodeTime);
                if (DateTime.Now > validTime.AddMinutes(1))
                {                    
                    return ReturnError("驗證碼因超過一分鐘失效!!");
                }

                if (code != verifyCode)
                {                    
                    return ReturnError("驗證碼輸入錯誤!!");
                }

                sql = "update VisitOrders set isConfirm=1 where seq=@seq   ";
                dicValue.Clear();
                dicValue.Add("@seq", order_id);
                APCommonFun.ExecSqlCommand_MSSQL(sql, dicValue);
                return ReturnOK();
            }
            catch (Exception ex)
            {
                APCommonFun.Error("[VisitOrderVerifyMobileController]99：" + ex.ToString());
                return ReturnException();
            }
        }


    }
}
