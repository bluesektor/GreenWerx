// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using TreeMon.Data;
using TreeMon.Data.Logging.Models;
using TreeMon.Managers;
using TreeMon.Managers.Membership;
using TreeMon.Managers.Store;
using TreeMon.Models.App;
using TreeMon.Models.Datasets;
using TreeMon.Models.Membership;
using TreeMon.Models.Store;
using TreeMon.Web.Tests;

namespace TreeMon.Web.Tests.API.V1
{

    [TestClass]
    public class RolesControllerTests
    {
        private string connectionKey = "MSSQL_TEST";
        private string _ownerAuthToken = "";


        [TestInitialize]
        public void TestSetup()
        {
            ServiceResult res = TestHelper.InitializeControllerTestData(connectionKey, "OWNER");
            Assert.AreNotEqual(res.Code, 500, res.Message);//should not be error. if it is assert.
            _ownerAuthToken = res.Result.ToString();
            Assert.IsNotNull(ConfigurationManager.AppSettings["DefaultDbConnection"]);
        }

        #region Role Permissions Tests

        [TestMethod]
        public void Api_RoleController_AddPermissionsToRole()
        {
            TreeMonDbContext context = new TreeMonDbContext(connectionKey);
            Role role = new Role();
            role.UUID = Guid.NewGuid().ToString("N");
            role.AccountUUID = SystemFlag.Default.Account;
            role.Name = Guid.NewGuid().ToString("N");
            role.DateCreated = DateTime.Now;
            Assert.IsTrue(context.Insert<Role>(role));

            Permission p = new Permission()
            {
                Action = "get",
                Active = true,
                Type = "t",
                AccountUUID = SystemFlag.Default.Account,
                AppType = "appType",
                Persists = true,
                StartDate = DateTime.UtcNow,
                Weight = 0,
                Key = "get" + Guid.NewGuid().ToString("N"),
                Name = Guid.NewGuid().ToString("N"),
                Deleted = false,
                 DateCreated = DateTime.Now
                
            };

            context.Insert<Permission>(p);

     
            RolePermission rolePermission = new RolePermission();
            rolePermission.RoleUUID = role.UUID;
            rolePermission.AccountUUID = role.AccountUUID;
            rolePermission.PermissionUUID = p.UUID;
            rolePermission.UUID = Guid.NewGuid().ToString("N");
            rolePermission.DateCreated = DateTime.Now;
            List<RolePermission> rolePerissions = new List<RolePermission>();
            rolePerissions.Add(rolePermission);

            string postData = JsonConvert.SerializeObject(rolePerissions);

            Task.Run(async () =>
            {                                                                      
                ServiceResult res = await TestHelper.SentHttpRequest("POST", "api/Roles/" + role.UUID + "/Permissions/Add", postData, _ownerAuthToken);
                Assert.IsNotNull(res);
                Assert.AreEqual(res.Code, 200);

                RolePermission dbRolePermission = context.GetAll<RolePermission>().Where(w => w.RoleUUID == role.UUID).FirstOrDefault();
                Assert.IsNotNull(dbRolePermission);
                Assert.AreEqual(rolePermission.AccountUUID, dbRolePermission.AccountUUID);
                Assert.AreEqual(rolePermission.PermissionUUID, p.UUID);

            }).GetAwaiter().GetResult();

        }

        [TestMethod]
        public void Api_RoleController_DeletePermissionsFromRole()
        {
            TreeMonDbContext context = new TreeMonDbContext(connectionKey);
            List<RolePermission> rolePermissions = context.GetAll<RolePermission>().Where( w => w.AccountUUID == SystemFlag.Default.Account ).ToList();
          
            if (rolePermissions == null)
            {
               Api_RoleController_AddPermissionsToRole();

                rolePermissions = context.GetAll<RolePermission>().Where(w => w.AccountUUID == SystemFlag.Default.Account).ToList();
            }

            Assert.IsNotNull(rolePermissions);

            string rolUUID = rolePermissions.FirstOrDefault().UUID;

            string postData = JsonConvert.SerializeObject(rolePermissions);

            Task.Run(async () =>
            {
                ServiceResult res = await TestHelper.SentHttpRequest("POST", "api/Roles/" + rolUUID + "/Permissions/Delete", postData, _ownerAuthToken);
                Assert.IsNotNull(res);
                Assert.AreEqual(res.Code, 200);

                RolePermission dbRolePermission = context.GetAll<RolePermission>().FirstOrDefault(w => w.RoleUUID == rolUUID);
                Assert.IsNull(dbRolePermission);
            }).GetAwaiter().GetResult();

        }

        [TestMethod]
        public void Api_RoleController_GetPermissionsForRole()
        {
            TreeMonDbContext context = new TreeMonDbContext(connectionKey);
            Role role = new Role();
            role.AccountUUID = SystemFlag.Default.Account;
            role.Name = Guid.NewGuid().ToString("N");
            role.DateCreated = DateTime.Now;
            Assert.IsTrue(context.Insert<Role>(role));

            Permission p = new Permission()
            {
                Action = "get",
                Active = true,
                Type = "t",
                AccountUUID = SystemFlag.Default.Account,
                AppType = "appType",
                Persists = true,
                StartDate = DateTime.UtcNow,
                Weight = 0,
                Key = "get" + Guid.NewGuid().ToString("N"),
                Name = Guid.NewGuid().ToString("N"),
                Deleted = false,
                 DateCreated = DateTime.Now
            };

            context.Insert<Permission>(p);

            RolePermission rolePermission = new RolePermission();
            rolePermission.RoleUUID = role.UUID;
            rolePermission.AccountUUID = role.AccountUUID;
            rolePermission.PermissionUUID = p.UUID;
            rolePermission.UUID = Guid.NewGuid().ToString("N");
            rolePermission.DateCreated = DateTime.Now;
            context.Insert<RolePermission>(rolePermission);

            Task.Run(async () =>
            {
                ServiceResult res = await TestHelper.SentHttpRequest("GET", "api/Roles/"+ role.UUID + "/Permissions/", "", _ownerAuthToken);

                Assert.IsNotNull(res);
                Assert.AreEqual(res.Code, 200);

                List<Permission> permissions = JsonConvert.DeserializeObject<List<Permission>>(res.Result.ToString());
                Assert.IsNotNull(permissions);
                Assert.IsTrue(permissions.Count >= 1);

                int foundPermission = 0;
                foreach (Permission perm in permissions)
                {
                    if (perm.Name == p.Name  && perm.AccountUUID == role.AccountUUID)
                        foundPermission++;
                }
                Assert.AreEqual(foundPermission, 1);

            }).GetAwaiter().GetResult();
        }

        [TestMethod]
        public void Api_RoleController_GetUnassignedPermissionsForRole()
        {
            TreeMonDbContext context = new TreeMonDbContext(connectionKey);
            Role role = new Role();
            role.AccountUUID = SystemFlag.Default.Account;
            role.Name = Guid.NewGuid().ToString("N");
            role.DateCreated = DateTime.Now;
            role.EndDate = DateTime.Now;
            role.StartDate = DateTime.Now;
            Assert.IsTrue(context.Insert<Role>(role));

            Permission assingedPermission = new Permission()
            {
                Action = "get",
                Active = true,
                Type = "t",
                AccountUUID = role.AccountUUID,
                AppType = "appType",
                Persists = true,
                StartDate = DateTime.UtcNow,
                Weight = 0,
                Key = "get" + Guid.NewGuid().ToString("N"),
                Name = Guid.NewGuid().ToString("N"),
                Deleted = false,
                 DateCreated = DateTime.Now,
                  EndDate = DateTime.Now
            };

            context.Insert<Permission>(assingedPermission);

            RolePermission rolePermission = new RolePermission();
            rolePermission.RoleUUID = role.UUID;
            rolePermission.AccountUUID = role.AccountUUID;
            rolePermission.PermissionUUID = assingedPermission.UUID;
            rolePermission.UUID = Guid.NewGuid().ToString("N");
            rolePermission.DateCreated = DateTime.Now;
        
            context.Insert<RolePermission>(rolePermission);

            //this is what we want to pull back.
            Permission unAssignedPermission = new Permission()
            {
                Action = "get",
                Active = true,
                Type = "t",
                AccountUUID = SystemFlag.Default.Account,
                AppType = "appType",
                Persists = true,
                StartDate = DateTime.UtcNow,
                Weight = 0,
                Key = "get" + Guid.NewGuid().ToString("N"),
                Name = Guid.NewGuid().ToString("N"),
                Deleted = false,
                DateCreated = DateTime.Now,
                 EndDate = DateTime.Now
            };

            context.Insert<Permission>(unAssignedPermission);


            RolePermission runAssignedRolePermission = new RolePermission();
            runAssignedRolePermission.RoleUUID = Guid.NewGuid().ToString("N"); //make up a role id not associated with above role
            runAssignedRolePermission.AccountUUID = role.AccountUUID;
            runAssignedRolePermission.PermissionUUID = unAssignedPermission.UUID;
            runAssignedRolePermission.UUID = Guid.NewGuid().ToString("N");
            runAssignedRolePermission.DateCreated = DateTime.Now;
   
            context.Insert<RolePermission>(runAssignedRolePermission);


            Task.Run(async () =>
            {
                DataFilter filter = new DataFilter();
                filter.PageResults = false;
                ServiceResult res = await TestHelper.SentHttpRequest("GET", "api/Roles/" + role.UUID + "/Permissions/Unassigned/?filter="+ JsonConvert.SerializeObject(filter), "", _ownerAuthToken);

                Assert.IsNotNull(res);
                Assert.AreEqual(res.Code, 200);

                List<Permission> permissions = JsonConvert.DeserializeObject<List<Permission>>(res.Result.ToString());
                Assert.IsNotNull(permissions);
                Assert.IsTrue(permissions.Count >= 1);

                int foundPermission = 0;
                int foundUnAssignedPermission = 0;
                foreach (Permission perm in permissions)
                {
                    if (perm.Name == assingedPermission.Name && perm.AccountUUID == role.AccountUUID)
                        foundPermission++;

                    if (perm.Name == unAssignedPermission.Name)
                        foundUnAssignedPermission++;
                }
                Assert.AreEqual(foundPermission,0); //should be zero. We don't want the assinged permission
                Assert.AreEqual(foundUnAssignedPermission, 1); //should be 1 for the one above, but could be more.

            }).GetAwaiter().GetResult();
        }



        #endregion

        #region Role Users Tests

        [TestMethod]
        public void Api_RoleController_AddUsersToRole()
        {
            TreeMonDbContext context = new TreeMonDbContext(connectionKey);
            Role role = new Role();
            role.UUID = Guid.NewGuid().ToString("N");
            role.AccountUUID = SystemFlag.Default.Account;
            role.Name = Guid.NewGuid().ToString("N");
            role.DateCreated = DateTime.Now;
            Assert.IsTrue(context.Insert<Role>(role));

            User u = TestHelper.GenerateTestUser(Guid.NewGuid().ToString("N"));
            Assert.IsTrue(context.Insert<User>(u));

            UserRole userRole = new UserRole();
            userRole.RoleUUID = role.UUID;
            userRole.UUID = Guid.NewGuid().ToString("N");
            userRole.AccountUUID = role.AccountUUID;
            userRole.UserUUID = u.UUID;
            role.DateCreated = DateTime.Now;

            List<UserRole> userRoles = new List<UserRole>();
            userRoles.Add(userRole);

            string postData = JsonConvert.SerializeObject(userRoles);

            Task.Run(async () =>
            {
                ServiceResult res = await TestHelper.SentHttpRequest("POST", "api/Roles/" + role.UUID  +"/Users/Add", postData, _ownerAuthToken);
                Assert.IsNotNull(res);
                Assert.AreEqual(res.Code, 200);

                UserRole dbUserRole = context.GetAll<UserRole>().Where(w => w.RoleUUID == role.UUID).FirstOrDefault();
                Assert.IsNotNull(dbUserRole);
                Assert.AreEqual(userRole.AccountUUID, dbUserRole.AccountUUID);
               // Assert.AreEqual(userRole.PermissionUUID, p.UUID);

            }).GetAwaiter().GetResult();

        }

        [TestMethod]
        public void Api_RoleController_DeleteUsersFromRole()
        {
            TreeMonDbContext context = new TreeMonDbContext(connectionKey);
            List<UserRole> userRoles = context.GetAll<UserRole>().Where(w => w.AccountUUID == SystemFlag.Default.Account).ToList();

            if(userRoles == null)
            {
                Api_RoleController_AddUsersToRole();
                userRoles = context.GetAll<UserRole>().Where(w => w.AccountUUID == SystemFlag.Default.Account).ToList();
            }

            Assert.IsNotNull(userRoles);
            string accountUUID = userRoles.FirstOrDefault().AccountUUID;
            string roleUUID = userRoles.First().RoleUUID;

            Role r = context.GetAll<Role>().FirstOrDefault(w => w.UUID == roleUUID);
            
            if (r == null)
            {
                r = new Role()
                {
                    UUID = roleUUID,
                    Name = Guid.NewGuid().ToString("N"),
                    DateCreated = DateTime.Now,
                    EndDate = DateTime.Now,
                    StartDate = DateTime.Now,
                    AccountUUID = accountUUID
                };
                Assert.IsTrue(context.Insert<Role>(r));
            }

            List<User> usersInRole  = context.GetAll<UserRole>()
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

            if(usersInRole.Count == 0)
            {
                usersInRole = context.GetAll<User>().ToList();
               
                foreach(User u in usersInRole)
                {
                    context.Insert<UserRole>(new UserRole()
                    {
                         AccountUUID = accountUUID,
                         DateCreated = DateTime.Now,
                         RoleUUID = r.UUID,
                         UserUUID = u.UUID
                    });
                }
            }
            Assert.IsTrue(usersInRole.Count > 0);

            string postData = JsonConvert.SerializeObject(usersInRole);

            Task.Run(async () =>
            {
                ServiceResult res = await TestHelper.SentHttpRequest("DELETE", "api/Roles/"+ roleUUID + "/Users/Remove", postData, _ownerAuthToken);
                Assert.IsNotNull(res);
                Assert.AreEqual(res.Code, 200);

                UserRole dbUserRole = context.GetAll<UserRole>().FirstOrDefault(w => w.RoleUUID == roleUUID && w.Deleted == false && usersInRole.Any(a=>a.UUID == w.UserUUID ));
                Assert.IsNull(dbUserRole);

            }).GetAwaiter().GetResult();
        }

        [TestMethod]
        public void Api_RoleController_GetUsersInRole()
        {
            TreeMonDbContext context = new TreeMonDbContext(connectionKey);
            Role role = new Role();
            role.AccountUUID = SystemFlag.Default.Account;
            role.Name = Guid.NewGuid().ToString("N");
            role.DateCreated = DateTime.Now;
            role.EndDate = DateTime.Now;
            role.StartDate = DateTime.Now;
           
            Assert.IsTrue(context.Insert<Role>(role));

            User user = TestHelper.GenerateTestUser(Guid.NewGuid().ToString("N"));
            Assert.IsTrue(context.Insert<User>(user));
            AccountMember am = new AccountMember()
            {
                AccountUUID = role.AccountUUID,
                MemberUUID = user.UUID,
                UUID = Guid.NewGuid().ToString("N")
            };
            Assert.IsTrue(context.Insert<AccountMember>(am));

            UserRole userRole = new UserRole();
            userRole.RoleUUID = role.UUID;
            userRole.UUID = Guid.NewGuid().ToString("N");
            userRole.AccountUUID = role.AccountUUID;
            userRole.UserUUID = user.UUID;
            userRole.DateCreated = DateTime.Now;
            userRole.StartDate = DateTime.Now;
            userRole.EndDate =DateTime.Now;

            Assert.IsTrue(context.Insert<UserRole>(userRole));

            Task.Run(async () =>
            {
                ServiceResult res = await TestHelper.SentHttpRequest("POST", "api/Roles/" + role.UUID + "/Users/", "", _ownerAuthToken);

                Assert.IsNotNull(res);
                Assert.AreEqual(res.Code, 200);

                List<User> usersInRole = JsonConvert.DeserializeObject<List<User>>(res.Result.ToString());
                Assert.IsNotNull(usersInRole);
                Assert.IsTrue(usersInRole.Count >= 1);

                int found = 0;
                foreach (User p in usersInRole)
                {
                    if (p.Name == user.Name)
                        found++;

                }

                Assert.AreEqual(found, 1);

            }).GetAwaiter().GetResult();
        }


        [TestMethod]
        public void Api_RoleController_GetUnassignedUsersForRole()
        {
            TreeMonDbContext context = new TreeMonDbContext(connectionKey);
            //add user to a role (we don't want to get this back
            Role role = new Role();
            role.AccountUUID = SystemFlag.Default.Account;
            role.Name = Guid.NewGuid().ToString("N");
            role.DateCreated = DateTime.Now;
            role.EndDate = DateTime.Now;
            role.StartDate = DateTime.Now;
            Assert.IsTrue(context.Insert<Role>(role));

            User user = TestHelper.GenerateTestUser(Guid.NewGuid().ToString("N"));
            Assert.IsTrue(context.Insert<User>(user));

            UserRole userRole = new UserRole();
            userRole.RoleUUID = role.UUID;
            userRole.UUID = Guid.NewGuid().ToString("N");
            userRole.AccountUUID = role.AccountUUID;
            userRole.UserUUID = user.UUID;
            userRole.DateCreated = DateTime.Now;
            userRole.EndDate = DateTime.Now;
            userRole.StartDate = DateTime.Now;
            Assert.IsTrue(context.Insert<UserRole>(userRole));

            AccountMember am = new AccountMember()
            {
                AccountUUID = role.AccountUUID,
                MemberUUID = user.UUID,
                UUID = Guid.NewGuid().ToString("N")
            };
            Assert.IsTrue(context.Insert<AccountMember>(am));


            //this is what we want to pull back.
            User unAssignedUser = TestHelper.GenerateTestUser(Guid.NewGuid().ToString("N"));
            Assert.IsTrue(context.Insert<User>(unAssignedUser));

            AccountMember uam = new AccountMember()
            {
                AccountUUID = role.AccountUUID,
                MemberUUID = unAssignedUser.UUID,
                UUID = Guid.NewGuid().ToString("N")
            };
            Assert.IsTrue(context.Insert<AccountMember>(uam));


            UserRole unAssigneduserRole = new UserRole();
            unAssigneduserRole.RoleUUID = Guid.NewGuid().ToString("N"); 
            unAssigneduserRole.UUID = Guid.NewGuid().ToString("N");
            unAssigneduserRole.AccountUUID = role.AccountUUID;
            unAssigneduserRole.UserUUID = unAssignedUser.UUID;
            unAssigneduserRole.Name = Guid.NewGuid().ToString("N"); ;
            unAssigneduserRole.DateCreated = DateTime.Now;
            unAssigneduserRole.EndDate = DateTime.Now;
            unAssigneduserRole.StartDate = DateTime.Now;

            Assert.IsTrue(context.Insert<UserRole>(unAssigneduserRole));

            Task.Run(async () =>
            {
                DataFilter filter = new DataFilter();
                filter.PageResults = false;
                ServiceResult res = await TestHelper.SentHttpRequest("GET", "api/Roles/" + role.UUID + "/Users/Unassigned/?filter="+ JsonConvert.SerializeObject(filter), "", _ownerAuthToken);

                Assert.IsNotNull(res);
                Assert.AreEqual(res.Code, 200);

                List<User> users = JsonConvert.DeserializeObject<List<User>>(res.Result.ToString());
                Assert.IsNotNull(users);
                Assert.IsTrue(users.Count >= 1);

                int foundUser= 0;
                int foundUnAssignedUser = 0;
                foreach (User tmpUser in users)
                {
                    if (tmpUser.Name == userRole.Name && tmpUser.AccountUUID == role.AccountUUID)
                        foundUser++;

                    if (tmpUser.UUID == unAssigneduserRole.UserUUID)
                        foundUnAssignedUser++;
                }
                Assert.AreEqual(foundUser, 0); //should be zero. We don't want the assinged permission
                Assert.AreEqual(foundUnAssignedUser, 1); //should be 1 for the one above, but could be more.

            }).GetAwaiter().GetResult();
        }

        #endregion

        #region Role Tests

        [TestMethod]
        public void Api_RoleController_AddRole()
        {
            Role mdl = new Role();
            mdl.AccountUUID = SystemFlag.Default.Account;
            mdl.Name = Guid.NewGuid().ToString("N");
            mdl.UUID = Guid.NewGuid().ToString("N");
            mdl.DateCreated = DateTime.Now;
            string postData = JsonConvert.SerializeObject(mdl);

            Task.Run(async () =>
            {
                ServiceResult res = await TestHelper.SentHttpRequest("POST", "api/Roles/Add", postData, _ownerAuthToken);
                Assert.IsNotNull(res);
                Assert.AreEqual(res.Code, 200);

                Role p = JsonConvert.DeserializeObject<Role>(res.Result.ToString());
                Assert.IsNotNull(p);
                TreeMonDbContext context = new TreeMonDbContext(connectionKey);
                Role dbRole = context.GetAll<Role>().Where(w => w.UUID == p.UUID).FirstOrDefault();
                Assert.IsNotNull(dbRole);
                Assert.AreEqual(mdl.Name, dbRole.Name);

            }).GetAwaiter().GetResult();

        }

        [TestMethod]
        public void Api_RoleController_GetRole()
        {
            TreeMonDbContext context = new TreeMonDbContext(connectionKey);
            Role mdl = new Role();
            mdl.UUID = Guid.NewGuid().ToString("N");
            mdl.AccountUUID = SystemFlag.Default.Account;
            mdl.Name = Guid.NewGuid().ToString("N");
            mdl.DateCreated = DateTime.Now;
            Assert.IsTrue(context.Insert<Role>(mdl));

            User user = TestHelper.GenerateTestUser(Guid.NewGuid().ToString("N"));
             SessionManager sessionManager = new SessionManager(connectionKey);
            string userJson = JsonConvert.SerializeObject(user);

            UserSession us = sessionManager.SaveSession("127.1.1.33", user.UUID, userJson, false);

            Task.Run(async () =>
            {
                ServiceResult res = await TestHelper.SentHttpRequest("GET", "api/Roles/" + mdl.Name, "", _ownerAuthToken);

                Assert.IsNotNull(res);
                Assert.AreEqual(res.Code, 200);

                Role p = JsonConvert.DeserializeObject<Role>(res.Result.ToString());
                Assert.IsNotNull(p);
                Assert.AreEqual(mdl.Name, p.Name);

            }).GetAwaiter().GetResult();
        }

        [TestMethod]
        public void Api_RoleController_GetRoles()
        {

            TreeMonDbContext context = new TreeMonDbContext(connectionKey);
            Role mdl = new Role();
            mdl.AccountUUID = SystemFlag.Default.Account;
            mdl.Name = Guid.NewGuid().ToString("N");
            mdl.DateCreated = DateTime.Now;
            mdl.StartDate = DateTime.Now;
            mdl.EndDate = DateTime.Now;
            Assert.IsTrue(context.Insert<Role>(mdl));

            Role mdl2 = new Role();
            mdl2.AccountUUID = SystemFlag.Default.Account;
            mdl2.Name = Guid.NewGuid().ToString("N");
            mdl2.DateCreated = DateTime.Now;
            mdl2.EndDate = DateTime.Now;
            mdl2.StartDate = DateTime.Now;
            Assert.IsTrue(context.Insert<Role>(mdl2));

            Task.Run(async () =>
            {
                DataFilter filter = new DataFilter();
                filter.PageResults = false;
                ServiceResult res = await TestHelper.SentHttpRequest("GET", "api/Roles/?filter="+ JsonConvert.SerializeObject(filter),"", _ownerAuthToken);

                Assert.IsNotNull(res);
                Assert.AreEqual(res.Code, 200);

                List<Role> Roles = JsonConvert.DeserializeObject<List<Role>>(res.Result.ToString());
                Assert.IsNotNull(Roles);
                Assert.IsTrue(Roles.Count >= 2);

                int foundRoles = 0;
                foreach (Role p in Roles)
                {
                    if (p.Name == mdl.Name || p.Name == mdl2.Name)
                        foundRoles++;

                }

                Assert.AreEqual(foundRoles, 2);

            }).GetAwaiter().GetResult();
        }

        [TestMethod]
        public void Api_RoleController_DeleteRole()
        {
            TreeMonDbContext context = new TreeMonDbContext(connectionKey);
            Role mdl = new Role();
            mdl.AccountUUID = SystemFlag.Default.Account;
            mdl.Name = Guid.NewGuid().ToString("N");
            mdl.UUID = Guid.NewGuid().ToString("N");
            mdl.DateCreated = DateTime.Now;
            Assert.IsTrue(context.Insert<Role>(mdl));
            string postData =  JsonConvert.SerializeObject(mdl);

            Task.Run(async () =>
            {
                ServiceResult res = await TestHelper.SentHttpRequest("POST", "api/Roles/Delete", postData, _ownerAuthToken);

                Assert.IsNotNull(res);
                Assert.AreEqual(res.Code, 200);

                Role dbRole = context.GetAll<Role>().Where(w => w.Name == mdl.Name).FirstOrDefault();
                Assert.IsNotNull(dbRole);
                Assert.IsTrue(dbRole.Deleted);
                //Assert.IsNull(dbRole);

            }).GetAwaiter().GetResult();
        }

        [TestMethod]
        public void Api_RoleController_UpdateRole()
        {
            TreeMonDbContext context = new TreeMonDbContext(connectionKey);
            Role mdl = new Role();
            mdl.AccountUUID = SystemFlag.Default.Account;
            mdl.Name = Guid.NewGuid().ToString("N");
            mdl.UUID = Guid.NewGuid().ToString("N");
            mdl.DateCreated = DateTime.Now;
            mdl.StartDate = DateTime.Now;
            mdl.EndDate = DateTime.Now;
            mdl.Private = true;
            Assert.IsTrue(context.Insert<Role>(mdl));
            
            mdl = context.GetAll<Role>().Where(w => w.Name == mdl.Name).FirstOrDefault();
            Role pv = new Role();
            pv.Id = mdl.Id; 
            pv.UUID = mdl.UUID;
            pv.AccountUUID = mdl.AccountUUID;
            pv.Name = mdl.Name;
            pv.Private = false;
            pv.DateCreated = DateTime.Now;
            pv.StartDate = DateTime.Now;
            pv.EndDate = DateTime.Now;
            //~~~ Updatable fields ~~~
            //Name     
            //Private  
            //SortOrder
            //Active   
            //Deleted         

            string postData = JsonConvert.SerializeObject(pv);
            Task.Run(async () =>
            {
                ServiceResult res = await TestHelper.SentHttpRequest("POST", "api/Roles/Update", postData, _ownerAuthToken);
                Assert.IsNotNull(res);
                Assert.AreEqual(res.Code, 200);

                Role dbRole = context.GetAll<Role>().Where(w => w.Name == mdl.Name).FirstOrDefault();
                Assert.IsNotNull(dbRole);
                Assert.IsFalse(dbRole.Private);
     
            }).GetAwaiter().GetResult();

        }

        #endregion 

    }
}
