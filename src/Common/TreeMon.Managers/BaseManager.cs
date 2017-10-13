// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
using Newtonsoft.Json;
using System;
using TreeMon.Managers.Interfaces;
using TreeMon.Managers.Membership;
using TreeMon.Models;
using TreeMon.Models.App;
using TreeMon.Models.Membership;

namespace TreeMon.Managers
{
    public class BaseManager 
    {
        private readonly RoleManager _roleManager;
        private readonly SessionManager _sessionManager;

        public string SessionKey{ get; set; }
        protected string _connectionKey;
        protected User _requestingUser = null;

        protected BaseManager() { }

        public  BaseManager(string connectionKey, string sessionKey )
        {
            _connectionKey = connectionKey;
            SessionKey = sessionKey;
            _sessionManager = new SessionManager(connectionKey);
            _requestingUser = this.GetUser(sessionKey);
            _roleManager = new RoleManager(connectionKey, _requestingUser);
           
        }

        public bool  DataAccessAuthorized(INode dataItem , string verb, bool isSensitiveData)
        {
            if (_requestingUser == null)
            {
                _requestingUser = this.GetUser(SessionKey);

                if (_requestingUser == null)
                    return false;
            }
            return _roleManager.DataAccessAuthorized(dataItem, _requestingUser, verb, isSensitiveData);
        }

        public User GetUser(string sessionToken)
        {
            if (string.IsNullOrWhiteSpace(sessionToken) || _sessionManager == null)
                return null;

            UserSession session = _sessionManager.GetSession(sessionToken);
            if (session == null)
                return null;

            return JsonConvert.DeserializeObject<User>(session.UserData);
        }

     
    }
}
