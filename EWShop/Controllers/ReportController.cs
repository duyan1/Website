using EWShop.Models;
using EWShop.Utilities;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EWShop.Controllers
{
    [Authorize]
    public class ReportController : BaseController
    {
        // GET: Report
        public ActionResult Index()
        {
            ViewData["MONTH"] = Common.loadListMonth();
            SellOutReport model = SellOutLogic.GetReportSellOut(DateTime.Now.Month.ToString(), DateTime.Now.Year.ToString(), User.Identity.Name.ToLower());

            return View(model);
        }
        [HttpPost]
        public ActionResult Index(string month)
        {
            ViewData["MONTH"] = Common.loadListMonth();
            SellOutReport model = SellOutLogic.GetReportSellOut(month, DateTime.Now.Year.ToString(), User.Identity.Name.ToLower());
            return View(model);
        }
        [HttpGet]
        public JsonResult getData4Months()
        {
            return Json(SellOutLogic.GetReportSellOut4Month(User.Identity.Name.ToLower()), JsonRequestBehavior.AllowGet);
        }
        public ActionResult ViewDetail()
        {
            ViewData["MONTH"] = Common.loadListMonth();
            ViewData[CommonConstants.PLANTS_INFO] = Common.getListPlants();
            List<ProductSellOutReport> model = SellOutLogic.GetSellOutDetail(DateTime.Now.Month.ToString(), DateTime.Now.Year.ToString(), "", User.Identity.Name.ToLower());
            return View(model);
        }
        [HttpPost]
        public ActionResult ViewDetail(string month, string plant)
        {
            ViewData["MONTH"] = Common.loadListMonth();
            ViewData[CommonConstants.PLANTS_INFO] = Common.getListPlants();
            List<ProductSellOutReport> model = SellOutLogic.GetSellOutDetail(month, DateTime.Now.Year.ToString(), plant, User.Identity.Name.ToLower());
            return PartialView("~/Views/Layout/_PartialListEWRegistersForPSI.cshtml", model);
        }
        public void DownloadExcel(string month, string plant)
        {
            List<ProductSellOutReport> output = SellOutLogic.GetSellOutDetail(month, DateTime.Now.Year.ToString(), plant, User.Identity.Name.ToLower());

            ExcelPackage Ep = new ExcelPackage();
            ExcelWorksheet Sheet = Ep.Workbook.Worksheets.Add("Report");
            Sheet.Cells["A1"].Value = "EWCARDNO";
            Sheet.Cells["B1"].Value = "POD";
            Sheet.Cells["C1"].Value = "BARCODE";
            Sheet.Cells["D1"].Value = "MODEL";
            Sheet.Cells["E1"].Value = "CATEGORY";
            Sheet.Cells["F1"].Value = "CUSTOMER NAME";
            Sheet.Cells["G1"].Value = "CUSTOMER PHONE";
            Sheet.Cells["H1"].Value = "CUSTOMER ADDRESS";
            Sheet.Cells["I1"].Value = "CHANNEL";
            Sheet.Cells["J1"].Value = "SHOPCODE";
            Sheet.Cells["K1"].Value = "SHOPNAME";
            Sheet.Cells["L1"].Value = "REGION";
            Sheet.Cells["M1"].Value = "REG.DATE";
            int row = 2;
            foreach (var item in output)
            {

                Sheet.Cells[string.Format("A{0}", row)].Value = item.eWCardNo;
                Sheet.Cells[string.Format("B{0}", row)].Value = item.POD;
                Sheet.Cells[string.Format("C{0}", row)].Value = item.barcode;
                Sheet.Cells[string.Format("D{0}", row)].Value = item.model;
                Sheet.Cells[string.Format("E{0}", row)].Value = item.plant;
                Sheet.Cells[string.Format("F{0}", row)].Value = item.cusName;
                Sheet.Cells[string.Format("G{0}", row)].Value = item.cusPhone;
                Sheet.Cells[string.Format("H{0}", row)].Value = item.cusAddress;
                Sheet.Cells[string.Format("I{0}", row)].Value = item.channel;
                Sheet.Cells[string.Format("J{0}", row)].Value = item.shopCode;
                Sheet.Cells[string.Format("K{0}", row)].Value = item.shopName;
                Sheet.Cells[string.Format("L{0}", row)].Value = item.region;
                Sheet.Cells[string.Format("M{0}", row)].Value = item.regDate;
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