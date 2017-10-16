// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
//using System.Web.Mvc;
using TreeMon.Data;
using TreeMon.Data.Logging.Models;
using TreeMon.Managers;
using TreeMon.Managers.Membership;
using TreeMon.Managers.Store;
using TreeMon.Models.App;
using TreeMon.Models.Membership;
using TreeMon.Models.Store;
//using TreeMon.Web.api.v1;
//using TreeMon.Web.Models;
using TreeMon.Web.Tests;

namespace TreeMon.Web.Tests.API.V1
{

    [TestClass]
    public class SiteControllerTests
    {
        private string connectionKey = "MSSQL_TEST";
        private string _ownerAuthToken = "";

        public void TestSetup()
        {
            ServiceResult res = TestHelper.InitializeControllerTestData(connectionKey,  "OWNER");
            Assert.AreNotEqual(res.Code, 500, res.Message);//should not be error. if it is assert.
            _ownerAuthToken = res.Result.ToString();
            Assert.IsNotNull(ConfigurationManager.AppSettings["DefaultDbConnection"]);
        }

        //This doesn't work because it blows up on the XCaptcha dll
        //[TestMethod]
        //public void Api_SiteController_GetCaptchaImage()
        //{

        //    //SiteController sc = new SiteController();
        //    //FileContentResult fc = sc.GetCaptchaImage();


        //    Task.Run(async () =>
        //    {
        //        ServiceResult res = await TestHelper.SentHttpRequest("GET", "api/Site/CaptchaImage", "", _ownerAuthToken);

        //        Assert.IsNotNull(res);
        //        Assert.AreEqual(res.Code, 200);

        //       // FileContentResult

        //        //MODELNAME p = JsonConvert.DeserializeObject<MODELNAME>(res.Result.ToString());
        //        //Assert.IsNotNull(p);
        //        //Assert.AreEqual(mdl.Name, p.Name);

        //    }).GetAwaiter().GetResult();
        //}


    }
}
