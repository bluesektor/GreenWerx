// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
using Dapper;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TreeMon.Data;
using TreeMon.Data.Logging;
using TreeMon.Models;
using TreeMon.Models.App;
using TreeMon.Models.Finance;
using TreeMon.Utilites.Extensions;

namespace TreeMon.Managers.Finance
{
    public class PriceManager : BaseManager, ICrud
    {
        private readonly string _sessionKey;
        private readonly SystemLogger _logger;

        public PriceManager(string connectionKey, string sessionKey) : base(connectionKey, sessionKey)
        {
            Debug.Assert(!string.IsNullOrWhiteSpace(connectionKey), "PriceRuleManager CONTEXT IS NULL!");

            _sessionKey = sessionKey;
            this._connectionKey = connectionKey;

            _logger = new SystemLogger(connectionKey);


        }
        public ServiceResult Delete(INode n, bool purge = false) //TODO check if finance account references this currency. if so then return error.
        {
            ServiceResult res = ServiceResponse.OK();

            if (n == null)
                return ServiceResponse.Error("No record sent.");

            if (!this.DataAccessAuthorized(n, "DELETE", false)) return ServiceResponse.Error("You are not authorized this action.");

            var p = (PriceRule)n;
            try
            {
                
                using (var context = new TreeMonDbContext(this._connectionKey))
                {
                    if (purge)
                    {
                        DynamicParameters parameters = new DynamicParameters();
                        parameters.Add("@UUID", p.UUID);
                        if (context.Delete<PriceRule>("WHERE UUID=@UUID", parameters) == 0)
                            return ServiceResponse.Error(p.Name + " failed to delete. ");
                    }
                    else {
                        p.Deleted = true;
                        if (context.Update<PriceRule>(p) == 0)
                            return ServiceResponse.Error(p.Name + " failed to delete. ");
                    }
                }
                //SQLITE
                //this was the only way I could get it to delete a RolePermission without some stupid EF error.
                //object[] paramters = new object[] { rp.PermissionUUID , rp.RoleUUID ,rp.AccountUUID };
                //context.Delete<RolePermission>("WHERE PermissionUUID=? AND RoleUUID=? AND AccountUUID=?", paramters);
                //  context.Delete<RolePermission>(rp);
            }
            catch (Exception ex)
            {
                _logger.InsertError(ex.Message, "ItemManager", "DeleteItem");
                Debug.Assert(false, ex.Message);
                return ServiceResponse.Error("Exception occured while deleting this record.");
            }

            return res;
        }

        public List<PriceRule> GetAccountPriceRule(string accountUUID)
        {
            //if (!this.DataAccessAuthorized(dbP, "GET", false)) return ServiceResponse.Error("You are not authorized this action.");

            using (var context = new TreeMonDbContext(this._connectionKey))
            {
                if (string.IsNullOrWhiteSpace(accountUUID))
                    return context.GetAll<PriceRule>().ToList();

                return context.GetAll<PriceRule>().Where(pw => pw.AccountUUID == accountUUID).ToList();
            }
        }

        public INode GetBy(string uuid)
        {
            if (string.IsNullOrWhiteSpace(uuid))
                return null;
            using (var context = new TreeMonDbContext(this._connectionKey))
            {
                //if (!this.DataAccessAuthorized(dbP, "GET", false)) return ServiceResponse.Error("You are not authorized this action.");
                return context.GetAll<PriceRule>().FirstOrDefault(sw => sw.UUID == uuid);
            }
        }

        public INode Get(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return null;
            using (var context = new TreeMonDbContext(this._connectionKey))
            {
                //if (!this.DataAccessAuthorized(dbP, "GET", false)) return ServiceResponse.Error("You are not authorized this action.");
                return context.GetAll<PriceRule>().FirstOrDefault(sw => sw.Name.EqualsIgnoreCase(name));
            }
        }

        public PriceRule GetPriceRuleByCode(string PriceRuleCode)
        {
            if (string.IsNullOrWhiteSpace(PriceRuleCode))
                return null;
            using (var context = new TreeMonDbContext(this._connectionKey))
            {
                //if (!this.DataAccessAuthorized(dbP, "GET", false)) return ServiceResponse.Error("You are not authorized this action.");
                return context.GetAll<PriceRule>().FirstOrDefault(sw => sw.Code.EqualsIgnoreCase(PriceRuleCode));
            }
        }
        public List<PriceRule> GetPriceRules(string accountUUID, bool deleted = false)
        {
            using (var context = new TreeMonDbContext(this._connectionKey))
            {
                //if (!this.DataAccessAuthorized(dbP, "GET", false)) return ServiceResponse.Error("You are not authorized this action.");
                //tod check if asset class is returned if so delete the line below.
                List<PriceRule> tmp = context.GetAll<PriceRule>().Where(sw => (sw.AccountUUID == accountUUID) && sw.Deleted == deleted).OrderBy(ob => ob.Name).ToList();

                return context.GetAll<PriceRule>().Where(sw => (sw.AccountUUID == accountUUID) && sw.Deleted == deleted).OrderBy(ob => ob.Name).ToList();
            }
        }

        public List<PriceRuleLog> GetPriceRules(string trackingId, string trackingType, bool isDeleted = false)
        {
          
                using (var context = new TreeMonDbContext(this._connectionKey))
                {

                    return context.GetAll<PriceRuleLog>().Where(sw => (sw.TrackingId == trackingId && (sw.TrackingType?.EqualsIgnoreCase(trackingType) ?? false)) && sw.Deleted == isDeleted).OrderBy(ob => ob.Name).ToList();
                }
          
        }

        public List<PriceRule> GetAll()
        {
            using (var context = new TreeMonDbContext(this._connectionKey))
            {
                //if (!this.DataAccessAuthorized(dbP, "GET", false)) return ServiceResponse.Error("You are not authorized this action.");
                return context.GetAll<PriceRule>().ToList();
            }
        }

        public ServiceResult Update(INode n)
        {
            if (!this.DataAccessAuthorized(n, "PATCH", false)) return ServiceResponse.Error("You are not authorized this action.");
            var p = (PriceRule)n;
            ServiceResult res = ServiceResponse.OK();
            using (var context = new TreeMonDbContext(this._connectionKey))
            {

                if (context.Update<PriceRule>(p) == 0)
                    return ServiceResponse.Error(p.Name + " failed to update. ");
            }
            return res;

        }


        /// <summary>
        /// This was created for use in the bulk process..
        /// 
        /// </summary>
        /// <param name="p"></param>
        /// <param name="checkName">This will check the products by name to see if they exist already. If it does an error message will be returned.</param>
        /// <returns></returns>
        public ServiceResult Insert(INode n, bool validateFirst = true)
        {
            if (!this.DataAccessAuthorized(n, "POST", false)) return ServiceResponse.Error("You are not authorized this action.");

            n.Initialize(this._requestingUser.UUID, this._requestingUser.AccountUUID, this._requestingUser.RoleWeight);

            var p = (PriceRule)n;

            if (validateFirst)
            {
                PriceRule dbU = (PriceRule)Get(p.Name);

                if (dbU != null)
                    return ServiceResponse.Error("PriceRule already exists.");

                if (string.IsNullOrWhiteSpace(p.CreatedBy))
                    return ServiceResponse.Error("You must assign who the product was created by.");

                if (string.IsNullOrWhiteSpace(p.AccountUUID))
                    return ServiceResponse.Error("The account id is empty.");
            }

            if ( p.Expires == DateTime.MinValue)
                p.Expires = DateTime.UtcNow.AddYears(200);

            using (var context = new TreeMonDbContext(this._connectionKey))
            {
                if (context.Insert<PriceRule>(p))
                    return ServiceResponse.OK("", p);
            }
            return ServiceResponse.Error("An error occurred inserting product " + p.Name);
        }


      
    }
}
