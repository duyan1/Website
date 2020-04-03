using EWShop.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Xml;

namespace EWShop.Models
{
    public class CheckBarcode
    {
        public string flat { get; set; }
        public string serial { get; set; }
        public string phone { get; set; }
        public string model { get; set; }
        public string matid { get; set; }
        public string cardNo { get; set; }
        public string clientCaptcha { get; set; }
    }
    public class RegisterEWInfo
    {
        public string code { get; set; }
        public string msg { get; set; }
        public string ewCode { get; set; }
        public string cusName { get; set; }
        public string cusPhone { get; set; }
        public string cusAddress { get; set; }
        public string barcode { get; set; }
        public string modeltype { get; set; }
        public string purchaseDate { get; set; }
        public string expiredDate { get; set; }
        public string shopName { get; set; }
        public string shopAddress { get; set; }
        public string remainTime { get; set; }
    }
    public class ServiceRequestInfo
    {
        public string CustomerName { get; set; }
        public string CustomerPhone { get; set; }
        public string CustomerDetailAddress { get; set; }
        public string Brand { get; set; }
        public string Category { get; set; }
        public string ProductModel { get; set; }
        public string serialNumber { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public List<Repair> listOfRepair { get; set; }
    }
    public class ServiceRequestInfo_New
    {
        public string CustomerName { get; set; }
        public string CustomerPhone { get; set; }
        public string CustomerDetailAddress { get; set; }
        public string Brand { get; set; }
        public string Category { get; set; }
        public string ProductModel { get; set; }
        public string serialNumber { get; set; }
        public string SRNum { get; set; }
        public string L3 { get; set; }
        public string Status { get; set; }
        public string ServiceClass { get; set; }
        public string PartNumber { get; set; }
        public string OrderDate { get; set; }
        public string ShipDate { get; set; }
        public string ReceiveDate { get; set; }
    }
    public class Repair
    {
        public string ChangedNo { get; set; }
        public string ChangedStatus { get; set; }
        public string StatusChangeDate { get; set; }
    }
    public class ServiceRequest
    {
        public string SRId { get; set; }
        public string cusName { get; set; }
        public string cusProvince { get; set; }
        public string cusDistrict { get; set; }
        public string cusWard { get; set; }
        public string cusAddress { get; set; }
        public string cusPhone { get; set; }
        public string cusOtherPhone { get; set; }
        public string cusEmail { get; set; }
        public string plant { get; set; }
        public string model { get; set; }
        public string barcode { get; set; }
        public string contentError { get; set; }
        public string tokenId { get; set; }
        public string brand { get; set; }
        public string description { get; set; }
        public string purchaseDate { get; set; }
        public string expiredDate { get; set; }
        public string requestCode { get; set; }
        public string shopName { get; set; }
        public string shopContact { get; set; }
        public string shopPhone { get; set; }
        public string shopEmail { get; set; }
        public string sourceCode { get; set; }
        public string sourceType { get; set; }
        public string captcha { get; set; }
    }
    public static class EWarranty
    {
        public static RegisterEWInfo CheckEWarranty(string phone, string barcode, string model, string cardNo, string flat)
        {
            RegisterEWInfo result = new RegisterEWInfo();
            string constr = ConfigurationManager.ConnectionStrings[CommonConstants.CONNECTION_STRING].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand(CommonConstants.STORED_PROCEDURE_CHECK_STATUS_EW))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Connection = con;
                    cmd.Parameters.Add("@PHONE", SqlDbType.VarChar).Value = phone;
                    cmd.Parameters.Add("@BARCODE", SqlDbType.VarChar).Value = barcode;
                    cmd.Parameters.Add("@MODEL", SqlDbType.VarChar).Value = model;
                    cmd.Parameters.Add("@CARDNO", SqlDbType.VarChar).Value = cardNo;
                    cmd.Parameters.Add("@FLAT", SqlDbType.Int).Value = flat;
                    con.Open();
                    try
                    {
                        using (SqlDataReader sdr = cmd.ExecuteReader())
                        {
                            while (sdr.Read())
                            {
                                result.code = sdr["RETURNID"].ToString();
                                result.msg = sdr["RETURNMSG"].ToString();
                                result.cusName = sdr["CUSNAME"].ToString();
                                result.cusPhone = sdr["CUSPHONE"].ToString();
                                result.barcode = sdr["BARCODE"].ToString();
                                result.modeltype = sdr["MODELTYPE"].ToString();
                                result.ewCode = sdr["EWCARDNO"].ToString();
                                result.purchaseDate = sdr["PURCHASEDDATE"] == DBNull.Value ? "" : Convert.ToDateTime(sdr["PURCHASEDDATE"]).ToString("dd/MM/yyyy");
                                DateTime dt = sdr["EXPIREDDATE"] == DBNull.Value ? (DateTime.Now) : Convert.ToDateTime(sdr["EXPIREDDATE"]);
                                if(dt != DateTime.Now)
                                {
                                    result.expiredDate = dt.ToString("dd/MM/yyyy");
                                }
                                result.shopName = sdr["SHOPNAME"].ToString();
                                result.shopAddress = sdr["SHOPADDRESS"].ToString();
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
    }
    public static class ServiceRequestLogics
    {
        public static ResultReturn RegisterServiceRequestForCustomer(ServiceRequest sr)
        {
            ResultReturn result = new ResultReturn();
            string errStr = checkData(sr.cusName, sr.cusPhone, sr.cusOtherPhone, sr.cusAddress, sr.contentError);
            if (!"".Equals(errStr))
            {
                result.code = "0";
                result.msg = errStr;
                return result;
            }
            string constr = ConfigurationManager.ConnectionStrings[CommonConstants.CONNECTION_STRING].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand(CommonConstants.STORED_PROCEDURE_SERVICE_REQUEST_FOR_CUSTOMER))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Connection = con;
                    cmd.Parameters.Add("@BARCODE", SqlDbType.VarChar).Value = sr.barcode.ToUpper();
                    cmd.Parameters.Add("@CUSADDRESS", SqlDbType.NVarChar).Value = sr.cusAddress;
                    cmd.Parameters.Add("@CUSDISTRICTID", SqlDbType.Int).Value = sr.cusDistrict;
                    cmd.Parameters.Add("@CUSEMAIL", SqlDbType.VarChar).Value = sr.cusEmail;
                    cmd.Parameters.Add("@CUSNAME", SqlDbType.NVarChar).Value = sr.cusName;
                    cmd.Parameters.Add("@CUSOTHERPHONE", SqlDbType.VarChar).Value = sr.cusOtherPhone;
                    cmd.Parameters.Add("@CUSPHONE", SqlDbType.VarChar).Value = sr.cusPhone;
                    cmd.Parameters.Add("@CUSPROVINCEID", SqlDbType.Int).Value = sr.cusProvince;
                    cmd.Parameters.Add("@MATID", SqlDbType.Int).Value = sr.model;
                    cmd.Parameters.Add("@PLANTID", SqlDbType.Int).Value = sr.plant;
                    cmd.Parameters.Add("@CONTENT", SqlDbType.NVarChar).Value = sr.contentError.Trim();
                    con.Open();
                    try
                    {
                        using (SqlDataReader sdr = cmd.ExecuteReader())
                        {
                            while (sdr.Read())
                            {
                                result.code = sdr["RETURNID"].ToString();
                                result.msg = sdr["RETURNMSG"].ToString();
                                sr.plant = sdr["PLANTNAME"].ToString();
                                sr.model = sdr["MODELTYPE"].ToString();
                                sr.cusProvince = sdr["PROVINCE"].ToString();
                                sr.cusDistrict = sdr["DISTRICT"].ToString();
                                sr.SRId = sdr["SRID"].ToString();
                                sr.brand = sdr["BRAND"].ToString();
                                sr.tokenId = sdr["TOKENID"].ToString();
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
            if(result.code == "1")
            {
                //string receiver = sr.cusEmail.Trim() == "" ? "cskh@aquavietnam.vn" : sr.cusEmail.Trim();
                string receiver = ("".Equals(sr.cusEmail) || sr.cusEmail == null) ? "cskh@aquavietnam.vn" : sr.cusEmail.Trim();
                sendEmailResult("1", "[Website] Yeu cau bao hanh/bao tri", sr, receiver);

                result.msg = "Xin chào " + sr.cusName + "<br/>Yêu cầu của quý khách đã được tiếp nhận. <br/>";
                result.msg += "Mã yêu cầu: <b>" + sr.tokenId + "</b><br/>";
                result.msg += "Quý khách vui lòng lưu giữ Mã yêu cầu trên để thuận tiện cho việc theo dõi quá trình Bảo hành/Sản phẩm. Chúng tôi sẽ liên hệ với quý khách trong thời gian sớm nhất.<br/>";
                result.msg += "Xin chân thành cảm ơn!";

                ItemReturn item = TransferSRToGSIS(GetItem(sr.SRId));
                UpdateStatusSRTransfer(item, sr.SRId);
            }
            return result;
        }
        public static ResultReturn RegisterServiceRequestForShop(ServiceRequest sr, string username)
        {
            ResultReturn result = new ResultReturn();
            string errStr = checkData(sr.cusName, sr.cusPhone, sr.cusOtherPhone, sr.cusAddress, sr.contentError);
            if (!"".Equals(errStr))
            {
                result.code = "0";
                result.msg = errStr;
                return result;
            }
            string constr = ConfigurationManager.ConnectionStrings[CommonConstants.CONNECTION_STRING].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand(CommonConstants.STORED_PROCEDURE_SERVICE_REQUEST_FOR_SHOP))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Connection = con;
                    /*--- Thông tin khách hàng ---*/
                    cmd.Parameters.Add("@CUSNAME", SqlDbType.NVarChar).Value = sr.cusName == null ? "" : sr.cusName;
                    cmd.Parameters.Add("@CUSPROVINCEID", SqlDbType.Int).Value = sr.cusProvince == null ? "0" : sr.cusProvince;
                    cmd.Parameters.Add("@CUSDISTRICTID", SqlDbType.Int).Value = sr.cusDistrict == null ? "0" : sr.cusDistrict;
                    cmd.Parameters.Add("@CUSWARDID", SqlDbType.VarChar).Value = sr.cusWard == null ? "" : sr.cusWard;
                    cmd.Parameters.Add("@CUSADDRESS", SqlDbType.NVarChar).Value = sr.cusAddress == null ? "" : sr.cusAddress;
                    cmd.Parameters.Add("@CUSPHONE", SqlDbType.VarChar).Value = sr.cusPhone == null ? "" : sr.cusPhone;
                    cmd.Parameters.Add("@CUSOTHERPHONE", SqlDbType.VarChar).Value = sr.cusOtherPhone == null ? "" : sr.cusOtherPhone;

                    /*--- Thông tin sản phẩm ---*/
                    cmd.Parameters.Add("@PLANTID", SqlDbType.Int).Value = sr.plant == null ? "0" : sr.plant;
                    cmd.Parameters.Add("@MATID", SqlDbType.Int).Value = sr.model == null ? "0" : sr.model;
                    cmd.Parameters.Add("@BARCODE", SqlDbType.VarChar).Value = sr.barcode == null ? "" : sr.barcode.ToUpper();
                    if (sr.purchaseDate != null && !"".Equals(sr.purchaseDate))
                        cmd.Parameters.Add("@PURCHASEDDATE", SqlDbType.DateTime).Value = Common.convertStringWithFormatDDMMYYYYToDate(sr.purchaseDate);
                    if (sr.expiredDate != null && !"".Equals(sr.expiredDate))
                        cmd.Parameters.Add("@EXPIREDDATE", SqlDbType.DateTime).Value = Common.convertStringWithFormatDDMMYYYYToDate(sr.expiredDate);
                    cmd.Parameters.Add("@ERRPRODUCT", SqlDbType.NVarChar).Value = sr.contentError == null ? "" : sr.contentError.Trim();
                    cmd.Parameters.Add("@DESCRIPTION", SqlDbType.NVarChar).Value = sr.description == null ? "" : sr.description.Trim();

                    /*--- Thông tin cửa hàng ---*/
                    cmd.Parameters.Add("@SHOPCONTACT", SqlDbType.NVarChar).Value = sr.shopContact == null ? "" : sr.shopContact;
                    cmd.Parameters.Add("@SHOPPHONE", SqlDbType.VarChar).Value = sr.shopPhone == null ? "" : sr.shopPhone;
                    cmd.Parameters.Add("@SHOPEMAIL", SqlDbType.NVarChar).Value = sr.shopEmail == null ? "" : sr.shopEmail;

                    /*--- Thông tin cửa hàng ---*/
                    cmd.Parameters.Add("@CREATEDBY", SqlDbType.VarChar).Value = username;
                    con.Open();
                    try
                    {
                        using (SqlDataReader sdr = cmd.ExecuteReader())
                        {
                            while (sdr.Read())
                            {
                                result.code = sdr["RETURNID"].ToString();
                                result.msg = sdr["RETURNMSG"].ToString();
                                sr.plant = sdr["PLANTNAME"].ToString();
                                sr.model = sdr["MODELTYPE"].ToString();
                                sr.cusProvince = sdr["PROVINCE"].ToString();
                                sr.cusDistrict = sdr["DISTRICT"].ToString();
                                sr.SRId = sdr["SRID"].ToString();
                                sr.brand = sdr["BRAND"].ToString();
                                sr.tokenId = sdr["TOKENID"].ToString();
                                sr.shopContact = sdr["SHOPCONTACT"].ToString();
                                sr.shopName = sdr["SHOPNAME"].ToString();
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
            if (result.code == "1")
            {
                string receiver = ("".Equals(sr.shopEmail) || sr.shopEmail == null) ? "cskh@aquavietnam.vn" : sr.shopEmail.Trim();
                sendEmailResult("0", "[ShopWeb] Yeu cau sua chua tu CH/STDM(So phieu: " + sr.SRId + ")", sr, receiver);

                result.msg = "Đã đăng ký thành công. Số phiếu ĐK: " + sr.SRId + " - Mã yêu cầu: " + sr.tokenId;

                ItemReturn item = TransferSRToGSIS(GetItem(sr.SRId));
                UpdateStatusSRTransfer(item, sr.SRId);
            }
            return result;
        }
        private static void UpdateStatusSRTransfer(ItemReturn item, string sRId)
        {
            string constr = ConfigurationManager.ConnectionStrings[CommonConstants.CONNECTION_STRING].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand(CommonConstants.STORED_PROCEDURE_SERVICE_REQUEST_UPDATE_STATUS))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Connection = con;
                    cmd.Parameters.Add("@SRID", SqlDbType.Int).Value = sRId;
                    if(item.data != null && !"".Equals(item.data))
                        cmd.Parameters.Add("@SRNUM", SqlDbType.VarChar).Value = item.data;
                    cmd.Parameters.Add("@ERRCODE", SqlDbType.VarChar).Value = item.code;
                    cmd.Parameters.Add("@ERRMSG", SqlDbType.NVarChar).Value = item.message;

                    con.Open();
                    try
                    {
                        cmd.ExecuteReader();
                    }
                    catch (Exception) { }
                }
            }
        }
        private static ItemReturn TransferSRToGSIS(ServiceRequest param)
        {
            ItemReturn result = new ItemReturn();
            if( param == null)
            {
                result.code = "1";
                result.message = "Cannot get info of service request";
                return result;
            }
            try
            {
                TransferGSIS.SPI_spcAEV_spcCCM_spcInterface_spcServiceClient client = new TransferGSIS.SPI_spcAEV_spcCCM_spcInterface_spcServiceClient();

                TransferGSIS.ServiceRequest request = new TransferGSIS.ServiceRequest();
                request.FirstName = param.cusName == null ? "" : param.cusName;
                request.LastName = param.cusName == null ? "" : param.cusName;
                request.Brand = (param.brand == null || "".Equals(param.brand)) ? "AQUA" : param.brand;
                request.Category = param.plant == null ? "" : param.plant;
                request.Country = "Vietnam";
                request.Description = param.description == null ? "" : param.description;
                request.DetailAddress = param.cusAddress == null ? "" : param.cusAddress;
                request.EmailAddress = param.cusEmail == null ? "" : param.cusEmail;
                request.HomePhone = param.cusPhone == null ? "" : param.cusPhone;
                request.MobilePhone = param.cusOtherPhone == null ? "" : param.cusOtherPhone;
                request.ProductModel = param.model == null ? "" : param.model;
                request.PurchaseDate = param.purchaseDate == null ? "" : param.purchaseDate;
                request.RequestCode = param.requestCode == null ? "" : param.requestCode;
                request.SerialNum = param.barcode == null ? "" : param.barcode;
                request.ShopContact = param.shopContact == null ? "" : param.shopContact;
                request.AddressId = param.cusWard == null ? "" : param.cusWard;
                request.SourceCode = param.sourceCode == null ? "" : param.sourceCode;
                request.SourceType = param.sourceType == null ? "" : param.sourceType;

                TransferGSIS.Request_Input requestInput = new TransferGSIS.Request_Input();
                requestInput.ServiceRequest = request;

                TransferGSIS.Request_Output requestOutput = client.Request(requestInput);
                result.code = requestOutput.Error_spcCode;
                result.message = requestOutput.Error_spcMessage;
                if (result.code == "0")
                {
                    result.data = requestOutput.SRNum;
                }
            }
            catch(Exception ex)
            {
                result.code = "1";
                result.message = ex.Message;
            }
            return result;
        }
        private static ServiceRequest GetItem(string SRId)
        {
            ServiceRequest sr = new ServiceRequest();
            string constr = ConfigurationManager.ConnectionStrings[CommonConstants.CONNECTION_STRING].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand(CommonConstants.STORED_PROCEDURE_SERVICE_REQUEST_GET_ITEM))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Connection = con;
                    cmd.Parameters.Add("@SRID", SqlDbType.Int).Value = SRId;
                    
                    con.Open();
                    try
                    {
                        using (SqlDataReader sdr = cmd.ExecuteReader())
                        {
                            while (sdr.Read())
                            {
                                sr.SRId = SRId;
                                sr.cusName = sdr["FIRSTNAME"] == DBNull.Value ? "" : sdr["FIRSTNAME"].ToString();
                                sr.brand = sdr["BRAND"] == DBNull.Value ? "AQUA" : sdr["BRAND"].ToString();
                                sr.plant = sdr["CATEROGY"] == DBNull.Value ? "" : sdr["CATEROGY"].ToString();
                                sr.description = sdr["DESCRIPTION"] == DBNull.Value ? "" : sdr["DESCRIPTION"].ToString();
                                sr.cusAddress = sdr["CUSADDRESS"] == DBNull.Value ? "" : sdr["CUSADDRESS"].ToString();
                                sr.cusEmail = sdr["CUSEMAIL"] == DBNull.Value ? "" : sdr["CUSEMAIL"].ToString();
                                sr.cusPhone = sdr["CUSPHONE"] == DBNull.Value ? "" : sdr["CUSPHONE"].ToString();
                                sr.cusOtherPhone = sdr["CUSMOBILE"] == DBNull.Value ? "" : sdr["CUSMOBILE"].ToString();
                                sr.model = sdr["MODELTYPE"] == DBNull.Value ? "" : sdr["MODELTYPE"].ToString();
                                sr.purchaseDate = sdr["PURCHASEDATE"] == DBNull.Value ? "" : Convert.ToDateTime(sdr["PURCHASEDATE"]).ToString("MM/dd/yyyy");
                                sr.requestCode = sdr["REQCODE"] == DBNull.Value ? "" : sdr["REQCODE"].ToString();
                                sr.barcode = sdr["BARCODE"] == DBNull.Value ? "" : sdr["BARCODE"].ToString();
                                sr.shopContact = sdr["SHOPCONTACT"] == DBNull.Value ? "" : sdr["SHOPCONTACT"].ToString();
                                sr.cusWard = sdr["ADDRESSID"] == DBNull.Value ? "" : sdr["ADDRESSID"].ToString();
                                sr.sourceCode = sdr["SOURCECODE"] == DBNull.Value ? "" : sdr["SOURCECODE"].ToString();
                                sr.sourceType = sdr["SOURCETYPE"] == DBNull.Value ? "" : sdr["SOURCETYPE"].ToString();
                            }
                        }
                    }
                    catch (Exception)
                    {
                        sr = null;
                    }
                }
            }
            return sr;
        }
        private static void sendEmailResult(string flat, string subject, ServiceRequest sr, string receiver)
        {
            string body = "";
            if (flat == "1")
            {
                body += "Xin chào. <br/> Thông tin tiếp nhận bảo hành/bảo trì sản phẩm từ <b>Khách Hàng</b> với nội dung như sau: <br/>";
                body += "<table border='1'><tr><td>Số phiếu ĐK </td><td style='color:Blue'><b>" + sr.SRId + "</b></td><td>Ngày yêu cầu</td><td><b>" + DateTime.Now.ToString("dd-MM-yyyy") + "</b></td></tr>";
                body += "<tr><td style='color:Blue'>Mã yêu cầu</td><td style='color:Blue'><b>" + sr.tokenId + "</b></td><td>Nơi đăng ký</td><td><b>Khách Hàng</b></td></tr>";
                body += "<tr><td>Loại sản phẩm</td><td><b>" + sr.plant + "</b></td><td>Kiểu máy, Số máy</td><td><b>" + sr.model + " , " + sr.barcode + "</b></td></tr>";
                body += "<tr><td>Tên khách hàng</td><td style='color:Blue'><b>" + sr.cusName + "</b></td><td>Điện thoại</td><td style='color:Blue'><b>" + sr.cusPhone + "</b></td></tr>";
                body += "<tr><td>Địa chỉ</td><td colspan='3' style='color:Blue'><b>" + sr.cusAddress + "</b></td></tr>";
                body += "<tr><td>Tỉnh/Thành phố</td><td><b>" + sr.cusProvince + "</b></td><td>Quận/Huyện</td><td><b>" + sr.cusDistrict + "</b></td></tr>";
                body += "<tr><td>Ý kiến khách hàng</td><td colspan='3' style='color:Blue'><b>" + sr.contentError + "</b></td></tr></table><br/>Xin chân thành cảm ơn! ";
            }else
            {
                body += "Xin chào! <br/> Thông tin tiếp nhận bảo hành/bảo trì sản phẩm từ <b>Cửa Hàng</b> với nội dung như sau: <br/>";
                body += "<table border='1'><tr><td style='color:Blue'>Số phiếu ĐK</td><td style='color:Blue'><b>" + sr.SRId + "</b></td><td>Ngày yêu cầu</td><td><b>" + DateTime.Now.ToString("dd-MM-yyyy") + "</b></td></tr>";
                body += "<tr><td style='color:Blue'>Mã yêu cầu</td><td style='color:Blue'><b>" + sr.tokenId + "</b></td><td>Nơi đăng ký</td><td><b>Cửa Hàng</b></td></tr>";
                body += "<tr><td>Cửa hàng</td><td style='color:#1f497d'><b>" + sr.shopName + "</b></td><td>Người liên hệ</td><td style='color:#1f497d'><b>" + "" + " (" + sr.shopContact + ")</b></td></tr>";
                body += "<tr><td>Loại sản phẩm</td><td><b>" + sr.plant + "</b></td><td>Kiểu máy, Số máy</td><td><b>" + sr.model + " , " + sr.barcode + "</b></td></tr>";
                body += "<tr><td>Tên khách hàng</td><td style='color:Blue'><b>" + sr.cusName + "</b></td><td>Điện thoại</td><td style='color:Blue'><b>" + sr.cusPhone + "</b></td></tr>";
                body += "<tr><td>Địa chỉ</td><td colspan='3' style='color:Blue'><b>" + sr.cusAddress + "</b></td></tr>";
                body += "<tr><td>Tỉnh/Thành phố</td><td><b>" + sr.cusProvince + "</b></td><td>Quận/Huyện</td><td><b>" + sr.cusDistrict + "</b></td></tr>";
                body += "<tr><td>Ý kiến khách hàng</td><td colspan='3' style='color:Blue'><b>" + sr.contentError + "</b></td></tr></table><br/>Xin chân thành cảm ơn!";
            }
            Common.SendEmail(receiver, subject, body);
        }
        private static string checkData(string name, string phone, string otherPhone, string address, string errStr)
        {
            if(name == null || "".Equals(name))
                return "Vui lòng nhập thông tin tên khách hàng.";
            if (phone == null || "".Equals(phone))
                return "Vui lòng nhập thông tin số điện thoại liên lạc";
            if (!Common.checkValidatePhone(phone))
                return "Số điện thoại liên lạc không hợp lệ";
            if(otherPhone != null && !"".Equals(otherPhone))
                if (!Common.checkValidatePhone(otherPhone))
                    return "Số điện thoại khác không hợp lệ";
            if (address == null || "".Equals(address))
                return "Vui lòng nhập địa chỉ liên hệ.";
            if (errStr == null || "".Equals(errStr))
                return "Vui lòng nhập thông tin lỗi sản phẩm.";
            return "";
        }
        public static ResultReturn TransferGSIS(string month, string year)
        {
            ResultReturn result = new ResultReturn();
            string constr = ConfigurationManager.ConnectionStrings[CommonConstants.CONNECTION_STRING].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand(CommonConstants.SQL_TEXT_GET_LIST_SERVICE_REQUEST_NOT_TRANSFER_GSIS))
                {
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.Connection = con;
                    cmd.Parameters.Add("@MONTH", SqlDbType.Int).Value = month;
                    cmd.Parameters.Add("@YEAR", SqlDbType.Int).Value = year;

                    con.Open();
                    try
                    {
                        using (SqlDataReader sdr = cmd.ExecuteReader())
                        {
                            while (sdr.Read())
                            {
                                ItemReturn item = TransferSRToGSIS(GetItem(sdr["SRID"].ToString()));
                                UpdateStatusSRTransfer(item, sdr["SRID"].ToString());
                                result.code = "1";
                                result.msg = "";
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
        public static ItemResult checkSRStatus(string phone, string serial, string model, string cardNo, string matid)
        {
            ItemResult result = new ItemResult();
            string srnum = "", sourceType = "", sourceCode = "";
            string constr = ConfigurationManager.ConnectionStrings[CommonConstants.CONNECTION_STRING].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand(CommonConstants.STORED_PROCEDURE_SERVICE_REQUEST_GET_SRNUM))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Connection = con;
                    if (cardNo != null && !"".Equals(cardNo))
                        cmd.Parameters.Add("@REQUESTCODE", SqlDbType.VarChar).Value = cardNo;
                    if (phone != null && !"".Equals(phone))
                        cmd.Parameters.Add("@PHONE", SqlDbType.VarChar).Value = phone;
                    if (serial != null && !"".Equals(serial))
                        cmd.Parameters.Add("@BARCODE", SqlDbType.VarChar).Value = serial;
                    if(matid != null && !"".Equals(matid))
                        cmd.Parameters.Add("@MATID", SqlDbType.Int).Value = Convert.ToInt32(matid);
                    if (model != null && !"".Equals(model))
                        cmd.Parameters.Add("@MODELTYPE", SqlDbType.VarChar).Value = model;
                    con.Open();
                    try
                    {
                        using (SqlDataReader sdr = cmd.ExecuteReader())
                        {
                            while (sdr.Read())
                            {
                                srnum = sdr["SRNUM"].ToString();
                                sourceCode = sdr["SOURCECODE"].ToString();
                                sourceType = sdr["SOURCETYPE"].ToString();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        result.code = "0";
                        result.message = ex.Message;
                        result.data = "";
                    }
                }
            }
            if(srnum != "")
            {
                result = getInfoServiceRequestToGSIS(srnum); 
            }
            return result;
        }
        private static ItemResult getInfoServiceRequestToGSIS(string srnum)
        {
            ItemResult result = new ItemResult();
            try
            {
                InterfaceServerRequest.SPI_spcAEV_spcCCM_spcInterface_spcServiceClient client = new InterfaceServerRequest.SPI_spcAEV_spcCCM_spcInterface_spcServiceClient();

                InterfaceServerRequest.CheckSrStatus_Input request = new InterfaceServerRequest.CheckSrStatus_Input();
                request.SRNum = srnum;
                request.SourceCode = "1";
                request.SourceType = "1";
                request.HomePhone = "";
                request.RequestCode = "";
                request.RequestDateFrom = "";
                request.RequestDateTo = "";
                request.SHOPCODE = "";
                request.Status = "";
                request.WorkPhone = "";

                InterfaceServerRequest.CheckSrStatus_Output responses = client.CheckSrStatus(request);
                result.code = responses.Error_spcCode == "0" ? "1" : "0";
                result.message = responses.Error_spcMessage;
                List<ServiceRequestInfo_New> listSR = new List<ServiceRequestInfo_New>();
                foreach (var item in responses.ListOfserviceRequest)
                {
                    ServiceRequestInfo_New sr = new ServiceRequestInfo_New();
                    sr.CustomerName = item.CustomerName;
                    sr.CustomerDetailAddress = item.CustomerDetailAddress;
                    sr.CustomerPhone = item.CustomerPhone;

                    sr.Brand = item.Brand;
                    sr.Category = item.Category;
                    sr.ProductModel = item.ProductModel;
                    sr.serialNumber = item.serialNumber;
                    sr.SRNum = item.SRNum;
                    sr.L3 = item.L3;
                    sr.Status = ConvertStatusToVietNamese(item.Status);
                    sr.ServiceClass = item.ServiceClass;
                    sr.PartNumber = item.PartNmuber;
                    sr.OrderDate = item.OrderDate;
                    sr.ShipDate = item.ShipDate;
                    sr.ReceiveDate = item.ReceiveDate;
                    listSR.Add(sr);
                }
                result.data = listSR;
            }
            catch (Exception ex)
            {
                result.code = "0";
                result.message = ex.Message;
            }

            return result;
        }
        private static string ConvertStatusToVietNamese(string status)
        {
            try
            {
                XmlNodeList nodes = ReadXML.InitializeCulture();
                string result = ReadXML.GetText(status, nodes);
                if ("".Equals(result))
                    return status;
                return result;
            }catch(Exception ex)
            {
                return ex.Message;
            }
        }
        public static ServiceRequest getInfoServiceRequest(string id, string flat)
        {
            ServiceRequest result = new ServiceRequest();
            string constr = ConfigurationManager.ConnectionStrings[CommonConstants.CONNECTION_STRING].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand(CommonConstants.STORED_PROCEDURE_SERVICE_REQUEST_GET_INFO_SR_BY_EWARRANTY))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Connection = con;
                    cmd.Parameters.Add("@EWCARDID", SqlDbType.Int).Value = id;
                    cmd.Parameters.Add("@FLAT", SqlDbType.Int).Value = flat;

                    con.Open();
                    try
                    {
                        using (SqlDataReader sdr = cmd.ExecuteReader())
                        {
                            while (sdr.Read())
                            {
                                result.cusName = sdr["CUSNAME"].ToString();
                                result.cusProvince = sdr["CUSPROVINCEID"].ToString();
                                result.cusDistrict = sdr["CUSDISTRICTID"].ToString();
                                result.cusAddress = sdr["CUSADDRESS"].ToString();
                                result.cusPhone = sdr["CUSPHONE"].ToString();

                                result.plant = sdr["PLANTID"].ToString();
                                result.model = sdr["MATID"].ToString();
                                result.barcode = sdr["BARCODE"].ToString();

                                result.purchaseDate = sdr["PURCHASEDDATE"] == DBNull.Value ? "" : Convert.ToDateTime(sdr["PURCHASEDDATE"]).ToString("dd/MM/yyyy");
                                result.expiredDate = sdr["EXPIREDDATE"] == DBNull.Value ? "" : Convert.ToDateTime(sdr["EXPIREDDATE"]).ToString("dd/MM/yyyy");

                                result.shopPhone = sdr["SHOPMOBILE"].ToString();
                                result.shopContact = sdr["SHOPNAME"].ToString();
                            }
                        }
                    }
                    catch (Exception)
                    {
                        result = new ServiceRequest();
                    }
                }
            }
            return result;
        }
        public static Ministry getInfoMinistry(string srnum)
        {
            Ministry result = new Ministry();
            string constr = ConfigurationManager.ConnectionStrings[CommonConstants.CONNECTION_STRING].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand(CommonConstants.STORED_PROCEDURE_SERVICE_REQUEST_GET_INFO_MINISTRY))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Connection = con;
                    cmd.Parameters.Add("@SRNUM", SqlDbType.VarChar).Value = srnum;

                    con.Open();
                    try
                    {
                        using (SqlDataReader sdr = cmd.ExecuteReader())
                        {
                            while (sdr.Read())
                            {
                                result.MId = sdr["MID"].ToString();
                                result.SRNum = sdr["SRNUM"].ToString();
                                result.title = sdr["MTITLE"].ToString();
                                result.subTitle = sdr["MSUBTITLE"].ToString();
                            }
                        }
                    }
                    catch (Exception)
                    {
                        result = new Ministry();
                    }
                }
            }
            return result;
        }
        public static List<Question> getListQuestions(string mId)
        {
            List<Question> result = new List<Question>();
            string constr = ConfigurationManager.ConnectionStrings[CommonConstants.CONNECTION_STRING].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand(CommonConstants.STORED_PROCEDURE_SURVEY_QUESTIONS))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Connection = con;
                    cmd.Parameters.Add("@MID", SqlDbType.Int).Value = mId;

                    con.Open();
                    try
                    {
                        using (SqlDataReader sdr = cmd.ExecuteReader())
                        {
                            while (sdr.Read())
                            {
                                Question item = new Question();
                                item.QId = sdr["QID"].ToString();
                                item.QContent = sdr["QCONTENT"].ToString();
                                string ministry = sdr["MID"].ToString();
                                item.answers = getListAnswers(ministry, item.QId);
                                result.Add(item);
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
        private static List<Answer> getListAnswers(string mId, string qId)
        {
            List<Answer> result = new List<Answer>();
            string constr = ConfigurationManager.ConnectionStrings[CommonConstants.CONNECTION_STRING].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand(CommonConstants.STORED_PROCEDURE_SURVEY_ANSWERS))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Connection = con;
                    cmd.Parameters.Add("@MID", SqlDbType.Int).Value = mId;
                    cmd.Parameters.Add("@QID", SqlDbType.Int).Value = qId;

                    con.Open();
                    try
                    {
                        using (SqlDataReader sdr = cmd.ExecuteReader())
                        {
                            while (sdr.Read())
                            {
                                Answer item = new Answer();
                                item.AId = sdr["AID"].ToString();
                                item.AContent = sdr["ACONTENT"].ToString();
                                item.QAId = sdr["QAID"].ToString();
                                result.Add(item);
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
    }
}
