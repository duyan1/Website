using System.Web.Mvc;

namespace EWShop.Areas.Distribution
{
    public class DistributionAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Distribution";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Distribution_default",
                "Distribution/{controller}/{action}/{id}",
                new { controller = "Default",action = "SellThrough", id = UrlParameter.Optional }
            );
        }
    }
}