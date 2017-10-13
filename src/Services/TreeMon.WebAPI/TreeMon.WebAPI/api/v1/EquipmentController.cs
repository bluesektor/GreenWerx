// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using TreeMon.Managers.Equipment;
using TreeMon.Models.App;
using TreeMon.Models.Datasets;
using TreeMon.Utilites.Extensions;
using TreeMon.Web;
using TreeMon.Web.api;
using TreeMon.Web.Filters;

namespace TreeMon.WebAPI.api.v1
{
    public class EquipmentController : ApiBaseController
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type">This is the class int the Equipment folder and should be the field UUIDType.. Ballast, bulb, vehicle...</param>
        /// <param name="filter"></param>
        /// <param name="startIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="sorting"></param>
        /// <returns></returns>
        [ApiAuthorizationRequired(Operator = ">=", RoleWeight = 4)]
        [HttpPost]
        [HttpGet]
        [Route("api/Equipment/Type/{type}")]
        public ServiceResult GetEquipment(string type = "", string filter = "")
        {
            EquipmentManager equipmentManager  = new  EquipmentManager( Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter );
            List<dynamic> Equipment = (List<dynamic>)equipmentManager.GetAll(type).Cast<dynamic>().ToList();

            int count;
            DataFilter tmpFilter = this.GetFilter(filter);
            Equipment = FilterEx.FilterInput(Equipment, tmpFilter, out count);
            return ServiceResponse.OK("", Equipment, count);
        }
    }
}
