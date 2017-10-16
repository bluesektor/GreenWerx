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
using TreeMon.Models;
using TreeMon.Models.App;
using TreeMon.Models.Datasets;
using TreeMon.Models.Membership;
using TreeMon.Models.Store;
using TreeMon.Web.Tests;

namespace TreeMon.Web.Tests.API.V1
{

    [TestClass]
    public class UnitsOfMeasureControllerTests
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
        public void Api_UnitOfMeasureController_AssignUOMsToProductCategories()
        {
            TreeMonDbContext context = new TreeMonDbContext(connectionKey);
            User u = TestHelper.GenerateTestUser(Guid.NewGuid().ToString("N"));
            u.SiteAdmin = true;

            List<UnitOfMeasure> uoms = new List<UnitOfMeasure>();
            UnitOfMeasure mdl = new UnitOfMeasure();
            mdl.AccountUUID = SystemFlag.Default.Account;
            mdl.Name = Guid.NewGuid().ToString("N");
            mdl.UUID = Guid.NewGuid().ToString("N");
            mdl.DateCreated = DateTime.UtcNow;

            uoms.Add(mdl);
            UnitOfMeasure mdl2 = new UnitOfMeasure();
            mdl2.AccountUUID = SystemFlag.Default.Account;
            mdl2.Name = Guid.NewGuid().ToString("N");
            mdl2.DateCreated = DateTime.UtcNow;
            uoms.Add(mdl2);

            string postData = JsonConvert.SerializeObject(uoms);

            SessionManager sessionManager = new SessionManager(connectionKey);
            string userJson = JsonConvert.SerializeObject(u);
            UserSession us = sessionManager.SaveSession("127.1.1.35", u.UUID, userJson, false);


            Task.Run(async () =>
            {
                ServiceResult res = await TestHelper.SentHttpRequest("POST", "api/UnitsOfMeasure/ProductCategories/Assign", postData, us.AuthToken);
                Assert.IsNotNull(res);
                Assert.AreEqual(res.Code, 200);

            
                UnitOfMeasure dbUnitOfMeasure = context.GetAll<UnitOfMeasure>().Where(w => w.Name == mdl.Name).FirstOrDefault();
                Assert.IsNotNull(dbUnitOfMeasure);
                Assert.AreEqual(u.UUID, dbUnitOfMeasure.CreatedBy);

                dbUnitOfMeasure = context.GetAll<UnitOfMeasure>().Where(w => w.Name == mdl2.Name).FirstOrDefault();
                Assert.IsNotNull(dbUnitOfMeasure);
                Assert.AreEqual(u.UUID, dbUnitOfMeasure.CreatedBy);

            }).GetAwaiter().GetResult();

        }

        [TestMethod]
        public void Api_UnitOfMeasureController_AddUnitOfMeasure()
        {
            UnitOfMeasure mdl = new UnitOfMeasure();
            mdl.AccountUUID = SystemFlag.Default.Account;
            mdl.Name = Guid.NewGuid().ToString("N");
            mdl.UUID = Guid.NewGuid().ToString("N");
            mdl.DateCreated = DateTime.UtcNow;

            string postData = JsonConvert.SerializeObject(mdl);

            Task.Run(async () =>
            {
                ServiceResult res = await TestHelper.SentHttpRequest("POST", "api/UnitsOfMeasure/Add", postData, _ownerAuthToken);
                Assert.IsNotNull(res);
                Assert.AreEqual(res.Code, 200);

                UnitOfMeasure p = JsonConvert.DeserializeObject<UnitOfMeasure>(res.Result.ToString());
                Assert.IsNotNull(p);
                TreeMonDbContext context = new TreeMonDbContext(connectionKey);
                UnitOfMeasure dbUnitOfMeasure = context.GetAll<UnitOfMeasure>().Where(w => w.UUID == p.UUID).FirstOrDefault();
                Assert.IsNotNull(dbUnitOfMeasure);
                Assert.AreEqual(mdl.Name, dbUnitOfMeasure.Name);

            }).GetAwaiter().GetResult();

        }

        [TestMethod]
        public void Api_UnitOfMeasureController_GetUnitOfMeasure()
        {
            TreeMonDbContext context = new TreeMonDbContext(connectionKey);
            UnitOfMeasure mdl = new UnitOfMeasure();
            mdl.UUID = Guid.NewGuid().ToString("N");
            mdl.AccountUUID = SystemFlag.Default.Account;
            mdl.Name = Guid.NewGuid().ToString("N");
            mdl.DateCreated = DateTime.UtcNow;
            Assert.IsTrue(context.Insert<UnitOfMeasure>(mdl));

            Task.Run(async () =>
            {
                ServiceResult res = await TestHelper.SentHttpRequest("GET", "api/UnitsOfMeasure/" + mdl.Name, "", _ownerAuthToken);

                Assert.IsNotNull(res);
                Assert.AreEqual(res.Code, 200);

                UnitOfMeasure p = JsonConvert.DeserializeObject<UnitOfMeasure>(res.Result.ToString());
                Assert.IsNotNull(p);
                Assert.AreEqual(mdl.Name, p.Name);

            }).GetAwaiter().GetResult();
        }

        [TestMethod]
        public void Api_UnitOfMeasureController_GetUnitsOfMeasure()
        {
            TreeMonDbContext context = new TreeMonDbContext(connectionKey);
            UnitOfMeasure mdl = new UnitOfMeasure();
            mdl.AccountUUID = SystemFlag.Default.Account;
            mdl.Name = Guid.NewGuid().ToString("N");
            mdl.DateCreated = DateTime.UtcNow;
            Assert.IsTrue(context.Insert<UnitOfMeasure>(mdl));

            UnitOfMeasure mdl2 = new UnitOfMeasure();
            mdl2.AccountUUID = SystemFlag.Default.Account;
            mdl2.Name = Guid.NewGuid().ToString("N");
            mdl2.DateCreated = DateTime.UtcNow;
            Assert.IsTrue(context.Insert<UnitOfMeasure>(mdl2));

            Task.Run(async () =>
            {
                DataFilter filter = new DataFilter();
                filter.PageResults = false;
                ServiceResult res = await TestHelper.SentHttpRequest("POST", "api/UnitsOfMeasure/?filter=" + JsonConvert.SerializeObject(filter), "", _ownerAuthToken);

                Assert.IsNotNull(res);
                Assert.AreEqual(res.Code, 200);

                List<UnitOfMeasure> UnitOfMeasures = JsonConvert.DeserializeObject<List<UnitOfMeasure>>(res.Result.ToString());
                Assert.IsNotNull(UnitOfMeasures);
                Assert.IsTrue(UnitOfMeasures.Count >= 2);

                int foundUnitOfMeasures = 0;
                foreach (UnitOfMeasure p in UnitOfMeasures)
                {
                    if (p.Name == mdl.Name || p.Name == mdl2.Name)
                        foundUnitOfMeasures++;

                }

                Assert.AreEqual(foundUnitOfMeasures, 2);

            }).GetAwaiter().GetResult();
        }

        [TestMethod]
        public void Api_UnitOfMeasureController_DeleteUnitOfMeasure()
        {
            TreeMonDbContext context = new TreeMonDbContext(connectionKey);
            UnitOfMeasure mdl = new UnitOfMeasure();
            mdl.AccountUUID = SystemFlag.Default.Account;
            mdl.Name = Guid.NewGuid().ToString("N");
            mdl.DateCreated = DateTime.UtcNow;
            mdl.UUID = Guid.NewGuid().ToString("N");
            Assert.IsTrue(context.Insert<UnitOfMeasure>(mdl));
            string postData =  JsonConvert.SerializeObject(mdl);

            Task.Run(async () =>
            {
                ServiceResult res = await TestHelper.SentHttpRequest("POST", "api/UnitsOfMeasure/Delete", postData, _ownerAuthToken);

                Assert.IsNotNull(res);
                Assert.AreEqual(res.Code, 200);

                UnitOfMeasure dbUnitOfMeasure = context.GetAll<UnitOfMeasure>().Where(w => w.Name == mdl.Name).FirstOrDefault();
                Assert.IsNotNull(dbUnitOfMeasure);
                Assert.IsTrue(dbUnitOfMeasure.Deleted);
                //Assert.IsNull(dbUnitOfMeasure);

            }).GetAwaiter().GetResult();
        }

        [TestMethod]
        public void Api_UnitOfMeasureController_UpdateUnitOfMeasure()
        {
            TreeMonDbContext context = new TreeMonDbContext(connectionKey);
            UnitOfMeasure mdl = new UnitOfMeasure();
            mdl.AccountUUID = SystemFlag.Default.Account;
            mdl.Name = Guid.NewGuid().ToString("N");
            mdl.UUID = Guid.NewGuid().ToString("N");
            mdl.DateCreated = DateTime.UtcNow;

            Assert.IsTrue(context.Insert<UnitOfMeasure>(mdl));
            
            mdl = context.GetAll<UnitOfMeasure>().Where(w => w.Name == mdl.Name).FirstOrDefault();
            UnitOfMeasure pv = new UnitOfMeasure();
            pv.Id = mdl.Id;
            pv.UUID = mdl.UUID;
            pv.AccountUUID = mdl.AccountUUID;
            pv.Name = mdl.Name;
              //~~~ Updatable fields ~~~
           
            string postData = JsonConvert.SerializeObject(pv);
            Task.Run(async () =>
            {
                ServiceResult res = await TestHelper.SentHttpRequest("POST", "api/UnitsOfMeasure/Update", postData, _ownerAuthToken);
                Assert.IsNotNull(res);
                Assert.AreEqual(res.Code, 200);

                UnitOfMeasure dbUnitOfMeasure = context.GetAll<UnitOfMeasure>().Where(w => w.Name == mdl.Name).FirstOrDefault();
                Assert.IsNotNull(dbUnitOfMeasure);
     
            }).GetAwaiter().GetResult();

        }

    }
}
