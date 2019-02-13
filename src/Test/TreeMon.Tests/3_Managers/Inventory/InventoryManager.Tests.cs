// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TreeMon.Managers.Inventory;
using TreeMon.Managers.Membership;
using TreeMon.Models.App;
using TreeMon.Models.Inventory;
using TreeMon.Web.Tests;
using TreeMon.WebAPI.api.v1;

namespace TreeMon.Tests._3_Managers.Inventory
{
    [TestClass]
    public class InventoryManager_Tests
    {
        private string connectionKey = "dev_drone";
        private string sessionKey = "TESTSESSION";
        private string _ownerAuthToken = "";

        [TestInitialize]
        public void TestSetup()
        {
            _ownerAuthToken = TestHelper.InitializeControllerTestData(connectionKey, "OWNER").Result.ToString();

        }

        [TestMethod]
        public void InventoryManager_GeneratePosts()
        {
            var rand = new Random();
            InventoryManager inventoryManager = new InventoryManager(connectionKey, _ownerAuthToken);
            //UserManager userManager = new UserManager(connectionKey, _ownerAuthToken);
            //var users = userManager.GetUsers(false);

            string pathToImages = "C:\\Users\\Home\\Downloads\\img";
            string[] images = Directory.GetFiles(pathToImages,"*.*", SearchOption.AllDirectories);
            foreach (string file in images)
            {
                FileInfo fi = new FileInfo(file);

                if (fi.Extension.ToLower() != ".jpg" && fi.Extension.ToLower() != ".png" && fi.Extension.ToLower() != ".jpeg")
                    continue;

                var fileName = DateTime.UtcNow.ToString("yyyyMMdd_hhmmss") + fi.Name;
                try
                {
                    fi.CopyTo("C:\\_Backup\\Dev\\Projects\\OperationPinnappleExpress\\Main\\Services\\TreeMon.WebAPI\\TreeMon.WebAPI\\Content\\Uploads\\6b99b60fbdd5455fa93784b93b274e49\\" + fileName);

                    InventoryItem item = new InventoryItem()
                    {
                        AccountUUID = "d255fa6dd9e64ac3a2322e3f1da3a321",
                        Active = true,
                        CreatedBy = "6b99b60fbdd5455fa93784b93b274e49",
                        CategoryUUID = "PRODUCT.VAPORIZER",
                        Condition = "new",
                        Cost = rand.Next(1, 20000),
                        DateCreated = DateTime.UtcNow,
                        Deleted = false,
                        LocationType = "coordinate",
                        LocationUUID = "56bc0308088b4bae8b817614b3aceb15",
                        ItemDate = DateTime.UtcNow,
                        Price = rand.Next(1, 20000),
                        RoleWeight = 0,
                        RoleOperation = ">=",
                        Name = fi.Name,
                        Description = fi.Name + fi.LastAccessTime + fi.Directory,
                        //   thats the user uuid under uploads
                        Image = "https://localhost:44318/Content/Uploads/6b99b60fbdd5455fa93784b93b274e49/" + fileName

                    };
                    ServiceResult res = inventoryManager.Insert(item);
                    if (res.Code != 200)
                        Debug.Assert(false, res.Message);
                  
                    //copy image to content folder
                    //   C:\_Backup\Dev\Projects\OperationPinnappleExpress\Main\Services\TreeMon.WebAPI\TreeMon.WebAPI\Content\Uploads


                    /*


                     */
                    //  InventoryController ctlr = new InventoryController();
                    // ctlr.Insert
                    // ctlr.PublishItem
                    // ctlr.PostFile  need to recreate file url
                    //  set item.Image to https://localhost:44318/Content/Uploads/fb722aaae0834dc09d22087bdfdbd4b4/20180701_052847mouse.jpg
                }
                catch(Exception ex)
                {
                    Debug.Assert(false, ex.Message);
                    // No need to log this because the file could still be locked.
                }
            }

        }

        [TestMethod]
        public void InventoryManager_GetItems()
        {
            InventoryManager im = new InventoryManager(connectionKey, _ownerAuthToken);
            List<InventoryItem> items =  im.GetItems("85395", 25);

            Assert.IsTrue(items.Count > 0);

            //--inventory uuid							location uuid
            //--dc3d9ff52f8e44f592169c9300158af5			56bc0308088b4bae8b817614b3aceb15		85395   same location
            //--6ae6baa3a6684e519ee7099f2b3ab72e			3a0250074a284e4d9d5b27327d3eff19		85027	dear valley same state within distance
            //--1c26f3b24ba645b1bf0971dd8d1616fb			4838285e19434d8c9f912bedd120b05b     86003   flagstaff same state out of distance
            //--21135a78feed4effaf0eb278f92c75d6			f77d52a3f05e4680a56f18f121d26c64		80002	 denver co. different state out of distance

            //LocationManager lm = new LocationManager(connectionKey, _ownerAuthToken);

            //List<Location> locations = lm.Search("85395", "coordinate");

            //Assert.IsTrue(locations.Count > 0);

            //var coord = locations.FirstOrDefault();
            //Assert.IsNotNull(coord.Latitude);
            //Assert.IsNotNull(coord.Longitude);
        }

    }
}
