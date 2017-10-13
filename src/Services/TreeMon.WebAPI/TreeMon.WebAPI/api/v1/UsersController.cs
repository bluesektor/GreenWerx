// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
using AutoMapper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using TreeMon.Data.Logging;
using TreeMon.Managers.Membership;

using TreeMon.Models.App;
using TreeMon.Models.Datasets;
using TreeMon.Models.Membership;
using TreeMon.Models.Services;
using TreeMon.Utilites;
using TreeMon.Utilites.Extensions;
using TreeMon.Utilites.Helpers;
using TreeMon.Utilites.Security;
using TreeMon.Web.api.Helpers;
using TreeMon.Web.Filters;
using TreeMon.Web.Models;
using TreeMon.WebAPI.Models;
using WebApiThrottle;

namespace TreeMon.Web.api.v1
{
    public class UsersController : ApiBaseController 
    {
        private readonly NetworkHelper network = null;

        public UsersController()
        {
            network = new NetworkHelper();
        }


        //NOTE: make sure the accountUUID is set before calling this.
        //
        [ApiAuthorizationRequired(Operator = ">=", RoleWeight = 3)]
        [HttpPost]
        [Route("api/Users/AddAsync")]
        [Route("api/Users/InsertAsync")]
        public async Task<ServiceResult> InsertAsync(User n)
        {
            if (n == null)
                return ServiceResponse.Error("No user sent.");

            AccountManager ac = new AccountManager(Globals.DBConnectionKey,Request.Headers?.Authorization?.Parameter);
            UserManager userManager = new UserManager(Globals.DBConnectionKey,Request.Headers?.Authorization?.Parameter);

            ServiceResult res = await userManager.InsertUserAsync((User)n, GetClientIpAddress(Request));

            //add user to account members (user is now a member of the account from which it was created).
            //
            if (res.Code == 200 && string.IsNullOrWhiteSpace(n.AccountUUID) == false)
            {
                ac.AddUserToAccount(n.AccountUUID, n.UUID, CurrentUser);
            }

            return res;
        }

        [ApiAuthorizationRequired(Operator = ">=", RoleWeight = 3)]
        [HttpPost]
        [Route("api/Users/Add")]
        [Route("api/Users/Insert")]
        public ServiceResult Insert(User n)
        {
            if (n == null)
                return ServiceResponse.Error("No user sent.");

            AccountManager ac = new AccountManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);
            UserManager userManager = new UserManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);

            ServiceResult res =  userManager.Insert(n, GetClientIpAddress(Request));

            if (res.Code != 200)
                return res;

            //add user to account members (user is now a member of the account from which it was created).
            //
            if (string.IsNullOrWhiteSpace(n.AccountUUID) == false)
            {
               res = ac.AddUserToAccount(n.AccountUUID, n.UUID, CurrentUser);
            }
            if (res.Code != 200)
                return res;
            res.Result = n.UUID;
            return res;
        }
        //{"Message":"An error has occurred.","ExceptionMessage":"Object reference not set to an instance of an object.",
        //"ExceptionType":"System.NullReferenceException","StackTrace":"   
        //at TreeMon.Web.Filters.ApiAuthorizationRequiredAttribute.OnActionExecuting(HttpActionContext actionContext)\r\n  
        //at System.Web.Http.Filters.ActionFilterAttribute.OnActionExecutingAsync(HttpActionContext actionContext, CancellationToken cancellationToken)\r\n--- 
        //End of stack trace from previous location where exception was thrown ---\r\n  
        //at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task task)\r\n   
        //at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task task)\r\n  
        //at System.Web.Http.Filters.ActionFilterAttribute.<ExecuteActionFilterAsyncCore>d__0.MoveNext()\r\n
        //--- End of stack trace from previous location where exception was thrown ---\r\n  
        //at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task task)\r\n  
        //at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task task)\r\n  
        //at System.Web.Http.Controllers.ActionFilterResult.<ExecuteAsync>d__2.MoveNext()\r\n
        //--- End of stack trace from previous location where exception was thrown ---\r\n  
        //at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task task)\r\n  
        //at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task task)\r\n 
        //at System.Web.Http.Dispatcher.HttpControllerDispatcher.<SendAsync>d__1.MoveNext()"}

        [ApiAuthorizationRequired(Operator = ">=", RoleWeight = 1)]
        [HttpGet]
        [Route("api/UsersBy/{uuid}")]
        public ServiceResult GetBy(string uuid)
        {
            try
            {
                UserManager userManager = new UserManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);

                User u = (User)userManager.GetBy(uuid, true);

                if (u == null)
                    return ServiceResponse.Error("User not found.");

                return ServiceResponse.OK("", u);
            }
            catch (Exception ex)
            {
                SystemLogger logger = new SystemLogger(Globals.DBConnectionKey);
                logger.InsertError(ex.Message, "UsersController", "GetBy");
            }
            return ServiceResponse.Error();
        }

        [ApiAuthorizationRequired(Operator = ">=", RoleWeight = 1)]
        [HttpGet]
        [Route("api/Users/{name}")]
        public ServiceResult Get(string name)
        {
            UserManager userManager = new UserManager(Globals.DBConnectionKey, Request.Headers?.Authorization?.Parameter);

            User u = (User)userManager.Get(name, false);

            if (u == null)
                return ServiceResponse.Error("User not found.");

            u = userManager.ClearSensitiveData(u);

            return ServiceResponse.OK("", u);
        }

        [ApiAuthorizationRequired(Operator =">=" , RoleWeight = 4)]
        [HttpPost]
        [HttpDelete]
        [Route("api/Users/Delete/{UUID}")]
        public ServiceResult Delete(string UUID)
        {
            if (string.IsNullOrWhiteSpace(UUID))
                return ServiceResponse.Error("No UUID sent.");

            UserManager userManager = new UserManager(Globals.DBConnectionKey,Request.Headers?.Authorization?.Parameter);

            ServiceResult delResult = userManager.Delete(UUID);

            if (delResult.Code != 200)
                return delResult;

            AccountManager ac = new AccountManager(Globals.DBConnectionKey,Request.Headers?.Authorization?.Parameter);
            ac.RemoveUserFromAllAccounts(UUID);
            return ServiceResponse.OK();
        }

        [ApiAuthorizationRequired(Operator =">=" , RoleWeight = 4)]
        [HttpPost]
        [HttpDelete]
        [Route("api/Users/Delete")]
        public ServiceResult Delete(User n)
        {
            if (n == null || string.IsNullOrWhiteSpace(n.UUID) )
                return ServiceResponse.Error("Invalid account was sent.");

            UserManager userManager = new UserManager(Globals.DBConnectionKey,Request.Headers?.Authorization?.Parameter);

            ServiceResult delResult = userManager.Delete(n.UUID);

            if (delResult.Code != 200)
                return delResult;

            AccountManager ac = new AccountManager(Globals.DBConnectionKey,Request.Headers?.Authorization?.Parameter);
            return ac.RemoveUserFromAllAccounts(n.UUID);
        }

        /// <summary>
        /// NOTE: This is account specific.
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="startIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="sorting"></param>
        /// <returns></returns>
        [ApiAuthorizationRequired(Operator = ">=", RoleWeight = 1)]
        [HttpPost]
        [HttpGet]
        [Route("api/Users/")]
        public ServiceResult GetUsers(string filter = "")
        {
            AccountManager ac = new AccountManager(Globals.DBConnectionKey,Request.Headers?.Authorization?.Parameter);
            List<dynamic> users = (List<dynamic>)ac.GetAccountMembers(this.GetUser(Request.Headers?.Authorization?.Parameter).AccountUUID).Cast<dynamic>().ToList();
            int count;

                            DataFilter tmpFilter = this.GetFilter(filter);
                users = FilterEx.FilterInput(users, tmpFilter, out count);

            return ServiceResponse.OK("",users, count);
        }

        [ApiAuthorizationRequired(Operator = ">=", RoleWeight = 3)]
        [HttpPost]
        [HttpGet]
        [Route("api/Users/{userUUID}/Flag/{userFlag}/Value/{flagValue}")]
        public ServiceResult SetUserFlag(string userUUID, string userFlag, string flagValue)
        {
            //pass in flags..
            //ban, liftBan
            //lock, unlock
            //put swith code in user manager 

            UserManager um = new UserManager(Globals.DBConnectionKey,Request.Headers?.Authorization?.Parameter);

            //this.Get(Request.Headers?.Authorization?.Parameter)

            //u.Banned = !u.Banned;

            //u.LastLockoutDate = DateTime.UtcNow;
            //u.LockedOut = !u.LockedOut;

            //UserQueries.Update<User>(u);

            //string status = UserFlag.ClientStatus.Ban;
            return ServiceResponse.Error("NOT IMPLEMENTED!");

        }

        /// <summary>
        /// Updated fields
        /// Name         
        /// Private      
        /// SortOrder    
        /// Active       
        /// LicenseNumber
        /// Anonymous    
        /// Approved     
        /// Banned       
        /// Deleted      
        /// LockedOut    
        /// Email
        /// </summary>
        /// <param name="u"></param>
        /// <returns></returns>
        [ApiAuthorizationRequired(Operator =">=" , RoleWeight = 4)]
        [HttpPost]
        [HttpPatch]
        [Route("api/Users/Update")]
        public ServiceResult Update(UserForm u)
        {
            if(u == null)
                return ServiceResponse.Error("Invalid user form sent.");

            UserSession session = SessionManager.GetSession(Request.Headers?.Authorization?.Parameter);
            if (session == null)
                return ServiceResponse.Error("User session has timed out. You must login to complete this action.");
 
            UserManager userManager = new UserManager(Globals.DBConnectionKey,Request.Headers?.Authorization?.Parameter);

            User dbAcct = (User)userManager.GetBy(u.UUID, false);

            if (dbAcct == null)
                return ServiceResponse.Error("User was not found.");
            

            dbAcct.Name             = u.Name;
            dbAcct.AccountUUID = u.AccountUUID;
            dbAcct.Private          = u.Private;
            dbAcct.SortOrder         = u.SortOrder;
            dbAcct.Active           = u.Active;
            dbAcct.LicenseNumber    = u.LicenseNumber;
            dbAcct.Anonymous         = u.Anonymous;
            dbAcct.Approved         = u.Approved;
            dbAcct.Banned           = u.Banned;
            dbAcct.Deleted           = u.Deleted;
            dbAcct.LockedOut         = u.LockedOut;
            dbAcct.Email             = u.Email;
            dbAcct.PasswordQuestion = u.PasswordQuestion;
            dbAcct.PasswordAnswer = u.PasswordAnswer;
            //ParentId 
            //UUIDType 
            //UUParentID
            //UUParentID
            //Status 


            ServiceResult updateResult = userManager.Update(dbAcct);
            if (updateResult.Code == 200 && CurrentUser.UUID == dbAcct.UUID  )
            {
                //update session
                session.UserData = JsonConvert.SerializeObject(userManager.ClearSensitiveData(dbAcct));
                SessionManager.Update(session);
                return ServiceResponse.OK();
            }
            return updateResult;
        }


        [ApiAuthorizationRequired(Operator = ">=", RoleWeight = 1)]
        [HttpPost]
        [HttpPatch]
        [Route("api/Users/Profile/Update")]
        public ServiceResult UpdateProfile(ProfileForm p)
        {
            if(p == null)
                return ServiceResponse.Error("Invalid form sent to server.");

            UserManager userManager = new UserManager(Globals.DBConnectionKey,Request.Headers?.Authorization?.Parameter);

            TreeMon.Models.Membership.Profile dbProfile = userManager.GetCurrentProfile(p.UserUUID);
            if (dbProfile == null)
                return ServiceResponse.Error("Profile not found.");

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<ProfileForm, TreeMon.Models.Membership.Profile>();
            });

            IMapper mapper = config.CreateMapper();
            var dest = mapper.Map<ProfileForm, TreeMon.Models.Membership.Profile>(p);

            return userManager.LogUserProfile(dest);
        }

        [ApiAuthorizationRequired(Operator = ">=", RoleWeight = 1)]
        [HttpGet]
        [Route("api/Users/Profile")]
        public ServiceResult GetUserProfile()
        {
            UserManager userManager = new UserManager(Globals.DBConnectionKey,Request.Headers?.Authorization?.Parameter);

            TreeMon.Models.Membership.Profile profile =userManager.GetCurrentProfile(GetUser(Request.Headers?.Authorization?.Parameter)?.UUID);

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap< TreeMon.Models.Membership.Profile, ProfileForm>();
            });

            IMapper mapper = config.CreateMapper();
            var p = mapper.Map< TreeMon.Models.Membership.Profile, ProfileForm>(profile);

            if (p == null)
            {
                p = new ProfileForm();
                p.UserUUID = CurrentUser.UUID;
                p.AccountUUID = CurrentUser.AccountUUID;
            }
      
            return ServiceResponse.OK("",p);
        }

     
        [EnableThrottling(PerHour = 5, PerDay = 20)]
        [HttpPost]
        [Route("api/Users/SendMessageAsync/")]
        public async Task<ServiceResult> SendMessageAsync()
        {
            if (CurrentUser == null)
                return ServiceResponse.Error("You must be logged in to access this function.");


            string emailTo = "";
            try
            {
                string body = await ActionContext.Request.Content.ReadAsStringAsync();

                if (string.IsNullOrEmpty(body))
                    return ServiceResponse.Error("You must send valid email info.");

                dynamic formData = JsonConvert.DeserializeObject<dynamic>(body);

                if (formData == null)
                    return ServiceResponse.Error("Invalid email info.");

                if( formData.MessageType == "ContactAdmin")
                {
                    if ( string.IsNullOrWhiteSpace(Globals.Application.AppSetting("SiteEmail","") ))
                    {
                        return ServiceResponse.Error("Site email is not set.");
                    }
                    emailTo = Globals.Application.AppSetting("SiteEmail", "");
                }

                EmailSettings settings = new EmailSettings();
                settings.EncryptionKey = Globals.Application.AppSetting("AppKey");
                settings.HostPassword =  Globals.Application.AppSetting("EmailHostPassword");
                settings.HostUser = Globals.Application.AppSetting("EmailHostUser");
                settings.MailHost = Globals.Application.AppSetting("MailHost");
                settings.MailPort = StringEx.ConvertTo<int>(Globals.Application.AppSetting("MailPort"));
                settings.SiteDomain = Globals.Application.AppSetting("SiteDomain");
                settings.SiteEmail = Globals.Application.AppSetting("SiteEmail");
                settings.UseSSL = StringEx.ConvertTo<bool>(Globals.Application.AppSetting("UseSSL"));

                string ip = network.GetClientIpAddress(this.Request);
                string Subject = formData.Subject.ToString();
                string msg = formData.Message.ToString();
                UserManager userManager = new UserManager(Globals.DBConnectionKey,Request.Headers?.Authorization?.Parameter);

                return await  userManager.SendEmailAsync(ip, emailTo, CurrentUser.Email, Subject, msg, settings);
            }
            catch (Exception ex)
            {
                Debug.Assert(false, ex.Message);
                return ServiceResponse.Error(ex.Message);
            }
        }


        /// <summary>
        ////This occures after the registration on the page, the user recieves an email, and clicks a link to validate
        ////their email.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="operation"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        ///
        [AllowAnonymous]
        [HttpPost]
        [EnableThrottling(PerSecond = 1, PerHour = 5, PerDay = 50)]
        [Route("api/Users/Validate/type/{type}/operation/{operation}/code/{code}")]
        public ServiceResult Validate(string type = "", string operation = "", string code = "")
        {
            ServiceResult res;
       
#if DEBUG
            //fullyQualifiedUrl += string.Format(returnParams, 200, "");
            res = ServiceResponse.OK("Code validated.");
#else
             string ip = network.GetClientIpAddress(this.Request);
            UserManager userManager = new UserManager(Globals.DBConnectionKey,Request.Headers?.Authorization?.Parameter);
            res = userManager.Validate(type, operation, code, ip, CurrentUser);
#endif
            return res;
          
        }
    }
}
