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
    public class AddBannerHitController : BaseAPIController
    {
        /// <summary>
        /// 新增Banner點擊數或曝光數
        /// </summary>
        /// <param name="Data"></param>
        /// <returns></returns>
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public object Post(Info_AddBannerHitModel Data)
        {
            string InputIsok = "Y";
            string ReturnErr = "";
            string type = string.Empty;
            string banner_id = string.Empty;

            try
            {
                if (Data.banner_id != null)
                {
                    banner_id = APCommonFun.CDBNulltrim(Data.banner_id);
                }

                if (Data.type != null)
                {
                    type = APCommonFun.CDBNulltrim(Data.type);
                }

                //第一步 : 先判斷有沒有必填未填寫，
                if (type == "" || banner_id == "") //必填                       
                {
                    if (banner_id == "")
                    {
                        InputIsok = "N";
                        ReturnErr = "執行動作錯誤-banner_id 為必填欄位";
                    }
                    //else
                    //{
                    //    InputIsok = "N";
                    //    ReturnErr = "執行動作錯誤-type 為必填欄位";
                    //}
                }

                //第二步 : 如果有必填未給值，回傳告知必填欄位要有資料
                if (InputIsok == "N")
                {
                    APCommonFun.Error("[AddBannerHitController]90-" + ReturnErr);
                    return ReturnError(ReturnErr);
                }

                string sqlPre = "select * from Banners where seq=@banner_id ";
                string sql = "update [Banners] set hitCount=@hitCount where seq=@banner_id  ";



                DataTable dt = APCommonFun.GetSafeDataTable_MSSQL(
                    sqlPre,
                    new List<SqlParameter>
                    {
                        new SqlParameter("@banner_id", banner_id)
                    }
                );
                string hitCount = "0";
                string viewCount = "0";
                if (dt.Rows.Count > 0)
                {
                    hitCount = APCommonFun.CDBNulltrim(dt.Rows[0]["hitCount"].ToString());
                    viewCount = APCommonFun.CDBNulltrim(dt.Rows[0]["viewCount"].ToString());
                }

                if (string.IsNullOrEmpty(hitCount)) hitCount = "0";
                if (string.IsNullOrEmpty(viewCount)) viewCount = "0";

                APCommonFun.ExecSafeSqlCommand_MSSQL(
                    sql,
                    new List<SqlParameter>
                    {
                        new SqlParameter("@hitCount", (Convert.ToInt32(hitCount) + 1).ToString()),
                        new SqlParameter("@banner_id", banner_id)
                    }
                );

                return ReturnOK();
            }
            catch (Exception ex)
            {
                APCommonFun.Error("[AddBannerHitController]99：" + ex.ToString());
                return ReturnException();
            }
        }
    }
}
