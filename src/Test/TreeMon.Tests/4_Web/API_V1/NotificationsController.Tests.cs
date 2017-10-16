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
using TreeMon.Models.Event;
using TreeMon.Models.Membership;
using TreeMon.Models.Store;
using TreeMon.Web.Tests;

namespace TreeMon.Web.Tests.API.V1
{

    [TestClass]
    public class NotificationControllerTests
    {
        private string connectionKey = "MSSQL_TEST";
        private string _ownerAuthToken = "";
       // private string _captcha = "TESTCAPTCHA";

        [TestInitialize]
        public void TestSetup()
        {
            ServiceResult res = TestHelper.InitializeControllerTestData(connectionKey,  "OWNER");
            Assert.AreNotEqual(res.Code, 500, res.Message);//should not be error. if it is assert.
            _ownerAuthToken = res.Result.ToString();
            Assert.IsNotNull(ConfigurationManager.AppSettings["DefaultDbConnection"]);
        }

        [TestMethod]
        public void Api_NotificationController_AddNotification()
        {
            Notification mdl = new Notification();
            mdl.AccountUUID = SystemFlag.Default.Account;
            mdl.Name = Guid.NewGuid().ToString("N");
            mdl.UUID = Guid.NewGuid().ToString("N");

            string postData = JsonConvert.SerializeObject(mdl);

            Task.Run(async () =>
            {
                ServiceResult res = await TestHelper.SentHttpRequest("POST", "api/Notifications/Add", postData, _ownerAuthToken);
                Assert.IsNotNull(res);
                Assert.AreEqual(res.Code, 200);

                Notification p = JsonConvert.DeserializeObject<Notification>(res.Result.ToString());
                Assert.IsNotNull(p);
                TreeMonDbContext context = new TreeMonDbContext(connectionKey);
                Notification dbNotification = context.GetAll<Notification>().Where(w => w.UUID == p.UUID).FirstOrDefault();
                Assert.IsNotNull(dbNotification);
                Assert.AreEqual(mdl.Name, dbNotification.Name);

            }).GetAwaiter().GetResult();

        }

        [TestMethod]
        public void Api_NotificationController_GetNotification()
        {
            TreeMonDbContext context = new TreeMonDbContext(connectionKey);
            Notification mdl = new Notification();
            mdl.UUID = Guid.NewGuid().ToString("N");
            mdl.AccountUUID = SystemFlag.Default.Account;
            mdl.Name = Guid.NewGuid().ToString("N");
            mdl.DateCreated = DateTime.Now;
            mdl.DateFirstViewed = DateTime.Now;
            mdl.DateLastViewed = DateTime.Now;

            Assert.IsTrue(context.Insert<Notification>(mdl));

            Task.Run(async () =>
            {
                ServiceResult res = await TestHelper.SentHttpRequest("GET", "api/Notifications/" + mdl.Name, "", _ownerAuthToken);

                Assert.IsNotNull(res);
                Assert.AreEqual(res.Code, 200);

                Notification p = JsonConvert.DeserializeObject<Notification>(res.Result.ToString());
                Assert.IsNotNull(p);
                Assert.AreEqual(mdl.Name, p.Name);

            }).GetAwaiter().GetResult();
        }

        [TestMethod]
        public void Api_NotificationController_GetNotifications()
        {
            TreeMonDbContext context = new TreeMonDbContext(connectionKey);
            Notification mdl = new Notification();
            mdl.AccountUUID = SystemFlag.Default.Account;
            mdl.Name = Guid.NewGuid().ToString("N");
            mdl.DateCreated = DateTime.Now;
            Assert.IsTrue(context.Insert<Notification>(mdl));

            Notification mdl2 = new Notification();
            mdl2.AccountUUID = SystemFlag.Default.Account;
            mdl2.Name = Guid.NewGuid().ToString("N");
            mdl2.DateCreated = DateTime.Now;
            Assert.IsTrue(context.Insert<Notification>(mdl2));

            Task.Run(async () =>
            {
                DataFilter filter = new DataFilter();
                filter.PageResults = false;
                ServiceResult res = await TestHelper.SentHttpRequest("POST", "api/Notifications/?filter=" + JsonConvert.SerializeObject(filter), "", _ownerAuthToken);

                Assert.IsNotNull(res);
                Assert.AreEqual(res.Code, 200);

                List<Notification> Notifications = JsonConvert.DeserializeObject<List<Notification>>(res.Result.ToString());
                Assert.IsNotNull(Notifications);
                Assert.IsTrue(Notifications.Count >= 2);

                int foundNotifications = 0;
                foreach (Notification p in Notifications)
                {
                    if (p.Name == mdl.Name || p.Name == mdl2.Name)
                        foundNotifications++;

                }

                Assert.AreEqual(foundNotifications, 2);

            }).GetAwaiter().GetResult();
        }

        [TestMethod]
        public void Api_NotificationController_DeleteNotification()
        {
            TreeMonDbContext context = new TreeMonDbContext(connectionKey);
            Notification mdl = new Notification();
            mdl.AccountUUID = SystemFlag.Default.Account;
            mdl.Name = Guid.NewGuid().ToString("N");
            mdl.UUID = Guid.NewGuid().ToString("N");
            mdl.DateCreated = DateTime.Now;

            Assert.IsTrue(context.Insert<Notification>(mdl));
            string postData =  JsonConvert.SerializeObject(mdl);

            Task.Run(async () =>
            {
                ServiceResult res = await TestHelper.SentHttpRequest("POST", "api/Notifications/Delete", postData, _ownerAuthToken);

                Assert.IsNotNull(res);
                Assert.AreEqual(res.Code, 200);

                Notification dbNotification = context.GetAll<Notification>().Where(w => w.Name == mdl.Name).FirstOrDefault();
                Assert.IsNotNull(dbNotification);
                Assert.IsTrue(dbNotification.Deleted);
                //Assert.IsNull(dbNotification);

            }).GetAwaiter().GetResult();
        }

        [TestMethod]
        public void Api_NotificationController_UpdateNotification()
        {
            TreeMonDbContext context = new TreeMonDbContext(connectionKey);
            Notification mdl = new Notification();
            mdl.AccountUUID = SystemFlag.Default.Account;
            mdl.Name = Guid.NewGuid().ToString("N");
            mdl.UUID = Guid.NewGuid().ToString("N");
            mdl.DateCreated = DateTime.Now;
            Assert.IsTrue(context.Insert<Notification>(mdl));
            
            mdl = context.GetAll<Notification>().Where(w => w.Name == mdl.Name).FirstOrDefault();
            Notification pv = new Notification();
            pv.UUID = mdl.UUID;
            pv.AccountUUID = mdl.AccountUUID;
            pv.Name = mdl.Name;
              //~~~ Updatable fields ~~~
           
            string postData = JsonConvert.SerializeObject(pv);
            Task.Run(async () =>
            {
                ServiceResult res = await TestHelper.SentHttpRequest("POST", "api/Notifications/Update", postData, _ownerAuthToken);
                Assert.IsNotNull(res);
                Assert.AreEqual(res.Code, 200);

                Notification dbNotification = context.GetAll<Notification>().Where(w => w.Name == mdl.Name).FirstOrDefault();
                Assert.IsNotNull(dbNotification);
     
            }).GetAwaiter().GetResult();

        }

    }
}
