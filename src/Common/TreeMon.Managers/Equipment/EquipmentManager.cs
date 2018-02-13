// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TreeMon.Data;
using TreeMon.Data.Logging;
using TreeMon.Models.App;
using TreeMon.Models.Equipment;

namespace TreeMon.Managers.Equipment
{
    public class EquipmentManager : BaseManager
    {
       
        private readonly SystemLogger _logger;

        public EquipmentManager(string connectionKey, string sessionKey) : base(connectionKey, sessionKey)
        {
            Debug.Assert(!string.IsNullOrWhiteSpace(connectionKey), "EquipmentManager CONTEXT IS NULL!");

            this._connectionKey = connectionKey;

            _logger = new SystemLogger(connectionKey);
        }

        public List<dynamic> GetAll(string type = "")
        {
            List<dynamic> res = new List<dynamic>();

            if (string.IsNullOrWhiteSpace(type))
                return res;

            if (this._requestingUser == null || string.IsNullOrWhiteSpace(this._requestingUser.AccountUUID))
                return res;

            if (type == "all") 
            {
                using (var context = new TreeMonDbContext(this._connectionKey))
                {
                    res.AddRange(context.GetAll<TreeMon.Models.Plant.Plant>().Where(w => w.AccountUUID == this._requestingUser.AccountUUID).Cast<dynamic>().ToList());
                    res.AddRange(context.GetAll<Ballast>().Where(w => w.AccountUUID == this._requestingUser.AccountUUID).Cast<dynamic>().ToList());
                    res.AddRange(context.GetAll<Bulb>().Where(w => w.AccountUUID == this._requestingUser.AccountUUID).Cast<dynamic>().ToList());
                    res.AddRange(context.GetAll<Fan>().Where(w => w.AccountUUID == this._requestingUser.AccountUUID).Cast<dynamic>().ToList());
                    res.AddRange(context.GetAll<Filter>().Where(w => w.AccountUUID == this._requestingUser.AccountUUID).Cast<dynamic>().ToList());
                    res.AddRange(context.GetAll<Pump>().Where(w => w.AccountUUID == this._requestingUser.AccountUUID).Cast<dynamic>().ToList());
                    res.AddRange(context.GetAll<Vehicle>().Where(w => w.AccountUUID == this._requestingUser.AccountUUID).Cast<dynamic>().ToList());
                }
                
                return res;
            }

            return this.GetEquipment(type).Where(w => w.AccountUUID == this._requestingUser.AccountUUID).ToList();
        }


        protected List<dynamic> GetEquipment(string type)
        {
            List<dynamic> res = new List<dynamic>();
            if (string.IsNullOrWhiteSpace(type))
                return res;

            using (var context = new TreeMonDbContext(this._connectionKey))
            {
                switch (type.ToUpper())
                {
                    case "PLANT":
                        res = context.GetAll<TreeMon.Models.Plant.Plant>().Cast<dynamic>().ToList();
                        break;
                    case "BALLAST":
                        res = context.GetAll<Ballast>().Cast<dynamic>().ToList();
                        break;
                    case "BULB":
                        res = context.GetAll<Bulb>().Cast<dynamic>().ToList();
                        break;
               
                    case "FAN":
                        res = context.GetAll<Fan>().Cast<dynamic>().ToList();
                        break;
                    case "FILTER":
                        res = context.GetAll<Filter>().Cast<dynamic>().ToList();
                        break;
                    case "PUMP":
                        res = context.GetAll<Pump>().Cast<dynamic>().ToList();
                        break;
                    case "VEHICLE":
                        res = context.GetAll<Vehicle>().Cast<dynamic>().ToList();
                        break;
                }
            }
            return res;
        }


        public ServiceResult Update(dynamic p)
        {
            if (!this.DataAccessAuthorized(p, "PATCH", false)) return ServiceResponse.Error("You are not authorized this action.");

            ServiceResult res = ServiceResponse.OK();
            int recordUpdated = 0;

            using (var context = new TreeMonDbContext(this._connectionKey))
            {

                string type = p.UUIDType.ToString().ToUpper();
                switch (type)
                {
                    case "PLANT":
                        recordUpdated = context.Update<TreeMon.Models.Plant.Plant>(p);
                        break;
                    case "BALLAST":
                        recordUpdated = context.Update<Ballast>(p);
                        break;
                    case "BULB":
                        recordUpdated = context.Update<Bulb>(p);
                        break;
                    case "CUSTOM":
                        recordUpdated = context.Update<Custom>(p);
                        break;
                    case "FAN":
                        recordUpdated = context.Update<Fan>(p);
                        break;
                    case "FILTER":
                        recordUpdated = context.Update<Filter>(p);
                        break;
                    case "PUMP":
                        recordUpdated = context.Update<Pump>(p);
                        break;
                    case "VEHICLE":
                        recordUpdated = context.Update<Vehicle>(p);
                        break;
                }
            }
            if (recordUpdated == 0)
                return ServiceResponse.Error(p.Name + " failed to update. ");

            return res;
        }
   
    }
}
