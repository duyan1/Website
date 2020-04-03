using EWShop.Areas.Admin.Logics;
using EWShop.Models;
using EWShop.Utilities;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;


namespace EWShop.Controllers
{
    [Authorize]
    public class EWController : BaseController
    {
        #region "View"
        public ActionResult CheckEwarranty()
        {
            ViewData["active"] = "1";
            Session["CAPTCHA"] = Common.RandomNumber(4);
            ViewData["data"] = null;
            return View();
        }
        public ActionResult RegisterEWForCustomer()
        {
            ViewData[CommonConstants.PROVINCES_INFO] = Common.loadListProvince();
            ViewData[CommonConstants.PLANTS_INFO] = Common.getListPlants();
            return View();
        }
        public ActionResult History()
        {
            ViewData[CommonConstants.PROVINCES_INFO] = Common.loadListProvince();
            SellOutLogics sol = new SellOutLogics();
            List<ProductSellOutReport> output = sol.listEWHistoryForShop(User.Identity.Name.ToString(), "0", DateTime.Now.AddDays(-7).ToString("dd/MM/yyyy"), DateTime.Now.ToString("dd/MM/yyyy"));
            return View(output);
        }
        public ActionResult FilterServiceRequest()
        {
            MasterDataLogics logics = new MasterDataLogics();
            ViewData[CommonConstants.MATERIAL_INFO] = logics.getMaterials();
            SellOutLogics sol = new SellOutLogics();
            List<ProductSellOutReport> output = sol.listEWHistoryForServiceRequest(User.Identity.Name.ToString(), "", "0");
            return View(output);
        }
        public ActionResult ServiceRequest(string id, string flat)
        {
            SelectList provList = new SelectList(Common.loadListProvince(), "value", "text");
            SelectList plantList = new SelectList(Common.getListPlants(), "value", "text");
            ViewBag.Provinces = provList;
            ViewBag.Plants = plantList;
            Session["CAPTCHA"] = Common.RandomNumber(4);
            ServiceRequest sr = new ServiceRequest();
            if ( id != null)
            {
                sr = ServiceRequestLogics.getInfoServiceRequest(id, flat);
            }
            return View(sr);
        }
        public ActionResult UploadExcel()
        {
            return View();
        }
        public ActionResult UploadExcel_Sub(string fileId)
        {
            List<ItemSellOutExcel> output = SellOutLogic.ListDataExcel(fileId, "2");
            ViewBag.fileId = fileId;
            ViewBag.info = SellOutLogic.loadInfoFileUpload(fileId);
            return View(output);
        }
        public ActionResult LoadDataExcel(string fileId, string flat)
        {
            List<ItemSellOutExcel> output = SellOutLogic.ListDataExcel(fileId, flat);
            ViewBag.fileId = fileId;
            ViewBag.info = SellOutLogic.loadInfoFileUpload(fileId);
            return PartialView("~/Views/Layout/_PartialListDataExcel.cshtml", output);
        }
        #endregion
        #region "HttpPost"
        [HttpPost]
        public ActionResult FilterServiceRequest(string model, string cusPhone)
        {
            MasterDataLogics logics = new MasterDataLogics();
            ViewData[CommonConstants.MATERIAL_INFO] = logics.getMaterials();
            SellOutLogics sol = new SellOutLogics();
            List<ProductSellOutReport> output = sol.listEWHistoryForServiceRequest(User.Identity.Name.ToString(), cusPhone, model);
            return PartialView("~/Views/Layout/_PartialListEWRegistersForServiceRequest.cshtml", output);
        }
        [HttpPost]
        public ActionResult CheckEwarranty(CheckBarcode data)
        {
            ItemResult result = new ItemResult();
            string clientCaptcha = data.clientCaptcha;
            string serverCaptcha = Session["CAPTCHA"].ToString();
            ViewData["active"] = data.flat;
            Session["CAPTCHA"] = Common.RandomNumber(4);
            if (!clientCaptcha.Equals(serverCaptcha))
            {
                result.code = "0";
                result.message = "Vui lòng nhập đúng mã xác nhận.";
                result.data = new RegisterEWInfo();
            }
            else
            {
                result.code = "1";
                result.message = "";
                result.data = EWarranty.CheckEWarranty(data.phone, data.serial, data.model, data.cardNo, data.flat);
            }
            return PartialView("~/Views/Layout/_ViewRegisterEW.cshtml", result);
        }
        [HttpPost]
        public ActionResult History(ItemFilterReport data)
        {
            SellOutLogics sol = new SellOutLogics();
            List<ProductSellOutReport> output = sol.listEWHistoryForShop(User.Identity.Name.ToString(), data.status, data.frmDate, data.toDate);
            return PartialView("~/Views/Layout/_PartialListEWRegistersForPSI.cshtml", output);
        }
        #endregion
        #region "Json"
        public JsonResult loadListDistrict(string provinceId)
        {
            if (provinceId == null || provinceId == "") provinceId = "0";
            return Json(Common.loadListDistrict(provinceId), JsonRequestBehavior.AllowGet);
        }
        public JsonResult completeUploadEW(string fileId)
        {
            return Json(FileUpload.completeUploadEW(fileId), JsonRequestBehavior.AllowGet);
        }
        public JsonResult loadListFileUpload(string frmDate, string toDate)
        {
            return Json(FileUpload.listFileUpload(frmDate, toDate, User.Identity.Name), JsonRequestBehavior.AllowGet);
        }
        public JsonResult loadDataTempFileUpload(string fileId)
        {
            return Json(FileUpload.listDataTempFileUpload(fileId), JsonRequestBehavior.AllowGet);
        }
        public JsonResult registerEW(ItemSellOut item)
        {
            return Json(SellOutLogic.RegisterEW(item,User.Identity.Name), JsonRequestBehavior.AllowGet);
        }
        public JsonResult loadInfoFileUpload(string fileId)
        {
            return Json(SellOutLogic.loadInfoFileUpload(fileId), JsonRequestBehavior.AllowGet);
        }
        public JsonResult checkBarcode(string barcode)
        {
            return Json(SellOutLogic.checkBarcode(barcode), JsonRequestBehavior.AllowGet);
        }
        public JsonResult searchModel(string term)
        {
            return Json(SellOutLogic.listModelFilterByPlantAndModel(term), JsonRequestBehavior.AllowGet);
        }
        public JsonResult getItemEWarranty(string ewCardId, string flat)
        {
            return Json(SellOutLogic.getItemEWarranty(ewCardId, flat), JsonRequestBehavior.AllowGet);
        }
        public JsonResult completeUploadEW_Test(string fileId)
        {
            return Json(FileUpload.completeUploadEW_Test(fileId), JsonRequestBehavior.AllowGet);
        }
        public JsonResult loadDataTempFileUpload_Test(string fileId, string status)
        {
            return Json(FileUpload.listDataTempFileUpload_Test(fileId, status), JsonRequestBehavior.AllowGet);
        }
        public JsonResult ExcuteAction(string fileId, string id)
        {
            ItemReturn output = new ItemReturn();
            output.code = "1";
            output.message = "";
            if (id != null)
            {
                ResultReturn result = SellOutLogic.RemoveItem(fileId, id, User.Identity.Name);
                output.code = result.code;
                output.message = result.msg;
            }
            else
            {
                output.code = "0";
                output.message = "Không chọn dòng dữ liệu cần xóa";
            }
            return Json(output, JsonRequestBehavior.AllowGet);
        }
        public JsonResult loadInfoFileUpload_Test(string fileId)
        {
            return Json(FileUpload.loadInfoFileUpload_Test(fileId), JsonRequestBehavior.AllowGet);
        }
        public JsonResult loadDataImageUpload(string fileId)
        {
            return Json(FileUpload.loadDataImageUpload(fileId), JsonRequestBehavior.AllowGet);
        }
        public JsonResult removeImage(string fileId,string imgId)
        {
            return Json(FileUpload.removeImage(fileId, imgId), JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult SaveChangeEW(ProductSellOutInfo input)
        {
            ResultReturn result = new ResultReturn();
            if (input != null)
            {
                SellOutLogics sol = new SellOutLogics();
                result = sol.SaveChangeEWItemForKeyShop(input, User.Identity.Name.ToString());
            }
            else
            {
                result.code = "0";
                result.msg = "No data input";
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult ServiceRequest(ServiceRequest data)
        {
            SelectList provList = new SelectList(Common.loadListProvince(), "value", "text");
            SelectList plantList = new SelectList(Common.getListPlants(), "value", "text");
            ViewBag.Provinces = provList;
            ViewBag.Plants = plantList;

            return Json(ServiceRequestLogics.RegisterServiceRequestForShop(data, User.Identity.Name), JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult uploadFileExcel()
        {
            ResultReturn result = new ResultReturn();
            HttpFileCollectionBase files = Request.Files;
            for (int i = 0; i < files.Count; i++)
            {

                HttpPostedFileBase file = files[i];
                if ((file != null) && (file.ContentLength > 0) && !string.IsNullOrEmpty(file.FileName))
                {
                    string origFileName = file.FileName;    
                     result = FileUpload.insertFileUpload_12072019(origFileName, User.Identity.Name);
                    if (result.code == "1")
                    {
                        NPOIReadExcel n = new NPOIReadExcel();
                        n.ReadExcelbyNPOI(file,result.msg);
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
        [HttpPost]
        public JsonResult uploadImageFile()
        {
            ResultReturn result = new ResultReturn();
            HttpFileCollectionBase files = Request.Files;
            bool flat = false;
            string fileNameStr = "";
            for (int i = 0; i < files.Count; i++)
            {
                HttpPostedFileBase file = files[i];
                if ((file != null) && (file.ContentLength > 0) && !string.IsNullOrEmpty(file.FileName))
                {
                    flat = true;
                    result.code = CommonConstants.EXECUTE_SUCCESS;
                    string origFileName = file.FileName;
                    string fileName = Common.RandomNumber(9) + System.IO.Path.GetExtension(file.FileName).ToLower();
                    string fileUrl = (DateTime.Now.Year * 100 + DateTime.Now.Month).ToString();
                    fileNameStr = ("".Equals(fileNameStr) ? "" : fileNameStr + ";") + FileUpload.upLoadImage(file, fileUrl, fileName);
                }
            }
            if (!flat)
            {
                result.code = CommonConstants.EXECUTE_UNSUCCESS;
                result.msg = "Không có hình ảnh cần tải";
            }
            else
            {
                result.msg = fileNameStr;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult uploadImage()
        {
            ResultReturn result = new ResultReturn();
            HttpFileCollectionBase files = Request.Files;
            string fileId = Request.Form["fileId"];
            bool flat = false;
            string fileNameStr = "";
        
            for (int i = 0; i < files.Count; i++)
            {
                HttpPostedFileBase file = files[i];
                if ((file != null) && (file.ContentLength > 0) && !string.IsNullOrEmpty(file.FileName))
                {
                    flat = true;
                    result.code = CommonConstants.EXECUTE_SUCCESS;
                    string origFileName = file.FileName;
                    string fileName = Common.RandomNumber(9) + System.IO.Path.GetExtension(file.FileName).ToLower();
                    string fileUrl = (DateTime.Now.Year * 100 + DateTime.Now.Month).ToString();
                    fileNameStr = FileUpload.upLoadImage(file, fileUrl, fileName);
                    if (!"".Equals(fileNameStr))
                    {
                        result = FileUpload.insertImageUpload(fileNameStr, origFileName, fileId, User.Identity.Name);
                    }
                }
            }
            if (!flat)
            {
                result.code = CommonConstants.EXECUTE_UNSUCCESS;
                result.msg = "Không có hình ảnh cần tải";
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult UploadExcel_Sub(ItemSellOutExcel input)
        {
            return Json(SellOutLogic.UpdateItemUploadExcel(input, User.Identity.Name), JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region "Excel"
        public void DownloadExcel(string status, string frmDate, string toDate)
        {
            SellOutLogics logics = new SellOutLogics();
            List<ProductSellOutReport> output = logics.listEWHistoryForShop(User.Identity.Name.ToString(), status, frmDate, toDate);

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
            Sheet.Cells["N1"].Value = "REMARKS";
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
                Sheet.Cells[string.Format("N{0}", row)].Value = item.status;
                row++;
            }


            Sheet.Cells["A:AZ"].AutoFitColumns();
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment: filename=" + "EWReport.xlsx");
            Response.BinaryWrite(Ep.GetAsByteArray());
            Response.End();
        }
        public void ExportExcel(string fileId, string flat)
        {
            List<ItemSellOutExcel> data = SellOutLogic.ListDataExcel(fileId, flat);
            ExcelPackage Ep = new ExcelPackage();
            ExcelWorksheet Sheet = Ep.Workbook.Worksheets.Add("Report");
            Sheet.Cells["A1"].Value = "TÊN KHÁCH HÀNG";
            Sheet.Cells["B1"].Value = "SĐT KHÁCH HÀNG";
            Sheet.Cells["C1"].Value = "EMAIL KHÁCH HÀNG";
            Sheet.Cells["D1"].Value = "ĐỊA CHỈ KHÁCH HÀNG";
            Sheet.Cells["E1"].Value = "SỐ MÁY";
            Sheet.Cells["F1"].Value = "KIỂU MÁY";
            Sheet.Cells["G1"].Value = "NGÀY MUA";
            Sheet.Cells["H1"].Value = "TRẠNG THÁI";
            int row = 2;
            foreach (var item in data)
            {
                Sheet.Cells[string.Format("A{0}", row)].Value = item.cusName;
                Sheet.Cells[string.Format("B{0}", row)].Value = item.cusPhone;
                Sheet.Cells[string.Format("C{0}", row)].Value = item.cusEmail;
                Sheet.Cells[string.Format("D{0}", row)].Value = item.cusAddress;
                Sheet.Cells[string.Format("E{0}", row)].Value = item.barcode;
                Sheet.Cells[string.Format("F{0}", row)].Value = item.model;
                Sheet.Cells[string.Format("G{0}", row)].Value = item.pod;
                Sheet.Cells[string.Format("H{0}", row)].Value = item.status;
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