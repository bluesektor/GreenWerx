using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using TreeMon.Data;
using TreeMon.Managers.Event;
using TreeMon.Models.App;
using TreeMon.Models.Event;

namespace TreeMon.Web.Tests.Managers.Event
{
    [TestClass]
    public  class ReminderManagerTests
    {
        private string connectionKey = "MSSQL_TEST";
        [TestMethod]
        public void ReminderManager_Insert()
        {
            ReminderManager nm = new ReminderManager(new TreeMonDbContext(connectionKey));

            ServiceResult sr = nm.Insert(new Reminder
            {
                Name = "TEST_REMINDER",
                UUID = Guid.NewGuid().ToString("N"),
                UUIDType = "Reminder",
                AccountId = "a"
            }, false);

            Assert.AreEqual(sr.Code, 200);

            sr = nm.Insert(new Reminder
            {
                Name = "TEST_REMINDER",
                UUID = Guid.NewGuid().ToString("N"),
                UUIDType = "Notification",
                AccountId = "a"
            });
            //this will is 500 because by default there's validation by name.
            //so it won't insert if the name already exists.
            Assert.AreEqual(sr.Code, 500);
         
        }

        [TestMethod]
        public void ReminderManager_InsertRule()
        {
            ReminderManager nm = new ReminderManager(new TreeMonDbContext(connectionKey));

            //rule to turn off notifications a the times
            ServiceResult res = nm.InsertRule(new ReminderRule()
            {
                RangeStart = "2",
                RangeEnd = "8",
                RangeType = "time", //date
                RuleType = "Reminder.Notifications.Off.Range" ,//turn off the notification
                UUID = Guid.NewGuid().ToString("N")
            });
            
            Assert.AreEqual(res.Code, 200);
        }

        [TestMethod]
        public void ReminderManager_GetReminder()
        {
            ReminderManager rm = new ReminderManager(new TreeMonDbContext(connectionKey));
            Reminder r = rm.GetReminder("TEST_REMINDER");
            Assert.IsNotNull(r);
       
        }

        [TestMethod]
        public void ReminderManager_GetReminders()
        {
            ReminderManager rm = new ReminderManager(new TreeMonDbContext(connectionKey));
            Assert.IsTrue(rm.GetReminders("a").Count > 0);
        }

        [TestMethod]
        public void ReminderManager_GetReminderBy()
        {
            ReminderManager rm = new ReminderManager(new TreeMonDbContext(connectionKey));
            Reminder r = rm.GetReminder("TEST_REMINDER");
            Assert.IsNotNull(rm.GetReminderBy(r.UUID));
        }

        [TestMethod]
        public void ReminderManager_UpdateReminder()
        {
            ReminderManager rm = new ReminderManager(new TreeMonDbContext(connectionKey));
            ServiceResult sr = rm.Insert(new Reminder
            {
                Name = "TEST_REMINDER",
                UUID = Guid.NewGuid().ToString("N"),
                UUIDType = "Reminder",
                AccountId = "a"
            }, false);
            Reminder r = rm.GetReminder("TEST_REMINDER");
            r.Name = "REMINDER_UPDATE";
            Assert.AreEqual(rm.UpdateReminder(r).Code, 200);
             r = rm.GetReminder("REMINDER_UPDATE");
            Assert.IsNotNull(r);
        }

        [TestMethod]
        public void ReminderManager_DeleteReminder()
        {
            ReminderManager rm = new ReminderManager(new TreeMonDbContext(connectionKey));
            Reminder r = rm.GetReminder("REMINDER_UPDATE");

            //Test setting the delete flag.
            Assert.IsTrue(rm.DeleteReminder(r) > 0);
            r = rm.GetReminder("REMINDER_UPDATE");
            Assert.IsNotNull(r);

            Assert.IsTrue(r.Deleted == true);

            r.Name = "PURGE";
            rm.UpdateReminder(r);
            //Test the purge
            Assert.IsTrue(rm.DeleteReminder(r,true) > 0);
            r = rm.GetReminder("PURGE");
            Assert.IsNull(r);
       
        }

    }
}
