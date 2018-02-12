// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using TreeMon.Data;
using TreeMon.Data.Logging.Models;
using TreeMon.Models.App;
using TreeMon.Models.Store;

namespace TreeMon.Web.Tests.API.V1
{
    //CLASSNAME = Product

    [TestClass]
    public class ProductControllerTests //todo re-implement
    {
        ////private string connectionKey = "MSSQL_TEST";
        //private string _ownerAuthToken = "";
        //private string _captcha = "TESTCAPTCHA";

        //#region Template tests

        //[TestInitialize]
        //public void TestSetup()
        //{
        //    _ownerAuthToken = TestHelper.InitializeControllerTestData(connectionKey, "OWNER");
        //    Assert.IsNotNull(ConfigurationManager.AppSettings["DefaultDbConnection"]);
        //}

        //[TestMethod]
        //public void Api_ProductController_Add_Product()
        //{
        //    ProductForm mdl = new ProductForm();
        //    mdl.AccountUUID = SystemFlag.Default.Account;
        //    mdl.Name = Guid.NewGuid().ToString("N");
        //    mdl.CategoryUUID = "BOOTH";
        //    mdl.Captcha = _captcha;
                        
        //    string postData = "";
        //    JsonSerializerSettings settings = new JsonSerializerSettings();
        //    settings.Formatting = Formatting.Indented;
        //     postData = JsonConvert.SerializeObject(mdl, settings);
        //    Task.Run(async () =>
        //    {
        //        ServiceResult res = await TestHelper.SentHttpRequest("POST", "api/Products/Add", postData, _ownerAuthToken);
        //        Assert.IsNotNull(res);
        //        Assert.AreEqual(res.Code, 200);

        //        Product p = JsonConvert.DeserializeObject<Product>(res.Result.ToString());
        //        Assert.IsNotNull(p);
        //        TreeMonDbContext context = new TreeMonDbContext(connectionKey);
        //        Product dbProduct = context.GetAll<Product>().Where(w => w.UUID == p.UUID).FirstOrDefault();
        //        Assert.IsNotNull(dbProduct);
        //        Assert.AreEqual(mdl.Name, dbProduct.Name);

        //    }).GetAwaiter().GetResult();
          
        //}

        //[TestMethod]
        //public void Api_ProductController_Get_Product()
        //{
        //    TreeMonDbContext context = new TreeMonDbContext(connectionKey);
        //    Product mdl = new Product();
        //    mdl.AccountUUID = SystemFlag.Default.Account;
        //    mdl.Name = Guid.NewGuid().ToString("N");
        //    mdl.CategoryUUID = "BOOTH";
        //    Assert.IsTrue(context.Insert<Product>(mdl));

        //    Task.Run(async () =>
        //    {
        //        ServiceResult res = await  TestHelper.SentHttpRequest("GET", "api/Product/" + mdl.Name, "", _ownerAuthToken);

        //        Assert.IsNotNull(res);
        //        Assert.AreEqual(res.Code, 200);

        //        Product p = JsonConvert.DeserializeObject<Product>(res.Result.ToString());
        //        Assert.IsNotNull(p);
        //        Assert.AreEqual(mdl.Name, p.Name);

        //    }).GetAwaiter().GetResult();
        //}

        //[TestMethod]
        //public void Api_ProductController_Get_Products_()
        //{
        //    TreeMonDbContext context = new TreeMonDbContext(connectionKey);
        //    Product mdl = new Product();
        //    mdl.AccountUUID = SystemFlag.Default.Account;
        //    mdl.Name = Guid.NewGuid().ToString("N");
        //    mdl.CategoryUUID = "BOOTH";
        //    Assert.IsTrue(context.Insert<Product>(mdl));

        //    Product mdl2 = new Product();
        //    mdl2.AccountUUID = SystemFlag.Default.Account;
        //    mdl2.Name = Guid.NewGuid().ToString("N");
        //    mdl2.CategoryUUID = "BOOTH";
        //    Assert.IsTrue(context.Insert<Product>(mdl2));

        //    Task.Run(async () =>
        //    {
        //        ServiceResult res = await TestHelper.SentHttpRequest("POST", "api/Products/", "", _ownerAuthToken);

        //        Assert.IsNotNull(res);
        //        Assert.AreEqual(res.Code, 200);

        //        List<Product> products = JsonConvert.DeserializeObject<List<Product>>(res.Result.ToString());
        //        Assert.IsNotNull(products);
        //        Assert.IsTrue(products.Count >= 2);

        //        int foundProducts = 0;
        //        foreach (Product p in products)
        //        {
        //            if (p.Name == mdl.Name || p.Name == mdl2.Name)
        //                foundProducts++;
                 
        //        }

        //        Assert.AreEqual(foundProducts,2);

        //    }).GetAwaiter().GetResult();
        //}

        //[TestMethod]
        //public void Api_ProductController_Get_Products_ByCategory()
        //{
        //    TreeMonDbContext context = new TreeMonDbContext(connectionKey);
        //    Product mdl = new Product();
        //    mdl.AccountUUID = SystemFlag.Default.Account;
        //    mdl.Name = Guid.NewGuid().ToString("N");
        //    mdl.CategoryUUID = "BOOTH";
        //    Assert.IsTrue(context.Insert<Product>(mdl));

        //    Product mdl2 = new Product();
        //    mdl2.AccountUUID = SystemFlag.Default.Account;
        //    mdl2.Name = Guid.NewGuid().ToString("N");
        //    mdl2.CategoryUUID = "BOOTH";
        //    Assert.IsTrue(context.Insert<Product>(mdl2));

        //    Task.Run(async () =>
        //    {
        //        ServiceResult res = await TestHelper.SentHttpRequest("POST", "api/Products/Categories/" + mdl.CategoryUUID, "", _ownerAuthToken);

        //        Assert.IsNotNull(res);
        //        Assert.AreEqual(res.Code, 200);

        //        List<Product> products = JsonConvert.DeserializeObject<List<Product>>(res.Result.ToString());
        //        Assert.IsNotNull(products );
        //        Assert.IsTrue(products.Count >= 2);

        //        int foundProducts = 0;
        //        foreach (Product p in products)
        //        {
        //            if (p.Name == mdl.Name || p.Name == mdl2.Name)
        //                foundProducts++;
        //        }

        //        Assert.AreEqual(foundProducts, 2);

        //    }).GetAwaiter().GetResult();
        //}

        //[TestMethod]
        //public void Api_ProductController_Delete_Product()
        //{
        //    TreeMonDbContext context = new TreeMonDbContext(connectionKey);
        //    Product mdl = new Product();
        //    mdl.AccountUUID = SystemFlag.Default.Account;
        //    mdl.Name = Guid.NewGuid().ToString("N");
        //    mdl.CategoryUUID = "BOOTH";
        //    Assert.IsTrue(context.Insert<Product>(mdl));
        //    string postData = "";
        //    JsonSerializerSettings settings = new JsonSerializerSettings();
        //    settings.Formatting = Formatting.Indented;
        //    postData = JsonConvert.SerializeObject(mdl, settings);

        //    Task.Run(async () =>
        //    {
        //        ServiceResult res = await TestHelper.SentHttpRequest("POST", "api/Products/Delete", postData, _ownerAuthToken);

        //        Assert.IsNotNull(res);
        //        Assert.AreEqual(res.Code, 200);

        //        Product dbProduct = context.GetAll<Product>().Where(w => w.Name == mdl.Name).FirstOrDefault();
        //        Assert.IsNull(dbProduct);

        //    }).GetAwaiter().GetResult();
        //}

        //[TestMethod]
        //public void Api_ProductController_Update_Product()
        //{
        //    TreeMonDbContext context = new TreeMonDbContext(connectionKey);
        //    Product mdl = new Product();
        //    mdl.AccountUUID   = SystemFlag.Default.Account;
        //    mdl.Name        = Guid.NewGuid().ToString("N");
        //    mdl.CategoryUUID = "BOOTH";
        //    Assert.IsTrue(context.Insert<Product>(mdl));
        //    mdl = context.GetAll<Product>().Where(w => w.Name == mdl.Name).FirstOrDefault(); 
        //    ProductForm pv = new ProductForm();
        //    pv.UUID = mdl.UUID;
        //    pv.AccountUUID=mdl.AccountUUID;
        //    pv.Name =mdl.Name;
        //    pv.CategoryUUID = "MISC";
        //    pv.Cost = new Random().Next();
        //    //~~~ Updatable fields ~~~
        //    //dbP.Category = p.Category;
        //    //dbP.Name = p.Name;
        //    //dbP.Cost = p.Cost;
        //    //dbP.Price = p.Price;
        //    //dbP.Weight = p.Weight;
        //    //dbP.WeightUOM = p.WeightUOM;
        //    string postData = JsonConvert.SerializeObject(pv);
        //    Task.Run(async () =>
        //    {
        //        ServiceResult res = await TestHelper.SentHttpRequest("POST", "api/Products/Update", postData, _ownerAuthToken);
        //        Assert.IsNotNull(res);
        //        Assert.AreEqual(res.Code, 200);

        //        Product dbProduct = context.GetAll<Product>().Where(w => w.Name == mdl.Name).FirstOrDefault();
        //        Assert.IsNotNull(dbProduct);
        //        Assert.AreEqual(pv.Cost, dbProduct.Cost);
        //        Assert.AreEqual(pv.CategoryUUID, dbProduct.CategoryUUID);

        //    }).GetAwaiter().GetResult();
         
        //}

        //#endregion

        ////[TestMethod]
        ////public void Api_ProductController_SearchProducts()
        ////{
        ////    string filters = "[{ \"SearchTerm\" :\"BOOTH\" , \"SearchBy\": \"CATEGORY\", \"ReturnFormat\":\"select2\" }]";

        ////    TreeMonDbContext context = new TreeMonDbContext(connectionKey);
        ////    Product mdl = new Product();
        ////    mdl.AccountUUID = SystemFlag.Default.Account;
        ////    mdl.Name = Guid.NewGuid().ToString("N");
        ////    mdl.CategoryUUID = "BOOTH";
        ////    Assert.IsTrue(context.Insert<Product>(mdl));

        ////    Product mdl2 = new Product();
        ////    mdl2.AccountUUID = SystemFlag.Default.Account;
        ////    mdl2.Name = Guid.NewGuid().ToString("N");
        ////    mdl2.CategoryUUID = "BOOTH";
        ////    Assert.IsTrue(context.Insert<Product>(mdl2));

        ////    Task.Run(async () =>
        ////    {
        ////        ServiceResult res = await TestHelper.SentHttpRequest("get", "api/Products/?filter=" + filters, "", _ownerAuthToken);

        ////        Assert.IsNotNull(res);
        ////        Assert.AreEqual(res.Code, 200);
        ////        string records = res.Result.ToString();
        ////        List<Select2Result> selRes = JsonConvert.DeserializeObject<List<Select2Result>>(res.Result.ToString());
        ////        Assert.IsNotNull(selRes);
        ////        Assert.IsTrue(selRes.Count >= 2);
        ////    }).GetAwaiter().GetResult();
        ////}


        //[TestMethod]
        //public void Api_ProductController_MoveProductsToCategory()
        //{

        //    TreeMonDbContext context = new TreeMonDbContext(connectionKey);
        //    Product mdl = new Product();
        //    mdl.AccountUUID = SystemFlag.Default.Account;
        //    mdl.Name = Guid.NewGuid().ToString("N");
        //    mdl.CategoryUUID = "BOOTH";
        //    Assert.IsTrue(context.Insert<Product>(mdl));

        //    Product mdl2 = new Product();
        //    mdl2.AccountUUID = SystemFlag.Default.Account;
        //    mdl2.Name = Guid.NewGuid().ToString("N");
        //    mdl2.CategoryUUID = "BOOTH";
        //    Assert.IsTrue(context.Insert<Product>(mdl2));

        //    mdl.CategoryUUID = "MISC";
        //    mdl2.CategoryUUID = "GLASS";
        //    List<Product> products = new List<Product>();
        //    products.Add(mdl);
        //    products.Add(mdl2);
        //    string postData = JsonConvert.SerializeObject(products);

        //    Task.Run(async () =>
        //    {
        //        ServiceResult res = await TestHelper.SentHttpRequest("POST", "api/Products/Categories/Move", postData, _ownerAuthToken);

        //        Assert.IsNotNull(res);
        //        Assert.AreEqual(res.Code, 200);

        //        Product dbProduct = context.GetAll<Product>().Where(w => w.Name == mdl.Name).FirstOrDefault();
        //        Assert.IsNotNull(dbProduct);
        //        Assert.AreEqual(mdl.CategoryUUID, dbProduct.CategoryUUID);

        //        dbProduct = context.GetAll<Product>().Where(w => w.Name == mdl2.Name).FirstOrDefault();
        //        Assert.AreEqual(mdl2.CategoryUUID, dbProduct.CategoryUUID);

        //    }).GetAwaiter().GetResult();
        //}

        //[TestMethod]
        //public void Api_ProductController_GetProductCategories()
        //{
        //    TreeMonDbContext context = new TreeMonDbContext(connectionKey);
        //    Product mdl = new Product();
        //    mdl.AccountUUID = SystemFlag.Default.Account;
        //    mdl.Name = Guid.NewGuid().ToString("N");
        //    mdl.CategoryUUID = "BOOTH";
        //    Assert.IsTrue(context.Insert<Product>(mdl));

        //    Product mdl2 = new Product();
        //    mdl2.AccountUUID = SystemFlag.Default.Account;
        //    mdl2.Name = Guid.NewGuid().ToString("N");
        //    mdl2.CategoryUUID = "MISC";
        //    Assert.IsTrue(context.Insert<Product>(mdl2));

        //    Task.Run(async () =>
        //    {
        //        ServiceResult res = await TestHelper.SentHttpRequest("POST", "api/Products/Categories/", "", _ownerAuthToken);

        //        Assert.IsNotNull(res);
        //        Assert.AreEqual(res.Code, 200);

        //        List<Product> products = JsonConvert.DeserializeObject<List<Product>>(res.Result.ToString());
        //        Assert.IsNotNull(products);
        //        Assert.IsTrue(products.Count >= 2);

        //        int foundProducts = 0;
        //        foreach (Product p in products)
        //        {
        //            if (p.CategoryUUID == mdl.CategoryUUID || p.CategoryUUID == mdl2.CategoryUUID)
        //                foundProducts++;

        //        }

        //        Assert.AreEqual(foundProducts, 2);

        //    }).GetAwaiter().GetResult();
        //}

    }
}
