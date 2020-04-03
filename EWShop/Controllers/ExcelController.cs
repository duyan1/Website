using EWShop.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EWShop.Controllers
{
    public class ExcelController : Controller
    {
        private const string SessionExcelData = "SessionExcelData";

        public ActionResult Index()
        {
            ViewData["JANUARY"] = (new DateTime(2019,01,25)).ToString("dd-MMM-yyyy");
            ViewData["FEBRUARY"] = (new DateTime(2019, 02, 25)).ToString("dd-MMM-yyyy");
            ViewData["MARCH"]= (new DateTime(2019, 03, 25)).ToString("dd-MMM-yyyy");
            
            ViewData["APRIL"] = (new DateTime(2019, 04, 25)).ToString("dd-MMM-yyyy");
            ViewData["MAY"] = (new DateTime(2019, 05, 25)).ToString("dd-MMM-yyyy");
            ViewData["JUNE"] = (new DateTime(2019, 06, 25)).ToString("dd-MMM-yyyy");
            
            ViewData["JULY"] = (new DateTime(2019, 07, 25)).ToString("dd-MMM-yyyy");
            ViewData["AUGUST"] = (new DateTime(2019, 08, 25)).ToString("dd-MMM-yyyy");
            ViewData["SEPTEMBER"] = (new DateTime(2019, 09, 25)).ToString("dd-MMM-yyyy");
            
            ViewData["OCTOBER"] = (new DateTime(2019, 10, 25)).ToString("dd-MMM-yyyy");
            ViewData["NOVEMBER"] = (new DateTime(2019, 11, 25)).ToString("dd-MMM-yyyy");
            ViewData["DECEMBER"] = (new DateTime(2019, 12, 25)).ToString("dd-MMM-yyyy");

            return View();
        }
    }
}