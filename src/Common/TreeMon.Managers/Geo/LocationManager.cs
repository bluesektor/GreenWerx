// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
using Dapper;
using MoreLinq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TreeMon.Data;
using TreeMon.Data.Logging;
using TreeMon.Data.Logging.Models;
using TreeMon.Models;
using TreeMon.Models.App;
using TreeMon.Models.Geo;
using TreeMon.Models.Membership;
using TreeMon.Utilites.Extensions;

namespace TreeMon.Managers.Store
{
    public class LocationManager : BaseManager,ICrud
    {
        private readonly SystemLogger _logger;

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
                //SQLITE
                //this was the only way I could get it to delete a RolePermission without some stupid EF error.
                //object[] paramters = new object[] { rp.PermissionUUID , rp.RoleUUID ,rp.AccountUUID };
                //context.Delete<RolePermission>("WHERE PermissionUUID=? AND RoleUUID=? AND AccountUUID=?", paramters);
                //  context.Delete<RolePermission>(rp);
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
                //if (!this.DataAccessAuthorized(p, "GET", false))return ServiceResponse.Error("You are not authorized this action.");

                if (string.IsNullOrWhiteSpace(accountUUID))
                    return context.GetAll<Location>().ToList();

                return context.GetAll<Location>().Where(pw => pw.AccountUUID == accountUUID && pw.Deleted == false).ToList();
            }
        }

        public List<Location> GetAll()
        {
            //if (!this.DataAccessAuthorized(dbP, "GET", false)) return ServiceResponse.Error("You are not authorized this action.");
            using (var context = new TreeMonDbContext(this._connectionKey))
            {
                    return context.GetAll<Location>()
                        .OrderBy(o => o.Name).ToList();
            }
        }

        public List<string> GetLocationTypes(string accountUUID)
        {
            List<string> geoTypes = new List<string>();
            //if (!this.DataAccessAuthorized(dbP, "GET", false)) return ServiceResponse.Error("You are not authorized this action.");
            using (var context = new TreeMonDbContext(this._connectionKey))
            {
                geoTypes = context.GetAll<Location>().Where(pw =>( pw.AccountUUID == accountUUID || pw.AccountUUID == SystemFlag.Default.Account) && pw.Deleted == false &&  string.IsNullOrWhiteSpace(pw.LocationType) == false).DistinctBy(pd => pd.LocationType).Select(s => s.LocationType).ToList();//.GroupBy( pg => pg.Category ).Select(ps => ps.First()).ToList();
            }
            return geoTypes;
        }

        public INode Get(string uuid)
        {
            if (string.IsNullOrWhiteSpace(uuid))
                return null;
            using (var context = new TreeMonDbContext(this._connectionKey))
            {
                //if (!this.DataAccessAuthorized(p, "GET", false))return ServiceResponse.Error("You are not authorized this action.");
                return context.GetAll<Location>().FirstOrDefault(sw => sw.UUID == uuid);
            }
        }

        public List<Location> Search(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return null;
            using (var context = new TreeMonDbContext(this._connectionKey))
            {
                //if (!this.DataAccessAuthorized(p, "GET", false))return ServiceResponse.Error("You are not authorized this action.");
                if (context.GetAll<Location>()?.Any() ?? false)
                    return context.GetAll<Location>().Where(sw => sw.Name.EqualsIgnoreCase(name)).ToList();
                else
                    return null;
            }
        }

        public List<Location> GetLocations(string accountUUID, bool deleted = false, bool includeSystemAccount = false)
        {
            using (var context = new TreeMonDbContext(this._connectionKey))
            {
                //if (!this.DataAccessAuthorized(p, "GET", false))
                //    return ServiceResponse.Error("You are not authorized this action.");
                if (includeSystemAccount)
                {
                    return context.GetAll<Location>().Where(sw => (sw.AccountUUID == accountUUID) && sw.Deleted == deleted).GroupBy(x => x.Name).Select(group => group.First()).OrderBy(ob => ob.Name).ToList();
                }

                return context.GetAll<Location>().Where(sw => (sw.AccountUUID == accountUUID) && sw.Deleted == deleted).OrderBy(ob => ob.Name).ToList();
            }
        }

        public ServiceResult Update(INode n)
        {
            if (!this.DataAccessAuthorized(n, "PATCH", false))
                return ServiceResponse.Error("You are not authorized this action.");

            var p = (Location)n;

            ServiceResult res = ServiceResponse.OK();
            using (var context = new TreeMonDbContext(this._connectionKey))
            {
                if (context.Update<Location>(p) == 0)
                    return ServiceResponse.Error(p.Name + " failed to update. ");
            }
            return res;
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
                List<Location> states = context.GetAll<Location>().Where(w => w.UUParentID == countryUUID
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
            List<Location> cities = context.GetAll<Location>().Where(w => w.UUParentID == stateUUID
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
        public ServiceResult Insert(INode n,  bool validateFirst = true)
        {
            if (!this.DataAccessAuthorized(n, "POST", false)) return ServiceResponse.Error("You are not authorized this action.");

            n.Initialize(this._requestingUser.UUID, this._requestingUser.AccountUUID, this._requestingUser.RoleWeight);

            var p = (Location)n;
            if (validateFirst)
            {
                //Location dbU = Get(p.Name) as Location;

                //if (dbU != null)
                //{
                //    if (p.AccountUUID == this._requestingUser.AccountUUID)
                //        return ServiceResponse.Error("Location already exists.");
                //}

                if (string.IsNullOrWhiteSpace(p.CreatedBy))
                    return ServiceResponse.Error("You must assign who the Location was created by.");

                if (string.IsNullOrWhiteSpace(p.AccountUUID))
                    return ServiceResponse.Error("The account id is empty.");
            }

            if (!this.DataAccessAuthorized(p, "POST", false))
                return ServiceResponse.Error("You are not authorized this action.");
 
            using (var context = new TreeMonDbContext(this._connectionKey))
            {
                if (context.Insert<Location>(p))
                    return ServiceResponse.OK("", p);
            }
            return ServiceResponse.Error("An error occurred inserting Location " + p.Name);
        }

    }
}
