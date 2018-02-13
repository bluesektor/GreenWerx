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
    public class FinanceAccountManager : BaseManager, ICrud
    {
        private readonly string _sessionKey;
        private readonly SystemLogger _logger;

        public FinanceAccountManager(string connectionKey, string sessionKey) : base(connectionKey, sessionKey)
        {
            Debug.Assert(!string.IsNullOrWhiteSpace(connectionKey), "FinanceAccountManager CONTEXT IS NULL!");

            _sessionKey = sessionKey;
            this._connectionKey = connectionKey;

            _logger = new SystemLogger(connectionKey);


        }
        public FinanceAccount GetSiteLedger(bool mustBeUnused, string accountUUID)
        {
            using (var context = new TreeMonDbContext(this._connectionKey))
            {

                if (mustBeUnused)
                    return context.GetAll<FinanceAccount>().FirstOrDefault(l => l.SourceUUID == -1 && l.AccountUUID == accountUUID);

                return context.GetAll<FinanceAccount>().FirstOrDefault(l => l.AccountUUID == accountUUID);
            }
        }

        public ServiceResult Delete(INode n, bool purge = false) //TODO check if finance account references this currency. if so then return error.
        {
            ServiceResult res = ServiceResponse.OK();

            if (n == null)
                return ServiceResponse.Error("No record sent.");

            if (!this.DataAccessAuthorized(n, "DELETE", false)) return ServiceResponse.Error("You are not authorized this action.");

            var p = (FinanceAccount)n;

            try
            {
             
                using (var context = new TreeMonDbContext(this._connectionKey))
                {
                    if (purge)
                    {
                        DynamicParameters parameters = new DynamicParameters();
                        parameters.Add("@UUID", p.UUID);
                        if (context.Delete<FinanceAccount>("WHERE UUID=@UUID", parameters) == 0)
                            return ServiceResponse.Error(p.Name + " failed to delete. ");
                    }
                    else
                    {
                        p.Deleted = true;
                        if (context.Update<FinanceAccount>(p) == 0)
                            return ServiceResponse.Error(p.Name + " failed to delete. ");
                    }
                }
                ////SQLITE
                ////this was the only way I could get it to delete a RolePermission without some stupid EF error.
                ////object[] paramters = new object[] { rp.PermissionUUID , rp.RoleUUID ,rp.AccountUUID };
                ////context.Delete<RolePermission>("WHERE PermissionUUID=? AND RoleUUID=? AND AccountUUID=?", paramters);
                ////  context.Delete<RolePermission>(rp);
            }
            catch (Exception ex)
            {
                _logger.InsertError(ex.Message, "ItemManager", "DeleteItem");
                Debug.Assert(false, ex.Message);
                return ServiceResponse.Error("Exception occured while deleting this record.");
            }

            return res;
        }

        public List<FinanceAccount> GetAccountFinanceAccount(string accountUUID)
        {
            ///if (!this.DataAccessAuthorized(dbP, "GET", false)) return ServiceResponse.Error("You are not authorized this action.");

            using (var context = new TreeMonDbContext(this._connectionKey))
            {
                if (string.IsNullOrWhiteSpace(accountUUID))
                    return context.GetAll<FinanceAccount>().ToList();

                return context.GetAll<FinanceAccount>().Where(pw => pw.AccountUUID == accountUUID).ToList();
            }
        }

        public INode Get( string uuid)
        {
            if (string.IsNullOrWhiteSpace(uuid))
                return null;
            using (var context = new TreeMonDbContext(this._connectionKey))
            {
                ////if (!this.DataAccessAuthorized(dbP, "GET", false)) return ServiceResponse.Error("You are not authorized this action.");
                return context.GetAll<FinanceAccount>().FirstOrDefault(sw => sw.UUID == uuid);
            }
        }

        public List<FinanceAccount> Search(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return new List<FinanceAccount>();

            using (var context = new TreeMonDbContext(this._connectionKey))
            {
                ///if (!this.DataAccessAuthorized(dbP, "GET", false)) return ServiceResponse.Error("You are not authorized this action.");
                return context.GetAll<FinanceAccount>().Where(sw => sw.Name.EqualsIgnoreCase(name)).ToList();
            }
        }

        public List<FinanceAccount> GetFinanceAccounts(string accountUUID, bool deleted = false)
        {
            using (var context = new TreeMonDbContext(this._connectionKey))
            {
                ///if (!this.DataAccessAuthorized(dbP, "GET", false)) return ServiceResponse.Error("You are not authorized this action.");

                List<FinanceAccount> tmp = context.GetAll<FinanceAccount>().Where(sw => (sw.AccountUUID == accountUUID) && sw.Deleted == deleted).OrderBy(ob => ob.Name).ToList();

               tmp.ForEach(x =>
               {
                   x.CurrencyName = context.GetAll<Currency>().FirstOrDefault(w => w.UUID == x.CurrencyUUID)?.Name;
               });
                return tmp;
            }
        }

        public List<FinanceAccount> GetPaymentOptions(string accountUUID, bool deleted = false)
        {
            using (var context = new TreeMonDbContext(this._connectionKey))
            {
                ///if (!this.DataAccessAuthorized(dbP, "GET", false)) return ServiceResponse.Error("You are not authorized this action.");

                List<FinanceAccount> tmp = context.GetAll<FinanceAccount>()
                    .Where(sw => (sw.AccountUUID == accountUUID) && sw.Deleted == deleted && sw.LocationType.EqualsIgnoreCase("ONLINE STORE") && sw.Active == true)
                    .OrderBy(ob => ob.Name)
                    .Select( s => { s.Password = ""; s.AccountNumber = ""; return s; })
                    .ToList();
                return tmp;
            }
        }

        public List<FinanceAccount> GetAll()
        {
            using (var context = new TreeMonDbContext(this._connectionKey))
            {
                ///if (!this.DataAccessAuthorized(dbP, "GET", false)) return ServiceResponse.Error("You are not authorized this action.");
                return context.GetAll<FinanceAccount>().ToList();
            }
        }

        public ServiceResult Update(INode n)
        {
            if (!this.DataAccessAuthorized(n, "PATCH", false)) return ServiceResponse.Error("You are not authorized this action.");

            var p = (FinanceAccount)n;

            ServiceResult res = ServiceResponse.OK();
            using (var context = new TreeMonDbContext(this._connectionKey))
            {

                if (context.Update<FinanceAccount>(p) == 0)
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
        public ServiceResult Insert(INode n)
        {
            if (!this.DataAccessAuthorized(n, "POST", false)) return ServiceResponse.Error("You are not authorized this action.");

            n.Initialize(this._requestingUser.UUID, this._requestingUser.AccountUUID, this._requestingUser.RoleWeight);

            var p = (FinanceAccount)n;

      
                if (string.IsNullOrWhiteSpace(p.CreatedBy))
                    return ServiceResponse.Error("You must assign who the product was created by.");

                if (string.IsNullOrWhiteSpace(p.AccountUUID))
                    return ServiceResponse.Error("The account id is empty.");
           

            using (var context = new TreeMonDbContext(this._connectionKey))
            {
                if (context.Insert<FinanceAccount>(p))
                    return ServiceResponse.OK("", p);
            }
            return ServiceResponse.Error("An error occurred inserting product " + p.Name);
        }
    }
}
