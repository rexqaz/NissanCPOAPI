using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WebApplication.Controllers.api
{
    /// <summary>
    /// 
    /// </summary>    
    public class BaseAPIController : ApiController
    {
        /// <summary>
        /// 從token解析出來的JWT資訊
        /// </summary>
        public DataModels.ApiJWT JWT
        {
            get
            {
                return new Services.JwtService().DecryptActiveToken(this.Request);
            }
        }

        /// <summary>
        /// 取得Request內的token
        /// </summary>
        public string RequestToken
        {
            get
            {
                Request.Headers.TryGetValues("Authorization", out var headerToken);
                return headerToken != null ? headerToken.FirstOrDefault() : "";
            }
        }

        public BaseAPIController()
        {

        }

        /// <summary>
        /// API-成功回傳
        /// </summary>
        [NonAction]
        public DataModels.ApiResult<object> ReturnOK()
        {
            return new DataModels.ApiResult<object>(DataModels.EnumApiStatus.Ok, "", new { });
        }

        /// <summary>
        /// API-成功回傳
        /// </summary>
        [NonAction]
        public DataModels.ApiResult<object> ReturnOK(object data)
        {
            return new DataModels.ApiResult<object>(DataModels.EnumApiStatus.Ok, "", data);
        }

        /// <summary>
        /// API-失敗回傳
        /// </summary>
        [NonAction]
        public DataModels.ApiResult<object> ReturnError(string message)
        {
            return new DataModels.ApiResult<object>(DataModels.EnumApiStatus.Error_400, message, new { });
        }

        /// <summary>
        /// API-異常回傳
        /// </summary>
        [NonAction]
        public DataModels.ApiResult<object> ReturnException()
        {
            return new DataModels.ApiResult<object>(DataModels.EnumApiStatus.Exception, "API異常，請洽系統管理員", new { });
        }

        /// <summary>
        /// API-異常回傳
        /// </summary>
        [NonAction]
        public DataModels.ApiResult<object> ReturnException(string message)
        {
            return new DataModels.ApiResult<object>(DataModels.EnumApiStatus.Exception, message, new { });
        }
    }
}
