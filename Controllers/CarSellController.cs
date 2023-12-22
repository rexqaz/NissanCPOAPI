using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WebApplication.Models;
using WebApplication.Services;

namespace WebApplication.Controllers
{
    public class CarSellController : Controller
    {
        private LogService srv = new LogService();
        private BrandService brandSrv = new BrandService();
        private RightService rightSrv = new RightService();
        CarShopEntities carShopEntities = new CarShopEntities();
        FileService fileService = new FileService();
        private OneidService oneidService = new OneidService();
        private const string folderPath = "CarSell";
        private const string _exportPath = "../XingUpdateFile/TemporaryFile/CarSell/";

        // GET: CarMgt
        public ActionResult Index()
        {
            if (!rightSrv.checkRight("CarSell", "Index", User.Identity.Name))
            {
                return RedirectToAction("Index", "Home");
            }


            if (!rightSrv.checkRight("CarSell", "ExportCSV", User.Identity.Name))
            {
                TempData["ExportRight"] = "False";
            }
            else
            {
                TempData["ExportRight"] = "True";
            }

            if (!rightSrv.checkRight("CarSell", "Update", User.Identity.Name))
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
        public JsonResult GetSellData(FormCollection post)
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

            List<Sells> sells = carShopEntities.Sells.Where(x => x.brand == brand).OrderByDescending(x => x.createTime).ToList();
            if (roleName != "ISC最高權限管理人員")
            {
                switch (roleName)
                {
                    case "品牌管理人員":
                        break;
                    case "經銷商管理人員":
                        sells = sells.Where(x => x.dealer.Contains(company)).ToList();
                        break;
                    case "據點管理人員":
                        sells = sells.Where(x => x.dealer.Contains(company) && x.stronghold.Contains(department)).ToList();
                        break;
                    case "據點所屬業代":
                        sells = sells.Where(x => x.dealer.Contains(company) && x.stronghold.Contains(department)).ToList();
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
                sells = carShopEntities.Sells.Where(x => x.brand == brand && x.createTime >= min && x.createTime <= max).ToList(); ;
            }

            if (!string.IsNullOrEmpty(dealerFilter))
            {                
                sells = sells.Where(x => x.dealer == dealerFilter).ToList();
            }

            if (!string.IsNullOrEmpty(strongholdFilter))
            {
                sells = sells.Where(x => x.stronghold == strongholdFilter).ToList();
            }

            if (!string.IsNullOrEmpty(statusFilter))
            {
                sells = sells.Where(x => x.status == statusFilter).ToList();
            }

            if (sortColumn == "編號")
            {
                if (sortColumnDirection == "asc") sells = sells.OrderBy(x => x.sellNo).ToList();
                else sells = sells.OrderByDescending(x => x.sellNo).ToList();
            }
            else if (sortColumn == "經銷商")
            {
                if (sortColumnDirection == "asc") sells = sells.OrderBy(x => x.dealer).ToList();
                else sells = sells.OrderByDescending(x => x.dealer).ToList();
            }
            else if (sortColumn == "中古車營業據點")
            {
                if (sortColumnDirection == "asc") sells = sells.OrderBy(x => x.stronghold).ToList();
                else sells = sells.OrderByDescending(x => x.stronghold).ToList();
            }
            else if (sortColumn == "會員姓名")
            {
                if (sortColumnDirection == "asc") sells = sells.OrderBy(x => x.owner).ToList();
                else sells = sells.OrderByDescending(x => x.owner).ToList();
            }
            else if (sortColumn == "車型")
            {
                if (sortColumnDirection == "asc") sells = sells.OrderBy(x => x.carModel).ToList();
                else sells = sells.OrderByDescending(x => x.carModel).ToList();
            }
            else if (sortColumn == "手機")
            {
                if (sortColumnDirection == "asc") sells = sells.OrderBy(x => x.mobile).ToList();
                else sells = sells.OrderByDescending(x => x.mobile).ToList();
            }
            else if (sortColumn == "收購價")
            {
                if (sortColumnDirection == "asc") sells = sells.OrderBy(x => x.price).ToList();
                else sells = sells.OrderByDescending(x => x.price).ToList();
            }
            else if (sortColumn == "狀態")
            {
                if (sortColumnDirection == "asc") sells = sells.OrderBy(x => x.status).ToList();
                else sells = sells.OrderByDescending(x => x.status).ToList();
            }
            else if (sortColumn == "接受新車諮詢")
            {
                if (sortColumnDirection == "asc") sells = sells.OrderBy(x => x.needConsult).ToList();
                else sells = sells.OrderByDescending(x => x.needConsult).ToList();
            }
            else if (sortColumn == "最後更新")
            {
                if (sortColumnDirection == "asc") sells = sells.OrderBy(x => x.updateTime).ToList();
                else sells = sells.OrderByDescending(x => x.updateTime).ToList();
            }

            if (!string.IsNullOrEmpty(keyword))
            {
                var shop1 = sells.Where(x => x.sellNo != null && x.sellNo.ToString().Contains(keyword));
                var shop2 = sells.Where(x => x.dealer != null && x.dealer.Contains(keyword));
                var shop3 = sells.Where(x => x.owner != null && x.owner.Contains(keyword));
                var shop4 = sells.Where(x => x.carModel != null && x.carModel.Contains(keyword));
                var shop5 = sells.Where(x => x.status != null && x.status.Contains(keyword));
                var shop7 = sells.Where(x => x.mobile != null && x.mobile.ToString().Contains(keyword));
                var shop6 = sells.Where(x => x.price != null && x.price.ToString().Contains(keyword));
                var shop8 = sells.Where(x => x.stronghold != null && x.stronghold.ToString().Contains(keyword));
                sells = shop1.Concat(shop2).Concat(shop3).Concat(shop4).Concat(shop5).Concat(shop6).Concat(shop7).Concat(shop8).ToList();
            }


            var data = sells.Skip(skip).Take(pageSize).ToList();
            var jsonData = new {  recordsFiltered = sells.Count, recordsTotal = sells.Count, data = data };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetSellHistory(FormCollection post)
        {
            var draw = Request.Form["draw"].FirstOrDefault();
            string sellNo = post["sellNo"];
            int recordsTotal = 0;

            List<SellsHistory> sellHistory = new List<SellsHistory>();
            sellHistory = carShopEntities.SellsHistory.Where(x => x.sellNo == sellNo).OrderByDescending(x => x.recordDate).ToList();

            var jsonData = new { draw = draw, recordsFiltered = sellHistory.Count, recordsTotal = sellHistory.Count, data = sellHistory };
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

            List<Sells> sells = carShopEntities.Sells.Where(x => x.brand == brand).ToList();
            if (roleName != "ISC最高權限管理人員")
            {
                switch (roleName)
                {
                    case "品牌管理人員":
                        break;
                    case "經銷商管理人員":
                        sells = sells.Where(x => x.dealer.Contains(company)).ToList();
                        break;
                    case "據點管理人員":
                        sells = sells.Where(x => x.dealer.Contains(department)).ToList();
                        break;
                    case "據點所屬業代":
                        sells = sells.Where(x => x.dealer.Contains(department)).ToList();
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
                sells = carShopEntities.Sells.Where(x => x.brand == brand && x.updateTime >= min && x.updateTime <= max).ToList(); ;
            }

            if (!string.IsNullOrEmpty(dealerFilter))
            {
                sells = sells.Where(x => x.dealer == dealerFilter).ToList();
            }

            if (!string.IsNullOrEmpty(strongholdFilter))
            {
                sells = sells.Where(x => x.stronghold == strongholdFilter).ToList();
            }

            if (!string.IsNullOrEmpty(statusFilter))
            {
                sells = sells.Where(x => x.status == statusFilter).ToList();
            }

            
            if (!string.IsNullOrEmpty(keyword))
            {
                var shop1 = sells.Where(x => x.sellNo != null && x.sellNo.ToString().Contains(keyword));
                var shop2 = sells.Where(x => x.dealer != null && x.dealer.Contains(keyword));
                var shop3 = sells.Where(x => x.owner != null && x.owner.Contains(keyword));
                var shop4 = sells.Where(x => x.carModel != null && x.carModel.Contains(keyword));
                var shop5 = sells.Where(x => x.status != null && x.status.Contains(keyword));
                var shop7 = sells.Where(x => x.mobile != null && x.mobile.ToString().Contains(keyword));
                var shop6 = sells.Where(x => x.price != null && x.price.ToString().Contains(keyword));
                var shop8 = sells.Where(x => x.stronghold != null && x.stronghold.ToString().Contains(keyword));
                sells = shop1.Concat(shop2).Concat(shop3).Concat(shop4).Concat(shop5).Concat(shop6).Concat(shop7).Concat(shop8).ToList();
            }


            var data = sells;
            var jsonData = new { needConsult = sells.Where(x => x.needConsult == true).ToList().Count, buyNewCarCount = sells.Where(x => x.isBuyNewCar == "已轉購買新車").ToList().Count, recordsTotal = sells.Count };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Create()
        {
            if (!rightSrv.checkRight("CarSell", "Create", User.Identity.Name))
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

            List<Dealers> dealers = carShopEntities.Dealers.ToList(); ;
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
            foreach (var item in dealers)
            {
                string tmp = item.dealerName + " - " + item.businessOffice;
                dealerOptions.Add(tmp);
            }

            TempData["dealerOptions"] = dealerOptions;
            TempData["today"] = DateTime.Today.ToString("yyyy/MM/dd");
            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult CreateInProgress(Sells model)
        {
            string file = string.Empty;
            Sells sell = new Sells();
            

            if (Request.Files.Count != 4)
            {
                TempData["MemberResult"] = "請上傳檔案";
                //return RedirectToAction("Create", "CarMgt");
            }

            foreach (var item in Request.Files)
            {
                HttpPostedFileBase tmpP = Request.Files[(string)item];
                if (tmpP != null && tmpP.ContentLength > 0)
                {
                    switch ((string)item)
                    {
                        case "carCondition1":
                            sell.carCondition1 = fileService.SaveAsBlob(tmpP, folderPath);
                            break;
                        case "carCondition2":
                            sell.carCondition2 = fileService.SaveAsBlob(tmpP, folderPath);
                            break;
                        case "carCondition3":
                            sell.carCondition3 = fileService.SaveAsBlob(tmpP, folderPath);
                            break;
                        case "carCondition4":
                            sell.carCondition4 = fileService.SaveAsBlob(tmpP, folderPath);
                            break;

                        default:
                            break;
                    }
                }
            }


            if (string.IsNullOrEmpty(model.carModel))
            {
                TempData["MemberResult"] = "車型為必填";
                return RedirectToAction("Create", "CarSell");
            }

            if (string.IsNullOrEmpty(model.owner))
            {
                TempData["MemberResult"] = "車主為必填";
                return RedirectToAction("Create", "CarSell");
            }

            if (string.IsNullOrEmpty(model.title))
            {
                TempData["MemberResult"] = "稱謂為必填";
                return RedirectToAction("Create", "CarSell");
            }

            if (string.IsNullOrEmpty(model.email))
            {
                TempData["MemberResult"] = "Email為必填";
                return RedirectToAction("Create", "CarSell");
            }

            if (string.IsNullOrEmpty(model.mobile))
            {
                TempData["MemberResult"] = "連絡電話為必填";
                return RedirectToAction("Create", "CarSell");
            }

            if (model.licensePlate == null)
            {
                TempData["MemberResult"] = "車牌號碼為必填";
                return RedirectToAction("Create", "CarSell");
            }

            if (string.IsNullOrEmpty(model.area))
            {
                TempData["MemberResult"] = "地區為必填";
                return RedirectToAction("Create", "CarSell");
            }

            if (string.IsNullOrEmpty(model.yearMonthOfManufacture))
            {
                TempData["MemberResult"] = "出廠年月為必填";
                return RedirectToAction("Create", "CarSell");
            }

            if (string.IsNullOrEmpty(model.contactPeriod))
            {
                TempData["MemberResult"] = "聯絡時間為必填";
                return RedirectToAction("Create", "CarSell");
            }

            if (string.IsNullOrEmpty(model.displacement))
            {
                TempData["MemberResult"] = "排氣輛為必填";
                return RedirectToAction("Create", "CarSell");
            }

            if (string.IsNullOrEmpty(model.dealer))
            {
                TempData["MemberResult"] = "營業所為必填";
                return RedirectToAction("Create", "CarSell");
            }

            if (model.milage == 0)
            {
                TempData["MemberResult"] = "里程數為必填";
                return RedirectToAction("Create", "CarSell");
            }

            if (string.IsNullOrEmpty(model.dealer))
            {
                TempData["MemberResult"] = "營業所為必填";
                return RedirectToAction("Create", "CarSell");
            }

            sell.carModel = model.carModel;
            sell.owner = model.owner;
            sell.mobile = model.mobile;
            sell.title = model.title;
            sell.email = model.email;
            sell.licensePlate = model.licensePlate;
            sell.area = model.area;
            sell.displacement = model.displacement;
            sell.milage = model.milage;
            sell.dealer = model.dealer;
            sell.brand = model.brand;
            sell.yearOfManufacture = 2022;
            sell.salesRep = model.dealer;
            sell.contactPeriod = model.contactPeriod;
            sell.yearMonthOfManufacture = model.yearMonthOfManufacture;
            sell.createTime = DateTime.Now;
            carShopEntities.Sells.Add(sell);
            carShopEntities.SaveChanges();
            TempData["MemberResult"] = "OKCreate";
            return RedirectToAction("Index", "CarSell");
        }

        public async Task<ActionResult> Update(int seq)
        {
            if (!rightSrv.checkRight("CarSell", "Update", User.Identity.Name))
            {
                return RedirectToAction("Index", "Home");
            }

            if (!rightSrv.checkRight("CarSell", "UpdateInProgress", User.Identity.Name))
            {
                TempData["UpdateRight"] = "False";
            }
            else
            {
                TempData["UpdateRight"] = "True";
            }
            string company = string.Empty;
            string department = string.Empty;
            string roleName = string.Empty;
            var user = carShopEntities.AspNetUsers.Where(x => x.Email == User.Identity.Name).FirstOrDefault();
            if (user != null)
            {
                int role = carShopEntities.UserRoles.Where(x => x.userName == user.NameIdentifier).Select(x => x.userRole).FirstOrDefault() ?? 0;
                roleName = carShopEntities.Roles.Where(x => x.seq == role).Select(x => x.RoleName).FirstOrDefault() ?? string.Empty;

                company = carShopEntities.Profiles.Where(x => x.Id == user.Id).Select(x => x.Company).FirstOrDefault() ?? string.Empty;
                department = carShopEntities.Profiles.Where(x => x.Id == user.Id).Select(x => x.Department).FirstOrDefault() ?? string.Empty;
            }

            List<Dealers> dealers = carShopEntities.Dealers.ToList(); 
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
            foreach (var item in dealers)
            {
                string tmp = item.dealerName + " - " + item.businessOffice;
                dealerOptions.Add(tmp);
            }

            TempData["today"] = DateTime.Today.ToString("yyyy/MM/dd");

            Sells model = carShopEntities.Sells.Where(x => x.seq == seq).FirstOrDefault();
            string[] yearMonth = model.yearMonthOfManufacture.Split('/');
            model.yearOfManufacture = Convert.ToInt32(yearMonth[0]);
            model.yearMonthOfManufacture = yearMonth[1].PadLeft(2, '0');
            model.carCondition1 = fileService.GetRealUrl(model.carCondition1);
            model.carCondition2 = fileService.GetRealUrl(model.carCondition2);
            model.carCondition3 = fileService.GetRealUrl(model.carCondition3);
            model.carCondition4 = fileService.GetRealUrl(model.carCondition4);
            model.licensePicture = fileService.GetRealUrl(model.licensePicture);

            var member = GetMember(model.mobile);
            model.liveArea = member.area;
            model.address = member.address;

            if (model.status == "結案")
            {
                return RedirectToAction("Update1", "CarSell", new { seq = seq });
            }

            List<string> strongholds = carShopEntities.DealerPersons.Where(x => x.dealer == model.dealer && x.businessOffice == model.stronghold).Select(x => x.name).ToList(); ;


            TempData["dealerOptions"] = dealerOptions;
            TempData["dealerPersonOptions"] = strongholds;
            return View(model);
        }

        public async Task<ActionResult> Update1(int seq)
        {
            if (!rightSrv.checkRight("CarSell", "Update", User.Identity.Name))
            {
                return RedirectToAction("Index", "Home");
            }

            if (!rightSrv.checkRight("CarSell", "UpdateInProgress", User.Identity.Name))
            {
                TempData["UpdateRight"] = "False";
            }
            else
            {
                TempData["UpdateRight"] = "True";
            }
            string company = string.Empty;
            string department = string.Empty;
            string roleName = string.Empty;
            var user = carShopEntities.AspNetUsers.Where(x => x.Email == User.Identity.Name).FirstOrDefault();
            if (user != null)
            {
                int role = carShopEntities.UserRoles.Where(x => x.userName == user.NameIdentifier).Select(x => x.userRole).FirstOrDefault() ?? 0;
                roleName = carShopEntities.Roles.Where(x => x.seq == role).Select(x => x.RoleName).FirstOrDefault() ?? string.Empty;

                company = carShopEntities.Profiles.Where(x => x.Id == user.Id).Select(x => x.Company).FirstOrDefault() ?? string.Empty;
                department = carShopEntities.Profiles.Where(x => x.Id == user.Id).Select(x => x.Department).FirstOrDefault() ?? string.Empty;
            }

            List<Dealers> dealers = carShopEntities.Dealers.ToList();
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
            foreach (var item in dealers)
            {
                string tmp = item.dealerName + " - " + item.businessOffice;
                dealerOptions.Add(tmp);
            }

            TempData["today"] = DateTime.Today.ToString("yyyy/MM/dd");

            Sells model = carShopEntities.Sells.Where(x => x.seq == seq).FirstOrDefault();
            string[] yearMonth = model.yearMonthOfManufacture.Split('/');
            model.yearOfManufacture = Convert.ToInt32(yearMonth[0]);
            model.yearMonthOfManufacture = yearMonth[1].PadLeft(2, '0');           
            model.carCondition1 = fileService.GetRealUrl(model.carCondition1);
            model.carCondition2 = fileService.GetRealUrl(model.carCondition2);
            model.carCondition3 = fileService.GetRealUrl(model.carCondition3);
            model.carCondition4 = fileService.GetRealUrl(model.carCondition4);
            model.licensePicture = fileService.GetRealUrl(model.licensePicture);

            List<string> strongholds = carShopEntities.DealerPersons.Where(x => x.dealer == model.dealer && x.businessOffice == model.stronghold).Select(x => x.name).ToList(); ;
           

            TempData["dealerOptions"] = dealerOptions;
            TempData["dealerPersonOptions"] = strongholds;
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult UpdateInProgress(Sells model)
        {
            if (!rightSrv.checkRight("CarSell", "UpdateInProgress", User.Identity.Name))
            {
                return RedirectToAction("Index", "Home");
            }
            string file = string.Empty;
            Sells sell = carShopEntities.Sells.Where(x => x.seq == model.seq).FirstOrDefault();

            if (string.IsNullOrEmpty(model.carModel))
            {
                TempData["MemberResult"] = "車型為必填";
                return RedirectToAction("Update", "CarSell", new { seq = model.seq });
            }

            if (string.IsNullOrEmpty(model.owner))
            {
                TempData["MemberResult"] = "車主為必填";
                return RedirectToAction("Update", "CarSell", new { seq = model.seq });
            }

            if (string.IsNullOrEmpty(model.yearMonthOfManufacture))
            {
                TempData["MemberResult"] = "出廠年月為必填";
                return RedirectToAction("Update", "CarSell", new { seq = model.seq });
            }

            if (model.yearOfManufacture == 0)
            {
                TempData["MemberResult"] = "出廠年月為必填";
                return RedirectToAction("Update", "CarSell", new { seq = model.seq });
            }

            //if (string.IsNullOrEmpty(model.consultant))
            //{
            //    TempData["MemberResult"] = "負責中古車銷售顧問為必填";
            //    return RedirectToAction("Update", "CarSell", new { seq = model.seq });
            //}

            if (string.IsNullOrEmpty(model.mobile))
            {
                TempData["MemberResult"] = "連絡電話為必填";
                return RedirectToAction("Update", "CarSell", new { seq = model.seq });
            }

            if (model.price == 0)
            {
                TempData["MemberResult"] = "收購價為必填";
                return RedirectToAction("Update", "CarSell", new { seq = model.seq });
            }

            if (model.milage == 0)
            {
                TempData["MemberResult"] = "里程數為必填";
                return RedirectToAction("Update", "CarSell", new { seq = model.seq });
            }

            if (sell != null)
            {
                sell.carModel = model.carModel;
                sell.carBrand = model.carBrand;
                sell.owner = model.owner;
                sell.mobile = model.mobile;
                sell.milage = model.milage;
                sell.salesRep = model.dealer;
                sell.consultant = model.consultant;
                sell.liveArea = model.liveArea;
                sell.address = model.address;
                sell.isBuyNewCar = model.isBuyNewCar;
                sell.loseReason = model.loseReason;
                sell.price = model.price;
                sell.formalSellNo = model.formalSellNo;
                sell.note = model.note;
                sell.yearMonthOfManufacture = model.yearOfManufacture.ToString() + "/" + model.yearMonthOfManufacture;
                sell.updateTime = DateTime.Now;

                switch (model.action)
                {
                    case "儲存":
                        sell.status = model.status;
                        break;
                    case "結案":
                        sell.status = "結案";
                        break;                    
                    default:
                        break;
                }

                foreach (var item in Request.Files)
                {
                    HttpPostedFileBase tmpP = Request.Files[(string)item];
                    if (tmpP != null && tmpP.ContentLength > 0)
                    {                        
                        switch ((string)item)
                        {
                            case "carCondition1":
                                sell.carCondition1 = fileService.SaveAsBlob(tmpP, folderPath);
                                break;
                            case "carCondition2":
                                sell.carCondition2 = fileService.SaveAsBlob(tmpP, folderPath);
                                break;
                            case "carCondition3":
                                sell.carCondition3 = fileService.SaveAsBlob(tmpP, folderPath);
                                break;
                            case "carCondition4":
                                sell.carCondition4 = fileService.SaveAsBlob(tmpP, folderPath);
                                break;
                            case "licensePicture":
                                sell.licensePicture = fileService.SaveAsBlob(tmpP, folderPath);
                                break;
                            default:
                                break;
                        }
                    }
                }
            }

            SellsHistory sellsHistory = new SellsHistory();
            sellsHistory.sellNo = model.sellNo;
            sellsHistory.recordDate = DateTime.Now;
            sellsHistory.consultant = model.consultant;
            sellsHistory.salesRep = model.salesRep;
            var user = carShopEntities.AspNetUsers.Where(x => x.Email == User.Identity.Name).FirstOrDefault();
            if (user != null)
            {
                sellsHistory.admin = user.NameIdentifier;
            }
            sellsHistory.note = model.note;
            switch (model.action)
            {
                case "儲存":
                    sellsHistory.status = model.status;
                    break;
                case "結案":
                    sellsHistory.status = "結案";
                    break;
                default:
                    break;
            }
            carShopEntities.SellsHistory.Add(sellsHistory);

            carShopEntities.SaveChanges();
            TempData["MemberResult"] = "OKUpdate";
            return RedirectToAction("Index", "CarSell");
        }

        public ActionResult ExportCSV(FormCollection post)
        {
            if (!rightSrv.checkRight("CarSell", "ExportCSV", User.Identity.Name))
            {
                return Content("NO");
            }
            string strFileName = "我要賣車列表.csv";

            string keyword = post["Keyword"];
            string dateFilter = post["dateFilter"];
            string dealerFilter = post["dealerFilter"];
            string strongholdFilter = post["strongholdFilter"];
            string statusFilter = post["status"];
            int recordsTotal = 0;

            string brand = brandSrv.getBrand(User.Identity.Name);
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

            List<Sells> sells = carShopEntities.Sells.Where(x => x.brand == brand).ToList();
            if (roleName != "ISC最高權限管理人員")
            {
                switch (roleName)
                {
                    case "品牌管理人員":
                        break;
                    case "經銷商管理人員":
                        sells = sells.Where(x => x.dealer.Contains(company)).ToList();
                        break;
                    case "據點管理人員":
                        sells = sells.Where(x => x.dealer.Contains(department)).ToList();
                        break;
                    case "據點所屬業代":
                        sells = sells.Where(x => x.dealer.Contains(department)).ToList();
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
                sells = carShopEntities.Sells.Where(x => x.brand == brand && x.updateTime >= min && x.updateTime <= max).ToList(); ;
            }

            if (!string.IsNullOrEmpty(dealerFilter))
            {
                sells = sells.Where(x => x.dealer == dealerFilter).ToList();
            }

            if (!string.IsNullOrEmpty(strongholdFilter))
            {
                sells = sells.Where(x => x.stronghold == strongholdFilter).ToList();
            }

            if (!string.IsNullOrEmpty(statusFilter))
            {
                sells = sells.Where(x => x.status == statusFilter).ToList();
            }


            if (!string.IsNullOrEmpty(keyword))
            {
                var shop1 = sells.Where(x => x.sellNo != null && x.sellNo.ToString().Contains(keyword));
                var shop2 = sells.Where(x => x.dealer != null && x.dealer.Contains(keyword));
                var shop3 = sells.Where(x => x.owner != null && x.owner.Contains(keyword));
                var shop4 = sells.Where(x => x.carModel != null && x.carModel.Contains(keyword));
                var shop5 = sells.Where(x => x.status != null && x.status.Contains(keyword));
                var shop7 = sells.Where(x => x.mobile != null && x.mobile.ToString().Contains(keyword));
                var shop6 = sells.Where(x => x.price != null && x.price.ToString().Contains(keyword));
                var shop8 = sells.Where(x => x.stronghold != null && x.stronghold.ToString().Contains(keyword));
                sells = shop1.Concat(shop2).Concat(shop3).Concat(shop4).Concat(shop5).Concat(shop6).Concat(shop7).Concat(shop8).ToList();
            }


            var result = sells;

            //before your loop
            var csv = new StringBuilder();
            csv.AppendLine("編號,經銷商,中古車營業據點,會員姓名,手機,車型,收購價,狀態,接受新車諮詢,最後更新");
            foreach (var item in result)
            {
                string needConsult = item.needConsult == true ? "是" : "否";
                //in your loop
                var newLine = string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9}", item.sellNo, item.dealer, item.stronghold, item.owner, item.mobile, item.carModel, item.price, item.status, needConsult, item.updateTime);
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
                Response.AddHeader("Content-Disposition", "attachment; filename=" + "我要賣車列表" + DateTime.Today.ToString("yyyyMMddHHmmss") + ".csv");

                Response.BinaryWrite(memStream.ToArray());
                Response.Flush();
                Response.Close();
                Response.End();
            }

            System.IO.File.Delete(Path.Combine(Server.MapPath(_exportPath), Session["genFileName"].ToString()));

            return RedirectToAction("Index", "CarSell");
        }

        /// <summary>
        /// 取得oneid登入後的token        
        /// </summary>
        /// <returns></returns>
        private string GetActiveToken()
        {
            var loginData = oneidService.Login("0999888777", "hMDFmy8sPxQ5");
            if (loginData.StatusCode != 200 || loginData.Data == null)
            {
                throw new Exception($"Oneid-API錯誤，StatusCode={loginData.StatusCode}, message={loginData.Message}");
            }

            return loginData.Data["ActiveToken"].ToString();
        }

        private DataModels.MembersModel GetMember(string member_mobile)
        {
            string brand = brandSrv.getBrand(User.Identity.Name);
            var aToken = GetActiveToken();
            return oneidService.GetMemberByMobile(aToken, member_mobile).Data;
        }
        [HttpDelete]
        public ActionResult DeleteImage(string column, int seq)
        {
            Sells model = carShopEntities.Sells.Where(x => x.seq == seq).FirstOrDefault();
            bool res = false;
            if (model != null)
            {
                var deleteProp = model.GetType().GetProperty(column);
                if (deleteProp != null)
                {
                    var deleteFileName = deleteProp.GetValue(model);
                    if (deleteFileName != null)
                    {
                        var deleteResult = fileService.DeleteBlob(deleteFileName.ToString());
                        if (deleteResult)
                        {
                            deleteProp.SetValue(model, "");
                            carShopEntities.SaveChanges();
                            res = true;
                        }
                    }
                }

            }
            return Json(new { success = res });
        }
    }
}