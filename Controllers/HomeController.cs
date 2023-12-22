using System.Web.Mvc;
using WebApplication.Services;

namespace WebApplication.Controllers
{
    public class HomeController : Controller
    {
        private RightService rightSrv = new RightService();
        public ActionResult Index()
        {

            if (!rightSrv.checkRight("CaseMgt", "Index", User.Identity.Name))
            {
                return RedirectToAction("Index2", "Home");
            }
            else
            {
                return RedirectToAction("Index", "CaseMgt");
            }            
        }

        public ActionResult Index2()
        {

            return View();
        }
    }
}