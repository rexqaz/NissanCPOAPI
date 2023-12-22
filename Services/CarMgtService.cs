using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Web;
using WebApplication.Models;

namespace WebApplication.Services
{
    public class CarMgtService
    {
        private string _InnerMailIp = ConfigurationManager.AppSettings["mailServerIp"];
        private string _InnerMailPort = ConfigurationManager.AppSettings["mailServerPort"];

        public CarMgtService()
        {

        }


        public void MailSend_01(Shops shop, string mailAddress)
        {
            using (MailMessage mail = new MailMessage())
            {
                string urlBase = string.Empty;
                if (shop.brand == "NISSAN")
                {
                    mail.From = new MailAddress("nissanCPO@yulon-nissan.com.tw");
                    mail.Body = "據點管理員您好<br>已收到您申請車輛上架案件， 請靜待經銷商管理員審核。<br><br>";
                }
                else
                {
                    mail.From = new MailAddress("infinitiCPO@infiniti-taiwan.com");
                    mail.Body = "據點管理員您好<br>已收到您申請車輛上架案件， 請靜待經銷商管理員審核。<br><br>";
                }

                mail.Body += "車輛編號: " + shop.ShopNo.ToString() + "<br>";
                mail.Body += "地區: " + shop.area + "<br>";
                mail.Body += "經銷商: " + shop.dealer + "<br>";
                mail.Body += "中古車營業據點: " + shop.stronghold + "<br>";
                mail.Body += "車款: " + shop.carType + "<br>";
                mail.Body += "車型: " + shop.carModel + "<br>";
                mail.Body += "售價: " + Convert.ToInt32(shop.price).ToString("###,###") + "元<br>";
                mail.Body += "排氣量: " + Convert.ToInt32(shop.displacement).ToString("###,###") + "cc<br>";
                mail.Body += "燃料: " + shop.fuelType + "<br>";
                mail.Body += "驅動方式: " + shop.driveMode + "<br>";
                mail.Body += "里程數: " + Convert.ToInt32(shop.milage).ToString("###,###") + "公里<br>";
                mail.Body += "出廠年月: 西元" + shop.yearOfManufacture + "年" + shop.monthOfManufacture + "月<br>";
                mail.Body += "車色: " + shop.outerColor + "<br>";
                mail.Body += "內外裝規格: " + shop.outEquip + "<br>";
                mail.Body += "安全重點功能: " + shop.feature + "<br><br><br>";
                mail.Body += "請點選單一後台網址登入查看<br>連結：" + ConfigurationManager.AppSettings["nissanPath"] + "backend/";

                mail.To.Add(new MailAddress(mailAddress));
                mail.IsBodyHtml = true;
                mail.Subject = "車輛申請上架通知";

                using (SmtpClient smtp = new SmtpClient(_InnerMailIp))
                {
                    smtp.Port = Convert.ToInt32(_InnerMailPort);
                    smtp.Send(mail);
                }
            }
        }


        /// <summary>
        /// email 通知經銷商管理員
        /// </summary>
        public void MailSend_02(Shops shop, string mailAddress)
        {
            using (MailMessage mail = new MailMessage())
            {
                string urlBase = string.Empty;
                if (shop.brand == "NISSAN")
                {
                    mail.From = new MailAddress("nissanCPO@yulon-nissan.com.tw");
                    mail.Body = "經銷商管理員您好<br>已收到所屬據點的車輛上架申請， 請進行審核。<br><br>";
                }
                else
                {
                    mail.From = new MailAddress("infinitiCPO@infiniti-taiwan.com");
                    mail.Body = "經銷商管理員您好<br>已收到所屬據點的車輛上架申請， 請進行審核。<br><br>";
                }

                mail.Body += "車輛編號: " + shop.ShopNo.ToString() + "<br>";
                mail.Body += "地區: " + shop.area + "<br>";
                mail.Body += "經銷商: " + shop.dealer + "<br>";
                mail.Body += "中古車營業據點: " + shop.stronghold + "<br>";
                mail.Body += "車款: " + shop.carType + "<br>";
                mail.Body += "車型: " + shop.carModel + "<br>";
                mail.Body += "售價: " + Convert.ToInt32(shop.price).ToString("###,###") + "元<br>";
                mail.Body += "排氣量: " + Convert.ToInt32(shop.displacement).ToString("###,###") + "cc<br>";
                mail.Body += "燃料: " + shop.fuelType + "<br>";
                mail.Body += "驅動方式: " + shop.driveMode + "<br>";
                mail.Body += "里程數: " + Convert.ToInt32(shop.milage).ToString("###,###") + "公里<br>";
                mail.Body += "出廠年月: 西元" + shop.yearOfManufacture + "年" + shop.monthOfManufacture + "月<br>";
                mail.Body += "車色: " + shop.outerColor + "<br>";
                mail.Body += "內外裝規格: " + shop.outEquip + "<br>";
                mail.Body += "安全重點功能: " + shop.feature + "<br><br><br>";
                mail.Body += "請點選單一後台網址登入查看<br>連結：" + ConfigurationManager.AppSettings["nissanPath"] + "backend/";

                mail.To.Add(new MailAddress(mailAddress));
                mail.IsBodyHtml = true;
                mail.Subject = "車輛申請上架通知";

                using (SmtpClient smtp = new SmtpClient(_InnerMailIp))
                {
                    smtp.Port = Convert.ToInt32(_InnerMailPort);
                    smtp.Send(mail);
                }
            }
        }


        /// <summary>
        /// email 通知品牌管理員
        /// </summary>
        public void MailSend_03(Shops shop, string mailAddress)
        {
            using (MailMessage mail = new MailMessage())
            {
                string urlBase = string.Empty;
                if (shop.brand == "NISSAN")
                {
                    mail.From = new MailAddress("nissanCPO@yulon-nissan.com.tw");
                    mail.Body = "品牌管理員您好<br>目前已有車輛上架案件已通過。<br><br>";
                }
                else
                {
                    mail.From = new MailAddress("infinitiCPO@infiniti-taiwan.com");
                    mail.Body = "品牌管理員您好<br>目前已有車輛上架案件已通過。<br><br>";
                }

                mail.Body += "車輛編號: " + shop.ShopNo.ToString() + "<br>";
                mail.Body += "地區: " + shop.area + "<br>";
                mail.Body += "經銷商: " + shop.dealer + "<br>";
                mail.Body += "中古車營業據點: " + shop.stronghold + "<br>";
                mail.Body += "車款: " + shop.carType + "<br>";
                mail.Body += "車型: " + shop.carModel + "<br>";
                mail.Body += "售價: " + Convert.ToInt32(shop.price).ToString("###,###") + "元<br>";
                mail.Body += "排氣量: " + Convert.ToInt32(shop.displacement).ToString("###,###") + "cc<br>";
                mail.Body += "燃料: " + shop.fuelType + "<br>";
                mail.Body += "驅動方式: " + shop.driveMode + "<br>";
                mail.Body += "里程數: " + Convert.ToInt32(shop.milage).ToString("###,###") + "公里<br>";
                mail.Body += "出廠年月: 西元" + shop.yearOfManufacture + "年" + shop.monthOfManufacture + "月<br>";
                mail.Body += "車色: " + shop.outerColor + "<br>";
                mail.Body += "內外裝規格: " + shop.outEquip + "<br>";
                mail.Body += "安全重點功能: " + shop.feature + "<br><br><br>";
                mail.Body += "請點選單一後台網址登入查看<br>連結：" + ConfigurationManager.AppSettings["nissanPath"] + "backend/";

                mail.To.Add(new MailAddress(mailAddress));
                mail.IsBodyHtml = true;
                mail.Subject = "車輛申請上架通過通知";

                using (SmtpClient smtp = new SmtpClient(_InnerMailIp))
                {
                    smtp.Port = Convert.ToInt32(_InnerMailPort);
                    smtp.Send(mail);
                }
            }
        }


        /// <summary>
        /// 通知據點管理人員-上架通過
        /// </summary>
        public void MailSend_04(Shops shop, string mailAddress)
        {
            using (MailMessage mail = new MailMessage())
            {
                string urlBase = string.Empty;
                if (shop.brand == "NISSAN")
                {
                    mail.From = new MailAddress("nissanCPO@yulon-nissan.com.tw");
                    mail.Body = "據點管理員您好<br>您的車輛上架案件已通過。<br><br>";
                }
                else
                {
                    mail.From = new MailAddress("infinitiCPO@infiniti-taiwan.com");
                    mail.Body = "據點管理員您好<br>您的車輛上架案件已通過。<br><br>";
                }

                mail.Body += "車輛編號: " + shop.ShopNo.ToString() + "<br>";
                mail.Body += "地區: " + shop.area + "<br>";
                mail.Body += "經銷商: " + shop.dealer + "<br>";
                mail.Body += "中古車營業據點: " + shop.stronghold + "<br>";
                mail.Body += "車款: " + shop.carType + "<br>";
                mail.Body += "車型: " + shop.carModel + "<br>";
                mail.Body += "售價: " + Convert.ToInt32(shop.price).ToString("###,###") + "元<br>";
                mail.Body += "排氣量: " + Convert.ToInt32(shop.displacement).ToString("###,###") + "cc<br>";
                mail.Body += "燃料: " + shop.fuelType + "<br>";
                mail.Body += "驅動方式: " + shop.driveMode + "<br>";
                mail.Body += "里程數: " + Convert.ToInt32(shop.milage).ToString("###,###") + "公里<br>";
                mail.Body += "出廠年月: 西元" + shop.yearOfManufacture + "年" + shop.monthOfManufacture + "月<br>";
                mail.Body += "車色: " + shop.outerColor + "<br>";
                mail.Body += "內外裝規格: " + shop.outEquip + "<br>";
                mail.Body += "安全重點功能: " + shop.feature + "<br><br><br>";
                mail.Body += "請點選單一後台網址登入查看<br>連結：" + ConfigurationManager.AppSettings["nissanPath"] + "backend/";

                mail.To.Add(new MailAddress(mailAddress));
                mail.IsBodyHtml = true;
                mail.Subject = "車輛申請上架通過通知";

                using (SmtpClient smtp = new SmtpClient(_InnerMailIp))
                {
                    smtp.Port = Convert.ToInt32(_InnerMailPort);
                    smtp.Send(mail);
                }
            }
        }


        /// <summary>
        /// 通知據點管理人員-上架退回
        /// </summary>
        public void MailSend_05(Shops shop, string mailAddress)
        {
            using (MailMessage mail = new MailMessage())
            {
                string urlBase = string.Empty;
                if (shop.brand == "NISSAN")
                {
                    mail.From = new MailAddress("nissanCPO@yulon-nissan.com.tw");
                    mail.Body = "據點管理員您好<br>您的申請車輛上架案件已被退回。<br><br>";
                }
                else
                {
                    mail.From = new MailAddress("infinitiCPO@infiniti-taiwan.com");
                    mail.Body = "據點管理員您好<br>您的申請車輛上架案件已被退回。<br><br>";
                }

                mail.Body += "車輛編號: " + shop.ShopNo.ToString() + "<br>";
                mail.Body += "地區: " + shop.area + "<br>";
                mail.Body += "經銷商: " + shop.dealer + "<br>";
                mail.Body += "中古車營業據點: " + shop.stronghold + "<br>";
                mail.Body += "車款: " + shop.carType + "<br>";
                mail.Body += "車型: " + shop.carModel + "<br>";
                mail.Body += "售價: " + Convert.ToInt32(shop.price).ToString("###,###") + "元<br>";
                mail.Body += "排氣量: " + Convert.ToInt32(shop.displacement).ToString("###,###") + "cc<br>";
                mail.Body += "燃料: " + shop.fuelType + "<br>";
                mail.Body += "驅動方式: " + shop.driveMode + "<br>";
                mail.Body += "里程數: " + Convert.ToInt32(shop.milage).ToString("###,###") + "公里<br>";
                mail.Body += "出廠年月: 西元" + shop.yearOfManufacture + "年" + shop.monthOfManufacture + "月<br>";
                mail.Body += "車色: " + shop.outerColor + "<br>";
                mail.Body += "內外裝規格: " + shop.outEquip + "<br>";
                mail.Body += "安全重點功能: " + shop.feature + "<br><br><br>";
                mail.Body += "請點選單一後台網址登入查看<br>連結：" + ConfigurationManager.AppSettings["nissanPath"] + "backend/";

                mail.To.Add(new MailAddress(mailAddress));
                mail.IsBodyHtml = true;
                mail.Subject = "車輛申請上架被退回通知";

                using (SmtpClient smtp = new SmtpClient(_InnerMailIp))
                {
                    smtp.Port = Convert.ToInt32(_InnerMailPort);
                    smtp.Send(mail);
                }
            }
        }


        /// <summary>
        /// 通知據點管理人員-成交下架通過
        /// </summary>

        public void MailSend_06(Shops shop, string mailAddress)
        {
            using (MailMessage mail = new MailMessage())
            {
                string urlBase = string.Empty;
                if (shop.brand == "NISSAN")
                {
                    mail.From = new MailAddress("nissanCPO@yulon-nissan.com.tw");
                    mail.Body = "據點管理員您好<br>您的車輛成交下架案件已通過。。<br><br>";
                }
                else
                {
                    mail.From = new MailAddress("infinitiCPO@infiniti-taiwan.com");
                    mail.Body = "據點管理員您好<br>您的車輛成交下架案件已通過。。<br><br>";
                }

                mail.Body += "車輛編號: " + shop.ShopNo.ToString() + "<br>";
                mail.Body += "地區: " + shop.area + "<br>";
                mail.Body += "經銷商: " + shop.dealer + "<br>";
                mail.Body += "中古車營業據點: " + shop.stronghold + "<br>";
                mail.Body += "車款: " + shop.carType + "<br>";
                mail.Body += "車型: " + shop.carModel + "<br>";
                mail.Body += "售價: " + Convert.ToInt32(shop.price).ToString("###,###") + "元<br>";
                mail.Body += "排氣量: " + Convert.ToInt32(shop.displacement).ToString("###,###") + "cc<br>";
                mail.Body += "燃料: " + shop.fuelType + "<br>";
                mail.Body += "驅動方式: " + shop.driveMode + "<br>";
                mail.Body += "里程數: " + Convert.ToInt32(shop.milage).ToString("###,###") + "公里<br>";
                mail.Body += "出廠年月: 西元" + shop.yearOfManufacture + "年" + shop.monthOfManufacture + "月<br>";
                mail.Body += "車色: " + shop.outerColor + "<br>";
                mail.Body += "內外裝規格: " + shop.outEquip + "<br>";
                mail.Body += "安全重點功能: " + shop.feature + "<br><br><br>";
                mail.Body += "請點選單一後台網址登入查看<br>連結：" + ConfigurationManager.AppSettings["nissanPath"] + "backend/";

                mail.To.Add(new MailAddress(mailAddress));
                mail.IsBodyHtml = true;
                mail.Subject = "車輛申請成交下架已通過通知";

                using (SmtpClient smtp = new SmtpClient(_InnerMailIp))
                {
                    smtp.Port = Convert.ToInt32(_InnerMailPort);
                    smtp.Send(mail);
                }
            }
        }


        /// <summary>
        /// 通知據點管理人員-非成交下架通過
        /// </summary>
        /// <param name="shop"></param>
        /// <param name="mailAddress"></param>
        public void MailSend_07(Shops shop, string mailAddress)
        {
            using (MailMessage mail = new MailMessage())
            {
                string urlBase = string.Empty;
                if (shop.brand == "NISSAN")
                {
                    mail.From = new MailAddress("nissanCPO@yulon-nissan.com.tw");
                    mail.Body = "據點管理員您好<br>您的車輛非成交下架案件已通過。<br><br>";
                }
                else
                {
                    mail.From = new MailAddress("infinitiCPO@infiniti-taiwan.com");
                    mail.Body = "據點管理員您好<br>您的車輛非成交下架案件已通過。<br><br>";
                }

                mail.Body += "車輛編號: " + shop.ShopNo.ToString() + "<br>";
                mail.Body += "地區: " + shop.area + "<br>";
                mail.Body += "經銷商: " + shop.dealer + "<br>";
                mail.Body += "中古車營業據點: " + shop.stronghold + "<br>";
                mail.Body += "車款: " + shop.carType + "<br>";
                mail.Body += "車型: " + shop.carModel + "<br>";
                mail.Body += "售價: " + Convert.ToInt32(shop.price).ToString("###,###") + "元<br>";
                mail.Body += "排氣量: " + Convert.ToInt32(shop.displacement).ToString("###,###") + "cc<br>";
                mail.Body += "燃料: " + shop.fuelType + "<br>";
                mail.Body += "驅動方式: " + shop.driveMode + "<br>";
                mail.Body += "里程數: " + Convert.ToInt32(shop.milage).ToString("###,###") + "公里<br>";
                mail.Body += "出廠年月: 西元" + shop.yearOfManufacture + "年" + shop.monthOfManufacture + "月<br>";
                mail.Body += "車色: " + shop.outerColor + "<br>";
                mail.Body += "內外裝規格: " + shop.outEquip + "<br>";
                mail.Body += "安全重點功能: " + shop.feature + "<br><br><br>";
                mail.Body += "請點選單一後台網址登入查看<br>連結：" + ConfigurationManager.AppSettings["nissanPath"] + "backend/";

                mail.To.Add(new MailAddress(mailAddress));
                mail.IsBodyHtml = true;
                mail.Subject = "車輛申請非成交下架通過通知";

                using (SmtpClient smtp = new SmtpClient(_InnerMailIp))
                {
                    smtp.Port = Convert.ToInt32(_InnerMailPort);
                    smtp.Send(mail);
                }
            }
        }



        /// <summary>
        /// 通知據點管理人員-下架退回
        /// </summary>
        public void MailSend_08(Shops shop, string mailAddress)
        {
            using (MailMessage mail = new MailMessage())
            {
                string urlBase = string.Empty;
                if (shop.brand == "NISSAN")
                {
                    mail.From = new MailAddress("nissanCPO@yulon-nissan.com.tw");
                    mail.Body = "據點管理員您好<br>您的車輛成交下架案件已被退回。<br><br>";
                }
                else
                {
                    mail.From = new MailAddress("infinitiCPO@infiniti-taiwan.com");
                    mail.Body = "據點管理員您好<br>您的車輛成交下架案件已被退回。<br><br>";
                }

                mail.Body += "車輛編號: " + shop.ShopNo.ToString() + "<br>";
                mail.Body += "地區: " + shop.area + "<br>";
                mail.Body += "經銷商: " + shop.dealer + "<br>";
                mail.Body += "中古車營業據點: " + shop.stronghold + "<br>";
                mail.Body += "車款: " + shop.carType + "<br>";
                mail.Body += "車型: " + shop.carModel + "<br>";
                mail.Body += "售價: " + Convert.ToInt32(shop.price).ToString("###,###") + "元<br>";
                mail.Body += "排氣量: " + Convert.ToInt32(shop.displacement).ToString("###,###") + "cc<br>";
                mail.Body += "燃料: " + shop.fuelType + "<br>";
                mail.Body += "驅動方式: " + shop.driveMode + "<br>";
                mail.Body += "里程數: " + Convert.ToInt32(shop.milage).ToString("###,###") + "公里<br>";
                mail.Body += "出廠年月: 西元" + shop.yearOfManufacture + "年" + shop.monthOfManufacture + "月<br>";
                mail.Body += "車色: " + shop.outerColor + "<br>";
                mail.Body += "內外裝規格: " + shop.outEquip + "<br>";
                mail.Body += "安全重點功能: " + shop.feature + "<br><br><br>";
                mail.Body += "請點選單一後台網址登入查看<br>連結：" + ConfigurationManager.AppSettings["nissanPath"] + "backend/";

                mail.To.Add(new MailAddress(mailAddress));
                mail.IsBodyHtml = true;
                mail.Subject = "車輛申請下架退回通知";

                using (SmtpClient smtp = new SmtpClient(_InnerMailIp))
                {
                    smtp.Port = Convert.ToInt32(_InnerMailPort);
                    smtp.Send(mail);
                }
            }
        }



        /// <summary>
        /// 通知據點管理人員-成交下架
        /// </summary>
        public void MailSend_09(Shops shop, string mailAddress)
        {
            using (MailMessage mail = new MailMessage())
            {
                string urlBase = string.Empty;
                if (shop.brand == "NISSAN")
                {
                    mail.From = new MailAddress("nissanCPO@yulon-nissan.com.tw");
                    mail.Body = "據點管理員您好<br>已收到您申請成交下架案件， 請靜待經銷商管理員審核。<br><br>";
                }
                else
                {
                    mail.From = new MailAddress("infinitiCPO@infiniti-taiwan.com");
                    mail.Body = "據點管理員您好<br>已收到您申請成交下架案件， 請靜待經銷商管理員審核。<br><br>";
                }

                mail.Body += "車輛編號: " + shop.ShopNo.ToString() + "<br>";
                mail.Body += "地區: " + shop.area + "<br>";
                mail.Body += "經銷商: " + shop.dealer + "<br>";
                mail.Body += "中古車營業據點: " + shop.stronghold + "<br>";
                mail.Body += "車款: " + shop.carType + "<br>";
                mail.Body += "車型: " + shop.carModel + "<br>";
                mail.Body += "售價: " + Convert.ToInt32(shop.price).ToString("###,###") + "元<br>";
                mail.Body += "排氣量: " + Convert.ToInt32(shop.displacement).ToString("###,###") + "cc<br>";
                mail.Body += "燃料: " + shop.fuelType + "<br>";
                mail.Body += "驅動方式: " + shop.driveMode + "<br>";
                mail.Body += "里程數: " + Convert.ToInt32(shop.milage).ToString("###,###") + "公里<br>";
                mail.Body += "出廠年月: 西元" + shop.yearOfManufacture + "年" + shop.monthOfManufacture + "月<br>";
                mail.Body += "車色: " + shop.outerColor + "<br>";
                mail.Body += "內外裝規格: " + shop.outEquip + "<br>";
                mail.Body += "安全重點功能: " + shop.feature + "<br><br><br>";
                mail.Body += "請點選單一後台網址登入查看<br>連結：" + ConfigurationManager.AppSettings["nissanPath"] + "backend/";

                mail.To.Add(new MailAddress(mailAddress));
                mail.IsBodyHtml = true;
                mail.Subject = "車輛申請成交下架通知";

                using (SmtpClient smtp = new SmtpClient(_InnerMailIp))
                {
                    smtp.Port = Convert.ToInt32(_InnerMailPort);
                    smtp.Send(mail);
                }
            }
        }


        /// <summary>
        /// 通知經銷商管理員-成交下架
        /// </summary>
        public void MailSend_10(Shops shop, string mailAddress)
        {
            using (MailMessage mail = new MailMessage())
            {
                string urlBase = string.Empty;
                if (shop.brand == "NISSAN")
                {
                    mail.From = new MailAddress("nissanCPO@yulon-nissan.com.tw");
                    mail.Body = "經銷商管理員您好<br>已收到所屬據點的車輛成交下架申請， 請進行審核。<br><br>";
                }
                else
                {
                    mail.From = new MailAddress("infinitiCPO@infiniti-taiwan.com");
                    mail.Body = "經銷商管理員您好<br>已收到所屬據點的車輛成交下架申請， 請進行審核。<br><br>";
                }

                mail.Body += "車輛編號: " + shop.ShopNo.ToString() + "<br>";
                mail.Body += "地區: " + shop.area + "<br>";
                mail.Body += "經銷商: " + shop.dealer + "<br>";
                mail.Body += "中古車營業據點: " + shop.stronghold + "<br>";
                mail.Body += "車款: " + shop.carType + "<br>";
                mail.Body += "車型: " + shop.carModel + "<br>";
                mail.Body += "售價: " + Convert.ToInt32(shop.price).ToString("###,###") + "元<br>";
                mail.Body += "排氣量: " + Convert.ToInt32(shop.displacement).ToString("###,###") + "cc<br>";
                mail.Body += "燃料: " + shop.fuelType + "<br>";
                mail.Body += "驅動方式: " + shop.driveMode + "<br>";
                mail.Body += "里程數: " + Convert.ToInt32(shop.milage).ToString("###,###") + "公里<br>";
                mail.Body += "出廠年月: 西元" + shop.yearOfManufacture + "年" + shop.monthOfManufacture + "月<br>";
                mail.Body += "車色: " + shop.outerColor + "<br>";
                mail.Body += "內外裝規格: " + shop.outEquip + "<br>";
                mail.Body += "安全重點功能: " + shop.feature + "<br><br><br>";
                mail.Body += "請點選單一後台網址登入查看<br>連結：" + ConfigurationManager.AppSettings["nissanPath"] + "backend/";

                mail.To.Add(new MailAddress(mailAddress));
                mail.IsBodyHtml = true;
                mail.Subject = "車輛申請成交下架通知";

                using (SmtpClient smtp = new SmtpClient(_InnerMailIp))
                {
                    smtp.Port = Convert.ToInt32(_InnerMailPort);
                    smtp.Send(mail);
                }
            }
        }


        /// <summary>
        /// 通知據點管理人員-非成交下架
        /// </summary>
        public void MailSend_11(Shops shop, string mailAddress)
        {
            using (MailMessage mail = new MailMessage())
            {
                string urlBase = string.Empty;
                if (shop.brand == "NISSAN")
                {
                    mail.From = new MailAddress("nissanCPO@yulon-nissan.com.tw");
                    mail.Body = "據點管理員您好<br>已收到您申請非成交下架案件， 請靜待經銷商管理員審核。<br><br>";
                }
                else
                {
                    mail.From = new MailAddress("infinitiCPO@infiniti-taiwan.com");
                    mail.Body = "據點管理員您好<br>已收到您申請非成交下架案件， 請靜待經銷商管理員審核。<br><br>";
                }

                mail.Body += "車輛編號: " + shop.ShopNo.ToString() + "<br>";
                mail.Body += "地區: " + shop.area + "<br>";
                mail.Body += "經銷商: " + shop.dealer + "<br>";
                mail.Body += "中古車營業據點: " + shop.stronghold + "<br>";
                mail.Body += "車款: " + shop.carType + "<br>";
                mail.Body += "車型: " + shop.carModel + "<br>";
                mail.Body += "售價: " + Convert.ToInt32(shop.price).ToString("###,###") + "元<br>";
                mail.Body += "排氣量: " + Convert.ToInt32(shop.displacement).ToString("###,###") + "cc<br>";
                mail.Body += "燃料: " + shop.fuelType + "<br>";
                mail.Body += "驅動方式: " + shop.driveMode + "<br>";
                mail.Body += "里程數: " + Convert.ToInt32(shop.milage).ToString("###,###") + "公里<br>";
                mail.Body += "出廠年月: 西元" + shop.yearOfManufacture + "年" + shop.monthOfManufacture + "月<br>";
                mail.Body += "車色: " + shop.outerColor + "<br>";
                mail.Body += "內外裝規格: " + shop.outEquip + "<br>";
                mail.Body += "安全重點功能: " + shop.feature + "<br><br><br>";
                mail.Body += "請點選單一後台網址登入查看<br>連結：" + ConfigurationManager.AppSettings["nissanPath"] + "backend/";

                mail.To.Add(new MailAddress(mailAddress));
                mail.IsBodyHtml = true;
                mail.Subject = "車輛申請非成交下架通知";

                using (SmtpClient smtp = new SmtpClient(_InnerMailIp))
                {
                    smtp.Port = Convert.ToInt32(_InnerMailPort);
                    smtp.Send(mail);
                }
            }
        }



        /// <summary>
        /// 通知經銷商管理員-非成交下架
        /// </summary>
        public void MailSend_12(Shops shop, string mailAddress)
        {
            using (MailMessage mail = new MailMessage())
            {
                string urlBase = string.Empty;
                if (shop.brand == "NISSAN")
                {
                    mail.From = new MailAddress("nissanCPO@yulon-nissan.com.tw");
                    mail.Body = "經銷商管理員您好<br>已收到所屬據點的車輛非成交下架申請， 請進行審核。<br><br>";
                }
                else
                {
                    mail.From = new MailAddress("infinitiCPO@infiniti-taiwan.com");
                    mail.Body = "經銷商管理員您好<br>已收到所屬據點的車輛非成交下架申請， 請進行審核。<br><br>";
                }

                mail.Body += "車輛編號: " + shop.ShopNo.ToString() + "<br>";
                mail.Body += "地區: " + shop.area + "<br>";
                mail.Body += "經銷商: " + shop.dealer + "<br>";
                mail.Body += "中古車營業據點: " + shop.stronghold + "<br>";
                mail.Body += "車款: " + shop.carType + "<br>";
                mail.Body += "車型: " + shop.carModel + "<br>";
                mail.Body += "售價: " + Convert.ToInt32(shop.price).ToString("###,###") + "元<br>";
                mail.Body += "排氣量: " + Convert.ToInt32(shop.displacement).ToString("###,###") + "cc<br>";
                mail.Body += "燃料: " + shop.fuelType + "<br>";
                mail.Body += "驅動方式: " + shop.driveMode + "<br>";
                mail.Body += "里程數: " + Convert.ToInt32(shop.milage).ToString("###,###") + "公里<br>";
                mail.Body += "出廠年月: 西元" + shop.yearOfManufacture + "年" + shop.monthOfManufacture + "月<br>";
                mail.Body += "車色: " + shop.outerColor + "<br>";
                mail.Body += "內外裝規格: " + shop.outEquip + "<br>";
                mail.Body += "安全重點功能: " + shop.feature + "<br><br><br>";
                mail.Body += "請點選單一後台網址登入查看<br>連結：" + ConfigurationManager.AppSettings["nissanPath"] + "backend/";

                mail.To.Add(new MailAddress(mailAddress));
                mail.IsBodyHtml = true;
                mail.Subject = "車輛申請非成交下架通知";

                using (SmtpClient smtp = new SmtpClient(_InnerMailIp))
                {
                    smtp.Port = Convert.ToInt32(_InnerMailPort);
                    smtp.Send(mail);
                }
            }
        }


        /// <summary>
        /// 上架申請
        /// </summary>
        public void MailSend_13(Shops shop, string mailAddress)
        {
            try
            {
                using (MailMessage mail = new MailMessage())
                {
                    string urlBase = string.Empty;
                    if (shop.brand == "NISSAN")
                    {
                        mail.From = new MailAddress("nissanCPO@yulon-nissan.com.tw");
                        mail.Body = "據點管理員您好<br>已收到您申請車輛上架案件， 請靜待經銷商管理員審核。<br><br>";
                    }
                    else
                    {
                        mail.From = new MailAddress("infinitiCPO@infiniti-taiwan.com");
                        mail.Body = "據點管理員您好<br>已收到您申請車輛上架案件， 請靜待經銷商管理員審核。<br><br>";
                    }

                    mail.Body += "車輛編號: " + shop.ShopNo.ToString() + "<br>";
                    mail.Body += "地區: " + shop.area + "<br>";
                    mail.Body += "經銷商: " + shop.dealer + "<br>";
                    mail.Body += "中古車營業據點: " + shop.stronghold + "<br>";
                    mail.Body += "車款: " + shop.carType + "<br>";
                    mail.Body += "車型: " + shop.carModel + "<br>";
                    mail.Body += "售價: " + Convert.ToInt32(shop.price).ToString("###,###") + "元<br>";
                    mail.Body += "排氣量: " + Convert.ToInt32(shop.displacement).ToString("###,###") + "cc<br>";
                    mail.Body += "燃料: " + shop.fuelType + "<br>";
                    mail.Body += "驅動方式: " + shop.driveMode + "<br>";
                    mail.Body += "里程數: " + Convert.ToInt32(shop.milage).ToString("###,###") + "公里<br>";
                    mail.Body += "出廠年月: 西元" + shop.yearOfManufacture + "年" + shop.monthOfManufacture + "月<br>";
                    mail.Body += "車色: " + shop.outerColor + "<br>";
                    mail.Body += "內外裝規格: " + shop.outEquip + "<br>";
                    mail.Body += "安全重點功能: " + shop.feature + "<br><br><br>";
                    mail.Body += "請點選單一後台網址登入查看<br>連結：" + ConfigurationManager.AppSettings["nissanPath"] + "backend/";

                    mail.To.Add(new MailAddress(mailAddress));
                    mail.IsBodyHtml = true;
                    mail.Subject = "車輛申請上架通知";

                    using (SmtpClient smtp = new SmtpClient(_InnerMailIp))
                    {
                        smtp.Port = Convert.ToInt32(_InnerMailPort);
                        smtp.Send(mail);
                    }
                }
            }
            catch (Exception ex)
            { }
        }


        /// <summary>
        /// 上架申請-通知經銷商管理員
        /// </summary>
        public void MailSend_14(Shops shop, string mailAddress)
        {
            using (MailMessage mail = new MailMessage())
            {
                string urlBase = string.Empty;
                if (shop.brand == "NISSAN")
                {
                    mail.From = new MailAddress("nissanCPO@yulon-nissan.com.tw");
                    mail.Body = "經銷商管理員您好<br>已收到所屬據點的車輛上架申請， 請進行審核。<br><br>";
                }
                else
                {
                    mail.From = new MailAddress("infinitiCPO@infiniti-taiwan.com");
                    mail.Body = "經銷商管理員您好<br>已收到所屬據點的車輛上架申請， 請進行審核。<br><br>";
                }

                mail.Body += "車輛編號: " + shop.ShopNo.ToString() + "<br>";
                mail.Body += "地區: " + shop.area + "<br>";
                mail.Body += "經銷商: " + shop.dealer + "<br>";
                mail.Body += "中古車營業據點: " + shop.stronghold + "<br>";
                mail.Body += "車款: " + shop.carType + "<br>";
                mail.Body += "車型: " + shop.carModel + "<br>";
                mail.Body += "售價: " + Convert.ToInt32(shop.price).ToString("###,###") + "元<br>";
                mail.Body += "排氣量: " + Convert.ToInt32(shop.displacement).ToString("###,###") + "cc<br>";
                mail.Body += "燃料: " + shop.fuelType + "<br>";
                mail.Body += "驅動方式: " + shop.driveMode + "<br>";
                mail.Body += "里程數: " + Convert.ToInt32(shop.milage).ToString("###,###") + "公里<br>";
                mail.Body += "出廠年月: 西元" + shop.yearOfManufacture + "年" + shop.monthOfManufacture + "月<br>";
                mail.Body += "車色: " + shop.outerColor + "<br>";
                mail.Body += "內外裝規格: " + shop.outEquip + "<br>";
                mail.Body += "安全重點功能: " + shop.feature + "<br><br><br>";
                mail.Body += "請點選單一後台網址登入查看<br>連結：" + ConfigurationManager.AppSettings["nissanPath"] + "backend/";

                mail.To.Add(new MailAddress(mailAddress));
                mail.IsBodyHtml = true;
                mail.Subject = "車輛申請上架通知";

                using (SmtpClient smtp = new SmtpClient(_InnerMailIp))
                {
                    smtp.Port = Convert.ToInt32(_InnerMailPort);
                    smtp.Send(mail);
                }
            }
        }


        /// <summary>
        /// 上架通過-通知經銷商管理員
        /// </summary>
        public void MailSend_15(Shops shop, string mailAddress)
        {
            using (MailMessage mail = new MailMessage())
            {
                string urlBase = string.Empty;
                if (shop.brand == "NISSAN")
                {
                    mail.From = new MailAddress("nissanCPO@yulon-nissan.com.tw");
                    mail.Body = "品牌管理員您好<br>目前已有車輛上架案件已通過。<br><br>";
                }
                else
                {
                    mail.From = new MailAddress("infinitiCPO@infiniti-taiwan.com");
                    mail.Body = "品牌管理員您好<br>目前已有車輛上架案件已通過。<br><br>";
                }

                mail.Body += "車輛編號: " + shop.ShopNo.ToString() + "<br>";
                mail.Body += "地區: " + shop.area + "<br>";
                mail.Body += "經銷商: " + shop.dealer + "<br>";
                mail.Body += "中古車營業據點: " + shop.stronghold + "<br>";
                mail.Body += "車款: " + shop.carType + "<br>";
                mail.Body += "車型: " + shop.carModel + "<br>";
                mail.Body += "售價: " + Convert.ToInt32(shop.price).ToString("###,###") + "元<br>";
                mail.Body += "排氣量: " + Convert.ToInt32(shop.displacement).ToString("###,###") + "cc<br>";
                mail.Body += "燃料: " + shop.fuelType + "<br>";
                mail.Body += "驅動方式: " + shop.driveMode + "<br>";
                mail.Body += "里程數: " + Convert.ToInt32(shop.milage).ToString("###,###") + "公里<br>";
                mail.Body += "出廠年月: 西元" + shop.yearOfManufacture + "年" + shop.monthOfManufacture + "月<br>";
                mail.Body += "車色: " + shop.outerColor + "<br>";
                mail.Body += "內外裝規格: " + shop.outEquip + "<br>";
                mail.Body += "安全重點功能: " + shop.feature + "<br><br><br>";
                mail.Body += "請點選單一後台網址登入查看<br>連結：" + ConfigurationManager.AppSettings["nissanPath"] + "backend/";

                mail.To.Add(new MailAddress(mailAddress));
                mail.IsBodyHtml = true;
                mail.Subject = "車輛申請上架通過通知";

                using (SmtpClient smtp = new SmtpClient(_InnerMailIp))
                {
                    smtp.Port = Convert.ToInt32(_InnerMailPort);
                    smtp.Send(mail);
                }
            }
        }


        /// <summary>
        /// 上架通過-通知據點管理人員
        /// </summary>
        public void MailSend_16(Shops shop, string mailAddress)
        {
            using (MailMessage mail = new MailMessage())
            {
                string urlBase = string.Empty;
                if (shop.brand == "NISSAN")
                {
                    mail.From = new MailAddress("nissanCPO@yulon-nissan.com.tw");
                    mail.Body = "據點管理員您好<br>您的車輛上架案件已通過。<br><br>";
                }
                else
                {
                    mail.From = new MailAddress("infinitiCPO@infiniti-taiwan.com");
                    mail.Body = "據點管理員您好<br>您的車輛上架案件已通過。<br><br>";
                }

                mail.Body += "車輛編號: " + shop.ShopNo.ToString() + "<br>";
                mail.Body += "地區: " + shop.area + "<br>";
                mail.Body += "經銷商: " + shop.dealer + "<br>";
                mail.Body += "中古車營業據點: " + shop.stronghold + "<br>";
                mail.Body += "車款: " + shop.carType + "<br>";
                mail.Body += "車型: " + shop.carModel + "<br>";
                mail.Body += "售價: " + Convert.ToInt32(shop.price).ToString("###,###") + "元<br>";
                mail.Body += "排氣量: " + Convert.ToInt32(shop.displacement).ToString("###,###") + "cc<br>";
                mail.Body += "燃料: " + shop.fuelType + "<br>";
                mail.Body += "驅動方式: " + shop.driveMode + "<br>";
                mail.Body += "里程數: " + Convert.ToInt32(shop.milage).ToString("###,###") + "公里<br>";
                mail.Body += "出廠年月: 西元" + shop.yearOfManufacture + "年" + shop.monthOfManufacture + "月<br>";
                mail.Body += "車色: " + shop.outerColor + "<br>";
                mail.Body += "內外裝規格: " + shop.outEquip + "<br>";
                mail.Body += "安全重點功能: " + shop.feature + "<br><br><br>";
                mail.Body += "請點選單一後台網址登入查看<br>連結：" + ConfigurationManager.AppSettings["nissanPath"] + "backend/";

                mail.To.Add(new MailAddress(mailAddress));
                mail.IsBodyHtml = true;
                mail.Subject = "車輛申請上架通過通知";

                using (SmtpClient smtp = new SmtpClient(_InnerMailIp))
                {
                    smtp.Port = Convert.ToInt32(_InnerMailPort);
                    smtp.Send(mail);
                }
            }
        }


        /// <summary>
        /// 上架退回
        /// </summary>
        public void MailSend_17(Shops shop, string mailAddress)
        {
            using (MailMessage mail = new MailMessage())
            {
                string urlBase = string.Empty;
                if (shop.brand == "NISSAN")
                {
                    mail.From = new MailAddress("nissanCPO@yulon-nissan.com.tw");
                    mail.Body = "據點管理員您好<br>您的申請車輛上架案件已被退回。<br><br>";
                }
                else
                {
                    mail.From = new MailAddress("infinitiCPO@infiniti-taiwan.com");
                    mail.Body = "據點管理員您好<br>您的申請車輛上架案件已被退回。<br><br>";
                }

                mail.Body += "車輛編號: " + shop.ShopNo.ToString() + "<br>";
                mail.Body += "地區: " + shop.area + "<br>";
                mail.Body += "經銷商: " + shop.dealer + "<br>";
                mail.Body += "中古車營業據點: " + shop.stronghold + "<br>";
                mail.Body += "車款: " + shop.carType + "<br>";
                mail.Body += "車型: " + shop.carModel + "<br>";
                mail.Body += "售價: " + Convert.ToInt32(shop.price).ToString("###,###") + "元<br>";
                mail.Body += "排氣量: " + Convert.ToInt32(shop.displacement).ToString("###,###") + "cc<br>";
                mail.Body += "燃料: " + shop.fuelType + "<br>";
                mail.Body += "驅動方式: " + shop.driveMode + "<br>";
                mail.Body += "里程數: " + Convert.ToInt32(shop.milage).ToString("###,###") + "公里<br>";
                mail.Body += "出廠年月: 西元" + shop.yearOfManufacture + "年" + shop.monthOfManufacture + "月<br>";
                mail.Body += "車色: " + shop.outerColor + "<br>";
                mail.Body += "內外裝規格: " + shop.outEquip + "<br>";
                mail.Body += "安全重點功能: " + shop.feature + "<br><br><br>";
                mail.Body += "請點選單一後台網址登入查看<br>連結：" + ConfigurationManager.AppSettings["nissanPath"] + "backend/";

                mail.To.Add(new MailAddress(mailAddress));
                mail.IsBodyHtml = true;
                mail.Subject = "車輛申請上架被退回通知";

                using (SmtpClient smtp = new SmtpClient(_InnerMailIp))
                {
                    smtp.Port = Convert.ToInt32(_InnerMailPort);
                    smtp.Send(mail);
                }
            }
        }


        /// <summary>
        /// 成交下架通過
        /// </summary>
        public void MailSend_18(Shops shop, string mailAddress)
        {
            using (MailMessage mail = new MailMessage())
            {
                string urlBase = string.Empty;
                if (shop.brand == "NISSAN")
                {
                    mail.From = new MailAddress("nissanCPO@yulon-nissan.com.tw");
                    mail.Body = "據點管理員您好<br>您的車輛成交下架案件已通過。。<br><br>";
                }
                else
                {
                    mail.From = new MailAddress("infinitiCPO@infiniti-taiwan.com");
                    mail.Body = "據點管理員您好<br>您的車輛成交下架案件已通過。。<br><br>";
                }

                mail.Body += "車輛編號: " + shop.ShopNo.ToString() + "<br>";
                mail.Body += "地區: " + shop.area + "<br>";
                mail.Body += "經銷商: " + shop.dealer + "<br>";
                mail.Body += "中古車營業據點: " + shop.stronghold + "<br>";
                mail.Body += "車款: " + shop.carType + "<br>";
                mail.Body += "車型: " + shop.carModel + "<br>";
                mail.Body += "售價: " + Convert.ToInt32(shop.price).ToString("###,###") + "元<br>";
                mail.Body += "排氣量: " + Convert.ToInt32(shop.displacement).ToString("###,###") + "cc<br>";
                mail.Body += "燃料: " + shop.fuelType + "<br>";
                mail.Body += "驅動方式: " + shop.driveMode + "<br>";
                mail.Body += "里程數: " + Convert.ToInt32(shop.milage).ToString("###,###") + "公里<br>";
                mail.Body += "出廠年月: 西元" + shop.yearOfManufacture + "年" + shop.monthOfManufacture + "月<br>";
                mail.Body += "車色: " + shop.outerColor + "<br>";
                mail.Body += "內外裝規格: " + shop.outEquip + "<br>";
                mail.Body += "安全重點功能: " + shop.feature + "<br><br><br>";
                mail.Body += "請點選單一後台網址登入查看<br>連結：" + ConfigurationManager.AppSettings["nissanPath"] + "backend/";

                mail.To.Add(new MailAddress(mailAddress));
                mail.IsBodyHtml = true;
                mail.Subject = "車輛申請成交下架已通過通知";

                using (SmtpClient smtp = new SmtpClient(_InnerMailIp))
                {
                    smtp.Port = Convert.ToInt32(_InnerMailPort);
                    smtp.Send(mail);
                }
            }
        }



        /// <summary>
        /// 非成交下架通過
        /// </summary>
        public void MailSend_19(Shops shop, string mailAddress)
        {
            using (MailMessage mail = new MailMessage())
            {
                string urlBase = string.Empty;
                if (shop.brand == "NISSAN")
                {
                    mail.From = new MailAddress("nissanCPO@yulon-nissan.com.tw");
                    mail.Body = "據點管理員您好<br>您的車輛非成交下架案件已通過。<br><br>";
                }
                else
                {
                    mail.From = new MailAddress("infinitiCPO@infiniti-taiwan.com");
                    mail.Body = "據點管理員您好<br>您的車輛非成交下架案件已通過。<br><br>";
                }

                mail.Body += "車輛編號: " + shop.ShopNo.ToString() + "<br>";
                mail.Body += "地區: " + shop.area + "<br>";
                mail.Body += "經銷商: " + shop.dealer + "<br>";
                mail.Body += "中古車營業據點: " + shop.stronghold + "<br>";
                mail.Body += "車款: " + shop.carType + "<br>";
                mail.Body += "車型: " + shop.carModel + "<br>";
                mail.Body += "售價: " + Convert.ToInt32(shop.price).ToString("###,###") + "元<br>";
                mail.Body += "排氣量: " + Convert.ToInt32(shop.displacement).ToString("###,###") + "cc<br>";
                mail.Body += "燃料: " + shop.fuelType + "<br>";
                mail.Body += "驅動方式: " + shop.driveMode + "<br>";
                mail.Body += "里程數: " + Convert.ToInt32(shop.milage).ToString("###,###") + "公里<br>";
                mail.Body += "出廠年月: 西元" + shop.yearOfManufacture + "年" + shop.monthOfManufacture + "月<br>";
                mail.Body += "車色: " + shop.outerColor + "<br>";
                mail.Body += "內外裝規格: " + shop.outEquip + "<br>";
                mail.Body += "安全重點功能: " + shop.feature + "<br><br><br>";
                mail.Body += "請點選單一後台網址登入查看<br>連結：" + ConfigurationManager.AppSettings["nissanPath"] + "backend/";

                mail.To.Add(new MailAddress(mailAddress));
                mail.IsBodyHtml = true;
                mail.Subject = "車輛申請非成交下架通過通知";

                using (SmtpClient smtp = new SmtpClient(_InnerMailIp))
                {
                    smtp.Port = Convert.ToInt32(_InnerMailPort);
                    smtp.Send(mail);
                }
            }
        }


        /// <summary>
        /// 下架退回
        /// </summary>
        public void MailSend_20(Shops shop, string mailAddress)
        {
            using (MailMessage mail = new MailMessage())
            {
                string urlBase = string.Empty;
                if (shop.brand == "NISSAN")
                {
                    mail.From = new MailAddress("nissanCPO@yulon-nissan.com.tw");
                    mail.Body = "據點管理員您好<br>您的車輛成交下架案件已被退回。<br><br>";
                }
                else
                {
                    mail.From = new MailAddress("infinitiCPO@infiniti-taiwan.com");
                    mail.Body = "據點管理員您好<br>您的車輛成交下架案件已被退回。<br><br>";
                }

                mail.Body += "車輛編號: " + shop.ShopNo.ToString() + "<br>";
                mail.Body += "地區: " + shop.area + "<br>";
                mail.Body += "經銷商: " + shop.dealer + "<br>";
                mail.Body += "中古車營業據點: " + shop.stronghold + "<br>";
                mail.Body += "車款: " + shop.carType + "<br>";
                mail.Body += "車型: " + shop.carModel + "<br>";
                mail.Body += "售價: " + Convert.ToInt32(shop.price).ToString("###,###") + "元<br>";
                mail.Body += "排氣量: " + Convert.ToInt32(shop.displacement).ToString("###,###") + "cc<br>";
                mail.Body += "燃料: " + shop.fuelType + "<br>";
                mail.Body += "驅動方式: " + shop.driveMode + "<br>";
                mail.Body += "里程數: " + Convert.ToInt32(shop.milage).ToString("###,###") + "公里<br>";
                mail.Body += "出廠年月: 西元" + shop.yearOfManufacture + "年" + shop.monthOfManufacture + "月<br>";
                mail.Body += "車色: " + shop.outerColor + "<br>";
                mail.Body += "內外裝規格: " + shop.outEquip + "<br>";
                mail.Body += "安全重點功能: " + shop.feature + "<br><br><br>";
                mail.Body += "請點選單一後台網址登入查看<br>連結：" + ConfigurationManager.AppSettings["nissanPath"] + "backend/";

                mail.To.Add(new MailAddress(mailAddress));
                mail.IsBodyHtml = true;
                mail.Subject = "車輛申請下架退回通知";

                using (SmtpClient smtp = new SmtpClient(_InnerMailIp))
                {
                    smtp.Port = Convert.ToInt32(_InnerMailPort);
                    smtp.Send(mail);
                }
            }
        }


        /// <summary>
        /// 成交下架
        /// </summary>
        public void MailSend_21(Shops shop, string mailAddress)
        {
            using (MailMessage mail = new MailMessage())
            {
                string urlBase = string.Empty;
                if (shop.brand == "NISSAN")
                {
                    mail.From = new MailAddress("nissanCPO@yulon-nissan.com.tw");
                    mail.Body = "據點管理員您好<br>已收到您申請成交下架案件， 請靜待經銷商管理員審核。<br><br>";
                }
                else
                {
                    mail.From = new MailAddress("infinitiCPO@infiniti-taiwan.com");
                    mail.Body = "據點管理員您好<br>已收到您申請成交下架案件， 請靜待經銷商管理員審核。<br><br>";
                }

                mail.Body += "車輛編號: " + shop.ShopNo.ToString() + "<br>";
                mail.Body += "地區: " + shop.area + "<br>";
                mail.Body += "經銷商: " + shop.dealer + "<br>";
                mail.Body += "中古車營業據點: " + shop.stronghold + "<br>";
                mail.Body += "車款: " + shop.carType + "<br>";
                mail.Body += "車型: " + shop.carModel + "<br>";
                mail.Body += "售價: " + Convert.ToInt32(shop.price).ToString("###,###") + "元<br>";
                mail.Body += "排氣量: " + Convert.ToInt32(shop.displacement).ToString("###,###") + "cc<br>";
                mail.Body += "燃料: " + shop.fuelType + "<br>";
                mail.Body += "驅動方式: " + shop.driveMode + "<br>";
                mail.Body += "里程數: " + Convert.ToInt32(shop.milage).ToString("###,###") + "公里<br>";
                mail.Body += "出廠年月: 西元" + shop.yearOfManufacture + "年" + shop.monthOfManufacture + "月<br>";
                mail.Body += "車色: " + shop.outerColor + "<br>";
                mail.Body += "內外裝規格: " + shop.outEquip + "<br>";
                mail.Body += "安全重點功能: " + shop.feature + "<br><br><br>";
                mail.Body += "請點選單一後台網址登入查看<br>連結：" + ConfigurationManager.AppSettings["nissanPath"] + "backend/";

                mail.To.Add(new MailAddress(mailAddress));
                mail.IsBodyHtml = true;
                mail.Subject = "車輛申請成交下架通知";

                using (SmtpClient smtp = new SmtpClient(_InnerMailIp))
                {
                    smtp.Port = Convert.ToInt32(_InnerMailPort);
                    smtp.Send(mail);
                }
            }
        }


        /// <summary>
        /// 成交下架
        /// </summary>
        public void MailSend_22(Shops shop, string mailAddress)
        {
            using (MailMessage mail = new MailMessage())
            {
                string urlBase = string.Empty;
                if (shop.brand == "NISSAN")
                {
                    mail.From = new MailAddress("nissanCPO@yulon-nissan.com.tw");
                    mail.Body = "經銷商管理員您好<br>已收到所屬據點的車輛成交下架申請， 請進行審核。<br><br>";
                }
                else
                {
                    mail.From = new MailAddress("infinitiCPO@infiniti-taiwan.com");
                    mail.Body = "經銷商管理員您好<br>已收到所屬據點的車輛成交下架申請， 請進行審核。<br><br>";
                }

                mail.Body += "車輛編號: " + shop.ShopNo.ToString() + "<br>";
                mail.Body += "地區: " + shop.area + "<br>";
                mail.Body += "經銷商: " + shop.dealer + "<br>";
                mail.Body += "中古車營業據點: " + shop.stronghold + "<br>";
                mail.Body += "車款: " + shop.carType + "<br>";
                mail.Body += "車型: " + shop.carModel + "<br>";
                mail.Body += "售價: " + Convert.ToInt32(shop.price).ToString("###,###") + "元<br>";
                mail.Body += "排氣量: " + Convert.ToInt32(shop.displacement).ToString("###,###") + "cc<br>";
                mail.Body += "燃料: " + shop.fuelType + "<br>";
                mail.Body += "驅動方式: " + shop.driveMode + "<br>";
                mail.Body += "里程數: " + Convert.ToInt32(shop.milage).ToString("###,###") + "公里<br>";
                mail.Body += "出廠年月: 西元" + shop.yearOfManufacture + "年" + shop.monthOfManufacture + "月<br>";
                mail.Body += "車色: " + shop.outerColor + "<br>";
                mail.Body += "內外裝規格: " + shop.outEquip + "<br>";
                mail.Body += "安全重點功能: " + shop.feature + "<br><br><br>";
                mail.Body += "請點選單一後台網址登入查看<br>連結：" + ConfigurationManager.AppSettings["nissanPath"] + "backend/";

                mail.To.Add(new MailAddress(mailAddress));
                mail.IsBodyHtml = true;
                mail.Subject = "車輛申請成交下架通知";

                using (SmtpClient smtp = new SmtpClient(_InnerMailIp))
                {
                    smtp.Port = Convert.ToInt32(_InnerMailPort);
                    smtp.Send(mail);
                }
            }
        }


        /// <summary>
        /// 成交下架
        /// </summary>
        public void MailSend_23(Shops shop, string mailAddress)
        {
            using (MailMessage mail = new MailMessage())
            {
                string urlBase = string.Empty;
                if (shop.brand == "NISSAN")
                {
                    mail.From = new MailAddress("nissanCPO@yulon-nissan.com.tw");
                    mail.Body = "據點管理員您好<br>已收到您申請非成交下架案件， 請靜待經銷商管理員審核。<br><br>";
                }
                else
                {
                    mail.From = new MailAddress("infinitiCPO@infiniti-taiwan.com");
                    mail.Body = "據點管理員您好<br>已收到您申請非成交下架案件， 請靜待經銷商管理員審核。<br><br>";
                }

                mail.Body += "車輛編號: " + shop.ShopNo.ToString() + "<br>";
                mail.Body += "地區: " + shop.area + "<br>";
                mail.Body += "經銷商: " + shop.dealer + "<br>";
                mail.Body += "中古車營業據點: " + shop.stronghold + "<br>";
                mail.Body += "車款: " + shop.carType + "<br>";
                mail.Body += "車型: " + shop.carModel + "<br>";
                mail.Body += "售價: " + Convert.ToInt32(shop.price).ToString("###,###") + "元<br>";
                mail.Body += "排氣量: " + Convert.ToInt32(shop.displacement).ToString("###,###") + "cc<br>";
                mail.Body += "燃料: " + shop.fuelType + "<br>";
                mail.Body += "驅動方式: " + shop.driveMode + "<br>";
                mail.Body += "里程數: " + Convert.ToInt32(shop.milage).ToString("###,###") + "公里<br>";
                mail.Body += "出廠年月: 西元" + shop.yearOfManufacture + "年" + shop.monthOfManufacture + "月<br>";
                mail.Body += "車色: " + shop.outerColor + "<br>";
                mail.Body += "內外裝規格: " + shop.outEquip + "<br>";
                mail.Body += "安全重點功能: " + shop.feature + "<br><br><br>";
                mail.Body += "請點選單一後台網址登入查看<br>連結：" + ConfigurationManager.AppSettings["nissanPath"] + "backend/";

                mail.To.Add(new MailAddress(mailAddress));
                mail.IsBodyHtml = true;
                mail.Subject = "車輛申請非成交下架通知";

                using (SmtpClient smtp = new SmtpClient(_InnerMailIp))
                {
                    smtp.Port = Convert.ToInt32(_InnerMailPort);
                    smtp.Send(mail);
                }
            }
        }


        /// <summary>
        /// 成交下架
        /// </summary>
        public void MailSend_24(Shops shop, string mailAddress)
        {
            using (MailMessage mail = new MailMessage())
            {
                string urlBase = string.Empty;
                if (shop.brand == "NISSAN")
                {
                    mail.From = new MailAddress("nissanCPO@yulon-nissan.com.tw");
                    mail.Body = "經銷商管理員您好<br>已收到所屬據點的車輛非成交下架申請， 請進行審核。<br><br>";
                }
                else
                {
                    mail.From = new MailAddress("infinitiCPO@infiniti-taiwan.com");
                    mail.Body = "經銷商管理員您好<br>已收到所屬據點的車輛非成交下架申請， 請進行審核。<br><br>";
                }

                mail.Body += "車輛編號: " + shop.ShopNo.ToString() + "<br>";
                mail.Body += "地區: " + shop.area + "<br>";
                mail.Body += "經銷商: " + shop.dealer + "<br>";
                mail.Body += "中古車營業據點: " + shop.stronghold + "<br>";
                mail.Body += "車款: " + shop.carType + "<br>";
                mail.Body += "車型: " + shop.carModel + "<br>";
                mail.Body += "售價: " + Convert.ToInt32(shop.price).ToString("###,###") + "元<br>";
                mail.Body += "排氣量: " + Convert.ToInt32(shop.displacement).ToString("###,###") + "cc<br>";
                mail.Body += "燃料: " + shop.fuelType + "<br>";
                mail.Body += "驅動方式: " + shop.driveMode + "<br>";
                mail.Body += "里程數: " + Convert.ToInt32(shop.milage).ToString("###,###") + "公里<br>";
                mail.Body += "出廠年月: 西元" + shop.yearOfManufacture + "年" + shop.monthOfManufacture + "月<br>";
                mail.Body += "車色: " + shop.outerColor + "<br>";
                mail.Body += "內外裝規格: " + shop.outEquip + "<br>";
                mail.Body += "安全重點功能: " + shop.feature + "<br><br><br>";
                mail.Body += "請點選單一後台網址登入查看<br>連結：" + ConfigurationManager.AppSettings["nissanPath"] + "backend/";

                mail.To.Add(new MailAddress(mailAddress));
                mail.IsBodyHtml = true;
                mail.Subject = "車輛申請非成交下架通知";

                using (SmtpClient smtp = new SmtpClient(_InnerMailIp))
                {
                    smtp.Port = Convert.ToInt32(_InnerMailPort);
                    smtp.Send(mail);
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public void MailSend_25(Shops shop, string mailAddress, string[] yearArray, string[] priceArray, string[] milageArray,string user_id, int counts)
        {
            using (MailMessage mail = new MailMessage())
            {
                string urlBase = string.Empty;
                if (shop.brand == "NISSAN")
                {
                    urlBase = ConfigurationManager.AppSettings["nissanPath"];
                    mail.From = new MailAddress("nissanCPO@yulon-nissan.com.tw");
                    mail.Body = "NISSAN 會員您好<br><br>";
                }
                else
                {
                    urlBase = ConfigurationManager.AppSettings["infinitiPath"];
                    mail.From = new MailAddress("infinitiCPO@infiniti-taiwan.com");
                    mail.Body = "INFINITI 會員您好<br><br>";
                }

                mail.Body += "您所設定的條件<br>價格：" + priceArray[0].ToString() + " - " + priceArray[1].ToString() + "<br>里程數：" + milageArray[0].ToString() + " - " + milageArray[1].ToString() + "<br>車廠年份：" + yearArray[0].ToString() + " - " + yearArray[1].ToString() + "<br>驅動方式：" + shop.driveMode + "<br>車種：" + shop.carType + "<br>車型：" + shop.carModel + "<br>顏色：" + shop.outerColor + "<br>車輛所在地：" + shop.area + "<br>服務據點：" + shop.dealer + "<br><br>有" + counts.ToString() + "台車輛符合條件<br><br>";

                mail.Body += "車輛: 網址" + urlBase + "search/" + shop.seq.ToString() + "<br>";
                mail.Body += "<br><br><a href='" + urlBase + "backend/api/CancelCarSubscription/" + user_id + "'>取消訂閱</a>";
                mail.To.Add(new MailAddress(mailAddress));
                mail.IsBodyHtml = true;
                mail.Subject = "訂閱車輛";

                using (SmtpClient smtp = new SmtpClient(_InnerMailIp))
                {
                    smtp.Port = Convert.ToInt32(_InnerMailPort);
                    smtp.Send(mail);
                }
            }
        }
    }
}