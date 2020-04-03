using EWShop.Models;
using EWShop.Utilities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;

namespace EWShop.Areas.CallCenter.Models
{
    public class EwarrantyItem
    {
        public string EWCardId { get; set; }
        public string EWSource { get; set; }
        public string barcode { get; set; }
        public string plant { get; set; }
        public string model { get; set; }
        public string POD { get; set; }
        public string expiredDate { get; set; }
        public string cusName { get; set; }
        public string cusPhone { get; set; }
        public string cusProvince { get; set; }
        public string cusDistrict { get; set; }
        public string cusAddress { get; set; }
        public string shopId { get; set; }
        public string shopCode { get; set; }
        public string shopName { get; set; }
        public string flag { get; set; }
        public string img1 { get; set; }
        public string img2 { get; set; }
    }
    public class SMSItem
    {
        public string id { get; set; }
        public string sender { get; set; }
        public string content { get; set; }
        public string regDate { get; set; }
        public string status { get; set; }
        public string isCheck { get; set; }
        public string isLock { get; set; }
        public string comment { get; set; }
    }
    public class BarcodeInfoDetail
    {
        public string code { get; set; }
        public string msg { get; set; }
        public string barcode { get; set; }
        public string plant { get; set; }
        public string matid { get; set; }
    }
    public class EwarrantySMS
    {
        public string id { get; set; }
        public string POD { get; set; }
        public string expiredDate { get; set; }
        public string barcode { get; set; }
        public string model { get; set; }
        public string cusName { get; set; }
        public string cusPhone { get; set; }
        public string cusAddress { get; set; }
        public string shopPhone { get; set; }
        public string shopId { get; set; }
        public string shopName { get; set; }
    }
    public static class EwarrantyLogics
    {
        #region "Đăng ký bảo hành không thành công"
        public static List<EwarrantyItem> listRegisterEWNotCompleted(string frmDate, string toDate, string ewSource, string isImage, string barcode)
        {
            List<EwarrantyItem> result = new List<EwarrantyItem>();
            string constr = ConfigurationManager.ConnectionStrings[CommonConstants.CONNECTION_STRING].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand("uspWeb_EWarranty_ListRegisterEWNotCompleted"))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Connection = con;
                    if (frmDate != null && frmDate != "")
                        cmd.Parameters.Add("@FRMDATE", SqlDbType.DateTime).Value = Common.convertStringWithFormatDDMMYYYYToDate(frmDate);
                    if (toDate != null && toDate != "")
                        cmd.Parameters.Add("@TODATE", SqlDbType.DateTime).Value = Common.convertStringWithFormatDDMMYYYYToDate(toDate);
                    if (ewSource != null && ewSource != "")
                        cmd.Parameters.Add("@SOURCE", SqlDbType.Int).Value = Convert.ToInt32(ewSource);
                    if (isImage != null && isImage != "")
                        cmd.Parameters.Add("@IMAGE", SqlDbType.Int).Value = Convert.ToInt32(isImage);
                    if (barcode != null && barcode != "")
                        cmd.Parameters.Add("@BARCODE", SqlDbType.VarChar).Value = barcode;
                    con.Open();
                    try
                    {
                        using (SqlDataReader sdr = cmd.ExecuteReader())
                        {
                            while (sdr.Read())
                            {
                                EwarrantyItem dt = new EwarrantyItem();
                                dt.EWCardId = sdr["EWCARDID"].ToString();
                                dt.barcode = sdr["BARCODE"].ToString();
                                dt.model = sdr["MTNAME"].ToString();
                                dt.POD = sdr["PURCHASEDDATE"] == DBNull.Value ? "" : Convert.ToDateTime(sdr["PURCHASEDDATE"]).ToString("dd/MM/yyyy");
                                dt.cusName = sdr["CUSNAME"].ToString();
                                dt.cusPhone = sdr["CUSPHONE"].ToString();
                                dt.EWSource = sdr["EWSOURCENAME"].ToString();
                                dt.shopCode = sdr["SHOPCODE"].ToString();
                                dt.shopName = sdr["SHOPNAME"].ToString();
                                dt.flag = sdr["EWPSI"].ToString();

                                result.Add(dt);
                            }
                        }
                    }
                    catch (Exception)
                    {

                    }
                }
            }
            return result;
        }
        public static ResultReturn rejectEWarranty(string cardId, string reason)
        {
            ResultReturn result = new ResultReturn();
            string constr = ConfigurationManager.ConnectionStrings[CommonConstants.CONNECTION_STRING].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand("uspWeb_EWarranty_ClosedEWNotCompleted"))
                {
                    try
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        if (cardId != null && !"".Equals(cardId) && !"null".Equals(cardId) && !"undefined".Equals(cardId))
                            cmd.Parameters.Add("@EWCARDID", SqlDbType.Int).Value = Convert.ToInt32(cardId);
                        if (reason != null && !"".Equals(reason) && !"null".Equals(reason) && !"undefined".Equals(reason))
                            cmd.Parameters.Add("@COMMENT", SqlDbType.NVarChar).Value = reason;
                        con.Open();
                        cmd.ExecuteReader();
                        result.code = CommonConstants.EXECUTE_SUCCESS;
                        result.msg = "";
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
        public static ItemResult saveEWarranty(EwarrantyItem input, string username)
        {
            ItemResult result = new ItemResult();
            string ewCardId = "";
            string error = checkDataInput(input);
            if (!"".Equals(error))
            {
                result.code = "0";
                result.message = error;
                result.data = "";
            }
            string constr = ConfigurationManager.ConnectionStrings[CommonConstants.CONNECTION_STRING].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand("uspWeb_EWarranty_RegisterEW"))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Connection = con;
                    if (input.EWCardId != null && !"".Equals(input.EWCardId) && !"null".Equals(input.EWCardId) && !"undefined".Equals(input.EWCardId))
                        cmd.Parameters.Add("@EWCARDID", SqlDbType.VarChar).Value = input.EWCardId;
                    if (input.barcode != null && !"".Equals(input.barcode) && !"null".Equals(input.barcode) && !"undefined".Equals(input.barcode))
                        cmd.Parameters.Add("@BARCODE", SqlDbType.VarChar).Value = input.barcode;
                    if (input.shopId != null && !"".Equals(input.shopId) && !"null".Equals(input.shopId) && !"undefined".Equals(input.shopId))
                        cmd.Parameters.Add("@SHOPID", SqlDbType.Int).Value = input.shopId;
                    if (input.model != null && !"".Equals(input.model) && !"null".Equals(input.model) && !"undefined".Equals(input.model))
                        cmd.Parameters.Add("@MATID", SqlDbType.Int).Value = input.model;
                    if (input.cusName != null && !"".Equals(input.cusName) && !"null".Equals(input.cusName) && !"undefined".Equals(input.cusName))
                        cmd.Parameters.Add("@CUSNAME", SqlDbType.NVarChar).Value = input.cusName;
                    if (input.cusProvince != null && !"".Equals(input.cusProvince) && !"null".Equals(input.cusProvince) && !"undefined".Equals(input.cusProvince))
                        cmd.Parameters.Add("@CUSPROVINCEID", SqlDbType.Int).Value = input.cusProvince;
                    if (input.cusDistrict != null && !"".Equals(input.cusDistrict) && !"null".Equals(input.cusDistrict) && !"undefined".Equals(input.cusDistrict))
                        cmd.Parameters.Add("@CUSDISTRICTID", SqlDbType.Int).Value = input.cusDistrict;
                    if (input.cusPhone != null && !"".Equals(input.cusPhone) && !"null".Equals(input.cusPhone) && !"undefined".Equals(input.cusPhone))
                        cmd.Parameters.Add("@CUSPHONE", SqlDbType.VarChar).Value = input.cusPhone;
                    if (input.cusAddress != null && !"".Equals(input.cusAddress) && !"null".Equals(input.cusAddress) && !"undefined".Equals(input.cusAddress))
                        cmd.Parameters.Add("@CUSADDRESS", SqlDbType.NVarChar).Value = input.cusAddress;
                    if (input.POD != null && !"".Equals(input.POD) && !"null".Equals(input.POD) && !"undefined".Equals(input.POD))
                        cmd.Parameters.Add("@POD", SqlDbType.DateTime).Value = Common.convertStringWithFormatDDMMYYYYToDate(input.POD);
                    if (input.expiredDate != null && !"".Equals(input.expiredDate) && !"null".Equals(input.expiredDate) && !"undefined".Equals(input.expiredDate))
                        cmd.Parameters.Add("@EXPIREDDATE", SqlDbType.DateTime).Value = Common.convertStringWithFormatDDMMYYYYToDate(input.expiredDate);
                    cmd.Parameters.Add("@TRANSUSER", SqlDbType.VarChar).Value = username;
                    con.Open();
                    try
                    {
                        using (SqlDataReader sdr = cmd.ExecuteReader())
                        {
                            while (sdr.Read())
                            {
                                result.code = "1";
                                result.message = "";
                                result.data = sdr["EWCARDNO"].ToString();
                                ewCardId = sdr["EWCARDID"].ToString();
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
            if(ewCardId != "")
            {
                FiboSMS.GetInfoEW(ewCardId);
            }
            return result;
        }
        private static string checkDataInput(EwarrantyItem input)
        {
            if (input.barcode != null && !"".Equals(input.barcode) && !"null".Equals(input.barcode) && !"undefined".Equals(input.barcode))
                return "Please enter barcode value";
            if (input.model != null && !"".Equals(input.model) && !"null".Equals(input.model) && !"undefined".Equals(input.model))
                return "Please choose model value";
            if (input.cusName != null && !"".Equals(input.cusName) && !"null".Equals(input.cusName) && !"undefined".Equals(input.cusName))
                return "Please enter customer name";
            if (input.cusPhone != null && !"".Equals(input.cusPhone) && !"null".Equals(input.cusPhone) && !"undefined".Equals(input.cusPhone))
                return "Please enter customer phone";
            if (input.POD != null && !"".Equals(input.POD) && !"null".Equals(input.POD) && !"undefined".Equals(input.POD))
                return "Please enter purchase of date";
            return "";
        }
        public static BarcodeInfoDetail checkBarcode(string barcode, string matId)
        {
            BarcodeInfoDetail result = new BarcodeInfoDetail();
            string constr = ConfigurationManager.ConnectionStrings[CommonConstants.CONNECTION_STRING].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand("uspWeb_CallCenter_CheckBarcode"))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Connection = con;
                    cmd.Parameters.Add("@BARCODE", SqlDbType.VarChar).Value = barcode;
                    if (matId != null && matId != "")
                        cmd.Parameters.Add("@MATID", SqlDbType.Int).Value = Convert.ToInt32(matId);
                    con.Open();
                    try
                    {
                        using (SqlDataReader sdr = cmd.ExecuteReader())
                        {
                            while (sdr.Read())
                            {
                                result.code = sdr["RETURNID"].ToString();
                                result.msg = sdr["RETURNMSG"].ToString();
                                result.barcode = sdr["BARCODE"].ToString();
                                result.plant = sdr["PLANTID"] == DBNull.Value ? "0" : ("".Equals(sdr["PLANTID"].ToString()) ? "0" : sdr["PLANTID"].ToString());
                                result.matid = sdr["MATID"] == DBNull.Value ? "0" : ("".Equals(sdr["MATID"].ToString()) ? "0" : sdr["MATID"].ToString());
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        result.code = "0";
                        result.msg = ex.Message;
                        result.barcode = barcode;
                        result.plant = "0";
                        result.matid = "0";
                    }
                }
            }
            return result;
        }
        public static EwarrantyItem getItemInfo(string cardId)
        {
            EwarrantyItem result = new EwarrantyItem();
            string constr = ConfigurationManager.ConnectionStrings[CommonConstants.CONNECTION_STRING].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM vTMPEWNotCompleted WHERE EWCARDID=@EWCARDID"))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = con;
                    if (cardId != null && cardId != "")
                        cmd.Parameters.Add("@EWCARDID", SqlDbType.Int).Value = Convert.ToInt32(cardId);
                    con.Open();
                    try
                    {
                        using (SqlDataReader sdr = cmd.ExecuteReader())
                        {
                            while (sdr.Read())
                            {
                                result.EWCardId = sdr["EWCARDID"].ToString();
                                result.barcode = sdr["BARCODE"].ToString();
                                result.plant = sdr["PLANTID"] == DBNull.Value ? "0" : ("".Equals(sdr["PLANTID"].ToString()) ? "0" : sdr["PLANTID"].ToString());
                                result.model = sdr["MATID"] == DBNull.Value ? "0" : ("".Equals(sdr["MATID"].ToString()) ? "0" : sdr["MATID"].ToString());
                                result.POD = sdr["PURCHASEDDATE"] == DBNull.Value ? "" : Convert.ToDateTime(sdr["PURCHASEDDATE"]).ToString("dd/MM/yyyy");
                                result.expiredDate = sdr["EXPIREDDATE"] == DBNull.Value ? "" : Convert.ToDateTime(sdr["EXPIREDDATE"]).ToString("dd/MM/yyyy");
                                result.cusName = sdr["CUSNAME"].ToString();
                                result.cusPhone = sdr["CUSPHONE"].ToString();
                                result.cusProvince = sdr["CUSPROVINCEID"] == DBNull.Value ? "0" : ("".Equals(sdr["CUSPROVINCEID"].ToString()) ? "0" : sdr["CUSPROVINCEID"].ToString());
                                result.cusDistrict = sdr["CUSDISTRICTID"] == DBNull.Value ? "0" : ("".Equals(sdr["CUSDISTRICTID"].ToString()) ? "0" : sdr["CUSDISTRICTID"].ToString());
                                result.cusAddress = sdr["CUSADDRESS"].ToString();
                                result.shopId = sdr["SHOPID"].ToString();
                                result.flag = sdr["COMMENT"].ToString();
                                result.img1 = sdr["IMG1"].ToString();
                                if (result.img1 == null || "".Equals(result.img1))
                                    result.img1 = "../Libraries/dist/img/no-image.png";
                                result.img2 = sdr["IMG2"].ToString();
                                if (result.img2 == null || "".Equals(result.img2))
                                    result.img2 = "../Libraries/dist/img/no-image.png";
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
        #endregion
        #region "Đăng ký SMS"
        public static List<SMSItem> listSMSInvalid(string frmDate, string toDate, string status, string check)
        {
                List<SMSItem> result = new List<SMSItem>();
                string constr = ConfigurationManager.ConnectionStrings[CommonConstants.CONNECTION_STRING].ConnectionString;
                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand("uspWeb_EWarranty_ListRegisterSMS"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        if (frmDate != null && frmDate != "")
                            cmd.Parameters.Add("@FRMDATE", SqlDbType.DateTime).Value = Common.convertStringWithFormatDDMMYYYYToDate(frmDate);
                        if (toDate != null && toDate != "")
                            cmd.Parameters.Add("@TODATE", SqlDbType.DateTime).Value = Common.convertStringWithFormatDDMMYYYYToDate(toDate);
                        if (status != null && status != "")
                            cmd.Parameters.Add("@STATUS", SqlDbType.Int).Value = Convert.ToInt32(status);
                        if (check != null && check != "")
                            cmd.Parameters.Add("@CHECK", SqlDbType.Int).Value = Convert.ToInt32(check);
                        con.Open();
                        try
                        {
                            using (SqlDataReader sdr = cmd.ExecuteReader())
                            {
                                while (sdr.Read())
                                {
                                    SMSItem dt = new SMSItem();
                                    dt.id = sdr["SMSID"].ToString();
                                    dt.sender = sdr["SENDER"].ToString();
                                    if (!"".Equals(dt.sender))
                                    {
                                        if(dt.sender[0] == '8' && dt.sender[1] == '4')
                                        {
                                            dt.sender = "0" + dt.sender.Remove(0, 2);
                                        }
                                    }
                                    dt.content = sdr["MSGCONTENT"].ToString().ToUpper();
                                    dt.regDate = sdr["ISSUEDDATE"] == DBNull.Value ? "" : Convert.ToDateTime(sdr["ISSUEDDATE"]).ToString("dd/MM/yyyy HH:mm");
                                    dt.status = sdr["ISVALID"].ToString();
                                    dt.isCheck = sdr["ISAPPROVED"].ToString();
                                    dt.isLock = sdr["ISLOCK"].ToString();
                                    dt.comment = sdr["ERRORMSG"].ToString();
                                    result.Add(dt);
                                }
                            }
                        }
                        catch (Exception)
                        {
                            
                        }
                    }
                }
                return result;
            }
        public static ResultReturn updateComment(string id, string comment)
        {
            ResultReturn result = new ResultReturn();
            string constr = ConfigurationManager.ConnectionStrings[CommonConstants.CONNECTION_STRING].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand("uspWeb_CallCenter_UpdateComment"))
                {
                    try
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        if (id != null && !"".Equals(id) && !"null".Equals(id) && !"undefined".Equals(id))
                            cmd.Parameters.Add("@ID", SqlDbType.Int).Value = Convert.ToInt32(id);
                        if (comment != null && !"".Equals(comment) && !"null".Equals(comment) && !"undefined".Equals(comment))
                            cmd.Parameters.Add("@COMMENT", SqlDbType.NVarChar).Value = comment;
                        con.Open();
                        cmd.ExecuteReader();
                        result.code = CommonConstants.EXECUTE_SUCCESS;
                        result.msg = "";
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
        public static ItemReturn checkShopPhone(string phone)
        {
            ItemReturn result = new ItemReturn();
            string constr = ConfigurationManager.ConnectionStrings[CommonConstants.CONNECTION_STRING].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand("uspWeb_EWarranty_CheckPhoneShop"))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Connection = con;
                    if (phone != null && phone != "")
                        cmd.Parameters.Add("@PHONE", SqlDbType.VarChar).Value = phone;
                    con.Open();
                    try
                    {
                        using (SqlDataReader sdr = cmd.ExecuteReader())
                        {
                            while (sdr.Read())
                            {
                                int number = sdr["NUMBER"] == DBNull.Value ? 0 : ("".Equals(sdr["NUMBER"].ToString()) ? 0 : Convert.ToInt32(sdr["NUMBER"]));
                                if(number > 0)
                                {
                                    result.code = CommonConstants.EXECUTE_SUCCESS;
                                    result.message = "";
                                    result.data = sdr["SHOPNAME"].ToString();
                                }else
                                {
                                    result.code = CommonConstants.EXECUTE_UNSUCCESS;
                                    result.message = "Phone of shop not exists in database system";
                                }
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
            return result;
        }
        public static ItemResult registerEWSMS(EwarrantySMS input, string username)
        {
            ItemResult result = new ItemResult();
            string ewCardId = "";
            string error = checkDataSMSInput(input);
            if (!"".Equals(error))
            {
                result.code = "0";
                result.message = error;
                result.data = "";
            }
            string constr = ConfigurationManager.ConnectionStrings[CommonConstants.CONNECTION_STRING].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand("uspWeb_CallCenter_RegisterEW"))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Connection = con;
                    if (input.id != null && !"".Equals(input.id) && !"null".Equals(input.id) && !"undefined".Equals(input.id))
                        cmd.Parameters.Add("@ID", SqlDbType.Int).Value = input.id;

                    if (input.barcode != null && !"".Equals(input.barcode) && !"null".Equals(input.barcode) && !"undefined".Equals(input.barcode))
                        cmd.Parameters.Add("@BARCODE", SqlDbType.VarChar).Value = input.barcode;
                    if (input.model != null && !"".Equals(input.model) && !"null".Equals(input.model) && !"undefined".Equals(input.model))
                        cmd.Parameters.Add("@MATID", SqlDbType.Int).Value = input.model;

                    if (input.shopId != null && !"".Equals(input.shopId) && !"null".Equals(input.shopId) && !"undefined".Equals(input.shopId))
                        cmd.Parameters.Add("@SHOPID", SqlDbType.Int).Value = input.shopId;
                    if (input.shopPhone != null && !"".Equals(input.shopPhone) && !"null".Equals(input.shopPhone) && !"undefined".Equals(input.shopPhone))
                        cmd.Parameters.Add("@SHOPPHONE", SqlDbType.VarChar).Value = input.shopPhone;
                    if (input.shopName != null && !"".Equals(input.shopName) && !"null".Equals(input.shopName) && !"undefined".Equals(input.shopName))
                        cmd.Parameters.Add("@SHOPNAME", SqlDbType.NVarChar).Value = input.shopName;

                    
                    if (input.cusName != null && !"".Equals(input.cusName) && !"null".Equals(input.cusName) && !"undefined".Equals(input.cusName))
                        cmd.Parameters.Add("@CUSNAME", SqlDbType.NVarChar).Value = input.cusName;
                    if (input.cusPhone != null && !"".Equals(input.cusPhone) && !"null".Equals(input.cusPhone) && !"undefined".Equals(input.cusPhone))
                        cmd.Parameters.Add("@CUSPHONE", SqlDbType.VarChar).Value = input.cusPhone;
                    if (input.cusAddress != null && !"".Equals(input.cusAddress) && !"null".Equals(input.cusAddress) && !"undefined".Equals(input.cusAddress))
                        cmd.Parameters.Add("@CUSADDRESS", SqlDbType.NVarChar).Value = input.cusAddress;

                    if (input.POD != null && !"".Equals(input.POD) && !"null".Equals(input.POD) && !"undefined".Equals(input.POD))
                        cmd.Parameters.Add("@POD", SqlDbType.DateTime).Value = Common.convertStringWithFormatDDMMYYYYToDate(input.POD);
                    if (input.expiredDate != null && !"".Equals(input.expiredDate) && !"null".Equals(input.expiredDate) && !"undefined".Equals(input.expiredDate))
                        cmd.Parameters.Add("@EXPIREDDATE", SqlDbType.DateTime).Value = Common.convertStringWithFormatDDMMYYYYToDate(input.expiredDate);
                    
                    cmd.Parameters.Add("@TRANSUSER", SqlDbType.VarChar).Value = username;
                    con.Open();
                    try
                    {
                        using (SqlDataReader sdr = cmd.ExecuteReader())
                        {
                            while (sdr.Read())
                            {
                                result.code = sdr["RETURNID"].ToString();
                                result.message = sdr["RETURNMSG"].ToString();
                                result.data = sdr["EWCARDNO"].ToString();
                                ewCardId = sdr["EWCARDID"].ToString();
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
            if (ewCardId != "")
            {
                FiboSMS.GetInfoEW(ewCardId);
            }
            return result;
        }
        private static string checkDataSMSInput(EwarrantySMS input)
        {
            if (input.id != null && !"".Equals(input.id) && !"null".Equals(input.id) && !"undefined".Equals(input.id) && !"0".Equals(input.id))
                return "Please choose item to process";
            if (input.barcode != null && !"".Equals(input.barcode) && !"null".Equals(input.barcode) && !"undefined".Equals(input.barcode))
                return "Please enter barcode value";
            if (input.model != null && !"".Equals(input.model) && !"null".Equals(input.model) && !"undefined".Equals(input.model) && !"0".Equals(input.model))
                return "Please choose model value";
            if (input.cusName != null && !"".Equals(input.cusName) && !"null".Equals(input.cusName) && !"undefined".Equals(input.cusName))
                return "Please enter customer name";
            if (input.cusPhone != null && !"".Equals(input.cusPhone) && !"null".Equals(input.cusPhone) && !"undefined".Equals(input.cusPhone))
                return "Please enter customer phone";
            if (input.POD != null && !"".Equals(input.POD) && !"null".Equals(input.POD) && !"undefined".Equals(input.POD))
                return "Please enter purchase of date";
            return "";
        }
        public static ResultReturn closeSMSInvalid(string id, string comment, string username)
        {
            ResultReturn result = new ResultReturn();
            string constr = ConfigurationManager.ConnectionStrings[CommonConstants.CONNECTION_STRING].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand("uspWeb_CallCenter_CloseSMS"))
                {
                    try
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        if (id != null && !"".Equals(id) && !"null".Equals(id) && !"undefined".Equals(id))
                            cmd.Parameters.Add("@ID", SqlDbType.Int).Value = Convert.ToInt32(id);
                        if (comment != null && !"".Equals(comment) && !"null".Equals(comment) && !"undefined".Equals(comment))
                            cmd.Parameters.Add("@COMMENT", SqlDbType.NVarChar).Value = comment;
                        cmd.Parameters.Add("@TRANSUSER", SqlDbType.VarChar).Value = username;
                        con.Open();
                        cmd.ExecuteReader();
                        result.code = CommonConstants.EXECUTE_SUCCESS;
                        result.msg = "";
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
        #endregion
        #region "Đăng ký thành công"
        public static List<ProductSellOutReport> ListRegisteredEW(string frmDate, string toDate, string source, string barcode)
        {
            List<ProductSellOutReport> list = new List<ProductSellOutReport>();
            string constr = ConfigurationManager.ConnectionStrings[CommonConstants.CONNECTION_STRING].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand("uspWeb_EWarranty_ListRegisterEWCompleted"))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Connection = con;
                    if (source != null && !"".Equals(source))
                        cmd.Parameters.Add("@EWSOURCEID", SqlDbType.Int).Value = source;
                    if (barcode != null && !"".Equals(barcode))
                        cmd.Parameters.Add("@BARCODE", SqlDbType.VarChar).Value = barcode;
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
                                dt.POD = sdr["PURCHASEDDATE"] == DBNull.Value ? "" : Convert.ToDateTime(sdr["PURCHASEDDATE"]).ToString("dd/MM/yyyy");
                                /*--- Thông tin khách hàng ---*/
                                dt.cusName = sdr["CUSNAME"].ToString();
                                dt.cusPhone = sdr["CUSPHONE"].ToString();
                                dt.cusProvince = sdr["PROVINCENAME"].ToString().Trim();
                                dt.cusDistrict = sdr["DISTRICTNAME"].ToString().Trim();
                                dt.cusAddress = sdr["CUSADDRESS"].ToString().Trim(); 
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
        public static ProductSellOutReport getEwarrantyInfo(string cardId)
        {
            ProductSellOutReport dt = new ProductSellOutReport();
            string constr = ConfigurationManager.ConnectionStrings[CommonConstants.CONNECTION_STRING].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand("uspWeb_EWarranty_GetDetailEWInfo"))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Connection = con;
                    if (cardId != null && !"".Equals(cardId))
                        cmd.Parameters.Add("@EWCardId", SqlDbType.Int).Value = cardId;
                    con.Open();
                    try
                    {
                        using (SqlDataReader sdr = cmd.ExecuteReader())
                        {
                            while (sdr.Read())
                            {
                                dt.eWCardId = cardId;
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
                                dt.cusProvince = sdr["PROVINCENAME"].ToString().Trim();
                                dt.cusDistrict = sdr["DISTRICTNAME"].ToString().Trim();
                                dt.cusAddress = sdr["CUSADDRESS"].ToString().Trim();
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
                            }
                        }
                    }
                    catch (Exception)
                    {
                        dt = new ProductSellOutReport();
                    }
                }
            }
            return dt;
        }
        public static ResultReturn cancelEwarranty(string cardId, string reason, string username)
        {
            ResultReturn result = new ResultReturn();
            string constr = ConfigurationManager.ConnectionStrings[CommonConstants.CONNECTION_STRING].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand("uspWeb_EWarranty_CancelItem"))
                {
                    try
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        if (cardId != null && !"".Equals(cardId) && !"null".Equals(cardId) && !"undefined".Equals(cardId))
                            cmd.Parameters.Add("@EWCARDID", SqlDbType.Int).Value = Convert.ToInt32(cardId);
                        if (reason != null && !"".Equals(reason) && !"null".Equals(reason) && !"undefined".Equals(reason))
                            cmd.Parameters.Add("@REASONCANCEL", SqlDbType.NVarChar).Value = reason;
                        cmd.Parameters.Add("@MODIFIEDBY", SqlDbType.VarChar).Value = username;
                        con.Open();
                        cmd.ExecuteReader();
                        result.code = CommonConstants.EXECUTE_SUCCESS;
                        result.msg = "";
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
        public static List<ProductSellOutReport> ListEWNotYetTransferGSIS(string frmDate, string toDate, string count, string type, string month)
        {
            List<ProductSellOutReport> list = new List<ProductSellOutReport>();
            string constr = ConfigurationManager.ConnectionStrings[CommonConstants.CONNECTION_STRING].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand("uspWeb_EWarranty_GetListEWNotEwCardNo"))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Connection = con;
                    cmd.Parameters.Add("@TYPE", SqlDbType.Int).Value = type;
                    if (count != null && !"".Equals(count))
                        cmd.Parameters.Add("@NUMBER", SqlDbType.Int).Value = count;
                    if (month != null && !"".Equals(month))
                        cmd.Parameters.Add("@MONTH", SqlDbType.Int).Value = month;
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
                                dt.POD = sdr["PURCHASEDDATE"] == DBNull.Value ? "" : Convert.ToDateTime(sdr["PURCHASEDDATE"]).ToString("dd/MM/yyyy");
                                /*--- Thông tin khách hàng ---*/
                                dt.cusName = sdr["CUSNAME"].ToString();
                                dt.cusPhone = sdr["CUSPHONE"].ToString();
                                /*--- Thông tin cửa hàng ---*/
                                dt.shopName = sdr["SHOPNAME"].ToString();
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
        #endregion
    }
}