using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Uranus.Suite
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }

        protected void Session_Start(object sender, EventArgs e)
        {
            HttpRequest httpRequest = HttpContext.Current.Request;
            if (httpRequest.Browser.IsMobileDevice)
            {
                string path = httpRequest.Url.PathAndQuery;
                bool isOnMobilePage = path.StartsWith("/Login/Mobile/",
                                                       StringComparison.OrdinalIgnoreCase);
                if (!isOnMobilePage)
                {
                    string redirectTo = "~/Login/Mobile";

                    // Could also add special logic to redirect from certain 
                    // recognised pages to the mobile equivalents of those 
                    // pages (where they exist). For example,
                    // if (HttpContext.Current.Handler is UserRegistration)
                    //     redirectTo = "~/Mobile/RegistrationMobile.aspx";

                    HttpContext.Current.Response.Redirect(redirectTo);
                }
            }
        }
    }
}
