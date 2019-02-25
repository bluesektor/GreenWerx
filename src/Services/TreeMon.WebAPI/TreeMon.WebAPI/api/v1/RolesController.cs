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
using TreeMon.Managers.Membership;
using TreeMon.Models;
using TreeMon.Models.App;
using TreeMon.Models.Datasets;
using TreeMon.Models.Membership;
using TreeMon.Utilites.Extensions;
using TreeMon.Utilites.Helpers;
using TreeMon.Web.Filters;
using TreeMon.WebAPI.Models;
using WebApi.OutputCache.V2;

namespace TreeMon.Web.api.v1
{
    [CacheOutput(ClientTimeSpan = 100, ServerTimeSpan = 100)]
    public class RolesController : ApiBaseController
    {
        public RolesController()
        {

        }

        /// <summary>
        /// This takes a json array of permissions as input and adds them to the RolePermissions.
        /// e.g. [{ PermissionUUID: pXXX, AccountUUID: aXXX, RoleUUID: rXXX },{ PermissionUUID: pYYY, AccountUUID: aYYY, RoleUUID: rYYY }]
        /// </summary>
        /// <returns></returns>
        [ApiAuthorizationRequired(Operator =">=" , RoleWeight = 4)]
        [HttpPost]
        [Route("api/Roles/{roleUUID}/Permissions/Add")]
        public ServiceResult AddPermissionsToRole(string roleUUID )
        {
            try
            {
                Task<string> content = ActionContext.Request.Content.ReadAsStringAsync();
                if (content == null)
                    return ServiceResponse.Error("No permissions were sent.");

                string body = content.Result;

                if (string.IsNullOrEmpty(body))
                    return ServiceResponse.Error("No permissions were sent.");

                List<Permission> perms = JsonConvert.DeserializeObject<List<Permission>>(body);

                RoleManager roleManager = new RoleManager(Globals.DBConnectionKey, this.GetUser(Request.Headers?.Authorization?.Parameter));
                return roleManager.AddPermisssionsToRole(roleUUID, perms, CurrentUser);
                    
            }
            catch (Exception ex)
            {
                Debug.Assert(false, ex.Message);
            }
            return ServiceResponse.OK();
        }

        [ApiAuthorizationRequired(Operator =">=" , RoleWeight = 4)]
        [HttpPost]
        [Route("api/Roles/Add")]
        [Route("api/Roles/Insert")]
        public ServiceResult Insert(Role n)
        {
            if (CurrentUser == null)
                return ServiceResponse.Error("You must be logged in to access this function.");


            if (string.IsNullOrWhiteSpace(n.AccountUUID))
                n.AccountUUID = CurrentUser.AccountUUID;

            n.CreatedBy = CurrentUser.UUID;
            n.DateCreated = DateTime.UtcNow;

            RoleManager roleManager = new RoleManager(Globals.DBConnectionKey, CurrentUser);
            return roleManager.Insert(n);
        }

        /// <summary>
        /// This takes a json array of users as input and adds them to the RolePermissions.
        /// e.g. [{ PermissionUUID: pXXX, AccountUUID: aXXX, RoleUUID: rXXX },{ PermissionUUID: pYYY, AccountUUID: aYYY, RoleUUID: rYYY }]
        /// </summary>
        /// <returns></returns>
        [ApiAuthorizationRequired(Operator =">=" , RoleWeight = 4)]
        [HttpPost]
        [Route("api/Roles/{roleUUID}/Users/Add")]
        public ServiceResult AddUsersToRole(string roleUUID)
        {
            ServiceResult res;

            try
            {
                Task<string> content = ActionContext.Request.Content.ReadAsStringAsync();
                if (content == null)
                    return ServiceResponse.Error("No users were sent.");

                string body = content.Result;

                if (string.IsNullOrEmpty(body))
                    return ServiceResponse.Error("No users were sent.");

                List<User> urs = JsonConvert.DeserializeObject<List<User>>(body);
                RoleManager roleManager = new RoleManager(Globals.DBConnectionKey, this.GetUser(Request.Headers?.Authorization?.Parameter));
                res =   roleManager.AddUsersToRole(roleUUID,urs, CurrentUser);
            }
            catch (Exception ex)
            {
                Debug.Assert(false, ex.Message);
                return ServiceResponse.Error(ex.Message);
            }
            return res;
        }

        /// <summary>
        /// This takes a json array of permissions as input and removes them to the RolePermissions.
        /// e.g. [{ PermissionUUID: pXXX, AccountUUID: aXXX, RoleUUID: rXXX },{ PermissionUUID: pYYY, AccountUUID: aYYY, RoleUUID: rYYY }]
        /// </summary>
        /// <returns></returns>
        [ApiAuthorizationRequired(Operator =">=" , RoleWeight = 4)]
        [HttpPost]
        [HttpDelete]
        [Route("api/Roles/{roleUUID}/Permissions/Delete")]
        public ServiceResult DeletePermissionsFromRole(string roleUUID)
        {
            try
            {
                Task<string> content = ActionContext.Request.Content.ReadAsStringAsync();
                if (content == null)
                    return ServiceResponse.Error("No permissions were sent.");

                string body = content.Result;

                if (string.IsNullOrEmpty(body))
                    return ServiceResponse.Error("No permissions were sent.");

                RoleManager roleManager = new RoleManager(Globals.DBConnectionKey, this.GetUser(Request.Headers?.Authorization?.Parameter));
                List<Permission> perms = JsonConvert.DeserializeObject<List<Permission>>(body);
                roleManager.DeletePermissionsFromRole(roleUUID, perms, CurrentUser);
            }
            catch (Exception ex)
            {
                Debug.Assert(false, ex.Message);
            }
            return ServiceResponse.OK();
        }

        [ApiAuthorizationRequired(Operator =">=" , RoleWeight = 4)]
        [HttpPost]
        [HttpDelete]
        [Route("api/Roles/Delete")]
        public ServiceResult Delete(Role n)
        {
            if (CurrentUser == null)
                return ServiceResponse.Error("You must be logged in to access this function.");

            RoleManager roleManager = new RoleManager(Globals.DBConnectionKey, CurrentUser);

            Role dbRole = (Role) roleManager.Get(n.UUID);

            if (dbRole == null)
                return ServiceResponse.Error("Invalid role.");

            return roleManager.Delete(dbRole);
        }

        [ApiAuthorizationRequired(Operator = ">=", RoleWeight = 4)]
        [HttpPost]
        [HttpDelete]
        [Route("api/Roles/Delete/{uuid}")]
        public ServiceResult Delete(string uuid)
        {
            if (string.IsNullOrWhiteSpace(uuid))
                return ServiceResponse.Error("No uuid sent.");

            if (CurrentUser == null)
                return ServiceResponse.Error("You must be logged in to access this function.");

            RoleManager roleManager = new RoleManager(Globals.DBConnectionKey, CurrentUser);

            Role dbRole = (Role)roleManager.Get(uuid);

            if (dbRole == null)
                return ServiceResponse.Error("Invalid role.");

            return roleManager.Delete(dbRole);
        }

        /// <summary>
        /// This takes a json array of permissions as input and removes them to the RolePermissions.
        /// e.g. [{ PermissionUUID: pXXX, AccountUUID: aXXX, RoleUUID: rXXX },{ PermissionUUID: pYYY, AccountUUID: aYYY, RoleUUID: rYYY }]
        /// </summary>
        /// <returns></returns>
        [ApiAuthorizationRequired(Operator =">=" , RoleWeight = 4)]
        [HttpPost]
        [HttpDelete]
        [Route("api/Roles/{roleUUID}/Users/Remove")]
        public ServiceResult DeleteUsersFromRole(string roleUUID)
        {
            ServiceResult res;
            try
            {
                Task<string> content = ActionContext.Request.Content.ReadAsStringAsync();
                if (content == null)
                    return ServiceResponse.Error("No users were sent.");

                string body = content.Result;

                if (string.IsNullOrEmpty(body))
                    return ServiceResponse.Error("No users were sent.");

                RoleManager roleManager = new RoleManager(Globals.DBConnectionKey, this.GetUser(Request.Headers?.Authorization?.Parameter));
                List<User> users= JsonConvert.DeserializeObject<List<User>>(body);
               res =    roleManager.DeleteUsersFromRole(roleUUID, users, CurrentUser);
            }
            catch (Exception ex)
            {
                Debug.Assert(false, ex.Message);
                return ServiceResponse.Error(ex.Message);
            }
            return res;
        }

        /// <summary>
        /// This returns all permissions for the role and 
        /// </summary>
        /// <param name="roleUUID"></param>
        /// <param name="accountUUID"></param>
        /// <param name="filter"></param>
        /// <param name="startIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="sorting"></param>
        /// <returns></returns>
        [ApiAuthorizationRequired(Operator =">=" , RoleWeight = 4)]
        [HttpPost]
        [HttpGet]
        [Route("api/Roles/{roleUUID}/Permissions/Unassigned")]
        public ServiceResult GetUnassignedPermissionsForRole(string roleUUID)
        {
            RoleManager rm = new RoleManager(Globals.DBConnectionKey, this.GetUser(Request.Headers?.Authorization?.Parameter));
            List<dynamic> availablePerms =rm.GetAvailablePermissions(roleUUID, CurrentUser.AccountUUID).OrderBy(ob => ob.Name).Cast<dynamic>().ToList();
            int count;

                             DataFilter tmpFilter = this.GetFilter(Request);
                availablePerms = availablePerms.Filter( tmpFilter, out count);

            return ServiceResponse.OK("", availablePerms, count);

        }

        [ApiAuthorizationRequired(Operator =">=" , RoleWeight = 4)]
        [HttpPost]
        [HttpGet]
        [Route("api/Roles/{roleUUID}/Permissions")]
        public ServiceResult GetPermissionsForRole(string roleUUID)
        {
            if (CurrentUser == null)
                return ServiceResponse.Error("You must be logged in to access this function.");

            RoleManager rm = new RoleManager(Globals.DBConnectionKey, CurrentUser);
            List<dynamic> permissions = rm.GetPermissionsForRole(roleUUID, CurrentUser.AccountUUID).Cast<dynamic>().ToList();
            int count;

                             DataFilter tmpFilter = this.GetFilter(Request);
                permissions = permissions.Filter( tmpFilter, out count);

            return ServiceResponse.OK("", permissions, count);
        }


        [ApiAuthorizationRequired(Operator =">=" , RoleWeight = 4)]
        [HttpPost]
        [HttpGet]
        [Route("api/Roles/{name}")]
        public ServiceResult Search(string name )
        {
            if (string.IsNullOrWhiteSpace(name))
                return ServiceResponse.Error("You must provide a name for the role.");

            if (CurrentUser == null)
                return ServiceResponse.Error("You must be logged in to access this function.");



            RoleManager roleManager = new RoleManager(Globals.DBConnectionKey, this.GetUser(Request.Headers?.Authorization?.Parameter));

            List<Role> s = roleManager.Search(name);

            if (s == null || s.Count == 0)
                return ServiceResponse.Error("Role could not be located for the name " + name);

            return ServiceResponse.OK("",s);
        }

    /// <summary>
    /// NOTE: This is account specific.
    /// I based the decision to get the roles by account on this post
    /// http://programmers.stackexchange.com/questions/278864/role-based-rest-api
    /// So we're getting resources (roles) based on the account id.
    /// </summary>
    /// <param name="filter"></param>
    /// <param name="startIndex"></param>
    /// <param name="pageSize"></param>
    /// <param name="sorting"></param>
    /// <returns></returns>
        [ApiAuthorizationRequired(Operator =">=" , RoleWeight = 4)]
        [HttpPost]
        [HttpGet]
        [Route("api/Roles")]
        public ServiceResult GetRoles()
        {
            if (CurrentUser == null)
                return ServiceResponse.Error("You must be logged in to access this function.");

            RoleManager roleManager = new RoleManager(Globals.DBConnectionKey, this.GetUser(Request.Headers?.Authorization?.Parameter));

            List<dynamic> roles = roleManager.GetRoles(CurrentUser.AccountUUID).Cast<dynamic>().ToList();
            int count;
             DataFilter tmpFilter = this.GetFilter(Request);
            roles = roles.Filter( tmpFilter, out count);

            return ServiceResponse.OK("", roles, count);
        }

        [ApiAuthorizationRequired(Operator = ">=", RoleWeight = 4)]
        [HttpPost]
        [HttpGet]
        [Route("api/Roles/IsMember/{roleName}")]
        public ServiceResult IsUserInRole( string roleName)
        {
            RoleManager rm = new RoleManager(Globals.DBConnectionKey, this.GetUser(Request.Headers?.Authorization?.Parameter));
            bool isMember = rm.IsUserInRole(CurrentUser.UUID, roleName, CurrentUser.AccountUUID);
            return ServiceResponse.OK("", isMember);
        }


        [ApiAuthorizationRequired(Operator = ">=", RoleWeight = 4)]
        [HttpPost]
        [HttpGet]
        [Route("api/RolesBy/{uuid}")]
        public ServiceResult GetBy(string uuid)
        {
            if (CurrentUser == null)
                return ServiceResponse.Error("You must be logged in to access this function.");

            RoleManager roleManager = new RoleManager(Globals.DBConnectionKey, CurrentUser);

            Role  role= (Role)roleManager.Get(uuid);
         
            return ServiceResponse.OK("", role);
        }

        [ApiAuthorizationRequired(Operator = ">=", RoleWeight = 4)]
        [HttpGet]
        [Route("api/Roles/User")]
        public ServiceResult GetUserRoles()
        {
            if (CurrentUser == null)
                return ServiceResponse.Error("You must be logged in to access this function.");

            RoleManager roleManager = new RoleManager(Globals.DBConnectionKey, CurrentUser);

            List<Role> userRoles = roleManager.GetRolesForUser(CurrentUser.UUID, CurrentUser.AccountUUID);

            return ServiceResponse.OK("", userRoles);
        }


        /// <summary>
        /// if getUnassigned == true it will return
        /// users that are NOT in the rol
        /// </summary>
        /// <param name="roleUUID"></param>
        /// <param name="getUnassigned"></param>
        /// <returns></returns>
        [ApiAuthorizationRequired(Operator =">=" , RoleWeight = 4)]
        [HttpPost]
        [HttpGet]
        [Route("api/Roles/{roleUUID}/Users")]
        public ServiceResult GetUsersInRole(string roleUUID )
        {
            if (CurrentUser == null)
                return ServiceResponse.Error("You must be logged in to access this function.");


            RoleManager rm = new RoleManager(Globals.DBConnectionKey, CurrentUser);
         
            List<dynamic> usersInRole =rm.GetUsersInRole(roleUUID, CurrentUser.AccountUUID).Cast<dynamic>().ToList();
            int count;

             DataFilter tmpFilter = this.GetFilter(Request);
            usersInRole = usersInRole.Filter( tmpFilter, out count);

            return ServiceResponse.OK("", usersInRole, count);
        }

        [ApiAuthorizationRequired(Operator =">=" , RoleWeight = 4)]
        [HttpPost]
        [HttpGet]
        [Route("api/Roles/{roleUUID}/Users/Unassigned")]
        public ServiceResult GetUnassignedUsersForRole(string roleUUID)
        {
            RoleManager rm = new RoleManager(Globals.DBConnectionKey, CurrentUser);
     
            List<dynamic> availableUsers =rm.GetUsersNotInRole(roleUUID, CurrentUser.AccountUUID).OrderBy(ob => ob.Name).Cast<dynamic>().ToList();
         
            int count =0;
            DataFilter filter = GetFilter(Request);
            if (filter != null)
            {
                 DataFilter tmpFilter = this.GetFilter(Request);
                availableUsers = availableUsers.Filter( tmpFilter, out count);
            }

            return ServiceResponse.OK("", availableUsers, count);

        }

        /// <summary>
        ///Updated fields 
        ///     Name     
        ///     Private  
        ///     SortOrder
        ///     Active   
        ///     Deleted  
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        [ApiAuthorizationRequired(Operator =">=" , RoleWeight = 4)]
        [HttpPost]
        [HttpPatch]
        [Route("api/Roles/Update")]
        public ServiceResult Update(Role r)
        {
            if (string.IsNullOrWhiteSpace(r.UUID))
                Debug.Assert(false, "NO UUID FOR ROLE");

            RoleManager roleManager = new RoleManager(Globals.DBConnectionKey, this.GetUser(Request.Headers?.Authorization?.Parameter));

            Role dbRole = (Role)roleManager.Get( r.UUID );

            if (dbRole == null)
                return ServiceResponse.Error("Role was not found.");

            
            dbRole.Name      = r.Name;
            dbRole.Private   = r.Private;
            dbRole.SortOrder = r.SortOrder;
            dbRole.Active    = r.Active;
            dbRole.Deleted   = r.Deleted;
            
            return roleManager.Update(dbRole);
        }


        [ApiAuthorizationRequired(Operator =">=" , RoleWeight = 4)]
        [HttpGet]
        [Route("api/Roles/Clone/{roleUUID}")]
        public ServiceResult CloneRole(string roleUUID)
        {
            RoleManager rm = new RoleManager(Globals.DBConnectionKey, CurrentUser);
            return rm.CloneRole(roleUUID);
        }
    }
}
