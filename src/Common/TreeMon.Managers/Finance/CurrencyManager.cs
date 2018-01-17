// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
using Dapper;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TreeMon.Data;
using TreeMon.Data.Logging;
using TreeMon.Data.Logging.Models;
using TreeMon.Models;
using TreeMon.Models.App;
using TreeMon.Models.Finance;
using TreeMon.Utilites.Extensions;

namespace TreeMon.Managers.Finance
{
    public class CurrencyManager : BaseManager, ICrud
    {
        private readonly SystemLogger _logger;

        public CurrencyManager(string connectionKey, string sessionKey) : base(connectionKey, sessionKey)
        {
            Debug.Assert(!string.IsNullOrWhiteSpace(connectionKey), "InventoryManager CONTEXT IS NULL!");


            this._connectionKey = connectionKey;

            _logger = new SystemLogger(connectionKey);

  
        }

        public ServiceResult Delete(INode n, bool purge = false) //TODO check if finance account references this currency. if so then return error.
        {
            ServiceResult res = ServiceResponse.OK();

            if (n == null)
                return ServiceResponse.Error("No record sent.");

            if (!this.DataAccessAuthorized(n, "DELETE", false)) return ServiceResponse.Error("You are not authorized this action.");

            var p = (Currency)n;

            try
            {
                // List<SqlParameter> parameters = new List<SqlParameter>();
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@UUID", p.UUID);
                using (var context = new TreeMonDbContext(this._connectionKey))
                {
                    if (context.Delete<Currency>("WHERE UUID=@UUID", parameters) == 0)
                        return ServiceResponse.Error(p.Name + " failed to delete. ");
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

        public List<Currency> GetAccountCurrency(string accountUUID)
        {
            //if (!this.DataAccessAuthorized(dbP, "GET", false)) return ServiceResponse.Error("You are not authorized this action.");

            using (var context = new TreeMonDbContext(this._connectionKey))
            {
                if (string.IsNullOrWhiteSpace(accountUUID))
                    return context.GetAll<Currency>().ToList();

                return context.GetAll<Currency>().Where(pw => pw.AccountUUID == accountUUID).ToList();
            }
        }

        public INode Get(string uuid)
        {
            if (string.IsNullOrWhiteSpace(uuid))
                return null;
            using (var context = new TreeMonDbContext(this._connectionKey))
            {
                //if (!this.DataAccessAuthorized(dbP, "GET", false)) return ServiceResponse.Error("You are not authorized this action.");
                return context.GetAll<Currency>().FirstOrDefault(sw => sw.UUID == uuid);
            }
        }

        public List<Currency> Search(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return null;
            using (var context = new TreeMonDbContext(this._connectionKey))
            {
                //if (!this.DataAccessAuthorized(dbP, "GET", false)) return ServiceResponse.Error("You are not authorized this action.");
                return context.GetAll<Currency>().Where(sw => (sw.Name?.Trim()?.EqualsIgnoreCase(name?.Trim()) ?? false)).ToList();
            }
        }

        public List<Currency> GetCurrencies(string accountUUID, bool deleted = false, bool inlcudeSystemDefault = false )
        {
            using (var context = new TreeMonDbContext(this._connectionKey))
            {
                //if (!this.DataAccessAuthorized(dbP, "GET", false)) return ServiceResponse.Error("You are not authorized this action.");
                //tod check if asset class is returned if so delete the line below.

                if(inlcudeSystemDefault)
                    return context.GetAll<Currency>().Where(sw => (sw.AccountUUID == accountUUID || sw.AccountUUID == SystemFlag.Default.Account) && sw.Deleted == deleted).OrderBy(ob => ob.Name).ToList();

                return context.GetAll<Currency>().Where(sw => (sw.AccountUUID == accountUUID) && sw.Deleted == deleted).OrderBy(ob => ob.Name).ToList();
            }
        }

        public List<Currency> GetAll()
        {
            using (var context = new TreeMonDbContext(this._connectionKey))
            {
                //if (!this.DataAccessAuthorized(dbP, "GET", false)) return ServiceResponse.Error("You are not authorized this action.");
                return context.GetAll<Currency>().ToList();
            }
        }

        public ServiceResult Update(INode n)
        {
            if (!this.DataAccessAuthorized(n, "PATCH", false)) return ServiceResponse.Error("You are not authorized this action.");

            var p = (Currency)n;

            ServiceResult res = ServiceResponse.OK();
            using (var context = new TreeMonDbContext(this._connectionKey))
            {

                if (context.Update<Currency>(p) == 0)
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

            var p = (Currency)n;

            if (validateFirst)
            {
                //Currency dbU = (Currency) Get(p.Name);

                //if (dbU != null)
                //    return ServiceResponse.Error("Currency already exists.");

                if (string.IsNullOrWhiteSpace(p.CreatedBy))
                    return ServiceResponse.Error("You must assign who the product was created by.");

                if (string.IsNullOrWhiteSpace(p.AccountUUID))
                    return ServiceResponse.Error("The account id is empty.");
            }

            using (var context = new TreeMonDbContext(this._connectionKey))
            {
                if (context.Insert<Currency>(p))
                    return ServiceResponse.OK("", p);
            }
            return ServiceResponse.Error("An error occurred inserting product " + p.Name);
        }
    }
}
