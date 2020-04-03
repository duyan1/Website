using EWShop.Areas.Admin.Logics;
using EWShop.Areas.CallCenter.Models;
using EWShop.Models;
using EWShop.Utilities;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace EWShop.Areas.CallCenter.Controllers
{
    public class EwarrantyController : CallCenterController
    {
        #region "Method Get"
        public ActionResult EWNotCompleted()
        {
            MasterDataLogics logics = new MasterDataLogics();
            string frmDate = DateTime.Now.AddDays(-3).ToString("dd/MM/yyyy");
            string toDate = DateTime.Now.ToString("dd/MM/yyyy");
            ViewData[CommonConstants.PROVINCES_INFO] = logics.getListProvinces();
            ViewData[CommonConstants.SHOP_INFO] = logics.getListShops();
            ViewData[CommonConstants.PLANTS_INFO] = logics.getListPlants();
            return View(EwarrantyLogics.listRegisterEWNotCompleted(frmDate, toDate, "0", "2", ""));
        }
        public ActionResult SMSInvalid()
        {
            MasterDataLogics logics = new MasterDataLogics();
            string frmDate = DateTime.Now.AddDays(-1).ToString("dd/MM/yyyy");
            string toDate = DateTime.Now.ToString("dd/MM/yyyy");
            ViewData[CommonConstants.PROVINCES_INFO] = logics.getListProvinces();
            ViewData[CommonConstants.SHOP_INFO] = logics.getListShops();
            ViewData[CommonConstants.PLANTS_INFO] = logics.getListPlants();
            return View(EwarrantyLogics.listSMSInvalid(frmDate, toDate, "", ""));
        }
        public ActionResult ListRegisteredEW()
        {
            string frmDate = DateTime.Now.AddDays(-7).ToString("dd/MM/yyyy");
            string toDate = DateTime.Now.ToString("dd/MM/yyyy");
            return View(EwarrantyLogics.ListRegisteredEW(frmDate, toDate, "", ""));
        }
        public ActionResult ListEWNotYetTransferGSIS()
        {
            string frmDate = DateTime.Now.AddDays(-7).ToString("dd/MM/yyyy");
            string toDate = DateTime.Now.ToString("dd/MM/yyyy");
            ViewData["MONTHS"] = Common.loadListMonth();
            return View(EwarrantyLogics.ListEWNotYetTransferGSIS(frmDate, toDate, "10", "0",""));
        }
        #endregion
        #region "Method Post"
        [HttpPost] 
        public ActionResult EWNotCompleted(string frmDate, string toDate, string ewSource, string isImage, string strBarcode)
        {
            return PartialView("~/Areas/CallCenter/Views/Shared/_PartialListOfEwarranty.cshtml", EwarrantyLogics.listRegisterEWNotCompleted(frmDate, toDate, ewSource, isImage, strBarcode));
        }
        [HttpPost]
        public ActionResult SMSInvalid(string frmDate, string toDate, string status, string check)
        {
            return PartialView("~/Areas/CallCenter/Views/Shared/_PartialListSMSInvalid.cshtml", EwarrantyLogics.listSMSInvalid(frmDate, toDate, status, check));
        }
        [HttpPost]
        public ActionResult ListRegisteredEW(string frmDate, string toDate, string source, string barcode)
        {
            return PartialView("~/Areas/CallCenter/Views/Shared/_PartialListRegisteredEwarranty.cshtml", EwarrantyLogics.ListRegisteredEW(frmDate, toDate, source, barcode)); 
        }
        [HttpPost]
        public ActionResult ListEWNotYetTransferGSIS(string frmDate, string toDate, string number, string option, string month)
        {
            return PartialView("~/Areas/CallCenter/Views/Shared/_PartialListRegisteredEwarranty.cshtml", EwarrantyLogics.ListEWNotYetTransferGSIS(frmDate, toDate, number, option, month));
        }
        #endregion
        #region "GetJsonResult"
        public JsonResult getItemInfo(string cardId)
        {
            if (cardId == null || "".Equals(cardId))
                cardId = "0";
            return Json(EwarrantyLogics.getItemInfo(cardId), JsonRequestBehavior.AllowGet);
        }
        public JsonResult loadListDistrict(string provinceId)
        {
            if (provinceId == null || provinceId == "" || "undefined".Equals(provinceId) || "null".Equals(provinceId)) provinceId = "0";
            return Json(Common.loadListDistrict(provinceId), JsonRequestBehavior.AllowGet);
        }
        public JsonResult loadListMaterials(string plantId)
        {
            if (plantId == null || "".Equals(plantId) || "undefined".Equals(plantId) || "null".Equals(plantId)) plantId = "0";
            return Json(Common.loadListModel(plantId), JsonRequestBehavior.AllowGet);
        }
        public JsonResult checkBarcode(string barcode, string matId)
        {
            if (barcode == null || "undefined".Equals(barcode) || "null".Equals(barcode)) barcode = "";
            if (matId == null || "undefined".Equals(matId) || "null".Equals(matId)) matId = "0";
            return Json(EwarrantyLogics.checkBarcode(barcode, matId), JsonRequestBehavior.AllowGet);
        }
        public JsonResult checkShopPhone(string phone)
        {
            return Json(EwarrantyLogics.checkShopPhone(phone), JsonRequestBehavior.AllowGet);
        }
        public JsonResult getEwarrantyInfo(string cardId)
        {
            return Json(EwarrantyLogics.getEwarrantyInfo(cardId), JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult Register(EwarrantySMS input)
        {
            return Json(EwarrantyLogics.registerEWSMS(input, User.Identity.Name.ToLower()), JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult saveEWarranty(EwarrantyItem input)
        {
            return Json(EwarrantyLogics.saveEWarranty(input, User.Identity.Name.ToLower()), JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult rejectEWarranty(string cardId, string reason)
        {
            return Json(EwarrantyLogics.rejectEWarranty(cardId, reason), JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult updateComment(string id, string comment)
        {
            return Json(EwarrantyLogics.updateComment(id, comment), JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult closeSMSInvalid(string id, string comment)
        {
            return Json(EwarrantyLogics.closeSMSInvalid(id, comment, User.Identity.Name.ToLower()), JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult cancelEwarranty(string cardId, string reason)
        {
            return Json(EwarrantyLogics.cancelEwarranty(cardId, reason, User.Identity.Name.ToLower()), JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region "Excel"
        public void DownloadExcelNotComplete(string source, string barcode, string image, string frmDate, string toDate)
        {
            SellOutLogics logics = new SellOutLogics();
            List<EwarrantyItem> output = EwarrantyLogics.listRegisterEWNotCompleted(frmDate, toDate, source, image, barcode);

            ExcelPackage Ep = new ExcelPackage();
            ExcelWorksheet Sheet = Ep.Workbook.Worksheets.Add("Report");
            Sheet.Cells["A1"].Value = "BARCODE";
            Sheet.Cells["B1"].Value = "MODEL";
            Sheet.Cells["C1"].Value = "POD";
            Sheet.Cells["D1"].Value = "CUSTOMER NAME";
            Sheet.Cells["E1"].Value = "CUSTOMER PHONE";
            Sheet.Cells["F1"].Value = "CHANNEL";
            Sheet.Cells["G1"].Value = "SHOPCODE";
            Sheet.Cells["H1"].Value = "SHOPNAME";
            Sheet.Cells["I1"].Value = "EWPSI";
            int row = 2;
            foreach (var item in output)
            {
                Sheet.Cells[string.Format("A{0}", row)].Value = item.barcode;
                Sheet.Cells[string.Format("B{0}", row)].Value = item.model;
                Sheet.Cells[string.Format("C{0}", row)].Value = item.POD;
                Sheet.Cells[string.Format("D{0}", row)].Value = item.cusName;
                Sheet.Cells[string.Format("E{0}", row)].Value = item.cusPhone;
                Sheet.Cells[string.Format("F{0}", row)].Value = item.EWSource;
                Sheet.Cells[string.Format("G{0}", row)].Value = item.shopCode;
                Sheet.Cells[string.Format("H{0}", row)].Value = item.shopName;
                Sheet.Cells[string.Format("I{0}", row)].Value = item.flag;
                row++;
            }

            Sheet.Cells["A:AZ"].AutoFitColumns();
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment: filename=" + "EWReport.xlsx");
            Response.BinaryWrite(Ep.GetAsByteArray());
            Response.End();
        }
        public void DownloadExcelCompleted(string source, string barcode, string frmDate, string toDate)
        {
            SellOutLogics logics = new SellOutLogics();
            List<ProductSellOutReport> output = EwarrantyLogics.ListRegisteredEW(frmDate, toDate, source, barcode);

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
                row++;
            }

            Sheet.Cells["A:AZ"].AutoFitColumns();
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment: filename=" + "EWReport.xlsx");
            Response.BinaryWrite(Ep.GetAsByteArray());
            Response.End();
        }
        public void DownloadExcelSMSRegister(string status, string check, string frmDate, string toDate)
        {
            SellOutLogics logics = new SellOutLogics();
            List<SMSItem> output = EwarrantyLogics.listSMSInvalid(frmDate, toDate, status, check);

            ExcelPackage Ep = new ExcelPackage();
            ExcelWorksheet Sheet = Ep.Workbook.Worksheets.Add("Report");
            Sheet.Cells["A1"].Value = "SENDER";
            Sheet.Cells["B1"].Value = "CONTENT";
            Sheet.Cells["C1"].Value = "REG.DATE";
            Sheet.Cells["D1"].Value = "STATUS";
            Sheet.Cells["E1"].Value = "CHECK";
            Sheet.Cells["F1"].Value = "LOCK";
            Sheet.Cells["G1"].Value = "COMMENT";
            int row = 2;
            foreach (var item in output)
            {
                Sheet.Cells[string.Format("A{0}", row)].Value = item.sender;
                Sheet.Cells[string.Format("B{0}", row)].Value = item.content;
                Sheet.Cells[string.Format("C{0}", row)].Value = item.regDate;
                Sheet.Cells[string.Format("D{0}", row)].Value = item.status;
                Sheet.Cells[string.Format("E{0}", row)].Value = item.isCheck;
                Sheet.Cells[string.Format("F{0}", row)].Value = item.isLock;
                Sheet.Cells[string.Format("G{0}", row)].Value = item.comment;
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