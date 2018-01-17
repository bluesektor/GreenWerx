// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using TreeMon.Data.Logging.Models;
using TreeMon.Managers;
using TreeMon.Managers.Store;
using TreeMon.Models;
using TreeMon.Models.App;
using TreeMon.Models.Datasets;
using TreeMon.Models.Geo;
using TreeMon.Utilites.Extensions;
using TreeMon.Web;
using TreeMon.Web.api;
using TreeMon.Web.Filters;
using TreeMon.WebAPI.Models;

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
            return locationManager.Insert(n, true);
        }

        [ApiAuthorizationRequired(Operator = ">=", RoleWeight = 1)]
        [HttpPost]
        [HttpGet]
        [Route("api/Locations/{name}")]
        public ServiceResult Get(string name )
        {
            if (string.IsNullOrWhiteSpace(name))
                return ServiceResponse.Error("You must provide a name for the strain.");

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
                return ServiceResponse.Error("You must provide a name for the strain.");

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
        public ServiceResult GetLocations(string filter = "")
        {
           
            LocationManager locationManager = new LocationManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);
            List<dynamic> Geo = (List<dynamic>)locationManager.GetAll().Cast<dynamic>().ToList();

            int count;
                            DataFilter tmpFilter = this.GetFilter(filter);
                Geo = FilterEx.FilterInput(Geo, tmpFilter, out count);

            return ServiceResponse.OK("", Geo, count);
        }

        [ApiAuthorizationRequired(Operator = ">=", RoleWeight = 4)]
        [HttpPost]
        [HttpGet]
        [Route("api/Locations/Custom")]
        public ServiceResult GetCustomLocations(string filter = "")
        {
           
            LocationManager locationManager = new LocationManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);
            List<dynamic> Geo = (List<dynamic>)locationManager.GetAll().Where( w => w.LocationType?.ToUpper() != "COUNTRY" && w.LocationType?.ToUpper() != "STATE" && w.LocationType?.ToUpper() != "CITY" && w.LocationType?.ToUpper() != "REGION").Cast<dynamic>().ToList();

          int count;

          DataFilter tmpFilter = this.GetFilter(filter);
          Geo = FilterEx.FilterInput(Geo, tmpFilter, out count);
          return ServiceResponse.OK("", Geo, count);
        }



        [ApiAuthorizationRequired(Operator = ">=", RoleWeight = 4)]
        [HttpPost]
        [HttpGet]
        [Route("api/ChildLocations/{parentUUID}")]
        public ServiceResult GetChildLocations( string parentUUID, string filter = "")
        {
           
            LocationManager locationManager = new LocationManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);
            List<dynamic> Geo = (List<dynamic>)locationManager.GetAll().Where(w => w.UUParentID == parentUUID 
                                                                                  && (  w.AccountUUID == CurrentUser.AccountUUID  ||
                                                                                        w.AccountUUID.EqualsIgnoreCase( SystemFlag.Default.Account )
                                                                                  )).Cast<dynamic>().ToList();

            int count;
            DataFilter tmpFilter = this.GetFilter(filter);
            Geo = FilterEx.FilterInput(Geo, tmpFilter, out count);
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
        [Route("api/Locations/LocationType/{geoType}/")]
        public ServiceResult GetLocatonsByLocationType(string geoType, string filter = "")
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
            DataFilter tmpFilter = this.GetFilter(filter);
            locations = FilterEx.FilterInput(locations, tmpFilter, out count);

            if (locations == null || locations.Count() == 0)
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
            var dbP = locationManager.GetAll().FirstOrDefault(pw => pw.UUID == pv.UUID);

            if (dbP == null)
                return ServiceResponse.Error("Location was not found.");
           

            dbP.Name = pv.Name;
            dbP.Address1 = pv.Address1;
            dbP.Address2 = pv.Address2;
            dbP.AccountReference = pv.AccountReference;
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

    }
}