// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using TreeMon.Data.Logging.Models;
using TreeMon.Managers;
using TreeMon.Managers.Events;
using TreeMon.Managers.Geo;
using TreeMon.Managers.Store;
using TreeMon.Models;
using TreeMon.Models.App;
using TreeMon.Models.Datasets;
using TreeMon.Models.Geo;
using TreeMon.Utilites.Extensions;
using TreeMon.Utilites.Helpers;
using TreeMon.Web;
using TreeMon.Web.api;
using TreeMon.Web.api.Helpers;
using TreeMon.Web.Filters;
using TreeMon.WebAPI.Models;

using WebApiThrottle;

namespace TreeMon.WebAPI.api.v1
{
    
    public class GeoController : ApiBaseController
    {
        public GeoController()
        {

        }

        [ApiAuthorizationRequired(Operator = ">=", RoleWeight = 4)]
        [HttpPost]
        [Route("api/Locations/Add")]
        [Route("api/Locations/Insert")]
        public ServiceResult Insert(Location n)
        {
            if (n == null)
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

            if (string.IsNullOrWhiteSpace(n.CreatedBy))
            {
                n.CreatedBy = CurrentUser.UUID;
                n.AccountUUID = CurrentUser.AccountUUID;
                n.DateCreated = DateTime.UtcNow;
            }

            LocationManager locationManager = new LocationManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);
            return locationManager.Insert(n);
        }

        [ApiAuthorizationRequired(Operator = ">=", RoleWeight = 4)]
        [HttpPost]
        [Route("api/Locations/Save")]
        public ServiceResult Save() 
        {
            try
            {
                string body = ActionContext.Request.Content.ReadAsStringAsync().Result;
                //  if (content == null)
                //      return ServiceResponse.Error("No permissions were sent.");

                // string body = content.Result;

                if (string.IsNullOrEmpty(body))
                    return ServiceResponse.Error("No permissions were sent.");

                Location geo = JsonConvert.DeserializeObject<Location>(body);

                if (geo == null)
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

                if (string.IsNullOrWhiteSpace(geo.CreatedBy))
                {
                    geo.CreatedBy = CurrentUser.UUID;
                    geo.AccountUUID = CurrentUser.AccountUUID;
                    geo.DateCreated = DateTime.UtcNow;
                }

                geo.Country = geo.Country?.Trim();
                geo.Postal = geo.Postal?.Trim();
                geo.State = geo.State?.Trim();
                geo.City = geo.City?.Trim();
                geo.Name = geo.Name?.Trim();

                LocationManager locationManager = new LocationManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);

                return locationManager.Save(geo);
            }
            catch (Exception ex)
            {
                return ServiceResponse.Error("Failed to save location.");
            }
           


        }

        [ApiAuthorizationRequired(Operator = ">=", RoleWeight = 4)]
        [HttpPost]
        [Route("api/Locations/Account/Save")]
        public ServiceResult SaveAccountLocation()
        {
            try
            {
                string body = ActionContext.Request.Content.ReadAsStringAsync().Result;
                //  if (content == null)
                //      return ServiceResponse.Error("No permissions were sent.");

                // string body = content.Result;

                if (string.IsNullOrEmpty(body))
                    return ServiceResponse.Error("No permissions were sent.");

                Location geo = JsonConvert.DeserializeObject<Location>(body);

                if (geo == null)
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

                if (string.IsNullOrWhiteSpace(geo.CreatedBy))
                {
                    geo.CreatedBy = CurrentUser.UUID;
                    geo.AccountUUID = CurrentUser.AccountUUID;
                    geo.DateCreated = DateTime.UtcNow;
                }
                
                geo.Country = geo.Country?.Trim();
                geo.Postal      = geo.Postal?.Trim();
                geo.State   = geo.State?.Trim();
                geo.City     = geo.City?.Trim();
                geo.Address1 = geo.Address1?.Trim();
                geo.Address2  = geo.Address2?.Trim();
                geo.Name    = geo.Name?.Trim();

                LocationManager locationManager = new LocationManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);

                if (geo.isDefault == true)
                {
                    //reset all others for this account to false.
                    var locations = locationManager.GetAccountLocations(geo.AccountUUID)?.Where(w => w.isDefault == true);
                    foreach (var location in locations)
                    {
                        location.isDefault = false;
                        locationManager.Update(location);
                    }
                }

                if (string.IsNullOrWhiteSpace(geo.UUID)) 
                    return locationManager.Insert(geo);

                Location dbGeo = locationManager.Get(geo.UUID) as Location;
                if (dbGeo == null || dbGeo.AccountUUID != geo.AccountUUID)
                {
                    
                    return locationManager.Insert(geo);
                }


                dbGeo.Name = geo.Name;
                dbGeo.Country  = geo.Country;
                dbGeo.Postal  = geo.Postal;
                dbGeo.State = geo.State;
                dbGeo.City = geo.City;
                dbGeo.Longitude = geo.Longitude;
                dbGeo.Latitude = geo.Latitude;
                dbGeo.isDefault = geo.isDefault;
                dbGeo.Description = geo.Description;
                dbGeo.Category = geo.Category;
                dbGeo.Address1 = geo.Address1?.Trim();
                dbGeo.Address2 = geo.Address2?.Trim();
                dbGeo.TimeZoneUUID = geo.TimeZoneUUID;
                return locationManager.Update(dbGeo);
            }
            catch (Exception ex)
            {
                return ServiceResponse.Error("Failed to save location.");
            }



        }


        [ApiAuthorizationRequired(Operator = ">=", RoleWeight = 1)]
        [HttpPost]
        [HttpGet]
        [Route("api/Locations/{name}")]
        public ServiceResult Get(string name )
        {
            if (string.IsNullOrWhiteSpace(name))
                return ServiceResponse.Error("You must provide a name for the location.");

            LocationManager locationManager = new LocationManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);

            List<Location> s = locationManager.Search(name);

            if (s == null || s.Count == 0)
                return ServiceResponse.Error("Location could not be located for the name " + name);

            return ServiceResponse.OK("", s);
        }

        [ApiAuthorizationRequired(Operator = ">=", RoleWeight = 1)]
        [HttpPost]
        [HttpGet]
        [Route("api/LocationsBy/{uuid}")]
        public ServiceResult GetBy(string uuid)
        {
            if (string.IsNullOrWhiteSpace(uuid))
                return ServiceResponse.Error("You must provide an id for the location.");

            LocationManager locationManager = new LocationManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);
            Location p = (Location)locationManager.Get(uuid);

            if (p == null)
                return ServiceResponse.Error("Location could not be located for the uuid " + uuid);

            return ServiceResponse.OK("", p);
        }

        [ApiAuthorizationRequired(Operator = ">=", RoleWeight = 4)]
        [HttpPost]
        [HttpGet]
        [Route("api/Locations")]
        public ServiceResult GetLocations()
        {
           
            LocationManager locationManager = new LocationManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);
            List<dynamic> Geo = (List<dynamic>)locationManager.GetAll().Cast<dynamic>().ToList();

            int count;
                             DataFilter tmpFilter = this.GetFilter(Request);
                Geo = Geo.Filter( tmpFilter, out count);

            return ServiceResponse.OK("", Geo, count);
        }

        [EnableThrottling(PerSecond = 1, PerMinute = 20, PerHour = 200, PerDay = 1500, PerWeek = 3000)]
        [AllowAnonymous]
        [HttpPost]
        [HttpGet]
        [Route("api/Locations/InArea/lat/{latitude}/lon/{longitude}/range/{range}")]
        public ServiceResult GetAreaData(double latitude, double longitude, double range)
        {
            if (range > 25)
                range = 25;

            LocationManager locationManager = new LocationManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);
            GeoCoordinate geo = locationManager.GetLocationsIn(latitude, longitude, range);
          

             //int count;
            // DataFilter tmpFilter = this.GetFilter(Request);
            // geo = geo.Filter(tmpFilter, out count);

            return ServiceResponse.OK("", geo);
        }
        // 

        [EnableThrottling(PerSecond = 1, PerMinute = 20, PerHour = 200, PerDay = 1500, PerWeek = 3000)]
        [AllowAnonymous]
        [HttpGet]
        [Route("api/Locations/{locationUUID}/Types/{locationType}")]
        public ServiceResult GetLocation(string locationUUID, string locationType)
        {
            if (string.IsNullOrWhiteSpace(locationUUID))
                return ServiceResponse.Error("Invalid location UUID");

            if (!string.IsNullOrWhiteSpace(locationType))
                locationType = locationType.ToUpper();

            switch (locationType)
            {
                case "EVENTLOCATION":
                    EventManager eventManager = new EventManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);
                    var eventGeo = eventManager.GetEventLocation(locationUUID);
                    return ServiceResponse.OK("", eventGeo);
                default:
                    LocationManager locationManager = new LocationManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);
                    var geo = locationManager.Get(locationUUID);
                    return ServiceResponse.OK("", geo);
            }
        }

        [ApiAuthorizationRequired(Operator = ">=", RoleWeight = 4)]
        [HttpPost]
        [HttpGet]
        [Route("api/Locations/Account/{accountUUID}")]
        public ServiceResult GetAccountLocations(string accountUUID)
        {

            LocationManager locationManager = new LocationManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);
            List<dynamic> Geo = (List<dynamic>)locationManager.GetLocations(accountUUID).Cast<dynamic>().ToList();

            int count;
            DataFilter tmpFilter = this.GetFilter(Request);
            Geo = Geo.Filter(tmpFilter, out count);

            return ServiceResponse.OK("", Geo, count);
        }

        [ApiAuthorizationRequired(Operator = ">=", RoleWeight = 4)]
        [HttpPost]
        [HttpGet]
        [Route("api/Locations/Custom")]
        public ServiceResult GetCustomLocations( )
        {
           
            LocationManager locationManager = new LocationManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);
            List<dynamic> Geo = (List<dynamic>)locationManager.GetAll()?.Where( w => w.LocationType?.ToUpper() != "COUNTRY" && w.LocationType?.ToUpper() != "STATE" && w.LocationType?.ToUpper() != "CITY" && w.LocationType?.ToUpper() != "REGION").Cast<dynamic>().ToList();

          int count;

            DataFilter tmpFilter = this.GetFilter(Request);
            Geo = Geo.Filter( tmpFilter, out count);
          return ServiceResponse.OK("", Geo, count);
        }



        [ApiAuthorizationRequired(Operator = ">=", RoleWeight = 4)]
        [HttpPost]
        [HttpGet]
        [Route("api/ChildLocations/{parentUUID}")]
        public ServiceResult GetChildLocations( string parentUUID)
        {
           
            LocationManager locationManager = new LocationManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);
            List<dynamic> Geo = (List<dynamic>)locationManager.GetAll()?.Where(w => w.UUParentID == parentUUID 
                                                                                  && (  w.AccountUUID == CurrentUser.AccountUUID  ||
                                                                                        w.AccountUUID.EqualsIgnoreCase( SystemFlag.Default.Account )
                                                                                  )).Cast<dynamic>().ToList();

            int count;
             DataFilter tmpFilter = this.GetFilter(Request);
            Geo = Geo.Filter( tmpFilter, out count);
            return ServiceResponse.OK("", Geo, count);
        }


        [ApiAuthorizationRequired(Operator = ">=", RoleWeight = 1)]
        [HttpPost]
        [HttpGet]
        [Route("api/Locations/LocationTypes")]
        public ServiceResult GetLocationTypes()
        {
            if (CurrentUser == null)
                return ServiceResponse.Error("You must be logged in to access this function.");

            LocationManager locationManager = new LocationManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);
            List<string> geoTypes = locationManager.GetLocationTypes(CurrentUser.AccountUUID);
            return ServiceResponse.OK("", geoTypes, geoTypes.Count );
        }

        [ApiAuthorizationRequired(Operator = ">=", RoleWeight = 1)]
        [HttpPost]
        [HttpGet]
        [Route("api/Locations/LocationType/{geoType}")]
        public ServiceResult GetLocatonsByLocationType(string geoType)
        {
            if(string.IsNullOrWhiteSpace(geoType))
                return ServiceResponse.Error("You must pass in a geo type.");

            if (CurrentUser == null)
                return ServiceResponse.Error("You must be logged in to access this function.");

            LocationManager locationManager = new LocationManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);
            List<dynamic> locations = (List<dynamic>)locationManager.GetAll()
                                                                    .Where(pw => (pw.AccountUUID == CurrentUser.AccountUUID || pw.AccountUUID == SystemFlag.Default.Account)  
                                                                                    && (pw.LocationType?.EqualsIgnoreCase(geoType) ?? false)
                                                                                    && pw.Deleted == false 
                                                                                    && string.IsNullOrWhiteSpace(pw.LocationType) == false)
                                                                                    .Cast<dynamic>().ToList();

            int count;
            DataFilter tmpFilter = this.GetFilter(Request);
            locations = locations.Filter( tmpFilter, out count);

            if (locations == null || locations.Count == 0)
                return ServiceResponse.Error("No locations available.");

            return ServiceResponse.OK("", locations, count);

        }

        [ApiAuthorizationRequired(Operator = ">=", RoleWeight = 4)]
        [HttpPost]
        [HttpDelete]
        [Route("api/Locations/Delete")]
        public ServiceResult Delete(Location n)
        {
            if (CurrentUser == null)
                return ServiceResponse.Error("You must be logged in to access this function.");


            if (n == null || string.IsNullOrWhiteSpace(n.UUID))
                return ServiceResponse.Error("Invalid account was sent.");

            LocationManager locationManager = new LocationManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);
            return locationManager.Delete(n);

        }

        [ApiAuthorizationRequired(Operator = ">=", RoleWeight = 4)]
        [HttpPost]
        [HttpDelete]
        [Route("api/Locations/Delete/{locationUUID}")]
        public ServiceResult Delete(string locationUUID)
        {
            if (CurrentUser == null)
                return ServiceResponse.Error("You must be logged in to access this function.");

            LocationManager locationManager = new LocationManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);
            Location p = (Location)locationManager.Get(locationUUID);

            if (p == null || string.IsNullOrWhiteSpace(p.UUID))
                return ServiceResponse.Error("Invalid account was sent.");

            return locationManager.Delete(p);

        }

      
        [ApiAuthorizationRequired(Operator = ">=", RoleWeight = 4)]
        [HttpPost]
        [HttpPatch]
        [Route("api/Locations/Update")]
        public ServiceResult Update(Location pv)
        {
            if (CurrentUser == null)
                return ServiceResponse.Error("You must be logged in to access this function.");

            if (pv == null)
                return ServiceResponse.Error("Invalid location sent to server.");

            LocationManager locationManager = new LocationManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);
            var dbP = locationManager.GetAll()?.FirstOrDefault(pw => pw.UUID == pv.UUID);

            if (dbP == null)
                return ServiceResponse.Error("Location was not found.");
           

            dbP.Name = pv.Name;
            dbP.Address1 = pv.Address1;
            dbP.Address2 = pv.Address2;
            dbP.City = pv.City;
            dbP.State = pv.State;
            dbP.Postal = pv.Postal;
            dbP.LocationType = pv.LocationType;
            dbP.Latitude = pv.Latitude;
            dbP.Longitude =pv.Longitude;
            dbP.Virtual = pv.Virtual;
            dbP.isDefault = pv.isDefault;//todo update other locations with same LocationType, account, default = false. only one default per type.
            return locationManager.Update(dbP);
        }

       // [ApiAuthorizationRequired(Operator = ">=", RoleWeight = 1)]
        [HttpGet]
        [Route("api/Locations/Current")]
        public ServiceResult GetCurrentLocation()
        {
            NetworkHelper network = new NetworkHelper();
            string ip = "70.175.111.49";//network.GetClientIpAddress(this.Request);// //"2404:6800:4001:805::1006";
            UInt64 ipNum;
            NetworkHelper.TryConvertIP(ip, out ipNum);
            if (ipNum < 0)
                return ServiceResponse.Error("Unable to get location.");


            LocationManager locationManager = new LocationManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);

            float version = NetworkHelper.GetIpVersion(ip);
            Location s = locationManager.Search(ipNum, version);

            if (s == null )
               return ServiceResponse.Error("Unable to get location." );

            return ServiceResponse.OK("", s);
        }

    }
}


