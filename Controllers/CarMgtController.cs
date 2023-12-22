using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.Mvc;
using WebApplication.Models;
using WebApplication.Services;

namespace WebApplication.Controllers
{
    public class CarMgtController : Controller
    {
        private const string _exportPath = "../XingUpdateFile/TemporaryFile/CarMgt/";
        private LogService srv = new LogService();
        private BrandService brandSrv = new BrandService();
        private RightService rightSrv = new RightService();
        CarShopEntities carShopEntities = new CarShopEntities();
        FileService fileService = new FileService();
        private const string folderPath = "CarShops";

        // GET: CarMgt
        public ActionResult Index()
        {
            if (!rightSrv.checkRight("CarMgt", "Index", User.Identity.Name))
            {
                return RedirectToAction("Index", "Home");
            }

            if (!rightSrv.checkRight("CarMgt", "ExportCSV", User.Identity.Name))
            {
                TempData["ExportRight"] = "False";
            }
            else
            {
                TempData["ExportRight"] = "True";
            }

            if (!rightSrv.checkRight("CarMgt", "Update", User.Identity.Name))
            {
                TempData["UpdateRight"] = "False";
            }
            else
            {
                TempData["UpdateRight"] = "True";
            }

            if (!rightSrv.checkRight("CarMgt", "Create", User.Identity.Name))
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
            List<Shops> shops = new List<Shops>();

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

            shops = carShopEntities.Shops.Where(x => x.brand == brand).OrderByDescending(x => x.createTime).ToList();
            if (roleName != "ISC最高權限管理人員")
            {
                switch (roleName)
                {
                    case "品牌管理人員":
                        shops = carShopEntities.Shops.Where(x => x.brand == brand).ToList();
                        break;
                    case "經銷商管理人員":
                        shops = carShopEntities.Shops.Where(x => x.brand == brand && x.dealer == company).ToList();
                        break;
                    case "據點管理人員":
                        shops = carShopEntities.Shops.Where(x => x.brand == brand && x.dealer == company && x.stronghold == department).ToList();
                        break;
                    case "據點所屬業代":
                        shops = carShopEntities.Shops.Where(x => x.brand == brand && x.dealer == company && x.stronghold == department).ToList();
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
                shops = carShopEntities.Shops.Where(x => x.brand == brand && x.createTime >= min && x.createTime <= max).ToList(); ;
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
            var jsonData = new {recordsFiltered = shops.Count, recordsTotal = shops.Count, data = data };
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
            List<Shops> shops = new List<Shops>();

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

            shops = carShopEntities.Shops.Where(x => x.brand == brand && x.status == "上架中").ToList();
            if (roleName != "ISC最高權限管理人員")
            {
                switch (roleName)
                {
                    case "品牌管理人員":
                        shops = carShopEntities.Shops.Where(x => x.brand == brand).ToList();
                        break;
                    case "經銷商管理人員":
                        shops = carShopEntities.Shops.Where(x => x.brand == brand && x.dealer == company).ToList();
                        break;
                    case "據點管理人員":
                        shops = carShopEntities.Shops.Where(x => x.brand == brand && x.dealer == company && x.stronghold == department).ToList();
                        break;
                    case "據點所屬業代":
                        shops = carShopEntities.Shops.Where(x => x.brand == brand && x.dealer == company && x.stronghold == department).ToList();
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

        [HttpPost]
        public JsonResult GetNoticeData(FormCollection post)
        {
            var draw = Request.Form["draw"].FirstOrDefault();
            string shopNo = post["shopNo"];
            int recordsTotal = 0;

            List<NoticeRecordViewModel> noticeRecords = new List<NoticeRecordViewModel>();

            string sql = "  SELECT YEAR([noticeTime]) as year, MONTH([noticeTime]) as month, COUNT(R.[shopNo]) AS TOTALCOUNT FROM [NoticeRecords] R where R.shopNo = @shopNo GROUP BY YEAR([noticeTime]), MONTH([noticeTime]) ORDER BY YEAR([noticeTime]), MONTH([noticeTime]) ";

            //DataTable dt = APCommonFun.GetDataTable_MSSQL(sql);
            DataTable dt = APCommonFun.GetSafeDataTable_MSSQL(
                sql,
                new List<SqlParameter>
                {
                    new SqlParameter("@shopNo", shopNo)
                }
            );

            int tmpNoticeTimes = 0;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                NoticeRecordViewModel tmp = new NoticeRecordViewModel();
                tmp.period = dt.Rows[i]["year"].ToString() + "/" + dt.Rows[i]["month"].ToString();
                tmp.noticeTimes = dt.Rows[i]["TOTALCOUNT"].ToString();
                tmpNoticeTimes += Convert.ToInt32(tmp.noticeTimes);
                tmp.accumulativeNoticeTimes = (tmpNoticeTimes).ToString();
                noticeRecords.Add(tmp);
            }

            //var data = noticeRecords.Skip(skip).Take(pageSize).ToList();
            var jsonData = new { draw = draw, recordsFiltered = noticeRecords.Count, recordsTotal = noticeRecords.Count, data = noticeRecords };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetVisitOrder(FormCollection post)
        {
            var draw = Request.Form["draw"].FirstOrDefault();
            string shopNo = post["shopNo"];

            Shops model = carShopEntities.Shops.Where(x => x.ShopNo == shopNo).FirstOrDefault();
            string[] dealerArray = model.dealer.Split('-');
            string dealer = dealerArray[0].Trim();
            string businessOffice = dealerArray[1].Trim();
            List<string> dealerPersons = carShopEntities.DealerPersons.Where(x => x.dealer == dealer && x.businessOffice == businessOffice).Select(x => x.name).ToList();



            List<VisitOrders> visitOrders = carShopEntities.VisitOrders.Where(x => x.shopNo == shopNo).ToList();
            List<VisitOrderViewModel> result = new List<VisitOrderViewModel>();
            foreach (var item in visitOrders)
            {
                VisitOrderViewModel tmp = new VisitOrderViewModel();
                tmp.seq = item.seq;
                tmp.name = item.name;
                tmp.mobile = item.mobile;
                tmp.title = item.title;
                tmp.visitTime = item.visitTime;
                tmp.salesRep = item.salesRep;
                tmp.status = item.status;
                tmp.consultantName = item.consultant;
                string consultant = "<select class='form-control consultant'" + item.seq + " id='consultant" + item.seq + "'>";
                foreach (var item2 in dealerPersons)
                {
                    consultant += "<option value='" + item2 + "' ";
                    if (item2 == item.consultant)
                    {
                        consultant += "selected >" + item2 + "</option>";
                    }
                    else
                    {
                        consultant += ">" + item2 + "</option>";
                    }
                }
                consultant += "</select>";
                tmp.consultant = consultant;

                result.Add(tmp);
            }

            var jsonData = new { draw = draw, recordsFiltered = result.Count, recordsTotal = result.Count, data = result };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult UpdateVisitOrder()
        {
            string result = string.Empty;

            var seq = Request["seq"];
            var status = Request["status"];
            var consultant = Request["consultant"];

            try
            {
                var visitOrder = carShopEntities.VisitOrders.Where(x => x.seq.ToString() == seq).FirstOrDefault();
                if (visitOrder != null)
                {
                    visitOrder.status = status;
                    visitOrder.consultant = consultant;
                }

                carShopEntities.SaveChanges();
                result = "OK";
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }

            return Content(result);
        }


        public JsonResult GetDealer()
        {
            var dealerOptions = GetDealerList();
            return Json(new
            {
                list = dealerOptions
            }, JsonRequestBehavior.AllowGet);
            //return Json(dealerOptions, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetStronghold(string dealer)
        {
            string brand = brandSrv.getBrand(User.Identity.Name);
            var stronghold = carShopEntities.Dealers.Where(x => x.brand == brand && x.dealerName == dealer && x.type == "old").Select(x => x.businessOffice).ToList();

            return Json(stronghold, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Create()
        {
            if (!rightSrv.checkRight("CarMgt", "Create", User.Identity.Name))
            {
                return RedirectToAction("Index", "Home");
            }

            TempData["dealerOptions"] = GetDealerList();
            TempData["shopNo"] = "系統自動產生";
            TempData["today"] = DateTime.Today.ToString("yyyy/MM/dd");
            return View();
        }

        private List<string> GetDealerList()
        {
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
            List<Dealers> dealers = carShopEntities.Dealers.Where(x => x.brand == brand).ToList();
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

            return dealerOptions;
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult CreateInProgress(CarShopViewModel model)
        {
            string file = string.Empty;
            Shops shop = new Shops();

            //新增車輛時的需要填寫所有欄位改為非必填
            //if(Request.Files.Count < 6)
            //{
            //    TempData["MemberResult"] = "請上傳檔案";
            //    return RedirectToAction("Create", "CarMgt");
            //}

            var fileCount = 0;
            foreach (var item in Request.Files)
            {
                HttpPostedFileBase tmpP = Request.Files[(string)item];
                if (tmpP != null && tmpP.ContentLength > 0)
                {
                    switch ((string)item)
                    {
                        case "carCondition1":
                            shop.carCondition1 = SaveAsLocal(tmpP);
                            fileCount++;
                            break;
                        case "carCondition2":
                            shop.carCondition2 = SaveAsLocal(tmpP);
                            fileCount++;
                            break;
                        case "carCondition3":
                            shop.carCondition3 = SaveAsLocal(tmpP);
                            fileCount++;
                            break;
                        case "carCondition4":
                            shop.carCondition4 = SaveAsLocal(tmpP);
                            fileCount++;
                            break;
                        case "carCondition5":
                            shop.carCondition5 = SaveAsLocal(tmpP);
                            fileCount++;
                            break;
                        case "carCondition6":
                            shop.carCondition6 = SaveAsLocal(tmpP);
                            fileCount++;
                            break;
                        case "carCondition7":
                            shop.carCondition7 = SaveAsLocal(tmpP);
                            fileCount++;
                            break;
                        case "carCondition8":
                            shop.carCondition8 = SaveAsLocal(tmpP);
                            fileCount++;
                            break;
                        case "carCondition9":
                            shop.carCondition9 = SaveAsLocal(tmpP);
                            fileCount++;
                            break;
                        case "carCondition10":
                            shop.carCondition10 = SaveAsLocal(tmpP);
                            fileCount++;
                            break;
                        case "inspectionTable":
                            shop.inspectionTable = SaveAsLocal(tmpP);
                            fileCount++;
                            break;
                        default:
                            break;
                    }
                }
            }

            #region 新增車輛時的需要填寫所有欄位改為非必填

            //新增車輛時的需要填寫所有欄位改為非必填
            //if (string.IsNullOrEmpty(model.carType))
            //{
            //    TempData["MemberResult"] = "車種為必填";
            //    return RedirectToAction("Create", "CarMgt");
            //}

            //if (string.IsNullOrEmpty(model.carModel))
            //{
            //    TempData["MemberResult"] = "車型為必填";
            //    return RedirectToAction("Create", "CarMgt");
            //}

            //if (model.yearOfManufacture == null)
            //{
            //    TempData["MemberResult"] = "出廠年月為必填";
            //    return RedirectToAction("Create", "CarMgt");
            //}

            //if (model.monthOfManufacture == null)
            //{
            //    TempData["MemberResult"] = "出廠年月為必填";
            //    return RedirectToAction("Create", "CarMgt");
            //}

            //if (string.IsNullOrEmpty(model.outerColor))
            //{
            //    TempData["MemberResult"] = "車色為必填";
            //    return RedirectToAction("Create", "CarMgt");
            //}

            //if (string.IsNullOrEmpty(model.horsepower))
            //{
            //    TempData["MemberResult"] = "馬力為必填";
            //    return RedirectToAction("Create", "CarMgt");
            //}

            //if (model.outEquip.Count() == 0)
            //{
            //    TempData["MemberResult"] = "內外裝配備";
            //    return RedirectToAction("Create", "CarMgt");
            //}

            //if (model.feature.Count() == 0)
            //{
            //    TempData["MemberResult"] = "安全重點功能";
            //    return RedirectToAction("Create", "CarMgt");
            //}

            //if (string.IsNullOrEmpty(model.fuelType))
            //{
            //    TempData["MemberResult"] = "燃料為必填";
            //    return RedirectToAction("Create", "CarMgt");
            //}

            //if (string.IsNullOrEmpty(model.driveMode))
            //{
            //    TempData["MemberResult"] = "驅動方式為必填";
            //    return RedirectToAction("Create", "CarMgt");
            //}

            //if (string.IsNullOrEmpty(model.displacement))
            //{
            //    TempData["MemberResult"] = "排氣輛為必填";
            //    return RedirectToAction("Create", "CarMgt");
            //}

            //if (string.IsNullOrEmpty(model.area))
            //{
            //    TempData["MemberResult"] = "地區為必填";
            //    return RedirectToAction("Create", "CarMgt");
            //}

            //if (model.milage == 0)
            //{
            //    TempData["MemberResult"] = "里程數為必填";
            //    return RedirectToAction("Create", "CarMgt");
            //}

            //if (string.IsNullOrEmpty(model.dealer))
            //{
            //    TempData["MemberResult"] = "經銷商為必填";
            //    return RedirectToAction("Create", "CarMgt");
            //}

            //if (string.IsNullOrEmpty(model.stronghold))
            //{
            //    TempData["MemberResult"] = "中古車營業據點為必填";
            //    return RedirectToAction("Create", "CarMgt");
            //}

            //if (model.price == 0)
            //{
            //    TempData["MemberResult"] = "售價為必填";
            //    return RedirectToAction("Create", "CarMgt");
            //}


            //if (string.IsNullOrEmpty(model.description))
            //{
            //    TempData["MemberResult"] = "簡介說明為必填";
            //    return RedirectToAction("Create", "CarMgt");
            //}

            #endregion

            shop.carType = model.carType;
            shop.carModel = model.carModel;
            shop.innerColor = model.innerColor;
            shop.outerColor = model.outerColor;
            shop.horsepower = model.horsepower;
            shop.transmissionType = model.transmissionType;
            shop.ListingDate = model.ListingDate;
            shop.fuelType = model.fuelType;
            shop.displacement = model.displacement;
            shop.milage = model.milage;
            shop.driveMode = model.driveMode;
            shop.dealer = model.dealer;
            shop.brand = brandSrv.getBrand(User.Identity.Name);
            shop.price = model.price;
            shop.yearOfManufacture = model.yearOfManufacture;
            shop.monthOfManufacture = model.monthOfManufacture;
            shop.salesRep = model.dealer;
            if (shop.salesRep == null) shop.salesRep = string.Empty;
            shop.description = model.description;
            shop.createTime = DateTime.Now;
            shop.updateTime = DateTime.Now;
            shop.outEquip = model.outEquip == null ? string.Empty : string.Join(",", model.outEquip);
            shop.innerEquip = model.innerEquip;
            shop.feature = model.feature == null ? string.Empty : string.Join(",", model.feature);
            shop.stronghold = model.stronghold;
            shop.area = model.area;
            //故意清空，透過交易模式增加流水號，避免重複
            shop.ShopNo = "";
            switch (model.action)
            {
                case "儲存":
                    shop.status = "草稿";
                    break;
                case "上架申請":
                    shop.status = "上架申請";
                    break;
                default:
                    shop.status = "";
                    break;
            }
            carShopEntities.Shops.Add(shop);
            try
            {
                carShopEntities.SaveChanges();

                //透過交易模式增加流水號
                TempData["shopNo"] = APCommonFun.Transaction_ExecShopsSeq_MSSQL(shop.seq);
            }
            catch (Exception ex)
            {
                TempData["MemberResult"] = ex.ToString();
                return RedirectToAction("Create", "CarMgt");
            }

            TempData["MemberResult"] = "OKCreate";
            return RedirectToAction("Index", "CarMgt");
        }

        //上架中
        public ActionResult Update(int seq)
        {
            if (!rightSrv.checkRight("CarMgt", "Update", User.Identity.Name))
            {
                return RedirectToAction("Index", "Home");
            }

            if (!rightSrv.checkRight("CarMgt", "UpdateInProgress", User.Identity.Name))
            {
                TempData["UpdateRight"] = "False";
            }
            else
            {
                TempData["UpdateRight"] = "True";
            }

            Shops shop = carShopEntities.Shops.Where(x => x.seq == seq).FirstOrDefault();
            CarShopViewModel model = new CarShopViewModel();
            bool reviewUpPermission = false;
            bool reviewDownPermission = false;
            string company = string.Empty;
            string department = string.Empty;
            string roleName = string.Empty;
            var user = carShopEntities.AspNetUsers.Where(x => x.Email == User.Identity.Name).FirstOrDefault();
            if (user != null)
            {
                int role = carShopEntities.UserRoles.Where(x => x.userName == user.NameIdentifier).Select(x => x.userRole).FirstOrDefault() ?? 0;
                roleName = carShopEntities.Roles.Where(x => x.seq == role).Select(x => x.RoleName).FirstOrDefault() ?? string.Empty;

                var roleMenus = carShopEntities.RoleMenus.Where(x => x.roleId == role).ToList();
                int carUpMenuId = carShopEntities.Menus.Where(x => x.menuName == "車輛管理-車輛上架審核(AE)").Select(x => x.seq).FirstOrDefault();
                int carDownMenuId = carShopEntities.Menus.Where(x => x.menuName == "車輛管理-車輛成交下架審核(AE)").Select(x => x.seq).FirstOrDefault();
                int carDownMenuId2 = carShopEntities.Menus.Where(x => x.menuName == "車輛管理-車輛非成交下架審核(AE)").Select(x => x.seq).FirstOrDefault();
                foreach (var menu in roleMenus)
                {
                    if (menu.menuId == carUpMenuId)
                    {
                        reviewUpPermission = true;
                    }

                    if (menu.menuId == carDownMenuId || menu.menuId == carDownMenuId2)
                    {
                        reviewDownPermission = true;
                    }
                }

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
            dealerOptions = dealers.Select(x => x.dealerName).Distinct().ToList();

            List<string> strongholdOptions = new List<string>();
            strongholdOptions = dealers.Where(x => x.dealerName == shop.dealer).Select(x => x.businessOffice).Distinct().ToList();

            TempData["dealerOptions"] = dealerOptions;
            TempData["strongholdOptions"] = strongholdOptions;

            model.seq = shop.seq;
            model.ShopNo = shop.ShopNo;
            model.createTime = shop.createTime;
            model.updateTime = shop.updateTime;
            model.area = shop.area;
            model.dealer = shop.dealer;
            model.stronghold = shop.stronghold;
            model.carType = shop.carType;
            model.carModel = shop.carModel;
            model.yearOfManufacture = shop.yearOfManufacture;
            model.monthOfManufacture = shop.monthOfManufacture;
            model.displacement = shop.displacement;
            model.milage = shop.milage;
            model.outerColor = shop.outerColor;
            model.fuelType = shop.fuelType;
            model.driveMode = shop.driveMode;
            model.outEquip = shop.outEquip.Split(',').ToList();
            model.feature = shop.feature.Split(',').ToList();
            model.horsepower = shop.horsepower;
            model.price = shop.price;
            model.description = shop.description;
            model.member = shop.member;
            model.consultant = shop.consultant;
            model.finalPrice = shop.finalPrice;
            model.formalShopNo = shop.formalShopNo;
            model.status = shop.status;

            List<string> dealerPersons = carShopEntities.DealerPersons.Where(x => x.dealer == shop.dealer && x.businessOffice == shop.stronghold).Select(x => x.name).ToList();
            TempData["dealerPersons"] = dealerPersons;            

            model.carCondition1 = fileService.GetRealUrl(shop.carCondition1);
            model.carCondition2 = fileService.GetRealUrl(shop.carCondition2);
            model.carCondition3 = fileService.GetRealUrl(shop.carCondition3);
            model.carCondition4 = fileService.GetRealUrl(shop.carCondition4);
            model.carCondition5 = fileService.GetRealUrl(shop.carCondition5);
            model.carCondition6 = fileService.GetRealUrl(shop.carCondition6);
            model.carCondition7 = fileService.GetRealUrl(shop.carCondition7);
            model.carCondition8 = fileService.GetRealUrl(shop.carCondition8);
            model.carCondition9 = fileService.GetRealUrl(shop.carCondition9);
            model.carCondition10 = fileService.GetRealUrl(shop.carCondition10);
            model.inspectionTable = fileService.GetRealUrl(shop.inspectionTable);
            model.contract = fileService.GetRealUrl(shop.contract);

            var prepaid = carShopEntities.Prepaid.Where(x => x.shopNo == model.ShopNo).FirstOrDefault();
            model.contact = string.Empty;
            if (prepaid == null)
            {
                model.contact = "";
            }
            else if (prepaid.paidStatus == "已付訂金")
            {
                model.contact = "預付保留";
            }
            else if (prepaid.paidStatus == "已退訂")
            {
                model.contact = "退訂";
            }

            if (model.status == "上架申請" && reviewUpPermission)
            {
                return RedirectToAction("Update1", "CarMgt", new { seq = seq });
            }
            else if (model.status == "上架申請" && !reviewUpPermission)
            {
                return RedirectToAction("Update2", "CarMgt", new { seq = seq });
            }
            else if (model.status == "草稿")
            {
                return RedirectToAction("Update4", "CarMgt", new { seq = seq });
            }
            else if (model.status == "上架中")
            {
                return View(model);
            }
            else if ((model.status == "成交下架申請" || model.status == "非成交下架申請") && reviewDownPermission)
            {
                return RedirectToAction("Update3", "CarMgt", new { seq = seq });
            }
            else if (model.status == "下架申請" && !reviewDownPermission)
            {
                return RedirectToAction("Update2", "CarMgt", new { seq = seq });
            }
            else if (model.status == "成交下架")
            {
                return RedirectToAction("Update5", "CarMgt", new { seq = seq });
            }

            return View(model);
        }

        //上架申請中 - 經銷商管理員審核
        public ActionResult Update1(int seq)
        {
            if (!rightSrv.checkRight("CarMgt", "Update", User.Identity.Name))
            {
                return RedirectToAction("Index", "Home");
            }

            if (!rightSrv.checkRight("CarMgt", "UpdateInProgress", User.Identity.Name))
            {
                TempData["UpdateRight"] = "False";
            }
            else
            {
                TempData["UpdateRight"] = "True";
            }

            if (!rightSrv.checkRight("CarMgt", "Delete", User.Identity.Name))
            {
                TempData["DeleteRight"] = "False";
            }
            else
            {
                TempData["DeleteRight"] = "True";
            }

            Shops shop = carShopEntities.Shops.Where(x => x.seq == seq).FirstOrDefault();
            CarShopViewModel model = new CarShopViewModel();
            TempData["today"] = DateTime.Today.ToString("yyyy/MM/dd");

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
            dealerOptions = dealers.Select(x => x.dealerName).Distinct().ToList();

            List<string> strongholdOptions = new List<string>();
            strongholdOptions = dealers.Where(x => x.dealerName == shop.dealer).Select(x => x.businessOffice).Distinct().ToList();

            TempData["dealerOptions"] = dealerOptions;
            TempData["strongholdOptions"] = strongholdOptions;

            model.seq = shop.seq;
            model.ShopNo = shop.ShopNo;
            model.createTime = shop.createTime;
            model.updateTime = shop.updateTime;
            model.area = shop.area;
            model.dealer = shop.dealer;
            model.stronghold = shop.stronghold;
            model.carType = shop.carType;
            model.carModel = shop.carModel;
            model.yearOfManufacture = shop.yearOfManufacture;
            model.monthOfManufacture = shop.monthOfManufacture;
            model.displacement = shop.displacement;
            model.milage = shop.milage;
            model.outerColor = shop.outerColor;
            model.fuelType = shop.fuelType;
            model.driveMode = shop.driveMode;
            model.outEquip = shop.outEquip.Split(',').ToList();
            model.feature = shop.feature.Split(',').ToList();
            model.horsepower = shop.horsepower;
            model.price = shop.price;
            model.description = shop.description;
            model.member = shop.member;
            model.consultant = shop.consultant;
            model.finalPrice = shop.finalPrice;
            model.formalShopNo = shop.formalShopNo;
            model.status = shop.status;

            List<string> dealerPersons = carShopEntities.DealerPersons.Where(x => x.dealer == shop.dealer && x.businessOffice == shop.stronghold).Select(x => x.name).ToList();
            TempData["dealerPersons"] = dealerPersons;

            model.carCondition1 = fileService.GetRealUrl(shop.carCondition1);
            model.carCondition2 = fileService.GetRealUrl(shop.carCondition2);
            model.carCondition3 = fileService.GetRealUrl(shop.carCondition3);
            model.carCondition4 = fileService.GetRealUrl(shop.carCondition4);
            model.carCondition5 = fileService.GetRealUrl(shop.carCondition5);
            model.carCondition6 = fileService.GetRealUrl(shop.carCondition6);
            model.carCondition7 = fileService.GetRealUrl(shop.carCondition7);
            model.carCondition8 = fileService.GetRealUrl(shop.carCondition8);
            model.carCondition9 = fileService.GetRealUrl(shop.carCondition9);
            model.carCondition10 = fileService.GetRealUrl(shop.carCondition10);
            model.inspectionTable = fileService.GetRealUrl(shop.inspectionTable);
            model.contract = fileService.GetRealUrl(shop.contract);

            int addPicStartCount = 5;
            addPicStartCount = string.IsNullOrEmpty(model.carCondition6) ? addPicStartCount : addPicStartCount + 1;
            addPicStartCount = string.IsNullOrEmpty(model.carCondition7) ? addPicStartCount : addPicStartCount + 1;
            addPicStartCount = string.IsNullOrEmpty(model.carCondition8) ? addPicStartCount : addPicStartCount + 1;
            addPicStartCount = string.IsNullOrEmpty(model.carCondition9) ? addPicStartCount : addPicStartCount++;
            addPicStartCount = string.IsNullOrEmpty(model.carCondition10) ? addPicStartCount : addPicStartCount + 1;
            TempData["addPicStartCount"] = addPicStartCount.ToString();
            return View(model);
        }

        //上下架申請中 - 據點管理員查看
        public ActionResult Update2(int seq)
        {
            if (!rightSrv.checkRight("CarMgt", "Update", User.Identity.Name))
            {
                return RedirectToAction("Index", "Home");
            }

            if (!rightSrv.checkRight("CarMgt", "UpdateInProgress", User.Identity.Name))
            {
                TempData["UpdateRight"] = "False";
            }
            else
            {
                TempData["UpdateRight"] = "True";
            }
            Shops shop = carShopEntities.Shops.Where(x => x.seq == seq).FirstOrDefault();
            CarShopViewModel model = new CarShopViewModel();
            TempData["today"] = DateTime.Today.ToString("yyyy/MM/dd");

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
            dealerOptions = dealers.Select(x => x.dealerName).Distinct().ToList();

            List<string> strongholdOptions = new List<string>();
            strongholdOptions = dealers.Where(x => x.dealerName == shop.dealer).Select(x => x.businessOffice).Distinct().ToList();

            TempData["dealerOptions"] = dealerOptions;
            TempData["strongholdOptions"] = strongholdOptions;

            model.status = shop.status;
            model.seq = shop.seq;
            model.ShopNo = shop.ShopNo;
            model.createTime = shop.createTime;
            model.updateTime = shop.updateTime;
            model.area = shop.area;
            model.dealer = shop.dealer;
            model.stronghold = shop.stronghold;
            model.carType = shop.carType;
            model.carModel = shop.carModel;
            model.yearOfManufacture = shop.yearOfManufacture;
            model.monthOfManufacture = shop.monthOfManufacture;
            model.displacement = shop.displacement;
            model.milage = shop.milage;
            model.outerColor = shop.outerColor;
            model.fuelType = shop.fuelType;
            model.driveMode = shop.driveMode;
            model.outEquip = shop.outEquip.Split(',').ToList();
            model.feature = shop.feature.Split(',').ToList();
            model.horsepower = shop.horsepower;
            model.price = shop.price;
            model.description = shop.description;
            model.member = shop.member;
            model.consultant = shop.consultant;
            model.finalPrice = shop.finalPrice;
            model.formalShopNo = shop.formalShopNo;

            List<string> dealerPersons = carShopEntities.DealerPersons.Where(x => x.dealer == shop.dealer && x.businessOffice == shop.stronghold).Select(x => x.name).ToList();
            TempData["dealerPersons"] = dealerPersons;
            model.carCondition1 = fileService.GetRealUrl(shop.carCondition1);
            model.carCondition2 = fileService.GetRealUrl(shop.carCondition2);
            model.carCondition3 = fileService.GetRealUrl(shop.carCondition3);
            model.carCondition4 = fileService.GetRealUrl(shop.carCondition4);
            model.carCondition5 = fileService.GetRealUrl(shop.carCondition5);
            model.carCondition6 = fileService.GetRealUrl(shop.carCondition6);
            model.carCondition7 = fileService.GetRealUrl(shop.carCondition7);
            model.carCondition8 = fileService.GetRealUrl(shop.carCondition8);
            model.carCondition9 = fileService.GetRealUrl(shop.carCondition9);
            model.carCondition10 = fileService.GetRealUrl(shop.carCondition10);
            model.inspectionTable = fileService.GetRealUrl(shop.inspectionTable);
            model.contract = fileService.GetRealUrl(shop.contract);

            return View(model);
        }

        //下架申請中 - 經銷商管理員審核
        public ActionResult Update3(int seq)
        {
            if (!rightSrv.checkRight("CarMgt", "Update", User.Identity.Name))
            {
                return RedirectToAction("Index", "Home");
            }

            if (!rightSrv.checkRight("CarMgt", "UpdateInProgress", User.Identity.Name))
            {
                TempData["UpdateRight"] = "False";
            }
            else
            {
                TempData["UpdateRight"] = "True";
            }

            if (!rightSrv.checkRight("CarMgt", "Delete", User.Identity.Name))
            {
                TempData["DeleteRight"] = "False";
            }
            else
            {
                TempData["DeleteRight"] = "True";
            }

            Shops shop = carShopEntities.Shops.Where(x => x.seq == seq).FirstOrDefault();
            CarShopViewModel model = new CarShopViewModel();
            TempData["today"] = DateTime.Today.ToString("yyyy/MM/dd");

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
            dealerOptions = dealers.Select(x => x.dealerName).Distinct().ToList();

            List<string> strongholdOptions = new List<string>();
            strongholdOptions = dealers.Where(x => x.dealerName == shop.dealer).Select(x => x.businessOffice).Distinct().ToList();

            TempData["dealerOptions"] = dealerOptions;
            TempData["strongholdOptions"] = strongholdOptions;

            model.status = shop.status;
            model.seq = shop.seq;
            model.ShopNo = shop.ShopNo;
            model.createTime = shop.createTime;
            model.updateTime = shop.updateTime;
            model.area = shop.area;
            model.dealer = shop.dealer;
            model.stronghold = shop.stronghold;
            model.carType = shop.carType;
            model.carModel = shop.carModel;
            model.yearOfManufacture = shop.yearOfManufacture;
            model.monthOfManufacture = shop.monthOfManufacture;
            model.displacement = shop.displacement;
            model.milage = shop.milage;
            model.outerColor = shop.outerColor;
            model.fuelType = shop.fuelType;
            model.driveMode = shop.driveMode;
            model.outEquip = shop.outEquip.Split(',').ToList();
            model.feature = shop.feature.Split(',').ToList();
            model.horsepower = shop.horsepower;
            model.price = shop.price;
            model.description = shop.description;
            model.member = shop.member;
            model.consultant = shop.consultant;
            model.finalPrice = shop.finalPrice;
            model.formalShopNo = shop.formalShopNo;

            List<string> dealerPersons = carShopEntities.DealerPersons.Where(x => x.dealer == shop.dealer && x.businessOffice == shop.stronghold).Select(x => x.name).ToList();
            TempData["dealerPersons"] = dealerPersons;
            model.carCondition1 = fileService.GetRealUrl(shop.carCondition1);
            model.carCondition2 = fileService.GetRealUrl(shop.carCondition2);
            model.carCondition3 = fileService.GetRealUrl(shop.carCondition3);
            model.carCondition4 = fileService.GetRealUrl(shop.carCondition4);
            model.carCondition5 = fileService.GetRealUrl(shop.carCondition5);
            model.carCondition6 = fileService.GetRealUrl(shop.carCondition6);
            model.carCondition7 = fileService.GetRealUrl(shop.carCondition7);
            model.carCondition8 = fileService.GetRealUrl(shop.carCondition8);
            model.carCondition9 = fileService.GetRealUrl(shop.carCondition9);
            model.carCondition10 = fileService.GetRealUrl(shop.carCondition10);
            model.inspectionTable = fileService.GetRealUrl(shop.inspectionTable);
            model.contract = fileService.GetRealUrl(shop.contract);

            int addPicStartCount = 5;
            addPicStartCount = string.IsNullOrEmpty(model.carCondition6) ? addPicStartCount : addPicStartCount + 1;
            addPicStartCount = string.IsNullOrEmpty(model.carCondition7) ? addPicStartCount : addPicStartCount + 1;
            addPicStartCount = string.IsNullOrEmpty(model.carCondition8) ? addPicStartCount : addPicStartCount + 1;
            addPicStartCount = string.IsNullOrEmpty(model.carCondition9) ? addPicStartCount : addPicStartCount++;
            addPicStartCount = string.IsNullOrEmpty(model.carCondition10) ? addPicStartCount : addPicStartCount + 1;
            TempData["addPicStartCount"] = addPicStartCount.ToString();
            return View(model);
        }

        //草稿
        public ActionResult Update4(int seq)
        {
            if (!rightSrv.checkRight("CarMgt", "Update", User.Identity.Name))
            {
                return RedirectToAction("Index", "Home");
            }

            if (!rightSrv.checkRight("CarMgt", "UpdateInProgress", User.Identity.Name))
            {
                TempData["UpdateRight"] = "False";
            }
            else
            {
                TempData["UpdateRight"] = "True";
            }

            if (!rightSrv.checkRight("CarMgt", "Delete", User.Identity.Name))
            {
                TempData["DeleteRight"] = "False";
            }
            else
            {
                TempData["DeleteRight"] = "True";
            }

            Shops shop = carShopEntities.Shops.Where(x => x.seq == seq).FirstOrDefault();
            CarShopViewModel model = new CarShopViewModel();
            TempData["today"] = DateTime.Today.ToString("yyyy/MM/dd");

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
            dealerOptions = dealers.Select(x => x.dealerName).Distinct().ToList();

            List<string> strongholdOptions = new List<string>();
            strongholdOptions = dealers.Where(x => x.dealerName == shop.dealer).Select(x => x.businessOffice).Distinct().ToList();

            TempData["dealerOptions"] = dealerOptions;
            TempData["strongholdOptions"] = strongholdOptions;
            model.status = shop.status;
            model.seq = shop.seq;
            model.ShopNo = shop.ShopNo;
            model.createTime = shop.createTime;
            model.updateTime = shop.updateTime;
            model.area = shop.area;
            model.dealer = shop.dealer;
            model.stronghold = shop.stronghold;
            model.carType = shop.carType;
            model.carModel = shop.carModel;
            model.yearOfManufacture = shop.yearOfManufacture;
            model.monthOfManufacture = shop.monthOfManufacture;
            model.displacement = shop.displacement;
            model.milage = shop.milage;
            model.outerColor = shop.outerColor;
            model.fuelType = shop.fuelType;
            model.driveMode = shop.driveMode;
            model.outEquip = shop.outEquip.Split(',').ToList();
            model.feature = shop.feature.Split(',').ToList();
            model.horsepower = shop.horsepower;
            model.price = shop.price;
            model.description = shop.description;
            model.member = shop.member;
            model.consultant = shop.consultant;
            model.finalPrice = shop.finalPrice;
            model.formalShopNo = shop.formalShopNo;

            List<string> dealerPersons = carShopEntities.DealerPersons.Where(x => x.dealer == shop.dealer && x.businessOffice == shop.stronghold).Select(x => x.name).ToList();
            TempData["dealerPersons"] = dealerPersons;
            
            model.carCondition1 = fileService.GetRealUrl(shop.carCondition1);
            model.carCondition2 = fileService.GetRealUrl(shop.carCondition2);
            model.carCondition3 = fileService.GetRealUrl(shop.carCondition3);
            model.carCondition4 = fileService.GetRealUrl(shop.carCondition4);
            model.carCondition5 = fileService.GetRealUrl(shop.carCondition5);
            model.carCondition6 = fileService.GetRealUrl(shop.carCondition6);
            model.carCondition7 = fileService.GetRealUrl(shop.carCondition7);
            model.carCondition8 = fileService.GetRealUrl(shop.carCondition8);
            model.carCondition9 = fileService.GetRealUrl(shop.carCondition9);
            model.carCondition10 = fileService.GetRealUrl(shop.carCondition10);
            model.inspectionTable = fileService.GetRealUrl(shop.inspectionTable);
            model.contract = fileService.GetRealUrl(shop.contract);

            int addPicStartCount = 5;
            addPicStartCount = string.IsNullOrEmpty(model.carCondition6) ? addPicStartCount : addPicStartCount + 1;
            addPicStartCount = string.IsNullOrEmpty(model.carCondition7) ? addPicStartCount : addPicStartCount + 1;
            addPicStartCount = string.IsNullOrEmpty(model.carCondition8) ? addPicStartCount : addPicStartCount + 1;
            addPicStartCount = string.IsNullOrEmpty(model.carCondition9) ? addPicStartCount : addPicStartCount++;
            addPicStartCount = string.IsNullOrEmpty(model.carCondition10) ? addPicStartCount : addPicStartCount + 1;
            TempData["addPicStartCount"] = addPicStartCount.ToString();
            return View(model);
        }

        //下架中
        public ActionResult Update5(int seq)
        {
            if (!rightSrv.checkRight("CarMgt", "Update", User.Identity.Name))
            {
                return RedirectToAction("Index", "Home");
            }

            if (!rightSrv.checkRight("CarMgt", "UpdateInProgress", User.Identity.Name))
            {
                TempData["UpdateRight"] = "False";
            }
            else
            {
                TempData["UpdateRight"] = "True";
            }
            Shops shop = carShopEntities.Shops.Where(x => x.seq == seq).FirstOrDefault();
            CarShopViewModel model = new CarShopViewModel();
            TempData["today"] = DateTime.Today.ToString("yyyy/MM/dd");

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
            dealerOptions = dealers.Select(x => x.dealerName).Distinct().ToList();

            List<string> strongholdOptions = new List<string>();
            strongholdOptions = dealers.Where(x => x.dealerName == shop.dealer).Select(x => x.businessOffice).Distinct().ToList();

            TempData["dealerOptions"] = dealerOptions;
            TempData["strongholdOptions"] = strongholdOptions;
            model.status = shop.status;
            model.seq = shop.seq;
            model.ShopNo = shop.ShopNo;
            model.createTime = shop.createTime;
            model.updateTime = shop.updateTime;
            model.area = shop.area;
            model.dealer = shop.dealer;
            model.stronghold = shop.stronghold;
            model.carType = shop.carType;
            model.carModel = shop.carModel;
            model.yearOfManufacture = shop.yearOfManufacture;
            model.monthOfManufacture = shop.monthOfManufacture;
            model.displacement = shop.displacement;
            model.milage = shop.milage;
            model.outerColor = shop.outerColor;
            model.fuelType = shop.fuelType;
            model.driveMode = shop.driveMode;
            model.outEquip = shop.outEquip.Split(',').ToList();
            model.feature = shop.feature.Split(',').ToList();
            model.horsepower = shop.horsepower;
            model.price = shop.price;
            model.description = shop.description;
            model.member = shop.member;
            model.consultant = shop.consultant;
            model.finalPrice = shop.finalPrice;
            model.formalShopNo = shop.formalShopNo;

            List<string> dealerPersons = carShopEntities.DealerPersons.Where(x => x.dealer == shop.dealer && x.businessOffice == shop.stronghold).Select(x => x.name).ToList();
            TempData["dealerPersons"] = dealerPersons;
            model.carCondition1 = fileService.GetRealUrl(shop.carCondition1);
            model.carCondition2 = fileService.GetRealUrl(shop.carCondition2);
            model.carCondition3 = fileService.GetRealUrl(shop.carCondition3);
            model.carCondition4 = fileService.GetRealUrl(shop.carCondition4);
            model.carCondition5 = fileService.GetRealUrl(shop.carCondition5);
            model.carCondition6 = fileService.GetRealUrl(shop.carCondition6);
            model.carCondition7 = fileService.GetRealUrl(shop.carCondition7);
            model.carCondition8 = fileService.GetRealUrl(shop.carCondition8);
            model.carCondition9 = fileService.GetRealUrl(shop.carCondition9);
            model.carCondition10 = fileService.GetRealUrl(shop.carCondition10);
            model.inspectionTable = fileService.GetRealUrl(shop.inspectionTable);
            model.contract = fileService.GetRealUrl(shop.contract);

            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult UpdateInProgress(CarShopViewModel model)
        {
            if (!rightSrv.checkRight("CarMgt", "UpdateInProgress", User.Identity.Name))
            {
                return RedirectToAction("Index", "Home");
            }
            string file = string.Empty;
            Shops shop = carShopEntities.Shops.Where(x => x.seq == model.seq).FirstOrDefault();

            string innerMailIp = ConfigurationManager.AppSettings["mailServerIp"];
            string innerMailPort = ConfigurationManager.AppSettings["mailServerPort"];
            var carMgtService = new Services.CarMgtService();


            var count = 0;
            if ((Request.Files["carCondition1"] != null && Request.Files["carCondition1"].ContentLength > 0) || string.IsNullOrEmpty(shop.carCondition1) == false) count++;
            if ((Request.Files["carCondition2"] != null && Request.Files["carCondition2"].ContentLength > 0) || string.IsNullOrEmpty(shop.carCondition2) == false) count++;
            if ((Request.Files["carCondition3"] != null && Request.Files["carCondition3"].ContentLength > 0) || string.IsNullOrEmpty(shop.carCondition3) == false) count++;
            if ((Request.Files["carCondition4"] != null && Request.Files["carCondition4"].ContentLength > 0) || string.IsNullOrEmpty(shop.carCondition4) == false) count++;
            if ((Request.Files["carCondition5"] != null && Request.Files["carCondition5"].ContentLength > 0) || string.IsNullOrEmpty(shop.carCondition5) == false) count++;
            if ((Request.Files["carCondition6"] != null && Request.Files["carCondition6"].ContentLength > 0) || string.IsNullOrEmpty(shop.carCondition6) == false) count++;
            if ((Request.Files["carCondition7"] != null && Request.Files["carCondition7"].ContentLength > 0) || string.IsNullOrEmpty(shop.carCondition7) == false) count++;
            if ((Request.Files["carCondition8"] != null && Request.Files["carCondition8"].ContentLength > 0) || string.IsNullOrEmpty(shop.carCondition8) == false) count++;
            if ((Request.Files["carCondition9"] != null && Request.Files["carCondition9"].ContentLength > 0) || string.IsNullOrEmpty(shop.carCondition9) == false) count++;
            if ((Request.Files["carCondition10"] != null && Request.Files["carCondition10"].ContentLength > 0) || string.IsNullOrEmpty(shop.carCondition10) == false) count++;
            if (count < 5 && shop.status == "草稿" && model.action == "儲存")
            {
                //就讓他存草稿，其餘都必須要大於5張
            }
            else if (count < 5)
            {
                var actionName = Request.UrlReferrer.Segments[Request.UrlReferrer.Segments.Length - 1];
                TempData["MemberResult"] = "最少需要上傳5張照片";
                return RedirectToAction(actionName, "CarMgt", new { seq = model.seq });
            }
            else
            {
                //超過5張就可以
            }

            if (model.action == "成交下架" || model.action == "非成交下架" || model.action == "重新上架" || shop.status == "上架中")
            {
                if (model.action == "成交下架")
                {
                    if (string.IsNullOrEmpty(model.member))
                    {
                        TempData["MemberResult"] = "會員姓名為必填";
                        return RedirectToAction("Update", "CarMgt", new { seq = model.seq });
                    }

                    if (string.IsNullOrEmpty(model.consultant))
                    {
                        TempData["MemberResult"] = "服務銷售顧問為必填";
                        return RedirectToAction("Update", "CarMgt", new { seq = model.seq });
                    }

                    if (string.IsNullOrEmpty(model.formalShopNo))
                    {
                        TempData["MemberResult"] = "正式訂單編號為必填";
                        return RedirectToAction("Update", "CarMgt", new { seq = model.seq });
                    }

                    if (model.finalPrice == 0)
                    {
                        TempData["MemberResult"] = "成交價為必填";
                        return RedirectToAction("Update", "CarMgt", new { seq = model.seq });
                    }

                    if (Request.Files["contract"].ContentLength == 0)
                    {
                        TempData["MemberResult"] = "合約書為必填";
                        return RedirectToAction("Update", "CarMgt", new { seq = model.seq });
                    }
                }

                if (shop != null)
                {
                    shop.updateTime = DateTime.Now;
                    shop.member = model.member;
                    shop.consultant = model.consultant;
                    shop.price = model.price;
                    shop.formalShopNo = model.formalShopNo;
                    shop.finalPrice = model.finalPrice;

                    switch (model.action)
                    {
                        case "上架申請":
                            shop.status = "上架申請";

                            carMgtService.MailSend_01(shop, User.Identity.Name);


                            //email 通知經銷商管理員                            
                            GetUserAndSendMail(shop, DataModels.EnumRoleName.經銷商管理人員, 2);
                            break;
                        case "上架通過":
                            shop.status = "上架中";

                            //email 通知品牌管理員
                            GetUserAndSendMail(shop, DataModels.EnumRoleName.品牌管理人員, 3);

                            //email 通知據點管理人員
                            GetUserAndSendMail(shop, DataModels.EnumRoleName.據點管理人員, 4);
                            break;
                        case "上架退回":
                            shop.status = "草稿";

                            //email 通知據點管理人員
                            GetUserAndSendMail(shop, DataModels.EnumRoleName.據點管理人員, 5);
                            break;
                        case "成交下架通過":
                            shop.status = "成交下架";

                            //email 通知據點管理人員
                            GetUserAndSendMail(shop, DataModels.EnumRoleName.據點管理人員, 6);
                            break;
                        case "非成交下架通過":
                            shop.status = "草稿";

                            //email 通知據點管理人員
                            GetUserAndSendMail(shop, DataModels.EnumRoleName.據點管理人員, 7);
                            break;
                        case "下架退回":
                            shop.status = "上架中";

                            //email 通知據點管理人員
                            GetUserAndSendMail(shop, DataModels.EnumRoleName.據點管理人員, 8);
                            break;
                        case "成交下架":
                            shop.status = "成交下架申請";

                            carMgtService.MailSend_09(shop, User.Identity.Name);

                            //email 通知經銷商管理員
                            GetUserAndSendMail(shop, DataModels.EnumRoleName.經銷商管理人員, 10);
                            break;
                        case "非成交下架":
                            shop.status = "非成交下架申請";

                            carMgtService.MailSend_11(shop, User.Identity.Name);

                            //email 通知經銷商管理員
                            GetUserAndSendMail(shop, DataModels.EnumRoleName.經銷商管理人員, 12);
                            break;
                        case "重新上架":
                            shop.status = "上架中";
                            shop.member = "";
                            shop.consultant = "";
                            shop.price = model.price;
                            shop.formalShopNo = "";
                            shop.finalPrice = 0;
                            shop.contract = "";
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
                                case "contract":
                                    shop.contract = SaveAsLocal(tmpP);
                                    break;

                                default:
                                    break;
                            }
                        }
                    }
                }
            }
            else
            {

                if (model.action != "儲存")
                {
                    if (string.IsNullOrEmpty(model.carType))
                    {
                        TempData["MemberResult"] = "車種為必填";
                        return RedirectToAction("Update", "CarMgt", new { seq = model.seq });
                    }

                    if (string.IsNullOrEmpty(model.carModel))
                    {
                        TempData["MemberResult"] = "車型為必填";
                        return RedirectToAction("Update", "CarMgt", new { seq = model.seq });
                    }

                    if (model.yearOfManufacture == null)
                    {
                        TempData["MemberResult"] = "出廠年月為必填";
                        return RedirectToAction("Update", "CarMgt", new { seq = model.seq });
                    }

                    if (model.monthOfManufacture == null)
                    {
                        TempData["MemberResult"] = "出廠年月為必填";
                        return RedirectToAction("Update", "CarMgt", new { seq = model.seq });
                    }

                    if (string.IsNullOrEmpty(model.outerColor))
                    {
                        TempData["MemberResult"] = "車色為必填";
                        return RedirectToAction("Update", "CarMgt", new { seq = model.seq });
                    }

                    if (string.IsNullOrEmpty(model.horsepower))
                    {
                        TempData["MemberResult"] = "馬力為必填";
                        return RedirectToAction("Update", "CarMgt", new { seq = model.seq });
                    }

                    if (model.outEquip.Count() == 0)
                    {
                        TempData["MemberResult"] = "內外裝配備為必填";
                        return RedirectToAction("Update", "CarMgt", new { seq = model.seq });
                    }

                    if (model.feature.Count() == 0)
                    {
                        TempData["MemberResult"] = "安全重點功能為必填";
                        return RedirectToAction("Update", "CarMgt", new { seq = model.seq });
                    }

                    if (string.IsNullOrEmpty(model.fuelType))
                    {
                        TempData["MemberResult"] = "燃料為必填";
                        return RedirectToAction("Update", "CarMgt", new { seq = model.seq });
                    }

                    if (string.IsNullOrEmpty(model.driveMode))
                    {
                        TempData["MemberResult"] = "驅動方式為必填";
                        return RedirectToAction("Update", "CarMgt", new { seq = model.seq });
                    }

                    if (string.IsNullOrEmpty(model.displacement))
                    {
                        TempData["MemberResult"] = "排氣輛為必填";
                        return RedirectToAction("Update", "CarMgt", new { seq = model.seq });
                    }

                    if (string.IsNullOrEmpty(model.area))
                    {
                        TempData["MemberResult"] = "地區為必填";
                        return RedirectToAction("Update", "CarMgt", new { seq = model.seq });
                    }

                    if (model.milage == 0)
                    {
                        TempData["MemberResult"] = "里程數為必填";
                        return RedirectToAction("Update", "CarMgt", new { seq = model.seq });
                    }

                    if (string.IsNullOrEmpty(model.dealer))
                    {
                        TempData["MemberResult"] = "營業所為必填";
                        return RedirectToAction("Update", "CarMgt", new { seq = model.seq });
                    }

                    if (string.IsNullOrEmpty(model.stronghold))
                    {
                        TempData["MemberResult"] = "中古車營業據點為必填";
                        return RedirectToAction("Update", "CarMgt", new { seq = model.seq });
                    }

                    if (model.price == 0)
                    {
                        TempData["MemberResult"] = "售價為必填";
                        return RedirectToAction("Update", "CarMgt", new { seq = model.seq });
                    }


                    if (string.IsNullOrEmpty(model.description))
                    {
                        TempData["MemberResult"] = "推薦理由為必填";
                        return RedirectToAction("Update", "CarMgt", new { seq = model.seq });
                    }
                }

                if (model.action == "上架申請")
                {
                    if (IsOver_CarConditionLimit(shop, 5))
                    {
                        TempData["MemberResult"] = "最少5張照片";
                        return RedirectToAction("Update", "CarMgt", new { seq = model.seq });
                    }
                    if (IsUpload_InspectionTable(shop) == false)
                    {
                        TempData["MemberResult"] = "請上傳查定表";
                        return RedirectToAction("Update", "CarMgt", new { seq = model.seq });
                    }
                }

                if (shop != null)
                {
                    shop.carType = model.carType;
                    shop.carModel = model.carModel;
                    shop.innerColor = model.innerColor;
                    shop.outerColor = model.outerColor;
                    shop.horsepower = model.horsepower;
                    shop.transmissionType = model.transmissionType;
                    shop.ListingDate = model.ListingDate;
                    shop.fuelType = model.fuelType;
                    shop.displacement = model.displacement;
                    shop.milage = model.milage;
                    shop.driveMode = model.driveMode;
                    shop.outEquip = model.outEquip == null ? string.Empty : string.Join(",", model.outEquip);
                    shop.feature = model.feature == null ? string.Empty : string.Join(",", model.feature);
                    shop.stronghold = model.stronghold;
                    shop.area = model.area;
                    shop.dealer = model.dealer;
                    shop.brand = brandSrv.getBrand(User.Identity.Name);
                    shop.price = model.price;
                    shop.yearOfManufacture = model.yearOfManufacture;
                    shop.monthOfManufacture = model.monthOfManufacture;
                    shop.salesRep = model.dealer;
                    if (shop.salesRep == null) shop.salesRep = string.Empty;
                    shop.description = model.description;
                    shop.updateTime = DateTime.Now;
                    shop.member = model.member;
                    shop.consultant = model.consultant;
                    shop.process = model.process;
                    shop.cashStatus = model.cashStatus;
                    shop.finalPrice = model.finalPrice;


                    switch (model.action)
                    {
                        case "上架申請":
                            shop.status = "上架申請";

                            carMgtService.MailSend_13(shop, User.Identity.Name);

                            //email 通知經銷商管理員
                            GetUserAndSendMail(shop, DataModels.EnumRoleName.經銷商管理人員, 14);

                            break;
                        case "上架通過":
                            shop.status = "上架中";
                            //email 通知品牌管理員
                            GetUserAndSendMail(shop, DataModels.EnumRoleName.品牌管理人員, 15);

                            //email 通知據點管理人員
                            GetUserAndSendMail(shop, DataModels.EnumRoleName.據點管理人員, 16);
                            break;
                        case "上架退回":
                            shop.status = "草稿";
                            //email 通知據點管理人員
                            GetUserAndSendMail(shop, DataModels.EnumRoleName.據點管理人員, 17);
                            break;
                        case "成交下架通過":
                            shop.status = "已下架";

                            //email 通知據點管理人員
                            GetUserAndSendMail(shop, DataModels.EnumRoleName.據點管理人員, 18);
                            break;
                        case "非成交下架通過":
                            shop.status = "草稿";

                            //email 通知據點管理人員
                            GetUserAndSendMail(shop, DataModels.EnumRoleName.據點管理人員, 19);
                            break;
                        case "下架退回":
                            shop.status = "上架中";

                            //email 通知據點管理人員
                            GetUserAndSendMail(shop, DataModels.EnumRoleName.據點管理人員, 20);
                            break;
                        case "成交下架":
                            shop.status = "下架申請";

                            carMgtService.MailSend_21(shop, User.Identity.Name);


                            //email 通知經銷商管理員
                            GetUserAndSendMail(shop, DataModels.EnumRoleName.經銷商管理人員, 22);
                            break;
                        case "非成交下架":
                            shop.status = "下架申請";

                            carMgtService.MailSend_23(shop, User.Identity.Name);


                            //email 通知經銷商管理員
                            GetUserAndSendMail(shop, DataModels.EnumRoleName.經銷商管理人員, 24);
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
                                    shop.carCondition1 = SaveAsLocal(tmpP);
                                    break;
                                case "carCondition2":
                                    shop.carCondition2 = SaveAsLocal(tmpP);
                                    break;
                                case "carCondition3":
                                    shop.carCondition3 = SaveAsLocal(tmpP);
                                    break;
                                case "carCondition4":
                                    shop.carCondition4 = SaveAsLocal(tmpP);
                                    break;
                                case "carCondition5":
                                    shop.carCondition5 = SaveAsLocal(tmpP);
                                    break;
                                case "carCondition6":
                                    shop.carCondition6 = SaveAsLocal(tmpP);
                                    break;
                                case "carCondition7":
                                    shop.carCondition7 = SaveAsLocal(tmpP);
                                    break;
                                case "carCondition8":
                                    shop.carCondition8 = SaveAsLocal(tmpP);
                                    break;
                                case "carCondition9":
                                    shop.carCondition9 = SaveAsLocal(tmpP);
                                    break;
                                case "carCondition10":
                                    shop.carCondition10 = SaveAsLocal(tmpP);
                                    break;
                                case "inspectionTable":
                                    shop.inspectionTable = SaveAsLocal(tmpP);
                                    break;
                                case "contract":
                                    shop.contract = SaveAsLocal(tmpP);
                                    break;

                                default:
                                    break;
                            }
                        }
                    }
                }
            }



            carShopEntities.SaveChanges();
            TempData["MemberResult"] = "OKUpdate";

            if (shop.status == "上架中")
            {
                var dicValue = new Dictionary<string, string>();

                string sql2 = "select * from Subscriptions where CHARINDEX(@driveMode, driveMode, 0) > 0  and CHARINDEX(@carType, carType, 0) > 0 " +
                    " and CHARINDEX(@carModel, carModel, 0) > 0 and CHARINDEX(@outerColor, outerColor, 0) > 0 " +
                    " and CHARINDEX(@area, area, 0) > 0 and CHARINDEX(@dealer, dealer, 0) > 0 and brand=@brand ";
               
                
                dicValue.Add("@driveMode", shop.driveMode);
                dicValue.Add("@carType", shop.carType);
                dicValue.Add("@carModel", shop.carModel);
                dicValue.Add("@outerColor", shop.outerColor);
                dicValue.Add("@area", shop.area);
                dicValue.Add("@dealer", shop.dealer);
                dicValue.Add("@brand", shop.brand);
                DataTable dt = APCommonFun.GetDataTable_MSSQL(sql2, dicValue);
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        string yearOfManufactureStr = APCommonFun.CDBNulltrim(dr["yearOfManufactureStr"].ToString());
                        string priceStr = APCommonFun.CDBNulltrim(dr["priceStr"].ToString());
                        string milageStr = APCommonFun.CDBNulltrim(dr["milageStr"].ToString());
                        string email = APCommonFun.CDBNulltrim(dr["email"].ToString());
                        string user_id = APCommonFun.CDBNulltrim(dr["user_id"].ToString());

                        string[] yearArray = yearOfManufactureStr.Split(',');
                        string[] priceArray = priceStr.Split(',');
                        string[] milageArray = milageStr.Split(',');

                        if (shop.price >= Convert.ToInt32(priceArray[0]) && shop.price <= Convert.ToInt32(priceArray[1]))
                        {
                            if (shop.milage >= Convert.ToInt32(milageArray[0]) && shop.milage <= Convert.ToInt32(milageArray[1]))
                            {
                                if (shop.yearOfManufacture >= Convert.ToInt32(yearArray[0]) && shop.yearOfManufacture <= Convert.ToInt32(yearArray[1]))
                                {
                                    carMgtService.MailSend_25(shop, email, yearArray, priceArray, milageArray, user_id, dt.Rows.Count);
                                }
                            }
                        }
                    }
                }
            }

            return RedirectToAction("Index", "CarMgt");
        }

        public ActionResult Delete(string seq)
        {
            if (!rightSrv.checkRight("CarMgt", "Delete", User.Identity.Name))
            {
                return RedirectToAction("Index", "Home");
            }
            var shop = carShopEntities.Shops.Where(x => x.seq.ToString() == seq).FirstOrDefault();
            carShopEntities.Shops.Remove(shop);

            carShopEntities.SaveChanges();

            TempData["MemberResult"] = "OKDelete";
            return RedirectToAction("Index", "CarMgt");
        }

        public ActionResult ExportCSV(FormCollection post)
        {
            if (!rightSrv.checkRight("CarMgt", "ExportCSV", User.Identity.Name))
            {
                return Content("NO");
            }
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
            var result = shops;

            //before your loop
            var csv = new StringBuilder();
            csv.AppendLine("編號,經銷商,中古車營業據點,車種,車型,出廠年月,出售價,關注次數,狀態,上架,成交下架,最後更新");
            foreach (var item in result)
            {
                //in your loop
                var newLine = string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11}", item.ShopNo, item.dealer, item.stronghold, item.carType, item.carModel, item.yearOfManufacture, item.price, item.NoticeCount, item.status, item.createTime, item.sellTime, item.updateTime);
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
                Response.AddHeader("Content-Disposition", "attachment; filename=" + "車輛管理列表" + DateTime.Today.ToString("yyyyMMddHHmmss") + ".csv");

                Response.BinaryWrite(memStream.ToArray());
                Response.Flush();
                Response.Close();
                Response.End();
            }

            System.IO.File.Delete(Path.Combine(Server.MapPath(_exportPath), Session["genFileName"].ToString()));

            return RedirectToAction("Index", "CarShop");
        }


        private string SaveAsLocal(HttpPostedFileBase picture)
        {
            return fileService.SaveAsBlob(picture, folderPath, "A");
        }

        private bool IsOver_CarConditionLimit(Shops shop)
        {
            var fileCount = 0;
            if (string.IsNullOrEmpty(shop.carCondition1) == false) fileCount++;
            if (string.IsNullOrEmpty(shop.carCondition2) == false) fileCount++;
            if (string.IsNullOrEmpty(shop.carCondition3) == false) fileCount++;
            if (string.IsNullOrEmpty(shop.carCondition4) == false) fileCount++;
            if (string.IsNullOrEmpty(shop.carCondition5) == false) fileCount++;
            if (string.IsNullOrEmpty(shop.carCondition6) == false) fileCount++;
            if (string.IsNullOrEmpty(shop.carCondition7) == false) fileCount++;
            if (string.IsNullOrEmpty(shop.carCondition8) == false) fileCount++;
            if (string.IsNullOrEmpty(shop.carCondition9) == false) fileCount++;
            if (string.IsNullOrEmpty(shop.carCondition10) == false) fileCount++;

            if (fileCount < 5)
            {
                TempData["MemberResult"] = "最少需要上傳5張照片";
                return true;
            }

            return false;
        }

        /// <summary>
        /// 上傳中 或 資料庫中的照片要超過限定張數
        /// </summary>
        /// <param name="shop"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        private bool IsOver_CarConditionLimit(Shops shop, int limit)
        {
            var count = 0;
            if ((Request.Files["carCondition1"] != null && Request.Files["carCondition1"].ContentLength > 0) || string.IsNullOrEmpty(shop.carCondition1) == false) count++;
            if ((Request.Files["carCondition2"] != null && Request.Files["carCondition2"].ContentLength > 0) || string.IsNullOrEmpty(shop.carCondition2) == false) count++;
            if ((Request.Files["carCondition3"] != null && Request.Files["carCondition3"].ContentLength > 0) || string.IsNullOrEmpty(shop.carCondition3) == false) count++;
            if ((Request.Files["carCondition4"] != null && Request.Files["carCondition4"].ContentLength > 0) || string.IsNullOrEmpty(shop.carCondition4) == false) count++;
            if ((Request.Files["carCondition5"] != null && Request.Files["carCondition5"].ContentLength > 0) || string.IsNullOrEmpty(shop.carCondition5) == false) count++;
            if ((Request.Files["carCondition6"] != null && Request.Files["carCondition6"].ContentLength > 0) || string.IsNullOrEmpty(shop.carCondition6) == false) count++;
            if ((Request.Files["carCondition7"] != null && Request.Files["carCondition7"].ContentLength > 0) || string.IsNullOrEmpty(shop.carCondition7) == false) count++;
            if ((Request.Files["carCondition8"] != null && Request.Files["carCondition8"].ContentLength > 0) || string.IsNullOrEmpty(shop.carCondition8) == false) count++;
            if ((Request.Files["carCondition9"] != null && Request.Files["carCondition9"].ContentLength > 0) || string.IsNullOrEmpty(shop.carCondition9) == false) count++;
            if ((Request.Files["carCondition10"] != null && Request.Files["carCondition10"].ContentLength > 0) || string.IsNullOrEmpty(shop.carCondition10) == false) count++;


            return count < 5;
        }

        /// <summary>
        /// 是否已經上傳審查表
        /// </summary>
        /// <param name="shop"></param>
        /// <returns></returns>
        private bool IsUpload_InspectionTable(Shops shop)
        {
            if ((Request.Files["inspectionTable"] != null && Request.Files["inspectionTable"].ContentLength > 0) || string.IsNullOrEmpty(shop.inspectionTable) == false)
                return true;
            else
                return false;
        }
        [HttpDelete]
        public ActionResult DeleteImage(string column, int seq)
        {
            Shops model = carShopEntities.Shops.Where(x => x.seq == seq).FirstOrDefault();
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


        /// <summary>
        /// 找出要寄信給誰
        /// </summary>
        /// <param name="shop"></param>
        /// <param name="enumRoleName"></param>
        /// <returns></returns>
        private void GetUserAndSendMail(Shops shop, DataModels.EnumRoleName enumRoleName, int index = 0)
        {
            var carMgtService = new Services.CarMgtService();


            //以下這幾封只會寄給登入使用者，所以判斷到就離開
            switch (index)
            {
                case 1:
                case 9:
                case 11:
                case 13:
                case 21:
                case 23:
                    return;
                default:
                    break;
            }

            var dicValue = new Dictionary<string, string>();
            string sql = @"SELECT * FROM [CarShop].[dbo].[AspNetUsers] a 
                            inner join [CarShop].[dbo].[Profiles] b on a.Id = b.Id 
                            inner join[CarShop].[dbo].[UserRoles] c on a.NameIdentifier = c.userName 
                            inner join[CarShop].[dbo].[Roles] d on c.userRole = d.seq";

            switch (enumRoleName)
            {
                case DataModels.EnumRoleName.經銷商管理人員:
                    {
                        dicValue.Add("@brand", shop.brand);
                        dicValue.Add("@dealer", shop.dealer);
                        sql += "  where b.brand = @brand and d.RoleName = N'經銷商管理人員' and b.Company=@dealer ";
                    }
                    break;
                case DataModels.EnumRoleName.品牌管理人員:
                    {
                        dicValue.Add("@brand", shop.brand);
                        sql += "  where b.brand = @brand and d.RoleName = N'品牌管理人員' ";
                    }
                    break;
                case DataModels.EnumRoleName.據點管理人員:
                    {
                        dicValue.Add("@brand", shop.brand);
                        dicValue.Add("@dealer", shop.dealer);
                        dicValue.Add("@Department", shop.stronghold);
                        sql += "  where b.brand = @brand and d.RoleName = N'據點管理人員'  and b.Company=@dealer and b.Department=@Department ";
                    }
                    break;
                default:
                    break;
            }


            var dt = APCommonFun.GetDataTable_MSSQL(sql, dicValue);
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string email = dt.Rows[i]["Email"].ToString();
                    switch (index)
                    {                        
                        case 2:
                            carMgtService.MailSend_02(shop, email);
                            break;
                        case 3:
                            carMgtService.MailSend_03(shop, email);
                            break;
                        case 4:
                            carMgtService.MailSend_04(shop, email);
                            break;
                        case 5:
                            carMgtService.MailSend_05(shop, email);
                            break;
                        case 6:
                            carMgtService.MailSend_06(shop, email);
                            break;
                        case 7:
                            carMgtService.MailSend_07(shop, email);
                            break;
                        case 8:
                            carMgtService.MailSend_08(shop, email);
                            break;                     
                        case 10:
                            carMgtService.MailSend_10(shop, email);
                            break;                     
                        case 12:
                            carMgtService.MailSend_12(shop, email);
                            break;                      
                        case 14:
                            carMgtService.MailSend_14(shop, email);
                            break;
                        case 15:
                            carMgtService.MailSend_15(shop, email);
                            break;
                        case 16:
                            carMgtService.MailSend_16(shop, email);
                            break;
                        case 17:
                            carMgtService.MailSend_17(shop, email);
                            break;
                        case 18:
                            carMgtService.MailSend_18(shop, email);
                            break;
                        case 19:
                            carMgtService.MailSend_19(shop, email);
                            break;
                        case 20:
                            carMgtService.MailSend_20(shop, email);
                            break;                    
                        case 22:
                            carMgtService.MailSend_22(shop, email);
                            break;                     
                        case 24:
                            carMgtService.MailSend_24(shop, email);
                            break;
                        default:
                            break;
                    }
                }
            }
        }
    }
}