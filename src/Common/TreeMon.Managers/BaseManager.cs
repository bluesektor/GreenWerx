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
        private  RoleManager _roleManager;
        private SessionManager _sessionManager;

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
            if(_roleManager == null)
                _roleManager = new RoleManager(_connectionKey, _requestingUser);
            return _roleManager.DataAccessAuthorized(dataItem, _requestingUser, verb, isSensitiveData);
        }

        public bool DataAccessAuthorized(string  type, bool allowSensitiveData, bool allowPrivateData)
        {
            if (_requestingUser == null)
            {
                _requestingUser = this.GetUser(SessionKey);

                if (_requestingUser == null)
                    return false;
            }
            if (_roleManager == null)
                _roleManager = new RoleManager(_connectionKey, _requestingUser);
            return _roleManager.DataAccessAuthorized(type, _requestingUser, allowSensitiveData, allowPrivateData);
        }

        public User GetUser(string sessionToken)
        {
            if (string.IsNullOrWhiteSpace(sessionToken) )
                return null;

            if (_sessionManager == null)
                _sessionManager = new SessionManager(this._connectionKey);

            UserSession session = _sessionManager.GetSession(sessionToken);
            if (session == null || string.IsNullOrWhiteSpace(session.UserData))
                return null;

            return JsonConvert.DeserializeObject<User>(session.UserData);
        }

     
    }
}
