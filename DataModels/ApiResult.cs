using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication.DataModels
{
    /// <summary>
    /// 
    /// </summary>
    public class ApiJWT
    {
        /// <summary>
        /// 使用者seq
        /// </summary>
        public int Member_seq { set; get; }
        /// <summary>
        /// 使用者mail
        /// </summary>
        public string Member_mail { set; get; }
        /// <summary>
        /// 所屬品牌
        /// </summary>
        public string Brand { get; set; }
        /// <summary>
        /// Token到期時間
        /// </summary>
        public DateTime Expires { set; get; }
    }

    /// <summary>
    /// API呼叫時，傳回的統一物件
    /// </summary>
    public class ApiResult<T>
    {
        /// <summary>
        /// 結果代碼(200=成功，其餘為錯誤代號)
        /// </summary>
        public int StatusCode { get; set; }
        /// <summary>
        /// 錯誤訊息
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// 資料本體
        /// </summary>
        public T Data { get; set; }

        /// <summary>
        /// ApiResult
        /// </summary>
        public ApiResult()
        {
            
        }

        /// <summary>
        /// ApiResult
        /// </summary>
        public ApiResult(EnumApiStatus statusCode, string message, T data)
        {
            this.StatusCode = (int)statusCode;
            this.Message = message;
            this.Data = data;
        }

    }
}