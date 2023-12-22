using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using WebApplication.Extensions;
using WebApplication.ViewModels;

namespace WebApplication.Controllers.api
{
    [JwtAuthActionFilter]
    public class AddCarSellController : BaseAPIController
    {

        private string _InnerMailIp;
        private string _InnerMailPort;

        /// <summary>
        /// 我要賣車 參數以form-data傳入
        /// </summary>
        /// <param name="Data"></param>
        /// <returns></returns>
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public async Task<object> Post()
        {
            try
            {
                // Check if the request contains multipart/form-data.
                if (!Request.Content.IsMimeMultipartContent())
                {
                    throw new Exception("The request not contains multipart/form-data.");
                }

                string root = HttpContext.Current.Server.MapPath("~/App_Data");
                var provider = new MultipartFormDataStreamProvider(root);


                _InnerMailIp = ConfigurationManager.AppSettings["mailServerIp"]; ;
                _InnerMailPort = ConfigurationManager.AppSettings["mailServerPort"];


                // Read the form data.
                await Request.Content.ReadAsMultipartAsync(provider);

                // 檢查資料
                var info = CheckData(provider);

                //第二步 : 如果有必填未給值，回傳告知必填欄位要有資料
                if (info.InputIsok == "N")
                {
                    APCommonFun.Error("[AddCarSellController]90-" + info.ReturnErr);
                    return ReturnError(info.ReturnErr);
                }

                var dealerCode = string.Empty;
                var dealerCode2 = string.Empty;
                var sqlString = string.Empty;
                var isSendDMS = info.needConsult && string.IsNullOrEmpty(info.dealerName) == false;

                if (isSendDMS)
                {
                    var dicValues = new Dictionary<string, string>();

                    sqlString = "select * from DealerCodes where dealer = @newCarDealer  ";
                    dicValues.Clear();
                    dicValues.Add("@newCarDealer", info.dealerName);
                    var dt2 = APCommonFun.GetDataTable_MSSQL(sqlString, dicValues);
                    if (dt2.Rows.Count > 0)
                    {
                        dealerCode = APCommonFun.CDBNulltrim(dt2.Rows[0]["code"].ToString());
                    }


                    sqlString = "select * from Dealers where  seq=@seq  ";
                    dicValues.Clear();
                    dicValues.Add("@seq", info.newDealerSeq.ToString());
                    var dt3 = APCommonFun.GetDataTable_MSSQL(sqlString, dicValues);
                    if (dt3.Rows.Count > 0)
                    {
                        dealerCode2 = APCommonFun.CDBNulltrim(dt3.Rows[0]["dealerCode"].ToString());
                    }
                }



                //用交易模式寫入資料，同時產生流水號
                APCommonFun.Transaction_AddCarSellController_MSSQL(info);
                if (string.IsNullOrEmpty(info.SellNo))
                {
                    info.ReturnErr = "寫入異常，沒有正確產生SellNo";

                    APCommonFun.Error("[AddCarSellController]90-" + info.ReturnErr);
                    return ReturnError(info.ReturnErr);
                }

                SendMail_01(info);

                if (!string.IsNullOrEmpty(info.salesRep))
                {
                    SendMail_02(info);
                }

                SendMail_03(info);

                //勾選我同意接受新車諮詢服務 && 有選擇新車營業據點
                if (isSendDMS)
                {
                    //如果這兩個代碼找不到就不寄信
                    if (string.IsNullOrEmpty(dealerCode) || string.IsNullOrEmpty(dealerCode2))
                    {
                        APCommonFun.Error($"[AddCarSellController]99：代碼找不到 =>SellNo={info.SellNo}, dealerCode={dealerCode}, dealerCode2={dealerCode2}");
                        return ReturnOK("儲存成功");
                    }

                    string autoDMSApi = ConfigurationManager.AppSettings["autoDMSApi"];
                    string accessToken = APCommonFun.HttpWebGet(autoDMSApi + "Clue/getkey");

                    MultipartFormDataContent multiContent = new MultipartFormDataContent();

                    StringContent key = new StringContent(accessToken);
                    StringContent p1 = new StringContent("CPO");
                    StringContent p2 = new StringContent(info.SellNo);
                    StringContent p3 = new StringContent(info.name);
                    StringContent p4 = new StringContent(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    StringContent p5 = new StringContent("B220191971");
                    StringContent p6 = new StringContent(info.mobile);
                    StringContent p7 = new StringContent(info.email);
                    StringContent p8 = new StringContent(info.title == "先生" ? "M" : "F");
                    StringContent p9 = new StringContent("08");
                    StringContent p10 = new StringContent(info.carModel);
                    StringContent p11 = new StringContent("Y");
                    StringContent p12 = new StringContent("035");
                    StringContent p13 = new StringContent("接受新車諮詢服務");
                    StringContent p14 = new StringContent("D");
                    StringContent p15 = new StringContent(dealerCode);
                    StringContent p16 = new StringContent(dealerCode2);
                    StringContent p17 = new StringContent(info.newCarConsultant);
                    StringContent p18 = new StringContent("無需付款");

                    multiContent.Add(key, "key");
                    multiContent.Add(p1, "p1");
                    multiContent.Add(p2, "p2");
                    multiContent.Add(p3, "p3");
                    multiContent.Add(p4, "p4");
                    multiContent.Add(p5, "p5");
                    multiContent.Add(p6, "p6");
                    multiContent.Add(p7, "p7");
                    multiContent.Add(p8, "p8");
                    multiContent.Add(p9, "p9");
                    multiContent.Add(p10, "p10");
                    multiContent.Add(p11, "p11");
                    multiContent.Add(p12, "p12");
                    multiContent.Add(p13, "p13");
                    multiContent.Add(p14, "p14");
                    multiContent.Add(p15, "p15");
                    multiContent.Add(p16, "p16");
                    multiContent.Add(p17, "p17");
                    multiContent.Add(p18, "p18");

                    HttpClient client = new HttpClient();
                    var uri = new Uri(autoDMSApi + "Clue/setClue");
                    var msg = await client.PostAsync(uri, multiContent);

                    var responseContent = await msg.Content.ReadAsStringAsync();

                    var dmsResult = JsonConvert.DeserializeObject<DMSResult>(responseContent);
                    if (dmsResult.result == "1" && string.IsNullOrEmpty(dmsResult.errorcode))
                    {
                        return ReturnOK($"SellNo={info.SellNo} 成功(DMS)");
                    }
                    else
                    {
                        return ReturnOK($"SellNo={info.SellNo} 成功({dmsResult.errorcode})");
                    }
                }

                return ReturnOK(new { Count = info.fileCount });
            }
            catch (Exception ex)
            {
                APCommonFun.Error("[AddCarSellController]99：" + ex.ToString());
                return ReturnException();
            }
        }



        /// <summary>
        /// 檢查資料
        /// </summary>
        private DataModels.AddCarSellDataInfo CheckData(MultipartFormDataStreamProvider provider)
        {            
            var info = new DataModels.AddCarSellDataInfo();
            info.yearOfManufacture = 0;
            info.monthOfManufacture = 0;
            info.milage = 0;
            info.newDealerSeq = 0;
            info.needConsult = false;
            info.Sells_seq = 0;
            info.birthday = null;


            if (provider.FormData["yearOfManufacture"] != null)
            {
                if (int.TryParse(provider.FormData["yearOfManufacture"], out int year))
                {
                    info.yearOfManufacture = year;
                }
            }
            if (provider.FormData["monthOfManufacture"] != null)
            {
                if (int.TryParse(provider.FormData["monthOfManufacture"], out int month))
                {
                    info.monthOfManufacture = month;
                }
            }
            if (provider.FormData["milage"] != null)
            {
                if (int.TryParse(provider.FormData["milage"], out int km))
                {
                    info.milage = km;
                }
            }
            if (provider.FormData["carBrand"] != null)
            {
                info.carBrand = APCommonFun.CDBNulltrim(provider.FormData["carBrand"]);
            }
            if (provider.FormData["otherBrand"] != null)
            {
                info.otherBrand = APCommonFun.CDBNulltrim(provider.FormData["otherBrand"]);
            }
            if (provider.FormData["carModel"] != null)
            {
                info.carModel = APCommonFun.CDBNulltrim(provider.FormData["carModel"]);
            }
            if (provider.FormData["licensePlate"] != null)
            {
                info.licensePlate = APCommonFun.CDBNulltrim(provider.FormData["licensePlate"]);
            }
            if (provider.FormData["area"] != null)
            {
                info.area = APCommonFun.CDBNulltrim(provider.FormData["area"]);
            }
            if (provider.FormData["dealer"] != null)
            {
                info.dealer = APCommonFun.CDBNulltrim(provider.FormData["dealer"]);
            }
            if (provider.FormData["dealerName"] != null)
            {
                info.dealerName = APCommonFun.CDBNulltrim(provider.FormData["dealerName"]);
            }
            if (provider.FormData["salesRep"] != null)
            {
                info.salesRep = APCommonFun.CDBNulltrim(provider.FormData["salesRep"]);
            }
            if (provider.FormData["brand"] != null)
            {
                info.brand = APCommonFun.CDBNulltrim(provider.FormData["brand"]);
            }
            if (provider.FormData["mobile"] != null)
            {
                info.mobile = APCommonFun.CDBNulltrim(provider.FormData["mobile"]);
            }
            if (provider.FormData["name"] != null)
            {
                info.name = APCommonFun.CDBNulltrim(provider.FormData["name"]);
            }
            if (provider.FormData["title"] != null)
            {
                info.title = APCommonFun.CDBNulltrim(provider.FormData["title"]);
            }
            if (provider.FormData["birthday"] != null)
            {
                if (DateTime.TryParse(provider.FormData["birthday"], out DateTime dateTime))
                {
                    info.birthday = dateTime;
                }
            }
            if (provider.FormData["email"] != null)
            {
                info.email = APCommonFun.CDBNulltrim(provider.FormData["email"]);
            }
            if (provider.FormData["needConsult"] != null)
            {
                info.needConsult = provider.FormData["needConsult"] == "1";
            }
            if (provider.FormData["newCarConsultant"] != null)
            {
                info.newCarConsultant = APCommonFun.CDBNulltrim(provider.FormData["newCarConsultant"]);
            }
            if (provider.FormData["newCarDealer"] != null)
            {
                info.newCarDealer = APCommonFun.CDBNulltrim(provider.FormData["newCarDealer"]);
            }
            if (provider.FormData["newDealerSeq"] != null)
            {
                var seqString = APCommonFun.CDBNulltrim(provider.FormData["newDealerSeq"]);
                if (int.TryParse(seqString, out int seqNumber))
                {
                    info.newDealerSeq = seqNumber;
                }
            }

            //判斷有沒有必填未填寫，
            if (info.yearOfManufacture == 0)
            {
                info.ReturnErr = "執行動作錯誤-[西元年]為必填欄位";
            }
            else if (info.monthOfManufacture == 0)
            {
                info.ReturnErr = "執行動作錯誤-[月份]為必填欄位";
            }
            else if (string.IsNullOrEmpty(info.licensePlate))
            {
                info.ReturnErr = "執行動作錯誤-[車牌號碼]為必填欄位";
            }
            else if (string.IsNullOrEmpty(info.area))
            {
                info.ReturnErr = "執行動作錯誤-[中古車營業據點-地區]為必填欄位";
            }
            else if (string.IsNullOrEmpty(info.mobile))
            {
                info.ReturnErr = "執行動作錯誤-[客戶-手機]為必填欄位";
            }
            else if (string.IsNullOrEmpty(info.name))
            {
                info.ReturnErr = "執行動作錯誤-[客戶-名稱]為必填欄位";
            }
            else if (string.IsNullOrEmpty(info.title))
            {
                info.ReturnErr = "執行動作錯誤-[客戶-尊稱]為必填欄位";
            }
            //else if (info.birthday == null)
            //{                
            //    info.ReturnErr = "執行動作錯誤-[客戶-生日]為必填欄位";
            //}
            else if (string.IsNullOrEmpty(info.email))
            {
                info.ReturnErr = "執行動作錯誤-[客戶-電子信箱]為必填欄位";
            }
            else if (string.IsNullOrEmpty(info.carBrand))
            {
                info.ReturnErr = "執行動作錯誤-[品牌]為必填欄位";
            }
            else if (info.carBrand == "其他品牌" && string.IsNullOrEmpty(info.otherBrand))
            {
                info.ReturnErr = "執行動作錯誤-[其他]為必填欄位";
            }
            else if (string.IsNullOrEmpty(info.carModel))
            {
                info.ReturnErr = "執行動作錯誤-[車型]為必填欄位";
            }
            else if (info.milage == 0)
            {
                info.ReturnErr = "執行動作錯誤-[使用里程]為必填欄位";
            }
            else if (string.IsNullOrEmpty(info.dealer))
            {
                info.ReturnErr = "執行動作錯誤-[中古車營業據點]為必填欄位";
            }
            else if(info.newDealerSeq == 0 && info.needConsult)
            {
                info.ReturnErr = "執行動作錯誤-[新車營業據點]為必填欄位";
            }

            //檢查圖檔
            foreach (MultipartFileData fileData in provider.FileData)
            {
                if (string.IsNullOrEmpty(fileData.Headers.ContentDisposition.FileName))
                {
                    //return Request.CreateResponse(HttpStatusCode.NotAcceptable, "This request is not properly formatted");
                    //info.ReturnErr = "This request is not properly formatted";
                    continue;
                }
                info.fileCount++;

                var fileService = new Services.FileService();
                var filePath = fileService.SaveAsBlob(fileData, "CarSell");

                string paraName = fileData.Headers.ContentDisposition.Name;
                switch (paraName)
                {
                    case string s when s.Contains("carCondition1"):
                        info.carCondition1 = filePath;
                        break;
                    case string s when s.Contains("carCondition2"):
                        info.carCondition2 = filePath;
                        break;
                    case string s when s.Contains("carCondition3"):
                        info.carCondition3 = filePath;
                        break;
                    case string s when s.Contains("carCondition4"):
                        info.carCondition4 = filePath;
                        break;
                    case string s when s.Contains("licensePicture"):
                        info.licensePicture = filePath;
                        break;
                    default:
                        break;
                }
            }


            if (string.IsNullOrEmpty(info.ReturnErr) == false)
            {
                info.InputIsok = "N";
            }

            return info;
        }


        private void SendMail_01(DataModels.AddCarSellDataInfo info)
        {
            using (MailMessage mail = new MailMessage())
            {
                string urlBase = string.Empty;
                if (info.brand == "NISSAN")
                {
                    urlBase = ConfigurationManager.AppSettings["nissanPath"];
                    mail.From = new MailAddress("nissanCPO@yulon-nissan.com.tw");
                }
                else
                {
                    urlBase = ConfigurationManager.AppSettings["infinitiPath"];
                    mail.From = new MailAddress("infinitiCPO@infiniti-taiwan.com");
                }

                mail.Body = "中古車據點管理員您好<br>";
                mail.Body += "已收到會員的我要賣車資訊<br><br>我要賣車編號：" + info.SellNo + "<br>會員姓名：" + info.name + "<br>稱謂：" + info.title + "<br>品牌：" + (info.carBrand == "其他" ? info.otherBrand : info.carBrand) + "<br>車型：" + info.carModel + "<br>出廠年月：西元" + info.yearOfManufacture + "年" + info.monthOfManufacture + "月<br>使用里程：" + Convert.ToInt32(info.milage).ToString("###,###") + "公里<br>車牌號碼：" + info.licensePlate + "<br>地區：" + info.area + "<br>中古車營業據點：" + info.dealer.Split('-')[1] + "-" + info.dealer.Split('-')[0] + "<br>中古車銷售顧問：" + (string.IsNullOrEmpty(info.salesRep) ? "無" : info.salesRep) + "<br>是否接受新車諮詢：" + (info.needConsult ? "是" : "否") + "<br><br>提醒，若無指定中古車銷售顧問，<br>請進入後台系統指派。<br><br>請點選單一後台網址登入查看<br>連結：" + urlBase + "backend/ <br><br>此為系統自動發送的郵件，請勿回覆";

                //寄信給該據點的據點管理人員
                var mailList = new Services.EmailService().GetUserMail("據點管理人員", info.brand, info.dealerName, info.dealer);
                foreach (var eMail in mailList)
                {
                    mail.To.Add(eMail);
                    mail.IsBodyHtml = true;
                    mail.Subject = "我要賣車進單通知";

                    using (SmtpClient smtp = new SmtpClient(_InnerMailIp))
                    {
                        smtp.Port = Convert.ToInt32(_InnerMailPort);
                        smtp.Send(mail);
                    }
                }
            }
        }


        private void SendMail_02(DataModels.AddCarSellDataInfo info)
        {
            using (MailMessage mail = new MailMessage())
            {
                string urlBase = string.Empty;
                if (info.brand == "NISSAN")
                {
                    urlBase = ConfigurationManager.AppSettings["nissanPath"];
                    mail.From = new MailAddress("nissanCPO@yulon-nissan.com.tw");
                }
                else
                {
                    urlBase = ConfigurationManager.AppSettings["infinitiPath"];
                    mail.From = new MailAddress("infinitiCPO@infiniti-taiwan.com");
                }

                mail.Body = "中古車銷售顧問您好<br>";
                mail.Body += "已收到會員的我要賣車資訊<br><br>我要賣車編號：" + info.Sells_seq + "<br>會員姓名：" + info.name + "<br>稱謂：" + info.title + "<br>品牌：" + (info.carBrand == "其他" ? info.otherBrand : info.carBrand) + "<br>車型：" + info.carModel + "<br>出廠年月：西元" + info.yearOfManufacture + "年" + info.monthOfManufacture + "月<br>使用里程：" + Convert.ToInt32(info.milage).ToString("###,###") + "公里<br>車牌號碼：" + info.licensePlate + "<br>地區：" + info.area + "<br>中古車營業據點：" + info.dealer.Split('-')[1] + "-" + info.dealer.Split('-')[0] + "<br>中古車銷售顧問：" + (string.IsNullOrEmpty(info.salesRep) ? "無" : info.salesRep) + "<br>是否接受新車諮詢：" + (info.needConsult ? "是" : "否") + "<br><br>提醒，請盡快與會員進行聯繫，<br>並回報給所屬中古車據點管理員。<br><br>此為系統自動發送的郵件，請勿回覆";


                string sql4 = "SELECT * FROM [CarShop].[dbo].[DealerPersons] where name = @salesRep   ";
                //DataTable dt2 = APCommonFun.GetDataTable_MSSQL(sql4);
                DataTable dt2 = APCommonFun.GetSafeDataTable_MSSQL(
                    sql4,
                    new List<SqlParameter>
                    {
                                            new SqlParameter("@salesRep", info.salesRep)
                    }
                );
                if (dt2.Rows.Count > 0)
                {
                    mail.To.Add(new MailAddress(dt2.Rows[0]["Email"].ToString()));


                    mail.IsBodyHtml = true;
                    mail.Subject = "我要賣車進單通知";

                    using (SmtpClient smtp = new SmtpClient(_InnerMailIp))
                    {
                        smtp.Port = Convert.ToInt32(_InnerMailPort);
                        smtp.Send(mail);
                    }
                }

            }
        }


        private void SendMail_03(DataModels.AddCarSellDataInfo info)
        {
            using (MailMessage mail = new MailMessage())
            {
                string urlBase = string.Empty;
                if (info.brand == "NISSAN")
                {
                    urlBase = ConfigurationManager.AppSettings["nissanPath"];
                    mail.From = new MailAddress("nissanCPO@yulon-nissan.com.tw");
                    mail.Body = "NISSAN會員您好<br>";
                }
                else
                {
                    urlBase = ConfigurationManager.AppSettings["infinitiPath"];
                    mail.From = new MailAddress("infinitiCPO@infiniti-taiwan.com");
                    mail.Body = "INFINITI會員您好<br>";
                }

                mail.Body += "已收到您在我要賣車的資料填寫。<br><br>以下為您在我要賣車的填寫資訊，，將會由中古車銷售顧問與您聯繫。<br>會員姓名：" + info.name + "<br>稱謂：" + info.title + "<br>品牌：" + (info.carBrand == "其他" ? info.otherBrand : info.carBrand) + "<br>車型：" + info.carModel + "<br>出廠年月：西元" + info.yearOfManufacture + "年" + info.monthOfManufacture + "月<br>使用里程：" + Convert.ToInt32(info.milage).ToString("###,###") + "公里<br>車牌號碼：" + info.licensePlate + "<br>地區：" + info.area + "<br>中古車營業據點：" + info.dealer.Split('-')[1] + "-'" + info.dealer.Split('-')[0] + "<br>中古車銷售顧問：" + (string.IsNullOrEmpty(info.salesRep) ? "無" : info.salesRep) + "<br>是否接受新車諮詢：" + (info.needConsult ? "是" : "否") + "<br>新車營業據點：" + info.newCarDealer + "<br>新車銷售顧問：" + info.newCarConsultant + "<br><br>另外，若您有勾選我願意接受新車諮詢，將由新車銷售顧問與您聯繫。<br><br>歡迎造訪" + (info.brand == "NISSAN" ? "NISSA" : "INFINITI") + "中古車網站。<br>網址：" + urlBase + "<br><br>此為系統自動發送的郵件，請勿回覆";

                mail.To.Add(new MailAddress(info.email));
                mail.IsBodyHtml = true;
                mail.Subject = "我要賣車通知";

                using (SmtpClient smtp = new SmtpClient(_InnerMailIp))
                {
                    smtp.Port = Convert.ToInt32(_InnerMailPort);
                    smtp.Send(mail);
                }
            }
        }

        private class DMSResult
        {
            public string result { get; set; }
            public string errorcode { get; set; }
            public string message { get; set; }
            public object data { get; set; }
        }

    }
}
