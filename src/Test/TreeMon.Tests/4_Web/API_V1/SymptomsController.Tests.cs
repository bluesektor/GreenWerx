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
using TreeMon.Models.Medical;
using TreeMon.Models.Membership;
using TreeMon.Models.Store;
using TreeMon.Web.Tests;

namespace TreeMon.Web.Tests.API.V1
{

    [TestClass]
    public class SymptomsControllerTests
    {
        private string connectionKey = "MSSQL_TEST";
        private string _ownerAuthToken = "";
   //     private string _captcha = "TESTCAPTCHA";

        [TestInitialize]
        public void TestSetup()
        {
            ServiceResult res = TestHelper.InitializeControllerTestData(connectionKey,  "OWNER");
            Assert.AreNotEqual(res.Code, 500, res.Message);//should not be error. if it is assert.
            _ownerAuthToken = res.Result.ToString();
            Assert.IsNotNull(ConfigurationManager.AppSettings["DefaultDbConnection"]);
        }

        [TestMethod]
        public void Api_SymptomController_AddSymptom()
        {
            Symptom mdl = new Symptom();
            mdl.AccountUUID = SystemFlag.Default.Account;
            mdl.Name = Guid.NewGuid().ToString("N");
            mdl.UUID = Guid.NewGuid().ToString("N");

            string postData = JsonConvert.SerializeObject(mdl);

            Task.Run(async () =>
            {
                ServiceResult res = await TestHelper.SentHttpRequest("POST", "api/Symptoms/Add", postData, _ownerAuthToken);
                Assert.IsNotNull(res);
                Assert.AreEqual(res.Code, 200);

                Symptom p = JsonConvert.DeserializeObject<Symptom>(res.Result.ToString());
                Assert.IsNotNull(p);
                TreeMonDbContext context = new TreeMonDbContext(connectionKey);
                Symptom dbSymptom = context.GetAll<Symptom>().Where(w => w.UUID == p.UUID).FirstOrDefault();
                Assert.IsNotNull(dbSymptom);
                Assert.AreEqual(mdl.Name, dbSymptom.Name);

            }).GetAwaiter().GetResult();

        }

        [TestMethod]
        public void Api_SymptomController_GetSymptom()
        {
            TreeMonDbContext context = new TreeMonDbContext(connectionKey);
            Symptom mdl = new Symptom();
            mdl.UUID = Guid.NewGuid().ToString("N");
            mdl.AccountUUID = SystemFlag.Default.Account;
            mdl.Name = Guid.NewGuid().ToString("N");
            mdl.DateCreated = DateTime.UtcNow;
            Assert.IsTrue(context.Insert<Symptom>(mdl));

            Task.Run(async () =>
            {
                ServiceResult res = await TestHelper.SentHttpRequest("GET", "api/Symptom/" + mdl.Name, "", _ownerAuthToken);

                Assert.IsNotNull(res);
                Assert.AreEqual(res.Code, 200);

                Symptom p = JsonConvert.DeserializeObject<Symptom>(res.Result.ToString());
                Assert.IsNotNull(p);
                Assert.AreEqual(mdl.Name, p.Name);

            }).GetAwaiter().GetResult();
        }

        [TestMethod]
        public void Api_SymptomController_GetSymptoms()
        {
            TreeMonDbContext context = new TreeMonDbContext(connectionKey);
            Symptom mdl = new Symptom();
            mdl.AccountUUID = SystemFlag.Default.Account;
            mdl.Name = Guid.NewGuid().ToString("N");
            mdl.DateCreated = DateTime.UtcNow;
            Assert.IsTrue(context.Insert<Symptom>(mdl));

            Symptom mdl2 = new Symptom();
            mdl2.AccountUUID = SystemFlag.Default.Account;
            mdl2.Name = Guid.NewGuid().ToString("N");
            mdl2.DateCreated = DateTime.UtcNow;
            Assert.IsTrue(context.Insert<Symptom>(mdl2));

            Task.Run(async () =>
            {
                DataFilter filter = new DataFilter();
                filter.PageResults = false;
                ServiceResult res = await TestHelper.SentHttpRequest("GET", "api/Symptoms/?filter=" + JsonConvert.SerializeObject(filter), "", _ownerAuthToken);

                Assert.IsNotNull(res);
                Assert.AreEqual(res.Code, 200);

                List<Symptom> Symptoms = JsonConvert.DeserializeObject<List<Symptom>>(res.Result.ToString());
                Assert.IsNotNull(Symptoms);
                Assert.IsTrue(Symptoms.Count >= 2);

                int foundSymptoms = 0;
                foreach (Symptom p in Symptoms)
                {
                    if (p.Name == mdl.Name || p.Name == mdl2.Name)
                        foundSymptoms++;

                }

                Assert.AreEqual(foundSymptoms, 2);

            }).GetAwaiter().GetResult();
        }

        [TestMethod]
        public void Api_SymptomController_DeleteSymptom()
        {
            TreeMonDbContext context = new TreeMonDbContext(connectionKey);
            Symptom mdl = new Symptom();
            mdl.AccountUUID = SystemFlag.Default.Account;
            mdl.Name = Guid.NewGuid().ToString("N");
            mdl.UUID = Guid.NewGuid().ToString("N");
            mdl.DateCreated = DateTime.Now;
            Assert.IsTrue(context.Insert<Symptom>(mdl));
            string postData =  JsonConvert.SerializeObject(mdl);

            Task.Run(async () =>
            {
                ServiceResult res = await TestHelper.SentHttpRequest("POST", "api/Symptoms/Delete", postData, _ownerAuthToken);

                Assert.IsNotNull(res);
                Assert.AreEqual(res.Code, 200);

                Symptom dbSymptom = context.GetAll<Symptom>().Where(w => w.Name == mdl.Name).FirstOrDefault();
                Assert.IsNotNull(dbSymptom);
                Assert.IsTrue(dbSymptom.Deleted);
                //Assert.IsNull(dbSymptom);

            }).GetAwaiter().GetResult();
        }

        [TestMethod]
        public void Api_SymptomController_UpdateSymptom()
        {
            TreeMonDbContext context = new TreeMonDbContext(connectionKey);
            Symptom mdl = new Symptom();
            mdl.AccountUUID = SystemFlag.Default.Account;
            mdl.Name = Guid.NewGuid().ToString("N");
            mdl.UUID = Guid.NewGuid().ToString("N");
            mdl.DateCreated = DateTime.Now;

            Assert.IsTrue(context.Insert<Symptom>(mdl));
            
            mdl = context.GetAll<Symptom>().Where(w => w.Name == mdl.Name).FirstOrDefault();
            Symptom pv = new Symptom();
            pv.Id = mdl.Id;
            pv.UUID = mdl.UUID;
            pv.AccountUUID = mdl.AccountUUID;
            pv.Name = mdl.Name;
            pv.DateCreated = DateTime.Now;
              //~~~ Updatable fields ~~~
           
            string postData = JsonConvert.SerializeObject(pv);
            Task.Run(async () =>
            {
                ServiceResult res = await TestHelper.SentHttpRequest("POST", "api/Symptoms/Update", postData, _ownerAuthToken);
                Assert.IsNotNull(res);
                Assert.AreEqual(res.Code, 200);

                Symptom dbSymptom = context.GetAll<Symptom>().Where(w => w.Name == mdl.Name).FirstOrDefault();
                Assert.IsNotNull(dbSymptom);
     
            }).GetAwaiter().GetResult();

        }

        [TestMethod]
        public void Api_SymptomController_SymptomSymptoms()
        {
            //NOTE: The last ReturnFormat listed is the one chosen to be returned.
            string uniqueCategory = Guid.NewGuid().ToString("N");
            string filters = "[{ \"SearchTerm\" :\""+ uniqueCategory+"\" , \"SearchBy\": \"Category\", \"ReturnFormat\":\"\" }]";

          
            TreeMonDbContext context = new TreeMonDbContext(connectionKey);
            Symptom mdl = new Symptom();
            mdl.AccountUUID = SystemFlag.Default.Account;
            mdl.Name = Guid.NewGuid().ToString("N");
            mdl.Category = uniqueCategory;
            mdl.AccountUUID = SystemFlag.Default.Account;
            mdl.DateCreated = DateTime.Now;
            Assert.IsTrue(context.Insert<Symptom>(mdl));

            Symptom mdl2 = new Symptom();
            mdl2.AccountUUID = SystemFlag.Default.Account;
            mdl2.Name = Guid.NewGuid().ToString("N");
            mdl2.Category = uniqueCategory;
            mdl2.AccountUUID = SystemFlag.Default.Account;
            mdl2.DateCreated = DateTime.Now;
            Assert.IsTrue(context.Insert<Symptom>(mdl2));

            Task.Run(async () =>
            {
                ServiceResult res = await TestHelper.SentHttpRequest("get", "api/Symptoms/?searchFilter=" + filters, "", _ownerAuthToken);

                Assert.IsNotNull(res);
                Assert.AreEqual(res.Code, 200);
                string records = res.Result.ToString();
                List<Symptom> selRes = JsonConvert.DeserializeObject<List<Symptom>>(res.Result.ToString());
                Assert.IsNotNull(selRes);
                Assert.IsTrue(selRes.Count >= 2);
            }).GetAwaiter().GetResult();
        }

    }
}
