// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using TreeMon.Models.App;
using TreeMon.Utilites.Helpers;
using TreeMon.Web.Filters;

namespace TreeMon.Web.api.v1
{
    public class SessionsController : ApiBaseController
    {
        [ApiAuthorizationRequired(Operator =">=" , RoleWeight = 4)]
        [HttpGet]
        [Route("api/Sessions/Status/{sessionId}")]
        public ServiceResult GetStatus(string sessionId)
        {
            if (string.IsNullOrWhiteSpace(sessionId))
                return ServiceResponse.Error("You must send a valid session id.");

            UserSession us =    SessionManager.GetSession(sessionId);

            if (us == null)
                return ServiceResponse.Error("Invalid or expired user session.");

            return ServiceResponse.OK();
        }

        [ApiAuthorizationRequired(Operator =">=" , RoleWeight = 4)]
        [HttpPost]
        [HttpDelete]
        [Route("api/Sessions/Delete")]
        public ServiceResult DeleteSesssion()
        {
            string root = EnvironmentEx.AppDataFolder;
            var provider = new MultipartFormDataStreamProvider(root);
            ServiceResult res = new ServiceResult();
            try
            {
                  Task<string> content = ActionContext.Request.Content.ReadAsStringAsync();
             
                if (content == null)
                    return ServiceResponse.Error("No session data was sent.");

                string body = content.Result;

                if (string.IsNullOrEmpty(body))
                    return ServiceResponse.Error("You must send valid session data.");

                dynamic  sessionInfo = JsonConvert.DeserializeObject<dynamic>(body);

                if (sessionInfo == null)
                    return ServiceResponse.Error("Invalid session data.");

                string sessionID = sessionInfo.SessionId.ToString();

                UserSession us = SessionManager.GetSession(sessionID, false);

                if (us == null) //session already cleared.
                    return ServiceResponse.OK();

                if (us.UserUUID != sessionInfo.UserUUID?.ToString())
                    return ServiceResponse.Error("You are not authorized to end the session.");

                if(! SessionManager.DeleteSession(sessionID))
                    return ServiceResponse.Error("Error occurred deleting session.");
            }
            catch (Exception ex)
            {
                Debug.Assert(false, ex.Message);
                return ServiceResponse.Error(ex.Message);
            }
            return ServiceResponse.OK();
        }

    }
}
