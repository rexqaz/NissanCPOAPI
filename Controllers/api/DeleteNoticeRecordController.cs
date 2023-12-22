using System;
using System.Collections.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using WebApplication.ViewModels;
using System.Web.Http.Cors;
using System.Data.SqlClient;
using WebApplication.Extensions;

namespace WebApplication.Controllers.api
{
    [JwtAuthActionFilter]
    public class DeleteNoticeRecordController : BaseAPIController
    {
        /// <summary>
        /// 移除車輛關注
        /// </summary>
        /// <param name="Data"></param>
        /// <returns></returns>
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public object Post(Info_AddNoticeRecordModel Data)
        {
            string InputIsok = "Y";
            string ReturnErr = "";
            string shopNo = string.Empty;
            string user_id = string.Empty;

            try
            {
                if (Data.shopNo != null)
                {
                    shopNo = APCommonFun.CDBNulltrim(Data.shopNo);
                }
                if (Data.user_id != null)
                {
                    user_id = APCommonFun.CDBNulltrim(Data.user_id);
                }

                //第一步 : 先判斷有沒有必填未填寫，
                if (shopNo == "" || user_id == "") //必填                       
                {
                    if (shopNo == "")
                    {
                        InputIsok = "N";
                        ReturnErr = "執行動作錯誤-shopNo 為必填欄位";
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
                    APCommonFun.Error("[DeleteNoticeRecordController]90-" + ReturnErr);
                    return ReturnError(ReturnErr);
                }


                string sql = "Delete [NoticeRecords] where member=@user_id  and shopNo=@shopNo   ";


                APCommonFun.ExecSafeSqlCommand_MSSQL(
                   sql,
                   new List<SqlParameter>
                   {
                            new SqlParameter("@user_id", user_id),
                            new SqlParameter("@shopNo", shopNo)
                   }
               );

                return ReturnOK();
            }
            catch (Exception ex)
            {
                APCommonFun.Error("[DeleteNoticeRecordController]99：" + ex.ToString());
                return ReturnException();
            }
        }
    }
}
