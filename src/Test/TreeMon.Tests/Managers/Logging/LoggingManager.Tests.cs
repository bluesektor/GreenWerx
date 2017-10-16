using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TreeMon.Data;
using TreeMon.Models.App;

namespace TreeMon.Web.Tests._templates
{
    [TestClass]
    public class LoggingManager_Tests
    {
        private string connectionKey = "MSSQL_TEST";

        //[TestMethod]
        //public void LoggingManager_Insert()
        //{
        //   // LoggingManager m = new LoggingManager(new TreeMonDbContext(connectionKey));

        //   // Assert.AreEqual(
        //   //     m.Insert(new MODELNAME()
        //   //     {
        //   //         AccountId = "a",
        //   //         Name = "TESTRECORD",
        //   //         DateCreated = DateTime.UtcNow
        //   //     })
        //   //  .Code, 200);

        //   // //won't allow a duplicate name
        //   // Assert.AreEqual(
        //   //   m.Insert(new MODELNAME()
        //   //   {
        //   //       AccountId = "a",
        //   //       Name = "TESTRECORD",
        //   //       DateCreated = DateTime.UtcNow
        //   //   })
        //   //.Code, 500);
        //}

        //[TestMethod]
        //public void LoggingManager_GetMODELNAME()
        //{
        //    //LoggingManager m = new LoggingManager(new TreeMonDbContext(connectionKey));
        //    //ServiceResult sr = m.Insert(new MODELNAME()
        //    //{
        //    //    AccountId = "a",
        //    //    Name = "ALPHA",
        //    //    UUID = Guid.NewGuid().ToString("N"),
        //    //    DateCreated = DateTime.UtcNow
        //    //}, false);

        //    //Assert.AreEqual(sr.Code, 200, sr.Message);
        //    //MODELNAME s = m.GetMODELNAME("ALPHA");
        //    //Assert.IsNotNull(s);
        //}

        //[TestMethod]
        //public void LoggingManager_GetMODELNAMEBy()
        //{
        //    //LoggingManager m = new LoggingManager(new TreeMonDbContext(connectionKey));
        //    //MODELNAME s = m.GetMODELNAME("TESTRECORD");
        //    //Assert.IsNotNull(s);
        //    //MODELNAME suid = m.GetMODELNAMEBy(s.UUID);
        //    //Assert.IsNotNull(suid);
        //}

        //[TestMethod]
        //public void LoggingManager_GetMODELNAMEs()
        //{
        //    //LoggingManager m = new LoggingManager(new TreeMonDbContext(connectionKey));
        //    //Assert.IsTrue(m.GetMODELNAMEs("a").Count > 0);
        //}

        //[TestMethod]
        //public void LoggingManager_UpdateMODELNAME()
        //{
        ////    LoggingManager m = new LoggingManager(new TreeMonDbContext(connectionKey));
        ////    m.Insert(new MODELNAME()
        ////    {
        ////        AccountId = "a",
        ////        Name = "TESTRECORD",
        ////        UUID = Guid.NewGuid().ToString("N")
        ////    });
        ////    MODELNAME s = m.GetMODELNAME("TESTRECORD");
        ////    s.Name = "UPDATEDRECORD";

        ////    Assert.AreEqual(m.UpdateMODELNAME(s).Code, 200);
        ////    MODELNAME u = m.GetMODELNAME("UPDATEDRECORD");
        ////    Assert.IsNotNull(u);
        //}

        //[TestMethod]
        //public void LoggingManager_GetStatusByType()
        //{
        //    //LoggingManager m = new LoggingManager(new TreeMonDbContext(connectionKey));
        //    //m.Insert(new MODELNAME()
        //    //{
        //    //    AccountId = "a",
        //    //    Name = "RECORD_TYPE",
        //    //    CreatedBy = "TESTUSER",
        //    //    StatusType = "STATUSTYPE",
        //    //    DateCreated = DateTime.UtcNow
        //    //});

        //    //List<MODELNAME> s = m.GetStatusByType("STATUSTYPE", "TESTUSER", "a");
        //    //Assert.IsTrue(s.Count > 0);
        //}

        //[TestMethod]
        //public void LoggingManager_DeleteMODELNAME()
        //{
        //    //LoggingManager m = new LoggingManager(new TreeMonDbContext(connectionKey));
        //    //MODELNAME s = new MODELNAME()
        //    //{
        //    //    AccountId = "a",
        //    //    Name = "DELETERECORD",
        //    //    CreatedBy = "TESTUSER",
        //    //    DateCreated = DateTime.UtcNow,
        //    //    StatusType = "SymptomLog"
        //    //};

        //    //m.Insert(s);

        //    ////Test the delete flag
        //    //Assert.IsTrue(m.DeleteMODELNAME(s) > 0);
        //    //m.GetMODELNAME("DELETERECORD");
        //    //MODELNAME d = m.GetMODELNAME("DELETERECORD");
        //    //Assert.IsNotNull(d);
        //    //Assert.IsTrue(d.Deleted == true);


        //    //Assert.IsTrue(m.DeleteMODELNAME(s, true) > 0);
        //    //d = m.GetMODELNAME("DELETERECORD");
        //    //Assert.IsNull(d);
        //}
    }
}
