// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TreeMon.Data;
using TreeMon.Data.Logging;
using TreeMon.Data.Logging.Models;
using TreeMon.Models;
using TreeMon.Models.App;
using TreeMon.Models.Store;
using TreeMon.Utilites.Extensions;

namespace TreeMon.Managers.Store
{
    public class OrderManager: BaseManager,ICrud
    {
        private readonly  string _sessionKey = string.Empty;
        private readonly SystemLogger _logger = null;

        public OrderManager(string connectionKey, string sessionKey) : base(connectionKey, sessionKey)
        {
            Debug.Assert(!string.IsNullOrWhiteSpace(connectionKey), "OrderManager CONTEXT IS NULL!");

            _sessionKey = sessionKey;
            this._connectionKey = connectionKey;
            _logger = new SystemLogger(connectionKey);
        }

        public ServiceResult Delete(INode n, bool purge = false)
        {
            if (n == null)
                return ServiceResponse.Error("Invalid Order data.");


            if (!this.DataAccessAuthorized(n, "DELETE", false)) return ServiceResponse.Error("You are not authorized this action.");

            var s = (Order)n;

            using (var context = new TreeMonDbContext(this._connectionKey))
            {
                if (purge)
                {
                    if (context.Delete<Order>(s) > 0)
                        return ServiceResponse.OK();
                    else
                        return ServiceResponse.Error("Failed to delete the order.");
                }

                //get the Order from the table with all the data so when its updated it still contains the same data.
                s = (Order)this.Get(s.UUID);
                if (s == null)
                    return ServiceResponse.Error("Could not find order.");

                s.Deleted = true;
                if (context.Update<Order>(s) > 0)
                    return ServiceResponse.OK();
                else
                    return ServiceResponse.Error("Failed to delete the order.");
            }
        }

        public List<Order> Search(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return new List<Order>();

            ///if (!this.DataAccessAuthorized(s, "GET", false)) return ServiceResponse.Error("You are not authorized this action.");

            using (var context = new TreeMonDbContext(this._connectionKey))
            {
                return context.GetAll<Order>().Where(w => w.Name.EqualsIgnoreCase(name) && w.AccountUUID == _requestingUser?.AccountUUID).ToList();
            }
        }

        public List<Order> GetOrders(string accountUUID, bool deleted = false)
        {
            ///if (!this.DataAccessAuthorized(s, "GET", false)) return ServiceResponse.Error("You are not authorized this action.");
            using (var context = new TreeMonDbContext(this._connectionKey))
            {
                return context.GetAll<Order>().Where(sw => (sw.AccountUUID == accountUUID) && sw.Deleted == deleted).OrderBy(ob => ob.Name).ToList();
            }
        }


        public INode Get(string uuid)
        {
            if (string.IsNullOrWhiteSpace(uuid))
                return null;

            ///if (!this.DataAccessAuthorized(s, "GET", false)) return ServiceResponse.Error("You are not authorized this action.");
            using (var context = new TreeMonDbContext(this._connectionKey))
            {
                return context.GetAll<Order>().FirstOrDefault(sw => sw.UUID == uuid);
            }
        }

        public ServiceResult Insert(INode n)
        {
            if (n == null)
                return ServiceResponse.Error("No record sent.");

            n.Initialize(this._requestingUser.UUID, this._requestingUser.AccountUUID, this._requestingUser.RoleWeight);

            if ( n.AccountUUID == SystemFlag.Default.Account)
                n.AccountUUID = this._requestingUser?.AccountUUID;

            if (!this.DataAccessAuthorized(n, "POST", false)) return ServiceResponse.Error("You are not authorized this action.");
           
            var s = (Order)n;

            using (var context = new TreeMonDbContext(this._connectionKey))
            {
               
                    Order dbU = context.GetAll<Order>().FirstOrDefault(wu => wu.Name.EqualsIgnoreCase(s.Name));

                    if (dbU != null)
                        return ServiceResponse.Error("Order already exists.");
                
    
                if (context.Insert<Order>(s))
                    return ServiceResponse.OK("",s);
            }
            return ServiceResponse.Error("An error occurred inserting Order " + s.Name);
        }

        public ServiceResult Update(INode n)
        {
            if (n == null)
                return ServiceResponse.Error("Invalid Order data.");

            var s = (Order)n;

            if (!this.DataAccessAuthorized(s, "UPDATE", false)) return ServiceResponse.Error("You are not authorized this action.");


            using (var context = new TreeMonDbContext(this._connectionKey))
            {
                if (context.Update<Order>(s) > 0)
                    return ServiceResponse.OK();
            }
            return ServiceResponse.Error("System error, Order was not updated.");
        }
    }
}
