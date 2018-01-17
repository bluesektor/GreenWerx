// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using TreeMon.Managers.Store;
using TreeMon.Models;
using TreeMon.Models.App;
using TreeMon.Models.Datasets;
using TreeMon.Models.Store;
using TreeMon.Utilites.Extensions;
using TreeMon.Web.Filters;
using TreeMon.WebAPI.Models;

namespace TreeMon.Web.api.v1
{
    public class VendorsController : ApiBaseController 
    {
        public VendorsController()
        {
        }

        [ApiAuthorizationRequired(Operator = ">=", RoleWeight = 4)]
        [HttpPost]
        [Route("api/Vendors/Add")]
        public ServiceResult Insert(Vendor n)
        {
            if (CurrentUser == null)
                return ServiceResponse.Error("You must be logged in to access this function.");


            if (string.IsNullOrWhiteSpace(n.AccountUUID))
                n.AccountUUID = CurrentUser.AccountUUID;

            if (string.IsNullOrWhiteSpace(n.CreatedBy))
                n.CreatedBy = CurrentUser.UUID;

            if (n.DateCreated == DateTime.MinValue)
                n.DateCreated = DateTime.UtcNow;

            VendorManager vendorManager = new VendorManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);

            return vendorManager.Insert(n, true);

        }

        [ApiAuthorizationRequired(Operator = ">=", RoleWeight = 4)]
        [HttpPost]
        [HttpGet]
        [Route("api/Vendors/{name}")]
        public ServiceResult Get( string name )
        {
            if (CurrentUser == null)
                return ServiceResponse.Error("You must be logged in to access this function.");

            VendorManager vendorManager = new VendorManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);

            List<Vendor> s = vendorManager.Search(name);

            if (s == null || s.Count == 0)
                return ServiceResponse.Error("Vendor could not be located for the name " + name);

            return ServiceResponse.OK("",s);
        }

        [ApiAuthorizationRequired(Operator = ">=", RoleWeight = 4)]
        [HttpPost]
        [HttpGet]
        [Route("api/VendorsBy/{uuid}")]
        public ServiceResult GetBy(string uuid)
        {
            if (CurrentUser == null)
                return ServiceResponse.Error("You must be logged in to access this function.");

            VendorManager vendorManager = new VendorManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);

            Vendor s = (Vendor)vendorManager.Get(uuid);

            if (s == null)
                return ServiceResponse.Error("Vendor could not be located for the name " + uuid);

            return ServiceResponse.OK("", s);
        }

        [ApiAuthorizationRequired(Operator = ">=", RoleWeight = 4)]
        [HttpPost]
        [HttpGet]
        [Route("api/Vendors/")]
        public ServiceResult GetVendors(string filter = "")
        {
            if (CurrentUser == null)
                return ServiceResponse.Error("You must be logged in to access this function.");

            VendorManager vendorManager = new VendorManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);

            List<dynamic> Vendors = vendorManager.GetVendors(CurrentUser.AccountUUID, false, true).Cast<dynamic>().ToList();
            int count;
                            DataFilter tmpFilter = this.GetFilter(filter);
                Vendors = FilterEx.FilterInput(Vendors, tmpFilter, out count);
            return ServiceResponse.OK("", Vendors, count);
        }

        [ApiAuthorizationRequired(Operator =">=" , RoleWeight = 4)]
        [HttpPost]
        [HttpDelete]
        [Route("api/Vendors/Delete")]
        public ServiceResult Delete(Vendor n)
        {
            if (n == null || string.IsNullOrWhiteSpace(n.UUID))
                return ServiceResponse.Error("Invalid account was sent.");

            if (CurrentUser == null)
                return ServiceResponse.Error("You must be logged in to access this function.");

            VendorManager vendorManager = new VendorManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);

            return vendorManager.Delete(n);
        }

        [ApiAuthorizationRequired(Operator = ">=", RoleWeight = 4)]
        [HttpPost]
        [HttpDelete]
        [Route("api/Vendors/Delete/{vendorUUID}")]
        public ServiceResult Delete(string vendorUUID)
        {
            if (  string.IsNullOrWhiteSpace(vendorUUID) )
                return ServiceResponse.Error("Invalid account was sent.");

            if (CurrentUser == null)
                return ServiceResponse.Error("You must be logged in to access this function.");

            VendorManager vendorManager = new VendorManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);

            Vendor v = (Vendor)vendorManager.Get(vendorUUID);

            return this.Delete(v);
        }

        /// <summary>
        /// Fields updated..
        ///     Category 
        ///     Name 
        ///     Cost
        ///     Price
        ///     Weight 
        ///     WeightUOM
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        [ApiAuthorizationRequired(Operator =">=" , RoleWeight = 4)]
        [HttpPost]
        [HttpPatch]
        [Route("api/Vendors/Update")]
        public ServiceResult Update(Vendor v)
        {
            if (v == null)
                return ServiceResponse.Error("Invalid Vendor sent to server.");

          
            if (CurrentUser == null)
                return ServiceResponse.Error("You must be logged in to access this function.");

            VendorManager vendorManager = new VendorManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);

            var dbv =(Vendor) vendorManager.Get(v.UUID);

            if (dbv == null)
                return ServiceResponse.Error("Vendor was not found.");

            if (dbv.DateCreated == DateTime.MinValue)
                dbv.DateCreated = DateTime.UtcNow;

            dbv.Deleted = v.Deleted;
            dbv.Name = v.Name;
            dbv.Status = v.Status;
            dbv.SortOrder = v.SortOrder;


            return vendorManager.Update(dbv);
        }
    }
}
