// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using TreeMon.Data;
using TreeMon.Data.Logging.Models;
using TreeMon.Managers;
using TreeMon.Models.App;
using TreeMon.Models.Medical;
using TreeMon.Models.Membership;

namespace TreeMon.Web.Tests.API.V1
{

    [TestClass]
    public class PollsControllerTests
    {
       private string connectionKey = "MSSQL_TEST";
        private string _ownerAuthToken = "";
    //    private string _captcha = "TESTCAPTCHA";

        [TestInitialize]
        public void TestSetup()
        {
            ServiceResult res = TestHelper.InitializeControllerTestData(connectionKey,"OWNER");
            Assert.AreNotEqual(res.Code, 500, res.Message);//should not be error. if it is assert.
            _ownerAuthToken = res.Result.ToString();
            Assert.IsNotNull(ConfigurationManager.AppSettings["DefaultDbConnection"]);
        }

        [TestMethod]
        public void Api_PollController_SaveRating_SYMPTOMLOG()
        {
            TreeMonDbContext context = new TreeMonDbContext(connectionKey);
            //User user = TestHelper.GenerateTestUser(Guid.NewGuid().ToString("N"));
            //string userJson = JsonConvert.SerializeObject(user);

             SessionManager sessionManager = new SessionManager(connectionKey);
            // UserSession us = sessionManager.SaveSession("127.1.1.40", user.UUID, userJson, false);
            UserSession us = sessionManager.GetSession(_ownerAuthToken);
            Assert.IsNotNull(us);
            User user = JsonConvert.DeserializeObject<User>(us.UserData);
            SymptomLog mdl = new SymptomLog();
            mdl.AccountUUID = SystemFlag.Default.Account;
            mdl.CreatedBy = user.UUID;
            mdl.Name = Guid.NewGuid().ToString("N");
            mdl.UUID = Guid.NewGuid().ToString("N");
            mdl.DateCreated = DateTime.Now;
            mdl.SymptomDate = DateTime.Now;
            
            Assert.IsTrue(context.Insert<SymptomLog>(mdl));

            int score = new Random().Next(1, 100000);//this is big for the test to reduce chance of collission when checking the result.

            Task.Run(async () =>
            {
                ServiceResult res = await TestHelper.SentHttpRequest("POST", "api/Ratings/SYMPTOMLOG/" + mdl.UUID +"/Save/" + score.ToString() , "", _ownerAuthToken);
                Assert.IsNotNull(res);
                Assert.AreEqual(res.Code, 200);

                SymptomLog dblog = context.GetAll<SymptomLog>().FirstOrDefault(w => w.UUID == mdl.UUID);
                Assert.IsNotNull(dblog);
                Assert.AreEqual(mdl.Name, dblog.Name);
                Assert.AreEqual(score, dblog.Efficacy);


            }).GetAwaiter().GetResult();

        }
     

    }
}
