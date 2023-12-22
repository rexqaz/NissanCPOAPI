using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using WebApplication.Controllers;

namespace WebApplication.Services
{
    public class EmailService : IIdentityMessageService
    {
        public async Task SendAsync(IdentityMessage message)
        {
            MailMessage mail = new MailMessage();
            NetworkCredential cred = new NetworkCredential("rexqaz@gmail.com", "imcsie@38");
            //收件者
            mail.To.Add(message.Destination);
            mail.Subject = message.Subject;
            //寄件者
            mail.From = new System.Net.Mail.MailAddress("nissan@gmail.com");
            mail.IsBodyHtml = true;
            mail.Body = message.Body;
            //設定SMTP
            SmtpClient smtp = new SmtpClient("smtp.gmail.com");
            smtp.UseDefaultCredentials = false;
            smtp.EnableSsl = true;
            smtp.Credentials = cred;
            smtp.Port = 587;
            //送出Mail
            await smtp.SendMailAsync(mail);
        }


        /// <summary>
        /// 取得後台人員的Mail列表
        /// </summary>
        /// <param name="roleName">角色名 ex:據點管理人員</param>
        /// <param name="brand">品牌 ex:NISSAN</param>
        /// <param name="company">經銷商 ex:匯聯</param>
        /// <param name="department">據點 ex:博賢所</param>
        /// <returns></returns>
        public List<string> GetUserMail(string roleName, string brand, string company, string department)
        {
            var sql = @"select a.Email from [AspNetUsers] a 
                        left join [Profiles] b on a.Id = b.Id
                        left join [UserRoles] c on a.NameIdentifier = c.userName
                        left join [Roles] d on c.userRole = d.seq";

            var dicValue = new Dictionary<string, string>();
            var strWhere = "";

            if (string.IsNullOrWhiteSpace(roleName) == false)
            {
                dicValue.Add("@RoleName", roleName);
                strWhere += " and d.RoleName = @RoleName";
            }
            if (string.IsNullOrWhiteSpace(brand) == false)
            {
                dicValue.Add("@brand", brand);
                strWhere += " and b.brand = @brand";
            }
            if (string.IsNullOrWhiteSpace(company) == false)
            {
                dicValue.Add("@company", company);
                strWhere += " and b.Company = @company";
            }
            if (string.IsNullOrWhiteSpace(department) == false)
            {
                department = department.Split('-')[0];
                dicValue.Add("@department", department);
                strWhere += " and b.Department = @department";
            }

            if (dicValue.Count > 0)
            {
                sql += $" where 1=1 {strWhere}";
            }

            var mailList = new List<string>();
            var dt = APCommonFun.GetDataTable_MSSQL(sql, dicValue);
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    mailList.Add(dt.Rows[i]["Email"].ToString());                    
                }
            }

            return mailList;
        }

        /// <summary>
        /// 取得後台人員的Mail列表
        /// </summary>
        /// <param name="roleName">角色名 ex:據點管理人員</param>
        /// <param name="brand">品牌 ex:NISSAN</param>
        /// <param name="department">據點 ex:博賢所</param>
        /// <returns></returns>
        public List<string> GetUserMail(string roleName, string brand, string department)
        {
            var sql = @"select a.Email from [AspNetUsers] a 
                        left join [Profiles] b on a.Id = b.Id
                        left join [UserRoles] c on a.NameIdentifier = c.userName
                        left join [Roles] d on c.userRole = d.seq";

            var dicValue = new Dictionary<string, string>();
            var strWhere = "";

            if (string.IsNullOrWhiteSpace(roleName) == false)
            {
                dicValue.Add("@RoleName", roleName);
                strWhere += " and d.RoleName = @RoleName";
            }
            if (string.IsNullOrWhiteSpace(brand) == false)
            {
                dicValue.Add("@brand", brand);
                strWhere += " and b.brand = @brand";
            }
            if (string.IsNullOrWhiteSpace(department) == false)
            {
                department = department.Split('-')[0];
                dicValue.Add("@department", department);
                strWhere += " and b.Department = @department";
            }

            if (dicValue.Count > 0)
            {
                sql += $" where 1=1 {strWhere}";
            }

            var mailList = new List<string>();
            var dt = APCommonFun.GetDataTable_MSSQL(sql, dicValue);
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    mailList.Add(dt.Rows[i]["Email"].ToString());
                }
            }

            return mailList;
        }
    }
}