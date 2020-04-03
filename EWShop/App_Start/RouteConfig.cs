using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace EWShop
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapRoute(
                name: "SupportRequest",
                url: "ho-tro-ky-thuat",
                defaults: new { controller = "Home", action = "SupportRequest" }
            );
            routes.MapRoute(
                name: "CheckEwarranty",
                url: "kiem-tra-bao-hanh",
                defaults: new { controller = "Home", action = "CheckEwarranty" }
            );
            routes.MapRoute(
                name: "ServiceRequest",
                url: "yeu-cau-bao-hanh",
                defaults: new { controller = "ServiceRequest", action = "ServiceRequest" }
            );
            routes.MapRoute(
                name: "CheckSRStatus",
                url: "tra-cuu-trang-thai-ycbh",
                defaults: new { controller = "ServiceRequest", action = "CheckSRStatus" }
            );
            routes.MapRoute(
                name: "RegisterEwarranty",
                url: "bao-hanh-san-pham",
                defaults: new { controller = "Home", action = "RegisterEwarranty" }
            );
            routes.MapRoute(
                name: "ACS",
                url: "dang-ky-tai-khoan",
                defaults: new { controller = "Home", action = "registerAccountACS" }
            );
            routes.MapRoute(
                name: "Survey",
                url: "khao-sat",
                defaults: new { controller = "ServiceRequest", action = "Survey" }
            );
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
