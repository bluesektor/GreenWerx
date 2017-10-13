// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
using System.Web.Mvc;
using System.Web.Routing;

namespace TreeMon.WebAPI
{
    public class RouteConfig
    {
        protected RouteConfig() { }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapMvcAttributeRoutes();
            
            //put more specific routes first.

            routes.MapRoute(
                name: "RouteControllerActionId",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
