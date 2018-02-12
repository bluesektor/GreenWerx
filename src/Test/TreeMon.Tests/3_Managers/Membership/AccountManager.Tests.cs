// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
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
        //private string connectionKey = "MSSQL_TEST";

        List<User> _users = new List<User>();


        //[TestInitialize]
        //public void TestSetup()
        //{
        //    TreeMonDbContext context = new TreeMonDbContext(connectionKey);
        //    Random rand = new Random();
        //    //Seed AuthenticationLog
        //    for (int i = 0; i < 5; i++)
        //    {
        //        context.Insert<User>(new User()
        //        {
        //            UUID = Guid.NewGuid().ToString("N"),
        //             AccountUUID = SystemFlag.Default.Account
        //        });
        //    }
        //    _users.AddRange(context.GetAll<User>());
        //}

        //[TestMethod]
        //public void AccountManager_Insert_Account()
        //{
        //    AccountManager m = new AccountManager(connectionKey);

        //    Assert.AreEqual(
        //        m.Insert(new Account()
        //        {
                    
        //            Name = "TESTRECORD",
        //            DateCreated = DateTime.UtcNow
        //        }, false)
        //     .Code, 200);

        //    //won't allow a duplicate name
        //    Assert.AreEqual(
        //      m.Insert(new Account()
        //      {
                  
        //          Name = "TESTRECORD",
        //          DateCreated = DateTime.UtcNow
        //      })
        //   .Code, 500);
        //}

        //[TestMethod]
        //public void AccountManager_Get()
        //{
        //    AccountManager m = new AccountManager(connectionKey);
        //    ServiceResult sr = m.Insert(new Account()
        //    {
                
        //        Name = "ALPHA",
        //        DateCreated = DateTime.UtcNow
        //    }, false);

        //    Assert.AreEqual(sr.Code, 200, sr.Message);
        //    string uuid = ((Account)sr.Result).UUID;
        //    Account s = (Account)m.GetBy(uuid);
        //    Assert.IsNotNull(s);
        //    Account n = m.Get("ALPHA");
        //    Assert.IsNotNull(n);

        //}

        //[TestMethod]
        //public void AccountManager_Update()
        //{
        //    AccountManager m = new AccountManager(connectionKey);
        //    ServiceResult sr = m.Insert(new Account()
        //    {
                
        //        Name = "TESTRECORD",
        //    },false);
        //    Assert.AreEqual(sr.Code, 200, sr.Message);
        //    string uuid = ((Account)sr.Result).UUID;
        //    Account s = m.GetBy(uuid);
        //    s.Name = "UPDATEDRECORD";

        //    Assert.AreEqual(m.Update(s).Code, 200);
        //     uuid = ((Account)sr.Result).UUID;
        //    Account u = m.GetBy(uuid);
        //    Assert.IsNotNull(u);
        //}

        //[TestMethod]
        //public void AccountManager_Delete()
        //{
        //    AccountManager m = new AccountManager(connectionKey);
        //    Account s = new Account()
        //    {
        //        Name = "DELETERECORD",
        //        CreatedBy = "TESTUSER",
                
        //        DateCreated = DateTime.UtcNow,
        //    };

        //    ServiceResult sr = m.Insert(s,false);

        //    //Test the delete flag
        //    Assert.IsTrue(m.Delete(s).Code == 200 );
        //    string uuid = ((Account)sr.Result).UUID;
          
        //    Account d = (Account)m.GetBy(uuid);
        //    Assert.IsNotNull(d);
        //    Assert.IsTrue(d.Deleted == true);


        //    Assert.IsTrue(m.Delete(s, true) );
        //    d = m.GetBy(uuid);
        //    Assert.IsNull(d);
        //}

        //[TestMethod]
        //public void AccountManager_Delete_ByUUID()
        //{
        //    AccountManager m = new AccountManager(connectionKey);
        //    Account s = new Account()
        //    {
        //        Name = "DELETERECORD",
        //        CreatedBy= "TESTUSER",

        //        DateCreated = DateTime.UtcNow,
        //    };

        //    ServiceResult sr = m.Insert(s, false);

        //    //Test the delete flag
        //    Assert.IsTrue(m.Delete(s.UUID));
        //    string uuid = ((Account)sr.Result).UUID;

        //    Account d = m.GetBy(uuid);
        //    Assert.IsNotNull(d);
        //    Assert.IsTrue(d.Deleted == true);


        //    Assert.IsTrue(m.Delete(s.UUID, true));
        //    d = m.GetBy(uuid);
        //    Assert.IsNull(d);
        //}

        //[TestMethod]
        //public void AccountManager_AddUserToAccount()
        //{
        //    //string AccountUUID =  Guid.NewGuid().ToString("N");
        //    AccountManager m = new AccountManager(connectionKey);
        //    Assert.IsTrue(_users.Count > 0);
        //    Account s = new Account()
        //    {
        //        Name = "DELETERECORD",
        //        CreatedBy = "TESTUSER",
        //         UUID = "AAA",
        //        DateCreated = DateTime.UtcNow,
        //    };

        //    //create account
        //    Assert.AreEqual(m.Insert(s, false).Code, 200);

        //    if (m.IsUserInAccount("AAA", _users[0].UUID))
        //        m.RemoveUserFromAccount("AAA", _users[0].UUID);

        //    Assert.AreEqual(m.AddUserToAccount("AAA", _users[0].UUID).Code, 200);

        //    //test invalid user
        //    Assert.AreEqual(m.AddUserToAccount("AAA", "").Code, 500);

        //    //test invalid account
        //    Assert.AreEqual(m.AddUserToAccount("", _users[0].UUID).Code, 500);

        //    //test readding to the account ( user already in account ).
        //    Assert.AreEqual(m.AddUserToAccount("AAA", _users[0].UUID).Code, 200);
        //}

        //[TestMethod]
        //public void AccountManager_AddUsersToAccount()
        //{
        //    //string AccountUUID =  Guid.NewGuid().ToString("N");
        //    AccountManager m = new AccountManager(connectionKey);

            
        //    List<AccountMember> members = new List<AccountMember>();
        //    for (int i = 0; i < _users.Count; i++)
        //    {
        //        members.Add(new AccountMember()
        //        {
        //            UUID = "ID",
        //            AccountUUID ="AAA",
        //            MemberUUID = _users[i].UUID
        //        });
        //    }

        //    //clear  from previous adds.
        //    Assert.AreEqual(m.RemoveUsersFromAccount(members).Code, 200);

        //    Assert.AreEqual(m.AddUsersToAccount(members).Code, 200);
        //}

        //[TestMethod]
        //public void AccountManager_SetActiveAccount_VerifyAccount()
        //{
        //    AccountManager m = new AccountManager(connectionKey);
        //    Assert.IsTrue(_users.Count > 0);
        //    Account s = new Account()
        //    {
        //        Name = "DELETERECORD",
        //        CreatedBy = "TESTUSER",
        //        UUID = "XXX",
        //        DateCreated = DateTime.UtcNow,
        //    };

        //    //create account
        //    Assert.AreEqual(m.Insert(s, false).Code, 200);

        //    //Add the user to the account (AccountMember).
        //    Assert.AreEqual(m.AddUserToAccount("XXX", _users[0].UUID).Code, 200);

        //    //Set the account as the active account.
        //    Assert.AreEqual(m.SetActiveAccount("XXX", _users[0].UUID).Code, 200);

        //    //blank user id
        //    Assert.AreEqual(m.SetActiveAccount("XXX", "").Code, 500);
            
        //    //invalid user
        //    Assert.AreEqual(m.SetActiveAccount("XXX", "asdrfqwfasdf244").Code, 500);

        //    //Should be alread active. test make sure ok
        //    Assert.AreEqual(m.SetActiveAccount("XXX", _users[0].UUID).Code, 200);

        //    //Verify via function call.
        //    Assert.IsTrue(m.IsUserInAccount("XXX", _users[0].UUID));
        //}

        //[TestMethod]
        //public void AccountManager_RemoveUserFromAccount()
        //{
        //    AccountManager m = new AccountManager(connectionKey);
        //    Assert.IsTrue(_users.Count > 0);
        //    Account s = new Account()
        //    {
        //        Name = "DELETERECORD",
        //        CreatedBy = "TESTUSER",
        //        UUID = "ZZZ",
        //        DateCreated = DateTime.UtcNow,
        //    };

        //    //create account
        //    Assert.AreEqual(m.Insert(s, false).Code, 200);

        //    Assert.AreEqual(m.AddUserToAccount("ZZZ", _users[0].UUID).Code, 200);

        //    Assert.AreEqual(m.RemoveUserFromAccount("ZZZ", _users[0].UUID).Code, 200);
        //    Assert.IsFalse(m.IsUserInAccount("ZZZ", _users[0].UUID));
        //}

        //[TestMethod]
        //public void AccountManager_RemoveUserFromAllAccounts()
        //{
        //    AccountManager m = new AccountManager(connectionKey);
        //    Assert.IsTrue(_users.Count > 0);
        //    Account s = new Account()
        //    {
        //        Name = "DELTAACCOUNT",
        //        CreatedBy = "TESTUSER",
        //        UUID = "DDD",
        //        DateCreated = DateTime.UtcNow,
        //    };

        //    //Create accounts
        //    Assert.AreEqual(m.Insert(s, false).Code, 200);
        //    s.UUID = "CCC";
        //    s.Name = "CHARLIEACCOUNT";
        //    Assert.AreEqual(m.Insert(s, false).Code, 200);

        //    //add user to accounts
        //    Assert.AreEqual(m.AddUserToAccount("DDD", _users[1].UUID).Code, 200);
        //    Assert.AreEqual(m.AddUserToAccount("CCC", _users[1].UUID).Code, 200);

        //    Assert.IsTrue(m.IsUserInAccount("DDD", _users[1].UUID));
        //    Assert.IsTrue(m.IsUserInAccount("CCC", _users[1].UUID));

        //    //now test the remove user from the accounts.
        //    Assert.AreEqual(m.RemoveUserFromAllAccounts(_users[1].UUID).Code, 200);

        //    Assert.IsFalse(m.IsUserInAccount("DDD", _users[1].UUID));
        //    Assert.IsFalse(m.IsUserInAccount("CCC", _users[1].UUID));
        //}

        //[TestMethod]
        //public void AccountManager_RemoveUsersFromAccount()
        //{
        //    AccountManager m = new AccountManager(connectionKey);
        //    Assert.IsTrue(_users.Count > 0);
        //    Account s = new Account()
        //    {
        //        Name = "ECHOACCOUNT",
        //        CreatedBy = "TESTUSER",
        //        UUID = "EEE",
        //        DateCreated = DateTime.UtcNow,
        //    };

        //    //Create account
        //    Assert.AreEqual(m.Insert(s, false).Code, 200);
            
        //    //add users to account
        //    Assert.AreEqual(m.AddUserToAccount("EEE", _users[0].UUID).Code, 200);
        //    Assert.AreEqual(m.AddUserToAccount("EEE", _users[1].UUID).Code, 200);

        //    Assert.IsTrue(m.IsUserInAccount("EEE", _users[0].UUID));
        //    Assert.IsTrue(m.IsUserInAccount("EEE", _users[1].UUID));

        //    List<AccountMember> users = new List<AccountMember>();
        //    users.Add(new AccountMember() { AccountUUID = "EEE", MemberUUID = _users[0].UUID });
        //    users.Add(new AccountMember() { AccountUUID = "EEE", MemberUUID = _users[1].UUID });
        //    Assert.AreEqual(m.RemoveUsersFromAccount(users).Code, 200);
            
        //    Assert.IsFalse(m.IsUserInAccount("EEE", _users[0].UUID));
        //    Assert.IsFalse(m.IsUserInAccount("EEE", _users[1].UUID));
        //}

        //[TestMethod]
        //public void AccountManager_GetNonMembers()
        //{
        //    AccountManager m = new AccountManager(connectionKey);
        //    Assert.IsTrue(_users.Count > 0);
        //    Account s = new Account()
        //    {
        //        Name = "FRANKACCOUNT",
        //        CreatedBy = "TESTUSER",
        //        UUID = "FFF",
        //        DateCreated = DateTime.UtcNow,
        //    };

        //    //Create account
        //    Assert.AreEqual(m.Insert(s, false).Code, 200);

        //    //add user to the account
        //    Assert.AreEqual(m.AddUserToAccount("FFF", _users[0].UUID).Code, 200);

        //    //Make sure use is not in this account
        //    m.RemoveUserFromAccount("FFF", _users[1].UUID);

        //    Assert.IsTrue(m.GetNonMembers("FFF").Count > 0);

        //}

        //[TestMethod]
        //public void AccountManager_GetMembers()
        //{
        //    AccountManager m = new AccountManager(connectionKey);
        //    Assert.IsTrue(_users.Count > 0);
        //    Account s = new Account()
        //    {
        //        Name = "GAMMAACCOUNT",
        //        CreatedBy = "TESTUSER",
        //        UUID = "GGG",
        //        DateCreated = DateTime.UtcNow,
        //    };

        //    //Create account
        //    Assert.AreEqual(m.Insert(s, false).Code, 200);

        //    //add user to the account
        //    Assert.AreEqual(m.AddUserToAccount("GGG", _users[0].UUID).Code, 200);
        //    Assert.IsTrue(m.GetMembers("GGG").Count > 0);
        //}

        //[TestMethod]
        //public void AccountManager_GetNonMemberAccounts()
        //{
        //    AccountManager m = new AccountManager(connectionKey);
        //    Assert.IsTrue(_users.Count > 0);
        //    Account s = new Account()
        //    {
        //        Name = "HEACCOUNT",
        //        CreatedBy= "TESTUSER",
        //        UUID = "HHH",
        //        DateCreated = DateTime.UtcNow,
        //    };

        //    //Create account
        //    Assert.AreEqual(m.Insert(s, false).Code, 200);
        //    s.UUID = "III";
        //    s.Name = "IDIOTACCOUNT";
        //    Assert.AreEqual(m.Insert(s, false).Code, 200);

        //    Assert.AreEqual(m.AddUserToAccount("HHH", _users[2].UUID).Code, 200);
        //    m.RemoveUserFromAccount("III", _users[2].UUID);

        //    Assert.IsTrue(m.GetNonMemberAccounts(_users[2].UUID).Count > 0);
        //}

        //[TestMethod]
        //public void AccountManager_GetUsersAccounts()
        //{
        //    AccountManager m = new AccountManager(connectionKey);
        //    Assert.IsTrue(_users.Count > 0);
        //    Account s = new Account()
        //    {
        //        Name = "JACKACCOUNT",
        //        CreatedBy = "TESTUSER",
        //        UUID = "JJJ",
        //        DateCreated = DateTime.UtcNow,
        //    };

        //    //Create account
        //    Assert.AreEqual(m.Insert(s, false).Code, 200);

        //    //add user to the account
        //    Assert.AreEqual(m.AddUserToAccount("JJJ", _users[3].UUID).Code, 200);
        //    Assert.IsTrue(m.GetUsersAccounts(_users[3].UUID).Count > 0);
        //}
    }
}
