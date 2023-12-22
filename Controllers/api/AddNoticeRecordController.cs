using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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
    public class AddNoticeRecordController : BaseAPIController
    {
        /// <summary>
        /// 新增車輛關注
        /// </summary>
        /// <param name="Data"></param>
        /// <returns></returns>
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public object Post(Info_AddNoticeRecordModel Data)
        {
            string InputIsok = "Y";
            string ReturnErr = "";
            string shopNo = string.Empty;
            string brand = string.Empty;
            string user_id = string.Empty;

            try
            {
                if (Data.shopNo != null)
                {
                    shopNo = APCommonFun.CDBNulltrim(Data.shopNo);
                }

                if (Data.brand != null)
                {
                    brand = APCommonFun.CDBNulltrim(Data.brand);
                }

                if (Data.user_id != null)
                {
                    user_id = APCommonFun.CDBNulltrim(Data.user_id);
                }

                //第一步 : 先判斷有沒有必填未填寫，
                if (shopNo == "" || user_id == "" || brand == "") //必填                       
                {
                    if (shopNo == "")
                    {
                        InputIsok = "N";
                        ReturnErr = "執行動作錯誤-shopNo 為必填欄位";
                    }
                    else if (brand == "")
                    {
                        InputIsok = "N";
                        ReturnErr = "執行動作錯誤-brand 為必填欄位";
                    }
                }

                //第二步 : 如果有必填未給值，回傳告知必填欄位要有資料
                if (InputIsok == "N")
                {
                    APCommonFun.Error("[AddNoticeRecordController]90-" + ReturnErr);
                    return ReturnError(ReturnErr);
                }

                string sql = "insert into [NoticeRecords] (noticeTime, member, shopNo, brand) values (getdate(), @user_id , @shopNo , @brand  ) ";
                string sql2 = "select * from NoticeRecords where member=@user_id ";
                string sql3 = "select * from NoticeRecords where member=@user_id  and shopNo=@shopNo  and brand=@brand  ";


                DataTable dt = APCommonFun.GetSafeDataTable_MSSQL(
                    sql2,
                    new List<SqlParameter>
                    {
                        new SqlParameter("@user_id", user_id)
                    }
                );
                if (dt.Rows.Count >= 99)
                {
                    return ReturnError("關注車輛最多99輛!!，已達上限");
                }

                DataTable dt2 = APCommonFun.GetSafeDataTable_MSSQL(
                    sql3,
                    new List<SqlParameter>
                    {
                        new SqlParameter("@user_id", user_id),
                        new SqlParameter("@shopNo", shopNo),
                        new SqlParameter("@brand", brand)
                    }
                );
                if (dt2.Rows.Count > 0)
                {
                    return ReturnError("您已關注此車輛!!");
                }

                APCommonFun.ExecSafeSqlCommand_MSSQL(
                    sql,
                    new List<SqlParameter>
                    {
                        new SqlParameter("@user_id", user_id),
                        new SqlParameter("@shopNo", shopNo),
                        new SqlParameter("@brand", brand)
                    }
                );
                if (user_id != "")
                {
                    new Services.OneidService().UpdateMemberLog(RequestToken, Convert.ToInt64(user_id), "關注車輛", brand);
                }

                return ReturnOK();
            }
            catch (Exception ex)
            {
                APCommonFun.Error("[AddNoticeRecordController]99：" + ex.ToString());
                return ReturnException();
            }
        }
    }
}
