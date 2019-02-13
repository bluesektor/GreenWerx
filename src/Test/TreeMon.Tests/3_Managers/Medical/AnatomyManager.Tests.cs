// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TreeMon.Data;
using TreeMon.Models.App;
using TreeMon.Models.Medical;
using System.Collections.Generic;

namespace TreeMon.Web.Tests._templates
{
    [TestClass]
    public class AnatomyManager_Tests
    {
        private string connectionKey = "MSSQL_TEST";

    //    [TestMethod]
    //    public void AnatomyManager_Insert()
    //    {
    //        AnatomyManager m = new AnatomyManager(connectionKey);

    //        Assert.AreEqual(
    //            m.Insert(new Anatomy()
    //            {
    //                AccountUUID = "a",
    //                Name = "TESTRECORD",
    //                DateCreated = DateTime.UtcNow
    //            })
    //         .Code, 200);

    //        //won't allow a duplicate name
    //        Assert.AreEqual(
    //          m.Insert(new Anatomy()
    //          {
    //              AccountUUID = "a",
    //              Name = "TESTRECORD",
    //              DateCreated = DateTime.UtcNow
    //          })
    //       .Code, 500);
    //    }

    //    [TestMethod]
    //    public void AnatomyManager_GetAnatomy()
    //    {
    //        AnatomyManager m = new AnatomyManager(connectionKey);
    //        ServiceResult sr = m.Insert(new Anatomy()
    //        {
    //            AccountUUID = "a",
    //            Name = "ALPHA",
    //            DateCreated = DateTime.UtcNow
    //        }, false);

    //        Assert.AreEqual(sr.Code, 200, sr.Message);
    //        Anatomy s = m.GetAnatomy("ALPHA");
    //        Assert.IsNotNull(s);
    //    }

    //    [TestMethod]
    //    public void AnatomyManager_GetAnatomyBy()
    //    {
    //        AnatomyManager m = new AnatomyManager(connectionKey);
    //        Anatomy s = m.GetAnatomy("TESTRECORD");
    //        Assert.IsNotNull(s);
    //        Anatomy suid = m.GetAnatomyBy(s.UUID);
    //        Assert.IsNotNull(suid);
    //    }

    //    [TestMethod]
    //    public void AnatomyManager_GetAnatomys()
    //    {
    //        AnatomyManager m = new AnatomyManager(connectionKey);
    //        Assert.IsTrue(m.GetAnatomies("a").Count > 0);
    //    }

    //    [TestMethod]
    //    public void AnatomyManager_UpdateAnatomy()
    //    {
    //        AnatomyManager m = new AnatomyManager(connectionKey);
    //        m.Insert(new Anatomy()
    //        {
    //            AccountUUID = "a",
    //            Name = "TESTRECORD",
    //            UUID = Guid.NewGuid().ToString("N")
    //        });
    //        Anatomy s = m.GetAnatomy("TESTRECORD");
    //        s.Name = "UPDATEDRECORD";

    //        Assert.AreEqual(m.UpdateAnatomy(s).Code, 200);
    //        Anatomy u = m.GetAnatomy("UPDATEDRECORD");
    //        Assert.IsNotNull(u);
    //    }


    //    [TestMethod]
    //    public void AnatomyManager_DeleteAnatomy()
    //    {
    //        AnatomyManager m = new AnatomyManager(connectionKey);
    //        Anatomy s = new Anatomy()
    //        {
    //            AccountUUID = "a",
    //            Name = "DELETERECORD",
    //            CreatedBy = "TESTUSER",
    //            DateCreated = DateTime.UtcNow
    //        };

    //        m.Insert(s);

    //        //Test the delete flag
    //        Assert.IsTrue(m.DeleteAnatomy(s) > 0);
    //        m.GetAnatomy("DELETERECORD");
    //        Anatomy d = m.GetAnatomy("DELETERECORD");
    //        Assert.IsNotNull(d);
    //        Assert.IsTrue(d.Deleted == true);


    //        Assert.IsTrue(m.DeleteAnatomy(s, true) > 0);
    //        d = m.GetAnatomy("DELETERECORD");
    //        Assert.IsNull(d);
    //    }


    //    [TestMethod]
    //    public void AnatomyManager_Insert_AnatomyTag()
    //    {
    //        AnatomyManager m = new AnatomyManager(connectionKey);

    //        Assert.AreEqual(
                
    //            m.Insert(new AnatomyTag()
    //            {
    //                AccountUUID = "a",
    //                Name = "TESTRECORD",
    //                DateCreated = DateTime.UtcNow
    //            })
    //         .Code, 200);

    //        //won't allow a duplicate name
    //        Assert.AreEqual(
    //          m.Insert(new AnatomyTag()
    //          {
    //              AccountUUID = "a",
    //              Name = "TESTRECORD",
    //              DateCreated = DateTime.UtcNow
    //          })
    //       .Code, 500);
    //    }

    //    [TestMethod]
    //    public void AnatomyManager_GetAnatomyTag()
    //    {
       
    //    AnatomyManager m = new AnatomyManager(connectionKey);
    //    ServiceResult sr = m.Insert(new AnatomyTag()
    //    {
    //        AccountUUID = "a",
    //        Name = "ALPHA",
    //        UUID = Guid.NewGuid().ToString("N"),
    //        DateCreated = DateTime.UtcNow
    //    }, false);

    //    Assert.AreEqual(sr.Code, 200, sr.Message);
    //    AnatomyTag s = m.GetAnatomyTag("ALPHA");
    //    Assert.IsNotNull(s);
    //}

    //    [TestMethod]
    //    public void AnatomyManager_GetAnatomyTagBy()
    //    {
    //        AnatomyManager m = new AnatomyManager(connectionKey);
    //        ServiceResult sr = m.Insert(new AnatomyTag()
    //        {
    //            AccountUUID = "a",
    //            Name = "TESTRECORD",
    //            DateCreated = DateTime.UtcNow
    //        }, false);

    //        AnatomyTag s = m.GetAnatomyTag("TESTRECORD");
    //        Assert.IsNotNull(s);
    //        AnatomyTag suid = m.GetAnatomyTagBy(s.UUID);
    //        Assert.IsNotNull(suid);
    //    }

    //    [TestMethod]
    //    public void AnatomyManager_GetAnatomyTags()
    //    {
    //        AnatomyManager m = new AnatomyManager(connectionKey);
    //        Assert.IsTrue(m.GetAnatomyTags("a").Count > 0);
    //    }

    //    [TestMethod]
    //    public void AnatomyManager_UpdateAnatomyTag()
    //    {
    //        AnatomyManager m = new AnatomyManager(connectionKey);
    //        m.Insert(new AnatomyTag()
    //        {
    //            AccountUUID = "a",
    //            Name = "TESTRECORD",
    //            UUID = Guid.NewGuid().ToString("N")
    //        });
    //        AnatomyTag s = m.GetAnatomyTag("TESTRECORD");
    //        s.Name = "UPDATEDRECORD";

    //        Assert.AreEqual(m.UpdateAnatomyTag(s).Code, 200);
    //        AnatomyTag u = m.GetAnatomyTag("UPDATEDRECORD");
    //        Assert.IsNotNull(u);
    //    }

    //    [TestMethod]
    //    public void AnatomyManager_DeleteAnatomyTag()
    //    {
    //        AnatomyManager m = new AnatomyManager(connectionKey);
    //        AnatomyTag s = new AnatomyTag()
    //        {
    //            AccountUUID = "a",
    //            Name = "DELETERECORD",
    //            CreatedBy = "TESTUSER",
    //            DateCreated = DateTime.UtcNow
               
    //        };

    //        m.Insert(s);

    //        //Test the delete flag
    //        Assert.IsTrue(m.DeleteAnatomyTag(s) > 0);
    //        m.GetAnatomyTag("DELETERECORD");
    //        AnatomyTag d = m.GetAnatomyTag("DELETERECORD");
    //        Assert.IsNotNull(d);
    //        Assert.IsTrue(d.Deleted == true);


    //        Assert.IsTrue(m.DeleteAnatomyTag(s, true) > 0);
    //        d = m.GetAnatomyTag("DELETERECORD");
    //        Assert.IsNull(d);
    //    }
    }
}
