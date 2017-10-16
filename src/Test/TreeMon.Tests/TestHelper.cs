// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using TreeMon.Data;
using TreeMon.Models.App;
using TreeMon.Models.Membership;
using System.Web.Http;

using Newtonsoft.Json;
using TreeMon.Data.Logging.Models;
using TreeMon.Managers.Membership;
using TreeMon.Managers;
using TreeMon.Web.Models;

namespace TreeMon.Web.Tests
{
    public class TestHelper
    {
        //backlog update unit tests where insert function creates user, account to reference these functions
        public static User GenerateTestUser(string name)
        {
            return new User()
            {
                UUID = Guid.NewGuid().ToString("N"),
                Name = name,
                Password = "password", //PasswordHash.ExtractHashPassword(tmpHashPassword),
                DateCreated = DateTime.UtcNow,
                Deleted = false,
                //PasswordSalt = PasswordHash.ExtractSalt(tmpHashPassword),PasswordHashIterations = PasswordHash.ExtractIterations(tmpHashPassword),
                Email = name + "@test.com",
                SiteAdmin = false,
                Approved = true,
                 AccountUUID = SystemFlag.Default.Account
            };
        }

        public static UserRegister GetUserRegister(string name)
        {
            return new UserRegister()
            {
                Name= name,
                Password = "password", 
                ConfirmPassword ="password",
                SecurityQuestion ="question",
                SecurityAnswer = "answer",
                Email = name + "@test.com"
            };
        }

        public static Account GenerateTestAccount(string name)
        {
            Account a = new Account()
            {
                UUID = Guid.NewGuid().ToString("N"),
                Name = name,
                AccountSource = "UNITTEST",
                Active = true,
                Email = name + "@unittest.com",
                DateCreated = DateTime.UtcNow

            };
            return a;
        }

        public static ServiceResult CreateUserSession(string connectionKey,  User user) {
            SessionManager m = new SessionManager(connectionKey);
            UserSession us = new UserSession()
            {
                AuthToken =  Guid.NewGuid().ToString("N"),
                Captcha = "TESTCAPTCHA",
                IsPersistent = false,
                Issued = DateTime.UtcNow,
                UserUUID = user.UUID,
                UserData = JsonConvert.SerializeObject(user),
                RoleWeight = 4,
                 Expires= DateTime.UtcNow.AddDays(2),
                  UUID = Guid.NewGuid().ToString("N")
                   
            };

            if (m.SaveSession(us) == null)
                return ServiceResponse.Error("Failed to SaveSession()");

            return ServiceResponse.OK("", us.AuthToken);
       
        }
        /// <summary>
        /// Returns authToken
        /// </summary>
        /// <returns></returns>
        public static ServiceResult InitializeControllerTestData(string connectionKey , string userRole)
        {
            ServiceResult res;
            User user = TestHelper.GenerateTestUser(Guid.NewGuid().ToString());

          
            if (userRole == "OWNER")
                user.SiteAdmin = true;

            ServiceResult sessionResult = TestHelper.CreateUserSession(connectionKey, user);

            if (sessionResult.Code != 200)
                return sessionResult;

            string sessionKey = sessionResult.Result.ToString();

            UserManager userManager = new UserManager(connectionKey, sessionKey);
            res = userManager.Insert(user);

            if (res.Code != 200)
                return res;

            RoleManager roleManager = new RoleManager(connectionKey,user);
            Role role = (Role)roleManager.Get(userRole);

            if (role == null)
            {
                //  call the function initialize roles.
                res = roleManager.InsertDefaults(SystemFlag.Default.Account, "web");
                if (res.Code != 200)
                    return res;


                role = (Role)roleManager.Get(userRole);
                if (role == null)
                    return ServiceResponse.Error("Failed to get role in InitializeControllerTestData(..)");
            }

            res = roleManager.AddUserToRole(role.UUID, user, GenerateTestUser("inituser"));
            if (res.Code != 200)
                return res;

            return sessionResult;

        }

        public static bool AccountHasPermissionsAdded( string connectionKey, string accountUUID)
        {
            TreeMonDbContext context = new TreeMonDbContext(connectionKey);
            return context.GetAll<Permission>().Where(w => w.AccountUUID == accountUUID).Count() > 0 ? true : false;
        }

        /// <summary>
        /// </summary>
        /// <param name="verb"></param>
        /// <param name="path"></param>
        /// <param name="postData"></param>
        /// <returns></returns>
        public static async Task<ServiceResult> SentHttpRequest(string verb, string path, string postData, string authToken)
        {
            //Need to force load the assembly by referencing an object in it.
            //If not you may get 404 errors.
            //
            Type controllerType = typeof(TosApiKey);
            
            ServiceResult res = new ServiceResult();
            var config = new  HttpConfiguration();//nuget Microsoft.AspNet.WebApi.Core
            //configure web api
            config.MapHttpAttributeRoutes();
            config.EnsureInitialized();
            config.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always;

            using (var server = new HttpServer(config))
            {
                var client = new HttpClient(server);
                string url = "https://localhost:44318/" + path;// "https://localhost/" + path;

                //
                StringContent tmp = new StringContent(postData, Encoding.UTF8, "application/json");

                var request = new HttpRequestMessage
                {
                    RequestUri = new Uri(url),
                    Content = new StringContent(postData, Encoding.UTF8, "application/json")
                };
                switch (verb.ToUpper()) {
                    case "GET":
                        request.Method = HttpMethod.Get;
                        break;
                    case "POST":
                        request.Method = HttpMethod.Post;
                        break;
                    case "PUT":
                        request.Method = HttpMethod.Put;
                        break;
                    case "DELETE":
                        request.Method = HttpMethod.Delete;
                        break;
                    default:
                        request.Method = HttpMethod.Get;
                        break;
                }

                request.Headers.Authorization = new AuthenticationHeaderValue("Authorization", authToken);
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // using (HttpResponseMessage response = client.SendAsync(request).Result)
                using (var response = await client.SendAsync(request))
                {
                    if (HttpStatusCode.OK != response.StatusCode)
                    {
                        string reason = response.ReasonPhrase;
                        return ServiceResponse.Error(reason);
                    }
                    string content = response.Content.ReadAsStringAsync().Result;
                    if (string.IsNullOrWhiteSpace(content))
                        return ServiceResponse.Error("No data was returned from server.");

                    try
                    {
                        res = JsonConvert.DeserializeObject<ServiceResult>(content);
                    }
                    catch
                    {
                        return ServiceResponse.Error(content);
                    }

                    if(res == null)
                        return ServiceResponse.Error("Error deserializing response.");
                    //Assert.IsInstanceOfType(rsp, typeof(ServiceResult));
                }
            }
            return res;
        }
    }
}
