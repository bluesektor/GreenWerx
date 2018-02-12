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
using TreeMon.Models.Event;
using TreeMon.Models.Membership;
using TreeMon.Models.Store;
using TreeMon.Web.Tests;

namespace TreeMon.Web.Tests.API.V1
{

    [TestClass]
    public class ReminderControllerTests
    {
        ////private string connectionKey = "MSSQL_TEST";
        //private string _ownerAuthToken = "";
        //private string _captcha = "TESTCAPTCHA";

        //[TestInitialize]
        //public void TestSetup()
        //{
        //    _ownerAuthToken = TestHelper.InitializeControllerTestData(connectionKey, "OWNER");
        //    Assert.IsNotNull(ConfigurationManager.AppSettings["DefaultDbConnection"]);
        //}

        //[TestMethod]
        //public void Api_ReminderController_AddReminder()
        //{
        //    ReminderForm mdl = new ReminderForm();
        //    mdl.AccountUUID = SystemFlag.Default.Account;
        //    mdl.Name = Guid.NewGuid().ToString("N");
        //    mdl.UUID = Guid.NewGuid().ToString("N");
        //    mdl.EventDateTime = DateTime.UtcNow;
        //    mdl.Frequency = "annual";
        //    mdl.RepeatCount = 1;
          
        //    mdl.Captcha = _captcha;

        //    string postData = JsonConvert.SerializeObject(mdl);

        //    Task.Run(async () =>
        //    {
        //        ServiceResult res = await TestHelper.SentHttpRequest("POST", "api/Reminders/Add", postData, _ownerAuthToken);
        //        Assert.IsNotNull(res);
        //        Assert.AreEqual(res.Code, 200);

        //        Reminder p = JsonConvert.DeserializeObject<Reminder>(res.Result.ToString());
        //        Assert.IsNotNull(p);
        //        TreeMonDbContext context = new TreeMonDbContext(connectionKey);
        //        Reminder dbReminder = context.GetAll<Reminder>().Where(w => w.UUID == p.UUID).FirstOrDefault();
        //        Assert.IsNotNull(dbReminder);
        //        Assert.AreEqual(mdl.Name, dbReminder.Name);

        //    }).GetAwaiter().GetResult();

        //}

        //[TestMethod]
        //public void Api_ReminderController_GetReminder()
        //{
        //    TreeMonDbContext context = new TreeMonDbContext(connectionKey);
        //    Reminder mdl = new Reminder();
        //    mdl.UUID = Guid.NewGuid().ToString("N");
        //    mdl.AccountUUID = SystemFlag.Default.Account;
        //    mdl.Name = Guid.NewGuid().ToString("N");
        //    Assert.IsTrue(context.Insert<Reminder>(mdl));

        //    Task.Run(async () =>
        //    {
        //        ServiceResult res = await TestHelper.SentHttpRequest("GET", "api/Reminder/" + mdl.Name, "", _ownerAuthToken);

        //        Assert.IsNotNull(res);
        //        Assert.AreEqual(res.Code, 200);

        //        Reminder p = JsonConvert.DeserializeObject<Reminder>(res.Result.ToString());
        //        Assert.IsNotNull(p);
        //        Assert.AreEqual(mdl.Name, p.Name);

        //    }).GetAwaiter().GetResult();
        //}

        //[TestMethod]
        //public void Api_ReminderController_GetReminders()
        //{
        //    TreeMonDbContext context = new TreeMonDbContext(connectionKey);
        //    Reminder mdl = new Reminder();
        //    mdl.AccountUUID = SystemFlag.Default.Account;
        //    mdl.Name = Guid.NewGuid().ToString("N");
        //    Assert.IsTrue(context.Insert<Reminder>(mdl));

        //    Reminder mdl2 = new Reminder();
        //    mdl2.AccountUUID = SystemFlag.Default.Account;
        //    mdl2.Name = Guid.NewGuid().ToString("N");
        //    Assert.IsTrue(context.Insert<Reminder>(mdl2));

        //    Task.Run(async () =>
        //    {
        //        ServiceResult res = await TestHelper.SentHttpRequest("POST", "api/Reminders/", "", _ownerAuthToken);

        //        Assert.IsNotNull(res);
        //        Assert.AreEqual(res.Code, 200);

        //        List<Reminder> Reminders = JsonConvert.DeserializeObject<List<Reminder>>(res.Result.ToString());
        //        Assert.IsNotNull(Reminders);
        //        Assert.IsTrue(Reminders.Count >= 2);

        //        int foundReminders = 0;
        //        foreach (Reminder p in Reminders)
        //        {
        //            if (p.Name == mdl.Name || p.Name == mdl2.Name)
        //                foundReminders++;

        //        }

        //        Assert.AreEqual(foundReminders, 2);

        //    }).GetAwaiter().GetResult();
        //}

        //[TestMethod]
        //public void Api_ReminderController_DeleteReminder()
        //{
        //    TreeMonDbContext context = new TreeMonDbContext(connectionKey);
        //    Reminder mdl = new Reminder();
        //    mdl.AccountUUID = SystemFlag.Default.Account;
        //    mdl.Name = Guid.NewGuid().ToString("N");
        //    mdl.UUID = Guid.NewGuid().ToString("N");
        //    Assert.IsTrue(context.Insert<Reminder>(mdl));
        //    string postData =  JsonConvert.SerializeObject(mdl);

        //    Task.Run(async () =>
        //    {
        //        ServiceResult res = await TestHelper.SentHttpRequest("POST", "api/Reminders/Delete", postData, _ownerAuthToken);

        //        Assert.IsNotNull(res);
        //        Assert.AreEqual(res.Code, 200);

        //        Reminder dbReminder = context.GetAll<Reminder>().Where(w => w.Name == mdl.Name).FirstOrDefault();
        //        Assert.IsNotNull(dbReminder);
        //        Assert.IsTrue(dbReminder.Deleted);
        //        //Assert.IsNull(dbReminder);

        //    }).GetAwaiter().GetResult();
        //}

        //[TestMethod]
        //public void Api_ReminderController_UpdateReminder()
        //{
        //    TreeMonDbContext context = new TreeMonDbContext(connectionKey);
        //    Reminder mdl = new Reminder();
        //    mdl.AccountUUID = SystemFlag.Default.Account;
        //    mdl.Name = Guid.NewGuid().ToString("N");
        //    mdl.UUID = Guid.NewGuid().ToString("N");
        //    Assert.IsTrue(context.Insert<Reminder>(mdl));
            
        //    mdl = context.GetAll<Reminder>().Where(w => w.Name == mdl.Name).FirstOrDefault();
        //    ReminderForm pv = new ReminderForm();
        //    pv.UUID = mdl.UUID;
        //    pv.AccountUUID = mdl.AccountUUID;
        //    pv.Name = mdl.Name;
        //      //~~~ Updatable fields ~~~
           
        //    string postData = JsonConvert.SerializeObject(pv);
        //    Task.Run(async () =>
        //    {
        //        ServiceResult res = await TestHelper.SentHttpRequest("POST", "api/Reminders/Update", postData, _ownerAuthToken);
        //        Assert.IsNotNull(res);
        //        Assert.AreEqual(res.Code, 200);

        //        Reminder dbReminder = context.GetAll<Reminder>().Where(w => w.Name == mdl.Name).FirstOrDefault();
        //        Assert.IsNotNull(dbReminder);
     
        //    }).GetAwaiter().GetResult();

        //}

    }
}
