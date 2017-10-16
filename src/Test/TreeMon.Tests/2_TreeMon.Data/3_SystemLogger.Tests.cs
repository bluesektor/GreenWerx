// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TreeMon.Data;
using System.Diagnostics;
using TreeMon.Data.Logging;
using TreeMon.Data.Logging.Models;
using System.Linq;
using System.Threading.Tasks;

namespace TreeMon.Web.Tests.TreeMon.Data
{
    [TestClass]
    public class SystemLoggerTests
    {
        private  string connectionKey = "MSSQL_TEST";

      
        [TestInitialize]
        public void TestSetup()
        {
        
        }
        [TestCleanup]
        public void TestCleanup()
        {
        }

        [TestMethod]
        public void TreeMonData_SystemLogger_Insert()
        {
            LogEntry logEntry = new LogEntry()
            {
                InnerException = "IE",
                Level = "TEST",
                LogDate = DateTime.UtcNow,
                Source = "SystemLoggerTests",
                Type = "TESTINSERT",
                User = "na"
            };

            SystemLogger sl = new SystemLogger(connectionKey);
            Assert.IsTrue( sl.Insert(logEntry));
            Assert.IsNotNull(sl.Get(logEntry.Id));
        }

        [TestMethod]
        public void TreeMonData_SystemLogger_Delete()
        {
            SystemLogger sl = new SystemLogger(connectionKey);
            LogEntry le = sl.GetAll().FirstOrDefault();
            Assert.IsTrue(sl.Delete(le) > 0);
        }

        [TestMethod]
        public void TreeMonData_SystemLogger_DeleteAll()
        {
            LogEntry logEntry = new LogEntry()
            {
                InnerException = "IE",
                Level = "TEST",
                LogDate = DateTime.UtcNow,
                Source = "SystemLoggerTests",
                Type = "TESTINSERT",
                User = "na"
            };
           
            SystemLogger sl = new SystemLogger(connectionKey);
            sl.Insert(logEntry);
            sl.Insert(logEntry);

            int currentCount = sl.GetAll().Count();
            Assert.IsTrue(sl.DeleteAll() > 0);
            Assert.AreNotEqual(currentCount, sl.GetAll().Count());

        }


        [TestMethod]
        public void TreeMonData_SystemLogger_Insert_Async()
        {
            Task.Run(async () =>
            {
                LogEntry logEntry = new LogEntry()
                {
                    InnerException = "IE",
                    Level = "TEST",
                    LogDate = DateTime.UtcNow,
                    Source = "SystemLoggerTests",
                    Type = "TESTINSERT",
                    User = "na"
                };

                SystemLogger sl = new SystemLogger(connectionKey);
                Assert.IsTrue(await sl.InsertAsync(logEntry));
                Assert.IsNotNull(sl.Get(logEntry.Id));
            }).GetAwaiter().GetResult();
        }
            
    }
}
