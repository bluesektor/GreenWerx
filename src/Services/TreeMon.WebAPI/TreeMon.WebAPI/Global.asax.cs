// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
using System;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using TreeMon.Data.Logging;
using TreeMon.Managers;
using TreeMon.Utilites;
using TreeMon.Utilites.Extensions;
using TreeMon.Utilites.Security;
using TreeMon.Web;

namespace TreeMon.WebAPI
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        public WebApiApplication()
        { }

        protected void Application_Start()
        {
            
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            if (!Globals.Application.Initialized)
            {
                Globals.InitializeGlobals();
                Globals.Application.Start();
            }

            if (string.IsNullOrWhiteSpace(Globals.Application.Status) || Globals.Application.Status == "true")
                Globals.Application.Status = "RUNNING";

            if (string.IsNullOrWhiteSpace(Globals.Application.ApiStatus) || Globals.Application.ApiStatus == "true")
                Globals.Application.ApiStatus = Globals.Application.AppSetting("ApiStatus", "PRIVATE");
        }

        private DateTime _lastSessionsClear = DateTime.MinValue;
        private const string ROOT_DOCUMENT = "/index.html";

        void Application_BeginRequest(object sender, EventArgs e)
        {

            if (!HttpContext.Current.Request.IsSecureConnection)
                HttpContext.Current.Response.Redirect(HttpContext.Current.Request.Url.AbsoluteUri.Replace("http://", "https://"));



            if (Validator.HasCodeInjection(HttpContext.Current.Request.Url.Query) || Validator.HasSqlCommand(HttpContext.Current.Request.Url.Query))
            {
                Response.Write("Invalid request, security violation occured.");
            }

              if (string.IsNullOrWhiteSpace(Globals.Application.ApiStatus) || Globals.Application.ApiStatus == "true")
                Globals.Application.ApiStatus = Globals.Application.AppSetting("ApiStatus", "PRIVATE");

            // Stop clickjacking and from being loaded in a frame.
            HttpContext.Current.Response.AddHeader("x-frame-options", "DENY");

            SetCorsOptions();

            ////if (Globals.Application.Status == "REQUIRES_INSTALL" || Globals.Application.Status == "INSTALLING")
            ////{
            ////    Globals.Application.Status = "INSTALLING";
            ////    return;
            ////}
           
            SessionManager sessionManager = new SessionManager(Globals.DBConnectionKey);
            TimeSpan ts = DateTime.UtcNow - _lastSessionsClear;

            //backlog move this to somewhere better. 
            if (_lastSessionsClear == DateTime.MinValue || ts.TotalMinutes > sessionManager.SessionLength)
            {   
                sessionManager.ClearExpiredSessions(sessionManager.SessionLength);
                _lastSessionsClear = DateTime.UtcNow;
            }

            if (Request.Url.LocalPath.StartsWith("/api") || Request.Url.LocalPath.StartsWith("/Content"))
                return;

            string url = Request.Url.LocalPath;
            if (!System.IO.File.Exists(Context.Server.MapPath(url)))
                Context.RewritePath(ROOT_DOCUMENT);
        }

        private void SetCorsOptions()
        {
            if (Globals.Application.Status == "REQUIRES_INSTALL" || Globals.Application.Status == "INSTALLING")
            {
                HttpContext.Current.Response.AddHeader("Access-Control-Allow-Origin", "*");
            }
            else
            {
                switch (Globals.Application?.ApiStatus?.ToUpper())
                {
                    case "PRIVATE":
                        string domain = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
                        HttpContext.Current.Response.AddHeader("Access-Control-Allow-Origin", domain);
                        break;
                    case "PROTECTED"://check to see if this domain is allowed to make an api request. NOTE: this is different from the request throttling code.
                        AppManager appManager = new AppManager(Globals.DBConnectionKey, "web", "");
                        var origin = HttpContext.Current.Request.Headers["Origin"];
                        if (Globals.Application.UseDatabaseConfig && appManager.SettingExistsInDatabase("AllowedOrigin", origin))
                            HttpContext.Current.Response.AddHeader("Access-Control-Allow-Origin", origin);
                        else if (!Globals.Application.UseDatabaseConfig)
                        {
                            var value = appManager.GetSetting("AllowedOrigins", false)?.Value;
                            if (!string.IsNullOrWhiteSpace(value) && value.Split(',').Any(x => x.EqualsIgnoreCase(origin) ) )
                                HttpContext.Current.Response.AddHeader("Access-Control-Allow-Origin", origin);

                        }
                        break;
                    case "PUBLIC":
                        HttpContext.Current.Response.AddHeader("Access-Control-Allow-Origin", "*");
                        break;
                }
            }

            if (HttpContext.Current.Request.HttpMethod == "OPTIONS")
            {
                //specific initialization
                //These headers are handling the "pre-flight" OPTIONS call sent by the browser
                HttpContext.Current.Response.AddHeader("Access-Control-Allow-Methods", "GET,POST,PATCH,DELETE,PUT");
                HttpContext.Current.Response.AddHeader("Access-Control-Allow-Headers", "Content-Type, Accept,authorization,content-type");
                HttpContext.Current.Response.AddHeader("Access-Control-Max-Age", "1728000");
                HttpContext.Current.Response.End();
            }
        }

        void Application_Error(object sender, EventArgs e)
        {

            try
            {
                // get the exception and re-throw
                Exception ex = Server.GetLastError();
                SystemLogger logger = new SystemLogger(Globals.DBConnectionKey);
                logger.InsertError(ex.Message, "Golbal.asax", "Application_Error");
                throw ex;
            }

            catch (HttpException httpEx)
            {
               Debug.Assert(false, httpEx.Message);
            }
        }
    }
}
