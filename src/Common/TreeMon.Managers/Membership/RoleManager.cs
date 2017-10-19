// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
using AutoMapper;
using Dapper;
using MoreLinq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Transactions;
using TreeMon.Data;
using TreeMon.Data.Logging;
using TreeMon.Data.Logging.Models;
using TreeMon.Models;
using TreeMon.Models.App;
using TreeMon.Models.Membership;
using TreeMon.Utilites.Extensions;

namespace TreeMon.Managers.Membership
{
    public class RoleManager:ICrud //Can't derive from BaseManager
    {

        private readonly SystemLogger _logger = null;
        private readonly List<string> _siteAdmins = null;
        private readonly User _requestingUser = null;
        private readonly string _dbConnectionKey = null;
        private bool _runningInstall = false; //for bypassing the authorization when adding roles/permissions

        public RoleManager(string connectionKey)
        {
            _dbConnectionKey = connectionKey;
            _logger = new SystemLogger(_dbConnectionKey);
           
        }

        public RoleManager(string connectionKey, User requestingUser)
        {
            _dbConnectionKey = connectionKey;
            _logger = new SystemLogger(_dbConnectionKey);
            _requestingUser =requestingUser;
        }

        public RoleManager(string connectionKey, List<string> siteAdmins, User requestingUser)
        {
            _dbConnectionKey = connectionKey;
            _logger = new SystemLogger(_dbConnectionKey);
            _requestingUser = requestingUser;
            _siteAdmins = siteAdmins;
        }

        private RoleManager( ){ }

        public string CreateKey(string name, string action, string appType, string accountUUID)
        {
            return name.ToSafeString(true)?.ToLower() + "." + action.ToSafeString(true)?.ToLower() + "." + appType.ToSafeString(true)?.ToLower() + "." + accountUUID.ToSafeString(true);
        }

        public string NameFromPath(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return path;

            path = path.StripGuids('-');

            path = StringEx.ReplaceIncluding("%", "", path, "");//replace from % to end
            path = StringEx.ReplaceIncluding("?", "", path, "");

            path = path.Replace("/", ".");
            path = path.Replace("..", ".");

            if (path[0] == '.')
                path = path.Remove(0, 1);

            if (path[path.Length - 1] == '.')
                path = path.Remove(path.Length - 1, 1);

            return path;
        }


        #region Permission TODO refactor into icrud class   ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        /// <summary>
        /// note: Don't add DataAccessAuthorized to this. It's called from the Api to create permissions on the fly.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="action"></param>
        /// <param name="request"></param>
        /// <param name="appType"></param>
        /// <param name="AccountUUID"></param>
        /// <param name="weight"></param>
        /// <returns></returns>
        public ServiceResult CreatePermission(string name, string action, string request, string appType, string AccountUUID = SystemFlag.Default.Account, int weight = 0)
        {
            // if (!_runningInstall &&  !this.DataAccessAuthorized(p, _requestingUser,"POST", false)) return ServiceResponse.Error("You are not authorized this action.");

            name = StringEx.ReplaceIncluding("?", "", name, "");
            request = StringEx.ReplaceIncluding("?", "", request, "");
            name = StringEx.ReplaceIncluding("%", "", name, "");
            request = StringEx.ReplaceIncluding("%", "", request, "");

            name = name.StripGuids('.');

            if (!string.IsNullOrWhiteSpace(name))
            {
                if (name[0] == '.')
                    name = name.Remove(0, 1);

                if (name[name.Length - 1] == '.')
                    name = name.Remove(name.Length - 1, 1);
            }
            action = action.StripGuids('/');

            if (!string.IsNullOrWhiteSpace(action))
            {
                if (action[0] == '/')
                    action = action.Remove(0, 1);

                if (action[action.Length - 1] == '/')
                    action = action.Remove(action.Length - 1, 1);
            }
            string key = CreateKey(name, action, appType, AccountUUID);

            if (PermissionExists(key, true))
                return ServiceResponse.Error("Permission " + key + " already exists.");

            try
            {

                using (TreeMonDbContext context = new TreeMonDbContext(_dbConnectionKey)) {
                    context.Insert<Permission>(new Permission
                    {
                        AccountUUID = AccountUUID,
                        Action = action,
                        Active = true,
                        AppType = appType,
                        Key = key,
                        Name = name,
                        Weight = weight,
                        Persists = true,
                        Request = request.StripGuids('/'),
                        EndDate = null,// new DateTime(2222, 2, 22), //sql isn't liking the min date :/
                        StartDate = DateTime.UtcNow

                    });
                }

            }
            catch (Exception ex)
            {
                _logger.InsertError(ex.Message, "RoleManager", "CreatePermission");
                return ServiceResponse.Error("An error occured inserting the permission.");
            }

            return ServiceResponse.OK();
        }


        public List<Permission> GetPermissionsForRole(string roleUUID, string accountUUID)
        {
            // if (!_runningInstall && !this.DataAccessAuthorized(r, _requestingUser,"GET", false)) return ServiceResponse.Error("You are not authorized this action.");

            List<Permission> members;

            using (TreeMonDbContext context = new TreeMonDbContext(_dbConnectionKey))
            {
                members = context.GetAll<RolePermission>()
                        .Where(rrw => rrw.RoleUUID == roleUUID &&
                               rrw.AccountUUID == accountUUID)
                        .Join(
                            context.GetAll<Permission>().Where(uw => uw.Deleted == false),
                            role => role.PermissionUUID,
                            perms => perms.UUID,
                            (role, perms) => new { role, perms }
                        )
                        .Select(s => s.perms)
                        .ToList();
            }
            return members;
        }

        public List<Permission> GetAccountPermissions(string accountUUID)
        {
            //if (!_runningInstall && !this.DataAccessAuthorized(permissions,_requestingUser, "GET", false)) return ServiceResponse.Error("You are not authorized this action.");

            List<Permission> permissions;

            using (TreeMonDbContext context = new TreeMonDbContext(_dbConnectionKey))
            {
                permissions = context.GetAll<Permission>().Where(pw => pw.AccountUUID == accountUUID && pw.Deleted == false).DistinctBy(d => d.Name).ToList();
            }

            return permissions;
        }

        public List<Permission> GetAvailablePermissions(string roleUUID, string accountUUID)
        {
            List<Permission> allAccountPermissions;

            using (TreeMonDbContext context = new TreeMonDbContext(_dbConnectionKey))
            {
                allAccountPermissions = context.GetAll<Permission>().Where( w =>  w.AccountUUID == accountUUID && w.Deleted == false ).DistinctBy(d => d.Name).ToList();
            }

            //Selected permissions for role.
            List<Permission> rolePermissions = GetPermissionsForRole(roleUUID, accountUUID);

            if (rolePermissions == null || !rolePermissions.Any() )
                return allAccountPermissions;//none are selected so return all.

            List<Permission> availablePermissions = new List<Permission>();

            foreach (Permission accountPermission in allAccountPermissions) {

                if( !rolePermissions.Any(w => w.UUID == accountPermission.UUID))
                    availablePermissions.Add(accountPermission);
            }

            return availablePermissions;
        }

        public bool PermissionExists(string key, bool creatingPermission = false)
        {
            try
            {
                IEnumerable<Permission> permissions;
                using (TreeMonDbContext context = new TreeMonDbContext(_dbConnectionKey))
                {
                    permissions = context.GetAll<Permission>().Where(pw => pw?.Key == key).ToList();
                }
                if (permissions == null || !permissions.Any())
                    return false;

                return true;
            }
            catch (Exception ex)
            {
                _logger.InsertError(ex.Message, "RoleManager", MethodInfo.GetCurrentMethod().Name);
                //this is cause sqlite is a pain in the ass. 
                //Keeps giving stupid errors when accessed too fast. 
                //so if we're creating permissions we don't want to accidentally duplicate, and hope it
                //won't error next call to recreate the permission. f***** wonky as hell. >:|
                if (creatingPermission)
                    return true;

                return false;
            }
        }


        #endregion

        #region Roles  ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        public ServiceResult Delete(INode n, bool purge = false)
        {
            ServiceResult res = ServiceResponse.OK();

            if (n == null )
                return ServiceResponse.Error("No record sent.");

            if (!this.DataAccessAuthorized(n, _requestingUser, "DELETE", false)) return ServiceResponse.Error("You are not authorized this action.");

            var r = (Role)n;

            List<UserRole> usersInRole;
            using (TreeMonDbContext context = new TreeMonDbContext(_dbConnectionKey))
            {
                usersInRole = context.GetAll<UserRole>().Where(w => w.AccountUUID == r.AccountUUID && w.RoleUUID == r.UUID).ToList();
            }
            List<RolePermission> rolePermissions = GetRolePermissions(r.UUID, r.AccountUUID);

            if (purge)
            {
                using (var transactionScope = new TransactionScope())
                using (var context = new TreeMonDbContext(_dbConnectionKey))
                {
                 
                        try
                        {
                            if(context.Delete<Role>(r) == 0)
                            {
                                res.Message += "Failed to delete role " + r.Name + Environment.NewLine;
                            }

                            foreach (RolePermission rp in rolePermissions)
                            {
                               if(context.Delete<RolePermission>(rp) == 0)
                                    res.Message += "Failed to delete role permission" + rp.UUID + Environment.NewLine;
                            }

                            foreach (UserRole ur in usersInRole)
                            {
                               if(context.Delete<UserRole>(ur) == 0)
                                    res.Message += "Failed to delete role user role" + ur.Name + Environment.NewLine;
                            }

                        transactionScope.Complete();
                        }
                        catch (Exception ex)
                        {
                            _logger.InsertError(ex.Message, "RoleManager", MethodInfo.GetCurrentMethod().Name);
                            return ServiceResponse.Error("Exception occured while deleting this record.");
                        }
                }
            }
            else //mark as deleted
            {
                using (var transactionScope = new TransactionScope())
                using (var context = new TreeMonDbContext(_dbConnectionKey))
                {
                        try
                        {
                            r.Deleted = true;

                            if (context.Update<Role>(r) == 0)
                                return ServiceResponse.Error(r.Name + " failed to delete. ");

                            //foreach(RolePermission rp in rolePermissions)
                            //{
                            //    rp.Deleted = true; //backlog there is no deleted flag for RolePermission
                            // context.Update<RolePermission>(rp);
                            //}
                            foreach (UserRole ur in usersInRole)
                            {
                                ur.Deleted = true;
                                if(context.Update<UserRole>(ur) == 0)
                                    res.Message += "Failed to delete role user role" + ur.Name + Environment.NewLine;
                            }
                        transactionScope.Complete();
                        }
                        catch (Exception ex)
                        {
                            _logger.InsertError(ex.Message, "RoleManager", MethodInfo.GetCurrentMethod().Name);
                            return ServiceResponse.Error("Exception occured while deleting this record.");
                        }
                    }

            }
           if(!string.IsNullOrWhiteSpace(res.Message))
            {
                res.Code = 500;
                res.Status = "ERROR";
            }
            return res;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="accountUUID"></param>
        /// <param name="purge">If this is true they system will do a hard delete, otherwise it set the delete flag to false.</param>
        /// <returns></returns>
        public ServiceResult DeleteRole(string roleUUID, bool purge = false)
        {
            Role r;
            using (TreeMonDbContext context = new TreeMonDbContext(_dbConnectionKey))
            {
                r = context.GetAll<Role>().FirstOrDefault(w => w.UUID == roleUUID);
            }
            if (r == null)
                return ServiceResponse.Error("Role not found.");

            return Delete(r, purge);
        }


        public List<Role> GetRoles()
        {
            // if (!_runningInstall && !this.DataAccessAuthorized(r, _requestingUser,"GET", false)) return ServiceResponse.Error("You are not authorized this action.");

            using (TreeMonDbContext context = new TreeMonDbContext(_dbConnectionKey))
            {
                return context.GetAll<Role>().ToList();
            }
        }


        public INode Get( string name)
        {
            if (string.IsNullOrWhiteSpace(name) )
                return null;

            // if (!_runningInstall && !this.DataAccessAuthorized(r, "GET", false)) return ServiceResponse.Error("You are not authorized this action.");

            using (TreeMonDbContext context = new TreeMonDbContext(_dbConnectionKey))
            {
                return context.GetAll<Role>().FirstOrDefault(rw => (rw.Name?.EqualsIgnoreCase(name)?? false) && rw.AccountUUID == _requestingUser.AccountUUID);
            }
        }

        public INode GetRole(string name, string accountUUID)
        {
            if (string.IsNullOrWhiteSpace(name))
                return null;

            // if (!_runningInstall && !this.DataAccessAuthorized(r, "GET", false)) return ServiceResponse.Error("You are not authorized this action.");

            using (TreeMonDbContext context = new TreeMonDbContext(_dbConnectionKey))
            {
                return context.GetAll<Role>().FirstOrDefault(rw => (rw.Name?.EqualsIgnoreCase(name) ?? false) && rw.AccountUUID == accountUUID);
            }
        }


        public INode GetBy(string uuid)
        {
            if (string.IsNullOrWhiteSpace(uuid))
                return null;

            // if (!_runningInstall && !this.DataAccessAuthorized(r,_requestingUser, "GET", false)) return ServiceResponse.Error("You are not authorized this action.");

            using (TreeMonDbContext context = new TreeMonDbContext(_dbConnectionKey))
            {
                return context.GetAll<Role>().FirstOrDefault(rw => rw.UUID == uuid);
            }
        }


        public List<Role> GetRoles(string accountUUID)
        {
            List<Role> roles;

            // if (!_runningInstall && !this.DataAccessAuthorized(r,_requestingUser, "GET", false)) return ServiceResponse.Error("You are not authorized this action.");
            using (TreeMonDbContext context = new TreeMonDbContext(_dbConnectionKey))
            {
                roles = context.GetAll<Role>().Where(rw => rw.AccountUUID == accountUUID && rw.Deleted == false).OrderBy(ob => ob.Name).ToList();
            }

            return roles;
        }

        /// <summary>
        /// NOTE: Make sure accountUUID is set.
        /// </summary>
        /// <param name="r"></param>
        /// <param name="ipAddress"></param>
        /// <returns></returns>
        public ServiceResult Insert(INode n, bool validateFirst = true)
        {
            if (!_runningInstall && !this.DataAccessAuthorized(n, _requestingUser, "POST", false)) return ServiceResponse.Error("You are not authorized this action.");

            n.Initialize(this._requestingUser.UUID, this._requestingUser.AccountUUID, this._requestingUser.RoleWeight);

            var r = (Role)n;

            Role dbU;
            using (TreeMonDbContext context = new TreeMonDbContext(_dbConnectionKey))
            {
                if (validateFirst)
                {
                    dbU = context.GetAll<Role>().FirstOrDefault(wu => wu.Name.EqualsIgnoreCase(r.Name) && wu.AccountUUID == r.AccountUUID);

                    if (dbU != null)
                        return ServiceResponse.Error("Role already exists.");
                }

                context.Insert<Role>(r);
            }

            return ServiceResponse.OK("", r);
        }

        /// <summary>
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public ServiceResult Update(INode n)
        {
            if (n == null)
                return ServiceResponse.Error("Invalid role data.");

            if (!_runningInstall && !this.DataAccessAuthorized(n, _requestingUser, "PATCH", false)) return ServiceResponse.Error("You are not authorized this action.");

            var r = (Role)n;

            using (TreeMonDbContext context = new TreeMonDbContext(_dbConnectionKey))
            {
                if (context.Update<Role>(r) > 0)
                    return ServiceResponse.OK();
            }
            return ServiceResponse.Error("System error, role was not updated.");
        }

        public ServiceResult CloneRole(string roleUUID)
        {
            Role originalRole = (Role)this.GetBy(roleUUID);

            if (originalRole == null)
                return ServiceResponse.Error("Role could not be found.");

            if (!_runningInstall && !this.DataAccessAuthorized(originalRole, _requestingUser, "POST", false)) return ServiceResponse.Error("You are not authorized this action.");

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Role, Role>();
            });

            IMapper mapper = config.CreateMapper();
            Role clonedRole = mapper.Map<Role, Role>(originalRole);
            clonedRole.Id = 0;
            clonedRole.UUID = Guid.NewGuid().ToString("N");
            clonedRole.Name += " - Copy";

            using (var transactionScope = new TransactionScope())
            using (var dbContext = new TreeMonDbContext(_dbConnectionKey))
            { 
                //backlog revisit this transaction. Using the member context to make it work seems off.
                try
                {
                    if (!dbContext.Insert<Role>(clonedRole))
                    {
                        _logger.InsertError("Failed cloning role:" + originalRole.UUID, "RoleManager", MethodInfo.GetCurrentMethod().Name);
                        return ServiceResponse.Error("Failed to insert cloned role.");
                    }

                    //Copy permissions into cloned role
                    List<RolePermission> rolePermissions;
                    using (var context = new TreeMonDbContext(_dbConnectionKey))
                    {
                        rolePermissions = context.GetAll<RolePermission>().Where(w => w.AccountUUID == originalRole.AccountUUID && w.RoleUUID == originalRole.UUID).ToList();
                    }
                    //assing the new roleUUID to the permissisons
                    rolePermissions.ForEach(x => x.RoleUUID = clonedRole.UUID);

                    foreach (RolePermission rp in rolePermissions)
                    {
                        if (!dbContext.Insert<RolePermission>(rp))
                        {
                            _logger.InsertError("Failed cloning role permissions:" + originalRole.UUID, "RoleManager", MethodInfo.GetCurrentMethod().Name);
                            return ServiceResponse.Error("Failed cloning role permissions.");
                        }
                    }

                    //Copy Users in to cloned role..
                    List<UserRole> userRoles;
                    using (var context = new TreeMonDbContext(_dbConnectionKey))
                    {
                        userRoles = context.GetAll<UserRole>()
                                                .Where(rrw => rrw.RoleUUID == originalRole.UUID &&
                                                        rrw.AccountUUID == originalRole.AccountUUID).ToList();
                    }
                    userRoles.ForEach(x => x.RoleUUID = clonedRole.UUID);

                    foreach (UserRole ur in userRoles)
                    {
                        if (!dbContext.Insert<UserRole>(ur))
                        {
                            _logger.InsertError("Failed cloning role users:" + originalRole.UUID, "RoleManager", MethodInfo.GetCurrentMethod().Name);
                            return ServiceResponse.Error("Failed cloning role users.");
                        }
                    }
                    transactionScope.Complete();
                }
                catch (Exception ex)
                {
                    _logger.InsertError(ex.Message, "RoleManager", MethodInfo.GetCurrentMethod().Name);
                }
            }

            return ServiceResponse.OK();
        }

        #endregion

        #region RolePermission  TODO refactor into icrud class  ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        public ServiceResult AddPermisssionsToRole(string roleUUID, List<Permission> rps, User requestingUser)
        {
            ServiceResult res = ServiceResponse.OK();

            Role r = (Role)this.GetBy(roleUUID);
            if (r == null)
                return ServiceResponse.Error("Invalid roleUUID.");

            if (!_runningInstall && !this.DataAccessAuthorized(r, _requestingUser, "PATCH", false)) return ServiceResponse.Error("You are not authorized this action.");

            foreach (Permission p in rps)
            {
                RolePermission rp = new RolePermission()
                {
                    PermissionUUID = p.UUID,
                    RoleUUID = roleUUID,
                    AccountUUID = r.AccountUUID,
                    RoleWeight = r.RoleWeight
                };
                ServiceResult addRes = AddRolePermission(rp);
                if (addRes.Code != 200)
                {
                    res.Message += addRes.Message + Environment.NewLine;
                    res.Code = 500;
                    res.Status = "ERROR";
                }
            }
            return res;
        }

        public ServiceResult AddRolePermission(RolePermission rp)
        {
             if (!_runningInstall && !this.DataAccessAuthorized(rp, _requestingUser, "PATCH", false)) return ServiceResponse.Error("You are not authorized this action.");

            ServiceResult res = ServiceResponse.OK();
            if (RolePermissionExists(rp.RoleUUID, rp.AccountUUID, rp.PermissionUUID))
                return res;

            using (TreeMonDbContext context = new TreeMonDbContext(_dbConnectionKey))
            {
                rp.DateCreated = DateTime.UtcNow;
                rp.CreatedBy = _requestingUser.UUID;
                if( !context.Insert<RolePermission>(rp) )
                    return ServiceResponse.Error("Failed to add. ");
            }
            return res;
        }

        public ServiceResult DeletePermissionsFromRole(string roleUUID, List<Permission> rps, User requestingUser)
        {
            ServiceResult res = ServiceResponse.OK();
            foreach (Permission p in rps)
            {
                if (!_runningInstall && !this.DataAccessAuthorized(p, _requestingUser, "DELETE", false)) return ServiceResponse.Error("You are not authorized this action.");

                ServiceResult delRes =  DeleteRolePermission(new RolePermission() { PermissionUUID = p.UUID, RoleUUID = roleUUID, AccountUUID = p.AccountUUID });
                if (delRes.Code != 200)
                {
                    res.Message += delRes.Message + Environment.NewLine;
                    res.Code = 500;
                    res.Status = "ERROR";
                }
            }
            return res;
        }

        public ServiceResult DeleteRolePermission(RolePermission rp)
        {
            ServiceResult res = ServiceResponse.OK();

            if (rp == null)
                return ServiceResponse.Error("No record sent.");

            if (!RolePermissionExists(rp.RoleUUID, rp.AccountUUID, rp.PermissionUUID))
                return ServiceResponse.Error("Record not found.");

             if (!_runningInstall && !this.DataAccessAuthorized(rp, _requestingUser, "DELETE", false)) return ServiceResponse.Error("You are not authorized this action.");

            try
            {
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@PERMISSIONUUID", rp.PermissionUUID);
                parameters.Add("@ROLEUUID", rp.RoleUUID);
                parameters.Add("@ACCOUNTUUID", rp.AccountUUID);
                using (TreeMonDbContext context = new TreeMonDbContext(_dbConnectionKey))
                {
                    if( context.Delete<RolePermission>("WHERE PermissionUUID=@PERMISSIONUUID AND RoleUUID=@ROLEUUID AND AccountUUID=@ACCOUNTUUID", parameters) == 0)
                        return ServiceResponse.Error("Failed to delete. ");
                }
                //SQLITE
                //this was the only way I could get it to delete a RolePermission without some stupid EF error.
                //object[] paramters = new object[] { rp.PermissionUUID , rp.RoleUUID ,rp.AccountUUID };
                //context.Delete<RolePermission>("WHERE PermissionUUID=? AND RoleUUID=? AND AccountUUID=?", paramters);
                //  context.Delete<RolePermission>(rp);
            }
            catch (Exception ex)
            {
                _logger.InsertError(ex.Message, "RoleManager", "DeleteRolePermission");
                Debug.Assert(false, ex.Message);
                return ServiceResponse.Error("Exception occured while deleting this record.");
            }
            return res;
        }

        public RolePermission GetRolePermission(string roleUUID, string accountUUID, string permissionUUID)
        {
            using (TreeMonDbContext context = new TreeMonDbContext(_dbConnectionKey))
            {
                // if (!_runningInstall &&  !this.DataAccessAuthorized(r,_requestingUser, "GET", false)) return ServiceResponse.Error("You are not authorized this action.");
                return context.GetAll<RolePermission>().FirstOrDefault(w => w.AccountUUID == accountUUID && w.RoleUUID == roleUUID && w.PermissionUUID == permissionUUID);
            }
        }

        public List<RolePermission> GetRolePermissions(string roleUUID, string accountUUID)
        {
            // if (!_runningInstall &&  !this.DataAccessAuthorized(r,_requestingUser, "GET", false)) return ServiceResponse.Error("You are not authorized this action.");
            using (TreeMonDbContext context = new TreeMonDbContext(_dbConnectionKey))
            {
                return context.GetAll<RolePermission>().Where(w => w.AccountUUID == accountUUID && w.RoleUUID == roleUUID).ToList();
            }
        }

        public bool RolePermissionExists(string roleUUID, string accountUUID, string permissionUUID)
        {
            using (TreeMonDbContext context = new TreeMonDbContext(_dbConnectionKey))
            {
                if (context.GetAll<RolePermission>().Any(rpw => rpw.AccountUUID == accountUUID && rpw.RoleUUID == roleUUID && rpw.PermissionUUID == permissionUUID))
                    return true;
            }
            return false;
        }

        #endregion

        #region UserRole  TODO refactor into icrud class ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        public ServiceResult AddUsersToRole( string roleUUID, List<User> urs, User requestingUser)
        {
            ServiceResult res = ServiceResponse.OK();
            foreach (User u in urs)
            {
                ServiceResult addRes = AddUserToRole(roleUUID, u , requestingUser);
                if (addRes.Code != 200)
                {
                    res.Message += addRes.Message + Environment.NewLine;
                    res.Code = 500;
                    res.Status = "ERROR";
                }
            }
            return res;
        }


        /// <summary>
        /// AccountUUID should be the users account id, not the account for the role.
        /// </summary>
        /// <param name="up"></param>
        /// <returns></returns>
        public ServiceResult AddUserToRole(string roleUUID, User u, User requestingUser)
        {
            ServiceResult res = ServiceResponse.OK();

            if (UserRoleExists(u.UUID, u.AccountUUID, roleUUID))
                return res;

            Role r = (Role)GetBy(roleUUID);
            //if the role doesn't match the account then the role
            //hasn't been created for the account so the user can not be added to it.
            //
            if (r == null || r.AccountUUID != requestingUser.AccountUUID)
                return ServiceResponse.Error("Invalid parameter");

            if (!_runningInstall && !this.DataAccessAuthorized(r, _requestingUser, "PATCH", false)) return ServiceResponse.Error("You are not authorized this action.");

            using (var context = new TreeMonDbContext(_dbConnectionKey))
            {
                UserRole ur = new UserRole() { AccountUUID = r.AccountUUID, Active = true, CreatedBy =requestingUser.UUID, DateCreated = DateTime.UtcNow,
                 UserUUID = u.UUID, RoleUUID = r.UUID, RoleOperation = r.RoleOperation, RoleWeight = r.RoleWeight};
                if( !context.Insert<UserRole>(ur))
                    return ServiceResponse.Error(u.Name + " failed to add. ");
            }
            return res;
        }

        public ServiceResult DeleteUsersFromRole(string roleUUID, List<User> users, User requestingUser)
        {
            ServiceResult res = ServiceResponse.OK();
            
            foreach (User user in users)
            {
                ServiceResult  delRes = DeleteUserFromRole(roleUUID, user, requestingUser);
                if (delRes.Code != 200)
                {
                    res.Message += delRes.Message + Environment.NewLine;
                    res.Code = 500;
                    res.Status = "ERROR";
                }
            }

            return res;
        }

        public ServiceResult DeleteUserFromRole(string roleUUID, User u, User requestingUser)
        {
            ServiceResult res = ServiceResponse.OK();
            
            if (u == null)
                return ServiceResponse.Error("User is null");

            Role r = (Role)GetBy(roleUUID);
            //if the role doesn't match the account then the role
            //hasn't been created for the account so the user can not be added to it.
            //
            if (r == null || r.AccountUUID != requestingUser.AccountUUID)
                return ServiceResponse.Error("Invalid roleUUID parameter");

            if (!_runningInstall && !this.DataAccessAuthorized(r, _requestingUser, "PATCH", false)) return ServiceResponse.Error("You are not authorized this action.");

            try
            {
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@USERUUID", u.UUID);
                parameters.Add("@ROLEUUID", roleUUID);
                parameters.Add("@ACCOUNTUUID", r.AccountUUID);
                using (TreeMonDbContext context = new TreeMonDbContext(_dbConnectionKey))
                {
                     if( context.Delete<UserRole>("WHERE UserUUID=@USERUUID AND RoleUUID=@ROLEUUID AND AccountUUID=@ACCOUNTUUID", parameters) == 0)
                        return ServiceResponse.Error(u.Name + " failed to remove from role. ");
                }
                //SQLITE
                //this was the only way I could get it to delete a RolePermission without some stupid EF error.
                // object[] paramters = new object[] { rp.UserUUID, rp.RoleUUID, rp.AccountUUID };
                //context.Delete<UserRole>("WHERE UserUUID=? AND RoleUUID=? AND AccountUUID=?", paramters);
                //  context.Delete<UserRole>(rp);
            }
            catch (Exception ex)
            {
                _logger.InsertError(ex.Message, "RoleManager", "DeleteUserFromRole");
                Debug.Assert(false, ex.Message);
                return ServiceResponse.Error("Exception occured while deleting this record.");
            }
            return res;
        }

        #endregion

        #region User and User authorization

        /// <summary>
        /// return users assigned to a role
        /// </summary>
        /// <param name="uuid"></param>
        /// <returns></returns>
        public List<User> GetUsersInRole(string roleUUID, string accountUUID)
        {
            List<User> members;
            using (TreeMonDbContext context = new TreeMonDbContext(_dbConnectionKey))
            {
                members = context.GetAll<UserRole>()
                        .Where(rrw => rrw.RoleUUID == roleUUID &&
                               rrw.AccountUUID == accountUUID)
                        .Join(
                            context.GetAll<User>().Where(uw => uw.Deleted == false),
                            role => role.UserUUID,
                            users => users.UUID,
                            (role, users) => new { role, users }
                        )
                        .Select(s => s.users)
                        .ToList();
            }
            return members;
        }

        public List<User> GetUsersNotInRole(string roleUUID, string accountUUID)
        {
                List<User> usersInAccount;
                    //GetAccountMembers
                using (var context = new TreeMonDbContext(_dbConnectionKey))
                {
                    usersInAccount = context.GetAll<AccountMember>().Where(w => w.AccountUUID == accountUUID)
                                            .Join(
                                                context.GetAll<User>()
                                                    .Where(w => w.Deleted == false),
                                                acct => acct.MemberUUID,
                                                users => users.UUID,
                                                (acct, users) => new { acct, users }
                                             )
                                             .Select(s => s.users)
                                             .ToList();
                }

            List<User> usersInRole = GetUsersInRole(roleUUID, accountUUID);

            if (usersInAccount == null)
                return new List<User>();

            return usersInAccount.Except(usersInRole).ToList();
        }

        /// <summary>
        /// This verifies the permission is still assigned to the role, and that the user is in the role.
        /// </summary>
        /// <param name="userUUID"></param>
        /// <param name="accountUUID"></param>
        /// <param name="requestPath"></param>
        /// <returns></returns>
        public bool IsUserRequestAuthorized(string userUUID, string accountUUID, string requestPath )
        {
            IEnumerable<UserRole> userRoles = new List<UserRole>();
            IEnumerable<RolePermission> rolePermissions = new List<RolePermission>();
            List<Permission> permissions;

            using (TreeMonDbContext context = new TreeMonDbContext(_dbConnectionKey))
            {
                //1. Get the roles the user is assigned
                userRoles = context.GetAll<UserRole>().Where(urw => urw.UserUUID == userUUID && urw.AccountUUID == accountUUID && urw.Deleted == false);

                if (userRoles == null || !userRoles.Any() )
                    return false;

                //2. Get the permissions for the role
                rolePermissions = context.GetAll<RolePermission>().Where(
                    rpw => rpw.AccountUUID == accountUUID
                    && userRoles.Any(ura => ura.RoleUUID == rpw.RoleUUID) //2.A. Filter the role permissions based on the users roles (return only RolePermissions where the user is in it).
                    ).DistinctBy(db => db.PermissionUUID);

                if (rolePermissions == null || !rolePermissions.Any() )
                    return false;

                //Get permissions  for the account and path and distinct Request
                permissions = context.GetAll<Permission>().Where(
                    pw => (pw.AccountUUID == accountUUID  &&//Whether the permission was created by the account or if it was a system created permission doesn't matter, we'll match the true permission below.
                    pw.Request == requestPath && //get the permission for the request being made
                    pw.Deleted == false
                     && rolePermissions.Any(ry => ry.PermissionUUID == pw.UUID) //now match the permission that was assigned to the account (above) in the resulting permissions.    
                    )).DistinctBy(db => db.Request).ToList();
            }

            if (permissions == null || permissions.Count == 0)
                return false;

            return true;
        }


        public bool DataAccessAuthorized(INode dataItem, User requestingUser, string verb, bool isSensitiveData)
        {
            if (dataItem == null || requestingUser == null || requestingUser.Banned || requestingUser.LockedOut )
                return false;

            if (dataItem.CreatedBy == requestingUser.UUID)
                return true;

            if (requestingUser.SiteAdmin)
                return true;

            if (dataItem.Private)
                return false;

            if (isSensitiveData)
                return false;

            if(dataItem.AccountUUID == SystemFlag.Default.Account)
            {
                switch (verb?.ToLower())
                {
                    case "get":
                        return true;
                    case "delete":
                        if (requestingUser.SiteAdmin)
                            return true;

                         return false;
                    case "post":
                        if (requestingUser.SiteAdmin)
                            return true;
                        return false;
                    case "put":
                        if (requestingUser.SiteAdmin)
                            return true;
                        return false;
                    case "patch":
                        if (requestingUser.SiteAdmin)
                            return true;
                        return false;
                }
            }
           
            if (  dataItem.AccountUUID == requestingUser.AccountUUID ) {
                 return UserInAuthorizedRole( requestingUser, dataItem.RoleWeight, dataItem.RoleOperation);
            }

            return false;
        }

        /// <summary>
        /// Used in ApiAuthorization.
        /// </summary>
        /// <param name="requestingUser"></param>
        /// <param name="roleWeight"></param>
        /// <param name="weightOperator"></param>
        /// <returns></returns>
        public bool UserInAuthorizedRole(User requestingUser, int roleWeight, string weightOperator)
        {
            if (roleWeight == 0)
                return true;

            if (requestingUser == null || roleWeight < 0 || string.IsNullOrWhiteSpace(weightOperator))
                return false;

            List<Role> allowedRoles = new List<Role>();
            switch (weightOperator)
            {
                case ">="://role weight greater or equal to
                    allowedRoles = this.GetRoles(requestingUser.AccountUUID)
                           .Where(w => w.RoleWeight >= roleWeight &&
                                  w.AccountUUID == requestingUser.AccountUUID)
                           .OrderBy(o => o.RoleWeight).ToList();
                    break;
                case "=":
                    allowedRoles = this.GetRoles(requestingUser.AccountUUID)
                        .Where(w => w.RoleWeight == roleWeight &&
                               w.AccountUUID == requestingUser.AccountUUID)
                        .OrderBy(o => o.RoleWeight).ToList();
                    break;

                case ">":
                    allowedRoles = this.GetRoles(requestingUser.AccountUUID)
                        .Where(w => w.RoleWeight > roleWeight &&
                               w.AccountUUID == requestingUser.AccountUUID)
                        .OrderBy(o => o.RoleWeight).ToList();
                    break;
                case "<=":
                    allowedRoles = this.GetRoles(requestingUser.AccountUUID)
                        .Where(w => w.RoleWeight <= roleWeight &&
                               w.AccountUUID == requestingUser.AccountUUID)
                        .OrderBy(o => o.RoleWeight).ToList();
                    break;
                case "<":
                    allowedRoles = this.GetRoles(requestingUser.AccountUUID)
                        .Where(w => w.RoleWeight < roleWeight &&
                               w.AccountUUID == requestingUser.AccountUUID)
                        .OrderBy(o => o.RoleWeight).ToList();
                    break;

            }
            if (allowedRoles.Count == 0)
                return false;

            //Check if user is in role 
            foreach (Role r in allowedRoles)
            {
                if (this.IsUserInRole(requestingUser.UUID, r.Name, requestingUser.AccountUUID))
                    return true;
            }
            return false;
        }

        public bool IsSiteAdmin(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
                return false;
            userName = userName.ToLower();

            if (_siteAdmins == null)
                return false;

           return _siteAdmins.Contains(userName);
        }

        public bool IsUserInRole(string userUUID, string roleName, string accountUUID)
        {
            Role r = (Role)Get(roleName);
            if (r == null)
                return false;

            if (GetUsersInRole(r.UUID, accountUUID).Any(w => w.UUID == userUUID))
                return true;

            return false;
        }
 
        public bool UserRoleExists(string userUUID, string accountUUID, string roleUUID)
        {
            using (TreeMonDbContext context = new TreeMonDbContext(_dbConnectionKey))
            {
                if (context.GetAll<UserRole>().Any(rpw => rpw.UserUUID == userUUID && rpw.AccountUUID == accountUUID && rpw.RoleUUID == roleUUID))
                    return true;
            }
            return false;
        }

        #endregion

        #region Install Methods  TODO refactor bulk inserts (see importjson function in appmanager).


        //Generating permissions
        private readonly string[] _verbs = new string[] { "insert", "update", "delete", "purge", "get" };
        private readonly string[] _roles = new string[] { "owner", "admin", "manager", "employee", "patient", "customer" };

        public string[] DefaultRoles {
            get { return _roles; }
        }

       
        public ServiceResult InsertDefaults(string AccountUUID, string appType )
        {
            this._runningInstall = true;

            ServiceResult res = InsertDefaultRoles(AccountUUID,  appType);

            if (res.Status != "OK")
                return res;

           res =  InsertDefaultPermissions(AccountUUID, appType);

           if (res.Status != "OK")
                return res;

            Role ownerRole = (Role)GetRole("owner", AccountUUID);

            res = AssignDefaultRolePermissions(AccountUUID, appType, ownerRole.UUID);

            if (res.Status != "OK")
                return res;
       
            //this should be 52 for Owner role
            List<Permission> availablePermissions = GetAvailablePermissions(ownerRole.UUID, AccountUUID);
            foreach (Permission p in availablePermissions)
            {
                RolePermission rp = new RolePermission()
                {
                    AccountUUID = AccountUUID,
                    RoleUUID = ownerRole.UUID,
                    RoleWeight = 1,
                    RoleOperation = ">=",
                    PermissionUUID = p.UUID,
                    DateCreated = DateTime.UtcNow,
                    Name = ownerRole.Name + p.Name,
                    Deleted = false,
                    Active = true
                };

                res = AddRolePermission(rp);
                if (res.Code != 200)
                    return res;
            }
            this._runningInstall = false;
            return res;
        }


        /// <summary>
        /// This add barebone roles to the system. Use the
        /// admin panel to create more.
        /// BACKLOG: add ability to create template for export/import..
        /// </summary>
        /// <param name="account"></param>
        /// <param name="appType"></param>
        /// <returns></returns>
        protected ServiceResult InsertDefaultRoles(string AccountUUID, string appType)
        {
            try
            {
                string lastGuid = "";
                using (TreeMonDbContext context = new TreeMonDbContext(_dbConnectionKey))
                {
                    foreach (string role in _roles)
                    {
                        if (context.GetAll<Role>().Any( w => w.Name == role && w.AppType == appType && w.AccountUUID == AccountUUID) )
                            continue;

                        Role r = new Role() { Name = role, AccountUUID = AccountUUID, AppType = appType, Persists = true };
                        r.UUID = Guid.NewGuid().ToString("N"); 
                        r.UUParentID = lastGuid;
                        r.UUParentIDType = "Role";
                        lastGuid = r.UUID;
                        r.UUIDType = "Role";
                        r.DateCreated = DateTime.UtcNow;
                        r.StartDate = DateTime.UtcNow;
                        r.EndDate = DateTime.UtcNow;

                        if(!context.Insert<Role>(r))
                            _logger.InsertError("Failed to insert:" + role , "RoleManager", "InsertDefaultRoels:" + AccountUUID);
                    }
                }
            }catch(Exception ex)
            {
                _logger.InsertError(ex.Message, "RoleManager", "InsertDefaultRoels:" + AccountUUID);
                return ServiceResponse.Error(ex.Message);
            }
            return new ServiceResult() { Code = 200, Status = "OK" };
        }

        protected ServiceResult InsertDefaultPermissions(string AccountUUID, string appType)
        {
            List<string> tables;
            using (TreeMonDbContext context = new TreeMonDbContext(_dbConnectionKey))
            {
                tables = context.GetTableNames();

                foreach (string verb in _verbs)
                {

                    foreach (string type in tables)
                    {
                        string name = type.ToLower() + "." + verb.ToLower();
                        string key = CreateKey(name, verb, appType, AccountUUID);

                        if (this.PermissionExists(key))
                            continue;

                        Permission p = new Permission()
                        {
                            Action = verb,
                            Active = true,
                            Type = type,
                            AccountUUID = AccountUUID,
                            AppType = appType,
                            Persists = true,
                            StartDate = DateTime.UtcNow,
                            Weight = 0,
                            Key = key,
                            Name = name,
                            Deleted = false,
                             DateCreated = DateTime.UtcNow,
                              EndDate = DateTime.UtcNow

                        };

                        if(!context.Insert<Permission>(p))
                            _logger.InsertError("Failed to insert:" + p.Name, "RoleManager", "InsertDefaultPermissions:" + AccountUUID);
                    }
                }
                context.SaveChanges();
            }
            return new ServiceResult() { Code = 200, Status = "OK" };
        }

        protected ServiceResult AssignDefaultRolePermissions(string AccountUUID, string appType, string ownerRoleUUID )
        {
            if (string.IsNullOrWhiteSpace(AccountUUID))
            {
                Debug.Assert(false, "Account id is empty.");
                return ServiceResponse.Error("Account id is empty.");
            }
            try
            {
                List<Role> tmpRoles = this.GetRoles(AccountUUID).Where(w => w.AppType == appType).ToList();
                List<Permission> permissions = this.GetAccountPermissions(AccountUUID).Where(w => w.AppType == appType).ToList();
                List<string> matrix = GetPermissionsMatrix();

                foreach (string permissionsSet in matrix)
                {
                    string[] sets = permissionsSet.Split('|');
                    if (sets.Length == 0) { continue; }

                    string table = sets[0].ToLower().Trim();

                    for (int roleIndex = 1; roleIndex < sets.Length; roleIndex++)
                    {
                        string[] roleTokens = sets[roleIndex].Split('{');
                        string role = roleTokens[0].ToLower().Trim();
                        Role r = tmpRoles.FirstOrDefault(w => w.Name.ToLower() == role && w.AccountUUID == AccountUUID && w.AppType?.ToLower() == appType);
                        if(r == null)
                        {
                            r = new Role()
                            {
                                 AccountUUID = AccountUUID,
                                 Name = role,
                                 DateCreated = DateTime.Now,
                                 AppType = appType,
                                CreatedBy = _requestingUser?.UUID,
                                   StartDate = DateTime.Now,
                                   EndDate = DateTime.Now,
                                    Weight =4,
                                     RoleWeight = 4,
                                      RoleOperation = ">="


                            };
                            using (var context = new TreeMonDbContext(_dbConnectionKey))
                            {
                                context.Insert<Role>(r);
                            }
                        }
                        string[] tmpVerbs = roleTokens[1].Split(',');
                        foreach (string verb in tmpVerbs)
                        {
                            string name = table.ToLower() + "." + verb.ToLower();
                            string key = CreateKey(name, verb, appType, AccountUUID);

                            Permission p = permissions.FirstOrDefault(w => w.Key == key && w.AccountUUID == AccountUUID && w.AppType?.ToLower() == appType?.ToLower());

                            if (p == null) { continue; }

                            RolePermission dbRp;
                            using (var context = new TreeMonDbContext(_dbConnectionKey))
                            {
                                dbRp = context.GetAll<RolePermission>().FirstOrDefault(w => w.RoleUUID == r.UUID && w.PermissionUUID == p.UUID && w.AccountUUID == AccountUUID);

                                if (dbRp != null) { continue; }

                                RolePermission rp = new RolePermission()
                                {
                                    Name = r.Name + p.Name,
                                    AccountUUID = AccountUUID,
                                    PermissionUUID = p.UUID,
                                    RoleUUID = r.UUID,
                                    UUID = Guid.NewGuid().ToString("N"),
                                    DateCreated = DateTime.UtcNow
                                      
                                };
                                if (string.IsNullOrWhiteSpace(rp.AccountUUID) || string.IsNullOrWhiteSpace(rp.PermissionUUID) || string.IsNullOrWhiteSpace(rp.RoleUUID))
                                    continue;

                                if (!context.Insert<RolePermission>(rp))
                                {
                                    _logger.InsertError("Failed to insert:" + rp.Name, "RoleManager", "AssingDefaultRolePermissions");
                                    continue;
                                }

                                 if (!string.IsNullOrWhiteSpace(ownerRoleUUID) && !RolePermissionExists(ownerRoleUUID, AccountUUID, p.UUID))
                                {
                                    RolePermission ownerRP = (RolePermission)rp.Clone();
                                    ownerRP.RoleUUID = ownerRoleUUID;
                                    ownerRP.DateCreated = DateTime.UtcNow;
                                    ownerRP.AccountUUID = AccountUUID;
                                    context.Insert<RolePermission>(ownerRP);
                                }
                            }
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                _logger.InsertError(ex.Message, "RoleManager", "AssingDefaultRolePermissions");
                return ServiceResponse.Error(ex.Message);
            }
            return ServiceResponse.OK();
        }

        private List<string> GetPermissionsMatrix()
        {
            //Permissions Matrix.. work in progress...
            List<string> matrix = new List<string>();
            matrix.Add("Notifications |Customer{insert,update,delete,purge,get }|Patient{insert,update,delete,purge,get }|Employee{insert,update,delete,purge,get  }|Manager{insert,update,delete,purge,get }|Admin{insert,update,delete,purge,get } |Owner{insert,update,delete,purge,get }   ");
            matrix.Add("SideAffects   |Customer{insert,update,delete,purge,get }|Patient{insert,update,delete,purge,get }|Employee{insert,update,delete,purge,get  }|Manager{insert,update,delete,purge,get }|Admin{insert,update,delete,purge,get } |Owner{insert,update,delete,purge,get }   ");
            matrix.Add("Reminders     |Customer{insert,update,delete,purge,get }|Patient{insert,update,delete,purge,get }|Employee{insert,update,delete,purge,get  }|Manager{insert,update,delete,purge,get }|Admin{insert,update,delete,purge,get } |Owner{insert,update,delete,purge,get }   ");
            matrix.Add("Anatomy       |Customer{insert,update,delete,purge,get }|Patient{insert,update,delete,purge,get }|Employee{insert,update,delete,purge,get  }|Manager{insert,update,delete,purge,get }|Admin{insert,update,delete,purge,get } |Owner{insert,update,delete,purge,get }   ");
            matrix.Add("Symptoms      |Customer{insert,update,delete,purge,get }|Patient{insert,update,delete,purge,get }|Employee{insert,update,delete,purge,get  }|Manager{insert,update,delete,purge,get }|Admin{insert,update,delete,purge,get } |Owner{insert,update,delete,purge,get }   ");
            matrix.Add("AnatomyTags   |Customer{insert,update,delete,purge,get }|Patient{insert,update,delete,purge,get }|Employee{insert,update,delete,purge,get  }|Manager{insert,update,delete,purge,get }|Admin{insert,update,delete,purge,get } |Owner{insert,update,delete,purge,get }   ");
            matrix.Add("ReminderRules |Customer{insert,update,delete,purge,get }|Patient{insert,update,delete,purge,get }|Employee{insert,update,delete,purge,get  }|Manager{insert,update,delete,purge,get }|Admin{insert,update,delete,purge,get } |Owner{insert,update,delete,purge,get }   ");
            matrix.Add("Ballasts      |Customer{insert,update,delete,purge,get }|Patient{insert,update,delete,purge,get }|Employee{insert,update,delete,purge,get  }|Manager{insert,update,delete,purge,get }|Admin{insert,update,delete,purge,get } |Owner{insert,update,delete,purge,get }   ");
            matrix.Add("Bulbs         |Customer{insert,update,delete,purge,get }|Patient{insert,update,delete,purge,get }|Employee{insert,update,delete,purge,get  }|Manager{insert,update,delete,purge,get }|Admin{insert,update,delete,purge,get } |Owner{insert,update,delete,purge,get }   ");
            matrix.Add("Fans          |Customer{insert,update,delete,purge,get }|Patient{insert,update,delete,purge,get }|Employee{insert,update,delete,purge,get  }|Manager{insert,update,delete,purge,get }|Admin{insert,update,delete,purge,get } |Owner{insert,update,delete,purge,get }   ");
            matrix.Add("Filters       |Customer{insert,update,delete,purge,get }|Patient{insert,update,delete,purge,get }|Employee{insert,update,delete,purge,get  }|Manager{insert,update,delete,purge,get }|Admin{insert,update,delete,purge,get } |Owner{insert,update,delete,purge,get }   ");
            matrix.Add("Vehicles      |Customer{insert,update,delete,purge,get }|Patient{insert,update,delete,purge,get }|Employee{insert,update,delete,purge,get  }|Manager{insert,update,delete,purge,get }|Admin{insert,update,delete,purge,get } |Owner{insert,update,delete,purge,get }   ");
            matrix.Add("SymptomsLog   |Customer{insert,update,delete,purge,get }|Patient{insert,update,delete,purge,get }|Employee{insert,update,delete,purge,get  }|Manager{insert,update,delete,purge,get }|Admin{insert,update,delete,purge,get } |Owner{insert,update,delete,purge,get }   ");
            matrix.Add("Measurements  |Customer{insert,update,delete,purge,get }|Patient{insert,update,delete,purge,get }|Employee{insert,update,delete,purge,get  }|Manager{insert,update,delete,purge,get }|Admin{insert,update,delete,purge,get } |Owner{insert,update,delete,purge,get }   ");
            matrix.Add("Plants        |Customer{insert,update,delete,purge,get }|Patient{insert,update,delete,purge,get }|Employee{insert,update,delete,purge,get  }|Manager{insert,update,delete,purge,get }|Admin{insert,update,delete,purge,get } |Owner{insert,update,delete,purge,get }   ");
            matrix.Add("Pumps         |Customer{insert,update,delete,purge,get }|Patient{insert,update,delete,purge,get }|Employee{insert,update,delete,purge,get  }|Manager{insert,update,delete,purge,get }|Admin{insert,update,delete,purge,get } |Owner{insert,update,delete,purge,get }   ");
            matrix.Add("Strains       |Customer{insert,update,delete,purge,get }|Patient{insert,update,delete,purge,get }|Employee{insert,update,delete,purge,get  }|Manager{insert,update,delete,purge,get }|Admin{insert,update,delete,purge,get } |Owner{insert,update,delete,purge,get }   ");
            matrix.Add("DoseLogs      |Customer{insert,update,delete,purge,get }|Patient{insert,update,delete,purge,get }|Employee{insert,update,delete,purge,get  }|Manager{insert,update,delete,purge,get }|Admin{insert,update,delete,purge,get } |Owner{insert,update,delete,purge,get }   ");
            matrix.Add("UnitsOfMeasure|Customer{insert,update,delete,purge,get }|Patient{insert,update,delete,purge,get }|Employee{insert,update,delete,purge,get  }|Manager{insert,update,delete,purge,get }|Admin{insert,update,delete,purge,get } |Owner{insert,update,delete,purge,get }   ");
            matrix.Add("UsersInAccount|Customer{insert,update,delete,purge,get }|Patient{insert,update,delete,purge,get }|Employee{insert,update,delete,purge,get }|Manager{insert,update,delete,purge,get }|Admin{insert,update,delete,purge,get  } |Owner{insert,update,delete,purge,get }   ");
            matrix.Add("Addresses     |Customer{insert,update,delete,purge,get }|Patient{insert,update,delete,purge,get }|Employee{insert,update,delete,purge,get }|Manager{insert,update,delete,purge,get }|Admin{insert,update,delete,purge,get  } |Owner{insert,update,delete,purge,get }   ");
            matrix.Add("StatusMessages|Customer{insert,update,delete,purge,get }|Patient{insert,update,delete,purge,get }|Employee{insert,update,delete,purge,get }|Manager{insert,update,delete,purge,get }|Admin{insert,update,delete,purge,get  } |Owner{insert,update,delete,purge,get }   ");
            matrix.Add("ProfileLogs   |Customer{insert,update,delete,purge,get }|Patient{insert,update,delete,purge,get }|Employee{insert,update,delete,purge,get  }|Manager{insert,update,delete,purge,get }|Admin{insert,update,delete,purge,get } |Owner{insert,update,delete,purge,get }   ");
            matrix.Add("Accounts      |Customer{insert,update,delete,get }|Patient{insert,update,delete,get }|Employee{insert,update,delete,get }|Manager{insert,update,delete,get }|Admin{insert,update,delete,purge,get  } |Owner{insert,update,delete,purge,get }                           ");
            matrix.Add("Credentials   |Customer{insert,update,delete,get }|Patient{insert,update,delete,get }|Employee{insert,update,delete,get }|Manager{insert,update,delete,get }|Admin{insert,update,delete,purge,get  } |Owner{insert,update,delete,purge,get }                           ");
            matrix.Add("Vendor        |Customer{insert,update,delete,get }|Patient{insert,update,delete,get }|Employee{insert,update,delete,get  }|Manager{insert,update,delete,get }|Admin{insert,update,delete,purge,get } |Owner{insert,update,delete,purge,get }                           ");
            matrix.Add("Users         |Customer{insert,update,delete,get }|Patient{insert,update,delete,get }|Employee{insert,update,delete,get }|Manager{insert,update,delete,get }|Admin{insert,update,delete,purge,get  } |Owner{insert,update,delete,purge,get }                           ");
            matrix.Add("LineItems       |Manager{insert,update,delete,get } |Admin{insert,update,delete,purge,get }|Owner{insert,update,delete,purge,get }                                                                                                                                     ");
            matrix.Add("Roles           |Manager{insert,update,delete,get } |Admin{insert,update,delete,purge,get }|Owner{insert,update,delete,purge,get }                                                                                                                                     ");
            matrix.Add("UserPermissions |Manager{insert,update,delete,get } |Admin{insert,update,delete,purge,get }|Owner{insert,update,delete,purge,get }                                                                                                                                     ");
            matrix.Add("UsersInRoles    |Manager{insert,update,delete,get } |Admin{insert,update,delete,purge,get }|Owner{insert,update,delete,purge,get }                                                                                                                                     ");
            matrix.Add("RolePermissions |Manager{insert,update,delete,get } |Admin{insert,update,delete,purge,get }|Owner{insert,update,delete,purge,get }                                                                                                                                     ");
            matrix.Add("UserSessions    |Manager{insert,update,delete,get } |Admin{insert,update,delete,purge,get }|Owner{insert,update,delete,purge,get }                                                                                                                                     ");
            matrix.Add("Permissions     |Manager{insert,update,delete,get } |Admin{insert,update,delete,purge,get }|Owner{insert,update,delete,purge,get }                                                                                                                                     ");
            matrix.Add("Products      |Customer{insert,update,delete,purge,get }|Patient{insert,update,delete,purge,get }|Employee{insert,update,delete,purge,get  }|Manager{insert,update,delete,purge,get }|Admin{insert,update,delete,purge,get } |Owner{insert,update,delete,purge,get }   ");
            matrix.Add("AppInfo           |Admin{insert,update,delete,get } |Owner{insert,update,delete,purge,get }                                                                                                                                                                            ");
            matrix.Add("AuthenticationLog |Admin{get} |Owner{insert,update,delete,purge,get }                                                                                                                                                                                                  ");
            matrix.Add("SystemLog         |Owner{insert,update,delete,purge,get }                                                                                                                                                                                                              ");
            matrix.Add("Settings          |Owner{insert,update,delete,purge,get }                                                                                                                                                                                                              ");

            return matrix;
        }

        #endregion
    }
}
