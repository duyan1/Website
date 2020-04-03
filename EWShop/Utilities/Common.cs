using EWShop.ExecuteWebserviceMWG;
using EWShop.LoginWebserviceMWG;
using EWShop.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace EWShop.Utilities
{
    public static class Common
    {
        static string key { get; set; } = "123456789QWERTYUIOPASDFGHJKLZXCVBNM!@#$%^&*()";
        static string[] arrayMonthString = { "jan","feb","mar","apr","may","jun","jul","aug","sep","oct","nov","dec" };
        static string[] arrayMonthInteger = { "01", "02", "03", "04", "05", "06", "07", "08", "09", "10", "11", "12" };
        public static bool checkSpecialCharacters(string str)
        {
            var regexItem = new Regex("^[a-zA-Z0-9 ]*$");

            if (regexItem.IsMatch(str)) {
                return true;
            }
            return false;
        }
        public static bool validatePhoneNumber(string number)
        {
            return Regex.Match(number, "^-*[0-9,\\.?\\-?\\(?\\)?\\ ]+$").Success;
        }
        public static bool checkValidatePhone(string cusPhone)
        {
            bool result = true;
            string constr = ConfigurationManager.ConnectionStrings[CommonConstants.CONNECTION_STRING].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand(CommonConstants.STORED_PROCEDURE_CHECK_PHONE_VALIDATE))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add("@PHONE", SqlDbType.VarChar).Value = cusPhone == null ? "" : cusPhone.Trim();
                    cmd.Connection = con;
                    con.Open();
                    try
                    {
                        using (SqlDataReader sdr = cmd.ExecuteReader())
                        {
                            while (sdr.Read())
                            {
                                string code = sdr["CODE"].ToString();
                                if (code == CommonConstants.EXECUTE_UNSUCCESS)
                                {
                                    result = false;
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
        public static List<DataReturn> loadListModel(string plantId)
        {
            List<DataReturn> list = new List<DataReturn>();
            string constr = ConfigurationManager.ConnectionStrings[CommonConstants.CONNECTION_STRING].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand(CommonConstants.SQL_TEXT_GET_LIST_MODELS_BY_PLANT))
                {
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.Connection = con;
                    cmd.Parameters.Add("@PLANTID", SqlDbType.Int).Value = Convert.ToInt32(plantId);
                    con.Open();
                    try
                    {
                        using (SqlDataReader sdr = cmd.ExecuteReader())
                        {
                            while (sdr.Read())
                            {
                                DataReturn dt = new DataReturn();
                                dt.value = sdr["VALUE"].ToString();
                                dt.text = sdr["TEXT"].ToString();
                                list.Add(dt);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.Write(ex.Message);
                    }
                }
            }
            return list;
        }
        public static string convertPhone(string phone)
        {
            if (phone.Length > 2)
            {
                if ("8".Equals(phone[0]) && "4".Equals(phone[1]))
                {
                    phone = "0" + phone.Remove(0, 2);
                }
            }
            return phone;
        }
        public static bool validateName(string name)
        {
            return Regex.Match(name, @"[A-Za-z ÀÁÂÃÈÉÊÌÍÒÓÔÕÙÚĂĐĨŨƠàáâãèéêìíòóôõùúăđĩũơƯĂẠẢẤẦẨẪẬẮẰẲẴẶẸẺẼỀỀỂưăạảấầẩẫậắằẳẵặẹẻẽềềểỄỆỈỊỌỎỐỒỔỖỘỚỜỞỠỢỤỦỨỪễệỉịọỏốồổỗộớờởỡợụủứừỬỮỰỲỴÝỶỸửữựỳỵỷỹ]+$").Success;
        }
        public static string convertMonth(string month)
        {
            month = month.ToLower();
            for(int i = 0; i < arrayMonthString.Length; i ++)
            {
                if (month.Equals(arrayMonthString[i]))
                {
                    return arrayMonthInteger[i];
                }
            }
            return month;
        }
        public static DateTime convertStringWithFormatDDMMYYYYToDate(string date)
        {
            try
            {
                if (date.Length > 0)
                {
                    char[] stringSeparators = new char[] { '-', '.', '/', ' ' };
                    string[] array = date.Split(stringSeparators);
                    string year = array[2];
                    if (year.Count() == 2)
                    {
                        year = "20" + year;
                    }
                    return new DateTime(Convert.ToInt32(year), Convert.ToInt32(convertMonth(array[1])), Convert.ToInt32(array[0]));
                }
                return DateTime.Now;
            }
            catch (Exception)
            {
                return DateTime.Now;
            }
            
            
        }
        public static string stringToFormatYYYYMMDD(string date)
        {
            string result = "";
            try
            {
                string[] array = date.Split(new char[] { '-', '/', ' ' });
                string year = array[2];
                if (year.Count() == 2)
                {
                    year = "20" + year;
                }
                result = year + "-" + array[1] + "-" + array[0];
            }
            catch (Exception)
            {
                result = "";
            }
            return result;
        }
        public static string Encrypt(string text)
        {
            try
            {
                using (var md5 = new MD5CryptoServiceProvider())
                {
                    using (var tdes = new TripleDESCryptoServiceProvider())
                    {
                        tdes.Key = md5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
                        tdes.Mode = CipherMode.ECB;
                        tdes.Padding = PaddingMode.PKCS7;

                        using (var transform = tdes.CreateEncryptor())
                        {
                            byte[] textBytes = UTF8Encoding.UTF8.GetBytes(text);
                            byte[] bytes = transform.TransformFinalBlock(textBytes, 0, textBytes.Length);
                            return Convert.ToBase64String(bytes, 0, bytes.Length);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

        }
        public static string RandomNumber(int length)
        {
            Random rand = new Random();
            string pool = "0123456789";
            var builder = new StringBuilder();

            for (var i = 0; i < length; i++)
            {
                var c = pool[rand.Next(0, pool.Length)];
                builder.Append(c);
            }

            return builder.ToString();
        }
        public static bool IsValidEmail(string email)
        {
            if ("".Equals(email))
                return true;
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
        public static List<DataReturn> loadListMonth()
        {
            List<DataReturn> result = new List<DataReturn>();

            for (int i = 1; i <= 12; i++)
            {
                DataReturn item = new DataReturn();
                item.value = i.ToString();
                item.text = "Tháng " + i.ToString();
                result.Add(item);
            }
            return result;
        }
        public static List<DataReturn> getListPlants()
        {
            List<DataReturn> list = new List<DataReturn>();
            string constr = ConfigurationManager.ConnectionStrings[CommonConstants.CONNECTION_STRING].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand(CommonConstants.SQL_TEXT_GET_LIST_PLANTS))
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
                                dt.value = sdr["VALUE"].ToString();
                                dt.text = sdr["TEXT"].ToString();
                                list.Add(dt);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.Write(ex.Message);
                    }
                }
            }
            return list;
        }
        public static List<DataReturn> getListCustomers()
        {
            List<DataReturn> list = new List<DataReturn>();
            string constr = ConfigurationManager.ConnectionStrings[CommonConstants.CONNECTION_STRING].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand(CommonConstants.SQL_TEXT_GET_LIST_CUSTOMERS))
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
                                dt.value = sdr["VALUE"].ToString();
                                dt.text = sdr["TEXT"].ToString();
                                list.Add(dt);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.Write(ex.Message);
                    }
                }
            }
            return list;
        }
        public static List<DataReturn> loadListProvince()
        {
            List<DataReturn> list = new List<DataReturn>();
            string constr = ConfigurationManager.ConnectionStrings[CommonConstants.CONNECTION_STRING].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand(CommonConstants.SQL_TEXT_GET_LIST_PROVINCE))
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
                                dt.value = sdr["VALUE"].ToString();
                                dt.text = sdr["TEXT"].ToString();
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
        public static List<DataReturn> loadListDistrict(string provinceId)
        {
            if(provinceId == null || provinceId == "")
                provinceId = "0";
            List<DataReturn> list = new List<DataReturn>();
            string constr = ConfigurationManager.ConnectionStrings[CommonConstants.CONNECTION_STRING].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand(CommonConstants.SQL_TEXT_GET_LIST_DISTRICTS))
                {
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.Connection = con;
                    cmd.Parameters.Add("@PROVINCEID", SqlDbType.Int).Value = Convert.ToInt32(provinceId);
                    con.Open();
                    try
                    {
                        using (SqlDataReader sdr = cmd.ExecuteReader())
                        {
                            while (sdr.Read())
                            {
                                DataReturn dt = new DataReturn();
                                dt.value = sdr["VALUE"].ToString();
                                dt.text = sdr["TEXT"].ToString();
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
        public static List<DataReturn> loadListWard(string districtId)
        {
            if (districtId == null || districtId == "")
                districtId = "0";
            List<DataReturn> list = new List<DataReturn>();
            string constr = ConfigurationManager.ConnectionStrings[CommonConstants.CONNECTION_STRING].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand(CommonConstants.STORED_PROCEDURE_SERVICE_REQUEST_LIST_WARD))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Connection = con;
                    cmd.Parameters.Add("@DISTRICTID", SqlDbType.Int).Value = Convert.ToInt32(districtId);
                    con.Open();
                    try
                    {
                        using (SqlDataReader sdr = cmd.ExecuteReader())
                        {
                            while (sdr.Read())
                            {
                                DataReturn dt = new DataReturn();
                                dt.value = sdr["VALUE"].ToString();
                                dt.text = sdr["TEXT"].ToString();
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
        public static ItemResult ExecuteService(string url)
        {
            ItemResult result = new ItemResult();
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.KeepAlive = true;
                request.Method = "GET";
                request.ContentType = "text/html; charset=utf-8";

                WebHeaderCollection headerReader = request.Headers;
                headerReader.Add("Accept-Language", "vi-VN,vi;q=0.8");

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                response.GetResponseStream();
                response.Close();
                result.code = CommonConstants.EXECUTE_SUCCESS;
                result.message = "Thành công";
            }
            catch (Exception ex)
            {
                result.code = CommonConstants.EXECUTE_UNSUCCESS;
                result.message = ex.Message;
            }
            return result;
        }
        public static string SendEmail(string receiver, string subject, string body)
        {
            string cc = "NTT-Hau@aquavietnam.vn;Khoa.NguyenDang@aquavietnam.vn;cskh@aquavietnam.vn";
            if (receiver == "")
                return "";
            if (subject == "")
                return "";
            if (body == "")
                return "";
            try
            {
                var smtp = new SmtpClient
                {
                    Host = CommonConstants.HOST_MAIL,
                    Port = CommonConstants.PORT,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(CommonConstants.SENDER_EMAIL, CommonConstants.SENDER_PASS)
                };

                using (var mess = new MailMessage(CommonConstants.SENDER_EMAIL, receiver)
                {
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                })
                {
                    string[] arrayCC = cc.Split(';', ',', ' ');
                    foreach (string rc in arrayCC)
                    {
                        if (!receiver.Contains(rc))
                            mess.Bcc.Add(rc);
                    }
                    smtp.Send(mess);
                }
                return "Đã gửi mail thành công!";
            }
            catch(Exception ex)
            {
                return ex.Message;
            }
            

            
        }
        public static string ReplacePhone(string phone)
        {
            string telephone = "";
            try
            {
                if (phone.ToString()[0] != '0')
                {
                    telephone = '0' + phone.ToString();
                }
                else
                {
                    telephone = phone.ToString();
                }
            }
            catch (Exception)
            {
                telephone = "";
            }
            return telephone;
        }
        public static bool checkUsernameExist(string username, string phone)
        {
            bool result = true;
            string constr = ConfigurationManager.ConnectionStrings[CommonConstants.CONNECTION_STRING].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand("uspWeb_ACS_CheckUser"))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    if(!"".Equals(username))
                        cmd.Parameters.Add("@USERNAME", SqlDbType.VarChar).Value = username.Trim();
                    if (!"".Equals(phone))
                        cmd.Parameters.Add("@PHONE", SqlDbType.VarChar).Value = phone.Trim();
                    cmd.Connection = con;
                    con.Open();
                    try
                    {
                        using (SqlDataReader sdr = cmd.ExecuteReader())
                        {
                            while (sdr.Read())
                            {
                                string code = sdr["RETURNID"].ToString();
                                if ("1".Equals(code))
                                {
                                    result = false;
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
        #region "Call webservice"
        public static string callLoginResult()
        {
            string result = "";
            LoginSvcSoapClient client = new LoginSvcSoapClient();
            return client.Login(CommonConstants.USERNAME_MWG, CommonConstants.PASSWORD_MWG, ref result);
        }
        public static ResponseMessage executeQuotation(string authen, string mat, string model, string price, string mrp, string sdp, string currency)
        {
            AquaSvcSoapClient client = new AquaSvcSoapClient();
            List<Quotation> list = new List<Quotation>();
            if (mrp != "0")
            {
                Quotation item = new Quotation();
                item.ItemCode = mat;
                item.ItemName = model;
                item.Price = Convert.ToDecimal(price);
                item.SalePrice = Convert.ToDecimal(mrp);
                item.PriceType = 1;
                list.Add(item);
            }
            if (sdp != "0")
            {
                Quotation item = new Quotation();
                item.ItemCode = mat;
                item.ItemName = model;
                item.Price = Convert.ToDecimal(price);
                item.SalePrice = Convert.ToDecimal(sdp);
                item.PriceType = 2;
                list.Add(item);
            }
            return client.ExecuteQuotation(authen, list.ToArray());
        }
        public static ResponseMessage executePOAcknowledgement(string auth, POAcknowledgement objPOA)
        {
            AquaSvcSoapClient client = new AquaSvcSoapClient();
            return client.ExecutePOAcknowledgement(auth, objPOA);
        }
        public static ResponseMessage executeStock(string auth, List<Stock> objPOA)
        {
            AquaSvcSoapClient client = new AquaSvcSoapClient();
            return client.ExecuteStock(auth, objPOA.ToArray());
        }
        #endregion
        #region "AMI"
        public static string executeOutlet(string ShopCode, string ShopName, string Channel, string Account, string Area, string Region, string Province, string District, string Address, int Status)
        {
            string result = "";
            string constr = ConfigurationManager.ConnectionStrings[CommonConstants.AMI_CONNECTION_STRING].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (con)
                {
                    try
                    {
                        using (SqlCommand cmd = new SqlCommand("[Outlet.Import]"))
                        {
                            cmd.Connection = con;
                            cmd.CommandType = CommandType.StoredProcedure;

                            if (con.State == ConnectionState.Closed)
                                con.Open();

                            SqlParameter param = new SqlParameter();
                            param.ParameterName = "@dt";
                            param.SqlDbType = SqlDbType.Structured;
                            param.Value = generalDataTable(ShopCode, ShopName, Channel, Account, Area, Region, Province, District, Address, Status);
                            cmd.Parameters.Add(param);
                            cmd.ExecuteReader();
                        }

                    }
                    catch (Exception ex)
                    {
                        result = ex.Message;
                    }

                    if (con.State != ConnectionState.Closed)
                        con.Close();
                }
            }
            return result;

        }
        private static DataTable generalDataTable(string ShopCode, string ShopName, string Channel, string Account, string Area, string Region, string Province, string District, string Address, int Status)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("ShopCode", typeof(string));
            dt.Columns.Add("ShopName", typeof(string));
            dt.Columns.Add("Channel", typeof(string));
            dt.Columns.Add("Account", typeof(string));
            dt.Columns.Add("Area", typeof(string));
            dt.Columns.Add("Region", typeof(string));
            dt.Columns.Add("Province", typeof(string));
            dt.Columns.Add("District", typeof(string));
            dt.Columns.Add("Latitude", typeof(string));
            dt.Columns.Add("Longitude", typeof(string));
            dt.Columns.Add("Address", typeof(string));
            dt.Columns.Add("NoofHouse", typeof(string));
            dt.Columns.Add("Street", typeof(string));
            dt.Columns.Add("Village", typeof(string));
            dt.Columns.Add("Grade", typeof(string));
            dt.Columns.Add("SaleRepName", typeof(string));
            dt.Columns.Add("SaleRepPhone", typeof(string));
            dt.Columns.Add("StatusMaster", typeof(int));

            dt.Rows.Add(ShopCode, ShopName, Channel, Account, Area, Region, Province, District, "", "", Address, "", "", "", "", "", "", Status);
            return dt;
        }
        public static string executeSyncOutletStatus(string shopCode, string status)
        {
            string result = "";
            string constr = ConfigurationManager.ConnectionStrings[CommonConstants.AMI_CONNECTION_STRING].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand("[Sync_Outlet_Status]"))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@ShopCode", SqlDbType.VarChar).Value = shopCode.Trim();
                    if (status != null & !"".Equals(status))
                        cmd.Parameters.Add("@status", SqlDbType.Bit).Value = Convert.ToInt32(status);
                    else
                        cmd.Parameters.Add("@status", SqlDbType.Bit).Value = Convert.ToInt32("0");
                    cmd.Connection = con;
                    con.Open();
                    try
                    {
                        cmd.ExecuteReader();
                    }
                    catch (Exception ex)
                    {
                        result = ex.Message;
                    }
                }
            }
            return result;
        }
        #endregion
    }
}