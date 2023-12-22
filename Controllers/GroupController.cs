using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication.Models;
using WebApplication.Services;

namespace WebApplication.Controllers
{
    public class GroupController : Controller
    {
        CarShopEntities carShopEntities = new CarShopEntities();
        private RightService rightSrv = new RightService();
        // GET: Group
        public ActionResult Index()
        {
            if (!rightSrv.checkRight("Group", "Index", User.Identity.Name))
            {
                return RedirectToAction("Index", "Home");
            }

            if (!rightSrv.checkRight("Group", "Update", User.Identity.Name))
            {
                TempData["UpdateRight"] = "False";
            }
            else
            {
                TempData["UpdateRight"] = "True";
            }

            if (!rightSrv.checkRight("Group", "Update", User.Identity.Name))
            {
                TempData["CreateRight"] = "False";
            }
            else
            {
                TempData["CreateRight"] = "True";
            }
            List<Roles> model = carShopEntities.Roles.OrderByDescending(x => x.createTime).ToList();
            return View(model);
        }

        [HttpPost]
        public JsonResult GetGroupData(FormCollection post)
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

            List<Roles> roles = carShopEntities.Roles.ToList();
            foreach (var role in roles)
            {
                role.Description = role.status == true ? "啟用" : "停用";
            }
            if (!string.IsNullOrEmpty(dateFilter))
            {
                string[] dateArray = dateFilter.Split(new string[] { " ~ " }, StringSplitOptions.None);
                string[] minArray = dateArray[0].Split('/');
                string[] maxArray = dateArray[1].Split('/');

                DateTime min = DateTime.Parse(dateArray[0]);
                DateTime max = DateTime.Parse(dateArray[1]);
                roles = roles.Where(x => x.createTime >= min && x.createTime <= max).ToList(); ;
            }


            if (!string.IsNullOrEmpty(keyword))
            {
                var shop1 = roles.Where(x => x.createTime != null && x.createTime.ToString().Contains(keyword));
                var shop2 = roles.Where(x => x.RoleName != null && x.RoleName.Contains(keyword));
                var shop3 = roles.Where(x => x.Description != null && x.Description.Contains(keyword));
                roles = shop1.Concat(shop2).Concat(shop3).ToList();
            }


            var data = roles.Skip(skip).Take(pageSize).ToList();
            var jsonData = new { draw = draw, recordsFiltered = roles.Count, recordsTotal = roles.Count, data = data };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Update(string seq)
        {
            if (!rightSrv.checkRight("Group", "Update", User.Identity.Name))
            {
                return RedirectToAction("Index", "Home");
            }

            if (!rightSrv.checkRight("Group", "UpdateInProgress", User.Identity.Name))
            {
                TempData["UpdateRight"] = "False";
            }
            else
            {
                TempData["UpdateRight"] = "True";
            }

            if (!rightSrv.checkRight("Group", "Delete", User.Identity.Name))
            {
                TempData["DeleteRight"] = "False";
            }
            else
            {
                TempData["DeleteRight"] = "True";
            }
            var roleMenus = carShopEntities.RoleMenus.Where(x => x.roleId.ToString() == seq).Select(x => x.menuId).ToList();
            var roleStatus = carShopEntities.Roles.Where(x => x.seq.ToString() == seq).Select(x => x.status).FirstOrDefault();
            GroupViewModel model = new GroupViewModel();
            model.roleId = seq;
            model.status = roleStatus ?? default(bool);
            model.groups = new List<Group>();
            var menus = carShopEntities.Menus.OrderBy(x => x.orderSeq).ToList();

            foreach (var item in menus)
            {
                Group tmp = new Group();
                tmp.id = item.seq.ToString();
                tmp.menuName = item.menuName;
                if (roleMenus.Contains(item.seq))
                {
                    tmp.isChecked = true;
                }
                else
                {
                    tmp.isChecked = false;
                }
                
                if (item.isRoot ?? default(bool))
                {
                    tmp.subMenus = new List<Group>();
                    model.groups.Add(tmp);
                }
                else
                {
                    Group parent = model.groups.Where(x => x.id == item.parent.ToString()).FirstOrDefault();
                    if (parent != null)
                    {
                        parent.subMenus.Add(tmp);
                    }
                }
            }

            if (roleMenus == null || roleMenus.Count == 0)
            {
                TempData["hasMenu"] = "False";
            }
            else
            {
                TempData["hasMenu"] = "True";
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult UpdateInProgress(FormCollection form, string roleId)
        {
            if (!rightSrv.checkRight("Group", "UpdateInProgress", User.Identity.Name))
            {
                return RedirectToAction("Index", "Home");
            }
            var permission = form["permission[]"];
            var status = form["status"];

            if (permission != null)
            {
                var roleMenus = carShopEntities.RoleMenus.Where(x => x.roleId.ToString() == roleId);
                carShopEntities.RoleMenus.RemoveRange(roleMenus);

                List<string> permissions = permission.Split(',').ToList();
                foreach (var item in permissions)
                {
                    RoleMenus newItem = new RoleMenus();
                    newItem.roleId = Convert.ToInt32(roleId);
                    newItem.menuId = Convert.ToInt32(item);
                    carShopEntities.RoleMenus.Add(newItem);
                }


                var role = carShopEntities.Roles.Where(x => x.seq.ToString() == roleId).FirstOrDefault();
                if (role != null)
                {
                    if (status == "False")
                    {
                        role.status = false;
                    }
                    else
                    {
                        role.status = true;
                    }
                    //role.status = status;
                }
                carShopEntities.SaveChanges();
            }

            return RedirectToAction("Index", "Group");
        }

        public ActionResult Create()
        {
            if (!rightSrv.checkRight("Group", "Create", User.Identity.Name))
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        public ActionResult CreateInProgress(Group input)
        {
            try
            {
                Roles newItem = new Roles();
                newItem.RoleName = input.roleName;
                newItem.Description = input.description;
                newItem.status = false;// input.status;
                newItem.createTime = DateTime.Now;
                carShopEntities.Roles.Add(newItem);
                carShopEntities.SaveChanges();
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }

            return RedirectToAction("Index", "Group");
        }

        public ActionResult Delete(string seq)
        {
            if (!rightSrv.checkRight("Group", "Delete", User.Identity.Name))
            {
                return RedirectToAction("Index", "Home");
            }
            try
            {
                var role = carShopEntities.Roles.Where(x => x.seq.ToString() == seq).FirstOrDefault();
                carShopEntities.Roles.Remove(role);
                carShopEntities.SaveChanges();
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }

            return RedirectToAction("Index", "Group");
        }
    }
}