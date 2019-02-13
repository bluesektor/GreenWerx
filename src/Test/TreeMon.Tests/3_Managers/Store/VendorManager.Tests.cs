// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TreeMon.Data;
using TreeMon.Models.App;
using System.Collections.Generic;
using TreeMon.Managers.Store;
using TreeMon.Models.Store;
using TreeMon.Models.Plant;

namespace TreeMon.Web.Tests._templates
{
    [TestClass]
    public class    _Tests
    {
        private string connectionKey = "MSSQL_TEST";

        //[TestMethod]
        //public void VendorManager_GetAccountVendors()
        //{
        //    VendorManager m = new VendorManager(connectionKey);
        //    string name = Guid.NewGuid().ToString();
        //    string userUUID = Guid.NewGuid().ToString();
        //    Vendor p = new Vendor()
        //    {
        //        AccountUUID = name,
        //        Name = name,
        //        DateCreated = DateTime.UtcNow,
        //        CreatedBy = userUUID

        //    };
        //    Assert.AreEqual(m.Insert(p, false).Code, 200);

        //    p = new Vendor()
        //    {
        //        AccountUUID = name,
        //        Name = Guid.NewGuid().ToString(),
        //        DateCreated = DateTime.UtcNow,
        //        CreatedBy = userUUID
        //    };
        //    Assert.AreEqual(m.Insert(p, false).Code, 200);

        //    Assert.IsTrue(m.GetAccountVendors("a").Count >= 2);
        //}

        //[TestMethod]
        //public void VendorManager_GetUserVendors()
        //{
        //    VendorManager m = new VendorManager(connectionKey);
        //    string guid = Guid.NewGuid().ToString();
        //    string userUUID = Guid.NewGuid().ToString();
        //    Vendor p = new Vendor()
        //    {
        //        AccountUUID = guid,
        //        Name = guid,
        //        DateCreated = DateTime.UtcNow,
        //        CreatedBy = userUUID

        //    };
        //    Assert.AreEqual(m.Insert(p, false).Code, 200);

        //    p = new Vendor()
        //    {
        //        AccountUUID = guid,
        //        Name = Guid.NewGuid().ToString(),
        //        DateCreated = DateTime.UtcNow,
        //        CreatedBy = userUUID
        //    };
        //    Assert.AreEqual(m.Insert(p, false).Code, 200);

        //    Assert.IsTrue(m.GetUserVendors(guid, userUUID).Count >= 2);
        //}

        //[TestMethod]
        //public void VendorManager_AddVendorFromProduct()
        //{
        //    VendorManager m = new VendorManager(connectionKey);
        //    string name = Guid.NewGuid().ToString();
        //    Product p = new Product()
        //    {
        //        AccountUUID = "a",
        //        Name = "Product-" + name,
        //        CreatedBy = "owner",
        //        ManufacturerUUID = name
        //    };
        //    Vendor v = new Vendor()
        //    {
        //        AccountUUID = p.AccountUUID,
        //        Active = true,
        //        CreatedBy = p.CreatedBy,
        //        DateCreated = DateTime.UtcNow,
        //        Deleted = false,
        //        Name = p.ManufacturerUUID,
        //        UUIDType = "Vendor"

        //    };

        //    Assert.AreEqual(m.Insert(v).Code, 200);
        //    p.ManufacturerUUID = v.UUID;

        //    Vendor vendorTest = m.AddVendorFromProduct(p);
        //    Assert.IsNotNull(vendorTest);
        //    Assert.AreEqual(vendorTest.UUID, p.ManufacturerUUID);

        //    v.UUID = v.Name;
        //    Assert.AreEqual(m.UpdateVendor(v).Code, 200);
        //    //try getting the strain by name with the UUID because the ui allows adding via text/combobox.
        //    vendorTest = m.AddVendorFromProduct(p);
        //    Assert.IsNotNull(vendorTest);
        //    Assert.AreEqual(vendorTest.Name, p.ManufacturerUUID);
        //}

        //[TestMethod]
        //public void VendorManager_AddVendorFromStrain()
        //{
        //    VendorManager m = new VendorManager(connectionKey);
        //    string name = Guid.NewGuid().ToString();
        //    Strain s = new Strain()
        //    {
        //        AccountUUID = "a",
        //        Name = "Strain-" + name,
        //        BreederUUID = "invalidid" + name,
        //        CreatedBy = "owner"
        //    };

        //    Vendor v = new Vendor()
        //    {
        //        AccountUUID = s.AccountUUID,
        //        Active = true,
        //        CreatedBy = s.CreatedBy,
        //        DateCreated = DateTime.UtcNow,
        //        Deleted = false,
        //        Name = s.BreederUUID,
        //        UUIDType = "Vendor"
                
        //    };

        //    Assert.AreEqual(m.Insert(v).Code, 200);
        //    s.BreederUUID = v.UUID;

        //    Vendor vendorTest = m.AddVendorFromStrain(s);
        //    Assert.IsNotNull(vendorTest);
        //    Assert.AreEqual(vendorTest.UUID, s.BreederUUID);

        //    v.UUID = v.Name;
        //    Assert.AreEqual(m.UpdateVendor(v).Code, 200);
        //    //try getting the strain by name with the UUID because the ui allows adding via text/combobox.
        //    vendorTest = m.AddVendorFromStrain(s);
        //    Assert.IsNotNull(vendorTest);
        //    Assert.AreEqual(vendorTest.Name, s.BreederUUID);
        //}

        //[TestMethod]
        //public void VendorManager_Insert_Vendor()
        //{
        //    VendorManager m = new VendorManager(connectionKey);
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
        //public void VendorManager_GetVendor()
        //{
        //    VendorManager m = new VendorManager(connectionKey);
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

        //[TestMethod]
        //public void VendorManager_GetVendorBy()
        //{
        //    VendorManager m = new VendorManager(connectionKey);
        //    Assert.AreEqual(
        //      m.Insert(new Vendor()
        //      {
        //          AccountUUID = "a",
        //          Name = "TESTRECORD",
        //          DateCreated = DateTime.UtcNow
        //      }, false)
        //   .Code, 200);
        //    Vendor s = m.GetVendor("TESTRECORD");
        //    Assert.IsNotNull(s);
        //    Vendor suid = m.GetVendorBy(s.UUID);
        //    Assert.IsNotNull(suid);
        //}

        //[TestMethod]
        //public void VendorManager_GetVendors()
        //{
        //    VendorManager m = new VendorManager(connectionKey);
        //    Assert.IsTrue(m.GetVendors("a").Count > 0);
        //}

        //[TestMethod]
        //public void VendorManager_UpdateVendor()
        //{
        //    VendorManager m = new VendorManager(connectionKey);
        //    m.Insert(new Vendor()
        //    {
        //        AccountUUID = "a",
        //        Name = "TESTRECORD",
        //    }, false);
        //    Vendor s = m.GetVendor("TESTRECORD");
        //    s.Name = "UPDATEDRECORD";

        //    Assert.AreEqual(m.UpdateVendor(s).Code, 200);
        //    Vendor u = m.GetVendor("UPDATEDRECORD");
        //    Assert.IsNotNull(u);
        //}

        //[TestMethod]
        //public void VendorManager_DeleteVendor()
        //{
        //    VendorManager m = new VendorManager(connectionKey);
        //    string name = Guid.NewGuid().ToString();
        //    Vendor s = new Vendor()
        //    {
        //        AccountUUID = "a",
        //        Name = name,
        //        CreatedBy = "TESTUSER",
        //        DateCreated = DateTime.UtcNow,
        //    };

        //    m.Insert(s, false);

        //    //Test the delete flag
        //    //Assert.IsTrue(m.DeleteVendor(s) > 0);
        //    //m.GetVendor("DELETERECORD");
        //    Vendor d = m.GetVendor(name);
        //    //Assert.IsNotNull(d);
        //    //Assert.IsTrue(d.Deleted == true);


        //    Assert.IsTrue(m.DeleteVendor(s) > 0);
        //    d = m.GetVendor(name);
        //    Assert.IsNull(d);
        //}
    }
}
