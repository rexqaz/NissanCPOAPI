using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Configuration;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Jose;

namespace WebApplication.Extensions
{
    public class JwtAuthActionFilter : ActionFilterAttribute
    {

        private static readonly string _SecretKey = WebConfigurationManager.AppSettings["secret"];

        /// <summary>
        /// 
        /// </summary>
        /// <param name="actionContext"></param>
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            var request = actionContext.Request;

            // 排除不需要驗證的 API
            if (WithoutVerifyToken(request.RequestUri.ToString()) == false)
            {
                // 有取到 JwtToken 後，判斷授權格式不存在且不正確時
                if (request.Headers.Authorization == null || request.Headers.Authorization.Scheme != "Bearer")
                {
                    TokenError(actionContext);
                }
                else
                {
                    try
                    {
                        // 解密token
                        var reqToken = request.Headers.Authorization.Parameter;
                        var jwt = GetToken(reqToken);

                        // token過期
                        if (jwt.Expires < DateTime.UtcNow.AddHours(8))
                        {
                            AuthRefresh(actionContext);
                        }
                        else
                        {
                            //跟oneid API要使用者資料
                            var apiResult = new Services.OneidService().GetMember(reqToken);

                            //取回來有異常就再往回踢
                            switch (apiResult.StatusCode)
                            {
                                case (int)DataModels.EnumApiStatus.Ok:
                                    {
                                        var member = apiResult.Data;
                                        if (member == null || member.seq != jwt.Member_seq)
                                        {
                                            OneidAPIError(actionContext, DataModels.EnumApiStatus.Error_400, "OneidAPI-會員資料異常");
                                        }
                                    }
                                    break;
                                case (int)DataModels.EnumApiStatus.Error_400:
                                case (int)DataModels.EnumApiStatus.Error_401:
                                case (int)DataModels.EnumApiStatus.Error_402:
                                case (int)DataModels.EnumApiStatus.Error_403:
                                case (int)DataModels.EnumApiStatus.Error_404:
                                case (int)DataModels.EnumApiStatus.Exception:
                                    OneidAPIError(actionContext, (DataModels.EnumApiStatus)apiResult.StatusCode, $"OneidAPI-{apiResult.Message}");
                                    break;                                                                   
                                case (int)DataModels.EnumApiStatus.ErrorToken:
                                    TokenError(actionContext);
                                    break;
                                case (int)DataModels.EnumApiStatus.NeedRefreshToken:
                                    AuthRefresh(actionContext);
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        // 解密失敗
                        TokenError(actionContext);
                    }
                }
            }
            base.OnActionExecuting(actionContext);
        }




        /// <summary>
        /// Token解密
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private static DataModels.ApiJWT GetToken(string token)
        {
            var jwtObject = JWT.Decode<Dictionary<string, object>>(token, Encoding.UTF8.GetBytes(_SecretKey), JwsAlgorithm.HS512);

            return new DataModels.ApiJWT()
            {
                Member_seq = Convert.ToInt32(jwtObject["member_seq"]),
                Member_mail = jwtObject["member_mail"].ToString(),
                Expires = DateTime.ParseExact(jwtObject["exp"].ToString(), "yyyyMMddHHmmss",
                    System.Globalization.CultureInfo.InvariantCulture)
            };
        }



        /// <summary>
        /// 要求更新token
        /// </summary>
        /// <param name="actionContext"></param>
        private void AuthRefresh(HttpActionContext actionContext)
        {
            var responseObject = new DataModels.ApiResult<object>(DataModels.EnumApiStatus.NeedRefreshToken, "", new { });
            var response = actionContext.Request.CreateResponse(HttpStatusCode.Forbidden, responseObject);
            actionContext.Response = response;
        }

        /// <summary>
        /// token異常
        /// </summary>
        /// <param name="actionContext"></param>
        private void TokenError(HttpActionContext actionContext)
        {
            var responseObject = new DataModels.ApiResult<object>(DataModels.EnumApiStatus.ErrorToken, "請重新登入", new { });
            var response = actionContext.Request.CreateResponse(HttpStatusCode.Forbidden, responseObject);
            actionContext.Response = response;
        }


        /// <summary>
        /// OneidAPI-回傳異常或資料錯誤
        /// </summary>
        /// <param name="actionContext"></param>
        /// <param name="message"></param>
        private void OneidAPIError(HttpActionContext actionContext, DataModels.EnumApiStatus apiStatus, string message)
        {
            var responseObject = new DataModels.ApiResult<object>(apiStatus, message, new { });
            var response = actionContext.Request.CreateResponse(HttpStatusCode.OK, responseObject);
            actionContext.Response = response;
        }

        /// <summary>
        /// 需要排除的API
        /// </summary>
        /// <param name="requestUri"></param>
        /// <returns></returns>
        private bool WithoutVerifyToken(string requestUri)
        {
            switch (requestUri)
            {
                case string s when s.Contains("/PayCallback"):
                    break;
                case string s when s.Contains("/Pay"):
                    break;
                case string s when s.Contains("/AddBannerHit"):
                    break;
                case string s when s.Contains("/CarCompare"):
                    break;
                case string s when s.Contains("/GetBanner"):
                    break;
                case string s when s.Contains("/GetCarShop"):
                    break;
                case string s when s.Contains("/GetDealerPersons"):
                    break;
                case string s when s.Contains("/GetDealers"):
                    break;
                case string s when s.Contains("/GetMonthlyHit"):
                    break;
                case string s when s.Contains("/GetNews"):
                    break;
                case string s when s.Contains("/GetNewsOne"):
                    break;
                case string s when s.Contains("/SearchCar"):
                    break;
                case string s when s.Contains("/GetNoticeRecord"):
                    break;            
                case string s when s.Contains("/GetOtpAndSendSms"):
                    break;      
                case string s when s.Contains("/VerifyOtp"):
                    break;
                case string s when s.Contains("/GetReleaseCarModel"):
                    break;
                case string s when s.Contains("/VerifyReCaptcha"):
                    break;
                case string s when s.Contains("/VisitOrder"):
                    break;
                case string s when s.Contains("/AddCarSell"):
                    break;
                default:
                    return false;
            }

            return true;
        }



        #region MyRegion


        //public override void OnActionExecuting(HttpActionContext actionContext)
        //{
        //    string secret = ConfigurationManager.AppSettings["secret"];

        //    var request = actionContext.Request;
        //    if (!WithoutVerifyToken(request.RequestUri.ToString()))
        //    {
        //        if (request.Headers.Authorization == null || request.Headers.Authorization.Scheme != "Bearer")
        //        {
        //            string failedValue = "Lost Token";
        //            actionContext.Request.Properties.Add(new KeyValuePair<string, object>("failed", failedValue));
        //            //throw new System.Exception("Lost Token");
        //        }
        //        else
        //        {
        //            try
        //            {
        //                //解密後會回傳Json格式的物件(即加密前的資料)
        //                var jwtObject = Jose.JWT.Decode<Dictionary<string, Object>>(
        //                request.Headers.Authorization.Parameter,
        //                Encoding.UTF8.GetBytes(secret),
        //                JwsAlgorithm.HS512);
        //            }
        //            catch (Exception ex)
        //            {
        //                actionContext.Request.Properties.Add(new KeyValuePair<string, object>("failed", ex.ToString()));
        //            }
        //        }
        //    }

        //    base.OnActionExecuting(actionContext);
        //}

        ////Login不需要驗證因為還沒有token
        //public bool WithoutVerifyToken(string requestUri)
        //{
        //    if (requestUri.EndsWith("/AddBannerHit") || requestUri.EndsWith("/CarCompare") || requestUri.EndsWith("/GetBanner") || requestUri.EndsWith("/GetCarShop") || requestUri.EndsWith("/GetDealerPersons") || requestUri.EndsWith("/GetDealers") || requestUri.EndsWith("/GetMonthlyHit") || requestUri.EndsWith("/GetNews") || requestUri.EndsWith("/GetNewsOne") || requestUri.EndsWith("/SearchCar"))
        //        return true;
        //    return false;
        //}

        #endregion
    }
}