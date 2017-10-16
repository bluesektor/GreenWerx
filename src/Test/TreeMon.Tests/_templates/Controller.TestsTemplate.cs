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
using TreeMon.Web.Models;
using TreeMon.Web.Tests;

namespace TreeMon.Web.Tests.API.V1
{

    [TestClass]
    public class MODELNAMEsControllerTests
    {
        private string connectionKey = "MSSQL_TEST";
        private string _ownerAuthToken = "";
        private string _captcha = "TESTCAPTCHA";

        [TestInitialize]
        public void TestSetup()
        {
            _ownerAuthToken = TestHelper.InitializeControllerTestData(connectionKey, "OWNER");
            Assert.IsNotNull(ConfigurationManager.AppSettings["DefaultDbConnection"]);
        }

        [TestMethod]
        public void Api_MODELNAMEController_AddMODELNAME()
        {
            MODELNAMEView mdl = new MODELNAMEView();
            mdl.AccountId = SystemFlag.Default.Account;//todo create SystemFlag.Test.Account
            mdl.Name = Guid.NewGuid().ToString("N");
            mdl.UUID = Guid.NewGuid().ToString("N");
            mdl.Captcha = _captcha;

            string postData = JsonConvert.SerializeObject(mdl);

            Task.Run(async () =>
            {
                ServiceResult res = await TestHelper.SentHttpRequest("POST", "api/MODELNAMEs/Add/", postData, _ownerAuthToken);
                Assert.IsNotNull(res);
                Assert.AreEqual(res.Code, 200);

                MODELNAME p = JsonConvert.DeserializeObject<MODELNAME>(res.Record.ToString());
                Assert.IsNotNull(p);
                TreeMonDbContext context = new TreeMonDbContext(connectionKey);
                MODELNAME dbMODELNAME = context.GetAll<MODELNAME>().Where(w => w.UUID == p.UUID).FirstOrDefault();
                Assert.IsNotNull(dbMODELNAME);
                Assert.AreEqual(mdl.Name, dbMODELNAME.Name);

            }).GetAwaiter().GetResult();

        }

        [TestMethod]
        public void Api_MODELNAMEController_GetMODELNAME()
        {
            TreeMonDbContext context = new TreeMonDbContext(connectionKey);
            MODELNAME mdl = new MODELNAME();
            mdl.UUID = Guid.NewGuid().ToString("N");
            mdl.AccountId = SystemFlag.Default.Account;//todo create SystemFlag.Test.Account
            mdl.Name = Guid.NewGuid().ToString("N");
            mdl.DateCreated = DateTime.UtcNow;
            Assert.IsTrue(context.Insert<MODELNAME>(mdl));

            Task.Run(async () =>
            {
                ServiceResult res = await TestHelper.SentHttpRequest("GET", "api/MODELNAME/" + mdl.Name, "", _ownerAuthToken);

                Assert.IsNotNull(res);
                Assert.AreEqual(res.Code, 200);

                MODELNAME p = JsonConvert.DeserializeObject<MODELNAME>(res.Record.ToString());
                Assert.IsNotNull(p);
                Assert.AreEqual(mdl.Name, p.Name);

            }).GetAwaiter().GetResult();
        }

        [TestMethod]
        public void Api_MODELNAMEController_GetMODELNAMEs()
        {
            TreeMonDbContext context = new TreeMonDbContext(connectionKey);
            MODELNAME mdl = new MODELNAME();
            mdl.AccountId = SystemFlag.Default.Account;//todo create SystemFlag.Test.Account
            mdl.Name = Guid.NewGuid().ToString("N");
            mdl.DateCreated = DateTime.UtcNow;
            Assert.IsTrue(context.Insert<MODELNAME>(mdl));

            MODELNAME mdl2 = new MODELNAME();
            mdl2.AccountId = SystemFlag.Default.Account;//todo create SystemFlag.Test.Account
            mdl2.Name = Guid.NewGuid().ToString("N");
            mdl2.DateCreated = DateTime.UtcNow;
            Assert.IsTrue(context.Insert<MODELNAME>(mdl2));

            Task.Run(async () =>
            {
                ServiceResult res = await TestHelper.SentHttpRequest("POST", "api/MODELNAMEs/", "", _ownerAuthToken);

                Assert.IsNotNull(res);
                Assert.AreEqual(res.Code, 200);

                List<MODELNAME> MODELNAMEs = JsonConvert.DeserializeObject<List<MODELNAME>>(res.Records.ToString());
                Assert.IsNotNull(MODELNAMEs);
                Assert.IsTrue(MODELNAMEs.Count >= 2);

                int foundMODELNAMEs = 0;
                foreach (MODELNAME p in MODELNAMEs)
                {
                    if (p.Name == mdl.Name || p.Name == mdl2.Name)
                        foundMODELNAMEs++;

                }

                Assert.AreEqual(foundMODELNAMEs, 2);

            }).GetAwaiter().GetResult();
        }

        [TestMethod]
        public void Api_MODELNAMEController_DeleteMODELNAME()
        {
            TreeMonDbContext context = new TreeMonDbContext(connectionKey);
            MODELNAME mdl = new MODELNAME();
            mdl.AccountId = SystemFlag.Default.Account;//todo create SystemFlag.Test.Account
            mdl.Name = Guid.NewGuid().ToString("N");
            mdl.UUID = Guid.NewGuid().ToString("N");
            Assert.IsTrue(context.Insert<MODELNAME>(mdl));
            string postData =  JsonConvert.SerializeObject(mdl);

            Task.Run(async () =>
            {
                ServiceResult res = await TestHelper.SentHttpRequest("POST", "api/MODELNAMEs/Delete", postData, _ownerAuthToken);

                Assert.IsNotNull(res);
                Assert.AreEqual(res.Code, 200);

                MODELNAME dbMODELNAME = context.GetAll<MODELNAME>().Where(w => w.Name == mdl.Name).FirstOrDefault();
                Assert.IsNotNull(dbMODELNAME);
                Assert.IsTrue(dbMODELNAME.Deleted);
                //Assert.IsNull(dbMODELNAME);

            }).GetAwaiter().GetResult();
        }

        [TestMethod]
        public void Api_MODELNAMEController_UpdateMODELNAME()
        {
            TreeMonDbContext context = new TreeMonDbContext(connectionKey);
            MODELNAME mdl = new MODELNAME();
            mdl.AccountId = SystemFlag.Default.Account;//todo create SystemFlag.Test.Account
            mdl.Name = Guid.NewGuid().ToString("N");
            mdl.UUID = Guid.NewGuid().ToString("N");

            Assert.IsTrue(context.Insert<MODELNAME>(mdl));
            
            mdl = context.GetAll<MODELNAME>().Where(w => w.Name == mdl.Name).FirstOrDefault();
            MODELNAMEView pv = new MODELNAMEView();
            pv.Id = mdl.Id;
            pv.UUID = mdl.UUID;
            pv.AccountId = mdl.AccountId;
            pv.Name = mdl.Name;
              //~~~ Updatable fields ~~~
           
            string postData = JsonConvert.SerializeObject(pv);
            Task.Run(async () =>
            {
                ServiceResult res = await TestHelper.SentHttpRequest("POST", "api/MODELNAMEs/Update", postData, _ownerAuthToken);
                Assert.IsNotNull(res);
                Assert.AreEqual(res.Code, 200);

                MODELNAME dbMODELNAME = context.GetAll<MODELNAME>().Where(w => w.Name == mdl.Name).FirstOrDefault();
                Assert.IsNotNull(dbMODELNAME);
     
            }).GetAwaiter().GetResult();

        }

        //[TestMethod]
        //public void Api_MODELNAMEController_Get_MODELNAMEs_ByCategory()
        //{
        //    TreeMonDbContext context = new TreeMonDbContext(connectionKey);
        //    MODELNAME mdl = new MODELNAME();
        //    mdl.AccountId = SystemFlag.Default.Account;//todo create SystemFlag.Test.Account
        //    mdl.Name = Guid.NewGuid().ToString("N");
        // mdl.DateCreated = DateTime.UtcNow;
        //    mdl.UUID = Guid.NewGuid().ToString("N");
        //    mdl.Category = "BOOTH";
        //    Assert.IsTrue(context.Insert<MODELNAME>(mdl));

        //    MODELNAME mdl2 = new MODELNAME();
        //    mdl2.AccountId = SystemFlag.Default.Account;//todo create SystemFlag.Test.Account
        //    mdl2.Name = Guid.NewGuid().ToString("N");
        // mdl2.DateCreated = DateTime.UtcNow;
        //    mdl2.UUID = Guid.NewGuid().ToString("N");
        //    mdl2.Category = "BOOTH";
        //    Assert.IsTrue(context.Insert<MODELNAME>(mdl2));

        //    Task.Run(async () =>
        //    {
        //        ServiceResult res = await TestHelper.SentHttpRequest("POST", "api/MODELNAMEs/Categories/" + mdl.Category, "", _ownerAuthToken);

        //        Assert.IsNotNull(res);
        //        Assert.AreEqual(res.Code, 200);

        //        List<MODELNAME> MODELNAMEs = JsonConvert.DeserializeObject<List<MODELNAME>>(res.Records.ToString());
        //        Assert.IsNotNull(MODELNAMEs);
        //        Assert.IsTrue(MODELNAMEs.Count >= 2);

        //        int foundMODELNAMEs = 0;
        //        foreach (MODELNAME p in MODELNAMEs)
        //        {
        //            if (p.Name == mdl.Name || p.Name == mdl2.Name)
        //                foundMODELNAMEs++;
        //        }

        //        Assert.AreEqual(foundMODELNAMEs, 2);

        //    }).GetAwaiter().GetResult();
        //}

    }
}
