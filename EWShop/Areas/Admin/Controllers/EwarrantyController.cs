using EWShop.Areas.Admin.Logics;
using EWShop.Models;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EWShop.Areas.Admin.Controllers
{
    [Authorize(Roles = "administrator")]
    public class EwarrantyController : AdminController
    {
        public ActionResult ViewHistory()
        {
            MasterDataLogics logics = new MasterDataLogics();
            ViewData[CommonConstants.SHOP_INFO] = logics.getListShops();
            ViewData[CommonConstants.MATERIAL_INFO] = logics.getMaterials();
            ViewBag.MainMenu = CommonConstants.MENU_MAIN_E_WARRANTY;
            ViewBag.ChildMenu = CommonConstants.MENU_VIEW_HISTORY;
            SellOutLogics sol = new SellOutLogics();
            List<ProductSellOutReport> output = sol.listEWHistory("","", DateTime.Now.AddDays(-7).ToString("dd/MM/yyyy"), DateTime.Now.ToString("dd/MM/yyyy"));
            return View(output);
        }
        public ActionResult CheckBarcode()
        {
            ViewBag.MainMenu = CommonConstants.MENU_MAIN_E_WARRANTY;
            ViewBag.ChildMenu = CommonConstants.MENU_CHECK_BARCODE;
            return View();
        }
        [HttpPost]
        public ActionResult ViewHistory(ItemFilterReport data)
        {
            SellOutLogics logics = new SellOutLogics();
            List<ProductSellOutReport> output = logics.listEWHistory(data.shop, data.material, data.frmDate, data.toDate);
            return PartialView("~/Views/Layout/_PartialListEWRegisters.cshtml", output);
        }
        public JsonResult getInfoBarcode(string barcode, string model, string matid)
        {
            if (model == null) model = "";
            if (matid == null) matid = "0";
            SellOutLogics logics = new SellOutLogics();
            return Json(logics.getInfoBarcode(barcode, model, matid), JsonRequestBehavior.AllowGet);
        }
        public void DownloadExcel(string shop, string material, string frmDate, string toDate)
        {
            SellOutLogics logics = new SellOutLogics();
            List<ProductSellOutReport> output = logics.listEWHistory(shop, material, frmDate, toDate);

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
            Sheet.Cells["L1"].Value = "SHOPADDRESS";
            Sheet.Cells["M1"].Value = "REGION";
            Sheet.Cells["N1"].Value = "AREA";
            Sheet.Cells["O1"].Value = "GROUPCODE";
            Sheet.Cells["P1"].Value = "GROUPNAME";
            Sheet.Cells["Q1"].Value = "REG.DATE";
            Sheet.Cells["R1"].Value = "EXP.DATE";
            Sheet.Cells["S1"].Value = "ISSUEDBY";
            Sheet.Cells["T1"].Value = "BRANCH";
            Sheet.Cells["U1"].Value = "SHOP PROVINCE";
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
                Sheet.Cells[string.Format("L{0}", row)].Value = item.shopAddress;
                Sheet.Cells[string.Format("M{0}", row)].Value = item.region;
                Sheet.Cells[string.Format("N{0}", row)].Value = item.area;
                Sheet.Cells[string.Format("O{0}", row)].Value = item.groupCode;
                Sheet.Cells[string.Format("P{0}", row)].Value = item.groupName;
                Sheet.Cells[string.Format("Q{0}", row)].Value = item.regDate;
                Sheet.Cells[string.Format("R{0}", row)].Value = item.expDate;
                Sheet.Cells[string.Format("S{0}", row)].Value = item.status;
                Sheet.Cells[string.Format("T{0}", row)].Value = item.code;
                Sheet.Cells[string.Format("U{0}", row)].Value = item.shopProvince;
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