using EWShop.Areas.Sales.Models;
using EWShop.ExecuteWebserviceMWG;
using EWShop.Models;
using EWShop.Utilities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace EWShop.Areas.Sales.Logics
{
    public static class PurchaseOrderLogic
    {
        public static List<PurchaseOrder> getListRecords(string frmDate, string toDate, string confirm, string poNo)
        {
            List<PurchaseOrder> result = new List<PurchaseOrder>();
            string constr = ConfigurationManager.ConnectionStrings[CommonConstants.CONNECTION_STRING_MWG_AQUA].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand("sp_sales_listpurchaseorders"))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Connection = con;
                    if (frmDate != null && !"".Equals(frmDate))
                        cmd.Parameters.Add("@FRMDATE", SqlDbType.DateTime).Value = Common.convertStringWithFormatDDMMYYYYToDate(frmDate);
                    if (toDate != null && !"".Equals(toDate))
                        cmd.Parameters.Add("@TODATE", SqlDbType.DateTime).Value = Common.convertStringWithFormatDDMMYYYYToDate(toDate);
                    if (confirm != null && !"".Equals(confirm))
                        cmd.Parameters.Add("@ISCONFIRM", SqlDbType.Int).Value = Convert.ToInt32(confirm);
                    if (poNo != null && !"".Equals(poNo))
                        cmd.Parameters.Add("@PONO", SqlDbType.VarChar).Value = poNo;
                    con.Open();
                    try
                    {
                        using (SqlDataReader sdr = cmd.ExecuteReader())
                        {
                            while (sdr.Read())
                            {
                                PurchaseOrder dt = new PurchaseOrder();
                                dt.numberPO = sdr["purchase_order_no"].ToString();
                                dt.orderDate = sdr["order_date"] == DBNull.Value ? "" : Convert.ToDateTime(sdr["order_date"]).ToString("dd/MM/yyyy HH:mm");
                                dt.currency = sdr["currency"].ToString();
                                dt.createdPO = sdr["person_name_created"].ToString();
                                dt.createdEmail = sdr["person_email_created"].ToString();
                                dt.orderType = sdr["order_type"].ToString();
                                dt.soldTo = sdr["sold_to"].ToString();
                                dt.shipTo = sdr["ship_to"].ToString();
                                dt.confirmed = sdr["is_confirmed"].ToString();
                                dt.dateConfirmed = sdr["confirmed_date"] == DBNull.Value ? "" : Convert.ToDateTime(sdr["confirmed_date"]).ToString("dd/MM/yyyy HH:mm");
                                result.Add(dt);
                            }
                        }
                    }
                    catch (Exception)
                    {
                        result = new List<PurchaseOrder>();
                    }
                }
            }
            return result;
        }
        public static ResultReturn rejectRecord(string purchaseOrder, string remarks)
        {
            ResultReturn result = new ResultReturn();
            if ("".Equals(remarks))
            {
                result.code = CommonConstants.EXECUTE_UNSUCCESS;
                result.msg = "Please input remarks to reject record";
            }else
            {
                string constr = ConfigurationManager.ConnectionStrings[CommonConstants.CONNECTION_STRING_MWG_AQUA].ConnectionString;
                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_sales_rejectitempo"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        if (purchaseOrder != null && !"".Equals(purchaseOrder) && !"null".Equals(purchaseOrder) && !"undefined".Equals(purchaseOrder))
                            cmd.Parameters.Add("@PONO", SqlDbType.VarChar).Value = purchaseOrder;
                        if (remarks != null && !"".Equals(remarks) && !"null".Equals(remarks) && !"undefined".Equals(remarks))
                            cmd.Parameters.Add("@REMARKS", SqlDbType.NVarChar).Value = remarks;
                        con.Open();
                        try
                        {
                            using (SqlDataReader sdr = cmd.ExecuteReader())
                            {
                                while (sdr.Read())
                                {
                                    result.code = sdr["RETURNID"].ToString();
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
            }
            if (CommonConstants.EXECUTE_SUCCESS.Equals(result.code))
            {
                transferToMWG(purchaseOrder);
            }
            return result;
        }
        public static ResultReturn confirmRecord(string poNo)
        {
            ResultReturn result = new ResultReturn();
            if ("".Equals(poNo))
            {
                result.code = CommonConstants.EXECUTE_UNSUCCESS;
                result.msg = "Please choose PO to confirm";
            }
            else
            {
                string constr = ConfigurationManager.ConnectionStrings[CommonConstants.CONNECTION_STRING_MWG_AQUA].ConnectionString;
                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_sales_confirmPORecord"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        if (poNo != null && !"".Equals(poNo) && !"null".Equals(poNo) && !"undefined".Equals(poNo))
                            cmd.Parameters.Add("@PONO", SqlDbType.VarChar).Value = poNo;
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
                            result.code = CommonConstants.EXECUTE_UNSUCCESS;
                            result.msg = ex.Message;
                        }
                    }
                }
            }
            if(CommonConstants.EXECUTE_SUCCESS.Equals(result.code))
            {
                transferToMWG(poNo);
            }
            return result;
        }
        public static ResultReturn saveRecord(PurchaseOrderDetail input)
        {
            ResultReturn result = new ResultReturn();
            string constr = ConfigurationManager.ConnectionStrings[CommonConstants.CONNECTION_STRING_MWG_AQUA].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand("sp_sales_saveitempurchaseorderdetail"))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Connection = con;
                    if (input.numberPO != null && !"".Equals(input.numberPO) && !"null".Equals(input.numberPO) && !"undefined".Equals(input.numberPO))
                        cmd.Parameters.Add("@PONO", SqlDbType.VarChar).Value = input.numberPO;
                    if (input.matCode != null && !"".Equals(input.matCode) && !"null".Equals(input.matCode) && !"undefined".Equals(input.matCode))
                        cmd.Parameters.Add("@MATCODE", SqlDbType.VarChar).Value = input.matCode;
                    if (input.priceConfirm != null && !"".Equals(input.priceConfirm) && !"null".Equals(input.priceConfirm) && !"undefined".Equals(input.priceConfirm))
                        cmd.Parameters.Add("@PRICE", SqlDbType.Money).Value = input.priceConfirm.Replace(",", "");
                    if (input.qtyConfirm != null && !"".Equals(input.qtyConfirm) && !"null".Equals(input.qtyConfirm) && !"undefined".Equals(input.qtyConfirm))
                        cmd.Parameters.Add("@QUANTITY", SqlDbType.Int).Value = input.qtyConfirm;
                    if (input.status != null && !"".Equals(input.status) && !"null".Equals(input.status) && !"undefined".Equals(input.status))
                        cmd.Parameters.Add("@STATUS", SqlDbType.Int).Value = Convert.ToInt32(input.status);
                    if (input.remarks != null && !"".Equals(input.remarks) && !"null".Equals(input.remarks) && !"undefined".Equals(input.remarks))
                        cmd.Parameters.Add("@REMARKS", SqlDbType.NVarChar).Value = input.remarks;
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
                        result.code = CommonConstants.EXECUTE_UNSUCCESS;
                        result.msg = ex.Message;
                    }
                }
            }
            return result;
        }
        public static PurchaseOrder getDetailRecords(string purchaseOrder)
        {
            PurchaseOrder result = new PurchaseOrder();
            string constr = ConfigurationManager.ConnectionStrings[CommonConstants.CONNECTION_STRING_MWG_AQUA].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand("sp_sales_getpurchaseorderdetail"))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Connection = con;
                    if (purchaseOrder != null && !"".Equals(purchaseOrder))
                        cmd.Parameters.Add("@purchase_order_no", SqlDbType.VarChar).Value = purchaseOrder;
                    con.Open();
                    try
                    {
                        using (SqlDataReader sdr = cmd.ExecuteReader())
                        {
                            while (sdr.Read())
                            {
                                result.numberPO = sdr["purchase_order_no"].ToString();
                                result.orderDate = sdr["order_date"] == DBNull.Value ? "" : Convert.ToDateTime(sdr["order_date"]).ToString("dd/MM/yyyy HH:mm");
                                string requestedDate = sdr["requested_date"] == DBNull.Value ? "" : Convert.ToDateTime(sdr["requested_date"]).ToString("dd/MM/yyyy");
                                string expiryDate = sdr["expiry_date"] == DBNull.Value ? "" : Convert.ToDateTime(sdr["expiry_date"]).ToString("dd/MM/yyyy");
                                result.requestedDate = requestedDate + " - " + expiryDate;
                                result.contactUser = sdr["contact_user"].ToString() + " - " + sdr["contact_phone"].ToString();
                                result.contactOtherUser = sdr["contact_other_user"].ToString() + " - " + sdr["contact_other_phone"].ToString();
                                result.remarks = sdr["remarks"].ToString();
                                result.confirmed = sdr["is_confirmed"].ToString();
                            }
                        }
                    }
                    catch (Exception)
                    {
                        result = new PurchaseOrder();
                    }
                }
            }
            return result;
        }
        public static List<PurchaseOrderDetail> getListDetailRecord(string purchaseOrder)
        {
            List<PurchaseOrderDetail> result = new List<PurchaseOrderDetail>();
            string constr = ConfigurationManager.ConnectionStrings[CommonConstants.CONNECTION_STRING_MWG_AQUA].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand("sp_sales_listdetailpurchaseorders"))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Connection = con;
                    if (purchaseOrder != null && !"".Equals(purchaseOrder))
                        cmd.Parameters.Add("@purchase_order_no", SqlDbType.VarChar).Value = purchaseOrder;
                    con.Open();
                    try
                    {
                        using (SqlDataReader sdr = cmd.ExecuteReader())
                        {
                            while (sdr.Read())
                            {
                                PurchaseOrderDetail dt = new PurchaseOrderDetail();
                                dt.numberPO = sdr["purchase_order_no"].ToString();
                                dt.matCode = sdr["material_code"].ToString();
                                dt.modelType = sdr["model_type"].ToString();
                                dt.plantCode = sdr["plant_code"].ToString();
                                dt.quantity = sdr["quantity"] == DBNull.Value ? "0" : sdr["quantity"].ToString();
                                dt.qtyConfirm = sdr["quantity"] == DBNull.Value ? "0" : sdr["quantity"].ToString();
                                dt.price = sdr["price"] == DBNull.Value ? "0" : String.Format("{0:0,0}", Convert.ToDecimal(sdr["price"]));
                                dt.priceConfirm = sdr["price"] == DBNull.Value ? "0" : String.Format("{0:0,0}", Convert.ToDecimal(sdr["price"]));
                                dt.status = sdr["status"].ToString();
                                dt.remarks = sdr["remarks"].ToString();
                                dt.giftFlag = sdr["gift_flag"].ToString();
                                dt.changeFlag = sdr["change_flag"].ToString();
                                dt.isLock = sdr["lock"] == DBNull.Value ? "0" : Convert.ToInt32(sdr["lock"]).ToString();
                                result.Add(dt);
                            }
                        }
                    }
                    catch (Exception)
                    {
                        result = new List<PurchaseOrderDetail>();
                    }
                }
            }
            return result;
        }
        #region "Transfer MWG"
        public static ResultReturn transferToMWG(string poNo)
        {
            ResultReturn result = new ResultReturn();
            result.code = CommonConstants.EXECUTE_UNSUCCESS;
            result.msg = "Cannot transfer MWG. Please check agian !!!";

            POAcknowledgement po = new POAcknowledgement();
            string constr = ConfigurationManager.ConnectionStrings[CommonConstants.CONNECTION_STRING_MWG_AQUA].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand("sp_sales_getpurchaseorderdetail"))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Connection = con;
                    cmd.Parameters.Add("@purchase_order_no", SqlDbType.VarChar).Value = poNo;
                    con.Open();
                    try
                    {
                        using (SqlDataReader sdr = cmd.ExecuteReader())
                        {
                            while (sdr.Read())
                            {
                                po.OrderID = sdr["purchase_order_no"].ToString();
                                po.OrderDate = Convert.ToDateTime(sdr["order_date"]);
                                po.InputDate = Convert.ToDateTime(sdr["requested_date"]);
                                Message msg = new Message();
                                string confirmed = sdr["is_confirmed"].ToString();
                                if (confirmed == "1")
                                    msg.Type = "SUCCESS";
                                else
                                    msg.Type = "ERROR";
                                msg.Detail = sdr["remarks"].ToString();
                                po.Message = msg;
                            }
                            result.code = CommonConstants.EXECUTE_SUCCESS;
                            result.msg = "Completed !!!";
                        }
                    }
                    catch (Exception ex)
                    {
                        result.code = CommonConstants.EXECUTE_UNSUCCESS;
                        result.msg = ex.Message;
                    }
                }
                con.Close();
            }
            po.Items = getPurchaseOrderDetails(po.OrderID).ToArray();
            string auth = Common.callLoginResult();
            if (po.OrderID != "0")
            {
                ResponseMessage res = Common.executePOAcknowledgement(auth, po);
                updateStatusPO(po.OrderID, res);
            }
            return result;
        }
        private static List<Item> getPurchaseOrderDetails(string orderID)
        {
            List<Item> result = new List<Item>();
            string constr = ConfigurationManager.ConnectionStrings[CommonConstants.CONNECTION_STRING_MWG_AQUA].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand("sp_sales_listdetailpurchaseorders"))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Connection = con;
                    if (orderID != null && !"".Equals(orderID))
                        cmd.Parameters.Add("@purchase_order_no", SqlDbType.VarChar).Value = orderID;
                    con.Open();
                    try
                    {
                        using (SqlDataReader sdr = cmd.ExecuteReader())
                        {
                            while (sdr.Read())
                            {
                                Item dt = new Item();
                                dt.Quantity = Convert.ToDecimal(sdr["quantity_confirm"]);
                                dt.ItemCode = sdr["material_code"].ToString();
                                dt.ItemName = sdr["model_type"].ToString();
                                dt.Price = Convert.ToDecimal(sdr["price_confirm"]);
                                dt.QuantityUnit = "VNĐ";
                                Message msg = new Message();
                                string confirmed = sdr["status"].ToString();
                                if (confirmed == "1")
                                    msg.Type = "SUCCESS";
                                else
                                    msg.Type = "ERROR";
                                msg.Detail = sdr["remarks"].ToString();
                                dt.Message = msg;
                                result.Add(dt);
                            }
                        }
                    }
                    catch (Exception)
                    {
                        result = new List<Item >();
                    }
                }
            }
            return result;
        }
        private static void updateStatusPO(string orderID, ResponseMessage res)
        {
            string constr = ConfigurationManager.ConnectionStrings[CommonConstants.CONNECTION_STRING_MWG_AQUA].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand("sp_sales_updatestatuspurchaseorders"))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Connection = con;
                    if (orderID != null && !"".Equals(orderID))
                        cmd.Parameters.Add("@purchase_order_no", SqlDbType.VarChar).Value = orderID;
                    if ("YES".Equals(res.Status))
                        cmd.Parameters.Add("@code", SqlDbType.Int).Value = 1;
                    else
                        cmd.Parameters.Add("@code", SqlDbType.Int).Value = 2;
                    cmd.Parameters.Add("@feedback", SqlDbType.NVarChar).Value = res.Description;
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
        #endregion
    }
}