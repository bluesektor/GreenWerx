// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using TreeMon.Data.Logging.Models;
using TreeMon.Managers.Medical;
using TreeMon.Models.App;
using TreeMon.Models.Datasets;
using TreeMon.Models.Medical;
using TreeMon.Utilites.Extensions;
using TreeMon.Web.Filters;
using TreeMon.WebAPI.Models;

namespace TreeMon.Web.api.v1
{
    public class SideAffectsController : ApiBaseController
    {
        public SideAffectsController()
        {

        }

        [ApiAuthorizationRequired(Operator = ">=", RoleWeight = 1)]
        [HttpPost]
        [Route("api/SideAffects/Add")]
        [Route("api/SideAffects/Insert")]
        public ServiceResult Insert(SideAffect n)
        {
            if (n == null)
                return ServiceResponse.Error("Invalid form data sent.");

            if (CurrentUser == null)
                return ServiceResponse.Error("You must be logged in to access this function.");


            if (string.IsNullOrWhiteSpace(n.AccountUUID) || n.AccountUUID == SystemFlag.Default.Account)
                n.AccountUUID = CurrentUser.AccountUUID;

            if (string.IsNullOrWhiteSpace(n.CreatedBy))
                n.CreatedBy = CurrentUser.UUID;

            if (n.DateCreated == DateTime.MinValue)
                n.DateCreated = DateTime.UtcNow;

            SideAffectManager SideAffectManager = new SideAffectManager(Globals.DBConnectionKey,Request.Headers?.Authorization?.Parameter);

            return SideAffectManager.Insert(n, true);
        }

        //[ApiAuthorizationRequired(Operator = ">=", RoleWeight = 1)]
        //[HttpPost]
        //[HttpGet]
        //[Route("api/SideAffect/{name}")]
        //public ServiceResult Get( string name )
        //{
        //    if (string.IsNullOrWhiteSpace(name))
        //        return ServiceResponse.Error("You must provide a name for the SideAffect.");

        //    SideAffectManager SideAffectManager = new SideAffectManager(Globals.DBConnectionKey,Request.Headers?.Authorization?.Parameter);

        //    List<SideAffect> s = (List<SideAffect>)SideAffectManager.Search(name);

        //    if (s == null)
        //        return ServiceResponse.Error("SideAffect could not be located for the name " + name);

        //    return ServiceResponse.OK("",s);
        //}

        [ApiAuthorizationRequired(Operator = ">=", RoleWeight = 1)]
        [HttpPost]
        [HttpGet]
        [Route("api/SideAffectBy/{uuid}")]
        public ServiceResult GetBy(string uuid)
        {
            if (string.IsNullOrWhiteSpace(uuid))
                return ServiceResponse.Error("You must provide a name for the SideAffect.");

            SideAffectManager SideAffectManager = new SideAffectManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);

            SideAffect s = (SideAffect)SideAffectManager.Get(uuid);

            if (s == null)
                return ServiceResponse.Error("SideAffect could not be located for the uuid " + uuid);

            return ServiceResponse.OK("", s);
        }

        [ApiAuthorizationRequired(Operator = ">=", RoleWeight = 4)]
        [HttpPost]
        [HttpGet]
        [Route("api/SideAffects/{parentUUID}")]
        public ServiceResult GetSideAffects(string parentUUID = "", string filter = "")
            {
            if (CurrentUser == null)
                return ServiceResponse.Error("You must be logged in to access this function.");

            List<dynamic> SideAffects;
               

            SideAffectManager SideAffectManager = new SideAffectManager(Globals.DBConnectionKey,Request.Headers?.Authorization?.Parameter);
            SideAffects = SideAffectManager.GetSideAffects(parentUUID, CurrentUser.AccountUUID).Cast<dynamic>().ToList(); ;
            int count;

                            DataFilter tmpFilter = this.GetFilter(filter);
                SideAffects = FilterEx.FilterInput(SideAffects, tmpFilter, out count);

            return ServiceResponse.OK("",SideAffects, count);
        }


        [HttpPost]
        [HttpGet]
        [Route("api/Doses/{doseUUID}/SideAffects/History/{parentUUID}")]
        public ServiceResult GetChildSideAffects(string doseUUID, string parentUUID = "", string filter = "")
        {
            if (CurrentUser == null)
                return ServiceResponse.Error("You must be logged in to access this function.");


            if (string.IsNullOrWhiteSpace(doseUUID))
                return ServiceResponse.Error("You must send a dose uuid.");

           
            SideAffectManager SideAffectManager = new SideAffectManager(Globals.DBConnectionKey,Request.Headers?.Authorization?.Parameter);

            List<dynamic> SymptomsLog =SideAffectManager.GetSideAffectsByDose(doseUUID, parentUUID, CurrentUser.AccountUUID).Cast<dynamic>().ToList();
            int count;


                            DataFilter tmpFilter = this.GetFilter(filter);
                SymptomsLog = FilterEx.FilterInput(SymptomsLog, tmpFilter, out count);

            return ServiceResponse.OK("",SymptomsLog, count);
        }




        [ApiAuthorizationRequired(Operator =">=" , RoleWeight = 4)]
        [HttpPost]
        [HttpDelete]
        [Route("api/SideAffects/Delete")]
        public ServiceResult Delete(SideAffect n)
        {
            if (n == null || string.IsNullOrWhiteSpace(n.UUID))
                return ServiceResponse.Error("Invalid account was sent.");

            SideAffectManager SideAffectManager = new SideAffectManager(Globals.DBConnectionKey,Request.Headers?.Authorization?.Parameter);

            return SideAffectManager.Delete(n);
                            
            
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
        [Route("api/SideAffects/Update")]
        public ServiceResult Update(SideAffect s)
        {
            if (s == null)
                return ServiceResponse.Error("Invalid SideAffect sent to server.");

            SideAffectManager SideAffectManager = new SideAffectManager(Globals.DBConnectionKey,Request.Headers?.Authorization?.Parameter);

            var dbS = (SideAffect) SideAffectManager.Get(s.UUID);

            if (dbS == null)
                return ServiceResponse.Error("SideAffect was not found.");
           

            if (dbS.DateCreated == DateTime.MinValue)
                dbS.DateCreated = DateTime.UtcNow;
            dbS.Deleted = s.Deleted;
            dbS.Name = s.Name;
            dbS.Status = s.Status;
            dbS.SortOrder = s.SortOrder;


            return SideAffectManager.Update(dbS);
        }
    }
}
