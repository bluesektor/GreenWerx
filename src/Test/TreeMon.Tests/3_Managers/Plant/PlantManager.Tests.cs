// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TreeMon.Data;
using TreeMon.Models.App;
using System.Collections.Generic;
using TreeMon.Managers.Plant;
using TreeMon.Models.Store;
using TreeMon.Models.Plant;

namespace TreeMon.Web.Tests._templates
{
    [TestClass]
    public class PlantManager_Tests
    {
        //private string connectionKey = "MSSQL_TEST";

        //[TestMethod]
        //public void PlantManager_AddStrainFromProduct()
        //{
        //    PlantManager m = new PlantManager(connectionKey);
        //    string name = Guid.NewGuid().ToString();
        //    Product p = new Product()
        //    {
        //        AccountUUID = "a",
        //        Name = "Product-" + name
        //    };

        //    p.CategoryUUID = "VAPORIZER";
        //    Assert.IsNull(m.AddStrainFromProduct(p));
        //    p.CategoryUUID = "BONG";
        //    Assert.IsNull(m.AddStrainFromProduct(p));
        //    p.CategoryUUID = "GLASS";
        //    Assert.IsNull(m.AddStrainFromProduct(p));
        //    p.CategoryUUID = "MISC";
        //    Assert.IsNull(m.AddStrainFromProduct(p));
        //    p.CategoryUUID = "VAPE PEN CARTRIDGES";
        //    Assert.IsNull(m.AddStrainFromProduct(p));
        //    p.CategoryUUID = "BOOTH";
        //    Assert.IsNull(m.AddStrainFromProduct(p));

        //    Strain s = new Strain()
        //    {
        //        AccountUUID = "a",
        //        Name = "Strain-" + name,
        //        CreatedBy = "test",
        //        VarietyUUID = "hybrid"
                
        //    };

        //    Assert.AreEqual(m.Insert(s).Code, 200);

        //    p.StrainUUID = s.UUID;
        //    p.CategoryUUID = s.VarietyUUID;

        //    Strain strainTest = m.AddStrainFromProduct(p);
        //    Assert.IsNotNull(strainTest);
        //    Assert.AreEqual(strainTest.UUID, s.UUID);

        //    s.UUID = s.Name;
        //    Assert.AreEqual(m.UpdateStrain(s).Code, 200);
        //    //try getting the strain by name with the UUID because the ui allows adding via text/combobox.
        //    strainTest = m.AddStrainFromProduct(p);
        //    Assert.IsNotNull(strainTest);
        //   // Assert.AreEqual(strainTest.UUID, s.Name);

        //    p.CategoryUUID = "xashybridseg";

        //    strainTest = m.AddStrainFromProduct(p);
        //    Assert.IsNotNull(strainTest);
        //    Assert.AreEqual(strainTest.VarietyUUID?.ToLower(), "hybrid");
        //}

        //[TestMethod]
        //public void PlantManager_Insert_Strain()
        //{
        //    PlantManager m = new PlantManager(connectionKey);
        //    string name = Guid.NewGuid().ToString();
        //    Assert.AreEqual(
        //        m.Insert(new Strain()
        //        {
        //            AccountUUID = "a",
        //            Name = name,
        //            DateCreated = DateTime.UtcNow
        //        }, false)
        //     .Code, 200);

        //    //won't allow a duplicate name
        //    Assert.AreEqual(
        //      m.Insert(new Strain()
        //      {
        //          AccountUUID = "a",
        //          Name = name,
        //          DateCreated = DateTime.UtcNow
        //      })
        //   .Code, 500);

        //    Assert.IsNotNull(m.GetStrain(name));
        //}

        //[TestMethod]
        //public void PlantManager_GetStrain()
        //{
        //    PlantManager m = new PlantManager(connectionKey);
        //    string name = Guid.NewGuid().ToString();
        //    ServiceResult sr = m.Insert(new Strain()
        //    {
        //        AccountUUID = "a",
        //        Name = "ALPHA" + name,
        //        DateCreated = DateTime.UtcNow
        //    }, false);

        //    Assert.AreEqual(sr.Code, 200, sr.Message);
        //    Strain s = m.GetStrain("ALPHA" + name);
        //    Assert.IsNotNull(s);
        //}

        //[TestMethod]
        //public void PlantManager_GetStrainBy()
        //{
        //    PlantManager m = new PlantManager(connectionKey);
        //    Assert.AreEqual(
        //      m.Insert(new Strain()
        //      {
        //          AccountUUID = "a",
        //          Name = "TESTRECORD",
        //          DateCreated = DateTime.UtcNow
        //      }, false)
        //   .Code, 200);
        //    Strain s = m.GetStrain("TESTRECORD");
        //    Assert.IsNotNull(s);
        //    Strain suid = m.GetStrainBy(s.UUID);
        //    Assert.IsNotNull(suid);
        //}

        //[TestMethod]
        //public void PlantManager_GetStrains()
        //{
        //    PlantManager m = new PlantManager(connectionKey);
        //    Assert.IsTrue(m.GetStrains("a").Count > 0);
        //}

        //[TestMethod]
        //public void PlantManager_UpdateStrain()
        //{
        //    PlantManager m = new PlantManager(connectionKey);
        //    m.Insert(new Strain()
        //    {
        //        AccountUUID = "a",
        //        Name = "TESTRECORD",
        //    }, false);
        //    Strain s = m.GetStrain("TESTRECORD");
        //    s.Name = "UPDATEDRECORD";

        //    Assert.AreEqual(m.UpdateStrain(s).Code, 200);
        //    Strain u = m.GetStrain("UPDATEDRECORD");
        //    Assert.IsNotNull(u);
        //}

        //[TestMethod]
        //public void PlantManager_DeleteStrain()
        //{
        //    PlantManager m = new PlantManager(connectionKey);
        //    string name = Guid.NewGuid().ToString();
        //    Strain s = new Strain()
        //    {
        //        AccountUUID = "a",
        //        Name = name,
        //        CreatedBy = "TESTUSER",
        //        DateCreated = DateTime.UtcNow,
        //    };

        //    m.Insert(s, false);

        //    //Test the delete flag
        //    Assert.IsTrue(m.DeleteStrain(s) > 0);
        //    m.GetStrain("DELETERECORD");
        //    Strain d = m.GetStrain(name);
        //    Assert.IsNotNull(d);
        //    Assert.IsTrue(d.Deleted == true);


        //    Assert.IsTrue(m.DeleteStrain(s, true) > 0);
        //    d = m.GetStrain(name);
        //    Assert.IsNull(d);
        //}
    }
}
