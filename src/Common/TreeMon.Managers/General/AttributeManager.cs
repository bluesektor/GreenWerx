// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TreeMon.Data;
using TreeMon.Models;
using TreeMon.Models.App;
using TreeMon.Utilites.Extensions;
using TMG = TreeMon.Models.General;

namespace TreeMon.Managers.General
{
    public  class AttributeManager : BaseManager, ICrud
    {
        public AttributeManager(string connectionKey, string sessionKey) : base(connectionKey, sessionKey)
        {
            Debug.Assert(!string.IsNullOrWhiteSpace(connectionKey), "AttributeManager CONTEXT IS NULL!");
            this._connectionKey = connectionKey;
        }

        public ServiceResult Delete(INode n, bool purge = false)
        {
            ServiceResult res = ServiceResponse.OK();

            if (n == null)
                return ServiceResponse.Error("No record sent.");

            if (!this.DataAccessAuthorized(n, "delete", false)) return ServiceResponse.Error("You are not authorized this action.");

            var s = (TMG.Attribute)n;

            using (var context = new TreeMonDbContext(this._connectionKey))
            {
                if (context.Delete<TMG.Attribute>(s) == 0)
                    return ServiceResponse.Error(s.Name + " failed to delete. ");
            }
            return res;
        }

        public INode Get(string uuid)
        {
            if (string.IsNullOrWhiteSpace(uuid))
                return null;
            using (var context = new TreeMonDbContext(this._connectionKey))
            {
                return context.GetAll<TMG.Attribute>()?.FirstOrDefault(sw => sw.UUID == uuid);
            }
        }


        public List<TMG.Attribute> Search(string status)
        {
            if (string.IsNullOrWhiteSpace(status))
                return new List<Models.General.Attribute>();

            using (var context = new TreeMonDbContext(this._connectionKey))
            {
                return context.GetAll<TMG.Attribute>()?.Where(sw => sw.Status.EqualsIgnoreCase(status)).ToList();
            }
        }


        public List<TMG.Attribute> GetAttributes(string referenceUUID, string referenceType, string accountUUID)
        {
            using (var context = new TreeMonDbContext(this._connectionKey))
            {
                return context.GetAll<TMG.Attribute>()?.Where(sw => (sw.AccountUUID == accountUUID  && 
                                                                        sw.ReferenceUUID == referenceUUID && 
                                                                        (sw.ReferenceType?.EqualsIgnoreCase(referenceType)??false)))
                                                                        .OrderBy(ob => ob.Name).ToList();
            }
        }

        public List<TMG.Attribute> GetAttributes(string accountUUID)
        {
            using (var context = new TreeMonDbContext(this._connectionKey))
            {

                return context.GetAll<TMG.Attribute>()?.Where(sw => (sw.AccountUUID == accountUUID)).OrderBy(ob => ob.Status).ToList();
            }
            ///if (!this.DataAccessAuthorized(s, "GET", false)) return ServiceResponse.Error("You are not authorized this action.");
        }

        public ServiceResult Insert(INode n)
        {
            if (!this.DataAccessAuthorized(n, "POST", false)) return ServiceResponse.Error("You are not authorized this action.");

            n.Initialize(this._requestingUser.UUID, this._requestingUser.AccountUUID, this._requestingUser.RoleWeight);

            var s = (TMG.Attribute)n;

            using (var context = new TreeMonDbContext(this._connectionKey))
            {
              
                    TMG.Attribute dbU = context.GetAll<TMG.Attribute>()?.FirstOrDefault(wu => wu.Status.EqualsIgnoreCase(s.Status) && wu.AccountUUID == s.AccountUUID);

                    if (dbU != null)
                        return ServiceResponse.Error("Attribute already exists.");
                
          
                if (context.Insert<TMG.Attribute>(s))
                    return ServiceResponse.OK("", s);
            }
            return ServiceResponse.Error("An error occurred inserting Attribute " + s.Status);
        }

        public ServiceResult Update(INode n)
        {
            if (n == null)
                return ServiceResponse.Error("Invalid Attribute data.");

            if (!this.DataAccessAuthorized(n, "PATCH", false)) return ServiceResponse.Error("You are not authorized this action.");

            var s = (TMG.Attribute)n;

            using (var context = new TreeMonDbContext(this._connectionKey))
            {

                if (context.Update<TMG.Attribute>(s) > 0)
                    return ServiceResponse.OK();
            }
            return ServiceResponse.Error("System error, Attribute was not updated.");
        }
    }
}
