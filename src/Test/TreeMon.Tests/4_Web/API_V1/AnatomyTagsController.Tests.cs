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
using TreeMon.Models.Medical;

namespace TreeMon.Web.Tests.API.V1
{

    [TestClass]
    public class AnatomyTagControllerTests//todo re-implement
    {
        //private string connectionKey = "MSSQL_TEST";
        //private string _ownerAuthToken = "";
        ////private string _captcha = "TESTCAPTCHA";

        //[TestInitialize]
        //public void TestSetup()
        //{
        //    _ownerAuthToken = TestHelper.InitializeControllerTestData(connectionKey, "OWNER");
        //    Assert.IsNotNull(ConfigurationManager.AppSettings["DefaultDbConnection"]);
        //}

        //[TestMethod]
        //public void Api_AnatomyTagController_AddAnatomyTag()
        //{
        //    AnatomyTag mdl = new AnatomyTag();
        //    mdl.AccountUUID = SystemFlag.Default.Account;
        //    mdl.Name = Guid.NewGuid().ToString("N");
        //    mdl.UUID = Guid.NewGuid().ToString("N");

        //    string postData = JsonConvert.SerializeObject(mdl);

        //    Task.Run(async () =>
        //    {
        //        ServiceResult res = await TestHelper.SentHttpRequest("POST", "api/AnatomyTags/Add", postData, _ownerAuthToken);
        //        Assert.IsNotNull(res);
        //        Assert.AreEqual(res.Code, 200);

        //        AnatomyTag p = JsonConvert.DeserializeObject<AnatomyTag>(res.Result.ToString());
        //        Assert.IsNotNull(p);
        //        TreeMonDbContext context = new TreeMonDbContext(connectionKey);
        //        AnatomyTag dbAnatomyTag = context.GetAll<AnatomyTag>().Where(w => w.UUID == p.UUID).FirstOrDefault();
        //        Assert.IsNotNull(dbAnatomyTag);
        //        Assert.AreEqual(mdl.Name, dbAnatomyTag.Name);

        //    }).GetAwaiter().GetResult();

        //}

        //[TestMethod]
        //public void Api_AnatomyTagController_GetAnatomyTag()
        //{
        //    TreeMonDbContext context = new TreeMonDbContext(connectionKey);
        //    AnatomyTag mdl = new AnatomyTag();
        //    mdl.UUID = Guid.NewGuid().ToString("N");
        //    mdl.AccountUUID = SystemFlag.Default.Account;
        //    mdl.Name = Guid.NewGuid().ToString("N");
        //    Assert.IsTrue(context.Insert<AnatomyTag>(mdl));

        //    Task.Run(async () =>
        //    {
        //        ServiceResult res = await TestHelper.SentHttpRequest("GET", "api/AnatomyTag/" + mdl.Name, "", _ownerAuthToken);

        //        Assert.IsNotNull(res);
        //        Assert.AreEqual(res.Code, 200);

        //        AnatomyTag p = JsonConvert.DeserializeObject<AnatomyTag>(res.Result.ToString());
        //        Assert.IsNotNull(p);
        //        Assert.AreEqual(mdl.Name, p.Name);

        //    }).GetAwaiter().GetResult();
        //}

        //[TestMethod]
        //public void Api_AnatomyTagController_GetAnatomyTags()
        //{
        //    TreeMonDbContext context = new TreeMonDbContext(connectionKey);
        //    AnatomyTag mdl = new AnatomyTag();
        //    mdl.AccountUUID = SystemFlag.Default.Account;
        //    mdl.Name = Guid.NewGuid().ToString("N");
        //    Assert.IsTrue(context.Insert<AnatomyTag>(mdl));

        //    AnatomyTag mdl2 = new AnatomyTag();
        //    mdl2.AccountUUID = SystemFlag.Default.Account;
        //    mdl2.Name = Guid.NewGuid().ToString("N");
        //    Assert.IsTrue(context.Insert<AnatomyTag>(mdl2));

        //    Task.Run(async () =>
        //    {
        //        ServiceResult res = await TestHelper.SentHttpRequest("POST", "api/AnatomyTags/", "", _ownerAuthToken);

        //        Assert.IsNotNull(res);
        //        Assert.AreEqual(res.Code, 200);

        //        List<AnatomyTag> AnatomyTags = JsonConvert.DeserializeObject<List<AnatomyTag>>(res.Result.ToString());
        //        Assert.IsNotNull(AnatomyTags);
        //        Assert.IsTrue(AnatomyTags.Count >= 2);

        //        int foundAnatomyTags = 0;
        //        foreach (AnatomyTag p in AnatomyTags)
        //        {
        //            if (p.Name == mdl.Name || p.Name == mdl2.Name)
        //                foundAnatomyTags++;

        //        }

        //        Assert.AreEqual(foundAnatomyTags, 2);

        //    }).GetAwaiter().GetResult();
        //}

    
        //[TestMethod]
        //public void Api_AnatomyTagController_DeleteAnatomyTag()
        //{
        //    TreeMonDbContext context = new TreeMonDbContext(connectionKey);
        //    AnatomyTag mdl = new AnatomyTag();
        //    mdl.AccountUUID = SystemFlag.Default.Account;
        //    mdl.Name = Guid.NewGuid().ToString("N");
        //    mdl.UUID = Guid.NewGuid().ToString("N");
        //    Assert.IsTrue(context.Insert<AnatomyTag>(mdl));
        //    string postData =  JsonConvert.SerializeObject(mdl);

        //    Task.Run(async () =>
        //    {
        //        ServiceResult res = await TestHelper.SentHttpRequest("POST", "api/AnatomyTags/Delete", postData, _ownerAuthToken);

        //        Assert.IsNotNull(res);
        //        Assert.AreEqual(res.Code, 200);

        //        AnatomyTag dbAnatomyTag = context.GetAll<AnatomyTag>().Where(w => w.Name == mdl.Name).FirstOrDefault();
        //        Assert.IsNotNull(dbAnatomyTag);
        //        Assert.IsTrue(dbAnatomyTag.Deleted);
        //        //Assert.IsNull(dbAnatomyTag);

        //    }).GetAwaiter().GetResult();
        //}

        //[TestMethod]
        //public void Api_AnatomyTagController_UpdateAnatomyTag()
        //{
        //    TreeMonDbContext context = new TreeMonDbContext(connectionKey);
        //    AnatomyTag mdl = new AnatomyTag();
        //    mdl.AccountUUID = SystemFlag.Default.Account;
        //    mdl.Name = Guid.NewGuid().ToString("N");
        //    mdl.UUID = Guid.NewGuid().ToString("N");
        //    Assert.IsTrue(context.Insert<AnatomyTag>(mdl));
            
        //    mdl = context.GetAll<AnatomyTag>().Where(w => w.Name == mdl.Name).FirstOrDefault();
        //    AnatomyTag pv = new AnatomyTag();
        //    pv.UUID = mdl.UUID;
        //    pv.AccountUUID = mdl.AccountUUID;
        //    pv.Name = mdl.Name;
        //      //~~~ Updatable fields ~~~
           
        //    string postData = JsonConvert.SerializeObject(pv);
        //    Task.Run(async () =>
        //    {
        //        ServiceResult res = await TestHelper.SentHttpRequest("POST", "api/AnatomyTags/Update", postData, _ownerAuthToken);
        //        Assert.IsNotNull(res);
        //        Assert.AreEqual(res.Code, 200);

        //        AnatomyTag dbAnatomyTag = context.GetAll<AnatomyTag>().Where(w => w.Name == mdl.Name).FirstOrDefault();
        //        Assert.IsNotNull(dbAnatomyTag);
     
        //    }).GetAwaiter().GetResult();

        //}

        ////[TestMethod]
        ////public void Api_AnatomyTagController_Get_AnatomyTags_ByCategory()
        ////{
        ////    TreeMonDbContext context = new TreeMonDbContext(connectionKey);
        ////    AnatomyTag mdl = new AnatomyTag();
        ////    mdl.AccountUUID = SystemFlag.Default.Account;
        ////    mdl.Name = Guid.NewGuid().ToString("N");
        ////    mdl.UUID = Guid.NewGuid().ToString("N");
        ////    mdl.Category = "BOOTH";
        ////    Assert.IsTrue(context.Insert<AnatomyTag>(mdl));

        ////    AnatomyTag mdl2 = new AnatomyTag();
        ////    mdl2.AccountUUID = SystemFlag.Default.Account;
        ////    mdl2.Name = Guid.NewGuid().ToString("N");
        ////    mdl.UUID = Guid.NewGuid().ToString("N");
        ////    mdl2.Category = "BOOTH";
        ////    Assert.IsTrue(context.Insert<AnatomyTag>(mdl2));

        ////    Task.Run(async () =>
        ////    {
        ////        ServiceResult res = await TestHelper.SentHttpRequest("POST", "api/AnatomyTags/Categories/" + mdl.Category, "", _ownerAuthToken);

        ////        Assert.IsNotNull(res);
        ////        Assert.AreEqual(res.Code, 200);

        ////        List<AnatomyTag> AnatomyTags = JsonConvert.DeserializeObject<List<AnatomyTag>>(res.Result.ToString());
        ////        Assert.IsNotNull(AnatomyTags);
        ////        Assert.IsTrue(AnatomyTags.Count >= 2);

        ////        int foundAnatomyTags = 0;
        ////        foreach (AnatomyTag p in AnatomyTags)
        ////        {
        ////            if (p.Name == mdl.Name || p.Name == mdl2.Name)
        ////                foundAnatomyTags++;
        ////        }

        ////        Assert.AreEqual(foundAnatomyTags, 2);

        ////    }).GetAwaiter().GetResult();
        ////}

    }
}
