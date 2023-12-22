using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Mvc;
using WebApplication.Models;
using WebApplication.Services;

namespace WebApplication.Controllers
{
    public class BannerController : Controller
    {
        CarShopEntities carShopEntities = new CarShopEntities();
        BrandService srv = new BrandService();
        RightService rightSrv = new RightService();
        FileService fileService = new FileService();
        private const string folderPath = "Banner";

        // GET: Banner
        public ActionResult Index()
        {
            if (!rightSrv.checkRight("Banner", "Index", User.Identity.Name))
            {
                return RedirectToAction("Index", "Home");
            }

            if (!rightSrv.checkRight("Banner", "Update", User.Identity.Name))
            {
                TempData["UpdateRight"] = "False";
            }
            else
            {
                TempData["UpdateRight"] = "True";
            }

            if (!rightSrv.checkRight("Banner", "Create", User.Identity.Name))
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
        public JsonResult GetBannerData(FormCollection post)
        {
            try
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

                string brand = srv.getBrand(User.Identity.Name);
                List<Banners> banners = carShopEntities.Banners.Where(x => x.brand == brand).OrderByDescending(x => x.createTime).ToList();
                if (sortColumn == "建立日期")
                {
                    if (sortColumnDirection == "asc") banners = banners.OrderBy(x => x.createTime).ToList();
                    else banners = banners.OrderByDescending(x => x.createTime).ToList();
                }
                else if (sortColumn == "狀態")
                {
                    if (sortColumnDirection == "asc") banners = banners.OrderBy(x => x.status).ToList();
                    else banners = banners.OrderByDescending(x => x.status).ToList();
                }
                else if (sortColumn == "編號")
                {
                    if (sortColumnDirection == "asc") banners = banners.OrderBy(x => x.seq).ToList();
                    else banners = banners.OrderByDescending(x => x.seq).ToList();
                }
                else if (sortColumn == "時間排程")
                {
                    if (sortColumnDirection == "asc") banners = banners.OrderBy(x => x.publishRange).ToList();
                    else banners = banners.OrderByDescending(x => x.publishRange).ToList();
                }
                else if (sortColumn == "標題摘錄")
                {
                    if (sortColumnDirection == "asc") banners = banners.OrderBy(x => x.title).ToList();
                    else banners = banners.OrderByDescending(x => x.title).ToList();
                }
                else if (sortColumn == "點擊數")
                {
                    if (sortColumnDirection == "asc") banners = banners.OrderBy(x => x.hitCount).ToList();
                    else banners = banners.OrderByDescending(x => x.hitCount).ToList();
                }
                else if (sortColumn == "曝光數")
                {
                    if (sortColumnDirection == "asc") banners = banners.OrderBy(x => x.viewCount).ToList();
                    else banners = banners.OrderByDescending(x => x.viewCount).ToList();
                }
                else if (sortColumn == "排序")
                {
                    if (sortColumnDirection == "asc") banners = banners.OrderBy(x => x.banner_sort).ToList();
                    else banners = banners.OrderByDescending(x => x.banner_sort).ToList();
                }


                if (!string.IsNullOrEmpty(keyword))
                {
                    var shop1 = banners.Where(x => x.createTime != null && x.createTime.ToString().Contains(keyword));
                    var shop2 = banners.Where(x => x.seq != null && x.seq.ToString().Contains(keyword));
                    var shop3 = banners.Where(x => x.publishRange != null && x.publishRange.Contains(keyword));
                    var shop4 = banners.Where(x => x.title != null && x.title.Contains(keyword));
                    var shop5 = banners.Where(x => x.status != null && x.status.ToString().Contains(keyword));
                    var shop6 = banners.Where(x => x.viewCount != null && x.viewCount.ToString().Contains(keyword));
                    var shop7 = banners.Where(x => x.hitCount != null && x.hitCount.ToString().Contains(keyword));
                    banners = shop1.Concat(shop2).Concat(shop3).Concat(shop4).Concat(shop5).Concat(shop6).Concat(shop7).ToList();
                }


                var data = banners.Skip(skip).Take(pageSize).ToList();
                var jsonData = new { draw = draw, recordsFiltered = banners.Count, recordsTotal = banners.Count, data = data };
                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                APCommonFun.Error("[GetBannerData]99：" + ex.ToString());
                throw;
            }
        }
        public ActionResult Create()
        {
            if (!rightSrv.checkRight("Banner", "Create", User.Identity.Name))
            {
                return RedirectToAction("Index", "Home");
            }

            string brand = srv.getBrand(User.Identity.Name);
            int bannerCurrentCount = carShopEntities.Banners.Where(x => x.brand == brand).Count();
            if (bannerCurrentCount == 5)
            {
                TempData["MemberResult"] = "Banner最多只能上架5個!!";
                return RedirectToAction("Index");
            }
            return View();
        }

        [HttpPost]
        public ActionResult CreateInProgress(BannerViewModel model,  HttpPostedFileBase picture, HttpPostedFileBase mobilePicture)
        {
            try
            {
                string contentType = string.Empty;
                var msg = CheckData(model, picture, mobilePicture, false);
                if (string.IsNullOrEmpty(msg) == false)
                {
                    TempData["MemberResult"] = msg;
                    return RedirectToAction("Create", "Banner");
                }

                if (picture != null && picture.ContentLength > 0)
                {
                    if (picture.FileName.Split('.')[1] == "pdf")
                    {
                        TempData["MemberResult"] = "請上傳圖片檔案格式";
                        return RedirectToAction("Create", "Banner");
                    }
                    contentType = picture.ContentType;
                }
                else
                {
                    TempData["MemberResult"] = "請上傳檔案";
                    return RedirectToAction("Create", "Banner");
                }

                if (mobilePicture != null && mobilePicture.ContentLength > 0)
                {
                    if (mobilePicture.FileName.Split('.')[1] == "pdf")
                    {
                        TempData["MemberResult"] = "請上傳圖片檔案格式";
                        return RedirectToAction("Create", "Banner");
                    }
                    contentType = mobilePicture.ContentType;
                }
                else
                {
                    TempData["MemberResult"] = "請上傳檔案";
                    return RedirectToAction("Create", "Banner");
                }

                if (ModelState.IsValid)
                {
                    Banners banner = new Banners();
                    banner.title = model.title;
                    banner.subTitle = model.subTitle;
                    banner.picture = SaveAsLocal(picture);
                    banner.mobilePicture = SaveAsLocal(mobilePicture);
                    banner.contentType = contentType;
                    banner.createTime = DateTime.Now;
                    switch (model.action)
                    {
                        case "儲存草稿":
                            banner.status = "草稿";
                            break;
                        case "送出":
                            banner.status = model.status;
                            break;
                        default:
                            break;
                    }

                    banner.banner_name = model.banner_name;
                    banner.banner_sort = model.banner_sort;
                    banner.publishRange = model.publishRange;
                    banner.url = model.url;
                    banner.brand = srv.getBrand(User.Identity.Name);

                    carShopEntities.Banners.Add(banner);
                    carShopEntities.SaveChanges();

                    TempData["MemberResult"] = "OKCreate";
                    return RedirectToAction("Index");
                }

                return View(model);
            }
            catch (Exception ex)
            {
                APCommonFun.Error("[BannerController.CreateInProgress()]99：" + ex.ToString());
                throw;
            }
        }

        public ActionResult Update(int seq)
        {
            if (!rightSrv.checkRight("Banner", "Update", User.Identity.Name))
            {
                return RedirectToAction("Index", "Home");
            }

            if (!rightSrv.checkRight("Banner", "UpdateInProgress", User.Identity.Name))
            {
                TempData["UpdateRight"] = "False";
            }
            else
            {
                TempData["UpdateRight"] = "True";
            }

            if (!rightSrv.checkRight("Banner", "Delete", User.Identity.Name))
            {
                TempData["DeleteRight"] = "False";
            }
            else
            {
                TempData["DeleteRight"] = "True";
            }

            BannerViewModel model = new BannerViewModel();
            var banner = carShopEntities.Banners.Where(x => x.seq == seq).First();

            if (banner != null)
            {
                model.seq = banner.seq;
                model.bodyContent = banner.bodyContent;
                model.title = banner.title;
                model.subTitle = banner.subTitle;
                model.publishRange = banner.publishRange;
                model.url = banner.url;                
                model.picture = fileService.GetRealUrl(banner.picture);               
                model.mobilePicture = fileService.GetRealUrl(banner.mobilePicture);
                model.status = banner.status;
                model.banner_name = banner.banner_name;
                model.banner_sort = banner.banner_sort == null ? 0 : banner.banner_sort.Value;
            }
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateInProgress(BannerViewModel model,  HttpPostedFileBase picture, HttpPostedFileBase mobilePicture)
        {
            if (!rightSrv.checkRight("Banner", "UpdateInProgress", User.Identity.Name))
            {
                return RedirectToAction("Index", "Home");
            }

            var msg = CheckData(model, picture, mobilePicture, true);
            if (string.IsNullOrEmpty(msg) == false)
            {
                TempData["MemberResult"] = msg;
                return RedirectToAction("Update", "Banner", new { seq = model.seq });
            }

            var file = string.Empty;
            var fileMobile = string.Empty;
            string contentType = string.Empty;


            if (picture != null && picture.ContentLength > 0)
            {
                if (picture.FileName.Split('.')[1] == "pdf")
                {
                    TempData["MemberResult"] = "請上傳圖片檔案格式";
                    return RedirectToAction("Update", "Banner");
                }
                file = SaveAsLocal(picture);
                contentType = picture.ContentType;
            }

            if (mobilePicture != null && mobilePicture.ContentLength > 0)
            {
                if (mobilePicture.FileName.Split('.')[1] == "pdf")
                {
                    TempData["MemberResult"] = "請上傳圖片檔案格式";
                    return RedirectToAction("Update", "Banner");
                }
                fileMobile = SaveAsLocal(mobilePicture);
                contentType = mobilePicture.ContentType;
            }

            if (ModelState.IsValid)
            {
                var banner = carShopEntities.Banners.Where(x => x.seq == model.seq).FirstOrDefault();
                if (banner != null)
                {
                    banner.publishRange = model.publishRange;
                    banner.title = model.title;
                    banner.url = model.url;
                    banner.banner_name = model.banner_name;
                    banner.banner_sort = model.banner_sort;
                    switch (model.action)
                    {
                        case "儲存草稿":
                            banner.status = "草稿";
                            break;
                        case "送出":
                            banner.status = model.status;
                            break;
                        default:
                            break;
                    }
                    if (picture != null && picture.ContentLength > 0)
                    { 
                        banner.picture = file;
                        banner.contentType = contentType;
                    }
                    if (mobilePicture != null && mobilePicture.ContentLength > 0)
                    {
                        banner.mobilePicture = fileMobile;
                    }
                }
                
                carShopEntities.SaveChanges();
                TempData["MemberResult"] = "OKUpdate";
                return RedirectToAction("Index");
            }

            return View(model);
        }

        public ActionResult Delete(string seq)
        {
            if (!rightSrv.checkRight("Banner", "Delete", User.Identity.Name))
            {
                return RedirectToAction("Index", "Home");
            }
            try
            {
                var banner = carShopEntities.Banners.Where(x => x.seq.ToString() == seq).FirstOrDefault();
                carShopEntities.Banners.Remove(banner);
                carShopEntities.SaveChanges();
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }

            TempData["MemberResult"] = "OKDelete";
            return RedirectToAction("Index", "Banner");
        }

        public ActionResult ExportCSV()
        {
            if (!rightSrv.checkRight("Banner", "ExportCSV", User.Identity.Name))
            {
                return RedirectToAction("Index", "Home");
            }
            string brand = srv.getBrand(User.Identity.Name);
            List<Banners> banners = carShopEntities.Banners.Where(x => x.brand == brand).ToList();

            string strFileName = "Banner列表.csv";

            //撈取 使用者資料 資料表 資訊
            IEnumerable<Banners> result = from p in banners
                                        select p;

            Response.AppendHeader("content-disposition", "attachment;filename=" + strFileName);
            Response.ContentType = "application/unkown";
            Response.ContentEncoding = Encoding.UTF8;

            GenerateCSV(result);

            return new EmptyResult();
        }

        private void GenerateCSV<T>(IEnumerable<T> list)
        {
            Type t = typeof(T);

            //將資料寫到前端
            StreamWriter sw = new StreamWriter(Response.OutputStream, Encoding.Default);

            object o = Activator.CreateInstance(t);

            PropertyInfo[] props = o.GetType().GetProperties();

            //將資料表的欄位名稱寫出
            foreach (PropertyInfo pi in props)
            {
                sw.Write(pi.Name.ToUpper() + ",");
            }
            sw.WriteLine();

            //將資料表的資料寫出
            foreach (T item in list)
            {
                foreach (PropertyInfo pi in props)
                {
                    string whatToWrite =
                     Convert.ToString(item.GetType().GetProperty(pi.Name).GetValue(item, null)).Replace(',', ' ') + ',';
                    sw.Write(whatToWrite);
                }
                sw.WriteLine();
            }

            sw.Close();
        }

        private string CheckData(BannerViewModel model, HttpPostedFileBase picture, HttpPostedFileBase mobilePicture, bool isUpdate)
        {
            var message = "";

            if (isUpdate)
            {
                //編輯情況底下要檢查是不是本來就有圖檔
                //如果沒有，就必須上傳
                //如果有，那就覆蓋圖檔
                var banner = carShopEntities.Banners.Where(x => x.seq == model.seq).FirstOrDefault();
                if (string.IsNullOrEmpty(banner.picture) && (picture == null || picture.ContentLength == 0))
                {
                    message += "請上傳[桌機圖片]<br>";
                }
                if (string.IsNullOrEmpty(banner.mobilePicture) && (mobilePicture == null || mobilePicture.ContentLength == 0))
                {
                    message += "請上傳[手機圖片]<br>";
                }
            }
            else
            {
                if (picture == null || picture.ContentLength == 0)
                {
                    message += "請上傳[桌機圖片]<br>";
                }
                if (mobilePicture == null || mobilePicture.ContentLength == 0)
                {
                    message += "請上傳[手機圖片]<br>";
                }
            }

            if (string.IsNullOrEmpty(model.url))
            {
                message += "請填寫[網址]<br>";
            }
            if (string.IsNullOrWhiteSpace(model.banner_name))
            {
                message += "請填寫[Banner名稱]<br>";
            }

            //switch (model.status)
            //{
            //    case "啟用":
            //    case "停用":
            //    case "草稿":
            //        break;
            //    default:
            //        message += "請選擇[貼文狀態]<br>";
            //        break;
            //}

            if (string.IsNullOrEmpty(model.publishRange))
            {
                message += "請填寫[時間排程]<br>";
            }
            else
            {
                var arrayRange = model.publishRange.Split('~');
                if (arrayRange.Length == 2)
                {
                    var sDate = arrayRange[0].Trim();
                    var eDate = arrayRange[1].Trim();
                    if (DateTime.TryParse(sDate, out _) == false || DateTime.TryParse(eDate, out _) == false)
                    {
                        message += "請輸入[時間排程]，起始或結束時間有誤<br>";
                    }
                }
                else
                {
                    message += "請輸入[時間排程]，包含起始跟結束時間<br>";
                }
            }

            return message;
        }

        private string SaveAsLocal(HttpPostedFileBase picture)
        {
            return fileService.SaveAsBlob(picture, folderPath, "A");
        }

        #region SUMMERNOTE上传图片

        [HttpPost]
        public ActionResult SummernoteUploadImage()
        {
            foreach (var fileKey in Request.Files.AllKeys)
            {
                var file = Request.Files[fileKey];
                try
                {
                    var fileName = Path.GetFileName(file?.FileName);
                    if (fileName != null)
                    {
                        var path = Server.MapPath("~/Uploads/SummernoteImages");
                        if (System.IO.File.Exists(path) == false)
                        {
                            Directory.CreateDirectory(path);
                        }

                        path = Path.Combine(Server.MapPath("~/Uploads/SummernoteImages"), fileName);
                        file.SaveAs(path);

                        Bitmap bmp = new Bitmap(path);

                        MemoryStream ms = new MemoryStream();
                        bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                        byte[] arr = new byte[ms.Length];
                        ms.Position = 0;
                        ms.Read(arr, 0, (int)ms.Length);
                        ms.Close();


                        return Json(new { Url = Url.Content("~/Uploads/SummernoteImages/" + fileName), base64 = Convert.ToBase64String(arr) });
                    }
                }
                catch (Exception ex)
                {
                    return Json(new { Message = $"Error in saving file ({ex.Message})" });
                }
            }
            return Json(new { Message = "File saved" });
        }

        #endregion
    }
}