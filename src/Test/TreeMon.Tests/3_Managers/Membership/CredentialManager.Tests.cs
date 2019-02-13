// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using TreeMon.Data;
using TreeMon.Managers.Membership;
using TreeMon.Models.Membership;

namespace TreeMon.Web.Tests._templates
{
    [TestClass]
    public class CredentialManager_Tests
    {
        private string connectionKey = "MSSQL_TEST";

        //[TestMethod]
        //public void CredentialManager_Insert_Credential()
        //{
        //    CredentialManager m = new CredentialManager(connectionKey);

        //    Assert.AreEqual(
        //        m.Insert(new Credential()
        //        {
        //            AccountUUID = "a",
        //            Name = "TESTRECORD",
        //            DateCreated = DateTime.UtcNow
        //        }, false)
        //     .Code, 200);

        //    //won't allow a duplicate name
        //    Assert.AreEqual(
        //      m.Insert(new Credential()
        //      {
        //          AccountUUID = "a",
        //          Name = "TESTRECORD",
        //          DateCreated = DateTime.UtcNow
        //      })
        //   .Code, 500);
        //}
    }
}
