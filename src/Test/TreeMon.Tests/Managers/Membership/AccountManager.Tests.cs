using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TreeMon.Data;
using TreeMon.Models.App;
using System.Collections.Generic;
using TreeMon.Managers.Membership;
using TreeMon.Models.Membership;
using System.Linq;
using TreeMon.Data.Logging.Models;

namespace TreeMon.Web.Tests._templates
{
    [TestClass]
    public class AccountManager_Tests
    {
        private string connectionKey = "MSSQL_TEST";

        List<User> _users = new List<User>();


        [TestInitialize]
        public void TestSetup()
        {
            TreeMonDbContext context = new TreeMonDbContext(connectionKey);
            Random rand = new Random();
            //Seed AuthenticationLog
            for (int i = 0; i < 5; i++)
            {
                context.Insert<User>(new User()
                {
                    UUID = Guid.NewGuid().ToString("N"),
                     AccountId = SystemFlag.Default.Account
                });
            }
            _users.AddRange(context.GetAll<User>());
        }

        [TestMethod]
        public void AccountManager_Insert_Account()
        {
            AccountManager m = new AccountManager(new TreeMonDbContext(connectionKey));

            Assert.AreEqual(
                m.Insert(new Account()
                {
                    
                    Name = "TESTRECORD",
                    DateCreated = DateTime.UtcNow
                }, false)
             .Code, 200);

            //won't allow a duplicate name
            Assert.AreEqual(
              m.Insert(new Account()
              {
                  
                  Name = "TESTRECORD",
                  DateCreated = DateTime.UtcNow
              })
           .Code, 500);
        }

        [TestMethod]
        public void AccountManager_GetAccount()
        {
            AccountManager m = new AccountManager(new TreeMonDbContext(connectionKey));
            ServiceResult sr = m.Insert(new Account()
            {
                
                Name = "ALPHA",
                DateCreated = DateTime.UtcNow
            }, false);

            Assert.AreEqual(sr.Code, 200, sr.Message);
            string uuid = ((Account)sr.Record).UUID;
            Account s = m.GetAccountBy(uuid);
            Assert.IsNotNull(s);
            Account n = m.GetAccount("ALPHA");
            Assert.IsNotNull(n);

        }

        [TestMethod]
        public void AccountManager_UpdateAccount()
        {
            AccountManager m = new AccountManager(new TreeMonDbContext(connectionKey));
            ServiceResult sr = m.Insert(new Account()
            {
                
                Name = "TESTRECORD",
            },false);
            Assert.AreEqual(sr.Code, 200, sr.Message);
            string uuid = ((Account)sr.Record).UUID;
            Account s = m.GetAccountBy(uuid);
            s.Name = "UPDATEDRECORD";

            Assert.AreEqual(m.UpdateAccount(s).Code, 200);
             uuid = ((Account)sr.Record).UUID;
            Account u = m.GetAccountBy(uuid);
            Assert.IsNotNull(u);
        }

        [TestMethod]
        public void AccountManager_DeleteAccount()
        {
            AccountManager m = new AccountManager(new TreeMonDbContext(connectionKey));
            Account s = new Account()
            {
                Name = "DELETERECORD",
                CreatedById = "TESTUSER",
                
                DateCreated = DateTime.UtcNow,
            };

            ServiceResult sr = m.Insert(s,false);

            //Test the delete flag
            Assert.IsTrue(m.DeleteAccount(s) );
            string uuid = ((Account)sr.Record).UUID;
          
            Account d = m.GetAccountBy(uuid);
            Assert.IsNotNull(d);
            Assert.IsTrue(d.Deleted == true);


            Assert.IsTrue(m.DeleteAccount(s, true) );
            d = m.GetAccountBy(uuid);
            Assert.IsNull(d);
        }

        [TestMethod]
        public void AccountManager_DeleteAccount_ByUUID()
        {
            AccountManager m = new AccountManager(new TreeMonDbContext(connectionKey));
            Account s = new Account()
            {
                Name = "DELETERECORD",
                CreatedById = "TESTUSER",

                DateCreated = DateTime.UtcNow,
            };

            ServiceResult sr = m.Insert(s, false);

            //Test the delete flag
            Assert.IsTrue(m.DeleteAccount(s.UUID));
            string uuid = ((Account)sr.Record).UUID;

            Account d = m.GetAccountBy(uuid);
            Assert.IsNotNull(d);
            Assert.IsTrue(d.Deleted == true);


            Assert.IsTrue(m.DeleteAccount(s.UUID, true));
            d = m.GetAccountBy(uuid);
            Assert.IsNull(d);
        }

        [TestMethod]
        public void AccountManager_AddUserToAccount()
        {
            //string accountId =  Guid.NewGuid().ToString("N");
            AccountManager m = new AccountManager(new TreeMonDbContext(connectionKey));
            Assert.IsTrue(_users.Count > 0);
            Account s = new Account()
            {
                Name = "DELETERECORD",
                CreatedById = "TESTUSER",
                 UUID = "AAA",
                DateCreated = DateTime.UtcNow,
            };

            //create account
            Assert.AreEqual(m.Insert(s, false).Code, 200);

            if (m.IsUserInAccount("AAA", _users[0].UUID))
                m.RemoveUserFromAccount("AAA", _users[0].UUID);

            Assert.AreEqual(m.AddUserToAccount("AAA", _users[0].UUID).Code, 200);

            //test invalid user
            Assert.AreEqual(m.AddUserToAccount("AAA", "").Code, 500);

            //test invalid account
            Assert.AreEqual(m.AddUserToAccount("", _users[0].UUID).Code, 500);

            //test readding to the account ( user already in account ).
            Assert.AreEqual(m.AddUserToAccount("AAA", _users[0].UUID).Code, 200);
        }

        [TestMethod]
        public void AccountManager_AddUsersToAccount()
        {
            //string accountId =  Guid.NewGuid().ToString("N");
            AccountManager m = new AccountManager(new TreeMonDbContext(connectionKey));

            
            List<AccountMember> members = new List<AccountMember>();
            for (int i = 0; i < _users.Count; i++)
            {
                members.Add(new AccountMember()
                {
                    UUID = "ID",
                    AccountId ="AAA",
                    MemberId = _users[i].UUID
                });
            }

            //clear  from previous adds.
            Assert.AreEqual(m.RemoveUsersFromAccount(members).Code, 200);

            Assert.AreEqual(m.AddUsersToAccount(members).Code, 200);
        }

        [TestMethod]
        public void AccountManager_SetActiveAccount_VerifyAccount()
        {
            AccountManager m = new AccountManager(new TreeMonDbContext(connectionKey));
            Assert.IsTrue(_users.Count > 0);
            Account s = new Account()
            {
                Name = "DELETERECORD",
                CreatedById = "TESTUSER",
                UUID = "XXX",
                DateCreated = DateTime.UtcNow,
            };

            //create account
            Assert.AreEqual(m.Insert(s, false).Code, 200);

            //Add the user to the account (AccountMember).
            Assert.AreEqual(m.AddUserToAccount("XXX", _users[0].UUID).Code, 200);

            //Set the account as the active account.
            Assert.AreEqual(m.SetActiveAccount("XXX", _users[0].UUID).Code, 200);

            //blank user id
            Assert.AreEqual(m.SetActiveAccount("XXX", "").Code, 500);
            
            //invalid user
            Assert.AreEqual(m.SetActiveAccount("XXX", "asdrfqwfasdf244").Code, 500);

            //Should be alread active. test make sure ok
            Assert.AreEqual(m.SetActiveAccount("XXX", _users[0].UUID).Code, 200);

            //Verify via function call.
            Assert.IsTrue(m.IsUserInAccount("XXX", _users[0].UUID));
        }

        [TestMethod]
        public void AccountManager_RemoveUserFromAccount()
        {
            AccountManager m = new AccountManager(new TreeMonDbContext(connectionKey));
            Assert.IsTrue(_users.Count > 0);
            Account s = new Account()
            {
                Name = "DELETERECORD",
                CreatedById = "TESTUSER",
                UUID = "ZZZ",
                DateCreated = DateTime.UtcNow,
            };

            //create account
            Assert.AreEqual(m.Insert(s, false).Code, 200);

            Assert.AreEqual(m.AddUserToAccount("ZZZ", _users[0].UUID).Code, 200);

            Assert.AreEqual(m.RemoveUserFromAccount("ZZZ", _users[0].UUID).Code, 200);
            Assert.IsFalse(m.IsUserInAccount("ZZZ", _users[0].UUID));
        }

        [TestMethod]
        public void AccountManager_RemoveUserFromAllAccounts()
        {
            AccountManager m = new AccountManager(new TreeMonDbContext(connectionKey));
            Assert.IsTrue(_users.Count > 0);
            Account s = new Account()
            {
                Name = "DELTAACCOUNT",
                CreatedById = "TESTUSER",
                UUID = "DDD",
                DateCreated = DateTime.UtcNow,
            };

            //Create accounts
            Assert.AreEqual(m.Insert(s, false).Code, 200);
            s.UUID = "CCC";
            s.Name = "CHARLIEACCOUNT";
            Assert.AreEqual(m.Insert(s, false).Code, 200);

            //add user to accounts
            Assert.AreEqual(m.AddUserToAccount("DDD", _users[1].UUID).Code, 200);
            Assert.AreEqual(m.AddUserToAccount("CCC", _users[1].UUID).Code, 200);

            Assert.IsTrue(m.IsUserInAccount("DDD", _users[1].UUID));
            Assert.IsTrue(m.IsUserInAccount("CCC", _users[1].UUID));

            //now test the remove user from the accounts.
            Assert.AreEqual(m.RemoveUserFromAllAccounts(_users[1].UUID).Code, 200);

            Assert.IsFalse(m.IsUserInAccount("DDD", _users[1].UUID));
            Assert.IsFalse(m.IsUserInAccount("CCC", _users[1].UUID));
        }

        [TestMethod]
        public void AccountManager_RemoveUsersFromAccount()
        {
            AccountManager m = new AccountManager(new TreeMonDbContext(connectionKey));
            Assert.IsTrue(_users.Count > 0);
            Account s = new Account()
            {
                Name = "ECHOACCOUNT",
                CreatedById = "TESTUSER",
                UUID = "EEE",
                DateCreated = DateTime.UtcNow,
            };

            //Create account
            Assert.AreEqual(m.Insert(s, false).Code, 200);
            
            //add users to account
            Assert.AreEqual(m.AddUserToAccount("EEE", _users[0].UUID).Code, 200);
            Assert.AreEqual(m.AddUserToAccount("EEE", _users[1].UUID).Code, 200);

            Assert.IsTrue(m.IsUserInAccount("EEE", _users[0].UUID));
            Assert.IsTrue(m.IsUserInAccount("EEE", _users[1].UUID));

            List<AccountMember> users = new List<AccountMember>();
            users.Add(new AccountMember() { AccountId = "EEE", MemberId = _users[0].UUID });
            users.Add(new AccountMember() { AccountId = "EEE", MemberId = _users[1].UUID });
            Assert.AreEqual(m.RemoveUsersFromAccount(users).Code, 200);
            
            Assert.IsFalse(m.IsUserInAccount("EEE", _users[0].UUID));
            Assert.IsFalse(m.IsUserInAccount("EEE", _users[1].UUID));
        }

        [TestMethod]
        public void AccountManager_GetAccountNonMembers()
        {
            AccountManager m = new AccountManager(new TreeMonDbContext(connectionKey));
            Assert.IsTrue(_users.Count > 0);
            Account s = new Account()
            {
                Name = "FRANKACCOUNT",
                CreatedById = "TESTUSER",
                UUID = "FFF",
                DateCreated = DateTime.UtcNow,
            };

            //Create account
            Assert.AreEqual(m.Insert(s, false).Code, 200);

            //add user to the account
            Assert.AreEqual(m.AddUserToAccount("FFF", _users[0].UUID).Code, 200);

            //Make sure use is not in this account
            m.RemoveUserFromAccount("FFF", _users[1].UUID);

            Assert.IsTrue(m.GetAccountNonMembers("FFF").Count > 0);

        }

        [TestMethod]
        public void AccountManager_GetAccountMembers()
        {
            AccountManager m = new AccountManager(new TreeMonDbContext(connectionKey));
            Assert.IsTrue(_users.Count > 0);
            Account s = new Account()
            {
                Name = "GAMMAACCOUNT",
                CreatedById = "TESTUSER",
                UUID = "GGG",
                DateCreated = DateTime.UtcNow,
            };

            //Create account
            Assert.AreEqual(m.Insert(s, false).Code, 200);

            //add user to the account
            Assert.AreEqual(m.AddUserToAccount("GGG", _users[0].UUID).Code, 200);
            Assert.IsTrue(m.GetAccountMembers("GGG").Count > 0);
        }

        [TestMethod]
        public void AccountManager_GetNonMemberAccounts()
        {
            AccountManager m = new AccountManager(new TreeMonDbContext(connectionKey));
            Assert.IsTrue(_users.Count > 0);
            Account s = new Account()
            {
                Name = "HEACCOUNT",
                CreatedById = "TESTUSER",
                UUID = "HHH",
                DateCreated = DateTime.UtcNow,
            };

            //Create account
            Assert.AreEqual(m.Insert(s, false).Code, 200);
            s.UUID = "III";
            s.Name = "IDIOTACCOUNT";
            Assert.AreEqual(m.Insert(s, false).Code, 200);

            Assert.AreEqual(m.AddUserToAccount("HHH", _users[2].UUID).Code, 200);
            m.RemoveUserFromAccount("III", _users[2].UUID);

            Assert.IsTrue(m.GetNonMemberAccounts(_users[2].UUID).Count > 0);
        }

        [TestMethod]
        public void AccountManager_GetUsersAccounts()
        {
            AccountManager m = new AccountManager(new TreeMonDbContext(connectionKey));
            Assert.IsTrue(_users.Count > 0);
            Account s = new Account()
            {
                Name = "JACKACCOUNT",
                CreatedById = "TESTUSER",
                UUID = "JJJ",
                DateCreated = DateTime.UtcNow,
            };

            //Create account
            Assert.AreEqual(m.Insert(s, false).Code, 200);

            //add user to the account
            Assert.AreEqual(m.AddUserToAccount("JJJ", _users[3].UUID).Code, 200);
            Assert.IsTrue(m.GetUsersAccounts(_users[3].UUID).Count > 0);
        }
    }
}
