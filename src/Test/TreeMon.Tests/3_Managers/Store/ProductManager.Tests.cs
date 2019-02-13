// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TreeMon.Data;
using TreeMon.Models.App;
using System.Collections.Generic;
using TreeMon.Managers.Store;
using TreeMon.Models.Store;
using TreeMon.Data.Logging.Models;

namespace TreeMon.Web.Tests._templates
{
    [TestClass]
    public class ProductManager_Tests
    {
        private string connectionKey = "MSSQL_TEST";

        public string SystemFlags { get; private set; }

     //   [TestMethod]
     //   public void ProductManager_GetAll()
     //   {
     //       ProductManager m = new ProductManager(connectionKey);
     //       string name = Guid.NewGuid().ToString();
     //       Assert.AreEqual(
     //           m.Insert(new Product()
     //           {
     //               AccountUUID = "a",
     //               Name = name,
     //               DateCreated = DateTime.UtcNow,
     //               CategoryUUID = "ZETA"
     //           }, false)
     //        .Code, 200);

     //       //won't allow a duplicate name
     //       Assert.AreEqual(
     //         m.Insert(new Product()
     //         {
     //             AccountUUID = "a",
     //             Name = name,
     //             DateCreated = DateTime.UtcNow,
     //             CategoryUUID = "ZETA"
     //         },false)
     //      .Code, 200);

     //       Assert.IsNotNull(m.GetAll("ZETA").Count >= 2);
     //   }
     
     ////  [TestMethod]
     //   //public void ProductManager_GetCategories() //todo test for getingn categories of type = product.
     //   //{
     //   //    ProductManager m = new ProductManager(connectionKey);
     //   //    string name = Guid.NewGuid().ToString();
     //   //    string userUUID = Guid.NewGuid().ToString();
     //   //    Product pg = new Product()
     //   //    {
     //   //        AccountUUID = "a",
     //   //        Name = name,
     //   //        DateCreated = DateTime.UtcNow,
     //   //        CategoryUUID = "gaMa",
     //   //        CreatedBy = userUUID

     //   //    };
     //   //    Assert.AreEqual(m.Insert(pg, false).Code, 200);

     //   //    Product pd = new Product()
     //   //    {
     //   //        AccountUUID = "a",
     //   //        Name = Guid.NewGuid().ToString(),
     //   //        DateCreated = DateTime.UtcNow,
     //   //        CategoryUUID = "deLta",
     //   //        CreatedBy = userUUID
     //   //    };
     //   //    Assert.AreEqual(m.Insert(pd, false).Code, 200);

     //   //    List<string> products = m.GetCategories(pd.AccountUUID);
     //   //    Assert.IsTrue(products.Count >= 2);
     //   //    int catCount = 0;
     //   //    foreach(string ptmp in products)
     //   //    {
     //   //        if (ptmp == "gaMa" || ptmp == "deLta")
     //   //            catCount++;

     //   //    }
     //   //    Assert.IsTrue(catCount == 2);

     //   //    m.DeleteProduct(pg);
     //   //    m.DeleteProduct(pd);
     //   //}

     //   [TestMethod]
     //   public void ProductManager_GetCombinedProducts()
     //   {
     //       TreeMonDbContext ctx = new TreeMonDbContext( connectionKey );
     //       ctx.Database.ExecuteSqlCommand("DELETE FROM Products WHERE AccountUUID='zxz' OR AccountUUID = '" + SystemFlag.Default.Account +"'");
            

     //       ProductManager m = new ProductManager(connectionKey);

     //       string name = Guid.NewGuid().ToString();
     //       string userUUID = Guid.NewGuid().ToString();
     //       Product p = new Product()
     //       {
     //           AccountUUID = "zxz",
     //           Name = name,
     //           DateCreated = DateTime.UtcNow,
     //           CategoryUUID = "BETA",
     //           CreatedBy = userUUID

     //       };
     //       Assert.AreEqual(m.Insert(p, false).Code, 200);

     //       p = new Product()
     //       {
     //           AccountUUID = "zxz",
     //           Name = Guid.NewGuid().ToString(),
     //           DateCreated = DateTime.UtcNow,
     //           CategoryUUID = "ALPHA",
     //           CreatedBy = userUUID
     //       };
     //       Assert.AreEqual(m.Insert(p, false).Code, 200);

     //       p = new Product()//this should not be in result
     //       {
     //           AccountUUID = SystemFlag.Default.Account,
     //           Name = Guid.NewGuid().ToString(),
     //           DateCreated = DateTime.UtcNow,
     //           CategoryUUID = "ALPHA",
     //           CreatedBy = userUUID
     //       };
     //       Assert.AreEqual(m.Insert(p, false).Code, 200);

     //       p = new Product()//this should be in result
     //       {
     //           AccountUUID = SystemFlag.Default.Account,
     //           Name = Guid.NewGuid().ToString(),
     //           DateCreated = DateTime.UtcNow,
     //           CategoryUUID = "GAMMA",
     //           CreatedBy = userUUID
     //       };
     //       Assert.AreEqual(m.Insert(p, false).Code, 200);

     //       List<Product> products = m.GetCombinedProducts(userUUID, "zxz");
     //       Assert.AreEqual( products.Count , 3);

     //       foreach(Product ptmp in products)
     //       {
     //           if(ptmp.CategoryUUID == "ALPHA")
     //               Assert.IsTrue(ptmp.AccountUUID == "zxz");

     //           if (ptmp.CategoryUUID == "BETA")
     //               Assert.IsTrue(ptmp.AccountUUID == "zxz");

     //           if (ptmp.CategoryUUID == "GAMMA")
     //               Assert.IsTrue(ptmp.AccountUUID == SystemFlag.Default.Account);

     //       }
     //   }

     //   [TestMethod]
     //   public void ProductManager_GetUserProducts()
     //   {
     //       ProductManager m = new ProductManager(connectionKey);
     //       string name = Guid.NewGuid().ToString();
     //       string userUUID = Guid.NewGuid().ToString();
     //       Product p = new Product()
     //       {
     //           AccountUUID = "a",
     //           Name = name,
     //           DateCreated = DateTime.UtcNow,
     //           CategoryUUID = "ALPHA",
     //           CreatedBy = userUUID

     //       };
     //       Assert.AreEqual(m.Insert(p, false).Code, 200);
       
     //       p = new Product()
     //       {
     //           AccountUUID = "a",
     //           Name = Guid.NewGuid().ToString(),
     //           DateCreated = DateTime.UtcNow,
     //           CategoryUUID = "ALPHA",
     //           CreatedBy = userUUID
     //       };
     //       Assert.AreEqual(m.Insert(p, false).Code, 200);

     //       Assert.IsTrue(m.GetUserProducts("a", userUUID).Count >= 2);
     //   }

     //   [TestMethod]
     //   public void ProductManager_UpdateCategory()
     //   {
     //       ProductManager m = new ProductManager(connectionKey);
     //       string name = Guid.NewGuid().ToString();
     //       Product p = new Product()
     //       {
     //           AccountUUID = "a",
     //           Name = name,
     //           DateCreated = DateTime.UtcNow,
     //           CategoryUUID = "ALPHA" 
     //       };
     //       Assert.AreEqual(m.Insert(p, false).Code, 200);
     //       List<Product> products = new List<Product>();
     //       products.Add(p);
     //       p = new Product()
     //       {
     //           AccountUUID = "a",
     //           Name = Guid.NewGuid().ToString(),
     //           DateCreated = DateTime.UtcNow,
     //           CategoryUUID = "ALPHA"
     //       };
     //       Assert.AreEqual(m.Insert(p, false).Code, 200);
     //       products.Add(p);

     //       Assert.IsTrue(m.GetAll("ALPHA").Count >= 2);

     //       products.ForEach(x => x.CategoryUUID = "BETA");

     //       Assert.AreEqual(m.UpdateCategory(products).Code, 200);

     //       Assert.IsTrue(m.GetAll("BETA").Count >= 2);

     //   }

     //   [TestMethod]
     //   public void ProductManager_Insert_Product()
     //   {
     //       ProductManager m = new ProductManager(connectionKey);
     //       string name = Guid.NewGuid().ToString();
     //       Assert.AreEqual(
     //           m.Insert(new Product()
     //           {
     //               AccountUUID = "a",
     //               Name = name,
     //               DateCreated = DateTime.UtcNow
     //           }, false)
     //        .Code, 200);

     //       //won't allow a duplicate name
     //       Assert.AreEqual(
     //         m.Insert(new Product()
     //         {
     //             AccountUUID = "a",
     //             Name = name,
     //             DateCreated = DateTime.UtcNow
     //         })
     //      .Code, 500);

     //       Assert.IsNotNull(m.GetProduct(name));
     //   }

     //   [TestMethod]
     //   public void ProductManager_GetProduct()
     //   {
     //       ProductManager m = new ProductManager(connectionKey);
     //       string name = Guid.NewGuid().ToString();
     //       ServiceResult sr = m.Insert(new Product()
     //       {
     //           AccountUUID = "a",
     //           Name = "ALPHA" + name,
     //           DateCreated = DateTime.UtcNow
     //       }, false);

     //       Assert.AreEqual(sr.Code, 200, sr.Message);
     //       Product s = m.GetProduct("ALPHA" + name);
     //       Assert.IsNotNull(s);
     //   }

     //   [TestMethod]
     //   public void ProductManager_GetProductBy()
     //   {
     //       ProductManager m = new ProductManager(connectionKey);
     //       Assert.AreEqual(
     //         m.Insert(new Product()
     //         {
     //             AccountUUID = "a",
     //             Name = "TESTRECORD",
     //             DateCreated = DateTime.UtcNow
     //         }, false)
     //      .Code, 200);
     //       Product s = m.GetProduct("TESTRECORD");
     //       Assert.IsNotNull(s);
     //       Product suid = m.GetProductBy(s.UUID);
     //       Assert.IsNotNull(suid);
     //   }

     //   [TestMethod]
     //   public void ProductManager_GetAccountProducts()
     //   {
     //       ProductManager m = new ProductManager(connectionKey);
     //       Assert.IsTrue(m.GetAccountProducts("a").Count > 0);
     //   }

     //   [TestMethod]
     //   public void ProductManager_UpdateProduct()
     //   {
     //       ProductManager m = new ProductManager(connectionKey);
     //       m.Insert(new Product()
     //       {
     //           AccountUUID = "a",
     //           Name = "TESTRECORD",
     //       }, false);
     //       Product s = m.GetProduct("TESTRECORD");
     //       s.Name = "UPDATEDRECORD";

     //       Assert.AreEqual(m.UpdateProduct(s).Code, 200);
     //       Product u = m.GetProduct("UPDATEDRECORD");
     //       Assert.IsNotNull(u);
     //   }

     //   [TestMethod]
     //   public void ProductManager_DeleteProduct()
     //   {
     //       ProductManager m = new ProductManager(connectionKey);
     //       string name = Guid.NewGuid().ToString();
     //       Product s = new Product()
     //       {
     //           AccountUUID = "a",
     //           Name = name,
     //           CreatedBy = "TESTUSER",
     //           DateCreated = DateTime.UtcNow,
     //       };

     //       m.Insert(s, false);

     //       //Test the delete flag
     //       //Assert.IsTrue(m.DeleteProduct(s) > 0);
     //       //m.GetProduct("DELETERECORD");
     //       //Product d = m.GetProduct(name);
     //       //Assert.IsNotNull(d);
     //       //Assert.IsTrue(d.Deleted == true);

     //       //there is only purge
     //       Assert.IsTrue(m.DeleteProduct(s) > 0);
     //       Product d = m.GetProduct(name);
     //       Assert.IsNull(d);
     //   }
    }
}
