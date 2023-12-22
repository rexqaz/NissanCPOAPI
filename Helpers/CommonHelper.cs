//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Web;

//namespace WebApplication.Helpers
//{
//    public class CommonHelper
//    {
//        public static string GetImgUrl(HttpRequest request, string filePath)
//        {
//            string urlBase = string.Format("{0}://{1}{2}", request.Url.Scheme, request.Url.Authority, (new System.Web.Mvc.UrlHelper(request.RequestContext)).Content("~"));
//            var root = Path.Combine(urlBase, "XingUpdateFile");

//            return string.IsNullOrEmpty(filePath) ? "" : root + "/" + (filePath.IndexOf("C:\\inetpub\\wwwroot\\CPO\\XingUpdateFile\\") > -1 ? filePath.Replace("C:\\inetpub\\wwwroot\\CPO\\XingUpdateFile\\", "") : filePath);
//        }
//    }
//}