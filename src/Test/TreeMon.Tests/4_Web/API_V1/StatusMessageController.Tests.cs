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
using TreeMon.Models.General;
using TreeMon.Models.Membership;
using TreeMon.Models.Store;
using TreeMon.Utilites.Security;
using TreeMon.Web.Tests;

namespace TreeMon.Web.Tests.API.V1
{

    [TestClass]
    public class StatusMessagesControllerTests
    {
        private string connectionKey = "MSSQL_TEST";
        private string _ownerAuthToken = "";
        private string _captcha = "TESTCAPTCHA";

        [TestInitialize]
        public void TestSetup()
        {
            ServiceResult res = TestHelper.InitializeControllerTestData(connectionKey, "OWNER");
            Assert.AreNotEqual(res.Code, 500, res.Message);//should not be error. if it is assert.
            _ownerAuthToken = res.Result.ToString();
            Assert.IsNotNull(ConfigurationManager.AppSettings["DefaultDbConnection"]);
        }

        [TestMethod]
        public void Api_StatusMessageController_AddStatusMessage()
        {
            StatusMessage mdl = new StatusMessage();
            mdl.AccountUUID = SystemFlag.Default.Account;
            mdl.Status = Guid.NewGuid().ToString("N");
            mdl.UUID = Guid.NewGuid().ToString("N");

            string postData = JsonConvert.SerializeObject(mdl);

            Task.Run(async () =>
            {
                ServiceResult res = await TestHelper.SentHttpRequest("POST", "api/StatusMessages/Add", postData, _ownerAuthToken);
                Assert.IsNotNull(res);
                Assert.AreEqual(res.Code, 200);

                StatusMessage p = JsonConvert.DeserializeObject<StatusMessage>(res.Result.ToString());
                Assert.IsNotNull(p);
                TreeMonDbContext context = new TreeMonDbContext(connectionKey);
                StatusMessage dbStatusMessage = context.GetAll<StatusMessage>().Where(w => w.UUID == p.UUID).FirstOrDefault();
                Assert.IsNotNull(dbStatusMessage);
                Assert.AreEqual(mdl.Status, dbStatusMessage.Status);

            }).GetAwaiter().GetResult();

        }

        [TestMethod]
        public void Api_StatusMessageController_GetStatusMessage()
        {
            TreeMonDbContext context = new TreeMonDbContext(connectionKey);
            StatusMessage mdl = new StatusMessage();
            mdl.UUID = Guid.NewGuid().ToString("N");
            mdl.AccountUUID = SystemFlag.Default.Account;
            mdl.Status = Guid.NewGuid().ToString("N");
            mdl.DateCreated = DateTime.UtcNow;
            Assert.IsTrue(context.Insert<StatusMessage>(mdl));

            Task.Run(async () =>
            {
                ServiceResult res = await TestHelper.SentHttpRequest("GET", "api/StatusMessage/" + mdl.Status, "", _ownerAuthToken);

                Assert.IsNotNull(res);
                Assert.AreEqual(res.Code, 200);

                StatusMessage p = JsonConvert.DeserializeObject<StatusMessage>(res.Result.ToString());
                Assert.IsNotNull(p);
                Assert.AreEqual(mdl.Status, p.Status);

            }).GetAwaiter().GetResult();
        }

        [TestMethod]
        public void Api_StatusMessageController_GetStatusMessages()
        {
            TreeMonDbContext context = new TreeMonDbContext(connectionKey);
            StatusMessage mdl = new StatusMessage();
            mdl.AccountUUID = SystemFlag.Default.Account;
            mdl.Status = Guid.NewGuid().ToString("N");
            mdl.DateCreated = DateTime.UtcNow;
            Assert.IsTrue(context.Insert<StatusMessage>(mdl));

            StatusMessage mdl2 = new StatusMessage();
            mdl2.AccountUUID = SystemFlag.Default.Account;
            mdl2.Status = Guid.NewGuid().ToString("N");
            mdl2.DateCreated = DateTime.UtcNow;
            Assert.IsTrue(context.Insert<StatusMessage>(mdl2));

            Task.Run(async () =>
            {
                DataFilter filter = new DataFilter();
                filter.PageResults = false;
                ServiceResult res = await TestHelper.SentHttpRequest("POST", "api/StatusMessages/?filter="+ JsonConvert.SerializeObject(filter), "", _ownerAuthToken);

                Assert.IsNotNull(res);
                Assert.AreEqual(res.Code, 200);

                List<StatusMessage> StatusMessages = JsonConvert.DeserializeObject<List<StatusMessage>>(res.Result.ToString());
                Assert.IsNotNull(StatusMessages);
                Assert.IsTrue(StatusMessages.Count >= 2);

                int foundStatusMessages = 0;
                foreach (StatusMessage p in StatusMessages)
                {
                    if (p.Status == mdl.Status || p.Status == mdl2.Status)
                        foundStatusMessages++;

                }

                Assert.AreEqual(foundStatusMessages, 2);

            }).GetAwaiter().GetResult();
        }

        [TestMethod]
        public void Api_StatusMessageController_Get_StatusMessages_ByType()
        {
            //TreeMonDbContext context = new TreeMonDbContext(connectionKey);

            //User u = TestHelper.GenerateTestUser(Guid.NewGuid().ToString("N"));
            //u.SiteAdmin = true;
            //string loginPassword = u.Password;
            //string tmpHashPassword = PasswordHash.CreateHash(u.Password);
            //u.Password = PasswordHash.ExtractHashPassword(tmpHashPassword);
            //u.AccountUUID = SystemFlag.Default.Account;
            //u.PasswordHashIterations = PasswordHash.ExtractIterations(tmpHashPassword);
            //u.PasswordSalt = PasswordHash.ExtractSalt(tmpHashPassword);
            //u.DateCreated = DateTime.Now;
            //Assert.IsTrue(context.Insert<User>(u));

            //// set a user session then pass the authtoken
            //SessionManager sessionManager = new SessionManager(connectionKey);
            //string userJson = JsonConvert.SerializeObject(u);
            //UserSession us = sessionManager.SaveSession("127.1.1.34", u.UUID, userJson, false);

            //string statusType = Guid.NewGuid().ToString("N");
            //StatusMessage mdl = new StatusMessage();
            //mdl.AccountUUID = SystemFlag.Default.Account;
            //mdl.Status = Guid.NewGuid().ToString("N");
            //mdl.UUID = Guid.NewGuid().ToString("N");
            //mdl.DateCreated = DateTime.UtcNow;
            //mdl.CreatedBy = u.UUID;
            
            //mdl.StatusType = statusType;
            //Assert.IsTrue(context.Insert<StatusMessage>(mdl));

            //StatusMessage mdl2 = new StatusMessage();
            //mdl2.AccountUUID = SystemFlag.Default.Account;
            //mdl2.Status = Guid.NewGuid().ToString("N");
            //mdl2.UUID = Guid.NewGuid().ToString("N");
            //mdl2.CreatedBy =  u.UUID;
          
            //mdl2.DateCreated = DateTime.UtcNow;
            //mdl2.StatusType = statusType;
            //Assert.IsTrue(context.Insert<StatusMessage>(mdl2));

            //Task.Run(async () =>
            //{
            //    ServiceResult res = await TestHelper.SentHttpRequest("POST", "api/StatusMessages/Type/" + mdl.StatusType, "", us.AuthToken);

            //    Assert.IsNotNull(res);
            //    Assert.AreEqual(res.Code, 200);

            //    List<StatusMessage> StatusMessages = JsonConvert.DeserializeObject<List<StatusMessage>>(res.Result.ToString());
            //    Assert.IsNotNull(StatusMessages);
            //    Assert.IsTrue(StatusMessages.Count >= 2);

            //    int foundStatusMessages = 0;
            //    foreach (StatusMessage p in StatusMessages)
            //    {
            //        if (p.Status == mdl.Status || p.Status == mdl2.Status)
            //            foundStatusMessages++;
            //    }

            //    Assert.AreEqual(foundStatusMessages, 2);

            //}).GetAwaiter().GetResult();
        }


        [TestMethod]
        public void Api_StatusMessageController_DeleteStatusMessage()
        {
            TreeMonDbContext context = new TreeMonDbContext(connectionKey);
            StatusMessage mdl = new StatusMessage();
            mdl.AccountUUID = SystemFlag.Default.Account;
            mdl.Status = Guid.NewGuid().ToString("N");
            mdl.DateCreated = DateTime.UtcNow;
            mdl.UUID = Guid.NewGuid().ToString("N");
            Assert.IsTrue(context.Insert<StatusMessage>(mdl));
            string postData =  JsonConvert.SerializeObject(mdl);

            Task.Run(async () =>
            {
                ServiceResult res = await TestHelper.SentHttpRequest("POST", "api/StatusMessages/Delete", postData, _ownerAuthToken);

                Assert.IsNotNull(res);
                Assert.AreEqual(res.Code, 200);

                StatusMessage dbStatusMessage = context.GetAll<StatusMessage>().FirstOrDefault(w => w.Status == mdl.Status);
                Assert.IsNotNull(dbStatusMessage);
           

            }).GetAwaiter().GetResult();
        }

        [TestMethod]
        public void Api_StatusMessageController_UpdateStatusMessage()
        {
            TreeMonDbContext context = new TreeMonDbContext(connectionKey);
            StatusMessage mdl = new StatusMessage();
            mdl.AccountUUID = SystemFlag.Default.Account;
            mdl.DateCreated = DateTime.UtcNow;
            mdl.UUID = Guid.NewGuid().ToString("N");
            Assert.IsTrue(context.Insert<StatusMessage>(mdl));
            
            mdl = context.GetAll<StatusMessage>().FirstOrDefault(w => w.Status == mdl.Status);
            StatusMessage pv = new StatusMessage();
            pv.Id = mdl.Id;
            pv.UUID = mdl.UUID;
            pv.AccountUUID = mdl.AccountUUID;
              //~~~ Updatable fields ~~~
           
            string postData = JsonConvert.SerializeObject(pv);
            Task.Run(async () =>
            {
                ServiceResult res = await TestHelper.SentHttpRequest("POST", "api/StatusMessages/Update", postData, _ownerAuthToken);
                Assert.IsNotNull(res);
                Assert.AreEqual(res.Code, 200);

                StatusMessage dbStatusMessage = context.GetAll<StatusMessage>().Where(w => w.Status == mdl.Status).FirstOrDefault();
                Assert.IsNotNull(dbStatusMessage);
     
            }).GetAwaiter().GetResult();

        }

      

    }
}
