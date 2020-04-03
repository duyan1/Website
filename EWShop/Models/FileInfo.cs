using EWShop.Utilities;
using Microsoft.Office.Core;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Web;
using excel = Microsoft.Office.Interop.Excel;

namespace EWShop.Models
{
    public class FileInfo
    {
        public string fileId { get; set; }
        public string fileName { get; set; }
        public string dateUpload { get; set; }
        public string userUpload { get; set; }
        public string total { get; set; }
        public string valid { get; set; }
        public string status { get; set; }
    }
    public class ImageInfo
    {
        public string imageId { get; set; }
        public string imageUrl { get; set; }
        public string imageOrig { get; set; }
        public string fileId { get; set; }
        public string uploadDate { get; set; }
    }
    public static class FileUpload
    {
        public static string upLoadImage(HttpPostedFileBase file, string fileUrl, string fileName)
        {
            string result = "";
            try
            {
                bool valid = IsAcceptedFileType(file.FileName, new string[] { ".png", ".jpg", ".gif", ".pdf" });
                if (valid)
                {
                    ResultReturn item = executeUploadImage(fileName, fileUrl);
                    if (item.code != "")
                    {
                        file.SaveAs(item.msg);
                        result = item.code;
                    }  
                }
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }
            return result;
        }
        public static ResultReturn removeImage(string fileId, string imgId)
        {
            ResultReturn result = new ResultReturn();
            string imgUrl = getUrlLocalImage(fileId, imgId);
            if(imgUrl != "")
            {
                if (File.Exists(imgUrl))
                {
                    File.Delete(imgUrl);
                }
                string constr = ConfigurationManager.ConnectionStrings[CommonConstants.CONNECTION_STRING].ConnectionString;
                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand(CommonConstants.STORED_PROCEDURE_IMAGE_DELETE))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.Add("@FILEID", SqlDbType.Int).Value = fileId;
                        cmd.Parameters.Add("@IMGID", SqlDbType.Int).Value = imgId;

                        con.Open();
                        try
                        {
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
            }
            return result;
        }
        private static string getUrlLocalImage(string fileId, string imgId)
        {
            string result = "";
            string constr = ConfigurationManager.ConnectionStrings[CommonConstants.CONNECTION_STRING].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand(CommonConstants.SQL_TEXT_GET_URL_IMAGE))
                {
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.Connection = con;
                    cmd.Parameters.Add("@FILEID", SqlDbType.Int).Value = fileId;
                    cmd.Parameters.Add("@IMGID", SqlDbType.Int).Value = imgId;

                    con.Open();
                    try
                    {
                        using (SqlDataReader sdr = cmd.ExecuteReader())
                        {
                            while (sdr.Read())
                            {
                                result = sdr["IMGURL"].ToString();
                            }
                        }
                    }
                    catch (Exception)
                    {
                        result = "";
                    }
                }
            }
            if(result != "")
            {
                result = result.Replace(CommonConstants.URL_SERVER, CommonConstants.UPLOAD_DIRECT_STRING);
            }
            return result;
        }
        public static ResultReturn insertImageUpload(string fileNameStr, string origFileName, string fileId, string userName)
        {
            ResultReturn result = new ResultReturn();
            string constr = ConfigurationManager.ConnectionStrings[CommonConstants.CONNECTION_STRING].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand(CommonConstants.STORED_PROCEDURE_IMAGE_INSERT))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Connection = con;
                    cmd.Parameters.Add("@IMGURL", SqlDbType.NVarChar).Value = fileNameStr == null ? "" : fileNameStr.Trim().ToLower();
                    cmd.Parameters.Add("@ORIG_IMG", SqlDbType.NVarChar).Value = origFileName == null ? "" : origFileName.Trim().ToLower();
                    cmd.Parameters.Add("@FILEID", SqlDbType.VarChar).Value = (fileId == null || fileId == "") ? "0" : fileId.Trim();
                    cmd.Parameters.Add("@USERNAME", SqlDbType.VarChar).Value = userName == null ? "" : userName.Trim().ToLower();
                    con.Open();
                    try
                    {
                        using (SqlDataReader sdr = cmd.ExecuteReader())
                        {
                            while (sdr.Read())
                            {
                                int returnId = Convert.ToInt32(sdr["IMGID"]);

                                result.code = returnId > 0 ? CommonConstants.EXECUTE_SUCCESS : CommonConstants.EXECUTE_UNSUCCESS;
                                result.msg = returnId > 0 ? "Ghi nhận hoàn tất" : "Không thể tải hình ảnh";
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
        public static ResultReturn completeUploadEW(string fileId)
        {
            return updateStatusFileUpload(fileId, 2);
        }
        public static List<ItemSellOut> listDataTempFileUpload(string fileId)
        {
            List<ItemSellOut> list = new List<ItemSellOut>();
            string constr = ConfigurationManager.ConnectionStrings[CommonConstants.CONNECTION_STRING].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand(CommonConstants.STORED_PROCEDURE_GET_LIST_DATA_FILE_UPLOAD))
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
                                ItemSellOut info = new ItemSellOut();
                                info.barcode = sdr["BARCODE"].ToString();
                                info.cusAddress = sdr["CUSADDRESS"].ToString();
                                info.cusEmail = sdr["CUSEMAIL"].ToString();
                                info.cusName = sdr["CUSNAME"].ToString();
                                info.cusPhone = sdr["CUSPHONE"].ToString();
                                info.id = sdr["ID"].ToString();
                                info.purchaseDate = sdr["POD"] == DBNull.Value ? "" : Convert.ToDateTime(sdr["POD"]).ToString("dd/MM/yyyy");
                                info.status = sdr["MSG"].ToString();
                                info.model = sdr["MODELTYPE"].ToString();
                                list.Add(info);
                            }
                        }
                    }
                    catch (Exception)
                    {
                        list = new List<ItemSellOut>();
                    }
                }
            }
            return list;
        }
        public static List<FileInfo> listFileUpload(string frmDate, string toDate, string name)
        {
            List<FileInfo> list = new List<FileInfo>();
            string constr = ConfigurationManager.ConnectionStrings[CommonConstants.CONNECTION_STRING].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand("uspWeb_FileUpload_View13072019"))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Connection = con;
                    if (frmDate != null && frmDate != "")
                        cmd.Parameters.Add("@FRMDATE", SqlDbType.DateTime).Value = Common.convertStringWithFormatDDMMYYYYToDate(frmDate);
                    if (toDate != null && toDate != "")
                        cmd.Parameters.Add("@TODATE", SqlDbType.DateTime).Value = Common.convertStringWithFormatDDMMYYYYToDate(toDate);
                    cmd.Parameters.Add("@USERNAME", SqlDbType.VarChar).Value = name;
                    con.Open();
                    try
                    {
                        using (SqlDataReader sdr = cmd.ExecuteReader())
                        {
                            while (sdr.Read())
                            {
                                FileInfo info = new FileInfo();
                                info.fileId = sdr["FILEID"].ToString();
                                info.fileName = sdr["FILENAME"].ToString();
                                info.total = sdr["TOTAL"].ToString();
                                info.valid = sdr["VALID"].ToString();
                                info.dateUpload = sdr["DATEUPLOAD"] == DBNull.Value ? "" : Convert.ToDateTime(sdr["DATEUPLOAD"]).ToString("dd/MM/yyyy HH:mm");
                                info.userUpload = sdr["USERUPLOAD"].ToString();
                                info.status = sdr["STATUS"].ToString();
                                list.Add(info);
                            }
                        }
                    }
                    catch (Exception)
                    {
                        list = new List<FileInfo>();
                    }
                }
            }
            return list;
        }
        public static ResultReturn updateStatusFileUpload(string fileId)
        {
            return updateStatusFileUpload(fileId, 1); 
        }
        private static bool IsAcceptedFileType(string fileName, string[] extention)
        {
            bool result = false;
            string fileExtention = System.IO.Path.GetExtension(fileName).ToLower();
            foreach (string item in extention) {
                if (fileExtention == item)
                    return true;
            }
            return result;
        }
        private static ResultReturn executeUploadImage(string fileName, string filePath)
        {
            ResultReturn item = new ResultReturn();
            var request = HttpContext.Current.Request;
            string url = HttpContext.Current.Server.MapPath(CommonConstants.UPLOAD_DIRECT_STRING);
            string urlServer = string.Format("{0}://{1}{2}", request.Url.Scheme, request.Url.Authority, (new System.Web.Mvc.UrlHelper(request.RequestContext)).Content("~")) + "/FileUploads/";
            try
            {
                if (!Directory.Exists(url + filePath))
                {
                    Directory.CreateDirectory(url + filePath);
                }
            }
            catch (Exception)
            {

            }
            string dirUrl = filePath + "/image";
            string dirPath = url + dirUrl;
            try
            {
                if (!Directory.Exists(dirPath))
                {
                    Directory.CreateDirectory(dirPath);
                }
                item.msg = dirPath + "/" + fileName;
                item.code = urlServer + dirUrl + "/" + fileName;
            }
            catch (Exception ex)
            {
                item.code = ex.Message;
            }
            return item;
        }
        private static ResultReturn updateStatusFileUpload(string fileId, int flat)
        {
            ResultReturn result = new ResultReturn();
            string constr = ConfigurationManager.ConnectionStrings[CommonConstants.CONNECTION_STRING].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand(CommonConstants.STORED_PROCEDURE_UPDATE_FILE_UPLOAD))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Connection = con;
                    cmd.Parameters.Add("@FILEID", SqlDbType.Int).Value = fileId;
                    cmd.Parameters.Add("@FLAT", SqlDbType.Int).Value = flat;

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
        #region "Test"
        public static ResultReturn insertFileUpload_12072019(string fileName, string username)
        {
            ResultReturn result = new ResultReturn();
            string constr = ConfigurationManager.ConnectionStrings[CommonConstants.CONNECTION_STRING].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand("uspWeb_FileUpload_Insert12072019"))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Connection = con;
                    cmd.Parameters.Add("@FILENAME", SqlDbType.NVarChar).Value = fileName;
                    cmd.Parameters.Add("@USERUPLOAD", SqlDbType.VarChar).Value = username;

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
        public static List<ImageInfo> loadDataImageUpload(string fileId)
        {
            List<ImageInfo> list = new List<ImageInfo>();
            int index = 1;
            string constr = ConfigurationManager.ConnectionStrings[CommonConstants.CONNECTION_STRING].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand(CommonConstants.STORED_PROCEDURE_IMAGE_VIEW))
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
                                ImageInfo info = new ImageInfo();
                                info.imageUrl = sdr["IMGURL"].ToString();
                                info.uploadDate = sdr["DATEUPLOAD"] == DBNull.Value ? "" : Convert.ToDateTime(sdr["DATEUPLOAD"]).ToString("dd/MM/yyyy");
                                info.imageId = sdr["IMGID"].ToString() + ";" + sdr["FILEID"].ToString();
                                info.imageOrig = sdr["ORIG_IMG"].ToString();
                                info.fileId = index.ToString();
                                list.Add(info);
                                index++;
                            }
                        }
                    }
                    catch (Exception)
                    {
                        list = new List<ImageInfo>();
                    }
                }
            }
            return list;
        }
        public static InfoReturn loadInfoFileUpload_Test(string fileId)
        {
            InfoReturn result = new InfoReturn();
            string constr = ConfigurationManager.ConnectionStrings[CommonConstants.CONNECTION_STRING].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand(CommonConstants.STORED_PROCEDURE_LOAD_INFO_FILE_UPLOAD_TEST))
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
                                result.verify = sdr["VERIFY"].ToString();
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
        public static List<ItemSellOut> listDataTempFileUpload_Test(string fileId, string status)
        {
            List<ItemSellOut> list = new List<ItemSellOut>();
            string constr = ConfigurationManager.ConnectionStrings[CommonConstants.CONNECTION_STRING].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand(CommonConstants.STORED_PROCEDURE_GET_LIST_DATA_FILE_UPLOAD))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Connection = con;
                    cmd.Parameters.Add("@FILEID", SqlDbType.Int).Value = fileId;
                    if(status != null && status != "")
                        cmd.Parameters.Add("@STATUSID", SqlDbType.Int).Value = status;
                    con.Open();
                    try
                    {
                        using (SqlDataReader sdr = cmd.ExecuteReader())
                        {
                            while (sdr.Read())
                            {
                                ItemSellOut info = new ItemSellOut();
                                info.barcode = sdr["BARCODE"].ToString();
                                info.cusAddress = sdr["CUSADDRESS"].ToString();
                                info.cusEmail = sdr["CUSEMAIL"].ToString();
                                info.cusName = sdr["CUSNAME"].ToString();
                                info.cusPhone = sdr["CUSPHONE"].ToString();
                                info.id = sdr["STATUSID"].ToString();
                                info.purchaseDate = sdr["POD"] == DBNull.Value ? "" : Convert.ToDateTime(sdr["POD"]).ToString("dd/MM/yyyy");
                                info.status = sdr["REMARKS"].ToString();
                                info.model = sdr["MODELTYPE"].ToString();
                                list.Add(info);
                            }
                        }
                    }
                    catch (Exception)
                    {
                        list = new List<ItemSellOut>();
                    }
                }
            }
            return list;
        }
        public static ResultReturn completeUploadEW_Test(string fileId)
        {
            ResultReturn result = new ResultReturn();
            string constr = ConfigurationManager.ConnectionStrings[CommonConstants.CONNECTION_STRING].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand(CommonConstants.SQL_TEXT_GET_LIST_ID_EXCEL))
                {
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.Connection = con;
                    cmd.Parameters.Add("@FILEID", SqlDbType.Int).Value = fileId;
                    con.Open();
                    try
                    {
                        using (SqlDataReader sdr = cmd.ExecuteReader())
                        {
                            while (sdr.Read())
                            {
                                string id = sdr["ID"].ToString();
                                processItem(id);
                            }
                            updateStatusFileUpload_Test(fileId);
                            result.code = CommonConstants.EXECUTE_SUCCESS;
                            result.msg = "Hoàn tất";
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
        private static void updateStatusFileUpload_Test(string fileId)
        {
            string constr = ConfigurationManager.ConnectionStrings[CommonConstants.CONNECTION_STRING].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand(CommonConstants.STORED_PROCEDURE_FILE_UPLOAD_UPDATE))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Connection = con;
                    cmd.Parameters.Add("@FILEID", SqlDbType.Int).Value = fileId;
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
        private static void processItem(string id)
        {
            string constr = ConfigurationManager.ConnectionStrings[CommonConstants.CONNECTION_STRING].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand(CommonConstants.STORED_PROCEDURE_COMPLETED_1ROW))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Connection = con;
                    cmd.Parameters.Add("@ID", SqlDbType.Int).Value = id;
                    con.Open();
                    try
                    {
                        using (SqlDataReader sdr = cmd.ExecuteReader())
                        {
                            while (sdr.Read())
                            {
                                string ewCardId = sdr["EWCARDID"].ToString();
                                if(ewCardId != "0")
                                    FiboSMS.GetInfoEW(ewCardId);
                            }
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
            }
        }
        #endregion
        #region "Test Upload"
        public static ResultReturn insertOneRow12072019(string cusName, string cusPhone, string brandCode, string cusAddress, string barcode, string model, string pod, string fileId)
        {
            ResultReturn result = new ResultReturn();
            string constr = ConfigurationManager.ConnectionStrings[CommonConstants.CONNECTION_STRING].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand("uspWeb_FileUpload_Execute1Row08082019"))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Connection = con;
                    cmd.Parameters.Add("@CUSNAME", SqlDbType.NVarChar).Value = cusName == null ? "" : cusName.Trim().ToUpper();
                    cmd.Parameters.Add("@CUSADDRESS", SqlDbType.NVarChar).Value = cusAddress == null ? "" : cusAddress.Trim().ToUpper();
                    cmd.Parameters.Add("@CUSPHONE", SqlDbType.VarChar).Value = cusPhone == null ? "" : cusPhone.Trim();
                    cmd.Parameters.Add("@BARCODE", SqlDbType.VarChar).Value = barcode == null ? "" : barcode.Trim().ToUpper();
                    cmd.Parameters.Add("@MODELTYPE", SqlDbType.VarChar).Value = model == null ? "" : model.Trim().ToUpper();
                    cmd.Parameters.Add("@FILEID", SqlDbType.Int).Value = fileId;
                    cmd.Parameters.Add("@BRANDCODE", SqlDbType.VarChar).Value = brandCode;
                    if (pod != null)
                        if (!"".Equals(pod.Trim()))
                        {
                            string newDate = Common.stringToFormatYYYYMMDD(pod.Trim());
                            cmd.Parameters.Add("@PURCHASEDATE", SqlDbType.VarChar).Value = newDate;
                        }
                    con.Open();
                    try
                    {
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
        public static ItemReturn CheckInstallmentBarCode(string barcode)
        {
            ItemReturn result = new ItemReturn();
            string constr = ConfigurationManager.ConnectionStrings[CommonConstants.CONNECTION_STRING].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand("uspWeb_Installment_CheckBarcode"))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Connection = con;
                    cmd.Parameters.Add("@BARCODE", SqlDbType.VarChar).Value = barcode == null ? "" : barcode.Trim().ToUpper();
                    con.Open();
                    try
                    {
                        using (SqlDataReader sdr = cmd.ExecuteReader())
                        {
                            while (sdr.Read())
                            {
                                string returnId = sdr["SerialNumber"].ToString();
                                result.code = returnId;
                                result.message = sdr["MODELTYPE"].ToString();
                                result.data = sdr["RETURNMSG"].ToString();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        result.code = CommonConstants.EXECUTE_UNSUCCESS;
                        result.message = ex.Message;
                    }
                }
            }
            return result;
        }
        #endregion
    }
}