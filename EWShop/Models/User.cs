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
    public class User
    {
        public string userId { get; set; }
        public string userName { get; set; }
        public string roleId { get; set; }
        public string roleName { get; set; }
        public string avatar { get; set; }
        public string fullName { get; set; }
        public string userPhone { get; set; }
        public string userEMail { get; set; }
        public string userAddress { get; set; }
        public string shopId { get; set; }
        public string shopName { get; set; }
        public string shopCode { get; set; }
        public string shopAddress { get; set; }
        public string active { get; set; }
        public string remarks { get; set; }
    }
    public class UserInfo
    {
        public string userId { get; set; }
        public string userName { get; set; }
        public string fullName { get; set; }
        public string role { get; set; }
        public string accessToken { get; set; }
    }
    public class UserLogin
    {
        public string userName { get; set; }
        public string userPass { get; set; }
    }
    public class UserChangePasss
    {
        public string oldPass { get; set; }
        public string newPass { get; set; }
        public string confirmPass { get; set; }
    }
    public static class UserLogic
    {
        public static User GetUserInfo(string userName)
        {
            User us = new User();
            string constr = ConfigurationManager.ConnectionStrings[CommonConstants.CONNECTION_STRING].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand(CommonConstants.STORED_PROCEDURE_GET_INFO_USER))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Connection = con;
                    cmd.Parameters.Add("@USERNAME", SqlDbType.VarChar).Value = userName;
                    con.Open();
                    try
                    {
                        using (SqlDataReader sdr = cmd.ExecuteReader())
                        {
                            while (sdr.Read())
                            {
                                us.userId = sdr["USERID"].ToString();
                                us.userName = sdr["USERNAME"].ToString();
                                us.fullName = sdr["FULLNAME"].ToString();
                                us.userAddress = sdr["USERADDRESS"].ToString();
                                us.userPhone = sdr["USERPHONE"].ToString();
                                us.userEMail = sdr["USEREMAIL"].ToString();
                                us.roleName = sdr["ROLENAME"].ToString();
                                us.avatar = sdr["AVATAR"].ToString();
                            }
                        }
                    }
                    catch (Exception)
                    {
                        us = null;
                    }
                }
            }
            return us;
        }
        public static ResultReturn changePass(string oldPass, string newPass, string confirmPass, string userName)
        {
            ResultReturn result = new ResultReturn();
            string oldPassEncrypt = Common.Encrypt(oldPass);
            if (CheckUserExist(userName, oldPassEncrypt))
            {
                if (newPass.Equals(confirmPass) && !"".Equals(newPass) && !"".Equals(confirmPass))
                {
                    string newPassEncrypt = Common.Encrypt(newPass);
                    result = updatePassword(userName, newPassEncrypt);
                }
                else
                {
                    result.code = CommonConstants.EXECUTE_UNSUCCESS;
                    result.msg = "Vui lòng nhập lại mật khẩu mới và mật khẩu xác nhận";
                }
            }
            else
            {
                result.code = CommonConstants.EXECUTE_UNSUCCESS;
                result.msg = "Không thể đổi mật khẩu";
            }
            return result;
        }
        public static ResultReturn resetPass(string userName, string newPass, string confirmPass)
        {
            ResultReturn result = new ResultReturn();
            if (newPass.Equals(confirmPass) && !"".Equals(newPass) && !"".Equals(confirmPass))
            {
                string newPassEncrypt = Common.Encrypt(newPass);
                result = updatePassword(userName, newPassEncrypt);
            }
            else
            {
                result.code = CommonConstants.EXECUTE_UNSUCCESS;
                result.msg = "Vui lòng nhập lại mật khẩu mới và mật khẩu xác nhận";
            }
            return result;
        }
        private static ResultReturn updatePassword(string userName, string newPassEncrypt)
        {
            ResultReturn result = new ResultReturn();
            string constr = ConfigurationManager.ConnectionStrings[CommonConstants.CONNECTION_STRING].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand(CommonConstants.STORED_PROCEDURE_CHANGE_PASSWORD))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Connection = con;
                    cmd.Parameters.Add("@USERNAME", SqlDbType.VarChar).Value = userName;
                    cmd.Parameters.Add("@PASSWORD", SqlDbType.VarChar).Value = newPassEncrypt;
                    con.Open();
                    try
                    {
                        using (SqlDataReader sdr = cmd.ExecuteReader())
                        {
                            while (sdr.Read())
                            {
                                result.code = sdr["RETURNID"].ToString();
                                result.msg = sdr["RETURNMESSAGE"].ToString();
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
        public static List<string> GetRoleForUser(string username)
        {
            List<string> result = new List<string>();
            string constr = ConfigurationManager.ConnectionStrings[CommonConstants.CONNECTION_STRING].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand(CommonConstants.STORED_PROCEDURE_GET_ROLES_FOR_USER))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Connection = con;
                    cmd.Parameters.Add("@USERNAME", SqlDbType.VarChar).Value = username;
                    con.Open();
                    try
                    {
                        using (SqlDataReader sdr = cmd.ExecuteReader())
                        {
                            while (sdr.Read())
                            {
                                result.Add(sdr["ROLENAME"].ToString());
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.Title = ex.Message;
                    }
                }
            }
            return result;
        }
        public static ResultReturn ValidateUser(string userName, string userPass)
        {
            ResultReturn result = new ResultReturn();

            string passEncrypt = Common.Encrypt(userPass);
            if (CheckUserExist(userName, passEncrypt))
            {
                result.code = CommonConstants.EXECUTE_SUCCESS;
                result.msg = "Đăng nhập thành công";
            }
            else
            {
                result.code = CommonConstants.EXECUTE_NODATA;
                result.msg = "Tên đăng nhập và mật khẩu không chính xác";
            }
            return result;
        }
        private static bool CheckUserExist(string userName, string passWord)
        {
            bool result = false;
            string constr = ConfigurationManager.ConnectionStrings[CommonConstants.CONNECTION_STRING].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand(CommonConstants.STORED_PROCEDURE_CHECK_USER_EXIST))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Connection = con;
                    cmd.Parameters.Add("@USERNAME", SqlDbType.VarChar).Value = userName;
                    cmd.Parameters.Add("@PASSWORD", SqlDbType.VarChar).Value = passWord;
                    con.Open();
                    try
                    {
                        using (SqlDataReader sdr = cmd.ExecuteReader())
                        {
                            while (sdr.Read())
                            {
                                string returnId = sdr["RETURNID"].ToString();
                                result = returnId == CommonConstants.EXECUTE_SUCCESS ? true : false;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.Title = ex.Message;
                    }
                }
            }
            return result;
        }
        public static List<Menus> GetListMainMenusForUser(string userName)
        {
            List<Menus> result = new List<Menus>();
            string constr = ConfigurationManager.ConnectionStrings[CommonConstants.CONNECTION_STRING].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand(CommonConstants.STORED_PROCEDURE_GET_LIST_MENU))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Connection = con;
                    cmd.Parameters.Add("@USERNAME", SqlDbType.VarChar).Value = userName;
                    cmd.Parameters.Add("@ISMAIN", SqlDbType.Bit).Value = Convert.ToBoolean(1);
                    con.Open();
                    try
                    {
                        using (SqlDataReader sdr = cmd.ExecuteReader())
                        {
                            while (sdr.Read())
                            {
                                Menus menu = new Menus();
                                menu.menuId = sdr["MENUID"].ToString();
                                menu.menuName = sdr["MENUNAME"].ToString();
                                menu.isMainMenu = sdr["ISMAINMENU"].ToString();
                                menu.mainMenuId = sdr["MAINMENUID"].ToString();
                                menu.menuUrl = sdr["MENUURL"].ToString() == "" ? "" : ConfigurationManager.AppSettings["urlDomain"] + '/' + sdr["MENUURL"].ToString();
                                menu.remarks = sdr["REMARKS"].ToString();
                                menu.iconMenu = sdr["ICONMENU"].ToString();
                                menu.menuShortName = sdr["MENUSHORTNAME"].ToString();
                                result.Add(menu);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.Title = ex.Message;
                    }
                }
            }
            return result;
        }
        public static List<Menus> GetListChildMenusForUser(string userName)
        {
            List<Menus> result = new List<Menus>();
            string constr = ConfigurationManager.ConnectionStrings[CommonConstants.CONNECTION_STRING].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand(CommonConstants.STORED_PROCEDURE_GET_LIST_MENU))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Connection = con;
                    cmd.Parameters.Add("@USERNAME", SqlDbType.VarChar).Value = userName;
                    cmd.Parameters.Add("@ISMAIN", SqlDbType.Bit).Value = Convert.ToBoolean(0);
                    con.Open();
                    try
                    {
                        using (SqlDataReader sdr = cmd.ExecuteReader())
                        {
                            while (sdr.Read())
                            {
                                Menus menu = new Menus();
                                menu.menuId = sdr["MENUID"].ToString();
                                menu.menuName = sdr["MENUNAME"].ToString();
                                menu.isMainMenu = sdr["ISMAINMENU"].ToString();
                                menu.mainMenuId = sdr["MAINMENUID"].ToString();
                                menu.menuUrl = ConfigurationManager.AppSettings["urlDomain"] + '/' + sdr["MENUURL"].ToString();
                                menu.remarks = sdr["REMARKS"].ToString();
                                menu.iconMenu = sdr["ICONMENU"].ToString();
                                menu.menuShortName = sdr["MENUSHORTNAME"].ToString();
                                result.Add(menu);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.Title = ex.Message;
                    }
                }
            }
            return result;
        }
        public static UserInfo findUserByAccessToken(string accessToken)
        {
            UserInfo us = new UserInfo();
            string constr = ConfigurationManager.ConnectionStrings[CommonConstants.CONNECTION_STRING].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand(CommonConstants.STORED_PROCEDURE_FIND_USER))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Connection = con;
                    cmd.Parameters.Add("@ACCESSTOKEN", SqlDbType.VarChar).Value = accessToken;
                    con.Open();
                    try
                    {
                        using (SqlDataReader sdr = cmd.ExecuteReader())
                        {
                            while (sdr.Read())
                            {
                                int index = Convert.ToInt32(sdr["RETURNID"].ToString());
                                if (index > 0)
                                {
                                    us.userName = sdr["USERNAME"].ToString();
                                    us.userId = sdr["USERID"].ToString();
                                    us.role = sdr["ROLE"].ToString();
                                    us.accessToken = sdr["ACCESS_TOKEN"].ToString();
                                    us.fullName = sdr["FULLNAME"].ToString();
                                }
                                else
                                {
                                    us = null;
                                }
                            }
                        }
                    }
                    catch (Exception)
                    {
                        us = null;
                    }
                }
            }
            return us;
        }
    }
}