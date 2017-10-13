// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using TreeMon.Data.Logging.Models;
using TreeMon.Managers.Event;
using TreeMon.Models.App;
using TreeMon.Models.Datasets;
using TreeMon.Models.Event;
using TreeMon.Utilites.Extensions;
using TreeMon.Web.Filters;
using TreeMon.WebAPI.Models;

namespace TreeMon.Web.api.v1
{
    public class NotificationsController : ApiBaseController
    {
        public NotificationsController()
        {


        }

        [ApiAuthorizationRequired(Operator = ">=", RoleWeight = 4)]
        [HttpPost]
        [Route("api/Notifications/Add")]
        [Route("api/Notifications/Insert")]
        public ServiceResult Insert(Notification s)
        {
            if (CurrentUser == null)
                return ServiceResponse.Error("You must be logged in to access this function.");


            if (string.IsNullOrWhiteSpace(s.AccountUUID) || s.AccountUUID == SystemFlag.Default.Account)
                s.AccountUUID = CurrentUser.AccountUUID;

            if (string.IsNullOrWhiteSpace(s.CreatedBy))
                s.CreatedBy = CurrentUser.UUID;

            if (s.DateCreated == DateTime.MinValue)
                s.DateCreated = DateTime.UtcNow;

            if (string.IsNullOrWhiteSpace(s.FromUUID))
            {
                s.FromUUID = GetClientIpAddress(Request);
                s.FromType = "ip";
            }
            NotificationManager NotificationManager = new NotificationManager(Globals.DBConnectionKey,Request.Headers?.Authorization?.Parameter);
            return NotificationManager.Insert(s, true);
        }

        [ApiAuthorizationRequired(Operator = ">=", RoleWeight = 1)]
        [HttpPost]
        [HttpGet]
        [Route("api/Notifications/{name}")]
        public ServiceResult Get(string name )
        {
            if (string.IsNullOrWhiteSpace(name))
                return ServiceResponse.Error("You must provide a name for the Notification.");

            NotificationManager NotificationManager = new NotificationManager(Globals.DBConnectionKey,Request.Headers?.Authorization?.Parameter);
            Notification s = (Notification)NotificationManager.Get(name);

            if (s == null)
                return ServiceResponse.Error("Notification could not be located for the name " + name);

            return ServiceResponse.OK("",s);
        }

        [ApiAuthorizationRequired(Operator = ">=", RoleWeight = 1)]
        [HttpPost]
        [HttpGet]
        [Route("api/NotificationsBy/{uuid}")]
        public ServiceResult GetBy(string uuid)
        {
            if (string.IsNullOrWhiteSpace(uuid))
                return ServiceResponse.Error("You must provide a name for the Notification.");

            NotificationManager NotificationManager = new NotificationManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);
            Notification s = (Notification)NotificationManager.GetBy(uuid);

            if (s == null)
                return ServiceResponse.Error("Notification could not be located for the uuid " + uuid);

            return ServiceResponse.OK("", s);
        }

        [ApiAuthorizationRequired(Operator =">=" , RoleWeight = 4)]
        [HttpPost]
        [HttpGet]
        [Route("api/Notifications/")]
        public ServiceResult GetNotifications(string filter = "")
        {
            if (Request.Headers.Authorization == null || string.IsNullOrWhiteSpace(Request.Headers?.Authorization?.Parameter))
                return ServiceResponse.Error("You must be logged in to access this functionality.");

            if (CurrentUser == null)
                return ServiceResponse.Error("You must be logged in to access this function.");

            NotificationManager NotificationManager = new NotificationManager(Globals.DBConnectionKey,Request.Headers?.Authorization?.Parameter);
            List<dynamic> Notifications = NotificationManager.GetNotifications(CurrentUser.AccountUUID).Cast<dynamic>().ToList();
            int count;

                            DataFilter tmpFilter = this.GetFilter(filter);
                Notifications = FilterEx.FilterInput(Notifications, tmpFilter, out count);

            return ServiceResponse.OK("", Notifications, count);
        }

        [ApiAuthorizationRequired(Operator =">=" , RoleWeight = 4)]
        [HttpPost]
        [HttpDelete]
        [Route("api/Notifications/Delete")]
        public ServiceResult Delete(Notification n)
        {
            if (n == null || string.IsNullOrWhiteSpace(n.UUID))
                return ServiceResponse.Error("Invalid account was sent.");

            NotificationManager NotificationManager = new NotificationManager(Globals.DBConnectionKey,Request.Headers?.Authorization?.Parameter);
            return NotificationManager.Delete(n);
        }

       /// <summary>
       /// Fields updated..
       ///     Category
       ///     Name
       ///     Cost
       ///     Price
       ///     Weight
       ///     WeightUOM
       /// </summary>
       /// <param name = "p" ></ param >
       /// < returns ></ returns >
         [ApiAuthorizationRequired(Operator =">=" , RoleWeight = 4)]
       [HttpPost]
        [HttpPatch]
        [Route("api/Notifications/Update")]
       public ServiceResult Update(Notification s)
        {
            if (s == null)
                return ServiceResponse.Error("Invalid Notification sent to server.");

            NotificationManager NotificationManager = new NotificationManager(Globals.DBConnectionKey,Request.Headers?.Authorization?.Parameter);
            var dbS = (Notification)NotificationManager.GetBy(s.UUID);

            if (dbS == null)
                return ServiceResponse.Error("Notification was not found.");

            if (dbS.DateCreated == DateTime.MinValue)
                dbS.DateCreated = DateTime.UtcNow;

       

            dbS.Deleted = s.Deleted;
            dbS.Name = s.Name;
            dbS.Status = s.Status;
            dbS.SortOrder = s.SortOrder;


            return NotificationManager.Update(dbS);
        }

    }
}
