// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TreeMon.Data;
using TreeMon.Models.App;
using System.Collections.Generic;
using TreeMon.Managers;
using System.Configuration;
using TreeMon.Utilites.Helpers;
using System.IO;
using TreeMon.Models.Membership;
using System.Linq;
using TreeMon.Utilites.Security;
using TreeMon.Managers.Membership;

namespace TreeMon.Web.Tests._templates
{
    [TestClass]
    public class AppManager_Tests
    {
        // //private string connectionKey = "MSSQL_TEST";
        //private string connectionKey = "MSSQL";
        //AppTypes are web, forms, mobil(for mobile phone app, not mobile theme).
       // private string appType = "web";


        //[TestMethod]
        //public void AppManager_Install()
        //{
        //    Assert.IsNotNull(ConfigurationManager.AppSettings["AppKey"]);
        //    Assert.IsNotNull(ConfigurationManager.AppSettings["SiteEmail"]);

        //    string dbUsername = Guid.NewGuid().ToString("N");
        //    string username = Guid.NewGuid().ToString("N");

        //    #region make sure install commands are coppied to app_data\install.cmd path

        //    string pathToInstallCommands = EnvironmentEx.AppDataFolder + "\\Install\\install.json";
        //    Assert.IsTrue(File.Exists(pathToInstallCommands));

        //    if(!File.Exists(EnvironmentEx.AppDataFolder + "\\Install\\install.json"))
        //        File.Copy(pathToInstallCommands, EnvironmentEx.AppDataFolder + "\\Install\\install.json");

        //    Assert.IsTrue(File.Exists(EnvironmentEx.AppDataFolder + "\\Install\\install.json"));

        //    #endregion

        //    AppManager m = new AppManager(connectionKey, appType);

        //    AppInfo setting = new AppInfo();

        //    Assert.AreEqual(m.Install(setting).Code, 500);
        //    Assert.IsTrue(m.Install(setting).Message.Contains("Connection key is not set"));

        //    setting.ActiveDbConnectionKey = connectionKey;

        //    #region  ValidateInstallSettings()

        //    Assert.AreEqual(m.Install(setting).Code, 500);
        //    Assert.IsTrue(m.Install(setting).Message.Contains("Account email not set"));
        //    //This would be whatever the user typed into the form.
        //    setting.AccountEmail = ConfigurationManager.AppSettings["SiteEmail"].ToString();

        //    Assert.AreEqual(m.Install(setting).Code, 500);
        //    Assert.IsTrue(m.Install(setting).Message.Contains("Account name not set"));
        //    //This would be whatever the user typed into the form.
        //    setting.AccountName = username;

        //    Assert.AreEqual(m.Install(setting).Code, 500);
        //    Assert.IsTrue(m.Install(setting).Message.Contains("Invalid apptype"));

        //    setting.AppType = appType;

        //    Assert.AreEqual(m.Install(setting).Code, 500);
        //    Assert.IsTrue(m.Install(setting).Message.Contains("Invalid Database provider"));

        //    setting.ActiveDbProvider = "MSSQL";

        //    Assert.AreEqual(m.Install(setting).Code, 500);
        //    Assert.IsTrue(m.Install(setting).Message.Contains("Database Username is empty"));

        //    setting.ActiveDbUser = dbUsername;

        //    Assert.AreEqual(m.Install(setting).Code, 500);
        //    Assert.IsTrue(m.Install(setting).Message.Contains("Database password is empty"));


        //    setting.ActiveDbPassword = "dppassword";

        //    Assert.AreEqual(m.Install(setting).Code, 500);
        //    Assert.IsTrue(m.Install(setting).Message.Contains("Database name is empty"));

        //    setting.ActiveDatabase = "TreemonSystemTest";

        //    //if (AppCommands.Count == 0 || (AppCommands.Count > 0 && AppCommands.Contains("web.install") == false))
        //    //    return new ServiceResult() { Code = 500, Status = "ERROR", Message = "Application is already installed." };
      

        //    Assert.AreEqual(m.Install(setting).Code, 500);
        //    Assert.IsTrue(m.Install(setting).Message.Contains("Username is empty"));

        //    setting.UserName = username;

        //    setting.UserPassword = "pass";

        //    Assert.AreEqual(m.Install(setting).Code, 500);
        //    Assert.IsTrue(m.Install(setting).Message.Contains("Passwords don't match"));

        //    setting.ConfirmPassword= setting.UserPassword;

        //    Assert.AreEqual(m.Install(setting).Code, 500);
        //    Assert.IsTrue(m.Install(setting).Message.Contains("Password is too weak"));

        //    setting.UserPassword = "userPassword";
        //    setting.ConfirmPassword = setting.UserPassword;


        //    Assert.AreEqual(m.Install(setting).Code, 500);
        //    Assert.IsTrue(m.Install(setting).Message.Contains("Email is empty"));

        //    setting.UserEmail = ConfigurationManager.AppSettings["SiteEmail"].ToString();


        //    Assert.AreEqual(m.Install(setting).Code, 500);
        //    Assert.IsTrue(m.Install(setting).Message.Contains("Security question is empty"));

        //    setting.SecurityQuestion = "question";

        //    Assert.AreEqual(m.Install(setting).Code, 500);
        //    Assert.IsTrue(m.Install(setting).Message.Contains("Security answer is empty"));

        //    setting.UserSecurityAnswer = "answer";//backlog make sure this is encrypted
        //    #endregion

        //    ServiceResult res = m.Install(setting);
        //    Assert.AreEqual(res.Code, 200);

        //    TreeMonDbContext context = new TreeMonDbContext( connectionKey );

        //    #region Insert Account Check

        //    Account acct = context.GetAll<Account>().Where(w => w.Email?.ToUpper() == setting.AccountEmail?.ToUpper() && w.Name.ToUpper() == setting.AccountName?.ToUpper()).FirstOrDefault();// && w.OwnerType == "user.username"
        //    Assert.IsNotNull(acct);
        //    Assert.IsTrue(acct.UUIDType == "Account");
        //    Assert.IsTrue(acct.AccountSource == setting.AccountName);
        //    Assert.IsTrue(acct.Active);
          
        //    Assert.IsTrue(acct.OwnerUUID == setting.UserName);
        //    Assert.IsTrue(acct.OwnerType == "user.username");
        //    Assert.IsTrue(acct.DateCreated != DateTime.MinValue);
        //    Assert.IsFalse(acct.Deleted);
        //    Assert.IsTrue(acct.Email == setting.AccountEmail);
        //    Assert.IsTrue(acct.Name == setting.AccountName);
        //    Assert.IsTrue(acct.Private); // appSettings.AccountIsPrivate

        //    #endregion

        //    #region Check that the appsettings were inserted
        //    AppInfo dbInfo = m.GetAppInfo(acct.UUID, appType);
        //    //  AppInfo dbInfo = context.GetAll<AppInfo>().Where(w => w.UserName  == username && w.ActiveDbUser == dbUsername).FirstOrDefault();
        //    Assert.IsNotNull(dbInfo);

        //   Assert.AreEqual(dbInfo.ActiveDbProvider      , setting.ActiveDbProvider       );
        //   Assert.AreEqual(dbInfo.ActiveDbUser          , setting.ActiveDbUser           );
        //   Assert.AreEqual(dbInfo.ActiveDbPassword      , setting.ActiveDbPassword       );
        //   Assert.AreEqual(dbInfo.ActiveDatabase        , setting.ActiveDatabase         );
        //   Assert.AreEqual(dbInfo.UserName              , setting.UserName               );
        //   Assert.AreEqual(dbInfo.UserEmail             , setting.UserEmail              );
        //   Assert.AreEqual(dbInfo.UserPassword          , setting.UserPassword           );
        //   Assert.AreEqual(dbInfo.SecurityQuestion, setting.SecurityQuestion);
        //    Assert.AreEqual(dbInfo.SecurityQuestion, setting.SecurityQuestion);

        //    #endregion

        //    #region Insert user check

        //    User u = context.GetAll<User>().Where(w => w.Name == setting.UserName && w.Email == setting.UserEmail).FirstOrDefault();
        //    Assert.IsNotNull(u);

        //     Assert.IsTrue(acct.CreatedBy == u.UUID);//Account check 

        //    //  Assert.AreEqual(u.Password, setting.UserPassword);
        //    Assert.AreEqual(u.PasswordQuestion, setting.SecurityQuestion);
        //    Assert.AreEqual(u.PasswordAnswer, setting.UserSecurityAnswer);

        //    Assert.IsTrue(u.UUIDType == "guid-N");
        //    //Assert.IsTrue(u.Password == PasswordHash.ExtractHashPassword(u.Password));
        //    //Assert.IsTrue(u.PasswordSalt == PasswordHash.ExtractSalt(u.Password));
        //    //Assert.IsTrue(u.PasswordHashIterations == PasswordHash.ExtractIterations(u.Password));
        //    Assert.IsTrue(u.DateCreated != DateTime.MinValue);
        //    Assert.IsTrue(u.AccountUUID== acct.UUID);
        //    Assert.IsTrue(u.SiteAdmin);     //note this is only hard coded for here
        //    Assert.IsTrue(u.Approved);       //note this is only hard coded for here
        //    Assert.IsFalse(u.Anonymous);
        //    Assert.IsFalse(u.Banned);
        //    Assert.IsFalse(u.LockedOut);

        //    Assert.IsTrue(u.Private); // Since its a site admin we'll make it private  appSettings.UserIsPrivate,
        //    Assert.IsTrue(u.LastActivityDate != DateTime.MinValue);
        //    Assert.IsTrue(u.FailedPasswordAnswerAttemptWindowStart == 0);
        //    Assert.IsTrue(u.FailedPasswordAttemptCount == 0);
        //    Assert.IsTrue(u.FailedPasswordAnswerAttemptCount == 0);

        //    #endregion

        //    #region Check to make sure the user is a member of the account added above

        //    AccountMember actMember = context.GetAll<AccountMember>().Where(w => w.AccountUUID== u.AccountUUID&& w.MemberUUID == u.UUID && w.MemberType == "user").FirstOrDefault();
        //    Assert.IsNotNull(actMember);
        //    Assert.IsTrue(actMember.Status == "active");

        //    #endregion

        //    #region check to make sure default roles are added.
        //    //backlog v2 put role creation commands in the install file
        //    RoleManager rm = new RoleManager(connectionKey);
        //     List<Role> roles =  rm.GetRoles();
        //     Assert.IsTrue(roles.Count >= rm.DefaultRoles.Count());

        //    foreach (string roleName in rm.DefaultRoles) {
        //        Assert.IsNotNull(rm.GetRole(roleName,acct.UUID));
        //     }

        //    List<Permission> permissions = rm.GetAccountPermissions(acct.UUID);
        //    Assert.IsTrue(permissions.Count > 0);

        //    permissions = rm.GetPermissionsForRole(roles[0].UUID, roles[0].AccountUUID);
        //    Assert.IsTrue(permissions.Count > 0);

        //    List<RolePermission> rolePermissions =  rm.GetRolePermissions(roles[0].UUID,roles[0].AccountUUID);
        //    Assert.IsTrue(rolePermissions.Count > 0);

        //    //make sure the above user is in a role
        //    Assert.IsNotNull(rm.GetUsersInRole(rolePermissions[0].RoleUUID, rolePermissions[0].AccountUUID).Where(w => w.UUID == u.UUID));
        //    #endregion

        //    #region make sure install commands are deleted
        //    Assert.IsFalse(File.Exists(EnvironmentEx.AppDataFolder + "\\install.cmd"));
        //    #endregion

            
           
        //}

        //[TestMethod]
        //public void AppManager_SeedDatabases()
        //{
        //    AppManager app = new AppManager(connectionKey, "web");
        //    string directory = AppDomain.CurrentDomain.BaseDirectory.Replace("\\bin\\Debug", "");
        //    ServiceResult res=    app.SeedDatabase(Path.Combine(directory, "App_Data\\SeedData\\"));
        //    Assert.AreEqual(res.Code, 200);
        //}



        //[TestMethod]
        //public void AppManager_Insert_Setting()
        //{
        //    AppManager m = new AppManager(connectionKey, appType);
        //    string name = Guid.NewGuid().ToString();
        //    Assert.Fail();
        //    //Assert.AreEqual(
        //    //    m.Insert(new Setting()
        //    //    {
        //    //        SettingClass = "TESTCLASS", Type = "STRING", Value = "testValue",
        //    //        AccountUUID = "a",
        //    //        Name = name, AppType = appType
                    
        //    //    })
        //    // .Code, 200);
        //}

        //[TestMethod]
        //public void AppManager_GetSetting()
        //{
        //    AppManager m = new AppManager(connectionKey, appType);
        //    string name = Guid.NewGuid().ToString();
        //    Assert.Fail();
        //    //ServiceResult sr = m.Insert(new Setting()
        //    //{
        //    //    SettingClass = "TESTCLASS", Type = "STRING", Value = "testValue",
        //    //    AccountUUID= "a",
        //    //    Name = "ALPHA" + name,
        //    //});

        //    //Assert.AreEqual(sr.Code, 200, sr.Message);
        //    //Setting s = m.GetSetting("ALPHA" + name);
        //  //  Assert.IsNotNull(s);
        //}

        //[TestMethod]
        //public void AppManager_GetSettingBy()
        //{
        //    AppManager m = new AppManager(connectionKey, appType);
        //    Assert.AreEqual(
        //      m.Insert(new Setting()
        //      {
        //           SettingClass = "TESTCLASS", Type = "STRING", Value = "testValue",
        //          AccountUUID= "a",
        //          Name = "TESTRECORD",
        //      }, "encryption.key")
        //   .Code, 200);
        //    Setting s = m.GetSetting("TESTRECORD");
        //    Assert.IsNotNull(s);
        //    Setting suid = m.GetSettingBy(s.UUID);
        //    Assert.IsNotNull(suid);
        //}

        //[TestMethod]
        //public void AppManager_GetSettings()
        //{
        //    AppManager m = new AppManager(connectionKey, appType);
        //    Assert.IsTrue(m.GetSettings(appType, "TESTCLASS").Count > 0);
        //}

        //[TestMethod]
        //public void AppManager_UpdateSetting()
        //{
        //    AppManager m = new AppManager(connectionKey, appType);
        //    m.Insert(new Setting()
        //    {
        //        SettingClass = "TESTCLASS", Type = "STRING", Value = "testValue",
        //        AccountUUID= "a",
        //        Name = "TESTRECORD",
        //    }, "encryption.key");
        //    Setting s = m.GetSetting("TESTRECORD");
        //    s.Name = "UPDATEDRECORD";

        //    Assert.AreEqual(m.UpdateSetting(s, Globals.Application.AppSetting("AppKey")).Code, 200);
        //    Setting u = m.GetSetting("UPDATEDRECORD");
        //    Assert.IsNotNull(u);
        //}

        //[TestMethod]
        //public void AppManager_DeleteSetting()
        //{
        //    AppManager m = new AppManager(connectionKey, appType);
        //    string name = Guid.NewGuid().ToString();
        //    Setting s = new Setting()
        //    {
        //        SettingClass = "TESTCLASS", Type = "STRING", Value = "testValue",
        //        AccountUUID= "a",
        //        Name = name, AppType = appType
        //    };

            
        //    m.Insert(s,"encryption.key");

        //    //Test the delete flag
        //    //Assert.IsTrue(m.DeleteSetting(s) > 0);
        //    //m.GetSetting("DELETERECORD");
        //    Setting d = m.GetSetting(name);
        //    //Assert.IsNotNull(d);
        //    //Assert.IsTrue(d.Deleted == true);

        //    Assert.AreEqual(m.DeleteSetting(s.UUID).Code,200);
        //    d = m.GetSetting(name);
        //    Assert.IsNull(d);
        //}
    }
}
