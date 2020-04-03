using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EWShop.Areas.CallCenter.Controllers
{
    public class CallCenterHomeController : CallCenterController
    {
        // GET: CallCenter/Home
        public ActionResult Index()
        {
            return View();
        }
    }
}