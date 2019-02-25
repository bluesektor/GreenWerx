// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using System.Web.Http.Controllers;
using TreeMon.Data.Logging;
using TreeMon.Managers;
using TreeMon.Models.App;
using TreeMon.Models.Datasets;
using TreeMon.Models.Membership;
using TreeMon.Utilites.Extensions;
using TreeMon.Utilites.Helpers;
using TreeMon.Web.api.Helpers;

namespace TreeMon.Web.api
{
    public class ApiBaseController : ApiController
    {
        readonly SystemLogger _logger;
        private readonly List<TreeMon.Models.Geo.TimeZone> _timesZones = new List<TreeMon.Models.Geo.TimeZone>();


        public ApiBaseController()
        {

            Globals.InitializeGlobals();
            _logger = new SystemLogger(Globals.DBConnectionKey);
            _sessionManager = new SessionManager(Globals.DBConnectionKey);

            try
            {
                string pathToFile = Path.Combine(EnvironmentEx.AppDataFolder.Replace("\\\\", "\\"), "WordLists\\timezones.json");

                if (File.Exists(pathToFile))
                {
                    string timezones = File.ReadAllText(pathToFile);
                    _timesZones = JsonConvert.DeserializeObject<List<TreeMon.Models.Geo.TimeZone>>(timezones);
                }
            }
            catch (Exception ex)
            {
                Debug.Assert(false, ex.Message);
            }
        }
        
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            if (controllerContext.Request == null)
                return;

            string auth = controllerContext.Request.Headers?.Authorization?.Parameter;
            if(!string.IsNullOrWhiteSpace(auth) && auth !="undefined")
              this.GetUser(auth);
           
            
        }

        public DataFilter GetFilter(HttpRequestMessage request)
        {
            if (request.Method != HttpMethod.Post)
                return null;

            string filter = this.Request.Content.ReadAsStringAsync().Result;

            if (string.IsNullOrWhiteSpace(filter))
                return null;

            return GetFilter(filter);
        }

        private DataFilter GetFilter(string filter) {

            int currentUserRoleWeight = CurrentUser?.RoleWeight ?? 0;
            bool siteAdmin = CurrentUser?.SiteAdmin ?? false;
            DataFilter tmpFilter = Globals.DefaultDataFilter;
            tmpFilter.UserRoleWeight = currentUserRoleWeight;


            if (string.IsNullOrWhiteSpace(filter) || filter == "[]")
            {
                return tmpFilter;
            }

            try
            {
                tmpFilter = JsonConvert.DeserializeObject<DataFilter>(filter);

                if (tmpFilter == null) {
                    return Globals.DefaultDataFilter;
                }

                if (tmpFilter.PageSize > Globals.MaxRecordsPerRequest && siteAdmin == false)
                    tmpFilter.PageSize =Globals.MaxRecordsPerRequest;

                if (currentUserRoleWeight > 0 && siteAdmin == false)
                {   //the higher thier role the more records they can get in one call
                    tmpFilter.PageSize = currentUserRoleWeight * Globals.DefaultDataFilter.PageSize;
                }
            }
            catch (Exception ex)
            {
                _logger.InsertError(ex.Message, "ApiBaseController", "GetFilter");
                return tmpFilter;
            }
            return tmpFilter;
        }

        public float GetTimezoneOffset(string timeZone)
        {
            float offset = 0;
            _timesZones.ForEach(tz =>
            {
                for (int i = 0; i < tz.utc.Length; i++)
                {
                    if (tz.utc[i].EqualsIgnoreCase(timeZone))
                    {
                        offset = tz.offset;
                        return;
                    }
                }
            });
            return offset;
        }

        public User CurrentUser{ get; set; }

        public SessionManager SessionManager { get { return _sessionManager; } }

        private readonly NetworkHelper _network = new NetworkHelper();

        private readonly SessionManager _sessionManager = null;

        public User GetUser(AuthenticationHeaderValue authorization )
        {
            if(authorization == null)
                return null;

            return GetUser(authorization.Parameter);
        }

        public User GetUser(string authToken)
        {
            if (string.IsNullOrWhiteSpace(authToken))
                return null;    

            UserSession us = SessionManager.GetSession(authToken);
            if (us == null)
                return null;
            
            if (string.IsNullOrWhiteSpace(us.UserData))
                return null;

            this.CurrentUser = JsonConvert.DeserializeObject<User>(us.UserData);

            return CurrentUser;
        }

        public string GetClientIpAddress(HttpRequestMessage request)
        {
            return _network.GetClientIpAddress(Request);

        }
    }
}