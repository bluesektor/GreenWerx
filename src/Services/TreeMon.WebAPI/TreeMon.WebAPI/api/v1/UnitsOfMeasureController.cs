// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using TreeMon.Data.Logging;
using TreeMon.Data.Logging.Models;
using TreeMon.Managers;
using TreeMon.Models;
using TreeMon.Models.App;
using TreeMon.Models.Datasets;
using TreeMon.Utilites.Extensions;
using TreeMon.Utilites.Helpers;
using TreeMon.Web.Filters;
using TreeMon.WebAPI.Models;

namespace TreeMon.Web.api.v1
{
    public class UnitsOfMeasureController : ApiBaseController
    {

        public UnitsOfMeasureController()
        {

        }

        [ApiAuthorizationRequired(Operator = ">=", RoleWeight = 4)]
        [HttpPost]
        [HttpGet]
        [Route("api/UnitsOfMeasure/ProductCategories/Assign")]
        public ServiceResult AssignUOMsToProductCategories()
        {
            if (CurrentUser == null)
                return ServiceResponse.Error("You must be logged in to access this function.");

            ServiceResult res = new ServiceResult();
            res.Code = 200;

            try
            {
                Task<string> content = ActionContext.Request.Content.ReadAsStringAsync();
                if (content == null)
                    return ServiceResponse.Error("No data was sent.");

                string body = content.Result;

                if (string.IsNullOrEmpty(body))
                    return ServiceResponse.Error("Content body is empty.");

                List<UnitOfMeasure> uoms = JsonConvert.DeserializeObject<List<UnitOfMeasure>>(body);

                foreach (UnitOfMeasure u in uoms)
                {
                    u.AccountUUID = CurrentUser.AccountUUID;
                    u.CreatedBy = CurrentUser.UUID;
                    u.DateCreated = DateTime.UtcNow;
                    UnitOfMeasureManager UnitsOfMeasureManager = new UnitOfMeasureManager(Globals.DBConnectionKey,Request.Headers?.Authorization?.Parameter);

                    ServiceResult tmpRes = UnitsOfMeasureManager.Insert(u,false);
                    if (tmpRes.Code != 200)
                    {
                        res.Code = tmpRes.Code;
                        res.Message += tmpRes.Message + "<br/>";
                    }
                }
            }
            catch (Exception ex)
            {
                res = ServiceResponse.Error(ex.Message);
                Debug.Assert(false, ex.Message);
                SystemLogger logger = new SystemLogger(Globals.DBConnectionKey);

                logger.InsertError(ex.Message, "UnitsOfMeasureController", "AssignUOMsToProductCategories");
            }
            return res;
        }


        [ApiAuthorizationRequired(Operator = ">=", RoleWeight = 4)]
        [HttpPost]
        [Route("api/UnitsOfMeasure/Add")]
        [Route("api/UnitsOfMeasure/Insert")]
        public ServiceResult Insert(UnitOfMeasure n)
        {

            if (CurrentUser == null)
                return ServiceResponse.Error("You must be logged in to access this function.");

            if (string.IsNullOrWhiteSpace(n.AccountUUID) || n.AccountUUID == SystemFlag.Default.Account)
                n.AccountUUID = CurrentUser.AccountUUID;

            if (string.IsNullOrWhiteSpace(n.CreatedBy))
                n.CreatedBy = CurrentUser.UUID;

            if (n.DateCreated == DateTime.MinValue)
                n.DateCreated = DateTime.UtcNow;

            UnitOfMeasureManager UnitsOfMeasureManager = new UnitOfMeasureManager(Globals.DBConnectionKey,Request.Headers?.Authorization?.Parameter);

            return UnitsOfMeasureManager.Insert(n, true);
            }

        [ApiAuthorizationRequired(Operator = ">=", RoleWeight = 1)]
        [HttpPost]
        [HttpGet]
        [Route("api/UnitsOfMeasure/{name}")]
        public ServiceResult Get( string name )
        {
            if (string.IsNullOrWhiteSpace(name))
                return ServiceResponse.Error("You must provide a name for the UnitsOfMeasure.");

            UnitOfMeasureManager UnitsOfMeasureManager = new UnitOfMeasureManager(Globals.DBConnectionKey,Request.Headers?.Authorization?.Parameter);

            UnitOfMeasure s = (UnitOfMeasure)UnitsOfMeasureManager.Get(name);

            if (s == null)
                return ServiceResponse.Error("UnitsOfMeasure could not be located for the name " + name);

            return ServiceResponse.OK("",s);
        }

        [ApiAuthorizationRequired(Operator = ">=", RoleWeight = 1)]
        [HttpPost]
        [HttpGet]
        [Route("api/UnitsOfMeasureBy/{uuid}")]
        public ServiceResult GetBy(string uuid)
        {
            if (string.IsNullOrWhiteSpace(uuid))
                return ServiceResponse.Error("You must provide a name for the UnitsOfMeasure.");

            UnitOfMeasureManager UnitsOfMeasureManager = new UnitOfMeasureManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);

            UnitOfMeasure s = (UnitOfMeasure)UnitsOfMeasureManager.GetBy(uuid);

            if (s == null)
                return ServiceResponse.Error("UnitsOfMeasure could not be located for the uuid " + uuid);

            return ServiceResponse.OK("", s);
        }


        [ApiAuthorizationRequired(Operator =">=" , RoleWeight = 4)]
        [HttpPost]
        [HttpGet]
        [Route("api/UnitsOfMeasure/")]
        public ServiceResult GetUnitsOfMeasure(string filter = "")
        {
            if (CurrentUser == null)
                return ServiceResponse.Error("You must be logged in to access this function.");

           
            
            UnitOfMeasureManager UnitsOfMeasureManager = new UnitOfMeasureManager(Globals.DBConnectionKey,Request.Headers?.Authorization?.Parameter);

            List<dynamic> UnitOfMeasures = UnitsOfMeasureManager.GetUnitsOfMeasure(CurrentUser.AccountUUID).Cast<dynamic>().ToList();
            int count;

                            DataFilter tmpFilter = this.GetFilter(filter);
                UnitOfMeasures = FilterEx.FilterInput(UnitOfMeasures, tmpFilter, out count);
            return ServiceResponse.OK("", UnitOfMeasures, count);
        }



        [ApiAuthorizationRequired(Operator =">=" , RoleWeight = 4)]
        [HttpPost]
        [HttpDelete]
        [Route("api/UnitsOfMeasure/Delete")]
        public ServiceResult Delete(UnitOfMeasure n)
        {
            if (n == null || string.IsNullOrWhiteSpace(n.UUID))
                return ServiceResponse.Error("Invalid account was sent.");

            UnitOfMeasureManager UnitsOfMeasureManager = new UnitOfMeasureManager(Globals.DBConnectionKey,Request.Headers?.Authorization?.Parameter);

            return UnitsOfMeasureManager.Delete(n);
        }

        [ApiAuthorizationRequired(Operator = ">=", RoleWeight = 4)]
        [System.Web.Http.HttpPost]
        [System.Web.Http.HttpDelete]
        [System.Web.Http.Route("api/UnitsOfMeasure/Delete/{uuid}")]
        public ServiceResult Delete(string uuid)
        {
            if (string.IsNullOrWhiteSpace(uuid))
                return ServiceResponse.Error("Invalid id was sent.");

            UnitOfMeasureManager UnitsOfMeasureManager = new UnitOfMeasureManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);

            UnitOfMeasure fa = (UnitOfMeasure)UnitsOfMeasureManager.GetBy(uuid);
            if (fa == null)
                return ServiceResponse.Error("Could not find measure.");

            return UnitsOfMeasureManager.Delete(fa);
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
        [Route("api/UnitsOfMeasure/Update")]
        public ServiceResult Update(UnitOfMeasure s)
                {
                    if (s == null)
                        return ServiceResponse.Error("Invalid UnitsOfMeasure sent to server.");

            UnitOfMeasureManager UnitsOfMeasureManager = new UnitOfMeasureManager(Globals.DBConnectionKey,Request.Headers?.Authorization?.Parameter);

          

            var dbS = (UnitOfMeasure)UnitsOfMeasureManager.GetBy(s.UUID);

                    if (dbS == null)
                        return ServiceResponse.Error("UnitsOfMeasure was not found.");

                    if (dbS.DateCreated == DateTime.MinValue)
                        dbS.DateCreated = DateTime.UtcNow;
                    dbS.Deleted = s.Deleted;
                    dbS.Name = s.Name;
                    dbS.Status = s.Status;
                    dbS.SortOrder = s.SortOrder;
            dbS.Category = s.Category;
            dbS.ShortName = s.ShortName;
                    return UnitsOfMeasureManager.Update(dbS);
                }
    }
}
