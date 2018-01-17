// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
using Dapper;
using MoreLinq;
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

namespace TreeMon.Managers.Store
{
    public class ProductManager : BaseManager, ICrud
    {
        private readonly SystemLogger _logger = null;

        public ProductManager(string connectionKey, string sessionKey) : base(connectionKey, sessionKey)
        {
            Debug.Assert(!string.IsNullOrWhiteSpace(connectionKey), "ProductManager CONTEXT IS NULL!");

            
                 this._connectionKey = connectionKey;

            _logger = new SystemLogger(connectionKey);
        }

        public ServiceResult Delete(INode n, bool purge = false)
        {
            ServiceResult res = ServiceResponse.OK();

            if (n == null)
                return ServiceResponse.Error("No record sent.");

            
            if (!this.DataAccessAuthorized(n, "DELETE", false)) return ServiceResponse.Error("You are not authorized this action.");

            var p = (Product)n;

            try
            {
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@PRODUCTUUID", p.UUID);
                using (var context = new TreeMonDbContext(this._connectionKey))
                {
                    if (purge)
                    {
                        if (context.Delete<Product>("WHERE UUID=@PRODUCTUUID", parameters) == 0)
                            return ServiceResponse.Error(p.Name + " failed to delete. ");
                    }
                    else
                    {
                        p.Deleted = true;
                        if (context.Update<Product>(p) == 0)
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
                _logger.InsertError(ex.Message, "ProductManager", "DeleteProduct");
                Debug.Assert(false, ex.Message);
                return ServiceResponse.Error("Exception occured while deleting this record.");
            }

            return res;
        }

        public List<Product> GetAll(string category = "")
        {
            //if (!this.DataAccessAuthorized(dbP, "GET", false)) return ServiceResponse.Error("You are not authorized this action.");
            using (var context = new TreeMonDbContext(this._connectionKey))
            {
                if (string.IsNullOrWhiteSpace(category)) {
                    return context.GetAll<Product>()
                        .OrderBy(o => o.Name).ToList();
                }
                return context.GetAll<Product>().Where(pw => pw.CategoryUUID == category)
                                                 .OrderBy(po => po.Name).ToList();
            }
        }

        public List<Product> GetAccountProducts(string accountUUID)
        {
            //if (!this.DataAccessAuthorized(dbP, "GET", false)) return ServiceResponse.Error("You are not authorized this action.");

            using (var context = new TreeMonDbContext(this._connectionKey))
            {
                if (string.IsNullOrWhiteSpace(accountUUID))
                    return context.GetAll<Product>().ToList();

                return context.GetAll<Product>().Where(pw => pw.AccountUUID == accountUUID).ToList();
            }
        }

        /// <summary>
        /// Returns where createdby = system or user and removes duplicates that are system
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        public List<Product> GetCombinedProducts( string userUUID, string accountUUID, string category = "")
        {
            //if (!this.DataAccessAuthorized(dbP, "GET", false)) return ServiceResponse.Error("You are not authorized this action.");
            List<Product> products = new List<Product>();
            using (var context = new TreeMonDbContext(this._connectionKey))
            {
                products = context.GetAll<Product>()
                    .Where(pw => ((pw.CreatedBy == userUUID && pw.AccountUUID == accountUUID) ) && pw.Deleted == false)
                    .DistinctBy(pd => pd.CategoryUUID).ToList();//.GroupBy( pg => pg.Category ).Select(ps => ps.First()).ToList();
            }
            return products;
        }

        public List<Product> GetUserProducts(string accountUUID, string userUUID)
        {
            using (var context = new TreeMonDbContext(this._connectionKey))
            {
                //if (!this.DataAccessAuthorized(dbP, "GET", false)) return ServiceResponse.Error("You are not authorized this action.");
                if (string.IsNullOrWhiteSpace(accountUUID))
                    return context.GetAll<Product>().ToList();

                return context.GetAll<Product>().Where(pw => pw.AccountUUID == accountUUID && pw.CreatedBy == userUUID).ToList();
            }
        }

        public INode Get(string uuid)
        {
            if (string.IsNullOrWhiteSpace(uuid))
                return new Product();

            using (var context = new TreeMonDbContext(this._connectionKey))
            {
                //if (!this.DataAccessAuthorized(dbP, "GET", false)) return ServiceResponse.Error("You are not authorized this action.");
                return context.GetAll<Product>().FirstOrDefault(sw => sw.UUID == uuid);
            }
        }

        public List<Product> Search(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return null;
            using (var context = new TreeMonDbContext(this._connectionKey))
            {
                //if (!this.DataAccessAuthorized(dbP, "GET", false)) return ServiceResponse.Error("You are not authorized this action.");
                return context.GetAll<Product>().Where(sw => sw.Name.EqualsIgnoreCase(name)).ToList();
            }
        }

        public List<Product> GetProducts(string accountUUID, bool deleted = false)
        {
            using (var context = new TreeMonDbContext(this._connectionKey))
            {
                //if (!this.DataAccessAuthorized(dbP, "GET", false)) return ServiceResponse.Error("You are not authorized this action.");
                return context.GetAll<Product>().Where(sw => (sw.AccountUUID == accountUUID) && sw.Deleted == deleted).OrderBy(ob => ob.Name).ToList();
            }
        }

        /// <summary>
        /// Note: this is used in the admin section and only requires
        /// the product uuid and the new category assigned to the category property.
        /// </summary>
        /// <param name="products"></param>
        /// <returns></returns>
        public ServiceResult Update(List<Product> products)
        {
            ServiceResult res = ServiceResponse.OK("Products moved to new categories.");
            using (var context = new TreeMonDbContext(this._connectionKey))
            {
                foreach (Product p in products)
                {
                    string newCat = p.CategoryUUID;
                    //Get the product from the database because when we update the framework will update all fields, and the products
                    //passed in may not be complete.
                    //
                    var dbP = context.GetAll<Product>().FirstOrDefault(pw => pw.UUID == p.UUID);
                    if (dbP == null)
                    {
                        if (res.Code == 200)
                            res = ServiceResponse.Error(p.UUID + " product not found.<br/>");
                        else
                            res.Message += p.UUID + " product not found.<br/>";

                        continue;
                    }

                    if (!this.DataAccessAuthorized(dbP, "PATCH", false)) return ServiceResponse.Error("You are not authorized this action.");

                    dbP.CategoryUUID = newCat;

                    if (context.Update<Product>(dbP) == 0)
                    {
                        if (res.Code == 200)
                            res = ServiceResponse.Error(dbP.Name + " failed to update. " + dbP.UUID + "<br/>");
                        else
                            res.Message += dbP.Name + " failed to update. " + dbP.UUID + "<br/>";
                    }
                }
            }

            return res;
        }
   
        public ServiceResult Update(INode n)
        {
            if (n == null)
                return ServiceResponse.Error("No record sent.");

            if (!this.DataAccessAuthorized(n, "PATCH", false)) return ServiceResponse.Error("You are not authorized this action.");

            var p = (Product)n;

            ServiceResult res = ServiceResponse.OK();
            using (var context = new TreeMonDbContext(this._connectionKey))
            {

                if (context.Update<Product>(p) == 0)
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
            if (n == null)
                return ServiceResponse.Error("No product to insert.");

            if (!this.DataAccessAuthorized(n, "POST", false)) return ServiceResponse.Error("You are not authorized this action.");

            n.Initialize(this._requestingUser.UUID, this._requestingUser.AccountUUID, this._requestingUser.RoleWeight);

            var p = (Product)n;

            if (validateFirst)
            {
                List<Product> dbU = Search(p.Name);

                if (dbU != null || dbU.Count > 0 )
                    return ServiceResponse.Error("Product already exists.");

                if(string.IsNullOrWhiteSpace(p.CreatedBy))
                    return ServiceResponse.Error("You must assign who the product was created by.");

                if (string.IsNullOrWhiteSpace(p.AccountUUID))
                    return ServiceResponse.Error("The account id is empty.");
            }
     
            p.Expires = null;  
 
            using (var context = new TreeMonDbContext(this._connectionKey))
            {
                if (context.Insert<Product>(p))
                    return ServiceResponse.OK("",p);
            }
            return ServiceResponse.Error("An error occurred inserting product " + p.Name);
        }
    }
}
