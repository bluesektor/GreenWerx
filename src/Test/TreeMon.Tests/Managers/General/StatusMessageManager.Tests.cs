using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TreeMon.Data;
using TreeMon.Data.Logging.Models;
using TreeMon.Managers.General;
using TreeMon.Models.App;
using TreeMon.Models.General;
using TreeMon.Utilites.Extensions;

namespace TreeMon.Web.Tests.Managers.General
{

    [TestClass]
    public class StatusMessageManagerTests
    {
        private string connectionKey = "MSSQL_TEST";

        [TestMethod]
        public void StatusMessageManager_Insert() {
            StatusMessageManager sm = new StatusMessageManager(new TreeMonDbContext(connectionKey));

            Assert.AreEqual(
                sm.Insert(new StatusMessage()
                {
                    AccountId = "a",
                    Name = "TEST_SM",
                    DateCreated = DateTime.UtcNow
                })
             .Code, 200);

            //won't allow a duplicate name
            Assert.AreEqual(
              sm.Insert(new StatusMessage()
              {
                  AccountId = "a",
                  Name = "TEST_SM",
                  DateCreated = DateTime.UtcNow
              })
           .Code, 500);
        }
      
        [TestMethod]
        public void StatusMessageManager_GetStatusMessage()
        {
            StatusMessageManager sm = new StatusMessageManager(new TreeMonDbContext(connectionKey));
            ServiceResult sr =  sm.Insert(new StatusMessage()
            {
                AccountId = "a",
                Name = "ALPHA",
                UUID = Guid.NewGuid().ToString("N"),
                 DateCreated = DateTime.UtcNow
            },false);

            Assert.AreEqual(sr.Code, 200, sr.Message);
            StatusMessage s = sm.GetStatusMessage("ALPHA");
            Assert.IsNotNull(s);
        }

        [TestMethod]
        public void StatusMessageManager_GetStatusMessageBy()
        {
            StatusMessageManager sm = new StatusMessageManager(new TreeMonDbContext(connectionKey));
            StatusMessage s = sm.GetStatusMessage("TEST_SM");
            Assert.IsNotNull(s);
            StatusMessage suid = sm.GetStatusMessageBy(s.UUID);
            Assert.IsNotNull(suid);
        }

        [TestMethod]
        public void StatusMessageManager_GetStatusMessages()
        {
            StatusMessageManager sm = new StatusMessageManager(new TreeMonDbContext(connectionKey));
            Assert.IsTrue(sm.GetStatusMessages("a").Count > 0);
        }

        [TestMethod]
        public void StatusMessageManager_UpdateStatusMessage()
        {
            StatusMessageManager sm = new StatusMessageManager(new TreeMonDbContext(connectionKey));
            sm.Insert(new StatusMessage()
            {
                AccountId = "a",
                Name = "TEST_SM",
                UUID = Guid.NewGuid().ToString("N")
            });
            StatusMessage s = sm.GetStatusMessage("TEST_SM");
            s.Name = "UPDATED_SM";

            Assert.AreEqual(sm.UpdateStatusMessage(s).Code, 200);
            StatusMessage u = sm.GetStatusMessage("UPDATED_SM");
            Assert.IsNotNull(u);
        }

        [TestMethod]
        public void StatusMessageManager_GetStatusByType()
        {
            StatusMessageManager sm = new StatusMessageManager(new TreeMonDbContext(connectionKey));
            sm.Insert(new StatusMessage()
            {
                AccountId = "a",
                Name = "SM_TYPE",
                CreatedBy = "TESTUSER",
                StatusType = "SymptomLog",
                DateCreated = DateTime.UtcNow
            });

            List<StatusMessage> s = sm.GetStatusByType("SymptomLog", "TESTUSER", "a");
            Assert.IsTrue(s.Count > 0);
         }

        [TestMethod]
        public void StatusMessageManager_DeleteStatusMessage()
        {
            StatusMessageManager sm = new StatusMessageManager(new TreeMonDbContext(connectionKey));
            StatusMessage s = new StatusMessage()
            {
                AccountId = "a",
                Name = "DELETESTATUS",
                CreatedBy = "TESTUSER",
                DateCreated = DateTime.UtcNow,
                StatusType = "SymptomLog"
            };

            sm.Insert(s);

            //Test the delete flag
            Assert.IsTrue(sm.DeleteStatusMessage(s) > 0);
            sm.GetStatusMessage("DELETESTATUS");
            StatusMessage d = sm.GetStatusMessage("DELETESTATUS");
            Assert.IsNotNull(d);
            Assert.IsTrue(d.Deleted == true);


            Assert.IsTrue(sm.DeleteStatusMessage(s,true) > 0);
            d = sm.GetStatusMessage("DELETESTATUS");
            Assert.IsNull(d);
        }


    }
}
