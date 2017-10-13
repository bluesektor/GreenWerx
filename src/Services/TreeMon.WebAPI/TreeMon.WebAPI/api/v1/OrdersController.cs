// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using TreeMon.Data.Logging.Models;
using TreeMon.Managers.Store;
using TreeMon.Models.App;
using TreeMon.Models.Datasets;
using TreeMon.Models.Store;
using TreeMon.Utilites.Extensions;
using TreeMon.Web;
using TreeMon.Web.api;
using TreeMon.Web.Filters;
using TreeMon.WebAPI.Models;

namespace TreeMon.WebAPI.api.v1
{
    public class OrdersController : ApiBaseController
    {

        [ApiAuthorizationRequired(Operator = ">=", RoleWeight = 4)]
        [HttpPost]
        [Route("api/Orders/Add")]
        [Route("api/Orders/Insert")]
        public ServiceResult Insert(Order n)
        {
            if (n == null || string.IsNullOrWhiteSpace(n.Name))
                return ServiceResponse.Error("Invalid Order sent to server.");

           
            if (!string.IsNullOrWhiteSpace(n.UUID) && this.GetBy(n.UUID).Code == 200)
            {
                return this.Update(n);
            }

          
            OrderManager orderManager = new OrderManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);

            return orderManager.Insert(n, true);
        }

        [ApiAuthorizationRequired(Operator = ">=", RoleWeight = 1)]
        [HttpPost]
        [HttpGet]
        [Route("api/Orders/{name}")]
        public ServiceResult Get(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return ServiceResponse.Error("You must provide a name for the strain.");

            OrderManager orderManager = new OrderManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);

            Order s = (Order) orderManager.Get(name);

            if (s == null)
                return ServiceResponse.Error("Order could not be located for the name " + name);

            return ServiceResponse.OK("", s);
        }

        [ApiAuthorizationRequired(Operator = ">=", RoleWeight = 1)]
        [HttpPost]
        [HttpGet]
        [Route("api/OrdersBy/{uuid}")]
        public ServiceResult GetBy(string uuid)
        {
            if (string.IsNullOrWhiteSpace(uuid))
                return ServiceResponse.Error("You must provide a name for the strain.");

            OrderManager orderManager = new OrderManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);

            Order s = (Order)orderManager.GetBy(uuid);

            if (s == null)
                return ServiceResponse.Error("Order could not be located for the uuid " + uuid);

            return ServiceResponse.OK("", s);
        }

        [ApiAuthorizationRequired(Operator = ">=", RoleWeight = 4)]
        [HttpPost]
        [HttpGet]
        [Route("api/Orders/")]
        public ServiceResult GetOrders(string filter = "")
        {
            if (CurrentUser == null)
                return ServiceResponse.Error("You must be logged in to access this function.");

            OrderManager orderManager = new OrderManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);

            List<dynamic> Orders = (List<dynamic>)orderManager.GetOrders(CurrentUser.AccountUUID, false).Cast<dynamic>().ToList();
            int count;

            DataFilter tmpFilter = this.GetFilter(filter);

            Orders = FilterEx.FilterInput(Orders, tmpFilter, out count);
            return ServiceResponse.OK("", Orders, count);

        }

        [ApiAuthorizationRequired(Operator = ">=", RoleWeight = 4)]
        [HttpPost]
        [HttpDelete]
        [Route("api/Orders/Delete")]
        public ServiceResult Delete(Order n)
        {
            if (n == null || string.IsNullOrWhiteSpace(n.UUID))
                return ServiceResponse.Error("Invalid order was sent.");

            OrderManager orderManager = new OrderManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);

            if (orderManager.Delete(n).Code == 200)
                return ServiceResponse.OK();

            return ServiceResponse.Error("Delete order failed.");
        }

        [ApiAuthorizationRequired(Operator = ">=", RoleWeight = 4)]
        [System.Web.Http.HttpPost]
        [System.Web.Http.HttpDelete]
        [System.Web.Http.Route("api/Orders/Delete/{uuid}")]
        public ServiceResult Delete(string uuid)
        {
            if (string.IsNullOrWhiteSpace(uuid))
                return ServiceResponse.Error("Invalid id was sent.");

            OrderManager orderManager = new OrderManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);

            Order fa = (Order)orderManager.GetBy(uuid);
            if (fa == null)
                return ServiceResponse.Error("Could not find strain.");

            return orderManager.Delete(fa);
        }


        [ApiAuthorizationRequired(Operator = ">=", RoleWeight = 4)]
        [HttpPost]
        [HttpPatch]
        [Route("api/Orders/Update")]
        public ServiceResult Update(Order n)
        {
            if (n == null)
                return ServiceResponse.Error("Invalid Order sent to server.");

            OrderManager orderManager = new OrderManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);

            var dbS = (Order)orderManager.GetBy(n.UUID);

            if (dbS == null)
                return ServiceResponse.Error("Order was not found.");

            if (dbS.DateCreated == DateTime.MinValue)
                dbS.DateCreated = DateTime.UtcNow;

            var form = (Order)n;
            dbS.Name = form.Name;
            dbS.AddedBy = form.AddedBy;
            dbS.AffiliateUUID = form.AffiliateUUID;
            dbS.BillingLocationUUID = form.BillingLocationUUID;
            dbS.CurrencyUUID = form.CurrencyUUID;
            dbS.CustomerEmail = form.CustomerEmail;
            dbS.Discount = form.Discount;
            dbS.FinancAccountUUID = form.FinancAccountUUID;
            dbS.ReconciledToAffiliate = form.ReconciledToAffiliate;
            dbS.ShippingCost = form.ShippingCost;
            dbS.ShippingDate = form.ShippingDate;
            dbS.ShippingLocationUUID = form.ShippingLocationUUID;
            dbS.ShippingMethodUUID = form.ShippingMethodUUID;
            dbS.ShippingSameAsBiling = form.ShippingSameAsBiling;
            dbS.SubTotal = form.SubTotal;
            dbS.Taxes = form.Taxes;
            dbS.Total = form.Total;
            dbS.TrackingUUID = form.TrackingUUID;
            dbS.TransactionID = form.TransactionID;
            dbS.UserUUID = form.UserUUID;
            dbS.CartUUID = form.CartUUID;
            dbS.PayStatus = form.PayStatus;
            //below are not on Order.cshtml form
            dbS.Deleted = form.Deleted;
            dbS.Status = form.Status;
            dbS.SortOrder = form.SortOrder;
            return orderManager.Update(dbS);
        }
    }
}
