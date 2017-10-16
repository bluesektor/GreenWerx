using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TreeMon.Data;
using TreeMon.Models.App;
using System.Collections.Generic;
using TreeMon.Managers.Medical;
using TreeMon.Models.Medical;

namespace TreeMon.Web.Tests._templates
{
    [TestClass]
    public class SymptomManager_Tests
    {
        private string connectionKey = "MSSQL_TEST";


        #region Symptoms
        [TestMethod]
        public void SymptomManager_Insert_Symptom()
        {
            SymptomManager m = new SymptomManager(new TreeMonDbContext(connectionKey));

            Assert.AreEqual(
                m.Insert(new Symptom()
                {
                    AccountId = "a",
                    Name = "TESTRECORD",
                    DateCreated = DateTime.UtcNow
                }, false)
             .Code, 200);

            //won't allow a duplicate name
            Assert.AreEqual(
              m.Insert(new Symptom()
              {
                  AccountId = "a",
                  Name = "TESTRECORD",
                  DateCreated = DateTime.UtcNow
              })
           .Code, 500);
        }

        [TestMethod]
        public void SymptomManager_GetSymptom()
        {
            SymptomManager m = new SymptomManager(new TreeMonDbContext(connectionKey));
            ServiceResult sr = m.Insert(new Symptom()
            {
                AccountId = "a",
                Name = "ALPHA",
                DateCreated = DateTime.UtcNow
            }, false);

            Assert.AreEqual(sr.Code, 200, sr.Message);
            Symptom s = m.GetSymptom("ALPHA");
            Assert.IsNotNull(s);
        }

        [TestMethod]
        public void SymptomManager_GetSymptomBy()
        {
            SymptomManager m = new SymptomManager(new TreeMonDbContext(connectionKey));
            Assert.AreEqual(
              m.Insert(new Symptom()
              {
                  AccountId = "a",
                  Name = "TESTRECORD",
                  DateCreated = DateTime.UtcNow
              }, false)
           .Code, 200);
            Symptom s = m.GetSymptom("TESTRECORD");
            Assert.IsNotNull(s);
            Symptom suid = m.GetSymptomBy(s.UUID);
            Assert.IsNotNull(suid);
        }

        [TestMethod]
        public void SymptomManager_GetSymptoms()
        {
            SymptomManager m = new SymptomManager(new TreeMonDbContext(connectionKey));
            Assert.IsTrue(m.GetSymptoms("a").Count > 0);
        }

        [TestMethod]
        public void SymptomManager_UpdateSymptom()
        {
            SymptomManager m = new SymptomManager(new TreeMonDbContext(connectionKey));
            m.Insert(new Symptom()
            {
                AccountId = "a",
                Name = "TESTRECORD",
            });
            Symptom s = m.GetSymptom("TESTRECORD");
            s.Name = "UPDATEDRECORD";

            Assert.AreEqual(m.UpdateSymptom(s).Code, 200);
            Symptom u = m.GetSymptom("UPDATEDRECORD");
            Assert.IsNotNull(u);
        }

        [TestMethod]
        public void SymptomManager_DeleteSymptom()
        {
            SymptomManager m = new SymptomManager(new TreeMonDbContext(connectionKey));
            Symptom s = new Symptom()
            {
                AccountId = "a",
                Name = "DELETERECORD",
                CreatedBy = "TESTUSER",
                DateCreated = DateTime.UtcNow,
            };

            m.Insert(s);

            //Test the delete flag
            Assert.IsTrue(m.DeleteSymptom(s) > 0);
            m.GetSymptom("DELETERECORD");
            Symptom d = m.GetSymptom("DELETERECORD");
            Assert.IsNotNull(d);
            Assert.IsTrue(d.Deleted == true);


            Assert.IsTrue(m.DeleteSymptom(s, true) > 0);
            d = m.GetSymptom("DELETERECORD");
            Assert.IsNull(d);
        }

        #endregion

        #region SymptomsLog

        [TestMethod]
        public void SymptomManager_Insert_SymptomLog()
        {
            SymptomManager m = new SymptomManager(new TreeMonDbContext(connectionKey));

            Assert.AreEqual(
                m.InsertLog(new SymptomLog()
                {
                    AccountId = "a",
                    Name = "TESTRECORD",
                    DateCreated = DateTime.UtcNow
                    , DoseUUID = "D"
                }, false)
             .Code, 200);

            //won't allow a duplicate name
            Assert.AreEqual(
              m.InsertLog(new SymptomLog()
              {
                  AccountId = "a",
                  Name = "TESTRECORD",
                  DateCreated = DateTime.UtcNow
                  ,
                  DoseUUID = "D"
              })
           .Code, 500);
        }

        [TestMethod]
        public void SymptomManager_GetSymptomLog()
        {
            SymptomManager m = new SymptomManager(new TreeMonDbContext(connectionKey));
            ServiceResult sr = m.InsertLog(new SymptomLog()
            {
                AccountId = "a",
                Name = "ALPHA",
                DateCreated = DateTime.UtcNow
                ,
                DoseUUID = "D"
            }, false);

            Assert.AreEqual(sr.Code, 200, sr.Message);
            SymptomLog s = m.GetSymptomLog("ALPHA");
            Assert.IsNotNull(s);
        }

        [TestMethod]
        public void SymptomManager_GetSymptomLogBy()
        {
            SymptomManager m = new SymptomManager(new TreeMonDbContext(connectionKey));
            Assert.AreEqual(
              m.InsertLog(new SymptomLog()
              {
                  AccountId = "a",
                  Name = "TESTRECORD",
                  DateCreated = DateTime.UtcNow
                  ,
                  DoseUUID = "D"
              }, false)
           .Code, 200);
            SymptomLog s = m.GetSymptomLog("TESTRECORD");
            Assert.IsNotNull(s);
            SymptomLog suid = m.GetSymptomLogBy(s.UUID);
            Assert.IsNotNull(suid);
        }

        [TestMethod]
        public void SymptomManager_GetSymptomLogs()
        {
            SymptomManager m = new SymptomManager(new TreeMonDbContext(connectionKey));
            List<SymptomLog> logs = m.GetSymptomsLog("a");
            Assert.IsTrue(logs.Count > 0);
        }

        [TestMethod]
        public void SymptomManager_UpdateSymptomLog()
        {
            SymptomManager m = new SymptomManager(new TreeMonDbContext(connectionKey));
            m.InsertLog(new SymptomLog()
            {
                AccountId = "a",
                Name = "TESTRECORD",                
                DoseUUID = "D"
            });
            SymptomLog s = m.GetSymptomLog("TESTRECORD");
            s.Name = "UPDATEDRECORD";

            Assert.AreEqual(m.UpdateSymptomLog(s).Code, 200);
            SymptomLog u = m.GetSymptomLog("UPDATEDRECORD");
            Assert.IsNotNull(u);
        }

        [TestMethod]
        public void SymptomManager_DeleteSymptomLog()
        {
            SymptomManager m = new SymptomManager(new TreeMonDbContext(connectionKey));
            SymptomLog s = new SymptomLog()
            {
                AccountId = "a",
                Name = "DELETERECORD",
                CreatedBy = "TESTUSER",
                DateCreated = DateTime.UtcNow,
                DoseUUID = "D"
            };

            m.InsertLog(s);

            //Test the delete flag
            Assert.IsTrue(m.DeleteSymptomLog(s) > 0);
            m.GetSymptomLog("DELETERECORD");
            SymptomLog d = m.GetSymptomLog("DELETERECORD");
            Assert.IsNotNull(d);
            Assert.IsTrue(d.Deleted == true);


            Assert.IsTrue(m.DeleteSymptomLog(s, true) > 0);
            d = m.GetSymptomLog("DELETERECORD");
            Assert.IsNull(d);
        }

        [TestMethod]
        public void SymptomManager_GetSymptomsByDose()
        {
            SymptomManager m = new SymptomManager(new TreeMonDbContext(connectionKey));
            Assert.AreEqual(
              m.InsertLog(new SymptomLog()
              {
                  AccountId = "a",
                  Name = "TESTRECORD",
                  DateCreated = DateTime.UtcNow,
                  DoseUUID = "D"
              }, false)
           .Code, 200);

            Assert.IsTrue(m.GetSymptomsByDose("D", "", "a").Count > 0);
        }

        #endregion

    }
}
