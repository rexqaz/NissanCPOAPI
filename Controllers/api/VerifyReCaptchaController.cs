using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Configuration;
using System.Web.Http;
using System.Web.Http.Cors;

namespace WebApplication.Controllers.api
{
    /// <summary>
    /// 驗證reCaptcha
    /// </summary>
    public class VerifyReCaptchaController : BaseAPIController
    {
        /// <summary>
        /// 
        /// </summary>
        public class VerifyReCaptchaParameter
        {
            /// <summary>
            /// 前端送來的token
            /// </summary>
            public string token { get; set; }
        }

        /// <summary>
        /// 驗證reCaptcha
        /// </summary>
        /// <param name="brand">品牌</param>
        /// <returns></returns>
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public object Post(VerifyReCaptchaParameter para)
        {

            try
            {
                JObject jObject = new JObject();
                if (string.IsNullOrWhiteSpace(para.token))
                {
                    return ReturnError("請確認是否為機器人");
                }
                else
                {
                    var reCAPTCHAurl = WebConfigurationManager.AppSettings["reCAPTCHAurl"];
                    var reCAPTCHAkey = WebConfigurationManager.AppSettings["reCAPTCHAkey"];



                    // 建立一個HttpWebRequest網址指向Google的驗證API
                    var req = (HttpWebRequest)HttpWebRequest.Create(reCAPTCHAurl);
                    // Post的資料
                    // secret:secret_key
                    // response:回傳的Token
                    // remoteip:設定的Domain Name
                    string posStr = $"secret={reCAPTCHAkey}&response={para.token}&remoteip={Request.RequestUri.Host}";
                    byte[] byteStr = Encoding.UTF8.GetBytes(posStr);
                    req.Method = "POST";
                    req.ContentType = "application/x-www-form-urlencoded";
                    // 把要Post資料寫進HttpWebRequest
                    using (Stream streamArr = req.GetRequestStream())
                    {
                        streamArr.Write(byteStr, 0, byteStr.Length);
                    }
                    // 取得回傳資料
                    using (var res = (HttpWebResponse)req.GetResponse())
                    {
                        using (StreamReader getJson = new StreamReader(res.GetResponseStream()))
                        {
                            var json = getJson.ReadToEnd();
                            jObject = JObject.Parse(json);
                        }
                    }
                }

                return ReturnOK(jObject);
            }
            catch (Exception ex)
            {
                APCommonFun.Error("[VerifyReCaptchaController]99：" + ex.ToString());
                return ReturnException();
            }
        }
    }
}
