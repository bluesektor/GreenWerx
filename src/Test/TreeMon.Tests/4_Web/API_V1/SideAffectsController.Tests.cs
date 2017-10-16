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
using TreeMon.Models.Medical;
using TreeMon.Models.Membership;
using TreeMon.Models.Store;
using TreeMon.Web.Tests;

namespace TreeMon.Web.Tests.API.V1
{

    [TestClass]
    public class SideAffectsControllerTests
    {
        private string connectionKey = "MSSQL_TEST";
        private string _ownerAuthToken = "";
        private string _captcha = "TESTCAPTCHA";

        [TestInitialize]
        public void TestSetup()
        {
            ServiceResult res = TestHelper.InitializeControllerTestData(connectionKey,  "OWNER");
            Assert.AreNotEqual(res.Code, 500, res.Message);//should not be error. if it is assert.
            _ownerAuthToken = res.Result.ToString();
            Assert.IsNotNull(ConfigurationManager.AppSettings["DefaultDbConnection"]);
        }


        [TestMethod]
        public void Api_SideAffectController_AddSideAffect()
        {
            SideAffect mdl = new SideAffect();
            mdl.AccountUUID = SystemFlag.Default.Account;
            mdl.Name = Guid.NewGuid().ToString("N");
            mdl.UUID = Guid.NewGuid().ToString("N");

            string postData = JsonConvert.SerializeObject(mdl);

            Task.Run(async () =>
            {
                ServiceResult res = await TestHelper.SentHttpRequest("POST", "api/SideAffects/Add", postData, _ownerAuthToken);
                Assert.IsNotNull(res);
                Assert.AreEqual(res.Code, 200);

                SideAffect p = JsonConvert.DeserializeObject<SideAffect>(res.Result.ToString());
                Assert.IsNotNull(p);
                TreeMonDbContext context = new TreeMonDbContext(connectionKey);
                SideAffect dbSideAffect = context.GetAll<SideAffect>().Where(w => w.UUID == p.UUID).FirstOrDefault();
                Assert.IsNotNull(dbSideAffect);
                Assert.AreEqual(mdl.Name, dbSideAffect.Name);

            }).GetAwaiter().GetResult();

        }

        [TestMethod]
        public void Api_SideAffectController_GetSideAffect()
        {
            TreeMonDbContext context = new TreeMonDbContext(connectionKey);
            SideAffect mdl = new SideAffect();
            mdl.UUID = Guid.NewGuid().ToString("N");
            mdl.AccountUUID = SystemFlag.Default.Account;
            mdl.Name = Guid.NewGuid().ToString("N");
            mdl.DateCreated = DateTime.Now;
            Assert.IsTrue(context.Insert<SideAffect>(mdl));

            Task.Run(async () =>
            {
                ServiceResult res = await TestHelper.SentHttpRequest("GET", "api/SideAffect/" + mdl.Name, "", _ownerAuthToken);

                Assert.IsNotNull(res);
                Assert.AreEqual(res.Code, 200);

                SideAffect p = JsonConvert.DeserializeObject<SideAffect>(res.Result.ToString());
                Assert.IsNotNull(p);
                Assert.AreEqual(mdl.Name, p.Name);

            }).GetAwaiter().GetResult();
        }

        [TestMethod]
        public void Api_SideAffectController_GetSideAffects()
        {
            string parentId = Guid.NewGuid().ToString("N");

            TreeMonDbContext context = new TreeMonDbContext(connectionKey);
            SideAffect mdl = new SideAffect();
            mdl.AccountUUID = SystemFlag.Default.Account;
            mdl.Name = Guid.NewGuid().ToString("N");
            mdl.UUParentID = parentId;
            mdl.DateCreated = DateTime.Now;
            Assert.IsTrue(context.Insert<SideAffect>(mdl));

            SideAffect mdl2 = new SideAffect();
            mdl2.AccountUUID = SystemFlag.Default.Account;
            mdl2.Name = Guid.NewGuid().ToString("N");
            mdl2.UUParentID = parentId;
            mdl2.DateCreated = DateTime.Now;
            Assert.IsTrue(context.Insert<SideAffect>(mdl2));

            Task.Run(async () =>
            {
                ServiceResult res = await TestHelper.SentHttpRequest("POST", "api/SideAffects/" +mdl.UUParentID, "", _ownerAuthToken);

                Assert.IsNotNull(res);
                Assert.AreEqual(res.Code, 200);

                List<SideAffect> SideAffects = JsonConvert.DeserializeObject<List<SideAffect>>(res.Result.ToString());
                Assert.IsNotNull(SideAffects);
                Assert.IsTrue(SideAffects.Count >= 2);

                int foundSideAffects = 0;
                foreach (SideAffect p in SideAffects)
                {
                    if (p.Name == mdl.Name || p.Name == mdl2.Name)
                        foundSideAffects++;

                }

                Assert.AreEqual(foundSideAffects, 2);

            }).GetAwaiter().GetResult();
        }

        [TestMethod]
        public void Api_SideAffectController_GetChildSideAffects()
        {
            string parentId = Guid.NewGuid().ToString("N");

            TreeMonDbContext context = new TreeMonDbContext(connectionKey);

            DoseLog dose = new DoseLog()
            {
                UUID = Guid.NewGuid().ToString("N"),
                 DateCreated = DateTime.UtcNow,
                  DoseDateTime = DateTime.UtcNow
            };
            Assert.IsTrue(context.Insert<DoseLog>(dose));

            SideAffect mdl = new SideAffect();
            mdl.AccountUUID = SystemFlag.Default.Account;
            mdl.Name = Guid.NewGuid().ToString("N");
            mdl.UUParentID = parentId;
            mdl.DoseUUID = dose.UUID;
            mdl.DateCreated = DateTime.Now;
            Assert.IsTrue(context.Insert<SideAffect>(mdl));

            SideAffect mdl2 = new SideAffect();
            mdl2.AccountUUID = SystemFlag.Default.Account;
            mdl2.Name = Guid.NewGuid().ToString("N");
            mdl2.UUParentID = parentId;
            mdl2.DoseUUID = dose.UUID;
            mdl2.DateCreated = DateTime.Now;
            Assert.IsTrue(context.Insert<SideAffect>(mdl2));

            Task.Run(async () =>
            {
                ServiceResult res = await TestHelper.SentHttpRequest("POST", "api/Doses/"+ dose.UUID +"/SideAffects/History/" + mdl.UUParentID, "", _ownerAuthToken);

                Assert.IsNotNull(res);
                Assert.AreEqual(res.Code, 200);

                List<SideAffect> SideAffects = JsonConvert.DeserializeObject<List<SideAffect>>(res.Result.ToString());
                Assert.IsNotNull(SideAffects);
                Assert.IsTrue(SideAffects.Count >= 2);

                int foundSideAffects = 0;
                foreach (SideAffect p in SideAffects)
                {
                    if (p.Name == mdl.Name || p.Name == mdl2.Name)
                        foundSideAffects++;

                }

                Assert.AreEqual(foundSideAffects, 2);

            }).GetAwaiter().GetResult();
        }


        [TestMethod]
        public void Api_SideAffectController_DeleteSideAffect()
        {
            TreeMonDbContext context = new TreeMonDbContext(connectionKey);
            SideAffect mdl = new SideAffect();
            mdl.AccountUUID = SystemFlag.Default.Account;
            mdl.Name = Guid.NewGuid().ToString("N");
            mdl.UUID = Guid.NewGuid().ToString("N");
            mdl.DateCreated = DateTime.Now;
            
            Assert.IsTrue(context.Insert<SideAffect>(mdl));
            string postData =  JsonConvert.SerializeObject(mdl);

            Task.Run(async () =>
            {
                ServiceResult res = await TestHelper.SentHttpRequest("POST", "api/SideAffects/Delete", postData, _ownerAuthToken);

                Assert.IsNotNull(res);
                Assert.AreEqual(res.Code, 200);

                SideAffect dbSideAffect = context.GetAll<SideAffect>().Where(w => w.Name == mdl.Name).FirstOrDefault();
                Assert.IsNotNull(dbSideAffect);
                Assert.IsTrue(dbSideAffect.Deleted);
                //Assert.IsNull(dbSideAffect);

            }).GetAwaiter().GetResult();
        }

        [TestMethod]
        public void Api_SideAffectController_UpdateSideAffect()
        {
            TreeMonDbContext context = new TreeMonDbContext(connectionKey);
            SideAffect mdl = new SideAffect();
            mdl.AccountUUID = SystemFlag.Default.Account;
            mdl.Name = Guid.NewGuid().ToString("N");
            mdl.UUID = Guid.NewGuid().ToString("N");
            mdl.DateCreated = DateTime.Now;
            Assert.IsTrue(context.Insert<SideAffect>(mdl));
            
            mdl = context.GetAll<SideAffect>().Where(w => w.Name == mdl.Name).FirstOrDefault();
            SideAffect pv = new SideAffect();
            pv.Id = mdl.Id;
            pv.UUID = mdl.UUID;
            pv.AccountUUID = mdl.AccountUUID;
            pv.Name = mdl.Name;
            pv.DateCreated = DateTime.Now;
            //~~~ Updatable fields ~~~

            string postData = JsonConvert.SerializeObject(pv);
            Task.Run(async () =>
            {
                ServiceResult res = await TestHelper.SentHttpRequest("POST", "api/SideAffects/Update", postData, _ownerAuthToken);
                Assert.IsNotNull(res);
                Assert.AreEqual(res.Code, 200);

                SideAffect dbSideAffect = context.GetAll<SideAffect>().Where(w => w.Name == mdl.Name).FirstOrDefault();
                Assert.IsNotNull(dbSideAffect);
     
            }).GetAwaiter().GetResult();

        }


    }
}
