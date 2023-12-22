using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WebApplication.Models;
using WebApplication.Services;

namespace WebApplication.Controllers
{
    public class CarBuyController : Controller
    {
        private BrandService brandSrv = new BrandService();
        private RightService rightSrv = new RightService();
        CarShopEntities carShopEntities = new CarShopEntities();
        CarShopMemberEntities carShopMemberEntities = new CarShopMemberEntities();
        private const string _exportPath = "../XingUpdateFile/TemporaryFile/CarBuy/";

        // GET: Prepaid
        public ActionResult Index()
        {
            if (!rightSrv.checkRight("CarBuy", "Index", User.Identity.Name))
            {
                return RedirectToAction("Index", "Home");
            }


            if (!rightSrv.checkRight("CarBuy", "ExportCSV", User.Identity.Name))
            {
                TempData["ExportRight"] = "False";
            }
            else
            {
                TempData["ExportRight"] = "True";
            }

            if (!rightSrv.checkRight("CarBuy", "Update", User.Identity.Name))
            {
                TempData["UpdateRight"] = "False";
            }
            else
            {
                TempData["UpdateRight"] = "True";
            }

            var user = carShopEntities.AspNetUsers.Where(x => x.Email == User.Identity.Name).FirstOrDefault();
            string roleName = string.Empty;
            string company = string.Empty;
            string department = string.Empty;
            if (user != null)
            {
                int role = carShopEntities.UserRoles.Where(x => x.userName == user.NameIdentifier).Select(x => x.userRole).FirstOrDefault() ?? 0;
                roleName = carShopEntities.Roles.Where(x => x.seq == role).Select(x => x.RoleName).FirstOrDefault() ?? string.Empty;

                company = carShopEntities.Profiles.Where(x => x.Id == user.Id).Select(x => x.Company).FirstOrDefault() ?? string.Empty;
                department = carShopEntities.Profiles.Where(x => x.Id == user.Id).Select(x => x.Department).FirstOrDefault() ?? string.Empty;
            }

            string brand = brandSrv.getBrand(User.Identity.Name);
            List<Dealers> dealers = carShopEntities.Dealers.Where(x => x.brand == brand && x.type == "old").ToList(); ;
            if (roleName != "ISC最高權限管理人員" && roleName != "品牌管理人員")
            {
                switch (roleName)
                {
                    case "經銷商管理人員":
                        dealers = dealers.Where(x => x.dealerName == company).ToList();
                        break;
                    case "據點管理人員":
                        dealers = dealers.Where(x => x.dealerName == company && x.businessOffice == department).ToList();
                        break;
                    case "據點所屬業代":
                        dealers = dealers.Where(x => x.dealerName == company && x.businessOffice == department).ToList();
                        break;
                    default:
                        break;
                }
            }

            List<string> dealerOptions = new List<string>();
            dealerOptions = dealers.Select(x => x.dealerName).Distinct().ToList();

            TempData["dealerOptions"] = dealerOptions;
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> GetBuyData(FormCollection post)
        {
            var draw = Request.Form["draw"].FirstOrDefault();
            string start = Request.Form["start"];
            string length = Request.Form["length"];
            string keyword = post["Keyword"];
            string dateFilter = post["dateFilter"];
            string dealerFilter = post["dealerFilter"];
            string strongholdFilter = post["strongholdFilter"];
            string statusFilter = post["status"];
            string categoryFilter = post["category"];
            string cashFlowFilter = post["cashFlow"];
            string sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"];
            string sortColumnDirection = Request.Form["order[0][dir]"];
            string searchValue = Request.Form["search[value]"];
            int pageSize = Convert.ToInt32(length);
            int skip = Convert.ToInt32(start);
            int recordsTotal = 0;

            string brand = brandSrv.getBrand(User.Identity.Name);

            string sql = "SELECT N'預付保留' as 'category', seq, createTime, shopNo, name as 'member', mobile,	sales as 'assignedConsulant', serveSales as 'serveConsulant', contactStatus as 'status', paidStatus as 'cashFlow', updateTime FROM [Prepaid] where brand = @brand UNION ALL select N'預約賞車' as 'category', seq, createTime, shopNo, name as 'member', mobile, salesRep as 'assignedConsulant', assignedConsultant as 'serveConsulant', status, '---' as 'cashFlow', updateTime FROM [VisitOrders] where brand = @brand ";

            //DataTable dt = APCommonFun.GetDataTable_MSSQL(sql);
            DataTable dt = APCommonFun.GetSafeDataTable_MSSQL(
                sql,
                new List<SqlParameter>
                {
                    new SqlParameter("@brand", brand)
                }
            );

            List<CarBuyViewModel> carbuys = new List<CarBuyViewModel>();
            try
            {
                carbuys = (from DataRow dr in dt.Rows
                           select new CarBuyViewModel()
                           {
                               category = dr["category"].ToString(),
                               createTime = DateTime.Parse(dr["createTime"].ToString()),
                               updateTime = DateTime.Parse(dr["updateTime"].ToString()),
                               shopNo = dr["shopNo"].ToString(),
                               member = dr["member"].ToString(),
                               mobile = dr["mobile"].ToString(),
                               assignedConsulant = dr["assignedConsulant"].ToString(),
                               serveConsulant = dr["serveConsulant"].ToString(),
                               status = dr["status"].ToString(),
                               cashFlow = dr["cashFlow"].ToString(),
                               seq = dr["category"].ToString() + dr["seq"].ToString()
                           }).ToList();
            }
            catch (Exception ex)
            {
                string tm = ex.ToString();
            }
            //string memberApi = ConfigurationManager.AppSettings["memberApi"];
            //foreach (var prepaid in prepaids)
            //{

            //    string baseAddress = memberApi + "GetMember/" + prepaid.mobile;
            //    Members member = new Members();

            //    using (var httpClient = new HttpClient())
            //    {

            //        HttpResponseMessage response = await httpClient.GetAsync(baseAddress);
            //        response.EnsureSuccessStatusCode();
            //        member = await response.Content.ReadAsAsync<Members>();

            //    }
            //    //var member = carShopMemberEntities.Members.Where(x => x.seq.ToString() == prepaid.mobile).FirstOrDefault();
            //    if (member != null)
            //    {
            //        prepaid.name = member.name;
            //        prepaid.mobile = member.mobile;
            //    }
            //}

            carbuys = carbuys.OrderByDescending(x => x.createTime).ToList();
            foreach (var carbuy in carbuys)
            {
                if (carbuy.category == "預約賞車")
                {
                    carbuy.dealer = carShopEntities.Shops.Where(x => x.seq.ToString() == carbuy.shopNo && x.status == "上架中").Select(x => x.dealer).FirstOrDefault();
                    carbuy.stronghold = carShopEntities.Shops.Where(x => x.seq.ToString() == carbuy.shopNo && x.status == "上架中").Select(x => x.stronghold).FirstOrDefault();
                    carbuy.shopNo = carShopEntities.Shops.Where(x => x.seq.ToString() == carbuy.shopNo && x.status == "上架中").Select(x => x.ShopNo).FirstOrDefault();
                }
                else
                {
                    carbuy.dealer = carShopEntities.Shops.Where(x => x.ShopNo.ToString() == carbuy.shopNo && x.status == "上架中").Select(x => x.dealer).FirstOrDefault();
                    carbuy.stronghold = carShopEntities.Shops.Where(x => x.ShopNo.ToString() == carbuy.shopNo && x.status == "上架中").Select(x => x.stronghold).FirstOrDefault();
                }
            }

            if (!string.IsNullOrEmpty(dateFilter))
            {
                string[] dateArray = dateFilter.Split(new string[] { " ~ " }, StringSplitOptions.None);
                string[] minArray = dateArray[0].Split('/');
                string[] maxArray = dateArray[1].Split('/');

                DateTime min = DateTime.Parse(dateArray[0]);
                DateTime max = DateTime.Parse(dateArray[1]);
                carbuys = carbuys.Where(x => x.createTime >= min && x.createTime <= max).ToList(); ;
            }

            if (!string.IsNullOrEmpty(dealerFilter))
            {
                carbuys = carbuys.Where(x => x.dealer == dealerFilter).ToList();
            }

            if (!string.IsNullOrEmpty(strongholdFilter))
            {
                carbuys = carbuys.Where(x => x.stronghold == strongholdFilter).ToList();
            }

            if (!string.IsNullOrEmpty(statusFilter))
            {
                carbuys = carbuys.Where(x => x.status == statusFilter).ToList();
            }

            if (!string.IsNullOrEmpty(categoryFilter))
            {
                carbuys = carbuys.Where(x => x.category == categoryFilter).ToList();
            }

            if (!string.IsNullOrEmpty(cashFlowFilter))
            {
                carbuys = carbuys.Where(x => x.cashFlow == cashFlowFilter).ToList();
            }

            if (sortColumn == "建立日期")
            {
                if (sortColumnDirection == "asc") carbuys = carbuys.OrderBy(x => x.createTime).ToList();
                else carbuys = carbuys.OrderByDescending(x => x.createTime).ToList();
            }
            else if (sortColumn == "編號")
            {
                if (sortColumnDirection == "asc") carbuys = carbuys.OrderBy(x => x.shopNo).ToList();
                else carbuys = carbuys.OrderByDescending(x => x.shopNo).ToList();
            }
            else if (sortColumn == "手機")
            {
                if (sortColumnDirection == "asc") carbuys = carbuys.OrderBy(x => x.mobile).ToList();
                else carbuys = carbuys.OrderByDescending(x => x.mobile).ToList();
            }
            else if (sortColumn == "會員姓名")
            {
                if (sortColumnDirection == "asc") carbuys = carbuys.OrderBy(x => x.mobile).ToList();
                else carbuys = carbuys.OrderByDescending(x => x.mobile).ToList();
            }
            else if (sortColumn == "金流")
            {
                if (sortColumnDirection == "asc") carbuys = carbuys.OrderBy(x => x.cashFlow).ToList();
                else carbuys = carbuys.OrderByDescending(x => x.cashFlow).ToList();
            }
            else if (sortColumn == "指定銷售顧問")
            {
                if (sortColumnDirection == "asc") carbuys = carbuys.OrderBy(x => x.assignedConsulant).ToList();
                else carbuys = carbuys.OrderByDescending(x => x.assignedConsulant).ToList();
            }
            else if (sortColumn == "負責銷售顧問")
            {
                if (sortColumnDirection == "asc") carbuys = carbuys.OrderBy(x => x.serveConsulant).ToList();
                else carbuys = carbuys.OrderByDescending(x => x.serveConsulant).ToList();
            }
            else if (sortColumn == "狀態")
            {
                if (sortColumnDirection == "asc") carbuys = carbuys.OrderBy(x => x.status).ToList();
                else carbuys = carbuys.OrderByDescending(x => x.status).ToList();
            }
            else if (sortColumn == "最後更新")
            {
                if (sortColumnDirection == "asc") carbuys = carbuys.OrderBy(x => x.updateTime).ToList();
                else carbuys = carbuys.OrderByDescending(x => x.updateTime).ToList();
            }
            else if (sortColumn == "分類")
            {
                if (sortColumnDirection == "asc") carbuys = carbuys.OrderBy(x => x.category).ToList();
                else carbuys = carbuys.OrderByDescending(x => x.category).ToList();
            }
            else if (sortColumn == "經銷商")
            {
                if (sortColumnDirection == "asc") carbuys = carbuys.OrderBy(x => x.dealer).ToList();
                else carbuys = carbuys.OrderByDescending(x => x.dealer).ToList();
            }
            else if (sortColumn == "中古車營業據點")
            {
                if (sortColumnDirection == "asc") carbuys = carbuys.OrderBy(x => x.stronghold).ToList();
                else carbuys = carbuys.OrderByDescending(x => x.stronghold).ToList();
            }
            carbuys = carbuys.Where(x => x.dealer != null).ToList();
            if (!string.IsNullOrEmpty(keyword))
            {
                var shop1 = carbuys.Where(x => x.createTime != null && x.createTime.ToString().Contains(keyword));
                var shop2 = carbuys.Where(x => x.category != null && x.category.Contains(keyword));
                var shop3 = carbuys.Where(x => x.shopNo != null && x.shopNo.Contains(keyword));
                var shop4 = carbuys.Where(x => x.dealer != null && x.dealer.Contains(keyword));
                var shop5 = carbuys.Where(x => x.mobile != null && x.mobile.Contains(keyword));
                var shop6 = carbuys.Where(x => x.stronghold != null && x.stronghold.ToString().Contains(keyword));
                var shop7 = carbuys.Where(x => x.member != null && x.member.Contains(keyword));
                var shop8 = carbuys.Where(x => x.assignedConsulant != null && x.assignedConsulant.Contains(keyword));
                var shop9 = carbuys.Where(x => x.serveConsulant != null && x.serveConsulant.ToString().Contains(keyword));
                var shop10 = carbuys.Where(x => x.status != null && x.status.ToString().Contains(keyword));
                var shop11 = carbuys.Where(x => x.cashFlow != null && x.cashFlow.ToString().Contains(keyword));
                var shop12 = carbuys.Where(x => x.updateTime != null && x.updateTime.ToString().Contains(keyword));
                carbuys = shop1.Concat(shop2).Concat(shop3).Concat(shop4).Concat(shop5).Concat(shop6).Concat(shop7).Concat(shop8).Concat(shop9).Concat(shop10).Concat(shop11).Concat(shop12).ToList();
            }

            var user = carShopEntities.AspNetUsers.Where(x => x.Email == User.Identity.Name).FirstOrDefault();
            string roleName = string.Empty;
            string company = string.Empty;
            string department = string.Empty;
            if (user != null)
            {
                int role = carShopEntities.UserRoles.Where(x => x.userName == user.NameIdentifier).Select(x => x.userRole).FirstOrDefault() ?? 0;
                roleName = carShopEntities.Roles.Where(x => x.seq == role).Select(x => x.RoleName).FirstOrDefault() ?? string.Empty;

                company = carShopEntities.Profiles.Where(x => x.Id == user.Id).Select(x => x.Company).FirstOrDefault() ?? string.Empty;
                department = carShopEntities.Profiles.Where(x => x.Id == user.Id).Select(x => x.Department).FirstOrDefault() ?? string.Empty;
            }

            if (roleName != "ISC最高權限管理人員" && roleName != "品牌管理人員")
            {
                switch (roleName)
                {
                    case "經銷商管理人員":
                        carbuys = carbuys.Where(x => x.dealer == company).ToList();
                        break;
                    case "據點管理人員":
                        carbuys = carbuys.Where(x => x.dealer == company && x.stronghold == department).ToList();
                        break;
                    default:
                        break;
                }
            }


            var data = carbuys.Skip(skip).Take(pageSize).ToList();
            var jsonData = new { recordsFiltered = carbuys.Count, recordsTotal = carbuys.Count, data = data };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetStatistics(FormCollection post)
        {
            string keyword = post["Keyword"];
            string dateFilter = post["dateFilter"];
            string dealerFilter = post["dealerFilter"];
            string strongholdFilter = post["strongholdFilter"];
            string statusFilter = post["status"];
            string categoryFilter = post["category"];
            string cashFlowFilter = post["cashFlow"];
            int recordsTotal = 0;

            string brand = brandSrv.getBrand(User.Identity.Name);
            var prePaidQuery = from prepaid in carShopEntities.Prepaid
                          join shop in carShopEntities.Shops
                          on prepaid.shopNo equals shop.ShopNo
                          where prepaid.brand == brand && shop.status == "上架中"
                          select new { prepaid, shop };
            var prePaid = prePaidQuery.ToList();
            List<Sells> sells = carShopEntities.Sells.Where(x => x.brand == brand).ToList();
            string sql = "select '預約賞車' as 'category', createTime, shopNo, name as 'member', mobile, assignedConsultant as 'assignedConsulant', consultant as 'serveConsulant', status, '---' as 'cashFlow', updateTime FROM [CarShop].[dbo].[VisitOrders] where brand = @brand  ";

            //DataTable dt = APCommonFun.GetDataTable_MSSQL(sql);
            DataTable dt = APCommonFun.GetSafeDataTable_MSSQL(
                sql,
                new List<SqlParameter>
                {
                    new SqlParameter("@brand", brand)
                }
            );

            List<CarBuyViewModel> carbuys = new List<CarBuyViewModel>();
            try
            {
                carbuys = (from DataRow dr in dt.Rows
                           select new CarBuyViewModel()
                           {
                               category = dr["category"].ToString(),
                               createTime = DateTime.Parse(dr["createTime"].ToString()),
                               updateTime = DateTime.Parse(dr["updateTime"].ToString()),
                               shopNo = dr["shopNo"].ToString(),
                               member = dr["member"].ToString(),
                               mobile = dr["mobile"].ToString(),
                               assignedConsulant = dr["assignedConsulant"].ToString(),
                               serveConsulant = dr["serveConsulant"].ToString(),
                               status = dr["status"].ToString(),
                               cashFlow = dr["cashFlow"].ToString()
                           }).ToList();
            }
            catch (Exception ex)
            {
                string tm = ex.ToString();
            }
           

            foreach (var carbuy in carbuys)
            {
                carbuy.dealer = carShopEntities.Shops.Where(x => x.seq.ToString() == carbuy.shopNo && x.status == "上架中").Select(x => x.dealer).FirstOrDefault();
                carbuy.stronghold = carShopEntities.Shops.Where(x => x.seq.ToString() == carbuy.shopNo && x.status == "上架中").Select(x => x.stronghold).FirstOrDefault();
            }
            carbuys = carbuys.Where(x => x.dealer != null).ToList();
            var user = carShopEntities.AspNetUsers.Where(x => x.Email == User.Identity.Name).FirstOrDefault();
            string roleName = string.Empty;
            string company = string.Empty;
            string department = string.Empty;
            if (user != null)
            {
                int role = carShopEntities.UserRoles.Where(x => x.userName == user.NameIdentifier).Select(x => x.userRole).FirstOrDefault() ?? 0;
                roleName = carShopEntities.Roles.Where(x => x.seq == role).Select(x => x.RoleName).FirstOrDefault() ?? string.Empty;

                company = carShopEntities.Profiles.Where(x => x.Id == user.Id).Select(x => x.Company).FirstOrDefault() ?? string.Empty;
                department = carShopEntities.Profiles.Where(x => x.Id == user.Id).Select(x => x.Department).FirstOrDefault() ?? string.Empty;
            }

            if (roleName != "ISC最高權限管理人員")
            {
                switch (roleName)
                {
                    case "品牌管理人員":
                        break;
                    case "經銷商管理人員":
                        prePaid = prePaid.Where(x => x.shop.dealer.Contains(company)).ToList();
                        sells = sells.Where(x => x.dealer.Contains(company)).ToList();
                        carbuys = carbuys.Where(x => x.dealer.Contains(company)).ToList();
                        break;
                    case "據點管理人員":
                        prePaid = prePaid.Where(x => x.shop.dealer.Contains(department)).ToList();
                        sells = sells.Where(x => x.dealer.Contains(department)).ToList();
                        carbuys = carbuys.Where(x => x.dealer.Contains(department)).ToList();
                        break;
                    case "據點所屬業代":
                        prePaid = prePaid.Where(x => x.shop.dealer.Contains(department)).ToList();
                        sells = sells.Where(x => x.dealer.Contains(department)).ToList();
                        carbuys = carbuys.Where(x => x.dealer.Contains(department)).ToList();
                        break;
                    default:
                        break;
                }
            }


            if (!string.IsNullOrEmpty(dateFilter))
            {
                string[] dateArray = dateFilter.Split(new string[] { " ~ " }, StringSplitOptions.None);
                string[] minArray = dateArray[0].Split('/');
                string[] maxArray = dateArray[1].Split('/');

                DateTime min = DateTime.Parse(dateArray[0]);
                DateTime max = DateTime.Parse(dateArray[1]);
                prePaid = prePaid.Where(x => x.prepaid.createTime >= min && x.prepaid.createTime <= max).ToList();
                sells = sells.Where(x => x.createTime >= min && x.createTime <= max).ToList();
                carbuys = carbuys.Where(x => x.createTime >= min && x.createTime <= max).ToList(); ;
            }

            if (!string.IsNullOrEmpty(dealerFilter))
            {
                prePaid = prePaid.Where(x => x.shop.dealer == dealerFilter).ToList();
                sells = sells.Where(x => x.dealer == dealerFilter).ToList();
                carbuys = carbuys.Where(x => x.dealer == dealerFilter).ToList();
            }

            if (!string.IsNullOrEmpty(strongholdFilter))
            {
                prePaid = prePaid.Where(x => x.shop.stronghold == strongholdFilter).ToList();
                sells = sells.Where(x => x.stronghold == strongholdFilter).ToList();
                carbuys = carbuys.Where(x => x.stronghold == strongholdFilter).ToList();
            }

            if (!string.IsNullOrEmpty(statusFilter))
            {
                prePaid = prePaid.Where(x => x.shop.status == statusFilter).ToList();
                sells = sells.Where(x => x.status == statusFilter).ToList();
                carbuys = carbuys.Where(x => x.status == statusFilter).ToList();
            }

            //if (!string.IsNullOrEmpty(categoryFilter))
            //{
            //    shops = shops.Where(x => x.category == categoryFilter).ToList();
            //    carbuys = carbuys.Where(x => x.category == categoryFilter).ToList();
            //}


            if (!string.IsNullOrEmpty(keyword))
            {
                var shop1 = carbuys.Where(x => x.shopNo != null && x.shopNo.ToString().Contains(keyword));
                var shop2 = carbuys.Where(x => x.dealer != null && x.dealer.Contains(keyword));
                var shop3 = carbuys.Where(x => x.stronghold != null && x.stronghold.Contains(keyword));
                var shop4 = carbuys.Where(x => x.category != null && x.category.Contains(keyword));
                var shop5 = carbuys.Where(x => x.status != null && x.status.Contains(keyword));
                var shop7 = carbuys.Where(x => x.member != null && x.member.ToString().Contains(keyword));
                var shop6 = carbuys.Where(x => x.mobile != null && x.mobile.ToString().Contains(keyword));
                var shop8 = carbuys.Where(x => x.cashFlow != null && x.cashFlow.Contains(keyword));
                var shop9 = carbuys.Where(x => x.assignedConsulant != null && x.assignedConsulant.Contains(keyword));
                var shop10 = carbuys.Where(x => x.serveConsulant != null && x.serveConsulant.Contains(keyword));
                carbuys = shop1.Concat(shop2).Concat(shop3).Concat(shop4).Concat(shop5).Concat(shop6).Concat(shop7).Concat(shop8).Concat(shop9).Concat(shop10).ToList();
            }

            if (!string.IsNullOrEmpty(keyword))
            {
                var shop1 = sells.Where(x => x.sellNo != null && x.sellNo.ToString().Contains(keyword));
                var shop2 = sells.Where(x => x.dealer != null && x.dealer.Contains(keyword));
                var shop3 = sells.Where(x => x.stronghold != null && x.stronghold.Contains(keyword));
                var shop5 = sells.Where(x => x.status != null && x.status.Contains(keyword));
                var shop7 = sells.Where(x => x.owner != null && x.owner.ToString().Contains(keyword));
                var shop6 = sells.Where(x => x.mobile != null && x.mobile.ToString().Contains(keyword));
                var shop9 = sells.Where(x => x.salesRep != null && x.salesRep.Contains(keyword));
                var shop10 = sells.Where(x => x.consultant != null && x.consultant.Contains(keyword));
                sells = shop1.Concat(shop2).Concat(shop3).Concat(shop5).Concat(shop6).Concat(shop7).Concat(shop9).Concat(shop10).ToList();
                prePaid = prePaid.Where(x => 
                (x.prepaid.shopNo.Contains(keyword)) ||
                (!string.IsNullOrEmpty(x.shop.dealer) && x.shop.dealer.Contains(keyword)) ||
                (!string.IsNullOrEmpty(x.shop.stronghold) && x.shop.stronghold.Contains(keyword)) ||
                (!string.IsNullOrEmpty(x.prepaid.mobile) && x.prepaid.mobile.Contains(keyword)) ||
                (!string.IsNullOrEmpty(x.prepaid.name) && x.prepaid.name.Contains(keyword)) ||
                (!string.IsNullOrEmpty(x.prepaid.sales) && x.prepaid.sales.Contains(keyword)) ||
                (!string.IsNullOrEmpty(x.prepaid.serveSales) && x.prepaid.serveSales.Contains(keyword)) ||
                (!string.IsNullOrEmpty(x.prepaid.contactStatus) && x.prepaid.contactStatus.Contains(keyword)) ||
                (!string.IsNullOrEmpty(x.prepaid.paidStatus) && x.prepaid.paidStatus.Contains(keyword))
                ).ToList();
            }

            var data = carbuys;
            var jsonData = new { visitOrder = carbuys.Count, prePay = prePaid.Count, transferToBuyNewCar = sells.Where(x => x.isBuyNewCar == "已轉購買新車").ToList().Count };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public JsonResult GetBuyHistory(FormCollection post)
        {
            var draw = Request.Form["draw"].FirstOrDefault();
            string seq = post["seq"];
            string type = post["type"];
            int recordsTotal = 0;

            List<BuyHistory> buyHistory = new List<BuyHistory>();
            buyHistory = carShopEntities.BuyHistory.Where(x => x.shopNo == seq && x.type == type).OrderByDescending(x => x.recordDate).ToList();

            var jsonData = new { draw = draw, recordsFiltered = buyHistory.Count, recordsTotal = buyHistory.Count, data = buyHistory };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> Update(string seq)
        {
            if (!rightSrv.checkRight("CarBuy", "Update", User.Identity.Name))
            {
                return RedirectToAction("Index", "Home");
            }

            if (!rightSrv.checkRight("CarBuy", "UpdateInProgress", User.Identity.Name))
            {
                TempData["UpdateRight"] = "False";
            }
            else
            {
                TempData["UpdateRight"] = "True";
            }

            CarBuyViewModel model = new CarBuyViewModel();
            string buySeq = seq.Substring(4, seq.Length - 4);
            if (!seq.Contains("預付保留")) //預約賞車
            {
                model.category = "預約賞車";
                var visitOrder = carShopEntities.VisitOrders.Where(x => x.seq.ToString() == buySeq).FirstOrDefault();
                if (visitOrder != null)
                {
                    if (visitOrder.isClose == "Y")
                    {
                        return RedirectToAction("Update2", "CarBuy", new { seq = seq });
                    }
                    var shop = carShopEntities.Shops.Where(x => x.seq.ToString() == visitOrder.shopNo).FirstOrDefault();
                    model.seq = buySeq;
                    model.shopNo = shop.ShopNo;
                    model.createTime = visitOrder.createTime ?? default(DateTime);
                    model.updateTime = visitOrder.updateTime ?? default(DateTime);
                    model.dealer = shop.dealer;
                    model.stronghold = visitOrder.dealerName;
                    model.assignedConsulant = visitOrder.salesRep;
                    model.serveConsulant = string.IsNullOrEmpty(visitOrder.salesRep) ? "無" : visitOrder.salesRep; ;
                    model.member = visitOrder.name;
                    model.mobile = visitOrder.mobile;
                    model.id = visitOrder.id;
                    model.email = visitOrder.email;
                    model.area = !string.IsNullOrEmpty(visitOrder.address) && visitOrder.address.Length > 3 ?  visitOrder.address.Substring(0, 3) : string.Empty;
                    model.address = !string.IsNullOrEmpty(visitOrder.address) && visitOrder.address.Length > 3 ? visitOrder.address.Substring(3, visitOrder.address.Length - 3): string.Empty;
                    model.status = visitOrder.status;
                    model.note = visitOrder.others;
                    model.visitTime = visitOrder.visitTime;
                    model.period = visitOrder.period;
                    model.loseReason = visitOrder.loseReason2;
                }
            }
            else
            {
                return RedirectToAction("Update1", "CarBuy", new { seq = seq });
            }


            List<string> strongholds = carShopEntities.DealerPersons.Where(x => x.dealer == model.dealer && x.businessOffice == model.stronghold).Select(x => x.name).ToList(); ;

            TempData["dealerPersonOptions"] = strongholds;
           
            return View(model);
        }

        public async Task<ActionResult> Update1(string seq)
        {
            if (!rightSrv.checkRight("CarBuy", "Update", User.Identity.Name))
            {
                return RedirectToAction("Index", "Home");
            }

            if (!rightSrv.checkRight("CarBuy", "UpdateInProgress", User.Identity.Name))
            {
                TempData["UpdateRight"] = "False";
            }
            else
            {
                TempData["UpdateRight"] = "True";
            }

            CarBuyViewModel model = new CarBuyViewModel();
            string buySeq = seq.Substring(4, seq.Length - 4);
            model.category = "預付保留";
            var prepaid = carShopEntities.Prepaid.Where(x => x.seq.ToString() == buySeq).FirstOrDefault();
            if (prepaid != null)
            {
                string[] countys = GetCounty();
                model.seq = buySeq;
                model.shopNo = prepaid.shopNo;
                var shop = carShopEntities.Shops.Where(x => x.ShopNo == prepaid.shopNo).FirstOrDefault();
                model.createTime = prepaid.createTime ?? default(DateTime);
                model.updateTime = prepaid.updateTime ?? default(DateTime);
                model.dealer = shop == null ? "" : shop.dealer;
                model.stronghold = shop == null ? "" : shop.stronghold;
                model.assignedConsulant = string.IsNullOrEmpty(prepaid.sales) ? "無" : prepaid.sales;
                model.serveConsulant = prepaid.serveSales;
                model.member = prepaid.name;
                model.mobile = prepaid.mobile;
                model.id = prepaid.id;
                model.email = prepaid.email;
                bool isHasCounty = !string.IsNullOrEmpty(prepaid.address) && countys.Where(x => prepaid.address.IndexOf(x) == 0).Any();
                model.area = isHasCounty ? prepaid.address.Substring(0, 3) : "";
                model.address = isHasCounty ? prepaid.address.Substring(3, prepaid.address.Length - 3) : prepaid.address;
                model.status = string.IsNullOrEmpty(prepaid.status) ? string.IsNullOrEmpty(prepaid.sales) ? "未處理" : "已指派" : prepaid.status;
                model.note = prepaid.others;
                model.visitTime = prepaid.contactTime ?? DateTime.Now;
                model.period = prepaid.contactPeriod;
                model.returnReason = prepaid.returnReason;
                model.cashFlow = prepaid.paidStatus;
                model.prepaidNo = prepaid.prepaidNo;
            }

            List<string> strongholds = carShopEntities.DealerPersons.Where(x => x.dealer == model.dealer && x.businessOffice == model.stronghold).Select(x => x.name).ToList(); ;

            TempData["dealerPersonOptions"] = strongholds;

            return View(model);
        }

        public async Task<ActionResult> Update2(string seq)
        {
            if (!rightSrv.checkRight("CarBuy", "Update", User.Identity.Name))
            {
                return RedirectToAction("Index", "Home");
            }

            if (!rightSrv.checkRight("CarBuy", "UpdateInProgress", User.Identity.Name))
            {
                TempData["UpdateRight"] = "False";
            }
            else
            {
                TempData["UpdateRight"] = "True";
            }

            CarBuyViewModel model = new CarBuyViewModel();
            string buySeq = seq.Substring(4, seq.Length - 4);
            model.category = "預約賞車";
            var visitOrder = carShopEntities.VisitOrders.Where(x => x.seq.ToString() == buySeq).FirstOrDefault();
            if (visitOrder != null)
            {
                var shop = carShopEntities.Shops.Where(x => x.seq.ToString() == visitOrder.shopNo).FirstOrDefault();
                model.seq = buySeq;
                model.shopNo = shop.ShopNo;
                model.createTime = visitOrder.createTime ?? default(DateTime);
                model.updateTime = visitOrder.updateTime ?? default(DateTime);
                model.dealer = shop.dealer;
                model.stronghold = visitOrder.dealerName;
                model.assignedConsulant = visitOrder.salesRep;
                model.serveConsulant = string.IsNullOrEmpty(visitOrder.salesRep) ? "無" : visitOrder.salesRep; ;
                model.member = visitOrder.name;
                model.mobile = visitOrder.mobile;
                model.id = visitOrder.id;
                model.email = visitOrder.email;
                model.area = !string.IsNullOrEmpty(visitOrder.address) && visitOrder.address.Length > 3 ? visitOrder.address.Substring(0, 3) : string.Empty;
                model.address = !string.IsNullOrEmpty(visitOrder.address) && visitOrder.address.Length > 3 ? visitOrder.address.Substring(3, visitOrder.address.Length - 3) : string.Empty;
                model.status = string.IsNullOrEmpty(visitOrder.assignedConsultant) ? "未處理" : "已指派";
                model.note = visitOrder.others;
                model.visitTime = visitOrder.visitTime;
                model.period = visitOrder.period;
                model.loseReason = visitOrder.loseReason2;
            }

            List<string> strongholds = carShopEntities.DealerPersons.Where(x => x.dealer == model.dealer && x.businessOffice == model.stronghold).Select(x => x.name).ToList(); ;

            TempData["dealerPersonOptions"] = strongholds;

            return View(model);
        }

        [HttpPost]
        public ActionResult UpdateInProgress(CarBuyViewModel model)
        {
            if (!rightSrv.checkRight("CarBuy", "UpdateInProgress", User.Identity.Name))
            {
                return RedirectToAction("Index", "Home");
            }
            if (string.IsNullOrEmpty(model.serveConsulant))
            {
                TempData["MemberResult"] = "負責中古車銷售顧問為必填";
                return RedirectToAction("Update", "CarBuy", new { seq = "預約賞車" + model.seq });
            }

            if (string.IsNullOrEmpty(model.member))
            {
                TempData["MemberResult"] = "會員姓名為必填";
                return RedirectToAction("Update", "CarBuy", new { seq = "預約賞車" + model.seq });
            }

            if (string.IsNullOrEmpty(model.mobile))
            {
                TempData["MemberResult"] = "手機為必填";
                return RedirectToAction("Update", "CarBuy", new { seq = "預約賞車" + model.seq });
            }

            if (model.visitTime == null)
            {
                TempData["MemberResult"] = "方便連絡時段為必填";
                return RedirectToAction("Update", "CarBuy", new { seq = "預約賞車" + model.seq });
            }

            if (string.IsNullOrEmpty(model.period))
            {
                TempData["MemberResult"] = "方便連絡時段為必填";
                return RedirectToAction("Update", "CarBuy", new { seq = "預約賞車" + model.seq });
            }

            if (model.status == "戰敗")
            {
                if (string.IsNullOrEmpty(model.loseReason))
                {
                    TempData["MemberResult"] = "戰敗原因為必填";
                    return RedirectToAction("Update", "CarBuy", new { seq = "預約賞車" + model.seq });
                }
            }

            var visitOrder = carShopEntities.VisitOrders.Where(x => x.seq.ToString() == model.seq).FirstOrDefault();
            if (visitOrder != null)
            {
                visitOrder.salesRep = model.assignedConsulant;
                visitOrder.assignedConsultant = model.serveConsulant;
                visitOrder.name = model.member;
                visitOrder.id = model.id;
                visitOrder.mobile = model.mobile;
                visitOrder.email = model.email;
                visitOrder.address = model.area + model.address;
                visitOrder.visitTime = model.visitTime.Value;
                visitOrder.period = model.period;
                visitOrder.others = model.note;
                visitOrder.formalNo = model.formalNo;
                visitOrder.status = model.status;

                switch (model.action)
                {
                    case "儲存":                        
                        visitOrder.loseReason2 = model.loseReason;
                        break;
                    case "結案":
                        visitOrder.isClose = "Y";
                        break;
                    case "預付保留":
                        break;
                    case "預付保留(退訂)":
                        break;
                    default:
                        break;
                }

                BuyHistory buyHistory = new BuyHistory();
                buyHistory.shopNo = model.seq;
                buyHistory.type = "預約賞車";
                buyHistory.recordDate = DateTime.Now;
                buyHistory.consultant = model.assignedConsulant;
                buyHistory.salesRep = model.serveConsulant;
                var user = carShopEntities.AspNetUsers.Where(x => x.Email == User.Identity.Name).FirstOrDefault();
                if (user != null)
                {
                    buyHistory.admin = user.NameIdentifier;
                }
                buyHistory.note = model.note;
                switch (model.action)
                {
                    case "結案":
                        buyHistory.status = "結案";
                        break;
                    default:
                        buyHistory.status = model.status;
                        break;
                }
                carShopEntities.BuyHistory.Add(buyHistory);
            }

            carShopEntities.SaveChanges();
            TempData["MemberResult"] = "OKUpdate";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Update1(CarBuyViewModel model)
        {
            if (!rightSrv.checkRight("CarBuy", "Update", User.Identity.Name))
            {
                return RedirectToAction("Index", "Home");
            }

            if (!rightSrv.checkRight("CarBuy", "UpdateInProgress", User.Identity.Name))
            {
                TempData["UpdateRight"] = "False";
            }
            else
            {
                TempData["UpdateRight"] = "True";
            }

            List<string> strongholds = carShopEntities.DealerPersons.Where(x => x.dealer == model.dealer && x.businessOffice == model.stronghold).Select(x => x.name).ToList(); ;

            TempData["dealerPersonOptions"] = strongholds;
            if (!rightSrv.checkRight("CarBuy", "UpdateInProgress", User.Identity.Name))
            {
                return RedirectToAction("Index", "Home");
            }
            if (string.IsNullOrEmpty(model.serveConsulant))
            {
                TempData["MemberResult"] = "負責中古車銷售顧問為必填";
                // return RedirectToAction("Update", "CarBuy", new { seq = "預付保留" + model.seq });
                return View(model);
            }

            if (string.IsNullOrEmpty(model.member))
            {
                TempData["MemberResult"] = "會員姓名為必填";
                // return RedirectToAction("Update", "CarBuy", new { seq = "預付保留" + model.seq });
                return View(model);
            }

            if (string.IsNullOrEmpty(model.mobile))
            {
                TempData["MemberResult"] = "手機為必填";
                // return RedirectToAction("Update", "CarBuy", new { seq = "預付保留" + model.seq });
                return View(model);
            }

            if (model.visitTime == null)
            {
                TempData["MemberResult"] = "方便連絡時段為必填";
                // return RedirectToAction("Update", "CarBuy", new { seq = "預付保留" + model.seq });
                return View(model);
            }

            if (string.IsNullOrEmpty(model.period))
            {
                TempData["MemberResult"] = "方便連絡時段為必填";
                // return RedirectToAction("Update", "CarBuy", new { seq = "預付保留" + model.seq });
                return View(model);
            }

            if (string.IsNullOrEmpty(model.cashFlow))
            {
                TempData["MemberResult"] = "金流為必填";
                // return RedirectToAction("Update", "CarBuy", new { seq = "預付保留" + model.seq });
                return View(model);
            }

            if (model.cashFlow == "申請退訂")
            {
                if (string.IsNullOrEmpty(model.returnReason))
                {
                    TempData["MemberResult"] = "申請退訂原因為必填";
                    // return RedirectToAction("Update", "CarBuy", new { seq = "預付保留" + model.seq });
                    return View(model);
                }
            }

            var prepaid = carShopEntities.Prepaid.Where(x => x.seq.ToString() == model.seq).FirstOrDefault();
            if (prepaid != null)
            {
                prepaid.serveSales = model.serveConsulant;
                prepaid.name = model.member;
                prepaid.id = model.id;
                prepaid.mobile = model.mobile;
                prepaid.email = model.email;
                prepaid.address = model.area + model.address;
                prepaid.contactTime = model.visitTime;
                prepaid.contactPeriod = model.period;
                prepaid.note = model.note;
                prepaid.formalNo = model.formalNo;
                prepaid.status = model.status;
                prepaid.paidStatus = model.cashFlow;

                switch (model.action)
                {
                    case "儲存":
                        if (prepaid.paidStatus == "申請退訂")
                            prepaid.returnReason = model.returnReason;
                        break;
                    case "結案":
                        prepaid.isClose = "Y";
                        break;
                    case "預付保留":
                        prepaid.paidStatus = "已付訂金";
                        break;
                    case "預付保留(退訂)":
                        prepaid.paidStatus = "已退款";
                        break;
                    default:
                        break;
                }

                BuyHistory buyHistory = new BuyHistory();
                buyHistory.shopNo = model.seq;
                buyHistory.type = "預付保留";
                buyHistory.recordDate = DateTime.Now;
                buyHistory.consultant = model.assignedConsulant;
                buyHistory.salesRep = model.serveConsulant;
                var user = carShopEntities.AspNetUsers.Where(x => x.Email == User.Identity.Name).FirstOrDefault();
                if (user != null)
                {
                    buyHistory.admin = user.NameIdentifier;
                }
                buyHistory.note = model.note;
                switch (model.action)
                {
                    case "結案":
                        buyHistory.status = "結案";
                        break;
                    case "預付保留":
                        buyHistory.status = "預付保留";
                        break;
                    case "預付保留(退訂)":
                        buyHistory.status = "預付保留(退訂)";
                        break;
                    default:
                        buyHistory.status = model.status;
                        break;
                }
                carShopEntities.BuyHistory.Add(buyHistory);
            }

            carShopEntities.SaveChanges();
            TempData["MemberResult"] = "OKUpdate";
            return RedirectToAction("Index");
        }

        public ActionResult ExportCSV(FormCollection post)
        {
            if (!rightSrv.checkRight("CarBuy", "ExportCSV", User.Identity.Name))
            {
                return Content("NO");
            }


            string strFileName = "我想買車列表.csv";

            string keyword = post["Keyword"];
            string dateFilter = post["dateFilter"];
            string dealerFilter = post["dealerFilter"];
            string strongholdFilter = post["strongholdFilter"];
            string statusFilter = post["status"];
            string categoryFilter = post["category"];
            string cashFlowFilter = post["cashFlow"];
            int recordsTotal = 0;

            string brand = brandSrv.getBrand(User.Identity.Name);

            string sql = "SELECT N'預付保留' as 'category', createTime, shopNo, name as 'member', mobile,	sales as 'assignedConsulant', serveSales as 'serveConsulant', contactStatus as 'status', paidStatus as 'cashFlow', updateTime FROM [CarShop].[dbo].[Prepaid] where brand = @brand  UNION ALL select N'預約賞車' as 'category', createTime, shopNo, name as 'member', mobile, assignedConsultant, consultant as 'serveConsulant', status, '---' as 'cashFlow', updateTime FROM [CarShop].[dbo].[VisitOrders] where brand = @brand ";

            //DataTable dt = APCommonFun.GetDataTable_MSSQL(sql);
            DataTable dt = APCommonFun.GetSafeDataTable_MSSQL(
                sql,
                new List<SqlParameter>
                {
                    new SqlParameter("@brand", brand)
                }
            );

            List<CarBuyViewModel> carbuys = new List<CarBuyViewModel>();
            try
            {
                carbuys = (from DataRow dr in dt.Rows
                           select new CarBuyViewModel()
                           {
                               category = dr["category"].ToString(),
                               createTime = DateTime.Parse(dr["createTime"].ToString()),
                               updateTime = DateTime.Parse(dr["updateTime"].ToString()),
                               shopNo = dr["shopNo"].ToString(),
                               member = dr["member"].ToString(),
                               mobile = dr["mobile"].ToString(),
                               assignedConsulant = dr["assignedConsulant"].ToString(),
                               serveConsulant = dr["serveConsulant"].ToString(),
                               status = dr["status"].ToString(),
                               cashFlow = dr["cashFlow"].ToString()
                           }).ToList();

                //return Content("YES");
            }
            catch (Exception ex)
            {
                string tm = ex.ToString();
                return Content(tm);
            }
            //string memberApi = ConfigurationManager.AppSettings["memberApi"];
            //foreach (var prepaid in prepaids)
            //{

            //    string baseAddress = memberApi + "GetMember/" + prepaid.mobile;
            //    Members member = new Members();

            //    using (var httpClient = new HttpClient())
            //    {

            //        HttpResponseMessage response = await httpClient.GetAsync(baseAddress);
            //        response.EnsureSuccessStatusCode();
            //        member = await response.Content.ReadAsAsync<Members>();

            //    }
            //    //var member = carShopMemberEntities.Members.Where(x => x.seq.ToString() == prepaid.mobile).FirstOrDefault();
            //    if (member != null)
            //    {
            //        prepaid.name = member.name;
            //        prepaid.mobile = member.mobile;
            //    }
            //}

            foreach (var carbuy in carbuys)
            {
                carbuy.dealer = carShopEntities.Shops.Where(x => x.ShopNo == carbuy.shopNo).Select(x => x.dealer).FirstOrDefault();
                carbuy.stronghold = carShopEntities.Shops.Where(x => x.ShopNo == carbuy.shopNo).Select(x => x.stronghold).FirstOrDefault();
            }

            if (!string.IsNullOrEmpty(dateFilter))
            {
                string[] dateArray = dateFilter.Split(new string[] { " ~ " }, StringSplitOptions.None);
                string[] minArray = dateArray[0].Split('/');
                string[] maxArray = dateArray[1].Split('/');

                DateTime min = DateTime.Parse(dateArray[0]);
                DateTime max = DateTime.Parse(dateArray[1]);
                carbuys = carbuys.Where(x => x.createTime >= min && x.createTime <= max).ToList(); ;
            }

            if (!string.IsNullOrEmpty(dealerFilter))
            {
                carbuys = carbuys.Where(x => x.dealer == dealerFilter).ToList();
            }

            if (!string.IsNullOrEmpty(strongholdFilter))
            {
                carbuys = carbuys.Where(x => x.stronghold == strongholdFilter).ToList();
            }

            if (!string.IsNullOrEmpty(statusFilter))
            {
                carbuys = carbuys.Where(x => x.status == statusFilter).ToList();
            }

            if (!string.IsNullOrEmpty(categoryFilter))
            {
                carbuys = carbuys.Where(x => x.category == categoryFilter).ToList();
            }

            if (!string.IsNullOrEmpty(cashFlowFilter))
            {
                carbuys = carbuys.Where(x => x.cashFlow == cashFlowFilter).ToList();
            }

            
            if (!string.IsNullOrEmpty(keyword))
            {
                var shop1 = carbuys.Where(x => x.createTime != null && x.createTime.ToString().Contains(keyword));
                var shop2 = carbuys.Where(x => x.category != null && x.category.Contains(keyword));
                var shop3 = carbuys.Where(x => x.shopNo != null && x.shopNo.Contains(keyword));
                var shop4 = carbuys.Where(x => x.dealer != null && x.dealer.Contains(keyword));
                var shop5 = carbuys.Where(x => x.mobile != null && x.mobile.Contains(keyword));
                var shop6 = carbuys.Where(x => x.stronghold != null && x.stronghold.ToString().Contains(keyword));
                var shop7 = carbuys.Where(x => x.member != null && x.member.Contains(keyword));
                var shop8 = carbuys.Where(x => x.assignedConsulant != null && x.assignedConsulant.Contains(keyword));
                var shop9 = carbuys.Where(x => x.serveConsulant != null && x.serveConsulant.ToString().Contains(keyword));
                var shop10 = carbuys.Where(x => x.status != null && x.status.ToString().Contains(keyword));
                var shop11 = carbuys.Where(x => x.cashFlow != null && x.cashFlow.ToString().Contains(keyword));
                var shop12 = carbuys.Where(x => x.updateTime != null && x.updateTime.ToString().Contains(keyword));
                carbuys = shop1.Concat(shop2).Concat(shop3).Concat(shop4).Concat(shop5).Concat(shop6).Concat(shop7).Concat(shop8).Concat(shop9).Concat(shop10).Concat(shop11).Concat(shop12).ToList();
            }

            var result = carbuys;

            //before your loop
            var csv = new StringBuilder();
            csv.AppendLine("分類,建立時間,編號,經銷商,中古車營業據點,會員姓名,手機,指定銷售顧問,負責銷售顧問,狀態,金流,最後更新");
            foreach (var item in result)
            {
                //in your loop
                var newLine = string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11}", item.category, item.createTime, item.shopNo, item.dealer, item.stronghold, item.member, item.mobile, item.assignedConsulant, item.serveConsulant, item.status, item.cashFlow, item.updateTime);
                csv.AppendLine(newLine);
            }

            //after your loop
            Session["genFileName"] = Guid.NewGuid().ToString() + ".csv";
            if (Directory.Exists(Server.MapPath(_exportPath)) == false)
            {
                Directory.CreateDirectory(_exportPath);
            }
            var path = Path.Combine(Server.MapPath(_exportPath), Session["genFileName"].ToString());
            System.IO.File.WriteAllText(path, csv.ToString(), Encoding.UTF8);
            return Content("OK");
        }

        public ActionResult GetWord()
        {
            if (Session["genFileName"] == null)
            {
                Response.Write("您登入時間已逾時，請關掉瀏覽器重新登入!");
                Response.End();
            }

            var TestResult = Path.Combine(Server.MapPath(_exportPath), Session["genFileName"].ToString());

            using (FileStream fileStream = System.IO.File.OpenRead(TestResult))
            {
                MemoryStream memStream = new MemoryStream();

                memStream.SetLength(fileStream.Length);
                fileStream.Read(memStream.GetBuffer(), 0, (int)fileStream.Length);
                Response.Clear();
                Response.ContentType = "application/force-download";
                Response.ContentEncoding = Encoding.UTF8;
                Response.AddHeader("Content-Disposition", "attachment; filename=" + "我想買車列表" + DateTime.Today.ToString("yyyyMMddHHmmss") + ".csv");

                Response.BinaryWrite(memStream.ToArray());
                Response.Flush();
                Response.Close();
                Response.End();
            }

            System.IO.File.Delete(Path.Combine(Server.MapPath(_exportPath), Session["genFileName"].ToString()));

            return RedirectToAction("Index", "CarBuy");
        }
        private string[] GetCounty()
        {
            return new string[] { "台北市", "新北市", "基隆市", "桃園市", "新竹市", "新竹縣", "苗栗縣", "台中市", "彰化縣", "南投縣", "嘉義市", "台南市", "高雄市", "屏東縣", "台東縣", "花蓮縣", "宜蘭縣" };
        }
    }
}