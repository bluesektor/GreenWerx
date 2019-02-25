// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
using AutoMapper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Http;
using TreeMon.Data.Logging.Models;
using TreeMon.Managers;
using TreeMon.Managers.Events;
using TreeMon.Models;
using TreeMon.Models.App;
using TreeMon.Models.Datasets;
using TreeMon.Models.Events;
using TreeMon.Utilites.Extensions;
using TreeMon.Utilites.Helpers;
using TreeMon.Utilites.Security;
using TreeMon.Web.Filters;
using TreeMon.Web.Models;
using TreeMon.WebAPI.Models;
using WebApi.OutputCache.V2;
using WebApiThrottle;

namespace TreeMon.Web.api.v1
{
    public class EventsController : ApiBaseController
    {
      
        public EventsController()
        {
            
          
        }

        [ApiAuthorizationRequired(Operator = ">=", RoleWeight = 1)]
        [HttpPost]
        [Route("api/Events/Add")]
        [Route("api/Events/Insert")]
        public ServiceResult Insert()//Event s)
        {
            try
            {
                string body = ActionContext.Request.Content.ReadAsStringAsync().Result;
                //  if (content == null)
                //      return ServiceResponse.Error("No permissions were sent.");

                // string body = content.Result;

                if (string.IsNullOrEmpty(body))
                    return ServiceResponse.Error("No permissions were sent.");

                Event s = JsonConvert.DeserializeObject<Event>(body);

                if (s == null)
                    return ServiceResponse.Error("Invalid event posted to server.");

                string authToken = Request.Headers.Authorization?.Parameter;
                SessionManager sessionManager = new SessionManager(Globals.DBConnectionKey);

                UserSession us = sessionManager.GetSession(authToken);
                if (us == null)
                    return ServiceResponse.Error("You must be logged in to access this function.");

                //  if (us.Captcha?.ToUpper() != s.Captcha?.ToUpper())                return ServiceResponse.Error("Invalid code.");

                if (CurrentUser == null)
                    return ServiceResponse.Error("You must be logged in to access this function.");


                if (string.IsNullOrWhiteSpace(s.AccountUUID) || s.AccountUUID == SystemFlag.Default.Account)
                    s.AccountUUID = CurrentUser.AccountUUID;

                if (string.IsNullOrWhiteSpace(s.CreatedBy))
                    s.CreatedBy = CurrentUser.UUID;

                if (s.DateCreated == DateTime.MinValue)
                    s.DateCreated = DateTime.UtcNow;

                if (string.IsNullOrWhiteSpace(s.HostAccountUUID))
                    s.HostAccountUUID = CurrentUser.AccountUUID;

                s.Active = true;
                s.Deleted = false;
                if (s.EventDateTime == null || s.EventDateTime == DateTime.MinValue)
                {
                    if (s.StartDate == null || s.StartDate == DateTime.MinValue)
                        return ServiceResponse.Error("You must provide a valid event start date.");
                    else
                        s.EventDateTime = s.StartDate;
                }

                //if (string.IsNullOrWhiteSpace(s.Frequency)) return ServiceResponse.Error("You must provide a valid frequency.");

                //if (s.RepeatForever == false && s.RepeatCount <= 0) return ServiceResponse.Error("You must set  repeat count or repeat forver.");

                EventManager EventManager = new EventManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);

                ServiceResult sr = EventManager.Insert(s);
                if (sr.Code != 200)
                    return sr;

                return sr;
            }
            catch (Exception ex)
            {
                return ServiceResponse.Error(ex.Message);
            }
        }

        [CacheOutput(ClientTimeSpan = 100, ServerTimeSpan = 100)]
        [EnableThrottling(PerSecond = 1, PerMinute = 20, PerHour = 200, PerDay = 1500, PerWeek = 3000)]
        [AllowAnonymous]
        [HttpPost]
        [HttpGet]
        [Route("api/Events/{name}")]
        public ServiceResult Get(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return ServiceResponse.Error("You must provide a name for the Event.");

            EventManager EventManager = new EventManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);

            List<Event> s = EventManager.Search(name);

            if (s == null || s.Count == 0)
                return ServiceResponse.Error("Event could not be located for the name " + name);

            return ServiceResponse.OK("", s);
        }

        [CacheOutput(ClientTimeSpan = 100, ServerTimeSpan = 100)]
        [EnableThrottling(PerSecond = 1, PerMinute = 20, PerHour = 200, PerDay = 1500, PerWeek = 3000)]
        [AllowAnonymous]
        [HttpPost]
        [HttpGet]
        [Route("api/EventBy/{uuid}")]
        public ServiceResult GetBy(string uuid)
        {
            if (string.IsNullOrWhiteSpace(uuid))
                return ServiceResponse.Error("You must provide a name for the Event.");

            EventManager EventManager = new EventManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);

            Event s = (Event)EventManager.Get(uuid);

            if (s == null)
                return ServiceResponse.Error("Event could not be located for the uuid " + uuid);

            /// Technically an event can have more than one location,
            /// for simplicity the ui is restricted to one.
            s.EventLocationUUID = EventManager.GetEventLocations(s.UUID)?.FirstOrDefault()?.UUID;
            return ServiceResponse.OK("", s);
        }

        [CacheOutput(ClientTimeSpan = 100, ServerTimeSpan = 100)]
        [AllowAnonymous]
        [EnableThrottling(PerSecond = 3)]
        [HttpPost]
        [HttpGet]
        [Route("api/Events")]
        public ServiceResult GetEvents( )
        {
            DataFilter tmpFilter = this.GetFilter(Request);
           
            int count;
            string devaultEventUUID = Globals.Application.AppSetting("DefaultEvent");
            EventManager EventManager = new EventManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);

            
            List<dynamic> Events;
         
            TimeZoneInfo tzInfo = null;

            try
            {
                if (string.IsNullOrWhiteSpace(tmpFilter.TimeZone))
                {
                    var defaultTimeZone = TimeZoneInfo.GetSystemTimeZones().FirstOrDefault(w => w.BaseUtcOffset.TotalHours < -9 && w.BaseUtcOffset.TotalHours > -12);
                    tzInfo = TimeZoneInfo.CreateCustomTimeZone(defaultTimeZone.StandardName, new TimeSpan(Convert.ToInt32(defaultTimeZone.BaseUtcOffset.TotalHours), 0, 0),
                        defaultTimeZone.StandardName, defaultTimeZone.StandardName);
                }
                else
                {
                    float offSet = this.GetTimezoneOffset(tmpFilter.TimeZone);
                    tzInfo = TimeZoneInfo.CreateCustomTimeZone(tmpFilter.TimeZone, new TimeSpan(Convert.ToInt32(offSet), 0, 0), tmpFilter.TimeZone, tmpFilter.TimeZone);
                }
            }
            catch
            {
               // tzInfo = TimeZoneInfo.FindSystemTimeZoneById(timeZone);
            }

            DateTime adjustedDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow.Date, tzInfo);
            Events = EventManager.GetSubEvents(devaultEventUUID, true,false, tmpFilter.IncludePrivate)?.Where(w => w.EndDate >= adjustedDate.Date)
                                    .Cast<dynamic>().ToList();

          
            Events = Events.Filter(tmpFilter, out count);
            return ServiceResponse.OK("", Events, count);
          
        }

        [CacheOutput(ClientTimeSpan = 100, ServerTimeSpan = 100)]
        [AllowAnonymous]
        [EnableThrottling(PerSecond = 3)]
        [HttpPost]
        [HttpGet]
        [Route("api/Events/Hosts/{accountUUID}")]
        public ServiceResult GetAccountEvents(string accountUUID)
        {
            if (string.IsNullOrWhiteSpace(accountUUID))
                return ServiceResponse.Error("You must send an account uuid.");

            DataFilter tmpFilter = this.GetFilter(Request);

            int count;
            EventManager EventManager = new EventManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);

            List<dynamic> Events;

            TimeZoneInfo tzInfo = null;

            try
            {
                if (string.IsNullOrWhiteSpace(tmpFilter.TimeZone))
                {
                    var defaultTimeZone = TimeZoneInfo.GetSystemTimeZones().FirstOrDefault(w => w.BaseUtcOffset.TotalHours < -9 && w.BaseUtcOffset.TotalHours > -12);
                    tzInfo = TimeZoneInfo.CreateCustomTimeZone(defaultTimeZone.StandardName, new TimeSpan(Convert.ToInt32(defaultTimeZone.BaseUtcOffset.TotalHours), 0, 0),
                        defaultTimeZone.StandardName, defaultTimeZone.StandardName);
                }
                else
                {
                    float offSet = this.GetTimezoneOffset(tmpFilter.TimeZone);
                    tzInfo = TimeZoneInfo.CreateCustomTimeZone(tmpFilter.TimeZone, new TimeSpan(Convert.ToInt32(offSet), 0, 0), tmpFilter.TimeZone, tmpFilter.TimeZone);
                }
            }
            catch
            {
                // tzInfo = TimeZoneInfo.FindSystemTimeZoneById(timeZone);
            }

            DateTime adjustedDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow.Date, tzInfo);
            Events = EventManager.GetHostEvents( accountUUID, tmpFilter.IncludeDeleted, tmpFilter.IncludePrivate)?.Where(w => w.StartDate >= adjustedDate.Date)
                                    .Cast<dynamic>().ToList();


            Events = Events.Filter(tmpFilter, out count);
            return ServiceResponse.OK("", Events, count);

        }

        [EnableThrottling(PerSecond = 3)]
        [HttpPost]
        [HttpGet]
        [Route("api/Events/Favorites")]
        public ServiceResult GetFavoriteEvents()
        {
            DataFilter tmpFilter = this.GetFilter(Request);

            if (CurrentUser == null)
                return ServiceResponse.Error("You must be logged in to view favorites.");

            int count;
            EventManager EventManager = new EventManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);

            List<dynamic> Events = EventManager.GetFavoriteEvents(CurrentUser.UUID, CurrentUser.AccountUUID);
            Events = Events.Filter(tmpFilter, out count);
            return ServiceResponse.OK("", Events, count);
        }

        [CacheOutput(ClientTimeSpan = 100, ServerTimeSpan = 100)]
        [AllowAnonymous]
        [EnableThrottling(PerSecond = 3)]
        [HttpGet]
        [Route("api/Events/Categories")]
        public ServiceResult GetEventCategories()
        {
            EventManager EventManager = new EventManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);
            List<string> categories = EventManager.GetEventCategories(); 
            return ServiceResponse.OK("", categories, categories?.Count ?? 0);
        }



        [ApiAuthorizationRequired(Operator = ">=", RoleWeight = 4)]
        [HttpPost]
        [HttpDelete]
        [Route("api/Events/Delete/{eventUUID}")]
        public ServiceResult Delete(string eventUUID)
        {
            if ( string.IsNullOrWhiteSpace(eventUUID))
                return ServiceResponse.Error("Invalid event was sent.");

            EventManager EventManager = new EventManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);

            var n = EventManager.Get(eventUUID);
            if(n == null)
                return ServiceResponse.Error("Invalid event id.");

            var res = EventManager.Delete(n);
            if (res.Code > 200)
                return res;

            ReminderManager reminderManager = new ReminderManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);
            reminderManager.DeleteForEvent(n.UUID, "The event has been deleted.", false);

            return res;
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
        [ApiAuthorizationRequired(Operator = ">=", RoleWeight = 4)]
        [HttpPost]
        [HttpPatch]
        [Route("api/Events/Update")]
        public TreeMon.Models.App.ServiceResult Update(Event s)
        {
            if (s == null)
                return ServiceResponse.Error("Invalid Event sent to server.");

            EventManager EventManager = new EventManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);

            var dbS = (Event)EventManager.Get(s.UUID);

            if (dbS == null)
                return ServiceResponse.Error("Event was not found.");


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
            dbS.Category = s.Category;
            return EventManager.Update(dbS);
        }

        [CacheOutput(ClientTimeSpan = 100, ServerTimeSpan = 100)]
        [AllowAnonymous]
        [EnableThrottling(PerSecond = 3)]
        [HttpPost]
        [HttpGet]
        [Route("api/Events/Locations/{eventLocationUUID}")]
        public ServiceResult GetLocation(string eventLocationUUID)
        {
            EventManager EventManager = new EventManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);
            EventLocation location = EventManager.GetEventLocation(eventLocationUUID);
            return ServiceResponse.OK("", location);
        }


        /// <summary>
        /// gets locations the "host" used in the past.
        /// </summary>
        /// <param name="eventLocationUUID"></param>
        /// <returns></returns>
        [EnableThrottling(PerSecond = 1, PerMinute = 20, PerHour = 200, PerDay = 1500, PerWeek = 3000)]
        [ApiAuthorizationRequired(Operator = ">=", RoleWeight = 1)]
        [EnableThrottling(PerSecond = 3)]
        [HttpPost]
        [HttpGet]
        [Route("api/Events/Locations/Account")]
        public ServiceResult GetAccountEventLocations()
        {
            EventManager EventManager = new EventManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);
            var locations = EventManager.GetAccountEventLocations(CurrentUser?.AccountUUID);
            return ServiceResponse.OK("", locations);
        }


        
        [AllowAnonymous]
        [EnableThrottling(PerSecond = 1)]
        [HttpPost]
        [HttpGet]
        [Route("api/Events/Location/Save")]
        public ServiceResult SaveLocation()//EventLocation eventLocation)
        {
            try
            {
                string body = ActionContext.Request.Content.ReadAsStringAsync().Result;
                //  if (content == null)
                //      return ServiceResponse.Error("No permissions were sent.");

                // string body = content.Result;

                if (string.IsNullOrEmpty(body))
                    return ServiceResponse.Error("No permissions were sent.");

                EventLocation eventLocation = JsonConvert.DeserializeObject<EventLocation>(body);

                if (eventLocation == null)
                    return ServiceResponse.Error("Invalid location posted to server.");

                string authToken = Request.Headers?.Authorization?.Parameter;
                SessionManager sessionManager = new SessionManager(Globals.DBConnectionKey);

                UserSession us = sessionManager.GetSession(authToken);
                if (us == null)
                    return ServiceResponse.Error("You must be logged in to access this function.");

                if (string.IsNullOrWhiteSpace(us.UserData))
                    return ServiceResponse.Error("Couldn't retrieve user data.");

                if (CurrentUser == null)
                    return ServiceResponse.Error("You must be logged in to access this function.");

                if (string.IsNullOrWhiteSpace(eventLocation.CreatedBy))
                {
                    eventLocation.CreatedBy = CurrentUser.UUID;
                    eventLocation.AccountUUID = CurrentUser.AccountUUID;
                    eventLocation.DateCreated = DateTime.UtcNow;
                }

                
                if (string.IsNullOrWhiteSpace(eventLocation.Email) && eventLocation.CreatedBy == CurrentUser.UUID)
                    eventLocation.Email = Cipher.Crypt(Globals.Application.AppSetting("AppKey"), CurrentUser.Email, true);

                EventManager EventManager = new EventManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);
                return EventManager.Save(eventLocation);
            }
            catch (Exception ex)
            {
                return ServiceResponse.Error("Failed to save event location.");
            }
        }


        [ApiAuthorizationRequired(Operator = ">=", RoleWeight = 0)]
        [HttpPost]
        [Route("api/Events/{eventUUID}/Favorite")]
        public ServiceResult AddEventToFavorites(string eventUUID)
        {
            if (string.IsNullOrWhiteSpace(eventUUID))
                return ServiceResponse.Error("No event id sent.");

            if (CurrentUser == null)
                return ServiceResponse.Error("You must be logged in to access this function.");

            EventManager EventManager = new EventManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);
            var temp = EventManager.Get(eventUUID);
            if (temp == null)
                return ServiceResponse.Error("Event does not exist for " + eventUUID);

            var e = (Event)temp;

            Reminder r = new Reminder()
            {
                Favorite = true,
                RoleOperation = e.RoleOperation,
                RoleWeight = e.RoleWeight,
                Private = true,
                Name = e.Name,
                EventUUID = e.UUID,
                UUIDType = e.UUIDType,
                AccountUUID = CurrentUser.AccountUUID,
                CreatedBy = CurrentUser.UUID,
                DateCreated = DateTime.UtcNow,
                RepeatCount = 0,
                RepeatForever = false,
                Active = true,
                Deleted = false,
                ReminderCount = 0, //its new, now reminders/notifications have taken place
                EventDateTime = e.StartDate//,
               // StartDate = e.StartDate,
               // EndDate = e.EndDate
            };

            ReminderManager reminderManager = new ReminderManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);
            return reminderManager.Insert(r);
        }

        [ApiAuthorizationRequired(Operator = ">=", RoleWeight = 0)]
        [HttpDelete]
        [Route("api/Events/{eventUUID}/Favorite")]
        public ServiceResult DeleteEventFromFavorites(string eventUUID)
        {
            if (string.IsNullOrWhiteSpace(eventUUID))
                return ServiceResponse.Error("No id sent.");

            if (CurrentUser == null)
                return ServiceResponse.Error("You must be logged in to access this function.");

           
            ReminderManager reminderManager = new ReminderManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);
            var r = reminderManager.GetByEvent(eventUUID);
            if (r == null)
                return ServiceResponse.Error("Record does not exist for id.");
            return reminderManager.Delete(r, true);
        }


    }
}

                        