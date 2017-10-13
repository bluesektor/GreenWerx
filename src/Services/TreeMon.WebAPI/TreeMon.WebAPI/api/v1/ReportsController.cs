// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using TreeMon.Data.Logging;
using TreeMon.Managers.DataSets;
using TreeMon.Managers.Membership;
using TreeMon.Models.App;
using TreeMon.Models.Datasets;
using TreeMon.Utilites.Helpers;
using TreeMon.Web.Filters;

namespace TreeMon.Web.api.v1
{
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
        [Route("api/Reports/{category}/Dataset/{field}")]
        public ServiceResult GetDataset(string category = "", string field = "")
        {
            List<DataPoint> dataSet;

            if (string.IsNullOrWhiteSpace(category))
                return ServiceResponse.Error("You must provide a category to get the datasets.");

            if (string.IsNullOrWhiteSpace(field))
                return ServiceResponse.Error("You must provide a series to get the datasets.");

            if (CurrentUser == null)
                return ServiceResponse.Error("You must be logged in to access this function.");



            if (category?.ToLower() == "users" && CurrentUser.SiteAdmin == false ) { //BACKLOG  turn on the flag to log permission routes to log this. add numeric value to roles sow we can include multiple roles by doing math >= roleWeight 
                RoleManager roleManager = new RoleManager(Globals.DBConnectionKey, CurrentUser);

                if (!roleManager.IsUserInRole(CurrentUser.UUID, "Admin", CurrentUser.AccountUUID) || !roleManager.IsUserInRole(CurrentUser.UUID, "owner", CurrentUser.AccountUUID))
                {
                    return ServiceResponse.Error("You are not authorized to query the category:" + category);
                }
            }

            try
            {
                Task<string> content = ActionContext.Request.Content.ReadAsStringAsync();
                if (content == null)
                    return ServiceResponse.Error("No screens were sent.");

                string body = content.Result;

                if (string.IsNullOrEmpty(body))
                    return ServiceResponse.Error("No screens were sent.");

                List<DataScreen> screens = JsonConvert.DeserializeObject<List<DataScreen>>(body);

                DatasetManager dm = new DatasetManager(Globals.DBConnectionKey,Request.Headers?.Authorization?.Parameter);
                dataSet = dm.GetData(category, field, screens);
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