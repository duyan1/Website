using EWShop.Areas.Admin.Models;
using EWShop.Models;
using EWShop.Utilities;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;

namespace EWShop.Areas.Admin.Logics
{
    public class MasterDataLogics
    {
        public List<DataReturn> getListProvinces()
        {
            List<DataReturn> ds = new List<DataReturn>();
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
                                ds.Add(dt);
                            }
                        }
                    }
                    catch (Exception)
                    {
                        ds = new List<DataReturn>();
                    }
                }
            }
            return ds;
        }
        public List<DataReturn> getListDistricts(string provinceId)
        {
            List<DataReturn> ds = new List<DataReturn>();
            string constr = ConfigurationManager.ConnectionStrings[CommonConstants.CONNECTION_STRING].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand(CommonConstants.SQL_TEXT_GET_LIST_DISTRICTS))
                {
                    cmd.CommandType = CommandType.Text;
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
                                ds.Add(dt);
                            }
                        }
                    }
                    catch (Exception)
                    {
                        ds = new List<DataReturn>();
                    }
                }
            }
            return ds;
        }
        public List<ShopItem> getListShops(string shopType, string group, string region, string area, string active)
        {
            List<ShopItem> list = new List<ShopItem>();
            string constr = ConfigurationManager.ConnectionStrings[CommonConstants.CONNECTION_STRING].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand(CommonConstants.STORED_PROCEDURE_GET_LIST_SHOP))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Connection = con;
                    if (shopType != null && shopType != "")
                        cmd.Parameters.Add("@SHOPTYPE", SqlDbType.Int).Value = Convert.ToInt32(shopType);
                    if (group != null && group != "")
                        cmd.Parameters.Add("@GROUPID", SqlDbType.Int).Value = Convert.ToInt32(group);
                    if (region != null && region != "")
                        cmd.Parameters.Add("@REGIONID", SqlDbType.Int).Value = Convert.ToInt32(region);
                    if (area != null && area != "")
                        cmd.Parameters.Add("@AREA", SqlDbType.VarChar).Value = area;
                    if (active != null && active != "")
                        cmd.Parameters.Add("@ACTIVE", SqlDbType.Int).Value = active;
                    con.Open();
                    try
                    {
                        using (SqlDataReader sdr = cmd.ExecuteReader())
                        {
                            while (sdr.Read())
                            {
                                ShopItem dt = new ShopItem();
                                dt.shopId = sdr["SHOPID"].ToString();
                                dt.shopType = sdr["SHOPTYPE"].ToString();
                                dt.shopGroup = sdr["SHOPGROUP"].ToString();
                                dt.shopLevel = sdr["SHOPLEVEL"].ToString();
                                dt.province = sdr["PROVINCENAME"].ToString();
                                dt.district = sdr["DISTRICTNAME"].ToString();
                                dt.region = sdr["REGION"].ToString();
                                dt.area = sdr["AREA"].ToString();
                                dt.groupCode = sdr["GROUPCODE"].ToString();
                                dt.groupName = sdr["GROUPNAME"].ToString();
                                dt.shopCode = sdr["SHOPCODE"].ToString();
                                dt.shopName = sdr["SHOPNAME"].ToString();
                                dt.shopAddress = sdr["SHOPADDRESS"].ToString();
                                dt.isActive = sdr["ISACTIVE"].ToString();
                                dt.transDate = sdr["TRANSDATE"].ToString();
                                dt.transUser = sdr["TRANSUSER"].ToString();
                                list.Add(dt);
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

        public ResultReturn activeUser(string userId, string active)
        {
            ResultReturn result = new ResultReturn();
            string constr = ConfigurationManager.ConnectionStrings[CommonConstants.CONNECTION_STRING].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand("uspWeb_User_Active"))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Connection = con;
                    cmd.Parameters.Add("@USERID", SqlDbType.Int).Value = userId;
                    cmd.Parameters.Add("@ACTIVE", SqlDbType.Bit).Value = Convert.ToInt32(active);
                    con.Open();
                    try
                    {
                        ; cmd.ExecuteReader();
                        result.code = "1";
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

        public ResultReturn saveUser(User user, string username)
        {
            ResultReturn result = new ResultReturn();
            string constr = ConfigurationManager.ConnectionStrings[CommonConstants.CONNECTION_STRING].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand(CommonConstants.STORED_PROCEDURE_SAVE_USER))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Connection = con;
                    cmd.Parameters.Add("@USERID", SqlDbType.Int).Value = user.userId == null ? "0" : user.userId.Trim();
                    cmd.Parameters.Add("@USERNAME", SqlDbType.VarChar).Value = user.userName.Trim();

                    if (user.fullName != null && !"".Equals(user.fullName))
                        cmd.Parameters.Add("@FULLNAME", SqlDbType.NVarChar).Value = user.fullName.Trim();
                    if (user.userAddress != null && !"".Equals(user.userAddress))
                        cmd.Parameters.Add("@USERADDRESS", SqlDbType.NVarChar).Value = user.userAddress.Trim();
                    if (user.userPhone != null && !"".Equals(user.userPhone))
                        cmd.Parameters.Add("@USERPHONE", SqlDbType.VarChar).Value = user.userPhone.Trim();
                    if (user.userEMail != null && !"".Equals(user.userEMail))
                        cmd.Parameters.Add("@USEREMAIL", SqlDbType.VarChar).Value = user.userEMail.Trim();
                    if (user.roleId != null && !"".Equals(user.roleId))
                        cmd.Parameters.Add("@ROLEID", SqlDbType.Int).Value = user.roleId.Trim();
                    if (user.shopId != null && !"".Equals(user.shopId))
                        cmd.Parameters.Add("@INCHANNELID", SqlDbType.Int).Value = user.shopId.Trim();

                    cmd.Parameters.Add("@TRANSUSER", SqlDbType.VarChar).Value = username.Trim();
                    con.Open();
                    try
                    {
                        using (SqlDataReader sdr = cmd.ExecuteReader())
                        {
                            while (sdr.Read())
                            {
                                result.code = sdr["RETURNID"].ToString(); ;
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
        public ResultReturn saveShopInfo(ShopItem shop, string username)
        {
            ResultReturn result = new ResultReturn();
            string error = checkDataInput(shop);
            if (error != "")
            {
                result.code = CommonConstants.EXECUTE_UNSUCCESS;
                result.msg = error;
                return result;
            }
            string constr = ConfigurationManager.ConnectionStrings[CommonConstants.CONNECTION_STRING].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand(CommonConstants.STORED_PROCEDURE_SAVE_SHOP))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Connection = con;
                    cmd.Parameters.Add("@SHOPID", SqlDbType.Int).Value = (shop.shopId == null || "".Equals(shop.shopId)) ? "0" : shop.shopId.Trim();
                    cmd.Parameters.Add("@SHOPCODE", SqlDbType.VarChar).Value = shop.shopCode == null ? "" : shop.shopCode.Trim();
                    cmd.Parameters.Add("@SHOPNAME", SqlDbType.NVarChar).Value = shop.shopName == null ? "" : shop.shopName.Trim().ToUpper();
                    cmd.Parameters.Add("@SHOPADDRESS", SqlDbType.NVarChar).Value = shop.shopAddress == null ? "" : shop.shopAddress.Trim().ToUpper();
                    cmd.Parameters.Add("@SHOPLEVEL", SqlDbType.VarChar).Value = shop.shopLevel == null ? "" : shop.shopLevel.Trim();
                    if (shop.shopGroup != null)
                        if (!"".Equals(shop.shopGroup.Trim()))
                            cmd.Parameters.Add("@SHOPGROUP", SqlDbType.Int).Value = Convert.ToInt32(shop.shopGroup.Trim());
                    if (shop.shopType != null)
                        if (!"".Equals(shop.shopType.Trim()))
                            cmd.Parameters.Add("@SHOPTYPE", SqlDbType.Int).Value = Convert.ToInt32(shop.shopType.Trim());
                    if (shop.groupId != null)
                        if (!"".Equals(shop.groupId.Trim()))
                            cmd.Parameters.Add("@GROUPID", SqlDbType.Int).Value = Convert.ToInt32(shop.groupId.Trim());
                    if (shop.province != null)
                        if (!"".Equals(shop.province.Trim()))
                            cmd.Parameters.Add("@PROVINCE", SqlDbType.Int).Value = Convert.ToInt32(shop.province.Trim());
                    if (shop.district != null)
                        if (!"".Equals(shop.district.Trim()))
                            cmd.Parameters.Add("@DISTRICT", SqlDbType.Int).Value = Convert.ToInt32(shop.district.Trim());
                    if (shop.isActive != null)
                        if (!"".Equals(shop.isActive.Trim()))
                            cmd.Parameters.Add("@ISACTIVE", SqlDbType.Bit).Value = Convert.ToInt32(shop.isActive.Trim());
                    if (shop.remarks != null)
                        if (!"".Equals(shop.remarks))
                            cmd.Parameters.Add("@REMARKS", SqlDbType.NVarChar).Value = shop.remarks.Trim();
                    if (shop.customerCode != null)
                        if (!"".Equals(shop.customerCode.Trim()))
                            cmd.Parameters.Add("@CUSTOMERCODE", SqlDbType.VarChar).Value = shop.customerCode.Trim();
                    cmd.Parameters.Add("@TRANSUSER", SqlDbType.VarChar).Value = username;
                    con.Open();
                    try
                    {
                        using (SqlDataReader sdr = cmd.ExecuteReader())
                        {
                            while (sdr.Read())
                            {
                                result.code = sdr["RETURNID"].ToString();
                                result.msg = sdr["RETURNMSG"].ToString();

                                string shopCode = sdr["SHOPCODE"].ToString();
                                string shopName = sdr["SHOPNAME"].ToString();
                                string channel = sdr["CHANNELTYPE"].ToString();
                                string area = sdr["AREA"].ToString();
                                string region = sdr["REGION"].ToString();
                                string province = sdr["PROVINCE"].ToString();
                                string district = sdr["DISTRICT"].ToString();
                                string address = sdr["SHOPADDRESS"].ToString();
                                string account = sdr["ACCOUNT"].ToString();
                                string status = shop.isActive;
                                int StatusMaster = 1;
                                if (status == null || "".Equals(status))
                                {
                                    status = "0";
                                    StatusMaster = 1;
                                }
                                else if ("1".Equals(status))
                                {
                                    status = "0";
                                    StatusMaster = 1;
                                }
                                else
                                {
                                    status = "1";
                                    StatusMaster = 0;
                                }
                                    
                                Common.executeOutlet(shopCode, shopName, channel, account, area, region, province, district, address, StatusMaster);
                                Common.executeSyncOutletStatus(shopCode, status);
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
        public ResultReturn ReadExcel(HttpPostedFileBase file, string username)
        {
            ResultReturn data = new ResultReturn();
            DataFormatter df = new DataFormatter();
            string sFileExtension = Path.GetExtension(file.FileName).ToLower();
            ISheet sheet = null;
            if (file.ContentLength <= 0)
            {
                data.code = "0";
                data.msg = "You uploaded an empty file";
                return data;
            }
            try
            {
                if (sFileExtension == ".xls")
                {
                    HSSFWorkbook hssfwb = new HSSFWorkbook(file.InputStream);
                    sheet = hssfwb.GetSheetAt(0);
                }
                else
                {
                    XSSFWorkbook hssfwb = new XSSFWorkbook(file.InputStream);
                    sheet = hssfwb.GetSheetAt(0);
                }
                IRow headerRow = sheet.GetRow(0);
                int cellCount = headerRow.LastCellNum;
                for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++)
                {
                    IRow row = sheet.GetRow(i);
                    if (row == null) continue;
                    if (row.Cells.All(d => d.CellType == CellType.Blank)) continue;
                    string shopCode = "";
                    if (row.GetCell(0) != null)
                        shopCode = row.GetCell(0).ToString();
                    string shopGroup = "";
                    if (row.GetCell(1) != null)
                        shopGroup = row.GetCell(1).ToString();
                    string groupCode = "";
                    if (row.GetCell(2) != null)
                        groupCode = row.GetCell(2).ToString();
                    string shopName = "";
                    if (row.GetCell(3) != null)
                        shopName = row.GetCell(3).ToString();
                    string shopLevel = "";
                    if (row.GetCell(4) != null)
                        shopLevel = row.GetCell(4).ToString();
                    string shopType = "";
                    if (row.GetCell(5) != null)
                        shopType = row.GetCell(5).ToString();
                    string province = "";
                    if (row.GetCell(6) != null)
                        province = row.GetCell(6).ToString();
                    string district = "";
                    if (row.GetCell(7) != null)
                        district = row.GetCell(7).ToString();
                    string shopAddress = "";
                    if (row.GetCell(8) != null)
                        shopAddress = row.GetCell(8).ToString();

                    if (groupCode != "" && shopName != "" && shopAddress != "")
                        insert1RowShop(shopCode, shopGroup, groupCode, shopName, shopLevel, shopType, province, district, shopAddress, username);
                }
                data.code = "1";
                data.msg = "Completed !!!";
            }
            catch (Exception ex)
            {
                data.code = "0";
                data.msg = ex.Message;
            }

            return data;
        }
        private void insert1RowShop(string shopCode, string shopGroup, string groupCode, string shopName, string shopLevel, string shopType, string province, string district, string shopAddress, string username)
        {
            string constr = ConfigurationManager.ConnectionStrings[CommonConstants.CONNECTION_STRING].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand("uspWeb_Channel_Insert1Row"))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Connection = con;
                    if (shopCode != null)
                        if (!"".Equals(shopCode))
                            cmd.Parameters.Add("@SHOPCODE", SqlDbType.VarChar).Value = shopCode.Trim();
                    cmd.Parameters.Add("@SHOPNAME", SqlDbType.NVarChar).Value = shopName == null ? "" : shopName.Trim().ToUpper();
                    cmd.Parameters.Add("@SHOPADDRESS", SqlDbType.NVarChar).Value = shopAddress == null ? "" : shopAddress.Trim().ToUpper();
                    cmd.Parameters.Add("@SHOPLEVEL", SqlDbType.VarChar).Value = shopLevel == null ? "" : shopLevel.Trim();
                    if (shopGroup != null)
                        if (!"".Equals(shopGroup.Trim()))
                            cmd.Parameters.Add("@SHOPGROUP", SqlDbType.VarChar).Value = shopGroup.Trim();
                    if (shopType != null)
                        if (!"".Equals(shopType.Trim()))
                            cmd.Parameters.Add("@SHOPTYPE", SqlDbType.VarChar).Value = shopType.Trim();
                    if (groupCode != null)
                        if (!"".Equals(groupCode.Trim()))
                            cmd.Parameters.Add("@GROUPCODE", SqlDbType.VarChar).Value = groupCode.Trim();
                    if (province != null)
                        if (!"".Equals(province.Trim()))
                            cmd.Parameters.Add("@PROVINCE", SqlDbType.NVarChar).Value = province.Trim();
                    if (district != null)
                        if (!"".Equals(district.Trim()))
                            cmd.Parameters.Add("@DISTRICT", SqlDbType.VarChar).Value = district.Trim();
                    cmd.Parameters.Add("@TRANSUSER", SqlDbType.VarChar).Value = username;
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
        private string checkDataInput(ShopItem shop)
        {
            if (shop.shopName == null || "".Equals(shop.shopName))
                return "ShopName is blank";
            if (shop.shopAddress == null || "".Equals(shop.shopAddress))
                return "ShopAddress is blank";
            if (shop.shopType == null || "".Equals(shop.shopType))
                return "ShopType is blank";
            if (shop.shopGroup == null || "".Equals(shop.shopGroup))
                return "Account is blank";
            if (shop.groupId == null || "".Equals(shop.groupId))
                return "Group is blank";
            if (shop.province == null || "".Equals(shop.province))
                return "Province is blank";
            if (shop.district == null || "".Equals(shop.district))
                return "District is blank";
            if ("0".Equals(shop.isActive))
                if (shop.remarks == null || "".Equals( shop.remarks))
                    return "Enter reason";
            return "";
        }
        public List<User> getListUsers(string regionId, string roleId)
        {
            List<User> list = new List<User>();
            string constr = ConfigurationManager.ConnectionStrings[CommonConstants.CONNECTION_STRING].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand(CommonConstants.STORED_PROCEDURE_GET_LIST_USER))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Connection = con;
                    if (regionId != null && regionId != "")
                        cmd.Parameters.Add("@REGIONID", SqlDbType.Int).Value = Convert.ToInt32(regionId);
                    if (roleId != null && roleId != "")
                        cmd.Parameters.Add("@ROLEID", SqlDbType.Int).Value = Convert.ToInt32(roleId);
                    con.Open();
                    try
                    {
                        using (SqlDataReader sdr = cmd.ExecuteReader())
                        {
                            while (sdr.Read())
                            {
                                User dt = new User();
                                dt.userId = sdr["USERID"].ToString();
                                dt.userName = sdr["USERNAME"].ToString();
                                dt.fullName = sdr["FULLNAME"].ToString();
                                dt.roleName = sdr["ROLENAME"].ToString();
                                dt.userAddress = sdr["USERADDRESS"].ToString();
                                dt.userPhone = sdr["USERPHONE"].ToString();
                                dt.userEMail = sdr["USEREMAIL"].ToString();
                                dt.shopId = sdr["INSHOPID"].ToString();
                                dt.shopName = sdr["CHANNELNAME"].ToString();
                                dt.shopCode = sdr["CHANNELCODE"].ToString();
                                dt.shopAddress = sdr["CHANNELADDRESS"].ToString();
                                dt.avatar = sdr["AVATAR"].ToString();
                                dt.active = sdr["ISACTIVE"].ToString();

                                list.Add(dt);
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
        public User getUser(string userId)
        {
            User dt = new User();
            string constr = ConfigurationManager.ConnectionStrings[CommonConstants.CONNECTION_STRING].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand(CommonConstants.STORED_PROCEDURE_GET_USER_SELECT))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Connection = con;
                    if (userId != null && userId != "")
                        cmd.Parameters.Add("@USERID", SqlDbType.Int).Value = Convert.ToInt32(userId);
                    con.Open();
                    try
                    {
                        using (SqlDataReader sdr = cmd.ExecuteReader())
                        {
                            while (sdr.Read())
                            {
                                dt.userAddress = sdr["USERADDRESS"].ToString();
                                dt.avatar = sdr["AVATAR"].ToString();
                                dt.userEMail = sdr["USEREMAIL"].ToString();
                                dt.fullName = sdr["FULLNAME"].ToString();
                                dt.userPhone = sdr["USERPHONE"].ToString();
                                dt.roleId = sdr["ROLEID"].ToString();
                                dt.roleName = sdr["ROLENAME"].ToString();
                                dt.shopAddress = sdr["CHANNELADDRESS"].ToString();
                                dt.shopCode = sdr["CHANNELCODE"].ToString();
                                dt.shopName = sdr["CHANNELNAME"].ToString();
                                dt.userId = sdr["USERID"].ToString();
                                dt.userName = sdr["USERNAME"].ToString();
                                dt.remarks = sdr["REMARKS"].ToString();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.Write(ex.Message);
                    }
                }
            }
            return dt;
        }
        public ShopItem getShop(string shopId)
        {
            ShopItem dt = new ShopItem();
            string constr = ConfigurationManager.ConnectionStrings[CommonConstants.CONNECTION_STRING].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand(CommonConstants.STORED_PROCEDURE_GET_SHOP))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Connection = con;
                    if (shopId != null && shopId != "")
                        cmd.Parameters.Add("@SHOPID", SqlDbType.Int).Value = Convert.ToInt32(shopId);
                    con.Open();
                    try
                    {
                        using (SqlDataReader sdr = cmd.ExecuteReader())
                        {
                            while (sdr.Read())
                            {
                                dt.shopId = sdr["SHOPID"].ToString();
                                dt.shopType = sdr["SHOPTYPE"].ToString();
                                dt.shopGroup = sdr["SHOPGROUP"].ToString();
                                dt.shopLevel = sdr["SHOPLEVEL"].ToString();
                                dt.province = sdr["PROVINCEID"].ToString();
                                dt.district = sdr["DISTRICTID"].ToString();
                                dt.region = sdr["REGION"].ToString();
                                dt.area = sdr["AREA"].ToString();
                                dt.groupId = sdr["GROUPID"].ToString();
                                dt.shopCode = sdr["SHOPCODE"].ToString();
                                dt.shopName = sdr["SHOPNAME"].ToString();
                                dt.shopAddress = sdr["SHOPADDRESS"].ToString();
                                dt.isActive = sdr["ISACTIVE"].ToString();
                                dt.customerCode = sdr["CUSTOMERCODE"].ToString();
                                dt.transDate = sdr["TRANSDATE"].ToString();
                                dt.transUser = sdr["TRANSUSER"].ToString();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.Write(ex.Message);
                    }
                }
            }
            return dt;
        }
        public List<DataReturn> getListShops()
        {
            List<DataReturn> list = new List<DataReturn>();
            string constr = ConfigurationManager.ConnectionStrings[CommonConstants.CONNECTION_STRING].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand(CommonConstants.SQL_TEXT_GET_LIST_CHANNELS))
                {
                    cmd.CommandType = CommandType.Text;
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

                    }
                }
            }
            return list;
        }
        public List<DataReturn> getListPlants()
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
        public List<DataReturn> getListModels()
        {
            List<DataReturn> list = new List<DataReturn>();
            string constr = ConfigurationManager.ConnectionStrings[CommonConstants.CONNECTION_STRING].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand(CommonConstants.SQL_TEXT_GET_LIST_MODELS))
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
        public List<DataReturn> getListRegions()
        {
            List<DataReturn> list = new List<DataReturn>();
            string constr = ConfigurationManager.ConnectionStrings[CommonConstants.CONNECTION_STRING].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand(CommonConstants.SQL_TEXT_GET_LIST_REGIONS))
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
        public List<DataReturn> getListRoles()
        {
            List<DataReturn> list = new List<DataReturn>();
            string constr = ConfigurationManager.ConnectionStrings[CommonConstants.CONNECTION_STRING].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand(CommonConstants.SQL_TEXT_GET_LIST_ROLES))
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
        public List<DataReturn> getChannelTypes()
        {
            List<DataReturn> list = new List<DataReturn>();
            string constr = ConfigurationManager.ConnectionStrings[CommonConstants.CONNECTION_STRING].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand(CommonConstants.SQL_TEXT_GET_LIST_CHANNEL_TYPES))
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
        public List<DataReturn> getShopGroups()
        {
            List<DataReturn> list = new List<DataReturn>();
            string constr = ConfigurationManager.ConnectionStrings[CommonConstants.CONNECTION_STRING].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand(CommonConstants.SQL_TEXT_GET_LIST_SHOP_GROUPS))
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
        public List<DataReturn> getChannelGroups()
        {
            List<DataReturn> list = new List<DataReturn>();
            string constr = ConfigurationManager.ConnectionStrings[CommonConstants.CONNECTION_STRING].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand(CommonConstants.SQL_TEXT_GET_LIST_CHANNEL_GROUPS))
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
        public List<DataReturn> getMaterials()
        {
            List<DataReturn> list = new List<DataReturn>();
            string constr = ConfigurationManager.ConnectionStrings[CommonConstants.CONNECTION_STRING].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand(CommonConstants.SQL_TEXT_GET_LIST_MATERIALS))
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
        public List<DataReturn> getListRewardStatus()
        {
            List<DataReturn> list = new List<DataReturn>();
            string constr = ConfigurationManager.ConnectionStrings[CommonConstants.CONNECTION_STRING].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand(CommonConstants.SQL_TEXT_GET_LIST_REWARD_STATUS))
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
        public List<DataReturn> getListPrograms()
        {
            List<DataReturn> list = new List<DataReturn>();
            string constr = ConfigurationManager.ConnectionStrings[CommonConstants.CONNECTION_STRING].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT PROID AS VALUE, PRONAME AS TEXT from TBLPROGRAM"))
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
    }
}