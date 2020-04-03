using EWShop.Areas.Admin.Logics;
using EWShop.Models;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EWShop.Areas.Admin.Controllers
{
    [Authorize(Roles = "administrator")]
    public class RewardController : AdminController
    {
        // GET: Admin/Reward
        public ActionResult Index()
        {
            MasterDataLogics logics = new MasterDataLogics();
            ViewData[CommonConstants.SHOP_INFO] = logics.getListShops();
            ViewData[CommonConstants.STATUS_INFO] = logics.getListRewardStatus();
            ViewData[CommonConstants.PROGRAM_INFO] = logics.getListPrograms();
            ViewBag.MainMenu = CommonConstants.MENU_MAIN_E_WARRANTY;
            ViewBag.ChildMenu = CommonConstants.MENU_VIEW_HISTORY;
            List<ProductPayWard> list = PayRewardLogics.getListEW(ConfigurationManager.AppSettings["programId"].ToString(), "4", User.Identity.Name.ToLower(),"","1");
            return View(list);
        }
        [HttpPost]
        public ActionResult Index(ItemFilterReport input)
        {
            return PartialView("~/Views/Layout/_PartialListEWRegistersForPayReward.cshtml", PayRewardLogics.getListEW(input.proid, input.status, User.Identity.Name.ToLower(), input.shop, input.flat));
        }
        public JsonResult loadImages(string ewCardNo)
        {
            List<string> result = new List<string>();
            result = PayRewardLogics.getListImages(ewCardNo);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult saveEWPayReward(string ewCardNo, string status, string comment, string proid)
        {
            ResultReturn result = PayRewardLogics.updatePayReward(ewCardNo, status, comment, User.Identity.Name.ToLower(), proid);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public void DownloadExcel(string proid, string status, string frmDate, string toDate, string shop)
        {
            List<ProductPayWard> list = PayRewardLogics.getListEW(proid, status, User.Identity.Name.ToLower(), shop, "");

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