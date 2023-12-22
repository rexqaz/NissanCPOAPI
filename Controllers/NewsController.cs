using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using WebApplication.Models;
using WebApplication.Services;

namespace WebApplication.Controllers
{
    public class NewsController : Controller
    {
        CarShopEntities carShopEntities = new CarShopEntities();
        BrandService srv = new BrandService();
        RightService rightSrv = new RightService();
        FileService fileService = new FileService();
        private const string folderPath = "News";
        private const string _exportPath = "../XingUpdateFile/TemporaryFile/News/";

        // GET: News
        public ActionResult Index()
        {
            if (!rightSrv.checkRight("News", "Index", User.Identity.Name))
            {
                return RedirectToAction("Index", "Home");
            }

            if (!rightSrv.checkRight("News", "Update", User.Identity.Name))
            {
                TempData["UpdateRight"] = "False";
            }
            else
            {
                TempData["UpdateRight"] = "True";
            }

            if (!rightSrv.checkRight("News", "Create", User.Identity.Name))
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
        public JsonResult GetNewsData(FormCollection post)
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
            List<News> news = carShopEntities.News.Where(x => x.brand == brand).OrderByDescending(x => x.createTime).ToList();

            if (sortColumn == "建立日期")
            {
                if (sortColumnDirection == "asc") news = news.OrderBy(x => x.createTime).ToList();
                else news = news.OrderByDescending(x => x.createTime).ToList();
            }
            else if (sortColumn == "狀態")
            {
                if (sortColumnDirection == "asc") news = news.OrderBy(x => x.status).ToList();
                else news = news.OrderByDescending(x => x.status).ToList();
            }
            else if (sortColumn == "編號")
            {
                if (sortColumnDirection == "asc") news = news.OrderBy(x => x.seq).ToList();
                else news = news.OrderByDescending(x => x.seq).ToList();
            }
            else if (sortColumn == "時間排程")
            {
                if (sortColumnDirection == "asc") news = news.OrderBy(x => x.publishRange).ToList();
                else news = news.OrderByDescending(x => x.publishRange).ToList();
            }
            else if (sortColumn == "標題摘錄")
            {
                if (sortColumnDirection == "asc") news = news.OrderBy(x => x.title).ToList();
                else news = news.OrderByDescending(x => x.title).ToList();
            }

            if (!string.IsNullOrEmpty(keyword))
            {
                var shop1 = news.Where(x => x.createTime != null && x.createTime.ToString().Contains(keyword));
                var shop2 = news.Where(x => x.seq != null && x.seq.ToString().Contains(keyword));
                var shop3 = news.Where(x => x.publishRange != null && x.publishRange.Contains(keyword));
                var shop4 = news.Where(x => x.title != null && x.title.Contains(keyword));
                var shop5 = news.Where(x => x.status != null && x.status.ToString().Contains(keyword));
                news = shop1.Concat(shop2).Concat(shop3).Concat(shop4).Concat(shop5).ToList();
            }


            var data = news.Skip(skip).Take(pageSize).ToList();
            var jsonData = new { draw = draw, recordsFiltered = news.Count, recordsTotal = news.Count, data = data };
            //return Json(jsonData, JsonRequestBehavior.AllowGet);
            return new JsonResult()
            {
                Data = jsonData,
                MaxJsonLength = int.MaxValue,/*重點在這行*/
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public ActionResult Create()
        {
            if (!rightSrv.checkRight("News", "Create", User.Identity.Name))
            {
                return RedirectToAction("Index", "Home");
            }

            TempData["title"] = "";
            TempData["bodyContent"] = "";
            TempData["picturePath"] = "";
            TempData["createTime"] = "";
            TempData["status"] = "";
            TempData["publishRange"] = "";
            TempData["brand"] = "";
            TempData["showBody"] = "";       
            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult CreateInProgress(NewsViewModel input,  HttpPostedFileBase picture)
        {
            try
            {                
                TempData["title"] = input.title;
                TempData["bodyContent"] = input.bodyContent;
                TempData["showBody"] = input.showBody;
                TempData["publishRange"] = input.publishRange;
                TempData["status"] = input.status;

                var msg = CheckData(input, picture, false);
                if (string.IsNullOrWhiteSpace(msg) == false)
                {
                    TempData["MemberResult"] = msg;
                    return RedirectToAction("Create", "News");
                }
                else
                {
                    News newItem = new News();
                    newItem.title = input.title;
                    newItem.bodyContent = input.bodyContent;
                    newItem.showBody = input.showBody;
                    newItem.status = input.status;
                    newItem.publishRange = input.publishRange;
                    newItem.picturePath = SaveAsLocal(picture);
                    newItem.createTime = DateTime.Now;
                    newItem.brand = srv.getBrand(User.Identity.Name);
                    carShopEntities.News.Add(newItem);
                    carShopEntities.SaveChanges();

                    TempData["MemberResult"] = "OKCreate";
                    return RedirectToAction("Index", "News");
                }
            }
            catch (Exception ex)
            {
                TempData["MemberResult"] = ex.ToString();
                return RedirectToAction("Create", "News");
            }
        }

        public ActionResult Delete(string seq)
        {
            if (!rightSrv.checkRight("News", "Delete", User.Identity.Name))
            {
                return RedirectToAction("Index", "Home");
            }
            try
            {                
                var news = carShopEntities.News.Where(x => x.seq.ToString() == seq).FirstOrDefault();
                carShopEntities.News.Remove(news);
                carShopEntities.SaveChanges();
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }

            TempData["MemberResult"] = "OKDelete";
            return RedirectToAction("Index", "News");
        }

        public ActionResult Update(string seq)
        {
            if (!rightSrv.checkRight("News", "Update", User.Identity.Name))
            {
                return RedirectToAction("Index", "Home");
            }

            if (!rightSrv.checkRight("News", "UpdateInProgress", User.Identity.Name))
            {
                TempData["UpdateRight"] = "False";
            }
            else
            {
                TempData["UpdateRight"] = "True";
            }

            if (!rightSrv.checkRight("News", "Delete", User.Identity.Name))
            {
                TempData["DeleteRight"] = "False";
            }
            else
            {
                TempData["DeleteRight"] = "True";
            }
            var news = carShopEntities.News.Where(x => x.seq.ToString() == seq).FirstOrDefault();
            NewsViewModel model = new NewsViewModel();
            if (news != null)
            {
                model.id = news.seq.ToString();
                model.title = news.title;
                model.bodyContent = news.bodyContent;
                model.showBody = news.showBody;
                model.publishRange = news.publishRange;
                model.status = news.status;
                model.picture = fileService.GetRealUrl(news.picturePath);
            }
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult UpdateInProgress(NewsViewModel input, HttpPostedFileBase picture)
        {
            if (!rightSrv.checkRight("News", "UpdateInProgress", User.Identity.Name))
            {
                return RedirectToAction("Index", "Home");
            }
            try
            {                
                var msg = CheckData(input, picture, true);
                if (string.IsNullOrWhiteSpace(msg) == false)
                {
                    TempData["MemberResult"] = msg;
                    return RedirectToAction("Update", "News", new { seq = input.id });
                }


                string file = string.Empty;
                News news = carShopEntities.News.Where(x => x.seq.ToString() == input.id).FirstOrDefault();
                if (news != null)
                {
                    news.title = input.title;
                    news.bodyContent = input.bodyContent;
                    news.showBody = input.showBody;

                    if (picture != null && picture.ContentLength > 0)
                    {                       
                        news.picturePath = SaveAsLocal(picture);
                    }
                    news.createTime = DateTime.Now;
                    news.publishRange = input.publishRange;
                    news.status = input.status;
                   
                    carShopEntities.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                TempData["MemberResult"] = ex.ToString();
                return RedirectToAction("Update", "News", new { seq = input.id });
            }

            TempData["MemberResult"] = "OKUpdate";
            return RedirectToAction("Index", "News");
        }

        public ActionResult Preview(string bodyContent)
        {
            NewsViewModel model = new NewsViewModel();
            model.bodyContent = bodyContent;

            return View(model);
        }

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

        public ActionResult ExportCSV(FormCollection post)
        {
            if (!rightSrv.checkRight("Account", "Update", User.Identity.Name))
            {
                return Content("NO");
            }
            string keyword = post["Keyword"];
            string brand = srv.getBrand(User.Identity.Name);
            List<News> news = carShopEntities.News.Where(x => x.brand == brand).ToList();            

            if (!string.IsNullOrEmpty(keyword))
            {
                var shop1 = news.Where(x => x.createTime != null && x.createTime.ToString().Contains(keyword));
                var shop2 = news.Where(x => x.seq != null && x.seq.ToString().Contains(keyword));
                var shop3 = news.Where(x => x.publishRange != null && x.publishRange.Contains(keyword));
                var shop4 = news.Where(x => x.title != null && x.title.Contains(keyword));
                var shop5 = news.Where(x => x.status != null && x.status.ToString().Contains(keyword));
                news = shop1.Concat(shop2).Concat(shop3).Concat(shop4).Concat(shop5).ToList();
            }
            
            //before your loop
            var csv = new StringBuilder();
            csv.AppendLine("狀態,編號,時間排程,標題摘錄,建立日期");
            foreach (var item in news)
            {
                //in your loop
                var newLine = string.Format("{0},{1},{2},{3},{4}", item.status, item.seq, item.publishRange, item.title, item.createTime);
                csv.AppendLine(newLine);
            }

            //after your loop
            Session["genFileName"] = Guid.NewGuid().ToString() + ".csv";
            var path = Path.Combine(Server.MapPath(_exportPath), Session["genFileName"].ToString());
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
                Response.AddHeader("Content-Disposition", "attachment; filename=" + "最新消息列表" + DateTime.Today.ToString("yyyyMMddHHmmss") + ".csv");

                Response.BinaryWrite(memStream.ToArray());
                Response.Flush();
                Response.Close();
                Response.End();
            }

            System.IO.File.Delete(Path.Combine(Server.MapPath(_exportPath), Session["genFileName"].ToString()));

            return RedirectToAction("Index", "News");
        }



        private string CheckData(NewsViewModel input, HttpPostedFileBase picture, bool isUpdate)
        {
            var memberResultMessage = "";
            if (picture != null && picture.ContentLength > 0)
            {
                using (var image = Image.FromStream(picture.InputStream))
                {
                    int width = image.Width;
                    int height = image.Height;
                    if (width > 390 || height > 190)
                    {
                        memberResultMessage += "縮圖圖片大小需為390*190<br>";
                    }
                }
            }
            else
            {
                if (isUpdate)
                {
                    //編輯情況底下要檢查是不是本來就有圖檔
                    //如果沒有，就必須上傳
                    //如果有，那就覆蓋圖檔
                    var news = carShopEntities.News.Where(x => x.seq.ToString() == input.id).FirstOrDefault();
                    if (string.IsNullOrEmpty(news.picturePath))
                    {
                        memberResultMessage += "請上傳檔案<br>";
                    }
                }
                else
                {
                    memberResultMessage += "請上傳檔案<br>";
                }                
            }

            if (string.IsNullOrEmpty(input.title))
            {
                memberResultMessage += "請輸入[標題]<br>";
            }

            if (string.IsNullOrEmpty(input.publishRange))
            {
                memberResultMessage += "請輸入[時間排程]<br>";
            }
            else
            {
                var arrayRange = input.publishRange.Split('~');
                if (arrayRange.Length == 2)
                {
                    var sDate = arrayRange[0].Trim();
                    var eDate = arrayRange[1].Trim();
                    if (DateTime.TryParse(sDate, out _) == false || DateTime.TryParse(eDate, out _) == false)
                    {
                        memberResultMessage += "請輸入[時間排程]，起始或結束時間有誤<br>";
                    }
                }
                else
                {
                    memberResultMessage += "請輸入[時間排程]，包含起始跟結束時間<br>";
                }
            }

            if (string.IsNullOrEmpty(input.bodyContent))
            {
                memberResultMessage += "請輸入[貼文內容]<br>";
            }
            else
            {
                //如果在編輯器模式只有純tag沒有任何文字那就過濾掉
                var body = System.Text.RegularExpressions.Regex.Replace(input.bodyContent, "<.*?>", "").Replace("&nbsp;", "");
                if (string.IsNullOrWhiteSpace(body))
                {
                    memberResultMessage += "請輸入[貼文內容*]<br>";
                }

            }

            if (string.IsNullOrEmpty(input.showBody))
            {
                memberResultMessage += "請輸入[貼文內容摘要]<br>";
            }

            if (string.IsNullOrEmpty(input.status))
            {
                memberResultMessage += "請輸入狀態<br>";
            }
            else
            {
                switch (input.status)
                {
                    case "啟用":
                    case "草稿":
                    case "停用":
                        break;
                    default:
                        memberResultMessage += $"[貼文狀態]錯誤，status={input.status}<br>";
                        break;
                }
            }

            return memberResultMessage;
        }


        private string SaveAsLocal(HttpPostedFileBase picture)
        {
            return fileService.SaveAsBlob(picture, folderPath, "A");
        }
    }
}