﻿// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TreeMon.Data;
using TreeMon.Models.App;
using System.Collections.Generic;
using TreeMon.Managers.Membership;
using TreeMon.Models.Membership;
using System.Threading.Tasks;
using TreeMon.Utilites.Security;
using System.Configuration;
using Omni.Managers.Services;
using TreeMon.Models.Flags;
using System.Linq;
using TreeMon.Models.Services;
using TreeMon.Utilites.Extensions;

namespace TreeMon.Web.Tests._templates
{
    [TestClass]
    public class UserManager_Tests
    {
        private string connectionKey = "MSSQL_TEST";
        private string _ownerAuthToken = "";
        private string _providerUserKey = Cipher.RandomString(12);
        private string _userUUID = Guid.NewGuid().ToString("N");

        [TestInitialize]
        public void TestSetup()
        {
            _ownerAuthToken = TestHelper.InitializeControllerTestData(connectionKey, "OWNER").Result.ToString();
        }

        #region reimplement
        //[TestMethod]
        //public void UserManager_SendEmailValidationAsync()
        //{
        //    string name = Guid.NewGuid().ToString("N");

        //    //Check for supporting keys since it sends email
        //    Assert.IsNotNull(ConfigurationManager.AppSettings["SiteDomain"]);
        //    Assert.IsNotNull(ConfigurationManager.AppSettings["SiteEmail"]);
        //    Assert.IsNotNull(ConfigurationManager.AppSettings["AppKey"]);
        //    Assert.IsNotNull(ConfigurationManager.AppSettings["MailHost"]);
        //    Assert.IsNotNull(ConfigurationManager.AppSettings["MailPort"]);
        //    Assert.IsNotNull(ConfigurationManager.AppSettings["EmailHostUser"]);
        //    Assert.IsNotNull(ConfigurationManager.AppSettings["EmailHostPassword"]);
        //    Assert.IsNotNull(ConfigurationManager.AppSettings["UseSSL"]);
        //    Assert.Fail();
        //    //Task.Run(async () =>
        //    //{
        //    //    UserManager m = new UserManager(connectionKey);
        //    //    User u = new User() {
        //    //        Name = name,
        //    //         ProviderUserKey = Cipher.RandomString(12),
        //    //        Email = ConfigurationManager.AppSettings["SiteEmail"].ToString()
        //    //    };

        //    //    ServiceResult res = await m.SendEmailValidationAsync(u,u.ProviderUserKey, "127.0.0.1");
        //    //    Assert.AreEqual(res.Code, 200);
        //    //}).GetAwaiter().GetResult();
        //}

        //[TestMethod]
        //public void UserManager_SendPasswordResetEmailAsync()
        //{
        //    string name = Guid.NewGuid().ToString("N");

        //    //Check for supporting keys since it sends email
        //    Assert.IsNotNull(ConfigurationManager.AppSettings["SiteDomain"]);
        //    Assert.IsNotNull(ConfigurationManager.AppSettings["SiteEmail"]);
        //    Assert.IsNotNull(ConfigurationManager.AppSettings["AppKey"]);
        //    Assert.IsNotNull(ConfigurationManager.AppSettings["MailHost"]);
        //    Assert.IsNotNull(ConfigurationManager.AppSettings["MailPort"]);
        //    Assert.IsNotNull(ConfigurationManager.AppSettings["EmailHostUser"]);
        //    Assert.IsNotNull(ConfigurationManager.AppSettings["EmailHostPassword"]);
        //    Assert.IsNotNull(ConfigurationManager.AppSettings["UseSSL"]);
        //    Assert.IsNotNull(ConfigurationManager.AppSettings["TemplatePasswordResetEmail"]);

        //    Assert.Fail();
        //    //Task.Run(async () =>
        //    //{
        //    //    UserManager m = new UserManager(connectionKey);
        //    //    User u = new User()
        //    //    {
        //    //        Name = name,
        //    //        ProviderUserKey = Cipher.RandomString(12),
        //    //        Email = ConfigurationManager.AppSettings["SiteEmail"].ToString()
        //    //    };

        //    //     ServiceResult res = await m.SendPasswordResetEmailAsync(u, "127.1.1.2");
        //    //     Assert.AreEqual(res.Code, 200);

        //    //}).GetAwaiter().GetResult();
        //}

        //[TestMethod]
        //public void UserManager_SendUserInfoAsync()
        //{
        //    string name = Guid.NewGuid().ToString("N");

        //    //Check for supporting keys since it sends email
        //    Assert.IsNotNull(ConfigurationManager.AppSettings["SiteDomain"]);
        //    Assert.IsNotNull(ConfigurationManager.AppSettings["SiteEmail"]);
        //    Assert.IsNotNull(ConfigurationManager.AppSettings["AppKey"]);
        //    Assert.IsNotNull(ConfigurationManager.AppSettings["MailHost"]);
        //    Assert.IsNotNull(ConfigurationManager.AppSettings["MailPort"]);
        //    Assert.IsNotNull(ConfigurationManager.AppSettings["EmailHostUser"]);
        //    Assert.IsNotNull(ConfigurationManager.AppSettings["EmailHostPassword"]);
        //    Assert.IsNotNull(ConfigurationManager.AppSettings["UseSSL"]);
        //    Assert.IsNotNull(ConfigurationManager.AppSettings["TemplatePasswordResetEmail"]);

        //    Assert.Fail();
        //    //Task.Run(async () =>
        //    //{
        //    //    UserManager m = new UserManager(connectionKey);
        //    //    User u = new User()
        //    //    {
        //    //        Name = name,
        //    //        ProviderUserKey = Cipher.RandomString(12),
        //    //        Email = ConfigurationManager.AppSettings["SiteEmail"].ToString()
        //    //    };

        //    //    ServiceResult res = await m.SendUserInfoAsync(u, "127.1.1.3");
        //    //    Assert.AreEqual(res.Code, 200);
        //    //}).GetAwaiter().GetResult();
        //}


        //[TestMethod]
        //public void UserManager_RegisterUser()
        //{
        //    bool sendValidationEmail = false; //Make true to test email send.

        //    string name = Guid.NewGuid().ToString();
        //    //Check for supporting keys since it sends email
        //    Assert.IsNotNull(ConfigurationManager.AppSettings["SiteDomain"]);
        //    Assert.IsNotNull(ConfigurationManager.AppSettings["SiteEmail"]);
        //    Assert.IsNotNull(ConfigurationManager.AppSettings["AppKey"]);
        //    Assert.IsNotNull(ConfigurationManager.AppSettings["MailHost"]);
        //    Assert.IsNotNull(ConfigurationManager.AppSettings["MailPort"]);
        //    Assert.IsNotNull(ConfigurationManager.AppSettings["EmailHostUser"]);
        //    Assert.IsNotNull(ConfigurationManager.AppSettings["EmailHostPassword"]);
        //    Assert.IsNotNull(ConfigurationManager.AppSettings["UseSSL"]);

        //    UserManager m = new UserManager(connectionKey);
        //    UserRegister ug = new UserRegister();

        //    ServiceResult res = m.RegisterUser(ug, true, "127.1.1.4");
        //    Assert.AreEqual(res.Code, 500);
        //    Assert.IsTrue(res.Message.Contains("Name"));

        //    ug.Name = "test";
        //    ug.Password = "pwd";
        //    ug.Email = name + "@test.com";

        //    User u = m.GetUser("test");
        //    if (u != null)
        //        m.DeleteUser(u.UUID, true);

        //    ug.ConfirmPassword = "pwdc";

        //    res = m.RegisterUser(ug, true, "127.1.1.5");
        //    Assert.AreEqual(res.Code, 500);
        //    Assert.IsTrue(res.Message.Contains("Passwords"));

        //    ug.ConfirmPassword = "pwd";

        //    if (sendValidationEmail)
        //        ug.Email = ConfigurationManager.AppSettings["SiteEmail"].ToString();


        //    res = m.RegisterUser(ug, true, "127.1.1.6");
        //    Assert.AreEqual(res.Code, 200);

        //    ug.Name = name;
        //    res = m.RegisterUser(ug, true, "127.1.1.7");
        //    Assert.AreEqual(res.Code, 500);
        //    Assert.IsTrue(res.Message.Contains("email account you provided is already on registered"));
        //}
        #endregion

        [TestMethod]
        public void UserManager_RegisterUserAsync()
        {
            string name = Guid.NewGuid().ToString();

            Task.Run(async () =>
            {
                UserManager m = new UserManager(connectionKey, _ownerAuthToken);

                UserRegister ug = new UserRegister();

                ServiceResult res = await m.RegisterUserAsync(ug, true, "127.1.1.8");
                Assert.AreEqual( 500, res.Code);
                
                ug.Name = "test";
                ug.Password = "pwd";
                ug.Email = ConfigurationManager.AppSettings["SiteEmail"].ToString();
                User u = m.Search("test").FirstOrDefault();
                if (u != null)
                    m.Delete(u.UUID, true);

                ug.ConfirmPassword = "pwdc";

                res = await m.RegisterUserAsync(ug, true, "127.1.1.9");
                Assert.AreEqual( 500, res.Code);
                Assert.IsTrue(res.Message.Contains("Passwords"));

                ug.ConfirmPassword = "pwd";

                res = await m.RegisterUserAsync(ug, true, "127.1.1.10");

                Assert.AreEqual( 200, res?.Code);
                
                ug.Name = name;
                res = await m.RegisterUserAsync(ug, true, "127.1.1.11");
                Assert.AreEqual( 500, res?.Code);
                Assert.IsTrue(res.Message.Contains("already registered"));

            }).GetAwaiter().GetResult();
        }

        [TestMethod]
        public void UserManager_SendUserEmailValidationAsync()
        {
            Assert.IsNotNull(ConfigurationManager.AppSettings["SiteDomain"]);
            Assert.IsNotNull(ConfigurationManager.AppSettings["SiteEmail"]);
            Assert.IsNotNull(ConfigurationManager.AppSettings["AppKey"]);
            Assert.IsNotNull(ConfigurationManager.AppSettings["MailHost"]);
            Assert.IsNotNull(ConfigurationManager.AppSettings["MailPort"]);
            Assert.IsNotNull(ConfigurationManager.AppSettings["EmailHostUser"]);
            Assert.IsNotNull(ConfigurationManager.AppSettings["EmailHostPassword"]);
            Assert.IsNotNull(ConfigurationManager.AppSettings["UseSSL"]);
            Task.Run(async () =>
            {
                EmailSettings settings = new EmailSettings();

                string appKey = ConfigurationManager.AppSettings["AppKey"];
                string emailPassword = ConfigurationManager.AppSettings["EmailHostPassword"];
                // var testHostPassword = Cipher.Crypt(appKey,emailPassword, false);
                settings.HostPassword = ConfigurationManager.AppSettings["EmailHostPassword"];
                settings.EncryptionKey = appKey;
                settings.HostUser = ConfigurationManager.AppSettings["EmailHostUser"];
                settings.MailHost = ConfigurationManager.AppSettings["MailHost"];
                settings.MailPort = StringEx.ConvertTo<int>(ConfigurationManager.AppSettings["MailPort"]);
                settings.SiteDomain = ConfigurationManager.AppSettings["SiteDomain"];
                settings.SiteEmail = ConfigurationManager.AppSettings["SiteEmail"];
                settings.UseSSL = StringEx.ConvertTo<bool>(ConfigurationManager.AppSettings["UseSSL"]);

                UserManager userManager = new UserManager(connectionKey, _ownerAuthToken);
                userManager.DeleteByEmail("bluesektor@hotmail.com", true);
                UserRegister ug = new UserRegister();
                ug.Name = Guid.NewGuid().ToString();  
                ug.Password = "pwd";
                ug.ConfirmPassword = "pwd";
                ug.Email = "bluesektor@hotmail.com";
                
                ServiceResult res = await userManager.RegisterUserAsync(ug, false, "127.1.1.10");

                Assert.IsNotNull( res.Result);

                User u = (User)res.Result;
                u.ProviderUserKey = _providerUserKey;
                userManager.Update(u);

                ServiceResult emailRes = await userManager.SendUserEmailValidationAsync(u, u.ProviderUserKey, "127.0.0.11", settings);

                if (emailRes.Code != 200)
                {
                    Assert.Fail();
                }
            }).GetAwaiter().GetResult();
        }

        [TestMethod]
        public void UserManager_Validate()
        {
            UserManager userManager = new UserManager(connectionKey, _ownerAuthToken);
            string name = Guid.NewGuid().ToString("N");

            //Task.Run(async () =>
            //{
                User u = new User()
                {
                    Name = name,
                    ProviderUserKey = _providerUserKey,
                    ProviderName = UserFlags.ProviderName.SendAccountInfo,
                    Email = ConfigurationManager.AppSettings["SiteEmail"].ToString(),
                    Banned = true,
                    Approved = true
                };

                ////this should fail because couldn't find user
             ServiceResult res = userManager.Validate("type", "action", "validationcode", "127.1.1.21", u);
            Assert.AreEqual(500,res.Code);
            Assert.IsTrue(res.Message.Contains("Invalid verification code"));

            res = userManager.Insert(u);

            Assert.AreEqual(200,res.Code,  res.Message);

            res = userManager.Validate("mbr", "mreg", u.ProviderUserKey, "127.1.1.22",u);
            Assert.AreEqual(500,res.Code, res.Message);
            Assert.IsTrue(res.Message.Contains("banned"));

            u.Banned = false;
            Assert.IsTrue(userManager.Update(u).Code == 200);

            res =  userManager.Validate("mbr", "mreg", u.ProviderUserKey, "127.1.1.23",u);
            Assert.AreEqual(200,res.Code, res.Message);//should be ok. already approved
            Assert.IsTrue(res.Message.Contains("approved"));

            u.Approved = false;
            Assert.IsTrue(userManager.Update(u).Code == 200);

            //checks the case logic
            res = userManager.Validate("XSDFQ", "DW#$SDFA", u.ProviderUserKey, "127.1.1.24", u);
            Assert.AreEqual(500,res.Code, res.Message);
            Assert.IsTrue(res.Message.Contains("invalid url"));

            res = userManager.Validate("XSDFQ", "DdfesSDFA", u.ProviderUserKey, "127.1.1.25", u);
            Assert.AreEqual(500,res.Code, res.Message);
            Assert.IsTrue(res.Message.Contains("Invalid code"));

            res = userManager.Validate("mbr", "mreg", u.ProviderUserKey, "127.1.1.26", u);
            Assert.AreEqual(200,res.Code, res.Message);

            User approvedUser = (User)userManager.Get(u.UUID);
            Assert.IsNotNull(approvedUser);
            Assert.IsTrue(approvedUser.Approved);

            //make sure the table has the right data.
            u.ProviderName = UserFlags.ProviderName.SendAccountInfo;
            u.UserKey = _providerUserKey;
            Assert.IsTrue(userManager.Update(u).Code == 200);
            //password reset. This will always be 200 cause it does a redirect to the password reset page in the controller.
            res =  userManager.Validate("mbr", "pwdr", u.ProviderUserKey, "127.1.1.27", u);
            Assert.AreEqual(200, res.Code, res.Message);

            //resend verification email..
            res = userManager.Validate("mbr", "resend", u.ProviderUserKey, "127.1.1.28", u);
            Assert.AreEqual(200, res.Code, res.Message);

            res = userManager.Validate("mbr", "mdel", u.ProviderUserKey, "127.1.1.29", u);
            Assert.AreEqual(200, res.Code, res.Message);
            Assert.IsTrue(res.Message.Contains("deleted"));

            User deletedUser = (User)userManager.Get(u.UUID);
            Assert.IsNotNull(deletedUser);
            Assert.IsTrue(deletedUser.Deleted);

            //}).GetAwaiter().GetResult();
        }

        //[TestMethod]
        //public void UserManager_SendEmailAsync()
        //{
        //    Assert.IsNotNull(ConfigurationManager.AppSettings["SiteDomain"]);
        //    Assert.IsNotNull(ConfigurationManager.AppSettings["SiteEmail"]          );
        //    Assert.IsNotNull(ConfigurationManager.AppSettings["AppKey"]             );
        //    Assert.IsNotNull(ConfigurationManager.AppSettings["MailHost"]           );
        //    Assert.IsNotNull(ConfigurationManager.AppSettings["MailPort"]           );
        //    Assert.IsNotNull(ConfigurationManager.AppSettings["EmailHostUser"]      );
        //    Assert.IsNotNull(ConfigurationManager.AppSettings["EmailHostPassword"]  );
        //    Assert.IsNotNull(ConfigurationManager.AppSettings["UseSSL"]);
        //    Assert.Fail();
        //    //string email = ConfigurationManager.AppSettings["SiteEmail"].ToString();
        //    //Task.Run(async () =>
        //    //{
        //    //    UserManager m = new UserManager(connectionKey);
        //    //    string name = Guid.NewGuid().ToString();

        //    //    ServiceResult res= await m.SendEmailAsync("127.1.1.12", email, email, "test send", name);
        //    //    Assert.AreEqual(res.Code, 200);
        //    // }).GetAwaiter().GetResult();
        //}

        //[TestMethod]
        //public void UserManager_ClearSensitiveData()
        //{

        //    UserManager m = new UserManager(connectionKey);
        //    string name = Guid.NewGuid().ToString();
        //    string tmpHashPassword = PasswordHash.CreateHash("test");

        //    User u = new User()
        //    {
        //        AccountUUID = "a",
        //        Name = name,
        //        DateCreated = DateTime.UtcNow,
        //        Password = PasswordHash.ExtractHashPassword(tmpHashPassword),
        //        PasswordHashIterations = PasswordHash.ExtractIterations(tmpHashPassword),
        //        PasswordSalt = PasswordHash.ExtractSalt(tmpHashPassword),
        //        PasswordAnswer = "answer",
        //        MobilPin = "5551212",
        //        ProviderUserKey = "pyramid",
        //        UserKey = "triangle",
        //    };

        //   User t =   m.ClearSensitiveData(u);
        //    Assert.IsTrue(string.IsNullOrWhiteSpace(t.Password));
        //    Assert.IsTrue(string.IsNullOrWhiteSpace(t.PasswordAnswer));
        //    Assert.IsTrue(string.IsNullOrWhiteSpace(t.MobilPin));
        //    Assert.IsTrue(t.PasswordHashIterations == 0);
        //    Assert.IsTrue(string.IsNullOrWhiteSpace(t.PasswordSalt));
        //    Assert.IsTrue(string.IsNullOrWhiteSpace(t.ProviderUserKey));
        //    Assert.IsTrue(string.IsNullOrWhiteSpace(t.UserKey));
        //}

        //[TestMethod]
        //public void UserManager_ClearSensitiveData_List()
        //{

        //    UserManager m = new UserManager(connectionKey);
        //    string name = Guid.NewGuid().ToString();
        //    string tmpHashPassword = PasswordHash.CreateHash("test");

        //    List<User> users = new List<User>();
        //    for (int i = 0; i < 5; i++)
        //    {
        //        User u = new User()
        //        {
        //            AccountUUID = "a",
        //            Name = name,
        //            DateCreated = DateTime.UtcNow,
        //            Password = PasswordHash.ExtractHashPassword(tmpHashPassword),
        //            PasswordHashIterations = PasswordHash.ExtractIterations(tmpHashPassword),
        //            PasswordSalt = PasswordHash.ExtractSalt(tmpHashPassword),
        //            PasswordAnswer = "answer",
        //            MobilPin = "5551212",
        //            ProviderUserKey = "pyramid",
        //            UserKey = "triangle",
        //        };
        //        users.Add(u);
        //    }

        //    List<User> clearedUsers = m.ClearSensitiveData(users);
        //    foreach (User t in clearedUsers)
        //    {
        //        Assert.IsTrue(string.IsNullOrWhiteSpace(t.Password));
        //        Assert.IsTrue(string.IsNullOrWhiteSpace(t.PasswordAnswer));
        //        Assert.IsTrue(string.IsNullOrWhiteSpace(t.MobilPin));
        //        Assert.IsTrue(t.PasswordHashIterations == 0);
        //        Assert.IsTrue(string.IsNullOrWhiteSpace(t.PasswordSalt));
        //        Assert.IsTrue(string.IsNullOrWhiteSpace(t.ProviderUserKey));
        //        Assert.IsTrue(string.IsNullOrWhiteSpace(t.UserKey));
        //    }
        //}

        //[TestMethod]
        //public void UserManager_AuthenticateUser()
        //{

        //    UserManager m = new UserManager(connectionKey);
        //    string name = Guid.NewGuid().ToString();
        //    string tmpHashPassword = PasswordHash.CreateHash("test");
        //    User u = new User()
        //    {
        //        AccountUUID = "a",
        //        Name = name,
        //        DateCreated = DateTime.UtcNow,
        //        Password = PasswordHash.ExtractHashPassword(tmpHashPassword),
        //        PasswordHashIterations = PasswordHash.ExtractIterations(tmpHashPassword),
        //        PasswordSalt = PasswordHash.ExtractSalt(tmpHashPassword)
        //    };
        //    Assert.AreEqual(m.InsertUser(u).Code, 200);
        //    ServiceResult res = m.AuthenticateUser(name, "test", "127.1.1.13");
        //    Assert.AreEqual(res.Code, 500);
        //    Assert.IsTrue(res.Message.Contains("not been approved"));

        //    u.Approved = true;
        //    Assert.IsTrue(m.Update(u) > 0);

        //    Assert.AreEqual(m.AuthenticateUser(name, "test", "127.1.1.13").Code, 200);
        //}

        //[TestMethod]
        //public void UserManager_AuthenticateUserAsync()
        //{
        //    Task.Run(async () =>
        //    {
        //        UserManager m = new UserManager(connectionKey);
        //    string name = Guid.NewGuid().ToString();
        //    string tmpHashPassword = PasswordHash.CreateHash("test");
        //        User u = new User()
        //        {
        //            AccountUUID = "a",
        //            Name = name,
        //            DateCreated = DateTime.UtcNow,
        //            Password = PasswordHash.ExtractHashPassword(tmpHashPassword),
        //            PasswordHashIterations = PasswordHash.ExtractIterations(tmpHashPassword),
        //            PasswordSalt = PasswordHash.ExtractSalt(tmpHashPassword)
        //        };
        //        Assert.AreEqual(m.InsertUser(u).Code, 200);
        //        ServiceResult res =await m.AuthenticateUserAsync(name, "test", "127.1.1.14");
        //        Assert.AreEqual(res.Code, 500);
        //        Assert.IsTrue(res.Message.Contains("not been approved"));

        //        u.Approved = true;
        //        m = new UserManager(connectionKey);
        //        Assert.IsTrue(m.Update(u) > 0);
        //        res = await m.AuthenticateUserAsync(name, "test", "127.1.1.14");
        //        Assert.AreEqual(res.Code, 200);
        //    }).GetAwaiter().GetResult();
        //}

        //[TestMethod]
        //public void UserManager_SetUserFlag()
        //{
        //    UserManager m = new UserManager(connectionKey);
        //    string name = Guid.NewGuid().ToString();
        //    User u = new User()
        //    {
        //        AccountUUID = "a",
        //        Name = name,
        //        DateCreated = DateTime.UtcNow,             
        //        PasswordAnswer = "answer",
        //        MobilPin = "5551212",
        //        ProviderUserKey = "pyramid",
        //        UserKey = "triangle",
        //        Active = false,
        //        Deleted = false,
        //        Private = false,
        //        Anonymous = false,
        //        Approved = false,
        //        Banned = false,
        //        LockedOut = false,
        //        Online = false
        //    };
        //    Assert.AreEqual( m.InsertUser(u).Code, 200);

        //    Assert.AreEqual( m.SetUserFlag(u.UUID, "ACTIVE", "true").Code, 200);
        //    Assert.AreEqual( m.SetUserFlag(u.UUID, "DELETED", "true").Code, 200);
        //    Assert.AreEqual( m.SetUserFlag(u.UUID, "PRIVATE", "true").Code, 200);
        //    Assert.AreEqual( m.SetUserFlag(u.UUID, "Anonymous", "true").Code, 200);
        //    Assert.AreEqual( m.SetUserFlag(u.UUID, "Approved", "true").Code, 200);
        //    Assert.AreEqual( m.SetUserFlag(u.UUID, "Banned", "true").Code, 200);
        //    Assert.AreEqual( m.SetUserFlag(u.UUID, "LockedOut", "true").Code, 200);
        //    Assert.AreEqual( m.SetUserFlag(u.UUID, "Online", "true").Code, 200);
        //    Assert.AreEqual( m.SetUserFlag(u.UUID, "", "true").Code, 500);

        //    User t = m.GetUser(u.Name);
        //    Assert.IsNotNull(t);

        //    Assert.IsTrue(t.Active     ); 
        //    Assert.IsTrue(t.Deleted    );
        //    Assert.IsTrue(t.Private    );
        //    Assert.IsTrue(t.Anonymous  );
        //    Assert.IsTrue(t.Approved   );
        //    Assert.IsTrue(t.Banned     );
        //    Assert.IsTrue(t.LockedOut  );
        //    Assert.IsTrue(t.Online);
        //}

        //[TestMethod]
        //public void UserManager_LogUserProfile()
        //{
        //    UserManager m = new UserManager(connectionKey);
        //    string name = Guid.NewGuid().ToString();
        //    User u = new User()
        //    {
        //        AccountUUID = "a",
        //        Name = name,
        //        DateCreated = DateTime.UtcNow,
        //        PasswordAnswer = "answer",
        //        MobilPin = "5551212",
        //        ProviderUserKey = "pyramid",
        //        UserKey = "triangle"

        //    };
        //    Assert.AreEqual(m.InsertUser(u), 200);
        //    Profile p = new Profile() {
        //         AccountUUID = "a",
        //          Name = u.Name,
        //           UserUUID = u.UUID
        //    };

        //    Assert.AreEqual(m.LogUserProfile(p).Code, 200);

        //    Profile pt = m.GetCurrentProfile(u.UUID);
        //    Assert.IsNotNull(pt);
        //    Assert.AreEqual(pt.UserUUID, u.UUID);
        //    Assert.AreEqual(pt.UserUUID, p.UUID);

        //}

        //public void UserManager_GetCurrentProfile()
        //{
        //    UserManager m = new UserManager(connectionKey);
        //    string name = Guid.NewGuid().ToString();
        //    User u = new User()
        //    {
        //        AccountUUID = "a",
        //        Name = name,
        //        DateCreated = DateTime.UtcNow,
        //        PasswordAnswer = "answer",
        //        MobilPin = "5551212",
        //        ProviderUserKey = "pyramid",
        //        UserKey = "triangle"

        //    };
        //    Assert.AreEqual(m.InsertUser(u), 200);
        //    Profile p = new Profile()
        //    {
        //        AccountUUID = "a",
        //        Name = u.Name,
        //        UserUUID = u.UUID
        //    };

        //    Assert.AreEqual(m.LogUserProfile(p).Code, 200);
        //    Assert.IsNotNull(m.GetCurrentProfile(u.UUID));
        //}

        //[TestMethod]
        //public void UserManager_Insert_User()
        //{

        //    UserManager m = new UserManager(connectionKey);
        //    string name = Guid.NewGuid().ToString();
        //    Assert.AreEqual(
        //        m.InsertUser(new User()
        //        {
        //            AccountUUID = "a",
        //            Name = name,
        //            DateCreated = DateTime.UtcNow
        //        }), 200);

        //    //won't allow a duplicate name
        //    Assert.AreEqual(
        //      m.InsertUser(new User()
        //      {
        //          AccountUUID = "a",
        //          Name = name,
        //          DateCreated = DateTime.UtcNow
        //      }), 500);

        //    Assert.IsNotNull(m.GetUser(name));

        //}

        //[TestMethod]
        //public void UserManager_Insert_UserAsync()
        //{
        //    Task.Run(async () =>
        //    {
        //            UserManager m = new UserManager(connectionKey);
        //        string name = Guid.NewGuid().ToString();
        //        Assert.AreEqual(
        //           await m.InsertUserAsync(new User()
        //            {
        //                AccountUUID = "a",
        //                Name = name,
        //                DateCreated = DateTime.UtcNow
        //            }), 200);

        //        //won't allow a duplicate name
        //        Assert.AreEqual(
        //         await m.InsertUserAsync(new User()
        //          {
        //              AccountUUID = "a",
        //              Name = name,
        //              DateCreated = DateTime.UtcNow
        //          }), 500);

        //        Assert.IsNotNull(m.GetUser(name));
        //    }).GetAwaiter().GetResult();
        //}

        //[TestMethod]
        //public void UserManager_IsUserAuthorized()
        //{
        //    UserManager m = new UserManager(connectionKey);
        //    string name = Guid.NewGuid().ToString();
        //    User u = new User()
        //    {
        //        AccountUUID = "a",
        //        Name = name,
        //        DateCreated = DateTime.UtcNow,
        //        Deleted = false,
        //        LockedOut = false,
        //        Banned = false,
        //        Approved = true
        //    };

        //    Assert.AreEqual(m.IsUserAuthorized(u, "127.1.1.15").Code, 200);

        //    u.Deleted = true;
        //    Assert.AreEqual(m.IsUserAuthorized(u, "127.1.1.16").Code, 500);

        //    u.Deleted = false;
        //    u.Approved = false;
        //    Assert.AreEqual(m.IsUserAuthorized(u, "127.1.1.17").Code, 500);

        //    u.Approved = true;
        //    u.LockedOut = true;
        //    Assert.AreEqual(m.IsUserAuthorized(u, "127.1.1.19").Code, 500);

        //    u.LockedOut = false;
        //    u.Banned = true;
        //    Assert.AreEqual(m.IsUserAuthorized(u, "127.1.1.2").Code, 500);

        //    //sanity check
        //    u.Banned = false;
        //    Assert.AreEqual(m.IsUserAuthorized(u, "127.1.1.21").Code, 200);
        //}

        //[TestMethod]
        //public void UserManager_GetUser()
        //{
        //    UserManager m = new UserManager(connectionKey);
        //    string name = Guid.NewGuid().ToString();
        //    ServiceResult sr = m.InsertUser(new User()
        //    {
        //        AccountUUID = "a",
        //        Name = "ALPHA" + name,
        //        DateCreated = DateTime.UtcNow
        //    });

        //    Assert.AreEqual(sr.Code, 200, sr.Message);
        //    User s = m.GetUser("ALPHA" + name);
        //    Assert.IsNotNull(s);
        //}

        //[TestMethod]
        //public void UserManager_GetUserBy()
        //{
        //    UserManager m = new UserManager(connectionKey);
        //    User u = new User()
        //    {
        //        AccountUUID = "a",
        //        Name = "TESTRECORD",
        //        DateCreated = DateTime.UtcNow
        //    };

        //    Assert.AreEqual(m.InsertUser(u).Code, 200);
        //    User s = m.GetUser("TESTRECORD");
        //    Assert.IsNotNull(s);
        //    User suid = m.GetUserBy(s.UUID);
        //    Assert.IsNotNull(suid);
        //}

        //[TestMethod]
        //public void UserManager_GetUserByEmail()
        //{
        //    string email = Guid.NewGuid().ToString("N") +"@email.com";
        //    UserManager m = new UserManager(connectionKey);
        //    Assert.AreEqual(
        //      m.InsertUser(new User()
        //      {
        //           Email = email,
        //          AccountUUID = "a",
        //          Name = "TESTRECORD",
        //          DateCreated = DateTime.UtcNow
        //      }).Code, 200);

        //    User s = m.GetUserByEmail(email);
        //    Assert.IsNotNull(s);
        //    User suid = m.GetUserBy(s.UUID);
        //    Assert.IsNotNull(suid);
        //}

        //[TestMethod]
        //public void UserManager_GetUsers()
        //{
        //    UserManager m = new UserManager(connectionKey);
        //    Assert.IsTrue(m.GetUsers().Count > 0);
        //}

        //[TestMethod]
        //public void UserManager_GetUsers_ByAccountUUID()
        //{
        //    UserManager m = new UserManager(connectionKey);
        //    Assert.IsTrue(m.GetUsers("a").Count > 0);
        //}

        //[TestMethod]
        //public void UserManager_UpdateUser()
        //{
        //    UserManager m = new UserManager(connectionKey);
        //    m.InsertUser(new User()
        //    {
        //        AccountUUID = "a",
        //        Name = "TESTRECORD",
        //    });
        //    User s = m.GetUser("TESTRECORD");
        //    s.Name = "UPDATEDRECORD";

        //    Assert.IsTrue(m.Update(s)>0);
        //    User u = m.GetUser("UPDATEDRECORD");
        //    Assert.IsNotNull(u);
        //}

        //[TestMethod]
        //public void UserManager_DeleteUser()
        //{
        //    UserManager m = new UserManager(connectionKey);
        //    string name = Guid.NewGuid().ToString();
        //    User s = new User()
        //    {
        //        AccountUUID = "a",
        //        Name = name,
        //        DateCreated = DateTime.UtcNow,
        //    };

        //    m.InsertUser(s);

        //    //Test the delete flag
        //    Assert.IsTrue( m.DeleteUser(s.UUID) );
        //    m.GetUser("DELETERECORD");
        //    User d = m.GetUser(name);
        //    Assert.IsNotNull(d);
        //    Assert.IsTrue(d.Deleted == true);

        //    Assert.IsTrue( m.DeleteUser(s.UUID, true) );
        //    d = m.GetUser(name);
        //    Assert.IsNull(d);
        //}


    }
}
