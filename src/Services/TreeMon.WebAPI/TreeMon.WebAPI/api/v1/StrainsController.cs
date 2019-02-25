// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using TreeMon.Data.Logging.Models;
using TreeMon.Managers;
using TreeMon.Managers.Membership;
using TreeMon.Managers.Plant;
using TreeMon.Models;
using TreeMon.Models.App;
using TreeMon.Models.Datasets;
using TreeMon.Models.Membership;
using TreeMon.Models.Plant;
using TreeMon.Utilites.Extensions;
using TreeMon.Web.Filters;
using TreeMon.Web.Models;
using TreeMon.WebAPI.Models;
using WebApi.OutputCache.V2;
using WebApiThrottle;

namespace TreeMon.Web.api.v1
{
    [CacheOutput(ClientTimeSpan = 100, ServerTimeSpan = 100)]
    public class StrainsController : ApiBaseController
    {

        public StrainsController()
        {
        }

        [ApiAuthorizationRequired(Operator = ">=", RoleWeight = 4)]
        [HttpPost]
        [Route("api/Strains/Add")]
        [Route("api/Strains/Insert")]
        public ServiceResult Insert(StrainForm s)
        {
            if(s== null || string.IsNullOrWhiteSpace(s.Name) )
                return ServiceResponse.Error("Invalid Strain sent to server.");

            if (!string.IsNullOrWhiteSpace(s.UUID))
            {
                return this.Update(s);
            }

            if (s.IndicaPercent + s.SativaPercent > 100)
                return ServiceResponse.Error("Variety percentages cannot be greater than 100%.");

            string authToken = Request.Headers?.Authorization?.Parameter;

            
            UserSession us = SessionManager.GetSession(authToken);
            if (us == null)
                return ServiceResponse.Error("You must be logged in to access this function.");

            if (us.Captcha?.ToUpper() != s.Captcha?.ToUpper())
                return ServiceResponse.Error("Invalid code.");

            if (string.IsNullOrWhiteSpace(us.UserData))
                return ServiceResponse.Error("Couldn't retrieve user data.");

            if (CurrentUser == null)
                return ServiceResponse.Error("You must be logged in to access this function.");


            if (string.IsNullOrWhiteSpace(s.AccountUUID) || s.AccountUUID == SystemFlag.Default.Account)
                s.AccountUUID = CurrentUser.AccountUUID;

            if (string.IsNullOrWhiteSpace(s.CreatedBy))
                s.CreatedBy = CurrentUser.UUID;

            if (s.DateCreated == DateTime.MinValue )
                   s.DateCreated = DateTime.UtcNow;

            AccountManager accountManager = new AccountManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);
            Account a = accountManager.AddAccountFromStrain(s);
            if (a != null && string.IsNullOrWhiteSpace(s.BreederUUID))
                s.BreederUUID = a.UUID;

            #region Convert to Strain from StrainView because entity frameworks doesn't recognize casting.

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<StrainForm, Strain>();
            });

            IMapper mapper = config.CreateMapper();
            var dest = mapper.Map<StrainForm, Strain>(s);
            #endregion

            StrainManager strainManager = new StrainManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);

            return strainManager.Insert(dest);
        }

        [EnableThrottling(PerSecond = 1, PerMinute = 20, PerHour = 200, PerDay = 1500, PerWeek = 3000)]
        [ApiAuthorizationRequired(Operator = ">=", RoleWeight = 1)]
        [HttpPost]
        [HttpGet]
        [Route("api/Strains/{name}")]
        public ServiceResult Get( string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return ServiceResponse.Error("You must provide a name for the strain.");

            StrainManager strainManager = new StrainManager(Globals.DBConnectionKey,Request.Headers?.Authorization?.Parameter);

            List<Strain> s = strainManager.Search(name);

            if (s == null || s.Count == 0)
                return ServiceResponse.Error("Strain could not be located for the name " + name);

            return ServiceResponse.OK("",s);
        }


        [EnableThrottling(PerSecond = 1, PerMinute = 20, PerHour = 200, PerDay = 1500, PerWeek = 3000)]
        [ApiAuthorizationRequired(Operator = ">=", RoleWeight = 1)]
        [HttpGet]
        [Route("api/StrainsBy/{uuid}")]
        public ServiceResult GetBy(string uuid)
        {
            if (string.IsNullOrWhiteSpace(uuid))
                return ServiceResponse.Error("You must provide a uuid for the strain.");

            StrainManager strainManager = new StrainManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);

            Strain s = (Strain)strainManager.Get(uuid);

            if (s == null)
                return ServiceResponse.Error("Strain could not be located for the name " + uuid);

            return ServiceResponse.OK("", s);
        }


        //todo bookmark latest. none of the paging is working
        //assets/strains
     //   assets/products
     //    not paging, maybe check the filter in the angular page

                [EnableThrottling(PerSecond = 1, PerMinute= 20, PerHour= 200, PerDay= 1500, PerWeek= 3000)]
        [ApiAuthorizationRequired(Operator =">=" , RoleWeight = 4)]
        [HttpPost]
        [HttpGet]
        [Route("api/Strains")]
        public ServiceResult GetStrains()
        {
            if (CurrentUser == null)
                return ServiceResponse.Error("You must be logged in to access this function.");

            StrainManager strainManager = new StrainManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);

            List<dynamic>   Strains = (List <dynamic>) strainManager.GetStrains(CurrentUser.AccountUUID, false, true).Cast<dynamic>().ToList();
            int count;

             DataFilter tmpFilter = this.GetFilter(Request);
            
            Strains = Strains.Filter( tmpFilter, out count);
            return ServiceResponse.OK("", Strains, count);
            
        }

          [ApiAuthorizationRequired(Operator =">=" , RoleWeight = 4)]
        [HttpPost]
        [HttpDelete]
        [Route("api/Strains/Delete")]
        public ServiceResult Delete(Strain s)
        {
            if (s == null || string.IsNullOrWhiteSpace(s.UUID))
                return ServiceResponse.Error("Invalid account was sent.");

            StrainManager strainManager = new StrainManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);

            return strainManager.Delete(s);
             
        }

        [ApiAuthorizationRequired(Operator = ">=", RoleWeight = 4)]
        [System.Web.Http.HttpPost]
        [System.Web.Http.HttpDelete]
        [System.Web.Http.Route("api/Strains/Delete/{uuid}")]
        public ServiceResult Delete( string uuid)
        {
            if (string.IsNullOrWhiteSpace(uuid))
                return ServiceResponse.Error("Invalid id was sent.");

            StrainManager strainManager = new StrainManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);

            Strain fa = (Strain)strainManager.Get(uuid);
            if (fa == null)
                return ServiceResponse.Error("Could not find strain.");

            return strainManager.Delete(fa);
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
        [Route("api/Strains/Update")]
        public ServiceResult Update(Strain form)
        {
            if (form == null)
                return ServiceResponse.Error("Invalid Strain sent to server.");

            

            if(form.IndicaPercent + form.SativaPercent > 100)
                    return ServiceResponse.Error("Variety percentages cannot be greater than 100%.");

            StrainManager strainManager = new StrainManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);

            var dbS = (Strain)strainManager.Get(form.UUID);

            if (dbS == null)
                return ServiceResponse.Error("Strain was not found.");

            if (dbS.DateCreated == DateTime.MinValue)
                dbS.DateCreated = DateTime.UtcNow;

            dbS.Name = form.Name;
            dbS.BreederUUID = form.BreederUUID;
            dbS.CategoryUUID = form.CategoryUUID;
            dbS.HarvestTime = form.HarvestTime;
            dbS.IndicaPercent = form.IndicaPercent;
            dbS.SativaPercent = form.SativaPercent;
            dbS.AutoFlowering = form.AutoFlowering;
            dbS.Generation = form.Generation;
            dbS.Lineage = form.Lineage;
            //below are not on Strain.cshtml form
            dbS.Deleted = form.Deleted;
            dbS.Status = form.Status;
            dbS.SortOrder = form.SortOrder;
            return strainManager.Update(dbS);
        }
    }
}
