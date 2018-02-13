// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using TreeMon.Data.Logging;
using TreeMon.Managers;
using TreeMon.Managers.Membership;
using TreeMon.Models.App;
using TreeMon.Models.Membership;
using TreeMon.Utilites.Extensions;
using TreeMon.Web.api.Helpers;

namespace TreeMon.Web.Filters
{
    //this creates the attribute [AuthorizationRequired] it will call the OnActionExecuting and verify the token in the header.
    //this can be an attribute of a function also

    public class ApiAuthorizationRequiredAttribute : ActionFilterAttribute
    {
        private SessionManager _sessionsManager = null;
        readonly private RoleManager _roleManager = null;

        public string Operator { get; set; }

        public int RoleWeight { get; set; }

        public ApiAuthorizationRequiredAttribute()
        {
            Globals.InitializeGlobals();

            if (string.IsNullOrWhiteSpace(Globals.DBConnectionKey) 
                && ConfigurationManager.AppSettings["DefaultDbConnection"] != null)
            {
                Globals.DBConnectionKey = ConfigurationManager.AppSettings["DefaultDbConnection"].ToString();
            }

            Globals.Application = new WebApplication(Globals.DBConnectionKey);

            //now pull the default connection from the database
            if(string.IsNullOrWhiteSpace(Globals.DBConnectionKey) && Globals.Application.UseDatabaseConfig)
                Globals.DBConnectionKey = Globals.Application.AppSetting("DefaultDbConnection");

            _roleManager = new RoleManager(Globals.DBConnectionKey);

        }

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
         

            if (actionContext == null || Globals.Application?.Status == "INSTALLING")
            {
                base.OnActionExecuting(actionContext);
                return;
            }

            _sessionsManager = new SessionManager(Globals.DBConnectionKey);

            #region API key verification code. BACKLOG not complete

                IEnumerable<KeyValuePair<string, string>> kvp = actionContext.Request.GetQueryNameValuePairs();
                KeyValuePair<string, string> apiKVP = kvp.FirstOrDefault(w => w.Key.EqualsIgnoreCase("APIKEY"));

                if (!string.IsNullOrWhiteSpace(apiKVP.Value))
                {
                    Debug.Assert(false, "NOT IMPLEMENTED");
                    //validate apikey
                }
                #endregion


            if (Globals.AddRequestPermissions)
            {
                //if we need to pass the user object in, move this down below after it gets the current user.
                RoleManager roleManager = new RoleManager(Globals.DBConnectionKey, null);
                string name = roleManager.NameFromPath(actionContext.Request.RequestUri.AbsolutePath.ToString());
                string uriPath = actionContext.Request.RequestUri.AbsolutePath.ToString();//absolute path doesn't have query string 
                roleManager.CreatePermission(name, actionContext.Request.Method.Method, uriPath, "api");
            }

            if(this.RoleWeight == 0)
            {   //no need to check permissions 
                base.OnActionExecuting(actionContext);
                return;
            }
            if (actionContext.Request.Headers.Authorization == null)
            {
                actionContext.Response = new HttpResponseMessage(HttpStatusCode.Unauthorized) { ReasonPhrase = "Invalid Request: missing authorization token." };
                base.OnActionExecuting(actionContext);
                return;
            }
            var tokenValue = actionContext.Request?.Headers?.Authorization?.Parameter;
      
            if (_sessionsManager.IsValidSession(tokenValue) == false)
            {
                actionContext.Response = new HttpResponseMessage(HttpStatusCode.Unauthorized) { ReasonPhrase = "Session expired." };
                actionContext.Response.StatusCode = HttpStatusCode.Unauthorized;
                base.OnActionExecuting(actionContext);
                return;
            }

            UserSession us = _sessionsManager.GetSession(tokenValue, false);

            if (us == null || us.UserData == null)
            {
                _sessionsManager.DeleteSession(tokenValue);
                actionContext.Response = new HttpResponseMessage(HttpStatusCode.Unauthorized) { ReasonPhrase = "Invalid session data." };
                base.OnActionExecuting(actionContext);
                return;
            }
         
            User currentUser = JsonConvert.DeserializeObject<User>(us.UserData);

            actionContext.Request.Properties.Add("CurrentUser", currentUser);

            if (currentUser == null)
            {
                actionContext.Response = new HttpResponseMessage(HttpStatusCode.Unauthorized) { ReasonPhrase = "Invalid session user data." };
                base.OnActionExecuting(actionContext);
                return;
            }
         
            // if not a site owner and we have a role, check their priviledge!
            if (!currentUser.SiteAdmin  && _roleManager.UserInAuthorizedRole(currentUser,this.RoleWeight,   this.Operator) == false )
            {
                if (!_roleManager.IsUserRequestAuthorized(currentUser.UUID, currentUser.AccountUUID, actionContext.Request.RequestUri.AbsolutePath.ToString()))
                {
                    actionContext.Response = new HttpResponseMessage(HttpStatusCode.Unauthorized) { ReasonPhrase = "You are not authorized for this action." }; ;
                    base.OnActionExecuting(actionContext);
                    return;
                }
            }
            base.OnActionExecuting(actionContext);
        }
    }
}