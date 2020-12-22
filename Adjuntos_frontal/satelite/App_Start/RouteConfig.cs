using System.Web.Mvc;
using System.Web.Routing;

namespace satelite
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{*token}",
                defaults: new { controller = "Services", action = "Index", token = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Scholarship",
                url: "{controller}/{action}/{*token}",
                defaults: new { controller = "Scholarship", action = "Index", token = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Error",
                url: "{controller}/{action}/{*token}",
                defaults: new { controller = "Error", action = "Index", token = UrlParameter.Optional }
            );


        }
    }
}
