using EWShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EWShop.Areas.Distribution.Controllers
{
    [Authorize(Roles = "distributor")]
    public class DistributorController : Controller
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            if (User.Identity.Name != null)
            {
                Session.Add(CommonConstants.USER_INFO, UserLogic.GetUserInfo(User.Identity.Name) as User);
                Session.Add(CommonConstants.MAIN_MENUS_INFO, UserLogic.GetListMainMenusForUser(User.Identity.Name) as List<Menus>);
                Session.Add(CommonConstants.CHILD_MENUS_INFO, UserLogic.GetListChildMenusForUser(User.Identity.Name) as List<Menus>);
            }
        }
    }
}