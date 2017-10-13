// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using TreeMon.Data.Logging.Models;
using TreeMon.Managers.General;
using TreeMon.Models.App;
using TreeMon.Models.Datasets;
using TreeMon.Models.General;
using TreeMon.Utilites.Extensions;
using TreeMon.Web.Filters;


namespace TreeMon.Web.api.v1
{
    public class CategoriesController : ApiBaseController 
    {
        public CategoriesController()
        {
          
        }

        [ApiAuthorizationRequired(Operator = ">=", RoleWeight = 4)]
        [HttpPost]
        [Route("api/Categories/Add")]
        [Route("api/Categories/Insert")]
        public ServiceResult Insert(Category n)
        {

            if (CurrentUser == null)
                return ServiceResponse.Error("You must be logged in to access this function.");


            if (string.IsNullOrWhiteSpace(n.AccountUUID) || n.AccountUUID == SystemFlag.Default.Account)
                n.AccountUUID = CurrentUser.AccountUUID;

            if (string.IsNullOrWhiteSpace(n.CreatedBy))
                n.CreatedBy = CurrentUser.UUID;

            if (n.DateCreated == DateTime.MinValue)
                n.DateCreated = DateTime.Now;

            CategoryManager CategoryManager = new CategoryManager(Globals.DBConnectionKey,Request.Headers?.Authorization?.Parameter);
            return CategoryManager.Insert(n, true);
        }

        [ApiAuthorizationRequired(Operator = ">=", RoleWeight = 1)]
        [HttpPost]
        [HttpGet]
        [Route("api/Categories/{name}/{type}")]
        public ServiceResult GetCategory(string name = "", string type = "")
        {
            if (string.IsNullOrWhiteSpace(name))
                return ServiceResponse.Error("You must provide a name for the Category.");

            if (string.IsNullOrWhiteSpace(type))
                return ServiceResponse.Error("You must provide a type for the Category.");


            if (CurrentUser == null)
                return ServiceResponse.Error("You must be logged in to access this function.");

            CategoryManager CategoryManager = new CategoryManager(Globals.DBConnectionKey,Request.Headers?.Authorization?.Parameter);
            Category s = (Category)CategoryManager.GetCategory(name, type, CurrentUser.AccountUUID);

            if (s == null)
                return ServiceResponse.Error("Category could not be located for the name " + name);

            return ServiceResponse.OK("",s);
        }

        [ApiAuthorizationRequired(Operator = ">=", RoleWeight = 1)]
        [HttpPost]
        [HttpGet]
        [Route("api/Categories/{name}")]
        public ServiceResult Get(string name )
        {
            if (string.IsNullOrWhiteSpace(name))
                return ServiceResponse.Error("You must provide a name for the Category.");
          
            if (CurrentUser == null)
                return ServiceResponse.Error("You must be logged in to access this function.");

            CategoryManager CategoryManager = new CategoryManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);
            Category s = (Category)CategoryManager.Get(name);

            if (s == null)
                return ServiceResponse.Error("Category could not be located for the name " + name);

            return ServiceResponse.OK("", s);
        }

        [ApiAuthorizationRequired(Operator = ">=", RoleWeight = 1)]
        [HttpPost]
        [HttpGet]
        [Route("api/CategoryBy/{uuid}")]
        public ServiceResult GetBy(string uuid )
        {
            if (string.IsNullOrWhiteSpace(uuid))
                return ServiceResponse.Error("You must provide a UUID for the Category.");

            if (CurrentUser == null)
                return ServiceResponse.Error("You must be logged in to access this function.");

            CategoryManager CategoryManager = new CategoryManager(Globals.DBConnectionKey,Request.Headers?.Authorization?.Parameter);
            Category category = (Category)CategoryManager.GetBy(uuid);

            if(category == null)
                return ServiceResponse.Error("Invalid category uuid" + uuid);

            if (CurrentUser.AccountUUID != category.AccountUUID)
                return ServiceResponse.Error("You are not authorized to access this functionality.");
   
            return ServiceResponse.OK("",category);
        }


        [ApiAuthorizationRequired(Operator =">=" , RoleWeight = 4)]
        [HttpPost]
        [HttpGet]
        [Route("api/Categories")]
        public ServiceResult GetCategories(string filter = "")
        {
            if (CurrentUser == null)
                return ServiceResponse.Error("You must be logged in to access this function.");

            CategoryManager CategoryManager = new CategoryManager(Globals.DBConnectionKey,Request.Headers?.Authorization?.Parameter);
            List<dynamic> Categorys = (List<dynamic>)CategoryManager.GetCategories(CurrentUser.AccountUUID, false, true).Cast<dynamic>().ToList();
            int count;
            DataFilter tmpFilter = this.GetFilter(filter);
            Categorys = FilterEx.FilterInput(Categorys, tmpFilter, out count);
            return ServiceResponse.OK("", Categorys, count);
        }


        [ApiAuthorizationRequired(Operator =">=" , RoleWeight = 4)]
        [HttpPost]
        [HttpDelete]
        [Route("api/Categories/Delete")]
        public ServiceResult Delete(Category n)
        {
            if (n == null || string.IsNullOrWhiteSpace(n.UUID))
                return ServiceResponse.Error("Invalid account was sent.");

            CategoryManager CategoryManager = new CategoryManager(Globals.DBConnectionKey,Request.Headers?.Authorization?.Parameter);
            return CategoryManager.Delete(n);
        }

        [ApiAuthorizationRequired(Operator = ">=", RoleWeight = 4)]
        [HttpPost]
        [HttpDelete]
        [Route("api/Categories/Delete/{categoryUUID}")]
        public ServiceResult Delete(string categoryUUID)
        {
            if ( string.IsNullOrWhiteSpace(categoryUUID))
                return ServiceResponse.Error("Invalid category was sent.");
            CategoryManager CategoryManager = new CategoryManager(Globals.DBConnectionKey,Request.Headers?.Authorization?.Parameter);
            return CategoryManager.Delete(CategoryManager.GetBy(categoryUUID));
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
        [Route("api/Categories/Update")]
        public ServiceResult Update(Category s)
        {
            if (s == null)
                return ServiceResponse.Error("Invalid Category sent to server.");

            CategoryManager CategoryManager = new CategoryManager(Globals.DBConnectionKey,Request.Headers?.Authorization?.Parameter);
            var dbS = (Category)CategoryManager.GetBy(s.UUID);

            if (dbS == null)
                return ServiceResponse.Error("Category was not found.");

            if (dbS.DateCreated == DateTime.MinValue)
                dbS.DateCreated = DateTime.Now;

            dbS.Deleted = s.Deleted;
            dbS.Name = s.Name;
            dbS.Status = s.Status;
            dbS.SortOrder = s.SortOrder;
            dbS.UsesStrains = s.UsesStrains;
            dbS.CategoryType = s.CategoryType;

            return CategoryManager.Update(dbS);
        }
    }
}
