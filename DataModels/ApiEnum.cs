using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication.DataModels
{
    /// <summary>
    /// API回傳狀態代碼
    /// </summary>
    public enum EnumApiStatus
    {
        /// <summary>
        /// API回傳成功
        /// </summary>
        Ok = 200,
        /// <summary>
        /// API回傳失敗
        /// </summary>
        Error_400 = 400,
        /// <summary>
        /// API回傳失敗(db找不到該帳號資料)
        /// </summary>
        Error_401 = 401,
        /// <summary>
        /// API回傳失敗(帳號停用)
        /// </summary>
        Error_402 = 402,
        /// <summary>
        /// API回傳失敗(禁止異地登入(AToken相同))
        /// </summary>
        Error_403 = 403,
        /// <summary>
        /// API回傳失敗(禁止異地登入(RToken相同))
        /// </summary>
        Error_404 = 404,
        /// <summary>
        /// 該號碼已註冊，請重新登入
        /// </summary>
        Error_405 = 405,
        /// <summary>
        /// API回傳失敗(系統錯誤)
        /// </summary>
        Exception = 500,
        /// <summary>
        /// Token異常
        /// </summary>
        ErrorToken = 600,
        /// <summary>
        /// 需要更新Token
        /// </summary>
        NeedRefreshToken = 601,
    }


    /// <summary>
    /// 使用者角色名稱
    /// </summary>
    public enum EnumRoleName
    {
        經銷商管理人員,
        品牌管理人員,
        據點管理人員,
    }
}