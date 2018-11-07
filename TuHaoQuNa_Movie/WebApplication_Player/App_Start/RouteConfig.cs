using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace WebApplication_Player
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");


            routes.MapRoute(
                name: "Play",
                url: "{action}/{type}/{vid}",
                defaults: new { controller = "Player", action = "Play", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Player", action = "Index", id = UrlParameter.Optional }
            );

            //routes.MapRoute(
            //    name: "Player",
            //    url: "{url}/{name}",
            //    defaults: new { controller = "Player", action = "Index", url = UrlParameter.Optional }
            //);

        }
    }
}
