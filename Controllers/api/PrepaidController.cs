using Microsoft.Owin.BuilderProperties;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
using System.Web.Http;
using System.Web.Http.Cors;
using WebApplication.Extensions;
using WebApplication.Models;
using WebApplication.Services;
using WebApplication.ViewModels;

namespace WebApplication.Controllers.api
{
    [JwtAuthActionFilter]
    public class PrepaidController : BaseAPIController
    {
        /// <summary>
        /// 預付保留
        /// </summary>
        /// <param name="Data"></param>
        /// <returns></returns>
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public async Task<object> Post(Info_PrepaidModel Data)
        {
            try
            {
                CarShopEntities _db = new CarShopEntities();
                string InputIsok = "Y";
                string ReturnErr = "";
                string shopNo = string.Empty;             
                string contactPeriod = string.Empty;
                string others = string.Empty;
                string sales = string.Empty;

                if (Data.shopNo != null)
                {
                    shopNo = APCommonFun.CDBNulltrim(Data.shopNo);
                }

                if (Data.contactPeriod != null)
                {
                    contactPeriod = APCommonFun.CDBNulltrim(Data.contactPeriod);
                }

                if (Data.others != null)
                {
                    others = APCommonFun.CDBNulltrim(Data.others);
                }

                if (Data.salesRep != null)
                {
                    sales = APCommonFun.CDBNulltrim(Data.salesRep);
                }

                //第一步 : 先判斷有沒有必填未填寫，
                if (shopNo == "") //必填                       
                {
                    InputIsok = "N";
                    ReturnErr = "執行動作錯誤-shopNo 為必填欄位";
                }

                if (contactPeriod == "")
                {
                    InputIsok = "N";
                    ReturnErr = "執行動作錯誤-contactPeriod 為必填欄位";

                }

                //第二步 : 如果有必填未給值，回傳告知必填欄位要有資料
                if (InputIsok == "N")
                {
                    APCommonFun.Error("[PrepaidController]90-" + ReturnErr);
                    return ReturnError(ReturnErr);
                }

                string today = DateTime.Now.ToString("yyMMdd");
                string prepaidNo = string.Empty;
                string sql = string.Empty;
                string queryShop = "select * from Shops where shopNo = @ShopNo and status='上架中'";
                string queryPrepaidNo = "select top 1 prepaidNo from Prepaid where prepaidNo like @BrandPrefix order by prepaidNo desc";
                string queryUserHasPrepaid = "select * from Prepaid where user_id = @UserId and paidStatus = '已付訂金'";


                // 找車輛

                var shops = long.TryParse(shopNo, out long seq) ? _db.Shops.Where(x => x.seq == seq && x.status == "上架中").FirstOrDefault() : null;
                if (shops == null)
                {
                    return ReturnError("找不到相關車輛資訊");
                }
                string shopsShopNo = shops.ShopNo;

                // 判斷是否被預訂
                DateTime ext = DateTime.Now.AddMinutes(-10);
                DateTime payExt = DateTime.Now.AddDays(-3);
                if (_db.Prepaid.Where(x => x.shopNo == shopsShopNo && x.paidStatus == "已付訂金" && payExt < x.createTime).Any())
                {                   
                    return ReturnError("車輛已被預訂");
                }

                if (_db.Prepaid.Where(x => x.shopNo == shopsShopNo && ext < x.createTime).Any())
                {                    
                    return ReturnError("系統交易更新中，請稍候再試。");
                }

                string brand = shops.brand;
                string brandPrefix = 'C' + brand.Substring(0, 1) + today;

                // 找會員
                Request.Headers.TryGetValues("Authorization", out var headerToken);
                string token = headerToken != null ? headerToken.FirstOrDefault() : "";
                var apiResult = new OneidService().GetMember(token.Replace("Bearer ", ""));
                if (apiResult.StatusCode != (int)DataModels.EnumApiStatus.Ok)
                {
                    return ReturnError("Oneid-API找不到會員帳號");
                }

                var member = apiResult.Data;
                if (member == null)
                {                    
                    return ReturnError("找不到會員帳號");
                }              

                // 檢查是否只存在一筆有效預付保留
                DataTable dt = APCommonFun.GetSafeDataTable_MSSQL(
                    queryUserHasPrepaid,
                    new List<SqlParameter>
                    {
                        new SqlParameter("@UserId", member.seq)
                    }
                );
                if (dt.Rows.Count > 0)
                {                  
                    return ReturnError("每位會員只能保留一台車");
                }

                // 建立訂單number
                prepaidNo = APCommonFun.Transaction_PrepaidController_MSSQL(new PrepaidDataInfo
                {
                    PrepaidNo = "",
                    ShopNo = shopsShopNo,
                    UserId = member.seq,
                    Name = member.name,
                    Mobile = member.mobile,
                    Email = member.email,
                    Title = member.title,
                    Id = member.id,
                    Birthday = member.birthday,
                    Address = member.address,
                    ContactPeriod = contactPeriod,
                    Others = others,
                    Sales = sales,
                    Brand = shops.brand
                });

                //dt = APCommonFun.GetSafeDataTable_MSSQL(
                //    queryPrepaidNo,
                //    new List<SqlParameter>
                //    {
                //        new SqlParameter("@BrandPrefix", brandPrefix + "%")
                //    }
                //);
                //if (dt.Rows.Count > 0)
                //{
                //    prepaidNo = APCommonFun.CDBNulltrim(dt.Rows[0]["prepaidNo"].ToString());
                //}

                //if (string.IsNullOrEmpty(prepaidNo))
                //{
                //    prepaidNo = brandPrefix + "000001";
                //}
                //else
                //{
                //    prepaidNo = brandPrefix + (Convert.ToInt32(prepaidNo.Split(new string[] { brandPrefix }, StringSplitOptions.None)[1]) + 1).ToString().PadLeft(6, '0');
                //}

                JObject paidData = new JObject
                {
                    ["prepaidNo"] = prepaidNo,
                };

                //sql = "insert into Prepaid (prepaidNo, shopNo, user_id, name, mobile, email, title, id, birthday, address, contactPeriod, others, createTime, updateTime, paidStatus, contactStatus, sales, brand, isClose) values (@PrepaidNo, @ShopNo, @UserId, @Name, @Mobile, @Email, @Title, @Id, @Birthday, @Address, @ContactPeriod, @Others, getdate(), getdate(), N'未付款', N'未聯絡', @Sales, @brand, 'N' )";

                //APCommonFun.ExecSafeSqlCommand_MSSQL(
                //    sql,
                //    new List<SqlParameter>
                //    {
                //        new SqlParameter("@PrepaidNo", prepaidNo),
                //        new SqlParameter("@ShopNo", shopsShopNo),
                //        new SqlParameter("@UserId", member.seq),
                //        new SqlParameter("@Name", member.name),
                //        new SqlParameter("@Mobile", member.mobile),
                //        new SqlParameter("@Email", member.email),
                //        new SqlParameter("@Title", member.title),
                //        new SqlParameter("@Id", member.id),
                //        new SqlParameter("@Birthday", member.birthday),
                //        new SqlParameter("@Address", member.address),
                //        new SqlParameter("@ContactPeriod", contactPeriod),
                //        new SqlParameter("@Others", others),
                //        new SqlParameter("@Sales", sales),
                //        new SqlParameter("@Brand", shops.brand)
                //    }
                //);

                string sql5 = "select * from Shops where ShopNo = @ShopNo";
                DataTable dt2 = APCommonFun.GetSafeDataTable_MSSQL(
                    sql5,
                    new List<SqlParameter>
                    {
                    new SqlParameter("@ShopNo", shopsShopNo)
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
                string carType = string.Empty;
                string dealer = string.Empty;
                string stronghold = string.Empty;
                string salesRep = string.Empty;

                if (dt2.Rows.Count > 0)
                {
                    salesRep = dt2.Rows[0]["salesRep"].ToString();
                    carType = dt2.Rows[0]["carType"].ToString();
                    carModel = dt2.Rows[0]["carModel"].ToString();
                    price = dt2.Rows[0]["price"].ToString();
                    displacement = dt2.Rows[0]["displacement"].ToString();
                    fuelType = dt2.Rows[0]["fuelType"].ToString();
                    driveMode = dt2.Rows[0]["driveMode"].ToString();
                    milage = dt2.Rows[0]["milage"].ToString();
                    yearOfManufacture = dt2.Rows[0]["yearOfManufacture"].ToString();
                    monthOfManufacture = dt2.Rows[0]["monthOfManufacture"].ToString();
                    color = dt2.Rows[0]["outerColor"].ToString();
                    outEquip = dt2.Rows[0]["outEquip"].ToString();
                    feature = dt2.Rows[0]["feature"].ToString();
                    area = dt2.Rows[0]["area"].ToString();
                    dealer = dt2.Rows[0]["dealer"].ToString();
                    stronghold = dt2.Rows[0]["stronghold"].ToString();
                }

                new Services.OneidService().UpdateMemberLog(RequestToken, member.seq, "預付保留", brand);

                return ReturnOK(new { prepaidNo = prepaidNo });
            }
            catch (Exception ex)
            {
                APCommonFun.Error("[PrepaidController]99：" + ex.ToString());
                APCommonFun.Error("[PrepaidController]99：Json:" + JsonConvert.SerializeObject(Data));
                return ReturnException();
            }
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
            public prepaidData data { get; set; }
            public string mac { get; set; }
            public string ksn { get; set; }
        }
    }
}
