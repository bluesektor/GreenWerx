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
using TreeMon.Models.Plant;
using TreeMon.Models.Store;
using TreeMon.Web.Tests;

namespace TreeMon.Web.Tests.API.V1
{

    [TestClass]
    public class StrainsControllerTests
    {
        //private string connectionKey = "MSSQL_TEST";
        private string _ownerAuthToken = "";
        private string _captcha = "TESTCAPTCHA";

        //[TestInitialize]
        //public void TestSetup()
        //{
        //    _ownerAuthToken = TestHelper.InitializeControllerTestData(connectionKey, "OWNER");
        //    Assert.IsNotNull(ConfigurationManager.AppSettings["DefaultDbConnection"]);
        //}

        //[TestMethod]
        //public void Api_StrainController_AddStrain()
        //{
        //    StrainForm mdl = new StrainForm();
        //    mdl.AccountUUID = SystemFlag.Default.Account;
        //    mdl.Name = Guid.NewGuid().ToString("N");
        //    mdl.UUID = Guid.NewGuid().ToString("N");
        //    mdl.Captcha = _captcha;

        //    string postData = JsonConvert.SerializeObject(mdl);

        //    Task.Run(async () =>
        //    {
        //        ServiceResult res = await TestHelper.SentHttpRequest("POST", "api/Strains/Add", postData, _ownerAuthToken);
        //        Assert.IsNotNull(res);
        //        Assert.AreEqual(res.Code, 200);

        //        Strain p = JsonConvert.DeserializeObject<Strain>(res.Result.ToString());
        //        Assert.IsNotNull(p);
        //        TreeMonDbContext context = new TreeMonDbContext(connectionKey);
        //        Strain dbStrain = context.GetAll<Strain>().Where(w => w.UUID == p.UUID).FirstOrDefault();
        //        Assert.IsNotNull(dbStrain);
        //        Assert.AreEqual(mdl.Name, dbStrain.Name);

        //    }).GetAwaiter().GetResult();

        //}

        //[TestMethod]
        //public void Api_StrainController_GetStrain()
        //{
        //    TreeMonDbContext context = new TreeMonDbContext(connectionKey);
        //    Strain mdl = new Strain();
        //    mdl.UUID = Guid.NewGuid().ToString("N");
        //    mdl.AccountUUID = SystemFlag.Default.Account;
        //    mdl.Name = Guid.NewGuid().ToString("N");
        //    mdl.DateCreated = DateTime.UtcNow;
        //    Assert.IsTrue(context.Insert<Strain>(mdl));

        //    Task.Run(async () =>
        //    {
        //        ServiceResult res = await TestHelper.SentHttpRequest("GET", "api/Strain/" + mdl.Name, "", _ownerAuthToken);

        //        Assert.IsNotNull(res);
        //        Assert.AreEqual(res.Code, 200);

        //        Strain p = JsonConvert.DeserializeObject<Strain>(res.Result.ToString());
        //        Assert.IsNotNull(p);
        //        Assert.AreEqual(mdl.Name, p.Name);

        //    }).GetAwaiter().GetResult();
        //}

        //[TestMethod]
        //public void Api_StrainController_GetStrains()
        //{
        //    TreeMonDbContext context = new TreeMonDbContext(connectionKey);
        //    Strain mdl = new Strain();
        //    mdl.AccountUUID = SystemFlag.Default.Account;
        //    mdl.Name = Guid.NewGuid().ToString("N");
        //    mdl.DateCreated = DateTime.UtcNow;
        //    Assert.IsTrue(context.Insert<Strain>(mdl));

        //    Strain mdl2 = new Strain();
        //    mdl2.AccountUUID = SystemFlag.Default.Account;
        //    mdl2.Name = Guid.NewGuid().ToString("N");
        //    mdl2.DateCreated = DateTime.UtcNow;
        //    Assert.IsTrue(context.Insert<Strain>(mdl2));

        //    Task.Run(async () =>
        //    {
        //        ServiceResult res = await TestHelper.SentHttpRequest("POST", "api/Strains/", "", _ownerAuthToken);

        //        Assert.IsNotNull(res);
        //        Assert.AreEqual(res.Code, 200);

        //        List<Strain> Strains = JsonConvert.DeserializeObject<List<Strain>>(res.Result.ToString());
        //        Assert.IsNotNull(Strains);
        //        Assert.IsTrue(Strains.Count >= 2);

        //        int foundStrains = 0;
        //        foreach (Strain p in Strains)
        //        {
        //            if (p.Name == mdl.Name || p.Name == mdl2.Name)
        //                foundStrains++;

        //        }

        //        Assert.AreEqual(foundStrains, 2);

        //    }).GetAwaiter().GetResult();
        //}

        //[TestMethod]
        //public void Api_StrainController_DeleteStrain()
        //{
        //    TreeMonDbContext context = new TreeMonDbContext(connectionKey);
        //    Strain mdl = new Strain();
        //    mdl.AccountUUID = SystemFlag.Default.Account;
        //    mdl.Name = Guid.NewGuid().ToString("N");
        //    mdl.UUID = Guid.NewGuid().ToString("N");
        //    Assert.IsTrue(context.Insert<Strain>(mdl));
        //    string postData =  JsonConvert.SerializeObject(mdl);

        //    Task.Run(async () =>
        //    {
        //        ServiceResult res = await TestHelper.SentHttpRequest("POST", "api/Strains/Delete", postData, _ownerAuthToken);

        //        Assert.IsNotNull(res);
        //        Assert.AreEqual(res.Code, 200);

        //        Strain dbStrain = context.GetAll<Strain>().Where(w => w.Name == mdl.Name).FirstOrDefault();
        //        Assert.IsNotNull(dbStrain);
        //        Assert.IsTrue(dbStrain.Deleted);
        //        //Assert.IsNull(dbStrain);

        //    }).GetAwaiter().GetResult();
        //}

        //[TestMethod]
        //public void Api_StrainController_UpdateStrain()
        //{
        //    TreeMonDbContext context = new TreeMonDbContext(connectionKey);
        //    Strain mdl = new Strain();
        //    mdl.AccountUUID = SystemFlag.Default.Account;
        //    mdl.Name = Guid.NewGuid().ToString("N");
        //    mdl.UUID = Guid.NewGuid().ToString("N");

        //    Assert.IsTrue(context.Insert<Strain>(mdl));
            
        //    mdl = context.GetAll<Strain>().Where(w => w.Name == mdl.Name).FirstOrDefault();
        //    StrainForm pv = new StrainForm();
        //    pv.Id = mdl.Id;
        //    pv.UUID = mdl.UUID;
        //    pv.AccountUUID = mdl.AccountUUID;
        //    pv.Name = mdl.Name;
        //      //~~~ Updatable fields ~~~
           
        //    string postData = JsonConvert.SerializeObject(pv);
        //    Task.Run(async () =>
        //    {
        //        ServiceResult res = await TestHelper.SentHttpRequest("POST", "api/Strains/Update", postData, _ownerAuthToken);
        //        Assert.IsNotNull(res);
        //        Assert.AreEqual(res.Code, 200);

        //        Strain dbStrain = context.GetAll<Strain>().Where(w => w.Name == mdl.Name).FirstOrDefault();
        //        Assert.IsNotNull(dbStrain);
     
        //    }).GetAwaiter().GetResult();

        //}

        //[TestMethod]
        //public void Api_StrainController_SearchStrains()
        //{
        //    //NOTE: The last ReturnFormat listed is the one chosen to be returned.

        //    string filters = "[{ \"SearchTerm\" :\"2\" , \"SearchBy\": \"HarvestTime\", \"ReturnFormat\":\"\" }]";

        //    TreeMonDbContext context = new TreeMonDbContext(connectionKey);
        //    Strain mdl = new Strain();
        //    mdl.AccountUUID = SystemFlag.Default.Account;
        //    mdl.Name = Guid.NewGuid().ToString("N");
        //    mdl.HarvestTime = 2;
        //    mdl.AccountUUID = SystemFlag.Default.Account;
        //    Assert.IsTrue(context.Insert<Strain>(mdl));

        //    Strain mdl2 = new Strain();
        //    mdl2.AccountUUID = SystemFlag.Default.Account;
        //    mdl2.Name = Guid.NewGuid().ToString("N");
        //    mdl2.HarvestTime =2;
        //    mdl2.AccountUUID = SystemFlag.Default.Account;
        //    Assert.IsTrue(context.Insert<Strain>(mdl2));

        //    Task.Run(async () =>
        //    {
        //        ServiceResult res = await TestHelper.SentHttpRequest("get", "api/Strains/?filters=" + filters, "", _ownerAuthToken);

        //        Assert.IsNotNull(res);
        //        Assert.AreEqual(res.Code, 200);
        //        string records = res.Result.ToString();
        //        List<Strain> selRes = JsonConvert.DeserializeObject<List<Strain>>(res.Result.ToString());
        //        Assert.IsNotNull(selRes);
        //        Assert.IsTrue(selRes.Count >= 2);
        //    }).GetAwaiter().GetResult();
        //}


    }
}
