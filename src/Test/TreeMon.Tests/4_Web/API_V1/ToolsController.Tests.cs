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
using TreeMon.Models.Store;
using TreeMon.Utilites.Security;
using TreeMon.Web.Tests;

namespace TreeMon.Web.Tests.API.V1
{

    [TestClass]
    public class ToolsControllerTests
    {
        private string connectionKey = "MSSQL_TEST";
        private string _ownerAuthToken = "";
        private string _captcha = "TESTCAPTCHA";

        [TestInitialize]
        public void TestSetup()
        {
            ServiceResult res = TestHelper.InitializeControllerTestData(connectionKey,  "OWNER");
            Assert.AreNotEqual(res.Code, 500, res.Message);//should not be error. if it is assert.
            _ownerAuthToken = res.Result.ToString();
            Assert.IsNotNull(ConfigurationManager.AppSettings["DefaultDbConnection"]);
        }

 
        [TestMethod]
        public void Api_ToolController_Cipher_Encrypt()
        {
            string forconfig = Cipher.Crypt(ConfigurationManager.AppSettings["AppKey"].ToString(), "yourpassword", true);

            Assert.IsNotNull(ConfigurationManager.AppSettings["AppKey"]);

            

            Task.Run(async () =>
            {
                ServiceResult res = await TestHelper.SentHttpRequest("GET", "api/Tools/Cipher/alpha/Encrypt/true", "", _ownerAuthToken);

                Assert.IsNotNull(res);
                Assert.AreEqual(res.Code, 200);
                string encText = Cipher.Crypt(ConfigurationManager.AppSettings["AppKey"].ToString(), "alpha", true);
                Assert.AreEqual(res.Result, encText);
             

            }).GetAwaiter().GetResult();
        }

        [TestMethod]
        public void Api_ToolController_Cipher_Decrypt()
        {
            Assert.IsNotNull(ConfigurationManager.AppSettings["AppKey"]);
            string encText =  Cipher.Crypt(ConfigurationManager.AppSettings["AppKey"].ToString(), "alpha", true);
            //carefull what text you encode because it can convert when sent 
            string args = "?text="+ encText +"&encrypt=false";

            Task.Run(async () =>
            {
                ServiceResult res = await TestHelper.SentHttpRequest("GET", "api/Tools/Cipher/" + encText + "/Encrypt/false", "", _ownerAuthToken);

                Assert.IsNotNull(res);
                Assert.AreEqual(res.Code, 200);
                Assert.AreEqual(res.Result, "alpha");
            }).GetAwaiter().GetResult();
        }


    }
}
