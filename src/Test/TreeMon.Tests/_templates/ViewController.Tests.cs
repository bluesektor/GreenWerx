using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using TreeMon.Data;
using TreeMon.Data.Logging.Models;
using TreeMon.Managers;
using TreeMon.Managers.Membership;
using TreeMon.Managers.Store;
using TreeMon.Models.App;
using TreeMon.Models.Membership;
using TreeMon.Models.Store;
using TreeMon.Web.Controllers;
using TreeMon.Web.Models;
using TreeMon.Web.Tests;

namespace TreeMon.Web.Tests.API.V1
{

    [TestClass]
    public class MODELNAMEController_View_Tests
    {
        private string connectionKey = "MSSQL_TEST";


        [TestMethod]
        public void MODELNAMEController_View_Index()
        {
            MODELNAMEController controller = new MODELNAMEController(connectionKey);

            //var timer = Stopwatch.StartNew();
            ActionResult result = controller.Index() as ActionResult;
            //timer.Stop();
            //return timer.Elapsed;
            Assert.IsNotNull(result);
        }
    }
}
