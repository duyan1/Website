using EWShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EWShop.Controllers
{
    public class GSISController : Controller
    {
        // GET: GSIS
        public JsonResult ServiceRequest(string month, string year)
        {
            ResultReturn result = ServiceRequestLogics.TransferGSIS(month, year);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}