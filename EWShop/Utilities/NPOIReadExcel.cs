using EWShop.Models;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace EWShop.Utilities
{
    public class NPOIReadExcel
    {
        public NPOIReadExcel() { }
        public SLExcelData ReadExcelbyNPOI(HttpPostedFileBase file, string fileId)
        {
            var data = new SLExcelData();
            DataFormatter df = new DataFormatter();
            string sFileExtension = Path.GetExtension(file.FileName).ToLower();
            ISheet sheet = null;
            if (file.ContentLength <= 0)
            {
                data.Status.Message = "You uploaded an empty file";
                return data;
            }
            if (sFileExtension == ".xls")//This will read the Excel 97-2000 formats    
            {
                HSSFWorkbook hssfwb = new HSSFWorkbook(file.InputStream);
                sheet = hssfwb.GetSheetAt(0);
            }
            else //This will read the Excel xlsx formats
            {
                XSSFWorkbook hssfwb = new XSSFWorkbook(file.InputStream);
                sheet = hssfwb.GetSheetAt(0);
            }
            IRow headerRow = sheet.GetRow(0);
            int cellCount = headerRow.LastCellNum;
            int indCustName = 0;
            int indCustPhone = 1;
            int indAddress = 2;
            int indBarCode = 3;
            int indPOD = 4;
            int indBranchCode = 5;
            for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++)
            {
                IRow row = sheet.GetRow(i);
                if (row == null) continue;
                if (row.Cells.All(d => d.CellType == CellType.Blank)) continue;
                string cusName = "";
                if (row.GetCell(indCustName) != null)
                    cusName = row.GetCell(indCustName).ToString();
                string cusPhone = "";
                if (row.GetCell(indCustPhone) != null)
                    cusPhone = Common.ReplacePhone(row.GetCell(indCustPhone).ToString());
                string cusAddress = "";
                if (row.GetCell(indAddress) != null)
                    cusAddress = row.GetCell(indAddress).ToString();
                string barcode = "";
                if (row.GetCell(indBarCode) != null)
                    barcode = row.GetCell(indBarCode).ToString();
                string ngaymua = "";
                if (row.GetCell(indPOD) != null)
                {
                    ngaymua = row.GetCell(indPOD).ToString();
                }
                string brandCode = "";
                if (row.GetCell(indBranchCode) != null)
                {
                    brandCode = row.GetCell(indBranchCode).ToString();
                }

                if (cusName != "" || cusPhone != "" || barcode != "" || ngaymua != "" || brandCode != "")
                    FileUpload.insertOneRow12072019(cusName, cusPhone, brandCode, cusAddress, barcode, "", ngaymua, fileId);
            }
                return data;
        }
        public List<ItemReturn> ReadExcelFileBarcode(HttpPostedFileBase file)
        {
            List<ItemReturn> lstResult = new List<ItemReturn>();
            ItemReturn result = new ItemReturn();
            var data = new SLExcelData();
            DataFormatter df = new DataFormatter();
            string sFileExtension = Path.GetExtension(file.FileName).ToLower();
            ISheet sheet = null;
            if (file.ContentLength <= 0)
            {
                return lstResult;
            }
            if (sFileExtension == ".xls")//This will read the Excel 97-2000 formats    
            {
                HSSFWorkbook hssfwb = new HSSFWorkbook(file.InputStream);
                sheet = hssfwb.GetSheetAt(0);
            }
            else //This will read the Excel xlsx formats
            {
                XSSFWorkbook hssfwb = new XSSFWorkbook(file.InputStream);
                sheet = hssfwb.GetSheetAt(0);
            }

            IRow headerRow = sheet.GetRow(0);
            int posBarCode = 0;
            for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++)
            {
                IRow row = sheet.GetRow(i);
                if (row == null) continue;
                if (row.Cells.All(d => d.CellType == CellType.Blank)) continue;
                string barcode = "";
                if (row.GetCell(posBarCode) != null)
                    barcode = row.GetCell(posBarCode).ToString();
                if (barcode != "")
                {
                    result = FileUpload.CheckInstallmentBarCode(barcode);
                    lstResult.Add(result);
                }
            }
            return lstResult;
        }
    }
}