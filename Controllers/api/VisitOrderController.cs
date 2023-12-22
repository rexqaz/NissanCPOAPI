using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using WebApplication.Extensions;
using WebApplication.ViewModels;

namespace WebApplication.Controllers.api
{
    [JwtAuthActionFilter]
    public class VisitOrderController : BaseAPIController
    {

        /// <summary>
        /// 預約賞車
        /// </summary>
        /// <param name="Data"></param>
        /// <returns></returns>
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public object Post(Info_VisitOrderModel Data)
        {
            try
            {
                string InputIsok = "Y";
                string ReturnErr = "";

                string dealerAddress = string.Empty;
                if (Data.dealerAddress != null)
                {
                    dealerAddress = APCommonFun.CDBNulltrim(Data.dealerAddress);
                }

                string dealerName = string.Empty;
                if (Data.dealerName != null)
                {
                    dealerName = APCommonFun.CDBNulltrim(Data.dealerName);
                }

                string brand = string.Empty;
                if (Data.brand != null)
                {
                    brand = APCommonFun.CDBNulltrim(Data.brand);
                }

                string dealerTel = string.Empty;
                if (Data.dealerTel != null)
                {
                    dealerTel = APCommonFun.CDBNulltrim(Data.dealerTel);
                }


                string name = string.Empty;
                if (Data.name != null)
                {
                    name = APCommonFun.CDBNulltrim(Data.name);
                }

                string mobile = string.Empty;
                if (Data.mobile != null)
                {
                    mobile = APCommonFun.CDBNulltrim(Data.mobile);
                }

                string id = string.Empty;
                if (Data.id != null)
                {
                    id = APCommonFun.CDBNulltrim(Data.id);
                }

                string email = string.Empty;
                if (Data.email != null)
                {
                    email = APCommonFun.CDBNulltrim(Data.email);
                }

                string title = string.Empty;
                if (Data.title != null)
                {
                    title = APCommonFun.CDBNulltrim(Data.title);
                }

                string birthday = string.Empty;
                if (Data.birthday != null)
                {
                    birthday = APCommonFun.CDBNulltrim(Data.birthday);
                }

                string address = string.Empty;
                if (Data.address != null)
                {
                    address = APCommonFun.CDBNulltrim(Data.address);
                }

                string others = string.Empty;
                if (Data.others != null)
                {
                    others = APCommonFun.CDBNulltrim(Data.others);
                }

                string salesRep = string.Empty;
                if (Data.salesRep != null)
                {
                    salesRep = APCommonFun.CDBNulltrim(Data.salesRep);
                }

                string period = string.Empty;
                if (Data.period != null)
                {
                    period = APCommonFun.CDBNulltrim(Data.period);
                }

                string visitCarType = string.Empty;
                if (Data.visitCarType != null)
                {
                    visitCarType = APCommonFun.CDBNulltrim(Data.visitCarType);
                }

                string visitCarYear = string.Empty;
                if (Data.visitCarYear != null)
                {
                    visitCarYear = APCommonFun.CDBNulltrim(Data.visitCarYear);
                }

                string shopNo = string.Empty;
                if (Data.shopNo != null)
                {
                    shopNo = APCommonFun.CDBNulltrim(Data.shopNo);
                }

                DateTime visitTime = new DateTime();
                if (Data.visitTime != null)
                {
                    visitTime = Data.visitTime;
                }

                //第一步 : 先判斷有沒有必填未填寫，
                if (name == "" || mobile == "" || email == "" || title == "" || birthday == "" || shopNo == "" || brand == "") //必填                       
                {
                    if (name == "")
                    {
                        InputIsok = "N";
                        ReturnErr = "執行動作錯誤-name 為必填欄位";
                    }
                    else if (mobile == "")
                    {
                        InputIsok = "N";
                        ReturnErr = "執行動作錯誤-mobile 為必填欄位";
                    }
                    else if (email == "")
                    {
                        InputIsok = "N";
                        ReturnErr = "執行動作錯誤-email 為必填欄位";
                    }
                    else if (title == "")
                    {
                        InputIsok = "N";
                        ReturnErr = "執行動作錯誤-title 為必填欄位";
                    }
                    else if (shopNo == "")
                    {
                        InputIsok = "N";
                        ReturnErr = "執行動作錯誤-shopNo 為必填欄位";
                    }
                    else if (brand == "")
                    {
                        InputIsok = "N";
                        ReturnErr = "執行動作錯誤-brand 為必填欄位";
                    }
                    else
                    {
                        InputIsok = "N";
                        ReturnErr = "執行動作錯誤-birthday 為必填欄位";
                    }
                }

                //第二步 : 如果有必填未給值，回傳告知必填欄位要有資料
                if (InputIsok == "N")
                {
                    APCommonFun.Error("[VisitOrderController]90-" + ReturnErr);
                    //return ReturnError(ReturnErr);
                }

                string verifyCode = string.Empty;
                string sql = "INSERT INTO VisitOrders (visitCarType, visitCarYear, dealerName, dealerAddress, dealerTel, name, mobile, email, title, id, birthday, address, salesRep, others, period, isConfirmed, verifyCode, verifyCodeValidTime, createTime, shopNo, visitTime, brand, updateTime, status, assignedConsultant) values "
                    + "(@visitCarType , @visitCarYear , @dealerName , @dealerAddress , @dealerTel, @name , @mobile , @email , @title , @id , @birthday ,  @address , @salesRep , @others , @period , '0', @verifyCode , getdate(), getdate(), @shopNo, @visitTime , @brand , getdate(), @status, @salesRep      ) ";

                string sql2 = "select * from Shops where seq=@shopNo  ";


                //APCommonFun.ExecSqlCommand_MSSQL(sql);
                //DataTable dt = APCommonFun.GetDataTable_MSSQL(sql2);

                APCommonFun.ExecSafeSqlCommand_MSSQL(
                    sql,
                    new List<SqlParameter>
                    {
                        new SqlParameter("@visitCarType", visitCarType),
                        new SqlParameter("@visitCarYear", visitCarYear),
                        new SqlParameter("@dealerName", dealerName),
                        new SqlParameter("@dealerAddress", dealerAddress),
                        new SqlParameter("@dealerTel", dealerTel),
                        new SqlParameter("@name", name),
                        new SqlParameter("@mobile", mobile),
                        new SqlParameter("@email", email),
                        new SqlParameter("@title", title),
                        new SqlParameter("@id", id),
                        new SqlParameter("@birthday", birthday),
                        new SqlParameter("@address", address),
                        new SqlParameter("@salesRep", salesRep),
                        new SqlParameter("@period", period),
                        new SqlParameter("@others", others),
                        new SqlParameter("@verifyCode", verifyCode),
                        new SqlParameter("@shopNo", shopNo),
                        new SqlParameter("@visitTime", visitTime.ToString("yyyy-MM-dd")),
                        new SqlParameter("@brand", brand),
                        new SqlParameter("@status", (salesRep == "不指定" ? "未處理" : "已指派")),
                    }
                );

                DataTable dt = APCommonFun.GetSafeDataTable_MSSQL(
                    sql2,
                    new List<SqlParameter>
                    {
                                        new SqlParameter("@shopNo", shopNo)
                    }
                );
                string carModel = string.Empty;
                string price = string.Empty;
                string displacement = string.Empty;
                string fuelType = string.Empty;
                string driveMode = string.Empty;
                string milage = string.Empty;
                string yearOfManufacture = string.Empty;
                string monthOfManufacture = string.Empty;
                string color = string.Empty;
                string outEquip = string.Empty;
                string feature = string.Empty;
                string area = string.Empty;
                string shopNo2 = string.Empty;

                if (dt.Rows.Count > 0)
                {
                    carModel = dt.Rows[0]["carModel"].ToString();
                    price = dt.Rows[0]["price"].ToString();
                    displacement = dt.Rows[0]["displacement"].ToString();
                    fuelType = dt.Rows[0]["fuelType"].ToString();
                    driveMode = dt.Rows[0]["driveMode"].ToString();
                    milage = dt.Rows[0]["milage"].ToString();
                    yearOfManufacture = dt.Rows[0]["yearOfManufacture"].ToString();
                    monthOfManufacture = dt.Rows[0]["monthOfManufacture"].ToString();
                    color = dt.Rows[0]["outerColor"].ToString();
                    outEquip = dt.Rows[0]["outEquip"].ToString();
                    feature = dt.Rows[0]["feature"].ToString();
                    area = dt.Rows[0]["area"].ToString();
                    shopNo2 = dt.Rows[0]["ShopNo"].ToString();
                }


                string innerMailIp = ConfigurationManager.AppSettings["mailServerIp"]; ;
                string innerMailPort = ConfigurationManager.AppSettings["mailServerPort"]; ;

                #region 寄信給-據點管理人員

                using (MailMessage mail = new MailMessage())
                {
                    string urlBase = string.Empty;
                    if (brand == "NISSAN")
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
                    mail.Body += "已收到會員的預約賞車資訊<br><br>車輛編號：" + shopNo2 + "<br>車款：" + visitCarType + "<br>車型：" + carModel + "<br>售價：" + Convert.ToInt32(price).ToString("###,###") + "<br>排氣量：" + Convert.ToInt32(displacement).ToString("###,###") + "<br>燃料：" + fuelType + "<br>驅動方式：" + driveMode + "<br>里程數：" + Convert.ToInt32(milage).ToString("###,###") + "<br>出廠年月：西元" + yearOfManufacture + "年" + monthOfManufacture + "月<br>車色：" + color + "<br>內外裝規格：" + outEquip + "<br>安全重點功能：" + feature + "<br>地區：" + area + "<br>中古車營業據點：" + dealerName + "<br>中古車銷售顧問：" + (string.IsNullOrEmpty(salesRep) ? "無" : salesRep) + "<br>方便連絡時段：" + visitTime.ToString("yyyy/MM/dd") + " " + period + "<br>其他需求：" + others + "<br><br>提醒，若無指定中古車銷售顧問，<br>請進入後台系統指派。<br><br>請點選單一後台網址登入查看<br>連結：" + urlBase + "backend/ <br><br>此為系統自動發送的郵件，請勿回覆";

                    //寄信給該據點的據點管理人員
                    var mailList = new Services.EmailService().GetUserMail("據點管理人員", brand, dealerName);
                    foreach (var eMail in mailList)
                    {
                        mail.To.Add(new MailAddress(eMail));
                        mail.IsBodyHtml = true;
                        mail.Subject = "預約賞車進單通知";

                        using (SmtpClient smtp = new SmtpClient(innerMailIp))
                        {
                            smtp.Port = Convert.ToInt32(innerMailPort);
                            smtp.Send(mail);
                        }
                    }
                }

                #endregion

                if (!string.IsNullOrEmpty(salesRep))
                {
                    #region 寄信給-中古車銷售顧問
                    
                    using (MailMessage mail = new MailMessage())
                    {
                        string urlBase = string.Empty;
                        if (brand == "NISSAN")
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
                        mail.Body += "已收到會員的預約賞車資訊<br><br>車輛編號：" + shopNo + "<br>車款：" + visitCarType + "<br>車型：" + carModel + "<br>售價：" + Convert.ToInt32(price).ToString("###,###") + "<br>排氣量：" + Convert.ToInt32(displacement).ToString("###,###") + "<br>燃料：" + fuelType + "<br>驅動方式：" + driveMode + "<br>里程數：" + Convert.ToInt32(milage).ToString("###,###") + "<br>出廠年月：西元" + yearOfManufacture + "年" + monthOfManufacture + "月<br>車色：" + color + "<br>內外裝規格：" + outEquip + "<br>安全重點功能：" + feature + "<br>地區：" + area + "<br>中古車營業據點：" + dealerName + "<br>中古車銷售顧問：" + (string.IsNullOrEmpty(salesRep) ? "無" : salesRep) + "<br>方便連絡時段：" + visitTime.ToString("yyyy/MM/dd") + " " + period + "<br>其他需求：" + others + "<br><br>提醒，請盡快與會員進行聯繫，<br>並回報給所屬中古車據點管理員。 <br><br>此為系統自動發送的郵件，請勿回覆";


                        string sql4 = "SELECT * FROM [CarShop].[dbo].[DealerPersons] where name =@salesRep   ";
                        DataTable dt2 = APCommonFun.GetSafeDataTable_MSSQL(
                            sql4,
                            new List<SqlParameter>
                            {
                                        new SqlParameter("@salesRep", salesRep)
                            }
                        );
                        if (dt2.Rows.Count > 0)
                        {
                            mail.To.Add(new MailAddress(dt2.Rows[0]["Email"].ToString()));

                            mail.IsBodyHtml = true;
                            mail.Subject = "預約賞車進單通知";

                            using (SmtpClient smtp = new SmtpClient(innerMailIp))
                            {
                                smtp.Port = Convert.ToInt32(innerMailPort);
                                smtp.Send(mail);
                            }
                        }

                    }

                    #endregion
                }

                #region 寄信給-會員
                
                using (MailMessage mail = new MailMessage())
                {
                    string urlBase = string.Empty;
                    if (brand == "NISSAN")
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


                    mail.Body += "已收到您預約賞車的資料填寫。<br><br>以下為您選擇預約賞車的車輛，將會由中古車銷售顧問與您聯繫。<br>車款：" + visitCarType + "<br>車型：" + carModel + "<br>售價：" + Convert.ToInt32(price).ToString("###,###") + "<br>排氣量：" + Convert.ToInt32(displacement).ToString("###,###") + "<br>燃料：" + fuelType + "<br>驅動方式：" + driveMode + "<br>里程數：" + Convert.ToInt32(milage).ToString("###,###") + "<br>出廠年月：西元" + yearOfManufacture + "年" + monthOfManufacture + "月<br>車色：" + color + "<br>內外裝規格：" + outEquip + "<br>安全重點功能：" + feature + "<br>地區：" + area + "<br>中古車營業據點：" + dealerName + "<br>中古車銷售顧問：" + (string.IsNullOrEmpty(salesRep) ? "無" : salesRep) + "<br>方便連絡時段：" + visitTime.ToString("yyyy/MM/dd") + " " + period + "<br>其他需求：" + others + "<br><br>歡迎造訪" + (brand == "NISSAN" ? "NISSA" : "INFINITI") + "中古車網站。<br>網址：" + urlBase + "<br><br>";


                    mail.To.Add(new MailAddress(email));
                    mail.IsBodyHtml = true;
                    mail.Subject = "預約賞車通知";

                    using (SmtpClient smtp = new SmtpClient(innerMailIp))
                    {
                        smtp.Port = Convert.ToInt32(innerMailPort);
                        smtp.Send(mail);
                    }
                }

                #endregion

                string smsPassword = ConfigurationManager.AppSettings["smsPassword"];
                string smsAccount = ConfigurationManager.AppSettings["smsAccount"];
                string smsMobile = string.Empty;
                if (brand == "NISSAN")
                {
                    smsMobile = ConfigurationManager.AppSettings["smsMobileNissan"];
                }
                else
                {
                    smsMobile = ConfigurationManager.AppSettings["smsMobileInfiniti"];
                }

                string smsApi = ConfigurationManager.AppSettings["smsApi"];
                string postData = "account=" + smsAccount + "&password=" + smsPassword + "& from_addr_type=0&from_addr=" + smsMobile + "&to_addr_type=0&to_addr=" + mobile + "&msg_expire_time=0&msg_type=0&msg=";

                if (brand == "NISSAN")
                {
                    postData += HttpUtility.UrlEncode("[NISSAN中古車會員通知]親愛的" + name + "您好，你已完成預約賞車，請查看Email確認詳細資訊。", Encoding.GetEncoding("big5"));
                }
                else
                {
                    postData += HttpUtility.UrlEncode("[INFINITI中古車會員通知]親愛的" + name + "您好，你已完成預約賞車，請查看Email確認詳細資訊。", Encoding.GetEncoding("big5"));
                }

                string result = APCommonFun.HttpWebPost(smsApi, postData, "UTF-8");

                var oneid = new Services.OneidService();
                var member = oneid.GetMemberByMobile(RequestToken, mobile);
                if (member.StatusCode == 200)
                {
                    oneid.UpdateMemberLog(RequestToken, member.Data.seq, "預約賞車", brand);
                }


                return ReturnOK();
            }
            catch (Exception ex)
            {
                APCommonFun.Error("[VisitOrderController]99：" + ex.ToString());
                return ReturnException();
            }
        }

    }
}
