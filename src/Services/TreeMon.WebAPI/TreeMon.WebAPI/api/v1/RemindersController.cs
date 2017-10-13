// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web.Http;
using TreeMon.Data.Logging.Models;
using TreeMon.Managers;
using TreeMon.Managers.Event;
using TreeMon.Models.App;
using TreeMon.Models.Datasets;
using TreeMon.Models.Event;
using TreeMon.Utilites.Extensions;
using TreeMon.Web.Filters;
using TreeMon.Web.Models;
using TreeMon.WebAPI.Models;

namespace TreeMon.Web.api.v1
{
    public class RemindersController : ApiBaseController
    {
        public RemindersController()
        {

        }

        [ApiAuthorizationRequired(Operator = ">=", RoleWeight = 1)]
        [HttpPost]
        [Route("api/Reminders/Add")]
        [Route("api/Reminders/Insert")]
        public ServiceResult Insert(ReminderForm s)
        {
            if (s == null)
                return ServiceResponse.Error("Invalid data sent.");

            string authToken = Request.Headers.Authorization?.Parameter;
            SessionManager sessionManager = new SessionManager(Globals.DBConnectionKey);

            UserSession us = sessionManager.GetSession(authToken);
            if (us == null)
                return ServiceResponse.Error("You must be logged in to access this function.");

            if (us.Captcha?.ToUpper() != s.Captcha?.ToUpper())
                return ServiceResponse.Error("Invalid code.");

            if (CurrentUser == null)
                return ServiceResponse.Error("You must be logged in to access this function.");


            if (string.IsNullOrWhiteSpace(s.AccountUUID) || s.AccountUUID == SystemFlag.Default.Account)
                s.AccountUUID = CurrentUser.AccountUUID;

            if (string.IsNullOrWhiteSpace(s.CreatedBy))
                s.CreatedBy = CurrentUser.UUID;

            if (s.DateCreated == DateTime.MinValue)
                   s.DateCreated = DateTime.UtcNow;

            s.Active = true;
            s.Deleted = false;
            s.ReminderCount = 0; //its new, now reminders/notifications have taken place
            if( s.EventDateTime == null || s.EventDateTime == DateTime.MinValue)
                return ServiceResponse.Error("You must provide a valid event date.");

            if(string.IsNullOrWhiteSpace(s.Frequency))
                return ServiceResponse.Error("You must provide a valid frequency.");

            if (s.RepeatForever == false && s.RepeatCount <= 0)
                return ServiceResponse.Error("You must set  repeat count or repeat forver.");
        
            #region Convert to Reminder from ReminderForm because entity frameworks doesn't recognize casting.

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<ReminderForm, Reminder>();
            });

            IMapper mapper = config.CreateMapper();
            var dest = mapper.Map<ReminderForm, Reminder>(s);
            #endregion

            ReminderManager reminderManager = new ReminderManager(Globals.DBConnectionKey,Request.Headers?.Authorization?.Parameter);

            ServiceResult sr = reminderManager.Insert(dest, true);
            if (sr.Code != 200)
                return sr;

            string ruleErrors = "";
           
            int index = 1;
            foreach (ReminderRule r in s.ReminderRules)
            {
                DateTime dt;

                switch (r.RangeType?.ToUpper())
                {
                    case "DATE":
                        if (!DateTime.TryParse(r.RangeStart, out dt))
                            ruleErrors += "Rule " + index + " is not a valid start date.";
                        if (!DateTime.TryParse(r.RangeEnd, out dt))
                            ruleErrors += "Rule " + index + " is not a valid end date.";
                        break;

                    case "TIME":
                        if (!DateTime.TryParse(r.RangeStart, out dt))
                            ruleErrors += "Rule " + index + " is not a valid start time.";
                        if (!DateTime.TryParse(r.RangeEnd, out dt))
                            ruleErrors += "Rule " + index + " is not a valid end time.";
                        break;
                }
                index++;
                Debug.Assert(false, "CHECK MAKE SURE UUID IS SET");
                r.ReminderUUID = s.UUID;
                r.CreatedBy = CurrentUser.UUID;
                r.DateCreated = DateTime.UtcNow;
            }

            if(!string.IsNullOrWhiteSpace(ruleErrors))
                return ServiceResponse.Error(ruleErrors);

            index = 1;
            foreach (ReminderRule r in s.ReminderRules) {
               ServiceResult srr =   reminderManager.Insert(r);
                if(srr.Code!= 200)
                {
                    ruleErrors += "Rule " + index + " failed to save.";
                }
             }
            if (!string.IsNullOrWhiteSpace(ruleErrors))
                return ServiceResponse.Error(ruleErrors);

            return sr;
        }

        [ApiAuthorizationRequired(Operator = ">=", RoleWeight = 1)]
        [HttpPost]
        [HttpGet]
        [Route("api/Reminders/{name}")]
        public ServiceResult Get(string name )
        {
            if (string.IsNullOrWhiteSpace(name))
                return ServiceResponse.Error("You must provide a name for the Reminder.");

            ReminderManager reminderManager = new ReminderManager(Globals.DBConnectionKey,Request.Headers?.Authorization?.Parameter);

            Reminder s = (Reminder)reminderManager.Get(name);

            if (s == null)
                return ServiceResponse.Error("Reminder could not be located for the name " + name);

            return ServiceResponse.OK("",s);
        }

        [ApiAuthorizationRequired(Operator = ">=", RoleWeight = 1)]
        [HttpPost]
        [HttpGet]
        [Route("api/ReminderBy/{uuid}")]
        public ServiceResult GetBy(string uuid)
        {
            if (string.IsNullOrWhiteSpace(uuid))
                return ServiceResponse.Error("You must provide a name for the Reminder.");

            ReminderManager reminderManager = new ReminderManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);

            Reminder s = (Reminder)reminderManager.GetBy(uuid);

            if (s == null)
                return ServiceResponse.Error("Reminder could not be located for the uuid " + uuid);

            return ServiceResponse.OK("", s);
        }

        [ApiAuthorizationRequired(Operator =">=" , RoleWeight = 4)]
        [HttpPost]
        [HttpGet]
        [Route("api/Reminders/")]
        public ServiceResult GetReminders(string filter = "")
        {
            if (Request.Headers.Authorization == null || string.IsNullOrWhiteSpace(Request.Headers?.Authorization?.Parameter))
                return ServiceResponse.Error("You must be logged in to access this functionality.");

            if (CurrentUser == null)
                return ServiceResponse.Error("You must be logged in to access this function.");

            ReminderManager reminderManager = new ReminderManager(Globals.DBConnectionKey,Request.Headers?.Authorization?.Parameter);

            List<dynamic> Reminders =reminderManager.GetReminders(CurrentUser.AccountUUID).Cast<dynamic>().ToList();
            int count;

            DataFilter tmpFilter = this.GetFilter(filter);
            Reminders = FilterEx.FilterInput(Reminders, tmpFilter, out count);

            return ServiceResponse.OK("", Reminders, count);
        }

        [ApiAuthorizationRequired(Operator =">=" , RoleWeight = 4)]
        [HttpPost]
        [HttpDelete]
        [Route("api/Reminders/Delete")]
        public ServiceResult Delete(Reminder n)
        {
            if (n == null || string.IsNullOrWhiteSpace(n.UUID))
                return ServiceResponse.Error("Invalid account was sent.");

            ReminderManager reminderManager = new ReminderManager(Globals.DBConnectionKey,Request.Headers?.Authorization?.Parameter);

            return reminderManager.Delete(n);

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
        /// <param name="p"></param>
        /// <returns></returns>
        [ApiAuthorizationRequired(Operator =">=" , RoleWeight = 4)]
        [HttpPost]
        [HttpPatch]
        [Route("api/Reminders/Update")]
        public TreeMon.Models.App.ServiceResult Update(Reminder s)
        {
            if (s == null)
                return ServiceResponse.Error("Invalid Reminder sent to server.");

            ReminderManager reminderManager = new ReminderManager(Globals.DBConnectionKey,Request.Headers?.Authorization?.Parameter);

            var dbS = (Reminder) reminderManager.GetBy(s.UUID);

            if (dbS == null)
                return ServiceResponse.Error("Reminder was not found.");

           
            if (dbS.DateCreated == DateTime.MinValue)
                dbS.DateCreated = DateTime.UtcNow;
            dbS.Deleted = s.Deleted;
            dbS.Name = s.Name;
            dbS.Status = s.Status;
            dbS.SortOrder = s.SortOrder;
            dbS.Active = s.Active;
            dbS.Body = s.Body;
            dbS.EventDateTime = s.EventDateTime;
            dbS.RepeatForever = s.RepeatForever;
            dbS.RepeatCount = s.RepeatCount;
            dbS.Frequency = s.Frequency;
            return reminderManager.Update(dbS);
        } 
    }
}
