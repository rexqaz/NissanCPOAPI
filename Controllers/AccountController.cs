using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using WebApplication.Models;
using WebApplication.Services;

namespace WebApplication.Controllers
{
    public class AccountController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private LogService srv = new LogService();
        private BrandService brandSrv = new BrandService();
        private RightService rightSrv = new RightService();
        CarShopEntities carShopEntities = new CarShopEntities();

        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        public ActionResult Index()
        {
            if (!rightSrv.checkRight("Account", "Index", User.Identity.Name))
            {
                return RedirectToAction("Index", "Home");
            }

            if (!rightSrv.checkRight("Account", "Update", User.Identity.Name))
            {
                TempData["UpdateRight"] = "False";
            }
            else
            {
                TempData["UpdateRight"] = "True";
            }

            if (!rightSrv.checkRight("Account", "Update", User.Identity.Name))
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
        public JsonResult GetAccountData(FormCollection post)
        {
            var draw = Request.Form["draw"].FirstOrDefault();
            string start = Request.Form["start"];
            string length = Request.Form["length"];
            string keyword = post["Keyword"];
            string dateFilter = post["dateFilter"];
            string sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"];
            string sortColumnDirection = Request.Form["order[0][dir]"];
            string searchValue = Request.Form["search[value]"];
            int pageSize = Convert.ToInt32(length);
            int skip = Convert.ToInt32(start);
            int recordsTotal = 0;

            List<AspNetUsers> users = carShopEntities.AspNetUsers.ToList();
            List<RegisterViewModel> accounts = new List<RegisterViewModel>();
            string brand = brandSrv.getBrand(User.Identity.Name);
            foreach (var user in users)
            {
                RegisterViewModel item = new RegisterViewModel();
                var userRole = carShopEntities.UserRoles.Where(x => x.userName == user.NameIdentifier).FirstOrDefault();
                Profiles profile = carShopEntities.Profiles.Where(x => x.Id == user.Id).FirstOrDefault();
                item.NameIdentifier = user.NameIdentifier;
                item.Email = user.Email;
                if (profile != null)
                {
                    item.Name = profile.Name;
                    item.Company = profile.Company;
                    item.Department = profile.Department;
                    item.Mobile = profile.Mobile;
                    item.createTime = profile.createTime ?? DateTime.Now;
                    item.status = profile.status ?? false;
                    if (userRole != null)
                    {
                        item.brand = userRole == null ? profile.brand : userRole.userRole == 1 ? "NISSAN/INFINITI" : profile.brand;
                        item.Role = carShopEntities.Roles.Where(x => x.seq == userRole.userRole).Select(x => x.RoleName).FirstOrDefault();
                    }
                    item.LastLoginTime = profile.LastLoginTime ?? DateTime.Now;
                }

                if (!string.IsNullOrEmpty(item.brand) && item.brand.Contains(brand))
                {
                    accounts.Add(item);
                }
            }

            var user1 = carShopEntities.AspNetUsers.Where(x => x.Email == User.Identity.Name).FirstOrDefault();
            string roleName = string.Empty;
            string company = string.Empty;
            string department = string.Empty;
            if (user1 != null)
            {
                int role = carShopEntities.UserRoles.Where(x => x.userName == user1.NameIdentifier).Select(x => x.userRole).FirstOrDefault() ?? 0;
                roleName = carShopEntities.Roles.Where(x => x.seq == role).Select(x => x.RoleName).FirstOrDefault() ?? string.Empty;

                company = carShopEntities.Profiles.Where(x => x.Id == user1.Id).Select(x => x.Company).FirstOrDefault() ?? string.Empty;
                department = carShopEntities.Profiles.Where(x => x.Id == user1.Id).Select(x => x.Department).FirstOrDefault() ?? string.Empty;
            }

            if (roleName != "ISC最高權限管理人員")
            {
                switch (roleName)
                {
                    case "品牌管理人員":
                        accounts = accounts.Where(x => x.Role != "ISC最高權限管理人員").ToList();
                        break;
                    case "經銷商管理人員":
                        accounts = accounts.Where(x => x.Company.Contains(company) && (x.Role == "經銷商管理人員" || x.Role == "據點管理人員" || x.Role == "客服人員")).ToList();
                        break;
                    case "據點管理人員":
                        accounts = accounts.Where(x => x.Company.Contains(company) && x.Department == department && (x.Role == "據點管理人員" || x.Role == "客服人員")).ToList();
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
                accounts = accounts.Where(x => x.brand == brand && x.createTime >= min && x.createTime <= max).ToList(); ;
            }

            if (sortColumn == "建立日期")
            {
                if (sortColumnDirection == "asc") accounts = accounts.OrderBy(x => x.createTime).ToList();
                else accounts = accounts.OrderByDescending(x => x.createTime).ToList();
            }
            else if (sortColumn == "姓名")
            {
                if (sortColumnDirection == "asc") accounts = accounts.OrderBy(x => x.Name).ToList();
                else accounts = accounts.OrderByDescending(x => x.Name).ToList();
            }
            else if (sortColumn == "帳號")
            {
                if (sortColumnDirection == "asc") accounts = accounts.OrderBy(x => x.NameIdentifier).ToList();
                else accounts = accounts.OrderByDescending(x => x.NameIdentifier).ToList();
            }
            else if (sortColumn == "公司法人")
            {
                if (sortColumnDirection == "asc") accounts = accounts.OrderBy(x => x.Company).ToList();
                else accounts = accounts.OrderByDescending(x => x.Company).ToList();
            }
            else if (sortColumn == "部門")
            {
                if (sortColumnDirection == "asc") accounts = accounts.OrderBy(x => x.Department).ToList();
                else accounts = accounts.OrderByDescending(x => x.Department).ToList();
            }
            else if (sortColumn == "品牌名稱")
            {
                if (sortColumnDirection == "asc") accounts = accounts.OrderBy(x => x.brand).ToList();
                else accounts = accounts.OrderByDescending(x => x.brand).ToList();
            }
            else if (sortColumn == "管理群組")
            {
                if (sortColumnDirection == "asc") accounts = accounts.OrderBy(x => x.Role).ToList();
                else accounts = accounts.OrderByDescending(x => x.Role).ToList();
            }
            else if (sortColumn == "帳號狀態")
            {
                if (sortColumnDirection == "asc") accounts = accounts.OrderBy(x => x.status).ToList();
                else accounts = accounts.OrderByDescending(x => x.status).ToList();
            }
            else if (sortColumn == "最後登入時間")
            {
                if (sortColumnDirection == "asc") accounts = accounts.OrderBy(x => x.LastLoginTime).ToList();
                else accounts = accounts.OrderByDescending(x => x.LastLoginTime).ToList();
            }

            if (!string.IsNullOrEmpty(keyword))
            {
                var shop1 = accounts.Where(x => x.createTime != null && x.createTime.ToString().Contains(keyword));
                var shop2 = accounts.Where(x => x.Name != null && x.Name.Contains(keyword));
                var shop3 = accounts.Where(x => x.NameIdentifier != null && x.NameIdentifier.Contains(keyword));
                var shop4 = accounts.Where(x => x.Company != null && x.Company.Contains(keyword));
                var shop5 = accounts.Where(x => x.Department != null && x.Department.Contains(keyword));
                var shop7 = accounts.Where(x => x.brand != null && x.brand.ToString().Contains(keyword));
                var shop6 = accounts.Where(x => x.Role != null && x.Role.ToString().Contains(keyword));
                var shop8 = accounts.Where(x => x.LastLoginTime != null && x.LastLoginTime.ToString().Contains(keyword));
                var shop9 = accounts.Where(x => x.status != null && x.status.ToString().Contains(keyword));
                accounts = shop1.Concat(shop2).Concat(shop3).Concat(shop4).Concat(shop5).Concat(shop6).Concat(shop7).Concat(shop8).Concat(shop9).ToList();
            }


            var data = accounts.Skip(skip).Take(pageSize).ToList();
            var jsonData = new { draw = draw, recordsFiltered = accounts.Count, recordsTotal = accounts.Count, data = data };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ContentResult Brand()
        {
            var brand = Request["brand"];
            string email = User.Identity.GetUserName();
            var user = carShopEntities.AspNetUsers.Where(x => x.Email == email).FirstOrDefault();
            if (user != null)
            {
                var profile = carShopEntities.Profiles.Where(x => x.Id == user.Id).FirstOrDefault();
                if (profile != null)
                {
                    profile.brand = brand.Trim();
                }

                carShopEntities.SaveChanges();
            }
            return Content("OK"); 
        }

        public ActionResult Create()
        {
            if (!rightSrv.checkRight("Account", "Create", User.Identity.Name))
            {
                return RedirectToAction("Index", "Home");
            }
            CreateUserViewModel model = new CreateUserViewModel();
            List<Roles> roles = carShopEntities.Roles.Where(x => x.status == true).ToList();
            model.Roles = roles;

            var user1 = carShopEntities.AspNetUsers.Where(x => x.Email == User.Identity.Name).FirstOrDefault();
            string roleName = string.Empty;
            string company = string.Empty;
            string department = string.Empty;
            if (user1 != null)
            {
                int role = carShopEntities.UserRoles.Where(x => x.userName == user1.NameIdentifier).Select(x => x.userRole).FirstOrDefault() ?? 0;
                roleName = carShopEntities.Roles.Where(x => x.seq == role).Select(x => x.RoleName).FirstOrDefault() ?? string.Empty;

                company = carShopEntities.Profiles.Where(x => x.Id == user1.Id).Select(x => x.Company).FirstOrDefault() ?? string.Empty;
                department = carShopEntities.Profiles.Where(x => x.Id == user1.Id).Select(x => x.Department).FirstOrDefault() ?? string.Empty;
            }

            if (roleName != "ISC最高權限管理人員")
            {
                switch (roleName)
                {
                    case "品牌管理人員":
                        model.Roles = model.Roles.Where(x => x.RoleName == "品牌管理人員" || x.RoleName == "經銷商管理人員" || x.RoleName == "據點管理人員" || x.RoleName == "客服人員").ToList();
                        break;
                    case "經銷商管理人員":
                        model.Roles = model.Roles.Where(x => x.RoleName == "經銷商管理人員" || x.RoleName == "據點管理人員" || x.RoleName == "客服人員").ToList();
                        break;
                    case "據點管理人員":
                        model.Roles = model.Roles.Where(x => x.RoleName == "據點管理人員" || x.RoleName == "客服人員").ToList();
                        break;
                    default:
                        break;
                }
            }

            return View(model);
        }

        public ActionResult Update(string email)
        {
            if (!rightSrv.checkRight("Account", "Update", User.Identity.Name))
            {
                return RedirectToAction("Index", "Home");
            }

            if (!rightSrv.checkRight("Account", "UpdateInProgress", User.Identity.Name))
            {
                TempData["UpdateRight"] = "False";
            }
            else
            {
                TempData["UpdateRight"] = "True";
            }

            if (!rightSrv.checkRight("Account", "Delete", User.Identity.Name))
            {
                TempData["DeleteRight"] = "False";
            }
            else
            {
                TempData["DeleteRight"] = "True";
            }

            RegisterViewModel model = new RegisterViewModel();
            List<Roles> roles = carShopEntities.Roles.ToList();
            model.Roles = roles;

            var user = carShopEntities.AspNetUsers.Where(x => x.Email == email).FirstOrDefault();            
            if (user != null)
            {
                var profile = carShopEntities.Profiles.Where(x => x.Id == user.Id).FirstOrDefault();
                model.NameIdentifier = user.NameIdentifier;
                model.Email = user.Email;
                if (profile != null)
                {
                    model.Name = profile.Name;
                    model.Company = profile.Company;
                    model.Department = profile.Department;
                    model.Mobile = profile.Mobile;
                    model.status = profile.status ?? false;
                    model.createTime = profile.createTime ?? DateTime.Now;
                    model.brand = profile.brand;
                }
                model.Password = string.Empty;
                model.ConfirmPassword = string.Empty;
            }

            var role = carShopEntities.UserRoles.Where(x => x.userName == model.NameIdentifier).FirstOrDefault();
            if (role != null)
            {
                model.Role = role.userRole.ToString();
                model.roleName = carShopEntities.Roles.Where(x => x.seq.ToString() == model.Role).Select(x => x.RoleName).FirstOrDefault() ?? string.Empty;
            }

            TempData["businessOffice"] = carShopEntities.Dealers.Where(x => x.dealerName == model.Company && x.type == "old").Select(x => x.businessOffice).ToList(); ;
            if (model.Company == "裕隆日產汽車")
            {
                List<string> businessOffice = new List<string>();
                businessOffice.Add("無");
                businessOffice.Add("MSD");
                businessOffice.Add("IBD");
                businessOffice.Add("DTO");
                businessOffice.Add("ISC");
                TempData["businessOffice"] = businessOffice;
            }
            return View(model);
        }

        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = carShopEntities.AspNetUsers.Where(x => x.NameIdentifier == model.Name).FirstOrDefault();
            if (user != null)
            {
                if (user.EmailConfirmed)
                {
                    string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                    TempData["ResetPassword"] = "First";
                    return RedirectToAction("ResetPassword", "Account", new { userId = user.Id, code = code });
                }

                var profile = carShopEntities.Profiles.Where(x => x.Id == user.Id).FirstOrDefault();
                if (profile != null)
                {
                    if (profile.status == false)
                    {
                        srv.AddLog(user.Email, "帳號停用", "[login]", "Info", user.NameIdentifier);
                        ModelState.AddModelError("", "帳號停用");
                        return View(model);
                    }
                    profile.LastLoginTime = DateTime.Now;

                    var userRole = carShopEntities.UserRoles.Where(x => x.userName == user.NameIdentifier).FirstOrDefault();
                    if (userRole != null)
                    {
                        if (userRole.userRole == 1)
                        {
                            profile.brand = "NISSAN";
                        }

                        carShopEntities.SaveChanges();
                    }
                }
            }
            else
            {
                ModelState.AddModelError("", "帳號不存在!!");
                return View(model);
            }

            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            var result = await SignInManager.PasswordSignInAsync(user.Email, model.Password, model.RememberMe, shouldLockout: true);
            switch (result)
            {
                case SignInStatus.Success:                    
                    srv.AddLog(user.Email, "Success", "[login]", "Info", user.NameIdentifier);
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    srv.AddLog(user.Email, "密碼錯誤超過5次", "[login]", "Info", user.NameIdentifier);
                    ModelState.AddModelError("", "密碼錯誤超過5次，帳號已被鎖定，請過5分鐘後再登入");
                    return View(model);
                    //return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                case SignInStatus.Failure:
                default:
                    srv.AddLog(model.Name, "密碼錯誤或帳號無效", "[login]", "Info", model.Password);
                    ModelState.AddModelError("", "密碼錯誤或帳號無效");
                    return View(model);
            }
        }

        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            // Require that the user has already logged in via username/password or external login
            if (!await SignInManager.HasBeenVerifiedAsync())
            {
                return View("Error");
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // The following code protects for brute force attacks against the two factor codes. 
            // If a user enters incorrect codes for a specified amount of time then the user account 
            // will be locked out for a specified amount of time. 
            // You can configure the account lockout settings in IdentityConfig
            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent: model.RememberMe, rememberBrowser: model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid code.");
                    return View(model);
            }
        }

        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email, NameIdentifier = model.NameIdentifier };
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                    // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                    // Send an email with this link
                    // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                     //var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                     //await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

                    PasswordHistory passwordHistory = new PasswordHistory();
                    passwordHistory.userEmail = model.Email;
                    passwordHistory.oldPassword1 = APCommonFun.SHA256Hash(model.Password);
                    passwordHistory.LastUpdateTime = DateTime.Now;
                    passwordHistory.round = 1;
                    carShopEntities.PasswordHistory.Add(passwordHistory);

                    UserRoles userRoles = new UserRoles();
                    userRoles.userName = model.NameIdentifier;
                    userRoles.userRole = Convert.ToInt32(model.Role);
                    userRoles.createTime = DateTime.Now;
                    userRoles.updateTime = DateTime.Now;
                    carShopEntities.UserRoles.Add(userRoles);

                    carShopEntities.SaveChanges();

                    return RedirectToAction("Index", "Account");
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateInProgress(CreateUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                string password = RandomString(9);
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email, NameIdentifier = model.NameIdentifier, EmailConfirmed = true };
                var result = await UserManager.CreateAsync(user, password);
                if (result.Succeeded)
                {
                    // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                    // Send an email with this link
                    // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    //var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    //await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

                    PasswordHistory passwordHistory = new PasswordHistory();
                    passwordHistory.userEmail = model.Email;
                    passwordHistory.oldPassword1 = APCommonFun.SHA256Hash(password);
                    passwordHistory.LastUpdateTime = DateTime.Now;
                    passwordHistory.round = 1;
                    carShopEntities.PasswordHistory.Add(passwordHistory);

                    UserRoles userRoles = new UserRoles();
                    userRoles.userName = model.NameIdentifier;
                    userRoles.userRole = Convert.ToInt32(model.Role);
                    userRoles.createTime = DateTime.Now;
                    userRoles.updateTime = DateTime.Now;
                    carShopEntities.UserRoles.Add(userRoles);

                    Profiles profile = new Profiles();
                    profile.Id = user.Id;
                    profile.Company = model.Company;
                    profile.Name = model.Name;
                    profile.Department = model.Department;
                    //profile.Mobile = model.Mobile;
                    profile.status = model.status;
                    profile.brand = model.brand;
                    profile.createTime = DateTime.Now;
                    carShopEntities.Profiles.Add(profile);

                    try
                    {
                        carShopEntities.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        TempData["MemberResult"] = ex.ToString();
                        return RedirectToAction("Create", "Account");
                    }

                    string innerMailIp = ConfigurationManager.AppSettings["mailServerIp"];
                    string innerMailPort = ConfigurationManager.AppSettings["mailServerPort"];

                    using (MailMessage mail = new MailMessage())
                    {
                        if (profile.brand == "NISSAN")
                        {
                            string urlBase = ConfigurationManager.AppSettings["nissanPath"];
                            mail.From = new MailAddress("nissanCPO@yulon-nissan.com.tw");
                        }
                        else
                        {
                            string urlBase = ConfigurationManager.AppSettings["infinitiPath"];
                            mail.From = new MailAddress("infinitiCPO@infiniti-taiwan.com");
                        }

                        mail.To.Add(new MailAddress(model.Email));
                        mail.IsBodyHtml = true;
                        mail.Subject = "CPO管理員帳號通知";
                        string roleName = carShopEntities.Roles.Where(x => x.seq == userRoles.userRole).Select(x => x.RoleName).FirstOrDefault();
                        string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                        mail.Body = "同仁您好<br><br>歡迎您成為後台管理員<br><br>姓名：" + profile.Name + "<br><br>公司法人：" + profile.Company + "<br><br>部門：" + profile.Department + "<br><br>品牌：" + profile.brand + "<br><br>角色：" + roleName + "<br><br>帳號：" + model.NameIdentifier + "<br><br><br><br>請點選連結進行密碼設定<br><br>連結：" + ConfigurationManager.AppSettings["nissanPath"] + "backend/Account/ResetPassword?first=1&userId=" +  user.Id + "&code=" + code.Replace("=", "")   + "<br><br>此為系統自動發送的郵件，請勿回覆";

                        using (SmtpClient smtp = new SmtpClient(innerMailIp))
                        {
                            smtp.Port = Convert.ToInt32(innerMailPort);
                            smtp.Send(mail);
                        }
                    }

                    TempData["MemberResult"] = "OKCreate";
                    return RedirectToAction("Index", "Account");
                }
                AddErrors(result);
            }

            var errors = ModelState.Values.SelectMany(v => v.Errors);
            // If we got this far, something failed, redisplay form
            //return View(model);

            TempData["name"] = model.Name;
            TempData["NameIdentifier"] = model.NameIdentifier;
            TempData["Email"] = model.Email;
            TempData["brand"] = model.brand;
            TempData["role"] = model.Role;
            TempData["status"] = model.status;
            TempData["company"] = model.Company;
            TempData["department"] = model.Department;

            TempData["MemberResult"] = errors;
            
            return RedirectToAction("Create", "Account");
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UpdateInProgress(RegisterViewModel model)
        {
            if (!rightSrv.checkRight("Account", "UpdateInProgress", User.Identity.Name))
            {
                return RedirectToAction("Index", "Home");
            }
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByEmailAsync(model.Email);
                user.NameIdentifier = model.NameIdentifier;

                PasswordHistory passwordHistory = carShopEntities.PasswordHistory.Where(x => x.userEmail == model.Email).FirstOrDefault();
                Profiles profile = carShopEntities.Profiles.Where(x => x.Id == user.Id).FirstOrDefault();
                string newPassword = APCommonFun.SHA256Hash(model.Password);
                if (passwordHistory != null)
                {                    
                    if (newPassword == passwordHistory.oldPassword1 || newPassword == passwordHistory.oldPassword2 || newPassword == passwordHistory.oldPassword3)
                    {
                        TempData["MemberResult"] = "same";
                        return RedirectToAction("Update", "Account", new { email = model.Email });
                    }                    
                }

                var result = await UserManager.UpdateAsync(user) ;
                if (result.Succeeded)
                {
                    string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                    var result2 = await UserManager.ResetPasswordAsync(user.Id, code, model.Password);

                    if (result2.Succeeded)
                    {
                        srv.AddLog("修改密碼", "Success", "[login]", "Info", user.NameIdentifier);
                        if (passwordHistory != null)
                        {
                            switch (passwordHistory.round)
                            {
                                case 1:
                                    passwordHistory.oldPassword2 = newPassword;
                                    passwordHistory.round = 2;
                                    break;
                                case 2:
                                    passwordHistory.oldPassword3 = newPassword;
                                    passwordHistory.round = 3;
                                    break;
                                case 3:
                                    passwordHistory.oldPassword1 = newPassword;
                                    passwordHistory.round = 1;
                                    break;
                                default:
                                    break;
                            }

                            passwordHistory.LastUpdateTime = DateTime.Now;
                            carShopEntities.PasswordHistory.Add(passwordHistory);
                        }

                        if (profile != null)
                        {
                            profile.Name = model.Name;
                            if (profile.Company != model.Company)
                            {
                                srv.AddLog("修改公司", "Success", "[login]", "Info", user.NameIdentifier);
                            }
                            profile.Company = model.Company;
                            if (profile.Department != model.Department)
                            {
                                srv.AddLog("修改部門", "Success", "[login]", "Info", user.NameIdentifier);
                            }
                            profile.Department = model.Department;
                            profile.Mobile = model.Mobile;
                            if (profile.brand != model.brand)
                            {
                                srv.AddLog("修改品牌", "Success", "[login]", "Info", user.NameIdentifier);
                            }
                            profile.brand = model.brand;
                            if (profile.status != model.status)
                            {
                                srv.AddLog("修改狀態", "Success", "[login]", "Info", user.NameIdentifier);
                            }
                            profile.status = model.status;
                        }

                        UserRoles userRoles = carShopEntities.UserRoles.Where(x => x.userName == model.NameIdentifier).FirstOrDefault();
                        if (userRoles != null)
                        {
                            if (userRoles.userRole != Convert.ToInt32(model.Role))
                            {
                                srv.AddLog("修改腳色", "Success", "[login]", "Info", user.NameIdentifier);
                            }
                            userRoles.userRole = Convert.ToInt32(model.Role);
                            userRoles.updateTime = DateTime.Now;
                            //carShopEntities.UserRoles.Add(userRoles);
                        }
                    }
                    else
                    {
                        AddErrors(result2);
                        var result2errors = ModelState.Values.SelectMany(v => v.Errors);
                        string errorResult = string.Empty;
                        foreach (var item in result2errors)
                        {
                            errorResult += item.ErrorMessage + "\n";
                        }
                        TempData["MemberUpdateFailed"] = "true";
                        TempData["MemberResult"] = errorResult;
                        return RedirectToAction("Update", "Account", new { email = model.Email });
                    }

                    carShopEntities.SaveChanges();

                    TempData["MemberResult"] = "OKUpdate";
                    return RedirectToAction("Index", "Account");
                }
                AddErrors(result);
            }

            var errors = ModelState.Values.SelectMany(v => v.Errors);
            TempData["MemberResult"] = errors;
            foreach(var item in errors)
            {
                if (item.ErrorMessage == "Password 欄位是必要項。" || item.ErrorMessage == "The Password field is required.")
                {
                    var user = await UserManager.FindByEmailAsync(model.Email);
                    user.NameIdentifier = model.NameIdentifier;
                    
                    var result = await UserManager.UpdateAsync(user);
                    if (result.Succeeded)
                    {
                        Profiles profile = carShopEntities.Profiles.Where(x => x.Id == user.Id).FirstOrDefault();
                        if (profile != null)
                        {
                            profile.Name = model.Name;
                            if (profile.Company != model.Company)
                            {
                                srv.AddLog("修改公司", "Success", "[login]", "Info", user.NameIdentifier);
                            }
                            profile.Company = model.Company;
                            if (profile.Department != model.Department)
                            {
                                srv.AddLog("修改部門", "Success", "[login]", "Info", user.NameIdentifier);
                            }
                            profile.Department = model.Department;
                            profile.Mobile = model.Mobile;
                            if (profile.status != model.status)
                            {
                                srv.AddLog("修改狀態", "Success", "[login]", "Info", user.NameIdentifier);
                            }
                            profile.status = model.status;
                            if (profile.brand != model.brand)
                            {
                                srv.AddLog("修改品牌", "Success", "[login]", "Info", user.NameIdentifier);
                            }
                            profile.brand = model.brand;
                        }

                        UserRoles userRoles = carShopEntities.UserRoles.Where(x => x.userName == model.NameIdentifier).FirstOrDefault();
                        if (userRoles != null)
                        {
                            if (userRoles.userRole != Convert.ToInt32(model.Role))
                            {
                                srv.AddLog("修改腳色", "Success", "[login]", "Info", user.NameIdentifier);
                            }
                            userRoles.userRole = Convert.ToInt32(model.Role);
                            userRoles.updateTime = DateTime.Now;
                            //carShopEntities.UserRoles.Add(userRoles);
                        }

                        carShopEntities.SaveChanges();

                        TempData["MemberResult"] = "OKUpdate";
                        return RedirectToAction("Index", "Account");
                    }
                }
            }
            return RedirectToAction("Update", "Account", new { email = model.Email });
        }


        [AllowAnonymous]
        public ActionResult Delete(string email, string name)
        {
            if (!rightSrv.checkRight("Account", "Delete", User.Identity.Name))
            {
                return RedirectToAction("Index", "Home");
            }
            if (ModelState.IsValid)
            {
                {
                    var user = carShopEntities.AspNetUsers.Where(x => x.Email == email).FirstOrDefault();
                    carShopEntities.AspNetUsers.Remove(user);

                    var passwordHistory = carShopEntities.PasswordHistory.Where(x => x.userEmail == email);
                    carShopEntities.PasswordHistory.RemoveRange(passwordHistory);

                    var userRoles = carShopEntities.UserRoles.Where(x => x.userName == name);
                    carShopEntities.UserRoles.RemoveRange(userRoles);

                    var profile = carShopEntities.Profiles.Where(x => x.Id == user.Id);
                    carShopEntities.Profiles.RemoveRange(profile);

                    carShopEntities.SaveChanges();

                    return RedirectToAction("Index", "Account");
                }
            }

            // If we got this far, something failed, redisplay form
            return View();
        }

        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            var cookie = HttpContext.Request.Cookies[AntiForgeryConfig.CookieName];
            AntiForgery.Validate(cookie != null ? cookie.Value : null, model.__RequestVerificationToken);

            if (ModelState.IsValid)
            {
                //string password = RandomString(9);
                var user = await UserManager.FindByNameAsync(model.Email);
                if (user == null)
                {
                    // Don't reveal that the user does not exist
                    return RedirectToAction("ResetPasswordConfirmation", "Account");
                }

                string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                //var result = await UserManager.ResetPasswordAsync(user.Id, code, password);

                //if (result.Succeeded)
                {
                    var dicValue = new Dictionary<string, string>();

                  
                    string sql = "select * from [Profiles] where Id=@Id ";
                    dicValue.Add("@Id", user.Id);
                    DataTable dt = APCommonFun.GetDataTable_MSSQL(sql, dicValue);
                    if (dt.Rows.Count > 0)
                    {
                        string status = dt.Rows[0]["status"].ToString();
                        if (status == "False")
                        {
                            TempData["MemberResult"] = "帳號已停用!";
                        }
                        else
                        {
                            sql = "update AspNetUsers set EmailConfirmed='1' where email=@email ";
                            dicValue.Clear();
                            dicValue.Add("@email", model.Email);
                            APCommonFun.ExecSqlCommand_MSSQL(sql, dicValue);

                            string innerMailIp = ConfigurationManager.AppSettings["mailServerIp"]; 
                            string innerMailPort = ConfigurationManager.AppSettings["mailServerPort"]; 

                            using (MailMessage mail = new MailMessage())
                            {
                                string brand = brandSrv.getBrand(model.Email);
                                if (brand == "NISSAN")
                                {
                                    string urlBase = ConfigurationManager.AppSettings["nissanPath"];
                                    mail.From = new MailAddress("nissanCPO@yulon-nissan.com.tw");
                                }
                                else
                                {
                                    string urlBase = ConfigurationManager.AppSettings["infinitiPath"];
                                    mail.From = new MailAddress("infinitiCPO@infiniti-taiwan.com");
                                }
                                mail.To.Add(new MailAddress(model.Email));
                                mail.IsBodyHtml = true;
                                mail.Subject = "重新設定密碼通知";
                                string link = ConfigurationManager.AppSettings["nissanPath"] + "backend/Account/ResetPassword?first=0&userId=" + user.Id + "&code=" + code.Replace("=", "");
                                mail.Body = "同仁您好<br><br>請點選連結重新設定密碼<br><br>連結：" + link + "<br><br>連結有效時間為 30 分鐘<br><br><br><br>此為系統自動發送的郵件，請勿回覆";
                                ResetPwLinks reset = new ResetPwLinks();
                                reset.link = link;
                                reset.createTime = DateTime.Now;
                                carShopEntities.ResetPwLinks.Add(reset);
                                carShopEntities.SaveChanges();

                                using (SmtpClient smtp = new SmtpClient(innerMailIp))
                                {
                                    smtp.Port = Convert.ToInt32(innerMailPort);
                                    smtp.Send(mail);
                                }
                            }
                            TempData["MemberResult"] = "已發送郵件通知修改密碼連結!";
                        }
                    }   
                }                
            }
            else
            {
                TempData["MemberResult"] = "臨時密碼產生失敗!";
            }
            // If we got this far, something failed, redisplay form
            return RedirectToAction("Login", "Account");
        }

        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        [AllowAnonymous]
        public async Task<ActionResult> ToResetPassword(string mail)
        {
            var user = await UserManager.FindByNameAsync(mail);
            string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
            return RedirectToAction("ResetPassword", "Account", new { userId = user.Id, code = code });
        }

        [AllowAnonymous]
        public ActionResult ResetPassword(string first, string userId, string code)
        {
            if (first == "1")
            {
                TempData["ResetPasswordWord"] = "第一次登入成功";
            }
            else if(first == "0")
            {
                string link = ConfigurationManager.AppSettings["nissanPath"] + "backend/Account/ResetPassword?first=" + first + "&userId=" + userId + "&code=" + code.Replace(" ", "+");
                var resetLink = carShopEntities.ResetPwLinks.Where(x => x.link == link).FirstOrDefault();
                if (resetLink == null)
                {
                    TempData["MemberResult"] = "連結不存在!!，請確認Email中的內容";
                    return RedirectToAction("Login", "Account");
                }
                else
                {
                    DateTime linkTime = resetLink.createTime ?? default(DateTime);
                    if (linkTime.AddMinutes(30) < DateTime.Now)
                    {
                        TempData["MemberResult"] = "連結已失效";
                        return RedirectToAction("Login", "Account");
                    }
                }
            }
            TempData["ResetPasswordId"] = userId;
            ResetPasswordViewModel model = new ResetPasswordViewModel();
            return code == null ? View("Error") : View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(model.Password))
                {
                    model.errorMsg = "請輸入密碼";
                }

                if (string.IsNullOrEmpty(model.ConfirmPassword))
                {
                    model.errorMsg = "請輸入確認密碼";
                }

                if (!string.IsNullOrEmpty(model.ConfirmPassword) && !string.IsNullOrEmpty(model.Password))
                {
                    if (model.Password != model.ConfirmPassword)
                    {
                        model.errorMsg = "密碼與確認密碼需相同";
                    }
                }

                if (model.Password.Length < 8)
                {
                    model.errorMsg = "密碼長度應至少8碼以上";
                }

                TempData["ResetPasswordId"] = model.Id;
                return View(model);
            }

            if (model.Password.Length < 8)
            {
                model.errorMsg = "密碼長度應至少8碼以上";
                TempData["ResetPasswordId"] = model.Id;
                return View(model);
            }

            if (!Regex.IsMatch(model.Password, "[A-Z]"))
            {
                model.errorMsg = "密碼至少要有一個大寫字母";
                TempData["ResetPasswordId"] = model.Id;
                return View(model);
            }

            if (!Regex.IsMatch(model.Password, "[a-z]"))
            {
                model.errorMsg = "密碼至少要有一個小寫字母";
                TempData["ResetPasswordId"] = model.Id;
                return View(model);
            }

            if (!Regex.IsMatch(model.Password, "[0-9]"))
            {
                model.errorMsg = "密碼至少要有一個數字";
                TempData["ResetPasswordId"] = model.Id;
                return View(model);
            }


            var user = await UserManager.FindByIdAsync(model.Id);
            if (model.Password == user.NameIdentifier)
            {
                model.errorMsg =  "密碼不可與帳號相同!";
                string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                return RedirectToAction("ResetPassword", "Account", new { userId = user.Id, code = code });
            }

            var passwordHistory = carShopEntities.PasswordHistory.Where(x => x.userEmail == user.Email).FirstOrDefault();
            var aspuser = carShopEntities.AspNetUsers.Where(x => x.Email == user.Email).FirstOrDefault();
            if (passwordHistory != null)
            {
                string newPassword = APCommonFun.SHA256Hash(model.Password);
                if (!string.IsNullOrEmpty(passwordHistory.oldPassword1))
                { 
                    if(newPassword == passwordHistory.oldPassword1)                         
                    {
                        model.errorMsg = "密碼不可與前3次相同";
                        TempData["ResetPasswordId"] = model.Id;
                        return View(model);
                    }
                }

                if (!string.IsNullOrEmpty(passwordHistory.oldPassword2))
                {
                    if (newPassword == passwordHistory.oldPassword2)
                    {
                        model.errorMsg = "密碼不可與前3次相同";
                        TempData["ResetPasswordId"] = model.Id;
                        return View(model);
                    }
                }

                if (!string.IsNullOrEmpty(passwordHistory.oldPassword3))
                {
                    if (newPassword == passwordHistory.oldPassword3)
                    {
                        model.errorMsg = "密碼不可與前3次相同";
                        TempData["ResetPasswordId"] = model.Id;
                        return View(model);
                    }
                }
            }

            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }

            string resetCode = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
            string codeInput = string.Empty;
            //if (TempData["ResetPasswordWord"] == null)
            //{
            //    codeInput = resetCode;
            //}
            //else
            //{
            //    codeInput = model.Code;
            //}
            codeInput = resetCode;

            var result = await UserManager.ResetPasswordAsync(user.Id, codeInput, model.Password);
            if (result.Succeeded)
            {                
                if (passwordHistory != null)
                {
                    switch (passwordHistory.round)
                    {
                        case 1:
                            passwordHistory.oldPassword2 = APCommonFun.SHA256Hash(model.Password);               
                            passwordHistory.round = 2;
                            break;
                        case 2:
                            passwordHistory.oldPassword3 = APCommonFun.SHA256Hash(model.Password);
                            passwordHistory.round = 3;
                            break;
                        case 3:
                            passwordHistory.oldPassword1 = APCommonFun.SHA256Hash(model.Password);
                            passwordHistory.round = 1;
                            break;
                        default:
                            break;
                    }
                    passwordHistory.LastUpdateTime = DateTime.Now;
                }

                if (aspuser != null)
                {
                    aspuser.EmailConfirmed = false;
                }

                carShopEntities.SaveChanges();
                //TempData["MemberResult"] = "密碼重設完成，請重新登入";
                await SignInManager.PasswordSignInAsync(user.Email, model.Password, true, shouldLockout: true);
                //await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                return RedirectToAction("Index", "Account");
            }
            AddErrors(result);
            string err = string.Empty;
            foreach (var item in result.Errors)
            {
                err = item.Replace("Passwords must be at least ", "密碼至少要輸入");
                err = err.Replace(" characters", "碼");
                err = err.Replace("Passwords must have at least one uppercase ('A'-'Z').", "密碼至少要有一個大寫字母。");
                err = err.Replace("Passwords must have at least one lowercase ('a'-'z').", "密碼至少要有一個小寫字母。");
                err = err.Replace("Passwords must have at least one digit ('0'-'9').", "密碼至少要有一個數字。");
            }
            model.errorMsg = err;
            TempData["ResetPasswordId"] = model.Id;
            return View();
        }

        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        {
            var userId = await SignInManager.GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return View("Error");
            }
            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // Generate the token and send it
            if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return View("Error");
            }
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }

        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
                case SignInStatus.Failure:
                default:
                    // If the user does not have an account, then prompt the user to create an account
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        private static Random random = new Random();

        public static string RandomString(int length)
        {
            const string chars1 = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string chars2 = "abcdefghijklmnopqrstuvwxyz";
            const string chars3 = "0123456789";
            return new string(Enumerable.Repeat(chars1, length / 3)
                .Select(s => s[random.Next(s.Length)]).ToArray()) + new string(Enumerable.Repeat(chars2, length / 3)
                .Select(s => s[random.Next(s.Length)]).ToArray()) + new string(Enumerable.Repeat(chars3, length / 3)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                string err = error.Replace("Passwords must be at least ", "密碼至少要輸入");
                err = err.Replace(" characters", "碼");
                err = err.Replace("Passwords must have at least one uppercase ('A'-'Z').", "密碼至少要有一個大寫字母。");
                err = err.Replace("Passwords must have at least one lowercase ('a'-'z').", "密碼至少要有一個小寫字母。");
                err = err.Replace("Passwords must have at least one digit ('0'-'9').", "密碼至少要有一個數字。");
                ModelState.AddModelError("", err);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}