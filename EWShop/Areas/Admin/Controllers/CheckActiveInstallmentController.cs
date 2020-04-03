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
    public class CheckActiveInstallmentController : AdminController
    {
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public void uploadFileExcel()
        {
            HttpFileCollectionBase files = Request.Files;
            for (int i = 0; i < files.Count; i++)
            {
                HttpPostedFileBase file = files[i];
                if ((file != null) && (file.ContentLength > 0) && !string.IsNullOrEmpty(file.FileName))
                {
                    string origFileName = file.FileName;
                    NPOIReadExcel n = new NPOIReadExcel();
                    List<ItemReturn> lstResult = n.ReadExcelFileBarcode(file);
                    DownloadExcel(lstResult);
                }
            }
        }
        public void DownloadExcel(List<ItemReturn> lstResult)
        {
            ExcelPackage Ep = new ExcelPackage();
            ExcelWorksheet Sheet = Ep.Workbook.Worksheets.Add("Sheet1");
            Sheet.Cells["A1"].Value = "SerialNumber";
            Sheet.Cells["B1"].Value = "ModelType";
            Sheet.Cells["C1"].Value = "Status";
            int row = 2;
            foreach (var item in lstResult)
            {
                Sheet.Cells[string.Format("A{0}", row)].Value = item.code;
                Sheet.Cells[string.Format("B{0}", row)].Value = item.message;
                Sheet.Cells[string.Format("C{0}", row)].Value = item.data;
                row++;
            }


            Sheet.Cells["A:AZ"].AutoFitColumns();
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment: filename=" + "Serial.xlsx");
            Response.BinaryWrite(Ep.GetAsByteArray());
            Response.End();
        }
    }
}