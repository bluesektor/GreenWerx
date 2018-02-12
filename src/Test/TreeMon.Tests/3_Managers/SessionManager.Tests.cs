// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TreeMon.Data;
using TreeMon.Models.App;
using System.Collections.Generic;
using TreeMon.Managers;
using System.Linq;
using TreeMon.Models.Membership;
using TreeMon.Managers.Membership;

namespace TreeMon.Web.Tests._templates
{
    [TestClass]
    public class SessionManager_Tests
    {
        private string connectionKey = "MSSQL_TEST";

        [TestMethod]
        public void SessionManager_GenerateToken_JWT()
        {
            string userName = Guid.NewGuid().ToString("N");
            User u =  TestHelper.GenerateTestUser(userName);
            SessionManager m = new SessionManager(connectionKey);
            string jwt = m.CreateJwt("jwt", u, "unitTest.Treemon.org");

            Assert.IsNotNull(jwt);
            //https://jwt.io/
        }

        //[TestMethod]
        //public void SessionManager_SaveSession()
        //{
        //    SessionManager m = new SessionManager(connectionKey);
        //    string name = Guid.NewGuid().ToString();

        //    UserSession us = new UserSession()
        //    {
        //         AuthToken = Guid.NewGuid().ToString("N"),
        //         Captcha = "abc123",
        //         IsPersistent = false,
        //         Issued = DateTime.UtcNow
        //    };

        //    Assert.IsNotNull(m.SaveSession(us));
        //}

        //[TestMethod]
        //public void SessionManager_SaveSession_with_Params()
        //{
        //    SessionManager m = new SessionManager(connectionKey);
        //    string name = Guid.NewGuid().ToString();
        //    Assert.IsNotNull(m.SaveSession("127.1.1.3", Guid.NewGuid().ToString("N"), ""));
        //}


        //[TestMethod]
        //public void SessionManager_IsValidSession()
        //{
        //    SessionManager m = new SessionManager(connectionKey);

        //    UserSession us = new UserSession()
        //    {
        //        AuthToken = Guid.NewGuid().ToString("N"),
        //        Captcha = "abc123",
        //        IsPersistent = false,
        //        Issued = DateTime.UtcNow
        //    };

        //    UserSession savedSession = m.SaveSession(us);
        //    Assert.IsNotNull(savedSession);
        //    Assert.IsTrue(m.IsValidSession(savedSession.AuthToken));
        //}

        //[TestMethod]
        //public void SessionManager_GetSessionByUser()
        //{
        //    SessionManager m = new SessionManager(connectionKey);
        //    string userUUID = Guid.NewGuid().ToString("N");

        //    UserSession us = new UserSession()
        //    {
        //        AuthToken = Guid.NewGuid().ToString("N"),
        //        Captcha = "abc123",
        //        IsPersistent = false,
        //        Issued = DateTime.UtcNow,
        //         UserUUID  = userUUID
        //    };

        //    Assert.IsNotNull(m.SaveSession(us));

        //    UserSession savedSession = m.GetSessionByUser(userUUID);
        //    Assert.IsNotNull(savedSession);
        //}

        //[TestMethod]
        //public void SessionManager_GetSessionByUserName()
        //{
        //    SessionManager m = new SessionManager(connectionKey);
        //    string userUUID = Guid.NewGuid().ToString("N");
        //    string username = Guid.NewGuid().ToString("N");

        //    UserSession us = new UserSession()
        //    {
        //        AuthToken = Guid.NewGuid().ToString("N"),
        //        Captcha = "abc123",
        //        IsPersistent = false,
        //        Issued = DateTime.UtcNow,
        //        UserUUID = userUUID,
        //         UserName = username
        //    };

        //    Assert.IsNotNull(m.SaveSession(us));

        //    UserSession savedSession = m.GetSessionByUserName(username);
        //    Assert.IsNotNull(savedSession);
        //}

        //[TestMethod]
        //public void SessionManager_GetSession()
        //{
        //    SessionManager m = new SessionManager(connectionKey);
        //    string userUUID = Guid.NewGuid().ToString("N");
        //    string AuthToken = Guid.NewGuid().ToString("N");

        //    UserSession us = new UserSession()
        //    {
        //        AuthToken = AuthToken,
        //        Captcha = "abc123",
        //        IsPersistent = false,
        //        Issued = DateTime.UtcNow,
        //        UserUUID = userUUID,
        //    };

        //    Assert.IsNotNull(m.SaveSession(us));

        //    UserSession savedSession = m.GetSession(AuthToken, false);
        //    Assert.IsNotNull(savedSession);

        //    savedSession = m.GetSession(AuthToken, true);
        //    Assert.IsNotNull(savedSession);
        //}

        //[TestMethod]
        //public void SessionManager_UpdateSession()
        //{
        //    SessionManager m = new SessionManager(connectionKey);

        //    UserSession us = new UserSession()
        //    {
        //        AuthToken = Guid.NewGuid().ToString("N"),
        //        Captcha = "abc123",
        //        IsPersistent = false,
        //        Issued = DateTime.UtcNow
        //    };

        //    UserSession savedSession = m.SaveSession(us);
        //    Assert.IsNotNull(savedSession);

        //    //update auth token
        //    savedSession.AuthToken = Guid.NewGuid().ToString("N");

        //    Assert.IsTrue(m.UpdateSession(savedSession));

        //    UserSession updatedSession = m.GetSession(savedSession.AuthToken);
        //    Assert.IsNotNull(updatedSession);
        //}

        //[TestMethod]
        //public void SessionManager_DeleteSession()
        //{
        //    SessionManager m = new SessionManager(connectionKey);
        //    string userUUID = Guid.NewGuid().ToString("N");
        //    string AuthToken = Guid.NewGuid().ToString("N");

        //    UserSession us = new UserSession()
        //    {
        //        AuthToken = AuthToken,
        //        Captcha = "abc123",
        //        IsPersistent = false,
        //        Issued = DateTime.UtcNow,
        //        UserUUID = userUUID
        //    };

        //    Assert.IsNotNull(m.SaveSession(us));

        //    UserSession savedSession = m.GetSession(AuthToken);
        //    Assert.IsNotNull(savedSession);

        //    Assert.IsTrue(m.DeleteSession(AuthToken));

        //    savedSession = m.GetSession(AuthToken);
        //    Assert.IsNull(savedSession);
        //}

        //[TestMethod]
        //public void SessionManager_DeleteByUserId()
        //{
        //    SessionManager m = new SessionManager(connectionKey);
        //    string userUUID = Guid.NewGuid().ToString("N");
        //    string AuthToken = Guid.NewGuid().ToString("N");

        //    UserSession us = new UserSession()
        //    {
        //        AuthToken = Guid.NewGuid().ToString("N"),
        //        Captcha = "abc123",
        //        IsPersistent = false,
        //        Issued = DateTime.UtcNow,
        //        UserUUID = userUUID
        //    };

        //    Assert.IsNotNull(m.SaveSession(us));

        //    UserSession savedSession = m.GetSessionByUser(userUUID);
        //    Assert.IsNotNull(savedSession);

        //    Assert.IsTrue(m.DeleteByUserId(userUUID));

        //    savedSession = m.GetSessionByUser(userUUID);
        //    Assert.IsNull(savedSession);
        //}


        //[TestMethod]
        //public void SessionManager_ClearExpiredSessions()
        //{
        //    SessionManager m = new SessionManager(connectionKey);
        //    string userUUID = Guid.NewGuid().ToString("N");
        //    string AuthToken = Guid.NewGuid().ToString("N");

        //    UserSession us = new UserSession()
        //    {
        //        AuthToken = Guid.NewGuid().ToString("N"),
        //        Captcha = "abc123",
        //        IsPersistent = false,
        //        Issued = DateTime.UtcNow,
        //        UserUUID = userUUID
        //    };

        //    Assert.IsNotNull(m.SaveSession(us));

        //    //Add an older token
        //    us.Issued = DateTime.UtcNow.AddMinutes(125 * -1);
        //    us.AuthToken = Guid.NewGuid().ToString("N");
        //    Assert.IsNotNull(m.SaveSession(us));

        //    TreeMonDbContext context = new TreeMonDbContext(connectionKey);
        //    int beforeClearCount = context.GetAll<UserSession>()
        //                                  .Where(w => w.Issued < DateTime.UtcNow.AddMinutes(120 * -1) && w.IsPersistent == false).Count();

        //    Assert.IsTrue(beforeClearCount > 0);

        //    int deletedSessions = m.ClearExpiredSessions(120);
        //    Assert.IsTrue(deletedSessions > 0);
        //    Assert.AreEqual(deletedSessions, beforeClearCount);


        //}



    }
}
