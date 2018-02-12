// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TreeMon.Data;
using TreeMon.Models.App;
using System.Collections.Generic;
using TreeMon.Managers.Membership;
using TreeMon.Data.Logging.Models;
using TreeMon.Models.Membership;
using System.Linq;
using TreeMon.Utilites.Extensions;

namespace TreeMon.Web.Tests._templates
{
    [TestClass]
    public class RoleManager_Tests
    {
        //private string connectionKey = "MSSQL_TEST";

        //List<User> _users = new List<User>();


        //[TestInitialize]
        //public void TestSetup()
        //{
        //    //RoleManager m = new RoleManager(connectionKey);
        //    //Assert.AreEqual(m.InsertDefaults(SystemFlag.Default.Account, SystemFlag.Default.AppType).Code, 200);

        //    //TreeMonDbContext context = new TreeMonDbContext(connectionKey);
        //    //_users = context.GetAll<User>().ToList();
        //    //if (_users == null || _users.Count == 0)
        //    //{
        //    //    Random rand = new Random();
        //    //    for (int i = 0; i < 5; i++)
        //    //    {
        //    //        context.Insert<User>(new User()
        //    //        {
        //    //            AccountUUID = SystemFlag.Default.Account,
        //    //            UUID = Guid.NewGuid().ToString("N")
        //    //        });
        //    //    }
        //    //    _users.AddRange(context.GetAll<User>());
        //    //}
        //}

        //[TestMethod]
        //public void RoleManager_CreateKey()
        //{
        //    RoleManager m = new RoleManager(connectionKey);
        //    Assert.IsTrue( m.CreateKey("name", "verb", "phone", "testaccount").Length > 0);
        //}

        //[TestMethod]
        //public void RoleManager_NameFromPath()
        //{
        //    RoleManager m = new RoleManager(connectionKey);
        //    string tmp = m.NameFromPath("api/Users/Delete%7B%7BUUID%7D%7D");
        //    Assert.IsFalse(tmp.Contains("%"));
        //    tmp = m.NameFromPath("api/Account//SetActiveFor/User/siteadminUUID1");
        //    Assert.IsFalse(tmp.Contains(".."));
        //    tmp = m.NameFromPath("Admin/Roles?error=You+are+not+authorized.");
        //    Assert.IsFalse(tmp.Contains("?"));
        //    tmp = m.NameFromPath("Admin/Roles?error=You+are+not+authorized.");
        //    Assert.IsFalse(tmp.Contains("+"));
        //}


        //#region RolePermission

       
        ////RolePermissionExists

        //[TestMethod]
        //public void RoleManager_AddRolePermission()
        //{
        //    RoleManager m = new RoleManager(connectionKey);
        //    Role r = m.GetRole("patient", SystemFlag.Default.Account);
        //    Assert.IsNotNull(r);

        //    // List<Permission> ps = m.GetPermissionsForRole(r.UUID, r.AccountUUID);
        //    // Assert.IsTrue(ps.Count > 0);
        //    Permission p = m.GetAvailablePermissions(r.UUID, r.AccountUUID).FirstOrDefault();
        //    Assert.IsNotNull(p);

        //    Assert.IsFalse(m.RolePermissionExists(r.UUID, r.AccountUUID, p.UUID));
            
        //    Assert.IsTrue(
        //        m.AddRolePermission(new RolePermission()
        //        {
        //             RoleUUID = r.UUID,
        //             AccountUUID  = r.AccountUUID,
        //              PermissionUUID  =  p.UUID
        //        }));

        //    Assert.IsTrue(m.RolePermissionExists(r.UUID, r.AccountUUID,  p.UUID));

        //}

        //[TestMethod]
        //public void RoleManager_AddRolePermissions()
        //{
        //    RoleManager m = new RoleManager(connectionKey);
        //    Role r = m.GetRole("customer",SystemFlag.Default.Account);
        //    Assert.IsNotNull(r);
        //    List<Permission> permissions = m.GetAvailablePermissions(r.UUID, r.AccountUUID);
        //    Assert.IsNotNull(permissions);
        //    List<RolePermission> RolePermissions = new List<RolePermission>();
        //    string name = Guid.NewGuid().ToString();
        //    foreach (Permission p in permissions)
        //    {
        //        RolePermission ur = new RolePermission()
        //        {
        //            PermissionUUID = p.UUID,
        //            AccountUUID = r.AccountUUID,
        //            RoleUUID = r.UUID,
        //        };
        //        RolePermissions.Add(ur);
        //    }
        //    Assert.IsTrue( m.AddRolePermissions(RolePermissions) );
        //    foreach (Permission p in permissions)
        //    {
        //        Assert.IsTrue(m.RolePermissionExists(r.UUID, p.AccountUUID, p.UUID));
        //    }
        //}

        //[TestMethod]
        //public void RoleManager_GetRolePermission()
        //{
        //    RoleManager m = new RoleManager(connectionKey);
        //    Role r = m.GetRole("patient", SystemFlag.Default.Account);
        //    Assert.IsNotNull(r);

   
        //    Permission p = m.GetAvailablePermissions(r.UUID, r.AccountUUID).FirstOrDefault(); 
        //    Assert.IsNotNull(p);

        //    Assert.IsFalse(m.RolePermissionExists(r.UUID, r.AccountUUID, p.UUID));

        //    Assert.IsTrue(
        //        m.AddRolePermission(new RolePermission()
        //        {
        //             RoleUUID = r.UUID,
        //            AccountUUID = r.AccountUUID,
        //            PermissionUUID = p.UUID
        //        }));
        //    Assert.IsNotNull(m.GetRolePermission(r.UUID, r.AccountUUID,  p.UUID));
        //}

        //[TestMethod]
        //public void RoleManager_GetRolePermissions()
        //{
        //    #region add role permissions
        //    RoleManager m = new RoleManager(connectionKey);
        //    Role r = m.GetRole("customer","a");
        //    Assert.IsNotNull(r);
        //    List<Permission> permissions = m.GetAvailablePermissions(r.UUID, r.AccountUUID);
        //    Assert.IsNotNull(permissions);
        //    List<RolePermission> RolePermissions = new List<RolePermission>();
        //    string name = Guid.NewGuid().ToString();
        //    foreach (Permission p in permissions)
        //    {
        //        RolePermission ur = new RolePermission()
        //        {
        //            PermissionUUID = p.UUID,
        //            AccountUUID = r.AccountUUID,
        //            RoleUUID = r.UUID,
        //        };
        //        RolePermissions.Add(ur);
        //    }
        //    Assert.IsTrue(m.AddRolePermissions(RolePermissions));
        //    #endregion

        //    Assert.IsTrue(m.GetRolePermissions(r.UUID,r.AccountUUID).Count > 0);
        //}

        //[TestMethod]
        //public void RoleManager_DeleteRolePermission()
        //{
        //    #region Insert role permission
        //    RoleManager m = new RoleManager(connectionKey);
        //    Role r = m.GetRole("patient", SystemFlag.Default.Account);
        //    Assert.IsNotNull(r);


        //    Permission p = m.GetAvailablePermissions(r.UUID, r.AccountUUID).FirstOrDefault(); 
        //    Assert.IsNotNull(p);

        //    Assert.IsFalse(m.RolePermissionExists(r.UUID, r.AccountUUID,  p.UUID));

        //    RolePermission rp = new RolePermission()
        //    {
        //        RoleUUID = r.UUID,
        //        AccountUUID = r.AccountUUID,
        //        PermissionUUID = p.UUID
        //    };

        //    Assert.IsTrue( m.AddRolePermission(rp));

        //    Assert.IsNotNull(m.GetRolePermission(r.UUID, r.AccountUUID,  p.UUID));
        //    #endregion

        //    Assert.IsTrue(m.DeleteRolePermission(rp) > 0);
        //}

        //[TestMethod]
        //public void RoleManager_DeleteRolePermissions()
        //{
        //    #region add role RolePermission
        //    RoleManager m = new RoleManager(connectionKey);
        //    Role r = m.GetRole("customer",  SystemFlag.Default.Account);
        //    Assert.IsNotNull(r);
        //    List<Permission> permissions = m.GetAvailablePermissions(r.UUID, r.AccountUUID);
        //    Assert.IsNotNull(permissions);
        //    List<RolePermission> RolePermissions = new List<RolePermission>();
        //    string name = Guid.NewGuid().ToString();
        //    foreach (Permission p in permissions)
        //    {
        //        RolePermission ur = new RolePermission()
        //        {
        //            PermissionUUID = p.UUID,
        //            AccountUUID = r.AccountUUID,
        //            RoleUUID = r.UUID,
        //        };
        //        RolePermissions.Add(ur);
        //    }
        //    Assert.IsTrue(m.AddRolePermissions(RolePermissions));
        //    #endregion

        //    Assert.IsTrue(m.DeleteRolePermissions(RolePermissions) > 0);
        //}


        //#endregion

        //#region Role

        //[TestMethod]
        //public void RoleManager_Insert_Role()
        //{
        //    RoleManager m = new RoleManager(connectionKey);
        //    string name = Guid.NewGuid().ToString();
        //    Assert.AreEqual(
        //        m.InsertRole(new Role()
        //        {
        //            AccountUUID = "a",
        //            Name = name,
        //             AppType = SystemFlag.Default.AppType
                      
        //        }).Code, 200);

        //    Assert.IsNotNull(m.GetRole(name,"a"));
        //}

        //[TestMethod]
        //public void RoleManager_GetRole()
        //{
        //    RoleManager m = new RoleManager(connectionKey);
        //    string name = Guid.NewGuid().ToString();
        //    ServiceResult sr = m.InsertRole(new Role()
        //    {
        //        AccountUUID = "a",
        //        Name =name,
        //    });

        //    Assert.AreEqual(sr.Code, 200, sr.Message);
        //    Role s = m.GetRole(name,"a");
        //    Assert.IsNotNull(s);
        //}

        //[TestMethod]
        //public void RoleManager_GetRoleBy()
        //{
        //    RoleManager m = new RoleManager(connectionKey);
        //    string name = Guid.NewGuid().ToString();
        //    Assert.AreEqual(
        //      m.InsertRole(new Role()
        //      {
        //          AccountUUID = "a",
        //          Name = name
        //      })
        //   .Code, 200);
        //    Role s = m.GetRole(name,"a");
        //    Assert.IsNotNull(s);
        //    Role suid = m.GetRoleBy(s.UUID);
        //    Assert.IsNotNull(suid);
        //}

        //[TestMethod]
        //public void RoleManager_GetRoles()
        //{
        //    RoleManager m = new RoleManager(connectionKey);
        //    Assert.IsTrue(m.GetRoles().Count > 0);
        //    Assert.IsTrue(m.GetRoles(SystemFlag.Default.Account).Count > 0);
        //}

        //[TestMethod]
        //public void RoleManager_UpdateRole()
        //{
        //    RoleManager m = new RoleManager(connectionKey);
        //    Role r = m.GetRole("admin", SystemFlag.Default.Account);
        //    Assert.IsNotNull(r);
        //    string parentID = "UPDATED";

        //    if (string.IsNullOrWhiteSpace(r.UUParentID))
        //        r.UUParentID = parentID;
        //    else
        //    {
        //        r.UUParentID = "";
        //        parentID = "";
        //    }

        //    Assert.AreEqual(m.UpdateRole(r).Code, 200);

        //    Role updatedR = m.GetRole("admin", SystemFlag.Default.Account);
        //    Assert.IsNotNull(updatedR);
        //    Assert.AreEqual(updatedR.UUParentID, parentID);
        //}

        //[TestMethod]
        //public void RoleManager_DeleteRole_Patient()
        //{
        //    RoleManager m = new RoleManager(connectionKey);

        //    //This will make sure the roles are in there.
        //    Assert.AreEqual(m.InsertDefaults(SystemFlag.Default.Account, SystemFlag.Default.AppType).Code, 200);

        //    string name = "Patient";
        //    Role s = m.GetRole(name, SystemFlag.Default.Account);

        //    //Test the delete flag
        //    Assert.IsTrue(m.DeleteRole(s));
        //    Role d = m.GetRole(name,SystemFlag.Default.Account);
        //    Assert.IsNotNull(d);
        //    Assert.IsTrue(d.Deleted == true);

        //    //Make sure UserRole delete flag is set. 
        //    TreeMonDbContext context = new TreeMonDbContext(connectionKey);
        //    List<UserRole> usersInRole = context.GetAll<UserRole>().Where(w => w.AccountUUID == s.AccountUUID && w.RoleUUID == s.UUID && w.Deleted == false).ToList();
        //    Assert.IsTrue(usersInRole.Count == 0);

        //    //RolePermission doesn't have a Deleted flag yet..

        //    //Test the purge!
        //    Assert.IsTrue(m.DeleteRole(s, true));
        //    d = m.GetRole(name, SystemFlag.Default.Account);
        //    Assert.IsNull(d);
        //    context = new TreeMonDbContext(connectionKey);
        //    //make sure these are purged also
        //    usersInRole = context.GetAll<UserRole>().Where(w => w.AccountUUID == s.AccountUUID && w.RoleUUID == s.UUID).ToList();
        //    Assert.IsTrue(usersInRole.Count == 0);
        //    List<RolePermission> rolePermissions = m.GetRolePermissions(s.UUID,s.AccountUUID);
        //    Assert.IsTrue(rolePermissions.Count == 0);
        //}

        //[TestMethod]
        //public void RoleManager_DeleteRole_ById()
        //{
        //    RoleManager m = new RoleManager(connectionKey);

        //    //This will make sure the roles are in there.
        //    Assert.AreEqual(m.InsertDefaults(SystemFlag.Default.Account, SystemFlag.Default.AppType).Code, 200);

        //    string name = "Patient";
        //    Role s = m.GetRole(name, SystemFlag.Default.Account);

        //    //Test the delete flag
        //    Assert.IsTrue(m.DeleteRole(s.UUID));
        //    Role d = m.GetRole(name, SystemFlag.Default.Account);
        //    Assert.IsNotNull(d);
        //    Assert.IsTrue(d.Deleted == true);

        //    //Make sure UserRole delete flag is set. 
        //    TreeMonDbContext context = new TreeMonDbContext(connectionKey);
        //    List<UserRole> usersInRole = context.GetAll<UserRole>().Where(w => w.AccountUUID == s.AccountUUID && w.RoleUUID == s.UUID && w.Deleted == false).ToList();
        //    Assert.IsTrue(usersInRole.Count == 0);

        //    //RolePermission doesn't have a Deleted flag yet..

        //    //Test the purge!
        //    Assert.IsTrue(m.DeleteRole(s.UUID, true));
        //    d = m.GetRole(name, SystemFlag.Default.Account);
        //    Assert.IsNull(d);
        //    context = new TreeMonDbContext(connectionKey);
        //    //make sure these are purged also
        //    usersInRole = context.GetAll<UserRole>().Where(w => w.AccountUUID == s.AccountUUID && w.RoleUUID == s.UUID).ToList();
        //    Assert.IsTrue(usersInRole.Count == 0);
        //    List<RolePermission> rolePermissions = m.GetRolePermissions(s.UUID,s.AccountUUID);
        //    Assert.IsTrue(rolePermissions.Count == 0);
        //}

        //#endregion

        //#region UserRole

        //[TestMethod]
        //public void RoleManager_AddUserToRole()
        //{
        //    RoleManager m = new RoleManager(connectionKey);
        //    Role r = m.GetRole("admin", SystemFlag.Default.Account);
        //    Assert.IsNotNull(r);

        //    string name = Guid.NewGuid().ToString();
        //    Assert.IsTrue(
        //        m.AddUserToRole(new UserRole()
        //        {
        //            UserUUID = _users[0].UUID,
        //            AccountUUID = _users[0].AccountUUID,
        //            RoleUUID = r.UUID,
        //            Name = name
        //        }));

        //    //IsUserInRole
        //    Assert.IsTrue(m.UserRoleExists(_users[0].UUID, _users[0].AccountUUID, r.UUID));
        //    //RoleManager m = new RoleManager(connectionKey);

        //    //Role r = m.GetRole("admin");
        //    //Assert.IsNotNull(r);

        //    ////User has to be in the account the role is in. 
        //    //User u = _users[0];

        //    //UserRole ur = new UserRole()
        //    //{
        //    //    AccountUUID = "xsadfqaswderfoai",
        //    //     UserUUID = u.UUID,
        //    //    AppType = SystemFlag.Default.AppType,
        //    //    RoleUUID = r.UUID
        //    //};

        //    ////this tests to make sure the user account matches the role account.
        //    ////if this fales (which it should for this test)
        //    ////then the Role has not been created for the account.
        //    ////
        //    //Assert.IsFalse(m.AddUserToRole(ur));

        //    ////There should be a default role, now assing the users account to the same as the
        //    ////role and this should be ok.
        //    ////
        //    //u.AccountUUID = SystemFlag.Default.Account;
        //    //Assert.IsTrue(m.AddUserToRole(ur));

        //}

        //[TestMethod]
        //public void RoleManager_AddUsersToRole()
        //{
        //    RoleManager m = new RoleManager(connectionKey);
        //    Role r = m.GetRole("admin", SystemFlag.Default.Account);
        //    Assert.IsNotNull(r);
        //    List<UserRole> userRoles = new List<UserRole>();
        //    string name = Guid.NewGuid().ToString();
        //    foreach (User u in _users)
        //    {
        //        UserRole ur = new UserRole()
        //        {
        //            UserUUID = u.UUID,
        //            AccountUUID = u.AccountUUID,
        //            RoleUUID = r.UUID,
        //            Name = name
        //        };
        //        userRoles.Add(ur);
        //    }

        //    m.AddUsersToRole(userRoles);

        //    //IsUserInRole
        //    foreach (User u in _users)
        //    {
        //        Assert.IsTrue(m.UserRoleExists(u.UUID, u.AccountUUID, r.UUID));
        //    }
        //    //RoleManager m = new RoleManager(connectionKey);

        //    //Role r = m.GetRole("admin");
        //    //Assert.IsNotNull(r);

        //    //List<UserRole> usersRole = new List<UserRole>();
        //    //for (int i = 0; i < 5; i++)
        //    //{
        //    //    ////User has to be in the account the role is in. 
        //    //    User u = _users[i];

        //    //    UserRole ur = new UserRole()
        //    //    {
        //    //        AccountUUID = "xsadfqaswderfoai",
        //    //        UserUUID = u.UUID,
        //    //         AppType = SystemFlag.Default.AppType,
        //    //        RoleUUID = r.UUID
        //    //    };
        //    //    usersRole.Add(ur);

        //    //}

        //    ////this tests to make sure the user account matches the role account.
        //    ////if this fales (which it should for this test)
        //    ////then the Role has not been created for the account.
        //    ////
        //    //Assert.IsFalse(m.AddUsersToRole(usersRole));

        //    //for(int j = 0; j < _users.Count; j++)
        //    //    _users[j].AccountUUID = SystemFlag.Default.Account;

        //    ////There should be a default role, now assing the users account to the same as the
        //    ////role and this should be ok.
        //    ////
        //    //Assert.IsTrue(m.AddUsersToRole(usersRole));
        //}

        //[TestMethod]
        //public void RoleManager_DeleteUserFromRole()
        //{
        //    #region add user to role
        //    RoleManager m = new RoleManager(connectionKey);
        //    Role r = m.GetRole("customer",SystemFlag.Default.Account);
        //    Assert.IsNotNull(r);

        //    string name = Guid.NewGuid().ToString();
        //    UserRole ur = new UserRole()
        //    {
        //        UserUUID = _users[1].UUID,
        //        AccountUUID = _users[1].AccountUUID,
        //        RoleUUID = r.UUID,
        //        Name = name
        //    };
        //    Assert.IsTrue(m.AddUserToRole(ur));

        //    //IsUserInRole
        //    Assert.IsTrue(m.UserRoleExists(_users[1].UUID, _users[1].AccountUUID, r.UUID));
        //    #endregion

        //    Assert.IsTrue(m.DeleteUserFromRole(ur) > 0);
        //    Assert.IsFalse(m.UserRoleExists(_users[1].UUID, _users[1].AccountUUID, r.UUID));
        //    //RoleManager m = new RoleManager(connectionKey);

        //    //Role r = m.GetRole("admin");
        //    //Assert.IsNotNull(r);

        //    ////User has to be in the account the role is in. 
        //    //User u = _users[0];
        //    //u.AccountUUID = SystemFlag.Default.Account;
        //    //UserRole ur = new UserRole()
        //    //{
        //    //    AccountUUID = u.AccountUUID,
        //    //    UserUUID = u.UUID,
        //    //    AppType = SystemFlag.Default.AppType,
        //    //    RoleUUID = r.UUID
        //    //};
        //    //Assert.IsTrue(m.AddUserToRole(ur));

        //    //Assert.IsTrue(m.UserRoleExists(u.UUID, u.AccountUUID, r.UUID));

        //    //Assert.IsTrue(m.DeleteUserFromRole(ur) == 1);
        //}

        //[TestMethod]
        //public void RoleManager_DeleteUsersFromRole()
        //{
        //    #region Add users to role
        //    RoleManager m = new RoleManager(connectionKey);
        //    Role r = m.GetRole("patient", SystemFlag.Default.Account);
        //    Assert.IsNotNull(r);
        //    List<UserRole> userRoles = new List<UserRole>();
        //    string name = Guid.NewGuid().ToString();
        //    foreach (User u in _users)
        //    {
        //        UserRole ur = new UserRole()
        //        {
        //            UserUUID = u.UUID,
        //            AccountUUID = u.AccountUUID,
        //            RoleUUID = r.UUID,
        //            Name = name
        //        };
        //        userRoles.Add(ur);
        //    }

        //    m.AddUsersToRole(userRoles);

        //    //IsUserInRole
        //    foreach (User u in _users)
        //    {
        //        Assert.IsTrue(m.UserRoleExists(u.UUID, u.AccountUUID, r.UUID));
        //    }
        //    #endregion

        //    Assert.IsTrue(m.DeleteUsersFromRole(userRoles) > 0);

        //    foreach (User u in _users)
        //    {
        //        Assert.IsFalse(m.UserRoleExists(u.UUID, u.AccountUUID, r.UUID));
        //    }
        //    //RoleManager m = new RoleManager(connectionKey);

        //    //Role r = m.GetRole("admin");
        //    //Assert.IsNotNull(r);

        //    //List<UserRole> usersRole = new List<UserRole>();
        //    //for (int i = 0; i < 5; i++)
        //    //{
        //    //    ////User has to be in the account the role is in. 
        //    //    User u = _users[i];
        //    //    u.AccountUUID = SystemFlag.Default.Account;
        //    //    UserRole ur = new UserRole()
        //    //    {
        //    //        AccountUUID = u.AccountUUID,
        //    //        UserUUID = u.UUID,
        //    //        AppType = SystemFlag.Default.AppType,
        //    //        RoleUUID = r.UUID
        //    //    };
        //    //    usersRole.Add(ur);

        //    //}
        //    //Assert.IsTrue(m.AddUsersToRole(usersRole));

        //    //Assert.IsTrue(m.DeleteUsersFromRole(usersRole) == usersRole.Count);
        //}

        //#endregion

        //#region Permissions
        //[TestMethod]
        //public void RoleManager_CreatePermission()
        //{
        //    RoleManager m = new RoleManager(connectionKey);
        //    Assert.AreEqual( m.CreatePermission("alpha", "insert", "api/get/users/", "phone").Code,200);

        //    //backlog test to make sure everything after ? is stripped out
        //    //string perm = "Admin.test?error=You are  not  authorized";
        //}

        //[TestMethod]
        //public void RoleManager_GetPermissionsForRole()
        //{
        //    RoleManager m = new RoleManager(connectionKey);

        //    Role r = m.GetRole("admin", SystemFlag.Default.Account);
        //    Assert.IsNotNull(r);

        //    List<Permission> ps = m.GetPermissionsForRole(r.UUID, r.AccountUUID);

        //    Assert.IsTrue(ps.Count > 0);

        //    //backlog we could do a more in depth check to see if the id's actually match. Or do a dbcontext query here
        //    //TreeMonDbContext context = new TreeMonDbContext(connectionKey);
        //    //context.GetAll<Permission>().Where(w=> w.

        //    //Make sure we're not pulling stuff from thin air..
        //    Assert.IsFalse(m.GetPermissionsForRole("xyxy12234212", r.AccountUUID).Count > 0);
        //    Assert.IsFalse(m.GetPermissionsForRole(r.UUID, "sdfswdfq23").Count > 0);
        //    Assert.IsFalse(m.GetPermissionsForRole("xyxy12234212", "sdfswdfq23").Count > 0);
        //}

        //[TestMethod]
        //public void RoleManager_GetAccountPermissions()
        //{
        //   RoleManager m = new RoleManager(connectionKey);
        //    Role r = m.GetRole("admin",SystemFlag.Default.Account);
        //    Assert.IsNotNull(r);

        //    Assert.IsTrue(m.GetAccountPermissions(r.AccountUUID).Count > 0);
        //    Assert.IsFalse(m.GetAccountPermissions("sdfgasdfas").Count > 0);
        //}

        //[TestMethod]
        //public void RoleManager_GetAvailablePermissions()
        //{
        //    RoleManager m = new RoleManager(connectionKey);
        //    //the owner should have all permissions..
        //    Role owner = m.GetRole("owner", SystemFlag.Default.Account);
        //    Role customer = m.GetRole("customer", SystemFlag.Default.Account);

        //    //Start with a clean slate..
        //    //m.DeleteRole(owner, true);
        //    //m.DeleteRole(customer, true);
        //    //m.InsertDefaults(owner.AccountUUID, SystemFlag.Default.AppType);
        //    customer = m.GetRole("customer", SystemFlag.Default.Account);
        //    Assert.IsNotNull(customer);
        //    //customer should not have all permissions, so some will be available but not assigned.
        //    List<Permission> availablePermissions = m.GetAvailablePermissions(customer.UUID, customer.AccountUUID);
        //    Assert.IsTrue(availablePermissions.Count > 0);
        //    //Get all permissions for account.
        //    List<Permission> accountPermissions = m.GetAccountPermissions(customer.AccountUUID);
        //    //The number of permissions in the account should be greater than the available permissions for thie role.
        //    Assert.IsTrue(accountPermissions.Count() > availablePermissions.Count );


        //    owner = m.GetRole("owner", SystemFlag.Default.Account);
        //    Assert.IsNotNull(owner);
        //    availablePermissions = m.GetAvailablePermissions(owner.UUID, owner.AccountUUID);
        //    //The owner should have all permisssions.
        //   // Assert.IsTrue(availablePermissions.Count == 0);
        //    Assert.IsTrue(accountPermissions.Count() > availablePermissions.Count );
        //}

        //[TestMethod]
        //public void RoleManager_PermissionExists()
        //{
        //    RoleManager m = new RoleManager(connectionKey);
        //    Assert.IsTrue(m.PermissionExists("settings.update.update.system.default.app.system.default.account"));
        //}

     
        //#endregion
    }
}
