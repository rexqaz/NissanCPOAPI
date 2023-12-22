using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication.Models;
using WebApplication.Services;

namespace WebApplication.Controllers
{
    public class DealerController : Controller
    {
        private BrandService brandSrv = new BrandService();
        private RightService rightSrv = new RightService();
        CarShopEntities carShopEntities = new CarShopEntities();
        FileService fileService = new FileService();
        private const string folderPath = "Dealer";
        // GET: Dealer
        public ActionResult Index(string type)
        {
            if (!rightSrv.checkRight("Dealer", "Index", User.Identity.Name))
            {
                return RedirectToAction("Index", "Home");
            }

            if (!rightSrv.checkRight("Dealer", "Update", User.Identity.Name))
            {
                TempData["UpdateRight"] = "False";
            }
            else
            {
                TempData["UpdateRight"] = "True";
            }

            if (!rightSrv.checkRight("Dealer", "Create", User.Identity.Name))
            {
                TempData["CreateRight"] = "False";
            }
            else
            {
                TempData["CreateRight"] = "True";
            }

            DealerViewModel model = new DealerViewModel();
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

            model.type = type;
            if (user != null)
            {
                int role = carShopEntities.UserRoles.Where(x => x.userName == user.NameIdentifier).Select(x => x.userRole).FirstOrDefault() ?? 0;
                model.role = carShopEntities.Roles.Where(x => x.seq == role).Select(x => x.RoleName).FirstOrDefault() ?? string.Empty;
            }
            return View(model);
        }

        public ActionResult Person()
        {
            if (!rightSrv.checkRight("Dealer", "Person", User.Identity.Name))
            {
                return RedirectToAction("Index", "Home");
            }

            if (!rightSrv.checkRight("Dealer", "UpdatePerson", User.Identity.Name))
            {
                TempData["UpdateRight"] = "False";
            }
            else
            {
                TempData["UpdateRight"] = "True";
            }

            if (!rightSrv.checkRight("Dealer", "CreatePerson", User.Identity.Name))
            {
                TempData["CreateRight"] = "False";
            }
            else
            {
                TempData["CreateRight"] = "True";
            }

            return View();
        }

        [HttpPost]
        public JsonResult GetDealerData(FormCollection post)
       {
            var draw = Request.Form["draw"].FirstOrDefault();
            string start = Request.Form["start"];
            string length = Request.Form["length"];
            string keyword = post["Keyword"];
            string type = post["type"];
            string sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"];
            string sortColumnDirection = Request.Form["order[0][dir]"];
            string searchValue = Request.Form["search[value]"];
            int pageSize = Convert.ToInt32(length);
            int skip = Convert.ToInt32(start);
            int recordsTotal = 0;

            string brand = brandSrv.getBrand(User.Identity.Name);
            List<Dealers> dealers = carShopEntities.Dealers.Where(x => x.type == type && x.brand == brand).OrderByDescending(x => x.createTime).ToList();
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
                        dealers = dealers.Where(x => x.dealerName.Contains(company)).ToList();
                        break;
                    case "據點管理人員":
                        dealers = dealers.Where(x => x.businessOffice.Contains(department)).ToList();
                        break;
                    case "據點所屬業代":
                        dealers = dealers.Where(x => x.businessOffice.Contains(department)).ToList();
                        break;
                    default:
                        break;
                }
            }

            if (sortColumn == "建立日期")
            {
                if (sortColumnDirection == "asc") dealers = dealers.OrderBy(x => x.createTime).ToList();
                else dealers = dealers.OrderByDescending(x => x.createTime).ToList();
            }
            else if (sortColumn == "地區")
            {
                if (sortColumnDirection == "asc") dealers = dealers.OrderBy(x => x.area).ToList();
                else dealers = dealers.OrderByDescending(x => x.area).ToList();
            }
            else if (sortColumn == "經銷商")
            {
                if (sortColumnDirection == "asc") dealers = dealers.OrderBy(x => x.dealerName).ToList();
                else dealers = dealers.OrderByDescending(x => x.dealerName).ToList();
            }
            else if (sortColumn == "營業所")
            {
                if (sortColumnDirection == "asc") dealers = dealers.OrderBy(x => x.businessOffice).ToList();
                else dealers = dealers.OrderByDescending(x => x.businessOffice).ToList();
            }
            else if (sortColumn == "Email")
            {
                if (sortColumnDirection == "asc") dealers = dealers.OrderBy(x => x.email).ToList();
                else dealers = dealers.OrderByDescending(x => x.email).ToList();
            }

            if (!string.IsNullOrEmpty(keyword))
            {
                var shop1 = dealers.Where(x => x.createTime != null && x.createTime.ToString().Contains(keyword));
                var shop2 = dealers.Where(x => x.area != null && x.area.Contains(keyword));
                var shop3 = dealers.Where(x => x.dealerName != null && x.dealerName.Contains(keyword));
                var shop4 = dealers.Where(x => x.businessOffice != null && x.businessOffice.Contains(keyword));
                var shop5 = dealers.Where(x => x.email != null && x.email.Contains(keyword));

                dealers = shop1.Concat(shop2).Concat(shop3).Concat(shop4).Concat(shop5).ToList();
            }


            var data = dealers.Skip(skip).Take(pageSize).ToList();
            var jsonData = new { draw = draw, recordsFiltered = dealers.Count, recordsTotal = dealers.Count, data = data };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetDealerPersonData(FormCollection post)
        {
            var draw = Request.Form["draw"].FirstOrDefault();
            string start = Request.Form["start"];
            string length = Request.Form["length"];
            string keyword = post["Keyword"];
            string sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"];
            string sortColumnDirection = Request.Form["order[0][dir]"];
            string searchValue = Request.Form["search[value]"];
            int pageSize = Convert.ToInt32(length);
            int skip = Convert.ToInt32(start);
            int recordsTotal = 0;

            string brand = brandSrv.getBrand(User.Identity.Name);
            List<DealerPersons> dealerPersons = carShopEntities.DealerPersons.Where(x => x.brand == brand).OrderByDescending(x => x.createTime).ToList();

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
                        dealerPersons = dealerPersons.Where(x => x.dealer.Contains(company)).ToList();
                        break;
                    case "據點管理人員":
                        dealerPersons = dealerPersons.Where(x => x.businessOffice.Contains(department)).ToList();
                        break;
                    case "據點所屬業代":
                        dealerPersons = dealerPersons.Where(x => x.businessOffice.Contains(department)).ToList();
                        break;
                    default:
                        break;
                }
            }

            if (sortColumn == "建立日期")
            {
                if (sortColumnDirection == "asc") dealerPersons = dealerPersons.OrderBy(x => x.createTime).ToList();
                else dealerPersons = dealerPersons.OrderByDescending(x => x.createTime).ToList();
            }
            else if (sortColumn == "地區")
            {
                if (sortColumnDirection == "asc") dealerPersons = dealerPersons.OrderBy(x => x.area).ToList();
                else dealerPersons = dealerPersons.OrderByDescending(x => x.area).ToList();
            }
            else if (sortColumn == "經銷商")
            {
                if (sortColumnDirection == "asc") dealerPersons = dealerPersons.OrderBy(x => x.dealer).ToList();
                else dealerPersons = dealerPersons.OrderByDescending(x => x.dealer).ToList();
            }
            else if (sortColumn == "營業所")
            {
                if (sortColumnDirection == "asc") dealerPersons = dealerPersons.OrderBy(x => x.businessOffice).ToList();
                else dealerPersons = dealerPersons.OrderByDescending(x => x.businessOffice).ToList();
            }
            else if (sortColumn == "姓名")
            {
                if (sortColumnDirection == "asc") dealerPersons = dealerPersons.OrderBy(x => x.name).ToList();
                else dealerPersons = dealerPersons.OrderByDescending(x => x.name).ToList();
            }

            if (!string.IsNullOrEmpty(keyword))
            {
                var shop1 = dealerPersons.Where(x => x.createTime != null && x.createTime.ToString().Contains(keyword));
                var shop2 = dealerPersons.Where(x => x.area != null && x.area.Contains(keyword));
                var shop3 = dealerPersons.Where(x => x.dealer != null && x.dealer.Contains(keyword));
                var shop4 = dealerPersons.Where(x => x.businessOffice != null && x.businessOffice.Contains(keyword));
                var shop5 = dealerPersons.Where(x => x.name != null && x.name.Contains(keyword));

                dealerPersons = shop1.Concat(shop2).Concat(shop3).Concat(shop4).Concat(shop5).ToList();
            }


            var data = dealerPersons.Skip(skip).Take(pageSize).ToList();
            var jsonData = new { draw = draw, recordsFiltered = dealerPersons.Count, recordsTotal = dealerPersons.Count, data = data };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Create(string type)
        {
            if (!rightSrv.checkRight("Dealer", "Create", User.Identity.Name))
            {
                return RedirectToAction("Index", "Home");
            }
            TempData["dealerType"] = type;
            return View();
        }

        public ActionResult CreatePerson()
        {
            if (!rightSrv.checkRight("Dealer", "CreatePerson", User.Identity.Name))
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult CreateInProgress(DealersViewModel2 model,  HttpPostedFileBase picture)
        {
            Dealers dealer = new Dealers();
            string contentType = string.Empty;
            

            if (string.IsNullOrEmpty(model.area))
            {
                TempData["MemberResult"] = "地區為必填";
                return RedirectToAction("Create", "Dealer", new { type=model.type});
            }

            if (string.IsNullOrEmpty(model.dealerName))
            {
                TempData["MemberResult"] = "經銷商為必填";
                return RedirectToAction("Create", "Dealer", new { type = model.type });
            }

            if (string.IsNullOrEmpty(model.businessOffice))
            {
                TempData["MemberResult"] = "營業所全名稱為必填";
                return RedirectToAction("CreatePerson", "Dealer");
            }

            if (model.type == "old")
            {
                if (picture != null && picture.ContentLength > 0)
                {
                    dealer.picture = SaveAsLocal(picture);
                }
                else
                {
                    TempData["MemberResult"] = "請上傳檔案";
                    return RedirectToAction("Create", "Dealer");
                }

                if (string.IsNullOrEmpty(model.businessOffice2))
                {
                    TempData["MemberResult"] = "營業所短稱為必填";
                    return RedirectToAction("Create", "Dealer", new { type = model.type });
                }
                dealer.businessOffice2 = model.businessOffice2;

                if (string.IsNullOrEmpty(model.telAreaCode))
                {
                    TempData["MemberResult"] = "營業所電話區碼為必填";
                    return RedirectToAction("Create", "Dealer", new { type = model.type });
                }
                dealer.telAreaCode = model.telAreaCode;

                if (string.IsNullOrEmpty(model.tel))
                {
                    TempData["MemberResult"] = "營業所電話為必填";
                    return RedirectToAction("Create", "Dealer", new { type = model.type });
                }
                dealer.tel = model.tel;

                if (string.IsNullOrEmpty(model.address))
                {
                    TempData["MemberResult"] = "營業所地址為必填";
                    return RedirectToAction("Create", "Dealer", new { type = model.type });
                }
                dealer.address = model.address;

                if (string.IsNullOrEmpty(model.gmap))
                {
                    TempData["MemberResult"] = "經緯度為必填";
                    return RedirectToAction("Create", "Dealer", new { type = model.type });
                }
                dealer.gmap = model.gmap;


                dealer.email = model.email;

                if (model.busDays.Count() == 0)
                {
                    TempData["MemberResult"] = "營業日期為必填";
                    return RedirectToAction("Create", "Dealer", new { type = model.type });
                }
                dealer.busDay = string.Join(",", model.busDays);
                dealer.businessStartHourDay = model.businessStartHourDay;
                dealer.businessStartHourNight = model.businessStartHourNight;
                dealer.businessEndHourDay = model.businessEndHourDay;
                dealer.businessEndHourNight = model.businessEndHourNight;
            }

            if (model.type == "new")
            {
                dealer.email = model.email;
            }

            dealer.area = model.area;
            dealer.dealerName = model.dealerName;
            dealer.dealerCode = model.dealerCode;
            dealer.businessOffice = model.businessOffice;
            dealer.type = model.type;
            dealer.createTime = DateTime.Now;
            dealer.brand = brandSrv.getBrand(User.Identity.Name);

            carShopEntities.Dealers.Add(dealer);
            carShopEntities.SaveChanges();
            TempData["MemberResult"] = "OKCreate";
            return RedirectToAction("Index", "Dealer", new { type=model.type});
        }

        [HttpPost]
        public ActionResult CreatePersonInProgress(DealerPersons model)
        {
            if (string.IsNullOrEmpty(model.name))
            {
                TempData["MemberResult"] = "姓名為必填";
                return RedirectToAction("CreatePerson", "Dealer");
            }

            if (string.IsNullOrEmpty(model.mobile))
            {
                TempData["MemberResult"] = "手機為必填";
                return RedirectToAction("CreatePerson", "Dealer");
            }

            if (string.IsNullOrEmpty(model.email))
            {
                TempData["MemberResult"] = "Email為必填";
                return RedirectToAction("CreatePerson", "Dealer");
            }

            if (string.IsNullOrEmpty(model.dealer))
            {
                TempData["MemberResult"] = "經銷商名稱為必填";
                return RedirectToAction("CreatePerson", "Dealer");
            }

            if (string.IsNullOrEmpty(model.businessOffice))
            {
                TempData["MemberResult"] = "營業所短稱為必填";
                return RedirectToAction("CreatePerson", "Dealer");
            }

            model.createTime = DateTime.Now;
            model.brand = brandSrv.getBrand(User.Identity.Name);
            carShopEntities.DealerPersons.Add(model);
            carShopEntities.SaveChanges();
            TempData["MemberResult"] = "OKCreate";
            return RedirectToAction("Person", "Dealer");
        }
        public ActionResult Delete(string seq)
        {
            if (!rightSrv.checkRight("Dealer", "Delete", User.Identity.Name))
            {
                return RedirectToAction("Index", "Home");
            }
            var dealer = carShopEntities.Dealers.Where(x => x.seq.ToString() == seq).FirstOrDefault();

            string type = string.Empty;
            if (dealer != null)
            {
                type = dealer.type;
            }
            carShopEntities.Dealers.Remove(dealer);

            carShopEntities.SaveChanges();

            TempData["MemberResult"] = "OKDelete";
            return RedirectToAction("Index", "Dealer", new { type=type});
        }

        public ActionResult DeletePerson(string seq)
        {
            if (!rightSrv.checkRight("Dealer", "DeletePerson", User.Identity.Name))
            {
                return RedirectToAction("Index", "Home");
            }

            var person = carShopEntities.DealerPersons.Where(x => x.seq.ToString() == seq).FirstOrDefault();
            carShopEntities.DealerPersons.Remove(person);

            carShopEntities.SaveChanges();

            TempData["MemberResult"] = "OKDelete";
            return RedirectToAction("Person", "Dealer");
        }

        public ActionResult Update(string seq, string type)
        {
            if (!rightSrv.checkRight("Dealer", "Update", User.Identity.Name))
            {
                return RedirectToAction("Index", "Home");
            }

            if (!rightSrv.checkRight("Dealer", "UpdateInProgress", User.Identity.Name))
            {
                TempData["UpdateRight"] = "False";
            }
            else
            {
                TempData["UpdateRight"] = "True";
            }

            if (!rightSrv.checkRight("Dealer", "Delete", User.Identity.Name))
            {
                TempData["DeleteRight"] = "False";
            }
            else
            {
                TempData["DeleteRight"] = "True";
            }

            Dealers dealer = carShopEntities.Dealers.Where(x => x.seq.ToString() == seq).FirstOrDefault();
            TempData["dealerType"] = type;

            var user = carShopEntities.AspNetUsers.Where(x => x.Email == User.Identity.Name).FirstOrDefault();
            if (user != null)
            {
                int role = carShopEntities.UserRoles.Where(x => x.userName == user.NameIdentifier).Select(x => x.userRole).FirstOrDefault() ?? 0;
                TempData["role"] = carShopEntities.Roles.Where(x => x.seq == role).Select(x => x.RoleName).FirstOrDefault() ?? string.Empty;
            }

            DealersViewModel2 model = new DealersViewModel2();
            model.seq = dealer.seq;
            model.area = dealer.area;
            model.dealerName = dealer.dealerName;
            model.dealerCode = dealer.dealerCode;
            model.businessOffice = dealer.businessOffice;
            model.businessOffice2 = dealer.businessOffice2;
            model.telAreaCode = dealer.telAreaCode;
            model.tel = dealer.tel;
            model.address = dealer.address;
            model.gmap = dealer.gmap;
            model.businessEndHourDay = dealer.businessEndHourDay;
            model.businessEndHourNight = dealer.businessEndHourNight;
            model.businessStartHourDay = dealer.businessStartHourDay;
            model.businessStartHourNight = dealer.businessStartHourNight;
            model.createTime = dealer.createTime;
            model.type = dealer.type;
            model.email = dealer.email;
            model.brand = dealer.brand;
            model.busDay = dealer.busDay;
            model.picture = fileService.GetRealUrl(dealer.picture);

            return View(model);
        }

        public ActionResult UpdatePerson(string seq)
        {
            if (!rightSrv.checkRight("Dealer", "UpdatePerson", User.Identity.Name))
            {
                return RedirectToAction("Index", "Home");
            }

            if (!rightSrv.checkRight("Dealer", "UpdatePersonInProgress", User.Identity.Name))
            {
                TempData["UpdateRight"] = "False";
            }
            else
            {
                TempData["UpdateRight"] = "True";
            }

            if (!rightSrv.checkRight("Dealer", "DeletePerson", User.Identity.Name))
            {
                TempData["DeleteRight"] = "False";
            }
            else
            {
                TempData["DeleteRight"] = "True";
            }

            DealerPersons model = carShopEntities.DealerPersons.Where(x => x.seq.ToString() == seq).FirstOrDefault();

            TempData["businessOffice"] = carShopEntities.Dealers.Where(x => x.dealerName == model.dealer && x.type == "old").Select(x => x.businessOffice).ToList(); ;
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult UpdateInProgress(DealersViewModel2 model, HttpPostedFileBase picture)
        {
            if (!rightSrv.checkRight("Dealer", "UpdateInProgress", User.Identity.Name))
            {
                return RedirectToAction("Index", "Home");
            }
            if (string.IsNullOrEmpty(model.area))
            {
                TempData["MemberResult"] = "地區為必填";
                return RedirectToAction("Update", "Dealer", new { seq = model.seq, type=model.type });
            }

            if (string.IsNullOrEmpty(model.dealerName))
            {
                TempData["MemberResult"] = "經銷商為必填";
                return RedirectToAction("Update", "Dealer", new { seq = model.seq, type = model.type });
            }

            if (string.IsNullOrEmpty(model.businessOffice))
            {
                TempData["MemberResult"] = "營業所全名為必填";
                return RedirectToAction("Update", "Dealer", new { seq = model.seq, type = model.type });
            }

            string contentType = string.Empty;

            if (model.type == "old")
            {
                if (string.IsNullOrEmpty(model.tel))
                {
                    TempData["MemberResult"] = "營業所電話為必填";
                    return RedirectToAction("Update", "Dealer", new { seq = model.seq, type = model.type });
                }

                if (string.IsNullOrEmpty(model.address))
                {
                    TempData["MemberResult"] = "營業所地址為必填";
                    return RedirectToAction("Update", "Dealer", new { seq = model.seq, type = model.type });
                }


                if (model.busDays.Count() == 0)
                {
                    TempData["MemberResult"] = "營業日期為必填";
                    return RedirectToAction("Create", "Dealer", new { type = model.type });
                }

                if (string.IsNullOrEmpty(model.businessOffice2))
                {
                    TempData["MemberResult"] = "營業所短稱為必填";
                    return RedirectToAction("Update", "Dealer", new { seq = model.seq, type = model.type });
                }
            }


            var dealer = carShopEntities.Dealers.Where(x => x.seq == model.seq).FirstOrDefault();
            if (dealer != null)
            {
                
                dealer.area = model.area;
                dealer.dealerName = model.dealerName;
                dealer.businessOffice = model.businessOffice;
                dealer.dealerCode = model.dealerCode;

                if (dealer.type == "old")
                {
                    if (picture != null && picture.ContentLength > 0)
                    {
                        dealer.picture = SaveAsLocal(picture);
                    }

                    dealer.busDay = string.Join(",", model.busDays);
                    dealer.tel = model.tel;
                    dealer.businessOffice2 = model.businessOffice2;
                    dealer.address = model.address;
                    dealer.gmap = model.gmap;
                    dealer.businessStartHourDay = model.businessStartHourDay;
                    dealer.businessEndHourDay = model.businessEndHourDay;
                    dealer.businessStartHourNight = model.businessStartHourNight;
                    dealer.businessEndHourNight = model.businessEndHourNight;
                    dealer.email = model.email;
                }
                else
                {
                    dealer.email = model.email;
                }
                dealer.type = model.type;

                carShopEntities.SaveChanges();
            }

            TempData["MemberResult"] = "OKUpdate";
            return RedirectToAction("Index", "Dealer", new { type=model.type});
        }

        [HttpPost]
        public ActionResult UpdatePersonInProgress(DealerPersons model)
        {
            if (!rightSrv.checkRight("Dealer", "UpdatePersonInProgress", User.Identity.Name))
            {
                return RedirectToAction("Index", "Home");
            }
            if (string.IsNullOrEmpty(model.name))
            {
                TempData["MemberResult"] = "姓名為必填";
                return RedirectToAction("UpdatePerson", "Dealer", new { seq = model.seq });
            }

            if (string.IsNullOrEmpty(model.mobile))
            {
                TempData["MemberResult"] = "手機為必填";
                return RedirectToAction("UpdatePerson", "Dealer", new { seq = model.seq });
            }

            if (string.IsNullOrEmpty(model.email))
            {
                TempData["MemberResult"] = "Email為必填";
                return RedirectToAction("UpdatePerson", "Dealer", new { seq = model.seq });
            }

            if (string.IsNullOrEmpty(model.dealer))
            {
                TempData["MemberResult"] = "經銷商名稱為必填";
                return RedirectToAction("UpdatePerson", "Dealer", new { seq = model.seq });
            }

            if (string.IsNullOrEmpty(model.businessOffice))
            {
                TempData["MemberResult"] = "營業所名稱為必填";
                return RedirectToAction("UpdatePerson", "Dealer", new { seq = model.seq });
            }

            var person = carShopEntities.DealerPersons.Where(x => x.seq == model.seq).FirstOrDefault();
            if (person != null)
            {
                person.name = model.name;
                person.mobile = model.mobile;
                person.email = model.email;
                person.dealer = model.dealer;
                person.businessOffice = model.businessOffice;

                carShopEntities.SaveChanges();
            }

            TempData["MemberResult"] = "OKUpdate";
            return RedirectToAction("Person", "Dealer");
        }

        public ActionResult GetBusinessOffice(string dealerName, string type)
        {
            string brand = brandSrv.getBrand(User.Identity.Name);
            if (type == "old")
            {
                List<string> businessOffice = carShopEntities.Dealers.Where(x => x.dealerName == dealerName && x.type == "old" && x.brand == brand).Select(x => x.businessOffice).ToList();

                return Json(new
                {
                    list = businessOffice
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                List<string> businessOffice = carShopEntities.Dealers.Where(x => x.dealerName == dealerName && x.brand == brand).Select(x => x.businessOffice).ToList();

                return Json(new
                {
                    list = businessOffice
                }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetDealer(string area)
        {
            string brand = brandSrv.getBrand(User.Identity.Name);
            List<string> dealers = carShopEntities.Dealers.Where(x => x.area == area && x.brand == brand).Select(x => x.dealerName).Distinct().ToList();

            return Json(new
            {
                list = dealers
            }, JsonRequestBehavior.AllowGet);
        }

        private string SaveAsLocal(HttpPostedFileBase picture)
        {
            return fileService.SaveAsBlob(picture, folderPath, "A");
        }
    }
}