using EWShop.Models;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace EWShop.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class BasicAuthorization : AuthorizationFilterAttribute
    {

        public override void OnAuthorization(HttpActionContext filterContext)
        {
            string authorizedToken = string.Empty;

            try
            {
                var accessToken = filterContext.Request.Headers.SingleOrDefault
                                      (x => x.Key == CommonConstants.AUTHORIZED_TOKEN);
                if (accessToken.Key != null)
                {
                    authorizedToken = Convert.ToString(accessToken.Value.SingleOrDefault());
                    if (!IsAuthorize(authorizedToken))
                    {
                        filterContext.Response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
                        return;
                    }
                }
                else
                {
                    filterContext.Response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
                    return;
                }
            }
            catch (Exception)
            {
                filterContext.Response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
                return;
            }

            base.OnAuthorization(filterContext);
        }
        private bool IsAuthorize(string authorizedToken)
        {
            UserInfo user = UserLogic.findUserByAccessToken(authorizedToken);
            if (user != null)
            {
                return true;
            }
                
            return false;
        }
    }
}