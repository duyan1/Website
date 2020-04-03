using EWShop.Areas.Admin.Logics;
using EWShop.Areas.Admin.Models;
using EWShop.Models;
using EWShop.Utilities;
using OfficeOpenXml;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;

namespace EWShop.Areas.Admin.Controllers
{
    [Authorize(Roles = "administrator")]
    public class BasicDataController : AdminController
    {
        #region "Material"
        [HttpGet]
        public ActionResult Materials()
        {
            ViewBag.MainMenu = CommonConstants.MENU_MAIN_BASIC_DATA;
            ViewBag.ChildMenu = CommonConstants.MENU_MATERIALS;
            MasterDataLogics logics = new MasterDataLogics();
            ViewData[CommonConstants.PLANTS_INFO] = logics.getListPlants();
            ViewData[CommonConstants.MODELS_INFO] = logics.getListModels();
            return View(MaterialLogics.getListMaterials(""));
        }
        [HttpGet]
        public JsonResult getMaterialInfo(string matId)
        {
            return Json(MaterialLogics.getMaterial(matId), JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public void DownloadExcelMaterials(string plantId)
        {
            List<Material> list = MaterialLogics.getListMaterials(plantId);

            ExcelPackage Ep = new ExcelPackage();
            ExcelWorksheet Sheet = Ep.Workbook.Worksheets.Add("Report");
            Sheet.Cells["A1"].Value = "MATERIAL CODE";
            Sheet.Cells["B1"].Value = "MATERIAL DESCRIPTION";
            Sheet.Cells["C1"].Value = "MODELTYPE";
            Sheet.Cells["D1"].Value = "PLANTCODE";
            Sheet.Cells["E1"].Value = "PLANTNAME";
            int row = 2;
            foreach (var item in list)
            {

                Sheet.Cells[string.Format("A{0}", row)].Value = item.materialCode;
                Sheet.Cells[string.Format("B{0}", row)].Value = item.materialDescription;
                Sheet.Cells[string.Format("C{0}", row)].Value = item.model;
                Sheet.Cells[string.Format("D{0}", row)].Value = item.plantCode;
                Sheet.Cells[string.Format("E{0}", row)].Value = item.plantName;
                row++;
            }


            Sheet.Cells["A:AZ"].AutoFitColumns();
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment: filename=" + "EWReport.xlsx");
            Response.BinaryWrite(Ep.GetAsByteArray());
            Response.End();
        }
        [HttpPost]
        public ActionResult Materials(string plantId)
        {
            return PartialView("~/Areas/Admin/Views/Shared/_PartialListMaterials.cshtml", MaterialLogics.getListMaterials(plantId));
        } 
        [HttpPost]
        public JsonResult saveMaterial(Material material)
        {
            return Json(MaterialLogics.saveMaterialInfo(material, User.Identity.Name), JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region "User"
        [HttpGet]
        public ActionResult Users()
        {
            MasterDataLogics logics = new MasterDataLogics();
            ViewData[CommonConstants.REGIONS_INFO] = logics.getListRegions();
            ViewData[CommonConstants.ROLES_INFO] = logics.getListRoles();
            ViewData[CommonConstants.SHOP_INFO] = logics.getListShops();
            return View(logics.getListUsers("4", "1"));
        }
        [HttpPost]
        public ActionResult Users(string region, string role)
        {
            MasterDataLogics logics = new MasterDataLogics();
            return PartialView("~/Areas/Admin/Views/Shared/_PartialListUsers.cshtml", logics.getListUsers(region, role));
        }
        [HttpGet]
        public JsonResult getUserInfo(string userId)
        {
            MasterDataLogics logics = new MasterDataLogics();
            User user = logics.getUser(userId);
            return Json(user, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult changePassUser(ChangeUser user)
        {
            ResultReturn result = UserLogic.resetPass(user.userName, user.newPass, user.confirmPass);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult UpdateUser(User user)
        {
            MasterDataLogics logics = new MasterDataLogics();
            ResultReturn result = logics.saveUser(user, User.Identity.Name);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult activeUser(string userId, string active)
        {
            MasterDataLogics logics = new MasterDataLogics();
            ResultReturn result = logics.activeUser(userId, active);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region "Shop"
        [HttpGet]
        public ActionResult Shops()
        {
            MasterDataLogics logics = new MasterDataLogics();
            ViewBag.MainMenu = CommonConstants.MENU_MAIN_BASIC_DATA;
            ViewBag.ChildMenu = CommonConstants.MENU_SHOPS;
            ViewData[CommonConstants.PROVINCES_INFO] = logics.getListProvinces();
            ViewData[CommonConstants.REGIONS_INFO] = logics.getListRegions();
            ViewData[CommonConstants.CHANNEL_TYPE_INFO] = logics.getChannelTypes();
            ViewData[CommonConstants.SHOP_GROUP_INFO] = logics.getShopGroups();
            ViewData[CommonConstants.CHANNEL_GROUP_INFO] = logics.getChannelGroups();
            return View(logics.getListShops("", "", "", "", ""));
        }
        [HttpPost]
        public ActionResult Shops(string type, string group, string region, string area, string active)
        {
            MasterDataLogics logics = new MasterDataLogics();
            return PartialView("~/Areas/Admin/Views/Shared/_Partiallistshops.cshtml", logics.getListShops(type, group, region, area, active));
        }
        public JsonResult getShopInfo(string shopId)
        {
            MasterDataLogics logics = new MasterDataLogics();
            ShopItem shop = logics.getShop(shopId);
            return Json(shop, JsonRequestBehavior.AllowGet);
        }
        public JsonResult loadListDistrict(string provinceId)
        {
            if (provinceId == null || provinceId == "") provinceId = "0";
            return Json(Common.loadListDistrict(provinceId), JsonRequestBehavior.AllowGet);
        }
        public JsonResult saveShopInfo(ShopItem shop)
        {
            MasterDataLogics logics = new MasterDataLogics();
            ResultReturn result = logics.saveShopInfo(shop, User.Identity.Name);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public void DownloadExcelShops(string type, string group, string region, string area, string active)
        {
            MasterDataLogics logics = new MasterDataLogics();
            List<ShopItem> list = logics.getListShops(type, group, region, area, active);

            ExcelPackage Ep = new ExcelPackage();
            ExcelWorksheet Sheet = Ep.Workbook.Worksheets.Add("data");
            Sheet.Cells["A1"].Value = "SHOPCODE";
            Sheet.Cells["B1"].Value = "SHOPNAME";
            Sheet.Cells["C1"].Value = "PROVINCE";
            Sheet.Cells["D1"].Value = "DISTRICT";
            Sheet.Cells["E1"].Value = "SHOPADDRESS";
            Sheet.Cells["F1"].Value = "SHOPTYPE";
            Sheet.Cells["G1"].Value = "ACCOUNT";
            Sheet.Cells["H1"].Value = "SHOPLEVEL";
            Sheet.Cells["I1"].Value = "REGION";
            Sheet.Cells["J1"].Value = "AREA";
            Sheet.Cells["K1"].Value = "GROUPCODE";
            Sheet.Cells["L1"].Value = "GROUPNAME";
            Sheet.Cells["M1"].Value = "ISACTIVE";
            Sheet.Cells["N1"].Value = "TRANSUSER";
            Sheet.Cells["O1"].Value = "TRANSDATE";
            int row = 2;
            foreach (var item in list)
            {

                Sheet.Cells[string.Format("A{0}", row)].Value = item.shopCode;
                Sheet.Cells[string.Format("B{0}", row)].Value = item.shopName;
                Sheet.Cells[string.Format("C{0}", row)].Value = item.province;
                Sheet.Cells[string.Format("D{0}", row)].Value = item.district;
                Sheet.Cells[string.Format("E{0}", row)].Value = item.shopAddress;
                Sheet.Cells[string.Format("F{0}", row)].Value = item.shopType;
                Sheet.Cells[string.Format("G{0}", row)].Value = item.shopGroup;
                Sheet.Cells[string.Format("H{0}", row)].Value = item.shopLevel;
                Sheet.Cells[string.Format("I{0}", row)].Value = item.region;
                Sheet.Cells[string.Format("J{0}", row)].Value = item.area;
                Sheet.Cells[string.Format("K{0}", row)].Value = item.groupCode;
                Sheet.Cells[string.Format("L{0}", row)].Value = item.groupName;
                Sheet.Cells[string.Format("M{0}", row)].Value = item.isActive;
                Sheet.Cells[string.Format("N{0}", row)].Value = item.transUser;
                Sheet.Cells[string.Format("O{0}", row)].Value = item.transDate;
                row++;
            }


            Sheet.Cells["A:AZ"].AutoFitColumns();
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment: filename=" + "EWReport.xlsx");
            Response.BinaryWrite(Ep.GetAsByteArray());
            Response.End();
        }
        public JsonResult ImportFileExcelShop()
        {
            ResultReturn result = new ResultReturn();
            HttpFileCollectionBase files = Request.Files;
            for (int i = 0; i < files.Count; i++)
            {

                HttpPostedFileBase file = files[i];
                if ((file != null) && (file.ContentLength > 0) && !string.IsNullOrEmpty(file.FileName))
                {
                    MasterDataLogics logics = new MasterDataLogics();
                    result = logics.ReadExcel(file, User.Identity.Name);
                }
                else
                {
                    result.code = "0";
                    result.msg = "File không tồn tại";
                }
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}