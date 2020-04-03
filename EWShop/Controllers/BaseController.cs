using EWShop.Models;
using System.Collections.Generic;
using System.Web.Mvc;

namespace EWShop.Controllers
{
    public class BaseController : Controller
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            if (User.Identity.Name != null)
            {
                Session.Add(CommonConstants.MAIN_MENUS_INFO, UserLogic.GetListMainMenusForUser(User.Identity.Name) as List<Menus>);
                Session.Add(CommonConstants.CHILD_MENUS_INFO, UserLogic.GetListChildMenusForUser(User.Identity.Name) as List<Menus>);
                if (Session[CommonConstants.USER_INFO] == null)
                {
                    Session.Add(CommonConstants.USER_INFO, UserLogic.GetUserInfo(User.Identity.Name) as User);
                }
            }
            if (User.IsInRole("callcenter"))
            {
                Redirect("/CallCenter");
            }
            if (User.IsInRole("sales"))
            {
                Redirect("/Sales/MWG/PurchaseChangeOrders");
            }
        }
    }
}