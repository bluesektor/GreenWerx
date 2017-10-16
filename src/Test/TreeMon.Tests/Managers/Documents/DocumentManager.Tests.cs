using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TreeMon.Managers.Documents;
using TreeMon.Models.App;

namespace TreeMon.Web.Tests.Managers
{
    [TestClass]
    public class DocumentManagerTests
    {
        [TestMethod]
        public void TestMethod1()
        {

            //  <add key="TemplateEmailNewMember" value="App_Data\Templates\Site\EmailNewMember.html" />
            DocumentManager dm = new DocumentManager();
            ServiceResult res = dm.GetTemplate("TemplateEmailNewMember");

            Assert.IsTrue(res.Code == 200);
            Assert.IsTrue(!string.IsNullOrWhiteSpace(res.Result));
            Assert.IsTrue(res.Result.Contains("[DOMAIN]"));
        }
    }
}
