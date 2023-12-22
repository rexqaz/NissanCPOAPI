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
    public class MyFavouriteAddController : BaseAPIController
    {
        /// <summary>
        /// 加入我的最愛
        /// </summary>
        /// <param name="Data"></param>
        /// <returns></returns>
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public object Post(Info_MyFavouriteAddModel Data)
        {
            try
            {                
                string InputIsok = "Y";
                string ReturnErr = "";
                string carShop_id = string.Empty;
                string user_id = string.Empty;

                if (Data.carShop_id != null)
                {
                    carShop_id = APCommonFun.CDBNulltrim(Data.carShop_id);
                }
                if (Data.user_id != null)
                {
                    user_id = APCommonFun.CDBNulltrim(Data.user_id);
                }

                //第一步 : 先判斷有沒有必填未填寫，
                if (carShop_id == "" || user_id == "") //必填                       
                {
                    if (carShop_id == "")
                    {
                        InputIsok = "N";
                        ReturnErr = "執行動作錯誤-carShop_id 為必填欄位";
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
                    APCommonFun.Error("[MyFavouriteAddController]90-" + ReturnErr);
                    return ReturnError(ReturnErr);
                }

                string sql = "insert into MyFavourites (carShop_id, user_id, createTime) values (@carShop_id , @user_id , getdate() ) ";
                string sql2 = "select * from MyFavourites where carShop_id=@carShop_id  and user_id=@user_id  ";


                DataTable dt = APCommonFun.GetSafeDataTable_MSSQL(
                     sql2,
                     new List<SqlParameter>
                     {
                                        new SqlParameter("@carShop_id", carShop_id),
                                        new SqlParameter("@user_id", user_id),
                     }
                 );
                //DataTable dt = APCommonFun.GetDataTable_MSSQL(sql2);
                if (dt.Rows.Count > 0)
                {
                    return ReturnError("此筆資料已存在");
                }

                APCommonFun.ExecSafeSqlCommand_MSSQL(
                    sql,
                    new List<SqlParameter>
                    {
                                        new SqlParameter("@carShop_id", carShop_id),
                                        new SqlParameter("@user_id", user_id)
                    }
                );

                return ReturnOK();
            }
            catch (Exception ex)
            {
                APCommonFun.Error("[MyFavouriteAddController]99：" + ex.ToString());
                return ReturnException();
            }
        }

    }
}
