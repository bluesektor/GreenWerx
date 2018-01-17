// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TreeMon.Data;
using TreeMon.Models;
using TreeMon.Models.App;
using TreeMon.Models.Medical;
using TreeMon.Utilites.Extensions;

namespace TreeMon.Managers.Medical
{
    public class SideAffectManager : BaseManager, ICrud
    {
        public SideAffectManager(string connectionKey, string sessionKey) : base(connectionKey, sessionKey)
        {
            Debug.Assert(!string.IsNullOrWhiteSpace(connectionKey), "SideAffectManager CONTEXT IS NULL!");

            
                 this._connectionKey = connectionKey;
        }

        public ServiceResult Delete(INode n, bool purge = false)
        {
            ServiceResult res = ServiceResponse.OK();

            if (n == null)
                return ServiceResponse.Error("No record sent.");

            if (!this.DataAccessAuthorized(n, "DELETE", false)) return ServiceResponse.Error("You are not authorized this action.");

            var s = (SideAffect)n;
            if (purge)
            {
                using (var context = new TreeMonDbContext(this._connectionKey))
                {
                    if (context.Delete<SideAffect>(s) == 0)
                        return ServiceResponse.Error(s.Name + " failed to delete. ");
                }
            }

            //get the SideAffect from the table with all the data so when its updated it still contains the same data.
            s = (SideAffect)this.Get(s.UUID);
            if (s == null)
                return ServiceResponse.Error("Side affect not found");
            s.Deleted = true;
            using (var context = new TreeMonDbContext(this._connectionKey))
            {
                if (context.Update<SideAffect>(s) == 0)
                    return ServiceResponse.Error(s.Name + " failed to delete. ");
            }
            return res;
        }

        public List<SideAffect> Search( string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return null;
            using (var context = new TreeMonDbContext(this._connectionKey))
            {
                return context.GetAll<SideAffect>().Where(sw => sw.Name.EqualsIgnoreCase(name)).ToList();
            }
            //if (!this.DataAccessAuthorized(s, "GET", false)) return ServiceResponse.Error("You are not authorized this action.");
        }

        public List<SideAffect> GetSideAffects(string accountUUID, bool deleted = false)
        {
            using (var context = new TreeMonDbContext(this._connectionKey))
            {

                return context.GetAll<SideAffect>().Where(sw => (sw.AccountUUID == accountUUID) && sw.Deleted == deleted).OrderBy(ob => ob.Name).ToList();
            }
            //if (!this.DataAccessAuthorized(s, "GET", false)) return ServiceResponse.Error("You are not authorized this action.");
        }

        public List<SideAffect> GetSideAffects(string parentUUID, string accountUUID, bool deleted = false)
        {
            using (var context = new TreeMonDbContext(this._connectionKey))
            {
                if (string.IsNullOrWhiteSpace(parentUUID) || parentUUID == "0")
                    return context.GetAll<SideAffect>().Where(sw => (sw.UUParentID == "" || sw.UUParentID == null) && sw.AccountUUID == accountUUID && sw.Deleted == deleted).OrderBy(ob => ob.Name).ToList();

                return context.GetAll<SideAffect>().Where(sw => sw.UUParentID == parentUUID && sw.AccountUUID == accountUUID && sw.Deleted == deleted).OrderBy(ob => ob.Name).ToList();
            }
            //if (!this.DataAccessAuthorized(s, "GET", false)) return ServiceResponse.Error("You are not authorized this action.");
        }

        public List<SideAffect> GetSideAffectsByDose(string doseUUID, string parentUUID, string accountUUID, bool deleted = false)
        {
            using (var context = new TreeMonDbContext(this._connectionKey))
            {
                if (string.IsNullOrWhiteSpace(parentUUID) || parentUUID == "0")
                    return context.GetAll<SideAffect>().Where(sw => sw.DoseUUID == doseUUID && (sw.UUParentID == "" || sw.UUParentID == null) && sw.AccountUUID == accountUUID && sw.Deleted == deleted).OrderBy(ob => ob.Name).ToList();

                return context.GetAll<SideAffect>().Where(sw => sw.DoseUUID == doseUUID && sw.UUParentID == parentUUID && sw.AccountUUID == accountUUID && sw.Deleted == deleted).OrderBy(ob => ob.Name).ToList();
            }
            //if (!this.DataAccessAuthorized(s, "GET", false)) return ServiceResponse.Error("You are not authorized this action.");
        }


        public INode Get(string uuid)
{
    if (string.IsNullOrWhiteSpace(uuid))
        return null;
            using (var context = new TreeMonDbContext(this._connectionKey))
            {
                return context.GetAll<SideAffect>().FirstOrDefault(sw => sw.UUID == uuid);
            }
            //if (!this.DataAccessAuthorized(s, "GET", false)) return ServiceResponse.Error("You are not authorized this action.");
        }

        public ServiceResult Insert(INode n, bool validateFirst = true)
        {
            if (!this.DataAccessAuthorized(n, "post", false)) return ServiceResponse.Error("You are not authorized this action.");

            n.Initialize(this._requestingUser.UUID, this._requestingUser.AccountUUID, this._requestingUser.RoleWeight);

            var s = (SideAffect)n;

            using (var context = new TreeMonDbContext(this._connectionKey))
            {
                if (validateFirst)
                {
                    SideAffect dbU = context.GetAll<SideAffect>().FirstOrDefault(wu => wu.Name.EqualsIgnoreCase(s.Name) && wu.AccountUUID == s.AccountUUID) ;

                    if (dbU != null)
                        return ServiceResponse.Error("SideAffect already exists.");
                }
        
                if (context.Insert<SideAffect>(s))
                    return ServiceResponse.OK("",s);
            }
            return ServiceResponse.Error("An error occurred inserting SideAffect " + s.Name);
        }

        public ServiceResult Update(INode n)
        {
            if (n == null)
                return ServiceResponse.Error("Invalid SideAffect data.");

            if (!this.DataAccessAuthorized(n, "PATCH", false)) return ServiceResponse.Error("You are not authorized this action.");

            var s = (SideAffect)n;

            using (var context = new TreeMonDbContext(this._connectionKey))
            {
                if (context.Update<SideAffect>(s) > 0)
                    return ServiceResponse.OK();
            }
            return ServiceResponse.Error("System error, SideAffect was not updated.");
        }

    }
}
