using EWShop.Models;
using EWShop.Utilities;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EWShop.Areas.Admin.Controllers
{
    public class NotificationController : AdminController
    {
        // GET: Admin/Notification
        public ActionResult Index()
        {
            ViewData["MONTH"] = Common.loadListMonth();
            return View();
        }
        public JsonResult getlistNotifications(string month)
        {
            if (month == null) month = "";
            return Json(NotificationLogics.getListNotification(month), JsonRequestBehavior.AllowGet);
        }
        public void DownloadExcel(string month)
        {
            if (month == null) month = "";
            List<Notification> list = NotificationLogics.getListNotification(month);

            ExcelPackage Ep = new ExcelPackage();
            ExcelWorksheet Sheet = Ep.Workbook.Worksheets.Add("Report");
            Sheet.Cells["A1"].Value = "Id";
            Sheet.Cells["B1"].Value = "Title";
            Sheet.Cells["C1"].Value = "Content";
            Sheet.Cells["D1"].Value = "IsActive";
            Sheet.Cells["E1"].Value = "BeginDate";
            Sheet.Cells["F1"].Value = "EndDate";
            Sheet.Cells["G1"].Value = "Remarks";
            int row = 2;
            foreach (var item in list)
            {
                Sheet.Cells[string.Format("A{0}", row)].Value = item.NId;
                Sheet.Cells[string.Format("B{0}", row)].Value = item.NTitle;
                Sheet.Cells[string.Format("C{0}", row)].Value = item.NContent;
                Sheet.Cells[string.Format("D{0}", row)].Value = item.isActive;
                Sheet.Cells[string.Format("E{0}", row)].Value = item.beginDate;
                Sheet.Cells[string.Format("F{0}", row)].Value = item.endDate;
                Sheet.Cells[string.Format("G{0}", row)].Value = item.remarks;
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