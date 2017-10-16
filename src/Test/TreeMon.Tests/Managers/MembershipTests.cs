using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TreeMon.Managers.Membership;
using TreeMon.Data;
using TreeMon.Models.Membership;
using TreeMon.Models.App;
using TreeMon.Web.api.v1;


/// <summary>
/// Integration tests..
/// In enterprise vs you can create ordered tests.. don't have so do this.
/// </summary>
namespace TreeMon.Web.Tests.Managers
{
    [TestClass]
    public class MembershipTest
    {
        private string _userName = "MembershipTest";

        //This test the creation of a user in the table.
        [TestMethod]
        public void UserManager_CreateUser()
        {
            //UserManager um = new UserManager(new TreeMonDbContext("MSSQL_TEST"));
            //User u = TestHelper.GetBasicUser(_userName + ".alpha" );
            //ServiceResult sr = um.InsertUser(u, "t.e.s.t");
            //Assert.AreEqual("OK", sr.Result);
        }

        [TestMethod]
        public void UserManager_RegisterUser()
        {
            //UserManager um = new UserManager(new TreeMonDbContext("MSSQL_TEST"));
            //ServiceResult sr = um.RegisterUser(TestHelper.GetUserRegister(_userName + ".beta" ), true, "t.e.s.t",false); //this doesn't send the validation email.
            //Assert.AreEqual("OK", sr.Result);

            //this would go in controllers test. maybe one for API.
            //AccountController api = new AccountController();
            //api.Register(new UserRegister() {  })
            //_userManager.RegisterUser()
        }


     
    }
}
