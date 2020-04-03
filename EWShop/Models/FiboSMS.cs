using EWShop.Models;
using EWShop.Utilities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web;

namespace EWShop.Models
{
    public static class FiboSMS
    {
        public static ItemResult GetInfoEW(string EWCardId)
        {
            ItemResult result = new ItemResult();
            string mobile = "";
            string content = "";
            string constr = ConfigurationManager.ConnectionStrings[CommonConstants.CONNECTION_STRING].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM vTBLEWarranty WHERE EWCARDID=@EWCARDID"))
                {
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.Connection = con;
                    cmd.Parameters.Add("@EWCARDID", SqlDbType.Int).Value = EWCardId;
                    con.Open();
                    try
                    {
                        using (SqlDataReader sdr = cmd.ExecuteReader())
                        {
                            while (sdr.Read())
                            {
                                string KM = sdr["MODELTYPE"].ToString();
                                string SM = sdr["BARCODE"].ToString();
                                string HBH = sdr["EXPIREDDATE"] == DBNull.Value ? "" : Convert.ToDateTime(sdr["EXPIREDDATE"]).ToString("dd/MM/yyyy");
                                string MBH = sdr["EWCARDNO"].ToString();
                                mobile = sdr["CUSPHONE"].ToString();
                                content = string.Format(getMessageContent(104), KM, SM, MBH, HBH);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }
            if(mobile.Trim() != "")
            {
                //mobile = "0366617801";
                string url = string.Format(CommonConstants.FIBO_URL, CommonConstants.CLIENT_NO, CommonConstants.CLIENT_PASS, CommonConstants.SENDER_NAME, mobile, content);
                result = Common.ExecuteService(url);
            }
            if(result.code == CommonConstants.EXECUTE_SUCCESS)
            {
                result = updateStatusSendSMSForEW(EWCardId);
            }
            return result;
        }
        private static ItemResult updateStatusSendSMSForEW(string eWCardId)
        {
            ItemResult result = new ItemResult();
            string constr = ConfigurationManager.ConnectionStrings[CommonConstants.CONNECTION_STRING].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand("uspWeb_EWarranty_UpdateStatusSMS"))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Connection = con;
                    cmd.Parameters.Add("@EWCARDID", SqlDbType.Int).Value = eWCardId;
                    con.Open();
                    try
                    {
                        cmd.ExecuteReader();
                        result.code = CommonConstants.EXECUTE_SUCCESS;
                        result.message = "";
                    }
                    catch (Exception ex)
                    {
                        result.code = CommonConstants.EXECUTE_UNSUCCESS;
                        result.message =  ex.Message;
                    }
                }
            }
            return result;
        }
        private static string getMessageContent(int level)
        {
            string result = "";
            string constr = ConfigurationManager.ConnectionStrings[CommonConstants.CONNECTION_STRING].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand("uspWeb_EWarranty_GetMessage"))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Connection = con;
                    cmd.Parameters.Add("@KEY", SqlDbType.Int).Value = level;
                    con.Open();
                    try
                    {
                        using (SqlDataReader sdr = cmd.ExecuteReader())
                        {
                            while (sdr.Read())
                            {
                                result = sdr["MSGCONTENT"].ToString();
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
    }
}