using Jose;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace WebApplication.Services
{
    /// <summary>
    /// JwtToken 生成功能
    /// </summary>
    public class JwtService
    {
        private readonly string _SecretKey = WebConfigurationManager.AppSettings["secret"];

        public JwtService()
        {
            
        }

        /// <summary>
        /// 解密 JwtToken
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public DataModels.ApiJWT DecryptActiveToken(HttpRequestMessage request)
        {
            try
            {
                var token = request.Headers.GetValues("Authorization").FirstOrDefault().Substring("Bearer ".Length);
                var jwtObject = JWT.Decode<Dictionary<string, object>>(token, Encoding.UTF8.GetBytes(_SecretKey), JwsAlgorithm.HS512);

                return new DataModels.ApiJWT()
                {
                    Brand = jwtObject["brand"].ToString(),
                    Member_seq = Convert.ToInt32(jwtObject["member_seq"]),
                    Member_mail = jwtObject["member_mail"].ToString(),
                    Expires = DateTime.ParseExact(jwtObject["exp"].ToString(), "yyyyMMddHHmmss",
                        System.Globalization.CultureInfo.InvariantCulture)
                };
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}