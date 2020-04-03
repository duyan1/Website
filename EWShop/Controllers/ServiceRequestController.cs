using EWShop.Areas.Admin.Logics;
using EWShop.Areas.Admin.Models;
using EWShop.Models;
using EWShop.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EWShop.Controllers
{
    public class ServiceRequestController : Controller
    {
        [HttpGet]
        public ActionResult ServiceRequest()
        {
            SelectList provList = new SelectList(Common.loadListProvince(), "value", "text");
            SelectList plantList = new SelectList(Common.getListPlants(), "value", "text");
            ViewBag.Provinces = provList;
            ViewBag.Plants = plantList;
            Session["CAPTCHA"] = Common.RandomNumber(4);
            return View();
        }
        [HttpGet]
        public ActionResult CheckSRStatus()
        {
            Session["CAPTCHA"] = Common.RandomNumber(4);
            ViewData["data"] = null;
            ViewData["active"] = "1";
            return View();
        }
        [HttpGet]
        public ActionResult Survey(string srnum)
        {
            if (srnum == null) srnum = "000000";
            ViewData["FIRST"] = "True";
            ViewData["SRNUM"] = srnum;
            QAItem item = SurveyLogics.getDataQAItem("0");
            return View(item);
        }
        [HttpPost]
        public JsonResult ServiceRequest(ServiceRequest data)
        {
            string clientCaptcha = data.captcha == null ? "" : data.captcha;
            string serverCaptcha = Session["CAPTCHA"].ToString();
            Session["CAPTCHA"] = Common.RandomNumber(4);
            SelectList provList = new SelectList(Common.loadListProvince(), "value", "text");
            SelectList plantList = new SelectList(Common.getListPlants(), "value", "text");
            ViewBag.Provinces = provList;
            ViewBag.Plants = plantList;
            ResultReturn result = new ResultReturn();
            if (!clientCaptcha.Equals(serverCaptcha))
            {
                result.msg = "Vui lòng nhập đúng mã xác nhận.";
                result.code = "0";
            }
            else
            {
                result = ServiceRequestLogics.RegisterServiceRequestForCustomer(data);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult CheckSRStatus(CheckBarcode input)
        {

            ItemResult result = new ItemResult();
            string clientCaptcha = input.clientCaptcha;
            string serverCaptcha = Session["CAPTCHA"].ToString();
            ViewData["active"] = input.flat;
            Session["CAPTCHA"] = Common.RandomNumber(4);
            if (!clientCaptcha.Equals(serverCaptcha))
            {
                result.code = "0";
                result.message = "Vui lòng nhập đúng mã xác nhận.";
                result.data = new RegisterEWInfo();
            }
            else
            {
                result = ServiceRequestLogics.checkSRStatus(input.phone, input.serial, input.model, input.cardNo, input.matid);
            }
            return PartialView("~/Views/Layout/_PartialListServiceRequest.cshtml", result);
        }
        [HttpPost]
        public ActionResult Survey(string question, string answer, string srnum, string remarks)
        {
            ViewData["FIRST"] = "False";
            ViewData["SRNUM"] = srnum;
            QAItem item = SurveyLogics.executeQASurvey(question, answer, srnum, remarks);
            return PartialView("~/Views/Layout/_PartialViewItemQASurvey.cshtml", item);
        }
    }
}