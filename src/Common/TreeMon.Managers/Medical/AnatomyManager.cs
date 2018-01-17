// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TreeMon.Data;
using TreeMon.Managers;
using TreeMon.Models.App;
using TreeMon.Utilites.Extensions;

namespace TreeMon.Models.Medical
{
    public class AnatomyManager : BaseManager, ICrud
    {
        public AnatomyManager(string connectionKey, string sessionKey) : base(connectionKey, sessionKey)
        {
            Debug.Assert(!string.IsNullOrWhiteSpace(connectionKey), "AnatomyManager CONTEXT IS NULL!");

    
                 this._connectionKey = connectionKey;
        }

        public ServiceResult Delete(INode n, bool purge = false)
        {
            ServiceResult res = ServiceResponse.OK();

            if (n == null)
                return ServiceResponse.Error("No record sent.");

            if (!this.DataAccessAuthorized(n, "delete", false)) return ServiceResponse.Error("You are not authorized this action.");

            var s = (Anatomy)n;
            if (purge)
            {
                using (var context = new TreeMonDbContext(this._connectionKey))
                {
                    if (context.Delete<Anatomy>(s) == 0)
                        return ServiceResponse.Error(s.Name + " failed to delete. ");
                }
            }

            //get the Anatomy from the table with all the data so when its updated it still contains the same data.
            s =  (Anatomy)this.Get(s.UUID);
            if (s == null)
                return ServiceResponse.Error("Anatomy not found");

            s.Deleted = true;
            using (var context = new TreeMonDbContext(this._connectionKey))
            {
                if (context.Update<Anatomy>(s) == 0)
                    return ServiceResponse.Error(s.Name + " failed to delete. ");
            }
            return res;
        }

        public List<Anatomy> Search(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return null;
            using (var context = new TreeMonDbContext(this._connectionKey))
            {
                return context.GetAll<Anatomy>().Where(sw => sw.Name.EqualsIgnoreCase(name) && sw.AccountUUID == this._requestingUser.AccountUUID).ToList();
            }
            //if (!this.DataAccessAuthorized(s, "GET", false)) return ServiceResponse.Error("You are not authorized this action.");
        }

        public List<Anatomy> GetAnatomies(string accountUUID, bool deleted = false)
        {
            using (var context = new TreeMonDbContext(this._connectionKey))
            {
                return context.GetAll<Anatomy>().Where(sw => (sw.AccountUUID == accountUUID) && sw.Deleted == deleted).OrderBy(ob => ob.Name).ToList();
            }
            //if (!this.DataAccessAuthorized(s, "GET", false)) return ServiceResponse.Error("You are not authorized this action.");
        }


        public INode Get(string uuid)
        {
            if (string.IsNullOrWhiteSpace(uuid))
                return null;
                    using (var context = new TreeMonDbContext(this._connectionKey))
                    {
                        return context.GetAll<Anatomy>().FirstOrDefault(sw => sw.UUID == uuid);
                    }
            //if (!this.DataAccessAuthorized(s, "GET", false)) return ServiceResponse.Error("You are not authorized this action.");
        }

        public ServiceResult Insert(INode n, bool validateFirst = true)
        {
            if (!this.DataAccessAuthorized(n, "post", false)) return ServiceResponse.Error("You are not authorized this action.");

            n.Initialize(this._requestingUser.UUID, this._requestingUser.AccountUUID, this._requestingUser.RoleWeight);

            var s = (Anatomy)n;

            using (var context = new TreeMonDbContext(this._connectionKey))
            {
                if (validateFirst)
                {
                    Anatomy dbU = context.GetAll<Anatomy>().FirstOrDefault(wu => wu.Name.EqualsIgnoreCase(s.Name) && wu.AccountUUID == s.AccountUUID);

                    if (dbU != null)
                        return ServiceResponse.Error("Anatomy already exists.");
                }
  
                if (context.Insert<Anatomy>(s))
                    return ServiceResponse.OK("",s);
            }
            return ServiceResponse.Error("An error occurred inserting Anatomy " + s.Name);
        }

        public ServiceResult Update(INode n)
        {
            if (n == null)
                return ServiceResponse.Error("Invalid Anatomy data.");

            if (!this.DataAccessAuthorized(n, "patch", false)) return ServiceResponse.Error("You are not authorized this action.");

            var s = (Anatomy)n;

            using (var context = new TreeMonDbContext(this._connectionKey))
            {
                if (context.Update<Anatomy>(s) > 0)
                    return ServiceResponse.OK();
            }
            return ServiceResponse.Error("System error, Anatomy was not updated.");
        }


        public List<AnatomyTag> GetAnatomyTags(string accountUUID, bool deleted = false)
        {
            using (var context = new TreeMonDbContext(this._connectionKey))
            {
                return context.GetAll<AnatomyTag>().Where(sw => (sw.AccountUUID == accountUUID) && sw.Deleted == deleted).OrderBy(ob => ob.Name).ToList();
            }
            //if (!this.DataAccessAuthorized(s, "GET", false)) return ServiceResponse.Error("You are not authorized this action.");
        }

        public int Delete(AnatomyTag s, bool purge = false)
        {
            if (s == null)
                return 0;

            if (!this.DataAccessAuthorized(s, "DELETE", false)) return 0;

            List<AnatomyTag> pms = new List<AnatomyTag>();

            if (purge)
            {
                using (var context = new TreeMonDbContext(this._connectionKey))
                {
                    return context.Delete<AnatomyTag>(s);
                }
            }

            //get the AnatomyTag from the table with all the data so when its updated it still contains the same data.
            s = this.GetAnatomyTagBy(s.UUID);
            if (s == null)
                return 0;
            s.Deleted = true;
            using (var context = new TreeMonDbContext(this._connectionKey))
            {
                return context.Update<AnatomyTag>(s);
            }
        }

        public AnatomyTag GetAnatomyTag(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return null;
            using (var context = new TreeMonDbContext(this._connectionKey))
            {
                return context.GetAll<AnatomyTag>().FirstOrDefault(sw => sw.Name.EqualsIgnoreCase(name));
            }
            //if (!this.DataAccessAuthorized(s, "GET", false)) return ServiceResponse.Error("You are not authorized this action.");
        }

        public List<AnatomyTag> GetAnatomyTag(string accountUUID, bool deleted = false)
        {
            using (var context = new TreeMonDbContext(this._connectionKey))
            {

                return context.GetAll<AnatomyTag>().Where(sw => (sw.AccountUUID == accountUUID) && sw.Deleted == deleted).OrderBy(ob => ob.Name).ToList();
            }
            //if (!this.DataAccessAuthorized(s, "GET", false)) return ServiceResponse.Error("You are not authorized this action.");
        }


        public AnatomyTag GetAnatomyTagBy(string uuid)
        {
            if (string.IsNullOrWhiteSpace(uuid))
                return null;
            using (var context = new TreeMonDbContext(this._connectionKey))
            {
                return context.GetAll<AnatomyTag>().FirstOrDefault(sw => sw.UUID == uuid);
            }
            //if (!this.DataAccessAuthorized(s, "GET", false)) return ServiceResponse.Error("You are not authorized this action.");
        }

        public ServiceResult Insert(AnatomyTag s, bool validateFirst = true)
        {
            using (var context = new TreeMonDbContext(this._connectionKey))
            {
                if (validateFirst)
                {
                    AnatomyTag dbU = context.GetAll<AnatomyTag>().FirstOrDefault(wu => wu.Name.EqualsIgnoreCase(s.Name) && wu.AccountUUID == s.AccountUUID);

                    if (dbU != null)
                        return ServiceResponse.Error("AnatomyTag already exists.");
                }
                if (string.IsNullOrWhiteSpace(s.UUID))
                    s.UUID = Guid.NewGuid().ToString("N");
                s.UUIDType = "AnatomyTag";

                if (!this.DataAccessAuthorized(s, "post", false)) return ServiceResponse.Error("You are not authorized this action.");

                if (context.Insert<AnatomyTag>(s))
                    return ServiceResponse.OK("",s);
            }
            return ServiceResponse.Error("An error occurred inserting AnatomyTag " + s.Name);
        }

        public ServiceResult Update(AnatomyTag s)
        {
            if (s == null)
                return ServiceResponse.Error("Invalid AnatomyTag data.");

            if (!this.DataAccessAuthorized(s, "PATCH", false)) return ServiceResponse.Error("You are not authorized this action.");

            using (var context = new TreeMonDbContext(this._connectionKey))
            {
                if (context.Update<AnatomyTag>(s) > 0)
                    return ServiceResponse.OK();
            }
            return ServiceResponse.Error("System error, AnatomyTag was not updated.");
        }
    }
}
