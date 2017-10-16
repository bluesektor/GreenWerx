// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
using Omni.Managers.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using TreeMon.Data;
using TreeMon.Data.Logging;
using TreeMon.Data.Logging.Models;
using TreeMon.Managers.Documents;
using TreeMon.Models;
using TreeMon.Models.App;
using TreeMon.Models.Flags;
using TreeMon.Models.Logging;
using TreeMon.Models.Membership;
using TreeMon.Models.Services;
using TreeMon.Utilites.Extensions;
using TreeMon.Utilites.Security;

namespace TreeMon.Managers.Membership
{
    public  class UserManager : BaseManager,ICrud
    {
        private RoleManager _roleManager = null;
        SystemLogger _logger = null;


        public UserManager(string connectionKey, string sessionKey) : base(connectionKey, sessionKey)
        {
            Debug.Assert(!string.IsNullOrWhiteSpace(connectionKey), "UserManager CONTEXT IS NULL!");

            this._connectionKey = connectionKey;

            _logger = new SystemLogger(connectionKey);
            _roleManager = new RoleManager(connectionKey,this.GetUser(SessionKey));
        }

        public ServiceResult AuthenticateUser(string userName, string password, string ip)
        {
            if (string.IsNullOrWhiteSpace(userName))
            {
                using (var context = new TreeMonDbContext(this._connectionKey))
                {
                    context.Insert<AuthenticationLog>(new AuthenticationLog() { Authenticated = false, AuthenticationDate = DateTime.UtcNow, IPAddress = ip, FailType = "Blank Username", UserName = userName, UUIDType = "AuthenticationLog", Vector = "UserManager.AuthenticateUser" });
                }
                return ServiceResponse.Error("Invalid username or password.");
            }

            User user = null;
            try
            {
                using (var context = new TreeMonDbContext(this._connectionKey))
                {
                 
                    user = context.GetAll<User>().FirstOrDefault(uw => (uw.Name?.EqualsIgnoreCase(userName)?? false));

                    if (user == null)
                        return ServiceResponse.Error("Invalid username or password.");

                    user.LastActivityDate = DateTime.UtcNow;

                    if (!PasswordHash.ValidatePassword(password, user.PasswordHashIterations + ":" + user.PasswordSalt + ":" + user.Password))
                    {
                        context.Insert<AuthenticationLog>(new AuthenticationLog() {
                            Authenticated = false,
                            AuthenticationDate = DateTime.UtcNow,
                            IPAddress = ip, FailType = "Invalid Password",
                            UserName = userName, UUIDType = "AuthenticationLog",
                            Vector = "UserManager.AuthenticateUser",
                             DateCreated = DateTime.UtcNow
                        });
                        user.FailedPasswordAttemptCount++;
                        context.Update<User>(user);
                        return ServiceResponse.Error("Invalid username or password.");
                    }

                    ServiceResult sr = IsUserAuthorized(user, ip);

                    if (sr.Status == "ERROR")
                        return sr;

                    user.Online = true;
                    user.LastLoginDate = DateTime.UtcNow;

                    context.Update<User>(user);
                }
            }
            catch (Exception ex)
            {
                _logger.InsertError(ex.Message, "UserManager", "AuthenticateUser:" + userName + " IP:" +ip);

                return ServiceResponse.Error(ex.Message);
            }

            return ServiceResponse.OK();
        }

        /// <summary>
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="ip"></param>
        /// <returns></returns>
        public async Task<ServiceResult> AuthenticateUserAsync(string userName, string password, string ip)
        {
            if (string.IsNullOrWhiteSpace(userName))
            {
                using (var context = new TreeMonDbContext(this._connectionKey))
                {
                    await context.InsertAsync<AuthenticationLog>(new AuthenticationLog() { Authenticated = false, AuthenticationDate = DateTime.UtcNow, IPAddress = ip, FailType = "Blank Username", UserName = userName, UUIDType = "AuthenticationLog", Vector = "UserManager.AuthenticateUser" });
                }
                return ServiceResponse.Error("Invalid username or password.");
            }
            
            User user = null;
            try
            {
                using (var context = new TreeMonDbContext(this._connectionKey))
                {
                    user = (await context.GetAllAsync<User>()).FirstOrDefault(uw => (uw.Name?.EqualsIgnoreCase(userName)??false));

                    if (user == null)
                        return ServiceResponse.Error("Invalid username or password.");

                    user.LastActivityDate = DateTime.UtcNow;

                    if (!PasswordHash.ValidatePassword(password, user.PasswordHashIterations + ":" + user.PasswordSalt + ":" + user.Password))
                    {
                        context.Insert<AuthenticationLog>(new AuthenticationLog() { Authenticated = false, AuthenticationDate = DateTime.UtcNow, IPAddress = ip, FailType = "Invalid Password", UserName = userName, UUIDType = "AuthenticationLog", Vector = "UserManager.AuthenticateUser" });
                        user.FailedPasswordAttemptCount++;
                        context.Update<User>(user);
                        return ServiceResponse.Error("Invalid username or password.");
                    }

                    ServiceResult sr = IsUserAuthorized(user, ip);

                    if (sr.Status == "ERROR")
                        return sr;

                    user.Online = true;
                    user.LastLoginDate = DateTime.UtcNow;

                    await context.UpdateAsync<User>(user);
                }
            }
            catch (Exception ex)
            {
                _logger.InsertError(ex.Message, "UserManager", "AuthenticateUserAsync:" + userName + " IP:" + ip);

                return ServiceResponse.Error(ex.Message);
            }

            return ServiceResponse.OK();
        }

        public User ClearSensitiveData(User u)
        {
            if (u == null)
                return u;

            u.Password = "";
            u.PasswordAnswer = "";
            u.MobilPin = "";
            u.PasswordHashIterations = 0;
            u.PasswordSalt = "";
            u.ProviderUserKey = "";
            u.UserKey = "";
            return u;
        }

        public List<User> ClearSensitiveData(List<User> users)
        {
            users.ForEach(am => {
              
                am.Password = "";
                am.PasswordAnswer = "";
                am.MobilPin = "";
                am.PasswordHashIterations = 0;
                am.PasswordSalt = "";
                am.ProviderUserKey = "";
                am.UserKey = "";
            });

            return users;
        }

        public async Task<ServiceResult> InsertUserAsync(User u, string ipAddress = "", bool ignoreAuthorization = true )
        {
            if (u == null)
                return ServiceResponse.Error("Invalid user object.");

            if (!ignoreAuthorization) //we ignore authorization when a new user is registering.
            {
                if (!this.DataAccessAuthorized(u, "POST", false)) return ServiceResponse.Error("You are not authorized this action.");
            }

            User dbU;
            using (var context = new TreeMonDbContext(this._connectionKey))
            {
                dbU = context.GetAll<User>().FirstOrDefault(wu => (wu.Name?.EqualsIgnoreCase(u.Name)??false));

                if (dbU != null)
                    return ServiceResponse.Error("Username already exists.");

                u.UUID = Guid.NewGuid().ToString("N");
                u.UUIDType = "User";
                u.DateCreated = DateTime.UtcNow;
                u.LastActivityDate = DateTime.UtcNow;

                if (await context.InsertAsync<User>(u) > 0)
                    return ServiceResponse.OK("",u);
            }
            return ServiceResponse.Error("Database failed to save user information.");
        }

        public ServiceResult Insert(INode n , bool ignoreAuthorization)
        {
            return this.Insert(n, "", ignoreAuthorization);
        }

        public  ServiceResult Insert(INode n, string ipAddress = "", bool ignoreAuthorization = false)
        {

            if (n == null)
                return ServiceResponse.Error("No record sent.");

            n.Initialize(this._requestingUser.UUID, this._requestingUser.AccountUUID, 1);

            var u = (User)n;

            if (!ignoreAuthorization) //we ignore authorization when a new user is registering.
            {
                if (!this.DataAccessAuthorized(u, "POST", false)) return ServiceResponse.Error("You are not authorized this action.");
            }
            User dbU = new User();
            using (var context = new TreeMonDbContext(this._connectionKey))
            {
                dbU = context.GetAll<User>().FirstOrDefault(wu => (wu.Name?.EqualsIgnoreCase(u.Name)?? false));

                if (dbU != null)
                    return ServiceResponse.Error("Username already exists.");

 
                u.LastActivityDate = DateTime.UtcNow;

                if (context.Insert<User>(u))
                    return ServiceResponse.OK("",u);
            }
            return ServiceResponse.Error("Database failed to save user information.");
        }

        public ServiceResult Delete(string userUUID,   bool purge = false)
        {
            try
            {
                User u = new User();
                using (var context = new TreeMonDbContext(this._connectionKey))
                {
                    u = context.GetAll<User>().FirstOrDefault(w => w.UUID == userUUID);
                    if (u == null)
                        return ServiceResponse.Error("User not found.");

                    if (!this.DataAccessAuthorized(u, "DELETE", false)) return ServiceResponse.Error("You are not authorized this action.");

                    if (!DataAccessAuthorized(u,"delete", false))
                        return ServiceResponse.Error("You are not authorized access to ." + u.Name);

                    if (!purge)
                    {
                        u.Deleted = true;
                        return Update(u);
                    }
                    context.Delete<User>(u);
                }
            }
            catch (Exception ex)
            {
                _logger.InsertError(ex.Message, "UserManager", "DeleteUser:" + userUUID);

                Debug.Assert(false, ex.Message);
                return ServiceResponse.Error(ex.Message);
            }
            return ServiceResponse.OK();
        }

        public ServiceResult Delete(INode n, bool purge = false)
        {
            if (n == null || string.IsNullOrWhiteSpace(n.UUID))
                return ServiceResponse.Error("You must pass in a valid user.");

            var user = (User)n;

            try
            {
                if (!this.DataAccessAuthorized(user, "DELETE", false)) return ServiceResponse.Error("You are not authorized this action.");

                User u = new User();
                using (var context = new TreeMonDbContext(this._connectionKey))
                {
                    if (!purge)
                    {
                        u.Deleted = true;
                        return Update(u);
                    }
                    context.Delete<User>(u);
                }
            }
            catch (Exception ex)
            {
                _logger.InsertError(ex.Message, "UserManager", "DeleteUser:" + user.UUID);
                Debug.Assert(false, ex.Message);
                return ServiceResponse.Error(ex.Message);
            }
            return ServiceResponse.OK();
        }

        public INode Get(string name)
        {
            return this.Get(name, true);
        }

        public INode Get( string userName, bool clearSensitiveData)
        {
            //   if (!this.DataAccessAuthorized(u, "GET", false)) return null; //todo fix this so log it or something

            User u;

            using (var context = new TreeMonDbContext(this._connectionKey))
            {
                u = context.GetAll<User>().FirstOrDefault(uw => ( uw.Name?.EqualsIgnoreCase(userName)?? false));
            }
            if (clearSensitiveData)
            {
                u = ClearSensitiveData(u);
            }

    
            return u;
        }

        public User GetUserByEmail(string email, bool clearSensitiveData = true)
        {
            if (string.IsNullOrWhiteSpace(email))
                return new User();

            // if (!this.DataAccessAuthorized(u, "GET", false)) return ServiceResponse.Error("You are not authorized this action.");
            using (var context = new TreeMonDbContext(this._connectionKey))
            {
                if (clearSensitiveData)
                    return ClearSensitiveData(context.GetAll<User>().FirstOrDefault(uw => (uw.Email?.EqualsIgnoreCase(email)??false)));

                return context.GetAll<User>().FirstOrDefault(uw => (uw.Email?.EqualsIgnoreCase(email)??false));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="AccountUUID">This is technically accountUUID</param>
        /// <param name="clearSensitiveData"></param>
        /// <returns></returns>
        public List<User> GetUsers(string AccountUUID, bool clearSensitiveData = true)
        {
            // if (!this.DataAccessAuthorized(u, "GET", false)) return ServiceResponse.Error("You are not authorized this action.");
            List<User> usrs = new List<User>();
            using (var context = new TreeMonDbContext(this._connectionKey))
            {
                if (clearSensitiveData)
                    return ClearSensitiveData(context.GetAll<User>().Where(w => w.AccountUUID == AccountUUID && w.Deleted == false).ToList());

                return context.GetAll<User>().Where(w => w.AccountUUID == AccountUUID && w.Deleted == false).ToList();
            }
        }

        public List<User> GetUsers(bool clearSensitiveData = true)
        {
            // if (!this.DataAccessAuthorized(u, "GET", false)) return ServiceResponse.Error("You are not authorized this action.");
            List<User> usrs = new List<User>();
            using (var context = new TreeMonDbContext(this._connectionKey))
            {
                if (clearSensitiveData)
                {
                    usrs = ClearSensitiveData(context.GetAll<User>().Where(w => w.Deleted == false).ToList());
                    return usrs;
                }

                return context.GetAll<User>().Where(w => w.Deleted == false).ToList();
            }
        }

        public INode GetBy(string UUID)
        {
            return this.GetBy(UUID, true);
        }

        public INode GetBy(string UUID, bool clearSensitiveData = true)
        {
            // if (!this.DataAccessAuthorized(u, "GET", false)) return ServiceResponse.Error("You are not authorized this action.");
            using (var context = new TreeMonDbContext(this._connectionKey))
            {
                if (clearSensitiveData)
                    return ClearSensitiveData(context.GetAll<User>().Where(uw => uw.UUID == UUID).FirstOrDefault());

                return context.GetAll<User>().FirstOrDefault(uw => uw.UUID == UUID);
            }
        }

        public ServiceResult SetUserFlag(string userUUID, string userFlag, string flagValue, bool authorize = true)
        {
            User u;

            using (var context = new TreeMonDbContext(this._connectionKey))
            {
                u = context.GetAll<User>().FirstOrDefault(uw => uw.UUID == userUUID);
            }

            if (u == null)
                return ServiceResponse.Error("User not found.");

            if (authorize)
            {
                if (!this.DataAccessAuthorized(u, "PATCH", false)) return ServiceResponse.Error("You are not authorized this action.");
            }

            if (string.IsNullOrWhiteSpace(userFlag ) || string.IsNullOrWhiteSpace(flagValue))
                return ServiceResponse.Error("You are missing a flag or value to set.");

            switch (userFlag.ToUpper())
            {  // CTRL+SHIFT+U. to toggle case
                case "PROVIDERNAME":
                    u.ProviderName = flagValue;
                    break;
                case "PROVIDERUSERKEY":
                    u.ProviderUserKey = flagValue;
                    break;
                case "ACTIVE":
                    u.Active = StringEx.ConvertTo<bool>(flagValue); 
                    break;
                case "DELETED":
                    u.Deleted = StringEx.ConvertTo<bool>(flagValue);
                    break;
                case "PRIVATE":
                    u.Private = StringEx.ConvertTo<bool>(flagValue);
                    break;
                case "ANONYMOUS":
                    u.Anonymous = StringEx.ConvertTo<bool>(flagValue);
                    break;
                case "APPROVED":
                    u.Approved = StringEx.ConvertTo<bool>(flagValue);
                    break;
                case "BANNED":
                    u.Banned = StringEx.ConvertTo<bool>(flagValue);
                    break;
                case "LOCKEDOUT":
                    u.LockedOut = StringEx.ConvertTo<bool>(flagValue);
                    if (u.LockedOut == true)
                    {
                        u.LastLockoutDate = DateTime.UtcNow;
                    }
                    break;
                case "ONLINE":
                    u.Online = StringEx.ConvertTo<bool>(flagValue);
                    break;
                //case "SITEADMIN": break;// backlog this requires security validation before implementing since its a very powerfull account
                default:
                    return ServiceResponse.Error("Invalid flag.");
            }
            u.LastUpdatedDate = DateTime.UtcNow;
            using (var context = new TreeMonDbContext(this._connectionKey))
            {
                if (context.Update<User>(u) < 0)
                    return ServiceResponse.Error("Error occurred saving user " + u.Name);
            }

            _logger.InsertInfo("userUUID:" + userUUID + ",userFlag:" + userFlag + ",flagValue:" + flagValue, "UserManager", "SetUserFlag");

            return ServiceResponse.OK("User " + u.Name + " updated.");
        }

        public ServiceResult Update(INode n)
        {
            return this.Update(n, true);
        }

        public ServiceResult Update(INode n, bool authorize = true)
        {
            if (n == null)
                return ServiceResponse.Error("No record was sent.");

            var u = (User)n;

            if (authorize)
            {
                if (!this.DataAccessAuthorized(n, "PATCH", false)) return ServiceResponse.Error("You are not authorized this action.");
            }

            using (var context = new TreeMonDbContext(this._connectionKey))
            {
                if (context.Update<User>(u) > 0)
                    return ServiceResponse.OK();
            }

            return ServiceResponse.Error("No records were updated.");
        }

        /// <summary>
        /// We log profiles instead of having a single record.
        /// This way the user can track changes.
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public ServiceResult LogUserProfile(Profile p)
        {
            if (p == null)
                return ServiceResponse.Error("Invalid profile.");

            p.DateCreated = DateTime.UtcNow;
            using (var context = new TreeMonDbContext(this._connectionKey))
            {
                if (context.Insert<Profile>(p))
                    return ServiceResponse.OK();
            }
            return ServiceResponse.Error("Failed to save profile.");
        }

        public Profile GetCurrentProfile(string userUUID)
        {
            if (string.IsNullOrWhiteSpace(userUUID))
                return null;
            using (var context = new TreeMonDbContext(this._connectionKey))
            {
                return context.GetAll<Profile>().OrderByDescending(po => po.DateCreated).FirstOrDefault(pw => pw.UserUUID == userUUID);
            }
        }

        /// <summary>
        /// This is just a basic check if they're banned, locked,
        /// approved etc.
        /// 
        /// </summary>
        /// <param name="u"></param>
        /// <returns></returns>
        public ServiceResult IsUserAuthorized(User user, string ip) 
        {
            using (var context = new TreeMonDbContext(this._connectionKey))
            {
                if (user.Deleted)
                {
                    context.Insert<AuthenticationLog>(new AuthenticationLog() { Authenticated = false, AuthenticationDate = DateTime.UtcNow, IPAddress = ip, FailType = "Deleted User", UserName = user.Name, UUIDType = "AuthenticationLog", Vector = "UserManager.AuthenticateUser" });
                    return ServiceResponse.Error("User was deleted.");
                }

                //this is set by clicking the validation email.
                if (!user.Approved)
                {
                    context.Insert<AuthenticationLog>(new AuthenticationLog() { Authenticated = false, AuthenticationDate = DateTime.UtcNow, IPAddress = ip, FailType = "Unapproved User", UserName = user.Name, UUIDType = "AuthenticationLog", Vector = "UserManager.AuthenticateUser" });
                    return ServiceResponse.Error("Account has not been approved. You must activate the account with the confirmation email before logging in. Check your spam/junk folder for the confirmation email if you have not received it. <a href=\"/Account/SendValidationEmail/?userUUID=" + user.UUID + "\">Resend Validation Email</a><br/>");
                }

                if (user.LockedOut)
                {
                    context.Insert<AuthenticationLog>(new AuthenticationLog() { Authenticated = false, AuthenticationDate = DateTime.UtcNow, IPAddress = ip, FailType = "Locked User", UserName = user.Name, UUIDType = "AuthenticationLog", Vector = "UserManager.AuthenticateUser" });
                    return ServiceResponse.Error("User is locked out.");
                }

                if (user.Banned)
                {
                    context.Insert<AuthenticationLog>(new AuthenticationLog() { Authenticated = false, AuthenticationDate = DateTime.UtcNow, IPAddress = ip, FailType = "Banned User", UserName = user.Name, UUIDType = "AuthenticationLog", Vector = "UserManager.AuthenticateUser" });
                    return ServiceResponse.Error("User is banned.");
                }
            }
            return ServiceResponse.OK();
        }

        public async Task<ServiceResult> SendEmailAsync(string ip, string toEmail,string fromEmail, string subject, string body, EmailSettings settings)
        {
            if (string.IsNullOrWhiteSpace(settings.SiteDomain))
            {
                _logger.InsertError("The  email setting site domain key is not set.", "UserManager", "SendEmail");
                return ServiceResponse.Error("An error occured when sending the message. <br/>ERROR: Site Domain configuration.");
            }

            if (string.IsNullOrWhiteSpace(settings.SiteEmail))
            {
                _logger.InsertError("The email setting SiteEmail is not set.", "UserManager", "SendEmail");
                return ServiceResponse.Error("An error occured when sending the message.");
            }

            string siteEmail = settings.SiteEmail;
            MailAddress ma = new MailAddress(siteEmail, settings.SiteDomain);
            MailMessage mail = new MailMessage();
            mail.From = ma;
            mail.ReplyToList.Add( ma );
            mail.To.Add(toEmail);
            mail.Subject = subject;
            mail.Body = body;
            mail.IsBodyHtml = true;
            SMTP mailServer = new SMTP(this._connectionKey, settings);
            return await mailServer.SendMailAsync(mail);
        }


        #region Registration Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Approved"></param>
        /// <param name="ipAddress"></param>
        /// <returns></returns>
        public async Task<ServiceResult> RegisterUserAsync(UserRegister ur, bool Approved, string ipAddress)
        {
            if (string.IsNullOrEmpty(ur.Name))
                return ServiceResponse.Error("Invalid username.");

            if (ur.Password != ur.ConfirmPassword)
                return ServiceResponse.Error("Passwords must match.");

            if (Validator.IsEmailInjectionAttempt(ur.Email))
            {
                _logger.InsertSecurity(ur.Email, "UserManager", "RegisterUser.IsEmailInjectionAttempt");
                return ServiceResponse.Error("Dangerous email format."); 
            }

            if ( !Validator.IsValidEmailFormat(ur.Email) ) { 
                return ServiceResponse.Error("Invalid email format.");
            }

            if (Validator.HasReservedLoginName(ur.Email))
            {
                _logger.InsertSecurity(ur.Email, "UserManager", "RegisterUser.HasReservedLoginName");
                return ServiceResponse.Error("Invalid email name.");
            }

            User dbUser;
            using (var context = new TreeMonDbContext(this._connectionKey))
            {
                dbUser = context.GetAll<User>().FirstOrDefault(uw => (uw.Email?.EqualsIgnoreCase(ur.Email)?? false) || (uw.Name?.EqualsIgnoreCase(ur.Name)??false));
            }

            if (dbUser != null && dbUser.Approved == false)
                return ServiceResponse.Error("The email account you provided is already on registered, but has not been validated. <br />Please check your email account and follow the instructions on the message sent.<br/><br/>Thank you,<br/> ");
         
         
            else if (dbUser != null)
                return ServiceResponse.Error("Username or email already exists.");

            if (string.IsNullOrWhiteSpace(ur.AccountUUID))
                ur.AccountUUID = SystemFlag.Default.Account;

            string tmpHashPassword = PasswordHash.CreateHash(ur.Password);
            bool approved = true;// false;TODO make this false when the email url is fixed
            //if mobile the email validation isn't going to be sent for them to validate=> approve. So auto approve.
            if (ur.ClientType == "mobile.app")
                approved = true; 

            User u = new User()
            {
                AccountUUID = ur.AccountUUID,
                Name = ur.Name,                
                Password = PasswordHash.ExtractHashPassword(tmpHashPassword),
                PasswordAnswer = ur.SecurityAnswer,
                PasswordQuestion = ur.SecurityQuestion,
                Active = true,
                DateCreated = DateTime.UtcNow,
                Deleted = false,
                PasswordSalt = PasswordHash.ExtractSalt(tmpHashPassword),
                PasswordHashIterations = PasswordHash.ExtractIterations(tmpHashPassword),
                Email = ur.Email,
                SiteAdmin = false,
                Approved = approved,
                Anonymous = false,
                Banned = false,
                LockedOut = false,
                Private = true, // Since its a site admin we'll make it private  appSettings.UserIsPrivate,
                FailedPasswordAnswerAttemptWindowStart = 0,
                FailedPasswordAttemptCount = 0,
                FailedPasswordAnswerAttemptCount = 0,
                ProviderUserKey = Cipher.RandomString(12),
                ProviderName = UserFlags.ProviderName.ValidateEmail
               
        };        

          ServiceResult res = await InsertUserAsync(u, ipAddress);

            if (res.Code != 200)
                return res;

            res.Result =  u;
            return res;
        }

        public  ServiceResult RegisterUser(UserRegister ur, bool Approved, string ipAddress)
        {
            if (string.IsNullOrEmpty(ur.Name))
                return ServiceResponse.Error("Invalid username.");

            if (ur.Password != ur.ConfirmPassword)
                return ServiceResponse.Error("Passwords must match.");

            if (Validator.IsEmailInjectionAttempt(ur.Email))
            {
                _logger.InsertSecurity(ur.Email, "UserManager", "RegisterUser.IsEmailInjectionAttempt");
                return ServiceResponse.Error("Dangerous email format.");
            }

            if (!Validator.IsValidEmailFormat(ur.Email))
            {
                return ServiceResponse.Error("Invalid email format.");
            }

            if (Validator.HasReservedLoginName(ur.Email))
            {
                _logger.InsertSecurity(ur.Email, "UserManager", "RegisterUser.HasReservedLoginName");
                return ServiceResponse.Error("Invalid email name.");
            }

            User dbUser = new User();
            using (var context = new TreeMonDbContext(this._connectionKey))
            {
                dbUser = context.GetAll<User>().FirstOrDefault(uw => (uw.Email?.EqualsIgnoreCase(ur.Email) ??false)|| (uw.Name?.EqualsIgnoreCase(ur.Name)??false));
            }

            if (dbUser != null && dbUser.Approved == false)
                return ServiceResponse.Error("The email account you provided is already on registered, but has not been validated. <br />Please check your email account and follow the instructions on the message sent.<br/><br/>Thank you,<br/> ");


            else if (dbUser != null)
                return ServiceResponse.Error("Username or email already exists.");

            string tmpHashPassword = PasswordHash.CreateHash(ur.Password);
            bool approved = false;
            //if mobile the email validation isn't going to be sent for them to validate=> approve. So auto approve.
            if (ur.ClientType == "mobile.app")
                approved = true;

            User u = new User()
            {
                //AccountUUID 
                Name = ur.Name,
                Password = PasswordHash.ExtractHashPassword(tmpHashPassword),
                PasswordAnswer = ur.SecurityAnswer,
                PasswordQuestion = ur.SecurityQuestion,
                Active = true,
                DateCreated = DateTime.UtcNow,
                Deleted = false,
                PasswordSalt = PasswordHash.ExtractSalt(tmpHashPassword),
                PasswordHashIterations = PasswordHash.ExtractIterations(tmpHashPassword),
                Email = ur.Email,
                SiteAdmin = false,
                Approved = approved,
                Anonymous = false,
                Banned = false,
                LockedOut = false,
                Private = true, // Since its a site admin we'll make it private  appSettings.UserIsPrivate,
                FailedPasswordAnswerAttemptWindowStart = 0,
                FailedPasswordAttemptCount = 0,
                FailedPasswordAnswerAttemptCount = 0,
                ProviderUserKey = Cipher.RandomString(12),
                ProviderName = UserFlags.ProviderName.ValidateEmail

            };

            return Insert(u, ipAddress,true);
        }

        //Used to resend validation email
        public async Task<ServiceResult> SendUserEmailValidationAsync(User user, string validationCode, string ipAddress, EmailSettings settings)
        {
            if (string.IsNullOrWhiteSpace(settings.SiteDomain))
                return ServiceResponse.Error("The applications site domain key is not set.");

            if (string.IsNullOrWhiteSpace(settings.SiteEmail))
                return ServiceResponse.Error("The applications site email key is not set.");

            #region build email from template
           
            string validationUrl = "http://" + settings.SiteDomain + "/membership/validate/type/mbr/operation/mreg/code/" + validationCode;
            string oopsUrl = "http://" + settings.SiteDomain + "/membership/validate/type/mbr/operation/mdel/code/" + validationCode;
           // string validationUrl = "http://" + settings.SiteDomain + "/users/validate?type=mbr&operation=mreg&code=" + validationCode;
          //  string oopsUrl = "http://" + settings.SiteDomain + "/users/validate?type=mbr&operation=mdel&code=" + validationCode;

            DocumentManager dm = new DocumentManager(this._connectionKey, SessionKey);
            ServiceResult docRes = dm.GetTemplate("EmailNewMember");
            if (docRes.Status != "OK")
                return docRes;

            string template = docRes.Result?.ToString();
            if (string.IsNullOrWhiteSpace(template))
            {
                _logger.InsertError("Email template is empty", "UserManager", "SendUserEmailValidationAsync");
                return ServiceResponse.Error("Template is empty.");
            }

            template = template.Replace("[DOMAIN]", settings.SiteDomain);
            template = template.Replace("[USERNAME]", user.Name);
            template = template.Replace("[REGISTRATION_LINK]", validationUrl);
            template = template.Replace("[UNREGISTRATION_LINK]", oopsUrl);

            #endregion

         
            MailAddress ma = new MailAddress(settings.SiteEmail, settings.SiteDomain);
            MailMessage mail = new MailMessage();
            mail.From = ma;
            mail.ReplyToList.Add(ma);
            mail.To.Add(user.Email?.Trim());
            mail.Subject = settings.SiteDomain + " account registration.";
            mail.Body = template;
            mail.IsBodyHtml = true;
            SMTP mailServer = new SMTP(this._connectionKey, settings);
            return await mailServer.SendMailAsync(mail);
        }

        public  ServiceResult SendUserEmailValidation(User user, string validationCode, string ipAddress, EmailSettings settings)
        {
            if (string.IsNullOrWhiteSpace(settings.SiteDomain))
                return ServiceResponse.Error("The applications site domain key is not set.");

            if (string.IsNullOrWhiteSpace(settings.SiteEmail))
                return ServiceResponse.Error("The applications site email key is not set.");

            #region build email from template 
            string validationUrl = "http://" + settings.SiteDomain + "/membership/validate/type/mbr/operation/mreg/code/" + validationCode;
            string oopsUrl = "http://" + settings.SiteDomain + "/membership/validate/type/mbr/operation/mdel/code/" + validationCode;
           // string validationUrl = "http://" + settings.SiteDomain + "/users/validate?type=mbr&operation=mreg&code=" + validationCode;
           // string oopsUrl = "http://" + settings.SiteDomain + "/users/validate?type=mbr&operation=mdel&code=" + validationCode;

            DocumentManager dm = new DocumentManager(this._connectionKey, SessionKey);
            ServiceResult docRes = dm.GetTemplate("EmailNewMember");
            if (docRes.Status != "OK")
                return docRes;

            string template = docRes.Result?.ToString();

            if (string.IsNullOrWhiteSpace(template))
                return ServiceResponse.Error("Unable to locate email template.");

            template = template.Replace("[DOMAIN]", settings.SiteDomain);
            template = template.Replace("[USERNAME]", user.Name);
            template = template.Replace("[REGISTRATION_LINK]", validationUrl);
            template = template.Replace("[UNREGISTRATION_LINK]", oopsUrl);

            #endregion

     

            MailAddress ma = new MailAddress(settings.SiteEmail, settings.SiteDomain);
            MailMessage mail = new MailMessage();
            mail.From = ma;
            mail.ReplyToList.Add(ma);
            mail.To.Add(user.Email);
            mail.Subject = settings.SiteDomain + " account registration.";
            mail.Body = template;
            mail.IsBodyHtml = true;
            SMTP mailServer = new SMTP(this._connectionKey, settings);
            return  mailServer.SendMail(mail);
        }


        public async Task<ServiceResult> SendPasswordResetEmailAsync(User user, string ipAddress, EmailSettings settings)
        {
            if (string.IsNullOrWhiteSpace(settings.SiteDomain))
                return ServiceResponse.Error("The applications site domain key is not set.");

            if (string.IsNullOrWhiteSpace(settings.SiteEmail))
                return ServiceResponse.Error("The applications site email key is not set.");


            #region build email from template backlog make function

            string validationUrl = "http://" + settings.SiteDomain + "/membership/validate/type/mbr/operation/pwdr/code/" + user.ProviderUserKey;
            string oopsUrl = "http://" + settings.SiteDomain + "/membership/validate/type/mbr/operation/mdel/code/" + user.ProviderUserKey;

            DocumentManager dm = new DocumentManager(this._connectionKey,SessionKey);
            ServiceResult docRes = dm.GetTemplate("PasswordResetEmail");
            if (docRes.Status != "OK")
                return docRes;

            string template = docRes.Result?.ToString();

            template = template.Replace("[DOMAIN]", settings.SiteDomain);
            template = template.Replace("[USERNAME]", user.Name);
            template = template.Replace("[UPDATEPASSWORD_LINK]", validationUrl);
            template = template.Replace("[UNREGISTRATION_LINK]", oopsUrl);

            #endregion

          
            MailAddress ma = new MailAddress(settings.SiteEmail, settings.SiteDomain);
            MailMessage mail = new MailMessage();
            mail.From = ma;
            mail.ReplyToList.Add(ma);
            mail.To.Add(user.Email);
            mail.Subject = settings.SiteDomain + " Password Reset.";
            mail.Body = template;
            mail.IsBodyHtml = true;
            SMTP mailServer = new SMTP(this._connectionKey, settings);
            return await mailServer.SendMailAsync(mail);
        }


        public async Task<ServiceResult> SendUserInfoAsync(User user, string ipAddress, EmailSettings settings)
        {
            if (string.IsNullOrWhiteSpace(settings.SiteDomain))
                return ServiceResponse.Error("The applications site domain key is not set.");

            if (string.IsNullOrWhiteSpace(settings.SiteEmail))
                return ServiceResponse.Error("The applications site email key is not set.");

            #region build email from template 
            //users/validate/type/:type/operation/:operation/code/:code
            string validationUrl = "http://" + settings.SiteDomain + "/membership/validate/type/mbr/operation/pwdr/code/" + user.ProviderUserKey;
            string oopsUrl = "http://" + settings.SiteDomain + "/membership/validate/type/mbr/operation/mdel/code/" + user.ProviderUserKey;
           // string validationUrl = "http://" + settings.SiteDomain + "/users/validate?type=mbr&operation=pwdr&code=" + user.ProviderUserKey;
            //string oopsUrl = "http://" + settings.SiteDomain + "/users/validate?type=mbr&operation=mdel&code=" + user.ProviderUserKey;

            DocumentManager dm = new DocumentManager(this._connectionKey, SessionKey);
            ServiceResult docRes = dm.GetTemplate("UserInfoEmail");
            if (docRes.Status != "OK")
                return docRes;

            string template = docRes.Result?.ToString();

            template = template.Replace("[DOMAIN]", settings.SiteDomain);
            template = template.Replace("[USERNAME]", user.Name);
            template = template.Replace("[UPDATEPASSWORD_LINK]", validationUrl);
            template = template.Replace("[UNREGISTRATION_LINK]", oopsUrl);

            #endregion

      
            MailAddress ma = new MailAddress(settings.SiteEmail, settings.SiteDomain);
            MailMessage mail = new MailMessage();
            mail.From = ma;
            mail.ReplyToList.Add(ma);
            mail.To.Add(user.Email);
            mail.Subject = settings.SiteDomain + "  Account Information";
            mail.Body = template;
            mail.IsBodyHtml = true;
            SMTP mailServer = new SMTP(this._connectionKey, settings);
            return await mailServer.SendMailAsync(mail);
        }


        public ServiceResult Validate(string type, string action, string validationCode, string ipAddress, User requestingUser)
        {
            if (!StringEx.IsSQLSafeString(type + " " + action + " " + validationCode))
            {
                _logger.InsertSecurity(type + " " + action + " " + validationCode, ipAddress, "Validate.IsSafeString:validationCode");
                return ServiceResponse.Error("Error processing request, invalid url.");
            }

            User user = new User();
            try
            {
                user = this.GetUsers(false).FirstOrDefault(dd => dd.ProviderUserKey == validationCode &&
                                                            (dd.ProviderName == UserFlags.ProviderName.ValidateEmail ||
                                                             dd.ProviderName == UserFlags.ProviderName.SendAccountInfo ||
                                                             dd.ProviderName == UserFlags.ProviderName.ForgotPassword));
            }
            catch (Exception ex)
            {
                _logger.InsertError(ex.Message, "UserManager", "Validate.GetUsers");
                return ServiceResponse.Error("Error occured retrieving user.");
            }

            if (user == null)
                return ServiceResponse.Error("Invalid verification code.");

            if (user.Banned)
                return ServiceResponse.Error("Account is banned.");

            string typeAction = string.Format("{0}_{1}", type, action);

            if (user.Approved && typeAction != "mbr_mdel")// The email account you provided to subscribe is already on file and activated, but if mdel is in the action the user wants to delete account
                return ServiceResponse.OK("Account is approved.");//may have been set by an admin. 

            user.LastActivityDate = DateTime.UtcNow;

            //Validate to subscribe.
            switch (typeAction)
            {
                case "mbr_mreg": //member subscribe.
                    user.Approved = true;
                    user.ProviderName = "";
                    user.ProviderUserKey = "";
                    //Add user to customer role by default.
                    //
                    RoleManager rm = new RoleManager(this._connectionKey, user);
                    Role customer = (Role)rm.GetRole("customer", user.AccountUUID);
                    
                    if (customer == null)
                    {
                        _logger.InsertError("Failed to get role customer!", "UserManager", "ValidateAsync.mbr_mreg");
                        return ServiceResponse.Error("Error saving role information. Try again later.");
                    }
                    else {
                        rm.AddUserToRole(customer.UUID, user, requestingUser);  
                    }
                    return Update(user);

                case "mbr_mdel": //membership oops/remove
                    user.ProviderName = "";
                    user.ProviderUserKey = "";
                    user.Email = string.Empty;
                    Update(user);
                    //don't purge or it may break internal references.
                    return this.Delete(user, false);

                case "mbr_pwdr"://password reset
                    return Update(user);
                default:
                    return ServiceResponse.Error("Invalid code.");
            }
            #endregion
        }
    }
}
