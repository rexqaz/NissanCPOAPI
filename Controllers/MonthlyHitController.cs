using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using WebApplication.Models;
using WebApplication.Services;

namespace WebApplication.Controllers
{
    public class MonthlyHitController : Controller
    {
        private LogService srv = new LogService();
        private BrandService brandSrv = new BrandService();
        private RightService rightSrv = new RightService();
        CarShopEntities carShopEntities = new CarShopEntities();
        // GET: MonthlyHit
        public ActionResult Index()
        {
            if (!rightSrv.checkRight("MonthlyHit", "Index", User.Identity.Name))
            {
                return RedirectToAction("Index", "Home");
            }

            if (!rightSrv.checkRight("MonthlyHit", "Update", User.Identity.Name))
            {
                TempData["UpdateRight"] = "False";
            }
            else
            {
                TempData["UpdateRight"] = "True";
            }

            if (!rightSrv.checkRight("MonthlyHit", "Create", User.Identity.Name))
            {
                TempData["CreateRight"] = "False";
            }
            else
            {
                TempData["CreateRight"] = "True";
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
        public JsonResult GetShopData(FormCollection post)
        {
            var draw = Request.Form["draw"].FirstOrDefault();
            string start = Request.Form["start"];
            string length = Request.Form["length"];
            string keyword = post["Keyword"];
            string dateFilter = post["dateFilter"];
            string dealerFilter = post["dealerFilter"];
            string strongholdFilter = post["strongholdFilter"];
            string statusFilter = post["status"];
            string sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"];
            string sortColumnDirection = Request.Form["order[0][dir]"];
            string searchValue = Request.Form["search[value]"];
            int pageSize = Convert.ToInt32(length);
            int skip = Convert.ToInt32(start);
            int recordsTotal = 0;

            string brand = brandSrv.getBrand(User.Identity.Name);
            List<Shops> shops = carShopEntities.Shops.Where(x => x.brand == brand && x.isHit == "Y").OrderBy(x => x.hitOrder).ToList();

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
                        shops = shops.Where(x => x.dealer.Contains(company)).ToList();
                        break;
                    case "據點管理人員":
                        shops = shops.Where(x => x.dealer.Contains(department)).ToList();
                        break;
                    case "據點所屬業代":
                        shops = shops.Where(x => x.dealer.Contains(department)).ToList();
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
                shops = carShopEntities.Shops.Where(x => x.brand == brand && x.updateTime >= min && x.updateTime <= max).ToList(); ;
            }

            if (!string.IsNullOrEmpty(dealerFilter))
            {
                shops = shops.Where(x => x.dealer == dealerFilter).ToList();
            }

            if (!string.IsNullOrEmpty(strongholdFilter))
            {
                shops = shops.Where(x => x.stronghold == strongholdFilter).ToList();
            }

            if (!string.IsNullOrEmpty(statusFilter))
            {
                shops = shops.Where(x => x.status == statusFilter).ToList();
            }

            if (sortColumn == "編號")
            {
                if (sortColumnDirection == "asc") shops = shops.OrderBy(x => x.ShopNo).ToList();
                else shops = shops.OrderByDescending(x => x.ShopNo).ToList();
            }
            else if (sortColumn == "經銷商")
            {
                if (sortColumnDirection == "asc") shops = shops.OrderBy(x => x.dealer).ToList();
                else shops = shops.OrderByDescending(x => x.dealer).ToList();
            }
            else if (sortColumn == "中古車營業據點")
            {
                if (sortColumnDirection == "asc") shops = shops.OrderBy(x => x.stronghold).ToList();
                else shops = shops.OrderByDescending(x => x.stronghold).ToList();
            }
            else if (sortColumn == "車種")
            {
                if (sortColumnDirection == "asc") shops = shops.OrderBy(x => x.carType).ToList();
                else shops = shops.OrderByDescending(x => x.carType).ToList();
            }
            else if (sortColumn == "車型")
            {
                if (sortColumnDirection == "asc") shops = shops.OrderBy(x => x.carModel).ToList();
                else shops = shops.OrderByDescending(x => x.carModel).ToList();
            }
            else if (sortColumn == "出廠年月")
            {
                if (sortColumnDirection == "asc") shops = shops.OrderBy(x => x.yearOfManufacture).ToList();
                else shops = shops.OrderByDescending(x => x.yearOfManufacture).ToList();
            }
            else if (sortColumn == "出售價")
            {
                if (sortColumnDirection == "asc") shops = shops.OrderBy(x => x.price).ToList();
                else shops = shops.OrderByDescending(x => x.price).ToList();
            }
            else if (sortColumn == "狀態")
            {
                if (sortColumnDirection == "asc") shops = shops.OrderBy(x => x.status).ToList();
                else shops = shops.OrderByDescending(x => x.status).ToList();
            }
            else if (sortColumn == "最後更新")
            {
                if (sortColumnDirection == "asc") shops = shops.OrderBy(x => x.updateTime).ToList();
                else shops = shops.OrderByDescending(x => x.updateTime).ToList();
            }
            else if (sortColumn == "上架")
            {
                if (sortColumnDirection == "asc") shops = shops.OrderBy(x => x.createTime).ToList();
                else shops = shops.OrderByDescending(x => x.createTime).ToList();
            }
            else if (sortColumn == "成交下架")
            {
                if (sortColumnDirection == "asc") shops = shops.OrderBy(x => x.sellTime).ToList();
                else shops = shops.OrderByDescending(x => x.sellTime).ToList();
            }

            if (!string.IsNullOrEmpty(keyword))
            {
                var shop1 = shops.Where(x => x.ShopNo != null && x.ShopNo.ToString().Contains(keyword));
                var shop2 = shops.Where(x => x.dealer != null && x.dealer.Contains(keyword));
                var shop3 = shops.Where(x => x.carType != null && x.carType.Contains(keyword));
                var shop4 = shops.Where(x => x.carModel != null && x.carModel.Contains(keyword));
                var shop5 = shops.Where(x => x.status != null && x.status.Contains(keyword));
                var shop7 = shops.Where(x => x.yearOfManufacture != null && x.yearOfManufacture.ToString().Contains(keyword));
                var shop6 = shops.Where(x => x.price != null && x.price.ToString().Contains(keyword));
                var shop8 = shops.Where(x => x.stronghold != null && x.stronghold.Contains(keyword));
                shops = shop1.Concat(shop2).Concat(shop3).Concat(shop4).Concat(shop5).Concat(shop6).Concat(shop7).Concat(shop8).ToList();
            }

            var data = shops.Skip(skip).Take(pageSize).ToList();
            foreach (var item in data)
            {
                item.NoticeCount = carShopEntities.NoticeRecords.Where(x => x.shopNo == item.ShopNo).ToList().Count;
            }
            var jsonData = new { draw = draw, recordsFiltered = shops.Count, recordsTotal = shops.Count, data = data };
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
            int recordsTotal = 0;

            string brand = brandSrv.getBrand(User.Identity.Name);
            List<Shops> shops = carShopEntities.Shops.Where(x => x.brand == brand).ToList();

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
                        shops = shops.Where(x => x.dealer.Contains(company)).ToList();
                        break;
                    case "據點管理人員":
                        shops = shops.Where(x => x.dealer.Contains(department)).ToList();
                        break;
                    case "據點所屬業代":
                        shops = shops.Where(x => x.dealer.Contains(department)).ToList();
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
                shops = carShopEntities.Shops.Where(x => x.brand == brand && x.updateTime >= min && x.updateTime <= max).ToList(); ;
            }

            if (!string.IsNullOrEmpty(dealerFilter))
            {
                shops = shops.Where(x => x.dealer == dealerFilter).ToList();
            }

            if (!string.IsNullOrEmpty(strongholdFilter))
            {
                shops = shops.Where(x => x.stronghold == strongholdFilter).ToList();
            }

            if (!string.IsNullOrEmpty(statusFilter))
            {
                shops = shops.Where(x => x.status == statusFilter).ToList();
            }


            if (!string.IsNullOrEmpty(keyword))
            {
                var shop1 = shops.Where(x => x.ShopNo != null && x.ShopNo.ToString().Contains(keyword));
                var shop2 = shops.Where(x => x.dealer != null && x.dealer.Contains(keyword));
                var shop3 = shops.Where(x => x.carType != null && x.carType.Contains(keyword));
                var shop4 = shops.Where(x => x.carModel != null && x.carModel.Contains(keyword));
                var shop5 = shops.Where(x => x.status != null && x.status.Contains(keyword));
                var shop7 = shops.Where(x => x.yearOfManufacture != null && x.yearOfManufacture.ToString().Contains(keyword));
                var shop6 = shops.Where(x => x.price != null && x.price.ToString().Contains(keyword));
                var shop8 = shops.Where(x => x.stronghold != null && x.stronghold.Contains(keyword));
                shops = shop1.Concat(shop2).Concat(shop3).Concat(shop4).Concat(shop5).Concat(shop6).Concat(shop7).Concat(shop8).ToList();
            }

            foreach (var item in shops)
            {
                item.NoticeCount = carShopEntities.NoticeRecords.Where(x => x.shopNo == item.ShopNo).ToList().Count;
            }
            var data = shops;
            var jsonData = new { carOnShelves = shops.Count, carSell = shops.Where(x => x.status == "成交下架").ToList().Count };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCarModel(string dealer, string stronghold, bool getOnline = false)
        {
            string brand = brandSrv.getBrand(User.Identity.Name);
            var carModel = carShopEntities.Shops.Where(x => x.dealer == dealer && x.stronghold == stronghold && x.brand == brand);
            if (getOnline)
            {
                carModel = carModel.Where(x => x.status == "上架中");
            }
            var result = carModel.GroupBy(x => x.carModel).Select(x => x.Key).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetYearOfManufacture(string dealer, string stronghold, string carModel, bool getOnline = false)
        {
            var yearOfManufacture = carShopEntities.Shops.Where(x => x.dealer == dealer && x.stronghold == stronghold && x.carModel == carModel);
            if (getOnline)
            {
                yearOfManufacture = yearOfManufacture.Where(x => x.status == "上架中");
            }
            var result = yearOfManufacture.Select(x => x.yearOfManufacture).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetMilage(string dealer, string stronghold, string carModel, int yearOfManufacture, bool getOnline = false)
        {
            var milage = carShopEntities.Shops.Where(x => x.dealer == dealer && x.stronghold == stronghold && x.carModel == carModel && x.yearOfManufacture == yearOfManufacture);
            if (getOnline)
            {
                milage = milage.Where(x => x.status == "上架中");
            }
            var result = milage.Select(x => x.milage).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetShopNo(string dealer, string stronghold, string carModel, int yearOfManufacture, int milage, bool getOnline = false)
        {
            var shopNo = carShopEntities.Shops.Where(x => x.dealer == dealer && x.stronghold == stronghold && x.carModel == carModel && x.yearOfManufacture == yearOfManufacture && x.milage == milage && x.status == "上架中");
            if (getOnline)
            {
                shopNo = shopNo.Where(x => x.status == "上架中");
            }
            var result = shopNo.Select(x => x.ShopNo).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Create()
        {
            if (!rightSrv.checkRight("MonthlyHit", "Create", User.Identity.Name))
            {
                return RedirectToAction("Index", "Home");
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
            List<Dealers> dealers = carShopEntities.Dealers.Where(x => x.brand == brand).ToList(); ;
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
        public ActionResult CreateInProgress(Shops model)
        {
            string brand = brandSrv.getBrand(User.Identity.Name);
            int hitCount = carShopEntities.Shops.Where(x => x.brand == brand && x.isHit == "Y").Count();
            if (hitCount == 20)
            {
                TempData["MemberResult"] = "本月主打車輛上限為20台!!";
                return RedirectToAction("Index", "MonthlyHit");
            }

            if (string.IsNullOrEmpty(model.carModel))
            {
                TempData["MemberResult"] = "車型為必填";
                return RedirectToAction("Create", "MonthlyHit");
            }

            if (model.yearOfManufacture == null)
            {
                TempData["MemberResult"] = "出廠年月為必填";
                return RedirectToAction("Create", "MonthlyHit");
            }
            
            if (model.milage == 0)
            {
                TempData["MemberResult"] = "里程數為必填";
                return RedirectToAction("Create", "MonthlyHit");
            }

            if (string.IsNullOrEmpty(model.dealer))
            {
                TempData["MemberResult"] = "經銷商為必填";
                return RedirectToAction("Create", "MonthlyHit");
            }

            if (string.IsNullOrEmpty(model.stronghold))
            {
                TempData["MemberResult"] = "中古車營業據點為必填";
                return RedirectToAction("Create", "MonthlyHit");
            }

            if (string.IsNullOrEmpty(model.ShopNo))
            {
                TempData["MemberResult"] = "編號為必填";
                return RedirectToAction("Create", "MonthlyHit");
            }

            Shops shop = carShopEntities.Shops.Where(x => x.ShopNo == model.ShopNo).FirstOrDefault();
            shop.isHit = "Y";
            shop.isFrame = model.isFrame;
            shop.hitOrder = model.hitOrder;
            
            carShopEntities.SaveChanges();
            TempData["MemberResult"] = "OKCreate";
            return RedirectToAction("Index", "MonthlyHit");
        }

        public ActionResult Update(int seq)
        {
            if (!rightSrv.checkRight("MonthlyHit", "Update", User.Identity.Name))
            {
                return RedirectToAction("Index", "Home");
            }

            if (!rightSrv.checkRight("MonthlyHit", "UpdateInProgress", User.Identity.Name))
            {
                TempData["UpdateRight"] = "False";
            }
            else
            {
                TempData["UpdateRight"] = "True";
            }

            if (!rightSrv.checkRight("MonthlyHit", "Delete", User.Identity.Name))
            {
                TempData["DeleteRight"] = "False";
            }
            else
            {
                TempData["DeleteRight"] = "True";
            }

            Shops model = carShopEntities.Shops.Where(x => x.seq == seq).FirstOrDefault();

            return View(model);
        }

        [HttpPost]
        public ActionResult UpdateInProgress(Shops model)
        {
            if (!rightSrv.checkRight("MonthlyHit", "UpdateInProgress", User.Identity.Name))
            {
                return RedirectToAction("Index", "Home");
            }

            Shops shop = carShopEntities.Shops.Where(x => x.seq == model.seq).FirstOrDefault();
            shop.isFrame = model.isFrame;
            shop.hitOrder = model.hitOrder;

            carShopEntities.SaveChanges();
            TempData["MemberResult"] = "OKUpdate";
            return RedirectToAction("Index", "MonthlyHit");
        }

        public ActionResult Delete(int seq)
        {
            if (!rightSrv.checkRight("MonthlyHit", "Delete", User.Identity.Name))
            {
                return RedirectToAction("Index", "Home");
            }
            var shop = carShopEntities.Shops.Where(x => x.seq == seq).FirstOrDefault();
            shop.isHit = "N";
            shop.isFrame = string.Empty;
            shop.hitOrder = null;

            carShopEntities.SaveChanges();

            TempData["MemberResult"] = "OKDelete";
            return RedirectToAction("Index", "MonthlyHit");
        }
    }
}