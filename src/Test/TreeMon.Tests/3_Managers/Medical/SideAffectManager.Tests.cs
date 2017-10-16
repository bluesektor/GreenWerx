// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TreeMon.Data;
using TreeMon.Models.App;
using System.Collections.Generic;
using TreeMon.Managers.Medical;
using TreeMon.Models.Medical;

namespace TreeMon.Web.Tests._templates
{
    [TestClass]
    public class SideAffectManager_Tests
    {
        private string connectionKey = "MSSQL_TEST";

        //[TestMethod]
        //public void SideAffectManager_Insert_SideAffect()
        //{
        //    SideAffectManager m = new SideAffectManager(connectionKey);

        //    Assert.AreEqual(
        //        m.Insert(new SideAffect()
        //        {
        //            AccountUUID = "a",
        //            Name = "TESTRECORD",
        //            DateCreated = DateTime.UtcNow,
        //             SymptomDate = DateTime.UtcNow

        //        },false)
        //     .Code, 200);

        //    //won't allow a duplicate name
        //    Assert.AreEqual(
        //      m.Insert(new SideAffect()
        //      {
        //          AccountUUID = "a",
        //          Name = "TESTRECORD",
        //          DateCreated = DateTime.UtcNow,
        //          SymptomDate = DateTime.UtcNow
        //      })
        //   .Code, 500);
        //}

        //[TestMethod]
        //public void SideAffectManager_GetSideAffect()
        //{
        //    SideAffectManager m = new SideAffectManager(connectionKey);
        //    ServiceResult sr = m.Insert(new SideAffect()
        //    {
        //        AccountUUID = "a",
        //        Name = "ALPHA",
        //        DateCreated = DateTime.UtcNow
        //    }, false);

        //    Assert.AreEqual(sr.Code, 200, sr.Message);
        //    SideAffect s = m.GetSideAffect("ALPHA");
        //    Assert.IsNotNull(s);
        //}

        //[TestMethod]
        //public void SideAffectManager_GetSideAffectBy()
        //{
        //    SideAffectManager m = new SideAffectManager(connectionKey);
        //    Assert.AreEqual(
        //      m.Insert(new SideAffect()
        //      {
        //          AccountUUID = "a",
        //          Name = "TESTRECORD",
        //          DateCreated = DateTime.UtcNow
        //      }, false)
        //   .Code, 200);
        //    SideAffect s = m.GetSideAffect("TESTRECORD");
        //    Assert.IsNotNull(s);
        //    SideAffect suid = m.GetSideAffectBy(s.UUID);
        //    Assert.IsNotNull(suid);
        //}

        //[TestMethod]
        //public void SideAffectManager_GetSideAffects()
        //{
        //    SideAffectManager m = new SideAffectManager(connectionKey);
        //    Assert.IsTrue(m.GetSideAffects("a").Count > 0);
        //}

        //[TestMethod]
        //public void SideAffectManager_UpdateSideAffect()
        //{
        //    SideAffectManager m = new SideAffectManager(connectionKey);
        //    m.Insert(new SideAffect()
        //    {
        //        AccountUUID = "a",
        //        Name = "TESTRECORD",
        //    });
        //    SideAffect s = m.GetSideAffect("TESTRECORD");
        //    s.Name = "UPDATEDRECORD";

        //    Assert.AreEqual(m.UpdateSideAffect(s).Code, 200);
        //    SideAffect u = m.GetSideAffect("UPDATEDRECORD");
        //    Assert.IsNotNull(u);
        //}

        //[TestMethod]
        //public void SideAffectManager_DeleteSideAffect()
        //{
        //    SideAffectManager m = new SideAffectManager(connectionKey);
        //    SideAffect s = new SideAffect()
        //    {
        //        AccountUUID = "a",
        //        Name = "DELETERECORD",
        //        CreatedBy = "TESTUSER",
        //        DateCreated = DateTime.UtcNow,
        //    };

        //    m.Insert(s);

        //    //Test the delete flag
        //    Assert.IsTrue(m.DeleteSideAffect(s) > 0);
        //    m.GetSideAffect("DELETERECORD");
        //    SideAffect d = m.GetSideAffect("DELETERECORD");
        //    Assert.IsNotNull(d);
        //    Assert.IsTrue(d.Deleted == true);


        //    Assert.IsTrue(m.DeleteSideAffect(s, true) > 0);
        //    d = m.GetSideAffect("DELETERECORD");
        //    Assert.IsNull(d);
        //}
    }
}
