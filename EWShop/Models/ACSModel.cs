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
    public class ACSModel
    {
        public string fullname { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string icard { get; set; }
        public string phone { get; set; }
        public string email { get; set; }
        public string province { get; set; }
        public string district { get; set; }
        public string address { get; set; }
        public string bank { get; set; }
        public string branch { get; set; }
        public string account { get; set; }
        public string captcha { get; set; }
    }
    public static class ACSLogics
    {
        public static ResultReturn registerACS(ACSModel user)
        {
            ResultReturn result = new ResultReturn();
            string error = checkDataInput(user);
            if (!"".Equals(error))
            {
                result.code = CommonConstants.EXECUTE_UNSUCCESS;
                result.msg = error;
            }
            else
            {
                string constr = ConfigurationManager.ConnectionStrings[CommonConstants.CONNECTION_STRING].ConnectionString;
                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand("uspWeb_ACS_InsertUser"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.Add("@FULLNAME", SqlDbType.NVarChar).Value = user.fullname;
                        cmd.Parameters.Add("@USERNAME", SqlDbType.VarChar).Value = user.username;
                        cmd.Parameters.Add("@PASSWORD", SqlDbType.NVarChar).Value = Common.Encrypt(user.password);
                        cmd.Parameters.Add("@USERPHONE", SqlDbType.VarChar).Value = user.phone;
                        if(user.email != null && !"".Equals(user.email) && !"null".Equals(user.email))
                            cmd.Parameters.Add("@USEREMAIL", SqlDbType.NVarChar).Value = user.email;
                        cmd.Parameters.Add("@PROVINCE", SqlDbType.Int).Value = user.province;
                        cmd.Parameters.Add("@DISTRICT", SqlDbType.Int).Value = user.district;
                        cmd.Parameters.Add("@ADDRESS", SqlDbType.NVarChar).Value = user.address;
                        cmd.Parameters.Add("@ICARD", SqlDbType.VarChar).Value = user.icard;

                        if (user.bank != null && !"".Equals(user.bank) && !"null".Equals(user.bank))
                            cmd.Parameters.Add("@BANK", SqlDbType.NVarChar).Value = user.bank;
                        if (user.branch != null && !"".Equals(user.branch) && !"null".Equals(user.branch))
                            cmd.Parameters.Add("@BRANCH", SqlDbType.NVarChar).Value = user.branch;
                        if (user.account != null && !"".Equals(user.account) && !"null".Equals(user.account))
                            cmd.Parameters.Add("@ACCOUNT", SqlDbType.NVarChar).Value = user.account;
                        con.Open();
                        try
                        {
                            using (SqlDataReader sdr = cmd.ExecuteReader())
                            {
                                while (sdr.Read())
                                {
                                    result.code = sdr["RETURNID"].ToString();
                                    result.msg = sdr["RETURNMSG"].ToString();
                                    string code = sdr["CODE"].ToString();
                                    string mobile = sdr["MOBILE"].ToString();

                                    if ("1".Equals(result.code))
                                    {
                                        string content = getContent(code);
                                        string url = string.Format(CommonConstants.FIBO_URL, CommonConstants.CLIENT_NO, CommonConstants.CLIENT_PASS, CommonConstants.SENDER_NAME, mobile, content);
                                        ItemResult item = Common.ExecuteService(url);
                                        if (item.code == "1")
                                        {
                                            result.code = CommonConstants.EXECUTE_SUCCESS;
                                            result.msg = "";
                                        }else
                                        {
                                            result.code = CommonConstants.EXECUTE_UNSUCCESS;
                                            result.msg = item.message;
                                        }
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
            }
            return result;
        }
        private static string getContent(string code)
        {
            return "Ma xac nhan (het han sau 20 phut) cua Quy khach la " + code;
        }
        private static string checkDataInput(ACSModel user)
        {
            if (user.fullname == null || "".Equals(user.fullname) || "null".Equals(user.fullname))
                return "Vui lòng nhập họ tên đăng ký";
            if (user.username == null || "".Equals(user.username) || "null".Equals(user.username))
                return "Vui lòng nhập thông tin tên đăng nhập";
            if(!Common.checkUsernameExist(user.username,""))
                return "Tên đăng nhập đã tồn tại trong hệ thống";
            if (user.password == null || "".Equals(user.password))
                return "Vui lòng nhập thông tin mật khẩu";
            if (user.phone == null || "".Equals(user.phone))
                return "Vui lòng nhập thông tin số điện thoại";
            if(!Common.checkUsernameExist("",user.phone))
                return "Số điện thoại đã tồn tại trong hệ thống";
            if (!Common.checkValidatePhone(user.phone))
                return "Số điện thoại đăng ký không hợp lệ";
            if (user.email != null && !"".Equals(user.email) && !"null".Equals(user.email))
                if(!Common.IsValidEmail(user.email))
                    return "Email đăng ký không hợp lệ";
            if (user.icard == null || "".Equals(user.icard) || "null".Equals(user.icard))
                return "Vui lòng nhập thông tin chứng minh thư";
            if (user.address == null || "".Equals(user.address) || "null".Equals(user.address))
                return "Vui lòng nhập thông tin địa chỉ";
            return "";
        }
        public static ResultReturn sendOTP(string mobile, string otp)
        {
            ResultReturn result = new ResultReturn();
            string constr = ConfigurationManager.ConnectionStrings[CommonConstants.CONNECTION_STRING].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand("uspWeb_ACS_CheckOTP"))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Connection = con;
                    if(mobile != null && !"".Equals(mobile))
                        cmd.Parameters.Add("@MOBILE", SqlDbType.VarChar).Value = mobile;
                    if (otp != null && !"".Equals(otp))
                        cmd.Parameters.Add("@OTP", SqlDbType.VarChar).Value = otp;
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
    }
}