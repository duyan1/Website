using EWShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace EWShop.Controllers
{
    public class AccountController : BaseController
    {
        // GET: Account
        public ActionResult Login(string ReturnUrl)
        {
            UserLogin user = new UserLogin();
            ViewBag.ReturnUrl = ReturnUrl;
            return View(user);
        }
        [HttpPost]
        public ActionResult Login(UserLogin user, string ReturnUrl)
        {
            try
            {
                if (ReturnUrl == null)
                    ReturnUrl = "/";
                if (user != null)
                {
                    ResultReturn result = UserLogic.ValidateUser(user.userName, user.userPass);
                    if (result.code == "1")
                    {
                        FormsAuthentication.SetAuthCookie(user.userName, false);
                        User us = UserLogic.GetUserInfo(user.userName);
                        Session.Add(CommonConstants.USER_INFO, us);
                        if ("/".Equals(ReturnUrl))
                        {
                            if ("administrator".Equals(us.roleName))
                            {
                                ReturnUrl = "/Admin";
                            }
                            else if("distributor".Equals(us.roleName))
                            {
                                ReturnUrl = "/Distribution";
                            }
                            else if ("callcenter".Equals(us.roleName))
                            {
                                ReturnUrl = "/CallCenter";
                            }
                            else if ("sales".Equals(us.roleName))
                            {
                                ReturnUrl = "/Sales/MWG/PurchaseChangeOrders";
                            }
                        }
                        return Redirect(ReturnUrl);
                    }
                    else
                    {
                        ModelState.AddModelError("", "Tên đăng nhập hoặc mật khẩu không đúng");
                    }
                }
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
            
            return View(user);
        }
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }
        [Authorize]
        public ActionResult UserProfile()
        {
            return View(UserLogic.GetUserInfo(User.Identity.Name) as User);
        }
        [Authorize]
        public JsonResult changePass(UserChangePasss us)
        {
            return Json(UserLogic.changePass(us.oldPass, us.newPass, us.confirmPass, User.Identity.Name), JsonRequestBehavior.AllowGet);
        }
    }
}