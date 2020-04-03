using EWShop.Models;
using EWShop.Utilities;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;

namespace EWShop.Areas.Distribution.Models
{
    public class SellThrough
    {
        public string barcode { get; set; }
        public string matCode { get; set; }
        public string model { get; set; }
        public string plant { get; set; }
        public string sellDate { get; set; }
        public string shopCode { get; set; }
        public string shopName { get; set; }
        public string transuser { get; set; }
        public string transdate { get; set; }
        public string comment { get; set; }
    }
    public class Shop
    {
        public string shopCode { get; set; }
        public string shopName { get; set; }
        public string shopPhone { get; set; }
        public string shopAddress { get; set; }
    }
    public static class FileLogics
    {
        public static ResultReturn ReadFileText(HttpPostedFileBase fileInput, string user)
        {
            ResultReturn result = new ResultReturn();
            int counter = 0;
            try
            {
                using (StreamReader file = new StreamReader(fileInput.InputStream))
                {
                    string ln;
                    while ((ln = file.ReadLine()) != null)
                    {
                        string[] array = ln.Split(',');
                        InsertDataSellThrough(array[4], array[3], "", "", "", "", user);
                        counter++;
                    }
                    file.Close();
                }
                result.code = "1";
                result.msg = "Count : " + counter;
            }
            catch(Exception ex)
            {
                result.code = "0";
                result.msg = ex.Message;
            }
            
            return result;
        }
        public static ResultReturn ReadFileExcel(HttpPostedFileBase file, string user)
        {
            ResultReturn result = new ResultReturn();
            DataFormatter df = new DataFormatter();
            string sFileExtension = Path.GetExtension(file.FileName).ToLower();
            ISheet sheet = null;
            if (file.ContentLength <= 0)
            {
                result.code = CommonConstants.EXECUTE_UNSUCCESS;
                result.msg = "You uploaded an empty file";
                return result;
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
            try
            {
                for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++)
                {
                    IRow row = sheet.GetRow(i);
                    if (row == null) continue;
                    if (row.Cells.All(d => d.CellType == CellType.Blank)) continue;
                    string barcode = "";
                    if (row.GetCell(0) != null)
                        barcode = row.GetCell(0).ToString();
                    string pod = "";
                    if (row.GetCell(1) != null)
                        pod = row.GetCell(1).ToString();
                    string shopCode = "";
                    if (row.GetCell(2) != null)
                        shopCode = row.GetCell(2).ToString();
                    string shopName = "";
                    if (row.GetCell(3) != null)
                        shopName = row.GetCell(3).ToString();
                    string shopAddress = "";
                    if (row.GetCell(4) != null)
                        shopAddress = row.GetCell(4).ToString();
                    string sPhone = "";
                    if (row.GetCell(5) != null)
                        sPhone = row.GetCell(5).ToString();
                    if (barcode != "" && pod != "")
                        if(("".Equals(shopCode) && !"".Equals(shopName)) || !"".Equals(shopCode))
                            InsertDataSellThrough(barcode, pod, shopCode, shopName, shopAddress, sPhone, user);
                }
                result.code = CommonConstants.EXECUTE_SUCCESS;
                result.msg = "";
            }
            catch(Exception ex)
            {
                result.code = CommonConstants.EXECUTE_UNSUCCESS;
                result.msg = ex.Message;
            }
            return result;
        }
        public static List<SellThrough> GetListData(string frmDate, string toDate, string username)
        {
            List<SellThrough> list = new List<SellThrough>();
            string constr = ConfigurationManager.ConnectionStrings[CommonConstants.CONNECTION_STRING].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand("uspWeb_SellThrough_View"))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Connection = con;
                    cmd.Parameters.Add("@FRMDATE", SqlDbType.DateTime).Value = Common.convertStringWithFormatDDMMYYYYToDate(frmDate);
                    cmd.Parameters.Add("@TODATE", SqlDbType.DateTime).Value = Common.convertStringWithFormatDDMMYYYYToDate(toDate);
                    cmd.Parameters.Add("@TRANSUSER", SqlDbType.VarChar).Value = username;
                    con.Open();
                    try
                    {
                        using (SqlDataReader sdr = cmd.ExecuteReader())
                        {
                            while (sdr.Read())
                            {
                                SellThrough item = new SellThrough();
                                item.barcode = sdr["BARCODE"].ToString();
                                item.sellDate = sdr["SELLDATE"] == DBNull.Value ? "" : Convert.ToDateTime(sdr["SELLDATE"]).ToString("dd/MM/yyyy");
                                item.model = sdr["MTNAME"].ToString();
                                item.shopName = sdr["CHANNELNAME"].ToString();
                                item.plant = sdr["PLANTNAMEVN"].ToString();
                                item.transuser = sdr["TRANSUSER"].ToString();
                                item.transdate = sdr["TRANSDATE"] == DBNull.Value ? "" : Convert.ToDateTime(sdr["TRANSDATE"]).ToString("dd/MM/yyyy");
                                item.comment = sdr["COMMENT"].ToString();
                                list.Add(item);
                            }
                        }
                    }
                    catch (Exception)
                    {
                        
                    }
                }
            }
            return list;
        }
        private static string InsertDataSellThrough(string barcode, string pod, string shopCode, string shopName, string shopAddress, string sPhone, string user)
        {
            string id = "";
            string constr = ConfigurationManager.ConnectionStrings[CommonConstants.CONNECTION_STRING].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand("uspWeb_SellThrough_Insert"))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Connection = con;
                    cmd.Parameters.Add("@BARCODE", SqlDbType.VarChar).Value = barcode;
                    cmd.Parameters.Add("@POD", SqlDbType.DateTime).Value = Common.convertStringWithFormatDDMMYYYYToDate(pod);
                    if(!"".Equals(shopCode.Trim()))
                        cmd.Parameters.Add("@SHOPCODE", SqlDbType.VarChar).Value = shopCode.Trim();
                    if (!"".Equals(shopName.Trim()))
                        cmd.Parameters.Add("@SHOPNAME", SqlDbType.NVarChar).Value = shopName.Trim();
                    if (!"".Equals(shopAddress.Trim()))
                        cmd.Parameters.Add("@SHOPADDRESS", SqlDbType.NVarChar).Value = shopAddress.Trim();
                    if (!"".Equals(sPhone.Trim()))
                        cmd.Parameters.Add("@SHOPPHONE", SqlDbType.VarChar).Value = sPhone.Trim();
                    cmd.Parameters.Add("@TRANSUSER", SqlDbType.VarChar).Value = user;
                    con.Open();
                    try
                    {
                        using (SqlDataReader sdr = cmd.ExecuteReader())
                        {
                            while (sdr.Read())
                            {
                                id = sdr["RETURNID"].ToString();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        id = ex.Message;
                    }
                }
            }
            return id;
        }
    }
    public static class StoreLogics
    {
        public static List<Shop> getListShopsChild(string shopCode)
        {
            List<Shop> list = new List<Shop>();
            string constr = ConfigurationManager.ConnectionStrings[CommonConstants.CONNECTION_STRING].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand("uspWeb_SellThrough_GetListShopChild"))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Connection = con;
                    cmd.Parameters.Add("@SHOPCODE", SqlDbType.VarChar).Value = shopCode;
                    con.Open();
                    try
                    {
                        using (SqlDataReader sdr = cmd.ExecuteReader())
                        {
                            while (sdr.Read())
                            {
                                Shop item = new Shop();
                                item.shopCode = sdr["SHOPCODE"].ToString();
                                item.shopPhone = sdr["SHOPPHONE"].ToString();
                                item.shopName = sdr["SHOPNAME"].ToString();
                                item.shopAddress = sdr["SHOPADDRESS"].ToString();
                                list.Add(item);
                            }
                        }
                    }
                    catch (Exception)
                    {

                    }
                }
            }
            return list;
        }
    }
}