// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using TreeMon.Data.Logging.Models;
using TreeMon.Managers.General;
using TreeMon.Models.App;
using TreeMon.Models.Datasets;
using TreeMon.Models.General;
using TreeMon.Utilites.Extensions;
using TreeMon.Web.Filters;
using TreeMon.WebAPI.Models;

namespace TreeMon.Web.api.v1
{
    public class StatusMessagesController : ApiBaseController
    {

        public StatusMessagesController()
        {

        }

        [ApiAuthorizationRequired(Operator = ">=", RoleWeight = 1)]
        [HttpPost]
        [Route("api/StatusMessages/Add")]
        [Route("api/StatusMessages/Insert")]
        public ServiceResult Insert(StatusMessage n)
            {
            if (CurrentUser == null)
                return ServiceResponse.Error("You must be logged in to access this function.");


            if (string.IsNullOrWhiteSpace(n.AccountUUID) || n.AccountUUID == SystemFlag.Default.Account)
                    n.AccountUUID = CurrentUser.AccountUUID;

                if (string.IsNullOrWhiteSpace(n.CreatedBy))
                    n.CreatedBy = CurrentUser.UUID;

                if (n.DateCreated == DateTime.MinValue)
                    n.DateCreated = DateTime.UtcNow;

            StatusMessageManager StatusMessageManager = new StatusMessageManager(Globals.DBConnectionKey,Request.Headers?.Authorization?.Parameter);

            return StatusMessageManager.Insert(n, true);
            }

        [ApiAuthorizationRequired(Operator = ">=", RoleWeight = 1)]
        [HttpPost]
        [HttpGet]
        [Route("api/StatusMessage/{name}")]
        public ServiceResult Get(string name)
                {
                    if (string.IsNullOrWhiteSpace(name))
                        return ServiceResponse.Error("You must provide a name for the StatusMessage.");

            StatusMessageManager StatusMessageManager = new StatusMessageManager(Globals.DBConnectionKey,Request.Headers?.Authorization?.Parameter);

            StatusMessage s = (StatusMessage)StatusMessageManager.Get(name);

                    if (s == null)
                        return ServiceResponse.Error("StatusMessage could not be located for the name " + name);

                    return ServiceResponse.OK("",s);
                }

        [ApiAuthorizationRequired(Operator = ">=", RoleWeight = 1)]
        [HttpPost]
        [HttpGet]
        [Route("api/StatusMessageBy/{uuid}")]
        public ServiceResult GetBy(string uuid)
        {
            if (string.IsNullOrWhiteSpace(uuid))
                return ServiceResponse.Error("You must provide a name for the StatusMessage.");

            StatusMessageManager StatusMessageManager = new StatusMessageManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);

            StatusMessage s = (StatusMessage)StatusMessageManager.Get(uuid);

            if (s == null)
                return ServiceResponse.Error("StatusMessage could not be located for the uuid " + uuid);

            return ServiceResponse.OK("", s);
        }

        [ApiAuthorizationRequired(Operator =">=" , RoleWeight = 4)]
        [HttpPost]
        [HttpGet]
        [Route("api/StatusMessages/Type/{statusType}")]
        public ServiceResult GetStatusMessages(string statusType, string filter = "")
        {
            if (CurrentUser == null)
                return ServiceResponse.Error("You must be logged in to access this function.");

            StatusMessageManager StatusMessageManager = new StatusMessageManager(Globals.DBConnectionKey,Request.Headers?.Authorization?.Parameter);

            List<dynamic> StatusMessages = StatusMessageManager.GetStatusByType(statusType, CurrentUser.UUID, CurrentUser.AccountUUID).OrderBy(ob => ob.Status).Cast<dynamic>().ToList();

            int count;
                            DataFilter tmpFilter = this.GetFilter(filter);
                StatusMessages = FilterEx.FilterInput(StatusMessages, tmpFilter, out count);

            return ServiceResponse.OK("", StatusMessages, count);
        }

        [ApiAuthorizationRequired(Operator =">=" , RoleWeight = 4)]
        [HttpPost]
        [HttpGet]
        [Route("api/StatusMessages/")]
        public ServiceResult GetStatusMessages(string filter = "")
        {
            if (CurrentUser == null)
                return ServiceResponse.Error("You must be logged in to access this function.");

            StatusMessageManager StatusMessageManager = new StatusMessageManager(Globals.DBConnectionKey,Request.Headers?.Authorization?.Parameter);


            List<dynamic> StatusMessages = StatusMessageManager.GetStatusMessages(CurrentUser.AccountUUID).Cast<dynamic>().ToList();

          int count;

                            DataFilter tmpFilter = this.GetFilter(filter);
                StatusMessages = FilterEx.FilterInput(StatusMessages, tmpFilter, out count);

            return ServiceResponse.OK("", StatusMessages, count);
        }

        [ApiAuthorizationRequired(Operator =">=" , RoleWeight = 4)]
        [HttpPost]
        [HttpDelete]
        [Route("api/StatusMessages/Delete")]
        public ServiceResult Delete(StatusMessage n)
                {
                    if (n == null || string.IsNullOrWhiteSpace(n.UUID))
                        return ServiceResponse.Error("Invalid account was sent.");

            StatusMessageManager StatusMessageManager = new StatusMessageManager(Globals.DBConnectionKey,Request.Headers?.Authorization?.Parameter);

            return StatusMessageManager.Delete(n);

                }

        [ApiAuthorizationRequired(Operator =">=" , RoleWeight = 4)]
        [HttpPost]
        [HttpPatch]
        [Route("api/StatusMessages/Update")]
        public ServiceResult Update(StatusMessage s)
        {
            if (s == null)
                return ServiceResponse.Error("Invalid StatusMessage sent to server.");

            StatusMessageManager StatusMessageManager = new StatusMessageManager(Globals.DBConnectionKey,Request.Headers?.Authorization?.Parameter);

            var dbS = (StatusMessage) StatusMessageManager.Get(s.UUID);

            if (dbS == null)
                return ServiceResponse.Error("StatusMessage was not found.");

            if (dbS.DateCreated == DateTime.MinValue)
                dbS.DateCreated = DateTime.UtcNow;
               
            dbS.Status = s.Status;

            return StatusMessageManager.Update(dbS);
        }
    }
}
