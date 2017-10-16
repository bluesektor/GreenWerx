// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TreeMon.Utilites.Helpers;
using TreeMon.Data.Logging;
using System.IO;
using System.Data;
using TreeMon.Data;

namespace TreeMon.Web.Tests.TreeMon.Data
{
    [TestClass]
    public class FileLoggingTests
    {
        string _pathToFile = EnvironmentEx.AppDataFolder + "_unitTest.log";

        [TestMethod]
        public void FileLoggingWrite_Message()
        {
            FileLog fl = new FileLog();
            fl.LogFile(_pathToFile);

            fl.Write("FileLoggingWrite_Message");

            if (!File.Exists(_pathToFile))
                Assert.Fail("Log file doesn't exist " + _pathToFile);

            string tmp = File.ReadAllText(_pathToFile);

            Assert.IsTrue(tmp.Contains( "FileLoggingWrite_Message"));

            File.Delete(_pathToFile);

        }

        [TestMethod]
        public void FileLogging_Write_Message_And_Exception()
        {
            FileLog fl = new FileLog();
            fl.LogFile(_pathToFile);

            try
            {
                throw new DivideByZeroException();
            }
            catch(DivideByZeroException ex)
            {
                fl.Write("FileLoggingWrite_Message",ex);
            }

            if (!File.Exists(_pathToFile))
                Assert.Fail("Log file doesn't exist " + _pathToFile);

            string tmp = File.ReadAllText(_pathToFile);

            Assert.IsTrue(tmp.Contains("FileLoggingWrite_Message"));

            File.Delete(_pathToFile);
        }

        [TestMethod]
        public void FileLogging_Write_Message_And_IDBCommand()
        {
            FileLog fl = new FileLog();

            fl.LogFile(_pathToFile);

            TreeMonDbContext context = new TreeMonDbContext("MSSQL_TEST");
            IDbCommand cmd = context.Database.Connection.CreateCommand();
            cmd.CommandText = "Select * from test";
            IDbDataParameter p = cmd.CreateParameter();
            p.ParameterName = "testParam";
            p.Value = "testValue";
            cmd.Parameters.Add(p);

            fl.Write("FileLoggingWrite_Message", cmd);

            if (!File.Exists(_pathToFile))
                Assert.Fail("Log file doesn't exist " + _pathToFile);

            string tmp = File.ReadAllText(_pathToFile);

            Assert.IsTrue(tmp.Contains("FileLoggingWrite_Message"));
            Assert.IsTrue(tmp.Contains("Select * from test"));
            Assert.IsTrue(tmp.Contains("testParam: testValue"));
            
            try
            {
                File.Delete(_pathToFile);
            }catch(Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod]
        public void FileLogging_Write_Message_And_Exception_And_IDBCommand()
        {
            FileLog fl = new FileLog();

            fl.LogFile(_pathToFile);
            TreeMonDbContext context = new TreeMonDbContext("MSSQL_TEST");
            IDbCommand cmd = context.Database.Connection.CreateCommand();
            cmd.CommandText = "Select * from test";
            IDbDataParameter p = cmd.CreateParameter();
            p.ParameterName = "testParam";
            p.Value = "testValue";
            cmd.Parameters.Add(p);
           
            try
            {
                throw new DivideByZeroException();
            }
            catch (DivideByZeroException ex)
            {
                fl.Write("FileLoggingWrite_Message", ex, cmd);
            }


            if (!File.Exists(_pathToFile))
                Assert.Fail("Log file doesn't exist " + _pathToFile);

            string tmp = File.ReadAllText(_pathToFile);

            Assert.IsTrue(tmp.Contains("FileLoggingWrite_Message"));
            Assert.IsTrue(tmp.Contains("FileLoggingWrite_Message"));
            Assert.IsTrue(tmp.Contains("Select * from test"));
            Assert.IsTrue(tmp.Contains("testParam: testValue"));

            File.Delete(_pathToFile);
        }
    }
}
