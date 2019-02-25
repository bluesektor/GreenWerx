// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TreeMon.Managers.Documents;
using TreeMon.Models.App;

namespace TreeMon.Web.Tests.Managers
{
    [TestClass]
    public class DocumentManagerTests
    {
        private string connectionKey = "MSSQL_TEST";
        private string sessionKey = "DOCTESTSESSION";
        [TestMethod]
        public void DocManager_GetEmailNewMemberTemplate()
        {

            //  <add key="TemplateEmailNewMember" value="App_Data\Templates\Site\EmailNewMember.html" />
            DocumentManager dm = new DocumentManager(connectionKey, sessionKey);
            ServiceResult res = dm.GetTemplate("TemplateEmailNewMember");

            Assert.IsTrue(res.Code == 200);
            Assert.IsNotNull(res.Result);
            Assert.IsTrue(res.Result.ToString().Contains("[DOMAIN]"));
        }

        [TestMethod]
        public void DocManager_Parse()
        {

            //  <add key="TemplateEmailNewMember" value="App_Data\Templates\Site\EmailNewMember.html" />
            DocumentManager dm = new DocumentManager(connectionKey, sessionKey);
            ServiceResult res = dm.GetTemplate("TemplateEmailNewMember");

            Assert.IsTrue(res.Code == 200);
            Assert.IsNotNull(res.Result);
            Assert.IsTrue(res.Result.ToString().Contains("[DOMAIN]"));
        }
    }
}
