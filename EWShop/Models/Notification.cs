using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace EWShop.Models
{
    public class Notification
    {
        public string NId { get; set; }
        public string NTitle { get; set; }
        public string NContent { get; set; }
        public string isActive { get; set; }
        public string beginDate { get; set; }
        public string endDate { get; set; }
        public string remarks { get; set; }
    }
    public static class NotificationLogics
    {
        public static List<Notification> getListNotification(string month)
        {
            List<Notification> result = new List<Notification>();
            string constr = ConfigurationManager.ConnectionStrings[CommonConstants.CONNECTION_STRING].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand("uspWeb_Notification_GetList"))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    if (month != null)
                        if ("".Equals(month.Trim()))
                            cmd.Parameters.Add("@MONTH", SqlDbType.Int).Value = month.Trim();
                    cmd.Connection = con;
                    con.Open();
                    try
                    {
                        using (SqlDataReader sdr = cmd.ExecuteReader())
                        {
                            while (sdr.Read())
                            {
                                Notification item = new Notification();
                                item.NId =  sdr["NID"].ToString();
                                item.NTitle = sdr["NID"].ToString() + "," + sdr["NTITLE"].ToString();
                                item.NContent = sdr["NCONTENT"].ToString();
                                item.beginDate = sdr["BEGINDATE"] == DBNull.Value ? "" : Convert.ToDateTime(sdr["BEGINDATE"]).ToString("dd/MM/yyyy");
                                item.endDate = sdr["ENDDATE"] == DBNull.Value ? "" : Convert.ToDateTime(sdr["ENDDATE"]).ToString("dd/MM/yyyy");
                                item.isActive =  sdr["ISACTIVE"].ToString();
                                item.remarks = sdr["REMARKS"].ToString();
                                result.Add(item);
                            }
                        }
                    }
                    catch (Exception)
                    {
                        result = new List<Notification>();
                    }
                }
            }
            return result;
        }
    }
}