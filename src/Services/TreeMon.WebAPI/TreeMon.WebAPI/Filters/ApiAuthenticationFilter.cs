// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
using System;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;


namespace TreeMon.Web.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class ApiAuthenticationFilterAttribute : AuthorizationFilterAttribute
    {
            /// <summary>
            /// Default Authentication Constructor
            /// </summary>
            public ApiAuthenticationFilterAttribute()
            {
            }

            /// <summary>
            /// AuthenticationFilter constructor with isActive parameter
            /// </summary>
            /// <param name="isActive"></param>
            public ApiAuthenticationFilterAttribute(bool isActive)                
            {
            }

            /// <summary>
            /// Protected overriden method for authorizing user
            /// </summary>
            /// <param name="username"></param>
            /// <param name="password"></param>
            /// <param name="actionContext"></param>
            /// <returns></returns>
            protected  bool OnAuthorizeUser(string username, string password, HttpActionContext actionContext)
            {
                return false;
            }
    }
}