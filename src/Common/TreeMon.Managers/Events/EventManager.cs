// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TreeMon.Data;
using TreeMon.Models;
using TreeMon.Models.App;
using TreeMon.Models.Events;
using TreeMon.Models.Geo;
using TreeMon.Utilites.Extensions;
using TreeMon.Utilites.Helpers;
using TreeMon.Utilites.Security;

namespace TreeMon.Managers.Events
{
    public class EventManager : BaseManager, ICrud
    {
        public EventManager(string connectionKey, string sessionKey) : base(connectionKey, sessionKey)
        {
            Debug.Assert(!string.IsNullOrWhiteSpace(connectionKey), "EventManager CONTEXT IS NULL!");
            this._connectionKey = connectionKey;
        }

        public ServiceResult Delete(INode n, bool purge = false)
        {
            ServiceResult res = ServiceResponse.OK();

            if (n == null)
                return ServiceResponse.Error("No record sent.");

            if (!this.DataAccessAuthorized(n, "DELETE", false)) return ServiceResponse.Error("You are not authorized this action.");

            var s = (Event)n;

            if (purge)
            {
                using (var context = new TreeMonDbContext(this._connectionKey))
                {
                    if (context.Delete<Event>(s) == 0)
                        return ServiceResponse.Error(s.Name + " failed to delete. ");
                }
            }

            //get the Event from the table with all the data so when its updated it still contains the same data.
            s = (Event)this.Get(s.UUID);
            if (s == null)
                return ServiceResponse.Error("Event not found");

            s.Deleted = true;
            using (var context = new TreeMonDbContext(this._connectionKey))
            {
                if (context.Update<Event>(s) == 0)
                    return ServiceResponse.Error(s.Name + " failed to delete. ");
            }
            return res;
        }

        public List<Event> Search(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return new List<Event>();
          
            using (var context = new TreeMonDbContext(this._connectionKey))
            {
                return context.GetAll<Event>()?.Where(sw => sw.Name.EqualsIgnoreCase(name)).ToList();
            }
            //////if (!this.DataAccessAuthorized(s, "GET", false)) return ServiceResponse.Error("You are not authorized this action.");
        }

        public List<Event> GetEvents(  bool deleted = false)
        {
            try
            {
                using (var context = new TreeMonDbContext(this._connectionKey))
                {
                    return context.GetAll<Event>()?.Where(sw => sw?.Deleted == deleted).OrderBy(ob => ob?.StartDate)?.ToList();
                }
                //////if (!this.DataAccessAuthorized(s, "GET", false)) return ServiceResponse.Error("You are not authorized this action.");
            }
            catch (Exception ex)
            {
                Debug.Assert(false, ex.Message);
            }
            return new List<Event>();
        }


        public List<string> GetEventCategories(bool deleted = false)
        {
            try
            {
                using (var context = new TreeMonDbContext(this._connectionKey))
                {
                    return context.GetAll<Event>()?.Where(sw =>
                            sw.Private == false &&
                            sw?.Deleted == deleted)
                        ?.OrderBy(o => o.Category)
                        ?.Select(s => s.Category)
                        ?.Distinct()
                        ?.ToList();
                }
                //////if (!this.DataAccessAuthorized(s, "GET", false)) return ServiceResponse.Error("You are not authorized this action.");
            }
            catch (Exception ex)
            {
                Debug.Assert(false, ex.Message);
            }
            return new List<string>();
        }
       

        public List<Event> GetSubEvents(string parentUUID, bool includeParent = false, bool deleted = false, bool includePrivate = false)
        {
            try
            {
                if (includePrivate)
                { //if looking at private then check permission
                    if (!this.DataAccessAuthorized("EVENT", false, includePrivate))
                    {
                        includePrivate = false;
                        // if not allowed private then check if they can see anything..
                        if (!this.DataAccessAuthorized("EVENT", false, includePrivate))
                            return new List<Event>();
                    }
                }
                using (var context = new TreeMonDbContext(this._connectionKey))
                {
                    if (includeParent)
                    {
                        var t = context.GetAll<Event>()?.Where(sw =>
                             (sw.UUID == parentUUID || sw.UUParentID == parentUUID) &&
                             ( sw.Private == false || sw.Private == includePrivate  ) &&
                            sw?.Deleted == deleted).OrderBy(ob => ob?.StartDate)?.ToList();
                        return t;
                    }

                    var x = context.GetAll<Event>()?.Where(sw => 
                        sw.UUParentID == parentUUID &&
                        (sw.Private == false || sw.Private == includePrivate) && 
                        sw?.Deleted == deleted).OrderBy(ob => ob?.StartDate)?.ToList();

                    return x;
                }
                //////if (!this.DataAccessAuthorized(s, "GET", false)) return ServiceResponse.Error("You are not authorized this action.");
            }
            catch (Exception ex)
            {
                Debug.Assert(false, ex.Message);
            }
            return new List<Event>();
        }

        public List<Event> GetHostEvents(string accountUUID, bool deleted = false, bool includePrivate = false)
        {
            try
            {
                if (includePrivate)
                { //if looking at private then check permission
                    if (!this.DataAccessAuthorized("EVENT", false, includePrivate))
                    {
                        includePrivate = false;
                        // if not allowed private then check if they can see anything..
                        if (!this.DataAccessAuthorized("EVENT", false, includePrivate))
                            return new List<Event>();
                    }
                }
                using (var context = new TreeMonDbContext(this._connectionKey))
                {
                
                    var ets = context.GetAll<Event>()?.Where(sw =>
                        sw.HostAccountUUID == accountUUID &&
                        (sw.Private == false || sw.Private == includePrivate) &&
                        sw?.Deleted == deleted).OrderBy(ob => ob?.StartDate)?.ToList();
                    return ets;
                }
                //////if (!this.DataAccessAuthorized(s, "GET", false)) return ServiceResponse.Error("You are not authorized this action.");
            }
            catch (Exception ex)
            {
                Debug.Assert(false, ex.Message);
            }
            return new List<Event>();
        }

        public List<Event> GetPrivateSubEvents(string parentUUID, string userUUID, string accountUUID, bool includeParent = false, bool deleted = false)
        {
            try
            {
                using (var context = new TreeMonDbContext(this._connectionKey))
                {
                    if (includeParent)
                    {
                        return context.GetAll<Event>()?.Where(sw =>
                            (sw.UUID == parentUUID || sw.UUParentID == parentUUID) &&
                            (sw.Private == true && sw.CreatedBy == userUUID && sw.AccountUUID == accountUUID ) &&
                            sw?.Deleted == deleted).OrderBy(ob => ob?.StartDate)?.ToList();
                    }

                    return context.GetAll<Event>()?.Where(sw =>
                        sw.UUParentID == parentUUID &&
                        (sw.Private == true && sw.CreatedBy == userUUID && sw.AccountUUID == accountUUID) &&
                        sw?.Deleted == deleted).OrderBy(ob => ob?.StartDate)?.ToList();
                }
                //////if (!this.DataAccessAuthorized(s, "GET", false)) return ServiceResponse.Error("You are not authorized this action.");
            }
            catch (Exception ex)
            {
                Debug.Assert(false, ex.Message);
            }
            return new List<Event>();
        }

        public INode Get(string uuid)
        {
            if (string.IsNullOrWhiteSpace(uuid))
                return null;
            using (var context = new TreeMonDbContext(this._connectionKey))
            {
                return context.GetAll<Event>()?.FirstOrDefault(sw => sw?.UUID == uuid);
            }
            ////if (!this.DataAccessAuthorized(s, "GET", false)) return ServiceResponse.Error("You are not authorized this action.");
        }

        public ServiceResult Insert(INode n)
        {
            if (!this.DataAccessAuthorized(n, "post", false)) return ServiceResponse.Error("You are not authorized this action.");

            n.Initialize(this._requestingUser.UUID, this._requestingUser.AccountUUID, this._requestingUser.RoleWeight);

            var s = (Event)n;
            try
            {
                using (var context = new TreeMonDbContext(this._connectionKey))
                {
                    Event dbU = context.GetAll<Event>()?.FirstOrDefault(wu => (wu?.Name?.EqualsIgnoreCase(s?.Name) ?? false) && wu?.AccountUUID == s?.AccountUUID);

                    if (dbU != null)
                        return ServiceResponse.Error("Event already exists.");


                    if (context.Insert<Event>(s))
                        return ServiceResponse.OK("", s);
                }
            }
            catch (Exception ex)
            {
                Debug.Assert(false, ex.Message);
            }
            return ServiceResponse.Error("An error occurred inserting Event " + s.Name);
        }

        public ServiceResult Update(INode n)
        {
            if (n == null)
                return ServiceResponse.Error("Invalid Event data.");

            if (!this.DataAccessAuthorized(n, "PATCH", false)) return ServiceResponse.Error("You are not authorized this action.");

            var s = (Event)n;

            using (var context = new TreeMonDbContext(this._connectionKey))
            {
                if (context.Update<Event>(s) > 0)
                    return ServiceResponse.OK("",s);
            }
            return ServiceResponse.Error("System error, Event was not updated.");
        }

        public ServiceResult UpdateGroup(INode n)
        {
            if (n == null)
                return ServiceResponse.Error("Invalid Event group.");

            if (!this.DataAccessAuthorized(n, "PATCH", false)) return ServiceResponse.Error("You are not authorized this action.");

            var s = (EventGroup)n;

            using (var context = new TreeMonDbContext(this._connectionKey))
            {
                if (context.Update<EventGroup>(s) > 0)
                    return ServiceResponse.OK();
            }
            return ServiceResponse.Error("System error, Event was not updated.");
        }

        public ServiceResult InsertEventGroup(INode n)
        {
            if (!this.DataAccessAuthorized(n, "post", false)) return ServiceResponse.Error("You are not authorized this action.");

            n.Initialize(this._requestingUser.UUID, this._requestingUser.AccountUUID, this._requestingUser.RoleWeight);

            var s = (EventGroup)n;

            using (var context = new TreeMonDbContext(this._connectionKey))
            {
                EventGroup dbU = context.GetAll<EventGroup>()?.FirstOrDefault(wu => (wu.Name?.EqualsIgnoreCase(s.Name) ?? false) && wu.AccountUUID == s.AccountUUID);

                if (dbU != null)
                    return ServiceResponse.Error("EventGroup already exists.");


                if (context.Insert<EventGroup>(s))
                    return ServiceResponse.OK("", s);
            }
            return ServiceResponse.Error("An error occurred inserting EventGroup " + s.Name);
        }

        public List<EventItem> GetEventInventory(string eventUUID, bool deleted = false)
        {
            try
            {
                using (var context = new TreeMonDbContext(this._connectionKey))
                {
                    return context.GetAll<EventItem>()?.Where(sw => sw.EventUUID == eventUUID && sw?.Deleted == deleted).OrderBy(ob => ob?.Name)?.ToList();
                }
                //////if (!this.DataAccessAuthorized(s, "GET", false)) return ServiceResponse.Error("You are not authorized this action.");
            }
            catch (Exception ex)
            {
                Debug.Assert(false, ex.Message);
            }
            return new List<EventItem>();
        }

        public List<EventGroup> GetEventGroups( bool deleted = false)
        {
            try
            {
                using (var context = new TreeMonDbContext(this._connectionKey))
                {
                    return context.GetAll<EventGroup>()?.Where(sw => sw?.Deleted == deleted).OrderBy(ob => ob?.StartDate)?.ToList();
                }
                //////if (!this.DataAccessAuthorized(s, "GET", false)) return ServiceResponse.Error("You are not authorized this action.");
            }
            catch (Exception ex)
            {
                Debug.Assert(false, ex.Message);
            }
            return new List<EventGroup>();
        }


        public List<EventGroup> GetEventGroups(string eventUUID, bool deleted = false)
        {
            try
            {
                using (var context = new TreeMonDbContext(this._connectionKey))
                {
                    return context.GetAll<EventGroup>()?.Where(sw => sw.EventUUID == eventUUID   && sw?.Deleted == deleted).OrderBy(ob => ob?.StartDate)?.ToList();
                }
                //////if (!this.DataAccessAuthorized(s, "GET", false)) return ServiceResponse.Error("You are not authorized this action.");
            }
            catch (Exception ex)
            {
                Debug.Assert(false, ex.Message);
            }
            return new List<EventGroup>();
        }


        public ServiceResult InsertEventMember(INode n)
        {
            if (!this.DataAccessAuthorized(n, "post", false)) return ServiceResponse.Error("You are not authorized this action.");

            n.Initialize(this._requestingUser.UUID, this._requestingUser.AccountUUID, this._requestingUser.RoleWeight);

            var s = (EventMember)n;

            using (var context = new TreeMonDbContext(this._connectionKey))
            {
                EventMember dbU = context.GetAll<EventMember>()?.FirstOrDefault(wu => (wu.Name?.EqualsIgnoreCase(s.Name) ?? false) && wu.AccountUUID == s.AccountUUID);

                if (dbU != null)
                    return ServiceResponse.Error("EventMember already exists.");


                if (context.Insert<EventMember>(s))
                    return ServiceResponse.OK("", s);
            }
            return ServiceResponse.Error("An error occurred inserting EventMember " + s.Name);
        }

        public List<EventMember> GetEventMembers(string eventUUID, bool deleted = false)
        {
            try
            {
                using (var context = new TreeMonDbContext(this._connectionKey))
                {
                    return context.GetAll<EventMember>()?.Where(sw => sw.EventUUID == eventUUID && sw?.Deleted == deleted)?.ToList();
                }
                //////if (!this.DataAccessAuthorized(s, "GET", false)) return ServiceResponse.Error("You are not authorized this action.");
            }
            catch (Exception ex)
            {
                Debug.Assert(false, ex.Message);
            }
            return new List<EventMember>();
        }

        public List<dynamic> GetFavoriteEvents(string userUUID, string accountUUID)
        {
              
            try
            {
                using (var context = new TreeMonDbContext(this._connectionKey))
                {
                    if (context.GetAll<Reminder>()?.FirstOrDefault() == null)
                        return new List<dynamic>();

                    var events = context.GetAll<Reminder>()?.Where(w => w?.CreatedBy == userUUID && w?.AccountUUID == accountUUID 
                        && w.UUIDType.EqualsIgnoreCase("event"))
                        ?.Join(context.GetAll<Event>(),
                         rem => rem.EventUUID,
                         evt => evt.UUID,
                        ( rem, evt) => new {  rem, evt })
                        ?.Select(s => new Reminder()
                        {
                            Event =   s.evt,
                            UUID = s.rem.UUID,
                            UUIDType = s.rem.UUIDType,
                            CreatedBy = s.rem.CreatedBy,
                            AccountUUID = s.rem.AccountUUID
                            
                        } );

                    return events.Cast<dynamic>().ToList();
                }
                //////if (!this.DataAccessAuthorized(s, "GET", false)) return ServiceResponse.Error("You are not authorized this action.");
            }
            catch (Exception ex)
            {
                Debug.Assert(false, ex.Message);
            }
            return new List<dynamic>();
        }

        public ServiceResult InsertEventLocation(INode n)
        {
            if (!this.DataAccessAuthorized(n, "post", false)) return ServiceResponse.Error("You are not authorized this action.");

            n.Initialize(this._requestingUser.UUID, this._requestingUser.AccountUUID, this._requestingUser.RoleWeight);

            var s = (EventLocation)n;

            using (var context = new TreeMonDbContext(this._connectionKey))
            {
                // EventLocation dbU = context.GetAll<EventLocation>()?.FirstOrDefault(wu => (wu.Name?.EqualsIgnoreCase(s.Name) ?? false) && wu.AccountUUID == s.AccountUUID);

                // if (dbU != null)                    return ServiceResponse.Error("EventLocation already exists.");

                if (string.IsNullOrWhiteSpace(s.UUID))
                    s.UUID = Guid.NewGuid().ToString("N");

                if (context.Insert<EventLocation>(s))
                    return ServiceResponse.OK("", s);
            }
            return ServiceResponse.Error("An error occurred inserting EventLocation " + s.Name);
        }

        public List<EventLocation> GetEventLocations(string eventUUID, bool deleted = false)
        {
            try
            {
                using (var context = new TreeMonDbContext(this._connectionKey))
                {
                    return context.GetAll<EventLocation>()?.Where(sw => sw.EventUUID == eventUUID && sw?.Deleted == deleted)?.ToList();
                }
                //////if (!this.DataAccessAuthorized(s, "GET", false)) return ServiceResponse.Error("You are not authorized this action.");
            }
            catch (Exception ex)
            {
                Debug.Assert(false, ex.Message);
            }
            return new List<EventLocation>();
        }

        public List<EventLocation> GetAccountEventLocations(string accountUUID, bool deleted = false)
        {
            try
            {
                using (var context = new TreeMonDbContext(this._connectionKey))
                {
                    return context.GetAll<EventLocation>()?.Where(sw => sw.AccountUUID == accountUUID && sw?.Deleted == deleted)?.ToList();
                }
                //////if (!this.DataAccessAuthorized(s, "GET", false)) return ServiceResponse.Error("You are not authorized this action.");
            }
            catch (Exception ex)
            {
                Debug.Assert(false, ex.Message);
            }
            return new List<EventLocation>();
        }


        public ServiceResult Save(EventLocation geo)
        {
            if (geo == null)
                return ServiceResponse.Error("No location information sent.");

            EventLocation dbLocation = null;
            using (var context = new TreeMonDbContext(this._connectionKey))
            {
                if (!string.IsNullOrWhiteSpace(geo.UUID))
                    dbLocation = (EventLocation)this.GetEventLocation(geo.UUID);
                else
                    geo.UUID = Guid.NewGuid().ToString("N");

                if (dbLocation == null)
                    return InsertEventLocation(geo);
            }

            // geo = ObjectHelper.Merge<EventLocation>(dbLocation, geo);
            dbLocation.Name = geo.Name;
            dbLocation.Country = geo.Country;
            dbLocation.Postal = geo.Postal;
            dbLocation.State = geo.State;
            dbLocation.City = geo.City;
            dbLocation.Longitude = geo.Longitude;
            dbLocation.Latitude = geo.Latitude;
            dbLocation.isDefault = geo.isDefault;
            dbLocation.Description = geo.Description;
            dbLocation.Category = geo.Category;
            dbLocation.Address1 = geo.Address1?.Trim();
            dbLocation.Address2 = geo.Address2?.Trim();
            dbLocation.TimeZoneUUID = geo.TimeZoneUUID;
            dbLocation.Email = geo.Email;
            return UpdateEventLocation(dbLocation);

        }

        public ServiceResult UpdateEventLocation(INode n)
        {
            if (n == null)
                return ServiceResponse.Error("Invalid Event location data.");

            if (!this.DataAccessAuthorized(n, "PATCH", false)) return ServiceResponse.Error("You are not authorized this action.");

            var s = (EventLocation)n;

            using (var context = new TreeMonDbContext(this._connectionKey))
            {
                if (context.Update<EventLocation>(s) > 0)
                    return ServiceResponse.OK("", s);
            }
            return ServiceResponse.Error("System error, Event location was not updated.");
        }

        public EventLocation GetEventLocation(string eventLocationUUID)
        {
            //Func<EventLocation, Location, EventLocation> UpdateLocation
            //                                                = ((a, b) => {
            //                                                    a.Latitude = b.Latitude;
            //                                                    a.Longitude = b.Longitude;
            //                                                    return a; });
            try
            {
                using (var context = new TreeMonDbContext(this._connectionKey))
                {
                    var location = context.GetAll<EventLocation>()?.FirstOrDefault(sw => sw.UUID == eventLocationUUID);
                        //.Join(context.GetAll<Location>(),
                        // evtLocation => evtLocation.LocationUUID,
                        // tmpLoc => tmpLoc.UUID,
                        // (evtLocation, tmpLoc) => new { evtLocation, tmpLoc })
                        // .Select(s => UpdateLocation(s.evtLocation, s.tmpLoc))?.FirstOrDefault();

                    return location;
                }
                //////if (!this.DataAccessAuthorized(s, "GET", false)) return ServiceResponse.Error("You are not authorized this action.");
            }
            catch (Exception ex)
            {
                Debug.Assert(false, ex.Message);
            }
            return null;
        }


    }
}
