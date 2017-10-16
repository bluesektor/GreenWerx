// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TreeMon.Data;
using TreeMon.Models;
using TreeMon.Models.App;
using TreeMon.Models.Event;
using TreeMon.Utilites.Extensions;

namespace TreeMon.Managers.Event
{
    public class NotificationManager : BaseManager, ICrud
    {
        public NotificationManager(string connectionKey, string sessionKey) : base(connectionKey, sessionKey)
        {
            Debug.Assert(!string.IsNullOrWhiteSpace (connectionKey), "NotificationManager CONTEXT IS NULL!");

            this._connectionKey = connectionKey;
        }

        public ServiceResult Delete(INode n, bool purge = false)
        {
            ServiceResult res = ServiceResponse.OK();
            if (n == null)
                return ServiceResponse.Error("No record sent.");

            if (!this.DataAccessAuthorized(n, "delete", false)) return ServiceResponse.Error("You are not authorized this action.");

            var s = (Notification)n;
            if (purge)
            {
                using (var context = new TreeMonDbContext(this._connectionKey))
                {
                    if (context.Delete<Notification>(s) == 0)
                        return ServiceResponse.Error(s.Name + " failed to delete. ");
                }
            }

            //get the Notification from the table with all the data so when its updated it still contains the same data.
            s = (Notification) this.GetBy(s.UUID);
            if (s == null)
                return ServiceResponse.Error("Symptom not found");
            s.Deleted = true;
            using (var context = new TreeMonDbContext(this._connectionKey))
            {
                if (context.Update<Notification>(s) == 0)
                    return ServiceResponse.Error(s.Name + " failed to delete. ");
            }
            return res;
        }

        public INode Get(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return null;
            using (var context = new TreeMonDbContext(this._connectionKey))
            {
                return context.GetAll<Notification>().FirstOrDefault(sw => sw.Name.EqualsIgnoreCase(name));
            }
            //if (!this.DataAccessAuthorized(s, "GET", false)) return ServiceResponse.Error("You are not authorized this action.");
        }

        public List<Notification> GetNotifications(string accountUUID, bool deleted = false)
        {
            using (var context = new TreeMonDbContext(this._connectionKey))
            {
                return context.GetAll<Notification>().Where(sw => (sw.AccountUUID == accountUUID) && sw.Deleted == deleted).OrderBy(ob => ob.Name).ToList();
            }
            //if (!this.DataAccessAuthorized(s, "GET", false)) return ServiceResponse.Error("You are not authorized this action.");
        }


        public INode GetBy(string uuid)
        {
            if (string.IsNullOrWhiteSpace(uuid))
                return null;
            using (var context = new TreeMonDbContext(this._connectionKey))
            {
                return context.GetAll<Notification>().FirstOrDefault(sw => sw.UUID == uuid);
            }
            //if (!this.DataAccessAuthorized(s, "GET", false)) return ServiceResponse.Error("You are not authorized this action.");
        }

        public ServiceResult Insert(INode n, bool validateFirst = true)
        {
            if (!this.DataAccessAuthorized(n, "post", false)) return ServiceResponse.Error("You are not authorized this action.");

            n.Initialize(this._requestingUser.UUID, this._requestingUser.AccountUUID, this._requestingUser.RoleWeight);

            var s = (Notification)n;

            using (var context = new TreeMonDbContext(this._connectionKey))
            {
                if (validateFirst)
                {
                    Notification dbU = context.GetAll<Notification>().FirstOrDefault(wu => wu.Name.EqualsIgnoreCase(s.Name) && wu.AccountUUID == s.AccountUUID);
              
                    if (dbU != null)
                        return ServiceResponse.Error("Notification already exists.");
                }
    
                if (context.Insert<Notification>(s))
                    return ServiceResponse.OK("",s);
            }
                return ServiceResponse.Error("An error occurred inserting Notification " + s.Name);
        }

        public ServiceResult Update(INode n)
        {
            if (n == null)
                return ServiceResponse.Error("Invalid Notification data.");

            if (!this.DataAccessAuthorized(n, "PATCH", false)) return ServiceResponse.Error("You are not authorized this action.");

            var s = (Notification)n;

            using (var context = new TreeMonDbContext(this._connectionKey))
            {
                if (context.Update<Notification>(s) > 0)
                    return ServiceResponse.OK();
            }
            return ServiceResponse.Error("System error, Notification was not updated.");
        }
    }
}
