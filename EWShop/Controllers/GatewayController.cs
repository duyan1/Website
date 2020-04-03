using EWShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EWShop.Controllers
{
    public class GatewayController : Controller
    {
        public JsonResult searchModel(string term)
        {
            return Json(SellOutLogic.listModelFilterByPlantAndModel(term), JsonRequestBehavior.AllowGet);
        }
        public JsonResult searchShop(string term)
        {
            return Json(SellOutLogic.listShopFilterByKeyword(term), JsonRequestBehavior.AllowGet);
        }
    }
}