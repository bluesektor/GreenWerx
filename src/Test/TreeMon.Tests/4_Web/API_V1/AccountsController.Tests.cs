// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using TreeMon.Data;
using TreeMon.Data.Logging.Models;
using TreeMon.Managers;
using TreeMon.Managers.Membership;
using TreeMon.Models.App;
using TreeMon.Models.Membership;
using TreeMon.Utilites.Security;

namespace TreeMon.Web.Tests.API.V1
{

    [TestClass]
    public class AccountControllerTests   //todo re-implement
    {
       // //private string connectionKey = "MSSQL_TEST";
      //  private string _ownerAuthToken = "";
       // private string _captcha = "TESTCAPTCHA";


        //[TestInitialize]
        //public void TestSetup()
        //{
        //    _ownerAuthToken = TestHelper.InitializeControllerTestData(connectionKey, "OWNER");
        //    Assert.IsNotNull(ConfigurationManager.AppSettings["DefaultDbConnection"]);
        //}

        //#region Standard Account tests

        //[TestMethod]
        //public void Api_AccountController_Add_Account()
        //{
        //    Account mdl = new Account();
        //    mdl.Name = Guid.NewGuid().ToString("N");
            
        //    string postData = JsonConvert.SerializeObject(mdl);
        //    Task.Run(async () =>
        //    {
        //        ServiceResult res = await TestHelper.SentHttpRequest("POST", "api/Accounts/Add", postData, _ownerAuthToken);
        //        Assert.IsNotNull(res);
        //        Assert.AreEqual(res.Code, 200);

        //        Account p = JsonConvert.DeserializeObject<Account>(res.Result.ToString());
        //        Assert.IsNotNull(p);
        //        TreeMonDbContext context = new TreeMonDbContext(connectionKey);
        //        Account dbAccount = context.GetAll<Account>().Where(w => w.UUID == p.UUID).FirstOrDefault();
        //        Assert.IsNotNull(dbAccount);
        //        Assert.AreEqual(mdl.Name, dbAccount.Name);

        //    }).GetAwaiter().GetResult();

        //}

        //[TestMethod]
        //public void Api_AccountController_Get_Account()
        //{
        //    TreeMonDbContext context = new TreeMonDbContext(connectionKey);
        //    Account mdl = new Account();
        //    mdl.Name = Guid.NewGuid().ToString("N");
        //    mdl.UUID = Guid.NewGuid().ToString("N");
        //    Assert.IsTrue(context.Insert<Account>(mdl));

        //    Task.Run(async () =>
        //    {
        //        ServiceResult res = await TestHelper.SentHttpRequest("GET", "api/Accounts/" + mdl.UUID, "", _ownerAuthToken);

        //        Assert.IsNotNull(res);
        //        Assert.AreEqual(res.Code, 200);

        //        Account p = JsonConvert.DeserializeObject<Account>(res.Result.ToString());
        //        Assert.IsNotNull(p);
        //        Assert.AreEqual(mdl.Name, p.Name);

        //    }).GetAwaiter().GetResult();
        //}

        //[TestMethod]
        //public void Api_AccountController_Get_Accounts()
        //{
        //    TreeMonDbContext context = new TreeMonDbContext(connectionKey);
        //    Account mdl = new Account();
        //    mdl.Name = Guid.NewGuid().ToString("N");
        //    Assert.IsTrue(context.Insert<Account>(mdl));

        //    Account mdl2 = new Account();
        //    mdl2.Name = Guid.NewGuid().ToString("N");
        //    Assert.IsTrue(context.Insert<Account>(mdl2));

        //    Task.Run(async () =>
        //    {
        //        ServiceResult res = await TestHelper.SentHttpRequest("POST", "api/Accounts/", "", _ownerAuthToken);

        //        Assert.IsNotNull(res);
        //        Assert.AreEqual(res.Code, 200);

        //        List<Account> Accounts = JsonConvert.DeserializeObject<List<Account>>(res.Result.ToString());
        //        Assert.IsNotNull(Accounts);
        //        Assert.IsTrue(Accounts.Count >= 2);

        //        int foundAccounts = 0;
        //        foreach (Account p in Accounts)
        //        {
        //            if (p.Name == mdl.Name || p.Name == mdl2.Name)
        //                foundAccounts++;

        //        }

        //        Assert.AreEqual(foundAccounts, 2);

        //    }).GetAwaiter().GetResult();
        //}

        //[TestMethod]
        //public void Api_AccountController_Delete_Account_ById()
        //{
        //    TreeMonDbContext context = new TreeMonDbContext(connectionKey);
        //    Account mdl = new Account();
        //    mdl.Name = Guid.NewGuid().ToString("N");
        //    mdl.UUID = Guid.NewGuid().ToString("N");
        //    Assert.IsTrue(context.Insert<Account>(mdl));

        //    Task.Run(async () =>
        //    {
        //        ServiceResult res = await TestHelper.SentHttpRequest("GET", "api/Accounts/"+ mdl.UUID + "/Delete", "", _ownerAuthToken);

        //        Assert.IsNotNull(res);
        //        Assert.AreEqual(res.Code, 200);

        //        Account dbAccount = context.GetAll<Account>().Where(w => w.Name == mdl.Name).FirstOrDefault();
        //        Assert.IsNotNull(dbAccount);
        //        Assert.IsTrue(dbAccount.Deleted);
        //        //Assert.IsNull(dbAccount);

        //    }).GetAwaiter().GetResult();
        //}

        //[TestMethod]
        //public void Api_AccountController_Delete_Account()
        //{
        //    TreeMonDbContext context = new TreeMonDbContext(connectionKey);
        //    Account mdl = new Account();
        //    mdl.Name = Guid.NewGuid().ToString("N");
        //    mdl.UUID = Guid.NewGuid().ToString("N");
        //    Assert.IsTrue(context.Insert<Account>(mdl));
        //    string postData = "";
        //    JsonSerializerSettings settings = new JsonSerializerSettings();
        //    settings.Formatting = Formatting.Indented;
        //    postData = JsonConvert.SerializeObject(mdl, settings);

        //    Task.Run(async () =>
        //    {
        //        ServiceResult res = await TestHelper.SentHttpRequest("POST", "api/Accounts/Delete", postData, _ownerAuthToken);

        //        Assert.IsNotNull(res);
        //        Assert.AreEqual(res.Code, 200);

        //        Account dbAccount = context.GetAll<Account>().Where(w => w.Name == mdl.Name).FirstOrDefault();
        //        Assert.IsNotNull(dbAccount);
        //        Assert.IsTrue(dbAccount.Deleted);
        //        //Assert.IsNull(dbAccount);

        //    }).GetAwaiter().GetResult();
        //}

        //[TestMethod]
        //public void Api_AccountController_Update_Account()
        //{
        //    TreeMonDbContext context = new TreeMonDbContext(connectionKey);
        //    Account mdl = new Account();
        //    mdl.Name = Guid.NewGuid().ToString("N");
        //    mdl.UUID = Guid.NewGuid().ToString("N");
        //    mdl.Email = "test@test.com";
        //    Assert.IsTrue(context.Insert<Account>(mdl));
        //    mdl = context.GetAll<Account>().Where(w => w.Name == mdl.Name).FirstOrDefault();
        //    Account pv = new Account();
        //    pv.UUID = mdl.UUID;
        //    pv.Name = mdl.Name;
        //    pv.Email = "updated@email.com";
        //    //~~~ Updatable fields ~~~

        //    string postData = JsonConvert.SerializeObject(pv);
        //    Task.Run(async () =>
        //    {
        //        ServiceResult res = await TestHelper.SentHttpRequest("POST", "api/Accounts/Update", postData, _ownerAuthToken);
        //        Assert.IsNotNull(res);
        //        Assert.AreEqual(res.Code, 200);

        //        Account dbAccount = context.GetAll<Account>().Where(w => w.Name == mdl.Name).FirstOrDefault();
        //        Assert.IsNotNull(dbAccount);
        //        Assert.AreEqual(pv.Email, dbAccount.Email);
        //        //Assert.AreEqual(pv.Category, dbAccount.Category);

        //    }).GetAwaiter().GetResult();

        //}

        //#endregion

        //[TestMethod]
        //public void Api_AccountController_AddUsersToAccount()
        //{
        //    TreeMonDbContext context = new TreeMonDbContext(connectionKey);

        //    Account acct = TestHelper.GenerateTestAccount(Guid.NewGuid().ToString("N"));
        //    Assert.IsTrue(context.Insert<Account>(acct));
        //    User u = TestHelper.GenerateTestUser(Guid.NewGuid().ToString("N"));
        //    Assert.IsTrue(context.Insert<User>(u));

        //    AccountMember mdl = new AccountMember();
        //    mdl.UUIDType = "AccountMember";
        //    mdl.AccountUUID = acct.UUID; 
        //    mdl.MemberUUID = u.UUID;// Id for the member type. In this case a User
        //    mdl.MemberType = "User";//type of id that is assigned to the account
        //    mdl.UUID = Guid.NewGuid().ToString("N");
        //    Assert.IsTrue(context.Insert<AccountMember>(mdl));

        //    acct = TestHelper.GenerateTestAccount(Guid.NewGuid().ToString("N"));
        //    Assert.IsTrue(context.Insert<Account>(acct));
        //    u = TestHelper.GenerateTestUser(Guid.NewGuid().ToString("N"));
        //    Assert.IsTrue(context.Insert<User>(u));

        //    AccountMember mdl2 = new AccountMember();
        //    mdl2.AccountUUID = acct.UUID;
        //    mdl2.MemberUUID =u.UUID;// Id for the member type. In this case a User
        //    mdl2.MemberType = "User";//type of id that is assigned to the account
        //    mdl2.UUID = Guid.NewGuid().ToString("N");

        //    List<AccountMember> AccountMembers = new List<AccountMember>();
        //    AccountMembers.Add(mdl);
        //    AccountMembers.Add(mdl2);
        //    string postData = JsonConvert.SerializeObject(AccountMembers);

        //    Task.Run(async () =>
        //    {
        //        ServiceResult res = await TestHelper.SentHttpRequest("POST", "api/Accounts/Users/Add", postData, _ownerAuthToken);

        //        Assert.IsNotNull(res);
        //        Assert.AreEqual(res.Code, 200);

        //        AccountMember dbAccountMember = context.GetAll<AccountMember>().Where(w => w.AccountUUID == mdl.AccountUUID && w.MemberUUID == mdl.MemberUUID).FirstOrDefault();
        //        Assert.IsNotNull(dbAccountMember);

        //        dbAccountMember = context.GetAll<AccountMember>().Where(w => w.AccountUUID == mdl2.AccountUUID && w.MemberUUID == mdl2.MemberUUID).FirstOrDefault();
        //        Assert.IsNotNull(dbAccountMember);

        //    }).GetAwaiter().GetResult();
        //}

        //[TestMethod]
        //public void Api_AccountController_AddUserToAccount()
        //{
        //    TreeMonDbContext context = new TreeMonDbContext(connectionKey);
        //    User u = TestHelper.GenerateTestUser(Guid.NewGuid().ToString("N"));
        //    Assert.IsTrue(context.Insert<User>(u));

        //    Account mdl = TestHelper.GenerateTestAccount(Guid.NewGuid().ToString("N"));
        //    Assert.IsTrue(context.Insert<Account>(mdl));

        //    Task.Run(async () =>
        //    {
        //        ServiceResult res = await TestHelper.SentHttpRequest("POST", "api/Accounts/"+ mdl.UUID + "/Users/"+ u.UUID + "/Add", "", _ownerAuthToken);

        //        Assert.IsNotNull(res);
        //        Assert.AreEqual(res.Code, 200);

        //        AccountMember dbAccountMember = context.GetAll<AccountMember>().Where(w => w.AccountUUID == mdl.UUID && w.MemberUUID == u.UUID).FirstOrDefault();
        //        Assert.IsNotNull(dbAccountMember);

        //    }).GetAwaiter().GetResult();
        //}

        //[TestMethod]
        //public void Api_AccountController_GetAccountPermissons()
        //{
        //    //     Assert.IsTrue(TestHelper.AccountHasPermissionsAdded( connectionKey, SystemFlag.Default.Account));

        //    TreeMonDbContext context = new TreeMonDbContext(connectionKey);
        //    Account account = TestHelper.GenerateTestAccount(Guid.NewGuid().ToString("N"));
        //    Assert.IsTrue(context.Insert<Account>(account));

        //    Task.Run(async () =>
        //    {
        //        ServiceResult res = await TestHelper.SentHttpRequest("POST", "api/Accounts/"+ SystemFlag.Default.Account + "/Permissions", "", _ownerAuthToken);

        //        Assert.IsNotNull(res);
        //        Assert.AreEqual(res.Code, 200);

        //         List<Permission> permissions = JsonConvert.DeserializeObject<List<Permission>>(res.Result.ToString());
        //        Assert.IsNotNull(permissions);
        //        Assert.IsTrue(permissions.Count > 0);
    
        //    }).GetAwaiter().GetResult();
        //}

        //[TestMethod]
        //public void Api_AccountController_GetMembers()
        //{
        //    TreeMonDbContext context = new TreeMonDbContext(connectionKey);
        //    Account account = TestHelper.GenerateTestAccount(Guid.NewGuid().ToString("N"));

        //    Assert.IsTrue(context.Insert<Account>(account));

        //    User user = TestHelper.GenerateTestUser(Guid.NewGuid().ToString("N"));
        //    Assert.IsTrue(context.Insert<User>(user));

        //    AccountMember mdl = new AccountMember();
        //    mdl.UUIDType = "AccountMember";
        //    mdl.AccountUUID = account.UUID;
        //    mdl.MemberUUID = user.UUID;// Id for the member type. In this case a User
        //    mdl.MemberType = "User";//type of id that is assigned to the account
        //    mdl.UUID = Guid.NewGuid().ToString("N");
        //    Assert.IsTrue(context.Insert<AccountMember>(mdl));

        //    Task.Run(async () =>
        //    {
        //        ServiceResult res = await TestHelper.SentHttpRequest("POST", "api/Accounts/" + account.UUID + "/Members", "", _ownerAuthToken);

        //        Assert.IsNotNull(res);
        //        Assert.AreEqual(res.Code, 200);

        //        List<User> accountMembers = JsonConvert.DeserializeObject<List<User>>(res.Result.ToString());
        //        Assert.IsNotNull(accountMembers);
        //        Assert.AreEqual(accountMembers.Count , 1);
        //        Assert.AreEqual(accountMembers[0].UUID, user.UUID);

        //    }).GetAwaiter().GetResult();
        //}

        //[TestMethod]
        //public void Api_AccountController_GetNonMembers()
        //{
        //    TreeMonDbContext context = new TreeMonDbContext(connectionKey);

        //    #region NonMember initialization
        //    //Add non member user.
        //    User nonMemberUser = TestHelper.GenerateTestUser(Guid.NewGuid().ToString("N"));
        //    nonMemberUser.AccountUUID = Guid.NewGuid().ToString("N");
        //    Assert.IsTrue(context.Insert<User>(nonMemberUser));

        //    string nonMemberUserAccountUUID = Guid.NewGuid().ToString("N");
        //    //Add a non member
        //    AccountMember accountNonMember = new AccountMember();
        //    accountNonMember.UUIDType = "AccountMember";
        //    accountNonMember.AccountUUID = nonMemberUser.AccountUUID;
        //    accountNonMember.MemberUUID = nonMemberUser.UUID; ;// Id for the member type. In this case a User
        //    accountNonMember.MemberType = "User";//type of id that is assigned to the account
        //    accountNonMember.UUID = Guid.NewGuid().ToString("N");
        //    Assert.IsTrue(context.Insert<AccountMember>(accountNonMember));

        //    #endregion

        //    #region Member Initialization

        //    //Add account member user.
        //    User user = TestHelper.GenerateTestUser(Guid.NewGuid().ToString("N"));
        //    Assert.IsTrue(context.Insert<User>(user));

        //    Account account = TestHelper.GenerateTestAccount(Guid.NewGuid().ToString("N"));
        //    Assert.IsTrue(context.Insert<Account>(account));

        //    //Add account member
        //    AccountMember member = new AccountMember();
        //    member.UUIDType = "AccountMember";
        //    member.AccountUUID = account.UUID;
        //    member.MemberUUID = user.UUID;// Id for the member type. In this case a User
        //    member.MemberType = "User";//type of id that is assigned to the account
        //    member.UUID = Guid.NewGuid().ToString("N");
        //    Assert.IsTrue(context.Insert<AccountMember>(member));

        //    #endregion

        //    Task.Run(async () =>
        //    {
        //        //note the filter. This is to make sure we return all records
        //        ServiceResult res = await TestHelper.SentHttpRequest("POST", "api/Accounts/" + account.UUID + "/NonMembers?filter=*.*", "", _ownerAuthToken);

        //        Assert.IsNotNull(res);
        //        Assert.AreEqual(res.Code, 200);

        //        List<User> accountNonMembers = JsonConvert.DeserializeObject<List<User>>(res.Result.ToString());
        //        Assert.IsNotNull(accountNonMembers);
                
        //        //nonMemberUser should be in accountNonMembers
        //        // the  User.AcccountId may not match the AcountMember.Account id because the user can be in multiple accounts
        //        //and the User.AcccountId is just the active account 
        //        //
        //        AccountMember nonAM = context.GetAll<AccountMember>().Where(w => w.MemberUUID == nonMemberUser.UUID).FirstOrDefault();
        //        Assert.IsNotNull(nonAM);
        //        Assert.AreEqual(nonAM.AccountUUID, nonMemberUser.AccountUUID);
        //        Assert.AreNotEqual(nonAM.AccountUUID, account.UUID);


        //        User nonAccountmember = accountNonMembers.Where(w => w.UUID == nonMemberUser.UUID).FirstOrDefault();
        //        Assert.IsNotNull(nonAccountmember);
        //        Assert.AreEqual(nonAccountmember.UUID, nonMemberUser.UUID);

        //    }).GetAwaiter().GetResult();
        //}

        //[TestMethod]
        //public void Api_AccountController_Login()
        //{
        //    Assert.IsNotNull(ConfigurationManager.AppSettings["AppKey"]);

        //    TreeMonDbContext context = new TreeMonDbContext(connectionKey);
        //    User u = TestHelper.GenerateTestUser(Guid.NewGuid().ToString("N"));
        //    string loginPassword = u.Password;
        //    string tmpHashPassword = PasswordHash.CreateHash(u.Password);
        //    u.Password = PasswordHash.ExtractHashPassword(tmpHashPassword);
        //    u.PasswordHashIterations = PasswordHash.ExtractIterations(tmpHashPassword);
        //    u.PasswordSalt = PasswordHash.ExtractSalt(tmpHashPassword);
        //    Assert.IsTrue(context.Insert<User>(u));

        //    LoginModel credentials = new LoginModel()
        //    {
        //        Password = loginPassword,
        //        UserName  = u.Name
        //    };

        //    string postData = JsonConvert.SerializeObject(credentials);

        //    Task.Run(async () =>
        //    {
        //        ServiceResult res = await TestHelper.SentHttpRequest("POST", "api/Accounts/Login", postData, _ownerAuthToken);

        //        Assert.IsNotNull(res);
        //        Assert.AreEqual(res.Code, 200);
        //    }).GetAwaiter().GetResult();
        //}

        //[TestMethod]
        //public void Api_AccountController_LoginAsync()
        //{
        //    Assert.IsNotNull(ConfigurationManager.AppSettings["AppKey"]);

        //    TreeMonDbContext context = new TreeMonDbContext(connectionKey);
        //    User u = TestHelper.GenerateTestUser(Guid.NewGuid().ToString("N"));
        //    string loginPassword = u.Password;
        //    string tmpHashPassword = PasswordHash.CreateHash(u.Password);
        //    u.Password = PasswordHash.ExtractHashPassword(tmpHashPassword);
        //    u.PasswordHashIterations = PasswordHash.ExtractIterations(tmpHashPassword);
        //    u.PasswordSalt = PasswordHash.ExtractSalt(tmpHashPassword);
        //    Assert.IsTrue(context.Insert<User>(u));

        //    LoginModel credentials = new LoginModel()
        //    {
        //        Password = loginPassword,
        //        UserName = u.Name
        //    };

        //    string postData = JsonConvert.SerializeObject(credentials);

        //    Task.Run(async () =>
        //    {
        //        ServiceResult res = await TestHelper.SentHttpRequest("POST", "api/Accounts/LoginAsync", postData, _ownerAuthToken);

        //        Assert.IsNotNull(res);
        //        Assert.AreEqual(res.Code, 200);
        //    }).GetAwaiter().GetResult();
        //}

        //[TestMethod]
        //public void Api_AccountController_WPLogin()
        //{
        //    Assert.IsNotNull(ConfigurationManager.AppSettings["AppKey"]);

        //    TreeMonDbContext context = new TreeMonDbContext(connectionKey);
        //    User u = TestHelper.GenerateTestUser(Guid.NewGuid().ToString("N"));

        //    LoginModel credentials = new LoginModel()
        //    {
        //        Password = u.Password,
        //        UserName = u.Name
        //    };

        //    string postData = JsonConvert.SerializeObject(credentials);

        //    Task.Run(async () =>
        //    {
        //        ServiceResult res = await TestHelper.SentHttpRequest("POST", "api/Accounts/WpLogin", postData, _ownerAuthToken);

        //        Assert.IsNotNull(res);
        //        Assert.AreEqual(res.Code, 500);//the service returns a string. The parse will fail
        //        Assert.IsTrue(res.Message.Contains("Error invalid name or password"));

        //        LogEntry le = context.GetAll<LogEntry>().Where(w => w.StackTrace.Contains(u.Name)).FirstOrDefault();
        //        Assert.IsNotNull(le);

        //    }).GetAwaiter().GetResult();
        //}

        //[TestMethod]
        //public void Api_AccountController_LogOut()
        //{
        //    Api_AccountController_Login();
        //    TreeMonDbContext context = new TreeMonDbContext(connectionKey);
        //    UserSession s = context.GetAll<UserSession>().Where(w => w.AuthToken == _ownerAuthToken).FirstOrDefault();
        //    Assert.IsNotNull(s);

        //    Task.Run(async () =>
        //    {
        //        ServiceResult res = await TestHelper.SentHttpRequest("GET", "api/Accounts/LogOut", "", _ownerAuthToken);

        //        Assert.IsNotNull(res);
        //        Assert.AreEqual(res.Code, 200);//the service returns a string. The parse will fail


        //         s = context.GetAll<UserSession>().Where(w => w.AuthToken == _ownerAuthToken).FirstOrDefault();
        //        Assert.IsNull(s);
        //    }).GetAwaiter().GetResult();
        //}

        //[TestMethod]
        //public void Api_AccountController_RegisterAsync()
        //{
        //    Assert.IsNotNull(ConfigurationManager.AppSettings["AppKey"]);

        //    TreeMonDbContext context = new TreeMonDbContext(connectionKey);
        //    UserRegister reg = TestHelper.GetUserRegister(Guid.NewGuid().ToString("N"));
        //    reg.ClientType= "mobile.app";//change to web if we want to send the validation email
        //    string postData = JsonConvert.SerializeObject(reg);

        //    Task.Run(async () =>
        //    {
        //        ServiceResult res = await TestHelper.SentHttpRequest("POST", "api/Accounts/Register", postData, _ownerAuthToken);

        //        Assert.IsNotNull(res);
        //        Assert.AreEqual(res.Code, 200);
        //        User u = context.GetAll<User>().Where(w => w.Name == reg.Name ).FirstOrDefault();
        //        Assert.IsNotNull(u);

        //    }).GetAwaiter().GetResult();
        //}

        //[TestMethod]
        //public void Api_AccountController_RemoveUserFromAccount()
        //{
        //    TreeMonDbContext context = new TreeMonDbContext(connectionKey);
        //    User user = TestHelper.GenerateTestUser(Guid.NewGuid().ToString("N"));
        //    Assert.IsTrue(context.Insert<User>(user));

        //    Account account = TestHelper.GenerateTestAccount(Guid.NewGuid().ToString("N"));
        //    Assert.IsTrue(context.Insert<Account>(account));

        //    Task.Run(async () =>
        //    {
        //        //First add the user to the an account
        //        ServiceResult res = await TestHelper.SentHttpRequest("POST", "api/Accounts/" + account.UUID + "/Users/" + user.UUID + "/Add", "", _ownerAuthToken);
        //        Assert.IsNotNull(res);
        //        Assert.AreEqual(res.Code, 200);

        //        //Now remove it
        //        res = await TestHelper.SentHttpRequest("POST", "api/Accounts/" + account.UUID + "/Users/" + user.UUID + "/Remove", "", _ownerAuthToken);
        //        Assert.IsNotNull(res);
        //        Assert.AreEqual(res.Code, 200);

        //        AccountMember dbAccountMember = context.GetAll<AccountMember>().Where(w => w.AccountUUID == account.UUID && w.MemberUUID == user.UUID).FirstOrDefault();
        //        Assert.IsNull(dbAccountMember);

        //    }).GetAwaiter().GetResult();
        //}

        //[TestMethod]
        //public void Api_AccountController_RemoveUsersFromAccount()
        //{
        //    TreeMonDbContext context = new TreeMonDbContext(connectionKey);

        //    Account acct = TestHelper.GenerateTestAccount(Guid.NewGuid().ToString("N"));
        //    Assert.IsTrue(context.Insert<Account>(acct));

        //    User u = TestHelper.GenerateTestUser(Guid.NewGuid().ToString("N"));
        //    Assert.IsTrue(context.Insert<User>(u));

        //    AccountMember mdl = new AccountMember();
        //    mdl.UUIDType = "AccountMember";
        //    mdl.AccountUUID = acct.UUID;
        //    mdl.MemberUUID = u.UUID;// Id for the member type. In this case a User
        //    mdl.MemberType = "User";//type of id that is assigned to the account
        //    mdl.UUID = Guid.NewGuid().ToString("N");
        //    Assert.IsTrue(context.Insert<AccountMember>(mdl));

        //    acct = TestHelper.GenerateTestAccount(Guid.NewGuid().ToString("N"));
        //    Assert.IsTrue(context.Insert<Account>(acct));
        //    u = TestHelper.GenerateTestUser(Guid.NewGuid().ToString("N"));
        //    Assert.IsTrue(context.Insert<User>(u));

        //    AccountMember mdl2 = new AccountMember();
        //    mdl2.AccountUUID = acct.UUID;
        //    mdl2.MemberUUID = u.UUID;// Id for the member type. In this case a User
        //    mdl2.MemberType = "User";//type of id that is assigned to the account
        //    mdl2.UUID = Guid.NewGuid().ToString("N");

        //    List<AccountMember> AccountMembers = new List<AccountMember>();
        //    AccountMembers.Add(mdl);
        //    AccountMembers.Add(mdl2);
        //    string postData = JsonConvert.SerializeObject(AccountMembers);

        //    Task.Run(async () =>
        //    {
        //        //Add users to an account
        //        ServiceResult res = await TestHelper.SentHttpRequest("POST", "api/Accounts/Users/Add", postData, _ownerAuthToken);
        //        Assert.IsNotNull(res);
        //        Assert.AreEqual(res.Code, 200);


        //        res = await TestHelper.SentHttpRequest("POST", "api/Accounts/Users/Remove", postData, _ownerAuthToken);
        //        Assert.IsNotNull(res);
        //        Assert.AreEqual(res.Code, 200);


        //        AccountMember dbAccountMember = context.GetAll<AccountMember>().Where(w => w.AccountUUID == mdl.AccountUUID && w.MemberUUID == mdl.MemberUUID).FirstOrDefault();
        //        Assert.IsNull(dbAccountMember);

        //        dbAccountMember = context.GetAll<AccountMember>().Where(w => w.AccountUUID == mdl2.AccountUUID && w.MemberUUID == mdl2.MemberUUID).FirstOrDefault();
        //        Assert.IsNull(dbAccountMember);

        //    }).GetAwaiter().GetResult();
        //}

        //[TestMethod]
        //public void Api_AccountController_SetAccountFlag()
        //{
        //    #region Log In
        //    TreeMonDbContext context = new TreeMonDbContext(connectionKey);
        //    User u = TestHelper.GenerateTestUser(Guid.NewGuid().ToString("N"));
        //    string loginPassword = u.Password;
        //    string tmpHashPassword = PasswordHash.CreateHash(u.Password);
        //    u.Password = PasswordHash.ExtractHashPassword(tmpHashPassword);
        //    u.PasswordHashIterations = PasswordHash.ExtractIterations(tmpHashPassword);
        //    u.PasswordSalt = PasswordHash.ExtractSalt(tmpHashPassword);
        //    Assert.IsTrue(context.Insert<User>(u));

        //    LoginModel credentials = new LoginModel()
        //    {
        //        Password = loginPassword,
        //        UserName = u.Name
        //    };

        //    string postData = JsonConvert.SerializeObject(credentials);

        //    Task.Run(async () =>
        //    {
        //        ServiceResult res = await TestHelper.SentHttpRequest("POST", "api/Accounts/Login", postData, _ownerAuthToken);

        //        Assert.IsNotNull(res);
        //        Assert.AreEqual(res.Code, 200);
        //    }).GetAwaiter().GetResult();
        //    #endregion

        //    Account account = new Account();
        //    account.Name = Guid.NewGuid().ToString("N");
        //    account.UUID = Guid.NewGuid().ToString("N");
        //    account.Email = "test@test.com";
        //    Assert.IsTrue(context.Insert<Account>(account));
        //    account = context.GetAll<Account>().Where(w => w.Name == account.Name).FirstOrDefault();
        //    Task.Run(async () =>
        //    {
        //        ServiceResult res = await TestHelper.SentHttpRequest("POST", "api/Accounts/"+ account.UUID + "/Users/"+u.UUID+ "/SetDefault", "", _ownerAuthToken);
        //        Assert.IsNotNull(res);
        //        Assert.AreEqual(res.Code, 500);
        //        Assert.IsTrue(res.Message.Contains("User must be added to the account"));

        //        //Add the user to the an account
        //        res = await TestHelper.SentHttpRequest("POST", "api/Accounts/" + account.UUID + "/Users/" + u.UUID + "/Add", "", _ownerAuthToken);
        //        Assert.IsNotNull(res);
        //        Assert.AreEqual(res.Code, 200);

        //        res = await TestHelper.SentHttpRequest("POST", "api/Accounts/" + account.UUID + "/Users/" + u.UUID + "/SetDefault", "", _ownerAuthToken);
        //        Assert.IsNotNull(res);
        //        Assert.AreEqual(res.Code, 200);

        //    }).GetAwaiter().GetResult();
        //}

        //[TestMethod]
        //public void Api_AccountController_ChangePassword()
        //{
        //    Assert.IsNotNull(ConfigurationManager.AppSettings["AppKey"]);

        //    TreeMonDbContext context = new TreeMonDbContext(connectionKey);
        //    User u = TestHelper.GenerateTestUser(Guid.NewGuid().ToString("N"));
        //    string loginPassword = u.Password;
        //    string tmpHashPassword = PasswordHash.CreateHash(u.Password);
        //    u.Password = PasswordHash.ExtractHashPassword(tmpHashPassword);
        //    u.PasswordHashIterations = PasswordHash.ExtractIterations(tmpHashPassword);
        //    u.PasswordSalt = PasswordHash.ExtractSalt(tmpHashPassword);
        //    Assert.IsTrue(context.Insert<User>(u));

        //    // set a user session then pass the authtoken
        //    SessionManager sessionManager = new SessionManager(connectionKey);
        //    string userJson = JsonConvert.SerializeObject(u);
        //    UserSession us = sessionManager.SaveSession("127.1.1.31", u.UUID, userJson, false);
   

        //    ChangePassword frm = new ChangePassword()
        //    {
        //        OldPassword = loginPassword,
        //        NewPassword = "NewPassword",
        //        ConfirmPassword = "NewPassword",
        //        //   ConfirmationCode
        //        // ResetPassword
        //    };

        //    string postData = JsonConvert.SerializeObject(frm);

        //    Task.Run(async () =>
        //    {
        //        ServiceResult res = await TestHelper.SentHttpRequest("POST", "api/Accounts/ChangePassword/", postData, us.AuthToken);

        //        Assert.IsNotNull(res);
        //        Assert.AreEqual(res.Code, 200);
        //    }).GetAwaiter().GetResult();
        //}

        ////SendAccountValidationEmailAsync is erroring out. It gets through the
        ////authorization filter then returns 500..
        ////[TestMethod]
        ////public void Api_AccountController_SendAccountValidationEmailAsync()
        ////{
        ////    //Check for supporting keys since it sends email
        ////    Assert.IsNotNull(ConfigurationManager.AppSettings["SiteDomain"]);
        ////    Assert.IsNotNull(ConfigurationManager.AppSettings["SiteEmail"]);
        ////    Assert.IsNotNull(ConfigurationManager.AppSettings["AppKey"]);
        ////    Assert.IsNotNull(ConfigurationManager.AppSettings["MailHost"]);
        ////    Assert.IsNotNull(ConfigurationManager.AppSettings["MailPort"]);
        ////    Assert.IsNotNull(ConfigurationManager.AppSettings["EmailHostUser"]);
        ////    Assert.IsNotNull(ConfigurationManager.AppSettings["EmailHostPassword"]);
        ////    Assert.IsNotNull(ConfigurationManager.AppSettings["UseSSL"]);
        ////    Assert.IsNotNull(ConfigurationManager.AppSettings["TemplatePasswordResetEmail"]);
        //// //   Api_AccountController_Login();
        ////    TreeMonDbContext context = new TreeMonDbContext(connectionKey);
        ////    SendAccountInfoForm form = new SendAccountInfoForm()
        ////    {
        ////        CaptchaCode = _captcha,
        ////        Email = ConfigurationManager.AppSettings["SiteEmail"].ToString(),//this would actually be another users email
        ////         PasswordReset = false  
        ////    };
        ////    string postData = JsonConvert.SerializeObject(form);
        ////    Task.Run(async () =>
        ////    {
        ////        ServiceResult res = await TestHelper.SentHttpRequest("POST", "api/Accounts/SendInfo/", postData, _ownerAuthToken);
        ////        Assert.IsNotNull(res);
        ////        Assert.AreEqual(res.Code, 200);
        ////    }).GetAwaiter().GetResult();
        ////}
        ////[TestMethod]
        ////public void Api_AccountController_SetAccountFlag()
        ////{
        ////    TreeMonDbContext context = new TreeMonDbContext(connectionKey);
        ////    Account account = new Account();
        ////    account.Name = Guid.NewGuid().ToString("N");
        ////    account.UUID = Guid.NewGuid().ToString("N");
        ////    account.Email = "test@test.com";
        ////    Assert.IsTrue(context.Insert<Account>(account));
        ////    account = context.GetAll<Account>().Where(w => w.Name == account.Name).FirstOrDefault();
        ////    Task.Run(async () =>
        ////    {
        ////        ServiceResult res = await TestHelper.SentHttpRequest("POST", "api/Accounts/"+ account.UUID + "/Flag/{accountFlag}/Value/{flagValue}", "", _ownerAuthToken);
        ////        Assert.IsNotNull(res);
        ////        Assert.AreEqual(res.Code, 200);
        ////        Account dbAccount = context.GetAll<Account>().Where(w => w.Name == account.Name).FirstOrDefault();
        ////        Assert.IsNotNull(dbAccount);
        ////        Assert.AreEqual(pv.Email, dbAccount.Email);
        ////        //Assert.AreEqual(pv.Category, dbAccount.Category);
        ////    }).GetAwaiter().GetResult();
        ////}
    }
}
