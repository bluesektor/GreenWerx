// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using TreeMon.Data;
using TreeMon.Managers.Event;
using TreeMon.Models.App;
using TreeMon.Models.Event;

namespace TreeMon.Web.Tests.Managers.Event
{
    [TestClass]
    public class NotificationManagerTests
    {
        ////private string connectionKey = "MSSQL_TEST";
        //[TestMethod]
        //public void NotificationManager_Insert() {
        //    NotificationManager nm = new NotificationManager(connectionKey);

        //    ServiceResult sr = nm.Insert(new Notification
        //    {
        //         Name = "TEST_NOTIFICATION",
        //        UUID = Guid.NewGuid().ToString("N"),
        //        UUIDType = "Notification",
        //        AccountUUID = "a"
        //    },false);

        //    Assert.AreEqual(sr.Code, 200);

        //    sr = nm.Insert(new Notification
        //    {
        //        Name = "TEST_NOTIFICATION",
        //        UUID = Guid.NewGuid().ToString("N"),
        //        UUIDType = "Notification",
        //        AccountUUID = "a"
        //    });
        //    //this will is 500 because by default there's validation by name.
        //    //so it won't insert if the name already exists.
        //    Assert.AreEqual(sr.Code, 500);
        //}

        //[TestMethod]
        //public void NotificationManager_GetNotification()
        //{
        //    NotificationManager nm = new NotificationManager(connectionKey);

        //    ServiceResult sr = nm.Insert(new Notification
        //    {
        //        Name = "TEST_NOTIFICATION",
        //        UUID = Guid.NewGuid().ToString("N"),
        //        UUIDType = "Notification",
        //        AccountUUID = "a"
        //    }, false);

        //    Notification n = nm.GetNotification("TEST_NOTIFICATION");
        //    Assert.IsNotNull(n);
        //    n = nm.GetNotificationBy(n.UUID);
        //    Assert.IsNotNull(n);
        //    Assert.IsTrue(nm.GetNotifications(n.AccountUUID).Count > 0);
        //}

        //[TestMethod]
        //public void NotificationManager_UpdateNotification()
        //{
        //    NotificationManager nm = new NotificationManager(connectionKey);
        //    Notification n = nm.GetNotification("TEST_NOTIFICATION");
        //    n.Name = "NOTIFICATION_UPDATED";

        //    Assert.AreEqual(nm.UpdateNotification(n).Code, 200);
        //    Notification nu = nm.GetNotification("NOTIFICATION_UPDATED");
        //    Assert.IsNotNull(nu);
        //}

        //[TestMethod]
        //public void NotificationManager_DeleteNotification()
        //{
        //    NotificationManager nm = new NotificationManager(connectionKey);

        //    ServiceResult sr = nm.Insert(new Notification
        //    {
        //        Name = "DELETE_NOTIFICATION",
        //        UUID = Guid.NewGuid().ToString("N"),
        //        UUIDType = "Notification",
        //        AccountUUID = "a"
        //    }, false);

        //    Notification n = nm.GetNotification("DELETE_NOTIFICATION");
        //    Assert.IsTrue(nm.DeleteNotification(n) > 0);
        //    Assert.IsTrue(nm.DeleteNotification(n,true) > 0);
            
        //}
    }
}
