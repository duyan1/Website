using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EWShop.Models;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using EWShop.Utilities;

namespace EWShop.Areas.Admin.Logics
{
    public class SellOutLogics
    {
        public List<ProductSellOutReport> listEWHistory(string shop, string material, string frmDate, string toDate)
        {
            List<ProductSellOutReport> list = new List<ProductSellOutReport>();
            string constr = ConfigurationManager.ConnectionStrings[CommonConstants.CONNECTION_STRING].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand(CommonConstants.STORED_PROCEDURE_LIST_SELL_OUT_HISTORY))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Connection = con;
                    if (shop != null && !"".Equals(shop))
                        cmd.Parameters.Add("@CHANNELID", SqlDbType.Int).Value = shop;
                    if (material != null && !"".Equals(material))
                        cmd.Parameters.Add("@MATID", SqlDbType.Int).Value = material;
                    if (frmDate != null && !"".Equals(frmDate))
                        cmd.Parameters.Add("@FRMDATE", SqlDbType.DateTime).Value = Common.convertStringWithFormatDDMMYYYYToDate(frmDate);
                    if (toDate != null && !"".Equals(toDate))
                        cmd.Parameters.Add("@TODATE", SqlDbType.DateTime).Value = Common.convertStringWithFormatDDMMYYYYToDate(toDate);
                    con.Open();
                    try
                    {
                        using (SqlDataReader sdr = cmd.ExecuteReader())
                        {
                            while (sdr.Read())
                            {
                                ProductSellOutReport dt = new ProductSellOutReport();
                                dt.channel = sdr["EWSOURCENAME"].ToString();
                                /*--- Thông tin sản phẩm ---*/
                                dt.eWCardNo = sdr["EWCARDNO"].ToString();
                                dt.barcode = sdr["BARCODE"].ToString();
                                dt.model = sdr["MODELTYPE"].ToString();
                                dt.plant = sdr["PLANTNAME"].ToString();
                                dt.POD = sdr["PURCHASEDDATE"] == DBNull.Value ? "" : Convert.ToDateTime(sdr["PURCHASEDDATE"]).ToString("dd/MM/yyyy");
                                /*--- Thông tin khách hàng ---*/
                                dt.cusName = sdr["CUSNAME"].ToString();
                                dt.cusPhone = sdr["CUSPHONE"].ToString();
                                string province = sdr["PROVINCENAME"].ToString().Trim();
                                string district = sdr["DISTRICTNAME"].ToString().Trim();
                                string address = sdr["CUSADDRESS"].ToString().Trim();
                                dt.cusAddress = ("".Equals(address) ? "" : address) + ("".Equals(district) ? "" : "/" + district) + ("".Equals(province) ? "" : "/" + province);
                                /*--- Thông tin cửa hàng ---*/
                                dt.shopCode = sdr["SHOPCODE"].ToString();
                                dt.shopName = sdr["SHOPNAME"].ToString();
                                dt.shopAddress = sdr["SHOPADDRESS"].ToString();
                                dt.region = sdr["RGNAME"].ToString();
                                dt.regDate = sdr["EWCARDDATE"] == DBNull.Value ? "" : Convert.ToDateTime(sdr["EWCARDDATE"]).ToString("dd/MM/yyyy");
                                dt.expDate = sdr["EXPIREDDATE"] == DBNull.Value ? "" : Convert.ToDateTime(sdr["EXPIREDDATE"]).ToString("dd/MM/yyyy");
                                dt.status = sdr["CREATEDBY"].ToString();
                                dt.area = sdr["AREA"].ToString();
                                dt.groupCode = sdr["GROUPCODE"].ToString();
                                dt.groupName = sdr["GROUPNAME"].ToString();
                                dt.code = sdr["BRANDCHCODE"].ToString();
                                dt.shopProvince = sdr["SHOPPROVINCENAME"].ToString();
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
        public List<ProductSellOutReport> listEWHistoryForServiceRequest(string username, string cusPhone, string model)
        {
            List<ProductSellOutReport> list = new List<ProductSellOutReport>();
            string constr = ConfigurationManager.ConnectionStrings[CommonConstants.CONNECTION_STRING].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand(CommonConstants.STORED_PROCEDURE_SERVICE_REQUEST_LIST_EW_FOR_REG_SR))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Connection = con;
                    if (cusPhone != null && !"".Equals(cusPhone))
                        cmd.Parameters.Add("@CUSPHONE", SqlDbType.VarChar).Value = cusPhone;
                    if (model != null && !"".Equals(model) && !"0".Equals(model))
                        cmd.Parameters.Add("@MATID", SqlDbType.Int).Value = model;
                    cmd.Parameters.Add("@USERNAME", SqlDbType.VarChar).Value = username;
                    con.Open();
                    try
                    {
                        using (SqlDataReader sdr = cmd.ExecuteReader())
                        {
                            while (sdr.Read())
                            {
                                ProductSellOutReport dt = new ProductSellOutReport();
                                dt.eWCardId = sdr["EWCARDID"].ToString();
                                /*--- Thông tin sản phẩm ---*/
                                dt.eWCardNo = sdr["EWCARDNO"].ToString();
                                dt.barcode = sdr["BARCODE"].ToString();
                                dt.model = sdr["MODELTYPE"].ToString();
                                dt.plant = sdr["PLANTNAME"].ToString();
                                dt.POD = sdr["PURCHASEDDATE"] == DBNull.Value ? "" : Convert.ToDateTime(sdr["PURCHASEDDATE"]).ToString("dd/MM/yyyy");
                                /*--- Thông tin khách hàng ---*/
                                dt.cusName = sdr["CUSNAME"].ToString();
                                dt.cusPhone = sdr["CUSPHONE"].ToString();
                                string province = sdr["PROVINCENAME"].ToString().Trim();
                                string district = sdr["DISTRICTNAME"].ToString().Trim();
                                string address = sdr["CUSADDRESS"].ToString().Trim();
                                dt.cusAddress = ("".Equals(address) ? "" : address) + ("".Equals(district) ? "" : "/" + district) + ("".Equals(province) ? "" : "/" + province);
                                dt.flat = sdr["FLAT"].ToString();
                                dt.regDate = sdr["EWCARDDATE"] == DBNull.Value ? "" : Convert.ToDateTime(sdr["EWCARDDATE"]).ToString("dd/MM/yyyy");
                                dt.expDate = sdr["EXPIREDDATE"] == DBNull.Value ? "" : Convert.ToDateTime(sdr["EXPIREDDATE"]).ToString("dd/MM/yyyy");
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
        public ResultReturn SaveChangeEWItemForKeyShop(ProductSellOutInfo input, string user)
        {
            ResultReturn result = new ResultReturn();
            string error = checkDataInput(input);
            if(error != "")
            {
                result.code = "0";
                result.msg = error;
                return result;
            }
            string constr = ConfigurationManager.ConnectionStrings[CommonConstants.CONNECTION_STRING].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand(CommonConstants.STORED_PROCEDURE_SAVE_CHANGE_EWARRANTY))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Connection = con;
                    cmd.Parameters.Add("@EWCARDID", SqlDbType.Int).Value = input.ewCardId == null ? "0" : input.ewCardId.Trim();

                    cmd.Parameters.Add("@CUSNAME", SqlDbType.NVarChar).Value = input.cusName == null ? "" : input.cusName.Trim().ToUpper();
                    cmd.Parameters.Add("@CUSPHONE", SqlDbType.VarChar).Value = input.cusPhone == null ? "" : input.cusPhone.Trim();
                    if (input.cusProvinceId != null)
                        if (!"".Equals(input.cusProvinceId.Trim()))
                            cmd.Parameters.Add("@CUSPROVINCEID", SqlDbType.Int).Value = Convert.ToInt32(input.cusProvinceId.Trim());
                    if (input.cusDistrictId != null)
                        if (!"".Equals(input.cusDistrictId.Trim()))
                            cmd.Parameters.Add("@CUSDISTRICTID", SqlDbType.Int).Value = Convert.ToInt32(input.cusDistrictId.Trim());
                    cmd.Parameters.Add("@CUSADDRESS", SqlDbType.NVarChar).Value = input.cusAddress == null ? "" : input.cusAddress.Trim().ToUpper();
                    
                    cmd.Parameters.Add("@BARCODE", SqlDbType.VarChar).Value = input.barcode == null ? "" : input.barcode.Trim().ToUpper();
                    if (input.matId != null)
                        if (!"".Equals(input.matId.Trim()))
                            cmd.Parameters.Add("@MATID", SqlDbType.Int).Value = Convert.ToInt32(input.matId.Trim());
                    cmd.Parameters.Add("@MODEL", SqlDbType.VarChar).Value = input.model == null ? "" : input.model.Trim().ToUpper();
                    if (input.purchaseDate != null)
                        if (!"".Equals(input.purchaseDate.Trim()))
                            cmd.Parameters.Add("@POD", SqlDbType.DateTime).Value = Common.convertStringWithFormatDDMMYYYYToDate(input.purchaseDate.Trim());
                    cmd.Parameters.Add("@IMG1", SqlDbType.NVarChar).Value = input.img1 == null ? "" : input.img1.Trim().ToLower();
                    cmd.Parameters.Add("@IMG2", SqlDbType.NVarChar).Value = input.img2 == null ? "" : input.img2.Trim().ToLower();
                    cmd.Parameters.Add("@USERNAME", SqlDbType.VarChar).Value = user;
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
            return result;
        }
        private string checkDataInput(ProductSellOutInfo input)
        {
            if (input.barcode == null || "".Equals(input.barcode))
                return "Vui lòng nhập thông tin số máy";
            if (!Common.checkSpecialCharacters(input.barcode))
                return "Số máy không hợp lệ (Vui lòng không nhập các ký tự đặc biệt)";
            if (input.barcode.Length < 8)
                return "Độ dài số máy không hợp lệ";
            if (input.model == null || "".Equals(input.model))
                return "Vui lòng nhập thông tin kiểu máy";
            if (input.purchaseDate == null || "".Equals(input.purchaseDate))
                return "Vui lòng nhập ngày mua";
            if (input.cusName == null || "".Equals(input.cusName))
                return "Vui lòng nhập thông tin tên khách hàng";
            if (input.cusPhone == null || "".Equals(input.cusPhone))
                return "Vui lòng nhập số điện thoại khách hàng";
            if (!Common.checkSpecialCharacters(input.cusPhone))
                return "Số điện thoại không hợp lệ (Vui lòng không nhập các ký tự đặc biệt)";
            if (!Common.checkValidatePhone(input.cusPhone))
                return "Số điện thoại khách hàng không hợp lệ";
            return "";
        }
        public List<ProductSellOutReport> listEWHistoryForShop(string user, string status, string frmDate, string toDate)
        {
            List<ProductSellOutReport> list = new List<ProductSellOutReport>();
            string constr = ConfigurationManager.ConnectionStrings[CommonConstants.CONNECTION_STRING].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand(CommonConstants.STORED_PROCEDURE_LIST_SELL_OUT_HISTORY_FOR_KEYSHOP))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Connection = con;
                    cmd.Parameters.Add("@USERNAME", SqlDbType.VarChar).Value = user;
                    if (status != null && !"".Equals(status))
                        cmd.Parameters.Add("@STATUS", SqlDbType.Int).Value = status;
                    if (frmDate != null && !"".Equals(frmDate))
                        cmd.Parameters.Add("@FRMDATE", SqlDbType.DateTime).Value = Common.convertStringWithFormatDDMMYYYYToDate(frmDate);
                    if (toDate != null && !"".Equals(toDate))
                        cmd.Parameters.Add("@TODATE", SqlDbType.DateTime).Value = Common.convertStringWithFormatDDMMYYYYToDate(toDate);
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
                                dt.flat = sdr["FLAT"] == DBNull.Value ? "0" : Convert.ToInt32(sdr["FLAT"]).ToString();
                                dt.status = sdr["STATUS"].ToString();
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
        public ItemResult getInfoBarcode(string barcode, string model, string matid)
        {
            ItemResult result = new ItemResult();
            if(barcode == null || barcode == "")
            {
                result.code = "2";
                result.message = "Vui lòng nhập số máy để tìm kiếm";
                return result;
            }
            string constr = ConfigurationManager.ConnectionStrings[CommonConstants.CONNECTION_STRING].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand(CommonConstants.STORED_PROCEDURE_GET_INFO_STATUS_BARCODE))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Connection = con;
                    cmd.Parameters.Add("@BARCODE", SqlDbType.VarChar).Value = barcode;
                    if (model != null && !"".Equals(model))
                        cmd.Parameters.Add("@MODEL", SqlDbType.VarChar).Value = model;
                    if (matid != null && !"".Equals(matid))
                        cmd.Parameters.Add("@MATID", SqlDbType.Int).Value = Convert.ToInt32(matid);
                    con.Open();
                    try
                    {
                        using (SqlDataReader sdr = cmd.ExecuteReader())
                        {
                            while (sdr.Read())
                            {
                                result.code = sdr["RETURNID"].ToString();
                                result.message = sdr["RETURNMSG"].ToString();
                                if(result.code == "1")
                                {
                                    ItemSellOut item = new ItemSellOut();
                                    /*--- Customer ---*/
                                    item.cusName = sdr["CUSNAME"].ToString();
                                    item.cusPhone = sdr["CUSPHONE"].ToString();
                                    item.cusAddress = sdr["CUSADDRESS"].ToString();
                                    /*--- Shop ---*/
                                    item.channelCode = sdr["SHOPCODE"].ToString();
                                    item.channelName = sdr["SHOPNAME"].ToString();
                                    item.channelAddress = sdr["SHOPADDRESS"].ToString();
                                    /*--- Production ---*/
                                    item.model = sdr["MODELTYPE"].ToString();
                                    item.purchaseDate = sdr["PURCHASEDATE"] == DBNull.Value ? "" : Convert.ToDateTime(sdr["PURCHASEDATE"]).ToString("dd/MMM/yyyy");
                                    item.status = sdr["EXPIREDDATE"] == DBNull.Value ? "" : Convert.ToDateTime(sdr["EXPIREDDATE"]).ToString("dd/MMM/yyyy");
                                    result.data = item;
                                }
                                else if(result.code == "0")
                                {
                                    result.data = sdr["PURCHASEDATE"] == DBNull.Value ? "" : Convert.ToDateTime(sdr["PURCHASEDATE"]).ToString("dd/MMM/yyyy");
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        result.code = "2";
                        result.message = ex.Message;
                    }
                }
            }
            return result;
        }
    }
}