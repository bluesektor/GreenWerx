// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
using MoreLinq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TreeMon.Data;
using TreeMon.Models;
using TreeMon.Models.App;
using TreeMon.Models.General;
using TreeMon.Utilites.Extensions;

namespace TreeMon.Managers.General
{
    public class StatusMessageManager : BaseManager, ICrud
    {
        public StatusMessageManager(string connectionKey, string sessionKey) : base(connectionKey, sessionKey)
        {
            Debug.Assert(!string.IsNullOrWhiteSpace(connectionKey), "StatusMessageManager CONTEXT IS NULL!");
            this._connectionKey = connectionKey;
        }

        public ServiceResult Delete(INode n, bool purge = false)
        {

            if (n == null)
                return ServiceResponse.Error("No record sent.");

            if (!this.DataAccessAuthorized(n, "delete", false)) return ServiceResponse.Error("You are not authorized this action.");

            var s = (StatusMessage)n;

            using (var context = new TreeMonDbContext(this._connectionKey))
            {
                if (purge)
                {
                    if (context.Delete<StatusMessage>(s) == 0)
                        return ServiceResponse.Error(s.Name + " failed to delete. ");
                }
                else
                {
                    s.Deleted = false;
                    if (context.Update<StatusMessage>(s) == 0)
                        return ServiceResponse.Error(s.Name + " failed to delete. ");
                }

            }
            return ServiceResponse.OK();
        }

        public List<StatusMessage> Search(string status)
        {
            if (string.IsNullOrWhiteSpace(status))
                return new List<StatusMessage>();
            using (var context = new TreeMonDbContext(this._connectionKey))
            {
                return context.GetAll<StatusMessage>()?.Where(sw => sw.Status.EqualsIgnoreCase(status)).ToList();
            }
        }

        public List<StatusMessage> GetStatusMessages(string accountUUID)
        {
            using (var context = new TreeMonDbContext(this._connectionKey))
            {

                return context.GetAll<StatusMessage>()?.Where(sw => (sw.AccountUUID == accountUUID)).OrderBy(ob => ob.Status).ToList();
            }
            ///if (!this.DataAccessAuthorized(s, "GET", false)) return ServiceResponse.Error("You are not authorized this action.");
        }


        public List<StatusMessage> GetStatusByType(string statusType,string userUUID, string accountUUID )
        {
            List<StatusMessage> status;
            using (var context = new TreeMonDbContext(this._connectionKey))
            {
                status = context.GetAll<StatusMessage>()?.Where(sw => (sw.StatusType?.EqualsIgnoreCase(statusType)??false) &&
                sw.CreatedBy == userUUID && sw.AccountUUID == accountUUID).OrderBy(ob => ob.Status).DistinctBy(sd => sd.Status)?.ToList();
            }
            ///if (!this.DataAccessAuthorized(s, "GET", false)) return ServiceResponse.Error("You are not authorized this action.");
            return status;
        }

        public INode Get(string uuid)
        {
            if (string.IsNullOrWhiteSpace(uuid))
                return null;
            using (var context = new TreeMonDbContext(this._connectionKey))
            {
                return context.GetAll<StatusMessage>()?.FirstOrDefault(sw => sw.UUID == uuid);
            }
            ///if (!this.DataAccessAuthorized(s, "GET", false)) return ServiceResponse.Error("You are not authorized this action.");
        }

        public ServiceResult Insert(INode n)
        {
            if (n == null)
                return ServiceResponse.Error("Invalid StatusMessage data.");

            if (!this.DataAccessAuthorized(n, "POST", false)) return ServiceResponse.Error("You are not authorized this action.");

            n.Initialize(this._requestingUser.UUID, this._requestingUser.AccountUUID, this._requestingUser.RoleWeight);

            var s = (StatusMessage)n;
            using (var context = new TreeMonDbContext(this._connectionKey))
            {
             
                    StatusMessage dbU = context.GetAll<StatusMessage>()?.FirstOrDefault(wu => wu.Status.EqualsIgnoreCase(s.Status) && wu.AccountUUID == s.AccountUUID);

                    if (dbU != null)
                        return ServiceResponse.Error("StatusMessage already exists.");
                 
   
                if (!this.DataAccessAuthorized(s, "POST", false)) return ServiceResponse.Error("You are not authorized this action.");

                if (context.Insert<StatusMessage>(s))
                    return ServiceResponse.OK("",s);
            }
            return ServiceResponse.Error("An error occurred inserting StatusMessage " + s.Status);
        }

        public ServiceResult Update(INode n)
        {
            if (n == null)
                return ServiceResponse.Error("Invalid StatusMessage data.");

            if (!this.DataAccessAuthorized(n, "PATCH", false)) return ServiceResponse.Error("You are not authorized this action.");

            var s = (StatusMessage)n;

            using (var context = new TreeMonDbContext(this._connectionKey))
            {

                if (context.Update<StatusMessage>(s) > 0)
                    return ServiceResponse.OK();
            }
            return ServiceResponse.Error("System error, StatusMessage was not updated.");
        }

    }
}
