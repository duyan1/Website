using EWShop.Areas.Admin.Logics;
using EWShop.Areas.Admin.Models;
using EWShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EWShop.Areas.Admin.Controllers
{
    public class SurveyController : AdminController
    {
        // GET: Admin/Survey
        public ActionResult Index()
        {
            ViewData["QUESTIONS"] = SurveyLogics.getListRecords("Q");
            ViewData["ANSWERS"] = SurveyLogics.getListRecords("A");
            QAItem model = null;
            return View(model);
        }
        [HttpPost]
        public ActionResult Index(string QId)
        {
            ViewData["QUESTIONS"] = SurveyLogics.getListRecords("Q");
            ViewData["ANSWERS"] = SurveyLogics.getListRecords("A");
            QAItem model = SurveyLogics.getDataQAItem(QId);
            return PartialView("~/Areas/Admin/Views/Shared/_PartialQuestionAnswers.cshtml", model);
        }
        public JsonResult getListQuestions(string term)
        {
            return Json(SurveyLogics.getListQuestions(term), JsonRequestBehavior.AllowGet);
        }
    }
}