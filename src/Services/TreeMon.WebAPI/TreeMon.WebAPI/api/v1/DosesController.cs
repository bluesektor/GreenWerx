// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web.Http;
using TreeMon.Managers;
using TreeMon.Managers.Medical;
using TreeMon.Managers.Store;
using TreeMon.Models;
using TreeMon.Models.App;
using TreeMon.Models.Datasets;
using TreeMon.Models.Medical;
using TreeMon.Models.Store;
using TreeMon.Utilites.Extensions;
using TreeMon.Web.Filters;
using TreeMon.Web.Models;
using TreeMon.WebAPI.Models;

namespace TreeMon.Web.api.v1
{
    public class DosesController : ApiBaseController
    {
        public DosesController()
        {


        }

        [ApiAuthorizationRequired(Operator = ">=", RoleWeight = 4)]
        [HttpPost]
        [Route("api/DoseLog/Add")]
        [Route("api/DoseLog/Insert")]
        public ServiceResult Insert(DoseLogForm d)
        {
            string authToken = Request.Headers?.Authorization?.Parameter;
            //d.UserUUID  <= patient id. for now make this a hidden field and use the cookie value.
            //               for an app that uses multiple patients then we'll need make a combobox or some list to select
            //whom we're logging for. 


            if (CurrentUser == null)
                return ServiceResponse.Error("You must be logged in to access this function.");


            UserSession us = SessionManager.GetSession(authToken);
            if (us == null)
                return ServiceResponse.Error("You must be logged in to access this function.");

         

            if (us.Captcha?.ToUpper() != d.Captcha?.ToUpper())
                return ServiceResponse.Error("Invalid code.");


            if (string.IsNullOrWhiteSpace(d.AccountUUID))
                d.AccountUUID = CurrentUser.AccountUUID;

            if (string.IsNullOrWhiteSpace(d.CreatedBy))
                d.CreatedBy = us.UUID;

            if (d.DateCreated == DateTime.MinValue)
                d.DateCreated = DateTime.UtcNow;

        
            d.Active = true;
            d.Deleted = false;

            if (d.DoseDateTime == null || d.DoseDateTime == DateTime.MinValue)
                return ServiceResponse.Error("You must a date time for the dose.");
            
            if(string.IsNullOrWhiteSpace(d.ProductUUID) )
                return ServiceResponse.Error("You must select a product.");
            
             ProductManager productManager = new ProductManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);
            Product p = (Product)productManager.Get(d.ProductUUID);
            if(p== null)
            {
                return ServiceResponse.Error("Product could not be found. You must select a product, or create one from the products page.");
            }

            if (string.IsNullOrWhiteSpace(d.Name))
                d.Name = string.Format("{0} {1} {2}", p.Name, d.Quantity, d.UnitOfMeasure);


            if (d.Quantity <= 0)
                return ServiceResponse.Error("You must enter a quantity");

            if(string.IsNullOrWhiteSpace(d.UnitOfMeasure))
                return ServiceResponse.Error("You must select a unit of measure.");

            UnitOfMeasureManager uomm = new UnitOfMeasureManager(Globals.DBConnectionKey,Request.Headers?.Authorization?.Parameter);
            if (uomm.Get(d.UnitOfMeasure) == null)
            {
                UnitOfMeasure uom = (UnitOfMeasure)uomm.Search(d.UnitOfMeasure)?.FirstOrDefault();
                if (uom == null)
                {
                    uom = new UnitOfMeasure();
                    uom.Name = d.UnitOfMeasure.Trim();
                    uom.AccountUUID = CurrentUser.AccountUUID;
                    uom.Active = true;
                    uom.Deleted = false;
                    uom.Private = true;
                    ServiceResult uomSr = uomm.Insert(uom);
                    if (uomSr.Code != 200)
                        return uomSr;
                }
                d.UnitOfMeasure = uom.UUID;
            }

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<DoseLogForm, DoseLog>();
            });

            IMapper mapper = config.CreateMapper();
            var dest = mapper.Map<DoseLogForm, DoseLog>(d);

            DoseManager DoseManager = new DoseManager(Globals.DBConnectionKey,Request.Headers?.Authorization?.Parameter);
            ServiceResult sr = DoseManager.Insert(dest,false);
            if (sr.Code != 200)
                return sr;

            SymptomManager sm = new SymptomManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);
            string symptomErrors = "";
           
            int index = 1;
            foreach (SymptomLog s in d.Symptoms)
            {
                if (string.IsNullOrWhiteSpace(s.UUID)) {
                    symptomErrors += "Symptom " + index + " UUID must be set! <br/>";
                    Debug.Assert(false, "SYMPTOM UUID MUST BE SET!!");
                    continue;
                }

                Symptom stmp = (Symptom)sm.Get(s.UUID);

                if (stmp == null)
                    stmp = (Symptom)sm.Get(s.UUID);

                if (stmp == null)
                {
                    symptomErrors += "Symptom " + s.UUID + " could not be found! <br/>";
                    continue;
                }
                s.Name = stmp.Name;

                if(s.SymptomDate == null || s.SymptomDate == DateTime.MinValue)
                {
                    symptomErrors += "Symptom " + s.UUID + " date must be set! <br/>";
                    continue;
                }
                //s.Status
                s.AccountUUID = CurrentUser.AccountUUID;
                s.Active = true;
                s.CreatedBy = CurrentUser.UUID;
                s.DateCreated = DateTime.UtcNow;
                s.Deleted = false;
                s.DoseUUID = dest.UUID;
                s.Private = true;
                ServiceResult slSr = sm.Insert(s, false);

                if(slSr.Code != 200)
                    symptomErrors += "Symptom " + index + " failed to save. " + slSr.Message;
                index++;
            }
            return ServiceResponse.OK("",dest);
        }

        [ApiAuthorizationRequired(Operator =">=" , RoleWeight = 4)]
        [HttpPost]
        [HttpGet]
        [Route("api/DoseLogs/")]
        public ServiceResult GetLogs(string filter = "")
        {
            if (Request.Headers.Authorization == null || string.IsNullOrWhiteSpace(Request.Headers?.Authorization?.Parameter))
                return ServiceResponse.Error("You must be logged in to access this functionality.");

            if (CurrentUser == null)
                return ServiceResponse.Error("You must be logged in to access this function.");

            int count=0;
            DoseManager DoseManager = new DoseManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);
            List<dynamic> Doses = DoseManager.GetDoses(CurrentUser.AccountUUID).Cast<dynamic>().ToList();

            if (!string.IsNullOrWhiteSpace(filter))
            {
                            DataFilter tmpFilter = this.GetFilter(filter);
                Doses = FilterEx.FilterInput(Doses, tmpFilter, out count);
           
                 //todo move the code below to the filter input
                string sortField = tmpFilter.SortBy?.ToUpper();
                string sortDirection = tmpFilter.SortDirection?.ToUpper();
            
                if (sortDirection  == "ASC")
                {
                    switch (sortField)
                    {
                        case "DOSEDATETIME":
                            Doses = Doses.OrderBy(uob => uob.DoseDateTime).ToList();
                            break;
                        case "NAME":
                            Doses = Doses.OrderBy(uob => uob.Name).ToList();
                            break;
                    }
                }
                else {
                    switch (sortField)
                    {
                        case "DOSEDATETIME":
                            Doses = Doses.OrderByDescending(uob => uob.DoseDateTime).ToList();
                            break;
                        case "NAME":
                            Doses = Doses.OrderByDescending(uob => uob.Name).ToList();
                            break;
                    }
                }
            }
         
            return ServiceResponse.OK("", Doses, count);
        }


        [ApiAuthorizationRequired(Operator = ">=", RoleWeight = 4)]
        [HttpPost]
        [HttpGet]
        [Route("api/DoseLogs/{name}")]
        public ServiceResult Get(string name )
        {
            if (Request.Headers.Authorization == null || string.IsNullOrWhiteSpace(Request.Headers?.Authorization?.Parameter))
                return ServiceResponse.Error("You must be logged in to access this functionality.");

            if (CurrentUser == null)
                return ServiceResponse.Error("You must be logged in to access this function.");

            int count = 0;
            DoseManager DoseManager = new DoseManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);
            List<DoseLog> s = DoseManager.Search(name);
                return ServiceResponse.OK("", s, count);
        }

        [ApiAuthorizationRequired(Operator = ">=", RoleWeight = 4)]
        [HttpPost]
        [HttpGet]
        [Route("api/DoseLogsBy/{uuid}")]
        public ServiceResult GetBy(string uuid)
        {
            if (Request.Headers.Authorization == null || string.IsNullOrWhiteSpace(Request.Headers?.Authorization?.Parameter))
                return ServiceResponse.Error("You must be logged in to access this functionality.");

            if (CurrentUser == null)
                return ServiceResponse.Error("You must be logged in to access this function.");

            int count = 0;
            DoseManager DoseManager = new DoseManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);
            DoseLog d = (DoseLog)DoseManager.Get(uuid);

            return ServiceResponse.OK("", d, count);
        }

        [ApiAuthorizationRequired(Operator = ">=", RoleWeight = 4)]
        [HttpPost]
        [HttpDelete]
        [Route("api/DoseLogs/Delete")]
        public ServiceResult Delete(DoseLog n)
        {
            if (n == null || string.IsNullOrWhiteSpace(n.UUID))
                return ServiceResponse.Error("Invalid account was sent.");

            DoseManager DoseManager = new DoseManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);

            return DoseManager.Delete(n);

        }

        [ApiAuthorizationRequired(Operator = ">=", RoleWeight = 4)]
        [HttpPost]
        [HttpPatch]
        [Route("api/DoseLogs/Update")]
        public ServiceResult Update(DoseLog form)
        {
            if (form == null)
                return ServiceResponse.Error("Invalid Strain sent to server.");

            DoseManager DoseManager = new DoseManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);

            var dbS = (DoseLog)DoseManager.Get(form.UUID);

            if (dbS == null)
                return ServiceResponse.Error("Strain was not found.");

            if (dbS.DateCreated == DateTime.MinValue)
                dbS.DateCreated = DateTime.UtcNow;

            dbS.Name = form.Name;
            dbS.Deleted = form.Deleted;
            dbS.Status = form.Status;
            dbS.SortOrder = form.SortOrder;
            return DoseManager.Update(dbS);
        }
    }
}
