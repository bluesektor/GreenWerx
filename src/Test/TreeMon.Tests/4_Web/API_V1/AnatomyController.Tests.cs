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
using TreeMon.Models.App;
using TreeMon.Models.Datasets;
using TreeMon.Models.Medical;
using TreeMon.Models.Membership;

namespace TreeMon.Web.Tests.API.V1
{

    [TestClass]
    public class AnatomyControllerTests
    {
        private string connectionKey = "MSSQL_TEST";
        private string _ownerAuthToken = "";
       // private string _captcha = "TESTCAPTCHA";

        [TestInitialize]
        public void TestSetup()
        {
            ServiceResult res   = TestHelper.InitializeControllerTestData(connectionKey,  "OWNER");
            Assert.AreNotEqual(res.Code, 500, res.Message);//should not be error. if it is assert.
            _ownerAuthToken = res.Result.ToString();

            Assert.IsNotNull(ConfigurationManager.AppSettings["DefaultDbConnection"]);
        }

        [TestMethod]
        public void Api_AnatomyController_Add_Anatomy()
        {
            Anatomy mdl = new Anatomy();
            mdl.AccountUUID = SystemFlag.Default.Account;
            mdl.Name = Guid.NewGuid().ToString("N");
            mdl.UUID = Guid.NewGuid().ToString("N");
     
            string postData = JsonConvert.SerializeObject(mdl);

            Task.Run(async () =>
            {
                ServiceResult res = await TestHelper.SentHttpRequest("POST", "api/Anatomy/Add", postData, _ownerAuthToken);
                Assert.IsNotNull(res);
                Assert.AreEqual(res.Code, 200);

                Anatomy p = JsonConvert.DeserializeObject<Anatomy>(res.Result.ToString());
                Assert.IsNotNull(p);
                TreeMonDbContext context = new TreeMonDbContext(connectionKey);
                Anatomy dbAnatomy = context.GetAll<Anatomy>().Where(w => w.UUID == p.UUID).FirstOrDefault();
                Assert.IsNotNull(dbAnatomy);
                Assert.AreEqual(mdl.Name, dbAnatomy.Name);

            }).GetAwaiter().GetResult();

        }

        [TestMethod]
        public void Api_AnatomyController_Get_Anatomy()
        {
            TreeMonDbContext context = new TreeMonDbContext(connectionKey);
            Anatomy mdl = new Anatomy();
            mdl.UUID = Guid.NewGuid().ToString("N");
            mdl.AccountUUID = SystemFlag.Default.Account;
            mdl.Name = Guid.NewGuid().ToString("N");
            mdl.DateCreated = DateTime.Now;
          
            Assert.IsTrue(context.Insert<Anatomy>(mdl));

            Task.Run(async () =>
            {
                ServiceResult res = await TestHelper.SentHttpRequest("GET", "api/Anatomy/" + mdl.Name, "", _ownerAuthToken);

                Assert.IsNotNull(res);
                Assert.AreEqual(res.Code, 200);

                Anatomy p = JsonConvert.DeserializeObject<Anatomy>(res.Result.ToString());
                Assert.IsNotNull(p);
                Assert.AreEqual(mdl.Name, p.Name);

            }).GetAwaiter().GetResult();
        }

        [TestMethod]
        public void Api_AnatomyController_Get_Anatomy_()
        {
            TreeMonDbContext context = new TreeMonDbContext(connectionKey);
            Anatomy mdl = new Anatomy();
            mdl.AccountUUID = SystemFlag.Default.Account;
            mdl.Name = Guid.NewGuid().ToString("N");
            mdl.DateCreated = DateTime.Now;
            Assert.IsTrue(context.Insert<Anatomy>(mdl));

            Anatomy mdl2 = new Anatomy();
            mdl2.AccountUUID = SystemFlag.Default.Account;
            mdl2.Name = Guid.NewGuid().ToString("N");
            mdl2.DateCreated = DateTime.Now;
            Assert.IsTrue(context.Insert<Anatomy>(mdl2));

            Task.Run(async () =>
            {
                DataFilter filter = new DataFilter();
                filter.PageResults = false;
                ServiceResult res = await TestHelper.SentHttpRequest("POST", "api/Anatomy/?filter="+ JsonConvert.SerializeObject(filter) , "", _ownerAuthToken);

                Assert.IsNotNull(res);
                Assert.AreEqual(res.Code, 200);

                List<Anatomy> Anatomy = JsonConvert.DeserializeObject<List<Anatomy>>(res.Result.ToString());
                Assert.IsNotNull(Anatomy);
                Assert.IsTrue(Anatomy.Count >= 2);

                int foundAnatomy = 0;
                foreach (Anatomy p in Anatomy)
                {
                    if (p.Name == mdl.Name || p.Name == mdl2.Name)
                        foundAnatomy++;

                }

                Assert.AreEqual(foundAnatomy, 2);

            }).GetAwaiter().GetResult();
        }

        //[TestMethod]
        //public void Api_AnatomyController_Get_Anatomy_ByCategory()
        //{
        //    TreeMonDbContext context = new TreeMonDbContext(connectionKey);
        //    Anatomy mdl = new Anatomy();
        //    mdl.AccountUUID = SystemFlag.Default.Account;
        //    mdl.Name = Guid.NewGuid().ToString("N");
        //    mdl.UUID = Guid.NewGuid().ToString("N");
        //    Assert.IsTrue(context.Insert<Anatomy>(mdl));

        //    Anatomy mdl2 = new Anatomy();
        //    mdl2.AccountUUID = SystemFlag.Default.Account;
        //    mdl2.Name = Guid.NewGuid().ToString("N");
        //    mdl.UUID = Guid.NewGuid().ToString("N");
        //    Assert.IsTrue(context.Insert<Anatomy>(mdl2));

        //    Task.Run(async () =>
        //    {
        //        ServiceResult res = await TestHelper.SentHttpRequest("POST", "api/Anatomy/Categories/" + mdl.Category, "", _ownerAuthToken);

        //        Assert.IsNotNull(res);
        //        Assert.AreEqual(res.Code, 200);

        //        List<Anatomy> Anatomy = JsonConvert.DeserializeObject<List<Anatomy>>(res.Result.ToString());
        //        Assert.IsNotNull(Anatomy);
        //        Assert.IsTrue(Anatomy.Count >= 2);

        //        int foundAnatomy = 0;
        //        foreach (Anatomy p in Anatomy)
        //        {
        //            if (p.Name == mdl.Name || p.Name == mdl2.Name)
        //                foundAnatomy++;
        //        }

        //        Assert.AreEqual(foundAnatomy, 2);

        //    }).GetAwaiter().GetResult();
        //}

        [TestMethod]
        public void Api_AnatomyController_Delete_Anatomy()
        {
            TreeMonDbContext context = new TreeMonDbContext(connectionKey);
            Anatomy mdl = new Anatomy();
            mdl.AccountUUID = SystemFlag.Default.Account;
            mdl.Name = Guid.NewGuid().ToString("N");
            mdl.UUID = Guid.NewGuid().ToString("N");
            mdl.DateCreated = DateTime.Now;
            Assert.IsTrue(context.Insert<Anatomy>(mdl));
            string postData =  JsonConvert.SerializeObject(mdl);

            Task.Run(async () =>
            {
                ServiceResult res = await TestHelper.SentHttpRequest("POST", "api/Anatomy/Delete", postData, _ownerAuthToken);

                Assert.IsNotNull(res);
                Assert.AreEqual(res.Code, 200);

                Anatomy dbAnatomy = context.GetAll<Anatomy>().Where(w => w.Name == mdl.Name).FirstOrDefault();
                Assert.IsNotNull(dbAnatomy);
                Assert.IsTrue(dbAnatomy.Deleted);
                //Assert.IsNull(dbAnatomy);

            }).GetAwaiter().GetResult();
        }

        [TestMethod]
        public void Api_AnatomyController_Update_Anatomy()
        {
            TreeMonDbContext context = new TreeMonDbContext(connectionKey);
            Anatomy mdl = new Anatomy();
            mdl.AccountUUID = SystemFlag.Default.Account;
            mdl.Name = Guid.NewGuid().ToString("N");
            mdl.UUID = Guid.NewGuid().ToString("N");
            mdl.DateCreated = DateTime.Now;
            Assert.IsTrue(context.Insert<Anatomy>(mdl));
            
            mdl = context.GetAll<Anatomy>().FirstOrDefault(w => w.Name == mdl.Name);
            Anatomy pv = new Anatomy();
            pv.UUID = mdl.UUID;
            pv.AccountUUID = mdl.AccountUUID;
            pv.Name = mdl.Name;
            //~~~ Updatable fields ~~~
           
            string postData = JsonConvert.SerializeObject(pv);
            Task.Run(async () =>
            {
                ServiceResult res = await TestHelper.SentHttpRequest("POST", "api/Anatomy/Update", postData, _ownerAuthToken);
                Assert.IsNotNull(res);
                Assert.AreEqual(res.Code, 200);

                Anatomy dbAnatomy = context.GetAll<Anatomy>().Where(w => w.Name == mdl.Name).FirstOrDefault();
                Assert.IsNotNull(dbAnatomy);
           

            }).GetAwaiter().GetResult();

        }
    }
}
