using EWShop.Areas.Admin.Logics;
using EWShop.Areas.Admin.Models;
using EWShop.Areas.Sales.Logics;
using EWShop.Areas.Sales.Models;
using EWShop.Models;
using EWShop.Utilities;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EWShop.Areas.Sales.Controllers
{
    public class MWGController : SalesController
    {
        // GET: Sales/MWG
        public ActionResult SalePriceQuotation()
        {
            ViewData["CUSTOMER"] = Common.getListCustomers();
            return View(MaterialLogic.getListRecords("","",""));
        }
        public ActionResult PurchaseChangeOrders()
        {
            string frmDate = DateTime.Now.AddDays(-7).ToString("dd/MM/yyyy");
            string toDate = DateTime.Now.ToString("dd/MM/yyyy");
            ViewData["DETAILS"] = new List<PurchaseOrderDetail>();
            return View(PurchaseOrderLogic.getListRecords(frmDate, toDate, "", ""));
        }
        public ActionResult InventoryStock()
        {
            return View();
        }
        #region "Method HttpPost"
        [HttpPost]
        public ActionResult SalePriceQuotation(string customer, string modified, string transfered)
        {
            return PartialView("~/Areas/Sales/Views/Shared/_PartialListMaterialPrices.cshtml", MaterialLogic.getListRecords(customer, modified, transfered));
        }
        [HttpPost]
        public ActionResult PurchaseChangeOrders(string frmDate, string toDate, string confirm, string poNo)
        {
            return PartialView("~/Areas/Sales/Views/Shared/_PartialListPurchaseOrders.cshtml", PurchaseOrderLogic.getListRecords(frmDate, toDate, confirm, poNo));
        }
        [HttpPost]
        public JsonResult UpdateMRPrice(string customer, string matCode, string mrp, string sdp, string currency)
        {
            return Json(MaterialLogic.updateMRPrice(customer, matCode, mrp, sdp, currency), JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult saveRecord(PurchaseOrderDetail input)
        {
            return Json(PurchaseOrderLogic.saveRecord(input), JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult rejectRecord(string purchaseOrder, string remarks)
        {
            return Json(PurchaseOrderLogic.rejectRecord(purchaseOrder, remarks), JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult getInfoDetail(string purchaseOrder)
        {
            return Json(PurchaseOrderLogic.getDetailRecords(purchaseOrder), JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult downloadMatPrice(string customer)
        {
            return Json(MaterialLogic.downloadMaterialPrice(customer), JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult loadListRecordNotTransfer(string customer)
        {
            return Json(MaterialLogic.loadListRecordNotTransfer(customer), JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult loadDetail(string customer, string matCode)
        {
            return Json(MaterialLogic.loadDetailRecord(customer, matCode), JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult getListDetailRecord(string purchaseOrder)
        {
            return PartialView("~/Areas/Sales/Views/Shared/_PartialPurchaseOrderDetail.cshtml", PurchaseOrderLogic.getListDetailRecord(purchaseOrder));
        }
        [HttpGet]
        public JsonResult confirmRecord(string poNo)
        {
            return Json(PurchaseOrderLogic.confirmRecord(poNo), JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region "Transfer MWG"
        [HttpPost]
        public JsonResult transferToMWG(string customer, List<string> matCode)
        {
            return Json(MaterialLogic.transferToMWG(customer,matCode), JsonRequestBehavior.AllowGet);
        }

        #endregion
        #region "Excel"
        public void DownloadExcelMaterial(string customer, string modified, string transfered)
        {
            List<MaterialPriceModel> list = MaterialLogic.getListRecords(customer, modified, transfered);

            ExcelPackage Ep = new ExcelPackage();
            ExcelWorksheet Sheet = Ep.Workbook.Worksheets.Add("Report");
            Sheet.Cells["A1"].Value = "CUSTOMER";
            Sheet.Cells["B1"].Value = "MATCODE";
            Sheet.Cells["C1"].Value = "MATDESCRIPTION";
            Sheet.Cells["D1"].Value = "AMOUNT";
            Sheet.Cells["E1"].Value = "AMOUNT OLD";
            Sheet.Cells["F1"].Value = "MRP";
            Sheet.Cells["G1"].Value = "MRP OLD";
            Sheet.Cells["H1"].Value = "SAMPLE";
            Sheet.Cells["I1"].Value = "SAMPLE OLD";
            Sheet.Cells["J1"].Value = "CURRENCY(Mặc định: VND)";
            Sheet.Cells["K1"].Value = "BEGINDATE";
            Sheet.Cells["L1"].Value = "ISMODIFIED";
            Sheet.Cells["M1"].Value = "ISTRANSFER";
            int row = 2;
            foreach (var item in list)
            {
                Sheet.Cells[string.Format("A{0}", row)].Value = customer;
                Sheet.Cells[string.Format("B{0}", row)].Value = item.matCode;
                Sheet.Cells[string.Format("C{0}", row)].Value = item.matName;
                Sheet.Cells[string.Format("D{0}", row)].Value = item.amount;
                Sheet.Cells[string.Format("E{0}", row)].Value = item.amount_old;
                Sheet.Cells[string.Format("F{0}", row)].Value = item.marketRetailPrice;
                Sheet.Cells[string.Format("G{0}", row)].Value = item.marketRetailPrice_old;
                Sheet.Cells[string.Format("H{0}", row)].Value = item.sampleDisplayPrice;
                Sheet.Cells[string.Format("I{0}", row)].Value = item.sampleDisplayPrice_old;
                Sheet.Cells[string.Format("J{0}", row)].Value = item.currency;
                Sheet.Cells[string.Format("K{0}", row)].Value = item.beginDate;
                Sheet.Cells[string.Format("L{0}", row)].Value = item.isChanged;
                Sheet.Cells[string.Format("M{0}", row)].Value = item.isTransfered;
                row++;
            }


            Sheet.Cells["A:AZ"].AutoFitColumns();
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment: filename=" + "EWReport.xlsx");
            Response.BinaryWrite(Ep.GetAsByteArray());
            Response.End();
        }
        #endregion
    }
}