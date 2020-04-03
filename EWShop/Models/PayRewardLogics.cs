using EWShop.Utilities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;

namespace EWShop.Models
{
    public class RewardInfo
    {
        public string title { get; set; }
        public string total { get; set; }
        public string ratio { get; set; }
        public string check { get; set; }
        public string valid { get; set; }
        public string inValid { get; set; }
        public string provide { get; set; }
        public string perMistakes { get; set; }
        public string perReward { get; set; }
    }
    public class ProductPayWard
    {
        public string ewCardNo { get; set; }
        public string cusName { get; set; }
        public string cusPhone { get; set; }
        public string cusAddress { get; set; }
        public string model { get; set; }
        public string plantName { get; set; }
        public string barcode { get; set; }
        public string pod { get; set; }
        public string regDate { get; set; }
        public string shopCode { get; set; }
        public string shopName { get; set; }
        public string rsid { get; set; }
        public string rsName { get; set; }
        public string comment { get; set; }
        public string feadback { get; set; }
        public string flat { get; set; }
        public string active { get; set; }
    }
    public static class PayRewardLogics
    {
        public static List<ProductPayWard> getListEW(string proid, string status, string userName, string shop, string flat)
        {
            List<ProductPayWard> list = new List<ProductPayWard>();
            string constr = ConfigurationManager.ConnectionStrings[CommonConstants.CONNECTION_STRING].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand("uspWeb_PayReward_ListEW"))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@PROID", SqlDbType.Int).Value = proid;
                    cmd.Parameters.Add("@USERNAME", SqlDbType.VarChar).Value = userName;
                    cmd.Parameters.Add("@STATUS", SqlDbType.Int).Value = status;
                    if (!"".Equals(shop) && shop != null)
                        cmd.Parameters.Add("@SHOPID", SqlDbType.Int).Value = shop;
                    if(!"".Equals(flat) && flat != null)
                        cmd.Parameters.Add("@FLAT", SqlDbType.Int).Value = flat;
                    cmd.Connection = con;
                    con.Open();
                    try
                    {
                        using (SqlDataReader sdr = cmd.ExecuteReader())
                        {
                            while (sdr.Read())
                            {
                                ProductPayWard item = new ProductPayWard();
                                item.barcode = sdr["BARCODE"].ToString().ToUpper();
                                item.comment = sdr["COMMENT"].ToString();
                                item.cusAddress = sdr["CUSADDRESS"].ToString().ToUpper();
                                item.cusName = sdr["CUSNAME"].ToString().ToUpper();
                                item.cusPhone = sdr["CUSPHONE"].ToString();
                                
                                item.ewCardNo = sdr["EWCARDNO"].ToString();
                                item.model = sdr["MODELTYPE"].ToString();
                                item.plantName = sdr["PLANTNAMEVN"].ToString();

                                item.pod = sdr["POD"] == DBNull.Value ? "" : Convert.ToDateTime(sdr["POD"]).ToString("dd/MM/yyyy");
                                item.regDate = sdr["REGDATE"] == DBNull.Value ? "" : Convert.ToDateTime(sdr["REGDATE"]).ToString("dd/MM/yyyy");
                                item.feadback = sdr["FEEDBACK"].ToString();
                                item.flat = sdr["FLAT"].ToString();
                                if(item.flat == "0")
                                    if (!"".Equals(item.feadback))
                                        item.flat = "1";
                                item.rsid = sdr["RSID"].ToString();
                                item.rsName = sdr["RSVNNAME"].ToString();
                                if ("".Equals(item.comment))
                                    item.comment = item.rsName;
                                item.active = sdr["ISACTIVE"].ToString();
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
        public static ResultReturn updatePayReward(string ewCardNo, string status, string comment, string username, string proid)
        {
            ResultReturn result = new ResultReturn();
            string constr = ConfigurationManager.ConnectionStrings[CommonConstants.CONNECTION_STRING].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand("uspWeb_PayReward_UpdatePayReward"))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@EWCARDNO", SqlDbType.VarChar).Value = ewCardNo;
                    cmd.Parameters.Add("@PROID", SqlDbType.Int).Value = proid;
                    cmd.Parameters.Add("@COMMENT", SqlDbType.NVarChar).Value = comment;
                    cmd.Parameters.Add("@STATUS", SqlDbType.Int).Value = status;
                    cmd.Parameters.Add("@TRANSUSER", SqlDbType.VarChar).Value = username;
                    cmd.Connection = con;
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
        public static ResultReturn updateComment(string ewCardNo, string comment, string username)
        {
            ResultReturn result = new ResultReturn();
            string constr = ConfigurationManager.ConnectionStrings[CommonConstants.CONNECTION_STRING].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand("uspWeb_PayReward_UpdateComment"))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@EWCARDNO", SqlDbType.VarChar).Value = ewCardNo;
                    cmd.Parameters.Add("@PROID", SqlDbType.Int).Value = ConfigurationManager.AppSettings["programId"];
                    cmd.Parameters.Add("@COMMENT", SqlDbType.NVarChar).Value = comment;
                    cmd.Parameters.Add("@TRANSUSER", SqlDbType.VarChar).Value = username;
                    cmd.Connection = con;
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
        public static List<string> getListImages(string ewCardNo)
        {
            List<string> result = new List<string>();
            var request = HttpContext.Current.Request;
            string urlServer = string.Format("{0}://{1}{2}", request.Url.Scheme, request.Url.Authority, (new System.Web.Mvc.UrlHelper(request.RequestContext)).Content("~")) + "/Libraries/dist/img/no-image.png";
            string constr = ConfigurationManager.ConnectionStrings[CommonConstants.CONNECTION_STRING].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand("uspWeb_PayReward_GetListImage"))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@EWCARDNO", SqlDbType.VarChar).Value = ewCardNo;
                    cmd.Parameters.Add("@PROID", SqlDbType.Int).Value = ConfigurationManager.AppSettings["programId"];
                    cmd.Connection = con;
                    con.Open();
                    try
                    {
                        using (SqlDataReader sdr = cmd.ExecuteReader())
                        {
                            while (sdr.Read())
                            {
                                string url = sdr["IMGURL"].ToString();
                                result.Add(url);
                            }
                        }
                    }
                    catch (Exception)
                    {
                        
                    }
                }
            }
            if(result.Count == 1)
            {
                result.Add(urlServer);
                result.Add(urlServer);
            }else if(result.Count == 2)
            {
                result.Add(urlServer);
            }else if(result.Count == 0)
            {
                result.Add(urlServer);
                result.Add(urlServer);
                result.Add(urlServer);
            }
            return result;
        }
        public static ResultReturn removeImage(string ewCardNo, string id)
        {
            ResultReturn result = new ResultReturn();
            string constr = ConfigurationManager.ConnectionStrings[CommonConstants.CONNECTION_STRING].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand("uspWeb_PayReward_RemoveImage"))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@EWCARDNO", SqlDbType.VarChar).Value = ewCardNo;
                    cmd.Parameters.Add("@PROID", SqlDbType.Int).Value = ConfigurationManager.AppSettings["programId"];
                    cmd.Parameters.Add("@ID", SqlDbType.Int).Value = id;
                    cmd.Connection = con;
                    con.Open();
                    try
                    {
                        using (SqlDataReader sdr = cmd.ExecuteReader())
                        {
                            while (sdr.Read())
                            {
                                string url = sdr["FILEURL"].ToString();
                                removeImg(url);
                                result.code = CommonConstants.EXECUTE_SUCCESS;
                                result.msg = "";
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
        private static void removeImg(string url)
        {
            try
            {
                var request = HttpContext.Current.Request;
                string urlLocation = HttpContext.Current.Server.MapPath(CommonConstants.UPLOAD_DIRECT_STRING);
                string urlServer = string.Format("{0}://{1}{2}", request.Url.Scheme, request.Url.Authority, (new System.Web.Mvc.UrlHelper(request.RequestContext)).Content("~")) + "/FileUploads/";
                url = url.Replace(urlServer, urlLocation);
                if (File.Exists(url))
                    File.Delete(url);
            }
            catch (Exception)
            {

            }
        }
        public static string insertImageIntoDB(string fileName, string fileUrl, string ewCardNo, string username)
        {
            string result = "";
            string constr = ConfigurationManager.ConnectionStrings[CommonConstants.CONNECTION_STRING].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand("uspWeb_PayReward_InsertImage"))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@EWCARDNO", SqlDbType.VarChar).Value = ewCardNo;
                    cmd.Parameters.Add("@PROID", SqlDbType.Int).Value = ConfigurationManager.AppSettings["programId"];
                    cmd.Parameters.Add("@FILENAME", SqlDbType.NVarChar).Value = fileName;
                    cmd.Parameters.Add("@FILEURL", SqlDbType.NVarChar).Value = fileUrl;
                    cmd.Parameters.Add("@TRANSUSER", SqlDbType.VarChar).Value = username;
                    cmd.Connection = con;
                    con.Open();
                    try
                    {
                        using (SqlDataReader sdr = cmd.ExecuteReader())
                        {
                            while (sdr.Read())
                            {
                                int id = Convert.ToInt32(sdr["ID"]);
                                result = id > 0 ? "" : "Tác vụ thất bại";
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        result = ex.Message;
                    }
                }
            }
            return result;
        }
        public static List<ImageInfo> getListImageUpload(string ewCardNo)
        {
            List<ImageInfo> list = new List<ImageInfo>();
            string constr = ConfigurationManager.ConnectionStrings[CommonConstants.CONNECTION_STRING].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand("uspWeb_PayReward_ListImage"))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@EWCARDNO", SqlDbType.VarChar).Value = ewCardNo;
                    cmd.Parameters.Add("@PROID", SqlDbType.Int).Value = ConfigurationManager.AppSettings["programId"];
                    cmd.Connection = con;
                    con.Open();
                    try
                    {
                        using (SqlDataReader sdr = cmd.ExecuteReader())
                        {
                            while (sdr.Read())
                            {
                                ImageInfo item = new ImageInfo();
                                item.fileId = sdr["ID"].ToString();
                                item.imageUrl = sdr["IMGURL"].ToString();
                                item.imageOrig = sdr["FILENAME"].ToString();
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
        public static RewardInfo getRewardInfo(string userName, string programId)
        {
            RewardInfo result = new RewardInfo();
            string constr = ConfigurationManager.ConnectionStrings[CommonConstants.CONNECTION_STRING].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand("uspWeb_PayReward_GetInfo"))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@PROID", SqlDbType.Int).Value = programId;
                    cmd.Parameters.Add("@USERNAME", SqlDbType.VarChar).Value = userName;
                    cmd.Connection = con;
                    con.Open();
                    try
                    {
                        using (SqlDataReader sdr = cmd.ExecuteReader())
                        {
                            while (sdr.Read())
                            {
                                result.title = sdr["PRONAME"].ToString();
                                result.total = sdr["TOTAL"].ToString();
                                result.ratio = sdr["RATIO"].ToString() + "%";
                                result.check = sdr["CHECK"].ToString();

                                result.valid = sdr["VALID"].ToString();
                                result.inValid = sdr["INVALID"].ToString();
                                result.provide = sdr["PROVIDE"].ToString();

                                result.perMistakes = sdr["PERMISTAKES"].ToString() + "%";
                                result.perReward = sdr["PERREWARD"].ToString() + "%";
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
        public static List<DataReturn> getListPrograms(string username)
        {
            List<DataReturn> result = new List<DataReturn>();
            string constr = ConfigurationManager.ConnectionStrings[CommonConstants.CONNECTION_STRING].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand("uspWeb_PayReward_GetListPrograms"))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@USERNAME", SqlDbType.VarChar).Value = username;
                    cmd.Connection = con;
                    con.Open();
                    try
                    {
                        using (SqlDataReader sdr = cmd.ExecuteReader())
                        {
                            while (sdr.Read())
                            {
                                DataReturn item = new DataReturn();
                                item.value = sdr["PROID"].ToString();
                                item.text = sdr["PRONAME"].ToString();
                                result.Add(item);
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
    }
}