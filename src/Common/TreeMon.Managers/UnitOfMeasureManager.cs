// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TreeMon.Data;
using TreeMon.Data.Logging;
using TreeMon.Models;
using TreeMon.Models.App;
using TreeMon.Models.Store;
using TreeMon.Utilites.Extensions;

namespace TreeMon.Managers
{
    public class UnitOfMeasureManager : BaseManager, ICrud
    {
        private readonly SystemLogger _logger;

        public UnitOfMeasureManager(string connectionKey, string sessionKey) : base(connectionKey, sessionKey)
        {
            Debug.Assert(!string.IsNullOrWhiteSpace(connectionKey), "UnitOfMeasureManager CONTEXT IS NULL!");
            this._connectionKey = connectionKey;
            
            _logger = new SystemLogger(this._connectionKey);
        }

        


        public ServiceResult Delete(INode n, bool purge = false)
        {
            if (n == null)
                return ServiceResponse.Error("No record sent.");

            var s = (UnitOfMeasure)n;
            using (var context = new TreeMonDbContext(this._connectionKey))
            {
                if (purge)
                {
                    if (context.Delete<UnitOfMeasure>(s) > 0)
                        return ServiceResponse.OK();

                    return ServiceResponse.Error("Failed to purge record.");
                }

                //get the UnitOfMeasure from the table with all the data so when its updated it still contains the same data.
                s = (UnitOfMeasure)this.Get(s.UUID);
                if (s == null)
                    return ServiceResponse.Error("Measure not found.");

                s.Deleted = true;
                if (context.Update<UnitOfMeasure>(s) > 0)
                    return ServiceResponse.OK();

                return ServiceResponse.Error("Failed to delete record.");
            }
        }

        public List<UnitOfMeasure> Search(string name)
                {
                    if (string.IsNullOrWhiteSpace(name))
                        return new List<UnitOfMeasure> ();

            using (var context = new TreeMonDbContext(this._connectionKey))
            {
                return context.GetAll<UnitOfMeasure>().Where(sw => sw.Name.EqualsIgnoreCase(name)).ToList();
            }
                }

        public List<UnitOfMeasure> GetUnitsOfMeasure(string accountUUID, bool deleted = false)
        {
            using (var context = new TreeMonDbContext(this._connectionKey))
            {
                return context.GetAll<UnitOfMeasure>().Where(sw => (sw.AccountUUID == accountUUID) && sw.Deleted == deleted).OrderBy(ob => ob.Name).ToList();
            }
        }

        public List<UnitOfMeasure> GetUnitsOfMeasure(string accountUUID, string category, bool deleted = false)
        {
            using (var context = new TreeMonDbContext(this._connectionKey))
            {
                return context.GetAll<UnitOfMeasure>().Where(sw => (sw.AccountUUID == accountUUID) && (sw.Category?.EqualsIgnoreCase(category) ?? false) && sw.Deleted == deleted).OrderBy(ob => ob.Name).ToList();
            }
        }
        
        public INode Get(string uuid)
        {
            if (string.IsNullOrWhiteSpace(uuid))
                return null;
            using (var context = new TreeMonDbContext(this._connectionKey))
            {
                return context.GetAll<UnitOfMeasure>().FirstOrDefault(sw => sw.UUID == uuid);
            }
        }

        private UnitOfMeasure FindUnitOfMeasure(string uuid, string name, string AccountUUID, bool includeDefaultAccount = true)
        {
            UnitOfMeasure res = null;

            if (string.IsNullOrWhiteSpace(uuid) && string.IsNullOrWhiteSpace(name) == false)
                res = this.Search(name).FirstOrDefault();
            else
            {
                using (var context = new TreeMonDbContext(this._connectionKey))
                {
                    res = context.GetAll<UnitOfMeasure>().FirstOrDefault(w => w.UUID == uuid || ( w.Name?.EqualsIgnoreCase(name)??false));
                }
            }
            if (res == null)
                return res;

            if (includeDefaultAccount &&   res.AccountUUID == AccountUUID )
                return res;

            if (res.AccountUUID == AccountUUID)
                return res;

            return null;
        }

        public ServiceResult Insert(INode n)
        {
            if (n == null)
                return ServiceResponse.Error("Value is empty.");

            n.Initialize(this._requestingUser.UUID, this._requestingUser.AccountUUID, this._requestingUser.RoleWeight);

            var s = (UnitOfMeasure)n;

            using (var context = new TreeMonDbContext(this._connectionKey))
            {
             
                    UnitOfMeasure dbU = context.GetAll<UnitOfMeasure>().FirstOrDefault(wu => (wu.Name?.EqualsIgnoreCase(s.Name)??false) && wu.AccountUUID == s.AccountUUID);

                    if (dbU != null)
                        return ServiceResponse.Error("UnitOfMeasure already exists.");
             
     
                if (context.Insert<UnitOfMeasure>(s))
                    return ServiceResponse.OK("",s);
            }
                    return ServiceResponse.Error("An error occurred inserting UnitOfMeasure " + s.Name);
                }

        public ServiceResult Update(INode n )
        {
            if (n == null)
                return ServiceResponse.Error("Invalid UnitOfMeasure data.");

            var s = (UnitOfMeasure)n;

            using (var context = new TreeMonDbContext(this._connectionKey))
            {
                if (context.Update<UnitOfMeasure>(s) > 0)
                    return ServiceResponse.OK();
            }
            return ServiceResponse.Error("System error, UnitOfMeasure was not updated.");
        }

    }
}
