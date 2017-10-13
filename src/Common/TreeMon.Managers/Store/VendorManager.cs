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
using TreeMon.Models.Membership;
using TreeMon.Models.Store;
using TreeMon.Utilites.Extensions;

namespace TreeMon.Managers.Store
{
    public class VendorManager : BaseManager, ICrud
    {
        private SystemLogger _logger = null;

        public VendorManager(string connectionKey, string sessionKey) : base(connectionKey, sessionKey)
        {
            Debug.Assert(!string.IsNullOrWhiteSpace(connectionKey), "VendorManager CONTEXT IS NULL!");
            this._connectionKey = connectionKey;
            _logger = new SystemLogger(connectionKey);
        }


        public ServiceResult Delete(INode n, bool purge = false)
        {
            if (n == null)
                return ServiceResponse.Error("No record sent.");

            if (!this.DataAccessAuthorized(n, "DELETE", false)) return ServiceResponse.Error("You are not authorized this action.");

            var p = (Vendor)n;

            try
            {
              
                using (var context = new TreeMonDbContext(this._connectionKey))
                {
                    if (purge)
                    {
                        DynamicParameters parameters = new DynamicParameters();
                        parameters.Add("@VendorUUID", p.UUID);
                        if (context.Delete<Vendor>("WHERE UUID=@VendorUUID", parameters) > 0)
                        {
                            DynamicParameters credParams = new DynamicParameters();
                            credParams.Add("@UUID", p.UUID);
                            credParams.Add("@TYPE", "Vendor");
                            context.Delete<Credential>("WHERE RecipientUUID=@UUID AND RecipientType=@TYPE", credParams);

                            return ServiceResponse.OK();
                        }
                    }
                    else
                    {
                            p.Deleted = true;
                        if (context.Update<Vendor>(p) > 0)
                            return ServiceResponse.OK();
                    }
                }
               

                return ServiceResponse.Error("No records deleted.");
                //SQLITE
                //this was the only way I could get it to delete a RolePermission without some stupid EF error.
                //object[] paramters = new object[] { rp.PermissionUUID , rp.RoleUUID ,rp.AccountUUID };
                //context.Delete<RolePermission>("WHERE PermissionUUID=? AND RoleUUID=? AND AccountUUID=?", paramters);
                //  context.Delete<RolePermission>(rp);
            }
            catch (Exception ex)
            {
                _logger.InsertError(ex.Message, "VendorManager", "DeleteVendor:" + p.UUID);
                Debug.Assert(false, ex.Message);
                return ServiceResponse.Error("Exception occured while deleting this record.");
            }
        }

        public List<Vendor> GetAccountVendors(string accountUUID)
        {
            using (var context = new TreeMonDbContext(this._connectionKey))
            {
                //if (!this.DataAccessAuthorized(p, "GET", false))return ServiceResponse.Error("You are not authorized this action.");

                if (string.IsNullOrWhiteSpace(accountUUID))
                    return context.GetAll<Vendor>().ToList();

                return context.GetAll<Vendor>().Where(pw => pw.AccountUUID == accountUUID).ToList();
            }
        }

        public List<Vendor> GetUserVendors(string accountUUID, string userUUID)
        {
            using (var context = new TreeMonDbContext(this._connectionKey))
            {
                //if (!this.DataAccessAuthorized(p, "GET", false))return ServiceResponse.Error("You are not authorized this action.");
                if (string.IsNullOrWhiteSpace(accountUUID))
                    return context.GetAll<Vendor>().ToList();

                return context.GetAll<Vendor>().Where(pw => pw.AccountUUID == accountUUID && pw.CreatedBy == userUUID).ToList();
            }
        }

        public INode GetBy(string uuid)
        {
            if (string.IsNullOrWhiteSpace(uuid))
                return null;
            using (var context = new TreeMonDbContext(this._connectionKey))
            {
                //if (!this.DataAccessAuthorized(p, "GET", false))return ServiceResponse.Error("You are not authorized this action.");
                return context.GetAll<Vendor>().FirstOrDefault(sw => sw.UUID == uuid);
            }
        }

        public INode Get( string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return null;
            using (var context = new TreeMonDbContext(this._connectionKey))
            {
                //if (!this.DataAccessAuthorized(p, "GET", false))return ServiceResponse.Error("You are not authorized this action.");

                return context.GetAll<Vendor>().FirstOrDefault(sw => sw.Name.EqualsIgnoreCase(name));
            }
        }

      

        public List<Vendor> GetVendors(string accountUUID , bool deleted = false ,bool includeSystemAccount = false)
        {
            using (var context = new TreeMonDbContext(this._connectionKey))
            {
                //if (!this.DataAccessAuthorized(p, "GET", false))
                //    return ServiceResponse.Error("You are not authorized this action.");
                if (includeSystemAccount)
                {
                    return context.GetAll<Vendor>().Where(sw => (sw.AccountUUID == accountUUID ) && sw.Deleted == deleted).GroupBy(x => x.Name).Select(group => group.First()).OrderBy(ob => ob.Name).ToList();
                }

                return context.GetAll<Vendor>().Where(sw => (sw.AccountUUID == accountUUID) && sw.Deleted == deleted).OrderBy(ob => ob.Name).ToList();
            }
        }

        public ServiceResult Update(INode n)
        {
            if (n == null)
                return ServiceResponse.Error("No record sent.");

            var p = (Vendor)n;

            if (!this.DataAccessAuthorized(p, "PATCH", false))
                return ServiceResponse.Error("You are not authorized this action.");

            ServiceResult res = ServiceResponse.OK();
            using (var context = new TreeMonDbContext(this._connectionKey))
            {
                if (context.Update<Vendor>(p) == 0)
                    return ServiceResponse.Error(p.Name + " failed to update. ");
            }
            return res;
        }


        /// <summary>
              /// This was created for use in the bulk process..
              /// 
              /// </summary>
              /// <param name="p"></param>
              /// <param name="checkName">This will check the Vendors by name to see if they exist already. If it does an error message will be returned.</param>
              /// <returns></returns>
        public ServiceResult Insert(INode n,bool validateFirst = true)
        {
            if (n == null)
                return ServiceResponse.Error("No record sent.");


            if (!this.DataAccessAuthorized(n, "POST", false))
                return ServiceResponse.Error("You are not authorized this action.");

            n.Initialize(this._requestingUser.UUID, this._requestingUser.AccountUUID, this._requestingUser.RoleWeight);

            var p = (Vendor)n;

            if (validateFirst)
            {
                Vendor dbU = (Vendor)Get(p.Name);


                if(string.IsNullOrWhiteSpace(p.CreatedBy))
                    return ServiceResponse.Error("You must assign who the Vendor was created by.");

                if (string.IsNullOrWhiteSpace(p.AccountUUID))
                    return ServiceResponse.Error("The account id is empty.");
            }
    
            using (var context = new TreeMonDbContext(this._connectionKey))
            {
                if (context.Insert<Vendor>(p))
                    return ServiceResponse.OK("",p);
            }
            return ServiceResponse.Error("An error occurred inserting Vendor " + p.Name);
        }
              
    }
}
