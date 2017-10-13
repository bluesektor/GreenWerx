// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

using TreeMon.Models.App;
using TreeMon.Models.Datasets;
using TreeMon.Models.Medical;
using TreeMon.Utilites.Extensions;
using TreeMon.Web.Filters;
using TreeMon.WebAPI.Models;

namespace TreeMon.Web.api.v1
{
    public class AnatomyTagsController : ApiBaseController 
    {
        public AnatomyTagsController()
        {
        
        }

        [ApiAuthorizationRequired(Operator = ">=", RoleWeight = 4)]
        [HttpPost]
        [Route("api/AnatomyTags/Add")]
        [Route("api/AnatomyTags/Insert")]
        public ServiceResult Insert(AnatomyTag n)
        {
            if (CurrentUser == null)
                return ServiceResponse.Error("You must be logged in to access this function.");


            if (string.IsNullOrWhiteSpace(n.AccountUUID))
                n.AccountUUID = CurrentUser.AccountUUID;

            if (string.IsNullOrWhiteSpace(n.CreatedBy))
                n.CreatedBy = CurrentUser.UUID;

            if (n.DateCreated == DateTime.MinValue)
                n.DateCreated = DateTime.UtcNow;

            AnatomyManager AnatomyTagsManager = new AnatomyManager(Globals.DBConnectionKey,Request.Headers?.Authorization?.Parameter);
            return AnatomyTagsManager.Insert(n, true);
        }

        [ApiAuthorizationRequired(Operator = ">=", RoleWeight = 1)]
        [HttpPost]
        [HttpGet]
        [Route("api/AnatomyTag/{name}")]
        public ServiceResult Get(string name )
        {
            if (string.IsNullOrWhiteSpace(name))
                return ServiceResponse.Error("You must provide a name for the AnatomyTags.");

            AnatomyManager AnatomyTagsManager = new AnatomyManager(Globals.DBConnectionKey,Request.Headers?.Authorization?.Parameter);
            AnatomyTag s = AnatomyTagsManager.GetAnatomyTag(name);

            if (s == null)
                return ServiceResponse.Error("AnatomyTags could not be located for the name " + name);

            return ServiceResponse.OK("",s);
        }

        [ApiAuthorizationRequired(Operator = ">=", RoleWeight = 1)]
        [HttpPost]
        [HttpGet]
        [Route("api/AnatomyTagBy/{uuid}")]
        public ServiceResult GetBy(string uuid)
        {
            if (string.IsNullOrWhiteSpace(uuid))
                return ServiceResponse.Error("You must provide a name for the AnatomyTags.");

            AnatomyManager AnatomyTagsManager = new AnatomyManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);
            AnatomyTag s = (AnatomyTag) AnatomyTagsManager.GetBy(uuid);

            if (s == null)
                return ServiceResponse.Error("AnatomyTags could not be located for the uuid " + uuid);

            return ServiceResponse.OK("", s);
        }


        [ApiAuthorizationRequired(Operator = ">=", RoleWeight = 4)]
        [HttpPost]
        [HttpGet]
        [Route("api/AnatomyTags/")]
        public ServiceResult GetAnatomyTags(string filter = "")
        {
            if (CurrentUser == null)
                return ServiceResponse.Error("You must be logged in to access this function.");


           

            AnatomyManager AnatomyTagsManager = new AnatomyManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);
            List<dynamic> AnatomyTags = AnatomyTagsManager.GetAnatomies(CurrentUser.AccountUUID).Cast<dynamic>().ToList();

            int count;

                            DataFilter tmpFilter = this.GetFilter(filter);
                AnatomyTags = FilterEx.FilterInput(AnatomyTags, tmpFilter, out count);
            return ServiceResponse.OK("", AnatomyTags, count);
        }

        [ApiAuthorizationRequired(Operator =">=" , RoleWeight = 4)]
        [HttpPost]
        [HttpDelete]
        [Route("api/AnatomyTags/Delete")]
        public ServiceResult Delete (AnatomyTag n)
        {
            if (n == null || string.IsNullOrWhiteSpace(n.UUID))
                return ServiceResponse.Error("Invalid account was sent.");

            AnatomyManager AnatomyTagsManager = new AnatomyManager(Globals.DBConnectionKey,Request.Headers?.Authorization?.Parameter);
            if ( AnatomyTagsManager.Delete(n)> 0)
                return ServiceResponse.OK();

            return ServiceResponse.Error("An error occurred deleting this AnatomyTags.");
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
        [Route("api/AnatomyTags/Update")]
        public ServiceResult Update(AnatomyTag n)
        {
            if (n == null)
                return ServiceResponse.Error("Invalid AnatomyTags sent to server.");

            AnatomyManager AnatomyTagsManager = new AnatomyManager(Globals.DBConnectionKey,Request.Headers?.Authorization?.Parameter);
            var dbS = (AnatomyTag) AnatomyTagsManager.GetBy(n.UUID);

            if (dbS == null)
                return ServiceResponse.Error("AnatomyTags was not found.");

            var s = (AnatomyTag)n;

            if (dbS.DateCreated == DateTime.MinValue)
                dbS.DateCreated = DateTime.UtcNow;
            dbS.Deleted = s.Deleted;
            dbS.Name = s.Name;
            dbS.Status = s.Status;
            dbS.SortOrder = s.SortOrder;


            return AnatomyTagsManager.Update(dbS);
        }
    }
}
