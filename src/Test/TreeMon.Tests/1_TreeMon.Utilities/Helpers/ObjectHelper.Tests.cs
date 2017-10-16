// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TreeMon.Models.Membership;
using TreeMon.Utilites.Helpers;

namespace TreeMon.Web.Tests._1_TreeMon.Utilities.Helpers
{
    [TestClass]
    public class ObjectHelperTests
    {
        [TestMethod]
        public void ObjectHelper_GetPropertyValue()
        {
            User u = new User();
            u.Name = "fname";
            u.Email = "test@unit.com";
            u.AccountUUID = "232323";
            Assert.AreEqual("fname", ObjectHelper.GetPropertyValue("Name",u));
        }
    }
}
