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
using TreeMon.Models.Medical;
using TreeMon.Models.Membership;
using TreeMon.Models.Store;
using TreeMon.Web.Tests;

namespace TreeMon.Web.Tests.API.V1
{

    [TestClass]
    public class DoseControllerTests //todo re-implement
    {
        ////private string connectionKey = "MSSQL_TEST";
        //private string _ownerAuthToken = "";
        //private string _captcha = "TESTCAPTCHA";
        //private string _symptomUUID = Guid.NewGuid().ToString("N");
        //private string _productUUID = Guid.NewGuid().ToString("N");

        //[TestInitialize]
        //public void TestSetup()
        //{
        //    _ownerAuthToken = TestHelper.InitializeControllerTestData(connectionKey, "OWNER");
        //    Assert.IsNotNull(ConfigurationManager.AppSettings["DefaultDbConnection"]);

        //    UnitOfMeasure uom = new UnitOfMeasure()
        //    {
        //        Name = "Gram",
        //        AccountUUID = SystemFlag.Default.Account,
        //        Active = true,
        //        Deleted = false,
        //        Private = true,
        //         DateCreated = DateTime.UtcNow
        //    };
        //    TreeMonDbContext context = new TreeMonDbContext(connectionKey);
        //    Assert.IsTrue(context.Insert<UnitOfMeasure>(uom));

        //    Symptom symptom = new Symptom()
        //    {
        //         UUID = _symptomUUID,
        //        Name = Guid.NewGuid().ToString("N"),
        //        AccountUUID = SystemFlag.Default.Account,
        //        Active = true,
        //        CreatedBy = Guid.NewGuid().ToString("N"),
        //        DateCreated = DateTime.UtcNow,
        //        Deleted = false,
        //        UUIDType = "Symptom",
        //        Category = "General"
        //    };
        //    Assert.IsTrue(context.Insert<Symptom>(symptom));

        //    Product  product = new Product()
        //    {
        //        UUID = _productUUID,
        //        AccountUUID = "a",
        //        Name = "TESTRECORD",
        //        DateCreated = DateTime.UtcNow
        //    };
        //    Assert.IsTrue(context.Insert<Product>(product));


        //}

        //[TestMethod]
        //public void Api_DoseController_AddDoseLog()
        //{
        //    DoseLogForm mdl = new DoseLogForm();
        //    mdl.AccountUUID = SystemFlag.Default.Account;
        //    mdl.Name = Guid.NewGuid().ToString("N");
        //    mdl.UUID = Guid.NewGuid().ToString("N");
        //    mdl.DoseDateTime = DateTime.UtcNow;
        //    mdl.ProductUUID = _productUUID;
        //    mdl.Quantity = 1;
        //    mdl.UnitOfMeasure = "Gram";

        //    mdl.Symptoms.Add(new SymptomLog()
        //    {
        //        AccountUUID = SystemFlag.Default.Account,
        //        SymptomDate = DateTime.UtcNow,
        //        SymptomUUID = _symptomUUID
        //    });
 
        //    mdl.Captcha = _captcha;

        //    string postData = JsonConvert.SerializeObject(mdl);

        //    Task.Run(async () =>
        //    {
        //        ServiceResult res = await TestHelper.SentHttpRequest("POST", "api/DoseLog/Add", postData, _ownerAuthToken);
        //        Assert.IsNotNull(res);
        //        Assert.AreEqual(res.Code, 200);

        //        DoseLog p = JsonConvert.DeserializeObject<DoseLog>(res.Result.ToString());
        //        Assert.IsNotNull(p);
        //        TreeMonDbContext context = new TreeMonDbContext(connectionKey);
        //        DoseLog dbDose = context.GetAll<DoseLog>().Where(w => w.UUID == p.UUID).FirstOrDefault();
        //        Assert.IsNotNull(dbDose);
        //        Assert.AreEqual(mdl.Name, dbDose.Name);

        //    }).GetAwaiter().GetResult();

        //}

        //[TestMethod]
        //public void Api_DoseController_GetDoses()
        //{
        //    TreeMonDbContext context = new TreeMonDbContext(connectionKey);
        //    DoseLog mdl = new DoseLog();
        //    mdl.AccountUUID = SystemFlag.Default.Account;
        //    mdl.Name = Guid.NewGuid().ToString("N");
        //    mdl.DateCreated = DateTime.UtcNow;
        //    mdl.DoseDateTime = DateTime.UtcNow;
        //    Assert.IsTrue(context.Insert<DoseLog>(mdl));

        //    DoseLog mdl2 = new DoseLog();
        //    mdl2.AccountUUID = SystemFlag.Default.Account;
        //    mdl2.Name = Guid.NewGuid().ToString("N");
        //    mdl2.DateCreated = DateTime.UtcNow;
        //    mdl2.DoseDateTime = DateTime.UtcNow;
        //    Assert.IsTrue(context.Insert<DoseLog>(mdl2));

        //    Task.Run(async () =>
        //    {
        //        ServiceResult res = await TestHelper.SentHttpRequest("POST", "api/DoseLogs/", "", _ownerAuthToken);

        //        Assert.IsNotNull(res);
        //        Assert.AreEqual(res.Code, 200);

        //        List<DoseLog> Doses = JsonConvert.DeserializeObject<List<DoseLog>>(res.Result.ToString());
        //        Assert.IsNotNull(Doses);
        //        Assert.IsTrue(Doses.Count >= 2);

        //        int foundDoses = 0;
        //        foreach (DoseLog p in Doses)
        //        {
        //            if (p.Name == mdl.Name || p.Name == mdl2.Name)
        //                foundDoses++;

        //        }

        //        Assert.AreEqual(foundDoses, 2);

        //    }).GetAwaiter().GetResult();
        //}

        ////[TestMethod]
        ////public void Api_DoseController_GetDose()
        ////{
        ////    TreeMonDbContext context = new TreeMonDbContext(connectionKey);
        ////    DoseLog mdl = new DoseLog();
        ////    mdl.UUID = Guid.NewGuid().ToString("N");
        ////    mdl.AccountUUID = SystemFlag.Default.Account;
        ////    mdl.Name = Guid.NewGuid().ToString("N");
        ////    Assert.IsTrue(context.Insert<DoseLog>(mdl));

        ////    Task.Run(async () =>
        ////    {
        ////        ServiceResult res = await TestHelper.SentHttpRequest("GET", "api/Dose/" + mdl.Name, "", _ownerAuthToken);

        ////        Assert.IsNotNull(res);
        ////        Assert.AreEqual(res.Code, 200);

        ////        DoseLog p = JsonConvert.DeserializeObject<DoseLog>(res.Result.ToString());
        ////        Assert.IsNotNull(p);
        ////        Assert.AreEqual(mdl.Name, p.Name);

        ////    }).GetAwaiter().GetResult();
        ////}

        ////[TestMethod]
        ////public void Api_DoseController_DeleteDose()
        ////{
        ////    TreeMonDbContext context = new TreeMonDbContext(connectionKey);
        ////    DoseLog mdl = new DoseLog();
        ////    mdl.AccountUUID = SystemFlag.Default.Account;
        ////    mdl.Name = Guid.NewGuid().ToString("N");
        ////    mdl.UUID = Guid.NewGuid().ToString("N");
        ////    Assert.IsTrue(context.Insert<DoseLog>(mdl));
        ////    string postData =  JsonConvert.SerializeObject(mdl);

        ////    Task.Run(async () =>
        ////    {
        ////        ServiceResult res = await TestHelper.SentHttpRequest("POST", "api/Doses/Delete", postData, _ownerAuthToken);

        ////        Assert.IsNotNull(res);
        ////        Assert.AreEqual(res.Code, 200);

        ////        DoseLog dbDose = context.GetAll<DoseLog>().Where(w => w.Name == mdl.Name).FirstOrDefault();
        ////        Assert.IsNotNull(dbDose);
        ////        Assert.IsTrue(dbDose.Deleted);
        ////        //Assert.IsNull(dbDose);

        ////    }).GetAwaiter().GetResult();
        ////}

        ////[TestMethod]
        ////public void Api_DoseController_UpdateDose()
        ////{
        ////    TreeMonDbContext context = new TreeMonDbContext(connectionKey);
        ////    DoseLog mdl = new DoseLog();
        ////    mdl.AccountUUID = SystemFlag.Default.Account;
        ////    mdl.Name = Guid.NewGuid().ToString("N");
        ////    mdl.UUID = Guid.NewGuid().ToString("N");
        ////    Assert.IsTrue(context.Insert<DoseLog>(mdl));

        ////    mdl = context.GetAll<DoseLog>().Where(w => w.Name == mdl.Name).FirstOrDefault();
        ////    DoseLog pv = new DoseLog();
        ////    pv.UUID = mdl.UUID;
        ////    pv.AccountUUID = mdl.AccountUUID;
        ////    pv.Name = mdl.Name;
        ////      //~~~ Updatable fields ~~~

        ////    string postData = JsonConvert.SerializeObject(pv);
        ////    Task.Run(async () =>
        ////    {
        ////        ServiceResult res = await TestHelper.SentHttpRequest("POST", "api/Doses/Update", postData, _ownerAuthToken);
        ////        Assert.IsNotNull(res);
        ////        Assert.AreEqual(res.Code, 200);

        ////        DoseLog dbDose = context.GetAll<DoseLog>().Where(w => w.Name == mdl.Name).FirstOrDefault();
        ////        Assert.IsNotNull(dbDose);

        ////    }).GetAwaiter().GetResult();

        ////}


    }
}
