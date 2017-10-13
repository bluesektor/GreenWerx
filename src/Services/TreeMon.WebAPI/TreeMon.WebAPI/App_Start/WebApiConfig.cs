// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
using System.Web.Http;
using WebApiThrottle;

namespace TreeMon.WebAPI
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.MessageHandlers.Add(new ThrottlingHandler()
            {
                Policy = new ThrottlePolicy(perSecond: 5, perMinute: 60, perHour: 600, perDay: 1500, perWeek: 3000)
                {
                    IpThrottling = true
                },
                Repository = new CacheRepository()
            });

            //If you enable this you will have to disable it in the Global.asax.cs
            //var cors = new EnableCorsAttribute("*", "*", "*");
            //config.EnableCors(cors);

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "WebApiAppGetSettings",
                routeTemplate: "api/{controller}/{action}/{param}",
                defaults: new { controller = "App", action = "GetSettings", param = RouteParameter.Optional }
            );
        }
    }
}
