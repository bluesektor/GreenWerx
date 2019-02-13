// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using TreeMon.Data.Helpers;
using TreeMon.Data.Logging;
using TreeMon.Managers.DataSets;
using TreeMon.Managers.Membership;
using TreeMon.Models.App;
using TreeMon.Models.Datasets;
using TreeMon.Utilites.Extensions;
using TreeMon.Utilites.Helpers;
using TreeMon.Web.Filters;
using WebApi.OutputCache.V2;

namespace TreeMon.Web.api.v1
{
    [CacheOutput(ClientTimeSpan = 100, ServerTimeSpan = 100)]
    public class ReportsController : ApiBaseController
    {

        public ReportsController()
        {
        }

        /// <summary>
        /// need to let the user select the series for the dataset.
        /// </summary>
        /// <param name = "category" > Database table to pull from.</param>
        /// <param name = "field" > field in the table to be returned</param>
        /// <param name = "startIndex" ></ param >
        /// < param name= "pageSize" ></ param >
        /// < param name= "sorting" ></ param >
        [ApiAuthorizationRequired(Operator =">=" , RoleWeight = 5)]
        [HttpPost]
        [HttpGet]
        [Route("api/Reports/{type}/Dataset/{field}")]
        public ServiceResult GetDataset(string type = "", string field = "")
        {
      

            List<DataPoint> dataSet;

            if (string.IsNullOrWhiteSpace(type))
                return ServiceResponse.Error("You must provide a type to get the datasets.");

            if (string.IsNullOrWhiteSpace(field))
                return ServiceResponse.Error("You must provide a series to get the datasets.");

            if (CurrentUser == null)
                return ServiceResponse.Error("You must be logged in to access this function.");

            //todo log all access to this.
            //CurrentUser.RoleWeight
            //CurrentUser.AccountUUID

            if (type?.ToLower() == "users" && CurrentUser.SiteAdmin == false ) {
                //BACKLOG  turn on the flag to log permission routes to log this. 
                //add numeric value to roles s we can include multiple roles by doing math >= roleWeight 
                RoleManager roleManager = new RoleManager(Globals.DBConnectionKey, CurrentUser);

                if (!roleManager.IsUserInRole(CurrentUser.UUID, "Admin", CurrentUser.AccountUUID) || !roleManager.IsUserInRole(CurrentUser.UUID, "owner", CurrentUser.AccountUUID))
                {
                    return ServiceResponse.Error("You are not authorized to query the type:" + type);
                }
            }

            try
            {
                 DataFilter tmpFilter = this.GetFilter(Request);
                DatasetManager dm = new DatasetManager(Globals.DBConnectionKey,Request.Headers?.Authorization?.Parameter);
                dataSet = dm.GetDataSet(type, tmpFilter);
                return ServiceResponse.OK("", dataSet);
            }
            catch (Exception ex)
            {
                Debug.Assert(false, ex.Message);
                SystemLogger logger = new SystemLogger(Globals.DBConnectionKey);

                logger.InsertError(ex.Message, "ReportsController", "GetDataset");
                return ServiceResponse.Error("Error retrieving dataset.");
            }
        }

    }
}