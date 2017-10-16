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
using TreeMon.Models.App;
using TreeMon.Models.Datasets;
using TreeMon.Models.Membership;

namespace TreeMon.Web.Tests.API.V1
{

    [TestClass]
    public class AppsControllerTests
    {
        private string connectionKey = "MSSQL_TEST";
        private string _ownerAuthToken = "";
        //private string _captcha = "TESTCAPTCHA";

        ///backlog add these tests.
        ///AppInfo api/Apps/Info
        /// 
        ///Install  
 

        [TestInitialize]
        public void TestSetup()
        {
            Assert.IsNotNull(ConfigurationManager.AppSettings["SiteDomain"]);
            Assert.IsNotNull(ConfigurationManager.AppSettings["SiteEmail"]);
            Assert.IsNotNull(ConfigurationManager.AppSettings["AppKey"]);
            Assert.IsNotNull(ConfigurationManager.AppSettings["MailHost"]);
            Assert.IsNotNull(ConfigurationManager.AppSettings["MailPort"]);
            Assert.IsNotNull(ConfigurationManager.AppSettings["EmailHostUser"]);
            Assert.IsNotNull(ConfigurationManager.AppSettings["EmailHostPassword"]);
            Assert.IsNotNull(ConfigurationManager.AppSettings["UseSSL"]);
            Assert.IsNotNull(ConfigurationManager.AppSettings["TemplatePasswordResetEmail"]);
            ServiceResult res = TestHelper.InitializeControllerTestData(connectionKey, "OWNER");
            Assert.AreNotEqual(res.Code, 500, res.Message);//should not be error. if it is assert.
            _ownerAuthToken = res.Result.ToString();
            Assert.IsNotNull(ConfigurationManager.AppSettings["DefaultDbConnection"]);
        }

        [TestMethod]
        public void Api_AppsController_AddSetting()
        {
            Setting mdl = new Setting();
            mdl.AccountUUID = SystemFlag.Default.Account;
            mdl.Name = Guid.NewGuid().ToString("N");
            mdl.UUID = Guid.NewGuid().ToString("N");
            mdl.Type = "string";
            mdl.Value = "testSetting";
            mdl.DateCreated = DateTime.Now;

            string postData = JsonConvert.SerializeObject(mdl);

            Task.Run(async () =>
            {
                ServiceResult res = await TestHelper.SentHttpRequest("POST", "api/Apps/Settings/Add", postData, _ownerAuthToken);
                Assert.IsNotNull(res);
                Assert.AreEqual(res.Code, 200);

                Setting p = JsonConvert.DeserializeObject<Setting>(res.Result.ToString());
                Assert.IsNotNull(p);
                TreeMonDbContext context = new TreeMonDbContext(connectionKey);
                Setting dbApps = context.GetAll<Setting>().Where(w => w.UUID == p.UUID).FirstOrDefault();
                Assert.IsNotNull(dbApps);
                Assert.AreEqual(mdl.Name, dbApps.Name);

            }).GetAwaiter().GetResult();

        }

        [TestMethod]
        public void Api_AppsController_GetSetting()
        {
            TreeMonDbContext context = new TreeMonDbContext(connectionKey);
            Setting mdl = new Setting();
            mdl.UUID = Guid.NewGuid().ToString("N");
            mdl.AccountUUID = SystemFlag.Default.Account;
            mdl.Name = Guid.NewGuid().ToString("N");
            mdl.DateCreated = DateTime.Now;
            Assert.IsTrue(context.Insert<Setting>(mdl));

            Task.Run(async () =>
            {
                ServiceResult res = await TestHelper.SentHttpRequest("GET", "api/Apps/SettingsBy/" + mdl.UUID, "", _ownerAuthToken);
                Assert.IsNotNull(res);
                Assert.AreEqual(res.Code, 200);

                Setting p = JsonConvert.DeserializeObject<Setting>(res.Result.ToString());
                Assert.IsNotNull(p);
                Assert.AreEqual(mdl.Name, p.Name);

            }).GetAwaiter().GetResult();
        }

        [TestMethod]
        public void Api_AppsController_GetSettings()
        {
            TreeMonDbContext context = new TreeMonDbContext(connectionKey);
            Setting mdl = new Setting();
            mdl.AccountUUID = SystemFlag.Default.Account;
            mdl.Name = Guid.NewGuid().ToString("N");
            mdl.AppType = "web";
            mdl.SettingClass = "app";
            mdl.DateCreated = DateTime.Now;
       

            Assert.IsTrue(context.Insert<Setting>(mdl));

            Setting mdl2 = new Setting();
            mdl2.AccountUUID = SystemFlag.Default.Account;
            mdl2.Name = Guid.NewGuid().ToString("N");
            mdl2.AppType = "web";
            mdl2.SettingClass = "app";
            mdl2.DateCreated = DateTime.Now;
            Assert.IsTrue(context.Insert<Setting>(mdl2));

            Task.Run(async () =>
            {
                DataFilter filter = new DataFilter();
                filter.PageResults = false;
                ServiceResult res = await TestHelper.SentHttpRequest("POST", "api/Apps/Settings?filter=" + JsonConvert.SerializeObject(filter), "", _ownerAuthToken);

                Assert.IsNotNull(res);
                Assert.AreEqual(res.Code, 200);

                List<Setting> Apps = JsonConvert.DeserializeObject<List<Setting>>(res.Result.ToString());
                Assert.IsNotNull(Apps);
                Assert.IsTrue(Apps.Count >= 2);

                int foundApps = 0;
                foreach (Setting p in Apps)
                {
                    if (p.Name == mdl.Name || p.Name == mdl2.Name)
                        foundApps++;

                }

                Assert.AreEqual(foundApps, 2);

            }).GetAwaiter().GetResult();
        }

        //todo this needs to be updated to get dashboards etc. i think the appinfo is gone.
        //[TestMethod]
        //public void Api_AppsController_AppInfo()
        //{
        //    TreeMonDbContext context = new TreeMonDbContext(connectionKey);

        //    Account account = TestHelper.GenerateTestAccount(Guid.NewGuid().ToString("N"));
        //    User user = TestHelper.GenerateTestUser(Guid.NewGuid().ToString("N"));
        //    user.AccountUUID = account.UUID;
        //    user.SiteAdmin = true;
        //    Assert.IsTrue(context.Insert<User>(user));

        //    context.Delete<AppInfo>("WHERE UUParentID = '" + account.UUID + "' AND UUParentIDType = 'account'", null);
        //    AppInfo appInfo = new AppInfo();
        //    appInfo.UUID            = Guid.NewGuid().ToString("N");
        //    appInfo.AccountEmail    = account.Email;
        //    appInfo.AccountName     = account.Name;
        //    appInfo.UserEmail       = user.Email;
        //    appInfo.UserName        = user.Name;
        //    appInfo.UserPassword    = user.Password;
        //    appInfo.RunInstaller    = false;
        //    appInfo.AppType = "web";
        //    appInfo.UUParentID = account.UUID;
        //    appInfo.UUParentIDType = "account";

        //    Assert.IsTrue(context.Insert<AppInfo>(appInfo));

        //   SessionManager sessionManager = new SessionManager(connectionKey);
        //    string userJson = JsonConvert.SerializeObject(user);

        //    UserSession us = sessionManager.SaveSession("127.1.1.32", user.UUID, userJson, false);


        //    Task.Run(async () =>
        //    {
        //        ServiceResult res = await TestHelper.SentHttpRequest("GET", "api/Apps/Info/", "", us.AuthToken);
        //        Assert.IsNotNull(res);
        //        Assert.AreEqual(res.Code, 200);

        //        AppInfo p = JsonConvert.DeserializeObject<AppInfo>(res.Result.ToString());
        //        Assert.IsNotNull(p);
        //        //foreach (Setting s in p.Settings)
        //        //{
        //        //    string tmp = s.AppType;
        //        //}

        //        Assert.AreEqual(appInfo.UUID, p.UUID);
        //        Assert.AreEqual(appInfo.AccountEmail  , p.AccountEmail );
        //        Assert.AreEqual(appInfo.AccountName   , p.AccountName  );
        //        Assert.AreEqual(appInfo.UserEmail     , p.UserEmail    );
        //        Assert.AreEqual(appInfo.UserName      , p.UserName     );
        //        Assert.AreEqual(appInfo.UserPassword  , p.UserPassword );
        //        Assert.AreEqual(appInfo.RunInstaller, p.RunInstaller);

        //    }).GetAwaiter().GetResult();
        //}


        [TestMethod]
        public void Api_AppsController_DeleteSetting()
        {
            TreeMonDbContext context = new TreeMonDbContext(connectionKey);
            Setting mdl = new Setting();
            mdl.AccountUUID = SystemFlag.Default.Account;
            mdl.Name = Guid.NewGuid().ToString("N");
            mdl.UUID = Guid.NewGuid().ToString("N");
            mdl.DateCreated = DateTime.Now;
            Assert.IsTrue(context.Insert<Setting>(mdl));
            string postData =  JsonConvert.SerializeObject(mdl);

            Task.Run(async () =>
            {
                ServiceResult res = await TestHelper.SentHttpRequest("POST", "api/Apps/Settings/Delete/" + mdl.UUID, postData, _ownerAuthToken);

                Assert.IsNotNull(res);
                Assert.AreEqual(res.Code, 200);

                Setting dbApps = context.GetAll<Setting>().Where(w => w.Name == mdl.Name).FirstOrDefault();
                Assert.IsNull(dbApps);

            }).GetAwaiter().GetResult();
        }

        [TestMethod]
        public void Api_AppsController_UpdateSetting()
        {
            TreeMonDbContext context = new TreeMonDbContext(connectionKey);
            Setting mdl = new Setting();
            mdl.AccountUUID = SystemFlag.Default.Account;
            mdl.Name = Guid.NewGuid().ToString("N");
            mdl.UUID = Guid.NewGuid().ToString("N");
            mdl.DateCreated = DateTime.Now;
            Assert.IsTrue(context.Insert<Setting>(mdl));
            
            mdl = context.GetAll<Setting>().Where(w => w.Name == mdl.Name).FirstOrDefault();
            Setting pv = new Setting();
            pv.UUID = mdl.UUID;
            pv.AccountUUID = mdl.AccountUUID;
            pv.Name = mdl.Name;
     
            //~~~ Updatable fields ~~~
           
            string postData = JsonConvert.SerializeObject(pv);
            Task.Run(async () =>
            {
                ServiceResult res = await TestHelper.SentHttpRequest("POST", "api/Apps/Settings/Update", postData, _ownerAuthToken);
                Assert.IsNotNull(res);
                Assert.AreEqual(res.Code, 200);

                Setting dbApps = context.GetAll<Setting>().Where(w => w.Name == mdl.Name).FirstOrDefault();
                Assert.IsNotNull(dbApps);
           
            }).GetAwaiter().GetResult();

        }
    }
}
