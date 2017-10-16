// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TreeMon.Data;
using TreeMon.Models.App;
using System.Collections.Generic;
using TreeMon.Managers.Store;
using TreeMon.Models.Store;

namespace TreeMon.Web.Tests._templates
{
    [TestClass]
    public class StoreManager_Tests
    {
        private string connectionKey = "MSSQL_TEST";

        //[TestMethod]
        //public void StoreManager_Insert_Vendor()
        //{
        //    StoreManager m = new StoreManager(connectionKey);
        //    string name = Guid.NewGuid().ToString();
        //    Assert.AreEqual(
        //        m.Insert(new Vendor()
        //        {
        //            AccountUUID = "a",
        //            Name = name,
        //            DateCreated = DateTime.UtcNow
        //        }, false)
        //     .Code, 200);

        //    //won't allow a duplicate name
        //    Assert.AreEqual(
        //      m.Insert(new Vendor()
        //      {
        //          AccountUUID = "a",
        //          Name = name,
        //          DateCreated = DateTime.UtcNow
        //      })
        //   .Code, 500);

        //    Assert.IsNotNull(m.GetVendor(name));
        //}

        //[TestMethod]
        //public void StoreManager_GetVendor()
        //{
        //    StoreManager m = new StoreManager(connectionKey);
        //    string name = Guid.NewGuid().ToString();
        //    ServiceResult sr = m.Insert(new Vendor()
        //    {
        //        AccountUUID = "a",
        //        Name = "ALPHA" + name,
        //        DateCreated = DateTime.UtcNow
        //    }, false);

        //    Assert.AreEqual(sr.Code, 200, sr.Message);
        //    Vendor s = m.GetVendor("ALPHA" + name);
        //    Assert.IsNotNull(s);
        //}

        ////[TestMethod]
        ////public void StoreManager_GetVendorBy()
        ////{
        ////    StoreManager m = new StoreManager(connectionKey);
        ////    Assert.AreEqual(
        ////      m.Insert(new Vendor()
        ////      {
        ////          AccountUUID = "a",
        ////          Name = "TESTRECORD",
        ////          DateCreated = DateTime.UtcNow
        ////      }, false)
        ////   .Code, 200);
        ////    Vendor s = m.GetVendor("TESTRECORD");
        ////    Assert.IsNotNull(s);
        ////    Vendor suid = m.GetVendorBy(s.UUID);
        ////    Assert.IsNotNull(suid);
        ////}

        ////[TestMethod]
        ////public void StoreManager_GetVendors()
        ////{
        ////    StoreManager m = new StoreManager(connectionKey);
        ////    Assert.IsTrue(m.GetVendors("a").Count > 0);
        ////}

        ////[TestMethod]
        ////public void StoreManager_UpdateVendor()
        ////{
        ////    StoreManager m = new StoreManager(connectionKey);
        ////    m.Insert(new Vendor()
        ////    {
        ////        AccountUUID = "a",
        ////        Name = "TESTRECORD",
        ////    }, false);
        ////    Vendor s = m.GetVendor("TESTRECORD");
        ////    s.Name = "UPDATEDRECORD";

        ////    Assert.AreEqual(m.UpdateVendor(s).Code, 200);
        ////    Vendor u = m.GetVendor("UPDATEDRECORD");
        ////    Assert.IsNotNull(u);
        ////}

        ////[TestMethod]
        ////public void StoreManager_DeleteVendor()
        ////{
        ////    StoreManager m = new StoreManager(connectionKey);
        ////    string name = Guid.NewGuid().ToString();
        ////    Vendor s = new Vendor()
        ////    {
        ////        AccountUUID = "a",
        ////        Name = name,
        ////        CreatedBy = "TESTUSER",
        ////        DateCreated = DateTime.UtcNow,
        ////    };

        ////    m.Insert(s, false);

        ////    //Test the delete flag
        ////    Assert.IsTrue(m.DeleteVendor(s) > 0);
        ////    m.GetVendor("DELETERECORD");
        ////    Vendor d = m.GetVendor(name);
        ////    Assert.IsNotNull(d);
        ////    Assert.IsTrue(d.Deleted == true);


        ////    Assert.IsTrue(m.DeleteVendor(s, true) > 0);
        ////    d = m.GetVendor(name);
        ////    Assert.IsNull(d);
        ////}
    }
}
