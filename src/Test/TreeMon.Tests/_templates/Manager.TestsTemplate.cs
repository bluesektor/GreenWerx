using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TreeMon.Data;
using TreeMon.Models.App;
using System.Collections.Generic;


namespace TreeMon.Web.Tests._templates
{
    [TestClass]
    public class CLASSNAME_Tests
    {
        private string connectionKey = "MSSQL_TEST";

        [TestMethod]
        public void CLASSNAME_Insert_MODELNAME()
        {
            // CLASSNAME m = new CLASSNAME(new TreeMonDbContext(connectionKey));
            //string name = Guid.NewGuid().ToString();
            // Assert.AreEqual(
            //     m.Insert(new MODELNAME()
            //     {
            //         AccountId = "a",
            //         Name =name,
            //         DateCreated = DateTime.UtcNow
            //     },false)
            //  .Code, 200);

            // //won't allow a duplicate name
            // Assert.AreEqual(
            //   m.Insert(new MODELNAME()
            //   {
            //       AccountId = "a",
            //       Name = name,
            //       DateCreated = DateTime.UtcNow
            //   })
            //.Code, 500);

            //      Assert.IsNotNull(m.GetMODELNAME(name));
        }

        [TestMethod]
        public void CLASSNAME_GetMODELNAME()
        {
            //CLASSNAME m = new CLASSNAME(new TreeMonDbContext(connectionKey));
            // string name = Guid.NewGuid().ToString();
            //ServiceResult sr = m.Insert(new MODELNAME()
            //{
            //    AccountId = "a",
            //    Name = "ALPHA" +  name,
            //    DateCreated = DateTime.UtcNow
            //}, false);

            //Assert.AreEqual(sr.Code, 200, sr.Message);
            //MODELNAME s = m.GetMODELNAME("ALPHA" +  name);
            //Assert.IsNotNull(s);
        }

        [TestMethod]
        public void CLASSNAME_GetMODELNAMEBy()
        {
            //CLASSNAME m = new CLASSNAME(new TreeMonDbContext(connectionKey));
           // Assert.AreEqual(
           //   m.Insert(new MODELNAME()
           //   {
           //       AccountId = "a",
           //       Name = "TESTRECORD",
           //       DateCreated = DateTime.UtcNow
           //   },false)
           //.Code, 200);
            //MODELNAME s = m.GetMODELNAME("TESTRECORD");
            //Assert.IsNotNull(s);
            //MODELNAME suid = m.GetMODELNAMEBy(s.UUID);
            //Assert.IsNotNull(suid);
        }

        [TestMethod]
        public void CLASSNAME_GetMODELNAMEs()
        {
            //CLASSNAME m = new CLASSNAME(new TreeMonDbContext(connectionKey));
            //Assert.IsTrue(m.GetMODELNAMEs("a").Count > 0);
        }

        [TestMethod]
        public void CLASSNAME_UpdateMODELNAME()
        {
        //    CLASSNAME m = new CLASSNAME(new TreeMonDbContext(connectionKey));
        //    m.Insert(new MODELNAME()
        //    {
        //        AccountId = "a",
        //        Name = "TESTRECORD",
        //    },false);
        //    MODELNAME s = m.GetMODELNAME("TESTRECORD");
        //    s.Name = "UPDATEDRECORD";

        //    Assert.AreEqual(m.UpdateMODELNAME(s).Code, 200);
        //    MODELNAME u = m.GetMODELNAME("UPDATEDRECORD");
        //    Assert.IsNotNull(u);
        }

        [TestMethod]
        public void CLASSNAME_DeleteMODELNAME()
        {
            //CLASSNAME m = new CLASSNAME(new TreeMonDbContext(connectionKey));
            //   string name = Guid.NewGuid().ToString();
            //MODELNAME s = new MODELNAME()
            //{
            //    AccountId = "a",
            //    Name = name,
            //    CreatedBy = "TESTUSER",
            //    DateCreated = DateTime.UtcNow,
            //};

            //m.Insert(s,false);

            ////Test the delete flag
            //Assert.IsTrue(m.DeleteMODELNAME(s) > 0);
            //m.GetMODELNAME("DELETERECORD");
            //MODELNAME d = m.GetMODELNAME(name);
            //Assert.IsNotNull(d);
            //Assert.IsTrue(d.Deleted == true);


            //Assert.IsTrue(m.DeleteMODELNAME(s, true) > 0);
            //d = m.GetMODELNAME(name);
            //Assert.IsNull(d);
        }
    }
}

