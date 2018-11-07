using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace WebApplication_NewPC
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            
            routes.MapRoute(
                name: "Movie",
                url: "{action}/{year}/{type}-{country}-{order}-{page}",
                defaults: new { controller = "Home", year = "all", type = 0, country = 0, order = 0, page = 1 },
                constraints: new {
                    action = "Movie|TvShow",
                    year = @"[1-9]\d{3}|other|all",
                    type = @"\d*",
                    country = @"\d*",
                    page = @"[1-9]\d*"
                }
            );

            routes.MapRoute(
                name: "MovieDefault",
                url: "{action}/{year}",
                defaults: new { controller = "Home", year = "all", type = 0, country = 0, order = 0, page = 1 },
                constraints: new
                {
                    action = "Movie|TvShow",
                    year = @"[1-9]\d{3}|other|all",
                    type = @"\d*",
                    country = @"\d*",
                    page = @"[1-9]\d*"
                }
            );

            routes.MapRoute(
                name: "MovieDefaultNotYear",
                url: "{action}/{type}-{country}-{order}-{page}",
                defaults: new { controller = "Home", year = "all", type = 0, country = 0, order = 0, page = 1 },
                constraints: new
                {
                    action = "Movie|TvShow",
                    year = @"[1-9]\d{3}|other|all",
                    type = @"\d*",
                    country = @"\d*",
                    page = @"[1-9]\d*"
                }
            );

            routes.MapRoute(
                name: "Subject",
                url: "Subject/{hash}",
                defaults: new { controller = "Home", action = "Subject", hash = "" }
            );

            routes.MapRoute(
                name: "Search",
                url: "Search/{p}",
                defaults: new { controller = "Home", action = "Search", p = "" }
            );


            routes.MapRoute(
                name: "Player",
                url: "Player/{id}",
                defaults: new { controller = "Home", action = "Player", id = 0 }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );

            //routes.MapRoute(
            //    name: "Default",
            //    url: "{controller}/{action}/{id}",
            //    defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            //);


        }
    }
}
