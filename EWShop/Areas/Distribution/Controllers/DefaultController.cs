using EWShop.Areas.Distribution.Models;
using EWShop.Models;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EWShop.Areas.Distribution.Controllers
{
    public class DefaultController : DistributorController
    {
        public ActionResult SellThrough()
        {
            ViewBag.MainMenu = CommonConstants.MENU_MAIN_SELL;
            ViewBag.ChildMenu = CommonConstants.MENU_MAIN_SELL_THROUGH;
            ViewData["data"] = FileLogics.GetListData(DateTime.Now.AddDays(-7).ToString("dd/MM/yyyy"), DateTime.Now.ToString("dd/MM/yyyy"), User.Identity.Name.ToLower());
            return View();
        }
        [HttpPost]
        public ActionResult SellThrough(string frmDate, string toDate)
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
                    }else if (".xls".Equals(ext) || ".xlsx".Equals(ext))
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
        public void DownloadExcel(string frmDate, string toDate)
        {
            List<SellThrough> list = FileLogics.GetListData(frmDate, toDate, User.Identity.Name.ToLower());

            ExcelPackage Ep = new ExcelPackage();
            ExcelWorksheet Sheet = Ep.Workbook.Worksheets.Add("Report");
            Sheet.Cells["A1"].Value = "BARCODE";
            Sheet.Cells["B1"].Value = "MODELTYPE";
            Sheet.Cells["C1"].Value = "PLANT";
            Sheet.Cells["D1"].Value = "SELLDATE";
            Sheet.Cells["E1"].Value = "SHOPNAME";
            Sheet.Cells["F1"].Value = "TRANSUSER";
            Sheet.Cells["G1"].Value = "TRANSDATE";
            int row = 2;
            foreach (var item in list)
            {
                Sheet.Cells[string.Format("A{0}", row)].Value = item.barcode;
                Sheet.Cells[string.Format("B{0}", row)].Value = item.model;
                Sheet.Cells[string.Format("C{0}", row)].Value = item.plant;
                Sheet.Cells[string.Format("D{0}", row)].Value = item.sellDate;
                Sheet.Cells[string.Format("E{0}", row)].Value = item.shopName;
                Sheet.Cells[string.Format("F{0}", row)].Value = item.transuser;
                Sheet.Cells[string.Format("G{0}", row)].Value = item.transdate;
                row++;
            }

            Sheet.Cells["A:AZ"].AutoFitColumns();
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment: filename=" + "EWReport.xlsx");
            Response.BinaryWrite(Ep.GetAsByteArray());
            Response.End();
        }
    }
}