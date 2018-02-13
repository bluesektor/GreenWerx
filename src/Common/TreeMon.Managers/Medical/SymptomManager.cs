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
using TreeMon.Models.Medical;
using TreeMon.Utilites.Extensions;

namespace TreeMon.Managers.Medical
{
    public class SymptomManager : BaseManager, ICrud
    {

       readonly SystemLogger _logger = null;

        public SymptomManager(string connectionKey, string sessionKey) : base(connectionKey, sessionKey)
        {
            Debug.Assert(!string.IsNullOrWhiteSpace(connectionKey), "SymptomManager CONTEXT IS NULL!");
            
                 this._connectionKey = connectionKey;

            _logger = new SystemLogger(this._connectionKey);
        }

        #region Symptoms

        public ServiceResult Delete(INode n, bool purge = false)
        {
            ServiceResult res = ServiceResponse.OK();
            if (n == null)
                return ServiceResponse.Error("No record sent.");

            if (!this.DataAccessAuthorized(n, "DELETE", false)) return ServiceResponse.Error("You are not authorized this action.");

            var s = (Symptom)n;

            if (purge)
            {
                using (var context = new TreeMonDbContext(this._connectionKey))
                {
                    if (context.Delete<Symptom>(s) == 0)
                        return ServiceResponse.Error(s.Name + " failed to delete. ");
                }
            }

            //get the Symptom from the table with all the data so when its updated it still contains the same data.
            s = (Symptom)this.Get(s.UUID);
            if (s == null)
                return ServiceResponse.Error("Symptom not found");

            s.Deleted = true;
            using (var context = new TreeMonDbContext(this._connectionKey))
            {
                if (context.Update<Symptom>(s) == 0)
                    return ServiceResponse.Error(s.Name + " failed to delete. ");
            }
            return res;
        }

        public List<Symptom> Search(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return new List<Symptom>();
            using (var context = new TreeMonDbContext(this._connectionKey))
            {
                return context.GetAll<Symptom>().Where(sw => sw.Name.EqualsIgnoreCase(name)).ToList();
            }
            ///if (!this.DataAccessAuthorized(s, "GET", false)) return ServiceResponse.Error("You are not authorized this action.");
        }

        public List<Symptom> GetSymptoms(string accountUUID, bool deleted = false)
        {
            using (var context = new TreeMonDbContext(this._connectionKey))
            {
                return context.GetAll<Symptom>().Where(sw => (sw.AccountUUID == accountUUID) && sw.Deleted == deleted).OrderBy(ob => ob.Name).ToList();
            }
            ///if (!this.DataAccessAuthorized(s, "GET", false)) return ServiceResponse.Error("You are not authorized this action.");
        }


        public INode Get(string uuid)
        {
            if (string.IsNullOrWhiteSpace(uuid))
                return null;
            using (var context = new TreeMonDbContext(this._connectionKey))
            {
                return context.GetAll<Symptom>().FirstOrDefault(sw => sw.UUID == uuid);
            }
            ///if (!this.DataAccessAuthorized(s, "GET", false)) return ServiceResponse.Error("You are not authorized this action.");
        }

        public ServiceResult Insert(INode n)
        {
            if (!this.DataAccessAuthorized(n, "POST", false)) return ServiceResponse.Error("You are not authorized this action.");

            n.Initialize(this._requestingUser.UUID, this._requestingUser.AccountUUID, this._requestingUser.RoleWeight);

            var s = (Symptom)n;

            using (var context = new TreeMonDbContext(this._connectionKey))
            {
                    Symptom dbU = context.GetAll<Symptom>().FirstOrDefault(wu => wu.Name.EqualsIgnoreCase(s.Name) && wu.AccountUUID == s.AccountUUID);

                    if (dbU != null)
                        return ServiceResponse.Error("Symptom already exists.");
                
   
                if (context.Insert<Symptom>(s))
                    return ServiceResponse.OK("",s);
            }
            return ServiceResponse.Error("An error occurred inserting Symptom " + s.Name);
        }

        public ServiceResult Update(INode n)
        {
            if (n == null)
                return ServiceResponse.Error("Invalid Symptom data.");

            if (!this.DataAccessAuthorized(n, "PATCH", false)) return ServiceResponse.Error("You are not authorized this action.");

            var s = (Symptom)n;

            using (var context = new TreeMonDbContext(this._connectionKey))
            {
                if (context.Update<Symptom>(s) > 0)
                    return ServiceResponse.OK();
            }
            return ServiceResponse.Error("System error, Symptom was not updated.");
        }

        #endregion

        #region SymptomsLog todo refactor into icrud class


        public ServiceResult Delete(SymptomLog s, bool purge = false)
        {
            ServiceResult res = ServiceResponse.OK();

            if (s == null)
                return ServiceResponse.Error("No record sent.");

            if (!this.DataAccessAuthorized(s, "DELETE", false)) return ServiceResponse.Error("You are not authorized this action.");

            if (purge)
            {
                using (var context = new TreeMonDbContext(this._connectionKey))
                {
                    if( context.Delete<SymptomLog>(s)== 0 )
                        return ServiceResponse.Error(s.Name + " failed to delete. ");
                }
            }

            //get the SymptomLog from the table with all the data so when its updated it still contains the same data.
            s = this.GetSymptomLogBy(s.UUID);
            if (s == null)
                return ServiceResponse.Error("Symptom not found");

            s.Deleted = true;
            using (var context = new TreeMonDbContext(this._connectionKey))
            {
                if(context.Update<SymptomLog>(s)== 0)
                    return ServiceResponse.Error(s.Name + " failed to delete. ");
            }
            return res;
        }

        public SymptomLog GetSymptomLog(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return null;
            using (var context = new TreeMonDbContext(this._connectionKey))
            {
                return context.GetAll<SymptomLog>().FirstOrDefault(sw => sw.Name.EqualsIgnoreCase(name));
            }
            ///if (!this.DataAccessAuthorized(s, "GET", false)) return ServiceResponse.Error("You are not authorized this action.");
        }

        public List<SymptomLog> GetSymptomsLog(string accountUUID, bool deleted = false)
        {
            using (var context = new TreeMonDbContext(this._connectionKey))
            {
                return context.GetAll<SymptomLog>().Where(sw => (sw.AccountUUID == accountUUID) &&
                (string.IsNullOrWhiteSpace(sw.UUParentID) == true || sw.UUParentID == null) &&
                 sw.Deleted == deleted).OrderBy(ob => ob.Name).ToList();
            }
            ///if (!this.DataAccessAuthorized(s, "GET", false)) return ServiceResponse.Error("You are not authorized this action.");
        }

        public List<SymptomLog> GetSymptomsLog(string parentUUID, string accountUUID, bool deleted = false)
        {
            using (var context = new TreeMonDbContext(this._connectionKey))
            {
                if (string.IsNullOrWhiteSpace(parentUUID) || parentUUID == "0")
                    return context.GetAll<SymptomLog>().Where(sw => (sw.UUParentID == "" || sw.UUParentID == null) && sw.AccountUUID == accountUUID && sw.Deleted == deleted).OrderBy(ob => ob.Name).ToList();

                return context.GetAll<SymptomLog>().Where(sw => sw.UUParentID == parentUUID && sw.AccountUUID == accountUUID && sw.Deleted == deleted).OrderBy(ob => ob.Name).ToList();
            }
            ///if (!this.DataAccessAuthorized(s, "GET", false)) return ServiceResponse.Error("You are not authorized this action.");
        }

        public List<SymptomLog> GetSymptomsByDose(string doseUUID, string parentUUID, string accountUUID, bool deleted = false)
        {
            using (var context = new TreeMonDbContext(this._connectionKey))
            {
                if (string.IsNullOrWhiteSpace(parentUUID) || parentUUID == "0")
                    return context.GetAll<SymptomLog>().Where(sw => sw.DoseUUID == doseUUID && (sw.UUParentID == "" || sw.UUParentID == null) && sw.AccountUUID == accountUUID && sw.Deleted == deleted).OrderBy(ob => ob.Name).ToList();

                return context.GetAll<SymptomLog>().Where(sw => sw.DoseUUID == doseUUID && sw.UUParentID == parentUUID && sw.AccountUUID == accountUUID && sw.Deleted == deleted).OrderBy(ob => ob.Name).ToList();
            }
            ///if (!this.DataAccessAuthorized(s, "POST", false)) return ServiceResponse.Error("You are not authorized this action.");
        }

        public SymptomLog GetSymptomLogBy(string uuid)
        {
            if (string.IsNullOrWhiteSpace(uuid))
                return null;
            using (var context = new TreeMonDbContext(this._connectionKey))
            {
                return context.GetAll<SymptomLog>().FirstOrDefault(sw => sw.UUID == uuid);
            }
            ///if (!this.DataAccessAuthorized(s, "GET", false)) return ServiceResponse.Error("You are not authorized this action.");
        }

        public ServiceResult Insert(SymptomLog s)
        {
            using (var context = new TreeMonDbContext(this._connectionKey))
            {
               
                    SymptomLog dbU = context.GetAll<SymptomLog>().FirstOrDefault(wu => wu.Name.EqualsIgnoreCase(s.Name) && wu.AccountUUID == s.AccountUUID);

                    if (dbU != null)
                        return ServiceResponse.Error("SymptomLog already exists.");
               
                s.UUID = Guid.NewGuid().ToString("N");
                s.UUIDType = "SymptomLog";

                if (!this.DataAccessAuthorized(s, "POST", false)) return ServiceResponse.Error("You are not authorized this action.");

                if (context.Insert<SymptomLog>(s))
                    return ServiceResponse.OK("",s);
            }
            return ServiceResponse.Error("An error occurred inserting SymptomLog " + s.Name);
        }

        public ServiceResult Update(SymptomLog s)
        {
            if (s == null)
                return ServiceResponse.Error("Invalid SymptomLog data.");

            if (!this.DataAccessAuthorized(s, "PATCH", false)) return ServiceResponse.Error("You are not authorized this action.");

            using (var context = new TreeMonDbContext(this._connectionKey))
            {
                if (context.Update<SymptomLog>(s) > 0)
                    return ServiceResponse.OK();
            }
            return ServiceResponse.Error("System error, SymptomLog was not updated.");
        }
        #endregion
    }
}
