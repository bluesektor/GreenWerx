using Newtonsoft.Json;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using TreeMon.Data;
using TreeMon.Managers;
using TreeMon.Managers.Membership;
using TreeMon.Models.App;
using TreeMon.Models.Membership;
using TreeMon.Web.Helpers;

/// <summary>
/// Use this attribute for site pages, e.g. this is used in the
///  [SiteAuthorizationRequired] AdminController for accessing the page to restrict whome
///  accesses the pages. Use [AuthorizationRequired] for api methods.
/// </summary>

namespace TreeMon.Web.Filters
{
    public class SiteAuthorizationRequiredAttribute : ActionFilterAttribute
    {
        public string Role { get; set; }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (Globals.AddRequestPermissions)
            {
                string url = filterContext.RequestContext.HttpContext.Request.RawUrl;
                string requestType = filterContext.RequestContext.HttpContext.Request.RequestType;
                RoleManager roleManager = new RoleManager(Globals.DBConnectionKey);
                string name = roleManager.NameFromPath(url);
                roleManager.CreatePermission(name, requestType, url, "site");
            }

            //Cookies will be the most likely way to get the token in a site request
            //For the api its required in the headers because it's gonna be an ajax call which should be set by the dev.
            string authorizationToken = CookieHelper.GetValue(filterContext.HttpContext.Request.Cookies, "Authorization");

            if (string.IsNullOrWhiteSpace(authorizationToken ) && filterContext.HttpContext.Request.Headers["Authorization"] != null)
            {
                authorizationToken = filterContext.HttpContext.Request.Headers["Authorization"].ToString();
            }

            if (string.IsNullOrWhiteSpace(authorizationToken))
            {
                RedirectToRoute(filterContext, HttpStatusCode.Unauthorized, "Account", "Login", filterContext.RequestContext.HttpContext.Request.Path, true, "You must be logged in to access this page.");
                 return;
            }
            SessionManager sessionManager = new SessionManager(Globals.DBConnectionKey);
            if (sessionManager.IsValidSession(authorizationToken) == false)
            {
                RedirectToRoute(filterContext, HttpStatusCode.Unauthorized, "Account", "Login", filterContext.RequestContext.HttpContext.Request.Path, true, "Session expired.");
                return;
            }

            UserSession us = sessionManager.GetSession(authorizationToken, false);
            if (us == null)
            {
                RedirectToRoute(filterContext, HttpStatusCode.Unauthorized, "Account", "Login", filterContext.RequestContext.HttpContext.Request.Path, true, "Invalid session data.");
                return;
            }

            User currentUser = JsonConvert.DeserializeObject<User>(us.UserData);

            if (currentUser == null)
            {
                sessionManager.DeleteSession(authorizationToken);
                RedirectToRoute(filterContext, HttpStatusCode.Unauthorized, "Account", "Login", filterContext.RequestContext.HttpContext.Request.Path, true, "Invalid user data in session.");
                return;
            }

            if (!currentUser.SiteAdmin && !string.IsNullOrWhiteSpace(Role))
            {
                RoleManager roleManager = new RoleManager(Globals.DBConnectionKey);

               if(!roleManager.IsUserAuthorized(currentUser.UUID, currentUser.AccountUUID, filterContext.RequestContext.HttpContext.Request.Path))
                {
                    RedirectToRoute(filterContext, HttpStatusCode.Unauthorized, "Home", "Index", filterContext.RequestContext.HttpContext.Request.Path, true, "You are not authorized to view this page.");
                    return;
                }
            }

            base.OnActionExecuting(filterContext);
        }

        public void RedirectToRoute( ActionExecutingContext filterContext, HttpStatusCode status, string controller, string action, string returnUrl="",  bool expireCookie = false, string description = "" )
        {
            filterContext.RequestContext.HttpContext.Response.StatusCode = (int)status;
            filterContext.RequestContext.HttpContext.Response.StatusDescription = description;

            filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary { { "controller", controller }, { "action", action  },
                    { "returnUrl", returnUrl },
                    { "logOut", expireCookie }
                });

            //this doesn't work.. just leave it in case it starts working for some magical microsoft reason.
            if (expireCookie)
            {
                if (filterContext.RequestContext.HttpContext.Request.Cookies["Authorization"] != null)
                {
                    filterContext.RequestContext.HttpContext.Request.Cookies["Authorization"].Value = "";
                    filterContext.RequestContext.HttpContext.Request.Cookies.Remove("Authorization");
                    filterContext.HttpContext.Request.Cookies.Remove("Authorization");
                }

                if (filterContext.RequestContext.HttpContext.Request.Cookies["userUUID"] != null)
                {
                    filterContext.RequestContext.HttpContext.Request.Cookies["userUUID"].Value = "";
                }
                if (filterContext.RequestContext.HttpContext.Request.Cookies["AccountUUID"] != null)
                {
                    filterContext.RequestContext.HttpContext.Request.Cookies["AccountUUID"].Value = "";
                }

                //if (filterContext.HttpContext.Request.Cookies["Authorization"] != null)
                //{
                //    filterContext.HttpContext.Request.Cookies["Authorization"].Value = "";
                //}

                //if (filterContext.HttpContext.Request.Cookies["userUUID"] != null)
                //{
                //    filterContext.HttpContext.Request.Cookies["userUUID"].Value = "";
                //}
                //if (filterContext.HttpContext.Request.Cookies["AccountUUID"] != null)
                //{
                //    filterContext.HttpContext.Request.Cookies["AccountUUID"].Value = "";
                //}
            }


        }
    }
}