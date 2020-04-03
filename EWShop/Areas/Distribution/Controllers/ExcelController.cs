using EWShop.Areas.Distribution.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EWShop.Areas.Distribution.Controllers
{
    public class ExcelController : Controller
    {
        public ActionResult SellThroughRoutingHuongThuy()
        {
            return View(StoreLogics.getListShopsChild("800000"));
        }
        public ActionResult SellThroughRouting266()
        {
            return View(StoreLogics.getListShopsChild("600000"));
        }
    }
}