using EWShop.Models;
using EWShop.Utilities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace EWShop.Models
{
    public class ProductSellOutInfo
    {
        public string ewCardId { get; set; }
        public string barcode { get; set; }
        public string model { get; set; }
        public string matId { get; set; }
        public string purchaseDate { get; set; }
        public string cusName { get; set; }
        public string cusPhone { get; set; }
        public string cusEmail { get; set; }
        public string cusAddress { get; set; }
        public string cusDistrictId { get; set; }
        public string cusProvinceId { get; set; }
        public string img1 { get; set; }
        public string img2 { get; set; }
    }
    public class ProductSellOutReport
    {
        public string eWCardId { get; set; }
        public string flat { get; set; }
        public string eWCardNo { get; set; }
        public string POD { get; set; }
        public string expDate { get; set; }
        public string barcode { get; set; }
        public string model { get; set; }
        public string plant { get; set; }
        public string cusName { get; set; }
        public string cusPhone { get; set; }
        public string cusProvince { get; set; }
        public string cusDistrict { get; set; }
        public string cusAddress { get; set; }
        public string channel { get; set; }
        public string regDate { get; set; }
        public string shopCode { get; set; }
        public string shopName { get; set; }
        public string shopAddress { get; set; }
        public string shopProvince { get; set; }
        public string region { get; set; }
        public string area { get; set; }
        public string groupCode { get; set; }
        public string groupName { get; set; }
        public string status { get; set; }
        public string code { get; set; }
    }
    public class ItemFilterReport
    {
        public string proid { get; set; }
        public string frmDate { get; set; }
        public string toDate { get; set; }
        public string shop { get; set; }
        public string material { get; set; }
        public string status { get; set; }
        public string flat { get; set; }
    }
    public static class ProductSellOutLogics
    {
        private static string shopName = "";
        private static string shopAddress = "";
        #region "Register E-Warranty by SMS"
        /*--- Đăng ký BHĐT bằng SMS (cú pháp của khách hàng) ---*/
        public static ItemReturn registerEWBySMSCustomer(string cusPhone, string barcode, string pod)
        {
            ItemReturn result = new ItemReturn();
            string errMsg = checkDataSMS("",cusPhone, barcode, pod);
            if ("".Equals(errMsg))
            {
                //Đăng ký bảo hành bằng SMS
                result = regEWBySMSCustomer(cusPhone, barcode, pod);
            } else
            {
                result.code = "0";
                result.message = errMsg;
            }
            return result;
        }
        private static ItemReturn regEWBySMSCustomer(string cusPhone, string barcode, string pod)
        {
            ItemReturn result = new ItemReturn();
            string constr = ConfigurationManager.ConnectionStrings[CommonConstants.CONNECTION_STRING].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand(CommonConstants.STORED_PROCEDURE_REGISTER_EW_SMS_CUSTOMER))
                {
                    try
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add("@CUSPHONE", SqlDbType.VarChar).Value = cusPhone.Trim();
                        cmd.Parameters.Add("@BARCODE", SqlDbType.VarChar).Value = barcode.Trim();
                        cmd.Parameters.Add("@POD", SqlDbType.DateTime).Value = Common.convertStringWithFormatDDMMYYYYToDate(pod);
                        cmd.Connection = con;
                        con.Open();
                        using (SqlDataReader sdr = cmd.ExecuteReader())
                        {
                            while (sdr.Read())
                            {
                                result.code = sdr["RETURNID"].ToString();
                                result.message = sdr["RETURNMSG"].ToString();
                                if(result.code == "1")
                                {
                                    string modeltype = sdr["MODELTYPE"].ToString();
                                    string ewCardNo = sdr["EWCARDNO"].ToString();
                                    string expiredDate = Convert.ToDateTime(sdr["EXPIREDDATE"]).ToString("dd/MM/yyyy");
                                    result.message = String.Format(result.message, modeltype, barcode, ewCardNo, expiredDate);
                                }
                                result.data = sdr["ERRORMSG"].ToString();
                            }
                        }
                    }
                    catch (Exception)
                    {
                        result.code = CommonConstants.EXECUTE_UNSUCCESS;
                        result.message = "Khong the dang ky bao hanh dien tu. Vui long lien lac voi doi ngu ho tro. Xin cam on.";
                    }
                }
            }
            return result;
        }
        /*--- Đăng ký BHĐT bằng SMS (cú pháp của cửa hàng) ---*/
        public static ItemReturn registerEWBySMSShop(string shopPhone, string cusPhone, string barcode, string pod)
        {
            ItemReturn result = new ItemReturn();
            string errMsg = checkDataSMS(shopPhone, cusPhone, barcode, pod);
            if ("".Equals(errMsg))
            {
                //Đăng ký bảo hành bằng SMS
                result = regEWBySMSShop(shopPhone,cusPhone, barcode, pod);
            }
            else
            {
                result.code = "0";
                result.message = errMsg;
            }
            return result;
        }
        private static ItemReturn regEWBySMSShop(string shopPhone, string cusPhone, string barcode, string pod)
        {
            ItemReturn result = new ItemReturn();
            string constr = ConfigurationManager.ConnectionStrings[CommonConstants.CONNECTION_STRING].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand(CommonConstants.STORED_PROCEDURE_REGISTER_EW_SMS_SHOP))
                {
                    try
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add("@SHOPADDRESS", SqlDbType.NVarChar).Value = shopAddress.Trim();
                        cmd.Parameters.Add("@SHOPNAME", SqlDbType.NVarChar).Value = shopName.Trim();
                        cmd.Parameters.Add("@SHOPPHONE", SqlDbType.VarChar).Value = shopPhone.Trim();
                        cmd.Parameters.Add("@CUSPHONE", SqlDbType.VarChar).Value = cusPhone.Trim();
                        cmd.Parameters.Add("@BARCODE", SqlDbType.VarChar).Value = barcode.Trim();
                        cmd.Parameters.Add("@POD", SqlDbType.DateTime).Value = Common.convertStringWithFormatDDMMYYYYToDate(pod);
                        cmd.Connection = con;
                        con.Open();
                        using (SqlDataReader sdr = cmd.ExecuteReader())
                        {
                            while (sdr.Read())
                            {
                                result.code = sdr["RETURNID"].ToString();
                                result.message = sdr["RETURNMSG"].ToString();
                            }
                        }
                    }
                    catch (Exception)
                    {
                        result.code = CommonConstants.EXECUTE_UNSUCCESS;
                        result.message = "Không thể đăng ký bảo hành";
                    }
                }
            }
            return result;
        }
        /*--- Kiểm tra dữ liệu SMS ---*/
        private static string checkDataSMS(string shopPhone, string cusPhone, string barcode, string pod)
        {
            if (barcode == "")
            {
                return "Số máy không được rỗng";
            }
            if (cusPhone == null || "".Equals(cusPhone))
            {
                return "Số điện thoại khách hàng không được rỗng";
            }
            else if (!Common.validatePhoneNumber(cusPhone))
            {
                return "Số điện thoại khách hàng không hợp lệ";
            }
            if (!"".Equals(shopPhone))
            {
                if (!Common.checkValidatePhone(cusPhone))
                {
                    return "Số điện thoại khách hàng không hợp lệ";
                }
                if (!checkPhoneOfShop(shopPhone))
                {
                    return "Số điện thoại cửa hàng không có trong hệ thống";
                }
            }
            try
            {
                DateTime dt = Common.convertStringWithFormatDDMMYYYYToDate(pod);
            }
            catch (Exception)
            {
                return "Ngày mua không hợp lệ";
            }

            return "";
        }
        private static bool checkPhoneOfShop(string phone)
        {
            bool result = false;
            string constr = ConfigurationManager.ConnectionStrings[CommonConstants.SERVICEINFO_CONNECTION_STRING].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand(CommonConstants.STORED_PROCEDURE_CHECK_PHONE_SHOP))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add("@PHONE", SqlDbType.VarChar).Value = phone.Trim();
                    cmd.Connection = con;
                    con.Open();
                    try
                    {
                        using (SqlDataReader sdr = cmd.ExecuteReader())
                        {
                            while (sdr.Read())
                            {
                                int index = Convert.ToInt32(sdr["NUMBER"]);
                                if (index > 0)
                                {
                                    result = true;
                                    shopName = sdr["SHOPNAME"].ToString();
                                    shopAddress = sdr["ADDRESS"].ToString();
                                }
                            }
                        }
                    }
                    catch (Exception)
                    {
                        result = false;
                    }
                }
            }
            return result;
        }
        /*--- Lưu nội dung tin nhắn ---*/
        public static void SaveContentSMS(string msgContent, string phoneNumber, ItemReturn result)
        {
            string constr = ConfigurationManager.ConnectionStrings[CommonConstants.CONNECTION_STRING].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand(CommonConstants.STORED_PROCEDURE_SAVE_CONTENT_SMS))
                {
                    try
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add("@CONTENT", SqlDbType.NVarChar).Value = msgContent.Trim();
                        cmd.Parameters.Add("@PHONE", SqlDbType.VarChar).Value = phoneNumber.Trim();
                        cmd.Parameters.Add("@ISVALID", SqlDbType.Bit).Value = Convert.ToInt32(result.code);
                        cmd.Parameters.Add("@MSGREPLY", SqlDbType.NVarChar).Value = result.message.Trim();
                        cmd.Parameters.Add("@ERRORMSG", SqlDbType.NVarChar).Value = result.data;
                        cmd.Connection = con;
                        con.Open();

                        cmd.ExecuteReader();
                    }
                    catch (Exception)
                    {
                    }
                }
            }
        }
        #endregion
        #region "Register E-Warranty by Website for customer"
        public static ItemResult registerEWarrantyByWebsiteCustomer(ItemSellOut item, string source)
        {
            ItemResult result = new ItemResult();
            User us = new User();
            string checkDataStr = checkDataInput(item);
            if (checkDataStr != "")
            {
                result.code = CommonConstants.EXECUTE_UNSUCCESS;
                result.message = checkDataStr;
                result.data = "";
                return result;
            }
            result = regEWByWebCustomer(item.cusName, item.cusPhone, item.cusProvinceId, item.cusDistrictId, item.cusAddress, item.barcode, item.model, item.matId, item.purchaseDate, item.channelId, item.channelName, item.img1, item.img2, source );
            return result;
        }
        private static string checkDataInput(ItemSellOut item)
        {
            if(item.barcode.Length < 8)
            {
                return "Độ dài số máy không hợp lệ";
            }
            if (!Common.checkSpecialCharacters(item.barcode))
            {
                return "Không nhập ký tự đặc biệt trong số máy";
            }
            if (item.barcode.Length != 16 && item.barcode.Length != 20)
            {
                if (item.model == null || "".Equals(item.model))
                {
                    return "Kiểu máy không được rỗng";
                }
            }
            if (item.cusPhone == null || "".Equals(item.cusPhone))
            {
                return "Số điện thoại khách hàng không được rỗng";
            }
            else if (!Common.validatePhoneNumber(item.cusPhone))
            {
                return "Số điện thoại khách hàng không hợp lệ";
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
        private static ItemResult regEWByWebCustomer(string cusName, string cusPhone, string cusProvinceId, string cusDistrictId, string cusAddress, string barcode, string model, string matId, string purchaseDate, string channelId, string channelName, string img1, string img2, string source)
        {
            ItemResult result = new ItemResult();
            string constr = ConfigurationManager.ConnectionStrings[CommonConstants.CONNECTION_STRING].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand(CommonConstants.STORED_PROCEDURE_REGISTER_EW_WEB_CUSTOMER))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Connection = con;
                    cmd.Parameters.Add("@CUSNAME", SqlDbType.NVarChar).Value = cusName == null ? "" : cusName.Trim().ToUpper();
                    cmd.Parameters.Add("@CUSADDRESS", SqlDbType.NVarChar).Value = cusAddress == null ? "" : cusAddress.Trim().ToUpper();
                    cmd.Parameters.Add("@CUSPHONE", SqlDbType.VarChar).Value = cusPhone == null ? "" : cusPhone.Trim();
                    cmd.Parameters.Add("@BARCODE", SqlDbType.VarChar).Value = barcode == null ? "" : barcode.Trim().ToUpper();
                    if (source != null)
                        if (!"".Equals(source.Trim()))
                            cmd.Parameters.Add("@SOURCE", SqlDbType.Int).Value = Convert.ToInt32(source.Trim());
                    if (cusProvinceId != null)
                        if (!"".Equals(cusProvinceId.Trim()))
                            cmd.Parameters.Add("@CUSPROVINCEID", SqlDbType.Int).Value = Convert.ToInt32(cusProvinceId.Trim());
                    if (cusDistrictId != null)
                        if (!"".Equals(cusDistrictId.Trim()))
                            cmd.Parameters.Add("@CUSDISTRICTID", SqlDbType.Int).Value = Convert.ToInt32(cusDistrictId.Trim());
                    if (purchaseDate != null)
                        if (!"".Equals(purchaseDate.Trim()))
                            cmd.Parameters.Add("@PURCHASEDATE", SqlDbType.DateTime).Value = Common.convertStringWithFormatDDMMYYYYToDate(purchaseDate.Trim());
                    cmd.Parameters.Add("@MATID", SqlDbType.Int).Value = (matId == null || matId == "" ) ? "0" : matId;
                    cmd.Parameters.Add("@MTNAME", SqlDbType.VarChar).Value = model == null ? "" : model.Trim();
                    cmd.Parameters.Add("@SHOPID", SqlDbType.Int).Value = channelId == null ? "0" : channelId;
                    cmd.Parameters.Add("@SHOPNAME", SqlDbType.NVarChar).Value = channelName == null ? "" : channelName.Trim().ToUpper();
                    cmd.Parameters.Add("@IMAGE1", SqlDbType.NVarChar).Value = img1 == null ? "" : img1.Trim();
                    cmd.Parameters.Add("@IMAGE2", SqlDbType.NVarChar).Value = img2 == null ? "" : img2.Trim();
                    con.Open();
                    try
                    {
                        using (SqlDataReader sdr = cmd.ExecuteReader())
                        {
                            while (sdr.Read())
                            {
                                string returnId = sdr["RETURNID"].ToString();
                                result.code = returnId == CommonConstants.EXECUTE_SUCCESS ? CommonConstants.EXECUTE_SUCCESS : CommonConstants.EXECUTE_UNSUCCESS;
                                result.message = sdr["RETURNMSG"].ToString();
                                result.data = sdr["EWCARDID"].ToString();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        result.code = CommonConstants.EXECUTE_UNSUCCESS;
                        result.message = ex.Message;
                        result.data = "";
                    }
                }
            }
            if (result.code == "1")
            {
                FiboSMS.GetInfoEW(result.data.ToString());
                result.message = "Ghi nhận thành công";
            }
            return result;
        }
        #endregion
    }
}