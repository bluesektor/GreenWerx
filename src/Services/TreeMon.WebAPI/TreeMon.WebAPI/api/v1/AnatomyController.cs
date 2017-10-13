// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using TreeMon.Data.Logging.Models;
using TreeMon.Models.App;
using TreeMon.Models.Datasets;
using TreeMon.Models.Medical;
using TreeMon.Utilites.Extensions;
using TreeMon.Web.Filters;

namespace TreeMon.Web.api.v1
{
    public class AnatomyController : ApiBaseController
    {
        public AnatomyController()
        {
          
        }

        [ApiAuthorizationRequired(Operator = ">=", RoleWeight = 4)]
        [HttpPost]
    [Route("api/Anatomy/Add")]
    public ServiceResult AddAnatomy(Anatomy s)
        {
            if (CurrentUser == null)
                return ServiceResponse.Error("You must be logged in to access this function.");


            if (string.IsNullOrWhiteSpace(s.AccountUUID) || s.AccountUUID == SystemFlag.Default.Account)
                s.AccountUUID = CurrentUser.AccountUUID;

            if (string.IsNullOrWhiteSpace(s.CreatedBy))
                s.CreatedBy = CurrentUser.UUID;

            if (s.DateCreated == DateTime.MinValue)
                s.DateCreated = DateTime.UtcNow;

            AnatomyManager anatomyManager = new AnatomyManager(Globals.DBConnectionKey,Request.Headers?.Authorization?.Parameter);
            return anatomyManager.Insert(s, true);
        }

    [ApiAuthorizationRequired(Operator = ">=", RoleWeight = 1)]
    [HttpPost]
    [HttpGet]
    [Route("api/Anatomy/{name}")]
    public ServiceResult GetAnatomyByName(string name)
            {
                if (string.IsNullOrWhiteSpace(name))
                    return ServiceResponse.Error("You must provide a name for the Anatomy.");

            AnatomyManager anatomyManager = new AnatomyManager(Globals.DBConnectionKey,Request.Headers?.Authorization?.Parameter);
            Anatomy s = (Anatomy) anatomyManager.Get(name );

                if (s == null)
                    return ServiceResponse.Error("Anatomy could not be located for the name " + name);

                return ServiceResponse.OK("",s);
            }

    [ApiAuthorizationRequired(Operator =">=" , RoleWeight = 4)]
        [HttpPost]
    [HttpGet]
    [Route("api/Anatomy/")]
    public ServiceResult Get( string filter = "")
    {
        if (CurrentUser == null)
            return ServiceResponse.Error("You must be logged in to access this function.");

        AnatomyManager anatomyManager = new AnatomyManager(Globals.DBConnectionKey,Request.Headers?.Authorization?.Parameter);
        List<dynamic> anatomy = anatomyManager.GetAnatomies(CurrentUser.AccountUUID).Cast<dynamic>().ToList();
        int count;
        DataFilter tmpFilter = this.GetFilter(filter);
        anatomy = FilterEx.FilterInput(anatomy, tmpFilter, out count);
        return ServiceResponse.OK("", anatomy, count);
    }

    [ApiAuthorizationRequired(Operator =">=" , RoleWeight = 4)]
    [HttpPost]
    [HttpDelete]
    [Route("api/Anatomy/Delete")]
    public ServiceResult Delete(Anatomy s)
    {
        if (s == null || string.IsNullOrWhiteSpace(s.UUID))
            return ServiceResponse.Error("Invalid account was sent.");

        AnatomyManager anatomyManager = new AnatomyManager(Globals.DBConnectionKey,Request.Headers?.Authorization?.Parameter);
        return anatomyManager.Delete(s);

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
        [Route("api/Anatomy/Update")]
    public ServiceResult Update(Anatomy s)
            {
                if (s == null)
                    return ServiceResponse.Error("Invalid Anatomy sent to server.");

            AnatomyManager anatomyManager = new AnatomyManager(Globals.DBConnectionKey,Request.Headers?.Authorization?.Parameter);
            var dbS = (Anatomy)anatomyManager.GetBy(s.UUID);

                if (dbS == null)
                    return ServiceResponse.Error("Anatomy was not found.");

                if (dbS.DateCreated == DateTime.MinValue)
                    dbS.DateCreated = DateTime.UtcNow;
                dbS.Deleted = s.Deleted;
                dbS.Name = s.Name;
                dbS.Status = s.Status;
                dbS.SortOrder = s.SortOrder;
              

                return anatomyManager.Update(dbS);
            }
        }
}
