using EWShop.Utilities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Policy;
using System.Web;
using System.Web.Mvc;

namespace EWShop.Models
{
    public class ItemSellOutExcel
    {
        public ItemSellOutExcel() : base() { }
        public ItemSellOutExcel(string id, string cusName, string cusPhone, string cusEmail, string cusAddress,
            string barcode, string model, string matid, string pod, string status)
        {
            this.id = id;
            this.cusName = cusName;
            this.cusPhone = cusPhone;
            this.cusEmail = cusEmail;
            this.cusAddress = cusAddress;
            this.barcode = barcode;
            this.model = model;
            this.matid = matid;
            this.pod = pod;
            this.status = status;
        }
        public string id { get; set; }
        public string cusName { get; set; }
        public string cusPhone { get; set; }
        public string cusEmail { get; set; }
        public string cusAddress { get; set; }
        public string barcode { get; set; }
        public string model { get; set; }
        public string matid { get; set; }
        public string pod { get; set; }
        public string img1 { get; set; }
        public string img2 { get; set; }
        public string fileId { get; set; }
        public string brandCode { get; set; }
        public string status { get; set; }
    }
    public class ItemSellOut
    {
        public string id { get; set; }
        public string ewCardNo { get; set; }
        public string barcode { get; set; }
        public string model { get; set; }
        public string matId { get; set; }
        public string plantId { get; set; }
        public string purchaseDate { get; set; }
        public string cusName { get; set; }
        public string cusAddress { get; set; }
        public string cusPhone { get; set; }
        public string cusEmail { get; set; }
        public string cusProvinceId { get; set; }
        public string cusProvinceName { get; set; }
        public string cusDistrictId { get; set; }
        public string cusDistrictName { get; set; }
        public string channelId { get; set; }
        public string channelCode { get; set; }
        public string channelName { get; set; }
        public string channelAddress { get; set; }
        public string regionName { get; set; }
        public string status { get; set; }
        public string img1 { get; set; }
        public string img2 { get; set; }
        public string msg { get; set; }
        public string captcha { get; set; }
    }
    public class BarcodeInfo
    {
        public string code { get; set; }
        public string msg { get; set; }
        public string plantId { get; set; }
        public string plantName { get; set; }
        public string matId { get; set; }
        public string model { get; set; }
    }
    public class SellOutReport
    {
        public string month { get; set; }
        public string year { get; set; }
        public string qtySellOut { get; set; }
        public string revenue { get; set; }
        public List<SellOutProductReport> detail { get; set; }
    }
    public class SellOutProductReport
    {
        public string avatar { get; set; }
        public string plantId { get; set; }
        public string plantName { get; set; }
        public string qtySellOut { get; set; }
        public string revenue { get; set; }
    }
    public class SellOutMonthly
    {
        public string plantId { get; set; }
        public string plantName { get; set; }
        public string color { get; set; }
        public List<int> data { get; set; }
        public List<string> months { get; set; }
    }
    public class DataSetChart{
        public string label { get; set; }
        public string fillColor { get; set; }
        public string strokeColor { get; set; }
        public List<int> data { get; set; }
    }
    public static class SellOutLogic
    {
        public static ResultReturn RegisterEW(ItemSellOut item,string username)
        {
            ResultReturn result = new ResultReturn();
            User us = new User();
            string checkDataStr = checkData(item);
            if (checkDataStr != "")
            {
                result.code = CommonConstants.EXECUTE_UNSUCCESS;
                result.msg = checkDataStr;
                return result;
            }
            string[] arrayBarcode = item.barcode.Split(new char[] { ';',',',' ' });
            if (arrayBarcode.Length > 0)
            {
                for (int i = 0; i < arrayBarcode.Length; i++)
                {
                    result = registerEWItem(item.cusName, item.cusAddress, item.cusPhone, item.cusEmail, arrayBarcode[i], username, item.cusProvinceId, item.cusDistrictId, item.purchaseDate, item.model , item.matId, item.img1, item.img2);
                }
            }
            return result;
        }
        private static ResultReturn registerEWItem(string cusName, string cusAddress, string cusPhone, string cusEmail, string barcode, string username, string cusProvinceId, string cusDistrictId, string purchaseDate, string model, string matId, string image1, string image2)
        {
            ResultReturn result = new ResultReturn();
            string constr = ConfigurationManager.ConnectionStrings[CommonConstants.CONNECTION_STRING].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand(CommonConstants.STORED_PROCEDURE_REGISTER_EW_PSI))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Connection = con;
                    cmd.Parameters.Add("@CUSNAME", SqlDbType.NVarChar).Value = cusName == null ? "" : cusName.Trim().ToUpper();
                    cmd.Parameters.Add("@CUSADDRESS", SqlDbType.NVarChar).Value = cusAddress == null ? "" : cusAddress.Trim().ToUpper();
                    cmd.Parameters.Add("@CUSPHONE", SqlDbType.VarChar).Value = cusPhone == null ? "" : cusPhone.Trim();
                    cmd.Parameters.Add("@CUSEMAIL", SqlDbType.VarChar).Value = cusEmail == null ? "" : cusEmail.Trim().ToUpper();
                    cmd.Parameters.Add("@BARCODE", SqlDbType.VarChar).Value = barcode == null ? "" : barcode.Trim().ToUpper();
                    cmd.Parameters.Add("@ISSUEDBY", SqlDbType.VarChar).Value = username;
                    if (cusProvinceId != null)
                        if (!"".Equals(cusProvinceId.Trim()))
                            cmd.Parameters.Add("@CUSPROVINCEID", SqlDbType.Int).Value = Convert.ToInt32(cusProvinceId.Trim());
                    if (cusDistrictId != null)
                        if (!"".Equals(cusDistrictId.Trim()))
                            cmd.Parameters.Add("@CUSDISTRICTID", SqlDbType.Int).Value = Convert.ToInt32(cusDistrictId.Trim());
                    if (purchaseDate != null)
                        if (!"".Equals(purchaseDate.Trim()))
                            cmd.Parameters.Add("@PURCHASEDATE", SqlDbType.DateTime).Value = Common.convertStringWithFormatDDMMYYYYToDate(purchaseDate.Trim());
                    cmd.Parameters.Add("@SOURCE", SqlDbType.Int).Value = 5;
                    cmd.Parameters.Add("@MATID", SqlDbType.Int).Value = (matId == null || "".Equals(matId.Trim())) ? "0" : matId;
                    cmd.Parameters.Add("@MTNAME", SqlDbType.VarChar).Value = model == null ? "" : model.Trim().ToUpper();
                    cmd.Parameters.Add("@IMAGE1", SqlDbType.NVarChar).Value = image1 == null ? "" : image1.Trim().ToUpper(); 
                    cmd.Parameters.Add("@IMAGE2", SqlDbType.NVarChar).Value = image2 == null ? "" : image2.Trim().ToUpper();
                    con.Open();
                    try
                    {
                        using (SqlDataReader sdr = cmd.ExecuteReader())
                        {
                            while (sdr.Read())
                            {
                                string returnId = sdr["RETURNID"].ToString();
                                result.code = returnId == CommonConstants.EXECUTE_SUCCESS ? CommonConstants.EXECUTE_SUCCESS : CommonConstants.EXECUTE_UNSUCCESS;
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
            if(result.code == null)
            {
                result.code = CommonConstants.EXECUTE_UNSUCCESS;
                result.msg = "Lỗi kết nối hệ thống. Vui lòng liên hệ IT Aqua để được hỗ trợ";
            }
            if(result.code == "1")
            {
                FiboSMS.GetInfoEW(result.msg);
                result.msg = "Ghi nhận thành công";
            }
            return result;
        }
        public static ProductSellOutInfo getItemEWarranty(string ewCardId, string flat)
        {
            ProductSellOutInfo result = new ProductSellOutInfo();
            string constr = ConfigurationManager.ConnectionStrings[CommonConstants.CONNECTION_STRING].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand(CommonConstants.SQL_TEXT_GET_ITEM_EWARRANTY))
                {
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.Parameters.Add("@EWCARDID", SqlDbType.Int).Value = ewCardId;
                    cmd.Parameters.Add("@FLAT", SqlDbType.Int).Value = flat;
                    cmd.Connection = con;
                    con.Open();
                    try
                    {
                        using (SqlDataReader sdr = cmd.ExecuteReader())
                        {
                            while (sdr.Read())
                            {
                                result.ewCardId = ewCardId;
                                result.barcode = sdr["BARCODE"].ToString();
                                result.matId = sdr["MATID"].ToString();
                                result.model = sdr["MODELTYPE"].ToString();
                                result.purchaseDate = sdr["PURCHASEDDATE"] == DBNull.Value ? "" : Convert.ToDateTime(sdr["PURCHASEDDATE"]).ToString("dd-MM-yyyy");

                                result.cusName = sdr["CUSNAME"].ToString();
                                result.cusPhone = sdr["CUSPHONE"].ToString();
                                result.cusAddress = sdr["CUSADDRESS"].ToString();
                                result.cusProvinceId = sdr["CUSPROVINCEID"] == DBNull.Value ? "0" : sdr["CUSPROVINCEID"].ToString();
                                result.cusDistrictId = sdr["CUSDISTRICTID"] == DBNull.Value ? "0" :  sdr["CUSDISTRICTID"].ToString();

                                result.img1 = sdr["IMG1"].ToString();
                                result.img2 = sdr["IMG2"].ToString();
                            }
                        }
                    }
                    catch (Exception)
                    {
                        result = null;
                    }
                }
            }
            return result;
        }
        public static List<ItemCompleted> listModelFilterByPlantAndModel(string model)
        {
            List<ItemCompleted> result = new List<ItemCompleted>();
            string constr = ConfigurationManager.ConnectionStrings[CommonConstants.CONNECTION_STRING].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand(CommonConstants.STORED_PROCEDURE_MODEL_FILTER))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add("@MTNAME", SqlDbType.VarChar).Value = model == null ? "" : model.Trim();
                    cmd.Connection = con;
                    con.Open();
                    try
                    {
                        using (SqlDataReader sdr = cmd.ExecuteReader())
                        {
                            while (sdr.Read())
                            {
                                ItemCompleted item = new ItemCompleted();
                                item.id = sdr["MATID"].ToString();
                                item.label = sdr["MTNAME"].ToString();
                                item.value = sdr["MTNAME"].ToString();
                                result.Add(item);
                            }
                        }
                    }
                    catch (Exception)
                    {
                        result = new List<Models.ItemCompleted>();
                    }
                }
            }
            return result;
        }
        public static List<ItemCompleted> listShopFilterByKeyword(string shop)
        {
            List<ItemCompleted> result = new List<ItemCompleted>();
            string constr = ConfigurationManager.ConnectionStrings[CommonConstants.CONNECTION_STRING].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand(CommonConstants.STORED_PROCEDURE_SHOP_FILTER))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add("@SHOPNAME", SqlDbType.VarChar).Value = shop == null ? "" : shop.Trim();
                    cmd.Connection = con;
                    con.Open();
                    try
                    {
                        using (SqlDataReader sdr = cmd.ExecuteReader())
                        {
                            while (sdr.Read())
                            {
                                ItemCompleted item = new ItemCompleted();
                                item.id = sdr["SHOPID"].ToString();
                                item.label = sdr["SHOPNAME"].ToString();
                                item.value = sdr["SHOPNAME"].ToString();
                                result.Add(item);
                            }
                        }
                    }
                    catch (Exception)
                    {
                        result = new List<Models.ItemCompleted>();
                    }
                }
            }
            return result;
        }
        public static BarcodeInfo checkBarcode(string barcode)
        {
            BarcodeInfo result = new BarcodeInfo();
            result.code = CommonConstants.EXECUTE_UNSUCCESS;
            result.msg = "";
            string constr = ConfigurationManager.ConnectionStrings[CommonConstants.CONNECTION_STRING].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand(CommonConstants.STORED_PROCEDURE_CHECK_BARCODE))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add("@BARCODE", SqlDbType.VarChar).Value = barcode == null ? "" : barcode.Trim();
                    cmd.Connection = con;
                    con.Open();
                    try
                    {
                        using (SqlDataReader sdr = cmd.ExecuteReader())
                        {
                            while (sdr.Read())
                            {
                                string flat = sdr["RETURNID"].ToString();
                                if(flat == "1")
                                {
                                    result.code = CommonConstants.EXECUTE_SUCCESS;
                                    result.msg = "";
                                    result.matId = sdr["MATID"].ToString();
                                    result.model = sdr["MTNAME"].ToString();
                                    result.plantId = sdr["PLANTID"].ToString();
                                    result.plantName = sdr["PLANTNAME"].ToString();
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        result.code = CommonConstants.EXECUTE_UNSUCCESS;
                        result.msg = ex.Message ;
                    }
                }
            }
            return result;
        }
        private static string checkData(ItemSellOut item)
        {
            string error = checkStringBarcode(item.barcode, item.model);
            if (error != "")
            {
                return error;
            }
            if(item.barcode.Length != 16 && item.barcode.Length != 20)
            {
                if(item.model == null || "".Equals(item.model))
                {
                    return "Kiểu máy không được rỗng";
                }
            }
            if (item.cusPhone == null || "".Equals(item.cusPhone))
            {
                return "Số điện thoại khách hàng không được rỗng";
            }
            else if (!Common.checkValidatePhone(item.cusPhone))
            {
                return "Số điện thoại khách hàng không hợp lệ";
            }
            if (item.cusName == null || "".Equals(item.cusName))
            {
                return "Tên khách hàng không được rỗng";
            }
            else if (!Common.validateName(item.cusName.Trim()))
            {
                return "Tên khách hàng không hợp lệ";
            }
            if (item.cusEmail != null)
            {
                if (!Common.IsValidEmail(item.cusEmail))
                {
                    return "Email khách hàng (" + item.cusEmail + ") không hợp lệ";
                }
            }

            return "";
        }
        private static string checkStringBarcode(string barcodes, string model)
        {
            string[] arrayBarcode = barcodes.Split(new char[] { ';',',',' ' });
            if (arrayBarcode.Length == 0)
            {
                return "Số máy không được rỗng";
            }
            for (int i = 0; i < arrayBarcode.Length; i++)
            {
                if (arrayBarcode[i].Trim().Length != 20 && arrayBarcode[i].Trim().Length != 16)
                {
                    if(model == null || "".Equals(model))
                        return "Vui lòng chọn kiểu máy";
                }
            }
            return "";
        }
        #region "Test"
        public static InfoReturn loadInfoFileUpload(string fileId)
        {
            InfoReturn result = new InfoReturn();
            string constr = ConfigurationManager.ConnectionStrings[CommonConstants.CONNECTION_STRING].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand("uspWeb_FileUpload_LoadInfoFileUpload10072019"))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Connection = con;
                    cmd.Parameters.Add("@FILEID", SqlDbType.Int).Value = fileId;

                    con.Open();
                    try
                    {
                        using (SqlDataReader sdr = cmd.ExecuteReader())
                        {
                            while (sdr.Read())
                            {
                                result.total = sdr["TOTAL"].ToString();
                                result.valid = sdr["VALID"].ToString();
                                result.inValid = sdr["INVALID"].ToString();
                            }
                        }
                    }
                    catch (Exception)
                    {
                        result = new InfoReturn();
                    }
                }
            }
            return result;
        }
        public static List<ItemSellOutExcel> ListDataExcel(string fileId, string flat)
        {
            List<ItemSellOutExcel> list = new List<ItemSellOutExcel>();
            string constr = ConfigurationManager.ConnectionStrings[CommonConstants.CONNECTION_STRING].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand("uspWeb_FileUpload_ViewData10072019"))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Connection = con;
                    cmd.Parameters.Add("@FILEID", SqlDbType.Int).Value = fileId;
                    if (flat != null && flat != "")
                        cmd.Parameters.Add("@STATUSID", SqlDbType.Int).Value = flat;
                    con.Open();
                    try
                    {
                        using (SqlDataReader sdr = cmd.ExecuteReader())
                        {
                            while (sdr.Read())
                            {
                                ItemSellOutExcel info = new ItemSellOutExcel();
                                info.id = sdr["ID"].ToString();
                                info.cusName = sdr["CUSNAME"].ToString();
                                info.cusPhone = sdr["CUSPHONE"].ToString();
                                info.cusEmail = sdr["CUSEMAIL"].ToString();
                                info.cusAddress = sdr["CUSADDRESS"].ToString();
                                info.barcode = sdr["BARCODE"].ToString();
                                info.model = sdr["MODELTYPE"].ToString();
                                info.matid = sdr["MATID"].ToString();
                                info.pod = sdr["POD"] == DBNull.Value ? "" : Convert.ToDateTime(sdr["POD"]).ToString("dd/MM/yyyy");
                                info.status = sdr["REMARKS"].ToString();
                                info.img1 = sdr["IMG1"].ToString();
                                info.img2 = sdr["IMG2"].ToString();
                                info.brandCode = sdr["BRANDCHCODE"].ToString();
                                list.Add(info);
                            }
                        }
                    }
                    catch (Exception)
                    {
                        list = null;
                    }
                }
            }
            return list;
        }
        public static ResultReturn RemoveItem(string fileId, string id, string username)
        {
            ResultReturn result = new ResultReturn();
            string constr = ConfigurationManager.ConnectionStrings[CommonConstants.CONNECTION_STRING].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand("uspWeb_FileUpload_RemoveItem"))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Connection = con;
                    cmd.Parameters.Add("@FILEID", SqlDbType.Int).Value = fileId;
                    cmd.Parameters.Add("@ID", SqlDbType.Int).Value = id;
                    cmd.Parameters.Add("@TRANSUSER", SqlDbType.VarChar).Value = username;

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
                        result.code = "0";
                        result.msg = ex.Message;
                    }
                }
            }
            return result;
        }
        public static ResultReturn UpdateItemUploadExcel(ItemSellOutExcel input, string username)
        {
            ResultReturn result = new ResultReturn();
            string constr = ConfigurationManager.ConnectionStrings[CommonConstants.CONNECTION_STRING].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand("uspWeb_FileUpload_Upload1Row"))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Connection = con;
                    cmd.Parameters.Add("@ID", SqlDbType.Int).Value = input.id == null ? "0" : input.id;
                    cmd.Parameters.Add("@CUSNAME", SqlDbType.NVarChar).Value = input.cusName == null ? "" : input.cusName;
                    cmd.Parameters.Add("@CUSPHONE", SqlDbType.VarChar).Value = input.cusPhone == null ? "" : input.cusPhone;
                    cmd.Parameters.Add("@CUSEMAIL", SqlDbType.VarChar).Value = input.cusEmail == null ? "" : input.cusEmail;
                    cmd.Parameters.Add("@CUSADDRESS", SqlDbType.NVarChar).Value = input.cusAddress == null ? "" : input.cusAddress;
                    cmd.Parameters.Add("@BARCODE", SqlDbType.VarChar).Value = input.barcode == null ? "" : input.barcode;
                    cmd.Parameters.Add("@MODELTYPE", SqlDbType.VarChar).Value = input.model == null ? "" : input.model;
                    if(input.pod != null && "".Equals(input.pod))
                        cmd.Parameters.Add("@PURCHASEDATE", SqlDbType.DateTime).Value = Common.convertStringWithFormatDDMMYYYYToDate(input.pod);
                    cmd.Parameters.Add("@IMG1", SqlDbType.NVarChar).Value = input.img1 == null ? "" : input.img1;
                    cmd.Parameters.Add("@IMG2", SqlDbType.NVarChar).Value = input.img2 == null ? "" : input.img2;
                    cmd.Parameters.Add("@BRANDCODE", SqlDbType.VarChar).Value = input.brandCode == null ? "" : input.brandCode;
                    cmd.Parameters.Add("@FILEID", SqlDbType.Int).Value = input.fileId == null ? "0" : input.fileId;
                    con.Open();
                    try
                    {
                        using (SqlDataReader sdr = cmd.ExecuteReader())
                        {
                            while (sdr.Read())
                            {
                                result.code = "1";
                                result.msg = "Ghi nhận hoàn tất";
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        result.code = "0";
                        result.msg = ex.Message;
                    }
                }
            }
            return result;
        }
        #endregion
        #region "New"
        public static SellOutReport GetReportSellOut(string month, string year, string username)
        {
            SellOutReport result = new SellOutReport();
            try
            {
                result.month = month;
                result.year = year;
                result.qtySellOut = "0";
                result.revenue = "0";
                var request = HttpContext.Current.Request;
                string constr = ConfigurationManager.ConnectionStrings[CommonConstants.CONNECTION_STRING].ConnectionString;
                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand("uspWeb_Report_SellOutMonthly"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        if(month != null && !"".Equals(month))
                            cmd.Parameters.Add("@MONTH", SqlDbType.Int).Value = month;
                        if (year != null && !"".Equals(year))
                            cmd.Parameters.Add("@YEAR", SqlDbType.Int).Value = year;
                        cmd.Parameters.Add("@USERNAME", SqlDbType.VarChar).Value = username.Trim();

                        con.Open();
                        using (SqlDataReader sdr = cmd.ExecuteReader())
                        {
                            while (sdr.Read())
                            {
                                result.month = sdr["MONTH"].ToString();
                                result.year = sdr["YEAR"].ToString();
                                result.revenue = sdr["REVENUE"] == DBNull.Value ? "0" : String.Format("{0:n0}", Convert.ToDecimal(sdr["REVENUE"])) + " VND";
                                result.qtySellOut = sdr["QTY"] == DBNull.Value ? "0" : String.Format("{0:n0}", Convert.ToDecimal(sdr["QTY"])) + " sản phẩm";
                                result.detail = new List<SellOutProductReport>();
                            }
                            sdr.NextResult();
                            while (sdr.Read())
                            {
                                SellOutProductReport item = new SellOutProductReport();
                                item.revenue = sdr["REVENUE"] == DBNull.Value ? "0" : String.Format("{0:n0}", Convert.ToDecimal(sdr["REVENUE"])) + " VND";
                                item.qtySellOut = sdr["QTY"] == DBNull.Value ? "0" : String.Format("{0:n0}", Convert.ToDecimal(sdr["QTY"])) + " sản phẩm";
                                item.plantId = sdr["PLANTID"].ToString();
                                item.plantName = sdr["PLANTNAMEVN"].ToString();
                                if(item.plantId == "1")
                                {
                                    item.avatar = string.Format("{0}://{1}{2}", request.Url.Scheme, request.Url.Authority, (new UrlHelper(request.RequestContext)).Content("~")) + "/Libraries/dist/img/maygiat.png";
                                }else if(item.plantId == "2")
                                {
                                    item.avatar = string.Format("{0}://{1}{2}", request.Url.Scheme, request.Url.Authority, (new UrlHelper(request.RequestContext)).Content("~")) + "/Libraries/dist/img/tulanh.png";
                                }
                                else if (item.plantId == "3")
                                {
                                    item.avatar = string.Format("{0}://{1}{2}", request.Url.Scheme, request.Url.Authority, (new UrlHelper(request.RequestContext)).Content("~")) + "/Libraries/dist/img/maylanh.png";
                                }
                                else
                                {
                                    item.avatar = string.Format("{0}://{1}{2}", request.Url.Scheme, request.Url.Authority, (new UrlHelper(request.RequestContext)).Content("~")) + "/Libraries/dist/img/giadung.png";
                                }
                                result.detail.Add(item);
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                result = null;
            }
            return result;
        }
        public static List<DataSetChart> GetReportSellOut4Month(string username)
        {
            List<SellOutMonthly> result = initData();
            try
            {
                var request = HttpContext.Current.Request;
                string constr = ConfigurationManager.ConnectionStrings[CommonConstants.CONNECTION_STRING].ConnectionString;
                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand("uspWeb_Report_SellOut4Months"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.Add("@USERNAME", SqlDbType.VarChar).Value = username.Trim();

                        con.Open();
                        using (SqlDataReader sdr = cmd.ExecuteReader())
                        {
                            while (sdr.Read())
                            {
                                string month = sdr["MONTH"].ToString();
                                string plantId = sdr["PLANTID"].ToString();
                                string plantName = sdr["PLANTNAMEVN"].ToString();
                                int qty = sdr["QTY"] == DBNull.Value ? 0 : Convert.ToInt32(sdr["QTY"]);
                                result = processData(result, month, plantId, plantName, qty);
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                result = null;
            }
            return convertData(result);
        }
        private static List<SellOutMonthly> initData()
        {
            List<SellOutMonthly> result = new List<SellOutMonthly>();
            List<string> array = new List<string>() { "#00a65a", "#00c0ef", "#f39c12", "#dd4b39" };
            for(int i = 0; i < 4; i++)
            {
                SellOutMonthly item = new SellOutMonthly();
                item.plantId = (i + 1).ToString();
                item.color = array[i];
                item.data = new List<int>() { 0, 0, 0, 0 };
                item.months = new List<string>() { DateTime.Now.AddMonths(-3).Month.ToString(), DateTime.Now.AddMonths(-2).Month.ToString(), DateTime.Now.AddMonths(-1).Month.ToString(), DateTime.Now.Month.ToString() };
                result.Add(item);
            }

            return result;
        }
        private static List<SellOutMonthly> processData(List<SellOutMonthly> result, string month, string plantId, string plantName, int qty)
        {
            for(int i = 0; i < 4; i++)
            {
                if(result[i].plantId == plantId)
                {
                    result[i].plantName = plantName;
                    for(int j = 0; j < 4; j++)
                    {
                        if(result[i].months[j] == month)
                        {
                            result[i].data[j] = qty;
                        }
                    }
                }
            }
            return result;
        }
        private static List<DataSetChart> convertData(List<SellOutMonthly> result)
        {
            if(result != null)
            {
                List<DataSetChart> list = new List<DataSetChart>();
                foreach (var item in result)
                {
                    DataSetChart record = new DataSetChart();
                    record.label = item.plantName;
                    record.fillColor = item.color;
                    record.strokeColor = item.color;
                    record.data = item.data;
                    list.Add(record);
                }
                return list;
            }
            return null;
        }
        public static List<ProductSellOutReport> GetSellOutDetail(string month, string year, string plant, string user)
        {
            List<ProductSellOutReport> list = new List<ProductSellOutReport>();
            string constr = ConfigurationManager.ConnectionStrings[CommonConstants.CONNECTION_STRING].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand("uspWeb_Report_GetSellOutDetail"))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Connection = con;
                    cmd.Parameters.Add("@USERNAME", SqlDbType.VarChar).Value = user;
                    cmd.Parameters.Add("@MONTH", SqlDbType.Int).Value = month;
                    cmd.Parameters.Add("@YEAR", SqlDbType.Int).Value = year;
                    if (plant != null && !"".Equals(plant))
                        cmd.Parameters.Add("@PLANTID", SqlDbType.Int).Value = plant;
                    con.Open();
                    try
                    {
                        using (SqlDataReader sdr = cmd.ExecuteReader())
                        {
                            while (sdr.Read())
                            {
                                ProductSellOutReport dt = new ProductSellOutReport();
                                dt.eWCardId = sdr["EWCARDID"].ToString();
                                dt.channel = sdr["EWSOURCENAME"].ToString();
                                /*--- Thông tin sản phẩm ---*/
                                dt.eWCardNo = sdr["EWCARDNO"].ToString();
                                dt.barcode = sdr["BARCODE"].ToString();
                                dt.model = sdr["MODELTYPE"].ToString();
                                dt.plant = sdr["PLANTNAME"].ToString();
                                dt.POD = sdr["PURCHASEDDATE"] == DBNull.Value ? "" : Convert.ToDateTime(sdr["PURCHASEDDATE"]).ToString("dd-MMM-yy");
                                /*--- Thông tin khách hàng ---*/
                                dt.cusName = sdr["CUSNAME"].ToString();
                                dt.cusPhone = sdr["CUSPHONE"].ToString();
                                string province = sdr["PROVINCENAME"].ToString().Trim();
                                string district = sdr["DISTRICTNAME"].ToString().Trim();
                                string address = sdr["CUSADDRESS"].ToString().Trim();
                                dt.cusAddress = ("".Equals(address) ? "" : address + "/") + ("".Equals(district) ? "" : district + "/") + ("".Equals(province) ? "" : province);
                                /*--- Thông tin cửa hàng ---*/
                                dt.shopCode = sdr["SHOPCODE"].ToString();
                                dt.shopName = sdr["SHOPNAME"].ToString();
                                dt.shopAddress = sdr["SHOPADDRESS"].ToString();
                                dt.region = sdr["RGNAME"].ToString();
                                dt.regDate = sdr["EWCARDDATE"] == DBNull.Value ? "" : Convert.ToDateTime(sdr["EWCARDDATE"]).ToString("dd-MMM-yy");
                                dt.flat = "1";
                                dt.status = "Đã ĐKBH";
                                list.Add(dt);
                            }
                        }
                    }
                    catch (Exception)
                    {
                        list = new List<ProductSellOutReport>();
                    }
                }
            }
            return list;
        }
        #endregion
    }
}