// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TreeMon.Data;
using TreeMon.Data.Logging.Models;
using TreeMon.Models;
using TreeMon.Models.App;
using TreeMon.Models.General;
using TreeMon.Utilites.Extensions;

namespace TreeMon.Managers.General
{
    public class TagManager : BaseManager 
    {
        public TagManager(string connectionKey, string sessionKey) : base(connectionKey, sessionKey)
        {
            Debug.Assert(!string.IsNullOrWhiteSpace(connectionKey), "TagManager CONTEXT IS NULL!");


            this._connectionKey = connectionKey;
        }

        public ServiceResult Delete(INode n )
        {
            ServiceResult res = ServiceResponse.OK();

            if (n == null)
                return ServiceResponse.Error("No record sent.");

            if (!this.DataAccessAuthorized(n, "DELETE", false)) return ServiceResponse.Error("You are not authorized this action.");

            var s = (Tag)n;

            
                using (var context = new TreeMonDbContext(this._connectionKey))
                {
                    if (context.Delete<Tag>(s) == 0)
                        return ServiceResponse.Error(s.Value + " failed to delete. ");
                }
            return ServiceResponse.OK();
        }

        public List<Tag> Search(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return new List<Tag>();
            using (var context = new TreeMonDbContext(this._connectionKey))
            {
                return context.GetAll<Tag>()?.Where(w => (w.Value?.EqualsIgnoreCase(name) ?? false) && w.AccountUUID == this._requestingUser.AccountUUID).ToList();
            }
            ///if (!this.DataAccessAuthorized(s, "GET", false)) return ServiceResponse.Error("You are not authorized this action.");
        }


        public Tag GetTag(string name, string tagType, string AccountUUID)
        {
            if (string.IsNullOrWhiteSpace(name))
                return null;
            using (var context = new TreeMonDbContext(this._connectionKey))
            {
                return context.GetAll<Tag>()?.FirstOrDefault(w => (w.Value?.EqualsIgnoreCase(name) ?? false) && (w.Type?.EqualsIgnoreCase(tagType) ?? false) && w.AccountUUID == AccountUUID);
            }
            ///if (!this.DataAccessAuthorized(s, "GET", false)) return ServiceResponse.Error("You are not authorized this action.");
        }

        public List<Tag> GetTags(string accountUUID, bool deleted = false, bool includeDefaults = false)
        {
            ///if (!this.DataAccessAuthorized(s, "GET", false)) return ServiceResponse.Error("You are not authorized this action.");
            using (var context = new TreeMonDbContext(this._connectionKey))
            {
                if (includeDefaults)
                    return context.GetAll<Tag>()?.Where(sw => (sw.AccountUUID == accountUUID || sw.AccountUUID == SystemFlag.Default.Account)  ).OrderBy(ob => ob.Value).ToList();

                return context.GetAll<Tag>()?.Where(sw => (sw.AccountUUID == accountUUID) ).OrderBy(ob => ob.Value).ToList();
            }
        }



        public List<Tag> GetTags(string tagType, string accountUUID, bool deleted = false)
        {
            using (var context = new TreeMonDbContext(this._connectionKey))
            {
                return context.GetAll<Tag>()?.Where(sw => (sw.Type?.EqualsIgnoreCase(tagType) ?? false) && (sw.AccountUUID == accountUUID) ).OrderBy(ob => ob.Value).ToList();
            }
            ///if (!this.DataAccessAuthorized(s, "GET", false)) return ServiceResponse.Error("You are not authorized this action.");
        }

        public Tag Get(string uuid)
        {
            if (string.IsNullOrWhiteSpace(uuid))
                return null;
            using (var context = new TreeMonDbContext(this._connectionKey))
            {
                return context.GetAll<Tag>()?.FirstOrDefault(sw => sw.UUID == uuid);
            }
            ///if (!this.DataAccessAuthorized(s, "GET", false)) return ServiceResponse.Error("You are not authorized this action.");
        }

        public ServiceResult Insert(INode n)
        {
            if (!this.DataAccessAuthorized(n, "post", false)) return ServiceResponse.Error("You are not authorized this action.");

            n.Initialize(this._requestingUser.UUID, this._requestingUser.AccountUUID, this._requestingUser.RoleWeight);

            var s = (Tag)n;

            using (var context = new TreeMonDbContext(this._connectionKey))
            {
                if (context.Insert<Tag>(s))
                    return ServiceResponse.OK("", s);
            }
            return ServiceResponse.Error("An error occurred inserting Tag " + s.Value);
        }

        public ServiceResult Update(INode n)
        {
            if (n == null)
                return ServiceResponse.Error("Invalid Tag data.");

            if (!this.DataAccessAuthorized(n, "PATCH", false)) return ServiceResponse.Error("You are not authorized this action.");

            var s = (Tag)n;

            using (var context = new TreeMonDbContext(this._connectionKey))
            {
                if (context.Update<Tag>(s) > 0)
                    return ServiceResponse.OK();
            }
            return ServiceResponse.Error("System error, Tag was not updated.");
        }

    }
}
