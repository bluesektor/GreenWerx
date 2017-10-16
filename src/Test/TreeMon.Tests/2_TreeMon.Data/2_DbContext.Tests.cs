// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TreeMon.Data;
using TreeMon.Data.Logging;
using System.Diagnostics;
using TreeMon.Models.App;
using TreeMon.Models.Membership;
using TreeMon.Utilites.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Data.SqlClient;
using System.Transactions;
using Dapper;
using System.Threading.Tasks;
using System.Data.Entity;

namespace TreeMon.Web.Tests.Data
{
    [TestClass]
    public class TreeMonDbContextTests
    {
        private User _user = TestHelper.GenerateTestUser(StringEx.GenerateAlphaNumId());
        private User _userAsync = TestHelper.GenerateTestUser(StringEx.GenerateAlphaNumId());

        //MSSQL_TEST
        //Create Database TreeMonDbContext.InstallDatabase
        //Create Tables
        //When All tests are dones drop tables and database



        //backlog after ALL the tests have run delete the database

        [TestMethod]
        public void TreeMonDbContext_CreateDatabase_MSSQL()
        {
            TreeMonDbContext context = new TreeMonDbContext("MSSQL_TEST");
            ServiceResult res = context.CreateDatabase(new AppInfo()
            {
                ActiveDbConnectionKey = "MSSQL_TEST",
                ActiveDbProvider = "MSSQL"
            },context.Database.Connection.ConnectionString);
            Assert.AreEqual(res.Code, 200);
        }

        [TestMethod]
        public void TreeMonDbContext_CreateDatabase_SQLite()
        {
            TreeMonDbContext context = new TreeMonDbContext("SQLITE_TEST");
            ServiceResult res = context.CreateDatabase(new AppInfo()
            {
                ActiveDbProvider = "SQLITE"
            }, context.Database.Connection.ConnectionString);
            Assert.AreEqual(res.Code, 500);
        }

        [TestMethod]
        public void TreeMonDbContext_CreateDatabase_MYSSQL()
        {
            TreeMonDbContext context = new TreeMonDbContext("MYSQL_TEST");
            ServiceResult res = context.CreateDatabase(new AppInfo()
            {
                ActiveDbProvider = "MYSSQL"
            }, context.Database.Connection.ConnectionString);

            Assert.AreEqual(res.Code, 500);
        }

        [TestMethod]
        public void TreeMonDbContext_CreateDatabase_Default()
        {
            TreeMonDbContext context = new TreeMonDbContext("MSSQL_TEST");
            ServiceResult res = context.CreateDatabase(new AppInfo()
            {
                ActiveDbProvider = ""
            }, context.Database.Connection.ConnectionString);

            Assert.AreEqual(res.Code, 500);
        }

        [TestMethod]
        public void TreeMonDbContext_Context_Insert()
        {
            TreeMonDbContext context = new TreeMonDbContext("MSSQL_TEST");
            Assert.IsTrue(context.Insert<User>(_user), context.Message);
        }

        [TestMethod]
        public void TreeMonDbContext_Context_Update()
        {
            TreeMonDbContext context = new TreeMonDbContext("MSSQL_TEST");
            _user = context.GetAll<User>().FirstOrDefault();
            _user.Name = "UPDATE_TEST";
            Assert.AreEqual(context.Update<User>(_user), 1);
            Assert.AreEqual(context.Get<User>(_user.Id).Name, "UPDATE_TEST");
        }

        [TestMethod]
        public void TreeMonDbContext_Context_Select_MSSQL()
        {
            TreeMonDbContext context = new TreeMonDbContext("MSSQL_TEST");
            _user = context.GetAll<User>().FirstOrDefault();
            DynamicParameters parameters = new DynamicParameters();//dapper
            parameters.Add("@NAME", _user.Name);
            // List<SqlParameter> parameters = new List<SqlParameter>();
            // parameters.Add(new SqlParameter("@NAME", _user.Name));
            List<User> users = context.Select<User>("SELECT * FROM USERS WHERE Name=@NAME", parameters).ToList();

            Assert.AreEqual(users.Count(), 1);
            Assert.AreEqual(users[0].Name, _user.Name);
        }

        [TestMethod]
        public void TreeMonDbContext_Context_ExecuteNonQuery_MSSQL()
        {
            TreeMonDbContext context = new TreeMonDbContext("MSSQL_TEST");
            _user = context.GetAll<User>().FirstOrDefault();
            DynamicParameters parameters = new DynamicParameters();//dapper
            parameters.Add("@PROVIDER", "UPDATED_PROVIDER");
            parameters.Add("@UUID", _user.UUID);

            //  List<SqlParameter> parameters = new List<SqlParameter>();
            // parameters.Add(new SqlParameter("@PROVIDER", "UPDATED_PROVIDER"));
            // parameters.Add(new SqlParameter("@UUID", _user.UUID));

            int res = context.ExecuteNonQuery("UPDATE USERS SET ProviderName=@PROVIDER WHERE UUID=@UUID", parameters);

            Assert.AreEqual(res, 1);
            Assert.AreEqual(context.Get<User>(_user.Id).ProviderName, "UPDATED_PROVIDER");
        }

        [TestMethod]
        public void TreeMonDbContext_Context_Delete()
        {
            TreeMonDbContext context = new TreeMonDbContext("MSSQL_TEST");
            User u = TestHelper.GenerateTestUser("DELETE_ME");
            Assert.IsTrue( context.Insert<User>(u),"failed to insert user for test");
             u = context.Get<User>(u.Id);
            Assert.IsNotNull(u);
            Assert.AreEqual(u.Name, "DELETE_ME");
            int res = context.Delete<User>(u);
            Assert.AreEqual(res, 1);
            Assert.IsNull(context.Get<User>(u.Id));
        }

        [TestMethod]
        public void TreeMonDbContext_Context_Delete_Where()
        {
            TreeMonDbContext context = new TreeMonDbContext("MSSQL_TEST");
            User u = TestHelper.GenerateTestUser("DELETE_ME_WHERE");
            context.Insert<User>(u);
            Assert.AreEqual(context.Get<User>(u.Id).Name, "DELETE_ME_WHERE");
            // List<SqlParameter> parameters = new List<SqlParameter>();
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@Name", "DELETE_ME_WHERE");
            int res = context.Delete<User>("WHERE Name=@Name", parameters);

            Assert.AreEqual(res, 1);
            Assert.IsNull(context.Get<User>(u.Id));
        }

        [TestMethod]
        public async void TreeMonDbContext_Context_Insert_Async()
        {
            TreeMonDbContext context = new TreeMonDbContext("MSSQL_TEST");
            int res = await context.InsertAsync<User>(_userAsync);
            Assert.IsTrue(res > 0);
        }

        [TestMethod]
        public void TreeMonDbContext_Context_Update_Async()
        {
            Task.Run(async () =>
            {
                TreeMonDbContext context = new TreeMonDbContext("MSSQL_TEST");
                _userAsync = context.GetAll<User>().FirstOrDefault();
                _userAsync.Name = "UPDATE_ASYNC_TEST";
                int res = await context.UpdateAsync<User>(_userAsync);
                Assert.AreEqual(res, 1);
                Assert.AreEqual(context.Get<User>(_userAsync.Id).Name, "UPDATE_ASYNC_TEST");
            }).GetAwaiter().GetResult();
        }

        [TestMethod]
        public void TreeMonDbContext_Context_Select_Async_MSSQL()
        {
            Task.Run(async () =>
            {
                TreeMonDbContext context = new TreeMonDbContext("MSSQL_TEST");
                _userAsync = context.GetAll<User>().FirstOrDefault();
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@NAME", _userAsync.Name);
                IEnumerable<User> users = await context.SelectAsync<User>("SELECT * FROM USERS WHERE Name=@NAME", parameters);

                Assert.AreEqual(users.ToList().Count(), 1);
                Assert.AreEqual(users.ToList()[0].Name, _userAsync.Name);
            }).GetAwaiter().GetResult();
        }

        [TestMethod]
        public void TreeMonDbContext_Context_ExecuteNonQuery_Async_MSSQL()
        {
            Task.Run(async () =>
            {
                TreeMonDbContext context = new TreeMonDbContext("MSSQL_TEST");
                _userAsync = context.GetAll<User>().FirstOrDefault();
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@PROVIDER", "UPDATED_ASYNC_PROVIDER");
                parameters.Add("@UUID", _userAsync.UUID);

                int res = await context.ExecuteNonQueryAsync("UPDATE USERS SET ProviderName=@PROVIDER WHERE UUID=@UUID", parameters);

                Assert.AreEqual(res, 1);
                Assert.AreEqual(context.Get<User>(_userAsync.Id).ProviderName, "UPDATED_ASYNC_PROVIDER");
            }).GetAwaiter().GetResult();
        }

        [TestMethod]
        public void TreeMonDbContext_Context_Delete_Async()
        {
            Task.Run(async () =>
            {
                TreeMonDbContext context = new TreeMonDbContext("MSSQL_TEST");
                User u = TestHelper.GenerateTestUser("DELETE_ME");
                context.Insert<User>(u);
                Assert.AreEqual(context.Get<User>(u.Id).Name, "DELETE_ME");

                int res = await context.DeleteAsync<User>(u);
                Assert.AreEqual(res, 1);
                Assert.IsNull(context.Get<User>(u.Id));

            }).GetAwaiter().GetResult();
        }

        [TestMethod]
        public void TreeMonDbContext_Context_Transaction_Succeed()
        {
            User u = TestHelper.GenerateTestUser("ALPHA");
            Account a = new Account() { Name = "ALPHA_ACCOUNT", DateCreated = DateTime.Now };

            TreeMonDbContext context = new TreeMonDbContext("MSSQL_TEST");


            using (var dbContext = new DbContext(context.ConnectionKey))
            {
                using (var contextTransaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        Assert.IsTrue(context.Insert<User>(u), "Failed to insert user.");
                        Assert.IsTrue(context.Insert<Account>(a), "Failed to insert account.");
                        contextTransaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        contextTransaction.Rollback();
                        Assert.Fail(ex.Message);
                    }
                }
            }

            Assert.AreEqual(context.Get<User>(u.Id).Name, "ALPHA");
            Assert.AreEqual(context.Get<Account>(a.Id).Name, "ALPHA_ACCOUNT");
        }

        [TestMethod]
        public void TreeMonDbContext_Context_Transaction_Fail()
        {
            User u = TestHelper.GenerateTestUser("BETA");
            Account a = new Account() { Name = "BETA_ACCOUNT" };
            string failTest = null;

            TreeMonDbContext context = new TreeMonDbContext("MSSQL_TEST");

            using (var dbContext = new DbContext(context.ConnectionKey))
            {
                using (var contextTransaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        context.Insert<User>(u);
                        failTest.ToUpper();//this should blow up.
                        context.Insert<Account>(a);
                        contextTransaction.Commit();
                    }
                    catch (Exception)
                    {
                        contextTransaction.Rollback();
                    }
                }
            }
            //Should have removed the user and not inserted the account.
            Assert.IsNull(context.Get<User>(u.Id), "BETA");
            Assert.IsNull(context.Get<Account>(a.Id), "BETA_ACCOUNT");
        }

        [TestMethod]
        public void TreeMonDbContext_Context_TransactionScope_Succeed()
        {
            User u = TestHelper.GenerateTestUser("CHARLIE");
            Account a = new Account() { Name = "CHARLIE_ACCOUNT", DateCreated =DateTime.Now };

            TreeMonDbContext contextUser = new TreeMonDbContext("MSSQL_TEST");
            TreeMonDbContext contextAccount = new TreeMonDbContext("MSSQL_TEST");

            using (var scope = new TransactionScope())
            {
                try
                {
                    contextUser.Insert<User>(u);
                    contextAccount.Insert<Account>(a);
                    scope.Complete();
                }
                catch (Exception)
                {

                }
            }

            Assert.AreEqual(contextUser.Get<User>(u.Id).Name, "CHARLIE");
            Assert.AreEqual(contextAccount.Get<Account>(a.Id).Name, "CHARLIE_ACCOUNT");
        }

        [TestMethod]
        public void TreeMonDbContext_Context_TransactionScope_Fail()
        {
            User u = TestHelper.GenerateTestUser("ECHO");
            Account a = new Account() { Name = "ECHO_ACCOUNT" };
            string failTest = null;

            TreeMonDbContext contextUser = new TreeMonDbContext("MSSQL_TEST");
            TreeMonDbContext contextAccount = new TreeMonDbContext("MSSQL_TEST");
            using (var scope = new TransactionScope())
            {
                try
                {
                    contextUser.Insert<User>(u);
                    failTest.ToUpper();//this should blow up.
                    contextAccount.Insert<Account>(a);
                    scope.Complete();
                }
                catch (Exception)
                {

                }
            }
            //Should have removed the user and not inserted the account.
            Assert.IsNull(contextUser.Get<User>(u.Id), "ECHO");
            Assert.IsNull(contextAccount.Get<Account>(a.Id), "ECHO_ACCOUNT");
        }
    }
}
