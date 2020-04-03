using EWShop.Areas.Distribution.Models;
using EWShop.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Mvc;

namespace EWShop.Controllers
{
    [Authorize]
    public class SellThroughController : BaseController
    {
        // GET: SellThrough
        public ActionResult Index()
        {
            ViewBag.MainMenu = CommonConstants.MENU_MAIN_SELL;
            ViewBag.ChildMenu = CommonConstants.MENU_MAIN_SELL_THROUGH;
            ViewData["data"] = FileLogics.GetListData(DateTime.Now.AddDays(-7).ToString("dd/MM/yyyy"), DateTime.Now.ToString("dd/MM/yyyy"), User.Identity.Name.ToLower());
            return View();
        }
        [HttpPost]
        public ActionResult Index(string frmDate, string toDate)
        {
            ViewBag.MainMenu = CommonConstants.MENU_MAIN_SELL;
            ViewBag.ChildMenu = CommonConstants.MENU_MAIN_SELL_THROUGH;
            List<SellThrough> list = FileLogics.GetListData(frmDate, toDate, User.Identity.Name.ToLower());
            return PartialView("~/Views/Layout/_PartialViewSellThrough.cshtml", list);
        }
        [HttpPost]
        public JsonResult UploadFileText()
        {
            ResultReturn result = new ResultReturn();
            HttpFileCollectionBase files = Request.Files;
            for (int i = 0; i < files.Count; i++)
            {

                HttpPostedFileBase file = files[i];
                if ((file != null) && (file.ContentLength > 0) && !string.IsNullOrEmpty(file.FileName))
                {
                    string ext = Path.GetExtension(file.FileName).ToLower();
                    if (".txt".Equals(ext))
                    {
                        result = FileLogics.ReadFileText(file, User.Identity.Name);
                    }
                    else if (".xls".Equals(ext) || ".xlsx".Equals(ext))
                    {
                        result = FileLogics.ReadFileExcel(file, User.Identity.Name);
                    }
                    else
                    {
                        result.code = "0";
                        result.msg = "Vui lòng tải tập tin theo định dạng";
                    }
                }
                else
                {
                    result.code = "0";
                    result.msg = "File không tồn tại";
                }
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}