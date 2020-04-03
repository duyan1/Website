using System.Web.Mvc;

namespace EWShop.Areas.Admin.Controllers
{
    [Authorize(Roles = "administrator")]
    public class DefaultController : AdminController
    {
        // GET: Admin/Home
        public ActionResult Index()
        {
            return View();
        }
    }
}