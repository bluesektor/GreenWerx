// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;
using TreeMon.Data.Logging;
using TreeMon.Managers.Membership;
using TreeMon.Managers.Store;

using TreeMon.Models.App;
using TreeMon.Models.Datasets;
using TreeMon.Models.Flags;
using TreeMon.Models.Membership;
using TreeMon.Models.Services;
using TreeMon.Utilites.Extensions;
using TreeMon.Utilites.Helpers;
using TreeMon.Utilites.Security;
using TreeMon.Web.api.Helpers;
using TreeMon.Web.Filters;
using TreeMon.Web.Models;
using WebApiThrottle;

namespace TreeMon.Web.api.v1
{
    public class AccountsController : ApiBaseController
    {
        public AccountsController()
        {
        }

        [ApiAuthorizationRequired(Operator =">=" , RoleWeight = 4)]
        [HttpPost]
        [Route("api/Accounts/Add")]
        [Route("api/Accounts/Insert")]
        public ServiceResult Insert(Account n)
        {
            AccountManager accountManager = new AccountManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);
            return accountManager.Insert(n);
        }


        [ApiAuthorizationRequired(Operator =">=" , RoleWeight = 4)]
        [HttpPost]
        [Route("api/Accounts/{accountUUID}/Users/Add")]
        public ServiceResult AddUsersToAccount(string accountUUID)
        {
            if (CurrentUser == null)
                return ServiceResponse.Error("You must be logged in to access this function.");

            ServiceResult res;
            try
            {
                Task<string> content = ActionContext.Request.Content.ReadAsStringAsync();
                if (content == null)
                    return ServiceResponse.Error("No users were sent.");

                string body = content.Result;

                if (string.IsNullOrEmpty(body))
                    return ServiceResponse.Error("No users were sent.");

                List<User> users = JsonConvert.DeserializeObject<List<User>>(body); 
                List<AccountMember> members = new List<AccountMember>();

                foreach (var user in users)
                {
                    members.Add(new AccountMember { AccountUUID = accountUUID, MemberType = "User", MemberUUID = user.UUID, RoleWeight = 1 });
                }

                AccountManager accountManager = new AccountManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);
                res = accountManager.AddUsersToAccount(accountUUID, members, CurrentUser);
            }
            catch (Exception ex)
            {
                Debug.Assert(false, ex.Message);
                return ServiceResponse.Error(ex.Message);
            }
            return res;
        }

        [ApiAuthorizationRequired(Operator =">=" , RoleWeight = 4)]
        [HttpPost]
        [Route("api/Accounts/{accountUUID}/Users/{userUUID}/Add")]
        public ServiceResult AddUserToAccount(string accountUUID, string userUUID)
        {
            AccountManager accountManager = new AccountManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);
            if (CurrentUser == null)
                return ServiceResponse.Error("You must be logged in to access this function.");

            return accountManager.AddUserToAccount(accountUUID, userUUID, CurrentUser );
        }

        [ApiAuthorizationRequired(Operator =">=" , RoleWeight = 4)]
        [HttpPost]
        [HttpGet]
        [HttpDelete]
        [Route("api/Accounts/Delete")]
        public ServiceResult Delete(Account n)
        {
            if (CurrentUser == null)
                return ServiceResponse.Error("You must be logged in to access this function.");

            AccountManager accountManager = new AccountManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);
            return accountManager.Delete(n);
        }

        [ApiAuthorizationRequired(Operator =">=" , RoleWeight = 4)]
        [HttpGet]
        [HttpDelete]
        [Route("api/Accounts/{accountUUID}/Delete")]
        public ServiceResult Delete(string accountUUID)
        {
            if (CurrentUser == null)
                return ServiceResponse.Error("You must be logged in to access this function.");

            AccountManager accountManager = new AccountManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);
            return accountManager.Delete(accountUUID);
        }

        [ApiAuthorizationRequired(Operator = ">=", RoleWeight = 4)]
        [Route("api/Accounts/{name}")]
        public ServiceResult Get(string name)
        {
            AccountManager accountManager = new AccountManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);
            Account a = (Account)accountManager.Get(name);
            if (a == null)
                return ServiceResponse.Error("Invalid name.");

            return ServiceResponse.OK("", a);
        }

        [ApiAuthorizationRequired(Operator =">=" , RoleWeight = 4)]
        [Route("api/AccountsBy/{uuid}")]
        public ServiceResult GetBy(string uuid)
        {
            AccountManager accountManager = new AccountManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);
            Account a = (Account) accountManager.GetBy(uuid);
            if (a == null)
                return ServiceResponse.Error("Invalid id.");

            return ServiceResponse.OK("",a);
        }



        [ApiAuthorizationRequired(Operator =">=" , RoleWeight = 4)]
        [HttpPost]
        [HttpGet]
        [Route("api/Accounts/{accountUUID}/Permissions")]
        public ServiceResult GetAccountPermissons(string accountUUID = "", string filter = "")
        {
            if (CurrentUser == null)
                return ServiceResponse.Error("You must be logged in to access this function.");


            if (string.IsNullOrWhiteSpace(accountUUID))
                return ServiceResponse.Error("You must provide an account id to view it's members.");

            RoleManager rm = new RoleManager(Globals.DBConnectionKey,this.GetUser( Request.Headers?.Authorization?.Parameter));
            List<dynamic> permissions =rm.GetAccountPermissions(accountUUID).Cast<dynamic>().ToList();
            int count;

                            DataFilter tmpFilter = this.GetFilter(filter);
                permissions = FilterEx.FilterInput(permissions, tmpFilter, out count);
          
            return ServiceResponse.OK("",permissions, count);
        }


        /// <summary>
        /// Returns accounts user is a member of in UsersInAccounts table
        /// </summary>
        /// <returns></returns>
        [ApiAuthorizationRequired(Operator =">=" , RoleWeight = 4)]
        [HttpPost]
        [HttpGet]
        [Route("api/Accounts/")]
        public ServiceResult GetAccounts(string filter = "")
        {
            GetUser(Request.Headers?.Authorization?.Parameter);
            if (CurrentUser == null)
                return ServiceResponse.Error("You must be logged in to access this function.");

            AccountManager accountManager = new AccountManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);
            List<dynamic> accounts =accountManager.GetAccounts(CurrentUser.UUID).Cast<dynamic>().ToList();
            int count;
            DataFilter tmpFilter = this.GetFilter(filter);

            accounts = FilterEx.FilterInput(accounts, tmpFilter, out count);

            return ServiceResponse.OK("",accounts, count);
        }

        [ApiAuthorizationRequired(Operator = ">=", RoleWeight = 4)]
        [HttpPost]
        [HttpGet]
        [Route("api/AllAccounts/")]
        public ServiceResult GetAllAccounts(string filter = "")
        {
             AccountManager accountManager = new AccountManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);

            var accounts = accountManager.GetAllAccounts().Cast<dynamic>().ToList();
            int count;
            DataFilter tmpFilter = this.GetFilter(filter);
            accounts = FilterEx.FilterInput(accounts, tmpFilter, out count);

            accounts = accounts.Select(s => new
            {
                Name = s.Name,
                UUID = s.UUID,
                SyncKey = s.SyncKey
            }).Cast<dynamic>().ToList();

            return ServiceResponse.OK("", accounts, count);
        }

        [ApiAuthorizationRequired(Operator =">=" , RoleWeight = 4)]
        [HttpPost]
        [HttpGet]
        [Route("api/Accounts/{accountUUID}/Members")]
        public ServiceResult GetMembers(string accountUUID = "", string filter = "")
        {
            if (string.IsNullOrWhiteSpace(accountUUID))
                return ServiceResponse.Error("You must provide an account id to view it's members.");

            AccountManager accountManager = new AccountManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);
            List<dynamic> accountMembers =accountManager.GetAccountMembers(accountUUID).Cast<dynamic>().ToList();
            int count;
            DataFilter tmpFilter = this.GetFilter(filter);
            accountMembers = FilterEx.FilterInput(accountMembers, tmpFilter, out count);
            return ServiceResponse.OK("",accountMembers, count);
        }

        [ApiAuthorizationRequired(Operator =">=" , RoleWeight = 4)]
        [HttpPost]
        [HttpGet]
        [Route("api/Accounts/{accountUUID}/NonMembers")]
        public ServiceResult GetNonMembers(string accountUUID = "", string filter = "")
        {
            if (string.IsNullOrWhiteSpace(accountUUID))
                return ServiceResponse.Error("You must provide an account id to complete this action.");

            AccountManager accountManager = new AccountManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);
            List<dynamic> accountMembers = accountManager.GetAccountNonMembers(accountUUID).Cast<dynamic>().ToList();
            int count;
            DataFilter tmpFilter = this.GetFilter(filter);
            accountMembers = FilterEx.FilterInput(accountMembers, tmpFilter, out count);
            return ServiceResponse.OK("",accountMembers, count);
        }



        //and then...
        

        [AllowAnonymous]
        [HttpPost]
        [EnableThrottling(PerSecond = 1, PerHour = 10, PerDay = 100)]
        [Route("api/Accounts/Login")]
        public ServiceResult Login(LoginModel credentials)
        {
            if ( credentials == null)
                return ServiceResponse.Error("Invalid login.");

            if (string.IsNullOrWhiteSpace(credentials.UserName))
                return ServiceResponse.Error("Invalid username.");

            if (string.IsNullOrWhiteSpace(credentials.Password))
                return ServiceResponse.Error("Invalid password.");


            if (string.IsNullOrEmpty(credentials.ReturnUrl))
                credentials.ReturnUrl = "";

            AccountManager accountManager = new AccountManager(Globals.DBConnectionKey, Request.Headers.Authorization?.Parameter);
            NetworkHelper network = new NetworkHelper();
            UserManager userManager = new UserManager(Globals.DBConnectionKey, Request.Headers.Authorization?.Parameter);
            RoleManager   roleManager = new RoleManager(Globals.DBConnectionKey, this.GetUser(Request.Headers.Authorization?.Parameter));
            LocationManager lm = new LocationManager(Globals.DBConnectionKey,Request.Headers.Authorization?.Parameter);

            string ipAddress = network.GetClientIpAddress(this.Request);
            string userName = credentials.UserName;
            string accountName = "";
            User user = null;
            #region

            //User user = null;
            if (userName.Contains("/"))
            {
                accountName = userName.Split('/')[0];
                userName = userName.Split('/')[1];
                user = (User)userManager.Get(userName, false);
                if (user == null)
                    return ServiceResponse.Error("Invalid username or password.");
                string accountUUID = accountManager.Get(accountName)?.UUID;

                if(string.IsNullOrWhiteSpace(accountUUID))
                    return ServiceResponse.Error("Invalid account name " + accountName );

                if (!accountManager.IsUserInAccount(accountUUID, user.UUID))
                    return ServiceResponse.Error("You are not a member of the account.");
                if (user.AccountUUID != accountUUID)
                {
                    user.AccountUUID = accountUUID;
                    userManager.Update(user);
                }
            }
            else
            {
                user = (User)userManager.Get(userName,false);
                if (user == null)
                    return ServiceResponse.Error("Invalid username or password.");
            }
            #endregion

            ServiceResult sr = userManager.AuthenticateUser(userName, credentials.Password, ipAddress);

            if (sr.Status != "OK")
                return sr;
          
            string userJson = JsonConvert.SerializeObject(user);

            UserSession us = null;
            if (credentials.ClientType == "mobile.app")//if mobile make the session persist.
               us = SessionManager.SaveSession(ipAddress, user.UUID, userJson, true);
            else
               us = SessionManager.SaveSession(ipAddress, user.UUID, userJson, false);

            if (us == null)
                return ServiceResponse.Error("Failed to save your session.");

            Request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", us.AuthToken);
            Dashboard dashBoard = new Dashboard();
         
            dashBoard.SessionLength = Convert.ToDouble(Globals.Application.AppSetting("SessionLength", "20"));           
         
            dashBoard.Authorization = us.AuthToken;
            dashBoard.UserUUID = user.UUID;
            dashBoard.AccountUUID = user.AccountUUID;
            dashBoard.ReturnUrl = credentials.ReturnUrl;
            dashBoard.DefaultLocationUUID = lm.GetLocations(user.AccountUUID)?.FirstOrDefault(w => w.isDefault == true)?.UUID;
            dashBoard.IsAdmin =  user.SiteAdmin == true ? true : roleManager.IsUserRequestAuthorized(dashBoard.UserUUID, dashBoard.AccountUUID, "/Admin");

            return ServiceResponse.OK("",dashBoard);
        }

        [AllowAnonymous]
        [HttpPost]
        [EnableThrottling(PerSecond = 1, PerHour = 10, PerDay = 100)]
        [Route("api/Accounts/LoginAsync")]
        public async Task<ServiceResult> LoginAsync(LoginModel credentials)
        {
       
            if (!ModelState.IsValid || credentials == null)
                return ServiceResponse.Error("Invalid login.");

            if (string.IsNullOrWhiteSpace(credentials.UserName))
                return ServiceResponse.Error("Invalid username.");

            if (string.IsNullOrWhiteSpace(credentials.Password))
                return ServiceResponse.Error("Invalid password.");

            AccountManager accountManager = new AccountManager(Globals.DBConnectionKey,Request.Headers?.Authorization?.Parameter);
            NetworkHelper network = new NetworkHelper();
            UserManager userManager = new UserManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);
            RoleManager   roleManager = new RoleManager(Globals.DBConnectionKey, CurrentUser);

            string ipAddress = network.GetClientIpAddress(this.Request);

            if (string.IsNullOrEmpty(credentials.ReturnUrl))
                credentials.ReturnUrl = "";

            string userName = credentials.UserName;
            string accountName = "";
            User user = null;

            if (userName.Contains("/"))
            {
                accountName = userName.Split('/')[0];
                userName = userName.Split('/')[1];

                user = (User)userManager.Get(userName,false);
                if (user == null)
                    return ServiceResponse.Error("Invalid username or password.");

                string accountUUID = accountManager.Get(accountName)?.UUID;

                if (!accountManager.IsUserInAccount(accountUUID, user.UUID))
                    return ServiceResponse.Error("You are not a member of the account.");

                if(user.AccountUUID != accountUUID)
                {
                    user.AccountUUID = accountUUID;
                    userManager.Update(user);
                }
            }
            else
            {
                user = (User)userManager.Get(userName);
                if (user == null)
                    return ServiceResponse.Error("Invalid username or password.");
            }

            userManager = new UserManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);
            ServiceResult sr = await userManager.AuthenticateUserAsync(userName, credentials.Password, ipAddress);

            if (sr.Status != "OK")
                return sr;
           
            string userJson = JsonConvert.SerializeObject(user);

            UserSession us = null;
            if (credentials.ClientType == "mobile.app")//if mobile make the session persist.
                 us = SessionManager.SaveSession(ipAddress, user.UUID, userJson,true);
            else
                us = SessionManager.SaveSession(ipAddress, user.UUID, userJson, false);

            if (us == null)
                return ServiceResponse.Error("Server was unable to create a session, try again later.");
      
            Dashboard dashBoard = new Dashboard();

            dashBoard.Authorization = us.AuthToken;
            dashBoard.UserUUID = user.UUID;
            dashBoard.AccountUUID = user.AccountUUID;
            dashBoard.ReturnUrl = credentials.ReturnUrl;
            dashBoard.IsAdmin = roleManager.IsUserRequestAuthorized(dashBoard.UserUUID, dashBoard.AccountUUID, "/Admin");
            return ServiceResponse.OK("",dashBoard);
        }


        [AllowAnonymous]
        [HttpPost]
        [EnableThrottling( PerSecond =1, PerHour = 10, PerDay = 100)]
        [Route("api/Accounts/WpLogin")]
        public async Task<string> WPLogin(LoginModel credentials)
        {
            NetworkHelper network = new NetworkHelper();
            SystemLogger  logger = new SystemLogger(Globals.DBConnectionKey);
            
            //this is just a honeypot to capture script kiddies.
            string ipAddress = network.GetClientIpAddress(this.Request);
            await logger.InsertSecurityAsync(JsonConvert.SerializeObject(credentials), ipAddress, "api/Accounts/WpLogin");
            return "Error invalid name or password";
        }

        [HttpGet]
        [Route("api/Accounts/LogOut")]
        public ServiceResult LogOut()
        {
            string authToken = Request.Headers?.Authorization?.Parameter;

            if (string.IsNullOrWhiteSpace(authToken))
            {
                return ServiceResponse.Error("Authorization token must be sent.");
            }

            UserSession us = SessionManager.GetSession(authToken);
            if (us == null)
                return ServiceResponse.OK();

            SessionManager.DeleteSession(authToken);
   
            UserManager   userManager = new UserManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);

            //IMPORTANT!!! when you know an update is going to be used on the user internally, 
            //DO NOT CLEAR THE SENSITIVE DATA! It will make the password blank! 
            User u = (User)userManager.GetBy(us.UserUUID, false);

            if (u == null)
                return ServiceResponse.OK();

            u.Online = false;

            userManager.Update(u);

            Request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", ""); 

            return ServiceResponse.OK("Authorization:");
        }

        [AllowAnonymous]
        [HttpPost]
        [EnableThrottling(PerHour = 1, PerDay = 3)]
        [Route("api/Accounts/Register")]
        public async Task<ServiceResult> RegisterAsync(UserRegister ur)
        {
            NetworkHelper network = new NetworkHelper();
            string ip = network.GetClientIpAddress(this.Request);

            if (!ModelState.IsValid)
            {
                SystemLogger logger = new SystemLogger(Globals.DBConnectionKey);
                logger.InsertInfo(" Invalid form data.", "AccountsController", "RegisterAsync:" + ip);
               
                return ServiceResponse.Error("Invalid form data.");
            }

            UserManager   userManager = new UserManager(Globals.DBConnectionKey, Request?.Headers?.Authorization?.Parameter);

            //if (ur.ClientType != "mobile.app")
            //{ //if not the mobile app then validate the captcha
            //    UserSession us = SessionManager.GetSessionByUser(ur.Captcha?.ToUpper());
            //    if (us == null)
            //    {
            //        us = SessionManager.GetSessionByUser(ip);//in the sitecontroller the captcha doesn't know whoe the user is when registering, so we used the ip addres as the name
            //        if (us == null)
            //            return ServiceResponse.Error("Invalid session.");
            //    }
            //    if (ur.Captcha?.ToUpper() != us.Captcha?.ToUpper())
            //        return ServiceResponse.Error("Code doesn't match.");
            //}

            //if (ur.ClientType == "mobile.app")
            //    sendValidationEmail = false; //if mobile app don't send the validation email.
            ServiceResult res = await userManager.RegisterUserAsync(ur, false, ip);
            return res;
            //TODO reimplement this. Change the url sent to be encrypted.
            //if (res.Code != 200  )
            //    return res;

            //User newUser = (User)res.Result;

            //EmailSettings settings = new EmailSettings();

            //string appKey = Globals.Application.AppSetting("AppKey");
            //string emailPassword = Globals.Application.AppSetting("EmailHostPassword");
            //// var testHostPassword = Cipher.Crypt(appKey,emailPassword, false);
            //settings.HostPassword =  Globals.Application.AppSetting("EmailHostPassword");
            //settings.EncryptionKey = Globals.Application.AppSetting("AppKey");
            //settings.HostUser= Globals.Application.AppSetting("EmailHostUser");
            //settings.MailHost= Globals.Application.AppSetting("MailHost");
            //settings.MailPort = StringEx.ConvertTo<int>(Globals.Application.AppSetting("MailPort"));
            //settings.SiteDomain= Globals.Application.AppSetting("SiteDomain");
            //settings.SiteEmail= Globals.Application.AppSetting("SiteEmail");
            //settings.UseSSL= StringEx.ConvertTo<bool>(Globals.Application.AppSetting("UseSSL"));
            
            //ServiceResult emailRes = await userManager.SendUserEmailValidationAsync(newUser, newUser.ProviderUserKey, ip, settings);

            //if (emailRes.Code != 200)
            //{
            //    return ServiceResponse.OK("Registration email failed to send. Check later for email confirmation.");
            //}
            //return emailRes;
        }


        [ApiAuthorizationRequired(Operator =">=" , RoleWeight = 4)]
        [HttpPost]
        [Route("api/Accounts/{accountUUID}/Users/{userUUID}/Remove")]
        public ServiceResult RemoveUserFromAccount(string accountUUID, string userUUID)
        {
            AccountManager accountManager = new AccountManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);
            if (CurrentUser == null)
                return ServiceResponse.Error("You must be logged in to access this function.");

            return accountManager.RemoveUserFromAccount(accountUUID, userUUID);
        }

        [ApiAuthorizationRequired(Operator =">=" , RoleWeight = 4)]
        [HttpPost]
        [Route("api/Accounts/{accountUUID}/Users/Remove")]
        public ServiceResult RemoveUsersFromAccount(string accountUUID)
        {
            if (CurrentUser == null)
                return ServiceResponse.Error("You must be logged in to access this function.");

            ServiceResult res = new ServiceResult();
            try
            {
                Task<string> content = ActionContext.Request.Content.ReadAsStringAsync();
                if (content == null)
                    return ServiceResponse.Error("No users were sent.");

                string body = content.Result;

                if (string.IsNullOrEmpty(body))
                    return ServiceResponse.Error("No users were sent.");
         
                List<User> users = JsonConvert.DeserializeObject<List<User>>(body);
                List<AccountMember> members = new List<AccountMember>();

                foreach (var user in users)
                {
                    members.Add(new AccountMember { AccountUUID = accountUUID, MemberType = "User", MemberUUID = user.UUID, RoleWeight = 1 });
                }
         
                AccountManager accountManager = new AccountManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);
                res = accountManager.RemoveUsersFromAccount(accountUUID, members, CurrentUser);
            }
            catch (Exception ex)
            {
                Debug.Assert(false, ex.Message);
            }

            return res;
        }

        
        [HttpPost]
        [Route("api/Accounts/SendInfo/")]
        public async Task<ServiceResult> SendAccountValidationEmailAsync(SendAccountInfoForm frm)
        {
            if (frm == null)
                return ServiceResponse.Error("Invalid form sent to server.");

            if (string.IsNullOrWhiteSpace(frm.Email)) 
                return ServiceResponse.Error("All fields must be filled out!");

            NetworkHelper network = new NetworkHelper();
            UserManager userManager = new UserManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);

            User u = userManager.GetUserByEmail(frm.Email,false);

            if (u == null)
                return ServiceResponse.OK("Email sent.");//return ok so the client can't fish for members.

            u.ProviderUserKey = Cipher.RandomString(12);

            if (userManager.SetUserFlag(u.UUID, "PROVIDERUSERKEY", u.ProviderUserKey, false).Code != 200)
                return ServiceResponse.Error("Unable to send email.");

            if (frm.ForgotPassword)
            {
                u.ProviderName = UserFlags.ProviderName.ForgotPassword;
            }
            else
            {
                u.ProviderName = UserFlags.ProviderName.SendAccountInfo;
            }

            ServiceResult updateResult = userManager.SetUserFlag(u.UUID, "PROVIDERNAME", u.ProviderName, false);
            if (updateResult.Code != 200)
                return updateResult;
            
            EmailSettings settings = new EmailSettings();
            settings.EncryptionKey = Globals.Application.AppSetting("AppKey");
            settings.HostPassword = Globals.Application.AppSetting("EmailHostPassword");
            settings.HostUser = Globals.Application.AppSetting("EmailHostUser");
            settings.MailHost = Globals.Application.AppSetting("MailHost");
            settings.MailPort = StringEx.ConvertTo<int>(Globals.Application.AppSetting("MailPort"));
            settings.SiteDomain = Globals.Application.AppSetting("SiteDomain");
            settings.SiteEmail = Globals.Application.AppSetting("SiteEmail");
            settings.UseSSL = StringEx.ConvertTo<bool>(Globals.Application.AppSetting("UseSSL"));
            string ipAddress = network.GetClientIpAddress(this.Request);
            if (frm.ForgotPassword)
            {
                return await userManager.SendPasswordResetEmailAsync(u, ipAddress, settings);
            }
            return await userManager.SendUserInfoAsync(u,  network.GetClientIpAddress(this.Request), settings);
        }

        [ApiAuthorizationRequired(Operator =">=" , RoleWeight = 4)]
        [HttpPost]
        [HttpGet]
        [Route("api/Accounts/SetActive/{accountUUID}")]
        public ServiceResult SetActive(string accountUUID)
        {
            if (string.IsNullOrWhiteSpace(accountUUID))
                return ServiceResponse.Error("You must pass a valid account UUID to mark as default.");

            UserSession us =  SessionManager.GetSession(Request.Headers?.Authorization?.Parameter, false);
            if (us == null)
                return ServiceResponse.Error("You must log in to access this functionality.");
            
            AccountManager accountManager = new AccountManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);
            ServiceResult res =accountManager.SetActiveAccount(accountUUID, CurrentUser.UUID, CurrentUser);

            if (res.Code != 200)
                return res;

            CurrentUser.AccountUUID = accountUUID;
            us.UserData = JsonConvert.SerializeObject(CurrentUser);
            SessionManager.Update(us);
            return res;
        }

        [ApiAuthorizationRequired(Operator =">=" , RoleWeight = 4)]
        [HttpPost]
        [HttpPatch]
        [Route("api/Accounts/Update")]
        public ServiceResult Update(Account account)
        {
            if (account == null)
                return ServiceResponse.Error("No record sent.");

            AccountManager accountManager = new AccountManager(Globals.DBConnectionKey,Request.Headers?.Authorization?.Parameter);
            var dbAcct = (Account)accountManager.GetBy(account.UUID);

            if (dbAcct == null)
                return ServiceResponse.Error("Invalid account id.");

            dbAcct.Email = account.Email;
            dbAcct.Name = account.Name;
            dbAcct.Active = account.Active;
            dbAcct.Private = account.Private;
            dbAcct.SortOrder = account.SortOrder;

            return accountManager.Update(dbAcct);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="newPassword"></param>
        /// <param name="passwordConfirm"></param>
        /// <param name="userUUID"></param>
        /// <param name="confCode"></param>
        /// <param name="captcha"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/Accounts/ChangePassword/")]
        public ServiceResult ChangePassword(ChangePassword frm)
        {
            if (frm == null)
            {
                return ServiceResponse.Error("Invalid data.");
            }
   
            NetworkHelper network = new NetworkHelper();
            string ipAddress = network.GetClientIpAddress(this.Request);
            string sessionToken = "";
            User u = null;

            UserManager userManager = new UserManager(Globals.DBConnectionKey,Request.Headers?.Authorization?.Parameter);

            if (frm.ResetPassword)
            {//if a reset then the user isn't logged in, so get the user by alt means.
             //only use captcha on reset
                if (string.IsNullOrWhiteSpace(frm.ConfirmationCode))
                    return ServiceResponse.Error("Invalid confirmation code. You must use the link provided in the email in order to reset your password.");

                u = userManager.GetUsers(false).FirstOrDefault(dw => (dw.ProviderUserKey == frm.ConfirmationCode && dw.Email.EqualsIgnoreCase(frm.Email)));

                if (u == null)
                    return ServiceResponse.Error("Invalid confirmation code.");
            }
            else
            {
                if (Request.Headers.Authorization == null)
                    return ServiceResponse.Error("You must be logged in to change your password.");

                sessionToken = Request.Headers?.Authorization?.Parameter;
                u = GetUser(sessionToken);//since the user session doesn't contain the password, wi have to pull it.
                u = (User)userManager.GetBy(u.UUID, false);
            }

            if (u == null)
            {
                SessionManager.DeleteSession(sessionToken);
                return ServiceResponse.Error("Session error. If your logged in try logging out and back in.");
            }

            if (frm.NewPassword != frm.ConfirmPassword)
                return ServiceResponse.Error("Password don't match.");

            if (string.IsNullOrWhiteSpace(frm.NewPassword) || string.IsNullOrWhiteSpace(frm.ConfirmPassword))
                return ServiceResponse.Error("Password can't be empty. ");

            if (PasswordHash.CheckStrength(frm.NewPassword) < PasswordHash.PasswordScore.Medium)
                return ServiceResponse.Error("Password is too weak. ");

            if (frm.ResetPassword)
            {
                if (u.ProviderName != UserFlags.ProviderName.ForgotPassword  || u.ProviderUserKey != frm.ConfirmationCode || u.Email.EqualsIgnoreCase( frm.Email) == false )
                {//
                    string msg ="Invalid informaition posted to server";
                    SystemLogger  logger = new SystemLogger(Globals.DBConnectionKey);
                    logger.InsertSecurity(msg, "AccountController", "ChangePassword");
                    return ServiceResponse.Error("Invalid confirmation code.");
                }
            }
            else //just a user updating their password.
            {   // verify old password
                if (!PasswordHash.ValidatePassword(frm.OldPassword, u.PasswordHashIterations + ":" + u.PasswordSalt + ":" + u.Password))
                {
                    return ServiceResponse.Error("Invalid password.");
                }
            }

            ServiceResult sr = userManager.IsUserAuthorized(u, ipAddress);
            if (sr.Status == "ERROR")
                return sr;

            string tmpHashPassword = PasswordHash.CreateHash(frm.NewPassword);

            u.Password = PasswordHash.ExtractHashPassword(tmpHashPassword);
            u.PasswordHashIterations = PasswordHash.ExtractIterations(tmpHashPassword);
            u.PasswordSalt = PasswordHash.ExtractSalt(tmpHashPassword);
            u.ProviderName = "";
            u.ProviderUserKey = "";
            u.LastPasswordChangedDate = DateTime.UtcNow;

            ServiceResult updateResult = userManager.Update(u, false);
            if (updateResult.Code != 200)
                return ServiceResponse.Error("Error updating password. Try again later.");

            return ServiceResponse.OK("Password has been updated." );
        }

        [HttpGet]
        [EnableThrottling(PerHour = 1, PerDay = 3)]
        [Route("api/Accounts/ReSendValidationEmail/{userUUID}")]
        public async Task<ServiceResult> ReSendValidationEmail(string userUUID)
        {
            if (string.IsNullOrWhiteSpace(userUUID))
                return ServiceResponse.Error("Invalid user id.");

            UserManager userManager = new UserManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);
            User user =   (User)userManager.GetBy(userUUID);

            if (user == null)
                return ServiceResponse.Error("User not found!");

            NetworkHelper network = new NetworkHelper();
            string ip = network.GetClientIpAddress(this.Request);
            EmailSettings settings = new EmailSettings();
            settings.HostPassword = Globals.Application.AppSetting("EmailHostPassword");
            settings.EncryptionKey = Globals.Application.AppSetting("AppKey");
            settings.HostUser = Globals.Application.AppSetting("EmailHostUser");
            settings.MailHost = Globals.Application.AppSetting("MailHost");
            settings.MailPort = StringEx.ConvertTo<int>(Globals.Application.AppSetting("MailPort"));
            settings.SiteDomain = Globals.Application.AppSetting("SiteDomain");
            settings.SiteEmail = Globals.Application.AppSetting("SiteEmail");
            settings.UseSSL = StringEx.ConvertTo<bool>(Globals.Application.AppSetting("UseSSL"));

            ServiceResult res = await userManager.SendUserEmailValidationAsync(user, user.ProviderUserKey, ip, settings);

            if (res.Code == 200)
            {
                return ServiceResponse.Error("Verification email will be sent. Please follow the instructions in the email. Check your spam/junk folder for the confirmation email if you have not received it.");

            }
            else
            {
                return ServiceResponse.Error("Failed to resend validation email. Try again later.");
            }
        }


       
    }
}
