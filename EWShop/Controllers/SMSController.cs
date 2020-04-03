using EWShop.Models;
using EWShop.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EWShop.Controllers
{
    public class SMSController : Controller
    {
        #region "SMS"
        [HttpGet]
        public JsonResult registerEW(string PhoneNumber, string MsgContent, string RequestId)
        {
            ItemReturn result = new ItemReturn();
            string cusPhone = "";
            string shopPhone = "";
            if (MsgContent == null || "".Equals(MsgContent))
            {
                result.code = "0";
                result.message = "Nội dung tin nhắn rỗng";
            }
            else
            {
                //Replace all case 
                MsgContent = MsgContent.Replace("+", " ");
                MsgContent = MsgContent.Replace("_", " ");
                MsgContent = MsgContent.Replace("  ", " ");
                MsgContent = MsgContent.Replace("   ", " ");
                MsgContent = MsgContent.Replace("    ", " ");
                MsgContent = MsgContent.Replace("     ", " ");

                //Cut string by space
                string[] keyWords = MsgContent.Split(new char[] { ',', ' ', '_' });
                if (keyWords.Length >= 2)
                {
                    string syntax = keyWords[1].ToUpper();
                    result.code = "0";
                    result.message = "Chưa xử lý thông tin";
                    //check each case
                    if ("TBH".Equals(syntax)) /*- CÚ PHÁP DÀNH CHO TRẠM BẢO HÀNH -*/
                    {

                    }
                    else if ("YCBH".Equals(syntax) || "YEUCAUBAOHANH".Equals(syntax)) /*- CÚ PHÁP TẠO PHIẾU YÊU CẦU -*/
                    {

                    }
                    else if ("TTBH".Equals(syntax)) /*- CÚ PHÁP LẤY THÔNG TIN BẢO HÀNH -*/
                    {

                    }
                    else if ("MABH".Equals(syntax)) /*- CÚ PHÁP LẤY THÔNG TIN BẢO HÀNH -*/
                    {

                    }
                    else if ("HNKH".Equals(syntax)) /*- CÚ PHÁP ĐĂNG KÝ GÓI TRƯNG BÀY -*/
                    {

                    }
                    else if ("MK".Equals(syntax)) /*- CÚ PHÁP LẤY LẠI MẬT KHẨU ĐĂNG NHẬP -*/
                    {

                    }
                    else
                    {
                        try
                        {
                            if (keyWords.Length >= 3)
                            {
                                string barcode = keyWords[1];
                                string pod = keyWords[2];
                                if (keyWords.Length == 4)
                                {
                                    shopPhone = Common.convertPhone(PhoneNumber);
                                    cusPhone = Common.convertPhone(keyWords[3]);
                                    result = ProductSellOutLogics.registerEWBySMSShop(shopPhone, cusPhone, barcode, pod);
                                } else
                                {
                                    cusPhone = Common.convertPhone(PhoneNumber);
                                    result = ProductSellOutLogics.registerEWBySMSCustomer(cusPhone, barcode, pod);
                                }
                            }
                            else
                            {
                                result.code = "0";
                                result.message = "Cu phap tin nhan khong hop le";
                            }
                        }
                        catch (Exception)
                        {
                            result.code = "0";
                            result.message = "Cu phap tin nhan khong hop le";
                        }
                    }
                }
                else
                {
                    result.code = "0";
                    result.message = "Cu phap tin nhan khong hop le";
                }
            }
            //Common.SendMessageAgain(result.message, cusPhone, shopPhone, RequestId, result.code);
            ProductSellOutLogics.SaveContentSMS(MsgContent, PhoneNumber, result);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult sendSMSToCustomer(string EWCardId)
        {
            if (EWCardId != "")
            {
                ItemResult result = FiboSMS.GetInfoEW(EWCardId);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            return Json("Không hợp lệ", JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}