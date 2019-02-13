// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TreeMon.Data;
using TreeMon.Models.App;
using System.Collections.Generic;
using TreeMon.Managers;
using TreeMon.Models;

namespace TreeMon.Web.Tests._templates
{
    [TestClass]
    public class UnitOfMeasureManager_Tests
    {
        private string connectionKey = "MSSQL_TEST";

        //[TestMethod]
        //public void UnitOfMeasureManager_Insert_UnitOfMeasure()
        //{
        //     UnitOfMeasureManager m = new UnitOfMeasureManager(connectionKey);
        //    string name = Guid.NewGuid().ToString();
        //    Assert.AreEqual(
        //        m.Insert(new UnitOfMeasure()
        //        {
        //            Category = name,
        //            AccountUUID = "a",
        //            Name = name,
        //            DateCreated = DateTime.UtcNow
        //        }, false)
        //     .Code, 200);

        //    //won't allow a duplicate name
        //    Assert.AreEqual(
        //      m.Insert(new UnitOfMeasure()
        //      {
        //          AccountUUID = "a",
        //          Name = name,
        //          DateCreated = DateTime.UtcNow
        //      })
        //   .Code, 500);

        //    Assert.IsNotNull(m.GetUnitOfMeasure(name));
        //}

        //[TestMethod]
        //public void UnitOfMeasureManager_GetUnitOfMeasure()
        //{
        //    UnitOfMeasureManager m = new UnitOfMeasureManager(connectionKey);
        //    string name = Guid.NewGuid().ToString();
        //    ServiceResult sr = m.Insert(new UnitOfMeasure()
        //    {
        //        AccountUUID = "a",
        //        Name = "ALPHA" + name,
        //        DateCreated = DateTime.UtcNow
        //    }, false);

        //    Assert.AreEqual(sr.Code, 200, sr.Message);
        //    UnitOfMeasure s = m.GetUnitOfMeasure("ALPHA" + name);
        //    Assert.IsNotNull(s);
        //}

        //[TestMethod]
        //public void UnitOfMeasureManager_GetUnitOfMeasureBy()
        //{
        //    UnitOfMeasureManager m = new UnitOfMeasureManager(connectionKey);
        //    Assert.AreEqual(
        //      m.Insert(new UnitOfMeasure()
        //      {
        //          AccountUUID = "a",
        //          Name = "TESTRECORD",
        //          DateCreated = DateTime.UtcNow
        //      }, false)
        //   .Code, 200);
        //    UnitOfMeasure s = m.GetUnitOfMeasure("TESTRECORD");
        //    Assert.IsNotNull(s);
        //    UnitOfMeasure suid = m.GetUnitOfMeasureBy(s.UUID);
        //    Assert.IsNotNull(suid);
        //}

        //[TestMethod]
        //public void UnitOfMeasureManager_GetUnitsOfMeasure()
        //{
        //    UnitOfMeasureManager m = new UnitOfMeasureManager(connectionKey);
        //    Assert.IsTrue(m.GetUnitsOfMeasure("a").Count > 0);
        //}

        //[TestMethod]
        //public void UnitOfMeasureManager_GetUnitsOfMeasure_Category()
        //{
        //    UnitOfMeasureManager m = new UnitOfMeasureManager(connectionKey);
        //    string category = Guid.NewGuid().ToString();
        //    Assert.AreEqual(
        //        m.Insert(new UnitOfMeasure()
        //        {
        //            Category = category,
        //            AccountUUID = "a",
        //            Name = "test",
        //            DateCreated = DateTime.UtcNow
        //        }, false)
        //     .Code, 200);

        //    Assert.IsTrue(m.GetUnitsOfMeasure("a", category).Count > 0);
        //}

        //[TestMethod]
        //public void UnitOfMeasureManager_UpdateUnitOfMeasure()
        //{
        //    UnitOfMeasureManager m = new UnitOfMeasureManager(connectionKey);
        //    m.Insert(new UnitOfMeasure()
        //    {
        //        AccountUUID = "a",
        //        Name = "TESTRECORD",
        //    }, false);
        //    UnitOfMeasure s = m.GetUnitOfMeasure("TESTRECORD");
        //    s.Name = "UPDATEDRECORD";

        //    Assert.AreEqual(m.UpdateUnitOfMeasure(s).Code, 200);
        //    UnitOfMeasure u = m.GetUnitOfMeasure("UPDATEDRECORD");
        //    Assert.IsNotNull(u);
        //}

        //[TestMethod]
        //public void UnitOfMeasureManager_DeleteUnitOfMeasure()
        //{
        //    UnitOfMeasureManager m = new UnitOfMeasureManager(connectionKey);
        //    string name = Guid.NewGuid().ToString();
        //    UnitOfMeasure s = new UnitOfMeasure()
        //    {
        //        AccountUUID = "a",
        //        Name = name,
        //        CreatedBy = "TESTUSER",
        //        DateCreated = DateTime.UtcNow,
        //    };

        //    m.Insert(s, false);

        //    //Test the delete flag
        //    Assert.IsTrue(m.DeleteUnitOfMeasure(s) > 0);
        //    m.GetUnitOfMeasure("DELETERECORD");
        //    UnitOfMeasure d = m.GetUnitOfMeasure(name);
        //    Assert.IsNotNull(d);
        //    Assert.IsTrue(d.Deleted == true);


        //    Assert.IsTrue(m.DeleteUnitOfMeasure(s, true) > 0);
        //    d = m.GetUnitOfMeasure(name);
        //    Assert.IsNull(d);
        //}
    }
}
