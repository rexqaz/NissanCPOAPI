using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
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
    public class MemberController : Controller
    {
        CarShopMemberEntities carShopMemberEntities = new CarShopMemberEntities();
        CarShopEntities carShopEntities = new CarShopEntities();
        private LogService srv = new LogService();
        private RightService rightSrv = new RightService();
        private BrandService brandSrv = new BrandService();
        private OneidService oneidService = new OneidService();

        private const string _exportPath = "../XingUpdateFile/TemporaryFile/Member/";

        public class MemberBehavior
        {
            public long seq { get; set; }
            public string mobile { get; set; }
            public string behavior { get; set; }
            public string brand { get; set; }
            public DateTime createTime { get; set; }

        }

        // GET: Member
        public ActionResult Index()
        {
            if (!rightSrv.checkRight("Member", "Index", User.Identity.Name))
            {
                return RedirectToAction("Index", "Home");
            }

            if (!rightSrv.checkRight("Member", "Update", User.Identity.Name))
            {
                TempData["UpdateRight"] = "False";
            }
            else
            {
                TempData["UpdateRight"] = "True";
            }
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> GetMemberData(FormCollection post)
        {
            var draw = Request.Form["draw"].FirstOrDefault();
            string start = Request.Form["start"];
            string length = Request.Form["length"];
            string keyword = post["Keyword"];
            string dateFilter = post["dateFilter"];
            string statusFilter = post["status"];
            string sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"];
            string sortColumnDirection = Request.Form["order[0][dir]"];
            string searchValue = Request.Form["search[value]"];
            int pageSize = Convert.ToInt32(length);
            int skip = Convert.ToInt32(start);
            int recordsTotal = 0;
           
           

            var members = GetMemberList();
            if (!string.IsNullOrEmpty(dateFilter))
            {
                string[] dateArray = dateFilter.Split(new string[] { " ~ " }, StringSplitOptions.None);
                string[] minArray = dateArray[0].Split('-');
                string[] maxArray = dateArray[1].Split('-');

                DateTime min = DateTime.Parse(dateArray[0]);
                DateTime max = DateTime.Parse(dateArray[1]);
                members = members.Where(x => x.createTime >= min && x.createTime <= max).ToList(); ;
            }

            if (!string.IsNullOrEmpty(statusFilter))
            {
                members = members.Where(x => x.status.ToString() == statusFilter).ToList();
            }

            if (sortColumn == "建立日期")
            {
                if (sortColumnDirection == "asc") members = members.OrderBy(x => x.createTime).ToList();
                else members = members.OrderByDescending(x => x.createTime).ToList();
            }
            else if (sortColumn == "姓名")
            {
                if (sortColumnDirection == "asc") members = members.OrderBy(x => x.name).ToList();
                else members = members.OrderByDescending(x => x.name).ToList();
            }
            else if (sortColumn == "手機")
            {
                if (sortColumnDirection == "asc") members = members.OrderBy(x => x.mobile).ToList();
                else members = members.OrderByDescending(x => x.mobile).ToList();
            }
            else if (sortColumn == "Email")
            {
                if (sortColumnDirection == "asc") members = members.OrderBy(x => x.mobile).ToList();
                else members = members.OrderByDescending(x => x.mobile).ToList();
            }
            else if (sortColumn == "稱謂")
            {
                if (sortColumnDirection == "asc") members = members.OrderBy(x => x.title).ToList();
                else members = members.OrderByDescending(x => x.title).ToList();
            }
            else if (sortColumn == "狀態")
            {
                if (sortColumnDirection == "asc") members = members.OrderBy(x => x.status).ToList();
                else members = members.OrderByDescending(x => x.status).ToList();
            }
            
            if (!string.IsNullOrEmpty(keyword))
            {
                var shop1 = members.Where(x => x.createTime != null && x.createTime.ToString().Contains(keyword));
                var shop2 = members.Where(x => x.name != null && x.name.Contains(keyword));
                var shop3 = members.Where(x => x.mobile != null && x.mobile.Contains(keyword));
                var shop4 = members.Where(x => x.email != null && x.email.Contains(keyword));
                var shop5 = members.Where(x => x.title != null && x.title.Contains(keyword));
                var shop6 = members.Where(x => x.status != null && x.status.ToString().Contains(keyword));
                members = shop1.Concat(shop2).Concat(shop3).Concat(shop4).Concat(shop5).Concat(shop6).ToList();
            }


            var data = members.Skip(skip).Take(pageSize).ToList();
            var jsonData = new { draw = draw, recordsFiltered = members.Count, recordsTotal = members.Count, data = data };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<JsonResult> GetStatistics(FormCollection post)
        {
            string keyword = post["Keyword"];
            string dateFilter = post["dateFilter"];
            string statusFilter = post["status"];


            var members = GetMemberList();
            if (!string.IsNullOrEmpty(dateFilter))
            {
                string[] dateArray = dateFilter.Split(new string[] { " - " }, StringSplitOptions.None);
                string[] minArray = dateArray[0].Split('/');
                string[] maxArray = dateArray[1].Split('/');

                DateTime min = DateTime.Parse(minArray[2] + "/" + minArray[0] + "/" + minArray[1]);
                DateTime max = DateTime.Parse(maxArray[2] + "/" + maxArray[0] + "/" + maxArray[1]).AddDays(1);
                members = members.Where(x => x.createTime >= min && x.createTime <= max).ToList(); ;
            }

            if (!string.IsNullOrEmpty(statusFilter))
            {
                members = members.Where(x => x.status.ToString() == statusFilter).ToList();
            }

            if (!string.IsNullOrEmpty(keyword))
            {
                var shop1 = members.Where(x => x.createTime != null && x.createTime.ToString().Contains(keyword));
                var shop2 = members.Where(x => x.name != null && x.name.Contains(keyword));
                var shop3 = members.Where(x => x.mobile != null && x.mobile.Contains(keyword));
                var shop4 = members.Where(x => x.email != null && x.email.Contains(keyword));
                var shop5 = members.Where(x => x.title != null && x.title.Contains(keyword));
                var shop6 = members.Where(x => x.status != null && x.status.ToString().Contains(keyword));
                members = shop1.Concat(shop2).Concat(shop3).Concat(shop4).Concat(shop5).Concat(shop6).ToList();
            }

            var jsonData = new { memberCount = members.Count };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<JsonResult> GetBehaviorData(FormCollection post)
        {
            var draw = Request.Form["draw"].FirstOrDefault();
            string seq = post["seq"];
            int recordsTotal = 0;
            string brand = brandSrv.getBrand(User.Identity.Name);


            List<BehaviorViewModel> behaviorRecords = new List<BehaviorViewModel>();
            BehaviorViewModel tmp = new BehaviorViewModel();
            var member = GetMember(seq);
            var behaviors = GetMembersBehavior(member.seq);
            var groupDate = behaviors.GroupBy(x => x.createTime.ToString("yyyyMMdd")).Select(x => x.Key).ToList();
            var count1 = 0;
            var count2 = 0;
            var count3 = 0;
            var count4 = 0;
            var count5 = 0;
            foreach (var item in behaviors)
            {
                tmp = new BehaviorViewModel();
                tmp.date = item.createTime;
                tmp.behavior = item.behavior;
                if (item.brand == brand)
                {
                    var idx = behaviorRecords.FindIndex(x =>
                                  x.date.ToString("yyyyMMdd") == item.createTime.ToString("yyyyMMdd") &&
                                  x.behavior.Contains(item.behavior));

                    switch (item.behavior)
                    {
                        case "關注車輛":
                            {
                                count1++;
                                tmp.behavior = item.behavior + "({count1})";
                                if (idx == -1)                                
                                    behaviorRecords.Add(tmp);                                                                
                            }
                            break;
                        case "條件訂閱":
                            {
                                count2++;
                                tmp.behavior = item.behavior + "({count2})";
                                if (idx == -1)
                                    behaviorRecords.Add(tmp);
                            }
                            break;
                        case "預付保留":
                            {
                                count3++;
                                tmp.behavior = item.behavior + "({count3})";
                                if (idx == -1)
                                    behaviorRecords.Add(tmp);
                            }
                            break;
                        case "比較車輛":
                            {
                                count4++;
                                tmp.behavior = item.behavior + "({count4})";
                                if (idx == -1)
                                    behaviorRecords.Add(tmp);
                            }
                            break;
                        case "預約賞車":
                            {
                                count5++;
                                tmp.behavior = item.behavior + "({count5})";
                                if (idx == -1)
                                    behaviorRecords.Add(tmp);
                            }
                            break;
                        default:
                            behaviorRecords.Add(tmp);
                            break;
                    }
                }
                else
                {
                    if (item.behavior.Contains("修改"))
                    {
                        behaviorRecords.Add(tmp);
                    }
                }
            }

            foreach (var item in behaviorRecords)
            {
                item.behavior = item.behavior
                    .Replace("{count1}", count1.ToString())
                    .Replace("{count2}", count2.ToString())
                    .Replace("{count3}", count3.ToString())
                    .Replace("{count4}", count4.ToString())
                    .Replace("{count5}", count5.ToString());
            }
            

            var jsonData = new { draw = draw, recordsFiltered = behaviorRecords.Count, recordsTotal = behaviorRecords.Count, data = behaviorRecords };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<JsonResult> GetNoticeData(FormCollection post)
        {
            var draw = Request.Form["draw"].FirstOrDefault();
            //string start = Request.Form["start"];
            //string length = Request.Form["length"];
            //int pageSize = Convert.ToInt32(length);
            //int skip = Convert.ToInt32(start);
            
            string seq = post["seq"];
            int recordsTotal = 0;

            List<NoticeRecordViewModel> noticeRecords = new List<NoticeRecordViewModel>();
            
            var member = GetMember(seq);

            //var member = carShopMemberEntities.Members.Where(x => x.seq.ToString() == seq).FirstOrDefault();
            string sql = "SELECT b.NoticeTime, c.* FROM [CarShop].[dbo].[NoticeRecords] b inner join[CarShop].[dbo].[Shops] c on b.shopNo = c.ShopNo where b.member = @mobile ";

            var dicValue = new Dictionary<string, string>();
            dicValue.Add("@mobile", member.mobile);

            DataTable dt = APCommonFun.GetDataTable_MSSQL(sql, dicValue);
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                NoticeRecordViewModel tmp = new NoticeRecordViewModel();
                tmp.price = dt.Rows[i]["price"].ToString();
                tmp.carModal = dt.Rows[i]["carModel"].ToString();
                tmp.yearOfManufacture = dt.Rows[i]["yearOfManufacture"].ToString();
                tmp.dealer = dt.Rows[i]["dealer"].ToString();
                tmp.NoticeTime = DateTime.Parse(dt.Rows[i]["NoticeTime"].ToString());
                noticeRecords.Add(tmp);
            }

            //var data = noticeRecords.Skip(skip).Take(pageSize).ToList();
            var jsonData = new { draw = draw, recordsFiltered = noticeRecords.Count, recordsTotal = noticeRecords.Count, data = noticeRecords };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetSubscribeData(FormCollection post)
        {
            var draw = Request.Form["draw"].FirstOrDefault();
            string seq = post["seq"];

            List<Subscriptions> subscriptions = carShopEntities.Subscriptions.Where(x => x.user_id.ToString() == seq.ToString()).ToList(); 
            //var data = noticeRecords.Skip(skip).Take(pageSize).ToList();
            var jsonData = new { draw = draw, recordsFiltered = subscriptions.Count, recordsTotal = subscriptions.Count, data = subscriptions };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetPrepaidData(FormCollection post)
        {
            var draw = Request.Form["draw"].FirstOrDefault();
            string seq = post["seq"];

            List<PrepaidDataViewModel> model = new List<PrepaidDataViewModel>();
            List<Prepaid> prepaids = carShopEntities.Prepaid.Where(x => x.user_id.ToString() == seq.ToString()).ToList();
            foreach (var item in prepaids)
            {
                var shop = carShopEntities.Shops.Where(x => x.ShopNo == item.shopNo).FirstOrDefault();
                PrepaidDataViewModel tmp = new PrepaidDataViewModel();
                tmp.prepaidMoney = "3000";
                tmp.createTime = item.createTime ?? default(DateTime);
                tmp.prepaidNo = item.prepaidNo;
                if (shop != null)
                {
                    tmp.price = shop.price.ToString();
                    tmp.yearOfManufacture = shop.yearOfManufacture.ToString();
                    tmp.dealer = shop.dealer;
                }

                model.Add(tmp);
            }

            //var data = noticeRecords.Skip(skip).Take(pageSize).ToList();
            var jsonData = new { draw = draw, recordsFiltered = model.Count, recordsTotal = model.Count, data = model };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }
        public async Task<ActionResult> Update(string seq)
        {
            if (!rightSrv.checkRight("Member", "Update", User.Identity.Name))
            {
                return RedirectToAction("Index", "Home");
            }

            if (!rightSrv.checkRight("Member", "UpdateInProgress", User.Identity.Name))
            {
                TempData["UpdateRight"] = "False";
            }
            else
            {
                TempData["UpdateRight"] = "True";
            }

            if (!rightSrv.checkRight("Member", "Delete", User.Identity.Name))
            {
                TempData["DeleteRight"] = "False";
            }
            else
            {
                TempData["DeleteRight"] = "True";
            }          

            MemberUpdateViewModel model = new MemberUpdateViewModel();
            model.updatePermission = false;

            var apiMember = GetMember(seq.ToString());
            model.member = new Members()
            {
                seq = apiMember.seq,
                name = apiMember.name,
                email = apiMember.email,
                password = apiMember.password,
                createTime = apiMember.createTime,
                updateTime = apiMember.updateTime,
                mobile = apiMember.mobile,
                title = apiMember.title,
                birthday = apiMember.birthday,
                address = apiMember.address,
                interestedCar = apiMember.interestedCar,
                isMailVerify = apiMember.isMailVerify,
                isMobileVerify = apiMember.isMobileVerify,
                id = apiMember.id,
                needToChangeFirst = apiMember.needToChangeFirst,
                status = apiMember.status,
                brand = apiMember.brand,
                area = apiMember.area,
            };

            var user = carShopEntities.AspNetUsers.Where(x => x.Email == User.Identity.Name).FirstOrDefault();
            if (user != null)
            {
                int role = carShopEntities.UserRoles.Where(x => x.userName == user.NameIdentifier).Select(x => x.userRole).FirstOrDefault() ?? 0;
                model.role = carShopEntities.Roles.Where(x => x.seq == role).Select(x => x.RoleName).FirstOrDefault() ?? string.Empty;

                var roleMenus = carShopEntities.RoleMenus.Where(x => x.roleId == role).ToList();
                int updateMemberMenuId = carShopEntities.Menus.Where(x => x.menuName == "會員管理-會員修改(U)").Select(x => x.seq).FirstOrDefault();
                foreach (var menu in roleMenus)
                {
                    if (menu.menuId == updateMemberMenuId)
                    {
                        model.updatePermission = true;
                        break;
                    }
                }
            }
            
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> UpdateInProgress(Members model)
        {
            if (!rightSrv.checkRight("Member", "UpdateInProgress", User.Identity.Name))
            {
                return RedirectToAction("Index", "Home");
            }
            try
            {                
                var member = GetMember(model.seq.ToString());
                if (member.mobile == "0999888777")
                {
                    //先暫時用這組帳號當作跟one-id溝通的
                    TempData["MemberResult"] = "測試帳號禁止修改";
                    return RedirectToAction("Index", "Member");
                }


                if (member != null)
                {                    
                    var para = new OneidService.UpdateMemberParameter()
                    {
                        seq = member.seq.ToString(),
                        email = model.email,
                        password = model.password,
                        mobile = model.mobile,
                        status = model.status.ToString()
                    };
                    UpdateMember(para);

                    srv.AddLog("修改狀態: " + member.status + " -> " + model.status + ", email: " + member.email + " -> " + model.email + ", mobile: " + member.mobile + " -> " + model.mobile + ", password -> " + model.password, "success", "[Member/Update]", "Info", User.Identity.Name);
                    
                }
                
                TempData["MemberResult"] = "OKUpdate";
                return RedirectToAction("Update", "Member", new { seq = member.seq });
            }
            catch (Exception ex)
            {
                TempData["MemberResult"] = ex.ToString();
                return RedirectToAction("Index", "Member");
            }
        }

        public async  Task<ActionResult> Delete(string mobile)
        {
            if (!rightSrv.checkRight("Member", "Delete", User.Identity.Name))
            {
                return RedirectToAction("Index", "Home");
            }
            try
            {
                if (mobile == "0999888777")
                {
                    //先暫時用這組帳號當作跟one-id溝通的
                    TempData["MemberResult"] = "測試帳號禁止刪除";
                    return RedirectToAction("Index", "Member");
                }


                DeleteMember(mobile);                
            }
            catch (Exception ex)
            {
                TempData["MemberResult"] = ex.ToString();
                return RedirectToAction("Index", "Member");
            }

            TempData["MemberResult"] = "OKDelete";
            return RedirectToAction("Index", "Member");
        }

        

        public async Task<ActionResult> ExportCSV(FormCollection post)
        {
            if (!rightSrv.checkRight("Member", "ExportCSV", User.Identity.Name))
            {
                return Content("NO");
            }


            string strFileName = "會員管理列表.csv";

            string keyword = post["Keyword"];
            string dateFilter = post["dateFilter"];
            string statusFilter = post["status"];
            
            var members = GetMemberList();
            if (!string.IsNullOrEmpty(dateFilter))
            {
                string[] dateArray = dateFilter.Split(new string[] { " ~ " }, StringSplitOptions.None);
                string[] minArray = dateArray[0].Split('-');
                string[] maxArray = dateArray[1].Split('-');

                DateTime min = DateTime.Parse(dateArray[0]);
                DateTime max = DateTime.Parse(dateArray[1]);
                members = members.Where(x => x.createTime >= min && x.createTime <= max).ToList(); ;
            }

            if (!string.IsNullOrEmpty(statusFilter))
            {
                members = members.Where(x => x.status.ToString() == statusFilter).ToList();
            }
            
            if (!string.IsNullOrEmpty(keyword))
            {
                var shop1 = members.Where(x => x.createTime != null && x.createTime.ToString().Contains(keyword));
                var shop2 = members.Where(x => x.name != null && x.name.Contains(keyword));
                var shop3 = members.Where(x => x.mobile != null && x.mobile.Contains(keyword));
                var shop4 = members.Where(x => x.email != null && x.email.Contains(keyword));
                var shop5 = members.Where(x => x.title != null && x.title.Contains(keyword));
                var shop6 = members.Where(x => x.status != null && x.status.ToString().Contains(keyword));
                members = shop1.Concat(shop2).Concat(shop3).Concat(shop4).Concat(shop5).Concat(shop6).ToList();
            }

            var result = members;

            //before your loop
            var csv = new StringBuilder();
            csv.AppendLine("建立日期,姓名,手機,Email,稱謂,狀態");
            foreach (var item in result)
            {
                //in your loop
                var newLine = string.Format("{0},{1},{2},{3},{4},{5}", item.createTime, item.name, item.mobile, item.email, item.title, item.status);
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
            System.IO.File.WriteAllText(path, csv.ToString(), Encoding.Default);

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
                Response.AddHeader("Content-Disposition", "attachment; filename=" + "會員管理列表" + DateTime.Today.ToString("yyyyMMddHHmmss") + ".csv");

                Response.BinaryWrite(memStream.ToArray());
                Response.Flush();
                Response.Close();
                Response.End();
            }

            System.IO.File.Delete(Path.Combine(Server.MapPath(_exportPath), Session["genFileName"].ToString()));

            return RedirectToAction("Index", "Member");
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

        

        private DataModels.MembersModel GetMember(string member_seq)
        {
            string brand = brandSrv.getBrand(User.Identity.Name);
            var aToken = GetActiveToken();
            return oneidService.GetMember(aToken, member_seq).Data;
        }


        /// <summary>
        /// 取得使用者列表
        /// </summary>
        /// <returns></returns>
        private List<DataModels.MembersModel> GetMemberList()
        {
            string brand = brandSrv.getBrand(User.Identity.Name);
            var aToken = GetActiveToken();
            return oneidService.GetMemberList(aToken, brand).Data;
        }

        /// <summary>
        /// 取得全部使用者列表
        /// </summary>
        /// <returns></returns>
        private List<DataModels.MembersModel> GetAllMemberList()
        {
            var aToken = GetActiveToken();
            return oneidService.GetMemberList(aToken).Data;
        }

        private List<MemberBehavior> GetMembersBehavior(long member_seq)
        {
            var aToken = GetActiveToken();
            return oneidService.GetMembersBehavior(aToken, member_seq).Data;
        }

        private void UpdateMember(OneidService.UpdateMemberParameter para)
        {            
            var aToken = GetActiveToken();
            oneidService.UpdateMember(aToken, para);
        }

        private void DeleteMember(string mobile)
        {            
            var aToken = GetActiveToken();
            oneidService.DeleteMember(aToken, mobile);
        }
    }
}