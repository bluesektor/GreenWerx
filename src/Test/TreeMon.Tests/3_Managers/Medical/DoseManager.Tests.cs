// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TreeMon.Data;
using TreeMon.Models.App;
using System.Collections.Generic;
using TreeMon.Managers;
using TreeMon.Models.Medical;

namespace TreeMon.Web.Tests._templates
{
    [TestClass]
    public class DoseManager_Tests
    {
        private string connectionKey = "MSSQL_TEST";

        //[TestMethod]
        //public void DoseManager_Insert_DoseLog()
        //{
        //    DoseManager m = new DoseManager(connectionKey);

        //    Assert.AreEqual(
        //        m.Insert(new DoseLog()
        //        {
        //            AccountUUID = "a",
        //            Name = "TESTRECORD",
        //            DateCreated = DateTime.UtcNow,
        //             DoseDateTime = DateTime.UtcNow
        //        })
        //     .Code, 200);

        //    //won't allow a duplicate name
        //    Assert.AreEqual(
        //      m.Insert(new DoseLog()
        //      {
        //          AccountUUID = "a",
        //          Name = "TESTRECORD",
        //          DateCreated = DateTime.UtcNow,
        //          DoseDateTime = DateTime.UtcNow
        //      })
        //   .Code, 500);
        //}

        //[TestMethod]
        //public void DoseManager_GetDose()
        //{
        //    DoseManager m = new DoseManager(connectionKey);
        //    ServiceResult sr = m.Insert(new DoseLog()
        //    {
        //        AccountUUID = "a",
        //        Name = "ALPHA",
        //        DateCreated = DateTime.UtcNow,
        //        DoseDateTime = DateTime.UtcNow
        //    }, false);

        //    Assert.AreEqual(sr.Code, 200, sr.Message);
        //    DoseLog s = m.GetDose("ALPHA");
        //    Assert.IsNotNull(s);
        //}

        //[TestMethod]
        //public void DoseManager_GetDoseBy()
        //{
        //    DoseManager m = new DoseManager(connectionKey);
        //    Assert.AreEqual(
        //      m.Insert(new DoseLog()
        //      {
        //          AccountUUID = "a",
        //          Name = "TESTRECORD",
        //          DateCreated = DateTime.UtcNow,
        //          DoseDateTime = DateTime.UtcNow
        //      },false)
        //   .Code, 200);
        //    DoseLog s = m.GetDose("TESTRECORD");
        //    Assert.IsNotNull(s);
        //    DoseLog suid = m.GetDoseBy(s.UUID);
        //    Assert.IsNotNull(suid);
        //}

        //[TestMethod]
        //public void DoseManager_GetDoses()
        //{
        //    DoseManager m = new DoseManager(connectionKey);
        //    Assert.IsTrue(m.GetDoses("a").Count > 0);
        //}

        //[TestMethod]
        //public void DoseManager_UpdateDoseLog()
        //{
        //    DoseManager m = new DoseManager(connectionKey);
        //    m.Insert(new DoseLog()
        //    {
        //        AccountUUID = "a",
        //        Name = "TESTRECORD",
        //        DoseDateTime = DateTime.UtcNow
        //    });
        //    DoseLog s = m.GetDose("TESTRECORD");
        //    s.Name = "UPDATEDRECORD";

        //    Assert.AreEqual(m.UpdateDose(s).Code, 200);
        //    DoseLog u = m.GetDose("UPDATEDRECORD");
        //    Assert.IsNotNull(u);
        //}

        //[TestMethod]
        //public void DoseManager_DeleteDoseLog()
        //{
        //    DoseManager m = new DoseManager(connectionKey);
        //    DoseLog s = new DoseLog()
        //    {
        //        AccountUUID = "a",
        //        Name = "DELETERECORD",
        //        CreatedBy = "TESTUSER",
        //        DateCreated = DateTime.UtcNow,
        //        DoseDateTime = DateTime.UtcNow
        //    };

        //    m.Insert(s);

        //    //Test the delete flag
        //    Assert.IsTrue(m.DeleteDose(s) > 0);
        //    m.GetDose("DELETERECORD");
        //    DoseLog d = m.GetDose("DELETERECORD");
        //    Assert.IsNotNull(d);
        //    Assert.IsTrue(d.Deleted == true);


        //    Assert.IsTrue(m.DeleteDose(s, true) > 0);
        //    d = m.GetDose("DELETERECORD");
        //    Assert.IsNull(d);
        //}
    }
}
