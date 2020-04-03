using System.Web.Mvc;

namespace EWShop.Areas.CallCenter
{
    public class CallCenterAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "CallCenter";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "CallCenter_ListRegisteredEW",
                "CallCenter/ListRegisteredEW",
                new { controller = "Ewarranty", action = "ListRegisteredEW", id = UrlParameter.Optional }
                );
            context.MapRoute(
                "CallCenter_ProcessSMSRegistered",
                "CallCenter/ListSMSRegisteredEW",
                new { controller = "Ewarranty", action = "SMSInvalid", id = UrlParameter.Optional }
                );
            context.MapRoute(
                "CallCenter_ProcessEWNotCompleted",
                "CallCenter/EwarrantiesPendingApproval",
                new {controller = "Ewarranty", action = "EWNotCompleted", id = UrlParameter.Optional }
                );
            context.MapRoute(
                "CallCenter_default",
                "CallCenter/{controller}/{action}/{id}",
                new { controller = "CallCenterHome", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}