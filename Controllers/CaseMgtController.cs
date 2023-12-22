using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WebApplication.Models;
using WebApplication.Services;

namespace WebApplication.Controllers
{
    public class CaseMgtController : Controller
    {
        private LogService srv = new LogService();
        private BrandService brandSrv = new BrandService();
        private RightService rightSrv = new RightService();
        CarShopEntities carShopEntities = new CarShopEntities();
        private const string _exportPath = "../XingUpdateFile/TemporaryFile/CaseMgt/";

        // GET: CaseMgt
        public ActionResult Index()
        {
            if (!rightSrv.checkRight("CaseMgt", "Index", User.Identity.Name))
            {
                return RedirectToAction("Index", "Home");
            }

            if (!rightSrv.checkRight("CaseMgt", "ExportCSV", User.Identity.Name))
            {
                TempData["ExportRight"] = "False";
            }
            else
            {
                TempData["ExportRight"] = "True";
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
        public async Task<JsonResult> GetCaseData(FormCollection post)
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

            List<Sells> sells = carShopEntities.Sells.Where(x => x.brand == brand).ToList();
            //string sql = "SELECT N'我想買車' as 'category', seq, createTime, shopNo, name as 'member', mobile,	sales as 'assignedConsulant', serveSales as 'serveConsulant', contactStatus as 'status', paidStatus as 'cashFlow', updateTime FROM [CarShop].[dbo].[Prepaid] where brand = @brand  UNION ALL select N'我想買車' as 'category', seq, createTime, shopNo, name as 'member', mobile, salesRep as 'assignedConsulant', assignedConsultant as 'serveConsulant', status, '---' as 'cashFlow', updateTime  FROM [CarShop].[dbo].[VisitOrders] where brand = @brand ";

            string sql2 = "SELECT N'我要賣車' as 'category', seq, createTime, sellNo as 'shopNo', owner as 'member', mobile, salesRep as 'assignedConsulant', consultant as 'serveConsulant', status, '' as 'cashFlow', updateTime, dealer, stronghold FROM [CarShop].[dbo].[Sells] where brand = @brand ";

            //DataTable dt = APCommonFun.GetDataTable_MSSQL(sql);
            //DataTable dt2 = APCommonFun.GetDataTable_MSSQL(sql2);
            DataTable dt2 = APCommonFun.GetSafeDataTable_MSSQL(
                sql2,
                new List<SqlParameter>
                {
                    new SqlParameter("@brand", brand)
                }
            );

            List<CarBuyViewModel> carSells = new List<CarBuyViewModel>();
            List<CarBuyViewModel> result = new List<CarBuyViewModel>();
            try
            {
                

                carSells = (from DataRow dr in dt2.Rows
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
                                seq = dr["category"].ToString() + dr["seq"].ToString(),
                                dealer = dr["dealer"].ToString(),
                                stronghold = dr["stronghold"].ToString()
                            }).ToList();
            }
            catch (Exception ex)
            {
                string tm = ex.ToString();
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
            var carbuyData = getCarBuyDataQuery(post, brand, roleName, company, department);
            result = carbuyData.Concat(carSells).ToList();
            result = result.OrderByDescending(x => x.createTime).ToList();

            if (!string.IsNullOrEmpty(dateFilter))
            {
                string[] dateArray = dateFilter.Split(new string[] { " ~ " }, StringSplitOptions.None);
                string[] minArray = dateArray[0].Split('/');
                string[] maxArray = dateArray[1].Split('/');

                DateTime min = DateTime.Parse(dateArray[0]);
                DateTime max = DateTime.Parse(dateArray[1]);
                result = result.Where(x => x.createTime >= min && x.createTime <= max).ToList();
            }

            if (!string.IsNullOrEmpty(dealerFilter))
            {
                result = result.Where(x => x.dealer == dealerFilter).ToList();
            }

            if (!string.IsNullOrEmpty(strongholdFilter))
            {
                result = result.Where(x => x.stronghold == strongholdFilter).ToList();
            }

            if (!string.IsNullOrEmpty(statusFilter))
            {
                result = result.Where(x => x.status == statusFilter).ToList();
            }

            if (!string.IsNullOrEmpty(categoryFilter))
            {
                result = result.Where(x => x.category == categoryFilter).ToList();
            }

            if (sortColumn == "建立日期")
            {
                if (sortColumnDirection == "asc") result = result.OrderBy(x => x.createTime).ToList();
                else result = result.OrderByDescending(x => x.createTime).ToList();
            }
            else if (sortColumn == "編號")
            {
                if (sortColumnDirection == "asc") result = result.OrderBy(x => x.shopNo).ToList();
                else result = result.OrderByDescending(x => x.shopNo).ToList();
            }
            else if (sortColumn == "手機")
            {
                if (sortColumnDirection == "asc") result = result.OrderBy(x => x.mobile).ToList();
                else result = result.OrderByDescending(x => x.mobile).ToList();
            }
            else if (sortColumn == "會員姓名")
            {
                if (sortColumnDirection == "asc") result = result.OrderBy(x => x.mobile).ToList();
                else result = result.OrderByDescending(x => x.mobile).ToList();
            }
            else if (sortColumn == "金流")
            {
                if (sortColumnDirection == "asc") result = result.OrderBy(x => x.cashFlow).ToList();
                else result = result.OrderByDescending(x => x.cashFlow).ToList();
            }
            else if (sortColumn == "指定銷售顧問")
            {
                if (sortColumnDirection == "asc") result = result.OrderBy(x => x.assignedConsulant).ToList();
                else result = result.OrderByDescending(x => x.assignedConsulant).ToList();
            }
            else if (sortColumn == "負責銷售顧問")
            {
                if (sortColumnDirection == "asc") result = result.OrderBy(x => x.serveConsulant).ToList();
                else result = result.OrderByDescending(x => x.serveConsulant).ToList();
            }
            else if (sortColumn == "狀態")
            {
                if (sortColumnDirection == "asc") result = result.OrderBy(x => x.status).ToList();
                else result = result.OrderByDescending(x => x.status).ToList();
            }
            else if (sortColumn == "最後更新")
            {
                if (sortColumnDirection == "asc") result = result.OrderBy(x => x.updateTime).ToList();
                else result = result.OrderByDescending(x => x.updateTime).ToList();
            }
            else if (sortColumn == "分類")
            {
                if (sortColumnDirection == "asc") result = result.OrderBy(x => x.category).ToList();
                else result = result.OrderByDescending(x => x.category).ToList();
            }
            else if (sortColumn == "經銷商")
            {
                if (sortColumnDirection == "asc") result = result.OrderBy(x => x.dealer).ToList();
                else result = result.OrderByDescending(x => x.dealer).ToList();
            }
            else if (sortColumn == "中古車營業據點")
            {
                if (sortColumnDirection == "asc") result = result.OrderBy(x => x.stronghold).ToList();
                else result = result.OrderByDescending(x => x.stronghold).ToList();
            }

            if (!string.IsNullOrEmpty(keyword))
            {
                var shop1 = result.Where(x => x.createTime != null && x.createTime.ToString().Contains(keyword));
                var shop2 = result.Where(x => x.category != null && x.category.Contains(keyword));
                var shop3 = result.Where(x => x.shopNo != null && x.shopNo.Contains(keyword));
                var shop4 = result.Where(x => x.dealer != null && x.dealer.Contains(keyword));
                var shop5 = result.Where(x => x.mobile != null && x.mobile.Contains(keyword));
                var shop6 = result.Where(x => x.stronghold != null && x.stronghold.ToString().Contains(keyword));
                var shop7 = result.Where(x => x.member != null && x.member.Contains(keyword));
                var shop8 = result.Where(x => x.assignedConsulant != null && x.assignedConsulant.Contains(keyword));
                var shop9 = result.Where(x => x.serveConsulant != null && x.serveConsulant.ToString().Contains(keyword));
                var shop10 = result.Where(x => x.status != null && x.status.ToString().Contains(keyword));
                var shop11 = result.Where(x => x.cashFlow != null && x.cashFlow.ToString().Contains(keyword));
                var shop12 = result.Where(x => x.updateTime != null && x.updateTime.ToString().Contains(keyword));
                result = shop1.Concat(shop2).Concat(shop3).Concat(shop4).Concat(shop5).Concat(shop6).Concat(shop7).Concat(shop8).Concat(shop9).Concat(shop10).Concat(shop11).Concat(shop12).ToList();
            }

            

            if (roleName != "ISC最高權限管理人員" && roleName != "品牌管理人員")
            {
                switch (roleName)
                {
                    case "經銷商管理人員":
                        result = result.Where(x => x.dealer == company).ToList();
                        break;
                    case "據點管理人員":
                        result = result.Where(x => x.dealer == company && x.stronghold == department).ToList();
                        break;
                    default:
                        break;
                }
            }

            var data = result.Skip(skip).Take(pageSize).ToList();
            var jsonData = new { recordsFiltered = result.Count, recordsTotal = result.Count, data = data };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }
        private List<CarBuyViewModel> getCarBuyDataQuery(FormCollection post, string brand, string roleName, string company, string department)
        {
            string keyword = post["Keyword"];
            string dateFilter = post["dateFilter"];
            string dealerFilter = post["dealerFilter"];
            string strongholdFilter = post["strongholdFilter"];
            string statusFilter = post["status"];
            string categoryFilter = post["category"];
            string cashFlowFilter = post["cashFlow"];
            var visitOrderQuery = from vis in carShopEntities.VisitOrders
                                  join shop in carShopEntities.Shops on vis.shopNo equals shop.seq.ToString()
                                  where vis.brand == brand && shop.brand == brand && shop.status == "上架中"
                                  select new CarBuyViewModel() { category = "我想買車", createTime = vis.createTime.Value, shopNo = shop.ShopNo, member = vis.name, mobile = vis.mobile, assignedConsulant = vis.assignedConsultant, serveConsulant = vis.consultant, status = vis.status, cashFlow = "---", updateTime = vis.updateTime.Value, dealer = shop.dealer, stronghold = shop.stronghold }
                      ;
            var prePaidQuery = from vis in carShopEntities.Prepaid
                               join shop in carShopEntities.Shops on vis.shopNo equals shop.ShopNo
                               where vis.brand == brand && shop.brand == brand && shop.status == "上架中"
                               select new CarBuyViewModel() { category = "我想買車", createTime = vis.createTime.Value, shopNo = shop.ShopNo, member = vis.name, mobile = vis.mobile, assignedConsulant = vis.sales, serveConsulant = vis.serveSales, status = vis.contactStatus, cashFlow = vis.paidStatus, updateTime = vis.updateTime.Value, dealer = shop.dealer, stronghold = shop.stronghold }
                                  ;
            if (roleName != "ISC最高權限管理人員")
            {
                switch (roleName)
                {
                    case "品牌管理人員":
                        break;
                    case "經銷商管理人員":
                        visitOrderQuery = visitOrderQuery.Where(x => x.dealer == company);
                        prePaidQuery = prePaidQuery.Where(x => x.dealer == company);
                        break;
                    case "據點管理人員":
                        visitOrderQuery = visitOrderQuery.Where(x => x.stronghold == department);
                        prePaidQuery = prePaidQuery.Where(x => x.stronghold == department);
                        break;
                    case "據點所屬業代":
                        visitOrderQuery = visitOrderQuery.Where(x => x.stronghold == department);
                        prePaidQuery = prePaidQuery.Where(x => x.stronghold == department);
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
                visitOrderQuery = visitOrderQuery.Where(x => x.createTime >= min && x.createTime <= max);
                prePaidQuery = prePaidQuery.Where(x => x.createTime >= min && x.createTime <= max);
            }

            if (!string.IsNullOrEmpty(dealerFilter))
            {
                visitOrderQuery = visitOrderQuery.Where(x => x.dealer == dealerFilter);
                prePaidQuery = prePaidQuery.Where(x => x.dealer == dealerFilter);
            }

            if (!string.IsNullOrEmpty(strongholdFilter))
            {
                visitOrderQuery = visitOrderQuery.Where(x => x.stronghold == strongholdFilter);
                prePaidQuery = prePaidQuery.Where(x => x.stronghold == strongholdFilter);
            }

            if (!string.IsNullOrEmpty(statusFilter))
            {
                visitOrderQuery = visitOrderQuery.Where(x => x.status == statusFilter);
                prePaidQuery = prePaidQuery.Where(x => x.status == statusFilter);
            }
            if (!string.IsNullOrEmpty(keyword))
            {
                prePaidQuery = prePaidQuery.Where(x =>
                (x.shopNo.Contains(keyword)) ||
                (x.dealer.Contains(keyword)) ||
                (x.stronghold.Contains(keyword)) ||
                x.category.Contains(keyword) ||
                x.status.Contains(keyword) ||
                (!string.IsNullOrEmpty(x.member) && x.member.Contains(keyword)) ||
                (!string.IsNullOrEmpty(x.mobile) && x.mobile.Contains(keyword)) ||
                (!string.IsNullOrEmpty(x.cashFlow) && x.cashFlow.Contains(keyword)) ||
                (!string.IsNullOrEmpty(x.assignedConsulant) && x.assignedConsulant.Contains(keyword)) ||
                (!string.IsNullOrEmpty(x.serveConsulant) && x.serveConsulant.Contains(keyword))
                );
                visitOrderQuery = visitOrderQuery.Where(x =>
                (x.shopNo.Contains(keyword)) ||
                (x.dealer.Contains(keyword)) ||
                (x.stronghold.Contains(keyword)) ||
                x.category.Contains(keyword) ||
                x.status.Contains(keyword) ||
                (!string.IsNullOrEmpty(x.member) && x.member.Contains(keyword)) ||
                (!string.IsNullOrEmpty(x.mobile) && x.mobile.Contains(keyword)) ||
                (!string.IsNullOrEmpty(x.cashFlow) && x.cashFlow.Contains(keyword)) ||
                (!string.IsNullOrEmpty(x.assignedConsulant) && x.assignedConsulant.Contains(keyword)) ||
                (!string.IsNullOrEmpty(x.serveConsulant) && x.serveConsulant.Contains(keyword))
                );
            }
            return visitOrderQuery.ToList().Concat(prePaidQuery.ToList()).ToList();
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
            List<Sells> sells = carShopEntities.Sells.Where(x => x.brand == brand).ToList();
            List<Shops> shops = carShopEntities.Shops.Where(x => x.brand == brand).ToList();
            string sql = "select '預約賞車' as 'category', createTime, shopNo, name as 'member', mobile, assignedConsultant as 'assignedConsulant', consultant as 'serveConsulant', status, '---' as 'cashFlow', updateTime, dealerName as 'dealer' FROM [CarShop].[dbo].[VisitOrders] where brand = @brand ";

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
                               dealer = dr["dealer"].ToString()
                           }).ToList();
            }
            catch (Exception ex)
            {
                string tm = ex.ToString();
            }


            foreach (var carbuy in carbuys)
            {
                carbuy.dealer = carShopEntities.Shops.Where(x => x.ShopNo == carbuy.shopNo).Select(x => x.dealer).FirstOrDefault();
                carbuy.stronghold = carShopEntities.Shops.Where(x => x.ShopNo == carbuy.shopNo).Select(x => x.stronghold).FirstOrDefault();
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

            if (roleName != "ISC最高權限管理人員")
            {
                switch (roleName)
                {
                    case "品牌管理人員":
                        break;
                    case "經銷商管理人員":
                        sells = sells.Where(x => x.dealer == company).ToList();
                        carbuys = carbuys.Where(x => x.dealer == company).ToList();
                        shops = shops.Where(x => x.dealer == company).ToList();
                        break;
                    case "據點管理人員":
                        sells = sells.Where(x => x.stronghold == department).ToList();
                        carbuys = carbuys.Where(x => x.stronghold == department).ToList();
                        shops = shops.Where(x => x.stronghold == department).ToList();
                        break;
                    case "據點所屬業代":
                        sells = sells.Where(x => x.stronghold == department).ToList();
                        carbuys = carbuys.Where(x => x.stronghold == department).ToList();
                        shops = shops.Where(x => x.stronghold == department).ToList();
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
                sells = sells.Where(x => x.createTime >= min && x.createTime <= max).ToList();
                carbuys = carbuys.Where(x => x.createTime >= min && x.createTime <= max).ToList();
                shops = shops.Where(x => x.updateTime >= min && x.updateTime <= max).ToList(); ;
            }

            if (!string.IsNullOrEmpty(dealerFilter))
            {
                sells = sells.Where(x => x.dealer == dealerFilter).ToList();
                carbuys = carbuys.Where(x => x.dealer == dealerFilter).ToList();
                shops = shops.Where(x => x.dealer == dealerFilter).ToList();
            }

            if (!string.IsNullOrEmpty(strongholdFilter))
            {
                sells = sells.Where(x => x.stronghold == strongholdFilter).ToList();
                carbuys = carbuys.Where(x => x.stronghold == strongholdFilter).ToList();
                shops = shops.Where(x => x.dealer == dealerFilter).ToList();
            }

            if (!string.IsNullOrEmpty(statusFilter))
            {
                sells = sells.Where(x => x.status == statusFilter).ToList();
                carbuys = carbuys.Where(x => x.status == statusFilter).ToList();
                shops = shops.Where(x => x.dealer == dealerFilter).ToList();
            }

            //if (!string.IsNullOrEmpty(categoryFilter))
            //{
            //    sells = sells.Where(x => x.category == categoryFilter).ToList();
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
            }

            if (!string.IsNullOrEmpty(keyword))
            {
                var shop1 = shops.Where(x => x.ShopNo != null && x.ShopNo.ToString().Contains(keyword));
                var shop2 = shops.Where(x => x.dealer != null && x.dealer.Contains(keyword));
                var shop5 = shops.Where(x => x.status != null && x.status.Contains(keyword));
                var shop8 = shops.Where(x => x.stronghold != null && x.stronghold.Contains(keyword));
                shops = shop1.Concat(shop2).Concat(shop5).Concat(shop8).ToList();
            }

            var buyCarList = getCarBuyDataQuery(post, brand, roleName, company, department);
            var jsonData = new { visitOrder = buyCarList.Count, carSell = sells.Count, transferToBuyNewCar = sells.Where(x => x.isBuyNewCar == "已轉購買新車").ToList().Count, carOnShelves = shops.Count, carOnShelvesSell = shops.Where(x => x.status == "成交下架").ToList().Count };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ExportCSV(FormCollection post)
        {
            try
            {
                if (!rightSrv.checkRight("CaseMgt", "ExportCSV", User.Identity.Name))
                {
                    return Content("NO");
                }
                string strFileName = "案件總覽列表.csv";

                string keyword = post["Keyword"];
                string dateFilter = post["dateFilter"];
                string dealerFilter = post["dealerFilter"];
                string strongholdFilter = post["strongholdFilter"];
                string statusFilter = post["status"];
                string categoryFilter = post["category"];

                string brand = brandSrv.getBrand(User.Identity.Name);

                List<Sells> sells = carShopEntities.Sells.Where(x => x.brand == brand).ToList();
                //string sql = "SELECT N'我想買車' as 'category', seq, createTime, shopNo, name as 'member', mobile,	sales as 'assignedConsulant', serveSales as 'serveConsulant', contactStatus as 'status', paidStatus as 'cashFlow', updateTime FROM [CarShop].[dbo].[Prepaid] where brand = @brand UNION ALL select N'我想買車' as 'category', seq, createTime, shopNo, name as 'member', mobile, assignedConsultant, consultant as 'serveConsulant', status, '---' as 'cashFlow', updateTime FROM [CarShop].[dbo].[VisitOrders] where brand = @brand ";

                string sql2 = "SELECT N'我要賣車' as 'category', seq, createTime, sellNo as 'shopNo', owner as 'member', mobile, salesRep as 'assignedConsulant', consultant as 'serveConsulant', status, '' as 'cashFlow', updateTime, dealer, stronghold FROM [CarShop].[dbo].[Sells] where brand = @brand ";

                //DataTable dt = APCommonFun.GetDataTable_MSSQL(sql);
                //DataTable dt2 = APCommonFun.GetDataTable_MSSQL(sql2);
                DataTable dt2 = APCommonFun.GetSafeDataTable_MSSQL(
                    sql2,
                    new List<SqlParameter>
                    {
                    new SqlParameter("@brand", brand)
                    }
                );

                List<CarBuyViewModel> carSells = new List<CarBuyViewModel>();
                List<CarBuyViewModel> result = new List<CarBuyViewModel>();
                try
                {
                    carSells = (from DataRow dr in dt2.Rows
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
                                    seq = dr["category"].ToString() + dr["seq"].ToString(),
                                    dealer = dr["dealer"].ToString(),
                                    stronghold = dr["stronghold"].ToString()
                                }).ToList();
                }
                catch (Exception ex)
                {
                    string tm = ex.ToString();
                }

                string roleName = string.Empty;
                string company = string.Empty;
                string department = string.Empty;
                var user = carShopEntities.AspNetUsers.Where(x => x.Email == User.Identity.Name).FirstOrDefault();
                if (user != null)
                {
                    int role = carShopEntities.UserRoles.Where(x => x.userName == user.NameIdentifier).Select(x => x.userRole).FirstOrDefault() ?? 0;
                    roleName = carShopEntities.Roles.Where(x => x.seq == role).Select(x => x.RoleName).FirstOrDefault() ?? string.Empty;

                    company = carShopEntities.Profiles.Where(x => x.Id == user.Id).Select(x => x.Company).FirstOrDefault() ?? string.Empty;
                    department = carShopEntities.Profiles.Where(x => x.Id == user.Id).Select(x => x.Department).FirstOrDefault() ?? string.Empty;
                }
                var carBuyData = getCarBuyDataQuery(post, brand, roleName, company, department);
                result = carBuyData.Concat(carSells).ToList();

                if (!string.IsNullOrEmpty(dateFilter))
                {
                    string[] dateArray = dateFilter.Split(new string[] { " ~ " }, StringSplitOptions.None);
                    string[] minArray = dateArray[0].Split('/');
                    string[] maxArray = dateArray[1].Split('/');

                    DateTime min = DateTime.Parse(dateArray[0]);
                    DateTime max = DateTime.Parse(dateArray[1]);
                    result = result.Where(x => x.createTime >= min && x.createTime <= max).ToList();
                }

                if (!string.IsNullOrEmpty(dealerFilter))
                {
                    result = result.Where(x => x.dealer == dealerFilter).ToList();
                }

                if (!string.IsNullOrEmpty(strongholdFilter))
                {
                    result = result.Where(x => x.stronghold == strongholdFilter).ToList();
                }

                if (!string.IsNullOrEmpty(statusFilter))
                {
                    result = result.Where(x => x.status == statusFilter).ToList();
                }

                if (!string.IsNullOrEmpty(categoryFilter))
                {
                    result = result.Where(x => x.category == categoryFilter).ToList();
                }

                if (!string.IsNullOrEmpty(keyword))
                {
                    var shop1 = result.Where(x => x.createTime != null && x.createTime.ToString().Contains(keyword));
                    var shop2 = result.Where(x => x.category != null && x.category.Contains(keyword));
                    var shop3 = result.Where(x => x.shopNo != null && x.shopNo.Contains(keyword));
                    var shop4 = result.Where(x => x.dealer != null && x.dealer.Contains(keyword));
                    var shop5 = result.Where(x => x.mobile != null && x.mobile.Contains(keyword));
                    var shop6 = result.Where(x => x.stronghold != null && x.stronghold.ToString().Contains(keyword));
                    var shop7 = result.Where(x => x.member != null && x.member.Contains(keyword));
                    var shop8 = result.Where(x => x.assignedConsulant != null && x.assignedConsulant.Contains(keyword));
                    var shop9 = result.Where(x => x.serveConsulant != null && x.serveConsulant.ToString().Contains(keyword));
                    var shop10 = result.Where(x => x.status != null && x.status.ToString().Contains(keyword));
                    var shop11 = result.Where(x => x.cashFlow != null && x.cashFlow.ToString().Contains(keyword));
                    var shop12 = result.Where(x => x.updateTime != null && x.updateTime.ToString().Contains(keyword));
                    result = shop1.Concat(shop2).Concat(shop3).Concat(shop4).Concat(shop5).Concat(shop6).Concat(shop7).Concat(shop8).Concat(shop9).Concat(shop10).Concat(shop11).Concat(shop12).ToList();
                }

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
                var dirPath = Server.MapPath(_exportPath);
                if (Directory.Exists(dirPath) == false)
                {
                    Directory.CreateDirectory(dirPath);
                }
                var path = Path.Combine(dirPath, Session["genFileName"].ToString());
                System.IO.File.WriteAllText(path, csv.ToString(), Encoding.UTF8);
            }
            catch (Exception ex)
            {
                APCommonFun.Error("[CaseMgt: ExportCSV]" + ex.ToString());
                return Content("failed");
            }

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
                Response.AddHeader("Content-Disposition", "attachment; filename=" + "案件總覽列表" + DateTime.Today.ToString("yyyyMMddHHmmss") + ".csv");

                Response.BinaryWrite(memStream.ToArray());
                Response.Flush();
                Response.Close();
                Response.End();
            }

            System.IO.File.Delete(Path.Combine(Server.MapPath(_exportPath), Session["genFileName"].ToString()));

            return RedirectToAction("Index", "CaseMgt");
        }
    }
}