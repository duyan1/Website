using EWShop.Areas.Admin.Logics;
using EWShop.Models;
using EWShop.Utilities;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EWShop.Controllers
{
    [Authorize]
    public class PayRewardController : BaseController
    {
        public ActionResult Index()
        {
            List<DataReturn> result = PayRewardLogics.getListPrograms(User.Identity.Name.ToLower());
            return View(result);
        }
        public ActionResult RewardInfo()
        {
            return RedirectToAction("Index", "PayReward");
        }
        [HttpPost]
        public ActionResult RewardInfo(string proid)
        {
            if(proid == null || "".Equals(proid))
                return RedirectToAction("Index", "PayReward");
            RewardInfo reward = PayRewardLogics.getRewardInfo(User.Identity.Name.ToLower(), proid);
            ViewBag.proid = proid;
            return View(reward);
        }
        public ActionResult RewardDetail(string status, string proid)
        {
            ViewBag.flat = status;
            ViewBag.proid = proid;
            List<ProductPayWard> list = PayRewardLogics.getListEW(proid, status, User.Identity.Name.ToLower(), "", "");
            return View(list);
        }
        [HttpPost]
        public ActionResult RewardDetail(ItemFilterReport input)
        {
            ViewBag.flat = input.status;
            ViewBag.proid = input.proid;
            List<ProductPayWard> list = PayRewardLogics.getListEW(input.proid, input.status, User.Identity.Name.ToLower(), "", "");
            return PartialView("~/Views/Layout/_PartialListEWRegistersForPayReward.cshtml", list);
        }
        [HttpPost]
        public JsonResult uploadFile(string id)
        {
            ResultReturn result = new ResultReturn();
            HttpFileCollectionBase files = Request.Files;
            bool flat = false;
            for (int i = 0; i < files.Count; i++)
            {
                HttpPostedFileBase file = files[i];
                if ((file != null) && (file.ContentLength > 0) && !string.IsNullOrEmpty(file.FileName))
                {
                    flat = true;
                    string origFileName = file.FileName;
                    string fileName = Common.RandomNumber(9) + System.IO.Path.GetExtension(file.FileName).ToLower();
                    string fileUrl = "reward/" + (DateTime.Now.Year * 100 + DateTime.Now.Month).ToString();
                    string fileNameStr = FileUpload.upLoadImage(file, fileUrl, fileName);
                    string res = PayRewardLogics.insertImageIntoDB(origFileName, fileNameStr, id, User.Identity.Name.ToLower());
                    result.code = "".Equals(res) ? CommonConstants.EXECUTE_SUCCESS : CommonConstants.EXECUTE_UNSUCCESS;
                    result.msg = res;
                }
            }
            if (!flat)
            {
                result.code = CommonConstants.EXECUTE_UNSUCCESS;
                result.msg = "Không có hình ảnh cần tải";
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult getItem(string ewCardNo)
        {
            List<ImageInfo> result = new List<ImageInfo>();
            result = PayRewardLogics.getListImageUpload(ewCardNo);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult saveComment(string ewCardNo, string comment)
        {
            ResultReturn result = PayRewardLogics.updateComment(ewCardNo, comment,User.Identity.Name.ToLower());
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult removeItem(string ewCardNo, string id)
        {
            ResultReturn result = new ResultReturn();
            result = PayRewardLogics.removeImage(ewCardNo, id);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public void DownloadExcel(string status, string proid)
        {
            List<ProductPayWard> list = PayRewardLogics.getListEW(proid, status, User.Identity.Name.ToLower(), "", "");

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
            Sheet.Cells["I1"].Value = "SHOPCODE";
            Sheet.Cells["J1"].Value = "SHOPNAME";
            Sheet.Cells["K1"].Value = "REG.DATE";
            Sheet.Cells["L1"].Value = "STATUS";
            Sheet.Cells["M1"].Value = "REMARKS";
            int row = 2;
            foreach (var item in list)
            {

                Sheet.Cells[string.Format("A{0}", row)].Value = item.ewCardNo;
                Sheet.Cells[string.Format("B{0}", row)].Value = item.pod;
                Sheet.Cells[string.Format("C{0}", row)].Value = item.barcode;
                Sheet.Cells[string.Format("D{0}", row)].Value = item.model;
                Sheet.Cells[string.Format("E{0}", row)].Value = item.plantName;
                Sheet.Cells[string.Format("F{0}", row)].Value = item.cusName;
                Sheet.Cells[string.Format("G{0}", row)].Value = item.cusPhone;
                Sheet.Cells[string.Format("H{0}", row)].Value = item.cusAddress;
                Sheet.Cells[string.Format("I{0}", row)].Value = item.shopCode;
                Sheet.Cells[string.Format("J{0}", row)].Value = item.shopName;
                Sheet.Cells[string.Format("K{0}", row)].Value = item.regDate;
                Sheet.Cells[string.Format("L{0}", row)].Value = item.rsName;
                Sheet.Cells[string.Format("M{0}", row)].Value = item.comment;
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