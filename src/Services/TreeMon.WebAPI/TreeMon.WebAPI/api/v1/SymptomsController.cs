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
    public class SymptomsController : ApiBaseController
    {
        public SymptomsController()
        {
        }
        [ApiAuthorizationRequired(Operator = ">=", RoleWeight = 1)]
        [HttpPost]
        [Route("api/Symptoms/Add")]
        [Route("api/Symptoms/Insert")]
        public ServiceResult Insert(Symptom n)
        {
            if (CurrentUser == null)
                return ServiceResponse.Error("You must be logged in to access this function.");

            if (string.IsNullOrWhiteSpace(n.AccountUUID) || n.AccountUUID == SystemFlag.Default.Account)
                n.AccountUUID = CurrentUser.AccountUUID;

            if (string.IsNullOrWhiteSpace(n.CreatedBy))
                n.CreatedBy = CurrentUser.UUID;

            if (n.DateCreated == DateTime.MinValue)
                n.DateCreated = DateTime.UtcNow;

            SymptomManager symptomManager = new SymptomManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);

            return symptomManager.Insert(n, true);
        }

        [ApiAuthorizationRequired(Operator = ">=", RoleWeight = 1)]
        [HttpPost]
        [HttpGet]
        [Route("api/Symptom/{name}")]
        public ServiceResult Get( string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return ServiceResponse.Error("You must provide a name for the Symptom.");

            SymptomManager symptomManager = new SymptomManager(Globals.DBConnectionKey,Request.Headers?.Authorization?.Parameter);

            Symptom s = (Symptom) symptomManager.Get(name);

            if (s == null)
                return ServiceResponse.Error("Symptom could not be located for the name " + name);

            return ServiceResponse.OK("",s);
        }

        [ApiAuthorizationRequired(Operator = ">=", RoleWeight = 1)]
        [HttpPost]
        [HttpGet]
        [Route("api/SymptomBy/{uuid}")]
        public ServiceResult GetBy(string uuid)
        {
            if (string.IsNullOrWhiteSpace(uuid))
                return ServiceResponse.Error("You must provide a name for the Symptom.");

            SymptomManager symptomManager = new SymptomManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);

            Symptom s = (Symptom)symptomManager.Get(uuid);

            if (s == null)
                return ServiceResponse.Error("Symptom could not be located for the uuid " + uuid);

            return ServiceResponse.OK("", s);
        }


        [ApiAuthorizationRequired(Operator =">=" , RoleWeight = 4)]
        [HttpPost]
        [HttpGet]
        [Route("api/Symptoms/")]
        public ServiceResult GetSymptoms(string filter = "")
        {
            if (CurrentUser == null)
                return ServiceResponse.Error("You must be logged in to access this function.");
           
            DataFilter tmpFilter = this.GetFilter(filter);
            SymptomManager symptomManager = new SymptomManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);

            List<Symptom> Symptoms = symptomManager.GetSymptoms(CurrentUser.AccountUUID);
            int count = 0;

            if (tmpFilter != null)
            {
                foreach (DataScreen f in tmpFilter.Screens)
                {
                    if ( f == null || string.IsNullOrWhiteSpace(f.Field) || string.IsNullOrWhiteSpace(f.Field))
                        continue;

                    switch (f.Field?.ToUpper())
                    {
                        case "NAME":
                            Symptoms = Symptoms.Where(w => (w.Name?.EqualsIgnoreCase(f.Value))??false).ToList();
                            break;

                        case "CATEGORY":
                            Symptoms = Symptoms.Where(w => ( w.Category?.EqualsIgnoreCase(f.Value)??false )).ToList();
                            break;
                    }
                }

                count = Symptoms.Count;

                Symptoms = Symptoms.Paginate<Symptom>(tmpFilter.StartIndex, tmpFilter.PageSize);
                return ServiceResponse.OK("", Symptoms, count);
            }

            count = Symptoms.Count;
            Symptoms = Symptoms.Paginate<Symptom>(tmpFilter?.StartIndex, tmpFilter?.PageSize);
            return ServiceResponse.OK("", Symptoms, count);
        }

        [ApiAuthorizationRequired(Operator =">=" , RoleWeight = 4)]
        [HttpPost]
        [HttpDelete]
        [Route("api/Symptoms/Delete")]
        public ServiceResult Delete(Symptom n)
        {
            if (n == null || string.IsNullOrWhiteSpace(n.UUID))
                return ServiceResponse.Error("Invalid account was sent.");

            SymptomManager symptomManager = new SymptomManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);

            return symptomManager.Delete(n);
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
        [Route("api/Symptoms/Update")]
        public ServiceResult Update(Symptom s)
        {
            if (s == null)
                return ServiceResponse.Error("Invalid Symptom sent to server.");
 
            SymptomManager symptomManager = new SymptomManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);

            var dbS = (Symptom)symptomManager.GetBy(s.UUID);

            if (dbS == null)
                return ServiceResponse.Error("Symptom was not found.");

            if (dbS.DateCreated == DateTime.MinValue)
                dbS.DateCreated = DateTime.UtcNow;
            dbS.Deleted = s.Deleted;
            dbS.Name = s.Name;
            dbS.Status = s.Status;
            dbS.SortOrder = s.SortOrder;

            return symptomManager.Update(dbS);
        }



        #region SymptomLog



        [ApiAuthorizationRequired(Operator = ">=", RoleWeight = 1)]
        [HttpPost]
        [Route("api/SymptomsLog/Add")]
        public ServiceResult AddSymptomLog(SymptomLog s)
        {
            if (CurrentUser == null)
                return ServiceResponse.Error("You must be logged in to access this function.");

            if (string.IsNullOrWhiteSpace(s.AccountUUID) || s.AccountUUID == SystemFlag.Default.Account)
                s.AccountUUID = CurrentUser.AccountUUID;

            if (string.IsNullOrWhiteSpace(s.CreatedBy))
                s.CreatedBy = CurrentUser.UUID;

            SymptomManager symptomManager = new SymptomManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);

            if (!string.IsNullOrWhiteSpace(s.UUParentID)){
                SymptomLog parentLog = symptomManager.GetSymptomLogBy(s.UUParentID);
                if(parentLog == null)
                    return ServiceResponse.Error("Invalid log parent id. The UUParentID must belong to a valid Symptom Log entry.");

                s.DoseUUID = parentLog.DoseUUID;
                if (string.IsNullOrWhiteSpace(s.UUParentIDType))
                    s.UUParentIDType = parentLog.UUIDType;
            }

            s.Active = true;

            if (string.IsNullOrWhiteSpace(s.UUIDType))
                s.UUIDType = "SymptomLog";



            //backlog go to dosehistory selsect a dose and in the symptomps list
            //click add . fill in the data and click
            //VERIFY the fields here.
            //rules for parent list symptom creation
            //Name <- may need to create
            //SymptomUUID <- may need to create
            if (string.IsNullOrWhiteSpace(s.Name) && string.IsNullOrWhiteSpace(s.SymptomUUID))
            {
                return ServiceResponse.Error("You must select a symptom.");
            }
            else if (string.IsNullOrWhiteSpace(s.Name) && string.IsNullOrWhiteSpace(s.SymptomUUID) == false)
            {   //get and assign the name
                Symptom symptom = (Symptom)symptomManager.GetBy(s.SymptomUUID);
                if(symptom == null)
                    return ServiceResponse.Error("Symptom could not be found for id " + s.SymptomUUID);

                s.Name = symptom.Name;
            }
            else if (!string.IsNullOrWhiteSpace(s.Name) && string.IsNullOrWhiteSpace(s.SymptomUUID) )
            {   //create the symptoms and assign it to the symptomuuid

                Symptom symptom  = (Symptom)symptomManager.Get(s.Name);

                if (symptom != null)
                {
                    s.SymptomUUID = symptom.UUID;
                }
                else {
                    symptom = new Symptom()
                    {
                        Name = s.Name,
                        AccountUUID = s.AccountUUID,
                        Active = true,
                        CreatedBy = CurrentUser.UUID,
                        DateCreated = DateTime.UtcNow,
                        Deleted = false,
                        UUIDType = "Symptom",
                        Category = "General"
                    };

                    ServiceResult sr = symptomManager.Insert(symptom, true);
                    if (sr.Code == 500)
                        return ServiceResponse.Error(sr.Message);

                    s.SymptomUUID = symptom.UUID;
                }
            }

            if (s.SymptomDate == null || s.SymptomDate == DateTime.MinValue)
                s.SymptomDate = DateTime.UtcNow;

            if (string.IsNullOrWhiteSpace(s.Status))//Status start middle end. Query StatusMessage table
                return ServiceResponse.Error("You must provide a status.");
            

            if (s.Severity > 5) return ServiceResponse.Error("Severity must not be greater than 5.");
            if (s.Efficacy > 5) return ServiceResponse.Error("Efficacy must not be greater than 5.");

            return symptomManager.Insert(s, false);
        }

        [ApiAuthorizationRequired(Operator = ">=", RoleWeight = 1)]
        [HttpPost]
        [HttpGet]
        [Route("api/SymptomLog/{name}")]
        public ServiceResult GetSymptomLog(string name = "")
        {
            if (string.IsNullOrWhiteSpace(name))
                return ServiceResponse.Error("You must provide a name for the SymptomLog.");

            SymptomManager symptomManager = new SymptomManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);

            SymptomLog s = symptomManager.GetSymptomLog(name);

            if (s == null)
                return ServiceResponse.Error("SymptomLog could not be located for the name " + name);

            return ServiceResponse.OK("",s);
        }


        //[ApiAuthorizationRequired(Operator =">=" , RoleWeight = 4)]
        [HttpPost]
        [HttpGet]
        [Route("api/SymptomsLog/{parentUUID}")]
        public ServiceResult GetSymptomLogs(string parentUUID ="",string filter = "")
        {
            if (CurrentUser == null)
                return ServiceResponse.Error("You must be logged in to access this function.");

            List<dynamic> SymptomsLog;

            SymptomManager symptomManager = new SymptomManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);
            SymptomsLog = symptomManager.GetSymptomsLog(parentUUID, CurrentUser.AccountUUID).Cast<dynamic>().ToList(); ;

            int count;

            DataFilter tmpFilter = this.GetFilter(filter);
            SymptomsLog = FilterEx.FilterInput(SymptomsLog, tmpFilter, out count);
            return ServiceResponse.OK("", SymptomsLog, count);
        }


        //[ApiAuthorizationRequired(Operator =">=" , RoleWeight = 4)]
        [HttpPost]
        [HttpGet]
        [Route("api/Doses/{doseUUID}/SymptomsLog/History/{parentUUID}")]
        public ServiceResult GetChildSymptomLogs(string doseUUID, string parentUUID = "", string filter = "")
        {
            if (CurrentUser == null)
                return ServiceResponse.Error("You must be logged in to access this function.");

            if (string.IsNullOrWhiteSpace(doseUUID))
                return ServiceResponse.Error("You must send a dose uuid.");
           
            SymptomManager symptomManager = new SymptomManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);

            List<dynamic> SymptomsLog = symptomManager.GetSymptomsByDose(doseUUID, parentUUID, CurrentUser.AccountUUID).Cast<dynamic>().ToList();

            int count;
            DataFilter tmpFilter = this.GetFilter(filter);
            SymptomsLog = FilterEx.FilterInput(SymptomsLog, tmpFilter, out count);
            return ServiceResponse.OK("", SymptomsLog, count);
        }


        //[ApiAuthorizationRequired(Operator =">=" , RoleWeight = 4)]
        [HttpPost]
        [HttpGet]
        [Route("api/Doses/{doseUUID}/SymptomsLog/")]
        public ServiceResult GetSymptomsLogByDose(string doseUUID, string filter = "")
        {
            if (CurrentUser == null)
                return ServiceResponse.Error("You must be logged in to access this function.");


            if (string.IsNullOrWhiteSpace(doseUUID))
                return ServiceResponse.Error("You must send a dose uuid.");

            List<dynamic> SymptomsLog;
           
            SymptomManager symptomManager = new SymptomManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);

            SymptomsLog = symptomManager.GetSymptomsByDose(doseUUID,"", CurrentUser.AccountUUID).Cast<dynamic>().ToList();
            int count;
            DataFilter tmpFilter = this.GetFilter(filter);
            SymptomsLog = FilterEx.FilterInput(SymptomsLog, tmpFilter, out count);
            return ServiceResponse.OK("", SymptomsLog, count);
        }



        //  [ApiAuthorizationRequired(Operator =">=" , RoleWeight = 4)]
        [HttpPost]
        [Route("api/SymptomsLog/Delete")]
        public ServiceResult Delete(SymptomLog s)
        {
            if (s == null || string.IsNullOrWhiteSpace(s.UUID))
                return ServiceResponse.Error("Invalid account was sent.");

            SymptomManager symptomManager = new SymptomManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);

            return symptomManager.Delete(s);
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
        //  [ApiAuthorizationRequired(Operator =">=" , RoleWeight = 4)]
        [HttpPost]
        [HttpPatch]
        [Route("api/SymptomsLog/Update")]
        public ServiceResult Update(SymptomLog s)
        {
            if (s == null)
                return ServiceResponse.Error("Invalid SymptomLog sent to server.");

            SymptomManager symptomManager = new SymptomManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);

            var dbS = symptomManager.GetSymptomLogBy(s.UUID);

            if (dbS == null)
                return ServiceResponse.Error("SymptomLog was not found.");

            if(s.Efficacy < -5 || s.Efficacy > 5)
                return ServiceResponse.Error("Efficacy is out of range.");

            if (CurrentUser == null)
                return ServiceResponse.Error("You must be logged in to access this function.");

            if (CurrentUser.UUID != dbS.CreatedBy)
                return ServiceResponse.Error("You are not authorized to change this item.");

            dbS.Name = s.Name;
            dbS.Status = s.Status;
            dbS.Duration = s.Duration;
            dbS.DurationMeasure = s.DurationMeasure;
            dbS.Efficacy = s.Efficacy;
            dbS.Severity = s.Severity;
            dbS.SymptomDate = s.SymptomDate;
            
            //test this.make sure date, status, severity, efficacy etc is copied over.

            return symptomManager.Update(dbS);
        }

        [ApiAuthorizationRequired(Operator = ">=", RoleWeight = 1)]
        [HttpPost]
        [HttpPatch]
        [Route("api/SymptomsLog/UpdateField")]
        public ServiceResult UpdateSymptomLogField(string symptomLogUUID, string fieldName, string fieldValue)
        {
            if (string.IsNullOrWhiteSpace(symptomLogUUID))
                return ServiceResponse.Error("You must provide a UUID.");

            if (string.IsNullOrWhiteSpace(fieldName))
                return ServiceResponse.Error("You must provide a field name.");

            if (string.IsNullOrWhiteSpace(fieldValue))
                return ServiceResponse.Error("You must provide a field value.");

            if (CurrentUser == null)
                return ServiceResponse.Error("You must be logged in to access this function.");

            SymptomManager symptomManager = new SymptomManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);

            SymptomLog sl = symptomManager.GetSymptomLogBy(symptomLogUUID);

            if (sl == null)
                return ServiceResponse.Error("Could not find the log item.");

            if (CurrentUser.UUID != sl.CreatedBy)
                return ServiceResponse.Error("You are not authorized to change this item.");

            bool success = false;
            fieldName = fieldName.ToLower();

            switch (fieldName)
            {
                case "duration":
                    sl.Duration = fieldValue.ConvertTo<float>(out success);
                    if (!success)
                        return ServiceResponse.Error("Invalid field value.");
                    break;

                case "durationmeasure":
                    sl.DurationMeasure = fieldValue;
                    break;
                default:
                    return ServiceResponse.Error("Field " + fieldName + " not supported.");
            }
            return symptomManager.Update(sl);
        }

        #endregion


    }
}
