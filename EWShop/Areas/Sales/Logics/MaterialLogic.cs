using EWShop.Areas.Sales.Models;
using EWShop.ExecuteWebserviceMWG;
using EWShop.LoginWebserviceMWG;
using EWShop.Models;
using EWShop.Utilities;
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

namespace EWShop.Areas.Sales.Logics
{
    public static class MaterialLogic
    {    
        public static List<MaterialPriceModel> getListRecords(string customer, string isModified, string isTransfered)
        {
            List<MaterialPriceModel> result = new List<MaterialPriceModel>();
            string constr = ConfigurationManager.ConnectionStrings[CommonConstants.CONNECTION_STRING].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand("uspWeb_Sales_PriceQuotation"))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Connection = con;
                    if (customer != null && !"".Equals(customer))
                        cmd.Parameters.Add("@CUSTOMER", SqlDbType.VarChar).Value = customer;
                    if (isModified != null && !"".Equals(isModified))
                        cmd.Parameters.Add("@ISCHANGED", SqlDbType.Int).Value = Convert.ToInt32(isModified);
                    if (isTransfered != null && !"".Equals(isTransfered))
                        cmd.Parameters.Add("@ISTRANSFERED", SqlDbType.Int).Value = Convert.ToInt32(isTransfered);
                    con.Open();
                    try
                    {
                        using (SqlDataReader sdr = cmd.ExecuteReader())
                        {
                            while (sdr.Read())
                            {
                                MaterialPriceModel dt = new MaterialPriceModel();
                                dt.matCode = sdr["MATCODE"].ToString();
                                dt.matName = sdr["MATNAME"].ToString();
                                dt.amount = sdr["AMOUNT"] == DBNull.Value ? "0" : String.Format("{0:n0}", Convert.ToDecimal(sdr["AMOUNT"]));
                                dt.amount_old = sdr["AMOUNT_O"] == DBNull.Value ? "0" : String.Format("{0:n0}", Convert.ToDecimal(sdr["AMOUNT_O"]));
                                dt.marketRetailPrice = sdr["MRP"] == DBNull.Value ? "0" : String.Format("{0:n0}", Convert.ToDecimal(sdr["MRP"]));
                                dt.marketRetailPrice_old = sdr["MRP_O"] == DBNull.Value ? "0" : String.Format("{0:n0}", Convert.ToDecimal(sdr["MRP_O"]));
                                dt.sampleDisplayPrice = sdr["SDP"] == DBNull.Value ? "0" : String.Format("{0:n0}", Convert.ToDecimal(sdr["SDP"]));
                                dt.sampleDisplayPrice_old = sdr["SDP_O"] == DBNull.Value ? "0" : String.Format("{0:n0}", Convert.ToDecimal(sdr["SDP_O"]));
                                dt.currency = sdr["CURRENCY"].ToString();
                                dt.beginDate = sdr["BEGINDATE"] == DBNull.Value ? "" : Convert.ToDateTime(sdr["BEGINDATE"]).ToString("dd/MM/yyyy");
                                dt.isChanged = sdr["ISCHANGED"].ToString();
                                dt.isTransfered = sdr["ISTRANSFERED"].ToString();
                                result.Add(dt);
                            }
                        }
                    }
                    catch (Exception)
                    {
                        result = new List<MaterialPriceModel>();
                    }
                }
            }
            return result;
        }
        public static List<MaterialTransferModel> loadListRecordNotTransfer(string customer)
        {
            List<MaterialTransferModel> result = new List<MaterialTransferModel>();
            string constr = ConfigurationManager.ConnectionStrings[CommonConstants.CONNECTION_STRING].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand("uspWeb_Sales_PriceQuotation"))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Connection = con;
                    if (customer != null && !"".Equals(customer))
                        cmd.Parameters.Add("@CUSTOMER", SqlDbType.VarChar).Value = customer;
                    con.Open();
                    try
                    {
                        using (SqlDataReader sdr = cmd.ExecuteReader())
                        {
                            while (sdr.Read())
                            {
                                MaterialTransferModel dt = new MaterialTransferModel();
                                dt.matCode = sdr["MATCODE"].ToString();
                                dt.matName = sdr["MATNAME"].ToString();
                                dt.amount = sdr["AMOUNT"] == DBNull.Value ? "0" : String.Format("{0:n0}", Convert.ToDecimal(sdr["AMOUNT"])) + " " + sdr["CURRENCY"].ToString();
                                dt.mrp = sdr["MRP"] == DBNull.Value ? "0" : String.Format("{0:n0}", Convert.ToDecimal(sdr["MRP"])) + " " + sdr["CURRENCY"].ToString();
                                dt.sdp = sdr["SDP"] == DBNull.Value ? "0" : String.Format("{0:n0}", Convert.ToDecimal(sdr["SDP"])) + " " + sdr["CURRENCY"].ToString();
                                dt.beginDate = sdr["BEGINDATE"] == DBNull.Value ? "" : Convert.ToDateTime(sdr["BEGINDATE"]).ToString("dd/MM/yyyy");
                                result.Add(dt);
                            }
                        }
                    }
                    catch (Exception)
                    {
                        result = new List<MaterialTransferModel>();
                    }
                }
            }
            return result;
        }
        public static MaterialModel loadDetailRecord(string customer, string matCode)
        {
            MaterialModel result = new MaterialModel();
            string constr = ConfigurationManager.ConnectionStrings[CommonConstants.CONNECTION_STRING].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand("uspWeb_Sales_MaterialPrice_GetRecordDetail"))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Connection = con;
                    if (customer != null && !"".Equals(customer))
                        cmd.Parameters.Add("@CUSTOMER", SqlDbType.VarChar).Value = customer;
                    if (matCode != null && !"".Equals(matCode))
                        cmd.Parameters.Add("@MATCODE", SqlDbType.VarChar).Value = matCode;
                    con.Open();
                    try
                    {
                        using (SqlDataReader sdr = cmd.ExecuteReader())
                        {
                            while (sdr.Read())
                            {
                                result.matCode = sdr["MATCODE"].ToString();
                                result.matName = sdr["MATNAME"].ToString();
                                result.amount = sdr["AMOUNT"] == DBNull.Value ? "0" : String.Format("{0:n0}", Convert.ToDecimal(sdr["AMOUNT"]));
                                result.marketRetailPrice = sdr["MRP"] == DBNull.Value ? "0" : String.Format("{0:n0}", Convert.ToDecimal(sdr["MRP"]));
                                result.sampleDisplayPrice = sdr["SDP"] == DBNull.Value ? "0" : String.Format("{0:n0}", Convert.ToDecimal(sdr["SDP"]));
                                result.currency = sdr["CURRENCY"].ToString();
                                result.isChanged = sdr["ISCHANGED"].ToString();
                                result.isTransfered = sdr["ISTRANSFERED"].ToString();
                            }
                        }
                    }
                    catch (Exception)
                    {
                        result = new MaterialModel();
                    }
                }
            }
            return result;
        }
        public static ResultReturn updateMRPrice(string customer, string matCode, string mrp, string sdp, string currency)
        {
            ResultReturn result = new ResultReturn();
            string constr = ConfigurationManager.ConnectionStrings[CommonConstants.CONNECTION_STRING].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand("uspWeb_Sales_MaterialPrice_Update"))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Connection = con;
                    if (customer != null && !"".Equals(customer) && !"null".Equals(customer) && !"undefined".Equals(customer))
                        cmd.Parameters.Add("@CUSTOMER", SqlDbType.VarChar).Value = customer;
                    if (matCode != null && !"".Equals(matCode) && !"null".Equals(matCode) && !"undefined".Equals(matCode))
                        cmd.Parameters.Add("@MATCODE", SqlDbType.VarChar).Value = matCode;
                    if (mrp != null && !"".Equals(mrp) && !"null".Equals(mrp) && !"undefined".Equals(mrp))
                        cmd.Parameters.Add("@MRP", SqlDbType.Money).Value = mrp.Replace(",","");
                    if (currency != null && !"".Equals(currency) && !"null".Equals(currency) && !"undefined".Equals(currency))
                        cmd.Parameters.Add("@CURRENCY", SqlDbType.VarChar).Value = currency.ToUpper();
                    if (sdp != null && !"".Equals(sdp) && !"null".Equals(sdp) && !"undefined".Equals(sdp))
                        cmd.Parameters.Add("@SDP", SqlDbType.Money).Value = sdp.Replace(",","");
                    con.Open();
                    try
                    {
                        using (SqlDataReader sdr = cmd.ExecuteReader())
                        {
                            while (sdr.Read())
                            {
                                result.code = sdr["RETURNID"].ToString();
                                result.msg = sdr["RETURNMSG"].ToString();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        result.code = CommonConstants.EXECUTE_UNSUCCESS;
                        result.msg = ex.Message;
                    }
                }
            }
            return result;
        }
        #region "Transfer MWG"
        public static ResultReturn transferToMWG(string customer, List<string> matCode)
        {
            ResultReturn result = new ResultReturn();
            result.code = CommonConstants.EXECUTE_UNSUCCESS;
            result.msg = "Cannot transfer MWG. Please check agian !!!";
            foreach (string mat in matCode)
            {
                string constr = ConfigurationManager.ConnectionStrings[CommonConstants.CONNECTION_STRING].ConnectionString;
                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand("uspWeb_Sales_MaterialPrice_GetMaterialTransfer"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.Add("@CUSTOMER", SqlDbType.VarChar).Value = customer;
                        cmd.Parameters.Add("@MATCODE", SqlDbType.VarChar).Value = mat;
                        con.Open();
                        try
                        {
                            using (SqlDataReader sdr = cmd.ExecuteReader())
                            {
                                while (sdr.Read())
                                {
                                    string model = sdr["MATNAME"].ToString();
                                    string price = sdr["AMOUNT"] == DBNull.Value ? "0" : sdr["AMOUNT"].ToString();
                                    string mrp = sdr["MRP"] == DBNull.Value ? "0" : sdr["MRP"].ToString();
                                    string sdp = sdr["SDP"] == DBNull.Value ? "0" : sdr["SDP"].ToString();
                                    string currency = sdr["CURRENCY"].ToString();
                                    string auth = Common.callLoginResult();
                                    if (mrp != "0")
                                    {
                                        ResponseMessage res = Common.executeQuotation(auth, mat, model, price, mrp, sdp, currency);
                                        updateStatus(customer, mat, res);
                                    }
                                }
                                result.code = CommonConstants.EXECUTE_SUCCESS;
                                result.msg = "Completed !!!";
                            }
                        }
                        catch (Exception ex)
                        {
                            result.code = CommonConstants.EXECUTE_UNSUCCESS;
                            result.msg = ex.Message;
                        }
                    }
                }
            }
            
            return result;
        }
        private static void updateStatus(string customer, string matCode, ResponseMessage response)
        {
            string constr = ConfigurationManager.ConnectionStrings[CommonConstants.CONNECTION_STRING].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand("uspWeb_Sales_MaterialPrice_UpdateStatus"))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Connection = con;
                    if (customer != null && !"".Equals(customer))
                        cmd.Parameters.Add("@CUSTOMER", SqlDbType.VarChar).Value = customer;
                    if (matCode != null && !"".Equals(matCode))
                        cmd.Parameters.Add("@MATCODE", SqlDbType.VarChar).Value = matCode;
                    if ("YES".Equals(response.Status))
                        cmd.Parameters.Add("@CODE", SqlDbType.Bit).Value = 1;
                    else
                        cmd.Parameters.Add("@CODE", SqlDbType.Bit).Value = 0;
                    cmd.Parameters.Add("@FEEDBACK", SqlDbType.NVarChar).Value = response.Description;
                    con.Open();
                    try
                    {
                        cmd.ExecuteReader();
                    }
                    catch (Exception)
                    {
                        
                    }
                }
            }
        }
        #endregion
        #region "Download From SAP"
        public static ResultReturn downloadMaterialPrice(string customer)
        {
            ResultReturn result = new ResultReturn();
            //result = executeDownloadFromSAP(customer);
            result = readFileExcelTemplate();
            return result;
        }
        private static ResultReturn readFileExcelTemplate()
        {
            ResultReturn result = new ResultReturn();
            DataFormatter df = new DataFormatter();
            string urlDir = HttpContext.Current.Server.MapPath("/");
            string path = urlDir + "Areas\\Sales\\Documents\\SAP.xlsx";
            if (File.Exists(path))
            {
                FileStream file = File.OpenRead(path);
                try
                {
                    XSSFWorkbook hssfwb = new XSSFWorkbook(file);
                    ISheet sheet = hssfwb.GetSheetAt(0);
                    IRow headerRow = sheet.GetRow(0);
                    int cellCount = headerRow.LastCellNum;

                    for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++)
                    {
                        IRow row = sheet.GetRow(i);
                        if (row == null) continue;
                        if (row.Cells.All(d => d.CellType == CellType.Blank)) continue;
                        string customer = "";
                        if (row.GetCell(0) != null)
                            customer = row.GetCell(0).ToString();
                        string material = "";
                        if (row.GetCell(1) != null)
                            material = row.GetCell(1).ToString();
                        string amount = "";
                        if (row.GetCell(2) != null)
                            amount = row.GetCell(2).ToString().Replace(".", "");
                        string unit = "VND";
                        if (row.GetCell(3) != null)
                            unit = row.GetCell(3).ToString();
                        string validFrm = "";
                        if (row.GetCell(4) != null)
                            validFrm = row.GetCell(4).ToString();

                        if (!"".Equals(customer) && !"".Equals(material) && !"".Equals(amount) && !"0".Equals(unit) && !"".Equals(validFrm))
                            insert1RowMaterialPrice(customer, material, amount, unit, validFrm);
                    }
                    result.code = "1";
                    result.msg = "Completed !!!";
                }
                catch (Exception ex)
                {
                    result.code = "0";
                    result.msg = ex.Message;
                }
            }else
            {
                result.code = "0";
                result.msg = "Cannot connect to get data";
            }
            return result;
        }
        private static void insert1RowMaterialPrice(string customer, string material, string amount, string unit, string validFrm)
        {
            string constr = ConfigurationManager.ConnectionStrings[CommonConstants.CONNECTION_STRING].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand("uspWeb_Sales_MaterialPrice_Insert1Row"))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Connection = con;
                    if (customer != null && !"".Equals(customer))
                        cmd.Parameters.Add("@CUSTOMER", SqlDbType.VarChar).Value = customer.Trim();
                    if (material != null && !"".Equals(material))
                        cmd.Parameters.Add("@MATCODE", SqlDbType.VarChar).Value = material.Trim();
                    if (amount != null && !"".Equals(amount))
                        cmd.Parameters.Add("@AMOUNT", SqlDbType.Money).Value = amount.Trim();
                    if (unit != null && !"".Equals(unit))
                        cmd.Parameters.Add("@CURRENCY", SqlDbType.VarChar).Value = unit.Trim();
                    if (validFrm != null && !"".Equals(validFrm))
                        cmd.Parameters.Add("@BEGINDATE", SqlDbType.DateTime).Value = Common.convertStringWithFormatDDMMYYYYToDate(validFrm.Trim());

                    con.Open();
                    try
                    {
                        cmd.ExecuteReader();
                    }
                    catch (Exception)
                    {
                        
                    }
                }
            }
        }
        #endregion
    }
}