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
using TreeMon.Models.Membership;
using TreeMon.Models.Store;
using TreeMon.Utilites.Security;
using TreeMon.Web.Tests;

namespace TreeMon.Web.Tests.API.V1
{

    [TestClass]
    public class SessionsControllerTests
    {
        private string connectionKey = "MSSQL_TEST";
        private string _ownerAuthToken = "";
        //private string _captcha = "TESTCAPTCHA";

        [TestInitialize]
        public void TestSetup()
        {
            ServiceResult res = TestHelper.InitializeControllerTestData(connectionKey, "OWNER");
            Assert.AreNotEqual(res.Code, 500, res.Message);//should not be error. if it is assert.
            _ownerAuthToken = res.Result.ToString();
            Assert.IsNotNull(ConfigurationManager.AppSettings["DefaultDbConnection"]);
        }

   
        [TestMethod]
        public void Api_UserSessionController_GetStatus()
        {
            Assert.IsNotNull(ConfigurationManager.AppSettings["AppKey"]);

            TreeMonDbContext context = new TreeMonDbContext(connectionKey);
            User u = TestHelper.GenerateTestUser(Guid.NewGuid().ToString("N"));
            string loginPassword = u.Password;
            string tmpHashPassword = PasswordHash.CreateHash(u.Password);
            u.Password = PasswordHash.ExtractHashPassword(tmpHashPassword);
            u.PasswordHashIterations = PasswordHash.ExtractIterations(tmpHashPassword);
            u.PasswordSalt = PasswordHash.ExtractSalt(tmpHashPassword);
            Assert.IsTrue(context.Insert<User>(u));

            // set a user session then pass the authtoken
             SessionManager sessionManager = new SessionManager(connectionKey);
            string userJson = JsonConvert.SerializeObject(u);
            UserSession us = sessionManager.SaveSession("127.1.1.34", u.UUID, userJson, false);

            Task.Run(async () =>
            {
                ServiceResult res = await TestHelper.SentHttpRequest("GET", "api/Sessions/Status/" + us.AuthToken, "", _ownerAuthToken);

                Assert.IsNotNull(res);
                Assert.AreEqual(res.Code, 200);
            }).GetAwaiter().GetResult();
        }

       [TestMethod]
        public void Api_UserSessionController_DeleteUserSession()
        {
            Assert.IsNotNull(ConfigurationManager.AppSettings["AppKey"]);

            TreeMonDbContext context = new TreeMonDbContext(connectionKey);

            User u = TestHelper.GenerateTestUser(Guid.NewGuid().ToString("N"));
            string loginPassword = u.Password;
            u.SiteAdmin = true;

            //  api/StatusMessages/Type/adfda6fe97774f6ea4b3f58f700c32e8
            
            string tmpHashPassword = PasswordHash.CreateHash(u.Password);
            u.Password = PasswordHash.ExtractHashPassword(tmpHashPassword);
            u.PasswordHashIterations = PasswordHash.ExtractIterations(tmpHashPassword);
            u.PasswordSalt = PasswordHash.ExtractSalt(tmpHashPassword);
            Assert.IsTrue(context.Insert<User>(u));

            // set a user session then pass the authtoken
             SessionManager sessionManager = new SessionManager(connectionKey);
            string userJson = JsonConvert.SerializeObject(u);
            UserSession us = sessionManager.SaveSession("127.1.1.35", u.UUID, userJson, false);

            string sessionInfo = "{ 'SessionId' : '" + us.AuthToken + "' , 'UserUUID' : '"+ u.UUID + "' }";

            Task.Run(async () =>
            {
                ServiceResult res = await TestHelper.SentHttpRequest("DELETE", "api/Sessions/Delete", sessionInfo, us.AuthToken);
                 Assert.IsNotNull(res);
                 Assert.AreEqual(res.Code, 200);

                 UserSession dbUserSession = context.GetAll<UserSession>().FirstOrDefault(w =>w.AuthToken == us.AuthToken);
                 Assert.IsNull(dbUserSession);
            }).GetAwaiter().GetResult();
        }
    }
}
