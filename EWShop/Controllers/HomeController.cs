using EWShop.Models;
using EWShop.Utilities;
using System;
using System.Drawing;
using System.IO;
using System.Web;
using System.Web.Mvc;

namespace EWShop.Controllers
{

    public class HomeController : BaseController
    {
        [Authorize]
        public ActionResult Index()
        {
            return View();
        }
        #region "Method Get"
        public ActionResult RegisterEwarranty()
        {
            SelectList provList = new SelectList(Common.loadListProvince(), "value", "text");
            ViewBag.Provinces = provList;
            Session["CAPTCHA"] = Common.RandomNumber(4);
            return View();
        }
        public ActionResult RegisterEwarrantyZalo()
        {
            SelectList provList = new SelectList(Common.loadListProvince(), "value", "text");
            ViewBag.Provinces = provList;
            Session["CAPTCHA"] = Common.RandomNumber(4);
            return View();
        }
        public ActionResult CheckEwarranty()
        {
            ViewData["active"] = "1";
            Session["CAPTCHA"] = Common.RandomNumber(4);
            ViewData["data"] = null;
            return View();
        }
        public ActionResult SupportRequest()
        {
            SelectList provList = new SelectList(Common.loadListProvince(), "value", "text");
            SelectList plantList = new SelectList(Common.getListPlants(), "value", "text");
            SelectList listTypeSPR = new SelectList(SupportRequestLogic.loadListSPRequestType(), "value", "text");
            ViewBag.TypeSPR = listTypeSPR;
            ViewBag.Provinces = provList;
            ViewBag.Plants = plantList;
            Session["CAPTCHA"] = Common.RandomNumber(4);
            return View();
        }
        public ActionResult UploadImageFile()
        {
            return View();
        }
        #endregion
        #region "Method Post"
        [HttpPost]
        public ActionResult CheckEwarranty(CheckBarcode data)
        {
            ItemResult result = new ItemResult();
            string clientCaptcha = data.clientCaptcha;
            string serverCaptcha = Session["CAPTCHA"].ToString();
            ViewData["active"] = data.flat;
            Session["CAPTCHA"] = Common.RandomNumber(4);
            if (!clientCaptcha.Equals(serverCaptcha))
            {
                result.code = "0";
                result.message = "Vui lòng nhập đúng mã xác nhận.";
                result.data = new RegisterEWInfo();
            }
            else
            {
                result.code = "1";
                result.message = "";
                result.data = EWarranty.CheckEWarranty(data.phone, data.serial, data.model, data.cardNo, data.flat);
            }
            return PartialView("~/Views/Layout/_ViewRegisterEW.cshtml", result);
        }
        [HttpPost]
        public JsonResult RegisterEwarranty(ItemSellOut data)
        {
            string clientCaptcha = data.captcha == null ? "" : data.captcha;
            string serverCaptcha = Session["CAPTCHA"].ToString();
            Session["CAPTCHA"] = Common.RandomNumber(4);
            SelectList provList = new SelectList(Common.loadListProvince(), "value", "text");
            ViewBag.Provinces = provList;
            ItemResult result = new ItemResult();
            if (!clientCaptcha.Equals(serverCaptcha))
            {
                result.message = "Vui lòng nhập đúng mã xác nhận.";
                result.code = "0";
            }
            else
            {
                result = ProductSellOutLogics.registerEWarrantyByWebsiteCustomer(data, "2");
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult RegisterEwarrantyZalo(ItemSellOut data)
        {
            string clientCaptcha = data.captcha == null ? "" : data.captcha;
            string serverCaptcha = Session["CAPTCHA"].ToString();
            Session["CAPTCHA"] = Common.RandomNumber(4);
            SelectList provList = new SelectList(Common.loadListProvince(), "value", "text");
            ViewBag.Provinces = provList;
            ItemResult result = new ItemResult();
            if (!clientCaptcha.Equals(serverCaptcha))
            {
                result.message = "Vui lòng nhập đúng mã xác nhận.";
                result.code = "0";
            }
            else
            {
                result = ProductSellOutLogics.registerEWarrantyByWebsiteCustomer(data, "4");
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult SupportRequest(SupportRequest data)
        {
            ResultReturn result = new ResultReturn();
            string clientCaptcha = data.captcha == null ? "" : data.captcha;
            string serverCaptcha = Session["CAPTCHA"].ToString();
            Session["CAPTCHA"] = Common.RandomNumber(4);
            if (!clientCaptcha.Equals(serverCaptcha))
            {
                result.msg = "Vui lòng nhập đúng mã xác nhận";
                result.code = "0";
            }
            else
            {
                result = SupportRequestLogic.RegisterSupportRequest(data);
                if (result.code == "1")
                {
                    string email = result.msg;

                    result.msg = "Gởi thành công. Cám ơn bạn đã liên hệ với chúng tôi";
                }
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult uploadImageFile()
        {
            ResultReturn result = new ResultReturn();
            HttpFileCollectionBase files = Request.Files;
            bool flat = false;
            string fileNameStr = "";
            for (int i = 0; i < files.Count; i++)
            {
                HttpPostedFileBase file = files[i];
                if ((file != null) && (file.ContentLength > 0) && !string.IsNullOrEmpty(file.FileName))
                {
                    flat = true;
                    result.code = CommonConstants.EXECUTE_SUCCESS;
                    string origFileName = file.FileName;
                    string fileName = Common.RandomNumber(9) + System.IO.Path.GetExtension(file.FileName).ToLower();
                    string fileUrl = (DateTime.Now.Year * 100 + DateTime.Now.Month).ToString();
                    fileNameStr = ("".Equals(fileNameStr) ? "" : fileNameStr + ";") + FileUpload.upLoadImage(file, fileUrl, fileName);
                }
            }
            if (!flat)
            {
                result.code = CommonConstants.EXECUTE_UNSUCCESS;
                result.msg = "Không có hình ảnh cần tải";
            }
            else
            {
                result.msg = fileNameStr;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region "Return json"
        public JsonResult loadListWard(string districtId)
        {
            if (districtId == null || districtId == "") districtId = "0";
            return Json(Common.loadListWard(districtId), JsonRequestBehavior.AllowGet);
        }
        public JsonResult loadListDistrict(string provinceId)
        {
            if (provinceId == null || provinceId == "") provinceId = "0";
            return Json(Common.loadListDistrict(provinceId), JsonRequestBehavior.AllowGet);
        }
        public JsonResult loadListModel(string plantId)
        {
            if (plantId == null || plantId == "") plantId = "0";
            return Json(Common.loadListModel(plantId), JsonRequestBehavior.AllowGet);
        }
        public JsonResult checkBarcode(string barcode)
        {
            return Json(SellOutLogic.checkBarcode(barcode), JsonRequestBehavior.AllowGet);
        }
        public JsonResult searchModel(string term)
        {
            return Json(SellOutLogic.listModelFilterByPlantAndModel(term), JsonRequestBehavior.AllowGet);
        }
        public JsonResult searchShop(string term)
        {
            return Json(SellOutLogic.listShopFilterByKeyword(term), JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region "Common"
        public FileResult GetCaptchaImage()
        {
            string text = Session["CAPTCHA"].ToString();

            //first, create a dummy bitmap just to get a graphics object
            Image img = new Bitmap(1, 1);
            Graphics drawing = Graphics.FromImage(img);

            Font font = new Font("Arial", 15);
            //measure the string to see how big the image needs to be
            SizeF textSize = drawing.MeasureString(text, font);

            //free up the dummy image and old graphics object
            img.Dispose();
            drawing.Dispose();

            //create a new image of the right size
            img = new Bitmap((int)textSize.Width + 40, (int)textSize.Height + 20);
            drawing = Graphics.FromImage(img);

            Color backColor = Color.SeaShell;
            Color textColor = Color.Red;
            //paint the background
            drawing.Clear(backColor);

            //create a brush for the text
            Brush textBrush = new SolidBrush(textColor);

            drawing.DrawString(text, font, textBrush, 20, 10);

            drawing.Save();

            font.Dispose();
            textBrush.Dispose();
            drawing.Dispose();

            MemoryStream ms = new MemoryStream();
            img.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            img.Dispose();

            return File(ms.ToArray(), "image/png");
        }
        #endregion
        #region "ACS"
        public ActionResult registerAccountACS()
        {
            SelectList provList = new SelectList(Common.loadListProvince(), "value", "text");
            ViewBag.Provinces = provList;
            Session["CAPTCHA"] = Common.RandomNumber(4);
            ViewBag.Tab = "1";
            return View();
        }
        [HttpPost]
        public ActionResult registerAccountACS(ACSModel input)
        {
            ResultReturn result = new ResultReturn();
            string clientCaptcha = input.captcha == null ? "" : input.captcha;
            string serverCaptcha = Session["CAPTCHA"].ToString();
            Session["CAPTCHA"] = Common.RandomNumber(4);
            SelectList provList = new SelectList(Common.loadListProvince(), "value", "text");
            ViewBag.Provinces = provList;
            if (!clientCaptcha.Equals(serverCaptcha))
            {
                ViewBag.Tab = "1";
                ViewBag.Error = "Vui lòng nhập đúng mã xác nhận.";
            }
            else
            {
                result = ACSLogics.registerACS(input);
                if(result.code == CommonConstants.EXECUTE_SUCCESS)
                {
                    ViewBag.Tab = "2";
                    ViewBag.Mobile = input.phone;
                }else
                {
                    ViewBag.Tab = "1";
                    ViewBag.Error = result.msg;
                }
            }
            input.captcha = "";
            return View(input);
        }
        [HttpPost]
        public ActionResult SendOTP(string mobile, string otp)
        {
            ResultReturn result = new ResultReturn();
            result = ACSLogics.sendOTP(mobile, otp);
            if(result.code == "1")
            {
                ViewBag.Tab = "3";
                ViewBag.Error = "";
            }else
            {
                ViewBag.Tab = "2";
                ViewBag.Error = result.msg;
            }
            return View("registerAccountACS");
        }
        #endregion
    }
}