// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TreeMon.Utilites.Helpers;
using System.IO;

namespace TreeMon.Web.Tests._1_TreeMon.Utilities.Helpers
{
    [TestClass]
    public class EnvironmentExTests
    {
        [TestMethod]
        public void EnvironmentEx_AppDataFolder()
        {
            string directory = EnvironmentEx.AppDataFolder;
            Assert.IsTrue(directory.EndsWith("App_Data"));
          //  Assert.IsTrue(Directory.Exists(directory));
        }
    }
}
