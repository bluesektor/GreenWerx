// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using TreeMon.Utilites.Extensions;
using TreeMon.Models.Membership;
using System.Linq;
using MoreLinq;

namespace TreeMon.Web.Tests._1_TreeMon.Utilities.Extentions
{
    [TestClass]
    public class ListExTests
    {
        [TestMethod]
        public void ListEX_Paginate_First_Test()
        {
            List<string> lst = new List<string>();
            lst.Add("a"); lst.Add("b"); lst.Add("c"); lst.Add("d");
            lst = lst.Paginate(0, 2);
            Assert.AreEqual(2, lst.Count);
            Assert.AreEqual("a", lst[0]);
        }

        [TestMethod]
        public void ListEX_Paginate_Last_Test()
        {
            List<string> lst = new List<string>();
            lst.Add("a"); lst.Add("b"); lst.Add("c"); lst.Add("d");
            lst = lst.Paginate(3, 2);
            Assert.AreEqual(2, lst.Count);
            Assert.AreEqual("c", lst[0]);
        }


        [TestMethod]
        public void ListEX_PaginatePage_First_Test()
        {
            List<string> lst = new List<string>();
            lst.Add("a"); lst.Add("b"); lst.Add("c"); lst.Add("d");
            lst = lst.PaginateByPage(0, 2);
            Assert.AreEqual(2, lst.Count);
            Assert.AreEqual("a", lst[0]);
        }

        [TestMethod]
        public void ListEX_PaginateByPage_Last_Test()
        {
            List<string> lst = new List<string>();
            lst.Add("a"); lst.Add("b"); lst.Add("c"); lst.Add("d");
            lst = lst.PaginateByPage(1, 2);
            Assert.AreEqual(2, lst.Count);
            Assert.AreEqual("c", lst[0]);
        }

        [TestMethod]
        public void ListEX_Distinct_Test()
        {
            List<User> lst = new List<User>();
            lst.Add(TestHelper.GenerateTestUser("alph")); lst.Add(TestHelper.GenerateTestUser("alph")); lst.Add(TestHelper.GenerateTestUser("zen")); lst.Add(TestHelper.GenerateTestUser("zen"));
            lst = lst.DistinctBy(x => x.Name).ToList();// nuget morelinq for distinctby
            Assert.AreEqual(2, lst.Count);
            Assert.AreEqual("alph", lst[0].Name);
        }
    }
}
