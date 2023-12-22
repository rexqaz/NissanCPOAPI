using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Mvc;
using WebApplication.Extensions;
using WebApplication.ViewModels;

namespace WebApplication.Controllers.api
{

    [JwtAuthActionFilter]
    public class CancelCarSubscriptionController : BaseAPIController
    {
        /// <summary>
        /// 取消車輛訂閱
        /// </summary>
        /// <param name="Data"></param>
        /// <returns></returns>
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public object Get(int id)
        {            
            try
            {
                string sql = "Delete [Subscriptions] where user_id=@id ";

                //取消訂閱";
                APCommonFun.ExecSafeSqlCommand_MSSQL(
                   sql,
                   new List<SqlParameter>
                   {
                        new SqlParameter("@id", id)
                   }
               );
              
                return ReturnOK();
            }
            catch (Exception ex)
            {
                APCommonFun.Error("[CancelCarSubscriptionController]99：" + ex.ToString());
                return ReturnException();
            }
        }
    }
}
