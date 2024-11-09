using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Restaurant_QKA
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            // Route mặc định cho Area User
            routes.MapRoute(
                name: "UserAreaDefault",
                url: "User/{controller}/{action}/{id}",
                defaults: new { area = "User", controller = "Home", action = "Index", id = UrlParameter.Optional }
            ).DataTokens["area"] = "User";

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );

            
        }
    }
}
