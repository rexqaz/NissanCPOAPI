using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web.Http;
using System.Web.Http.Cors;
using WebApplication.Models;

namespace WebApplication.Controllers.api
{
    public class PayController : ApiController
    {
        /// <summary>
        /// 支付
        /// </summary>
        /// <param name="Data"></param>
        /// <returns></returns>
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [Route("api/Pay/{prepaidNo}")]
        public HttpResponseMessage Post(string prepaidNo)
        {
            string nissanPath = ConfigurationManager.AppSettings["nissanPath"];
            string nissanBackendPath = ConfigurationManager.AppSettings["nissanBackendPath"];
            string esuncardPath = ConfigurationManager.AppSettings["esuncardPath"]; 
            CarShopEntities _db = new CarShopEntities();
            DateTime ext = DateTime.Now.AddMinutes(-5);
            Prepaid prepaid = _db.Prepaid.Where(x => x.prepaidNo == prepaidNo && x.paidStatus == "未付款" && ext < x.createTime).FirstOrDefault();
            Shops shops = prepaid != null ? _db.Shops.Where(x => x.ShopNo == prepaid.shopNo).FirstOrDefault() : null;
            if (prepaid == null || shops == null)
            {
                var response = new HttpResponseMessage(HttpStatusCode.Redirect);
                response.Headers.Location = new Uri($"{nissanPath}?code=404");
                return response;
            }

            //string macKey = "QN6EULEVA546CXLX6KGJSCPPP7PEDQIE";
            var key = GetDealersKey(shops.dealer);

            // 查無經銷無法取得金流key
            if (key == null)
            {
                var response = new HttpResponseMessage(HttpStatusCode.Redirect);
                response.Headers.Location = new Uri($"{nissanPath}?code=404");
                return response;
            }

            prepaidData tmp = new prepaidData();
            string macKey = key.MACKEY;
            tmp.ONO = prepaidNo;
            tmp.U = $"{nissanBackendPath}api/PayCallback";
            tmp.MID = key.MID;
            tmp.TA = "3000";
            tmp.TID = "EC000001";

            string responseStr = @"<!DOCTYPE html>
            <html lang ='en'>
            <head>
                <meta charset ='UTF-8'>
                <meta name = 'viewport' content='width=device-width, initial-scale=1.0'>
                <title>Form Data </title>
            </head>
            <body onload='send()'>
            <form action = '" + esuncardPath + @"' method = 'post' style='display: none;'>
                <label for= 'data'> Data:</label>

                    <input type='hidden' id = 'data' name = 'data' value='" + JsonConvert.SerializeObject(tmp) + @"' required>

                    <br>

                    <label for='mac'> MAC:</label>

                    <input type ='hidden' id='mac' name='mac' value='" + APCommonFun.SHA256HashHex(JsonConvert.SerializeObject(tmp) + macKey) + @"' required>

                    <br>

                    <label for='ksn'> KSN:</label>

                    <input type ='hidden' id = 'ksn' name = 'ksn' value='1' required>

                    <br>

                    <input id='send-btn' type = 'submit' value = '送出'>

                </form>
            </body>
            <script>function send() { document.getElementById('send-btn').click() }</script>
            </html> ";

            var htmlResponse = new HttpResponseMessage();
            htmlResponse.Content = new StringContent(responseStr);
            htmlResponse.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");

            return htmlResponse;
        }

        /// <summary>
        /// 取得金流key
        /// </summary>
        /// <param name="dealerName"></param>
        /// <returns></returns>
        public DealersKey GetDealersKey(string dealerName)
        {
            string id = "";
            switch (dealerName)
            {
                case "裕民":
                    id = "EM";
                    break;
                case "裕信":
                    id = "ES";
                    break;
                case "匯聯":
                    id = "HL";
                    break;
                case "國通":
                    id = "KT";
                    break;
                case "元隆":
                    id = "UL";
                    break;
                case "裕昌":
                    id = "YA";
                    break;
                case "裕唐":
                    id = "YF";
                    break;
                case "裕新":
                    id = "YJ";
                    break;
                case "誠隆":
                    id = "YK";
                    break;
            }

            if (string.IsNullOrEmpty(id))
                return null;

            return new DealersKey
            {
                MID = ConfigurationManager.AppSettings[$"dealer:{id}:MID"],
                MACKEY = ConfigurationManager.AppSettings[$"dealer:{id}:MACKEY"],
            };
        }
    }

    public class DealersKey
    {
        public string MID { get; set; }
        public string MACKEY { get; set; }
    }
    public class prepaidData
    {
        public string ONO { get; set; }
        public string U { get; set; }
        public string MID { get; set; }
        public string TA { get; set; }
        public string TID { get; set; }
    }

    public class prepaidClass
    {
        public string data { get; set; }
        public string mac { get; set; }
        public string ksn { get; set; }
    }
}
