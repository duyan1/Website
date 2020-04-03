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
    public class SupportRequestLogic
    {
        public static ResultReturn RegisterSupportRequest(SupportRequest sr)
        {
            ResultReturn result = new ResultReturn();
            string constr = ConfigurationManager.ConnectionStrings[CommonConstants.CONNECTION_STRING].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand(CommonConstants.STORED_PROCEDURE_SUPPORT_REQUEST))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Connection = con;
                    cmd.Parameters.Add("@SenderName", SqlDbType.VarChar).Value = sr.SenderName;
                    cmd.Parameters.Add("@SenderAddress", SqlDbType.NVarChar).Value = sr.SenderAddress;
                    cmd.Parameters.Add("@SenderCoName", SqlDbType.NVarChar).Value = sr.SenderCoName;
                    cmd.Parameters.Add("@SenderPhone", SqlDbType.VarChar).Value = sr.SenderPhone;
                    cmd.Parameters.Add("@SenderFax", SqlDbType.NVarChar).Value = sr.SenderFax;
                    cmd.Parameters.Add("@SenderEmail", SqlDbType.VarChar).Value = sr.SenderEmail;
                    cmd.Parameters.Add("@MessageBody", SqlDbType.VarChar).Value = sr.MessageBody;
                    cmd.Parameters.Add("@SupportRequestType", SqlDbType.Int).Value = sr.SupportRequestType;
                    cmd.Parameters.Add("@MatID", SqlDbType.Int).Value = sr.MatID;
                    cmd.Parameters.Add("@ModelOther", SqlDbType.NVarChar).Value = sr.ModelOther;
                    
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
                                if(returnId.Equals("1"))
                                {
                                    string modelname = sdr["MODELNAME"].ToString();
                                    string supType = sdr["SUPPORTTYPENAME"].ToString();
                                    sendEmail(result.msg, sr, modelname, supType);
                                }
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
        public static List<DataReturn> loadListSPRequestType()
        {
            List<DataReturn> list = new List<DataReturn>();
            string constr = ConfigurationManager.ConnectionStrings[CommonConstants.CONNECTION_STRING].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand(CommonConstants.SQL_TEXT_GET_LIST_SUPPORTREQUESTTYPE))
                {
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.Connection = con;
                    con.Open();
                    try
                    {
                        using (SqlDataReader sdr = cmd.ExecuteReader())
                        {
                            while (sdr.Read())
                            {
                                DataReturn dt = new DataReturn();
                                dt.value = sdr["TypeID"].ToString();
                                dt.text = sdr["TypeName"].ToString();
                                list.Add(dt);
                            }
                        }
                    }
                    catch (Exception)
                    {
                        list = new List<DataReturn>();
                    }
                }
            }
            return list;
        }
        private static void sendEmail(string emailto, SupportRequest spr,string modelname, string suptypeName)
        {
            string titleEmail = "[AQUA]- LIÊN HỆ - GÓP Ý - HỖ TRỢ KỸ THUẬT";
            string bodyEmail = "";
            bodyEmail += "<b> Người gởi:</b>" + spr.SenderName +
                         "<br/><b> Email:  </b>" + spr.SenderEmail +
                         "<br/><b>Địa chỉ:  </b>" + spr.SenderAddress +
                         "<br/><b>Điện thoại:  </b>" + spr.SenderPhone +
                         "<br/><b>Fax:  </b>" + spr.SenderFax +
                         "<br/><b>Công ty:  </b>" + spr.SenderCoName +
                         "<br/><b>Danh mục câu hỏi:  </b>" + suptypeName +
                         "<br/><b>Loại sản phẩm:  </b>" + modelname +
                         //"<br/> Model:  <b>" + spr.SenderName + "</b>" +
                         //"<br/> Model:  <b>" + spr.SenderName + "</b>" +
                         "<br/> <p>" + spr.MessageBody + "</p>";
            Common.SendEmail(emailto, titleEmail, bodyEmail);
        }
    }
}