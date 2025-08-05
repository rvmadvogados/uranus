using System.Web.Mvc;
using System.Web.Routing;

namespace Uranus.Suite
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            //enable attribute routing
            routes.MapMvcAttributeRoutes();

            //convention-based routes
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Login", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Erro",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Login", action = "Mobile", id = UrlParameter.Optional }
            );

        }
    }
}
