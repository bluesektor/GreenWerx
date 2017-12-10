// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
using Dapper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using TreeMon.Data;
using TreeMon.Data.Logging;
using TreeMon.Models.App;
using TreeMon.Models.Membership;
using TreeMon.Utilites.Extensions;

namespace TreeMon.Managers
{
    public class SessionManager
    {
        readonly SystemLogger _logger;
        readonly string _connectionKey;

       
        public int SessionLength { get; set; }

        public SessionManager(string connectionKey)
        {
            _connectionKey = connectionKey;
            _logger = new SystemLogger(connectionKey);

            SessionLength = GetSessionLength();
        }

        private int GetSessionLength()
        {
            int length = 30;
         
            try
            {
                using (var context = new TreeMonDbContext(this._connectionKey))
                {
                    Setting s = context.GetAll<Setting>().FirstOrDefault(sw => (sw.Name?.EqualsIgnoreCase("SESSIONLENGTH") ?? false));
                    return StringEx.ConvertTo<int>(s?.Value ?? "30");
                }
            }
            catch (Exception ex)
            {
                _logger.InsertError(ex.Message, "SessionManager", "GetSessionLength");
                return length;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <param name="userUUID"></param>
        /// <param name="userData">This is the User objec serialized in json</param>
        /// <returns></returns>
        public UserSession SaveSession(string ipAddress, string userUUID, string userData, bool persistSession=false)
        {
            UserSession us = new UserSession();
            us.IsPersistent = persistSession;
            us.Issued = DateTime.UtcNow;
            us.AuthToken = GenerateToken(ipAddress);
            us.UserData = userData;
            us.UserUUID = userUUID;
            return SaveSession(us);
        }

        /// <summary>
        /// Saves user session, expects the authtoken to
        /// not be null or empty. If authtoken is null it uses
        /// the user id instead of ip to generate it.
        /// </summary>
        /// <param name="us"></param>
        /// <returns></returns>
        public UserSession SaveSession(UserSession us)
        {
            if (us == null)
                return us;

            if (string.IsNullOrWhiteSpace(us.AuthToken))
            {   
                Debug.Assert(false, "AUTHTOKEN IS NULL");
                us.AuthToken = GenerateToken(us.UserUUID);
            }
            if (us.SessionLength != SessionLength)
                us.SessionLength = this.SessionLength;

            us.Expires = DateTime.UtcNow.AddMinutes(us.SessionLength);
            using (var context = new TreeMonDbContext(_connectionKey))
            {
                if (context.Insert<UserSession>(us))
                    return us;
            }
             _logger.InsertError("Failed to save session." + us.UserName, "SessionManager", "SaveSession");
             return null;
        }

        public bool Update(UserSession us)
        {
            if (us == null)
                return false;
            using (var context = new TreeMonDbContext(_connectionKey))
            {
                return context.Update<UserSession>(us) > 0 ? true : false;
            }
        }

        /// <summary>
        ///  Function to generate unique token based on provided srcValue
        ///  Uses ToSafeString to return only alphanum string.
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <returns></returns>
        public string GenerateToken(string ipAddress )
        {
            string token = "";

            using (SHA512 sha = new SHA512Managed())
            {
                token =  Convert.ToBase64String(sha.ComputeHash(Encoding.UTF8.GetBytes(ipAddress + DateTime.Now.ToLongTimeString())));
                token = token.ToSafeString(true);
            }
            return token;
        }

        /// <summary>
        /// Method to validate token against expiry and existence in database.
        /// </summary>
        /// <param name="tokenId"></param>
        /// <returns></returns>
        public bool IsValidSession(string authToken) 
        {
            if (string.IsNullOrEmpty(authToken))
                return false;

            UserSession us = null;
            using (var context = new TreeMonDbContext(_connectionKey))
            {
                try
                {
                    us = context.GetAll<UserSession>().OrderByDescending(ob => ob.Issued).FirstOrDefault(w => w.AuthToken == authToken);
                }
                catch (Exception ex)
                {
                    _logger.InsertError(ex.Message, "SessionManager", "IsValidSession");
                    Debug.Assert(false, ex.Message);
                    return false;
                }

                if (us == null)
                    return false;

                 double secondsLeft =  us.Expires.Subtract( DateTime.UtcNow ).TotalSeconds;

                if (us.IsPersistent == false && secondsLeft <= 0 )
                {
                    context.Delete<UserSession>(us);
                    return false;
                }

                if (us.SessionLength != SessionLength)
                    us.SessionLength = this.SessionLength;
                //This adds time to the session so it won't cut them off while they're working (non idle user).
                us.Expires = DateTime.UtcNow.AddMinutes(us.SessionLength);
                context.Update<UserSession>(us);
            }
            return true;
        }

        public UserSession GetSessionByUser( string userUUID)
        {
            if (string.IsNullOrEmpty(userUUID))
                return new UserSession();
            using (var context = new TreeMonDbContext(_connectionKey))
            {
                return context.GetAll<UserSession>().FirstOrDefault(w => w.UserUUID == userUUID);
            }
        }

        public UserSession GetSessionByUserName(string userName)
        {
            if (string.IsNullOrEmpty(userName))
                return new UserSession();
            using (var context = new TreeMonDbContext(_connectionKey))
            {
                return context.GetAll<UserSession>().FirstOrDefault(w => (w.UserName?.EqualsIgnoreCase(userName)?? false));
            }
        }

        public UserSession GetSession(string authToken, bool validate = true)
        {
            try {
                using (var context = new TreeMonDbContext(_connectionKey))
                {
                    if (!validate)
                        return context.GetAll<UserSession>().OrderByDescending(ob => ob.Issued).FirstOrDefault(w => w.AuthToken == authToken);

                    if (!IsValidSession(authToken))
                    {
                        return null;
                    }

                    return context.GetAll<UserSession>().OrderByDescending(ob => ob.Issued).FirstOrDefault(w => w.AuthToken == authToken);
                }
            }
            catch(Exception ex)
            {
                _logger.InsertError(ex.Message, "SessionManager", "GetSession");
                Debug.Assert(false, ex.Message);
            }
            return null;
        }

        /// <summary>
        /// Method to kill the provided token id.
        /// </summary>
        /// <param name="tokenId">true for successful delete</param>
        public bool DeleteSession(string tokenId)
        {
            DynamicParameters p = new DynamicParameters();
            p.Add("@TOKEN", tokenId);
            using (var context = new TreeMonDbContext(_connectionKey))
            {
                return context.Delete<UserSession>("WHERE AuthToken=@TOKEN", p) > 0 ? true : false;
            }
        }

        /// <summary>
        /// Delete tokens for the specific deleted user
        /// </summary>
        /// <param name="userUUID"></param>
        /// <returns>true for successful delete</returns>
        public bool DeleteByUserId(string userUUID)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@USERID", userUUID);
            using (var context = new TreeMonDbContext(_connectionKey))
            {
                return context.Delete<UserSession>("WHERE UserUUID=@USERID", parameters) > 0 ? true : false;
            }
        }

        public int ClearExpiredSessions(int minutes)
        {
            if (minutes > 0) {
                minutes = minutes * -1;
            }

            int deletedSessions = 0;
            DateTime expireDate = DateTime.UtcNow.AddMinutes(minutes);
            try
            {
                List<UserSession> sessions;

                using (var context = new TreeMonDbContext(_connectionKey))
                {
                    sessions = context.GetAll<UserSession>().
                             Where(w => w.Issued < expireDate &&
                                   w.IsPersistent == false)?.ToList();
                }
                if (sessions == null)
                    return 0;

                foreach (UserSession s in sessions)
                {
                    if (DeleteSession(s.AuthToken))
                        deletedSessions++;
                }
            }
            catch (Exception ex) {
                _logger.InsertError(ex.Message, "SessionManager", MethodInfo.GetCurrentMethod().Name);
            }
            return deletedSessions;
        }

        public string GetAccountUUID(string sessionKey)
        {
            UserSession us = GetSession(sessionKey);
            User u = JsonConvert.DeserializeObject<User>(us.UserData);
            return u?.AccountUUID;
        }
    }
}
