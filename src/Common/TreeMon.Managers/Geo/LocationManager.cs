// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
using AutoMapper;
using Dapper;
using MoreLinq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using System.Xml.Linq;
using TreeMon.Data;
using TreeMon.Data.Logging;
using TreeMon.Data.Logging.Models;
using TreeMon.Models;
using TreeMon.Models.App;
using TreeMon.Models.Geo;
using TreeMon.Models.Membership;
using TreeMon.Utilites.Extensions;
using TreeMon.Utilites.Helpers;

namespace TreeMon.Managers.Geo
{
    public class LocationManager : BaseManager,ICrud
    {
        protected readonly SystemLogger _logger;
    
        public LocationManager(string connectionKey, string sessionKey) : base(connectionKey, sessionKey)
        {
            Debug.Assert(!string.IsNullOrWhiteSpace(connectionKey), "LocationManager CONTEXT IS NULL!");
            this._connectionKey = connectionKey;
            _logger = new SystemLogger(connectionKey);
        }


        public ServiceResult Delete(INode n, bool purge = false)
        {
            if (n == null)
                return ServiceResponse.Error("No record sent.");

            if (!this.DataAccessAuthorized(n, "DELETE", false)) return ServiceResponse.Error("You are not authorized this action.");

            if (n.GetType().Name != "Location")
                Debug.Assert(false, "TODO FIX THIS IT SHOULDN'T BE ANOTHER TYPE");
            var p = (Location)n;

            try
            {
                using (var context = new TreeMonDbContext(this._connectionKey))
                {
                    if (purge)
                    {
                        DynamicParameters parameters = new DynamicParameters();
                        parameters.Add("@LocationUUID", p.UUID);
                        if (context.Delete<Location>("WHERE UUID=@LocationUUID", parameters) > 0)
                        {
                            DynamicParameters credParams = new DynamicParameters();
                            credParams.Add("@UUID", p.UUID);
                            credParams.Add("@TYPE", "Location");
                            context.Delete<Credential>("WHERE RecipientUUID=@UUID AND RecipientType=@TYPE", credParams);

                            return ServiceResponse.OK();
                        }
                    }
                    else
                    {
                        p.Deleted = true;

                        if (context.Update<Location>(p) > 0)
                            return ServiceResponse.OK();
                    }
                }

                return ServiceResponse.Error("No records deleted.");
                ////SQLITE
                ////this was the only way I could get it to delete a RolePermission without some stupid EF error.
                ////object[] paramters = new object[] { rp.PermissionUUID , rp.RoleUUID ,rp.AccountUUID };
                ////context.Delete<RolePermission>("WHERE PermissionUUID=? AND RoleUUID=? AND AccountUUID=?", paramters);
                ////  context.Delete<RolePermission>(rp);
            }
            catch (Exception ex)
            {
                _logger.InsertError(ex.Message, "LocationManager", "DeleteLocation:" + p.UUID);
                Debug.Assert(false, ex.Message);
                return ServiceResponse.Error("Exception occured while deleting this record.");
            }
        }

        public List<Location> GetAccountLocations(string accountUUID)
        {
            using (var context = new TreeMonDbContext(this._connectionKey))
            {
                ///if (!this.DataAccessAuthorized(p, "GET", false))return ServiceResponse.Error("You are not authorized this action.");

                if (string.IsNullOrWhiteSpace(accountUUID))
                    return context.GetAll<Location>().ToList();

                return context.GetAll<Location>()?.Where(pw => pw.AccountUUID == accountUUID && pw.Deleted == false).ToList();
            }
        }

        public List<Location> GetAll()
        {
            ///if (!this.DataAccessAuthorized(dbP, "GET", false)) return ServiceResponse.Error("You are not authorized this action.");
            using (var context = new TreeMonDbContext(this._connectionKey))
            {
                    return context.GetAll<Location>()
                        .OrderBy(o => o.Name).ToList();
            }
        }

        public List<string> GetLocationTypes(string accountUUID)
        {
            List<string> geoTypes;
            ///if (!this.DataAccessAuthorized(dbP, "GET", false)) return ServiceResponse.Error("You are not authorized this action.");
            using (var context = new TreeMonDbContext(this._connectionKey))
            {
                geoTypes = context.GetAll<Location>()?.Where(pw =>( pw.AccountUUID == accountUUID || pw.AccountUUID == SystemFlag.Default.Account) && 
                        pw.Deleted == false &&  
                        string.IsNullOrWhiteSpace(pw.LocationType) == false)
                        .DistinctBy(pd => pd.LocationType)
                        .Select(s => s.LocationType).ToList();
            }
            return geoTypes;
        }

        public INode Get(string uuid)
        {
            if (string.IsNullOrWhiteSpace(uuid))
                return null;
            using (var context = new TreeMonDbContext(this._connectionKey))
            {
                ///if (!this.DataAccessAuthorized(p, "GET", false))return ServiceResponse.Error("You are not authorized this action.");
                return context.GetAll<Location>()?.FirstOrDefault(sw => sw.UUID == uuid);
            }
        }

        public Location Search(float ipNumber, float ipVersion)
        {
            if ( ipNumber <= 0)
                return null;


            using (var context = new TreeMonDbContext(this._connectionKey))
            {
                ///if (!this.DataAccessAuthorized(p, "GET", false))return ServiceResponse.Error("You are not authorized this action.");

                return context.GetAll<Location>()?.FirstOrDefault(w => w.IpNumStart <= ipNumber && w.IpNumEnd >= ipNumber && w.IpVersion == ipVersion);
              
            }
        }

        public List<Location> Search(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return new List<Location>();

            using (var context = new TreeMonDbContext(this._connectionKey))
            {
                ///if (!this.DataAccessAuthorized(p, "GET", false))return ServiceResponse.Error("You are not authorized this action.");
                if (context.GetAll<Location>()?.Any() ?? false)
                    return context.GetAll<Location>()?.Where(sw => sw.Name.EqualsIgnoreCase(name)).ToList();
                else
                    return new List<Location>();
            }
        }

        public List<Location> Search(string name, string locationType)
        {
            if (string.IsNullOrWhiteSpace(name))
                return new List<Location>();

            using (var context = new TreeMonDbContext(this._connectionKey))
            {
                ///if (!this.DataAccessAuthorized(p, "GET", false))return ServiceResponse.Error("You are not authorized this action.");
                if (context.GetAll<Location>()?.Any() ?? false)
                    return context.GetAll<Location>()?.Where(sw => sw.Name.EqualsIgnoreCase(name) && sw.LocationType.EqualsIgnoreCase(locationType)).ToList();
                else
                    return new List<Location>();
            }
        }

        public List<Location> GetLocations(string accountUUID, bool deleted = false, bool includeSystemAccount = false)
        {
            using (var context = new TreeMonDbContext(this._connectionKey))
            {
                /////if (!this.DataAccessAuthorized(p, "GET", false))
                ////    return ServiceResponse.Error("You are not authorized this action.");
                if (includeSystemAccount)
                {
                    return context.GetAll<Location>()?.Where(sw => (sw.AccountUUID == accountUUID) && sw.Deleted == deleted).GroupBy(x => x.Name).Select(group => group.First()).OrderBy(ob => ob.Name).ToList();
                }

                return context.GetAll<Location>()?.Where(sw => (sw.AccountUUID == accountUUID) && sw.Deleted == deleted).OrderBy(ob => ob.Name).ToList();
            }
        }

        public GeoCoordinate GetLocationsIn(double latitude, double longitude, double range)
        {
            GeoCoordinate result = new GeoCoordinate();// assign the found locations to result.GeoDistances
            result.SearchDistance = range;

            if (range > result.MaxDistance)
                return result;

            result.Longitude = longitude;
            result.Latitude = latitude;
          

            using (var context = new TreeMonDbContext(this._connectionKey))
            {
                result.Distances = context.GetAll<Location>()?.Where(x => (x.Latitude != null && x.Longitude != null) &&
                                         MathHelper.Distance(result.Latitude, result.Longitude, x.Latitude ?? 0, x.Longitude ?? 0) <= range).
                                         Select(s => new GeoCoordinate()
                                         {
                                             UUID = s.UUID,
                                             Name = s.Name,
                                             LocationType = s.LocationType,
                                             Latitude = s.Latitude ?? 0,
                                             Longitude = s.Longitude ?? 0,
                                             SearchDistance = result.SearchDistance,
                                             Distance = MathHelper.Distance(result.Latitude, result.Longitude, s.Latitude ?? 0, s.Longitude ?? 0)//todo figure out how to do this only once
                                         }
                                          //Tags.Push(""),
                                         ).OrderBy(d => d.Distance).ToList();
            }

            return result;
             
        }

        public GeoCoordinate GetLocationsIn(string name, double range)
        {
            GeoCoordinate result = new GeoCoordinate();// assign the found locations to result.GeoDistances
            result.SearchDistance = range;

            if (range > result.MaxDistance)
                return result;

            Location loc = this.Search(name, "coordinate")?.FirstOrDefault();
            if (loc == null)
                return result;

            result.Longitude = loc.Longitude ?? 0;
            result.Latitude = loc.Latitude ?? 0;
            result.Name = loc.Name;
            result.LocationType = loc.LocationType;
            result.UUID = loc.UUID;

            using (var context = new TreeMonDbContext(this._connectionKey))
            {
               result.Distances = context.GetAll<Location>()?.Where(x => (x.Latitude != null && x.Longitude != null ) &&
                                        MathHelper.Distance(result.Latitude, result.Longitude, x.Latitude ?? 0, x.Longitude ?? 0) <= range).
                                        Select(s => new GeoCoordinate() {
                                            UUID = s.UUID,
                                            Name = s.Name,
                                            LocationType = s.LocationType,
                                            Latitude = s.Latitude ?? 0,
                                            Longitude = s.Longitude ?? 0,
                                            SearchDistance = result.SearchDistance,
                                            Distance = MathHelper.Distance(result.Latitude, result.Longitude, s.Latitude ?? 0, s.Longitude ?? 0)//todo figure out how to do this only once
                                        }).OrderBy(d =>d.Distance).ToList();
            }

            return result;
            //Get the data for the zipCode arg.
            //ZipCode zip = new ZipCode()
            //{
            //    Latitude = loc.Latitude ?? 0,
            //    Longitude = loc.Longitude ?? 0,
            //    UUID = loc.UUID,
            //    Code = loc.Name
            //};

            // MathHelper.Distance(
            /*
               if (distance > MaxDistance)
                 {
                     throw new ArgumentOutOfRangeException("distance",
                         string.Format("Must be less than {0}.", MaxDistance));
                 }

                 IEnumerable<ZipCodeDistance> codes1 = null;
                 if (startingZipCode.DistanceCache == null)
                 {
                     // grab all less than the MaxDistance in first pass
                     codes1 = from c in this.Values
                              let d = c - startingZipCode
                              where (d <= MaxDistance)
                              orderby d
                              select new ZipCodeDistance() { ZipCode = c, Distance = d };
                     // this might just be temporary storage depending on caching settings
                     startingZipCode.DistanceCache = codes1;
                 }
                 else
                 {
                     // grab the cached copy
                     codes1 = startingZipCode.DistanceCache;
                 }
                 List<ZipCodeDistance> filtered = new List<ZipCodeDistance>();

                 foreach (ZipCodeDistance zcd in codes1)
                 {
                     // since the list is pre-sorted, we can now drop out 
                     // quickly and efficiently as soon as something doesn't
                     // match
                     if (zcd.Distance > distance)
                     {
                         break;
                     }
                     filtered.Add(zcd);
                 }

                 // if no caching, don't leave the cached result in place
                 if (!IsCaching) { startingZipCode.DistanceCache = null; }
                 return filtered;
             */

            // return this.ZipCodeManager.FindLessThanDistance(zip, distance).ToList();


            // Console.WriteLine("Find all zips < 25 miles from 13126:");
            // var distanced = codes.FindLessThanDistance(codes[13126], 25);
        }


        public ServiceResult Update(INode n)
        {
            if (!this.DataAccessAuthorized(n, "PATCH", false))
                return ServiceResponse.Error("You are not authorized this action.");

            var p = (Location)n;

            using (var context = new TreeMonDbContext(this._connectionKey))
            {
                if (context.Update<Location>(p) == 0)
                    return ServiceResponse.Error(p.Name + " failed to update. ");
            }
            return ServiceResponse.OK("", p);
        }

        public List<Location> GetCountries(string accountUUID)
        {
            using (var context = new TreeMonDbContext(this._connectionKey))
            {
                IEnumerable<Location> locations = context.GetAll<Location>()
                                                               .Where(pw =>    (pw?.AccountUUID == accountUUID || pw?.AccountUUID == SystemFlag.Default.Account)
                                                                               && (pw?.LocationType?.EqualsIgnoreCase("Country") ?? false)
                                                                               && pw?.Deleted == false
                                                                               && string.IsNullOrWhiteSpace(pw.LocationType) == false
                                                                               ).OrderBy(o=> o.Name); 
                return locations.ToList();
            }

        }
        public List<Location> GetStates(string accountUUID, string countryUUID)
        {
            using (var context = new TreeMonDbContext(this._connectionKey))
            {
                List<Location> states = context.GetAll<Location>()?.Where(w => w.UUParentID == countryUUID
                                                                                  && (w.AccountUUID == accountUUID ||
                                                                                        w.AccountUUID.EqualsIgnoreCase(SystemFlag.Default.Account)
                                                                                         && (w?.LocationType?.EqualsIgnoreCase("State") ?? false)
                                                                                         && w?.Deleted == false
                                                                                  )).OrderBy(o => o.Name).ToList();
                return states;
            }

        }

        public List<Location> GetCities(string accountUUID, string stateUUID)
        {
            using (var context = new TreeMonDbContext(this._connectionKey)) {
            List<Location> cities = context.GetAll<Location>()?.Where(w => w.UUParentID == stateUUID
                                                                                  && (w.AccountUUID == accountUUID ||
                                                                                        w.AccountUUID.EqualsIgnoreCase(SystemFlag.Default.Account)
                                                                                             && (w?.LocationType?.EqualsIgnoreCase("city") ?? false)
                                                                                         && w?.Deleted == false
                                                                                  )).OrderBy(o => o.Name).ToList();
                return cities;

            }
        }

        /// <summary>
        /// This was created for use in the bulk process..
        /// 
        /// </summary>
        /// <param name="p"></param>
        /// <param name="checkName">This will check the Locations by name to see if they exist already. If it does an error message will be returned.</param>
        /// <returns></returns>
        public ServiceResult Insert(INode n )
        {
            if (!this.DataAccessAuthorized(n, "POST", false)) return ServiceResponse.Error("You are not authorized this action.");

            n.Initialize(this._requestingUser.UUID, this._requestingUser.AccountUUID, this._requestingUser.RoleWeight);

            var p = (Location)n;
          
                if (string.IsNullOrWhiteSpace(p.CreatedBy))
                    return ServiceResponse.Error("You must assign who the Location was created by.");

                if (string.IsNullOrWhiteSpace(p.AccountUUID))
                    return ServiceResponse.Error("The account id is empty.");
          

            if (!this.DataAccessAuthorized(p, "POST", false))
                return ServiceResponse.Error("You are not authorized this action.");
 
            using (var context = new TreeMonDbContext(this._connectionKey))
            {
                if (context.Insert<Location>(p))
                    return ServiceResponse.OK("", p);
            }
            return ServiceResponse.Error("An error occurred inserting Location " + p.Name);
        }


       

        public string GetFullAddress(Location geo)
        {
            string res = "";
            if (!string.IsNullOrWhiteSpace(geo.Address1))
                res = geo.Address1.Trim();

            if (!string.IsNullOrWhiteSpace(geo.Address2))
                res += " " + geo.Address2.Trim();

            if (!string.IsNullOrWhiteSpace(geo.City))
                res += " " + geo.City.Trim();

            if (!string.IsNullOrWhiteSpace(geo.State))
                res += " " + geo.State.Trim();

            if (!string.IsNullOrWhiteSpace(geo.Postal))
                res += " " + geo.Postal.Trim();

            if (!string.IsNullOrWhiteSpace(geo.Country))
                res += " " + geo.Country.Trim();

            return res;
        }

        public Location GetCoordinate(string postalCode)
        {
            using (var context = new TreeMonDbContext(this._connectionKey))
            {
              return  context.GetAll<Location>()?.FirstOrDefault(x => x.Name.EqualsIgnoreCase( postalCode ) && x.LocationType == "coordinate");
            }
        }


        public ServiceResult Save(Location geo)
        {
            if (geo == null)
                return ServiceResponse.Error("No location information sent.");

            Location dbLocation = null;
            using (var context = new TreeMonDbContext(this._connectionKey))
            {
                if(!string.IsNullOrWhiteSpace(geo.UUID))
                    dbLocation = (Location)this.Get(geo.UUID);

                if (dbLocation == null || string.IsNullOrWhiteSpace(geo.UUID))
                {
                    dbLocation = context.GetAll<Location>()?.FirstOrDefault(w =>
                    w.Latitude == geo.Latitude &&
                    w.Longitude == geo.Longitude);
                }
                

                if (dbLocation == null)
                {
                    return Insert(geo);
                }
            }
            geo = ObjectHelper.Merge<Location>( dbLocation, geo);

            return Update(geo);

        }




    }
}
//ZIP Code Tabulation Areas
//Column Label      Description
//Column 1	        GEOID Five digit ZIP Code Tabulation Area Census Code
//Column 2	        ALAND Land Area(square meters) - Created for statistical purposes only
//Column 3	        AWATER Water Area(square meters) - Created for statistical purposes only
//Column 4	        ALAND_SQMI Land Area(square miles) - Created for statistical purposes only
//Column 5	        AWATER_SQMI Water Area(square miles) - Created for statistical purposes only
//Column 6	        INTPTLAT Latitude(decimal degrees) First character is blank or "-" denoting North or South latitude respectively
//Column 7	        INTPTLONG Longitude(decimal degrees) First character is blank or "-" denoting East or West longitude respectively

//urban areas
//Column Label   Description
//Column 1	GEOID Five digit Urban Area Census Code
//Column 2	NAME Name
//Column 3	UATYPE Urban Area Type
//C = Urban Cluster
//U = Urbanized Area
//Column 4	ALAND Land Area(square meters) - Created for statistical purposes only
//Column 5	AWATER Water Area(square meters) - Created for statistical purposes only
//Column 6	ALAND_SQMI Land Area(square miles) - Created for statistical purposes only
//Column 7	AWATER_SQMI Water Area(square miles) - Created for statistical purposes only
//Column 8	INTPTLAT Latitude(decimal degrees) First character is blank or "-" denoting North or South latitude respectively
//Column 9	INTPTLONG Longitude(decimal degrees) First character is blank or "-" denoting East or West longitude respectively

//State Legislative Districts - Upper Chamber
//Column 1	USPS United States Postal Service State Abbreviation
//Column 2	GEOID Geographic Identifier - fully concatenated geographic code(State FIPS and district number)
//Column 3	NAME Name
//Column 4	ALAND Land Area(square meters) - Created for statistical purposes only
//Column 5	AWATER Water Area(square meters) - Created for statistical purposes only
//Column 6	ALAND_SQMI Land Area(square miles) - Created for statistical purposes only
//Column 7	AWATER_SQMI Water Area(square miles) - Created for statistical purposes only
//Column 8	INTPTLAT Latitude(decimal degrees) First character is blank or "-" denoting North or South latitude respectively
//Column 9	INTPTLONG Longitude(decimal degrees) First character is blank or "-" denoting East or West longitude respectively


//State Legislative Districts - Lower Chamber
//Column 1	USPS United States Postal Service State Abbreviation
//Column 2	GEOID Geographic Identifier - fully concatenated geographic code(State FIPS and district number)
//Column 3	NAME Name
//Column 4	ALAND Land Area(square meters) - Created for statistical purposes only
//Column 5	AWATER Water Area(square meters) - Created for statistical purposes only
//Column 6	ALAND_SQMI Land Area(square miles) - Created for statistical purposes only
//Column 7	AWATER_SQMI Water Area(square miles) - Created for statistical purposes only
//Column 8	INTPTLAT Latitude(decimal degrees) First character is blank or "-" denoting North or South latitude respectively
//Column 9	INTPTLONG Longitude(decimal degrees) First character is blank or "-" denoting East or West longitude respectively


//School Districts - Unified
//Column 1	USPS United States Postal Service State Abbreviation
//Column 2	GEOID Geographic Identifier - fully concatenated geographic code(State FIPS and local education agency number)
//Column 3	NAME Name
//Column 4	LOGRADE Lowest Grade Provided
//Column 5	HIGRADE Highest Grade Provided
//Column 6	ALAND Land Area(square meters) - Created for statistical purposes only
//Column 7	AWATER Water Area(square meters) - Created for statistical purposes only
//Column 8	ALAND_SQMI Land Area(square miles) - Created for statistical purposes only
//Column 9	AWATER_SQMI Water Area(square miles) - Created for statistical purposes only
//Column 10	INTPTLAT Latitude(decimal degrees) First character is blank or "-" denoting North or South latitude respectively
//Column 11	INTPTLONG Longitude(decimal degrees) First character is blank or "-" denoting East or West longitude respectively

//School Districts - Secondary
//Column Label   Description
//Column 1	USPS United States Postal Service State Abbreviation
//Column 2	GEOID Geographic Identifier - fully concatenated geographic code(State FIPS and local education agency number)
//Column 3	NAME Name
//Column 4	LOGRADE Lowest Grade Provided
//Column 5	HIGRADE Highest Grade Provided
//Column 6	ALAND Land Area(square meters) - Created for statistical purposes only
//Column 7	AWATER Water Area(square meters) - Created for statistical purposes only
//Column 8	ALAND_SQMI Land Area(square miles) - Created for statistical purposes only
//Column 9	AWATER_SQMI Water Area(square miles) - Created for statistical purposes only
//Column 10	INTPTLAT Latitude(decimal degrees) First character is blank or "-" denoting North or South latitude respectively
//Column 11	INTPTLONG Longitude(decimal degrees) First character is blank or "-" denoting East or West longitude respectively

//School Districts - Elementary
//Column Label   Description
//Column 1	USPS United States Postal Service State Abbreviation
//Column 2	GEOID Geographic Identifier - fully concatenated geographic code(State FIPS and local education agency number)
//Column 3	NAME Name
//Column 4	LOGRADE Lowest Grade Provided
//Column 5	HIGRADE Highest Grade Provided
//Column 6	ALAND Land Area(square meters) - Created for statistical purposes only
//Column 7	AWATER Water Area(square meters) - Created for statistical purposes only
//Column 8	ALAND_SQMI Land Area(square miles) - Created for statistical purposes only
//Column 9	AWATER_SQMI Water Area(square miles) - Created for statistical purposes only
//Column 10	INTPTLAT Latitude(decimal degrees) First character is blank or "-" denoting North or South latitude respectively
//Column 11	INTPTLONG Longitude(decimal degrees) First character is blank or "-" denoting East or West longitude respectively

//Places
//    Column Label   Description
//Column 1	USPS United States Postal Service State Abbreviation
//Column 2	GEOID Geographic Identifier - fully concatenated geographic code(State FIPS and Place FIPS)
//Column 3	ANSICODE American National Standards Insititute code
//Column 4	NAME Name
//Column 5	LSAD Legal/Statistical area descriptor
//Column 6	FUNCSTAT Functional status of entity
//Column 7	ALAND Land Area(square meters) - Created for statistical purposes only
//Column 8	AWATER Water Area(square meters) - Created for statistical purposes only
//Column 9	ALAND_SQMI Land Area(square miles) - Created for statistical purposes only
//Column 10	AWATER_SQMI Water Area(square miles) - Created for statistical purposes only
//Column 11	INTPTLAT Latitude(decimal degrees) First character is blank or "-" denoting North or South latitude respectively
//Column 12	INTPTLONG Longitude(decimal degrees) First character is blank or "-" denoting East or West longitude respectively


//County Subdivisions
//Column Label   Description
//Column 1	USPS United States Postal Service State Abbreviation
//Column 2	GEOID Geographic Identifier - fully concatenated geographic code(State FIPS, County FIPS, County Subdivision FIPS)
//Column 3	ANSICODE American National Standards Institute code
//Column 4	NAME Name
//Column 5	FUNCSTAT Functional status of entity
//Column 6	ALAND Land Area(square meters) - Created for statistical purposes only
//Column 7	AWATER Water Area(square meters) - Created for statistical purposes only
//Column 8	ALAND_SQMI Land Area(square miles) - Created for statistical purposes only
//Column 9	AWATER_SQMI Water Area(square miles) - Created for statistical purposes only
//Column 10	INTPTLAT Latitude(decimal degrees) First character is blank or "-" denoting North or South latitude respectively
//Column 11	INTPTLONG Longitude(decimal degrees) First character is blank or "-" denoting East or West longitude respectively

//Counties
//    Column Label   Description
//Column 1	USPS United States Postal Service State Abbreviation
//Column 2	GEOID Geographic Identifier - fully concatenated geographic code(State FIPS and County FIPS)
//Column 3	ANSICODE American National Standards Institute code
//Column 4	NAME Name
//Column 5	ALAND Land Area(square meters) - Created for statistical purposes only
//Column 6	AWATER Water Area(square meters) - Created for statistical purposes only
//Column 7	ALAND_SQMI Land Area(square miles) - Created for statistical purposes only
//Column 8	AWATER_SQMI Water Area(square miles) - Created for statistical purposes only
//Column 9	INTPTLAT Latitude(decimal degrees) First character is blank or "-" denoting North or South latitude respectively
//Column 10	INTPTLONG Longitude(decimal degrees) First character is blank or "-" denoting East or West longitude respectively

//Core Based Statistical Areas
//Column Label   Description
//Column 1	CSAFP Combined statistical area code(may be blank for some records)
//Column 2	GEOID Metropolitan statistical area/micropolitan statistical area code
//Column 3	NAME Name
//Column 4	CBSA_TYPE Metropolitan/micropolitan status indicator
//Column 5	ALAND Land Area(square meters) - Created for statistical purposes only
//Column 6	AWATER Water Area(square meters) - Created for statistical purposes only
//Column 7	ALAND_SQMI Land Area(square miles) - Created for statistical purposes only
//Column 8	AWATER_SQMI Water Area(square miles) - Created for statistical purposes only
//Column 9	INTPTLAT Latitude(decimal degrees) First character is blank or "-" denoting North or South latitude respectively
//Column 10	INTPTLONG Longitude(decimal degrees) First character is blank or "-" denoting East or West longitude respectively

//    Census Tracts
//Column Label   Description
//Column 1	USPS United States Postal Service State Abbreviation
//Column 2	GEOID Geographic Identifier - fully concatenated geographic code(State FIPS, County FIPS, census tract number)
//Column 3	ALAND Land Area(square meters) - Created for statistical purposes only
//Column 4	AWATER Water Area(square meters) - Created for statistical purposes only
//Column 5	ALAND_SQMI Land Area(square miles) - Created for statistical purposes only
//Column 6	AWATER_SQMI Water Area(square miles) - Created for statistical purposes only
//Column 7	INTPTLAT Latitude(decimal degrees) First character is blank or "-" denoting North or South latitude respectively
//Column 8	INTPTLONG Longitude(decimal degrees) First character is blank or "-" denoting East or West longitude respectively

//    American Indian Areas - Legal American Indian Areas Off-Reservation Trust Lands and Hawaiian Home Lands
//Column  Label Description
//Column 1	GEOID Geographic Identifier - fully concatenated geographic code(AIANNHCE)
//Column 2	ANSICODE American National Standards Institute code
//Column 3	NAME Off-Reservation Trust Land or Hawaiian Home Land full name
//Column 4	ALAND Land Area(square meters) - Created for statistical purposes only
//Column 5	AWATER Water Area(square meters) - Created for statistical purposes only
//Column 6	ALAND_SQMI Land Area(square miles) - Created for statistical purposes only
//Column 7	AWATER_SQMI Water Area(square miles) - Created for statistical purposes only
//Column 8	INTPTLAT Latitude(decimal degrees) First character is blank or "-" denoting North or South latitude respectively
//Column 9	INTPTLONG Longitude(decimal degrees) First character is blank or "-" denoting East or West longitude respectively


//American Indian Areas - Legal and Statistical American Indian Areas
//Column Label   Description
//Column 1	GEOID Geographic Identifier - fully concatenated geographic code(AIANNHCE)
//Column 2	ANSICODE American National Standards Institute code
//Column 3	NAME American Indian Area full name
//Column 4	ALAND Land Area(square meters) - Created for statistical purposes only
//Column 5	AWATER Water Area(square meters) - Created for statistical purposes only
//Column 6	ALAND_SQMI Land Area(square miles) - Created for statistical purposes only
//Column 7	AWATER_SQMI Water Area(square miles) - Created for statistical purposes only
//Column 8	INTPTLAT Latitude(decimal degrees) First character is blank or "-" denoting North or South latitude respectively
//Column 9	INTPTLONG Longitude(decimal degrees) First character is blank or "-" denoting East or West longitude respectively


//    American Indian Areas - Legal American Indian Areas Excluding Off-Reservation Trust Lands and Hawaiian Home Land Records
//    Column Label   Description
//Column 1	GEOID Geographic Identifier - fully concatenated geographic code(AIANNHCE)
//Column 2	ANSICODE American National Standards Institute code
//Column 3	NAME American Indian Area full name
//Column 4	ALAND Land Area(square meters) - Created for statistical purposes only
//Column 5	AWATER Water Area(square meters) - Created for statistical purposes only
//Column 6	ALAND_SQMI Land Area(square miles) - Created for statistical purposes only
//Column 7	AWATER_SQMI Water Area(square miles) - Created for statistical purposes only
//Column 8	INTPTLAT Latitude(decimal degrees) First character is blank or "-" denoting North or South latitude respectively
//Column 9	INTPTLONG Longitude(decimal degrees) First character is blank or "-" denoting East or West longitude respectively


//    American Indian Areas - Legal American Indian Reservations and Off-Reservation Trust Lands
//    Column Label   Description
//Column 1	GEOID Geographic Identifier - fully concatenated geographic code(AIANNHCE)
//Column 2	ANSICODE American National Standards Institute code
//Column 3	NAME American Indian Reservation and Off-Reservation Trust Land full name
//Column 4	ALAND Land Area(square meters) - Created for statistical purposes only
//Column 5	AWATER Water Area(square meters) - Created for statistical purposes only
//Column 6	ALAND_SQMI Land Area(square miles) - Created for statistical purposes only
//Column 7	AWATER_SQMI Water Area(square miles) - Created for statistical purposes only
//Column 8	INTPTLAT Latitude(decimal degrees) First character is blank or "-" denoting North or South latitude respectively
//Column 9	INTPTLONG Longitude(decimal degrees) First character is blank or "-" denoting East or West longitude respectively


//114th Congressional Districts
//Column Label   Description
//Column 1	USPS United States Postal Service State Abbreviation
//Column 2	GEOID Geographic Identifier - fully concatenated geographic code(State FIPS and district number)
//Column 3	ALAND Land Area(square meters) - Created for statistical purposes only
//Column 4	AWATER Water Area(square meters) - Created for statistical purposes only
//Column 5	ALAND_SQMI Land Area(square miles) - Created for statistical purposes only
//Column 6	AWATER_SQMI Water Area(square miles) - Created for statistical purposes only
//Column 7	INTPTLAT Latitude(decimal degrees) First character is blank or "-" denoting North or South latitude respectively
//Column 8	INTPTLONG Longitude(decimal degrees) First character is blank or "-" denoting East or West longitude respectively